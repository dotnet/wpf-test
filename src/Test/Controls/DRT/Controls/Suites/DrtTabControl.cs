// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Text;
using System.Globalization;
//#if OLD_AUTOMATION
using System.Windows.Automation.Provider;
//#endif
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Automation;
using System.IO;
using System.Windows.Data;
using System.Reflection;
using System.Windows.Markup;
using System.Collections.Generic;
using System.Windows.Documents;

namespace DRT
{
    public class TabControlSuite : DrtTestSuite
    {
        public TabControlSuite() : base("TabControl")
        {
            Contact = "Microsoft";
        }

        StackPanel _stackPanel;
        Button _button1;
        TabControl _tabControl;
        Button _button2;

        static int s_numItems = 20;

        public override DrtTest[] PrepareTests()
        {
            // Create Elements
            _stackPanel = new StackPanel();
            _stackPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
            _stackPanel.VerticalAlignment = VerticalAlignment.Top;
            _stackPanel.Background = SystemColors.WindowBrush;

            _button1 = new Button();
            _button1.VerticalAlignment = VerticalAlignment.Bottom;
            _button1.Content = "Button1";

            _tabControl = new TabControl();
            _tabControl.VerticalAlignment = VerticalAlignment.Top;
            _tabControl.Width = 250;
            _tabControl.Height = 300;
            for (int i = 0; i < s_numItems; i++)
            {
                if ((i + 1) % 4 == 0)
                {
                    TabItem item = new TabItem();
                    item.MinWidth=20;
                    item.MinHeight=20;
                    Button b = new Button();
                    b.Content = "Button " + i.ToString();
                    item.Content = b;
                    _tabControl.Items.Add(item);
                }
                else
                {
                    TabItem item = new TabItem();
                    item.MinWidth=20;
                    item.MinHeight=20;

                    item.Name = "Item" + i;

                    if (i==1)
                    {
                        item.Header = "_Item 1";
                    }
                    else
                        item.Header = "Item " + i.ToString();

                    _tabControl.Items.Add(item);

                    StackPanel sp = new StackPanel();
                    sp.Orientation = System.Windows.Controls.Orientation.Horizontal;
                    Button b = new Button();

                    b.Content = "Content Button " + i;
                    b.Name = "ContentButton" + i;
                    sp.Children.Add(b);
                    item.Content = sp;
                }
            }

            _button2 = new Button();
            _button2.VerticalAlignment = VerticalAlignment.Bottom;
            _button2.Content = "Button2";

            // Build tree links
            DRT.Show(_stackPanel);
            _stackPanel.Children.Add(_button1);
            _stackPanel.Children.Add(_tabControl);
            _stackPanel.Children.Add(_button2);

            _stackPanel.Width = 700;
            _stackPanel.Height = 500;

            if (!DRT.KeepAlive)
            {
                return new DrtTest[]
                {
                    new DrtTest(Start),
                    new DrtTest(Step1),
                    new DrtTest(Step2),
                    new DrtTest(Step3),
                    new DrtTest(Step4),
                    new DrtTest(Step5),
                    new DrtTest(Step6),
                    new DrtTest(Step7),
                    new DrtTest(Step8),
                    new DrtTest(Step9),
                    new DrtTest(Cleanup),
                };
            }
            else
            {
                return new DrtTest[] { };
            }
        }

        private void Start()
        {
            // Set initial focus to button1
            _button1.Focus();
            DRT.MoveMouse(_button1, 0.5, 0.5);
            DRT.ClickMouse();

            // Verify initial selection
            DRT.Assert(_tabControl.SelectedIndex == 0, "TabControl.SelectedIndex should be 0");
            TabItem ti = _tabControl.SelectedItem as TabItem;
            DRT.Assert(ti != null, "TabControl.SelectedItem should not be null");
        }


        private void Step1()
        {
            // Tab to TabControl
            DRT.SendKeyboardInput(Key.Tab, true);
            DRT.SendKeyboardInput(Key.Tab, false);
        }

        private void Step2()
        {
            // Verify focus
            TabItem ti = _tabControl.Items[0] as TabItem;
            DRT.Assert(ti.IsKeyboardFocused, "First TabItem should have focus");

            // Tab to content
            DRT.SendKeyboardInput(Key.Tab, true);
            DRT.SendKeyboardInput(Key.Tab, false);
        }

        private void Step3()
        {
            // Verify focus
            TabItem ti = _tabControl.Items[0] as TabItem;
            Button b = ((StackPanel)ti.Content).Children[0] as Button;
            DRT.Assert(b.IsKeyboardFocused, "First TabItem button should have focus");

            // Tab outside TabControl
            DRT.SendKeyboardInput(Key.Tab, true);
            DRT.SendKeyboardInput(Key.Tab, false);
        }

        private void Step4()
        {
            // Verify focus
            DRT.Assert(_button2.IsKeyboardFocused, "First TabItem button should have focus");

            // Click on second TabItem
            TabItem ti = _tabControl.Items[1] as TabItem;

            DRT.MoveMouse(ti, 0.5, 0.5);
            DRT.ClickMouse();
        }

        private void Step5()
        {
            // Verify focus and selection
            TabItem ti = _tabControl.Items[1] as TabItem;
            Button b = ((StackPanel)ti.Content).Children[0] as Button;
            DRT.Assert(b.IsKeyboardFocused, "Second TabItem button should have focus");
            DRT.Assert(ti.IsSelected, "tabitem1.IsSelected should be true");
            DRT.Assert(_tabControl.SelectedIndex == 1, "SelectedIndex should be 1 instead of " + _tabControl.SelectedIndex);
            DRT.Assert(_tabControl.SelectedItem == ti, "Second TabItem should be selected");

            // Ctrl+Tab
            DRT.SendKeyboardInput(Key.LeftCtrl, true);
            DRT.SendKeyboardInput(Key.Tab, true);
            DRT.SendKeyboardInput(Key.Tab, false);
            DRT.SendKeyboardInput(Key.LeftCtrl, false);
        }

