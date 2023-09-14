using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Avalon.Test.ComponentModel;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Layout;
using System.Text;
using System;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    ////////////////////////////////////////////////////////////////////////////////////////////
    // DISABLEDUNSTABLETEST:
    // TestName: DataGridRowDetailsBVT
    // Area: Controls�� SubArea: DataGrid
    // Disable this case due to high fail rate, will enable after fix it.
    // to find all disabled tests in test tree, use: �findstr /snip DISABLEDUNSTABLETEST� 
    ////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// <description>
    /// Auto-sorting Behavioral tests for DataGrid.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridRowDetailsBVT", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite", Disabled = true)]
    public class DataGridRowDetailsBVT : DataGridTest
    {
        #region Private Fields

        private DataTemplate rowDetailsTemplate;

        #endregion Private Fields

        #region Constructor

        public DataGridRowDetailsBVT()
            : base(@"DataGridRowDetails.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestRowDetailsVisibility);
            RunSteps += new TestStep(TestRowDetailsWhenFrozenSingle);
            RunSteps += new TestStep(TestRowDetailsWhenFrozenMultiple);
            RunSteps += new TestStep(TestSetAndClearDetailsVisibilityForItem);
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initial Setup  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public override TestResult Setup()
        {
            base.Setup();

            Status("Setup specific for DataGridRowDetailsBVT");

            this.SetupDataSource();

            this.rowDetailsTemplate = MyDataGrid.RowDetailsTemplate;

            LogComment("Setup for DataGridRowDetailsBVT was successful");
            return TestResult.Pass;
        }

        public override TestResult CleanUp()
        {
            rowDetailsTemplate = null;
            return base.CleanUp();
        }

        /// <summary>
        /// Factors:
        /// RowDetailsVisibilityMode: {Collapsed, Visible, VisibleWhenSelected} 
        /// DataGridItem: {not NewItemPlaceholder, NewItemPlaceholder}
        /// DetailsTemplate: {has template, does not have template}
        /// Selection: {row is selected, row is not selected}
        /// 
        /// Transfer property tests not tested here
        /// DataGridRow.DetailsVisibility: {default, Collapsed, Visible, Hidden}
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestRowDetailsVisibility()
        {
            Status("TestRowDetailsVisibility");

            // init
            MyDataGrid.SelectionUnit = DataGridSelectionUnit.CellOrRowHeader;

            foreach (DataGridRowDetailsVisibilityMode visibilityMode in new[] { DataGridRowDetailsVisibilityMode.VisibleWhenSelected, DataGridRowDetailsVisibilityMode.Visible, DataGridRowDetailsVisibilityMode.Collapsed})
            {
                LogComment(string.Format("Begin testing with visibilityMode: {0}", visibilityMode));

                LogComment("set DataGrid.RowDetailsVisibilityMode");
                MyDataGrid.RowDetailsVisibilityMode = visibilityMode;

                if (visibilityMode == DataGridRowDetailsVisibilityMode.Collapsed)
                {
                    VerifyRowDetailsVisibilityForAll(0, Visibility.Collapsed);
                }
                else
                {
                    foreach (int rowIndex in new[] { 0, 1, MyDataGrid.Items.Count / 2, MyDataGrid.Items.Count - 1})
                    {
                        LogComment(string.Format("Begin testing with rowIndex: {0}", rowIndex));

                        LogComment("select the rowIndex");
                        DataGridActionHelper.ClickOnCell(MyDataGrid, rowIndex, 0);

                        foreach (bool hasDetailsTemplate in new[] { true, false})
                        {
                            LogComment(string.Format("Begin testing with hasDetailsTemplate: {0}", hasDetailsTemplate));

                            if (hasDetailsTemplate)
                            {
                                if (MyDataGrid.RowDetailsTemplate == null)
                                {
                                    MyDataGrid.RowDetailsTemplate = this.rowDetailsTemplate;
                                }
                            }
                            else
                            {
                                MyDataGrid.RowDetailsTemplate = null;
                            }

                            foreach (bool rowIsSelected in new[] { true, false})
                            {
                                LogComment(string.Format("Begin testing with rowIsSelected: {0}", rowIsSelected));

                                DataGridRow row = DataGridHelper.GetRow(MyDataGrid, rowIndex);

                                if (rowIsSelected)
                                {
                                    if (!row.IsSelected)
                                    {
                                        MyDataGrid.SelectedItems.Add(MyDataGrid.Items[rowIndex]);
                                        //DataGridActionHelper.ClickOnRowHeader(MyDataGrid, rowIndex);
                                        QueueHelper.WaitTillQueueItemsProcessed();
                                    }
                                }
                                else
                                {
                                    if (row.IsSelected)
                                    {
                                        MyDataGrid.SelectedItems.Remove(MyDataGrid.Items[rowIndex]);
                                        //DataGridActionHelper.ClickOnCell(MyDataGrid, rowIndex, 0);
                                        QueueHelper.WaitTillQueueItemsProcessed();
                                    }
                                }

                                // verify the correct propogation to DataGridRow.DetailsVisibility
                                if (visibilityMode == DataGridRowDetailsVisibilityMode.Visible)
                                {
                                    if (hasDetailsTemplate)
                                    {
                                        this.VerifyRowDetailsVisibilityForAll(rowIndex, Visibility.Visible);
                                    }
                                    else
                                    {
                                        this.VerifyRowDetailsVisibilityForAll(rowIndex, Visibility.Collapsed);
                                    }

                                }
                                else if (visibilityMode == DataGridRowDetailsVisibilityMode.VisibleWhenSelected)
                                {
                                    if (hasDetailsTemplate && rowIsSelected)
                                    {
                                        this.VerifyRowDetailsVisibileWhenSelected(rowIndex);
                                    }
                                    else
                                    {
                                        this.VerifyRowDetailsVisibilityForAll(rowIndex, Visibility.Collapsed);
                                    }
                                }

                                LogComment(string.Format("End testing with rowIsSelected: {0}", rowIsSelected));
                            }

                            LogComment(string.Format("End testing with hasDetailsTemplate: {0}", hasDetailsTemplate));
                        }

                        LogComment(string.Format("End testing with rowIndex: {0}", rowIndex));
                    }
                }

                LogComment(string.Format("End testing with visibilityMode: {0}", visibilityMode));
            }

            LogComment("TestRowDetailsVisibility was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify that RowDetails template is frozen when AreRowDetailsIsFrozen is set and VisibleWhenSelected.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestRowDetailsWhenFrozenSingle()
        {
            Status("TestRowDetailsWhenFrozenSingle");

            // init
            MyDataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;
            MyDataGrid.RowDetailsTemplate = this.rowDetailsTemplate;
            MyDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
            MyDataGrid.AutoGenerateColumns = true;

            foreach (bool isFrozen in new[] { true, false })
            {
                LogComment(string.Format("Begin testing with isFrozen: {0}", isFrozen));

                int rowIndex = 0;
                MyDataGrid.ScrollIntoView(MyDataGrid.Items[rowIndex], MyDataGrid.Columns[0]);
                QueueHelper.WaitTillQueueItemsProcessed();

                MyDataGrid.AreRowDetailsFrozen = isFrozen;

                LogComment("Select the row");
                MyDataGrid.SelectedItems.Clear();
                QueueHelper.WaitTillQueueItemsProcessed();
                MyDataGrid.SelectedItems.Add(MyDataGrid.Items[rowIndex]);
                QueueHelper.WaitTillQueueItemsProcessed();

                DataGridRow row = DataGridHelper.GetRow(MyDataGrid, rowIndex);
                DataGridDetailsPresenter presenter = DataGridHelper.FindVisualChild<DataGridDetailsPresenter>(row);
                ScrollViewer scrollViewer = DataGridHelper.FindVisualChild<ScrollViewer>(MyDataGrid);

                LogComment("Verify the RowDetails appear");
                if (row.DetailsVisibility != Visibility.Visible)
                {
                    throw new TestValidationException(string.Format("Row: {0} is expected to be visible but is not.", rowIndex));
                }

                GeneralTransform transformPrev = presenter.TransformToAncestor(MyDataGrid);

                LogComment("Scroll right");
                scrollViewer.PageRight();
                QueueHelper.WaitTillQueueItemsProcessed();

                LogComment("Verify the RowDetails template is frozen");
                GeneralTransform transformAfter = presenter.TransformToAncestor(MyDataGrid);
                this.VerifyTransforms(transformPrev, transformAfter, isFrozen);

                LogComment(string.Format("End testing with isFrozen: {0}", isFrozen));
            }

            LogComment("TestRowDetailsWhenFrozenSingle was successful");

            return TestResult.Pass;
        }

        /// <summary>
        /// Verify that RowDetails template is frozen when AreRowDetailsIsFrozen is set and Visible.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestRowDetailsWhenFrozenMultiple()
        {
            Status("TestRowDetailsWhenFrozenMultiple");

            // init
            MyDataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;
            MyDataGrid.RowDetailsTemplate = this.rowDetailsTemplate;
            MyDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Visible;
            MyDataGrid.AutoGenerateColumns = true;

            foreach (bool isFrozen in new[] { true, false })
            {
                LogComment(string.Format("Begin testing with isFrozen: {0}", isFrozen));

                int rowIndex = 0;
                MyDataGrid.ScrollIntoView(MyDataGrid.Items[rowIndex], MyDataGrid.Columns[0]);
                QueueHelper.WaitTillQueueItemsProcessed();

                MyDataGrid.AreRowDetailsFrozen = isFrozen;

                LogComment("Select the row");
                MyDataGrid.SelectedItems.Clear();
                QueueHelper.WaitTillQueueItemsProcessed();
                MyDataGrid.SelectedItems.Add(MyDataGrid.Items[rowIndex]);
                QueueHelper.WaitTillQueueItemsProcessed();

                // get rows 0, 1, and 2
                DataGridRow row = DataGridHelper.GetRow(MyDataGrid, 0);
                DataGridRow row1 = DataGridHelper.GetRow(MyDataGrid, 1);
                DataGridRow row2 = DataGridHelper.GetRow(MyDataGrid, 2);

                DataGridDetailsPresenter presenter = DataGridHelper.FindVisualChild<DataGridDetailsPresenter>(row);
                DataGridDetailsPresenter presenter1 = DataGridHelper.FindVisualChild<DataGridDetailsPresenter>(row1);
                DataGridDetailsPresenter presenter2 = DataGridHelper.FindVisualChild<DataGridDetailsPresenter>(row2);

                ScrollViewer scrollViewer = DataGridHelper.FindVisualChild<ScrollViewer>(MyDataGrid);

                LogComment("Verify the RowDetails appear");
                if (row.DetailsVisibility != Visibility.Visible || 
                    row1.DetailsVisibility != Visibility.Visible ||
                    row2.DetailsVisibility != Visibility.Visible)
                {
                    throw new TestValidationException(string.Format("Row: {0} is expected to be visible but is not.", rowIndex));
                }

                GeneralTransform transformPrev = presenter.TransformToAncestor(MyDataGrid);
                GeneralTransform transformPrev1 = presenter1.TransformToAncestor(MyDataGrid);
                GeneralTransform transformPrev2 = presenter2.TransformToAncestor(MyDataGrid);

                LogComment("Scroll right");
                scrollViewer.PageRight();
                QueueHelper.WaitTillQueueItemsProcessed();

                LogComment("Verify the RowDetails template is frozen");
                GeneralTransform transformAfter = presenter.TransformToAncestor(MyDataGrid);
                GeneralTransform transformAfter1 = presenter1.TransformToAncestor(MyDataGrid);
                GeneralTransform transformAfter2 = presenter2.TransformToAncestor(MyDataGrid);
                this.VerifyTransforms(transformPrev, transformAfter, isFrozen);
                this.VerifyTransforms(transformPrev1, transformAfter1, isFrozen);
                this.VerifyTransforms(transformPrev2, transformAfter2, isFrozen);                

                LogComment(string.Format("End testing with isFrozen: {0}", isFrozen));
            }

            LogComment("TestRowDetailsWhenFrozenMultiple was successful");

            return TestResult.Pass;
        }
        
        private TestResult TestSetAndClearDetailsVisibilityForItem()
        {
            Status("TestSetAndClearDetailsVisibilityForItem");

            // init
            MyDataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;
            MyDataGrid.RowDetailsTemplate = this.rowDetailsTemplate;
            MyDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;

            int index = MyDataGrid.Items.Count - 3;
            MyDataGrid.SetDetailsVisibilityForItem(MyDataGrid.Items[index], Visibility.Visible);

            DataGridRow row = DataGridHelper.GetRow(MyDataGrid, index);
            if (row.DetailsVisibility != Visibility.Visible)
            {
                throw new TestValidationException(string.Format("row.DetailsVisibility is incorrect after setting.  Expected: {0}, Actual: {1}", Visibility.Visible, row.DetailsVisibility));
            }

            MyDataGrid.ScrollIntoView(MyDataGrid.Items[0]);
            QueueHelper.WaitTillQueueItemsProcessed();

            MyDataGrid.ClearDetailsVisibilityForItem(MyDataGrid.Items[index]);

            row = DataGridHelper.GetRow(MyDataGrid, index);
            if (row.DetailsVisibility != Visibility.Collapsed)
            {
                throw new TestValidationException(string.Format("row.DetailsVisibility is incorrect after clearing.  Expected: {0}, Actual: {1}", Visibility.Collapsed, row.DetailsVisibility));
            }

            LogComment("TestSetAndClearDetailsVisibilityForItem was successful");

            return TestResult.Pass;
        }

        #endregion Test Steps

        #region Helpers

        private void VerifyRowDetailsVisibilityForAll(int rowIndex, Visibility expectedVisibility)
        {




            LogComment("verify the correct propogation to DataGridRow.DetailsVisibility");
            for (int i = 0; i < MyDataGrid.Items.Count - 1; i++)
            {
                DataGridRow row = DataGridHelper.GetRow(MyDataGrid, i);
                if (i == MyDataGrid.Items.Count - 1)
                {
                    if (row.DetailsVisibility != Visibility.Collapsed)
                    {
                        throw new TestValidationException(string.Format(
                            "Expect Visibility: {0}, Actual: {1}",
                            Visibility.Collapsed,
                            row.DetailsVisibility));
                    }
                }
                else
                {
                    if (row.DetailsVisibility != expectedVisibility)
                    {
                        throw new TestValidationException(string.Format(
                            "Expect Visibility: {0}, Actual: {1}",
                            expectedVisibility,
                            row.DetailsVisibility));
                    }
                }
            }
            
            MyDataGrid.ScrollIntoView(MyDataGrid.Items[rowIndex]);
            QueueHelper.WaitTillQueueItemsProcessed();
        }

        private void VerifyRowDetailsVisibileWhenSelected(int rowIndex)
        {
            LogComment("verify the correct propogation to DataGridRow.DetailsVisibility");
            for (int i = 0; i < MyDataGrid.Items.Count - 1; i++)
            {
                DataGridRow row = DataGridHelper.GetRow(MyDataGrid, i);
                if (rowIndex == i)
                {
                    if (row.DetailsVisibility != Visibility.Visible)
                    {
                        throw new TestValidationException(string.Format(
                            "Expect Visibility: {0}, Actual: {1}",
                            Visibility.Visible,
                            row.DetailsVisibility));
                    }
                }
                else
                {
                    if (row.DetailsVisibility != Visibility.Collapsed)
                    {
                        throw new TestValidationException(string.Format(
                            "Expect Visibility: {0}, Actual: {1}",
                            Visibility.Collapsed,
                            row.DetailsVisibility));
                    }
                }
            }

            LogComment("verify GetDetailsVisibilityForItem returns the correct info");
            for (int i = 0; i < MyDataGrid.Items.Count - 1; i++)
            {
                Visibility visibility = MyDataGrid.GetDetailsVisibilityForItem(MyDataGrid.Items[i]);
                if (rowIndex == i)
                {
                    if (visibility != Visibility.Visible)
                    {
                        throw new TestValidationException(string.Format(
                            "Expect Visibility: {0}, Actual: {1}",
                            Visibility.Visible,
                            visibility));
                    }
                }
                else
                {
                    if (visibility != Visibility.Collapsed)
                    {
                        throw new TestValidationException(string.Format(
                            "Expect Visibility: {0}, Actual: {1}",
                            Visibility.Collapsed,
                            visibility));
                    }
                }
            }

            MyDataGrid.ScrollIntoView(MyDataGrid.Items[rowIndex]);
            QueueHelper.WaitTillQueueItemsProcessed();
        }

        private void VerifyTransforms(GeneralTransform transformPrev, GeneralTransform transformAfter, bool isFrozen)
        {
            double epsilon = 0.0001;
            if (isFrozen)
            {
                if (!DataGridHelper.AreClose((transformPrev as MatrixTransform).Value.M11, (transformAfter as MatrixTransform).Value.M11, epsilon) ||
                    !DataGridHelper.AreClose((transformPrev as MatrixTransform).Value.M12, (transformAfter as MatrixTransform).Value.M12, epsilon) ||
                    !DataGridHelper.AreClose((transformPrev as MatrixTransform).Value.M21, (transformAfter as MatrixTransform).Value.M21, epsilon) ||
                    !DataGridHelper.AreClose((transformPrev as MatrixTransform).Value.M22, (transformAfter as MatrixTransform).Value.M22, epsilon) ||
                    !DataGridHelper.AreClose((transformPrev as MatrixTransform).Value.OffsetX, (transformAfter as MatrixTransform).Value.OffsetX, epsilon) ||
                    !DataGridHelper.AreClose((transformPrev as MatrixTransform).Value.OffsetY, (transformAfter as MatrixTransform).Value.OffsetY, epsilon))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("RowDetails did not stay frozen during a scroll when AreRowDetailsFrozen was set to true");
                    sb.Append(string.Format(
                        "prevTransform: M11: {0}, M12: {1}, M21: {2}, M22: {3}, OffSetX: {4}, OffSetY: {5}{6}",
                        (transformPrev as MatrixTransform).Value.M11, (transformPrev as MatrixTransform).Value.M12,
                        (transformPrev as MatrixTransform).Value.M21, (transformPrev as MatrixTransform).Value.M22,
                        (transformPrev as MatrixTransform).Value.OffsetX, (transformPrev as MatrixTransform).Value.OffsetY, Environment.NewLine));
                    sb.Append(string.Format(
                        "afterTransform: M11: {0}, M12: {1}, M21: {2}, M22: {3}, OffSetX: {4}, OffSetY: {5}{6}",
                        (transformAfter as MatrixTransform).Value.M11, (transformAfter as MatrixTransform).Value.M12,
                        (transformAfter as MatrixTransform).Value.M21, (transformAfter as MatrixTransform).Value.M22,
                        (transformAfter as MatrixTransform).Value.OffsetX, (transformAfter as MatrixTransform).Value.OffsetY, Environment.NewLine));
                    throw new TestValidationException(sb.ToString());
                }
            }
            else
            {
                if (DataGridHelper.AreClose((transformPrev as MatrixTransform).Value.M11, (transformAfter as MatrixTransform).Value.M11, epsilon) &&
                    DataGridHelper.AreClose((transformPrev as MatrixTransform).Value.M12, (transformAfter as MatrixTransform).Value.M12, epsilon) &&
                    DataGridHelper.AreClose((transformPrev as MatrixTransform).Value.M21, (transformAfter as MatrixTransform).Value.M21, epsilon) &&
                    DataGridHelper.AreClose((transformPrev as MatrixTransform).Value.M22, (transformAfter as MatrixTransform).Value.M22, epsilon) &&
                    DataGridHelper.AreClose((transformPrev as MatrixTransform).Value.OffsetX, (transformAfter as MatrixTransform).Value.OffsetX, epsilon) &&
                    DataGridHelper.AreClose((transformPrev as MatrixTransform).Value.OffsetY, (transformAfter as MatrixTransform).Value.OffsetY, epsilon))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("RowDetails was frozen during a scroll when AreRowDetailsFrozen was set to false.");
                    sb.Append(string.Format(
                        "prevTransform: M11: {0}, M12: {1}, M21: {2}, M22: {3}, OffSetX: {4}, OffSetY: {5}{6}",
                        (transformPrev as MatrixTransform).Value.M11, (transformPrev as MatrixTransform).Value.M12,
                        (transformPrev as MatrixTransform).Value.M21, (transformPrev as MatrixTransform).Value.M22,
                        (transformPrev as MatrixTransform).Value.OffsetX, (transformPrev as MatrixTransform).Value.OffsetY, Environment.NewLine));
                    sb.Append(string.Format(
                        "afterTransform: M11: {0}, M12: {1}, M21: {2}, M22: {3}, OffSetX: {4}, OffSetY: {5}{6}",
                        (transformAfter as MatrixTransform).Value.M11, (transformAfter as MatrixTransform).Value.M12,
                        (transformAfter as MatrixTransform).Value.M21, (transformAfter as MatrixTransform).Value.M22,
                        (transformAfter as MatrixTransform).Value.OffsetX, (transformAfter as MatrixTransform).Value.OffsetY, Environment.NewLine));
                    throw new TestValidationException(sb.ToString());
                }
            }
        }

        #endregion Helpers
    }
}
