// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Text;
using System.Globalization;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Threading;
using System.Reflection;
using System.IO;
using System.Windows.Markup;

namespace DRT
{
    public class DrtListViewSuite : DrtTestSuite
    {
        public DrtListViewSuite()
            : base("ListView")
        {
            Contact = "Microsoft";
        }

        ListView _listView;
        Canvas _canvas;
        static bool s_isPageLoaded = false;
        const int _itemNumberPerPage = 5;
        const int itemNumber = 20;
        private WarningLevel _previousWarningLevel = WarningLevel.Ignore;

        public override DrtTest[] PrepareTests()
        {
            if (!s_isPageLoaded)
            {
                string fullname = DRT.BaseDirectory + "DrtListView.xaml";
                System.IO.Stream stream = File.OpenRead(fullname);
                Visual root = (Visual)XamlReader.Load(stream);

                InitTree(root);

                _listView.ItemsSource = CreateDataSource();

                DRT.Show(root);

                s_isPageLoaded = true;
            }
            else
            {
                Keyboard.Focus(null);
            }

            if (!DRT.KeepAlive)
            {
                List<DrtTest> tests = new List<DrtTest>();

                tests.Add(new DrtTest(Start));

                tests.Add(new DrtTest(BasicTest));

                tests.Add(new DrtTest(RestyleTest));

                tests.Add(new DrtTest(ColumnOperationTest));

                tests.Add(new DrtTest(KeyboardInputTest));

                tests.Add(new DrtTest(MouseInputTest));

                tests.Add(new DrtTest(DMB_CT_Test));

                tests.Add(new DrtTest(HT_HTS_Test));

                tests.Add(new DrtTest(AccessibilityTest));

                tests.Add(new DrtTest(Cleanup));

                return tests.ToArray();
            }
            else
            {
                return new DrtTest[] { };
            }
        }

        private void InitTree(DependencyObject root)
        {
            _canvas = DRT.FindElementByID("canvas", root) as Canvas;
            _listView = DRT.FindElementByID("listView", root) as ListView;

            DRT.Assert(_canvas != null, "Can't find the element whose ID is 'canvas' in DrtListView.xaml");
            DRT.Assert(_listView != null, "Can't find the element whose ID is 'listView' in DrtListView.xaml");

            ((FrameworkElement)root).DataContext = this;
        }

        DispatcherTimer _suicideTimer = null;

        private void Start()
        {
            if (!DRT.KeepAlive)
            {
                _suicideTimer = new DispatcherTimer();
                _suicideTimer.Interval = new TimeSpan(0, 5, 0);
                _suicideTimer.Tick += new EventHandler(OnTimeout);
                _suicideTimer.Start();
            }
        }

        public void Cleanup()
        {
            if (_suicideTimer != null)
            {
                _suicideTimer.Stop();
            }

            if (_previousWarningLevel != WarningLevel.Ignore)
            {
                ((DrtControlsBase)DRT).WindowDeactivatedWarningLevel = _previousWarningLevel;
                _previousWarningLevel = WarningLevel.Ignore;
            }
        }

        private void OnTimeout(object sender, EventArgs e)
        {
            throw new TimeoutException();
        }

        private class TimeoutException : Exception
        {
            public TimeoutException() : base("Timeout expired, quitting") { }
        }

        private IEnumerable CreateDataSource()
        {
            DateTime[] dataSource = new DateTime[itemNumber];

            DateTime dt = new DateTime(1900, 1, 2);
            for (int i = 0; i < itemNumber; i++)
            {
                dataSource[i] = dt;
                dt = dt.AddDays(1);
            }

            return dataSource;
        }

        #region Resources

        // The simplest details view
        private GridView CreateGridView1()
        {
            GridView gridView1 = new GridView();

            GridViewColumn cl1 = new GridViewColumn();
            cl1.DisplayMemberBinding = new Binding("Day");
            cl1.Header = "Day";
            cl1.Width = 80.0;
            gridView1.Columns.Add(cl1);

            GridViewColumn cl2 = new GridViewColumn();
            cl2.DisplayMemberBinding = new Binding("Month");
            cl2.Header = "Month";
            cl2.Width = 80.0;
            gridView1.Columns.Add(cl2);

            GridViewColumn cl3 = new GridViewColumn();
            cl3.DisplayMemberBinding = new Binding("Year");
            cl3.Header = "Year";
            cl3.Width = 80.0;
            gridView1.Columns.Add(cl3);

            return gridView1;
        }

        // A details view uses:
        // DisplayMemberBinding     CellTemplate            CellTemplateSelector
        // Header                   ColumnHeaderTemplate    ColumnHeaderTemplateSelector
        private GridView CreateGridView2()
        {
            GridView _gridView2 = new GridView();

            GridViewColumn cl1 = new GridViewColumn();
            cl1.DisplayMemberBinding = new Binding("Day");
            cl1.Header = "Day";
            cl1.Width = 80.0;
            _gridView2.Columns.Add(cl1);

            GridViewColumn cl2 = new GridViewColumn();
            cl2.CellTemplate = _canvas.Resources["CellTemplate_Month_Blue"] as DataTemplate;
            cl2.HeaderTemplate = _canvas.Resources["ColumnHeaderTemplate_Month_Blue"] as DataTemplate;
            cl2.Width = 80.0;
            _gridView2.Columns.Add(cl2);

            GridViewColumn cl3 = new GridViewColumn();
            cl3.CellTemplateSelector = new CellTemplateSelector_Year_Red();
            cl3.Header = "Year";
            cl3.HeaderTemplateSelector = new ColumnHeaderTemplateSelector_Red();
            cl3.Width = 80.0;
            _gridView2.Columns.Add(cl3);

            return _gridView2;
        }

        private GridView CreateGridView3()
        {
            /*
            <GridView ColumnHeaderTemplate="{StaticResource ColumnHeaderTemplate_Day_Pink}"
                      ColumnHeaderTemplateSelector="{new ColumnHeaderTemplateSelector_Brown()}" >
                <GridViewColumn HeaderTemplate="{StaticResource ColumnHeaderTemplate_Month_Blue}"
                                HeaderTemplateSelector="{new ColumnHeaderTemplateSelector_Green()}"
                                DisplayMemberBinding="{Binding Year}"
                                Width="120">
                    <GridViewColumnHeader ContentTemplate="{StaticResource ColumnHeaderTemplate_Year_Yellow}"
                                          ContentTemplateSelector="{new ColumnHeaderTemplateSelector_Red()}" >
                        User Header0
                    </GridViewColumnHeader>
                </GridViewColumn>
            </GridView>
            */

            GridView _gridView3 = new GridView();
            _gridView3.ColumnHeaderTemplate = _canvas.Resources["ColumnHeaderTemplate_Day_Pink"] as DataTemplate;
            _gridView3.ColumnHeaderTemplateSelector = new ColumnHeaderTemplateSelector_Brown();

            GridViewColumn cl1 = new GridViewColumn();
            cl1.HeaderTemplate = _canvas.Resources["ColumnHeaderTemplate_Month_Blue"] as DataTemplate;
            cl1.HeaderTemplateSelector = new ColumnHeaderTemplateSelector_Green();

            GridViewColumnHeader header = new GridViewColumnHeader();
            header.ContentTemplate = _canvas.Resources["ColumnHeaderTemplate_Year_Yellow"] as DataTemplate;
            header.ContentTemplateSelector = new ColumnHeaderTemplateSelector_Red();
            header.Content = "User Header0";

            cl1.DisplayMemberBinding = new Binding("Year");
            cl1.Header = header;
            cl1.Width = 120.0;
            _gridView3.Columns.Add(cl1);

            return _gridView3;
        }

        #region CellTemplateSelector

        // DataTemplateSelector for cell
        private class CellTemplateSelector_Year_Red : CellTemplateSelectorBase
        {
            public CellTemplateSelector_Year_Red()
            {
                _brush = Brushes.Red;
                _bindingPath = "Year";
                _content = "Provided by CellTemplateSelector_Year_Red";
            }
        }

        // DataTemplateSelector for cell
        private class CellTemplateSelector_Month_Green : CellTemplateSelectorBase
        {
            public CellTemplateSelector_Month_Green()
            {
                _brush = Brushes.Green;
                _bindingPath = "Month";
                _content = "Provided by CellTemplateSelector_Month_Green ";
            }
        }

        // DataTemplateSelector for cell
        private class CellTemplateSelectorBase : DataTemplateSelector
        {
            protected Brush _brush = Brushes.Black;
            protected string _bindingPath = "";
            protected string _content = "Provided by CelllTemplateSelectorBase";

            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                DataTemplate template = new DataTemplate();

                if (item is DateTime)
                {
                    /*
                    <Border BorderBrush="{_brush}" BroderThickness="1">
                        <TextBlock Text="{Binding _bindingPath}" />
                    </Border>
                    */
                    FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border));
                    border.SetValue(Border.BorderBrushProperty, _brush);
                    border.SetValue(Border.BorderThicknessProperty, new Thickness(1.0));

                    FrameworkElementFactory textBlock = new FrameworkElementFactory(typeof(TextBlock));
                    Binding bind = new Binding(_bindingPath);
                    textBlock.SetBinding(TextBlock.TextProperty, bind);

                    border.AppendChild(textBlock);

                    template.VisualTree = border;
                    template.Seal();
                }
                else
                {
                    /*
                    <TextBlock Text="{_content}"/>
                     */
                    FrameworkElementFactory textBlock = new FrameworkElementFactory(typeof(TextBlock));
                    textBlock.SetValue(TextBlock.TextProperty, _content);

                    template.VisualTree = textBlock;
                    template.Seal();
                }

