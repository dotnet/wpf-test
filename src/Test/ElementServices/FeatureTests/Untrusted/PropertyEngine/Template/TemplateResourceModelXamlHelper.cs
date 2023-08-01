// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Constructs xaml for TemplateResourceModel.
 * Contributors: Microsoft
 *
 
  
 * Revision:         $Revision: 5 $
 
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

namespace Avalon.Test.CoreUI.PropertyEngine.TemplateResources
{
    /// <summary>
    /// Constructs xaml for the TemplateResourceModel.
    /// </summary>
    internal class TemplateResourceModelXamlHelper
    {
        /// <summary>
        /// Constructs xaml according to the model param values.
        /// </summary>
        /// <returns>A stream of the constructed xaml.</returns>
        public Stream GenerateXaml(TemplateResourceModelState modelState)
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
            tempDoc.Load("TemplateResourceModel_empty.xaml");
            _mainDoc = tempDoc;

            // Load XmlDocument with snippets of xaml to use
            // for contructing the main document. 
            tempDoc = new XmlDocument();
            tempDoc.Load("TemplateResourceModel_elements.xaml");
            _elementsDoc = tempDoc;

            // Hold a reference to the "root" node to use for 
            // inserting xaml into the main document.
            _testRootNode = _mainDoc.SelectSingleNode("//*[@Name='TestRoot']");

            // Handle the various model params to construct the xaml.
            
            _InsertTemplatedNodeParent();
            _InsertTemplatedControl();
            _InsertHowTemplateIsSet();  // Point the templated control at the template.

            _InsertTemplate();
            _InsertKeySet();            // Set the control template key (if any).
            _InsertTargetTypeSet();     // Set the control template target type (if any).

            _InsertTemplateResources();

            // todo: This will create an external dictionary file and add a merged dictionary
            // entry in the _mainDoc. This will also reassign the templateResourceNode to point to the new document.
            // _InsertExternalDictionary();
            

            _InsertStoryboard();
            _InsertBrush();
            _InsertVisualBrush();
            _InsertStyle();
            _InsertViewport3D();
            _InsertXmlDataSource();
            _InsertPropertyTrigger();
            _InsertConflictingResourceName();
            _InsertHasTemplate();
            _InsertHasStyleBasedOn();
            _InsertContentHasResources();

            _RemoveTestIds();

            // Convert XmlDocument to stream.
           return IOHelper.ConvertXmlDocumentToStream(_mainDoc);
        }

        /// <summary>
        /// Insert a wrapper for the templated control as needed and assign the templated node parent
        /// </summary>
        /// <remarks>
        /// This is only really necessary for TableTemplate because the Table must be in 
        /// something like a FlowDocument.
        /// </remarks>
        private void _InsertTemplatedNodeParent()
        {
            if (_state.TemplateType == "TableTemplate")
            {
                //
                // For tables grab the FlowDocument from elements and set it as the templated node parent.
                // Tables must be in an IContentHost.  todo: correct?
                //
                _templatedNodeParent = _elementsDoc.SelectSingleNode("//*[@TestId='TableParent']");

                // Insert the templated node parent.
                // Hold reference to templated node parent.
                _templatedNodeParent = _ImportAndInsertNode(_testRootNode, _templatedNodeParent);

                // Set _templatedNodeParent to the table inside the FlowDocument grabbed from elements.
                _templatedNodeParent = _templatedNodeParent.FirstChild;
            }
            else
            {
                // Otherwise the templated node's parent is the root node.
                _templatedNodeParent = _testRootNode;
            }
        }

        /// <summary>
        /// Insert the element/control that will be templated.
        /// </summary>
        private void _InsertTemplatedControl()
        {
            // Grab templated control from elements doc.
            XmlNode templatedNode = _elementsDoc.SelectSingleNode("//*[@Name='TemplatedControlType_" + _state.TemplatedControlType + "']", _nsmgr);
            if (templatedNode == null)
            {
                throw new NotSupportedException("TemplatedControlType: " + _state.TemplatedControlType);
            }


            if ((_state.HowTemplateIsSet == "StaticResource") || (_state.HowTemplateIsSet == "DynamicResource"))
            {
                // Insert template property name appropriate for the template type.
                XmlAttribute templateAttribute = _mainDoc.CreateAttribute(_state.TemplatePropertyName);
                templateAttribute.Value = "FOO";
                templatedNode.Attributes.SetNamedItem(templateAttribute);
            }

            if (_state.HowTemplateIsSet != "Style")
            {
                // Remove Style attribute if template is not set in a style.
                templatedNode.Attributes.Remove(templatedNode.Attributes["Style"]);

            }
            else
            {
                // todo: Set style
            }

            // Insert the templated node.
            // Hold reference to templated control node.
            _templatedNode = _ImportAndInsertNode(_templatedNodeParent, templatedNode);
        }

