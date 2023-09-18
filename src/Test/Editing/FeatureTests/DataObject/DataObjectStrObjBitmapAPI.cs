// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*************************************************
 *  This file test:
 *    1. DataObject(object, string)         - create DataObject(DataFormats.Bitmap, myBitmap)
 *    2. GetForamts(bool)                   - returns string array
 *    3. GetData(string, bool)              - GetData from string array of GetFormats
 *    4. GetDataPresent(string, bool)       - return bool value
 *    5. SetData(string, object)            - SetData("Testing", myBitmap)
 *  Command Line: exe.exe /TestCaseType=DataObjectStrObjBitmapAPI /Bool=ture
 *  Bool: true or false
 * ************************************************/

namespace DataTransfer
{
    using System;
    using System.Drawing;
    using System.Windows;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.Utils;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    /// <summary>
    /// Verifies that the DataObject class works correctly with Bitmap data.
    /// </summary>
    [Test(0, "DataObject", "DataObjectStrObjBitmapAPI1", MethodParameters = "/TestCaseType=DataObjectStrObjBitmapAPI /Bool=true", Keywords = "MicroSuite")]
    [Test(2, "DataObject", "DataObjectStrObjBitmapAPI2", MethodParameters = "/TestCaseType=DataObjectStrObjBitmapAPI /Bool=false")]
    [TestOwner("Microsoft"), TestTactics("182, 183")]
    [WindowlessTest(true)]
    public class DataObjectStrObjBitmapAPI: Test.Uis.TestTypes.CustomTestCase
    {
        bool _pass = false;
        DataObject _DO;
        Bitmap _myBitmap;
        bool _boolArg;
        int _counter = 0;

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase ()
        {
            _myBitmap = new Bitmap (150, 150);
            _DO = new DataObject (DataFormats.Bitmap, _myBitmap);
            Eval ();
            _DO.SetData ("Testing", _myBitmap, true);
            Eval ();
            Logger.Current.Quit (_pass);
        }

        private void Eval()
        {
            _counter++;
            //set to true or false for GetFormats(bool)
            _boolArg = ConfigurationSettings.Current.GetArgumentAsBool ("Bool", true);

            string[] formatStr = _DO.GetFormats (_boolArg);

            Logger.Current.Log ("formatStr.Length: " + formatStr.Length);
            for (int i = 0; i < formatStr.Length; i++)
            {
                object myData = _DO.GetData (formatStr[i], _boolArg);

                if (_boolArg)
                {
                    if (_counter == 1)
                    {
                        if ("System.Windows.Media.Imaging.BitmapSource" == formatStr[i] || myData is System.Windows.Media.Imaging.BitmapSource || _DO.GetDataPresent(formatStr[i], _boolArg))
                        {
                            _pass = true;
                        }
                    }
                    else
                    {
                        if (myData is System.Windows.Media.Imaging.BitmapSource || _DO.GetDataPresent(formatStr[i], _boolArg))
                        {
                            _pass = true;
                        }
                    }
                }
                else
                {
                    if (_counter == 1)
                    {
                        if (myData == _myBitmap || _DO.GetDataPresent (formatStr[i], _boolArg) || "Bitmap" == formatStr[i])
                        {
                            _pass = true;
                        }
                    }
                    else
                    {
                        if (myData == _myBitmap || _DO.GetDataPresent(formatStr[i], _boolArg))
                        {
                            _pass = true;
                        }
                    }
                }

                Logger.Current.Log("Counter: " + i + " formatStr     : " + formatStr[i]);
                Logger.Current.Log("Counter: " + i + " myData        : " + myData + " : " + _myBitmap);
                Logger.Current.Log("Counter: " + i + " getDataPresent: " + _DO.GetDataPresent(formatStr[i]));
            }
        }
    }
}
