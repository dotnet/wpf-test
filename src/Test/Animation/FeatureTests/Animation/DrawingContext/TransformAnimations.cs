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
using System.Windows.Input;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// <area>Animation.DrawingContext.Regressions</area>
    /// <priority>0</priority>

    [Test(0, "Animation.DrawingContext.Regressions", "TransformAnimations")]
    public class TransformAnimationsTest : WindowTest
    {
        #region Test case members

        private Canvas              _body;
        private DispatcherTimer     _aTimer              = null;
        private int                 _tickCount           = 0;
        
        #endregion


        #region Constructor
        
        /******************************************************************************
        * Function:          TransformAnimationsTest Constructor
        ******************************************************************************/
        public TransformAnimationsTest()
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
            _body = new Canvas();
            _body.Background = Brushes.LightGray;
            _body.Focusable = true;
            _body.Children.Add( new Sprite( new Point( 400, 150 ) ) );

            Window.Content = _body;
            Window.Width = 1200;
            Window.Height = 300;
            Window.Title = "Animation Regression Test";
            Window.ContentRendered     += new EventHandler(OnContentRendered);
            
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
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,500);
            _aTimer.Start();
            GlobalLog.LogStatus("----DispatcherTimer Started----");

            Sprite.MoveLeft();  //Start the Animation.
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
            GlobalLog.LogStatus("-----Tick #" + _tickCount);
            
            if (_tickCount == 4)
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

            double actValue = (double)Sprite.position.GetValue(TranslateTransform.XProperty);
            double expValue = 148d;
            double tolerance = 5d;
            
            GlobalLog.LogEvidence("-----Verifying the Animation-----");
            GlobalLog.LogEvidence("Expected Value: " + expValue);
            GlobalLog.LogEvidence("Actual Value:   " + actValue);
            
            if (actValue <= expValue+tolerance && actValue >= expValue-tolerance)
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
    public class Sprite : FrameworkElement
    {
        private static Point                    s_center;
        private static double                   s_radius;
        private static RotateTransform          s_eyeTransform;
        public  static TranslateTransform       position;
        private static TimeSpan                 s_spriteRotateTime;

        public Sprite( Point c )
        {
            s_center = c;
            s_radius = 40;
            s_spriteRotateTime = TimeSpan.FromMilliseconds( 500 );
        }

        public static void MoveLeft()
        {
            // ---------------------------------------------------
            //   Set up the animations to roll to the left
            // ---------------------------------------------------

            double angle = s_eyeTransform.CloneCurrentValue().Angle;

            DoubleAnimation angleAnimation = new DoubleAnimation( angle+360, angle, s_spriteRotateTime );
            //angleAnimation.RepeatBehavior = RepeatBehavior.Forever;
            angleAnimation.BeginTime = null;
                
            AnimationClock clock1 = angleAnimation.CreateClock();
            s_eyeTransform.ApplyAnimationClock(RotateTransform.AngleProperty, clock1);
            clock1.Controller.Begin();

            double x = position.CloneCurrentValue().X;

            DoubleAnimation xAnimation = new DoubleAnimation( x, x-2*s_radius*Math.PI, s_spriteRotateTime );
            xAnimation.BeginTime = null;
                
            AnimationClock clock2 = xAnimation.CreateClock();
            position.ApplyAnimationClock(TranslateTransform.XProperty, clock2);
            clock2.Controller.Begin();
        }

        protected override void OnRender( DrawingContext context )
        {
            // Move to a 0,0 coordinate system.
            // We will start drawing from there.
            GlobalLog.LogStatus("----OnRender----");

            position = new TranslateTransform( s_center.X, s_center.Y );

            context.PushTransform( position );
            DrawBody( context );
            context.Pop();
        }

        private void DrawBody( DrawingContext context )
        {
            GlobalLog.LogStatus("----DrawBody----");

            context.DrawEllipse(
                        Brushes.White,
                        new Pen( Brushes.Black, 3.0 ),
                        new Point( 0,0 ),
                        null,
                        s_radius,
                        null,
                        s_radius,
                        null
                        );
            DrawEyes( context );
        }

        private void DrawEyes( DrawingContext context )
        {
            s_eyeTransform = new RotateTransform( 0 );

            context.PushTransform( s_eyeTransform );
                context.PushTransform( new TranslateTransform( -5, 0 ) );
                DrawEye( context );
                context.Pop();

                context.PushTransform( new TranslateTransform( 5, 0 ) );
                DrawEye( context );
                context.Pop();
            context.Pop();
        }

        private void DrawEye( DrawingContext context )
        {
            context.DrawEllipse(
                    Brushes.Black,
                    null,
                    new Point( s_radius * .5, -s_radius * .40 ),
                    s_radius * .05,
                    s_radius * .10
                    );
        }
    }
}