        /// <summary>
        /// Create an external dictionary. 
        /// </summary>
        private void _InsertExternalDictionary()
        {
            throw new NotSupportedException("External dictionary");
        }


        /// <summary>
        /// Add template and top-level child inside template.
        ///     &lt;TemplateType&gt;
        ///         &lt;TopLevelChild /&gt;
        ///     &lt;/TemplateType&gt;
        /// </summary>
        private void _InsertTemplate()
        {
            //
            // Find target xml node to put template in.
            //
            XmlNode targetNode = null;

            // Three possible targets:
            // 1. Insert a template propertytag. Template will be inline (inside templated control).
            // 2. Insert a Resources propertytag. Template set via resource reference.
            // 3. Insert a Resources propertytag with Style for templated control. Template will be set in a Style.

            // Target 1: Target xml node for template inside templated control.
            if (_state.HowTemplateIsSet == "Inline")
            {
                targetNode = _mainDoc.CreateElement(_templatedNode.Prefix, _templatedNode.LocalName + "." + _state.TemplatePropertyName, _templatedNode.NamespaceURI);
                _templatedNode.PrependChild(targetNode);
            }
            // Targets 2/3: Target xml node for template is inside main document resources, as a style or standalone.
            else
            {
                targetNode = _GetResourcesNode();

                // Target 3: Target template inside Style in main document resources.
                if ((_state.HowTemplateIsSet == "Style") || (_state.HowTemplateIsSet == "ImplicitStyle"))
                {
                    XmlNode styleNode = null;

                    // Grab styling code from elements.
                    styleNode = _elementsDoc.SelectSingleNode("//*[@x:Key='styleForSettingTemplate']", _nsmgr);

                    // Change Style target type to match templated node.
                    XmlAttribute targetTypeAttribute = styleNode.Attributes["TargetType"];
                    targetTypeAttribute.Value = targetTypeAttribute.Value.Replace("FOO", _templatedNode.Name);

                    // Change the Style's Setter and change its Property attribute to appropriate template type
                    XmlNode setterNode = styleNode.SelectSingleNode("//*[@TestId='TemplateSetter']", _nsmgr);
                    XmlAttribute propertyAttribute = setterNode.Attributes["Property"];
                    propertyAttribute.Value = propertyAttribute.Value.Replace("FOO", _state.TemplatePropertyName);

                    // Put style inside resource node.
                    styleNode = _ImportAndInsertNode(targetNode, styleNode);

                    // Set target to Setter.Value node in style.
                    targetNode = styleNode.SelectSingleNode("//*[@TestId='TemplateSetterValue']", _nsmgr);
                }
            }

            //
            // Target node aquired, now create the template.
            //

            // Grab template tree root from elements.
            XmlNode templateRootNode = _elementsDoc.SelectSingleNode("//*[@x:Name='TemplateRootType_Panel']", _nsmgr);
            if (templateRootNode == null)
            {
                throw new NotSupportedException("TemplateRootType: Panel");
            }

            // Grab template definition from elements doc and add it to the template root.
            XmlNode templateNode = _elementsDoc.SelectSingleNode("//av:" + _state.TemplateType, _nsmgr);
            if (_state.TemplateType == "TableTemplate")
            {
                // Put template root inside TableCell inside TableRow of TableTemplate
                templateNode.FirstChild.FirstChild.FirstChild.PrependChild(templateRootNode);
            }
            else
            {
                templateNode.PrependChild(templateRootNode);
            }

            //
            // Put the template in the target node. Cache reference to the template node.
            //
            _templateNode = _ImportAndInsertNode(targetNode, templateNode);

            //
            // Get reference to template tree root.
            //
            _templateRootNode = _mainDoc.SelectSingleNode("//*[@x:Name='TemplateRootType_Panel']", _nsmgr);
            if (_templateRootNode == null)
            {
                throw new Exception("Could not find template root");
            }
        }

