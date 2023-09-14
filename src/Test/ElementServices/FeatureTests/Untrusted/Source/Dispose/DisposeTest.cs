// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Test.Discovery;
using Microsoft.Test.Win32;
using Microsoft.Test.Threading;
using System.Windows.Media;
using System.Collections;
using System.Security;
using System.Security.Permissions;
using System.Collections.Generic;

namespace Avalon.Test.CoreUI.DisposePattern
{
    /// <summary>
    /// instance of Sourcing hook test
    /// </summary>
    [TestDefaults]
    public class DisposeTest
    {
        Helper _helper = null;
        Validation _report = null;

        enum MsgSet
        {
            WM_DESTROY = 0x0002,
            WM_NCDESTROY = 0x0082
        };

        /// <summary>
        /// Create an instance for test dispose pattern
        /// </summary>
        public DisposeTest()
        {
            _helper = new Helper();
            _report = Validation.GetInstance();
        }

        /// <summary>
        /// Run Test 1: Dispose succeeds
        /// </summary>
        [TestAttribute(0, @"Source", TestCaseSecurityLevel.FullTrust, "DisposeTest1", Area = "AppModel")]
        public void RunTest1()
        {
            CoreLogger.BeginVariation();
            bool bPass = true;

            HwndSource source = CreateHwndSource();

            if (source.IsDisposed)
            {
                bPass = false;
                CoreLogger.LogStatus("expected value of IsDisposed: false;unexpected: true");
            }

            source.Dispose();

            try
            {
                if (!source.IsDisposed)
                {
                    bPass = false;
                    CoreLogger.LogStatus("expected value of IsDisposed: true;unexpected: false");
                }

                source.AddHook(new HwndSourceHook(SourceHook));
            }
            catch (Exception e)
            {
                ObjectDisposedException ede = e as ObjectDisposedException;
                if (ede == null)
                {
                    bPass = false;
                    CoreLogger.LogStatus("expect: ObjectDisposedException; unexpected: " + e.Message);
                }
            }
            finally
            {
                if (bPass)
                {
                    CoreLogger.LogStatus("Dispose succeeded");
                }
                else
                {
                    CoreLogger.LogTestResult(false, "Dispose failed");
                }
            }
            CoreLogger.EndVariation();
        }


        /// <summary>
        /// Run Test 2: Dipose a HwndSource attached with StackPanel succeeds
        /// </summary>
        [TestAttribute(0, @"Source", TestCaseSecurityLevel.FullTrust, "DisposeTest2", Area = "AppModel")]
        public void RunTest2()
        {
            CoreLogger.BeginVariation();
            bool bPass = true;

            HwndSource source = CreateHwndSource(true);

            source.Dispose();

            try
            {
                Visual root = source.RootVisual;
                if (root != null)
                {
                    bPass = false;
                    CoreLogger.LogStatus("expected: Visual tree should be removed");
                }
                else
                {
                    StackPanel sp = new StackPanel();
                    source.RootVisual = sp;
                }
            }
            catch (Exception e)
            {
                ObjectDisposedException ede = e as ObjectDisposedException;
                if (ede == null)
                {
                    bPass = false;
                    CoreLogger.LogStatus("expect: ObjectDisposedException; unexpected: " + e.Message);
                }
            }
            finally
            {
                if (bPass)
                {
                    CoreLogger.LogStatus("Dispose succeeded");
                }
                else
                {
                    CoreLogger.LogTestResult(false, "Dispose failed");
                }
            }
            CoreLogger.EndVariation();
        }

        /// <summary>
        /// Run Test 2: n times Dipose succeeded
        /// </summary>
        [TestAttribute(0, @"Source", TestCaseSecurityLevel.FullTrust, "DisposeTest3", Area = "AppModel")]
        public void RunTest3()
        {
            CoreLogger.BeginVariation();
            bool bPass = true;

            HwndSource source = CreateHwndSource();

            source.Dispose();

            try
            {
                for (int i = 0; i < 100; i++)
                {
                    source.Dispose();
                }
            }
            catch (Exception e)
            {
                if (e != null)
                {
                    bPass = false;
                    CoreLogger.LogStatus("unexpected exception: " + e.Message);
                }
            }
            finally
            {
                if (bPass)
                {
                    CoreLogger.LogStatus("n times Dipose succeeded");
                }
                else
                {
                    CoreLogger.LogTestResult(false, "n times Dispose failed");
                }
            }            
            CoreLogger.EndVariation();
        }


