using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// ExpandSubtree test framework
    /// 
    /// Overview:
    /// It defines a test contract to help people to understand how to test ExpandSubtree feature in Run method.
    /// It forces the derive test types to implement ExpandSubtree method to provide concrete actions to expand subtree.
    /// It provides some basic validations that people can reuse or override.
    /// It implements the IDisposable pattern for cleanup, so please use "using" statement for the concrete test type.
    /// 
    /// Useage:
    /// using (ExpandSubtreeValidatorBase validator = new KeyboardExpandSubtreeValidator(topLevelContainer, "Item"))
    /// {
    ///     validator.Run();
    /// }
    /// </summary>
    public abstract class ExpandSubtreeValidatorBase : IDisposable
    {
        /// <summary>
        /// Initial setup to prepare testing.
        /// Get treeviewitem count from XmlNode count.
        /// </summary>
        /// <param name="treeview">A TreeView reference</param>
        /// <param name="treeviewitemName">A xpath name for getting treeviewitem count from XmlElement</param>
        public ExpandSubtreeValidatorBase(TreeViewItem treeviewitem, string xpathName)
        {
            if (treeviewitem == null)
            {
                throw new ArgumentNullException("Fail: TreeView is null.");
            }

            if (String.IsNullOrEmpty(xpathName))
            {
                throw new ArgumentNullException("Fail: the TreeViewItem name is empty.");
            }

            this.treeviewitem = treeviewitem;

            expectedExpandedItemCount = TreeViewItemHelper.GetTreeViewItemCount(treeviewitem, xpathName);
        }

        protected TreeViewItem treeviewitem;
        private bool isDisposed = false;
        private int expectedExpandedItemCount;

        #region Protected members

        /// <summary>
        /// Defer implementation to concrete types because more than one way to expand subtree
        /// Press * (multiply) key to expand or all TreeViewItem.ExpandSubtree().
        /// </summary>
        protected abstract void ExpandSubtree();

        /// <summary>
        /// Make it protected virtual because maybe people want to override it.
        /// Walk through all treeviewitems from top to bottom and make sure each treeviewitem is expanded and 
        /// the count of expanded items equals to count of treeviewitem xml data nodes.
        /// </summary>
        protected virtual void ValidateExpandedSubtree()
        {
            TreeViewHelper.ScrollTreeViewToTop(treeviewitem);

            if (!ViewportHelper.IsInViewport(treeviewitem))
            {
                throw new TestValidationException("Fail: the TreeViewItem is not viewport.");
            }

            int actualExpandedItemCount = 0;
            if (!TreeViewItemHelper.IsSubtreeExpanded(treeviewitem, ref actualExpandedItemCount))
            {
                throw new TestValidationException("Fail: subtree is not expanded.");
            }

            if (VirtualizingStackPanel.GetVirtualizationMode(treeviewitem) != VirtualizationMode.Recycling)
            {
                if (actualExpandedItemCount != expectedExpandedItemCount)
                {
                    throw new TestValidationException("Fail: the actual expanded TreeViewItem count " + actualExpandedItemCount + " does not equal to the expected TreeViewItem count " + expectedExpandedItemCount + ".");
                }
            }
        }

        #endregion

        public void Run()
        {
            ExpandSubtree();
            // We need to use SystemIdle here to allow more time to wait for rendering to be ready.
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            ValidateExpandedSubtree();
        }

        #region IDisposable Members

        protected virtual void Dispose(bool disposing)
        {
            // When finalizer calls Microsoft.Test.Input.WpfMouse.Reset() will cause exception below.
            // "The calling thread must be STA, because many UI components require this."
            // So, remove finalizer.
            if (!isDisposed)
            {
                Microsoft.Test.Input.Mouse.Reset();
                DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);
                Microsoft.Test.Input.Keyboard.Reset();
                DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

                isDisposed = true;
            }
        }

        public void Cleanup()
        {
            Dispose(true);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
