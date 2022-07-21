// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Threading;
using System.Windows;
using MS.Win32;
using DRT;
using System.Diagnostics;
using System.Runtime.InteropServices;

public class DispatcherTimerTest : DrtTestSuite
{
    public DispatcherTimerTest() : base ("DispatcherTimer")
    {
        this.TeamContact = "WPF";
        this.Contact = "Microsoft";
    }

    public override DrtTest[] PrepareTests()
    {
        return new DrtTest[]
        {
            new DrtTest(TestDispatcher),
            new DrtTest(TestDispatcherWithPrivatePump),
            new DrtTest(TestChangingTimer)
        };
    }

    public void TestDispatcher()
    {
        Run(false, true);
        Run(false, false);
    }

    public void TestDispatcherWithPrivatePump()
    {
        Run(true, true);
        Run(true, false);
    }

    public void TestChangingTimer()
    {
        Console.WriteLine("");
        Console.WriteLine("TestChangingTimer");
        
        DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal);
        timer.Tick += new EventHandler(OnChangingTimerTick);
        timer.Interval = TimeSpan.FromMinutes(1);

        _stopwatch = new Stopwatch();
        _stopwatch.Start();

        Console.WriteLine("Starting the timer for 60 seconds...");
        timer.Start();

        Console.WriteLine("Sleeping for 10ms ...");
        System.Threading.Thread.Sleep(10);

        Console.WriteLine("Changing the timer interval to 1ms...");
        timer.Interval = TimeSpan.FromMilliseconds(0);

        _dispatcherFrame2 = new DispatcherFrame();
        Dispatcher.PushFrame(_dispatcherFrame2);

        _stopwatch.Stop();
        Console.WriteLine("The timer took {0}", _stopwatch.Elapsed);
    }
    
    private void Run(bool usePrivateMessagePump, bool isWarmUp)
    {
        if(isWarmUp)
        {
            Console.WriteLine("Warming up...");
        }
        else
        {
            Console.WriteLine("Testing...");
        }
        
        _dispatcher = Dispatcher.CurrentDispatcher;
        _stopwatch = new Stopwatch();
        _isWarmUp = isWarmUp;
        
        // Execute 10 timers.
        DispatcherTimer[] timers = new DispatcherTimer[10];
        _numTicks = 0;
        TimeSpan timeout = TimeSpan.Zero;
        for(int i = 0; i < 10; i++)
        {
            timeout += TimeSpan.FromMilliseconds(10);

            timers[i] = new DispatcherTimer(DispatcherPriority.Normal);
            timers[i].Tick += new EventHandler(OnTick);
            timers[i].Interval = timeout;
            timers[i].Tag = i;
        }
        timeout += TimeSpan.FromMilliseconds(10);
        DispatcherTimer shutdownTimer = new DispatcherTimer(DispatcherPriority.Normal);
        shutdownTimer.Tick += new EventHandler(ShutDown);
        shutdownTimer.Interval = timeout;

        _stopwatch.Start();

        // Start all of the timers.
        for(int i = 0; i < 10; i++)
        {
            timers[i].Start();
        }        
        shutdownTimer.Start();

        RunDispatcher(usePrivateMessagePump);

        _stopwatch.Stop();

        if(!_isWarmUp)
        {
            // Require the correct number of timer callbacks.            
            if(_numTicks != 10)
            {
                throw new Exception("Wrong number of timer ticks: " + _numTicks);
            }

            TimeSpan lowerLimit = timeout - TimeSpan.FromMilliseconds(20);
            TimeSpan upperLimit = timeout + TimeSpan.FromMilliseconds(20);
            
            // Warn if the timers did not complete in +- 20ms.
            if(_stopwatch.Elapsed < lowerLimit || _stopwatch.Elapsed > upperLimit)
            {
                Console.WriteLine("Warning: Timer ticks did not happen within the expected window.  {0} not within [{1}...{2}]",
                                  _stopwatch.Elapsed,
                                  lowerLimit,
                                  upperLimit);
            }
        }
    }

    private void OnTick(object sender, EventArgs e)
    {
        DispatcherTimer timer = (DispatcherTimer) sender;
        timer.IsEnabled = false;

        if(_numTicks != (int)timer.Tag)
        {
            Console.WriteLine("ERROR: timer inversion!  Expecting {0} but got {1}", _numTicks, (int)timer.Tag);
            throw new Exception("ERROR: timer inversion");
        }
        
        _numTicks++;

        if(!_isWarmUp)
        {
            Console.WriteLine("OnTick {0} at {1}", _numTicks, _stopwatch.Elapsed);
        }
    }

    private void OnChangingTimerTick(object sender, EventArgs e)
    {
        DispatcherTimer timer = (DispatcherTimer) sender;
        timer.IsEnabled = false;

        _dispatcherFrame2.Continue = false;
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

    private void ShutDown(object sender, EventArgs e)
    {
        DispatcherTimer timer = (DispatcherTimer) sender;
        timer.IsEnabled = false;

        _dispatcherFrame.Continue = false;
    }

    private Dispatcher _dispatcher;
    private DispatcherFrame _dispatcherFrame;
    private DispatcherFrame _dispatcherFrame2;
    private int _numTicks;
    private Stopwatch _stopwatch;
    private bool _isWarmUp;
}

