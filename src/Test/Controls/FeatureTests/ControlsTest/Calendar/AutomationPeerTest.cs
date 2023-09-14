using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Assert = Microsoft.Test.Controls.TestAsserts;
using Microsoft.Test.Display;
using Glob = System.Globalization;

namespace Microsoft.Test.Controls
{
    [Test(0, "Calendar", TestCaseSecurityLevel.FullTrust, "AutomationPeerTest", Keywords = "Localization_Suite")]
    public class CalendarAutomationPeerTest : CalendarTest
    {
        public CalendarAutomationPeerTest()
            : base()
        {
            this.RunSteps += AutomationPeerTest;
            this.RunSteps += DayButtonAutomationPeerTest;
            this.RunSteps += CalendarButtonAutomationPeerTest;
            this.RunSteps += DayButtonGetName;
            this.RunSteps += CalendarButtonPatterns;
            this.RunSteps += CalendarButtonGetClassName;
        }

        /// <summary>
        /// Tests the creation of an automation peer for the Calendar
        /// </summary>
        public TestResult AutomationPeerTest()
        {
            DateTime date = new DateTime(2003, 2, 2);

            Calendar calendar = new Calendar();
            calendar.Loaded += new RoutedEventHandler(Calendar_Loaded);
            calendar.Height = 200;
            calendar.Width = 200;
            calendar.DisplayDate = date;
            calendar.SelectedDate = date;

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();
            Assert.IsTrue(isLoaded, "Calendar never fired loaded event.");
            Assert.IsNotNull(calendar, "Calendar should not be null");

            Assert.IsNotNull(this.CalendarAE, "Calendar Automation Element should not be null");

            CalendarAutomationPeer peer = ((CalendarAutomationPeer)CalendarAutomationPeer.CreatePeerForElement(calendar));
            Assert.IsNotNull(peer, "AutomationPeer should not null");

            TestPeer testPeer = new TestPeer(calendar);
            Assert.IsNotNull(testPeer, "TestPeer should not be null");

            Assert.AreEqual(peer.GetBoundingRectangle().Height, Monitor.ConvertLogicalToScreen(Dimension.Height, calendar.ActualHeight), "Incorrect BoundingRectangle.Height value");
            Assert.AreEqual(peer.GetBoundingRectangle().Width, Monitor.ConvertLogicalToScreen(Dimension.Width, calendar.ActualWidth), "Incorrect BoundingRectangle.Width value");
            Assert.AreEqual(peer.GetAutomationControlType(), AutomationControlType.Calendar, "Incorrect Control type for calendar");

            Assert.AreEqual(peer.GetClassName(), calendar.GetType().Name, "Incorrect ClassName value for Calendar");
            Assert.IsTrue(peer.IsContentElement(), "Incorrect IsContentElement value");
            Assert.IsTrue(peer.IsControlElement(), "Incorrect IsControlElement value");

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            IGridProvider calendarGridProvider = ((IGridProvider)peer.GetPattern(PatternInterface.Grid));
            Assert.IsNotNull(calendarGridProvider, "Incorrect calendarGridProvider value");
            Assert.AreEqual(tcalendar.Month.UI.RowDefinitions.Count - 1, calendarGridProvider.RowCount, "Incorrect RowCount value");
            Assert.AreEqual(tcalendar.Month.UI.ColumnDefinitions.Count, calendarGridProvider.ColumnCount, "Incorrect ColumnCount value");

            IRawElementProviderSimple cell = calendarGridProvider.GetItem(0, 3);
            Assert.IsNotNull(cell, "GetItem returned null for valid cell");
            AutomationPeer cellPeer = testPeer.GetPeerFromProvider(cell);
            Assert.AreEqual(typeof(CalendarDayButton).Name, cellPeer.GetClassName(), "GetItem did not return DayButton");

            calendar.DisplayMode = CalendarMode.Year;
            tcalendar = new TemplatedCalendar(calendar);
            DispatcherHelper.DoEvents();

            Assert.AreEqual(tcalendar.Year.UI.RowDefinitions.Count, calendarGridProvider.RowCount, "Incorrect RowCount value");
            Assert.AreEqual(tcalendar.Year.UI.ColumnDefinitions.Count, calendarGridProvider.ColumnCount, "Incorrect ColumnCount value");

            cell = calendarGridProvider.GetItem(2, 3);
            Assert.IsNotNull(cell, "GetItem returned null for valid cell");
            cellPeer = testPeer.GetPeerFromProvider(cell);

            Assert.AreEqual(typeof(CalendarButton).Name, cellPeer.GetClassName(), "GetItem did not return CalendarButton");

            calendar.DisplayMode = CalendarMode.Decade;
            tcalendar = new TemplatedCalendar(calendar);
            DispatcherHelper.DoEvents();

            Assert.AreEqual(tcalendar.Year.UI.RowDefinitions.Count, calendarGridProvider.RowCount, "Incorrect RowCount value");
            Assert.AreEqual(tcalendar.Year.UI.ColumnDefinitions.Count, calendarGridProvider.ColumnCount, "Incorrect ColumnCount value");

            cell = calendarGridProvider.GetItem(2, 3);
            Assert.IsNotNull(cell, "GetItem returned null for valid cell");
            cellPeer = testPeer.GetPeerFromProvider(cell);
            Assert.AreEqual(typeof(CalendarButton).Name, cellPeer.GetClassName(), "GetItem did not return CalendarButton");

            cell = calendarGridProvider.GetItem(10, 10);
            Assert.IsNull(cell, "GetItem returned object for invalid cell");

            calendar.Focus();
            DispatcherHelper.DoEvents();

            IMultipleViewProvider calendarMultiViewProvider = ((IMultipleViewProvider)peer.GetPattern(PatternInterface.MultipleView));
            Assert.IsNotNull(calendarMultiViewProvider, "Calendar MultiViewProvider should not be null");
            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Decade);
            Assert.AreEqual(calendarMultiViewProvider.CurrentView, (int)CalendarMode.Decade);
            Assert.AreEqual(CalendarMode.Decade.ToString(), calendarMultiViewProvider.GetViewName(calendarMultiViewProvider.CurrentView));

