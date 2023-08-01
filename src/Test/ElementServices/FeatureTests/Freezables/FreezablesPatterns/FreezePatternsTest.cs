// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2004
 *
 *   Program:   Test Freeze
 
 *
 ************************************************************/
using System;
using System.Reflection;
using System.Windows;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.ElementServices.Freezables.Utils;


namespace Microsoft.Test.ElementServices.Freezables
{

    /**********************************************************************************
    * CLASS:          FreezePatternsTest
    **********************************************************************************/
    internal class FreezePatternsTest : FreezablesPatternsBase
    {

        #region Constructor
        /******************************************************************************
        * Function:          Constructor
        ******************************************************************************/
        internal FreezePatternsTest(Microsoft.Test.ElementServices.Freezables.Utils.Result result)
            : base(result)
        {
            testName = "FreezePatternsTest";
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
            if (IsKnownIssue(t))
            {
                return;
            }
            try
            {
                GlobalLog.LogStatus("ClassName: " + t.ToString());

                Freezable obj = null;

                // create an instance of this type
                obj = FreezablesPatternsHelper.CreateNewChangeable(t);

                // if the object can be in an immutable state, check for its Freeze method
                if (obj != null && obj.CanFreeze)
                {

                    MethodInfo mi = t.GetMethod("Freeze", BindingFlags.Public | BindingFlags.Instance);

                    try
                    {
                        mi.Invoke(obj, null);
                        result.passed &= FreezablesPatternsHelper.CheckNotChangeable(obj);
                    }
                    catch (System.Reflection.TargetInvocationException e)
                    {
                        if (!obj.CanFreeze && e.Message.ToString().StartsWith("Exception has been thrown by the target of an invocation"))
                        {
                            result.passed &= true;
                        }
                        else
                        {
                            result.failures.Add(t.ToString() + "!!!NewObject : Exception: " + e.ToString());
                            GlobalLog.LogEvidence("        !!! NewObject: Exception: {0}", e.ToString());
                            result.passed &= false;
                        }
                    }
                }

                obj = null;
            }
            catch (System.Reflection.AmbiguousMatchException e)
            {
                result.passed &= false;
                result.failures.Add(t.ToString() + ": Freeze method not found");
                GlobalLog.LogEvidence("    !!!Freeze method not found: Exception: {0}", e.ToString());
            }
        }
        #endregion
    }
}
