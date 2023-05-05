
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/****************  BouncingBalls Integration Test *****************
*     Description:
*    Major Actions:
*       (a) Create a new Navigation window through the Animation Test Framwork.
*       (b) Create and animate four circles, i.e.,
*           --- Access the canvas1 object.
*           --- Add each circle to the Canvas.
*           --- Set up animation by assigning values to KeyValues and KeyTimes properties
*               of the DoubleAnimation.
*       (d) Use a TimeNode's End event to carry out verification after the animation
*           is complete.
*    Pass Conditions:
*       For each circle, the following APIs are read and compared to expected values:
*           (a) the last KeyValue
*           (b) Begin
*           (c) Duration
*           (d) Fill
*    How verified:
*       The result of the comparisons between actual and expected values is passed to TestResult.
*
*  Framework:        An executable is created.
*  Area:             Animation/Timing
*  Dependencies:     TestRuntime.dll, Avalon.Test.Animation.dll
*  Support Files:
**********************************************************/
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
     
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Display;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.HighLevelScenarios</area>
    /// <priority>1</priority>
    /// <description>
    /// Scenario:  Use a LinearDoubleKeyFrame animation to make the balls bounce.
    /// </description>
    /// </summary>
    [Test(2, "Animation.HighLevelScenarios", "BouncingBallsTest")]

    class BouncingBallsTest : WindowTest
    {
        #region Test case members

        private VisualVerifier                  _verifier;

        private Canvas                        _canvas1;
        private DoubleAnimationUsingKeyFrames _LA1;
        private DoubleAnimationUsingKeyFrames _LA2;
        private DoubleAnimationUsingKeyFrames _LA3;
        private DoubleAnimationUsingKeyFrames _LA4;
        private AnimationClock                _clock1;
        private AnimationClock                _clock2;
        private AnimationClock                _clock3;
        private AnimationClock                _clock4;
        private double                        _firstVal1         = 250;
        private double                        _firstVal2         = 200;
        private double                        _firstVal3         = 150;
        private double                        _firstVal4         = 100;
        private Ellipse                       _ellipse1;
        private Ellipse                       _ellipse2;
        private Ellipse                       _ellipse3;
        private Ellipse                       _ellipse4;
        private string                        _windowTitle       = "Bouncing Balls";     

        private int                           _ALLBEGIN          = 0;
        private int                           _DUR1              = 6000;
        private int                           _DUR2              = 7000;
        private int                           _DUR3              = 8000;
        private int                           _DUR4              = 9000;
        private double                        _SIZE              = 50d;
        private byte                          _color1F           = 0x1F;
        private Clock                         _tlc;
        private bool                          _testPassed        = false;

        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          BouncingBallsTest Constructor
        ******************************************************************************/
        public BouncingBallsTest()
        {
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Initialize: create a new Window and add content to it.
        /// </summary>
        /// <returns>TestResult.Pass</returns>
        TestResult Initialize()
        {
            GlobalLog.LogStatus("---Initialize---");

            Window.Left              = 0;
            Window.Top               = 0;
            Window.Height            = 600;
            Window.Width             = 600;
            Window.Title             = _windowTitle;
            Window.WindowStyle       = WindowStyle.None;

            _canvas1  = new Canvas();
            Window.Content = (_canvas1);
            _canvas1.Width            = 600;
            _canvas1.Height           = 600;
            _canvas1.Background       = Brushes.White;

            _ellipse1 = CreateCircle(_SIZE, 75d, 225d);
            _ellipse1.Fill = CreateBrush();
            _canvas1.Children.Add(_ellipse1);

            _ellipse2 = CreateCircle(_SIZE, 175d, 175d);
            _ellipse2.Fill = CreateBrush();
            _canvas1.Children.Add(_ellipse2);

            _ellipse3 = CreateCircle(_SIZE, 275d, 125d);
            _ellipse3.Fill = CreateBrush();
            _canvas1.Children.Add(_ellipse3);

            _ellipse4 = CreateCircle(_SIZE, 375d, 75d);
            _ellipse4.Fill = Brushes.Red;
            _canvas1.Children.Add(_ellipse4);

            //Set up the animation verifier.
            _verifier = new VisualVerifier();
            _verifier.InitRender(Window);

            GlobalLog.LogStatus("---NavigationWindow populated---");

            CreateAnimations();

            return TestResult.Pass;
        }

               
        /******************************************************************************
           * Function:          CreateAnimations
           ******************************************************************************/
        public void CreateAnimations()
        {
            GlobalLog.LogStatus("---CreateAnimations---");

            //Establish a Timeline for the overall Parent.
            ParallelTimeline TL1 = new ParallelTimeline();
            TL1.BeginTime       = null;
            TL1.FillBehavior    = FillBehavior.HoldEnd;

            //This is a separate timeline, used only for verification.
            ParallelTimeline TL2 = new ParallelTimeline();
            TL2.BeginTime            = TimeSpan.FromMilliseconds(0);
            TL2.Duration             = new Duration(TimeSpan.FromMilliseconds(14000));
            TL2.CurrentStateInvalidated   += new EventHandler(OnCurrentStateInvalidated);
            TL1.Children.Add(TL2);
            _tlc = TL1.CreateClock();

            //CIRCLE #1 ------------------------------------------------------------
            _LA1 = CreateDoubleAnimationUsingKeyFrames(_ALLBEGIN, _DUR1);
            _LA1.KeyFrames = CreateKeyFrameCollection(_firstVal1);                    
            _clock1 = _LA1.CreateClock();
            _ellipse1.ApplyAnimationClock(Canvas.TopProperty, _clock1);
            GlobalLog.LogStatus("---Ellipse #1 Animation Added");

            //CIRCLE #2 ------------------------------------------------------------
            _LA2 = CreateDoubleAnimationUsingKeyFrames(_ALLBEGIN, _DUR2);
            _LA2.KeyFrames = CreateKeyFrameCollection(_firstVal2);
            _clock2 = _LA2.CreateClock();
            _ellipse2.ApplyAnimationClock(Canvas.TopProperty, _clock2);
            GlobalLog.LogStatus("---Ellipse #2 Animation Added");

            //CIRCLE #3 ------------------------------------------------------------
            _LA3 = CreateDoubleAnimationUsingKeyFrames(_ALLBEGIN, _DUR3);
            _LA3.KeyFrames = CreateKeyFrameCollection(_firstVal3);
            _clock3 = _LA3.CreateClock();
            _ellipse3.ApplyAnimationClock(Canvas.TopProperty, _clock3);
            GlobalLog.LogStatus("---Ellipse #3 Animation Added");

            //CIRCLE #4 ------------------------------------------------------------
            _LA4 = CreateDoubleAnimationUsingKeyFrames(_ALLBEGIN, _DUR4);
            _LA4.KeyFrames = CreateKeyFrameCollection(_firstVal4);
            _clock4 = _LA4.CreateClock();
            _ellipse4.ApplyAnimationClock(Canvas.TopProperty, _clock4);
            GlobalLog.LogStatus("---Ellipse #4 Animation Added");

            //Begin Timelines and Animations.
            _tlc.Controller.Begin();               
        }

        /******************************************************************************
           * Function:          CreateCircle
           ******************************************************************************/
        /// <summary>
        /// CreateCircle: Returns a new Ellipse object.
        /// </summary>
        public Ellipse CreateCircle(double newSize, double newLeft, double newTop)
        {
            Ellipse newEllipse = new Ellipse();
            newEllipse.Width     = newSize;
            newEllipse.Height    = newSize;
            Canvas.SetLeft (newEllipse, newLeft);
            Canvas.SetTop  (newEllipse, newTop);

            return newEllipse;
        }

        /******************************************************************************
        * Function:          CreateBrush
        ******************************************************************************/
        /// <summary>
        /// CreateBrush: Returns a RadialGradientBrush object used to set up a Radial Gradient.
        /// </summary>
        public RadialGradientBrush CreateBrush()
        {
            RadialGradientBrush RG = new RadialGradientBrush();
            RG.RadiusX          = 1f;
            RG.Center           = new Point(0.3f,0.3f);
            RG.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x00, 0xFF, 0x00), 0));
            RG.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, _color1F, _color1F, _color1F), 1));

            return RG;
        }

        /******************************************************************************
           * Function:          CreateKeyFrameCollection
           ******************************************************************************/
        /// <summary>
        /// CreateFrames: Returns a DoubleKeyFrameCollection.
        /// </summary>
        public DoubleKeyFrameCollection CreateKeyFrameCollection(Double firstValue)
        {
            DoubleKeyFrameCollection KFC = new DoubleKeyFrameCollection();
            KFC.Add(new LinearDoubleKeyFrame(firstValue,        KeyTime.FromPercent(0f)));
            KFC.Add(new LinearDoubleKeyFrame(500.0, KeyTime.FromPercent(0.3000f)));
            KFC.Add(new LinearDoubleKeyFrame(300.0, KeyTime.FromPercent(0.5128f)));
            KFC.Add(new LinearDoubleKeyFrame(500.0, KeyTime.FromPercent(0.6154f)));
            KFC.Add(new LinearDoubleKeyFrame(400.0, KeyTime.FromPercent(0.6923f)));
            KFC.Add(new LinearDoubleKeyFrame(500.0, KeyTime.FromPercent(0.7436f)));
            KFC.Add(new LinearDoubleKeyFrame(450.0, KeyTime.FromPercent(0.7949f)));
            KFC.Add(new LinearDoubleKeyFrame(500.0, KeyTime.FromPercent(0.8462f)));
            KFC.Add(new LinearDoubleKeyFrame(475.0, KeyTime.FromPercent(0.8974f)));
            KFC.Add(new LinearDoubleKeyFrame(500.0, KeyTime.FromPercent(0.9231f)));
            KFC.Add(new LinearDoubleKeyFrame(488.0, KeyTime.FromPercent(0.9700f)));
            KFC.Add(new LinearDoubleKeyFrame(500.0, KeyTime.FromPercent(1f)));                    

            return KFC;
        }

        /******************************************************************************
           * Function:          CreateDoubleAnimationUsingKeyFrames
           ******************************************************************************/
        /// <summary>
        /// CreateDoubleAnimationUsingKeyFrames: Returns a DoubleAnimation object used to set up animation.
        /// </summary>
        public DoubleAnimationUsingKeyFrames CreateDoubleAnimationUsingKeyFrames(int newBegin, int newDuration)
        {
            DoubleAnimationUsingKeyFrames newAnim = new DoubleAnimationUsingKeyFrames();
            newAnim.BeginTime                  = TimeSpan.FromMilliseconds(newBegin);
            newAnim.Duration                   = new Duration(TimeSpan.FromMilliseconds(newDuration));
            newAnim.FillBehavior               = FillBehavior.HoldEnd;
            newAnim.AccelerationRatio          = 0.4;
            newAnim.DecelerationRatio          = 0.6;

            return newAnim;
        }


        /******************************************************************************
           * Function:          OnCurrentStateInvalidated
           ******************************************************************************/
        /// <summary>
        /// OnCurrentStateInvalidated: Invoked when the separate TimeLine has changed state.
        /// </summary>
        private void OnCurrentStateInvalidated(object sender, EventArgs e)          
        {
            if (((Clock)sender).CurrentState != ClockState.Active)
            {

                GlobalLog.LogStatus("---CurrentStateInvalidated---");

                //Check rendering.
                //Color expectedColor = Color.FromArgb(0xFF, color1F, color1F, color1F);

                float expTop = (float)Monitor.ConvertLogicalToScreen(Dimension.Width,501);
                float actTop = ((BoundingBoxProperties)_verifier.getBoundingBoxProperties(Colors.Red)).top;
                float expTolerance1 = 5f;

                bool b17 = (Math.Abs((actTop - expTop)) < expTolerance1);

                float expLeft = (float)Monitor.ConvertLogicalToScreen(Dimension.Width,375f);
                float actLeft = ((BoundingBoxProperties)_verifier.getBoundingBoxProperties(Colors.Red)).left;
                float expTolerance2 = 5f;
                bool b18 = (Math.Abs((actLeft - expLeft)) < expTolerance2);

                GlobalLog.LogEvidence("RENDERING - Top:  Actual = " + actTop.ToString()  + " Expected = " + expTop.ToString());
                GlobalLog.LogEvidence("RENDERING - Left: Actual = " + actLeft.ToString() + " Expected = " + expLeft.ToString());


                Double keyValueExpected = 500d;
                int beginExpected = _ALLBEGIN;
                int durationExpected1 = _DUR1;
                int durationExpected2 = _DUR2;
                int durationExpected3 = _DUR3;
                int durationExpected4 = _DUR4;
                string fillExpected = "HoldEnd";

                double keyValueActual1 = (double)_clock1.GetCurrentValue(0d, 0d);
                double keyValueActual2 = (double)_clock2.GetCurrentValue(0d, 0d);
                double keyValueActual3 = (double)_clock3.GetCurrentValue(0d, 0d);
                double keyValueActual4 = (double)_clock4.GetCurrentValue(0d, 0d);

                int beginActual1 = ((TimeSpan)_LA1.BeginTime.Value).Seconds * 1000 + ((TimeSpan)_LA1.BeginTime.Value).Milliseconds;
                int beginActual2 = ((TimeSpan)_LA2.BeginTime.Value).Seconds * 1000 + ((TimeSpan)_LA2.BeginTime.Value).Milliseconds;
                int beginActual3 = ((TimeSpan)_LA3.BeginTime.Value).Seconds * 1000 + ((TimeSpan)_LA3.BeginTime.Value).Milliseconds;
                int beginActual4 = ((TimeSpan)_LA4.BeginTime.Value).Seconds * 1000 + ((TimeSpan)_LA4.BeginTime.Value).Milliseconds;

                int durationActual1 = (int)_LA1.Duration.TimeSpan.TotalMilliseconds;
                int durationActual2 = (int)_LA2.Duration.TimeSpan.TotalMilliseconds;
                int durationActual3 = (int)_LA3.Duration.TimeSpan.TotalMilliseconds;
                int durationActual4 = (int)_LA4.Duration.TimeSpan.TotalMilliseconds;

                string fillActual1  = _LA1.FillBehavior.ToString();
                string fillActual2  = _LA2.FillBehavior.ToString();
                string fillActual3  = _LA3.FillBehavior.ToString();
                string fillActual4  = _LA4.FillBehavior.ToString();

                bool b1  = (keyValueActual1 == keyValueExpected);
                bool b2  = (keyValueActual2 == keyValueExpected);
                bool b3  = (keyValueActual3 == keyValueExpected);
                bool b4  = (keyValueActual4 == keyValueExpected);
                bool b5  = (beginActual1 == beginExpected);
                bool b6  = (beginActual2 == beginExpected);
                bool b7  = (beginActual3 == beginExpected);
                bool b8  = (beginActual4 == beginExpected);
                bool b9  = (durationActual1 == durationExpected1);
                bool b10 = (durationActual2 == durationExpected2);
                bool b11 = (durationActual3 == durationExpected3);
                bool b12 = (durationActual4 == durationExpected4);
                bool b13 = (fillActual1 == fillExpected);
                bool b14 = (fillActual2 == fillExpected);
                bool b15 = (fillActual3 == fillExpected);
                bool b16 = (fillActual4 == fillExpected);

                bool bValues = (b1 && b2 && b3 && b4 && b5 && b6 && b7 && b8 && b9 && b10 && b11 && b12 && b13 && b14 && b15 && b16);

                GlobalLog.LogEvidence("RENDERING RESULTS: " + (b17 && b18).ToString());
                GlobalLog.LogEvidence("VALUES RESULTS:    " + bValues);

                if (bValues && b17 && b18)
                {
                    _testPassed = true;
                }
                else
                {
                    string failMessage = "";
                    if(!b1) failMessage += "KeyValue #1 in error.  Expected: " + keyValueExpected.ToString() + " // Actual: " + keyValueActual1.ToString() + "\n";
                    if(!b2) failMessage += "KeyValue #2 in error.  Expected: " + keyValueExpected.ToString() + " // Actual: " + keyValueActual2.ToString() + "\n";
                    if(!b3) failMessage += "KeyValue #3 in error.  Expected: " + keyValueExpected.ToString() + " // Actual: " + keyValueActual3.ToString() + "\n";
                    if(!b4) failMessage += "KeyValue #4 in error.  Expected: " + keyValueExpected.ToString() + " // Actual: " + keyValueActual4.ToString() + "\n";
                    if(!b5) failMessage += "Begin #1 in error.  Expected: " + beginExpected.ToString() + " // Actual: " + beginActual1.ToString() + "\n";
                    if(!b6) failMessage += "Begin #2 in error.  Expected: " + beginExpected.ToString() + " // Actual: " + beginActual2.ToString() + "\n";
                    if(!b7) failMessage += "Begin #3 in error.  Expected: " + beginExpected.ToString() + " // Actual: " + beginActual3.ToString() + "\n";
                    if(!b8) failMessage += "Begin #4 in error.  Expected: " + beginExpected.ToString() + " // Actual: " + beginActual4.ToString() + "\n";
                    if(!b9) failMessage +=  "Duration #1 in error.  Expected: " + durationExpected1.ToString() + " // Actual: " + durationActual1.ToString() + "\n";
                    if(!b10) failMessage += "Duration #2 in error.  Expected: " + durationExpected2.ToString() + " // Actual: " + durationActual2.ToString() + "\n";
                    if(!b11) failMessage += "Duration #3 in error.  Expected: " + durationExpected3.ToString() + " // Actual: " + durationActual3.ToString() + "\n";
                    if(!b12) failMessage += "Duration #4 in error.  Expected: " + durationExpected4.ToString() + " // Actual: " + durationActual4.ToString() + "\n";
                    if(!b13) failMessage += "Fill #1 in error.  Expected: " + fillExpected.ToString() + " // Actual: " + fillActual1.ToString() + "\n";
                    if(!b14) failMessage += "Fill #2 in error.  Expected: " + fillExpected.ToString() + " // Actual: " + fillActual2.ToString() + "\n";
                    if(!b15) failMessage += "Fill #3 in error.  Expected: " + fillExpected.ToString() + " // Actual: " + fillActual3.ToString() + "\n";
                    if(!b16) failMessage += "Fill #4 in error.  Expected: " + fillExpected.ToString() + " // Actual: " + fillActual4.ToString() + "\n";
                    if(!b17) failMessage += "Bounds (Top) results in error";
                    if(!b18) failMessage += "Bounds (Left) results in error";

                    GlobalLog.LogEvidence("Values Tests Failed\n" + failMessage);
                }
                
                if (_testPassed)
                {
                    Signal("TestFinished", TestResult.Pass);
                }
                else
                {
                    Signal("TestFinished", TestResult.Fail);
                }
            }
        }

        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        /// <summary>
        /// Verifies the result of the test case, and returns a Pass/Fail result.
        /// </summary>
        /// <returns>A TestResult, indicating pass or fail</returns>
        TestResult Verify()
        {
            TestResult result = WaitForSignal("TestFinished");

            return result;
        }
        #endregion
     }
}
