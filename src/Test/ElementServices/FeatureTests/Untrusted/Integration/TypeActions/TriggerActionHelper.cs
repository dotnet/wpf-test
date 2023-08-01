// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using System.Xml;

using Microsoft.Test.Serialization;
using Microsoft.Test.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.Windows;
using Microsoft.Test.Markup;

namespace Avalon.Test.CoreUI.Integration.TypeActions
{
    /******************************************************************************
    * CLASS:          TriggerActionHelper
    ******************************************************************************/
    /// <summary>
    /// Implements helper routines for testing Avalon Property Trigger, all types and properties.
    ///</summary>
    public class TriggerActionHelper
    {
        #region Private Data
        private static Hashtable s_keyFramesTable = null;
        #endregion


        #region Static Members
        /******************************************************************************
        * Function:          InsertPropertyTriggers
        ******************************************************************************/
        //Every property serves as Source for a Property trigger to trig something to change the value of next available 
        // property in the list. In the case actionsName is null, a Setter will be used. Otherwise, storyboard created. 
        // actionsName can be EnterActions and ExitActions
        public static void InsertPropertyTriggers(XamlTestDocument doc, XmlElement triggersNode, Hashtable properties, Type ownerType, ArrayList keysForProperty, bool withStoryborad)
        {
            XmlElement lastTriggerNode = null;
            
            // Reference equality property cannot be used for trigger condition, use last non equality properties instead. 
            DependencyPropertyDescriptor[] dpds = new DependencyPropertyDescriptor[properties.Count];
            properties.Keys.CopyTo(dpds, 0);
            //For storyboard, there is no circle. Otherwise, the value cannot be re-set thus
            //Triggers are alway active.
            int lastPropertyIndex = withStoryborad ? dpds.Length - 1 : dpds.Length;
            for (int i = 0; i < lastPropertyIndex; i++)
            {
                //object value = null;
                
                XmlElement triggerNode = doc.CreateElement("Trigger", XamlGenerator.AvalonXmlns);
                triggersNode.InsertAfter(triggerNode, lastTriggerNode);
                lastTriggerNode = triggerNode;

                triggerNode.SetAttribute("Property", dpds[i].Name);
                GeneratePropertyValueXaml(doc, triggerNode, dpds[i], properties, keysForProperty, withStoryborad, 0);
                //setter, set next property with the second available value. 
                DependencyPropertyDescriptor dpdNext = dpds[(i + 1) % dpds.Length];

                if (!withStoryborad)
                {
                    XmlElement setterNode = doc.CreateElement("Setter", XamlGenerator.AvalonXmlns);
                    setterNode.SetAttribute("Property", dpdNext.Name);
                    triggerNode.InsertAfter(setterNode, null);
                    GeneratePropertyValueXaml(doc, setterNode, dpdNext, properties, keysForProperty);
                }
                else
                {
                    CreateActions(doc, triggerNode, dpdNext, properties, ownerType, keysForProperty, false);
                }
            }
        }

        /******************************************************************************
        * Function:          InsertsBeginStoryboard
        ******************************************************************************/
        public static void InsertsBeginStoryboard(XamlTestDocument doc, XmlElement actionsNode, DependencyPropertyDescriptor property, Hashtable properties, Type ownerType, ArrayList keysForProperty, int valueIndex)
        {
            XmlElement beginStoryboardNode = doc.CreateElement("BeginStoryboard", XamlGenerator.AvalonXmlns);
            actionsNode.InsertAfter(beginStoryboardNode, null);

            XmlElement storyboardNode = doc.CreateElement("Storyboard", XamlGenerator.AvalonXmlns);
            beginStoryboardNode.InsertAfter(storyboardNode, null);

            object value = ((object[])properties[property])[valueIndex];

            string stringValue = null;
            string key = null;
            if (value == null)
            {
                stringValue = "{x:Null}";
            }
            else if ((value is string && !IsReferenceEqualityTypeWityConverter(property.PropertyType))|| value.GetType().IsValueType)
            {
                stringValue = value.ToString();
            }
            else
            {
                key = GetResourceDictionaryKey(doc, property, properties, keysForProperty, valueIndex);
            }
            XmlElement framesNode = GetKeyFramesForType(property.PropertyType, doc);
            bool foundKeyFrames = true;
            if (framesNode == null)
            {
                framesNode = s_keyFramesTable["object"] as XmlElement;
                foundKeyFrames = false;
                //GlobalLog.LogStatus("Not found keyframe for type: " + property.PropertyType.Name + ".");
            }
            
            XmlElement keyFrames = (XmlElement)doc.ImportNode(framesNode, true);
            if (keyFrames != null)
                keyFrames.SetAttribute("Storyboard.TargetProperty", property.Name);

            storyboardNode.InsertAfter(keyFrames, null);

            XmlElement frameNode = keyFrames.FirstChild as XmlElement;
            if (key != null)
            {
                frameNode.SetAttribute("Value", "{StaticResource " + key + "}");
            }
            else if (stringValue != null)
            {
                if(String.Equals(stringValue, "{x:Null}", StringComparison.InvariantCulture)
                    )
                {
                    frameNode.SetAttribute("Value",  stringValue);
                }
                else if(!foundKeyFrames)
                {
                    //For a Type keyframes not found, just use x:Static for now. 
                    //It works for enum type and FontStyle, for others, need more testing. 
                    frameNode.SetAttribute("Value", "{x:Static " +property.PropertyType.Name + "." + stringValue + "}");
                }
                else
                {
                    frameNode.SetAttribute("Value", stringValue);
                }
            }
        }

