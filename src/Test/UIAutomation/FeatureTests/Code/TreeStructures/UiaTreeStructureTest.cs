// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Logging;
using Microsoft.Test.Hosting;
using System.Collections;
using System.Threading;
using Microsoft.Test.UIAutomaion;


namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Testing Controls below
    /// ComboBox
    /// ListBox
    /// TabControl
    /// TreeView
    /// ScrollBar
    /// ToolBar
    /// Slider
    /// </summary>    
    [Serializable]
    public class UiaTreeStructureTest : UiaSimpleTestcase
    {
        public override void Init(object target)
        {
        }

        public override void DoTest(AutomationElement target)
        {
            ArrayList items = new ArrayList();
            AutomationElement ae = TreeWalker.ControlViewWalker.GetFirstChild(target);
            if (ae == null)
            {
                ae = FindVirtualizedElement(target, null, null, null);
            }

            AutomationElement nextae = TreeWalker.ControlViewWalker.GetNextSibling(ae);
            if (nextae == null)
            {
                nextae = FindVirtualizedElement(target, ae, null, null);
            }

            AutomationElement temp = ae;
            while (temp != null)
            {
                nextae = TreeWalker.ControlViewWalker.GetNextSibling(temp);
                if (nextae == null)
                {
                    nextae = FindVirtualizedElement(target, temp, null, null);
                }

                items.Add(temp.Current.AutomationId);
                temp = nextae;
            }
            SharedState["items"] = items;
        }

        public override void Validate(object target)
        {
            TestLog.Current.Result = TestResult.Pass;
            ArrayList items = SharedState["items"] as ArrayList;

            if (target.GetType() == typeof(ComboBox))
            {
                ComboBox combobox = target as ComboBox;
                if (combobox != null)
                {
                    for (int i = 0; i < combobox.Items.Count; i++)
                    {
                        if (items[i].ToString() != ((ListBoxItem)combobox.Items[i]).Name)
                        {
                            TestLog.Current.LogEvidence("The combobox tree structure issue.");
                            TestLog.Current.LogEvidence("Automation comboboxitem id = " + items[i].ToString());
                            TestLog.Current.LogEvidence("Avalon comboboxitem id = " + ((ComboBoxItem)combobox.Items[i]).Name.ToString());
                            TestLog.Current.Result = TestResult.Fail;
                        }
                        TestLog.Current.LogEvidence("Automation comboboxitem id = " + items[i].ToString());
                        TestLog.Current.LogEvidence("Avalon comboboxitem id = " + ((ComboBoxItem)combobox.Items[i]).Name.ToString());
                    }
                }
            }
            else if (target.GetType() == typeof(ListBox))
            {
                ListBox listbox = target as ListBox;
                if (listbox != null)
                {
                    for (int i = 0; i < listbox.Items.Count; i++)
                    {
                        if (items[i].ToString() != ((ListBoxItem)listbox.Items[i]).Name)
                        {
                            TestLog.Current.LogEvidence("The listbox tree structure issue.");
                            TestLog.Current.LogEvidence("Automation listboxitem id = " + items[i].ToString());
                            TestLog.Current.LogEvidence("Avalon listboxitem id = " + ((ListBoxItem)listbox.Items[i]).Name.ToString());
                            TestLog.Current.Result = TestResult.Fail;
                        }
                        TestLog.Current.LogEvidence("Automation listboxitem id = " + items[i].ToString());
                        TestLog.Current.LogEvidence("Avalon listboxitem id = " + ((ListBoxItem)listbox.Items[i]).Name.ToString());
                    }
                }
            }
            else if (target.GetType() == typeof(TabControl))
            {
                TabControl tabcontrol = target as TabControl;
                if (tabcontrol != null)
                {
                    for (int i = 0; i < tabcontrol.Items.Count; i++)
                    {
                        if (items[i].ToString() != ((TabItem)tabcontrol.Items[i]).Name)
                        {
                            TestLog.Current.LogEvidence("The tabcontrol tree structure issue.");
                            TestLog.Current.LogEvidence("Automation tabitem id = " + items[i].ToString());
                            TestLog.Current.LogEvidence("Avalon tabitem id = " + ((TabItem)tabcontrol.Items[i]).Name.ToString());
                            TestLog.Current.Result = TestResult.Fail;
                        }
                        TestLog.Current.LogEvidence("Automation tabitem id = " + items[i].ToString());
                        TestLog.Current.LogEvidence("Avalon tabitem id = " + ((TabItem)tabcontrol.Items[i]).Name.ToString());
                    }
                }
            }
            else if (target.GetType() == typeof(TreeView))
            {
                TreeView treeview = target as TreeView;
                if (treeview != null)
                {
                    for (int i = 0; i < treeview.Items.Count; i++)
                    {
                        if (items[i].ToString() != ((TreeViewItem)treeview.Items[i]).Name)
                        {
                            TestLog.Current.LogEvidence("The treeview tree structure issue.");
                            TestLog.Current.LogEvidence("Automation treeviewitem id = " + items[i].ToString());
                            TestLog.Current.LogEvidence("Avalon treeviewitem id = " + ((TreeViewItem)treeview.Items[i]).Name.ToString());
                            TestLog.Current.Result = TestResult.Fail;
                        }
                        TestLog.Current.LogEvidence("Automation treeviewitem id = " + items[i].ToString());
                        TestLog.Current.LogEvidence("Avalon treeviewitem id = " + ((TreeViewItem)treeview.Items[i]).Name.ToString());
                    }
                }
            }
            else if (target.GetType() == typeof(ScrollBar))
            {
                ScrollBar scrollbar = target as ScrollBar;
                if (scrollbar != null)
                {
                    if (items[0].ToString() != "LineUp")
                    {
                        TestLog.Current.LogEvidence("The ScrollBar control tree first item is not LineUp Button");
                        TestLog.Current.LogEvidence("The ScrollBar control tree first item is " + items[0].ToString());
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    if (items[1].ToString() != "PageUp")
                    {
                        TestLog.Current.LogEvidence("The ScrollBar control tree second item is not PageUp Button");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    if (items[2].ToString() != "PageDown")
                    {
                        TestLog.Current.LogEvidence("The ScrollBar control tree third item is not PageDown Button");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    if (items[3].ToString() != "")
                    {
                        TestLog.Current.LogEvidence("The ScrollBar control tree forth item is not Thumb");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    if (items[4].ToString() != "LineDown")
                    {
                        TestLog.Current.LogEvidence("The ScrollBar control tree fifth item is not LineDown Button");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                }
            }
            else if (target.GetType() == typeof(ToolBar))
            {
                ToolBar toolbar = target as ToolBar;
                if (toolbar != null)
                {
                    if (items[0].ToString() != "OverflowButton")
                    {
                        TestLog.Current.LogEvidence("The ToolBar control tree first item is not OverflowButton Button");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    if (items[1].ToString() != "ToolBarThumb")
                    {
                        TestLog.Current.LogEvidence("The ToolBar control tree second item is not ToolBarThumb Thumb");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    if (items[2].ToString() != "button1")
                    {
                        TestLog.Current.LogEvidence("The ToolBar control tree third item is not button1 Button");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    if (items[3].ToString() != "button2")
                    {
                        TestLog.Current.LogEvidence("The ToolBar control tree forth item is not button2 Button");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    if (items[4].ToString() != "button3")
                    {
                        TestLog.Current.LogEvidence("The ToolBar control tree fifth item is not button3 Button");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                }
            }
            else if (target.GetType() == typeof(Slider))
            {
                Slider slider = target as Slider;
                if (slider != null)
                {
                    if (items[0].ToString() != "DecreaseLarge")
                    {
                        TestLog.Current.LogEvidence("The Slider control tree first item is not DecreaseLarge Button");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    if (items[1].ToString() != "IncreaseLarge")
                    {
                        TestLog.Current.LogEvidence("The Slider control tree second item is not IncreaseLarge Button");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    if (items[2].ToString() != "Thumb")
                    {
                        TestLog.Current.LogEvidence("The Slider control tree third item is not Thumb");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                }
            }
            else
            {
                TestLog.Current.LogEvidence("Unknown test object.");
                TestLog.Current.Result = TestResult.Fail;
            }
            Thread.Sleep(1000);
        }
        /// <summary>
        /// If Virtualization is happening, this function will find
        /// virtualized element that is otherwise not visible via
        /// regular find APIs
        /// </summary>
        /// <param name=""></param>
        /// <returns>AutomationElement that follows startAfter element
        /// inside a container
        /// </returns>
        private AutomationElement FindVirtualizedElement(AutomationElement parentElement, AutomationElement startAfter, AutomationProperty property, object value)
        {

#if TESTBUILD_CLR40

            if (AutomationElement.IsItemContainerPatternAvailableProperty != null)
            {
                if (((bool)(parentElement.GetCurrentPropertyValue(AutomationElement.IsItemContainerPatternAvailableProperty))) == true)
                {
                    ItemContainerPattern itemContainerPattern = parentElement.GetCurrentPattern(ItemContainerPattern.Pattern) as ItemContainerPattern;
                    return (itemContainerPattern.FindItemByProperty(startAfter, property, value));
                }
            }

 #endif

            return null;
        }
    }
}
