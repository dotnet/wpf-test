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
using System.Windows.Automation;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Input;
using System.Windows.Input;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression Test   Verify that inherited properties are coerced down the 
    /// tree correctly.
    /// </description>
    /// </summary>
    [Test(0, "Calendar", "CalendarRegressionTest2", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class CalendarRegressionTest2 : StepsTest
    {
        private const int DefaultTimeoutInMS = 60000;
        private readonly string fileName = "ControlsAutomationTest.exe";
        private readonly string windowClassName = "Microsoft.Test.Controls.CalendarRegressionTest2";

        #region Constructor

        public CalendarRegressionTest2()            
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestCalendarSelectedItemEvent);
        }

        #endregion
        
        #region Test Steps

        /// <summary>
        /// Initial Setup  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public TestResult Setup()
        {
            Status("Setup specific for CalendarRegressionTest2");


            LogComment("Setup for CalendarRegressionTest2 was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// 1. Start the app
        /// 2. select a date
        /// 3. ctrl+click to select another date
        /// 
        /// Verify the last step triggers the SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent
        /// </summary>
        private TestResult TestCalendarSelectedItemEvent()
        {
            Status("TestCalendarSelectedItemEvent");

            bool eventFired = false;

            var automatedApp = new InProcessApplication(new WpfInProcessApplicationSettings
                {
                    InProcessApplicationType = InProcessApplicationType.InProcessSeparateThread,
                    Path = fileName,
                    WindowClassName = windowClassName,
                    ApplicationImplementationFactory = new WpfInProcessApplicationFactory()
                });

            try
            {
                LogComment("1. Start the app");
                automatedApp.Start();
                automatedApp.WaitForMainWindow(TimeSpan.FromMilliseconds(DefaultTimeoutInMS));

                var calendarElement = AutomationElement.RootElement.FindFirst(TreeScope.Descendants | TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, "testCalendar"));
                Assert.AssertTrue("unable to find the calendar element", calendarElement != null);

                Automation.AddAutomationEventHandler(
                   SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent,
                   calendarElement,
                   TreeScope.Subtree | TreeScope.Element | TreeScope.Descendants | TreeScope.Children,
                   (s, e) => 
                   { 
                       eventFired = true; 
                   });

                automatedApp.WaitForInputIdle(TimeSpan.FromSeconds(30));

                var app = (automatedApp as InProcessApplication).ApplicationDriver as Application;
                app.Dispatcher.Invoke(DispatcherPriority.Normal,
                   (ThreadStart)delegate
                   {
                       var mainWindow = automatedApp.MainWindow as Window;
                       mainWindow.Activate();

                       var calendarDayButtons = VisualTreeHelper.GetVisualChildren<CalendarDayButton>(mainWindow);

                       LogComment("2. select a date");
                       UserInput.MouseLeftClickCenter(calendarDayButtons[calendarDayButtons.Count / 2]);
                       automatedApp.WaitForInputIdle(TimeSpan.FromSeconds(30));

                       LogComment("3. ctrl+click to select another date");
                       UserInput.KeyDown(System.Windows.Input.Key.LeftCtrl.ToString());
                       automatedApp.WaitForInputIdle(TimeSpan.FromSeconds(30));

                       UserInput.MouseLeftDown(calendarDayButtons[(calendarDayButtons.Count / 2) + 1]);
                       automatedApp.WaitForInputIdle(TimeSpan.FromSeconds(30));

                       UserInput.MouseLeftUp(calendarDayButtons[(calendarDayButtons.Count / 2) + 1]);
                       automatedApp.WaitForInputIdle(TimeSpan.FromSeconds(30));

                       UserInput.KeyUp(System.Windows.Input.Key.LeftCtrl.ToString());
                       automatedApp.WaitForInputIdle(TimeSpan.FromSeconds(30));
                   });

                automatedApp.WaitForInputIdle(TimeSpan.FromSeconds(30));

                if (!eventFired)
                {
                    throw new TestValidationException("SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent did not fire correctly during multi-selection of calendar item.");
                }
            }
            finally
            {
                automatedApp.WaitForInputIdle(TimeSpan.FromSeconds(30));
                automatedApp.Close();
            }

            LogComment("TestCalendarSelectedItemEvent was successful");
            return TestResult.Pass;
        }                

        #endregion Test Steps      
    }
}
