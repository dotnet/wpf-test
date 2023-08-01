// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Constructs xaml for TemplateModel.
 * Contributors: Microsoft
 *
 
  
 * Revision:         $Revision: 1 $
 
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

namespace Avalon.Test.CoreUI.PropertyEngine.Template
{
    /// <summary>
    /// Constructs xaml for the TemplateModel.
    /// </summary>
    internal class TemplateModelXamlHelper
    {
        /// <summary>
        /// Constructs xaml according to the model param values.
        /// </summary>
        /// <returns>A stream of the constructed xaml.</returns>
        public Stream GenerateXaml(TemplateModelState modelState)
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
            tempDoc.Load("TemplateModel_empty.xaml");
            _mainDoc = tempDoc;

            // Create XmlDocument with snippets of xaml to use
            // for contructing the main document.tempDoc = new XmlDocument();
            tempDoc = new XmlDocument();
            tempDoc.Load("TemplateModel_elements.xaml");
            _elementsDoc = tempDoc;

            // Hold a reference to the "root" node to use for 
            // inserting xaml into the main document.
            _testRootNode = _mainDoc.SelectSingleNode("//*[@Name='TestRoot']");

            // Handle the various model params to construct the xaml.
            _InsertTemplatedNodeParent();
            _InsertTemplatedControl();
            
            // Insert the Template.
            _InsertTemplateParent();
            _InsertTemplate();
            // Inserted the Template.
            
            // Insert template contents.
            _InsertTemplateRootType();
            _InsertTemplateChildType();
            _InsertDoesReferToStyle();

            _InsertHasPropertyTrigger();
            _InsertHasEventTrigger();
            _InsertHasEventSet();
            _InsertHasTemplateBind();

            // Insert additional stuff.
            // 
            _InsertDataSourceType();

            _RemoveTestIds();

            // Convert XmlDocument to stream.
            return IOHelper.ConvertXmlDocumentToStream(_mainDoc);
        }

        /// <summary>
        /// Insert a wrapper for the templated control as needed and assign the templated node parent.
        /// </summary>
        private void _InsertTemplatedNodeParent()
        {
            // This did more when TableTemplates existed.

            // The test root node is the templated node parent.
            _templatedNodeParent = _testRootNode;
        }