            calendarMultiViewProvider.SetCurrentView((int)CalendarMode.Year);
            DispatcherHelper.DoEvents();

            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Year);
            Assert.AreEqual(calendarMultiViewProvider.CurrentView, (int)CalendarMode.Year);
            Assert.AreEqual(CalendarMode.Year.ToString(), calendarMultiViewProvider.GetViewName(calendarMultiViewProvider.CurrentView));

            ISelectionProvider calendarSelectionProvider = ((ISelectionProvider)peer.GetPattern(PatternInterface.Selection));
            Assert.IsNotNull(calendarSelectionProvider, "Calendar SelectionProvider should not be null");
            Assert.IsFalse(calendarSelectionProvider.IsSelectionRequired, "Incorrect IsSelectionRequired value");
            Assert.IsFalse(calendarSelectionProvider.CanSelectMultiple, "Incorrect CanSelectMultiple value");

            calendar.SelectionMode = CalendarSelectionMode.MultipleRange;
            DispatcherHelper.DoEvents();

            Assert.IsNull(calendar.SelectedDate, "SelectedDate should be null");
            Assert.IsTrue(calendarSelectionProvider.CanSelectMultiple, "Incorrect CanSelectMultiple value");
            calendar.SelectedDates.AddRange(new DateTime(2003, 2, 10), new DateTime(2003, 3, 30));
            DispatcherHelper.DoEvents();

            calendar.DisplayMode = CalendarMode.Month;
            DispatcherHelper.DoEvents();

            IRawElementProviderSimple[] selection = calendarSelectionProvider.GetSelection();
            Assert.IsNotNull(selection, "GetSelection returned null for valid selection");
            Assert.AreEqual(selection.Length, 49, "GetSelection returned wrong number of selections");

