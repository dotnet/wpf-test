using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    public static class AutomationHelper
    {
        public static AutomationElement GetRootAutomationElement()
        {
            AutomationElement window = AutomationElement.FromHandle(Process.GetCurrentProcess().MainWindowHandle);
            return window;
        }

        public static void DoUIAAction(DispatcherPriority priority, Delegate callback, object arg)
        {
            if (priority == DispatcherPriority.Inactive)
            {
                throw new ArgumentOutOfRangeException("priority", priority, string.Empty);
            }

            DispatcherFrame frame = new DispatcherFrame(true);

            DispatcherOperation op = UIADispatcher.BeginInvoke(DispatcherPriority.Inactive, callback, arg);

            op.Completed += delegate(object sender, EventArgs e)
            {
                frame.Continue = false;
            };

            op.Aborted += delegate(object sender, EventArgs e)
            {
                frame.Continue = false;
            };

            op.Priority = priority;
            Dispatcher.PushFrame(frame);
        }

        private static Dispatcher UIADispatcher
        {
            get
            {
                if (_UIADispatcher == null)
                {
                    Thread UIAThread = new Thread(UIAThreadProc);
                    UIAThread.IsBackground = true;
                    UIAThread.Start();
                    _UIAThreadStartedEvent.WaitOne();
                    _UIADispatcher = Dispatcher.FromThread(UIAThread);
                }
                return _UIADispatcher;
            }
        }

        [STAThread]
        private static void UIAThreadProc()
        {
            if (!Dispatcher.CurrentDispatcher.HasShutdownStarted)
            {
                _UIAThreadStartedEvent.Set();
                Dispatcher.Run();
            }
        }

        private static Dispatcher _UIADispatcher;
        private static ManualResetEvent _UIAThreadStartedEvent = new ManualResetEvent(false);

        /// <summary>
        /// enum used for testing purposes.
        /// </summary>
        public enum Months
        {
            January = 1,
            February = 2,
            March = 3,
            April = 4,
            May = 5,
            June = 6,
            July = 7,
            August = 8,
            September = 9,
            October = 10,
            November = 11,
            December = 12
        }
    }
}