        /******************************************************************************
        * Function:          SetInitialValues
        ******************************************************************************/
        public static void SetInitialValues(XamlTestDocument doc, XmlElement element, List<DependencyPropertyDescriptor> properties)
        {
            foreach (DependencyPropertyDescriptor dpd in properties)
            {
                object value = ActionHelper.GetPropertyValue(doc, element.Name, dpd);

                if (value == null)
                {
                    element.SetAttribute(dpd.Name, "{x:Null}");
                }
                else if (value is string || value.GetType().IsValueType)
                {
                    element.SetAttribute(dpd.Name, value.ToString());
                }
                else
                {
                    XmlElement propertyNode = doc.CreateElement("", element.Name + "." + dpd.Name, element.NamespaceURI);

                    if (value is XmlNode)
                    {
                        propertyNode.PrependChild(doc.ImportNode(((XmlNode)value), true));
                    }
                    else
                    {
                        propertyNode.InnerXml = SerializationHelper.SerializeObjectTree(value);
                    }

                    element.PrependChild(propertyNode);
                }
            }
        }

        /******************************************************************************
        * Function:          SetInitialValues
        ******************************************************************************/
        public static void SetInitialValues(XmlDocument doc, XmlElement element, Hashtable properties, ArrayList keysForProperty, int valueIndex)
        {
            foreach (DependencyPropertyDescriptor property in properties.Keys)
            {
                SetInitialValue(doc, element, property, properties, keysForProperty, valueIndex);
            }
        }

        /******************************************************************************
        * Function:          SetInitialValue
        ******************************************************************************/
        public static void SetInitialValue(XmlDocument doc, XmlElement element, DependencyPropertyDescriptor property, Hashtable properties, ArrayList keysForProperty, int valueIndex)
        {
            GenerateValueXaml(doc, element, property, properties, keysForProperty, property.Name, false, valueIndex);
        }

