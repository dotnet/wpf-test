using System.Windows.Controls;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System;
using Microsoft.Test.Input;
using Avalon.Test.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Controls.Primitives;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Verify Popup.IsOpen SetCurrentValue is working correctly.
    /// </description>

    /// </summary>
    [Test(0, "ContextMenu", "ContextMenuRegressionTest18", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class ContextMenuRegressionTest18 : RegressionXamlTest
    {
        private Button launchPopUpButton;
        private ContextMenu popup;

        #region Constructor

        public ContextMenuRegressionTest18()
            : base(@"ContextMenuRegressionTest18.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestPopUpIsOpen);
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initial Setup  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public override TestResult Setup()
        {
            Status("Setup specific for ContextMenuRegressionTest18");

            launchPopUpButton = (Button)RootElement.FindName("launchPopUpButton");
            Assert.AssertTrue("Unable to find launchPopUpButton from the resources", launchPopUpButton != null);

            popup = (ContextMenu)RootElement.FindResource("popup");
            Assert.AssertTrue("Unable to find popup from the resources", popup != null);

            launchPopUpButton.Click += launchPopUpButton_Click;            

            QueueHelper.WaitTillQueueItemsProcessed();

            // Uncomment for ad-hoc debugging
            //base.Setup();

            LogComment("Setup for ContextMenuRegressionTest18 was successful");
            return TestResult.Pass;
        }

        public override TestResult CleanUp()
        {
            launchPopUpButton.Click -= launchPopUpButton_Click;
            launchPopUpButton = null;
            popup = null;
            return base.CleanUp();
        }

        /// <summary>
        /// 1. click the launchPopupButton => verify popup is opened
        /// 2. click away from the window
        /// 3. click the launchPopupButton again
        /// 
        /// Verify popup is opened again 
        /// </summary>
        private TestResult TestPopUpIsOpen()
        {
            Status("TestPopUpIsOpen");

            LogComment("click the launch button");
            UserInput.MouseLeftClickCenter(launchPopUpButton);
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("verify popup is open");
            Assert.AssertTrue("popup should be open when button is clicked", popup.IsOpen);

            LogComment("click away from the window");
            UserInput.MouseLeftDown(RootElement, -4, -4);
            QueueHelper.WaitTillQueueItemsProcessed();
            UserInput.MouseLeftUp(RootElement, -4, -4);
            QueueHelper.WaitTillQueueItemsProcessed();

            this.WaitFor(500);

            LogComment("verify popup is closed");
            Assert.AssertTrue("popup should not be open at this point", !popup.IsOpen);

            LogComment("click the launch button again");
            UserInput.MouseLeftClickCenter(launchPopUpButton);
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("verify popup is open");
            Assert.AssertTrue("popup should be open when button is clicked", popup.IsOpen);

            LogComment("TestPopUpIsOpen was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps

        #region Helpers        

        private void launchPopUpButton_Click(object sender, RoutedEventArgs e)
        {
            popup.PlacementTarget = launchPopUpButton;
            popup.Placement = PlacementMode.Right;
            popup.IsOpen = true;
            popup.StaysOpen = false;
        }

        #endregion Helpers        
    }    
}
