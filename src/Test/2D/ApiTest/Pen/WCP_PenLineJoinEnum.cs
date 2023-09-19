// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test all public API's in the PenLineJoin enum
//
using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class WCP_PenLineJoinEnum : ApiTest
    {

        public WCP_PenLineJoinEnum( double left, double top, double width, double height)
            : base(left, top, width, height)
        {            
            _class_testresult = true;
            _objectType = typeof(System.Windows.Media.PenLineJoin);
            _helper = new HelperClass();
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus(_objectType.ToString());

            // Create a Pen
            Pen pen = new Pen(Brushes.Red, 10.0);

            // Create a Path Geometry
            PathGeometry path = new PathGeometry();
            PathFigure fig1 = new PathFigure();
            fig1.StartPoint = new Point(20.0, 20.0);
            fig1.Segments.Add(new LineSegment(new Point(80.0, 20.0), true));
            fig1.Segments.Add(new LineSegment(new Point(50.0, 80.0), true));
            fig1.IsClosed = true;
            path.Figures.Add(fig1);

            #region SECTION I - ENUMERATIONS
            CommonLib.LogStatus("***** SECTION I - ENUMERATIONS *****");

            #region Test #1 - Miter
            // Usage: PenLineJoin.Miter
            // Notes: Value = int32(0x00000000)  
            CommonLib.LogStatus("Test #1 - Miter");

            // Set the LineJoin of a Pen to Miter
            pen.LineJoin = PenLineJoin.Miter;

            // Draw a Geometry with the Pen
            DC.DrawGeometry(null, pen, path);

            // Check the LineJoin value to assure that it is Miter.
            _class_testresult &= _helper.CompareProp("Miter - LineJoin", (int)PenLineJoin.Miter, (int)pen.LineJoin);
            #endregion

            #region Test #2 - Bevel
            // Usage: PenLineJoin.Bevel
            // Notes: Value = int32(0x00000001)  
            CommonLib.LogStatus("Test #2 - Bevel");

            // Set the LineJoin of a Pen to Bevel
            Pen pen2 = pen.Clone();
            pen2.LineJoin = PenLineJoin.Bevel;

            // Draw a Geometry with the Pen
            PathGeometry path2 = path.Clone();
            path2.Transform = new TranslateTransform(80, 0);
            DC.DrawGeometry(null, pen2, path2);

            // Check the LineJoin value to assure that it is Bevel.
            _class_testresult &= _helper.CompareProp("Bevel - LineJoin", (int)PenLineJoin.Bevel, (int)pen2.LineJoin);
            #endregion

            #region Test #3 - Round
            // Usage: PenLineJoin.Round
            // Notes: Value = int32(0x00000002)  
            CommonLib.LogStatus("Test #3 - Round");

            // Set the LineJoin of a Pen to Round
            Pen pen3 = pen.Clone();
            pen3.LineJoin = PenLineJoin.Round;

            // Draw a Geometry with the Pen
            PathGeometry path3 = path.Clone();
            path3.Transform = new TranslateTransform(43, 76);
            DC.DrawGeometry(null, pen3, path3);

            // Check the LineJoin value to assure that it is Round.
            _class_testresult &= _helper.CompareProp("Round - LineJoin", (int)PenLineJoin.Round, (int)pen3.LineJoin);
            #endregion
            #endregion End Of SECTION I

            #region TEST LOGGING           
            CommonLib.LogTest("Result for: "+_objectType);
            #endregion End of TEST LOGGING
        }

        private bool _class_testresult;
        private Type _objectType;
        private HelperClass _helper;
    }
}
