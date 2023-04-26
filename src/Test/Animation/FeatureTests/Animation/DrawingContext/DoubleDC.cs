// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
     
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;


namespace Microsoft.Test.Animation
{
    public class OpenTheWindowDouble
    {
        public static object RunTest (object arg)
        {
            System.Windows.Interop.HwndSourceParameters parameters = new System.Windows.Interop.HwndSourceParameters("animation",400,400);           
            parameters.WindowStyle = 0x10CF0000;        
            DoubleDCTest.source = new System.Windows.Interop.HwndSource (parameters);
            DoubleDCTest.verifier.hWnd = DoubleDCTest.source.Handle;
            return arg;
        }
    }

    
    public class ShowVisualDouble
    {
        public static object RunTest(object arg)
        {
            System.Windows.Media.DrawingVisual rootVisual = new System.Windows.Media.DrawingVisual();
            System.Windows.Media.DrawingContext ctx = rootVisual.RenderOpen();

            DoubleAnimation anim = new DoubleAnimation();
            anim.To                 = 0;
            anim.BeginTime          = TimeSpan.FromMilliseconds(2000);
            anim.Duration           = new Duration(TimeSpan.FromMilliseconds(2000));
            anim.AutoReverse        = true;
            anim.FillBehavior       = FillBehavior.HoldEnd;

            SolidColorBrush scBrush = new SolidColorBrush(Colors.Blue);
            scBrush.Opacity = 1;
            scBrush.BeginAnimation(SolidColorBrush.OpacityProperty, anim);

            ctx.DrawRectangle (scBrush, null, new Rect (new Point (0,0), new Point (100, 100)));

            ctx.Close ();

            DoubleDCTest.hwndTarget.RootVisual = rootVisual;
            return arg;
        }
    }


    public class DoubleDCTest : AvalonTest
    {
        #region Instantiating the variables

        public DoubleDCTest()
        {
        }

        public Dispatcher s_dispatcher;
        public int hBmp;
        public static System.Windows.Interop.HwndSource source;
        public static System.Windows.Interop.HwndTarget hwndTarget
        {
            get
            {
                return source.CompositionTarget;
            }
        }
        public static VisualVerifier  verifier;
        private Double[]           _expValues       = {0.776,.518,.259,0};
        private bool               _testPassed      = true;
        private string             _resultInfo      = "Results: ";
        private int                _expCounter      = 0;
        private Color              _actColor;
        private float              _expTolerance    = .30f;
        private Clock              _tlc;
        private int                _iterations      = 1;

        #endregion


        private void TimeRepeat(object sender, EventArgs e)
        {
            if ((((Clock)sender).CurrentIteration != _iterations))
            {
                GetData();
                _iterations++;
            }
        }

        private void TimeOver(object sender, EventArgs e)
        {
            if (((Clock)sender).CurrentState != ClockState.Active)
            {
                GetData();

                GlobalLog.LogEvidence(_resultInfo);

                Signal("TestFinished", TestResult.Pass);
            }
        }

        private void GetData()
        {
            if (_expCounter < 4)
            {
                _actColor = verifier.getColorAtPoint(75,75);   

                double floatValue = Math.Abs((double)(Decimal.Round(_actColor.B,3) / 255));

                if (  Math.Abs(((floatValue - _expValues[_expCounter]))) >= _expTolerance)
                { 
                    _testPassed = false && _testPassed; 
                }
                _resultInfo += "\n--Tick #" + _expCounter + "------------ [Opacity At 75,75]";
                _resultInfo += "\n Actual: " + (Decimal.Round(_actColor.B,3) / 255) + "\n Expected:" + (_expValues[_expCounter]).ToString();
            }

            _expCounter ++;
        }

        public bool StartTest()
        {
            s_dispatcher = Dispatcher.CurrentDispatcher;

            GlobalLog.LogStatus("StartTest");
            verifier = new VisualVerifier();
            //verifier.InitRender(Window);

            ParallelTimeline tlb = new ParallelTimeline();
            tlb.BeginTime = TimeSpan.FromMilliseconds(2000);
            tlb.Duration = new Duration(TimeSpan.FromMilliseconds(502));
            tlb.RepeatBehavior = new RepeatBehavior(4);

            tlb.CurrentStateInvalidated += new EventHandler(TimeOver);
            tlb.CurrentTimeInvalidated += new EventHandler(TimeRepeat);
            _tlc = tlb.CreateClock(); 

            s_dispatcher.BeginInvoke(DispatcherPriority.Normal ,  new DispatcherOperationCallback(OpenTheWindowDouble.RunTest), null);
            s_dispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback (ShowVisualDouble.RunTest), null);

            WaitForSignal("TestFinished");

            GlobalLog.LogStatus("FinishedTest");
            
            return _testPassed;
        }
    }

}
