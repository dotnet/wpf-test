// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//  Description:    Developer Regression Test for the VisualTarget class. 
//
//

using DRT;
using System;
using System.Windows.Threading;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Interop;

public class DrtVisualTarget : DrtBase
{   
    [STAThread]
    public static int Main(string[] args)
    {
        DrtBase drt = new DrtVisualTarget();
        return drt.Run(args);
    }
    
    private DrtVisualTarget()
    {
        DrtName = "DrtVisualTarget";
        TeamContact = "WPF";
        Contact = "Microsoft";
        
        Suites = new DrtTestSuite[]
        {
            new BasicVisualTargetTest(),
        };
    }
}
 
public class BasicVisualTargetTest : DrtTestSuite
{
    #region Constants

    internal const int WM_CLOSE = 0x0010;
    internal const int WM_HELP = 0x0053;
    internal const int WM_LBUTTONDOWN = 0x0201;
    internal const int WM_TIMER = 0x0113;

    #endregion

    #region Rectangle

    public class Rectangle : DrawingVisual
    {
        public Rectangle(Color color, Rect rect, bool addText) : base()
        {
            DoubleAnimation a = new DoubleAnimation(1.0, 1.3, new TimeSpan(0));

            a.BeginTime = new TimeSpan(0, 0, 0, 0, 10);
            a.Duration = new TimeSpan(0, 0, 0, 0, 1000);
            a.RepeatBehavior = RepeatBehavior.Forever;
            a.AutoReverse = true;

            ScaleTransform t = new ScaleTransform(
                1.0,
                1.0,
                /* centerX = */ 70.0, /* centerY = */ 70.0
                );

            if (!s_noAnimation)
            {
                t.BeginAnimation(ScaleTransform.ScaleXProperty, a);
                t.BeginAnimation(ScaleTransform.ScaleYProperty, a);
            }

            DrawingContext ctx = RenderOpen();

            ctx.PushTransform(t);
            ctx.DrawRectangle(new SolidColorBrush(color), null, rect);
            if (addText)
            {
                ctx.DrawText(new FormattedText("Hello Possum", 
                                                        System.Globalization.CultureInfo.GetCultureInfo("en-US"), 
                                                        FlowDirection.LeftToRight, 
                                                        new Typeface("Arial"), 
                                                        25, 
                                                        new SolidColorBrush(Color.FromRgb(200,50,0))
                                                        ),
                                                new Point(rect.Right + 20, rect.Top)
                                                );
            }
            ctx.Pop();
            if (addText)
            {
                ctx.DrawText(new FormattedText("Static Text", 
                                               System.Globalization.CultureInfo.GetCultureInfo("en-US"), 
                                               FlowDirection.LeftToRight, 
                                               new Typeface("Arial"), 
                                               50, 
                                               new SolidColorBrush(Color.FromRgb(50,200,100))), 
                                               new Point(rect.Left, rect.Top + 150)
                                               );
            }
            ctx.Close();
        }
    }

    #endregion Rectangle

    #region Initialization

    public BasicVisualTargetTest() : base("BasicVisualTargetTest")
    {
        this.TeamContact = "WPF";
        this.Contact = "Microsoft";
    }

    public override DrtTest[] PrepareTests()
    {
        return new DrtTest[]
        {
            new DrtTest(Run),
        };
    }

    public override bool HandleCommandLineArgument(string arg, bool option, string[] args, ref int k)
    {
        // start by giving the base class the first chance
        if (base.HandleCommandLineArgument(arg, option, args, ref k))
        {
            return true;
        }

        // process your own arguments here, using these parameters:
        //      arg     - current argument
        //      option  - true if there was a leading - or /.
        //      args    - all arguments
        //      k       - current index into args
        // Here's a typical sketch:

        if (option)
        {
            switch (arg)    // arg is lower-case, no leading - or /
            {
                case "noanimation":
                    s_noAnimation = true;
                    break;

                default:                
                    // unknown option.  don't handle it
                    return false;
            }
            return true;
        }

        return false;
    }
    
    public override void PrintOptions()
    {
        Console.WriteLine("VisualTarget Options:");
        Console.WriteLine("  -noanimation      don't continuously disconnect/reconnect HostVisual/VisualTarget or animate rectangles");
        Console.WriteLine("                         - useful for TS testing");
        base.PrintOptions();
    }

    #endregion Initialization

    #region Execution

