using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression Test   Verify that DatePicker.SelectedDate does not get set by local value
    /// internally.
    /// </description>

    /// </summary>
    [Test(0, "DatePicker", "DatePickerRegressionTest46", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DatePickerRegressionTest46 : XamlTest
    {
        private Button debugButton;
        private CollectionViewSource cvs;
        private DatePicker datePicker;
        private Button moveNextButton;
        private Button movePrevButton;

        #region Constructor

        public DatePickerRegressionTest46()
            : base(@"DatePickerRegressionTest46.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestSelectedDate);
        }

        #endregion
        
        #region Test Steps

        /// <summary>
        /// Initial Setup  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public TestResult Setup()
        {
            Status("Setup specific for DatePickerRegressionTest46");

            debugButton = (Button)RootElement.FindName("btn_Debug");
            Assert.AssertTrue("Unable to find btn_Debug from the resources", debugButton != null);

            cvs = (CollectionViewSource)RootElement.FindResource("cvs");
            Assert.AssertTrue("Unable to find cvs from the resources", cvs != null);

            datePicker = (DatePicker)RootElement.FindName("datePicker");
            Assert.AssertTrue("Unable to find datePicker from the resources", datePicker != null);

            moveNextButton = (Button)RootElement.FindName("moveNextButton");
            Assert.AssertTrue("Unable to find moveNextButton from the resources", moveNextButton != null);

            movePrevButton = (Button)RootElement.FindName("movePrevButton");
            Assert.AssertTrue("Unable to find movePrevButton from the resources", movePrevButton != null);

            moveNextButton.Click += new RoutedEventHandler(moveNextButton_Click);
            movePrevButton.Click += new RoutedEventHandler(movePrevButton_Click);

            QueueHelper.WaitTillQueueItemsProcessed();

            cvs.View.MoveCurrentToFirst();
            QueueHelper.WaitTillQueueItemsProcessed();

            //Debug();

            LogComment("Setup for DatePickerRegressionTest46 was successful");
            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            debugButton = null;
            cvs = null;
            datePicker = null; 
            moveNextButton = null;
            movePrevButton = null;
            return TestResult.Pass;
        }

        /// <summary>
        /// 1. call MoveCurrentToNext twice => verify SelectedDate updates DisplayDate accordingly
        /// 2. call MoveCurrentToPrev => verify SelectedDate updates DisplayDate accordingly
        /// </summary>
        private TestResult TestSelectedDate()
        {
            Status("TestSelectedDate");

            LogComment("move to next");
            cvs.View.MoveCurrentToNext();
            QueueHelper.WaitTillQueueItemsProcessed();

            // get the initial value
            var prevDateTime = datePicker.DisplayDate;

            LogComment("move to next");
            cvs.View.MoveCurrentToNext();
            QueueHelper.WaitTillQueueItemsProcessed();

            var curDateTime = datePicker.DisplayDate;
            Assert.AssertTrue(
                string.Format("SelectedDate should not be equal at this time.  Prev: {0}, After: {1}", prevDateTime, curDateTime), 
                prevDateTime != curDateTime);

            prevDateTime = datePicker.DisplayDate;

            LogComment("move to next");
            cvs.View.MoveCurrentToNext();
            QueueHelper.WaitTillQueueItemsProcessed();

            curDateTime = datePicker.DisplayDate;
            Assert.AssertTrue(
                string.Format("SelectedDate should not be equal at this time.  Prev: {0}, After: {1}", prevDateTime, curDateTime),
                prevDateTime != curDateTime);

            prevDateTime = datePicker.DisplayDate;

            LogComment("move to prev");
            cvs.View.MoveCurrentToPrevious();
            QueueHelper.WaitTillQueueItemsProcessed();

            curDateTime = datePicker.DisplayDate;
            Assert.AssertTrue(
                string.Format("SelectedDate should not be equal at this time.  Prev: {0}, After: {1}", prevDateTime, curDateTime),
                prevDateTime != curDateTime);

            LogComment("TestSelectedDate was successful");
            return TestResult.Pass;
        }                

        #endregion Test Steps   
   
        #region Helpers

        private void moveNextButton_Click(object sender, RoutedEventArgs e)
        {
            cvs.View.MoveCurrentToNext();
        }

        private void movePrevButton_Click(object sender, RoutedEventArgs e)
        {
            cvs.View.MoveCurrentToPrevious();
        }

        
        private void Debug()
        {
            // To keep this thread busy, we'll have to push a frame.
            DispatcherFrame frame = new DispatcherFrame();

            debugButton.MouseLeftButtonDown += (sender, e) =>
            {
                frame.Continue = false;
            };

            Dispatcher.PushFrame(frame);
        }        

        #endregion Helpers
    }
}
