// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using System.Windows;
using System.Xaml;
using System.Xml;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Markup;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.MethodTests.SdxCore
{
    /// <summary>
    /// XamlLineInfo Test
    /// </summary>
    public static class XamlLineInfoTest
    {
        /// <summary> current Element </summary>
        private static XmlElement s_currentElement;

        /// <summary> InfosetProcessor element </summary>
        private static InfosetProcessor s_ip;

        /// <summary> Xaml Reader </summary>
        private static XamlReader s_reader;

        /// <summary> result Xml Doc </summary>
        private static XmlDocument s_resultDoc;

        /// <summary>
        /// This test loads the specified xaml file and serialized each SO/SP with its line info
        /// to XML.  It then compared the generated xml with a master file.
        /// </summary>
        public static void RunXamlLineInfoTest()
        {
            FrameworkElement fe = new FrameworkElement();
            fe = null;

            string xamlFile = DriverState.DriverParameters["TestParams"] + ".xaml";
            string xmlFile = DriverState.DriverParameters["TestParams"] + ".xml";
            XmlReader xmlReader = XmlReader.Create(xamlFile);
            XamlXmlReaderSettings settings = new XamlXmlReaderSettings();
            settings.ProvideLineInfo = true;
            System.Xaml.XamlXmlReader xtr = new System.Xaml.XamlXmlReader(xmlReader, settings);
            s_reader = xtr;
            s_ip = new InfosetProcessor(xtr.SchemaContext);
            s_ip.WriteObjectDelegate = CheckWriteObject;
            s_ip.WriteMemberDelegate = CheckWriteMember;

            s_resultDoc = new XmlDocument();
            s_resultDoc.AppendChild(s_resultDoc.CreateElement("Root"));
            s_currentElement = s_resultDoc.DocumentElement;

            GlobalLog.LogStatus("Loading: " + xamlFile);
            XamlServices.Transform(xtr, s_ip);

            GlobalLog.LogStatus("Loading: " + xmlFile);
            XmlDocument loadedDoc = new XmlDocument();
            loadedDoc.Load(xmlFile);

            XmlCompareResult result = XamlComparer.Compare(loadedDoc, s_resultDoc, null);
            if (result.Result == CompareResult.Different)
            {
                GlobalLog.LogEvidence("The Generated xml tree did not match the loaded one. Saving file");
                XmlTextWriter writer = new XmlTextWriter("Output_" + xmlFile, Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                s_resultDoc.Save(writer);
                writer.Flush();
                writer.Close();
                GlobalLog.LogFile("Output_" + xmlFile);
                TestLog.Current.Result = TestResult.Fail;
            }
            else
            {
                GlobalLog.LogStatus("Trees matched");
                TestLog.Current.Result = TestResult.Pass;
            }
        }

        /// <summary>
        /// Checks the write object.
        /// </summary>
        /// <param name="xamlType">Type of the xaml.</param>
        /// <param name="context">The context.</param>
        public static void CheckWriteObject(XamlType xamlType, XamlSchemaContext context)
        {
            if (s_reader.NodeType == XamlNodeType.GetObject)
            {
                return;
            }

            XmlElement element = s_resultDoc.CreateElement("Element");
            element.Attributes.Append(s_resultDoc.CreateAttribute("Name"));
            element.Attributes.Append(s_resultDoc.CreateAttribute("LineNumber"));
            element.Attributes.Append(s_resultDoc.CreateAttribute("LinePosition"));

            element.SetAttribute("Name", xamlType.Name);
            element.SetAttribute("LineNumber", s_ip.CurrentLineNumber.ToString());
            element.SetAttribute("LinePosition", s_ip.CurrentLinePosition.ToString());

            s_currentElement.AppendChild(element);
        }

        /// <summary>
        /// Checks the write member.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="context">The context.</param>
        public static void CheckWriteMember(XamlMember property, XamlSchemaContext context)
        {
            if (XamlTestHelper.IsImplicit(property))
            {
                return;
            }

            XmlElement element;
            if (property.IsDirective)
            {
                element = s_resultDoc.CreateElement("Directive");
            }
            else
            {
                element = s_resultDoc.CreateElement("Property");
            }

            element.Attributes.Append(s_resultDoc.CreateAttribute("Name"));
            element.Attributes.Append(s_resultDoc.CreateAttribute("LineNumber"));
            element.Attributes.Append(s_resultDoc.CreateAttribute("LinePosition"));

            element.SetAttribute("Name", property.Name);
            element.SetAttribute("LineNumber", s_ip.CurrentLineNumber.ToString());
            element.SetAttribute("LinePosition", s_ip.CurrentLinePosition.ToString());

            s_currentElement.AppendChild(element);
        }
    }
}
