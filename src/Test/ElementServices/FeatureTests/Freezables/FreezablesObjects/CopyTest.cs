// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2005
 *
 *   Program:   Test Copy for freezable
 *   Description: This test does the following:
 *   1. Freeze the object, then do the copy
 *   2. Enumerate the properities of the copy object recursively, get the freezable property
 *      verify that if the freezable property is frezen, fail the test.
 
 *
 ************************************************************/
using System;
using System.Xml;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using System.Security;
using System.Security.Policy;
using System.Windows;

using Microsoft.Test.ElementServices.Freezables.Objects;
using Microsoft.Test.Logging;


namespace Microsoft.Test.ElementServices.Freezables
{
    /**********************************************************************************
    * CLASS:          CopyTest
    **********************************************************************************/
    public class CopyTest : FreezablesObjectsBase
    {
        #region Constructor
        /******************************************************************************
        * Function:          Constructor
        ******************************************************************************/
        public CopyTest(string testName, string objName) : base(testName, objName)
        {
        }
        #endregion


        #region Public and Protected Members
        /******************************************************************************
        * Function:          RunTest
        ******************************************************************************/
        public override void RunTest()
        {
            foreach (Freezable freezable in freezables)
            {
                // Verify the copied object and it subobjects are unfrozen
                freezable.Freeze();

                Freezable copiedObj = freezable.Clone();

                VerifyIsFrozen(copiedObj);      
            }
        }

        /******************************************************************************
        * Function:          VerifyIsFrozen
        ******************************************************************************/
        private void VerifyIsFrozen(Freezable freezable)
        {
            Verify(freezable);
                  
            Type t = freezable.GetType();
            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in pi)
            {
                if (DoesCauseStackOverflow(freezable, prop))
                {
                    // When GetValue from some object (ex: MatrixTransform)
                    // The property value that I get is the same object as it class
                    // This will cause stack overflow, as this process is going thru
                    // Infinite loop. So I have to do this to prevent stack overflow from 
                    // happening.
                    continue;
                }

                if (TypeHelper.IsFreezable(prop.PropertyType))
                {
                    if (freezable is IEnumerable)
                    {
                        // This means that the current Freezable is a collection
                        foreach (Freezable f in (IEnumerable) freezable)
                        {
                            VerifyIsFrozen(f);
                        }
                    }
                    else
                    {
                        Freezable subFreezable = (Freezable)prop.GetValue(freezable, null);
                        VerifyIsFrozen(subFreezable);

                    }
                }
            }
        }

        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        private void Verify(Freezable freezable)
        {
            if (freezable.IsFrozen)
            {
                GlobalLog.LogEvidence(freezable.GetType().ToString() + " should not be frozen ");
                failures.Add(freezable.GetType().ToString() + " should not be frozen ");
                passed &= false;
            }
        }
        #endregion
    }
}
  