        /// <summary>
        /// Run Test 2: validate Diposing from different threads is not allowed
        /// </summary>
        [TestAttribute(0, @"Source", TestCaseSecurityLevel.FullTrust, "DisposeTest4", Area = "AppModel")]
        public void RunTest4()
        {
            CoreLogger.BeginVariation();
            _test4pass = true;

            _source = CreateHwndSource();

            Thread worker2 = new Thread(new ThreadStart(Test4Helper));

            worker2.Start();
            CoreLogger.LogStatus("start another thread to dipose");

            worker2.Join();
            CoreLogger.LogStatus("another thread terminated");

            if (_test4pass)
                CoreLogger.LogStatus("Dipose from different thread failed as expected");
            else
                CoreLogger.LogTestResult(false, "Dipose from different thread should fail");

            CoreLogger.EndVariation();
        }

        private HwndSource _source = null;
        private bool _test4pass = true;

        void Test4Helper()
        {
            try
            {
                CoreLogger.LogStatus("diposed by another thread");
                _source.Dispose();
            }
            catch (Exception e)
            {
                InvalidOperationException ioe = e as InvalidOperationException;
                if (ioe == null)
                {
                    _test4pass = false;
                    CoreLogger.LogStatus("expected: CheckAccess throws exception;unexpected: " + e.Message);
                }
            }
        }

        /// <summary>
        /// Run Test 5: Diposing from within pushframe succeeded
        /// </summary>
        [TestAttribute(0, @"Source", TestCaseSecurityLevel.FullTrust, "DisposeTest5", Area = "AppModel")]
        public void RunTest5()
        {
            CoreLogger.BeginVariation();
            bool bPass = true;

            HwndSource source = CreateHwndSource();

            DispatcherFrame frame = new DispatcherFrame();

            try
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback)delegate(object o)
                    {
                        object[] args = (object[])o;
                        ((HwndSource)args[0]).Dispose();
                        ((DispatcherFrame)args[1]).Continue = false;

                        return null;

                    }, new object[] { source, frame });


                Dispatcher.PushFrame(frame);

