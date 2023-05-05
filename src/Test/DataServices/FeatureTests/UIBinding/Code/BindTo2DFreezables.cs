// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.Collections;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using System.Windows.Shapes;
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// This tests binding to several properties of 2D freezables.
	/// </description>
	/// <relatedBugs>




	/// </relatedBugs>
	/// </summary>
    [Test(1, "Binding", TestCaseSecurityLevel.FullTrust,"BindTo2DFreezables")]
    public class BindTo2DFreezables : XamlTest
    {

        public BindTo2DFreezables()
            : base(@"BindTo2DFreezables.xaml")
        {
            RunSteps += new TestStep(BindColorOfSolidColorBrush);
            RunSteps += new TestStep(BindVisualOfVisualBrush);
            RunSteps += new TestStep(BindColorOfGradientStop);
            RunSteps += new TestStep(BindEndPointOfLineGeometry);
            //RunSteps += new TestStep(BindShadowDepthOfDropShadowBitmapEffect);
            RunSteps += new TestStep(BindOffsetOfGradientStop);
            RunSteps += new TestStep(BindRadiusYOfRadialGradientBrush);
            RunSteps += new TestStep(BindAngleOfRotateTransform);
            RunSteps += new TestStep(SourceIsDataContext);
            RunSteps += new TestStep(SourceInBinding);
            RunSteps += new TestStep(BindToSelf);
            RunSteps += new TestStep(BindScaleYOfRotateTransform);
            RunSteps += new TestStep(BindToOfDoubleAnimation);
        }

        TestResult BindColorOfSolidColorBrush()
        {
            Status("BindColorOfSolidColorBrush");
            Rectangle rect1 = (Rectangle)(LogicalTreeHelper.FindLogicalNode(RootElement, "rect1"));
            SolidColorBrush brush = (SolidColorBrush)(rect1.Fill);
            Color color = brush.Color;
            if (color != Colors.Red)
            {
                LogComment("Fail - The SolidColorBrush's Color should be Red, instead it is " + color.ToString());
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        TestResult BindVisualOfVisualBrush()
        {
            Status("BindVisualOfVisualBrush");
            Rectangle rect2 = (Rectangle)(LogicalTreeHelper.FindLogicalNode(RootElement, "rect2"));
            VisualBrush brush = (VisualBrush)(rect2.Fill);
            Ellipse actualVisual = (Ellipse)(brush.Visual);
            Ellipse expectedVisual = (Ellipse)(LogicalTreeHelper.FindLogicalNode(RootElement, "ellipse2"));
            if (actualVisual != expectedVisual)
            {
                LogComment("Fail - The expected visual is an Ellipse. Actual: " + actualVisual.ToString());
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        TestResult BindColorOfGradientStop()
        {
            Status("BindColorOfGradientStop");
            Rectangle rect3 = (Rectangle)(LogicalTreeHelper.FindLogicalNode(RootElement, "rect3"));
            LinearGradientBrush brush = (LinearGradientBrush)(rect3.Fill);
            GradientStopCollection gradientCollection = brush.GradientStops;
            GradientStop gradient = gradientCollection[0];
            Color actualColor = gradient.Color;
            if (actualColor != Colors.Red)
            {
                LogComment("Fail - The GradientStop's Color should be Red, instead it is " + actualColor.ToString());
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        TestResult BindEndPointOfLineGeometry()
        {
            Status("BindEndPointOfLineGeometry");
            Path path4 = (Path)(LogicalTreeHelper.FindLogicalNode(RootElement, "path4"));
            LineGeometry lineGeometry = (LineGeometry)(path4.Data);
            Point endPoint = lineGeometry.EndPoint;
            Point expectedPoint = new Point(50, 50);
            if (endPoint != expectedPoint)
            {
                LogComment("Fail - Expected end Point:" + expectedPoint.ToString() + ". Actual end point: " + endPoint.ToString());
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        TestResult BindShadowDepthOfDropShadowBitmapEffect()
        {
            Status("BindShadowDepthOfDropShadowBitmapEffect");
            Image image5 = (Image)(LogicalTreeHelper.FindLogicalNode(RootElement, "image5"));
#pragma warning disable 0618
            DropShadowBitmapEffect effect = (DropShadowBitmapEffect)(image5.BitmapEffect);
            double shadowDepth = effect.ShadowDepth;
#pragma warning restore 0618
            if (shadowDepth != 10)
            {
                LogComment("Fail - Expected shadow depth is 10, actual is " + shadowDepth);
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        TestResult BindOffsetOfGradientStop()
        {
            Status("BindOffsetOfGradientStop");
            Rectangle rect6 = (Rectangle)(LogicalTreeHelper.FindLogicalNode(RootElement, "rect6"));
            RadialGradientBrush brush = (RadialGradientBrush)(rect6.Fill);
            GradientStop gradient = brush.GradientStops[2];
            double offset = gradient.Offset;
            if (offset != 0.75)
            {
                LogComment("Fail - Expected offset to be 0.75, instead it is " + offset);
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        TestResult BindRadiusYOfRadialGradientBrush()
        {
            Status("BindRadiusYOfRadialGradientBrush");
            Rectangle rect7 = (Rectangle)(LogicalTreeHelper.FindLogicalNode(RootElement, "rect7"));
            RadialGradientBrush brush = (RadialGradientBrush)(rect7.Fill);
            double radiusY = brush.RadiusY;
            if (radiusY != 0.2)
            {
                LogComment("Fail - Expected radiusY to be 0.2, instead it is " + radiusY);
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        TestResult BindAngleOfRotateTransform()
        {
            Status("BindAngleOfRotateTransform");
            Rectangle rect8 = (Rectangle)(LogicalTreeHelper.FindLogicalNode(RootElement, "rect8"));
            LinearGradientBrush brush = (LinearGradientBrush)(rect8.Fill);
            RotateTransform transform = (RotateTransform)(brush.RelativeTransform);
            if (transform.Angle != 45)
            {
                LogComment("Fail - Expected RotateTransform's Angle to be 45, instead it is " + transform.Angle);
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        TestResult SourceIsDataContext()
        {
            Status("SourceIsDataContext");
            WaitForPriority(DispatcherPriority.SystemIdle);
            Rectangle rect9 = (Rectangle)(LogicalTreeHelper.FindLogicalNode(RootElement, "rect9"));
            DrawingBrush drawingBrush = (DrawingBrush)(rect9.Fill);
            DrawingGroup drawingGroup = (DrawingGroup)drawingBrush.Drawing;
            DrawingCollection drawingCollection = drawingGroup.Children;
            GeometryDrawing geometryDrawing = (GeometryDrawing)(drawingCollection[0]);
            SolidColorBrush brush = (SolidColorBrush)(geometryDrawing.Brush);
            if (brush.Color != Colors.Green)
            {
                LogComment("Fail - Expected Brush of Geometry drawing to be of Green color, instead it is " + brush.Color);
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        TestResult SourceInBinding()
        {
            Status("SourceInBinding");
            TextBlock tb10 = (TextBlock)(LogicalTreeHelper.FindLogicalNode(RootElement, "tb10"));
            TextDecoration textDecoration = tb10.TextDecorations[0];
            TextDecorationLocation location = textDecoration.Location;
            if (location != TextDecorationLocation.Underline)
            {
                LogComment("Fail - Expected text decoration's location to be Underline, but instead it is " + location);
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        TestResult BindToSelf()
        {
            Status("BindToSelf");
            Rectangle rect11 = (Rectangle)(LogicalTreeHelper.FindLogicalNode(RootElement, "rect11"));
            DrawingBrush brush = (DrawingBrush)(rect11.Fill);
            GeometryDrawing geometryDrawing = (GeometryDrawing)(brush.Drawing);
            Pen pen = geometryDrawing.Pen;
            if (pen.EndLineCap != PenLineCap.Triangle)
            {
                LogComment("Fail - Expected pen's EndLineCap to be triangle, instead it is " + pen.EndLineCap);
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        TestResult BindScaleYOfRotateTransform()
        {
            Status("BindScaleYOfRotateTransform");
            Rectangle rect12 = (Rectangle)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "rect12"));
            ScaleTransform transform = (ScaleTransform)(rect12.LayoutTransform);
            if (transform.ScaleY != 0.8)
            {
                LogComment("Fail - Expected ScaleTransform's Angle: 0.8. Actual: " + transform.ScaleY);
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        TestResult BindToOfDoubleAnimation()
        {
            Status("BindToOfDoubleAnimation");
            Button btn13 = (Button)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "btn13"));
            btn13.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            EventTrigger trigger = (EventTrigger)(btn13.Triggers[0]);
            BeginStoryboard beginStoryboard = (BeginStoryboard)(trigger.Actions[0]);
            Storyboard storyboard = beginStoryboard.Storyboard;
            Clock clock = storyboard.CreateClock();
            clock.CurrentStateInvalidated += new EventHandler(clock_CurrentStateInvalidated);
            WaitForSignal("CurrentStateInvalidated");

            double actualWidth = btn13.Width;
            double expectedWidth = 400;

            if (actualWidth != expectedWidth)
            {
                LogComment("Fail - Actual width: " + actualWidth + ". Expected: " + expectedWidth);
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        void clock_CurrentStateInvalidated(object sender, EventArgs e)
        {
            Clock clock = sender as Clock;
            ClockState clockState = clock.CurrentState;
            Status("Clock's state inside clock_CurrentStateInvalidated: " + clockState);
            if (clockState == ClockState.Filling)
            {
                Signal("CurrentStateInvalidated", TestResult.Pass);
            }
        }
    }
}
