// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Security;
using System.Windows;
using System.Windows.Data;

using Microsoft.Test.Logging;

namespace Microsoft.Test.ElementServices.Freezables
{
    public class DataBindingTest : FreezablesObjectsBase
    {
        #region Private Data
        private bool _isEventFired;
        #endregion


        #region Constructor
        /******************************************************************************
        * Function:          Constructor
        ******************************************************************************/
        public DataBindingTest(string testName, string objName) : base(testName, objName)
        {
        }
        #endregion


        #region Public and Protected Members
        /******************************************************************************
        * Function:          RunTest
        ******************************************************************************/
        public override void RunTest()
        {
            int i = 0;
            foreach (Freezable freezable in freezables)
            {
                DataBindingBase dataObject = (DataBindingBase)dataBindingObjs[i];
                i++;
                Binding binding = new Binding("DataProperty");
                binding.Source = dataObject;
                freezable.Changed += new EventHandler(FreezableEventHandler);

                // TestEvent - event should fire whe call SetBinding() on a freezable
                _isEventFired = false;
                BindingOperations.SetBinding(freezable, dataObject.DP, binding);
                if (!_isEventFired)
                {
                    GlobalLog.LogEvidence("Changed event not fired when called SetBinding() on " + freezable.GetType().ToString());
                    failures.Add("Changed event not fired when called SetBinding() on " + freezable.GetType().ToString());
                    passed = false;
                }

                // Verify that CanFreeze == false since a data-bound freezable can not be frozen
                if (freezable.CanFreeze)
                {
                    GlobalLog.LogEvidence("A data-bound freezable can not be frozen. Expect: CanFreeze == false, Actual CanFreeze == " + freezable.CanFreeze);
                    failures.Add("A data-bound freezable can not be frozen. Expect: CanFreeze == false, Actual CanFreeze == " + freezable.CanFreeze);
                    passed = false;
       
                }

                // TestEvent - when changing a freezable property inside DataBinding object
                // Changed event should fired.
                _isEventFired = false;
                dataObject.UpdateFreezableProperty();
                if (!_isEventFired)
                {
                    GlobalLog.LogEvidence("Changed event not fired when changing property value on a freezable in DataBinding object " + freezable.GetType().ToString());
                    failures.Add("Changed event not fired when changing proerty value on a freezable in DataBinding object " + freezable.GetType().ToString());
                    passed = false;
                }
            }
        }
 
        /******************************************************************************
        * Function:          FreezableEventHandler
        ******************************************************************************/
        public void FreezableEventHandler(Object sender, EventArgs args)
        {
            _isEventFired = true;
        }
        #endregion
    }
}
  


