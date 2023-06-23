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
    /// TreeViewItemSelectionTest
    /// </description>
    /// </summary>
    [Test(0, "Selection", "TreeViewItemSelectionTest", Keywords = "Localization_Suite")]
    public class TreeViewItemSelectionTest : XamlTest
    {
        #region Private Members
        TreeViewItem treeviewitem;
        #endregion

        #region Public Members

        public TreeViewItemSelectionTest()
            : base(@"TreeViewItemSelectionTest.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestTreeViewItemSelection);
        }

        public TestResult Setup()
        {
            Status("Setup");

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            treeviewitem = (TreeViewItem)RootElement.FindName("treeviewitem");
            if (treeviewitem == null)
            {
                throw new TestValidationException("TreeViewItem is null");
            }

            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            treeviewitem = null;
            return TestResult.Pass;
        }

        public TestResult TestTreeViewItemSelection()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            SelectionActions.TreeViewItemSelection(treeviewitem);

            return TestResult.Pass;
        }

        #endregion
    } 
}
