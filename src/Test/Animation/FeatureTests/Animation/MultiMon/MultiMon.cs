// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* ***************  MIL Integration Test *****************
*     Purpose: to verify that Animation takes place with multi-monitors.
*
*     Pass Conditions: the test passes if the animated element appears correctly during and after the
*                      animation.
*
*     How verified: the VScan utility, getColorAtPoint(x1, y1), is called to determine if the actual
*                   color of the animated element is correct.  The following checks are carried out:
*                   (1) at the initial pre-animation position (in most cases, the animated element
*                       straddles the monitors; the check is made in the monitor #2.
*                   (2) at the position the animated element is at, before reversing.
*                   (3) at the final position, to ensure the animation ends up at the starting point.
*                       The check is made on -both- monitors.
*                   Exception: some cases only check animation within monitor #2.
*
*     Framework:          A CLR executable is created.
*     Area:               Animation/Timing
*     Dependencies:       TestRuntime.dll, AnimationFramework.dll
*     Support Files:          
*
**********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.MultiMon</area>
    /// <priority>2</priority>
    /// <description>
    /// Verify that Animation takes place with multi-monitors.
    /// </description>
    /// </summary>
    
    [Test(4, "Animation.MultiMon", "MultiMonTest")]
    public class MultiMonTest : WindowTest
    {
        #region Test case members

        private VisualVerifier      _verifier;
        private DispatcherTimer     _aTimer                      = null;  //Used for Timing method verification.

        private string              _animType                    = null;
        private int                 _animMonitor                 = 0;
        private string              _windowTitle                 = "MultiMon Animation Test";
        private ArrayList           _expectedList;
        private Canvas              _body                        = null;
        private Ellipse             _ellipse1                    = null;
        private EllipseGeometry     _ellipseGeo                  = null;
        private TimeSpan            _DISPATCHER_TICK_INTERVAL    = TimeSpan.FromMilliseconds(2500);
        private int                 _dispatcherTickCount         = 0;
        private int                 _actCurrentState             = 0;
        private int                 _expCurrentState             = 2;
        private int                 _actCurrentGlobalSpeed       = 0;
        private int                 _expCurrentGlobalSpeed       = 3;
        private Color               _expColor                    = Colors.Black;
        private Color               _expMidColor                 = Colors.Black;
        private bool                _testPassed                  = true;
        private TimeSpan            _BEGIN_TIME                  = TimeSpan.FromMilliseconds(750);
        private TimeSpan            _DURATION_TIME               = TimeSpan.FromMilliseconds(1500);

        #endregion


        #region Constructor

        [Variation("ColorBackground", "1")]
        [Variation("ColorBackground", "2")]
        [Variation("Left", "1")]
        [Variation("Left", "2")]
        [Variation("Opacity", "1")]
        [Variation("Opacity", "2")]
        [Variation("Rotate", "1")]
        [Variation("Rotate", "2")]
        [Variation("Scale", "1")]
        [Variation("Scale", "2")]
        [Variation("TranslateXY", "2")]
        [Variation("Top", "1")]
        [Variation("Top", "2")]
        [Variation("Width", "1")]
        [Variation("Width", "2", Disabled=true)]    // Flakey test: disabling - 09-22-10.

       
        /******************************************************************************
        * Function:          MultiMonTest Constructor
        ******************************************************************************/
        public MultiMonTest(string testValue1, string testValue2)
        {

            _animType    = testValue1;
            _animMonitor = (int)Convert.ToInt16(testValue2); //Indicates which monitor the animation will go -to-.
            InitializeSteps += new TestStep(CheckInputParameters);
            RunSteps += new TestStep(CreateWindow);
            RunSteps += new TestStep(Verify);
        }

        #endregion

        #region Test Steps

        /******************************************************************************
        * Function:          CheckInputParameters
        ******************************************************************************/
        /// <summary>
        /// CheckInputParameters: checks for a valid input string.
        /// </summary>
        /// <returns>Returns TestResult</returns>
        TestResult CheckInputParameters()
        {
            bool        arg1Found   = false;
            bool        arg2Correct = true;
            string      errMessage  = "";
            string[]    expList     = { "ColorBackground", "Left", "Opacity", "Rotate", "Scale", "TranslateXY", "Top", "Width" };

            arg1Found = AnimationUtilities.CheckInputString(_animType, expList, ref errMessage);
            if (errMessage != "")
            {
                GlobalLog.LogEvidence("ERROR!!! CheckInputParameters -- 1st Parameter: " + errMessage);
            }
            else
            {
                if (_animMonitor != 1 && _animMonitor != 2)
                {
                    errMessage = "The 2nd parameter must be either 1 or 2, to specify the Monitor on which the Animation is occur.";
                    GlobalLog.LogEvidence("ERROR!!! CheckInputParameters -- 2nd Parameter: " + errMessage);
                    arg2Correct = false;
                }
            }

            if (arg1Found && arg2Correct)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        /******************************************************************************
        * Function:          CreateWindow
        ******************************************************************************/
        /// <summary>
        /// CreateWindow: Creates the window content.
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        TestResult CreateWindow()
        {
            GlobalLog.LogStatus("----StartTest----");

            //animMonitor = (int)Convert.ToInt16(resultArray[1]);

            // bail if we aren't running on multimon
            if (Microsoft.Test.Display.Monitor.GetDisplayCount() < 2)
            {
                  GlobalLog.LogEvidence("ERROR!!! Multi-Mon test was run on a non Multi-Mon machine");
                  return TestResult.Fail;
            }
            else
            {

                Window.Left                 = 100d; //Place the window inbetween the monitors.
                Window.Top                  = 0d;
                Window.Height               = 800d;
                Window.Width                = 1800d;
                Window.Title                = _windowTitle; 
                Window.ContentRendered     += new EventHandler(OnContentRendered);
                NameScope.SetNameScope(Window, new NameScope());

                _body  = new Canvas();
                Window.Content = _body;
                _body.Background = Brushes.White;

                _ellipse1  = new Ellipse();
                _ellipse1.Name   = "ellipse1";
                _ellipse1.Fill   = Brushes.Black;
                _ellipse1.Height = 400d;
                _ellipse1.Width  = 400d;
                Canvas.SetTop  (_ellipse1, 200d);
                Canvas.SetLeft (_ellipse1, 700d);

                //In some cases, need to set the base value; it needs to occur before the page renders,
                //so that the initial verification will pass.
                if (_animType == "ClipCenter")
                {
                    _ellipse1.Height = 800d;
                    _ellipse1.Width  = 800d;
                    Canvas.SetLeft (_ellipse1, 600d);
                    _ellipseGeo = new EllipseGeometry();
                    _ellipseGeo.RadiusX               = 100d;
                    _ellipseGeo.RadiusY               = 100d;
                    _ellipseGeo.Center                = new Point(300,200);
                    _ellipse1.Clip = _ellipseGeo;
                }
                if (_animType == "Text")
                {
                    _ellipse1.Fill   = Brushes.White;
                }
                if (_animType == "Width")
                {
                    Canvas.SetLeft (_ellipse1, 100d);
                }
                if (_animType == "Top")
                {
                    Canvas.SetTop  (_ellipse1, -200d);
                    Canvas.SetLeft (_ellipse1, 1200d);
                }

                _body.Children.Add(_ellipse1);

                _verifier = new VisualVerifier();
                _verifier.InitRender(Window);

                _expectedList = new ArrayList();
            
                return TestResult.Pass;
            }
        }
        
        /******************************************************************************
           * Function:          OnContentRendered
           ******************************************************************************/
        /// <summary>
        /// OnContentRendered: Invoked when all content is rendered.
        /// Obtain references to elements defined in the .xaml page. Then start the test.
        /// </summary>
        private void OnContentRendered(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("----ContentRendered Fired----");

            if (Animate())
            {
                StartTest();
            }
        }

        /******************************************************************************
        * Function:          Animate
        ******************************************************************************/
        /// <summary>
        /// Create and begin an Animation.
        /// </summary>
        /// <returns>A boolean indicating whether or not the requested animation type was found.</returns>
        public bool Animate()
        {
            bool typeFound = true;
            
            switch (_animType)
            {
                case "ColorBackground":
                    //Rather than reversing a single animation, using two animations, with an interval
                    //between them that allows for verification.
                    _expMidColor = Colors.White;
                    _expCurrentGlobalSpeed = 2;
                    
                    ColorAnimation animColor1 = new ColorAnimation();
                    animColor1.To            = Colors.White;
                    animColor1.BeginTime     = _BEGIN_TIME;
                    animColor1.Duration      = new Duration(_DURATION_TIME);
                    
                    ColorAnimation animColor2 = new ColorAnimation();
                    animColor2.To            = Colors.Black;
                    animColor2.BeginTime     = _DURATION_TIME + _BEGIN_TIME + _BEGIN_TIME + _BEGIN_TIME;
                    animColor2.Duration      = new Duration(_DURATION_TIME - _BEGIN_TIME);
                    
                    _expectedList.Add("800,401");            //Verification point #1 - monitor #1.
                    _expectedList.Add("1000,401");           //Verification point #2 - monitor #2.
                    if (_animMonitor == 1)
                    {
                        _expectedList.Add("800,401");        //Verification point #3.
                    }
                    else
                    {
                        _expectedList.Add("1200,401");       //Verification point #3.
                    }
                    _expectedList.Add("800,401");            //Verification point #4 - monitor #1.
                    _expectedList.Add("1000,401");           //Verification point #5 - monitor #2.

                    SolidColorBrush SCB = new SolidColorBrush();
                    SCB.Color = Colors.White;
                    _ellipse1.Fill = SCB;
                    
                    Storyboard storyboard1 = new Storyboard();
                    storyboard1.Name = "story1";
                    storyboard1.Children.Add(animColor1);
                    storyboard1.Children.Add(animColor2);
                    storyboard1.CurrentStateInvalidated       += new EventHandler(OnCurrentStateInvalidated);
                    storyboard1.CurrentGlobalSpeedInvalidated += new EventHandler(OnCurrentGlobalSpeedInvalidated);
                    PropertyPath path1  = new PropertyPath("(0).(1)", Ellipse.FillProperty, SolidColorBrush.ColorProperty);
                    Storyboard.SetTargetProperty(animColor1, path1);
                    Storyboard.SetTargetProperty(animColor2, path1);
                    Window.RegisterName(_ellipse1.Name, _ellipse1);
                    Storyboard.SetTargetName(animColor1, _ellipse1.Name);
                    Storyboard.SetTargetName(animColor2, _ellipse1.Name);
                    storyboard1.Begin(_ellipse1);
                    break;
                    
                case "Left":
                    DoubleAnimation animLeft = new DoubleAnimation();
                    animLeft.BeginTime      = _BEGIN_TIME;
                    animLeft.Duration       = new Duration(_DURATION_TIME);
                    animLeft.AutoReverse    = true;
                    animLeft.CurrentStateInvalidated       += new EventHandler(OnCurrentStateInvalidated);
                    animLeft.CurrentGlobalSpeedInvalidated += new EventHandler(OnCurrentGlobalSpeedInvalidated);
                    
                    _expectedList.Add("800,401");            //Verification point #1 - monitor #1.
                    _expectedList.Add("1000,401");           //Verification point #2 - monitor #2.
                    if (_animMonitor == 1)
                    {
                        animLeft.To = 0d;
                        _expectedList.Add("200,401");        //Verification point #3.
                    }
                    else
                    {
                        animLeft.To = 1500d;
                        _expectedList.Add("1700,401");       //Verification point #3.
                    }
                    _expectedList.Add("800,401");            //Verification point #4 - monitor #1.
                    _expectedList.Add("1000,401");           //Verification point #5 - monitor #2.
                    
                    _ellipse1.BeginAnimation(Canvas.LeftProperty, animLeft);
                    break;
                    
                case "Opacity":
                    //Rather than reversing a single animation, using two animations, with an interval
                    //between them that allows for verification.
                    _expMidColor = Colors.White;
                    _expCurrentGlobalSpeed = 2;
                    
                    DoubleAnimation animOpacity1 = new DoubleAnimation();
                    animOpacity1.From        = 1;
                    animOpacity1.To          = 0;
                    animOpacity1.BeginTime   = _BEGIN_TIME;
                    animOpacity1.Duration    = new Duration(_DURATION_TIME);
                    
                    DoubleAnimation animOpacity2 = new DoubleAnimation();
                    animOpacity2.From        = 0;
                    animOpacity2.To          = 1;
                    animOpacity2.BeginTime   = _DURATION_TIME + _BEGIN_TIME + _BEGIN_TIME + _BEGIN_TIME;
                    animOpacity2.Duration    = new Duration(_DURATION_TIME - _BEGIN_TIME);

                    _expectedList.Add("800,401");            //Verification point #1 - monitor #1.
                    _expectedList.Add("1000,401");           //Verification point #2 - monitor #2.
                    if (_animMonitor == 1)
                    {
                        _expectedList.Add("800,401");        //Verification point #3.
                    }
                    else
                    {
                        _expectedList.Add("1000,401");       //Verification point #3.
                    }
                    _expectedList.Add("800,401");            //Verification point #4 - monitor #1.
                    _expectedList.Add("1000,401");           //Verification point #5 - monitor #2.
                    
                    Storyboard storyboard2 = new Storyboard();
                    storyboard2.Name = "story2";
                    storyboard2.Children.Add(animOpacity1);
                    storyboard2.Children.Add(animOpacity2);
                    storyboard2.CurrentStateInvalidated       += new EventHandler(OnCurrentStateInvalidated);
                    storyboard2.CurrentGlobalSpeedInvalidated += new EventHandler(OnCurrentGlobalSpeedInvalidated);
                    PropertyPath path2  = new PropertyPath("(0)", Ellipse.OpacityProperty);
                    Storyboard.SetTargetProperty(animOpacity1, path2);
                    Storyboard.SetTargetProperty(animOpacity2, path2);
                    Window.RegisterName(_ellipse1.Name, _ellipse1);
                    Storyboard.SetTargetName(animOpacity1, _ellipse1.Name);
                    Storyboard.SetTargetName(animOpacity2, _ellipse1.Name);
                    storyboard2.Begin(_ellipse1);
                    break;
                    
                case "Rotate":
                    DoubleAnimation animRotate1  = new DoubleAnimation();                                             
                    animRotate1.BeginTime       = _BEGIN_TIME;
                    animRotate1.Duration        = new Duration(_DURATION_TIME);
                    animRotate1.From            = 0;
                    animRotate1.AutoReverse     = true;
                    animRotate1.CurrentStateInvalidated       += new EventHandler(OnCurrentStateInvalidated);
                    animRotate1.CurrentGlobalSpeedInvalidated += new EventHandler(OnCurrentGlobalSpeedInvalidated);
                    
                    _expectedList.Add("800,401");            //Verification point #1 - monitor #1.
                    _expectedList.Add("1000,401");           //Verification point #2 - monitor #2.
                    if (_animMonitor == 1)
                    {
                        animRotate1.To = 170;
                        _expectedList.Add("500,100");        //Verification point #3.
                    }
                    else
                    {
                        animRotate1.To = -90;
                        _expectedList.Add("1000,100");       //Verification point #3.
                    }
                    _expectedList.Add("800,401");            //Verification point #4 - monitor #1.
                    _expectedList.Add("1000,401");           //Verification point #5 - monitor #2.

                    RotateTransform rotateTransform = new RotateTransform();
                    rotateTransform.Angle       = 0.0f;
                    rotateTransform.CenterX     = 0;
                    rotateTransform.CenterY     = 0;

                    _ellipse1.RenderTransform = rotateTransform;
                    
                    rotateTransform.BeginAnimation(RotateTransform.AngleProperty, animRotate1);
                    break;
                    
                case "Scale":
                    DoubleAnimation animScale  = new DoubleAnimation();                                             
                    animScale.BeginTime         = _BEGIN_TIME;
                    animScale.Duration          = new Duration(_DURATION_TIME);
                    animScale.AutoReverse       = true;
                    animScale.CurrentStateInvalidated       += new EventHandler(OnCurrentStateInvalidated);
                    animScale.CurrentGlobalSpeedInvalidated += new EventHandler(OnCurrentGlobalSpeedInvalidated);
                    
                    _expectedList.Add("800,401");            //Verification point #1 - monitor #1.
                    _expectedList.Add("1000,401");           //Verification point #2 - monitor #2.
                    if (_animMonitor == 1)
                    {
                        animScale.By = -5;
                        _expectedList.Add("100,401");        //Verification point #3.
                    }
                    else
                    {
                        animScale.By = 5;
                        _expectedList.Add("1700,401");       //Verification point #3.
                    }
                    _expectedList.Add("800,401");            //Verification point #4 - monitor #1.
                    _expectedList.Add("1000,401");           //Verification point #5 - monitor #2.
                    
                    ScaleTransform scaleTransform = new ScaleTransform();
                    scaleTransform.ScaleX       = 1d;
                    scaleTransform.ScaleY       = 1d;

                    _ellipse1.RenderTransform = scaleTransform;
                    
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animScale);
                    break;
                    
                case "Text":
                    _expColor = Colors.White;
                    TextBlock textblock1 = new TextBlock();
                    _body.Children.Add(textblock1);
                    textblock1.Text = "Avalon";
                    textblock1.FontSize = 128d;
                    textblock1.Foreground = Brushes.Black;
                    textblock1.FontFamily = new FontFamily("Rockwell Extra Bold");
                    Canvas.SetTop  (textblock1, 360d);
                    Canvas.SetLeft (textblock1, 650d);
                    
                    StringAnimationUsingKeyFrames animString = new StringAnimationUsingKeyFrames();

                    StringKeyFrameCollection SKFC = new StringKeyFrameCollection();
                    SKFC.Add(new DiscreteStringKeyFrame("AAAAA",KeyTime.FromPercent(0.1f)));
                    SKFC.Add(new DiscreteStringKeyFrame("WWWWW", KeyTime.FromPercent(0.8f)));
                    animString.KeyFrames = SKFC;
                    animString.BeginTime        = _BEGIN_TIME;
                    animString.Duration         = new Duration(_DURATION_TIME);
                    animString.AutoReverse      = true;
                    animString.CurrentStateInvalidated       += new EventHandler(OnCurrentStateInvalidated);
                    animString.CurrentGlobalSpeedInvalidated += new EventHandler(OnCurrentGlobalSpeedInvalidated);
                    
                    _expectedList.Add("800,401");            //Verification point #1 - monitor #1.
                    _expectedList.Add("1000,401");           //Verification point #2 - monitor #2.
                    if (_animMonitor == 1)
                    {
                        _expectedList.Add("1080,401");       //Verification point #3.
                    }
                    else
                    {
                        _expectedList.Add("1135,401");       //Verification point #3.
                    }
                    _expectedList.Add("800,401");            //Verification point #4 - monitor #1.
                    _expectedList.Add("1000,401");           //Verification point #5 - monitor #2.
                    
                    textblock1.BeginAnimation(TextBlock.TextProperty, animString);
                    break;
                    
                case "TranslateXY":
                    _expCurrentState         = 4;
                    _expCurrentGlobalSpeed   = 6;
                    
                    DoubleAnimation animTrans = new DoubleAnimation();                                             
                    animTrans.BeginTime         = _BEGIN_TIME;
                    animTrans.Duration          = new Duration(_DURATION_TIME);
                    animTrans.AutoReverse       = true;
                    animTrans.CurrentStateInvalidated       += new EventHandler(OnCurrentStateInvalidated);
                    animTrans.CurrentGlobalSpeedInvalidated += new EventHandler(OnCurrentGlobalSpeedInvalidated);
                    
                    _expectedList.Add("800,401");            //Verification point #1 - monitor #1.
                    _expectedList.Add("1000,401");           //Verification point #2 - monitor #2.
                    if (_animMonitor == 1)
                    {
                        animTrans.By = -300d;
                        _expectedList.Add("700,200");        //Verification point #3.
                    }
                    else
                    {
                        animTrans.By = 300d;
                        _expectedList.Add("1200,600");       //Verification point #3.
                    }
                    _expectedList.Add("800,401");            //Verification point #4 - monitor #1.
                    _expectedList.Add("1000,401");           //Verification point #5 - monitor #2.
                    
                    TranslateTransform translateTransform = new TranslateTransform();
                    translateTransform.X     = 0;
                    translateTransform.Y     = 0;

                    _ellipse1.RenderTransform = translateTransform;
                    translateTransform.BeginAnimation(TranslateTransform.XProperty, animTrans);
                    translateTransform.BeginAnimation(TranslateTransform.YProperty, animTrans);
                    break;

                case "Top":
                    //This animation takes place only in monitor #2.
                    DoubleAnimation animTop = new DoubleAnimation();
                    animTop.BeginTime      = _BEGIN_TIME;
                    animTop.Duration       = new Duration(_DURATION_TIME);
                    animTop.AutoReverse    = true;
                    animTop.CurrentStateInvalidated       += new EventHandler(OnCurrentStateInvalidated);
                    animTop.CurrentGlobalSpeedInvalidated += new EventHandler(OnCurrentGlobalSpeedInvalidated);
                    
                    _expectedList.Add("1500,100");            //Verification point #1 - monitor #1.
                    _expectedList.Add("1500,150");           //Verification point #2 - monitor #2.
                    
                    animTop.By = 650d;
                    _expectedList.Add("1500,700");           //Verification point #3.
                    
                    _expectedList.Add("1500,100");            //Verification point #4 - monitor #1.
                    _expectedList.Add("1500,150");           //Verification point #5 - monitor #2.
                    
                    _ellipse1.BeginAnimation(Canvas.TopProperty, animTop);
                    break;
                    
                case "Width":
                    DoubleAnimation animWidth = new DoubleAnimation();
                    animWidth.To            = 1600d;
                    animWidth.BeginTime     = _BEGIN_TIME;
                    animWidth.Duration      = new Duration(_DURATION_TIME);
                    animWidth.AutoReverse   = true;
                    animWidth.CurrentStateInvalidated       += new EventHandler(OnCurrentStateInvalidated);
                    animWidth.CurrentGlobalSpeedInvalidated += new EventHandler(OnCurrentGlobalSpeedInvalidated);
                    
                    _expectedList.Add("200,401");            //Verification point #1 - monitor #1.
                    _expectedList.Add("300,401");            //Verification point #2 - monitor #2.
                    if (_animMonitor == 1)
                    {
                        _expectedList.Add("900,401");        //Verification point #3.
                    }
                    else
                    {
                        _expectedList.Add("1500,401");      //Verification point #3.
                    }
                    _expectedList.Add("200,401");            //Verification point #4 - monitor #1.
                    _expectedList.Add("300,401");            //Verification point #5 - monitor #2.
                    
                    _ellipse1.BeginAnimation(FrameworkElement.WidthProperty, animWidth);
                    break;
                    
                default:
                    typeFound = false;
                    GlobalLog.LogEvidence("ERROR!!! Animate: Incorrect animation type specified.");
                    Signal("TestFinished", TestResult.Fail);
                    break;
             }
             
             return typeFound;
        }

        /******************************************************************************
           * Function:          StartTest
           ******************************************************************************/
        /// <summary>
        /// StartTest: set up verification.
        /// Then, start the Animation.
        /// </summary>
        private void StartTest()
        {
            //Conduct an intial check before the Animation starts.
            Thread.Sleep(1000);
            GlobalLog.LogStatus("----DispatcherTimer Started ---- Tick #0");
            _testPassed = CheckColor((string)_expectedList[0], _expColor) && _testPassed;  //Check on monitor #1.
            _testPassed = CheckColor((string)_expectedList[1], _expColor) && _testPassed;  //Check on monitor #2.

            //Verify Timing Methods using OnDispatcherTick to control UIAutomation.
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnDispatcherTick);
            _aTimer.Interval = _DISPATCHER_TICK_INTERVAL;
            _aTimer.Start();
        }
        
        
        /******************************************************************************
        * Function:          OnDispatcherTick
        ******************************************************************************/
        /// <summary>
        /// The OnDispatcherTick event handler of the DispatcherTimer is used solely as a
        /// means of verifying the Animation.
        /// </summary>
        /// <returns></returns>              
        private void OnDispatcherTick(object sender, EventArgs e)          
        {
            _dispatcherTickCount++;
            GlobalLog.LogEvidence("-----------------------------------Tick #" + _dispatcherTickCount);
            
            
            if (_dispatcherTickCount == 1)
            {
                //Verify position at the point that the animation reverses.
                _testPassed = CheckColor((string)_expectedList[_dispatcherTickCount+1], _expMidColor) && _testPassed;
            }
            else if (_dispatcherTickCount == 2)
            {
                //Verify position after the animation finishes, on both monitors.
                _testPassed = CheckColor((string)_expectedList[_dispatcherTickCount+1], _expColor) && _testPassed;
                _testPassed = CheckColor((string)_expectedList[_dispatcherTickCount+2], _expColor) && _testPassed;
            }
            else
            {
                //Last Tick:  Stop the ticking (after all events have occurred), and verify.
                _aTimer.Stop();

                //Check event firing.
                GlobalLog.LogEvidence("CurrentStateInvalidated       -- Act: " + _actCurrentState       + " / Exp: " + _expCurrentState);
                GlobalLog.LogEvidence("CurrentGlobalSpeedInvalidated -- Act: " + _actCurrentGlobalSpeed + " / Exp: " + _expCurrentGlobalSpeed);
               
                bool eventsCorrect = (_actCurrentState == _expCurrentState) && (_actCurrentGlobalSpeed == _expCurrentGlobalSpeed);
               
                _testPassed = (eventsCorrect && _testPassed);
               
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
           * Function:          CheckColor
           ******************************************************************************/
        /// <summary>
        /// CheckColor: retrieve the color at the specified point and compare it to the expected color.
        /// </summary>
        private bool CheckColor(string points, Color expectedColor)
        {
            bool passed        = true;
            
            //Color expColor = (Color)ColorConverter.ConvertFromString(color);
            
            Point point = Point.Parse(points);
            int x1 = (int)point.X;
            int y1 = (int)point.Y;
            
            //At each Tick, check the color at the specified point.
            Color actualColor = _verifier.getColorAtPoint(x1, y1);
            
            float tolerance  = 0.0f;

            if (_animType == "colorbackground" || _animType == "opacity")
            {
                //Need to set greater tolerance when animating the background.
                tolerance  = 0.002f;
            }
            else
            {
                tolerance  = 0.001f;
            }

            passed = AnimationUtilities.CompareColors(expectedColor, actualColor, tolerance);

            GlobalLog.LogEvidence("---------- Result at (" + x1 + "," + y1 + ") ------");
            GlobalLog.LogEvidence(" Actual   : " + actualColor.ToString());
            GlobalLog.LogEvidence(" Expected : " + expectedColor.ToString());

            return passed;
        }

        /******************************************************************************
           * Function:          OnCurrentStateInvalidated
           ******************************************************************************/
        /// <summary>
        /// OnCurrentStateInvalidated: Used to validate that the animatin began.
        /// </summary>
        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {               
           GlobalLog.LogStatus("CurrentStateInvalidated:  " + ((Clock)sender).CurrentState);
           _actCurrentState++;
        }

        /******************************************************************************
           * Function:          OnCurrentGlobalSpeedInvalidated
           ******************************************************************************/
        /// <summary>
        /// OnCurrentGlobalSpeedInvalidated: Used to validate that the animatin began.
        /// </summary>
        private void OnCurrentGlobalSpeedInvalidated(object sender, EventArgs e)
        {               
            GlobalLog.LogStatus("CurrentGlobalSpeedInvalidated:  " + ((Clock)sender).CurrentGlobalSpeed);
            _actCurrentGlobalSpeed++;
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
