using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// OutOfProcTestHelper
    /// </summary>
    public static class OutOfProcTestHelper
    {
        public static Process StartProcess(string executableName, string args, ref AutomationElement rootElement)
        {
            AutomationEventHandler OnWindowOpened = null;
            Process process = null;
            AutomationElement windowElement = null;

            OnWindowOpened = delegate(object sender, AutomationEventArgs e)
            {
                if (process != null)
                {
                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        Automation.RemoveAutomationEventHandler(WindowPatternIdentifiers.WindowOpenedEvent, AutomationElement.RootElement, OnWindowOpened);
                        windowElement = AutomationElement.FromHandle(process.MainWindowHandle);
                    }
                    else
                    {
                        process.Refresh();
                    }
                }
            };

            Automation.AddAutomationEventHandler(WindowPatternIdentifiers.WindowOpenedEvent, AutomationElement.RootElement, TreeScope.Subtree, OnWindowOpened);

            process = Process.Start(new ProcessStartInfo(executableName, args));

            while (windowElement == null)
            {
                Thread.Sleep(10);
            }

            rootElement = windowElement;

            return process;
        }
    }
}