        /******************************************************************************
        * Function:          InsertMultiplePropertyTriggers
        ******************************************************************************/
        public static void InsertMultiplePropertyTriggers(XamlTestDocument doc, XmlElement TriggersNode, Hashtable properties, Type ownerType, ArrayList keysForProperty, bool withStoryboard)
        {
            XmlElement lastTriggerNode = null;

            // Reference equality property cannot be used for trigger condition, use last non equality properties instead. 
            DependencyPropertyDescriptor[] dpds = new DependencyPropertyDescriptor[properties.Count];
            properties.Keys.CopyTo(dpds, 0);
            int lastPropertyIndex = withStoryboard ? dpds.Length - 2 : dpds.Length;
            for (int i = 0; i < lastPropertyIndex; i++)
            {
                XmlElement triggerNode = doc.CreateElement("MultiTrigger", XamlGenerator.AvalonXmlns);
                TriggersNode.InsertAfter(triggerNode, lastTriggerNode);
                lastTriggerNode = triggerNode;
                
                XmlElement conditionsNode = doc.CreateElement("MultiTrigger.Conditions", XamlGenerator.AvalonXmlns);
                triggerNode.InsertAfter(conditionsNode, null);

                XmlElement conditionNode = doc.CreateElement("Condition", XamlGenerator.AvalonXmlns);
                GeneratePropertyValueXaml(doc, conditionNode, dpds[i], properties, keysForProperty, withStoryboard, 0);
                conditionsNode.InsertAfter(conditionNode, null);

                conditionNode = doc.CreateElement("Condition", XamlGenerator.AvalonXmlns);
                GeneratePropertyValueXaml(doc, conditionNode, dpds[(i + 1) % dpds.Length], properties, keysForProperty, withStoryboard, 0);
                conditionsNode.InsertAfter(conditionNode, null);

                //setter, set next property with the second available value. 

                if (!withStoryboard)
                {
                    XmlElement setterNode = doc.CreateElement("Setter", XamlGenerator.AvalonXmlns);
                    setterNode.SetAttribute("Property", dpds[(i + 2) % dpds.Length].Name);
                    triggerNode.InsertAfter(setterNode, null);
                    GeneratePropertyValueXaml(doc, setterNode, dpds[(i + 2) % dpds.Length], properties, keysForProperty);
                }
                else
                {
                    CreateActions(doc, triggerNode, dpds[(i + 2) % dpds.Length], properties, ownerType, keysForProperty, true);
                }
            }
        }

        /******************************************************************************
        * Function:          GeneratePropertyValueXaml
        ******************************************************************************/
        public static void GeneratePropertyValueXaml(XmlDocument doc, XmlElement node, DependencyPropertyDescriptor property, Hashtable propertyValues, ArrayList keysForProperty, bool withStoryborad, int valueIndex)
        {
            node.SetAttribute("Property", property.Name);
            GenerateValueXaml(doc, node, property, propertyValues, keysForProperty, "Value", withStoryborad, valueIndex);
        }

        /******************************************************************************
        * Function:          GeneratePropertyValueXaml
        ******************************************************************************/
        /// <summary>
        /// For a node like Setter, Trigger, and Condition, set the "property" attribute and Value.
        /// </summary>
        /// <param name="doc">XmlDocument for the xml generation.</param>
        /// <param name="node">Node to set Property and Value.</param>
        /// <param name="property">The property.</param>
        /// <param name="propertyValues">property value table.</param>
        /// <param name="keysForProperty"></param>
        public static void GeneratePropertyValueXaml(XmlDocument doc, XmlElement node, DependencyPropertyDescriptor property, Hashtable propertyValues, ArrayList keysForProperty)
        {
            GeneratePropertyValueXaml(doc, node, property, propertyValues, keysForProperty, false, 0);
        }

        /******************************************************************************
        * Function:          GenerateValueXaml
        ******************************************************************************/
        public static void GenerateValueXaml(XmlDocument doc, XmlElement node, DependencyPropertyDescriptor property, Hashtable propertyValues, ArrayList keysForProperty, string valueFor, bool withStoryborad, int valueIndex)
        {
            object value = null;
            value = ((object[])propertyValues[property])[valueIndex];

            if (value == null)
            {
                node.SetAttribute(valueFor, "{x:Null}");
            }
            else if ((value is string && !IsReferenceEqualityTypeWityConverter(property.PropertyType))|| value.GetType().IsValueType)
            {
                node.SetAttribute(valueFor, value.ToString());
            }
            else
            {
                string key = GetResourceDictionaryKey(doc, property, propertyValues, keysForProperty, valueIndex);
                if (key == null || key.Trim().Length==0)
                    throw new Microsoft.Test.TestSetupException("Have not find the key.");
                node.SetAttribute(valueFor, "{StaticResource " + key + "}");
            }
        }

        /******************************************************************************
        * Function:          InsertTemplateTriggersNode
        ******************************************************************************/
        public static XmlElement InsertTemplateTriggersNode(XmlDocument doc, XmlElement elementNode)
        {
            XmlElement templateComplexProp = doc.CreateElement("", elementNode.Name + ".Template", elementNode.NamespaceURI);
            elementNode.InsertAfter(templateComplexProp, null);

            XmlElement templateElement = doc.CreateElement("ControlTemplate", XamlGenerator.AvalonXmlns);
            // Set TargetType attribute on Template.
            templateElement.SetAttribute("TargetType", "{x:Type " + elementNode.Name + "}");
            templateComplexProp.InsertAfter(templateElement, null);

            XmlElement triggersElement = doc.CreateElement("ControlTemplate.Triggers", XamlGenerator.AvalonXmlns);
            templateElement.InsertAfter(triggersElement, null);
            return triggersElement;
        }

