// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Reflection;
using System.Xml;

namespace DRT
{   
    // TextView DRT.   
    internal class DrtEditing : DrtBase
    {
        // Application entry point.   
        [STAThread]
        internal static int Main(string[] args)
        {
            DrtBase drt = new DrtEditing();
            return drt.Run(args);
        }

        
        // Constructor.
        static DrtEditing()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(System.Windows.FrameworkElement));

            Type textRangeType = assembly.GetType("System.Windows.Documents.TextRange", /*ingoreCase:*/false);
            PropertyInfo xmlPropertyInfo = textRangeType.GetProperty("Xml", BindingFlags.NonPublic | BindingFlags.Instance);
            if (xmlPropertyInfo == null)
            {
                throw new Exception("TextRange.Xml property is not accessible");
            }
            s_reflectionTextRange_GetXmlMethodInfo = xmlPropertyInfo.GetGetMethod(true/*nonPublic*/);
            if (s_reflectionTextRange_GetXmlMethodInfo == null)
            {
                throw new Exception("TextEditor.IsXamlRtfConverterEnabled property set method is not accessible");
            }
            s_reflectionTextRange_SetXmlMethodInfo = xmlPropertyInfo.GetSetMethod(true/*nonPublic*/);
            if (s_reflectionTextRange_SetXmlMethodInfo == null)
            {
                throw new Exception("TextEditor.IsXamlRtfConverterEnabled property set method is not accessible");
            }
            Type textRangeBaseType = assembly.GetType("System.Windows.Documents.TextRangeBase", /*ignoreCase:*/false);
            s_normalizeRangeInfo = textRangeBaseType.GetMethod("NormalizeRange", BindingFlags.NonPublic | BindingFlags.Static);
            if (s_normalizeRangeInfo == null)
            {
                throw new Exception("TextRangeBase.Normalize method is not accessible");
            }
            Type ITextRangeType = assembly.GetType("System.Windows.Documents.ITextRange");
            Type WpfPayloadType = assembly.GetType("System.Windows.Documents.WpfPayload");
            Type textRangeSerializationType = assembly.GetType("System.Windows.Documents.TextRangeSerialization", /*ignoreCase:*/false);
            s_writeXamlInfo = textRangeSerializationType.GetMethod("WriteXaml", BindingFlags.NonPublic | BindingFlags.Static, null,
             new Type[] { typeof(XmlWriter), ITextRangeType, typeof(bool), WpfPayloadType, typeof(bool) }, null);
            if (s_writeXamlInfo == null)
            {
                throw new Exception("TextRangeSerialization.WriteXaml method is not accessible");
            }

