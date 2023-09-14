// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2006
 *
 *   Program:   Test freezable collections
 
 *
 ************************************************************/

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.ElementServices.Freezables.Objects;
using Microsoft.Test.ElementServices.Freezables.Utils;

namespace Microsoft.Test.ElementServices.Freezables
{
    /// <summary>
    /// <area>ElementServices\Freezables\Collections</area>
 
    /// <priority>2</priority>
    /// <description>
    /// Freezables collection tests
    /// </description>
    /// </summary>

    [Test(2, "Freezables.Collections", "FreezablesCollections")]

    /**********************************************************************************
    * CLASS:          FreezablesCollections
    **********************************************************************************/
    public class FreezablesCollections : AvalonTest
    {
        #region Private Data

        public Microsoft.Test.ElementServices.Freezables.Utils.Result result;
        private string              _testName        = "";
        
        #endregion


        #region Constructors
        
        [Variation("Brush")]
        [Variation("Geometry")]
        [Variation("Transform")]

        /******************************************************************************
        * Function:          FreezablesCollections Constructor
        ******************************************************************************/
        public FreezablesCollections(string inputValue)
        {
            _testName = inputValue;

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
        /// Initializing.
        /// </summary>
        /// <returns>Returns TestResult=True</returns>
        private TestResult Initialize ()
        {
            result = new Microsoft.Test.ElementServices.Freezables.Utils.Result();

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          StartTestCase
        ******************************************************************************/
        /// <summary>
        /// Carries out a series of Freezables Collection tests.  A global variable tracks pass/fail.
        /// </summary>
        /// <returns>Returns TestResult=True</returns>
        private TestResult StartTestCase ()
        {
            // 


            switch (_testName)
            {
                case "Brush":
                {
                    BrushCollectionTest b = new BrushCollectionTest();
                    result = b.Perform();
                    break;
                }
                case "Geometry":
                {
                    GeometryCollectionTest g = new GeometryCollectionTest();
                    result = g.Perform();
                    break;
                }
                case "Transform":
                {
                    TransformCollectionTest t = new TransformCollectionTest();
                    result = t.Perform();
                    break;
                }
                default:
                    throw new ApplicationException("Unknown class: " + _testName);
            }

            return TestResult.Pass;
        }


        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        /// <summary>
        /// Returns a Pass/Fail result for the test case.
        /// </summary>
        /// <returns>Returns TestResult=True</returns>
        private TestResult Verify()
        {
            // report the failures all together
            if (!result.passed)
            {
                GlobalLog.LogEvidence("------------------------------------------");
                GlobalLog.LogEvidence("FAILURE REPORT");
                GlobalLog.LogEvidence("------------------------------------------");
                for (int i = 0; i < result.failures.Count; i++)
                {
                    GlobalLog.LogEvidence (result.failures[i]);
                }
            }

            if (result.passed)
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