    public void Run()
    {
        s_dispatcher1 = Dispatcher.CurrentDispatcher;
        HwndSourceParameters param = new HwndSourceParameters("DrtVisualTarget", 500, 400);
        param.SetPosition(20, 20);

        s_hwndSource = new HwndSource(param);
        s_hwndSource.AddHook(new HwndSourceHook(ApplicationMessageFilter));
                   
        s_hwndSource.AddHook(new HwndSourceHook(ApplicationMessageFilter));
        
        Rectangle visual = new Rectangle(
            Color.FromRgb(0, 0, 255), 
            new Rect(20.0f, 20.0f, 100.0f, 100.0f),
            false
            );
        
        s_hwndSource.RootVisual = visual;

        s_hostVisual = new HostVisual();

        s_event = new AutoResetEvent(false);

        s_thread = new Thread(new ThreadStart(ThreadProc));
        s_thread.TrySetApartmentState(ApartmentState.STA);
        s_thread.Start();
        
        s_event.WaitOne();
                
        ((DrawingVisual)(s_hwndSource.RootVisual)).Children.Add(s_hostVisual);

        Rectangle childVisual = new Rectangle(
            Color.FromRgb(100, 100, 255),
            new Rect(60.0f, 60.0f, 50.0f, 50.0f),
            false
            );

        s_hostVisual.Children.Add(childVisual);

        SetTimer(s_hwndSource.Handle, 0, 200, IntPtr.Zero);

        Dispatcher.Run();

        s_hwndSource.Dispose();
    }

    [STAThread]
    static void ThreadProc()
    {
        s_dispatcher2 = Dispatcher.CurrentDispatcher;

        s_visualTarget = new VisualTarget(s_hostVisual);
                
        s_event.Set();

        Rectangle rectangle = new Rectangle(
            Color.FromRgb(0, 0, 188), 
            new Rect(80.0f, 80.0f, 150.0f, 150.0f),
            true
            );

        s_visualTarget.RootVisual = rectangle;

        Dispatcher.Run();
        
        s_visualTarget.Dispose();
    }

    #endregion Execution
    
    #region Miscellaneous
    
    [DllImport("User32.dll")]
    public static extern int SetTimer(
        IntPtr hWnd, 
        int nIDEvent, 
        int uElapse, 
        IntPtr lpTimerFunc
        ); 

    private static IntPtr ApplicationMessageFilter(
        IntPtr hwnd, 
        int message, 
        IntPtr wParam, 
        IntPtr lParam, 
        ref bool handled)
    {
        if (message == WM_CLOSE)
        {
            s_dispatcher1.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(Quit1), 
                null);
            
            s_dispatcher2.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(Quit2), 
                null);
            
            handled = true;
        }
        
        if (message == WM_TIMER)
        {
            s_dispatcher1.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(Action), 
                null);
            handled = true;
        }

        if (message == WM_HELP)
        {
            Thread.Sleep(500);
        }

        return IntPtr.Zero ;
    }

    private static object Quit1(object arg)
    {
        s_dispatcher1.InvokeShutdown();
        return null;
    }

    private static object Quit2(object arg)
    {
        s_dispatcher2.InvokeShutdown();
        return null;
    }

    private static object Action(object arg)
    {
        switch(s_step)
        {
            case 0:
            {
                break;
            }

            case 1:
            {
                ((DrawingVisual)(s_hwndSource.RootVisual)).Children.Remove(s_hostVisual);
                break;
            }

            case 2:
            {
                ((DrawingVisual)(s_hwndSource.RootVisual)).Children.Add(s_hostVisual);
                break;
            }

            case 3:
            {
                s_dispatcher2.BeginInvoke(
                    DispatcherPriority.Background,
                    (DispatcherOperationCallback)delegate(object args)
                    {
                        s_visualTarget.Dispose();
                        s_visualTarget = null;
                        return null;
                    },
                    null
                    );
            
                break;
            }

            case 4:
            {
                s_dispatcher2.BeginInvoke(
                    DispatcherPriority.Background,
                    (DispatcherOperationCallback)delegate(object args) 
                    {
                        s_visualTarget = new VisualTarget(s_hostVisual);

                        Rectangle visual = new Rectangle(
                            Color.FromRgb(0, 0, 188),
                            new Rect(80.0f, 80.0f, 150.0f, 150.0f),
                            true
                            );

                        s_visualTarget.RootVisual = visual;

                        return null;
                    },
                    null
                    );

                break;
            }

        case 5:
            {
                //s_dispatcher2.BeginInvokeShutdown(DispatcherPriority.Normal);

                break;
            }

        case 6:
            {
                s_dispatcher1.BeginInvoke(
                    DispatcherPriority.Background,
                    new DispatcherOperationCallback(Quit1),
                    null);

                s_dispatcher2.BeginInvoke(
                    DispatcherPriority.Background,
                    new DispatcherOperationCallback(Quit2),
                    null);
                break;
            }
        }

        // If run with noDisconnect flag, keep content static without adding
        // and removing it
        if (!s_noAnimation)
        {
            s_step++;
        }
        
        return null;
    }

    #endregion

    #region Static Fields

    static Dispatcher s_dispatcher1;
    static Dispatcher s_dispatcher2;
    static HwndSource s_hwndSource;
    
    static VisualTarget s_visualTarget;
    static HostVisual s_hostVisual;
    
    static int s_step;

    static Thread s_thread;
    static AutoResetEvent s_event;

    static bool s_noAnimation;

    #endregion
}




