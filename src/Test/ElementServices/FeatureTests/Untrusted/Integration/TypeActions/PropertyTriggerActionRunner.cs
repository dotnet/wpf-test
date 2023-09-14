// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Contributor: 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Reflection;
using System.Windows.Media;
using Microsoft.Test.Serialization;
using Microsoft.Test.Threading;
using System.Threading;
using System.IO;
using System.Collections;
using System.Xml;
using Microsoft.Test.Logging;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test.MSBuildEngine;
using Avalon.Test.CoreUI.Parser;
using Microsoft.Test.Windows;
using Microsoft.Test.Markup;
using Avalon.Test.CoreUI.Common;

namespace Avalon.Test.CoreUI.Integration.TypeActions
{
    /// <summary>
    /// Implements routines for testing Avalon Property Trigger in Style, all types and properties.
    ///</summary>
    public class TemplatePropertyTriggerActionRunner : TriggerActionRunner
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public TemplatePropertyTriggerActionRunner() : base()
        {
        }

        /// <summary>
        /// Builds xaml with elements with Property Trigger in Template, loads it,
        /// and verifies that the instance's property values were triggered correctly.
        /// </summary>
        public void DoTemplatePropertyTriggerTest(string typeName, ActionForType actionForType)
        {
            BuildTemplatePropertyTriggerXaml(typeName);
            GlobalLog.LogFile(this.XamlFileName);

            //
            // Parse xaml.
            // Diplay tree.
            // Verify tree. 
            //
            // Verification routine is called automatically when the tree
            // is rendered.
            //
            object root = SerializationHelper.ParseXamlFile(this.XamlFileName);

            CoreLogger.LogStatus("xaml loaded. ");

            SerializationHelper helper = new SerializationHelper();
            helper.DisplayTree(root);
        }
        // Returns list of properties that can be used for property trigger on the given type.
        internal Hashtable GetPropertyTableForTriggerInTemplate(string typeName)
        {
            Type type = GetType(typeName);
            Hashtable properties = new Hashtable();
            
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(type))
            {
                DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(prop);
                if (StyleActionRunner.IsStylableProperty(dpd))
                {
                    //Usually, the value those property depends on some other factor, not always the value set, thus
                    //cannot be used for trigger purpose. 
                    if (ActionForTypeHelper.ShouldIgnoreProperty(dpd.Name, type, this.PropertiesToSkip))
                        continue;

                    object[] value = new object[2];
                    value[0] = ActionRunner.GetPropertyValue(this.TestDocument, typeName, dpd);
                    if (value[0] == null)
                    {
                        CoreLogger.LogStatus("Value for: " + dpd.Name + ", which is of type : " + dpd.PropertyType.Name + ", is null. ");
                        //continue;
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

            return properties;
        }
            
        // Builds a new xaml file for the type to use for loading and verification.
        // The xaml file contains 5 instance declarations: 
        //      - StyledElement - has a Style set via property element syntax, and propertyTriggers are under the style.
        //      - StyledElementMultiTrigger - same as above except the triggers are MultiTrigger.
        //      - TemplatedElement - has a template set via property element syntax, and propertyTriggers are under the template.
        //      - TemplatedElementMultiTrigger - same as above except the triggers are MultiTrigger.
        //      - elementWithInitialValues - has locally-set properties for comparison purpose.
        
        private void BuildTemplatePropertyTriggerXaml(string typeName)
        {
            Type type = GetType(typeName);

            XamlTestDocument doc = this.TestDocument;

//            XmlNode testRootNode = null;

            Hashtable properties = GetPropertyTableForTriggerInTemplate(typeName);
            ArrayList keysForProperty = new ArrayList();

            XmlElement containerNode = doc.GetContainerNode(type);
            // Insert element with just initial value for comparision.
            XmlElement elementWithInitialValues = TriggerActionRunner.InsertMainElement(doc, containerNode, "RootForElementWithInitialValues", "elementWithInitialValues", type);
            this.SetInitialValues(doc, elementWithInitialValues, properties, keysForProperty, 0);
            CoreLogger.LogStatus("elementWithInitialValues generated.");

            // Insert styled element where style composed of property trigger. 
            XmlElement templatedElement = TriggerActionRunner.InsertMainElement(doc, containerNode, "RootForTemplatedElement", "TemplatedElement", type);
            XmlElement templateTriggersNode = InsertTemplateTriggersNode(doc, templatedElement);
            this.InsertPropertyTriggers(doc, templateTriggersNode, properties, type, keysForProperty, false);
            CoreLogger.LogStatus("templatedElement set.");

            // Insert styled element where style composed of multiple property trigger. 
            XmlElement templatedElemenMultiTrigger = TriggerActionRunner.InsertMainElement(doc, containerNode, "RootForTemplatedElementMultiTrigger", "TemplatedElementMultiTrigger", type);
            templateTriggersNode = InsertTemplateTriggersNode(doc, templatedElemenMultiTrigger);
            this.InsertMultiplePropertyTriggers(doc, templateTriggersNode, properties, type, keysForProperty, false);
            CoreLogger.LogStatus("templatedElement with MultiTrigger set.");

            // Set Verifier property.
            doc.SetVerifierRoutine(this.GetType()).GetMethod("VerifyTemplatePropertyTrigger");

            // Save final xml to file.
            IOHelper.SaveTextToFile(IOHelper.ConvertXmlDocumentToStream(doc), this.XamlFileName);
            CoreLogger.LogStatus("Xaml Generated : " + this.XamlFileName + ".");
        }

        /// <summary>
        /// Verifies preperty trigger after the first render.
        /// </summary>
        /// <param name="uielement"></param>
        public void VerifyTemplatePropertyTrigger(UIElement uielement)
        {
            FrameworkElement root = (FrameworkElement)uielement;
            Type type = root.FindName("TemplatedElement").GetType();
            Hashtable properties = GetPropertyTableForTriggerInTemplate(type.FullName);
            this.VerifyPropertyTrigger(root, "elementWithInitialValues", "TemplatedElement", "TemplatedElementMultiTrigger", properties);
        }

        /// <summary>
        /// Insert two layers like
        /// <!-- <Element.Template>
        ///     <ControlTemplate>
        ///     </ControlTemplate>
        /// </Element.Template> -->
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="elementNode">The node under which to insert this.</param>
        /// <returns>inner Template node</returns>
        new internal static XmlElement InsertTemplateTriggersNode(XmlDocument doc, XmlElement elementNode)
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
    }

    /// <summary>
    /// Implements routines for testing Avalon Property Trigger in Style, all types and properties.
    ///</summary>
    public class StylePropertyTriggerActionRunner : TriggerActionRunner
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public StylePropertyTriggerActionRunner() : base()
        {
        }

        /// <summary>
        /// Builds xaml with elements with Property Trigger in Style/Template, loads it,
        /// and verifies that the instance's property values were triggered correctly.
        /// </summary>
        public void DoStylePropertyTriggerTest(string typeName, ActionForType actionForType)
        {
            BuildStylePropertyTriggerXaml(typeName);
            GlobalLog.LogFile(this.XamlFileName);

            //
            // Parse xaml.
            // Diplay tree.
            // Verify tree. 
            //
            // Verification routine is called automatically when the tree
            // is rendered.
            //
            object root = SerializationHelper.ParseXamlFile(this.XamlFileName);

            CoreLogger.LogStatus("xaml loaded. ");

            SerializationHelper helper = new SerializationHelper();
            helper.DisplayTree(root);
        }
        /// <summary>
        /// Returns list of properties that can be used for property trigger on the given type.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        protected Hashtable GetPropertyTableForTriggerInStyle(string typeName)
        {
            Type type = GetType(typeName);
            Hashtable properties = new Hashtable();
            
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(type))
            {
                DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(prop);                
                if (StyleActionRunner.IsStylableProperty(dpd))
                {
                    // setting Template causes a lot of property changes.. 
                    if (dpd != null && String.Equals(dpd.Name, "Template", StringComparison.InvariantCulture))
                        continue;
                    
                    //Usually, the value those property depends on some other factor, not always the value set, thus
                    //cannot be used for trigger purpose. 
                    if (ActionForTypeHelper.ShouldIgnoreProperty(dpd.Name, type, this.PropertiesToSkip))
                        continue;
                    object[] value = new object[2];
                    value[0] = ActionRunner.GetPropertyValue(this.TestDocument, typeName, dpd);

                    properties.Add(dpd, value);
                }
            }

            return properties;
        }