        /******************************************************************************
        * Function:          InsertMainElement
        ******************************************************************************/
        public static XmlElement InsertMainElement(XmlDocument doc, XmlElement containerSnippet, string rootName, string nodeName, Type type)
        {
            // Insert styled element.
            XmlElement testRootNode = doc.SelectSingleNode("//*[@Name='" + rootName + "']", ActionHelper.NamespaceManager) as XmlElement;
            XmlElement containerNode = (XmlElement)doc.ImportNode(containerSnippet, true);
            testRootNode.AppendChild(containerNode);
            containerNode = (XmlElement)containerNode.SelectSingleNode("//*[@Name='ContainerElement']", ActionHelper.NamespaceManager);
            containerNode.RemoveAttribute("Name");

            XmlElement element = doc.CreateElement("", type.Name,
                testRootNode.NamespaceURI);
            element.SetAttribute("Name", nodeName);
            containerNode.AppendChild(element);
            return element;
        }

        /******************************************************************************
        * Function:          VerifyPropertyTrigger
        ******************************************************************************/
        public static void VerifyPropertyTrigger(FrameworkElement root, string firstName, string secondName, Hashtable properties)
        {
            GlobalLog.LogStatus("Verify EnterActions...");
            VerifyPropertyTrigger(root, "elementWithInitialValues", firstName, secondName, properties);
            GlobalLog.LogStatus("Verify ExitActions...");
            VerifyPropertyTrigger(root, "elementWithInitialValues2", firstName, secondName, properties);
        }

        /******************************************************************************
        * Function:          VerifyPropertyTrigger
        ******************************************************************************/
        public static void VerifyPropertyTrigger(FrameworkElement root, string baseName, string firstName, string secondName, Hashtable properties)
        {
            DependencyObject firstElement = (DependencyObject)root.FindName(firstName);
            DependencyObject secondElement = (DependencyObject)root.FindName(secondName);
            DependencyObject elementWithInitialValues = (DependencyObject)root.FindName(baseName);
            Type type = firstElement.GetType();
            //
            // Loop through every property.
            // - check property has expected value, compare to locally-set property
            //
            DependencyPropertyDescriptor[] propertyArray = new DependencyPropertyDescriptor[properties.Count];
            properties.Keys.CopyTo(propertyArray, 0);
            
            //set initial value for trigger.
            if (properties.Count > 1)
            {
                firstElement.SetValue(propertyArray[0].DependencyProperty, elementWithInitialValues.GetValue(propertyArray[0].DependencyProperty));
                secondElement.SetValue(propertyArray[0].DependencyProperty, elementWithInitialValues.GetValue(propertyArray[0].DependencyProperty));
                secondElement.SetValue(propertyArray[1].DependencyProperty, elementWithInitialValues.GetValue(propertyArray[1].DependencyProperty));
            }

            DispatcherHelper.DoEvents(500);
            int propertyIndex = 0;
            foreach (DependencyPropertyDescriptor dpd in properties.Keys)
            {
                //First two properties maybe locally set as trigger. Skip the verification for source. 
                propertyIndex++;
                if (propertyIndex > 2)
                {
                    VerifyValueIsFromStyleOrTemplate(firstElement, dpd);
                    VerifyValueIsFromStyleOrTemplate(secondElement, dpd);
                }

                // Check property value is equivalent to locally-set property
                // on untemplated element.
                GlobalLog.LogStatus("Check property: " + dpd.Name + " on the first element.");
                ActionForTypeHelper.CheckExpectedPropertyValue(firstElement, elementWithInitialValues, dpd.DependencyProperty, ActionHelper.PropertiesToSkip);
                GlobalLog.LogStatus("On the second element.");
                ActionForTypeHelper.CheckExpectedPropertyValue(secondElement, elementWithInitialValues, dpd.DependencyProperty, ActionHelper.PropertiesToSkip);
            }
        }

