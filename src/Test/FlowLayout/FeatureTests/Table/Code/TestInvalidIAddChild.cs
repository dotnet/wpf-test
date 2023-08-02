// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Regression test for Regression_Bug54
// 1.  Create a Table
// 2.  Use IAddChild to add a non null, non Table object to the table
// 
// Expected: We should get a System.ArgumentException
// Created by: Microsoft

using System;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Markup;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using System.Windows.Documents;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Testing Invalid IAddChild.    
    /// </summary>
    [Test(3, "Table", "InvalidIAddChild")]
    public class InvalidIAddChild : AvalonTest
    {     
        #region Constructor
     
        public InvalidIAddChild()
            : base()
        {
            RunSteps += new TestStep(RunTests);
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// RunTests: Runs a set of tests based on the input variation.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTests()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);
            
            string expectedExceptionString = "System.ArgumentException";

            Table table = new Table();
            DependencyObject depObj = new DependencyObject();
            try
            {
                ((IAddChild)table).AddChild(depObj);
            }
            catch (Exception e)
            {
                if (e.GetType().ToString() == expectedExceptionString)
                {
                    LogComment("Got the expected exception.  Test has passed!!");
                    return TestResult.Pass;
                }
                else
                {
                    LogComment("We got an exception, but it was not the expected one!");
                    LogComment("Expected: " + expectedExceptionString + ", Actual: " + e.GetType().ToString());
                    LogComment("Test has failed!!");
                    return TestResult.Fail;
                }               
            }

            LogComment("Never got an exception.  Test has failed!!");
            
            return TestResult.Fail;
        }
        #endregion
    }
}
