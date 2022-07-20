// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Threading;
using System.Windows;
using MS.Win32;
using DRT;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;

public class BeginInvokeTest : DrtTestSuite
{
    public BeginInvokeTest() : base ("BeginInvoke")
    {
        this.TeamContact = "WPF";
        this.Contact = "Microsoft";
    }

    public override DrtTest[] PrepareTests()
    {
        return new DrtTest[]
        {
            new DrtTest(Test),
            new DrtTest(TestWithPrivatePump),
        };
    }

    public void Test()
    {
        Run(false);
    }

    public void TestWithPrivatePump()
    {
        Run(true);
    }
    
    private void Run(bool usePrivateMessagePump)
    {
        _dispatcher = Dispatcher.CurrentDispatcher;
        _dispatcher.Hooks.DispatcherInactive += new EventHandler(OnDispatcherInactive);
        _dispatcher.Hooks.OperationPosted += new DispatcherHookEventHandler(OnOperationPosted);
        _dispatcher.Hooks.OperationCompleted += new DispatcherHookEventHandler(OnOperationCompleted);
        _dispatcher.Hooks.OperationPriorityChanged += new DispatcherHookEventHandler(OnOperationPriorityChanged);
        _dispatcher.Hooks.OperationAborted += new DispatcherHookEventHandler(OnOperationAborted);

        _numInvokes = 0;

        Test0Param((Action)NoReflectionCallback);
        Test0Param((FakeAction)ReflectionCallback);

        Test1Param((DispatcherOperationCallback)delegate(object unused){NoReflectionCallback();return null;});
        Test1Param((Action<string>)((x)=>{ReflectionCallback();}));
        
        // Post an item to stop the Dispatcher at the end...
        _dispatcher.BeginInvoke(StopDispatcher);

        // Run Dispatcher
        RunDispatcher(usePrivateMessagePump);

        _dispatcher.Hooks.DispatcherInactive -= new EventHandler(OnDispatcherInactive);
        _dispatcher.Hooks.OperationPosted -= new DispatcherHookEventHandler(OnOperationPosted);
        _dispatcher.Hooks.OperationCompleted -= new DispatcherHookEventHandler(OnOperationCompleted);
        _dispatcher.Hooks.OperationPriorityChanged -= new DispatcherHookEventHandler(OnOperationPriorityChanged);
        _dispatcher.Hooks.OperationAborted -= new DispatcherHookEventHandler(OnOperationAborted);
    }

