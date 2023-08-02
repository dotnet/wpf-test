// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Collections;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted.Controls;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Win32;
using Microsoft.Test.Threading;
using Avalon.Test.CoreUI.SourceHook;

namespace Avalon.Test.CoreUI.SourceHook
{
    // normal message
    internal enum MsgSet_Normal
    {
        WM_SETFOCUS = 0x0007,
        WM_KILLFOCUS = 0x0008,
        WM_ENABLE = 0x000A,
        WM_SETREDRAW = 0x000B,
        WM_SETTEXT = 0x000C,
        WM_GETTEXT = 0x000D,
        WM_GETTEXTLENGTH = 0x000E,
        WM_DEVMODECHANGE = 0x001B,
        WM_ACTIVATEAPP = 0x001C,
        WM_FONTCHANGE = 0x001D,
        WM_TIMECHANGE = 0x001E,
        WM_CANCELMODE = 0x001F,
        WM_SETCURSOR = 0x0020,
        WM_MOUSEACTIVATE = 0x0021,
        WM_CHILDACTIVATE = 0x0022,
        WM_QUEUESYNC = 0x0023,
        WM_MOUSEFIRST = 0x0200,
        WM_MOUSEMOVE = 0x0200,
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_LBUTTONDBLCLK = 0x0203,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205,
        WM_RBUTTONDBLCLK = 0x0206,
        WM_MBUTTONDOWN = 0x0207,
        WM_MBUTTONUP = 0x0208,
        WM_MBUTTONDBLCLK = 0x0209,
        WM_GETMINMAXINFO = 0x0024,
        WM_TIMER = 0x0113,
        //WM_SIZING = 0x0214,
        WM_SIZE = 0x0005,
        WM_PAINT = 0x000F,
        WM_CREATE = 0x0001
    };

    // messages consumed by HwndSource
    internal enum MsgSet_Special
    {
        WM_CLOSE = 0x0010,
        WM_DESTROY = 0x0002,
        WM_NCDESTROY = 0x0082
    };

    /// <summary>
    /// instance of Sourcing hook test
    /// </summary>
    [TestDefaults]
    public class SourceHookTest : TestCase
    {
        private SourceHookHelper _helper;
        private Dispatcher _dispatcher;
        private Validation _validate;

        private const int S_DETACHMESSAGE = 0x0000c257;

   
        // Test messages
        private const int TEST_MESSAGE = NativeConstants.WM_KEYDOWN,
                            TEST_MESSAGE2 = NativeConstants.WM_KEYUP;


        /// <summary>
        /// initialization
        /// </summary>
        public SourceHookTest()
        {
            _helper = new SourceHookHelper();
            _validate = Validation.GetInstance();
        }

        /// <summary>
        /// Run Test 1: Validate simple AddHook and RemoveHook
        /// </summary>
        [Test(0, @"Source\Hook", TestCaseSecurityLevel.FullTrust, "SourceHookTest1", Area="AppModel")]
        public void RunTest1()
        {
            CoreLogger.BeginVariation(DriverState.TestName);
            bool bPass = true;

            CoreLogger.LogStatus("Run Test 1: Validate simple AddHook and RemoveHook");

            //Create an instance of HwndSource with a hook that can receive all message
            CoreLogger.LogStatus("Construct a HwndSource with SourceHook");

            HwndSourceHook hook = new HwndSourceHook(_helper.SimpleHook1);
            HwndSource source = CreateHwndSource(hook);

            if (source == null)
            {
                throw new Exception("failed to create HwndSource");
            }

            // Send messages
            DoDispatcherWork(source.Handle, TEST_MESSAGE);

            // Do validation
            if (FindMessage("SimpleHook1", TEST_MESSAGE))
            {
                CoreLogger.LogStatus("Add SimpleHook1 succeeded");
            }
            else
            {
                bPass = false;
                CoreLogger.LogStatus("Add SimpleHook1 failed", ConsoleColor.Red);
            }

            // Add a hook
            hook = new HwndSourceHook(_helper.SimpleHook2);
            source.AddHook(hook);

            // Send messages
            DoDispatcherWork(source.Handle, TEST_MESSAGE);

            // Do validation
            if (FindMessage("SimpleHook2", TEST_MESSAGE))
            {
                CoreLogger.LogStatus("AddHook SimpleHook2 succeeded");
            }
            else
            {
                CoreLogger.LogStatus("AddHook SimpleHook2 failed", ConsoleColor.Red);
            }

            // Reset test states
            _validate.Reset();

            // remove hook
            CoreLogger.LogStatus("Remove SimpleHook1");
            source.RemoveHook(new HwndSourceHook(_helper.SimpleHook1));

            // send messages
            DoDispatcherWork(source.Handle, TEST_MESSAGE);

            // Do validation
            if (FindMessage("SimpleHook1", TEST_MESSAGE) == true
                        && FindMessage("SimpleHook2", TEST_MESSAGE) != true)
            {
                bPass = false;
                CoreLogger.LogStatus("RemoveHook SimpleHook1 failed", ConsoleColor.Red);
            }
            else
            {
                CoreLogger.LogStatus("RemoveHook SimpleHook1 succeeded");
            }

            // Reset test states
            _validate.Reset();

            // remove another hook
            CoreLogger.LogStatus("Remove SimpleHook2");
            source.RemoveHook(new HwndSourceHook(_helper.SimpleHook2));

            // send messages
            DoDispatcherWork(source.Handle, TEST_MESSAGE);

            // Do validation
            if (FindMessage("SimpleHook2", TEST_MESSAGE) == true)
            {
                bPass = false;
                CoreLogger.LogStatus("RemoveHook failed", ConsoleColor.Red);
            }
            else
            {
                CoreLogger.LogStatus("RemoveHook succeeded");
            }

            _validate.Reset();

            if (bPass)
            {
                CoreLogger.LogStatus("Run Test 1: passed", ConsoleColor.Green);
            }
            else
            {
                CoreLogger.LogTestResult(false, "Test 1: Validate simple AddHook and RemoveHook.");
            }

            CoreLogger.EndVariation();            
        }

