// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using System.Xml;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Markup;
using Microsoft.Test.Serialization;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
using Microsoft.Test.Windows;

namespace Avalon.Test.CoreUI.Integration.TypeActions
{
    /******************************************************************************
    * CLASS:          StyleActionRunner
    ******************************************************************************/
    /// <summary>
    /// Implements routines for testing Avalon Style in Style, all types and properties.
    ///</summary>
    [Test(1, "Integration.Style", "StyleActionRunner", SecurityLevel=TestCaseSecurityLevel.FullTrust, SupportFiles=@"FeatureTests\ElementServices\ActionForTypeExclude.xml,Common\PropertiesToSkip.xml,FeatureTests\ElementServices\PropertyTriggerActionForType_empty.xaml,FeatureTests\ElementServices\IntegrationXamlSnippets.xaml,FeatureTests\ElementServices\MouseActionIntegrationXamlSnippets.xaml,FeatureTests\ElementServices\ActionForType_PropertiesToSkip.xml,FeatureTests\ElementServices\StyleActionForType_empty.xaml")]
    public class StyleActionRunner : WindowTest
    {
        #region Private Data
        private static XamlTestDocument                 s_xamlTestDocument        = null;
        private static string                           s_typeName                = "";
        #endregion


        #region Constructor

        [Variation("AdornerDecorator")]
        [Variation("BlockUIContainer")]
        [Variation("Bold")]
        [Variation("Border")]
        [Variation("Button")]
        [Variation("Canvas")]
        [Variation("CheckBox")]
        [Variation("ColumnDefinition")]
        // [DISABLE WHILE PORTING]
        // [Variation("ComboBox", Versions = "3.0SP1,3.0SP2,AH")]  // Failing regularly on only 4.X, disabled there. 
        [Variation("ComboBoxItem")]
        [Variation("ContentControl")]
        [Variation("ContentPresenter")]
        // [Variation("ContextMenu")] // [DISABLE WHILE PORTING]
        [Variation("Control")]
        [Variation("Decorator")]
        [Variation("DockPanel")]
        [Variation("DocumentPageView")]
        [Variation("DocumentReference")]
        [Variation("DocumentViewer")]
        [Variation("Ellipse")]
        [Variation("Expander")]
        [Variation("Figure")]
        [Variation("FixedDocument")]
        [Variation("FixedDocumentSequence")]
        [Variation("FixedPage")]
        [Variation("Floater")]
        [Variation("FlowDocument")]
        [Variation("FlowDocumentPageViewer")]
        [Variation("FlowDocumentReader")]
        [Variation("FlowDocumentScrollViewer")]
        [Variation("Frame")]
        [Variation("FrameworkContentElement")]
        [Variation("FrameworkElement")]
        [Variation("Glyphs")]
        [Variation("Grid")]
        [Variation("GridSplitter")]
        [Variation("GridViewColumnHeader")]
        [Variation("GroupBox")]
        [Variation("GroupItem")]
        [Variation("HeaderedContentControl")]
        [Variation("HeaderedItemsControl")]
        [Variation("Hyperlink")]
        [Variation("Image")]
        [Variation("InkCanvas")]
        [Variation("InkPresenter")]
        [Variation("InlineUIContainer")]
        [Variation("Italic")]
        [Variation("ItemsControl")]
        [Variation("ItemsPresenter")]
        [Variation("Label")]
        [Variation("Line")]
        [Variation("LineBreak")]
        [Variation("List")]
        [Variation("ListBox")]
        [Variation("ListBoxItem")]
        [Variation("ListItem")]
        [Variation("ListView")]
        [Variation("ListViewItem")]
        // [DISABLE WHILE PORTING]
        // [Variation("Menu")]
        // [Variation("MenuItem")]
        [Variation("PageContent")]
        [Variation("Paragraph")]
        [Variation("PasswordBox")]
        [Variation("Path")]
        [Variation("Polygon")]
        [Variation("Polyline")]
        [Variation("Popup")]
        [Variation("ProgressBar")]
        [Variation("Rectangle")]
        [Variation("RadioButton")]
        [Variation("RepeatButton")]
        [Variation("ResizeGrip")]
        [Variation("RichTextBox", Versions = "3.0SP1,3.0SP2,AH")]  // Failing regularly on only 4.X, disabled there. 
        [Variation("RowDefinition")]
        [Variation("Run")]
        [Variation("Section")]
        [Variation("Separator")]
        [Variation("ScrollBar")]
        [Variation("ScrollViewer")]
        [Variation("Section")]
        [Variation("Separator")]
        [Variation("Slider")]
        [Variation("Span")]
        [Variation("StackPanel")]
        // [DISABLE WHILE PORTING]
        // [Variation("StatusBar")]
        [Variation("StatusBarItem")]
        [Variation("TabControl")]
        [Variation("TabItem")]
        [Variation("Table")]
        [Variation("TableCell")]
        [Variation("TableColumn")]
        [Variation("TableRow")]
        [Variation("TableRowGroup")]
        [Variation("TabPanel")]
        [Variation("TextBlock")]
        [Variation("TextBox", Versions = "3.0SP1,3.0SP2,AH")]  // Failing regularly on only 4.X, disabled there. 
        [Variation("Thumb")]
        [Variation("TickBar")]
        [Variation("ToggleButton")]
        [Variation("ToolBarOverflowPanel")]
        [Variation("ToolBar")]
        [Variation("ToolBarPanel")]
        [Variation("ToolBarTray")]
        [Variation("ToolTip")]
        [Variation("Track")]
        [Variation("TreeView")]
        [Variation("TreeViewItem")]
        [Variation("Underline")]
        [Variation("UniformGrid")]
        [Variation("UserControl")]
        [Variation("Viewbox")]
        [Variation("Viewport3D")]
        [Variation("VirtualizingStackPanel")]
        [Variation("WebBrowser")]
        [Variation("WrapPanel")]

