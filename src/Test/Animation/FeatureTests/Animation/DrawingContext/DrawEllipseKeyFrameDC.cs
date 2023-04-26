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
    /// <summary>
    /// <area>Animation.DrawingContext.Regressions</area>


    [Test(3, "Animation.DrawingContext.Regressions", "DrawEllipseKeyFrameDC")]

    /******************************************************************************
    *******************************************************************************
    * CLASS:          DrawingContextTest Constructor
    *******************************************************************************
    ******************************************************************************/
    class DrawEllipseKeyFrameDCTest : AvalonTest
    {
        #region Test case members

        public Dispatcher s_dispatcher;
        //public int hBmp;
        public static System.Windows.Interop.HwndSource source;
        public static System.Windows.Interop.HwndTarget hwndTarget
        {
            get
            {
                return source.CompositionTarget;
            }
        }
        public static VisualVerifier  verifier;
        private Color               _expBefore       = Colors.Black;
        private Color               _expAfter        = Colors.White;
        private bool                _testPassed      = true;
        private string              _resultInfo      = "";
        public static Clock         tlc             = null;

        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          DrawingContextTest Constructor
        ******************************************************************************/
        public DrawEllipseKeyFrameDCTest()
        {
            InitializeSteps += new TestStep(StartTest);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Create and start a ParallelTimeline for verification purposes, then start the Animation.
        /// </summary>
        /// <returns>Returns success if the element was found</returns>
        TestResult StartTest()
        {
            s_dispatcher = Dispatcher.CurrentDispatcher;

            GlobalLog.LogStatus("---StartTest---");
            verifier = new VisualVerifier();

            ParallelTimeline tlb = new ParallelTimeline();
            tlb.BeginTime               = TimeSpan.FromMilliseconds(2000);
            tlb.Duration                = new Duration(TimeSpan.FromMilliseconds(4000));

            tlc = tlb.CreateClock(); 
            tlc.CurrentStateInvalidated += new EventHandler(OnState);

            s_dispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(OpenTheWindowKF.RunTest), null);
            s_dispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(ShowVisualKF.RunTest), null);

            WaitForSignal("TestFinished");
            
            return _testPassed ? TestResult.Pass : TestResult.Fail;
        }

        /******************************************************************************
        * Function:          OnState
        ******************************************************************************/
        private void OnState(object sender, EventArgs e)
        {
            _resultInfo += "---CurrentState--- " + ((Clock)sender).CurrentState;

            if (((Clock)sender).CurrentState == ClockState.Active)
            {
                //Verify the intial Color -before- the Animation begins.
                _testPassed = GetData(_expBefore) && _testPassed;
            }
            else if (((Clock)sender).CurrentState == ClockState.Filling)
            {
                //Verify the intial Color -after- the Animation ends.
                tlc.CurrentStateInvalidated -= new EventHandler(OnState);
                tlc.Controller.Remove();

                _testPassed = GetData(_expAfter) && _testPassed;

                GlobalLog.LogEvidence(_resultInfo);

                Signal("TestFinished", TestResult.Pass);
            }
            
        }

        /******************************************************************************
        * Function:          GetData
        ******************************************************************************/
        private bool GetData(Color expColor)
        {
            float tolerance = 0.1f;
            int x           = 250;
            int y           = 250;

            Color actColor = verifier.getColorAtPoint(x,y);
            
            bool result = AnimationUtilities.CompareColors(expColor, actColor, tolerance);

            _resultInfo += "---------- Result at (" + x + "," + y + ") ------\n";
            _resultInfo += " Actual   : " + actColor.ToString() + "\n";
            _resultInfo += " Expected : " + expColor.ToString() + "\n";

            return result;
        }

        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        /// <summary>
        /// Verifies the result of the test case, and returns a Pass/Fail result.
        /// </summary>
        /// <returns></returns>
        TestResult Verify()
        {
            WaitForSignal("TestFinished");
            
            if (_testPassed)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }
        #endregion
    }

    /******************************************************************************
    *******************************************************************************
    * CLASS:          OpenTheWindowKF
    *******************************************************************************
    ******************************************************************************/
    public class OpenTheWindowKF
    {
        public static object RunTest (object arg)
        {
            System.Windows.Interop.HwndSourceParameters parameters = new System.Windows.Interop.HwndSourceParameters("animation",400,400);           
            parameters.WindowStyle = 0x10CF0000;        
            DrawEllipseKeyFrameDCTest.source = new System.Windows.Interop.HwndSource (parameters);
            DrawEllipseKeyFrameDCTest.verifier.hWnd = DrawEllipseKeyFrameDCTest.source.Handle;
            return arg;
        }
    }

    
    /******************************************************************************
    *******************************************************************************
    * CLASS:          ShowVisualKF
    *******************************************************************************
    ******************************************************************************/
    public class ShowVisualKF
    {
        public static object RunTest(object arg)
        {
            Point center = new Point( 100,100 );

            System.Windows.Media.DrawingVisual rootVisual = new System.Windows.Media.DrawingVisual ();
            System.Windows.Media.DrawingContext ctx = rootVisual.RenderOpen();

            DoubleKeyFrameCollection collection = new DoubleKeyFrameCollection();
            collection.Add( new LinearDoubleKeyFrame( 50, KeyTime.FromPercent( .50 ) ) );
            collection.Add( new LinearDoubleKeyFrame( 350, KeyTime.FromPercent( 1.0 ) ) );

            DoubleAnimationUsingKeyFrames radiusAnimations = new DoubleAnimationUsingKeyFrames();
            radiusAnimations.KeyFrames      = collection;
            radiusAnimations.Duration       = TimeSpan.FromMilliseconds( 1000 );
            radiusAnimations.BeginTime      = TimeSpan.FromMilliseconds( 2000 );

            AnimationClock clockX = radiusAnimations.CreateClock();
            AnimationClock clockY = radiusAnimations.CreateClock();

            ctx.DrawEllipse(
                Brushes.White,
                new Pen( Brushes.Black, 3.0 ),
                center,
                null,
                5,
                clockX,
                5,
                clockY
                );

            ctx.Close ();

            DrawEllipseKeyFrameDCTest.hwndTarget.RootVisual = rootVisual;
            return arg;
        }
    }
}