        private void Step6()
        {
            // Verify focus and selection
            TabItem ti = _tabControl.Items[2] as TabItem;
            Button b = ((StackPanel)ti.Content).Children[0] as Button;

            DRT.Assert(b.IsKeyboardFocused, "Third TabItem button should have focus");
            DRT.Assert(ti.IsSelected, "tabitem2.IsSelected should be true");
            DRT.Assert(_tabControl.SelectedIndex == 2, "SelectedIndex should be 1 instead of " + _tabControl.SelectedIndex);
            DRT.Assert(_tabControl.SelectedItem == ti, "Third TabItem should be selected");

            // Click over the same header - focus should move to header
            DRT.MoveMouse(ti, 0.5, 0.5);
            DRT.ClickMouse();
        }

        private void Step7()
        {
            // Verify focus and selection
            TabItem ti = _tabControl.Items[2] as TabItem;
            Button b = ((StackPanel)ti.Content).Children[0] as Button;

            DRT.Assert(ti.IsKeyboardFocused, "Third TabItem should have focus");
            DRT.Assert(ti.IsSelected, "tabitem2.IsSelected should be true");
            DRT.Assert(_tabControl.SelectedIndex == 2, "SelectedIndex should be 1 instead of " + _tabControl.SelectedIndex);
            DRT.Assert(_tabControl.SelectedItem == ti, "Third TabItem should be selected");

            // Click over the next header - focus should move to header
            ti = _tabControl.ItemContainerGenerator.ContainerFromIndex(3) as TabItem;
            DRT.MoveMouse(ti, 0.5, 0.5);
            DRT.ClickMouse();
        }

        private void Step8()
        {
            // Verify focus and selection
            TabItem ti = _tabControl.ItemContainerGenerator.ContainerFromIndex(3) as TabItem;

            DRT.Assert(ti.IsKeyboardFocused, "TabItem[3] should have focus");
            DRT.Assert(ti.IsSelected, "tabitem[3].IsSelected should be true");
            DRT.Assert(_tabControl.SelectedIndex == 3, "SelectedIndex should be 1 instead of " + _tabControl.SelectedIndex);
            DRT.Assert(_tabControl.SelectedItem == _tabControl.ItemContainerGenerator.ItemFromContainer(ti), "TabItem[3] should be selected");

            // Alt+I
            if (_tabControl.TabStripPlacement == Dock.Right)
            {
                DRT.SendKeyboardInput(Key.LeftAlt, true);
                DRT.SendKeyboardInput(Key.I, true);
                DRT.SendKeyboardInput(Key.I, false);
                DRT.SendKeyboardInput(Key.LeftAlt, false);
            }

        }

        private void Step9()
        {
            if (_tabControl.TabStripPlacement == Dock.Right)
                DRT.Assert(_tabControl.SelectedIndex == 1, "SelectedIndex should be 1 instead of " + _tabControl.SelectedIndex);

            // Change to orientation and repeat test
            if (_tabControl.TabStripPlacement == Dock.Top)
                _tabControl.TabStripPlacement = Dock.Bottom;
            else if (_tabControl.TabStripPlacement == Dock.Bottom)
                _tabControl.TabStripPlacement = Dock.Left;
            else if (_tabControl.TabStripPlacement == Dock.Left)
                _tabControl.TabStripPlacement = Dock.Right;
            else //if (_tabControl.TabStripPlacement == Dock.Right)
                return;

#if OLD_AUTOMATION
            ISelectionItemProvider isip = (ISelectionItemProvider)(_tabControl.ItemContainerGenerator.ContainerFromIndex(0));
            isip.Select();

            DRT.Assert(isip.IsSelected, "TabItem[0] should be selected");

            bool tryAdd = true;
            try {
                isip.AddToSelection();
            }
            catch( InvalidOperationException )
            {
                tryAdd = false;
            }
            DRT.Assert(!tryAdd, "AddToSelection shouldn't select the item");

            tryAdd = true;
            try
            {
                isip.Select();
            }
            catch (InvalidOperationException)
            {
                tryAdd = false;
            }
            DRT.Assert(tryAdd, "Select should select the item");

            bool tryRemove = true;
            try {
                isip.RemoveFromSelection();
            }
            catch( InvalidOperationException )
            {
                tryRemove = false;
            }
            DRT.Assert(!tryRemove, "RemoveFromSelection should always fail");
#else
            int tabItemIndex = 0;
            TabItem tabItem = (TabItem)(_tabControl.ItemContainerGenerator.ContainerFromIndex(tabItemIndex));
            _tabControl.SelectedIndex = tabItemIndex;

            DRT.Assert(tabItem.IsSelected, "TabItem[0] should be selected");
#endif

            _button1.Focus();

            // They get pushed on the resume stack in reverse order -- this API should be different...
            DRT.ResumeAt(new DrtTest(Step9));
            DRT.ResumeAt(new DrtTest(Step8));
            DRT.ResumeAt(new DrtTest(Step7));
            DRT.ResumeAt(new DrtTest(Step6));
            DRT.ResumeAt(new DrtTest(Step5));
            DRT.ResumeAt(new DrtTest(Step4));
            DRT.ResumeAt(new DrtTest(Step3));
            DRT.ResumeAt(new DrtTest(Step2));
            DRT.ResumeAt(new DrtTest(Step1));
        }

        private void Cleanup()
        {
        }

    }

}
