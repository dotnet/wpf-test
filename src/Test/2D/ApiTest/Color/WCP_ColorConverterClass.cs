// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test all public API's in the ColorConverter class
//
using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class WCP_ColorConverterClass : ApiTest
    {
        public WCP_ColorConverterClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(ColorConverter);
            _helper = new HelperClass();
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus(_objectType.ToString());

            // Shared test variables
            SolidColorBrush brush = new SolidColorBrush();
            Rect rect1 = new Rect(new Point(10, 10), new Point(170, 90));
            Rect rect2a = new Rect(new Point(10, 94), new Point(88, 154));
            Rect rect2b = new Rect(new Point(92, 94), new Point(170, 154));

            // Expected exception strings
            string null_value_exception = "ColorConverter cannot convert from (null).";
            string null_type_exception = "Value cannot be null." + (char)13 + (char)10 + "Parameter name: destinationType";
            string CT_wrong_type_exception = "Expected object of type 'Color'.";
            //Getting the exception message directly from the ExceptionStringTable in PresentationCore.dll in order
            //to the trouble of prepare all of those localized exception strings running on different locales.
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager("FxResources.PresentationCore.SR", typeof(Color).Assembly);

            #region SECTION I - CONSTRUCTORS
            CommonLib.LogStatus("***** SECTION I - CONSTRUCTORS *****");

            #region Test #1 - Default Constructor
            // Usage: ColorConverter()
            // Notes: Constructor creates a default ColorConverter object.
            CommonLib.LogStatus("Test #1 - Default Constructor");

            // Create a ColorConverter
           ColorConverter CCProp = new ColorConverter();

            // Confirm that a ColorConverter object was created and log result.
            _class_testresult &= _helper.CheckType(CCProp, _objectType);
            #endregion
            #endregion End Of SECTION I

            #region SECTION II - METHODS
            CommonLib.LogStatus("***** SECTION II - METHODS *****");

            #region Test #1 - The CanConvertFrom Method
            // Usage: bool = ColorConverter.CanConvertFrom(System.ComponentModel.ITypeDescriptorContext, System.Type)
            // Notes: Returns a bool that indicates whether the Type can be converted with the
            // specified converter.
            CommonLib.LogStatus("Test #1 - The CanConvertFrom Method");

            // CanConvertFrom reads the object type and determines whether that type can be
            // converted
            string valid_type = "Yellow";
            SolidColorBrush invalid_type = new SolidColorBrush();
            bool can_convert_from = true;

            // Test a valid Type using the CanConvertFrom Method.
            can_convert_from = CCProp.CanConvertFrom(null, valid_type.GetType());
            _class_testresult &= _helper.CompareProp("CanConvertFrom - String", can_convert_from, true);

            // Test an invalid Type using the CanConvertFrom Method.
            can_convert_from = CCProp.CanConvertFrom(null, invalid_type.GetType());
            _class_testresult &= _helper.CompareProp("CanConvertFrom - Invalid Type", can_convert_from, false);
            #endregion

            #region Test #2 - The CanConvertTo Method
            // Usage: bool = ColorConverter.CanConvertTo(System.ComponentModel.ITypeDescriptorContext, System.Type)
            // Notes: Returns a bool that indicates whether this object can be converted to
            // the Type of the specified converter.
            CommonLib.LogStatus("Test #2 - The CanConvertTo Method");

            // CanConvertTo reads the object type and determines whether that type can be
            // converted
            bool can_convert_to = true;

            // Test a valid Type using the CanConvertTo Method.
            can_convert_to = CCProp.CanConvertTo(null, valid_type.GetType());
            _class_testresult &= _helper.CompareProp("CanConvertTo - String", can_convert_to, true);

            // Test an invalid Type using the CanConvertTo Method.
            can_convert_to = CCProp.CanConvertTo(null, invalid_type.GetType());
            _class_testresult &= _helper.CompareProp("CanConvertTo - Invalid Type", can_convert_to, false);
            #endregion

            #region Test #3 - The ConvertFrom Method
            // Usage: object = ColorConverter.ConvertFrom(System.ComponentModel.ITypeDescriptorContext, System.Globalization.CultureInfo, object)
            // Notes: Returns an object that is converted from a string descriptor.
            CommonLib.LogStatus("Test #3 - The ConvertFrom Method");

            // NOTE: The ConvertFrom Method will only accept an
            // Invariant CultureInfo object.
            System.Globalization.CultureInfo Cinfo = System.Globalization.CultureInfo.InvariantCulture;

            // Use the ConvertFrom Method to convert a string into a Color type.
           Color color = (Color)CCProp.ConvertFrom(null, Cinfo, "SeaGreen");
            _class_testresult &= _helper.CheckType(color, typeof(Color));

            // Fill a Rectangle with the Color
            brush.Color = color;
            DC.DrawRectangle(brush, null, rect1);

            // Calling the ConvertFrom Method with a null string should generate the
            // expected exception.
           Color null_color;
            try
            {
                null_color = (Color)CCProp.ConvertFrom(null, Cinfo, null);
            }
            catch (System.Exception e)
            {
                if (e.Message == null_value_exception)
                {
                    CommonLib.LogStatus("Pass: ConvertFrom with a Null string returns the expected exception");
                }
                else
                {
                    CommonLib.LogFail("Fail: ConvertFrom with a Null string returns " + e.Message + " should be " + null_value_exception);
                    _class_testresult &= false;
                }
            }

            // Calling the ConvertFrom Method with the wrong type of object should
            // generate the expected exception.
            try
            {
                null_color = (Color)CCProp.ConvertFrom(null, Cinfo, Brushes.Red);
            }
            catch (System.Exception e)
            {
                //Getting the exception message directly from the ExceptionStringTable in PresentationCore.dll in order
                //to the trouble of prepare all of those localized exception strings running on different locales.
                string message = rm.GetString("General_BadType");
                message = string.Format(message, "ConvertFrom");
                message += "\r\nParameter name: value";
                if (String.Compare(e.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Pass: ConvertFrom with the wrong type of object returns the expected exception");
                }
                else
                {
                    CommonLib.LogFail("Fail: ConvertFrom the wrong type of object returns " + e.Message + " should be " + message);
                    _class_testresult &= false;
                }
            }
            #endregion

            #region Test #4 - The ConvertTo Method
            // Usage: object = ColorConverter.ConvertTo(System.ComponentModel.ITypeDescriptorContext, System.Globalization.CultureInfo, object, System.Type)
            // Notes: Returns an object that is converted to the specified type.
            CommonLib.LogStatus("Test #4 - The ConvertTo Method");

            // Use the ConvertTo Method to convert certain a Color type into a string.
            String colorstring = (String)CCProp.ConvertTo(null, Cinfo, Colors.Red, valid_type.GetType());
            _class_testresult &= _helper.CompareProp("ConvertTo", colorstring, "#FFFF0000");

            // Calling the ConvertTo Method with a Type argument that is null 
            // should generate the expected exception.
            String nullstring;
            try
            {
                nullstring = (String)CCProp.ConvertTo(null, Cinfo, Colors.Red, null);
            }
            catch (System.Exception e)
            {
                if (e.Message == null_type_exception)
                {
                    CommonLib.LogStatus("Pass: ConvertTo with a Null Type returns the expected exception");
                }
                else
                {
                    CommonLib.LogFail("Fail: ConvertTo with a Null Type returns " + e.Message + " should be " + null_type_exception);
                    _class_testresult &= false;
                }
            }

            // Calling the ConvertTo Method with an object argument that is null 
            // should generate the expected exception.
            try
            {
                nullstring = (String)CCProp.ConvertTo(null, Cinfo, null, valid_type.GetType());
            }
            catch (System.Exception e)
            {
                if (e.Message == null_value_exception)
                {
                    CommonLib.LogStatus("Pass: ConvertTo with a Null object returns the expected exception");
                }
                else
                {
                    CommonLib.LogFail("Fail: ConvertTo with a Null object returns " + e.Message + " should be " + null_value_exception);
                    _class_testresult &= false;
                }
            }

            // Calling the ConvertTo Method with the wrong type of object should
            // generate the expected exception.
            try
            {
                nullstring = (String)CCProp.ConvertTo(null, Cinfo, Brushes.Red, valid_type.GetType());
            }
            catch (System.Exception e)
            {
                if (e.Message == CT_wrong_type_exception)
                {
                    CommonLib.LogStatus("Pass: ConvertTo with the wrong type of object returns the expected exception");
                }
                else
                {
                    CommonLib.LogFail("Fail: ConvertTo with the wrong type of object returns " + e.Message + " should be " + CT_wrong_type_exception);
                    _class_testresult &= false;
                }
            }
            #endregion
            #endregion End Of SECTION II

            #region SECTION III - STATIC METHODS
            CommonLib.LogStatus("***** SECTION III - STATIC METHODS *****");

            #region Test #1 - The ConvertFromString Method
            // Usage: object = ColorConverter.ConvertFromString(string)
            // Notes: Returns an object that is converted to the specified type.
            CommonLib.LogStatus("Test #1 - The ConvertFromString Method");

            bool test31_result = true;

            // Use the ConvertFromString Method to create Color objects from different
            // types of Color strings.
           Color C31a = (Color)ColorConverter.ConvertFromString("Red");
            test31_result &= _helper.CompareColorPropFloats("ConvertFromString - Known Color", C31a, 1, 1, 0, 0);

            // Fill a Rectangle with the Color
            SolidColorBrush brush31a = new SolidColorBrush();
            brush31a.Color = C31a;
            DC.DrawRectangle(brush31a, null, rect2a);

           Color C31b = (Color)ColorConverter.ConvertFromString("#FFAA11EE");
            test31_result &= _helper.CompareColorPropInts("ConvertFromString - Hex String", C31b, 255, 170, 17, 238);

            // Fill a Rectangle with the Color
            SolidColorBrush brush31b = new SolidColorBrush();
            brush31b.Color = C31b;
            DC.DrawRectangle(brush31b, null, rect2b);

            // Calling ConvertFromString with a Null string should return null.
            if (ColorConverter.ConvertFromString(null) == null)
            {
                CommonLib.LogStatus("Pass: ConvertFromString - Null String=Null");
            }
            else
            {
                CommonLib.LogFail("Fail: ConvertFromString - Null String should equal Null");
                test31_result &= false;
            }

            // Add results for this method to the results for the test.
            _class_testresult &= test31_result;
            #endregion
            #endregion


            CommonLib.LogTest("Result for :" + _objectType);
        }

        private bool _class_testresult;
        private Type _objectType;
        private HelperClass _helper;
    }
}