                StackPanel sp = new StackPanel();
                source.RootVisual = sp;

            }
            catch (Exception e)
            {
                ObjectDisposedException ode = e as ObjectDisposedException;
                if (ode == null)
                {
                    bPass = false;
                    CoreLogger.LogStatus("expected: ObjectDisposedException; unexpected: " + e.Message);
                }
            }
            finally
            {
                if (bPass)
                    CoreLogger.LogStatus("diposing from within pushframe succeeded");
                else
                    CoreLogger.LogTestResult(false, "diposing from within pushframe failed");
            }
            CoreLogger.EndVariation();
        }

        /// <summary>
        /// Run Test 6: Validate Dispose Event
        /// </summary>
        [TestAttribute(0, @"Source", TestCaseSecurityLevel.FullTrust, "DisposeTest6", Area = "AppModel")]
        public void RunTest6()
        {
            CoreLogger.BeginVariation();
            HwndSource source = CreateHwndSource();
            source.Disposed += _helper.OnDispose;

            source.Dispose();

            if (_report.FindEvent("DisposeEvent"))
            {
                CoreLogger.LogStatus("Validate Dispose Event succeeded");
            }
            else
            {
                CoreLogger.LogTestResult(false, "Dispose Event failed to happen");
            }
            CoreLogger.EndVariation();
        }


        /// <summary>
        /// Run Test 7: Validate Disposing from Dispose Event
        /// </summary>
        [TestAttribute(0, @"Source", TestCaseSecurityLevel.FullTrust, "DisposeTest7", Area = "AppModel")]
        public void RunTest7()
        {
            CoreLogger.BeginVariation();
            bool bPass = true;

            HwndSource source = _helper.Source;

            source.Disposed += _helper.DiposeFromDisposeEvent;

            try
            {
                source.Dispose();
                try
                {
                    source.AddHook(new HwndSourceHook(SourceHook));
                }
                catch (Exception ed)
                {
                    ObjectDisposedException ode = ed as ObjectDisposedException;
                    if (ode == null)
                    {
                        bPass = false;
                        CoreLogger.LogStatus("unexpected exception: " + ed.Message);
                    }
                }

            }
            catch (Exception e)
            {
                if (e != null)
                {
                    bPass = false;
                    CoreLogger.LogStatus("unexpected exception when disposing in disposing: " + e.Message);
                }
            }
            finally
            {
                if (bPass)
                    CoreLogger.LogStatus("diposing in disposing event succeeded");
                else
                    CoreLogger.LogTestResult(false, "diposing in disposing event failed");
            }
            CoreLogger.EndVariation();

        }

        /// <summary>
        /// Run Test 7: Validate Dispatcher.BeginInvokeShutdown 
        /// </summary>
        [TestAttribute(0, @"Source", TestCaseSecurityLevel.FullTrust, "DisposeTest8", Area = "AppModel")]
        public void RunTest8()
        {
            CoreLogger.BeginVariation();
            bool bPass = true;

            HwndSource source = CreateHwndSource();

            source.Disposed += _helper.DisposeShutdown;


            Dispatcher MainDispatcher = Dispatcher.CurrentDispatcher;
            MainDispatcher.ShutdownFinished += _helper.OnShutDownFinish;

            MainDispatcher.InvokeShutdown();

            bool bRet = true;
            if (_report.GetEventBool("Dispose_ShutdownStart", ref bRet))
            {
                if (!bRet)
                {
                    bPass = false;
                    CoreLogger.LogStatus("Shutdown is expected started", ConsoleColor.Red);
                }
            }
            else
            {
                bPass = false;
                CoreLogger.LogStatus("Dispose is not invoked", ConsoleColor.Red);
            }

            if (_report.GetEventBool("Dispose_ShutdownFinished", ref bRet))
            {
                if (bRet)
                {
                    bPass = false;
                    CoreLogger.LogStatus("Shutdown should not complete", ConsoleColor.Red);
                }
            }
            else
            {
                bPass = false;
                CoreLogger.LogStatus("Dispose is not invoked", ConsoleColor.Red);
            }

            int order = 0;
            if ((order = _report.GetEventOrder("DisposeEvent")) != 1)
            {
                bPass = false;
                CoreLogger.LogStatus("DisposeEvent order expected: 1; unexpected: " + order);
            }

            if ((order = _report.GetEventOrder("OnDispatcherShutdownFinish")) != 2)
            {
                bPass = false;
                CoreLogger.LogStatus("DispatcherShutdownFinish expected: 1; unexpected: " + order);
            }

            if (bPass)
                CoreLogger.LogStatus("Dispose before shutdown complete");
            else
                CoreLogger.LogTestResult(false, "Dispose before shutdown complete failed");
            CoreLogger.EndVariation();
        }

        /// <summary>
        /// Run Test 9: Validate WM_*msg that should be received by hook when disposing
        /// </summary>
        [TestAttribute(0, @"Source", TestCaseSecurityLevel.FullTrust, "DisposeTest9", Area = "AppModel")]
        public void RunTest9()
        {
            CoreLogger.BeginVariation();
            bool bPass = true;

            Window win = new Window();

            win.Show();

            HwndSource source = HwndSource.FromVisual(win) as HwndSource;
            source.AddHook(new HwndSourceHook(_helper.MsgListerner));

            Dispatcher MainDispatcher = Dispatcher.CurrentDispatcher;
            DispatcherFrame frame = new DispatcherFrame();

            MainDispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (DispatcherOperationCallback)delegate(object o)
                    {
                        object[] args = (object[])o;
                        Window _win = (Window)args[0];
                        DispatcherFrame _frame = (DispatcherFrame)args[1];
                        
                        _win.Close();

                        _frame.Continue = false;
                        return null;

                    }, new object[] { win, frame });


            Dispatcher.PushFrame(frame);

            List<int> msgList = new List<int>();
            if (_report.GetMessageList("MsgListerner", ref msgList) == true)
            {
                foreach (int i in Enum.GetValues(typeof(MsgSet)))
                {
                    if (msgList.BinarySearch(i) < 0)
                    {
                        bPass = false;
                        CoreLogger.LogStatus("msgListerner failed to get message: " + Enum.GetName(typeof(MsgSet), i),
                            ConsoleColor.Red);
                    }
                }
            }
            else
            {
                bPass = false;
                CoreLogger.LogStatus("msgListerner failed to be invoked");
            }

            if (bPass)
                CoreLogger.LogStatus("got messages successfully");
            else
                CoreLogger.LogTestResult(false, "failed to get messages");
            CoreLogger.EndVariation();
        }

        private HwndSource CreateHwndSource()
        {
            HwndSourceParameters parameters = new HwndSourceParameters("DisposeTest");

            HwndSource source = new HwndSource(parameters);

            return source;
        }

        private HwndSource CreateHwndSource(bool hasWindow)
        {
            HwndSourceParameters parameters = new HwndSourceParameters("DisposeTest", 200, 200);
            HwndSource source = new HwndSource(parameters);

            parameters.WindowStyle = NativeConstants.WS_VISIBLE;

            StackPanel sp = new StackPanel();

            source.RootVisual = sp;

            Button b = new Button();

            //b.Click += OnButtonClick;

            return source;
        }

        private IntPtr SourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool bHandled)
        {
            return IntPtr.Zero;
        }

    }
}
