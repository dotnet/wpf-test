// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Schema tests.


using System;
using System.Windows;
using System.Windows.Annotations.Storage;
using System.Windows.Annotations;
using Annotations.Test.Framework;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using Proxies.MS.Internal.Annotations;
using System.Text.RegularExpressions;

namespace Avalon.Test.Annotations.Suites
{
    #region PRIORITY TESTS
    public abstract class SchemaSuite : TestSuite
    {
        #region Overrides

        [TestCase_Setup]
        protected virtual void Setup()
        {
            StoreEditor = new StoreManipulator(STORE_TEXTNOTE);
            printStatus("Setup StoreManipulator...");
        }

        [TestCase_Cleanup]
        protected override void CleanupVariation()
        {
            StoreEditor = null;
            XPath = string.Empty;
            ExpectedExceptionType = null;
        }

        public override void ProcessArgs(string[] args)
        {
            base.ProcessArgs(args);

            for (int i = 0; i < args.Length; i++)
            {
                args[i] = args[i].Replace("%20", " ");
            }

            Match match;
            Regex xpathExpression = new Regex("/xpath=(.*)");
            Regex exceptionExpression = new Regex("/exception=(.*)");
            foreach (string arg in args)
            {
                if ((match = xpathExpression.Match(arg)).Success)
                {
                    XPath = match.Groups[1].Value;
                    printStatus("XPath = '" + XPath + "'.");
                }
                else if ((match = exceptionExpression.Match(arg)).Success)
                {
                    ExpectedExceptionType = match.Groups[1].Value;
                    printStatus("Expected Exception = '" + ExpectedExceptionType + "'.");
                }
            }

            Assert("Must have parameter /xpath=XXX.", !string.IsNullOrEmpty(XPath));

            if (string.IsNullOrEmpty(ExpectedExceptionType))
                printStatus("Exception Expected = <none>");
        }

        #endregion

        #region Helper Methods

        protected void PrintStream(StoreManipulator store)
        {
            string stream = new StreamReader(store.GetStream()).ReadToEnd();
            printStatus("Store Stream:\n" + stream);
        }

        protected void VerifyStore(StoreManipulator store, string expectExceptionType)
        {
            if (string.IsNullOrEmpty(expectExceptionType))
                VerifyValidStore(store);
            else
                VerifyInvalidStore(store, expectExceptionType);                
        }

        protected void VerifyValidStore(StoreManipulator storeManipulator)
        {
            using (Stream stream = storeManipulator.GetStream())
            {
                AnnotationStore store = new XmlStreamStore(stream);
                Assert("Verify at least one annotation.", store.GetAnnotations().Count > 0);
            }
        }

        protected void VerifyInvalidStore(StoreManipulator storeManipulator, string expecectedExceptionType)
        {
            bool failed = false;
            using (Stream stream = storeManipulator.GetStream())
            {
                AnnotationStore store = new XmlStreamStore(stream);
                try
                {
                    store.GetAnnotations();
                }
                catch (Exception e)
                {
                    printStatus("Exception Occurred: [" + e.GetType().Name + "] - " + e.Message);
                    AssertEquals("Verify expected exception type.", expecectedExceptionType, e.GetType().Name);                    
                    failed = true;
                }
            }
            Assert("Verify exception occurred.", failed);
        }

        #endregion

        #region Fields

        protected string XPath;
        protected  StoreManipulator StoreEditor;
        protected string ExpectedExceptionType = null;

        public static string STORE_TEXTNOTE = "Store_TextNote.xml";

        #endregion
    }

