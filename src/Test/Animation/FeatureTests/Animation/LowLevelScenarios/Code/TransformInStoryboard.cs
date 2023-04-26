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
using System.Windows.Shapes;
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
    /// <area>Storyboards.LowLevelScenarios.Regressions</area>


    [Test(2, "Storyboards.LowLevelScenarios.Regressions", "TransformInStoryboardTest")]
    public class TransformInStoryboardTest : WindowTest
    {
        #region Test case members

        private double              _baseValue               = 45d;
        private double              _fromValue               = 5d;
        private double              _toValue                 = 600d;
        private Rectangle           _rectangle;
        private RotateTransform     _animatedTransform;
        private Storyboard          _storyboard;
        private DispatcherTimer     _aTimer                  = null;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          TransformInStoryboardTest Constructor
        ******************************************************************************/
        public TransformInStoryboardTest()
        {
            InitializeSteps += new TestStep(CreateTree);
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
            Window.ContentRendered     += new EventHandler(OnContentRendered);

            Canvas body = new Canvas();
            body.Background = Brushes.SlateBlue;
            body.Height     = 300d;
            body.Width      = 300d;

            _animatedTransform = new RotateTransform();
            _animatedTransform.Angle = _baseValue;

            _rectangle = new Rectangle();
            body.Children.Add(_rectangle);
            _rectangle.SetValue(Canvas.LeftProperty, 10d);
            _rectangle.SetValue(Canvas.TopProperty, 10d);
            _rectangle.Fill              = Brushes.DeepSkyBlue;
            _rectangle.Height            = 50d;
            _rectangle.Width             = 50d;
            _rectangle.Name              = "TheRectangle";
            _rectangle.StrokeThickness   = 1d;
            _rectangle.Stroke            = Brushes.Teal;
            _rectangle.RenderTransform   = _animatedTransform;
            
            DoubleAnimationUsingKeyFrames animKeyFrame = new DoubleAnimationUsingKeyFrames();
            DoubleKeyFrameCollection DKFC = new DoubleKeyFrameCollection();
            DKFC.Add(new DiscreteDoubleKeyFrame(_fromValue, KeyTime.FromTimeSpan(new TimeSpan(0,0,0,0,0))));
            DKFC.Add(new DiscreteDoubleKeyFrame(_toValue, KeyTime.FromTimeSpan(new TimeSpan(0,0,0,0,2000))));
            animKeyFrame.KeyFrames = DKFC;

            animKeyFrame.BeginTime      = TimeSpan.FromMilliseconds(0);
            animKeyFrame.Duration       = new Duration(TimeSpan.FromMilliseconds(2000));
            
            _storyboard = new Storyboard();
            _storyboard.Name = "story";
            _storyboard.Children.Add(animKeyFrame);
            PropertyPath path1 = new PropertyPath("(0).(1)", new DependencyProperty[] { UIElement.RenderTransformProperty, RotateTransform.AngleProperty });
            Storyboard.SetTargetProperty(_storyboard, path1);

            Window.Content = body;
            
            return TestResult.Pass;
        }
          
        /******************************************************************************
        * Function:          OnContentRendered
        ******************************************************************************/
        /// <summary>
        /// Fires when the page loads and the page content is finished rendering.
        /// </summary>
        /// <returns></returns>
        private void OnContentRendered(object sender, EventArgs e)
        {
            _storyboard.Begin(_rectangle);
            
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,3000);
            _aTimer.Start();
            GlobalLog.LogStatus("----DispatcherTimer Started----");
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
            Signal("AnimationDone", TestResult.Pass);
            _aTimer.Stop();
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

            double actValue = (double)_animatedTransform.GetValue(RotateTransform.AngleProperty);
            
            GlobalLog.LogEvidence("----Verifying the Animation----");
            GlobalLog.LogEvidence("Expected Value: " + _toValue);
            GlobalLog.LogEvidence("Actual Value:   " + actValue);
            
            if (actValue == _toValue)
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
