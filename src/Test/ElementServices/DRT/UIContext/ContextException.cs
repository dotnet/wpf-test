// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows;
using DRT;

public class ExceptionTests : DrtTestSuite
{
    public ExceptionTests()  : base("Exception")
    {
        this.TeamContact = "WPF";
        this.Contact = "Microsoft";
    }
    
    public override DrtTest[] PrepareTests()
    {
        return new DrtTest[]
        {
            new DrtTest(Run)
        };
    }

    public void Run()
    {
        bool success = true;
        bool totalSuccess = true;
        
        // Test Dispatcher exception with no handlers
        {
            ExceptionTest test = null;
            bool _exceptionCaught = false;
            
            try
            {
                Console.WriteLine("ExceptionTest No Handlers... ");
                test = new ExceptionTest();
                test.Run();
                success &= test.Success;
            }
            catch (ArgumentException)
            {
                _exceptionCaught = true;
                if (test != null)
                    success &= test.Success;

                if (success)
                {
                    Console.WriteLine("    passed: Exception surfaced as expected when no handlers are attached.");
                }
            }
            if (!_exceptionCaught)
            {
                Console.WriteLine("FAILED! ExceptionTest: Exception was quietly suppressed when it should have surfaced (no handlers are attached).");
                success = false;
            }
            else
                Console.WriteLine("done");
        }
        totalSuccess &= success;
        success = true;

        // Test exception filters 1: verify RequestCatch stays OFF
        {
            ExceptionTest test = null;
            bool _exceptionCaught = false;

            try
            {
                Console.WriteLine("ExceptionTest Filters 1... ");
                test = new ExceptionFilterTest1();
                test.Run();
                success &= test.Success;
            }
            catch (ArgumentException)
            {
                _exceptionCaught = true;
                if (test != null)
                    success &= test.Success;

                if (success)
                {
                    Console.WriteLine("    passed: Exception surfaced as expected.");
                }
            }
            if (!_exceptionCaught)
            {
                Console.WriteLine("FAILED! ExceptionFilterTest1: Exception was quietly suppressed when it should have surfaced.");
                success = false;
            }
            else
                Console.WriteLine("done");
        }
        totalSuccess &= success;
        success = true;

        // Test exception filters 2: verify turning ReqCatch OFF
        {
            ExceptionTest test = null;
            bool _exceptionCaught = false;

            try
            {
                Console.WriteLine("ExceptionTest Filters 2... ");
                test = new ExceptionFilterTest2();
                test.Run();
                success &= test.Success;
            }
            catch (ArgumentException)
            {
                _exceptionCaught = true;
                if (test != null)
                    success &= test.Success;

                if (success)
                {
                    Console.WriteLine("    passed: Exception surfaced as expected.");
                }
            }
            if (!_exceptionCaught)
            {
                Console.WriteLine("FAILED! ExceptionFilterTest2: Exception was quietly suppressed when it should have surfaced.");
                success = false;
            }
            else
                Console.WriteLine("done");
        }
        totalSuccess &= success;
        success = true;

        // test DispatcherUnhandledExceptions, turning ON args.Handled and that it stays ON
        {
            DispatcherUnhandledExceptionTest test = null;

            try
            {
               Console.WriteLine("DispatcherUnhandledExceptions turning ON args.Handled and that it stays ON... ");
                test = new DispatcherUnhandledExceptionTest();
                test.Run();
                success &= test.Success;
            }
            catch (ArgumentException)
            {
                if (test != null)
                    success &= test.Success;

                if (Debugger.IsAttached)
                {
                    Console.WriteLine("    Exception surfaced even though DispatcherUnhandledException handlers exist.  This is expected when Debugger.IsAttached.");
                }
                else
                {
                    Console.WriteLine("FAILED! DispatcherUnhandledExceptionTest: handlers were not called or could not properly set handled to true.");
                    success = false;
                }
            }
            Console.WriteLine("done");
        }
        totalSuccess &= success;

        if (!totalSuccess)
            DRT.Fail("Failed: wrong behavior for DispatcherUnhandledException filters and handlers.");
    }
}

// Test the scenario where no handlers are hooked up.
// Throws ArgumentException as part of test.
// Base class for the rest of these tests.
public class ExceptionTest
{
    public ExceptionTest()
    {
        Console.WriteLine("  -- " + TestID + " --");
    }

