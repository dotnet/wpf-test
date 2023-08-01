// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2006
 *
 *   Program:   Perform a roundtrip test on a xaml
 
 *
 ************************************************************/

using System;

using Microsoft.Test;
using Microsoft.Test.Serialization;
using Microsoft.Test.Logging;
using Microsoft.Test.Discovery;


namespace Microsoft.Test.ElementServices.Freezables
{
    /// <summary>
    /// <area>ElementServices\Freezables\RoundTrip</area>
 
    /// <priority>3</priority>
    /// <description>
    /// RoundTrip tests - Serialization
    /// </description>
    /// </summary>

    [Test(3, "Freezables.RoundTrip", "RoundTrip", SupportFiles=@"FeatureTests\ElementServices\png.png,FeatureTests\ElementServices\puppies.jpg")]

    /**********************************************************************************
    * CLASS:          RoundTrip
    **********************************************************************************/
    public class RoundTrip
    {
        #region Private Data
        private TestLog         _log;
        private string          _xamlFile        = "";
        #endregion


        #region Constructor
        [Variation(@"FeatureTests\ElementServices\brushes.xaml", SupportFiles = @"FeatureTests\ElementServices\brushes.xaml")]
        [Variation(@"FeatureTests\ElementServices\databindingColorAnimToProperty.xaml", SupportFiles = @"FeatureTests\ElementServices\databindingColorAnimToProperty.xaml")]
        [Variation(@"FeatureTests\ElementServices\drawingbrush.xaml", SupportFiles = @"FeatureTests\ElementServices\drawingbrush.xaml")]
        [Variation(@"FeatureTests\ElementServices\geometries.xaml", SupportFiles = @"FeatureTests\ElementServices\geometries.xaml")]
        [Variation(@"FeatureTests\ElementServices\Transforms.xaml", SupportFiles = @"FeatureTests\ElementServices\Transforms.xaml")]

        /******************************************************************************
        * Function:          Constructor
        ******************************************************************************/
        // Input Parameter:  TestName
        public RoundTrip(string inputValue)
        {
            _xamlFile = inputValue;
        }
        #endregion


        #region Public and Protected Members
        /******************************************************************************
        * Function:          Run
        ******************************************************************************/
        /// <summary>
        /// Runs the Test and returns the Final Result of the Test.
        /// </summary>
        /// <returns>the Final Result of the Test</returns>
        public TestLog Run()
        {
            _log = new TestLog("RoundTrip");

            StartTest();

            if (TestLog.Current != null)
            {
                TestLog.Current.Result = _log.Result;
            }

            _log.Close();  //Must close the log to avoid a Failure.

            return _log;
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Carries out the requested RoundTrip tests.  A global variable tracks pass/fail.
        /// </summary>
        /// <returns></returns>
        private void StartTest()
        {
            SerializationHelper helper = new SerializationHelper();

            try
            {
                int indx = _xamlFile.LastIndexOf("\\");
                _xamlFile = _xamlFile.Substring(indx+1);

                GlobalLog.LogStatus("Starting test for " + _xamlFile);
                helper.RoundTripTestFile(_xamlFile);
                GlobalLog.LogStatus("No exception for " + _xamlFile);

                _log.Result = TestResult.Pass;
            }
            catch (Exception e)
            {
                GlobalLog.LogEvidence("Unexpected Exception occurred:\n" + e.ToString());
                _log.Result = TestResult.Fail;
            }
        }
        #endregion
    }
}
