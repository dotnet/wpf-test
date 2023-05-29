// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2004
 *
 *   Program:   Test Copy
 
 *
 ************************************************************/

using System;
using System.Reflection;
using System.Windows;

using Microsoft.Test;
using Microsoft.Test.Graphics;
using Microsoft.Test.Logging;
using Microsoft.Test.ElementServices.Freezables.Utils;


namespace Microsoft.Test.ElementServices.Freezables
{
    /**********************************************************************************
    * CLASS:          CopyPatternsTest
    **********************************************************************************/
    internal class CopyPatternsTest : FreezablesPatternsBase
    {

        #region Constructor
        /******************************************************************************
        * Function:          Constructor
        ******************************************************************************/
        internal CopyPatternsTest(Microsoft.Test.ElementServices.Freezables.Utils.Result result)
            : base(result)
        {
            testName = "CopyPatternsTest";
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

                // check if they implement Copy method
                MethodInfo mi = t.GetMethod("Clone", BindingFlags.Public | BindingFlags.Instance , null, new Type[0], new ParameterModifier[0]);

                if (mi == null)
                {
                    // Copy method not found
                    result.passed &= false;
                    result.failures.Add(t.ToString() + ": Clone  method not found");
                    GlobalLog.LogEvidence("    !!!Clone method not found for type {0}", t.ToString());
                    return;
                }

                // create an instance of this type
                Freezable freezable = null;
                if (FreezablesPatternsHelper.IsSpecialFreezable(t))
                {
                    freezable = FreezablesPatternsHelper.CreateSpecialFreezable(t);
                }
                else
                {
                    freezable = FreezablesPatternsHelper.CreateNewChangeable(t);
                }
                if (freezable != null)
                {
                    //GlobalLog.LogEvidence("GoodFreezable: " + freezable.GetType().ToString());
                    Type type = freezable.GetType();
                    MethodInfo method = type.GetMethod("Clone", BindingFlags.Public | BindingFlags.Instance);
                    if (method == null)
                    {

                        throw new ApplicationException("Could not find Clone method on " + type.Name);

                    }
                    Freezable clonedFreezable = (Freezable)method.Invoke(freezable, null);
      
                    if (clonedFreezable.IsFrozen)
                    {
                        result.passed &= false;
                        result.failures.Add(t.ToString() + ": IsFrozen is " + clonedFreezable.IsFrozen);
            
                    }
                    if (!ObjectUtils.DeepEqualsToAnimatable(freezable, clonedFreezable))
                    {

                        result.passed &= false;
                        result.failures.Add(t.ToString() + "Clone() did not produce exact copy");
         
                    }
                }
            }
            catch (System.Reflection.TargetInvocationException e)
            {
                result.passed &= false;
                result.failures.Add(t.ToString() + "-!!!COPY: Exception: " + e.InnerException.ToString());
                GlobalLog.LogEvidence(" !!!COPY: Exception: {0}", e.InnerException.ToString());
            }
            catch (System.Reflection.AmbiguousMatchException e)
            {
                result.passed &= false;
                result.failures.Add(t.ToString() + ": copy method not found");
                GlobalLog.LogEvidence(" !!!COPY method not found: Exception: {0}", e.ToString());
            }
        }
        #endregion
    }
}
