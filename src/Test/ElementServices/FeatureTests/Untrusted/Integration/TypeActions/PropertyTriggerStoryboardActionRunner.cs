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
    public class TemplatePropertyTriggerStoryboardActionRunner : TemplatePropertyTriggerActionRunner
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public TemplatePropertyTriggerStoryboardActionRunner() : base()
        {
        }

        /// <summary>
        /// Builds xaml with elements with Property Trigger in Template, loads it,
        /// and verifies that the instance's property values were triggered correctly.
        /// </summary>
        new public void DoTemplatePropertyTriggerTest(string typeName, ActionForType actionForType)
        {
            BuildTemplatePropertyTriggerStoryboardXaml(typeName);
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
     
        // Builds a new xaml file for the type to use for loading and verification.
        // The xaml file contains 5 instance declarations: 
        //      - StyledElement - has a Style set via property element syntax, and propertyTriggers are under the style.
        //      - StyledElementMultiTrigger - same as above except the triggers are MultiTrigger.
        //      - TemplatedElement - has a template set via property element syntax, and propertyTriggers are under the template.
        //      - TemplatedElementMultiTrigger - same as above except the triggers are MultiTrigger.
        //      - elementWithInitialValues - has locally-set properties for comparison purpose.
        
        private void BuildTemplatePropertyTriggerStoryboardXaml(string typeName)
        {
            Type type = GetType(typeName);

            XamlTestDocument doc = this.TestDocument;

//            XmlNode testRootNode = null;

            Hashtable properties = this.GetPropertyTableForStoryboard(typeName);
            ArrayList keysForProperty = new ArrayList();
            
            XmlElement containerNode = doc.GetContainerNode(type);
            // Insert element with just initial value for comparision.
            XmlElement elementWithInitialValues = TriggerActionRunner.InsertMainElement(doc, containerNode, "RootForElementWithInitialValues", "elementWithInitialValues", type);
            this.SetInitialValues(doc, elementWithInitialValues, properties, keysForProperty, 0);
            elementWithInitialValues = TriggerActionRunner.InsertMainElement(doc, containerNode, "RootForElementWithInitialValues2", "elementWithInitialValues2", type);
            this.SetInitialValues(doc, elementWithInitialValues, properties, keysForProperty, 1);
            CoreLogger.LogStatus("elementWithInitialValues generated.");

            // Insert styled element where style composed of property trigger. 
            XmlElement templatedElement = TriggerActionRunner.InsertMainElement(doc, containerNode, "RootForTemplatedElement", "TemplatedElement", type);
            XmlElement templateTriggersNode = InsertTemplateTriggersNode(doc, templatedElement);
            this.InsertPropertyTriggers(doc, templateTriggersNode, properties, type, keysForProperty, true);
            CoreLogger.LogStatus("templatedElement set.");

            // Insert styled element where style composed of multiple property trigger with storyboard.
            XmlElement templatedElemenMultiTrigger = TriggerActionRunner.InsertMainElement(doc, containerNode, "RootForTemplatedElementMultiTrigger", "TemplatedElementMultiTrigger", type);
            templateTriggersNode = InsertTemplateTriggersNode(doc, templatedElemenMultiTrigger);
            this.InsertMultiplePropertyTriggers(doc, templateTriggersNode, properties, type, keysForProperty, true);
            CoreLogger.LogStatus("templatedElement with MultiTrigger set.");

            // Set Verifier property.
            doc.SetVerifierRoutine(this.GetType()).GetMethod("VerifyTemplatePropertyTriggerStoryboard");

            // Save final xml to file.
            IOHelper.SaveTextToFile(IOHelper.ConvertXmlDocumentToStream(doc), this.XamlFileName);
            CoreLogger.LogStatus("Xaml Generated : " + this.XamlFileName + ".");
        }

        /// <summary>
        /// Verifies preperty trigger after the first render.
        /// </summary>
        /// <param name="uielement"></param>
        public void VerifyTemplatePropertyTriggerStoryboard(UIElement uielement)
        {
            FrameworkElement root = (FrameworkElement)uielement;

            Type type = root.FindName("TemplatedElement").GetType();
            Hashtable properties = this.GetPropertyTableForStoryboard(type.FullName);
            this.VerifyPropertyTrigger(root, "TemplatedElement", "TemplatedElementMultiTrigger", properties);
        }

        /// <summary>
        /// Insert two layers like
        /// <Element.Template>
        ///     <ControlTemplate>
        ///     </ControlTemplate>
        /// </Element.Template>
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="elementNode">The node under which to insert this.</param>
        /// <returns>inner Template node</returns>
        new private XmlElement InsertTemplateTriggersNode(XmlDocument doc, XmlElement elementNode)
        {
            XmlElement templateComplexProp = doc.CreateElement("", elementNode.Name + ".Template", elementNode.NamespaceURI);
            elementNode.InsertAfter(templateComplexProp, null);

            XmlElement templateElement = doc.CreateElement("ControlTemplate", XamlGenerator.AvalonXmlns);
            // Set TargetType attribute on Template.
            templateElement.SetAttribute("TargetType", "{x:Type " + elementNode.Name + "}");
            templateComplexProp.InsertAfter(templateElement, null);

            // Insert dummy element as template root.
            XmlElement dummyTemplateRoot = doc.CreateElement("Rectangle", XamlGenerator.AvalonXmlns);
            dummyTemplateRoot.SetAttribute("Fill", "{x:Null}");
            dummyTemplateRoot.SetAttribute("Height", "0");
            dummyTemplateRoot.SetAttribute("Width", "0");
            templateElement.InsertAfter(dummyTemplateRoot, null);

            XmlElement triggersElement = doc.CreateElement("ControlTemplate.Triggers", XamlGenerator.AvalonXmlns);
            templateElement.InsertAfter(triggersElement, dummyTemplateRoot);
            return triggersElement;
        }
    }

    /// <summary>
    /// Implements routines for testing Avalon Property Trigger in Style, all types and properties.
    ///</summary>
    public class StylePropertyTriggerStoryboardActionRunner : StylePropertyTriggerActionRunner
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public StylePropertyTriggerStoryboardActionRunner() : base()
        {
            this.PropertiesToSkipFileName = "StylePropertyTriggerActionForType_PropertiesToSkip.xml";
        }

        /// <summary>
        /// Builds xaml with elements with Property Trigger in Style/Template, loads it,
        /// and verifies that the instance's property values were triggered correctly.
        /// </summary>
        public void DoStylePropertyTriggerStoryboardTest(string typeName, ActionForType actionForType)
        {
            BuildStylePropertyTriggerStoryboardXaml(typeName);
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

        // Builds a new xaml file for the type to use for loading and verification.
        // The xaml file contains 5 instance declarations: 
        //      - StyledElement - has a Style set via property element syntax, and propertyTriggers are under the style.
        //      - StyledElementMultiTrigger - same as above except the triggers are MultiTrigger.
        //      - TemplatedElement - has a template set via property element syntax, and propertyTriggers are under the template.
        //      - TemplatedElementMultiTrigger - same as above except the triggers are MultiTrigger.
        //      - elementWithInitialValues - has locally-set properties for comparison purpose.

        private void BuildStylePropertyTriggerStoryboardXaml(string typeName)
        {
            Type type = GetType(typeName);

            XamlTestDocument doc = this.TestDocument;

//            XmlNode testRootNode = null;

            Hashtable properties = this.GetPropertyTableForStoryboard(typeName);
            ArrayList keysForProperty = new ArrayList();

            XmlElement containerNode = doc.GetContainerNode(type);
            // Insert element with just initial value for comparision.
            XmlElement elementWithInitialValues = TriggerActionRunner.InsertMainElement(doc, containerNode, "RootForElementWithInitialValues", "elementWithInitialValues", type);
            this.SetInitialValues(doc, elementWithInitialValues, properties, keysForProperty, 0);
            elementWithInitialValues = TriggerActionRunner.InsertMainElement(doc, containerNode, "RootForElementWithInitialValues2", "elementWithInitialValues2", type);
            this.SetInitialValues(doc, elementWithInitialValues, properties, keysForProperty, 1);
            CoreLogger.LogStatus("elementWithInitialValues generated.");
            //Testing EnterActions
            // Insert styled element where style composed of property trigger. 
            XmlElement styledElement = TriggerActionRunner.InsertMainElement(doc, containerNode, "RootForStyledElement", "StyledElement", type);
            XmlElement styleTriggersNode = InsertStyleTriggersNode(doc, styledElement);
            this.InsertPropertyTriggers(doc, styleTriggersNode, properties, type, keysForProperty, true);
            CoreLogger.LogStatus("styledElement set.");

            // Insert styled element where style composed of multiple property trigger. 
            XmlElement styledElementMultiTrigger = TriggerActionRunner.InsertMainElement(doc, containerNode, "RootForStyledElementMultiTrigger", "StyledElementMultiTrigger", type);
            styleTriggersNode = InsertStyleTriggersNode(doc, styledElementMultiTrigger);
            this.InsertMultiplePropertyTriggers(doc, styleTriggersNode, properties, type, keysForProperty, true);
            CoreLogger.LogStatus("styledElement with MultiTrigger set.");

            // Set Verifier property.
            doc.SetVerifierRoutine(this.GetType()).GetMethod("VerifyStylePropertyTriggerStoryboard");

            // Save final xml to file.
            IOHelper.SaveTextToFile(IOHelper.ConvertXmlDocumentToStream(doc), this.XamlFileName);
            CoreLogger.LogStatus("Xaml Generated : " + this.XamlFileName + ".");
        }
        /// <summary>
        /// Verifies preperty trigger after the first render.
        /// </summary>
        /// <param name="uielement"></param>
        public void VerifyStylePropertyTriggerStoryboard(UIElement uielement)
        {
            FrameworkElement root = (FrameworkElement)uielement;
            Type type = root.FindName("StyledElement").GetType();
            Hashtable properties = this.GetPropertyTableForStoryboard(type.FullName);
            this.VerifyPropertyTrigger(root, "StyledElement", "StyledElementMultiTrigger", properties);
        }
    }
}
