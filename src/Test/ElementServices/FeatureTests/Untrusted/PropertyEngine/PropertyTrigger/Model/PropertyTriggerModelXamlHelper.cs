// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Constructs xaml for PropertyTriggerModel.
 * Contributors: Microsoft
 *
 
  
 * Revision:         $Revision: 11 $
 
********************************************************************/
using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Avalon.Test.CoreUI;
using System.IO;
using System.Xml;
using Microsoft.Test.Modeling;
using Microsoft.Test.Serialization;
using Avalon.Test.CoreUI.Common;

namespace Avalon.Test.CoreUI.PropertyEngine.PropertyTrigger
{
    /// <summary>
    /// Constructs xaml for the PropertyTriggerModel.
    /// </summary>
    internal class PropertyTriggerModelXamlHelper
    {
        /// <summary>
        /// Constructs xaml according to the model param values.
        /// </summary>
        /// <returns>A stream of the constructed xaml.</returns>
        public Stream GenerateXaml(PropertyTriggerModelState modelState)
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
            tempDoc.Load("PropertyTriggerModel_empty.xaml");
            _mainDoc = tempDoc;

            // Create XmlDocument with snippets of xaml to use
            // for contructing the main document.tempDoc = new XmlDocument();
            tempDoc = new XmlDocument();
            tempDoc.Load("PropertyTriggerModel_elements.xaml");
            _elementsDoc = tempDoc;

            // Hold a reference to the "root" node to use for 
            // inserting xaml into the main document.
            _testRootNode = _mainDoc.SelectSingleNode("//*[@Name='TestRoot']");

            if (_state.StyleLocation != "None")
            {
                _SetStyledItemName();

                // Insert the styled item and its optional parent.
                _InsertStyledItemParent();
                _InsertStyledItem();

                // Insert the style that will be applied to the styled item.
                _InsertStyle();

                //insert singleTriggers
                for (int i = 1; i <= Convert.ToInt32(_state.NumSingleTrigs); i++)
                {
                    _InsertSingleStyleTrigger(i);
                }

                //insert multiTriggers
                for (int j = 1; j <= Convert.ToInt32(_state.NumMultiTrigs); j++)
                {
                    _InsertMultiStyleTrigger(j);
                }

                //insert StoryBoardActions Trigger
                _InsertStyleHasStoryBoardActions();
            }
            else
            {
                //// Handle the various model params to construct the xaml.
                _InsertTemplatedNodeParent();
                _InsertTemplatedControl();

                //// Insert the Template.
                _InsertTemplateParent();
                _InsertTemplate();
                //// Inserted the Template.

                //// Insert template contents.
                _InsertTemplateRootType();
                _InsertTemplateChildType();


                //insert singleTriggers
                for (int i = 1; i <= Convert.ToInt32(_state.NumSingleTrigs); i++)
                {
                    _InsertSingleTrigger(i);
                }

                //insert multiTriggers
                for (int j = 1; j <= Convert.ToInt32(_state.NumMultiTrigs); j++)
                {
                    _InsertMultiTrigger(j);
                }

                //insert StoryBoardActions Trigger
                _InsertHasStoryBoardActions();
            }

            _RemoveTestIds();

            // Convert XmlDocument to stream.
            return IOHelper.ConvertXmlDocumentToStream(_mainDoc);
        }

        /// <summary>
        /// Set the item name used to test the model styled item type.
        /// </summary>
        private void _SetStyledItemName()
        {
            _styledItemName = "Button";
        }

        /// <summary>
        /// Insert a parent of styled item if necessary or just set _styledItemParent to the test root.
        /// </summary>
        private void _InsertStyledItemParent()
        {
            // Default: The styled node's parent is the root node.
            _styledNodeParent = _testRootNode;
        }
        /// <summary>
        /// Insert the item that will be styled.
        /// </summary>
        private void _InsertStyledItem()
        {
            // Grab styled item from elements doc.
            _styledNode = _elementsDoc.SelectSingleNode("//*[@Name='StyledItem_FrameworkElement']", _nsmgr);
            if (_styledNode == null)
            {
                throw new NotSupportedException("StyledItem: StyledItem_FrameworkElement");
            }

            // Remove style attribute if style is inline or implicit.
            if (_state.StyleLocation == "ElementInline")
            {
                // Remove Style attribute if Style is set implicitly or inline.
                _styledNode.Attributes.Remove(_styledNode.Attributes["Style"]);
            }
            else
            {
                XmlAttribute styleAttribute = _styledNode.Attributes["Style"];
                styleAttribute.Value = styleAttribute.Value.Replace("FOO", "DynamicResource");
            }

            // Insert the styled node in the test doc and hold reference to it.
            _styledNode = _ImportAndInsertNode(_styledNodeParent, _styledNode);
        }

