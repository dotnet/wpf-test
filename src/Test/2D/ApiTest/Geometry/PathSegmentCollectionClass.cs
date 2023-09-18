// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  GMC API Tests - Testing PathSegmentCollection class
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
    /// Summary description for PathSegmentCollectionClass.
    /// </summary>
    internal class PathSegmentCollectionClass : ApiTest
    {
        public PathSegmentCollectionClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus("PathSegmentCollection Class");

            string objectType = "System.Windows.Media.PathSegmentCollection";

            #region Section I: Constructors
            #region Test #1: default constructor
            CommonLib.LogStatus("Test #1: default constructor");
            PathSegmentCollection psc1 = new PathSegmentCollection();
            psc1.Add(new LineSegment(new Point(0, 0), false));
            psc1.Add(new LineSegment(new Point(20, 60), true));

            CommonLib.TypeVerifier(psc1, objectType);
            #endregion

            #endregion

            #region Section II: Public properties

            #region Test #6: Count property
            CommonLib.LogStatus("Test #6: Cound property");
            CommonLib.GenericVerifier(psc1.Count == 2, "Count property Getter");
            #endregion

            #region Test #7: CanFreeze property
            CommonLib.LogStatus("Test #7: CanFreeze property");
            CommonLib.GenericVerifier(psc1.CanFreeze, "CanFreeze property Getter");
            #endregion

            #region Test #11: Indexer
            CommonLib.LogStatus("Test 11: Indexer");
            CommonLib.GenericVerifier(psc1[1] is LineSegment && ((LineSegment)psc1[1]).Point.X == 20 && ((LineSegment)psc1[1]).Point.Y == 60, "Indexer of PathSegmentCollection");
            #endregion

            #region Test #11.1: IList.IsReadOnly
            CommonLib.LogStatus("Test #11.1: IList.IsReadOnly");
            CommonLib.GenericVerifier(!((IList)psc1).IsReadOnly, "IList.IsReadOnly property");
            #endregion

            #region Test #11.2: IList.Contains
            CommonLib.LogStatus("Test #11.2: IList.Contains");
            psc1.Add(new LineSegment(new Point(1.2, 32), true));
            CommonLib.GenericVerifier(psc1.Contains(psc1[0]) && !psc1.Contains(new LineSegment(new Point(2, 2), false)), "IList.Contains property");
            #endregion

            #region Test #11.3: IList.Item
            CommonLib.LogStatus("Test #11.3: IList.Item property");
            PathSegmentCollection psc113 = new PathSegmentCollection();
            psc113.Add(new LineSegment(new Point(0, 2), false));
            ((IList)psc113)[0] = new BezierSegment(new Point(2, 2), new Point(32, 1), new Point(-12, 19), false);
            CommonLib.GenericVerifier(((IList)psc113)[0] is BezierSegment, "IList.Item property");
            #endregion
            #endregion

            #region Section III: public methods
            #region Test #12: Add() method
            CommonLib.LogStatus("Test #12: Add() method");
            psc1.Add(new LineSegment(new Point(60, 100), false));
            CommonLib.GenericVerifier(psc1.Count == 4 && psc1[3] is LineSegment && ((LineSegment)psc1[3]).Point.X == 60 && ((LineSegment)psc1[3]).Point.Y == 100, "Add() method");
            #endregion

            #region Test #14: Clear() method
            CommonLib.LogStatus("Test #14: clear() method");
            PathSegmentCollection psc14 = new PathSegmentCollection();
            psc14.Add(new LineSegment(new Point(0, 0), false));
            psc14.Add(new LineSegment(new Point(20, 60), true));
            psc14.Clear();
            CommonLib.GenericVerifier(psc14.Count == 0, "Clear() method");
            #endregion

            #region Test #15: Copy() method
            CommonLib.LogStatus("Test #15: Copy() method");
            PathSegmentCollection psc_temp = new PathSegmentCollection();
            psc_temp.Add(new ArcSegment(new Point(0, 2), new Size(400, 20), 45, true, SweepDirection.Clockwise, false));
            psc_temp.Add(new LineSegment(new Point(20, 60), true));
            PathSegmentCollection psc5 = psc_temp.Clone();
            CommonLib.GenericVerifier(psc5.Count == 2 && psc5[0] is ArcSegment && psc5[1] is LineSegment, "Copy() method");
            #endregion

            #region Test #16: CopyTo() method
            CommonLib.LogStatus("Test #16: CopyTo() method");
            PathSegment[] newSegmentArray = new PathSegment[4];
            psc1.CopyTo(newSegmentArray, 0);
            CommonLib.GenericVerifier(newSegmentArray[0] is LineSegment && newSegmentArray[1] is LineSegment && newSegmentArray[2] is LineSegment, "CopyTo() method");
            #endregion

            #region Test #17: CloneCurrentValue() method
            CommonLib.LogStatus("Test #17: CloneCurrentValue() method");
            PathSegmentCollection psc6 = psc1.CloneCurrentValue();
            CommonLib.GenericVerifier(psc6 != null && psc6.Count == 4 && psc6[2] is LineSegment, "CloneCurrentValue() method");
            #endregion

            #region Test #22: IndexOf(PathSegment)
            CommonLib.LogStatus("Test #22: IndexOf(PathSegment)");
            CommonLib.GenericVerifier(psc1.IndexOf(psc1[2]) == 2, "IndexOf(PathSegment) method");
            #endregion

            #region Test #23: Insert()
            CommonLib.LogStatus("Test #23: Insert()");
            psc1.Insert(2, new BezierSegment(new Point(12, 20), new Point(50, 29), new Point(29, 90), false));
            CommonLib.GenericVerifier(psc1.Count == 5 && psc1[2] is BezierSegment, "Insert() method");
            #endregion

            #region Test #27: Remove() method
            CommonLib.LogStatus("Test #27: Remove() method");
            psc1.Remove(psc1[2]);
            CommonLib.GenericVerifier(psc1.Count == 4, "Remove() method");
            #endregion

            #region Test #28: RemoveAt() method
            CommonLib.LogStatus("Test #28: RemoveAt() method");
            psc6.RemoveAt(2);
            CommonLib.GenericVerifier(psc6.Count == 3, "RemoveAt() method");
            #endregion

            #region Test #30: IList.Add()
            CommonLib.LogStatus("Test #30: IList.Add()");
            PathSegmentCollection psc30 = new PathSegmentCollection();
            ((IList)psc30).Add(new LineSegment(new Point(30, 23.123), true));
            CommonLib.GenericVerifier(psc30.Count == 1 && psc30[0] is LineSegment, "IList.Add() method");
            #endregion

            #region Test #31:  IList.IndexOf()
            CommonLib.LogStatus("Test #31: IList.IndexOf()");
            PathSegmentCollection psc31 = new PathSegmentCollection();
            ((IList)psc31).Add(new LineSegment(new Point(32.32, 22), false));
            CommonLib.GenericVerifier(((IList)psc31).IndexOf(psc31[0]) == 0 && ((IList)psc31).IndexOf(new LineSegment(new Point(32.32, 22), false)) == -1, "IList.IndexOf() method");
            #endregion

            #region Test #32: IList.Insert()
            CommonLib.LogStatus("Test #32: IList.Insert()");
            PathSegmentCollection psc32 = new PathSegmentCollection();
            ((IList)psc32).Insert(0, new ArcSegment(new Point(22, 22), new Size(1, 12.22), 12, true, SweepDirection.Counterclockwise, true));
            CommonLib.GenericVerifier(psc32.Count == 1 && psc32[0] is ArcSegment, "IList.Insert() method");
            #endregion

            #region Test #33: IList.Remove()
            CommonLib.LogStatus("Test #33: IList.Remove()");

            //Should not find a match, so nothing should be removed
            ((IList)psc6).Remove(new LineSegment(new Point(3232, 32.1), true));

            //Should remove the index 1 object which is an ArcSegment
            ((IList)psc6).Remove(psc6[1]);

            CommonLib.GenericVerifier(psc6.Count == 2 && !(psc6[1] is ArcSegment), "IList.Remove method");
            #endregion

            #region Test #34:  IList.Contains()
            CommonLib.LogStatus("Test #34:  IList.Contains()");
            PathSegmentCollection psc34 = new PathSegmentCollection();
            psc34.Add(new ArcSegment(new Point(0, 2), new Size(400, 20), 45, true, SweepDirection.Clockwise, false));
            psc34.Add(new LineSegment(new Point(23, 23), false));
            CommonLib.GenericVerifier(psc34.Contains(psc34[0]) == true && psc34.Contains(new LineSegment(new Point(0, 0), true)) == false, "IList.Contains()");
            #endregion
            #endregion

            #region Section IV: Running stage
            CommonLib.Stage = TestStage.Run;

            CommonLib.LogTest("Result for: " + objectType);

            PathFigure pf = new PathFigure(new Point(0, 0), psc1, true);
            PathGeometry pg = new PathGeometry();
            pg.Figures.Add(pf);
            DC.DrawGeometry(null, new Pen(new SolidColorBrush(Colors.Red), 5), pg);
            #endregion
        }
    }
}
