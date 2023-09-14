// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2004
 *
 *   Program:   Test CloneCurrentValue
 
 *   Description: This test call CloneCurrentValue() then does deep comparion
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
    * CLASS:          CloneCurrentValuePatternsTest
    **********************************************************************************/
    internal class CloneCurrentValuePatternsTest : FreezablesPatternsBase
    {

        #region Constructor
        /******************************************************************************
        * Function:          Constructor
        ******************************************************************************/
        internal CloneCurrentValuePatternsTest(Microsoft.Test.ElementServices.Freezables.Utils.Result log)
            : base(log)
        {
            testName = "CloneCurrentValuePatternsTest";
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
            GlobalLog.LogStatus("ClassName: " + t.ToString());

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
                //Console.WriteLine("GoodFreezable: " + freezable.GetType().ToString());
                Type type = freezable.GetType();
                MethodInfo method = type.GetMethod("CloneCurrentValue", BindingFlags.Public | BindingFlags.Instance);
                if (method == null)
                {

                    throw new ApplicationException("Could not find CloneCurrentValue method on " + type.Name);
                }

                Freezable currentFreezable = (Freezable)method.Invoke(freezable, null);
                // They should be equal
                if (!ObjectUtils.DeepEquals(freezable, currentFreezable))
                {
                    result.passed &= false;
                    result.failures.Add(t.ToString() + ": CloneCurrentValue for this object should be equal");
                }
                // Freeze the freezable, then GetCurrentValue() again
                // Objects should not be equal
                freezable.Freeze();
                currentFreezable = (Freezable)method.Invoke(freezable, null);
                if (ObjectUtils.DeepEquals(freezable, currentFreezable))
                {
                    result.passed &= false;
                    result.failures.Add(t.ToString() + ": CloneCurrentValue from a frozen freeze should not be equal");
                }
            }
        }
        #endregion
    }
}
