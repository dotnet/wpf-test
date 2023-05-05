// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
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
    /// <area>Animation.PropertyMethodEvent.Regressions</area>

    [Test(2, "Animation.PropertyMethodEvent.Regressions", "EnumBadTest", Keywords = "Localization_Suite")]
    public class EnumBadTest : WindowTest
    {

        #region Test case members
            private string              _inputString = null;
            private DispatcherTimer     _aTimer      = null;
        #endregion


        #region Constructor

        [Variation("Seek")]
        [Variation("SeekAligned")]
        [Variation("FillBehavior")]
        [Variation("SlipBehavior")]
        [Variation("HandoffBehavior")]
        [Variation("PathAnimationSource", Disabled=true)]

        /******************************************************************************
        * Function:          EnumBadTest Constructor
        ******************************************************************************/
        public EnumBadTest(string testValue)
        {
            _inputString = testValue;
            RunSteps += new TestStep(CreateTree);
            RunSteps += new TestStep(StartTimer);
        }

        #endregion


        #region Test Steps
        
        /******************************************************************************
        * Function:          StartTimer
        ******************************************************************************/
        /// <summary>
        /// Starts a Timer to control the timing of the verification.
        /// </summary>
        /// <returns>Returns TestResult.Pass</returns>
        private TestResult StartTimer()
        {
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = new TimeSpan(0,0,0,0,1000);
            _aTimer.Start();
            
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
            _aTimer.Stop();
            Signal("AnimationDone", TestResult.Pass);
        }
        
        /******************************************************************************
        * Function:          CreateTree
        ******************************************************************************/
        /// <summary>
        /// Creates the page content, an Animation, and a corresponding Clock.
        /// </summary>
        /// <returns>A TestResult</returns>
        private TestResult CreateTree()
        {
            Window.Width        = 600d;
            Window.Height       = 300d;
            Window.Left         = 0d;
            Window.Top          = 0d;

            Canvas body  = new Canvas();
            Window.Content = body;

            TextBlock textblock1 = new TextBlock();
            textblock1.Width        = 200d;
            textblock1.Height       = 200d;
            textblock1.Background   = Brushes.Maroon;
            textblock1.Foreground   = Brushes.LightYellow;
            textblock1.FontSize     = 48d;
            textblock1.Margin       = new Thickness(12d);
            textblock1.Text         = "Avalon!";
            body.Children.Add(textblock1);

            switch (_inputString)
            {
                case "Seek" :
                    DoubleAnimation doubleAnim1 = CreateDoubleAnimation();
                    AnimationClock clock1 = doubleAnim1.CreateClock();
                    textblock1.ApplyAnimationClock(TextBlock.WidthProperty, clock1);
                    TimeSeekOrigin timeSeekOrigin1 = (TimeSeekOrigin)Enum.ToObject(typeof(TimeSeekOrigin), 2);

                    SetExpectedErrorTypeInStep(typeof(InvalidEnumArgumentException), "Outer");
                    clock1.Controller.Seek(TimeSpan.FromSeconds(0), timeSeekOrigin1);
                    break;

                case "SeekAligned" :
                    DoubleAnimation doubleAnim2 = CreateDoubleAnimation();
                    AnimationClock clock2 = doubleAnim2.CreateClock();
                    textblock1.ApplyAnimationClock(TextBlock.WidthProperty, clock2);
                    TimeSeekOrigin timeSeekOrigin2 = (TimeSeekOrigin)Enum.ToObject(typeof(TimeSeekOrigin), 2);

                    SetExpectedErrorTypeInStep(typeof(InvalidEnumArgumentException), "Outer");                
                    clock2.Controller.SeekAlignedToLastTick(TimeSpan.FromSeconds(0), timeSeekOrigin2);
                    break;

                case "FillBehavior" :
                    DoubleAnimation doubleAnim3 = CreateDoubleAnimation();
                    FillBehavior fillBehavior = (FillBehavior)Enum.ToObject(typeof(FillBehavior), 2);

                    SetExpectedErrorTypeInStep(typeof(ArgumentException), "Outer");
                    doubleAnim3.FillBehavior = fillBehavior;
                    break;

                case "SlipBehavior" :
                    DoubleAnimation doubleAnim4 = CreateDoubleAnimation();
                    SlipBehavior slipBehavior = (SlipBehavior)Enum.ToObject(typeof(SlipBehavior), 3);
                    Storyboard storyboard = new Storyboard();
                    storyboard.Name         = "story";

                    SetExpectedErrorTypeInStep(typeof(ArgumentException), "Outer");
                    storyboard.SlipBehavior = slipBehavior;
                    break;

                case "HandoffBehavior" :
                    DoubleAnimation doubleAnim5 = CreateDoubleAnimation();
                    HandoffBehavior handoffBehavior = (HandoffBehavior)Enum.ToObject(typeof(HandoffBehavior), 2);
                    SetExpectedErrorTypeInStep(typeof(ArgumentException), "Outer");                
                    AnimationClock clock3 = doubleAnim5.CreateClock();
                    textblock1.ApplyAnimationClock(TextBlock.WidthProperty, clock3, handoffBehavior);
                    break;

                case "PathAnimationSource" :
                    
                    DoubleAnimationUsingPath doubleAnimPath = CreateDoubleAnimationUsingPath();
                    PathAnimationSource source = (PathAnimationSource)Enum.ToObject(typeof(PathAnimationSource), 99);
                    doubleAnimPath.Source = source;

                    PathFigureCollection PFC = SpecialObjects.CreatePathFigureCollection();
                    PathGeometry pathGeometry   = new PathGeometry();
                    pathGeometry.Figures        = PFC;
                    doubleAnimPath.PathGeometry = pathGeometry;

                    SetExpectedErrorTypeInStep(typeof(InvalidEnumArgumentException), "Outer");                
                    AnimationClock clock4 = doubleAnimPath.CreateClock();
                    textblock1.ApplyAnimationClock(Canvas.LeftProperty, clock4);
                    break;
            }

            WaitForSignal("AnimationDone");
                       
            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          CreateDoubleAnimation
        ******************************************************************************/
        /// <summary>
        /// CreateDoubleAnimation: create a double animation.
        /// </summary>
        /// <returns>TestResult.Pass</returns>
        private DoubleAnimation CreateDoubleAnimation()
        {
            DoubleAnimation animDouble = new DoubleAnimation();
            animDouble.From             = 0d;
            animDouble.To               = 500d;
            animDouble.BeginTime        = TimeSpan.FromMilliseconds(0);
            animDouble.Duration         = new Duration(TimeSpan.FromMilliseconds(50));

            return animDouble;
        }

        /******************************************************************************
        * Function:          CreateDoubleAnimationUsingPath
        ******************************************************************************/
        /// <summary>
        /// CreateDoubleAnimationUsingPath: create a DoubleAnimationUsingPath.
        /// </summary>
        /// <returns>TestResult.Pass</returns>
        private DoubleAnimationUsingPath CreateDoubleAnimationUsingPath()
        {
            DoubleAnimationUsingPath animDUP = new DoubleAnimationUsingPath();
            animDUP.BeginTime                = TimeSpan.FromMilliseconds(0);
            animDUP.Duration                 = new Duration(TimeSpan.FromMilliseconds(50));
            animDUP.RepeatBehavior           = new RepeatBehavior(2d);
            animDUP.IsAdditive               = true;
            animDUP.IsCumulative             = true;
            
            return animDUP;
        }

        #endregion
    }
}