        /// <summary>
        /// Validate Hook can receive all messages
        /// </summary>
        [Test(0, @"Source\Hook", TestCaseSecurityLevel.FullTrust, "SourceHookTest2", Area = "AppModel")]
        public void RunTest2()
        {
            CoreLogger.BeginVariation(DriverState.TestName);
            bool bPass = true;

            CoreLogger.LogStatus("Run Test 2: Validate Hook can receive all messages");

            // construct a HwndSource with a hook that receives all messages;
            CoreLogger.LogStatus("construct a HwndSource with AllMessagesHook");
            HwndSourceHook hook = new HwndSourceHook(_helper.AllMessagesHook);
            HwndSource source = CreateHwndSource(hook);

            // Send messages
            DoDispatcherWork(source.Handle, typeof(MsgSet_Normal));

            // validate 
            CoreLogger.LogStatus("do validation:");
            bPass = FindMessages("AllMessagesHook", typeof(MsgSet_Normal));

            CoreLogger.LogStatus("Test two done");

            if (bPass == true)
            {
                CoreLogger.LogStatus("AllMessageHook succeeded");
            }
            else
            {
                CoreLogger.LogTestResult(false, "AllMessagesHook failed");
            }

            _validate.Reset();
            CoreLogger.EndVariation();
        }



        /// <summary>
        /// Validate when a message has been handled by a SourceHook, the rest won't get this message
        /// </summary>
        [Test(0, @"Source\Hook", TestCaseSecurityLevel.FullTrust, "SourceHookTest3", Area = "AppModel")]       
        public void RunTest3()
        {
            CoreLogger.BeginVariation(DriverState.TestName);
            bool bPass = true;

            CoreLogger.LogStatus("Run Test 3: Validate when messages being handled will stop passing messages on");

            // Create an HwndSource
            HwndSource source = CreateHwndSource(null);

            // AddHook: AllMessageHook1
            CoreLogger.LogStatus("AddHook: AllMessageHook1");
            source.AddHook(new HwndSourceHook(_helper.AllMessagesHook));

            // AddHook: SimpleHook1
            CoreLogger.LogStatus("AddHook: SimpleHook1");
            source.AddHook(new HwndSourceHook(_helper.SimpleHook1));

            // AddHook: SimpleHook3
            CoreLogger.LogStatus("AddHook: SimpleHook3");
            source.AddHook(new HwndSourceHook(_helper.SimpleHook3));

            // AddHook: SimpleHook2
            CoreLogger.LogStatus("AddHook: SimpleHook2");
            source.AddHook(new HwndSourceHook(_helper.SimpleHook2));

            // send message
            DoDispatcherWork(source.Handle, TEST_MESSAGE);

            // do validation
            if (!FindMessage("SimpleHook2", TEST_MESSAGE))
            {
                bPass = false;
                CoreLogger.LogStatus("SimpleHook2 should get the message", ConsoleColor.Red);
            }

            if (!FindMessage("SimpleHook3", TEST_MESSAGE) == true)
            {
                bPass = false;
                CoreLogger.LogStatus("SimpleHook3 should get the message", ConsoleColor.Red);
            }

            if (FindMessage("SimpleHook1", TEST_MESSAGE))
            {
                bPass = false;
                CoreLogger.LogStatus("SimpleHook1 shouldn't get the message", ConsoleColor.Red);
            }

            if (FindMessage("AllMessagesHook", TEST_MESSAGE))
            {
                CoreLogger.LogStatus("AllMessagesHook shouldn't get the messages", ConsoleColor.Red);
                bPass = false;
            }

            if (bPass)
                CoreLogger.LogStatus("Run Test 3: Passed", ConsoleColor.Green);
            else
                CoreLogger.LogTestResult(false, "Run Test 3: Validate when messages being handled will stop passing messages on failed");

            _validate.Reset();

            CoreLogger.EndVariation();
        }

