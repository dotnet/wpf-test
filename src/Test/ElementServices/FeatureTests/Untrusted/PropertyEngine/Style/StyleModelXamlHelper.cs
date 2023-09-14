// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Constructs Xaml for StyleModel.
 * Contributors: Microsoft
 *
 
  
 * Revision:         $Revision: 5 $
 
********************************************************************/
using System;
using System.IO;
using System.Xml;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.Xaml.Markup;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Modeling;
using Microsoft.Test.Serialization;


namespace Avalon.Test.CoreUI.PropertyEngine.StyleModel
{
    /// <summary>
    /// Constructs xaml for the StyleModel.
    /// </summary>
    internal class StyleModelXamlHelper
    {
        /// <summary>
        /// Generate xaml according to the model state values.
        /// </summary>
        /// <returns>A stream of the constructed xaml.</returns>
        public Stream GenerateXaml(StyleModelState modelState)
        {
            // Save the model state.
            _state = modelState;

            _testDoc = new XamlTestDocument("StyleModel_empty.xaml", "StyleModel_elements.xaml");
       
            _testRootNode = _testDoc.TestRoot;

            //
            // Handle the various model params to construct the xaml.
            //

            // Add Color to resources section.
            if ((_state.FreezableSetterValue == "DynamicResource") || (_state.FreezableSetterValue == "StaticResource"))
            {
                // Put color in Page resources.
                _testDoc.ImportAndAppendElement((XmlElement)_testRootNode.ParentNode.FirstChild, "myColor");
            }

            _SetStyledItemName();

            // Insert the styled item and its optional parent.
            _InsertStyledItemParent();
            _InsertStyledItem();

            // Insert the optional base style.
            _InsertBase();

            // Insert the style that will be applied to the styled item.
            _InsertStyle();

            // Insert style setters.
            _InsertSetter();

            // Insert event triggers.
            _InsertEventTrigger();

            // Insert property triggers.
            _InsertPropertyTrigger();

            // Insert template.
            _InsertTemplate();

            // Insert data trigger.
            _InsertDataTrigger();

            // Insert Style.Resource sections.
            _InsertStyleResources();
           

            // Remove the stub test id attributes used to make grabbing nodes from the element doc easier.
            _testDoc.RemoveTestIds();

            // Convert XmlDocument to stream.
            return IOHelper.ConvertXmlDocumentToStream(_testDoc);
        }

        /// <summary>
        /// Set the item name used to test the model styled item type.
        /// </summary>
        private void _SetStyledItemName()
        {
            switch (_state.StyledItem)
            {
                case "FrameworkElement":
                    _styledItemName = "Button";
                    break;
                case "CustomFrameworkElement":
                    _styledItemName = "cmn:CustomButton";
                    break;
                case "FrameworkContentElement":
                    _styledItemName = "Bold";
                    break;
                default:
                    throw new NotSupportedException(_state.StyledItem);                    
            }
        }

        /// <summary>
        /// Insert a parent of styled item if necessary or just set _styledItemParent to the test root.
        /// </summary>
        private void _InsertStyledItemParent()
        {
            if (_state.StyledItem == "FrameworkContentElement")
            {
                // Insert a textblock for the styled FCE.
                _styledNodeParent = _testDoc.ImportAndAppendElement(_testRootNode, "FrameworkContentElementParent");
            }
            else
            {
                // Default: The styled node's parent is the root node.
                _styledNodeParent = _testRootNode;
            }
        }

