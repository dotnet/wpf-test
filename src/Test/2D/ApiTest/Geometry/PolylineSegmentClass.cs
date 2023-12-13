// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  GMC API Tests - Testing PolyLineSegment class
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
    /// Summary description for PolyBezierSegmentClass.
    /// </summary>
    internal class PolyLineSegmentClass : ApiTest
    {
        public PolyLineSegmentClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus("PolyLineSegment Class");

            string objectType = "System.Windows.Media.PolyLineSegment";

            #region Section I: Constructors
            #region Test #1: Default constructor
            CommonLib.LogStatus("Test #1: Default Constructor");
            PolyLineSegment pls1 = new PolyLineSegment();
            CommonLib.TypeVerifier(pls1, objectType);
            #endregion

            #region Test #2: PolyLineSegment(Point[], bool) constructor
            CommonLib.LogStatus("Test #2: PolyLineSegment(Point[], bool) constructor");
            PolyLineSegment pls2 = new PolyLineSegment(new Point[] { new Point(32, 10), new Point(39, 100) }, false);
            CommonLib.TypeVerifier(pls2, objectType);
            #endregion

            #endregion

            #region Section II: public methods
            #region Test #4: Copy() method
            CommonLib.LogStatus("Test #4: Copy() method");
            PolyLineSegment pls4 = pls2.Clone();
            CommonLib.GenericVerifier(pls4 != null && pls4.Points.Count == 2, "Copy() method");
            #endregion

            #region Test #5: CloneCurrentValue() method
            CommonLib.LogStatus("Test #5: CloneCurrentValue() method");
            PolyLineSegment pls5 = pls2.CloneCurrentValue();
            CommonLib.GenericVerifier(pls5 != null && pls5.Points.Count == 2, "Copy() method");
            #endregion

            #region Test #5.6 - AddPoint()
            CommonLib.LogStatus("Test #5.6 - AddPoint()");
            PolyLineSegment pls56 = new PolyLineSegment();
            pls56.Points.Add(new Point(0, 23));
            CommonLib.GenericVerifier(pls56.Points.Count == 1, "AddPoint() method");
            #endregion

            #region Test #6 - Internal API - AddToFigure()
            CommonLib.LogStatus("Test #6 - Internal API - AddToFigure()");
            //The AddToFigure() will be called on the PolyLineSegment when a PathGeometry with the PolyLineSegment being adding 
            //to another PathGeometry.  The target PathGeometry will recursively call the AddToFigure in each Segment on the base PathGeometry
            //to add the Segment into its own FigureCollection.
            PathGeometry pg6_base = new PathGeometry();
            PathFigure pf6 = new PathFigure();
            pf6.StartPoint = new Point(23, 2);
            pf6.Segments.Add(new PolyLineSegment(new Point[] { new Point(20, 20), new Point(39, 20), new Point(10, 100) }, false));
            pg6_base.Figures.Add(pf6);
            PathGeometry pg6_target = new PathGeometry();
            pg6_target.AddGeometry(pg6_base);

            CommonLib.GenericVerifier(pg6_target != null && pg6_target.Figures[0].Segments[0] is PolyLineSegment, "AddToFigure()");
            #endregion

            #region Test #7 - Internal API - IsCurved()
            CommonLib.LogStatus("Test #7 - Internal API - IsCurved()");
            //IsCurved on PolyLineSegment will be called when PathFigure.MayHaveCurves() is used by program.  
            //MayHaveCurves() will call each Segment object in the SegmentsCollection to figure up if it contains Curve
            PathFigure pf7 = new PathFigure();
            pf7.StartPoint = new Point(92, 12.32);
            pf7.Segments.Add(new PolyLineSegment(new Point[] { new Point(20, 20), new Point(39, 20), new Point(10, 100) }, true));
            bool result7 = pf7.MayHaveCurves();

            CommonLib.GenericVerifier(result7 == false, "IsCurved()");
            #endregion
            #endregion

            #region Section III: public properties
            #region Test #9: CanFreeze Property
            CommonLib.LogStatus("Test #9: CanFreeze property");
            CommonLib.GenericVerifier(pls2.CanFreeze, "CanFreeze property");
            #endregion

            #region Test #10a: Points property with basevalue
            CommonLib.LogStatus("Test #10a: Points property with basevalue");
            PolyLineSegment pls10a = new PolyLineSegment();
            CommonLib.GenericVerifier(pls10a.Points.Count == 0, "Points property with basevalue");
            #endregion

            #region Test #10b:  Points property in Invalid state
            CommonLib.LogStatus("Test #10b:  Points property in Invalid property");
            PolyLineSegment pls10b = new PolyLineSegment();
            CommonLib.GenericVerifier(pls10b.Points.Count == 0, "Points property in Invalid state");
            #endregion

            #region Test #10: Points property
            CommonLib.LogStatus("Test #10: Points property");
            PointCollection pc = pls2.Points;
            CommonLib.GenericVerifier(pc != null && pc.Count == 2, "Points property");
            #endregion

            #region Test #11: PointsProperty
            CommonLib.LogStatus("Test #11: PointsProperty");
            PolyLineSegment pls11 = new PolyLineSegment(new Point[] { new Point(32, 10), new Point(39, 100) }, false);
            PointCollection pc11 = (PointCollection)pls11.GetValue(PolyLineSegment.PointsProperty);
            CommonLib.GenericVerifier(pc11 != null && pc11.Count == 2, "PointsProperty");
            #endregion
            #endregion

            #region Section IV: Running stage
            CommonLib.Stage = TestStage.Run;
            CommonLib.LogTest("Result for:" + objectType);

            PathFigure pf = new PathFigure(new Point(30, 20), new PathSegment[] { pls2 }, false);

            pf.IsClosed = true;

            PathGeometry pg = new PathGeometry();
            pg.Figures.Add(pf);

            DC.DrawGeometry(new SolidColorBrush(Colors.Red), null, pg);
            #endregion
        }
    }
}