        /******************************************************************************
        * Function:          VerifyValueIsFromStyleOrTemplate
        ******************************************************************************/
        public static void VerifyValueIsFromStyleOrTemplate(DependencyObject element, DependencyPropertyDescriptor dpd)
        {
            ValueSource valueSource = DependencyPropertyHelper.GetValueSource(element, dpd.DependencyProperty);
            if (valueSource.BaseValueSource != BaseValueSource.TemplateTrigger
                && valueSource.BaseValueSource != BaseValueSource.StyleTrigger
                && !valueSource.IsAnimated)
            {
                throw new Microsoft.Test.TestValidationException("'" + dpd.Name + "' property value does not come from the Trigger, Source: " + valueSource.BaseValueSource.ToString() + " and it is not Animated.");
            }
        }

        /******************************************************************************
        * Function:          IsReferenceEqualityTypeWityConverter
        ******************************************************************************/
        public static bool IsReferenceEqualityTypeWityConverter(Type type)
        {
            if (type.Equals(typeof(System.Windows.Input.InputScope))) return true;
            return false;
        }

        /******************************************************************************
        * Function:          GetKeyFrames
        ******************************************************************************/
        // Returns the xml node where we should put the type tag.
        public static void GetKeyFrames(XamlTestDocument doc)
        {
            XmlNodeList keyFramesNodes = doc.SnippetsDoc.GetElementsByTagName("KeyFrames");
            s_keyFramesTable = new Hashtable();
            foreach (XmlNode node in keyFramesNodes)
            {
                XmlElement element = node as XmlElement;
                s_keyFramesTable.Add(element.GetAttribute("Type"), element.FirstChild);
            }
        }

        /******************************************************************************
        * Function:          GetKeyFramesForType
        ******************************************************************************/
        public static XmlElement GetKeyFramesForType(Type type, XamlTestDocument doc)
        {
            if (s_keyFramesTable == null) GetKeyFrames(doc);
            Type baseType = type;
            XmlElement keyFrames = null;
            while (baseType != null && keyFrames == null)
            {
                keyFrames = s_keyFramesTable[type.Name] as XmlElement;
                baseType = baseType.BaseType;
            }
            if (keyFrames != null)
            {
                return keyFrames as XmlElement;
            }
            else
            {
                return null;
            }
        }

        /******************************************************************************
        * Function:          TrimPropertyTableForStoryboard
        ******************************************************************************/
        public static Hashtable TrimPropertyTableForStoryboard(Hashtable propertyTable)
        {
            Hashtable newTable = new Hashtable();
            foreach (object key in propertyTable.Keys)
            {
                string propertyName = ((PropertyDescriptor)key).Name;
                //
                if (!string.Equals(propertyName, "Language", StringComparison.InvariantCulture)
                    && !string.Equals(propertyName, "Tag", StringComparison.InvariantCulture)
                    //
                    && !propertyName.EndsWith("Style")
                    && !string.Equals(propertyName, "Cursor", StringComparison.InvariantCulture)
                    && !string.Equals(propertyName, "ItemsPanel", StringComparison.InvariantCulture)
                    && !string.Equals(propertyName, "ItemContainerStyle", StringComparison.InvariantCulture)
                    //Don't know how to set the value for nullable yet . 
                    && !((PropertyDescriptor)key).PropertyType.Name.StartsWith("Nullable")
                    //GridLength.* is not correct syntax. 
                    && !string.Equals(((PropertyDescriptor)key).PropertyType.Name, "GridLength", StringComparison.InvariantCulture)
                    && !string.Equals(propertyName, "Text", StringComparison.InvariantCulture)
                    )
                {
                    newTable.Add(key, propertyTable[key]);
                }
            }
            return newTable;
        }

