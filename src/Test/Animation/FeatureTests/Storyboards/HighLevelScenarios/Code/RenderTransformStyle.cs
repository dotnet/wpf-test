// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Storyboards.HighLevelScenarios.Regressions</area>


    [Test(2, "Storyboards.HighLevelScenarios.Regressions", "RenderTransformStyleTest")]
    public class RenderTransformStyleTest : WindowTest
    {
        #region Test case members

        private Button                          _button1;
        private TranslateTransform              _translateTransform;
        private DoubleAnimationUsingKeyFrames   _animKeyFrame;
        private double                          _fromValue       = 50d;
        private double                          _toValue         = 80d;
        private DispatcherTimer                 _aTimer          = null;
        private int                             _tickCount       = 0;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          RenderTransformStyleTest Constructor
        ******************************************************************************/
        public RenderTransformStyleTest()
        {
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
        /// Creates the window content.
        /// </summary>
        /// <returns>Returns success if the element was found</returns>
        TestResult CreateTree()
        {
            Window.Width        = 300d;
            Window.Height       = 300d;
            Window.Left         = 0d;
            Window.Top          = 0d;

            Canvas body = new Canvas();
            body.Background = Brushes.MidnightBlue;
            body.Height     = 300d;
            body.Width      = 300d;

            _button1 = new Button();
            body.Children.Add(_button1);
            _button1.Background        = Brushes.LemonChiffon;
            _button1.Height            = 20d;
            _button1.Width             = 20d;
            _button1.Name              = "TheButton";
            _button1.Content           = "Hello";
            
            _translateTransform = new TranslateTransform();
            _translateTransform.X = 0d;
            _translateTransform.Y = 0d;
            _button1.RenderTransform = _translateTransform;
            
            Window.Content = body;
            
            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          RebuildStyle
        ******************************************************************************/
        /// <summary>
        /// Creates the window content.
        /// </summary>
        /// <returns></returns>
        private void RebuildStyle()
        {
            _animKeyFrame = new DoubleAnimationUsingKeyFrames();
            DoubleKeyFrameCollection DKFC = new DoubleKeyFrameCollection();
            DKFC.Add(new DiscreteDoubleKeyFrame(_fromValue, KeyTime.FromTimeSpan(new TimeSpan(0,0,0,0,0))));
            DKFC.Add(new DiscreteDoubleKeyFrame(_toValue, KeyTime.FromTimeSpan(new TimeSpan(0,0,0,0,1000))));
            _animKeyFrame.KeyFrames = DKFC;

            _animKeyFrame.BeginTime      = TimeSpan.FromMilliseconds(0);
            _animKeyFrame.Duration       = new Duration(TimeSpan.FromMilliseconds(1000));
            _animKeyFrame.IsAdditive     = true;
            
            Storyboard storyboard = new Storyboard();
            storyboard.Name = "story";
            storyboard.Children.Add(_animKeyFrame);
            PropertyPath path1 = new PropertyPath("(0).(1)", new DependencyProperty[] { UIElement.RenderTransformProperty, TranslateTransform.XProperty });
            Storyboard.SetTargetProperty(storyboard, path1);
        
            Style style = new Style();
            style.TargetType = typeof(Button);
            style.Resources.Add("StoryKey", storyboard);

            _button1.Style = style;
            storyboard.Begin(_button1);
        }
        
        /******************************************************************************
        * Function:          StartTimer
        ******************************************************************************/
        /// <summary>
        /// Fires when the page loads and the page content is finished rendering.
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        private TestResult StartTimer()
        {
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,500);
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
                RebuildStyle();
            }
            else if (_tickCount == 4)
            {
                RebuildStyle();
            }
            else if (_tickCount == 8)
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

            double actValue = (double)_translateTransform.GetValue(TranslateTransform.XProperty);
            double expValue = _toValue * 2;
            
            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected Value: " + expValue);
            GlobalLog.LogEvidence("Actual Value:   " + actValue);
            
            if (actValue == expValue)
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
