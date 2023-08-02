// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Regression test for 






using System;
using System.IO;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Markup;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>    
    /// Testing commands in FlowDocumentPageViewer.   
    /// </summary>
    [Test(3, "Table", "InvalidCell")]
    public class InvalidCell : AvalonTest
    {       
        private string _inputXaml;
      
        [Variation("InvalidCell.xaml")]       
        public InvalidCell(string testValue)
            : base()
        {
            _inputXaml = testValue;
            RunSteps += new TestStep(RunTests);
        }
        
        /// <summary>
        /// RunTests: Runs a set of tests based on the input variation.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTests()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);
            System.IO.Stream xamlFileStream = File.OpenRead(_inputXaml);
            string expectedExceptionString = "System.Windows.Markup.XamlParseException";

            UIElement root = null;

            try
            {
                root = (UIElement)XamlReader.Load(xamlFileStream);
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
    }
}
