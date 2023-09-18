// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Tests TextElements serialization with default property values

using System;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.XPath;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

using Microsoft.Test.Diagnostics;
using Microsoft.Test;
using Microsoft.Test.Discovery;

using Test.Uis.IO;
using Test.Uis.Loggers;
using Test.Uis.Management;
using Test.Uis.TestTypes;
using Test.Uis.Utils;

namespace Microsoft.Test.Editing.Serialization
{
    /// <summary>
    /// This tests spefically tests changes introduced to fix Regression_Bug311 in Orcas and the behavior
    /// before the fix. It explicitly assigns default values (local value is set) to NonInheritable 
    /// properties on TextElements and verifies those are serialized during serialization.
    /// </summary>
    [Test(0, "TextRangeSerialization", "DefaultPropertyValueSerialization", MethodParameters = "/TestCaseType:DefaultPropertyValueSerialization", Keywords = "Localization_Suite")]
    [TestBugs("311")]
    public class DefaultPropertyValueSerialization : CustomTestCase
    {        
        /// <summary>Runs test case</summary>
        public override void RunTestCase()
        {                        
            string[] elementNames = new string[] {"Run", "Hyperlink", "Figure", "Floater",
                "Paragraph", "Section", "List", "ListItem",
                "Table", "TableColumn", "TableRowGroup", "TableRow", "TableCell"};
            
            if (SystemInformation.WpfVersion == WpfVersions.Wpf40)
            {
                Log("Testing ***4.0*** behavior");
            }
            else if (SystemInformation.WpfVersion == WpfVersions.Wpf35)
            {
                Log("Testing ***3.5*** behavior");
            }
            else if (SystemInformation.WpfVersion == WpfVersions.Wpf30)
            {
                Log("Testing ***3.0*** behavior");
            }

            foreach (string elementName in elementNames)
            {
                RichTextBox richTextBox = CreateRichTextBox(s_contentXaml);
                FrameworkContentElement contentElement = (FrameworkContentElement)LogicalTreeHelper.FindLogicalNode(richTextBox, elementName);                
                Log("\r\n***** Testing with " + contentElement.GetType().Name + " Properties *****");
                SetDefaultValues(contentElement);

                if ((SystemInformation.WpfVersion == WpfVersions.Wpf30) &&
                    (contentElement is TableColumn))
                {
                    //TableColumn is atomic element. No optimization was done in serializing 
                    //attributes for this element in RTM. So dont check for this element.
                }
                else
                {
                    VerifyPropertyValuesAfterSerialization(contentElement.GetType(), richTextBox, elementName);
                }
            }                                    

            Logger.Current.ReportSuccess();
        }

        private RichTextBox CreateRichTextBox(string contentXaml)
        {            
            StringStream stringStream = new StringStream(contentXaml);
            return (RichTextBox)XamlReader.Load(stringStream);            
        }
        
        // Gets list of non-inheritable properties defined in TextSchema which TextEditor uses to 
        // serialize the elements.
        private DependencyProperty[] GetTextSchemaNonInheritableProperties(Type elementType)
        {            
            Type textSchemaType = ReflectionUtils.FindType("TextSchema");
            return (DependencyProperty[])ReflectionUtils.InvokeStaticMethod(textSchemaType, "GetNoninheritableProperties", new object[] { elementType });            
        }

        // Returns non-null string if this value can be converted to a string, null otherwise.
        private string GetStringValue(DependencyProperty property, object propertyValue)
        {
            Type dpTypeDescriptorContextType = ReflectionUtils.FindType("DPTypeDescriptorContext");
            return (string)ReflectionUtils.InvokeStaticMethod(dpTypeDescriptorContextType, "GetStringValue", new object[] { property, propertyValue });
        }
        
        // Returns false if the value is null or NaN else returns true        
        private bool IsPropertyValueSerializable(object value)
        {
            if (value == null)
            {
                return false;
            }
            if ((value is Double) && (Double.IsNaN((double)value)))
            {
                return false;
            }
            return true;
        }                

