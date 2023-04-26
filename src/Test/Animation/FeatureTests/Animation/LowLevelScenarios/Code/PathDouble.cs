// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* ***************  MIL Integration Test *****************
*     Major Actions:
*          (a) Create a new window via OnStartup.
*          (b) Create a Path, and for each test, associate an animation with one or more of its properties
*          (d) Use the TimeEventHandler to carry out verification after the animation is complete.
*     Pass Conditions:
*          The test passes if the actual rendering matches the expected rendering during the 
*          course of the animation.
*     How verified:
*          A simplified rendering verification is used, consisting of Bitmap comparisons of the 
*          page captured before, during, and after the Animation.  The test case passes if each of
*          the three pair-wise comparisons show a difference in rendering.
*
*     Framework:          An executable is created.
*     Area:               Animation/Timing
*     Dependencies:       TestRuntime.dll
*     Support Files:          
**********************************************************/
using System;
using System.Globalization;
using System.Threading;
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

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.LowLevelScenarios.Shapes</area>
    /// <priority>1</priority>
    /// <description>
    /// Verify rendering of a DoubleAnimationUsingPath under various conditions.
    /// </description>
    /// </summary>
    [Test(2, "Animation.LowLevelScenarios.Shapes", "PathDoubleTest")]

    class PathDoubleTest : WindowTest
    {
        #region Test case members

        private ClockManager                    _clockManager;

        private Canvas                          _body;
        private Canvas                          _container;
        private Path                            _path1;
        private Button                          _button1;
        private Decorator                       _dec;
        private AnimationClock                  _clock               = null;
        private string                          _windowTitle         = "DoubleAnimationUsingPath";
        private TimeSpan                        _BEGIN_TIME          = TimeSpan.FromMilliseconds(2000);
        private TimeSpan                        _ANIM_DURATION_TIME  = TimeSpan.FromMilliseconds(3000);
        private int                             _tickCount           = 0;
        private string                          _parm1               = null;
        private string                          _parm2               = null;
        private bool                            _testPassed          = false;

        private ImageComparator                 _imageCompare        = new ImageComparator();
        private System.Drawing.Rectangle        _clientRect;
        private System.Drawing.Bitmap           _beforeCapture;
        private System.Drawing.Bitmap           _betweenCapture;
        private System.Drawing.Bitmap           _afterCapture;

        #endregion


        #region Constructor
        // [DISABLE WHILE PORTING]
        // [Variation("Angle", "1", Priority=1)]
        // [Variation("Angle", "2", Priority=0)]
        // [Variation("Angle", "3")]
        // [Variation("Angle", "4")]

        [Variation("X", "1", Disabled=true)]   // Flakey test: disabling - 09-22-10.
        [Variation("X", "2", Disabled=true)]   // Flakey test: disabling - 09-22-10.
        // [Variation("X", "3")]
        // [Variation("X", "4")]
        
        // [Variation("Y", "1")]
        // [Variation("Y", "2")]
        // [Variation("Y", "3")]
        // [Variation("Y", "4")]

        /******************************************************************************
        * Function:          PathDoubleTest Constructor
        ******************************************************************************/
        public PathDoubleTest(string testValue1, string testValue2)
        {
            _parm1 = testValue1;
            _parm2 = testValue2;
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
            bool        arg2Found   = false;
            string      errMessage  = "";
            string[]    expList1    = { "Angle", "X", "Y" };
            string[]    expList2    = { "1", "2", "3", "4" };

            arg1Found = AnimationUtilities.CheckInputString(_parm1, expList1, ref errMessage);
            if (errMessage != "")
            {
                GlobalLog.LogEvidence("ERROR!!! CheckInputParameters -- 1st Parameter: " + errMessage);
            }
            else
            {
                arg2Found = AnimationUtilities.CheckInputString(_parm2, expList2, ref errMessage);
                if (errMessage != "")
                {
                    GlobalLog.LogEvidence("ERROR!!! CheckInputParameters -- 2nd Parameter: " + errMessage);
                }
            }

            if (arg1Found && arg2Found)
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
        /// CreateWindow: create a new Window and add content to it.
        /// </summary>
        /// <returns>TestResult.Pass</returns>
        TestResult CreateWindow()
        {
            GlobalLog.LogStatus("----CreateWindow----");

            Window.Title            = _windowTitle;
            Window.Left             = 0d;
            Window.Top              = 0d;
            Window.Height           = 400d;
            Window.Width            = 400d;
            Window.WindowStyle      = WindowStyle.None;
            Window.ContentRendered  += new EventHandler(OnContentRendered);

            _body  = new Canvas();
            Window.Content = _body;
            _body.Width              = 400d;
            _body.Height             = 400d;
            _body.Background         = Brushes.DarkSeaGreen;
            Canvas.SetTop  (_body, 0d);
            Canvas.SetLeft (_body, 0d);

            _container  = new Canvas();
            _body.Children.Add(_container);
            _container.Width              = 125d;
            _container.Height             = 125d;
            _container.Background         = Brushes.PaleGreen;
            Canvas.SetTop  (_container, 120d);
            Canvas.SetLeft (_container, 120d);

            _button1 = new Button();
            _button1.Height      = 20d;
            _button1.Width       = 20d;
            _button1.Content     = "B";
            _button1.FontSize    = 16d;
            _button1.Padding     = new Thickness(0,0,0,0);

            //Create a Path, with a PathGeometry.
            _path1  = CreatePath();
            _container.Children.Add(_path1);
            GlobalLog.LogStatus("----Window created----");

            //Create a Decorator containing an animated RotateTransform along a Path.
            _dec = CreateDecorator();
            _dec.Child = _button1;
            _container.Children.Add(_dec);

            _clientRect = new System.Drawing.Rectangle((int)Window.Left, (int)Window.Top, (int)Window.Width, (int)Window.Height);              

            return TestResult.Pass;
        }

        /******************************************************************************
           * Function:          OnContentRendered
           ******************************************************************************/
        /// <summary>
        /// OnContentRendered: Invoked when the page loads.
        /// </summary>
        private void OnContentRendered(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("---OnContentRendered Fired---");

            //Set up the Clock Manager to control verification: a check is made just before and just after the animation.
            //ASSUMPTION: the animation in Markup begins at 2000 and has a duration of 3000.
            int[] times = new int[]{1000, 3500, 6000}; // before, between and after
            _clockManager = new ClockManager(times);
            ClockManager.Ticked += new TickEvent(OnTimeTicked);
            _clockManager.hostManager.Resume();
            
            GlobalLog.LogStatus("---End of OnContentRendered---");
        }
          
        /******************************************************************************
        * Function:          CreateDecorator
        ******************************************************************************/
        /// <summary>
        /// CreateDecorator: Creates a Decorator containing an animated MatrixProperty.
        /// </summary>
        /// <returns>Decorator</returns>
        private Decorator CreateDecorator()
        {
            RotateTransform RT = new RotateTransform();
            RT.Angle        = 90;
            RT.CenterX      = 10;
            RT.CenterY      = 10;
            
            TranslateTransform TT = new TranslateTransform();
            TT.X    = 7.5;
            TT.Y    = 7.5;
            
            //Create a TranformGroup, and add the two Transforms to it.
            TransformGroup TGRP = new TransformGroup();
            TGRP.Children.Add(RT);
            TGRP.Children.Add(TT);

            //Create a DoubleAnimationUsingPath, based on the incoming test case parameters.
            DoubleAnimationUsingPath animation = CreateAnimation(RT, TT);

            //Create a Decorator and add the TransformGroup to it.
            Decorator decorator = new Decorator();
            decorator.Margin          = new Thickness(15,15,15,15);
            decorator.RenderTransform = TGRP;
                        
            return decorator;
        }
          
        /******************************************************************************
        * Function:          CreateAnimation
        ******************************************************************************/
        /// <summary>
        /// CreateAnimation: Calls a routine to create a DoubleAnimationUsingPath animation,
        /// based on input parameters.
        /// </summary>
        /// <returns>DoubleAnimationUsingPath</returns>
        private DoubleAnimationUsingPath CreateAnimation(RotateTransform RT, TranslateTransform TT)
        {
            DoubleAnimationUsingPath anim = null;
            
            switch (_parm1)
            {
                case "Angle":
                    //TEST 1-4: Animating DoubleAnimationUsingPath.AngleProperty
                    switch (_parm2)
                    {
                        case "1":
                            //IsAdditive=false, IsCumulative=false
                            anim = CreateDoubleAnimationUsingPath(_parm1, false, false);
                            break;
                        case "2":
                            //IsAdditive=false, IsCumulative=true
                            anim = CreateDoubleAnimationUsingPath(_parm1, false, true);
                            break;
                        case "3":
                            //IsAdditive=true, IsCumulative=false
                            anim = CreateDoubleAnimationUsingPath(_parm1, true, false);
                            break;
                        case "4":
                            //IsAdditive=true, IsCumulative=true
                            anim = CreateDoubleAnimationUsingPath(_parm1, true, true);
                            break;
                        default:
                            GlobalLog.LogEvidence("ERROR!!! CreateAnimation: Incorrect 2nd argument (1).");
                            Signal("TestFinished", TestResult.Fail);
                            break;
                    }
                    _clock = anim.CreateClock();
                    RT.ApplyAnimationClock(RotateTransform.AngleProperty, _clock);
                    break;
                case "X":
                    //TEST 5-8: Animating DoubleAnimationUsingPath.XProperty
                    switch (_parm2)
                    {
                        case "1":
                            //IsAdditive=false, IsCumulative=false
                            anim = CreateDoubleAnimationUsingPath(_parm1, false, false);
                            break;
                        case "2":
                            //IsAdditive=false, IsCumulative=true
                            anim = CreateDoubleAnimationUsingPath(_parm1, false, true);
                            break;
                        case "3":
                            //IsAdditive=true, IsCumulative=false
                            anim = CreateDoubleAnimationUsingPath(_parm1, true, false);
                            break;
                        case "4":
                            //IsAdditive=true, IsCumulative=true
                            anim = CreateDoubleAnimationUsingPath(_parm1, true, true);
                            break;
                        default:
                            GlobalLog.LogEvidence("ERROR!!! CreateAnimation: Incorrect 2nd argument (2).");
                            Signal("TestFinished", TestResult.Fail);
                            break;
                    }
                    _clock = anim.CreateClock();
                    TT.ApplyAnimationClock(TranslateTransform.XProperty, _clock);
                    break;
                case "Y":
                    //TEST 9-12: Animating DoubleAnimationUsingPath.YProperty
                    switch (_parm2)
                    {
                        case "1":
                            //IsAdditive=false, IsCumulative=false
                            anim = CreateDoubleAnimationUsingPath(_parm1, false, false);
                            break;
                        case "2":
                            //IsAdditive=false, IsCumulative=true
                            anim = CreateDoubleAnimationUsingPath(_parm1, false, true);
                            break;
                        case "3":
                            //IsAdditive=true, IsCumulative=false
                            anim = CreateDoubleAnimationUsingPath(_parm1, true, false);
                            break;
                        case "4":
                            //IsAdditive=true, IsCumulative=true
                            anim = CreateDoubleAnimationUsingPath(_parm1, true, true);
                            break;
                        default:
                            GlobalLog.LogEvidence("ERROR!!! CreateAnimation: Incorrect 2nd argument (3).");
                            Signal("TestFinished", TestResult.Fail);
                            break;
                    }
                    _clock = anim.CreateClock();
                    TT.ApplyAnimationClock(TranslateTransform.YProperty, _clock);
                    break;
                default:
                    GlobalLog.LogEvidence("ERROR!!! CreateAnimation: Incorrect 1st argument (1).");
                    Signal("TestFinished", TestResult.Fail);
                    break;
            }
            return anim;
        }
        
        /******************************************************************************
        * Function:          CreateDoubleAnimationUsingPath
        ******************************************************************************/
        /// <summary>
        /// CreateDoubleAnimationUsingPath: Creates a DoubleAnimationUsingPath animation.
        /// </summary>
        /// <returns>DoubleAnimationUsingPath</returns>
        private DoubleAnimationUsingPath CreateDoubleAnimationUsingPath(string animProp, bool parm1, bool parm2)
        {
            DoubleAnimationUsingPath animDUP = new DoubleAnimationUsingPath();
            animDUP.BeginTime                = _BEGIN_TIME;
            animDUP.Duration                 = new Duration(_ANIM_DURATION_TIME);
            animDUP.RepeatBehavior           = new RepeatBehavior(2d);
            animDUP.IsAdditive               = parm1;
            animDUP.IsCumulative             = parm2;

            //Add a PathGeometry to the DoubleAnimationUsingPath.
            //It is also assigned to a Path element to render a path the animation will follow.
            animDUP.PathGeometry = CreatePathGeometry();
            
            switch (animProp)
            {
                case "Angle":
                    animDUP.Source = PathAnimationSource.Angle;
                    break;
                case "X":
                    animDUP.Source = PathAnimationSource.X;
                    break;
                case "Y":
                    animDUP.Source = PathAnimationSource.Y;
                    break;
                default:
                    GlobalLog.LogEvidence("ERROR!!! CreateDoubleAnimationUsingPath: Incorrect 1st argument. (2)");
                    Signal("TestFinished", TestResult.Fail);
                    break;
            }      
            return animDUP;
        }          
          
        /******************************************************************************
        * Function:          CreatePath
        ******************************************************************************/
        /// <summary>
        /// CreatePath: Creates a Path with a PathGeometry.
        /// </summary>
        /// <returns>Path</returns>
        private Path CreatePath()
        {
            Path path  = new Path();
            Canvas.SetTop  (path, 15d);
            Canvas.SetLeft (path, 15d);
            path.Stroke             = Brushes.SteelBlue;
            path.StrokeThickness    = 2d;

            //Create a PathGeometry and assign it to the Path's Data property, to display the path.
            //It is also assigned to the DoubleAnimation.
            path.Data   = CreatePathGeometry();
            
            return path;
        }
        
        /******************************************************************************
        * Function:          CreatePathGeometry
        ******************************************************************************/
        /// <summary>
        /// CreatePathGeometry: Creates a PathGeometry.
        /// </summary>
        /// <returns>PathGeometry</returns>
        private PathGeometry CreatePathGeometry()
        {
            //Create a PathFigureCollection.
            PathFigureCollection PFC1 = SpecialObjects.CreatePathFigureCollection();

            //Create a PathGeometry: animation.
            //Assign it a ScaleTransform and PathFigureCollection.
            ScaleTransform scaleTransform1 = new ScaleTransform();
            scaleTransform1.ScaleX      = 1.5;
            scaleTransform1.ScaleY      = 1.5;

            PathGeometry pathGeometry   = new PathGeometry();
            pathGeometry.Transform      = scaleTransform1;
            pathGeometry.Figures        = PFC1;
            
            return pathGeometry;
        }        

        /******************************************************************************
           * Function:          CaptureTheScreen
          ******************************************************************************/
        /// <summary>
        /// CaptureTheScreen: gets a screen capture and checks for null;
        /// </summary>
        /// <returns>A Bitmap, used for animation verification</returns>
        private System.Drawing.Bitmap CaptureTheScreen()
        {
            System.Drawing.Bitmap tempBitmap = ImageUtility.CaptureScreen(_clientRect);

            return tempBitmap;
        }

        /******************************************************************************
           * Function:          OnTimeTicked
           ******************************************************************************/
        /// <summary>
        /// OnTimeTicked: Calls verification routines.
        /// </summary>
        private void OnTimeTicked(object sender, TimeControlArgs e)
        {
            string outputData   = "";
            
            _tickCount++;
            
            GlobalLog.LogStatus("**********Tick #" + _tickCount + " at time " + e.curTime);

            if (_tickCount == 1 )
            {
                _beforeCapture = CaptureTheScreen();
            }
            else if (_tickCount == 2)
            {
                _betweenCapture = CaptureTheScreen();
            }
            else 
            {
                _afterCapture = CaptureTheScreen();
            }

            //On the last tick, pass/fail the test case, then close the application.           
            if (e.lastTick)
            {                
                GlobalLog.LogStatus("**********Last Tick");

                outputData = "<--------Final Result-------->\n";
                bool pass1 = _imageCompare.Compare(new ImageAdapter(_beforeCapture), new ImageAdapter(_betweenCapture), true);
                bool pass2 = _imageCompare.Compare(new ImageAdapter(_betweenCapture), new ImageAdapter(_afterCapture), true);
                bool pass3 = _imageCompare.Compare(new ImageAdapter(_beforeCapture), new ImageAdapter(_afterCapture), true);      

                if ( (!pass1) && (!pass2) && (!pass3) ) 
                { 
                    _testPassed = true;
                    outputData += "  All screen captures were different\n";
                }

                if (pass1) { outputData += "  Before and Between animation captures were identical \n"; }
                if (pass2) { outputData += "  Between and After animation captures were identical \n"; }
                if (pass3) { outputData += "  Before and After animation captures were identical \n"; }

                GlobalLog.LogEvidence( outputData );
                
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
