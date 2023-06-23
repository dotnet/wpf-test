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
    /// SliderExceptions
    /// </description>
    /// </summary>
    [Test(1, "Slider", "Exceptions")]
    public class SliderExceptions : StepsTest
    {
        #region Public Members

        public SliderExceptions()
        {
            RunSteps += new TestStep(TestExceptions);
        }

        public TestResult TestExceptions()
        {
            Slider Slider = new Slider();
            SliderActions.TestExceptions(Slider);

            return TestResult.Pass;
        }

        #endregion
    } 
}
