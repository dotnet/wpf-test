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
    public class OpenTheWindowPoint
    {
        public static object RunTest (object arg)
        {
            System.Windows.Interop.HwndSourceParameters parameters = new System.Windows.Interop.HwndSourceParameters("animation",400,400);           
            parameters.WindowStyle = 0x10CF0000;          
            PointDCTest.source = new System.Windows.Interop.HwndSource (parameters);
            PointDCTest.verifier.hWnd = PointDCTest.source.Handle;
            return arg;
        }
    }

    
    public class ShowVisualPoint
    {
        public static object RunTest(object arg)
        {
            System.Windows.Media.DrawingVisual rootVisual = new System.Windows.Media.DrawingVisual ();
            System.Windows.Media.DrawingContext ctx = rootVisual.RenderOpen();
 
            Point point1 = new Point(50,50);
            Point point2 = new Point(100,50);
            Point point3 = new Point(300,50);

            PointAnimation anim2 = new PointAnimation();
            anim2.From              = point2;
            anim2.To                = point3;
            anim2.BeginTime         = TimeSpan.FromMilliseconds(3000);
            anim2.Duration          = new Duration(TimeSpan.FromMilliseconds(2000));
            anim2.FillBehavior      = FillBehavior.HoldEnd;

            // from to are the same, just should keep the value at this point
            PointAnimation anim1 = new PointAnimation();
            anim1.From              = point1;
            anim1.To                = point1;
            anim1.BeginTime         = TimeSpan.FromMilliseconds(3000);
            anim1.Duration          = new Duration(TimeSpan.FromMilliseconds(2000));
            anim1.FillBehavior      = FillBehavior.HoldEnd;

            Pen myPen = new Pen (new SolidColorBrush (Colors.Blue), 50);
            AnimationClock anim1Clock = ((AnimationTimeline)anim1).CreateClock();
            AnimationClock anim2Clock = ((AnimationTimeline)anim2).CreateClock();

            ctx.DrawLine(myPen, point1, anim1Clock, point2, anim2Clock);

            ctx.Close ();

            PointDCTest.hwndTarget.RootVisual = rootVisual;
            return arg;
        }
    }


    public class PointDCTest : AvalonTest
    {
        #region Instantiating the variables

        public PointDCTest()
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
        public static VisualVerifier  verifier;
        private bool                _testPassed      = true;
        private string              _resultInfo      = "Results: ";
        private Color[]             _expColors       = {Colors.Black,Colors.Black,Colors.Blue,Colors.Blue};
        private float[]             _expWidths       = {100,150,200,250};
        private int                 _expCounter      = 0;
        private Color               _actColor;
        private float               _actWidth;
        private float               _expTolerance    = 25f;
        private Clock               _tlc;
        private int                 _iterations      = 1;
        private int                 _xValidationPosition = 225;


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

                // Convert for high dpi
                _expWidths[_expCounter] = (float)(Monitor.ConvertLogicalToScreen(Dimension.Width,(double)_expWidths[_expCounter]));

                _actColor = verifier.getColorAtPoint(_xValidationPosition,50);  
                _actWidth = ((BoundingBoxProperties)verifier.getBoundingBoxProperties(Colors.Blue)).width; 

                _resultInfo += "\n--Tick #" + _expCounter + "----------------";

                bool b1 = (_actColor == _expColors[_expCounter]);
                _resultInfo += "\n Actual Color at " + _xValidationPosition + ",50: " + _actColor.ToString() + "\n Expected Color :" + (_expColors[_expCounter]).ToString();
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

            _xValidationPosition = (int)(Monitor.ConvertLogicalToScreen(Dimension.Width,_xValidationPosition));

            s_dispatcher.BeginInvoke(DispatcherPriority.Normal ,  new DispatcherOperationCallback(OpenTheWindowPoint.RunTest), null);
            s_dispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback (ShowVisualPoint.RunTest), null);

            
            WaitForSignal("TestFinished");

            GlobalLog.LogStatus("FinishedTest");



            
            return _testPassed;
        }
    }
}