        /// <summary>
        /// Insert the template resources section in the template.
        /// </summary>
        private void _InsertTemplateResources()
        {
            // Create Template.Resources node.
            XmlNode templateResources = _mainDoc.CreateElement("", _state.TemplateType + ".Resources", _templateNode.NamespaceURI);

            _templateResourceNode = _templateNode.PrependChild(templateResources);
        }

        /// <summary>
        /// Set ControlTemplate's TargetType attribute.
        /// </summary>
        private void _InsertTargetTypeSet()
        {
            if (_state.TemplateType != "ControlTemplate") return;

            XmlAttribute attribute = _templateNode.Attributes["TargetType"];

            attribute.Value = attribute.Value.Replace("FOO", _templatedNode.Name);
        }

        /// <summary>
        /// Set x:Key attribute on ControlTemplate node if it is set as a resource. 
        /// Otherwise remove it.
        /// </summary>
        private void _InsertKeySet()
        {
            XmlAttribute keyAttribute = _templateNode.Attributes["x:Key"];

            if ((_state.HowTemplateIsSet == "StaticResource") || (_state.HowTemplateIsSet == "DynamicResource"))
            {
                keyAttribute.Value = "resKey1";
            }
            else
            {
                _templateNode.Attributes.Remove(keyAttribute);
            }

        }


        /// <summary>
        /// Change templated node attributes to use the template for its visual tree.
        /// </summary>
        private void _InsertHowTemplateIsSet()
        {
            // Acquire template attribute.
            XmlAttribute templateAttribute = _templatedNode.Attributes[_state.TemplatePropertyName];

            // Set template attribute on templated node or remove it as needed.
            if (_state.HowTemplateIsSet == "Inline" || _state.HowTemplateIsSet == "ImplicitStyle" || _state.HowTemplateIsSet == "Style")
            {
                // Remove the template attribute.
                _templatedNode.Attributes.Remove(templateAttribute);
            }
            else 
            {                
                // Static or dynamic resource reference.
                string template = "{" + _state.HowTemplateIsSet + " resKey1}";
                templateAttribute.Value = templateAttribute.Value.Replace("FOO", template);
            }
        }

        private void _InsertStoryboard()
        {
            if (_state.HasStoryboard == "None")
            {
                return;
            }

            _InsertTemplateResourceItem("Storyboard");

            _InsertTemplateTreeItem("Storyboard");

            XmlNode eventTriggerNode = _InsertTemplateTriggerItem("Storyboard");
            XmlAttribute sourceNameAttribute = eventTriggerNode.Attributes["SourceName"];

            sourceNameAttribute.Value = sourceNameAttribute.Value.Replace("FOO", "TemplateTreeItem_Storyboard");
        }

        /// <summary>
        /// 
        /// </summary>
        private void _InsertBrush() 
        {
            if (_state.HasBrush == "None")
            {
                return;
            }

            // Get template tree element with brush reference from elements doc.
            _InsertTemplateResourceItem("Brush_Foreground");
            _InsertTemplateResourceItem("Brush_Background");
            
            // Get button that uses references brush resources from elements doc.
            XmlNode styledNode = _InsertTemplateTreeItem("Brush");

            // Create references to brush resources.
            XmlAttribute foregroundAttribute = styledNode.Attributes["Foreground"];
            foregroundAttribute.Value = foregroundAttribute.Value.Replace("FOO", _state.HasBrush + " TemplateResourceItem_Brush_Foreground");
            XmlAttribute backgroundAttribute = styledNode.Attributes["Background"];
            backgroundAttribute.Value = backgroundAttribute.Value.Replace("FOO", _state.HasBrush + " TemplateResourceItem_Brush_Background");
        }

        /// <summary>
        /// 
        /// </summary>
        private void _InsertVisualBrush()
        {
            if (_state.HasVisualBrush == "None")
            {
                return;
            }

            _InsertTemplateResourceItem("VisualBrush");

            // Get button that references the visual brush resources from elements doc.
            XmlNode styledNode = _InsertTemplateTreeItem("VisualBrush");

            // Create references to the visual brush resource .
            XmlAttribute backgroundAttribute = styledNode.Attributes["Background"];
            backgroundAttribute.Value = backgroundAttribute.Value.Replace("FOO", _state.HasVisualBrush + " TemplateResourceItem_VisualBrush");
        }


