// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test all public API's in the Pen class
//
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class WCP_PenClass : ApiTest
    {
        public WCP_PenClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(Pen);
            _helper = new HelperClass();
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus(_objectType.ToString());

            // Animations to be used for testing Properties
            SolidColorBrush AnimBrush = new SolidColorBrush(Colors.Blue);
            ColorAnimation AnimColor = new ColorAnimation(Colors.Fuchsia, Colors.Yellow, new System.Windows.Duration(TimeSpan.FromMilliseconds(1000)));
            AnimBrush.BeginAnimation(SolidColorBrush.ColorProperty, AnimColor);
            DoubleAnimation AnimDouble01 = new DoubleAnimation(0, 1.0, new System.Windows.Duration(TimeSpan.FromMilliseconds(0)), FillBehavior.Stop);
            DoubleAnimation AnimDouble1020 = new DoubleAnimation(10.0, 20.0, new System.Windows.Duration(TimeSpan.FromMilliseconds(0)), FillBehavior.Stop);
            DoubleAnimation AnimDouble020 = new DoubleAnimation(0, 20.0, new System.Windows.Duration(TimeSpan.FromMilliseconds(1000)));

            #region SECTION I - CONSTRUCTORS
            CommonLib.LogStatus("***** SECTION I - CONSTRUCTORS *****");

            #region Test #1 - Default Constructor
            // Usage: Pen()
            // Notes: Default Constructor creates an empty Pen.
            CommonLib.LogStatus("Test #1 - Default Constructor");

            // Create a Pen 
            Pen P11 = new Pen();

            // Confirm that a Pen was created successfully.
            _class_testresult &= _helper.CheckType(P11, _objectType);
            #endregion

            #region Test #2 - Constructor with a Brush and a double
            // Usage: Pen(Brush, float64)
            // Notes: Create a Pen with a Brush type and Thickness.
            CommonLib.LogStatus("Test #2 - Constructor with a Brush and a double");

            // Create a Pen with a Color 
            Pen Pprop = new Pen(Brushes.Blue, 2.0);

            // Confirm that a Pen was created successfully.
            _class_testresult &= _helper.CheckType(Pprop, _objectType);
            #endregion
            #endregion End Of SECTION I

            #region SECTION II - PROPERTIES
            CommonLib.LogStatus("***** SECTION II - PROPERTIES *****");

            #region Test #1 - The Brush Property
            // Usage: Brush = Pen.Brush (R/W)
            // Notes: Gets or Sets the Brush that is contained in the Pen.
            CommonLib.LogStatus("Test #1 - The Brush Property");

            // Set the Brush property.
            Pprop.Brush = new SolidColorBrush(Colors.Yellow);

            // Get the Brush property.
            SolidColorBrush SCBrush = (SolidColorBrush)Pprop.Brush;

            // Check the Brush value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareColorPropFloats("Brush", SCBrush.Color, 1.0f, 1.0f, 1.0f, 0.0f);

            // Getting the Brush property when the Brush is null (not set) should
            // return null.
            if (P11.Brush == null)
            {
                CommonLib.LogStatus("Pass: Brush (null test) Brush = null");
            }
            else
            {
                CommonLib.LogFail("Fail: Brush (null test) Brush should equal null");
                _class_testresult &= false;
            }

            // Set the Brush property to an Animated Brush
            Pprop.Brush = AnimBrush;

            // Get the Brush property.
            SolidColorBrush SCAnimBrush = (SolidColorBrush)Pprop.Brush;

            // Animated values can't be checked reliably due to timing issues so just
            // ensure that the property can Set/Get an animated Brush.
            if (SCAnimBrush != null)
            {
                CommonLib.LogStatus("Pass: Brush (With Animation) Does not crash");
            }
            else
            {
                CommonLib.LogStatus("Fail: Brush (With Animation) is Null");
                _class_testresult &= false;
            }
            #endregion

            #region Test #2 - The DashStyle Property
            // Usage: DashStyle = Pen.DashStyle (R/W)
            // Notes: Gets or Sets a DashStyle that describes the dash pattern for the Pen.
            // The DashStyles Enum contains DashStyles for several commonly used dash patterns.
            CommonLib.LogStatus("Test #2 - The DashStyle Property");

            // Set the DashArray property.
            Pprop.DashStyle = DashStyles.Dash;

            // Get the DashStyle value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("DashStyle - Dashes", Pprop.DashStyle.Dashes, DashStyles.Dash.Dashes);
            _class_testresult &= _helper.CompareProp("DashStyle - Offset", Pprop.DashStyle.Offset, DashStyles.Dash.Offset);
            #endregion

            #region Test #3 - The DashCap Property
            // Usage: PenDashCap = Pen.LineCap (R/W)
            // Notes: Gets or Sets the PenDashCap enumeration for the Pen.
            CommonLib.LogStatus("Test #3 - The DashCap Property");

            // Set the DashCap property.
            Pprop.DashCap = PenLineCap.Triangle;

            // Get the DashCap value as an Int to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("DashCap", (int)Pprop.DashCap, (int)PenLineCap.Triangle);
            #endregion

            #region Test #4 - The EndLineCap Property
            // Usage: PenLineCap = Pen.EndLineCap (R/W)
            // Notes: Gets or Sets the PenLineCap enumeration for the end of the line. 
            CommonLib.LogStatus("Test #4 - The EndLineCap Property");

            // Set the EndLineCap property.
            Pprop.EndLineCap = PenLineCap.Round;

            // Get the EndLineCap value as an Int to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("EndLineCap", (int)Pprop.EndLineCap, (int)PenLineCap.Round);
            #endregion

            #region Test #5 - The LineJoin Property
            // Usage: PenLineJoin = Pen.LineJoin (R/W)
            // Notes: Gets or Sets the PenLineJoin enumeration for the Pen. 
            CommonLib.LogStatus("Test #5 - The LineJoin Property");

            // Set the LineJoin property.
            Pprop.LineJoin = PenLineJoin.Miter;

            // Get the LineJoin value as an Int to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("LineJoin", (int)Pprop.LineJoin, (int)PenLineJoin.Miter);
            #endregion

            #region Test #6 - The MiterLimit Property
            // Usage: float64 = Pen.MiterLimit (R/W)
            // Notes: Gets or Sets the MiterLimit of the Pen.
            CommonLib.LogStatus("Test #6 - The MiterLimit Property");

            // Set the MiterLimit property.
            Pprop.MiterLimit = 10.0;

            // Get the MiterLimit value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("MiterLimit", Pprop.MiterLimit, 10.0);

            // Add an Double Animation to the Pen
            Pprop.BeginAnimation(Pen.MiterLimitProperty, AnimDouble1020);

            // Get the MiterLimit value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("MiterLimit (With Animation)", Pprop.MiterLimit, 10.0);
            #endregion

            #region Test #7 - The StartLineCap Property
            // Usage: PenLineCap = Pen.StartLineCap (R/W)
            // Notes: Gets or Sets the PenLineCap enumeration for the end of the line. 
            CommonLib.LogStatus("Test #7 - The StartLineCap Property");

            // Set the StartLineCap property.
            Pprop.StartLineCap = PenLineCap.Triangle;

            // Get the StartLineCap value as an Int to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("StartLineCap", (int)Pprop.StartLineCap, (int)PenLineCap.Triangle);
            #endregion

            #region Test #8 - The Thickness Property
            // Usage: float64 = Pen.Thickness (R/W)
            // Notes: Gets or Sets the thickness of the Pen's stroke.
            CommonLib.LogStatus("Test #8 - The Thickness Property");

            // Set the Thickness property.
            Pprop.Thickness = 15.0;

            // Get the Thickness value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("Thickness", Pprop.Thickness, 15.0);

            // Add an Double Animation to the Pen
            Pprop.BeginAnimation(Pen.ThicknessProperty, AnimDouble020);

            // The Thickness value should be between 0 and 20
            if ((Pprop.Thickness >= 0.0) && (Pprop.Thickness <= 20.0))
            {
                CommonLib.LogStatus("Pass: Thickness (With Animation)=" + Pprop.Thickness.ToString());
            }
            else
            {
                CommonLib.LogFail("Fail: Thickness (With Animation)=" + Pprop.Thickness.ToString());
                _class_testresult &= false;
            }
            #endregion

            #region Test #9 - The HasAnimatedProperties Property
            // Usage: bool = Pen.HasAnimatedProperties (Read Only)
            // Notes: Returns a bool that indicates whether the Pen is animatable.
            CommonLib.LogStatus("Test #9 - The HasAnimatedProperties Property");

            Pen P29 = new Pen(Brushes.Blue, 2.0);

            // Check the HasAnimatedProperties value of a non-animated Pen.
            _class_testresult &= _helper.CompareProp("HasAnimatedProperties", P29.HasAnimatedProperties, false);

            // Add an Animation to the Pen
            P29.BeginAnimation(Pen.ThicknessProperty, AnimDouble020);

            // Check the HasAnimatedProperties value of an animated Pen.
            _class_testresult &= _helper.CompareProp("HasAnimatedProperties (Animated)", P29.HasAnimatedProperties, true);
            #endregion

            #region Test #10 - Basic rendering with a Pen
            // Draw a Line with the Pen.
            CommonLib.LogStatus("Test #10 - Basic rendering with a Pen");

            // Clear the Animations and Animated Brush from the Pen
            Pprop.Brush = new SolidColorBrush(Colors.Yellow);
            Pprop.BeginAnimation(Pen.MiterLimitProperty, null);
            Pprop.BeginAnimation(Pen.ThicknessProperty, null);

            CommonLib.LogStatus("Draw a Line using the Pen");
            DC.DrawLine(Pprop, new Point(40.0, 40.0), new Point(160.0, 40.0));

            // Create a Path Geometry
            PathGeometry path = new PathGeometry();
            PathFigure fig1 = new PathFigure();
            fig1.StartPoint = new Point(40.0, 80.0);
            fig1.Segments.Add(new LineSegment(new Point(160.0, 80.0), true));
            fig1.Segments.Add(new LineSegment(new Point(100.0, 140.0), true));
            fig1.IsClosed = true;
            path.Figures.Add(fig1);

            CommonLib.LogStatus("Draw joined Lines using the Pen");
            DC.DrawGeometry(null, Pprop, path);
            #endregion
            #endregion End Of SECTION II

            #region SECTION III - METHODS
            CommonLib.LogStatus("***** SECTION III - METHODS *****");

            #region Test #1 - The Copy Method
            // Usage: Pen = Pen.Clone()
            // Notes: Returns a Pen that is a copy of this Pen
            CommonLib.LogStatus("Test #1 - The Copy Method");

            // Create a Pen.
            Pen P31a = new Pen(Brushes.Red, 10.0);

            // Use the Copy method to create a new Pen that has the same value.
            Pen P31b = P31a.Clone();

            SolidColorBrush B31a = (SolidColorBrush)P31a.Brush;
            SolidColorBrush B31b = (SolidColorBrush)P31b.Brush;

            // Check both Pens for equality by comparing their property values
            if ((B31a.Color == B31b.Color) && (P31a.Thickness == P31b.Thickness))
            {
                CommonLib.LogStatus("Pass: Copy - Pen properties have the same value");
            }
            else
            {
                CommonLib.LogFail("Fail: Copy - Pen properties do not have the same value");
                _class_testresult &= false;
            }
            #endregion

            #region Test #2 - The CloneCurrentValue Method
            // Usage: Pen = Pen.CloneCurrentValue()
            // Notes: Returns a new Brush that has the same CurrentValue as this Brush
            CommonLib.LogStatus("Test #2 - The CloneCurrentValue Method");

            // Create a Pen.
            Pen P32a = new Pen(Brushes.Green, 20.0);

            // Use the CloneCurrentValue method to create a new Pen that has the same current value.
            Pen P32b = P32a.CloneCurrentValue();

            SolidColorBrush B32a = (SolidColorBrush)P32a.Brush;
            SolidColorBrush B32b = (SolidColorBrush)P32b.Brush;

            // Check both Penes for equality by comparing their property values
            if ((B32a.Color == B32b.Color) && (P32a.Thickness == P32b.Thickness))
            {
                CommonLib.LogStatus("Pass: CloneCurrentValue - Pen properties have the same value");
            }
            else
            {
                CommonLib.LogFail("Fail: CloneCurrentValue - Pen properties do not have the same value");
                _class_testresult &= false;
            }
            #endregion

            #endregion End Of SECTION III

            #region SECTION IV - STATIC PROPERTIES
            CommonLib.LogStatus("***** SECTION IV - STATIC PROPERTIES *****");

            #region Test #1 - The BrushProperty Property
            // Usage: DependencyProperty = Pen.BrushProperty (Read only)
            // Notes: Returns a Dependency Property for Brush.
            CommonLib.LogStatus("Test #1 - The BrushProperty Property");

            System.Windows.DependencyProperty BrushDp = Pen.BrushProperty;

            _class_testresult &= _helper.CompareProp("BrushProperty", (BrushDp.OwnerType.ToString() + "::" + BrushDp.PropertyType.ToString()), (_objectType + "::" + typeof(Brush).ToString()));
            #endregion

            #region Test #2 - The DashStyleProperty Property
            // Usage: DependencyProperty = Pen.DashStyle (Read only)
            // Notes: Returns a Dependency Property for DashStyle.
            CommonLib.LogStatus("Test #2 - The DashStyleProperty Property");

            System.Windows.DependencyProperty DashStyleDp = Pen.DashStyleProperty;

            _class_testresult &= _helper.CompareProp("DashStyleProperty", (DashStyleDp.OwnerType.ToString() + "::" + DashStyleDp.PropertyType.ToString()), (_objectType + "::" + typeof(DashStyle).ToString()));
            #endregion

            #region Test #3 - The DashCapProperty Property
            // Usage: DependencyProperty = Pen.DashCap (Read only)
            // Notes: Returns a Dependency Property for DashCap.
            CommonLib.LogStatus("Test #3 - The DashCapProperty Property");

            System.Windows.DependencyProperty DashCapDp = Pen.DashCapProperty;

            _class_testresult &= _helper.CompareProp("DashCapProperty", (DashCapDp.OwnerType.ToString() + "::" + DashCapDp.PropertyType.ToString()), (_objectType + "::" + typeof(PenLineCap).ToString()));
            #endregion

            #region Test #4 - The EndLineCapProperty Property
            // Usage: DependencyProperty = Pen.EndLineCap (Read only)
            // Notes: Returns a Dependency Property for EndLineCap.
            CommonLib.LogStatus("Test #4 - The EndLineCapProperty Property");

            System.Windows.DependencyProperty EndLineCapDp = Pen.EndLineCapProperty;

            _class_testresult &= _helper.CompareProp("EndLineCapProperty", (EndLineCapDp.OwnerType.ToString() + "::" + EndLineCapDp.PropertyType.ToString()), (_objectType + "::" + typeof(PenLineCap).ToString()));
            #endregion

            #region Test #5 - The LineJoinProperty Property
            // Usage: DependencyProperty = Pen.LineJoin (Read only)
            // Notes: Returns a Dependency Property for LineJoin.
            CommonLib.LogStatus("Test #5 - The LineJoinProperty Property");

            System.Windows.DependencyProperty LineJoinDp = Pen.LineJoinProperty;

            _class_testresult &= _helper.CompareProp("LineJoinProperty", (LineJoinDp.OwnerType.ToString() + "::" + LineJoinDp.PropertyType.ToString()), (_objectType + "::" + typeof(PenLineJoin).ToString()));
            #endregion

            #region Test #6 - The MiterLimitProperty Property
            // Usage: DependencyProperty = Pen.MiterLimit (Read only)
            // Notes: Returns a Dependency Property for MiterLimit.
            CommonLib.LogStatus("Test #6 - The MiterLimitProperty Property");

            System.Windows.DependencyProperty MiterLimitDp = Pen.MiterLimitProperty;

            _class_testresult &= _helper.CompareProp("MiterLimitProperty", (MiterLimitDp.OwnerType.ToString() + "::" + MiterLimitDp.PropertyType.ToString()), (_objectType + "::" + typeof(System.Double).ToString()));
            #endregion

            #region Test #7 - The StartLineCapProperty Property
            // Usage: DependencyProperty = Pen.StartLineCap (Read only)
            // Notes: Returns a Dependency Property for StartLineCap.
            CommonLib.LogStatus("Test #7 - The StartLineCapProperty Property");

            System.Windows.DependencyProperty StartLineCapDp = Pen.StartLineCapProperty;

            _class_testresult &= _helper.CompareProp("StartLineCapProperty", (StartLineCapDp.OwnerType.ToString() + "::" + StartLineCapDp.PropertyType.ToString()), (_objectType + "::" + typeof(PenLineCap).ToString()));
            #endregion

            #region Test #8 - The ThicknessProperty Property
            // Usage: DependencyProperty = Pen.Thickness (Read only)
            // Notes: Returns a Dependency Property for Thickness.
            CommonLib.LogStatus("Test #8 - The ThicknessProperty Property");

            System.Windows.DependencyProperty ThicknessDp = Pen.ThicknessProperty;

            _class_testresult &= _helper.CompareProp("ThicknessProperty", (ThicknessDp.OwnerType.ToString() + "::" + ThicknessDp.PropertyType.ToString()), (_objectType + "::" + typeof(System.Double).ToString()));
            #endregion
            #endregion End Of SECTION IV

            #region TEST LOGGING
            // Log the programmatic result for this API test using the
            // Automation Framework LogTest method.  If This result is False,
            // it will override the result of a Visual Comparator.  Conversely,
            // if a Visual Comparator is False it will override a True result
            // from this test.
            CommonLib.LogStatus("Logging the Test Result");
            CommonLib.LogTest("Result for:"+ _objectType );
            #endregion End of TEST LOGGING
        }

        private bool _class_testresult;
        private Type _objectType;
        private HelperClass _helper;
    }
}