            s_flowDocumentViewType = assembly.GetType("MS.Internal.Documents.FlowDocumentView");
            s_flowDocumentType = assembly.GetType("System.Windows.Documents.FlowDocument");
            s_flowDocument = s_flowDocumentViewType.GetProperty("Document", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic);
            s_flowDocumentContentStart = s_flowDocumentType.GetProperty("ContentStart", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic);
        }

        private DrtEditing()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(System.Windows.FrameworkElement));
            Type typeBackgroundFormatInfo = assembly.GetType("MS.Internal.PtsHost.BackgroundFormatInfo");
            FieldInfo fieldInfo = typeBackgroundFormatInfo.GetField("_isBackgroundFormatEnabled", BindingFlags.Static | BindingFlags.NonPublic);
            fieldInfo.SetValue(null, false);

            this.BlockInput = true;
            this.WindowSize = new Size(800, 600);
            this.WindowTitle = "Editing DRT";
            this.TeamContact = "Wpf";
            this.DrtName = "DRTEditing";
            this.Contact = "Microsoft";
            this.DrtName = "DrtEditing";
            this.Suites = new DrtTestSuite[] {
                new TextChangedSuite(),
                new AdornerLayerSuite(),
                new TextBoxSuite(),
                new PasswordBoxSuite(),
#if DISABLED_BY_TOM_BREAKING_CHANGE
                new TextTreeSuite(),
#endif // DISABLED_BY_TOM_BREAKING_CHANGE
                new TextElementUndoSuite(),
                new TextContainerSuite(),
                new TextRangeSerializationSuite(),
            };
        }

        
        // Internal Methods.

        // Wrapper for TextRange.get_Xml.
        internal static string GetTextRangeXml(TextRange range)
        {
            return (string)s_reflectionTextRange_GetXmlMethodInfo.Invoke(range, new object[] { });
        }

        // Wrapper for TextRange.set_Xml.
        internal static void SetTextRangeXml(TextRange range, string xml)
        {
            s_reflectionTextRange_SetXmlMethodInfo.Invoke(range, new object[] { xml });
        }

        internal static string GetTextRangeXmlWithInlines(TextRange range)
        {
            s_normalizeRangeInfo.Invoke(range, new object[] { range });
            StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
            XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
            s_writeXamlInfo.Invoke(range, new object[] { xmlWriter, range, false, null, true });
            return stringWriter.ToString();
        }

        // Wraps FlowDocument content in a standard clipboard Section.
        internal static string GetClipboardXaml(string content)
        {
            return (
                "<Section xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xml:space=\"preserve\" TextAlignment=\"Left\" LineHeight=\"Auto\" IsHyphenationEnabled=\"False\" xml:lang=\"en-us\" FlowDirection=\"LeftToRight\" NumberSubstitution.CultureSource=\"Text\" NumberSubstitution.Substitution=\"AsCulture\" FontFamily=\"Tahoma\" FontStyle=\"Normal\" FontWeight=\"Normal\" FontStretch=\"Normal\" FontSize=\"11\" Foreground=\"#FF000000\" Typography.StandardLigatures=\"True\" Typography.ContextualLigatures=\"True\" Typography.DiscretionaryLigatures=\"False\" Typography.HistoricalLigatures=\"False\" Typography.AnnotationAlternates=\"0\" Typography.ContextualAlternates=\"True\" Typography.HistoricalForms=\"False\" Typography.Kerning=\"True\" Typography.CapitalSpacing=\"False\" Typography.CaseSensitiveForms=\"False\" Typography.StylisticSet1=\"False\" Typography.StylisticSet2=\"False\" Typography.StylisticSet3=\"False\" Typography.StylisticSet4=\"False\" Typography.StylisticSet5=\"False\" Typography.StylisticSet6=\"False\" Typography.StylisticSet7=\"False\" Typography.StylisticSet8=\"False\" Typography.StylisticSet9=\"False\" Typography.StylisticSet10=\"False\" Typography.StylisticSet11=\"False\" Typography.StylisticSet12=\"False\" Typography.StylisticSet13=\"False\" Typography.StylisticSet14=\"False\" Typography.StylisticSet15=\"False\" Typography.StylisticSet16=\"False\" Typography.StylisticSet17=\"False\" Typography.StylisticSet18=\"False\" Typography.StylisticSet19=\"False\" Typography.StylisticSet20=\"False\" Typography.Fraction=\"Normal\" Typography.SlashedZero=\"False\" Typography.MathematicalGreek=\"False\" Typography.EastAsianExpertForms=\"False\" Typography.Variants=\"Normal\" Typography.Capitals=\"Normal\" Typography.NumeralStyle=\"Normal\" Typography.NumeralAlignment=\"Normal\" Typography.EastAsianWidths=\"Normal\" Typography.EastAsianLanguage=\"Normal\" Typography.StandardSwashes=\"0\" Typography.ContextualSwashes=\"0\" Typography.StylisticAlternates=\"0\">"
                + content +
                "</Section>");
        }

        internal static TextPointer TextFlowContentStart(FrameworkElement flowDocumentView)
        {
            FlowDocument document = (FlowDocument)s_flowDocument.GetValue(flowDocumentView, null);
            return (TextPointer)s_flowDocumentContentStart.GetValue(document, null);
        }

        internal static Type TextFlowType { get { return s_flowDocumentViewType; } }

        /// <summary>
        /// Verifies that given values are equal as xml trees
        /// </summary>
        /// <param name='actualValue'>
        /// Actual value.
        /// </param>
        /// <param name='expectedValue'>
        /// Expected value.
        /// </param>
        /// <param name='description'>
        /// An optional descriptive message.
        /// </param>
        internal static void AssertEqualXml(string actualValue, string expectedValue, string description)
        {
            XmlDocument actualXml = new XmlDocument();
            XmlDocument expectedXml = new XmlDocument();

            actualXml.LoadXml(actualValue);
            expectedXml.LoadXml(expectedValue);

            if (!XmlElementsEqual(actualXml.DocumentElement, expectedXml.DocumentElement))
            {
                string message = String.Format("Expected \n[{0}]\n got \n[{1}]\n {2}", expectedValue, actualValue, description);
                throw new Exception(message);
            }
        }

        
        // Private Methods.

        private static bool XmlElementsEqual(XmlElement actualElement, XmlElement expectedElement)
        {
            if (actualElement == null || expectedElement == null)
            {
                return false;
            }

            // Compare element names
            if (actualElement.NamespaceURI != expectedElement.NamespaceURI ||
                actualElement.LocalName != expectedElement.LocalName)
            {
                return false;
            }

            // Compare Attributes
            XmlAttributeCollection expectedAttributes = expectedElement.Attributes;
            if (expectedAttributes.Count != actualElement.Attributes.Count)
            {
                return false;
            }
            for (int i = 0; i < expectedAttributes.Count; i++)
            {
                XmlAttribute attribute = expectedAttributes[i];
                if (actualElement.GetAttribute(attribute.LocalName, attribute.NamespaceURI) != attribute.Value)
                {
                    return false;
                }
            }

            // Compare children
            if (expectedElement.HasChildNodes != actualElement.HasChildNodes)
            {
                return false;
            }

            if (expectedElement.HasChildNodes)
            {
                XmlNodeList expectedNodes = expectedElement.ChildNodes;
                XmlNodeList actualNodes = actualElement.ChildNodes;

                if (expectedNodes.Count != actualNodes.Count)
                {
                    return false;
                }
                for (int i = 0; i < expectedNodes.Count; i++)
                {
                    XmlNode expectedNode = expectedNodes[i];
                    XmlNode actualNode = actualNodes[i];

                    if (expectedNode.GetType() != actualNode.GetType())
                    {
                        return false;
                    }
                    if (expectedNode is XmlElement)
                    {
                        if (!XmlElementsEqual((XmlElement)expectedNode, (XmlElement)actualNode))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (expectedNode.Value != actualNode.Value)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        
        // Private Fields.
    
        private static MethodInfo s_reflectionTextRange_GetXmlMethodInfo;
        private static MethodInfo s_reflectionTextRange_SetXmlMethodInfo;
        private static MethodInfo s_normalizeRangeInfo;
        private static MethodInfo s_writeXamlInfo;
        private static Type s_flowDocumentViewType;
        private static Type s_flowDocumentType;
        private static PropertyInfo s_flowDocument;
        private static PropertyInfo s_flowDocumentContentStart;
    }
}