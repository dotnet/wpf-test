// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
     Regression test for Regression_Bug31
     when hosted the down mouse arrow is pressed on a RightToLeft
     FlowDocumentScrollViewer an exception is thrown
     Expected: No exception is thrown
*/
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>  
    /// Testing regression test.    
    /// </summary>
    [Test(3, "Bottomless", "Regression_Bug34")]
    class TestRegression_Bug34: AvalonTest
    {       
        private Window _window;
        private Table _table;
               
        public TestRegression_Bug34()
            : base()
        {            
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);
        }
             
        /// <summary>
        /// Initialize: setup tests
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {            
            Status("Initialize ....");

            _window = new Window();
            _table = new Table();
            TableRowGroup body = new TableRowGroup();
            TableRow row = new TableRow();
            TableCell cell = new TableCell(new Paragraph(new Run("some random text")));
            cell.ColumnSpan = 12; //this is the magic sauce
            row.Cells.Add(cell);
            body.Rows.Add(row);
            _table.RowGroups.Add(body);
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            fdsv.Document = new FlowDocument(_table);
            _window.Content = fdsv;
            _window.Show();
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _window.Close();
            return TestResult.Pass;
        }

        /// <summary>
        /// RunTests: Run tests
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTests()
        {           
            try
            {
                _table.RowGroups.Clear();
                return TestResult.Pass;
            }
            catch (Exception e)
            {
                Status("Regression_Bug36 may have regressed" + e.Message);
                return TestResult.Fail;                               
            }          
        }     
    }
}
