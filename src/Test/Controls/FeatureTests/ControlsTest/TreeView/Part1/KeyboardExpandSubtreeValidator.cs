using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Use keyboard to expand subtree.
    /// </summary>
    public class KeyboardExpandSubtreeValidator : ExpandSubtreeValidatorBase
    {
        public KeyboardExpandSubtreeValidator(TreeViewItem treeviewitem, string xpathName)
            : base(treeviewitem, xpathName)
        {
        }

        protected override void ExpandSubtree()
        {
            Microsoft.Test.Input.Keyboard.Type(Microsoft.Test.Input.Key.Multiply);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);
        }
    }
}
