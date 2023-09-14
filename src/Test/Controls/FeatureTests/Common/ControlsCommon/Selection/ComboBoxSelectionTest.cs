using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Avalon.Test.ComponentModel;

namespace Microsoft.Test.Controls
{
    class ComboBoxSelectionTest<T> : ISelectionTest<T> where T : ComboBoxItem, new()
    {
        ItemsControl itemsControl;
        SelectionOption selectionOption;
        public ComboBoxSelectionTest(ItemsControl itemsControl, SelectionOption selectionOption)
        {
            this.itemsControl = itemsControl;
            this.selectionOption = selectionOption;
        }
        public void Run()
        {
            for (int i = 1; i <= 3; i++)
            {
                T item = new T();
                item.Content = "Item" + i.ToString();
                if (i == 2)
                {
                    item.IsSelected = true;
                }
                itemsControl.Items.Add(item);
            }

            T selectedItem = (T)itemsControl.Items[1];
            if (!selectedItem.IsSelected)
            {
                throw new TestValidationException("selected item is not selected.");
            }

            T newSelectedItem = new T();
            newSelectedItem.Content = "New Item";
            newSelectedItem.IsSelected = true;

            switch (selectionOption)
            {
                case SelectionOption.AddItem:
                    itemsControl.Items.Add(newSelectedItem);
                    ValidateAddedSelectedItem(selectedItem, newSelectedItem);
                    break;
                case SelectionOption.InsertItem:
                    int selectedItemIndex = itemsControl.Items.Count / 2;
                    itemsControl.Items.Insert(selectedItemIndex, newSelectedItem);
                    ValidateAddedSelectedItem(selectedItem, newSelectedItem);
                    break;
                case SelectionOption.RemoveItem:
                    itemsControl.Items.Remove(selectedItem);
                    ValidateRemovedSelectedItem(itemsControl);
                    break;
                case SelectionOption.RemoveAtItem:
                    int removeItemIndex = itemsControl.Items.IndexOf(selectedItem);
                    itemsControl.Items.RemoveAt(removeItemIndex);
                    ValidateRemovedSelectedItem(itemsControl);
                    break;
                case SelectionOption.Refresh:
                    itemsControl.Items.Refresh();
                    QueueHelper.WaitTillQueueItemsProcessed();
                    if (Selector.GetIsSelectionActive(selectedItem))
                    {
                        throw new TestValidationException("The selected item IsSelectionActive should be false after Refresh.");
                    }
                    break;
            }

            // clean up.
            itemsControl.Items.Clear();
        }

        private static void ValidateRemovedSelectedItem(ItemsControl itemsControl)
        {
            QueueHelper.WaitTillQueueItemsProcessed();

            foreach (T item in itemsControl.Items)
            {
                if (Selector.GetIsSelected(item))
                {
                    throw new TestValidationException("There is still selected item after remove the selected item.");
                }
            }
        }

        private static void ValidateAddedSelectedItem(T selectedItem, T newSelectedItem)
        {
            QueueHelper.WaitTillQueueItemsProcessed();

            if (!newSelectedItem.IsSelected)
            {
                throw new TestValidationException("new selected item is not selected.");
            }

            if (selectedItem.IsSelected)
            {
                throw new TestValidationException("The old selected item is selected after added a new selected item.");
            }
        }
    }
}
