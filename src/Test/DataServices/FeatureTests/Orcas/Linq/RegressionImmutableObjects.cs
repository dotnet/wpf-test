// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Reflection;
using System.Windows.Threading;
using System.Collections.Generic;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Verifies that objects return as a result of Linq Projections are infact immutable.
    /// </description>
    /// <relatedBugs>
    /// </relatedBugs>
    /// </summary>            
    [Test(1, "Linq.Queries", "RegressionImmutableObjects")]
    public class RegressionImmutableObjects : AvalonTest
    {        
        #region Public Members

        public RegressionImmutableObjects()
        {
            InitializeSteps += new TestStep(RunTest);            
        }

        public TestResult RunTest()
        {
            Status("SetUp");

            string[] words = { "aPPLE", "BlUeBeRrY", "cHeRry" };

            // Perform Projection query on DataSource - need to get multiple selections to verify immutability.
            var wordsSelection =
                from w in words
                select new { Upper = w.ToUpper(), Lower = w.ToLower() };

            // Walk through the reults and verify.
            foreach (var ul in wordsSelection)
            {                
                Type objectType = ul.GetType();
                PropertyInfo propertyInfo = objectType.GetProperty("Lower");

                // Verify that the Property is not writeable.
                if (propertyInfo.CanWrite)
                {
                    LogComment("Object returned as a result of the project is not immutable.");
                    return TestResult.Fail;
                }
            }

            LogComment("Test ran successfully");
            return TestResult.Pass;
        }       

        #endregion        
    }    
}
