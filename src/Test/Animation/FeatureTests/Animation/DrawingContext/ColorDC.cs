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
    public class OpenTheWindowColor
    {
        public static object RunTest (object arg)
        {
            System.Windows.Interop.HwndSourceParameters parameters = new System.Windows.Interop.HwndSourceParameters("animation",400,400);           
            parameters.WindowStyle = 0x10CF0000;
            ColorDCTest.source = new System.Windows.Interop.HwndSource (parameters);
            ColorDCTest.verifier.hWnd = ColorDCTest.source.Handle;
            return arg;
        }
    }

    
    public class ShowVisualColor
    {
        public static object RunTest(object arg)
        {
            GlobalLog.LogStatus("RunTest");

            System.Windows.Media.DrawingVisual rootVisual = new System.Windows.Media.DrawingVisual ();
            System.Windows.Media.DrawingContext ctx = rootVisual.RenderOpen();

            ColorAnimation anim = new ColorAnimation();
            anim.From               = Colors.Black;
            anim.To                 = Colors.Red;
            anim.BeginTime          = TimeSpan.FromMilliseconds(2000);
            anim.Duration           = new Duration(TimeSpan.FromMilliseconds(2000));
            anim.AutoReverse        = true;
            anim.FillBehavior       = FillBehavior.HoldEnd;

            SolidColorBrush scBrush = new SolidColorBrush(Colors.Black);
            scBrush.BeginAnimation(SolidColorBrush.ColorProperty, anim);

            ctx.DrawRectangle (scBrush, null, new Rect (new Point (10, 10), new Point (100, 100)));

            ctx.Close ();

            ColorDCTest.hwndTarget.RootVisual = rootVisual;
            return arg;
        }
    }


    public class ColorDCTest : AvalonTest
    {
        #region Instantiating the variables

        public ColorDCTest()
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
        public  static VisualVerifier verifier;
        private bool                _testPassed      = true;
        private string              _resultInfo      = "Results: ";
        private Color[]             _expColors       = {Color.FromArgb(255,123,0,0),Color.FromArgb(255,181,0,0),Color.FromArgb(255,222,0,0),Color.FromArgb(255,255,0,0),};
        private int                 _expCounter      = 0;
        private Color               _actColor;
        private float               _expTolerance    = .30f;
        private Clock               _tlc;
        private int                 _iterations      = 1;

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
            GlobalLog.LogStatus("GetData");

            if (_expCounter < 4)
            {
                _actColor = verifier.getColorAtPoint(50,50);   

                if (Math.Abs(Math.Round((double)(Decimal.Round(_actColor.R,3) - _expColors[_expCounter].R) / _expColors[_expCounter].R,4)) >= _expTolerance) 
                { 
                    _testPassed = false && _testPassed; 
                }
                _resultInfo += "\n--Tick #" + _expCounter + "------------ [At 50,50]";
                _resultInfo += "\n Actual Color: " + _actColor.ToString() + "\n Expected Color:" + (_expColors[_expCounter]).ToString() + "   " + _testPassed;
                
                _expCounter ++;
            }
        }

        public bool StartTest()
        {
            s_dispatcher = Dispatcher.CurrentDispatcher;

            GlobalLog.LogStatus("StartTest");
            verifier = new VisualVerifier();

            ParallelTimeline tlb = new ParallelTimeline();
            tlb.BeginTime           = TimeSpan.FromMilliseconds(2000);
            tlb.Duration            = new Duration(TimeSpan.FromMilliseconds(501));
            tlb.RepeatBehavior      = new RepeatBehavior(4);

            tlb.CurrentStateInvalidated += new EventHandler(TimeOver);
            tlb.CurrentTimeInvalidated += new EventHandler(TimeRepeat);
            _tlc = tlb.CreateClock();

            s_dispatcher.BeginInvoke(DispatcherPriority.Normal , new DispatcherOperationCallback(OpenTheWindowColor.RunTest), null);
            s_dispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback (ShowVisualColor.RunTest), null);

            WaitForSignal("TestFinished");

            GlobalLog.LogStatus("FinishedTest");
            
            return _testPassed;
        }
    }
}