        // Builds a new xaml file for the type to use for loading and verification.
        // The xaml file contains 5 instance declarations: 
        //      - StyledElement - has a Style set via property element syntax, and propertyTriggers are under the style.
        //      - StyledElementMultiTrigger - same as above except the triggers are MultiTrigger.
        //      - TemplatedElement - has a template set via property element syntax, and propertyTriggers are under the template.
        //      - TemplatedElementMultiTrigger - same as above except the triggers are MultiTrigger.
        //      - elementWithInitialValues - has locally-set properties for comparison purpose.

        private void BuildStylePropertyTriggerXaml(string typeName)
        {
            Type type = GetType(typeName);

            XamlTestDocument doc = this.TestDocument;

//            XmlNode testRootNode = null;

            Hashtable properties = GetPropertyTableForTriggerInStyle(typeName);
            ArrayList keysForProperty = new ArrayList();

            XmlElement containerNode = doc.GetContainerNode(type);
            // Insert element with just initial value for comparision.
            XmlElement elementWithInitialValues = TriggerActionRunner.InsertMainElement(doc, containerNode, "RootForElementWithInitialValues", "elementWithInitialValues", type);
            this.SetInitialValues(doc, elementWithInitialValues, properties, keysForProperty, 0);
            CoreLogger.LogStatus("elementWithInitialValues generated.");

            // Insert styled element where style composed of property trigger. 
            XmlElement styledElement = TriggerActionRunner.InsertMainElement(doc, containerNode, "RootForStyledElement", "StyledElement", type);
            XmlElement styleTriggersNode = InsertStyleTriggersNode(doc, styledElement);
            this.InsertPropertyTriggers(doc, styleTriggersNode, properties, type, keysForProperty, false);
            CoreLogger.LogStatus("styledElement set.");

            // Insert styled element where style composed of multiple property trigger. 
            XmlElement styledElementMultiTrigger = TriggerActionRunner.InsertMainElement(doc, containerNode, "RootForStyledElementMultiTrigger", "StyledElementMultiTrigger", type);
            styleTriggersNode = InsertStyleTriggersNode(doc, styledElementMultiTrigger);
            this.InsertMultiplePropertyTriggers(doc, styleTriggersNode, properties, type, keysForProperty, false);
            CoreLogger.LogStatus("styledElement with MultiTrigger set.");
            
            // Set Verifier property.
            doc.SetVerifierRoutine(this.GetType()).GetMethod("VerifyStylePropertyTrigger");

            // Save final xml to file.
            IOHelper.SaveTextToFile(IOHelper.ConvertXmlDocumentToStream(doc), this.XamlFileName);
            CoreLogger.LogStatus("Xaml Generated : " + this.XamlFileName + ".");
        }

