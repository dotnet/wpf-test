// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description : Test TableOM.  
//               
//               This loads a xaml with a Table with either a FlowDocumentScrollViewer or FlowDocument as the root.  
//               Tests are then run to verify the TableOM APIs for TableColumnCollection, TableRowGroupCollection, 
//               TableRowCollection, and TableCellCollection.
//                                   
// Verification: Basic API validation.  
// Created by:  Microsoft
////////////////////////////////////////////////////////////////////////////////////////////////////////////// 

using System;
using System.IO;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Markup;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>
    /// <area>Table</area>
    /// <owner>Microsoft</owner>
    /// <priority>0</priority>
    /// <description>
    /// Testing Table OM.
    /// </description>
    /// </summary>
    [Test(0, "Table", "TestTableOM", MethodName="Run")]
    public class TestTableOM : AvalonTest
    {       
        private Window _w;
        private string _loadFile;
        private int _testID;
        private Table _testTable;
        private int[] _originalCollectionCount;
       
        [Variation(@"FeatureTests\FlowLayout\TableOMContent_FD.xaml", 1)]
        [Variation(@"FeatureTests\FlowLayout\TableOMContent_FD.xaml", 2, Priority = 2)]
        [Variation(@"FeatureTests\FlowLayout\TableOMContent_FD.xaml", 3)]
        [Variation(@"FeatureTests\FlowLayout\TableOMContent_FD.xaml", 4)]
        [Variation(@"FeatureTests\FlowLayout\TableOMContent_FD.xaml", 5, Priority = 2)]
        [Variation(@"FeatureTests\FlowLayout\TableOMContent_FD.xaml", 6)]
        [Variation(@"FeatureTests\FlowLayout\TableOMContent_FD.xaml", 7, Priority = 2)]
        [Variation(@"FeatureTests\FlowLayout\TableOMContent_FD.xaml", 8, Priority = 2)]
        [Variation(@"FeatureTests\FlowLayout\TableOMContent_FD.xaml", 9, Priority = 2)]
        [Variation(@"FeatureTests\FlowLayout\TableOMContent_TF.xaml", 1, Priority = 2)]
        [Variation(@"FeatureTests\FlowLayout\TableOMContent_TF.xaml", 2)]
        [Variation(@"FeatureTests\FlowLayout\TableOMContent_TF.xaml", 3, Priority = 2)]
        [Variation(@"FeatureTests\FlowLayout\TableOMContent_TF.xaml", 4, Priority = 2)]
        [Variation(@"FeatureTests\FlowLayout\TableOMContent_TF.xaml", 5, Priority = 2)]
        [Variation(@"FeatureTests\FlowLayout\TableOMContent_TF.xaml", 6, Priority = 2)]
        [Variation(@"FeatureTests\FlowLayout\TableOMContent_TF.xaml", 7, Priority = 2)]
        [Variation(@"FeatureTests\FlowLayout\TableOMContent_TF.xaml", 8, Priority = 2)]
        [Variation(@"FeatureTests\FlowLayout\TableOMContent_TF.xaml", 9, Priority = 2)]       
        public TestTableOM(string testValue, int inputId)
            : base()
        {
            CreateLog = false;
            _loadFile = testValue;
            int indx = _loadFile.LastIndexOf("\\");
            if (indx >= 0)
            {
                _loadFile = _loadFile.Remove(0, indx + 1);     //Remove the Animation prefix.
            }

            _testID = inputId;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTableOMTests);
        }        
        
        /// <summary>
        /// Initialize: Setup the tests
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {                       
            _w = new Window();
            System.IO.Stream xamlFileStream = File.OpenRead(_loadFile);
            UIElement root = null;
            root = (UIElement)XamlReader.Load(xamlFileStream);

            _w.Content = root;
            _w.Width = 800;
            _w.Height = 600;
            _w.Top = 0;
            _w.Left = 0;
            _w.Show();
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _w.Close();
            return TestResult.Pass;
        }

        private TestResult RunTableOMTests()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            _testTable = (Table)LogicalTreeHelper.FindLogicalNode((DependencyObject)_w.Content, "testTable");
            _originalCollectionCount = new int[4];
            _originalCollectionCount = GetCollectionCount(_testTable);

            switch (_testID)
            {
                case 1:
                    InsertAndRemoveTest();
                    break;
                case 2:
                    RemoveAtTest();
                    break;
                case 3:
                    RemoveRangeTest();
                    break;
                case 4:
                    ClearTest();
                    break;
                case 5:
                    TrimToSizeAndCapcityTest();
                    break;
                case 6:
                    AddAndContainsTest();
                    break;
                case 7:
                    IndexOfTest();
                    break;
                case 8:
                    CopyToTest();
                    break;
                case 9:
                    GetEnumeratorTest();
                    break;
                default:
                    throw new Exception("Failed to execute switch");                    
            }
            return TestResult.Pass;
        }
       
        private void InsertAndRemoveTest()
        {
            int[] expectedCollectionCount = new int[4];
            _originalCollectionCount.CopyTo(expectedCollectionCount, 0);

            TestLog log = new TestLog("InsertTest on TableColumnCollection");                     
            log.LogStatus("Running InsertTest on TableColumnCollection...");
            TableColumn newColumn = new TableColumn();
            newColumn.Name = "newColumn";
            newColumn.Background = Brushes.LightGreen;
            _testTable.Columns.Insert(1, newColumn);
            //Verify
            TableColumn tc = _testTable.Columns[1];
            log.Result = VerifyName("newColumn", tc.Name);
            log.Close();

            log = new TestLog("InsertTest on TableRowGroupCollection");   
            log.LogStatus("Running InsertTest on TableRowGroupCollection...");
            TableRow tr = new TableRow();
            TableCell newtrg = new TableCell(new Paragraph(new Run("New TableRowGroup")));
            tr.Cells.Add(newtrg);
            TableRowGroup newRowGroup = new TableRowGroup();
            newRowGroup.Rows.Add(tr);
            newRowGroup.Name = "newTableRowGroup";
            _testTable.RowGroups.Insert(1, newRowGroup);
            //Verify
            TableRowGroup trg = _testTable.RowGroups[1];
            log.Result = VerifyName("newTableRowGroup", trg.Name);
            log.Close();

            log = new TestLog("InsertTest on TableRowCollection");   
            log.LogStatus("Running InsertTest on TableRowCollection...");
            TableRow newRow = new TableRow();
            TableCell newtr = new TableCell(new Paragraph(new Run("New TableRow")));
            newRow.Cells.Add(newtr);
            newRow.Name = "newRow";
            _testTable.RowGroups[2].Rows.Insert(1, newRow);
            //Verify
            TableRow vtr = _testTable.RowGroups[2].Rows[1];
            log.Result = VerifyName("newRow", vtr.Name);
            log.Close();

            log = new TestLog("InsertTest on TableCellCollection");
            log.LogStatus("Running InsertTest on TableCellCollection...");
            TableCell newCell = new TableCell(new Paragraph(new Run("New TableCell")));
            newCell.Name = "newCell";
            newCell.Background = Brushes.LightYellow;
            _testTable.RowGroups[2].Rows[0].Cells.Insert(1, newCell);
            //Verify
            TableCell tcell = _testTable.RowGroups[2].Rows[0].Cells[1];
            log.Result = VerifyName("newCell", tcell.Name);
            log.Close();

            //Verify Collection counts
            log = new TestLog("Verify Collection");
            int[] currentCollectionCount = GetCollectionCount(_testTable);
            //should be one more Column and RowGroup, 2 more Rows, and 3 more Cells
            expectedCollectionCount[0] = _originalCollectionCount[0] + 1;
            expectedCollectionCount[1] = _originalCollectionCount[1] + 1;
            expectedCollectionCount[2] = _originalCollectionCount[2] + 2;
            expectedCollectionCount[3] = _originalCollectionCount[3] + 3;

            log.Result = VerifyCollectionCount(expectedCollectionCount, currentCollectionCount);            
            log.Close();
                                    
            log = new TestLog("RemoveTest on TableColumnCollection");            
            log.LogStatus("Running RemoveTest on TableColumnCollection...");
            _testTable.Columns.Remove(newColumn);
            if (DoesElementExistInTable(newColumn, _testTable))
            {
                log.LogEvidence("TableColumn exists in the Table after it has been removed.");
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();

            log = new TestLog("RemoveTest on TableCellCollection");
            log.LogStatus("Running RemoveTest on TableCellCollection...");
            _testTable.RowGroups[2].Rows[0].Cells.Remove(newCell);            
            if (DoesElementExistInTable(newCell, _testTable))
            {                
                log.LogEvidence("TableCell exists in the Table after it has been removed.");
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();
            
            log = new TestLog("RemoveTest on TableRowCollection");
            log.LogStatus("Running RemoveTest on TableRowCollection...");
            _testTable.RowGroups[2].Rows.Remove(newRow);
            if (DoesElementExistInTable(newRow, _testTable))
            {
                log.LogEvidence("TableRow exists in the Table after it has been removed.");
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();
           
            log = new TestLog("RemoveTest on TableRowGroupCollection");
            log.LogStatus("Running RemoveTest on TableRowGroupCollection...");
            _testTable.RowGroups.Remove(newRowGroup);
            if (DoesElementExistInTable(newRowGroup, _testTable))
            {
                log.LogEvidence("TableRowGroup exists in the Table after it has been removed.");
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();

            log = new TestLog("Collection verification");
            currentCollectionCount = GetCollectionCount(_testTable);
            log.Result = VerifyCollectionCount(_originalCollectionCount, currentCollectionCount);
            log.Close();
        }

        private void RemoveAtTest()
        {            
            TestLog log = new TestLog("RemoveAtTest on TableColumnCollection");
            log.LogStatus("Running RemoveAtTest on TableColumnCollection...");
            TableColumn tcoll = _testTable.FindName("testColumn2") as TableColumn;
            if (tcoll == null)
            {
                log.LogEvidence("Could not get the Table element int the Table.");
                log.Result = TestResult.Fail;
            }

            _testTable.Columns.RemoveAt(1);           
            if (DoesElementExistInTable(tcoll, _testTable))
            {
                log.LogEvidence("TableColumn exists in the Table after it has been removed.");
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();
           
            log = new TestLog("RemoveAtTest on TableCellCollection");
            log.LogStatus("Running RemoveAtTest on TableCellCollection...");
            TableCell tc = _testTable.FindName("cell_trg0_tr2_tc0") as TableCell;
            if (tc == null)
            {
                log.LogEvidence("Could not get the Table element int the Table.");
                log.Result = TestResult.Fail;
            }
            _testTable.RowGroups[0].Rows[2].Cells.RemoveAt(0);
            if (DoesElementExistInTable(tc, _testTable))
            {
                log.LogEvidence("TableCell exists in the Table after it has been removed.");
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();
            
            log = new TestLog("RemoveAtTest on TableRowCollection");
            log.LogStatus("Running RemoveAtTest on TableRowCollection...");
            TableRow tr = _testTable.FindName("row_trg1_tr2") as TableRow;
            if (tr == null)
            {
                log.LogEvidence("Could not get the Table element int the Table.");
                log.Result = TestResult.Fail;
            }
            _testTable.RowGroups[1].Rows.RemoveAt(2);
            if (DoesElementExistInTable(tr, _testTable))
            {
                log.LogEvidence("TableRow exists in the Table after it has been removed.");
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();
            
            log = new TestLog("RemoveAtTest on TableRowGroupCollection");
            log.LogStatus("Running RemoveAtTest on TableRowGroupCollection...");
            TableRowGroup trg = _testTable.FindName("group_trg2") as TableRowGroup;
            if (trg == null)
            {
                log.LogEvidence("Could not get the Table element int the Table.");
                log.Result = TestResult.Fail;
            }
            _testTable.RowGroups.RemoveAt(2);
            if (DoesElementExistInTable(trg, _testTable))
            {
                log.LogEvidence("TableRowGroup exists in the Table after it has been removed.");
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();

            log = new TestLog("Verifying Collection");
            int[] currentCollectionCount = GetCollectionCount(_testTable);
            _originalCollectionCount[0] = _originalCollectionCount[0] - 1;
            _originalCollectionCount[1] = _originalCollectionCount[1] - 1;
            _originalCollectionCount[2] = _originalCollectionCount[2] - 4;
            _originalCollectionCount[3] = _originalCollectionCount[3] - 13;
            log.Result = VerifyCollectionCount(_originalCollectionCount, currentCollectionCount);
            log.Close();
        }

        private void RemoveRangeTest()
        {
            TestLog log = new TestLog("RemoveRangeTest on Table");
            log.LogStatus("Running RemoveRangeTest on TableColumnCollection...");
            _testTable.Columns.RemoveRange(0, 3);

            log.LogStatus("Running RemoveRangeTest on TableCellCollection...");
            _testTable.RowGroups[0].Rows[2].Cells.RemoveRange(0, 2);

            log.LogStatus("Running RemoveRangeTest on TableRowCollection...");
            _testTable.RowGroups[1].Rows.RemoveRange(2, 1);

            log.LogStatus("Running RemoveRangeTest on TableRowGroupCollection...");
            _testTable.RowGroups.RemoveRange(1, 1);

            int[] currentCollectionCount = GetCollectionCount(_testTable);
            _originalCollectionCount[0] = _originalCollectionCount[0] - 3;
            _originalCollectionCount[1] = _originalCollectionCount[1] - 1;
            _originalCollectionCount[2] = _originalCollectionCount[2] - 3;
            _originalCollectionCount[3] = _originalCollectionCount[3] - 11;
            log.Result = VerifyCollectionCount(_originalCollectionCount, currentCollectionCount);
            log.Close();
        }

        private void ClearTest()
        {
            TestLog log = new TestLog("ClearTest on TableColumnCollection");
            log.LogStatus("Running ClearTest on TableColumnCollection...");
            _testTable.Columns.Clear();
            int[] currentCollectionCount = GetCollectionCount(_testTable);
            _originalCollectionCount[0] = 0;  //expect 0 columns
            log.Result = VerifyCollectionCount(_originalCollectionCount, currentCollectionCount);
            log.Close();

            log = new TestLog("ClearTest on TableCellCollection");
            log.LogStatus("Running ClearTest on TableCellCollection...");
            _testTable.RowGroups[2].Rows[1].Cells.Clear();
            currentCollectionCount = GetCollectionCount(_testTable);
            _originalCollectionCount[3] = _originalCollectionCount[3] - 3;  //expect 3 less TableCells
            log.Result = VerifyCollectionCount(_originalCollectionCount, currentCollectionCount);
            log.Close();

            log = new TestLog("ClearTest on TableRowCollection");
            log.LogStatus("Running ClearTest on TableRowCollection...");
            _testTable.RowGroups[1].Rows.Clear();
            currentCollectionCount = GetCollectionCount(_testTable);
            _originalCollectionCount[2] = _originalCollectionCount[2] - 3;  //expect 3 less TableRows
            _originalCollectionCount[3] = _originalCollectionCount[3] - 9;  //expect 9 less TableCells as a result
            log.Result = VerifyCollectionCount(_originalCollectionCount, currentCollectionCount);
            log.Close();

            log = new TestLog("ClearTest on TableRowGroupCollection");
            log.LogStatus("Running ClearTest on TableRowGroupCollection...");
            _testTable.RowGroups.Clear();
            currentCollectionCount = GetCollectionCount(_testTable);
            _originalCollectionCount[1] = 0;  //expect 0 TableRowGroups, TableRows, and TableCells
            _originalCollectionCount[2] = 0;
            _originalCollectionCount[3] = 0;
            log.Result = VerifyCollectionCount(_originalCollectionCount, currentCollectionCount);
            log.Close();
        }

        private void TrimToSizeAndCapcityTest()
        {            
            TestLog log = new TestLog("TrimToSizeTest on TableColumnCollection");
            if (_testTable.Columns.Capacity != 8)
            {
                log.LogEvidence("Initial Capacity value was incorrect.  Expected 8, actual was " + _testTable.Columns.Capacity);
                log.Result = TestResult.Fail;
            }

            _testTable.Columns.TrimToSize();
            if (_testTable.Columns.Capacity != _testTable.Columns.Count)
            {
                log.LogEvidence("Capacity value after TrimToSize was incorrect.  Expected " + _testTable.Columns.Count + ", actual was " + _testTable.Columns.Capacity);
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();

            log = new TestLog("Set Capacity to < the TableColumnCollection count");
            try
            {
                _testTable.Columns.Capacity = _testTable.Columns.Count - 1;
                log.LogEvidence("Did not get an exception when setting Capacity to a value less then the collection count.");
                log.Result = TestResult.Fail;
            }
            catch (Exception e)
            {
                if (e.GetType() != typeof(System.ArgumentOutOfRangeException))
                {
                    log.LogEvidence("Did not the correct exception type when setting Capacity to a value less then the collection count.");
                    log.LogEvidence("Expected System.ArgumentOutOfRangeException got " + e.GetType().ToString());
                    log.Result = TestResult.Fail;                    
                }
                else
                {
                    log.Result = TestResult.Pass;
                }
            }
            log.Close();
            
            log = new TestLog("TrimToSizeTest on TableRowGroupCollection");
            if (_testTable.RowGroups.Capacity != 8)
            {
                log.LogEvidence("Initial Capacity value was incorrect.  Expected 8, actual was " + _testTable.RowGroups.Capacity);
                log.Result = TestResult.Fail;
            }

            _testTable.RowGroups.TrimToSize();
            if (_testTable.Columns.Capacity != _testTable.Columns.Count)
            {
                log.LogEvidence("Capacity value after TrimToSize was incorrect.  Expected " + _testTable.RowGroups.Count + ", actual was " + _testTable.RowGroups.Capacity);
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();

            log = new TestLog("Set Capacity to < the TableRowGroupCollection count");
            try
            {
                _testTable.RowGroups.Capacity = _testTable.RowGroups.Count - 1;
                log.LogEvidence("Did not get an exception when setting Capacity to a value less then the collection count.");
                log.Result = TestResult.Fail;
            }
            catch (Exception e)
            {
                if (e.GetType() != typeof(System.ArgumentOutOfRangeException))
                {
                    log.LogEvidence("Did not the correct exception type when setting Capacity to a value less then the collection count.");
                    log.LogEvidence("Expected System.ArgumentOutOfRangeException got " + e.GetType().ToString());
                    log.Result = TestResult.Fail;                    
                }
                else
                {
                    log.Result = TestResult.Pass;
                }                
            }
            log.Close();
            
            log = new TestLog("TrimToSizeTest on TableRowCollection");
            log.LogStatus("Running TrimToSizeTest on TableRowCollection...");
            if (_testTable.RowGroups[1].Rows.Capacity != 8)
            {
                log.LogEvidence("Initial Capacity value was incorrect.  Expected 8, actual was " + _testTable.RowGroups[1].Rows.Capacity);
                log.Result = TestResult.Fail;
            }

            _testTable.RowGroups[1].Rows.TrimToSize();
            if (_testTable.RowGroups[1].Rows.Capacity != _testTable.RowGroups[1].Rows.Count)
            {
                log.LogEvidence("Capacity value after TrimToSize was incorrect.  Expected " + _testTable.RowGroups[1].Rows.Count + ", actual was " + _testTable.RowGroups[1].Rows.Capacity);
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }                
            log.Close();

            log = new TestLog("Set Capacity to < the TableRowCollection count");
            try
            {
                _testTable.RowGroups[1].Rows.Capacity = _testTable.RowGroups[1].Rows.Count - 1;
                log.LogEvidence("Did not get an exception when setting Capacity to a value less then the collection count.");
                log.Result = TestResult.Fail;
            }
            catch (Exception e)
            {
                if (e.GetType() != typeof(System.ArgumentOutOfRangeException))
                {
                    log.LogEvidence("Did not the correct exception type when setting Capacity to a value less then the collection count.");
                    log.LogEvidence("Expected System.ArgumentOutOfRangeException got " + e.GetType().ToString());
                    log.Result = TestResult.Fail;                    
                }
                else
                {
                    log.Result = TestResult.Pass;
                }                
            }
            log.Close();
            
            log = new TestLog("TrimToSizeTest on TableCellCollection");
            log.LogStatus("Running TrimToSizeTest on TableCellCollection...");
            if (_testTable.RowGroups[0].Rows[2].Cells.Capacity != 8)
            {
                log.LogEvidence("Initial Capacity value was incorrect.  Expected 8, actual was " + _testTable.RowGroups[0].Rows[2].Cells.Capacity);
                log.Result = TestResult.Fail;
            }

            _testTable.RowGroups[0].Rows[2].Cells.TrimToSize();
            if (_testTable.RowGroups[0].Rows[2].Cells.Capacity != _testTable.RowGroups[0].Rows[2].Cells.Count)
            {
                log.LogEvidence("Capacity value after TrimToSize was incorrect.  Expected " + _testTable.RowGroups[0].Rows[2].Cells.Count + ", actual was " + _testTable.RowGroups[0].Rows[2].Cells.Capacity);
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();

            log = new TestLog("Set Capacity to < the TableCellCollection count");
            try
            {
                _testTable.RowGroups[0].Rows[2].Cells.Capacity = _testTable.RowGroups[0].Rows[2].Cells.Count - 1;
                log.LogEvidence("Did not get an exception when setting Capacity to a value less then the collection count.");
                log.Result = TestResult.Fail;
            }
            catch (Exception e)
            {
                if (e.GetType() != typeof(System.ArgumentOutOfRangeException))
                {
                    log.LogEvidence("Did not the correct exception type when setting Capacity to a value less then the collection count.");
                    log.LogEvidence("Expected System.ArgumentOutOfRangeException got " + e.GetType().ToString());
                    log.Result = TestResult.Fail;                    
                }
                else
                {
                    log.Result = TestResult.Pass;
                }
            }
            log.Close();
        }

        private void AddAndContainsTest()
        {
            GlobalLog.LogStatus("Creating and adding Table elements to set this test up...");

            TableColumn newColumn1 = new TableColumn();
            TableColumn newColumn2 = new TableColumn();

            TableCell newCell1 = new TableCell(new Paragraph(new Run("newCell1")));
            TableRow newRow1 = new TableRow();
            newRow1.Cells.Add(newCell1);
            TableRowGroup newRowGroup1 = new TableRowGroup();
            newRowGroup1.Rows.Add(newRow1);

            TableCell newCell2 = new TableCell(new Paragraph(new Run("newCell2")));
            TableRow newRow2 = new TableRow();
            newRow2.Cells.Add(newCell2);
            TableRowGroup newRowGroup2 = new TableRowGroup();
            newRowGroup2.Rows.Add(newRow2);

            _testTable.Columns.Add(newColumn1);
            _testTable.RowGroups.Add(newRowGroup1);
            
            TestLog log = new TestLog("AddTest on TableColumnCollection");
            log.LogStatus("Running AddAndContainsTest on TableColumnCollection...");
            if (!_testTable.Columns.Contains(newColumn1))
            {
                log.LogEvidence("Contains() returns false when looking for a TableElement that should exist in this collection");
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();

            log = new TestLog("ContainsTest on TableColumnCollection");
            if (_testTable.Columns.Contains(newColumn2))
            {
                log.LogEvidence("Contains() returns true when looking for a TableElement that should NOT exist in this collection");
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();
            
            log = new TestLog("AddTest on TableRowGroupCollection");
            log.LogStatus("Running AddAndContainsTest on TableRowGroupCollection...");
            if (!_testTable.RowGroups.Contains(newRowGroup1))
            {
                log.LogEvidence("Contains() returns false when looking for a TableElement that should exist in this collection");
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();

            log = new TestLog("ContainsTest on TableRowGroupCollection");
            if (_testTable.RowGroups.Contains(newRowGroup2))
            {
                log.LogEvidence("Contains() returns true when looking for a TableElement that should NOT exist in this collection");
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();
            
            log = new TestLog("AddTest on TableRowCollection");
            log.LogStatus("Running AddAndContainsTest on TableRowCollection...");
            if (!_testTable.RowGroups[_testTable.RowGroups.Count - 1].Rows.Contains(newRow1))
            {
                log.LogEvidence("Contains() returns false when looking for a TableElement that should exist in this collection");
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();

            log = new TestLog("ContainsTest on TableRowCollection");
            if (_testTable.RowGroups[_testTable.RowGroups.Count - 1].Rows.Contains(newRow2))
            {
                log.LogEvidence("Contains() returns true when looking for a TableElement that should NOT exist in this collection");
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();
            
            log = new TestLog("AddTest on TableCellCollection");
            log.LogStatus("Running AddAndContainsTest on TableCellCollection...");
            if (!_testTable.RowGroups[_testTable.RowGroups.Count - 1].Rows[0].Cells.Contains(newCell1))
            {
                log.LogEvidence("Contains() returns false when looking for a TableElement that should exist in this collection");
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();

            log = new TestLog("ContainsTest on TableCellCollection");
            if (_testTable.RowGroups[_testTable.RowGroups.Count - 1].Rows[0].Cells.Contains(newCell2))
            {
                log.LogEvidence("Contains() returns true when looking for a TableElement that should NOT exist in this collection");
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();
        }

        private void IndexOfTest()
        {            
            TestLog log = new TestLog("Get Table Elements for test");
            log.LogStatus("Getting Table elements to set this test up...");
            log.Result = TestResult.Pass;
            TableColumn col = _testTable.FindName("testColumn2") as TableColumn;
            if (col == null)
            {
                log.LogEvidence("Could not find the TableColumn in the markup");
                log.Result = TestResult.Fail;
            }

            TableRowGroup group = _testTable.FindName("testTableRowGroup1") as TableRowGroup;
            if (group == null)
            {
                log.LogEvidence("Could not find the TableRowGroup in the markup");
                log.Result = TestResult.Fail;
            }

            TableRow row = _testTable.FindName("testRow1") as TableRow;
            if (row == null)
            {
                log.LogEvidence("Could not find the TableRow in the markup");
                log.Result = TestResult.Fail;
            }

            TableCell cell = _testTable.FindName("testCell1") as TableCell;
            if (cell == null)
            {
                log.LogEvidence("Could not find the TableCell in the markup");
                log.Result = TestResult.Fail;
            }
            log.Close();
            
            log = new TestLog("IndexOfTest on TableColumnCollection");
            log.LogStatus("Running IndexOfTest on TableColumnCollection...");
            if (_testTable.Columns.IndexOf(col) != 1)
            {
                log.LogEvidence("Element was not as the expected index.  Expected 1 got " + _testTable.Columns.IndexOf(col));
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();
            
            log = new TestLog("IndexOfTest on TableRowGroupCollection");
            log.LogStatus("Running IndexOfTest on TableRowGroupCollection...");
            if (_testTable.RowGroups.IndexOf(group) != 0)
            {
                log.LogEvidence("Element was not as the expected index.  Expected 0 got " + _testTable.RowGroups.IndexOf(group));
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();
            
            log = new TestLog("IndexOfTest on TableRowCollection");
            log.LogStatus("Running IndexOfTest on TableRowCollection...");
            if (_testTable.RowGroups[2].Rows.IndexOf(row) != 1)
            {
                log.LogEvidence("Element was not as the expected index.  Expected 1 got " + _testTable.RowGroups[2].Rows.IndexOf(row));
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();
            
            log = new TestLog("IndexOfTest on TableCellCollection");
            log.LogStatus("Running IndexOfTest on TableCellCollection...");
            if (_testTable.RowGroups[0].Rows[0].Cells.IndexOf(cell) != 2)
            {
                log.LogEvidence("Element was not as the expected index.  Expected 1 got " + _testTable.RowGroups[0].Rows[0].Cells.IndexOf(cell));
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();
        }

        private void CopyToTest()
        {            
            TestLog log = new TestLog("CopyToTest on TableColumnCollection");
            log.LogStatus("Running CopyToTest on TableColumnCollection...");
            log.Result = TestResult.Pass;
            _testTable.Columns.TrimToSize();
            TableColumn[] colArray = new TableColumn[_testTable.Columns.Capacity];
            _testTable.Columns.CopyTo(colArray, 0);
            foreach (TableColumn tc in colArray)
            {
                if (tc == null)
                {
                    log.LogEvidence("Not all of the Columns in the ColumnCollection were copied to the Column array");
                    log.Result = TestResult.Fail;
                }
            }
            // Check to see if we have "real" Table elements in the array
            if (colArray[0].Name == "")
            {
                log.LogEvidence("This TableColumn in the ColumnArray should have a valid Name");
                log.Result = TestResult.Fail;
            }

            Array cArray = Array.CreateInstance(typeof(TableColumn), _testTable.Columns.Capacity);
            _testTable.Columns.CopyTo(cArray, 0);
            foreach (TableColumn tc in cArray)
            {
                if (tc == null)
                {
                    log.LogEvidence("Not all of the Columns in the ColumnCollection were copied to the Array");
                    log.Result = TestResult.Fail;
                }
            }
            //Check to see if we have "real" Table elements in the array
            TableColumn c = cArray.GetValue(0) as TableColumn;
            if (c.Name == "")
            {
                log.LogEvidence("This TableColumn in the Array should have a valid Name");
                log.Result = TestResult.Fail;
            }
            log.Close();
            
            log = new TestLog("CopyToTest on TableRowGroupCollection");
            log.LogStatus("Running CopyToTest on TableRowGroupCollection...");
            log.Result = TestResult.Pass;
            _testTable.RowGroups.TrimToSize();
            TableRowGroup[] groupArray = new TableRowGroup[_testTable.RowGroups.Capacity];
            _testTable.RowGroups.CopyTo(groupArray, 0);
            foreach (TableRowGroup trg in groupArray)
            {
                if (trg == null)
                {
                    log.LogEvidence("Not all of the TableRowGroups in the RowGroupCollection were copied to the RowGroup array");
                    log.Result = TestResult.Fail;
                }
            }
            //Check to see if we have "real" Table elements in the array
            if (groupArray[0].Name == "")
            {
                log.LogEvidence("This TableRowGroup should have a valid Name");
                log.Result = TestResult.Fail;
            }

            Array gArray = Array.CreateInstance(typeof(TableRowGroup), _testTable.RowGroups.Capacity);
            _testTable.RowGroups.CopyTo(gArray, 0);
            foreach (TableRowGroup trg in gArray)
            {
                if (trg == null)
                {
                    log.LogEvidence("Not all of the RowGroups in the RowGroupCollection were copied to the Array");
                    log.Result = TestResult.Fail;
                }
            }
            //Check to see if we have "real" Table elements in the array
            TableRowGroup g = gArray.GetValue(0) as TableRowGroup;
            if (g.Name == "")
            {
                log.LogEvidence("This TableRowGroup in the Array should have a valid Name");
                log.Result = TestResult.Fail;
            }
            log.Close();
           
            log = new TestLog("CopyToTest on TableRowCollection");
            log.LogStatus("Running CopyToTest on TableRowCollection...");
            log.Result = TestResult.Pass;
            _testTable.RowGroups[0].Rows.TrimToSize();
            TableRow[] rowArray = new TableRow[_testTable.RowGroups[0].Rows.Capacity];
            _testTable.RowGroups[0].Rows.CopyTo(rowArray, 0);
            foreach (TableRow tr in rowArray)
            {
                if (tr == null)
                {
                    log.LogEvidence("Not all of the TableRows in the RowCollection were copied to the Row array");
                    log.Result = TestResult.Fail;
                }
            }
            //Check to see if we have "real" Table elements in the array
            if (rowArray[0].Name == "")
            {
                log.LogEvidence("This TableRow should have a valid Name");
                log.Result = TestResult.Fail;
            }

            Array rArray = Array.CreateInstance(typeof(TableRow), _testTable.RowGroups[0].Rows.Capacity);
            _testTable.RowGroups[0].Rows.CopyTo(rArray, 0);
            foreach (TableRow tr in rArray)
            {
                if (tr == null)
                {
                    log.LogEvidence("Not all of the Rows in the RowCollection were copied to the Array");
                    log.Result = TestResult.Fail;
                }
            }
            //Check to see if we have "real" Table elements in the array
            TableRow r = rArray.GetValue(0) as TableRow;
            if (r.Name == "")
            {
                log.LogEvidence("This TableRow in the Array should have a valid Name");
                log.Result = TestResult.Fail;
            }
            log.Close();
            
            log = new TestLog("CopyToTest on TableCellCollection");
            log.LogStatus("Running CopyToTest on TableCellCollection...");
            log.Result = TestResult.Pass;
            _testTable.RowGroups[0].Rows[0].Cells.TrimToSize();
            TableCell[] cellArray = new TableCell[_testTable.RowGroups[0].Rows[0].Cells.Capacity];
            _testTable.RowGroups[0].Rows[0].Cells.CopyTo(cellArray, 0);
            foreach (TableCell cell in cellArray)
            {
                if (cell == null)
                {
                    log.LogEvidence("Not all of the TableCellss in the CellCollection were copied to the Cell array");
                    log.Result = TestResult.Fail;
                }
            }
            //Check to see if we have "real" Table elements in the array
            if (cellArray[0].Name == "")
            {
                log.LogEvidence("This TableCell should have a valid Name");
                log.Result = TestResult.Fail;
            }

            Array clArray = Array.CreateInstance(typeof(TableCell), _testTable.RowGroups[0].Rows[0].Cells.Capacity);
            _testTable.RowGroups[0].Rows[0].Cells.CopyTo(clArray, 0);
            foreach (TableCell cl in clArray)
            {
                if (cl == null)
                {
                    log.LogEvidence("Not all of the Cells in the CellCollection were copied to the Array");
                    log.Result = TestResult.Fail;
                }
            }
            //Check to see if we have "real" Table elements in the array
            TableCell cel = clArray.GetValue(0) as TableCell;
            if (cel.Name == "")
            {

                log.LogEvidence("This TableCell in the Array should have a valid Name");
                log.Result = TestResult.Fail;
            }
            log.Close();

            //CopyTo at a different Index            
            log = new TestLog("CopyToTest on TableRowCollection index not 0");
            log.LogStatus("Running CopyToTest on TableRowCollection starting at an index other than 0...");
            log.Result = TestResult.Pass;
            TableRow[] rowArray2 = new TableRow[_testTable.RowGroups[1].Rows.Capacity];
            _testTable.RowGroups[1].Rows.CopyTo(rowArray2, 2);
            for (int i = 0; i < 3; i++)
            {
                if (i < 2)
                {
                    if (rowArray2[i] != null)
                    {
                        log.LogEvidence("This TableRow in the Array should be null.");
                        log.Result = TestResult.Fail;
                    }
                }
                else
                {
                    if (rowArray2[i] == null)
                    {
                        log.LogEvidence("This TableRow in the Array should NOT be null.");
                        log.Result = TestResult.Fail;
                    }
                }
            }
            log.Close();          
        }

        private void GetEnumeratorTest()
        {
            int count = 0;
            
            TestLog log = new TestLog("GetEnumeratorTest on TableColumnCollection");
            log.LogStatus("Running GetEnumeratorTest on TableColumnCollection...");
            IEnumerator colE = ((IEnumerable)_testTable.Columns).GetEnumerator();
            while (colE.MoveNext())
            {
                count++;
            }
            if (count != _testTable.Columns.Count)
            {
                log.LogEvidence("The number of TableColumn elements in the Enumerator does not equal the count of the TableColumnCollection.");
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();

            count = 0;            
            log = new TestLog("GetEnumeratorTest on TableRowGroupCollection");
            log.LogStatus("Running GetEnumeratorTest on TableRowGroupCollection...");
            IEnumerator groupE = ((IEnumerable)_testTable.RowGroups).GetEnumerator();
            while (groupE.MoveNext())
            {
                count++;
            }
            if (count != _testTable.RowGroups.Count)
            {
                log.LogEvidence("The number of TableRowGroup elements in the Enumerator does not equal the count of the TableRowGroupCollection.");
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();

            count = 0;            
            log = new TestLog("GetEnumeratorTest on TableRowCollection");
            log.LogStatus("Running GetEnumeratorTest on TableRowCollection...");
            IEnumerator rowE = ((IEnumerable)_testTable.RowGroups[1].Rows).GetEnumerator();
            while (rowE.MoveNext())
            {
                count++;
            }
            if (count != _testTable.RowGroups[1].Rows.Count)
            {
                log.LogEvidence("The number of TableRow elements in the Enumerator does not equal the count of the TableRowCollection.");
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();

            count = 0;            
            log = new TestLog("GetEnumeratorTest on TableCellCollection");
            log.LogStatus("Running GetEnumeratorTest on TableCellCollection...");
            IEnumerator cellE = ((IEnumerable)_testTable.RowGroups[1].Rows[2].Cells).GetEnumerator();
            while (cellE.MoveNext())
            {
                count++;
            }
            if (count != _testTable.RowGroups[1].Rows[2].Cells.Count)
            {
                log.LogEvidence("The number of TableCell elements in the Enumerator does not equal the count of the TableCellCollection.");
                log.Result = TestResult.Fail;
            }
            else
            {
                log.Result = TestResult.Pass;
            }
            log.Close();
        }

        ///////////////////////////////////////////////////////////
        ///                                                     ///
        ///                 Helper Methods                      /// 
        ///                                                     ///
        ///////////////////////////////////////////////////////////

        private int[] GetCollectionCount(Table table)
        {
            int[] collectionCount = new int[4];

            collectionCount[0] = _testTable.Columns.Count;
            collectionCount[1] = _testTable.RowGroups.Count;

            foreach (TableRowGroup trg in _testTable.RowGroups)
            {
                collectionCount[2] = collectionCount[2] + trg.Rows.Count;
                foreach (TableRow row in trg.Rows)
                {
                    collectionCount[3] = collectionCount[3] + row.Cells.Count;
                }
            }
            return collectionCount;
        }

        private TestResult VerifyName(string control, string experimental)
        {
            GlobalLog.LogStatus("Verify Name");
            if (control != experimental)
            {
                GlobalLog.LogStatus("Element Name was not as expected.");
                GlobalLog.LogStatus("Expected: " + control + " Actual was " + experimental);
                return TestResult.Fail;
            }
            else
            {
                return TestResult.Pass;
            }           
        }

        private TestResult VerifyCollectionCount(int[] control, int[] experimental)
        {
            GlobalLog.LogStatus("Verify Collection Count");              
            for (int i = 0; i < control.Length; i++)
            {
                if (control[i] != experimental[i])
                {
                    GlobalLog.LogStatus("Collection counts were not as expected:");
                    GlobalLog.LogStatus("Count at " + TableElement(i) + " was " + experimental[i] + " expected " + control[i]);
                    return TestResult.Fail;
                }               
            } 
            return TestResult.Pass;
        }

        private string TableElement(int index)
        {
            switch (index)
            {
                case 0:
                    return "TableColumn";
                case 1:
                    return "TableRowGroup";
                case 2:
                    return "TableRow";
                case 3:
                    return "TableCell";
            }
            return "unkown";
        }

        private bool DoesElementExistInTable(object tableElement, Table table)
        {
            foreach (TableColumn tcol in table.Columns)
            {
                if (tcol == tableElement)
                    return true;
            }

            foreach (TableRowGroup trg in table.RowGroups)
            {
                if (trg == tableElement)
                    return true;
                foreach (TableRow tr in trg.Rows)
                {
                    if (tr == tableElement)
                        return true;

                    foreach (TableCell tc in tr.Cells)
                    {
                        if (tc == tableElement)
                            return true;
                    }
                }
            }
            return false;
        }
    }
}