            cellPeer = testPeer.GetPeerFromProvider(selection[0]);
            Assert.AreEqual(cellPeer.GetClassName(), typeof(CalendarDayButton).Name, "Incorrect name for CalendarDayButton");

            ITableProvider calendarTableProvider = ((ITableProvider)peer.GetPattern(PatternInterface.Table));
            Assert.IsNotNull(calendarTableProvider, "Calendar TableProvider should not be null");
            Assert.IsTrue(Enum.Equals(calendarTableProvider.RowOrColumnMajor, RowOrColumnMajor.RowMajor), "Incorrect RowOrColumnMajor value");

            IRawElementProviderSimple[] headers = calendarTableProvider.GetRowHeaders();
            Assert.IsNull(headers, "GetRowHeaders should return null");

            headers = calendarTableProvider.GetColumnHeaders();
            Assert.IsNotNull(headers, "GetColumnHeaders returned null");
            Assert.AreEqual(headers.Length, 7, "Incorrect number of column headers");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Tests the creation of an automation peer for the DayButton
        /// </summary>
        public TestResult DayButtonAutomationPeerTest()
        {
            DateTime date = new DateTime(2000, 2, 2);

            Calendar calendar = new Calendar();
            calendar.Loaded += new RoutedEventHandler(Calendar_Loaded);
            calendar.Height = 200;
            calendar.Width = 200;
            calendar.DisplayDate = date;
            calendar.SelectedDate = date;

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            Assert.IsTrue(isLoaded, "Calendar never fired loaded event.");

            Assert.IsNotNull(this.CalendarAE, "Calendar Automation Element should not be null");

            CalendarAutomationPeer calendarAutomationPeer = (CalendarAutomationPeer)CalendarAutomationPeer.CreatePeerForElement(calendar);
            Assert.IsNotNull(calendarAutomationPeer, "Calendar AutomationPeer should not be null");
            TestPeer testPeer = new TestPeer(calendar);

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);

            CalendarDayButton dayButton = tcalendar.CalendarDayButtonByDate(date);
            Assert.IsNotNull(dayButton, "DayButton should not be null");
            AutomationPeer peer = FrameworkElementAutomationPeer.CreatePeerForElement(dayButton);
            Assert.IsNotNull(peer, "AutomationPeer for DayButton should not be null");

            Assert.AreEqual(peer.GetAutomationControlType(), AutomationControlType.Button, "Incorrect Control type for Daybutton");
            Assert.AreEqual(peer.GetClassName(), dayButton.GetType().Name, "Incorrect ClassName value for DayButton");

