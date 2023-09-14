using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Avalon.Test.ComponentModel;

namespace Microsoft.Test.Controls
{
    class ListBoxSelectionTest<T> : ISelectionTest<T> where T : ListBoxItem, new()
    {
        ItemsControl itemsControl;
        SelectionOption selectionOption;
        IEnumerable<T> currentSelectedItemsQuery;
        public ListBoxSelectionTest(ItemsControl itemsControl, SelectionOption selectionOption)
        {
            this.itemsControl = itemsControl;
            this.selectionOption = selectionOption;
            currentSelectedItemsQuery =
                            from T item in itemsControl.Items
                            where item.IsSelected == true
                            select item;
        }
        public void Run()
        {
            for (int i = 1; i <= 5; i++)
            {
                T item = new T();
                item.Content = "Item" + i.ToString();
                if (i % 2 == 0)
                {
                    item.IsSelected = true;
                }
                itemsControl.Items.Add(item);
            }

            List<T> beforeModifySelectedItems = new List<T>(currentSelectedItemsQuery);
            T lastSelectedItem = currentSelectedItemsQuery.Last();
            if (lastSelectedItem == null)
            {
                throw new TestValidationException("The last selected item is null, so no selected item.");
            }

            T newSelectedItem = new T();
            newSelectedItem.Content = "New Item";
            newSelectedItem.IsSelected = true;

            switch (selectionOption)
            {
                case SelectionOption.AddItem:
                    itemsControl.Items.Add(newSelectedItem);
                    ValidateAddedSelectedItem(itemsControl, lastSelectedItem, newSelectedItem);
                    break;
                case SelectionOption.InsertItem:
                    int selectedItemIndex = itemsControl.Items.Count / 2;
                    itemsControl.Items.Insert(selectedItemIndex, newSelectedItem);
                    ValidateAddedSelectedItem(itemsControl, lastSelectedItem, newSelectedItem);
                    break;
                case SelectionOption.RemoveItem:
                    itemsControl.Items.Remove(lastSelectedItem);
                    ValidateRemovedSelectedItem(itemsControl, beforeModifySelectedItems);
                    break;
                case SelectionOption.RemoveAtItem:
                    int removeItemIndex = itemsControl.Items.IndexOf(lastSelectedItem);
                    itemsControl.Items.RemoveAt(removeItemIndex);
                    ValidateRemovedSelectedItem(itemsControl, beforeModifySelectedItems);
                    break;
                case SelectionOption.Refresh:
                    itemsControl.Items.Refresh();
                    QueueHelper.WaitTillQueueItemsProcessed();
                    if (Selector.GetIsSelectionActive(lastSelectedItem))
                    {
                        throw new TestValidationException("The last selected item IsSelectionActive should be false after Refresh.");
                    }
                    break;
            }

            // clean up.
            itemsControl.Items.Clear();
        }

        private void ValidateRemovedSelectedItem(ItemsControl itemsControl, List<T> beforeModifySelectedItems)
        {
            QueueHelper.WaitTillQueueItemsProcessed();

            switch (((ListBox)itemsControl).SelectionMode)
            {
                case SelectionMode.Single:
                    if (!currentSelectedItemsQuery.Count().Equals(0))
                    {
                        throw new TestValidationException(((ListBox)itemsControl).SelectionMode.ToString() + " SelectionMode: there is still selected item after remove the selected item.");
                    }
                    break;
                case SelectionMode.Extended:
                case SelectionMode.Multiple:
                    if (!currentSelectedItemsQuery.Count().Equals(beforeModifySelectedItems.Count - 1))
                    {
                        throw new TestValidationException(((ListBox)itemsControl).SelectionMode.ToString() + " SelectionMode: after removed selected item, we have same number of selected item count as before removed selected item.");
                    }
                    break;
            }
        }

        private void ValidateAddedSelectedItem(ItemsControl itemsControl, T lastSelectedItem, T newSelectedItem)
        {
            QueueHelper.WaitTillQueueItemsProcessed();

            if (!newSelectedItem.IsSelected)
            {
                throw new TestValidationException("new selected item is not selected.");
            }

            switch (((ListBox)itemsControl).SelectionMode)
            {
                case SelectionMode.Single:
                    if (lastSelectedItem.IsSelected)
                    {
                        throw new TestValidationException(((ListBox)itemsControl).SelectionMode.ToString() + " SelectionMode: the old selected item is selected after added a new selected item.");
                    }
                    break;
                case SelectionMode.Extended:
                case SelectionMode.Multiple:
                    if (!lastSelectedItem.IsSelected)
                    {
                        throw new TestValidationException(((ListBox)itemsControl).SelectionMode.ToString() + " SelectionMode: the old selected item is not selected after added a new selected item.");
                    }
                    break;
            }
        }
    }
}
