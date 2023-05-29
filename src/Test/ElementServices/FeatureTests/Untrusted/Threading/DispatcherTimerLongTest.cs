// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Avalon.Test.CoreUI.Threading
{
    /******************************************************************************
    * CLASS:          DispatcherTimerLong
    ******************************************************************************/
    /// <summary>
    /// DispatcherTimer Long Test (ps # 


    [Test(1, "Threading.DispatcherTimer", TestCaseSecurityLevel.FullTrust, "DispatcherTimerLong")]
    public class DispatcherTimerLong : TestCase
    {
        #region Data
        private DateTime _beginTime;     //Test begin time
        private int _tickCount;          //Current tick count
        private int _tickCountIncrement; //increment the tickCount
        private System.Timers.Timer _sysTimer;
        private DispatcherTimer _disTimer; 
        private int _countDispatcherTick;    //Number of timess dispatcher timer called
        private int _expectedElapsedTimeLimit = 30; //In Seconds
        private int _dispatcherTimerInterval = 3000; // In seconds
        private int _countNumberOfTimesSysTimerCalled; //Number of times sys timer called
        //private int elapsedTicks; //Number of elapsed tick count
        //private int startTickCount; //Start tick count
        private int _rollOverIndex = -1; //After how many times systimer called, we set target tick count 
        private int _targetTickCount; //Target tickcount to rollover to -ve etc.
        private string _testName = null; // Test
        private Dispatcher _dispathcher = Dispatcher.CurrentDispatcher;
        private Button _button;
        #endregion


        #region Constructor
        /******************************************************************************
        * Function:          DispatcherTimerLong Constructor
        ******************************************************************************/
        /// <summary>
        /// Constructor
        /// </summary>
        public DispatcherTimerLong()
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// </summary>
        TestResult StartTest()
        {
            //string commandLine = Environment.CommandLine;
            //testName = commandLine.Substring(commandLine.IndexOf("/testName="));
            //testName = (testName.Split('='))[1];
            GlobalLog.LogStatus("Test : Dispatcher Timer Test");

            Application app = new Application();
            app.Startup += new StartupEventHandler(app_Startup);
            app.Run();

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          app_Startup
        ******************************************************************************/
        private void app_Startup(object sender, EventArgs args)
        {
            GlobalLog.LogStatus("app_Startup. ");

            Application app;
            app = (Application)sender;

            Window  mainWindow = new Window();
            _button = new Button();
            mainWindow.Content = _button;
            mainWindow.ContentRendered += new EventHandler(mainWindow_ContentRendered);

            mainWindow.Show();

            app.Shutdown();
        }

        /******************************************************************************
        * Function:          mainWindow_ContentRendered
        ******************************************************************************/
        /// <summary>
        /// 
        /// </summary>
        private void mainWindow_ContentRendered(object sender, EventArgs args)
        {
            GlobalLog.LogStatus("mainWindow_ContentRendered");

            MouseHelper.Move(_button, MouseLocation.Center);

            GlobalLog.LogStatus("Set detour for GetTickCount() funtion");
            SetDetourOnCurrentProcess();

            _beginTime = DateTime.Now;

            switch (_testName.ToLower())
            {
                //Normal
                case "normal":
                    TestNormal();
                    break;

                //Rollover to negative from positive, due in +ve
                case "rollovertonegativefrompositive1":
                    TestRolloverToNegative1();
                    break;

                //Rollover to negative from positive, due in negative
                case "rollovertonegativefrompositive2":
                    TestRolloverToNegative2();
                    break;

                //Negative value near 0, rollover to +ve value near zero
                case "testrollovertopositivefromnegative":
                    TestRolloverToPositiveFromNegative();
                    break;

                //Test large rollover from max positive
                case "testlargerolloverfrommaxpositive":
                    TestLargeRolloverFromMaxPositive();
                    break;

                //Test large rollover from Max negative
                case "testlargerolloverfrommaxnegative":
                    TestLargeRolloverFromMaxNegative();
                    break;

                default:
                    GlobalLog.LogStatus("ERROR:  unknown testcase");
                    RegisterResult(false);
                    break;

            }

            Thread.Sleep(1000);

            StartTickAdjustment(null);
        }

        /******************************************************************************
        * Function:          TestNormal
        ******************************************************************************/
        /// <summary>
        /// Normal
        /// Start in +ve, due to +ve, increment the Tickcount
        /// This make sure test infrastructure actually works
        /// </summary>
        private void TestNormal()
        {
            GlobalLog.LogStatus("TestNormal");
            _tickCount = 37833890;
            _dispatcherTimerInterval = 300; //300 seconds
            _tickCountIncrement = 60000; //60 seconds
            SetTickCount(_tickCount);
        }

        /******************************************************************************
        * Function:          TestRolloverToNegative1
        ******************************************************************************/
        /// <summary>
        /// Start in +ve, due to +ve, rollover to negative
        /// -2^31 less than TickCount less than 2^31. We set the value somewhere near
        /// the Int32.MaxValue and set the interval 
        /// where (current tick count)+interval is less than 2^31
        /// Then we increment the tick count past Int32.MaxValue (rollover to -ve value)
        /// and verify DispatcherTimer fired Tick
        /// </summary>
        private void TestRolloverToNegative1()
        {
            GlobalLog.LogStatus("TestRolloverToNegative1");

            _tickCount = Int32.MaxValue - 5000000; //5000s

            _dispatcherTimerInterval = 3000; //3000 seconds
            _tickCountIncrement = 1000; //1 seconds
            SetTickCount(_tickCount);

            _targetTickCount = Int32.MinValue + 10000000; // Int32.MinValue+10000 seconds

            _rollOverIndex = 5; //After sys timer fired this many times, we set the tickCount to targetTickCount

        }

        /******************************************************************************
        * Function:          TestRolloverToNegative2
        ******************************************************************************/
        /// <summary>
        /// Start in +ve, due to -ve, rollover to negative
        /// </summary>
        private void TestRolloverToNegative2()
        {
            GlobalLog.LogStatus("TestRolloverToNegative1");

            _tickCount = Int32.MaxValue - 500; //5s

            _dispatcherTimerInterval = 3000; //3000 seconds (due is in -ve)
            _tickCountIncrement = 1000; //1 seconds
            SetTickCount(_tickCount);

            _targetTickCount = Int32.MinValue + 10000000; // Int32.MinValue+10000 seconds

            _rollOverIndex = 5;
        }

        /******************************************************************************
        * Function:          TestRolloverToPositiveFromNegative
        ******************************************************************************/
        /// <summary>
        /// Negative value near 0, rollover to +ve value near zero
        /// </summary>
        private void TestRolloverToPositiveFromNegative()
        {
            GlobalLog.LogStatus("TestRolloverToPositiveFromNegative");
            _tickCount = -5000000; // Near positive, - 5000 seconds

            _dispatcherTimerInterval = 3000; //3000 seconds
            _tickCountIncrement = 1000; //1 seconds

            GlobalLog.LogStatus("tickCount tickCount tickCount" + _tickCount);
            SetTickCount(_tickCount);

            _targetTickCount = 100000; // 100 seconds

            _rollOverIndex = 5;
        }

        /******************************************************************************
        * Function:          TestLargeRolloverFromMaxNegative
        ******************************************************************************/
        /// <summary>
        /// Test large rollover from Max negative
        /// </summary>
        private void TestLargeRolloverFromMaxNegative()
        {
            GlobalLog.LogStatus("TestLargeRolloverFromMaxNegative");
            _tickCount = Int32.MinValue + 10000;

            _dispatcherTimerInterval = 3000; //3000 seconds
            _tickCountIncrement = 1000; //1 seconds

            SetTickCount(_tickCount);

            _targetTickCount = 30000;

            _rollOverIndex = 5;
        }

        /******************************************************************************
        * Function:          TestLargeRolloverFromMaxPositive
        ******************************************************************************/
        /// <summary>
        /// Test large rollover from max positive
        /// </summary>
        private void TestLargeRolloverFromMaxPositive()
        {
            GlobalLog.LogStatus("TestLargeRolloverFromMaxPositive");
            _tickCount = Int32.MaxValue - 10000;

            _dispatcherTimerInterval = 3000; //3000 seconds
            _tickCountIncrement = 1000; //1 seconds

            SetTickCount(_tickCount);

            _targetTickCount = -30000;

            _targetTickCount = 1000;

            _rollOverIndex = 5;
        }

        /******************************************************************************
        * Function:          StartTickAdjustment
        ******************************************************************************/
        /// <summary>
        /// Setup the dispatcher timer
        /// Setup the system timer
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private object StartTickAdjustment(object args)
        {
            GlobalLog.LogStatus("StartTickAdjustment");

            _disTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _disTimer.Interval = TimeSpan.FromSeconds(_dispatcherTimerInterval);
            _disTimer.Tick += new EventHandler(OnTick);

            _disTimer.Start();

            IncrementTickCount();

            _sysTimer = new System.Timers.Timer();
            _sysTimer.Interval = 1000;
            _sysTimer.Enabled = true;
            _sysTimer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);

            IncrementTickCount();

            _sysTimer.Start();

            IncrementTickCount();

            return null;
        }

        /******************************************************************************
        * Function:          timer_Elapsed
        ******************************************************************************/
        /// <summary>
        /// System.Timers.Timer.Elapsed
        /// In this we increment the tick count
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void timer_Elapsed(object source, ElapsedEventArgs e)
        {
            GlobalLog.LogStatus("timer_Elapsed");
            Console.WriteLine("Thread ID " + Thread.CurrentThread.ManagedThreadId);

            if (_countDispatcherTick > 0)
                return;

            _countNumberOfTimesSysTimerCalled++;

            if (_rollOverIndex == _countNumberOfTimesSysTimerCalled)
            {
                //Set tick count to do the roll over
                _tickCount = _targetTickCount;
            }

            if (_countNumberOfTimesSysTimerCalled > 100)
            {
                _sysTimer.Enabled = false;
                _sysTimer.Stop();
                _disTimer.IsEnabled = false;
                _disTimer.Stop();

                GlobalLog.LogStatus("FAIL-System timer fired 100 times, but we havent got Tick from Dispatcher timer");
                
                //Register failure
                RegisterResult(false);

                return;
            }

            IncrementTickCount();

            _dispathcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(ButtonClick), null);

        }

        /******************************************************************************
        * Function:          ButtonClick
        ******************************************************************************/
        /// <summary>
        /// Keep Dispatcher active
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private object ButtonClick(object args)
        {
            MouseHelper.PressButton(MouseButton.Left);
            MouseHelper.ReleaseButton(MouseButton.Left);

            return null;
        }

        /******************************************************************************
        * Function:          IncrementTickCount
        ******************************************************************************/
        /// <summary>
        /// Increment TikcCount
        /// </summary>
        private void IncrementTickCount()
        {
            //Increment the TickCount
            _tickCount = _tickCount + _tickCountIncrement;
            SetTickCount(_tickCount);
        }


        /******************************************************************************
        * Function:          ExecuteSetTickCount
        ******************************************************************************/
        /// <summary>
        /// Call SetTickCount.exe with certain command line args
        /// </summary>
        /// <param name="cmdArgs"></param>
        /// <param name="input"></param>
        private void ExecuteSetTickCount(string cmdArgs, string input)
        {
            GlobalLog.LogStatus("ExecuteSetTickCount");

            GlobalLog.LogStatus("CommandLineArgs " + cmdArgs);

            if(input!=null)
                GlobalLog.LogStatus("Input " + input);

            Process setTickCountProcess = new Process();
            setTickCountProcess.StartInfo.FileName = "settickcount.exe";
            setTickCountProcess.StartInfo.Arguments = cmdArgs;
            setTickCountProcess.StartInfo.UseShellExecute = false;
            setTickCountProcess.StartInfo.RedirectStandardInput = true;
            setTickCountProcess.Start();
            if (input != null )
                setTickCountProcess.StandardInput.WriteLine(input);
            setTickCountProcess.CloseMainWindow();
            setTickCountProcess.Close();
            setTickCountProcess.Dispose();
            setTickCountProcess = null;
        }

        /******************************************************************************
        * Function:          SetDetourOnCurrentProcess
        ******************************************************************************/
        /// <summary>
        /// Set detour for GetTickCount, using settickcount.exe
        /// </summary>
        private void SetDetourOnCurrentProcess()
        {
            int processID = Process.GetCurrentProcess().Id;
            GlobalLog.LogStatus("Setting detour for GetTickCount() on current process with ID : " + processID);

            string commandlineArgs = "/p:" + processID;

            ExecuteSetTickCount(commandlineArgs, null);
        }

        /******************************************************************************
        * Function:          SetTickCount
        ******************************************************************************/
        /// <summary>
        /// Set TickCount, using settickcount.exe
        /// </summary>
        private void SetTickCount(int tickCount)
        {
            GlobalLog.LogStatus("Setting GetTickCount() value to : " + tickCount);

            string commandlineArgs = "/s";

            ExecuteSetTickCount(commandlineArgs, tickCount.ToString());
        }

        /******************************************************************************
        * Function:          ResetTickCount
        ******************************************************************************/
        private void ResetTickCount()
        {
            GlobalLog.LogStatus("Reset GetTickCount() to original value");

            string commandlineArgs = "/r";

            ExecuteSetTickCount(commandlineArgs,null);
        }

        /******************************************************************************
        * Function:          OnTick
        ******************************************************************************/
        /// <summary>
        /// EventHandler for DispatcherTimer.Tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTick(object sender, EventArgs args)
        {
            GlobalLog.LogStatus("DispatcherTimer.OnTick");

            Interlocked.Increment(ref _countDispatcherTick);

            _sysTimer.Enabled = false;
            _sysTimer.Stop();
            _disTimer.IsEnabled = false;
            _disTimer.Stop();

            TimeSpan elapsedTime = DateTime.Now.Subtract(_beginTime);

            if (elapsedTime.TotalSeconds > _expectedElapsedTimeLimit)
            {
                GlobalLog.LogStatus("TEST FAIL");
                GlobalLog.LogStatus("Expected elapsed time " + _expectedElapsedTimeLimit);
                GlobalLog.LogStatus("Actual elapsed time " + elapsedTime.TotalSeconds);
                GlobalLog.LogStatus("Actual elapsed time is more than expected");
                //Register fail
                RegisterResult(false);
                return;
            }

            //Register pass
            RegisterResult(true);
        }

        /******************************************************************************
        * Function:          RegisterResult
        ******************************************************************************/
        //Register test result with GlobalLog, shut down the application
        private void RegisterResult(bool result)
        {
            if (!result)
            {
                GlobalLog.LogEvidence("FAIL: DispatcherTimerLong tests");
            }
            else
            {
                GlobalLog.LogEvidence( "PASS: DispatcherTimerLong tests");
            }

            ResetTickCount();

            Thread.Sleep(10);

            GlobalLog.LogStatus("Shutdown the application");

            Application.Current.MainWindow.Close();
            Application.Current.Shutdown();
        }
        #endregion
    }
}


