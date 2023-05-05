// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
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
    /// Regression Test : Verify that no ArgumentExcpetions occur when binding is set to have inadmissable fallbackvalue    
    /// </description>
    /// </summary>            
    [Test(1, "Binding", "RegressionFallBackBindingExpressionException")]
    public class RegressionFallBackBindingExpressionException : AvalonTest
    {
        #region Public Members

        public RegressionFallBackBindingExpressionException()
        {            
                InitializeSteps += new TestStep(RunTest);            
        }

        public TestResult RunTest()
        {
            Status("RunTest");           

            Rectangle rectangle = new Rectangle();
            Binding binding = new Binding();
            binding.Path = new PropertyPath(FrameworkElement.CursorProperty);
            binding.FallbackValue = "Test";            

            try
            {
                rectangle.SetBinding(FrameworkElement.CursorProperty, binding);                
            }
            catch (ArgumentException e)
            {
                LogComment("Exception was not expected: " + e.Message);
                return TestResult.Fail;
            }           

            LogComment("Test ran successfully");
            return TestResult.Pass;
        }

        #endregion
    }
}