        /// <summary>
        /// Insert the item that will be styled.
        /// </summary>
        private void _InsertStyledItem()
        {
             // Grab styled item from elements doc.
            _styledNode = _testDoc.GetSnippetByXPath("//*[@Name='StyledItem_" + _state.StyledItem + "']");
            if (_styledNode == null)
            {
                throw new NotSupportedException("Could not find StyledItem: " + _state.StyledItem);
            }

            // Remove style attribute if style is inline or implicit.
            if ((_state.StyleLocation == "ElementInline") || (_state.StyleHasKey == false))
            {
                // Remove Style attribute if Style is set implicitly or inline.
                _testDoc.RemoveAttribute(_styledNode, "Style");
            }

            // Style is in a resource, set reference.
            else if ((_state.StyleReference == "StaticResource") || (_state.StyleReference == "DynamicResource"))
            {
                _testDoc.ReplaceAttributeSubstring(_styledNode, "Style", "FOO", _state.StyleReference);
            }

            // Remove content if item is data bound.
            // 
            if (_state.HasDataTrigger != "None")
            {
                _styledNode.InnerText = String.Empty;
            }
        
            // Insert the styled node in the test doc and hold reference to it.
            _styledNode = _testDoc.ImportAndAppendElement(_styledNodeParent, _styledNode);
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
            if (_state.StyleLocation == "PageResource")
            {
                _styleParentNode = (XmlElement)_testRootNode.ParentNode.FirstChild;

                if (_styleParentNode == null)
                {
                    throw new Exception("Could not find page resource element");
                }
            }

            // Get the resource section of the styled item's parent.
            else if (_state.StyleLocation == "ParentResource") 
            {
                // Add resources to the test root.
                _styleParentNode = _testDoc.AddElement(_testRootNode, "StackPanel.Resources");
            }

            // Get the resources section in the styled item.
            else if (_state.StyleLocation == "ElementResource")
            {
                if (_state.StyledItem == "CustomFrameworkElement")
                {
                    // Custom element must be handled differently because it is from a different namespace.
                    _styleParentNode = (XmlElement)_styledNode.SelectSingleNode("//"+ _styledItemName + ".Resources", _testDoc.Nsmgr);

                    if (_styleParentNode == null)
                    {
                        // Create new node in main doc.
                        _styleParentNode = _testDoc.CreateElement(_styledItemName + ".Resources", _styledNode.NamespaceURI);

                        // Add the new node under the parent.
                        _styleParentNode = (XmlElement)_styledNode.AppendChild(_styleParentNode);
                    }
                }
                else
                {
                    _styleParentNode = _testDoc.AddElement(_styledNode, _styledItemName + ".Resources");
                }
            }

            // Get an inline item.Style element to parent the style.
            else if (_state.StyleLocation == "ElementInline")
            {
                if (_state.StyledItem == "CustomFrameworkElement")
                {
                    //_styleParentNode = FindOrAddNode("cmn:" + _styledItemName + ".Style", _styledNode);
                    _styleParentNode = (XmlElement)_styledNode.SelectSingleNode("//" + _styledItemName + ".Style", _testDoc.Nsmgr);

                    if (_styleParentNode == null)
                    {
                        // Create new node in main doc.
                        _styleParentNode = (XmlElement)_testDoc.CreateElement(_styledItemName + ".Style", _styledNode.NamespaceURI);

                        // Add the new node under the parent.
                        _styleParentNode = (XmlElement)_styledNode.AppendChild(_styleParentNode);
                    }
                }
                else
                {
                    _styleParentNode = (XmlElement)_testDoc.CreateElement(_styledItemName + ".Style", _styledNode.NamespaceURI);

                    // Add the new node under the parent.
                    _styleParentNode = (XmlElement)_styledNode.AppendChild(_styleParentNode);
                }
            }

            // Oops, don't know where to put the style.
            else
            {
                throw new NotSupportedException(_state.StyleLocation);
            }

            //
            // Grab the style node from elements, edit, and insert it.
            //

            _styleNode = _testDoc.GetSnippet("StyleNode");

            // Remove key attribute.
            if (!_state.StyleHasKey)
            {
                _testDoc.RemoveAttribute(_styleNode, "x:Key");
            }

            // Change or remove TargetType attribute.
            if (_state.StyleHasTargetType)
            {
                _testDoc.ReplaceAttributeSubstring(_styleNode, "TargetType", "FOO", _styledItemName);
            }
            else
            {
                _testDoc.RemoveAttribute(_styleNode, "TargetType");
            }

            // Change or remove style's BasedOn attribute.
            if (_state.BaseStyleLocation == "None")
            {
                // Location "None" implies base style has nowhere to be and therefore isn't :)
                _testDoc.RemoveAttribute(_styleNode, "BasedOn");
            }
            else
            {
                // There is a base style somewhere, set the BasedOn value appropriately (implicit vs. explicit).
                string basedOnName = String.Empty;
                if (_state.BaseHasKey)
                {
                    basedOnName = "testBaseStyle";
                }
                else
                {
                    // Set BasedOn attribute to TargetType Key.
                    basedOnName = "{x:Type " + _styledItemName + "}";
                }

                _testDoc.ReplaceAttributeSubstring(_styleNode, "BasedOn", "FOO", basedOnName);
            }

            
            //
            // Insert the style!
            //

            _styleNode = _testDoc.ImportAndAppendElement(_styleParentNode, _styleNode);

        }

