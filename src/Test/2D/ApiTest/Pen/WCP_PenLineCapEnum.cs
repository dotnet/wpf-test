// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test all public API's in the PenLineCap enum
//
using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class WCP_PenLineCapEnum : ApiTest
    {

        public WCP_PenLineCapEnum( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(System.Windows.Media.PenLineCap);
            _helper = new HelperClass();
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus(_objectType.ToString());

            // Create a Pen
            Pen pen = new Pen(Brushes.Red, 10.0);
            Point P1 = new Point(20, 30);
            Point P2 = new Point(170, 30);
            Vector V1 = new Vector(0, 30);

            #region SECTION I - ENUMERATIONS
            CommonLib.LogStatus("***** SECTION I - ENUMERATIONS *****");

            #region Test #1 - Flat
            // Usage: PenLineCap.Flat
            // Notes: Value = int32(0x00000000)  
            CommonLib.LogStatus("Test #1 - Flat");

            // Set the Start and End LineCap of a Pen to Flat
            pen.StartLineCap = PenLineCap.Flat;
            pen.EndLineCap = PenLineCap.Flat;

            // Draw a Line with the Pen
            DC.DrawLine(pen, P1, P2);

            // Check both LineCap values to assure that they are Flat.
            _class_testresult &= _helper.CompareProp("Flat - StartLineCap", (int)PenLineCap.Flat, (int)pen.StartLineCap);
            _class_testresult &= _helper.CompareProp("Flat - EndLineCap", (int)PenLineCap.Flat, (int)pen.EndLineCap);
            #endregion

            #region Test #2 - Square
            // Usage: PenLineCap.Square
            // Notes: Value = int32(0x00000001)  
            CommonLib.LogStatus("Test #2 - Square");

            // Set the Start and End LineCap of a Pen to Square
            Pen pen2 = pen.Clone();
            pen2.StartLineCap = PenLineCap.Square;
            pen2.EndLineCap = PenLineCap.Square;

            // Draw a Line with the Pen
            DC.DrawLine(pen2, P1 += V1, P2 += V1);

            // Check both LineCap values to assure that they are Square.
            _class_testresult &= _helper.CompareProp("Square - StartLineCap", (int)PenLineCap.Square, (int)pen2.StartLineCap);
            _class_testresult &= _helper.CompareProp("Square - EndLineCap", (int)PenLineCap.Square, (int)pen2.EndLineCap);
            #endregion

            #region Test #3 - Round
            // Usage: PenLineCap.Round
            // Notes: Value = int32(0x00000002)  
            CommonLib.LogStatus("Test #3 - Round");

            // Set the Start and End LineCap of a Pen to Round
            Pen pen3 = pen.Clone();
            pen3.StartLineCap = PenLineCap.Round;
            pen3.EndLineCap = PenLineCap.Round;

            // Draw a Line with the Pen
            DC.DrawLine(pen3, P1 += V1, P2 += V1);

            // Check both LineCap values to assure that they are Round.
            _class_testresult &= _helper.CompareProp("Round - StartLineCap", (int)PenLineCap.Round, (int)pen3.StartLineCap);
            _class_testresult &= _helper.CompareProp("Round - EndLineCap", (int)PenLineCap.Round, (int)pen3.EndLineCap);
            #endregion

            #region Test #4 - Triangle
            // Usage: PenLineCap.Triangle
            // Notes: Value = int32(0x00000003)  
            CommonLib.LogStatus("Test #4 - Triangle");

            // Set the Start and End LineCap of a Pen to Triangle
            Pen pen4 = pen.Clone();
            pen4.StartLineCap = PenLineCap.Triangle;
            pen4.EndLineCap = PenLineCap.Triangle;

            // Draw a Line with the Pen
            DC.DrawLine(pen4, P1 += V1, P2 += V1);

            // Check both LineCap values to assure that they are Triangle.
            _class_testresult &= _helper.CompareProp("Triangle - StartLineCap", (int)PenLineCap.Triangle, (int)pen4.StartLineCap);
            _class_testresult &= _helper.CompareProp("Triangle - EndLineCap", (int)PenLineCap.Triangle, (int)pen4.EndLineCap);
            #endregion

            #endregion End Of SECTION I

            #region TEST LOGGING
            // Log the programmatic result for this API test using the
            // Automation Framework LogTest method.  If This result is False,
            // it will override the result of a Visual Comparator.  Conversely,
            // if a Visual Comparator is False it will override a True result
            // from this test.
            CommonLib.LogTest("Result for :" + _objectType);
            #endregion End of TEST LOGGING
        }

        private bool _class_testresult;
        private Type _objectType;
        private HelperClass _helper;
    }
}
