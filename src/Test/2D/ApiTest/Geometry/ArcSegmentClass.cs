// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  GMC API Tests - Testing ArcSegment class
//  Author:   Microsoft
//
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Summary description for ArcSegmentClass.
    /// </summary>
    internal class ArcSegmentClass : ApiTest
    {
        public ArcSegmentClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus("ArcSegment Class");

            string objectType = "System.Windows.Media.ArcSegment";

            #region Section I: Initialization stage
            CommonLib.Stage = TestStage.Initialize;

            #region Test #1 - Default constructor
            CommonLib.LogStatus("Test #1: Default Constructor");
            ArcSegment arcSegment1 = new ArcSegment();
            CommonLib.TypeVerifier(arcSegment1, objectType);
            #endregion

            #region Test #2 - Constructor with Point, Size, RotationAngle, IsLargeArc, SweepArc and IsStroked
            CommonLib.LogStatus("Test #2 - Constructor with Point, Size, RotationAngle, IsLargeArc, SweepArc and IsStroked");
            ArcSegment arcSegment2 = new ArcSegment(new Point(200, 200), new Size(50, 10), 20, false, SweepDirection.Clockwise, true);
            CommonLib.TypeVerifier(arcSegment2, objectType);
            #endregion

            #region Test #2.5 - Constructor with public ArcSegment ( System.Windows.Point point , PointAnimationCollection pointAnimations , System.Windows.Size size , SizeAnimationCollection sizeAnimations , System.Double xRotation , DoubleAnimationCollection xRotationAnimations , System.Boolean largeArc , System.Boolean sweep , System.Boolean isStroked )
            CommonLib.LogStatus("Test #2.5 - Constructor public ArcSegment ( System.Windows.Point point , PointAnimationCollection pointAnimations , System.Windows.Size size , SizeAnimationCollection sizeAnimations , System.Double xRotation , DoubleAnimationCollection xRotationAnimations , System.Boolean largeArc , System.Boolean sweep , System.Boolean isStroked )");
            ArcSegment arcSegment25 = new ArcSegment(new Point(39, 10), new Size(5, 4.0), 2.3, true, SweepDirection.Clockwise, false);
            CommonLib.TypeVerifier(arcSegment25, objectType);
            #endregion
            #endregion


            #region Section II - ArcSegment properties testing

            #region Test #6a - the IsLargeArc property with basevalue
            CommonLib.LogStatus("Test #6a - the IsLargeArc property with basevalue");
            ArcSegment as6a = new ArcSegment();
            //the basevalue of the IsLargeArc is false
            CommonLib.GenericVerifier(!as6a.IsLargeArc, "IsLargeArc property with basevalue");
            #endregion

            #region Test #6b - the IsLargeArc property in Invalid state
            CommonLib.LogStatus("Test #6b - the IsLargeArc property in Invalid state");
            ArcSegment as6b = new ArcSegment();
            as6b.InvalidateProperty(ArcSegment.IsLargeArcProperty);
            CommonLib.GenericVerifier(!as6b.IsLargeArc, "IsLargeArc property in Invalid state");
            #endregion

            #region Test #6 - the IsLargeArc property
            CommonLib.LogStatus("Test #6 - the IsLargeArc property");
            arcSegment1.IsLargeArc = true;
            CommonLib.GenericVerifier(arcSegment1.IsLargeArc, "IsLargeArc property");
            #endregion

            #region Test #6.1 - LargeArcProperty DP
            CommonLib.LogStatus("Test #6.1 - LargeArcProperty DP");
            ArcSegment as61 = new ArcSegment(new Point(1.2, 0), new Size(100, 23), -90, true, SweepDirection.Counterclockwise, false);
            as61.SetValue(ArcSegment.IsLargeArcProperty, false);
            bool return61 = (bool)as61.GetValue(ArcSegment.IsLargeArcProperty);
            CommonLib.GenericVerifier(return61 == false, "IsLargeArcProperty DP");
            #endregion

            #region Test #7a - the Point property with base value
            CommonLib.LogStatus("Test #7a - the Point property with basevalue");
            ArcSegment as7a = new ArcSegment();
            CommonLib.GenericVerifier(as7a.Point.X == 0 && as7a.Point.Y == 0, "Point property with base value");
            #endregion

            #region Test #7b - the Point property in Invalid state
            CommonLib.LogStatus("Test #7b - the Point property in Invalid state");
            ArcSegment as7b = new ArcSegment();
            as7b.InvalidateProperty(ArcSegment.PointProperty);
            CommonLib.GenericVerifier(as7b.Point.X == 0 && as7b.Point.Y == 0, "Point property in invalid state");
            #endregion

            #region Test #7 - the Point property
            CommonLib.LogStatus("Test #7 - the Point property");
            arcSegment1.Point = new Point(100, 22);
            CommonLib.PointVerifier(arcSegment1.Point, new Point(100, 22));
            #endregion

            #region Test #7.5 - the PointAnimations property
            CommonLib.LogStatus("Test #7.5 - the PointAnimations property");
            PointAnimation pointAnim1 = new PointAnimation(new Point(0, 0), new Point(20, 110), new Duration(TimeSpan.FromMilliseconds(1000)));
            PointAnimation pointAnim2 = new PointAnimation(new Point(39, 10), new Point(120, 110), new Duration(TimeSpan.FromMilliseconds(1000)));
            AnimationClock pointClock1 = pointAnim1.CreateClock();
            AnimationClock pointClock2 = pointAnim2.CreateClock();
            arcSegment25.ApplyAnimationClock(ArcSegment.PointProperty, pointClock1);
            arcSegment25.ApplyAnimationClock(ArcSegment.PointProperty, pointClock2);
            CommonLib.GenericVerifier(pointClock1 != null, "PointAnimation property 1");
            CommonLib.GenericVerifier(pointClock2 != null, "PointAnimation property 2");
            #endregion

            #region Test #8a - Size property with basevalue
            CommonLib.LogStatus("Test #8a - Size property with basevalue");
            ArcSegment as8a = new ArcSegment();
            CommonLib.SizeVerifier(as8a.Size, new Size(0, 0));
            #endregion

            #region Test #8b - Size property in Invalid state
            CommonLib.LogStatus("Test #8b - Size property in Invalid state");
            ArcSegment as8b = new ArcSegment();
            as8b.InvalidateProperty(ArcSegment.SizeProperty);
            CommonLib.SizeVerifier(as8b.Size, new Size(0, 0));
            #endregion

            #region Test #8 - the Size property
            CommonLib.LogStatus("Test #8 - the Size property");
            arcSegment1.Size = new Size(150, 100);
            CommonLib.SizeVerifier(arcSegment1.Size, new Size(150, 100));
            #endregion

            #region Test #8.5 - the SizeAnimations property
            CommonLib.LogStatus("Test #8.5 - the SizeAnimations property");
            SizeAnimation sizeAnim = new SizeAnimation(new Size(10, 10), new Size(30, 4), new Duration(TimeSpan.FromMilliseconds(2000)), FillBehavior.HoldEnd);
            AnimationClock sizeClock = sizeAnim.CreateClock();
            arcSegment25.ApplyAnimationClock(ArcSegment.SizeProperty, sizeClock);
            CommonLib.GenericVerifier(sizeClock != null, "SizeAnimation property");
            #endregion

            #region Test #9a - SweepDirection property with basevalue
            CommonLib.LogStatus("Test #9 - SweepDirection property with basevalue");
            ArcSegment as9a = new ArcSegment();
            CommonLib.GenericVerifier(as9a.SweepDirection == SweepDirection.Counterclockwise, "SweepDirection property with basevalue");
            #endregion

            #region Test #9b - SweepDirection property in Invalid state
            CommonLib.LogStatus("Test #9b - SweepDirection property in Invalid state");
            ArcSegment as9b = new ArcSegment();
            as9b.InvalidateProperty(ArcSegment.SweepDirectionProperty);
            CommonLib.GenericVerifier(as9b.SweepDirection == SweepDirection.Counterclockwise, "SweepDirection in Invalid state");
            #endregion

            #region Test #9 - the SweepDirection property
            CommonLib.LogStatus("Test #9 - the SweepDirection property");
            arcSegment1.SweepDirection = SweepDirection.Counterclockwise;
            CommonLib.GenericVerifier(arcSegment1.SweepDirection == SweepDirection.Counterclockwise, "SweepDirection property");
            #endregion

            #region Test #9.1 - SweepDirectionProperty DP
            CommonLib.LogStatus("Test #9.1 - SweepDirectionProperty DP");
            ArcSegment as91 = new ArcSegment(new Point(-22, -0.23), new Size(10, 1.233), 0, false, SweepDirection.Counterclockwise, false);
            as91.SetValue(ArcSegment.SweepDirectionProperty, SweepDirection.Clockwise);
            SweepDirection return91 = (SweepDirection)as91.GetValue(ArcSegment.SweepDirectionProperty);
            CommonLib.GenericVerifier(return91 == SweepDirection.Clockwise, "SweepDirectionProperty DP");
            #endregion

            #region Test #10a - RotationAngle property with basevalue
            CommonLib.LogStatus("Test #10a - RotationAngle property with basevalue");
            ArcSegment as10a = new ArcSegment();
            CommonLib.GenericVerifier(as10a.RotationAngle == 0, "RotationAngle property with basevalue");
            #endregion

            #region Test #10b - RotationAngle property in Invalid state
            CommonLib.LogStatus("Test #10b - RotationAngle property in Invalid state");
            ArcSegment as10b = new ArcSegment();
            as10b.InvalidateProperty(ArcSegment.RotationAngleProperty);
            CommonLib.GenericVerifier(as10b.RotationAngle == 0, "RotationAngle property in Invalid state");
            #endregion

            #region Test #10 - the RotationAngle property
            CommonLib.LogStatus("Test #10 - the RotationAngle property");
            arcSegment1.RotationAngle = 50;
            CommonLib.GenericVerifier(arcSegment1.RotationAngle == 50, "RotationAngle property");
            #endregion

            #region Test #10.2 - the RotationAngleAnimations property
            CommonLib.LogStatus("Test #10.2 - the RotationAngleAnimations property");
            DoubleAnimation doubleAnim = new DoubleAnimation(2.1, 30.333, new Duration(TimeSpan.FromMilliseconds(2000)));
            AnimationClock doubleClock = doubleAnim.CreateClock();
            arcSegment25.ApplyAnimationClock(ArcSegment.RotationAngleProperty, doubleClock);
            CommonLib.GenericVerifier(doubleClock != null, "RotationAngleAnimations property");
            #endregion

            #region Test #10.5b - CanFreeze
            CommonLib.LogStatus("Test #10.5b - the CanFreeze");
            CommonLib.GenericVerifier(!arcSegment25.CanFreeze, "CanFreeze property");
            #endregion

            #endregion

            #region Section III - ArcSegment class public methods
            #region Test #11 - the Copy() method
            //more in-depth tests of Copy() method will be in IChangeable test framework
            CommonLib.LogStatus("Test #11 - the Copy() method");
            ArcSegment arcSegment3 = arcSegment1.Clone() as ArcSegment;
            CommonLib.GenericVerifier(arcSegment3 != null, "Copy() method");
            #endregion

            #region Test #11.1 - Copy() with invalidated fields
            CommonLib.LogStatus("Test #11.1 - Copy() with invalidated fields");
            ArcSegment as111 = new ArcSegment();
            as111.InvalidateProperty(ArcSegment.IsLargeArcProperty);
            as111.InvalidateProperty(ArcSegment.SweepDirectionProperty);
            ArcSegment return111 = as111.Clone();
            CommonLib.GenericVerifier(return111 != null, "Copy() with invalidated fields");
            #endregion

            #region Test #12 - the CloneCurrentValue() method
            CommonLib.LogStatus("Test #12 - the CloneCurrentValue() method");
            ArcSegment arcSegment4 = arcSegment1.CloneCurrentValue() as ArcSegment;
            CommonLib.GenericVerifier(arcSegment4.Point == arcSegment1.Point && arcSegment4.Size == arcSegment1.Size && arcSegment4.RotationAngle == arcSegment1.RotationAngle && arcSegment4.SweepDirection == arcSegment1.SweepDirection && arcSegment4.IsLargeArc == arcSegment1.IsLargeArc && arcSegment4.IsStroked == arcSegment1.IsStroked, "CloneCurrentValue() method");
            #endregion

            #region Test #15 - Internal AddToFigure ()
            CommonLib.LogStatus("Test #15 - Internal AddToFigure ()");
            PathGeometry pg15 = new PathGeometry();
            pg15.Transform = new RotateTransform(98);
            PathGeometry pg151 = new PathGeometry();
            PathFigure pf15 = new PathFigure();
            pf15.StartPoint = new Point(32, 34);
            pf15.Segments.Add(new ArcSegment(new Point(200, 200), new Size(50, 10), 20, false, SweepDirection.Clockwise, true));
            pg151.Figures.Add(pf15);
            pg151.Transform = new RotateTransform(45);
            pg15.AddGeometry(pg151);
            CommonLib.GenericVerifier(pg15.Figures[0].Segments[0] is BezierSegment, "Internal AddToFigure()");
            #endregion

            #region Test #16 - Internal IsCurved ()
            CommonLib.LogStatus("Test #16 - Internal IsCurved ()");
            PathGeometry pg16 = new PathGeometry();
            PathFigure pf16 = new PathFigure();
            pf16.Segments.Add(new ArcSegment(new Point(1, 10), Size.Empty, 45, false, SweepDirection.Clockwise, false));
            pf16.Segments.Add(new LineSegment(new Point(100, 100), true));
            pg16.Figures.Add(pf16);
            bool result16 = pg16.MayHaveCurves();
            CommonLib.GenericVerifier(result16, "Internal IsCurved()");
            #endregion
            #endregion

            #region Section IV : Running stage
            CommonLib.Stage = TestStage.Run;

            CommonLib.LogStatus("Result for :" + objectType);

            PathFigure pf = new PathFigure();
            pf.StartPoint = new Point(30, 30);
            pf.Segments.Add(arcSegment1);
            pf.Segments.Add(arcSegment2);
            PathGeometry pg = new PathGeometry();
            pg.Figures.Add(pf);

            DC.DrawGeometry(new SolidColorBrush(Colors.Blue), null, pg);
            #endregion
        }
    }
}