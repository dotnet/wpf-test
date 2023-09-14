using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// TreeViewRegressionTest73
    /// </description>
    /// </summary>
    [Test(1, "TreeView", "TreeViewRegressionTest73")]
    public class TreeViewRegressionTest73 : XamlTest
    {
        #region Private Members

        private TreeViewItem item1;

        #endregion

        #region Public Members

        public TreeViewRegressionTest73()
            : base(@"TreeViewRegressionTest73.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(Repro);
        }

        public TestResult Setup()
        {
            Status("Setup");

            item1 = (TreeViewItem)RootElement.FindName("item1");

            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            item1 = null;            
            return TestResult.Pass;
        }

        /// <summary>
        /// Ensure no exception occur after pressed Multiple key twice on a selected treeviewitem has lots of items
        /// </summary>
        public TestResult Repro()
        {
            ContentPresenter header = item1.Template.FindName("PART_Header", item1) as ContentPresenter;
            InputHelper.MouseClickCenter(header, System.Windows.Input.MouseButton.Left);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            Microsoft.Test.Input.Keyboard.Type(Microsoft.Test.Input.Key.Multiply);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);
            Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.Multiply);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            return TestResult.Pass;
        }

        #endregion
    } 
}