        /// <summary>
        /// Validate 7,8,9,1000 times AddHook
        /// </summary>
        [Test(0, @"Source\Hook", TestCaseSecurityLevel.FullTrust, "SourceHookTest4", Area = "AppModel")]       
        public void RunTest4()
        {
            CoreLogger.BeginVariation(DriverState.TestName);
            bool bPass = true;

            CoreLogger.LogStatus("Run test 4: Validate 7, 8, 9, 1000 times AddHook");

            CoreLogger.LogStatus("Create HwndSource");
            HwndSource source = CreateHwndSource(null);

            bPass &= RunTest4Helper(source, 7);

            bPass &= RunTest4Helper(source, 8);

            bPass &= RunTest4Helper(source, 9);

            bPass &= RunTest4Helper(source, 1000);

            if (bPass)
            {
                CoreLogger.LogStatus("Run test 4: Validate 7,8,9,1000 times AddHook passed", ConsoleColor.Green);
            }
            else
            {
                CoreLogger.LogTestResult(false, "Run test 4: Validate 7,8,9,1000 times AddHook failed");
            }

            _validate.Reset();
            CoreLogger.EndVariation();
        }

        private bool RunTest4Helper(HwndSource source, int total)
        {
            bool bPass = true;

            CoreLogger.LogStatus("AddHook " + total + " times");
            for (int i = 0; i < total; i++)
            {
                source.AddHook(new HwndSourceHook(_helper.SimpleHook1));
            }

            // Send a message
            DoDispatcherWork(source.Handle, TEST_MESSAGE);

            // do validation
            CoreLogger.LogStatus("Do validation");
            int n = _validate.GetCallTimes("SimpleHook1");
            if (n != total)
            {
                bPass = false;
                CoreLogger.LogStatus("Expected times: " + total + "; Unexpected times: " + n);
            }

            _validate.Reset();

            // Remove all SourceHook
            CoreLogger.LogStatus("Remove all SourceHook");
            for (int i = 0; i < total; i++)
                source.RemoveHook(new HwndSourceHook(_helper.SimpleHook1));

            // send a message
            DoDispatcherWork(source.Handle, TEST_MESSAGE);

            // do validation
            CoreLogger.LogStatus("Do validation");
            n = _validate.GetCallTimes("SimpleHook1");
            if (n != 0)
            {
                bPass = false;
                CoreLogger.LogStatus("Expected times: 0; Unexpected times: " + n);
            }

            return bPass;
        }

        /// <summary>
        /// Validate SourceHook can send a message and receive this message
        /// </summary>
        [Test(0, @"Source\Hook", TestCaseSecurityLevel.FullTrust, "SourceHookTest5", Area = "AppModel")]       
        public void RunTest5()
        {
            CoreLogger.BeginVariation(DriverState.TestName);
            bool bPass = true;

            CoreLogger.LogStatus("Run test 5: Validate SourceHook can send a message and receive this message");

            CoreLogger.LogStatus("Create a HwndSource and Add a hook");
            HwndSource source = CreateHwndSource(null);

            source.AddHook(new HwndSourceHook(_helper.SndMessageHook));

            // send a message
            DoDispatcherWork(source.Handle, TEST_MESSAGE);

            // do validation
            if (!FindMessage("SndMessageHook", TEST_MESSAGE))
            {
                bPass = false;
                CoreLogger.LogStatus("SndMessageHook: failed to get the first message", ConsoleColor.Red);
            }

            if (!FindMessage("SndMessageHook", TEST_MESSAGE2))
            {
                bPass = false;
                CoreLogger.LogStatus("SndMessageHook: failed to get the second message", ConsoleColor.Red);
            }

            if (bPass)
                CoreLogger.LogStatus("Run Test 5: passed", ConsoleColor.Green);
            else
                CoreLogger.LogTestResult(false, "Run test 5: Validate SourceHook can send a message and receive this message failed");

            CoreLogger.EndVariation();
        }

        /// <summary>
        /// Validate SourceHook can receive disposing messages consumed by HwndSource
        /// </summary>
        [Test(0, @"Source\Hook", TestCaseSecurityLevel.FullTrust, "SourceHookTest6", Area = "AppModel")]        
        public void RunTest6()
        {
            CoreLogger.BeginVariation(DriverState.TestName);
            bool bPass = true;

            CoreLogger.LogStatus("Run Test 6: Validate SourceHook can receive disposing messages consumed by HwndSource");

            // Create HwndSource with AllMessageHook 
            HwndSource source = CreateHwndSource(new HwndSourceHook(_helper.AllMessagesHook));

            // Add a hook later
            //source.AddHook(new HwndSourceHook(_helper.AllMessagesHook2));

            // Send special message
            DoDispatcherWork(source.Handle, typeof(MsgSet_Special));

            // Validate that both hook receive these special messages
            CoreLogger.LogStatus("Validate that both hook receive these special messages");

            if (!FindMessages("AllMessagesHook", typeof(MsgSet_Special)))
            {
                bPass = false;
                CoreLogger.LogStatus("Failed to AddHook: AllMessageHook", ConsoleColor.Red);
            }

            //if (!FindMessages("AllMessagesHook2", typeof(MsgSet_Special)))
            //{
            //    bPass = false;
            //    CoreLogger.LogStatus("Failed to AddHook: AllMessageHook2", ConsoleColor.Red);
            //}

            if (bPass)
            {
                CoreLogger.LogStatus("Run Test 6: passed", ConsoleColor.Green);
            }
            else
            {
                CoreLogger.LogTestResult(false, "Validate SourceHook can receive messages consumed by HwndSource Failed");
            }
            CoreLogger.EndVariation();
        }

