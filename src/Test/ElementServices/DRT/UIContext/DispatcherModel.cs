// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Windows.Threading;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MS.Win32;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Linq;

public class DispatcherModel
{
    public DispatcherModel()
    {
    }
    
    public void Start(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        
        _dispatcher.Hooks.OperationPosted += OnOperationPosted;
        _dispatcher.Hooks.OperationStarted += OnOperationStarted;
        _dispatcher.Hooks.OperationCompleted += OnOperationCompleted;
        _dispatcher.Hooks.OperationPriorityChanged += OnOperationPriorityChanged;
        _dispatcher.Hooks.OperationAborted += OnOperationAborted;
        _dispatcher.UnhandledException += OnDispatcherUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

        _namedOperations = new Dictionary<DispatcherOperation, string>();
        ExecutionSequence = String.Empty;
    }

    public void Stop()
    {
        if(_dispatcherFrame == null)
        {
            throw new InvalidOperationException("DispatcherModel must be running to call Stop.");
        }
        
        _dispatcherFrame.Continue = false;

        _dispatcher.Hooks.OperationPosted -= OnOperationPosted;
        _dispatcher.Hooks.OperationStarted -= OnOperationStarted;
        _dispatcher.Hooks.OperationCompleted -= OnOperationCompleted;
        _dispatcher.Hooks.OperationPriorityChanged -= OnOperationPriorityChanged;
        _dispatcher.Hooks.OperationAborted -= OnOperationAborted;
        _dispatcher.UnhandledException -= OnDispatcherUnhandledException;
        TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;

        _namedOperations = null;
        ExecutionSequence = null;
    }
    

    public async Task FlushAsync()
    {
        foreach(DispatcherOperation operation in _namedOperations.Keys)
        {
            await operation;
        }

        // Force a GC in order to flush out any unobserved exceptions.
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }
    
    public void Reset()
    {
        foreach(DispatcherOperation operation in _namedOperations.Keys)
        {
            if(operation.Status == DispatcherOperationStatus.Pending || 
               operation.Status == DispatcherOperationStatus.Executing)
            {
                throw new InvalidOperationException("The model must be flushed before Reset may be called.");
            }
        }

        _namedOperations.Clear();
        ExecutionSequence = String.Empty;
    }

    public void Log(string message)
    {
        AppendStep(message);
    }
    
    public DispatcherOperation InvokeAsync(string name, Action callback, DispatcherPriority priority)
    {
        _pendingOperationName = name;
        try
        {
            DispatcherOperation operation = _dispatcher.InvokeAsync(callback, priority);
            return operation;
        }
        finally
        {
            _pendingOperationName = null;
        }
    }
    
    public DispatcherOperation InvokeAsync(string name, Action callback, DispatcherPriority priority, CancellationToken ct)
    {
        _pendingOperationName = name;
        try
        {
            DispatcherOperation operation = _dispatcher.InvokeAsync(callback, priority, ct);
            return operation;
        }
        finally
        {
            _pendingOperationName = null;
        }
    }

    public DispatcherOperation<T> InvokeAsync<T>(string name, Func<T> callback, DispatcherPriority priority)
    {
        _pendingOperationName = name;
        try
        {
            DispatcherOperation<T> operation = _dispatcher.InvokeAsync(callback, priority);
            return operation;
        }
        finally
        {
            _pendingOperationName = null;
        }
    }
    
    public DispatcherOperation<T> InvokeAsync<T>(string name, Func<T> callback, DispatcherPriority priority, CancellationToken ct)
    {
        _pendingOperationName = name;
        try
        {
            DispatcherOperation<T> operation = _dispatcher.InvokeAsync(callback, priority, ct);
            return operation;
        }
        finally
        {
            _pendingOperationName = null;
        }
    }

    public void Invoke(string name, Action callback, DispatcherPriority priority)
    {
        _pendingOperationName = name;
        try
        {
            _dispatcher.Invoke(callback, priority);
        }
        finally
        {
            _pendingOperationName = null;
        }
    }

    public void Invoke(string name, Action callback, DispatcherPriority priority, CancellationToken ct)
    {
        _pendingOperationName = name;
        try
        {
            _dispatcher.Invoke(callback, priority, ct);
        }
        finally
        {
            _pendingOperationName = null;
        }
    }

    public void Invoke(string name, Action callback, DispatcherPriority priority, CancellationToken ct, TimeSpan timeout)
    {
        _pendingOperationName = name;
        try
        {
            _dispatcher.Invoke(callback, priority, ct, timeout);
        }
        finally
        {
            _pendingOperationName = null;
        }
            
    }
    
    public string ExecutionSequence {get; private set;}

    public void VerifyExecutionSequence(string sequence)
    {
        if(ExecutionSequence != sequence)
        {
            throw new InvalidOperationException("Execution sequence incorrect!  Expected=\""+sequence+"\".  Actual=\"" + ExecutionSequence + "\".");
        }
    }

    public void Run(bool usePrivateMessagePump)
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

        _dispatcherFrame = null;
    }
    
    private void OnOperationPosted(object sender, DispatcherHookEventArgs e)
    {
        string name = GetOperationName(e.Operation);
        if(name != null)
        {
            AppendStep("P(" + name +")");
        }
    }

    private void OnOperationStarted(object sender, DispatcherHookEventArgs e)
    {
        string name = GetOperationName(e.Operation);
        if(name != null)
        {
            AppendStep("S(" + name +")");
        }
    }

    private void OnOperationCompleted(object sender, DispatcherHookEventArgs e)
    {
        string name = GetOperationName(e.Operation);
        if(name != null)
        {
            AppendStep("C(" + name +")");
        }
    }

    private void OnOperationPriorityChanged(object sender, DispatcherHookEventArgs e)
    {
        string name = GetOperationName(e.Operation);
        if(name != null)
        {
            AppendStep("PC(" + name +")");
        }
    }

    private void OnOperationAborted(object sender, DispatcherHookEventArgs e)
    {
        string name = GetOperationName(e.Operation);
        if(name != null)
        {
            AppendStep("A(" + name +")");
        }
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        AppendStep("UE(" + e.Exception.GetType() +")");
    }

    private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        AppendStep("UTE(" + e.Exception.GetType() +")");
    }

    private string GetOperationName(DispatcherOperation operation)
    {
        string name = null;
        
        if(_namedOperations.ContainsKey(operation))
        {
            // We already have a name for this operation.
            name = _namedOperations[operation];
        }
        else
        {
            // We haven't seen this operation yet, check if there is a pending
            // name for it.  If so, add it to our name dictionary.
            if(_pendingOperationName != null)
            {
                name = _pendingOperationName;
                _pendingOperationName = null;
                _namedOperations[operation] = name;
            }
        }

        return name;
    }
    
    private void AppendStep(string step)
    {
        if(ExecutionSequence == String.Empty)
        {
            ExecutionSequence = step;
        }
        else
        {
            ExecutionSequence = ExecutionSequence + "," + step;
        }
    }
    
    private Dispatcher _dispatcher;
    private Dictionary<DispatcherOperation, string> _namedOperations;
    private string _pendingOperationName;
    private DispatcherFrame _dispatcherFrame;
}