        /// <summary>
        /// Insert the base style that the applied style will derive from.
        /// </summary>
        private void _InsertBase()
        {
            //
            // Get reference to parent node the style will be put in...
            //

            switch (_state.BaseStyleLocation)
            {
                // No base style.
                case "None":
                    return;
                    
                // Put base style in document root.
                case "PageResource":
                    _baseStyleParentNode = (XmlElement)_testRootNode.ParentNode.FirstChild;

                    if (_baseStyleParentNode == null)
                    {
                        throw new Exception("Could not find page resource element.");
                    }
                    break;

                case "ParentResource":                    
                    _baseStyleParentNode = _testDoc.AddElement(_testRootNode, "StackPanel.Resources");

                    break;
                    
                case "ElementResource":
                    // Find or add Resources node
                    if (_state.StyledItem == "CustomFrameworkElement")
                    {
                        _baseStyleParentNode = (XmlElement)_styledNode.SelectSingleNode("//" + _styledItemName + ".Resources", _testDoc.Nsmgr);

                        if (_baseStyleParentNode == null)
                        {
                            // Create new node in main doc.
                            _baseStyleParentNode = (XmlElement)_testDoc.CreateElement(_styledItemName + ".Resources", _styledNode.NamespaceURI);

                            // Add the new node under the parent.
                            _baseStyleParentNode = (XmlElement)_styledNode.PrependChild(_baseStyleParentNode);
                        }
                    }
                    else
                    {
                        _baseStyleParentNode = _testDoc.AddElement(_styledNode, _styledItemName + ".Resources");
                    }
                    
                    break;
                    
                default:
                    throw new NotSupportedException(_state.BaseStyleLocation);
                    
            }

            //
            // Grab, edit, and insert the base style node.
            //
            _baseStyleNode = _testDoc.GetSnippet("BaseStyleNode");

            if (!_state.BaseHasKey)
            {
                _testDoc.RemoveAttribute(_baseStyleNode, "x:Key");
            }

            if (_state.BaseHasTargetType)
            {
                // Set TargetType to the styled item type.
                _testDoc.ReplaceAttributeSubstring(_baseStyleNode, "TargetType", "FOO", _styledItemName);
            }
            else
            {
                // Remove TargetType attribute.
                _testDoc.RemoveAttribute(_baseStyleNode, "TargetType");
            }

            _baseStyleNode = _testDoc.ImportAndAppendElement(_baseStyleParentNode, _baseStyleNode);

        

        }