    [TestDimension(new string[] { 
            "/xpath=//anc:Annotation[1]",
            "/xpath=//anc:Annotation[1]/anc:Authors[1]",
            "/xpath=//anc:Annotation[1]/anc:Anchors[1]",
            "/xpath=//anc:Annotation[1]/anc:Anchors[1]/anc:Resource[1]",
            "/xpath=//anc:Annotation[1]/anc:Anchors[1]/anc:Resource[1]/anc:ContentLocator[1]",
            "/xpath=//anc:Annotation[1]/anc:Anchors[1]/anc:Resource[1]/anc:ContentLocator[1]/*/anc:Item[1]",
            "/xpath=//anc:Annotation[1]/anc:Cargos[1]",
            "/xpath=//anc:Annotation[1]/anc:Cargos[1]/anc:Resource[1]",        
    })]
    public class NodeTestSuite : SchemaSuite
    {
        #region Tests

        protected void schema_unknownattribute()
        {
            StoreEditor.CreateAttribute(XPath, "anc", "Bogus", "foo");
            VerifyInvalidStore(StoreEditor, typeof(XmlException).Name);
            passTest("Exception for unknown attribute.");
        }

        protected void schema_whitespacewithattributes()
        {
            StoreEditor.InsertTextWithin(XPath, "  \r\n  ");
            VerifyValidStore(StoreEditor);
            passTest("Extra whitespace inside node ok.");
        }

        protected void schema_plaintextchild()
        {
            StoreEditor.InsertTextAsChild(XPath, "Plain text inside a node!");
            VerifyInvalidStore(StoreEditor, typeof(XmlException).Name);
            passTest("Exception for plain text inside a node.");
        }

        protected void schema_emptychild()
        {
            StoreEditor.DeleteChildren(XPath);
            VerifyValidStore(StoreEditor);
            passTest("Verified empty children.");
        }

        [OverrideClassTestDimensions]
        [TestDimension(new string[] { 
            "/xpath=//anc:Annotation[1] /exception=XmlException",
            "/xpath=//anc:Annotation[1]/anc:Authors[1] /exception=XmlException",
            "/xpath=//anc:Annotation[1]/anc:Anchors[1] /exception=XmlException",
            "/xpath=//anc:Annotation[1]/anc:Anchors[1]/anc:Resource[1]",
            "/xpath=//anc:Annotation[1]/anc:Anchors[1]/anc:Resource[1]/anc:ContentLocator[1] /exception=XmlException",
            "/xpath=//anc:Annotation[1]/anc:Cargos[1] /exception=XmlException",
            "/xpath=//anc:Annotation[1]/anc:Cargos[1]/anc:Resource[1]",
        })]
        protected void schema_unknownchild()
        {
            StoreEditor.InsertChild(XPath, "anc", "NotAValidNode", "invalid");
            VerifyStore(StoreEditor, ExpectedExceptionType);
            passTest("Verified unknown child.");
        }

        protected void schema_extranodewhitespace()
        {
            StoreEditor.InsertTextAsChild(XPath, "    \r\n  ");
            VerifyValidStore(StoreEditor);
            passTest("Extra whitespace in node definition is ok.");
        }

        #endregion
    }

    public class AttributeTestSuite : SchemaSuite
    {
        #region Overrides

        public override void ProcessArgs(string[] args)
        {
            base.ProcessArgs(args);
            
            Match match;
            Regex valueExp = new Regex("/value=(.*)");
            foreach (string arg in args)
            {
                if ((match = valueExp.Match(arg)).Success)
                {
                    AttributeValue = match.Groups[1].Value;
                }
            }

            printStatus("AttributeValue = '" + AttributeValue + "'");
        }

        protected override void CleanupVariation()
        {
            base.CleanupVariation();
            AttributeValue = string.Empty;
        }

        #endregion

        #region Tests

