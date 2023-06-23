using System.Collections.Generic;
using System.Windows.Controls;

namespace Microsoft.Test.Controls.Actions
{
    public static class SelectionActions
    {
        #region public methods

        public static void ListBoxSelection(ListBox listbox)
        {
            List<ListBoxSelectionTest<ListBoxItem>> listBoxSelectionTests = new List<ListBoxSelectionTest<ListBoxItem>>();
            listBoxSelectionTests.Add(new ListBoxSelectionTest<ListBoxItem>(listbox, SelectionOption.AddItem));
            listBoxSelectionTests.Add(new ListBoxSelectionTest<ListBoxItem>(listbox, SelectionOption.InsertItem));
            listBoxSelectionTests.Add(new ListBoxSelectionTest<ListBoxItem>(listbox, SelectionOption.RemoveItem));
            listBoxSelectionTests.Add(new ListBoxSelectionTest<ListBoxItem>(listbox, SelectionOption.RemoveAtItem));
            listBoxSelectionTests.Add(new ListBoxSelectionTest<ListBoxItem>(listbox, SelectionOption.Refresh));

            listbox.SelectionMode = SelectionMode.Single;
            foreach (ISelectionTest<ListBoxItem> selectionTest in listBoxSelectionTests)
            {
                selectionTest.Run();
            }

            listbox.SelectionMode = SelectionMode.Extended;
            foreach (ISelectionTest<ListBoxItem> selectionTest in listBoxSelectionTests)
            {
                selectionTest.Run();
            }

            listbox.SelectionMode = SelectionMode.Multiple;
            foreach (ISelectionTest<ListBoxItem> selectionTest in listBoxSelectionTests)
            {
                selectionTest.Run();
            }
        }

        public static void ListViewSelection(ListView listview)
        {
            List<ListBoxSelectionTest<ListViewItem>> listBoxSelectionTests = new List<ListBoxSelectionTest<ListViewItem>>();
            listBoxSelectionTests.Add(new ListBoxSelectionTest<ListViewItem>(listview, SelectionOption.AddItem));
            listBoxSelectionTests.Add(new ListBoxSelectionTest<ListViewItem>(listview, SelectionOption.InsertItem));
            listBoxSelectionTests.Add(new ListBoxSelectionTest<ListViewItem>(listview, SelectionOption.RemoveItem));
            listBoxSelectionTests.Add(new ListBoxSelectionTest<ListViewItem>(listview, SelectionOption.RemoveAtItem));
            listBoxSelectionTests.Add(new ListBoxSelectionTest<ListViewItem>(listview, SelectionOption.Refresh));

            listview.SelectionMode = SelectionMode.Single;
            foreach (ISelectionTest<ListViewItem> selectionTest in listBoxSelectionTests)
            {
                selectionTest.Run();
            }

            listview.SelectionMode = SelectionMode.Extended;
            foreach (ISelectionTest<ListViewItem> selectionTest in listBoxSelectionTests)
            {
                selectionTest.Run();
            }

            listview.SelectionMode = SelectionMode.Multiple;
            foreach (ISelectionTest<ListViewItem> selectionTest in listBoxSelectionTests)
            {
                selectionTest.Run();
            }
        }

        public static void ComboBoxSelection(ComboBox combobox)
        {
            List<ComboBoxSelectionTest<ComboBoxItem>> comboBoxSelectionTests = new List<ComboBoxSelectionTest<ComboBoxItem>>();
            comboBoxSelectionTests.Add(new ComboBoxSelectionTest<ComboBoxItem>(combobox, SelectionOption.AddItem));
            comboBoxSelectionTests.Add(new ComboBoxSelectionTest<ComboBoxItem>(combobox, SelectionOption.InsertItem));
            comboBoxSelectionTests.Add(new ComboBoxSelectionTest<ComboBoxItem>(combobox, SelectionOption.RemoveItem));
            comboBoxSelectionTests.Add(new ComboBoxSelectionTest<ComboBoxItem>(combobox, SelectionOption.RemoveAtItem));
            comboBoxSelectionTests.Add(new ComboBoxSelectionTest<ComboBoxItem>(combobox, SelectionOption.Refresh));

            foreach (ISelectionTest<ComboBoxItem> selectionTest in comboBoxSelectionTests)
            {
                selectionTest.Run();
            }
        }

        public static void TreeViewSelection(TreeView treeview)
        {
            List<TreeViewSelectionTest<TreeViewItem>> treeViewSelectionTests = new List<TreeViewSelectionTest<TreeViewItem>>();
            treeViewSelectionTests.Add(new TreeViewSelectionTest<TreeViewItem>(treeview, SelectionOption.AddItem));
            treeViewSelectionTests.Add(new TreeViewSelectionTest<TreeViewItem>(treeview, SelectionOption.InsertItem));
            treeViewSelectionTests.Add(new TreeViewSelectionTest<TreeViewItem>(treeview, SelectionOption.RemoveItem));
            treeViewSelectionTests.Add(new TreeViewSelectionTest<TreeViewItem>(treeview, SelectionOption.RemoveAtItem));
            treeViewSelectionTests.Add(new TreeViewSelectionTest<TreeViewItem>(treeview, SelectionOption.Refresh));

            foreach (ISelectionTest<TreeViewItem> selectionTest in treeViewSelectionTests)
            {
                selectionTest.Run();
            }
        }

        public static void TreeViewItemSelection(TreeViewItem treeviewitem)
        {
            List<TreeViewSelectionTest<TreeViewItem>> treeViewItemSelectionTests = new List<TreeViewSelectionTest<TreeViewItem>>();
            treeViewItemSelectionTests.Add(new TreeViewSelectionTest<TreeViewItem>(treeviewitem, SelectionOption.AddItem));
            treeViewItemSelectionTests.Add(new TreeViewSelectionTest<TreeViewItem>(treeviewitem, SelectionOption.InsertItem));
            treeViewItemSelectionTests.Add(new TreeViewSelectionTest<TreeViewItem>(treeviewitem, SelectionOption.RemoveItem));
            treeViewItemSelectionTests.Add(new TreeViewSelectionTest<TreeViewItem>(treeviewitem, SelectionOption.RemoveAtItem));
            treeViewItemSelectionTests.Add(new TreeViewSelectionTest<TreeViewItem>(treeviewitem, SelectionOption.Refresh));

            foreach (ISelectionTest<TreeViewItem> selectionTest in treeViewItemSelectionTests)
            {
                selectionTest.Run();
            }

            // expanded treeviewitem selection test
            treeviewitem.IsExpanded = true;
            foreach (ISelectionTest<TreeViewItem> selectionTest in treeViewItemSelectionTests)
            {
                selectionTest.Run();
            }
        }

        #endregion
    }
}