        /// <summary>
        /// Insert Setter in base and derived style.
        /// </summary>
        private void _InsertSetter()
        {
            if (_state.HasSetter == "None")
            {
                return;
            }

            // Returned if there is not setter. So there is one, in the base, derived, or both.

            // Add setter to base style if not only in derived style.
            if ((_state.HasSetter != "InDerivedStyle") && (_state.BaseStyleLocation != "None"))
            {
                XmlElement baseSetterNode = _testDoc.ImportAndAppendElement(_baseStyleNode, "BaseSetter");

                if (_state.FreezableSetter)
                {
                        _InsertFreezableSetterValue(baseSetterNode, _baseStyleNode);
                }

                // Qualify the property name in the setter with the target type if the Style doesn't.
                if (!_state.BaseHasTargetType)
                {
                    XmlAttribute propertyAttribute = baseSetterNode.Attributes["Property"];

                    propertyAttribute.Value = propertyAttribute.Value.Insert(0, _styledItemName + ".");
                }

                // Make setter value a reference to Style.Resource
                if (_state.StyleResources == "InBaseStyle" || _state.StyleResources == "OverrideBase" || _state.StyleResources == "Both")
                {
                    _testDoc.ReplaceAttributeSubstring(baseSetterNode, "Value", "Red", "{StaticResource baseStyleResource}");
                }

            }

            // Add setter to main style.
            if (_state.HasSetter != "InBaseStyle") // (Setters are not in base style only...)
            {
                XmlElement setterNode = _testDoc.ImportAndAppendElement(_styleNode, "Setter");

                if (_state.FreezableSetter)
                {
                    _InsertFreezableSetterValue(setterNode, _styleNode);
                }

                if (!_state.StyleHasTargetType)
                {
                    XmlAttribute propertyAttribute = setterNode.Attributes["Property"];

                    propertyAttribute.Value = propertyAttribute.Value.Insert(0, _styledItemName + ".");
                }

                // Make value a resource reference to Style.Resources.
                if (_state.StyleResources == "InDerivedStyle" || _state.StyleResources == "OverrideBase" || _state.StyleResources == "Both")
                {
                    _testDoc.ReplaceAttributeSubstring(setterNode, "Value", "Yellow", "{StaticResource styleResource}");
                }
            }

            
        }

        /// <summary>
        /// Change Setter Value="" to Freezable in Setter.Value
        /// </summary>
        private void _InsertFreezableSetterValue(XmlElement setterNode, XmlElement setterStyleNode)
        {
            string value = setterNode.Attributes["Value"].Value;

            // Remove Value attribute from setterNode
            setterNode.Attributes.Remove(setterNode.Attributes["Value"]);

            // Add Setter.Value
            XmlElement setterValueNode = _testDoc.AddElement(setterNode, "Setter.Value");

            // Add SolidColorBrush
            XmlElement brushNode = _testDoc.AddElement(setterValueNode, "SolidColorBrush");

            // Set brush Color
            string colorValue = String.Empty;
            switch (_state.FreezableSetterValue)
            {
                case "Local":
                    // Set brush Color to old setterNode value.
                    colorValue = value;
                    break;

                case "DynamicResource":
                    // Set brush color to DynamicResource to brush in resources.
                    colorValue = "{DynamicResource myColor}";
                    break;

                case "StaticResource":
                    // Set brush color to StaticResource to brush in resources.
                    colorValue = "{StaticResource myColor}";
                    break;

                case "Binding":
                    // Set brush color to Binding.
                    throw new NotSupportedException("Freezable setter value: Binding");

                default:
                    throw new NotSupportedException(_state.FreezableSetterValue);
            }

            _testDoc.AddAttribute(brushNode, "Color", colorValue);
        }

        /// <summary>
        /// Insert event triggers in base and derived style.
        /// </summary>
        private void _InsertEventTrigger()
        {
            if (_state.HasEventTrigger == "None")
            {
                return;
            }

            //
            // Call InsertTrigger with the appropriate trigger names for base and derived style.
            //
            
            // If event trigger is not only in derived style add it to the base.
            if (_state.HasEventTrigger != "InDerivedStyle")
            {
                _InsertTrigger(_baseStyleNode, "BaseEventTrigger_" +  _state.RoutedEvent);
            }
            
            // If event trigger is not only in the base style add it to the derived style.
            if (_state.HasEventTrigger != "InBaseStyle")
            {
                _InsertTrigger(_styleNode, "EventTrigger_" + _state.RoutedEvent);
            }
        }


