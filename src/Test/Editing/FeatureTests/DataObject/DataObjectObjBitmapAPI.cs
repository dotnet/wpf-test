// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*************************************************
 *  This file test:
 *    1. DataObject(object)                - create DataObject(DataFormats.Bitmap)
 *    2. GetForamts()                        - returns string array
 *    3. GetData(string, bool)            - GetData from string array of GetFormats
 *    4. GetDataPresent(string, bool)        - return bool value
 *    5. SetData(object)                    - SetData(myBitmap)
 *  Command Line: exe.exe /TestCaseType=DataObjectObjBitmapAPI
 * ************************************************/

namespace DataTransfer
{
    #region Namespaces.

    using System;
    using System.Drawing;
    using System.Windows;

    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.Utils;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    #endregion Namespaces.

    /// <summary>
    /// Verifies that the DataObject class works correctly with string and
    /// Bitmap data.
    /// </summary>
    [Test(0, "DataObject", "DataObjectObjBitmapAPI", MethodParameters = "/TestCaseType=DataObjectObjBitmapAPI")]
    [TestOwner("Microsoft"), TestTactics("181")]
    [WindowlessTest(true)]
    public class DataObjectObjBitmapAPI : Test.Uis.TestTypes.CustomTestCase
    {
        private bool _pass = true;
        private DataObject _dataObject;
        private Bitmap _myBitmap;
        private bool _autoConvert;

        #region Test case constants.

        private const string StringValue = "Any string content.";
        private string[] _stringFormats = new string[] {
            DataFormats.Text, DataFormats.UnicodeText, DataFormats.StringFormat
        };
        private string[] _bitmapFormats = new string[] {
            DataFormats.Bitmap, "System.Drawing.Bitmap", DataFormats.Serializable
        };
        private readonly string _stringOriginalFormat = DataFormats.StringFormat;
        private readonly string _bitmapOriginalFormat = "System.Drawing.Bitmap";

        #endregion Test case constants.

        /// <summary>Runs the test case.</summary>
        //[TestEntryPoint]
        public override void RunTestCase()
        {
            _autoConvert = true;
            TestDataObject();

            _autoConvert = false;
            TestDataObject();

            Logger.Current.Quit (_pass);
        }

        /// <summary>
        /// Runs all tests on a new DataObject, with the current test case
        /// variables.
        /// </summary>
        private void TestDataObject()
        {
            Log("Testing data object with AutoConvert=" + _autoConvert);

            Log("Verifying string handling...");
            _dataObject = new DataObject(StringValue);
            VerifyStringContent();
            
            Log("Verifying bitmap handling...");
            _myBitmap = new Bitmap(100,100);
            _dataObject.SetData(_myBitmap);
            VerifyBitmapContent();
        }

        /// <summary>Logs all the specified formats.</summary>
        /// <param name="formats">Formats to log.</param>
        private void LogFormats(string[] formats)
        {
            System.Diagnostics.Debug.Assert(formats != null);

            Log("Formats available: " + formats.Length);
            foreach(string format in formats)
            {
                Log("Format: " + format);
            }
        }

        /// <summary>
        /// Verifies that the data object added a string correctly.
        /// </summary>
        private void VerifyStringContent()
        {
            string[] formats;
            object data;

            // Verify that DataObject.GetFormats returns the expected result.
            formats = _dataObject.GetFormats();
            LogFormats(formats);
            CheckExpectedFormats(formats, _stringFormats);
            CheckDataPresent(formats);

            // Verify that DataObject.GetData returns the expected result in
            // all available formats.
            foreach(string format in formats)
            {
                data = _dataObject.GetData(format, _autoConvert);
                if (data == null && _autoConvert)
                {
                    Log("FAIL: Data not available in format: " + format +
                        " when AutoConvert is " + _autoConvert);
                    _pass = false;
                }
                else if (data != null && !data.Equals(StringValue))
                {
                    Log("FAIL: Data [ " + data + "] does not match expected " +
                        "value [" + StringValue + "]" +
                        " when AutoConvert is " + _autoConvert);
                    _pass = false;
                }
            }
        }

        /// <summary>
        /// Verifies that the bitmap was successfully added to the
        /// DataObject with a string.
        /// </summary>
        private void VerifyBitmapContent()
        {
            string[] formats;
            object data;

            Log("Verifying that DataObject.GetFormats returns bitmap formats " +
                "in addition to the original string formats.");
            formats = _dataObject.GetFormats();
            LogFormats(formats);
            CheckExpectedFormats(formats, _bitmapFormats);
            CheckExpectedFormats(formats, _stringFormats);
            CheckDataPresent(formats);

            // Verify that DataObject.GetData returns the expected result in
            // all available formats.
            foreach(string format in formats)
            {
                if (_dataObject.GetDataPresent(format, _autoConvert))
                {
                    data = _dataObject.GetData(format, _autoConvert);
                    if (data == null)
                    {
                        Log("FAIL: Data not available in format: " + format +
                            " when AutoConvert is " + _autoConvert);
                        _pass = false;
                    }
                }
                else
                {
                    if (_autoConvert == true)
                    {
                        Log("FAIL: Data format reported but cannot be " +
                            "supplied even with AutoConvert.");
                        _pass = false;
                    }
                    else
                    {
                        Log("Data format " + format + " cannot be supplied " +
                            "unless auto-converting.");
                    }
                }
            }
        }

        /// <summary>
        /// Verifies that the specified formats are present.
        /// </summary>
        /// <param name="formats">Expected formats.</param>
        /// <remarks>
        /// If autoConvert is true, all formats are required to be present.
        /// Otherwise, the original formats should be present.
        /// </remarks>
        private void CheckDataPresent(string[] formats)
        {
            if (_autoConvert)
            {
                Log("Verifying that all reported formats are present...");
                foreach(string format in formats)
                {
                    if (!_dataObject.GetDataPresent(format, true))
                    {
                        Log("FAIL: Data format not present (" + format +
                            " when AutoConvert is " + _autoConvert + ")");
                        _pass = false;
                    }
                }
            }
            else
            {
                Log("Verifying that the original formats are present...");
                foreach(string format in formats)
                {
                    if (!_dataObject.GetDataPresent(format, false) &&
                        (format == _stringOriginalFormat || format == _bitmapOriginalFormat))
                    {
                        Log("FAIL: Data format not present (" + format +
                            " when AutoConvert is " + _autoConvert + ")");
                        _pass = false;
                    }
                }
            }
        }

        /// <summary>
        /// Verifies that the available formats match the expectations.
        /// </summary>
        /// <param name="available">DataObject supported formats.</param>
        /// <param name="expected">Formats the test case expects.</param>
        private void CheckExpectedFormats(string[] available, string[] expected)
        {
            bool formatFound;   // Whether an expected value is available.

            Log("Ensuring that all the expected formats are available...");
            foreach(string expectedValue in expected)
            {
                // Look for the value in the available strings.
                formatFound = false;
                foreach(string availableFormat in available)
                {
                    if (availableFormat == expectedValue)
                    {
                        formatFound = true;
                        break;
                    }
                }
                if (!formatFound)
                {
                    _pass = false;
                    Log("FAIL: Expected format " + expectedValue + 
                        " not found in available formats.");
                }
            }
        }
    }
}
