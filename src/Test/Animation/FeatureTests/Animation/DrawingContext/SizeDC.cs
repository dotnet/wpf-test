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
    public class OpenTheWindowSize
    {
        public static object RunTest (object arg)
        {
            System.Windows.Interop.HwndSourceParameters parameters = new System.Windows.Interop.HwndSourceParameters("animation",400,400);           
            parameters.WindowStyle = 0x10CF0000;        
            SizeDCTest.source = new System.Windows.Interop.HwndSource (parameters);
            SizeDCTest.verifier.hWnd = SizeDCTest.source.Handle;
            return arg;
        }
    }

    
    public class ShowVisualSize
    {
        public static object RunTest(object arg)
        {
            System.Windows.Media.DrawingVisual rootVisual = new System.Windows.Media.DrawingVisual ();
            System.Windows.Media.DrawingContext ctx = rootVisual.RenderOpen();

            SizeAnimation anim = new SizeAnimation();
            anim.From               = new Size(0,0);
            anim.To                 = new Size(100,100);
            anim.BeginTime          = TimeSpan.FromMilliseconds(3050);
            anim.Duration           = new Duration(TimeSpan.FromMilliseconds(2000));
            anim.FillBehavior       = FillBehavior.HoldEnd;

            ArcSegment mySegment = new ArcSegment();
            mySegment.Point = new Point(100,40);
            mySegment.RotationAngle = 75;
            mySegment.Size = new Size(0,0);
            mySegment.BeginAnimation(ArcSegment.SizeProperty, anim);
            mySegment.IsLargeArc = true;

            PathFigure myPathFigure = new PathFigure();
            myPathFigure.StartPoint = new Point (25,25);
            myPathFigure.Segments.Add(new LineSegment(new Point(25,50),true));
            myPathFigure.Segments.Add(mySegment);
            myPathFigure.Segments.Add(new LineSegment(new Point(125,25),true));


            ctx.DrawGeometry(Brushes.Blue, new Pen(new SolidColorBrush(Colors.Blue), 50), new PathGeometry(new PathFigure[] { myPathFigure }));

            ctx.Close ();

            SizeDCTest.hwndTarget.RootVisual = rootVisual;
            return arg;
        }
    }


    public class SizeDCTest : AvalonTest
    {
        #region Instantiating the variables

        public SizeDCTest()
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
        internal bool               testPassed      = true;
        internal string             resultInfo      = "Results: ";
        internal float[]            expWidths       = {137,137,155,195};
        internal int                expCounter      = 0;
        internal float              actWidth;
        internal float              expTolerance    = 30f;
        internal Clock              tlc;
        internal int                iterations      = 1;

        #endregion

        private void TimeRepeat(object sender, EventArgs e)
        {
            if ((((Clock)sender).CurrentIteration != iterations))
            {
                GetData();
                iterations++;
            }
        }

        private void TimeOver(object sender, EventArgs e)
        {
            if (((Clock)sender).CurrentState != ClockState.Active)
            {
                GetData();

                GlobalLog.LogEvidence(resultInfo);

                Signal("TestFinished", testPassed ? TestResult.Pass : TestResult.Fail);
            }
        }

        private void GetData()
        {
            if (expCounter < 4)
            {
                actWidth = ((BoundingBoxProperties)verifier.getBoundingBoxProperties(Colors.Blue)).width; 

                resultInfo += "\n--Tick #" + expCounter + "----------------";

                // Apply dpi ratio multiplication
                expWidths[expCounter] = (float)Monitor.ConvertLogicalToScreen(Dimension.Width, expWidths[expCounter]);

                bool b2 = (Math.Abs((actWidth - expWidths[expCounter])) < expTolerance);
                
                resultInfo += "\n Actual Width: " + actWidth + "\n Expected Width:" + expWidths[expCounter];
                resultInfo += "\n WIDTH RESULT: " + b2.ToString();
                
                testPassed = b2 && testPassed;
            }
            
            expCounter ++;
        }


        public bool StartTest()
        {
            s_dispatcher = Dispatcher.CurrentDispatcher;

            verifier = new VisualVerifier();

            ParallelTimeline tlb = new ParallelTimeline();
            tlb.BeginTime = TimeSpan.FromMilliseconds(3000);
            tlb.Duration = new Duration(TimeSpan.FromMilliseconds(501));
            tlb.RepeatBehavior = new RepeatBehavior(4);

            tlb.CurrentStateInvalidated += new EventHandler(TimeOver);
            tlb.CurrentTimeInvalidated += new EventHandler(TimeRepeat);
            tlc = tlb.CreateClock();

            s_dispatcher.BeginInvoke(DispatcherPriority.Normal ,  new DispatcherOperationCallback(OpenTheWindowSize.RunTest), null);
            s_dispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback (ShowVisualSize.RunTest), null);

            
            WaitForSignal("TestFinished");

            GlobalLog.LogStatus("FinishedTest");
            
            return testPassed;
        }
    }
}