        /// <summary>
        /// Verifies preperty trigger after the first render.
        /// </summary>
        /// <param name="uielement"></param>
        public void VerifyStylePropertyTrigger(UIElement uielement)
        {
            FrameworkElement root = (FrameworkElement)uielement;
            Type type = root.FindName("StyledElement").GetType();
            Hashtable properties = GetPropertyTableForTriggerInStyle(type.FullName);
            this.VerifyPropertyTrigger(root, "elementWithInitialValues", "StyledElement", "StyledElementMultiTrigger", properties);
        }
 
        /// <summary>
        /// Insert two layers like
        /// <Element.Style>
        ///     <Style>
        ///     </Style>
        /// </Element.Style>
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="elementNode">The node under which to insert this.</param>
        /// <returns>inner Style node</returns>
        internal static XmlElement InsertStyleTriggersNode(XmlDocument doc, XmlElement elementNode)
        {
            XmlElement styleComplexProp = doc.CreateElement("", elementNode.Name + ".Style", elementNode.NamespaceURI);
            elementNode.InsertAfter(styleComplexProp, null);

            XmlElement styleElement = doc.CreateElement("Style", XamlGenerator.AvalonXmlns);
            // Set TargetType attribute on Style.
            styleElement.SetAttribute("TargetType", "{x:Type " + elementNode.Name + "}");
            styleComplexProp.InsertAfter(styleElement, null);

            XmlElement triggersElement = doc.CreateElement("Style.Triggers", XamlGenerator.AvalonXmlns);
            styleElement.InsertAfter(triggersElement, null);
            return triggersElement;
        }
    }

    /// <summary>
    /// Implements routines for testing Avalon Property Trigger, all types and properties.
    ///</summary>
    public abstract class TriggerActionRunner : ActionRunner
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public TriggerActionRunner() : base()
        {
            this.EmptyFileName = "PropertyTriggerActionForType_empty.xaml";
            this.XamlFileName = "__PropertyTriggerActionForType.xaml";
        }

        //Every property serves as Source for a Property trigger to trig something to change the value of next available 
        // property in the list. In the case actionsName is null, a Setter will be used. Otherwise, storyboard created. 
        // actionsName can be EnterActions and ExitActions
        internal void InsertPropertyTriggers(XamlTestDocument doc, XmlElement triggersNode, Hashtable properties, Type ownerType, ArrayList keysForProperty, bool withStoryborad)
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

        private void CreateActions(XamlTestDocument doc, XmlElement triggerNode, DependencyPropertyDescriptor property, Hashtable properties, Type ownerType, ArrayList keysForProperty, bool isMultiTrigger)
        {
            CreateActions(doc, triggerNode, property, properties, ownerType, keysForProperty, "EnterActions", 0, isMultiTrigger);
            CreateActions(doc, triggerNode, property, properties, ownerType, keysForProperty, "ExitActions", 1, isMultiTrigger);
        }

        private void CreateActions(XamlTestDocument doc, XmlElement triggerNode, DependencyPropertyDescriptor property, Hashtable properties, Type ownerType, ArrayList keysForProperty, string actionsName, int valueIndex, bool isMultiTrigger)
        {
            string triggerType = isMultiTrigger ? "MultiTrigger" : "Trigger";
            XmlElement actionsNode = doc.CreateElement(triggerType + "." + actionsName, XamlGenerator.AvalonXmlns);
            triggerNode.InsertAfter(actionsNode, null);
            InsertsBeginStoryboard(doc, actionsNode, property, properties, ownerType, keysForProperty, valueIndex);
        }
        internal static void InsertsBeginStoryboard(XamlTestDocument doc, XmlElement actionsNode, DependencyPropertyDescriptor property, Hashtable properties, Type ownerType, ArrayList keysForProperty, int valueIndex)
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
                key = GetReourceDictionaryKey(doc, property, properties, keysForProperty, valueIndex);
            }
            XmlElement framesNode = GetKeyFramesForType(property.PropertyType, doc);
            bool foundKeyFrames = true;
            if (framesNode == null)
            {
                framesNode = s_keyFramesTable["object"] as XmlElement;
                foundKeyFrames = false;
                //CoreLogger.LogStatus("Not found keyframe for type: " + property.PropertyType.Name + ".");
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

        internal void InsertMultiplePropertyTriggers(XamlTestDocument doc, XmlElement TriggersNode, Hashtable properties, Type ownerType, ArrayList keysForProperty, bool withStoryboard)
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

        internal void GeneratePropertyValueXaml(XmlDocument doc, XmlElement node, DependencyPropertyDescriptor property, Hashtable propertyValues, ArrayList keysForProperty, bool withStoryborad, int valueIndex)
        {
            node.SetAttribute("Property", property.Name);
            GenerateValueXaml(doc, node, property, propertyValues, keysForProperty, "Value", withStoryborad, valueIndex);
        }
        /// <summary>
        /// For a node like Setter, Trigger, and Condition, set the "property" attribute and Value.
        /// </summary>
        /// <param name="doc">XmlDocument for the xml generation.</param>
        /// <param name="node">Node to set Property and Value.</param>
        /// <param name="property">The property.</param>
        /// <param name="propertyValues">property value table.</param>
        /// <param name="keysForProperty"></param>
        internal void GeneratePropertyValueXaml(XmlDocument doc, XmlElement node, DependencyPropertyDescriptor property, Hashtable propertyValues, ArrayList keysForProperty)
        {
            GeneratePropertyValueXaml(doc, node, property, propertyValues, keysForProperty, false, 0);
        }
        internal void GenerateValueXaml(XmlDocument doc, XmlElement node, DependencyPropertyDescriptor property, Hashtable propertyValues, ArrayList keysForProperty, string valueFor, bool withStoryborad, int valueIndex)
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
                string key = GetReourceDictionaryKey(doc, property, propertyValues, keysForProperty, valueIndex);
                if (key == null || key.Trim().Length==0)
                    throw new Microsoft.Test.TestSetupException("Have not find the key.");
                node.SetAttribute(valueFor, "{StaticResource " + key + "}");
            }
        }

        private static string GetReourceDictionaryKey(XmlDocument doc, DependencyPropertyDescriptor property, Hashtable propertyValues, ArrayList keysForProperty, int valueIndex)
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
        internal static XmlElement InsertTemplateTriggersNode(XmlDocument doc, XmlElement elementNode)
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

        internal void SetInitialValues(XmlDocument doc, XmlElement element, Hashtable properties, ArrayList keysForProperty, int valueIndex)
        {
            foreach (DependencyPropertyDescriptor property in properties.Keys)
            {
                SetInitialValue(doc, element, property, properties, keysForProperty, valueIndex);
            }
        }

        internal void SetInitialValue(XmlDocument doc, XmlElement element, DependencyPropertyDescriptor property, Hashtable properties, ArrayList keysForProperty, int valueIndex)
        {
            GenerateValueXaml(doc, element, property, properties, keysForProperty, property.Name, false, valueIndex);
        }

        internal static XmlElement InsertMainElement(XmlDocument doc, XmlElement containerSnippet, string rootName, string nodeName, Type type)
        {
            // Insert styled element.
            XmlElement testRootNode = doc.SelectSingleNode("//*[@Name='" + rootName + "']", NamespaceManager) as XmlElement;
            XmlElement containerNode = (XmlElement)doc.ImportNode(containerSnippet, true);
            testRootNode.AppendChild(containerNode);
            containerNode = (XmlElement)containerNode.SelectSingleNode("//*[@Name='ContainerElement']", NamespaceManager);
            containerNode.RemoveAttribute("Name");

            XmlElement element = doc.CreateElement("", type.Name,
                testRootNode.NamespaceURI);
            element.SetAttribute("Name", nodeName);
            containerNode.AppendChild(element);
            return element;
        }

        internal void VerifyPropertyTrigger(FrameworkElement root, string firstName, string secondName, Hashtable properties)
        {
            CoreLogger.LogStatus("Verify EnterActions...");
            VerifyPropertyTrigger(root, "elementWithInitialValues", firstName, secondName, properties);
            CoreLogger.LogStatus("Verify ExitActions...");
            VerifyPropertyTrigger(root, "elementWithInitialValues2", firstName, secondName, properties);
        }
        internal void VerifyPropertyTrigger(FrameworkElement root, string baseName, string firstName, string secondName, Hashtable properties)
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
                CoreLogger.LogStatus("Check property: " + dpd.Name + " on the first element.");
                ActionForTypeHelper.CheckExpectedPropertyValue(firstElement, elementWithInitialValues, dpd.DependencyProperty, this.PropertiesToSkip);
                CoreLogger.LogStatus("On the second element.");
                ActionForTypeHelper.CheckExpectedPropertyValue(secondElement, elementWithInitialValues, dpd.DependencyProperty, this.PropertiesToSkip);
            }
        }
        void VerifyValueIsFromStyleOrTemplate(DependencyObject element, DependencyPropertyDescriptor dpd)
        {
            ValueSource valueSource = DependencyPropertyHelper.GetValueSource(element, dpd.DependencyProperty);
            if (valueSource.BaseValueSource != BaseValueSource.TemplateTrigger
                && valueSource.BaseValueSource != BaseValueSource.StyleTrigger
                && !valueSource.IsAnimated)
            {
                throw new Microsoft.Test.TestValidationException("'" + dpd.Name + "' property value does not come from the Trigger, Source: " + valueSource.BaseValueSource.ToString() + " and it is not Animated.");
            }
        }
        internal static bool IsReferenceEqualityTypeWityConverter(Type type)
        {
            if (type.Equals(typeof(System.Windows.Input.InputScope))) return true;
            return false;
        }
        // Returns the xml node where we should put the type tag.
        static internal void GetKeyFrames(XamlTestDocument doc)
        {
            XmlNodeList keyFramesNodes = doc.SnippetsDoc.GetElementsByTagName("KeyFrames");
            s_keyFramesTable = new Hashtable();
            foreach (XmlNode node in keyFramesNodes)
            {
                XmlElement element = node as XmlElement;
                s_keyFramesTable.Add(element.GetAttribute("Type"), element.FirstChild);
            }
        }

        static internal XmlElement GetKeyFramesForType(Type type, XamlTestDocument doc)
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
        internal static Hashtable TrimPropertyTableForStoryboard(Hashtable propertyTable)
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
        // Returns list of properties that can be used for property trigger on the given type.
        internal Hashtable GetPropertyTableForStoryboard(string typeName)
        {
            Type type = GetType(typeName);
            Hashtable properties = new Hashtable();
            
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(type))
            {
                DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(prop);
                if (StyleActionRunner.IsStylableProperty(dpd))
                {
                    //Usually, the value those property depends on some other factor, not always the value set, thus
                    //cannot be used for trigger purpose. 
                    if (ActionForTypeHelper.ShouldIgnoreProperty(dpd.Name, type, this.PropertiesToSkip))
                        continue;

                    object[] value = new object[2];
                    value[0] = ActionRunner.GetPropertyValue(this.TestDocument, typeName, dpd, 0);
                    value[1] = ActionRunner.GetPropertyValue(this.TestDocument, typeName, dpd, 1);
                    if (value[0] == null || value[1]==null)
                    {
                        CoreLogger.LogStatus("Value for: " + dpd.Name + ", which is of type : " + dpd.PropertyType.Name + ", is null. ");
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
            Hashtable trimedTable = TriggerActionRunner.TrimPropertyTableForStoryboard(properties);
            return trimedTable;
        }

        private static Hashtable s_keyFramesTable = null;
    }
}