    private void Test0Param(Delegate callback)
    {
        bool succeeded = false;
        int numInvokes = 0;
        
        // This is one of the "old" APIs that has no parameters.
        numInvokes = _numInvokes;
        _dispatcher.BeginInvoke(DispatcherPriority.Normal, callback);
        _dispatcher.Invoke(DispatcherPriority.Normal, callback);
        _dispatcher.Invoke(DispatcherPriority.Normal, TimeSpan.FromMilliseconds(-1), callback);
        if(_numInvokes != numInvokes + 3)
        {
            throw new Exception("ERROR: the [Begin]Invoke(DispatcherPriority,Delegate) callbacks were not invoked.");
        }
        
        // This is one of the "old" APIs that explicitly takes
        // a single parameter.  We should _not_ be able to call
        // a 0-parameter delegate via this method.
        try
        {
            // We only call the synchronous version here so we can
            // catch the expected exception.
            succeeded = false;
            numInvokes = _numInvokes;
            _dispatcher.Invoke(DispatcherPriority.Normal, callback, null);
        }
        catch(Exception)
        {
            succeeded = true;
        }        
        if(!succeeded)
        {
            throw new Exception("ERROR: the Invoke(DispatcherPriority,Delegate,object) method should have thrown an exception.");
        }
        if(_numInvokes != numInvokes)
        {
            throw new Exception("ERROR: the Invoke(DispatcherPriority,Delegate,object) method should not have invoked the callback.");
        }
        
        // This is one of the "old" APIs that takes one explicit parameter
        // and then an array of additional parameters.  We should not be
        // able to call a 0-parameter delegate via this method.
        try
        {
            // We only call the synchronous version here so we can
            // catch the expected exception.
            succeeded = false;
            numInvokes = _numInvokes;
            _dispatcher.Invoke(DispatcherPriority.Normal, callback, null, null);
        }
        catch(Exception)
        {
            succeeded = true;
        }        
        if(!succeeded)
        {
            throw new Exception("ERROR: the Invoke(DispatcherPriority,Delegate,object,object[]) method should have thrown an exception.");
        }
        if(_numInvokes != numInvokes)
        {
            throw new Exception("ERROR: the Invoke(DispatcherPriority,Delegate,object,object[]) method should not have invoked the callback.");
        }
        

        // This is a "new" API that just takes a "params object[]".  We
        // should be able to call this method without passing any arguments.
        numInvokes = _numInvokes;
        _dispatcher.BeginInvoke(callback);
        _dispatcher.BeginInvoke(callback, DispatcherPriority.Normal);
        _dispatcher.Invoke(callback);
        _dispatcher.Invoke(callback, DispatcherPriority.Normal);
        _dispatcher.Invoke(callback, TimeSpan.FromMilliseconds(-1));
        _dispatcher.Invoke(callback, TimeSpan.FromMilliseconds(-1), DispatcherPriority.Normal);
        if(_numInvokes != numInvokes + 6)
        {
            throw new Exception("ERROR: the [Begin]Invoke(Delegate,object[]) callbacks were not invoked when not passing any arguments.");
        }

        // This is a "new" API that just takes a "params object[]".  We
        // should be able to call this method explicitly passing an empty
        // array.
        numInvokes = _numInvokes;
        object[] noArgs = new object[0];
        _dispatcher.BeginInvoke(callback, noArgs);
        _dispatcher.BeginInvoke(callback, DispatcherPriority.Normal, noArgs);
        _dispatcher.Invoke(callback, noArgs);
        _dispatcher.Invoke(callback, DispatcherPriority.Normal, noArgs);
        _dispatcher.Invoke(callback, TimeSpan.FromMilliseconds(-1), noArgs);
        _dispatcher.Invoke(callback, TimeSpan.FromMilliseconds(-1), DispatcherPriority.Normal, noArgs);
        if(_numInvokes != numInvokes + 6)
        {
            throw new Exception("ERROR: the [Begin]Invoke(Delegate,object[]) callbacks were not invoked when passing an empty array.");
        }
        
        // This is a "new" API that just takes a "params object[]".  We
        // should be able to call this method explicitly passing null.
        numInvokes = _numInvokes;
        noArgs = null;
        _dispatcher.BeginInvoke(callback, noArgs);
        _dispatcher.BeginInvoke(callback, DispatcherPriority.Normal, noArgs);
        _dispatcher.Invoke(callback, noArgs);
        _dispatcher.Invoke(callback, DispatcherPriority.Normal, noArgs);
        _dispatcher.Invoke(callback, TimeSpan.FromMilliseconds(-1), noArgs);
        _dispatcher.Invoke(callback, TimeSpan.FromMilliseconds(-1), DispatcherPriority.Normal, noArgs);
        if(_numInvokes != numInvokes + 6)
        {
            throw new Exception("ERROR: the [Begin]Invoke(Delegate,object[]) callbacks were not invoked when passing null.");
        }

        // This is a "new" API that just takes a "params object[]".  We
        // should not be able to call this method passing a parameter.
        try
        {
            // We only call one of the synchronous versions here so we can
            // catch the expected exception.
            succeeded = false;
            numInvokes = _numInvokes;
            _dispatcher.Invoke(callback, new object());
        }
        catch(Exception)
        {
            succeeded = true;
        }        
        if(!succeeded)
        {
            throw new Exception("ERROR: the Invoke(Delegate,object[]) method should have thrown an exception.");
        }
        if(_numInvokes != numInvokes)
        {
            throw new Exception("ERROR: the Invoke(Delegate,object[]) method should not have invoked the callback.");
        }
    }