        /******************************************************************************
        * Function:          StyleActionRunner Constructor
        ******************************************************************************/
        public StyleActionRunner(string arg)
        {
            s_typeName = arg;

            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(DoStyleTest);
        }
        public StyleActionRunner() { }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Initialize.
        /// </summary>
        /// <returns>A TestResult</returns>
        TestResult Initialize()
        {
            s_typeName = ActionHelper.GetFullName(s_typeName);
            ActionHelper.XamlFileName = "__StyleActionForType.xaml";
            ActionHelper.EmptyFileName= "StyleActionForType_empty.xaml";
            s_xamlTestDocument = ActionHelper.TestDocument;

            // Set culture to 'en-us' so round-tripping will work correctly.
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfoByIetfLanguageTag("en-us");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag("en-us");

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          DoStyleTest
        ******************************************************************************/
        /// <summary>
        /// Builds xaml that styles an instance of the type, loads it,
        /// and verifies that the instance's property values come from the style correctly.
        /// </summary>
        TestResult DoStyleTest()
        {
            BuildStyleXaml(s_typeName, s_xamlTestDocument);
            GlobalLog.LogFile(ActionHelper.XamlFileName);

            //
            // Parse xaml.
            // Diplay tree.
            // Verify tree. 
            //
            // Verification routine is called automatically when the tree
            // is rendered.
            //
            object root = SerializationHelper.ParseXamlFile(ActionHelper.XamlFileName);

            SerializationHelper helper = new SerializationHelper();
            helper.DisplayTree(root, "Style", true);

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          GetStylableProperties
        ******************************************************************************/
        /// <summary>
        /// Returns list of properties that are stylable on the given type.
        /// </summary>
        private static List<DependencyPropertyDescriptor> GetStylableProperties(string typeName)
        {
            Type type = ActionHelper.GetType(typeName, true);
            List<DependencyPropertyDescriptor> properties = new List<DependencyPropertyDescriptor>();

            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(type))
            {
                DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(prop);

                if(ActionHelper.IsStylableProperty(dpd))
                {
                    properties.Add(dpd);
                }
            }

            return properties;
        }

        /******************************************************************************
        * Function:          BuildStyleXaml
        ******************************************************************************/
        /// <summary>
        /// Builds a new xaml file for the type to use for loading and verification.
        /// The xaml file contains 2 instance declarations: 
        ///      - StyledElement - has a Style set via property element syntax, i.e. <Foo.Style>
        ///      - UnstyledElement - has locally-set properties and no Style.
        /// </summary>
        private void BuildStyleXaml(string typeName, XamlTestDocument doc)
        {
            Type type = ActionHelper.GetType(typeName, true);
            string styleKey = "StyleFor" + type.Name;

            XmlElement testRoot = doc.TestRoot;

            XmlNode testRootNode = null;
            XmlElement containerSnippet = doc.GetContainerNode(type);
            
            // Insert styled element.
            testRootNode = doc.SelectSingleNode("//*[@Name='RootForStyledElement']", ActionHelper.NamespaceManager);
            XmlElement containerNode1 = (XmlElement)doc.ImportNode(containerSnippet, true);
            testRootNode.AppendChild(containerNode1);
            containerNode1 = (XmlElement)containerNode1.SelectSingleNode("//*[@Name='ContainerElement']", ActionHelper.NamespaceManager);
            containerNode1.RemoveAttribute("Name");

            XmlElement styledElement = doc.CreateElement("", type.Name, testRootNode.NamespaceURI);
            styledElement.SetAttribute("Name", "StyledElement");
            containerNode1.AppendChild(styledElement);

            // Insert unstyled element.
            testRootNode = doc.SelectSingleNode("//*[@Name='RootForUnstyledElement']", ActionHelper.NamespaceManager);
            XmlElement containerNode2 = (XmlElement)doc.ImportNode(containerSnippet, true);
            testRootNode.AppendChild(containerNode2);
            containerNode2 = (XmlElement)containerNode2.SelectSingleNode("//*[@Name='ContainerElement']", ActionHelper.NamespaceManager);
            containerNode2.RemoveAttribute("Name");

            XmlElement unstyledElement = doc.CreateElement("", type.Name, testRootNode.NamespaceURI);
            unstyledElement.SetAttribute("Name", "UnstyledElement");
            containerNode2.AppendChild(unstyledElement);

            //
            // Create Style section.
            // Set Style on StyledElement using property element syntax.
            //
            XmlElement styleElement = doc.CreateElement("", "Style", testRootNode.NamespaceURI);

            // Set TargetType attribute on Style.
            styleElement.SetAttribute("TargetType", "{x:Type " + type.Name + "}");

            List<DependencyPropertyDescriptor> properties = GetStylableProperties(typeName);

            foreach (DependencyPropertyDescriptor dpd in properties)
            {
                XmlElement setterElement = doc.CreateElement("", "Setter", testRootNode.NamespaceURI);

                string propName = dpd.Name;
                if (dpd.IsAttached)
                {
                    propName = dpd.DependencyProperty.OwnerType.Name + "." + dpd.Name;
                }

                setterElement.SetAttribute("Property", propName);


                object value = ActionHelper.GetPropertyValue(doc, type.Name, dpd);


                if (value == null)
                {
                    setterElement.SetAttribute("Value", "{x:Null}");
                }
                else if (value is string || value.GetType().IsValueType)
                {
                    setterElement.SetAttribute("Value", value.ToString());
                }
                else
                {
                    XmlElement valueElement = doc.CreateElement("", "Setter.Value", testRootNode.NamespaceURI);
                        
                    if (value is XmlNode)
                    {
                        valueElement.PrependChild(doc.ImportNode(((XmlNode)value), true));
                    }
                    else
                    {
                        valueElement.InnerXml = SerializationHelper.SerializeObjectTree(value);
                    }

                    setterElement.PrependChild(valueElement);
                }

                styleElement.AppendChild(setterElement);
            }

            XmlElement styleComplexProp = doc.CreateElement("", type.Name + ".Style", testRootNode.NamespaceURI);
            styleComplexProp.PrependChild(styleElement);
            styledElement.PrependChild(styleComplexProp);

            //
            // Set styled properties locally on UnstyledElement.
            //
            XmlNodeList setterNodes = styleElement.SelectNodes("./av:Setter", ActionHelper.NamespaceManager);
            foreach (XmlNode node in setterNodes)
            {
                XmlElement setterElement = (XmlElement)node;
                string propName = setterElement.GetAttribute("Property");
                string[] propParts = propName.Split('.');

                if (setterElement.FirstChild == null)
                {
                    if (propParts.Length > 1 && 0 == String.Compare(propParts[0], type.Name, StringComparison.InvariantCulture))
                    {
                        propName = propParts[1];
                    }

                    unstyledElement.SetAttribute(propName, setterElement.GetAttribute("Value"));
                }
                else
                {
                    XmlNode valueNode = setterElement.FirstChild.FirstChild.Clone();

                    if (propParts.Length == 1)
                    {
                        propName = type.Name + "." + propName;
                    }

                    XmlElement complexProp = doc.CreateElement("", propName, testRootNode.NamespaceURI);
                    complexProp.PrependChild(valueNode);

                    unstyledElement.AppendChild(complexProp);
                }
            }

            // Set Verifier property.
            doc.SetVerifierRoutine(this.GetType().GetMethod("VerifyStyle"));

            // Save final xml to file.
            IOHelper.SaveTextToFile(IOHelper.ConvertXmlDocumentToStream(doc), ActionHelper.XamlFileName);
        }
        #endregion


        #region Verification
        /******************************************************************************
        * Function:          VerifyStyle
        ******************************************************************************/
        /// <summary>
        /// Verifies a styled element after the first render.
        /// </summary>
        /// <param name="uielement"></param>
        public void VerifyStyle(UIElement uielement)
        {
            FrameworkElement root = (FrameworkElement)uielement;

            DependencyObject styledElement = (DependencyObject)root.FindName("StyledElement");
            DependencyObject unstyledElement = (DependencyObject)root.FindName("UnstyledElement");
            Type type = styledElement.GetType();

            // Verify Style is set.
            Style style = (Style)styledElement.GetValue(FrameworkElement.StyleProperty);
            if (style == null)
            {
                throw new Microsoft.Test.TestValidationException("'" + type.Name + "' element's Style property is null.");
            }

            //
            // Loop through every stylable property.
            // - check property is set from style
            // - check property has expected value, compare to locally-set property
            //
            List<DependencyPropertyDescriptor> properties = GetStylableProperties(type.FullName);
            
            foreach (DependencyPropertyDescriptor dpd in properties)
            {
                if (ActionForTypeHelper.ShouldIgnoreProperty(dpd.Name, type, ActionHelper.PropertiesToSkip))
                    continue;

                GlobalLog.LogStatus("Check " + dpd.Name);

                // Check property is set from Style.
                ValueSource valueSource = DependencyPropertyHelper.GetValueSource(styledElement, dpd.DependencyProperty);
                if (valueSource.BaseValueSource != BaseValueSource.Style)
                {
                    throw new Microsoft.Test.TestValidationException("'" + type.Name + "' element's '" + dpd.Name + "' property value does not come from the Style.");
                }

                // Check property value is equivalent to locally-set property
                // on unstyled element.
                ActionForTypeHelper.CheckExpectedPropertyValue(styledElement, unstyledElement, dpd.DependencyProperty, ActionHelper.PropertiesToSkip);
            }
        }
        #endregion
    }
}
