using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text;
using Microsoft.Test;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Controls.Data
{
    /// <summary>
    /// This static class contains the test-case specification detail 
    /// </summary>
    internal static class ResizeDefaultTestData
    {
        #region Private memebers

        /// <summary>
        /// Populate a target dictionary from a source dictionary -- shallow copies
        /// </summary>
        private static void CloneTestVariation(Dictionary<String, Object> source, Dictionary<String, Object> target)
        {
            foreach (String key in source.Keys)
            {
                target.Add(key, source[key]);
            }
        }

        private static class Constants
        {
            private static readonly Size verticalAspect;
            private static readonly Size horizontalAspect;

            static Constants()
            {
                verticalAspect = new Size(10, 20);
                horizontalAspect = new Size(20, 10);
            }

            public static Double deltaX { get { return 150.0; } }
            public static Double deltaY { get { return 150.0; } }
            public static Double tolerance { get { return 0.1; } }
            public static Double margin { get { return 50.0; } }
            public static Size aspectVertical { get { return verticalAspect; } }
            public static Size aspectHorizontal { get { return horizontalAspect; } }
            public static Int32 splitterMinWidth { get { return ((Int32)verticalAspect.Width) / 2; } }
            public static Int32 splitterMinHeight { get { return splitterMinWidth; } }
        }

        #endregion

        #region (private) GridSplitterActionDelegate Implementations 

        /// <summary>
        /// Delegate that makes a single call to ITransformProvider.Move(x,y) based on the
        /// two values of a Double[2] array Object[] actionArgs. The ITransformProvide
        /// is obtained from the AutomationPeer for GridSplitter.
        /// Returns two GridDefinitionSnapShot objects: "before" and "after" the invokation
        /// </summary>
        private static void MoveViaPeerAction(out GridDefinitionSnapshot snapBefore,
                                       out GridDefinitionSnapshot snapAfter,
                                       Grid grid,
                                       GridSplitter splitter,
                                       Object[] actionArgs)
        {
            // Verify parameter expectations
            if (actionArgs == null || actionArgs.Length != 1
                || !(actionArgs[0] is Double[]) || ((Double[])actionArgs[0]).Length != 2)
            {
                throw new TestValidationException(
                    "MoveViaPeerAction 'actionArgs' parameter has wrong structure");
            }

            GridSplitterAutomationPeer autoPeer = new GridSplitterAutomationPeer(splitter);
            if (autoPeer == null)
            {
                throw new TestValidationException("splitter autoPeer is null");
            }

            ITransformProvider splitterPeer
                = (ITransformProvider)autoPeer.GetPattern(PatternInterface.Transform);

            if (splitterPeer == null)
            {
                throw new TestValidationException(
                    "splitter autoPeer ITransformProvider is null");
            }

            // Extract values from untyped parameters
            //
            Double xDelta = ((Double[])actionArgs[0])[0];
            Double yDelta = ((Double[])actionArgs[0])[1];

            snapBefore = GridSplitterGridExtend.GetDefinitionSnapshot(grid);

            // Perform a single action
            splitterPeer.Move(xDelta, yDelta);

            snapAfter = GridSplitterGridExtend.GetDefinitionSnapshot(grid);
        }

        #endregion

        #region (private) GridSplitterVerifyDelegate Implementations

        private enum Relative
        {
            Previous = -1,
            Current,
            Next
        }

        private interface IBehaviorPredicates
        {
            Boolean Check(Relative relative);
            Int32 DiffChangeCount { get; }
        }

        /// <summary>
        /// Functor-like object so that 'Check' method can be called.
        /// Helper class for "VerifyResizeBehavior" and VerifyAutoResizeDirection delegates:
        /// Genericize on T[] to abstract away the difference between a RowDiff[] and ColumnDiff[].
        /// Implement logic of interpreting a T[] in terms of Grid.ResizeBehavior semantics
        /// and GridDefinitionSnapshot.IDiff design.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class BehaviorPredicates<T> : IBehaviorPredicates where T : GridDefinitionSnapshot.IDiff
        {
            #region Private members

            /// <summary>
            /// Helper for accessing a RowDiff or ColumnDiff as an IDiff
            /// for the explicitly implemented property "IDiff.ActualDimension"
            /// </summary>
            private static class AdaptDiff
            {
                public static Double Actual(GridDefinitionSnapshot.IDiff diff)
                {
                    return diff.ActualDimension;
                }
            }

            // See constructor comments for field descriptions
            private readonly Boolean expectChanges;
            private readonly Int32 splitterIndex;
            private readonly Double delta;
            private readonly Double tolerance;
            private GridResizeBehavior resizeBehavior;
            private T[] diffs;

            private Boolean TestForIncrease(Double testValue, Double expectedValue, Double tolerance)
            {
                return Math.Abs(testValue - expectedValue) <= tolerance;
            }

            private Boolean TestForDecrease(Double testValue, Double expectedValue, Double tolerance)
            {
                return Math.Abs(testValue + expectedValue) <= tolerance;
            }

            private Boolean TestForNoChange(Double testValue, Double tolerance)
            {
                return Math.Abs(testValue) <= tolerance;
            }

            /// <summary>
            /// Construct helper object so that 'Check' method can be called.
            /// </summary>
            /// <param name="expectChanges">Modality of 'Check' method: 'false' overrides resizeBehavior param.</param>
            /// <param name="resizeBehavior">Modality of 'Check' method: the GridResizeBehavior semantics to use.</param>
            /// <param name="splitterIndex">The GridSplitter's Grid.Row or Grid.Column value.</param>
            /// <param name="diffs">The RowDiff[] or ColumnDiff[] whose elements are being evaluated.</param>
            /// <param name="delta">The expected absolute value of a T[] element that has changed.</param>
            /// <param name="tolerance">The numeric tolerance for comparisons between expected and acutal values.</param>
            private BehaviorPredicates(Boolean expectChanges, 
                                       GridResizeBehavior resizeBehavior,
                                       Int32 splitterIndex,
                                       T[] diffs,
                                       Double delta,
                                       Double tolerance)
            {
                this.expectChanges = expectChanges;
                this.resizeBehavior = resizeBehavior;
                this.splitterIndex = splitterIndex;
                this.diffs = diffs;
                this.delta = delta;
                this.tolerance = tolerance;

                // Preconditions:
                // *  RowDiff[] or ColumnDiff[] must represent a least three rows (or columns)
                // * there must be at least one row (or column) on each side of the row (or column) occupied by the GridSplitter
                if (null == diffs || diffs.Length < 3)
                {
                    throw new TestValidationException(
                        "BehaviorPredicates constructor parameter requires: (diffs.Length >= 3)");
                }
                if (splitterIndex < 1 || splitterIndex > diffs.Length )
                {
                    throw new TestValidationException(
                        "BehaviorPredicates constructor parameter requires: (splitterIndex >= 1) && (splitterIndex <= diffs.Length)");
                }
            }

            #endregion

            #region Public members

            // See private constructor for parameter comments
            public BehaviorPredicates(Int32 splitterIndex, T[] diffs, Double tolerance)
                : this(false, GridResizeBehavior.PreviousAndNext, splitterIndex, diffs, 0.0, tolerance) {}

            // See private constructor for parameter comments
            public BehaviorPredicates(GridResizeBehavior resizeBehavior, Int32 splitterIndex, T[] diffs, Double delta, Double tolerance)
                : this(true, resizeBehavior, splitterIndex, diffs, delta, tolerance) {}

            public Int32 DiffChangeCount
            {
                get
                {
                    Int32 count = 0;
                    foreach (T diff in diffs)
                    {
                        if (!TestForNoChange( AdaptDiff.Actual(diff), tolerance))
                        {
                            ++count;
                        }
                    }
                    return count;
                }
            }

            public Boolean Check(Relative relative)
            {
                Int32 idx = splitterIndex + (Int32)relative;

                if (!expectChanges)
                {
                    return TestForNoChange(AdaptDiff.Actual(diffs[idx]), tolerance);
                }
                switch (resizeBehavior)
                {
                    case GridResizeBehavior.PreviousAndCurrent:
                        switch (relative)
                        {
                            case Relative.Previous:
                                return TestForIncrease(AdaptDiff.Actual(diffs[idx]), delta, tolerance);
                            case Relative.Current:
                                return TestForDecrease(AdaptDiff.Actual(diffs[idx]), delta, tolerance);
                            case Relative.Next:
                                return TestForNoChange(AdaptDiff.Actual(diffs[idx]), tolerance);
                        }
                        break;
                    case GridResizeBehavior.CurrentAndNext:
                        switch (relative)
                        {
                            case Relative.Previous:
                                return TestForNoChange(AdaptDiff.Actual(diffs[idx]), tolerance);
                            case Relative.Current:
                                return TestForIncrease(AdaptDiff.Actual(diffs[idx]), delta, tolerance);
                            case Relative.Next:
                                return TestForDecrease(AdaptDiff.Actual(diffs[idx]), delta, tolerance);
                        }
                        break;
                    case GridResizeBehavior.PreviousAndNext:
                        switch (relative)
                        {
                            case Relative.Previous:
                                return TestForIncrease(AdaptDiff.Actual(diffs[idx]), delta, tolerance);
                            case Relative.Current:
                                return TestForNoChange(AdaptDiff.Actual(diffs[idx]), tolerance);
                            case Relative.Next:
                                return TestForDecrease(AdaptDiff.Actual(diffs[idx]), delta, tolerance);
                        }
                        break;
                    default:
                        throw new TestValidationException("GridResizeBehavior has impossible value");
                }
                return false;
            }
            #endregion
        }

        /// Delegate that is specified as an entry in test variation data that is built by
        /// method "InitializeResizeBehaviorTestVariations()" called by 
        ///                       GridSplitterResizeDefaultsTests.SetUp(),
        /// and invoked by 
        ///                       GridSplitterResizeDefaultsTests.RunResizeDefaultsTests().
        /// The arguments to the call come from the same test
        /// variation entry. (See class GridSplitterVariationContext)
        private static TestResult VerifyResizeBehavior(Int32 variationIndex,
                                                         Grid grid,
                                                         GridSplitter splitter,
                                                         GridSplitterActionDelegate action,
                                                         Object[] actionArgs,
                                                         Object[] verifyArgs)
        {
            TestLog.Current.LogStatus(
                "Test index: " + variationIndex.ToString() + ", Entered VerifyResizeBehavior");

            // Verify parameter expectations
            if (verifyArgs == null || verifyArgs.Length != 3
                 || !(verifyArgs[0] is Double)
                 || !(verifyArgs[1] is Double[]) || ((Double[])verifyArgs[1]).Length != 2
                 || !(verifyArgs[2] is GridResizeBehavior)
               )
            {
                throw new TestValidationException(
                    "VerifyResizeBehavior: A 'verifyArgs' parameter has wrong structure");
            }

            GridDefinitionSnapshot snapBefore = null;
            GridDefinitionSnapshot snapAfter = null;

            // Invoke the action delegate
            //
            action(out snapBefore, out snapAfter, grid, splitter, actionArgs);

            // Process the resulting snapshots to accomplish verification
            //
            GridDefinitionSnapshot.RowDiff[] rowDiffs = snapBefore.CompareRows(snapAfter);
            GridDefinitionSnapshot.ColumnDiff[] columnDiffs = snapBefore.CompareColumns(snapAfter);

            IBehaviorPredicates changeRules = null;
            IBehaviorPredicates unchangeRules = null;

            Int32 splitterRow = (Int32)splitter.GetValue(Grid.RowProperty);
            Int32 splitterColumn = (Int32)splitter.GetValue(Grid.ColumnProperty);

            // Extract values from untyped parameters
            //
            Double tolerance = (Double)verifyArgs[0];
            Double xDelta = ((Double[])verifyArgs[1])[0];
            Double yDelta = ((Double[])verifyArgs[1])[1];
            GridResizeBehavior resizeBehavior = (GridResizeBehavior)verifyArgs[2];

            // Instantiate the correct BehaviorPredicates<T> helper classes
            // 
            StringBuilder exceptionStringRoot
                = new StringBuilder("VerifyResizeBehavior " + resizeBehavior.ToString());
            String changeRuleExceptionString = null;
            String unchangeRuleExceptionString = null;

            if (GridResizeDirection.Rows == splitter.ResizeDirection)
            {
                changeRules = new BehaviorPredicates<GridDefinitionSnapshot.RowDiff>
                    (resizeBehavior, splitterRow, rowDiffs, yDelta, tolerance);

                unchangeRules = new BehaviorPredicates<GridDefinitionSnapshot.ColumnDiff>
                    (splitterColumn, columnDiffs, tolerance);

                changeRuleExceptionString
                    = (exceptionStringRoot.Append("; Failed in ROW Direction.")).ToString();

                unchangeRuleExceptionString
                    = (exceptionStringRoot.Append("; for ROWS found COLUMN changes.")).ToString();
            }
            else // GridResizeDirection.Columns
            {
                changeRules = new BehaviorPredicates<GridDefinitionSnapshot.ColumnDiff>
                    (resizeBehavior, splitterColumn, columnDiffs, xDelta, tolerance);

                unchangeRules = new BehaviorPredicates<GridDefinitionSnapshot.RowDiff>
                    (splitterRow, rowDiffs, tolerance);

                changeRuleExceptionString
                    = (exceptionStringRoot.Append("; Failed in COLUMN Direction.")).ToString();

                unchangeRuleExceptionString
                    = (exceptionStringRoot.Append("; for COLUMNS found ROW changes.")).ToString();
            }

            // Execute the verification details
            //
            if ( !(   changeRules.Check(Relative.Previous)
                   && changeRules.Check(Relative.Current)
                   && changeRules.Check(Relative.Next)))
            {
                throw new TestValidationException(changeRuleExceptionString);
            }

            if ( !(   unchangeRules.Check(Relative.Previous)
                   && unchangeRules.Check(Relative.Current)
                   && unchangeRules.Check(Relative.Next)))
            {
                throw new TestValidationException(unchangeRuleExceptionString);
            }
            TestLog.Current.LogStatus("Finished VerifyResizeBehavior");
            return TestResult.Pass;
        }

        /// Delegate that is specified as an entry in test variation data that is built by
        /// method "InitializeResizeDirectionTestVariations()" called by 
        ///                       GridSplitterResizeDefaultsTests.SetUp(),
        /// and invoked by 
        ///                       GridSplitterResizeAndIncrementTests.RunTests().
        /// The arguments to the call come from the same test
        /// variation entry. (See class GridSplitterVariationContext)
        private static TestResult VerifyAutoResizeDirection( Int32 variationIndex,
                                                             Grid grid,
                                                             GridSplitter splitter,
                                                             GridSplitterActionDelegate action,
                                                             Object[] actionArgs,
                                                             Object[] verifyArgs)
        {
            TestLog.Current.LogStatus(
                "Test index: " + variationIndex.ToString() + ", Entered VerifyAutoResizeDirection");

            // Verify parameter expectations            
            if (verifyArgs == null || verifyArgs.Length != 2
                 || !(verifyArgs[0] is Double)
                 || !(verifyArgs[1] is GridResizeDirection)
               )
            {
                throw new TestValidationException(
                    "VerifyAutoResizeDirection: A 'verifyArgs' parameter has wrong structure");
            }

            GridDefinitionSnapshot snapBefore;
            GridDefinitionSnapshot snapAfter;

            // Invoke the action delegate
            //
            action(out snapBefore, out snapAfter, grid, splitter, actionArgs);

            // Process the resulting snapshots to accomplish verification
            //
            GridDefinitionSnapshot.RowDiff[] rowDiffs = snapBefore.CompareRows(snapAfter);
            GridDefinitionSnapshot.ColumnDiff[] columnDiffs = snapBefore.CompareColumns(snapAfter);

            IBehaviorPredicates expectUnchanged;
            IBehaviorPredicates expectChanged;

            Int32 splitterRow = (Int32)splitter.GetValue(Grid.RowProperty);
            Int32 splitterColumn = (Int32)splitter.GetValue(Grid.ColumnProperty);

            // Extract values from untyped parameters
            //
            Double tolerance = (Double)verifyArgs[0];
            GridResizeDirection expectedResizeDirection = (GridResizeDirection)verifyArgs[1];

            // Instantiate the correct BehaviorPredicates<T> helper classes
            //
            StringBuilder exceptionStringRoot
                = new StringBuilder("VerifyAutoResizeDirection " + expectedResizeDirection.ToString());
            String foundChangesExceptionString;
            String missingChangesExceptionString;

            if (GridResizeDirection.Rows == expectedResizeDirection)
            {
                expectChanged   = new BehaviorPredicates<GridDefinitionSnapshot.RowDiff>
                    (splitterRow, rowDiffs, tolerance);

                expectUnchanged = new BehaviorPredicates<GridDefinitionSnapshot.ColumnDiff>
                    (splitterColumn, columnDiffs, tolerance);

                foundChangesExceptionString
                    = (exceptionStringRoot.Append("; found COLUMN changes.")).ToString();

                missingChangesExceptionString
                    = (exceptionStringRoot.Append("; did not find ROW changes.")).ToString();
            }
            else // GridResizeDirection.Columns
            {
                expectChanged   = new BehaviorPredicates<GridDefinitionSnapshot.ColumnDiff>
                    (splitterColumn, columnDiffs, tolerance);

                expectUnchanged = new BehaviorPredicates<GridDefinitionSnapshot.RowDiff>
                    (splitterRow, rowDiffs, tolerance);

                foundChangesExceptionString
                    = (exceptionStringRoot.Append("; found ROW changes.")).ToString();

                missingChangesExceptionString
                    = (exceptionStringRoot.Append("; did not find COLUMN changes.")).ToString();
            }

            // any ResizeBehavior changes at least two offsets
            if (expectChanged.DiffChangeCount < 2)
            {
                throw new TestValidationException(missingChangesExceptionString);
            }

            if (expectUnchanged.DiffChangeCount > 0)
            {
                throw new TestValidationException(foundChangesExceptionString);
            }

            TestLog.Current.LogStatus("Finished VerifyAutoResizeDirection");
            return TestResult.Pass;
        }

        #endregion

        #region Public Methods

        public static Boolean AssertResizeDefaultsTestsPreconditions(Panel parentPanel, Grid grid)
        {
            if (grid.RowDefinitions.Count < 3)
            {
                throw new TestValidationException("GridSplitterResizeDefaultsTests assumption failed: Grid.RowDefinitions.Count >= 3");
            }
            if (grid.ColumnDefinitions.Count < 3)
            {
                throw new TestValidationException("GridSplitterResizeDefaultsTests assumption failed: Grid.ColumnDefinitions.Count >= 3");
            }
            if (parentPanel.ActualWidth < (grid.ColumnDefinitions.Count * Constants.deltaX + Constants.margin))
            {
                throw new TestValidationException(
                    "GridSplitterResizeDefaultsTests assumption failed:\n"
                    + String.Format("parentPanel.ActualWidth >= (grid.ColumnDefinitions.Count * {0}) + {1}", Constants.deltaX, Constants.margin));
            }
            if (parentPanel.ActualHeight < (grid.RowDefinitions.Count * Constants.deltaY + Constants.margin))
            {
                throw new TestValidationException(
                    "GridSplitterResizeDefaultsTests assumption failed:\n"
                    + String.Format("parentPanel.ActualWidth >= (grid.RowDefinitions.Count * {0}) + {1}", Constants.deltaY, Constants.margin));
            }
            return true;
        }

        /// <summary>
        /// Test-case specification data for the "ResizeBehavior" test variation.
        /// Builds a dictionary of test specifications that are executed by 
        /// GridSplitterResizeAndIncrementTests.RunTests()
        /// </summary>
        public static Dictionary<String, Object>[] InitializeResizeBehaviorTestVariations()
        {
            Dictionary<String, Object> testVariation;
            Dictionary<String, Object>[] testVariations = new Dictionary<String, Object>[32];
            //---- [0 ] comment --------------------------
            // ResizeDirection.Rows && VerticalAlignment.Stretch
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            testVariation.Add("PanelGridResourceKey", "GridResizeAlpha");
            testVariation.Add("SplitterMinWidth", Constants.splitterMinWidth);
            testVariation.Add("SplitterMinHeight", Constants.splitterMinHeight);
            testVariation.Add("Grid.Row", (Int32)1);
            testVariation.Add("Grid.Column", (Int32)1);
            testVariation.Add("SplitterColor", Brushes.Red);
            testVariation.Add("ResizeBehavior", GridResizeBehavior.BasedOnAlignment);
            testVariation.Add("ResizeDirection", GridResizeDirection.Rows);
            testVariation.Add("VerticalAlignment", VerticalAlignment.Stretch);
            testVariation.Add("HorizontalAlignment", HorizontalAlignment.Stretch);
            testVariation.Add("WidthHeight", null);
            testVariation.Add(
                "Action", new GridSplitterActionDelegate(MoveViaPeerAction));
            testVariation.Add(
                "ActionArgs", new Object[] { new Double[] { Constants.deltaX, Constants.deltaY } });
            testVariation.Add("Verify", new GridSplitterVerifyDelegate(VerifyResizeBehavior));
            testVariation.Add(
                "VerifyArgs", new Object[] { 
                    (Double)Constants.tolerance,
                    new Double[] { Constants.deltaX, Constants.deltaY },
                    GridResizeBehavior.PreviousAndNext });
            testVariations[0] = testVariation;

            //---- [1 ] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[0], testVariation);
            testVariation["HorizontalAlignment"] = HorizontalAlignment.Left;
            testVariations[1] = testVariation;

            //---- [2 ] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[0], testVariation);
            testVariation["HorizontalAlignment"] = HorizontalAlignment.Right;
            testVariations[2] = testVariation;
            
            //---- [3 ] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[0], testVariation);
            testVariation["HorizontalAlignment"] = HorizontalAlignment.Center;
            testVariations[3] = testVariation;

            //---- [4 ] comment --------------------------
            // ResizeDirection.Rows && VerticalAlignment.Top
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[0], testVariation);
            testVariation["VerticalAlignment"] = VerticalAlignment.Top;
            testVariation["VerifyArgs"] = new Object[] 
                {   (Double)Constants.tolerance,
                    new Double[] { Constants.deltaX, Constants.deltaY },
                    GridResizeBehavior.PreviousAndCurrent
                };
            testVariations[4] = testVariation;
            
            //---- [5 ] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[4], testVariation);
            testVariation["HorizontalAlignment"] = HorizontalAlignment.Left;
            testVariations[5] = testVariation;

            //---- [6 ] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[4], testVariation);
            testVariation["HorizontalAlignment"] = HorizontalAlignment.Right;
            testVariations[6] = testVariation;
            
            //---- [7 ] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[4], testVariation);
            testVariation["HorizontalAlignment"] = HorizontalAlignment.Center;
            testVariations[7] = testVariation;

            //---- [8 ] comment --------------------------
            // ResizeDirection.Rows && VerticalAlignment.Bottom
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[0], testVariation);
            testVariation["VerticalAlignment"] = VerticalAlignment.Bottom;
            testVariation["VerifyArgs"] = new Object[] 
                {   (Double)Constants.tolerance,
                    new Double[] { Constants.deltaX, Constants.deltaY },
                    GridResizeBehavior.CurrentAndNext
                };
            testVariations[8] = testVariation;

            //---- [9 ] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[8], testVariation);
            testVariation["HorizontalAlignment"] = HorizontalAlignment.Left;
            testVariations[9] = testVariation;
            
            //---- [10] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[8], testVariation);
            testVariation["HorizontalAlignment"] = HorizontalAlignment.Right;
            testVariations[10] = testVariation;
            
            //---- [11] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[8], testVariation);
            testVariation["HorizontalAlignment"] = HorizontalAlignment.Center;
            testVariations[11] = testVariation;

            //---- [12] comment --------------------------
            //--------------------------------------------
            // ResizeDirection.Rows && VerticalAlignment.Center
            //------------------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[0], testVariation);
            testVariation["VerticalAlignment"] = VerticalAlignment.Center;
            testVariation["VerifyArgs"] = new  Object[] 
                {   (Double)Constants.tolerance,
                    new Double[] { Constants.deltaX, Constants.deltaY },
                    GridResizeBehavior.PreviousAndNext
                };
            testVariations[12] = testVariation;

            //---- [13] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[12], testVariation);
            testVariation["HorizontalAlignment"] = HorizontalAlignment.Left;
            testVariations[13] = testVariation;

            //---- [14] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[12], testVariation);
            testVariation["HorizontalAlignment"] = HorizontalAlignment.Right;
            testVariations[14] = testVariation;

            //---- [15] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[12], testVariation);
            testVariation["HorizontalAlignment"] = HorizontalAlignment.Center;
            testVariations[15] = testVariation;

            //---- [16] comment --------------------------
            // ResizeDirection.Columns && HorizontalAlignment.Stretch
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            testVariation.Add("PanelGridResourceKey", "GridResizeAlpha");
            testVariation.Add("SplitterMinWidth", Constants.splitterMinWidth);
            testVariation.Add("SplitterMinHeight", Constants.splitterMinHeight);
            testVariation.Add("Grid.Row", (Int32)1);
            testVariation.Add("Grid.Column", (Int32)1);
            testVariation.Add("SplitterColor", Brushes.Red);
            testVariation.Add("ResizeBehavior", GridResizeBehavior.BasedOnAlignment);
            testVariation.Add("ResizeDirection", GridResizeDirection.Columns);
            testVariation.Add("VerticalAlignment", VerticalAlignment.Stretch);
            testVariation.Add("HorizontalAlignment", HorizontalAlignment.Stretch);
            testVariation.Add("WidthHeight", null);
            testVariation.Add(
                "Action", new GridSplitterActionDelegate(MoveViaPeerAction));
            testVariation.Add(
                "ActionArgs", new Object[] { new Double[] { Constants.deltaX, Constants.deltaY } });
            testVariation.Add("Verify", new GridSplitterVerifyDelegate(VerifyResizeBehavior));
            testVariation.Add(
                "VerifyArgs", new Object[] { 
                    (Double)Constants.tolerance,
                    new Double[] { Constants.deltaX, Constants.deltaY },
                    GridResizeBehavior.PreviousAndNext });
            testVariations[16] = testVariation;

            //---- [17] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[16], testVariation);
            testVariation["VerticalAlignment"] = VerticalAlignment.Top;
            testVariations[17] = testVariation;
            
            //---- [18] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[16], testVariation);
            testVariation["VerticalAlignment"] = VerticalAlignment.Bottom;
            testVariations[18] = testVariation;
            
            //---- [19] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[16], testVariation);
            testVariation["VerticalAlignment"] = VerticalAlignment.Center;
            testVariations[19] = testVariation;
            
            //---- [20] comment --------------------------
            //--------------------------------------------
            // ResizeDirection.Columns && HorizontalAlignment.Left
            //------------------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[16], testVariation);
            testVariation["HorizontalAlignment"] = HorizontalAlignment.Left;
            testVariation["VerifyArgs"] = new Object[] 
                {   (Double)Constants.tolerance,
                    new Double[] { Constants.deltaX, Constants.deltaY },
                    GridResizeBehavior.PreviousAndCurrent
                };
            testVariations[20] = testVariation;
            
            //---- [21] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[20], testVariation);
            testVariation["VerticalAlignment"] = VerticalAlignment.Top;
            testVariations[21] = testVariation;

            //---- [22] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[20], testVariation);
            testVariation["VerticalAlignment"] = VerticalAlignment.Bottom;
            testVariations[22] = testVariation;
            
            //---- [23] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[20], testVariation);
            testVariation["VerticalAlignment"] = VerticalAlignment.Center;
            testVariations[23] = testVariation;

            //---- [24] comment --------------------------
            // ResizeDirection.Columns && HorizontalAlignment.Right
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[16], testVariation);
            testVariation["HorizontalAlignment"] = HorizontalAlignment.Right;
            testVariation["VerifyArgs"] = new Object[] 
                {   (Double)Constants.tolerance,
                    new Double[] { Constants.deltaX, Constants.deltaY },
                    GridResizeBehavior.CurrentAndNext
                };
            testVariations[24] = testVariation;

            //---- [25] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[24], testVariation);
            testVariation["VerticalAlignment"] = VerticalAlignment.Top;
            testVariations[25] = testVariation;

            //---- [26] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[24], testVariation);
            testVariation["VerticalAlignment"] = VerticalAlignment.Bottom;
            testVariations[26] = testVariation;
            
            //---- [27] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[24], testVariation);
            testVariation["VerticalAlignment"] = VerticalAlignment.Center;
            testVariations[27] = testVariation;

            //---- [28] comment --------------------------
            // ResizeDirection.Columns && HorizontalAlignment.Center
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[16], testVariation);
            testVariation["HorizontalAlignment"] = HorizontalAlignment.Center;
            testVariation["VerifyArgs"] = new Object[] 
                {   (Double)Constants.tolerance,
                    new Double[] { Constants.deltaX, Constants.deltaY },
                    GridResizeBehavior.PreviousAndNext
                };
            testVariations[28] = testVariation;

            //---- [29] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[28], testVariation);
            testVariation["VerticalAlignment"] = VerticalAlignment.Top;
            testVariations[29] = testVariation;

            //---- [30] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[28], testVariation);
            testVariation["VerticalAlignment"] = VerticalAlignment.Bottom;
            testVariations[30] = testVariation;
            
            //---- [31] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[28], testVariation);
            testVariation["VerticalAlignment"] = VerticalAlignment.Center;
            testVariations[31] = testVariation;

            //------------------------------------------------------
            return testVariations;
        }

        /// <summary>
        /// Test-case specification data for the "ResizeDirection" test variation.
        /// This variation tests that GridResizeDirection "Auto" effectively
        /// resolves to rows or columns based on splitter alignment and aspect.
        /// Builds a dictionary of test specifications that are executed by 
        /// GridSplitterResizeAndIncrementTests.RunTests()
        /// </summary>
        public static Dictionary<String, Object>[] InitializeResizeDirectionTestVariations()
        {
            Dictionary<String, Object> testVariation;
            Dictionary<String, Object>[] testVariations = new Dictionary<String, Object>[32];
            //---- [0 ] comment --------------------------
            // HorizontalAlignment.Stretch
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            testVariation.Add("PanelGridResourceKey", "GridResizeAlpha");
            testVariation.Add("SplitterMinWidth", Constants.splitterMinWidth);
            testVariation.Add("SplitterMinHeight", Constants.splitterMinHeight);
            testVariation.Add("Grid.Row", (Int32)1);
            testVariation.Add("Grid.Column", (Int32)1);
            testVariation.Add("SplitterColor", Brushes.Red);
            testVariation.Add("ResizeDirection", GridResizeDirection.Auto);
            testVariation.Add("ResizeBehavior", GridResizeBehavior.PreviousAndNext);
            testVariation.Add("HorizontalAlignment", HorizontalAlignment.Stretch);
            testVariation.Add("VerticalAlignment", VerticalAlignment.Stretch);
            testVariation.Add("WidthHeight", Constants.aspectVertical);
            testVariation.Add(
                "Action", new GridSplitterActionDelegate(MoveViaPeerAction));
            testVariation.Add(
                "ActionArgs", new Object[] { new Double[] { Constants.deltaX, Constants.deltaY } });
            testVariation.Add("Verify", new GridSplitterVerifyDelegate(VerifyAutoResizeDirection));
            testVariation.Add(
                "VerifyArgs", new Object[] { (Double)Constants.tolerance, GridResizeDirection.Columns });
            testVariations[0] = testVariation;

            //---- [1 ] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[0], testVariation);
            testVariation["WidthHeight"] = Constants.aspectHorizontal;
            testVariation["VerifyArgs"] = new Object[] {(Double)Constants.tolerance, GridResizeDirection.Rows };
            testVariations[1] = testVariation;

            //---- [2 ] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[0], testVariation);
            testVariation["VerticalAlignment"] = VerticalAlignment.Top;
            testVariation["VerifyArgs"] = new Object[] { (Double)Constants.tolerance, GridResizeDirection.Rows };
            testVariations[2] = testVariation;

            //---- [3 ] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[2], testVariation);
            testVariation["WidthHeight"] = Constants.aspectHorizontal;
            testVariations[3] = testVariation;

            //---- [4 ] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[0], testVariation);
            testVariation["VerticalAlignment"] = VerticalAlignment.Bottom;
            testVariation["VerifyArgs"] = new Object[] { (Double)Constants.tolerance, GridResizeDirection.Rows };
            testVariations[4] = testVariation;

            //---- [5 ] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[4], testVariation);
            testVariation["WidthHeight"] = Constants.aspectHorizontal;
            testVariations[5] = testVariation;

            //---- [6 ] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[0], testVariation);
            testVariation["VerticalAlignment"] = VerticalAlignment.Center;
            testVariation["VerifyArgs"] = new Object[] { (Double)Constants.tolerance, GridResizeDirection.Rows };
            testVariations[6] = testVariation;

            //---- [7 ] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[6], testVariation);
            testVariation["WidthHeight"] = Constants.aspectHorizontal;
            testVariations[7] = testVariation;

            //---- [8 ] comment --------------------------
            // HorizontalAlignment.Left
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            testVariation.Add("PanelGridResourceKey", "GridResizeAlpha");
            testVariation.Add("SplitterMinWidth", Constants.splitterMinWidth);
            testVariation.Add("SplitterMinHeight", Constants.splitterMinHeight);
            testVariation.Add("Grid.Row", (Int32)1);
            testVariation.Add("Grid.Column", (Int32)1);
            testVariation.Add("SplitterColor", Brushes.Red);
            testVariation.Add("ResizeDirection", GridResizeDirection.Auto);
            testVariation.Add("ResizeBehavior", GridResizeBehavior.PreviousAndNext);
            testVariation.Add("HorizontalAlignment", HorizontalAlignment.Left);
            testVariation.Add("VerticalAlignment", VerticalAlignment.Stretch);
            testVariation.Add("WidthHeight", Constants.aspectVertical);
            testVariation.Add(
                "Action", new GridSplitterActionDelegate(MoveViaPeerAction));
            testVariation.Add(
                "ActionArgs", new Object[] { new Double[] { Constants.deltaX, Constants.deltaY } });
            testVariation.Add("Verify", new GridSplitterVerifyDelegate(VerifyAutoResizeDirection));
            testVariation.Add(
                "VerifyArgs", new Object[] { (Double)Constants.tolerance, GridResizeDirection.Columns });
            testVariations[8] = testVariation;

            //---- [9 ] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[8], testVariation);
            testVariation["WidthHeight"] = Constants.aspectHorizontal;
            testVariations[9] = testVariation;

            //---- [10] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[8], testVariation);
            testVariation["VerticalAlignment"] = VerticalAlignment.Top;
            testVariations[10] = testVariation;

            //---- [11] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[10], testVariation);
            testVariation["WidthHeight"] = Constants.aspectHorizontal;
            testVariations[11] = testVariation;

            //---- [12] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[8], testVariation);
            testVariation["VerticalAlignment"] = VerticalAlignment.Bottom;
            testVariations[12] = testVariation;

            //---- [13] comment --------------------------
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[12], testVariation);
            testVariation["WidthHeight"] = Constants.aspectHorizontal;
            testVariations[13] = testVariation;

            //---- [14] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[8], testVariation);
            testVariation["VerticalAlignment"] = VerticalAlignment.Center;
            testVariations[14] = testVariation;

            //---- [15] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[14], testVariation);
            testVariation["WidthHeight"] = Constants.aspectHorizontal;
            testVariations[15] = testVariation;

            //---- [16] comment --------------------------
            // HorizontalAlignment.Right
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            testVariation.Add("PanelGridResourceKey", "GridResizeAlpha");
            testVariation.Add("SplitterMinWidth", Constants.splitterMinWidth);
            testVariation.Add("SplitterMinHeight", Constants.splitterMinHeight);
            testVariation.Add("Grid.Row", (Int32)1);
            testVariation.Add("Grid.Column", (Int32)1);
            testVariation.Add("SplitterColor", Brushes.Red);
            testVariation.Add("ResizeDirection", GridResizeDirection.Auto);
            testVariation.Add("ResizeBehavior", GridResizeBehavior.PreviousAndNext);
            testVariation.Add("HorizontalAlignment", HorizontalAlignment.Right);
            testVariation.Add("VerticalAlignment", VerticalAlignment.Stretch);
            testVariation.Add("WidthHeight", Constants.aspectHorizontal);
            testVariation.Add(
                "Action", new GridSplitterActionDelegate(MoveViaPeerAction));
            testVariation.Add(
                "ActionArgs", new Object[] { new Double[] { Constants.deltaX, Constants.deltaY } });
            testVariation.Add("Verify", new GridSplitterVerifyDelegate(VerifyAutoResizeDirection));
            testVariation.Add(
                "VerifyArgs", new Object[] { (Double)Constants.tolerance, GridResizeDirection.Columns });
            testVariations[16] = testVariation;

            //---- [17] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[16], testVariation);
            testVariation["WidthHeight"] = Constants.aspectHorizontal;
            testVariations[17] = testVariation;

            //---- [18] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[16], testVariation);
            testVariation["VerticalAlignment"] = VerticalAlignment.Top;
            testVariations[18] = testVariation;

            //---- [19] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[18], testVariation);
            testVariation["WidthHeight"] = Constants.aspectHorizontal;
            testVariations[19] = testVariation;

            //---- [20] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[16], testVariation);
            testVariation["VerticalAlignment"] = VerticalAlignment.Bottom;
            testVariations[20] = testVariation;

            //---- [21] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[20], testVariation);
            testVariation["WidthHeight"] = Constants.aspectHorizontal;
            testVariations[21] = testVariation;

            //---- [22] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[16], testVariation);
            testVariation["VerticalAlignment"] = VerticalAlignment.Center;
            testVariations[22] = testVariation;

            //---- [23] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[22], testVariation);
            testVariation["WidthHeight"] = Constants.aspectHorizontal;
            testVariations[23] = testVariation;

            //---- [24] comment --------------------------
            // HorizontalAlignment.Center
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            testVariation.Add("PanelGridResourceKey", "GridResizeAlpha");
            testVariation.Add("SplitterMinWidth", Constants.splitterMinWidth);
            testVariation.Add("SplitterMinHeight", Constants.splitterMinHeight);
            testVariation.Add("Grid.Row", (Int32)1);
            testVariation.Add("Grid.Column", (Int32)1);
            testVariation.Add("SplitterColor", Brushes.Red);
            testVariation.Add("ResizeDirection", GridResizeDirection.Auto);
            testVariation.Add("ResizeBehavior", GridResizeBehavior.PreviousAndNext);
            testVariation.Add("HorizontalAlignment", HorizontalAlignment.Center);
            testVariation.Add("VerticalAlignment", VerticalAlignment.Stretch);
            testVariation.Add("WidthHeight", Constants.aspectHorizontal);
            testVariation.Add(
                "Action", new GridSplitterActionDelegate(MoveViaPeerAction));
            testVariation.Add(
                "ActionArgs", new Object[] { new Double[] { Constants.deltaX, Constants.deltaY } });
            testVariation.Add("Verify", new GridSplitterVerifyDelegate(VerifyAutoResizeDirection));
            testVariation.Add(
                "VerifyArgs", new Object[] { (Double)Constants.tolerance, GridResizeDirection.Columns });
            testVariations[24] = testVariation;

            //---- [25] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[24], testVariation);
            testVariation["WidthHeight"] = Constants.aspectHorizontal;
            testVariations[25] = testVariation;

            //---- [26] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[24], testVariation);
            testVariation["VerticalAlignment"] = VerticalAlignment.Top;
            testVariations[26] = testVariation;

            //---- [27] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[26], testVariation);
            testVariation["WidthHeight"] = Constants.aspectHorizontal;
            testVariations[27] = testVariation;

            //---- [28] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[24], testVariation);
            testVariation["VerticalAlignment"] = VerticalAlignment.Bottom;
            testVariations[28] = testVariation;

            //---- [29] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[28], testVariation);
            testVariation["WidthHeight"] = Constants.aspectHorizontal;
            testVariations[29] = testVariation;

            //---- [30] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[24], testVariation);
            testVariation["VerticalAlignment"] = VerticalAlignment.Center;
            testVariations[30] = testVariation;

            //---- [31] comment --------------------------
            //--------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[30], testVariation);
            testVariation["WidthHeight"] = Constants.aspectHorizontal;
            testVariations[31] = testVariation;

            //-------------------------------------------
            return testVariations;
        }
        #endregion
    }
}