        /// <summary>
        /// Insert the element/control that will be templated.
        /// </summary>
        private void _InsertTemplatedControl()
        {
            // Grab templated control element from elements.
            XmlNode templatedNode = _elementsDoc.SelectSingleNode("//*[@Name='TemplatedControlType_" + _state.TemplatedControlType + "']", _nsmgr);
            if (templatedNode == null)
            {
                throw new NotSupportedException("TemplatedControlType: " + _state.TemplatedControlType);
            }

            // Remove data binding attribute.
            if (!_state.HasDataBinding)
            {
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

                // Add logical child to Item control when data templating WITHOUT a data binding.
                // 
                if ((_state.TemplateType == "DataTemplate") && (_state.TemplatedControlType == "ItemsControl"))
                {
                    // Add a logical child to ItemsControl for the DataTemplate to apply to.
                    // Add an item to ListBox
                    templatedNode.InnerText = "Foo";
                }
            }
            else
            {
                // Remove logical children of the templated control if doing a data bind.
                templatedNode.InnerText = String.Empty;
            }

            // Remove the Background property if template triggers target the templated parent.
            // Template triggers (like style triggers) are overridden by settings in the templated control.
            if (_state.TriggerTarget == "TemplatedParent")
            {
                templatedNode.Attributes.Remove(templatedNode.Attributes["Background"]);
            }

            // 

            if (_state.HowTemplateIsSet == "DynamicResource" || _state.HowTemplateIsSet == "StaticResource")
            {
                // Create template property attribute according to the template type.
                XmlAttribute templateAttribute = _elementsDoc.CreateAttribute(_state.TemplatePropertyName);
                templateAttribute.Value = "{" + _state.HowTemplateIsSet + " resKey1}";
                templatedNode.Attributes.Prepend(templateAttribute);
            }

            // Remove style attribute if template is not set by a style.
            if (_state.HowTemplateIsSet != "Style")
            {
                templatedNode.Attributes.Remove(templatedNode.Attributes["Style"]);
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

            if (_state.HowTemplateIsSet == "Inline") 
            {
                // Template is inline in templated control, create <Element.Template/>
                _templateNodeParent = _mainDoc.CreateElement(_templatedNode.Prefix, _templatedNode.LocalName + "." + _state.TemplatePropertyName, _templatedNode.NamespaceURI);
                _templatedNode.PrependChild(_templateNodeParent);
            }
            else if (_state.HowTemplateIsSet == "InlineStyle")
            {
                // Template is in a style in templated control, create <Element.Style/>
                _templateNodeParent = _mainDoc.CreateElement(_templatedNode.Prefix, _templatedNode.LocalName + ".Style", _templatedNode.NamespaceURI);
                _templatedNode.PrependChild(_templateNodeParent);
            }
            else
            {
                // Template is in the page resources, possibly in a style.
                _templateNodeParent = _GetResourcesNode();
            }

            //
            // If template is in a style insert a style with template setter in the parent and make <Setter.Value> the template parent.
            //

            if ((_state.HowTemplateIsSet == "Style") || (_state.HowTemplateIsSet == "InlineStyle"))
            {
                // Grab styling code from elements.
                XmlNode styleNode = _elementsDoc.SelectSingleNode("//*[@x:Key='styleForSettingTemplate']", _nsmgr);

                // Change Style target type to match templated node.
                XmlAttribute targetTypeAttribute = styleNode.Attributes["TargetType"];
                targetTypeAttribute.Value = targetTypeAttribute.Value.Replace("FOO", _templatedNode.Name);

                // Remove x:Key="styleForSettingTemplate" if style is inline.
                XmlAttribute keyAttribute = styleNode.Attributes["x:Key"];
                if (_state.HowTemplateIsSet == "InlineStyle")
                {
                    styleNode.Attributes.Remove(keyAttribute);
                }

                // Change the Style's Setter Property attribute to appropriate template type
                XmlNode setterNode = styleNode.SelectSingleNode("//*[@TestId='TemplateSetter']", _nsmgr);
                XmlAttribute propertyAttribute = setterNode.Attributes["Property"];
                propertyAttribute.Value = propertyAttribute.Value.Replace("FOO", _state.TemplatePropertyName);

                // Put style inside current template parent
                styleNode = _ImportAndInsertNode(_templateNodeParent, styleNode);

                // Make the template parent the Setter.Value node in style.
                // 
                _templateNodeParent = styleNode.SelectSingleNode("//*[@TestId='TemplateSetterValue']", _nsmgr);
            }
                
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
            if (_state.IsKeySet)
            {
                keyAttribute.Value = "resKey1";
            }
            else
            {
                templateNode.Attributes.Remove(keyAttribute);
            }
            
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
            XmlNode templateRootNode = _elementsDoc.SelectSingleNode("//*[@x:Name='TemplateRootType_" + _state.TemplateRootType + "']", _nsmgr);
            if (templateRootNode == null)
            {
                throw new NotSupportedException("TemplateRootType: " + _state.TemplateRootType);
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
            if (_state.TemplateChildType == "None")
            {
                return;
            }

            // Grab child code from elements.
            XmlNode templateChildNode = null;
            if (_state.HasDataBinding)
            {
                templateChildNode = _elementsDoc.SelectSingleNode("//*[@x:Name='TemplateChildType_" + _state.TemplateChildType + "_WithBinding']", _nsmgr);
            }
            else
            {
                templateChildNode = _elementsDoc.SelectSingleNode("//*[@x:Name='TemplateChildType_" + _state.TemplateChildType + "']", _nsmgr);
            }

            if (templateChildNode == null)
            {
                throw new NotSupportedException("TemplateChildType: " + _state.TemplateChildType);
            }

            // Insert template child under template root in main doc.
            _templateChildNode = _ImportAndInsertNode(_templateRootNode, templateChildNode);
        }


        /// <summary>
        /// Inserts xaml for a Style reference from the template child.
        /// </summary>
        private void _InsertDoesReferToStyle()
        {
            // 

            if (_state.DoesReferToStyle)
            {
                // Insert Style node and set the template child type as the Style's TargetType.
                XmlNode styleNode = _elementsDoc.SelectSingleNode("//av:Style", _nsmgr);

                string targetType = styleNode.Attributes["TargetType"].Value;
                targetType = targetType.Replace("FOO", _templateChildNode.Name);
                styleNode.Attributes["TargetType"].Value = targetType;

                _ImportAndInsertNode(_GetResourcesNode(), styleNode);
            }
            else if (_state.TemplateChildType != "None")
            {
                // Remove the Style attribute if necessary.
                XmlAttribute styleAttrib = _templateChildNode.Attributes["Style"];
                if (styleAttrib != null)
                {
                    _templateChildNode.Attributes.Remove(styleAttrib);
                }
            }
        }

        /// <summary>
        /// Inserts xaml for the property Trigger.
        /// </summary>
        private void _InsertHasPropertyTrigger()
        {
            if (!_state.HasPropertyTrigger) return;

            // Retrieve property Trigger node from elements.
            XmlNode propTriggerNode;
            if (_state.TriggerTarget == "TemplateChild")
            {
                propTriggerNode = _elementsDoc.SelectSingleNode("//*[@TestId='ChildPropertyTrigger_" + _state.TemplateChildType + "']", _nsmgr);
            }
            else
            {
                propTriggerNode = _elementsDoc.SelectSingleNode("//*[@TestId='PropertyTrigger_" + _state.TemplateRootType + "']", _nsmgr);
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

            //
            // Set TargetName value.
            //

            // Select all the setters in the template.
            XmlNodeList setNodes = propTriggerNode.SelectNodes("./av:Setter", _nsmgr);

            // Set the trigger target and source properties as appropriate.
            for (int i = 0; i < setNodes.Count; i++)
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

            // 

            // Insert the Trigger in the Triggers node.
            propTriggerNode = _ImportAndInsertNode(_GetTriggersNode(), propTriggerNode);

            setNodes = propTriggerNode.SelectNodes("./av:Setter", _nsmgr);

            //
            // Adjust the first Setter to contain a Freezable with a dynamic resource.
            //

            if (_state.HasFreezableTriggerSetter)
            {
                _InsertFreezableSetterValue(setNodes[0]);
            }


        }

        private void _InsertFreezableSetterValue(XmlNode setterNode)
        {
            
            string value = setterNode.Attributes["Value"].Value;

            // Remove Value attribute from setterNode
            setterNode.Attributes.Remove(setterNode.Attributes["Value"]);

            // Add Setter.Value
            XmlNode setterValueNode = _mainDoc.CreateElement("Setter.Value", setterNode.NamespaceURI);
            setterValueNode = setterNode.PrependChild(setterValueNode);

            // Add SolidColorBrush
            XmlNode brushNode = _mainDoc.CreateElement("SolidColorBrush", setterValueNode.NamespaceURI);
            brushNode = setterValueNode.PrependChild(brushNode);

            // Set brush Color
            XmlAttribute colorAttribute = _mainDoc.CreateAttribute("Color");
            switch (_state.FreezableSetterValue)
            {
                case "Local":
                    // Set brush Color to old setterNode value.
                    colorAttribute.Value = value;
                    break;

                case "DynamicResource":
                    // Set brush color to DynamicResource to brush in resources.
                    colorAttribute.Value = "{DynamicResource myColor}";
                    break;

                case "StaticResource":
                    // Set brush color to StaticResource to brush in resources.
                    colorAttribute.Value = "{StaticResource myColor}";
                    break;

                case "Binding":
                    // Set brush color to Binding.
                    throw new NotSupportedException("Freezable setter value: Binding");

                default:
                    throw new NotSupportedException(_state.FreezableSetterValue);
            }
            brushNode.Attributes.Prepend(colorAttribute);
        }


        /// <summary>
        /// Set TemplateBinding extension on a property in the template tree.
        /// </summary>
        private void _InsertHasEventTrigger()
        {
            if (_state.HasEventTrigger == "None") return;

            // Select event trigger and storyboard from elements.
            XmlNode eventtriggerNode = _elementsDoc.SelectSingleNode("//av:EventTrigger", _nsmgr);

            // Change RoutedEvent as appropriate
            if (_state.HasEventTrigger == "Loaded")
            {
                eventtriggerNode.Attributes["RoutedEvent"].Value = "FrameworkElement.Loaded";
            }

            // Make EventTrigger SourceName point at template root node.
            XmlAttribute targetAttrib = eventtriggerNode.Attributes["SourceName"];
            targetAttrib.Value = targetAttrib.Value.Replace("FOO", _templateRootNode.Attributes["x:Name"].Value);

            // Make both ColorAnimation Storyboard.TargetName's point at template root node.
            XmlNodeList animationNodes = eventtriggerNode.SelectNodes(".//av:ColorAnimation", _nsmgr);

            for (int i = 0; i < animationNodes.Count; i++)
            {
                XmlAttribute targetNameAttrib = animationNodes[i].Attributes["Storyboard.TargetName"];
                targetNameAttrib.Value = targetNameAttrib.Value.Replace("FOO", _templateRootNode.Attributes["x:Name"].Value);
            }

            // Put the event trigger in Triggers.
            _ImportAndInsertNode(_GetTriggersNode(), eventtriggerNode);
        }


        /// <summary>
        /// Set event handler attribute(s) on template root node.
        /// </summary>
        private void _InsertHasEventSet()
        {
            if (!_state.HasEventSet) return;

            // Add event attribute to template root node.
            XmlAttribute eventAttrib = _mainDoc.CreateAttribute("MouseLeave");
            eventAttrib.Value = "OnMouseLeave";
            _templateRootNode.Attributes.Append(eventAttrib);

            // Select node from element file containing code.
            XmlNode eventCodeNode = _elementsDoc.SelectSingleNode("//*[@TestId='EventCode']", _nsmgr);

            // Insert code under document root, code is not allowed inside the template.
            _ImportAndInsertNode(_testRootNode, eventCodeNode);
        
        }

        /// <summary>
        /// Set TemplateBinding extension on a property in the template tree.
        /// </summary>
        private void _InsertHasTemplateBind()
        {
            // 



            if (!_state.HasTemplateBind) return;

            // Select attribute in templated control that is not Name, Template or Style... (Background).
            XmlAttribute targetAttrib = (XmlAttribute)_templatedNode.SelectSingleNode("@*[name() != 'Name' and name() != 'Template' and name() != 'Style']", _nsmgr);

            // Get attribute in template root that will have a TemplateBinding.
            XmlAttribute attrib = (XmlAttribute)_templateRootNode.SelectSingleNode("@*[name() = '" + targetAttrib.Name + "']", _nsmgr);

            // Apply template binding.
            attrib.Value = "{TemplateBinding " + targetAttrib.Name + "}";
        }


        //
        // Additional stuff (extra resources)
        // 

        /// <summary>
        /// Inserts a data source for use by a data template.
        /// </summary>
        private void _InsertDataSourceType()
        {
            if (_state.DataSourceType == "XmlDataSource")
            {
                // Select data source from elements.
                XmlNode dataSourceNode = _elementsDoc.SelectSingleNode("//*[@TestId='XmlDataSource']", _nsmgr);

                _ImportAndInsertNode(_GetResourcesNode(), dataSourceNode);
            }
            else if (_state.DataSourceType == "ObjectDataSource")
            {
                throw new NotSupportedException("DataSourceType: " + _state.DataSourceType);
            }
        }

        //
        // Helper functions.
        //

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
        
        //
        // Private Fields
        //

        // Holds the model instance that works with this helper.
        private TemplateModelState _state = null;

        // Necessary for xpath queries.
        private XmlNamespaceManager _nsmgr = null;

        // Convenient references to xml nodes used throughout 
        // the xaml construction routines.
        private XmlDocument _elementsDoc = null;
        private XmlDocument _mainDoc = null;
        private XmlNode _testRootNode = null;
        private XmlNode _templatedNodeParent = null;
        private XmlNode _templatedNode = null;
        private XmlNode _templateNodeParent = null;
        private XmlNode _templateNode = null;
        
        private XmlNode _templateRootNode = null;
        private XmlNode _templateChildNode = null;
        // private XmlNode _templateTriggers = null;
        // private XmlNode _templateResources = null;
    }
}
