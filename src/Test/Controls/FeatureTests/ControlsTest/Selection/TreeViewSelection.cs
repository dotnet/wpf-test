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
    /// TreeViewSelectionTest
    /// </description>
    /// </summary>
    [Test(0, "Selection", "TreeViewSelectionTest")]
    public class TreeViewSelectionTest : XamlTest
    {
        #region Private Members
        TreeView treeview;
        #endregion

        #region Public Members

        public TreeViewSelectionTest()
            : base(@"TreeViewSelectionTest.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestTreeViewSelection);
        }

        public TestResult Setup()
        {
            Status("Setup");

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            treeview = (TreeView)RootElement.FindName("treeview");
            if (treeview == null)
            {
                throw new TestValidationException("TreeView is null");
            }

            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            treeview = null;
            return TestResult.Pass;
        }

        public TestResult TestTreeViewSelection()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            SelectionActions.TreeViewSelection(treeview);

            return TestResult.Pass;
        }

        #endregion
    } 
}
