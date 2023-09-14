using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// AutomationValidator
    ///    It’s an abstract  Automation test base type.
    ///    The default behavior is that it creates process to launch ControlsAutomationTest.exe with an arg.
    ///    It gets the windowElement and targetElement for derived test types.
    ///    It implements IDisposable interface to provide cleanup service.  In Dispose cleanup method, we call Process.CloseMainWindow() to close wpf window.
    /// </summary>
    public abstract class AutomationValidator : IDisposable
    {
        public AutomationValidator(string args, string targetElementName)
        {
            AutomationEventHandler onWindowOpened = null;
            onWindowOpened = delegate(object sender, AutomationEventArgs e)
            {
                if (process != null)
                {
                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        Automation.RemoveAutomationEventHandler(WindowPatternIdentifiers.WindowOpenedEvent, AutomationElement.RootElement, onWindowOpened);
                        windowElement = AutomationElement.FromHandle(process.MainWindowHandle);
                    }
                    else
                    {
                        process.Refresh();
                    }
                }
            };

            Automation.AddAutomationEventHandler(WindowPatternIdentifiers.WindowOpenedEvent, AutomationElement.RootElement, TreeScope.Subtree, onWindowOpened);

            process = Process.Start(new ProcessStartInfo(fileName, args));

            while (windowElement == null)
            {
                Thread.Sleep(10);
            }

            targetElement = windowElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, targetElementName));
        }

        private Process process = null;

        protected string fileName = "ControlsAutomationTest.exe";
        protected AutomationElement windowElement = null;
        protected AutomationElement targetElement = null;

        public abstract void Run();

        public TestResult TestResult { get; protected set; }

        public bool HasProcessCrashed()
        {
            if (process == null)
            {
                return false;
            }

            process.WaitForExit();
            return (process.ExitCode != 0);
        }

        #region IDisposable Members

        public void Dispose()
        {
            process.CloseMainWindow();
        }

        #endregion
    }
}
