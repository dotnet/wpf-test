// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*************************************************
 *  This file test:
 *    1. DataObject(object, string, bool)   - create DataObject(DataFormats.Bitmap, myBitmap, bool)
 *    2. GetForamts(bool)                   - returns string array
 *    3. GetData(string, bool)              - GetData from string array of GetFormats
 *    4. GetDataPresent(string, bool)       - return bool value
 *    5. SetData(string, object)            - SetData(DataFormats.Bitmap, myNewBitmap)
 *  Command Line: exe.exe /TestCaseType=DataObjectStrObjBoolBitmapAPI /Bool=ture
 *  Bool: true or false
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
    using Test.Uis.TestTypes;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    
    #endregion Namespaces.
    /// <summary>
    ///
    /// </summary>
    [Test(0, "DataObject", "DataObjectStrObjBoolBitmapAPI1", MethodParameters = "/TestCaseType=DataObjectStrObjBoolBitmapAPI /Bool=true")]
    [Test(2, "DataObject", "DataObjectStrObjBoolBitmapAPI2", MethodParameters = "/TestCaseType=DataObjectStrObjBoolBitmapAPI /Bool=false")]
    [TestOwner("Microsoft"), TestTactics("184, 185")]
    public class DataObjectStrObjBoolBitmapAPI:CustomTestCase
    {
        bool _pass = true;
        DataObject _DO;
        Bitmap _myBitmap;
        Bitmap _myNewBitmap;
        bool _boolArg;
        int _counter = 0;

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _myBitmap = new Bitmap(150, 150);
            _myNewBitmap = new Bitmap(100, 100);
            _DO = new DataObject(DataFormats.Bitmap, _myBitmap, _boolArg);
            Eval();
            _DO.SetData(DataFormats.Bitmap, _myNewBitmap);
            Eval();
            Logger.Current.Quit (_pass);
        }

        private void Eval()
        {
            _counter ++;
            _boolArg = ConfigurationSettings.Current.GetArgumentAsBool("Bool", true); //set to true for false for GetFormats(bool)
            string [] formatStr =_DO.GetFormats(_boolArg);
            Logger.Current.Log("formatStr.Length: "+formatStr.Length);
            for(int i=0; i<formatStr.Length; i++)
            {
                object myData = _DO.GetData(formatStr[i], _boolArg);
                if(_boolArg)
                {
                    if(_counter == 1)
                    {
                        if ("Bitmap,System.Drawing.Bitmap,System.Windows.Media.Imaging.BitmapSource".Split(',')[i] != formatStr[i] || myData != _myBitmap || !_DO.GetDataPresent(formatStr[i], _boolArg))
                        {
                            Logger.Current.Log("Fail in Counter:1 and boolArg: true");
                            _pass = false;
                        }
                    }
                    else
                    {
                        if ("Bitmap,System.Drawing.Bitmap,System.Windows.Media.Imaging.BitmapSource".Split(',')[i] != formatStr[i] || myData != _myNewBitmap || !_DO.GetDataPresent(formatStr[i], _boolArg))
                        {
                            Logger.Current.Log("Fail in Counter:2 and boolArg: true");
                            _pass = false;
                        }
                    }
                }
                else
                {
                    if(_counter == 1)
                    {
                        if(myData != _myBitmap || !_DO.GetDataPresent(formatStr[i], _boolArg) || "Bitmap".Split(',')[i] != formatStr[i])
                        {
                            Logger.Current.Log("Fail in Counter:1 and boolArg: false");
                            _pass = false;
                        }
                    }
                    else
                    {
                        if("Bitmap".Split(',')[i] != formatStr[i] || myData != _myNewBitmap || !_DO.GetDataPresent(formatStr[i], _boolArg))
                        {
                            Logger.Current.Log("Fail in Counter:2 and boolArg: true");
                            _pass = false;
                        }
                    }
                }
                Logger.Current.Log(i+" formatStr     : "+formatStr[i]);
                Logger.Current.Log(i+" myData        : "+myData+" : "+_myBitmap);
                Logger.Current.Log(i+" getDataPresent: "+_DO.GetDataPresent(formatStr[i]));
            }
        }
    }
}
