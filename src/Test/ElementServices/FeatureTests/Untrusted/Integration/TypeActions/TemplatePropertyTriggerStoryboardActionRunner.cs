// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using System.Xml;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Markup;
using Microsoft.Test.Serialization;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
using Microsoft.Test.Windows;

namespace Avalon.Test.CoreUI.Integration.TypeActions
{
    /// <summary>
    /// Implements routines for testing Avalon Property Trigger in Style, all types and properties.
    ///</summary>
    [Test(1, "Integration.TemplatePropertyTriggerStoryboard", "TemplatePropertyTriggerStoryboardActionRunner", SecurityLevel=TestCaseSecurityLevel.FullTrust, SupportFiles=@"FeatureTests\ElementServices\ActionForTypeExclude.xml,Common\PropertiesToSkip.xml,FeatureTests\ElementServices\PropertyTriggerActionForType_empty.xaml,FeatureTests\ElementServices\IntegrationXamlSnippets.xaml,FeatureTests\ElementServices\ActionForType_PropertiesToSkip.xml,FeatureTests\ElementServices\ControlTemplateActionForType_empty.xaml")]
    public class TemplatePropertyTriggerStoryboardActionRunner : WindowTest
    {
        #region Private Data
        private static XamlTestDocument                 s_xamlTestDocument        = null;
        private static string                           s_typeName                = "";
        #endregion


        #region Constructor


        [Variation("Button")]
        [Variation("CheckBox")]
        [Variation("Expander")]
        [Variation("GridViewColumnHeader")]
        [Variation("Label")]
        [Variation("ListBox")]
        [Variation("ListView")]
        [Variation("Menu")]
        [Variation("MenuItem")]
        [Variation("PasswordBox")]
        [Variation("ProgressBar")]
        [Variation("RadioButton")]
        [Variation("RepeatButton")]

        ////////////////////////////////////////////////////////////////////////////////////////////        
        // DISABLEDUNSTABLETEST:
        // TestName:TemplatePropertyTriggerStoryboardActionRunner(RichTextBox)
        // Area: ElementServices   SubArea: Integration.TemplatePropertyTriggerStoryboard
        // Disable this case due to high fail rate, will enable after fix it.
        // to find all disabled tests in test tree, use: “findstr /snip DISABLEDUNSTABLETEST” 
        ////////////////////////////////////////////////////////////////////////////////////////////
        
        //[Variation("RichTextBox")]

        [Variation("ScrollBar")]
        [Variation("StatusBar")]
//        [Variation("StatusBarItem")]
        [Variation("TabControl")]
        [Variation("TabItem")]

        ////////////////////////////////////////////////////////////////////////////////////////////        
        // DISABLEDUNSTABLETEST:
        // TestName:TemplatePropertyTriggerStoryboardActionRunner(TextBox)
        // Area: ElementServices   SubArea: Integration.TemplatePropertyTriggerStoryboard
        // Disable this case due to high fail rate, will enable after fix it.
        // to find all disabled tests in test tree, use: “findstr /snip DISABLEDUNSTABLETEST” 
        ////////////////////////////////////////////////////////////////////////////////////////////

        //[Variation("TextBox")]

        [Variation("Thumb")]
        [Variation("ToggleButton")]
        [Variation("TreeView")]
        [Variation("TreeViewItem")]

