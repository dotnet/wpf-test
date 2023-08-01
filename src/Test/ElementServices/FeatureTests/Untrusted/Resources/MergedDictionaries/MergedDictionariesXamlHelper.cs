// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Constructs xaml for MergedDictionariesModel.
 * Contributors: 
 *
 
  
 * Revision:         $Revision: 11 $
 
********************************************************************/
using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Avalon.Test.CoreUI;
using System.IO;
using System.Xml;
using System.Text;
using Microsoft.Test.Modeling;
using Microsoft.Test.Serialization;
using Avalon.Test.CoreUI.Common;

namespace Avalon.Test.CoreUI.Resources.MergedDictionaries
{
    /// <summary>
    /// Constructs xaml for the PropertyTriggerModel.
    /// </summary>
    internal class MergedDictionariesModelXamlHelper
    {
        /// <summary>
        /// Constructs xaml for external resourcedictionary file according to the model param values.
        /// </summary>
        /// <returns>A stream of the constructed xaml.</returns>
        public Stream GenerateExternalXaml(MergedDictionariesModelState modelState, int num)
        {
            // Load the model state.
            _state = modelState;

            // Construct the XmlNamespaceManager used for xpath queries later.
            NameTable ntable = new NameTable();
            _nsmgr = new XmlNamespaceManager(ntable);
            _nsmgr.AddNamespace("av", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            _nsmgr.AddNamespace("x", "http://schemas.microsoft.com/winfx/2006/xaml");            

            // Create main XmlDocument.
            XmlDocument tempDoc = new XmlDocument();
            tempDoc.Load("MergedDictionariesModelExternal_empty.xaml");
            s_externalDoc = tempDoc;

            // Create XmlDocument with snippets of xaml to use
            // for contructing the main document.tempDoc = new XmlDocument();
            tempDoc = new XmlDocument();
            tempDoc.Load("MergedDictionariesModel_elements.xaml");
            _elementsDoc = tempDoc;

            _externalTestRootNode = s_externalDoc.SelectSingleNode("//*[@TestId='ExternalRD']");

            _InsertExternalResources(num);

            _RemoveExternalTestIds();

            return IOHelper.ConvertXmlDocumentToStream(s_externalDoc);
        }

        // Insert Resources into external resource dictionary file.
        private void _InsertExternalResources(int num)
        {
            // First External ResourceDictionary
            if (num == 1)
            {
                // Internal External Duplicate Key
                if (_state.DuplicateKey == "InternalExternal")
                {
                    _externalResourceNode = _elementsDoc.SelectSingleNode("//*[@TestId='InternalExternalDup1']");
                    _resourcedNode = _ImportAndInsertNode(_externalTestRootNode, _externalResourceNode);
                }

                // Uses lookup from second dictionary.
                else if (_state.KeyLookup == "Ext_Ext")
                {
                    _externalResourceNode = _elementsDoc.SelectSingleNode("//*[@TestId='External1_extend']");
                    _resourcedNode = _ImportAndInsertNode(_externalTestRootNode, _externalResourceNode);
                }

                // Direct Lookup - no multirefs.
                else
                {
                    _externalResourceNode = _elementsDoc.SelectSingleNode("//*[@TestId='External1']");
                    _resourcedNode = _ImportAndInsertNode(_externalTestRootNode, _externalResourceNode);
                }
            }

            // Second External ResourceDictionary
            if (num == 2)
            {
                // Direct Lookup - no multirefs.
                if (_state.KeyLookup == "Ext_Ext" || _state.KeyLookup == "Loc_Ext" || _state.KeyLookup == "Foreign_Ext")
                {
                    _externalResourceNode = _elementsDoc.SelectSingleNode("//*[@TestId='External2_extended']");
                    _resourcedNode = _ImportAndInsertNode(_externalTestRootNode, _externalResourceNode);                    
                }

                // Resource used by some other resource.
                else
                {
                    _externalResourceNode = _elementsDoc.SelectSingleNode("//*[@TestId='External2']");
                    _resourcedNode = _ImportAndInsertNode(_externalTestRootNode, _externalResourceNode);             
                }
            }
        }

        /// <summary>
        /// Constructs xaml according to the model param values.
        /// </summary>
        /// <returns>A stream of the constructed xaml.</returns>
        public Stream GenerateXaml(MergedDictionariesModelState modelState)
        {
            // Load the model state.
            _state = modelState;

            // Construct the XmlNamespaceManager used for xpath queries later.
            NameTable ntable = new NameTable();
            _nsmgr = new XmlNamespaceManager(ntable);
            _nsmgr.AddNamespace("av", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            _nsmgr.AddNamespace("x", "http://schemas.microsoft.com/winfx/2006/xaml");
            _nsmgr.AddNamespace("cmn", "clr-namespace:Microsoft.Test.Serialization.CustomElements;assembly=TestRuntime");

            // Create main XmlDocument.
            XmlDocument tempDoc = new XmlDocument();
            tempDoc.Load("MergedDictionariesModel_empty.xaml");
            s_mainDoc = tempDoc;

            // Create XmlDocument with snippets of xaml to use
            // for contructing the main document.tempDoc = new XmlDocument();
            tempDoc = new XmlDocument();
            tempDoc.Load("MergedDictionariesModel_elements.xaml");
            _elementsDoc = tempDoc;


            // Set the Application Node
            if (_state.ResourceLocation == "ApplicationResources")
            {
                _SetApplicationRDNode();
            }

            // Hold a reference to the "root" node to use for 
            // inserting xaml into the main document.
            _testRootNode = s_mainDoc.SelectSingleNode("//*[@Name='TestRoot']");
                    
            _SetResourcedItemName();

            // Insert the styled item and its optional parent.
            _InsertResourcedItemParent();
            _InsertResourcedItem();

            // Insert other resources - for duplication and precedence verification purposes.
            _InsertOtherResources();

            // Insert the ResourceDictionary that is used by the resourced item.
            _InsertResource();


            if (_state.ResourceLocation == "ApplicationResources")
            {
                s_additionalAppMarkup = FormatElement(s_applicationRDNode.FirstChild);

                Console.WriteLine("================");
                Console.WriteLine(FormatElement(s_applicationRDNode.FirstChild));
                Console.WriteLine("================");
            }

            _RemoveTestIds();

            // Convert XmlDocument to stream.
            return IOHelper.ConvertXmlDocumentToStream(s_mainDoc);
        }

        /// <summary>
        /// Set the item name used to test the model styled item type.
        /// </summary>
        private void _SetResourcedItemName()
        {
            _resourcedItemName = "Button";
        }

        /// <summary>
        /// Insert a parent of styled item if necessary or just set _styledItemParent to the test root.
        /// </summary>
        private void _InsertResourcedItemParent()
        {
            // Default: The styled node's parent is the root node.
            _resourcedNodeParent = _testRootNode;
        }

        /// <summary>
        /// Insert the item that will be styled.
        /// </summary>
        private void _InsertResourcedItem()
        {
            // Grab styled item from elements doc.
            _resourcedNode = _elementsDoc.SelectSingleNode("//*[@Name='FrameworkElement']", _nsmgr);
            if (_resourcedNode == null)
            {
                throw new NotSupportedException("ResourcedITem: FrameworkElement");
            }

            // Grab the Background attribute and attach appropriate lookup type
            XmlAttribute resourceAttribute = _resourcedNode.Attributes["Background"];
            resourceAttribute.Value = resourceAttribute.Value.Replace("FOO", _state.Lookup);

            // Assign a Key that the item looks for.
            if (_state.DuplicateKey != "None")
            {
                resourceAttribute.Value = resourceAttribute.Value.Replace("Key", _state.DuplicateKey);
            }
            else if (_state.KeyLookup != "Direct")
            {
                resourceAttribute.Value = resourceAttribute.Value.Replace("Key", _state.KeyLookup);
            }
            else
            {
                resourceAttribute.Value = resourceAttribute.Value.Replace("Key", _state.Key);
            }
            
            // Insert the styled node in the test doc and hold reference to it.
            _resourcedNode = _ImportAndInsertNode(_resourcedNodeParent, _resourcedNode);
        }

        /// <summary>
        /// Insert non-mergeddictionary resources.
        /// </summary>
        private void _InsertOtherResources()
        {
            

            // Add resources to the test root if resource dictionary not in root.
            if (_state.ResourceLocation != "ParentResources")
            {
                _foreignResourceParentNode = FindOrAddNode("StackPanel.Resources", _testRootNode);
            }
            else
            {
                _foreignResourceParentNode = _testRootNode.ParentNode.FirstChild;
            }

            // Inserting Dupe Key.
            _foreignResourceNode = _elementsDoc.SelectSingleNode("//*[@TestId='InternalForeignDup']");
            _foreignResourceNode = _ImportAndInsertNode(_foreignResourceParentNode, _foreignResourceNode);


            // Insert KeyLookup.
            XmlNode _foreignResourceNode2 = _elementsDoc.SelectSingleNode("//*[@TestId='Foreign_extend']");
            XmlAttribute _foreignResourceNode2Key = null;

            if (_state.KeyLookup == "Foreign_Loc" || _state.KeyLookup == "Foreign_Ext")
            {                
                _foreignResourceNode2Key = _foreignResourceNode2.Attributes["x:Key"];
                _foreignResourceNode2Key.Value = _foreignResourceNode2Key.Value.Replace("FOO", _state.KeyLookup);
                _foreignResourceNode2 = _ImportAndInsertNode(_foreignResourceParentNode, _foreignResourceNode2);
            }
            else
            {
                _foreignResourceNode2.ParentNode.RemoveChild(_foreignResourceNode2);
            }

        }

        /// <summary>
        /// Insert the resource dictionary that is used by the resourced item.
        /// </summary>
        private void _InsertResource()
        {
            //
            // Get reference to parent node the style will be put in...
            //

            // If Resources are in the application, generate text to attach
            if (_state.ResourceLocation == "ApplicationResources")
            {
                _resourceParentNode = FindOrAddNode("Application.Resources", s_applicationRDNode);
            }
            // Get resources section of the document root.
            if (_state.ResourceLocation == "PageResources")
            {
                _resourceParentNode = _testRootNode.ParentNode.FirstChild;
            }

            // Get the resource section of the resourced item's parent.
            else if (_state.ResourceLocation == "ParentResources")
            {
                // Add resources to the test root.
                _resourceParentNode = FindOrAddNode("StackPanel.Resources", _testRootNode);
            }

            // Get the resources section in the resourced item.
            else if (_state.ResourceLocation == "ElementResources")
            {
                _resourceParentNode = FindOrAddNode(_resourcedItemName + ".Resources", _resourcedNode);
            }

            // Get an inline item.Style element to parent the style.
            else if (_state.ResourceLocation == "ElementInline")
            {
                _resourceParentNode = FindOrAddNode(_resourcedItemName + ".Resources", _resourcedNode);
            }

            if (_resourceParentNode == null)
            {
                throw new Exception("Could not find resource element");
            }

            //
            // Grab the Resource Dictionary node from elements, edit, and insert it.
            //
            
            _resourceNode = _elementsDoc.SelectSingleNode("//*[@TestId='ResourceDictionary']");
            _AdjustResources();
            
            //
            // Insert the resourceDictionary!
            //

            // Remove extra Markup
            _RemoveTestIdsMarkup();

            _resourceNode = _ImportAndInsertNode(_resourceParentNode, _resourceNode);
        }

        private void _AdjustResources()
        {
            //
            // Adjust the number of internal keys.
            //

            //
            // If Local Extension then retain extend and extension keys.
            //
            XmlNode Loc_extend = _elementsDoc.SelectSingleNode("//*[@TestId='Loc_extend']");
            XmlAttribute Loc_extendKey = Loc_extend.Attributes["x:Key"];

            XmlNode Loc_extension = _elementsDoc.SelectSingleNode("//*[@TestId='Loc_extension']");
                        
            // Extended Key Adjustment.
            if (_state.KeyLookup == "Loc_Ext" || _state.KeyLookup == "Loc_Loc")
            {
                Loc_extendKey.Value = Loc_extendKey.Value.Replace("FOO", _state.KeyLookup);            
            }
            else
            {                
                Loc_extend.ParentNode.RemoveChild(Loc_extend);                
            }

            // Extension Key adjustment.
            if (_state.KeyLookup != "Foreign_Loc" && _state.KeyLookup != "Loc_Loc")
            {
                Loc_extension.ParentNode.RemoveChild(Loc_extension);
            }

            //
            // Adjust Dupe Key.
            //
            
            XmlNode DupKey = _elementsDoc.SelectSingleNode("//*[@TestId='DupKey']");
            XmlAttribute resourceAttribute = DupKey.Attributes["x:Key"];

            // If not dupe then remove other resources and grab resource to use.
            if (_state.DuplicateKey == "None")
            {   
                // Remove Dup Key.
                DupKey.ParentNode.RemoveChild(DupKey);

                
            }

            // If dup - then retain appropriate resource.
            else
            {
                if (_state.DuplicateKey == "InternalLocal")
                {
                    resourceAttribute.Value = resourceAttribute.Value.Replace("FOO", _state.DuplicateKey);            
                }
                else if (_state.DuplicateKey == "InternalForeign")
                {
                    resourceAttribute.Value = resourceAttribute.Value.Replace("FOO", _state.DuplicateKey);
                }
                else if (_state.DuplicateKey == "InternalExternal")
                {
                    resourceAttribute.Value = resourceAttribute.Value.Replace("FOO", _state.DuplicateKey);
                }
            }

            // Adjust number of internal dictionaries.
            if (_state.InternalDicts == "1")
            {
                XmlNode internalDict2 = _elementsDoc.SelectSingleNode("//*[@TestId='InternalDict2']");
                internalDict2.ParentNode.RemoveChild(internalDict2);
            }

            // Adjust number of external dictionaries.
            if (Convert.ToInt32(_state.ExternalDicts) <= 1)
            {
                XmlNode externalDict2 = _elementsDoc.SelectSingleNode("//*[@TestId='ExternalDict2']");
                externalDict2.ParentNode.RemoveChild(externalDict2);
            }
            if (Convert.ToInt32(_state.ExternalDicts) == 0)
            {
                XmlNode externalDict1 = _elementsDoc.SelectSingleNode("//*[@TestId='ExternalDict1']");
                externalDict1.ParentNode.RemoveChild(externalDict1);
            }
        }


        private static void _SetApplicationRDNode()
        {
            XmlElement e1 = s_mainDoc.CreateElement("Application", s_mainDoc.NamespaceURI);
            if (e1.FirstChild == null)
                CoreLogger.LogStatus("Application Resource not set correclty");

            s_applicationRDNode = e1;
        }

        internal static string FormatElement(XmlNode element)
        {
            string spaces = new string(' ', 4);

            StringWriter sw = new StringWriter();

            XmlTextWriter writer = new XmlTextWriter(sw);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 4;           
            
            element.WriteTo(writer);            

            string original = sw.ToString();

            string[] lines = original.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb1 = new StringBuilder();
            foreach (string line in lines)
            {
                sb1.Append(spaces);
                sb1.Append(line.Replace(@" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""", "").Replace(@" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""", ""));
                sb1.Append(System.Environment.NewLine);
            }

            sb1.Remove(sb1.Length - 1, 1); //cosmetic: The last NewLine is not needed 
            return sb1.ToString();
        }

        // Clones a sub-tree from one XmlDocument instance
        // to another.
        private XmlNode _ImportAndInsertNode(XmlNode firstNode, XmlNode secondNode)
        {
            // Import second node to first node's document.
            XmlNode newNode = firstNode.OwnerDocument.ImportNode(secondNode, true);

            // Insert newly-imported node under the first node.
            firstNode.AppendChild(newNode);

            return newNode;
        }
        
        // Removes all TestIds from main document.
        private void _RemoveTestIdsMarkup()
        {
            // Get all elements with TestId attribute.
            XmlNodeList testList = _elementsDoc.SelectNodes(".//*[@TestId]", _nsmgr);

            // For each one, remove the TestId attribute.
            for (int i = 0; i < testList.Count; i++)
            {
                XmlAttribute attrib = testList[i].Attributes["TestId"];
                attrib.OwnerElement.Attributes.Remove(attrib);
            }
        }
        // Removes all TestIds from main document.
        private void _RemoveTestIds()
        {
            // Get all elements with TestId attribute.
            XmlNodeList testList = s_mainDoc.SelectNodes(".//*[@TestId]", _nsmgr);

            // For each one, remove the TestId attribute.
            for (int i = 0; i < testList.Count; i++)
            {
                XmlAttribute attrib = testList[i].Attributes["TestId"];
                attrib.OwnerElement.Attributes.Remove(attrib);
            }
        }

        // Removes all TestIds from external document.
        private void _RemoveExternalTestIds()
        {
            // Get all elements with TestId attribute.
            XmlNodeList testList = s_externalDoc.SelectNodes(".//*[@TestId]", _nsmgr);

            // For each one, remove the TestId attribute.
            for (int i = 0; i < testList.Count; i++)
            {
                XmlAttribute attrib = testList[i].Attributes["TestId"];
                attrib.OwnerElement.Attributes.Remove(attrib);
            }
        }

        /// <summary>
        /// Search for Xml node with name nodeName under parent and return a reference to it,
        /// otherwise create it.
        /// </summary>
        private XmlNode FindOrAddNode(string nodeName, XmlNode parent)
        {
            XmlNode node = parent.SelectSingleNode("av:" + nodeName, _nsmgr);

            if (node == null)
            {
                // Create new node in main doc.
                //node = _mainDoc.CreateElement(prefix, nodeName, _testRootNode.NamespaceURI);
                node = s_mainDoc.CreateElement(nodeName, parent.NamespaceURI);

                // Add the new node under the parent.
                // HACK: Always put resources sections first
                if (nodeName.Contains("Resources"))
                {
                    node = parent.PrependChild(node);
                }
                else
                {
                    node = parent.AppendChild(node);
                }
            }

            return node;
        }

        //
        // Private Fields
        //

        internal static string AdditionalAppMarkup
        {
            get
            {
                return s_additionalAppMarkup;
            }
            private set
            {
                s_additionalAppMarkup = value;
            }
        }
        private static string s_additionalAppMarkup;

        // Holds the model instance that works with this helper.
        private MergedDictionariesModelState _state = null;

        // Necessary for xpath queries.
        private XmlNamespaceManager _nsmgr = null;
        private XmlDocument _elementsDoc = null;
        private static XmlDocument s_mainDoc;
        private static XmlDocument s_externalDoc;

        // Convenient references to xml nodes used throughout 
        // the xaml construction routines.                
        private static XmlNode s_applicationRDNode;
        private XmlNode _testRootNode = null;
        private XmlNode _externalTestRootNode = null;

        // Resource items
        private XmlNode _externalResourceNode = null;

        private XmlNode _foreignResourceNode = null;
        private XmlNode _foreignResourceParentNode = null;
            
        private XmlNode _resourcedNodeParent = null;
        private XmlNode _resourcedNode = null;
        private XmlNode _resourceParentNode = null;
        private XmlNode _resourceNode = null;

        private string _resourcedItemName = null;
    }
}
