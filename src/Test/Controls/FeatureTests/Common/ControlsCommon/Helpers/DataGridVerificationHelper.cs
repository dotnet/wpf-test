using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Controls.Helpers
{
#if TESTBUILD_CLR40
    /// <summary>
    /// DataGrid Verification helper
    /// </summary>
    public static class DataGridVerificationHelper
    {
        /// <summary>
        /// Verifies the current cell edit mode against the parameters provided
        /// </summary>
        /// <param name="row">row of the cell to verify</param>
        /// <param name="col">column of cell to verify</param>
        /// <param name="isEditing">expected value for isEditing</param>
        /// <param name="verifyDataGridCellInfo">if true, will verify CurrentCell, CurrentItem, and CurrentColumn</param>
        public static void VerifyCurrentCellEditMode(
            DataGrid dataGrid,
            int row,
            int col,
            bool isEditingExpectedValue,
            bool verifyDataGridCellInfo,
            int newRow,
            int newCol)
        {
            DataGridCell currentCell = DataGridHelper.GetCell(dataGrid, row, col);

            // verify IsEditing
            if (currentCell.IsEditing != isEditingExpectedValue)
            {
                throw new TestValidationException(string.Format(
                    "Current cell: {0}, {1} expected IsEditing: {2} does not equal actual IsEditing: {3}",
                    row,
                    col,
                    isEditingExpectedValue,
                    currentCell.IsEditing));
            }

            if (verifyDataGridCellInfo)
            {
                DataGridCell newCurrentCell = DataGridHelper.GetCell(dataGrid, newRow, newCol);
                object expectedItem = DataGridHelper.GetRowDataItemFromCell(newCurrentCell);

                // verify CurrentCell item
                if (dataGrid.CurrentCell.Item != expectedItem)
                {
                    throw new TestValidationException(string.Format(
                        "Current cell: {0}, {1} expected CurrentCell.Item: {2} does not equal actual Item: {3}",
                        row,
                        col,
                        expectedItem,
                        dataGrid.CurrentCell.Item));
                }

                // verify CurrentCell Column
                if (dataGrid.CurrentCell.Column != currentCell.Column)
                {
                    throw new TestValidationException(string.Format(
                        "Current cell: {0}, {1} expected CurrentCell.Column: {2} does not equal actual Column: {3}",
                        row,
                        col,
                        currentCell.Column,
                        dataGrid.CurrentCell.Column));
                }

                // verify CurrentColumn
                if (dataGrid.CurrentColumn != currentCell.Column)
                {
                    throw new TestValidationException(string.Format(
                        "Current cell: {0}, {1} expected CurrentColumn: {2} does not equal actual Column: {3}",
                        row,
                        col,
                        currentCell.Column,
                        dataGrid.CurrentColumn));
                }

                // verify CurrentItem
                if (dataGrid.CurrentItem != expectedItem)
                {
                    throw new TestValidationException(string.Format(
                        "Current cell: {0}, {1} expected CurrentItem: {2} does not equal actual item: {3}",
                        row,
                        col,
                        expectedItem,
                        dataGrid.CurrentItem));
                }
            }
        }

        /// <summary>
        /// Verifies that the current row is either in editing mode or not
        /// </summary>
        public static void VerifyCurrentRowEditMode(DataGrid dataGrid, int row, bool isEditing)
        {
            DataGridRow curRow = DataGridHelper.GetRow(dataGrid, row);
            if (curRow.IsEditing != isEditing)
            {
                throw new TestValidationException(string.Format(
                    "Current row: {0}, expected IsEditing: {1} does not equal actual IsEditing: {2}",
                    row,
                    isEditing,
                    curRow.IsEditing));
            }

            IEditableCollectionView iecv = dataGrid.Items as IEditableCollectionView;
            Assert.AssertTrue("DataGrid.Items is not of type IEditableCollectionView", iecv != null);

            if (iecv.IsEditingItem != isEditing && iecv.IsAddingNew != isEditing)
            {
                throw new TestValidationException(string.Format(
                    "IEditableCollectionView: expected IsEditingItem: {0} does not equal actual IsEditingItem/IsAddingNew: {1}/{2}",
                    isEditing,
                    iecv.IsEditingItem,
                    iecv.IsAddingNew));
            }
        }

        /// <summary>
        /// Verifies the cell data given the expected data
        /// </summary>
        /// <param name="dataGrid">the datagrid under test</param>
        /// <param name="dataSource">the data source used with the datagrid</param>
        /// <param name="typeFromDataSource">the data type of the data source</param>
        /// <param name="row">the row of the cell to verify</param>
        /// <param name="col">the column of the cell to verify</param>
        /// <param name="expectedData">expected data to verify against</param>
        /// <param name="isEditing">if the cell is in edit mode or not</param>
        /// <param name="verifyDataSource">if the expectedData should also appear in the data item</param>
        /// <param name="GetDataFromTemplateColumn">delegate for custom retrieval of the data from a template column</param>
        /// <param name="GetDisplayBindingFromTemplateColumn">delegate for custom retrieval of the binding path from a template column</param>
        public static void VerifyCellData(
            DataGrid dataGrid,
            IEnumerable dataSource,
            Type typeFromDataSource,
            int row,
            int col,
            string expectedData,
            bool isEditing,
            bool verifyDataSource,
            DataGridHelper.GetDataFromTemplateColumn GetDataFromTemplateColumn,
            DataGridHelper.GetDisplayBindingFromTemplateColumn GetDisplayBindingFromTemplateColumn)
        {
            string actualDataFromCell = DataGridHelper.GetDataFromCell(
                dataGrid,
                row,
                col,
                isEditing,
                GetDataFromTemplateColumn);

            // verify cell data
            if (expectedData.ToLower() != actualDataFromCell.ToLower())
            {
                throw new TestValidationException(string.Format(
                    "Current cell: {0}, {1}\nexpected data: {2} does not equal actual data: {3}",
                    row,
                    col,
                    expectedData,
                    actualDataFromCell));
            }

            if (verifyDataSource)
            {
                object actualDataFromDS = DataGridHelper.GetDataFromDataSource(
                    dataGrid,
                    dataSource,
                    typeFromDataSource,
                    row,
                    col,
                    GetDisplayBindingFromTemplateColumn);
                string actualDataFromDSString = actualDataFromDS.ToString();

                // verify the data source
                if (expectedData.ToLower() != actualDataFromDSString.ToLower())
                {
                    throw new TestValidationException(string.Format(
                        "Current cell: {0}, {1}\nexpected data: {2} does not equal actual data: {3}",
                        row,
                        col,
                        expectedData,
                        actualDataFromDSString));
                }
            }
        }

        /// <summary>
        /// Verifies columns are resized correctly
        /// </summary>
        /// <param name="previousWidths">the previous widths before any action was taken</param>
        /// <param name="colIndexToResize">the column to verify</param>
        /// <param name="resizeDiff">what the resize should be</param>
        public static void VerifyColumnResizing(DataGrid dataGrid, List<double> previousWidths, int colIndexToResize, double resizeDiff)
        {
            //
            // Calculate expected widths
            //

            // get total width
            double totalWidth = 0;
            foreach (double width in previousWidths)
                totalWidth += width;

            // calculate width left for Star sized columns
            int numStarColumns = 0;
            double totalSizeLeft = totalWidth;
            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                if (i == colIndexToResize)
                {
                    totalSizeLeft -= previousWidths[i] + resizeDiff;
                }
                else
                {
                    DataGridColumn column = DataGridHelper.GetColumn(dataGrid, i);
                    if (!column.Width.IsStar)
                        totalSizeLeft -= previousWidths[i];
                    else
                        numStarColumns++;
                }
            }

            // calculate the expected widths
            double epsilon = 4;
            List<double> expectedWidths = new List<double>();
            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                if (i == colIndexToResize)
                {
                    expectedWidths.Add(previousWidths[i] + resizeDiff - epsilon);
                }
                else
                {
                    DataGridColumn column = DataGridHelper.GetColumn(dataGrid, i);
                    if (!column.Width.IsStar)
                        expectedWidths.Add(previousWidths[i]);
                    else
                        expectedWidths.Add((totalSizeLeft / numStarColumns) + epsilon);
                }
            }

            // get the actual widths
            List<double> actualHeaderWidths = DataGridHelper.GetColumnHeaderWidths(dataGrid);
            List<double> actualColumnWidths = DataGridHelper.GetColumnWidths(dataGrid);

            TestLog.Current.LogStatus("VerifyColumnResizing(): Begin verifying resized columns");
            for (int j = 0; j < actualHeaderWidths.Count; j++)
            {
                if (!DataGridHelper.AreClose(expectedWidths[j], actualColumnWidths[j], DataGridHelper.ColumnWidthFuzziness))
                {
                    DataGridVerificationHelper.DumpColumnWidths(expectedWidths, actualColumnWidths, actualHeaderWidths);
                    throw new TestValidationException(string.Format(
                        "VerifyColumnResizing(): Index changed: {0}, Current index position: {1}, Expected column width: {2}, actual column width: {3}",
                        colIndexToResize,
                        j,
                        expectedWidths[j],
                        actualColumnWidths[j]));
                }
                if (!DataGridHelper.AreClose(expectedWidths[j], actualHeaderWidths[j], DataGridHelper.ColumnWidthFuzziness))
                {
                    DataGridVerificationHelper.DumpColumnWidths(expectedWidths, actualColumnWidths, actualHeaderWidths);
                    throw new TestValidationException(string.Format(
                        "VerifyColumnResizing(): Index changed: {0}, Current index position: {1}, Expected header width: {2}, actual header width: {3}",
                        colIndexToResize,
                        j,
                        expectedWidths[j],
                        actualHeaderWidths[j]));
                }
            }
            TestLog.Current.LogStatus("VerifyColumnResizing(): End verifying resized columns");
        }

        /// <summary>
        /// Verifies that a column is of a particular DataGridLengthUnitType
        /// </summary>
        /// <param name="colIndex">the column index to verify</param>
        /// <param name="expectedType">the expected DataGridLengthUnitType</param>
        public static void VerifyColumnWidthType(DataGrid dataGrid, int colIndex, DataGridLengthUnitType expectedType)
        {
            DataGridColumn column = DataGridHelper.GetColumn(dataGrid, colIndex);

            if (expectedType != column.Width.UnitType)
            {
                throw new TestValidationException(string.Format(
                    "Column Index: {0}, Expected type: {1}, Actual type: {2}",
                    colIndex,
                    expectedType.ToString(),
                    column.Width.UnitType.ToString()));
            }
        }

        /// <summary>
        /// Verifies that the DisplayIndexMap maps correctly to the column.DisplayIndex
        /// </summary>
        public static void VerifyDisplayIndices(DataGrid dataGrid)
        {
            //DEBUG: For visual debugging purposes, uncomment these two lines and set a break point on them
            //_dataGrid.UpdateLayout();
            //this.WaitForPriority(DispatcherPriority.ApplicationIdle);

            List<int> displayIndexMap = DataGridHelper.GetDisplayIndexMap(dataGrid);

            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                string errorMsg = string.Format(
                    "Column[i = {0}].DisplayIndex == {1}.  DisplayIndexMap[j = {2}]  == {3}. 'i = {0}' should be equal to DisplayIndexMap[j].",
                    i,
                    dataGrid.Columns[i].DisplayIndex,
                    dataGrid.Columns[i].DisplayIndex,
                    displayIndexMap[dataGrid.Columns[i].DisplayIndex]);

                Assert.AssertTrue(errorMsg, displayIndexMap[dataGrid.Columns[i].DisplayIndex] == i);
            }
        }

        public static void VerifyHeaderProperties(
            ButtonBase header,
            Brush expectedHeaderBackground,
            bool isExpectedContentOverrideByTemplate,
            string expectedContent)
        {
            VerifyHeaderProperties(
                header,
                expectedHeaderBackground,
                isExpectedContentOverrideByTemplate,
                expectedContent,
                null,
                null);
        }

        /// <summary>
        /// Verifies header properties are correct based on the expected parameters.  This is
        /// used specifically with header styling tests.
        /// </summary>
        /// <param name="header">the header to verify</param>
        /// <param name="expectedHeaderBackground">expected header background</param>
        /// <param name="isExpectedContentOverrideByTemplate">if true, will verify the content of the template, otherwise will verify header.Content</param>
        /// <param name="expectedContent">the expected content</param>
        /// <param name="expectedContentTemplateBackground">expected item background</param>
        /// <param name="expectedContentTemplateForeground">expected item foreground</param>
        public static void VerifyHeaderProperties(
            ButtonBase header,
            Brush expectedHeaderBackground,
            bool isExpectedContentOverrideByTemplate,
            string expectedContent,
            Brush expectedContentTemplateBackground,
            Brush expectedContentTemplateForeground)
        {
            if (expectedHeaderBackground != null)
            {
                if (header.Background != expectedHeaderBackground)
                {
                    throw new TestValidationException(string.Format(
                        "Expected background brush: {0}, Actual background brush: {1}",
                        expectedHeaderBackground.ToString(),
                        header.Background.ToString()));
                }
            }

            if (!isExpectedContentOverrideByTemplate)
            {
                if (!string.IsNullOrEmpty(expectedContent) && header.Content == null)
                {
                    throw new TestValidationException(string.Format(
                            "Expected header content: {0}, Actual header content: null",
                            expectedContent));
                }
                else if (header.Content != null)
                {
                    if (header.Content.ToString() != expectedContent)
                    {
                        throw new TestValidationException(string.Format(
                            "Expected header content: {0}, Actual header content: {1}",
                            expectedContent,
                            header.Content.ToString()));
                    }
                }
            }

            if (header.ContentTemplate != null)
            {
                TextBlock textBlock = DataGridHelper.FindVisualChild<TextBlock>(header);
                if (expectedContentTemplateBackground != null)
                {
                    if (textBlock.Background != expectedContentTemplateBackground)
                    {
                        throw new TestValidationException(string.Format(
                            "Expected foreground brush: {0}, Actual background brush: {1}",
                            expectedContentTemplateBackground.ToString(),
                            textBlock.Background.ToString()));
                    }
                }

                if (expectedContentTemplateForeground != null)
                {
                    if (textBlock.Foreground != expectedContentTemplateForeground)
                    {
                        throw new TestValidationException(string.Format(
                            "Expected foreground brush: {0}, Actual background brush: {1}",
                            expectedContentTemplateForeground.ToString(),
                            textBlock.Foreground.ToString()));
                    }
                }

                if (isExpectedContentOverrideByTemplate)
                {
                    if (textBlock.Text != expectedContent)
                    {
                        throw new TestValidationException(string.Format(
                            "Expected header content: {0}, Actual header content: {1}",
                            expectedContent,
                            textBlock.Text));
                    }
                }
            }
        }

        public static void VerifyDisplayOrder(DataGrid dataGrid, ObservableCollection<DataGridColumn> expectedColumns)
        {
            // get the new display index map
            List<int> newDisplayIndexMap = DataGridHelper.GetDisplayIndexMap(dataGrid);

            // verify the order with the new display index map
            for (int i = 0; i < expectedColumns.Count; i++)
            {
                if (expectedColumns[i] != dataGrid.Columns[newDisplayIndexMap[i]])
                {
                    DataGridVerificationHelper.DumpColumns(expectedColumns, "Expected output:============");
                    DataGridVerificationHelper.DumpColumns(dataGrid.Columns, "Actual output:============");

                    string errorMsg = string.Format(
                        "cur index: {0}, ExpectedColumn: {1}, Actual Column: {2}",
                        i,
                        expectedColumns[i],
                        dataGrid.Columns[newDisplayIndexMap[i]]);
                    throw new TestValidationException(errorMsg);
                }
            }

            // verify the display indices match the column display indices
            DataGridVerificationHelper.VerifyDisplayIndices(dataGrid);
        }


    #region OM and DPs Validation Helpers

        /// <summary>
        /// int DP value tester
        /// </summary>
        /// <param name="d"></param>
        /// <param name="dpString"></param>
        public static void ValidateIntValue(DependencyObject d, DependencyProperty dp)
        {
            // test valid positive values
            d.SetValue(dp, 0);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} to 0", dp.ToString()), 0, d.GetValue(dp));

            d.SetValue(dp, 2);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} to 2", dp.ToString()), 2, d.GetValue(dp));
        }

        /// <summary>
        ///  double DP value tester
        /// </summary>
        /// <param name="d"></param>
        /// <param name="dpString"></param>
        public static void ValidateDoubleValue(DependencyObject d, DependencyProperty dp)
        {
            // test a valid positive value
            d.SetValue(dp, 25.0);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} to 25.0", dp.ToString()), 25.0, d.GetValue(dp));

            // test the positiveInfinite
            d.SetValue(dp, double.PositiveInfinity);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} to double.PositiveInfinity", dp.ToString()), double.PositiveInfinity, d.GetValue(dp));
        }

        /// <summary>
        /// a Bool DP value tester
        /// </summary>
        /// <param name="d"></param>
        /// <param name="dp"></param>
        public static void ValidateBoolenValue(DependencyObject d, DependencyProperty dp)
        {
            d.SetValue(dp, true);
            QueueHelper.WaitTillQueueItemsProcessed();
            Assert.AssertEqual(string.Format("Error in setting the DP {0} to true", dp), true, d.GetValue(dp));

            d.SetValue(dp, false);
            QueueHelper.WaitTillQueueItemsProcessed();
            Assert.AssertEqual(string.Format("Error in setting the DP {0} to false", dp), false, d.GetValue(dp));

            d.SetValue(dp, true);
            QueueHelper.WaitTillQueueItemsProcessed();
            Assert.AssertEqual(string.Format("Error in setting the DP {0} back to true", dp), true, d.GetValue(dp));
        }

        /// <summary>
        /// Brush DP value tester
        /// </summary>
        /// <param name="d"></param>
        /// <param name="dp"></param>
        public static void ValidateBrushValue(DependencyObject d, DependencyProperty dp)
        {
            d.SetValue(dp, Brushes.Red);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} to Red", dp), Brushes.Red, d.GetValue(dp));

            d.SetValue(dp, Brushes.Blue);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} to Blue", dp), Brushes.Blue, d.GetValue(dp));

            d.SetValue(dp, Brushes.Red);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} back to Red", dp), Brushes.Red, d.GetValue(dp));
        }

        /// <summary>
        /// Style DP value tester
        /// </summary>
        /// <param name="d"></param>
        /// <param name="dp"></param>
        public static void ValidateStyleValue(DependencyObject d, DependencyProperty dp)
        {
            Style style1 = new Style();
            Style style2 = new Style();

            d.SetValue(dp, style1);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} to style1", dp.ToString()), style1, d.GetValue(dp));

            d.SetValue(dp, style2);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} to style2", dp.ToString()), style2, d.GetValue(dp));

            d.SetValue(dp, style1);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} back to style1", dp.ToString()), style1, d.GetValue(dp));
        }

        /// <summary>
        /// DataTemplate DP value tester
        /// </summary>
        /// <param name="d"></param>
        /// <param name="dp"></param>
        public static void ValidateDataTemplateValue(DependencyObject d, DependencyProperty dp)
        {
            DataTemplate template1 = new DataTemplate();
            DataTemplate template2 = new DataTemplate();

            d.SetValue(dp, template1);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} to template1", dp), template1, d.GetValue(dp));

            d.SetValue(dp, template2);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} to template2", dp), template2, d.GetValue(dp));

            d.SetValue(dp, template1);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} back to template1", dp), template1, d.GetValue(dp));
        }

        /// <summary>
        /// DataTemplateSelector DP tester
        /// </summary>
        /// <param name="d"></param>
        /// <param name="dp"></param>
        public static void ValidateDataTemplateSelectorValue(DependencyObject d, DependencyProperty dp)
        {
            DataTemplateSelector template1 = new DataTemplateSelector();
            DataTemplateSelector template2 = new DataTemplateSelector();

            d.SetValue(dp, template1);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} to template1", dp), template1, d.GetValue(dp));

            d.SetValue(dp, template2);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} to template2", dp), template2, d.GetValue(dp));

            d.SetValue(dp, template1);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} back to template1", dp), template1, d.GetValue(dp));
        }

        /// <summary>
        /// ControlTemplate tester
        /// </summary>
        /// <param name="d"></param>
        /// <param name="dp"></param>
        public static void ValidateControlTemplateValue(DependencyObject d, DependencyProperty dp)
        {
            ControlTemplate template1 = new ControlTemplate();
            ControlTemplate template2 = new ControlTemplate();

            d.SetValue(dp, template1);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} to control template1", dp), template1, d.GetValue(dp));

            d.SetValue(dp, template2);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} to control template2", dp), template2, d.GetValue(dp));

            d.SetValue(dp, template1);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} back to control template1", dp), template1, d.GetValue(dp));
        }

        /// <summary>
        /// Enum DP tester
        /// </summary>
        /// <param name="d"></param>
        /// <param name="dp"></param>
        /// <param name="enumType"></param>
        public static void ValidateEnumValue(DependencyObject d, DependencyProperty dp, Type enumType)
        {
            if (enumType.IsEnum)
            {
                foreach (Enum e in Enum.GetValues(enumType))
                {
                    // set the value
                    d.SetValue(dp, e);
                    // verify the set
                    Assert.AssertEqual(string.Format("{0} did not set to {1}", dp.ToString(), e), e, d.GetValue(dp));
                }
            }
        }

        /// <summary>
        /// DataGridLength DP tester
        /// </summary>
        /// <param name="d"></param>
        /// <param name="dp"></param>
        public static void ValidateDataGridLengthValue(DependencyObject d, DependencyProperty dp)
        {
            d.SetValue(dp, new DataGridLength(5.0));
            Assert.AssertEqual(string.Format("Error in setting the DP {0} to 5.0", dp), new DataGridLength(5.0), d.GetValue(dp));

            d.SetValue(dp, DataGridLength.Auto);
            Assert.AssertTrue(string.Format("Error in setting the DP {0} to Auto", dp), ((DataGridLength)d.GetValue(dp)).IsAuto);

            d.SetValue(dp, DataGridLength.SizeToCells);
            Assert.AssertTrue(string.Format("Error in setting the DP {0} to SizeToCells", dp), ((DataGridLength)d.GetValue(dp)).IsSizeToCells);

            d.SetValue(dp, DataGridLength.SizeToHeader);
            Assert.AssertTrue(string.Format("Error in setting the DP {0} to SizeToHeader", dp), ((DataGridLength)d.GetValue(dp)).IsSizeToHeader);

            d.SetValue(dp, DataGridLength.Auto);
            Assert.AssertTrue(string.Format("Error in setting the DP {0} back to Auto", dp), ((DataGridLength)d.GetValue(dp)).IsAuto);
        }

        /// <summary>
        /// object DP tester
        /// </summary>
        /// <param name="d"></param>
        /// <param name="dp"></param>
        public static void ValidateObjectValue(DependencyObject d, DependencyProperty dp)
        {
            Button button1 = new Button();
            Button button2 = new Button();

            d.SetValue(dp, button1);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} to button1", dp.ToString()), button1, d.GetValue(dp));

            d.ClearValue(dp);
            QueueHelper.WaitTillQueueItemsProcessed();

            d.SetValue(dp, button2);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} to button2", dp.ToString()), button2, d.GetValue(dp));

            d.ClearValue(dp);
             QueueHelper.WaitTillQueueItemsProcessed();

            d.SetValue(dp, button1);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} back to button1", dp.ToString()), button1, d.GetValue(dp));
        }

        /// <summary>
        /// Nullable ListSortDirection tester
        /// </summary>
        /// <param name="d"></param>
        /// <param name="dp"></param>
        public static void ValidateSortDirectionValue(DependencyObject d, DependencyProperty dp)
        {
            d.SetValue(dp, ListSortDirection.Descending);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} to str1", dp.ToString()), ListSortDirection.Descending, d.GetValue(dp));

            d.ClearValue(dp);
            QueueHelper.WaitTillQueueItemsProcessed();

            d.SetValue(dp, ListSortDirection.Ascending);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} to str2", dp.ToString()), ListSortDirection.Ascending, d.GetValue(dp));

            d.ClearValue(dp);
            QueueHelper.WaitTillQueueItemsProcessed();

            d.SetValue(dp, null);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} BACK to str1", dp.ToString()), null, d.GetValue(dp));
        }


        /// <summary>
        /// string DP tester
        /// </summary>
        /// <param name="d"></param>
        /// <param name="dp"></param>
        public static void ValidateStringValue(DependencyObject d, DependencyProperty dp)
        {
            string str1 = "First Name";
            string str2 = "Home Page";

            d.SetValue(dp, str1);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} to str1", dp.ToString()), str1, d.GetValue(dp));

            d.ClearValue(dp);
            QueueHelper.WaitTillQueueItemsProcessed();

            d.SetValue(dp, str2);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} to str2", dp.ToString()), str2, d.GetValue(dp));

            d.ClearValue(dp);
            QueueHelper.WaitTillQueueItemsProcessed();

            d.SetValue(dp, str1);
            Assert.AssertEqual(string.Format("Error in setting the DP {0} BACK to str1", dp.ToString()), str1, d.GetValue(dp));
        }

        /// <summary>
        /// The basic DP tests method
        /// </summary>
        /// <param name="d">DO to eval against</param>
        /// <param name="dpString">DP string</param>
        /// <param name="local">DPPropertyLocal struct</param>
        /// <param name="callBackMethod">the callback method if any</param>
        public static void TestDP<d, T>(String dpString, DPPropertyLocal local, string callBackMethod) where d : DependencyObject
        {
            Type propertyType = typeof(T);

            string propToEval = dpString;

            // Verify DP Property member
            FieldInfo fieldInfo = typeof(d).GetField(dpString + "Property", BindingFlags.Static | BindingFlags.Public);
            Assert.AssertEqual(propToEval + "Property not expected type 'DependencyProperty'.", typeof(DependencyProperty), fieldInfo.FieldType);

            // Verify DP Property's value type
            DependencyProperty property = fieldInfo.GetValue(null) as DependencyProperty;
            Assert.AssertTrue(propToEval + " value type is wrong", property != null);

            // Verify DP CLR property member
            PropertyInfo propertyInfo = typeof(d).GetProperty(dpString, BindingFlags.Instance | BindingFlags.Public);
            Assert.AssertTrue("Expected CLR property " + propToEval + " does not exist.", propertyInfo != null);
            Assert.AssertEqual(string.Format(propToEval + " not expected type '{0}'", propertyType.GetType().ToString()), propertyType, propertyInfo.PropertyType);

            // Verify getter/setter access
            Assert.AssertEqual("Unexpected value for propertyInfo.CanRead.", local.expectGet, propertyInfo.CanRead);
            Assert.AssertEqual("Unexpected value for propertyInfo.CanWrite.", local.expectSet, propertyInfo.CanWrite);

            // Verify DP callback
            if (local.hasCallBack)
            {
                MethodInfo methodInfo = typeof(d).GetMethod(callBackMethod, BindingFlags.Static | BindingFlags.NonPublic);
                Assert.AssertTrue("Expected " + propToEval + " to have static, non-public side-effect callback '" + callBackMethod + "'.", methodInfo != null);
            }
            else
            {
                MethodInfo methodInfo = typeof(d).GetMethod(callBackMethod, BindingFlags.Static | BindingFlags.NonPublic);
                Assert.AssertTrue("Expected " + propToEval + " NOT to have static side-effect callback '" + callBackMethod + "'.", methodInfo == null);
            }

            // 

            // Verify DP default value???
            // 

        }

        public static void TestSetterGetterDP(DependencyObject d, string dpString, object valueToSet) 
        {
            // set property to value
            d.GetType().InvokeMember(dpString, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, null, d, new object[] { valueToSet });
            var valueToGet = d.GetType().InvokeMember(dpString, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, null, d, null);

            Assert.AssertEqual(string.Format("mismatch in values.  Expected: {0}, Actual: {1}", valueToSet, valueToGet), valueToSet, valueToGet);
        }

        #endregion


    #region Logging Helpers

        public static void DumpColumnWidths(
            List<double> expected,
            List<double> actualColumnWidths,
            List<double> actualHeaderWidths)
        {
            for (int i = 0; i < expected.Count; i++)
            {
                TestLog.Current.LogEvidence(string.Format(
                    "ExpectedWidth[{0}]: {1}, ActualColumnWidth[{0}]: {2}, ActualHeaderWidth[{0}]: {3}",
                    i,
                    expected[i],
                    actualColumnWidths[i],
                    actualHeaderWidths[i]));
            }
        }

        public static void DumpColumns(IList<DataGridColumn> columns, string header)
        {
            TestLog.Current.LogEvidence(header);
            for (int i = 0; i < columns.Count; i++)
            {
                TestLog.Current.LogEvidence(string.Format(
                    "index: {0}, Column.Header: {1}, Column.DisplayIndex: {2}",
                    i,
                    columns[i].Header,
                    columns[i].DisplayIndex));
            }
            TestLog.Current.LogEvidence("End Column output:============\n");
        }

        #endregion Logging Helpers
    }

    /// <summary>
    /// DP test struct
    /// </summary>
    public struct DPPropertyLocal
    {
        public bool expectGet;
        public bool expectSet;
        public bool hasCallBack;
        //public object defaultValue; // 
    }
#endif
}