            Assert.IsTrue(peer.IsContentElement(), "Incorrect IsContentElement value");
            Assert.IsTrue(peer.IsControlElement(), "Incorrect IsControlElement value");
            Assert.IsTrue(peer.IsKeyboardFocusable(), "Incorrect IsKeyBoardFocusable value");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        /// Tests the creation of an automation peer for the DayButton
        /// </summary>
        public TestResult CalendarButtonAutomationPeerTest()
        {
            Calendar calendar = new Calendar();

            DateTime date = new DateTime(2000, 2, 2);
            calendar.DisplayDate = date;
            calendar.SelectedDate = date;

            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            Assert.IsNotNull(this.CalendarAE, "Calendar Automation Element should not be null");

            CalendarAutomationPeer calendarAutomationPeer = (CalendarAutomationPeer)CalendarAutomationPeer.CreatePeerForElement(calendar);
            Assert.IsNotNull(calendarAutomationPeer, "Calendar Automation Peer is null.");

            calendar.DisplayMode = CalendarMode.Year;
            DispatcherHelper.DoEvents();

            TemplatedCalendar tcalendar = new TemplatedCalendar(calendar);
            TestPeer testPeer = new TestPeer(calendar);

            CalendarButton calendarButton = (CalendarButton)tcalendar.Year.UI.Children[1];
            Assert.IsNotNull(calendarButton, "CalendarButton [1] is null");
            AutomationPeer peer = FrameworkElementAutomationPeer.CreatePeerForElement(calendarButton);
            Assert.IsNotNull(peer, "AutomationPeer for CalendarButton is null");

            date = new DateTime(2000, 2, 1);

            Assert.AreEqual(peer.GetAutomationControlType(), AutomationControlType.Button, "Incorrect Control type for CalendarButton");
            Assert.AreEqual(peer.GetClassName(), calendarButton.GetType().Name, "Incorrect ClassName value for CalendarButton");

            Assert.IsTrue(peer.IsControlElement(), "Incorrect IsControlElement value");
            Assert.IsTrue(peer.IsKeyboardFocusable(), "Incorrect IsKeyBoardFocusable value");

            foreach (CalendarButton child in tcalendar.Year.Items)
            {
                int childCol = (int)child.GetValue(Grid.ColumnProperty);
                int childRow = (int)child.GetValue(Grid.RowProperty);
                IGridProvider gridprovider = ((IGridProvider)calendarAutomationPeer.GetPattern(PatternInterface.Grid));
                IRawElementProviderSimple item = gridprovider.GetItem(childRow, childCol);
                Assert.IsNotNull(item, string.Format("Item at {0},{1} is null", childCol, childRow));
            }

            Assert.AreEqual(calendar.DisplayMode, CalendarMode.Year);

            calendarButton = (CalendarButton)tcalendar.Year.Items[1];
            peer = CalendarAutomationPeer.CreatePeerForElement(calendarButton);

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        public TestResult DayButtonGetName()
        {
            DateTime date = new DateTime(1979, 9, 2);
            Calendar calendar = new Calendar();
            calendar.DisplayDate = date;
            calendar.Loaded += new RoutedEventHandler(Calendar_Loaded);
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            Assert.IsTrue(isLoaded, "Calendar never fired loaded event.");

            TemplatedCalendar t = new TemplatedCalendar(calendar);
            AutomationPeer peer = CalendarAutomationPeer.CreatePeerForElement(t.CalendarDayButtonByDay(2));
            Assert.IsNotNull(peer, "AutomationPeer for calendar button should not be null");

            Assert.AreEqual("2", peer.GetName());

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        public TestResult CalendarButtonPatterns()
        {
            Calendar calendar = new Calendar();
            calendar.DisplayMode = CalendarMode.Year;
            calendar.Loaded += new RoutedEventHandler(Calendar_Loaded);
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            Assert.IsTrue(isLoaded, "Calendar never fired loaded event.");

            TemplatedCalendar t = new TemplatedCalendar(calendar);

            AutomationPeer peer = CalendarButtonAutomationPeer.CreatePeerForElement(t.CalendarButtonWithSelectedDays());
            Assert.IsNotNull(peer, "AutomationPeer for calendar button should not be null");

            ResetTest();
            return TestResult.Pass;
        }

        /// <summary>
        public TestResult CalendarButtonGetClassName()
        {
            Calendar calendar = new Calendar();
            calendar.DisplayMode = CalendarMode.Year;
            calendar.Loaded += new RoutedEventHandler(Calendar_Loaded);
            this.TestUI.Children.Add(calendar);
            DispatcherHelper.DoEvents();

            Assert.IsTrue(isLoaded, "Calendar never fired loaded event.");

            TemplatedCalendar t = new TemplatedCalendar(calendar);

            AutomationPeer peer = CalendarButtonAutomationPeer.CreatePeerForElement(t.CalendarButtonWithSelectedDays());
            Assert.IsNotNull(peer, "AutomationPeer for calendar button should not be null");
            Assert.AreEqual(peer.GetClassName(), "CalendarButton");

            ResetTest();
            return TestResult.Pass;
        }
    }

    public class TestPeer : FrameworkElementAutomationPeer
    {
        public TestPeer(UIElement element)
            : base(element as FrameworkElement)
        {
        }

        public AutomationPeer GetPeerFromProvider(IRawElementProviderSimple provider)
        {
            return PeerFromProvider(provider);
        }
    }
}