    private void Test1Param(Delegate callback)
    {
        bool succeeded = false;
        int numInvokes = 0;
        string arg = "test";
        
        // This is one of the "old" APIs that has no parameters.  We should not
        // be able to invoke a 1-parameter delegate via this method.
        try
        {
            // We only call the synchronous version here so we can
            // catch the expected exception.
            succeeded = false;
            numInvokes = _numInvokes;
            _dispatcher.Invoke(DispatcherPriority.Normal, callback);
        }
        catch(Exception)
        {
            succeeded = true;
        }        
        if(!succeeded)
        {
            throw new Exception("ERROR: the Invoke(DispatcherPriority,Delegate) method should have thrown an exception.");
        }
        if(_numInvokes != numInvokes)
        {
            throw new Exception("ERROR: the Invoke(DispatcherPriority,Delegate) method should not have invoked the callback.");
        }
        
        // This is one of the "old" APIs that explicitly takes
        // a single parameter.  We should be able to call
        // a 1-parameter delegate via this method, passing the
        // directly parameter.
        numInvokes = _numInvokes;
        _dispatcher.BeginInvoke(DispatcherPriority.Normal, callback, arg);
        _dispatcher.BeginInvoke(DispatcherPriority.Normal, callback, null);
        _dispatcher.Invoke(DispatcherPriority.Normal, callback, arg);
        _dispatcher.Invoke(DispatcherPriority.Normal, callback, null);
        _dispatcher.Invoke(DispatcherPriority.Normal, TimeSpan.FromMilliseconds(-1), callback, arg);
        _dispatcher.Invoke(DispatcherPriority.Normal, TimeSpan.FromMilliseconds(-1), callback, null);
        if(_numInvokes != numInvokes + 6)
        {
            throw new Exception("ERROR: the [Begin]Invoke(DispatcherPriority,Delegate,object) callbacks were not invoked.");
        }

        // This is one of the "old" APIs that takes one explicit parameter
        // and then an array of additional parameters.  We should not be
        // able to call a 1-parameter delegate via this method.
        try
        {
            // We only call the synchronous version here so we can
            // catch the expected exception.
            succeeded = false;
            numInvokes = _numInvokes;
            _dispatcher.Invoke(DispatcherPriority.Normal, callback, arg, null);
        }
        catch(Exception)
        {
            succeeded = true;
        }        
        if(!succeeded)
        {
            throw new Exception("ERROR: the Invoke(DispatcherPriority,Delegate,object,object[]) method should have thrown an exception.");
        }
        if(_numInvokes != numInvokes)
        {
            throw new Exception("ERROR: the Invoke(DispatcherPriority,Delegate,object,object[]) method should not have invoked the callback.");
        }
        

        // This is a "new" API that just takes a "params object[]".  We
        // should not be able to call this method without passing an argument.
        try
        {
            // We only call the synchronous version here so we can
            // catch the expected exception.
            succeeded = false;
            numInvokes = _numInvokes;
            _dispatcher.Invoke(callback);
        }
        catch(Exception)
        {
            succeeded = true;
        }        
        if(!succeeded)
        {
            throw new Exception("ERROR: the Invoke(Delegate,object[]) method should have thrown an exception when not passing any arguments.");
        }
        if(_numInvokes != numInvokes)
        {
            throw new Exception("ERROR: the Invoke(Delegate,object[]) method should not have invoked the callback when not passing any arguments.");
        }

        // This is a "new" API that just takes a "params object[]".  We
        // should be able to call this method passing a single argument.
        numInvokes = _numInvokes;
        _dispatcher.BeginInvoke(callback, arg);
        _dispatcher.BeginInvoke(callback, DispatcherPriority.Normal, arg);
        _dispatcher.Invoke(callback, arg);
        _dispatcher.Invoke(callback, DispatcherPriority.Normal, arg);
        _dispatcher.Invoke(callback, TimeSpan.FromMilliseconds(-1), arg);
        _dispatcher.Invoke(callback, TimeSpan.FromMilliseconds(-1), DispatcherPriority.Normal, arg);
        if(_numInvokes != numInvokes + 6)
        {
            throw new Exception("ERROR: the [Begin]Invoke(Delegate,object[]) callbacks were not invoked when passing a single argument.");
        }

        // This is a "new" API that just takes a "params object[]".  We
        // should be able to call this method explicitly passing an 
        // array with a single item.
        numInvokes = _numInvokes;
        object[] args = new object[] {arg};
        _dispatcher.BeginInvoke(callback, args);
        _dispatcher.BeginInvoke(callback, DispatcherPriority.Normal, args);
        _dispatcher.Invoke(callback, args);
        _dispatcher.Invoke(callback, DispatcherPriority.Normal, args);
        _dispatcher.Invoke(callback, TimeSpan.FromMilliseconds(-1), args);
        _dispatcher.Invoke(callback, TimeSpan.FromMilliseconds(-1), DispatcherPriority.Normal, args);
        if(_numInvokes != numInvokes + 6)
        {
            throw new Exception("ERROR: the [Begin]Invoke(Delegate,object[]) callbacks were not invoked when passing an array with a single item.");
        }
        
        // This is a "new" API that just takes a "params object[]".  We
        // should not be able to call this method explicitly passing null.
        // This is because reflection assumes this means no arguments.
        try
        {
            // We only call one of the synchronous versions here so we can
            // catch the expected exception.
            succeeded = false;
            numInvokes = _numInvokes;
            _dispatcher.Invoke(callback, null);
        }
        catch(Exception)
        {
            succeeded = true;
        }        
        if(!succeeded)
        {
            throw new Exception("ERROR: the Invoke(Delegate,object[]) method should have thrown an exception when passing null.");
        }
        if(_numInvokes != numInvokes)
        {
            throw new Exception("ERROR: the Invoke(Delegate,object[]) method should not have invoked the callback when passing null.");
        }

        // This is a "new" API that just takes a "params object[]".  We
        // should not be able to call this method explicitly passing an.
        // empty array.  This is because reflection assumes this means no
        // arguments.
        try
        {
            // We only call one of the synchronous versions here so we can
            // catch the expected exception.
            succeeded = false;
            numInvokes = _numInvokes;
            _dispatcher.Invoke(callback, new object[0]);
        }
        catch(Exception)
        {
            succeeded = true;
        }        
        if(!succeeded)
        {
            throw new Exception("ERROR: the Invoke(Delegate,object[]) method should have thrown an exception when passing an empty array.");
        }
        if(_numInvokes != numInvokes)
        {
            throw new Exception("ERROR: the Invoke(Delegate,object[]) method should not have invoked the callback when passing an empty array.");
        }
    }
    
