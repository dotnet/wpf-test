using System.Windows.Controls;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// ListBoxHelper
    /// </summary>
    public static class ListBoxHelper
    {
        /// <summary>
        /// GEt listbox from item container
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public static ListBox GetListBox(Control container)
        {
            return ItemsControl.ItemsControlFromItemContainer(container) as ListBox;
        }
    }
}


