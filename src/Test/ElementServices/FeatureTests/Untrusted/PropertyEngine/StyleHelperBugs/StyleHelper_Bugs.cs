// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Data;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.PropertyEngine;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.UtilityHelper;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;


namespace Avalon.Test.CoreUI.PropertyEngine.TestStyleResourcesApi
{
    /******************************************************************************
    * CLASS:          StyleHelperBugs
    ******************************************************************************/
    [Test(0, "PropertyEngine.StyleHelperApi", TestCaseSecurityLevel.PartialTrust, "StyleHelperBugs")]
    public class StyleHelperBugs : TestCase
    {
        #region Private Data
        private string _testName = "";
        #endregion

        #region Constructor

        [Variation("Bug53")]

        /******************************************************************************
        * Function:          StyleHelperBugs Constructor
        ******************************************************************************/
        public StyleHelperBugs(string arg)
        {
            _testName = arg;
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult StartTest()
        {
            TestStyleHelperApi test = new TestStyleHelperApi();
            TestResult _result;
            switch (_testName)
            {
                // New tests can be entered here
                case "Bug53":
                    _result  = test.Bug53();
                    break;
                default:
                    throw new Microsoft.Test.TestValidationException("ERROR!!! Test case not found.");
            }

            return _result;
        }
        #endregion
    }


    /******************************************************************************
    * CLASS:          TestStyleHelperApi
    ******************************************************************************/
    public class TestStyleHelperApi
    {
        /******************************************************************************
        * Function:          Bug53
        ******************************************************************************/
        /// <summary>
        /// Regression Test for 



        public TestResult Bug53()
        {
            try{
                var viewSource = new CollectionViewSource();
                var be = BindingOperations.GetBindingExpression(viewSource, CollectionViewSource.SourceProperty);
            } catch(Exception ex)
            {
                CoreLogger.LogStatus("Test has thrown an exception : " + ex.ToString());
                return TestResult.Fail;
            }

            // Reaching this line means the test has executed successfully
            return TestResult.Pass;            
        }
    }
}
