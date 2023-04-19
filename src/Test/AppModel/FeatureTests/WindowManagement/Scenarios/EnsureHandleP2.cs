// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using Microsoft.Test.Input;
using Microsoft.Test.Threading;
using Microsoft.Windows.Test.Client.AppSec.BVT.ELCommon;

namespace WindowTest
{
    /// <summary>
    ///  WindowInteropHelper.EnsureHandle P2 cases
    /// </summary>
    public partial class EnsureHandleP2
    {
        DispatcherTimer _timer = new DispatcherTimer();
#if TESTBUILD_CLR40
        Window newWindow;
#endif

        void OnContentRendered(object sender, EventArgs e)
        {
#if TESTBUILD_CLR40
            bool caught = false;
            WindowInteropHelper interopHelper = null;
            IntPtr handle = IntPtr.Zero;
            newWindow = new Window();
            newWindow.Owner = Application.Current.MainWindow;
            
            // Case 1: Call WindowInteropHelper.EnsureHandle, then set Topmost and WindowState, then ShowDialog.  Should work.
            // Finally call ShowDialog().
            Logger.Status("Call WindowInteropHelper.EnsureHandle, then set Topmost and WindowState, then ShowDialog");
            try
            {
                interopHelper = new WindowInteropHelper(newWindow);
                handle = interopHelper.EnsureHandle();
                newWindow.Topmost = true;
                newWindow.WindowState = WindowState.Maximized;             

                timer.Tick += timer_Tick;
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Start();

                newWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                Logger.LogFail("Unexpected exception caught: " + ex.ToString());
                caught = true;
            }

            if (!caught)
            {
                Logger.LogPass("Tested calling Topmost and WindowState after WindowInteropHelper.EnsureHandle");
            }

            newWindow.Close();

            
            // Case 2: Call WindowInteropHelper.EnsureHandle, then call IsActive, then Show.  Should work.
            newWindow = new Window();
            newWindow.Owner = Application.Current.MainWindow;

            Logger.Status("Call WindowInteropHelper.EnsureHandle, then IsActive, then Show");
            try
            {
                interopHelper = new WindowInteropHelper(newWindow);
                handle = interopHelper.EnsureHandle();

                bool active = newWindow.IsActive;
                if (!active)
                {
                    Logger.LogPass("IsActive after WindowInteropHelper.EnsureHandle returned false as expected");
                }
                else
                {
                    Logger.LogFail("IsActive after WindowInteropHelper.EnsureHandle unexpectedly returned true");
                }

                newWindow.Show();
            }
            catch (Exception ex)
            {
                Logger.LogFail("Unexpected exception caught: " + ex.ToString());
                caught = true;
            }

            if (!caught)
            {
                Logger.LogPass("Tested calling IsActive after WindowInteropHelper.EnsureHandle");
            }

            newWindow.Close();

            // Case 3: Call WindowInteropHelper.EnsureHandle, then set Owner, then Show.  Should work.
            newWindow = new Window();

            Logger.Status("Call WindowInteropHelper.EnsureHandle, then set Owner, then Show");
            try
            {
                interopHelper = new WindowInteropHelper(newWindow);
                handle = interopHelper.EnsureHandle();
                newWindow.Owner = Application.Current.MainWindow;
                newWindow.Show();
            }
            catch (Exception ex)
            {
                Logger.LogFail("Unexpected exception caught: " + ex.ToString());
                caught = true;
            }

            if (!caught)
            {
                Logger.LogPass("Tested setting Owner after WindowInteropHelper.EnsureHandle");
            }

            newWindow.Close();

            // Case 4: Try calling EnsureHandle on another thread (should fail)
            newWindow = new Window();

            // Create a new thread
            Thread controlThread = new Thread(new ThreadStart(ControlThreadEntry));

            Logger.Status("Starting new thread and waiting for it to finish");
            controlThread.Start();
            controlThread.Join(); //wait for it to finish

            newWindow.Close();

#endif
            Logger.LogPass("test complete");

            this.Close();

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Logger.Status("Timer for closing dialog window has fired.");
            _timer.Stop();

            // Close dialog window
            Logger.Status("Sending Alt+F4 to dialog window");
            Input.SendKeyboardInput(System.Windows.Input.Key.LeftAlt, true);
            Input.SendKeyboardInput(System.Windows.Input.Key.F4, true);
            Input.SendKeyboardInput(System.Windows.Input.Key.F4, false);
            Input.SendKeyboardInput(System.Windows.Input.Key.LeftAlt, false);
        }

        private void ControlThreadEntry()
        {
#if TESTBUILD_CLR40
            bool caught = false;
            try
            {
                Logger.Status("Calling WindowInteropHelper.EnsureHandle on wrong thread.  Expect InvalidOperationException.");
                WindowInteropHelper interopHelper = new WindowInteropHelper(newWindow);
                interopHelper.EnsureHandle();
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogPass("Expected exception caught: " + ex.ToString());
                caught = true;
            }
            catch (Exception ex)
            {
                Logger.LogFail("Unexpected exception caught: " + ex.ToString());
            }

            if (caught)
            {
                Logger.LogPass("Caught expected exception when calling WindowInteropHelper.EnsureHandle from another thread.");
            }
            else
            {
                Logger.LogFail("Didn't catch expected exception when calling WindowInteropHelper.EnsureHandle from another thread.");
            }

#endif
        }
    }
}
