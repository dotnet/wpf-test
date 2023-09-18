// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*************************************************
 *  This file test:
 *	1. call DataObject(DataFormats.GetFormat (null));
 *  2. verify exception is catched with catch statement then log as pass otherwise it will throw exception and case will fail
 *  Command Line as follow:
 *	name.exe /TestCaseType=DataFormatInvalidGetFormats
 * 
 * ************************************************/
namespace DataTransfer
{
    using System;
    using System.Windows;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.Utils;
    using Test.Uis.TestTypes;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    /// <summary>
    /// invalid test
    /// </summary>
    [Test(2, "DataObject", "DataFormatInvalidGetFormats", MethodParameters = "/TestCaseType=DataFormatInvalidGetFormats")]
    public class DataFormatInvalidGetFormats : CustomTestCase
    {
        DataObject _DO = null;
        bool _pass = true;

        /// <summary>
        /// RunTestCase
        /// </summary>
        public override void RunTestCase()
        {
            VerifyInvalidGetFormats();
            Logger.Current.Quit(_pass);
        }

        /// <summary>
        /// VerifyInvalidGetFormats
        /// </summary>
        public void VerifyInvalidGetFormats()
        {
            try //check with null string
            {
                _DO = new DataObject(DataFormats.GetDataFormat(null));
                throw new Exception("DataFormats.GetDataFormat(null) accepted a null data.");
            }
            catch (ArgumentException)
            {
                Logger.Current.Log("DataFormats.GetDataFormat(null) rejects null data.");
            }
        }
    }
}