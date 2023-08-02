using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Assert = Microsoft.Test.Controls.TestAsserts;

namespace Microsoft.Test.Controls
{
    [Test(0, "Calendar", TestCaseSecurityLevel.FullTrust, "SelectedDateTest", Keywords = "Localization_Suite")]
    public class SelectedDateTest : CalendarTest
    {
        public SelectedDateTest()
            : base()
        {
            this.RunSteps += ArePropertiesNullable;
            this.RunSteps += SetToMaxValue;
            this.RunSteps += SetToMinValue;
            this.RunSteps += SelectedDatesChangedEvent;
            this.RunSteps += KeyboardInputProviderAcquireFocusEvent;
            this.RunSteps += SelectedDatesChangedEventArgs_SingleDate;
            this.RunSteps += SelectedDatesChangedEventArgs_SingleRange;
            this.RunSteps += SelectedDatesChangedEventArgs_MultipleRange;
            this.RunSteps += SelectDayNoneInvalidOperationException;
            this.RunSteps += SelectedDatesCollectionException_None;
            this.RunSteps += SelectedDatesCollectionException_SingleDate;
            this.RunSteps += SelectedDateSingle;
            this.RunSteps += SelectedDateSingleRange;
            this.RunSteps += SelectedDateMultipleRange;
            this.RunSteps += SelectedatesAddEmpty;
        }

