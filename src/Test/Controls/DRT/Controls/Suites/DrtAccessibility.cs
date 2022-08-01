// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;

using System.Windows;
using System.Windows.Automation;
#if OLD_AUTOMATION
using System.Windows.Automation.Provider;
#endif
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DRT
{
    public class DrtAccessibilitySuite : DrtTestSuite
    {
        public DrtAccessibilitySuite() : base("Accessibility")
        {
            Contact = "Microsoft";
        }

        private ListBox _listbox;
        private ListBoxItem _listitem1;
        private ListBoxItem _listitem2;
        private ListBoxItem _listitem3;
        private Menu _menu;
        private MenuItem _subsubsubmi;

        public override DrtTest[] PrepareTests()
        {
            // Build tree
            StackPanel root = new StackPanel();     // Returned root object
            root.Orientation = System.Windows.Controls.Orientation.Horizontal;
            root.Background = Brushes.Pink;

            _listbox = new ListBox();
            _listitem1 = new ListBoxItem();
            _listitem2 = new ListBoxItem();
            _listitem3 = new ListBoxItem();
            _listitem1.Content = "ListBoxItem 1";
            _listitem2.Content = "ListBoxItem 2";
            _listitem3.Content = "ListBoxItem 3";
            _listbox.Items.Add(_listitem1);
            _listbox.Items.Add(_listitem2);
            _listbox.Items.Add(_listitem3);

            _menu = new Menu();
            for (int i = 0; i < 5; i++)
            {
                MenuItem mi = new MenuItem();

                mi.Header = String.Format("Item {0}", i);
                _menu.Items.Add(mi);
                for (int c = 0; c < 5; c++)
                {
                    MenuItem submi = new MenuItem();

                    submi.Header = String.Format("SubItem {0}", c);
                    mi.Items.Add(submi);
                    if ((i == 0) && (c == 1))
                    {
                        for (int z = 0; z < 5; z++)
                        {
                            MenuItem subsubmi = new MenuItem();

                            subsubmi.Header = String.Format("SubSubMenuItem {0}", z);
                            submi.Items.Add(subsubmi);
                            if (z == 0)
                            {
                                _subsubsubmi = new MenuItem();

                                _subsubsubmi.Header = "SubSubSubMenuItem";
                                subsubmi.Items.Add(_subsubsubmi);
                            }
                        }
                    }
                    else if (c == 2)
                    {
                        MenuItem check = new MenuItem();

                        check.IsCheckable = true;
                        check.Header = "Checked Item";
                        mi.Items.Add(check);
                        mi.Items.Add(new Separator());
                    }
                }
            }



            root.Children.Add(_listbox);
            root.Children.Add(_menu);

            DRT.Show(root);

            return new DrtTest[]
                {
                    new DrtTest(StartUpComplete),
                };
        }

        private void StartUpComplete()
        {
#if OLD_AUTOMATION
            bool actionDone = true;

            //
            // ListBox : ISelectionProvider
            //
            // The listboxs default state is single select, selection required, and no items selected.
            DRT.Assert(((ISelectionProvider)_listbox).CanSelectMultiple == false, "((ISelectionProvider)_listbox).CanSelectMultiple should fail");
            DRT.Assert(((ISelectionProvider)_listbox).IsSelectionRequired == false, "((ISelectionProvider)_listbox).IsSelectionRequired should fail");
            DRT.Assert(((ISelectionProvider)_listbox).GetSelection().GetEnumerator().MoveNext() == false, "((ISelectionProvider)_listbox).Selection should be empty");

            //
            // ListBoxItem : ISelectionItemProvider
            //
            // Select on a listbox item should succeed and not throw
            actionDone = true;
            try
            {
                ((ISelectionItemProvider)_listitem1).Select();
            }
            catch (InvalidOperationException)
            {
                actionDone = false;
            }
            DRT.Assert(actionDone == true, "((ISelectionItemProvider)_listitem1).Select() failed");

            // Select on a listbox item should succeed and not throw
            actionDone = true;
            try
            {
                ((ISelectionItemProvider)_listitem2).Select();
            }
            catch (InvalidOperationException)
            {
                actionDone = false;
            }
            DRT.Assert(actionDone == true, "((ISelectionItemProvider)_listitem2).Select() failed");

            // Item2 is currently the only item selected
            IEnumerator ie = ((ISelectionProvider)_listbox).GetSelection().GetEnumerator();
            DRT.Assert(ie.MoveNext() == true, "((ISelectionProvider)_listbox).Selection should not be empty. _listitem2 should be selected.");
            DRT.Assert(AutomationProvider.VisualFromProvider((IRawElementProviderSimple)ie.Current) == _listitem2, "_listitem2 should be selected");
            DRT.Assert(ie.MoveNext() == false, "Only _listitem2 should be selected");

            // AddToSelection on a listbox item should succeed and not throw
            actionDone = true;
            try
            {
                ((ISelectionItemProvider)_listitem1).AddToSelection();
            }
            catch (InvalidOperationException)
            {
                actionDone = false;
            }
            DRT.Assert(actionDone == false, "((ISelectionItemProvider)_listitem1).AddToSelection() didn't fail");

            actionDone = true;
            try
            {
                ((ISelectionItemProvider)_listitem1).Select();
            }
            catch (InvalidOperationException)
            {
                actionDone = false;
            }
            DRT.Assert(actionDone == true, "((ISelectionItemProvider)_listitem1).Select() failed");

            // RemoveFromSelection on a selected listbox item should succeed and not throw
            actionDone = true;
            try
            {
                ((ISelectionItemProvider)_listitem1).RemoveFromSelection();
            }
            catch (InvalidOperationException)
            {
                actionDone = false;
            }
            DRT.Assert(actionDone == true, "((ISelectionItemProvider)_listitem1).RemoveFromSelection() failed");

            // RemoveFromSelection on a selected listbox item should succeed and not throw
            actionDone = true;
            try
            {
                ((ISelectionItemProvider)_listitem2).RemoveFromSelection();
            }
            catch (InvalidOperationException)
            {
                actionDone = false;
            }
            DRT.Assert(actionDone == true, "((ISelectionItemProvider)_listitem2).RemoveFromSelection() failed");

            // The listbox is empty after removing _listitem1 and _listitem2
            DRT.Assert(((ISelectionProvider)_listbox).GetSelection().GetEnumerator().MoveNext() == false,"((ISelectionProvider)_listbox).Selection should be empty after removing _listitem1 and _listitem2.");

            //
            // MenuItem : IExpandCollapseProvider
            //
            IExpandCollapseProvider iecp = ((IExpandCollapseProvider)(LogicalTreeHelper.GetParent(LogicalTreeHelper.GetParent(LogicalTreeHelper.GetParent(_subsubsubmi)))));

            // The top-level menu item is collapsed
            DRT.Assert(iecp.ExpandCollapseState == ExpandCollapseState.Collapsed, "ExpandCollapseState.Collapsed");

            // Expand on a top-level menu item should succeed and not throw
            actionDone = true;
            try
            {
                iecp.Expand();
            }
            catch (InvalidOperationException)
            {
                actionDone = false;
            }
            DRT.Assert(actionDone == true, "iecp.Expand() failed");

            // The top-level menu item is now be expanded
            DRT.Assert(iecp.ExpandCollapseState == ExpandCollapseState.Expanded, "ExpandCollapseState.Expanded");
            
            actionDone = true;
            try
            {
                iecp.Collapse();
            }
            catch (InvalidOperationException)
            {
                actionDone = false;
            }
            DRT.Assert(actionDone == true, "iecp.Collapse() failed");
#endif
        }
    }
}