        /// <summary>
        /// Insert property trigger in base and derived style.
        /// </summary>
        private void _InsertPropertyTrigger()
        {
            if (_state.HasPropertyTrigger == "None")
            {
                return;
            }

            // If trigger is not only in derived style add it to the base.
            if (_state.HasPropertyTrigger != "InDerivedStyle")
            {
                XmlElement triggerNode = _InsertTrigger(_baseStyleNode, "BaseTrigger");
                if (_state.FreezableTriggerSetter)
                {
                    _InsertFreezableSetterValue((XmlElement)triggerNode.FirstChild, _baseStyleNode);
                }

                // If style does not have target type property should include type name.
                if (_state.BaseHasTargetType == false)
                {
                    XmlElement setterNode = (XmlElement)triggerNode.SelectSingleNode("av:Setter", _testDoc.Nsmgr);
                    if (setterNode == null)
                    {
                        throw new Exception("Could not get setter node");
                    }

                    XmlAttribute propertyAttribute = setterNode.Attributes["Property"];

                    propertyAttribute.Value = propertyAttribute.Value.Insert(0, _styledItemName + ".");
                }
            }

            // If trigger is not only in the base style add it to the derived style.
            if (_state.HasPropertyTrigger != "InBaseStyle")
            {
                // Property trigger in derived style or there is no base style.

                XmlElement triggerNode = _InsertTrigger(_styleNode, "Trigger");
                if (_state.FreezableTriggerSetter)
                {
                    _InsertFreezableSetterValue((XmlElement)triggerNode.FirstChild, _styleNode);
                }

                if (_state.StyleHasTargetType == false)
                {
                    XmlElement setterNode = (XmlElement)triggerNode.SelectSingleNode("av:Setter", _testDoc.Nsmgr);
                    if (setterNode == null)
                    {
                        throw new Exception("Could not get setter node");
                    }

                    XmlAttribute propertyAttribute = setterNode.Attributes["Property"];
                    propertyAttribute.Value = propertyAttribute.Value.Insert(0, _styledItemName + ".");
                }
            }

        }


        /// <summary>
        /// HasTemplate;
        /// TemplateType;
        /// </summary>
        private void _InsertTemplate()
        {
            if (_state.HasTemplate == "None")
            {
                return;
            }

            // 

            // Put template with style binding in TestRoot.Resources.
            XmlElement templateNode = _testDoc.ImportAndAppendElement(_testDoc.AddElement(_testRootNode, "StackPanel.Resources"), _testDoc.GetSnippetByXPath("//*[@x:Key='ControlTemplateWithStyleBinding']"));
            
            // Change template's targetype accordingly.
            _testDoc.ReplaceAttributeSubstring(templateNode, "TargetType", "FOO", _styledItemName);

            // Add template attribute to styled control. I add the attribute instead of removing it in _InsertStyledItem
            // because FCE does not have a template property and it would be weird to always have one in the elements doc 
            // just to remove it and a condition on item type is gross.
            
            _testDoc.AddAttribute(_styledNode, "Template", "{DynamicResource ControlTemplateWithStyleBinding}");
        }

