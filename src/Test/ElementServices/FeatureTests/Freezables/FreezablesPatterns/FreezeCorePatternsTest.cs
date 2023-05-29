// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2004
 *
 *   Program:   Test FreezeCore
 
 *
 ************************************************************/
using System;
using System.Reflection;
using System.Windows;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.ElementServices.Freezables.Utils;
using Microsoft.Test.ElementServices.Freezables.Objects;


namespace Microsoft.Test.ElementServices.Freezables
{

    /**********************************************************************************
    * CLASS:          FreezeCorePatternsTest
    **********************************************************************************/
    internal class FreezeCorePatternsTest : FreezablesPatternsBase
    {

        #region Constructor
        /******************************************************************************
        * Function:          Constructor
        ******************************************************************************/
        internal FreezeCorePatternsTest(Microsoft.Test.ElementServices.Freezables.Utils.Result result)
            : base(result)
        {
            testName = "FreezeCorePatternsTest";
        }
        #endregion


        #region Internal Members
        /******************************************************************************
        * Function:          Perform
        ******************************************************************************/
        /// <summary>
        /// Invoke the specific test case requested.
        /// </summary>
        /// <param name="t">The Type of the Freezable object to be tested.</param>
        /// <returns></returns>
        internal override void Perform(Type t)
        {
            if (!TypeHelper.IsComplexChangeable(t) || IsKnownIssue(t))
            {
                return;
            }

            try
            {
                GlobalLog.LogStatus("ClassName: " + t.ToString());

                // check that a complex freezable type implements the FreezeCore method
                // create an object and invoke the FreezeCore method on it
                Freezable obj = FreezablesPatternsHelper.CreateNewChangeable(t);

                if (obj != null && obj.CanFreeze)
                {
                    // invoke the FreezeCore method
                    Object[] args = new Object[1]; // the FreezeCore method arguments

                    args[0] = false;

                    // invoke the FreezeCore method
                    t.InvokeMember("FreezeCore", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, obj, args);
                }
            }
            catch (System.Reflection.TargetInvocationException e)
            {
                result.passed &= false;
                result.failures.Add(t.ToString() + "-!!!FreezeCore Exception: " + e.InnerException.ToString());
                GlobalLog.LogEvidence("    !!!FreezeCore: Exception: {0}", e.InnerException.ToString());
            }
            catch (System.MissingMethodException e)
            {
                result.passed &= false;
                result.failures.Add(t.ToString() + ": FreezeCore method not found");
                GlobalLog.LogEvidence("    !!!FreezeCore method not found: Exception: {0}", e.ToString());
            }
            catch (System.Reflection.AmbiguousMatchException e)
            {
                result.passed &= false;
                result.failures.Add(t.ToString() + ": FreezeCore method not found");
                GlobalLog.LogEvidence("    !!!FreezeCore method not found: Exception: {0}", e.ToString());
            }
        }
        #endregion
    }
}
