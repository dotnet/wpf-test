// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: DRT test suite for transform methods on the Visual class
//

using System;
using System.Collections;

using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace DRT
{
    public sealed class TestVisualTransforms : DrtTestSuite
    {
        public TestVisualTransforms () : base("VisualTransforms")
        {
            Contact = "Microsoft";
        }

        #region Setup

        public override DrtTest[] PrepareTests()
        {
            // load the markup
            DRT.LoadXamlFile(@"DrtFiles\PropertyEngine\TestVisualTransforms.xaml");

            return new DrtTest[]
            {
                new DrtTest(VerifyPointToScreen),
            };
        }

        #endregion Setup

        #region Tests for PointToScreen / PointFromScreen

        // A bit of roundoff error is expected during the transforms.  Deal with it.
        bool CloseEnough(Point p1, Point p2)
        {
            return Math.Abs(p1.X - p2.X) < 1 && Math.Abs(p1.Y - p2.Y) < 1;
        }

        // PointToScreen and PointFromScreen should be inverses
        void VerifyPointToScreen(FrameworkElement visual)
        {
            for (int i=0; i<VisualPoints.Length; ++i)
            {
                Point screenPoint = visual.PointToScreen(VisualPoints[i]);
                Point visualPoint = visual.PointFromScreen(screenPoint);
                DRT.Assert(CloseEnough(VisualPoints[i], visualPoint),
                    "PointToScreen failed at point {0} for {1}  Expected: {2}  Got: {3}",
                    i, visual.Name, VisualPoints[i], visualPoint);
            }

            for (int i=0; i<ScreenPoints.Length; ++i)
            {
                Point visualPoint = visual.PointFromScreen(ScreenPoints[i]);
                Point screenPoint = visual.PointToScreen(visualPoint);
                DRT.Assert(CloseEnough(ScreenPoints[i], screenPoint),
                    "ScreenToPoint failed at point {0} for {1}  Expected: {2}  Got: {3}",
                    i, visual.Name, ScreenPoints[i], screenPoint);
            }
        }

        // Test PointToScreen on various visual elements
        void VerifyPointToScreen()
        {
            FrameworkElement visual;

            visual = (FrameworkElement)DRT.FindVisualByID("Button1");
            VerifyPointToScreen(visual);

            visual = (FrameworkElement)DRT.FindVisualByID("Button2");
            VerifyPointToScreen(visual);

            visual = (FrameworkElement)DRT.FindVisualByID("Button3");
            VerifyPointToScreen(visual);

            visual = (FrameworkElement)DRT.FindVisualByID("Label1");
            VerifyPointToScreen(visual);
        }

        #endregion Tests for PointToScreen / ScreenToPoint

        #region Fields

        static Point[] VisualPoints = new Point[]{
                    new Point(0,0),
                    new Point(100,0),
                    new Point(0,200),
                    new Point(50,50),
                    new Point(-30,80),
                    new Point(70,-120),
                    new Point(-200,-20),
        };


        static Point[] ScreenPoints = new Point[]{
                    new Point(0,0),
                    new Point(100,0),
                    new Point(0,200),
                    new Point(50,50),
                    new Point(-30,80),
                    new Point(70,-120),
                    new Point(-200,-20),
        };

        #endregion Fields
    }
}

