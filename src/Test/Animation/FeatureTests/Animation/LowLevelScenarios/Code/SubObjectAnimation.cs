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
    /// <area>Animation.LowLevelScenarios.Regressions</area>

    [Test(2, "Animation.LowLevelScenarios.Regressions", "SubObjectAnimationTest")]
    public class SubObjectAnimationTest : WindowTest
    {
        #region Test case members

        private Path                    _original;
        private TranslateTransform      _tx;
        private RectangleGeometry       _rect;
        private double                  _fromValueTrans  = 0d;
        private double                  _toValueTrans    = 200d;
        private double                  _fromValueRect   = 100d;
        private double                  _toValueRect     = 0d;
        private DispatcherTimer         _aTimer          = null;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          SubObjectAnimationTest Constructor
        ******************************************************************************/
        public SubObjectAnimationTest()
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
        /// Creates the window content and starts a DispatcherTimer used for controlling 
        /// verification of the Animation.
        /// </summary>
        /// <returns>Returns TestResult=success</returns>
        TestResult CreateTree()
        {
            Window.Width        = 400d;
            Window.Height       = 300d;

            Canvas body  = new Canvas();
            Window.Content = body;
            body.Background = Brushes.IndianRed;

            _original = new Path();

            //
            // Create a Path with two animations in it:
            //      One animates a value-type property on Path.
            //      One animates a reference-type property on Path.
            //
            _rect = new RectangleGeometry( new Rect( 0,0,200,200 ), 100, 100 );
            DoubleAnimation anim = new DoubleAnimation( _fromValueRect, _toValueRect, TimeSpan.FromSeconds( 2.0 ), FillBehavior.HoldEnd );
            anim.AutoReverse = true;

            _rect.BeginAnimation( RectangleGeometry.RadiusXProperty, anim );
            _rect.Transform = GetAnimatedTransform();
            _original.Data = _rect;
            _original.Fill = Brushes.Red;

            body.Children.Add( _original );

            Window.Content = body;

            //
            // This Path (the one on the right) should not animate.
            //
            Path copy = new Path();
            copy.Data = _original.Data.CloneCurrentValue();
            copy.Fill = Brushes.Blue;

            Canvas.SetLeft( copy, 300 );

            ((Canvas)Window.Content).Children.Add( copy );

            return TestResult.Pass;
        }

        public Transform GetAnimatedTransform()
        {
            DoubleAnimation anim = new DoubleAnimation( _fromValueTrans, _toValueTrans, TimeSpan.FromSeconds( 2.0 ), FillBehavior.HoldEnd );

            _tx = new TranslateTransform( 0, 0 );
            _tx.BeginAnimation( TranslateTransform.YProperty, anim );

            return _tx;
        }
        
        /******************************************************************************
        * Function:          StartTimer
        ******************************************************************************/
        /// <summary>
        /// Starts a Timer to control the timing of the verification.
        /// </summary>
        /// <returns>Returns TestResult=Success</returns>
        TestResult StartTimer()
        {
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,3000);
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

            double actValueTrans = (double)_tx.GetValue(TranslateTransform.YProperty);
            double actValueRect  = (double)_rect.GetValue(TranslateTransform.YProperty);
            
            GlobalLog.LogEvidence("----Verifying the Animations----");
            GlobalLog.LogEvidence("Expected Value [TranslateTransform.Y]     : " + _toValueTrans);
            GlobalLog.LogEvidence("Actual Value   [TranslateTransform.Y]     : " + actValueTrans);
            GlobalLog.LogEvidence("Expected Value [RectangleGeometry.RadiusX]: " + _toValueRect);
            GlobalLog.LogEvidence("Actual Value   [RectangleGeometry.RadiusX]: " + actValueRect);
            
            if (actValueTrans == _toValueTrans && actValueRect == _toValueRect)
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