        /// <summary>
        /// Ensure Nullable Properties can be set to null.
        /// </summary>
        public TestResult ArePropertiesNullable()
        {
            Calendar calendar = new Calendar();
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            DateTime value = DateTime.Today;

            calendar.SelectedDate = null;
            Assert.IsNull(calendar.SelectedDate, "SelectedDate was not set to null.");

            calendar.SelectedDate = value;
            Assert.AreEqual(calendar.SelectedDate.Value, value);

            calendar.SelectedDate = null;
            Assert.IsNull(calendar.SelectedDate, "SelectedDate was not set back to null.");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure DateTime Properties can be set to DateTime.MaxValue.
        /// </summary>
        public TestResult SetToMaxValue()
        {
            Calendar calendar = new Calendar();
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            calendar.DisplayDate = DateTime.MaxValue;

            calendar.SelectedDate = DateTime.MaxValue;

            Assert.AreEqual((DateTime)calendar.SelectedDate, DateTime.MaxValue);

            Assert.IsTrue(calendar.SelectedDates.Count == 1, "SelectedDate should == 1.");
            Assert.AreEqual(calendar.SelectedDates[0], calendar.SelectedDate.Value);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure DateTime Properties can be set to DateTime.MinValue.
        /// </summary>
        public TestResult SetToMinValue()
        {
            Calendar calendar = new Calendar();
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            calendar.DisplayDate = DateTime.MinValue;

            calendar.SelectedDate = DateTime.MinValue;

            Assert.AreEqual((DateTime)calendar.SelectedDate, DateTime.MinValue);

            Assert.IsTrue(calendar.SelectedDates.Count == 1, "SelctedDates.Count should == 1");
            Assert.AreEqual(calendar.SelectedDates[0], calendar.SelectedDate.Value);

            ResetTest();
            return TestResult.Pass;
        }

        private void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            isEventFired = true;
        }

        /// <summary>
        /// Ensure DateSelected event is fired.
        /// </summary>
        public TestResult SelectedDatesChangedEvent()
        {
            Calendar calendar = new Calendar();
            calendar.SelectionMode = CalendarSelectionMode.SingleDate;

            calendar.SelectedDatesChanged += new EventHandler<SelectionChangedEventArgs>(calendar_SelectedDatesChanged);

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            DateTime value = new DateTime(2000, 10, 10);
            calendar.SelectedDate = value;

            Assert.IsTrue(isEventFired, "Event not handled.");
            Assert.AreEqual(calendar.ToString(), value.ToString());

            calendar.SelectedDatesChanged -= new EventHandler<SelectionChangedEventArgs>(calendar_SelectedDatesChanged);

            ResetTest();
            return TestResult.Pass;
        }

        private void TestKeyboardInputProviderAcquireFocusEvent(RoutingStrategy routingStrategy)
        {
            StackPanel stackpanel = new StackPanel();
            this.TestUI.Children.Add(stackpanel);

            Calendar calendar = new Calendar();
            stackpanel.Children.Add(calendar);

            // Test event callback is fired when there is event attached
            // Attach event
            switch(routingStrategy)
            {
                case RoutingStrategy.Tunnel:
                    Keyboard.AddPreviewKeyboardInputProviderAcquireFocusHandler(calendar, KeyboardInputProviderAcquireFocus);
                    break;
                case RoutingStrategy.Bubble:
                    Keyboard.AddKeyboardInputProviderAcquireFocusHandler(calendar, KeyboardInputProviderAcquireFocus);
                    break;
                default:
                    throw new TestValidationException("Unknown RoutingStrategy");
            }

            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            // Perform action to raise the event
            calendar.Focus();
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            // Verify the event is fired
            Assert.IsTrue(isEventFired, "Event not fired.");

            // Remove event
            switch (routingStrategy)
            {
                case RoutingStrategy.Tunnel:
                    Keyboard.RemovePreviewKeyboardInputProviderAcquireFocusHandler(calendar, KeyboardInputProviderAcquireFocus);
                    break;
                case RoutingStrategy.Bubble:
                    Keyboard.RemoveKeyboardInputProviderAcquireFocusHandler(calendar, KeyboardInputProviderAcquireFocus);
                    break;
                default:
                    throw new TestValidationException("Unknown RoutingStrategy");
            }

            // Test event callback is not fired when there is no event attached
            Button button = new Button();
            button.Content = "Button";
            stackpanel.Children.Add(button);

            // Lose calendar focus and set isEventFired to false
            button.Focus();
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            isEventFired = false;

            // Perform the action again
            calendar.Focus();
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            // Verify the event is fired
            Assert.IsFalse(isEventFired, "Event fired.");

            ResetTest();
        }

        private void KeyboardInputProviderAcquireFocus(object sender, KeyboardInputProviderAcquireFocusEventArgs e)
        {
            isEventFired = true;
        }

        public TestResult KeyboardInputProviderAcquireFocusEvent()
        {
            TestKeyboardInputProviderAcquireFocusEvent(RoutingStrategy.Tunnel);
            TestKeyboardInputProviderAcquireFocusEvent(RoutingStrategy.Bubble);

            return TestResult.Pass;
        }

        /// <summary>
        /// Verify correct SelectedDatesChangedEventArgs when in CalendarSelectionMode.SingleDate
        /// </summary>
        public TestResult SelectedDatesChangedEventArgs_SingleDate()
        {
            int added = 0;
            int removed = 0;

            Calendar calendar = new Calendar();
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            calendar.SelectionMode = CalendarSelectionMode.SingleDate;
            calendar.SelectedDatesChanged += (sender, e) =>
            {
                added = e.AddedItems.Count;
                removed = e.RemovedItems.Count;
            };

            DateTime value = new DateTime(2000, 10, 10);
            calendar.SelectedDate = value;

            DispatcherHelper.DoEvents();

            Assert.IsTrue(added == 1, "Added items should be equal to 1 with SelectionMode.SingleDate");
            Assert.IsTrue(removed == 0, "Removed items should be equal to 0 with SelectionMode.SingleDate");

            value = new DateTime(1979, 09, 02);
            calendar.SelectedDate = value;

            DispatcherHelper.DoEvents();

            Assert.IsTrue(added == 1, "Added items should be equal to 1 with SelectionMode.SingleDate");
            Assert.IsTrue(removed == 1, "Removed items should be equal to 1 with SelectionMode.SingleDate");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify correct SelectedDatesChangedEventArgs when in CalendarSelectionMode.SingelRange
        /// </summary>
        public TestResult SelectedDatesChangedEventArgs_SingleRange()
        {
            int added = 0;
            int removed = 0;

            Calendar calendar = new Calendar();
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            calendar.SelectionMode = CalendarSelectionMode.SingleRange;
            calendar.SelectedDatesChanged += (sender, e) =>
            {
                added = e.AddedItems.Count;
                removed = e.RemovedItems.Count;
            };

            // Add 2 days
            DateTime start = new DateTime(2000, 10, 10);
            DateTime end = new DateTime(2000, 10, 12);
            calendar.SelectedDates.AddRange(start, end);

            DispatcherHelper.DoEvents();

            Assert.IsTrue(added == 3, "Added items should be equal to 3 with SelectionMode.SingleRange");
            Assert.IsTrue(removed == 0, "Removed items should be equal to 0 with SelectionMode.SingleRange");

            // Add 366 days -- includes 1980 Feb, 29 leap year
            start = new DateTime(1979, 09, 02);
            end = new DateTime(1980, 09, 01);
            calendar.SelectedDates.AddRange(start, end);

            DispatcherHelper.DoEvents();

            Assert.IsTrue(added == 366, "Added items should be equal to 366 with SelectionMode.SingleRange");
            Assert.IsTrue(removed == 3, "Removed items should be equal to 3 with SelectionMode.SingleRange");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify correct SelectedDatesChangedEventArgs when in CalendarSelectionMode.MultipleRange
        /// </summary>
        public TestResult SelectedDatesChangedEventArgs_MultipleRange()
        {
            int added = 0;
            int removed = 0;

            Calendar calendar = new Calendar();
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            calendar.SelectionMode = CalendarSelectionMode.MultipleRange;
            calendar.SelectedDatesChanged += (sender, e) =>
            {
                added = e.AddedItems.Count;
                removed = e.RemovedItems.Count;
            };

            // Add 2 days
            DateTime start = new DateTime(2000, 10, 10);
            DateTime end = new DateTime(2000, 10, 12);
            calendar.SelectedDates.AddRange(start, end);

            DispatcherHelper.DoEvents();

            Assert.IsTrue(added == 3, "Added items should be equal to 3 with SelectionMode.MultipleRange");
            Assert.IsTrue(removed == 0, "Removed items should be equal to 0 with SelectionMode.MultipleRange");

            // Add 366 days -- includes 1980 Feb, 29 leap year
            start = new DateTime(1979, 09, 02);
            end = new DateTime(1980, 09, 01);
            calendar.SelectedDates.AddRange(start, end);

            DispatcherHelper.DoEvents();

            Assert.IsTrue(added == 366, "Added items should be equal to 366 with SelectionMode.MultipleRange");
            Assert.IsTrue(removed == 0, "Removed items should be equal to 0 with SelectionMode.MultipleRange");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify selection is not possible is None Mode.
        /// </summary>
        public TestResult SelectDayNoneInvalidOperationException()
        {
            Calendar calendar = new Calendar();
            calendar.SelectionMode = CalendarSelectionMode.None;
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            Assert.IsExceptionThrown(typeof(InvalidOperationException), () =>
            {
                calendar.SelectedDate = new DateTime(2000, 2, 2);
            });

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// InvalidOperationException when adding to SelectedDates when CalendarSelectionMode.None
        /// </summary>
        public TestResult SelectedDatesCollectionException_None()
        {
            Calendar calendar = new Calendar();
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            calendar.SelectionMode = CalendarSelectionMode.None;

            DateTime value = new DateTime(2000, 10, 10);

            Assert.IsExceptionThrown(typeof(InvalidOperationException), () =>
            {
                calendar.SelectedDates.AddRange(value, value);
            });

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// InvalidOperationException should be thrown when adding to SelectedDates when CalendarSelectionMode.SingleDate
        /// </summary>
        public TestResult SelectedDatesCollectionException_SingleDate()
        {
            Calendar calendar = new Calendar();
            calendar.SelectionMode = CalendarSelectionMode.SingleDate;
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            DateTime start = new DateTime(2000, 10, 10);
            DateTime end = new DateTime(2000, 10, 12);

            Assert.IsExceptionThrown(typeof(InvalidOperationException), () =>
            {
                calendar.SelectedDates.AddRange(start, end);
            });

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify SelectedDate/Dates behavior with SelectionMode.SingleDate
        /// </summary>
        public TestResult SelectedDateSingle()
        {
            int added = 0;
            int removed = 0;
            int events = 0;

            Calendar calendar = new Calendar();
            calendar.SelectedDatesChanged += (sender, e) =>
            {
                added = e.AddedItems.Count;
                removed = e.RemovedItems.Count;
                events++;
            };
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            calendar.SelectionMode = CalendarSelectionMode.SingleDate;
            calendar.SelectedDate = DateTime.Today;
            DispatcherHelper.DoEvents();

            Assert.AreEqual(calendar.SelectedDate.Value, DateTime.Today);
            Assert.IsTrue(calendar.SelectedDates.Count == 1, "SelectedDates.Count should be equal to 1");
            Assert.AreEqual(calendar.SelectedDates[0], DateTime.Today);

            Assert.IsTrue(events == 1, "Event count should be 1");
            Assert.IsTrue(added == 1, "Added days should be 1");
            Assert.IsTrue(removed == 0, "Removed days should b 0");

            added = removed = events = 0;
            calendar.SelectedDate = DateTime.Today;
            DispatcherHelper.DoEvents();

            Assert.AreEqual(calendar.SelectedDate.Value, DateTime.Today);
            Assert.IsTrue(calendar.SelectedDates.Count == 1, "SelectedDates.Count should be equal to 1");
            Assert.AreEqual(calendar.SelectedDates[0], DateTime.Today);
            Assert.IsTrue(events == 0, "Event count should be 0");

            calendar.SelectionMode = CalendarSelectionMode.None;
            DispatcherHelper.DoEvents();

            Assert.IsTrue(calendar.SelectedDates.Count == 0, "SelectedDates.Count should 0 after changeing SelectionMode");
            Assert.IsNull(calendar.SelectedDate, "SelectedDate should be null after changeing SelectionMode");

            added = removed = events = 0;
            calendar.SelectionMode = CalendarSelectionMode.SingleDate;
            calendar.SelectedDates.Add(DateTime.Today.AddDays(1));
            DispatcherHelper.DoEvents();

            Assert.AreEqual(calendar.SelectedDate.Value, DateTime.Today.AddDays(1));
            Assert.IsTrue(calendar.SelectedDates.Count == 1, "SelectedDates.Count should be equal to 1");
            Assert.AreEqual(calendar.SelectedDates[0], DateTime.Today.AddDays(1));
            Assert.IsTrue(events == 1, "Event count should be 1");
            Assert.IsTrue(added == 1, "Added days should be 1");
            Assert.IsTrue(removed == 0, "Removed days should be 0");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify SelectedDate/Dates behavior with SelectionMode.SingleRange
        /// </summary>
        public TestResult SelectedDateSingleRange()
        {
            int added = 0;
            int removed = 0;
            int events = 0;

            Calendar calendar = new Calendar();
            calendar.SelectedDatesChanged += (sender, e) =>
            {
                added = e.AddedItems.Count;
                removed = e.RemovedItems.Count;
                events++;
            }; 
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            calendar.SelectionMode = CalendarSelectionMode.SingleRange;
            calendar.SelectedDate = DateTime.Today;
            DispatcherHelper.DoEvents();

            Assert.AreEqual(calendar.SelectedDate.Value, DateTime.Today);
            Assert.IsTrue(calendar.SelectedDates.Count == 1, "SelectedDates.Count should be 1");
            Assert.AreEqual(calendar.SelectedDates[0], DateTime.Today);
            Assert.IsTrue(events == 1, "Event count should be 1");
            Assert.IsTrue(added== 1, "Added days should be 1");
            Assert.IsTrue(removed== 0, "Removed days should 0");

            added = removed = events = 0;
            calendar.SelectedDates.Clear(); 
            DispatcherHelper.DoEvents();

            Assert.IsNull(calendar.SelectedDate, "SelectedDate should be null");
            Assert.IsTrue(removed == 1, "Removed days should 1");

            added = removed = events = 0;
            calendar.SelectedDates.AddRange(DateTime.Today, DateTime.Today.AddDays(10));
            DispatcherHelper.DoEvents();

            Assert.AreEqual(calendar.SelectedDate.Value, DateTime.Today);
            Assert.IsTrue(calendar.SelectedDates.Count == 11, "SelectedDates.Count should be 11");
            Assert.AreEqual(calendar.SelectedDates[0], DateTime.Today);
            Assert.IsTrue(events == 1, "Event count should be 1");
            Assert.IsTrue(added == 11, "Added days should be 11");
            Assert.IsTrue(removed == 0, "Removed days should 0");

            added = removed = events = 0;
            calendar.SelectedDates.AddRange(DateTime.Today, DateTime.Today.AddDays(10));
            DispatcherHelper.DoEvents();

            Assert.IsTrue(calendar.SelectedDates.Count == 11,"SelectedDate.Count should be 11");
            Assert.IsTrue(events == 0, "Event count should be 0");

            added = removed = events = 0;
            calendar.SelectedDates.AddRange(DateTime.Today.AddDays(-20), DateTime.Today);
            DispatcherHelper.DoEvents();
            
            Assert.AreEqual(calendar.SelectedDate.Value, DateTime.Today.AddDays(-20));
            Assert.IsTrue(calendar.SelectedDates.Count == 21, "SelectedDates.Count should be 21");
            Assert.AreEqual(calendar.SelectedDates[0], DateTime.Today.AddDays(-20));
            Assert.IsTrue(events == 1, "Event count should be 1");
            Assert.IsTrue(added == 21, "Added days should be 21");
            Assert.IsTrue(removed == 11, "Removed days should 11");

            added = removed = events = 0;
            calendar.SelectedDates.Add(DateTime.Today.AddDays(100));
            DispatcherHelper.DoEvents();
            
            Assert.AreEqual(calendar.SelectedDate.Value, DateTime.Today.AddDays(100));
            Assert.IsTrue(calendar.SelectedDates.Count == 1, "SelectedDates.Count should be 1");
            Assert.AreEqual(calendar.SelectedDates[0], DateTime.Today.AddDays(100));
            Assert.IsTrue(events == 1, "Event count should be 1");
            Assert.IsTrue(added == 1, "Added days should be 1");
            Assert.IsTrue(removed == 21, "Removed days should 21");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify SelectedDate/Dates behavior with SelectionMode.MultipleRange
        /// </summary>
        public TestResult SelectedDateMultipleRange()
        {
            Calendar calendar = new Calendar();
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            calendar.SelectionMode = CalendarSelectionMode.MultipleRange;
            calendar.SelectedDate = DateTime.Today;
            DispatcherHelper.DoEvents();

            Assert.AreEqual(calendar.SelectedDate.Value, DateTime.Today);
            Assert.IsTrue(calendar.SelectedDates.Count == 1, "SelectedDates.Count should be 1");
            Assert.AreEqual(calendar.SelectedDates[0], DateTime.Today);

            calendar.SelectedDates.Clear();
            DispatcherHelper.DoEvents();

            Assert.IsNull(calendar.SelectedDate,"SelectedDate should be null");

            calendar.SelectedDates.AddRange(DateTime.Today, DateTime.Today.AddDays(10));
            DispatcherHelper.DoEvents();

            Assert.AreEqual(calendar.SelectedDate.Value, DateTime.Today);
            Assert.IsTrue(calendar.SelectedDates.Count == 11, "SelectedDates.Count should be 11");

            calendar.SelectedDates.Add(DateTime.Today);
            DispatcherHelper.DoEvents();
            Assert.IsTrue(calendar.SelectedDates.Count == 11,"SelectedDates.Count should 11");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Check if an exception is thrown if SelectedDates are updated in None Selection Mode
        /// </summary>
        public TestResult SelectedatesAddEmpty()
        {
            Calendar calendar = new Calendar();
            calendar.SelectionMode = CalendarSelectionMode.None;
            Assert.IsExceptionThrown(typeof(InvalidOperationException), () => 
            { 
                calendar.SelectedDates.Add(new DateTime()); 
            });

            return TestResult.Pass;
        }
    }
}
