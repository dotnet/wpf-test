// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* ***************  MIL Recoverability Test ***************************************************
*     Purpose: To test the AnimationException feature, verifying that the Clock, Property, and 
*     Target properties return the appropriate values when an Animation exception is generated.
*
*     Pass Conditions: A test case passes if (a) an InnerException is returned, and (b) the Clock,
*     Property, and Target properties return expected values. 
*
*     How verified:  Properties of AnimationException are checked when the exception is thrown.
*
*     Framework:          An Avalon executable is created.
*     Area:               Animation/Timing
*     Dependencies:       TestRuntime.dll       
*     Support Files:               
********************************************************************************************/
using System;
using System.Globalization;
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
    /// <area>Animation</area>
    /// <priority>2</priority>
    /// <description>
    /// Verify that the Clock, Property, and Target properties return the appropriate values when an Animation exception is generated.
    /// </description>
    /// </summary>

    [Test(2, "Animation.InputTesting", "AnimationExceptionTest", Keywords = "Localization_Suite")]

    public class AnimationExceptionTest : WindowTest
    {
        #region Test case members
        
        private string                      _windowTitle             = "AnimationException Test";
        private string                      _inputString             = "";
        private DispatcherTimer             _aTimer                  = null;
        private Canvas                      _body;
        private TextBox                     _textbox1                = null;
        private Button                      _button1                 = null;
        private ScrollBar                   _scrollbar1              = null;
        private LinearGradientBrush         _LGB1                    = null;
        private RectangleGeometry           _rectangleGeo            = null;

        private string                      _exceptionMessage        = "";
        private AnimationClock              _clock1                  = null;
        
        private ClockState                  _expectedState;
        private Type                        _expectedElement         = null;
        private DependencyProperty          _expectedProperty        = null;
        private bool                        _testPassed              = false;

        #endregion


        #region Constructor

        //[Variation("NegativeWidth", Priority=1)]  //NOTE: due to change in behavior from EasingFunctions, AnimationException is not thrown anymore. 
        [Variation("NoBaseHeight", Priority=2)]
        //[Variation("NegativeViewportSize", Priority=2)]  //NOTE: due to change in behavior from EasingFunctions, AnimationException is not thrown anymore. 
        [Variation("NoRectProperty", Priority=2)]

        /******************************************************************************
        * Function:          AnimationExceptionTest Constructor
        ******************************************************************************/
        public AnimationExceptionTest(string testValue)
        {
            _inputString = testValue;
            
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(StartAnimation);
        }

        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Initialize: create a new Window and add Elements to it.
        /// </summary>
        /// <returns></returns>
        TestResult Initialize()
        {
            GlobalLog.LogStatus("--------Initialize--------");

            Window.Title            = _windowTitle;
            Window.Left             = 0;
            Window.Top              = 0;
            Window.Height           = 300;
            Window.Width            = 500;
            Window.WindowStyle      = WindowStyle.None;

            NameScope.SetNameScope(Window, new NameScope());

            _body  = new Canvas();
            Window.Content = _body;
            _body.Background     = Brushes.LightGreen;

            _textbox1  = new TextBox();
            _textbox1.Width               = 300;
            _textbox1.Height              = 125;
            _textbox1.FontSize            = 48;
            _textbox1.Text                = "Avalon!";
            _textbox1.Foreground          = new SolidColorBrush(Colors.Brown);
            Canvas.SetTop  (_textbox1, 50d);
            Canvas.SetLeft (_textbox1, 50d);
            _body.Children.Add(_textbox1);

            _LGB1 = new LinearGradientBrush();
            _LGB1.StartPoint = new Point(0.0, 0.0);
            _LGB1.EndPoint = new Point(1.0, 1.0);
            _LGB1.MappingMode = BrushMappingMode.RelativeToBoundingBox;
            GradientStop GS1 = new GradientStop(Colors.Blue, 0.0);
            GradientStop GS2 = new GradientStop(Colors.DodgerBlue, 0.5);
            GradientStop GS3 = new GradientStop(Colors.LightBlue, 1.0);
            GradientStopCollection GSC = new GradientStopCollection();
            GSC.Add(GS1);
            GSC.Add(GS2);
            GSC.Add(GS3);
            _LGB1.GradientStops = GSC;
            _textbox1.Background = _LGB1;

            _button1 = new Button();
            _button1.Name = "Button1";
            _body.Children.Add(_button1);

            _scrollbar1 = new ScrollBar();
            _scrollbar1.Background    = Brushes.LightYellow;
            _scrollbar1.Name          = "SCROLL";
            _scrollbar1.Orientation   = Orientation.Horizontal;
            _scrollbar1.Value         = 0.2d;
            _scrollbar1.Height        = 30d;
            _scrollbar1.Width         = 150d;
            _scrollbar1.ViewportSize  = 0.2d;
            Canvas.SetTop  (_scrollbar1, 250d);
            Canvas.SetLeft (_scrollbar1, 50d);
            _body.Children.Add(_scrollbar1);

            _rectangleGeo = new RectangleGeometry();
            
            GlobalLog.LogStatus("--------Window created--------");

            return TestResult.Pass;
        }
        
        /******************************************************************************
        * Function:          OnTick
        ******************************************************************************/
        /// <summary>
        /// Invoked every time the DispatcherTimer ticks. Used to control the timing of verification.
        /// </summary>
        /// <returns></returns>
        private void OnTick(object sender, EventArgs e)          
        {
            GlobalLog.LogStatus("--------OnTick--------");
            _aTimer.Stop();
        }

        /******************************************************************************
        * Function:          StartAnimation
        ******************************************************************************/
        /// <summary>
        /// Verifies the result of the test case, and returns a Pass/Fail result.
        /// </summary>
        /// <returns>TestResult.Pass</returns>
        TestResult StartAnimation()
        {
            GlobalLog.LogStatus("--------StartAnimation--------");
            Animate();  //This will produce the AnimationException.

            if (_testPassed)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        /******************************************************************************
           * Function:          Animate
           ******************************************************************************/
        /// <summary>
        /// Animate: Call a routines that starts the animation, based on the test Variation.
        /// </summary>
        private void Animate()
        {
            switch (_inputString)
            {
                case "NegativeWidth" :
                    _expectedState       = ClockState.Active;
                    _expectedElement     = _textbox1.GetType();
                    _expectedProperty    = TextBox.WidthProperty;
                    NegativeWidth();
                    break;

                case "NoBaseHeight" :
                    _expectedState       = ClockState.Active;
                    _expectedElement     = _button1.GetType();
                    _expectedProperty    = Button.HeightProperty;
                    NoBaseHeight();
                    break;

                case "NegativeViewportSize" :
                    _expectedState       = ClockState.Active;
                    _expectedElement     = _scrollbar1.GetType();
                    _expectedProperty    = ScrollBar.ViewportSizeProperty;
                    NegativeViewportSize();
                    break;

                case "NoRectProperty" :
                    _expectedState       = ClockState.Active;
                    _expectedElement     = _rectangleGeo.GetType();
                    _expectedProperty    = RectangleGeometry.RectProperty;
                    NoRectProperty();
                    break;

                default:
                    break;
            }
        }

        /******************************************************************************
           * Function:          NegativeWidth
           ******************************************************************************/
        /// <summary>
        /// NegativeWidth: An exception is produced by animating to a negative width on a TextBox.
        /// </summary>
        /// <returns></returns>
        public void NegativeWidth()
        {
            GlobalLog.LogStatus("--------NegativeWidth--------");

            DoubleAnimation animWidth   = new DoubleAnimation();
            animWidth.By                = -401d;
            animWidth.BeginTime         = TimeSpan.FromMilliseconds(0);
            animWidth.Duration          = new Duration(TimeSpan.FromMilliseconds(1000));

            _clock1 = animWidth.CreateClock();
            try
            {
                _textbox1.ApplyAnimationClock(_expectedProperty, _clock1);

//HACK-HACK -- NEED THIS WAITFORSIGNAL, WITH NO CORRESPONDING SIGNAL!!:
                WaitForSignal("TestFinished");
            }
            catch (AnimationException exception)
            {
                GlobalLog.LogStatus("--------Exception Caught--------");
                CheckResults(exception);
            }
        }

        /******************************************************************************
           * Function:          NoBaseHeight
           ******************************************************************************/
        /// <summary>
        /// NoBaseHeight: An exception is produced by failing to specify a base value for the button
        /// before animating it.
        /// </summary>
        /// <returns></returns>
        public void NoBaseHeight()
        {
            GlobalLog.LogStatus("--------NoBaseHeight--------");
            
            DoubleAnimation animHeight  = new DoubleAnimation();
            animHeight.BeginTime           = TimeSpan.FromMilliseconds(0);
            animHeight.Duration            = new Duration(TimeSpan.FromMilliseconds(1000));
            animHeight.To                  = 5d;

            _clock1 = animHeight.CreateClock();
            try
            {
                _button1.ApplyAnimationClock(_expectedProperty, _clock1);

                WaitForSignal("TestFinished");
            }
            catch (AnimationException exception)
            {
                GlobalLog.LogStatus("--------Exception Caught--------");
                CheckResults(exception);
            }
        }

        /******************************************************************************
           * Function:          NegativeViewportSize
           ******************************************************************************/
        /// <summary>
        /// NegativeViewportSize: An exception is produced by animating to a negative ViewportSize.
        /// </summary>
        /// <returns></returns>
        public void NegativeViewportSize()
        {
            GlobalLog.LogStatus("--------NegativeViewportSize--------");
            
            DoubleAnimation animView = new DoubleAnimation();
            animView.BeginTime      = TimeSpan.FromMilliseconds(0);
            animView.Duration       = new Duration(TimeSpan.FromMilliseconds(1000));
            animView.To             = -500d;

            try
            {
                _scrollbar1.BeginAnimation(_expectedProperty, animView);

                WaitForSignal("TestFinished");
            }
            catch (AnimationException exception)
            {
                GlobalLog.LogStatus("--------Exception Caught--------");
                CheckResults(exception);
            }
        }

        /******************************************************************************
           * Function:          NoRectProperty
           ******************************************************************************/
        /// <summary>
        /// NoRectProperty: An exception is produced by failing to set the Rect property of the
        /// RectangleGeometry.
        /// </summary>
        /// <returns></returns>
        public void NoRectProperty()
        {
            GlobalLog.LogStatus("--------NoRectProperty--------");
            
            RectAnimation animRect = new RectAnimation();
            animRect.To                 = new Rect(0,0,20,15);
            animRect.BeginTime          = TimeSpan.FromMilliseconds(0);
            animRect.Duration           = new Duration(TimeSpan.FromMilliseconds(1000));

            Style triggerStyle = new Style();
            triggerStyle.TargetType = typeof(Button);

            BeginStoryboard beginStory = AnimationUtilities.CreateBeginStoryboard(Window, "beginStory");

            Trigger pTrigger = AnimationUtilities.CreatePropertyTrigger(Button.WidthProperty, 150d, beginStory);

            triggerStyle.Triggers.Add(pTrigger);

            _button1.Clip = _rectangleGeo;

            Storyboard storyboard = new Storyboard();
            storyboard.Name = "story";
            storyboard.Children.Add(animRect);
            PropertyPath path1  = new PropertyPath("(0).(1)", Button.ClipProperty, _expectedProperty);
            Storyboard.SetTargetProperty(animRect, path1);
            
            beginStory.Storyboard = storyboard;

            ResourceDictionary elementDictionary = new ResourceDictionary();
            elementDictionary.Add("StyleKey", triggerStyle);
            ((FrameworkElement)_button1).Resources = elementDictionary;

            _button1.SetResourceReference(FrameworkElement.StyleProperty, "StyleKey");
            
            try
            {
                _button1.Width = 150d;  //Trigger the Animation.

                WaitForSignal("TestFinished");
            }
            catch (AnimationException exception)
            {
                GlobalLog.LogStatus("--------Exception Caught--------");
                CheckResults(exception);
            }
        }

        /******************************************************************************
        * Function:          CheckResults
        ******************************************************************************/
        /// <summary>
        /// Compares actual and expected results.
        /// </summary>
        /// <returns>TestResult.Pass</returns>
        private void CheckResults(AnimationException animationException)
        {
            GlobalLog.LogStatus("--------CheckResults--------");


            if (animationException.InnerException == null)
            {
                _exceptionMessage = animationException.ToString();
            }
            else
            {
                _exceptionMessage = animationException.InnerException.ToString();
            }

            Clock exceptionClock                    = animationException.Clock;
            Type exceptionElement                   = animationException.Target.GetType();
            DependencyProperty exceptionProperty    = animationException.Property;

            //GlobalLog.LogEvidence("CAUGHT:"     + exceptionMessage);
            //GlobalLog.LogEvidence("CLOCK:"      + animationException.Clock);
            //GlobalLog.LogEvidence("PROPERTY:"   + animationException.Property);
            //GlobalLog.LogEvidence("TARGET:"     + animationException.Target);
            //GlobalLog.LogEvidence("TARGET:"     + animationException.Target.GetType() );

            _testPassed = (  _exceptionMessage   != ""
                  && exceptionClock.CurrentState    == _expectedState
                  && exceptionElement               == _expectedElement
                  && exceptionProperty              == _expectedProperty );


            GlobalLog.LogEvidence("-----------RESULTS-----------");
            GlobalLog.LogEvidence("--Message returned  -- Act: " + (_exceptionMessage != "").ToString() + " / Exp: True");
            GlobalLog.LogEvidence("--Element  --\n    Act: " + exceptionElement + "\n    Exp: " + _expectedElement);
            GlobalLog.LogEvidence("--State    -- Act:  " + exceptionClock.CurrentState + " / Exp: " + _expectedState);
            GlobalLog.LogEvidence("--Property -- Act:  " + exceptionProperty + " / Exp: " + _expectedProperty);
            GlobalLog.LogEvidence("------------------------");

        }
        #endregion
    }
}
