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
    /******************************************************************************
    * CLASS:          TemplatePropertyTriggerActionRunner
    ******************************************************************************/
    /// <summary>
    /// Implements routines for testing Avalon Property Trigger in Style, all types and properties.
    ///</summary>
    [Test(1, "Integration.TemplatePropertyTrigger", "TemplatePropertyTriggerActionRunner", SecurityLevel=TestCaseSecurityLevel.FullTrust, SupportFiles=@"FeatureTests\ElementServices\ActionForTypeExclude.xml,Common\PropertiesToSkip.xml,FeatureTests\ElementServices\PropertyTriggerActionForType_empty.xaml,FeatureTests\ElementServices\IntegrationXamlSnippets.xaml,FeatureTests\ElementServices\ActionForType_PropertiesToSkip.xml,FeatureTests\ElementServices\ControlTemplateActionForType_empty.xaml")]
    public class TemplatePropertyTriggerActionRunner : WindowTest
    {
        #region Private Data
        private static XamlTestDocument                 s_xamlTestDocument        = null;
        private static string                           s_typeName                = "";
        #endregion


        #region Constructor

        [Variation("Button")]
        [Variation("CheckBox")]

        ////////////////////////////////////////////////////////////////////////////////////////////������� 
        // DISABLEDUNSTABLETEST:
        // TestName:TemplatePropertyTriggerActionRunner(ComboBox)
        // Area: ElementServices�� SubArea: Integration.TemplatePropertyTrigger
        // Disable this case due to high fail rate, will enable after fix it.
        // to find all disabled tests in test tree, use: �findstr /snip DISABLEDUNSTABLETEST� 
        ////////////////////////////////////////////////////////////////////////////////////////////

        //[Variation("ComboBox")]

        [Variation("ComboBoxItem")]
        [Variation("ContentControl")]
        // [Variation("ContextMenu")] // [DISABLE WHILE PORTING]
        [Variation("Control")]
        [Variation("DocumentViewer")]
        [Variation("Expander")]
        [Variation("FlowDocumentReader")]
        [Variation("FlowDocumentPageViewer")]
        [Variation("FlowDocumentScrollViewer")]
        [Variation("Frame")]
        [Variation("GridSplitter")]
        [Variation("GridViewColumnHeader")]
        [Variation("GroupBox")]
        [Variation("GroupItem")]
        [Variation("HeaderedContentControl")]
        [Variation("HeaderedItemsControl")]
        [Variation("ItemsControl")]
        [Variation("Label")]
        [Variation("ListBox")]
        [Variation("ListBoxItem")]
        [Variation("ListView")]
        [Variation("ListViewItem")]
        // [DISABLE WHILE PORTING]
        // [Variation("Menu")]
        // [Variation("MenuItem")]
        [Variation("PasswordBox")]
        [Variation("ProgressBar")]
        [Variation("RepeatButton")]
        [Variation("ResizeGrip")]
        [Variation("RadioButton")]

        //////////////////////////////////////////////////////////////////////////////////////////////������� 
        // DISABLEDUNSTABLETEST:
        // TestName:TemplatePropertyTriggerActionRunner(RichTextBox)
        // Area: ElementServices�� SubArea: Integration.TemplatePropertyTrigger
        // Disable this case due to high fail rate, will enable after fix it.
        // to find all disabled tests in test tree, use: �findstr /snip DISABLEDUNSTABLETEST� 
        ////////////////////////////////////////////////////////////////////////////////////////////

        //[Variation("RichTextBox")]

        [Variation("ScrollBar")]
        [Variation("ScrollViewer")]
        [Variation("Separator")]
        [Variation("Slider")]
        // [Variation("StatusBar")] // [DISABLE WHILE PORTING]
        [Variation("StatusBarItem")]
        [Variation("TabControl")]
        [Variation("TabItem")]

        ////////////////////////////////////////////////////////////////////////////////////////////����� 
        // DISABLEDUNSTABLETEST:
        // TestName:TemplatePropertyTriggerActionRunner(TextBox)
        // Area: ElementServices�� SubArea: Integration.TemplatePropertyTrigger
        // Disable this case due to high fail rate, will enable after fix it.
        // to find all disabled tests in test tree, use: �findstr /snip DISABLEDUNSTABLETEST� 
        ////////////////////////////////////////////////////////////////////////////////////////////

        //[Variation("TextBox")]

        [Variation("Thumb")]
        [Variation("ToggleButton")]
        // [Variation("ToolBar")] // [DISABLE WHILE PORTING]
        [Variation("ToolTip")]
        [Variation("TreeView")]
        [Variation("TreeViewItem")]
        [Variation("UserControl")]

        /******************************************************************************
        * Function:          TemplatePropertyTriggerActionRunner Constructor
        ******************************************************************************/
        public TemplatePropertyTriggerActionRunner(string arg)
        {
            s_typeName = arg;

            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(DoTemplatePropertyTriggerTest);
        }
        public TemplatePropertyTriggerActionRunner() { }
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
            ActionHelper.XamlFileName = "__TemplatePropertyTriggerActionForType.xaml";
            ActionHelper.EmptyFileName= "PropertyTriggerActionForType_empty.xaml";
            s_xamlTestDocument = ActionHelper.TestDocument;

            // Set culture to 'en-us' so round-tripping will work correctly.
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfoByIetfLanguageTag("en-us");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag("en-us");

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          DoTemplatePropertyTriggerTest
        ******************************************************************************/
        /// <summary>
        /// Builds xaml with elements with Property Trigger in Template, loads it,
        /// and verifies that the instance's property values were triggered correctly.
        /// </summary>
        TestResult DoTemplatePropertyTriggerTest()
        {
            BuildTemplatePropertyTriggerXaml(s_typeName, s_xamlTestDocument);
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
            helper.DisplayTree(root, "TemplatePropertyTrigger", true);

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          GetPropertyTableForTriggerInTemplate
        ******************************************************************************/
        /// <summary>
        /// Returns list of properties that can be used for property trigger on the given type.
        /// </summary>
        private Hashtable GetPropertyTableForTriggerInTemplate(string typeName)
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
                    value[0] = ActionHelper.GetPropertyValue(s_xamlTestDocument, typeName, dpd);
                    if (value[0] == null)
                    {
                        GlobalLog.LogStatus("Value for: " + dpd.Name + ", which is of type : " + dpd.PropertyType.Name + ", is null. ");
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
            
        /******************************************************************************
        * Function:          BuildTemplatePropertyTriggerXaml
        ******************************************************************************/
        // Builds a new xaml file for the type to use for loading and verification.
        // The xaml file contains 5 instance declarations: 
        //      - StyledElement - has a Style set via property element syntax, and propertyTriggers are under the style.
        //      - StyledElementMultiTrigger - same as above except the triggers are MultiTrigger.
        //      - TemplatedElement - has a template set via property element syntax, and propertyTriggers are under the template.
        //      - TemplatedElementMultiTrigger - same as above except the triggers are MultiTrigger.
        //      - elementWithInitialValues - has locally-set properties for comparison purpose.
        private void BuildTemplatePropertyTriggerXaml(string typeName, XamlTestDocument doc)
        {
            Type type = ActionHelper.GetType(typeName, true);

//            XmlNode testRootNode = null;

            Hashtable properties = GetPropertyTableForTriggerInTemplate(typeName);
            ArrayList keysForProperty = new ArrayList();

            XmlElement containerNode = doc.GetContainerNode(type);
            // Insert element with just initial value for comparision.
            XmlElement elementWithInitialValues = TriggerActionHelper.InsertMainElement(doc, containerNode, "RootForElementWithInitialValues", "elementWithInitialValues", type);
            TriggerActionHelper.SetInitialValues(doc, elementWithInitialValues, properties, keysForProperty, 0);
            GlobalLog.LogStatus("elementWithInitialValues generated.");

            // Insert styled element where style composed of property trigger. 
            XmlElement templatedElement = TriggerActionHelper.InsertMainElement(doc, containerNode, "RootForTemplatedElement", "TemplatedElement", type);
            XmlElement templateTriggersNode = ActionHelper.InsertTemplateTriggersNode(doc, templatedElement);
            TriggerActionHelper.InsertPropertyTriggers(doc, templateTriggersNode, properties, type, keysForProperty, false);
            GlobalLog.LogStatus("templatedElement set.");

            // Insert styled element where style composed of multiple property trigger. 
            XmlElement templatedElemenMultiTrigger = TriggerActionHelper.InsertMainElement(doc, containerNode, "RootForTemplatedElementMultiTrigger", "TemplatedElementMultiTrigger", type);
            templateTriggersNode = ActionHelper.InsertTemplateTriggersNode(doc, templatedElemenMultiTrigger);
            TriggerActionHelper.InsertMultiplePropertyTriggers(doc, templateTriggersNode, properties, type, keysForProperty, false);
            GlobalLog.LogStatus("templatedElement with MultiTrigger set.");

            // Set Verifier property.
            doc.SetVerifierRoutine(this.GetType().GetMethod("VerifyTemplatePropertyTrigger"));

            // Save final xml to file.
            IOHelper.SaveTextToFile(IOHelper.ConvertXmlDocumentToStream(doc), ActionHelper.XamlFileName);
            GlobalLog.LogStatus("Xaml Generated : " + ActionHelper.XamlFileName + ".");
        }
        #endregion


        #region Verification
        /******************************************************************************
        * Function:          VerifyTemplatePropertyTrigger
        ******************************************************************************/
        /// <summary>
        /// Verifies preperty trigger after the first render.
        /// </summary>
        /// <param name="uielement"></param>
        public void VerifyTemplatePropertyTrigger(UIElement uielement)
        {
            FrameworkElement root = (FrameworkElement)uielement;
            Type type = root.FindName("TemplatedElement").GetType();
            Hashtable properties = GetPropertyTableForTriggerInTemplate(type.FullName);
            TriggerActionHelper.VerifyPropertyTrigger(root, "elementWithInitialValues", "TemplatedElement", "TemplatedElementMultiTrigger", properties);
        }
        #endregion
    }
}
