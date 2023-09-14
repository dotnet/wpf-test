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
    * CLASS:          StylePropertyTriggerActionRunner
    ******************************************************************************/
    /// <summary>
    /// Implements routines for testing Avalon Style in Style, all types and properties.
    ///</summary>
    [Test(1, "Integration.StylePropertyTrigger", "StylePropertyTriggerActionRunner", SecurityLevel=TestCaseSecurityLevel.FullTrust, SupportFiles=@"FeatureTests\ElementServices\ActionForTypeExclude.xml,Common\PropertiesToSkip.xml,FeatureTests\ElementServices\IntegrationXamlSnippets.xaml,FeatureTests\ElementServices\MouseActionIntegrationXamlSnippets.xaml,FeatureTests\ElementServices\ActionForType_PropertiesToSkip.xml,FeatureTests\ElementServices\PropertyTriggerActionForType_empty.xaml")]
    public class StylePropertyTriggerActionRunner : WindowTest
    {
        #region Private Data
        private static XamlTestDocument                 s_xamlTestDocument        = null;
        private static string                           s_typeName                = "";
        #endregion


        #region Constructor

        [Variation("AccessText")]
        [Variation("AdornerDecorator")]
        [Variation("BlockUIContainer")]
        [Variation("Bold")]
        [Variation("Border")]
        [Variation("BulletDecorator")]
        [Variation("Button")]
        [Variation("Canvas")]
        [Variation("CheckBox")]
        [Variation("ColumnDefinition")]
        // [DISABLE WHILE PORTING]
        // [Variation("ComboBox", Versions = "3.0SP1,3.0SP2,AH")]  // Failing regularly on only 4.X, disabled there. 
        [Variation("ComboBoxItem")]
        [Variation("ContentControl")]
        [Variation("ContentPresenter")]
        // [Variation("ContextMenu")] // [DISABLE WHILE PORTING]
        [Variation("Control")]
        [Variation("Decorator")]
        [Variation("DockPanel")]
        [Variation("DocumentPageView")]
        [Variation("DocumentReference")]
        [Variation("DocumentViewer")]
        [Variation("Ellipse")]
        [Variation("Expander")]
        [Variation("Figure")]
        [Variation("FixedDocument")]
        [Variation("FixedDocumentSequence")]
        [Variation("FixedPage", Disabled=true)] // Broken for both 4.0 and 3.X, disabled.  
        [Variation("Floater")]
        [Variation("FlowDocument")]
        [Variation("FlowDocumentPageViewer")]
        [Variation("FlowDocumentReader")]
        [Variation("FlowDocumentScrollViewer")]
        [Variation("Frame")]
        [Variation("FrameworkContentElement")]
        [Variation("FrameworkElement")]
        [Variation("Glyphs")]
        [Variation("Grid")]
        [Variation("GridSplitter")]
        [Variation("GridViewColumnHeader")]
        [Variation("GroupBox")]
        [Variation("GroupItem")]
        [Variation("HeaderedContentControl")]
        [Variation("HeaderedItemsControl")]
        [Variation("Hyperlink")]
        [Variation("Image")]
        [Variation("InkCanvas")]
        [Variation("InkPresenter")]
        [Variation("InlineUIContainer")]
        [Variation("Italic")]
        [Variation("ItemsControl")]
        [Variation("ItemsPresenter")]
        [Variation("Label")]
        [Variation("Line")]
        [Variation("LineBreak")]
        [Variation("List")]
        [Variation("ListBox")]
        [Variation("ListBoxItem")]
        [Variation("ListItem")]
        [Variation("ListView")]
        [Variation("ListViewItem")]
        // [Variation("Menu")]
        // [Variation("MenuItem")]
        [Variation("PageContent")]
        [Variation("Paragraph")]
        [Variation("PasswordBox")]
        [Variation("Path")]
        [Variation("Polygon")]
        [Variation("Polyline")]
        [Variation("Popup")]
        [Variation("ProgressBar")]
        [Variation("RadioButton")]
        [Variation("RepeatButton")]
        [Variation("Rectangle")]
        [Variation("ResizeGrip")]
        [Variation("RichTextBox", Versions = "3.0SP1,3.0SP2,AH")]  // Failing regularly on only 4.X, disabled there. 
        [Variation("RowDefinition")]
        [Variation("Run")]
        [Variation("Section")]
        [Variation("Separator")]
        [Variation("ScrollBar")]
        [Variation("ScrollContentPresenter")]
        [Variation("ScrollViewer")]
        [Variation("Section")]
        [Variation("Separator")]
        [Variation("Slider")]
        [Variation("Span")]
        [Variation("StackPanel")]
        // [Variation("StatusBar")] // [DISABLE WHILE PORTING]
        [Variation("StatusBarItem")]
        [Variation("TabControl")]
        [Variation("TabItem")]
        [Variation("Table")]
        [Variation("TableCell")]
        [Variation("TableColumn")]
        [Variation("TableRow")]
        [Variation("TableRowGroup")]
        [Variation("TabPanel")]
        [Variation("TextBlock")]
        [Variation("TextBox", Versions = "3.0SP1,3.0SP2,AH")]  // Failing regularly on only 4.X, disabled there. 
        [Variation("Thumb")]
        [Variation("TickBar")]
        [Variation("ToggleButton")]
        [Variation("ToolBarOverflowPanel")]
        // [DISABLE WHILE PORTING]
        // [Variation("ToolBar", Versions = "3.0SP1,3.0SP2,AH")]  // Failing regularly on only 4.X, disabled there. 
        [Variation("ToolBarPanel")]
        [Variation("ToolBarTray")]
        [Variation("ToolTip")]
        [Variation("Track")]
        [Variation("TreeView")]
        [Variation("TreeViewItem")]
        [Variation("Underline")]
        [Variation("UniformGrid")]
        [Variation("UserControl")]
        [Variation("Viewbox")]
        [Variation("Viewport3D")]
        [Variation("VirtualizingStackPanel")]
        [Variation("WebBrowser")]
        [Variation("WrapPanel")]

        /******************************************************************************
        * Function:          StylePropertyTriggerActionRunner Constructor
        ******************************************************************************/
        public StylePropertyTriggerActionRunner(string arg)
        {
            s_typeName = arg;

            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(DoStylePropertyTriggerTest);
        }
        public StylePropertyTriggerActionRunner() { }
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
            ActionHelper.XamlFileName = "__StylePropertyTriggerActionForType.xaml";
            ActionHelper.EmptyFileName= "PropertyTriggerActionForType_empty.xaml";
            s_xamlTestDocument = ActionHelper.TestDocument;

            // Set culture to 'en-us' so round-tripping will work correctly.
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfoByIetfLanguageTag("en-us");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag("en-us");

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          DoStylePropertyTriggerTest
        ******************************************************************************/
        /// <summary>
        /// Builds xaml with elements with Property Trigger in Style/Template, loads it,
        /// and verifies that the instance's property values were triggered correctly.
        /// </summary>
        TestResult DoStylePropertyTriggerTest()
        {
            BuildStylePropertyTriggerXaml(s_typeName, s_xamlTestDocument);
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
            helper.DisplayTree(root, "StylePropertyTrigger", true);

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          GetPropertyTableForTriggerInStyle
        ******************************************************************************/
        /// <summary>
        /// Returns list of properties that can be used for property trigger on the given type.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        private Hashtable GetPropertyTableForTriggerInStyle(string typeName, XamlTestDocument doc)
        {
            Type type = ActionHelper.GetType(typeName, true);
            Hashtable properties = new Hashtable();
            
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(type))
            {
                DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(prop);                
                if (ActionHelper.IsStylableProperty(dpd))
                {
                    // setting Template causes a lot of property changes.. 
                    if (dpd != null && String.Equals(dpd.Name, "Template", StringComparison.InvariantCulture))
                        continue;
                    
                    //Usually, the value those property depends on some other factor, not always the value set, thus
                    //cannot be used for trigger purpose. 
                    if (ActionForTypeHelper.ShouldIgnoreProperty(dpd.Name, type, ActionHelper.PropertiesToSkip))
                        continue;
                    object[] value = new object[2];
                    value[0] = ActionHelper.GetPropertyValue(doc, typeName, dpd);

                    properties.Add(dpd, value);
                }
            }

            return properties;
        }

        /******************************************************************************
        * Function:          BuildStylePropertyTriggerXaml
        ******************************************************************************/
        // Builds a new xaml file for the type to use for loading and verification.
        // The xaml file contains 5 instance declarations: 
        //      - StyledElement - has a Style set via property element syntax, and propertyTriggers are under the style.
        //      - StyledElementMultiTrigger - same as above except the triggers are MultiTrigger.
        //      - TemplatedElement - has a template set via property element syntax, and propertyTriggers are under the template.
        //      - TemplatedElementMultiTrigger - same as above except the triggers are MultiTrigger.
        //      - elementWithInitialValues - has locally-set properties for comparison purpose.
        private void BuildStylePropertyTriggerXaml(string typeName, XamlTestDocument doc)
        {
            Type type = ActionHelper.GetType(typeName, true);

//            XmlNode testRootNode = null;

            Hashtable properties = GetPropertyTableForTriggerInStyle(typeName, doc);
            ArrayList keysForProperty = new ArrayList();

            XmlElement containerNode = doc.GetContainerNode(type);
            // Insert element with just initial value for comparision.
            XmlElement elementWithInitialValues = TriggerActionHelper.InsertMainElement(doc, containerNode, "RootForElementWithInitialValues", "elementWithInitialValues", type);
            TriggerActionHelper.SetInitialValues(doc, elementWithInitialValues, properties, keysForProperty, 0);
            GlobalLog.LogStatus("elementWithInitialValues generated.");

            // Insert styled element where style composed of property trigger. 
            XmlElement styledElement = TriggerActionHelper.InsertMainElement(doc, containerNode, "RootForStyledElement", "StyledElement", type);
            XmlElement styleTriggersNode = ActionHelper.InsertStyleTriggersNode(doc, styledElement);
            TriggerActionHelper.InsertPropertyTriggers(doc, styleTriggersNode, properties, type, keysForProperty, false);
            GlobalLog.LogStatus("styledElement set.");

            // Insert styled element where style composed of multiple property trigger. 
            XmlElement styledElementMultiTrigger = TriggerActionHelper.InsertMainElement(doc, containerNode, "RootForStyledElementMultiTrigger", "StyledElementMultiTrigger", type);
            styleTriggersNode = ActionHelper.InsertStyleTriggersNode(doc, styledElementMultiTrigger);
            TriggerActionHelper.InsertMultiplePropertyTriggers(doc, styleTriggersNode, properties, type, keysForProperty, false);
            GlobalLog.LogStatus("styledElement with MultiTrigger set.");
            
            // Set Verifier property.
            doc.SetVerifierRoutine(this.GetType().GetMethod("VerifyStylePropertyTrigger"));

            // Save final xml to file.
            IOHelper.SaveTextToFile(IOHelper.ConvertXmlDocumentToStream(doc), ActionHelper.XamlFileName);
            GlobalLog.LogStatus("Xaml Generated : " + ActionHelper.XamlFileName + ".");
        }
        #endregion


        #region Verification
        /******************************************************************************
        * Function:          VerifyStylePropertyTrigger
        ******************************************************************************/
        /// <summary>
        /// Verifies preperty trigger after the first render.
        /// </summary>
        /// <param name="uielement"></param>
        public void VerifyStylePropertyTrigger(UIElement uielement)
        {
            FrameworkElement root = (FrameworkElement)uielement;
            Type type = root.FindName("StyledElement").GetType();
            Hashtable properties = GetPropertyTableForTriggerInStyle(type.FullName, s_xamlTestDocument);
            TriggerActionHelper.VerifyPropertyTrigger(root, "elementWithInitialValues", "StyledElement", "StyledElementMultiTrigger", properties);
        }
        #endregion
    }
}