        /// <summary>
        /// Test result of modifying attribute values.
        /// </summary>
        [TestDimension(
        new string[] { 
            "/xpath=//anc:Annotation[1]/@Id /value=123-14 /exception=InvalidOperationException",
            "/xpath=//anc:Annotation[1]/@CreationTime /value=1:30pm /exception=FormatException",
            "/xpath=//anc:Annotation[1]/@LastModificationTime /value=2006-02-02foo /exception=FormatException",
            "/xpath=//anc:Annotation[1]/@LastModificationTime /value=2006-02-01T13:16:40.4537351-08:00",
            "/xpath=//anc:Annotation[1]/@Type /value=ImaginaryType",
            "/xpath=//anc:Annotation[1]/@Type /value=Foo:Bar",
            "/xpath=//anc:Annotation[1]/@Type /value= /exception=XmlException",
            "/xpath=//anc:Annotation[1]/@Type /value=: /exception=FormatException",
            "/xpath=//anc:Annotation[1]/@Type /value=:FooBar /exception=FormatException",
            "/xpath=//anc:Annotation[1]/@Type /value=Fo:oB:ar /exception=FormatException",
            "/xpath=//anc:Annotation[1]/anc:Anchors[1]/anc:Resource[1]/@Id /value=foo /exception=FormatException",
            "/xpath=//anc:Annotation[1]/anc:Anchors[1]/anc:Resource[1]/@Id /value=00000000-0000-0000-0000-000000000000 /exception=XmlException",
            "/xpath=//anc:Annotation[1]/anc:Anchors[1]/anc:Resource[1]/@Name /value=doesn'tmatter",
            "/xpath=//anc:Annotation[1]/anc:Anchors[1]/anc:Resource[1]/@Name /value=",
            "/xpath=//anc:Annotation[1]/anc:Cargos[1]/anc:Resource[1]/@Id /value=99985556751355 /exception=FormatException",
            "/xpath=//anc:Annotation[1]/anc:Cargos[1]/anc:Resource[1]/@Id /value=00000000-0000-0000-0000-000000000000 /exception=XmlException",
            "/xpath=//anc:Annotation[1]/anc:Cargos[1]/anc:Resource[1]/@Name /value=NoOp",
            "/xpath=//anc:Annotation[1]/anc:Cargos[1]/anc:Resource[1]/@Name /value=Meta%20Data",
            "/xpath=//anc:Annotation[1]/anc:Cargos[1]/anc:Resource[1]/@Name /value=",
        })]
        [Priority(1)]
        protected void schema_testmodifyattribute()
        {
            StoreEditor.EditAttributeValue(XPath, AttributeValue);
            VerifyStore(StoreEditor, ExpectedExceptionType);
            passTest("Verified modifying attribute value.");
        }

        /// <summary>
        /// Test result of removing attributes.
        /// </summary>
        [TestDimension(
        new string[] { 
            "/xpath=//anc:Annotation[1]/@Id /exception=XmlException",
            "/xpath=//anc:Annotation[1]/@CreationTime /exception=XmlException",
            "/xpath=//anc:Annotation[1]/@LastModificationTime /exception=XmlException",
            "/xpath=//anc:Annotation[1]/@Type /exception=XmlException",
            "/xpath=//anc:Annotation[1]/anc:Anchors[1]/anc:Resource[1]/@Id /exception=XmlException",
            "/xpath=//anc:Annotation[1]/anc:Anchors[1]/anc:Resource[1]/@Name",
            "/xpath=//anc:Annotation[1]/anc:Cargos[1]/anc:Resource[1]/@Id /exception=XmlException",
            "/xpath=//anc:Annotation[1]/anc:Cargos[1]/anc:Resource[1]/@Name",
        })]
        [Priority(1)]
        protected void schema_testmissingattribute()
        {
            StoreEditor.RemoveAttribute(XPath);
            VerifyStore(StoreEditor, ExpectedExceptionType);
            passTest("Verified removing attribute value.");
        }

        #endregion

        #region Fields

        private string AttributeValue = string.Empty;

        #endregion
    }

