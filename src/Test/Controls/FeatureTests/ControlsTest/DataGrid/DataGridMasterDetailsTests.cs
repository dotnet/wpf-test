using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Actions;
using Avalon.Test.ComponentModel.DataSources;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Verification;
using Microsoft.Test.Controls.DataSources;


namespace Microsoft.Test.Controls
{
    /// <summary>
    ///  DataGrid's Master/Details Scenario Tests
    ///         a. DG binding to the collectionview of NewPeople
    ///         b. Details' ContentControl binding to the same CSV as the DG
    ///         c. Verify the sync-up of the master and details

    /// </summary>
    [Test(0, "DataGrid", "DataGridMasterDetailsTestsBVT", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridMasterDetailsTests : XamlTest
    {
        #region Private Fields

        Page page;
        DataGrid dataGrid;
        NewPeople people;
        ContentControl detailControl;        
        TextBlock firstNameTextBlock;
        
        #endregion

        #region Constructor

        public DataGridMasterDetailsTests()
            : base(@"DataGridMasterDetailsTestsBVT.xaml")
        {
            InitializeSteps += new TestStep(Setup);          // initial setup
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestMasterDetails);   
        }

        #endregion
        
        #region Test Steps

        /// <summary>
        /// Initial Setup  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        TestResult Setup()
        {
            Status("Setup");

            dataGrid = (DataGrid)RootElement.FindName("DataGrid_Standard");
            Assert.AssertTrue("Can not find the DataGrid in the xaml file!", dataGrid != null);

            page = (Page)this.Window.Content;     
            people = (NewPeople)(page.FindResource("people")); 
            Assert.AssertTrue("Can not find the data source in the xaml file!", people != null);
            
            detailControl = (ContentControl)RootElement.FindName("DetailContent");
            Assert.AssertTrue("Can not find the details ContentControl!", detailControl != null);

            ContentPresenter myContentPresenter = DataGridHelper.FindVisualChild<ContentPresenter>(detailControl);
            DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;
            firstNameTextBlock = (TextBlock)myDataTemplate.FindName("firstNameTextBlock", myContentPresenter);
            Assert.AssertTrue("Can not find the detail textblock!", firstNameTextBlock != null);

            dataGrid.Focus();

            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        TestResult CleanUp()
        {
            dataGrid = null;
            page = null;
            detailControl = null;
            firstNameTextBlock = null;
            return TestResult.Pass;
        }

        /// <summary>
        /// Test the sync-up of the master and the detail
        /// </summary>
        /// <returns></returns>
        TestResult TestMasterDetails()
        {
            Status("TestMasterDetails");

            for (int i = 0; i < people.Count; i++)
            {
                dataGrid.SelectedItem = people[i];
                Assert.AssertTrue("Error in getting the details!", firstNameTextBlock.Text == people[i].FirstName);
            }

            LogComment("TestMasterDetails was successful");
            return TestResult.Pass;
        }

        #endregion

    }
}