                return template;
            }
        }

        #endregion

        #region ColumnHeaderTemplateSelector

        // DataTemplateSelector for column header
        private class ColumnHeaderTemplateSelector_Red : ColumnHeaderTemplateSelectorBase
        {
            public ColumnHeaderTemplateSelector_Red()
            {
                _brush = Brushes.Red;
                _content = "Provided by ColumnHeaderTemplateSelector_Red";
            }
        }

        // DataTemplateSelector for column header
        private class ColumnHeaderTemplateSelector_Green : ColumnHeaderTemplateSelectorBase
        {
            public ColumnHeaderTemplateSelector_Green()
            {
                _brush = Brushes.Green;
                _content = "Provided by ColumnHeaderTemplateSelector_Green";
            }
        }

        // DataTemplateSelector for column header
        private class ColumnHeaderTemplateSelector_Brown : ColumnHeaderTemplateSelectorBase
        {
            public ColumnHeaderTemplateSelector_Brown()
            {
                _brush = Brushes.Brown;
                _content = "Provided by ColumnHeaderTemplateSelector_Brown";
            }
        }

        // DataTemplateSelector for column header
        private class ColumnHeaderTemplateSelectorBase : DataTemplateSelector
        {
            protected Brush _brush = Brushes.Black;
            protected string _content = "Provided by ColumnHeaderTemplateSelectorBase";

            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                DataTemplate template = new DataTemplate();

                if (item is string)
                {
                    /*
                    <Border BorderBrush="{_brush}" BroderThickness="1">
                        <TextBlock Text="{Binding}" />
                    </Border>
                    */
                    FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border));
                    border.SetValue(Border.BorderBrushProperty, _brush);
                    border.SetValue(Border.BorderThicknessProperty, new Thickness(1.0));

                    FrameworkElementFactory textBlock = new FrameworkElementFactory(typeof(TextBlock));
                    textBlock.SetValue(TextBlock.TextProperty, (string)item);

                    border.AppendChild(textBlock);

                    template.VisualTree = border;
                    template.Seal();
                }
                else
                {
                    /*
                    <TextBlock Text="{_content}"/>
                     */
                    FrameworkElementFactory textBlock = new FrameworkElementFactory(typeof(TextBlock));
                    textBlock.SetValue(TextBlock.TextProperty, _content);

                    template.VisualTree = textBlock;
                    template.Seal();
                }

                return template;
            }
        }

        #endregion

        #endregion Resources

        private void ColumnHeaderClickHandler(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is GridViewColumnHeader)
            {
                _isColumnHeaderClicked = true;
            }
        }

        bool _isColumnHeaderClicked;

        private bool HaveColorBorder(Visual visual, Brush brush)
        {
            ArrayList borders = ListViewVisualTreeHelper.FindPartByType(visual, typeof(Border));
            foreach (Border border in borders)
            {
                if (border.BorderBrush == brush)
                {
                    return true;
                }
            }

            return false;
        }

        public void VerifyHeaderWidth(double actualWidth, double expectedWidth)
        {
            double tol = 1e-5;

            double diff = actualWidth - expectedWidth;
            DRT.Assert(Math.Abs(diff) < tol, "Header's actualWidth width ({0}) does not match the expected width ({1})", actualWidth, expectedWidth);
        }

        /// <summary>
        /// Check the appearance of column header
        /// </summary>
        /// <param name="headerIndex">index of header</param>
        /// <param name="text">text should be shown in header</param>
        /// <param name="borderColor">header should have border of this color</param>
        private void CheckColumnHeaderStyle(int headerIndex, string text, Brush borderColor)
        {
            GridView view = _listView.View as GridView;
            DRT.Assert(view != null, "listView.View should be GridView");

            GridViewColumnHeader header = ListViewVisualTreeHelper.FindColumnHeaderByColumn(_listView, view.Columns[headerIndex]);
            DRT.Assert(header != null, "Should have GridViewColumnHeader in column " + headerIndex.ToString());

            TextBlock testBlock = ListViewVisualTreeHelper.FindPartByType(header, typeof(TextBlock), 0) as TextBlock;
            DRT.Assert(testBlock != null, "Should have TextBlock in header " + headerIndex.ToString());
            if (text != null)
            {
                DRT.Assert(testBlock.Text == text, "Text in column header " + headerIndex.ToString() + " should be \"" + text + "\"");
            }

            if (borderColor != null)
            {
                DRT.Assert(HaveColorBorder(header, borderColor), "Should have color border in header " + headerIndex.ToString());
            }
        }

        /// <summary>
        /// Check the appearance of item 0 cell [cellIndex]
        /// </summary>
        /// <param name="cellIndex">index of cell</param>
        /// <param name="text">text should be shown in cell</param>
        /// <param name="cellType">typeof(ContentPresenter) or typeof(TextBlock), cell should be this type</param>
        /// <param name="borderColor">cell should have border of this color</param>
        private void CheckCellStyle(int cellIndex, string text, Type cellType, Brush borderColor)
        {
            GridView view = _listView.View as GridView;
            DRT.Assert(view != null, "listView.View should be GridView");

            ListViewItem item = _listView.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
            DRT.Assert(item != null, "Item 0 should not be null");
            FrameworkElement cell = ListViewVisualTreeHelper.FindCellInItem(item, cellIndex);
            DRT.Assert(cell != null, "Should have cell " + cellIndex.ToString() + " in ListViewItem 0");
            DRT.Assert(cell.GetType() == cellType, "Cell should be " + cellType);

            TextBlock textBlock = null;
            if (cellType == typeof(TextBlock))
            {
                textBlock = cell as TextBlock;
            }
            else if (cellType == typeof(ContentPresenter))
            {
                textBlock = ListViewVisualTreeHelper.FindPartByType(cell, typeof(TextBlock), 0) as TextBlock;
            }
            DRT.Assert(textBlock != null, "Should have TextBlcok in cell " + cellIndex.ToString() + " of item 0");
            DRT.Assert(textBlock.Text == text, "Text in cell " + cellIndex.ToString() + " of item 0 should be \"" + text + "\"");

            if (borderColor != null)
            {
                DRT.Assert(HaveColorBorder(cell, borderColor), "Should have color border in cell " + cellIndex.ToString() + " of item 0");
            }
        }

        #region Basic

        private void BasicTest()
        {
            // Verify default values
            DRT.Assert(_listView.View == null, "Basic1-1 View should be null by default");

            ArrayList columnHeadess = ListViewVisualTreeHelper.FindPartByType(_listView, typeof(GridViewColumnHeader));
            DRT.Assert(columnHeadess.Count == 0, "Basic1-3 Shouldn't have column header in non details view mode");
        }

        #endregion

        #region Restyle Test

        RestyleStep _restylestep = RestyleStep.Start;

        enum RestyleStep
        {
            Start,

            // Switch between details view mode and non details view mode.
            Test1_SwitchToGridView1,
            Test1_Verify_1,
            Test1_SwitchToNonGridView,
            Test1_Verify_2,
            Test1_SwitchToGridView2,
            Test1_Verify_3,

            End,
        }

        private void RestyleTest()
        {
            if (DRT.Verbose) Console.WriteLine("RestyleTest: " + _restylestep);

            switch (_restylestep)
            {
                case RestyleStep.Start:
                    _listView.View = null;
                    break;

                case RestyleStep.Test1_SwitchToGridView1:
                    _listView.View = CreateGridView1();
                    break;

                case RestyleStep.Test1_Verify_1:
                    CheckColumnHeaderStyle(0, "Day", null);
                    CheckColumnHeaderStyle(1, "Month", null);
                    CheckColumnHeaderStyle(2, "Year", null);
                    CheckCellStyle(0, "2", typeof(TextBlock), null);
                    CheckCellStyle(1, "1", typeof(TextBlock), null);
                    CheckCellStyle(2, "1900", typeof(TextBlock), null);
                    break;

                case RestyleStep.Test1_SwitchToNonGridView:
                    _listView.View = null;
                    break;

                case RestyleStep.Test1_Verify_2:
                    ArrayList columnHeaders = ListViewVisualTreeHelper.FindPartByType(_listView, typeof(GridViewColumnHeader));
                    DRT.Assert(columnHeaders.Count == 0, "Restyle1-11 Shouldn't have column header in non details view mode");
                    break;

                case RestyleStep.Test1_SwitchToGridView2:
                    _listView.View = CreateGridView2();
                    break;

                case RestyleStep.Test1_Verify_3:
                    CheckColumnHeaderStyle(0, "Day", null);
                    CheckColumnHeaderStyle(1, "Month", Brushes.Blue);
                    CheckColumnHeaderStyle(2, "Year", Brushes.Red);
                    CheckCellStyle(0, "2", typeof(TextBlock), null);
                    CheckCellStyle(1, "1", typeof(ContentPresenter), Brushes.Blue);
                    CheckCellStyle(2, "1900", typeof(ContentPresenter), Brushes.Red);
                    break;

                case RestyleStep.End:
                    break;

                default:
                    break;
            }

            if (_restylestep++ <= RestyleStep.End)
            {
                DRT.RepeatTest();
            }
        }

        #endregion Restyle Test

        // Resize/Insert/Remove/Move/Clear column by code.
        #region Column Operation Test

        ColumnOperationStep _columnOperationStep = ColumnOperationStep.Start;

        GridViewColumn _newColumn = null;
        GridViewColumn _removedColumn = null;
        GridViewColumn _movedColumn = null;

        enum ColumnOperationStep
        {
            Start,

            Test1_ChangeColumnWidth,
            Test1_Verify_ColumnWidth,

            Test2_InsertColumn,
            Test2_Verify_InsertColumn,

            Test3_RemoveColumn,
            Test3_Verify_RemoveColumn,

            Test4_MoveColumn,
            Test4_Verify_MoveColumn,

            Test5_ClearColumn,
            Test5_Verify_ClearColumn,

            End,
        }

        private void ColumnOperationTest()
        {
            if (DRT.Verbose) Console.WriteLine("ColumnOperationTest: " + _columnOperationStep);
            GridViewColumnHeader header;
            GridView view;

            switch (_columnOperationStep)
            {
                case ColumnOperationStep.Start:
                    _listView.View = CreateGridView1();
                    break;

                case ColumnOperationStep.Test1_ChangeColumnWidth:
                    view = _listView.View as GridView;
                    DRT.Assert(view != null, "ColumnOperation1-1 _listView.View should be GridView");
                    view.Columns[0].Width = 100;
                    break;

                case ColumnOperationStep.Test1_Verify_ColumnWidth:
                    view = _listView.View as GridView;
                    DRT.Assert(view != null, "ColumnOperation1-2 _listView.View should be GridView");
                    DRT.Assert(view.Columns[0].Width == 100, "ColumnOperation1-3 Columns[0].Width should be 100");
                    header = ListViewVisualTreeHelper.FindColumnHeaderByColumn(_listView, view.Columns[0]);
                    DRT.Assert(header != null, "ColumnOperation1-4 Should have GridViewColumnHeader in deatisl view mode");
                    VerifyHeaderWidth(header.ActualWidth, 100);
                    break;

                case ColumnOperationStep.Test2_InsertColumn:
                    view = _listView.View as GridView;
                    DRT.Assert(view != null, "ColumnOperation2-1 _listView.View should be GridView");
                    _newColumn = new GridViewColumn();
                    _newColumn.Header = "DayOfWeek";
                    _newColumn.DisplayMemberBinding = new Binding("DayOfWeek");
                    _newColumn.Width = 80.0;
                    view.Columns.Insert(1, _newColumn);
                    break;

                case ColumnOperationStep.Test2_Verify_InsertColumn:
                    view = _listView.View as GridView;
                    DRT.Assert(view != null, "ColumnOperation2-2 _listView.View should be GridView");
                    DRT.Assert(view.Columns[1] == _newColumn, "ColumnOperation2-3 New column should be inserted into index 1");
                    header = ListViewVisualTreeHelper.FindColumnHeaderByColumn(_listView, _newColumn);
                    DRT.Assert(header != null, "ColumnOperation2-4 Should have GridViewColumnHeader for column inserted.");
                    break;

                case ColumnOperationStep.Test3_RemoveColumn:
                    view = _listView.View as GridView;
                    DRT.Assert(view != null, "ColumnOperation3-1 _listView.View should be GridView");
                    _removedColumn = view.Columns[2];
                    view.Columns.RemoveAt(2);
                    break;

                case ColumnOperationStep.Test3_Verify_RemoveColumn:
                    view = _listView.View as GridView;
                    DRT.Assert(view != null, "ColumnOperation3-2 _listView.View should be GridView");
                    DRT.Assert(view.Columns[2] != _removedColumn, "ColumnOperation2-3 Removed column shouldn't in column collection");
                    header = ListViewVisualTreeHelper.FindColumnHeaderByColumn(_listView, _removedColumn);
                    DRT.Assert(header == null, "ColumnOperation3-3 Shouldn't have GridViewColumnHeader for column removed.");
                    break;

                case ColumnOperationStep.Test4_MoveColumn:
                    view = _listView.View as GridView;
                    DRT.Assert(view != null, "ColumnOperation4-1 _listView.View should be GridView");
                    _movedColumn = view.Columns[0];
                    view.Columns.Move(0, 2);
                    break;

                case ColumnOperationStep.Test4_Verify_MoveColumn:
                    view = _listView.View as GridView;
                    DRT.Assert(view != null, "ColumnOperation4-2 _listView.View should be GridView");
                    DRT.Assert(view.Columns[2] == _movedColumn, "ColumnOperation4-3 Index of moved column should be 2");
                    header = ListViewVisualTreeHelper.FindColumnHeaderByColumn(_listView, _movedColumn);
                    DRT.Assert(header != null, "ColumnOperation4-4 Should have GridViewColumnHeader for column moved.");
                    break;

                case ColumnOperationStep.Test5_ClearColumn:
                    view = _listView.View as GridView;
                    DRT.Assert(view != null, "ColumnOperation5-1 _listView.View should be GridView");
                    view.Columns.Clear();
                    break;

                case ColumnOperationStep.Test5_Verify_ClearColumn:
                    GridViewHeaderRowPresenter columnHeadersPresenter = ListViewVisualTreeHelper.FindPartByType(_listView, typeof(GridViewHeaderRowPresenter), 0) as GridViewHeaderRowPresenter;
                    DRT.Assert(columnHeadersPresenter != null, "GridViewHeaderRowPresenter should still shown, though there is no column.");
                    ArrayList headerList = ListViewVisualTreeHelper.FindPartByType(_listView, typeof(GridViewColumnHeader));
                    DRT.Assert(headerList.Count == 2, "ColumnOperation5-3 Should only contain dummy & floating header when there is not column.");
                    break;

                case ColumnOperationStep.End:
                    break;

                default:
                    break;
            }

            if (_columnOperationStep++ <= ColumnOperationStep.End)
            {
                DRT.RepeatTest();
            }
        }

        #endregion Column Operation Test

        #region Keyboard Test

        KeyboardStep _keyboardstep = KeyboardStep.Start;

        enum KeyboardStep
        {
            Start,

            // Select and unselect by [Space] key.
            Test1_FocusOnItem,
            Test1_PressSpaceToSelectItem0,
            Test1_Verify_1,
            Test1_ToMultipleSelectionMode,
            Test1_PressDown,
            Test1_PressSpaceToSelectItem1,
            Test1_Verify_2,
            Test1_PressSpaceToUnselectItem1,
            Test1_Verify_3,
            Test1_PressUp,
            Test1_PressSpaceToUnselectItem0,
            Test1_Verify_4,

            // Keyboard Navigation
            Test2_Prepare,
            Test2_FocusOnTheFirstItem,
            Test2_ArrowDown,
            Test2_Verify_ArrowDown,
            Test2_ArrowUp,
            Test2_Verify_ArrowUp,
            Test2_End,
            Test2_Verify_End,
            Test2_Home,
            Test2_Verify_Home,

            End,
        }

        private void KeyboardInputTest()
        {
            if (DRT.Verbose) Console.WriteLine("KeyboardInputTest: " + _keyboardstep);
            ListViewItem item;
            int indexOfItem;
            switch (_keyboardstep)
            {
                case KeyboardStep.Start:
                    _listView.View = CreateGridView1();
                    _listView.SelectionMode = SelectionMode.Single;
                    break;

                // Use [Space] to select and unselect a focused item.
                #region single/multiple select/unselect

                case KeyboardStep.Test1_FocusOnItem:
                    item = _listView.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
                    DRT.Assert(item != null, "Item 0 should not be null");
                    item.Focus();
                    break;

                case KeyboardStep.Test1_PressSpaceToSelectItem0:
                    DRT.PressKey(Key.Space);
                    break;

                case KeyboardStep.Test1_Verify_1:
                    item = _listView.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
                    DRT.Assert(item != null, "Item 0 should not be null");
                    DRT.Assert(item.IsSelected == true, "KI1-1 Item 0 should be selected");
                    break;

                case KeyboardStep.Test1_ToMultipleSelectionMode:
                    _listView.SelectionMode = SelectionMode.Multiple;
                    break;

                case KeyboardStep.Test1_PressDown:
                    DRT.PressKey(Key.Down);
                    break;

                case KeyboardStep.Test1_PressSpaceToSelectItem1:
                    item = _listView.ItemContainerGenerator.ContainerFromIndex(1) as ListViewItem;
                    DRT.Assert(item != null, "Item 1 should not be null");
                    DRT.Assert(item.IsFocused == true, "Item 1 should be focused");
                    DRT.PressKey(Key.Space);
                    break;

                case KeyboardStep.Test1_Verify_2:
                    item = _listView.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
                    DRT.Assert(item != null, "Item 0 should not be null");
                    DRT.Assert(item.IsSelected == true, "KI1-2 Item 0 should be selected");
                    item = _listView.ItemContainerGenerator.ContainerFromIndex(1) as ListViewItem;
                    DRT.Assert(item != null, "Item 1 should not be null");
                    DRT.Assert(item.IsSelected == true, "KI1-3 Item 1 should be selected");
                    break;

                case KeyboardStep.Test1_PressSpaceToUnselectItem1:
                    DRT.PressKey(Key.Space);
                    break;

                case KeyboardStep.Test1_Verify_3:
                    item = _listView.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
                    DRT.Assert(item != null, "Item 0 should not be null");
                    DRT.Assert(item.IsSelected == true, "KI1-4 Item 0 should be selected");
                    item = _listView.ItemContainerGenerator.ContainerFromIndex(1) as ListViewItem;
                    DRT.Assert(item != null, "Item 1 should not be null");
                    DRT.Assert(item.IsSelected == false, "KI1-5 Item 1 should not be selected");
                    break;

                case KeyboardStep.Test1_PressUp:
                    DRT.PressKey(Key.Up);
                    break;

                case KeyboardStep.Test1_PressSpaceToUnselectItem0:
                    DRT.PressKey(Key.Space);
                    break;

                case KeyboardStep.Test1_Verify_4:
                    item = _listView.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
                    DRT.Assert(item != null, "Item 0 should not be null");
                    DRT.Assert(item.IsSelected == false, "KI1-6 Item 0 should not be selected");
                    item = _listView.ItemContainerGenerator.ContainerFromIndex(1) as ListViewItem;
                    DRT.Assert(item != null, "Item 1 should not be null");
                    DRT.Assert(item.IsSelected == false, "KI1-7 Item 1 should not be selected");
                    break;

                #endregion single/multiple select/unselect

                #region keyboard navigation

                case KeyboardStep.Test2_Prepare:
                    _listView.View = CreateGridView1();
                    _listView.SelectionMode = SelectionMode.Single;
                    break;

                case KeyboardStep.Test2_FocusOnTheFirstItem:
                    item = _listView.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
                    DRT.Assert(item != null, "KI2-1 Item 0 should not be null");
                    item.Focus();
                    break;

                case KeyboardStep.Test2_ArrowDown:
                    DRT.PressKey(Key.Down);
                    break;

                case KeyboardStep.Test2_Verify_ArrowDown:
                    indexOfItem = 1;
                    item = _listView.ItemContainerGenerator.ContainerFromIndex(indexOfItem) as ListViewItem;
                    DRT.Assert(item != null, "KI2-2 Item " + indexOfItem.ToString() + " should not be null");
                    DRT.Assert(item.IsSelected == true, "KI2-3 Item " + indexOfItem.ToString() + " should be selected");
                    break;

                case KeyboardStep.Test2_ArrowUp:
                    DRT.PressKey(Key.Up);
                    break;

                case KeyboardStep.Test2_Verify_ArrowUp:
                    indexOfItem = 0;
                    item = _listView.ItemContainerGenerator.ContainerFromIndex(indexOfItem) as ListViewItem;
                    DRT.Assert(item != null, "KI2-6 Item " + indexOfItem.ToString() + " should not be null");
                    DRT.Assert(item.IsSelected == true, "KI2-7 Item " + indexOfItem.ToString() + " should be selected");
                    break;

                case KeyboardStep.Test2_End:
                    DRT.PressKey(Key.End);
                    break;

                case KeyboardStep.Test2_Verify_End:
                    indexOfItem = itemNumber - 1;
                    item = _listView.ItemContainerGenerator.ContainerFromIndex(indexOfItem) as ListViewItem;
                    DRT.Assert(item != null, "KI2-10 Item " + indexOfItem.ToString() + " should not be null");
                    DRT.Assert(item.IsSelected == true, "KI2-11 Item " + indexOfItem.ToString() + " should be selected");
                    break;

                case KeyboardStep.Test2_Home:
                    DRT.PressKey(Key.Home);
                    break;

                case KeyboardStep.Test2_Verify_Home:
                    item = _listView.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
                    DRT.Assert(item != null, "KI2-12 Item 0 should not be null");
                    DRT.Assert(item.IsSelected == true, "KI2-13 Item 0 should be selected");
                    break;

                #endregion keyboard navigation

                case KeyboardStep.End:
                    break;

                default:
                    break;
            }

            if (_keyboardstep++ <= KeyboardStep.End)
            {
                DRT.RepeatTest();
            }
        }

        #endregion

        #region Mouse Test

        MouseStep _mousestep = MouseStep.Start;
        double _expectedWidth;
        GridViewColumn _MouseTestMovedColumn;

        enum MouseStep
        {
            Start,

            // Select and unselect by mouse click
            Test1_ClickFirstItemToSelect,
            Test1_Verify_1,
            Test1_ToMultipleSelectionMode,
            Test1_ClickSecondItemToSelect,
            Test1_Verify_2,
            Test1_ClickFirstItemToUnselect,
            Test1_Verify_3,
            Test1_ClickSecondItemToUnSelect,
            Test1_Verify_4,

            // Click column header to fire ColumnHeaderClickEvent event
            Test2_ClickColumnHeader,
            Test2_Verify,

            // Resize column
            Test3_Resize_MoveToGrip_1,
            Test3_Resize_PressMouse_1,
            Test3_Resize_DragToHalfWidth,
            Test3_Resize_ReleaseMouse_1,
            Test3_Resize_Verify_1,
            Test3_Resize_MoveToGrip_2,
            Test3_Resize_PressMouse_2,
            Test3_Resize_DragToZero,
            Test3_Resize_ReleaseMouse_2,
            Test3_Resize_Verify_2,
            Test3_Resize_MoveToGrip_3,
            Test3_Resize_PressMouse_3,
            Test3_Resize_DragToFullWidth,
            Test3_Resize_ReleaseMouse_3,
            Test3_Resize_Verify_3,

            // Move column
            Test4_Move_MoveToGrip,
            Test4_Move_PressMouse,
            Test4_Move_Drag,
            Test4_Move_ReleaseMouse,
            Test4_Move_Verify,

            End,
        }

        private void MouseInputTest()
        {
            if (DRT.Verbose) Console.WriteLine("MouseInputTest: " + _mousestep.ToString());
            ListViewItem item;
            GridView view;
            GridViewColumnHeader header;
            Thumb grip;

            switch (_mousestep)
            {
                case MouseStep.Start:
                    _listView.View = CreateGridView1();
                    _listView.SelectionMode = SelectionMode.Single;
                    break;

                // Select and unselect items by mouse click.
                #region Selection

                case MouseStep.Test1_ClickFirstItemToSelect:
                    item = _listView.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
                    DRT.Assert(item != null, "Item 0 should not be null");
                    DRT.MoveMouse(item, 0.5, 0.5);
                    DRT.ClickMouse();
                    break;

                case MouseStep.Test1_Verify_1:
                    item = _listView.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
                    DRT.Assert(item != null, "Item 0 should not be null");
                    DRT.Assert(item.IsSelected == true, "MI1-1 Item 0 should be selected");
                    break;

                case MouseStep.Test1_ToMultipleSelectionMode:
                    _listView.SelectionMode = SelectionMode.Multiple;
                    break;

                case MouseStep.Test1_ClickSecondItemToSelect:
                    item = _listView.ItemContainerGenerator.ContainerFromIndex(1) as ListViewItem;
                    DRT.Assert(item != null, "Item 1 should not be null");
                    DRT.MoveMouse(item, 0.5, 0.5);
                    DRT.ClickMouse();
                    break;

                case MouseStep.Test1_Verify_2:
                    item = _listView.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
                    DRT.Assert(item != null, "Item 0 should not be null");
                    DRT.Assert(item.IsSelected == true, "MI1-2 Item 0 should be selected");
                    item = _listView.ItemContainerGenerator.ContainerFromIndex(1) as ListViewItem;
                    DRT.Assert(item != null, "Item 1 should not be null");
                    DRT.Assert(item.IsSelected == true, "MI1-3 Item 1 should be selected");
                    break;

                case MouseStep.Test1_ClickFirstItemToUnselect:
                    item = _listView.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
                    DRT.Assert(item != null, "Item 0 should not be null");
                    DRT.MoveMouse(item, 0.5, 0.5);
                    DRT.ClickMouse();
                    break;

                case MouseStep.Test1_Verify_3:
                    item = _listView.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
                    DRT.Assert(item != null, "Item 0 should not be null");
                    DRT.Assert(item.IsSelected == false, "MI1-4 Item 0 should not be selected");
                    item = _listView.ItemContainerGenerator.ContainerFromIndex(1) as ListViewItem;
                    DRT.Assert(item != null, "Item 1 should not be null");
                    DRT.Assert(item.IsSelected == true, "MI1-5 Item 1 should be selected");
                    break;

                case MouseStep.Test1_ClickSecondItemToUnSelect:
                    item = _listView.ItemContainerGenerator.ContainerFromIndex(1) as ListViewItem;
                    DRT.Assert(item != null, "Item 1 should not be null");
                    DRT.MoveMouse(item, 0.5, 0.5);
                    DRT.ClickMouse();
                    break;

                case MouseStep.Test1_Verify_4:
                    item = _listView.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
                    DRT.Assert(item != null, "Item 0 should not be null");
                    DRT.Assert(item.IsSelected == false, "MI1-6 Item 0 should not be selected");
                    item = _listView.ItemContainerGenerator.ContainerFromIndex(1) as ListViewItem;
                    DRT.Assert(item != null, "Item 1 should not be null");
                    DRT.Assert(item.IsSelected == false, "MI1-7 Item 1 should not be selected");
                    break;

                #endregion Selection

                // Click column header to fire ColumnHeaderClickEvent evert.
                case MouseStep.Test2_ClickColumnHeader:
                    _listView.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeaderClickHandler));
                    view = _listView.View as GridView;
                    DRT.Assert(view != null, "MI1-8 _listView.View should be GridView");
                    header = ListViewVisualTreeHelper.FindColumnHeaderByColumn(_listView, view.Columns[0]);
                    DRT.Assert(header != null, "MI1-9 _listView should have column headers");
                    _isColumnHeaderClicked = false;
                    DRT.MoveMouse(header, 0.5, 0.5);
                    DRT.ClickMouse();
                    break;

                case MouseStep.Test2_Verify:
                    DRT.Assert(_isColumnHeaderClicked == true, "MI2-1 ColumnHeaderClickEvent should be fired");
                    break;

                // Resize
                #region Resize

                case MouseStep.Test3_Resize_MoveToGrip_1:
                    view = _listView.View as GridView;
                    DRT.Assert(view != null, "MI3-1 _listView.View should be GridView");
                    header = ListViewVisualTreeHelper.FindColumnHeaderByColumn(_listView, view.Columns[1]);
                    DRT.Assert(header != null, "MI3-2 Should have column header in column 1");
                    // Mouse button down on the right edge of GridViewColumnHeader. It's not the mid of the grip.
                    DRT.MoveMouse(header, 1.0, 0.5);
                    break;

                case MouseStep.Test3_Resize_PressMouse_1:
                    DRT.MouseButtonDown();
                    break;

                case MouseStep.Test3_Resize_DragToHalfWidth:
                    view = _listView.View as GridView;
                    DRT.Assert(view != null, "MI3-3 _listView.View should be GridView");
                    header = ListViewVisualTreeHelper.FindColumnHeaderByColumn(_listView, view.Columns[1]);
                    DRT.Assert(header != null, "MI3-4 Should have column header");
                    _expectedWidth = header.ActualWidth / 2;
                    DRT.MoveMouse(header, 0.5, 0.5);
                    break;

                case MouseStep.Test3_Resize_ReleaseMouse_1:
                    DRT.MouseButtonUp();
                    break;

                case MouseStep.Test3_Resize_Verify_1:
                    view = _listView.View as GridView;
                    DRT.Assert(view != null, "MI3-5 _listView.View should be GridView");
                    header = ListViewVisualTreeHelper.FindColumnHeaderByColumn(_listView, view.Columns[1]);
                    DRT.Assert(header != null, "MI3-6 Should have column header");
                    string strout = "_expectedWidth = " + _expectedWidth.ToString();
                    strout += " header.AcutalWidth = " + header.ActualWidth.ToString();
                    VerifyHeaderWidth(header.ActualWidth, _expectedWidth);
                    break;

                case MouseStep.Test3_Resize_MoveToGrip_2:
                    view = _listView.View as GridView;
                    DRT.Assert(view != null, "MI3-8 _listView.View should be GridView");
                    grip = ListViewVisualTreeHelper.FindGripByColumn(_listView, view.Columns[1]);
                    DRT.Assert(grip != null, "MI3-9 Should have grip on column header");
                    DRT.MoveMouse(grip, 0.5, 0.5);
                    break;

                case MouseStep.Test3_Resize_PressMouse_2:
                    DRT.MouseButtonDown();
                    break;

                case MouseStep.Test3_Resize_DragToZero:
                    view = _listView.View as GridView;
                    DRT.Assert(view != null, "MI3-10 _listView.View should be GridView");
                    header = ListViewVisualTreeHelper.FindColumnHeaderByColumn(_listView, view.Columns[0]);
                    DRT.Assert(header != null, "MI3-11 Should have column header");
                    _expectedWidth = header.ActualWidth;
                    DRT.MoveMouse(header, 0.5, 0.5);
                    break;

                case MouseStep.Test3_Resize_ReleaseMouse_2:
                    DRT.MouseButtonUp();
                    break;

                case MouseStep.Test3_Resize_Verify_2:
                    view = _listView.View as GridView;
                    DRT.Assert(view != null, "MI3-12 _listView.View should be GridView");
                    header = ListViewVisualTreeHelper.FindColumnHeaderByColumn(_listView, view.Columns[1]);
                    DRT.Assert(header != null, "MI3-13 Should have column header in column 1");
                    VerifyHeaderWidth(header.ActualWidth, 0);
                    header = ListViewVisualTreeHelper.FindColumnHeaderByColumn(_listView, view.Columns[0]);
                    DRT.Assert(header != null, "MI3-15 Should have column header in column 0");
                    VerifyHeaderWidth(header.ActualWidth, _expectedWidth);
                    break;

                case MouseStep.Test3_Resize_MoveToGrip_3:
                    view = _listView.View as GridView;
                    DRT.Assert(view != null, "MI3-17 _listView.View should be GridView");
                    grip = ListViewVisualTreeHelper.FindGripByColumn(_listView, view.Columns[1]);
                    DRT.Assert(grip != null, "MI3-18 Should have grip on column header");
                    DRT.MoveMouse(grip, 0.75, 0.5);
                    break;

                case MouseStep.Test3_Resize_PressMouse_3:
                    DRT.MouseButtonDown();
                    break;

                case MouseStep.Test3_Resize_DragToFullWidth:
                    view = _listView.View as GridView;
                    DRT.Assert(view != null, "MI3-19 _listView.View should be GridView");
                    header = ListViewVisualTreeHelper.FindColumnHeaderByColumn(_listView, view.Columns[2]);
                    DRT.Assert(header != null, "MI3-20 Should have column header in column 2");
                    _expectedWidth = header.ActualWidth;
                    grip = ListViewVisualTreeHelper.FindGripByColumn(_listView, view.Columns[2]);
                    DRT.Assert(grip != null, "MI3-21 Should have grip on column header");
                    DRT.MoveMouse(grip, 0.75, 0.5);
                    break;

                case MouseStep.Test3_Resize_ReleaseMouse_3:
                    DRT.MouseButtonUp();
                    break;

                case MouseStep.Test3_Resize_Verify_3:
                    view = _listView.View as GridView;
                    DRT.Assert(view != null, "MI3-22 _listView.View should be GridView");
                    header = ListViewVisualTreeHelper.FindColumnHeaderByColumn(_listView, view.Columns[1]);
                    DRT.Assert(header != null, "MI3-23 Should have column header in column 1");
                    VerifyHeaderWidth(header.ActualWidth, _expectedWidth);
                    break;

                #endregion Resize

                #region Move Column

                case MouseStep.Test4_Move_MoveToGrip:
                    view = _listView.View as GridView;
                    DRT.Assert(view != null, "MI4-1 _listView.View should be GridView");
                    _MouseTestMovedColumn = view.Columns[0];
                    header = ListViewVisualTreeHelper.FindColumnHeaderByColumn(_listView, view.Columns[0]);
                    DRT.Assert(header != null, "MI4-2 Should have column header in.column 0");
                    DRT.MoveMouse(header, 0.5, 0.5);
                    break;

                case MouseStep.Test4_Move_PressMouse:
                    DRT.MouseButtonDown();
                    break;

                case MouseStep.Test4_Move_Drag:
                    view = _listView.View as GridView;
                    DRT.Assert(view != null, "MI4-3 _listView.View should be GridView");
                    grip = ListViewVisualTreeHelper.FindGripByColumn(_listView, view.Columns[1]);
                    DRT.Assert(grip != null, "MI4-4 Should have grip on column header");
                    DRT.MoveMouse(grip, 0.5, 0.5);
                    break;

                case MouseStep.Test4_Move_ReleaseMouse:
                    DRT.MouseButtonUp();
                    break;

                case MouseStep.Test4_Move_Verify:
                    view = _listView.View as GridView;
                    DRT.Assert(view != null, "MI4-5 _listView.View should be GridView");
                    DRT.Assert(view.Columns[1] == _MouseTestMovedColumn, "MI4-6 column 0 should be moved to column 1");
                    break;

                #endregion Move Column

                case MouseStep.End:
                    break;

                default:
                    break;
            }

            if (_mousestep++ <= MouseStep.End)
            {
                DRT.RepeatTest();
            }
        }

        #endregion


        #region AccessibilityTest

        private void AccessibilityTest()
        {
            Process process = null;
            AutomationElement aeListView, aeBtnView0, aeBtnView1, aeBtnView2;
            AutomationElement aeBtnSelect1, aeBtnSelect2, aeBtnSelect3;
            AutomationElement aeBtnAddItem, aeBtnRemoveItem, aeBtnClearItem, aeBtnAddItems;
            AutomationElement aeBtnAddColumn, aeBtnRemoveColumn, aeBtnClearColumn;
            AutomationElement aeBtnAdd1000Items, aeBtnToggleGrouping1, aeBtnToggleGrouping2;

            _previousWarningLevel = ((DrtControlsBase)DRT).WindowDeactivatedWarningLevel;
            ((DrtControlsBase)DRT).WindowDeactivatedWarningLevel = WarningLevel.Ignore;

            try
            {
                // start a process
                process = new Process();
                process.StartInfo.FileName = @".\ListView2app.exe";
                process.StartInfo.Arguments = null;
                process.Start();

                // find the process main window handle
                const int MAXTIME = 30000; // maximum time to wait
                const int TIMEWAIT = 100; // polling time interval
                int runningTime = 0;
                while (process.MainWindowHandle.Equals(IntPtr.Zero))
                {
                    if (runningTime > MAXTIME)
                    {
                        return;
                    }

                    Thread.Sleep(TIMEWAIT);
                    runningTime += TIMEWAIT;

                    process.Refresh();
                }
                IntPtr handle = process.MainWindowHandle;

                // find the root window
                AutomationElement root = AutomationElement.FromHandle(handle);
                DRT.Assert(root != null, "ATT1-1 Could not convert root window handle to AutomationElement");

                aeListView = RobustFindElement(root, "listView", "ATT1-2");
                aeBtnView0 = RobustFindElement(root, "view0", "ATT1-3");
                aeBtnView1 = RobustFindElement(root, "view1", "ATT1-4");
                aeBtnView2 = RobustFindElement(root, "view2", "ATT1-5");
                aeBtnSelect1 = RobustFindElement(root, "select1", "ATT1-6");
                aeBtnSelect2 = RobustFindElement(root, "select2", "ATT1-7");
                aeBtnSelect3 = RobustFindElement(root, "select3", "ATT1-8");
                aeBtnAddItem = RobustFindElement(root, "additem", "ATT1-9");
                aeBtnRemoveItem = RobustFindElement(root, "removeitem", "ATT1-10");
                aeBtnClearItem = RobustFindElement(root, "clearitem", "ATT1-11");
                aeBtnAddItems = RobustFindElement(root, "additems", "ATT1-12");
                aeBtnAddColumn = RobustFindElement(root, "addcolumn", "ATT1-13");
                aeBtnRemoveColumn = RobustFindElement(root, "removecolumn", "ATT1-14");
                aeBtnClearColumn = RobustFindElement(root, "clearcolumn", "ATT1-15");
                aeBtnAdd1000Items = RobustFindElement(root, "add1000items", "ATT1-16");
                aeBtnToggleGrouping1 = RobustFindElement(root, "togglegrouping1", "ATT1-17");
                aeBtnToggleGrouping2 = RobustFindElement(root, "togglegrouping2", "ATT1-18");

                //1) Verify All patterns are supported in List and Grid view
                AutomationPattern[] arr = aeListView.GetSupportedPatterns();
                DRT.Assert(arr.Length == 4, "ATT2-1 ListView only supports 4 patterns by default");
                DRT.Assert(aeListView.GetCurrentPattern(ScrollPatternIdentifiers.Pattern) != null, "ATT2-2 ListView should support ScrollPattern by default");
                SelectionPattern sPattern = aeListView.GetCurrentPattern(SelectionPatternIdentifiers.Pattern) as SelectionPattern;
                DRT.Assert(sPattern != null, "ATT2-3 ListView should support SelectionPattern by default");
                DRT.Assert(aeListView.GetCurrentPattern(ItemContainerPatternIdentifiers.Pattern) != null, "ATT2-3.1 ListView should support ItemContainerPattern by default");
                DRT.Assert(aeListView.GetCurrentPattern(SynchronizedInputPatternIdentifiers.Pattern) != null, "ATT2-3.2 ListView should support SynchronizedInputPattern by default");
                DRT.Assert(sPattern.Current.CanSelectMultiple, "ATT2-4 CanSelectMultiple should be true");
                DRT.Assert(!sPattern.Current.IsSelectionRequired, "ATT2-5 IsSelectionRequired should be false");
                object[] objs = sPattern.Current.GetSelection();
                DRT.Assert(objs == null || objs.Length == 0, "ATT2-6 GetSelection should return nothing");

                //switch to gridview
                InvokeButton(aeBtnView1);

                RobustVerify(
                    delegate()
                    {
                        arr = aeListView.GetSupportedPatterns();
                        return (arr.Length == 6);
                    }, "ATT2-8 ListView only supports 6 patterns by default");

                GridPattern gPattern = null;

                RobustVerify(
                    delegate()
                    {
                        gPattern = aeListView.GetCurrentPattern(GridPatternIdentifiers.Pattern) as GridPattern;
                        return (gPattern != null);
                    }, "ATT2-9 GridView should support GridPattern");

                TablePattern tPattern = null;

                RobustVerify(
                    delegate()
                    {
                        tPattern = aeListView.GetCurrentPattern(TablePatternIdentifiers.Pattern) as TablePattern;
                        return (tPattern != null);
                    }, "ATT2-10 GridView should support TablePattern");

                // GridPattern & TablePattern has the correct values
                //
                // Note: shouldn't have race here, because
                //  1.) for ColumnCount, as long as the gPattern has value, that means ListView
                // had got GridView for its View property.
                //  2.) for RowCount, since view change doesn't affect item list, therefore, need
                // not pooling to fetch the item count.
                DRT.Assert(gPattern.Current.ColumnCount == 3 && gPattern.Current.RowCount == 15, "ATT2-11 Column/RowCount should be 3/15");

                RobustVerify(
                    delegate()
                    {
                        objs = tPattern.Current.GetColumnHeaders();
                        return (objs != null && objs.Length == 3);
                    }, "ATT2-12 TablePattern.GetColumnHeaders should return array with 3 content");

                //switch View to null
                InvokeButton(aeBtnView0);

                RobustVerify(
                    delegate()
                    {
                        try
                        {
                            aeListView.GetCurrentPattern(GridPatternIdentifiers.Pattern);
                            return false;
                        }
                        catch (InvalidOperationException)
                        {
                            return true;
                        }
                    }, "ATT2-13 ListView shouldn't support GridPattern by default");

                RobustVerify(
                    delegate()
                    {
                        try
                        {
                            aeListView.GetCurrentPattern(TablePatternIdentifiers.Pattern);
                            return false;
                        }
                        catch (InvalidOperationException)
                        {
                            return true;
                        }
                    }, "ATT2-14 ListView shouldn't support TablePattern by default");


                //2) SelectionPattern
                //switch to gridview
                InvokeButton(aeBtnView1);

                RobustVerify(
                    delegate()
                    {
                        sPattern = aeListView.GetCurrentPattern(SelectionPatternIdentifiers.Pattern) as SelectionPattern;
                        return (sPattern != null);
                    }, "ATT3-1 GridView should support SelectionPattern");

                RobustVerify(
                    delegate()
                    {
                        objs = sPattern.Current.GetSelection();
                        return (objs == null || objs.Length == 0);
                    }, "ATT3-2 No item is selected by default");

                //Select 3 items
                InvokeButton(aeBtnSelect1);

                RobustVerify(
                    delegate()
                    {
                        objs = sPattern.Current.GetSelection();
                        return (objs != null && objs.Length == 3);
                    }, "ATT3-3 3 items are selected");

                //Set SelectionMode to Single
                InvokeButton(aeBtnSelect2);

                RobustVerify(
                    delegate()
                    {
                        objs = sPattern.Current.GetSelection();
                        return (objs != null && objs.Length == 1);
                    }, "ATT3-4 1 items is selected");

                DRT.Assert(!sPattern.Current.CanSelectMultiple, "ATT3-5 CanSelectMultiple should be false");

                //Set SelectedIndex to -1
                InvokeButton(aeBtnSelect3);

                RobustVerify(
                    delegate()
                    {
                        objs = sPattern.Current.GetSelection();
                        return (objs == null || objs.Length == 0);
                    }, "ATT3-6 No items ise selected");

                //3) GridPattern
                //Switch to GridView2
                InvokeButton(aeBtnView2);

                RobustVerify(
                    delegate()
                    {
                        gPattern = aeListView.GetCurrentPattern(GridPatternIdentifiers.Pattern) as GridPattern;
                        return (gPattern != null);
                    }, "ATT4-1 GridView should support GridPattern");

                DRT.Assert(gPattern.Current.ColumnCount == 3 && gPattern.Current.RowCount == 15, "ATT4-2 Column/RowCount should be 3/15");


                RobustVerify(
                    delegate()
                    {
                        object obj = gPattern.GetItem(0, 0);
                        return (obj != null);
                    }, "ATT4-3 GetItem shouldn't return null");

                RobustVerify(
                    delegate()
                    {
                        object obj = gPattern.GetItem(0, 2);
                        return (obj != null);
                    }, "ATT4-4 GetItem shouldn't return null");

                RobustVerify(
                    delegate()
                    {
                        object obj = gPattern.GetItem(14, 2);
                        return (obj != null);
                    }, "ATT4-5 GetItem shouldn't return null");


                RobustVerify(
                    delegate()
                    {
                        bool bEx = false;
                        try
                        {
                            gPattern.GetItem(14, 3);
                        }
                        catch (Exception)
                        {
                            bEx = true;
                        }
                        return bEx;
                    }, "ATT4-6 GetItem should throw exception");

                //Add one item
                InvokeButton(aeBtnAddItem);
                RobustVerify(
                    delegate()
                    {
                        return (gPattern.Current.ColumnCount == 3 && gPattern.Current.RowCount == 16);
                    }, "ATT4-7 Column/RowCount should be 3/16");

                //Remove 2 items
                InvokeButton(aeBtnRemoveItem);
                RobustVerify(
                    delegate()
                    {
                        return (gPattern.Current.ColumnCount == 3 && gPattern.Current.RowCount == 14);
                    }, "ATT4-8 Column/RowCount should be 3/14");

                //Clear items
                InvokeButton(aeBtnClearItem);
                RobustVerify(
                    delegate()
                    {
                        return (gPattern.Current.ColumnCount == 3 && gPattern.Current.RowCount == 0);
                    }, "ATT4-9 Column/RowCount should be 3/0");

                //Add Items(15)
                InvokeButton(aeBtnAddItems);
                RobustVerify(
                    delegate()
                    {
                        return (gPattern.Current.ColumnCount == 3 && gPattern.Current.RowCount == 15);
                    },
                    "ATT4-10 Column/RowCount should be 3/15"
                );

                //4) TablePattern
                RobustVerify(
                   delegate()
                   {
                       gPattern = aeListView.GetCurrentPattern(GridPatternIdentifiers.Pattern) as GridPattern;
                       return (gPattern != null);
                   }, "ATT5-1 GridView should support GridPattern");

                RobustVerify(
                   delegate()
                   {
                       tPattern = aeListView.GetCurrentPattern(TablePatternIdentifiers.Pattern) as TablePattern;
                       return (tPattern != null);
                   }, "ATT5-2 GridView should support TablePattern");

                DRT.Assert(gPattern.Current.ColumnCount == 3, "ATT5-3 ColumnCount should be 3");

                RobustVerify(
                    delegate()
                    {
                        objs = tPattern.Current.GetColumnHeaders();
                        return (objs != null && objs.Length == 3);
                    }, "ATT5-4 GetColumnHeaders should return an array with 3 items");

                //Add one column
                InvokeButton(aeBtnAddColumn);
                RobustVerify(
                    delegate()
                    {
                        objs = tPattern.Current.GetColumnHeaders();
                        return (objs != null && objs.Length == 4);
                    },
                    "ATT5-5 GetColumnHeaders should return an array with 4 items"
                );

                //Remove 2 columns
                InvokeButton(aeBtnRemoveColumn);
                RobustVerify(
                    delegate()
                    {
                        objs = tPattern.Current.GetColumnHeaders();
                        return (objs != null && objs.Length == 2);
                    },
                    "ATT5-6 GetColumnHeaders should return an array with 2 items"
                );

                //Clear column
                InvokeButton(aeBtnClearColumn);
                RobustVerify(
                    delegate()
                    {
                        objs = tPattern.Current.GetColumnHeaders();
                        return (objs == null || objs.Length == 0);
                    },
                    "ATT5-7 GetColumnHeaders should return an array with 0 item"
                );

                //switch to another view
                InvokeButton(aeBtnView1);
                RobustVerify(
                    delegate()
                    {
                        objs = tPattern.Current.GetColumnHeaders();
                        return (objs != null && objs.Length == 3);
                    },
                    "ATT5-8 GetColumnHeaders should return an array with 3 item"
                );

                //5) Grid pattern with virtualization and grouping
                //add 1000 items
                InvokeButton(aeBtnAdd1000Items);

                RobustVerify(
                   delegate()
                   {
                       gPattern = aeListView.GetCurrentPattern(GridPatternIdentifiers.Pattern) as GridPattern;
                       return (gPattern != null);
                   }, "ATT6-1 GridView should support GridPattern");

                RobustVerify(
                    delegate()
                    {
                        return (gPattern.Current.ColumnCount == 3 && gPattern.Current.RowCount == 1000);
                    }, "ATT6-2 Column/RowCount should be 3/1000");

                // GetItem should scroll item into view
                RobustVerify(
                    delegate()
                    {
                        object obj = gPattern.GetItem(0, 0);
                        return (obj != null);
                    }, "ATT6-3 GetItem shouldn't return null");

                RobustVerify(
                    delegate()
                    {
                        object obj = gPattern.GetItem(200, 1);
                        return (obj != null);
                    }, "ATT6-4 GetItem shouldn't return null");

                RobustVerify(
                    delegate()
                    {
                        object obj = gPattern.GetItem(400, 2);
                        return (obj != null);
                    }, "ATT6-5 GetItem shouldn't return null");

                RobustVerify(
                    delegate()
                    {
                        object obj = gPattern.GetItem(600, 0);
                        return (obj != null);
                    }, "ATT6-6 GetItem shouldn't return null");

                RobustVerify(
                    delegate()
                    {
                        object obj = gPattern.GetItem(800, 1);
                        return (obj != null);
                    }, "ATT6-7 GetItem shouldn't return null");

                RobustVerify(
                    delegate()
                    {
                        object obj = gPattern.GetItem(999, 2);
                        return (obj != null);
                    }, "ATT6-8 GetItem shouldn't return null");

                // add one level of grouping, verify grid pattern still works
                InvokeButton(aeBtnToggleGrouping1);

                RobustVerify(
                    delegate()
                    {
                        return (gPattern.Current.ColumnCount == 3 && gPattern.Current.RowCount == 1000);
                    }, "ATT6-9 Column/RowCount should be 3/1000");

                // GetItem should scroll item into view
                RobustVerify(
                    delegate()
                    {
                        object obj = gPattern.GetItem(0, 0);
                        return (obj != null);
                    }, "ATT6-10 GetItem shouldn't return null");

                RobustVerify(
                    delegate()
                    {
                        object obj = gPattern.GetItem(200, 1);
                        return (obj != null);
                    }, "ATT6-11 GetItem shouldn't return null");

                RobustVerify(
                    delegate()
                    {
                        object obj = gPattern.GetItem(400, 2);
                        return (obj != null);
                    }, "ATT6-12 GetItem shouldn't return null");

                RobustVerify(
                    delegate()
                    {
                        object obj = gPattern.GetItem(600, 0);
                        return (obj != null);
                    }, "ATT6-13 GetItem shouldn't return null");

                RobustVerify(
                    delegate()
                    {
                        object obj = gPattern.GetItem(800, 1);
                        return (obj != null);
                    }, "ATT6-14 GetItem shouldn't return null");

                RobustVerify(
                    delegate()
                    {
                        object obj = gPattern.GetItem(999, 2);
                        return (obj != null);
                    }, "ATT6-15 GetItem shouldn't return null");

                // add two levels of grouping, verify grid pattern still works
                InvokeButton(aeBtnToggleGrouping2);

                RobustVerify(
                    delegate()
                    {
                        return (gPattern.Current.ColumnCount == 3 && gPattern.Current.RowCount == 1000);
                    }, "ATT6-16 Column/RowCount should be 3/1000");

                // GetItem should scroll item into view
                RobustVerify(
                    delegate()
                    {
                        object obj = gPattern.GetItem(0, 0);
                        return (obj != null);
                    }, "ATT6-17 GetItem shouldn't return null");

                RobustVerify(
                    delegate()
                    {
                        object obj = gPattern.GetItem(200, 1);
                        return (obj != null);
                    }, "ATT6-18 GetItem shouldn't return null");

                RobustVerify(
                    delegate()
                    {
                        object obj = gPattern.GetItem(400, 2);
                        return (obj != null);
                    }, "ATT6-19 GetItem shouldn't return null");

                RobustVerify(
                    delegate()
                    {
                        object obj = gPattern.GetItem(600, 0);
                        return (obj != null);
                    }, "ATT6-20 GetItem shouldn't return null");

                RobustVerify(
                    delegate()
                    {
                        object obj = gPattern.GetItem(800, 1);
                        return (obj != null);
                    }, "ATT6-21 GetItem shouldn't return null");

                RobustVerify(
                    delegate()
                    {
                        object obj = gPattern.GetItem(999, 2);
                        return (obj != null);
                    }, "ATT6-22 GetItem shouldn't return null");
            }
            finally
            {
                if (process != null)
                    process.Kill();
            }
        }

        private AutomationElement RobustFindElement(AutomationElement root, string elementId, string caseId)
        {
            const int MAXTIME = 3000; // wait 3 seconds at most
            const int TIMEWAIT = 100; // polling time interval
            int runningTime = 0;
            AutomationElement ae = null;

            while (runningTime <= MAXTIME)
            {
                ae = root.FindFirst(TreeScope.Descendants | TreeScope.Children,
                    new PropertyCondition(AutomationElement.AutomationIdProperty, elementId));

                if (ae != null)
                    break;

                Thread.Sleep(TIMEWAIT);
                runningTime += TIMEWAIT;
            }

            DRT.Assert(ae != null, caseId + " Could not find instances of id " + elementId);

            return ae;
        }

        delegate bool Query();

        private void RobustVerify(Query query, string assertFailMsg)
        {
            const int MAXTIME = 3000; // wait 3 seconds at most
            const int TIMEWAIT = 100; // polling time interval
            int runningTime = 0;

            while (runningTime <= MAXTIME)
            {
                if (query())
                    break;

                Thread.Sleep(TIMEWAIT);
                runningTime += TIMEWAIT;
            }

            DRT.Assert(runningTime < MAXTIME, assertFailMsg);
        }

        private void InvokeButton(AutomationElement aeBtn)
        {
            InvokePattern ip = aeBtn.GetCurrentPattern(InvokePatternIdentifiers.Pattern) as InvokePattern;
            DRT.Assert(ip != null, "Button should support invoke pattern");
            ip.Invoke();
        }

        #endregion

        #region Test DisplayMemberBinding and CellTemplate work together

        DMB_CT_Step _DMP_CT_Step = DMB_CT_Step.Start;

        enum DMB_CT_Step
        {
            Start,

            // Change DisplayMemberBinding.
            Test1_DMB_To_DMB,
            Test1_Verify,

            // Change CellTemplate
            Test2_CT_TO_CT,
            Test2_Verify,

            // Change CellTemplateSelector
            Test3_CTS_TO_CTS,
            Test3_Verify,

            // Test for priority: DisplayMemberBinding > CellTemplate > CellTemplateSelector

            // (CellTemplateSelector) + CellTemplate -> CellTemplate work
            Test4_Set_CellTemplate,
            Test4_Verify,

            // (CellTemplate + CellTemplateSelector) + DisplayMemberBinding -> DisplayMemberBinding work
            Test5_Set_DisplayMemberBinding,
            Test5_Verify,

            // (DisplayMemberBinding + CellTemplate + CellTemplateSelector) - DisplayMemberBinding -> CellTemplate work
            Test6_Clear_DisplayMemberBinding,
            Test6_Verify,

            // (CellTemplate + CellTemplateSelector) - CellTemplate -> CellTemplateSelector work
            Test7_Clear_CellTemplate,
            Test7_Verify,

            End,
        }

        private void DMB_CT_Test()
        {
            if (DRT.Verbose) Console.WriteLine("DMP_CT_Test: " + _DMP_CT_Step);

            switch (_DMP_CT_Step)
            {
                case DMB_CT_Step.Start:
                    _listView.View = CreateGridView2();
                    break;

                case DMB_CT_Step.Test1_DMB_To_DMB:
                    {
                        GridView view = _listView.View as GridView;
                        DRT.Assert(view != null, "DMP_CT_Test1-1 _listView.View should be GridView");
                        // Test early-binding here
                        Binding binding = new Binding();
                        binding.Path = new PropertyPath("(0)");
                        PropertyInfo dataInfo = typeof(DateTime).GetProperty("Month");
                        binding.Path.PathParameters.Add(dataInfo);
                        view.Columns[0].DisplayMemberBinding = binding;
                    }
                    break;

                case DMB_CT_Step.Test1_Verify:
                    CheckCellStyle(0, "1", typeof(TextBlock), null);
                    break;

                case DMB_CT_Step.Test2_CT_TO_CT:
                    {
                        GridView view = _listView.View as GridView;
                        DRT.Assert(view != null, "DMP_CT_Test2-1 _listView.View should be GridView");
                        view.Columns[1].CellTemplate = _canvas.Resources["CellTemplate_Year_Yellow"] as DataTemplate;
                    }
                    break;

                case DMB_CT_Step.Test2_Verify:
                    CheckCellStyle(1, "1900", typeof(ContentPresenter), Brushes.Yellow);
                    break;

                case DMB_CT_Step.Test3_CTS_TO_CTS:
                    {
                        GridView view = _listView.View as GridView;
                        DRT.Assert(view != null, "DMP_CT_Test3-1 _listView.View should be GridView");
                        view.Columns[2].CellTemplateSelector = new CellTemplateSelector_Month_Green();
                    }
                    break;

                case DMB_CT_Step.Test3_Verify:
                    CheckCellStyle(2, "1", typeof(ContentPresenter), Brushes.Green);
                    break;


                // Test for priority: DisplayMemberBinding > CellTemplate > CellTemplateSelector

                case DMB_CT_Step.Test4_Set_CellTemplate:
                    {
                        GridView view = _listView.View as GridView;
                        DRT.Assert(view != null, "DMP_CT_Test4-1 _listView.View should be GridView");
                        view.Columns[2].CellTemplate = _canvas.Resources["CellTemplate_Year_Yellow"] as DataTemplate;
                    }
                    break;

                case DMB_CT_Step.Test4_Verify:
                    CheckCellStyle(2, "1900", typeof(ContentPresenter), Brushes.Yellow);
                    break;

                case DMB_CT_Step.Test5_Set_DisplayMemberBinding:
                    {
                        GridView view = _listView.View as GridView;
                        DRT.Assert(view != null, "DMP_CT_Test5-1 _listView.View should be GridView");
                        view.Columns[2].DisplayMemberBinding = new Binding("Day");
                    }
                    break;

                case DMB_CT_Step.Test5_Verify:
                    CheckCellStyle(2, "2", typeof(TextBlock), null);
                    break;

                case DMB_CT_Step.Test6_Clear_DisplayMemberBinding:
                    {
                        GridView view = _listView.View as GridView;
                        DRT.Assert(view != null, "DMP_CT_Test6-1 _listView.View should be GridView");
                        view.Columns[2].DisplayMemberBinding = null;
                    }
                    break;

                case DMB_CT_Step.Test6_Verify:
                    CheckCellStyle(2, "1900", typeof(ContentPresenter), Brushes.Yellow);
                    break;

                case DMB_CT_Step.Test7_Clear_CellTemplate:
                    {
                        GridView view = _listView.View as GridView;
                        DRT.Assert(view != null, "DMP_CT_Test7-1 _listView.View should be GridView");
                        view.Columns[2].CellTemplate = null;
                    }
                    break;

                case DMB_CT_Step.Test7_Verify:
                    CheckCellStyle(2, "1", typeof(ContentPresenter), Brushes.Green);
                    break;

                case DMB_CT_Step.End:
                    break;

                default:
                    break;
            }

            if (_DMP_CT_Step++ <= DMB_CT_Step.End)
            {
                DRT.RepeatTest();
            }
        }

        #endregion Test DisplayMemberPath and CellTemplate work together

        #region Test Template/TemplateSelector on Header/Column/GridView

        // Generally, one rule.
        //
        //  Rule one: Template > TemplateSelector
        //  Rule two: Level 1 > Level 2 > Level 3
        //
        // Level 1: properties directly set on user provided Header
        // Level 2: properties on GridViewColumn
        // Level 3: properties on GridView
        //
        // So, the result is
        // Header.HT > Column.HT > GridView.HT > Header.HTS > Column.HTS > GridView.HTS

        HT_HTS_Step _HT_HTS_Step = HT_HTS_Step.Start;

        enum HT_HTS_Step
        {
            // Header.HT + Column.HT + GridView.HT + Header.HTS + Column.HTS + GridView.HTS -> Header.HT work
            Start,
            Test0_Verify,

            // Column.HT + GridView.HT + Header.HTS + Column.HTS + GridView.HTS -> Column.HT work
            Test1_Clear_HeaderHT,
            Test1_Verify,

            // GridView.HT + Header.HTS + Column.HTS + GridView.HTS -> GridView.HT work
            Test2_Clear_ColumnHT,
            Test2_Verify,

            // Header.HTS + Column.HTS + GridView.HTS -> Header.HTS work
            Test3_Clear_GridViewHT,
            Test3_Verify,

            // Column.HTS + GridView.HTS -> Column.HTS work
            Test4_Clear_HeaderHTS,
            Test4_Verify,

            // GridView.HTS -> GridView.HTS work
            Test5_Clear_ColumnHTS,
            Test5_Verify,

            End,
        }

        private void HT_HTS_Test()
        {
            if (DRT.Verbose) Console.WriteLine("HT_HTS_Test: " + _HT_HTS_Step);

            /*
            <GridView ColumnHeaderTemplate="{StaticResource ColumnHeaderTemplate_Day_Pink}"
                      ColumnHeaderTemplateSelector="{new ColumnHeaderTemplateSelector_Brown()}" >
                <GridViewColumn HeaderTemplate="{StaticResource ColumnHeaderTemplate_Month_Blue}"
                                HeaderTemplateSelector="{new ColumnHeaderTemplateSelector_Green()}"
                                DisplayMemberBinding="{Binding Year}"
                                Width="120">
                    <GridViewColumnHeader ContentTemplate="{StaticResource ColumnHeaderTemplate_Year_Yellow}"
                                          ContentTemplateSelector="{new ColumnHeaderTemplateSelector_Red()}" >
                        User Header0
                    </GridViewColumnHeader>
                </GridViewColumn>
            </GridView>
            */

            switch (_HT_HTS_Step)
            {
                // 0
                case HT_HTS_Step.Start:
                    _listView.View = CreateGridView3();
                    break;

                case HT_HTS_Step.Test0_Verify:
                    CheckColumnHeaderStyle(0, "Year", Brushes.Yellow);
                    break;

                // 1
                case HT_HTS_Step.Test1_Clear_HeaderHT:
                    {
                        GridView view = _listView.View as GridView;
                        DRT.Assert(view != null, "HT_HTS_Test1-1 _listView.View should be GridView");
                        GridViewColumnHeader header = view.Columns[0].Header as GridViewColumnHeader;
                        DRT.Assert(header != null, "HT_HTS_Test1-2 _listView.View.Columns[0].Header should be GridViewColumnHeader");
                        header.ClearValue(GridViewColumnHeader.ContentTemplateProperty);
                    }
                    break;

                case HT_HTS_Step.Test1_Verify:
                    CheckColumnHeaderStyle(0, "Month", Brushes.Blue);
                    break;

                // 2
                case HT_HTS_Step.Test2_Clear_ColumnHT:
                    {
                        GridView view = _listView.View as GridView;
                        DRT.Assert(view != null, "HT_HTS_Test2-1 _listView.View should be GridView");
                        view.Columns[0].HeaderTemplate = null;
                    }
                    break;

                case HT_HTS_Step.Test2_Verify:
                    CheckColumnHeaderStyle(0, "Day", Brushes.Pink);
                    break;

                // 3
                case HT_HTS_Step.Test3_Clear_GridViewHT:
                    {
                        GridView view = _listView.View as GridView;
                        DRT.Assert(view != null, "HT_HTS_Test3-1 _listView.View should be GridView");
                        view.ColumnHeaderTemplate = null;
                    }
                    break;

                case HT_HTS_Step.Test3_Verify:
                    CheckColumnHeaderStyle(0, "User Header0", Brushes.Red);
                    break;

                // 4
                case HT_HTS_Step.Test4_Clear_HeaderHTS:
                    {
                        GridView view = _listView.View as GridView;
                        DRT.Assert(view != null, "HT_HTS_Test4-1 _listView.View should be GridView");
                        GridViewColumnHeader header = view.Columns[0].Header as GridViewColumnHeader;
                        DRT.Assert(header != null, "HT_HTS_Test4-2 _listView.View.Columns[0].Header should be GridViewColumnHeader");
                        header.ClearValue(GridViewColumnHeader.ContentTemplateSelectorProperty);
                    }
                    break;

                case HT_HTS_Step.Test4_Verify:
                    CheckColumnHeaderStyle(0, "User Header0", Brushes.Green);
                    break;

                // 5
                case HT_HTS_Step.Test5_Clear_ColumnHTS:
                    {
                        GridView view = _listView.View as GridView;
                        DRT.Assert(view != null, "HT_HTS_Test5-1 _listView.View should be GridView");
                        view.Columns[0].HeaderTemplateSelector = null;
                    }
                    break;

                case HT_HTS_Step.Test5_Verify:
                    CheckColumnHeaderStyle(0, "User Header0", Brushes.Brown);
                    break;

                case HT_HTS_Step.End:
                    break;

                default:
                    break;
            }

            if (_HT_HTS_Step++ <= HT_HTS_Step.End)
            {
                DRT.RepeatTest();
            }
        }

        #endregion Template/TemplateSelector on Header/Column/GridView

        /// <summary>
        /// Used for testing Controls that require you to dig into the Visual Tree of a Control
        /// to get at it's parts.
        /// </summary>
        private static class ListViewVisualTreeHelper
        {
            public static GridViewColumnHeader FindColumnHeaderByColumn(ListView listView, GridViewColumn column)
            {
                ArrayList columnHeaders = FindPartByType(listView, typeof(GridViewColumnHeader));
                foreach (GridViewColumnHeader header in columnHeaders)
                {
                    if (header != null && header.Column == column)
                    {
                        return header;
                    }
                }

                return null;
            }

            public static Thumb FindGripByColumn(ListView listView, GridViewColumn column)
            {
                GridViewColumnHeader header = FindColumnHeaderByColumn(listView, column);
                if (header != null)
                {
                    return FindPartByName(header, "PART_HeaderGripper") as Thumb;
                }
                return null;
            }

            public static FrameworkElement FindCellInItem(ListViewItem item, int cellIndex)
            {
                GridViewRowPresenter rowPresenter = FindPartByType(item, typeof(GridViewRowPresenter), 0) as GridViewRowPresenter;
                if (rowPresenter != null)
                {
                    int count = VisualTreeHelper.GetChildrenCount(rowPresenter);
                    if (cellIndex < count)
                    {
                        return VisualTreeHelper.GetChild(rowPresenter, cellIndex) as FrameworkElement;
                    }
                }

                return null;
            }

            /// <summary>
            /// Used to get an item of a specific type in the visual tree.
            /// So to find the 2nd item of type Button it would be something like.
            /// FindPartByType(visual,typeof(Button),1);
            /// </summary>
            /// <param name="vis">The visual who's tree you want to search.</param>
            /// <param name="visType">The type of object you want to find.</param>
            /// <param name="index">The count of the item as it is found in the tree.</param>
            /// <returns>The object of type visType found in vis that is the index item</returns>
            public static object FindPartByType(System.Windows.Media.Visual vis, System.Type visType, int index)
            {
                if (vis != null)
                {
                    System.Collections.ArrayList parts = FindPartByType(vis, visType);

                    if (index >= 0 && index < parts.Count)
                    {
                        return parts[index];
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }

            /// <summary>
            /// Returns an ArrayList of all of the items of the specified type in the visual tree.
            /// </summary>
            /// <param name="vis">The visual who's tree you want to search.</param>
            /// <param name="visType">The type of object you want to find.</param>
            /// <returns>An ArrayList containing all of the objects of type visType found in the tree of vis.</returns>
            public static System.Collections.ArrayList FindPartByType(System.Windows.Media.Visual vis, System.Type visType)
            {
                System.Collections.ArrayList parts = new System.Collections.ArrayList();

                if (vis != null)
                {
                    parts = FindPartByTypeRecurs(vis, visType, parts);
                }

                return parts;
            }

            private static System.Collections.ArrayList FindPartByTypeRecurs(DependencyObject vis, System.Type visType, System.Collections.ArrayList parts)
            {
                if (vis != null)
                {
                    if (vis.GetType() == visType)
                    {
                        parts.Add(vis);
                    }

                    int count = VisualTreeHelper.GetChildrenCount(vis);
                    for (int i = 0; i < count; i++)
                    {
                        DependencyObject curVis = VisualTreeHelper.GetChild(vis, i);
                        parts = FindPartByTypeRecurs(curVis, visType, parts);
                    }

                }

                return parts;
            }

            /// <summary>
            /// traverse the visual tree to get a visua by Name.
            /// </summary>
            /// <param name="ID"></param>
            /// <param name="ele"></param>
            /// <returns></returns>
            private static DependencyObject FindPartByName(DependencyObject ele, string name)
            {
                DependencyObject result;
                if (ele == null)
                {
                    return null;
                }
                if (name.Equals(ele.GetValue(FrameworkElement.NameProperty)))
                {
                    return ele;
                }

                int count = VisualTreeHelper.GetChildrenCount(ele);
                for (int i = 0; i < count; i++)
                {
                    DependencyObject vis = VisualTreeHelper.GetChild(ele, i);
                    if ((result = FindPartByName(vis, name)) != null)
                    {
                        return result;
                    }
                }

                return null;
            }
        }
    }
}