    public class StoreManipulator
    {
        public StoreManipulator(string source)
        {
            using (Stream stream = new FileStream(source, FileMode.Open, FileAccess.Read))
            {
                _document = new XmlDocument();
                _document.Load(stream);
                _document.PreserveWhitespace = true;
                _namespaceManager = new XmlNamespaceManager(_document.NameTable);
                _namespaceManager.AddNamespace(AnnotationXmlConstants.Prefixes.CoreSchemaPrefix, AnnotationXmlConstants.Namespaces.CoreSchemaNamespace);
                _namespaceManager.AddNamespace(AnnotationXmlConstants.Prefixes.BaseSchemaPrefix, AnnotationXmlConstants.Namespaces.BaseSchemaNamespace);
            }
        }

        public void RemoveAttribute(string xpath)
        {
            XPathNavigator node = GetAttribute(xpath);
            node.DeleteSelf();
        }

        public void EditAttributeValue(string xpath, string value)
        {
            XPathNavigator node = GetAttribute(xpath);
            node.SetValue(value);
        }

        /// <summary>
        /// Add an Attribute to the node specified by the XPath.
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="prefix"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        public void CreateAttribute(string xpath, string prefix, string attributeName, string attributeValue)
        {
            XPathNodeIterator iterator = FindNode(xpath);
            iterator.Current.CreateAttribute(prefix, attributeName, null, attributeValue);
        }

        /// <summary>
        /// Insert element as a child of the given XPath.
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="prefix"></param>
        /// <param name="elementName"></param>
        /// <param name="elementValue"></param>
        public void InsertChild(string xpath, string prefix, string elementName, string elementValue)
        {
            XPathNavigator root = FindNode(xpath).Current;
            root.AppendChildElement(prefix, elementName, null, elementValue);
        }

        public void InsertTextWithin(string xpath, string text)
        {
            XPathNavigator root = FindNode(xpath).Current;
            string xml = root.OuterXml;
            int startIdx = xml.IndexOf(root.Name);
            xml = xml.Insert(startIdx + root.Name.Length, text);
            root.OuterXml = xml;
        }

        /// <summary>
        /// Insert plain text before node specified by given XPath.
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="text"></param>
        public void InsertTextAsChild(string xpath, string text)
        {
            XPathNodeIterator iterator = FindNode(xpath);
            using (XmlWriter writer = iterator.Current.AppendChild())
            {
                writer.WriteString(text);
                writer.Flush();
            }
        }

        /// <summary>
        /// Delete node specified by XPath.
        /// </summary>
        /// <param name="xpath"></param>
        public void DeleteNode(string xpath)
        {
            XPathNodeIterator node = FindNode(xpath);
            node.Current.DeleteSelf();
        }

        /// <summary>
        /// Delete all children of the specified XPath.
        /// </summary>
        /// <param name="xpath"></param>
        public void DeleteChildren(string xpath)
        {
            XPathNavigator root = FindNode(xpath).Current;
            if (root.HasChildren)
            {
                while (root.HasChildren)
                {
                    XPathNodeIterator children = root.SelectChildren(XPathNodeType.All);
                    children.MoveNext();
                    children.Current.DeleteSelf();
                }
            }        
        }

        public Stream GetStream()
        {
            MemoryStream outStream = new MemoryStream();
            _document.Save(outStream);
            outStream.Position = 0;
            return outStream;
        }

        private XPathNodeIterator FindNode(string xpathExpression)
        {
            XPathNavigator navigator = _document.CreateNavigator();
            XPathNodeIterator iterator = navigator.Select(xpathExpression, _namespaceManager);
            if (iterator.Count == 0)
                throw new ArgumentException("Couldn't find xpath = '" + xpathExpression + "'.");
            iterator.MoveNext();
            return iterator;
        }

        private XPathNavigator GetAttribute(string xpath)
        {
            XPathNavigator node = FindNode(xpath).Current;
            if (node.NodeType != XPathNodeType.Attribute)
                throw new ArgumentException("XPath must point to an Attribute: '" + xpath + "'.");
            return node;
        }

        XmlDocument _document;
        XmlNamespaceManager _namespaceManager;
    }

    #endregion PRIORITY TESTS
}

