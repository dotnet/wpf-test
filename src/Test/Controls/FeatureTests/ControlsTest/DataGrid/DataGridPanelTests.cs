using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using Microsoft.Test;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.Discovery;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Verification;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Controls.DataSources;


namespace Microsoft.Test.Controls
{
    /// <summary>
    ///
    /// Panel tests for DataGridRowsPanel and DataGridCellsPanel
    ///         DataGridRowsPanel tests
    ///             a. VSP compat tests
    ///             b. Verification of the row layout
    ///             c. Verification of the layout upon 
    ///                     Scrolling
    ///                     Add and Remove item in/from the collection
    ///                     Column resize
    ///                     Header Visibility Changes 
    ///                     Frozen columns
    ///             
    ///         DataGridCellsPanel tests
    ///             a. Panel compat tests - NYI
    ///             b. Visual verification of the cell layout

    /// </summary>
    [Test(0, "DataGrid", "DataGridPanelTestsBVT", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridPanelTests : XamlTest
    {
        #region Private Fields

        Page page;
        DataGrid dataGrid;
        NewPeople people;

        #endregion

        #region Constructor

        public DataGridPanelTests()
            : base(@"DataGridPanelTestsBVT.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestScrolling);
            RunSteps += new TestStep(TestColumnResizing);
            //RunSteps += new TestStep(TestHeaderVisibility);

        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initial setup to locate the DataGrid being tested and other controls used in testing, 
        /// applying the data source, etc. 
        /// </summary>
        /// <returns></returns>
        TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            dataGrid = (DataGrid)RootElement.FindName("DataGrid_Standard");
            Assert.AssertFalse("Can not find the DataGrid in the xaml file!", dataGrid == null);

            page = (Page)this.Window.Content;     
            people = (NewPeople)(page.FindResource("people"));     
            Assert.AssertTrue("Can not find the data source in the xaml file!", people != null);

            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        TestResult CleanUp()
        {
            dataGrid = null;
            page = null;
            return TestResult.Pass;
        }

        /// <summary>
        /// Tests for scrolling and re-layout
        /// </summary>
        /// <returns></returns>
        TestResult TestScrolling()
        {
            Status("TestScrolling");
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            // Ad-hoc testing currently

            LogComment("TestScrolling was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Tests for column width changes
        /// </summary>
        /// <returns></returns>
        TestResult TestColumnResizing()
        {
            Status("TestColumnResizing");
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            // 

            LogComment("TestColumnResizing was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Tests for HeaderVisibility changes
        /// </summary>
        /// <returns></returns>
        TestResult TestHeaderVisibility()
        {
            Status("TestHeaderVisibility");
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            // 

            LogComment("TestHeaderVisibility was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Tests for FrozenColumn changes - not in M1
        /// </summary>
        /// <returns></returns>
        TestResult TestFrozenColumn()
        {
            Status("TestFrozenColumn");
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            // 

            LogComment("TestFrozenColumn was successful");
            return TestResult.Pass;
        }


        #endregion
    }
}
