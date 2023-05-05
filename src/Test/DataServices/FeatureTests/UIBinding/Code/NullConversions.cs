// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
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
using System.Diagnostics;

namespace Microsoft.Test.DataServices
{
    /// <summary> Null Conversion Tests 
    /// <description> 
    ///     Source type: SqlTypes, Nullable types 
    ///     Target type: string, object, int, DateTime, non-convertible type 
    ///     Data transfer direction: source to target, target to source
    ///     Objects in testing: Binding, MultiBinding, PriorityBinding
    /// 
    ///     w/ or w/o TargetNullValue set  : Back Compat issue: must have TargetNullValue set
    ///     w/ or w/o value Converter
    ///     w/ or w/o StringFormat : SqlTypes excluded
    ///     
    ///     NOTE: the test for SqlTypes is at the minimum due to no IFormattable support at the 
    ///           SQL side and thus no StringFormat at our end. 
    /// </description>
    /// <relatedTasks>

    /// </relatedTasks>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(0, "Binding", "NullConversions", Keywords = "MicroSuite")]
    public class NullConversions : XamlTest
    {
        #region Fields

        private Page _page;
        private ArrayList _list;
        private TextBlock _tb;
        private Slider _sl;
        private TextBox _tbox;
        private CheckBox _cb;
        private ListBox _lb;
        private ListView _lv;
        private DataTable _table;
        private StackPanel _sp;
        private TextBlock _tbshadow;
        private ListBox _lbmb;
        private TextBlock _tbmb1;
        private TextBlock _tbmb2;
        private TextBlock _tbpb1;
        private TextBlock _tbpb2;
        private TextBlock _tbpb3;

        #endregion

        #region Constructor

        public NullConversions()
            : base(@"NullConversions.xaml")
        {
            InitializeSteps += new TestStep(Init);
            RunSteps += new TestStep(TestDefaults); // S2T, w/ TargetNullValue set
            RunSteps += new TestStep(TestTargetToSource); // w/ TargetNullValue set; + StringFormat; + value Converter;  
            RunSteps += new TestStep(TestMultiBinding); // w/ TargetNullValue set;
            RunSteps += new TestStep(TestPriorityBinding); // w/ TargetNullValue set;
            RunSteps += new TestStep(SourceTypeNull);
        }

        #endregion

        #region TestSteps

        /// <summary>
        /// Initialize all controls being tested
        /// </summary>
        /// <returns>fail if some control can't be initialized for some reason</returns>
        private TestResult Init()
        {
            Status("Init...");
            WaitForPriority(DispatcherPriority.SystemIdle);

            // the page
            _page = (Page)this.Window.Content;
            _sp = (StackPanel)Util.FindElement(RootElement, "sp");

            // S2T and T2S
            _tb = (TextBlock)Util.FindElement(RootElement, "tb");
            _sl = (Slider)Util.FindElement(RootElement, "sl");
            _tbox = (TextBox)Util.FindElement(RootElement, "tbox");
            _cb = (CheckBox)Util.FindElement(RootElement, "cb");
            _lb = (ListBox)Util.FindElement(RootElement, "lb");
            _lv = (ListView)Util.FindElement(RootElement, "lv");
            _tbshadow = (TextBlock)Util.FindElement(RootElement, "tbshadow");

            // MultiBinding
            _tbmb1 = (TextBlock)Util.FindElement(RootElement, "tbmb1");
            _tbmb2 = (TextBlock)Util.FindElement(RootElement, "tbmb2");
            _lbmb = (ListBox)Util.FindElement(RootElement, "lbmb");

            // PriorityBinding
            _tbpb1 = (TextBlock)Util.FindElement(RootElement, "tbpb1");
            _tbpb2 = (TextBlock)Util.FindElement(RootElement, "tbpb2");
            _tbpb3 = (TextBlock)Util.FindElement(RootElement, "tbpb3");

            // setup nullable types source and do binding
            _list = new ArrayList();
            _list.Add(new MyPropeties(null, null, null, null, null));
            _table = new DataTableSource(_list);
            _sp.DataContext = _table;
            WaitForPriority(DispatcherPriority.Render);

            LogComment("The step Init was successful!");
            return TestResult.Pass;
        }
        
