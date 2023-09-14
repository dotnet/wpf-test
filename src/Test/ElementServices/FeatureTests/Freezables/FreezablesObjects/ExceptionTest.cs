// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Collections.Generic;
using System.Security;
using System.Security.Policy;
using System.Windows;

using Microsoft.Test.ElementServices.Freezables.Objects;
using Microsoft.Test.Logging;


namespace Microsoft.Test.ElementServices.Freezables
{
    /**********************************************************************************
    * CLASS:          ExceptionTest
    **********************************************************************************/
    public class ExceptionTest : FreezablesObjectsBase
    {
        #region Constructors
        /******************************************************************************
        * Function:          Constructor
        ******************************************************************************/
        public ExceptionTest(string testName, string objName) : base(testName, objName)
        {
        }
        #endregion


        #region Public and Protected Members

        /******************************************************************************
        * Function:          RunTest
        ******************************************************************************/
        public override void RunTest()
        {
            // Create freezable objects
            foreach (Freezable freezable in freezables)
            {
                if (freezable.CanFreeze)
                {
                    return;
                }
                try
                {
                    freezable.Freeze();
                    // log failure if there is no exception
                    GlobalLog.LogEvidence("No exception thrown when trying to freeze an object where CanFreeze==false");
                    failures.Add("No exception thrown when trying to freeze an object where CanFreeze==false");
                    passed &= false;
                }
                catch (System.InvalidOperationException e)
                {
                    if (!e.InnerException.ToString().Contains("This Freezable can not be frozen"))
                    {
                        GlobalLog.LogEvidence("Bad exception message: " + e.InnerException.ToString());
                        failures.Add("Bad exception message: " + e.InnerException.ToString());
                        passed &= false;

                    }
                }
                catch (System.Exception e)
                {
                    GlobalLog.LogEvidence("Expected InvalidOperationException thrown, but get this exception instread: " + e.ToString());
                    failures.Add("Expected InvalidOperationException thrown, but get this exception instread: " + e.ToString());
                    passed &= false;
                }
            }
        }
        #endregion
    }
}

