using System.Windows.Controls;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Programmatically expand subtree
    /// </summary>
    public class ProgrammaticallyExpandSubtreeValidator : ExpandSubtreeValidatorBase
    {
        public ProgrammaticallyExpandSubtreeValidator(TreeViewItem treeviewitem, string xpathName)
            : base(treeviewitem, xpathName)
        {
        }

        protected override void ExpandSubtree()
        {
            treeviewitem.ExpandSubtree();
        }
    }
}