    private void NoReflectionCallback()
    {
        if(CheckForReflection())
        {
            throw new InvalidOperationException("Reflection detected!");
        }
        _numInvokes++;
    }

    private void ReflectionCallback()
    {
        if(!CheckForReflection())
        {
            throw new InvalidOperationException("No reflection detected!");
        }
        _numInvokes++;
    }

    private void StopDispatcher()
    {
        _dispatcherFrame.Continue = false;
    }
    
    private void OnDispatcherInactive(object sender, EventArgs e)
    {
        Console.WriteLine("<<Inactive>>");
    }

    private void OnOperationPosted(object sender, DispatcherHookEventArgs e)
    {
        Console.WriteLine("<<OperationPosted>>");
    }

    private void OnOperationCompleted(object sender, DispatcherHookEventArgs e)
    {
        Console.WriteLine("<<OperationCompleted>>");
    }

    private void OnOperationPriorityChanged(object sender, DispatcherHookEventArgs e)
    {
        Console.WriteLine("<<OperationPriorityChanged>>");
    }

    private void OnOperationAborted(object sender, DispatcherHookEventArgs e)
    {
        Console.WriteLine("<<OperationAborted>>");
    }
        
    private void RunDispatcher(bool usePrivateMessagePump)
    {
        _dispatcherFrame = new DispatcherFrame();
        
        if(usePrivateMessagePump)
        {
            System.Windows.Interop.MSG msg = new System.Windows.Interop.MSG();
            while(_dispatcherFrame.Continue && UnsafeNativeMethods.GetMessageW(ref msg, new HandleRef(null, IntPtr.Zero), 0, 0))
            {
                UnsafeNativeMethods.TranslateMessage(ref msg);
                UnsafeNativeMethods.DispatchMessage(ref msg);
            }
        }
        else
        {
            Dispatcher.PushFrame(_dispatcherFrame);
        }
    }

    private bool CheckForReflection()
    {
        StackTrace st = new StackTrace(false);
        for(int i = 0; i < st.FrameCount; i++)
        {
            StackFrame sf = st.GetFrame(i);
            MethodBase method = sf.GetMethod();
            if(method != null)
            {
                Type declaringType = method.DeclaringType;
                if(declaringType != null)
                {
                    string ns = declaringType.Namespace;
                    if(ns != null)
                    {
                        if(String.Compare(ns, "System.Reflection", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    private delegate void FakeAction();
    
    private Dispatcher _dispatcher;
    private DispatcherFrame _dispatcherFrame;
    private int _numInvokes;
}
