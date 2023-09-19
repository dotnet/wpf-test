// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test all public API's in the RadialGradientBrush class
//
using System;
using System.ComponentModel.Design.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class WCP_RadialGradientBrushClass : ApiTest
    {
        public WCP_RadialGradientBrushClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(RadialGradientBrush);
            _helper = new HelperClass();
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus(_objectType.ToString());

            // Test variables
            BrushConverter BConverter = new BrushConverter();
            System.Globalization.CultureInfo Cinfo = System.Globalization.CultureInfo.InvariantCulture;
            String ValidType = "String";
            InstanceDescriptor ValidType2 = new InstanceDescriptor(null, null);
            bool Comp2;
            Point NewPoint;
            String SimpleBrushString, NonSimpleBrushString;
            Type BaseClassType = typeof(Brush);
            Type ParentClassType = typeof(GradientBrush);

            // Expected exception messages
            string ConvertToNotSupportedException = "Cannot convert to type.";

            // Animations to be used for testing Properties
            PointAnimation AnimPoint = new PointAnimation(new Point(0, 0), new Point(200, 200), new System.Windows.Duration(TimeSpan.FromMilliseconds(1000)));
            DoubleAnimation AnimDouble = new DoubleAnimation(0, 1.0, new System.Windows.Duration(TimeSpan.FromMilliseconds(0)), FillBehavior.Stop);
            Transform AnimTransform = new TranslateTransform(0, 0);
            AnimTransform.BeginAnimation(TranslateTransform.XProperty, AnimDouble);

            // SimpleBrush and Non-SimpleBrush for Serialization method tests
            RadialGradientBrush SimpleBrush = new RadialGradientBrush(Colors.Blue, Colors.Red);
            RadialGradientBrush NonSimpleBrush = new RadialGradientBrush();
            NonSimpleBrush.Center = new Point(0.3, 0.4);

            #region SECTION I - CONSTRUCTORS
            CommonLib.LogStatus("***** SECTION I - CONSTRUCTORS *****");

            #region Test #1 - Default Constructor
            // Usage: RadialGradientBrush()
            // Notes: Default Constructor creates an empty RadialGradientBrush.
            CommonLib.LogStatus("Test #1 - Default Constructor");

            // Create a RadialGradientBrush 
            RadialGradientBrush RGB11 = new RadialGradientBrush();

            // Confirm that a RadialGradientBrush was created successfully.
            _class_testresult &= _helper.CheckType(RGB11, _objectType);
            #endregion

            #region Test #2 - Constructor with two Colors
            // Usage: RadialGradientBrush(Color, Color)
            // Notes: Creates a RadialGradientBrush with (Inner Color, Outer Color). 
            CommonLib.LogStatus("Test #2 - Constructor with two Colors");

            // Create a RadialGradientBrush 
            RadialGradientBrush RGBprop = new RadialGradientBrush(Colors.Blue, Colors.Red);

            // Confirm that a RadialGradientBrush was created successfully.
            _class_testresult &= _helper.CheckType(RGBprop, _objectType);
            #endregion

            #region Test #3 - Constructor with a GradientStopCollection
            // Usage: RadialGradientBrush(GradientStopCollection)
            // Notes: Creates a RadialGradientBrush using a GradientStopCollection
            CommonLib.LogStatus("Test #3 - Constructor with a GradientStopCollection");

            // Create a GradientStopCollection
            GradientStopCollection GSC13 = new GradientStopCollection();
            GSC13.Add(new GradientStop(Colors.Red, 0.0));
            GSC13.Add(new GradientStop(Colors.Green, 0.5));
            GSC13.Add(new GradientStop(Colors.Blue, 1.0));

            // Create a RadialGradientBrush
            RadialGradientBrush RGB13 = new RadialGradientBrush(GSC13);

            // Confirm that a RadialGradientBrush was created successfully.
            _class_testresult &= _helper.CheckType(RGB13, _objectType);
            #endregion
            #endregion End Of SECTION I

            #region SECTION II - PROPERTIES
            CommonLib.LogStatus("***** SECTION II - PROPERTIES *****");

            #region Test #1 - The Center Property
            // Usage: Point = RadialGradientBrush.Center (R/W)
            // Notes: Gets or Sets a Point that describes the Center of the RadialGradient.
            CommonLib.LogStatus("Test #1 - The Center Property");

            // Set the Center property.
            NewPoint = new Point(100.0, 80.0);
            RGBprop.Center = NewPoint;

            // Get the Center value to assure that it was the one that was set.
            _class_testresult &= _helper.ComparePoint("Center", RGBprop.Center, NewPoint);

            // Add a Point Animation to the Brush
            RGBprop.BeginAnimation(RadialGradientBrush.CenterProperty, AnimPoint);

            // Center should be between 0,0 and 200,200
            _class_testresult &= _helper.ComparePointWithRange("Center (With Animation)", RGBprop.Center, 0, 200);
            #endregion

            #region Test #2 - The RadiusX Property
            // Usage: float64 = RadialGradientBrush.RadiusX (R/W)
            // Notes: Gets or Sets the X coordinate of the Radius or extent of the RadialGradient.
            CommonLib.LogStatus("Test #2 - The RadiusX Property");

            // Set the RadiusX property.
            RGBprop.RadiusX = 50.0;

            // Get the RadiusX value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("RadiusX", RGBprop.RadiusX, 50.0);

            // Add a Double Animation to the Brush
            RGBprop.BeginAnimation(RadialGradientBrush.RadiusXProperty, AnimDouble);

            // Get the RadiusX value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("RadiusX (With Animation)", RGBprop.RadiusX, 50.0);
            #endregion

            #region Test #3 - The RadiusY Property
            // Usage: float64 = RadialGradientBrush.RadiusY (R/W)
            // Notes: Gets or Sets the Y coordinate of the Radius or extent of the RadialGradient.
            CommonLib.LogStatus("Test #3 - The RadiusY Property");

            // Set the RadiusY property.
            RGBprop.RadiusY = 60.0;

            // Get the RadiusY value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("RadiusY", RGBprop.RadiusY, 60.0);

            // Add a Double Animation to the Brush
            RGBprop.BeginAnimation(RadialGradientBrush.RadiusYProperty, AnimDouble);

            // Get the RadiusY value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("RadiusY (With Animation)", RGBprop.RadiusY, 60.0);
            #endregion

            #region Test #4 - The GradientOrigin Property
            // Usage: Point = RadialGradientBrush.GradientOrigin (R/W)
            // Notes: Gets or Sets the GradientOrigin point of the Radial Gradient, where you want the eye to focus.
            CommonLib.LogStatus("Test #4 - The GradientOrigin Property");

            // Set the GradientOrigin property.
            NewPoint = new Point(80.0, 100.0);
            RGBprop.GradientOrigin = NewPoint;

            // Get the GradientOrigin value to assure that it was the one that was set.
            _class_testresult &= _helper.ComparePoint("GradientOrigin", RGBprop.GradientOrigin, NewPoint);

            // Add a Point Animation to the Brush
            RGBprop.BeginAnimation(RadialGradientBrush.GradientOriginProperty, AnimPoint);

            // GradientOrigin should be between 0,0 and 200,200
            _class_testresult &= _helper.ComparePointWithRange("GradientOrigin (With Animation)", RGBprop.GradientOrigin, 0, 200);
            #endregion

            #region Test #5 - The ColorInterpolationMode Property
            // Usage: ColorInterpolationMode = RadialGradientBrush.ColorInterpolationMode (R/W)
            // Notes: Gets or Sets a ColorInterpolationMode that indicates whether the Colors
            // in the Gradient are being interpolated with a perceptual (sRGB) or physical (scRGB)
            // Radial gamma.
            CommonLib.LogStatus("Test #5 - The ColorInterpolationMode Property");

            // Set the ColorInterpolationMode property.
            RGBprop.ColorInterpolationMode = ColorInterpolationMode.ScRgbLinearInterpolation;

            // Get the ColorInterpolationMode as an Int value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("ColorInterpolationMode", (int)ColorInterpolationMode.ScRgbLinearInterpolation, (int)RGBprop.ColorInterpolationMode);
            #endregion

            #region Test #6 - The GradientStops Property
            // Usage: GradientStopCollection = RadialGradientBrush.GradientStops (R/W)
            // Notes: Gets or Sets a GradientStopCollection that can be used to access the 
            // individual GradientStops.
            CommonLib.LogStatus("Test #6 - The GradientStops Property");

            // Test for null gradient stops. Just to check if it crashes or not.
            RGBprop.GradientStops = null;
            DC.DrawRectangle(RGBprop, null, new System.Windows.Rect(0, 0, 0, 0));
            CommonLib.LogStatus("Passed: Null GradientStops");

            // Set the GradientStops property.
            GradientStopCollection GSC46 = new GradientStopCollection();
            GSC46.Add(new GradientStop(Colors.Red, 0.0));
            GSC46.Add(new GradientStop(Colors.White, 0.5));
            GSC46.Add(new GradientStop(Colors.Blue, 1.0));
            RGBprop.GradientStops = GSC46;

            // Get the GradientStops value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("GradientStops", GSC46, RGBprop.GradientStops);

            // Set the GradientStops property to a GradientStopCollection containing an Animated GradientStop.
            GradientStop AnimStop = new GradientStop(Colors.Blue, 1);
            AnimStop.BeginAnimation(GradientStop.OffsetProperty, AnimDouble);
            GradientStopCollection GSC46Anim = new GradientStopCollection();
            GSC46Anim.Add(new GradientStop(Colors.Red, 0));
            GSC46Anim.Add(AnimStop);
            RGBprop.GradientStops = GSC46Anim;

            // Get the GradientStops value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("GradientStops (With Animation)", GSC46Anim, RGBprop.GradientStops);
            #endregion

            #region Test #7 - The MappingMode Property
            // Usage: BrushMappingMode = RadialGradientBrush.MappingMode (R/W)
            // Notes: Gets or Sets the BrushMappingMode that indicates whether the GradientBrush
            // will map it's coordinates to the entire user space or just to the shape that is being filled.
            CommonLib.LogStatus("Test #7 - The MappingMode Property");

            // Set the MappingMode property.
            RGBprop.MappingMode = BrushMappingMode.Absolute;

            // Get the MappingMode value as an Int value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("MappingMode", (int)BrushMappingMode.Absolute, (int)RGBprop.MappingMode);
            #endregion

            #region Test #8 - The SpreadMethod Property
            // Usage: GradientSpreadMethod = RadialGradientBrush.SpreadMethod (R/W)
            // Notes: Gets or Sets a GradientSpreadMethod that indicates how the area outside of the
            // GradientStops will be filled.
            CommonLib.LogStatus("Test #8 - The SpreadMethod Property");

            // Set the SpreadMethod property.
            RGBprop.SpreadMethod = GradientSpreadMethod.Pad;

            // Get the SpreadMethod value as an Int value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("SpreadMethod", (int)GradientSpreadMethod.Pad, (int)RGBprop.SpreadMethod);
            #endregion

            #region Test #9 - The Opacity Property
            // Usage: double = RadialGradientBrush.Opacity (R/W)
            // Notes: Gets or Sets the Opacity value of the Brush.
            CommonLib.LogStatus("Test #9 - The Opacity Property");

            // Set the Opacity property.
            RGBprop.Opacity = 0.9;

            // Get the Opacity value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("Opacity", RGBprop.Opacity, 0.9);

            // Add an Opacity Animation to the Brush
            RGBprop.BeginAnimation(RadialGradientBrush.OpacityProperty, AnimDouble);

            // Get the Opacity value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("Opacity (With Animation)", RGBprop.Opacity, 0.9);
            #endregion

            #region Test #10 - The Transform Property
            // Usage: Transform = RadialGradientBrush.Transform (R/W)
            // Notes: Gets or Sets the Transform for the Brush.
            CommonLib.LogStatus("Test #10 - The Transform Property");

            // Set the Transform property.
            RGBprop.Transform = new ScaleTransform(0.5, 0.5, 100, 100);

            // Get the Transform value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareMatrix("Transform", RGBprop.Transform, 0.5, 0, 0, 0.5, 50, 50);

            // Set the Transform property to an Animated Transform.
            RGBprop.Transform = AnimTransform;

            // Get the Transform value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareMatrix("Transform (With Animation)", RGBprop.Transform, 1, 0, 0, 1, 0, 0);
            #endregion

            #region Test #11 - The RelativeTransform Property
            // Usage: Transform = RadialGradientBrush.RelativeTransform (R/W)
            // Notes: Gets or Sets the Transform for the Brush relative to the object it is filling.
            CommonLib.LogStatus("Test #11 - The RelativeTransform Property");

            // Use the RelativeTransform property to set the Brush Transform relative to the object it is filling.
            RGBprop.RelativeTransform = new ScaleTransform(2, 2, .5, .5);

            // Get the RelativeTransform value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareMatrix("RelativeTransform", RGBprop.RelativeTransform, 2, 0, 0, 2, -.5, -.5);

            // Set the RelativeTransform property to an Animated Transform.
            RGBprop.RelativeTransform = AnimTransform;

            // Get the Transform value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareMatrix("RelativeTransform (With Animation)", RGBprop.RelativeTransform, 1, 0, 0, 1, 0, 0);
            #endregion

            #region Test #12 - The CanFreeze Property
            // Usage: bool = RadialGradientBrush.CanFreeze (Read Only)
            // Notes: Returns a bool that indicates whether the Brush is animatable.
            CommonLib.LogStatus("Test #12 - The CanFreeze Property");

            RadialGradientBrush RGB212 = new RadialGradientBrush(Colors.Blue, Colors.Red);

            // Check the CanFreeze value of a non-animated Brush.
            _class_testresult &= _helper.CompareProp("CanFreeze", RGB212.CanFreeze, true);

            // Add an Animation to the Brush
            RGB212.BeginAnimation(RadialGradientBrush.CenterProperty, AnimPoint);

            // Check the CanFreeze value of an animated Brush.
            _class_testresult &= _helper.CompareProp("CanFreeze (Animated Brush)", RGB212.CanFreeze, false);
            #endregion

            #region Test #13 - Basic rendering with a RadialGradientBrush
            // Just fill the surface with a RadialGradientBrush to see if we crash or not.
            CommonLib.LogStatus("Test #13 - Basic rendering with a RadialGradientBrush");

            // Clear all Animations before rendering
            RGBprop.BeginAnimation(RadialGradientBrush.CenterProperty, null);
            RGBprop.BeginAnimation(RadialGradientBrush.GradientOriginProperty, null);
            RGBprop.BeginAnimation(RadialGradientBrush.RadiusXProperty, null);
            RGBprop.BeginAnimation(RadialGradientBrush.RadiusYProperty, null);
            RGBprop.BeginAnimation(RadialGradientBrush.OpacityProperty, null);
            RGBprop.GradientStops = GSC46;
            RGBprop.Transform = null;
            RGBprop.RelativeTransform = null;

            DC.DrawRectangle(RGBprop, null, new System.Windows.Rect(m_top, m_left, m_width, m_height));
            #endregion
            #endregion End Of SECTION II

            #region SECTION III - METHODS
            CommonLib.LogStatus("***** SECTION III - METHODS *****");

            #region Test #1 - The Copy Method
            // Usage: RadialGradientBrush = RadialGradientBrush.Clone()
            // Notes: Returns a RadialGradientBrush that is a copy of this RadialGradientBrush
            CommonLib.LogStatus("Test #1 - The Copy Method");

            // Create a RadialGradientBrush.
            RadialGradientBrush RGB31a = new RadialGradientBrush(Colors.Blue, Colors.Yellow);

            // Use the Copy method to create a new RadialGradientBrush that has the same value.
            RadialGradientBrush RGB31b = RGB31a.Clone();

            // Check both RadialGradientBrushes for equality by comparing their property values
            if ((RGB31a.Center == RGB31b.Center) && (RGB31a.GradientOrigin == RGB31b.GradientOrigin) &&
                (RGB31a.RadiusX == RGB31b.RadiusX) && (RGB31a.RadiusY == RGB31b.RadiusY) &&
                (_helper.CompareGradientStopCollections(RGB31a.GradientStops, RGB31b.GradientStops)))
            {
                CommonLib.LogStatus("Pass: Copy - RadialGradientBrush properties have the same value");
            }
            else
            {
                CommonLib.LogFail("Fail: Copy - RadialGradientBrush properties do not have the same value");
                _class_testresult &= false;
            }
            #endregion

            #region Test #2 - The CloneCurrentValue Method
            // Usage: RadialGradientBrush = RadialGradientBrush.CloneCurrentValue()
            // Notes: Returns a new Brush that has the same CurrentValue as this Brush
            CommonLib.LogStatus("Test #2 - The CloneCurrentValue Method");

            // Create a RadialGradientBrush.
            RadialGradientBrush RGB32a = new RadialGradientBrush(Colors.Green, Colors.Red);

            // Use the CloneCurrentValue method to create a new RadialGradientBrush that has the same current value.
            RadialGradientBrush RGB32b = RGB32a.CloneCurrentValue();

            // Check both RadialGradientBrushes for equality by comparing their property values
            if ((RGB32a.Center == RGB32b.Center) && (RGB32a.GradientOrigin == RGB32b.GradientOrigin) &&
                (RGB32a.RadiusX == RGB32b.RadiusX) && (RGB32a.RadiusY == RGB32b.RadiusY) &&
                (_helper.CompareGradientStopCollections(RGB32a.GradientStops, RGB32b.GradientStops)))
            {
                CommonLib.LogStatus("Pass: CloneCurrentValue - RadialGradientBrush properties have the same value");
            }
            else
            {
                CommonLib.LogFail("Fail: CloneCurrentValue - RadialGradientBrush properties do not have the same value");
                _class_testresult &= false;
            }
            #endregion

            #region Test #3 - The GradientStops.Add Method
            // Usage: RadialGradientBrush.GradientStops.Add(new GradientStop(Color, float64))
            // Notes: Convenience Method for adding a new GradientStop to the GradientStopCollection.
            CommonLib.LogStatus("Test #3 - The GradientStops.Add Method");

            // Create a RadialGradientBrush.
            RadialGradientBrush RGB33 = new RadialGradientBrush(Colors.Red, Colors.Blue);

            // Use the GradientStops.Add Method
            RGB33.GradientStops.Add(new GradientStop(Colors.Green, 0.5));

            // Create a GradientStopCollection to compare with the Brush
            GradientStopCollection GSExpected = new GradientStopCollection();

            GSExpected.Add(new GradientStop(Colors.Red, 0.0));
            GSExpected.Add(new GradientStop(Colors.Blue, 1.0));
            GSExpected.Add(new GradientStop(Colors.Green, 0.5));

            // Check that the GradientStop was added correctly
            _class_testresult &= _helper.CompareProp("GradientStops.Add", RGB33.GradientStops, GSExpected);
            #endregion

            #region Test #4 - The ToString Method
            // Usage: string = RadialGradientBrush.ToString()
            // Notes: Persists the Brush in a string.  If the Brush is not a "simple brush" then this should currently
            // return a System Not Supported exception.
            CommonLib.LogStatus("Test #4 - The ToString Method");

            // Use the ToString method on a SimpleBrush
            SimpleBrushString = SimpleBrush.ToString();
            _class_testresult &= _helper.CompareProp("ToString - Simple Brush", SimpleBrushString, "System.Windows.Media.RadialGradientBrush");

            try
            {
                // Use the ToString method on a Non-SimpleBrush
                NonSimpleBrushString = NonSimpleBrush.ToString();
            }
            catch (System.Exception e)
            {
                if (e.Message == ConvertToNotSupportedException)
                {
                    CommonLib.LogStatus("Pass: ToString - Non Simple Brush returns the expected exception");
                }
                else
                {
                    CommonLib.LogFail("Fail: ToString - Non Simple Brush returns " + e.Message + " should be " + ConvertToNotSupportedException);
                    _class_testresult &= false;
                }
            }
            #endregion

            #region Test #5 - The ToString(IFormatProvider) Method
            // Usage: string = RadialGradientBrush.ToString(IFormatProvider)
            // Notes: Persists the Brush in a string according to the IFormatProvider.  If the Brush is not a "simple brush" then this should currently
            // return a System Not Supported exception.
            CommonLib.LogStatus("Test #5 - The ToString(IFormatProvider) Method");

            // Use the ToString method on a SimpleBrush
            SimpleBrushString = SimpleBrush.ToString(default(System.IFormatProvider));
            _class_testresult &= _helper.CompareProp("ToString(IFormatProvider) - Simple Brush", SimpleBrushString, "System.Windows.Media.RadialGradientBrush");

            try
            {
                // Use the ToString method on a Non-SimpleBrush
                NonSimpleBrushString = NonSimpleBrush.ToString(default(System.IFormatProvider));
            }
            catch (System.Exception e)
            {
                if (e.Message == ConvertToNotSupportedException)
                {
                    CommonLib.LogStatus("Pass: ToString(IFormatProvider) - Non Simple Brush returns the expected exception");
                }
                else
                {
                    CommonLib.LogFail("Fail: ToString(IFormatProvider) - Non Simple Brush returns " + e.Message + " should be " + ConvertToNotSupportedException);
                    _class_testresult &= false;
                }
            }
            #endregion
            #endregion End Of SECTION III

            #region SECTION IV - INTERNAL METHODS
            CommonLib.LogStatus("***** SECTION IV - INTERNAL METHODS *****");

            #region Test #1 - The IsSimpleBrush Method
            // Usage: bool = RadialGradientBrush.IsSimpleBrush()
            // Notes: Returns a bool that indicates whether the Brush is "simple" enough to
            // be converted to a String or Serialized.
            CommonLib.LogStatus("Test #1 - The IsSimpleBrush Method");

            MyTypeConverter non_simple_type_converter = new MyTypeConverter(NonSimpleBrush);
            Comp2 = BConverter.CanConvertTo(non_simple_type_converter, ValidType.GetType());
            _class_testresult &= _helper.CompareProp("IsSimpleBrush - Non Simple", Comp2, false);
            #endregion

            #region Test #2 - The ConvertToString Method
            // Usage: string RadialGradientBrush.ConvertToString(IFormatProvider)
            // Notes: Returns a string representation of the Brush.  If the Brush is not a
            // simple brush then a "not supported exception" is raised.
            CommonLib.LogStatus("Test #2 - The ConvertToString Method");
            SimpleBrushString = (String)BConverter.ConvertTo(null, Cinfo, SimpleBrush, ValidType.GetType());
            _class_testresult &= _helper.CompareProp("ConvertToString - Simple Brush", SimpleBrushString, "System.Windows.Media.RadialGradientBrush");
            try
            {
                NonSimpleBrushString = (String)BConverter.ConvertTo(null, Cinfo, NonSimpleBrush, ValidType.GetType());
            }
            catch (System.Exception e)
            {
                if (e.Message == ConvertToNotSupportedException)
                {
                    CommonLib.LogStatus("Pass: ConvertToString - Non Simple Brush returns the expected exception");
                }
                else
                {
                    CommonLib.LogFail("Fail: ConvertToString - Non Simple Brush returns " + e.Message + " should be " + ConvertToNotSupportedException);
                    _class_testresult &= false;
                }
            }
            #endregion
            #endregion End Of SECTION IV

            #region SECTION V - STATIC PROPERTIES
            CommonLib.LogStatus("***** SECTION V - STATIC PROPERTIES *****");

            #region Test #1 - The CenterProperty Property
            // Usage: DependencyProperty = RadialGradientBrush.CenterProperty (Read only)
            // Notes: Returns a Dependency Property for Center.
            CommonLib.LogStatus("Test #1 - The CenterProperty Property");

            System.Windows.DependencyProperty CenterDp = RadialGradientBrush.CenterProperty;

            _class_testresult &= _helper.CompareProp("CenterProperty", (CenterDp.OwnerType.ToString() + "::" + CenterDp.PropertyType.ToString()), (_objectType + "::" + typeof(System.Windows.Point).ToString()));
            #endregion

            #region Test #2 - The RadiusXProperty Property
            // Usage: DependencyProperty = RadialGradientBrush.RadiusXProperty (Read only)
            // Notes: Returns a Dependency Property for RadiusX.
            CommonLib.LogStatus("Test #2 - The RadiusXProperty Property");

            System.Windows.DependencyProperty RadiusXDp = RadialGradientBrush.RadiusXProperty;

            _class_testresult &= _helper.CompareProp("RadiusXProperty", (RadiusXDp.OwnerType.ToString() + "::" + RadiusXDp.PropertyType.ToString()), (_objectType + "::" + typeof(System.Double).ToString()));
            #endregion

            #region Test #3 - The RadiusYProperty Property
            // Usage: DependencyProperty = RadialGradientBrush.RadiusYProperty (Read only)
            // Notes: Returns a Dependency Property for RadiusY.
            CommonLib.LogStatus("Test #3 - The RadiusYProperty Property");

            System.Windows.DependencyProperty RadiusYDp = RadialGradientBrush.RadiusYProperty;

            _class_testresult &= _helper.CompareProp("RadiusYProperty", (RadiusYDp.OwnerType.ToString() + "::" + RadiusYDp.PropertyType.ToString()), (_objectType + "::" + typeof(System.Double).ToString()));
            #endregion

            #region Test #4 - The GradientOriginProperty Property
            // Usage: DependencyProperty = RadialGradientBrush.GradientOriginProperty (Read only)
            // Notes: Returns a Dependency Property for GradientOrigin.
            CommonLib.LogStatus("Test #4 - The GradientOriginProperty Property");

            System.Windows.DependencyProperty GradientOriginDp = RadialGradientBrush.GradientOriginProperty;

            _class_testresult &= _helper.CompareProp("GradientOriginProperty", (GradientOriginDp.OwnerType.ToString() + "::" + GradientOriginDp.PropertyType.ToString()), (_objectType + "::" + typeof(System.Windows.Point).ToString()));
            #endregion

            #region Test #5 - The ColorInterpolationModeProperty Property
            // Usage: DependencyProperty = RadialGradientBrush.ColorInterpolationModeProperty (Read only)
            // Notes: Returns a Dependency Property for ColorInterpolationMode
            CommonLib.LogStatus("Test #5 - The ColorInterpolationModeProperty Property");

            System.Windows.DependencyProperty ColorInterpolationModeDp = RadialGradientBrush.ColorInterpolationModeProperty;

            _class_testresult &= _helper.CompareProp("ColorInterpolationModeProperty", (ColorInterpolationModeDp.OwnerType.ToString() + "::" + ColorInterpolationModeDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(ColorInterpolationMode).ToString()));
            #endregion

            #region Test #6 - The GradientStopsProperty Property
            // Usage: DependencyProperty = RadialGradientBrush.GradientStopsProperty (Read only)
            // Notes: Returns a Dependency Property for GradientStops
            CommonLib.LogStatus("Test #6 - The GradientStopsProperty Property");

            System.Windows.DependencyProperty GradientStopsDp = RadialGradientBrush.GradientStopsProperty;

            _class_testresult &= _helper.CompareProp("GradientStopsProperty", (GradientStopsDp.OwnerType.ToString() + "::" + GradientStopsDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(GradientStopCollection).ToString()));
            #endregion

            #region Test #7 - The MappingModeProperty Property
            // Usage: DependencyProperty = RadialGradientBrush.MappingModeProperty (Read only)
            // Notes: Returns a Dependency Property for MappingMode
            CommonLib.LogStatus("Test #7 - The MappingModeProperty Property");

            System.Windows.DependencyProperty MappingModeDp = RadialGradientBrush.MappingModeProperty;

            _class_testresult &= _helper.CompareProp("MappingModeProperty", (MappingModeDp.OwnerType.ToString() + "::" + MappingModeDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(BrushMappingMode).ToString()));
            #endregion

            #region Test #8 - The SpreadMethodProperty Property
            // Usage: DependencyProperty = RadialGradientBrush.SpreadMethod (Read only)
            // Notes: Returns a Dependency Property for SpreadMethod
            CommonLib.LogStatus("Test #8 - The SpreadMethodProperty Property");

            System.Windows.DependencyProperty SpreadMethodDp = RadialGradientBrush.SpreadMethodProperty;

            _class_testresult &= _helper.CompareProp("SpreadMethodProperty", (SpreadMethodDp.OwnerType.ToString() + "::" + SpreadMethodDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(GradientSpreadMethod).ToString()));
            #endregion

            #region Test #9 - The OpacityProperty Property
            // Usage: DependencyProperty = RadialGradientBrush.Opacity (Read only)
            // Notes: Returns a Dependency Property for Opacity.
            CommonLib.LogStatus("Test #9 - The OpacityProperty Property");

            System.Windows.DependencyProperty OpacityDp = RadialGradientBrush.OpacityProperty;

            _class_testresult &= _helper.CompareProp("OpacityProperty", (OpacityDp.OwnerType.ToString() + "::" + OpacityDp.PropertyType.ToString()), (BaseClassType + "::" + typeof(System.Double).ToString()));
            #endregion

            #region Test #10 - The TransformProperty Property
            // Usage: DependencyProperty = RadialGradientBrush.TransformProperty (Read only)
            // Notes: Returns a Dependency Property for Transform.
            CommonLib.LogStatus("Test #10 - The TransformProperty Property");

            System.Windows.DependencyProperty TransformDp = RadialGradientBrush.TransformProperty;

            _class_testresult &= _helper.CompareProp("TransformProperty", (TransformDp.OwnerType.ToString() + "::" + TransformDp.PropertyType.ToString()), (BaseClassType + "::" + typeof(Transform).ToString()));
            #endregion

            #region Test #11 - The RelativeTransformProperty Property
            // Usage: DependencyProperty = RadialGradientBrush.RelativeTransformProperty (Read only)
            // Notes: Returns a Dependency Property for RelativeTransform.
            CommonLib.LogStatus("Test #11 - The RelativeTransformProperty Property");

            System.Windows.DependencyProperty RelativeTransformDp = RadialGradientBrush.RelativeTransformProperty;

            _class_testresult &= _helper.CompareProp("RelativeTransformProperty", (RelativeTransformDp.OwnerType.ToString() + "::" + RelativeTransformDp.PropertyType.ToString()), (BaseClassType + "::" + typeof(Transform).ToString()));
            #endregion
            #endregion End Of SECTION V

            CommonLib.LogTest("Result for: " + _objectType);
        }

        private bool _class_testresult;
        private Type _objectType;
        private HelperClass _helper;
    }
}