    public void Run()
    {
        _dispatcher = Dispatcher.CurrentDispatcher;
        
        RegisterHandlers();
        try
        {
            _dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new DispatcherOperationCallback(GoodOperation),
                null);
            _dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new DispatcherOperationCallback(FailingOperation),
                null);
            _dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new DispatcherOperationCallback(StopDispatcher),
                null);
            RunDispatcher();
        }
        finally
        {
            UnregisterHandlers();
        }
        Console.WriteLine("    RunDispatcher() complete: no exceptions broke through");
    }

    public bool Success
    {
        get
        {
            return _success;
        }
        set
        {
            if (value == false)
            {
                _success = value;
            }
        }
    }

    virtual protected string TestID
    {
        get
        {
            return "Exception Test";
        }
    }

    virtual protected void RegisterHandlers()
    {
    }

    virtual protected void UnregisterHandlers()
    {
    }

    protected void ReportFailure(string message)
    {
        Console.WriteLine("[" + TestID + "] FAILED: " + message);
        Success = false;
    }

    private object GoodOperation(object arg)
    {
        Console.WriteLine("    Dispatcher operation: no exception");
        return null;
    }

    private object FailingOperation(object arg)
    {
        Console.WriteLine("    Dispatcher operation: throwing test exception");
        throw new ArgumentException("Dude, I won't like any argument you pass me", "arg");
    }

    private void RunDispatcher()
    {
        _dispatcherFrame = new DispatcherFrame();
        Dispatcher.PushFrame(_dispatcherFrame);
    }

    private object StopDispatcher(object arg)
    {
        _dispatcherFrame.Continue = false;
        return null;
    }

    private bool _success = true;
    protected Dispatcher _dispatcher;
    private DispatcherFrame _dispatcherFrame;
}

// Check that args.RequestCatch is OFF when no exception handlers exist
// and verify that it stays OFF.
public class ExceptionFilterTest1 : ExceptionTest
{
    public ExceptionFilterTest1()
    {
    }

    override protected string TestID
    {
        get
        {
            return "ExceptionFilter Test 1";
        }
    }

    override protected void RegisterHandlers()
    {
        Console.WriteLine("    Registering handlers: ExceptionFilter only");
        _dispatcher.UnhandledExceptionFilter += new DispatcherUnhandledExceptionFilterEventHandler(FilterException1);
        _dispatcher.UnhandledExceptionFilter += new DispatcherUnhandledExceptionFilterEventHandler(FilterException2);
    }

    override protected void UnregisterHandlers()
    {
        Console.WriteLine("    Unregistering handlers: ExceptionFilter only");
        _dispatcher.UnhandledExceptionFilter -= new DispatcherUnhandledExceptionFilterEventHandler(FilterException1);
        _dispatcher.UnhandledExceptionFilter -= new DispatcherUnhandledExceptionFilterEventHandler(FilterException2);
    }

    protected void FilterException1(object sender, DispatcherUnhandledExceptionFilterEventArgs args)
    {
        Console.WriteLine("    ExceptionFilter1");
        if (!(args.Exception is ArgumentException))
        {
            // unexpected, don't catch it
            args.RequestCatch = false;
            ReportFailure("ExceptionFilter1: unexpected exception, RequestCatch turned OFF");
        }

        if (args.RequestCatch)
        {
            ReportFailure("ExceptionFilter1: RequestCatch is ON even though there are no handlers");
        }
        else
        {
            Console.WriteLine("      passed: RequestCatch is OFF");
            Console.WriteLine("      Attempt to setting RequestCatch to ON");
            args.RequestCatch = true;
        }
    }

    protected void FilterException2(object sender, DispatcherUnhandledExceptionFilterEventArgs args)
    {
        Console.WriteLine("    ExceptionFilter2");
        if (args.RequestCatch)
        {
            ReportFailure("ExceptionFilter2: RequestCatch failed to stay OFF");
        }
        else
        {
            Console.WriteLine("      passed: RequestCatch is OFF");
        }
    }
}

// Test turning off args.RequestCatch when catch handlers exist.
public class ExceptionFilterTest2 : ExceptionTest
{
    public ExceptionFilterTest2()
    {
    }

    override protected string TestID
    {
        get
        {
            return "ExceptionFilter Test 2";
        }
    }

    override protected void RegisterHandlers()
    {
        Console.WriteLine("    Registering handlers: ExceptionFilters and DispatcherUnhandledException");
        _dispatcher.UnhandledExceptionFilter += new DispatcherUnhandledExceptionFilterEventHandler(FilterException1);
        _dispatcher.UnhandledExceptionFilter += new DispatcherUnhandledExceptionFilterEventHandler(FilterException2);
        _dispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(HandleDispatcherUnhandledException);
    }

    override protected void UnregisterHandlers()
    {
        Console.WriteLine("    Unregistering handlers: ExceptionFilters and DispatcherUnhandledException");
        _dispatcher.UnhandledExceptionFilter -= new DispatcherUnhandledExceptionFilterEventHandler(FilterException1);
        _dispatcher.UnhandledExceptionFilter -= new DispatcherUnhandledExceptionFilterEventHandler(FilterException2);
        _dispatcher.UnhandledException -= new DispatcherUnhandledExceptionEventHandler(HandleDispatcherUnhandledException);
    }

