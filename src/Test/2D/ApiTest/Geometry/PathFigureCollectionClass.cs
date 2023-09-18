// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  GMC API Tests - Testing PathFigureCollectionClass class
//  Author:   Microsoft
//
using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Summary description for PathFigureCollectionClass.
    /// </summary>
    internal class PathFigureCollectionClass : ApiTest
    {
        public PathFigureCollectionClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus("PathFigureCollection Class");

            string objectType = "System.Windows.Media.PathFigureCollection";

            #region Section I: Constructors
            #region Test #1: default constructor
            CommonLib.LogStatus("Test #1: default constructor");

            PathFigure pf1 = new PathFigure();
            pf1.StartPoint = new Point(1, 1);
            pf1.Segments.Add(new LineSegment(new Point(29, 39), true));
            pf1.IsClosed = true;

            PathFigure pf2 = new PathFigure();

            PathFigureCollection pfc1 = new PathFigureCollection();
            pfc1.Add(pf1);
            pfc1.Add(pf2);

            CommonLib.TypeVerifier(pfc1, objectType);
            #endregion

            #endregion

            #region Public properties

            #region Test #5.5: CanFreeze property
            CommonLib.LogStatus("Test #5.5: CanFreeze property");
            CommonLib.GenericVerifier(pfc1.CanFreeze, "CanFreeze property");
            #endregion

            #region Test #9: Indexer property
            CommonLib.LogStatus("Test #9: Indexer property");
            CommonLib.GenericVerifier(pfc1.Count == 2 && pfc1[0].Segments.Count == 1, "Indexer property");
            #endregion

            #region Test #9.25: IList.IsReadOnly property
            CommonLib.LogStatus("Test #9.25: IList.IsReadOnly property");
            CommonLib.GenericVerifier(!((IList)pfc1).IsReadOnly, "IList.IsReadOnly property");
            #endregion

            #region Test #9.5: IList.Item property
            CommonLib.LogStatus("Test #9.5: IList.Item property");
            PathFigureCollection pfc2 = new PathFigureCollection();
            pfc2.Add(new PathFigure());
            pfc2[0] = new PathFigure(new Point(23, 0.323),
                                     new PathSegment[] { new LineSegment(new Point(122.3, 100), true) },
                                     false);
            CommonLib.GenericVerifier(pfc2.Count == 1 && pfc2[0].Segments[0] is LineSegment, "IList.Item property");
            #endregion

            #endregion

            #region Section III: public methods
            #region Test #10: Add() method
            CommonLib.LogStatus("Test #10: Add() method");
            pfc1.Add(new PathFigure(new Point(10, 10),
                                        new PathSegment[] { new ArcSegment(new Point(60, 90), new Size(29, 34), 39, true, SweepDirection.Clockwise, false) },
                                        false));
            CommonLib.GenericVerifier(pfc1.Count == 3 && pfc1[2].Segments.Count == 1, "Add() method");
            #endregion

            #region Test #12: Clear() method
            CommonLib.LogStatus("Test #12: Clear() method");
            PathFigureCollection pfc12 = new PathFigureCollection();
            pfc12.Add(new PathFigure());
            pfc12.Clear();
            CommonLib.GenericVerifier(pfc12.Count == 0, "Clear() method");
            #endregion

            #region Test #13: Contains() method
            CommonLib.LogStatus("Test #13: Contains() method");
            CommonLib.GenericVerifier(pfc1.Contains(pfc1[0]) && !pfc1.Contains(new PathFigure()), "Contains() method");
            #endregion

            #region Test #14: Copy() method
            CommonLib.LogStatus("Test #14: Copy() method");
            PathFigureCollection pfc5 = pfc1.Clone();
            CommonLib.GenericVerifier(pfc5.Count == 3, "Copy() method");
            #endregion

            #region Test #15: CopyTo() method
            CommonLib.LogStatus("Test #15: CopyTo() method");
            PathFigure[] pfList = new PathFigure[5];
            pfc1.CopyTo(pfList, 0);
            CommonLib.GenericVerifier(pfList[0].Segments.Count == 1 && pfList[1].Segments.Count == 0 && pfList[2].Segments.Count == 1 && pfList[3] == null && pfList[4] == null, "CopyTo() method");
            #endregion

            #region Test #16: CloneCurrentValue() method
            CommonLib.LogStatus("Test #16: CloneCurrentValue() method");
            PathFigureCollection pfc6 = pfc1.CloneCurrentValue();
            CommonLib.GenericVerifier(pfc6.Count == 3 && pfc6[1].Segments.Count == 0, "CloneCurrentValue() method");
            #endregion

            #region Test #21: IndexOf(PathFigure) method
            CommonLib.LogStatus("Test #21: IndexOf(PathFigure) method");
            CommonLib.GenericVerifier(pfc1.IndexOf(pfc1[2]) == 2, "IndexOf(PathFigure) method");
            #endregion

            #region Test #22: Insert()
            CommonLib.LogStatus("Test #22: Insert()");
            PathFigureCollection pfc22 = new PathFigureCollection();
            pfc22.Add(pf1);
            pfc22.Add(pf2);
            //Insert a new StartSegment object to the second PathFigure object in the collection
            pfc22.Insert(1, new PathFigure(new Point(0, 10),
                                            new PathSegment[] { new LineSegment(new Point(10, 30), false) },
                                            false));
            //Check to see if pfc22 has three PathFigure object now, and the second PathFigure contains a LineSegment
            CommonLib.GenericVerifier(pfc22.Count == 3 && pfc22[1].Segments[0] is LineSegment, "Insert() method");
            #endregion

            #region Test #26: Remove()
            CommonLib.LogStatus("Test #26: Remove() method");
            pfc1.Remove(pfc1[2]);
            CommonLib.GenericVerifier(pfc1.Count == 2, "Remove() method");
            #endregion

            #region Test #27: RemoveAt() method
            CommonLib.LogStatus("Test #27: RemoveAt() method");
            PathFigureCollection pfc27 = new PathFigureCollection();
            pfc27.Add(new PathFigure());
            pfc27.Add(new PathFigure());
            pfc27.Add(new PathFigure());
            pfc27.Add(new PathFigure());

            pfc27.RemoveAt(1);
            CommonLib.GenericVerifier(pfc27.Count == 3, "RemoveAt() method");
            #endregion

            #region Test #30: IList.Add()
            CommonLib.LogStatus("Test #30: IList.Add()");
            PathFigureCollection pfc30 = new PathFigureCollection();
            pfc30.Add(new PathFigure());
            pfc30.Add(new PathFigure());
            ((IList)pfc30).Add(new PathFigure(new Point(0, 0), new PathSegment[] { }, false));
            CommonLib.GenericVerifier(pfc30.Count == 3, "IList.Add()");
            #endregion

            #region Test #31: IList.Contains
            CommonLib.LogStatus("Test #31: IList.Contains()");
            PathFigureCollection pfc31 = new PathFigureCollection();
            pfc31.Add(new PathFigure());
            pfc31.Add(new PathFigure(new Point(23, 20), new PathSegment[] { }, false));
            CommonLib.GenericVerifier(((IList)pfc31).Contains(pfc31[1]), "IList.Contains() method");
            #endregion

            #region Test #32: IList.IndexOf()
            CommonLib.LogStatus("Test #32: IList.IndexOf()");
            PathFigureCollection pfc32 = new PathFigureCollection();
            pfc32.Add(new PathFigure());
            pfc32.Add(new PathFigure(new Point(32, -123), new PathSegment[] { }, true));
            CommonLib.GenericVerifier(((IList)pfc32).IndexOf(pfc32[1]) == 1, "IList.IndexOf() method");
            #endregion

            #region Test #33: IList.Insert()
            CommonLib.LogStatus("Test #33: IList.Insert()");
            PathFigureCollection pfc33 = new PathFigureCollection();
            pfc33.Add(new PathFigure());
            pfc33.Add(new PathFigure(new Point(0, 0), new PathSegment[] { }, true));

            ((IList)pfc33).Insert(0, pf2);
            CommonLib.GenericVerifier(pfc33.Count == 3, "IList.Insert() method");
            #endregion

            #region Test #34: IList.Remove()
            CommonLib.LogStatus("Test #34: IList.Remove()");
            PathFigureCollection pfc34 = new PathFigureCollection();
            pfc34.Add(new PathFigure());
            pfc34.Add(new PathFigure(new Point(32, 2.345), new PathSegment[] { }, true));
            ((IList)pfc34).Remove(pfc34[1]);
            CommonLib.GenericVerifier(pfc34.Count == 1, "IList.Remove() method");
            #endregion

            #region Test #35: Regression case for Regression_Bug10
            CommonLib.LogStatus("Test #35: Regression case for Regression_Bug10");
            PathFigureCollection pfc35 = new PathFigureCollection();
            pfc35.Add(new PathFigure());
            pfc35.Add(new PathFigure());
            pfc35.Add(new PathFigure());

            PathFigureCollection pfc35_temp = pfc35.CloneCurrentValue();
            // As long as this case doesn't stuck, and fall into infinit loop, then it passes
            CommonLib.LogStatus("No regression for Regression_Bug10");
            #endregion

            #region Test: 36: ToString()
            CommonLib.LogStatus("Test: 36: ToString()");
            // Empty PathFigureCollection
            PathFigureCollection pfc36_Empty = new PathFigureCollection();

            // Contains one Empty PathFigure
            PathFigureCollection pfc36_SinglePF = new PathFigureCollection();
            pfc36_SinglePF.Add(new PathFigure());

            // Contains one PathFigure with contents
            // PathFigure contains all different segments
            PathFigure pf36_All = new PathFigure();
            pf36_All.Segments.Add(new LineSegment(new Point(-1, 100), true));
            pf36_All.Segments.Add(new ArcSegment(new Point(100, 100), new Size(100, 0), -10000, false, SweepDirection.Counterclockwise, false));
            pf36_All.Segments.Add(new BezierSegment(new Point(0, 0), new Point(100, Double.Epsilon), new Point(-1, Double.NegativeInfinity), true));
            pf36_All.Segments.Add(new PolyLineSegment(new Point[] { }, true));
            pf36_All.Segments.Add(new PolyBezierSegment(new Point[] { new Point(435.3, -123.23), new Point(0, 0), new Point(100, 10) }, true));
            pf36_All.Segments.Add(new PolyQuadraticBezierSegment(new Point[] { new Point(0, 0), new Point(-1, -1) }, false));
            pf36_All.Segments.Add(new QuadraticBezierSegment(new Point(0, 0), new Point(Double.NegativeInfinity, Double.PositiveInfinity), true));
            PathFigureCollection pfc36_ContentPF = new PathFigureCollection();
            pfc36_ContentPF.Add(pf36_All);

            // Contains multiple empty PathFigures
            PathFigureCollection pfc36_PFs = new PathFigureCollection();
            for (int i = 0; i < 10; i++)
            {
                pfc36_PFs.Add(new PathFigure());
            }

            // The middle PathFigure is closed, but others are opened.
            PathFigureCollection pfc36_MilClosed = new PathFigureCollection();
            pfc36_MilClosed.Add(new PathFigure());
            PathFigure pf36_Closed = new PathFigure();
            pf36_Closed.IsClosed = true;
            pfc36_MilClosed.Add(pf36_Closed);
            pfc36_MilClosed.Add(new PathFigure());

            bool result36 = CommonLib.CompareString("", pfc36_Empty.ToString());
            result36 &= CommonLib.CompareString("M0,0", pfc36_SinglePF.ToString());
            result36 &= CommonLib.CompareString(
                "M0,0L-1,100A100,0,-10000,0,0,100,100C0,0,100,4.94065645841247E-324,-1,-InfinityLC435.3,-123.23 0,0 100,10Q0,0 -1,-1Q0,0,-Infinity,Infinity",
                pfc36_ContentPF.ToString());
            result36 &= CommonLib.CompareString("M0,0 M0,0 M0,0 M0,0 M0,0 M0,0 M0,0 M0,0 M0,0 M0,0",
                pfc36_PFs.ToString());

            CommonLib.GenericVerifier(result36, "ToString()");
            #endregion

            #region Test #37: ToString(System.IFormatProvider)
            CommonLib.LogStatus("Test #37: ToString(System.IFormatProvider)");
            bool result37 = CommonLib.CompareString("", pfc36_Empty.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
            result37 &= CommonLib.CompareString("M0,0", pfc36_SinglePF.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
            result37 &= CommonLib.CompareString(
                "M0,0L-1,100A100,0,-10000,0,0,100,100C0,0,100,4.94065645841247E-324,-1,-InfinityLC435.3,-123.23 0,0 100,10Q0,0 -1,-1Q0,0,-Infinity,Infinity",
                pfc36_ContentPF.ToString(CultureInfo.InvariantCulture),
                CultureInfo.InvariantCulture);
            result37 &= CommonLib.CompareString("M0,0 M0,0 M0,0 M0,0 M0,0 M0,0 M0,0 M0,0 M0,0 M0,0",
                pfc36_PFs.ToString(CultureInfo.InvariantCulture),
                CultureInfo.InvariantCulture);

            CommonLib.GenericVerifier(result37, "ToString(System.IFormatProvider)");
            #endregion
            #endregion

            #region Section IV: Running stage
            CommonLib.Stage = TestStage.Run;

            CommonLib.LogStatus("Result for:" + objectType);
            #endregion
        }
    }
}
