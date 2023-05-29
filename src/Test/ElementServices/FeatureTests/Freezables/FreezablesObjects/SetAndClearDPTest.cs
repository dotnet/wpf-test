// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Reflection;
using System.Collections;
using System.Security;
using System.Security.Policy;
using System.Windows;
using Microsoft.Test.ElementServices.Freezables.Objects;
using Microsoft.Test.Logging;


namespace Microsoft.Test.ElementServices.Freezables
{
    /**********************************************************************************
    * CLASS:          EventTest
    **********************************************************************************/
    public class SetAndClearDPTest : FreezablesObjectsBase
    {
        #region Constructor
        /******************************************************************************
        * Function:          Constructor
        ******************************************************************************/
        public SetAndClearDPTest(string testName, string objName) : base(testName, objName)
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
                Type type = freezable.GetType();
                FieldInfo[] fieldInfo = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                foreach (FieldInfo field in fieldInfo)
                {
                    if (field.FieldType.ToString() !=  "System.Windows.DependencyProperty")
                    {
                        continue;
                    }
                    DependencyProperty dp = (DependencyProperty)field.GetValue(freezable);
                    object dpValue = PredefinedObjects.MakeValue(dp.PropertyType);
                    if (dpValue == null)
                    {
                        // Nothing much the test can do
                        // This test depends on PredefineObject, if PredefinedObject
                        // cannot make the value, it returns null.
                        continue;
                    }
                    //GlobalLog.LogEvidence("FieldName " + field.Name);
                    TestSetDP(freezable, dp, dpValue);
                    TestClearDP(freezable, dp, dpValue);
                }
            }
        }

        /******************************************************************************
        * Function:          TestSetDP
        ******************************************************************************/
        private void TestSetDP(Freezable freezable, DependencyProperty dp, object dpValue)
        {
            freezable.Freeze();
            try
            {
                // SetValue of DP to a frozen freezable should cause exception 
                freezable.SetValue(dp, dpValue);

                // fail, if not exception raise
                FailTest(dp, freezable, "Setting value ");
            }
            catch (InvalidOperationException e)
            {
                LogTest(e, dp, freezable, "Setting Value ");
            }
        }
        
        /******************************************************************************
        * Function:          TestClearDP
        ******************************************************************************/
        private void TestClearDP(Freezable freezable, DependencyProperty dp, object dpValue)
        { 
            // Make a copy, SetValue, freeze it, then ClearValue and expect the exception
            freezable = freezable.Clone();
            freezable.SetValue(dp, dpValue);
            freezable.Freeze();
            try
            {
                freezable.ClearValue(dp);
    
                // ClearValue of DP to a frozen freezable should cause exception 
                FailTest(dp, freezable, "Clearing value ");
            }
            catch (InvalidOperationException e)
            {
                LogTest(e, dp, freezable, "Clearing Value ");
            }
        }
        
        /******************************************************************************
        * Function:          LogTest
        ******************************************************************************/
        private void LogTest(InvalidOperationException e, DependencyProperty dp, Freezable freezable, string Message)
        {
            if (e.TargetSite.Name != "SetValueCommon" && e.TargetSite.Name != "ClearValueCommon")
            {
                FailTest(dp, freezable, Message);
            }
            // If no failure, Framework will handle the Log Pass
        }
        
        /******************************************************************************
        * Function:          FailTest
        ******************************************************************************/
        private void FailTest(DependencyProperty dp, Freezable freezable, String Message)
        {
            GlobalLog.LogEvidence(Message + dp.GetType().ToString() + " into a frozen freezable " + freezable.GetType().ToString() + " does not cause causes InvalidOperationException, expecting one");
            failures.Add(Message + dp.GetType().ToString() + " into a frozen freezable " + freezable.GetType().ToString() + " does not cause causes InvalidOperationException, expecting one");
            passed &= false;
        }
        #endregion

    }
}
  


