// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2004
 *
 *   Program:   Test freezable Constructors
 
 *
 ************************************************************/

using System;
using System.Reflection;
using System.Windows;
using System.Threading;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.ElementServices.Freezables.Utils;


namespace Microsoft.Test.ElementServices.Freezables
{
    /**********************************************************************************
    * CLASS:          ConstructorPatternsTest
    **********************************************************************************/
    internal class ConstructorPatternsTest : FreezablesPatternsBase 
    {
        #region Constructor
        /******************************************************************************
        * Function:          Constructor
        ******************************************************************************/
        internal ConstructorPatternsTest(Microsoft.Test.ElementServices.Freezables.Utils.Result result)
            : base(result)
        {
            testName = "ConstructorPatternsTest";
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
            if (t.IsAbstract || IsKnownIssue(t))
            {
                return;
            }

            GlobalLog.LogStatus("ClassName: " + t.ToString());

            ConstructorInfo[] ci = t.GetConstructors();
            if (t.ToString() == "System.Windows.Media.Imaging.RenderTargetBitmap")
                GlobalLog.LogStatus("");

            // for each constructor
            for (int i = 0; i < ci.Length; i++)
            {
                try
                {
                    System.Windows.Freezable obj = FreezablesPatternsHelper.MakeChangeableObject(t, ci, i);

                    if (obj != null)
                    {
                        if (obj.CanFreeze)
                        {
                            if (obj.IsFrozen)
                            {
                                result.passed &= false;
                                result.failures.Add(t.ToString() + ": IsFrozen is " + obj.IsFrozen );
                            }
                        }
                    }
            
                }
                catch (System.Reflection.TargetInvocationException e)
                {
                    if (!e.InnerException.ToString().StartsWith("System.ArgumentNullException"))
                    {
                        GlobalLog.LogEvidence(" !!!CTOR: Exception: {0}", e.InnerException.ToString());
                        result.failures.Add(t.ToString() + ": System.ArgumentNullException - " + e.InnerException.ToString());
                        result.passed &= false;
                    }
                }
                catch (System.NotImplementedException e)
                {
                    GlobalLog.LogEvidence(" !!!CTOR: Exception: {0}", e.ToString());
                    result.failures.Add(t.ToString() + ": !!!CTOR Exception - " + e.ToString());
                    result.passed &= false;
                }
            }
        }
        #endregion
    }
}
