// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test edge cases for Gradient Brushes (i.e. coincident stops, stops > 0)
//
using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class WCP_GradientEdgeCases : ApiTest
    {
        //--------------------------------------------------------------------

        public WCP_GradientEdgeCases( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _objectType = typeof(System.Windows.Media.GradientStopCollection);
            _helper = new HelperClass();
            Update();
        }

        //--------------------------------------------------------------------

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus(_objectType.ToString());

            // Default LinearGradientBrush for this test will have a perfectly horizontal vector.
            Point SPoint = new Point(0.0, 0.5);
            Point EPoint = new Point(1.0, 0.5);

            // Classic valid GradientStopCollection
            GradientStopCollection GSC = new GradientStopCollection();
            GSC.Add(new GradientStop(Colors.Red, 0.0));
            GSC.Add(new GradientStop(Colors.Blue, 1.0));

            // Default LinearGradientBrush
            LinearGradientBrush LGB = new LinearGradientBrush(GSC, SPoint, EPoint);

            // Pen for border use           
            Pen pen = new Pen(Brushes.White, 2.0);

            // Points for coincident point tests
            Point zeroPoint = new Point(0.0, 0.0);
            Point onePoint = new Point(1.0, 1.0);
            Point halfPoint = new Point(0.5, 0.5);

            // Line 1
            Rect rect1 = new Rect(new Point(5, 5), new Point(260, 25));

            // Line 2
            Rect rect2b = new Rect(new Point(132, 35), new Point(260, 55));

            // Line 3
            Rect rect2 = new Rect(new Point(5, 60), new Point(260, 80));

            // Line 4
            Rect rect3b = new Rect(new Point(5, 90), new Point(133, 110));

            // Line 5
            Rect rect3 = new Rect(new Point(5, 115), new Point(260, 135));

            // Line 6
            Rect rect4 = new Rect(new Point(5, 145), new Point(55, 165));
            Rect rect5 = new Rect(new Point(60, 145), new Point(110, 165));
            Rect rect6 = new Rect(new Point(115, 145), new Point(165, 165));
            Rect rect7 = new Rect(new Point(170, 145), new Point(220, 165));
            Rect rect8 = new Rect(new Point(225, 145), new Point(275, 165));

            // Line 7
            Rect rect9 = new Rect(new Point(5, 175), new Point(260, 195));

            // Line 8
            Rect rect10 = new Rect(new Point(5, 205), new Point(260, 225));

            // Line 9
            Rect rect11 = new Rect(new Point(5, 235), new Point(260, 255));

            // Line 10
            Rect rect12 = new Rect(new Point(5, 265), new Point(55, 285));
            Rect rect13 = new Rect(new Point(60, 265), new Point(110, 285));
            Rect rect14 = new Rect(new Point(115, 265), new Point(165, 285));
            Rect rect15 = new Rect(new Point(170, 265), new Point(220, 285));
            Rect rect16 = new Rect(new Point(225, 265), new Point(275, 285));
            Rect rect17 = new Rect(new Point(280, 265), new Point(330, 285));

            // Line 11
            Rect rect18 = new Rect(new Point(5, 295), new Point(55, 315));
            Rect rect19 = new Rect(new Point(60, 295), new Point(110, 315));
            Rect rect20 = new Rect(new Point(115, 295), new Point(165, 315));
            Rect rect21 = new Rect(new Point(170, 295), new Point(220, 315));
            Rect rect22 = new Rect(new Point(225, 295), new Point(275, 315));

            // Line 12
            Rect rect23 = new Rect(new Point(5, 325), new Point(55, 345));
            Rect rect24 = new Rect(new Point(60, 325), new Point(110, 345));
            Rect rect25 = new Rect(new Point(115, 325), new Point(165, 345));
            Rect rect26 = new Rect(new Point(170, 325), new Point(220, 345));

            // Line 13
            Rect rect27 = new Rect(new Point(5, 355), new Point(55, 375));
            Rect rect28 = new Rect(new Point(60, 355), new Point(110, 375));
            Rect rect29 = new Rect(new Point(115, 355), new Point(165, 375));
            Rect rect30 = new Rect(new Point(170, 355), new Point(220, 375));


            #region Test #1 - No GradientStops
            // Description: LinearGradientBrush with no GradientStops
            // Expected: The Brush should render Transparent (ARGB: 0,0,0,0)
            CommonLib.LogStatus("Test #1 - No GradientStops");
            CommonLib.LogStatus("Line 1 - Rectangle should be filled with Transparent color");

            LinearGradientBrush LGB1 = new LinearGradientBrush();

            // Fill a rectangle with the brush
            DC.DrawRectangle(LGB1, pen, rect1);
            #endregion

            #region Test #2 - GradientStops at Offset < 0
            // Description: LinearGradientBrush with two GradientStops that have offsets < 0
            // Expected: The Color at 0.0 should be an interpolation between the Color < and nearest to 0.0 and 1.
            // This means that the stop at -0.8 is not taken into account.
            CommonLib.LogStatus("Test #2 - GradientStops at Offset < 0");
            CommonLib.LogStatus("Compare Lines 2 and 3: Gradient in line 2 should match the portion of Gradient in line 3 that is directly below it.");

            GradientStopCollection GSC2 = new GradientStopCollection();
            GSC2.Add(new GradientStop(Colors.Green, -1.8));
            GSC2.Add(new GradientStop(Colors.Red, -1.0));
            GSC2.Add(new GradientStop(Colors.Blue, 1.0));

            LinearGradientBrush LGB2 = new LinearGradientBrush(GSC2, SPoint, EPoint);

            // Fill a Rectangle with a classic, valid LinearGradientBrush for reference.
            LinearGradientBrush LGB2r = LGB.Clone();
            DC.DrawRectangle(LGB2r, null, rect2);

            // Fill a Rectangle with the LinearGradientBrush that has the < 0 offsets.
            LGB2.GradientStops = GSC2;
            DC.DrawRectangle(LGB2, null, rect2b);
            #endregion

            #region Test #3 - GradientStops at Offset > 1.0
            // Description: LinearGradientBrush with two GradientStops that have offsets > 1.0
            // Expected: The Color at 0.0 should be an interpolation between the Color < and nearest to 0.0 and 1.
            // This means that the stop at -0.8 is not taken into account.
            CommonLib.LogStatus("Test #3 - GradientStops at Offset > 1.0");
            CommonLib.LogStatus("Compare Lines 4 and 5: Gradient in line 4 should match the portion of Gradient in line 5 that is directly below it.");

            GradientStopCollection GSC3 = new GradientStopCollection();
            GSC3.Add(new GradientStop(Colors.Red, 0.0));
            GSC3.Add(new GradientStop(Colors.Blue, 2.0));
            GSC3.Add(new GradientStop(Colors.Green, 2.5));

            LinearGradientBrush LGB3 = new LinearGradientBrush(GSC3, SPoint, EPoint);

            // Fill a Rectangle with a classic, valid LinearGradientBrush for reference.
            LinearGradientBrush LGB3r = LGB.Clone();
            DC.DrawRectangle(LGB3r, null, rect3);

            // Fill a Rectangle with the LinearGradientBrush that has the > 1 offsets.
            LGB3.GradientStops = GSC3;
            DC.DrawRectangle(LGB3, null, rect3b);
            #endregion

            #region Test #4 - One Gradient Stop (< 0)
            // Description: LinearGradientBrush with one GradientStop < 0
            // Expected: The entire area should be filled with the solid color in the one stop.
            CommonLib.LogStatus("Test #4 - One Gradient Stop (< 0)");
            CommonLib.LogStatus("Line 6 Rectangle 1: Should be solid Red.");

            GradientStopCollection GSC4 = new GradientStopCollection();
            GSC4.Add(new GradientStop(Colors.Red, -1.0));

            LinearGradientBrush LGB4 = new LinearGradientBrush(GSC4, SPoint, EPoint);
            DC.DrawRectangle(LGB4, null, rect4);
            #endregion

            #region Test #5 - One Gradient Stop (0)
            // Description: LinearGradientBrush with one GradientStop at 0
            // Expected: The entire area should be filled with the solid color in the one stop.
            CommonLib.LogStatus("Test #5 - One Gradient Stop (0)");
            CommonLib.LogStatus("Line 6 Rectangle 2: Should be solid Red.");

            GradientStopCollection GSC5 = new GradientStopCollection();
            GSC5.Add(new GradientStop(Colors.Red, 0.0));

            LinearGradientBrush LGB5 = new LinearGradientBrush(GSC5, SPoint, EPoint);
            DC.DrawRectangle(LGB5, null, rect5);
            #endregion

            #region Test #6 - One Gradient Stop (0.5)
            // Description: LinearGradientBrush with one GradientStop at 0.5
            // Expected: The entire area should be filled with the solid color in the one stop.
            CommonLib.LogStatus("Test #6 - One Gradient Stop (0.5)");
            CommonLib.LogStatus("Line 6 Rectangle 3: Should be solid Red.");

            GradientStopCollection GSC6 = new GradientStopCollection();
            GSC6.Add(new GradientStop(Colors.Red, 0.5));

            LinearGradientBrush LGB6 = new LinearGradientBrush(GSC6, SPoint, EPoint);
            DC.DrawRectangle(LGB6, null, rect6);
            #endregion

            #region Test #7 - One Gradient Stop (1.0)
            // Description: LinearGradientBrush with one GradientStop at 1.0
            // Expected: The entire area should be filled with the solid color in the one stop.
            CommonLib.LogStatus("Test #7 - One Gradient Stop (1.0)");
            CommonLib.LogStatus("Line 6 Rectangle 4: Should be solid Red.");

            GradientStopCollection GSC7 = new GradientStopCollection();
            GSC7.Add(new GradientStop(Colors.Red, 1.0));

            LinearGradientBrush LGB7 = new LinearGradientBrush(GSC7, SPoint, EPoint);
            DC.DrawRectangle(LGB7, null, rect7);
            #endregion

            #region Test #8 - One Gradient Stop (> 1.0)
            // Description: LinearGradientBrush with one GradientStop > 1.0
            // Expected: The entire area should be filled with the solid color in the one stop.
            CommonLib.LogStatus("Test #8 - One Gradient Stop (> 1.0)");
            CommonLib.LogStatus("Line 6 Rectangle 5: Should be solid Red.");

            GradientStopCollection GSC8 = new GradientStopCollection();
            GSC8.Add(new GradientStop(Colors.Red, 2.0));

            LinearGradientBrush LGB8 = new LinearGradientBrush(GSC8, SPoint, EPoint);
            DC.DrawRectangle(LGB8, null, rect8);
            #endregion

            #region Test #9 - Valid case
            // Description: LinearGradientBrush with stops at 0 and 1
            // Expected: Typical valid Gradient with color interpolated between the colors at 0 and 1.
            CommonLib.LogStatus("Test #9 - Valid case");
            CommonLib.LogStatus("Line 7: Rectangle should contain Red to Blue Gradient.");

            DC.DrawRectangle(LGB, null, rect9);
            #endregion

            #region Test #10 - No Stop at 0 (0.5, 1.0) only
            // Description: LinearGradientBrush with no GradientStop at 0
            // Expected: (0.00, 0.50) is solid 1st color.  Interpolation between colors over (0.50, 1.00).					
            CommonLib.LogStatus("Test #10 - No Stop at 0 (0.5, 1.0) only");
            CommonLib.LogStatus("Line 8: Rectangle should contain 0.0->0.5 solid Red and 0.5->1.0 Red to Blue Gradient.");

            GradientStopCollection GSC10 = new GradientStopCollection();
            GSC10.Add(new GradientStop(Colors.Red, 0.5));
            GSC10.Add(new GradientStop(Colors.Blue, 1.0));

            LinearGradientBrush LGB10 = new LinearGradientBrush(GSC10, SPoint, EPoint);
            DC.DrawRectangle(LGB10, null, rect10);
            #endregion

            #region Test #11 - No Stop at 1.0 (0, 0.5) only
            // Description: LinearGradientBrush with no GradientStop at 1.0
            // Expected: Interpolation between colors over (0.00, 0.50).  (0.50, 1.00) is solid 2nd color.	
            CommonLib.LogStatus("Test #11 - No Stop at 1.0 (0, 0.5) only");
            CommonLib.LogStatus("Line 9: Rectangle should contain 0.0->0.5 Red to Blue Gradient and 0.5->1.0 solid Blue.");

            GradientStopCollection GSC11 = new GradientStopCollection();
            GSC11.Add(new GradientStop(Colors.Red, 0.0));
            GSC11.Add(new GradientStop(Colors.Blue, 0.5));

            LinearGradientBrush LGB11 = new LinearGradientBrush(GSC11, SPoint, EPoint);
            DC.DrawRectangle(LGB11, null, rect11);
            #endregion

            #region Test #12 - Two Coincident Stops of the same color at (0.0, 0.0)
            // Description: LinearGradientBrush with Two Coincident Stops of the same color at (0.0, 0.0)
            // Expected: The Brush contains the solid color in the last Stop.
            CommonLib.LogStatus("Test #12 - Two Coincident Stops of the same color at (0.0, 0.0)");
            CommonLib.LogStatus("Line 10 Rectangle 1: Should be solid Red");

            GradientStopCollection GSC12 = new GradientStopCollection();
            GSC12.Add(new GradientStop(Colors.Red, 0.0));
            GSC12.Add(new GradientStop(Colors.Red, 0.0));

            LinearGradientBrush LGB12 = new LinearGradientBrush(GSC12, SPoint, EPoint);
            DC.DrawRectangle(LGB12, null, rect12);
            #endregion

            #region Test #13 - Two Coincident Stops of the same color at (0.5, 0.5)
            // Description: LinearGradientBrush with Two Coincident Stops of the same color at (0.5, 0.5)
            // Expected: The Brush contains the solid color in the last Stop.
            CommonLib.LogStatus("Test #13 - Two Coincident Stops of the same color at (0.5, 0.5)");
            CommonLib.LogStatus("Line 10 Rectangle 2: Should be solid Red");

            GradientStopCollection GSC13 = new GradientStopCollection();
            GSC13.Add(new GradientStop(Colors.Red, 0.5));
            GSC13.Add(new GradientStop(Colors.Red, 0.5));

            LinearGradientBrush LGB13 = new LinearGradientBrush(GSC13, SPoint, EPoint);
            DC.DrawRectangle(LGB13, null, rect13);
            #endregion

            #region Test #14 - Two Coincident Stops of the same color at (1.0, 1.0)
            // Description: LinearGradientBrush with Two Coincident Stops of the same color at (1.0, 1.0)
            // Expected: The Brush contains the solid color in the last Stop.
            CommonLib.LogStatus("Test #14 - Two Coincident Stops of the same color at (1.0, 1.0)");
            CommonLib.LogStatus("Line 10 Rectangle 3: Should be solid Red");

            GradientStopCollection GSC14 = new GradientStopCollection();
            GSC14.Add(new GradientStop(Colors.Red, 1.0));
            GSC14.Add(new GradientStop(Colors.Red, 1.0));

            LinearGradientBrush LGB14 = new LinearGradientBrush(GSC14, SPoint, EPoint);
            DC.DrawRectangle(LGB14, null, rect14);
            #endregion

            #region Test #15 - Two Coincident Stops of different colors at (0.0, 0.0)
            // Description: LinearGradientBrush with Two Coincident Stops different colors at (0.0, 0.0)
            // Expected: The Brush contains a solid color, specified by the last (right-most) color value.
            CommonLib.LogStatus("Test #15 - Two Coincident Stops of different colors at (0.0, 0.0)");
            CommonLib.LogStatus("Line 10 Rectangle 4: Should be solid Blue");

            GradientStopCollection GSC15 = new GradientStopCollection();
            GSC15.Add(new GradientStop(Colors.Red, 0.0));
            GSC15.Add(new GradientStop(Colors.Blue, 0.0));

            LinearGradientBrush LGB15 = new LinearGradientBrush(GSC15, SPoint, EPoint);
            DC.DrawRectangle(LGB15, null, rect15);
            #endregion

            #region Test #16 - Two Coincident Stops of different colors at (0.5, 0.5)
            // Description: LinearGradientBrush with Two Coincident Stops of different colors at (0.5, 0.5)
            // Expected: (0.00, 0.50) is solid 1st color, (0.50, 1.0) is solid 2nd color
            CommonLib.LogStatus("Test #16 - Two Coincident Stops of different colors at (0.5, 0.5)");
            CommonLib.LogStatus("Line 10 Rectangle 5: 0.0->0.5 Should be solid Red and 0.5->1.0 Should be solid Blue");

            GradientStopCollection GSC16 = new GradientStopCollection();
            GSC16.Add(new GradientStop(Colors.Red, 0.5));
            GSC16.Add(new GradientStop(Colors.Blue, 0.5));

            LinearGradientBrush LGB16 = new LinearGradientBrush(GSC16, SPoint, EPoint);
            DC.DrawRectangle(LGB16, null, rect16);
            #endregion

            #region Test #17 - Two Coincident Stops of different colors at (1.0, 1.0)
            // Description: LinearGradientBrush with Two Coincident Stops of different colors at (1.0, 1.0)
            // Expected: The Brush contains a solid color, specified by the first (left-most) color value
            CommonLib.LogStatus("Test #17 - Two Coincident Stops of different colors at (1.0, 1.0)");
            CommonLib.LogStatus("Line 10 Rectangle 6: Should be solid Red");

            GradientStopCollection GSC17 = new GradientStopCollection();
            GSC17.Add(new GradientStop(Colors.Red, 1.0));
            GSC17.Add(new GradientStop(Colors.Blue, 1.0));

            LinearGradientBrush LGB17 = new LinearGradientBrush(GSC17, SPoint, EPoint);
            DC.DrawRectangle(LGB17, null, rect17);
            #endregion

            #region Test #18 - Three Stops with Two Coincident Stops of different colors at (0.0, 0.5, 0.5)
            // Description: LinearGradientBrush that contains Three Stops with Two Coincident Stops of different colors at (0.0, 0.5, 0.5).
            // Expected: Interpolation between first 2 colors over (0.00, 0.50).  (0.50, 1.00) is solid 3rd color.
            CommonLib.LogStatus("Test #18 - Three Stops with Two Coincident Stops of different colors at (0.0, 0.5, 0.5)");
            CommonLib.LogStatus("Line 11 Rectangle 1: 0.0->0.5 Should be Red to Green Gradient and 0.5->1.0 Should be solid Blue");

            GradientStopCollection GSC18 = new GradientStopCollection();
            GSC18.Add(new GradientStop(Colors.Red, 0.0));
            GSC18.Add(new GradientStop(Colors.Green, 0.5));
            GSC18.Add(new GradientStop(Colors.Blue, 0.5));

            LinearGradientBrush LGB18 = new LinearGradientBrush(GSC18, SPoint, EPoint);
            DC.DrawRectangle(LGB18, null, rect18);
            #endregion

            #region Test #19 - Three Coincident Stops of different colors at (0.0, 0.0, 0.0)
            // Description: LinearGradientBrush that contains Three Coincident Stops of different colors at (0.0, 0.0, 0.0)
            // Expected: The Brush contains a solid color.  The last (right-most) color value is used, other stops are ignored.
            CommonLib.LogStatus("Test #19 - Three Coincident Stops of different colors at (0.0, 0.0, 0.0)");
            CommonLib.LogStatus("Line 11 Rectangle 2: Should be solid Blue");

            GradientStopCollection GSC19 = new GradientStopCollection();
            GSC19.Add(new GradientStop(Colors.Red, 0.0));
            GSC19.Add(new GradientStop(Colors.Green, 0.0));
            GSC19.Add(new GradientStop(Colors.Blue, 0.0));

            LinearGradientBrush LGB19 = new LinearGradientBrush(GSC19, SPoint, EPoint);
            DC.DrawRectangle(LGB19, null, rect19);
            #endregion

            #region Test #20 - Three Coincident Stops of different colors at (0.5, 0.5, 0.5)
            // Description: LinearGradientBrush that contains Three Coincident Stops of different colors at (0.5, 0.5, 0.5)
            // Expected: (0.00, 0.50) is solid 1st color, (0.50, 1.0) is solid 3rd color.  The Middle color is ignored.
            CommonLib.LogStatus("Test #20 - Three Coincident Stops of different colors at (0.5, 0.5, 0.5)");
            CommonLib.LogStatus("Line 11 Rectangle 3: 0.0->0.5 Should be solid Red and 0.5->1.0 Should be solid Blue");

            GradientStopCollection GSC20 = new GradientStopCollection();
            GSC20.Add(new GradientStop(Colors.Red, 0.5));
            GSC20.Add(new GradientStop(Colors.Green, 0.5));
            GSC20.Add(new GradientStop(Colors.Blue, 0.5));

            LinearGradientBrush LGB20 = new LinearGradientBrush(GSC20, SPoint, EPoint);
            DC.DrawRectangle(LGB20, null, rect20);
            #endregion

            #region Test #21 - Three Coincident Stops of different colors at (1.0, 1.0, 1.0)
            // Description: LinearGradientBrush that contains Three Coincident Stops of different colors at (1.0, 1.0, 1.0)
            // Expected: The Brush contains a solid color.  The first (left-most) color value is used, the other stops are ignored.
            CommonLib.LogStatus("Test #21 - Three Coincident Stops of different colors at (1.0, 1.0, 1.0)");
            CommonLib.LogStatus("Line 11 Rectangle 4: Should be solid Red");

            GradientStopCollection GSC21 = new GradientStopCollection();
            GSC21.Add(new GradientStop(Colors.Red, 1.0));
            GSC21.Add(new GradientStop(Colors.Green, 1.0));
            GSC21.Add(new GradientStop(Colors.Blue, 1.0));

            LinearGradientBrush LGB21 = new LinearGradientBrush(GSC21, SPoint, EPoint);
            DC.DrawRectangle(LGB21, null, rect21);
            #endregion

            #region Test #22 - Four Stops with three Coincident Stops of different colors at (0.0, 0.5, 0.5, 0.5)
            // Description: LinearGradientBrush with Four Stops with three Coincident Stops of different colors at (0.0, 0.5, 0.5, 0.5)
            // Expected: Interpolation between the first 2 colors over (0.00, 0.50).  (0.50, 1.00) is the solid 4th color.  The 3rd color is ignored.
            CommonLib.LogStatus("Test #22 - Four Stops with three Coincident Stops of different colors at (0.0, 0.5, 0.5, 0.5)");
            CommonLib.LogStatus("Line 11 Rectangle 4: 0.0->0.5 Should be a Red to Green Gradient and 0.5->1.0 Should be solid Blue");

            GradientStopCollection GSC22 = new GradientStopCollection();
            GSC22.Add(new GradientStop(Colors.Red, 0.0));
            GSC22.Add(new GradientStop(Colors.Green, 0.5));
            GSC22.Add(new GradientStop(Colors.Yellow, 0.5));
            GSC22.Add(new GradientStop(Colors.White, 0.5));
            GSC22.Add(new GradientStop(Colors.Blue, 0.5));

            LinearGradientBrush LGB22 = new LinearGradientBrush(GSC22, SPoint, EPoint);
            DC.DrawRectangle(LGB22, null, rect22);
            #endregion

            #region Test #23 - Gradient Stops < 0, and one GradientStop at 0
            // Description: LinearGradientBrush with two Gradient Stops < 0, and one GradientStop at 0
            // Expected: The Brush is filled with the solid color in the 0 Stop.  Stops < 0.0 are ignored.
            CommonLib.LogStatus("Test #23 - Gradient Stops < 0, and one GradientStop at 0");
            CommonLib.LogStatus("Line 12 Rectangle 1: Should be solid Blue");

            GradientStopCollection GSC23 = new GradientStopCollection();
            GSC23.Add(new GradientStop(Colors.Green, -1.8));
            GSC23.Add(new GradientStop(Colors.Red, -1.0));
            GSC23.Add(new GradientStop(Colors.Blue, 0.0));

            LinearGradientBrush LGB23 = new LinearGradientBrush(GSC23, SPoint, EPoint);
            DC.DrawRectangle(LGB23, null, rect23);
            #endregion

            #region Test #24 - Gradient Stops > 1.0, and one GradientStop at 1.0
            // Description: LinearGradientBrush with two Gradient Stops > 1.0, and one GradientStop at 1.0
            // Expected: The Brush is filled with the solid color in the 1.0 Stop.  Stops > 1.0 are ignored.
            CommonLib.LogStatus("Test #24 - Gradient Stops > 1.0, and one GradientStop at 1.0");
            CommonLib.LogStatus("Line 12 Rectangle 2: Should be solid Red");

            GradientStopCollection GSC24 = new GradientStopCollection();
            GSC24.Add(new GradientStop(Colors.Red, 1.0));
            GSC24.Add(new GradientStop(Colors.Green, 1.5));
            GSC24.Add(new GradientStop(Colors.Blue, 2.0));

            LinearGradientBrush LGB24 = new LinearGradientBrush(GSC24, SPoint, EPoint);
            DC.DrawRectangle(LGB24, null, rect24);
            #endregion

            #region Test #25 - All Gradient Stops < 0
            // Description: LinearGradientBrush with all Gradient Stops < 0
            // Expected: The Brush is filled with a solid color specified by the stop closest to 0.0.
            CommonLib.LogStatus("Test #25 - All Gradient Stops < 0");
            CommonLib.LogStatus("Line 12 Rectangle 3: Should be solid Blue");

            GradientStopCollection GSC25 = new GradientStopCollection();
            GSC25.Add(new GradientStop(Colors.Green, -1.8));
            GSC25.Add(new GradientStop(Colors.Red, -1.0));
            GSC25.Add(new GradientStop(Colors.Blue, -0.5));

            LinearGradientBrush LGB25 = new LinearGradientBrush(GSC25, SPoint, EPoint);
            DC.DrawRectangle(LGB25, null, rect25);
            #endregion

            #region Test #26 - All Gradient Stops > 1.0
            // Description: LinearGradientBrush with all Gradient Stops > 1.0
            // Expected: The Brush is filled with a solid color specified by the stop closest to 1.0
            CommonLib.LogStatus("Test #26 - All Gradient Stops > 1.0");
            CommonLib.LogStatus("Line 12 Rectangle 4: Should be solid Red");

            GradientStopCollection GSC26 = new GradientStopCollection();
            GSC26.Add(new GradientStop(Colors.Red, 1.1));
            GSC26.Add(new GradientStop(Colors.Green, 1.8));
            GSC26.Add(new GradientStop(Colors.Blue, 2.7));

            LinearGradientBrush LGB26 = new LinearGradientBrush(GSC26, SPoint, EPoint);
            DC.DrawRectangle(LGB26, null, rect26);
            #endregion

            #region Test #27 - Coincident Gradient Line Points (Pad)
            // Description: LinearGradientBrush with Start/End Points at (0.0, 0.0) and SpreadMethod = Pad
            // Expected: The Brush is filled with a Solid color set to the end pad color (stop with the largest position)
            CommonLib.LogStatus("Test #27 - Coincident Gradient Line Points (Pad)");
            CommonLib.LogStatus("Line 13 Rectangle 1: Should be solid Blue");

            LinearGradientBrush LGB27 = LGB.Clone();

            LGB27.StartPoint = zeroPoint;
            LGB27.EndPoint = zeroPoint;
            LGB27.SpreadMethod = GradientSpreadMethod.Pad;

            DC.DrawRectangle(LGB27, null, rect27);
            #endregion

            #region Test #28 - Coincident Gradient Line Points (Reflect)
            // Description: LinearGradientBrush with Start/End Points at (1.0, 1.0) and SpreadMethod = Reflect
            // Expected: Solid color determined by summing the weighted contribution of each GradientStop pair
            CommonLib.LogStatus("Test #28 - Coincident Gradient Line Points (Reflect)");
            CommonLib.LogStatus("Line 13 Rectangle 2: Should be solid Purple (128, 0, 128)");

            LinearGradientBrush LGB28 = LGB.Clone();

            LGB28.StartPoint = onePoint;
            LGB28.EndPoint = onePoint;
            LGB28.SpreadMethod = GradientSpreadMethod.Reflect;

            DC.DrawRectangle(LGB28, null, rect28);
            #endregion

            #region Test #29 - Coincident Gradient Line Points (Repeat)
            // Description: LinearGradientBrush with Start/End Points at (0.5, 0.5) and SpreadMethod = Repeat
            // Expected: Solid color determined by summing the weighted contribution of each GradientStop pair
            CommonLib.LogStatus("Test #29 - Coincident Gradient Line Points (Repeat)");
            CommonLib.LogStatus("Line 13 Rectangle 3: Should be solid Purple (128, 0, 128)");

            LinearGradientBrush LGB29 = LGB.Clone();

            LGB29.StartPoint = halfPoint;
            LGB29.EndPoint = halfPoint;
            LGB29.SpreadMethod = GradientSpreadMethod.Repeat;

            DC.DrawRectangle(LGB29, null, rect29);
            #endregion

            #region Test #30 - Coincident Stops that are very close to another Stop of the same Color
            // Description: Regression for Regression_Bug230 - Coincedent stops that are very close to another stop of
            // the same Color should be ignored and the last stop should be used.
            // Expected: A Red to Blue Gradient
            CommonLib.LogStatus("Test #30 - Coincident Stops that are very close to another Stop of the same Color");
            CommonLib.LogStatus("Line 13 Rectangle 4: Should be a Red to Blue Gradient");

            GradientStopCollection GSC30 = new GradientStopCollection();
            GSC30.Add(new GradientStop(Colors.Red, 0.0));
            GSC30.Add(new GradientStop(Colors.Blue, 0.986679554));
            GSC30.Add(new GradientStop(Colors.Blue, 0.9866800904));
            GSC30.Add(new GradientStop(Colors.Blue, 0.986681));
            GSC30.Add(new GradientStop(Colors.Blue, 1.0));

            LinearGradientBrush LGB30 = new LinearGradientBrush(GSC30, SPoint, EPoint);
            DC.DrawRectangle(LGB30, null, rect30);
            #endregion
            
            CommonLib.LogTest("Result for:"+ _objectType );
            
        }

        //--------------------------------------------------------------------

        private Type _objectType;
        private HelperClass _helper;
    }
}
