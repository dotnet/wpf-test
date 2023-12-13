// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Transform Converter Tests - These are the unit tests for transform converter

using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{

    internal class TransformConverterTest : ApiTest
    {
        public TransformConverterTest( double left, double top,
            double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(TransformConverter);
            _helper = new HelperClass();
            Update();
        }
        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus(_objectType.ToString());

            #region Initialization stage
            CommonLib.Stage = TestStage.Initialize;
            #endregion

            #region Running stage
            CommonLib.Stage = TestStage.Run;

            #region SECTION I - CONSTRUCTORS
            CommonLib.LogStatus("***** SECTION I - CONSTRUCTORS *****");

            #region Test - Default constructor
            CommonLib.LogStatus("Test: TransformConverter default constructor");

            // Create a TransformConverter 
            TransformConverter tc = new TransformConverter();

            // Confirm that the right type of object was created.
            _class_testresult &= _helper.CheckType(tc, _objectType);
            #endregion
            #endregion

            #region SECTION II - Methods
            CommonLib.LogStatus("***** SECTION II - Methods *****");

            #region Test CanConvertFrom
            CommonLib.LogStatus("Test: CanConvertFrom method");

            // TransformConverter should be able to convert from string
            _class_testresult &= _helper.CompareResults("CanConvertFrom", tc.CanConvertFrom(null, typeof(string)), true);
            // TransformConverter should not be able to convert from TranslateTransform
            _class_testresult &= _helper.CompareResults("CanConvertFrom", tc.CanConvertFrom(null, typeof(TranslateTransform)), false);
            #endregion

            #region Test CanConvertTo
            CommonLib.LogStatus("Test: CanConvertTo method");
            // TransformConverter should be able to convert to string
            _class_testresult &= _helper.CompareResults("CanConvertTo", tc.CanConvertTo(null, typeof(string)), true);
            // TransformConverter should not be able to convert to TranslateTransform
            _class_testresult &= _helper.CompareResults("CanConvertTo", tc.CanConvertTo(null, typeof(TranslateTransform)), false);
            #endregion

            #region Test ConvertFrom
            CommonLib.LogStatus("Test: ConvertFrom method");

            object obj = tc.ConvertFrom(null, System.Globalization.CultureInfo.InvariantCulture, "Identity");
            _class_testresult &= CompareConvertResults("ConvertFrom", obj, typeof(MatrixTransform), new Matrix(1.0, 0.0, 0.0, 1.0, 0.0, 0.0));

            obj = tc.ConvertFrom(null, System.Globalization.CultureInfo.InvariantCulture, "-10.0 0.0 0.0 0.0 0.0 10.0");
            _class_testresult &= CompareConvertResults("ConvertFrom", obj, typeof(MatrixTransform), new Matrix(-10.0, 0.0, 0.0, 0.0, 0.0, 10.0));

            obj = tc.ConvertFrom(null, System.Globalization.CultureInfo.InvariantCulture, "1 0.0 0.1 0.0 0.0 1");
            _class_testresult &= CompareConvertResults("ConvertFrom", obj, typeof(MatrixTransform), new Matrix(1.0, 0.0, 0.1, 0.0, 0.0, 1.0));
            #endregion

            #region Test ConvertTo
            CommonLib.LogStatus("Test: ConvertTo method");
            try
            {
                MatrixTransform mt = new MatrixTransform(1.0, 0.0, 0.0, 1.0, 0.0, 0.0);
                _class_testresult &= _helper.CompareResults("ConvertTo", tc.ConvertTo(null, System.Globalization.CultureInfo.InvariantCulture, mt, typeof(int)) as string, "Identity");
            }
            catch (StackOverflowException)
            {
                throw;
            }
            catch (OutOfMemoryException)
            {
                throw;
            }
            catch (NotSupportedException)
            {
                CommonLib.LogStatus("Pass: ConvertTo - Destination type Int returns the expected exception");
            }
            catch (Exception e)
            {
                CommonLib.LogStatus(string.Format("Fail: ConvertTo - Destination type Int returns {0} should be System.NotSupportedException", e.GetType().Name));
            }

            try
            {
                MatrixTransform mt = new MatrixTransform(1.0, 0.0, 0.0, 1.0, 0.0, 0.0);
                _class_testresult &= _helper.CompareResults("ConvertTo", tc.ConvertTo(null, System.Globalization.CultureInfo.InvariantCulture, mt, null) as string, "Identity");
            }
            catch (StackOverflowException)
            {
                throw;
            }
            catch (OutOfMemoryException)
            {
                throw;
            }
            catch (ArgumentNullException)
            {
                CommonLib.LogStatus("Pass: ConvertTo - Destination type null returns the expected exception");
            }
            catch (Exception e)
            {
                CommonLib.LogStatus(string.Format("Fail: ConvertTo - Destination type null returns {0} should be System.ArgumentNullException", e.GetType().Name));
            }

            MatrixTransform mt1 = new MatrixTransform(1.0, 0.0, 0.0, 1.0, 0.0, 0.0);
            _class_testresult &= _helper.CompareResults("ConvertTo", tc.ConvertTo(null, System.Globalization.CultureInfo.InvariantCulture, mt1, typeof(string)) as string, "Identity");

            MatrixTransform mt2 = new MatrixTransform(10.5, 0.0, 0.0, 10.5, 0.0, 0.0);
            _class_testresult &= _helper.CompareResults("ConvertTo", tc.ConvertTo(null, System.Globalization.CultureInfo.InvariantCulture, mt2, typeof(string)) as string, "10.5,0,0,10.5,0,0");
            #endregion
            #endregion
            #endregion // Running stage

            #region TEST LOGGING
            // Log the programmatic result for this API test using the
            // Automation Framework LogTest method.  If This result is False,
            // it will override the result of a Visual Comparator.  Conversely,
            // if a Visual Comparator is False it will override a True result
            // from this test.            
            CommonLib.LogTest("Result for:" + _objectType);
            #endregion End of TEST LOGGING
        }

        private bool CompareConvertResults(string testName, object obj, System.Type expectedType, Matrix mx)
        {
            if (obj == null)
            {
                CommonLib.LogFail("Fail: converter from " + testName + "returned null object.");
                return false;
            }
            if (obj.GetType().ToString() != expectedType.ToString())
            {
                CommonLib.LogFail("Fail: converter from " + testName + "returned wrong data type.");
                return false;
            }
            if (!_helper.CompareMatrix("Value", ((Transform)obj).Value, mx.M11, mx.M12, mx.M21, mx.M22, mx.OffsetX, mx.OffsetY))
            {
                return false;
            }
            CommonLib.LogStatus("Pass: converter from " + testName + " was successful");
            return true;
        }

        private bool _class_testresult;
        private Type _objectType;
        private HelperClass _helper;
    }
}

