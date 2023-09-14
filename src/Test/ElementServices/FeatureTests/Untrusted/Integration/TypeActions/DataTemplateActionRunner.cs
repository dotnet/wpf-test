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
    * CLASS:          DataTemplateActionRunner
    ******************************************************************************/
    /// <summary>
    /// Implements routines for testing Avalon DataTemplate in Style, all types and properties.
    ///</summary>
    [Test(1, "Integration.DataTemplate", "DataTemplateActionRunner", SecurityLevel=TestCaseSecurityLevel.FullTrust, SupportFiles=@"FeatureTests\ElementServices\ActionForTypeExclude.xml,Common\PropertiesToSkip.xml,FeatureTests\ElementServices\PropertyTriggerActionForType_empty.xaml,FeatureTests\ElementServices\IntegrationXamlSnippets.xaml,FeatureTests\ElementServices\MouseActionIntegrationXamlSnippets.xaml,FeatureTests\ElementServices\ActionForType_PropertiesToSkip.xml,FeatureTests\ElementServices\DataTemplateActionForType_empty.xaml")]
    public class DataTemplateActionRunner : WindowTest
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
        [Variation("BulletDecorator")]
        [Variation("Button")]
        [Variation("Canvas")]
        [Variation("CheckBox")]
        [Variation("ComboBox")]
        [Variation("ComboBoxItem")]
        [Variation("ContentControl")]
        [Variation("ContentPresenter")]
        [Variation("Control")]
        [Variation("Decorator")]
        [Variation("DockPanel")]
        [Variation("DocumentPageView")]
        [Variation("DocumentReference")]
        [Variation("DocumentViewer")]
        [Variation("Ellipse")]
        [Variation("Expander")]
        [Variation("FixedPage")]
        [Variation("FlowDocumentPageViewer")]
        [Variation("FlowDocumentReader")]
        [Variation("FlowDocumentScrollViewer")]
        [Variation("Frame")]
        [Variation("FrameworkElement")]
        [Variation("Glyphs")]
        [Variation("Grid")]
        [Variation("GridSplitter")]
        [Variation("GridViewColumnHeader")]
        [Variation("GroupBox")]
        [Variation("GroupItem")]
        [Variation("HeaderedContentControl")]
        [Variation("HeaderedItemsControl")]
        [Variation("InkCanvas")]
        [Variation("InkPresenter")]
        [Variation("ItemsControl")]
        [Variation("ItemsPresenter")]
        [Variation("MediaElement")]
        [Variation("Image")]
        [Variation("Label")]
//        [Variation("Line")]
        [Variation("ListBox")]
        [Variation("ListBoxItem")]
        [Variation("ListView")]
        [Variation("ListViewItem")]
        // [DISABLE WHILE PORTING]
        // [Variation("Menu")]
        // [Variation("MenuItem")]
        [Variation("PageContent")]
        [Variation("PasswordBox")]
        [Variation("Path")]
//        [Variation("Polygon")]
//        [Variation("Polyline")]
        [Variation("Popup")]
        [Variation("ProgressBar")]
        [Variation("RadioButton")]
        [Variation("Rectangle")]
        [Variation("RepeatButton")]
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
        [Variation("TabPanel")]
        [Variation("TextBlock")]
        [Variation("TextBox")]
        [Variation("Thumb")]
        [Variation("TickBar")]
        [Variation("ToggleButton")]
        [Variation("ToolBar")]
        [Variation("ToolBarPanel")]
        [Variation("ToolBarTray")]
//        [Variation("ToolBarOverflowPanel")]
        [Variation("Track")]
        [Variation("TreeView")]
        [Variation("TreeViewItem")]
        [Variation("UniformGrid")]
