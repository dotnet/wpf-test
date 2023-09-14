// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
    Test to prevent regression of Regression_Bug11
    Table with Body and Row but no cells throws PTS exception in MeasureOverride
*/
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>    
    /// Testing Rows with no cells.    
    /// </summary>
    [Test(2, "Table", "RowNoCells")]
    public class RowNoCells : WindowTest
    {
        private NavigationWindow _testWin;

        #region Constructor

        public RowNoCells()
            : base()
        {
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);           
        }

        #endregion

        #region Test Steps
      
        /// <summary>
        /// Initialize: Setup the test
        /// </summary>
        /// <returns>TestResult.Pass;</returns>
        private TestResult Initialize()
        {           
            Status("Initialize");
            _testWin = new NavigationWindow();
            _testWin.Top = 0;
            _testWin.Left = 0;
            _testWin.Height = 600;
            _testWin.Width = 800;
            _testWin.Title = "LayoutTestWindow";
            LogComment("Adding layout elements...");
            _testWin.Content = content() as FrameworkElement;
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
        /// RunTests: Runs a set of tests based on the input variation.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTests()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);
            this.Window.Close();
            LogComment("Success");
            
            return TestResult.Pass;
        }

        #endregion

        private UIElement content()
        {
            TableRow r = new TableRow();

            TableRowGroup b = new TableRowGroup();
            b.Rows.Add(r);

            Table t = new Table();
            t.RowGroups.Add(b);

            FlowDocumentScrollViewer eRoot = new FlowDocumentScrollViewer();
            eRoot.Document = new FlowDocument(t);
            
            return eRoot;
        }
    }
}
