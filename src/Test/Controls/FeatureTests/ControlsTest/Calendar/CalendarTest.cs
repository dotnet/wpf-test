using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
using Glob = System.Globalization;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Base Class for all Calendar Tests
    /// </summary>
    [TestDefaults(DefaultTimeout = 180)]
    public abstract class CalendarTest : StepsTest
    {
        #region Constructor

        public CalendarTest()
            : base()
        {
            this.InitializeSteps += Setup;
            this.CleanUpSteps += Cleanup;
        }

        protected bool isEventFired = false;

        #endregion Constructor

        public virtual TestResult Setup()
        {
            this.window = new Window();
            this.window.Title = string.Format("Calendar Test : {0}", this.GetType().Name);
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Width = 600;
            this.window.Height = 400;
            this.window.Content = this.TestUI;
            this.window.Show();

            DispatcherHelper.DoEvents();

            return TestResult.Pass;
        }

        public virtual TestResult Cleanup()
        {
            if (this.Window != null)
                this.Window.Close();
            return TestResult.Pass;
        }

        public Window Window
        {
            get { return window; }
        }
   
        public Grid TestUI
        {
            get
            {
                if (testui == null)
                    testui = new Grid();
                return testui;
            }
        }

        public AutomationElement CalendarAE
        {
            get
            {
                if (calendarAE == null)
                {
                    AutomationHelper.DoUIAAction(DispatcherPriority.Background, new DispatcherOperationCallback(FindCalendarAE), new object());
                }
                return calendarAE;
            }
        }

        public void ResetTest()
        {
            this.TestUI.Children.Clear();
            isLoaded = false;
            isEventFired = false;
        }

        public FrameworkElement LoadXaml(string xaml)
        {
            return ResourceHelper.LoadXamlResource(string.Format("calendar/xaml/{0}", xaml));
        }

        private object FindCalendarAE(object arg)
        {
            AutomationElement windowAE = AutomationHelper.GetRootAutomationElement();
            PropertyCondition condition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Calendar);
            calendarAE = windowAE.FindFirst(TreeScope.Descendants, condition);
            return null;
        }

        public Calendar FindCalendar(string name)
        {
            DependencyObject calendar = System.Windows.LogicalTreeHelper.FindLogicalNode(this.window, name);
            if (calendar is Calendar)
                return calendar as Calendar;
            else
                return null;
        }

        public void Calendar_Loaded(object sender, RoutedEventArgs e)
        {
            isLoaded = true;
        }

        public bool isLoaded = false;

        private Grid testui = null;
        private Window window = null;
        private AutomationElement calendarAE = null;

        public static IFormatProvider GetGregorianFormatProvider(Glob.CultureInfo culture)
        {
            if (culture.Calendar is Glob.GregorianCalendar)
            {
                return culture;
            }

            Glob.DateTimeFormatInfo info = (Glob.DateTimeFormatInfo)culture.DateTimeFormat.Clone();
            if (culture.OptionalCalendars != null)
            {
                foreach (Glob.Calendar calendar in culture.OptionalCalendars)
                {
                    if (calendar is Glob.GregorianCalendar)
                    {
                        info.Calendar = calendar;
                        break;
                    }
                }
            }

            if ((info.Calendar as Glob.GregorianCalendar) == null)
            {
                info.Calendar = new Glob.GregorianCalendar();
            }

            return info;
        }
    }
}
