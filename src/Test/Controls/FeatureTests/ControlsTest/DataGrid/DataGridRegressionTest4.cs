using System.Windows.Controls;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.ApplicationControl;
using System;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Automation;
using Microsoft.Test.Input;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression Test   Verify that 'esc' key is routed up the tree
    /// when a DataGrid does not make use of it for editing scenarios.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridRegressionTest4", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridRegressionTest4 : StepsTest
    {
        public const int DefaultTimeoutInMS = 60000;
        private string fileName = "ControlsAutomationTest.exe";
        private string windowClassName = "Microsoft.Test.Controls.DataGridRegressionTest4";

        #region Constructor

        public DataGridRegressionTest4()            
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestEscKeyRouting);
        }

        #endregion
        
        #region Test Steps

        /// <summary>
        /// Initial Setup  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public TestResult Setup()
        {
            Status("Setup specific for DataGridRegressionTest4");

                   

            LogComment("Setup for DataGridRegressionTest4 was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 1. From the main window, launch the dialog from the 'launch button'
        /// 2. In the dialog, click on one of the DataGrid cells (setting focus to the DataGrid)
        /// 3. Press 'Esc' to close the dialog
        /// 
        /// Verify the dialog closed.
        /// </summary>
        private TestResult TestEscKeyRouting()
        {
            Status("TestEscKeyRouting");

            var automatedApp = new InProcessApplication(new WpfInProcessApplicationSettings
            {
                InProcessApplicationType = InProcessApplicationType.InProcessSeparateThread,
                Path = fileName,
                WindowClassName = windowClassName,
                ApplicationImplementationFactory = new WpfInProcessApplicationFactory()
            });     
            automatedApp.Start();
            automatedApp.WaitForMainWindow(TimeSpan.FromMilliseconds(DefaultTimeoutInMS));
            var app = (automatedApp as InProcessApplication).ApplicationDriver as Application;

            // find the launch button (the first button in the tree), and click it
            app.Dispatcher.Invoke(DispatcherPriority.Normal,
                (ThreadStart)delegate
                {                    
                    var mainWindow = automatedApp.MainWindow as Window;
                    mainWindow.Activate();
                    
                    var launchButton = (Button)DataGridHelper.FindVisualChild<Button>(mainWindow);
                    Assert.AssertTrue("launchButton was not found", launchButton != null);                   

                    LogComment("1. From the main window, launch the dialog from the 'launch button'");                    
                    var clickPointWPF = launchButton.PointToScreen(new Point(2, 2));
                    var clickPoint = new System.Drawing.Point();
                    clickPoint.X = (int)clickPointWPF.X;
                    clickPoint.Y = (int)clickPointWPF.Y;

                    Microsoft.Test.Input.Mouse.MoveTo(clickPoint);
                    automatedApp.WaitForInputIdle(TimeSpan.FromMilliseconds(DefaultTimeoutInMS));
                    Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Left);
                });

            LogComment("wait for the window");
            string testDialogStr = "TestDialog";
            automatedApp.WaitForWindow(testDialogStr, TimeSpan.FromMilliseconds(DefaultTimeoutInMS));

            app.Dispatcher.Invoke(DispatcherPriority.Normal,
                (ThreadStart)delegate
                {
                    Window testDialog = null;
                    foreach (Window window in app.Windows)
                    {
                        var id = (string)window.GetValue(AutomationProperties.AutomationIdProperty);
                        if (id != null && id == testDialogStr)
                        {
                            testDialog = window;
                        }
                    }
                    Assert.AssertTrue("testDialog was not found", testDialog != null);                   

                    // find the DataGrid
                    var dataGrid = (DataGrid)DataGridHelper.FindVisualChild<DataGrid>(testDialog);
                    Assert.AssertTrue("dataGrid was not found", dataGrid != null);                   

                    LogComment("2. In the dialog, click on one of the DataGrid cells (setting focus to the DataGrid)");
                    DataGridActionHelper.ClickOnCell(dataGrid, 0, 0);
                    automatedApp.WaitForInputIdle(TimeSpan.FromMilliseconds(DefaultTimeoutInMS));

                    LogComment("3. Press 'Esc' to close the dialog");
                    Keyboard.Press(Key.Escape);
                    automatedApp.WaitForInputIdle(TimeSpan.FromMilliseconds(DefaultTimeoutInMS));

                    LogComment("Verify the dialog closed.");
                    Assert.AssertTrue("Dialog did not close correctly.", app.Windows.Count == 1);
                });

            automatedApp.WaitForInputIdle(TimeSpan.FromMilliseconds(DefaultTimeoutInMS));
            automatedApp.Close();

            LogComment("TestEscKeyRouting was successful");
            return TestResult.Pass;
        }                

        #endregion Test Steps      
    }
}
