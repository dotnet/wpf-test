// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*************************************************
 *  This file test:
 *  1. call DataObject with all invalid type for parameter(s)
 *  2. call GetData with all invalid type for parameter(s)
 *  3. call GetDataPresent with all invalid type for parameter(s)
 *  4. call SetData with all invalid type for parameter(s)
 *  5. verify exception is catched with catch statement then log as pass otherwise it will throw exception and case will fail
 *  Command Line as follow:
 *  name.exe /TestCaseType=DataObjectInvalidAPIs /Bool=true or false
 * 
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
    /// Testing DataObject.cs apis with invalid argument
    /// </summary>
    [Test(2, "DataObject", "DataObjectInvalidAPIs1", MethodParameters = "/TestCaseType=DataObjectInvalidAPIs /Bool=true")]
    [Test(2, "DataObject", "DataObjectInvalidAPIs2", MethodParameters = "/TestCaseType=DataObjectInvalidAPIs /Bool=false")]
    [TestOwner("Microsoft"), TestTactics("180")]
    [WindowlessTest(true)]
    public class DataObjectInvalidAPIs:Test.Uis.TestTypes.CustomTestCase
    {
        DataObject _DO = null;
        Bitmap _myBitmap;
        DataObject _dobj;    //DataObject should not accpet data as Dataobject
        bool _autoConvert;

        /// <summary>
        /// RunTestCase
        /// </summary>
        public override void RunTestCase ()
        {
            _myBitmap = new Bitmap (150, 150);
            _autoConvert = ConfigurationSettings.Current.GetArgumentAsBool ("Bool", true); //set to true or false
            _dobj = new DataObject();
            _dobj.SetData("123");
            VerifyInvalidDataObject ();
            VerifyInvalidGetData ();
            VerifyInvalidGetDataPresent ();
            VerifyInvalidSetData ();
            Logger.Current.Log("Test Completed.");
            Logger.Current.ReportSuccess();
        }


        private void VerifyInvalidDataObject ()
        {
            // DataObject(object)
            try
            {
                _DO = new DataObject (null); //check for null format
                throw new Exception ("DataObject(obj) accepted a null data.");
            }
            catch (ArgumentException)
            {
                Logger.Current.Log ("DataObject(obj) rejects null data");
            }

            //DataObject(string, object)
            try
            {
                _DO = new DataObject ((string)null, _myBitmap);   //check for null format
                throw new Exception ("DataObject(string, obj) accepted a null format.");
            }
            catch (ArgumentException)
            {
                Logger.Current.Log ("DataObject(string, obj) rejects null format");
            }

            //DataObject(string, object)
            try
            {
                _DO = new DataObject (DataFormats.Bitmap, null); //check for null object
                throw new Exception ("DataObject(string, obj) accepted a null data.");
            }
            catch (ArgumentException)
            {
                Logger.Current.Log ("DataObject(string, obj) rejects null data");
            }
            
            //DataObject(System.Type, object)
            try
            {
                _DO = new DataObject ((System.Type)null, _myBitmap);  //check for null format
                throw new Exception ("DataObject(System.Type, obj) accepted a null format.");
            }
            catch (ArgumentException)
            {
                Logger.Current.Log ("DataObject(System.Type, obj) rejects null format");
            }

            //DataObject(System.Type, object)
            try
            {
                _DO = new DataObject(typeof(string), null); //check for null data
                throw new Exception ("DataObject(System.Type, obj) accepted a null data.");
            }
            catch (ArgumentNullException)
            {
                Logger.Current.Log ("DataObject(System.Type, obj) rejects null data");
            }

            //DataObject(string, object, bool)
            try
            {
                _DO = new DataObject ((string)null, _myBitmap, _autoConvert);  //check for null format
                throw new Exception ("DataObject(string, obj, bool) accepted a null format.");
            }
            catch (ArgumentException)
            {
                Logger.Current.Log ("DataObject(string, obj, bool) rejects null format");
            }
            try
            {
                _DO = new DataObject (DataFormats.Bitmap, null, _autoConvert);    //check for null format
                throw new Exception ("DataObject(string, obj, bool) accepted a null data.");
            }
            catch (ArgumentException)
            {
                Logger.Current.Log ("DataObject(string, obj, bool) rejects null data");
            }
        }

        private void VerifyInvalidGetData ()
        {
            _DO = new DataObject();
            //GetData(string)
            try
            {
                _DO.GetData ((string)null);
                throw new ApplicationException ("DataObject.GetData(string) accepts null strings.");
            }
            catch (SystemException)
            {
                Logger.Current.Log ("DataObject.GetData(string) rejects null strings");
            }

            //GetData(string, bool)
            try
            {
                _DO.GetData ((string)null, _autoConvert);
                throw new ApplicationException ("DataObject.GetData(string, bool) accepts null strings.");
            }
            catch (SystemException)
            {
                Logger.Current.Log ("DataObject.GetData(string, bool) rejects null strings");
            }

            //GetData(System.Type)
            try
            {
                _DO.GetData ((System.Type)null);
                throw new ApplicationException ("DataObject.GetData(System.Type) accepts null System.Type.");
            }
            catch (SystemException)
            {
                Logger.Current.Log ("DataObject.GetData(System.Type) rejects null System.Type");
            }
        }

        private void VerifyInvalidGetDataPresent ()
        {
            //GetDataPresent(string)
            try
            {
                _DO.GetDataPresent ((string)null);
                throw new ApplicationException ("DataObject.GetDataPresent(string) accepts null strings.");
            }
            catch (SystemException)
            {
                Logger.Current.Log ("DataObject.GetDataPresent(string) rejects null strings");
            }

            //GetDataPresent(string, bool)
            try
            {
                _DO.GetDataPresent ((string)null, _autoConvert);
                throw new ApplicationException ("DataObject.GetDataPresent(string, bool) accepts null strings.");
            }
            catch (SystemException)
            {
                Logger.Current.Log ("DataObject.GetDataPresent(string, bool) rejects null strings");
            }

            //GetDataPresent(System.Type)
            try
            {
                _DO.GetDataPresent ((System.Type)null);
                throw new ApplicationException ("DataObject.GetDataPresent(System.Type) accepts null System.Type.");
            }
            catch (SystemException)
            {
                Logger.Current.Log ("DataObject.GetDataPresent(System.Type) rejects null System.Type");
            }
        }

        private void VerifyInvalidSetData ()
        {
            //SetData(object)
            try
            {
                _DO.SetData (null);
                throw new ApplicationException ("DataObject.SetData(object) accepts null object.");
            }
            catch (SystemException)
            {
                Logger.Current.Log ("DataObject.SetData(object) rejects null object");
            }

            //SetData(string, object, bool)
            try
            {
                _DO.SetData ((string)null, _myBitmap, _autoConvert);
                throw new ApplicationException ("DataObject.SetData(string, object, bool) accepts null string.");
            }
            catch (SystemException)
            {
                Logger.Current.Log ("DataObject.SetData(string, object, bool) rejects null string");
            }
            try
            {
                _DO.SetData (DataFormats.Bitmap, null, _autoConvert);
                throw new ApplicationException ("DataObject.SetData(string, object, bool) accepts null data.");
            }
            catch (SystemException)
            {
                Logger.Current.Log ("DataObject.SetData(string, object, bool) rejects null data");
            }
            
            //SetData(string, object).
            try
            {
                _DO.SetData ((string)null, _myBitmap);
                throw new ApplicationException ("DataObject.SetData(string, object) accepts null string.");
            }
            catch (SystemException)
            {
                Logger.Current.Log ("DataObject.SetData(string, object) rejects null string");
            }
            try
            {
                _DO.SetData (DataFormats.Bitmap, null);
                throw new ApplicationException ("DataObject.SetData(string, object) accepts null data.");
            }
            catch (SystemException)
            {
                Logger.Current.Log ("DataObject.SetData(string, object) rejects null data");
            }

            //SetData(System.Type,object)
            try
            {
                _DO.SetData ((System.Type)null, _myBitmap);
                throw new ApplicationException ("DataObject.SetData(System.Type, object) accepts null System.Type.");
            }
            catch (SystemException)
            {
                Logger.Current.Log ("DataObject.SetData(System.Type, object) rejects null System.Type");
            }
            try
            {
                _DO.SetData (DataFormats.Bitmap, null);
                throw new ApplicationException ("DataObject.SetData(System.Type, object) accepts null data.");
            }
            catch (SystemException)
            {
                Logger.Current.Log ("DataObject.SetData(System.Type, object) rejects null data");
            }
        }
    }
}
