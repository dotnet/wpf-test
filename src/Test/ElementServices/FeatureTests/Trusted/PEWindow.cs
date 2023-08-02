// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Security.Permissions;

namespace Avalon.Test.CoreUI.Trusted.Helper
{
    /// <summary>
    /// Property Engine Window will be used as the base class for various
    /// Property Engine Tests. For example, EventTrigger tests derive from 
    /// EvtTriggerWindow that derives from PEWindow
    /// 
    /// This PEWindow is defined in trusted assembly so that it can assert
    /// security permissions
    /// </summary>
    public class PEWindow
    {
        private Window _testWindow;

        private static System.DateTime s_dtStartup;
        static PEWindow()
        {
            s_dtStartup = System.DateTime.Now;
        }

        /// <summary>
        /// So that we assert right permission
        /// </summary>
        [UIPermission(SecurityAction.Assert, Window = UIPermissionWindow.AllWindows)]
        public PEWindow()
        {
            _testWindow = new Window();
            SetupTimer();
        }

        private static TimeSpan CurrentPEWindowTime
        {
            get
            {
                return System.DateTime.Now - s_dtStartup;
            }
        }

        /// <summary>
        /// Subclass override this method to set up its UI
        /// </summary>
        protected virtual void TestSetupUI(Window testWindow)
        {
        }

        //
        private void SetupTimer()
        {
            _timerTickCount = 0;
            _timer = new System.Windows.Threading.DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(200);
            _timer.Tick += new EventHandler(timer_Tick);
            _timer.Start();
        }

        /// <summary>
        /// ShowDialog
        /// </summary>
        /// <returns></returns>
        public bool? ShowDialog()
        {
            return _testWindow.ShowDialog();
        }

        private void SetupTimer2()
        {

        }

        /// <summary>
        /// </summary>
        protected void SetResultAbort()
        {
            _testWindow.Tag = new bool?(false);
        }

        /// <summary>
        /// </summary>
        protected void SetResultOK()
        {
            _testWindow.Tag = new bool?(true);
        }

        /// <summary>
        /// </summary>
        protected bool IsResultSetAbort
        {
            get
            {
                if (_testWindow.Tag != null && !((bool)_testWindow.Tag))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// This method is called every 100ms (0.1second) in which 
        /// test case can do its validation, perform additional action
        /// </summary>
        /// <param name="pulseCount"></param>
        /// <returns>True to close window, false to continue</returns>
        [UIPermission(SecurityAction.Assert, Window = UIPermissionWindow.AllWindows)]
        [SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected virtual bool PulseEvery100Ms(int pulseCount)
        {
            if (pulseCount % 10 == 2)
            {
                //L:Local, G:Global, D:Difference
                //string info = string.Format("L {0} - G {1} - D {2}", CurrentPEWindowTime.ToString(), TimeManager.CurrentGlobalTime.ToString(), TimeManager.CurrentGlobalTime - CurrentPEWindowTime);
                string info = "";
                _testWindow.Title = info;
            }
            if (_testWindow.Tag != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private System.Windows.Threading.DispatcherTimer _timer;
        private int _timerTickCount;

        [UIPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
        void timer_Tick(object sender, EventArgs e)
        {
            _timerTickCount++;
            _timerTickCount++;
            if (_timerTickCount > 10)
            {
                if (PulseEvery100Ms(_timerTickCount - 10))
                {
                    _timer.Stop();
                    _testWindow.DialogResult = (bool?)_testWindow.Tag;
                }
            }
            else
            {
                if (_timerTickCount == 10)
                {
                    TestSetupUI(_testWindow);
                }
            }
        }

        //---- What follows are commonly used code snippet

        /// <summary>
        /// Assert used in PEWindow
        /// </summary>
        /// <param name="condition">If false, set failure</param>
        /// <param name="message">Message to print out</param>
        protected void WindowAssert(bool condition, string message)
        {
            if (condition)
            {
                CoreLogger.LogStatus("[Vevified OK] " + message, System.ConsoleColor.Blue);
            }
            else
            {
                string failMessage = "[Verification Failed] " + message;

                CoreLogger.LogStatus(failMessage, System.ConsoleColor.Red);
                SetResultAbort();
            }
        }
    }
}

