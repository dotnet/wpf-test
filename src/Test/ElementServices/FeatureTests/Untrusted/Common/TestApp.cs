// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************************
*    Purpose:   
     This class is intended to be a lite test application framework.
     It implements IHostedTest and can be used to develop functional and scenario test cases.
*    
 
  
*    Revision:         $Revision: 2 $
 
******************************************************************************/
using System;
using System.Diagnostics;
using System.Security;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using Microsoft.Test.Input;
using Microsoft.Test.Win32;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using System.Collections.Generic;
using Microsoft.Test.Integration;

namespace Avalon.Test.CoreUI
{

    /// <summary>
    /// 
    /// </summary>
    public delegate void InputCallback();
    
    /// <summary>
    /// A lite test application framework.
    /// </summary>
    public abstract class TestApp : IHostedTest
    {
        /// <summary>
        /// Things every app needs.
        /// </summary>
        public TestApp()
        {
            if (TestLog.Current == null)
            {
                _testLog = new TestLog("temp");
            }
            // Create valid context if needed
            if (_dispatcher == null)
            {
                _dispatcher = Dispatcher.CurrentDispatcher;
            }
        }

        TestLog _testLog = null;

        /// <summary>
        /// </summary>
        public ITestContainer TestContainer
        {
            get
            {
                return _testContainer;
            }
            set
            {
                _testContainer = value;
            }
        }

        ITestContainer _testContainer = null;

        /// <summary>
        /// 
        /// </summary>
        public static void RunTestAction(ContentItem item)
        {
            TestLog.Current.LogStatus("In TestApp.RunTestAction...");

            if (!CommonStorage.Current.Contains("TestContainer"))
            {
                throw new Microsoft.Test.TestSetupException("No 'TestContainer' in storage.");
            }

            TestLog.Current.LogStatus("'TestContainer' is in storage.");
            ITestContainer container = CommonStorage.Current.Get("TestContainer") as ITestContainer;

            Type type = Utility.FindType((string)item.Content, false);

            TestLog.Current.LogStatus("Running '" + type.Name + "'.");

            TestApp app = (TestApp)Activator.CreateInstance(type);
            app.TestContainer = container;
            app.Run();
        }

        /// <summary>
        /// Run our test.
        /// </summary>
        /// <remarks>
        /// We set and get the TestPassed property within this method. 
        /// </remarks>
        public void Run()
        {
            // Do sanity checks
            Debug.Assert(_dispatcher != null, "Dispatcher does not exist!");
            Debug.Assert(!this.TestPassed, "Case should not have passed by now!");

            if (TestContainer == null)
            {
                CoreLogger.LogStatus("Using Default TestContainer (ExeStubContainerFramework with HwndSource)");
                TestContainer = new ExeStubContainerCore();

            }

            _dispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(CommonTestAppDispatcherExceptionHandler);

            _testContainer.ExceptionThrown += new EventHandler(ExceptionWasThrown);

            // Do everything related to the test within the registered context
            try
            {
                SetupExecuteAndValidate(null);
            }
            finally
            {
                // Clean up after test app.
                Avalon.Test.CoreUI.Trusted.Input.ResetKeyboardState();
            }
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        protected virtual void ExceptionWasThrown(object o, EventArgs args)
        {
            CoreLogger.LogStatus("TestContainer detected a thrown exception.");
            if (this.FailAfterException)
            {
                CoreLogger.LogStatus("Failing this case...");
                this.TestPassed = false;
                CoreLogger.LogTestResult(this.TestPassed, "");
            }
        }

        private void AddToPostNotifyInput(NotifyInputEventHandler handler)
        {
            InputManagerHelper.Current.PostNotifyInput += handler;
        }

        /// <summary>
        /// Begin setting up, executing, and validating our test.
        /// </summary>
        /// <param name="obj">Not used.</param>
        /// <returns>Nothing.</returns>
        /// <remarks>
        /// Called eventually by the DoCallback method to indicate our context.
        /// </remarks>
        private object SetupExecuteAndValidate(object obj)
        {
            try
            {
                _dispatcher = Dispatcher.CurrentDispatcher;

                // Setup logger for our test app
                if (this.VerboseTrace)
                {
                    CoreLogger.LogStatus("Adding PostNotifyInput handler...");
                    AddToPostNotifyInput(new NotifyInputEventHandler(_OnPostNotifyInput));
                }


                // Setup test
                using (CoreLogger.AutoStatus("Posting test setup routine..."))
                {
                    _dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(DoSetup), null);
                }

                // Run test
                using (CoreLogger.AutoStatus("Posting test execution routine..."))
                {
                    _dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(DoExecute), null);
                }

                // Let's go!
                TestContainer.RequestStartDispatcher();

                // Validation (DoValidate) and tear-down (DoTearDown) occur later....
            }
            finally
            {
                // Clean up after test app.
                Avalon.Test.CoreUI.Trusted.Input.ResetKeyboardState();
            }
            return null;
        }



