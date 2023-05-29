// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Reflection;
using System.Security;
using System.Security.Policy;
using System.Collections;
using System.Windows;

using Microsoft.Test.ElementServices.Freezables.Objects;
using Microsoft.Test.Logging;


namespace Microsoft.Test.ElementServices.Freezables
{
    /**********************************************************************************
    * CLASS:          GetAsFrozenTest
    **********************************************************************************/
    public class GetAsFrozenTest : FreezablesObjectsBase
    {
        #region Constructor
        /******************************************************************************
        * Function:          Constructor
        ******************************************************************************/
        public GetAsFrozenTest(string testName, string objName) : base(testName, objName)
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
                VerifyGetAsFrozen(freezable);
            }
        }


        /******************************************************************************
        * Function:          VerifyGetAsFrozen
        ******************************************************************************/
        private void VerifyGetAsFrozen(Freezable freezable)
        {
            Type t = freezable.GetType();
            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in pi)
            {
                if (!TypeHelper.IsFreezable(property.PropertyType))
                {
                    // no need to verify
                    continue;
                }
                if (DoesCauseStackOverflow(freezable, property))
                {
                    // When GetValue from some object (ex: MatrixTransform)
                    // The property value that I get is the same object as it class
                    // This will cause stack overflow, as this process is going thru
                    // Infinite loop. So I have to do this to prevent stack overflow from 
                    // happening.
                    continue;
                }
                if (property.Name == "Item" )
                {
                    if (freezable is IEnumerable)
                    {
                        // This means that the current Freezable is a collection
                        VerifyForCollection((IEnumerable)freezable, property);
                    }
                }
                else
                {
                    Freezable obj = (Freezable)property.GetValue(freezable, null);
                    if (obj != null)
                    {
                        VerifyGetAsFrozen(obj);
                    }
                    Verify(freezable, property, false, 0);
                }
            }
        }


        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        private void Verify(Freezable freezable, PropertyInfo pi, bool isCollection, int index)
        {
            object[] indexObj = null;
            if (isCollection)
            {
                indexObj = new object[] { index };
            }
            // Get a subobject, and freeze it, so that we can test GetAsFrozenTest
            Freezable obj = (Freezable)pi.GetValue(freezable, indexObj);
            if (obj == null)
            {
                return;
            }

            obj.Freeze();

            Freezable clonedObj = freezable.GetAsFrozen();
            Freezable clonedSubObj = (Freezable)pi.GetValue(clonedObj, indexObj);
            if (pi.Name == "Inverse" && TypeHelper.IsDerivative(freezable.GetType(), "System.Windows.Media.Transform"))
            {
                // This is by design, Transform.Inverse return a new instance everytime.
                return;
            }
            if (clonedSubObj != obj || !clonedObj.IsFrozen)
            {
                GlobalLog.LogEvidence(freezable.GetType().ToString() + " does not stop cloning when sub object " + pi.Name + " is frozen");
                failures.Add(freezable.GetType().ToString() + " does not stop cloning when sub object " + pi.Name + " is frozen");
                passed &= false;

            }
        }


        /******************************************************************************
        * Function:          VerifyForCollection
        ******************************************************************************/
        private void VerifyForCollection(IEnumerable freezable, PropertyInfo pi)
        {
            int i = 0;
            foreach (Freezable f in freezable)
            {
                VerifyGetAsFrozen(f);
                Verify((Freezable)freezable, pi, true, i);
                i++;
            }
        }
        #endregion
    }
}
  


