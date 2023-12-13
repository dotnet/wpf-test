// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test all public API's in the DashStyles enum
//
using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class WCP_DashStylesClass : ApiTest
    {

        public WCP_DashStylesClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(System.Windows.Media.DashStyles);
            _helper = new HelperClass();
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus(_objectType.ToString());

            // Create a Pen
            Pen pen = new Pen(Brushes.Red, 5.0);
            Point P1 = new Point(20, 20);
            Point P2 = new Point(170, 20);
            Vector V1 = new Vector(0, 30);

            #region SECTION I - ENUMERATIONS
            CommonLib.LogStatus("***** SECTION I - ENUMERATIONS *****");

            #region Test #1 - Dash
            // Usage: DoubleCollection = DashStyles.Dash
            // Notes: Returns a DoubleCollection that describes a Dash pattern for use with Pen.
            CommonLib.LogStatus("Test #1 - Dash");

            // Set the DashStyle of a Pen to Dash
            pen.DashStyle = DashStyles.Dash;

            // Draw a line with the Pen
            DC.DrawLine(pen, P1, P2);

            // Check the DashStyle value to assure that it is Dash.
            _class_testresult &= _helper.CompareProp("Dash - Dashes", pen.DashStyle.Dashes, DashStyles.Dash.Dashes);
            _class_testresult &= _helper.CompareProp("Dash - Offset", pen.DashStyle.Offset, DashStyles.Dash.Offset);
            #endregion

            #region Test #2 - DashDot
            // Usage: DoubleCollection = DashStyles.DashDot
            // Notes: Returns a DoubleCollection that describes a Dash Dot pattern for use with Pen.
            CommonLib.LogStatus("Test #2 - DashDot");

            // Set the DashStyle of a Pen to DashDot
            Pen pen2 = pen.Clone();
            pen2.DashStyle = DashStyles.DashDot;

            // Draw a line with the Pen
            DC.DrawLine(pen2, P1 += V1, P2 += V1);

            // Check the DashStyle value to assure that it is DashDot.
            _class_testresult &= _helper.CompareProp("DashDot - Dashes", pen2.DashStyle.Dashes, DashStyles.DashDot.Dashes);
            _class_testresult &= _helper.CompareProp("DashDot - Offset", pen2.DashStyle.Offset, DashStyles.DashDot.Offset);
            #endregion

            #region Test #3 - DashDotDot
            // Usage: DoubleCollection = DashStyles.DashDotDot
            // Notes: Returns a DoubleCollection that describes a Dash Dot Dot pattern for use with Pen.
            CommonLib.LogStatus("Test #3 - DashDotDot");

            // Set the DashStyle of a Pen to DashDotDot
            Pen pen3 = pen.Clone();
            pen3.DashStyle = DashStyles.DashDotDot;

            // Draw a line with the Pen
            DC.DrawLine(pen3, P1 += V1, P2 += V1);

            // Check the DashStyle value to assure that it is DashDotDot.
            _class_testresult &= _helper.CompareProp("DashDotDot - Dashes", pen3.DashStyle.Dashes, DashStyles.DashDotDot.Dashes);
            _class_testresult &= _helper.CompareProp("DashDotDot - Offset", pen3.DashStyle.Offset, DashStyles.DashDotDot.Offset);
            #endregion

            #region Test #4 - Dot
            // Usage: DoubleCollection = DashStyles.Dot
            // Notes: Returns a DoubleCollection that describes a Dot pattern for use with Pen.
            CommonLib.LogStatus("Test #4 - Dot");

            // Set the DashStyle of a Pen to Dot
            Pen pen4 = pen.Clone();
            pen4.DashStyle = DashStyles.Dot;

            // Draw a line with the Pen
            DC.DrawLine(pen4, P1 += V1, P2 += V1);

            // Check the DashStyle value to assure that it is Dot.
            _class_testresult &= _helper.CompareProp("Dot - Dashes", pen4.DashStyle.Dashes, DashStyles.Dot.Dashes);
            _class_testresult &= _helper.CompareProp("Dot - Offset", pen4.DashStyle.Offset, DashStyles.Dot.Offset);
            #endregion

            #region Test #5 - Solid
            // Usage: DoubleCollection = DashStyles.Solid
            // Notes: Returns a DoubleCollection that describes a Solid pattern for use with Pen.
            CommonLib.LogStatus("Test #5 - Solid");

            // Set the DashStyle of a Pen to Solid
            Pen pen5 = pen.Clone();
            pen5.DashStyle = DashStyles.Solid;

            // Draw a line with the Pen
            DC.DrawLine(pen5, P1 += V1, P2 += V1);

            // Check the DashStyle value to assure that it is Solid.
            _class_testresult &= _helper.CompareProp("Solid - Dashes", pen5.DashStyle.Dashes, DashStyles.Solid.Dashes);
            _class_testresult &= _helper.CompareProp("Solid - Offset", pen5.DashStyle.Offset, DashStyles.Solid.Offset);
            #endregion

            #endregion End Of SECTION I

            #region TEST LOGGING
            // Log the programmatic result for this API test using the
            // Automation Framework LogTest method.  If This result is False,
            // it will override the result of a Visual Comparator.  Conversely,
            // if a Visual Comparator is False it will override a True result
            // from this test.
            CommonLib.LogStatus("Logging the Test Result");
            CommonLib.LogTest("Result for: "+_objectType );
            #endregion End of TEST LOGGING
        }

        private bool _class_testresult;
        private Type _objectType;
        private HelperClass _helper;
    }
}