        /******************************************************************************
        * Function:          GetPropertyTableForStoryboard
        ******************************************************************************/
        // Returns list of properties that can be used for property trigger on the given type.
        public static Hashtable GetPropertyTableForStoryboard(string typeName, XamlTestDocument doc)
        {
            Type type = ActionHelper.GetType(typeName, true);
            Hashtable properties = new Hashtable();
            
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(type))
            {
                DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(prop);
                if (ActionHelper.IsStylableProperty(dpd))
                {
                    //Usually, the value those property depends on some other factor, not always the value set, thus
                    //cannot be used for trigger purpose. 
                    if (ActionForTypeHelper.ShouldIgnoreProperty(dpd.Name, type, ActionHelper.PropertiesToSkip))
                        continue;

                    object[] value = new object[2];
                    value[0] = ActionHelper.GetPropertyValue(doc, typeName, dpd, 0);
                    value[1] = ActionHelper.GetPropertyValue(doc, typeName, dpd, 1);
                    if (value[0] == null || value[1]==null)
                    {
                        GlobalLog.LogStatus("Value for: " + dpd.Name + ", which is of type : " + dpd.PropertyType.Name + ", is null. ");
                        continue;
                    }

                    //Cannot set Template in Template. 
                    if (String.Equals(dpd.Name, "Template", StringComparison.InvariantCulture))
                        continue;
                    //Cannot set OverridesDefaultStyle in Template. 
                    if (String.Equals(dpd.Name, "OverridesDefaultStyle", StringComparison.InvariantCulture))
                        continue;

                    properties.Add(dpd, value);
                }
            }
            Hashtable trimedTable = TriggerActionHelper.TrimPropertyTableForStoryboard(properties);
            return trimedTable;
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          CreateActions
        ******************************************************************************/
        private static void CreateActions(XamlTestDocument doc, XmlElement triggerNode, DependencyPropertyDescriptor property, Hashtable properties, Type ownerType, ArrayList keysForProperty, bool isMultiTrigger)
        {
            CreateActions(doc, triggerNode, property, properties, ownerType, keysForProperty, "EnterActions", 0, isMultiTrigger);
            CreateActions(doc, triggerNode, property, properties, ownerType, keysForProperty, "ExitActions", 1, isMultiTrigger);
        }

        /******************************************************************************
        * Function:          CreateActions
        ******************************************************************************/
        private static void CreateActions(XamlTestDocument doc, XmlElement triggerNode, DependencyPropertyDescriptor property, Hashtable properties, Type ownerType, ArrayList keysForProperty, string actionsName, int valueIndex, bool isMultiTrigger)
        {
            string triggerType = isMultiTrigger ? "MultiTrigger" : "Trigger";
            XmlElement actionsNode = doc.CreateElement(triggerType + "." + actionsName, XamlGenerator.AvalonXmlns);
            triggerNode.InsertAfter(actionsNode, null);
            InsertsBeginStoryboard(doc, actionsNode, property, properties, ownerType, keysForProperty, valueIndex);
        }

        /******************************************************************************
        * Function:          GetResourceDictionaryKey
        ******************************************************************************/
        private static string GetResourceDictionaryKey(XmlDocument doc, DependencyPropertyDescriptor property, Hashtable propertyValues, ArrayList keysForProperty, int valueIndex)
        {
            XmlElement resourcesNode = doc.DocumentElement.FirstChild as XmlElement;
            object value = ((object[])propertyValues[property])[valueIndex];
            
            string key = property.Name + "_" + valueIndex.ToString();

            if (value == null
                || (value is string && !IsReferenceEqualityTypeWityConverter(property.PropertyType)) || value.GetType().IsValueType)
            {
                return null;
            }

            if (!keysForProperty.Contains(key))
            {
                // for s string value for reference equlity type, if the typeconverter of the
                //type create a new instance, need to add one item in dictionary. 
                if (value is string && IsReferenceEqualityTypeWityConverter(property.PropertyType))
                {
                    XmlElement element = doc.CreateElement(property.PropertyType.Name, XamlGenerator.AvalonXmlns);
                    element.InnerXml = value as string;
                    element.SetAttribute("Key", XamlGenerator.AvalonXmlnsX, key);
                    resourcesNode.InsertAfter(element, null);
                }
                else if (value is XmlNode)
                {
                    XmlElement element = value as XmlElement;
                    element.SetAttribute("Key", XamlGenerator.AvalonXmlnsX, key);
                    resourcesNode.PrependChild(doc.ImportNode(element, true));
                }
                else
                {
                    string stringRepresentation = SerializationHelper.SerializeObjectTree(value);
                    int index = stringRepresentation.IndexOf('>');
                    if (stringRepresentation[index - 1] == '/')
                        stringRepresentation = stringRepresentation.Insert(index - 1, " x:Key=\"" + key + "\"");
                    else
                        stringRepresentation = stringRepresentation.Insert(index, " x:Key=\"" + key + "\"");
                    resourcesNode.InnerXml += stringRepresentation;
                }
                keysForProperty.Add(key);
            }
            return key;
        }
        #endregion
    }
}
