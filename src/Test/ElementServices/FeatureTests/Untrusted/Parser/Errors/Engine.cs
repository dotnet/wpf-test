// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Please see comments on class declaration below.
 *          
 * Contributor: 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/

using System;
using System.Collections;
using Avalon.Test.CoreUI.Trusted;
//using Avalon.Test.CoreUI.CoreTestsHarness;

namespace Avalon.Test.CoreUI.Parser.Error
{
    /// <summary>
    /// Driving engine for error tests.
    /// 
    /// This engine sets up the three components of error testing framework:
    /// DataManager, TestExecutor and ErrComparer
    ///                        
    /// After that, it runs the following loop             
    ///      1. Get the expectedErrData from the DataManager for the current test file            
    ///      2. Pass the expectedErrData to the TestExecutor.            
    ///      3. Get the actualErrData from the TestExecutor
    ///      4. Pass the expectedErrData and actualErrData to the ErrComparer
    ///      5. Get the ShouldChange decision value from the ErrComparer
    ///      6. If ShouldChange is true, pass actualErrData to the DataManager and ask it 
    ///         to update the master (expectedErrData)
    ///      7. Ask the DataManager to go to the next test file.
    ///         
    ///      After the loop is over, ask DataManager to save changes to error file (master).
    /// </summary>
    public class Engine
    {
        /// <summary>
        /// Test case entry point
        /// </summary>
        public void RunTest()
        {
            // Retrieve info from ErrorTestCaseInfo
            ErrorTestCaseInfo etci = (ErrorTestCaseInfo)TestCaseInfo.GetCurrentInfo();
            string errorFilename = etci.ErrorFile;
            string route = etci.Route;
            string reportingMode = etci.ReportingMode;

            // Setup the three components of error testing framework that this engine drives:
            // DataManager, TestExecutor and ErrComparer
            switch (route)
            {
                case "XamlLoad":
                    this.DataManager = new XamlLoadDataManager(errorFilename);
                    this.TestExecutor = new XamlLoadTestExecutor();
                    break;

                case "XamlCompile":
                    this.DataManager = new XamlCompileDataManager(errorFilename);
                    this.TestExecutor = new XamlCompileTestExecutor();
                    break;

                default:
                    throw new Microsoft.Test.TestSetupException("Route " + route + " is not valid");
                    break;
            }

            object errComparerArgs = null;
            switch (reportingMode)
            {
                case "Interactive":
                    this.ErrComparer = new GUIErrComparer();
                    break;

                case "NonInteractive":
                    this.ErrComparer = new ReportingErrComparer();
                    break;

                default:
                    throw new Microsoft.Test.TestSetupException("ReportingMode " + reportingMode + " is not valid");
                    break;
            }

            // Run the loop described in the comments on this class
            Hashtable expectedErrData = null;
            Hashtable actualErrData = null;
            bool testFailed = false;

            while (null != (expectedErrData = DataManager.ReadNextRecord()))
            {
                String strFailingXaml = null;
                try
                {
                    actualErrData = TestExecutor.Run(expectedErrData);
                    strFailingXaml = expectedErrData["XamlFileName"] as String;
                    bool errorsSame = true;
                    bool shouldChange = ErrComparer.Run(expectedErrData, actualErrData, errComparerArgs, ref errorsSame);
                    if (shouldChange)
                    {
                        DataManager.ChangeCurrentRecord(actualErrData);
                    }


                    if (!errorsSame)
                    {
                        // If even one file throws an error different from expected, the test fails
                        testFailed = true;
                    }

                    CoreLogger.LogStatus("\nThe initial result (prior to change) for " + strFailingXaml + " is : "
                        + (errorsSame? "passed" : "failed") + "\n");

                }
                catch (Exception e)
                {
                    
                    if ((null != expectedErrData) && (expectedErrData.Count > 0))
                    {
                        strFailingXaml = expectedErrData["XamlFileName"] as String;
                    }

                    if (null != strFailingXaml)
                    {
                        CoreLogger.LogStatus(   "\nThere was a failure when trying to run Error Comparison for file: " 
                                                + strFailingXaml + "\n");
                    }
                    else
                    {
                        CoreLogger.LogStatus("\nThere was a failure when trying to run Error Comparison for one of the files in: " 
                                                + errorFilename + "\n");
                    }

                    CoreLogger.LogStatus("The Error was: \n" + e.ToString() + "\n");
                    if (null != e.InnerException)
                    {
                        CoreLogger.LogStatus("There was an Inner Exception: \n" + e.InnerException.ToString() + "\n");
                    }

                    testFailed = true;
                    if (e is Microsoft.Test.TestValidationException)
                    {
                        throw new Microsoft.Test.TestValidationException(e.ToString()); 
                    }
                    continue;
                }
                DataManager.PersistChanges();
            }
            //Done. Now ask DataManager to persist all changes.


            if (testFailed)
            {
                throw new Microsoft.Test.TestValidationException("Actual error different from expected error, for atleast one file.");
            }
        }

        private DataManager _dataManager;
        private DataManager DataManager
        {
            get { return _dataManager; }
            set { _dataManager = value; }
        }

        private TestExecutor _testExecutor;
        private TestExecutor TestExecutor
        {
            get { return _testExecutor; }
            set { _testExecutor = value; }
        }

        private ErrComparer _errComparer;
        private ErrComparer ErrComparer
        {
            get { return _errComparer; }
            set { _errComparer = value; }
        }

    }
}
