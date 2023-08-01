// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
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
    /******************************************************************************
    * CLASS:          ElementBindingActionRunner
    ******************************************************************************/
    /// <summary>
    /// Implements routines for testing Avalon ElementBinding in Style, all types and properties.
    ///</summary>
    [Test(1, "Integration.ElementBinding", "ElementBindingActionRunner", SecurityLevel=TestCaseSecurityLevel.FullTrust, SupportFiles=@"FeatureTests\ElementServices\ActionForTypeExclude.xml,Common\PropertiesToSkip.xml,FeatureTests\ElementServices\PropertyTriggerActionForType_empty.xaml,FeatureTests\ElementServices\IntegrationXamlSnippets.xaml,FeatureTests\ElementServices\MouseActionIntegrationXamlSnippets.xaml,FeatureTests\ElementServices\ActionForType_PropertiesToSkip.xml,FeatureTests\ElementServices\ElementBindingActionForType_empty.xaml")]
    public class ElementBindingActionRunner : WindowTest
    {
        #region Private Data
        private static XamlTestDocument                 s_xamlTestDocument        = null;
        private static string                           s_typeName                = "";
        private const string                            baseElementName         = "elementWithInitialValues";
        #endregion


        #region Constructor

        [Variation("AccessText")]
        [Variation("AdornerDecorator")]
        [Variation("Border")]
        [Variation("Button")]
        [Variation("Canvas")]
        [Variation("CheckBox")]
        [Variation("ComboBox")]
        [Variation("ComboBoxItem")]
        [Variation("ContentControl")]
        [Variation("Control")]
        [Variation("Decorator")]
        [Variation("Ellipse")]
        [Variation("Expander")]
        [Variation("FixedPage")]
        [Variation("FlowDocumentPageViewer")]
        [Variation("FlowDocumentReader")]
        [Variation("FlowDocumentScrollViewer")]
        [Variation("Frame")]
        [Variation("Glyphs")]
        [Variation("Grid")]
        [Variation("GridSplitter")]
        [Variation("GridViewColumnHeader")]
        [Variation("GroupBox")]
        [Variation("GroupItem")]
        [Variation("HeaderedContentControl")]
        [Variation("HeaderedItemsControl")]
        [Variation("Image")]
        [Variation("InkCanvas")]
        [Variation("InkPresenter")]
        [Variation("ItemsControl")]
        [Variation("Label")]
        [Variation("ListBox")]
        [Variation("ListBoxItem")]
        [Variation("ListView")]
        [Variation("ListViewItem")]
        [Variation("MediaElement")]
        // [DISABLE WHILE PORTING]
        // [Variation("Menu")]
        // [Variation("MenuItem")]
        [Variation("Popup")]
        [Variation("PasswordBox")]
        [Variation("ProgressBar")]
        [Variation("RepeatButton")]
        [Variation("RadioButton")]
        [Variation("Rectangle")]
        [Variation("ResizeGrip")]
        [Variation("RichTextBox")]
        [Variation("ScrollBar")]
        [Variation("ScrollContentPresenter")]
        [Variation("ScrollViewer")]
        [Variation("Separator")]
        [Variation("Slider")]
        [Variation("StackPanel")]
        // [DISABLE WHILE PORTING]
        // [Variation("StatusBar")]
        [Variation("StatusBarItem")]
        [Variation("TabControl")]
        [Variation("TabItem")]
        [Variation("TextBlock")]
        [Variation("TextBox")]
        [Variation("Thumb")]
        [Variation("TickBar")]
        [Variation("ToggleButton")]
        [Variation("TreeView")]
        [Variation("TreeViewItem")]
        [Variation("UserControl")]
        [Variation("Viewbox")]
        [Variation("Viewport3D")]

        /******************************************************************************
        * Function:          ElementBindingActionRunner Constructor
        ******************************************************************************/
        public ElementBindingActionRunner(string arg)
        {
            s_typeName = arg;

            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(DoElementBindingTest);
        }
        public ElementBindingActionRunner() { }
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
            ActionHelper.XamlFileName = "__ElementBindingActionForType.xaml";
            ActionHelper.EmptyFileName= "ElementBindingActionForType_empty.xaml";
            s_xamlTestDocument = ActionHelper.TestDocument;

            // Set culture to 'en-us' so round-tripping will work correctly.
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfoByIetfLanguageTag("en-us");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag("en-us");

            return TestResult.Pass;
        }

        /// <summary>
        /// Builds xaml with elements with ElementBinding in Template, loads it,
        /// and verifies that the instance's property values were triggered correctly.
        /// </summary>
        TestResult DoElementBindingTest()
        {
            BuildElementBindingXaml(s_typeName, s_xamlTestDocument);
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
            helper.DisplayTree(root, "ElementBinding", true);

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Private Members        
        /******************************************************************************
        * Function:          BuildElementBindingXaml
        ******************************************************************************/
        // Builds a new xaml file for the type to use for loading and verification.
        // The xaml file contains 5 instance declarations: 
        //      - StyledElement - has a Style set via property element syntax, and ElementBindings are under the style.
        //      - StyledElementMultiTrigger - same as above except the triggers are MultiTrigger.
        //      - TemplatedElement - has a template set via property element syntax, and ElementBindings are under the template.
        //      - TemplatedElementMultiTrigger - same as above except the triggers are MultiTrigger.
        //      - elementWithInitialValues - has locally-set properties for comparison purpose.
        private void BuildElementBindingXaml(string typeName, XamlTestDocument doc)
        {
            Type type = ActionHelper.GetType(typeName, true);

            XmlElement resourcesNode = doc.DocumentElement.FirstChild as XmlElement;

            //XmlNode testRootNode = null;

            List<DependencyPropertyDescriptor> properties = ActionHelper.GetPropertyTableElementBinding(typeName);
            ArrayList keysForProperty = new ArrayList();
            // Insert styled element with locally set value. 
            XmlElement containerNode = doc.GetContainerNode(type);
            XmlElement elementWithInitialValues = TriggerActionHelper.InsertMainElement(doc, containerNode, "RootForElementWithInitialValues", baseElementName, type);
            TriggerActionHelper.SetInitialValues(doc, elementWithInitialValues, properties);
            GlobalLog.LogStatus("elementWithInitialValues generated.");


            // Insert element with Bindings. 
            XmlElement elementWithBindings = TriggerActionHelper.InsertMainElement(doc, containerNode, "RootForElementWithBindings", "elementWithBindings", type);
            ActionHelper.InsertBindings(doc, elementWithBindings, properties, false, baseElementName);
            GlobalLog.LogStatus("elementWithBindings generated.");

            // Insert element with Bindings in Style
            XmlElement elementWithBindingsInStyle = TriggerActionHelper.InsertMainElement(doc, containerNode, "RootForElementWithBindingsInStyle", "elementWithBindingsInStyle", type);
            ApplyStyle(doc, elementWithBindingsInStyle, properties);
            GlobalLog.LogStatus("elementWithBindingsInStyle generated.");

            if (type.IsSubclassOf(typeof(Control)) || type.Equals(typeof(Control)))
            {
                // Insert element with Bindings in Template
                XmlElement elementWithBindingsInTemplate = TriggerActionHelper.InsertMainElement(doc, containerNode, "RootForElementWithBindingsInTemplate", "elementWithBindingsInTemplate", type);
                ApplyTemplate(doc, elementWithBindingsInTemplate, properties, "ControlTemplate");
                GlobalLog.LogStatus("elementWithBindingsInTemplate generated.");
            }

            if (type.IsSubclassOf(typeof(ContentControl)) || type.Equals(typeof(ContentControl)))
            {
                // Insert element with Bindings in DataTemplate
                XmlElement elementWithBindingsInDataTemplate = TriggerActionHelper.InsertMainElement(doc, containerNode, "RootForElementWithBindingsInDataTemplate", "elementWithBindingsInDataTemplate", type);
                ApplyTemplate(doc, elementWithBindingsInDataTemplate, properties, "DataTemplate");
                GlobalLog.LogStatus("elementWithBindingsInDataTemplate generated.");
            }
            //Insert an element inside resource dictionary
            XmlElement styleNode = InsertStyleNode(doc, resourcesNode, properties, type.Name);
            styleNode.SetAttribute("Key", XamlGenerator.AvalonXmlnsX, "styleInResources");


            XmlElement elementWithStyleInResources = TriggerActionHelper.InsertMainElement(doc, containerNode, "RootForElementWithStyleInResources", "ElementWithStyleInResources", type);
            elementWithStyleInResources.SetAttribute("Style", "{DynamicResource styleInResources}");

            // Set Verifier property.
            doc.SetVerifierRoutine(this.GetType().GetMethod("VerifyElementBinding"));

            // Save final xml to file.
            IOHelper.SaveTextToFile(IOHelper.ConvertXmlDocumentToStream(doc), ActionHelper.XamlFileName);
            GlobalLog.LogStatus("Xaml Generated : " + ActionHelper.XamlFileName + ".");
        }

        /******************************************************************************
        * Function:          ApplyTemplate
        ******************************************************************************/
        private static void ApplyTemplate(XmlDocument doc, XmlElement element, List<DependencyPropertyDescriptor> properties, string templateTypeName)
        {
            string templatePropertyName = templateTypeName.StartsWith("Data") ? "ContentTemplate" : "Template";
            XmlElement dotTemplate = doc.CreateElement("", element.Name + "." + templatePropertyName,
                 element.NamespaceURI);
            element.InsertAfter(dotTemplate, null);
            XmlElement templateNode = doc.CreateElement(templateTypeName, XamlGenerator.AvalonXmlns);
            //templateNode.SetAttribute("TargetType", element.Name);
            dotTemplate.InsertAfter(templateNode, null);

            XmlElement vt = doc.CreateElement("", element.Name, element.NamespaceURI);
            vt.SetAttribute("Name", "elementInTemplate");
            templateNode.InsertAfter(vt, null);
            ActionHelper.InsertBindings(doc, vt, properties, false, baseElementName);
        }

        /******************************************************************************
        * Function:          ApplyStyle
        ******************************************************************************/
        private static void ApplyStyle(XmlDocument doc, XmlElement element, List<DependencyPropertyDescriptor> properties)
        {
            XmlElement dotStyle = doc.CreateElement("", element.Name + ".Style",
                 element.NamespaceURI);
            element.InsertAfter(dotStyle, null);
            InsertStyleNode(doc, dotStyle, properties, element.Name);
        }

        /******************************************************************************
        * Function:          InsertStyleNode
        ******************************************************************************/
        private static XmlElement InsertStyleNode(XmlDocument doc, XmlElement element, List<DependencyPropertyDescriptor> properties, string targetType)
        {
            XmlElement styleNode = doc.CreateElement("Style",  XamlGenerator.AvalonXmlns);
            styleNode.SetAttribute("TargetType", targetType);
            element.InsertAfter(styleNode, null);

            foreach (DependencyPropertyDescriptor dpd in properties)
            {
                XmlElement setter = doc.CreateElement("Setter", XamlGenerator.AvalonXmlns);
                styleNode.InsertAfter(setter, null);
                setter.SetAttribute("Property", dpd.Name);
                XmlElement setterValue = doc.CreateElement("Setter.Value", XamlGenerator.AvalonXmlns);
                setter.InsertAfter(setterValue, null);
                ActionHelper.InsertBindingNode(doc, setterValue, dpd, baseElementName);
            }
            return styleNode;
        }
        #endregion
            
        #region Verification
        /******************************************************************************
        * Function:          VerifyElementBinding
        ******************************************************************************/
        /// <summary>
        /// Verifies preperty trigger after the first render.
        /// </summary>
        /// <param name="uielement"></param>
        public void VerifyElementBinding(UIElement uielement)
        {
            GlobalLog.LogStatus("Verify ElementBindings...");
            FrameworkElement root = (FrameworkElement)uielement;

            DispatcherHelper.DoEvents(500);

            Type type = root.FindName(baseElementName).GetType();
            List<DependencyPropertyDescriptor> properties = ActionHelper.GetPropertyTableElementBinding(type.FullName);

            GlobalLog.LogStatus("Verify ElementWithtBinding.", ConsoleColor.Blue);
            DependencyObject elementWithInitialValues = root.FindName(baseElementName) as DependencyObject;
            DependencyObject ElementWithtBinding = root.FindName("ElementWithBindings") as DependencyObject;
            ActionHelper.CompareElements(properties, elementWithInitialValues, ElementWithtBinding, ActionHelper.PropertiesToSkip);

            GlobalLog.LogStatus("Verify ElementWithtBindingInStyle.", ConsoleColor.Blue);
            DependencyObject ElementWithtBindingInStyle = root.FindName("ElementWithBindingsInStyle") as DependencyObject;
            ActionHelper.CompareElements(properties, elementWithInitialValues, ElementWithtBindingInStyle, ActionHelper.PropertiesToSkip);

            GlobalLog.LogStatus("Verify ElementWithtBindingInTemplate.", ConsoleColor.Blue);
            FrameworkElement templatedParent = root.FindName("ElementWithBindingsInTemplate") as FrameworkElement;
            if (templatedParent is Control)
            {
                DependencyObject ElementWithtBindingInTemplate = ((Control)templatedParent).Template.FindName("elementInTemplate", templatedParent) as DependencyObject;
                ActionHelper.CompareElements(properties, elementWithInitialValues, ElementWithtBinding, ActionHelper.PropertiesToSkip);
            }

            GlobalLog.LogStatus("Verify ElementWithtBindingInElementBinding.", ConsoleColor.Blue);
            templatedParent = root.FindName("ElementWithBindingsInElementBinding") as FrameworkElement;
            if (templatedParent is ContentControl)
            {
                DependencyObject ElementWithtBindingInElementBinding = ((Control)templatedParent).Template.FindName("elementInTemplate", templatedParent) as DependencyObject;
                ActionHelper.CompareElements(properties, elementWithInitialValues, ElementWithtBindingInElementBinding, ActionHelper.PropertiesToSkip);
            }

            GlobalLog.LogStatus("Verify elementInResources.", ConsoleColor.Blue);
            DependencyObject elementWithStyleInResources = root.FindName("ElementWithStyleInResources") as DependencyObject;
            if (null == elementWithStyleInResources)
            {
                throw new Microsoft.Test.TestValidationException("Not found element WithStyleInResources.");
            }

            ActionHelper.CompareElements(properties, elementWithInitialValues, elementWithStyleInResources, ActionHelper.PropertiesToSkip);
        }
        #endregion
    }
}
