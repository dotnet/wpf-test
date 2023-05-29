// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Events;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Avalon.Test.CoreUI.Events
{
    /// <summary>
    /// Create an custom event using EventHandlersStore on frameworkelement
    /// 
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\Clr
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  CustomClrEvewntOnFCE.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>For Button test DataContextChanged, MouseEnter/Leave, 
    /// IsStylusOverChanged, IsFocussWithinChanged, and Loaded</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.CLR", "CustomClrEvewntOnFCE")]
    public class CustomClrEvewntOnFCE : EventHelper
    {
        #region Private Data
        private int _numCalls;
        #endregion


        #region Constructor
        public CustomClrEvewntOnFCE()
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult StartTest()
        {
            // Create controls            

            CustomContentElement control = new CustomContentElement();
        
            // Add event handlers for Clr event           
            control.CustomClrEvent += new CustomDelegate(CustomClrEventHander);
        
            // Raise the Clr event
            _numCalls = 0;
            control.RaiseCustomClrEvent(EventArgs.Empty, null);

            if (_numCalls != 1)
            {
                throw new Exception("Incorrect Number of Clr Events Fired");
            }
            control.CustomClrEvent += new CustomDelegate(CustomClrEventHander);

            // Raise the Clr event again
            _numCalls = 0;
            control.RaiseCustomClrEvent(EventArgs.Empty, null);

            if (_numCalls != 2)
            {
                throw new Exception("Incorrect Number of Clr Events Fired");
            }

            // Remove event handlers for Clr event           
            control.CustomClrEvent -= new CustomDelegate(CustomClrEventHander);
            
            // Raise the Clr event
            _numCalls = 0;
            control.RaiseCustomClrEvent(EventArgs.Empty, null);

            if (_numCalls != 1)
            {
                throw new Exception("Incorrect Number of Clr Events Fired");
            }

            // Remove event handlers for Clr event           
            control.CustomClrEvent -= new CustomDelegate(CustomClrEventHander);
            
            // Raise the Clr event
            _numCalls = 0;
            control.RaiseCustomClrEvent(EventArgs.Empty, null);
            
            if (_numCalls != 0)
            {
                throw new Exception("Incorrect Number of Clr Events Fired");
            }
            // Remove event handlers that is not exist           
            control.CustomClrEvent -= new CustomDelegate(CustomClrEventHander);

            // Raise the Clr event
            _numCalls = 0;
            control.RaiseCustomClrEvent(EventArgs.Empty, null);

            if (_numCalls != 0)
            {
                throw new Exception("Incorrect Number of Clr Events Fired");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Private Members
        private void CustomClrEventHander(object sender, EventArgs args, object other)
        {
            _numCalls++;
        }
        #endregion
    }
}
