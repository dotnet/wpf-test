// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Test.Uis.TestTypes
{
    #region Namespaces.

    using System;
    using System.Windows;
    using System.Collections;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.TestTypes;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// Manage to run a collection of combined test cases.
    /// example of using this class: GroupedCustomTestCase
    /// </summary>
    public abstract class CombinedTestCaseManager : CustomTestCase
    {
        /// <summary>
        /// Start to loop of regression test.
        /// </summary>
        public override void RunTestCase()
        {
            _testCases = GetTestCases();

            Verifier.Verify(_testCases!= null && _testCases.Length > 0, "No Test cases found!");
    
            //This flag should be set after checking the test cases.
            Verifier.WillThrow = false;

            StartAIndividualTest();
        }

        /// <summary>
        /// Specify the array of test cases.
        /// </summary>
        public abstract CombinedTestCase[] GetTestCases();

        /// <summary>
        /// The test loop start here.
        /// </summary>
        void StartAIndividualTest()
        {
            KeyboardInput.ResetCapsLock();
            Log("******************************* " + _testCases[_TestCaseCounter].GetType().Name + " **************************************");
            _testCases[_TestCaseCounter].StartUp(MainWindow, EndTest);
        }

        /// <summary>
        /// the test end here. After each case, we will do a evaluation.
        /// Clean up will be done for next combined test.
        /// </summary>
        void EndTest()
        {
            if (Verifier.Failed)
            {
                _failedCases += _testCases[_TestCaseCounter].GetType().Name + ", ";
            }
            else
            {
                Log(_testCases[_TestCaseCounter].GetType().Name + " passed!");
            }
            _TestCaseCounter++;
            
            if (_TestCaseCounter < _testCases.Length)
            {
                //we must reset the failed flag for each case.
                Verifier.Failed = false;

                //release the content of the last case.
                MainWindow.Content = null; 

                //Start a new test case.
                StartAIndividualTest();
            }
            else
            {
                if (_failedCases.Length > 13)
                {
                    //Log out the failed cases.
                    Log(_failedCases);

                    //failed the test.
                    Logger.Current.ReportResult(false, "There are some case(s) failed!");   
                }
                else
                {
                    //Test passed with no fails.
                    Logger.Current.ReportSuccess();
                }
            }
        }

        /// <summary>Fininshed tests counter</summary>
        int _TestCaseCounter=0; 

        /// <summary>string that hold the failed test case</summary>
        string _failedCases="Failed cases:";

        /// <summary>Array of test cases</summary>
        CombinedTestCase[] _testCases; 
    }
    
    /// <summary>
    /// Base class for combinedTestCase.
    /// example of using this class: KeyboardRegressionTest, RegressionTest_Regression_Bug196 
    /// </summary>
    public abstract class CombinedTestCase : CustomTestCase
    {
        SimpleHandler _returnHandler = null;
        /// <summary>
        /// This method is called by a test case manager. 
        /// Caller will have the window and return handler ready to pass in. 
        /// the local case will not create a window and application.
        /// the return handler must be passed in so that the local case can exist. We will enforce this. 
        /// Note: When run the stand alone case, this method is not called and _returnHandler is null.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="handler"></param>
        public void StartUp(Window window, SimpleHandler handler)
        {
            if (window == null)
            {
                throw new ArgumentNullException("window", "Window Passed in can not be null!");
            }
            if (handler == null)
            {
                throw new ArgumentNullException("handler", "return handler Passed in can not be null!");
            }

            this.MainWindow = window;
            _returnHandler = handler;
            
            //Start to run the case
            //Note: RunTestCase is defined in the base class.
            QueueDelegate(RunTestCase);
        }

        /// <summary>
        /// End this test
        /// Assume:
        ///     1. _returnHandler == null, when this cases is running stand alone.
        ///     2. _returnHandler != null, when this cases is running by a case manager combined with other cases.
        /// </summary>
        public void EndTest()
        {
            if (_returnHandler != null)
            {
                QueueDelegate(_returnHandler);
            }
            else
            {
                Logger.Current.ReportSuccess();
            }
        }
    }
}
