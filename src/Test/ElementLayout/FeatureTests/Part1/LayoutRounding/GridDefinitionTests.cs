// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Collections;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Logging;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.ElementLayout.FeatureTests.Part1
{
    /// <summary>    
    /// Test Grid Column and Row sizing in Grids with LayoutRounding == true.    
    /// </summary>
    [Test(0, "Part1.LayoutRounding", "GridDefinitionTest Star Rows", MethodName = "Run", TestParameters = "content=LR_GridDefinitionTest_StarRows.xaml", Timeout = 200, Disabled = true )]
    [Test(0, "Part1.LayoutRounding", "GridDefinitionTest Star Columns", MethodName = "Run", TestParameters = "content=LR_GridDefinitionTest_StarColumns.xaml", Timeout = 200, Disabled = true )]
    [Test(1, "Part1.LayoutRounding", "GridDefinitionTest Span", MethodName = "Run", TestParameters = "content=LR_GridDefinitionTest_Span.xaml", Timeout = 200, Disabled = true )]
    [Test(1, "Part1.LayoutRounding", "GridDefinitionTest Mixed Sized Rows", MethodName = "Run", TestParameters = "content=LR_GridDefinitionTest_MixedSizedRows.xaml", Timeout = 200, Disabled = true )]
    [Test(1, "Part1.LayoutRounding", "GridDefinitionTest Mixed Sized Columns", MethodName = "Run", TestParameters = "content=LR_GridDefinitionTest_MixedSizedColumns.xaml", Timeout = 200, Disabled = true )]
    [Test(1, "Part1.LayoutRounding", "GridDefinitionTest SnapsToDevicePixels", MethodName = "Run", TestParameters = "content=LR_GridDefinitionTest_StarRows_SnapsToDevicePixels.xaml", Timeout = 200, Disabled = true )]
    public class GridDefinitionTest : AvalonTest
    {
        private Window _testWin;
        private CurrentTestData _currentTestData;
        private static readonly double s_reduceWindowWidthBy = 11; // This value was selected because as an low prime we get a lot of odd sizes to test, but not so many that the test takes too long.
        private static readonly double s_reduceWindowHeightBy = 7; // Value selection similar to above, but using a different low prime to get some different sizes to test.
        private static readonly double s_minWindowLength = 250; // This min Window size should be sufficient to get enough layout variations.
        
        private struct CurrentTestData
        {
            public CurrentTestData(double originalWindowHeight, double originalWindowWidth)
            {               
                this.OriginalWindowHeight = originalWindowHeight;
                this.OriginalWindowWidth = originalWindowWidth;
                IsColumnSingleStarSizingTest = false;
                IsRowSingleStarSizingTest = false;
            }           
            public bool IsRowSingleStarSizingTest;
            public bool IsColumnSingleStarSizingTest;            
            public double OriginalWindowHeight;
            public double OriginalWindowWidth;
        }

        public GridDefinitionTest() :
            base()
        {            
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTest);            
        }

        /// <summary>
        /// Initialize Window content.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {
            _testWin = new Window();
            if (DriverState.DriverParameters["content"] != null)
            {
                _testWin.Content = (UIElement)XamlReader.Load(File.OpenRead(DriverState.DriverParameters["content"].ToLowerInvariant()));
            }
            else
            {
                TestLog.Current.LogEvidence("Could not find content to load!");
                return TestResult.Fail;
            }

            _testWin.Width = _testWin.Height = 800;            
            _testWin.Show();
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _testWin.Close();
            return TestResult.Pass;
        }

        /// <summary>
        /// Grab the root Border and start layout manipulation.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTest()
        {
            _testWin.Width = _testWin.Height = 800;
 
            Grid mainGrid = _testWin.Content as Grid;
            if (mainGrid == null)
            {
                TestLog.Current.LogEvidence("Could not find a valid test Element to test!");
                return TestResult.Fail;
            }
                       
            _currentTestData = new CurrentTestData(_testWin.ActualHeight, _testWin.ActualWidth);           
            _currentTestData.IsColumnSingleStarSizingTest = IsColumnDefinitionSingleStarSizedOnly(mainGrid.ColumnDefinitions);
            _currentTestData.IsRowSingleStarSizingTest = IsRowDefinitionSingleStarSizedOnly(mainGrid.RowDefinitions);

            // Basically we try a whole bunch of Layout combinations, 
            // looking for a layout where the Grid definition or its content does not size right when rounded.            
            do
            {
                do
                {
                    CheckGridUnits(mainGrid);
                }
                while (ReduceWindowSize(mainGrid, s_reduceWindowWidthBy, s_reduceWindowHeightBy));
                RestoreOriginalWindowDimensions();
            }
            while (RemoveGridUnit(mainGrid));

            return TestResult.Pass;
        }

        /// <summary>
        /// Calls various verification methods, depending on Grid structure.
        /// </summary>        
        private void CheckGridUnits(Grid mainGrid)
        {           
            VerifyGridDefinitionSizes(mainGrid);

            if (_currentTestData.IsColumnSingleStarSizingTest || _currentTestData.IsRowSingleStarSizingTest)
            {
                VerifyGridSingleStarSizing(mainGrid);
            }
        }

        /// <summary>       
        /// Calls methods to verify Grid definition sizes.
        /// </summary>       
        private void VerifyGridDefinitionSizes(Grid mainGrid)
        {
            if (mainGrid.RowDefinitions.Count > 0)
            {
                VerifyRowDefinitionSize(mainGrid);
            }

            if (mainGrid.ColumnDefinitions.Count > 0)
            {
                VerifyColumnDefinitionSize(mainGrid);
            }
        }

        /// <summary>
        /// Verifies that the sum of RowDefinition's ActualHeights == Grid's ActualHeight.       
        /// </summary>       
        private void VerifyRowDefinitionSize(Grid mainGrid)
        {
            double totalRowDefinitionSize = 0;

            for (int i = 0; i < mainGrid.RowDefinitions.Count; i++)
            {
                // Verify that the RowDefinition is rounded.                                
                if (!LayoutRoundingCommon.IsLengthRounded(mainGrid.RowDefinitions[i].ActualHeight, false))
                {
                    TestLog.Current.LogEvidence(string.Format("Found a Grid RowDefinition that is not rounded: {0} ", mainGrid.RowDefinitions[i].ActualHeight));
                    LogGridInfo(mainGrid);
                    TestLog.Current.Result = TestResult.Fail;
                    return;
                }                             
                totalRowDefinitionSize += mainGrid.RowDefinitions[i].ActualHeight;
            }            
            
            totalRowDefinitionSize = LayoutRoundingCommon.ClearDoublePrecisionDiff(totalRowDefinitionSize);
            double clearedActualHeight = LayoutRoundingCommon.ClearDoublePrecisionDiff(mainGrid.ActualHeight);
            if (!totalRowDefinitionSize.Equals(clearedActualHeight))
            {                
                TestLog.Current.LogEvidence("The sum of the Grid's RowDefinitions is different than the ActualHeight of the Grid.");
                TestLog.Current.LogEvidence(string.Format("Sum of Grid RowDefinitions: {0}", totalRowDefinitionSize));
                LogGridInfo(mainGrid);
                TestLog.Current.Result = TestResult.Fail;                
            }
        }

        /// <summary>        
        /// Verifies that the sum of ColumnDefinition's ActualWidths == Grid's ActualWidth.
        /// </summary>       
        private void VerifyColumnDefinitionSize(Grid mainGrid)
        {
            double totalColumnDefinitionSize = 0;

            for (int i = 0; i < mainGrid.ColumnDefinitions.Count; i++)
            {
                // Verify that the ColumnDefinition is rounded.                               
                if (!LayoutRoundingCommon.IsLengthRounded(mainGrid.ColumnDefinitions[i].ActualWidth, true))
                {
                    TestLog.Current.LogEvidence(string.Format("Found a Grid ColumnDefinition that is not rounded: {0} ", mainGrid.ColumnDefinitions[i].ActualWidth));
                    LogGridInfo(mainGrid);
                    TestLog.Current.Result = TestResult.Fail;
                    return;
                }
                totalColumnDefinitionSize += mainGrid.ColumnDefinitions[i].ActualWidth;
            }

            totalColumnDefinitionSize = LayoutRoundingCommon.ClearDoublePrecisionDiff(totalColumnDefinitionSize);
            double clearedActualWidth = LayoutRoundingCommon.ClearDoublePrecisionDiff(mainGrid.ActualWidth);
            if (!totalColumnDefinitionSize.Equals(clearedActualWidth))
            //if (totalColumnDefinitionSize != mainGrid.ActualWidth)
            {
                TestLog.Current.LogEvidence("The sum of the Grid's ColumnDefinitions is different than the ActualWidth of the Grid.");
                TestLog.Current.LogEvidence(string.Format("Sum of Grid ColumnDefinitions: {0}", totalColumnDefinitionSize));
                LogGridInfo(mainGrid);
                TestLog.Current.Result = TestResult.Fail;
            }
        }

        /// <summary>    
        /// Calls VerifyGridStarSizedChildren for Rows/Columns.
        /// </summary>  
        private void VerifyGridSingleStarSizing(Grid mainGrid)
        {
            if (_currentTestData.IsRowSingleStarSizingTest)
            {
                VerifyGridStarSizedChildren(true, mainGrid);
            }

            if (_currentTestData.IsColumnSingleStarSizingTest)
            {
                VerifyGridStarSizedChildren(false, mainGrid);
            }
        }

        /// <summary>    
        /// Verification for single star sized Grid children.
        /// Verifies that the sum of the Rows/Columns Heights/Widths == the Grids ActualHeight/ActualWidth
        /// and that there are not too many different Grid lengths in a single Row or Column group.
        /// Example - Grid rows that are 24 or 25 px high is valid, but rows of 24, 25, or 26 px high are not.
        /// This verification is only appropriate for certain defined layouts:
        ///  - All RowDefinitions/ColumnDefinitions must be single star sized
        ///  - No Grid Chidren can span > 1 definition
        /// </summary>       
        private void VerifyGridStarSizedChildren(bool rowTest, Grid mainGrid)
        {            
            int childrenCount = 0;
            double gridUnitTotalLength = 0;
            ArrayList lengthCollection = new ArrayList();           
            
            if (rowTest)
            {
                childrenCount = mainGrid.RowDefinitions.Count;
            }
            else
            {
                childrenCount = mainGrid.ColumnDefinitions.Count;
            }

            for (int i = 0; i < childrenCount; i++)
            {                             
                // This verification is not appropriate for Row or Column spanned layouts, so return in those cases.
                int spanValue = 0;
                int.TryParse(mainGrid.Children[i].GetValue(Grid.RowSpanProperty).ToString(), out spanValue);
                if (spanValue != 1)
                {                    
                    return;
                }

                int.TryParse(mainGrid.Children[i].GetValue(Grid.ColumnSpanProperty).ToString(), out spanValue);
                if (spanValue != 1)
                {                    
                    return;
                }
                               
                double childLength;
                bool isWidth = true;
                if (rowTest)
                {
                    childLength = ((FrameworkElement)mainGrid.Children[i]).ActualHeight;
                    isWidth = false;
                }
                else
                {
                    childLength = ((FrameworkElement)mainGrid.Children[i]).ActualWidth;
                }

                // Verify that the child length is rounded.                               
                if (!LayoutRoundingCommon.IsLengthRounded(childLength, isWidth))
                {
                    TestLog.Current.LogEvidence(string.Format("Found a Grid length that is not rounded: {0} ", childLength));
                    LogGridInfo(mainGrid);
                    TestLog.Current.Result = TestResult.Fail;
                    return;
                }
                gridUnitTotalLength += childLength;
                
                if (!lengthCollection.Contains(childLength))
                {                    
                    lengthCollection.Add(childLength);                    
                }               
            }
            
            if (lengthCollection.Count == 0)
            {
                TestLog.Current.LogEvidence("There was some problem with the test.  Did not find any Column or Row element lengths");
                LogGridInfo(mainGrid);
                TestLog.Current.Result = TestResult.Fail;
            }                       

            // Verify that there are not too many different grid lengths.
            if (lengthCollection.Count > 2)
            {
                TestLog.Current.LogEvidence("Found too many Grid member lengths!");
                LogGridInfo(mainGrid);
                foreach (object o in lengthCollection)
                {
                    TestLog.Current.LogEvidence(string.Format("Found a Grid length of {0}", o.ToString()));
                }
                TestLog.Current.Result = TestResult.Fail;
            }
            else if (lengthCollection.Count == 2)
            {
                // Verify that the two values are a pixel apart.
                double length1 = 0;
                double length2 = 0;
                double.TryParse(lengthCollection[0].ToString(), out length1);
                double.TryParse(lengthCollection[1].ToString(), out length2);

                double acceptableDefinitionDelta = 0;
                if (rowTest)
                {
                    acceptableDefinitionDelta = 96.0 / Microsoft.Test.Display.Monitor.Dpi.y;
                }
                else
                {
                    acceptableDefinitionDelta = 96.0 / Microsoft.Test.Display.Monitor.Dpi.y;
                }

                length1 = LayoutRoundingCommon.ClearDoublePrecisionDiff(length1);                

                double highAcceptableLength = length2 + acceptableDefinitionDelta;
                double lowAcceptableLength = length2 - acceptableDefinitionDelta;
                highAcceptableLength = LayoutRoundingCommon.ClearDoublePrecisionDiff(highAcceptableLength);
                lowAcceptableLength = LayoutRoundingCommon.ClearDoublePrecisionDiff(lowAcceptableLength);
                
                if ((length1 != highAcceptableLength) && (length1 != lowAcceptableLength))
                {
                    TestLog.Current.LogEvidence("length1 = {0} diffHigh = {1} diffLow = {2}", length1, highAcceptableLength, lowAcceptableLength);
                    LogGridInfo(mainGrid);
                    TestLog.Current.LogEvidence(string.Format("Found Grid lengths that are incorrect! Grid length 1: {0} Grid length 2: {1} acceptableDefinitionDelta = {2}", lengthCollection[0], lengthCollection[1], acceptableDefinitionDelta));
                    TestLog.Current.Result = TestResult.Fail;
                }
            } 
                        
            // Verify that the sum of the Grid Rows/Columns are the same as the actual width/height of the Grid
            double mainGridLength = 0;
            if (rowTest)
            {
                mainGridLength = mainGrid.ActualHeight;
            }
            else
            {
                mainGridLength = mainGrid.ActualWidth;
            }

            mainGridLength = LayoutRoundingCommon.ClearDoublePrecisionDiff(mainGridLength);
            gridUnitTotalLength = LayoutRoundingCommon.ClearDoublePrecisionDiff(gridUnitTotalLength);
            if (mainGridLength != gridUnitTotalLength)
            {
                TestLog.Current.LogEvidence("The sum of the Grid's Rows or Columns is different than the Actual Height or Width of the Grid.");
                TestLog.Current.LogEvidence(string.Format("Sum of Grid units: {0}", gridUnitTotalLength));
                LogGridInfo(mainGrid);
                TestLog.Current.Result = TestResult.Fail;
            }
        }
       
        /// <summary>
        /// Dumps Grid layout info for failure analysis.
        /// </summary>       
        private void LogGridInfo(Grid mainGrid)
        {
            TestLog.Current.LogEvidence(string.Format("Grid.ActualWidth = {0} Grid.ActualHeight = {1}", mainGrid.ActualWidth, mainGrid.ActualHeight));
            TestLog.Current.LogEvidence(string.Format("Grid has {0} Rows, {1} Columns", mainGrid.RowDefinitions.Count, mainGrid.ColumnDefinitions.Count));
        }
        
        /// <summary>
        /// Attempts to remove a Column or Row from a Grid.
        /// </summary>
        /// <returns>bool</returns>
        private bool RemoveGridUnit(Grid mainGrid)
        {
            if (mainGrid.RowDefinitions.Count > 1)
            {
                mainGrid.RowDefinitions.RemoveAt(0);
                WaitForPriority(DispatcherPriority.ApplicationIdle);
                return true;
            }

            if (mainGrid.ColumnDefinitions.Count > 1)
            {
                mainGrid.ColumnDefinitions.RemoveAt(0);
                WaitForPriority(DispatcherPriority.ApplicationIdle);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Reduces the Width or Height of a Window depending on Grid structure and if the Window is larger than a predetermined minimum size.
        /// </summary>
        /// <returns>bool</returns>
        private bool ReduceWindowSize(Grid mainGrid, double reduceWidthBy, double reduceHeightBy)
        {            
            if (mainGrid.RowDefinitions.Count > 1)
            {
                if (_testWin.ActualHeight > s_minWindowLength)
                {
                    _testWin.Height = _testWin.ActualHeight - reduceHeightBy;
                    WaitForPriority(DispatcherPriority.ApplicationIdle);
                    return true;
                }
            }

            if (mainGrid.ColumnDefinitions.Count > 1)
            {
                if (_testWin.Width > s_minWindowLength)
                {
                    _testWin.Width = _testWin.ActualWidth - reduceWidthBy;
                    WaitForPriority(DispatcherPriority.ApplicationIdle);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Restores the Window to the size it had at the beginning of the test.
        /// </summary>       
        private void RestoreOriginalWindowDimensions()
        {
            _testWin.Width = _currentTestData.OriginalWindowWidth;
            _testWin.Height = _currentTestData.OriginalWindowHeight;
            WaitForPriority(DispatcherPriority.ApplicationIdle);
        }

        /// <summary>
        /// Determines if a RowDefinitionCollection is single star sized
        /// </summary>       
        private bool IsRowDefinitionSingleStarSizedOnly(RowDefinitionCollection rowDefinitionCollection)
        {
            if (rowDefinitionCollection.Count == 0)
            {
                return false;
            }
            
            foreach (RowDefinition rowDefinition in rowDefinitionCollection)
            {              
                if(rowDefinition.Height != new GridLength(1, GridUnitType.Star))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Determines if a ColumnDefinitionCollection is single star sized
        /// </summary>       
        private bool IsColumnDefinitionSingleStarSizedOnly(ColumnDefinitionCollection columnDefinitionCollection)
        {
            if (columnDefinitionCollection.Count == 0)
            {
                return false;
            }

            foreach (ColumnDefinition columnDefinition in columnDefinitionCollection)
            {
                if (columnDefinition.Width != new GridLength(1, GridUnitType.Star))
                {
                    return false;
                }
            }
            return true;
        }
    }   
}
