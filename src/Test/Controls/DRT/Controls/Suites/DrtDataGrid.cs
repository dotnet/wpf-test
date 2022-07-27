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
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Threading;
using System.Diagnostics;

namespace DRT
{
    public class DrtDataGridSuite : DrtTestSuite
    {
        public DrtDataGridSuite()
            : base("DataGrid")
        {
            Contact = "Microsoft";
            _testGrid = null;
        }

        public override DrtTest[] PrepareTests()
        {
            if (!_isPageLoaded)
            {
                string fileName = DRT.BaseDirectory + "DrtDataGrid.xaml";
                LoadXamlPage(fileName);
                _isPageLoaded = true;
            }
            else
            {
                Keyboard.Focus(null);
            }

            if (!DRT.KeepAlive)
            {
                List<DrtTest> tests = new List<DrtTest>();

                tests.Add(new DrtTest(Setup));
                tests.Add(new DrtTest(TestRowDetails));
                tests.Add(new DrtTest(TestColumnRemoval));
                tests.Add(new DrtTest(Column_GetCellContent));
                tests.Add(new DrtTest(Toggle_Binding_1));
                tests.Add(new DrtTest(Toggle_Binding_2));
                tests.Add(new DrtTest(Toggle_Binding_3));
                tests.Add(new DrtTest(Toggle_IsEditing));
                tests.Add(new DrtTest(Toggle_CellStyle));
                tests.Add(new DrtTest(Toggle_RowBackground));
                tests.Add(new DrtTest(Toggle_AlternatingRowBackground));
                tests.Add(new DrtTest(AlternatingBackgroundStyle));
                tests.Add(new DrtTest(Toggle_Columns));
                tests.Add(new DrtTest(Toggle_ColumnHeader));
                tests.Add(new DrtTest(Toggle_ColumnHeaderTemplate));
                tests.Add(new DrtTest(Toggle_ColumnHeaderStyle));
                tests.Add(new DrtTest(Style_EditingElementStyle));
                tests.Add(new DrtTest(TestColumnWidths));
                tests.Add(new DrtTest(TestColumnMinMaxWidth));
                tests.Add(new DrtTest(TestRowHeaders));
                tests.Add(new DrtTest(TestRowHeaderStyle));
                tests.Add(new DrtTest(TestRowHeight));
                tests.Add(new DrtTest(EditMode_CurrentCell));
                tests.Add(new DrtTest(Selection));
                tests.Add(new DrtTest(EditMode_Input));
                tests.Add(new DrtTest(EditMode_Events));
                tests.Add(new DrtTest(EditMode_Row));
                tests.Add(new DrtTest(TestDisplayIndex));
                tests.Add(new DrtTest(TestDisplayIndexValidation));
                tests.Add(new DrtTest(ClipboardTest));
                tests.Add(new DrtTest(AddRemoveColumns));
                tests.Add(new DrtTest(TestAutoSorting));
                tests.Add(new DrtTest(TestGroupingAndFiltering));
                tests.Add(new DrtTest(TestAutoColumnGeneration));
                tests.Add(new DrtTest(FrozenColumns));
                tests.Add(new DrtTest(ReorderColumns));
                tests.Add(new DrtTest(TestHeadersVisibility));
                tests.Add(new DrtTest(TestHeaderSizes));
                tests.Add(new DrtTest(TestRowValidation));
                tests.Add(new DrtTest(TestColumnWidthChange));
                tests.Add(new DrtTest(TestIsReadOnly));
                tests.Add(new DrtTest(ColumnVirtualizationTest));
                tests.Add(new DrtTest(HiddenColumnsTest));
                tests.Add(new DrtTest(IsReadOnlyColumnTest));
                return tests.ToArray();
            }
            else
            {
                return new DrtTest[] { new DrtTest(Setup) };
            }
        }

        private void LoadXamlPage(string fileName)
        {
            System.IO.Stream stream = File.OpenRead(fileName);
            Visual root = (Visual)XamlReader.Load(stream);
            InitTree(root);

            if (DRT.KeepAlive)
            {
                FrameworkElement rootBorder = (FrameworkElement)DRT.FindVisualByID("Root_Border", root);
                DRT.MainWindow.SizeToContent = SizeToContent.Manual;
                Matrix deviceTransform = DRT.MainWindow.CompositionTarget.TransformToDevice;
                Point pt = deviceTransform.Transform(new Point(rootBorder.Width, rootBorder.Height));
                DrtBase.SetWindowPos(DRT.MainWindow.Handle, IntPtr.Zero, 0, 0, (int)pt.X, (int)pt.Y, DrtBase.SWP_NOMOVE | DrtBase.SWP_NOZORDER);
                rootBorder.ClearValue(FrameworkElement.WidthProperty);
                rootBorder.ClearValue(FrameworkElement.HeightProperty);
            }

            DRT.Show(root);
        }

        private void InitTree(DependencyObject root)
        {
            _testGrid = (DataGrid)DRT.FindVisualByID("DataGrid_Standard", root);
            _testGrid.LoadingRowDetails += (s, e) =>
            {
                Debug.WriteLine("LoadingRowDetails");
            };

            _testGrid.UnloadingRowDetails += (s, e) =>
            {
                Debug.WriteLine("UnloadingRowDetails");
            };

            DRT.Assert(_testGrid != null, "Standard DataGrid not found.");

            _mainGrid = (Grid)DRT.FindVisualByID("MainGrid", root);

            _repeatCountChooser = (ComboBox)DRT.FindVisualByID("RepeatCountChooser", root);
            _repeatCountChooser.SelectionChanged += new SelectionChangedEventHandler(OnRepeatCountChooserChanged);

            _useIEditableObject = (CheckBox)DRT.FindVisualByID("IEditableObjectToggle", root);
            _useIEditableObject.Checked += new RoutedEventHandler(OnIEditableObjectToggleCheckedChanged);
            _useIEditableObject.Unchecked += new RoutedEventHandler(OnIEditableObjectToggleCheckedChanged);

            _checkBoxCopyHeaders = (CheckBox)DRT.FindVisualByID("checkBoxCopyHeaders", root);
            _checkBoxCopyHeaders.Checked += new RoutedEventHandler(OnCheckBoxCopyHeadersCheckedChanged);
            _checkBoxCopyHeaders.Unchecked += new RoutedEventHandler(OnCheckBoxCopyHeadersCheckedChanged);

            _checkBoxFreezeColumns = (CheckBox)DRT.FindVisualByID("checkBoxFreezeColumns", root);
            _checkBoxFreezeColumns.Checked += new RoutedEventHandler(OnCheckBoxFreezeColumnsCheckedChanged);
            _checkBoxFreezeColumns.Unchecked += new RoutedEventHandler(OnCheckBoxFreezeColumnsCheckedChanged);

            _newItemPlaceholderChooser = (ComboBox)DRT.FindVisualByID("NewItemPlaceholderChooser", root);
            _newItemPlaceholderChooser.SelectionChanged += new SelectionChangedEventHandler(OnNewItemPlaceholderChanged);

            _makeDetailsVisible = (Button)DRT.FindVisualByID("MakeDetailsVisible", root);
            _makeDetailsVisible.Click += (s, e) =>
            {
                if (_testGrid.SelectedItem != null)
                {
                    var row = (DataGridRow)_testGrid.ItemContainerGenerator.ContainerFromItem(_testGrid.SelectedItem);
                    if (row != null)
                    {
                        row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    }
                }
            };

            var changeFontFamily = (Button)DRT.FindVisualByID("ChangeFontFamily", root);
            changeFontFamily.Click += (s, e) =>
            {
                var column = _testGrid.Columns[1] as DataGridTextColumn;
                if (column != null)
                {
                    var comic = new System.Windows.Media.FontFamily("Comic Sans MS");
                    var arial = new System.Windows.Media.FontFamily("Arial");
                    if (column.FontFamily.ToString() == comic.ToString())
                    {
                        column.FontFamily = arial;
                    }
                    else
                    {
                        column.FontFamily = comic;
                    }
                }
            };

            var changeIsThreeState = (Button)DRT.FindVisualByID("ChangeIsThreeState", root);
            changeIsThreeState.Click += (s, e) =>
            {
                var column = _testGrid.Columns[2] as DataGridCheckBoxColumn;
                if (column != null)
                {
                    column.IsThreeState = !column.IsThreeState;
                }
            };

            var toggleHiddenColumn = (Button)DRT.FindVisualByID("ToggleHiddenColumn", root);
            toggleHiddenColumn.Click += (s, e) =>
            {
                var column = _testGrid.Columns[2];
                column.Visibility = column.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            };

            var toggleIsReadOnly = (Button)DRT.FindVisualByID("ToggleIsReadOnly", root);
            toggleIsReadOnly.Click += (s, e) =>
            {
                var column = _testGrid.Columns[1];  // LastName column
                column.IsReadOnly = !column.IsReadOnly;
            };
        }

        private void OnRepeatCountChooserChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSetup();
        }

        private void OnNewItemPlaceholderChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_newItemPlaceholderChooser.SelectedItem != null)
            {
                ((IEditableCollectionView)_testGrid.Items).NewItemPlaceholderPosition = (NewItemPlaceholderPosition)_newItemPlaceholderChooser.SelectedItem;
            }
        }

        private void OnCheckBoxCopyHeadersCheckedChanged(object sender, RoutedEventArgs e)
        {
            _testGrid.ClipboardCopyMode = _checkBoxCopyHeaders.IsChecked==true ? DataGridClipboardCopyMode.IncludeHeader : DataGridClipboardCopyMode.ExcludeHeader;
        }

        private void OnCheckBoxFreezeColumnsCheckedChanged(object sender, RoutedEventArgs e)
        {
            _testGrid.FrozenColumnCount = ((bool)_checkBoxFreezeColumns.IsChecked) ? 2 : 0;
        }


        private void OnIEditableObjectToggleCheckedChanged(object sender, RoutedEventArgs e)
        {
            UpdateSetup();
        }

        private void UpdateSetup()
        {
            Setup((int)_repeatCountChooser.SelectedItem, true, _useIEditableObject.IsChecked == true);
        }

        private void Setup(int repeatCount, bool force, bool editable)
        {
            if (_people == null || force == true)
            {
                Person.ResetGlobalId();
                _people = new ObservableCollection<Person>();

                for (int i = 0; i < repeatCount; i++)
                {
                    Person p = NewPerson("George", "Washington", editable);
                    p.Cake = "Chocolate";
                    _people.Add(p);
                    _people.Add(NewPerson("John", "Adams", editable));
                    _people.Add(NewPerson("Thomas", "Jefferson", editable));
                    _people.Add(NewPerson("James", "Madison", editable));
                    _people.Add(NewPerson("James", "Monroe", editable));
                    _people.Add(NewPerson("John", "Quincy", "Adams", editable));
                    _people.Add(NewPerson("Andrew", "Jackson", editable));
                    _people.Add(NewPerson("Martin", "Van Buren", editable));
                    _people.Add(NewPerson("William", "H.", "Harrison", editable));
                    _people.Add(NewPerson("John", "Tyler", editable));
                    _people.Add(NewPerson("James", "K.", "Polk", editable));
                    _people.Add(NewPerson("Zachary", "Taylor", editable));
                    _people.Add(NewPerson("Millard", "Fillmore", editable));
                    _people.Add(NewPerson("Franklin", "Pierce", editable));
                    _people.Add(NewPerson("James", "Buchanan", editable));
                    _people.Add(NewPerson("Abraham", "Lincoln", editable));
                    _people.Add(NewPerson("Andrew", "Johnson", editable));
                    _people.Add(NewPerson("Ulysses", "S.", "Grant", editable));
                    _people.Add(NewPerson("Rutherford", "B.", "Hayes", editable));
                    _people.Add(NewPerson("James", "Garfield", editable));
                    _people.Add(NewPerson("Chester", "A.", "Arthur", editable));
                    _people.Add(NewPerson("Grover", "Cleveland", editable));
                    _people.Add(NewPerson("Benjamin", "Harrison", editable));
                    _people.Add(NewPerson("William", "McKinley", editable));
                    _people.Add(NewPerson("Theodore", "Roosevelt", editable));
                    _people.Add(NewPerson("William", "H.", "Taft", editable));
                    _people.Add(NewPerson("Woodrow", "Wilson", editable));
                    _people.Add(NewPerson("Warren", "G.", "Harding", editable));
                    _people.Add(NewPerson("Calvin", "Coolidge", editable));
                    _people.Add(NewPerson("Herbert", "Hoover", editable));
                    _people.Add(NewPerson("Franklin", "D.", "Roosevelt", editable));
                    _people.Add(NewPerson("Harry", "S.", "Truman", editable));
                    _people.Add(NewPerson("Dwight", "D.", "Eisenhower", editable));
                    _people.Add(NewPerson("John", "F.", "Kennedy", editable));
                    _people.Add(NewPerson("Lyndon", "B.", "Johnson", editable));
                    _people.Add(NewPerson("Richard", "Nixon", editable));
                    _people.Add(NewPerson("Gerald", "Ford", editable));
                    _people.Add(NewPerson("Jimmy", "Carter", editable));
                    _people.Add(NewPerson("Ronald", "Reagan", editable));
                    _people.Add(NewPerson("George", "Bush", editable));
                    _people.Add(NewPerson("Bill", "Clinton", editable));
                    _people.Add(NewPerson("George", "W.","Bush", editable));
                }
            }

            _testGrid.ItemsSource = _people;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
        }

        private Person NewPerson(string firstName, string lastName, bool editable)
        {
            return NewPerson(firstName, String.Empty, lastName, editable);
        }

        private Person NewPerson(string firstName, string middleName, string lastName, bool editable)
        {
            if (editable)
            {
                return new EditablePerson(firstName, middleName, lastName);
            }
            else
            {
                return new Person(firstName, middleName, lastName);
            }
        }

        private void Setup()
        {
            Setup(100, false, false);
        }

        private void Setup(bool force)
        {
            Setup(100, force, false);
        }

        #region Helpers

        private int GetVisibleColumnCount()
        {
            int count = _testGrid.Columns.Count;

            foreach (DataGridColumn column in _testGrid.Columns)
            {
                if (column.Visibility != Visibility.Visible)
                {
                    count--;
                }
            }
            return count;
        }

        // Returns the cells presenter from its parent (the DataGridRow)
        private DataGridCellsPresenter GetCellsPresenter(Visual parent)
        {
            return FindChild<DataGridCellsPresenter>(parent);
        }

        private DataGridCellInfo CellInfo(int row, int column)
        {
            return new DataGridCellInfo(_people[row], _testGrid.Columns[column]);
        }

        private DataGridCell GetCell(DataGridRow rowContainer, int column)
        {
            DataGridCellsPresenter presenter = GetCellsPresenter(rowContainer);
            return (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
        }

        private DataGridCell GetCell(int row, int column)
        {
            DataGridRow rowContainer = GetRow(row);
            if (rowContainer != null)
            {
                return GetCell(rowContainer, column);
            }

            return null;
        }

        private DataGridCell GetCell(DataGridCellInfo cellInfo)
        {
            DataGridRow rowContainer = GetRow(cellInfo.Item);
            if (rowContainer != null)
            {
                int columnIndex = _testGrid.Columns.IndexOf(cellInfo.Column);
                if (columnIndex >= 0)
                {
                    return GetCell(rowContainer, columnIndex);
                }
            }

            return null;
        }

        private DataGridColumn GetColumn(int column)
        {
            return _testGrid.Columns[column];
        }

        private DataGridColumn GetColumn(DataGridColumnHeader header)
        {
            return header.Column;
        }

        private DataGrid GetDataGridOwner(DataGridColumn column)
        {
            return (DataGrid)typeof(DataGridColumn).InvokeMember("DataGridOwner",
                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty,
                        null, column, null);
        }

        private List<int> GetDisplayIndexMap(DataGrid dataGrid)
        {
            return (List<int>)typeof(DataGrid).InvokeMember("DisplayIndexMap",
                                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty,
                                    null, dataGrid, null);
        }

        private DataGridColumnHeadersPresenter GetColumnHeadersPresenter(Visual parent)
        {
            return FindChild<DataGridColumnHeadersPresenter>(parent);
        }

        private DataGridColumnHeader GetColumnHeader(int index)
        {
            DataGridColumnHeadersPresenter presenter = GetColumnHeadersPresenter(_testGrid);

            if (presenter != null)
            {
                return (DataGridColumnHeader)presenter.ItemContainerGenerator.ContainerFromIndex(index);
            }

            return null;
        }

        private DataGridColumnHeader GetColumnHeaderFromDisplayIndex(int displayIndex)
        {
            int index = GetDisplayIndexMap(_testGrid)[displayIndex];
            return GetColumnHeader(index);
        }

        private DataGridRow GetRow(int index)
        {
            return (DataGridRow)_testGrid.ItemContainerGenerator.ContainerFromIndex(index);
        }

        private DataGridRow GetRow(object dataItem)
        {
            return (DataGridRow)_testGrid.ItemContainerGenerator.ContainerFromItem(dataItem);
        }

        private DataGridRowHeader GetRowHeader(int index)
        {
            return GetRowHeader(GetRow(index));
        }

        private DataGridRowHeader GetRowHeader(DataGridRow row)
        {
            if (row != null)
            {
                return (DataGridRowHeader)typeof(DataGridRow).InvokeMember("RowHeader",
                            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty,
                            null, row, null);
            }

            return null;
        }


        private ScrollViewer GetScrollViewer()
        {
            return FindChild<ScrollViewer>(_testGrid);
        }


        // Walks the subtree of the given visual and finds a child of the given type.
        private T FindChild<T>(Visual parent) where T : Visual
        {
            T child = null;
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;

                if (child == null)
                {
                    child = FindChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }

            return child;
        }

        // Walks up the visual tree to find a parent of the given type
        private T FindParent<T>(Visual child) where T : Visual
        {
            DependencyObject current = VisualTreeHelper.GetParent(child);
            while (current != null)
            {
                T parent = current as T;
                if (parent != null)
                    return parent;

                current = VisualTreeHelper.GetParent(current);
            }

            return null;
        }

        private void EnsureScrollViewer()
        {
            if (_scrollViewer == null)
            {
                _scrollViewer = GetScrollViewer();
            }
        }


        private void LineDown()
        {
            EnsureScrollViewer();
            _scrollViewer.LineDown();
        }

        private void LineUp()
        {
            EnsureScrollViewer();
            _scrollViewer.LineUp();
        }

        /// <summary>
        /// Walks through all rows and ensures that the content in the given column index matches the given property for the data item in that row.
        /// This is specific to text columns.
        /// </summary>
        private void ValidateTextColumn(int columnIndex, string propertyName)
        {
            PropertyInfo property = typeof(Person).GetProperty(propertyName);

            if (property != null)
            {
                // Validate that the current binding generated a valid string
                for (int rowIndex = 0; rowIndex < _people.Count; rowIndex++)
                {
                    DataGridCell cellContainer = GetCell(rowIndex, columnIndex);
                    if (cellContainer == null)
                    {
                        // We encountered virtualization, no more cells
                        break;
                    }

                    DRT.Assert(cellContainer.Content is TextBlock, string.Format("The cell for column {0} has the wrong content; it is of type {1} instead of TextBlock", columnIndex, cellContainer.Content.GetType()));
                    TextBlock textBlock = (TextBlock)cellContainer.Content;
                    string cellValue = textBlock.Text;
                    object propertyValue = property.GetValue(_people[rowIndex], null);

                    string dataValue = propertyValue as string;

                    if (dataValue == null)
                    {
                        dataValue = propertyValue.ToString();
                    }

                    DRT.Assert(cellValue == dataValue, String.Format("Value of TextBlock.Text for Column {0} (\"{1}\") does not match \"{2}\" (row = {3})", columnIndex, cellValue, dataValue, rowIndex));
                }
            }
        }

        private void ValidateFullNameColumn()
        {
            int columnIndex = 2;
            for (int rowIndex = 0; rowIndex < _people.Count; rowIndex++)
            {
                DataGridCell cellContainer = GetCell(rowIndex, columnIndex);
                if (cellContainer == null)
                {
                    // We encountered virtualization, no more cells
                    break;
                }

                DRT.Assert(cellContainer.Content is ContentPresenter, string.Format("The cell for column {0} has the wrong content; it is of type {1} instead of ContentPresenter", columnIndex, cellContainer.Content.GetType()));
                ContentPresenter cp = (ContentPresenter)cellContainer.Content;

                DRT.Assert(VisualTreeHelper.GetChildrenCount(cp) > 0, "ContentPresenter should have at least one child.");
                DependencyObject cpChild = VisualTreeHelper.GetChild(cp, 0);
                DRT.Assert(cpChild is TextBlock, string.Format("The cell's ContentPresenter for column {0} has the wrong child type; it is of type {1} instead of TextBlock", columnIndex, cpChild.GetType()));
                TextBlock textBlock = (TextBlock)cpChild;
                string cellValue = textBlock.Text;
                Person person = _people[rowIndex];
                string fullName = person.LastName + ", " + person.FirstName + " " + person.MiddleName;

                DRT.Assert(cellValue == fullName, String.Format("Value of TextBlock.Text for Column {0} (\"{1}\") does not match \"{2}\" (row = {3})", columnIndex, cellValue, fullName, rowIndex));
            }
        }

        private void ValidateCheckBoxColumn(int columnIndex, string propertyName)
        {
            PropertyInfo property = typeof(Person).GetProperty(propertyName);

            if (property != null)
            {
                // Validate that the current binding generated a valid string
                for (int rowIndex = 0; rowIndex < _people.Count; rowIndex++)
                {
                    DataGridCell cellContainer = GetCell(rowIndex, columnIndex);
                    if (cellContainer == null)
                    {
                        // We encountered virtualization, no more cells
                        break;
                    }
                    DRT.Assert(cellContainer.Content is CheckBox, string.Format("The cell for column {0} has the wrong content; it is of type {1} instead of CheckBox", columnIndex, cellContainer.Content.GetType()));
                    CheckBox checkBox = (CheckBox)cellContainer.Content;
                    bool cellValue = checkBox.IsChecked.Value;
                    object propertyValue = property.GetValue(_people[rowIndex], null);

                    bool dataValue = (bool)propertyValue;

                    DRT.Assert(cellValue == dataValue, String.Format("Value of TextBlock.Text for Column {0} (\"{1}\") does not match \"{2}\" (row = {3})", columnIndex, cellValue, dataValue, rowIndex));
                }
            }
        }

        private void ValidateAutoGeneratedColumn(string propertyName)
        {
            int index = FindAutoGeneratedColumn(propertyName);
            DRT.Assert(index >= 0, "Column not found for property " + propertyName);
            DataGridColumn column = _testGrid.Columns[index];

            DRT.Assert(column.IsAutoGenerated, "IsAutoGenerated flag is not set for an auto generated column");

            if (column is DataGridTextColumn)
            {
                ValidateTextColumn(index, propertyName);
            }
            else if (column is DataGridCheckBoxColumn)
            {
                ValidateCheckBoxColumn(index, propertyName);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private int FindAutoGeneratedColumn(string propertyName)
        {
            for (int i = 0; i < _testGrid.Columns.Count; i++)
            {
                if (_testGrid.Columns[i].Header.ToString() == propertyName)
                {
                    return i;
                }
            }
            return -1;
        }

        #endregion

        #region Tests

        private const int Toggle_Binding_ColumnIndex = 1;

        private void Toggle_Binding_1()
        {
            ValidateTextColumn(Toggle_Binding_ColumnIndex, "LastName");

            _originalBinding = ((DataGridBoundColumn)_testGrid.Columns[Toggle_Binding_ColumnIndex]).Binding;
            _alternateBinding = new Binding("FirstName");
            ((DataGridBoundColumn)_testGrid.Columns[Toggle_Binding_ColumnIndex]).Binding = _alternateBinding;
        }

        private void Toggle_Binding_2()
        {
            // Validate that the new binding generated a valid string
            ValidateTextColumn(Toggle_Binding_ColumnIndex, "FirstName");
            ((DataGridBoundColumn)_testGrid.Columns[Toggle_Binding_ColumnIndex]).Binding = _originalBinding;
        }

        private void Toggle_Binding_3()
        {
            // Validate that the original binding again generated a valid string
            ValidateTextColumn(Toggle_Binding_ColumnIndex, "LastName");
        }

        private void Toggle_IsEditing()
        {
            DataGridCell cell = GetCell(0, 0);
            DRT.Assert(cell.Content is TextBlock, "Cell content should be a TextBlock.");

            cell.IsEditing = true;
            DRT.Assert(cell.Content is TextBox, "Cell content did not change to a TextBox.");

            cell.IsEditing = false;
            DRT.Assert(cell.Content is TextBlock, "Cell content should change back to a TextBlock.");
        }

        private void Toggle_CellStyle()
        {
            DataGridCell cell = GetCell(0, 0);
            DRT.Assert(cell.Style == null, "Cell.Style should be null.");

            _testGrid.SetResourceReference(DataGrid.CellStyleProperty, "TestCellStyle");
            Style testStyle = _testGrid.CellStyle;
            DRT.Assert(testStyle != null, "Unable to find the CellStyle test style.");
            DRT.Assert(cell.Style == testStyle, "CellStyle did not propagate down to the cell.");

            _testGrid.ClearValue(DataGrid.CellStyleProperty);
            DRT.Assert(cell.Style == null, "Cell.Style did not change back to null.");
        }

        private void Toggle_RowBackground()
        {
            DataGridRow row = (DataGridRow)_testGrid.ItemContainerGenerator.ContainerFromIndex(0);
            DRT.Assert(row != null, "Unable to retrieve first row.");
            DRT.Assert(row.Background == SystemColors.WindowBrush, "Row Background should default to the system color.");

            _testGrid.RowBackground = Brushes.Red;
            DRT.Assert(row.Background == Brushes.Red, "Row Background did not change to red.");

            _testGrid.ClearValue(DataGrid.RowBackgroundProperty);
            DRT.Assert(row.Background == SystemColors.WindowBrush, "Row Background did not change back to the system color.");
        }

        private void Toggle_AlternatingRowBackground()
        {
            DataGridRow row0 = (DataGridRow)_testGrid.ItemContainerGenerator.ContainerFromIndex(0);
            DataGridRow row1 = (DataGridRow)_testGrid.ItemContainerGenerator.ContainerFromIndex(1);
            DRT.Assert(row0 != null, "Unable to retrieve first row.");
            DRT.Assert(row1 != null, "Unable to retrieve second row.");
            DRT.Assert(_testGrid.AlternationCount == 0, "AlternationCount should have the default value of 0.");
            DRT.Assert(row0.AlternationIndex == 0, "Row0 AlternationIndex is not equal to 0");
            DRT.Assert(row1.AlternationIndex == 0, "Row1 AlternationIndex is not equal to 0");
            DRT.Assert(row0.Background == SystemColors.WindowBrush, "Row0 Background should default to the system color.");
            DRT.Assert(row1.Background == SystemColors.WindowBrush, "Row1 Background should default to the system color.");

            _testGrid.AlternatingRowBackground = Brushes.Red;
            DRT.Assert(_testGrid.AlternationCount == 2, "AlternationCount should have coerced to 2.");
            DRT.Assert(row0.AlternationIndex == 0, "Row0 AlternationIndex is not equal to 0");
            DRT.Assert(row1.AlternationIndex == 1, "Row1 AlternationIndex is not equal to 1");
            DRT.Assert(row0.Background == SystemColors.WindowBrush, "Row0 Background should remain the system color.");
            DRT.Assert(row1.Background == Brushes.Red, "Row1 Background did not change to red.");

            _testGrid.ClearValue(DataGrid.AlternatingRowBackgroundProperty);
            DRT.Assert(_testGrid.AlternationCount == 0, "AlternationCount did not change back to 0.");
            DRT.Assert(row0.AlternationIndex == 0, "Row0 AlternationIndex did not change back to 0");
            DRT.Assert(row1.AlternationIndex == 0, "Row1 AlternationIndex did not change back to 0");
            DRT.Assert(row0.Background == SystemColors.WindowBrush, "Row0 Background did not change back to the system color.");
            DRT.Assert(row1.Background == SystemColors.WindowBrush, "Row1 Background did not change back to the system color.");
        }

        private void AlternatingBackgroundStyle()
        {
            // Set alternation count using a Style verify it works.
            _testGrid.ItemContainerStyle = (Style)_testGrid.FindResource("alternatingRowStyle");
            _testGrid.AlternationCount = 3;

            DrtBase.WaitForPriority(DispatcherPriority.Background);

            DataGridRow row0 = (DataGridRow)_testGrid.ItemContainerGenerator.ContainerFromIndex(0);
            DataGridRow row1 = (DataGridRow)_testGrid.ItemContainerGenerator.ContainerFromIndex(1);
            DataGridRow row2 = (DataGridRow)_testGrid.ItemContainerGenerator.ContainerFromIndex(2);

            DRT.Assert(row0.Background == Brushes.LightBlue, "Style didn't properly set the Row background based on the Alternation Index");
            DRT.Assert(row1.Background == Brushes.LightGoldenrodYellow, "Style didn't properly set the Row background based on the Alternation Index");
            DRT.Assert(row2.Background == Brushes.LightGreen, "Style didn't properly set the Row background based on the Alternation Index");


            _testGrid.AlternationCount = 0;
            _testGrid.ClearValue(ItemsControl.ItemContainerStyleProperty);
        }


        private void Toggle_Columns()
        {
            // Remove a column from the list and add it back; ensure the column headers are correct.

            DataGridColumnHeader columnHeader;
            DataGridColumn column0 = GetColumn(0);
            DataGridColumn column1 = GetColumn(1);

            _testGrid.Columns.Remove(column0);
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            columnHeader = GetColumnHeader(0);
            DRT.Assert(GetColumn(columnHeader) == column1, "Column 1 didn't move to take Column 0's place when column 0 was removed from the list");

            _testGrid.Columns.Insert(0, column0);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            columnHeader = GetColumnHeader(0);
            DRT.Assert(GetColumn(columnHeader) == column0, "The column header for column 0 wasn't properly updated when column 0 was added back to the list");
        }

        private void Toggle_ColumnHeader()
        {
            DataGridColumnHeadersPresenter  presenter = GetColumnHeadersPresenter(_testGrid);
            DataGridColumnHeader            columnHeader = GetColumnHeader(0);
            DataGridColumn                  column = GetColumn(0);
            DateTime                        header = new DateTime(2008, 1, 1);

            DRT.Assert(column == GetColumn(columnHeader), "ColumnHeader object must point to the column that generated it");

            // Change the column header
            column.Header = header;

            // Ensure that our new column header has the new header object as its data item.
            DRT.Assert((DateTime)presenter.ItemContainerGenerator.ItemFromContainer(columnHeader) == header, "ColumnHeader should be displaying our new header object");
        }

        private void Toggle_ColumnHeaderTemplate()
        {
            DataGridColumn column;
            DataGridColumnHeader columnHeader;
            DataTemplate template;

            // Column at index 0 was given a template in xaml
            columnHeader = GetColumnHeader(0);
            DRT.Assert(columnHeader.ContentTemplate != null, "Column header template should not be null");
            template = columnHeader.ContentTemplate;

            // Column at index 1 has no template
            columnHeader = GetColumnHeader(1);
            DRT.Assert(columnHeader.ContentTemplate == null, "Column header template did not get cleared");

            // Specify a template on column 1; ensure it percolates down.
            column = GetColumn(1);
            column.HeaderTemplate = template;
            DRT.Assert(columnHeader.ContentTemplate == column.HeaderTemplate, "Column.HeaderTemplate did not get sent down to the ColumnHeader");


            column.ClearValue(DataGridColumn.HeaderTemplateProperty);
            DRT.Assert(columnHeader.ContentTemplate == null, "Column header template should have been cleared");


            //Specify a template on Column 0; Column-specified templates take precedence, so ensure the template has changed.
            column = GetColumn(0);
            columnHeader = GetColumnHeader(0);
            column.HeaderTemplate = new DataTemplate();
            DRT.Assert(columnHeader.ContentTemplate == column.HeaderTemplate, "Column.HeaderTemplate did not get sent down to the ColumnHeader");

            column.ClearValue(DataGridColumn.HeaderTemplateProperty);
            DRT.Assert(columnHeader.ContentTemplate == null, "Column header template should have been cleared");

            column.HeaderTemplate = template;
        }


        private void Toggle_ColumnHeaderStyle()
        {
            DataGridColumn column;
            DataGridColumnHeader columnHeader;
            Style gridStyle = new Style();
            Style columnStyle = new Style();
            Style localStyle = new Style();

            columnHeader = GetColumnHeader(0);
            column = GetColumn(0);

            columnHeader.Style = localStyle;
            DRT.Assert(columnHeader.Style == localStyle, "ColumnHeader.Style was not set locally");

            column.HeaderStyle = columnStyle;
            DRT.Assert(column.HeaderStyle == columnStyle, "Column.HeaderStyle was not set locally");
            DRT.Assert(columnHeader.Style == localStyle, "Column.HeaderStyle should not override a locally set Style");

            _testGrid.ColumnHeaderStyle = gridStyle;
            DRT.Assert(_testGrid.ColumnHeaderStyle == gridStyle, "DataGrid.ColumnHeaderStyle was not set locally");
            DRT.Assert(column.HeaderStyle == columnStyle, "Column.HeaderStyle should not be overriden by DataGrid.ColumnHeaderStyle");
            DRT.Assert(columnHeader.Style == localStyle, "DataGrid.ColumnHeaderStyle should not override a locally set Style");

            columnHeader.ClearValue(DataGridColumnHeader.StyleProperty);
            DRT.Assert(columnHeader.Style == columnStyle, "ColumnHeader.Style should be transferred from Column.HeaderStyle");

            column.ClearValue(DataGridColumn.HeaderStyleProperty);
            DRT.Assert(column.HeaderStyle == gridStyle, "Column.HeaderStyle should be transferred from DataGrid.ColumnHeaderStyle");
            DRT.Assert(columnHeader.Style == gridStyle, "ColumnHeader.Style should be transferred from DataGrid.ColumnHeaderStyle");


            columnHeader = GetColumnHeader(1);
            DRT.Assert(columnHeader.Style == gridStyle, "DataGrid.ColumnHeaderStyle did not affect all columns");

            _testGrid.ClearValue(DataGrid.ColumnHeaderStyleProperty);
            DRT.Assert(columnHeader.Style != gridStyle, "Clearing DataGrid.ColumnHeaderStyle did not affect all columns");
        }

        private void Style_EditingElementStyle()
        {
            DataGridComboBoxColumn column = (DataGridComboBoxColumn)_testGrid.Columns[4];
            IEnumerable origItemsSource = column.ItemsSource;
            column.ClearValue(DataGridComboBoxColumn.ItemsSourceProperty);

            Style editingElementStyle = (Style)_testGrid.FindResource("cakeEditingElementStyle");
            DRT.Assert(editingElementStyle != null, "Couldn't find cakeEditingElementStyle");
            column.EditingElementStyle = editingElementStyle;
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            DataGridCell cell = GetCell(0, 4);
            cell.IsEditing = true;
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            ComboBox comboBox = (ComboBox)cell.Content;
            DRT.Assert(((string)comboBox.Items[0]) == "Strawberry", "New ItemsSource from EditingElementStyle did not apply.");

            cell.IsEditing = false;
            column.ClearValue(DataGridComboBoxColumn.EditingElementStyleProperty);
            column.ItemsSource = origItemsSource;
        }

        private const double ColumnWidthFuzziness = 0.5; // Allowing calculated values to be 0.5 pixels off
        private static bool AreClose(double n1, double n2, double epsilon)
        {
            return (Math.Abs(n1 - n2) < epsilon);
        }

        private void TestColumnWidths()
        {
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            DataGridColumnHeader columnHeader130 = GetColumnHeader(0);
            DataGridColumnHeader columnHeaderAuto = GetColumnHeader(1);
            DataGridColumnHeader columnHeaderSizeToCells = GetColumnHeader(3);
            DataGridColumnHeader columnHeaderStar = GetColumnHeader(4);
            DataGridColumnHeader columnHeaderSizeToHeader = GetColumnHeader(5);

            DataGridCell cell130 = GetCell(0, 0);
            DataGridCell cellAuto = GetCell(0, 1);
            DataGridCell cellSizeToCells = GetCell(0, 3);
            DataGridCell cellStar = GetCell(0, 4);
            DataGridCell cellSizeToHeader = GetCell(0, 5);

            // Verify the initial sizes
            DRT.Assert(AreClose(cell130.RenderSize.Width, 130.0, ColumnWidthFuzziness), "Cell wasn't 130 pixels at start");
            DRT.Assert(AreClose(columnHeader130.RenderSize.Width, 130.0, ColumnWidthFuzziness), "Header wasn't 130 pixels at start");

            double autoWidth = cellAuto.RenderSize.Width;
            double starWidth = cellStar.RenderSize.Width;
            double sizeToHeaderWidth = columnHeaderSizeToHeader.RenderSize.Width;

            // Insert a data item with long strings to change the column widths
            _people.Insert(0, new Person("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"));
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            DRT.Assert(AreClose(cell130.RenderSize.Width, 130.0, ColumnWidthFuzziness), "Cell wasn't 130 pixels");
            DRT.Assert(AreClose(columnHeader130.RenderSize.Width, 130.0, ColumnWidthFuzziness), "Header wasn't 130 pixels");

            DRT.Assert(AreClose(columnHeaderSizeToHeader.RenderSize.Width, sizeToHeaderWidth, ColumnWidthFuzziness), "SizeToHeader column header resized");
            DRT.Assert(AreClose(cellSizeToHeader.RenderSize.Width, sizeToHeaderWidth, ColumnWidthFuzziness), "SizeToHeader cell resized");

            DRT.Assert(autoWidth < cellAuto.RenderSize.Width, "Auto cell did not increase in size");
            DRT.Assert(autoWidth < columnHeaderAuto.RenderSize.Width, "Auto header did not increase in size");

            DRT.Assert(starWidth > cellStar.RenderSize.Width, "Star cell did not decrease in size");
            DRT.Assert(starWidth > columnHeaderStar.RenderSize.Width, "Star header did not decrease in size");

            // Remove the added data item
            _people.RemoveAt(0);
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            DRT.Assert(AreClose(cell130.RenderSize.Width, 130.0, ColumnWidthFuzziness), "Cell wasn't 130 pixels at end");
            DRT.Assert(AreClose(columnHeader130.RenderSize.Width, 130.0, ColumnWidthFuzziness), "Header wasn't 130 pixels at end");

            DRT.Assert(AreClose(columnHeaderSizeToHeader.RenderSize.Width, sizeToHeaderWidth, ColumnWidthFuzziness), "SizeToHeader column header did not return to the original size");
            DRT.Assert(AreClose(cellSizeToHeader.RenderSize.Width, sizeToHeaderWidth, ColumnWidthFuzziness), "SizeToHeader cell did not return to the original size");
        }

        private void TestColumnMinMaxWidth()
        {
            DataGridColumn column = GetColumn(3); // Checkbox column -- should be default min width of 20
            PropertyMetadata metadata = DataGridColumn.MinWidthProperty.GetMetadata(column);
            DRT.Assert(column.ActualWidth == column.MinWidth && column.ActualWidth == (double)metadata.DefaultValue, "Width of the checkbox column isn't the default min width");

            // Set a width too small and ensure it's clamped.
            column = GetColumn(0);
            metadata = DataGridColumn.MinWidthProperty.GetMetadata(column);

            column.MinWidth = 30d;
            _testGrid.MinColumnWidth = 40d;
            column.Width = new DataGridLength(10d);

            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(column.ActualWidth == 30d, "Column didn't honor min width");

            column.ClearValue(DataGridColumn.MinWidthProperty);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(column.ActualWidth == 40d, "Column didn't transfer min width");

            _testGrid.ClearValue(DataGrid.MinColumnWidthProperty);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(column.ActualWidth == column.MinWidth && column.ActualWidth == (double)metadata.DefaultValue, "Column didn't transfer cleared min width");

            DataGridColumn starColumn = GetStarColumn();
            DataGridLength originalStarWidth = starColumn.Width;
            starColumn.Width = 40d;
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            metadata = DataGridColumn.MaxWidthProperty.GetMetadata(column);
            // Set a width too large
            column.MaxWidth = 50d;
            _testGrid.MaxColumnWidth = 40d;
            column.Width = new DataGridLength(80d);

            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(column.ActualWidth == 50d, "Column didn't honor max width");

            column.ClearValue(DataGridColumn.MaxWidthProperty);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(column.ActualWidth == 40d, "Column didn't transfer max width");

            _testGrid.ClearValue(DataGrid.MaxColumnWidthProperty);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(column.ActualWidth == 80d && column.MaxWidth == (double)metadata.DefaultValue, "Column didn't transfer cleared max width");

            starColumn.Width = originalStarWidth;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
        }

        private void TestRowHeaders()
        {
            DataGridRow row = GetRow(0);
            DataGridRowHeader header = GetRowHeader(row);
            DRT.Assert((int)header.Content == 0, "Row Header content property incorrect");
        }

        private void TestRowHeaderStyle()
        {
            DataGridRow row;
            DataGridRowHeader rowHeader;
            Style gridStyle = new Style();
            Style rowStyle = new Style();
            Style localStyle = new Style();

            rowHeader = GetRowHeader(0);
            row = GetRow(0);

            var oldGridStyle = _testGrid.RowHeaderStyle;
            _testGrid.ClearValue(DataGrid.RowHeaderStyleProperty);

            DRT.Assert(rowHeader.Style == null && row.HeaderStyle == null && _testGrid.RowHeaderStyle == null, "Row Style properties should be null");

            rowHeader.Style = localStyle;
            DRT.Assert(rowHeader.Style == localStyle, "RowHeader.Style was not set locally");

            row.HeaderStyle = rowStyle;
            DRT.Assert(row.HeaderStyle == rowStyle, "Row.HeaderStyle was not set locally");
            DRT.Assert(rowHeader.Style == localStyle, "Row.HeaderStyle should not override a locally set Style");

            _testGrid.RowHeaderStyle = gridStyle;
            DRT.Assert(_testGrid.RowHeaderStyle == gridStyle, "DataGrid.RowHeaderStyle was not set locally");
            DRT.Assert(row.HeaderStyle == rowStyle, "Row.HeaderStyle should not be overriden by DataGrid.RowHeaderStyle");
            DRT.Assert(rowHeader.Style == localStyle, "DataGrid.RowHeaderStyle should not override a locally set Style");

            rowHeader.ClearValue(DataGridRowHeader.StyleProperty);
            DRT.Assert(rowHeader.Style == rowStyle, "RowHeader.Style should be transferred from Row.HeaderStyle");

            row.ClearValue(DataGridRow.HeaderStyleProperty);
            DRT.Assert(row.HeaderStyle == gridStyle, "Row.HeaderStyle should be transferred from DataGrid.RowHeaderStyle");
            DRT.Assert(rowHeader.Style == gridStyle, "RowHeader.Style should be transferred from DataGrid.RowHeaderStyle");

            rowHeader = GetRowHeader(1);
            DRT.Assert(rowHeader.Style == gridStyle, "DataGrid.RowHeaderStyle did not affect all rows");

            _testGrid.RowHeaderStyle = oldGridStyle;
            DRT.Assert(rowHeader.Style != gridStyle, "Clearing DataGrid.RowHeaderStyle did not affect all rows");
        }

        private void TestRowHeight()
        {
            var cellsPresenter = FindChild<DataGridCellsPresenter>(GetRow(0));

            DRT.Assert(double.IsNaN(cellsPresenter.Height) && double.IsNaN(_testGrid.RowHeight), "CellsPresenter.Height & DataGrid.RowHeight should start this test with Auto values.");

            var startHeight = cellsPresenter.ActualHeight;

            _testGrid.RowHeight = 50d;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(cellsPresenter.ActualHeight == 50d, "CellsPresenter.Height should get transfered value from DataGrid.RowHeight.");

            cellsPresenter.Height = 100d;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(cellsPresenter.ActualHeight == 100d, "CellsPresenter.Height should be higher presedence than DataGrid.RowHeight");

            cellsPresenter.MinHeight = 200d;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(cellsPresenter.ActualHeight == 200d, "CellsPresenter.MinHeight is not being respected.");

            _testGrid.MinRowHeight = 300d;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(cellsPresenter.ActualHeight == 200d, "DataGrid.MinRowHeight should not override DataGridCellsPresenter.MinHeight if both are set.");

            cellsPresenter.ClearValue(DataGridRow.MinHeightProperty);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(cellsPresenter.ActualHeight == 300d, "DataGrid.MinRowHeight is not being transferred to the DataGridRow.");

            _testGrid.ClearValue(DataGrid.MinRowHeightProperty);
            cellsPresenter.ClearValue(DataGridRow.HeightProperty);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(cellsPresenter.ActualHeight == 50d, "CellsPresenter.Height should get transfered value from DataGrid.RowHeight after local value is cleared.");

            _testGrid.ClearValue(DataGrid.RowHeightProperty);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(cellsPresenter.ActualHeight == startHeight, "CellsPresenter.Height should get transfered value from DataGrid.RowHeight after local value is cleared.");
        }


    

        private void EditMode_CurrentCell()
        {
            DataGridCell testCell = GetCell(0, 0);
            DRT.Assert(testCell != null, "Unable to find first cell.");
            if (testCell.IsKeyboardFocusWithin || !_testGrid.IsKeyboardFocusWithin)
            {
                _testGrid.Focus();
            }
            DRT.Assert(!testCell.IsKeyboardFocusWithin, "First cell already should start unfocused.");

            DRT.Assert(!_testGrid.CurrentCell.IsValid, "CurrentCell should start invalid.");
            DRT.Assert(_testGrid.CurrentItem == null, "CurrentItem should start null.");
            DRT.Assert(_testGrid.CurrentColumn == null, "CurrentColumn should start null.");

            _testGrid.CurrentCell = new DataGridCellInfo(_people[0], _testGrid.Columns[0]);
            DRT.Assert(_testGrid.CurrentItem == _people[0], "CurrentItem is not equal to the first data item.");
            DRT.Assert(_testGrid.CurrentColumn == _testGrid.Columns[0], "CurrentColumn is not equal to the first column.");
            DRT.Assert(testCell.IsKeyboardFocusWithin, "First cell should have become focused.");

            _testGrid.CurrentColumn = _testGrid.Columns[1];
            DRT.Assert(_testGrid.CurrentCell.Column == _testGrid.Columns[1], "CurrentCell.Column did not update correctly.");

            DataGridCell testCell2 = GetCell(0, 1);
            DRT.Assert(testCell2 != null, "Unable to find second cell.");
            DRT.Assert(testCell2.IsKeyboardFocusWithin, "Second cell should have become focused.");

            DRT.Assert(testCell.Focus(), "Should be able to focus first cell.");
            DRT.Assert(_testGrid.CurrentColumn == _testGrid.Columns[0], "CurrentColumn should go back to the first column.");
            DRT.Assert(_testGrid.CurrentItem == _people[0], "CurrentItem should have stayed on the first item.");
            DRT.Assert(_testGrid.CurrentCell.Column == _testGrid.Columns[0], "CurrentCell.Column should go back to the first column.");
            DRT.Assert(_testGrid.CurrentCell.Item == _people[0], "CurrentCell.Item should have stayed on the first item.");

            _testGrid.CurrentItem = null;
            _testGrid.CurrentColumn = null;
            DRT.Assert(!_testGrid.CurrentCell.IsValid, "CurrentCell should be invalid.");

            _testGrid.CurrentItem = _testGrid.Items[0];
            DRT.Assert(!_testGrid.CurrentCell.IsValid, "CurrentCell should be invalid.");
            DRT.Assert(_testGrid.CurrentItem == _testGrid.Items[0], "CurrentItem reverted.");
            DRT.Assert(_testGrid.CurrentColumn == null, "CurrentColumn should be null.");

            _testGrid.CurrentColumn = _testGrid.Columns[0];
            DRT.Assert(_testGrid.CurrentCell.IsValid, "CurrentCell should be valid.");
            DRT.Assert(_testGrid.CurrentItem == _testGrid.Items[0], "CurrentItem reverted.");
            DRT.Assert(_testGrid.CurrentColumn == _testGrid.Columns[0], "CurrentColumn reverted.");

            _testGrid.CurrentItem = null;
            _testGrid.CurrentColumn = null;

            Keyboard.Focus(_testGrid);
        }

        private void EditMode_Input()
        {
            string origString = _people[0].FirstName;

            DataGridCell testCell = GetCell(0, 0);
            DataGridCell nextCell = GetCell(1, 0);
            DRT.Assert(testCell != null, "Unable to find first cell.");
            DRT.Assert(nextCell != null, "Unable to find first cell in the second row.");
            DRT.Assert(!testCell.IsKeyboardFocusWithin, "First cell should be unfocused.");

            DRT.MoveMouse(testCell, 0.1, 0.5);
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(testCell.IsKeyboardFocusWithin, "First cell should be focused.");
            DRT.Assert(!testCell.IsEditing, "First cell should not be in edit mode.");

            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(testCell.IsEditing, "First cell should be in edit mode.");

            DRT.PressKey(Key.A);
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            DRT.PressKey(Key.Escape);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(_people[0].FirstName == origString, "FirstName should not have changed.");
            DRT.Assert(!testCell.IsEditing, "First cell should not be in edit mode following ESC.");

            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(testCell.IsEditing, "First cell should be in edit mode.");

            DRT.PressKey(Key.A);
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            DRT.PressKey(Key.Enter);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(_people[0].FirstName != origString, "FirstName should have changed.");
            DRT.Assert(_people[0].FirstName[0] == 'a', "FirstName should start with an 'a.'");
            DRT.Assert(!testCell.IsEditing, "First cell should not be in edit mode following ENTER.");
            DRT.Assert(nextCell.IsKeyboardFocusWithin, "The next row's cell did not get focus.");
            DRT.Assert(!nextCell.IsEditing, "The next row's cell should not enter edit mode.");

            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(testCell.IsEditing, "First cell should be in edit mode.");

            DRT.PressKey(Key.Delete);
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            DRT.PressKey(Key.Enter);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(_people[0].FirstName == origString, "FirstName should have returned to the original string.");
            DRT.Assert(!testCell.IsEditing, "First cell should not be in edit mode following ENTER.");

            Keyboard.Focus(_testGrid);
        }

        private void EditMode_Events()
        {
            _committingEdit = 0;
            _cancelingEdit = 0;
            _beginningEdit = 0;
            _preparingCellForEdit = 0;

            _testGrid.CellEditEnding += new EventHandler<DataGridCellEditEndingEventArgs>(OnCellEditEnding);
            _testGrid.RowEditEnding += new EventHandler<DataGridRowEditEndingEventArgs>(OnRowEditEnding);

            //_testGrid.CommittingEdit += new EventHandler<DataGridEndingEditEventArgs>(OnCommittingEdit);
            //_testGrid.CancelingEdit += new EventHandler<DataGridEndingEditEventArgs>(OnCancelingEdit);
            _testGrid.BeginningEdit += new EventHandler<DataGridBeginningEditEventArgs>(OnBeginningEdit);
            _testGrid.PreparingCellForEdit += new EventHandler<DataGridPreparingCellForEditEventArgs>(OnPreparingCellForEdit);

            _editingColumn = _testGrid.Columns[0];
            _editingRow = GetRow(0);
            _commitColumn = _editingColumn;
            _commitRow = _editingRow;

            DataGridCell testCell = GetCell(0, 0);
            DRT.Assert(testCell != null, "Unable to find first cell.");
            DRT.Assert(!testCell.IsKeyboardFocusWithin, "First cell should be unfocused.");

            DRT.MoveMouse(testCell, 0.1, 0.5);
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(testCell.IsKeyboardFocusWithin, "First cell should be focused.");
            DRT.Assert(!testCell.IsEditing, "First cell should not be in edit mode.");

            DRT.PressKey(Key.F2);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(testCell.IsEditing, "First cell should be in edit mode.");
            DRT.Assert(_beginningEdit == 1, "BeginningEdit should have been called.");
            DRT.Assert(_preparingCellForEdit == 1, "PreparingCellForEdit should have been called.");
            DRT.Assert(_committingEdit == 0, "CommittingEdit should not have been called.");
            DRT.Assert(_cancelingEdit == 0, "CancelingEdit should not have been called.");

            DRT.PressKey(Key.Escape);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(!testCell.IsEditing, "First cell should not be in edit mode following ESC.");
            DRT.Assert(_beginningEdit == 1, "BeginningEdit should not have been called.");
            DRT.Assert(_preparingCellForEdit == 1, "PreparingCellForEdit should not have been called.");
            DRT.Assert(_committingEdit == 0, "CommittingEdit should not have been called.");
            DRT.Assert(_cancelingEdit == 1, "CancelingEdit should have been called.");

            DRT.PressKey(Key.F2);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(testCell.IsEditing, "First cell should be in edit mode.");
            DRT.Assert(_beginningEdit == 2, "BeginningEdit should have been called.");
            DRT.Assert(_preparingCellForEdit == 2, "PreparingCellForEdit should have been called.");
            DRT.Assert(_committingEdit == 0, "CommittingEdit should not have been called.");
            DRT.Assert(_cancelingEdit == 1, "CancelingEdit should not have been called.");

            _editingRow = GetRow(1);
            DRT.PressKey(Key.Enter);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(!testCell.IsEditing, "First cell should not be in edit mode following ESC.");
            DRT.Assert(_beginningEdit == 2, "BeginningEdit should not have been called on the next row's cell.");
            DRT.Assert(_preparingCellForEdit == 2, "PreparingCellForEdit not should not have been called.");
            DRT.Assert(_committingEdit == 2, "CommittingEdit should have been called twice - once for cell and once for row.");
            DRT.Assert(_cancelingEdit == 1, "CancelingEdit should not have been called.");

            _testGrid.CellEditEnding -= new EventHandler<DataGridCellEditEndingEventArgs>(OnCellEditEnding);
            _testGrid.RowEditEnding -= new EventHandler<DataGridRowEditEndingEventArgs>(OnRowEditEnding);

            //_testGrid.CommittingEdit -= new EventHandler<DataGridEndingEditEventArgs>(OnCommittingEdit);
            //_testGrid.CancelingEdit -= new EventHandler<DataGridEndingEditEventArgs>(OnCancelingEdit);
            _testGrid.BeginningEdit -= new EventHandler<DataGridBeginningEditEventArgs>(OnBeginningEdit);
            _testGrid.PreparingCellForEdit -= new EventHandler<DataGridPreparingCellForEditEventArgs>(OnPreparingCellForEdit);

            _editingColumn = null;
            _editingRow = null;
            _commitRow = null;
            _commitColumn = null;

            _testGrid.CancelEdit();
        }

        private void OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                _committingEdit++;
                DRT.Assert(e.Row == _commitRow, "DataGridCellEditEndingEventArgs.Row should be the same as the row being edited.");
                DRT.Assert(e.Column == _commitColumn, "DataGridCellEditEndingEventArgs.Column should be the same as the column being edited.");
            }
            else if (e.EditAction == DataGridEditAction.Cancel)
            {
                _cancelingEdit++;
                DRT.Assert(e.Row == _editingRow, "DataGridCellEditEndingEventArgs.Row should be the same as the row being edited.");
                DRT.Assert(e.Column == _editingColumn, "DataGridCellEditEndingEventArgs.Column should be the same as the column being edited.");
            }
        }

        private void OnRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                _committingEdit++;
                DRT.Assert(e.Row == _commitRow, "DataGridRowEditEndingEventArgs.Row should be the same as the row being edited.");
            }
            else if (e.EditAction == DataGridEditAction.Cancel)
            {
                _cancelingEdit++;
                DRT.Assert(e.Row == _editingRow, "DataGridRowEditEndingEventArgs.Row should be the same as the row being edited.");
            }
        }

        private void OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            _beginningEdit++;
            DRT.Assert(e.Row == _editingRow, "DataGridBeginningEditEventArgs.Row should be the same as the row being edited.");
            DRT.Assert(e.Column == _editingColumn, "DataGridBeginningEditEventArgs.Column should be the same as the column being edited.");
        }

        private void OnPreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            _preparingCellForEdit++;
            DRT.Assert(e.Row == _editingRow, "DataGridPreparingCellForEditEventArgs.Row should be the same as the row being edited.");
            DRT.Assert(e.Column == _editingColumn, "DataGridPreparingCellForEditEventArgs.Column should be the same as the column being edited.");
        }

        private void EditMode_Row()
        {
            // Setup a read-only collection
            Setup(1, true, false); // Set to 1 set of people
            _testGrid.ItemsSource = new ReadOnlyCollection<Person>(_people); // Make it a read-only collection
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(!_testGrid.CanUserAddRows, "CanUserAddRows should be false.");
            DRT.Assert(!_testGrid.CanUserDeleteRows, "CanUserDeleteRows should be false.");
            Person person0 = _people[0];
            Person person1 = _people[1];

            // Select something
            DataGridCell cell00a = GetCell(0, 0);
            DataGridRow row0 = GetRow(0);
            DRT.MoveMouse(cell00a, 0.5, 0.5);
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(cell00a.IsKeyboardFocusWithin, "Cell 0,0 did not get focus.");
            DRT.Assert(cell00a.IsSelected, "Cell 0,0 was not selected.");
            DRT.Assert(!row0.IsEditing, "Row 0 should not be in edit mode.");

            // Press DELETE, nothing should happen
            DRT.PressKey(Key.Delete);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(_people[0] == person0, "Should not have been allowed to delete.");

            // Setup an editable collection
            Setup(1, true, true);
            DRT.Assert(_testGrid.CanUserAddRows, "CanUserAddRows should be true.");
            DRT.Assert(_testGrid.CanUserDeleteRows, "CanUserDeleteRows should be true.");
            person0 = _people[0];
            person1 = _people[1];

            // Select something
            row0 = GetRow(0);
            cell00a = GetCell(0, 0);
            DRT.MoveMouse(cell00a, 0.5, 0.5);
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(cell00a.IsKeyboardFocusWithin, "Cell 0,0 did not get focus.");
            DRT.Assert(cell00a.IsSelected, "Cell 0,0 was not selected.");
            DRT.Assert(!row0.IsEditing, "Row 0 should not be in edit mode.");

            // Delete the row
            DRT.PressKey(Key.Delete);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DataGridCell cell00b = GetCell(0, 0);
            DRT.Assert(_people[0] == person1, "Person 0 was not deleted.");
            DRT.Assert(cell00b.IsKeyboardFocusWithin, "New cell 0,0 did not get focus.");
            DRT.Assert(cell00b.IsSelected, "New cell 0,0 was not selected.");
            DRT.Assert(cell00a != cell00b, "Cell 0,0 reference did not change.");

            // Select multiple rows
            cell00a = GetCell(0, 0);
            row0 = GetRow(0);
            DataGridCell cell10 = GetCell(1, 0);
            DRT.MoveMouse(cell10, 0.5, 0.5);
            DRT.SendKeyboardInput(Key.LeftShift, /* press = */ true);
            DRT.ClickMouse();
            DRT.SendKeyboardInput(Key.LeftShift, /* press = */ false);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(_testGrid.SelectedItems.Count == 2, "SelectedItems should be 2.");
            DRT.Assert(!row0.IsEditing, "Row 0 should not be in edit mode.");
            person0 = _people[0];
            person1 = _people[1];
            Person person2 = _people[2];
            int count = _people.Count;

            // Delete the rows
            DRT.PressKey(Key.Delete);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(_people.Count == (count - 2), "The count of items should have reduced by 2.");
            DRT.Assert(_people[0] == person2, "Person 2 should have moved to index 0.");
            DRT.Assert(_testGrid.SelectedItems.Count == 1, "SelectedItems should be 1.");
            DRT.Assert(_testGrid.SelectedItem == person2, "SelectedItem should be person 2.");

            // Edit a cell
            string origFirstName = _people[0].FirstName;
            string origLastName = _people[0].LastName;
            cell00a = GetCell(0, 0);
            row0 = GetRow(0);
            DRT.Assert(!row0.IsEditing, "Row 0 should not be in edit mode.");
            DataGridCell cell01 = GetCell(0, 1);
            DRT.MoveMouse(cell00a, 0.5, 0.5);
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.PressKey(Key.F2);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(cell00a.IsEditing, "Cell 0,0 did not enter edit mode.");
            DRT.Assert(row0.IsEditing, "Row 0 should have entered edit mode.");
            DRT.PressKey(Key.A);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.PressKey(Key.Tab);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(_people[0].FirstName == origFirstName, "FirstName should not change yet (until row is committed)");
            DRT.Assert(!cell00a.IsEditing, "Cell 0,0 should have exited edit mode.");
            DRT.Assert(cell01.IsEditing, "Cell 0,1 should have entered edit mode.");
            DRT.Assert(row0.IsEditing, "Row 0 should have stayed in edit mode.");
            DRT.PressKey(Key.B);
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            // Cancel cell and row edit
            DRT.PressKey(Key.Escape);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(!cell01.IsEditing, "Cell 0,1 should have left edit mode.");
            DRT.Assert(row0.IsEditing, "Row 0 should have stayed in edit mode.");
            DRT.Assert(_people[0].LastName == origLastName, "LastName should not have changed.");
            DRT.Assert(_people[0].FirstName == origFirstName, "FirstName should not have changed.");
            DRT.PressKey(Key.Escape);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(_people[0].FirstName == origFirstName, "FirstName should have reverted to its original string.");
            DRT.Assert(!row0.IsEditing, "Row 0 should have exited edit mode.");

            _testGrid.InitializingNewItem += new InitializingNewItemEventHandler(OnInitializingNewItem);

            // Add a new row
            int peopleCount = _people.Count;
            int itemsCount = _testGrid.Items.Count;
            DRT.Assert(itemsCount == (peopleCount + 1), "Items.Count should have one more than _people.Count due to the NewItemPlaceholder.");
            DRT.Assert(_testGrid.Items[_testGrid.Items.Count - 1] == CollectionView.NewItemPlaceholder, "Last item in Items should be the new item placeholder.");
            DRT.SendKeyboardInput(Key.LeftCtrl, /* press = */ true);
            DRT.PressKey(Key.End);
            DRT.SendKeyboardInput(Key.LeftCtrl, /* press = */ false);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DataGridCell cellE0 = GetCell(peopleCount, 0); // Should be the new item placeholder
            DRT.Assert(cellE0 != null, "Could not get cell from new item placeholder.");
            if (!cellE0.IsKeyboardFocusWithin)
            {
                DRT.MoveMouse(cellE0, 0.5, 0.5);
                DRT.ClickMouse();
                DrtBase.WaitForPriority(DispatcherPriority.Background);
                DRT.Assert(cellE0.IsKeyboardFocusWithin, "Could not move focus to cell E, 0");
            }
            DRT.PressKey(Key.F2);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(_testGrid.Items.Count == itemsCount + 1, "Items.Count should be 1 bigger because a new row was added for editing.");
            DRT.Assert(_testGrid.Items[_testGrid.Items.Count - 1] == CollectionView.NewItemPlaceholder, "Last item in Items should be the new item placeholder.");
            DRT.Assert(((DataGridRow)_testGrid.ItemContainerGenerator.ContainerFromItem(_testGrid.Items[_testGrid.Items.Count - 1])).Visibility == Visibility.Collapsed, "The NewItemPlaceholder should be collapsed.");
            DRT.Assert(_people.Count == (peopleCount + 1), "_people.Count should have increased by 1.");
            DRT.Assert(((Person)_testGrid.Items[_testGrid.Items.Count - 2]).MiddleName == "Middle Name", "New item was not initialized.");
            DRT.PressKey(Key.Enter);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(_testGrid.Items.Count == (itemsCount + 1), "Items.Count should have increased by 1.");
            DRT.Assert(_testGrid.Items[_testGrid.Items.Count - 1] == CollectionView.NewItemPlaceholder, "Last item in Items should be the new item placeholder.");
            DRT.Assert(_people.Count == (peopleCount + 1), "_people.Count should have increased by 1.");

            // Add another new item, this time with app creating the item
            _testGrid.AddingNewItem += OnAddingNewItem;

            peopleCount = _people.Count;
            itemsCount = _testGrid.Items.Count;
            DRT.Assert(itemsCount == (peopleCount + 1), "Items.Count should have one more than _people.Count due to the NewItemPlaceholder.");
            DRT.Assert(_testGrid.Items[_testGrid.Items.Count - 1] == CollectionView.NewItemPlaceholder, "Last item in Items should be the new item placeholder.");
            cellE0 = GetCell(peopleCount, 0); // Should be the new item placeholder
            DRT.Assert(cellE0 != null, "Could not get cell from new item placeholder.");
            if (!cellE0.IsKeyboardFocusWithin)
            {
                DRT.MoveMouse(cellE0, 0.5, 0.5);
                DRT.ClickMouse();
                DrtBase.WaitForPriority(DispatcherPriority.Background);
                DRT.Assert(cellE0.IsKeyboardFocusWithin, "Could not move focus to cell E, 0");
            }
            DRT.PressKey(Key.F2);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(_testGrid.Items.Count == itemsCount + 1, "Items.Count should be 1 bigger because a new row was added for editing.");
            DRT.Assert(_testGrid.Items[_testGrid.Items.Count - 1] == CollectionView.NewItemPlaceholder, "Last item in Items should be the new item placeholder.");
            DRT.Assert(_people.Count == (peopleCount + 1), "_people.Count should have increased by 1.");
            DRT.Assert(_testGrid.Items[_testGrid.Items.Count - 2] is EditablePerson, "AddingNewItem event should have been called");
            DRT.Assert(((Person)_testGrid.Items[_testGrid.Items.Count - 2]).MiddleName == "Middle Name", "New item was not initialized.");
            DRT.PressKey(Key.Escape);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.PressKey(Key.Escape);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(_people.Count == peopleCount, "_people.Count should reverted to its original amount.");
            DRT.Assert(_testGrid.Items.Count == itemsCount, "Items.Count should reverted to its original amount.");
            DRT.Assert(_testGrid.Items[_testGrid.Items.Count - 1] == CollectionView.NewItemPlaceholder, "Last item in Items should be the new item placeholder.");

            DRT.SendKeyboardInput(Key.LeftCtrl, /* press = */ true);
            DRT.PressKey(Key.Home);
            DRT.SendKeyboardInput(Key.LeftCtrl, /* press = */ false);
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            _testGrid.AddingNewItem -= OnAddingNewItem;
            _testGrid.InitializingNewItem -= new InitializingNewItemEventHandler(OnInitializingNewItem);

            Setup(true); // Restore ItemsSource
        }

        private void OnInitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            Person person = (Person)e.NewItem;
            person.MiddleName += "Middle Name"; // += to detect if this was called multiple times
        }

        private void OnAddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new EditablePerson();
        }

        private void TestDisplayIndex()
        {
            List<int> displayIndexMap = GetDisplayIndexMap(_testGrid);
            DataGridColumn column1 = GetColumn(1);
            DataGridColumn column2 = GetColumn(2);

            //
            // Test initial display index
            //

            for (int i = 0; i < _testGrid.Columns.Count; i++)
            {
                DRT.Assert(displayIndexMap[i] == i && _testGrid.Columns[i].DisplayIndex == i, "Initial DisplayIndex incorrect");
            }

            //
            // Test setting the display index locally -- other columns need to adjust.
            //

            // Larger to smaller
            column2.DisplayIndex = 0;
            DRT.Assert(displayIndexMap[0] == 2 && GetColumn(2).DisplayIndex == 0);
            DRT.Assert(displayIndexMap[1] == 0 && GetColumn(0).DisplayIndex == 1);
            DRT.Assert(displayIndexMap[2] == 1 && GetColumn(1).DisplayIndex == 2);
            column2.DisplayIndex = 2;
            DRT.Assert(displayIndexMap[0] == 0 && GetColumn(0).DisplayIndex == 0);
            DRT.Assert(displayIndexMap[1] == 1 && GetColumn(1).DisplayIndex == 1);
            DRT.Assert(displayIndexMap[2] == 2 && GetColumn(2).DisplayIndex == 2);

            // Smaller to larger
            column1.DisplayIndex = 3;
            DRT.Assert(displayIndexMap[0] == 0 && GetColumn(0).DisplayIndex == 0);
            DRT.Assert(displayIndexMap[3] == 1 && GetColumn(1).DisplayIndex == 3);
            DRT.Assert(displayIndexMap[1] == 2 && GetColumn(2).DisplayIndex == 1);
            DRT.Assert(displayIndexMap[2] == 3 && GetColumn(3).DisplayIndex == 2);

            //
            // Remove and bring back column 1 and 2 and ensure we don't get a hole in the display index.
            //

            _testGrid.Columns.RemoveAt(1);
            _testGrid.Columns.RemoveAt(1);

            // removing resets the DisplayIndex to -1, so we need to manually restore
            DRT.Assert(column1.DisplayIndex == -1);
            DRT.Assert(column2.DisplayIndex == -1);
            column1.DisplayIndex = 3;
            column2.DisplayIndex = 1;

            DRT.Assert(displayIndexMap[0] == 0 && GetColumn(0).DisplayIndex == 0, "DisplayIndex incorrect after removing a column");
            DRT.Assert(displayIndexMap[1] == 1 && GetColumn(1).DisplayIndex == 1, "DisplayIndex incorrect after removing a column");

            _testGrid.Columns.Insert(1, column2);
            _testGrid.Columns.Insert(1, column1);

            DRT.Assert(displayIndexMap[0] == 0 && GetColumn(0).DisplayIndex == 0);
            DRT.Assert(displayIndexMap[3] == 1 && GetColumn(1).DisplayIndex == 3);
            DRT.Assert(displayIndexMap[1] == 2 && GetColumn(2).DisplayIndex == 1);
            DRT.Assert(displayIndexMap[2] == 3 && GetColumn(3).DisplayIndex == 2);


            //
            //  Ensure ClearValue works -- it gives the column the same display index as its index in the collection.
            //
            column1.ClearValue(DataGridColumn.DisplayIndexProperty);
            DRT.Assert(displayIndexMap[0] == 0 && GetColumn(0).DisplayIndex == 0);
            DRT.Assert(displayIndexMap[1] == 1 && GetColumn(1).DisplayIndex == 1);
            DRT.Assert(displayIndexMap[2] == 2 && GetColumn(2).DisplayIndex == 2);

            //
            // Replace a column
            //
            DataGridTextColumn newColumn = new DataGridTextColumn();
            newColumn.DisplayIndex = 3;
            _testGrid.Columns[1] = newColumn;
            DRT.Assert(displayIndexMap[3] == 1 && GetColumn(1).DisplayIndex == 3);
            DRT.Assert(displayIndexMap[1] == 2 && GetColumn(2).DisplayIndex == 1);

            //
            // Replace a column with one with a default display index
            //
            newColumn = new DataGridTextColumn();
            _testGrid.Columns[1] = newColumn;
            DRT.Assert(displayIndexMap[1] == 1 && GetColumn(1).DisplayIndex == 1);
            DRT.Assert(displayIndexMap[2] == 2 && GetColumn(2).DisplayIndex == 2);
            _testGrid.Columns[1] = column1;

            //
            // Clear out the collection
            //
            DataGrid newGrid = new DataGrid();
            newGrid.Columns.Add(new DataGridTextColumn());
            newGrid.Columns.Add(new DataGridTextColumn());
            newGrid.Columns.Clear();
            DRT.Assert(GetDisplayIndexMap(newGrid).Count == 0);
        }

        /// <summary>
        ///     Ensure we throw on an invalid display index
        /// </summary>
        private void TestDisplayIndexValidation()
        {
            DataGridColumn column1 = GetColumn(1);
            bool foundException = false;

            //
            // Give an invalid display index to an existing column
            //
            try
            {
                column1.DisplayIndex = 8;
            }
            catch (ArgumentOutOfRangeException)
            {
                foundException = true;
            }
            finally
            {
                DRT.Assert(foundException, "Didn't catch an invalid DisplayIndex");
                foundException = false;
                column1.ClearValue(DataGridColumn.DisplayIndexProperty);
            }

            //
            // Add a column with an already invalid display index to the grid
            //
            DataGridColumn column = new DataGridTextColumn();
            column.DisplayIndex = -1;

            try
            {
                _testGrid.Columns.Add(column);
            }
            catch (ArgumentOutOfRangeException)
            {
                foundException = true;
            }
            finally
            {
                DRT.Assert(foundException, "Didn't catch an invalid DisplayIndex");
                _testGrid.Columns.Remove(column);
                foundException = false;
            }

            //
            // Replace an existing column with one with a bad display index.
            //
            try
            {
                _testGrid.Columns[1] = column;
            }
            catch (ArgumentOutOfRangeException)
            {
                foundException = true;
            }
            finally
            {
                DRT.Assert(foundException, "Didn't catch an invalid DisplayIndex");
                _testGrid.Columns[1] = column1;
            }
        }

        /// <summary>
        ///     Tests that the MultipleCopiesCollection is firing the proper events for the CellsPresenter to stay in sync with changes to columns.
        /// </summary>
        private void AddRemoveColumns()
        {
            DataGridColumnHeader columnHeader;
            List<int> displayIndexMap = GetDisplayIndexMap(_testGrid);
            DataGridTextColumn idColumn = (DataGridTextColumn)_testGrid.FindResource("idColumn");
            DataGridTextColumn middleNameColumn = (DataGridTextColumn)_testGrid.FindResource("middleNameColumn");

            //
            // Assert that the existing columns are what we expect before starting the test
            //
            ValidateTextColumn(0, "FirstName");
            ValidateTextColumn(1, "LastName");

            //
            // Insert a Template column and ensure that cells were updated properly
            //

            _testGrid.Columns.Insert(0, idColumn);

            DrtBase.WaitForPriority(DispatcherPriority.Background);

            ValidateTextColumn(0, "Id");
            columnHeader = GetColumnHeader(0);
            DRT.Assert(GetColumn(columnHeader) == idColumn);
            DRT.Assert((string)columnHeader.Content == "Id");

            ValidateTextColumn(2, "LastName");

            //
            // Move a column
            //
            _testGrid.Columns.Move(0, 1);
            DRT.Assert(displayIndexMap[0] == 1 && GetColumn(1).DisplayIndex == 0);

            DrtBase.WaitForPriority(DispatcherPriority.Background);
            ValidateTextColumn(0, "FirstName");
            ValidateTextColumn(1, "Id");
            ValidateTextColumn(2, "LastName");

            //
            // Replace a column
            //
            _testGrid.Columns[1] = middleNameColumn;


            DrtBase.WaitForPriority(DispatcherPriority.Background);
            ValidateTextColumn(1, "MiddleName");

            // scroll down and ensure that virtualized columns were updated properly
            LineDown();
            LineDown();
            DrtBase.WaitForPriority(DispatcherPriority.SystemIdle);
            ValidateTextColumn(1, "MiddleName");

            //
            // Remove a column
            //
            _testGrid.Columns.RemoveAt(1);

            DrtBase.WaitForPriority(DispatcherPriority.Background);
            LineUp();
            LineUp();
            DrtBase.WaitForPriority(DispatcherPriority.SystemIdle);
            ValidateTextColumn(0, "FirstName");
            ValidateTextColumn(1, "LastName");

            //
            // Clear columns
            //
            DataGrid newGrid = new DataGrid();
            newGrid.ItemsSource = _people;
            Grid.SetRow(newGrid, Grid.GetRow(_testGrid));
            _mainGrid.Children.Add(newGrid);
            DRT.Assert(GetDataGridOwner(idColumn) == null);
            DRT.Assert(GetDataGridOwner(middleNameColumn) == null);
            newGrid.Columns.Add(idColumn);
            newGrid.Columns.Add(middleNameColumn);

            DrtBase.WaitForPriority(DispatcherPriority.Background);

            // we're hoping no exceptions are thrown.
            newGrid.Columns.Clear();
            newGrid.Columns.Add(idColumn);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            _mainGrid.Children.Remove(newGrid);
        }

        /// <summary>
        /// Tests for sorting on datagrid
        /// </summary>
        private void TestAutoSorting()
        {
            //setting up the repeatition count of people to low so that sort makes sense
            Setup(1, true, false);

            //Click on the first name column header and check if datagrid is sorted
            //on first name in ascending order
            DataGridColumnHeader firstNameHeader = GetColumnHeader(0);
            DRT.MoveMouse(firstNameHeader, 0.2, 0.5);
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(CheckIfDataGridSortedonFirstName(ListSortDirection.Ascending),
                        "Ascending Sort didn't happen");

            //Click again on the first name column header and check if datagrid is sorted
            //on first name in descending order
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(CheckIfDataGridSortedonFirstName(ListSortDirection.Descending),
                        "Descending Sort didn't happen");

            //Try to click on header of non- IComparable column and see if exception is caught by DataGrid
            bool foundException = false;
            try
            {
                DataGridColumnHeader homePageHeader = GetColumnHeader(5);
                DRT.MoveMouse(homePageHeader, 0.2, 0.5);
                DRT.ClickMouse();
                DrtBase.WaitForPriority(DispatcherPriority.Background);
            }
            catch (InvalidOperationException)
            {
                foundException = true;
            }
            finally
            {
                DRT.Assert(!foundException, "Caught an exception for sorting on non-IComparable property");
                foundException = false;
            }

            //Click on header of last name so that descriptors on firstname gets cleared off
            DataGridColumnHeader lastNameHeader = GetColumnHeader(1);
            DRT.MoveMouse(lastNameHeader, 0.2, 0.5);
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            //Set CanUserSort of firstname to false and see if datagrid gets sorted on it or not
            DataGridColumn firstNameColumn = GetColumn(0);
            firstNameColumn.CanUserSort = false;
            DRT.MoveMouse(firstNameHeader, 0.2, 0.5);
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(!CheckIfDataGridSortedonFirstName(ListSortDirection.Ascending)
                && !CheckIfDataGridSortedonFirstName(ListSortDirection.Descending),
                "Sort happened even when the CanUserSort flag was set to false");
            firstNameColumn.CanUserSort = true;

            _testGrid.CanUserSortColumns = false;
            DRT.MoveMouse(firstNameHeader, 0.2, 0.5);
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(!CheckIfDataGridSortedonFirstName(ListSortDirection.Ascending)
                && !CheckIfDataGridSortedonFirstName(ListSortDirection.Descending),
                "Sort happened even when the CanUserSortColumns flag on DataGrid was set to false");
            _testGrid.CanUserSortColumns = true;

            DRT.MoveMouse(lastNameHeader, 0.2, 0.5);
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            //Attach a custom handler to the sorting event and see if the chain of invocation works or not.
            _testGrid.Sorting += new DataGridSortingEventHandler(OnDataGridReverseSorting);
            firstNameColumn.SortMemberPath = "FirstName";
            DRT.MoveMouse(firstNameHeader, 0.2, 0.5);
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(CheckIfDataGridSortedonFirstName(ListSortDirection.Descending),
                "Descending sort due to ReverseSort listener didn't happen");
            firstNameColumn.SortMemberPath = "";
            _testGrid.Sorting -= new DataGridSortingEventHandler(OnDataGridReverseSorting);

            //Tests on the CanUserSort property of Template column to verify its coercion
            DataGridTemplateColumn templateColumn = new DataGridTemplateColumn();
            _testGrid.Columns.Add(templateColumn);
            DRT.Assert(!templateColumn.CanUserSort, "CanUserSort of template column should be false when no SortMemberPath is set.");
            templateColumn.SortMemberPath = "LastName";
            DRT.Assert(templateColumn.CanUserSort, "CanUserSort of template column should be true when not explicitly set and SortMemberPath is set.");
            templateColumn.SortMemberPath = string.Empty;
            templateColumn.CanUserSort = false;
            templateColumn.SortMemberPath = "LastName";
            DRT.Assert(!templateColumn.CanUserSort, "CanUserSort of template column should be false if set explicitly irrespective of SortMemberPath.");
            templateColumn.SortMemberPath = string.Empty;
            templateColumn.CanUserSort = true;
            DRT.Assert(!templateColumn.CanUserSort, "CanUserSort of template column should be false when no SortMemberPath is set even if explicitly set to true.");
            templateColumn.SortMemberPath = "LastName";
            DRT.Assert(templateColumn.CanUserSort, "CanUserSort of template column should be restored to true if it was explicitly set to true, even if it was set before SortMemberPath was set.");
            _testGrid.Items.SortDescriptions.Clear();
            _testGrid.Columns.Remove(templateColumn);

            Setup(true);
        }

        /// <summary>
        /// Listerner to the sorting event of datagrid. This hanlder sorts in descending order on
        /// first click rather than the default ascending.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDataGridReverseSorting(object sender, DataGridSortingEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            dataGrid.Items.SortDescriptions.Clear();
            ListSortDirection sortDirection = ListSortDirection.Descending;
            if (e.Column.SortDirection.HasValue &&
                e.Column.SortDirection.Value == ListSortDirection.Descending)
            {
                sortDirection = ListSortDirection.Ascending;
            }

            SortDescription sortDescription = new SortDescription(e.Column.SortMemberPath, sortDirection);
            dataGrid.Items.SortDescriptions.Add(sortDescription);
            e.Column.SortDirection = sortDirection;
            e.Handled = true;
        }

        /// <summary>
        /// Verification method which checks if all the visible columns are in specfic sorted
        /// order on firstname
        /// </summary>
        /// <param name="sortDirection"></param>
        /// <returns></returns>
        private bool CheckIfDataGridSortedonFirstName(ListSortDirection sortDirection)
        {
            int index = 1;
            DataGridRow row1 = GetRow(0);
            DataGridRow row2 = GetRow(1);
            while (true)
            {
                if (row2 == null)
                {
                    break;
                }
                Person person1 = row1.Item as Person;
                Person person2 = row2.Item as Person;

                if (sortDirection == ListSortDirection.Ascending)
                {
                    if (person1.FirstName.CompareTo(person2.FirstName) > 0)
                    {
                        return false;
                    }
                }
                else
                {
                    if (person1.FirstName.CompareTo(person2.FirstName) < 0)
                    {
                        return false;
                    }
                }

                row1 = row2;
                ++index;
                row2 = GetRow(index);
            }
            return true;
        }
        /// <summary>
        /// Test method to verify grouping and filtering
        /// </summary>
        private void TestGroupingAndFiltering()
        {
            Setup(1, true, false);

            //Adding a group descriptor to datagrid which groups of the count of characters
            //of firstname and then checking if it really happens
            _testGrid.Items.GroupDescriptions.Clear();
            _testGrid.Items.SortDescriptions.Clear();
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("FirstName", new LetterCountConverter());
            _testGrid.Items.GroupDescriptions.Add(groupDescription);
            DRT.Assert(_testGrid.Items.SortDescriptions.Count == 0, "Sort Description was added for GroupDescription when not expected");

            int index = 0;
            List<int> usedList = new List<int>();
            int lastLetterCount = -1;
            while (true)
            {
                DataGridRow row = GetRow(index);
                if (row == null)
                {
                    break;
                }
                Person person = row.Item as Person;
                int letterCount = person.FirstName.Length;
                if (letterCount != lastLetterCount)
                {
                    DRT.Assert(!usedList.Contains(letterCount), "Grouping failed since first names with same letter count were found earlier but not in sequence");
                    lastLetterCount = letterCount;
                    usedList.Add(letterCount);
                }
                index++;
            }
            _testGrid.Items.GroupDescriptions.Clear();

            _testGrid.Items.SortDescriptions.Clear();

            groupDescription = new PropertyGroupDescription("FirstName");
            _testGrid.Items.GroupDescriptions.Add(groupDescription);

            DataGridColumnHeader header2 = GetColumnHeader(1);
            DRT.MoveMouse(header2, 0.2, 0.2);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            DRT.Assert(_testGrid.Items.SortDescriptions.Count == 2, "Sort Description was not added for GroupDescription when expected");
            _testGrid.Items.GroupDescriptions.Clear();

            //Adding a filter on the datagrid to allow only people who like cake
            //and check if really happens
            _testGrid.Items.Filter = new Predicate<object>(FilterDataGridOnLikingCake);
            index = 0;
            int peopleIndex = 0;
            while (peopleIndex < _people.Count)
            {
                if (_people[peopleIndex].LikesCake)
                {
                    DataGridRow row = GetRow(index++);
                    if (row == null)
                    {
                        break;
                    }
                    Person person = row.Item as Person;
                    DRT.Assert(person.LikesCake, "Filter failed, non cake liking row found");
                }
            }
            _testGrid.Items.Filter = null;
            Setup(true);
        }

        /// <summary>
        /// Filter method which allows only Persons who like cake
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool FilterDataGridOnLikingCake(object value)
        {
            Person person = value as Person;
            if (person == null)
            {
                return false;
            }

            if(person.LikesCake)
                return true;

            return false;
        }

        /// <summary>
        /// A ValueConverter class which would be used as a group descriptor.
        /// The value returned by this converter is the count of characters.
        /// </summary>
        private class LetterCountConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                string str = (string)value;
                return str.Length;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }

        }

        private void TestAutoColumnGeneration()
        {
            List<DataGridColumn> oldColumns = new List<DataGridColumn>(_testGrid.Columns);
            _testGrid.Columns.Clear();
            DrtBase.WaitForPriority(DispatcherPriority.Background);


            _testGrid.AutoGeneratingColumn += new EventHandler<DataGridAutoGeneratingColumnEventArgs>(AutoGeneratingColumnHandler);
            _testGrid.AutoGenerateColumns = true;
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            DRT.Assert(_testGrid.Columns.Count == 7, "Columns not found after adding first item to a notification collection.");
            ValidateAutoGeneratedColumn("FirstName");
            ValidateAutoGeneratedColumn("MiddleName");
            ValidateAutoGeneratedColumn("LastName");
            ValidateAutoGeneratedColumn("Id");
            ValidateAutoGeneratedColumn("LikesCake");
            int cakeColumnIndex = FindAutoGeneratedColumn("Cake");
            DRT.Assert(cakeColumnIndex < 0, "Cake column found when not expected");
            ValidateAutoGeneratedColumn("Homepage");
            ValidateAutoGeneratedColumn("ReadOnlyProperty");

            _testGrid.AutoGenerateColumns = false;
            DRT.Assert(_testGrid.Columns.Count == 0, "Columns found when not expected");

            Setup(0, true, false);
            _testGrid.AutoGenerateColumns = true;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(_testGrid.Columns.Count == 7, "Columns not found for TypedList with no items.");

            _testGrid.AutoGenerateColumns = false;
            _testGrid.AutoGeneratingColumn -= new EventHandler<DataGridAutoGeneratingColumnEventArgs>(AutoGeneratingColumnHandler);

            foreach (DataGridColumn column in oldColumns)
            {
                _testGrid.Columns.Add(column);
            }

            Setup(true);
        }

        private void AutoGeneratingColumnHandler(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string headerValue = e.Column.Header.ToString();
            if (headerValue == "Cake")
            {
                e.Cancel = true;
            }
            else if (headerValue == "Homepage")
            {
                DataGridTextColumn column = new DataGridTextColumn();
                column.Binding = new Binding("Homepage");
                column.CanUserSort = false;
                column.Header = e.Column.Header;
                e.Column = column;
            }
        }

        private void Selection_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectionChanged++;
            _lastAddedItems = e.AddedItems;
            _lastRemovedItems = e.RemovedItems;
        }

        private void Selection_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            _selectedCellsChanged++;
            _lastAddedCells = e.AddedCells;
            _lastRemovedCells = e.RemovedCells;
        }

        private void ResetSelectionTrackers()
        {
            _selectionChanged = 0;
            _selectedCellsChanged = 0;

            _lastAddedItems = null;
            _lastRemovedItems = null;
            _lastAddedCells = null;
            _lastRemovedCells = null;
        }

        private string ModeAndUnitString
        {
            get
            {
                return String.Format("({0},{1})", _testGrid.SelectionMode, _testGrid.SelectionUnit);
            }
        }

        private void AssertSelectionChanged(int numChanged)
        {
            DRT.Assert(_selectionChanged == numChanged, String.Format("Exactly {0} SelectionChanged should have occured. {1}", numChanged, ModeAndUnitString));
        }

        private void AssertSelectedCellsChanged(int numChanged)
        {
            DRT.Assert(_selectedCellsChanged == numChanged, String.Format("Exactly {0} SelectedCellsChanged should have occured. {1}", numChanged, ModeAndUnitString));
        }

        private bool CheckListCount(IList list, int count)
        {
            return ((list == null) && (count == 0)) ||
                ((list != null) && (list.Count == count));
        }

        private bool CheckListCount(IList<DataGridCellInfo> list, int count)
        {
            return ((list == null) && (count == 0)) ||
                ((list != null) && (list.Count == count));
        }

        private void AssertAddedItems(int count)
        {
            DRT.Assert(CheckListCount(_lastAddedItems, count), String.Format("{0} items should have been added. {1}", count, ModeAndUnitString));
        }

        private void AssertRemovedItems(int count)
        {
            DRT.Assert(CheckListCount(_lastRemovedItems, count), String.Format("{0} items should have been removed. {1}", count, ModeAndUnitString));
        }

        private void AssertAddedCells(int count)
        {
            DRT.Assert(CheckListCount(_lastAddedCells, count), String.Format("{0} cells should have been added. {1}", count, ModeAndUnitString));
        }

        private void AssertRemovedCells(int count)
        {
            DRT.Assert(CheckListCount(_lastRemovedCells, count), String.Format("{0} cells should have been removed. {1}", count, ModeAndUnitString));
        }

        private void AssertIsSelected(IList dataItems, bool isSelected)
        {
            if (dataItems != null)
            {
                foreach (object dataItem in dataItems)
                {
                    DataGridRow row = GetRow(dataItem);
                    if (row != null)
                    {
                        DRT.Assert(row.IsSelected == isSelected, String.Format("Row ({0}) at index {1}, IsSelected is not {2}. {3}", dataItem.ToString(), _people.IndexOf((Person)dataItem), isSelected, ModeAndUnitString));
                    }
                }
            }
        }

        private void AssertIsSelected(IList<DataGridCellInfo> cells, bool isSelected)
        {
            if (cells != null)
            {
                foreach (DataGridCellInfo cellInfo in cells)
                {
                    DataGridCell cell = GetCell(cellInfo);
                    if (cell != null)
                    {
                        DRT.Assert(cell.IsSelected == isSelected, String.Format("Cell at index {0} on row ({1}) at index {2}, IsSelected is not {3}. {4}", _testGrid.Columns.IndexOf(cell.Column), cellInfo.Item.ToString(), _people.IndexOf((Person)cellInfo.Item), isSelected, ModeAndUnitString));
                    }
                }
            }
        }

        private void AssertIsSelected()
        {
            AssertIsSelected(_lastAddedItems, true);
            AssertIsSelected(_lastRemovedItems, false);
            AssertIsSelected(_lastAddedCells, true);
            AssertIsSelected(_lastRemovedCells, false);
        }

        private void AssertIsSelected(DataGridCell cell, bool isSelected)
        {
            object dataItem = cell.DataContext;
            DRT.Assert(cell.IsSelected == isSelected, String.Format("Cell at index {0} on row ({1}) at index {2}, IsSelected is not {3}. {4}", _testGrid.Columns.IndexOf(cell.Column), dataItem.ToString(), _people.IndexOf((Person)dataItem), isSelected, ModeAndUnitString));
        }

        private void AssertIsSelected(DataGridCellInfo cellInfo, bool isSelected)
        {
            AssertIsSelected(GetCell(cellInfo), isSelected);
        }

        private void AssertIsSelected(DataGridRow row, bool isSelected)
        {
            DRT.Assert(row.IsSelected == isSelected, String.Format("Row ({0}) at index {1}, IsSelected is not {2}. {3}", row.Item.ToString(), _people.IndexOf((Person)row.Item), isSelected, ModeAndUnitString));
        }

        private void AssertCellsAreSelected(DataGridRow row, bool isSelected)
        {
            int numColumns = _testGrid.Columns.Count;
            for (int i = 0; i < numColumns; i++)
            {
                if (_testGrid.Columns[i].Visibility != Visibility.Visible)
                {
                    continue;
                }
                DataGridCell cell = GetCell(row, i);
                AssertIsSelected(cell, isSelected);
            }
        }

        private void Selection()
        {
            DataGridSelectionMode origSelectionMode = _testGrid.SelectionMode;
            DataGridSelectionUnit origSelectionUnit = _testGrid.SelectionUnit;

            _testGrid.SelectionChanged += new SelectionChangedEventHandler(Selection_OnSelectionChanged);
            _testGrid.SelectedCellsChanged += new SelectedCellsChangedEventHandler(Selection_OnSelectedCellsChanged);

            foreach (DataGridSelectionMode mode in Enum.GetValues(typeof(DataGridSelectionMode)))
            {
                foreach (DataGridSelectionUnit unit in Enum.GetValues(typeof(DataGridSelectionUnit)))
                {
                    _testGrid.SelectionMode = mode;
                    _testGrid.SelectionUnit = unit;

                    if (unit != DataGridSelectionUnit.Cell)
                    {
                        ClearSelection();
                        Selection_Rows(mode, unit);
                    }
                    if (unit != DataGridSelectionUnit.FullRow)
                    {
                        ClearSelection();
                        Selection_Cells(mode, unit);
                    }

                    ClearSelection();
                    Selection_Input_RowHeaders(mode, unit);
                    ClearSelection();
                    Selection_Input_Cells(mode, unit);
                    ClearSelection();
                    Selection_API(mode, unit);
                }
            }

            _testGrid.SelectionChanged -= new SelectionChangedEventHandler(Selection_OnSelectionChanged);
            _testGrid.SelectedCellsChanged -= new SelectedCellsChangedEventHandler(Selection_OnSelectedCellsChanged);

            _testGrid.SelectionMode = origSelectionMode;
            _testGrid.SelectionUnit = origSelectionUnit;

        }

        private void ClearSelection()
        {
            if (_testGrid.SelectionUnit != DataGridSelectionUnit.Cell)
            {
                if (_testGrid.SelectionMode == DataGridSelectionMode.Extended)
                {
                    _testGrid.SelectedItems.Clear();
                }
                else
                {
                    _testGrid.SelectedItem = null;
                }
            }

            if (_testGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
            {
                _testGrid.SelectedCells.Clear();
            }

            ResetSelectionTrackers();
        }

        private void Selection_Rows(DataGridSelectionMode mode, DataGridSelectionUnit unit)
        {
            bool multiple = (mode == DataGridSelectionMode.Extended);
            int numColumns = _testGrid.Columns.Count;

            // Select item 0
            if (multiple)
            {
                _testGrid.SelectedItems.Add(_people[0]);
            }
            else
            {
                _testGrid.SelectedItem = _people[0];
            }
            AssertSelectionChanged(1);
            AssertSelectedCellsChanged(1);
            AssertAddedItems(1);
            AssertRemovedItems(0);
            AssertAddedCells(numColumns);
            AssertRemovedCells(0);
            AssertIsSelected();
            AssertCellsAreSelected(GetRow(0), true);
            ResetSelectionTrackers();

            // Insert a column
            DataGridTemplateColumn templateColumn = new DataGridTemplateColumn();
            templateColumn.Header = "Test Column";
            _testGrid.Columns.Insert(0, templateColumn);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            AssertSelectionChanged(0);
            AssertSelectedCellsChanged(1);
            AssertAddedCells(1);
            AssertRemovedCells(0);
            AssertIsSelected();
            AssertCellsAreSelected(GetRow(0), true);
            ResetSelectionTrackers();

            // Remove a column
            _testGrid.Columns.Remove(templateColumn);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            AssertSelectionChanged(0);
            AssertSelectedCellsChanged(1);
            AssertAddedCells(0);
            AssertRemovedCells(1);
            AssertIsSelected();
            AssertCellsAreSelected(GetRow(0), true);
            ResetSelectionTrackers();

            // Add item 1
            if (multiple)
            {
                _testGrid.SelectedItems.Add(_people[1]);
            }
            else
            {
                _testGrid.SelectedItem = _people[1];
            }
            AssertSelectionChanged(1);
            AssertSelectedCellsChanged(1);
            AssertAddedItems(1);
            AssertRemovedItems(multiple ? 0 : 1);
            AssertAddedCells(numColumns);
            AssertRemovedCells(multiple ? 0 : numColumns);
            AssertIsSelected();
            AssertCellsAreSelected(GetRow(0), multiple);
            AssertCellsAreSelected(GetRow(1), true);
            ResetSelectionTrackers();

            // Remove item 1
            if (multiple)
            {
                _testGrid.SelectedItems.Remove(_people[1]);
            }
            else
            {
                _testGrid.SelectedItem = null;
            }
            AssertSelectionChanged(1);
            AssertSelectedCellsChanged(1);
            AssertAddedItems(0);
            AssertRemovedItems(1);
            AssertAddedCells(0);
            AssertRemovedCells(numColumns);
            AssertIsSelected();
            AssertCellsAreSelected(GetRow(0), multiple);
            AssertCellsAreSelected(GetRow(1), false);
            ResetSelectionTrackers();

            // Remove item 0 (only when multiple selection)
            if (multiple)
            {
                _testGrid.SelectedItems.Remove(_people[0]);
                AssertSelectionChanged(1);
                AssertSelectedCellsChanged(1);
                AssertAddedItems(0);
                AssertRemovedItems(1);
                AssertAddedCells(0);
                AssertRemovedCells(numColumns);
                AssertIsSelected();
                AssertCellsAreSelected(GetRow(0), false);
                ResetSelectionTrackers();
            }
        }

        private void Selection_Cells(DataGridSelectionMode mode, DataGridSelectionUnit unit)
        {
            bool multiple = (mode == DataGridSelectionMode.Extended);
            int numColumns = _testGrid.Columns.Count;

            // Select cell 0,0
            DataGridCellInfo cell00 = CellInfo(0, 0);
            _testGrid.SelectedCells.Add(cell00);
            AssertSelectionChanged(0);
            AssertSelectedCellsChanged(1);
            AssertAddedItems(0);
            AssertRemovedItems(0);
            AssertAddedCells(1);
            AssertRemovedCells(0);
            AssertIsSelected();
            AssertIsSelected(cell00, true);
            ResetSelectionTrackers();

            // Insert a column
            DataGridTemplateColumn templateColumn = new DataGridTemplateColumn();
            templateColumn.Header = "Test Column";
            _testGrid.Columns.Insert(0, templateColumn);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            AssertSelectionChanged(0);
            AssertSelectedCellsChanged(0);
            AssertAddedCells(0);
            AssertRemovedCells(0);
            AssertIsSelected();
            AssertIsSelected(cell00, true);
            ResetSelectionTrackers();

            // Remove a column
            _testGrid.Columns.Remove(templateColumn);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            AssertSelectionChanged(0);
            AssertSelectedCellsChanged(0);
            AssertAddedCells(0);
            AssertRemovedCells(0);
            AssertIsSelected();
            AssertIsSelected(cell00, true);
            ResetSelectionTrackers();

            // Add cell 0,1
            DataGridCellInfo cell01 = CellInfo(0, 1);
            _testGrid.SelectedCells.Add(cell01);
            AssertSelectionChanged(0);
            AssertSelectedCellsChanged(1);
            AssertAddedItems(0);
            AssertRemovedItems(0);
            AssertAddedCells(1);
            AssertRemovedCells(multiple ? 0 : 1);
            AssertIsSelected();
            AssertIsSelected(cell00, multiple);
            AssertIsSelected(cell01, true);
            ResetSelectionTrackers();

            // Remove item 0,1
            _testGrid.SelectedCells.Remove(cell01);
            AssertSelectionChanged(0);
            AssertSelectedCellsChanged(1);
            AssertAddedItems(0);
            AssertRemovedItems(0);
            AssertAddedCells(0);
            AssertRemovedCells(1);
            AssertIsSelected();
            AssertIsSelected(cell00, multiple);
            AssertIsSelected(cell01, false);
            ResetSelectionTrackers();

            // Remove item 0 (only when multiple selection)
            if (multiple)
            {
                _testGrid.SelectedCells.Remove(cell00);
                AssertSelectionChanged(0);
                AssertSelectedCellsChanged(1);
                AssertAddedItems(0);
                AssertRemovedItems(0);
                AssertAddedCells(0);
                AssertRemovedCells(1);
                AssertIsSelected();
                AssertIsSelected(cell00, false);
                ResetSelectionTrackers();
            }
        }

        private void Selection_Input_RowHeaders(DataGridSelectionMode mode, DataGridSelectionUnit unit)
        {
            bool multiple = (mode == DataGridSelectionMode.Extended);
            bool noRowSelection = (unit == DataGridSelectionUnit.Cell);
            int numColumns = _testGrid.Columns.Count;

            // Click on the first row header
            DataGridRowHeader rowHeader0 = GetRowHeader(0);
            DRT.MoveMouse(rowHeader0, 0.5, 0.5);
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            AssertSelectionChanged(noRowSelection ? 0 : 1);
            AssertSelectedCellsChanged(noRowSelection ? 0 : 1);
            AssertAddedItems(noRowSelection ? 0 : 1);
            AssertRemovedItems(0);
            AssertAddedCells(noRowSelection ? 0 : numColumns);
            AssertRemovedCells(0);
            AssertIsSelected();
            AssertCellsAreSelected(GetRow(0), !noRowSelection);
            ResetSelectionTrackers();

            // Mouse down on the fourth row header
            DataGridRowHeader rowHeader3 = GetRowHeader(3);
            DRT.MoveMouse(rowHeader3, 0.5, 0.5);
            DRT.MouseButtonDown();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            AssertSelectionChanged(noRowSelection ? 0 : 1);
            AssertSelectedCellsChanged(noRowSelection ? 0 : 1);
            AssertAddedItems(noRowSelection ? 0 : 1);
            AssertRemovedItems(noRowSelection ? 0 : 1);
            AssertAddedCells(noRowSelection ? 0 : numColumns);
            AssertRemovedCells(noRowSelection ? 0 : numColumns);
            AssertIsSelected();
            AssertCellsAreSelected(GetRow(0), false);
            AssertCellsAreSelected(GetRow(3), !noRowSelection);
            ResetSelectionTrackers();

            // Drag to the sixth row header
            DataGridRowHeader rowHeader5 = GetRowHeader(5);
            DRT.MoveMouse(rowHeader5, 0.5, 0.5);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            AssertSelectionChanged(noRowSelection ? 0 : 1);
            AssertSelectedCellsChanged(noRowSelection ? 0 : 1);
            AssertAddedItems(noRowSelection ? 0 : multiple ? 2 : 1);
            AssertRemovedItems(noRowSelection ? 0 : multiple ? 0 : 1);
            AssertAddedCells(noRowSelection ? 0 : multiple ? numColumns * 2 : numColumns);
            AssertRemovedCells(noRowSelection ? 0 : multiple ? 0 : numColumns);
            AssertIsSelected();
            AssertCellsAreSelected(GetRow(3), !noRowSelection && multiple);
            AssertCellsAreSelected(GetRow(4), !noRowSelection && multiple);
            AssertCellsAreSelected(GetRow(5), !noRowSelection);
            ResetSelectionTrackers();

            // Drag to the third row header
            DataGridRowHeader rowHeader2 = GetRowHeader(2);
            DRT.MoveMouse(rowHeader2, 0.5, 0.5);
            DRT.MouseButtonUp();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            AssertSelectionChanged(noRowSelection ? 0 : 1);
            AssertSelectedCellsChanged(noRowSelection ? 0 : 1);
            AssertAddedItems(noRowSelection ? 0 : 1);
            AssertRemovedItems(noRowSelection ? 0 : multiple ? 2 : 1);
            AssertAddedCells(noRowSelection ? 0 : numColumns);
            AssertRemovedCells(noRowSelection ? 0 : multiple ? numColumns * 2 : numColumns);
            AssertIsSelected();
            AssertCellsAreSelected(GetRow(2), !noRowSelection);
            AssertCellsAreSelected(GetRow(3), !noRowSelection && multiple);
            AssertCellsAreSelected(GetRow(4), false);
            AssertCellsAreSelected(GetRow(5), false);
            ResetSelectionTrackers();

            // SHIFT+Click on the sixth row header
            DRT.MoveMouse(rowHeader5, 0.5, 0.5);
            DRT.SendKeyboardInput(Key.LeftShift, /* press = */ true);
            DRT.ClickMouse();
            DRT.SendKeyboardInput(Key.LeftShift, /* press = */ false);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            AssertSelectionChanged(noRowSelection ? 0 : 1);
            AssertSelectedCellsChanged(noRowSelection ? 0 : 1);
            AssertAddedItems(noRowSelection ? 0 : multiple ? 2 : 1);
            AssertRemovedItems(noRowSelection ? 0 : multiple ? 1 : 1);
            AssertAddedCells(noRowSelection ? 0 : multiple ? numColumns * 2 : numColumns);
            AssertRemovedCells(noRowSelection ? 0 : numColumns);
            AssertIsSelected();
            AssertCellsAreSelected(GetRow(2), false);
            AssertCellsAreSelected(GetRow(3), !noRowSelection && multiple);
            AssertCellsAreSelected(GetRow(4), !noRowSelection && multiple);
            AssertCellsAreSelected(GetRow(5), !noRowSelection);
            ResetSelectionTrackers();

            // SHIFT+Click on the third row header
            DRT.MoveMouse(rowHeader2, 0.5, 0.5);
            DRT.SendKeyboardInput(Key.LeftShift, /* press = */ true);
            DRT.ClickMouse();
            DRT.SendKeyboardInput(Key.LeftShift, /* press = */ false);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            AssertSelectionChanged(noRowSelection ? 0 : 1);
            AssertSelectedCellsChanged(noRowSelection ? 0 : 1);
            AssertAddedItems(noRowSelection ? 0 : 1);
            AssertRemovedItems(noRowSelection ? 0 : multiple ? 2 : 1);
            AssertAddedCells(noRowSelection ? 0 : numColumns);
            AssertRemovedCells(noRowSelection ? 0 : multiple ? numColumns * 2 : numColumns);
            AssertIsSelected();
            AssertCellsAreSelected(GetRow(2), !noRowSelection);
            AssertCellsAreSelected(GetRow(3), !noRowSelection && multiple);
            AssertCellsAreSelected(GetRow(4), false);
            AssertCellsAreSelected(GetRow(5), false);
            ResetSelectionTrackers();
        }

        private void Selection_Input_Cells(DataGridSelectionMode mode, DataGridSelectionUnit unit)
        {
            bool multiple = (mode == DataGridSelectionMode.Extended);
            bool onlyRowSelection = (unit == DataGridSelectionUnit.FullRow);
            int numColumns = _testGrid.Columns.Count;

            // Click cell 2,3
            DataGridCell cell23 = GetCell(2, 3);
            DRT.MoveMouse(cell23, 0.5, 0.5);
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            AssertSelectionChanged(onlyRowSelection ? 1 : 0);
            AssertSelectedCellsChanged(1);
            AssertAddedItems(onlyRowSelection ? 1 : 0);
            AssertRemovedItems(0);
            AssertAddedCells(onlyRowSelection ? numColumns : 1);
            AssertRemovedCells(0);
            AssertIsSelected();
            if (onlyRowSelection)
            {
                AssertCellsAreSelected(GetRow(2), true);
            }
            else
            {
                AssertIsSelected(cell23, true);
            }
            ResetSelectionTrackers();

            // Mouse down on cell 2,1
            DataGridCell cell21 = GetCell(2, 1);
            DRT.MoveMouse(cell21, 0.5, 0.5);
            DRT.MouseButtonDown();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            AssertSelectionChanged(0);
            AssertSelectedCellsChanged(onlyRowSelection ? 0 : 1);
            AssertAddedItems(0);
            AssertRemovedItems(0);
            AssertAddedCells(onlyRowSelection ? 0 : 1);
            AssertRemovedCells(onlyRowSelection ? 0 : 1);
            AssertIsSelected();
            if (onlyRowSelection)
            {
                AssertCellsAreSelected(GetRow(2), true);
            }
            else
            {
                AssertIsSelected(cell21, true);
                AssertIsSelected(cell23, false);
            }
            ResetSelectionTrackers();

            // Drag to cell 2,4
            DataGridCell cell24 = GetCell(2, 4);
            DRT.MoveMouse(cell24, 0.5, 0.5);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            AssertSelectionChanged(0);
            AssertSelectedCellsChanged(onlyRowSelection ? 0 : 1);
            AssertAddedItems(0);
            AssertRemovedItems(0);
            AssertAddedCells(onlyRowSelection ? 0 : multiple ? 3 : 1);
            AssertRemovedCells((multiple || onlyRowSelection) ? 0 : 1);
            AssertIsSelected();
            if (onlyRowSelection)
            {
                AssertCellsAreSelected(GetRow(2), true);
            }
            else
            {
                AssertIsSelected(cell21, multiple);
                AssertIsSelected(cell23, multiple);
                AssertIsSelected(cell24, true);
            }
            ResetSelectionTrackers();

            // Drag to cell 2,0
            DataGridCell cell20 = GetCell(2, 0);
            DRT.MoveMouse(cell20, 0.5, 0.5);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            AssertSelectionChanged(0);
            AssertSelectedCellsChanged(onlyRowSelection ? 0 : 1);
            AssertAddedItems(0);
            AssertRemovedItems(0);
            AssertAddedCells(onlyRowSelection ? 0 : 1);
            AssertRemovedCells(onlyRowSelection ? 0 : multiple ? 3 : 1);
            AssertIsSelected();
            if (onlyRowSelection)
            {
                AssertCellsAreSelected(GetRow(2), true);
            }
            else
            {
                AssertIsSelected(cell20, true);
                AssertIsSelected(cell21, multiple);
                AssertIsSelected(cell23, false);
                AssertIsSelected(cell24, false);
            }
            ResetSelectionTrackers();

            // Drag to cell 3,0
            DataGridCell cell30 = GetCell(3, 0);
            DRT.MoveMouse(cell30, 0.5, 0.5);
            DRT.MouseButtonUp();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            AssertSelectionChanged(onlyRowSelection ? 1 : 0);
            AssertSelectedCellsChanged(1);
            AssertAddedItems(onlyRowSelection ? 1 : 0);
            AssertRemovedItems((onlyRowSelection && !multiple) ? 1 : 0);
            AssertAddedCells(onlyRowSelection ? numColumns : multiple ? 2 : 1);
            AssertRemovedCells(onlyRowSelection ? multiple ? 0 : numColumns : multiple ? 0 : 1);
            AssertIsSelected();
            if (onlyRowSelection)
            {
                AssertCellsAreSelected(GetRow(2), multiple);
                AssertCellsAreSelected(GetRow(3), true);
            }
            else
            {
                AssertIsSelected(cell20, multiple);
                AssertIsSelected(cell21, multiple);
                AssertIsSelected(cell23, false);
                AssertIsSelected(cell24, false);
                AssertIsSelected(cell30, true);
                AssertIsSelected(GetCell(3, 1), multiple);
            }
            ResetSelectionTrackers();

            // Click cell 2,1
            DRT.MoveMouse(cell21, 0.5, 0.5);
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.PressKey(Key.Escape);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            AssertSelectionChanged(!onlyRowSelection ? 0 : 1);
            AssertSelectedCellsChanged(1);
            AssertAddedItems((onlyRowSelection && !multiple) ? 1 : 0);
            AssertRemovedItems(onlyRowSelection ? 1 : 0);
            AssertAddedCells(onlyRowSelection ? multiple ? 0 : numColumns : multiple ? 0 : 1);
            AssertRemovedCells(onlyRowSelection ? numColumns : multiple ? 3 : 1);
            AssertIsSelected();
            if (onlyRowSelection)
            {
                AssertCellsAreSelected(GetRow(2), true);
                AssertCellsAreSelected(GetRow(3), false);
            }
            else
            {
                AssertIsSelected(cell20, false);
                AssertIsSelected(cell21, true);
                AssertIsSelected(cell23, false);
                AssertIsSelected(cell24, false);
                AssertIsSelected(cell30, false);
                AssertIsSelected(GetCell(3, 1), false);
            }
            ResetSelectionTrackers();

            // SHIFT+Click cell 3,0
            DRT.MoveMouse(cell30, 0.5, 0.5);
            DRT.SendKeyboardInput(Key.LeftShift, /* press = */ true);
            DRT.ClickMouse();
            DRT.SendKeyboardInput(Key.LeftShift, /* press = */ false);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            AssertSelectionChanged(onlyRowSelection ? 1 : 0);
            AssertSelectedCellsChanged(1);
            AssertAddedItems(onlyRowSelection ? 1 : 0);
            AssertRemovedItems((onlyRowSelection && !multiple) ? 1 : 0);
            AssertAddedCells(onlyRowSelection ? numColumns : multiple ? 3 : 1);
            AssertRemovedCells(onlyRowSelection ? multiple ? 0 : numColumns : multiple ? 0 : 1);
            AssertIsSelected();
            if (onlyRowSelection)
            {
                AssertCellsAreSelected(GetRow(2), multiple);
                AssertCellsAreSelected(GetRow(3), true);
            }
            else
            {
                AssertIsSelected(cell20, multiple);
                AssertIsSelected(cell21, multiple);
                AssertIsSelected(cell23, false);
                AssertIsSelected(cell24, false);
                AssertIsSelected(cell30, true);
                AssertIsSelected(GetCell(3, 1), multiple);
            }
            ResetSelectionTrackers();

            // Up Arrow
            DRT.PressKey(Key.Up);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            AssertSelectionChanged(onlyRowSelection ? 1 : 0);
            AssertSelectedCellsChanged(1);
            AssertAddedItems((onlyRowSelection && !multiple) ? 1 : 0);
            AssertRemovedItems(onlyRowSelection ? 1 : 0);
            AssertAddedCells(onlyRowSelection ? multiple ? 0 : numColumns : multiple ? 0 : 1);
            AssertRemovedCells(onlyRowSelection ? numColumns : multiple ? 3 : 1);
            AssertIsSelected();
            if (onlyRowSelection)
            {
                AssertCellsAreSelected(GetRow(2), true);
                AssertCellsAreSelected(GetRow(3), false);
            }
            else
            {
                AssertIsSelected(cell20, true);
                AssertIsSelected(cell21, false);
                AssertIsSelected(cell23, false);
                AssertIsSelected(cell24, false);
                AssertIsSelected(cell30, false);
                AssertIsSelected(GetCell(3, 1), false);
            }
            ResetSelectionTrackers();

            // SHIFT+Down Arrow
            DRT.SendKeyboardInput(Key.LeftShift, /* press = */ true);
            DRT.PressKey(Key.Down);
            DRT.SendKeyboardInput(Key.LeftShift, /* press = */ false);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            AssertSelectionChanged(onlyRowSelection ? 1 : 0);
            AssertSelectedCellsChanged(1);
            AssertAddedItems(onlyRowSelection ? 1 : 0);
            AssertRemovedItems((onlyRowSelection && !multiple) ? 1 : 0);
            AssertAddedCells(onlyRowSelection ? numColumns : 1);
            AssertRemovedCells(multiple ? 0 : onlyRowSelection ? numColumns : 1);
            AssertIsSelected();
            if (onlyRowSelection)
            {
                AssertCellsAreSelected(GetRow(2), multiple);
                AssertCellsAreSelected(GetRow(3), true);
            }
            else
            {
                AssertIsSelected(cell20, multiple);
                AssertIsSelected(cell30, true);
            }
            ResetSelectionTrackers();
        }

        private void Selection_API(DataGridSelectionMode mode, DataGridSelectionUnit unit)
        {
            if (mode == DataGridSelectionMode.Extended)
            {
                int numItems = _testGrid.Items.Count;
                int numColumns = _testGrid.Columns.Count;
                int numCells = numItems * numColumns;

                if (unit != DataGridSelectionUnit.Cell)
                {
                    _testGrid.SelectAll();
                    AssertSelectionChanged(1);
                    AssertSelectedCellsChanged(1);
                    AssertAddedItems(numItems);
                    AssertRemovedItems(0);
                    AssertAddedCells(numCells);
                    AssertRemovedCells(0);
                    AssertIsSelected();
                    ResetSelectionTrackers();

                    _testGrid.UnselectAll();
                    AssertSelectionChanged(1);
                    AssertSelectedCellsChanged(1);
                    AssertAddedItems(0);
                    AssertRemovedItems(numItems);
                    AssertAddedCells(0);
                    AssertRemovedCells(numCells);
                    AssertIsSelected();
                    ResetSelectionTrackers();
                }

                if (unit != DataGridSelectionUnit.FullRow)
                {
                    _testGrid.SelectAllCells();
                    AssertSelectionChanged(0);
                    AssertSelectedCellsChanged(1);
                    AssertAddedItems(0);
                    AssertRemovedItems(0);
                    AssertAddedCells(numCells);
                    AssertRemovedCells(0);
                    AssertIsSelected();
                    ResetSelectionTrackers();

                    _testGrid.UnselectAllCells();
                    AssertSelectionChanged(0);
                    AssertSelectedCellsChanged(1);
                    AssertAddedItems(0);
                    AssertRemovedItems(0);
                    AssertAddedCells(0);
                    AssertRemovedCells(numCells);
                    AssertIsSelected();
                    ResetSelectionTrackers();
                }
            }
        }


        private void FrozenColumns()
        {
            int originalFrozenCount = _testGrid.FrozenColumnCount;
            _testGrid.FrozenColumnCount = 2;

            //checking if columns and headers got frozen property set appropriately
            DataGridColumn column = GetColumn(0);
            DRT.Assert(column.IsFrozen, "Column 0 should be frozen");
            DataGridColumnHeader header = GetColumnHeader(1);
            DRT.Assert(header.IsFrozen, "Header 1 should be frozen");
            column = GetColumn(4);
            DRT.Assert(!column.IsFrozen, "Column 4 should not be frozen");

            //Setting autogeneration to true so that horizontal scroll bar appears
            _testGrid.AutoGenerateColumns = true;
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            //checking how transforms of frozen and non-frozen columns change after scroll
            DataGridColumnHeader header1 = header;
            DataGridColumnHeader header2 = GetColumnHeader(4);

            DataGridColumn starColumn = GetStarColumn();
            DataGridLength originalStarWidth = starColumn.Width;
            starColumn.Width = 100d;
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            ScrollViewer scrollViewer = GetScrollViewer();
            GeneralTransform transform1 = header1.TransformToAncestor(_testGrid);
            GeneralTransform transform3 = header2.TransformToAncestor(_testGrid);

            scrollViewer.LineRight();
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            GeneralTransform transform2 = header1.TransformToAncestor(_testGrid);
            GeneralTransform transform4 = header2.TransformToAncestor(_testGrid);

            DRT.Assert(transform1.ToString() == transform2.ToString(), "Transforms For Header1 should be equal");
            DRT.Assert(transform3.ToString() != transform4.ToString(), "Transforms for Header3 should not be equal");

            _testGrid.AutoGenerateColumns = false;
            _testGrid.FrozenColumnCount = originalFrozenCount;
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            starColumn.Width = originalStarWidth;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
        }

        private void ReorderColumns()
        {
            int originalFrozenCount = _testGrid.FrozenColumnCount;
            _testGrid.FrozenColumnCount = 0;

            //move header 0 to first half of header 3
            DataGridColumn column = _testGrid.ColumnFromDisplayIndex(0);
            DataGridColumnHeader header1 = GetColumnHeaderFromDisplayIndex(0);
            DRT.MoveMouse(header1, 0.2, 0.5);
            DRT.MouseButtonDown();
            DataGridColumnHeader header2 = GetColumnHeaderFromDisplayIndex(3);
            DRT.MoveMouse(header2, 0.2, 0.5);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.MouseButtonUp();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(column.DisplayIndex == 2, "Display index of column should get changed to 2");

            //move header 2 to first half of header 0
            DRT.MoveMouse(header1, 0.2, 0.5);
            DRT.MouseButtonDown();
            DataGridColumnHeader header3 = GetColumnHeaderFromDisplayIndex(0);
            DRT.MoveMouse(header3, 0.2, 0.5);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.MouseButtonUp();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(column.DisplayIndex == 0, "Display index of column should get changed to 0");

            //move header 0 to first half of header 3 on datagrid with frozen count 2
            _testGrid.FrozenColumnCount = 2;
            DRT.MoveMouse(header1, 0.2, 0.5);
            DRT.MouseButtonDown();
            DRT.MoveMouse(header2, 0.2, 0.5);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.MouseButtonUp();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(column.DisplayIndex == 0, "Display index of frozen column should not get changed to 2");

            //move header 3 to first half of header 0 on datagrid with frozen count 2
            column = _testGrid.ColumnFromDisplayIndex(3);
            DRT.MoveMouse(header2, 0.2, 0.5);
            DRT.MouseButtonDown();
            DRT.MoveMouse(header1, 0.2, 0.5);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.MouseButtonUp();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(column.DisplayIndex == 3, "Display index of non frozen column should get changed to 0  ");

            _testGrid.FrozenColumnCount = originalFrozenCount;
        }

        private void TestHeadersVisibility()
        {
            var oldVis = _testGrid.HeadersVisibility;
            var columnHeader = FindChild<DataGridColumnHeadersPresenter>(_testGrid);
            var rowHeader = FindChild<DataGridRowHeader>(_testGrid);

            _testGrid.HeadersVisibility = DataGridHeadersVisibility.None;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(columnHeader.Visibility == Visibility.Collapsed, "When HeadersVisibility = None, column headers should be collapsed.");
            DRT.Assert(rowHeader.Visibility == Visibility.Collapsed, "When HeadersVisibility = None, row headers should be collapsed.");

            _testGrid.HeadersVisibility = DataGridHeadersVisibility.Row;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(columnHeader.Visibility == Visibility.Collapsed, "When HeadersVisibility = Row, column headers should be collapsed.");
            DRT.Assert(rowHeader.Visibility == Visibility.Visible, "When HeadersVisibility = Row, row headers should be visible.");

            _testGrid.HeadersVisibility = DataGridHeadersVisibility.Column;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(columnHeader.Visibility == Visibility.Visible, "When HeadersVisibility = Column, column headers should be visible.");
            DRT.Assert(rowHeader.Visibility == Visibility.Collapsed, "When HeadersVisibility = Column, row headers should be collapsed.");

            _testGrid.HeadersVisibility = DataGridHeadersVisibility.All;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(columnHeader.Visibility == Visibility.Visible, "When HeadersVisibility = All, column headers should be visible.");
            DRT.Assert(rowHeader.Visibility == Visibility.Visible, "When HeadersVisibility = All, row headers should be visible.");

            _testGrid.HeadersVisibility = oldVis;
        }

        private void TestHeaderSizes()
        {
            var oldColumnHeaderHeight = _testGrid.ColumnHeaderHeight;
            var oldRowHeaderWidth = _testGrid.RowHeaderWidth;
            var columnHeader = FindChild<DataGridColumnHeadersPresenter>(_testGrid);
            var rowHeader = FindChild<DataGridRowHeader>(_testGrid);

            var curRowHeaderWidth = rowHeader.ActualWidth;
            _testGrid.ColumnHeaderHeight = 30.0;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(columnHeader.ActualHeight == 30.0, "When ColumnHeaderHeight = 30.0, the DataGridColumnHeadersPresenter should have a height of 30.0");
            DRT.Assert(rowHeader.ActualWidth == curRowHeaderWidth, "Changing the ColumnHeaderHeight should not affect the RowHeader's Width");

            _testGrid.ColumnHeaderHeight = 60.0;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(columnHeader.ActualHeight == 60.0, "When ColumnHeaderHeight = 60.0, the DataGridColumnHeadersPresenter should have a height of 60.0");
            DRT.Assert(rowHeader.ActualWidth == curRowHeaderWidth, "Changing the ColumnHeaderHeight should not affect the RowHeader's Width");

            var curColumnHeaderHeight = columnHeader.ActualHeight;
            _testGrid.RowHeaderWidth = 30.0;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(rowHeader.ActualWidth == 30.0, "When RowHeaderWidth = 30.0, the RowHeader's should have a width of 30.0");
            DRT.Assert(columnHeader.ActualHeight == curColumnHeaderHeight, "Changing the RowHeaderWidth should not affect the DataGridColumnHeadersPresenter's Height");

            _testGrid.RowHeaderWidth = 60.0;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(rowHeader.ActualWidth == 60.0, "When RowHeaderWidth = 60.0, the RowHeader's should have a width of 60.0");
            DRT.Assert(columnHeader.ActualHeight == curColumnHeaderHeight, "Changing the RowHeaderWidth should not affect the DataGridColumnHeadersPresenter's Height");

            var oldSource = _testGrid.ItemsSource;
            _testGrid.RowHeaderWidth = double.NaN;
            _testGrid.ItemsSource = new Person[] { NewPerson("ASDF", "ADSF", false) };
            DRT.Assert(_testGrid.RowHeaderActualWidth == 0.0, "Changing DataGrid.ItemsSource should cause RowHeaderActualWidth to be reset when in 'Auto' mode.");
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(_testGrid.RowHeaderActualWidth != 0.0, "DataGridRowHeader.MeasureOverride should update RowHeaderActualWidth when in 'Auto' mode.");

            _testGrid.ColumnHeaderHeight = oldColumnHeaderHeight;
            _testGrid.RowHeaderWidth = oldRowHeaderWidth;
            _testGrid.ItemsSource = oldSource;
        }

        private void TestRowValidation()
        {
            var container = (DataGridRow)_testGrid.ItemContainerGenerator.ContainerFromIndex(0);
            var bindingGroup = container.BindingGroup;
            var item = (Person)container.Item;
            var oldfirstname = item.FirstName;
            var oldlastname = item.LastName;
            DataGridCell firstNameCell = GetCell(container, 0);

            DRT.Assert(!Validation.GetHasError(container), "Container shouldn't have an error to begin with.");

            // test NameRule
            SetName("Bob", "Bob", firstNameCell);
            DRT.Assert(Validation.GetHasError(container), "Container should fail validation.");

            // test JohnAdamsRule
            SetName("John", "Adams", firstNameCell);
            DRT.Assert(Validation.GetHasError(container), "Container should fail validation.");

            SetName(oldfirstname, oldlastname, firstNameCell);
            DRT.Assert(!Validation.GetHasError(container), "Container shouldn't fail validation.");
        }

        private void SetName(string firstName, string lastName, DataGridCell firstNameCell)
        {
            DRT.MoveMouse(firstNameCell, 0.5, 0.5);     // move mouse over FirstName cell
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.ClickMouse();                           // focus cell
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.PressKey(Key.F2);                       // open row for edit
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.SendString(firstName);                  // type new value
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.PressKey(Key.Tab);                      // tab to LastName cell
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.SendString(lastName);                   // type new value
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.PressKey(Key.Return);                   // commit row
            DrtBase.WaitForPriority(DispatcherPriority.Background);
        }

        private void TestRowDetails()
        {
            var detailsChangedCount = 0;
            EventHandler<DataGridRowDetailsEventArgs> handler = (s, e) =>
                {
                    detailsChangedCount++;
                    DRT.Assert(e.DetailsElement != null, "RowDetailsVisibilityChanged should fire after template expansion.");
                };
            _testGrid.RowDetailsVisibilityChanged += handler;
            _testGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Visible;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(detailsChangedCount > 0, "RowDetailsVisibilityChanged did not fire.");

            _testGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
            _testGrid.RowDetailsVisibilityChanged -= handler;
        }


        private void TestColumnRemoval()
        {
            DataGridColumn[] oldColumns = new DataGridColumn[_testGrid.Columns.Count];
            _testGrid.Columns.CopyTo(oldColumns, 0);

            _testGrid.Columns.Clear();
            DRT.Assert(_testGrid.Columns.Count == 0, "Clearing the ColumnsCollection didn't remove all the columns");
            DRT.Assert(oldColumns[1].DisplayIndex == -1, "Clearing the ColumnsCollection didn't clear the columns DisplayIndex");

            _testGrid.Columns.Add(oldColumns[1]);
            DRT.Assert(_testGrid.Columns.Count == 1, "Adding to the ColumnsCollection didn't add the column");
            DRT.Assert(oldColumns[1].DisplayIndex != -1, "Adding to the ColumnsCollection didn't change the DisplayIndex");

            _testGrid.Columns.Remove(oldColumns[1]);
            DRT.Assert(_testGrid.Columns.Count == 0, "Removing from the ColumnsCollection didn't remove the column");
            DRT.Assert(oldColumns[1].DisplayIndex == -1, "Removing from the ColumnsCollection didn't clear the columns DisplayIndex");

            foreach (var column in oldColumns)
            {
                _testGrid.Columns.Add(column);
            }

            DRT.Assert(_testGrid.Columns.Count == oldColumns.Length, "All of the original columns were not re-added.");
        }

        private void TestColumnWidthChange()
        {
            DataGridColumn starColumn = GetStarColumn();
            DataGridLength originalStarWidth = starColumn.Width;

            DataGridColumn firstColumn = _testGrid.ColumnFromDisplayIndex(0);
            DataGridLength originalFirstWidth = firstColumn.Width;
            firstColumn.Width = 1000d;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(firstColumn.ActualWidth < 1000d, "In presence of star column the first column cannot be 1000 pixel wide");

            starColumn.Width = 40d;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(firstColumn.ActualWidth == 1000d, "First column should be 1000 pixel wide after removal of star column");

            firstColumn.Width = 1020d;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(firstColumn.ActualWidth == 1020d, "First column should be 1020 pixel wide after removal of star column");

            starColumn.Width = originalStarWidth;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(firstColumn.ActualWidth < 1000d, "After reverting to the star column, the first column cannot be 1000 pixel wide");

            firstColumn.MinWidth = 800d;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(firstColumn.ActualWidth == 800d, "In presence of star column the first column should be atleast MinWidth wide");
            firstColumn.ClearValue(DataGridColumn.MinWidthProperty);

            firstColumn.MaxWidth = 20d;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(firstColumn.ActualWidth == 20d, "In presence of star column the first column should be atmost MaxWidth wide");
            firstColumn.ClearValue(DataGridColumn.MaxWidthProperty);

            firstColumn.Width = originalFirstWidth;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
        }

        private DataGridColumn GetStarColumn()
        {
            return _testGrid.ColumnFromDisplayIndex(4);
        }

        private void TestIsReadOnly()
        {
            var cell = GetCell(1,1);
            DRT.Assert(cell.IsReadOnly == false, "the cell should not be read only");
            DRT.Assert(cell.IsEditing == false, "the cell should not be editing");

            DRT.MoveMouse(cell, 0.5, 0.5);
            DRT.ClickMouse();
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            // double clicking a cell should edit it
            DRT.Assert(cell.IsEditing == true, "the cell should be editable");

            cell.Column.IsReadOnly = true;
            // making the column read only makes the cell read only
            DRT.Assert(cell.IsReadOnly == true, "the cell should be read only");
            DRT.Assert(cell.IsEditing == false, "the cell should not be editing");

            DRT.MoveMouse(cell, 0.5, 0.5);
            DRT.ClickMouse();
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            // you cant edit a read only cell
            DRT.Assert(cell.IsReadOnly == true, "the cell should be read only");
            DRT.Assert(cell.IsEditing == false, "the cell should not be editing");

            cell.Column.ClearValue(DataGridColumn.IsReadOnlyProperty);
            DRT.MoveMouse(cell, 0.5, 0.5);
            DRT.ClickMouse();
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            // you can edit a non-read only cell
            DRT.Assert(cell.IsReadOnly == false, "the cell should not be read only");
            DRT.Assert(cell.IsEditing == true, "the cell should be editing");

            var dataGrid = FindParent<DataGrid>(cell);
            dataGrid.IsReadOnly = true;
            // if the DG is read only, everything is read only.
            DRT.Assert(cell.Column.IsReadOnly == true, "the column should be read only");
            DRT.Assert(cell.IsReadOnly == true, "the cell should be read only");
            DRT.Assert(cell.IsEditing == false, "the cell should not be editing");

            DRT.MoveMouse(cell, 0.5, 0.5);
            DRT.ClickMouse();
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            // you cant edit a read only cell
            DRT.Assert(cell.Column.IsReadOnly == true, "the column should be read only");
            DRT.Assert(cell.IsReadOnly == true, "the cell should be read only");
            DRT.Assert(cell.IsEditing == false, "the cell should not be editing");

            dataGrid.ClearValue(DataGrid.IsReadOnlyProperty);
            DRT.MoveMouse(cell, 0.5, 0.5);
            DRT.ClickMouse();
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            // you can edit a non-read only cell
            DRT.Assert(cell.Column.IsReadOnly == false, "the column should not be read only");
            DRT.Assert(cell.IsReadOnly == false, "the cell should not be read only");
            DRT.Assert(cell.IsEditing == true, "the cell should be editing");

            // cancel editing
            DRT.SendKeyboardInput(Key.Escape, /* press = */ true);
        }

        private void ColumnVirtualizationTest()
        {
            _testGrid.AutoGenerateColumns = true;
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            DataGridColumn starColumn = GetStarColumn();
            DataGridLength originalStarWidth = starColumn.Width;
            starColumn.Width = 300d;
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            DataGridCell cell = GetCell(0, 9);
            DRT.Assert(cell != null, "Cell is not expected to be virtualized, when DataGrid.EnableColumnVirtualization is false.");

            _testGrid.EnableColumnVirtualization = true;
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            cell = GetCell(0, 9);
            DRT.Assert(cell == null, "Cell is expected to be virtualized.");

            ScrollViewer scrollViewer = GetScrollViewer();
            scrollViewer.PageRight();
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            cell = GetCell(0, 9);
            DRT.Assert(cell != null, "Cell is expected to be de-virtualized after right scroll.");

            scrollViewer.PageLeft();
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            cell = GetCell(0, 9);
            DRT.Assert(cell == null, "Cell is expected to be virtualized after left scroll.");

            _testGrid.AutoGenerateColumns = false;
            starColumn.Width = originalStarWidth;
            _testGrid.ClearValue(DataGrid.EnableColumnVirtualizationProperty);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
        }

        private void HiddenColumnsTest()
        {
            DataGridCell cell00 = GetCell(0, 0);
            DRT.MoveMouse(cell00, 0.5, 0.5);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.PressKey(Key.Tab);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(_testGrid.CurrentCell.Column == _testGrid.Columns[1], "1st tab should go to 2nd column");
            DRT.PressKey(Key.Tab);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(_testGrid.CurrentCell.Column == _testGrid.Columns[3], "2nd tab should go to 4th column");

            DataGridCell cell01 = GetCell(0, 1);
            DRT.MoveMouse(cell01, 0.5, 0.5);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.ClickMouse();
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            DataGridColumn fullNameColumn = GetColumn(2);
            fullNameColumn.Visibility = Visibility.Visible;
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            DRT.PressKey(Key.Tab);
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            DRT.Assert(_testGrid.CurrentCell.Column == _testGrid.Columns[2], "3rd tab should go to 3rd column.");

            DataGridColumnHeader fullNameHeader = GetColumnHeader(2);
            DRT.Assert(fullNameHeader != null, "FullName column header should not be null, if the column is visible.");
            ValidateTextColumn(1, "LastName");
            ValidateFullNameColumn();
            ValidateCheckBoxColumn(3, "LikesCake");

            fullNameColumn.Visibility = Visibility.Collapsed;
            DrtBase.WaitForPriority(DispatcherPriority.Background);
            fullNameHeader = GetColumnHeader(2);

            DRT.Assert(fullNameHeader == null, "FullName column header should be null, if the column is hidden.");

            // selection and clipboard scenarios for hidden columns are already tested in those cases.

            // See that nothing fails on multiple visibility changes
            GetColumn(0).Visibility = Visibility.Hidden;
            GetColumn(1).Visibility = Visibility.Hidden;
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            // See that nothing fails after all the columns are hidden
            for (int i = 3; i < _testGrid.Columns.Count; i++)
            {
                GetColumn(i).Visibility = Visibility.Collapsed;
                DrtBase.WaitForPriority(DispatcherPriority.Background);
            }

            // Restoring to original configuration
            for (int i = 0; i < _testGrid.Columns.Count; i++)
            {
                DataGridColumn column = GetColumn(i);
                if (column != fullNameColumn)
                {
                    column.Visibility = Visibility.Visible;
                }
            }
            DrtBase.WaitForPriority(DispatcherPriority.Background);
        }

        private void IsReadOnlyColumnTest()
        {
            var oldIsReadOnly = _testGrid.Columns[1].IsReadOnly;
            _testGrid.Columns[1].IsReadOnly = true;

            var cell = GetCell(_testGrid, 1, 1);
            DRT.Assert(cell.IsReadOnly, "cell 1 from column 1 should be read only.");

            cell = GetCell(_testGrid, 9, 1);
            DRT.Assert(cell.IsReadOnly, "cell 9 from column 1 should be read only.");

            _testGrid.Columns[1].IsReadOnly = oldIsReadOnly;
        }

        public DataGridCell GetCell(DataGrid dataGrid, int row, int column)
        {
            DataGridRow rowContainer = GetRow(dataGrid, row);
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = FindChild<DataGridCellsPresenter>(rowContainer);

                // try to get the cell but it may possibly be virtualized
                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell == null)
                {
                    // now try to bring into view and retreive the cell
                    dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
                    DrtBase.WaitForPriority(DispatcherPriority.Background);

                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }

                return cell;
            }

            return null;
        }

        public DataGridRow GetRow(DataGrid dataGrid, int index)
        {
            DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // may be virtualized, bring into view and try again
                dataGrid.ScrollIntoView(dataGrid.Items[index]);
                DrtBase.WaitForPriority(DispatcherPriority.Background);

                row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
            }

            return row;
        }

        private void ClipboardTest()
        {
            DataGridSelectionMode origSelectionMode = _testGrid.SelectionMode;
            DataGridSelectionUnit origSelectionUnit = _testGrid.SelectionUnit;
            Clipboard.Clear(); // Clear the clipboard before we copy a datagrid selection

            DRT.Assert(_testGrid.ClipboardCopyMode == DataGridClipboardCopyMode.ExcludeHeader, "Default ClipboardCopyMode should be ExcludeHeader.");

            _testGrid.SelectionMode = DataGridSelectionMode.Extended;
            _testGrid.SelectionUnit = DataGridSelectionUnit.FullRow;

            // Select 3 rows
            ClearSelection();
            _testGrid.SelectedItems.Add(_people[10]);
            _testGrid.SelectedItems.Add(_people[3]);
            _testGrid.SelectedItems.Add(_people[8]);

            _testGrid.CopyingRowClipboardContent += new EventHandler<DataGridRowClipboardEventArgs>(OnCopyingRowClipboardContent);

            ClipboardCopy(false /*headers*/);
            ClipboardCopy(true /*headers*/);

            ClearSelection();
            _testGrid.SelectionMode = origSelectionMode;
            _testGrid.SelectionUnit = origSelectionUnit;
        }

        private int _countCopyingRowClipboardContent;
        private int _numHeaderRowsInClipboard;

        void OnCopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
        {
            _countCopyingRowClipboardContent++;

            if (e.IsColumnHeadersRow)
            {
                _numHeaderRowsInClipboard++;
                DRT.Assert(e.Item == null, "DataGridRowClipboardEventArgs.Item should be null for the header row.");

                // Replace the default header
                e.ClipboardRowContent[0] = new DataGridClipboardCellContent(null,null,"custom header");
            }
        }

        private void ClipboardCopy(bool withHeaders)
        {
            _countCopyingRowClipboardContent = 0;
            _numHeaderRowsInClipboard = 0;

            // Include headers
            _testGrid.ClipboardCopyMode = withHeaders ? DataGridClipboardCopyMode.IncludeHeader : DataGridClipboardCopyMode.ExcludeHeader;

            // Press Ctrl+C
            _testGrid.Focus();
            DRT.SendKeyboardInput(Key.LeftCtrl, /* press = */ true);
            DRT.PressKey(Key.C);
            DRT.SendKeyboardInput(Key.LeftCtrl, /* press = */ false);
            DrtBase.WaitForPriority(DispatcherPriority.Background);

            // Verify clipboard content
            string htmlString = Clipboard.GetData(DataFormats.Html) as string;
            string unicodeTextString = Clipboard.GetData(DataFormats.UnicodeText) as string;
            string textString = Clipboard.GetData(DataFormats.Text) as string;
            string csvString = Clipboard.GetData(DataFormats.CommaSeparatedValue) as string;

            DRT.Assert(htmlString != null, "Clipboard HTML format is missing");
            DRT.Assert(unicodeTextString != null, "Clipboard UnicodeText format is missing");
            DRT.Assert(textString != null, "Clipboard Text format is missing");
            DRT.Assert(csvString != null, "Clipboard CommaSeparatedValue format is missing");

            int expectedRows = withHeaders ? 4 : 3;
            int expectedCells = expectedRows * GetVisibleColumnCount();
            VerifyHTML(htmlString, expectedRows, expectedCells);
            VerifyText(unicodeTextString, expectedRows, expectedCells);
            VerifyText(textString, expectedRows, expectedCells);
            VerifyCSV(csvString, expectedRows, expectedCells);

            // Verify event count
            DRT.Assert(_countCopyingRowClipboardContent == expectedRows, "CopyingRowClipboardContent event should raise {0} times. Was: {1}.", expectedRows, _countCopyingRowClipboardContent);

            // Verify header count in the event
            DRT.Assert(_numHeaderRowsInClipboard == (withHeaders ? 1 : 0), "NumHeaderRowsInClipboard should be {0}. Was: {1}.", (withHeaders ? 1 : 0), _numHeaderRowsInClipboard);

            // Verify custom header replacement from the event
            if (withHeaders)
                DRT.Assert(GetRepeatCount(textString, "custom header") == 1, "custom header cannot be found");
        }

        private void VerifyHTML(string htmlString, int lineCount, int cellCount)
        {
            DRT.Assert(htmlString.Contains("<HTML>"), "<HTML> is missing");
            DRT.Assert(htmlString.Contains("</HTML>"), "</HTML> is missing");
            DRT.Assert(htmlString.Contains("<BODY>"), "<BODY> is missing");
            DRT.Assert(htmlString.Contains("</BODY>"), "</BODY> is missing");
            DRT.Assert(htmlString.Contains("<TABLE>"), "<TABLE> is missing");
            DRT.Assert(htmlString.Contains("</TABLE>"), "</TABLE> is missing");

            DRT.Assert(GetRepeatCount(htmlString, "<TR>") == lineCount, "{0} <TR> expected in HTML clipboard format", lineCount);
            DRT.Assert(GetRepeatCount(htmlString, "</TR>") == lineCount, "{0} </TR> expected in HTML clipboard format", lineCount);

            int numberOfCells = GetRepeatCount(htmlString, "<TD>");
            DRT.Assert(numberOfCells == cellCount, "{0} <TD> expected in HTML clipboard format. Found:{1}, Lines:{2}", cellCount, numberOfCells, lineCount);

            numberOfCells = GetRepeatCount(htmlString, "</TD>");
            DRT.Assert(numberOfCells == cellCount, "{0} </TD> expected in HTML clipboard format. Found:{1}, Lines:{2}", cellCount, numberOfCells, lineCount);
        }

        private void VerifyText(string textString, int lineCount, int cellCount)
        {
            int numberOfLines = GetRepeatCount(textString, "\r\n");
            DRT.Assert(numberOfLines == lineCount, "{0} lines expected in TEXT clipboard format. Found:{1}", lineCount, numberOfLines);
            int numberOfTabs = GetRepeatCount(textString, "\t");
            DRT.Assert(numberOfTabs == (cellCount - lineCount), "{0} tabs expected in TEXT clipboard format. Found tabs:{1}, Lines:{2}", (cellCount - lineCount), numberOfTabs, numberOfLines);

        }

        private void VerifyCSV(string csvString, int lineCount, int cellCount)
        {
            int numberOfLines = GetRepeatCount(csvString, "\r\n");
            DRT.Assert(numberOfLines == lineCount, "{0} lines expected in CSV clipboard format", lineCount);
            int numberOfCommas = GetRepeatCount(csvString, ",");
            DRT.Assert(numberOfCommas == (cellCount - lineCount), "{0} commas expected in TEXT clipboard format. Found commas:{1}, Lines:{2}", (cellCount - lineCount), numberOfCommas, numberOfLines);
        }

        private int GetRepeatCount(string text, string repeatedString)
        {
            return text.Split(new string[] {repeatedString}, StringSplitOptions.None).Length - 1;
        }

        private void Column_GetCellContent()
        {
            Person row0 = (Person)_testGrid.Items[0];
            Person row1 = (Person)_testGrid.Items[1];
            DataGridColumn column0 = _testGrid.Columns[0];
            DataGridColumn column1 = _testGrid.Columns[1];

            TextBlock textBlock00 = column0.GetCellContent(row0) as TextBlock;
            DRT.Assert(textBlock00 != null, "Could not retrieve Cell Content 0,0");
            DRT.Assert(textBlock00.Text == row0.FirstName, "Value of cell 0,0 is not correct");

            TextBlock textBlock01 = column1.GetCellContent(row0) as TextBlock;
            DRT.Assert(textBlock01 != null, "Could not retrieve Cell Content 0,1");
            DRT.Assert(textBlock01.Text == row0.LastName, "Value of cell 0,1 is not correct");

            TextBlock textBlock10 = column0.GetCellContent(row1) as TextBlock;
            DRT.Assert(textBlock10 != null, "Could not retrieve Cell Content 1,0");
            DRT.Assert(textBlock10.Text == row1.FirstName, "Value of cell 1,0 is not correct");

            TextBlock textBlock11 = column1.GetCellContent(row1) as TextBlock;
            DRT.Assert(textBlock11 != null, "Could not retrieve Cell Content 1,1");
            DRT.Assert(textBlock11.Text == row1.LastName, "Value of cell 1,1 is not correct");
        }

        #endregion

        #region Data

        private bool _isPageLoaded;
        private DataGrid _testGrid;
        private ScrollViewer _scrollViewer;
        private Grid _mainGrid;    // parent of the datagrid
        private ComboBox _repeatCountChooser;
        private CheckBox _useIEditableObject;
        private CheckBox _checkBoxCopyHeaders;
        private CheckBox _checkBoxFreezeColumns;
        private ComboBox _newItemPlaceholderChooser;
        private Button _makeDetailsVisible;

        private ObservableCollection<Person> _people;
        private BindingBase _alternateBinding;
        private BindingBase _originalBinding;

        private int _committingEdit = 0;
        private int _cancelingEdit = 0;
        private int _beginningEdit = 0;
        private int _preparingCellForEdit = 0;

        private DataGridRow _editingRow;
        private DataGridColumn _editingColumn;
        private DataGridRow _commitRow;
        private DataGridColumn _commitColumn;

        private int _selectionChanged = 0;
        private int _selectedCellsChanged = 0;
        private IList _lastAddedItems;
        private IList _lastRemovedItems;
        private IList<DataGridCellInfo> _lastAddedCells;
        private IList<DataGridCellInfo> _lastRemovedCells;

        #endregion
    }

    public class Person : INotifyPropertyChanged
    {
        public Person() : this(String.Empty, String.Empty, String.Empty)
        {
        }

        public Person(string firstName, string lastName) : this(firstName, String.Empty, lastName)
        {
        }

        public Person(string firstName, string middleName, string lastName)
        {
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            LikesCake = true; // Everyone likes cake
            Id = s_globalId++;

            string prefix = String.Empty;
            if (!String.IsNullOrEmpty(firstName))
            {
                prefix += firstName.ToLower() + ".";
            }
            if (!String.IsNullOrEmpty(lastName))
            {
                prefix += lastName.ToLower() + ".";
            }
            prefix = prefix.Replace(' ', '_');
            Homepage = new Uri("http://" + prefix + "whitehouse.gov/");
        }

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    OnPropertyChanged("FirstName");
                }
            }
        }

        public string MiddleName
        {
            get { return _middleName; }
            set
            {
                if (_middleName != value)
                {
                    _middleName = value;
                    OnPropertyChanged("MiddleName");
                }
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (_lastName != value)
                {
                    _lastName = value;
                    OnPropertyChanged("LastName");
                }
            }
        }

        public bool LikesCake
        {
            get { return _likesCake; }
            set
            {
                if (_likesCake != value)
                {
                    _likesCake = value;
                    OnPropertyChanged("LikesCake");
                }
            }
        }

        public string Cake
        {
            get { return _cake; }
            set
            {
                if (_cake != value)
                {
                    _cake = value;
                    OnPropertyChanged("Cake");
                }
            }
        }

        public Uri Homepage
        {
            get
            {
                return _homepage;
            }
            set
            {
                if (_homepage != value)
                {
                    _homepage = value;
                    OnPropertyChanged("Homepage");
                }
            }
        }

        public override string ToString()
        {
            return _firstName + " " + _lastName;
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public override bool Equals(object o)
        {
            Person person = o as Person;
            if (person != null)
            {
                return person._id == _id &&
                    person._firstName.Equals(_firstName) &&
                    person._lastName.Equals(_lastName) &&
                    person._middleName.Equals(_middleName);
            }

            return false;
        }

        public int Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        public int ReadOnlyProperty
        {
            get { return 10; }
        }

        // Not thread safe
        public static void ResetGlobalId()
        {
            s_globalId = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        internal Person Copy()
        {
            Person person = new Person(_firstName, _middleName, _lastName);
            person.LikesCake = LikesCake;
            person.Cake = Cake;
            person.Homepage = Homepage;
            person.Id = Id;

            return person;
        }

        internal void Copy(Person person)
        {
            FirstName = person.FirstName;
            LastName = person.LastName;
            MiddleName = person.MiddleName;
            LikesCake = person.LikesCake;
            Cake = person.Cake;
            Homepage = person.Homepage;
            Id = person.Id;
        }

        private string _firstName, _lastName, _middleName;
        private bool _likesCake;
        private string _cake = String.Empty;
        private Uri _homepage = null;
        private int _id;

        private static int s_globalId = 0;   // not thread-safe
    }

    public class EditablePerson : Person, IEditableObject
    {
        public EditablePerson() : base()
        {
        }

        public EditablePerson(string firstName, string lastName) : base(firstName, lastName)
        {
        }

        public EditablePerson(string firstName, string middleName, string lastName) : base (firstName, middleName, lastName)
        {
        }

        public void BeginEdit()
        {
            if (!_isEditing)
            {
                _personCopy = Copy();
                _isEditing = true;
            }
        }

        public void CancelEdit()
        {
            if (_isEditing)
            {
                Copy(_personCopy);
                _personCopy = null;
                _isEditing = false;
            }
        }

        public void EndEdit()
        {
            if (_isEditing)
            {
                _personCopy = null;
                _isEditing = false;
            }
        }

        private bool _isEditing = false;
        private Person _personCopy;
    }

    public class FullNameConverter : IValueConverter
    {
        /// <summary>
        ///     Convert a person into full name
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fullName = string.Empty;

            if (value is Person)
            {
                var person = (Person)value;
                fullName = person.LastName + ", " + person.FirstName + " " + person.MiddleName;
            }

            if (targetType == typeof(string))
            {
                return fullName;
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        /// <summary>
        ///     Not implemented
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SillyNameRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = ValidationResult.ValidResult;
            BindingGroup bindingGroup = (BindingGroup)value;
            if (bindingGroup.Items.Count == 0)
                return result;
            Person person = (Person)bindingGroup.Items[0];

            object ofn, oln;
            string fn, ln;
            var result1 = bindingGroup.TryGetValue(person, "FirstName", out ofn);
            var result2 = bindingGroup.TryGetValue(person, "LastName", out oln);

            if (result1 && result2)
            {
                fn = (string)ofn;
                ln = (string)oln;

                if (fn == ln && fn != string.Empty)
                {
                    result = new ValidationResult(false, "That's a Silly name.");
                }
            }

            return result;
        }
    }

    public class JohnAdamsRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = ValidationResult.ValidResult;
            BindingGroup bindingGroup = (BindingGroup)value;
            if (bindingGroup.Items.Count == 0)
                return result;
            Person person = (Person)bindingGroup.Items[0];

            object ofn, oln;
            string fn, ln;
            var result1 = bindingGroup.TryGetValue(person, "FirstName", out ofn);
            var result2 = bindingGroup.TryGetValue(person, "LastName", out oln);

            if (result1 && result2)
            {
                fn = (string)ofn;
                ln = (string)oln;

                if (fn == "John" && ln == "Adams")
                {
                    result = new ValidationResult(false, "John Adams isn't a Person.  He's a robot!");
                }
            }

            return result;
        }
    }

    public class CakeData
    {
        public string Kind { get; set; }
        public string DisplayName { get; set; }
    }
}