        /// <summary>
        /// S2T all defaults
        /// </summary>
        /// <returns></returns>
        private TestResult TestDefaults()
        {
            Status("TestDefaults...");

            // Initial nullable types
            Util.AssertEquals(_tb.Text, "null", "Error in default - TextBlock tb");
            Util.AssertEquals(_tbox.Text, "0", "Error in default - TextBox tbox");
            Util.AssertEquals(_sl.Value.ToString(), "0", "Error in default - Slider sl");
            Util.AssertEquals(_cb.GetValue(CheckBox.IsCheckedProperty), null, "Error in default - CheckBox cb");
            Util.AssertEquals(GetListBoxText(_lb, 0), "", "Error in default - ListBox lb");
 
            // Initial SqlTypes
            Util.AssertEquals(GetColumnCellText(_lv, 1, 0), "FANCY", "Error in default - SqlInt16");
            Util.AssertEquals(GetColumnCellText(_lv, _lv.Items.Count - 1, 1), "FANCY", "Error in default - SqlBoolean");
            Util.AssertEquals(GetColumnCellText(_lv, _lv.Items.Count - 1, 2), "FANCY", "Error in default - SqlByte");
            Util.AssertEquals(GetColumnCellText(_lv, _lv.Items.Count - 1, 3), "FANCY", "Error in default - SqlDateTime");
            Util.AssertEquals(GetColumnCellText(_lv, _lv.Items.Count - 1, 4), "FANCY", "Error in default - SqlDecimal");
            Util.AssertEquals(GetColumnCellText(_lv, _lv.Items.Count - 1, 5), "FANCY", "Error in default - SqlDouble");
            Util.AssertEquals(GetColumnCellText(_lv, _lv.Items.Count - 1, 6), "FANCY", "Error in default - SqlGuid");
            Util.AssertEquals(GetColumnCellText(_lv, _lv.Items.Count - 1, 7), "FANCY", "Error in default - SqlInt32");
            Util.AssertEquals(GetColumnCellText(_lv, _lv.Items.Count - 1, 8), "FANCY", "Error in default - SqlMoney");
            Util.AssertEquals(GetColumnCellText(_lv, _lv.Items.Count - 1, 9), "FANCY", "Error in default - SqlSingle");
            Util.AssertEquals(GetColumnCellText(_lv, _lv.Items.Count - 1, 10), "FANCY", "Error in default - SqlString");

            LogComment("The step TestDefaults was successful!");
            return TestResult.Pass;
        }

        /// <summary>
        /// Tests for Target to Source updates
        /// </summary>
        /// <returns></returns>
        private TestResult TestTargetToSource()
        {
            Status("TestTargetToSource...");
            WaitForPriority(DispatcherPriority.SystemIdle);

            // --------------------
            // Nullable types T2S
            // --------------------
            // reset all values to not null and rebind all           
            _list = new ArrayList();
            _list.Add(new MyPropeties("All is well", 32, 32.32, new DateTime(2008, 2, 26), true));
            _table = new DataTableSource(_list);
            WaitForPriority(DispatcherPriority.SystemIdle);
            _sp.DataContext = _table;
            WaitForPriority(DispatcherPriority.Render);

            // verify the reset
            Util.AssertEquals(_tb.Text, "02/26/2008", "Error in T2S - TextBlock tb");
            Util.AssertEquals(_tbox.Text, "$32.32", "Error in T2S - TextBox tbox");
            Util.AssertEquals(_sl.Value.ToString(), "32", "Error in T2S - Slider sl");
            Util.AssertEquals(_cb.GetValue(CheckBox.IsCheckedProperty), true, "Error in T2S - CheckBox cb");

            // T2S update
            _tbox.Text = "0";
            BindingExpression be = _tbox.GetBindingExpression(TextBox.TextProperty);
            be.UpdateSource();
            WaitForPriority(DispatcherPriority.SystemIdle);
            while (be.Status == BindingStatus.Unattached)
            { WaitFor(200); }
            // verify
            Util.AssertEquals(_tbshadow.Text, "0", "Error in T2S done - TextBox tbox");
            be = null;

            // ------------------------------------------------------------
            // SqlTypes' T2S - not test, see not at the top of the class.
            // ------------------------------------------------------------
            
            LogComment("The step TestTargetToSource was successful!");
            return TestResult.Pass;
        }

        /// <summary>
        /// MultiBinding test w/ Converter and TargetNullValue at different level
        /// </summary>
        /// <returns></returns>
        private TestResult TestMultiBinding()
        {
            Status("TestMultiBinding...");
            WaitForPriority(DispatcherPriority.SystemIdle);

            // initial 
            Util.AssertEquals(_tbmb1.Text, "Beatriz portuguese", "Errin in Initial in MB - TextBlock tbmb1");
            Util.AssertEquals(_tbmb2.Text, "portuguese, Beatriz", "Errin in Initial in MB - TextBlock tbmb2");
            Util.AssertEquals(GetListBoxText(_lbmb,0), "Beatriz portuguese", "Errin in Initial in MB - ListBox lbmb");

            // add null source
            People src = (People)(_page.Resources["peoplelist"]);
            src.Add(new Person(null, "Chinese"));
            BindingExpression be = _lbmb.GetBindingExpression(ListBox.ItemsSourceProperty);
            be.UpdateSource();
            WaitForPriority(DispatcherPriority.SystemIdle);

            // re-sync
            _lbmb.SelectedIndex = 10;
            WaitForPriority(DispatcherPriority.SystemIdle);
            // verify
            Util.AssertEquals(_tbmb1.Text, "awesome Chinese", "Errin in re-sync in MB - TextBlock tbmb1");
            Util.AssertEquals(_tbmb2.Text, "Chinese, awesome", "Errin in re-sync in MB - TextBlock tbmb2");
            be = null;

            // change target
            _lbmb.SelectedIndex = 1;
            WaitForPriority(DispatcherPriority.SystemIdle);
            _tbmb1.Text = "awesome romanian";
            MultiBindingExpression mbe = BindingOperations.GetMultiBindingExpression(_tbmb1, TextBlock.TextProperty);
            mbe.UpdateSource();
            WaitForPriority(DispatcherPriority.SystemIdle);
            //verify
            Util.AssertEquals(_tbmb2.Text, "romanian, awesome", "Error in change target in MB - tbmb2.Text");

            LogComment("The step TestMultiBinding was successful!");
            return TestResult.Pass;
        }