    protected void FilterException1(object sender, DispatcherUnhandledExceptionFilterEventArgs args)
    {
        Console.WriteLine("    ExceptionFilter1");
        if (!(args.Exception is ArgumentException))
        {
            // unexpected, don't catch it
            args.RequestCatch = false;
            ReportFailure("ExceptionFilter1: unexpected exception, RequestCatch turned OFF");
        }

        if (args.RequestCatch)
        {
            Console.WriteLine("      passed: RequestCatch is ON");
            Console.WriteLine("      Setting RequestCatch to OFF");
            args.RequestCatch = false;
        }
        else
        {
            ReportFailure("ExceptionFilter1: RequestCatch is OFF even though handlers exist");
        }
    }

    protected void FilterException2(object sender, DispatcherUnhandledExceptionFilterEventArgs args)
    {
        Console.WriteLine("    ExceptionFilter2");
        if (args.RequestCatch)
        {
            ReportFailure("ExceptionFilter2: RequestCatch failed to turn OFF");
        }
        else
        {
            Console.WriteLine("      passed: RequestCatch is OFF");
        }
    }

    protected void HandleDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
    {
        Console.WriteLine("    DispatcherUnhandledException handler");
        ReportFailure("HandleDispatcherUnhandledException: ExceptionFilter failed!\n" +
                      "You probably have a build problem where your WindowsBase.dll is not injected with exception filtering code.  " +
                      "Please check your build log.  Contact Microsoft if you need help.");
    }
}

// Test turning args.Handled ON and (not) OFF.
public class DispatcherUnhandledExceptionTest : ExceptionTest
{
    public DispatcherUnhandledExceptionTest()
    {
    }

    override protected string TestID
    {
        get
        {
            return "DispatcherUnhandledException Test";
        }
    }

    override protected void RegisterHandlers()
    {
        Console.WriteLine("    Registering handlers: ExceptionFilter and DispatcherUnhandledExceptions");
        _dispatcher.UnhandledExceptionFilter += new DispatcherUnhandledExceptionFilterEventHandler(FilterException);
        _dispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(HandleDispatcherUnhandledException1);
        _dispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(HandleDispatcherUnhandledException2);
        _dispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(HandleDispatcherUnhandledException3);
    }

    override protected void UnregisterHandlers()
    {
        Console.WriteLine("    Unregistering handlers: ExceptionFilter and DispatcherUnhandledExceptions");
        _dispatcher.UnhandledExceptionFilter -= new DispatcherUnhandledExceptionFilterEventHandler(FilterException);
        _dispatcher.UnhandledException -= new DispatcherUnhandledExceptionEventHandler(HandleDispatcherUnhandledException1);
        _dispatcher.UnhandledException -= new DispatcherUnhandledExceptionEventHandler(HandleDispatcherUnhandledException2);
        _dispatcher.UnhandledException -= new DispatcherUnhandledExceptionEventHandler(HandleDispatcherUnhandledException3);
    }

    protected void FilterException(object sender, DispatcherUnhandledExceptionFilterEventArgs args)
    {
        Console.WriteLine("    DispatcherUnhandledExceptionFilter");
        Console.WriteLine("      RequestCatch is " + (args.RequestCatch ? "ON" : "OFF"));
        if (!(args.Exception is ArgumentException))
        {
            // unexpected, don't catch it
            args.RequestCatch = false;
            ReportFailure("FilterException: unexpected exception, RequestCatch turned OFF");
        }
    }

    protected void HandleDispatcherUnhandledException1(object sender, DispatcherUnhandledExceptionEventArgs args)
    {
        Console.WriteLine("    DispatcherUnhandledException handler 1");
        if (args.Handled)
        {
            ReportFailure("HandleDispatcherUnhandledException1: exception already handled");
        }
        else
        {
            Console.WriteLine("      setting handled to true");
            args.Handled = true;
        }
    }

    virtual protected void HandleDispatcherUnhandledException2(object sender, DispatcherUnhandledExceptionEventArgs args)
    {
        Console.WriteLine("    DispatcherUnhandledException handler 2");
        if (args.Handled)
        {
            Console.WriteLine("      passed: exception already handled");
            Console.WriteLine("      attempt to set handled to false");
            args.Handled = false;
        }
        else
        {
            ReportFailure("HandleDispatcherUnhandledException2: exception not handled");
        }
    }

    virtual protected void HandleDispatcherUnhandledException3(object sender, DispatcherUnhandledExceptionEventArgs args)
    {
        Console.WriteLine("    DispatcherUnhandledException handler 3");
        if (args.Handled)
        {
            Console.WriteLine("      passed: exception already handled");
        }
        else
        {
            ReportFailure("HandleDispatcherUnhandledException3: exception not handled");
        }
    }
}
