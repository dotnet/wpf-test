// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*************************************************
 *  This file test:
 *    1. DataObject(System.Type, object)    - create DataObject(typeof(DataObjectTypeObjBitmapAPI), myBitmap)
 *    2. GetForamts(bool)                   - returns string array
 *    3. GetData(System.Type)               - GetData from System.Type
 *    4. GetData(string)                    - GetData from string array of GetFormats
 *    5. GetDataPresent(System.Type)        - return bool value
 *    6. SetData(System.Type, object)       - SetData(typeof(DataObjectTypeObjBitmapAPI), myBitmap)
 *  Command Line: exe.exe /TestCaseType=DataObjectTypeObjBitmapAPI /Bool=ture
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
    [Test(0, "DataObject", "DataObjectTypeObjBitmapAPI1", MethodParameters = "/TestCaseType=DataObjectTypeObjBitmapAPI /Bool=true")]
    [Test(2, "DataObject", "DataObjectTypeObjBitmapAPI2", MethodParameters = "/TestCaseType=DataObjectTypeObjBitmapAPI /Bool=false")]
    [TestOwner("Microsoft"), TestTactics("186, 187")]
    public class DataObjectTypeObjBitmapAPI : CustomTestCase
    {
        bool _pass = true;
        DataObject _DO;
        Bitmap _myBitmap;
        bool _boolArg;

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _myBitmap = new Bitmap(150, 150);
            _DO = new DataObject(typeof(DataObjectTypeObjBitmapAPI), _myBitmap);
            Eval();
            _DO.SetData(typeof(DataObjectTypeObjBitmapAPI), _myBitmap);
            Eval();
            Logger.Current.Quit (_pass);
        }
        
        private void Eval()
        {
            _boolArg = ConfigurationSettings.Current.GetArgumentAsBool("Bool", true); //set to true for false for GetFormats(bool)
            string [] formatStr =_DO.GetFormats(_boolArg);
            Logger.Current.Log("formatStr.Length: "+formatStr.Length);
            for(int i=0; i<formatStr.Length; i++)
            {
                object myData = _DO.GetData(formatStr[i]);
                object myData2=_DO.GetData(typeof(DataObjectTypeObjBitmapAPI));
                if("DataTransfer.DataObjectTypeObjBitmapAPI" != formatStr[i] || myData != _myBitmap || myData2 != _myBitmap || !_DO.GetDataPresent(typeof(DataObjectTypeObjBitmapAPI)))
                {
                    Logger.Current.Log("Fail in Counter:1 and boolArg: true");
                    _pass = false;
                }
                Logger.Current.Log(i+" formatStr     : "+formatStr[i]+" : "+"DataTransfer.DataObjectTypeObjBitmapAPI");
                Logger.Current.Log(i+" myData        : "+myData+" : "+_myBitmap);
                Logger.Current.Log(i+" myData2        : "+myData2+" : "+_myBitmap);
                Logger.Current.Log(i+" getDataPresent: "+_DO.GetDataPresent(typeof(DataObjectTypeObjBitmapAPI)));
            }
        }
    }
}
