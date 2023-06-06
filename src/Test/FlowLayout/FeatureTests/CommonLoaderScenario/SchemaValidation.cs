// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Xml;
using System.Collections;
using System.Windows.Markup;

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;


namespace Microsoft.Test.Layout
{
    [Test(0, "SchemaValidation", "FlowLayout SchemaValidation")]
    public class SchemaValidationTests : AvalonTest
    {
        private ushort _levels;
        private string _rootDoc;
        private bool _validDocOnly;
        private bool _failure = false;
        private XmlDocument _xmlContainmentDoc = new XmlDocument();
        private XmlDocument _newDoc = new XmlDocument();
        private XmlAttribute _xmlNS;
     
        [Variation("BlockUIContainer", 3, true)]
        [Variation("BlockUIContainer", 4, false)]
        [Variation("InlineUIContainer", 2, true)]
        [Variation("InlineUIContainer", 2, false)]
        [Variation("Paragraph", 3, true)]
        [Variation("Paragraph", 4, false)]
        [Variation("Section", 4, true)]
        [Variation("Section", 3, false)]
        [Variation("List", 4, true)]
        [Variation("List", 3, false)]
        [Variation("Table", 5, true)]
        [Variation("Table", 4, false)]
        [Variation("FlowDocument", 4, true)]
        [Variation("FlowDocument", 4, false)]
        [Variation("Figure", 3, true)]
        [Variation("Figure", 3, false)]
        [Variation("Floater", 3, true)]
        [Variation("Floater", 3, false)]
        [Variation("Hyperlink", 2, true)]
        [Variation("Hyperlink", 2, false)]
        [Variation("TextBlock", 3, true)]
        [Variation("TextBlock", 4, false)]
        [Variation("FlowDocumentScrollViewer", 4, true)]
        [Variation("FlowDocumentScrollViewer", 4, false)]
        [Variation("FlowDocumentPageViewer", 4, true)]
        [Variation("FlowDocumentPageViewer", 4, false)]
        public SchemaValidationTests(string rootDoc, ushort levels, bool validDocOnly)
            : base()
        {
            this._rootDoc = rootDoc;
            this._levels = levels;
            this._validDocOnly = validDocOnly;
            InitializeSteps += new TestStep(SchemaValidationTests_Run);
        }        

        private TestResult SchemaValidationTests_Run()
        {
            StreamReader strReaderSW = null;
            if (_validDocOnly)
            {
                strReaderSW = new StreamReader("Containment.xml");
            }
            else
            {
                strReaderSW = new StreamReader("InvalidContainment.xml");
            }

            _xmlContainmentDoc.Load(strReaderSW.BaseStream);
            XmlNode xmlSchemaNode = _xmlContainmentDoc.SelectSingleNode("descendant::" + _rootDoc);           
            _xmlNS = _newDoc.CreateAttribute("xmlns");
            _xmlNS.Value = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
            // create top level element
            XmlNode xmlRootNode = _newDoc.CreateNode(XmlNodeType.Element, xmlSchemaNode.Name, "");
            AddMandatoryChildren(xmlSchemaNode, xmlRootNode);
            CreateValidSchema(xmlRootNode, _levels);
            if (!_failure)
            {
                Log.LogStatus("No failures encountered.  Test Passed.");
                return TestResult.Pass;
            }
            else
            {
                Log.LogStatus("Encountered at least one failure.  Test Failed.");
                return TestResult.Fail;                
            }       
        }

