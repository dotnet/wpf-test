// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2006
 *
 *   Program:   Test GetCurrentValueAsFrozen
 
 ************************************************************/

using System;
using System.Globalization;
using System.Reflection;
using System.Windows;

using Microsoft.Test;
using Microsoft.Test.Graphics;
using Microsoft.Test.Logging;
using Microsoft.Test.ElementServices.Freezables.Utils;
using Microsoft.Test.ElementServices.Freezables.Objects;

namespace Microsoft.Test.ElementServices.Freezables
{

    /**********************************************************************************
    * CLASS:          GetCurrentValueAsFrozenPatternsTest
    **********************************************************************************/
    internal class GetCurrentValueAsFrozenPatternsTest : FreezablesPatternsBase
    {

        #region Constructor
        /******************************************************************************
        * Function:          Constructor
        ******************************************************************************/
        internal GetCurrentValueAsFrozenPatternsTest(Microsoft.Test.ElementServices.Freezables.Utils.Result result)
            : base(result)
        {
            testName = "GetCurrentValueAsFrozenPatternsTest";
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
                GlobalLog.LogStatus("ClassName: " + t.ToString());
                Type type = freezable.GetType();
                if (TypeHelper.IsComplexChangeable(type))
                {
                    // ComplexFreezable: Freezable with embedded freezable as its property
                    GetCurrentValueAsFrozenPatternsTestForComplexFreezable(type, freezable);
                }
                else
                {
                    // SimpleFreezable:  Freezable without embedded freezable as its property
                    GetCurrentValueAsFrozenPatternsTestForSimpleFreezable(type, freezable);
                }
            }
        }
        #endregion
   

        #region Private Members
        /******************************************************************************
        * Function:          GetCurrentValueAsFrozenPatternsTestForComplexFreezable
        ******************************************************************************/
        /// <summary>
        /// Testing a Complex Freezable.
        /// </summary>
        /// <param name="type">The Type of the Freezable object to be tested.</param>
        /// <param name="parent">The parent of a Freezable.</param>
        /// <returns></returns>
        private void GetCurrentValueAsFrozenPatternsTestForComplexFreezable(Type type, Freezable parent)
        {
            PropertyInfo[] pi = type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

            for (int i = 0; i < pi.Length; i++)
            {
                Freezable child = FreezablesPatternsHelper.GetComplexChangeable(pi[i], parent);

                if (child == null)
                {
                    continue;
                }

                child.Freeze();

                // The clone should not be the same instance since parent freezable wasn't frozen
                Freezable frozenClone = parent.GetCurrentValueAsFrozen(); 

                if (frozenClone == parent)
                {
                    result.passed &= false;
                    result.failures.Add(type.ToString() + ": The clone should not be the same instance since parent (freezable) wasn't frozen");
                }

                //
                if (   (type != typeof(System.Windows.Media.TransformGroup) )
                    && (type.ToString().ToLower(CultureInfo.InvariantCulture).IndexOf("usingkeyframes") < 0) )
                {
                    // After calling GetCurrentValueAsFrozen(), frozenChild should be equal to child since the child is frozen
                    Freezable frozenChild = FreezablesPatternsHelper.GetComplexChangeable(pi[i], frozenClone);
                    if (frozenChild != child)
                    {
                        result.passed &= false;
                        result.failures.Add(type.ToString() + ": GetCurrentValueAsFrozen should not have made a clone of the child since it is already frozen");
                    }
                }

                // Make a new clone and test for SimpleFreezable
                child = child.Clone();

                GetCurrentValueAsFrozenPatternsTestForSimpleFreezable(type, child);
            }
        }

        /******************************************************************************
        * Function:          GetCurrentValueAsFrozenPatternsTestForSimpleFreezable
        ******************************************************************************/
        /// <summary>
        /// Testing a Freezable, from a Complex Freezable.
        /// </summary>
        /// <param name="type">The Type of the Freezable object to be tested.</param>
        /// <param name="freezable">The Freezable object to be tested.</param>
        /// <returns></returns>
        private void GetCurrentValueAsFrozenPatternsTestForSimpleFreezable(Type type, Freezable freezable)
        {
            Freezable currentFrozen = freezable.GetCurrentValueAsFrozen();

            //IsFrozen should be true
            if (!currentFrozen.IsFrozen)
            {
                result.passed &= false;
                result.failures.Add(type.ToString() + ": IsFrozen should be true after calling GetCurrentValueAsFrozen()");

            }  
         
            // They should not be deep equal
            if (ObjectUtils.DeepEquals(freezable, currentFrozen))
            {
                result.passed &= false;
                result.failures.Add(type.ToString() + ": GetCurrentValueAsFrozen for this object should be equal");
            }
            // Freeze() the freezable, then call GetCurrentValueAsFrozen() again
            // Objects should be equal
            freezable.Freeze();
            currentFrozen = freezable.GetCurrentValueAsFrozen();
            if (!ObjectUtils.DeepEquals(freezable, currentFrozen))
            {
                result.passed &= false;
                result.failures.Add(type.ToString() + ": GetCurrentValueAsFrozen from a frozen freeze should be equal");
            }
        }
        #endregion
    }
}
