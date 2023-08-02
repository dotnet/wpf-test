using System;
using System.Windows.Controls;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Avalon.Test.ComponentModel.Actions;

namespace Avalon.Test.ComponentModel.UnitTests
{
    /// <summary>
    /// <description>
    /// ToolTipServiceExceptions
    /// </description>
    /// </summary>
    [Test(1, "ToolTipService", "TestToolTipServiceExceptions")]
    public class ToolTipServiceExceptions : XamlTest
    {
        #region Private Members
        Button defaultButton;
        #endregion

        #region Public Members

        public ToolTipServiceExceptions()
            : base(@"ToolTipServiceBehavior.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestExceptions);
        }

        public TestResult Setup()
        {
            Status("Setup");

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            defaultButton = (Button)RootElement.FindName("defaultButton");
            if (defaultButton == null)
            {
                throw new TestValidationException("Default button is null");
            }

            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            defaultButton = null;            
            return TestResult.Pass;
        }

        public TestResult TestExceptions()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            
            ToolTipServiceActions.TestExceptions(defaultButton);

            return TestResult.Pass;
        }

        #endregion
    } 
}
