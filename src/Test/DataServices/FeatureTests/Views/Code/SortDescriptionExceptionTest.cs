// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System;
using Microsoft.Test;
using Microsoft.Test.Diagnostics;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Test the exceptions in SortDescription
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(3, "Views", "SortDescriptionExceptionTest")]
    public class SortDescriptionExceptionTest : AvalonTest
    {
        public SortDescriptionExceptionTest()
        {
            RunSteps += new TestStep(nullTest);
            RunSteps += new TestStep(EmptyString);
            RunSteps += new TestStep(BadDirection);
        }

        TestResult nullTest()
        {
            TestResult result = TestResult.Pass;

            if (SystemInformation.WpfVersion == WpfVersions.Wpf30)
            {
                SetExpectedErrorTypeInStep(typeof(ArgumentNullException));
                SortDescription desc = new SortDescription(null, ListSortDirection.Ascending);

                LogComment("Expected ArgumentNullException");
                result = TestResult.Fail;
            }
            else
            {
                try
                {
                    SortDescription desc = new SortDescription(null, ListSortDirection.Ascending);
                }
                catch (Exception e)
                {
                    LogComment("Expected No Exception. Got: " + e.Message);
                    result = TestResult.Fail;
                }                
            }

            return result;
        }

        TestResult EmptyString()
        {
            TestResult result = TestResult.Pass;

            if (SystemInformation.WpfVersion == WpfVersions.Wpf30)
            {
                SetExpectedErrorTypeInStep(typeof(ArgumentException));
                SortDescription desc = new SortDescription("", ListSortDirection.Ascending);

                LogComment("Expected ArgumentException");
                result = TestResult.Fail;
            }
            else
            {
                try
                {
                    SortDescription desc = new SortDescription("", ListSortDirection.Ascending);
                }
                catch (Exception e)
                {
                    LogComment("Expected No Exception. Got: " + e.Message);
                    result = TestResult.Fail;
                }
            }

            return result;
        }

        TestResult BadDirection()
        {
            SetExpectedErrorTypeInStep(typeof(ArgumentException));
            SortDescription desc = new SortDescription("Hello", (ListSortDirection)2);

            LogComment("Expected ArgumentException");
            return TestResult.Fail;
        }
    }
}