        /// <summary>
        /// 
        /// </summary>
        private void _InsertStyle()
        {
            if (_state.HasStyle == "None")
            {
                return;
            }

            // Get style
            _InsertTemplateResourceItem("Style");

            // Get element that uses style resource
            XmlNode styledNode = _InsertTemplateTreeItem("Style");

            // Create reference to style resource.
            XmlAttribute styleAttribute = styledNode.Attributes["Style"];
            styleAttribute.Value = styleAttribute.Value.Replace("FOO", _state.HasStyle + " TemplateResourceItem_Style");
        }

        /// <summary>
        /// 

        private void _InsertViewport3D()
        {
            if (_state.HasViewport3D == "None")
            {
                return;
            }

            // Get the model.
            _InsertTemplateResourceItem("Viewport3D");

            // Get element that uses the model.
            _InsertTemplateTreeItem("Viewport3D");

            // 


        }

        private void _InsertXmlDataSource()
        {
            if (_state.HasXmlDataSource == "None")
            {
                return;
            }

            // Get data source resource.
            _InsertTemplateResourceItem("XmlDataSource");

            // Get element that references the XmlDataSource resource.
            XmlNode boundNode = _InsertTemplateTreeItem("XmlDataSource");

            // Create DataBinding with resource reference to XmlDataSource
            XmlAttribute backgroundAttribute = boundNode.Attributes["Background"];
            backgroundAttribute.Value = backgroundAttribute.Value.Replace("FOO", "Binding Source={" + _state.HasXmlDataSource + " TemplateResourceItem_XmlDataSource}, XPath=Color");
        }



        private void _InsertPropertyTrigger()
        {
            if (_state.HasPropertyTrigger == "None")
            {
                return;
            }

            _InsertTemplateResourceItem("PropertyTrigger_Orange");
            _InsertTemplateResourceItem("PropertyTrigger_Black");

            _InsertTemplateTreeItem("PropertyTrigger");

            // Get and insert trigger node.
            XmlNode triggerNode = _InsertTemplateTriggerItem("PropertyTrigger");

            // Set resource reference in the Trigger's Setters.
            foreach (XmlNode setterNode in triggerNode.ChildNodes)
            {
                XmlAttribute valueAttribute = setterNode.Attributes["Value"];
                valueAttribute.Value = valueAttribute.Value.Replace("FOO", _state.HasPropertyTrigger);
            }

        }

        private void _InsertConflictingResourceName()
        {
            if (_state.HasConflictingResourceName == "None")
            {
                return;
            }

            _InsertTemplateResourceItem("ConflictingResourceName");

            // Make the Key's the same.
            XmlNode resourceNode = _InsertResourceItem("ConflictingResourceName");
            XmlAttribute keyAttribute = resourceNode.Attributes["x:Key"];
            keyAttribute.Value = keyAttribute.Value.Replace("ResourceItem", "TemplateResourceItem");

            // Set reference type.
            XmlNode conflictingNode = _InsertTemplateTreeItem("ConflictingResourceName");
            XmlAttribute backgroundAttribute = conflictingNode.Attributes["Background"];
            backgroundAttribute.Value = backgroundAttribute.Value.Replace("FOO", _state.HasConflictingResourceName);
        }

        private void _InsertHasTemplate()
        {
            if (_state.HasTemplate == "None")
            {
                return;
            }

            _InsertTemplateResourceItem("Template");

            XmlNode templatedTreeNode = _InsertTemplateTreeItem("Template");
            XmlAttribute templateAttribute = templatedTreeNode.Attributes["Template"];
            templateAttribute.Value = templateAttribute.Value.Replace("FOO", _state.HasTemplate + " TemplateResourceItem_Template");
        }

        private void _InsertHasStyleBasedOn()
        {
            if (_state.HasStyleBasedOn == "None")
            {
                return;
            }

            XmlNode styledNode = _InsertTemplateTreeItem("BasedOnStyle");
            XmlAttribute styleAttribute = styledNode.Attributes["Style"];
            styleAttribute.Value = styleAttribute.Value.Replace("FOO", _state.HasStyleBasedOn);

            _InsertTemplateResourceItem("BasedOnStyleBase");
            _InsertTemplateResourceItem("BasedOnStyle");
        }