        /// <summary>
        /// Create an HwndSource based on a standard Win32 window.
        /// </summary>
        /// <param name="x">x position of window (screen coordinates).</param>
        /// <param name="y">y position of window (screen coordinates).</param>
        /// <param name="w">Width of window (pixels).</param>
        /// <param name="h">Height of window (pixels).</param>
        /// <returns>HwndSource object.</returns>
        /// <remarks>
        public HwndSource CreateStandardSource(int x, int y, int w, int h)
        {
            TestHwndSource newTestSource = new TestHwndSource(x, y, w, h);

            return newTestSource.Source;
        }

        /// <summary>
        /// Create directory name of assembly in which this code is an HwndSource based on a standard Win32 window.
        /// </summary>
        /// <returns>String representing absolute path to this assembly.</returns>
        protected string GetDirectoryNameOfAssembly()
        {
            return Path.GetDirectoryName(Assembly.GetAssembly(this.GetType()).Location);
        }

        /// <summary>
        /// Display an element in our test container.
        /// </summary>
        public void DisplayMe(UIElement element, int x, int y, int w, int h)
        {
            Surface surface = TestContainer.DisplayObject(element, x, y, w, h);

            _rootElement = element;

            // Save Win32 window handle for later
            _hwnd = new HandleRef(null, surface.Handle);
            NativeMethods.SetForegroundWindow(_hwnd);

            // Waiting until Render happen and layout.
            DispatcherHelper.DoEvents(DispatcherPriority.Loaded);
        }

        /// <summary>
        /// Navigate to new element in our test container.
        /// </summary>
        /// <param name="element"></param>
        public void NavigateTo(UIElement element)
        {
            bool bNavigated = TestContainer.NavigateToObject(element);
            DispatcherHelper.DoEventsPastInput();

            if (bNavigated)
            {
                int surfaceCount = TestContainer.CurrentSurface.Length;
                if (surfaceCount > 0)
                {
                    Surface s = TestContainer.CurrentSurface[0];
                    if (s != null)
                    {
                        CoreLogger.LogStatus("Navigated to '" + s.RootDisplayedObject + "'");
                        bool bSurfaceNavigationSucceeded = ((UIElement)s.RootDisplayedObject == element);
                        CoreLogger.LogStatus(" Succeeded? " + bSurfaceNavigationSucceeded + "'");
                        _rootElement = (UIElement)s.RootDisplayedObject;
                    }
                }
            }
        }

        /// <summary>
        /// Navigate back to old element in our test container.
        /// </summary>
        public void NavigateBack()
        {
            NavigateBackCore();
            DispatcherHelper.DoEventsPastInput();

            int surfaceCount = TestContainer.CurrentSurface.Length;
            if (surfaceCount > 0)
            {
                Surface s = TestContainer.CurrentSurface[0];
                if (s != null)
                {
                    CoreLogger.LogStatus("Gone back to '" + s.RootDisplayedObject + "'");
                    _rootElement = (UIElement)s.RootDisplayedObject;
                }
            }
        }

        private void NavigateBackCore()
        {
            TestContainer.GoBack();
        }

        /// <summary>
        /// Set up our test. 
        /// </summary>
        /// <param name="arg">User-defined argument.</param>
        /// <returns>Null object.</returns>
        /// <remarks>
        /// Override this in your derived app to do custom setup like window creation and initialization.
        /// </remarks>
        public abstract object DoSetup(object arg);

