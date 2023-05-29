// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;

namespace Microsoft.Test.ElementServices.Resources.RegressionTests
{
    /// <summary>
    /// Regression tests for Resources-related bugs (non-Xaml tests).
    /// </summary>
    [Test(1, "Resources.RegressionTests", "ResourceBugs")]

    public class ResourceBugs : WindowTest
    {
        #region Private Data

        private string _inputString = "";

        #endregion


        #region Constructors

        [Variation("ResourceDictionaryCopyTo")]
        [Variation("AddNullToMergedDictionaries")]
        [Variation("ClearMergedDictionaries")]

        /******************************************************************************
        * Function:          ResourceBugs Constructor
        ******************************************************************************/
        public ResourceBugs(string testValue)
        {
            _inputString = testValue;
            RunSteps += new TestStep(StartTest);
        }
        #endregion

        #region Test Steps

        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// StartTest:  begin the test case, depending on the variation passed in.
        /// </summary>
        /// <returns>TestResult</returns>
        TestResult StartTest()
        {
            bool testPassed = false;
            ResourceDictionary rd;

            GlobalLog.LogStatus("---Test Case Variation: " + _inputString);

            switch (_inputString)
            {
                // Regression test for 

                case "ResourceDictionaryCopyTo":
                    GlobalLog.LogStatus("Bug Verification: ResourceDictionary.CopyTo is not implemented correctly.");
                    rd = new ResourceDictionary();
                    rd.BeginInit();
                    rd.Add("1", System.Windows.Media.Brushes.RosyBrown);
                    rd.Add("2", System.Windows.Media.Brushes.Snow);
                    rd.EndInit();
                    DictionaryEntry[] array1 = new DictionaryEntry[5];

                    rd.CopyTo(array1, 2);
                    if (array1[0].Value == null)
                        GlobalLog.LogStatus("After CopyTo, first element is null");
                    if (array1[1].Value == null)
                        GlobalLog.LogStatus("Second element is null");
                    if (array1[2].Value != null)
                        GlobalLog.LogStatus("Third element is not null, will check further");
                    System.Collections.DictionaryEntry entry = (System.Collections.DictionaryEntry)array1[2];
                    if ((string)entry.Key == "1")
                        GlobalLog.LogStatus("Key is 1");
                    if (entry.Value == (object)System.Windows.Media.Brushes.RosyBrown)
                        GlobalLog.LogStatus("value is verified");

                    GlobalLog.LogStatus("Now check the next element");
                    entry = (System.Collections.DictionaryEntry)array1[3];
                    if ((string)entry.Key == "2")
                        GlobalLog.LogStatus("Key is 2");
                    if (entry.Value == (object)System.Windows.Media.Brushes.Snow)
                        GlobalLog.LogStatus("value is verified");
                    if (array1[4].Value == null)
                        GlobalLog.LogStatus("Fifth element is null");

                    GlobalLog.LogStatus("Make Sure That ResourceDictionary Remains the same");
                    if (rd.Contains("1"))
                        GlobalLog.LogStatus("Original ResourceDictionary still contains '1'");
                    if (rd["2"] == (Object)System.Windows.Media.Brushes.Snow)
                        GlobalLog.LogStatus("Original ResourceDictionary remain intact");

                    //Negative Case when destArray is not long enough. It should throw ArgumentException.
                    ExceptionHelper.ExpectException<ArgumentException>
                    (
                        delegate() { rd.CopyTo(array1, 4); },
                        delegate(ArgumentException e) { ;}
                    );
                    testPassed = true;
                    break;

                // Regression test for 


                case "AddNullToMergedDictionaries":
                    rd = new ResourceDictionary();
                    rd.Add("one", Brushes.Green);            
                    GlobalLog.LogStatus("Executing MergedDictionaries.Add(null) throws an ArgumentNullException.");
                    ExceptionHelper.ExpectException<ArgumentNullException>
                    (
                        delegate() { rd.MergedDictionaries.Add(null); },
                        delegate(ArgumentNullException e) { ;}
                    );
                    testPassed = true;
                    break;

                // Regression test for 

                case "ClearMergedDictionaries":
                    StackPanel stackPanel = new StackPanel();
                    ResourceDictionary a = new ResourceDictionary();
                    ResourceDictionary b = stackPanel.Resources;

                    b.MergedDictionaries.Add(a);
                    b.MergedDictionaries.Clear();
                    b.MergedDictionaries.Add(a);

                    GlobalLog.LogEvidence("Expected MergedDictionaries Count: 1");
                    GlobalLog.LogEvidence("Actual MergedDictionaries Count:   " + b.MergedDictionaries.Count);

                    if (b.MergedDictionaries.Count == 1)
                    {
                        testPassed = true;
                    }
                    break;

                default:
                    GlobalLog.LogEvidence("ERROR!!! StartTest: Unexpected failure to match argument.");
                    testPassed = false;
                    break;
            }
            
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

