// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Program:  GMC API Tests - Testing PathSegment & PathSegmentEnumerator class
//  Author:   Microsoft
//
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{ 
    /// <summary>
    /// Summary description for PathSegmentEnumerator.
    /// </summary>
    internal class PathSegmentClass : ApiTest
    {
        public PathSegmentClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus("PathSegmentCollection Class");
            string objectType = "LineGeometry";

            CommonLib.LogStatus("Create a PathSegmentCollection");
            PathSegmentCollection psc = new PathSegmentCollection();
            psc.Add(new LineSegment(new Point(23.23, 11), true));
            psc.Add(new ArcSegment(new Point(0, 2), new Size(400, 20), 45, true, SweepDirection.Clockwise, false));
            psc.Add(new LineSegment(new Point(-10, 10), false));

            CommonLib.LogStatus("Getting the PathSegmentEnumerator from the PathSegmentCollection");
            IEnumerator pse = ((IEnumerable)psc).GetEnumerator();

            if (pse == null)
            {
                throw new System.ApplicationException("Fail to get the PathSegmentEnumerator");
            }

            #region PathSegmentEnumerator's public APIs
            CommonLib.LogStatus("Test:  MoveNext()");
            pse.MoveNext();
            pse.MoveNext();
            CommonLib.GenericVerifier(pse.Current is ArcSegment, "MoveNext");

            CommonLib.LogStatus("Test:  MoveNext() with NULL list");
            PathSegmentCollection pscNULL = new PathSegmentCollection();
            IEnumerator pseNULL = ((IEnumerable)pscNULL).GetEnumerator();
            bool result = pseNULL.MoveNext();
            CommonLib.GenericVerifier(!result, "MoveNext with NULL list");

            CommonLib.LogStatus("Test:  Reset()");
            pse.Reset();
            //Reset() will reset the internal index to -1, 
            //so MoveNext() will move to the first item in the list.
            pse.MoveNext();
            CommonLib.GenericVerifier(pse.Current is LineSegment, "Reset()");

            CommonLib.LogStatus("Test:  Current property in a PathSegmentEnumerator with NULL list");
            try
            {
                pseNULL.Reset();
                object o = pseNULL.Current;
                CommonLib.GenericVerifier(false, "Reset()");
            }
            catch (System.InvalidOperationException)
            {
                CommonLib.LogStatus("System.InvalidOperationException is correctly thrown");
                CommonLib.GenericVerifier(true, "Reset()");
            }
            #endregion

            #region PathSegment class
            CommonLib.LogStatus("Test:  PathSegment.IsStrokedGetPropertyValue");
            LineSegment ls1 = new LineSegment(new Point(12, 2), true);
            bool result1 = (bool)ls1.GetValue(PathSegment.IsStrokedProperty);
            CommonLib.GenericVerifier(result1 == true, "PathSegment.IsStrokedGetPropertyValue");
            #endregion

            CommonLib.Stage = TestStage.Run;
            CommonLib.LogTest("Results for:" + objectType);
        }
    }
}
