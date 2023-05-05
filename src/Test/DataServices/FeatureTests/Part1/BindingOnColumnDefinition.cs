// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Windows.Data;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where WPF only:Creating a binding on a columnDefinition produces compilation errors
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "BindingOnColumnDefinition")]
    public class BindingOnColumnDefinition : AvalonTest
    {
        #region Constructors

        public BindingOnColumnDefinition()
            : base()
        {
            InitializeSteps += new TestStep(Validate);       
        }

        #endregion

        #region Private Members

        private TestResult Validate()
        {
            Grid myGrid = new Grid();
            myGrid.Name = "myGrid";

            ColumnDefinition columnDefinition = new ColumnDefinition();
            Binding binding = new Binding("IsEnabled");
            binding.ElementName = "myGrid";

            // Setting this binding should not throw an exception.
            BindingOperations.SetBinding(columnDefinition, ColumnDefinition.IsEnabledProperty, binding);

            // If we make it to this point the bug has not regressed.
            return TestResult.Pass;
        }
               
        #endregion
        
    }
}
