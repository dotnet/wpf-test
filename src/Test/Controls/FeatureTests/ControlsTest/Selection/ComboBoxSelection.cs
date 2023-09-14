using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Controls.Actions;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// ComboBoxSelectionTest
    /// </description>
    /// </summary>
    [Test(0, "Selection", "ComboBoxSelectionTest", Keywords = "Localization_Suite")]
    public class ComboBoxSelectionTest : XamlTest
    {
        #region Private Members
        ComboBox combobox;
        #endregion

        #region Public Members

        public ComboBoxSelectionTest()
            : base(@"ComboBoxSelectionTest.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestComboBoxSelection);
        }

        public TestResult Setup()
        {
            Status("Setup");

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            combobox = (ComboBox)RootElement.FindName("combobox");
            if (combobox == null)
            {
                throw new TestValidationException("ComboBox is null");
            }

            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            combobox = null;
            return TestResult.Pass;
        }

        public TestResult TestComboBoxSelection()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            SelectionActions.ComboBoxSelection(combobox);

            return TestResult.Pass;
        }

        #endregion
    } 
}
