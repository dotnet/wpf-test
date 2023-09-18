// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test all public API's in the SolidColorBrush class
//
using System;
using System.ComponentModel.Design.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class WCP_SolidColorBrushClass : ApiTest
    {
        //--------------------------------------------------------------------

        public WCP_SolidColorBrushClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(SolidColorBrush);
            _helper = new HelperClass();
            Update();
        }

        //--------------------------------------------------------------------

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus(_objectType.ToString());

            // Test variables
            BrushConverter BConverter = new BrushConverter();
            System.Globalization.CultureInfo Cinfo = System.Globalization.CultureInfo.InvariantCulture;
            String ValidType = "String";
            InstanceDescriptor ValidType2 = new InstanceDescriptor(null, null);
            bool Comp1, Comp2;
            String SimpleBrushString, NonSimpleBrushString;
            Type ParentClassType = typeof(Brush);

            // Expected exception messages
            String ConvertToNotSupportedException = "Cannot convert to type.";

            // Animations to be used for testing Properties
            ColorAnimation AnimColor = new ColorAnimation(Colors.Fuchsia, Colors.Yellow, new System.Windows.Duration(TimeSpan.FromMilliseconds(1000)));
            DoubleAnimation AnimDouble = new DoubleAnimation(0, 1.0, new System.Windows.Duration(TimeSpan.FromMilliseconds(0)), FillBehavior.Stop);
            Transform AnimTransform = new TranslateTransform(0, 0);
            AnimTransform.BeginAnimation(TranslateTransform.XProperty, AnimDouble);

            // SimpleBrush and Non-SimpleBrush for Serialization method tests
            SolidColorBrush SimpleBrush = new SolidColorBrush(Colors.Blue);
            SolidColorBrush NonSimpleBrush = new SolidColorBrush(Colors.Blue);
            NonSimpleBrush.Transform = new RotateTransform(105.0);

            #region SECTION I - CONSTRUCTORS
            CommonLib.LogStatus("***** SECTION I - CONSTRUCTORS *****");

            #region Test #1 - Default Constructor
            // Usage: SolidColorBrush()
            // Notes: Default Constructor creates an empty SolidColorBrush.
            CommonLib.LogStatus("Test #1 - Default Constructor");

            // Create a SolidColorBrush 
            SolidColorBrush B11 = new SolidColorBrush();

            // Confirm that a SolidColorBrush was created successfully.
            _class_testresult &= _helper.CheckType(B11, _objectType);
            #endregion

            #region Test #2 - Constructor with a Color
            // Usage: SolidColorBrush(Color)
            // Notes: Creates a SolidColorBrush object using a Color argument.
            CommonLib.LogStatus("Test #2 - Constructor with a Color");

            // Create a SolidColorBrush with a Color 
            SolidColorBrush Bprop = new SolidColorBrush(Colors.Blue);

            // Confirm that a SolidColorBrush was created successfully.
            _class_testresult &= _helper.CheckType(Bprop, _objectType);
            #endregion
            #endregion End Of SECTION I

            #region SECTION II - PROPERTIES
            CommonLib.LogStatus("***** SECTION II - PROPERTIES *****");

            #region Test #1 - The Color Property
            // Usage: Color = SolidColorBrush.Color (R/W)
            // Notes: Returns the Brush that is contained in the Pen.
            CommonLib.LogStatus("Test #1 - The Color Property");

            // Use the Color property to set the Brush Color.
            Bprop.Color = Colors.Red;

            // Get the Color value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareColorPropFloats("System.Windows.Media.Color", Bprop.Color, 1.0f, 1.0f, 0.0f, 0.0f);

            // Add a Color Animation to the Brush
            Bprop.BeginAnimation(SolidColorBrush.ColorProperty, AnimColor);

            // Red value should be 1.0, Green and Blue values should be between 0.0 and 1.0
            _class_testresult &= _helper.CompareColorWithRange("Color (With Animation)=", Bprop.Color, 0.0, 1.0);
            #endregion

            #region Test #2 - The Opacity Property
            // Usage: double = SolidColorBrush.Opacity (R/W)
            // Notes: Gets or Sets the Opacity value of the Brush.
            CommonLib.LogStatus("Test #2 - The Opacity Property");

            // Use the Opacity property to set the Brush Opacity.
            Bprop.Opacity = 0.5;

            // Get the Opacity value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("Opacity", Bprop.Opacity, 0.5);

            // Add an Opacity Animation to the Brush
            Bprop.BeginAnimation(SolidColorBrush.OpacityProperty, AnimDouble);

            // Get the Opacity value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("Opacity (With Animation)", Bprop.Opacity, 0.5);
            #endregion

            #region Test #3 - The Transform Property
            // Usage: Transform = SolidColorBrush.Transform (R/W)
            // Notes: Gets or Sets the Transform for the Brush.
            CommonLib.LogStatus("Test #3 - The Transform Property");

            // Use the Transform property to set the Brush Transform.
            Bprop.Transform = new ScaleTransform(2, 2, 100, 100);

            // Get the Transform value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareMatrix("Transform", Bprop.Transform, 2, 0, 0, 2, -100, -100);

            // Set the Transform property to an Animated Transform.
            Bprop.Transform = AnimTransform;

            // Get the Transform value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareMatrix("Transform (With Animation)", Bprop.Transform, 1, 0, 0, 1, 0, 0);
            #endregion

            #region Test #4 - The RelativeTransform Property
            // Usage: Transform = SolidColorBrush.RelativeTransform (R/W)
            // Notes: Gets or Sets the Transform for the Brush relative to the object it is filling.
            CommonLib.LogStatus("Test #4 - The RelativeTransform Property");

            // Use the RelativeTransform property to set the Brush Transform relative to the object it is filling.
            Bprop.RelativeTransform = new ScaleTransform(2, 2, .5, .5);

            // Get the RelativeTransform value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareMatrix("RelativeTransform", Bprop.RelativeTransform, 2, 0, 0, 2, -.5, -.5);

            // Set the RelativeTransform property to an Animated Transform.
            Bprop.RelativeTransform = AnimTransform;

            // Get the Transform value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareMatrix("RelativeTransform (With Animation)", Bprop.RelativeTransform, 1, 0, 0, 1, 0, 0);
            #endregion

            #region Test #5 - The CanFreeze Property
            // Usage: bool = SolidColorBrush.CanFreeze (Read Only)
            // Notes: Returns a bool that indicates whether the Brush is animatable.
            CommonLib.LogStatus("Test #5 - The CanFreeze Property");

            SolidColorBrush B25 = new SolidColorBrush(Color.FromScRgb(1.0f, 1.0f, 0f, 0f));

            // Check the CanFreeze value of a non-animated Brush.
            _class_testresult &= _helper.CompareProp("CanFreeze", B25.CanFreeze, true);

            // Add an Animation to the Brush
            B25.BeginAnimation(SolidColorBrush.ColorProperty, AnimColor);

            // Check the CanFreeze value of an animated Brush.
            _class_testresult &= _helper.CompareProp("CanFreeze (Animated Brush)", B25.CanFreeze, false);
            #endregion

            #region Test #6 - Basic rendering with a SolidColorBrush
            // Just fill the surface with a SolidColorBrush to see if we crash or not.
            CommonLib.LogStatus("Test #6 - Basic rendering with a SolidColorBrush");

            // Clear Animations before rendering.
            Bprop.BeginAnimation(SolidColorBrush.ColorProperty, null);
            Bprop.BeginAnimation(SolidColorBrush.OpacityProperty, null);
            Bprop.Transform = new ScaleTransform(2, 2, 100, 100);
            Bprop.RelativeTransform = new ScaleTransform(2, 2, 0.5, 0.5);
            DC.DrawRectangle(Bprop, null, new System.Windows.Rect(m_top, m_left, m_width, m_height));
            #endregion
            #endregion End Of SECTION II

            #region SECTION III - METHODS
            CommonLib.LogStatus("***** SECTION III - METHODS *****");

            #region Test #1 - The Copy Method
            // Usage: SolidColorBrush = SolidColorBrush.Clone()
            // Notes: Returns a SolidColorBrush that is a copy of this SolidColorBrush
            CommonLib.LogStatus("Test #1 - The Copy Method");

            // Create a SolidColorBrush.
            SolidColorBrush B31a = new SolidColorBrush(Colors.Blue);

            // Use the Copy method to create a new SolidColorBrush that has the same value.
            SolidColorBrush B31b = B31a.Clone();

            // Check both SolidColorBrushes for equality by comparing their property values
            if ((B31a.Color == B31b.Color) && (B31a.Opacity == B31b.Opacity) && (B31a.Transform.Value == B31b.Transform.Value))
            {
                CommonLib.LogStatus("Pass: Copy - SolidColorBrush properties have the same value");
            }
            else
            {
                CommonLib.LogFail("Fail: Copy - SolidColorBrush properties do not have the same value");
                _class_testresult &= false;
            }
            #endregion

            #region Test #2 - The CloneCurrentValue Method
            // Usage: SolidColorBrush = SolidColorBrush.CloneCurrentValue()
            // Notes: Returns a new Brush that has the same CurrentValue as this Brush
            CommonLib.LogStatus("Test #2 - The CloneCurrentValue Method");

            // Create a SolidColorBrush.
            SolidColorBrush B32a = new SolidColorBrush(Colors.Green);

            // Use the CloneCurrentValue method to create a new SolidColorBrush that has the same current value.
            SolidColorBrush B32b = B32a.CloneCurrentValue();

            // Check both SolidColorBrushes for equality by comparing their property values
            if ((B32a.Color == B32b.Color) && (B32a.Opacity == B32b.Opacity) && (B32a.Transform.Value == B32b.Transform.Value))
            {
                CommonLib.LogStatus("Pass: CloneCurrentValue - SolidColorBrush properties have the same value");
            }
            else
            {
                CommonLib.LogFail("Fail: CloneCurrentValue - SolidColorBrush properties do not have the same value");
                _class_testresult &= false;
            }
            #endregion

            #region Test #3 - The ToString Method
            // Usage: string = SolidColorBrush.ToString()
            // Notes: Persists the Brush in a string.  If the Brush is not a "simple brush" then this should currently
            // return a System Not Supported exception.
            CommonLib.LogStatus("Test #3 - The ToString Method");

            // Use the ToString method on a SimpleBrush
            SimpleBrushString = SimpleBrush.ToString();
            _class_testresult &= _helper.CompareProp("ToString - Simple Brush", SimpleBrushString, "#FF0000FF");

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

            #region Test #4 - The ToString(IFormatProvider) Method
            // Usage: string = SolidColorBrush.ToString(IFormatProvider)
            // Notes: Persists the Brush in a string according to the IFormatProvider.  If the Brush is not a "simple brush" then this should currently
            // return a System Not Supported exception.
            CommonLib.LogStatus("Test #4 - The ToString(IFormatProvider) Method");

            // Use the ToString method on a SimpleBrush
            SimpleBrushString = SimpleBrush.ToString(default(System.IFormatProvider));
            _class_testresult &= _helper.CompareProp("ToString(IFormatProvider) - Simple Brush", SimpleBrushString, "#FF0000FF");

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
            // Usage: bool = SolidColorBrush.IsSimpleBrush()
            // Notes: Returns a bool that indicates whether the Brush is "simple" enough to
            // be converted to a String or Serialized.
            CommonLib.LogStatus("Test #1 - The IsSimpleBrush Method");

            // Test a valid but non null ITypeDescriptorContext.  This should return
            // the value of Brush.IsSimpleBrush().  Should be True unless the Brush has
            // a non-identity Transform or is non-opaque.
            MyTypeConverter SimpleTypeConverter = new MyTypeConverter(SimpleBrush);
            Comp1 = BConverter.CanConvertTo(SimpleTypeConverter, ValidType.GetType());
            _class_testresult &= _helper.CompareProp("IsSimpleBrush - Simple", Comp1, true);

            MyTypeConverter NonSimpleTypeConverter = new MyTypeConverter(NonSimpleBrush);
            Comp2 = BConverter.CanConvertTo(NonSimpleTypeConverter, ValidType.GetType());
            _class_testresult &= _helper.CompareProp("IsSimpleBrush - Non Simple", Comp2, false);
            #endregion

            #region Test #2 - The ConvertToString Method
            // Usage: string SolidColorBrush.ConvertToString(IFormatProvider)
            // Notes: Returns a string representation of the Brush.  If the Brush is not a
            // simple brush then a "not supported exception" is raised.
            CommonLib.LogStatus("Test #2 - The ConvertToString Method");

            SimpleBrushString = (String)BConverter.ConvertTo(null, Cinfo, SimpleBrush, ValidType.GetType());
            _class_testresult &= _helper.CompareProp("ConvertToString - Simple Brush", SimpleBrushString, "#FF0000FF");

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

            #region Test #1 - The ColorProperty Property
            // Usage: DependencyProperty = SolidColorBrush.ColorProperty (Read only)
            // Notes: Returns a Dependency Property for Color.
            CommonLib.LogStatus("Test #1 - The ColorProperty Property");

            System.Windows.DependencyProperty ColorDp = SolidColorBrush.ColorProperty;

            _class_testresult &= _helper.CompareProp("ColorProperty Owner", ColorDp.OwnerType, _objectType);
            _class_testresult &= _helper.CompareProp("ColorProperty ", ColorDp.PropertyType, typeof(Color));
            #endregion

            #region Test #2 - The OpacityProperty Property
            // Usage: DependencyProperty = SolidColorBrush.Opacity (Read only)
            // Notes: Returns a Dependency Property for Opacity.
            CommonLib.LogStatus("Test #2 - The OpacityProperty Property");

            System.Windows.DependencyProperty OpacityDp = SolidColorBrush.OpacityProperty;

            _class_testresult &= _helper.CompareProp("OpacityProperty", (OpacityDp.OwnerType.ToString() + "::" + OpacityDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(System.Double).ToString()));
            #endregion

            #region Test #3 - The TransformProperty Property
            // Usage: DependencyProperty = SolidColorBrush.TransformProperty (Read only)
            // Notes: Returns a Dependency Property for Transform.
            CommonLib.LogStatus("Test #3 - The TransformProperty Property");

            System.Windows.DependencyProperty TransformDp = SolidColorBrush.TransformProperty;

            _class_testresult &= _helper.CompareProp("TransformProperty", (TransformDp.OwnerType.ToString() + "::" + TransformDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(Transform).ToString()));
            #endregion

            #region Test #4 - The RelativeTransformProperty Property
            // Usage: DependencyProperty = SolidColorBrush.RelativeTransformProperty (Read only)
            // Notes: Returns a Dependency Property for RelativeTransform.
            CommonLib.LogStatus("Test #4 - The RelativeTransformProperty Property");

            System.Windows.DependencyProperty RelativeTransformDp = SolidColorBrush.RelativeTransformProperty;

            _class_testresult &= _helper.CompareProp("RelativeTransformProperty", (RelativeTransformDp.OwnerType.ToString() + "::" + RelativeTransformDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(Transform)));
            #endregion
            #endregion End Of SECTION V

            
            CommonLib.LogTest("Results for :"+_objectType );                    
        }        

        private bool _class_testresult;
        private Type _objectType;
        private HelperClass _helper;
    }
}
