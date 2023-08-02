// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2003
 *
 *   Program:   Test freezable object properties
 
 *
 ************************************************************/

using System;
using System.Windows;
using System.Reflection;
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
    /// <area>ElementServices\Freezables\Patterns</area>
 
    /// <priority>2</priority>
    /// <description>
    /// Freezables tests
    /// </description>
    /// </summary>

    [Test(2, "Freezables.Patterns", "FreezablesPatterns", SupportFiles=@"FeatureTests\ElementServices\FreezablesPatterns.xtc,FeatureTests\ElementServices\25DPI.jpg,FeatureTests\ElementServices\BORG.jpg")]

    /**********************************************************************************
    * CLASS:          FreezablesPatterns
    **********************************************************************************/
    public class FreezablesPatterns : AvalonTest
    {
        #region Private Data

        public Microsoft.Test.ElementServices.Freezables.Utils.Result result;
        private string              _testName        = "";
        private Type[]              _types;

        #endregion


        #region Constructor

        ////////////////////////////////////////////////////////////////////////////////////////////������� 
        // DISABLEDUNSTABLETEST:
        // TestName:  FreezablesPatterns(CloneCurrentValuePatternsTest)
        // Area: ElementServices �� SubArea: Freezables.Patterns
        // Disable this case due to high fail rate, will enable after fix it.
        // to find all disabled tests in test tree, use: �findstr /snip DISABLEDUNSTABLETEST� 
        ////////////////////////////////////////////////////////////////////////////////////////////
//        [Variation("CloneCurrentValuePatternsTest")]

        [Variation("ConstructorPatternsTest")]

        ////////////////////////////////////////////////////////////////////////////////////////////������� 
        // DISABLEDUNSTABLETEST:
        // TestName:  FreezablesPatterns(CopyPatternsTest)
        // Area: ElementServices �� SubArea: Freezables.Patterns
        // Disable this case due to high fail rate, will enable after fix it.
        // to find all disabled tests in test tree, use: �findstr /snip DISABLEDUNSTABLETEST� 
        ////////////////////////////////////////////////////////////////////////////////////////////
//        [Variation("CopyPatternsTest")]

        ////////////////////////////////////////////////////////////////////////////////////////////������� 
        // DISABLEDUNSTABLETEST:
        // TestName: FreezablesPatterns(EventPatternsTest)
        // Area: ElementServices �� SubArea: Freezables.Patterns
        // Disable this case due to high fail rate, will enable after fix it.
        // to find all disabled tests in test tree, use: �findstr /snip DISABLEDUNSTABLETEST� 
        ////////////////////////////////////////////////////////////////////////////////////////////
//        [Variation("EventPatternsTest")]

//        [Variation("GetCurrentValueAsFrozenPatternsTest")]

        ////////////////////////////////////////////////////////////////////////////////////////////������� 
        // DISABLEDUNSTABLETEST:
        // TestName: FreezablesPatterns(FreezePatternsTest)
        // Area: ElementServices �� SubArea: Freezables.Patterns
        // Disable this case due to high fail rate, will enable after fix it.
        // to find all disabled tests in test tree, use: �findstr /snip DISABLEDUNSTABLETEST� 
        ////////////////////////////////////////////////////////////////////////////////////////////
//        [Variation("FreezePatternsTest")]

        ////////////////////////////////////////////////////////////////////////////////////////////������� 
        // DISABLEDUNSTABLETEST:
        // TestName: FreezablesPatterns(FreezeCorePatternsTest)
        // Area: ElementServices �� SubArea: Freezables.Patterns
        // Disable this case due to high fail rate, will enable after fix it.
        // to find all disabled tests in test tree, use: �findstr /snip DISABLEDUNSTABLETEST� 
        ////////////////////////////////////////////////////////////////////////////////////////////
//        [Variation("FreezeCorePatternsTest")]


        /******************************************************************************
        * Function:          FreezablesPatterns Constructor
        ******************************************************************************/
        // Input Parameter:  TestName
        public FreezablesPatterns(string inputValue)
        {
            _testName = inputValue;

            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(SelectTest);
            RunSteps += new TestStep(Verify);
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Determines the Type requested by a fiven test case.
        /// </summary>
        /// <returns>Returns TestResult=True</returns>
        private TestResult Initialize()
        {
            result = new Microsoft.Test.ElementServices.Freezables.Utils.Result();

            Assembly dll = typeof(UIElement).Assembly;
            _types = dll.GetTypes();

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          SelectTest
        ******************************************************************************/
        /// <summary>
        /// Carries out the requested Freezables tests.  A global variable tracks pass/fail.
        /// </summary>
        /// <returns>Returns TestResult=True</returns>
        private TestResult SelectTest()
        {
            FreezablesPatternsBase test = null;

            switch (_testName)
            {
                case "CloneCurrentValuePatternsTest":
                    test = new CloneCurrentValuePatternsTest(result);
                    break;
                
                case "ConstructorPatternsTest":
                    test = new ConstructorPatternsTest(result);
                    break;

                case "CopyPatternsTest":
                    test = new CopyPatternsTest(result);
                    break;

                case "EventPatternsTest":
                    test = new EventPatternsTest(result);
                    break;

                case "GetCurrentValueAsFrozenPatternsTest":
                    test = new GetCurrentValueAsFrozenPatternsTest(result);
                    break;
                
                case "FreezePatternsTest":
                    test = new FreezePatternsTest(result);
                    break;

                case "FreezeCorePatternsTest":
                    test = new FreezeCorePatternsTest(result);
                    break;

                default:
                    throw new ApplicationException("!!!Unknown test name: " + _testName);
            }

            foreach (Type t in _types)
            {

                if (TypeHelper.IsFreezable(t))
                {
                    test.Perform(t);
                }
            }

            return TestResult.Pass;
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
            // report the failures all together
            if (!result.passed)
            {
                GlobalLog.LogEvidence("-------------------------------------------------");
                GlobalLog.LogEvidence("FAILURE  REPORT");
                for (int i = 0; i < result.failures.Count; i++)
                {
                    GlobalLog.LogEvidence(result.failures[i]);
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
