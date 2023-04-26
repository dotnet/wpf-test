// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.HighLevelScenarios.Regressions</area>



    [Test(2, "Animation.HighLevelScenarios.Regressions", "SetBaseValueWhenAnimatingTest")]
    public class SetBaseValueWhenAnimatingTest : WindowTest
    {
        #region Test case members

        private string              _inputString     = "";
        private Ellipse             _ellipse1        = null;
        private SolidColorBrush     _SCB             = null;
        private Color               _baseColor       = Colors.Azure;
        private Color               _toColor         = Colors.RoyalBlue;
        private Thickness           _baseThickness   = new Thickness(0d);
        private Thickness           _toThickness     = new Thickness(5d);
        private int                 _tickCount       = 0;
        private DispatcherTimer     _aTimer          = null;
        
        #endregion


        #region Constructor
        // [DISABLE WHILE PORTING]
        // [Variation("Animatable")]
        [Variation("FE")]
        
        /******************************************************************************
        * Function:          SetBaseValueWhenAnimatingTest Constructor
        ******************************************************************************/
        public SetBaseValueWhenAnimatingTest(string testValue)
        {
            _inputString = testValue;
            InitializeSteps += new TestStep(CreateTree);
            RunSteps += new TestStep(StartTimer);
            RunSteps += new TestStep(Verify);
        }

        #endregion


        #region Test Steps

        /******************************************************************************
        * Function:          CreateTree
        ******************************************************************************/
        /// <summary>
        /// Creates the window content and starts a DispatcherTimer used for controlling 
        /// verification of the Animation.
        /// </summary>
        /// <returns>Returns success</returns>
        TestResult CreateTree()
        {
            Window.Width        = 300d;
            Window.Height       = 400d;

            Canvas body  = new Canvas();
            body.Background = Brushes.DarkViolet;

            _ellipse1 = new Ellipse();
            body.Children.Add(_ellipse1);
            _ellipse1.SetValue(Canvas.LeftProperty, 80d);
            _ellipse1.SetValue(Canvas.TopProperty, 80d);
            _ellipse1.Height            = 150d;
            _ellipse1.Width             = 150d;
            _ellipse1.Name              = "TheEllipse";
            _ellipse1.StrokeThickness   = 2d;
            _ellipse1.Stroke            = Brushes.Teal;
            _ellipse1.Margin            = _baseThickness;

            _SCB = new SolidColorBrush();
            _SCB.Color = _baseColor;
            _ellipse1.Fill = _SCB;

            Window.Content = body;
            
            Storyboard story = new Storyboard();
            story.Name = "story";
            PropertyPath path = null;

            if (_inputString == "Animatable")
            {
                ColorAnimationUsingKeyFrames animColor = new ColorAnimationUsingKeyFrames();
                ColorKeyFrameCollection CKFC = new ColorKeyFrameCollection();
                CKFC.Add(new LinearColorKeyFrame(_baseColor, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
                CKFC.Add(new LinearColorKeyFrame(_toColor,   KeyTime.FromTimeSpan(TimeSpan.FromSeconds(2))));
                animColor.KeyFrames = CKFC;

                story.Children.Add(animColor);
                path = new PropertyPath("(0).(1)", new DependencyProperty[] { Shape.FillProperty, SolidColorBrush.ColorProperty });
            }
            else if (_inputString == "FE")
            {
                ThicknessAnimationUsingKeyFrames animThickness = new ThicknessAnimationUsingKeyFrames();
                ThicknessKeyFrameCollection TKFC = new ThicknessKeyFrameCollection();
                TKFC.Add(new LinearThicknessKeyFrame(_baseThickness, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
                TKFC.Add(new LinearThicknessKeyFrame(_toThickness,   KeyTime.FromTimeSpan(TimeSpan.FromSeconds(2))));
                animThickness.KeyFrames = TKFC;

                story.Children.Add(animThickness);
                path = new PropertyPath("(0)", new DependencyProperty[] { Ellipse.MarginProperty });
            }

            Storyboard.SetTargetProperty(story, path);
            story.Begin(_ellipse1);

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          StartTimer
        ******************************************************************************/
        /// <summary>
        /// Starts the DispatcherTimer.
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        private TestResult StartTimer()
        {
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,1000);
            _aTimer.Start();

            GlobalLog.LogStatus("----DispatcherTimer Started----");

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
            _tickCount++;

            if (_tickCount == 1)
            {
                if (_inputString == "Animatable")
                {
                    _SCB.Color = Colors.DeepPink;
                    _ellipse1.Fill = _SCB;
                }
                else if (_inputString == "FE")
                {
                    _ellipse1.Margin = new Thickness(25d);
                }
            }
            else if (_tickCount == 5)
            {
                Signal("AnimationDone", TestResult.Pass);
                _aTimer.Stop();
            }
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
            WaitForSignal("AnimationDone");

            bool testPassed = false;

            GlobalLog.LogStatus("----Verifying the Animation----");

            if (_inputString == "Animatable")
            {
                Color actColor = Color.FromScRgb(_SCB.Color.ScA, _SCB.Color.ScR, _SCB.Color.ScG, _SCB.Color.ScB);
                Color expColor = _toColor;
                testPassed = (actColor == expColor);

                GlobalLog.LogEvidence("Expected Value: " + expColor);
                GlobalLog.LogEvidence("Actual Value:   " + actColor);
            }
            else if (_inputString == "FE")
            {
                Thickness actThickness = (Thickness)_ellipse1.GetValue(Ellipse.MarginProperty);
                Thickness expThickness = _toThickness;
                testPassed = (actThickness == expThickness);

                GlobalLog.LogEvidence("Expected Value: " + expThickness);
                GlobalLog.LogEvidence("Actual Value:   " + actThickness);
            }
            
            if ( testPassed ) 
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
}
