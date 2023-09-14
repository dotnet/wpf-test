// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2004
 *
 *   Program:   Test freezable object
 
 *
 ************************************************************/

using System;
using System.Reflection;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.ElementServices.Freezables.Objects;


namespace Microsoft.Test.ElementServices.Freezables
{
    /// <summary>
    /// <area>ElementServices\Freezables\MicroBVT</area>
 
    /// <priority>0</priority>
    /// <description>
    /// MicroBVT tests for Freezables
    /// </description>
    /// </summary>

    [Test(0, "Freezables.MicroBVT", "FreezablesMicroBVT")]

    /**********************************************************************************
    * CLASS:          FreezablesMicroBVT
    **********************************************************************************/
    public class FreezablesMicroBVT : AvalonTest
    {
        #region Private Data

        private bool                _passed;
        private StringCollection    _failures;
        private bool                _eventfired;
        
        #endregion


        #region Constructor
        /******************************************************************************
        * Function:          FreezablesMicroBVT Constructor
        ******************************************************************************/
        public FreezablesMicroBVT()
        {
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(StartTestCase);
            RunSteps += new TestStep(Verify);
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Sets global variables.
        /// </summary>
        /// <returns>Returns TestResult=True</returns>
        private TestResult Initialize()
        {
            _failures = new StringCollection();
            _eventfired = false;
            _passed = true;

            return TestResult.Pass;
        }


        /******************************************************************************
        * Function:          StartTestCase
        ******************************************************************************/
        /// <summary>
        /// Carries out a series of basic Freezables tests.  A global variable tracks pass/fail.
        /// </summary>
        /// <returns>Returns TestResult=True</returns>
        private TestResult StartTestCase()
        {
            //------------------------------------------------
            GlobalLog.LogStatus("Case #1: When Freezable is created, make sure !IsFrozen");
            MyFreezable mc = new MyFreezable();

            if (mc.IsFrozen)
            {
                _passed &= false;
                _failures.Add("Case#1 - !IsFrozen is false, expected true");
                GlobalLog.LogEvidence("Case#1 - FAIL: !IsFrozen is false, expected true");
            }

            //------------------------------------------------
            GlobalLog.LogStatus("Case #2: Call Freeze(), make sure IsFozen == true;");
            mc.Freeze();
            if (!mc.IsFrozen)
            {
                _passed &= false;
                _failures.Add("Case#2 - !IsFrozen is true, expected false");
                GlobalLog.LogEvidence("Case#2 - FAIL: !IsFrozen is true, expected false");
            }

            //------------------------------------------------
            GlobalLog.LogStatus("Case #4: Make sure UIContext == null, since !IsFrozen is false");
            if (mc.Dispatcher != null)
            {
                _passed &= false;
                _failures.Add("Case#4 - Dispatcher should be be null");
                GlobalLog.LogEvidence("Case#4 -FAIL: Dispatcher should be null");
            }

            //------------------------------------------------
            GlobalLog.LogStatus("Case #5: When Freezable is copied, make sure !IsFrozen");
            Freezable mc2 = mc.Clone();
            if (mc2.IsFrozen)
            {
                _passed &= false;
                _failures.Add("Case#5 - !IsFrozen is false, expected true");
                GlobalLog.LogEvidence("Case#5 - FAIL: !IsFrozen is false, expected true");
            }
   
            //------------------------------------------------
            GlobalLog.LogStatus("Case #6: Test Changed event");
            LineGeometry line = new LineGeometry();
            MyFreezable mc3 = new MyFreezable(line);
            mc3.Changed += new EventHandler(SimpleEventTest);
            mc3.FreezableObj = null;
            if (!_eventfired)
            {
                _passed &= false;
                _failures.Add("Case#6 - Event not fired");
                GlobalLog.LogEvidence("Case#6 - FAIL: Event not fired");
            }

            //------------------------------------------------
            // Report the failures all together
            if (!_passed)
            {
                GlobalLog.LogStatus("-------------------------------------------------");
                GlobalLog.LogEvidence("FAILURE  REPORT");
                for (int i = 0; i < _failures.Count; i++)
                {
                    GlobalLog.LogEvidence(_failures[i]);
                }
            }

            return TestResult.Pass;
        }


        /******************************************************************************
        * Function:          SimpleEventTest
        ******************************************************************************/
        /// <summary>
        /// Event handler for Changed event.
        /// </summary>
        /// <param name="sender">The object involved with the event.</param>
        /// <param name="args">Event args.</param>
        /// <returns></returns>
        private void SimpleEventTest(Object sender, EventArgs args)
        {
            _eventfired = true;
        }


        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        /// <summary>
        /// Returns a Pass/Fail result for the test case.
        /// </summary>
        /// <returns>A TestResult, indicating whether or not the test passed.</returns>
        private TestResult Verify()
        {
            if (_passed)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        #endregion
    }
}