        /// <summary>
        /// Validate SourceHook can send detach message
        /// </summary>
        [Test(0, @"Source\Hook", TestCaseSecurityLevel.FullTrust, "SourceHookTest7", Area = "AppModel")]        
        public void RunTest7()
        {
            CoreLogger.BeginVariation(DriverState.TestName);
            bool bPass = true;

            CoreLogger.LogStatus("Run Test 7: Validate SourceHook sending detach message won't break the application");

            // Create hwndsource
            HwndSource source = CreateHwndSource(null);

            // Add a regular hook
            CoreLogger.LogStatus("Add SimplerHook1 and DetachMsgHook");
            source.AddHook(new HwndSourceHook(_helper.DetachMsgHook));

            // send message to ask detach
            DoDispatcherWork(source.Handle, TEST_MESSAGE);

            if (!FindMessage("DetachMsgHook", TEST_MESSAGE))
            {
                bPass = false;
                CoreLogger.LogStatus("DetachMsgHook failed to get the message");
            }

            _validate.Reset();

            source.AddHook(new HwndSourceHook(_helper.SimpleHook1));

            // send message to ask detach
            DoDispatcherWork(source.Handle, TEST_MESSAGE);

            if (!FindMessage("SimpleHook1", TEST_MESSAGE))
            {
                bPass = false;
                CoreLogger.LogStatus("Detach messages won't break the system");
            }

            if (bPass)
                CoreLogger.LogStatus("Run test 7: passed", ConsoleColor.Green);
            else
                CoreLogger.LogTestResult(false, "Run test 7: Validate SourceHook can send detach message failed");

            CoreLogger.EndVariation();
        }

        /// <summary>
        /// Validate AddHook and RemoveHook after HwndSource.Dispose
        /// </summary>
        [Test(0, @"Source\Hook", TestCaseSecurityLevel.FullTrust, "SourceHookTest8", Area = "AppModel")]        
        public void RunTest8()
        {
            CoreLogger.BeginVariation(DriverState.TestName);
            bool bPass = true;

            CoreLogger.LogStatus("Run test 8: Validate AddHook and RemoveHook after HwndSource.Dispose");

            //Create HwndSource and AddHook: SimpleHook1
            CoreLogger.LogStatus("Create HwndSource and AddHook: SimpleHook1");
            HwndSource source = CreateHwndSource(null);
            source.AddHook(new HwndSourceHook(_helper.SimpleHook1));

            // Dispose
            CoreLogger.LogStatus("Dipose this HwndSource");
            source.Dispose();

            // Remove this hook
            CoreLogger.LogStatus("RemoveHook: SimpleHook1");
            source.RemoveHook(new HwndSourceHook(_helper.SimpleHook1));

            // Send Message
            DoDispatcherWork(source.Handle, TEST_MESSAGE);
            if (FindMessage("SimpleHook1", TEST_MESSAGE))
            {
                bPass = false;
                CoreLogger.LogStatus("RemoveHook after dispose failed");
            }
            else
                CoreLogger.LogStatus("RemoveHook after dispose succeeded");

            // AddHook after dispose
            CoreLogger.LogStatus("AddHook: SimpleHook2");
            try
            {
                source.AddHook(new HwndSourceHook(_helper.SimpleHook2));
            }
            catch (Exception e)
            {
                ObjectDisposedException oe = e as ObjectDisposedException;
                if (oe == null)
                {
                    bPass = false;
                    CoreLogger.LogStatus("AddHook after dispose: throw unexpected exception: " + e.Message, ConsoleColor.Red);
                }
                else
                    CoreLogger.LogStatus("AddHook after dispose: does throw an exception");
            }
            finally
            {
                if (!bPass)
                {
                    CoreLogger.LogTestResult(false, "AddHook and RemoveHook after Dispose failed");
                }
                _validate.Reset();
            }
            CoreLogger.EndVariation();
        }

