using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Assert = Microsoft.Test.Controls.TestAsserts;
using Microsoft.Test.Display;
using Glob = System.Globalization;

namespace Microsoft.Test.Controls
{
    [Test(0, "DatePicker", TestCaseSecurityLevel.FullTrust, "AutomationPeerTest")]
    public class DatePickerAutomationPeerTest : DatePickerTest
    {
        public DatePickerAutomationPeerTest()
            : base()
        {
            this.RunSteps += AutomationPeerTest;
        }

        /// <summary>
        /// Tests the creation of an automation peer for the DatePicker
        /// </summary>
        public TestResult AutomationPeerTest()
        {
            DateTime date = new DateTime(2000, 2, 2); 
            
            DatePicker datepicker = new DatePicker();
            datepicker.SelectedDate = date;
            datepicker.Loaded += new RoutedEventHandler(DatePicker_Loaded);

            this.TestUI.Children.Add(datepicker);
            DispatcherHelper.DoEvents();
            TemplatedDatePicker tdatepicker = new TemplatedDatePicker(datepicker);

            Assert.IsTrue(isLoaded, "DatePicker never fired loaded event.");
            Assert.IsNotNull(this.DatePickerAE, "DatePicker Automation Element should not be null");

            DatePickerAutomationPeer datePickerAutomationPeer = (DatePickerAutomationPeer)DatePickerAutomationPeer.CreatePeerForElement(datepicker);
            Assert.IsNotNull(datePickerAutomationPeer, "DatePickerAutomationPeer should not be null");
            
            Assert.AreEqual(datePickerAutomationPeer.GetBoundingRectangle().Height, Monitor.ConvertLogicalToScreen(Dimension.Height, datepicker.ActualHeight), "Incorrect BoundingRectangle.Height value");
            Assert.AreEqual(datePickerAutomationPeer.GetBoundingRectangle().Width, Monitor.ConvertLogicalToScreen(Dimension.Width, datepicker.ActualWidth), "Incorrect BoundingRectangle.Width value");
            Assert.AreEqual(datePickerAutomationPeer.GetAutomationControlType(), AutomationControlType.Custom, "Incorrect Control type for datepicker");
            Assert.AreEqual(datePickerAutomationPeer.GetClassName(), datepicker.GetType().Name, "Incorrect ClassName value for datepicker");
            Assert.IsTrue(datePickerAutomationPeer.IsContentElement(), "Incorrect IsContentElement value");
            Assert.IsTrue(datePickerAutomationPeer.IsControlElement(), "Incorrect IsControlElement value");
            Assert.IsTrue(datePickerAutomationPeer.IsKeyboardFocusable() == (bool) datepicker.GetValue(UIElement.FocusableProperty), "Incorrect IsKeyBoardFocusable value");

            IExpandCollapseProvider datePickerExpandCollapseProvider = ((IExpandCollapseProvider)datePickerAutomationPeer.GetPattern(PatternInterface.ExpandCollapse));
            Assert.IsNotNull(datePickerExpandCollapseProvider, "DatePickerExpandCollapseProvider should not be null");
            Assert.AreEqual((object)datePickerExpandCollapseProvider.ExpandCollapseState, (object)ExpandCollapseState.Collapsed);

            datepicker.IsDropDownOpen = true;
            DispatcherHelper.DoEvents();

            Assert.IsTrue(tdatepicker.Popup.IsOpen,"Popup.IsOpen should be true");
            Assert.AreEqual(datePickerExpandCollapseProvider.ExpandCollapseState, ExpandCollapseState.Expanded);

            datePickerExpandCollapseProvider.Collapse();
            DispatcherHelper.DoEvents();

            Assert.IsFalse(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be false");
            Assert.AreEqual(datePickerExpandCollapseProvider.ExpandCollapseState, ExpandCollapseState.Collapsed);

            datePickerExpandCollapseProvider.Expand();
            DispatcherHelper.DoEvents();

            Assert.IsTrue(tdatepicker.Popup.IsOpen, "Popup.IsOpen should be true");
            Assert.AreEqual(datePickerExpandCollapseProvider.ExpandCollapseState, ExpandCollapseState.Expanded);

            datePickerExpandCollapseProvider.Collapse();

            IValueProvider datePickerValueProvider = ((IValueProvider)datePickerAutomationPeer.GetPattern(PatternInterface.Value));

            Assert.IsNotNull(datePickerValueProvider, "DatePickerValueProvider should not be null");
            Assert.IsFalse(datePickerValueProvider.IsReadOnly, "DatePickerValueProvider.IsReadOnly should be false");
            Assert.AreEqual(datePickerValueProvider.Value, date.ToString(CalendarTest.GetGregorianFormatProvider(Glob.CultureInfo.CurrentCulture)));

            datepicker.SelectedDate = null;
            DispatcherHelper.DoEvents();

            Assert.AreEqual(datePickerValueProvider.Value, string.Empty);

            DateTime date2 = new DateTime(2000, 5, 5);
            datePickerValueProvider.SetValue(date2.ToString(CalendarTest.GetGregorianFormatProvider(Glob.CultureInfo.CurrentCulture)));
            DispatcherHelper.DoEvents();

            Assert.AreEqual(datepicker.SelectedDate, date2);
            Assert.AreEqual(datePickerValueProvider.Value, date2.ToString(CalendarTest.GetGregorianFormatProvider(Glob.CultureInfo.CurrentCulture)));
            Assert.AreEqual(datepicker.Text, date2.ToString("d", CalendarTest.GetGregorianFormatProvider(Glob.CultureInfo.CurrentCulture)));

            ResetTest();
            return TestResult.Pass;
        }
    }
}