        /// <summary>
        /// Identify test operations to run.
        /// </summary>
        /// <param name="hwnd">Window handle.</param>
        /// <returns>Array of test operations against the window.</returns>
        /// <remarks>
        /// Override this in your derived app to define the input you want to send to the app.
        /// </remarks>
        protected virtual InputCallback[] GetTestOps(HandleRef hwnd)
        {
            // Default behavior - return an empty array.
            return new InputCallback[0];
        }

        /// <summary>
        /// Execute the test.
        /// </summary>
        /// <param name="arg">User-defined argument.</param>
        /// <returns>Null object.</returns>
        protected virtual object DoExecute(object arg)
        {
            // Default behavior - execute all test operations.
            ExecuteTestOperations();
            return null;
        }

        /// <summary>
        /// Execute the test operations.
        /// </summary>
        /// <remarks>
        /// Operations are supplied by the test when it overrides the GetTestOps method.
        /// </remarks>
        private void ExecuteTestOperations()
        {
            CoreLogger.LogStatus("Performing test operations....");

            InputCallback[] ops = GetTestOps(_hwnd);

            if (ops == null || ops.Length == 0)
            {
                // No test operations to execute. Just go immediately to the next step.
                CoreLogger.LogStatus(".... no operations found. Going immediately to validation.");
                InputCompleteHandler();
                return;
            }

            CoreLogger.LogStatus("Number of operations = " + ops.Length);

            Queue<InputCallback> actionQueue = new Queue<InputCallback>(ops);

            actionQueue.Enqueue(new InputCallback(InputCompleteHandler));
            CoreLogger.LogStatus(".... starting operation");            

            DispatcherHelper.EnqueueBackgroundCallback(new DispatcherOperationCallback (ExecuteNextInputCallback),
                actionQueue);

        }

        private object ExecuteNextInputCallback(object o)
        {
            Queue<InputCallback> actionQueue = (Queue<InputCallback>)o;
            InputCallback callback = actionQueue.Dequeue();

            callback();

