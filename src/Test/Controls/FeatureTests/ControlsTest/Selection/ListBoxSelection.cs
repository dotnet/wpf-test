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
    /// ListBoxSelectionTest
    /// </description>
    /// </summary>
    [Test(0, "Selection", "ListBoxSelectionTest", Versions = "3.0+")]
    public class ListBoxSelectionTest : XamlTest
    {
        #region Private Members
        ListBox listbox;
        ListView listview;
        #endregion

        #region Public Members

        public ListBoxSelectionTest()
            : base(@"ListBoxSelectionTest.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestListBoxSelection);
            RunSteps += new TestStep(TestListViewSelection);
        }

        public TestResult Setup()
        {
            Status("Setup");

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            listbox = (ListBox)RootElement.FindName("listbox");
            if (listbox == null)
            {
                throw new TestValidationException("ListBox is null");
            }

            listview = (ListView)RootElement.FindName("listview");
            if (listview == null)
            {
                throw new TestValidationException("ListView is null");
            }

            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            listbox = null;
            listview = null;
            return TestResult.Pass;
        }

        public TestResult TestListBoxSelection()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            SelectionActions.ListBoxSelection(listbox);

            return TestResult.Pass;
        }

        public TestResult TestListViewSelection()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            SelectionActions.ListViewSelection(listview);

            return TestResult.Pass;
        }

        #endregion
    }
}