        private void CreateValidSchema(XmlNode xmlSchemaNode, ushort level)
        {
            level--;

            if (level == 0)
            {
                VerifyValidSchema(xmlSchemaNode);
                return;
            }
            ArrayList exp = new ArrayList();
            exp = ExpandNodes(xmlSchemaNode, exp);

            if (exp.Count == 0)
            {
                // nothing below this - no expansion
                VerifyValidSchema(xmlSchemaNode);
                return;
            }
           
            foreach (XmlNode schemaNodeToAdd in exp)
            {
                XmlNode validChild;                
                if (schemaNodeToAdd.Name == "#text")
                {
                    validChild = _newDoc.CreateNode(XmlNodeType.Text, schemaNodeToAdd.Name, "");
                    validChild.Value = schemaNodeToAdd.Value;
                }
                else
                {
                    validChild = _newDoc.CreateNode(XmlNodeType.Element, schemaNodeToAdd.Name, "");
                }

                if (_validDocOnly)
                {
                    xmlSchemaNode.AppendChild(validChild);
                }
                else
                {
                    // allow only one child at a level when invalid schemas are being tested
                    xmlSchemaNode.RemoveAll();
                    xmlSchemaNode.AppendChild(validChild);
                }

                AddMandatoryChildren(schemaNodeToAdd, validChild);
                CreateValidSchema(validChild, level);
            }                        
        }

        private void AddMandatoryChildren(XmlNode schemaNodeToAdd, XmlNode validChild)
        {
            if (schemaNodeToAdd.Attributes == null)
            {
                return;
            }
            
            if (schemaNodeToAdd.Attributes.GetNamedItem("Child") != null)
            {
                ArrayList expSub = new ArrayList();
                expSub = ExpandNodes(schemaNodeToAdd, expSub);
               
                foreach (XmlNode mandatorySchemaNodeToAdd in expSub)
                {                   
                    XmlNode validChildSub = _newDoc.CreateNode(XmlNodeType.Element, mandatorySchemaNodeToAdd.Name, "");
                    validChild.AppendChild(validChildSub);
                }
            }
        }

        private ArrayList ExpandNodes(XmlNode xmlSchemaNode, ArrayList arrList)
        {
            if (xmlSchemaNode.Name == "#text")
            {
                return arrList;
            }

            xmlSchemaNode = _xmlContainmentDoc.SelectSingleNode("ContainmentSchema/" + xmlSchemaNode.Name);
            if (xmlSchemaNode == null)
            {
                return arrList;
            }

            if (!xmlSchemaNode.HasChildNodes)
            {
                return arrList;
            }

            foreach (XmlNode contentNode in xmlSchemaNode.ChildNodes)
            {
                if (contentNode.Name.ToUpper() == contentNode.Name)
                {
                    // supernode
                    arrList = ExpandNodes(contentNode, arrList);
                }
                else
                {
                    arrList.Add(contentNode);                    
                }
            }
            return arrList;
        }

        private void VerifyValidSchema(XmlNode parent)
        {
            XmlNode root = parent;
            while ((root.ParentNode != null) && (root.ParentNode != root.OwnerDocument))
            {
                root = root.ParentNode;
            }

            root.Attributes.Append(_xmlNS);
            _newDoc.RemoveAll();
            _newDoc.AppendChild(root);           
            StreamWriter strWriter = new StreamWriter("tmpXaml.xaml");            
            _newDoc.Save(strWriter.BaseStream);
            strWriter.Close();            
            StreamReader sr = new StreamReader("tmpXaml.xaml");            

            object XAMLroot = null;

            //With invalid schema tests we will get an exception when we try to parse the stream.
            //This is ok, so we catch it and move forward.
            try
            {
                XAMLroot = XamlReader.Load(sr.BaseStream);
            }
            catch (System.Windows.Markup.XamlParseException e) 
            {
                if (_validDocOnly)
                {
                    //Should not get this exception for a valid doc
                    TestLog.Current.LogEvidence("Got a XamlParseException when trying to parse a valid document!");
                    throw e;
                }
            }

            if (_validDocOnly)
            {
                if (XAMLroot == null)
                {                   
                    _failure = true;
                    TestLog.Current.LogEvidence("**Failed: xaml root object is null!");
                    TestLog.Current.LogEvidence("Xaml doc InnerXml: " + _newDoc.InnerXml);
                }
            }
            else
            {
                if (XAMLroot != null)
                {                                 
                    _failure = true;
                    TestLog.Current.LogEvidence("**Failed: xaml root object is not null!");
                    TestLog.Current.LogEvidence("Xaml doc InnerXml: " + _newDoc.InnerXml);
                }
            }
            sr.Close();
        }
    }
}