        /******************************************************************************
        * Function:          TemplatePropertyTriggerStoryboardActionRunner Constructor
        ******************************************************************************/
        public TemplatePropertyTriggerStoryboardActionRunner(string arg)
        {
            s_typeName = arg;

            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(DoTemplatePropertyTriggerStoryboardTest);
        }
        public TemplatePropertyTriggerStoryboardActionRunner() { }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Initialize.
        /// </summary>
        /// <returns>A TestResult</returns>
        TestResult Initialize()
        {
            s_typeName = ActionHelper.GetFullName(s_typeName);
            ActionHelper.XamlFileName = "__TemplatePropertyTriggerStoryboardActionForType.xaml";
            ActionHelper.EmptyFileName= "PropertyTriggerActionForType_empty.xaml";
            s_xamlTestDocument = ActionHelper.TestDocument;

            // Set culture to 'en-us' so round-tripping will work correctly.
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfoByIetfLanguageTag("en-us");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag("en-us");

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          DoTemplatePropertyTriggerStoryboardTest
        ******************************************************************************/
        /// <summary>
        /// Builds xaml with elements with Property Trigger in Template, loads it,
        /// and verifies that the instance's property values were triggered correctly.
        /// </summary>
        TestResult DoTemplatePropertyTriggerStoryboardTest()
        {
            BuildTemplatePropertyTriggerStoryboardStoryboardXaml(s_typeName, s_xamlTestDocument);
            GlobalLog.LogFile(ActionHelper.XamlFileName);

            //
            // Parse xaml.
            // Diplay tree.
            // Verify tree. 
            //
            // Verification routine is called automatically when the tree
            // is rendered.
            //
            object root = SerializationHelper.ParseXamlFile(ActionHelper.XamlFileName);

            GlobalLog.LogStatus("xaml loaded. ");

            SerializationHelper helper = new SerializationHelper();
            helper.DisplayTree(root, "TemplatePropertyTriggerStoryboardStoryboard", true);

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion

     
        #region Private Members
        /******************************************************************************
        * Function:          BuildTemplatePropertyTriggerStoryboardStoryboardXaml
        ******************************************************************************/
        // Builds a new xaml file for the type to use for loading and verification.
        // The xaml file contains 5 instance declarations: 
        //      - StyledElement - has a Style set via property element syntax, and propertyTriggers are under the style.
        //      - StyledElementMultiTrigger - same as above except the triggers are MultiTrigger.
        //      - TemplatedElement - has a template set via property element syntax, and propertyTriggers are under the template.
        //      - TemplatedElementMultiTrigger - same as above except the triggers are MultiTrigger.
        //      - elementWithInitialValues - has locally-set properties for comparison purpose.
        private void BuildTemplatePropertyTriggerStoryboardStoryboardXaml(string typeName, XamlTestDocument doc)
        {
            Type type = ActionHelper.GetType(typeName, true);

            Hashtable properties = TriggerActionHelper.GetPropertyTableForStoryboard(typeName, doc);
            ArrayList keysForProperty = new ArrayList();
            
            XmlElement containerNode = doc.GetContainerNode(type);
            // Insert element with just initial value for comparision.
            XmlElement elementWithInitialValues = TriggerActionHelper.InsertMainElement(doc, containerNode, "RootForElementWithInitialValues", "elementWithInitialValues", type);
            TriggerActionHelper.SetInitialValues(doc, elementWithInitialValues, properties, keysForProperty, 0);
            elementWithInitialValues = TriggerActionHelper.InsertMainElement(doc, containerNode, "RootForElementWithInitialValues2", "elementWithInitialValues2", type);
            TriggerActionHelper.SetInitialValues(doc, elementWithInitialValues, properties, keysForProperty, 1);
            GlobalLog.LogStatus("elementWithInitialValues generated.");

            // Insert styled element where style composed of property trigger. 
            XmlElement templatedElement = TriggerActionHelper.InsertMainElement(doc, containerNode, "RootForTemplatedElement", "TemplatedElement", type);
            XmlElement templateTriggersNode = InsertTemplateTriggersNode(doc, templatedElement);
            TriggerActionHelper.InsertPropertyTriggers(doc, templateTriggersNode, properties, type, keysForProperty, true);
            GlobalLog.LogStatus("templatedElement set.");

            // Insert styled element where style composed of multiple property trigger with storyboard.
            XmlElement templatedElemenMultiTrigger = TriggerActionHelper.InsertMainElement(doc, containerNode, "RootForTemplatedElementMultiTrigger", "TemplatedElementMultiTrigger", type);
            templateTriggersNode = InsertTemplateTriggersNode(doc, templatedElemenMultiTrigger);
            TriggerActionHelper.InsertMultiplePropertyTriggers(doc, templateTriggersNode, properties, type, keysForProperty, true);
            GlobalLog.LogStatus("templatedElement with MultiTrigger set.");

            // Set Verifier property.
            doc.SetVerifierRoutine(this.GetType().GetMethod("VerifyTemplatePropertyTriggerStoryboardStoryboard"));

            // Save final xml to file.
            IOHelper.SaveTextToFile(IOHelper.ConvertXmlDocumentToStream(doc), ActionHelper.XamlFileName);
            GlobalLog.LogStatus("Xaml Generated : " + ActionHelper.XamlFileName + ".");
        }

        /******************************************************************************
        * Function:          InsertTemplateTriggersNode
        ******************************************************************************/
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
        private XmlElement InsertTemplateTriggersNode(XmlDocument doc, XmlElement elementNode)
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
        #endregion


        #region Verification
        /******************************************************************************
        * Function:          VerifyTemplatePropertyTriggerStoryboardStoryboard
        ******************************************************************************/
        /// <summary>
        /// Verifies preperty trigger after the first render.
        /// </summary>
        /// <param name="uielement"></param>
        public void VerifyTemplatePropertyTriggerStoryboardStoryboard(UIElement uielement)
        {
            FrameworkElement root = (FrameworkElement)uielement;

            Type type = root.FindName("TemplatedElement").GetType();
            Hashtable properties = TriggerActionHelper.GetPropertyTableForStoryboard(type.FullName, s_xamlTestDocument);
            TriggerActionHelper.VerifyPropertyTrigger(root, "TemplatedElement", "TemplatedElementMultiTrigger", properties);
        }
        #endregion
    }
}
