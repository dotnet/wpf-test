// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test all public API's in the BrushConverter class 
//
using System;
//using System.ComponentModel.Design.Serialization;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class WCP_BrushConverterClass : ApiTest
    {
        public WCP_BrushConverterClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(BrushConverter);
            _helper = new HelperClass();
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus(_objectType.ToString());

            // Drawing variables
            Rect rect1 = new Rect(new Point(10, 10), new Point(63, 160));
            Rect rect2 = new Rect(new Point(65, 10), new Point(118, 160));
            Rect rect3 = new Rect(new Point(120, 10), new Point(173, 160));

            // Common variables used in these tests
            System.Globalization.CultureInfo Cinfo = System.Globalization.CultureInfo.InvariantCulture;
            string valid_type = "String";
            SolidColorBrush invalid_type = new SolidColorBrush();
            bool can_convert_from = true;
            bool can_convert_to = true;

            // Expected exception messages
            string wrong_typedescriptorcontext = "Expected object of type 'Brush'." + (char)13 + (char)10 + "Parameter name: context";
            string null_value_exception = "BrushConverter cannot convert from (null).";
            //string CF_wrong_type_exception = "The object passed to 'ConvertFrom' was of an invalid type." + (char)13 + (char)10 + "Parameter name: value";
            string CF_wrong_type_exception = "BrushConverter cannot convert from System.Windows.Media.Color.";
            string null_type_exception = "Value cannot be null." + (char)13 + (char)10 + "Parameter name: destinationType";
            string CT_wrong_type_exception = "Expected object of type 'Brush'." + (char)13 + (char)10 + "Parameter name: value";

            #region SECTION I - CONSTRUCTORS
            CommonLib.LogStatus("***** SECTION I - CONSTRUCTORS *****");

            #region Test #1 - Default Constructor
            // Usage: BrushConverter()
            // Notes: Constructor creates a default BrushConverter object.
            CommonLib.LogStatus("Test #1 - Default Constructor");

            // Create a BrushConverter
            BrushConverter BCProp = new BrushConverter();

            // Confirm that a BrushConverter object was created and log result.
            _class_testresult &= _helper.CheckType(BCProp, _objectType);
            #endregion
            #endregion End Of SECTION I

            #region SECTION II - METHODS
            CommonLib.LogStatus("***** SECTION II - METHODS *****");

            #region Test #1 - The CanConvertFrom Method
            // Usage: bool = BrushConverter.CanConvertFrom(System.ComponentModel.ITypeDescriptorContext, System.Type)
            // Notes: Returns a bool that indicates whether the Type can be converted with the
            // specified converter.
            CommonLib.LogStatus("Test #1 - The CanConvertFrom Method");

            // Test a valid Type using the CanConvertFrom Method.
            can_convert_from = BCProp.CanConvertFrom(null, valid_type.GetType());
            _class_testresult &= _helper.CompareProp("CanConvertFrom - String", can_convert_from, true);

            // Test an invalid Type using the CanConvertFrom Method.
            can_convert_from = BCProp.CanConvertFrom(null, invalid_type.GetType());
            _class_testresult &= _helper.CompareProp("CanConvertFrom - Invalid Type", can_convert_from, false);
            #endregion

            #region Test #2 - The CanConvertTo Method
            // Usage: bool = BrushConverter.CanConvertTo(System.ComponentModel.ITypeDescriptorContext, System.Type)
            // Notes: Returns a bool that indicates whether this object can be converted to
            // the Type of the specified converter.
            CommonLib.LogStatus("Test #2 - The CanConvertTo Method");

            // Test a valid Type using the CanConvertTo Method.
            can_convert_to = BCProp.CanConvertTo(null, valid_type.GetType());
            _class_testresult &= _helper.CompareProp("CanConvertTo - String", can_convert_to, true);

            // Test an invalid Type using the CanConvertTo Method.
            can_convert_to = BCProp.CanConvertTo(null, invalid_type.GetType());
            _class_testresult &= _helper.CompareProp("CanConvertTo - Invalid Type", can_convert_to, false);

            // Test a valid Type using an invalid ITypeDescriptorContext.  This should return
            // the expected exception.
            MyTypeConverter wrong_type_converter = new MyTypeConverter(Colors.Red);

            try
            {
                can_convert_to = BCProp.CanConvertTo(wrong_type_converter, valid_type.GetType());
            }
            catch (System.Exception e)
            {

                //the exception message differs between 3.5 and 4.0, 3.5 has ".Instance" appended. The test now just checks that the exception message
                //contains the core of the message, rather than checking that it matches exactly.
                if (e.Message.Contains(wrong_typedescriptorcontext))
                {
                    CommonLib.LogStatus("Pass: CanConvertTo with an invalid TypeDescriptor returns the expected exception");
                }
                else
                {
                    CommonLib.LogFail("Fail: CanConvertTo with an invalid TypeDescriptor returns " + e.Message + " should be " + wrong_typedescriptorcontext);
                    _class_testresult &= false;
                }
            }

            // Test a valid but non null ITypeDescriptorContext.  This should return
            // the value of Brush.IsSimpleBrush().  Should be True unless the Brush is animated.
            MyTypeConverter non_null_type_converter = new MyTypeConverter(Brushes.Red);
            can_convert_to = BCProp.CanConvertTo(non_null_type_converter, valid_type.GetType());
            _class_testresult &= _helper.CompareProp("CanConvertTo - Non Null TypeDescriptor", can_convert_to, true);
            #endregion

            #region Test #3 - The ConvertFrom Method
            // Usage: object = BrushConverter.ConvertFrom(System.ComponentModel.ITypeDescriptorContext, System.Globalization.CultureInfo, object)
            // Notes: Returns an object that is converted from a string descriptor.
            CommonLib.LogStatus("Test #3 - The ConvertFrom Method");

            bool test3_result = true;

            // Use the ConvertFrom Method to convert strings into Brush types.
            SolidColorBrush solidbrush3 = (SolidColorBrush)BCProp.ConvertFrom(null, Cinfo, "SeaGreen");
            test3_result &= _helper.CheckType(solidbrush3, typeof(SolidColorBrush));
            DC.DrawRectangle(solidbrush3, null, rect1);


            // Calling the ConvertFrom Method with a null string should generate the
            // expected exception.
            SolidColorBrush null_brush;

            try
            {
                null_brush = (SolidColorBrush)BCProp.ConvertFrom(null, Cinfo, null);
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
                    test3_result &= false;
                }
            }

            // Calling the ConvertFrom Method with the wrong type of object should
            // generate the expected exception.
            try
            {
                null_brush = (SolidColorBrush)BCProp.ConvertFrom(null, Cinfo, Colors.Green);
            }
            catch (System.Exception e)
            {
                if (e.Message == CF_wrong_type_exception)
                {
                    CommonLib.LogStatus("Pass: ConvertFrom with the wrong type of object returns the expected exception");
                }
                else
                {
                    CommonLib.LogFail("Fail: ConvertFrom the wrong type of object returns " + e.Message + " should be " + CF_wrong_type_exception);
                    test3_result &= false;
                }
            }

            // Add results for this method to the results for the test.
            _class_testresult &= test3_result;
            #endregion

            #region Test #4 - The ConvertTo Method
            // Usage: object = BrushConverter.ConvertTo(System.ComponentModel.ITypeDescriptorContext, System.Globalization.CultureInfo, object, System.Type)
            // Notes: Returns an object that is converted to the specified type.
            CommonLib.LogStatus("Test #4 - The ConvertTo Method");

            bool test4_result = true;

            // Use the ConvertTo Method to convert certain Brush types into strings.
            String solidbrushstring = (String)BCProp.ConvertTo(null, Cinfo, new SolidColorBrush(Colors.Green), valid_type.GetType());
            test4_result &= _helper.CompareProp("ConvertTo", solidbrushstring, "#FF008000");

            LinearGradientBrush lgb = new LinearGradientBrush(Colors.Red, Colors.Blue, 90.0);
            String lineargradientbrushstring = (String)BCProp.ConvertTo(null, Cinfo, lgb, valid_type.GetType());
            test4_result &= _helper.CompareProp("ConvertTo", lineargradientbrushstring, "System.Windows.Media.LinearGradientBrush");

            String radialgradientbrushstring = (String)BCProp.ConvertTo(null, Cinfo, new RadialGradientBrush(Colors.Purple, Colors.Yellow), valid_type.GetType());
            test4_result &= _helper.CompareProp("ConvertTo", radialgradientbrushstring, "System.Windows.Media.RadialGradientBrush");

            // Calling the ConvertTo Method with a Type argument that is null 
            // should generate the expected exception.
            String nullstring;

            try
            {
                nullstring = (String)BCProp.ConvertTo(null, Cinfo, new SolidColorBrush(Colors.Green), null);
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
                    test4_result &= false;
                }
            }

            // Calling the ConvertTo Method with an object argument that is null 
            // should generate the expected exception.
            try
            {
                nullstring = (String)BCProp.ConvertTo(null, Cinfo, null, valid_type.GetType());
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
                    test4_result &= false;
                }
            }

            // Calling the ConvertTo Method with the wrong type of object should
            // generate the expected exception.
            try
            {
                nullstring = (String)BCProp.ConvertTo(null, Cinfo, Colors.Red, valid_type.GetType());
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
                    test4_result &= false;
                }
            }

            // Add results for this method to the results for the test.
            _class_testresult &= test4_result;
            #endregion
            #endregion End Of SECTION II

            CommonLib.LogTest("Test case result: "+_class_testresult );
        }

        private bool _class_testresult;
        private Type _objectType;
        private HelperClass _helper;
    }
}
