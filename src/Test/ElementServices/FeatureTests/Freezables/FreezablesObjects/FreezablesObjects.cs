// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2004
 *
 *   Program:   Test freezable objects
 
 *
 ************************************************************/

using System;
using System.Reflection;
using System.Collections.Specialized;
using System.Windows;
using System.Xml;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;


namespace Microsoft.Test.ElementServices.Freezables
{
    /// <summary>
    /// <area>ElementServices\Freezables\Objects</area>
 
    /// <priority>3</priority>
    /// <description>
    /// BVT tests for Freezables
    /// </description>
    /// </summary>

    [Test(3, "Freezables.Objects", "FreezablesObjects")]

    /**********************************************************************************
    * CLASS:          FreezableObjects
    **********************************************************************************/
    public class FreezablesObjects : AvalonTest
    {
        #region Private Data

        public static bool          testPassed      = false;
        private string              _testName        = "";
        private string              _objName         = "";

        #endregion


        #region Constructor
        // [DISABLE WHILE PORTING]
        // [Variation("AttachedDPTest", "Brush")]
        // [Variation("AttachedDPTest", "Geometry")]
        // [Variation("AttachedDPTest", "Pen")]
        [Variation("AttachedDPTest", "Transform")]

//BUG-BUG:         [Variation("CopyTest", "Brush")]     // get "MatrixTransform should not be frozen" error, whether or not one is assigned to the Brush.
//BUG-BUG:         [Variation("CopyTest", "Pen")]       // get several "should not be frozen" errors.
        [Variation("CopyTest", "Geometry")]             // avoid "MatrixTransform should not be frozen" error by assigning a Transform to the Geometry.
        [Variation("CopyTest", "Transform")]

        [Variation("DataBindingTest", "Brush", Priority=2)]
        [Variation("DataBindingTest", "Geometry", Priority=2)]
        [Variation("DataBindingTest", "Pen", Priority=2)]

        [Variation("EventTest", "Brush", Priority=0)]
        [Variation("EventTest", "Geometry", Priority=1)]
        [Variation("EventTest", "Pen", Priority=1)]
        [Variation("EventTest", "Transform", Priority=1)]

        [Variation("ExceptionTest", "Brush", Priority=1)]
        [Variation("ExceptionTest", "Geometry", Priority=0)]
        [Variation("ExceptionTest", "Pen", Priority=1)]
        [Variation("ExceptionTest", "Transform", Priority=1)]

        [Variation("GetAsFrozenTest", "Brush")]
        [Variation("GetAsFrozenTest", "Geometry")]
        [Variation("GetAsFrozenTest", "Pen")]
        [Variation("GetAsFrozenTest", "Transform")]

        [Variation("MultiThreadsTest", "Brush")]
        [Variation("MultiThreadsTest", "Geometry")]
        [Variation("MultiThreadsTest", "Pen")]
        [Variation("MultiThreadsTest", "Transform")]

        [Variation("SetAndClearDPTest", "Brush")]
        [Variation("SetAndClearDPTest", "Geometry")]
        [Variation("SetAndClearDPTest", "Pen")]
        [Variation("SetAndClearDPTest", "Transform")]


        /******************************************************************************
        * Function:          FreezablesObjects Constructor
        ******************************************************************************/
        public FreezablesObjects(string inputValue0, string inputValue1)
        {
            _testName = inputValue0;
            _objName = inputValue1;
            RunSteps += new TestStep(StartTestCase);
            RunSteps += new TestStep(Verify);
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          StartTestCase
        ******************************************************************************/
        /// <summary>
        /// Carries out a series of basic Freezables tests.  A global variable tracks pass/fail.
        /// </summary>
        /// <returns>Returns TestResult=True</returns>
        private TestResult StartTestCase ()
        {
            FreezablesObjectsBase freezableBase = null;

            switch (_testName)
            {
                case "AttachedDPTest":
                {
                    freezableBase = new AttachedDPTest(_testName, _objName);
                    break;
                }
                case "CopyTest":
                {
                    freezableBase = new CopyTest(_testName, _objName);
                    break;
                }
                case "DataBindingTest":
                {
                    freezableBase = new DataBindingTest(_testName, _objName);
                    break;
                }
                case "ExceptionTest":
                {
                    freezableBase = new ExceptionTest(_testName, _objName);
                    break;
                }
                case "EventTest":
                {
                    freezableBase = new EventTest(_testName, _objName);
                    break;
                }
                case "GetAsFrozenTest":
                {
                    freezableBase = new GetAsFrozenTest(_testName, _objName);
                    break;
                }
                case "MultiThreadsTest":
                {
                    freezableBase = new MultiThreadsTest(_testName, _objName);
                    break;
                }
                case "SetAndClearDPTest":
                {
                    freezableBase = new SetAndClearDPTest(_testName, _objName);
                    break;
                }
                default:
                    throw new ApplicationException("Unknown test name: " + _testName);
            }

            freezableBase.Perform();

            return TestResult.Pass;
        }


        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        /// <summary>
        /// Returns a Pass/Fail result for the test case.
        /// </summary>
        /// <returns></returns>
        private TestResult Verify()
        {
            if (testPassed)
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
