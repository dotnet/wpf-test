// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Avalon.Test.CoreUI.Events
{
    /******************************************************************************
    * CLASS:          RoutedEventHandlerInfoTest
    ******************************************************************************/
    /// <summary>
    /// </summary>
    [Test(0, "Events", "RoutedEventHandlerInfoTest")]
    public class RoutedEventHandlerInfoTest : WindowTest
    {
        #region Constructor
        /******************************************************************************
        * Function:          RoutedEventHandlerInfoTest Constructor
        ******************************************************************************/
        public RoutedEventHandlerInfoTest()
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// TestEquality.
        /// ConditionCollection.IsSealed from Code and Condition ctor with BindingBase param
        /// </summary>
        TestResult StartTest()
        {
            object r1 = new RoutedEventHandlerInfo();
            object r2 = new RoutedEventHandlerInfo();

            Delegate g = new EventHandler(foo);
            Delegate g1 = new EventHandler(bar);
            
            AddValues( r1,g, true); 
            AddValues( r2,g, true); 

            if (!Validate(r1,r2,true))
            {
                GlobalLog.LogEvidence("RoutedEventHandlerInfo.Equals(RoutedEventHandlerInfo) failed 1");
                return TestResult.Fail;
            }

            r1 = new RoutedEventHandlerInfo();
            r2 = new RoutedEventHandlerInfo();

            AddValues( r1,g1, true); 
            AddValues( r2,g, true);         
            
            if (!Validate(r1,r2,false))
            {
                GlobalLog.LogEvidence("RoutedEventHandlerInfo.Equals(RoutedEventHandlerInfo) failed 2");
                return TestResult.Fail;
            }

            r1 = new RoutedEventHandlerInfo();
            r2 = new RoutedEventHandlerInfo();
            
            AddValues( r1,g1, true); 
            AddValues( r2,g, false);                

            if (!Validate(r1,r2,false))
            {
                GlobalLog.LogEvidence("RoutedEventHandlerInfo.Equals(RoutedEventHandlerInfo) failed 3");
                return TestResult.Fail;
            }

            r1 = new RoutedEventHandlerInfo();
            r2 = new RoutedEventHandlerInfo();

            AddValues( r1,g, false); 
            AddValues( r2,g, true);              
            
            if (!Validate(r1,r2,false))
            {
                GlobalLog.LogEvidence("RoutedEventHandlerInfo.Equals(RoutedEventHandlerInfo) failed");
                return TestResult.Fail;
            }

            GlobalLog.LogEvidence("PASS");

            return TestResult.Pass;
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          AddValues
        ******************************************************************************/
        private void AddValues(object r, Delegate d, bool b)
        {
            Type t = r.GetType();
            FieldInfo f = t.GetField("_handler", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.GetField | BindingFlags.SetField);
            f.SetValue(r, d);

            f = t.GetField("_handledEventsToo", BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField);
            f.SetValue(r, b);
        }

        /******************************************************************************
        * Function:          Validate
        ******************************************************************************/
        private bool Validate(object r1, object r2, bool areEquals)
        {
            if (areEquals && (RoutedEventHandlerInfo)r1 == (RoutedEventHandlerInfo)r2 && (RoutedEventHandlerInfo)r2 == (RoutedEventHandlerInfo)r1)
            {
                return true;
            }
            else if (!areEquals && (RoutedEventHandlerInfo)r1 != (RoutedEventHandlerInfo)r2 && (RoutedEventHandlerInfo)r2 != (RoutedEventHandlerInfo)r1)
            {
                return true;
            }
            return false;
        }

        void foo(object o, EventArgs args)
        {
        }

        void bar(object o, EventArgs args)
        {
        }
        #endregion
    }
}