        /// <summary>
        /// Insert the style that will be applied to the styled item.
        /// </summary>
        private void _InsertStyle()
        {
            //
            // Get reference to parent node the style will be put in...
            //

            // Get resources section of the document root.
            if (_state.StyleLocation == "PageResources")
            {
                _styleParentNode = _testRootNode.ParentNode.FirstChild;
            }

            // Get the resource section of the styled item's parent.
            else if (_state.StyleLocation == "ParentResources")
            {
                // Add resources to the test root.
                _styleParentNode = FindOrAddNode("StackPanel.Resources", _testRootNode);
            }

            // Get the resources section in the styled item.
            else if (_state.StyleLocation == "ElementResources")
            {
                _styleParentNode = FindOrAddNode(_styledItemName + ".Resources", _styledNode);
            }

            // Get an inline item.Style element to parent the style.
            else if (_state.StyleLocation == "ElementInline")
            {
                _styleParentNode = FindOrAddNode(_styledItemName + ".Style", _styledNode);                
            }

            if (_styleParentNode == null)
            {
                throw new Exception("Could not find page resource element");
            }

            //
            // Grab the style node from elements, edit, and insert it.
            //

            _styleNode = _elementsDoc.SelectSingleNode("//*[@TestId='StyleNode']");

            _styleNode.Attributes.Remove(_styleNode.Attributes["BasedOn"]);

            // Remove key attribute if inline style.
            if (_state.StyleLocation == "ElementInline")
            {
                _styleNode.Attributes.Remove(_styleNode.Attributes["x:Key"]);
            }
            
            XmlAttribute targetTypeAttribute = _styleNode.Attributes["TargetType"];
            targetTypeAttribute.Value = targetTypeAttribute.Value.Replace("FOO", _styledItemName);

            //
            // Insert the style!
            //
            _styleNode = _ImportAndInsertNode(_styleParentNode, _styleNode);
        }

        /// <summary>
        /// Insert property trigger in base and derived style.
        /// </summary>
        private void _InsertSingleStyleTrigger(int id)
        {
            // Insert the Trigger in the Triggers node.
            //throw new Exception("Could not get setter nodeStyle_Single_" + id.ToString());
            XmlNode triggerNode = _InsertTrigger(_styleNode, "Style_Single_" + id.ToString());
            
            // Select all the setters in the template.
            XmlNodeList setNodes = triggerNode.SelectNodes("./av:Setter", _nsmgr);

            //
            // Adjust Number of Setters
            //
            int setters_to_remove = (setNodes.Count - Convert.ToInt32(_state.NumSetters));
            for (int j = 0; j < setters_to_remove; j++)
            {
                triggerNode.RemoveChild(triggerNode.LastChild);
            }
        }

        /// <summary>
        /// Insert property trigger in base and derived style.
        /// </summary>
        private void _InsertMultiStyleTrigger(int id)
        {
            // Insert the Trigger in the Triggers node.
            XmlNode triggerNode = _InsertTrigger(_styleNode, "Style_Multiple_"+ id.ToString());

            //Remove any extraneous conditions
            int conditions_to_remove = 3 - Convert.ToInt32(_state.NumConditions);
            for (int i = 0; i < conditions_to_remove; i++)
            {
                triggerNode.FirstChild.RemoveChild(triggerNode.FirstChild.LastChild);
            }


            // Select all the setters in the template.
            XmlNodeList setNodes = triggerNode.SelectNodes("./av:Setter", _nsmgr);
            if (setNodes == null)
            {
                throw new Exception("Could not get setter node");
            }

            

            //
            // Adjust Number of Setters
            //
            int setters_to_remove = (setNodes.Count - Convert.ToInt32(_state.NumSetters));
            for (int j = 0; j < setters_to_remove; j++)
            {
                triggerNode.RemoveChild(triggerNode.LastChild);
            }

        }
        /// <summary>
        /// Helper, create Style.Triggers node in styleNode if it doesn't already exist. 
        /// Select trigger with TestId triggerName from elements and insert it.
        /// </summary>
        private XmlNode _InsertTrigger(XmlNode styleNode, string triggerName)
        {
            // Add Triggers node, if none.
            XmlNode triggersNode = FindOrAddNode("Style.Triggers", styleNode);

            // Import trigger and insert in style.
            return _InsertFromElementsDoc("//*[@TestId='" + triggerName + "']", triggersNode);
        }

