// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Logging;
using Microsoft.Test.Hosting;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.UIAutomaion;


namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Testing ScrollPattern for Controls Below
    /// ListBoxItem
    /// TreeViewItem
    /// </summary>
    [Serializable]
    public class UiaScrollItemTest : UiaSimpleTestcase
    {
        bool _visible = false;
        public override void Init(object target)
        {
            //make sure it is not in the visual tree
            if (target.GetType() == typeof(ListBoxItem))
            {
                ListBoxItem listboxitem = target as ListBoxItem;
                if (listboxitem != null)
                {
                    if (listboxitem.IsAncestorOf((ListBox)(listboxitem.Parent)))
                    {
                        _visible = true;
                    }
                }
            }
            else if (target.GetType() == typeof(TreeViewItem))
            {
                TreeViewItem treeviewitem = target as TreeViewItem;
                if (treeviewitem != null)
                {
                    if (treeviewitem.IsAncestorOf((TreeView)(((TreeViewItem)((TreeViewItem)treeviewitem.Parent).Parent).Parent)))
                    {
                        _visible = true;
                    }
                }
            }
        }

        public override void DoTest(AutomationElement target)
        {
            ScrollItemPattern sip = target.GetCurrentPattern(ScrollItemPattern.Pattern) as ScrollItemPattern;
            sip.ScrollIntoView();
        }

        public override void Validate(object target)
        {
            TestLog.Current.Result = TestResult.Pass;
            if (_visible)
            {
                TestLog.Current.LogEvidence("listboxitem or treeviewitem is in the visual tree and visible before scroll.");
                TestLog.Current.Result = TestResult.Fail;
            }

            if (target.GetType() == typeof(ListBoxItem))
            {
                ListBoxItem listboxitem = target as ListBoxItem;
                if (listboxitem != null)
                {
                    ListBox listbox = listboxitem.Parent as ListBox;
                    GeneralTransform transform = listboxitem.TransformToAncestor(listbox);
                    Rect listboxrect = new Rect(new Point(), listbox.RenderSize);
                    Rect listboxitemrect = new Rect(new Point(), listboxitem.RenderSize);
                    listboxitemrect = transform.TransformBounds(listboxitemrect);
                    // make sure listboxitem Y offset >= 0
                    if (listboxitemrect.Y < 0)
                    {
                        TestLog.Current.LogEvidence("listboxitem Y offset is less than zero.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    // make sure listboxitem Y offset plus listboxitem height is less than or equal listbox height.
                    if ((listboxitemrect.Y + listboxitem.Height) > listbox.Height)
                    {
                        TestLog.Current.LogEvidence("listboxitem Y offset plus listboxitem height is bigger than listbox height.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    TestLog.Current.LogEvidence("Validate listboxrect = " + listboxrect.ToString());
                    TestLog.Current.LogEvidence("Validate listboxitemrect = " + listboxitemrect.ToString());
                }
            }
            else if (target.GetType() == typeof(TreeViewItem))
            {
                TreeViewItem treeviewitem = target as TreeViewItem;
                if (treeviewitem != null)
                {
                    TreeView treeview = ((TreeViewItem)((TreeViewItem)treeviewitem.Parent).Parent).Parent as TreeView;
                    //TreeView treeview = TreeRoot(treeviewitem);
                    GeneralTransform transform = treeviewitem.TransformToAncestor(treeview);
                    Rect treeviewrect = new Rect(new Point(), treeview.RenderSize);
                    Rect treeviewitemrect = new Rect(new Point(), treeviewitem.RenderSize);
                    treeviewitemrect = transform.TransformBounds(treeviewitemrect);
                    // make sure treeviewitem Y offset >= 0
                    if (treeviewitemrect.Y < 0)
                    {
                        TestLog.Current.LogEvidence("treeviewitem Y offset is less than zero.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    // make sure treeviewitem Y offset plus treeviewitem height is less than or equal treeview height.
                    if ((treeviewitemrect.Y + treeviewitem.Height) > treeview.Height)
                    {
                        TestLog.Current.LogEvidence("treeviewitem Y offset plus treeviewitem height is bigger than treeview height.");
                        TestLog.Current.Result = TestResult.Fail;
                    }
                    TestLog.Current.LogEvidence("Validate treeviewrect = " + treeviewrect.ToString());
                    TestLog.Current.LogEvidence("Validate treeviewitemrect = " + treeviewitemrect.ToString());
                }
            }
            else
            {
                TestLog.Current.LogEvidence("Unknown test object.");
                TestLog.Current.Result = TestResult.Fail;
            }
        }

        TreeView TreeRoot(TreeViewItem treeviewitem)
        {
            if (!(treeviewitem.Parent is TreeView))
                TreeRoot((TreeViewItem)treeviewitem.Parent);
            return (TreeView)(treeviewitem.Parent);
        }
    }
}
