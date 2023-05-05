// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
     
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Display;

namespace Microsoft.Test.Animation
{
    public class OpenTheWindowRect
    {
        public static object RunTest (object arg)
        {
            System.Windows.Interop.HwndSourceParameters parameters = new System.Windows.Interop.HwndSourceParameters("animation",400,400);           
            parameters.WindowStyle = 0x10CF0000;
            RectDCTest.source = new System.Windows.Interop.HwndSource (parameters);
            RectDCTest.verifier.hWnd = RectDCTest.source.Handle;
            return arg;
        }
    }
    

    public class ShowVisualRect
    {
        public static object RunTest(object arg)
        {
            System.Windows.Media.DrawingVisual rootVisual = new System.Windows.Media.DrawingVisual ();
            System.Windows.Media.DrawingContext ctx = rootVisual.RenderOpen();

            RectAnimation anim = new RectAnimation();
            anim.To                 = new Rect(50,50,250,100);
            anim.BeginTime          = TimeSpan.FromMilliseconds(3000);
            anim.Duration           = new Duration(TimeSpan.FromMilliseconds(2000));
            anim.FillBehavior       = FillBehavior.HoldEnd;

            Pen myPen = new Pen (new SolidColorBrush (Colors.Red), 0);
            AnimationClock animClock = ((AnimationTimeline)anim).CreateClock();
            Rect myRect = new Rect(50,50,50,100);

            ctx.DrawRectangle(Brushes.Blue, myPen, myRect, animClock);

            ctx.Close ();

            RectDCTest.hwndTarget.RootVisual = rootVisual;
            return arg;
        }
    }


    public class RectDCTest : AvalonTest
    {
        #region Instantiating the variables

        public RectDCTest()
        {
        }

        public Dispatcher s_dispatcher;
        public int hBmp;
        public static System.Windows.Interop.HwndSource source;
        public static System.Windows.Interop.HwndTarget hwndTarget
        {
            get
            {
                return ((System.Windows.Interop.HwndSource)source).CompositionTarget;
            }
        }
        public static VisualVerifier    verifier;
        private bool                   _testPassed      = true;
        private string                 _resultInfo      = "Results: ";
        private Color[]                _expColors       = {Colors.Black,Colors.Black,Colors.Blue,Colors.Blue};
        private float[]                _expWidths       = {100,150,200,250};
        private float                  _expTolerance    = 55f;
        private int                    _expCounter      = 0;
        private Color                  _actColor;
        private float                  _actWidth;
        private Clock                  _tlc;
        private int                    _iterations      = 1;
        private int                    _xValidationPosition = 225;

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

                // Convert based on dpi ratio 
                _expWidths[_expCounter] = (float)Monitor.ConvertLogicalToScreen(Dimension.Width, _expWidths[_expCounter]);

                _actColor = verifier.getColorAtPoint(_xValidationPosition,75);   
                _actWidth = ((BoundingBoxProperties)verifier.getBoundingBoxProperties(Colors.Blue)).width;

                _resultInfo += "\n--Tick #" + _expCounter + "----------------";

                bool b1 = (_actColor == _expColors[_expCounter]);
                _resultInfo += "\n Actual Color at " + _xValidationPosition +",75: " + _actColor.ToString() + "\n Expected Color:" + (_expColors[_expCounter]).ToString();
                _resultInfo += "\n COLOR RESULT: " + b1.ToString();

                bool b2 = (Math.Abs((_actWidth - _expWidths[_expCounter])) < _expTolerance);
                
                _resultInfo += "\n Actual Width: " + _actWidth + "\n Expected Width:" + _expWidths[_expCounter];
                _resultInfo += "\n WIDTH RESULT: " + b2.ToString();
                
                _testPassed = b1 && b2 && _testPassed;
            }

            _expCounter ++;
        }


        public bool StartTest()
        {        
            s_dispatcher = Dispatcher.CurrentDispatcher;

            verifier = new VisualVerifier();

            ParallelTimeline tlb = new ParallelTimeline();
            tlb.BeginTime           = TimeSpan.FromMilliseconds(3000);
            tlb.Duration            = new Duration(TimeSpan.FromMilliseconds(502));
            tlb.RepeatBehavior      = new RepeatBehavior(4);

            tlb.CurrentStateInvalidated += new EventHandler(TimeOver);
            tlb.CurrentTimeInvalidated += new EventHandler(TimeRepeat);
            _tlc = tlb.CreateClock();

            _xValidationPosition =  (int)Monitor.ConvertLogicalToScreen(Dimension.Width, _xValidationPosition);

            s_dispatcher.BeginInvoke(DispatcherPriority.Normal ,  new DispatcherOperationCallback(OpenTheWindowRect.RunTest), null);
            s_dispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback (ShowVisualRect.RunTest), null);
            
            WaitForSignal("TestFinished");

            GlobalLog.LogStatus("FinishedTest");
            
            return _testPassed;
        }
    }
}
