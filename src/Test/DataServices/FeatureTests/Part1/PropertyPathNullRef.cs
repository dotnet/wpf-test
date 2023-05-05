// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.ComponentModel;
using System.Globalization;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where PropertyPath constructor throws a null ref when pathParameters is null
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "PropertyPathNullRef", SecurityLevel = TestCaseSecurityLevel.PartialTrust)]
    public class PropertyPathNullRef : AvalonTest
    {

        #region Constructors

        public PropertyPathNullRef()            
        {
            InitializeSteps += new TestStep(Validate);                        
        }

        #endregion

        #region Private Members

        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Render);         
            
            // Create a propertypath with null arguments       
            try
            {
                new PropertyPath(null, null);
            }
            catch (NullReferenceException)
            {
                LogComment("Unexpected NullReferenceException.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes
    
    #endregion
}