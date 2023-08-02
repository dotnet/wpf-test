// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Reflection;
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
    public class EventTest : FreezablesObjectsBase
    {
        #region Private Data
        private bool _isEventFired;
        #endregion


        #region Constructor
        /******************************************************************************
        * Function:          Constructor
        ******************************************************************************/
        public EventTest(string testName, string objName) : base(testName, objName)
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
                if (freezable.IsFrozen)
                {
                    //Cannot modify a frozen object, so bail out
                    return;
                }

                Type t = freezable.GetType();

                PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                freezable.Changed += new EventHandler(FreezableEventHandler);
                for (int i = 0; i < pi.Length; i++)
                {
                    if (pi[i].CanWrite)
                    {
                        Object arg = new Object();
                        arg = PredefinedObjects.MakeValue(pi[i].PropertyType);
                        if (arg != null)
                        {
                            Object[] index = null;
                            // collection
                            if (pi[i].Name == "Item" || pi[i].Name.ToString().EndsWith(".Item"))
                            {
                                index = new Object[] { 0 };
                            }
                            // we need to get the mathching DP for this property
                            // and if the value (arg in this case) is the default value
                            // we don't need to call SetValue, since event won't fire
                            // when setting default value.
                            DependencyProperty dp = GetMatchingDP(pi[i], freezable);
                            if (dp != null)
                            {
                                if (arg.Equals(dp.DefaultMetadata.DefaultValue))
                                {
                                    return;
                                }
                            }

                            _isEventFired = false;

                            object value = pi[i].GetValue(freezable, null);
                            pi[i].SetValue(freezable, arg, index);
                            if (!_isEventFired)
                            {
                                GlobalLog.LogEvidence("Event not fired when setting value into " + t.FullName + "." + pi[i].Name);
                                failures.Add("Event not fired when setting value into " + t.FullName + pi[i].Name);
                                passed &= false;

                            }
                        }
                    }
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
  


