using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.ApplicationControl;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression Test   Verify that inherited properties are coerced down the
    /// tree correctly.
    /// </description>

    /// </summary>
    // Not enabled for v3.X tests because this bug fix is only present in v4.X
    [Test(0, "ElementServices", "InheritedDPRegressionTest7", SecurityLevel = TestCaseSecurityLevel.FullTrust, Versions = "4.0+,4.0Client+")]
    public class InheritedDPRegressionTest7 : StepsTest
    {
        private const int DefaultTimeoutInMS = 60000;
        private readonly string fileName = "ControlsAutomationTest.exe";
        private readonly string windowClassName = "Microsoft.Test.Controls.InheritedDPRegressionTest7";

        #region Constructor

        public InheritedDPRegressionTest7()
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestInheritedPropertyCoercion);
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initial Setup
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public TestResult Setup()
        {
            Status("Setup specific for InheritedDPRegressionTest7");


            LogComment("Setup for InheritedDPRegressionTest7 was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 1. Start the app
        /// 2. Check the entire visual tree and make sure Window changes to 10, Grid and all descendents change to 5
        /// 3. Click the 'Force Coercion' button
        /// 4. Verify Window stays as 10, Grid and all decendents change to 5.
        /// </summary>
        private TestResult TestInheritedPropertyCoercion()
        {
            Status("TestInheritedPropertyCoercion");

            var automatedApp = new InProcessApplication(new WpfInProcessApplicationSettings
                {
                    InProcessApplicationType = InProcessApplicationType.InProcessSeparateThread,
                    Path = fileName,
                    WindowClassName = windowClassName,
                    ApplicationImplementationFactory = new WpfInProcessApplicationFactory()
                });

            LogComment("1. Start the app");
            automatedApp.Start();
            automatedApp.WaitForMainWindow(TimeSpan.FromMilliseconds(DefaultTimeoutInMS));

            var app = (automatedApp as InProcessApplication).ApplicationDriver as Application;
            bool failed = false;
            int expectedValue = 10;

            app.Dispatcher.Invoke(DispatcherPriority.Normal,
                (ThreadStart)delegate
                {
                    var mainWindow = automatedApp.MainWindow as Window;
                    mainWindow.Activate();

                    var dp = mainWindow.GetType().InvokeMember(
                        "InheritedIntProperty",
                        BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public,
                        null,
                        mainWindow,
                        null) as DependencyProperty;

                    LogComment("2. Check the entire visual tree and make sure Window changes to 10, Grid and all descendents change to 5");

                    // NOTE: have to run this algorithm within the dispatcher
                    var stack = new Stack<DependencyObject>();
                    stack.Push(mainWindow);
                    while (stack.Count > 0)
                    {
                        var elem = stack.Pop();
                        if (elem != null)
                        {
                            LogComment(string.Format("Visiting element: {0}", elem));

                            // once grid is found, DP and decendents should update to 5
                            if (elem is Grid)
                            {
                                expectedValue = 5;
                            }

                            var actualValue = (int)elem.GetValue(dp);
                            if (expectedValue != actualValue)
                            {
                                LogComment(string.Format("Failed at: {0}.{1} expected: {2}, actual: {3}", elem, dp, expectedValue, actualValue));
                                failed = true;
                                break;
                            }

                            int numVisuals = System.Windows.Media.VisualTreeHelper.GetChildrenCount(elem);
                            for (int i = 0; i < numVisuals; i++)
                            {
                                stack.Push(System.Windows.Media.VisualTreeHelper.GetChild(elem, i));
                            }
                        }
                    }
                });

            if (failed)
            {
                throw new TestValidationException("Inherited DP did not coerce correctly.");
            }

            failed = false;
            expectedValue = 10;

            LogComment("3. Click the 'Force Coercion' button");
            app.Dispatcher.Invoke(DispatcherPriority.Normal,
               (ThreadStart)delegate
               {
                   var mainWindow = automatedApp.MainWindow as Window;
                   mainWindow.Activate();

                   var launchButton = (Button)VisualTreeUtils.GetVisualChild<Button>(mainWindow);
                   Assert.AssertTrue("launchButton was not found", launchButton != null);

                   var clickPointWPF = launchButton.PointToScreen(new Point(2, 2));
                   var clickPoint = new System.Drawing.Point();
                   clickPoint.X = (int)clickPointWPF.X;
                   clickPoint.Y = (int)clickPointWPF.Y;

                   Microsoft.Test.Input.Mouse.MoveTo(clickPoint);
                   automatedApp.WaitForInputIdle(TimeSpan.FromMilliseconds(DefaultTimeoutInMS));
                   Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Left);

                   var dp = mainWindow.GetType().InvokeMember(
                       "InheritedIntProperty",
                       BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public,
                       null,
                       mainWindow,
                       null) as DependencyProperty;

                   LogComment("4. Verify Window stays as 10, Grid and all decendents change to 5.");

                   // NOTE: have to run this algorithm within the dispatcher
                   var stack = new Stack<DependencyObject>();
                   stack.Push(mainWindow);
                   while (stack.Count > 0)
                   {
                       var elem = stack.Pop();
                       if (elem != null)
                       {
                           LogComment(string.Format("Visiting element: {0}", elem));

                           // once grid is found, DP and decendents should update to 5
                           if (elem is Grid)
                           {
                               expectedValue = 5;
                           }

                           var actualValue = (int)elem.GetValue(dp);
                           if (expectedValue != actualValue)
                           {
                               LogComment(string.Format("Failed at: {0}.{1} expected: {2}, actual: {3}", elem, dp, expectedValue, actualValue));
                               failed = true;
                               break;
                           }

                           int numVisuals = System.Windows.Media.VisualTreeHelper.GetChildrenCount(elem);
                           for (int i = 0; i < numVisuals; i++)
                           {
                               stack.Push(System.Windows.Media.VisualTreeHelper.GetChild(elem, i));
                           }
                       }
                   }
               });

            automatedApp.WaitForInputIdle(TimeSpan.FromMilliseconds(DefaultTimeoutInMS));
            automatedApp.Close();

            LogComment("TestInheritedPropertyCoercion was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps
    }
}