        // Explicitly sets default values to the TextSchema non-inheritable properties of the element.        
        private void SetDefaultValues(DependencyObject element)
        {
            object defaultValue;
            DependencyProperty[] dpList = GetTextSchemaNonInheritableProperties(element.GetType());

            foreach (DependencyProperty property in dpList)
            {
                PropertyMetadata metadata = property.GetMetadata(element.GetType());
                defaultValue = metadata.DefaultValue;

                //Dont set the value if the defaultValue is null or NaN                
                if (IsPropertyValueSerializable(defaultValue))
                {
                    Log("Assigning value [" + defaultValue.ToString() + "] to the property [" + 
                        property.Name + "] for the element [" + element.GetType().Name + "]");
                    element.SetValue(property, defaultValue);
                }
                else
                {
                    Log("Skipping property [" + property.Name + "] as it doesnt have a default value");
                }
            }
        }        
        
        // Verification to check if the properties are serialized.        
        private void VerifyPropertyValuesAfterSerialization(Type elementType, RichTextBox richTextBox, string elementName)
        {
            TextRange tr = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            MemoryStream memoryStream = new MemoryStream();
            tr.Save(memoryStream, DataFormats.Xaml);                                     
            memoryStream.Seek(0, SeekOrigin.Begin);

            //Get the Xaml of the element being tested
            XmlDocument rootXmlDocument = new XmlDocument();
            rootXmlDocument.Load(memoryStream);            
            //namespace agnostic xpath format ".//*[local-name()='foo']"
            //The first * is to not include the top level Section container in the search.
            XmlElement xmlElement = (XmlElement)rootXmlDocument.SelectSingleNode("/*//*[local-name()='" + elementName + "']");
            string elementXaml = xmlElement.OuterXml;            

            //Verify for all applicable properties
            DependencyProperty[] dpList = GetTextSchemaNonInheritableProperties(elementType);
            foreach (DependencyProperty property in dpList)
            {                
                PropertyMetadata metadata = property.GetMetadata(elementType);
                object defaultValue = metadata.DefaultValue;
                               
                if (IsPropertyValueSerializable(defaultValue))            
                {
                    string expectedAttributeString = GetStringValue(property, defaultValue);                    
                    
                    Log("ElementXaml [" + elementXaml + "]");
                    Log("ExpectedAttributeString for [" + property.Name + "] property : [" + 
                        expectedAttributeString + "]");

                    if (SystemInformation.WpfVersion == WpfVersions.Wpf30)
                    {
                        Verifier.Verify(!xmlElement.HasAttribute(property.Name),
                            "Verifying that element ***DOES NOT** have attribute [" + property.Name + "]", false);
                    }
                    else // when WpfVersion is Wpf35 or Wpf40
                    {
                        Verifier.Verify(xmlElement.HasAttribute(property.Name),
                            "Verifying that element has attribute [" + property.Name + "]", false);

                        string actualPropertyValue = xmlElement.GetAttribute(property.Name);
                        int result = string.Compare(actualPropertyValue, expectedAttributeString, StringComparison.InvariantCulture);
                        Verifier.Verify(result == 0,
                            "Verifying that value [" + expectedAttributeString + "] for property [" +
                            property.Name + "] in element [" + elementType.Name + "] is serialized", true);                                                
                    }
                }
            }
        }

        //Xaml content used to test all TextElements
        private static readonly string s_contentXaml = @"
                <RichTextBox xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xml:space='default'>
                <FlowDocument>
                <Paragraph Name='Paragraph'><Run Name='Run'>Run</Run></Paragraph>
                <Paragraph><Hyperlink Name='Hyperlink' NavigateUri='http://www.live.com'>Hyperlink</Hyperlink></Paragraph>
                <Paragraph>
                    <Figure Name='Figure'><Paragraph>Figure</Paragraph></Figure>
                    <Floater Name='Floater'><Paragraph>Floater</Paragraph></Floater>
                </Paragraph>
                <Section Name='Section'><Paragraph>Section</Paragraph></Section>
                <List Name='List'><ListItem Name='ListItem'><Paragraph>List</Paragraph></ListItem></List>
                <Table Name='Table'>
                    <Table.Columns><TableColumn Name='TableColumn'/><TableColumn/></Table.Columns>
                    <TableRowGroup Name='TableRowGroup'>
                        <TableRow Name='TableRow'>
                            <TableCell Name='TableCell'><Paragraph>Table</Paragraph></TableCell>
                        </TableRow>
                    </TableRowGroup>
                </Table>
                </FlowDocument>
                </RichTextBox>                            
                ";
    }
}