        /// <summary>
        /// Priority Binding Tests w/ TargetNullValue set at different level
        /// </summary>
        /// <returns></returns>
        private TestResult TestPriorityBinding()
        {
            Status("TestPriorityBinding...");
            WaitForPriority(DispatcherPriority.SystemIdle);

            // initial
            Util.AssertEquals(_tbpb1.Text, "$18,934.19", "Error in PriorityBinding - on PB");
            Util.AssertEquals(_tbpb2.Text, "$18,934.188", "Error in PriorityBinding - on Subbindings");
            Util.AssertEquals(_tbpb3.Text, "18934", "Error in PriorityBinding - on both");

            // new S
            StackPanel sppb = (StackPanel)Util.FindElement(RootElement, "sppb");
            double? mydouble = ((double?)Math.PI);
            sppb.DataContext = mydouble;
            WaitForPriority(DispatcherPriority.SystemIdle);
            // verify
            Util.AssertEquals(_tbpb1.Text, "$3.14", "Error in PB nullable double - on PB");
            Util.AssertEquals(_tbpb2.Text, "$3.142", "Error in PB nullable double - on Subbindings");
            Util.AssertEquals(_tbpb3.Text, "3", "Error in PB nullable double - on both");

            // null S
            mydouble = null;
            sppb.DataContext = mydouble;
            WaitForPriority(DispatcherPriority.SystemIdle);
            // verify 
            Util.AssertEquals(_tbpb1.Text, "4.0d", "Error in PB double null - on PB");
            Util.AssertEquals(_tbpb2.Text, "4.0d", "Error in PB double null- on Subbindings");
            Util.AssertEquals(_tbpb3.Text, "4.0d", "Error in PB double null - on both");

            LogComment("The step TestPriorityBinding was successful!");
            return TestResult.Pass;       
        }

        // 
        private TestResult SourceTypeNull()
        {
            

            Places places = new Places();
            ICollectionView icv = CollectionViewSource.GetDefaultView(places);

            Binding b = new Binding("/Name");
            b.Mode = BindingMode.TwoWay;
            b.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;

            Label l = new Label();
            l.DataContext = places;
            l.SetBinding(Label.ContentProperty, b);

            icv.MoveCurrentToPrevious();

            l.Content = "Foo";
            l.Content = null;

            BindingExpression be = l.GetBindingExpression(Label.ContentProperty);

            // UpdateSource depends upon CanUpdate and was null-ref'ing. When we can't update
            // we shouldn't be trying.
            be.UpdateSource();

            // The null value code wasn't resilient to a null type. This gets at that code.
            // (UpdateSource was getting at it, but since CanUpdate has been fixed we won't get
            // to this part of code anymore)
            icv.MoveCurrentToNext();

            return TestResult.Pass;
        }

        #endregion
        
        #region Helpers

        /// <summary>
        /// ListView - get (row, GridViewColumn) cell text
		/// </summary>




        private string GetColumnCellText(ListView lv, int row, int column)
        {
            if ((lv == null) || (row < 0) || (column < 0))
            {
                throw new TestValidationException("Input is invalid in GetColumnCellText");
            }
            TextBlock tb = GetTextBlockInGridViewCell(lv, row, column);

            return tb.Text;
        }
        private TextBlock GetTextBlockInGridViewCell(ListView lv, int row, int column)
        {
            if ((lv == null) || (row < 0) || (column < 0))
            {
                throw new TestValidationException("Input is invalid in GetTextBlockInGridViewCell");
            }
            ListViewItem lvi = (ListViewItem)Util.FindVisualByType(typeof(ListViewItem), lv, true, row);
            GridViewRowPresenter rp = (GridViewRowPresenter)Util.FindVisualByType(typeof(GridViewRowPresenter), lvi, true, 0);
            TextBlock tb = (TextBlock)Util.FindVisualByType(typeof(TextBlock), rp, true, column);

            return tb;
        }

        /// <summary>
        /// Get the currently displayed text in a list box
        /// </summary>
        /// <param name="listbox">the listbox to find the text against</param>
        /// <returns>the displayed text</returns>
        private string GetListBoxText(ListBox listbox, int index)
        {
            if ((listbox == null) || (index < 0))
            {
                throw new TestValidationException("Input is invalid in GetListBoxText");
            }
            ListBoxItem lbi = (ListBoxItem)(listbox.ItemContainerGenerator.ContainerFromIndex(index));
            TextBlock tb = Util.FindControlByTypeInTemplate<TextBlock>(lbi);

            return tb.Text;
        }
        
        #endregion
    }
}