        /// <summary>
        /// Cover low level Attach
        /// </summary>
        [Test(0, @"Source\Hook", TestCaseSecurityLevel.FullTrust, "SourceHookTest9", Area = "AppModel")]
        public void RunTest9()
        {
            CoreLogger.BeginVariation(DriverState.TestName);
            bool bPass = true;

            IntPtr hwnd = CreateWindow();

            CoreLogger.LogStatus("Create and Attach SimpleHook1 to an exsiting window");
            HwndSubclassHelper _subclass = new HwndSubclassHelper(hwnd);

            // send message
            DoDispatcherWork(hwnd, TEST_MESSAGE);

            // do Validation
            CoreLogger.LogStatus("Validate SimpleHook1 can receive messages");
            if (_subclass.GotMessage > 0)
            {
                CoreLogger.LogStatus("SimpleHook1 receives messages");
            }
            else
            {
                bPass = false;
                CoreLogger.LogStatus("SimpleHook1 failed to get messages");
            }

            _subclass.Reset();

            // Detach
            CoreLogger.LogStatus("Detach SimpleHook1 from hwnd");
            _subclass.Detach(true);

            // send message
            DoDispatcherWork(hwnd, TEST_MESSAGE);

            // do Validation
            CoreLogger.LogStatus("Validate Detach");
            if (_subclass.GotMessage > 0)
            {
                bPass = false;
                CoreLogger.LogStatus("SimpleHook1 should not receives messages", ConsoleColor.Red);
            }
            else
            {
                CoreLogger.LogStatus("Detach succeeded");
            }

            if (!bPass)
                CoreLogger.LogTestResult(false, "Attach/Detach failed");
            else
                CoreLogger.LogStatus("Run Test 9: passed", ConsoleColor.Yellow);

            CoreLogger.EndVariation();
        }

        /// <summary>
        /// Cover low level Attach
        /// </summary>
        //[Test(1, @"Source\Hook", TestCaseSecurityLevel.FullTrust, "SourceHookTest10", Disabled=true, Area="AppModel")]
        public void RunTest10()
        {
            CoreLogger.BeginVariation(DriverState.TestName);
            bool bPass = true;

            //IntPtr hwnd = CreateWindow();

            HwndSource source = CreateHwndSource(null);
            IntPtr hwnd = source.Handle;

            DispatcherHelper.DoEvents(100);

            CoreLogger.LogStatus("Run Test 10: lowel level Attach");
            CoreLogger.LogStatus("Create and Attach AllMessagesHook to an exsiting window");
            HwndSubclassHelper _subclass = new HwndSubclassHelper();

            // Attach all messages
            _subclass.Attach(hwnd);

            CoreLogger.LogStatus("Send normal messages");
            DoDispatcherWork(hwnd, typeof(MsgSet_Normal));

            ArrayList msgList = _subclass.GotMessages;
            if (!FindMessage(msgList, typeof(MsgSet_Normal)))
            {
                CoreLogger.LogStatus("Did not find message from normal set.");
                bPass = false;
            }

            CoreLogger.LogStatus("Send normal messages done");
            _subclass.Reset();

            CoreLogger.LogStatus("Send special messages");
            DoDispatcherWork(hwnd, typeof(MsgSet_Special));

            msgList = _subclass.GotMessages;
            if (!FindMessage(msgList, typeof(MsgSet_Special)))
            {
                CoreLogger.LogStatus("Did not find message from normal set.");
                bPass = false;
            }

            _subclass.Reset();

            if (bPass)
            {
                CoreLogger.LogStatus("Run Test 10: passed");
            }
            else
            {
                CoreLogger.LogTestResult(false, "Run Test 10: lowel level Attach");
            }
            CoreLogger.EndVariation();
        }

        /// <summary>
        /// Validate AddHook in Dispose event handler
        /// </summary>
        [Test(0, @"Source\Hook", TestCaseSecurityLevel.FullTrust, "SourceHookTest11", Area = "AppModel")]
        public void RunTest11()
        {
            CoreLogger.BeginVariation(DriverState.TestName);
            bool bPass = true;

            CoreLogger.LogStatus("Run Test 11: Validate AddHook in Dispose event handler");

            CoreLogger.LogStatus("Add Dispose event to Source");
            HwndSource source = CreateHwndSource(null);
            source.Disposed += OnDispose1;

            CoreLogger.LogStatus("Source.Dispose");
            source.Dispose();

            try
            {
                DoDispatcherWork(source.Handle, TEST_MESSAGE);

            }
            catch (Exception e)
            {
                CoreLogger.LogStatus("Exception shouldn't be thrown " + e.Message);
                bPass = false;
            }
            finally
            {
                if (FindMessage("SimpleHook1", TEST_MESSAGE))
                {
                    bPass = false;
                    CoreLogger.LogStatus("Sourcing hook in Dispose: succeeded");
                }

                if (!bPass)
                {
                    CoreLogger.LogTestResult(false, "Run Test 11 AddHook still succeeded after disposing");
                }
                else
                {
                    CoreLogger.LogStatus("Run Test 11: passed", ConsoleColor.Green);
                } 
            }
            CoreLogger.EndVariation();
        }

        private void OnDispose1(object sender, EventArgs e)
        {
            CoreLogger.LogStatus("In HwndSource.OnDispose: AddHook: SimpleHook1");

            HwndSource source = sender as HwndSource;
            if (source == null)
            {
                CoreLogger.LogStatus("Source failed to get Disposed Event");
            }
            else
            {
                source.AddHook(new HwndSourceHook(_helper.SimpleHook1));
            }
        }