//        [Variation("UserControl")]
        [Variation("Viewbox")]
        [Variation("Viewport3D")]
        [Variation("VirtualizingStackPanel")]
        [Variation("WebBrowser")]
        [Variation("WrapPanel")]

        /******************************************************************************
        * Function:          DataTemplateActionRunner Constructor
        ******************************************************************************/
        public DataTemplateActionRunner(string arg)
        {
            s_typeName = arg;

            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(DoDataTemplateTest);
        }
        public DataTemplateActionRunner() { }
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
            ActionHelper.XamlFileName = "__DataTemplateActionForType.xaml";
            ActionHelper.EmptyFileName= "DataTemplateActionForType_empty.xaml";
            s_xamlTestDocument = ActionHelper.TestDocument;

            // Set culture to 'en-us' so round-tripping will work correctly.
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfoByIetfLanguageTag("en-us");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag("en-us");

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          DoDataTemplateTest
        ******************************************************************************/
        /// <summary>
        /// Builds xaml with elements with DataTemplate in Template, loads it,
        /// and verifies that the instance's property values were triggered correctly.
        /// </summary>
        TestResult DoDataTemplateTest()
        {
            BuildDataTemplateXaml(s_typeName, s_xamlTestDocument);
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
            helper.DisplayTree(root, "DataTemplate", true);

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion
            
        #region Private Members
        /******************************************************************************
        * Function:          BuildDataTemplateXaml
        ******************************************************************************/
        // Builds a new xaml file for the type to use for loading and verification.
        // The xaml file contains 5 instance declarations: 
        //      - StyledElement - has a Style set via property element syntax, and DataTemplates are under the style.
        //      - StyledElementMultiTrigger - same as above except the triggers are MultiTrigger.
        //      - TemplatedElement - has a template set via property element syntax, and DataTemplates are under the template.
        //      - TemplatedElementMultiTrigger - same as above except the triggers are MultiTrigger.
        //      - elementWithInitialValues - has locally-set properties for comparison purpose.
        private void BuildDataTemplateXaml(string typeName, XamlTestDocument doc)
        {
            Type type = ActionHelper.GetType(typeName, true);

            XmlElement resourcesNode = doc.DocumentElement.FirstChild as XmlElement;
            XmlElement dateTemplateNode = resourcesNode.FirstChild as XmlElement;
            List<DependencyPropertyDescriptor> properties = ActionHelper.GetPropertyTableElementBinding(typeName);

            XmlElement containerNode = doc.GetContainerNode(type);
            
            XmlElement elementWithInitialValues = TriggerActionHelper.InsertMainElement(doc, containerNode, "RootForElementWithInitialValues", baseElementName, type);
            TriggerActionHelper.SetInitialValues(doc, elementWithInitialValues, properties);
            
            CompleteDataTemplate(doc, dateTemplateNode, type, containerNode, properties);

            // Set Verifier property.
            doc.SetVerifierRoutine(this.GetType().GetMethod("VerifyDataTemplate"));

            // Save final xml to file.
            IOHelper.SaveTextToFile(IOHelper.ConvertXmlDocumentToStream(doc), ActionHelper.XamlFileName);
            GlobalLog.LogStatus("Xaml Generated : " + ActionHelper.XamlFileName + ".");
        }

        /******************************************************************************
        * Function:          CompleteDataTemplate
        ******************************************************************************/
        /// <summary>
        /// Complate the DataTemplate.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="dataTemplate"></param>
        /// <param name="type"></param>
        /// <param name="container"></param>
        /// <param name="properties"></param>
        private void CompleteDataTemplate(XmlDocument doc, XmlElement dataTemplate, Type type, XmlElement container, List<DependencyPropertyDescriptor> properties)
        {
            //Add element
            XmlElement element = doc.CreateElement(type.Name, XamlGenerator.AvalonXmlns);
            element.SetAttribute("Name", "elementInDataTemplate");
            //containerNode.
              
            dataTemplate.AppendChild(element);

            //Setter value
            ActionHelper.InsertBindings(doc, element, properties, false, ActionHelper.baseElementName);
        }

        /******************************************************************************
        * Function:          ContentPresenter
        ******************************************************************************/
        /// <summary>
        /// Find ContentPresenter in visual tree.
        /// Adapted from FindDataVisuals in ConnectedData\Common\Util.cs
        /// </summary>
        private ContentPresenter GetHeaderContentPresenter(FrameworkElement element)
        {
            ContentPresenter cp = null;
            if ((element is ContentPresenter) && !(element is ScrollContentPresenter))
            {
                cp = element as ContentPresenter;
                if (string.Equals(cp.ContentSource, "Header"))
                    return cp;
            }

            int count = VisualTreeHelper.GetChildrenCount(element);

            for (int i = 0; i < count; i++)
            {
                // Common base class for Visual and Visual3D is DependencyObject

                DependencyObject child = VisualTreeHelper.GetChild(element, i);
                if (child is FrameworkElement)
                    cp = GetHeaderContentPresenter((FrameworkElement)child);

                if (cp != null) return cp;
            }
            return null;
        }
        #endregion


        #region Verification
        /******************************************************************************
        * Function:          VerifyDataTemplate
        ******************************************************************************/
        /// <summary>
        /// Verifies DataTemplate.
        /// </summary>
        /// <param name="uielement"></param>
        public void VerifyDataTemplate(UIElement uielement)
        {
            GlobalLog.LogStatus("Verify DataTemplates...");
            FrameworkElement root = (FrameworkElement)uielement;
            DispatcherHelper.DoEvents(500);

            DependencyObject elementWithInitialValues = root.FindName(baseElementName) as DependencyObject;
            Type type = elementWithInitialValues.GetType();

            List<DependencyPropertyDescriptor> properties = ActionHelper.GetPropertyTableElementBinding(type.FullName);

            GlobalLog.LogStatus("Verify ContentControl...", ConsoleColor.Blue);
            ContentControl contentControl = root.FindName("contentControl") as ContentControl;
            FrameworkElement cp = VisualTreeUtils.getFirstContentPresenter(contentControl);
            DependencyObject elementInDataTemplate = contentControl.ContentTemplate.FindName("elementInDataTemplate", cp) as DependencyObject;
            ActionHelper.CompareElements(properties, elementWithInitialValues, elementInDataTemplate, ActionHelper.PropertiesToSkip);

            GlobalLog.LogStatus("Verify ListBox...", ConsoleColor.Blue);
            ListBox listBox = root.FindName("listBox") as ListBox;
            ListBoxItem lbi1 = (ListBoxItem)(listBox.ItemContainerGenerator.ContainerFromIndex(0));

            cp = VisualTreeUtils.getFirstContentPresenter(lbi1 as FrameworkElement);
            elementInDataTemplate = listBox.ItemTemplate.FindName("elementInDataTemplate", cp) as DependencyObject;
            ActionHelper.CompareElements(properties, elementWithInitialValues, elementInDataTemplate, ActionHelper.PropertiesToSkip);

            GlobalLog.LogStatus("Verify HeaderedContentControl...", ConsoleColor.Blue);
            HeaderedContentControl headeredContentControl = root.FindName("headeredContentControl") as HeaderedContentControl;
            cp = GetHeaderContentPresenter(headeredContentControl);
            elementInDataTemplate = headeredContentControl.HeaderTemplate.FindName("elementInDataTemplate", cp) as DependencyObject;
            ActionHelper.CompareElements(properties, elementWithInitialValues, elementInDataTemplate, ActionHelper.PropertiesToSkip);

            GlobalLog.LogStatus("Verify TreeView...", ConsoleColor.Blue);
            TreeView treeView = root.FindName("treeView") as TreeView;
            TreeViewItem tvi = (TreeViewItem)(treeView.ItemContainerGenerator.ContainerFromIndex(0));
            TreeViewItem tvii = (TreeViewItem)(tvi.ItemContainerGenerator.ContainerFromIndex(0));
            FrameworkElement templateRoot = VisualTreeUtils.GetChild(cp, 0) as FrameworkElement;
            elementInDataTemplate = templateRoot.FindName("elementInDataTemplate") as DependencyObject;
            ActionHelper.CompareElements(properties, elementWithInitialValues, elementInDataTemplate, ActionHelper.PropertiesToSkip);

            GlobalLog.LogStatus("Verify TabControl...", ConsoleColor.Blue);
            TabControl tabControl = root.FindName("tabControl") as TabControl;
            cp = VisualTreeUtils.getFirstContentPresenter(tabControl);
            elementInDataTemplate = contentControl.ContentTemplate.FindName("elementInDataTemplate", cp) as DependencyObject;
            ActionHelper.CompareElements(properties, elementWithInitialValues, elementInDataTemplate, ActionHelper.PropertiesToSkip);

            GlobalLog.LogStatus("Verify ListView...", ConsoleColor.Blue);
            ListView listView = root.FindName("listView") as ListView;
            //cp = GetHeaderContentPresenter(listView);
            cp = VisualTreeUtils.getFirstContentPresenter(listView);
            elementInDataTemplate = contentControl.ContentTemplate.FindName("elementInDataTemplate", cp) as DependencyObject;
            ActionHelper.CompareElements(properties, elementWithInitialValues, elementInDataTemplate, ActionHelper.PropertiesToSkip);
        }
        #endregion
    }


    /******************************************************************************
    * CLASS:          Places
    ******************************************************************************/
    /// <summary>
    /// This is a class serves as data source, from ConnectedData team from the same name. 
    /// </summary>
    public class Places : ObservableCollection<Place>
    {
        /// <summary>
        /// 
        /// </summary>
        public Places()
        {
            Add(new Place("Seattle", "WA"));
            Add(new Place("Redmond", "WA"));
            Add(new Place("Bellevue", "WA"));
            Add(new Place("Kirkland", "WA"));
            Add(new Place("Portland", "OR"));
            Add(new Place("San Francisco", "CA"));
            Add(new Place("Los Angeles", "CA"));
            Add(new Place("San Diego", "CA"));
            Add(new Place("San Jose", "CA"));
            Add(new Place("Santa Ana", "CA"));
            Add(new Place("Bellingham", "WA"));
        }
    }

    /******************************************************************************
    * CLASS:          Place
    ******************************************************************************/
    /// <summary>
    /// 
    /// </summary>
    public class Place : INotifyPropertyChanged
    {
        private string _name;

        private string _state;
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChangedEvent("Name");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string State
        {
            get { return _state; }
            set
            {
                _state = value;
                RaisePropertyChangedEvent("State");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Place()
        {
            this._name = "";
            this._state = "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="state"></param>
        public Place(string name, string state)
        {
            this._name = name;
            this._state = state;
        }

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /******************************************************************************
        * Function:          RaisePropertyChangedEvent
        ******************************************************************************/
        private void RaisePropertyChangedEvent(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
