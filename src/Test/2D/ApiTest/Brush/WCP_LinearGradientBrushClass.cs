// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test all public API's in the LinearGradientBrush class
//
using System;
using System.ComponentModel.Design.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Globalization;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class WCP_LinearGradientBrushClass : ApiTest
    {
        public WCP_LinearGradientBrushClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(LinearGradientBrush);
            _helper = new HelperClass();
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus(_objectType.ToString());

            // Test variables
            BrushConverter BConverter = new BrushConverter();
            CultureInfo Cinfo = CultureInfo.InvariantCulture;
            String ValidType = "String";
            InstanceDescriptor ValidType2 = new InstanceDescriptor(null, null);
            bool  Comp2;
            Point NewPoint;
            String SimpleBrushString, NonSimpleBrushString;
            Type BaseClassType = typeof(Brush);
            Type ParentClassType = typeof(GradientBrush);
            char separator = _helper.ValueSeparator;

            // Expected exception messages
            String ConvertToNotSupportedException = "Cannot convert to type.";

            // Animations to be used for testing Properties
            PointAnimation AnimPoint = new PointAnimation(new Point(0, 0), new Point(200, 200), new System.Windows.Duration(TimeSpan.FromMilliseconds(1000)));
            DoubleAnimation AnimDouble = new DoubleAnimation(0, 1.0, new System.Windows.Duration(TimeSpan.FromMilliseconds(0)), FillBehavior.Stop);
            Transform AnimTransform = new TranslateTransform(0, 0);
            AnimTransform.BeginAnimation(TranslateTransform.XProperty, AnimDouble);

            // SimpleBrush and Non-SimpleBrush for Serialization method tests
            LinearGradientBrush SimpleBrush = new LinearGradientBrush(Colors.Blue, Colors.Red, 0.0);
            LinearGradientBrush NonSimpleBrush = new LinearGradientBrush();
            GradientStopCollection GSColl = new GradientStopCollection();
            GSColl.Add(new GradientStop(Colors.Red, 0.0));
            GSColl.Add(new GradientStop(Colors.Green, 0.5));
            GSColl.Add(new GradientStop(Colors.Blue, 1.0));
            NonSimpleBrush.GradientStops = GSColl;

            #region SECTION I - CONSTRUCTORS
            CommonLib.LogStatus("***** SECTION I - CONSTRUCTORS *****");

            #region Test #1 - Default Constructor
            // Usage: LinearGradientBrush()
            // Notes: Default Constructor creates an empty LinearGradientBrush.
            CommonLib.LogStatus("Test #1 - Default Constructor");

            // Create a LinearGradientBrush 
            LinearGradientBrush LGB11 = new LinearGradientBrush();

            // Confirm that a LinearGradientBrush was created successfully.
            _class_testresult &= _helper.CheckType(LGB11, _objectType);
            #endregion

            #region Test #2 - Constructor with two Colors and a double
            // Usage: LinearGradientBrush(Color, Color, float64)
            // Notes: Creates a LinearGradientBrush using two Colors and an angle: (Color1, Color2, Angle)
            CommonLib.LogStatus("Test #2 - Constructor with two Colors and a double");

            // Create a LinearGradientBrush
            LinearGradientBrush LGBprop = new LinearGradientBrush(Colors.Blue, Colors.Red, 0.0);

            // Confirm that a LinearGradientBrush was created successfully.
            _class_testresult &= _helper.CheckType(LGBprop, _objectType);
            #endregion

            #region Test #3 - Constructor with two Colors and two Points
            // Usage: LinearGradientBrush(Color, Color, Point, Point)
            // Notes: Creates a LinearGradientBrush using two Colors and 
            // two Points: (startColor, endColor, StartPoint, EndPoint)
            CommonLib.LogStatus("Test #3 - Constructor with two Colors and two Points");

            // Create a LinearGradientBrush
            LinearGradientBrush LGB13 = new LinearGradientBrush(Colors.Blue, Colors.Red, new Point(50.0, 50.0), new Point(150.0, 150.0));

            // Confirm that a LinearGradientBrush was created successfully.
            _class_testresult &= _helper.CheckType(LGB13, _objectType);
            #endregion

            #region Test #4 - Constructor with a GradientStopCollection
            // Usage: LinearGradientBrush(GradientStopCollection)
            // Notes: Creates a LinearGradientBrush using a GradientStopCollection
            CommonLib.LogStatus("Test #4 - Constructor with a GradientStopCollection");

            // Create a GradientStopCollection
            GradientStopCollection GSC14 = new GradientStopCollection();
            GSC14.Add(new GradientStop(Colors.Red, 0.0));
            GSC14.Add(new GradientStop(Colors.Green, 0.5));
            GSC14.Add(new GradientStop(Colors.Blue, 1.0));

            // Create a LinearGradientBrush
            LinearGradientBrush LGB14 = new LinearGradientBrush(GSC14);

            // Confirm that a LinearGradientBrush was created successfully.
            _class_testresult &= _helper.CheckType(LGB14, _objectType);
            #endregion
            #endregion End Of SECTION I

            #region SECTION II - PROPERTIES
            CommonLib.LogStatus("***** SECTION II - PROPERTIES *****");

            #region Test #1 - The StartPoint Property
            // Usage: Point = LinearGradientBrush.StartPoint (R/W)
            // Notes: Gets or Sets a Point that describes the StartPoint of the LinearGradient.
            CommonLib.LogStatus("Test #1 - The StartPoint Property");

            // Set the StartPoint property.
            NewPoint = new Point(0.25, 0.25);
            LGBprop.StartPoint = NewPoint;

            // Get the StartPoint value to assure that it was the one that was set.
            _class_testresult &= _helper.ComparePoint("StartPoint", LGBprop.StartPoint, NewPoint);

            // Add a Point Animation to the Brush
            LGBprop.BeginAnimation(LinearGradientBrush.StartPointProperty, AnimPoint);

            // StartPoint should be between 0,0 and 200,200
            _class_testresult &= _helper.ComparePointWithRange("StartPoint (With Animation)", LGBprop.StartPoint, 0, 200);
            #endregion

            #region Test #2 - The EndPoint Property
            // Usage: Point = LinearGradientBrush.EndPoint (R/W)
            // Notes: Gets or Sets a Point that describes the EndPoint of the LinearGradient.
            CommonLib.LogStatus("Test #2 - The EndPoint Property");

            // Set the EndPoint property.
            NewPoint = new Point(0.75, 0.75);
            LGBprop.EndPoint = NewPoint;

            // Get the EndPoint value to assure that it was the one that was set.
            _class_testresult &= _helper.ComparePoint("EndPoint", LGBprop.EndPoint, NewPoint);

            // Add a Point Animation to the Brush
            LGBprop.BeginAnimation(LinearGradientBrush.EndPointProperty, AnimPoint);

            // EndPoint should be between 0,0 and 200,200
            _class_testresult &= _helper.ComparePointWithRange("EndPoint (With Animation)", LGBprop.EndPoint, 0, 200);
            #endregion

            #region Test #3 - The ColorInterpolationMode Property
            // Usage: ColorInterpolationMode = LinearGradientBrush.ColorInterpolationMode (R/W)
            // Notes: Gets or Sets a ColorInterpolationMode that indicates whether the Colors
            // in the Gradient are being interpolated with a perceptual or physical linear gamma.
            CommonLib.LogStatus("Test #3 - The ColorInterpolationMode Property");

            // Set the ColorInterpolationMode property.
            LGBprop.ColorInterpolationMode = ColorInterpolationMode.SRgbLinearInterpolation;

            // Get the ColorInterpolationMode value (as an Int) to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("ColorInterpolationMode", (int)ColorInterpolationMode.SRgbLinearInterpolation, (int)LGBprop.ColorInterpolationMode);
            #endregion

            #region Test #4 - The GradientStops Property
            // Usage: GradientStopCollection = LinearGradientBrush.GradientStops (R/W)
            // Notes: Gets or Sets a GradientStopCollection that can be used to access the 
            // individual GradientStops.
            CommonLib.LogStatus("Test #4 - The GradientStops Property");

            // Test for null gradient stops. Just to check if it crashes or not.
            LGBprop.GradientStops = null;
            DC.DrawRectangle(LGBprop, null, new System.Windows.Rect(0, 0, 0, 0));
            CommonLib.LogStatus("Passed: Null GradientStops");

            // Set the GradientStops property.
            GradientStopCollection GSC41 = new GradientStopCollection();
            GSC41.Add(new GradientStop(Colors.Red, 0.0));
            GSC41.Add(new GradientStop(Colors.Green, 0.5));
            GSC41.Add(new GradientStop(Colors.Blue, 1.0));
            LGBprop.GradientStops = GSC41;

            // Get the GradientStops value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("GradientStops", GSC41, LGBprop.GradientStops);

            // Set the GradientStops property to a GradientStopCollection containing an Animated GradientStop.
            GradientStop AnimStop = new GradientStop(Colors.Blue, 1);
            AnimStop.BeginAnimation(GradientStop.OffsetProperty, AnimDouble);
            GradientStopCollection GSC41Anim = new GradientStopCollection();
            GSC41Anim.Add(new GradientStop(Colors.Red, 0));
            GSC41Anim.Add(AnimStop);
            LGBprop.GradientStops = GSC41Anim;

            // Get the GradientStops value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("GradientStops (With Animation)", GSC41Anim, LGBprop.GradientStops);

            #endregion

            #region Test #5 - The MappingMode Property
            // Usage: BrushMappingMode = LinearGradientBrush.MappingMode (R/W)
            // Notes: Gets or Sets the BrushMappingMode that indicates whether the GradientBrush
            // will map it's coordinates to the entire user space or just to the shape that is being filled.
            CommonLib.LogStatus("Test #5 - The MappingMode Property");

            // Set the MappingMode property.
            LGBprop.MappingMode = BrushMappingMode.RelativeToBoundingBox;

            // Get the MappingMode value as an Int to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("MappingMode", (int)BrushMappingMode.RelativeToBoundingBox, (int)LGBprop.MappingMode);
            #endregion

            #region Test #6 - The SpreadMethod Property
            // Usage: GradientSpreadMethod = LinearGradientBrush.SpreadMethod (R/W)
            // Notes: Gets or Sets a GradientSpreadMethod that indicates how the area outside of the
            // GradientStops will be filled.
            CommonLib.LogStatus("Test #6 - The SpreadMethod Property");

            // Set the SpreadMethod property.
            LGBprop.SpreadMethod = GradientSpreadMethod.Pad;

            // Get the SpreadMethod value as an Int to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("SpreadMethod", (int)GradientSpreadMethod.Pad, (int)LGBprop.SpreadMethod);
            #endregion

            #region Test #7 - The Opacity Property
            // Usage: double = LinearGradientBrush.Opacity (R/W)
            // Notes: Gets or Sets the Opacity value of the Brush.
            CommonLib.LogStatus("Test #7 - The Opacity Property");

            // Set the Opacity property.
            LGBprop.Opacity = 0.8;

            // Get the Opacity value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("Opacity", LGBprop.Opacity, 0.8);

            // Add an Opacity Animation to the Brush
            LGBprop.BeginAnimation(LinearGradientBrush.OpacityProperty, AnimDouble);

            // Get the Opacity value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("Opacity (With Animation)", LGBprop.Opacity, 0.8);
            #endregion

            #region Test #8 - The Transform Property
            // Usage: Transform = LinearGradientBrush.Transform (R/W)
            // Notes: Gets or Sets the Transform for the Brush.
            CommonLib.LogStatus("Test #8 - The Transform Property");

            // Set the Transform property.
            LGBprop.Transform = new ScaleTransform(0.5, 0.5, 100, 100);

            // Get the Transform value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareMatrix("Transform", LGBprop.Transform, 0.5, 0, 0, 0.5, 50, 50);

            // Set the Transform property to an Animated Transform.
            LGBprop.Transform = AnimTransform;

            // Get the Transform value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareMatrix("Transform (With Animation)", LGBprop.Transform, 1, 0, 0, 1, 0, 0);
            #endregion

            #region Test #9 - The RelativeTransform Property
            // Usage: Transform = LinearGradientBrush.RelativeTransform (R/W)
            // Notes: Gets or Sets the Transform for the Brush relative to the object it is filling.
            CommonLib.LogStatus("Test #9 - The RelativeTransform Property");

            // Use the RelativeTransform property to set the Brush Transform relative to the object it is filling.
            LGBprop.RelativeTransform = new ScaleTransform(2, 2, .5, .5);

            // Get the RelativeTransform value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareMatrix("RelativeTransform", LGBprop.RelativeTransform, 2, 0, 0, 2, -.5, -.5);

            // Set the RelativeTransform property to an Animated Transform.
            LGBprop.RelativeTransform = AnimTransform;

            // Get the Transform value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareMatrix("RelativeTransform (With Animation)", LGBprop.RelativeTransform, 1, 0, 0, 1, 0, 0);
            #endregion

            #region Test #10 - The CanFreeze Property
            // Usage: bool = LinearGradientBrush.CanFreeze (Read Only)
            // Notes: Returns a bool that indicates whether the Brush is animatable.
            CommonLib.LogStatus("Test #10 - The CanFreeze Property");

            LinearGradientBrush LGB210 = new LinearGradientBrush(Colors.Blue, Colors.Red, 90.0);

            // Check the CanFreeze value of a non-animated Brush.
            _class_testresult &= _helper.CompareProp("CanFreeze", LGB210.CanFreeze, true);

            // Add an Animation to the Brush
            LGB210.BeginAnimation(LinearGradientBrush.StartPointProperty, AnimPoint);

            // Check the CanFreeze value of an animated Brush.
            _class_testresult &= _helper.CompareProp("CanFreeze (Animated Brush)", LGB210.CanFreeze, false);
            #endregion

            #region Test #11 - Basic rendering with a LinearGradientBrush
            // Just fill the surface with a LinearGradientBrush to see if we crash or not.
            CommonLib.LogStatus("Test #11 - Basic rendering with a LinearGradientBrush");

            // Clear all Animations before rendering
            LGBprop.BeginAnimation(LinearGradientBrush.StartPointProperty, null);
            LGBprop.BeginAnimation(LinearGradientBrush.EndPointProperty, null);
            LGBprop.GradientStops[1].BeginAnimation(GradientStop.OffsetProperty, null);
            LGBprop.BeginAnimation(LinearGradientBrush.OpacityProperty, null);
            LGBprop.Transform = null;
            LGBprop.RelativeTransform = null;

            DC.DrawRectangle(LGBprop, null, new System.Windows.Rect(m_top, m_left, m_width, m_height));
            #endregion
            #endregion End Of SECTION II

            #region SECTION III - METHODS
            CommonLib.LogStatus("***** SECTION III - METHODS *****");

            #region Test #1 - The Copy Method
            // Usage: LinearGradientBrush = LinearGradientBrush.Clone()
            // Notes: Returns a LinearGradientBrush that is a copy of this LinearGradientBrush
            CommonLib.LogStatus("Test #1 - The Copy Method");

            // Create a LinearGradientBrush.
            LinearGradientBrush LGB31a = new LinearGradientBrush(Colors.Blue, Colors.Yellow, 90.0);

            // Use the Copy method to create a new LinearGradientBrush that has the same value.
            LinearGradientBrush LGB31b = LGB31a.Clone();

            // Check both LinearGradientBrushes for equality by comparing their property values
            if ((LGB31a.StartPoint == LGB31b.StartPoint) && (LGB31a.EndPoint == LGB31b.EndPoint) &&
            (_helper.CompareGradientStopCollections(LGB31a.GradientStops, LGB31b.GradientStops)))
            {
                CommonLib.LogStatus("Pass: Copy - LinearGradientBrush properties have the same value");
            }
            else
            {
                CommonLib.LogFail("Fail: Copy - LinearGradientBrush properties do not have the same value");
                _class_testresult &= false;
            }
            #endregion

            #region Test #2 - The CloneCurrentValue Method
            // Usage: LinearGradientBrush = LinearGradientBrush.CloneCurrentValue()
            // Notes: Returns a new Brush that has the same CurrentValue as this Brush
            CommonLib.LogStatus("Test #2 - The CloneCurrentValue Method");

            // Create a LinearGradientBrush.
            LinearGradientBrush LGB32a = new LinearGradientBrush(Colors.Green, Colors.Red, 0.0);

            // Use the CloneCurrentValue method to create a new LinearGradientBrush that has the same current value.
            LinearGradientBrush LGB32b = LGB32a.CloneCurrentValue();

            // Check both LinearGradientBrushes for equality by comparing their property values
            if ((LGB32a.StartPoint == LGB32b.StartPoint) && (LGB32a.EndPoint == LGB32b.EndPoint) &&
                 (_helper.CompareGradientStopCollections(LGB32a.GradientStops, LGB32b.GradientStops)))
            {
                CommonLib.LogStatus("Pass: CloneCurrentValue - LinearGradientBrush properties have the same value");
            }
            else
            {
                CommonLib.LogFail("Fail: CloneCurrentValue - LinearGradientBrush properties do not have the same value");
                _class_testresult &= false;
            }
            #endregion

            #region Test #3 - The GradientStops.Add Method
            // Usage: LinearGradientBrush.GradientStops.Add(new GradientStop(Color, float64))
            // Notes: Convenience Method for adding a new GradientStop to the GradientStopCollection.
            CommonLib.LogStatus("Test #3 - The GradientStops.Add Method");

            // Create a LinearGradientBrush.
            LinearGradientBrush LGB33 = new LinearGradientBrush(Colors.Red, Colors.Blue, 90.0);

            // Use the GradientStops.Add Method
            LGB33.GradientStops.Add(new GradientStop(Colors.Green, 0.5));

            // Create a GradientStopCollection to compare with the Brush
            GradientStopCollection GSCExpected = new GradientStopCollection();

            GSCExpected.Add(new GradientStop(Colors.Red, 0.0));
            GSCExpected.Add(new GradientStop(Colors.Blue, 1.0));
            GSCExpected.Add(new GradientStop(Colors.Green, 0.5));

            // Get the GradientStopCollection from the Brush
            GradientStopCollection GSC33 = LGB33.GradientStops;

            // Check that the GradientStop was added correctly
            _class_testresult &= _helper.CompareProp("GradientStops.Add", GSC33, GSCExpected);
            #endregion

            #region ToString Tests

            #region Test #4 - The ToString Method
            // Usage: string = LinearGradientBrush.ToString()
            // Notes: Persists the Brush in a string.  If the Brush is not a "simple brush" then this should currently
            // return a System Not Supported exception.
            CommonLib.LogStatus("Test #4 - The ToString Method");

            // Use the ToString method on a SimpleBrush
            SimpleBrushString = SimpleBrush.ToString();
            _class_testresult &= _helper.CompareProp("ToString - Simple Brush", SimpleBrushString, "System.Windows.Media.LinearGradientBrush");

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
            // Usage: string = LinearGradientBrush.ToString(IFormatProvider)
            // Notes: Persists the Brush in a string according to the IFormatProvider.  If the Brush is not a "simple brush" then this should currently
            // return a System Not Supported exception.
            CommonLib.LogStatus("Test #5 - The ToString(IFormatProvider) Method");

            // Use the ToString method on a SimpleBrush
            SimpleBrushString = SimpleBrush.ToString(default(System.IFormatProvider));
            _class_testresult &= _helper.CompareProp("ToString(IFormatProvider) - Simple Brush", SimpleBrushString, "System.Windows.Media.LinearGradientBrush");

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
            #endregion ToString Tests
            #endregion End Of SECTION III

            #region SECTION IV - INTERNAL METHODS
            CommonLib.LogStatus("***** SECTION IV - INTERNAL METHODS *****");

            #region Test #1 - The IsSimpleBrush Method
            // Usage: bool = LinearGradientBrush.IsSimpleBrush()
            // Notes: Returns a bool that indicates whether the Brush is "simple" enough to
            // be converted to a String or Serialized.
            CommonLib.LogStatus("Test #1 - The IsSimpleBrush Method");

            MyTypeConverter NonSimpleTypeConverter = new MyTypeConverter(NonSimpleBrush);

            Comp2 = BConverter.CanConvertTo(NonSimpleTypeConverter, ValidType.GetType());
            _class_testresult &= _helper.CompareProp("IsSimpleBrush - Non Simple", Comp2, false);
            #endregion

            #region Test #2 - The ConvertToString Method
            // Usage: string LinearGradientBrush.ConvertToString(IFormatProvider)
            // Notes: Returns a string representation of the Brush.  If the Brush is not a
            // simple brush then a "not supported exception" is raised.
            CommonLib.LogStatus("Test #2 - The ConvertToString Method");
            SimpleBrushString = (String)BConverter.ConvertTo(null, Cinfo, SimpleBrush, ValidType.GetType());
            _class_testresult &= _helper.CompareProp("ConvertToString - Simple Brush", SimpleBrushString, "System.Windows.Media.LinearGradientBrush");
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

            #region Test #1 - The EndPointProperty Property
            // Usage: DependencyProperty = LinearGradientBrush.EndPointProperty (Read only)
            // Notes: Returns a Dependency Property for EndPoint.
            CommonLib.LogStatus("Test #1 - The EndPointProperty Property");

            System.Windows.DependencyProperty EndPointDp = LinearGradientBrush.EndPointProperty;

            _class_testresult &= _helper.CompareProp("EndPointProperty", (EndPointDp.OwnerType.ToString() + "::" + EndPointDp.PropertyType.ToString()), (_objectType + "::" + typeof(Point).ToString()));
            #endregion

            #region Test #2 - The StartPointProperty Property
            // Usage: DependencyProperty = LinearGradientBrush.StartPointProperty (Read only)
            // Notes: Returns a Dependency Property for StartPoint.
            CommonLib.LogStatus("Test #2 - The StartPointProperty Property");

            System.Windows.DependencyProperty StartPointDp = LinearGradientBrush.StartPointProperty;

            _class_testresult &= _helper.CompareProp("StartPointProperty", (StartPointDp.OwnerType.ToString() + "::" + StartPointDp.PropertyType.ToString()), (_objectType + "::" + typeof(Point).ToString()));
            #endregion

            #region Test #3 - The ColorInterpolationModeProperty Property
            // Usage: DependencyProperty = LinearGradientBrush.ColorInterpolationModeProperty (Read only)
            // Notes: Returns a Dependency Property for ColorInterpolationMode
            CommonLib.LogStatus("Test #3 - The ColorInterpolationModeProperty Property");

            System.Windows.DependencyProperty ColorInterpolationModeDp = LinearGradientBrush.ColorInterpolationModeProperty;

            _class_testresult &= _helper.CompareProp("ColorInterpolationModeProperty", (ColorInterpolationModeDp.OwnerType.ToString() + "::" + ColorInterpolationModeDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(ColorInterpolationMode).ToString()));
            #endregion

            #region Test #4 - The GradientStopsProperty Property
            // Usage: DependencyProperty = LinearGradientBrush.GradientStopsProperty (Read only)
            // Notes: Returns a Dependency Property for GradientStops
            CommonLib.LogStatus("Test #4 - The GradientStopsProperty Property");

            System.Windows.DependencyProperty GradientStopsDp = LinearGradientBrush.GradientStopsProperty;

            _class_testresult &= _helper.CompareProp("GradientStopsProperty", (GradientStopsDp.OwnerType.ToString() + "::" + GradientStopsDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(GradientStopCollection).ToString()));
            #endregion

            #region Test #5 - The MappingModeProperty Property
            // Usage: DependencyProperty = LinearGradientBrush.MappingModeProperty (Read only)
            // Notes: Returns a Dependency Property for MappingMode
            CommonLib.LogStatus("Test #5 - The MappingModeProperty Property");

            System.Windows.DependencyProperty MappingModeDp = LinearGradientBrush.MappingModeProperty;

            _class_testresult &= _helper.CompareProp("MappingModeProperty", (MappingModeDp.OwnerType.ToString() + "::" + MappingModeDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(BrushMappingMode).ToString()));
            #endregion

            #region Test #6 - The SpreadMethodProperty Property
            // Usage: DependencyProperty = LinearGradientBrush.SpreadMethod (Read only)
            // Notes: Returns a Dependency Property for SpreadMethod
            CommonLib.LogStatus("Test #6 - The SpreadMethodProperty Property");

            System.Windows.DependencyProperty SpreadMethodDp = LinearGradientBrush.SpreadMethodProperty;

            _class_testresult &= _helper.CompareProp("SpreadMethodProperty", (SpreadMethodDp.OwnerType.ToString() + "::" + SpreadMethodDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(GradientSpreadMethod).ToString()));
            #endregion

            #region Test #7 - The OpacityProperty Property
            // Usage: DependencyProperty = LinearGradientBrush.Opacity (Read only)
            // Notes: Returns a Dependency Property for Opacity.
            CommonLib.LogStatus("Test #7 - The OpacityProperty Property");

            System.Windows.DependencyProperty OpacityDp = LinearGradientBrush.OpacityProperty;

            _class_testresult &= _helper.CompareProp("OpacityProperty", (OpacityDp.OwnerType.ToString() + "::" + OpacityDp.PropertyType.ToString()), (BaseClassType + "::" + typeof(Double).ToString()));
            #endregion

            #region Test #8 - The TransformProperty Property
            // Usage: DependencyProperty = LinearGradientBrush.TransformProperty (Read only)
            // Notes: Returns a Dependency Property for Transform.
            CommonLib.LogStatus("Test #8 - The TransformProperty Property");

            System.Windows.DependencyProperty TransformDp = LinearGradientBrush.TransformProperty;

            _class_testresult &= _helper.CompareProp("TransformProperty", (TransformDp.OwnerType.ToString() + "::" + TransformDp.PropertyType.ToString()), (BaseClassType + "::" + typeof(Transform).ToString()));
            #endregion

            #region Test #9 - The RelativeTransformProperty Property
            // Usage: DependencyProperty = LinearGradientBrush.RelativeTransformProperty (Read only)
            // Notes: Returns a Dependency Property for RelativeTransform.
            CommonLib.LogStatus("Test #9 - The RelativeTransformProperty Property");

            System.Windows.DependencyProperty RelativeTransformDp = LinearGradientBrush.RelativeTransformProperty;

            _class_testresult &= _helper.CompareProp("RelativeTransformProperty", (RelativeTransformDp.OwnerType.ToString() + "::" + RelativeTransformDp.PropertyType.ToString()), (BaseClassType + "::" + typeof(Transform).ToString()));
            #endregion
            #endregion End Of SECTION V

            CommonLib.LogTest("Result for:" + _objectType);
        }

        private bool _class_testresult;
        private Type _objectType;
        private HelperClass _helper;
    }
}