        /// <summary>
        /// Cover low level Finalize
        /// </summary>
        [Test(0, @"Source\Hook", TestCaseSecurityLevel.FullTrust, "SourceHookTest12", Area = "AppModel")]        
        public void RunTest12()
        {
            CoreLogger.BeginVariation(DriverState.TestName);
            bool bPass = false;

            CoreLogger.LogStatus("RunTest12: Validate: AddHook after finialier");

            HwndSourceParameters p = new HwndSourceParameters("subsource");
            SubSource source = new SubSource(p);

            CoreLogger.LogStatus("Garbage collect");
            WeakReference weak = new WeakReference(source);
            source = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            try
            {
                if (weak.IsAlive)
                {
                    CoreLogger.LogStatus("After finalizer, AddHook");
                    source.AddHook(new HwndSourceHook(_helper.SimpleHook1));
                }
            }
            catch (Exception)
            {
                bPass = true;
            }
            finally
            {
                if (!bPass)
                {
                    CoreLogger.LogTestResult(false, "AddHook still succeeded after finalizer");
                }
                else
                {
                    CoreLogger.LogStatus("Run Test 12: passed", ConsoleColor.Green);
                }
            }
            CoreLogger.EndVariation();
        }

        /// <summary>
        /// Cover low level Finalize
        /// </summary>
        [Test(0, @"Source\Hook", TestCaseSecurityLevel.FullTrust, "SourceHookTest13", Area = "AppModel")]
        public void RunTest13()
        {
            CoreLogger.BeginVariation(DriverState.TestName);
            bool bPass = false;

            CoreLogger.LogStatus("Run Test 13: Validate: RemoveHook after finialier");

            HwndSourceParameters p = new HwndSourceParameters("subsource");
            p.HwndSourceHook = new HwndSourceHook(_helper.SimpleHook1);
            SubSource source = new SubSource(p);

            CoreLogger.LogStatus("Garbage collect");
            WeakReference weak = new WeakReference(source);
            source = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            try
            {
                if (weak.IsAlive)
                {
                    CoreLogger.LogStatus("After finalizer, RemoveHook");
                    source.RemoveHook(new HwndSourceHook(_helper.SimpleHook1));
                }
            }
            catch (Exception)
            {
                bPass = true;
            }
            finally
            {
                if (!bPass)
                {
                    CoreLogger.LogTestResult(false, "Run Test 13: Validate: RemoveHook after finialier");
                }
                else
                {
                    CoreLogger.LogStatus("Run Test 13: passed", ConsoleColor.Green);
                }
            }
            CoreLogger.EndVariation();
        }

        /// <summary>
        /// Validate hook throws an exception
        /// </summary>
        [Test(0, @"Source\Hook", TestCaseSecurityLevel.FullTrust, "SourceHookTest14", Area = "AppModel")]
        public void RunTest14()
        {
            CoreLogger.BeginVariation(DriverState.TestName);
            bool bPassed = false;

            CoreLogger.LogStatus("Run Test 14: Validate hook throws an exception");

            CoreLogger.LogStatus("AddHook: ExceptionHook");
            HwndSource source = CreateHwndSource(null);
            source.AddHook(new HwndSourceHook(_helper.ExceptionHook));

            try
            {
                DoDispatcherWork(source.Handle, TEST_MESSAGE);
            }
            catch (Exception e)
            {
                if (e.Message == "ExceptionHook")
                {
                    bPassed = true;
                }
            }
            finally
            {
                if (!bPassed)
                {
                    CoreLogger.LogTestResult(false, "Run Test 14: Validate hook throws an exception");
                }
                else
                {
                    CoreLogger.LogStatus("Run Test 14: passed", ConsoleColor.Green);
                }

            }
            CoreLogger.EndVariation();
        }

