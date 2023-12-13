// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test all public API's in the GradientStop class
//
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Globalization;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class WCP_GradientStopClass : ApiTest
    {
        //--------------------------------------------------------------------

        public WCP_GradientStopClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(GradientStop);
            _helper = new HelperClass();
            Update();
        }

        //--------------------------------------------------------------------

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus(_objectType.ToString());

            //Test variables
            char separator = _helper.ValueSeparator;
            string compareGradientStopString;

            // Animations to be used for testing Properties
            ColorAnimation AnimColor = new ColorAnimation(Colors.Fuchsia, Colors.Yellow, new System.Windows.Duration(TimeSpan.FromMilliseconds(1000)));
            DoubleAnimation AnimDouble = new DoubleAnimation(0, 1.0, new System.Windows.Duration(TimeSpan.FromMilliseconds(0)), FillBehavior.Stop);


            #region SECTION I - CONSTRUCTORS
            CommonLib.LogStatus("***** SECTION I - CONSTRUCTORS *****");

            #region Test #1 - Default Constructor
            // Usage: GradientStop()
            // Notes: Default Constructor creates an empty GradientStop.
            CommonLib.LogStatus("Test #1 - Default Constructor");

            // Create a GradientStop 
            GradientStop GS11 = new GradientStop();

            // Confirm that a GradientStop was created successfully.
            _class_testresult &= _helper.CheckType(GS11, _objectType);
            #endregion

            #region Test #2 - Constructor with a Color and a double
            // Usage: GradientStop(Color, float64)
            // Notes: Constructor creates a new GradientStop object with a Color and
            // a float for the offset.
            CommonLib.LogStatus("Test #2 - Constructor with two Colors and a double");

            // Create a GradientStop
            GradientStop GSprop = new GradientStop(Colors.Blue, 0.0);

            // Confirm that a GradientStop was created successfully.
            _class_testresult &= _helper.CheckType(GSprop, _objectType);
            #endregion
            #endregion End Of SECTION I

            #region SECTION II - PROPERTIES
            CommonLib.LogStatus("***** SECTION II - PROPERTIES *****");

            #region Test #1 - The Color Property
            // Usage: Color = GradientStop.Color (R/W)
            // Notes: Gets or Sets the Color for this GradientStop.
            CommonLib.LogStatus("Test #1 - The Color Property");

            // Set the Color property.
           Color C21 = Colors.Red;
            GSprop.Color = C21;

            // Get the Color value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("System.Windows.Media.Color", GSprop.Color, C21);

            // Add a Color Animation to the GradientStop
            GSprop.BeginAnimation(GradientStop.ColorProperty, AnimColor);

            // Red value should be 1.0, Green and Blue values should be between 0.0 and 1.0
            _class_testresult &= _helper.CompareColorWithRange("Color (With Animation)=", GSprop.Color, 0.0, 1.0);
            #endregion

            #region Test #2 - The Offset Property
            // Usage: float64 = GradientStop.Offset (R/W)
            // Notes: Gets or Sets the offset (as a percentage) from the beginning
            // of the area to be filled. 
            CommonLib.LogStatus("Test #2 - The Offset Property");

            // Set the Offset property.
            GSprop.Offset = 0.4;

            // Get the Offset property and Check the Offset value to assure that it was
            // the one that was set.
            _class_testresult &= _helper.CompareProp("Offset", GSprop.Offset, 0.4);

            // Add an Offset Animation to the GradientStop
            GSprop.BeginAnimation(GradientStop.OffsetProperty, AnimDouble);

            // Get the Offset value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("Offset (With Animation)", GSprop.Offset, 0.4);
            #endregion

            #region Test #3 - The CanFreeze Property
            // Usage: bool = GradientStop.CanFreeze (Read Only)
            // Notes: Returns a bool that indicates whether the GradientStop is animatable.
            CommonLib.LogStatus("Test #3 - The CanFreeze Property");

            GradientStop GS23 = new GradientStop(Colors.Yellow, 1.0);

            // Check the CanFreeze value of a non-animated GradientStop.
            _class_testresult &= _helper.CompareProp("CanFreeze", GS23.CanFreeze, true);

            // Add an Animation to the GradientStop
            GS23.BeginAnimation(GradientStop.ColorProperty, AnimColor);

            // Check the CanFreeze value of an animated GradientStop.
            _class_testresult &= _helper.CompareProp("CanFreeze (Animated)", GS23.CanFreeze, false);
            #endregion

            #region Test #4 - Basic rendering with a GradientStop
            // Create a GradientBrush, add a GradientStop and fill a rectangle to see
            // if we crash or not.
            CommonLib.LogStatus("Test #4 - Basic rendering with a GradientStop");

            // Clear Animations before rendering.
            GSprop.BeginAnimation(GradientStop.ColorProperty, null);
            GSprop.BeginAnimation(GradientStop.OffsetProperty, null);
            LinearGradientBrush brush = new LinearGradientBrush(Colors.Blue, Colors.White, 0.0);
            brush.GradientStops.Add(new GradientStop(GSprop.Color, GSprop.Offset));
            DC.DrawRectangle(brush, null, new System.Windows.Rect(m_top, m_left, m_width, m_height));
            #endregion
            #endregion End Of SECTION II

            #region SECTION III - METHODS
            CommonLib.LogStatus("***** SECTION III - METHODS *****");

            #region Test #1 - The Copy Method
            // Usage: GradientStop = GradientStop.Clone()
            // Notes: Returns a GradientStop that is a copy of this GradientStop
            CommonLib.LogStatus("Test #1 - The Copy Method");

            // Create a GradientStop.
            GradientStop GS31a = new GradientStop(Colors.Yellow, 0.5);

            // Use the Copy method to create a new GradientStop that has the same value.
            GradientStop GS31b = GS31a.Clone();

            // Check both GradientStopes for equality by comparing their property values
            if ((GS31a.Color == GS31b.Color) && (GS31a.Offset == GS31b.Offset))
            {
                CommonLib.LogStatus("Pass: Copy - GradientStop properties have the same value");
            }
            else
            {
                CommonLib.LogFail("Fail: Copy - GradientStop properties do not have the same value");
                _class_testresult &= false;
            }
            #endregion

            #region Test #2 - The CloneCurrentValue Method
            // Usage: GradientStop = GradientStop.CloneCurrentValue()
            // Notes: Returns a new Brush that has the same CurrentValue as this Brush
            CommonLib.LogStatus("Test #2 - The CloneCurrentValue Method");

            // Create a GradientStop.
            GradientStop GS32a = new GradientStop(Colors.Green, 0.0);

            // Use the CloneCurrentValue method to create a new GradientStop that has the same current value.
            GradientStop GS32b = GS32a.CloneCurrentValue();

            // Check both GradientStopes for equality by comparing their property values
            if ((GS32a.Color == GS32b.Color) && (GS32a.Offset == GS32b.Offset))
            {
                CommonLib.LogStatus("Pass: CloneCurrentValue - GradientStop properties have the same value");
            }
            else
            {
                CommonLib.LogFail("Fail: CloneCurrentValue - GradientStop properties do not have the same value");
                _class_testresult &= false;
            }
            #endregion

            #region Test #3 - The ToString Method
            // Usage: string = GradientStop.ToString()
            // Notes: Creates a string representation of this object based on the current
            // culture.
            CommonLib.LogStatus("Test #3 - The ToString Method");

            // Create a GradientStop.
            GradientStop GS33 = new GradientStop(Colors.Green, 0.0);
            //Construct the comparison string according to the current culture
            compareGradientStopString = GS33.Color.ToString(CultureInfo.InvariantCulture)
                                        + separator + GS33.Offset.ToString(CultureInfo.InvariantCulture);
            compareGradientStopString = _helper.ToLocale(compareGradientStopString, CultureInfo.CurrentCulture);
            _class_testresult &= _helper.CompareProp("ToString", GS33.ToString(), compareGradientStopString);
            #endregion

            #region Test #4 - The ToString Method (IFormatProvider)
            // Usage: string = GradientStop.ToString(IFormatProvider)
            // Notes: Creates a string representation of this object based on the IFormatProvider
            // passed in.  If the provider is null, the CurrentCulture is used.
            CommonLib.LogStatus("Test #4 - The ToString Method (IFormatProvider)");

            // Create a GradientStop.
            GradientStop GS34 = new GradientStop(Colors.Red, 0.0);
            //Construct the comparison string according to the current culture
            compareGradientStopString = GS34.Color.ToString(CultureInfo.InvariantCulture)
                                        + separator + GS34.Offset.ToString(CultureInfo.InvariantCulture);
            compareGradientStopString = _helper.ToLocale(compareGradientStopString, CultureInfo.CurrentCulture);
            // Use the ToString method to create a new GradientStop that has the same current value.
            _class_testresult &= _helper.CompareProp("ToString", GS34.ToString(default(System.IFormatProvider)), compareGradientStopString);
            #endregion
            #endregion End Of SECTION III

            #region SECTION IV - STATIC PROPERTIES
            CommonLib.LogStatus("***** SECTION IV - STATIC PROPERTIES *****");

            #region Test #1 - The ColorProperty Property
            // Usage: DependencyProperty = GradientStop.ColorProperty (Read only)
            // Notes: Returns a Dependency Property for Color.
            CommonLib.LogStatus("Test #1 - The ColorProperty Property");

            System.Windows.DependencyProperty ColorDp = GradientStop.ColorProperty;

            _class_testresult &= _helper.CompareProp("ColorProperty", (ColorDp.OwnerType.ToString() + "::" + ColorDp.PropertyType.ToString()), (_objectType + "::" + typeof(Color).ToString()));
            #endregion

            #region Test #2 - The OffsetProperty Property
            // Usage: DependencyProperty = GradientStop.Offset (Read only)
            // Notes: Returns a Dependency Property for Offset.
            CommonLib.LogStatus("Test #2 - The OffsetProperty Property");

            System.Windows.DependencyProperty OffsetDp = GradientStop.OffsetProperty;

            _class_testresult &= _helper.CompareProp("OffsetProperty", (OffsetDp.OwnerType.ToString() + "::" + OffsetDp.PropertyType.ToString()), (_objectType + "::" + typeof(System.Double).ToString()));
            #endregion
            #endregion End Of SECTION IV

            CommonLib.LogTest("Result for :" + _objectType);
        }

        //--------------------------------------------------------------------

        private bool _class_testresult;
        private Type _objectType;
        private HelperClass _helper;
    }
}
