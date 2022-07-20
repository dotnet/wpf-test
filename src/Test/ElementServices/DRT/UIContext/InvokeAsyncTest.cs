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
using System.Threading;
using System.Threading.Tasks;

public class InvokeAsyncTest : DrtTestSuite
{
    public InvokeAsyncTest() : base ("InvokeAsync")
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
        DispatcherModel model = new DispatcherModel();
        
        RunTestsAsync(model); // start tests async, return immediately
        
        model.Run(usePrivateMessagePump);
    }

    private async void RunTestsAsync(DispatcherModel model)
    {
        model.Start(Dispatcher.CurrentDispatcher);
        await TestInvokeAsyncPriorityOrder(model);
        await TestInvokePriorityOrder(model);
        await TestInvokeAsyncPreCancellation(model);
        await TestInvokeAsyncPostCancellation(model);
        await TestInvokeAsyncCooperativeCancellation(model);
        await TestInvokeAsyncException(model);
        await TestInvokeAsyncExceptionNoAwait(model);
        await TestInvokeAsyncYield(model);
        await TestInvokeInfiniteTimeout(model);
        await TestInvokeBadTimeout(model);
        await TestInvokeShortTimeout(model);
        await TestInvokeLongTimeout(model);
        model.Stop();
    }

    // InvokeAsync high, middle, and low priority work items, and make sure
    // they get invoked in the expected order.
    private async Task TestInvokeAsyncPriorityOrder(DispatcherModel model)
    {
        object ignoreAwait;

        DRT.LogOutput("TestInvokeAsyncPriorityOrder");
        
        model.Reset();
        
        ignoreAwait = model.InvokeAsync("lower", ()=>{}, DispatcherPriority.Background);
        ignoreAwait = model.InvokeAsync("higher", ()=>{}, DispatcherPriority.Normal);
        ignoreAwait = model.InvokeAsync("middle", ()=>{}, DispatcherPriority.Input);
        ignoreAwait = null;
        
        await model.FlushAsync();

        model.VerifyExecutionSequence(
            "P(lower),"+
            "P(higher),"+
            "P(middle),"+
            "S(higher),"+
            "C(higher),"+
            "S(middle),"+
            "C(middle),"+
            "S(lower),"+
            "C(lower)");
    }

    // InvokeAsync high and low priority work items, then Invoke one at a
    // middle priority.  Ensure that when the middle one is invoked that the
    // higher operation has already been invoked and that the lower operation
    // has not been invoked.  
    private async Task TestInvokePriorityOrder(DispatcherModel model)
    {
        object ignoreAwait;

        DRT.LogOutput("TestInvokePriorityOrder");
        model.Reset();
        
        ignoreAwait = model.InvokeAsync("lower", ()=>{}, DispatcherPriority.Background);
        ignoreAwait = model.InvokeAsync("higher", ()=>{}, DispatcherPriority.Normal);
        ignoreAwait = null;
        model.Invoke("middle", ()=>{}, DispatcherPriority.Input);
        
        model.VerifyExecutionSequence(
            "P(lower),"+
            "P(higher),"+
            "P(middle),"+
            "S(higher),"+
            "C(higher),"+
            "S(middle),"+
            "C(middle)");

        await model.FlushAsync();

        model.VerifyExecutionSequence(
            "P(lower),"+
            "P(higher),"+
            "P(middle),"+
            "S(higher),"+
            "C(higher),"+
            "S(middle),"+
            "C(middle),"+
            "S(lower),"+
            "C(lower)");
    }
    
    // Post higher, middle, and lower priority work items, and pass the
    // middle operation a cancellation token.  Have the higher operation
    // cancel the token.  This should abort the middle operation before
    // it can execute.
    private async Task TestInvokeAsyncPreCancellation(DispatcherModel model)
    {
        object ignoreAwait;

        DRT.LogOutput("TestInvokeAsyncPreCancellation");
        model.Reset();
        
        CancellationTokenSource cts = new CancellationTokenSource();

        ignoreAwait = model.InvokeAsync("lower", ()=>{}, DispatcherPriority.Background);
        ignoreAwait = model.InvokeAsync("higher", ()=>cts.Cancel(), DispatcherPriority.Normal);
        ignoreAwait = model.InvokeAsync("middle", ()=>ShouldNeverRun(), DispatcherPriority.Input, cts.Token);
        ignoreAwait = null;
        
        try
        {
            await model.FlushAsync();

            // Failure
            throw new InvalidOperationException("Should have seen a TaskCanceledException!");
        }
        catch(TaskCanceledException)
        {
            // Success...
        }

        model.VerifyExecutionSequence(
            "P(lower),"+
            "P(higher),"+
            "P(middle),"+
            "S(higher),"+
            "A(middle),"+
            "C(higher),"+
            "S(lower),"+
            "C(lower)");
    }

    // InvokeAsync higher, middle, and lower priority work items, and pass the
    // high operation a cancellation token.  Have the middle operation cancel
    // the token.  This should have no effect, since the higher operation
    // should have already completed by the time the middle operation canceled
    // canceled the token.
    private async Task TestInvokeAsyncPostCancellation(DispatcherModel model)
    {
        object ignoreAwait;
        
        DRT.LogOutput("TestInvokeAsyncPostCancellation");
        model.Reset();
        
        CancellationTokenSource cts = new CancellationTokenSource();

        ignoreAwait = model.InvokeAsync("lower", ()=>{}, DispatcherPriority.Background);
        ignoreAwait = model.InvokeAsync("higher", ()=>{}, DispatcherPriority.Normal, cts.Token);
        ignoreAwait = model.InvokeAsync("middle", ()=>cts.Cancel(), DispatcherPriority.Input);
        ignoreAwait = null;
        
        await model.FlushAsync();

        model.VerifyExecutionSequence(
            "P(lower),"+
            "P(higher),"+
            "P(middle),"+
            "S(higher),"+
            "C(higher),"+
            "S(middle),"+
            "C(middle),"+
            "S(lower),"+
            "C(lower)");
    }

    // InvokeAsync higher, middle, and lower priority work items, and pass the
    // middle operation a cancellation token.  Have the middle operation spin for
    // 5 seconds checking the cancellation token and throwing the proper
    // OperationCanceledException when cancellation is detected.  Use a
    // 1-second timer to cancel the token source.  The middle operation should
    // be reported as aborted.
    private async Task TestInvokeAsyncCooperativeCancellation(DispatcherModel model)
    {
        object ignoreAwait;

        DRT.LogOutput("TestInvokeAsyncCooperativeCancellation");
        model.Reset();
        
        CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

        ignoreAwait = model.InvokeAsync("lower", ()=>{}, DispatcherPriority.Background);
        ignoreAwait = model.InvokeAsync("higher", ()=>{}, DispatcherPriority.Normal);
        DispatcherOperation op = model.InvokeAsync("middle", ()=>FiveSecondLoop(cts.Token), DispatcherPriority.Input);
        ignoreAwait = null;
        
        try
        {
            await model.FlushAsync();

            // Failure
            throw new InvalidOperationException("Should have seen a TaskCanceledException!");
        }
        catch(TaskCanceledException)
        {
            // Success...
        }

        model.VerifyExecutionSequence(
            "P(lower),"+
            "P(higher),"+
            "P(middle),"+
            "S(higher),"+
            "C(higher),"+
            "S(middle),"+ // OperationCanceledException thrown in operation...
            "C(middle),"+ // Operations completes in the aborted state
            "S(lower),"+
            "C(lower)");

        if(op.Status != DispatcherOperationStatus.Aborted)
        {
            throw new InvalidOperationException("Operation status should be aborted.");
        }

        if(op.Task.Status != TaskStatus.Canceled)
        {
            throw new InvalidOperationException("Task status should be canceled.");
        }
    }

    // InvokeAsync higher, middle, and lower priority work items, and have the
    // middle operation throw an exception.  The middle operation should
    // be reported as completed.
    private async Task TestInvokeAsyncException(DispatcherModel model)
    {
        object ignoreAwait;

        DRT.LogOutput("TestInvokeAsyncException");
        model.Reset();
        
        CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

        ignoreAwait = model.InvokeAsync("lower", ()=>{}, DispatcherPriority.Background);
        ignoreAwait = model.InvokeAsync("higher", ()=>{}, DispatcherPriority.Normal);
        ignoreAwait = null;

        DispatcherOperation op = model.InvokeAsync("middle", ()=>{throw new TestException();}, DispatcherPriority.Input);
        try
        {
            await model.FlushAsync();
            
            // Failure
            throw new InvalidOperationException("The test exception should have been thrown!");
        }
        catch(TestException)
        {
            // Success
        }

        model.VerifyExecutionSequence(
            "P(lower),"+
            "P(higher),"+
            "P(middle),"+
            "S(higher),"+
            "C(higher),"+
            "S(middle),"+ // TestException thrown in operation...
            "C(middle),"+ // Operations completes, but task is faulted
            "S(lower),"+
            "C(lower)");

        if(op.Status != DispatcherOperationStatus.Completed)
        {
            throw new InvalidOperationException("Operation status should be completed.");
        }

        if(op.Task.Status != TaskStatus.Faulted)
        {
            throw new InvalidOperationException("Task status should be faulted.");
        }
    }

    // InvokeAsync higher, middle, and lower priority work items, and have the
    // middle operation throw an exception.  The exception should be reported to
    // the TaskScheduler.UnobservedTaskException handler by the middle Task's
    // finalizer.
    private async Task TestInvokeAsyncExceptionNoAwait(DispatcherModel model)
    {
        object ignoreAwait;

        DRT.LogOutput("TestInvokeAsyncExceptionNoWait");
        model.Reset();
        
        CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

        ignoreAwait = model.InvokeAsync("lower", ()=>{}, DispatcherPriority.Background);
        ignoreAwait = model.InvokeAsync("higher", ()=>{}, DispatcherPriority.Normal);

        // Do not name the middle operation, so we won't await it.
        ignoreAwait = model.InvokeAsync(null, ()=>{throw new TestException();}, DispatcherPriority.Input);
        ignoreAwait = null;

        try
        {
            await model.FlushAsync();
        }
        catch(TestException)
        {
            // Failure, this exception should not have been observed.
            throw new InvalidOperationException("The test exception should not have been observed!");
        }

        model.VerifyExecutionSequence(
            "P(lower),"+
            "P(higher),"+
            "S(higher),"+
            "C(higher),"+
            "S(lower),"+
            "C(lower),"+
            "UTE(System.AggregateException)" // The exception thrown from the middle operation is reported as unobserved.
            );
    }

    // InvokeAsync higher, middle, and lower priority work items, and have the
    // middle operation Yield to background priority.  The background priority
    // work should complete before the middle item completes.
    private async Task TestInvokeAsyncYield(DispatcherModel model)
    {
        object ignoreAwait;

        DRT.LogOutput("TestInvokeAsyncExceptionNoWait");
        model.Reset();
        
        CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

        ignoreAwait = model.InvokeAsync("idle", ()=>{}, DispatcherPriority.SystemIdle);
        ignoreAwait = model.InvokeAsync("lower", ()=>{}, DispatcherPriority.Background);
        ignoreAwait = model.InvokeAsync("higher", ()=>{}, DispatcherPriority.Normal);
        ignoreAwait = model.InvokeAsync("middle", async delegate()
            {
                model.Log("BeforeYield");
                await Dispatcher.Yield();
                model.Log("AfterYield");
            }, DispatcherPriority.Input);
        ignoreAwait = null;

        await model.FlushAsync();

        model.VerifyExecutionSequence(
            "P(idle),"+
            "P(lower),"+
            "P(higher),"+
            "P(middle),"+
            "S(higher),"+
            "C(higher),"+
            "S(middle),"+
            "BeforeYield,"+
            "C(middle),"+ // completing middle because at await Dispatcher.Yield
            "S(lower),"+ // part of yielding
            "C(lower),"+ // part of yielding
            "AfterYield,"+ // back to middle operation
            "S(idle),"+
            "C(idle)"
            );
    }

    // InvokeAsync high and low priority work items, then Invoke one at a
    // middle priority with an infinite timeout. Ensure that the middle one
    // invokes, that the higher operation has already been invoked and that
    // the lower operation has not been invoked.  
    private async Task TestInvokeInfiniteTimeout(DispatcherModel model)
    {
        object ignoreAwait;

        DRT.LogOutput("TestInvokeInfiniteTimeout");
        model.Reset();
        
        ignoreAwait = model.InvokeAsync("lower", ()=>{}, DispatcherPriority.Background);
        ignoreAwait = model.InvokeAsync("higher", ()=>{}, DispatcherPriority.Normal);
        ignoreAwait = null;
        
        try
        {
            CancellationToken ct = CancellationToken.None;
            
            model.Invoke(
                "middle",
                ()=>FiveSecondLoop(ct),
                DispatcherPriority.Input,
                ct,
                TimeSpan.FromMilliseconds(-1));
            
            // Success
        }
        catch(TimeoutException)
        {
            throw new InvalidOperationException("Invoke should have not have timed out!");
        }

        model.VerifyExecutionSequence(
            "P(lower),"+
            "P(higher),"+
            "P(middle),"+
            "S(higher),"+
            "C(higher),"+
            "S(middle),"+
            "C(middle)");

        await model.FlushAsync();

        model.VerifyExecutionSequence(
            "P(lower),"+
            "P(higher),"+
            "P(middle),"+
            "S(higher),"+
            "C(higher),"+
            "S(middle),"+
            "C(middle),"+
            "S(lower),"+
            "C(lower)");
    }

    // InvokeAsync high and low priority work items, then Invoke one at a
    // middle priority with a bad timeout. Ensure that the middle one fails to
    // invoke, that the higher operation has already been invoked and that
    // the lower operation has not been invoked.  
    private async Task TestInvokeBadTimeout(DispatcherModel model)
    {
        object ignoreAwait;
        
        DRT.LogOutput("TestInvokeBadTimeout");
        model.Reset();
        
        ignoreAwait = model.InvokeAsync("lower", ()=>{}, DispatcherPriority.Background);
        ignoreAwait = model.InvokeAsync("higher", ()=>{}, DispatcherPriority.Normal);
        ignoreAwait = null;
        
        try
        {
            CancellationToken ct = CancellationToken.None;
            
            model.Invoke(
                "middle",
                ()=>FiveSecondLoop(ct),
                DispatcherPriority.Input,
                ct,
                TimeSpan.FromMilliseconds(-2));
            
            // Failure
            throw new InvalidOperationException("Invoke should have given an error due to bad timeout!");
        }
        catch(ArgumentOutOfRangeException)
        {
            // Success
        }

        model.VerifyExecutionSequence(
            "P(lower),"+
            "P(higher)");

        await model.FlushAsync();

        model.VerifyExecutionSequence(
            "P(lower),"+
            "P(higher),"+
            "S(higher),"+
            "C(higher),"+
            "S(lower),"+
            "C(lower)");
    }

    // InvokeAsync high and low priority work items, then Invoke one at a
    // middle priority with a 1s timeout. Ensure that the middle one fails to
    // invoke, that the higher operation has already been invoked and that
    // the lower operation has not been invoked.  
    private async Task TestInvokeShortTimeout(DispatcherModel model)
    {
        object ignoreAwait;
        
        DRT.LogOutput("TestInvokeShortTimeout");
        model.Reset();
        
        ignoreAwait = model.InvokeAsync("lower", ()=>{}, DispatcherPriority.Background);
        ignoreAwait = model.InvokeAsync("higher", ()=>{}, DispatcherPriority.Normal);
        ignoreAwait = null;

        try
        {
            CancellationToken ct = CancellationToken.None;
            
            model.Invoke(
                "middle",
                ()=>FiveSecondLoop(ct),
                DispatcherPriority.Input,
                ct,
                TimeSpan.FromSeconds(1));
            
            // Success
            // The 1s timeout is long enough to start the operation, and then we have
            // to wait for it to complete.
        }
        catch(TimeoutException)
        {
            // Failure
            throw new InvalidOperationException("Invoke should have timed out!");
        }

        model.VerifyExecutionSequence(
            "P(lower),"+
            "P(higher),"+
            "P(middle),"+
            "S(higher),"+
            "C(higher),"+
            "S(middle),"+
            "C(middle)");

        await model.FlushAsync();

        model.VerifyExecutionSequence(
            "P(lower),"+
            "P(higher),"+
            "P(middle),"+
            "S(higher),"+
            "C(higher),"+
            "S(middle),"+
            "C(middle),"+
            "S(lower),"+
            "C(lower)");
    }

    // InvokeAsync high and low priority work items, then Invoke one at a
    // middle priority with a 10s timeout. Ensure that the middle one invokes
    // invoke, that the higher operation has already been invoked and that
    // the lower operation has not been invoked.  
    private async Task TestInvokeLongTimeout(DispatcherModel model)
    {
        object ignoreAwait;
        
        DRT.LogOutput("TestInvokeLongTimeout");
        model.Reset();
        
        ignoreAwait = model.InvokeAsync("lower", ()=>{}, DispatcherPriority.Background);
        ignoreAwait = model.InvokeAsync("higher", ()=>{}, DispatcherPriority.Normal) as object;
        ignoreAwait = null;
        
        try
        {
            CancellationToken ct = CancellationToken.None;
            
            model.Invoke(
                "middle",
                ()=>FiveSecondLoop(ct),
                DispatcherPriority.Input,
                ct,
                TimeSpan.FromSeconds(10));
            
            // Success
        }
        catch(TimeoutException)
        {
            throw new InvalidOperationException("Invoke should have not have timed out!");
        }

        model.VerifyExecutionSequence(
            "P(lower),"+
            "P(higher),"+
            "P(middle),"+
            "S(higher),"+
            "C(higher),"+
            "S(middle),"+
            "C(middle)");

        await model.FlushAsync();

        model.VerifyExecutionSequence(
            "P(lower),"+
            "P(higher),"+
            "P(middle),"+
            "S(higher),"+
            "C(higher),"+
            "S(middle),"+
            "C(middle),"+
            "S(lower),"+
            "C(lower)");
    }

    private void ShouldNeverRun()
    {
        throw new InvalidOperationException("This callback should never run!");
    }

    private void FiveSecondLoop(CancellationToken ct)
    {
        DateTime start = DateTime.Now;
        while((DateTime.Now - start).TotalSeconds < 5.0)
        {
            ct.ThrowIfCancellationRequested();
            Thread.Sleep(TimeSpan.FromMilliseconds(100));
        }
    }
}

public class TestException : ApplicationException
{
    public TestException() : base("test exception")
    {
    }

    public TestException(string message) : base(message)
    {
    }
}