            if (actionQueue.Count > 0)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle,
                    new DispatcherOperationCallback (ExecuteNextInputCallback),
                    actionQueue);
            }
                
            return null;
        }


        /// <summary>
        /// This is called when the last InputReport is processed as a result
        /// of the test operation, or when no test operations need to be run.
        /// </summary>
        /// <remarks>
        /// This handler will post the validation and cleanup routines with Background priority,
        /// so other input events should have plenty of time to execute.
        /// </remarks>
        private void InputCompleteHandler()
        {
            // Validate test
            CoreLogger.LogStatus("Posting test validation...");
            _dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback(DoValidate), null);

            if (this.QuitAfterRun)
            {
                // Exit test
                CoreLogger.LogStatus("Posting test cleanup...");
                _dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback(DoTearDown), null);
            }
        }

        private void CommonTestAppDispatcherExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            GlobalLog.LogEvidence("Unhandled test exception occurred in dispatcher.");
            if (!this.FailAfterException)
            {
                GlobalLog.LogEvidence("Handling this in order to continue running.");
                e.Handled = true;
            }
        }


        /// <summary>
        /// Validate our test.
        /// </summary>
        /// <param name="sender">User-defined argument.</param>
        /// <returns>Null object.</returns>
        /// <remarks>
        /// Override this in your derived app to validate your executed test.
        /// If your test results are as expected, set TestPassed=true within your override.
        /// </remarks>
        public abstract object DoValidate(object sender);

        /// <summary>
        /// Tear down our test.
        /// </summary>
        /// <param name="sender">User-defined argument.</param>
        /// <returns>Null object.</returns>
        protected object DoTearDown(object sender)
        {

            // Log final test results
            CoreLogger.LogTestResult(this.TestPassed, "Case result");

            // Clean up after test app.
            Avalon.Test.CoreUI.Trusted.Input.ResetKeyboardState();

            TestContainer.EndTest();

            return null;
        }

        /// <summary>
        /// Do we do a verbose trace of our output?
        /// </summary>
        public bool VerboseTrace
        {
            get { return s_verboseTrace; }
            set { s_verboseTrace = value; }
        }

        private static bool s_verboseTrace = false;

        /// <summary>
        /// Do we quit after the test is run?
        /// </summary>
        public bool QuitAfterRun
        {
            get { return _quitAfterRun; }
            set { _quitAfterRun = value; }
        }

        private bool _quitAfterRun = true;

        /// <summary>
        /// Do we unconditionally fail after an unexpected exception?
        /// </summary>
        public bool FailAfterException
        {
            get { return _quitAfterException; }
            set { _quitAfterException = value; }
        }

        private bool _quitAfterException = true;

        /// <summary>
        /// Did our test pass?
        /// </summary>
        public bool TestPassed
        {
            get { return _testPassed; }
            set { _testPassed = value; }
        }

        private bool _testPassed = false;

        /// <summary>
        /// Check if the condition is true, otherwise fail the test app.
        /// </summary>
        /// <param name="condition">Condition to be tested.</param>
        /// <param name="exceptionMsg">Message to be returned to the client if the condition is false.</param>
        /// <remarks>
        /// Should be used as a custom Assert macro.
        /// </remarks>
        protected void Assert(bool condition, string exceptionMsg)
        {
            // Log intermediate result
            if (!condition)
            {
                // Intermediate result = FAIL
                string exceptionString = exceptionMsg;
                this.TestPassed = false;

                throw new Microsoft.Test.TestValidationException(exceptionMsg);
            }
        }


        /// <summary>
        /// Default Dispatcher for app.
        /// </summary>
        private Dispatcher _dispatcher;

        /// <summary>
        /// Handle to app window.
        /// </summary>
        protected HandleRef _hwnd = NativeMethods.NullHandleRef;

        /// <summary>
        /// The root element in the app window.
        /// </summary>
        protected UIElement _rootElement;


        /// <summary>
        /// Default handler for the PostNotifyInput event.
        /// </summary>
        /// <param name="sender">Input manager.</param>
        /// <param name="args">Event-specific arguments.</param>
        /// <remarks>
        /// This is an informational event handler, not to be used for any test operations.
        /// </remarks>
        private void _OnPostNotifyInput(object sender, NotifyInputEventArgs args)
        {
            using (CoreLogger.AutoStatus("[TestApp] _OnPostNotifyInput"))
            {
                // Use InputManager and StagingItem to find input event
                RoutedEvent reid = (args.StagingItem).Input.RoutedEvent;

                CoreLogger.LogStatus("    InputName      =  " + reid.Name +
                    " [" + (args.StagingItem)).Input.Timestamp.ToString() + "]";

                // Input Report stuff
                if (InputHelper.IsInputReport(reid))
                {
                    InputEventArgs inputArgs = args.StagingItem.Input;
                    InputReportEventArgsWrapper inputReportArgs = new InputReportEventArgsWrapper(inputArgs);
                    InputReportWrapper ir = inputReportArgs.Report;
                    Debug.WriteLine("       ReportType  =  " + ir.Name);

                    if (ir.Name == "RawKeyboardInputReport")
                    {
                        // raw keyboard stuff
                        RawKeyboardInputReportWrapper rkir = new RawKeyboardInputReportWrapper(ir);

                        CoreLogger.LogStatus("         Raw       =  '" +
                            rkir.Actions.ToString() +
                            "',VK=" + rkir.VirtualKey.ToString("X2") + " (" + KeyInterop.KeyFromVirtualKey(rkir.VirtualKey).ToString() +
                            "),SCAN=" + rkir.ScanCode.ToString("X2") +
                            " - " + rkir.ExtraInformation);
                    }
                    else
                    {
                        if (ir.Name == "RawMouseInputReport")
                        {
                            // raw mouse stuff
                            RawMouseInputReportWrapper rkim = new RawMouseInputReportWrapper(ir);

                            CoreLogger.LogStatus("         Raw       =  '" +
                                rkim.Actions.ToString() +
                                "',X=" + rkim.X + ",Y=" + rkim.Y +
                                ",WHEEL=" + rkim.Wheel +
                                " - " + rkim.ExtraInformation);
                        }
                    }
                }

                // Input device stuff
                InputDevice device = args.StagingItem.Input.Device;

                if (device != null)
                {
                    CoreLogger.LogStatus("       Device      =  " + device.ToString());

                    IInputElement target = device.Target;

                    if (target != null)
                    {
                        CoreLogger.LogStatus("         Target    =  " + target.ToString());
                    }
                }
            }
        }
    }
}