        /// <summary>
        /// Run Test 14: Validate layered window
        /// </summary>
        [Test(0, @"Source\Hook", TestCaseSecurityLevel.FullTrust, "SourceHookTest15", Area = "AppModel")]
        public void RunTest15()
        {
            CoreLogger.BeginVariation(DriverState.TestName);
            bool bPass = true;

            CoreLogger.LogStatus("Run Test 14: Validate layered window");

            CoreLogger.LogStatus("Create layered windows");
            object[] arr = CreateWindowHelper.CreateLayerWindows();

            HwndSource source = (HwndSource)arr[0];
            LayeredHost lh = (LayeredHost)arr[1];

            // Sourcing Avalon's hook
            source.AddHook(new HwndSourceHook(_helper.SimpleHook1));
            source.AddHook(new HwndSourceHook(_helper.SimpleHook2));
            source.AddHook(new HwndSourceHook(_helper.SimpleHook4));

            // Sourcing hwnd hook
            lh.MessageHook += new HwndSourceHook(_helper.AllMessagesHook);
            lh.MessageHook += new HwndSourceHook(_helper.AllMessagesHook2);
            lh.MessageHook += new HwndSourceHook(_helper.SimpleHook1);

            // send message
            DoDispatcherWork(source.Handle, TEST_MESSAGE);
            DoDispatcherWork(lh.Handle, TEST_MESSAGE);

            if (!FindMessage("SimpleHook2", TEST_MESSAGE))
            {
                bPass = false;
                CoreLogger.LogStatus("Source: AddHook: SimpleHook2 failed");
            }

            if (!FindMessage("SimpleHook4", TEST_MESSAGE))
            {
                bPass = false;
                CoreLogger.LogStatus("Source: AddHook: SimpleHook4 failed");
            }

            if (!FindMessage("AllMessagesHook", TEST_MESSAGE))
            {
                bPass = false;
                CoreLogger.LogStatus("Hwnd: AddHook: AllMesagesHook failed");
            }

            if (!FindMessage("AllMessagesHook2", TEST_MESSAGE))
            {
                bPass = false;
                CoreLogger.LogStatus("Hwnd: AddHook: AllMesagesHook2 failed");
            }

            if (_validate.GetCallTimes("SimpleHook1") != 2)
            {
                bPass = false;
                CoreLogger.LogStatus("SimpleHook1 should be called twice");
            }

            if (bPass == false)
            {
                CoreLogger.LogTestResult(false, "Run Test 14: Validate layered window");
            }
            else
            {
                CoreLogger.LogStatus("Run Test 14: passed", ConsoleColor.Green);
            }
            CoreLogger.EndVariation();
        }

        /// <summary>
        /// Run Test 14: Validate layered window
        /// </summary>
        [Test(0, @"Source\Hook", TestCaseSecurityLevel.FullTrust, "SourceHookTest16", Area = "AppModel")]
        public void RunTest16()
        {
            CoreLogger.BeginVariation(DriverState.TestName);
            bool bPass = true;

            CoreLogger.LogStatus("Run Test 14: Validate layered window");

            CoreLogger.LogStatus("Create layered windows");
            object[] arr = CreateWindowHelper.CreateLayerWindows();

            HwndSource source = (HwndSource)arr[0];
            LayeredHost lh = (LayeredHost)arr[1];

            // AddHook: AllMessagesHook
            CoreLogger.LogStatus("Hwnd: AllMessagesHook");
            lh.MessageHook += new HwndSourceHook(_helper.AllMessagesHook);

            DoDispatcherWork(lh.Handle, typeof(MsgSet_Normal));

            // validate 
            CoreLogger.LogStatus("do validation:");
            ArrayList msgList = new ArrayList();
            if (_validate.GetMessageList("AllMessagesHook", ref msgList) == true)
            {
                foreach (int i in Enum.GetValues(typeof(MsgSet_Normal)))
                {
                    if (!msgList.Contains(i))
                    {
                        bPass = false;
                        CoreLogger.LogStatus("AllMessagesHook failed to get msg:" + Enum.GetName(typeof(MsgSet_Normal), i), ConsoleColor.Red);
                    }
                }
            }


            _validate.Reset();

            // Send special message
            DoDispatcherWork(lh.Handle, typeof(MsgSet_Special));

            // Validate that both hook receive these special messages
            CoreLogger.LogStatus("Validate receive these special messages");
            if (_validate.GetMessageList("AllMessagesHook", ref msgList))
            {
                foreach (int i in Enum.GetValues(typeof(MsgSet_Special)))
                {
                    if (!msgList.Contains(i))
                    {
                        bPass = false;
                        CoreLogger.LogStatus("AllMessagesHook failed to get msg:" + Enum.GetName(typeof(MsgSet_Special), i), ConsoleColor.Red);
                    }
                }
            }
            else
            {
                bPass = false;
                CoreLogger.LogStatus("Failed to AddHook: AllMessageHook");
            }

            if (!bPass)
            {
                CoreLogger.LogTestResult(false, "Host receives all or special messages Failed");
            }
            else
            {
                CoreLogger.LogStatus("Run Test 16: passed", ConsoleColor.Green);
            }
            CoreLogger.EndVariation();
        }