        private XmlNode _InsertFromElementsDoc(string xpath, XmlNode parent)
        {
            XmlNode newNode = _elementsDoc.SelectSingleNode(xpath, _nsmgr);
            if (parent == null)
            {
                throw new Exception("Could not get newNode");
            }
            return _ImportAndInsertNode(parent, newNode);
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
            XmlNode templatedNode = _elementsDoc.SelectSingleNode("//*[@Name='TemplatedControlType_ContentControl']", _nsmgr);
            if (templatedNode == null)
            {
                throw new NotSupportedException("TemplatedControlType: ContentControl");
            }

            // Remove data binding attribute.
            XmlAttribute bindingAttribute = templatedNode.Attributes["ItemsSource"];
            if (bindingAttribute != null)
            {
                templatedNode.Attributes.Remove(bindingAttribute);
            }

            bindingAttribute = templatedNode.Attributes["ContentTemplate"];
            if (bindingAttribute != null)
            {
                templatedNode.Attributes.Remove(bindingAttribute);
            }

            // Remove content binding from button.
            bindingAttribute = templatedNode.Attributes["Content"];
            if (bindingAttribute != null)
            {
                templatedNode.Attributes.Remove(bindingAttribute);
            }

            // Remove the Background property if template triggers target the templated parent.
            // Template triggers (like style triggers) are overridden by settings in the templated control.
            if (_state.TriggerTarget == "TemplatedParent")
            {
                templatedNode.Attributes.Remove(templatedNode.Attributes["Background"]);
            }

            templatedNode.Attributes.Remove(templatedNode.Attributes["Style"]);
            
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
            _templateNodeParent = _mainDoc.CreateElement(_templatedNode.Prefix, _templatedNode.LocalName + "." + _state.TemplatePropertyName, _templatedNode.NamespaceURI);
            _templatedNode.PrependChild(_templateNodeParent);
        }

