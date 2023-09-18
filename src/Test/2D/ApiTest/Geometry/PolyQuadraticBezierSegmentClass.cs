// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  GMC API Tests - Testing PolyQuadraticBezierSegment class
//  Author:   Microsoft
//
using System;
using System.ComponentModel;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Summary description for PolyQuadraticBezierSegmentClass.
    /// </summary>
    internal class PolyQuadraticBezierSegmentClass : ApiTest
    {
        public PolyQuadraticBezierSegmentClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus("PolyQuadraticBezierSegment Class");

            string objectType = "System.Windows.Media.PolyQuadraticBezierSegment";

            #region Section I: Constructors
            #region Test #1: Default constructor
            CommonLib.LogStatus("Test #1: Default Constructor");

            PolyQuadraticBezierSegment pls1 = new PolyQuadraticBezierSegment();

            CommonLib.TypeVerifier(pls1, objectType);
            #endregion

            #region Test #2: PolyQuadraticBezierSegment(Point[], bool) constructor
            CommonLib.LogStatus("Test #2: PolyQuadraticBezierSegment(Point[], bool) constructor");

            PolyQuadraticBezierSegment pls2 = new PolyQuadraticBezierSegment(new Point[] { new Point(32, 10), new Point(39, 100) }, false);

            CommonLib.TypeVerifier(pls2, objectType);
            #endregion

            #endregion

            #region Section II: public methods
            #region Test #4: Copy() method
            CommonLib.LogStatus("Test #4: Copy() method");

            PolyQuadraticBezierSegment pls4 = pls2.Clone();

            CommonLib.GenericVerifier(pls4 != null && pls4.Points.Count == 2, "Copy() method");
            #endregion

            #region Test #5: CloneCurrentValue() method
            CommonLib.LogStatus("Test #5: CloneCurrentValue() method");

            PolyQuadraticBezierSegment pls5 = pls2.CloneCurrentValue();

            CommonLib.GenericVerifier(pls5 != null && pls5.Points.Count == 2, "Copy() method");
            #endregion

            #region Test #6 - AddPoint()
            CommonLib.LogStatus("Test #6 - AddPoint()");
            PolyQuadraticBezierSegment pqs6 = new PolyQuadraticBezierSegment();
            pqs6.Points.Add(new Point(3, 23));
            CommonLib.GenericVerifier(pqs6.Points.Count == 1, "AddPoint() method");
            #endregion

            #region Test #7 - Internal API - AddToFigure()
            CommonLib.LogStatus("Test #7 - Internal API - AddToFigure()");
            //The AddToFigure() will be called on the PolyQuadraticBezierSegment when a PathGeometry with the PolyQuadraticBezierSegment being adding 
            //to another PathGeometry.  The target PathGeometry will recursively call the AddToFigure in each Segment on the base PathGeometry
            //to add the Segment into its own FigureCollection.
            PathGeometry pg7_base = new PathGeometry();
            PathFigure pf7 = new PathFigure();
            pf7.StartPoint = new Point(23, 2);
            pf7.Segments.Add(new PolyQuadraticBezierSegment(new Point[] { new Point(32, 10), new Point(39, 100) }, false));
            pg7_base.Figures.Add(pf7);
            PathGeometry pg7_target = new PathGeometry();
            pg7_target.AddGeometry(pg7_base);

            CommonLib.GenericVerifier(pg7_target != null && pg7_target.Figures[0].Segments[0] is PolyQuadraticBezierSegment, "AddToFigure()");
            #endregion

            #region Test #8 - Internal API - IsCurved()
            CommonLib.LogStatus("Test #8 - Internal API - IsCurved()");
            //IsCurved on PolyQuadraticBezierSegment will be called when PathFigure.MayHaveCurves() is used by program.  
            //MayHaveCurves() will call each Segment object in the SegmentsCollection to figure up if it contains Curve
            PathFigure pf8 = new PathFigure();
            pf8.StartPoint = new Point(92, 12.32);
            pf8.Segments.Add(new PolyQuadraticBezierSegment(new Point[] { new Point(32, 10), new Point(39, 100) }, false));
            bool result8 = pf8.MayHaveCurves();

            CommonLib.GenericVerifier(result8 == true, "IsCurved()");
            #endregion
            #endregion

            #region Section III: public properties
            #region Test #9: CanFreeze Property
            CommonLib.LogStatus("Test #6: CanFreeze property");
            CommonLib.GenericVerifier(pls2.CanFreeze, "CanFreeze property");
            #endregion

            #region Test #10a:  Points property with basevalue
            CommonLib.LogStatus("Test #10a: Points property with basevalue");
            PolyQuadraticBezierSegment pqs10a = new PolyQuadraticBezierSegment();
            CommonLib.GenericVerifier(pqs10a.Points.Count == 0, "Points property with basevalue");
            #endregion

            #region Test #10b:  Points property in Invalid state
            CommonLib.LogStatus("Test #10b:  Points property in Invalid state");
            PolyQuadraticBezierSegment pqs10b = new PolyQuadraticBezierSegment();
            pqs10b.InvalidateProperty(PolyQuadraticBezierSegment.PointsProperty);
            CommonLib.GenericVerifier(pqs10b.Points.Count == 0, "Points property in Invalid state");
            #endregion

            #region Test #10: Points property
            CommonLib.LogStatus("Test #10: Points property");
            PointCollection pc = pls2.Points;
            CommonLib.GenericVerifier(pc != null && pc.Count == 2, "Points property");
            #endregion

            #region #11 PointsProperty
            CommonLib.LogStatus("Test #11: PointsProperty");
            PolyQuadraticBezierSegment pqb11 = new PolyQuadraticBezierSegment(new Point[] { new Point(32, 10), new Point(39, 100) }, false);
            PointCollection pc11 = (PointCollection)pqb11.GetValue(PolyQuadraticBezierSegment.PointsProperty);
            CommonLib.GenericVerifier(pc11 != null && pc11.Count == 2, "PointsProperty");
            #endregion
            #endregion

            #region Section IV: Running stage
            CommonLib.Stage = TestStage.Run;

            CommonLib.LogTest("Result for: " + objectType);

            PathFigure pf = new PathFigure(new Point(30, 20), new PathSegment[] { pls2 }, false);

            pf.IsClosed = true;

            PathGeometry pg = new PathGeometry();
            pg.Figures.Add(pf);

            DC.DrawGeometry(new SolidColorBrush(Colors.Red), null, pg);
            #endregion
        }
    }
}