        /// <summary>
        /// order
        /// </summary>
        [Test(0, @"Source\Hook", TestCaseSecurityLevel.FullTrust, "SourceHookTest17", Area = "AppModel")]
        public void RunTest17()
        {
            CoreLogger.BeginVariation(DriverState.TestName);
            bool bPass = true;

            Stack _stack = new Stack(6);

            CoreLogger.LogStatus("Run Test 17: Validate order");

            CoreLogger.LogStatus("Construct HwndSource with AllMessagesHook");

            //HwndSource source = CreateHwndSource(new HwndSourceHook(_helper.AllMessagesHook));
            //_stack.Push("AllMessagesHook");

            HwndSource source = CreateHwndSource(null);

            // Sourcing Avalon's hook
            source.AddHook(new HwndSourceHook(_helper.SimpleHook1));
            _stack.Push("SimpleHook1");

            source.AddHook(new HwndSourceHook(_helper.SimpleHook2));
            _stack.Push("SimpleHook2");

            source.AddHook(new HwndSourceHook(_helper.SimpleHook4));
            _stack.Push("SimpleHook4");

            //Send message
            DoDispatcherWork(source.Handle, TEST_MESSAGE);

            string[] funNames = new string[_stack.Count];
            _stack.CopyTo(funNames, 0);

            string[] results = new string[_stack.Count];
            if (_validate.GetOrders(results, _stack.Count))
            {
                if (results.Length < funNames.Length)
                {
                    bPass = false;
                }
                else
                {
                    for (int i = 0; i < funNames.Length; i++)
                    {
                        if (funNames[i] != results[i])
                        {
                            bPass = false;
                            CoreLogger.LogStatus("Expected: " + funNames[i] + "unExpected: " + results[i]);
                        }
                    }
                }
            }

            // test results
            if (bPass)
            {
                CoreLogger.LogStatus("Test 17: passed", ConsoleColor.Green);
            }
            else
            {
                CoreLogger.LogTestResult(false, "Run Test 17: Validate order: failed");
            }
            CoreLogger.EndVariation();
        }
        private IntPtr CreateWindow()
        {
            //IntPtr hwnd = CreateWindowEx
            HwndSource source = CreateHwndSource(null);
            return source.Handle;
        }

        private void DoDispatcherWork(IntPtr hwnd, Type msgType)
        {
            object[] args = new object[] { hwnd, msgType };

            _dispatcher = Dispatcher.CurrentDispatcher;
            _dispatcher.BeginInvoke(DispatcherPriority.Send, new DispatcherOperationCallback(DoSendMessage), (object)args);

            DispatcherHelper.DoEvents(100);
        }

        object DoSendMessage(object o)
        {
            object[] args = (object[])o;
            IntPtr hwnd = (IntPtr)args[0];
            Type msgType = (Type)args[1];

            foreach (int i in Enum.GetValues(msgType))
            {
                CoreLogger.LogStatus("Send message: " + Enum.GetName(msgType, i));
                NativeMethods.SendMessage(new HandleRef(null, hwnd), i, IntPtr.Zero, IntPtr.Zero);
            }

            return null;
        }


        private void DoDispatcherWork(IntPtr hwnd, int msg)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _dispatcher.BeginInvoke(DispatcherPriority.Send,
                (DispatcherOperationCallback)delegate(object o)
                {
                    object[] args = (object[])o;
                    IntPtr h = (IntPtr)args[0];
                    int m = (int)args[1];
                    
                    NativeMethods.SendMessage(new HandleRef(null, h), m, IntPtr.Zero, IntPtr.Zero);

                    return null;

                }, new object[] { hwnd, msg } );
            
            //_dispatcher.BeginInvokeShutdown(DispatcherPriority.SystemIdle);
            //DispatcherHelper.DoEvents(10);

            DispatcherHelper.DoEvents(100);
        
        }


        object SendMessage(object o)
        {
            object[] args = (object[])o;
            IntPtr hwnd = (IntPtr)args[0];
            int msg = (int)args[1];

            NativeMethods.SendMessage(new HandleRef(null, hwnd), msg, IntPtr.Zero, IntPtr.Zero);

            return null;
        }

        private bool FindMessage(ArrayList msgList, Type msgType)
        {
            bool bPass = true;

            if (msgList == null)
            {
                bPass = false;
                CoreLogger.LogStatus("SubClassing does not receive any messages", ConsoleColor.Red);
            }

            foreach (int i in Enum.GetValues(msgType))
            {
                if (!msgList.Contains(i))
                {
                    bPass = false;
                    CoreLogger.LogStatus("SubClassing did not receive message: " + Enum.GetName(msgType, i), ConsoleColor.Red);
                }
            }

            return bPass;
        }

        private bool FindMessages(string funName, Type msgType)
        {
            bool bPass = true;

            ArrayList msgList = new ArrayList();

            if (_validate.GetMessageList(funName, ref msgList) == true)
            {
                foreach (int i in Enum.GetValues(msgType))
                {
                    if (!msgList.Contains(i))
                    {
                        bPass = false;
                        CoreLogger.LogStatus(funName + " failed to get msg:" + Enum.GetName(msgType, i), ConsoleColor.Red);
                    }
                }
            }

            return bPass;
        }

        private bool FindMessage(string funName, int msg)
        {
            return _validate.FindMessage(funName, msg);
        }

    
   
        private HwndSource CreateHwndSource(HwndSourceHook hook)
        {
            HwndSourceParameters parameters = new HwndSourceParameters("SourceHookTest", 200, 200);

            if (hook != null)
                parameters.HwndSourceHook = hook;

            HwndSource source = new HwndSource(parameters);

            StackPanel sp = new StackPanel();

            source.RootVisual = sp;

            Button b = new Button();
            b.Height = 100;
            sp.Children.Add(b);

            if (source == null)
                throw new Exception("failed to create hwndsource");


            return source;
        }

    }



}