        /// <summary>
        /// Add template.
        /// </summary>
        private void _InsertTemplate()
        {
            // Grab template definition and add the template root.
            XmlNode templateNode = _elementsDoc.SelectSingleNode("//av:" + _state.TemplateType, _nsmgr);
            if (templateNode == null)
            {
                throw new NotSupportedException("Template type: " + _state.TemplateType);
            }

            // Set ControlTemplate.TargetType attribute.
            if (_state.TemplateType == "ControlTemplate")
            {
                XmlAttribute attribute = templateNode.Attributes["TargetType"];
                attribute.Value = attribute.Value.Replace("FOO", _templatedNode.Name);
            }

            // Set x:Key attribute 
            XmlAttribute keyAttribute = templateNode.Attributes["x:Key"];
            templateNode.Attributes.Remove(keyAttribute);

            // Insert template.
            _templateNode = _ImportAndInsertNode(_templateNodeParent, templateNode);
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
        /// Inserts xaml for the property Trigger.
        /// </summary>
        private void _InsertSingleTrigger(int id)
        {
            // Retrieve property Trigger node from elements.
            XmlNode propTriggerNode;
            if (_state.TriggerTarget == "TemplateChild")
            {
                propTriggerNode = _elementsDoc.SelectSingleNode("//*[@TestId='ChildPropertyTrigger_FrameworkElement_Single_" + id.ToString() + "']", _nsmgr);
            }
            else
            {
                propTriggerNode = _elementsDoc.SelectSingleNode("//*[@TestId='PropertyTrigger_Control_Single_" + id.ToString() + "']", _nsmgr);
            }

            if (propTriggerNode == null)
            {
                throw new NotSupportedException("Trigger target " + _state.TriggerTarget);
            }

            //
            // Set SourceName value.
            //
            XmlAttribute sourceAttrib = propTriggerNode.Attributes["SourceName"];

            switch (_state.TriggerSource)
            {
                case "TemplateRoot":
                    // Trigger sources off of TemplateRoot.
                    sourceAttrib.Value = sourceAttrib.Value.Replace("BAR", _templateRootNode.Attributes["x:Name"].Value);
                    break;
                case "TemplateChild":
                    // Trigger sources off of TemplateChild.
                    sourceAttrib.Value = sourceAttrib.Value.Replace("BAR", _templateChildNode.Attributes["x:Name"].Value);
                    break;
                case "TemplatedParent":
                    // Trigger sources off the templated control. Remove the SourceName property.
                    propTriggerNode.Attributes.Remove(sourceAttrib);
                    break;
                default:
                    throw new NotSupportedException("Trigger source " + _state.TriggerSource + " not supported");
            }

            //Add Setters to Trigger
            _AddSetters(propTriggerNode);

            // Insert the Trigger in the Triggers node.
            propTriggerNode = _ImportAndInsertNode(_GetTriggersNode(), propTriggerNode);

        }

        /// <summary>
        /// Inserts xaml for the property Trigger.
        /// </summary>
        private void _InsertMultiTrigger(int id)
        {
            if (id == 0) return;

            // Retrieve property Trigger node from elements.
            XmlNode propTriggerNode;
            if (_state.TriggerTarget == "TemplateChild")
            {
                propTriggerNode = _elementsDoc.SelectSingleNode("//*[@TestId='ChildPropertyTrigger_FrameworkElement_Multiple_" + id.ToString() + "']", _nsmgr);
            }
            else
            {
                propTriggerNode = _elementsDoc.SelectSingleNode("//*[@TestId='PropertyTrigger_Control_Multiple_" + id.ToString() + "']", _nsmgr);
            }

            if (propTriggerNode == null)
            {
                throw new NotSupportedException("Trigger target " + _state.TriggerTarget);
            }

            //XmlAttribute sourceAttrib = propTriggerNode.FirstChild.FirstChild.Attributes["SourceName"];
            XmlNodeList condNodes = propTriggerNode.FirstChild.SelectNodes("./av:Condition", _nsmgr);

            for (int i = 0; i < condNodes.Count; i++)
            {
                XmlAttribute srcAttrib = condNodes[i].Attributes["SourceName"];

                switch (_state.TriggerSource)
                {
                    case "TemplateRoot":
                        // Trigger sources off of TemplateRoot.
                        srcAttrib.Value = srcAttrib.Value.Replace("BAR", _templateRootNode.Attributes["x:Name"].Value);
                        break;
                    case "TemplateChild":
                        // Trigger sources off of TemplateChild.
                        srcAttrib.Value = srcAttrib.Value.Replace("BAR", _templateChildNode.Attributes["x:Name"].Value);
                        break;
                    case "TemplatedParent":
                        // Trigger sources off the templated control. Remove the SourceName property.
                        condNodes[i].Attributes.Remove(srcAttrib);
                        break;
                    default:
                        throw new NotSupportedException("Trigger source " + _state.TriggerSource + " not supported");
                }
            }

            int conditions_to_remove = 3 - Convert.ToInt32(_state.NumConditions);
            for (int i = 0; i < conditions_to_remove; i++)
            {
                propTriggerNode.FirstChild.RemoveChild(propTriggerNode.FirstChild.LastChild);
            }

            //Add Setters to Trigger
            _AddSetters(propTriggerNode);

            // Insert the Trigger in the Triggers node.
            propTriggerNode = _ImportAndInsertNode(_GetTriggersNode(), propTriggerNode);
        }

        /// <summary>
        /// Insert StoryBoardAction Node for templates.
        /// </summary>
        private void _InsertHasStoryBoardActions()
        {
            if (_state.HasStoryBoardActions == "False") return;

            // Select event trigger and storyboard from elements.
            XmlNode triggerNode = _elementsDoc.SelectSingleNode("//*[@TestId='StoryBoard_Trigger']", _nsmgr);

            if (triggerNode == null)
            {
                throw new NotSupportedException("Trigger target " + _state.TriggerTarget);
            }

            // Make EventTrigger SourceName point at template root node.
            XmlAttribute targetAttrib = triggerNode.Attributes["SourceName"];
            targetAttrib.Value = targetAttrib.Value.Replace("BAR", _templateRootNode.Attributes["x:Name"].Value);

            // Make both ColorAnimation Storyboard.TargetName's point at template root node.
            XmlNodeList animationNodes = triggerNode.SelectNodes(".//av:DoubleAnimation", _nsmgr);

            for (int i = 0; i < animationNodes.Count; i++)
            {
                XmlAttribute targetNameAttrib = animationNodes[i].Attributes["Storyboard.TargetName"];
                // if the target is the templated parent remove the TargetName attribute, otherwise set it to the template root's name.
                switch (_state.TriggerTarget)
                {
                    case "TemplatedParent":
                        animationNodes[i].Attributes.Remove(targetNameAttrib);
                        break;
                    case "TemplateChild":
                        targetNameAttrib.Value = targetNameAttrib.Value.Replace("FOO", _templateChildNode.Attributes["x:Name"].Value);
                        break;
                    case "TemplateRoot":
                        targetNameAttrib.Value = targetNameAttrib.Value.Replace("FOO", _templateRootNode.Attributes["x:Name"].Value);
                        break;
                    default:
                        throw new NotSupportedException("TriggerTarget " + _state.TriggerTarget);
                }                
            }

            // Put the event trigger in Triggers.
            _ImportAndInsertNode(_GetTriggersNode(), triggerNode);
        }

        /// <summary>
        /// Insert StoryBoardAction Node for templates.
        /// </summary>
        private void _InsertStyleHasStoryBoardActions()
        {
            if (_state.HasStoryBoardActions == "False") return;

            // Select event trigger and storyboard from elements.
            XmlNode triggerNode = _InsertTrigger(_styleNode, "Style_StoryBoard_Trigger");            
        }

        //
        // Helper functions.
        //
        //Adds Setters to the Trigger
        private void _AddSetters(XmlNode propTriggerNode)
        {
            // Select all the setters in the template.
            XmlNodeList setNodes = propTriggerNode.SelectNodes("./av:Setter", _nsmgr);

            //
            // Adjust Number of Setters
            //
            int setters_to_remove = (setNodes.Count - Convert.ToInt32(_state.NumSetters));
            for (int j = 0; j < setters_to_remove; j++)
            {
                propTriggerNode.RemoveChild(propTriggerNode.LastChild);
            }

            // Set the trigger target and source properties as appropriate.
            for (int i = 0; i < Convert.ToInt32(_state.NumSetters); i++)
            {
                XmlAttribute targetAttrib = setNodes[i].Attributes["TargetName"];

                // if the target is the templated parent remove the TargetName attribute, otherwise set it to the template root's name.
                switch (_state.TriggerTarget)
                {
                    case "TemplatedParent":
                        setNodes[i].Attributes.Remove(targetAttrib);
                        break;
                    case "TemplateChild":
                        targetAttrib.Value = targetAttrib.Value.Replace("FOO", _templateChildNode.Attributes["x:Name"].Value);
                        break;
                    case "TemplateRoot":
                        targetAttrib.Value = targetAttrib.Value.Replace("FOO", _templateRootNode.Attributes["x:Name"].Value);
                        break;
                    default:
                        throw new NotSupportedException("TriggerTarget " + _state.TriggerTarget);
                }
            }
        }

        // Gets the ControlTemplate.Triggers propertytag from the main XmlDocument.
        // If it isn't there, it inserts it.
        private XmlNode _GetTriggersNode()
        {
            XmlNode triggersNode = _mainDoc.SelectSingleNode("//av:" + _state.TemplateType + ".Triggers", _nsmgr);

            if (triggersNode == null)
            {
                triggersNode = _elementsDoc.SelectSingleNode("//av:" + _state.TemplateType + ".Triggers", _nsmgr);

                triggersNode = _ImportAndInsertNode(_templateNode, triggersNode);
            }

            return triggersNode;
        }

        // Gets the RootNode.Resources propertytag from the main XmlDocument.
        // If it isn't there, it inserts it.
        private XmlNode _GetResourcesNode()
        {
            string tagName = _testRootNode.Name + ".Resources";
            XmlNode resourcesNode = _mainDoc.SelectSingleNode("//av:" + tagName, _nsmgr);

            if (resourcesNode == null)
            {
                resourcesNode = _mainDoc.CreateElement("", tagName, _testRootNode.NamespaceURI);
                _testRootNode.PrependChild(resourcesNode);
            }

            return resourcesNode;
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
            XmlNodeList testList = _mainDoc.SelectNodes(".//*[@TestId]", _nsmgr);

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
                node = _mainDoc.CreateElement(nodeName, parent.NamespaceURI);

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

        //// Holds the model instance that works with this helper.
        private PropertyTriggerModelState _state = null;

        //// Necessary for xpath queries.
        private XmlNamespaceManager _nsmgr = null;

        //// Convenient references to xml nodes used throughout 
        //// the xaml construction routines.
        private XmlDocument _elementsDoc = null;
        private XmlDocument _mainDoc = null;
        private XmlNode _testRootNode = null;
        private XmlNode _templatedNodeParent = null;
        private XmlNode _templatedNode = null;
        private XmlNode _templateNodeParent = null;
        private XmlNode _templateNode = null;

        private XmlNode _templateRootNode = null;
        private XmlNode _templateChildNode = null;
        private string _styledItemName = null;

        // Style item
        private XmlNode _styledNodeParent = null;
        private XmlNode _styledNode = null;
        private XmlNode _styleParentNode = null;
        private XmlNode _styleNode = null;
    }
}
