// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Markup;
using System.Threading;
using Microsoft.Test.DataServices;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary> Test Control Specific StringFormat and Priority Bindings 
    /// <description>
    ///   Scenario 5 - Tests various control specific StringFormat getters/setters: 
    ///        ContentStringFormat: ContentPresenter, ContentControl
    ///        HeaderStringFormat: HeaderedContentControl, HeaderedItemsControl, GridViewColumn, GroupStyle w/ Header
    ///        ContentStringFormat: TabControl w/ string content
    ///        SelectedContentStringFormat: TabControl w/ string selected content
    ///        SelectionBoxItemStringFormat: ComboBox
    ///        ItemStringFormat: on ItemsControl itself w/ the DisplayMemberPath set
    ///        ItemStringFormat: on ItemsControl's items directly w/o the DisplayMemberPath set  
    ///        ColumnHeaderStringFormat: GridView 
    ///        ContentStringFormat: HierarchicalDataTemplate            
    ///  Scenario 7 - Priority Binding: on PB; on subbindings; on both
    /// </description>
    /// <relatedTasks>

    /// </relatedTasks>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    // [DISABLED_WHILE_PORTING]
    // [Test(2, "Binding", "StringFormatControlSpecific")]
    public class StringFormatControlSpecific : XamlTest
    {
        #region private fields

        private ContentControl _mycc;
        private Button _btn;		                // contentpresenter
        private TextBlock _mytb1;

        private HeaderedContentControl _myhc;
        private HeaderedItemsControl _myhic;
        private ListView _mygvc;  		        // GridViewColumn 
        private ItemsControl _myItemsControl; 	// GroupStyle w/ Header
        private TextBlock _tbHeadered;

        private TabControl _mytc;
        private ComboBox _myComboBox;
        private TextBlock _myComboBoxTb;         // tb binding to the selecteditem.content of the combo
        private ComboBox _myComboBoxTemp;

        private ListBox _lb1;                    // ItemsControl w/o DisplayMemberPath
        private ListBox _lb2;                    // ItemsControl w/ DisplayMemberPath

        private TreeView _tv;

        private TextBlock _pbTB1;                // PriorityBinding
        private TextBlock _pbTB3;                // PriorityBinding
        private TextBlock _pbTB4;                // PriorityBinding

        #endregion

        public StringFormatControlSpecific()
            : base(@"StringFormatControlSpecific.xaml")
        {
            // Get all the controls to be tested against
            InitializeSteps += new TestStep(Init);
            //Scenario 5 - control specific StringFormats
            RunSteps += new TestStep(TestControlStringFormat);
            //Scenario 7 - PriorityBinding tests
            RunSteps += new TestStep(TestPriorityBindingCase);
            RunSteps += new TestStep(StringFormatContentStringXml);
        }

        #region Test Steps

        /// <summary>
        /// get all controls being tested
        /// </summary>
        /// <returns></returns>
        private TestResult Init()
        {
            Status("Init");
            WaitForPriority(DispatcherPriority.SystemIdle);

            _mycc = (ContentControl)Util.FindElement(RootElement, "mycc");
            _btn = (Button)Util.FindElement(this.RootElement, "btn");
            _mytb1 = (TextBlock)Util.FindElement(RootElement, "mytb1");

            _myhc = (HeaderedContentControl)Util.FindElement(RootElement, "myhc");
            _myhic = (HeaderedItemsControl)Util.FindElement(RootElement, "myhic");
            _mygvc = (ListView)Util.FindElement(RootElement, "mygvc");
            _myItemsControl = (ItemsControl)Util.FindElement(RootElement, "myItemsControl");
            _tbHeadered = (TextBlock)Util.FindElement(RootElement, "tbHeadered");

            _mytc = (TabControl)Util.FindElement(RootElement, "mytc");
            _myComboBox = (ComboBox)Util.FindElement(RootElement, "myComboBox");
            _myComboBoxTb = (TextBlock)Util.FindElement(RootElement, "MyComboBoxTB");
            _myComboBoxTemp = (ComboBox)Util.FindElement(RootElement, "myComboBoxTemp");

            _lb1 = (ListBox)Util.FindElement(RootElement, "lb1");
            _lb2 = (ListBox)Util.FindElement(RootElement, "lb2");
            _tv = (TreeView)Util.FindElement(RootElement, "tv");

            _pbTB1 = (TextBlock)Util.FindElement(RootElement, "pbTB1");
            _pbTB3 = (TextBlock)Util.FindElement(RootElement, "pbTB3");
            _pbTB4 = (TextBlock)Util.FindElement(RootElement, "pbTB4");

            if ((_mycc == null) || (_btn == null) || (_mytb1 == null))
            {
                LogComment("Unable to locate content controls.");
                return TestResult.Fail;
            }
            if ((_myhc == null) || (_myhic == null) || (_mygvc == null) || (_myItemsControl == null) || (_tbHeadered == null))
            {
                LogComment("Unable to locate Headered controls.");
                return TestResult.Fail;
            }
            if ((_mytc == null) || (_myComboBox == null) || (_myComboBoxTb == null) || (_myComboBoxTemp == null))
            {
                LogComment("Unable to locate TabControl or ComboBox controls.");
                return TestResult.Fail;
            }
            if ((_lb1 == null) || (_lb2 == null))
            {
                LogComment("Unable to locate ItemsControl controls.");
                return TestResult.Fail;
            }
            if (_tv == null)
            {
                LogComment("Unable to locate the TreeView control.");
                return TestResult.Fail;
            }
            if ((_pbTB1 == null) || (_pbTB3 == null) || (_pbTB4 == null))
            {
                LogComment("Unable to locate the PriorityBinding control.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        /// <summary>
        /// Scenario 5 - Test control specific StringFormat
        ///     Based on the dev code implementation, we should be able to just test one scenario
        ///     for each control based StringFormat, without other verifications.  
        /// The tests can be combined with other scenarios, separate them for clarity and managability
        /// </summary>
        /// <returns></returns>
        private TestResult TestControlStringFormat()
        {
            Status("TestControlStringFormat");
            FrameworkElement container;
            ContentPresenter cp;
            DataTemplate myDataTemplate;
            CultureInfo en_us = CultureInfo.CreateSpecificCulture("en-us");
            ListBoxItem lbi;
            TextBlock tb;

            Util.AssertEquals(_mycc.Content.ToString(), "ContentControl AND ContentPresenter Tests", "Error in mycc.Content.");

            cp = (ContentPresenter)Util.FindElement(_btn, "cp");
            Util.AssertEquals(cp.Content.ToString(), "Hello in Button", "Error in cp.Content.");

            Util.AssertEquals(_myhc.Header.ToString(), "HeaderedContentControl - My Header", "Error in myhc.Header.");

            Util.AssertEquals(_myhic.Header.ToString(), "HeaderedItemsControl", "Error in myhic.Header");

            // GridView & GridViewColumn: GridView.ColumnHeaderStringFormat, GridViewColumn.HeaderStringFormat                        
            GridViewColumnHeader gvch = (GridViewColumnHeader)Util.FindVisualByType(typeof(GridViewColumnHeader), _mygvc, false, 1);
            tb = (TextBlock)Util.FindVisualByType(typeof(TextBlock), gvch);
            Util.AssertEquals(tb.Text, "18934.188", "Error in GridViewColumn.HeaderStringFormat");
            gvch = (GridViewColumnHeader)Util.FindVisualByType(typeof(GridViewColumnHeader), _mygvc, true, 1);
            tb = (TextBlock)Util.FindVisualByType(typeof(TextBlock), gvch);
            Util.AssertEquals(tb.Text, "18934.188", "Error in GridViewColumn.HeaderStringFormat");

            // GroupStyle w/ header for ItemsControl myItemsControl  
            GroupItem gi = Util.FindVisualChild<GroupItem>(_myItemsControl);
            cp = Util.FindVisualChild<ContentPresenter>(gi);
            myDataTemplate = cp.ContentTemplate;
            tb = (TextBlock)myDataTemplate.FindName("mytbgroup", cp);
            Util.AssertEquals(tb.Text, "Home", "Error in myItemsControl.GroupStyle w/ Header");

            // TabControl - ContentStringFormat,ItemStringFormat
            container = (FrameworkElement)_mytc.ItemContainerGenerator.ContainerFromIndex(0);
            tb = (TextBlock)Util.FindVisualByType(typeof(TextBlock), container);
            Util.AssertEquals(tb.Text, "345", "Item formatting failed on TabControl");
            cp = Util.FindVisualChild<ContentPresenter>(_mytc);
            tb = (TextBlock)Util.FindVisualByType(typeof(TextBlock), cp);
            Util.AssertEquals(tb.Text, "345.23", "Selected content formatting failed on TabControl");

            // ComboBox - ComboBox.ItemStringFormat; ComboBoxItem.ContentStringFormat
            cp = Util.GetSelectionBox(_myComboBox);
            tb = (TextBlock)Util.FindVisualByType(typeof(TextBlock), cp);
            Util.AssertEquals(tb.Text, "Republic", "Error in ComboBox.ContentStringFormat");
            Util.AssertEquals(((Country)_myComboBox.Items[2]).Government, GovernmentType.Democracy, "Error in ComboBox.ContentStringFormat");

            cp = Util.GetSelectionBox(_myComboBoxTemp);
            tb = (TextBlock)Util.FindVisualByType(typeof(TextBlock), cp);
            Util.AssertEquals(tb.Text, "18934.19", "Error in myComboBoxTemp SelectionBox");

            // ItemsControl w/o DisplayMemberPath  
            lbi = Util.FindVisualChild<ListBoxItem>(_lb1);
            tb = Util.FindControlByTypeInTemplate<TextBlock>(lbi);
            Util.AssertEquals(tb.Text, "3.1", "Error in ItemsControl w/o DisplayMemberPath");
            // ItemsControl w/ DisplayMemberPath 
            lbi = Util.FindVisualChild<ListBoxItem>(_lb2);
            tb = Util.FindControlByTypeInTemplate<TextBlock>(lbi);
            Util.AssertEquals(tb.Text, "Hemisphere Name is: Western Hemisphere", "Error in ItemsControl w/ DisplayMemberPath");

            // HierarchicalDataTemplate.ItemStringFormat in HeaderedItemsControl
            TreeViewItem tvi = (TreeViewItem)_tv.ItemContainerGenerator.ContainerFromIndex(0);
            tvi.IsExpanded = true;
            container = (FrameworkElement)_tv.ItemContainerGenerator.ContainerFromIndex(0);
            tb = (TextBlock)Util.FindVisualByType(typeof(TextBlock), container);
            Util.AssertEquals(tb.Text, "Root", "Error in HierarchicalDataTemplate.ItemStringFormat");

            return TestResult.Pass;
        }

        /// <summary>
        /// Test StringFormat for PriorityBinding scenario
        /// </summary>
        /// <returns>pass if as expected</returns>
        private TestResult TestPriorityBindingCase()
        {
            // SF on PB, on Subbindings, on both. 
            Util.AssertEquals(_pbTB1.Text, "$18,934.19", "Error in PriorityBinding - SF on PB");
            Util.AssertEquals(_pbTB3.Text, "$18,934.188", "Error in PriorityBinding - SF on Subbindings");
            Util.AssertEquals(_pbTB4.Text, "18934", "Error in PriorityBinding - SF on both");

            return TestResult.Pass;
        }

        /// <summary>
        /// Test ContentStringFormat being applied when content is a string or XmlNode.
        /// Coverage for Data Formatting : ContentPresenter isn�t using its ContentStringFormat when its content is an XmlNode
        /// </summary>
        /// <returns></returns>
        private TestResult StringFormatContentStringXml()
        {
            Label l = (Label)RootElement.FindName("ContentStringFormatContentLabel");
            Label l2 = (Label)RootElement.FindName("ContentStringFormatXmlContentLabel");

            TextBlock tb = (TextBlock)Util.FindVisualByType(typeof(TextBlock), l);
            TextBlock tb2 = (TextBlock)Util.FindVisualByType(typeof(TextBlock), l2);

            if (tb.Text != "This is my string mystring") return TestResult.Fail;
            if (tb2.Text != "This is my string Terry Adams") return TestResult.Fail;

            return TestResult.Pass;
        }

        #endregion
    }
}