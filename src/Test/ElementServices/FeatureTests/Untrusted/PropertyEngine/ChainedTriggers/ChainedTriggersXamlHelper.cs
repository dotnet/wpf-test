// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Constructs xaml for ChainedTriggersModel.
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
using System.Collections;
using Microsoft.Test.Modeling;
using Microsoft.Test.Serialization;
using Avalon.Test.CoreUI.Common;
//using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.Threading;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.PropertyEngine.ChainedTriggers
{
    /// <summary>
    /// Constructs xaml for the PropertyTriggerModel.
    /// </summary>
    internal class ChainedTriggersModelXamlHelper
    {        
        /// <summary>
        /// Constructs xaml according to the model param values.
        /// </summary>
        /// <returns>A stream of the constructed xaml.</returns>
        public Stream GenerateXaml(ChainedTriggersModelState modelState)
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
            tempDoc.Load("ChainedTriggersModel_empty.xaml");
            s_mainDoc = tempDoc;

            // Create XmlDocument with snippets of xaml to use
            // for contructing the main document.tempDoc = new XmlDocument();
            tempDoc = new XmlDocument();
            tempDoc.Load("ChainedTriggersModel_elements.xaml");
            _elementsDoc = tempDoc;

            // Assign properties to be used in triggers.
            _AssignTriggerProperties();

            // Hold a reference to the "root" node to use for 
            // inserting xaml into the main document.
            _testRootNode = s_mainDoc.SelectSingleNode("//*[@Name='TestRoot']");


            if (_state.Context == "Style")
            {                
                // Insert the styled item and its optional parent.
                _InsertResourcedItemParent();
                _InsertResourcedItem();

                // Insert the ResourceDictionary that Contains the triggers.
                _InsertResource();
            }

            else
            {
                // Handle the various model params to construct the xaml.
                _InsertTemplatedNodeParent();
                _InsertTemplatedControl();

                //// Insert the Template.
                _InsertTemplateParent();
                _InsertTemplate();
                //// Inserted the Template.

                //// Insert template contents.
                _InsertTemplateRootType();
                _InsertTemplateChildType();
            }

            _RemoveTestIds();

            // Convert XmlDocument to stream.
            return IOHelper.ConvertXmlDocumentToStream(s_mainDoc);
        }

        /// <summary>
        /// Assigns the properties and the values to be used by the triggers
        /// </summary>
        private void _AssignTriggerProperties()
        {
            _properties.Add(1, "FontSize");
            _properties.Add(2, "Height");
            _properties.Add(3, "Width");
            _properties.Add(4, "Opacity");
            _properties.Add(5, "Background");

            _values.Add(1, "24");
            _values.Add(2, "200");
            _values.Add(3, "100");
            _values.Add(4, "0.5");
            _values.Add(5, "Green");
        }

        /// <summary>
        /// Insert a wrapper for the templated control as needed and assign the templated node parent.
        /// </summary>
        private void _InsertTemplatedNodeParent()
        {
            // The test root node is the templated node parent.
            _templatedNodeParent = _testRootNode;
        }

        /// <summary>
        /// Insert the element/control that will be templated.
        /// </summary>
        private void _InsertTemplatedControl()
        {
            // Grab templated control element from elements.
            XmlNode templatedNode = _elementsDoc.SelectSingleNode("//*[@Name='FrameworkElement']", _nsmgr);
            if (templatedNode == null)
            {
                throw new NotSupportedException("TemplatedControlType: ContentControl");
            }

            // Remove the Background property if template triggers target the templated parent.
            // Template triggers (like style triggers) are overridden by settings in the templated control.
            if (_state.First != "Animation")
            {
                templatedNode.Attributes.Remove(templatedNode.Attributes["Height"]);
            }
            if (_state.Second != "Animation")
            {
                templatedNode.Attributes.Remove(templatedNode.Attributes["Width"]);             
            }

            // Insert the templated node.
            _templatedNode = _ImportAndInsertNode(_templatedNodeParent, templatedNode);
        }

        /// <summary>
        /// Create inline elements or get resources section to host controltemplate or its style.
        /// </summary>
        private void _InsertTemplateParent()
        {
            //
            // Create/Get _templateNodeParent. 
            //
            // Template is inline in templated control, create <Element.Template/>
            _templateNodeParent = s_mainDoc.CreateElement(_templatedNode.Prefix, _templatedNode.LocalName + ".Template", _templatedNode.NamespaceURI);
            _templatedNode.PrependChild(_templateNodeParent);
        }

        /// <summary>
        /// Add template.
        /// </summary>
        private void _InsertTemplate()
        {
            // Grab template definition and add the template root.
            _templateNode = _elementsDoc.SelectSingleNode("//av:ControlTemplate", _nsmgr);
            
            // Set ControlTemplate.TargetType attribute.
            XmlAttribute attribute = _templateNode.Attributes["TargetType"];
            attribute.Value = attribute.Value.Replace("FOO", _templatedNode.Name);
            
            // Set x:Key attribute 
            XmlAttribute keyAttribute = _templateNode.Attributes["x:Key"];
            _templateNode.Attributes.Remove(keyAttribute);

            _templateTriggersNode = _templateNode.FirstChild;
            _AdjustTriggers();   

            // Insert template.
            _templateNode = _ImportAndInsertNode(_templateNodeParent, _templateNode);
        }

        /// <summary>
        /// Add top-level child inside template.
        ///     &lt;TemplateType&gt;
        ///         &lt;TopLevelChild /&gt;
        ///     &lt;/TemplateType&gt;
        /// </summary>
        private void _InsertTemplateRootType()
        {
            // Grab template root from elements.
            XmlNode templateRootNode = _elementsDoc.SelectSingleNode("//*[@x:Name='TemplateRootType_Control']", _nsmgr);
            if (templateRootNode == null)
            {
                throw new NotSupportedException("TemplateRootType: Control");
            }

            _templateRootNode = _ImportAndInsertNode(_templateNode, templateRootNode);
        }

        /// <summary>
        /// Add nested child inside template.
        ///     &lt;TemplateType&gt;
        ///         &lt;TopLevelChild&gt;
        ///             &lt;NestedChild /&gt;
        ///         &lt;/TopLevelChild&gt;
        ///     &lt;/TemplateType&gt;
        /// </summary>
        private void _InsertTemplateChildType()
        {
            // Grab child code from elements.
            XmlNode templateChildNode = null;
            templateChildNode = _elementsDoc.SelectSingleNode("//*[@x:Name='TemplateChildType_FrameworkElement']", _nsmgr);

            if (templateChildNode == null)
            {
                throw new NotSupportedException("TemplateChildType: FrameworkElement");
            }

            // Insert template child under template root in main doc.
            _templateChildNode = _ImportAndInsertNode(_templateRootNode, templateChildNode);
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
                throw new NotSupportedException("ResourcedItem: FrameworkElement");
            }

            // Grab the Background attribute and attach appropriate lookup type
            if (_state.First != "Animation")
            {
                _resourcedNode.Attributes.Remove(_resourcedNode.Attributes["Height"]);
            }
            if (_state.Second != "Animation")
            {
                _resourcedNode.Attributes.Remove(_resourcedNode.Attributes["Width"]);
            }

            // Insert the styled node in the test doc and hold reference to it.
            _resourcedNode = _ImportAndInsertNode(_resourcedNodeParent, _resourcedNode);
        }        

        /// <summary>
        /// Insert the resource dictionary that is used by the resourced item.
        /// </summary>
        private void _InsertResource()
        {
            //
            // Get reference to parent node the style will be put in...
            //

            // Get the resource section of the resourced item's parent.
            // Add resources to the test root.            
            _resourceParentNode = FindOrAddNode("StackPanel.Resources", _testRootNode);
            
            if (_resourceParentNode == null)
            {
                throw new Exception("Could not find resource element");
            }


            // Grab the Resource Dictionary node from elements, edit, and insert it.
            _styleNode = _elementsDoc.SelectSingleNode("//*[@TestId='StyleNode']");
            _styleTriggersNode = _styleNode.FirstChild;            
            _AdjustTriggers();            
            
            //
            // Insert the resourceDictionary!
            //

            // Remove extra Markup
            _RemoveTestIdsMarkup();

            _styleNode = _ImportAndInsertNode(_resourceParentNode, _styleNode);
        }

        private void _InsertTrigger(int order, string triggerType)
        {
            XmlNode trigger = null;

            XmlNode triggerNode = _styleTriggersNode;
            if (_state.Context != "Style")
            {
                triggerNode = _templateTriggersNode;
            }

            // Multiple Trigger.
            if (triggerType == "Multi")
            {
                trigger = _elementsDoc.SelectSingleNode("//*[@TestId='MultiTrigger']");
                trigger = _ImportAndInsertNode(triggerNode, trigger);
                XmlNode conditionCollection = trigger.FirstChild;                
                XmlNode setter = trigger.LastChild;

                // Assign CONDITION property and value.
                XmlAttribute conditionProperty = conditionCollection.FirstChild.Attributes["Property"];
                XmlAttribute conditionValue = conditionCollection.FirstChild.Attributes["Value"];

                conditionProperty.Value = _properties[order] as string;
                conditionValue.Value = _values[order] as string;
            
                // Assign SETTER property and value.
                XmlAttribute setterProperty = setter.Attributes["Property"];
                XmlAttribute setterValue = setter.Attributes["Value"];

                setterProperty.Value = _properties[order + 1] as string;
                setterValue.Value = _values[order + 1] as string;              
            }

            // Single Trigger.
            else if (triggerType == "Single")
            {
                trigger = _elementsDoc.SelectSingleNode("//*[@TestId='SingleTrigger']");
                trigger = _ImportAndInsertNode(triggerNode, trigger);                
                XmlNode setter = trigger.LastChild;

                // Assign CONDITION property and value.
                XmlAttribute conditionProperty = trigger.Attributes["Property"];
                XmlAttribute conditionValue = trigger.Attributes["Value"];

                conditionProperty.Value = _properties[order] as string;
                conditionValue.Value = _values[order] as string;

                // Assign SETTER property and value.
                XmlAttribute setterProperty = setter.Attributes["Property"];
                XmlAttribute setterValue = setter.Attributes["Value"];

                setterProperty.Value = _properties[order + 1] as string;
                setterValue.Value = _values[order + 1] as string;
            }

            // Triger With storyBoard Action (enter and exit).
            else
            {                
                trigger = _elementsDoc.SelectSingleNode("//*[@TestId='StoryBoardTrigger']");
                trigger = _ImportAndInsertNode(triggerNode, trigger);
                XmlNode animationEnter = trigger.FirstChild.FirstChild.FirstChild.FirstChild;

                // Assign CONDITION property and value.
                XmlAttribute conditionProperty = trigger.Attributes["Property"];
                XmlAttribute conditionValue = trigger.Attributes["Value"];

                conditionProperty.Value = _properties[order] as string;
                conditionValue.Value = _values[order] as string;

                // Assign EnterAction Property and To Value.
                XmlAttribute enterTargetProperty = animationEnter.Attributes["Storyboard.TargetProperty"];
                XmlAttribute enterToValue = animationEnter.Attributes["To"];

                enterTargetProperty.Value = _properties[order + 1] as string;
                enterToValue.Value = _values[order + 1] as string;                
            }
        }

        private void _AdjustTriggers()
        {
            //
            // Adjust the positioning and number of triggers.
            //

            // Insert Trigger.
            _InsertTrigger(1, _state.First);
            _InsertTrigger(2, _state.Second);
            _InsertTrigger(3, _state.Third);
            _InsertTrigger(4, _state.Fourth);
          
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

        // Holds the model instance that works with this helper.
        private ChainedTriggersModelState _state = null;

        // Necessary for xpath queries.
        private XmlNamespaceManager _nsmgr = null;
        private XmlDocument _elementsDoc = null;
        private static XmlDocument s_mainDoc;

        // Convenient references to xml nodes used throughout 
        // the xaml construction routines.                
        private XmlNode _testRootNode = null;

        // Template Items.
        private XmlNode _templatedNodeParent = null;
        private XmlNode _templatedNode = null;
        private XmlNode _templateNodeParent = null;
        private XmlNode _templateNode = null;
        private XmlNode _templateTriggersNode = null;

        private XmlNode _templateRootNode = null;
        private XmlNode _templateChildNode = null;
        
        //// Resource items.
        private XmlNode _resourcedNodeParent = null;
        private XmlNode _resourcedNode = null;
        private XmlNode _resourceParentNode = null;
        private XmlNode _styleNode = null;
        private XmlNode _styleTriggersNode = null;
        
        // Condition Properties and Values.
        Hashtable _properties = new Hashtable();
        Hashtable _values = new Hashtable();
    }
}
