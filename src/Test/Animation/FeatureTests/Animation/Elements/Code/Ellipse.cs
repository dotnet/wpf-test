// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* ***************  MIL Integration Test *****************
*     Major Actions:
*          (a) Create a new Windowdow via OnStartup.
*          (b) Create an Ellipse, and for each test, associate an animation with one or more of its
*              properties, including: DoubleAnimation, ColorAnimation, DoubleAnimation, etc.
*          (d) Use the AnimationValidator to carry out verification during and after the animation.
*     Pass Conditions:
*          The test passes if (a) the animation's GetValue() API returns the correct value, and
*          (b) the actual rendering matches the expected rendering during the course of the animation.
*     How verified:
*          The SideBySideVerifier routine is called to (a) verify values during the animation, and
*          (b) verify rendering as well using VScan to compare the expected vs. actual bitmaps at
*          specified intervals during the animation (controlled by ClockManager).
*
*     Framework:          An executable is created.
*     Area:               AnimationAndTiming
*     Dependencies:       TestRuntime.dll
*     Support Files:          
**********************************************************/
using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Media.Animation;
     
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.Elements.Shapes</area>
    /// <priority>1</priority>
    /// <description>
    /// Full verification of animation of Ellipse Dependency properties.
    /// </description>
    /// </summary>
    [Test(1, "Animation.Elements.Shapes", "EllipseTest")]

    public class EllipseTest : WindowTest
    {
        #region Test case members
        
        private ClockManager            _clockManager;
        private SideBySideVerifier      _sideBySide;
        private string                  _inputString = "";

        private Canvas                  _body;
        private Ellipse                 _ellipse;
        private Color                   _colorBody            = Colors.Navy;
        private Color                   _colorShape1          = Colors.Orange;
        private Color                   _colorShape2          = Colors.Purple;
        private Color                   _colorShape3          = Colors.MediumAquamarine;
        private Color                   _colorShape4          = Colors.Wheat;
        private Color                   _colorShape5          = Colors.Blue;
        private string                  _windowTitle          = "Shape Animations";
        private int                     _BEGIN_TIME           = 3000;
        private int                     _ANIM_DURATION_TIME   = 3000;
        private AnimationClock          _clock1               = null;
        private AnimationClock          _clock2               = null;

        private double                  _left;
        private double                  _top;
        private double                  _width;
        private double                  _height;
        private double                  _animFrom;
        private double                  _animTo;
        private double                  _animFromDouble;
        private double                  _animToDouble;
        private double                  _animByDouble;
        private Color                   _animFromColor;
        private Color                   _animToColor;
        private double                  _thickness;
        private bool                    _testPassed        = true;

        AnimationValidator _myValidator = new AnimationValidator();

        #endregion


        #region Constructor

        [Variation("Top")]
        [Variation("LeftTop")]
        [Variation("StrokeThickness")]
        [Variation("ScaleXY")]
        [Variation("RadiusXClip")]
        [Variation("CenterClip")]
        [Variation("Height", Priority=0)]
        [Variation("Opacity")]
        [Variation("OpacitySCB")]
        // [DISABLE WHILE PORTING]
        // [Variation("ColorSCB")]
        [Variation("RotateTransform", Priority=0)]
        [Variation("MoveAway")]
        [Variation("GradientStopOffset")]
        [Variation("StrokeDashOffset")]
        [Variation("Margin")]
        [Variation("OpacityMask")]

        /******************************************************************************
        * Function:          EllipseTest Constructor
        ******************************************************************************/
        public EllipseTest(string testValue)
        {
            _inputString = testValue;
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(Verify);
        }

        #endregion

        #region Test Steps

        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Initialize: create a new Window and add an Ellipse to it.
        /// </summary>
        /// <returns></returns>
        TestResult Initialize()
        {
            GlobalLog.LogEvidence("---Initialize--- [Variation=" + _inputString + "]");

            Window.Title            = _windowTitle;
            Window.Left             = 0;
            Window.Top              = 0;
            Window.Height           = 400;
            Window.Width            = 400;
            Window.WindowStyle      = WindowStyle.None;
            Window.ContentRendered  += new EventHandler(OnContentRendered);

            _body  = new Canvas();
            Window.Content = _body;
            _body.Width           = 400;
            _body.Height          = 400;
            Canvas.SetTop  (_body, 0d);
            Canvas.SetLeft (_body, 0d);

            _ellipse  = new Ellipse();               
            _body.Children.Add(_ellipse);   //Properties are added later, specific to each test.
            
            GlobalLog.LogStatus("--------Window created----");
            
            return TestResult.Pass;
        }
        
        
        /******************************************************************************
        * Function:          OnContentRendered
        ******************************************************************************/
        /// <summary>
        /// Fires when the page loads and the page content is finished rendering.
        /// Starting the Animation here.
        /// </summary>
        /// <returns></returns>
        private void OnContentRendered(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("----OnContentRendered----");

            //Set up the Clock Manager to control the Clock. This pauses the Root timeline.
            int[] times = new int[]{2500,3500,4500,5500,6500};
            _clockManager = new ClockManager(times);
            ClockManager.Ticked += new TickEvent(OnTimeTicked);
            GlobalLog.LogStatus("---Clock Manager set up---");

            //Create an instance of the SideBySideVerifier, passing the Window.
            _sideBySide = new SideBySideVerifier(Window);

            CreateAnimation();
        }
          
        /******************************************************************************
        * Function:          CreateAnimation
        ******************************************************************************/
        /// <summary>
        /// Establish an Animation.
        /// </summary>
        /// <returns></returns>
        public void CreateAnimation()
        {
/*
            //Use Reflection to invoke the proper animation test method, based on inputString.
            //string methodName = "Microsoft.Test.Animation.Animate" + inputString;
            string methodName = "Animate" + inputString;
            Type typeOfTestCase = Type.GetType("Microsoft.Test.Animation.EllipseTest");
            object[] args = new object[1];
            args[0] = "testValue";
            object instanceOfTestCase = Activator.CreateInstance( typeOfTestCase, args );
            MethodInfo methodTest = typeOfTestCase.GetMethod(methodName);

//THIS WORKS, BUT THE METHOD INVOKED CANNOT FIND GLOBAL VARIABLES OF THE CLASS.
            methodTest.Invoke( instanceOfTestCase, null );
            //methodTest.Invoke( instanceOfTestCase, BindingFlags.InvokeMethod, null, args, null );

            clockManager.hostManager.Resume();
            GlobalLog.LogStatus("---Root Timeline Resumed---");
*/
            try
            {
                switch (_inputString)
                {
                    case "Top":
                        AnimateTop();
                        break;
                    case "LeftTop":
                        AnimateLeftTop();
                        break;
                    case "StrokeThickness":
                        AnimateStrokeThickness();
                        break;
                    case "ScaleXY":
                        AnimateScaleXY();
                        break;
                    case "RadiusXClip":
                        AnimateRadiusXClip();
                        break;
                    case "CenterClip":
                        AnimateCenterClip();
                        break;
                    case "Height":
                        AnimateHeight();
                        break;
                    case "Opacity":
                        AnimateOpacity();
                        break;
                    case "OpacitySCB":
                        AnimateOpacitySCB();
                        break;
                    case "ColorSCB":
                        AnimateColorSCB();
                        break;
                    case "RotateTransform":
                        AnimateRotateTransform();
                        break;
                    case "MoveAway":
                        AnimateMoveAway();
                        break;
                    case "GradientStopOffset":
                        AnimateGradientStopOffset();
                        break;
                    case "StrokeDashOffset":
                        AnimateStrokeDashOffset();
                        break;
                    case "Margin":
                        AnimateMargin();
                        break;
                    case "OpacityMask":
                        //Verification not working.
                        AnimateOpacityMask();
                        break;
                    default:
                        GlobalLog.LogEvidence("ERROR!!! CreateAnimation: Incorrect variation specified: " + _inputString);
                        _testPassed = false;
                        Signal("TestFinished", TestResult.Fail);
                        break;
                }

                _clockManager.hostManager.Resume();
                GlobalLog.LogStatus("---Root Timeline Resumed---");
            }
            catch (Exception e1)
            {
                GlobalLog.LogEvidence("Exception 1 caught: " + e1.ToString());
                throw;
            }
        }


        /******************************************************************************
        * Function:          AnimateTop
        ******************************************************************************/
        //TEST 0: TOP --- Double Animation of the Top Property.
        public void AnimateTop()
        {
            _left          = 150d;
            _top           = 150d;
            _width         = 100d;
            _height        = 100d;
            _animFrom      = 150d;
            _animTo        = 10d;
            SetEllipseProperties(_ellipse, Brushes.Orange, _left, _top, _width, _height);

            DoubleAnimation animTop0 = new DoubleAnimation();
            animTop0.From            = _animFrom;
            animTop0.To              = _animTo;
            animTop0.BeginTime       = TimeSpan.FromMilliseconds(_BEGIN_TIME);
            animTop0.Duration        = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION_TIME));
            animTop0.FillBehavior    = FillBehavior.HoldEnd;

            _clock1 = animTop0.CreateClock();
            _ellipse.ApplyAnimationClock(Canvas.TopProperty, _clock1);

            //Register an animation for verification, passing the animated DO and DP.
            _sideBySide.RegisterAnimation(_ellipse,Canvas.TopProperty, _clock1);
        }
          

        /******************************************************************************
        * Function:          AnimateLeftTop
        ******************************************************************************/
        //TEST 1: LEFTTOP --- Double Animation of the Top and Left properties.
        public void AnimateLeftTop()
        {
            _left          = -50d;
            _top           = -50d;
            _width         = 50d;
            _height        = 50d;
            _animTo        = 200d;

            SetEllipseProperties(_ellipse, Brushes.Purple, _left, _top, _width, _height);

            DoubleAnimation animTop1a = new DoubleAnimation();
            animTop1a.To             = _animTo;
            animTop1a.BeginTime      = TimeSpan.FromMilliseconds(_BEGIN_TIME);
            animTop1a.Duration       = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION_TIME));
            animTop1a.FillBehavior   = FillBehavior.HoldEnd;

            DoubleAnimation animTop1b = new DoubleAnimation();
            animTop1b.To             = _animTo;
            animTop1b.BeginTime      = TimeSpan.FromMilliseconds(_BEGIN_TIME);
            animTop1b.Duration       = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION_TIME));
            animTop1b.FillBehavior   = FillBehavior.HoldEnd;

            _clock1 = animTop1a.CreateClock();
            _ellipse.ApplyAnimationClock(Canvas.TopProperty, _clock1);
            _clock2 = animTop1b.CreateClock();
            _ellipse.ApplyAnimationClock(Canvas.LeftProperty, _clock2);

            //Register an animation for verification, passing the animated DO and DP.
            _sideBySide.RegisterAnimation(_ellipse,Canvas.TopProperty, _clock1);
            _sideBySide.RegisterAnimation(_ellipse,Canvas.LeftProperty, _clock2);
        }

        /******************************************************************************
        * Function:          AnimateStrokeThickness
        ******************************************************************************/
        //TEST 3: STROKETHICKNESS --- Double Animation of the StrokeThickness Property.
     
        public void AnimateStrokeThickness()
        {
            _ellipse.Stroke = Brushes.MediumVioletRed;
            _left          = 90d;
            _top           = 70d;
            _width         = 20d;
            _height        = 60d;
            _animFrom      = 2d;
            _animTo        = 30d;

            SetEllipseProperties(_ellipse, Brushes.MediumTurquoise, _left, _top, _width, _height);

            DoubleAnimation animStrokeThickness = new DoubleAnimation();
            animStrokeThickness.From           = _animFrom;
            animStrokeThickness.To             = _animTo;
            animStrokeThickness.BeginTime      = TimeSpan.FromMilliseconds(_BEGIN_TIME);
            animStrokeThickness.Duration       = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION_TIME));
            animStrokeThickness.FillBehavior   = FillBehavior.HoldEnd;

            _clock1 = animStrokeThickness.CreateClock();
            _ellipse.ApplyAnimationClock(Ellipse.StrokeThicknessProperty, _clock1);

            //Register an animation for verification, passing the animated DO and DP.
            _sideBySide.RegisterAnimation(_ellipse, Ellipse.StrokeThicknessProperty, _clock1);
        }

        /******************************************************************************
        * Function:          AnimateScaleXY
        ******************************************************************************/
        //TEST 4: SCALEXY --- Double Animation applied to ScaleXAnimations & ScaleYAnimations via a Transform.
        public void AnimateScaleXY()
        {
            _left                 = 170d;
            _top                  = 170d;
            _width                = 60d;
            _height               = 60d;
            _animFromDouble       = 1d;
            _animToDouble         = 2d;

            SetEllipseProperties(_ellipse, Brushes.Orange, _left, _top, _width, _height);

            DoubleAnimation animScaleXY = new DoubleAnimation();                                             
            animScaleXY.FillBehavior      = FillBehavior.HoldEnd;
            animScaleXY.BeginTime         = TimeSpan.FromMilliseconds(_BEGIN_TIME);
            animScaleXY.Duration          = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION_TIME));
            animScaleXY.From              = _animFromDouble;
            animScaleXY.To                = _animToDouble;

            ScaleTransform scaleTransform = new ScaleTransform();
            //scaleTransform.CenterX     = 200;
            //scaleTransform.CenterY     = 200;
            scaleTransform.ScaleX     = 1.5;
            scaleTransform.ScaleY     = 1.5;

            _clock1 = animScaleXY.CreateClock();
            scaleTransform.ApplyAnimationClock(ScaleTransform.ScaleXProperty, _clock1);
            _clock2 = animScaleXY.CreateClock();
            scaleTransform.ApplyAnimationClock(ScaleTransform.ScaleYProperty, _clock2);

            _ellipse.RenderTransform = scaleTransform;

            //Register an animation for verification, passing the animated DO and DP.
            _sideBySide.RegisterAnimation(scaleTransform, ScaleTransform.ScaleXProperty, _clock1);
            _sideBySide.RegisterAnimation(scaleTransform, ScaleTransform.ScaleYProperty, _clock2);
        }

        /******************************************************************************
        * Function:          AnimateRadiusXClip
        ******************************************************************************/
        //TEST 5:  DoubleAnimation applied to an EllipseGeometry's RadiusX property.
        public void AnimateRadiusXClip()
        {
            _left          = 0d;
            _top           = 0d;
            _width         = 300d;
            _height        = 300d;
            _animFrom      = 150d;
            _animTo        = 10d;

            SetEllipseProperties(_ellipse, Brushes.Red, _left, _top, _width, _height);

            DoubleAnimation animRadiusXClip = new DoubleAnimation();                                             
            animRadiusXClip.FillBehavior       = FillBehavior.HoldEnd;
            animRadiusXClip.BeginTime          = TimeSpan.FromMilliseconds(_BEGIN_TIME);
            animRadiusXClip.Duration           = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION_TIME));
            animRadiusXClip.From               = 50d;
            animRadiusXClip.To                 = 80d;

            EllipseGeometry EG5 = new EllipseGeometry();
            EG5.RadiusX               = 50d;
            EG5.RadiusY               = 50d;
            EG5.Center                = new Point(100,100);

            _clock1 = animRadiusXClip.CreateClock();
            EG5.ApplyAnimationClock(EllipseGeometry.RadiusXProperty, _clock1);

            _ellipse.Clip = EG5;

            //Register an animation for verification, passing the animated DO and DP.
            _sideBySide.RegisterAnimation(EG5, EllipseGeometry.RadiusXProperty, _clock1);
        }

        /******************************************************************************
        * Function:          AnimateCenterClip
        ******************************************************************************/
        //TEST 6: 
        public void AnimateCenterClip()
        {
            _left          = 0d;
            _top           = 0d;
            _width         = 200d;
            _height        = 200d;
            _animFrom      = 150;
            _animTo        = 10;

            SetEllipseProperties(_ellipse, Brushes.Brown, _left, _top, _width, _height);

            PointAnimation animCenterClip = new PointAnimation();
            animCenterClip.From           = new Point(100,100);
            animCenterClip.To             = new Point(200,200);
            animCenterClip.BeginTime      = TimeSpan.FromMilliseconds(_BEGIN_TIME);
            animCenterClip.Duration       = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION_TIME));
            animCenterClip.FillBehavior = FillBehavior.HoldEnd;

            EllipseGeometry EG6 = new EllipseGeometry();
            EG6.RadiusX               = 50d;
            EG6.RadiusY               = 50d;
            EG6.Center                = new Point(100,100);

            _clock1 = animCenterClip.CreateClock();
            EG6.ApplyAnimationClock(EllipseGeometry.CenterProperty, _clock1);

            _ellipse.Clip = EG6;

            //Register an animation for verification, passing the animated DO and DP.
            _sideBySide.RegisterAnimation(EG6, EllipseGeometry.CenterProperty, _clock1);
        }


        /******************************************************************************
        * Function:          AnimateHeight
        ******************************************************************************/
        //TEST 8: HEIGHT --- Double Animation of the Height Property.
        public void AnimateHeight()
        {
            _left              = 150d;
            _top               = 150d;
            _width             = 90d;
            _height            = 30d;
            _animFrom          = 90d;
            _animTo            = 30d;

            SetEllipseProperties(_ellipse, Brushes.Wheat, _left, _top, _width, _height);

            DoubleAnimation animHeight = new DoubleAnimation();
            animHeight.From               = _animFrom;
            animHeight.To                 = _animTo;
            animHeight.BeginTime          = TimeSpan.FromMilliseconds(_BEGIN_TIME);
            animHeight.Duration           = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION_TIME));
            animHeight.FillBehavior       = FillBehavior.HoldEnd;

            _clock1 = animHeight.CreateClock();
            _ellipse.ApplyAnimationClock(Ellipse.HeightProperty, _clock1);

            //Register an animation for verification, passing the animated DO and DP.
            _sideBySide.RegisterAnimation(_ellipse, Ellipse.HeightProperty, _clock1);
        }

        /******************************************************************************
        * Function:          AnimateOpacity
        ******************************************************************************/
        //TEST 9: OPACITY --- Double Animation of the Opacity Property.
        public void AnimateOpacity()
        {
            _left             = 50d;
            _top              = 50d;
            _width            = 100d;
            _height           = 100d;
            _animFromDouble   = 0d; //Not set in this animation, but used in verification.
            _animToDouble     = 1d;

            SetEllipseProperties(_ellipse, Brushes.Blue, _left, _top, _width, _height);

            _ellipse.Opacity = 0; //Starting out invisible.

            DoubleAnimation animOpacity = new DoubleAnimation();                                             
            animOpacity.FillBehavior      = FillBehavior.HoldEnd;
            animOpacity.BeginTime         = TimeSpan.FromMilliseconds(_BEGIN_TIME);
            animOpacity.Duration          = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION_TIME));
            animOpacity.To                = _animToDouble;

            _clock1 = animOpacity.CreateClock();
            _ellipse.ApplyAnimationClock(Ellipse.OpacityProperty, _clock1);

            //Register an animation for verification, passing the animated DO and DP.
            _sideBySide.RegisterAnimation(_ellipse, Ellipse.OpacityProperty, _clock1);
        }

        /******************************************************************************
        * Function:          AnimateOpacitySCB
        ******************************************************************************/
        //TEST 9: OPACITYSCB --- Double Animation of the Opacity Property via a SolidColorBrush.
        public void AnimateOpacitySCB()
        {
            _left                     = 190d;
            _top                      = 190d;
            _width                    = 20d;
            _height                   = 20d;
            _animFromDouble           = 1d;
            _animToDouble             = 0d;

            SetEllipseProperties(_ellipse, Brushes.Blue, _left, _top, _width, _height);

            DoubleAnimation animOpacitySCB = new DoubleAnimation();                                             
            animOpacitySCB.FillBehavior        = FillBehavior.HoldEnd;
            animOpacitySCB.BeginTime           = TimeSpan.FromMilliseconds(_BEGIN_TIME);
            animOpacitySCB.Duration            = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION_TIME));
            animOpacitySCB.From                = _animFromDouble;
            animOpacitySCB.To                  = _animToDouble;

            SolidColorBrush SCB = new SolidColorBrush();
            SCB.Color = Colors.Blue;

            _clock1 = animOpacitySCB.CreateClock();
            SCB.ApplyAnimationClock(SolidColorBrush.OpacityProperty, _clock1);

            _ellipse.Fill = SCB;

            //Register an animation for verification, passing the animated DO and DP.
            _sideBySide.RegisterAnimation(SCB, SolidColorBrush.OpacityProperty, _clock1);
        }

        /******************************************************************************
        * Function:          AnimateColorSCB
        ******************************************************************************/
        //TEST 10: COLORSCB --- ColorAnimation of the Color Property via a SolidColorBrush.
        public void AnimateColorSCB()
        {
            _left                     = 50d;
            _top                      = 50d;
            _width                    = 300d;
            _height                   = 300d;
            _animFromColor            = Colors.Blue;
            _animToColor              = Colors.Black;
            _thickness                = 15d;

            SetEllipseProperties(_ellipse, Brushes.Blue, _left, _top, _width, _height);

            //The thickness "overlaps" half over the ellipse, and half outside of it.
            _ellipse.StrokeThickness = _thickness;

            ColorAnimation animStroke = new ColorAnimation();
            animStroke.FillBehavior       = FillBehavior.HoldEnd;
            animStroke.BeginTime          = TimeSpan.FromMilliseconds(_BEGIN_TIME);
            animStroke.Duration           = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION_TIME));
            animStroke.From               = _animFromColor;
            animStroke.To                 = _animToColor;

            SolidColorBrush SCB = new SolidColorBrush();
            SCB.Color = _animFromColor;

            _clock1 = animStroke.CreateClock();
            SCB.ApplyAnimationClock(SolidColorBrush.ColorProperty, _clock1);

            _ellipse.Stroke = SCB;

            //Register an animation for verification, passing the animated DO and DP.
            _sideBySide.RegisterAnimation(SCB, SolidColorBrush.ColorProperty, _clock1);
        }

        /******************************************************************************
        * Function:          AnimateRotateTransform
        ******************************************************************************/
        //TEST 11: RotateTransform --- DoubleAnimation applied to a RotateTransform's AngleProperty.
        public void AnimateRotateTransform()
        {
            _left                 = 120d;
            _top                  = 90d;
            _width                = 60d;
            _height               = 120d;
            _animFromDouble       = 0d;
            _animToDouble         = 90d;

            SetEllipseProperties(_ellipse, Brushes.MediumAquamarine, _left, _top, _width, _height);

            DoubleAnimation animAngle = new DoubleAnimation();                                             
            animAngle.FillBehavior        = FillBehavior.HoldEnd;
            animAngle.BeginTime           = TimeSpan.FromMilliseconds(_BEGIN_TIME);
            animAngle.Duration            = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION_TIME));
            animAngle.From                = _animFromDouble;
            animAngle.To                  = _animToDouble;

            RotateTransform rotateTransform = new RotateTransform();
            rotateTransform.Angle              = 0d;
            rotateTransform.CenterX             = 30;
            rotateTransform.CenterY             = 60;

            _clock1 = animAngle.CreateClock();
            rotateTransform.ApplyAnimationClock(RotateTransform.AngleProperty, _clock1);

            _ellipse.RenderTransform = rotateTransform;

            //Register an animation for verification, passing the animated DO and DP.
            _sideBySide.RegisterAnimation(rotateTransform,RotateTransform.AngleProperty, _clock1);
        }

        /******************************************************************************
        * Function:          AnimateMoveAway
        ******************************************************************************/
        //TEST 12: MoveAway --- Use Left/Top to move one ellipse away from another.
        public void AnimateMoveAway()
        {
            _left             = 50d;
            _top              = 50d;
            _width            = 100d;
            _height           = 100d;

            //Main ellipse.
            SetEllipseProperties(_ellipse, Brushes.Purple, _left, _top, _width, _height);

            //Create an ellipse that partially covers the main ellipse; it will move.
            Ellipse ellipse12  = new Ellipse();
            double left12 = 75d;
            SetEllipseProperties(ellipse12, Brushes.Orange, left12, _top, _width, _height);
            _body.Children.Add(ellipse12);
            _animTo = 250;               

            DoubleAnimation animLeftTop = new DoubleAnimation();
            animLeftTop.To               = _animTo;
            animLeftTop.BeginTime        = TimeSpan.FromMilliseconds(_BEGIN_TIME);
            animLeftTop.Duration         = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION_TIME));
            animLeftTop.FillBehavior     = FillBehavior.HoldEnd;

            _clock1 = animLeftTop.CreateClock();
            ellipse12.ApplyAnimationClock(Canvas.LeftProperty, _clock1);
            _clock2 = animLeftTop.CreateClock();
            ellipse12.ApplyAnimationClock(Canvas.TopProperty, _clock2);

            //Register an animation for verification, passing the animated DO and DP.
            _sideBySide.RegisterAnimation(ellipse12, Canvas.LeftProperty, _clock1);
            _sideBySide.RegisterAnimation(ellipse12, Canvas.TopProperty, _clock2);
        }

        /******************************************************************************
        * Function:          AnimateGradientStopOffset
        ******************************************************************************/
        //TEST 13: GRADIENTSTOP OFFSET - Animate the Offset property of the Ellipse's Gradient Stop.
        public void AnimateGradientStopOffset()
        {
            _left               = 150d;
            _top                = 150d;
            _width              = 150d;
            _height             = 150d;
            _animFromDouble     = 0.2d;
            _animToDouble       = 0.9d;

            SetEllipseProperties(_ellipse, Brushes.Red, _left, _top, _width, _height);

            DoubleAnimation animOffset = new DoubleAnimation();                                             
            animOffset.FillBehavior       = FillBehavior.HoldEnd;
            animOffset.BeginTime          = TimeSpan.FromMilliseconds(_BEGIN_TIME);
            animOffset.Duration           = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION_TIME));
            animOffset.From               = _animFromDouble;
            animOffset.To                 = _animToDouble;

            LinearGradientBrush LGB = new LinearGradientBrush();
            LGB.StartPoint = new Point(0.0, 0.0);
            LGB.EndPoint   = new Point(1.0, 1.0);
            LGB.MappingMode = BrushMappingMode.RelativeToBoundingBox;

            GradientStop GS1 = new GradientStop(Colors.RoyalBlue, 0.0);
            GradientStop GS2 = new GradientStop(Colors.SteelBlue, 0.2);
            GradientStop GS3 = new GradientStop(Colors.LightBlue, 1.0);
            GradientStopCollection GSC = new GradientStopCollection();
            GSC.Add(GS1);
            GSC.Add(GS2);
            GSC.Add(GS3);

            _clock1 = animOffset.CreateClock();
            GSC[1].ApplyAnimationClock(GradientStop.OffsetProperty, _clock1);

            LGB.GradientStops = GSC;
            _ellipse.Fill = LGB;

            //Register an animation for verification, passing the animated DO and DP.
            _sideBySide.RegisterAnimation(GSC[1], GradientStop.OffsetProperty, _clock1);
        }

        /******************************************************************************
        * Function:          AnimateStrokeDashOffset
        ******************************************************************************/
        //TEST 14: STROKEDASHOFFSET --- Double Animation of the StrokeThickness Property.
        public void AnimateStrokeDashOffset()
        {
            _ellipse.Stroke                   = Brushes.Green;
            _ellipse.StrokeThickness          = 8d;
            _ellipse.StrokeDashOffset         = 2d;
            _ellipse.StrokeStartLineCap       = PenLineCap.Flat;
            _ellipse.StrokeEndLineCap         = PenLineCap.Triangle;
            _ellipse.StrokeDashCap            = PenLineCap.Round;
            _ellipse.StrokeLineJoin           = PenLineJoin.Bevel;

            _ellipse.StrokeMiterLimit         = 100d;
            DoubleCollection dc              = new DoubleCollection();
            dc.Add(1);
            dc.Add(1);             
            _ellipse.StrokeDashArray          = dc;

            _left               = 100d;
            _top                = 100d;
            _width              = 200d;
            _height             = 200d;
            _animByDouble       = 20d;

            SetEllipseProperties(_ellipse, Brushes.Black, _left, _top, _width, _height);

            DoubleAnimation animDashOffset = new DoubleAnimation();                                             
            animDashOffset.FillBehavior        = FillBehavior.HoldEnd;
            animDashOffset.BeginTime           = TimeSpan.FromMilliseconds(_BEGIN_TIME);
            animDashOffset.Duration            = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION_TIME));
            animDashOffset.By                  = _animByDouble;

            _clock1 = animDashOffset.CreateClock();
            _ellipse.ApplyAnimationClock(Ellipse.StrokeDashOffsetProperty, _clock1);

            //Register an animation for verification, passing the animated DO and DP.
            _sideBySide.RegisterAnimation(_ellipse, Ellipse.StrokeDashOffsetProperty, _clock1);
        }

        /******************************************************************************
        * Function:          AnimateMargin
        ******************************************************************************/
        //TEST 15: MARGIN --- Thickness Animation of the Margin Property.
        public void AnimateMargin()
        {
            _ellipse.Margin                   = new Thickness(1, 1, 1, 1);
            _ellipse.Stroke                   = Brushes.Red;
            _ellipse.StrokeThickness          = 8d;
            _ellipse.StrokeDashOffset         = 5d;
            _ellipse.StrokeStartLineCap       = PenLineCap.Round;
            _ellipse.StrokeEndLineCap         = PenLineCap.Round;
            _ellipse.StrokeDashCap            = PenLineCap.Round;
            _ellipse.StrokeLineJoin           = PenLineJoin.Round;
            _ellipse.StrokeMiterLimit         = 100;

            _ellipse.StrokeDashArray          = new DoubleCollection();
            _ellipse.StrokeDashArray.Add(1);
            _ellipse.StrokeDashArray.Add(1);           

            _left               = 100d;
            _top                = 100d;
            _width              = 200d;
            _height             = 200d;
            _animByDouble       = 20d;

            SetEllipseProperties(_ellipse, Brushes.Yellow, _left, _top, _width, _height);

            ThicknessAnimation animThickness = new ThicknessAnimation();
            animThickness.FillBehavior    = FillBehavior.HoldEnd;
            animThickness.BeginTime       = TimeSpan.FromMilliseconds(_BEGIN_TIME);
            animThickness.Duration        = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION_TIME));
            animThickness.By              = new Thickness(9, 9, 9, 9);

            _clock1 = animThickness.CreateClock();
            _ellipse.ApplyAnimationClock(Ellipse.MarginProperty, _clock1);

            //Register an animation for verification, passing the animated DO and DP.
            _sideBySide.RegisterAnimation(_ellipse, Ellipse.MarginProperty, _clock1);
        }

        /******************************************************************************
        * Function:          AnimateOpacityMask
        ******************************************************************************/
        //TEST 16: OPACITYMASK - Animate the OffsetProperty of a LinearGradientBrush via an
        //OpacityMask.
        public void AnimateOpacityMask()
        {
            _left               = 150d;
            _top                = 150d;
            _width              = 150d;
            _height             = 150d;
            _animFromDouble     = 0d;
            _animToDouble       = 0.8d;

            SetEllipseProperties(_ellipse, Brushes.Azure, _left, _top, _width, _height);

            DoubleAnimation animOffset = new DoubleAnimation();                                             
            animOffset.FillBehavior       = FillBehavior.HoldEnd;
            animOffset.BeginTime          = TimeSpan.FromMilliseconds(_BEGIN_TIME);
            animOffset.Duration           = new Duration(TimeSpan.FromMilliseconds(_ANIM_DURATION_TIME));
            animOffset.From               = _animFromDouble;
            animOffset.To                 = _animToDouble;

            LinearGradientBrush LGB = new LinearGradientBrush();
            LGB.Opacity         = .8d;
            LGB.StartPoint      = new Point(0.0, 0.0);
            LGB.EndPoint        = new Point(1.0, 1.0);
            LGB.MappingMode     = BrushMappingMode.Absolute;
            LGB.SpreadMethod    = GradientSpreadMethod.Reflect;

            GradientStop GS1 = new GradientStop(Colors.Lavender, 0.0);
            GradientStop GS2 = new GradientStop(Colors.Thistle, 0.5);
            GradientStop GS3 = new GradientStop(Colors.MediumPurple, 1.0);
            GradientStopCollection GSC = new GradientStopCollection();
            GSC.Add(GS1);
            GSC.Add(GS2);
            GSC.Add(GS3);

            _clock1 = animOffset.CreateClock();
            GSC[1].ApplyAnimationClock(GradientStop.OffsetProperty, _clock1);

            LGB.GradientStops = GSC;
            _ellipse.OpacityMask = LGB;

            //Register an animation for verification, passing the animated DO and DP.
            _sideBySide.RegisterAnimation(GSC[0], GradientStop.OffsetProperty, _clock1);
        }

        /******************************************************************************
        * Function:          SetEllipseProperties
        ******************************************************************************/
        private void SetEllipseProperties(  Ellipse e,
                                            Brush color,
                                            Object left,
                                            Object top,
                                            Object width,
                                            Object height)
        {
            if (color != null)
                e.Fill = color;                    
            if (left != null)
                Canvas.SetLeft(e, (double)left);
            if (top != null)
                Canvas.SetTop(e, (double)top);
            if (width != null)
                e.Width       = (double)width;
            if (height != null)
                e.Height      = (double)height;
        }

        /******************************************************************************
        * Function:          OnTimeTicked
        ******************************************************************************/
        /// <summary>
        /// OnTimeTicked: Calls verification routines every tick.
        /// </summary>
        private void OnTimeTicked(object sender, TimeControlArgs e)
        {
            GlobalLog.LogEvidence("---------------------------------------------");
            if (e.lastTick)
            {
                GlobalLog.LogEvidence("---Last Tick---");
            }

            _sideBySide.toleranceInPercent = .25;

            //Verify Values.
            GlobalLog.LogEvidence("VERIFICATION RESULTS");
            bool valueResult = _sideBySide.ValuesOnlyVerify(e.curTime);
            GlobalLog.LogEvidence("  VALUES:    " + valueResult);

            //Verify Rendering.
            bool renderResult = _sideBySide.Verify(e.curTime);
            GlobalLog.LogEvidence("  RENDERING: " + renderResult);
            GlobalLog.LogEvidence(_sideBySide.verboseLog);

            _testPassed = (_testPassed && (valueResult && renderResult));

            if (e.lastTick)
            {
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
