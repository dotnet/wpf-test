// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  GMC API Tests - Testing PolyBezierSegment class
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
    internal class PolyBezierSegmentClass : ApiTest
    {
        public PolyBezierSegmentClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus("PolyBezierSegment Class");

            string objectType = "PolyBezierSegment";

            #region Section I: Constructors
            #region Test #1: Default constructor
            CommonLib.LogStatus("Test #1: Default Constructor");
            PolyBezierSegment pbs1 = new PolyBezierSegment();
            CommonLib.TypeVerifier(pbs1, objectType);
            #endregion

            #region Test #2: PolyBezierSegment(Point[], int) constructor
            CommonLib.LogStatus("Test #2: PolyBezierSegment(Point[], bool) constructor");
            PolyBezierSegment pbs2 = new PolyBezierSegment(new Point[] { new Point(20, 20), new Point(39, 20), new Point(10, 100) }, false);
            CommonLib.TypeVerifier(pbs2, objectType);
            #endregion
            #endregion

            #region Section II: public properties
            #region Test #4: CanFreeze property
            CommonLib.LogStatus("Test #4: CanFreeze property");
            CommonLib.GenericVerifier(pbs2.CanFreeze, "CanFreeze property");
            #endregion

            #region Test #7a: Point property with basevalue
            CommonLib.LogStatus("Test #7a: Point property with basevalue");
            PolyBezierSegment pbs7a = new PolyBezierSegment();
            CommonLib.GenericVerifier(pbs7a.Points.Count == 0, "Points property with basevalue");
            #endregion

            #region Test #7b: Point property in Invalid state
            CommonLib.LogStatus("Test #7b: Point property in Invalid state");
            PolyBezierSegment pbs7b = new PolyBezierSegment();
            pbs7b.InvalidateProperty(PolyBezierSegment.PointsProperty);
            CommonLib.GenericVerifier(pbs7b.Points.Count == 0, "Points property in Invalid state");
            #endregion

            #region Test #7: Points property
            CommonLib.LogStatus("Test #7: Points property");
            PointCollection pc = pbs2.Points;
            CommonLib.GenericVerifier(pc.Count == 3, "Points property");
            #endregion

            #region Test #7.1: PointsProperty
            CommonLib.LogStatus("Test #7.1: PointsProperty");
            PolyBezierSegment pbs71 = new PolyBezierSegment(new Point[] { new Point(20, 20), new Point(39, 20), new Point(10, 100) }, false);
            PointCollection pc71 = (PointCollection)pbs71.GetValue(PolyBezierSegment.PointsProperty);
            CommonLib.GenericVerifier(pc71 != null && pc71.Count == 3, "PointsProperty");
            #endregion

            #endregion

            #region Section II: Public methods
            #region Test #8: Copy() method
            CommonLib.LogStatus("Test #8: Copy() method");
            PolyBezierSegment pbs4 = pbs2.Clone();
            CommonLib.GenericVerifier(pbs4 != null && pbs4.Points.Count == 3, "Copy() method");
            #endregion

            #region Test #9: CloneCurrentValue()
            CommonLib.LogStatus("Test #9: CloneCurrentValue() method");
            PolyBezierSegment pbs5 = pbs2.CloneCurrentValue();
            CommonLib.GenericVerifier(pbs5 != null && pbs5.Points.Count == 3, "CloneCurrentValue() method");
            #endregion

            #region Test #11 - AddPoint()
            CommonLib.LogStatus("Test #11 - AddPoint()");
            PolyBezierSegment pbs11 = new PolyBezierSegment();
            pbs11.Points = new PointCollection();
            pbs11.Points.Add(new Point(0, 0));
            pbs11.Points.Add(new Point(1, 2));
            pbs11.Points.Add(new Point(19, 229));

            pbs11.Points.Add(new Point(-1, -22));
            CommonLib.GenericVerifier(pbs11.Points.Count == 4, "AddPoint() method");
            #endregion

            #region Test #12 - Internal API - AddToFigure()
            CommonLib.LogStatus("Test #12 - Internal API - AddToFigure()");
            //The AddToFigure() will be called on the PolyBezierSegment when a PathGeometry with the PolyBezierSemgnet being adding 
            //to another PathGeometry.  The target PathGeometry will recursively call the AddToFigure in each Segment on the base PathGeometry
            //to add the Segment into its own FigureCollection.
            PathGeometry pg12_base = new PathGeometry();
            PathFigure pf12 = new PathFigure();
            pf12.StartPoint = new Point(23, 2);
            pf12.Segments.Add(new PolyBezierSegment(new Point[] { new Point(20, 20), new Point(39, 20), new Point(10, 100) }, false));
            pg12_base.Figures.Add(pf12);
            PathGeometry pg12_target = new PathGeometry();
            pg12_target.AddGeometry(pg12_base);

            CommonLib.GenericVerifier(pg12_target != null && pg12_target.Figures[0].Segments[0] is PolyBezierSegment, "AddToFigure()");
            #endregion

            #region Test #13 - Internal API - IsCurved()
            CommonLib.LogStatus("Test #13 - Internal API - IsCurved()");
            //IsCurved on PolyBezierSegment will be called when PathFigure.MayHaveCurves() is used by program.  
            //MayHaveCurves() will call each Segment object in the SegmentsCollection to figure up if it contains Curve
            PathFigure pf13 = new PathFigure();
            pf13.StartPoint = new Point(92, 12.32);
            pf13.Segments.Add(new PolyBezierSegment(new Point[] { new Point(20, 20), new Point(39, 20), new Point(10, 100) }, true));
            bool result13 = pf13.MayHaveCurves();

            CommonLib.GenericVerifier(result13 == true, "IsCurved()");
            #endregion
            #endregion

            #region Section IV: Running stage
            CommonLib.Stage = TestStage.Run;
            if (CommonLib.TestPassing)
            {
                CommonLib.LogStatus(objectType + " class passed the unit tests");
            }

            PathFigure pf = new PathFigure(new Point(30, 20), new PathSegment[] { pbs2 }, false);
            pf.IsClosed = true;
            PathGeometry pg = new PathGeometry();
            pg.Figures.Add(pf);
            DC.DrawGeometry(new SolidColorBrush(Colors.Red), null, pg);
            #endregion
        }
    }
}