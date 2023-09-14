using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

using System.Collections;
using System;
using Microsoft.Test.Layout;
using System.Windows.Media;
using System.Windows;
using System.Linq;
using Avalon.Test.ComponentModel;
using System.Text;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Base class for testing end-user column width resizing for star columns.
    /// </description>

    /// </summary>
    public class DataGridColumnUserResizingStarBase : DataGridTest
    {
        #region Constructor

        public DataGridColumnUserResizingStarBase(string filename)
            : base(filename)
        {
        }

        #endregion

        #region General Helpers

        /// <summary>
        /// Gets a list of ColumnWidthData in the DataGrid
        /// </summary>
        protected List<DataGridHelper.ColumnWidthData> GetAllColumnData()
        {
            List<DataGridHelper.ColumnWidthData> colInfoList = new List<DataGridHelper.ColumnWidthData>();
            for (int i = 0; i < MyDataGrid.Columns.Count; i++)
            {
                DataGridHelper.ColumnWidthData colInfo;
                DataGridHelper.GetColumnWidthData(MyDataGrid, i, out colInfo, true);
                colInfoList.Add(colInfo);
            }
            return colInfoList;
        }

        protected bool HasStarColumns()
        {
            double perStarWidth;
            return HasStarColumns(out perStarWidth);
        }

        protected bool HasStarColumns(
            out double perStarWidth)
        {
            bool retVal = false;
            perStarWidth = 0.0;

            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                if (column.Width.IsStar)
                {
                    // compute per star width
                    perStarWidth = column.Width.DesiredValue / column.Width.Value;

                    retVal = true;
                    break;
                }
            }
            return retVal;
        }

        protected double GetViewportWidthForColumns()
        {
            ScrollViewer sv = DataGridHelper.FindVisualChild<ScrollViewer>(MyDataGrid);
            double totalAvailableWidth = sv.ViewportWidth;

            DataGridCell cell = DataGridHelper.GetCell(MyDataGrid, 0, 0);
            totalAvailableWidth -= GetParentCellsPanelOffset(cell);

            return totalAvailableWidth;
        }

        private double GetParentCellsPanelOffset(
            DataGridCell cell)
        {
            double retVal = 0.0;

            DataGridCellsPanel panel = System.Windows.Media.VisualTreeHelper.GetParent((UIElement)cell) as DataGridCellsPanel;
            ItemsControl cellsPanelParent = (panel.TemplatedParent as FrameworkElement).TemplatedParent as ItemsControl;
            Visual dataGridRow = cellsPanelParent.TemplatedParent as Visual;
            retVal = panel.TransformToAncestor(dataGridRow).Transform(new Point()).X;

            return retVal;
        }

        /// <summary>
        /// Given an expected list of ColumnWidthData, will compare each to the actual column widths
        /// </summary>
        protected void VerifyColumnWidths(
            DataGridHelper.ColumnWidthData[] expectedColumns,
            double epsilon)
        {
            StringBuilder sb = new StringBuilder();

            // make sure expectedColumns meet their min/max widths
            for (int i = 0; i < expectedColumns.Length; i++)
            {
                expectedColumns[i].displayWidth = Math.Max(expectedColumns[i].displayWidth, expectedColumns[i].minWidth);
                expectedColumns[i].displayWidth = Math.Min(expectedColumns[i].displayWidth, expectedColumns[i].maxWidth);
            }

            //
            // Verify against the actual data
            //
            int idx = 0;
            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                StringBuilder tempsb = new StringBuilder();
                bool hasError = false;

                if (column.Width.UnitType != expectedColumns[idx].unitType)
                {
                    hasError = true;
                    tempsb.Append(string.Format(
                        "Column: {2}, width unit type do not match: Expected: {0}, Actual: {1}",
                        expectedColumns[idx].unitType,
                        column.Width.UnitType,
                        idx));
                    tempsb.Append(Environment.NewLine);
                }

                if (column.Width.UnitType == DataGridLengthUnitType.Pixel)
                {
                    if (!DataGridHelper.AreClose(column.Width.Value, expectedColumns[idx].width, epsilon + 0.1))
                    {
                        hasError = true;
                        tempsb.Append(string.Format(
                            "Column: {2}, widths do not match: Expected: {0}, Actual: {1}",
                            expectedColumns[idx].width,
                            column.Width.Value,
                            idx));
                        tempsb.Append(Environment.NewLine);
                    }
                }
                else if (column.Width.UnitType == DataGridLengthUnitType.Star)
                {
                    if (!DataGridHelper.AreClose(column.Width.Value, expectedColumns[idx].width, epsilon + 0.1))
                    {
                        hasError = true;
                        tempsb.Append(string.Format(
                            "Column: {2}, widths do not match: Expected: {0}, Actual: {1}",
                            expectedColumns[idx].width,
                            column.Width.Value,
                            idx));
                        tempsb.Append(Environment.NewLine);
                    }
                }

                if (!DataGridHelper.AreClose(column.Width.DisplayValue, expectedColumns[idx].displayWidth, epsilon + 0.1))
                {
                    hasError = true;
                    tempsb.Append(string.Format(
                        "Column: {2}, display widths do not match: Expected: {0}, Actual: {1}",
                        expectedColumns[idx].displayWidth,
                        column.Width.DisplayValue,
                        idx));
                    tempsb.Append(Environment.NewLine);
                }
                double expectedActualWidth = Math.Max(expectedColumns[idx].displayWidth, expectedColumns[idx].minWidth);
                expectedActualWidth = Math.Min(expectedColumns[idx].maxWidth, expectedActualWidth);

                if (!DataGridHelper.AreClose(column.ActualWidth, expectedActualWidth, epsilon + 0.1))
                {
                    hasError = true;
                    tempsb.Append(string.Format(
                        "Column: {2}, Acutal widths do not match: Expected: {0}, Actual: {1}",
                        expectedActualWidth,
                        column.ActualWidth,
                        idx));
                    tempsb.Append(Environment.NewLine);
                }
                //









                // now verify that Width is correctly coerced to Min/Max
                if (DoubleUtil.LessThan(column.Width.DisplayValue, column.MinWidth))
                {
                    hasError = true;
                    tempsb.Append(string.Format(
                        "Column: {2}, DisplayValue: {0}, is less than MinWidth: {1}",
                        column.Width.DisplayValue,
                        column.MinWidth,
                        idx));
                    tempsb.Append(Environment.NewLine);
                }
                if (DoubleUtil.GreaterThan(column.Width.DisplayValue, column.MaxWidth))
                {
                    hasError = true;
                    tempsb.Append(string.Format(
                        "Column: {2}, DisplayValue: {0}, is greater than MaxWidth: {1}",
                        column.Width.DisplayValue,
                        column.MaxWidth,
                        idx));
                    tempsb.Append(Environment.NewLine);
                }

                if (hasError)
                {
                    sb.Append(Environment.NewLine);
                    sb.Append(tempsb.ToString());
                    sb.Append(Environment.NewLine);
                }
                idx++;
            }

            if (sb.Length > 0)
            {
                throw new TestValidationException(sb.ToString());
            }
        }

        /// <summary>
        /// Unused space is based on ViewportWidth - total display widths for columns
        /// </summary>
        /// <param name="expectedColumns">the expect column values that are considered in the calculation</param>
        /// <param name="takeAwayWidth">the current width that can be taken away</param>
        /// <param name="widthAlreadyUtilized">if the width has already been realized by the star column calcualtion engine</param>
        /// <returns></returns>
        protected double GetUnusedSpaceFromDataGrid(
            DataGridHelper.ColumnWidthData[] expectedColumns,
            double takeAwayWidth,
            bool widthAlreadyUtilized)
        {
            double unusedSpace = 0.0;

            double totalAvailableWidth = GetViewportWidthForColumns();
            if (DoubleUtil.GreaterThan(totalAvailableWidth, 0.0))
            {
                double usedSpace = 0.0;
                foreach (DataGridHelper.ColumnWidthData colWidth in expectedColumns)
                {
                    usedSpace += colWidth.displayWidth;
                }

                if (widthAlreadyUtilized)
                {
                    if (DoubleUtil.GreaterThanOrClose(totalAvailableWidth, usedSpace))
                    {
                        return 0.0;
                    }
                    else
                    {
                        return Math.Min(usedSpace - totalAvailableWidth, takeAwayWidth);
                    }
                }
                else
                {
                    unusedSpace = totalAvailableWidth - usedSpace;
                    if (DoubleUtil.GreaterThan(unusedSpace, 0.0))
                    {
                        takeAwayWidth = Math.Max(0.0, takeAwayWidth - unusedSpace);
                    }

                    if (takeAwayWidth != 0.0)
                    {
                        return takeAwayWidth;
                    }
                    else
                    {
                        return unusedSpace;
                    }
                }
            }

            return unusedSpace;
        }

        private int IndexOfColumn(
            DataGridHelper.ColumnWidthData[] expectedColumns,
            DataGridHelper.ColumnWidthData searchColumn)
        {
            int retVal = -1;
            int i = 0;
            foreach (DataGridHelper.ColumnWidthData column in expectedColumns)
            {
                if (column.Equals(searchColumn))
                {
                    retVal = i;
                    break;
                }
                i++;
            }

            return retVal;
        }

        private string DisplayActualColumnWidths()
        {
            StringBuilder tempsb = new StringBuilder();

            int i = 0;
            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                tempsb.Append(string.Format(
                    "actual column: {0}, UnitType: {1}, Width.Value: {2}, DisplayValue: {3}, DesiredValue: {4}, MinWidth: {5}, MaxWidth: {6}",
                    i,
                    column.Width.UnitType,
                    column.Width.Value,
                    column.Width.DisplayValue,
                    column.Width.DesiredValue,
                    column.MinWidth,
                    column.MaxWidth,
                    column.ActualWidth));
                tempsb.Append(Environment.NewLine);
                i++;
            }

            return tempsb.ToString();
        }

        private string DisplayExpectedColumnWidths(
            DataGridHelper.ColumnWidthData[] expectedColumns)
        {
            StringBuilder tempsb = new StringBuilder();

            int i = 0;
            foreach (DataGridHelper.ColumnWidthData column in expectedColumns)
            {
                tempsb.Append(string.Format(
                    "expected column: {0}, UnitType: {1}, Width.Value: {2}, DisplayValue: {3}, DesiredValue: {4}, MinWidth: {5}, MaxWidth: {6}",
                    i,
                    expectedColumns[i].unitType,
                    expectedColumns[i].width,
                    expectedColumns[i].displayWidth,
                    expectedColumns[i].desiredWidth,
                    expectedColumns[i].minWidth,
                    expectedColumns[i].maxWidth));
                tempsb.Append(Environment.NewLine);
                i++;
            }

            return tempsb.ToString();
        }

        #endregion General Helpers

        #region Resizing Helpers

        /// <summary>
        /// Convenience method for computing and verifying column widths on Width resize
        /// </summary>
        /// <param name="prevColumns">the state before the resize</param>
        /// <param name="expectedColumns">the expected state that will be computed here</param>
        /// <param name="indexToResize">the column index that is resized</param>
        /// <param name="resizeDiff">the resize difference</param>
        /// <param name="resizeRight">direction</param>
        /// <param name="epsilon">error</param>
        protected void ComputeAndVerifyColumnWidthsOnResize(
            List<DataGridHelper.ColumnWidthData> prevColumns,
            out DataGridHelper.ColumnWidthData[] expectedColumns,
            int indexToResize,
            double resizeDiff,
            bool resizeRight,
            double epsilon)
        {
            expectedColumns = ComputeExpectedColumnsWidthOnResize(prevColumns, indexToResize, resizeDiff, resizeRight, epsilon);
            QueueHelper.WaitTillQueueItemsProcessed();

            try
            {
                VerifyColumnWidths(expectedColumns, epsilon);
            }
            catch (Exception ex)
            {
                // dump previous, expected and actual column data
                StringBuilder sb = new StringBuilder();
                sb.Append("Previous column data===============: " + Environment.NewLine);
                sb.Append(DisplayExpectedColumnWidths(prevColumns.ToArray()));
                sb.Append("Expected column data===============: " + Environment.NewLine);
                sb.Append(DisplayExpectedColumnWidths(expectedColumns));
                sb.Append("Actual column data===============: " + Environment.NewLine);
                sb.Append(DisplayActualColumnWidths());
                LogComment(sb.ToString());

                throw ex;
            }
        }

        protected DataGridHelper.ColumnWidthData[] ComputeExpectedColumnsWidthOnResize(
            List<DataGridHelper.ColumnWidthData> prevColumns,
            int indexToResize,
            double resizeDiff,
            bool resizeRight,
            double epsilon)
        {
            // initial setup
            DataGridHelper.ColumnWidthData colToResize = prevColumns[indexToResize];
            DataGridHelper.ColumnWidthData[] expectedColumns = new DataGridHelper.ColumnWidthData[prevColumns.Count];
            prevColumns.CopyTo(expectedColumns);

            if (!MyDataGrid.Columns[indexToResize].CanUserResize)
            {
                LogComment(string.Format("column: {0}, CanUserResize is false.  expectedColumns are the prevColumns", indexToResize));
                return expectedColumns;
            }

            // account for resizing beyond min or max limit
            double expectedResizedColumnWidth = resizeRight ? colToResize.displayWidth + resizeDiff : colToResize.displayWidth - resizeDiff;
            if (DoubleUtil.LessThan(expectedResizedColumnWidth, colToResize.minWidth))
            {
                resizeDiff = colToResize.minWidth - colToResize.displayWidth;
            }
            else if (DoubleUtil.GreaterThan(expectedResizedColumnWidth, colToResize.maxWidth))
            {
                resizeDiff = colToResize.maxWidth - colToResize.displayWidth;
            }

            if (resizeRight)
            {
                ComputeExpectedColumnWidthsOnPositiveResize(ref expectedColumns, indexToResize, resizeDiff, resizeRight, epsilon);
            }
            else
            {
                ComputeExpectedColumnWidthsOnNegativeResize(ref expectedColumns, indexToResize, resizeDiff, resizeRight, epsilon);
            }

            return expectedColumns;
        }

        protected void ComputeExpectedColumnWidthsOnPositiveResize(
            ref DataGridHelper.ColumnWidthData[] expectedColumns,
            int indexToResize,
            double resizeDiff,
            bool resizeRight,
            double epsilon)
        {
            double perStarWidth = 0.0;
            if (HasStarColumns(out perStarWidth))
            {
                //
                // with star columns, columns are resized on the right
                //

                double resizeDiffLeft = resizeDiff;

                //
                // first reuse unused space
                //
                double unusedSpace = GetUnusedSpaceFromDataGrid(expectedColumns, 0.0, false);
                if (DoubleUtil.GreaterThan(unusedSpace, 0.0))
                {
                    double resize = DoubleUtil.LessThan(resizeDiffLeft, unusedSpace) ? resizeDiffLeft : unusedSpace;
                    SetResizedColumnWidth(ref expectedColumns, indexToResize, resize, resizeRight, epsilon);
                    resizeDiffLeft = Math.Max(0.0, resizeDiffLeft - unusedSpace);
                }

                //
                // if there is more left, reducing columns to the right which are greater than the desired size
                //
                RecomputeNonStarColumnWidthsOnColumnPositiveResize(ref expectedColumns, ref resizeDiffLeft, indexToResize, resizeRight, epsilon, true);

                //
                // if there is more left, reduce star columns to the right
                //
                if (DoubleUtil.GreaterThan(resizeDiffLeft, 0.0))
                {
                    ComputeExpectedStarColumnWidthsOnResize(ref expectedColumns, indexToResize, resizeRight, perStarWidth, ref resizeDiffLeft, epsilon);
                }

                //
                // if there is more left, reduce non-star columns on the right starting from last to first
                //
                RecomputeNonStarColumnWidthsOnColumnPositiveResize(ref expectedColumns, ref resizeDiffLeft, indexToResize, resizeRight, epsilon, false);
            }
            else
            {
                //
                // with no star columns, resizing does not affect other columns
                //
                SetResizedColumnWidth(ref expectedColumns, indexToResize, resizeDiff, resizeRight, epsilon);
            }
        }

        protected void RecomputeNonStarColumnWidthsOnColumnPositiveResize(
            ref DataGridHelper.ColumnWidthData[] expectedColumns,
            ref double resizeDiffLeft,
            int indexToResize,
            bool resizeRight,
            double epsilon,
            bool onlyShrinkToDesiredWidth)
        {
            if (DoubleUtil.GreaterThan(resizeDiffLeft, 0.0))
            {
                double totalExcessWidth = 0.0;
                bool iterationNeeded = true;
                for (int i = expectedColumns.Length - 1; iterationNeeded && i > indexToResize; i--)
                {
                    double columnExcessWidth = onlyShrinkToDesiredWidth ? expectedColumns[i].displayWidth - Math.Max(expectedColumns[i].desiredWidth, expectedColumns[i].minWidth) : expectedColumns[i].displayWidth - expectedColumns[i].minWidth;
                    if (expectedColumns[i].unitType != DataGridLengthUnitType.Star &&
                        expectedColumns[i].CanUserResize &&
                        DoubleUtil.GreaterThan(columnExcessWidth, 0.0))
                    {
                        double excessWidth = expectedColumns[i].displayWidth - expectedColumns[i].minWidth;
                        if (DoubleUtil.GreaterThan(totalExcessWidth + excessWidth, resizeDiffLeft))
                        {
                            // enough space has been reached
                            excessWidth = resizeDiffLeft - totalExcessWidth;
                            iterationNeeded = false;
                        }

                        // update the display width here
                        expectedColumns[i].displayWidth = expectedColumns[i].displayWidth - excessWidth;

                        totalExcessWidth += excessWidth;
                    }
                }
                // now update the column being resized
                if (DoubleUtil.GreaterThan(totalExcessWidth, 0.0))
                {
                    SetResizedColumnWidth(ref expectedColumns, indexToResize, totalExcessWidth, resizeRight, epsilon);
                    resizeDiffLeft -= totalExcessWidth;
                }
            }
        }

        protected void ComputeExpectedColumnWidthsOnNegativeResize(
            ref DataGridHelper.ColumnWidthData[] expectedColumns,
            int indexToResize,
            double resizeDiff,
            bool resizeRight,
            double epsilon)
        {
            double perStarWidth = 0.0;
            if (HasStarColumns(out perStarWidth))
            {
                double resizeDiffLeft = resizeDiff;

                //
                // increase non-star columns on the right which are less than desired size
                //
                RecompleteNonStartColumnWidthsOnColumnNegativeResize(ref expectedColumns, ref resizeDiffLeft, indexToResize, resizeRight, epsilon, false);

                //
                // if there is more left, increase star columns on the right
                //
                if (DoubleUtil.GreaterThan(resizeDiffLeft, 0.0))
                {
                    ComputeExpectedStarColumnWidthsOnResize(ref expectedColumns, indexToResize, resizeRight, perStarWidth, ref resizeDiffLeft, epsilon);
                }

                //
                // increase non-star columns on the right which are less than max size
                //
                RecompleteNonStartColumnWidthsOnColumnNegativeResize(ref expectedColumns, ref resizeDiffLeft, indexToResize, resizeRight, epsilon, true);

                //
                // update the column to resize as it is free to enlarge up to its max
                //
                if (DoubleUtil.GreaterThan(resizeDiffLeft, 0.0))
                {
                    if (expectedColumns[indexToResize].unitType != DataGridLengthUnitType.Star)
                        SetResizedColumnWidth(ref expectedColumns, indexToResize, resizeDiffLeft, resizeRight, epsilon);
                }
            }
            else
            {
                //
                // with no star columns, resizing does not affect other columns
                //
                SetResizedColumnWidth(ref expectedColumns, indexToResize, resizeDiff, resizeRight, epsilon);
            }
        }

        protected void RecompleteNonStartColumnWidthsOnColumnNegativeResize(
            ref DataGridHelper.ColumnWidthData[] expectedColumns,
            ref double resizeDiffLeft,
            int indexToResize,
            bool resizeRight,
            double epsilon,
            bool expandBeyondDesiredWidth)
        {
            double totalLagWidth = 0.0;
            bool iterationNeeded = true;
            for (int i = indexToResize + 1; iterationNeeded && i < expectedColumns.Length; i++)
            {
                double maxColumnResizeWidth = expandBeyondDesiredWidth ? expectedColumns[i].maxWidth : Math.Min(expectedColumns[i].desiredWidth, expectedColumns[i].maxWidth);
                if (expectedColumns[i].unitType != DataGridLengthUnitType.Star &&
                    expectedColumns[i].CanUserResize &&
                    DoubleUtil.LessThan(expectedColumns[i].displayWidth, maxColumnResizeWidth) &&
                    DoubleUtil.LessThanOrClose(expectedColumns[i].displayWidth, expectedColumns[i].maxWidth))
                {
                    double lagWidth = maxColumnResizeWidth - expectedColumns[i].displayWidth;
                    if (DoubleUtil.GreaterThanOrClose(totalLagWidth + lagWidth, resizeDiffLeft))
                    {
                        lagWidth = resizeDiffLeft - totalLagWidth;
                        iterationNeeded = false;
                    }

                    expectedColumns[i].displayWidth = Math.Min(expectedColumns[i].displayWidth + lagWidth, expectedColumns[i].maxWidth);
                    totalLagWidth += lagWidth;
                }
            }

            if (DoubleUtil.GreaterThan(totalLagWidth, 0.0))
            {
                SetResizedColumnWidth(ref expectedColumns, indexToResize, totalLagWidth, resizeRight, epsilon);
                resizeDiffLeft -= totalLagWidth;
            }
        }

        protected void ComputeExpectedStarColumnWidthsOnResize(
            ref DataGridHelper.ColumnWidthData[] expectedColumns,
            int indexToResize,
            bool resizeRight,
            double perStarWidth,
            ref double resizeDiffLeft,
            double epsilon)
        {
            while (DoubleUtil.GreaterThan(resizeDiffLeft, 0.0))
            {
                double minPerStarExcessRatio = Double.PositiveInfinity;
                double rightStarFactors = GetStarFactors(expectedColumns, indexToResize + 1, resizeRight, out minPerStarExcessRatio);

                if (DoubleUtil.GreaterThan(rightStarFactors, 0.0))
                {
                    double changePerStar = 0.0;
                    double resizeDiffLeftForIteration = 0.0;
                    if (DoubleUtil.LessThan(resizeDiffLeft, minPerStarExcessRatio * rightStarFactors))
                    {
                        // there is enough space to resize all star columns down
                        changePerStar = resizeDiffLeft / rightStarFactors;
                        resizeDiffLeftForIteration = resizeDiffLeft;
                        resizeDiffLeft = 0.0;
                    }
                    else
                    {
                        changePerStar = minPerStarExcessRatio;
                        resizeDiffLeftForIteration = changePerStar * rightStarFactors;
                        resizeDiffLeft -= resizeDiffLeftForIteration;
                    }

                    for (int i = indexToResize; i < expectedColumns.Length; i++)
                    {
                        if (i == indexToResize)
                        {
                            SetResizedColumnWidth(ref expectedColumns, indexToResize, resizeDiffLeftForIteration, resizeRight, epsilon);
                        }
                        else if (expectedColumns[i].unitType == DataGridLengthUnitType.Star &&
                                expectedColumns[i].CanUserResize)
                        {
                            if (resizeRight && DoubleUtil.GreaterThan(expectedColumns[i].displayWidth, expectedColumns[i].minWidth))
                            {
                                double desiredWidth = expectedColumns[i].displayWidth - (expectedColumns[i].width * changePerStar);
                                expectedColumns[i].displayWidth = Math.Max(desiredWidth, expectedColumns[i].minWidth);
                                expectedColumns[i].desiredWidth = desiredWidth;
                                expectedColumns[i].width = desiredWidth / perStarWidth;
                            }
                            else if (!resizeRight && DoubleUtil.LessThan(expectedColumns[i].displayWidth, expectedColumns[i].maxWidth))
                            {
                                double desiredWidth = expectedColumns[i].displayWidth + (expectedColumns[i].width * changePerStar);
                                expectedColumns[i].displayWidth = Math.Min(desiredWidth, expectedColumns[i].maxWidth);
                                expectedColumns[i].desiredWidth = desiredWidth;
                                expectedColumns[i].width = desiredWidth / perStarWidth;
                            }
                        }
                    }

                    if (DoubleUtil.AreClose(resizeDiffLeft, 0.0))
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        protected void SetResizedColumnWidth(
            ref DataGridHelper.ColumnWidthData[] expectedColumns,
            int indexToResize,
            double resizeDiff,
            bool resizeRight,
            double epsilon)
        {
            if (expectedColumns[indexToResize].CanUserResize)
            {
                double newDisplayWidth = resizeRight ? expectedColumns[indexToResize].displayWidth + resizeDiff : expectedColumns[indexToResize].displayWidth - resizeDiff;
                newDisplayWidth = Math.Max(newDisplayWidth, expectedColumns[indexToResize].minWidth);
                newDisplayWidth = Math.Min(newDisplayWidth, expectedColumns[indexToResize].maxWidth);

                if (expectedColumns[indexToResize].unitType == DataGridLengthUnitType.Star)
                {
                    double starValue = expectedColumns[indexToResize].desiredWidth / expectedColumns[indexToResize].width;
                    expectedColumns[indexToResize].displayWidth = newDisplayWidth;
                    expectedColumns[indexToResize].desiredWidth = newDisplayWidth;
                    expectedColumns[indexToResize].width = newDisplayWidth / starValue;
                }
                else if (expectedColumns[indexToResize].unitType == DataGridLengthUnitType.Pixel)
                {
                    // display value and desired value update
                    expectedColumns[indexToResize].displayWidth = newDisplayWidth;
                    expectedColumns[indexToResize].width = newDisplayWidth;
                    expectedColumns[indexToResize].desiredWidth = newDisplayWidth;
                }
                else
                {
                    // only display value updates
                    expectedColumns[indexToResize].displayWidth = newDisplayWidth;
                    expectedColumns[indexToResize].unitType = DataGridLengthUnitType.Pixel;
                    expectedColumns[indexToResize].width = newDisplayWidth;
                    expectedColumns[indexToResize].desiredWidth = newDisplayWidth;
                }
            }
        }

        protected double GetStarFactors(
            DataGridHelper.ColumnWidthData[] expectedColumns,
            int startIndex,
            bool resizeRight,
            out double minPerStarExcessRatio)
        {
            minPerStarExcessRatio = Double.PositiveInfinity;
            double rightStarFactors = 0.0;

            //
            // has to be a star column that can be resized and it needs a width greater than min width
            //
            for (int i = startIndex; i < expectedColumns.Length; i++)
            {

                if (expectedColumns[i].unitType == DataGridLengthUnitType.Star &&
                    expectedColumns[i].CanUserResize &&
                    !DoubleUtil.AreClose(expectedColumns[i].width, 0.0))
                {
                    if (resizeRight && DoubleUtil.GreaterThan(expectedColumns[i].displayWidth, expectedColumns[i].minWidth))
                    {
                        rightStarFactors += expectedColumns[i].width;

                        // compute excess ratio
                        double excessRatio = (expectedColumns[i].displayWidth - expectedColumns[i].minWidth) / expectedColumns[i].width;
                        if (DoubleUtil.LessThan(excessRatio, minPerStarExcessRatio))
                            minPerStarExcessRatio = excessRatio;
                    }
                    else if (!resizeRight && DoubleUtil.LessThan(expectedColumns[i].displayWidth, expectedColumns[i].maxWidth))
                    {
                        rightStarFactors += expectedColumns[i].width;

                        // compute lag ratio
                        double lagRatio = (expectedColumns[i].maxWidth - expectedColumns[i].displayWidth) / expectedColumns[i].width;
                        if (DoubleUtil.LessThan(lagRatio, minPerStarExcessRatio))
                            minPerStarExcessRatio = lagRatio;
                    }
                }
            }

            return rightStarFactors;
        }

        #endregion Resizing Helpers

        #region Redistribution on Width Change Helpers

        /// <summary>
        /// Convenience method for computing and verifying column widths on Width change
        /// </summary>
        /// <param name="expectedColumns">the expected state that will be computed here</param>
        /// <param name="colIndex">the column index width that is changed</param>
        /// <param name="oldWidth">the previous width</param>
        /// <param name="epsilon">error</param>
        protected void ComputeAndVerifyOnColumnWidthChange(
            ref DataGridHelper.ColumnWidthData[] expectedColumns,
            int colIndex,
            DataGridHelper.ColumnWidthData oldWidth,
            double epsilon)
        {
            ComputeExpectedWidthsOnColumnWidthChange(ref expectedColumns, colIndex, oldWidth);
            VerifyColumnWidths(expectedColumns, epsilon);
        }

        protected void ComputeExpectedWidthsOnColumnWidthChange(
            ref DataGridHelper.ColumnWidthData[] expectedColumns,
            int colIndex,
            DataGridHelper.ColumnWidthData oldWidth)
        {
            bool hasStarColumns = HasStarColumns();

            if (oldWidth.unitType == DataGridLengthUnitType.Star && expectedColumns[colIndex].unitType != DataGridLengthUnitType.Star && !hasStarColumns)
            {
                //
                // no more star widths after updated, so expand all to desired
                //
                ExpandAllColumnWidthsToDesiredValue(ref expectedColumns);
            }
            else if (oldWidth.unitType != DataGridLengthUnitType.Star && expectedColumns[colIndex].unitType == DataGridLengthUnitType.Star)
            {
                int starCount = 0;
                foreach (DataGridHelper.ColumnWidthData column in expectedColumns)
                {
                    if (column.unitType == DataGridLengthUnitType.Star)
                        starCount++;
                }
                if (starCount == 1)
                {
                    //
                    //setting this width to star is the first star column of the set, need to recompute everything
                    //
                    ComputeExpectedColumnWidths(ref expectedColumns);
                }
                else
                {
                    //
                    // there were already other star columns even if this one changed to star. give away your space
                    // then recompute the star columns
                    //
                    double leftOverSpace = GiveAwayWidthToNonStarColumns(ref expectedColumns, -1, oldWidth.displayWidth - expectedColumns[colIndex].minWidth);
                    expectedColumns[colIndex].displayWidth = expectedColumns[colIndex].minWidth + leftOverSpace;
                    RecomputeStarColumnWidths(ref expectedColumns);
                }
            }
            else if (oldWidth.unitType == DataGridLengthUnitType.Star && expectedColumns[colIndex].unitType == DataGridLengthUnitType.Star)
            {
                //
                // column was already a star, just recompute the star column widths
                //
                RecomputeStarColumnWidths(ref expectedColumns);
            }
            else if (hasStarColumns)
            {
                //
                // column changes from non-star to non-star but there are still other star columns.
                // need to give or take away space from columns
                //
                ComputeExpectedWidthsOnNonStarColumnWidthChange(ref expectedColumns, colIndex, oldWidth);
            }
        }

        #endregion Redistribution on Width Change Helpers

        #region Redistribution on MinWidth Change Helper

        /// <summary>
        /// Convenience method for computing and verifying column widths on MinWidth change
        /// </summary>
        /// <param name="expectedColumns">the expected state that will be computed here</param>
        /// <param name="colIndex">the column index width that is changed</param>
        /// <param name="oldWidth">the previous width</param>
        /// <param name="epsilon">error</param>
        protected void ComputeAndVerifyOnColumnMinWidthChange(
            ref DataGridHelper.ColumnWidthData[] expectedColumns,
            int colIndex,
            DataGridHelper.ColumnWidthData oldWidth,
            double epsilon)
        {
            DataGridHelper.ColumnWidthData[] prevColumns = new DataGridHelper.ColumnWidthData[expectedColumns.Length];
            expectedColumns.CopyTo(prevColumns, 0);

            ComputeExpectedWidthsOnColumnMinWidthChange(ref expectedColumns, colIndex, oldWidth);
            VerifyColumnWidths(expectedColumns, epsilon);
        }

        protected void ComputeExpectedWidthsOnColumnMinWidthChange(
            ref DataGridHelper.ColumnWidthData[] expectedColumns,
            int colIndex,
            DataGridHelper.ColumnWidthData oldWidth)
        {
            bool hasStarColumns = HasStarColumns();

            if (DoubleUtil.GreaterThan(expectedColumns[colIndex].minWidth, expectedColumns[colIndex].displayWidth))
            {
                if (hasStarColumns)
                {
                    TakeAwayWidthFromColumns(ref expectedColumns, colIndex, expectedColumns[colIndex].minWidth - expectedColumns[colIndex].displayWidth, false);
                }
                expectedColumns[colIndex].displayWidth = expectedColumns[colIndex].minWidth;
            }
            else if (DoubleUtil.LessThan(expectedColumns[colIndex].minWidth, oldWidth.minWidth))
            {
                if (expectedColumns[colIndex].unitType == DataGridLengthUnitType.Star)
                {
                    if (DoubleUtil.AreClose(expectedColumns[colIndex].displayWidth, oldWidth.minWidth))
                    {
                        GiveAwayWidthToColumns(ref expectedColumns, colIndex, oldWidth.minWidth - expectedColumns[colIndex].minWidth, true);
                    }
                }
                else if (DoubleUtil.GreaterThan(oldWidth.minWidth, expectedColumns[colIndex].desiredWidth))
                {
                    double displayWidth = Math.Max(expectedColumns[colIndex].desiredWidth, expectedColumns[colIndex].minWidth);
                    if (hasStarColumns)
                    {
                        double giveAwayWidth = oldWidth.minWidth - displayWidth;
                        GiveAwayWidthToColumns(ref expectedColumns, colIndex, giveAwayWidth);
                    }
                    expectedColumns[colIndex].displayWidth = displayWidth;
                }
            }
        }

        #endregion Redistribution on MinWidth Change Helper

        #region Redistribution on MaxWidth Change Helper

        /// <summary>
        /// Convenience method for computing and verifying column widths on MaxWidth change
        /// </summary>
        /// <param name="expectedColumns">the expected state that will be computed here</param>
        /// <param name="colIndex">the column index width that is changed</param>
        /// <param name="oldWidth">the previous width</param>
        /// <param name="epsilon">error</param>
        protected void ComputeAndVerifyOnColumnMaxWidthChange(
            ref DataGridHelper.ColumnWidthData[] expectedColumns,
            int colIndex,
            DataGridHelper.ColumnWidthData oldWidth,
            double epsilon)
        {
            DataGridHelper.ColumnWidthData[] prevColumns = new DataGridHelper.ColumnWidthData[expectedColumns.Length];
            expectedColumns.CopyTo(prevColumns, 0);

            ComputeExpectedWidthsOnColumnMaxWidthChange(ref expectedColumns, colIndex, oldWidth);
            VerifyColumnWidths(expectedColumns, epsilon);
        }

        protected void ComputeExpectedWidthsOnColumnMaxWidthChange(
            ref DataGridHelper.ColumnWidthData[] expectedColumns,
            int colIndex,
            DataGridHelper.ColumnWidthData oldWidth)
        {
            bool hasStarColumns = HasStarColumns();

            if (DoubleUtil.LessThan(expectedColumns[colIndex].maxWidth, expectedColumns[colIndex].displayWidth))
            {
                if (hasStarColumns)
                {
                    GiveAwayWidthToColumns(ref expectedColumns, colIndex, expectedColumns[colIndex].displayWidth - expectedColumns[colIndex].maxWidth);
                }
                expectedColumns[colIndex].displayWidth = expectedColumns[colIndex].maxWidth;
            }
            else if (DoubleUtil.GreaterThan(expectedColumns[colIndex].maxWidth, oldWidth.maxWidth))
            {
                if (expectedColumns[colIndex].unitType == DataGridLengthUnitType.Star)
                {
                    RecomputeStarColumnWidths(ref expectedColumns);
                }
                else if (DoubleUtil.LessThan(oldWidth.maxWidth, expectedColumns[colIndex].desiredWidth))
                {
                    double displayWidth = Math.Min(expectedColumns[colIndex].desiredWidth, expectedColumns[colIndex].maxWidth);
                    if (hasStarColumns)
                    {
                        double leftOverSpace = 0.0;
                        double unusedSpace = GetUnusedSpaceFromDataGrid(expectedColumns, 0.0, false);
                        if (DoubleUtil.GreaterThan(unusedSpace, 0.0))
                        {
                            leftOverSpace = Math.Max(0.0, (displayWidth - oldWidth.maxWidth) - unusedSpace);
                        }

                        TakeAwayWidthFromStarColumns(ref expectedColumns, colIndex, ref leftOverSpace);
                        displayWidth -= leftOverSpace;
                    }
                    expectedColumns[colIndex].displayWidth = displayWidth;
                }
            }
        }

        #endregion Redistribution on MaxWidth Change Helper

        #region Width Computation Helpers

        private void ComputeExpectedColumnWidths(
            ref DataGridHelper.ColumnWidthData[] expectedColumns)
        {
            if (HasStarColumns())
            {
                //
                // first set all non-star columns to their desired values
                //
                int i = 0;
                foreach (DataGridHelper.ColumnWidthData width in expectedColumns)
                {
                    if (width.unitType != DataGridLengthUnitType.Star)
                    {
                        double actualWidth = Math.Max(width.minWidth, (Double.IsNaN(width.desiredWidth) ? width.minWidth : width.desiredWidth));
                        actualWidth = Math.Min(actualWidth, width.maxWidth);
                        expectedColumns[i].displayWidth = actualWidth;
                    }
                    i++;
                }

                //
                // then distribute space among all columns
                //
                DistributeSpaceAmongColumns(ref expectedColumns, GetViewportWidthForColumns());
            }
            else
            {
                //
                // expand all column widths to desired
                //
                ExpandAllColumnWidthsToDesiredValue(ref expectedColumns);
            }
        }

        private void DistributeSpaceAmongColumns(
            ref DataGridHelper.ColumnWidthData[] expectedColumns,
            double availableSpace)
        {
            double totalMinWidths = 0.0;
            double totalMaxWidths = 0.0;
            double totalStarMinWidths = 0.0;

            foreach (DataGridHelper.ColumnWidthData column in expectedColumns)
            {
                totalMinWidths += column.minWidth;
                totalMaxWidths += column.maxWidth;
                if (column.unitType == DataGridLengthUnitType.Star)
                {
                    totalStarMinWidths += column.minWidth;
                }
            }

            // account for available space and the total min/max values
            if (DoubleUtil.LessThan(availableSpace, totalMinWidths))
            {
                availableSpace = totalMinWidths;
            }

            if (DoubleUtil.GreaterThan(availableSpace, totalMaxWidths))
            {
                availableSpace = totalMaxWidths;
            }

            //
            // distribute the space among non-star columns giving it the available space minus the total min star width
            //
            double nonStarSpaceLeftOver = DistributeSpaceAmongNonStarColumns(ref expectedColumns, availableSpace - totalStarMinWidths);

            //
            // then with any left over space from the non-star columns, give it to the star columns
            //
            ComputeStarColumnWidths(ref expectedColumns, -1, totalStarMinWidths + nonStarSpaceLeftOver);
        }

        private double DistributeSpaceAmongNonStarColumns(
            ref DataGridHelper.ColumnWidthData[] expectedColumns,
            double availableSpace)
        {
            double requiredSpace = 0.0;
            foreach (DataGridHelper.ColumnWidthData column in expectedColumns)
            {
                if (column.unitType != DataGridLengthUnitType.Star)
                {
                    requiredSpace += column.displayWidth;
                }
            }

            if (DoubleUtil.LessThan(availableSpace, requiredSpace))
            {
                //
                // need to take away width from non-star columns
                //
                double spaceDeficit = requiredSpace - availableSpace;
                TakeAwayWidthFromNonStarColumns(ref expectedColumns, -1, ref spaceDeficit);
            }

            return Math.Max(availableSpace - requiredSpace, 0.0);
        }

        private double ComputeStarColumnWidths(
            ref DataGridHelper.ColumnWidthData[] expectedColumns,
            int colIndex,
            double availableStarSpace)
        {
            List<DataGridHelper.ColumnWidthData> unResolvedColumns = new List<DataGridHelper.ColumnWidthData>();
            List<DataGridHelper.ColumnWidthData> partialResolvedColumns = new List<DataGridHelper.ColumnWidthData>();
            double totalFactors = 0.0;
            double totalMinWidths = 0.0;
            double totalMaxWidths = 0.0;
            double utilizedStarSpace = 0.0;

            int ignoreIndex = 0;
            foreach (DataGridHelper.ColumnWidthData column in expectedColumns)
            {
                if (column.unitType == DataGridLengthUnitType.Star)
                {
                    unResolvedColumns.Add(column);
                    totalFactors += column.width;
                    totalMinWidths += column.minWidth;
                    totalMaxWidths += column.maxWidth;
                }
                ignoreIndex++;
            }

            // account for available star space and the total min/max values
            if (DoubleUtil.LessThan(availableStarSpace, totalMinWidths))
            {
                availableStarSpace = totalMinWidths;
            }

            if (DoubleUtil.GreaterThan(availableStarSpace, totalMaxWidths))
            {
                availableStarSpace = totalMaxWidths;
            }

            while (unResolvedColumns.Count > 0)
            {
                double starValue = availableStarSpace / totalFactors;

                // first find all star columns whose share is less than their min widths
                // and remove them from the unResolvedColumns
                for (int i = 0, count = unResolvedColumns.Count; i < count; i++)
                {
                    DataGridHelper.ColumnWidthData column = unResolvedColumns[i];
                    double starColumnWidth = availableStarSpace * column.width / totalFactors;
                    if (DoubleUtil.GreaterThan(column.minWidth, starColumnWidth))
                    {
                        availableStarSpace = Math.Max(0.0, availableStarSpace - column.minWidth);
                        totalFactors -= column.width;
                        unResolvedColumns.RemoveAt(i);
                        i--;
                        count--;
                        partialResolvedColumns.Add(column);
                    }
                }

                // now do the same with columns whose share is more than their max widths
                bool iterationRequired = false;
                for (int i = 0, count = unResolvedColumns.Count; i < count; i++)
                {
                    DataGridHelper.ColumnWidthData column = unResolvedColumns[i];
                    double starColumnWidth = availableStarSpace * column.width / totalFactors;
                    if (DoubleUtil.LessThan(column.maxWidth, starColumnWidth))
                    {
                        iterationRequired = true;
                        unResolvedColumns.RemoveAt(i);
                        availableStarSpace -= column.maxWidth;
                        utilizedStarSpace += column.maxWidth;
                        totalFactors -= column.width;

                        int index = IndexOfColumn(expectedColumns, column);
                        expectedColumns[index].displayWidth = column.maxWidth;
                        expectedColumns[index].desiredWidth = starValue * column.width;
                        break;
                    }
                }

                if (iterationRequired)
                {
                    for (int i = 0, count = partialResolvedColumns.Count; i < count; i++)
                    {
                        DataGridHelper.ColumnWidthData column = partialResolvedColumns[i];
                        unResolvedColumns.Add(column);

                        availableStarSpace += column.minWidth;
                        totalFactors += column.width;
                    }
                    partialResolvedColumns.Clear();
                }
                else
                {
                    for (int i = 0, count = partialResolvedColumns.Count; i < count; i++)
                    {
                        int index = IndexOfColumn(expectedColumns, partialResolvedColumns[i]);
                        expectedColumns[index].displayWidth = expectedColumns[index].minWidth;
                        expectedColumns[index].desiredWidth = starValue * expectedColumns[index].width;
                        utilizedStarSpace += expectedColumns[index].minWidth;
                    }
                    partialResolvedColumns.Clear();

                    for (int i = 0, count = unResolvedColumns.Count; i < count; i++)
                    {
                        int index = IndexOfColumn(expectedColumns, unResolvedColumns[i]);
                        double starColumnWidth = availableStarSpace * expectedColumns[index].width / totalFactors;

                        expectedColumns[index].displayWidth = starColumnWidth;
                        expectedColumns[index].desiredWidth = starValue * expectedColumns[index].width;
                        utilizedStarSpace += starColumnWidth;
                    }
                    unResolvedColumns.Clear();
                }
            }

            return utilizedStarSpace;
        }

        private void ExpandAllColumnWidthsToDesiredValue(
            ref DataGridHelper.ColumnWidthData[] expectedColumnWidths)
        {
            for (int i = 0; i < expectedColumnWidths.Length; i++)
            {
                if (DoubleUtil.GreaterThan(expectedColumnWidths[i].desiredWidth, expectedColumnWidths[i].displayWidth) &&
                    !DoubleUtil.AreClose(expectedColumnWidths[i].displayWidth, expectedColumnWidths[i].maxWidth))
                {
                    expectedColumnWidths[i].displayWidth = Math.Min(expectedColumnWidths[i].desiredWidth, expectedColumnWidths[i].maxWidth);
                }
            }
        }

        private void ComputeExpectedWidthsOnNonStarColumnWidthChange(
            ref DataGridHelper.ColumnWidthData[] expectedColumns,
            int colIndex,
            DataGridHelper.ColumnWidthData oldWidth)
        {
            Avalon.Test.ComponentModel.Utilities.Assert.AssertTrue("This method can only be ran when there are star columns.", HasStarColumns());

            if (DoubleUtil.GreaterThan(expectedColumns[colIndex].desiredWidth, oldWidth.displayWidth))
            {
                //
                // column's new width increased, so take away widths from other columns to place on this column
                //
                double nonRetrievableSpace = TakeAwayWidthFromColumns(ref expectedColumns, colIndex, expectedColumns[colIndex].desiredWidth - oldWidth.displayWidth);
                if (DoubleUtil.GreaterThan(nonRetrievableSpace, 0.0))
                {
                    expectedColumns[colIndex].displayWidth = Math.Max(expectedColumns[colIndex].displayWidth - nonRetrievableSpace, oldWidth.minWidth);
                }
                else
                {
                    expectedColumns[colIndex].displayWidth = Math.Max(expectedColumns[colIndex].displayWidth, oldWidth.minWidth);
                }

                expectedColumns[colIndex].width = expectedColumns[colIndex].width;
                expectedColumns[colIndex].desiredWidth = expectedColumns[colIndex].desiredWidth;
                expectedColumns[colIndex].unitType = expectedColumns[colIndex].unitType;
            }
            else if (DoubleUtil.LessThan(expectedColumns[colIndex].desiredWidth, oldWidth.displayWidth))
            {
                //
                // column's new width decreased, so give away width to other columns
                //
                double newDesiredValue = Math.Max(expectedColumns[colIndex].desiredWidth, oldWidth.minWidth);
                newDesiredValue = Math.Min(newDesiredValue, oldWidth.maxWidth);
                GiveAwayWidthToColumns(ref expectedColumns, colIndex, oldWidth.displayWidth - newDesiredValue);

                expectedColumns[colIndex].displayWidth = expectedColumns[colIndex].displayWidth;
                expectedColumns[colIndex].width = expectedColumns[colIndex].width;
                expectedColumns[colIndex].desiredWidth = expectedColumns[colIndex].desiredWidth;
                expectedColumns[colIndex].unitType = expectedColumns[colIndex].unitType;
            }
        }

        /// <summary>
        /// computes the star widths based on total viewport width minus all non-star column widths
        /// </summary>
        private void RecomputeStarColumnWidths(
            ref DataGridHelper.ColumnWidthData[] expectedColumns)
        {
            double totalDisplaySpace = GetViewportWidthForColumns();
            double nonStarSpace = 0.0;
            foreach (DataGridHelper.ColumnWidthData column in expectedColumns)
            {
                if (column.unitType != DataGridLengthUnitType.Star)
                    nonStarSpace += column.displayWidth;
            }

            if (Double.IsNaN(nonStarSpace))
                return;

            ComputeStarColumnWidths(ref expectedColumns, -1, totalDisplaySpace - nonStarSpace);
        }

        #endregion Width Computation Helpers

        #region Take Away Helpers

        private double TakeAwayWidthFromColumns(
            ref DataGridHelper.ColumnWidthData[] expectedColumns,
            int colIndex,
            double takeAwayWidth)
        {
            return TakeAwayWidthFromColumns(ref expectedColumns, colIndex, takeAwayWidth, true);
        }

        /// <summary>
        /// Called by:
        /// ComputeExpectedWidthsOnNonStarColumnWidthChange
        ///     => desired width of column increased, so take away widths from other columns
        ///
        /// Algorithm:
        /// 1. try to take away width from unused space
        /// 2. then take away from star columns
        /// 3. then take away from non star columns
        /// </summary>
        /// <returns></returns>
        private double TakeAwayWidthFromColumns(
            ref DataGridHelper.ColumnWidthData[] expectedColumns,
            int colIndex,
            double takeAwayWidth,
            bool spaceAlreadyUtilized)
        {
            //
            // try to take away width from unused space
            //
            takeAwayWidth = GetUnusedSpaceFromDataGrid(expectedColumns, takeAwayWidth, spaceAlreadyUtilized);

            //
            // then take away from star columns
            //
            TakeAwayWidthFromStarColumns(ref expectedColumns, colIndex, ref takeAwayWidth);

            //
            // then take away from non star columns
            //
            TakeAwayWidthFromNonStarColumns(ref expectedColumns, colIndex, ref takeAwayWidth);

            return takeAwayWidth;
        }

        private void TakeAwayWidthFromStarColumns(
            ref DataGridHelper.ColumnWidthData[] expectedColumns,
            int colIndex,
            ref double takeAwayWidth)
        {
            if (DoubleUtil.GreaterThan(takeAwayWidth, 0.0))
            {
                double sumOfStarDisplayWidths = 0.0;
                double sumOfStarMinWidths = 0.0;
                int i = 0;
                foreach (DataGridHelper.ColumnWidthData width in expectedColumns)
                {
                    if (width.unitType == DataGridLengthUnitType.Star)
                    {
                        // include the takeAwayWidth of the column that changed
                        if (i == colIndex)
                        {
                            sumOfStarDisplayWidths += takeAwayWidth;
                        }
                        sumOfStarDisplayWidths += width.displayWidth;
                        sumOfStarMinWidths += width.minWidth;
                    }
                    i++;
                }

                double expectedStarSpace = sumOfStarDisplayWidths - takeAwayWidth;
                double usedStarSpace = ComputeStarColumnWidths(ref expectedColumns, colIndex, Math.Max(expectedStarSpace, sumOfStarMinWidths));
                takeAwayWidth = Math.Max(usedStarSpace - expectedStarSpace, 0.0);
            }
        }

        private void TakeAwayWidthFromNonStarColumns(
            ref DataGridHelper.ColumnWidthData[] expectedColumns,
            int colIndex,
            ref double takeAwayWidth)
        {
            if (DoubleUtil.GreaterThan(takeAwayWidth, 0.0))
            {
                while (DoubleUtil.GreaterThan(takeAwayWidth, 0.0))
                {
                    int countOfParticipatingColumns = 0;
                    double minExcessWidth = FindMinimumExcessWidthOfNonStarColumns(expectedColumns, colIndex, out countOfParticipatingColumns);
                    if (countOfParticipatingColumns == 0)
                    {
                        break;
                    }

                    double minTotalExcessWidth = minExcessWidth * countOfParticipatingColumns;
                    if (DoubleUtil.GreaterThanOrClose(minTotalExcessWidth, takeAwayWidth))
                    {
                        minExcessWidth = takeAwayWidth / countOfParticipatingColumns;
                        takeAwayWidth = 0.0;
                    }
                    else
                    {
                        takeAwayWidth -= minTotalExcessWidth;
                    }

                    TakeAwayWidthFromEveryNonStarColumn(ref expectedColumns, colIndex, minExcessWidth);
                }
            }
        }

        /// <summary>
        /// MinExcessWidth is calculated by the min (ActualWidth - MinWidth)
        /// </summary>
        private double FindMinimumExcessWidthOfNonStarColumns(
            DataGridHelper.ColumnWidthData[] expectedColumns,
            int colIndex,
            out int countOfParticipatingColumns)
        {
            double minExcessWidth = Double.PositiveInfinity;
            countOfParticipatingColumns = 0;
            int i = 0;
            foreach (DataGridHelper.ColumnWidthData column in expectedColumns)
            {
                if (colIndex != i &&
                    column.unitType != DataGridLengthUnitType.Star &&
                    DoubleUtil.GreaterThan(column.displayWidth, column.minWidth))
                {
                    countOfParticipatingColumns++;
                    double excessWidth = column.displayWidth - column.minWidth;
                    if (DoubleUtil.LessThan(excessWidth, minExcessWidth))
                    {
                        minExcessWidth = excessWidth;
                    }
                }
                i++;
            }

            return minExcessWidth;
        }

        /// <summary>
        /// Updates the ActualWidth on every nonstar column to => ActualWidth - perColumnTakeAwayWidth
        /// </summary>
        private void TakeAwayWidthFromEveryNonStarColumn(
            ref DataGridHelper.ColumnWidthData[] expectedColumns,
            int colIndex,
            double perColumnTakeAwayWidth)
        {
            for (int i = 0; i < expectedColumns.Length; i++)
            {
                if (colIndex != i &&
                    expectedColumns[i].unitType != DataGridLengthUnitType.Star &&
                    DoubleUtil.GreaterThan(expectedColumns[i].displayWidth, expectedColumns[i].minWidth))
                {
                    expectedColumns[i].displayWidth = expectedColumns[i].displayWidth - perColumnTakeAwayWidth;
                }
            }
        }

        #endregion Take Away Helpers

        #region Give Away Helpers

        private double GiveAwayWidthToColumns(
            ref DataGridHelper.ColumnWidthData[] expectedColumns,
            int colIndex,
            double giveAwayWidth)
        {
            return GiveAwayWidthToColumns(ref expectedColumns, colIndex, giveAwayWidth, false);
        }

        private double GiveAwayWidthToColumns(
            ref DataGridHelper.ColumnWidthData[] expectedColumns,
            int colIndex,
            double giveAwayWidth,
            bool recomputeStars)
        {
            double originalGiveAwayWidth = giveAwayWidth;
            giveAwayWidth = GiveAwayWidthToScrollViewerExcess(giveAwayWidth);

            //
            // first give away width to non-star columns
            //
            giveAwayWidth = GiveAwayWidthToNonStarColumns(ref expectedColumns, colIndex, giveAwayWidth);

            //
            // then give away width to star columns
            //
            if (DoubleUtil.GreaterThan(giveAwayWidth, 0.0) || recomputeStars)
            {
                double sumOfStarDisplayWidths = 0.0;
                double sumOfStarMaxWidths = 0.0;
                bool giveAwayWidthIncluded = false;
                int i = 0;
                foreach (DataGridHelper.ColumnWidthData column in expectedColumns)
                {
                    if (column.unitType == DataGridLengthUnitType.Star)
                    {
                        if (i == colIndex)
                        {
                            giveAwayWidthIncluded = true;
                        }
                        sumOfStarDisplayWidths += column.displayWidth;
                        sumOfStarMaxWidths += column.maxWidth;
                    }
                    i++;
                }
                double expectedStarSpace = sumOfStarDisplayWidths;
                if (!giveAwayWidthIncluded)
                {
                    expectedStarSpace += giveAwayWidth;
                }
                else if (!DoubleUtil.AreClose(originalGiveAwayWidth, giveAwayWidth))
                {
                    expectedStarSpace -= (originalGiveAwayWidth - giveAwayWidth);
                }
                double usedStarSpace = ComputeStarColumnWidths(ref expectedColumns, colIndex, Math.Min(expectedStarSpace, sumOfStarMaxWidths));
                giveAwayWidth = Math.Max(usedStarSpace - expectedStarSpace, 0.0);
            }

            return giveAwayWidth;
        }

        private double GiveAwayWidthToNonStarColumns(
            ref DataGridHelper.ColumnWidthData[] expectedColumns,
            int colIndex,
            double giveAwayWidth)
        {
            while (DoubleUtil.GreaterThan(giveAwayWidth, 0.0))
            {
                int countOfParticipatingColumns = 0;
                double minLagWidth = FindMinimumLaggingWidthOfNonStarColumns(expectedColumns, colIndex, out countOfParticipatingColumns);

                if (countOfParticipatingColumns == 0)
                {
                    break;
                }

                double minTotalLagWidth = minLagWidth * countOfParticipatingColumns;
                if (DoubleUtil.GreaterThanOrClose(minTotalLagWidth, giveAwayWidth))
                {
                    minLagWidth = giveAwayWidth / countOfParticipatingColumns;
                    giveAwayWidth = 0.0;
                }
                else
                {
                    giveAwayWidth -= minTotalLagWidth;
                }
                GiveAwayWidthToEveryNonStarColumn(ref expectedColumns, colIndex, minLagWidth);
            }

            return giveAwayWidth;
        }

        private double FindMinimumLaggingWidthOfNonStarColumns(
            DataGridHelper.ColumnWidthData[] expectedColumns,
            int colIndex,
            out int countOfParticipatingColumns)
        {
            double minLagWidth = Double.PositiveInfinity;
            countOfParticipatingColumns = 0;
            int i = 0;
            foreach (DataGridHelper.ColumnWidthData column in expectedColumns)
            {
                if (colIndex != i &&
                    column.unitType != DataGridLengthUnitType.Star &&
                    DoubleUtil.LessThan(column.displayWidth, column.desiredWidth) &&
                    !DoubleUtil.AreClose(column.displayWidth, column.maxWidth))
                {
                    countOfParticipatingColumns++;
                    double lagWidth = Math.Min(column.desiredWidth, column.maxWidth) - column.displayWidth;
                    if (DoubleUtil.LessThan(lagWidth, minLagWidth))
                    {
                        minLagWidth = lagWidth;
                    }
                }
                i++;
            }

            return minLagWidth;
        }

        private void GiveAwayWidthToEveryNonStarColumn(
            ref DataGridHelper.ColumnWidthData[] expectedColumns,
            int colIndex,
            double perColumnGiveAwayWidth)
        {
            for (int i = 0; i < expectedColumns.Length; i++)
            {
                DataGridHelper.ColumnWidthData column = expectedColumns[i];
                if (colIndex == i)
                    continue;

                if (column.unitType == DataGridLengthUnitType.Star)
                    continue;

                if (DoubleUtil.LessThan(column.displayWidth, Math.Min(column.desiredWidth, column.maxWidth)))
                {
                    expectedColumns[i].displayWidth = column.displayWidth + perColumnGiveAwayWidth;
                }
            }
        }

        private double GiveAwayWidthToScrollViewerExcess(
            double giveAwayWidth)
        {
            double totalSpace = GetViewportWidthForColumns();
            double usedSpace = 0.0;
            foreach (DataGridColumn column in MyDataGrid.Columns)
            {
                usedSpace += column.Width.DisplayValue;
            }
            if (DoubleUtil.GreaterThan(usedSpace, totalSpace))
            {
                double contributingSpace = usedSpace - totalSpace;
                giveAwayWidth -= Math.Min(contributingSpace, giveAwayWidth);
            }
            return giveAwayWidth;
        }

        #endregion Give Away Helpers
    }
}