        /// <summary>
        /// HasDataTrigger
        /// </summary>
        private void _InsertDataTrigger()
        {
            if (_state.HasDataTrigger == "None")
            {
                return;
            }

            // Add data source to TestRoot.Resources.
            XmlElement dataSourceNode = _testDoc.ImportAndAppendElement(_testDoc.AddElement(_testRootNode, "StackPanel.Resources"), _testDoc.GetSnippetByXPath("//*[@x:Key='GameDataSource']"));
            
            //// Add data binding to element content.

            // Add data context attribute to styled element.
            _testDoc.AddAttribute(_styledNode, "DataContext", "{StaticResource GameDataSource}");

            //
            // Add DataTrigger and a Content DataBinding Setter to base style (if any), derived style (if any), or both...
            //

            // If trigger is not only in derived style add it to the base.
            if (_state.HasDataTrigger != "InDerivedStyle")
            {
                XmlElement dataTriggerNode = _InsertTrigger(_baseStyleNode, "BaseDataTrigger");

                // Insert styled item type name '.' before Setter Property name if TargetType is undefined.
                if (_state.BaseHasTargetType == false)
                {
                    XmlElement setterNode = (XmlElement)dataTriggerNode.SelectSingleNode("av:Setter", _testDoc.Nsmgr);
                    if (setterNode == null)
                    {
                        throw new Exception("Could not get setter node");
                    }

                    XmlAttribute propertyAttribute = setterNode.Attributes["Property"];

                    propertyAttribute.Value = propertyAttribute.Value.Insert(0, _styledItemName + ".");
                }
            }

            // If trigger is not only in the base style add it to the derived style.
            if (_state.HasDataTrigger != "InBaseStyle")
            {
                // Property trigger in derived style or there is no base style.

                XmlElement dataTriggerNode = _InsertTrigger(_styleNode, "DataTrigger");

                // Insert styled item type name '.' before Setter Property name if TargetType is undefined.
                if (_state.StyleHasTargetType == false)
                {
                    XmlElement setterNode = (XmlElement)dataTriggerNode.SelectSingleNode("av:Setter", _testDoc.Nsmgr);
                    if (setterNode == null)
                    {
                        throw new Exception("Could not get setter node");
                    }

                    XmlAttribute propertyAttribute = setterNode.Attributes["Property"];
                    propertyAttribute.Value = propertyAttribute.Value.Insert(0, _styledItemName + ".");
                }
            }
        }

        /// <summary>
        /// Helper, create Style.Triggers node in styleNode if it doesn't already exist. 
        /// Select trigger with TestId triggerName from elements and insert it.
        /// </summary>
        private XmlElement _InsertTrigger(XmlElement styleNode, string triggerName)
        {
            // Import trigger and insert in style.
            return _testDoc.ImportAndAppendElement(_testDoc.AddElement(styleNode, "Style.Triggers"), _testDoc.GetSnippet(triggerName));
        }

        /// <summary>
        /// Add Style.Resources section to base and derived style.
        /// </summary>
        private void _InsertStyleResources()
        {
            // Add Style.Resources to base style.
            if ((_state.StyleResources == "InBaseStyle") || (_state.StyleResources == "OverrideBase") || (_state.StyleResources == "Both"))
            {
                // Insert style resources used by the style setters. 
                XmlElement styleResources = _testDoc.AddElement(_baseStyleNode, "Style.Resources");

                // Add resources, prepend because resource section should be first.
                _testDoc.ImportAndPrependElement(styleResources, _testDoc.GetSnippet("baseStyleResource"));
            }

            // Add Style.Resources to derived style.
            if ((_state.StyleResources == "InDerivedStyle") || (_state.StyleResources == "OverrideBase") || (_state.StyleResources == "Both"))
            {
                // Insert style resources used by the style setters. 
                XmlElement styleResources = _testDoc.AddElement(_styleNode, "Style.Resources");

                // Add resources, prepend because resource section should come first.
                _testDoc.ImportAndPrependElement(styleResources, _testDoc.GetSnippet("styleResource"));
            }
        }

        // Holds the model instance that works with this helper.
        private StyleModelState _state = null;

        // Convenient references to xml nodes used throughout the xaml construction routines.
        XamlTestDocument _testDoc = null;

        private XmlElement _testRootNode = null;

        private string _styledItemName = null;

        // Style item
        private XmlElement _styledNodeParent = null;
        private XmlElement _styledNode = null;

        // Base style
        private XmlElement _baseStyleParentNode = null;
        private XmlElement _baseStyleNode = null;

        // Derived style
        private XmlElement _styleParentNode = null;
        private XmlElement _styleNode = null;
    }
}

