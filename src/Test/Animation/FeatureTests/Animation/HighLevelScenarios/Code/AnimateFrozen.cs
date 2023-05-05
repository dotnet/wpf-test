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
    /// <priority>2</priority>
    /// <description>
    /// Regression Test: "Animation should throw a meaningful exception when animated a frozen object"
    /// </description>
    /// </summary>
    [Test(2, "Animation.HighLevelScenarios.Regressions", "AnimateFrozenTest")]
    public class AnimateFrozenTest : WindowTest
    {

        #region Test case members
        
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          AnimateFrozenTest Constructor
        ******************************************************************************/
        public AnimateFrozenTest()
        {
            RunSteps += new TestStep(CreateTree);
        }

        #endregion


        #region Test Steps
        
        /******************************************************************************
        * Function:          CreateTree
        ******************************************************************************/
        /// <summary>
        /// Creates the page content, an Animation, and a corresponding Clock.
        /// </summary>
        /// <returns>A TestResult</returns>
        private TestResult CreateTree()
        {
            Window.Width        = 350d;
            Window.Height       = 600d;
            Window.Left         = 0d;
            Window.Top          = 0d;

            Canvas body  = new Canvas();
            Window.Content = body;

            LineGeometry lineGeometry = new LineGeometry();
            lineGeometry.StartPoint     = new Point(60,60);
            lineGeometry.EndPoint       = new Point(200,200);
            lineGeometry.Freeze();

            Path path = new Path();
            path.Data               = lineGeometry;
            path.StrokeThickness    = 2;
            path.Stroke             = Brushes.Black;
            body.Children.Add(path);
            
            PointAnimation PA = new PointAnimation();
            PA.From             = new Point(200,200);
            PA.To               = new Point(120,120);
            PA.BeginTime        = null;
            PA.Duration         = new Duration(TimeSpan.FromMilliseconds(2000));
            PA.FillBehavior     = FillBehavior.HoldEnd;

            AnimationClock clock1 = PA.CreateClock();

            SetExpectedErrorTypeInStep(typeof(InvalidOperationException), "Outer");                
            lineGeometry.ApplyAnimationClock(LineGeometry.EndPointProperty, clock1);
                       
            return TestResult.Pass;
        }

        #endregion
    }
}