        private void _InsertContentHasResources()
        {
            if (_state.ContentHasResources == "false")
            {
                return;
            }

            // Insert Element that contains Element.Resources section
            _InsertTemplateTreeItem("ContentHasResources");

            // Insert element in template resources with conflicting resource name.
            _InsertTemplateResourceItem("TemplateContentResource");
        }

        //
        // Helper functions
        //

        /// <summary>
        /// Helper function.
        /// </summary>
        /// <param name="itemName">Item name substring.</param>
        /// <returns>Pointer to inserted node.</returns>
        private XmlNode _InsertTemplateResourceItem(string itemName)
        {
            if (_templateResourceNode == null)
            {
                throw new Exception("Template resource node not created.");
            }

            if (itemName == null)
            {
                throw new ArgumentNullException("itemName");
            }

            // Select a template resource item from elements.
            XmlNode resourceItem = _elementsDoc.SelectSingleNode("//*[@x:Key='TemplateResourceItem_" + itemName + "']", _nsmgr);
            if (resourceItem == null)
            {
                throw new NotSupportedException("Template resource: " + itemName);
            }

            return _ImportAndInsertNode(_templateResourceNode, resourceItem);
        }

        /// <summary>
        /// Helper function.
        /// </summary>
        /// <param name="itemName">Item name substring.</param>
        /// <returns>Pointer to inserted node.</returns>
        private XmlNode _InsertResourceItem(string itemName)
        {
            XmlNode _resourceNode = _GetResourcesNode();
            if (_resourceNode == null)
            {
                throw new Exception("Resource node not created.");
            }

            if (itemName == null)
            {
                throw new ArgumentNullException("itemName");
            }

            // Select a template resource item from elements.
            XmlNode resourceItem = _elementsDoc.SelectSingleNode("//*[@x:Key='ResourceItem_" + itemName + "']", _nsmgr);
            if (resourceItem == null)
            {
                throw new NotSupportedException("Resource: " + itemName);
            }

            return _ImportAndInsertNode(_resourceNode, resourceItem);
        }
        /// <summary>
        /// Helper function. Looks up template tree item by Name in elements doc and inserts
        /// it in the template.
        /// </summary>
        /// <param name="itemName">Item name substring.</param>
        /// <returns>Pointer to inserted node.</returns>
        private XmlNode _InsertTemplateTreeItem(string itemName)
        {
            if (_templateRootNode == null)
            {
                throw new Exception("Template root node not created.");
            }

            if (itemName == null)
            {
                throw new ArgumentNullException("itemName");
            }

            // Select a template tree item from elements.
            XmlNode treeItem = _elementsDoc.SelectSingleNode("//*[@x:Name='TemplateTreeItem_" + itemName + "']", _nsmgr);
            if (treeItem == null)
            {
                throw new NotSupportedException("Template tree element: " + itemName);
            }

            return _ImportAndInsertNode(_templateRootNode, treeItem);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemName">Item name substring.</param>
        /// <returns>Pointer to inserted node.</returns>
        private XmlNode _InsertTemplateTriggerItem(string itemName)
        {
            _templateTriggerNode = _GetTriggersNode();

            if (_templateTriggerNode == null)
            {
                throw new Exception("Template trigger node not created.");
            }

            if (itemName == null)
            {
                throw new ArgumentNullException("itemName");
            }

            XmlNode triggerItem = _elementsDoc.SelectSingleNode("//*[@TestId='TemplateTriggerItem_" + itemName + "']", _nsmgr);
            if (triggerItem == null)
            {
                throw new NotSupportedException("Template trigger item: " + itemName);
            }

            XmlNode insertedNode = _ImportAndInsertNode(_templateTriggerNode, triggerItem);
            if (insertedNode == null)
            {
                throw new Exception("Could not import and insert node: " + itemName);
            }
            return insertedNode;
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

        // Holds the model instance that works with this helper.
        private TemplateResourceModelState _state = null;

        // Necessary for xpath queries.
        private XmlNamespaceManager _nsmgr = null;

        // Convenient references to xml nodes used throughout 
        // the xaml construction routines.
        private XmlDocument _elementsDoc = null;
        private XmlDocument _mainDoc = null;
        private XmlNode _testRootNode = null;
        private XmlNode _templatedNodeParent = null;
        private XmlNode _templateNode = null;
        private XmlNode _templatedNode = null;
        private XmlNode _templateRootNode = null;
        private XmlNode _templateResourceNode = null;
        private XmlNode _templateTriggerNode = null;
    }
}
