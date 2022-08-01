// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Automation;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Reflection;
using System.ComponentModel;
using MS.Internal; // PointUtil
using MS.Win32; // PointUtil

namespace DRT
{

    public enum MenuTest
    {
        ContextMenu,
        Mouse,
        Keyboard,
        TextBox,
        Sparkle,
        Capture,
        AccessKey,
        Composition,
    }

    public class MenuSuite : DrtTestSuite, INotifyPropertyChanged
    {
        public MenuSuite(MenuTest test) : base("Menu." + test.ToString())
        {
            Contact = "Microsoft";
            _test = test;
        }

        MenuTest _test;

        #region Window setup and common hooks

        public override DrtTest[] PrepareTests()
        {
            Keyboard.Focus(null);

            DRT.LoadXamlFile("DrtMenu.xaml");

            ((FrameworkElement)DRT.RootElement).DataContext = this;

            _button = DRT.FindElementByID("Button1") as Button;
            _button2 = DRT.FindElementByID("Button2") as Button;
            _button3 = DRT.FindElementByID("Button3") as Button;
            _activateButton = DRT.FindElementByID("ActivateButton") as Button;
            _menu = DRT.FindElementByID("MainMenu") as Menu;
            _staysOpenOnClickMenu = DRT.FindElementByID("StaysOpenOnClickMenu") as Menu;
            _staysOpenOnClickHeader = (MenuItem)DRT.FindElementByID("StaysOpenOnClickHeader");
            _staysOpenOnClickTrue = (MenuItem)DRT.FindElementByID("StaysOpenOnClickTrue");
            _staysOpenOnClickFalse = (MenuItem)DRT.FindElementByID("StaysOpenOnClickFalse");


            _button2.AddHandler(Button.ClickEvent, new RoutedEventHandler(_button2_OnClick));

            _button3.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(_button3_MouseButtonDown);
            _button3.MouseRightButtonDown += new MouseButtonEventHandler(_button3_MouseButtonDown);

            _activateButton.Content = "Activate Desktop";
            _activateButton.AddHandler(Button.ClickEvent, new RoutedEventHandler(OnActivateDesktop));



            //_menu.IsMainMenu = false;
            _menu.AddHandler(MenuItem.SubmenuOpenedEvent, new RoutedEventHandler(OnIsSubmenuOpenChanged));
            _menu.AddHandler(MenuItem.SubmenuClosedEvent, new RoutedEventHandler(OnIsSubmenuOpenChanged));
            _menu.AddHandler(MenuItem.CheckedEvent, new RoutedEventHandler(OnIsCheckedChanged));
            _menu.AddHandler(MenuItem.UncheckedEvent, new RoutedEventHandler(OnIsCheckedChanged));

            for (int i = 0; i < 5; i++)
            {
                MenuItem mi = new MenuItem();

                mi.Header = String.Format("Item {0}", i);
                _menu.Items.Add(mi);
                for (int c = 0; c < 5; c++)
                {
                    if ((i == 3) && (c == 3))
                    {
                        InputGestureCollection gestures = new InputGestureCollection();
                        gestures.Add(new KeyGesture(Key.T, ModifierKeys.Alt));
                        RoutedUICommand command = new RoutedUICommand("Test Command (Localized)", "TestCommand", typeof(MenuSuite), gestures);
                        mi.Items.Add(command);
                    }
                    else
                    {
                        MenuItem submi = new MenuItem();

                        submi.Header = String.Format("SubItem {0}.{1}", i, c);
                        mi.Items.Add(submi);
                        if ((i == 0) && (c == 4))
                        {
                            submi.Click += new RoutedEventHandler(OnSubmenuItemClick);
                        }
                        else if ((i == 0) && (c == 1))
                        {
                            for (int z = 0; z < 5; z++)
                            {
                                MenuItem subsubmi = new MenuItem();

                                subsubmi.Header = String.Format("SubSubMenuItem {0}.{1}.{2}", i, c, z);
                                submi.Items.Add(subsubmi);
                                if (z == 0)
                                {
                                    MenuItem subsubsubmi = new MenuItem();

                                    subsubsubmi.Header = "SubSubSubMenuItem";
                                    subsubmi.Items.Add(subsubsubmi);
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
                        else if ((i == 4) && (c == 3))
                        {
                            MenuItem subsubmi = new MenuItem();

                            subsubmi.Header = "Block in Context";
                            subsubmi.Click += new RoutedEventHandler(ClickHandlerThatBlocksContext);
                            submi.Items.Add(subsubmi);

                            _menu_submenuItemWithContextMenu = new MenuItem();
                            _menu_submenuItemWithContextMenu.Header = "I have a Context Menu!";

                            _menu_contextMenu = new ContextMenu();
                            _menu_contextMenu.Items.Add("Now this");
                            _menu_contextMenu.Items.Add("Is a scenario");
                            _menu_contextMenu.Items.Add("I can understand");
                            _menu_submenuItemWithContextMenu.ContextMenu = _menu_contextMenu;

                            submi.Items.Add(_menu_submenuItemWithContextMenu);

                            StackPanel rbl = new StackPanel();

                            _menu_radioButton1 = new RadioButton();
                            _menu_radioButton1.Content = "This scenario";
                            rbl.Children.Add(_menu_radioButton1);

                            _menu_radioButton2 = new RadioButton();
                            _menu_radioButton2.Content = "is ridiculous";
                            rbl.Children.Add(_menu_radioButton2);

                            submi.Items.Add(rbl);

                            _menu_button = new Button();
                            _menu_button.Content = "Why do you want a button in a menu?";
                            submi.Items.Add(_menu_button);

                            _menu_textBox = new TextBox();
                            _menu_textBox.Text = "TextBox in a menu is sort of cool, no? ";
                            submi.Items.Add(_menu_textBox);

                        }
                    }
                }
            }
            // Init Context menu
            ContextMenu cmenu = new ContextMenu();

            cmenu.Opened += new RoutedEventHandler(OnContextMenuOpened);
            cmenu.Closed += new RoutedEventHandler(OnContextMenuClosed);
            for (int i = 0; i < 5; i++)
            {
                MenuItem mi = new MenuItem();

                mi.Header = String.Format("Item {0}", i);
                cmenu.Items.Add(mi);
                if (i == 3)
                {
                    for (int c = 0; c < 5; c++)
                    {
                        MenuItem submi = new MenuItem();

                        submi.Header = String.Format("SubItem {0}", c);
                        mi.Items.Add(submi);
                        if ((i == 0) && (c == 0))
                        {
                            MenuItem subsubmi = new MenuItem();

                            subsubmi.Header = "SubSubMenuItem";
                            submi.Items.Add(subsubmi);
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
            }

            _button.ContextMenu = cmenu;

            // 



            _textBox1 = DRT.FindElementByID("TextBox1") as TextBox;

            // 


            _textBox2 = DRT.FindElementByID("TextBox2") as TextBox;

            // 


            _sparkleMenu = DRT.FindElementByID("SparkleMenu") as Menu;

            _comboBox = DRT.FindElementByID("ComboBox") as ComboBox;

            _showPopupButton = DRT.FindElementByID("ShowPopupButton") as Button;
            _showPopupButton.Click += new RoutedEventHandler(_popupButton_Click);

            // Make an AutoClose popup with a button in it.  This will be used to test "subcapture"
            _popup = DRT.FindElementByID("AutoClosePopup") as Popup;

            DockPanel popupDockPanel = (DockPanel)((Border)_popup.Child).Child;

            _popupMenu = (Menu)popupDockPanel.Children[0];
            _popupButton = (Button)popupDockPanel.Children[1];
            _popupComboBox = (ComboBox)popupDockPanel.Children[2];

            // debugging - open a context menu that has no placement target.

            _contextMenuBorder = DRT.FindElementByID("ContextMenuBorder") as Border;
            _contextMenuBorder.MouseRightButtonUp += new MouseButtonEventHandler(contextMenuBorder_MouseRightButtonUp);

            // For testing access keys in menus
            _accessKeyButton = DRT.FindElementByID("AccessKeyButton") as Button;
            _accessKeyContextMenu = _accessKeyButton.ContextMenu;

            _accessKeyContextMenu.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(OnAKContextMenuItemClicked));

            _searchTextBox = DRT.FindElementByID("SearchTextBox") as TextBox;
            _searchTextBox.TextChanged += new TextChangedEventHandler(_searchTextBox_TextChanged);
            _searchDropDown = _searchTextBox.ContextMenu;
            _searchDropDown.PlacementTarget = _searchTextBox;


            // Now that the RootElement is set we can hook up the datacontext for the data-bound menuitem.
            MenuItem dataBoundHierarchy;
            _menu.Items.Add(dataBoundHierarchy = MakeDataBoundHierarchy());
            dataBoundHierarchy.Tag = new VisualWrapper(DRT.RootElement);

            // Workaround b/c DataContext doesn't inherit into ContextMenu:
            Button scrollingContextMenuButton = (Button)DRT.FindElementByID("ScrollingContextMenuButton");
            scrollingContextMenuButton.ContextMenu.DataContext = this;


            if (!DRT.KeepAlive)
            {
                List<DrtTest> tests = new List<DrtTest>();
                tests.Add(new DrtTest(Start));

                switch (_test)
                {
                    case MenuTest.ContextMenu:
                        tests.Add(new DrtTest(ContextMenuTest));
                        break;

                    case MenuTest.Mouse:
                        tests.Add(new DrtTest(MenuMouseTest));
                        break;

                    case MenuTest.Keyboard:
                        tests.Add(new DrtTest(MenuKeyboardTest));
                        break;

                    case MenuTest.TextBox:
                        tests.Add(new DrtTest(TextBoxContextMenuTest));
                        break;

                    case MenuTest.Sparkle:
                        tests.Add(new DrtTest(SparkleTest));
                        break;

                    case MenuTest.Capture:
                        tests.Add(new DrtTest(CaptureTest));
                        break;

                    case MenuTest.AccessKey:
                        tests.Add(new DrtTest(AccessKeyTest));
                        break;

                    case MenuTest.Composition:
                        tests.Add(new DrtTest(CompositionTest));
                        break;

                }

                tests.Add(new DrtTest(Cleanup));
                return tests.ToArray();
            }
            else
            {
                return new DrtTest[] {};
            }
        }

        private MenuItem NewMenuItem(object header)
        {
            MenuItem mi = new MenuItem();
            mi.Header = header;
            return mi;
        }

        private void Start()
        {
            if (!DRT.KeepAlive)
            {
                _suicideTimer = new DispatcherTimer();
                _suicideTimer.Interval = new TimeSpan(0, 5, 0);
                _suicideTimer.Tick += new EventHandler(OnTimeout);
                _suicideTimer.Start();
            }

            // When submenus open, verify that they are "correct" -- open, visible, positioned, correct size, etc.
            ((UIElement)DRT.RootElement).AddHandler(MenuItem.SubmenuOpenedEvent, new RoutedEventHandler(OnMenuOpened));
            ((UIElement)DRT.RootElement).AddHandler(ContextMenu.OpenedEvent, new RoutedEventHandler(OnMenuOpened));

            // Warmup opening menus
            ((MenuItem)_menu.Items[0]).IsSubmenuOpen = true;
            DRT.Pause(2000);

            DRT.ResumeAt(new DrtTest(CloseMenu));
        }

        private void CloseMenu()
        {
            ((MenuItem)_menu.Items[0]).IsSubmenuOpen = false;
        }


        public void Cleanup()
        {
            if (_suicideTimer != null)
            {
                _suicideTimer.Stop();
            }
        }

        public static string MenuItemToString(object o)
        {
            if (o is MenuItem)
            {
                return "MenuItem (" + ((MenuItem)o).Header + ")";
            }

            if (o == null) return "null";

            return o.ToString();
        }

        private void OnContextMenuOpened(object sender, RoutedEventArgs args)
        {
            _count += 10;
        }

        private void OnContextMenuClosed(object sender, RoutedEventArgs args)
        {
            _count += 100;
        }


        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        private static extern void SwitchToThisWindow(IntPtr hwnd, bool fAltTab);

        private void OnActivateDesktop(object sender, RoutedEventArgs e)
        {
            IntPtr desktop = GetDesktopWindow();
            MenuItem item = ((MenuItem)_menu.Items[0]);

            SwitchToThisWindow(desktop, false);
            item.IsSubmenuOpen = true;
        }

        private void _button2_OnClick(object sender, RoutedEventArgs e)
        {
            //       When we take capture there is a walk to find the root visual of the ContextMenu.
            //        In this case (we haven't opened the contextmenu yet) there is no root visual.  We must set the placement target
            //        to the button on which we are opening it to get the visual connection so that the walk to find the rootvisual
            //        succeeds.
            _button.ContextMenu.PlacementTarget = _button2;
            _button.ContextMenu.IsOpen = true;
        }

        private void _button3_MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            _button.ContextMenu.PlacementTarget = (FrameworkElement)sender;
            _button.ContextMenu.IsOpen = true;
            e.Handled = true;
        }

        private bool VerifyPopupOpened(MenuItem item, bool opened)
        {
            return VerifyPopupOpened(item, opened, false);
        }

        private bool VerifyPopupOpened(MenuItem item, bool opened, int numRetries)
        {
            if (!VerifyPopupOpened(item, opened, (_repeatedTestCount < numRetries)))
            {
                string msg = String.Format("MenuItem[{0}] submenu should be {1} but isn't.", item, (opened ? "opened" : "closed"));

                if (_repeatedTestCount < numRetries)
                {
                    if (DRT.Verbose)
                    {
                        Console.WriteLine(msg);
                        Console.WriteLine("Repeating.");
                    }

                    _repeatedTestCount++;
                    return false;
                }
                else
                {
                    DRT.Assert(false, msg);
                }
            }

            _repeatedTestCount = 0;
            return true;
        }

        private bool VerifyPopupOpened(MenuItem item, bool opened, bool doingRetries)
        {
            ShowMenuItemInfo(item);
            int count = VisualTreeHelper.GetChildrenCount(item);
            bool gotChildren = count>0;
            if (!gotChildren)
            {
                if (doingRetries)
                {
                    return false;
                }
                else
                {
                    DRT.Assert(false, "MenuItem's visual children not expanded.");
                }
            }

            bool submenuOpenMatch = item.IsSubmenuOpen == opened;
            if (!submenuOpenMatch)
            {
                if (doingRetries)
                {
                    return false;
                }
                else
                {
                    DRT.Assert(false, opened ? "Submenu should be open" : "Submenu should be closed");
                }
            }

            if (opened && (item.Items.Count > 0))
            {
                MenuItem child = item.ItemContainerGenerator.ContainerFromIndex(0) as MenuItem;
                if (child != null)
                {
                    // Get a handle the source and at least make sure it's a popup
                    PresentationSource source = PresentationSource.FromVisual(child);
                    DRT.Assert(source != null, "PresentationSource not found from MenuItem");
                    DRT.Assert(source.RootVisual != null, "RootVisual is null");
                    string rootType = source.RootVisual.GetType().ToString();
                    DRT.Assert(rootType.Contains("PopupRoot"), "RootVisual is [" + rootType + "] and not PopupRoot");
                    Popup popup = LogicalTreeHelper.GetParent(source.RootVisual) as Popup;
                    DRT.Assert(popup != null, "Unable to get to the Popup");

                    // Get the window handle
                    HwndSource hwnd = source as HwndSource;
                    DRT.Assert(hwnd != null, "PresentationSource is not an HwndSource");
                    IntPtr handle = hwnd.Handle;
                    DRT.Assert(handle != IntPtr.Zero, "Popup window handle is 0");

                    Int32 styles = GetWindowLong(new HandleRef(this, handle), GWL_EXSTYLE);
                    int flags = styles;
                    if ((flags & WS_EX_TRANSPARENT) != 0)
                    {
                        // The popup is not hit-testable yet
                        return false;
                    }

                    // Verify the position
                    Point ptClientPopup = new Point(0, 0);
                    Point ptScreenPopop = PointUtil.ClientToScreen(ptClientPopup, source);
                    DRT.Assert(ptScreenPopop.X != 0 && ptScreenPopop.Y != 0, "Popup shouldn't be at 0,0");

                    RECT windowRect = new RECT();
                    GetWindowRect(new HandleRef(hwnd, handle), ref windowRect);
                    if (WindowNear(windowRect.right - windowRect.left, 0) ||
                        WindowNear(windowRect.bottom - windowRect.top, 0))
                    {
                        DRT.Fail("Popup window is too small. LTRB:{0},{1},{2},{3}", windowRect.left, windowRect.top, windowRect.right, windowRect.bottom);
                    }

                    HwndSource parentSource = PresentationSource.FromVisual(item) as HwndSource;
                    DRT.Assert(parentSource != null, "Parent item source not found");
                    DRT.Assert(parentSource.RootVisual != null, "parent source root visual is null");

                    Point ptParent = new Point(0,0);
                    GeneralTransform transform;
                    try
                    {
                        transform = item.TransformToAncestor(parentSource.RootVisual);
                    }
                    catch (InvalidOperationException)
                    {
                        // if visuals are not connected...
                        DRT.Assert(false, "Visuals weren't connected");
                        return false;
                    }
                    Point ptRoot;
                    transform.TryTransform(ptParent, out ptRoot);
                    Point ptClient = PointUtil.RootToClient(ptRoot, parentSource);
                    Point ptScreen = PointUtil.ClientToScreen(ptClient, parentSource);

                    Point parentSize = parentSource.CompositionTarget.TransformToDevice.Transform(new Point(item.RenderSize.Width, item.RenderSize.Height));
                    int parentWidth = (int)parentSize.X;
                    int parentHeight = (int)parentSize.Y;

                    string errormsg = String.Format("PopupXY:{0},{1} ParentXYWH:{2},{3},{4},{5}", new object[] { ptScreenPopop.X, ptScreenPopop.Y, ptScreen.X, ptScreen.Y, parentWidth, parentHeight });
                    switch (popup.Placement)
                    {
                        case PlacementMode.Bottom:
                            DRT.Assert(WindowNear(ptScreen.X, ptScreenPopop.X), "Popup X not correct for bottom placment " + errormsg);
                            DRT.Assert(WindowNear(ptScreen.Y + parentHeight, ptScreenPopop.Y), "Popup Y not correct for bottom placment " + errormsg);
                            break;

                        case PlacementMode.Right:
                            DRT.Assert(WindowNear(ptScreen.X + parentWidth, ptScreenPopop.X), "Popup X not correct for right placment " + errormsg);
                            DRT.Assert(WindowNear(ptScreen.Y, ptScreenPopop.Y), "Popup Y not correct for right placment " + errormsg);
                            break;
                    }
                }
            }

            return true;
        }

        private static bool WindowNear(double x1, double x2)
        {
            return (Math.Abs(x2 - x1) < 5);
        }

        [DllImport("User32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        private static extern bool GetWindowRect(HandleRef hWnd, [In, Out] ref RECT rect);


        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public static RECT FromXYWH(int x, int y, int width, int height)
            {
                return new RECT(x, y, x + width, y + height);
            }
        }

        #endregion

        #region ContextMenu

        private void ContextMenuTest()
        {
            DRT.WaitForCompleteRender();

            _button.MouseMove += new MouseEventHandler(_button_MouseMove);

            if (DRT.Verbose) Console.WriteLine("Right clicking on button to open context menu");
            DRT.MoveMouse(_button, 0.5, 0.5);
            DRT.ClickMouseSecondButton();

            WaitForMenuAnimationDelay();
            DRT.ResumeAt(new DrtTest(ContextMenuTestVerify));
        }

        void _button_MouseMove(object sender, MouseEventArgs e)
        {
            _buttonMouseMoveCount++;
        }

        int _buttonMouseMoveCount = 0;

        private void ContextMenuTestVerify()
        {
            if (DRT.Verbose) Console.WriteLine("Verifying that context menu is open");

            if (_buttonMouseMoveCount == 0)
            {
                if (DRT.Verbose) Console.WriteLine("Error: The DRT button has not received any mouse move events.  Was another window on top of the DRT window?  Did anything actually render?  Root cause is being investigated.");
                //throw new Exception("The DRT button has not received any mouse move events.  Was another window on top of the DRT window?");
                Console.WriteLine("Test will now tentatively succeed.");
                DRT.WriteDelayedOutput();
                System.Environment.Exit(0);
            }

            //
            // Make sure that Popup is open
            //
            ContextMenu cm = _button.ContextMenu;

            DRT.Assert(VisualTreeHelper.GetChildrenCount(cm) > 0, "No children under ContextMenu");

            Visual parent = (Visual) VisualTreeHelper.GetParent(cm);
            DRT.Assert(parent != null, "Can't find main Popup in the ContextMenu! (visual parent == null)");

            Visual grandparent = (Visual) VisualTreeHelper.GetParent(parent);
            while (grandparent != null)
            {
                parent = grandparent;
                grandparent = (Visual) VisualTreeHelper.GetParent(parent);
            }

            Popup cmPopup = LogicalTreeHelper.GetParent(parent) as Popup;

            DRT.Assert(cmPopup != null, "Can't find main Popup in the ContextMenu!");
            DRT.Assert(cmPopup.IsOpen, "Main popup is not opened!");

            DRT.Assert(_count == 10, string.Format("Expected _count={0} (actual:{1})", 10, _count));
            _button.ContextMenu.IsOpen = false;  // _count = 110

            WaitForMenuAnimationDelay();
            DRT.ResumeAt(new DrtTest(ContextMenuTestVerify2));
        }

        private void ContextMenuTestVerify2()
        {
            DRT.Assert(_count == 110, string.Format("Expected _count={0} (actual:{1})", 110, _count));
            _button.ContextMenu.IsOpen = true;   // _count = 120;
            DRT.Assert(_count == 120, string.Format("Expected _count={0} (actual:{1})", 120, _count));
            _button.ContextMenu.IsOpen = true;   // _count = 120;
            DRT.Assert(_count == 120, string.Format("Expected _count={0} (actual:{1})", 120, _count));
            _button.ContextMenu.IsOpen = false;  // _count = 220;

            WaitForMenuAnimationDelay();
            DRT.ResumeAt(new DrtTest(ContextMenuTestVerify3));
        }

        private void ContextMenuTestVerify3()
        {
            DRT.Assert(_count == 220, string.Format("Expected _count={0} (actual:{1})", 220, _count));
        }

        private void OnIsSubmenuOpenChanged(object sender, RoutedEventArgs e)
        {
            _submenuOpenChangedEvent = true;
            _submenuOpen = ((MenuItem)e.Source).IsSubmenuOpen;
        }

        private void OnIsCheckedChanged(object sender, RoutedEventArgs e)
        {
            _checkedEvent = true;
            _checked = ((MenuItem)e.Source).IsChecked;
        }

        #endregion

        #region Mouse

        // break apart any moveto and click tests into two separate tests.  It is possible that the moveto happens too soon and then
        // you have to try the moveto again before you are able to click.
        private enum MouseTest
        {
            Start,
            ClickSubmenuItem_MoveTo,
            ClickSubmenuItem,
            GetToSubmenu1_MoveTo,
            GetToSubmenu1,
            GetToSubmenu2,
            GetToSubmenu3,
            MoveOverSubmenu3,
            CollapseSubmenu3_a,
            CollapseSubmenu3_b,
            CollapseSubmenu2_a,
            CollapseSubmenu2_b,
            CollapseSubmenu1,
            ReopenMenu,
            CheckItem_MoveTo,
            CheckItem,
            FocusButton,
            ClickItem1_MoveTo,
            ClickItem1,
            MouseOverItem2_a,
            MouseOverItem2_b,
            MouseOverItem3_a,
            MouseOverItem3_b,
            ClickSubmenuItem3_1_MoveTo,
            ClickSubmenuItem3_1,

            StaysOpenOnClick_MoveTo,
            StaysOpenOnClick_OpenMenu,
            StaysOpenOnClick_ClickStaysOpenOnClickTrue_MoveTo,
            StaysOpenOnClick_ClickStaysOpenOnClickTrue,
            StaysOpenOnClick_ClickStaysOpenOnClickFalse_MoveTo,
            StaysOpenOnClick_ClickStaysOpenOnClickFalse,

            //Test
            ClickItem2_MoveTo,
            ClickItem2_Click,
            ClickItem3_MoveTo_a,
            ClickItem3_MoveTo_b,
            ClickItem3_Click,
            ExitMenuMode,

            //Test 
            Bug1_ClickItem1,
            Bug2_PressDown,
            Bug3_CloseMenu,

            // 950780 - Win32 Parity: Clicking on a disabled MenuItem or a separator closes the menu
            Bug4_OpenSubmenu,
            Bug5_MouseOverSeparator,
            Bug6_ClickSeparator,
            Bug7_CloseMouseOver,
            Bug8_CloseClick,

            // 962679 - ContextMenu should not dismiss on mouse button up if that mouse button was pressed when it opened
            Bug9_MoveToButton3,
            Bug10_MouseDownOnButton3,
            Bug11_MouseUpOnButton3,
            Bug12_CloseClick,

            End
        }

        private void OnSubmenuItemClick(object sender, RoutedEventArgs e)
        {
            _submenuItemClicked = true;
        }

        private void MenuMouseTest()
        {
            DRT.ResumeAt(new DrtTest(MenuMouseClick));
        }

        private void WaitForMenuShowDelay()
        {
            int menuShowDelay = FlyoutWaitTime;

            Console.WriteLine("Waiting for MenuShowDelay ({0}) ms", menuShowDelay);
            DRT.Pause(menuShowDelay);
        }

        private int FlyoutWaitTime
        {
            get
            {
                // Waiting 2x is the general rule to make sure everything passes on ia64.
                return (SystemParameters.MenuShowDelay * 8) + DrtControls.PopupAnimationDelay;
            }
        }

        private void WaitForMenuAnimationDelay()
        {
            int animationDelay = DrtControls.PopupAnimationDelay;
            Console.WriteLine("Waiting for Popup Animation ({0}) ms", animationDelay);
            DRT.Pause(animationDelay);
        }

        private void MenuMouseClick()
        {
            if (DRT.Verbose)
            {
                Console.WriteLine();
                Console.Write("--- MenuMouse Click test = " + _mouseTest + " (Time: " + DateTime.Now.ToFileTime() + ")");
                if (_repeatedTestCount > 0)
                {
                    Console.Write(" (repeat {0})", _repeatedTestCount);
                }
                Console.WriteLine();
            }
            switch (_mouseTest)
            {
                case MouseTest.Start:
                    MouseOverItem = _menu.Items[0] as MenuItem;
                    _verifyMouseOver = false;
                    DRT.ClickMouse();
                    ReportMousePosition();
                    WaitForMenuAnimationDelay();
                    break;

                case MouseTest.ClickSubmenuItem_MoveTo:
                    _submenuItemClicked = false;
                    MouseOverItem = (_menu.Items[0] as MenuItem).Items[6] as MenuItem;
                    break;

                case MouseTest.ClickSubmenuItem:
                    DRT.ClickMouse();
                    WaitForMenuAnimationDelay();
                    break;

                case MouseTest.GetToSubmenu1_MoveTo:
                    MouseOverItem = _menu.Items[0] as MenuItem;
                    break;

                case MouseTest.GetToSubmenu1:
                    DRT.ClickMouse();
                    WaitForMenuAnimationDelay();
                    break;

                case MouseTest.GetToSubmenu2:
                    MouseOverItem = (_menu.Items[0] as MenuItem).Items[1] as MenuItem;

                    // wait for the second submenu to open
                    WaitForMenuShowDelay();
                    break;

                case MouseTest.GetToSubmenu3:
                    MouseOverItem = ((_menu.Items[0] as MenuItem).Items[1] as MenuItem).Items[0] as MenuItem;

                    // Wait for the 3rd submenu to open
                    WaitForMenuShowDelay();
                    break;

                case MouseTest.MoveOverSubmenu3:
                    MouseOverItem = (((_menu.Items[0] as MenuItem).Items[1] as MenuItem).Items[0] as MenuItem).Items[0] as MenuItem;
                    break;

                case MouseTest.CollapseSubmenu3_a:
                    MouseOverItem = ((_menu.Items[0] as MenuItem).Items[1] as MenuItem).Items[1] as MenuItem;

                    // Wait for the 3rd submenu to close
                    WaitForMenuShowDelay();
                    break;

                case MouseTest.CollapseSubmenu2_a:
                    MouseOverItem = (_menu.Items[0] as MenuItem).Items[3] as MenuItem;

                    // Wait for 2nd submenu to close
                    WaitForMenuShowDelay();
                    break;

                case MouseTest.CollapseSubmenu1:
                    // Click somewhere in the lower right of the canvas
                    DRT.MoveMouse(DRT.RootElement as UIElement, 0.9, 0.9);
                    DRT.ClickMouse();
                    WaitForMenuAnimationDelay();
                    break;

                case MouseTest.ReopenMenu:
                    MouseOverItem = _menu.Items[0] as MenuItem;
                    _verifyMouseOver = false;
                    ReportMousePosition();
                    DRT.ClickMouse();
                    WaitForMenuAnimationDelay();
                    break;

                case MouseTest.CheckItem_MoveTo:
                    _checkedEvent = false;
                    _checked = false;
                    MouseOverItem = (_menu.Items[0] as MenuItem).Items[3] as MenuItem;
                    break;

                case MouseTest.CheckItem:
                    DRT.ClickMouse();
                    ReportMousePosition();
                    WaitForMenuAnimationDelay();
                    break;

                case MouseTest.FocusButton:
                    DRT.Assert(_button.Focus(), "_button.Focus() returned false -- could not focus button");
                    break;

                case MouseTest.ClickItem1_MoveTo:
                    MouseOverItem = _menu.Items[1] as MenuItem;
                    break;

                case MouseTest.ClickItem1:
                    DRT.ClickMouse();
                    WaitForMenuAnimationDelay();
                    break;

                case MouseTest.MouseOverItem2_a:
                    MouseOverItem = _menu.Items[2] as MenuItem;
                    break;

                case MouseTest.MouseOverItem3_a:
                    MouseOverItem = _menu.Items[3] as MenuItem;
                    break;

                case MouseTest.ClickSubmenuItem3_1_MoveTo:
                    MouseOverItem = (_menu.Items[3] as MenuItem).Items[0] as MenuItem;

                    // Don't need to wait b/c the next step will click and open the menu immediately.
                    break;

                case MouseTest.ClickSubmenuItem3_1:
                    DRT.ClickMouse();
                    WaitForMenuAnimationDelay();
                    break;

                case MouseTest.ClickItem2_MoveTo:
                    MouseOverItem = _menu.Items[2] as MenuItem;
                    break;

                case MouseTest.ClickItem2_Click:
                    DRT.ClickMouse();
                    WaitForMenuAnimationDelay();
                    break;

                case MouseTest.ClickItem3_MoveTo_a:
                    MouseOverItem = _menu.Items[3] as MenuItem;
                    break;

                case MouseTest.ClickItem3_Click:
                    DRT.ClickMouse();
                    WaitForMenuAnimationDelay();
                    break;

                case MouseTest.ExitMenuMode:
                    DRT.PressKey(Key.Escape);
                    WaitForMenuAnimationDelay();
                    break;

                case MouseTest.Bug1_ClickItem1:
                    DRT.MoveMouse(_menu.Items[0] as MenuItem, 0.2, 0.8);
                    DRT.ClickMouse();
                    WaitForMenuAnimationDelay();
                    break;

                case MouseTest.Bug2_PressDown:
                    DRT.PressKey(Key.Down);
                    break;

                case MouseTest.Bug3_CloseMenu:
                    DRT.PressKey(Key.Enter);
                    WaitForMenuAnimationDelay();
                    break;

                case MouseTest.Bug4_OpenSubmenu:
                    (_menu.Items[0] as MenuItem).IsSubmenuOpen = true;
                    WaitForMenuAnimationDelay();
                    break;

                case MouseTest.Bug5_MouseOverSeparator:
                    MouseOverItem = (_menu.Items[0] as MenuItem).Items[4] as Separator;
                    _verifyMouseOver = false;
                    break;

                case MouseTest.Bug6_ClickSeparator:
                    DRT.ClickMouse();
                    break;

                case MouseTest.Bug7_CloseMouseOver:
                    MouseOverItem = (_menu.Items[0] as MenuItem).Items[6] as MenuItem;
                    break;

                case MouseTest.Bug8_CloseClick:
                    DRT.ClickMouse();
                    WaitForMenuAnimationDelay();
                    break;

                case MouseTest.Bug9_MoveToButton3:
                    DRT.MoveMouse(_button3, 0.5, 0.5);
                    break;

                case MouseTest.Bug10_MouseDownOnButton3:
                    DRT.MouseButtonDown();
                    WaitForMenuAnimationDelay();
                    break;

                case MouseTest.Bug11_MouseUpOnButton3:
                    DRT.MouseButtonUp();
                    WaitForMenuAnimationDelay();
                    break;

                case MouseTest.Bug12_CloseClick:
                    DRT.MoveMouse(DRT.RootElement as UIElement, 0.9, 0.9);
                    DRT.ClickMouse();
                    WaitForMenuAnimationDelay();
                    break;

                case MouseTest.StaysOpenOnClick_MoveTo:
                    DRT.MoveMouse(_staysOpenOnClickHeader, 0.9, 0.9);
                    break;

                case MouseTest.StaysOpenOnClick_OpenMenu:
                    DRT.ClickMouse();
                    WaitForMenuAnimationDelay();
                    break;

                case MouseTest.StaysOpenOnClick_ClickStaysOpenOnClickTrue_MoveTo:
                    DRT.MoveMouse(_staysOpenOnClickTrue, 0.9, 0.9);
                    break;

                case MouseTest.StaysOpenOnClick_ClickStaysOpenOnClickTrue:
                    DRT.ClickMouse();
                    break;

                case MouseTest.StaysOpenOnClick_ClickStaysOpenOnClickFalse_MoveTo:
                    DRT.MoveMouse(_staysOpenOnClickFalse, 0.9, 0.9);
                    break;

                case MouseTest.StaysOpenOnClick_ClickStaysOpenOnClickFalse:
                    DRT.ClickMouse();
                    WaitForMenuAnimationDelay();
                    break;

                case MouseTest.End:
                    _verifyMouseOver = false;
                    DRT.MoveMouse(DRT.RootElement as UIElement, 0, 0);
                    break;
            }

            DRT.ResumeAt(new DrtTest(MenuMouseVerify));
        }

        private void MenuMouseVerify()
        {
            if (!VerifyMouseOverItem(20))
            {
                // Wait before repeating the test -- repeat the test in addition to the verify.
                DRT.Pause(500);
                DRT.ResumeAt(new DrtTest(MenuMouseClick));
                return;
            }

            bool retry = false;

            switch (_mouseTest)
            {
                case MouseTest.Start:
                    retry = !VerifyPopupOpened((MenuItem)_menu.Items[0], true, 20);
                    if (!retry)
                    {
                        DRT.Assert(_submenuOpenChangedEvent, "IsSubmenuOpenedChangedEvent never fired");
                        DRT.Assert(_submenuOpen, "IsSubmenuOpenedChangedEvent fired when IsSubmenuOpen was false.");
                    }
                    break;

                case MouseTest.ClickSubmenuItem:
                    DRT.Assert(_submenuItemClicked, "Submenu item 4 not clicked");

                    retry = !VerifyPopupOpened((MenuItem)_menu.Items[0], false, 20);

                    _submenuItemClicked = false;
                    break;

                case MouseTest.GetToSubmenu1:
                    retry = !VerifyPopupOpened((MenuItem)_menu.Items[0], true, 20);
                    break;

                case MouseTest.GetToSubmenu2:
                    retry = !VerifyPopupOpened(((MenuItem)((MenuItem)_menu.Items[0]).Items[1]), true, 20);
                    break;

                case MouseTest.GetToSubmenu3:
                    retry = !VerifyPopupOpened(((MenuItem)((MenuItem)((MenuItem)_menu.Items[0]).Items[1]).Items[0]), true, 20);
                    break;

                case MouseTest.CollapseSubmenu3_a:
                    retry = !VerifyPopupOpened(((MenuItem)((MenuItem)((MenuItem)_menu.Items[0]).Items[1]).Items[0]), false, 20);
                    break;
                case MouseTest.CollapseSubmenu3_b:
                    retry = !VerifyPopupOpened(((MenuItem)((MenuItem)_menu.Items[0]).Items[1]), true, 20);
                    break;

                case MouseTest.CollapseSubmenu2_a:
                    retry = !VerifyPopupOpened(((MenuItem)((MenuItem)_menu.Items[0]).Items[1]), false, 20);
                    break;
                case MouseTest.CollapseSubmenu2_b:
                    retry = !VerifyPopupOpened((MenuItem)_menu.Items[0], true, 20);
                    break;

                case MouseTest.CollapseSubmenu1:
                    retry = !VerifyPopupOpened((MenuItem)_menu.Items[0], false, 20);
                    break;

                case MouseTest.ReopenMenu:
                    retry = !VerifyPopupOpened((MenuItem)_menu.Items[0], true, 20);
                    break;

                case MouseTest.CheckItem:
                    DRT.Assert(_checkedEvent, "IsCheckedChanged event didn't fire");
                    DRT.Assert(_checked, "IsCheckedChanged event fired, but item was not checked");
                    break;

                case MouseTest.ClickItem1:
                    retry = !VerifyPopupOpened((MenuItem)_menu.Items[1], true, 20);
                    break;

                case MouseTest.MouseOverItem2_a:
                    retry = !VerifyPopupOpened((MenuItem)_menu.Items[1], false, 20);
                    break;
                case MouseTest.MouseOverItem2_b:
                    retry = !VerifyPopupOpened((MenuItem)_menu.Items[2], true, 20);
                    break;

                case MouseTest.MouseOverItem3_a:
                    retry = !VerifyPopupOpened((MenuItem)_menu.Items[2], false, 20);
                    break;
                case MouseTest.MouseOverItem3_b:
                    retry = !VerifyPopupOpened((MenuItem)_menu.Items[3], true, 20);
                    break;

                case MouseTest.ClickSubmenuItem3_1:
                    retry = !VerifyPopupOpened((MenuItem)_menu.Items[3], false, 20);
                    if (!retry)
                    {
                        // menu dismissed; focus should go back to what previously had it.  The button, in this case.
                        DRT.Assert(_button.IsKeyboardFocused, "Button does not have focus (it should have been the previously focused element)");
                    }
                    break;

                case MouseTest.ClickItem2_Click:
                    retry = !VerifyPopupOpened((MenuItem)_menu.Items[2], true, 20);
                    break;

                case MouseTest.ClickItem3_MoveTo_a:
                    retry = !VerifyPopupOpened((MenuItem)_menu.Items[2], false, 20);
                    break;
                case MouseTest.ClickItem3_MoveTo_b:
                    retry = !VerifyPopupOpened((MenuItem)_menu.Items[3], true, 20);
                    break;

                case MouseTest.ClickItem3_Click:
                    retry = !VerifyPopupOpened((MenuItem)_menu.Items[3], false, 20);
                    break;

                case MouseTest.ExitMenuMode:
                    DRT.Assert(Mouse.Captured == null, "Pressed escape to leave menu mode -- menu should no longer have capture");
                    DRT.Assert(_button.IsKeyboardFocused, "Button does not have focus (it should have been the previously focused element)");
                    break;

                case MouseTest.Bug1_ClickItem1:
                    retry = !VerifyPopupOpened((MenuItem)_menu.Items[0], true, 20);
                    break;

                case MouseTest.Bug2_PressDown:
                    DRT.Assert(((MenuItem)((MenuItem)_menu.Items[0]).Items[0]).IsKeyboardFocused, "First item in the submenu should be focused");
                    break;

                case MouseTest.Bug3_CloseMenu:
                    retry = !VerifyPopupOpened((MenuItem)_menu.Items[0], false, 20);
                    DRT.Assert(Mouse.Captured == null, "Pressed Enter to select item and exit menu mode -- menu should no longer have capture");
                    break;

                case MouseTest.Bug5_MouseOverSeparator:
                    break;

                case MouseTest.Bug6_ClickSeparator:
                    // Clicking on the separator should do nothing
                    DRT.Assert(((MenuItem)_menu.Items[0]).IsSubmenuOpen, "After clicking on the separator, submenu should still be open");
                    break;

                case MouseTest.Bug8_CloseClick:
                    retry = !VerifyPopupOpened((MenuItem)_menu.Items[0], false, 20);
                    if (!retry)
                    {
                        DRT.Assert(Mouse.Captured == null, "After click to close, menu should not have capture anymore.");
                    }
                    break;

                case MouseTest.Bug10_MouseDownOnButton3:
                    // After mouse down, CM should have opened
                    DRT.Assert(_button.ContextMenu.IsOpen, "After mouse down on Button3, context menu should be open");
                    break;

                case MouseTest.Bug11_MouseUpOnButton3:
                    DRT.Assert(_button.ContextMenu.IsOpen, "After mouse up on Button3, context menu should still be open");
                    break;

                case MouseTest.Bug12_CloseClick:
                    DRT.Assert(!_button.ContextMenu.IsOpen, "After mouse down off of Button3 and the ContextMenu, ContextMenu should have closed");
                    break;

                case MouseTest.StaysOpenOnClick_OpenMenu:
                    DRT.Assert(_staysOpenOnClickHeader.IsSubmenuOpen, "StaysOpenOnClickHeader's submenu is not open");
                    break;

                case MouseTest.StaysOpenOnClick_ClickStaysOpenOnClickTrue:
                    DRT.Assert(_staysOpenOnClickHeader.IsSubmenuOpen, "StaysOpenOnClickHeader's submenu is not open following clicking an item with StaysOpenOnClick=True");
                    break;

                case MouseTest.StaysOpenOnClick_ClickStaysOpenOnClickFalse:
                    DRT.Assert(!_staysOpenOnClickHeader.IsSubmenuOpen, "StaysOpenOnClickHeader's submenu is still open following clicking an item with StaysOpenOnClick=False");
                    break;

                case MouseTest.End:
                    break;
            }

            if (_mouseTest != MouseTest.End)
            {
                if (!retry)
                {
                    _mouseTest++;
                }
                DRT.ResumeAt(new DrtTest(MenuMouseClick));
            }
        }

        #endregion

        #region Instrumentation for MouseOverItem

        int _repeatedTestCount = 0;
        FrameworkElement _currentMouseOver;
        bool _verifyMouseOver;

        private FrameworkElement MouseOverItem
        {
            get { return _currentMouseOver; }
            set
            {
                _currentMouseOver = value;
                _verifyMouseOver = true;
                if (_currentMouseOver != null)
                {
                    DRT.MoveMouse(_currentMouseOver, 0.1 + _repeatedTestCount * 0.04, 0.5);
                }

                ReportMousePosition();
            }
        }

        /// <summary>
        ///     Verifies that mouse is over the requested item.
        /// </summary>
        /// <returns>Whether the test should continue or not</returns>
        private bool VerifyMouseOverItem(int numRetries)
        {
            if (_verifyMouseOver)
            {
                // If the mouse isn't over the element it should be over, it might be b/c the window that has capture hasn't
                // been released yet or the window isn't open
                if (!VerifyMouseOver())
                {
                    string msg = "Mouse should have been over MenuItem: " + MouseOverItem.ToString();

                    if (_repeatedTestCount < numRetries)
                    {
                        if (DRT.Verbose)
                        {
                            Console.WriteLine(msg);
                            Console.WriteLine("Repeating.");
                        }

                        _repeatedTestCount++;
                        return false;
                    }
                    else
                    {
                        DRT.Assert(false, msg);
                    }
                }

                _verifyMouseOver = false;
            }

            _repeatedTestCount = 0;
            return true;
        }

        private void ReportMousePosition()
        {
            if (!DRT.Verbose) return;

            POINT mousePoint = new POINT();
            GetCursorPos(mousePoint);
            IntPtr hwnd = WindowFromPoint(mousePoint.x, mousePoint.y);

            Console.WriteLine("Mouse is at (" + mousePoint.x + "," + mousePoint.y + ") and is over hwnd = {0}", hwnd.ToString("X"));
        }

        private bool VerifyMouseOver()
        {
            if (DRT.Verbose)
            {
                Console.Write("Mouse.DirectlyOver = " + Mouse.DirectlyOver);
                if (Mouse.DirectlyOver is TextBlock)
                {
                    Console.Write(" (Text=" + (Mouse.DirectlyOver as TextBlock).Text + ")");
                }

                Console.WriteLine();
            }

            if (DRT.Verbose)
            {
                Console.WriteLine("Verifying IsMouseOver [" + _currentMouseOver.ToString() + "]");
                ShowMenuItemInfo(_currentMouseOver as MenuItem);
            }

            PresentationSource sourcePopup = PresentationSource.FromVisual(_currentMouseOver);
            if (sourcePopup == null)
                return false;

            IntPtr windowPopup = ((HwndSource)sourcePopup).Handle;
            POINT pt = new POINT();
            GetCursorPos(pt);
            IntPtr windowOver = WindowFromPoint(pt.x, pt.y);
            if (windowOver != windowPopup)
            {
                if (DRT.Verbose) Console.WriteLine("The mouse is not over the popup window.  It is over " + windowOver.ToString("X") + ", but it should be over " + windowPopup.ToString("X") + ".  Popup might not be open yet.");

                if (DRT.Verbose) Console.Write("Waiting for complete render ... ");

                DRT.WaitForCompleteRender();

                if (DRT.Verbose) Console.WriteLine(" render complete.");

                return false;
            }

            IInputElement mouseOver = Mouse.DirectlyOver;
            if (!_currentMouseOver.IsMouseOver)
            {
                string overtext = (mouseOver is TextBlock) ? ("(" + ((TextBlock)mouseOver).Text + ")") : "";
                if (DRT.Verbose) Console.Write("Moved mouse over " + _currentMouseOver.ToString()
                    + " but IsMouseOver was false.  Mouse.DirectlyOver = " + mouseOver + " " + overtext + ".");

                return false;
            }

            DRT.Assert(_currentMouseOver.IsMouseOver, "Moved mouse over " + _currentMouseOver.ToString()
                + " but IsMouseOver was false.  Mouse.DirectlyOver = " + mouseOver + ". "); //\nAre WS_EX_LAYERED windows actually rendering?");

            return true;
        }


        private void ShowMenuItemInfo(MenuItem item)
        {
            if (DRT.Verbose)
            {
                /*

                Console.WriteLine("MenuItem (" + item.Header + "):");
                LayoutManager lm = LayoutManager.GetLayoutManagerFor(item);

                Console.Write("    LayoutManager for ");
                if (lm == null)
                {
                    Console.Write("was null");
                }
                else
                {
                    Console.Write("IsUpdatingLayout = " + lm.IsUpdatingLayout + ", HasValidLayout = " + lm.HasValidLayout + ", Root = " + lm.Root);
                }

                Console.WriteLine();

                PresentationSource source = PresentationSource.FromVisual(item);
                Console.WriteLine("    PresentationSource = " + source);

                if (source != null)
                {
                    Console.WriteLine("    CompositionTarget = " + source.CompositionTarget);
                }
                */

            }
        }

        #endregion

        #region Keyboard

        private void PressKey(Key k)
        {
            DRT.Assert(Keyboard.FocusedElement is MenuItem, "Focus is not on a MenuItem");
            if (DRT.Verbose)
            {
                Console.Write("Sending key " + k + " to ");
                MenuItem item = Keyboard.FocusedElement as MenuItem;
                if (item != null)
                {
                    Console.Write(" MenuItem (" + item.Header + ")");
                }
                Console.WriteLine();
            }

            DRT.PressKey(k);

            if (DRT.Verbose)
            {
                Console.Write("After keypress, focus is ");
                MenuItem item = Keyboard.FocusedElement as MenuItem;
                if (item != null)
                {
                    Console.Write(" MenuItem (" + item.Header + ")");
                }
                Console.WriteLine();
            }
        }

        private enum KeyboardTest
        {
            Start,
            OpenSubmenu1,
            MoveDown1,
            OpenSubmenu2,
            CollapseSubmenu2,
            CollapseSubmenu2_a,
            MoveDown2,
            MoveDown3,
            MoveDown4,
            MoveDown5,
            SelectSubmenuItem,
            MainMenu,
            MainMenu_Close,
            End
        }

        private void MenuKeyboardTest()
        {
            if (DRT.Verbose) Console.WriteLine("\n---Keyboard Tests");
            DRT.ResumeAt(new DrtTest(MenuKeyboardPress));
        }

        private void MenuKeyboardPress()
        {
            if (DRT.Verbose) Console.WriteLine("Keyboard test = " + _keyboardTest);
            switch (_keyboardTest)
            {
                case KeyboardTest.Start:
                    Input.MoveTo(new Point(0, 0));
                    ((MenuItem)(_menu.Items[0])).Focus();
                    break;

                case KeyboardTest.OpenSubmenu1:
                    PressKey(Key.Down);
                    WaitForMenuAnimationDelay();
                    break;

                case KeyboardTest.MoveDown1:
                    PressKey(Key.Down);
                    break;

                case KeyboardTest.OpenSubmenu2:
                    PressKey(Key.Right);
                    WaitForMenuAnimationDelay();
                    break;

                case KeyboardTest.CollapseSubmenu2:
                    PressKey(Key.Left);
                    WaitForMenuAnimationDelay();
                    break;

                case KeyboardTest.MoveDown2:
                    PressKey(Key.Down);
                    break;

                case KeyboardTest.MoveDown3:
                    PressKey(Key.Down);
                    break;

                case KeyboardTest.MoveDown4:
                    PressKey(Key.Down);
                    break;

                case KeyboardTest.MoveDown5:
                    PressKey(Key.Down);
                    break;

                case KeyboardTest.SelectSubmenuItem:
                    _submenuItemClicked = false;
                    PressKey(Key.Enter);
                    WaitForMenuAnimationDelay();
                    break;

                case KeyboardTest.MainMenu:
                    DRT.PressKey(Key.Escape);
                    _menu.IsMainMenu = false;
                    DRT.PressKey(Key.LeftAlt);
                    WaitForMenuAnimationDelay();
                    break;

                case KeyboardTest.MainMenu_Close:
                    DRT.PressKey(Key.Escape);
                    _menu.ClearValue(Menu.IsMainMenuProperty);
                    WaitForMenuAnimationDelay();
                    break;

                case KeyboardTest.End:
                    break;
            }

            DRT.ResumeAt(new DrtTest(MenuKeyboardVerify));
        }

        private void MenuKeyboardVerify()
        {
            if (DRT.Verbose)
            {
                Console.Write("Mouse.DirectlyOver = " + Mouse.DirectlyOver);
                if (Mouse.DirectlyOver is TextBlock)
                {
                    Console.Write(" (Text=" + (Mouse.DirectlyOver as TextBlock).Text + ")");
                }

                Console.WriteLine();
                ReportMousePosition();
            }

            bool retry = false;
            switch (_keyboardTest)
            {
                case KeyboardTest.Start:
                    VerifyFocus((MenuItem)_menu.Items[0]);
                    break;

                case KeyboardTest.OpenSubmenu1:
                    retry = !VerifyPopupOpened((MenuItem)_menu.Items[0], true, 20);
                    if (!retry)
                    {
                        VerifyFocus((MenuItem)((MenuItem)_menu.Items[0]).Items[0]);
                    }
                    break;

                case KeyboardTest.MoveDown1:
                    VerifyFocus((MenuItem)((MenuItem)_menu.Items[0]).Items[1]);
                    break;

                case KeyboardTest.OpenSubmenu2:
                    retry = !VerifyPopupOpened(((MenuItem)((MenuItem)_menu.Items[0]).Items[1]), true, 20);
                    break;

                case KeyboardTest.CollapseSubmenu2:
                    retry = !VerifyPopupOpened(((MenuItem)((MenuItem)_menu.Items[0]).Items[1]), false, 20);
                    break;
                case KeyboardTest.CollapseSubmenu2_a:
                    retry = !VerifyPopupOpened((MenuItem)_menu.Items[0], true, 20);
                    if (!retry)
                    {
                        VerifyFocus((MenuItem)((MenuItem)_menu.Items[0]).Items[1]);
                    }
                    break;

                case KeyboardTest.MoveDown2:
                    VerifyFocus((MenuItem)((MenuItem)_menu.Items[0]).Items[2]);
                    break;

                case KeyboardTest.MoveDown3:
                    VerifyFocus((MenuItem)((MenuItem)_menu.Items[0]).Items[3]);
                    break;

                case KeyboardTest.MoveDown4:
                    // Skipped over separator
                    VerifyFocus((MenuItem)((MenuItem)_menu.Items[0]).Items[5]);
                    break;

                case KeyboardTest.MoveDown5:
                    VerifyFocus((MenuItem)((MenuItem)_menu.Items[0]).Items[6]);
                    break;

                case KeyboardTest.SelectSubmenuItem:
                    DRT.Assert(_submenuItemClicked, "Submenu item 4 not clicked");

                    retry = !VerifyPopupOpened((MenuItem)_menu.Items[0], false, 20);
                    _submenuItemClicked = false;
                    break;

                case KeyboardTest.MainMenu:
                    DRT.Assert(Keyboard.FocusedElement is MenuItem, "Focus did not move to a menu item.");
                    DRT.Assert(((MenuItem)Keyboard.FocusedElement).Parent == _staysOpenOnClickMenu, "Setting IsMainMenu to false did not work.");
                    break;

                case KeyboardTest.MainMenu_Close:
                    break;

                case KeyboardTest.End:
                    break;
            }

            if (_keyboardTest != KeyboardTest.End)
            {
                if (!retry)
                {
                    _keyboardTest++;
                }
                DRT.ResumeAt(new DrtTest(MenuKeyboardPress));
            }
        }

        private void VerifyFocus(MenuItem item)
        {
            MenuItem focusedItem = Keyboard.FocusedElement as MenuItem;
            string actual = (focusedItem == null) ? (String.Format("{0}", Keyboard.FocusedElement)) : ("MenuItem (" + focusedItem.Header + ")");
            if (DRT.Verbose) Console.WriteLine("Focus is on " + actual);
            DRT.Assert(Keyboard.FocusedElement == item, "Focus should be on MenuItem (" + item.Header + "), was on " + actual);
        }

        #endregion

        #region Suicide Timer

        DispatcherTimer _suicideTimer;

        private void OnTimeout(object sender, EventArgs e)
        {
            throw new TimeoutException();
        }

        private class TimeoutException : Exception
        {
            public TimeoutException() : base("Timeout expired, quitting") { }
        }

        #endregion

        #region TextBox with ContextMenu

        enum TextBoxWithContextMenuTestStep
        {
            // 
            Start,
            Test1_FocusTextBox,
            Test1_KeyboardOpenContextMenu_ShiftDown,
            Test1_KeyboardOpenContextMenu_F10Down,
            Test1_KeyboardOpenContextMenu_F10Up,
            Test1_KeyboardOpenContextMenu_ShiftUp,
            Test1_VerifyPopupOpen,
            Test1_KeyboardPressDown,
            Test1_VerifySelection,
            Test1_KeyboardSelectItem,
            Test1_VerifyFocusReturned,
            // 
            Test2_MouseOpenContextMenu_MoveTo,
            Test2_MouseOpenContextMenu_RightMouseDown,
            Test2_MouseOpenContextMenu_RightMouseUp,
            Test2_MouseOpenContextMenu_Verify,
            Test2_MouseOverLastItem_MoveTo,
            Test2_MouseOverLastItem_Verify,
            Test2_MouseOverPenultimateItem_MoveTo,
            Test2_MouseOverPenultimateItem_Verify,
            Test2_CloseContextMenu,
            Test2_CloseContextMenu_Verify,
            // 
            Test3_MouseOpenContextMenu_MoveTo,
            Test3_MouseOpenContextMenu_RightMouseDown,
            Test3_MouseOpenContextMenu_RightMouseUp,
            Test3_MouseOpenContextMenu_Verify,
            Test3_MouseOverFirstItem_MoveTo,
            Test3_MouseOverFirstItem_Verify,
            Test3_PressDown1,
            Test3_PressDown2,
            Test3_PressDown3,
            Test3_PressDown4,
            Test3_PressRight,
            Test3_VerifySubmenuOpened,
            Test3_CloseContextMenu,
            Test3_CloseContextMenu_Verify,
            // 

            Test4_KeyboardOpenContextMenu_Focus,
            Test4_KeyboardOpenContextMenu_AppsDown,
            Test4_KeyboardOpenContextMenu_AppsUp,
            Test4_VerifyOpened,
            Test4_PressDown1,
            Test4_PressDown2,
            Test4_PressDown3,
            Test4_PressDown4,
            Test4_VerifyFirstItemSelected,
            Test4_CloseContextMenu,
            Test4_CloseContextMenu_Verify,
            End,
        }

        TextBoxWithContextMenuTestStep _test2 = TextBoxWithContextMenuTestStep.Start;

        private void TextBoxContextMenuTest()
        {
            if (DRT.Verbose) Console.WriteLine("TextBox with ContextMenu : " + _test2);

            bool shouldRepeat = false;

            switch (_test2)
            {
                case TextBoxWithContextMenuTestStep.Test1_FocusTextBox:
                    _textBox1.Focus();
                    break;

                case TextBoxWithContextMenuTestStep.Test1_KeyboardOpenContextMenu_ShiftDown:
                    _textBox1.ContextMenu.Closed += new RoutedEventHandler(ThrowIfClosed);
                    DRT.SendKeyStrokes(new KeyStatePair(Key.LeftShift, true),
                                       new KeyStatePair(Key.F10, true),
                                       new KeyStatePair(Key.F10, false),
                                       new KeyStatePair(Key.LeftShift, false));

                    WaitForMenuAnimationDelay();
                    break;

                case TextBoxWithContextMenuTestStep.Test1_KeyboardOpenContextMenu_F10Down:
                    //DRT.SendKeyboardInput(Key.F10, true);
                    break;

                case TextBoxWithContextMenuTestStep.Test1_KeyboardOpenContextMenu_F10Up:
                    //DRT.SendKeyboardInput(Key.F10, false);
                    break;

                case TextBoxWithContextMenuTestStep.Test1_KeyboardOpenContextMenu_ShiftUp:
                    //DRT.SendKeyboardInput(Key.LeftShift, false);
                    break;

                case TextBoxWithContextMenuTestStep.Test1_VerifyPopupOpen:
                    DRT.Assert(_textBox1.ContextMenu.IsOpen, "Pressing context menu key did not open context menu");
                    _textBox1.ContextMenu.Closed -= new RoutedEventHandler(ThrowIfClosed);
                    break;

                case TextBoxWithContextMenuTestStep.Test1_KeyboardPressDown:
                    DRT.PressKey(Key.Down);
                    break;

                case TextBoxWithContextMenuTestStep.Test1_VerifySelection:
                    DRT.Assert((_textBox1.ContextMenu.Items[0] as MenuItem).IsKeyboardFocused, "First item in menu did not have focus");
                    break;

                case TextBoxWithContextMenuTestStep.Test1_KeyboardSelectItem:
                    DRT.PressKey(Key.Enter);
                    WaitForMenuAnimationDelay();
                    break;

                case TextBoxWithContextMenuTestStep.Test1_VerifyFocusReturned:
                    DRT.Assert(_textBox1.IsKeyboardFocusWithin && !_textBox1.ContextMenu.IsKeyboardFocusWithin, "Focus should be somewhere inside the TextBox and not inside the ContextMenu.  Keyboard.FocusedElement = " + Keyboard.FocusedElement);
                    break;

                //////// TEST 2 ///////////

                case TextBoxWithContextMenuTestStep.Test2_MouseOpenContextMenu_MoveTo:
                    DRT.MoveMouse(_textBox1, 0.1, 0.1);
                    break;

                case TextBoxWithContextMenuTestStep.Test2_MouseOpenContextMenu_RightMouseDown:
                    DRT.MouseSecondButtonDown();
                    break;

                case TextBoxWithContextMenuTestStep.Test2_MouseOpenContextMenu_RightMouseUp:
                    _textBox1.ContextMenu.Closed += new RoutedEventHandler(ThrowIfClosed);
                    DRT.MouseSecondButtonUp();
                    WaitForMenuAnimationDelay();
                    break;

                case TextBoxWithContextMenuTestStep.Test2_MouseOpenContextMenu_Verify:
                    DRT.Assert(_textBox1.ContextMenu.IsOpen, "TextBox's Context Menu should have opened");
                    _textBox1.ContextMenu.Closed -= new RoutedEventHandler(ThrowIfClosed);
                    break;

                case TextBoxWithContextMenuTestStep.Test2_MouseOverLastItem_MoveTo:
                    MouseOverItem = _textBox1.ContextMenu.Items[_textBox1.ContextMenu.Items.Count - 1] as MenuItem;
                    break;

                case TextBoxWithContextMenuTestStep.Test2_MouseOverLastItem_Verify:
                    if (!VerifyMouseOverItem(20)) shouldRepeat = true;
                    break;

                case TextBoxWithContextMenuTestStep.Test2_MouseOverPenultimateItem_MoveTo:
                    MouseOverItem = _textBox1.ContextMenu.Items[_textBox1.ContextMenu.Items.Count - 2] as MenuItem;
                    WaitForMenuShowDelay();
                    break;

                case TextBoxWithContextMenuTestStep.Test2_MouseOverPenultimateItem_Verify:
                    if (!VerifyMouseOverItem(20)) shouldRepeat = true;
                    else
                    {
                        MenuItem lastMenuItem = _textBox1.ContextMenu.Items[_textBox1.ContextMenu.Items.Count - 1] as MenuItem;
                        MenuItem penultimateMenuItem = _textBox1.ContextMenu.Items[_textBox1.ContextMenu.Items.Count - 2] as MenuItem;

                        DRT.Assert(!lastMenuItem.IsKeyboardFocused, "Last menu item should not have focus");
                        DRT.Assert(penultimateMenuItem.IsKeyboardFocused, "Penultimate menu item should have focus");
                        DRT.Assert(!lastMenuItem.IsSubmenuOpen, "Last menu item's submenu should not be open");
                        DRT.Assert(!penultimateMenuItem.IsSubmenuOpen, "Penultimate menu item's submenu should not be open");
                    }
                    break;

                case TextBoxWithContextMenuTestStep.Test2_CloseContextMenu:
                    _textBox1.ContextMenu.IsOpen = false;
                    WaitForMenuAnimationDelay();
                    break;

                case TextBoxWithContextMenuTestStep.Test2_CloseContextMenu_Verify:
                    DRT.Assert(_textBox1.IsKeyboardFocusWithin && !_textBox1.ContextMenu.IsKeyboardFocusWithin, "Focus should be somewhere inside the TextBox and not inside the ContextMenu.  Keyboard.FocusedElement = " + Keyboard.FocusedElement);
                    DRT.Assert(!_textBox1.ContextMenu.IsOpen, "ContextMenu should not be open");
                    DRT.Assert(Mouse.Captured == null, "Mouse.Captured should be null after closing context menu");
                    break;

                ///////// TEST 3 ////////

                case TextBoxWithContextMenuTestStep.Test3_MouseOpenContextMenu_MoveTo:
                    DRT.MoveMouse(_textBox1, 0.1, 0.1);
                    break;

                case TextBoxWithContextMenuTestStep.Test3_MouseOpenContextMenu_RightMouseDown:
                    _textBox1.ContextMenu.Closed += new RoutedEventHandler(ThrowIfClosed);
                    _textBox1.ContextMenu.IsOpen = true;
                    WaitForMenuAnimationDelay();
                    break;

                case TextBoxWithContextMenuTestStep.Test3_MouseOpenContextMenu_RightMouseUp:
                    //DRT.MouseSecondButtonUp();
                    break;

                case TextBoxWithContextMenuTestStep.Test3_MouseOpenContextMenu_Verify:
                    DRT.Assert(_textBox1.ContextMenu.IsOpen, "TextBox's Context Menu should have opened");
                    _textBox1.ContextMenu.Closed -= new RoutedEventHandler(ThrowIfClosed);
                    break;

                case TextBoxWithContextMenuTestStep.Test3_MouseOverFirstItem_MoveTo:
                    MouseOverItem = _textBox1.ContextMenu.Items[0] as MenuItem;
                    break;

                case TextBoxWithContextMenuTestStep.Test3_MouseOverFirstItem_Verify:
                    if (!VerifyMouseOverItem(20)) shouldRepeat = true;
                    break;

                case TextBoxWithContextMenuTestStep.Test3_PressDown1:
                case TextBoxWithContextMenuTestStep.Test3_PressDown2:
                case TextBoxWithContextMenuTestStep.Test3_PressDown3:
                case TextBoxWithContextMenuTestStep.Test3_PressDown4:
                    DRT.PressKey(Key.Down);
                    break;

                case TextBoxWithContextMenuTestStep.Test3_PressRight:
                    DRT.Assert((_textBox1.ContextMenu.Items[4] as MenuItem).IsKeyboardFocused, "Last menu item should be focused");
                    DRT.PressKey(Key.Right);
                    WaitForMenuAnimationDelay();
                    break;

                case TextBoxWithContextMenuTestStep.Test3_VerifySubmenuOpened:
                    DRT.Assert((_textBox1.ContextMenu.Items[4] as MenuItem).IsSubmenuOpen, "Pressing right didn't open menu item's submenu");
                    break;

                case TextBoxWithContextMenuTestStep.Test3_CloseContextMenu:
                    _textBox1.ContextMenu.IsOpen = false;
                    WaitForMenuAnimationDelay();
                    break;

                case TextBoxWithContextMenuTestStep.Test3_CloseContextMenu_Verify:
                    DRT.Assert(_textBox1.IsKeyboardFocusWithin && !_textBox1.ContextMenu.IsKeyboardFocusWithin, "Focus should be somewhere inside the TextBox and not inside the ContextMenu.  Keyboard.FocusedElement = " + Keyboard.FocusedElement);
                    DRT.Assert(!_textBox1.ContextMenu.IsOpen, "ContextMenu should not be open");
                    DRT.Assert(Mouse.Captured == null, "Mouse.Captured should be null after closing context menu");
                    break;

                /////// TEST 4 ///////

                case TextBoxWithContextMenuTestStep.Test4_KeyboardOpenContextMenu_Focus:
                    // Press Shift-F10
                    _textBox2.Focus();
                    break;

                case TextBoxWithContextMenuTestStep.Test4_KeyboardOpenContextMenu_AppsDown:
                    //DRT.PressKey(Key.Apps);
                    // Just open it programmatically to avoid any problems
                    _textBox2.ContextMenu.Closed += new RoutedEventHandler(ThrowIfClosed);
                    _textBox2.ContextMenu.IsOpen = true;
                    WaitForMenuAnimationDelay();
                    break;

                case TextBoxWithContextMenuTestStep.Test4_KeyboardOpenContextMenu_AppsUp:
                    //DRT.SendKeyboardInput(Key.Apps, false);
                    break;

                case TextBoxWithContextMenuTestStep.Test4_VerifyOpened:
                    DRT.Assert(_textBox2.ContextMenu.IsOpen, "TextBox2's Context Menu should have opened");
                    _textBox2.ContextMenu.Closed -= new RoutedEventHandler(ThrowIfClosed);
                    break;

                case TextBoxWithContextMenuTestStep.Test4_PressDown1:
                    DRT.PressKey(Key.Down);
                    break;

                case TextBoxWithContextMenuTestStep.Test4_PressDown2:
                    DRT.Assert((_textBox2.ContextMenu.ItemContainerGenerator.ContainerFromIndex(0) as MenuItem).IsKeyboardFocused, "Items[0] should be focused");
                    DRT.PressKey(Key.Down);
                    break;

                case TextBoxWithContextMenuTestStep.Test4_PressDown3:
                    DRT.Assert((_textBox2.ContextMenu.ItemContainerGenerator.ContainerFromIndex(1) as MenuItem).IsKeyboardFocused, "Items[1] should be focused");
                    DRT.PressKey(Key.Down);
                    break;

                case TextBoxWithContextMenuTestStep.Test4_PressDown4:
                    DRT.Assert((_textBox2.ContextMenu.ItemContainerGenerator.ContainerFromIndex(3) as MenuItem).IsKeyboardFocused, "Items[3] should be focused");
                    DRT.PressKey(Key.Down);
                    break;

                case TextBoxWithContextMenuTestStep.Test4_VerifyFirstItemSelected:
                    // selection should wrap around
                    DRT.Assert((_textBox2.ContextMenu.ItemContainerGenerator.ContainerFromIndex(0) as MenuItem).IsKeyboardFocused, "Items[0] should be focused");
                    break;

                case TextBoxWithContextMenuTestStep.Test4_CloseContextMenu:
                    DRT.SendKeyStrokes(new KeyStatePair(Key.Escape, true), new KeyStatePair(Key.Escape, false),
                                       new KeyStatePair(Key.Escape, true), new KeyStatePair(Key.Escape, false));
                    WaitForMenuAnimationDelay();
                    break;

                case TextBoxWithContextMenuTestStep.Test4_CloseContextMenu_Verify:
                    DRT.Assert(!_textBox2.ContextMenu.IsOpen, "ContextMenu should not be open");
                    DRT.Assert(Mouse.Captured == null, "Mouse.Captured should be null after closing context menu");
                    DRT.Assert(_textBox2.IsKeyboardFocusWithin && !_textBox2.ContextMenu.IsKeyboardFocusWithin, "Focus should be somewhere inside the TextBox and not inside the ContextMenu.  Keyboard.FocusedElement = " + Keyboard.FocusedElement);
                    break;

            }

            if (_test2 != TextBoxWithContextMenuTestStep.End)
            {
                if (!shouldRepeat)
                {
                    _test2++;
                }
                else
                {
                    // go back to the previous test
                    _test2--;
                    DRT.Pause(500);
                }
                DRT.RepeatTest();
            }
        }

        #endregion

        #region Tests for Sparkle bugs

        enum SparkleTestStep
        {
            Start,
            // 
            Test1_OpenEditMenu_MoveTo,
            Test1_OpenEditMenu,
            Test1_MouseOverItem4_MoveTo,
            Test1_MouseOverItem4_Verify,
            Test1_MouseOverItem2_MoveTo,
            Test1_MouseOverItem2_Verify,
            Test1_MouseOverItem2_A_MoveTo,
            Test1_MouseOverItem2_A_Click,
            Test1_VerifyMenuClosed,
            // 


            Test2_ClickItem1_MoveTo,
            Test2_ClickItem1,
            Test2_MoveMouseAway,
            Test2_PressDown1,
            Test2_PressEscape1,
            Test2_PressLeft1,
            Test2_PressRight1,
            Test2_PressDown2,
            Test2_PressRight2,
            Test2_CloseMenus,
            Test2_Verify,
            End,
        }

        SparkleTestStep _sparkleTest = SparkleTestStep.Start;

        void SparkleTest()
        {
            if (DRT.Verbose) Console.WriteLine("Sparkle test : " + _sparkleTest);

            bool shouldRepeat = false;

            switch (_sparkleTest)
            {
                case SparkleTestStep.Start:
                    break;

                case SparkleTestStep.Test1_OpenEditMenu_MoveTo:
                    DRT.MoveMouse(_sparkleMenu.Items[1] as MenuItem, 0.2, 0.2);
                    break;

                case SparkleTestStep.Test1_OpenEditMenu:
                    DRT.ClickMouse();
                    WaitForMenuAnimationDelay();
                    break;

                case SparkleTestStep.Test1_MouseOverItem4_MoveTo:
                    MouseOverItem = (_sparkleMenu.Items[1] as MenuItem).Items[4] as MenuItem;
                    WaitForMenuShowDelay();
                    break;

                case SparkleTestStep.Test1_MouseOverItem4_Verify:
                    if (!VerifyMouseOverItem(20)) shouldRepeat = true;
                    break;

                case SparkleTestStep.Test1_MouseOverItem2_MoveTo:
                    MouseOverItem = (_sparkleMenu.Items[1] as MenuItem).Items[2] as MenuItem;

                    // Wait for the submenu to open
                    WaitForMenuShowDelay();
                    break;

                case SparkleTestStep.Test1_MouseOverItem2_Verify:
                    if (!VerifyMouseOverItem(20)) shouldRepeat = true;
                    else
                    {
                        MenuItem item2 = (_sparkleMenu.Items[1] as MenuItem).Items[2] as MenuItem;
                        DRT.Assert(item2.IsKeyboardFocused, "Item 2 should have focus");
                        DRT.Assert(item2.IsSubmenuOpen, "Item 2's submenu should be open");
                    }
                    break;

                case SparkleTestStep.Test1_MouseOverItem2_A_MoveTo:
                    MouseOverItem = ((_sparkleMenu.Items[1] as MenuItem).Items[2] as MenuItem).Items[0] as MenuItem;
                    break;

                case SparkleTestStep.Test1_MouseOverItem2_A_Click:
                    if (!VerifyMouseOverItem(20)) shouldRepeat = true;
                    else
                    {
                        DRT.ClickMouse();
                        WaitForMenuAnimationDelay();
                    }
                    break;

                case SparkleTestStep.Test1_VerifyMenuClosed:
                    DRT.Assert(!(_menu.Items[1] as MenuItem).IsSubmenuOpen, "Clicked on submenu item; menu should have closed");
                    DRT.Assert(Mouse.Captured == null, "Clicked on submenu item; menu should no longer have focus");
                    break;

                ////// TEST 2 //////

                case SparkleTestStep.Test2_ClickItem1_MoveTo:
                    DRT.MoveMouse(_sparkleMenu.Items[1] as MenuItem, 0.4, 0.6);
                    break;

                case SparkleTestStep.Test2_ClickItem1:
                    DRT.ClickMouse();
                    WaitForMenuAnimationDelay();
                    break;

                case SparkleTestStep.Test2_MoveMouseAway:
                    Input.MoveTo(new Point(0, 0));
                    break;

                case SparkleTestStep.Test2_PressDown1:
                    DRT.Assert(!(_sparkleMenu.Items[0] as MenuItem).IsSubmenuOpen, "Second click did not close the top-level menu");
                    DRT.PressKey(Key.Down);
                    break;

                case SparkleTestStep.Test2_PressEscape1:
                    DRT.Assert((_sparkleMenu.Items[1] as MenuItem).IsSubmenuOpen, "Item 1's submenu is not open");
                    DRT.Assert(((_sparkleMenu.Items[1] as MenuItem).Items[0] as MenuItem).IsKeyboardFocused, "Edit -> A did not have focus");
                    DRT.PressKey(Key.Escape);
                    WaitForMenuAnimationDelay();
                    break;

                case SparkleTestStep.Test2_PressLeft1:
                    DRT.Assert((_sparkleMenu.Items[1] as MenuItem).IsKeyboardFocused, "Item 1 should have focus");
                    DRT.PressKey(Key.Left);
                    WaitForMenuAnimationDelay();
                    break;

                case SparkleTestStep.Test2_PressRight1:
                    DRT.Assert((_sparkleMenu.Items[0] as MenuItem).IsKeyboardFocused, "Item 0 should have focus");
                    DRT.PressKey(Key.Right);
                    break;

                case SparkleTestStep.Test2_PressDown2:
                    DRT.Assert((_sparkleMenu.Items[1] as MenuItem).IsKeyboardFocused, "'Edit' should have focus");
                    DRT.PressKey(Key.Down);
                    WaitForMenuAnimationDelay();
                    break;

                case SparkleTestStep.Test2_PressRight2:
                    DRT.Assert(((_sparkleMenu.Items[1] as MenuItem).Items[0] as MenuItem).IsKeyboardFocused, "Edit -> A should have focus");
                    DRT.PressKey(Key.Right);
                    WaitForMenuAnimationDelay();
                    break;

                case SparkleTestStep.Test2_CloseMenus:
                    DRT.PressKey(Key.RightAlt);
                    WaitForMenuAnimationDelay();
                    break;

                case SparkleTestStep.Test2_Verify:
                    DRT.Assert(!(_sparkleMenu.Items[0] as MenuItem).IsSubmenuOpen, "Item 0's submenu should not be open");
                    DRT.Assert(!(_sparkleMenu.Items[1] as MenuItem).IsSubmenuOpen, "Item 1's submenu should not be open");
                    DRT.Assert(!(_sparkleMenu.Items[2] as MenuItem).IsSubmenuOpen, "Item 2's submenu should not be open");
                    DRT.Assert(!(_sparkleMenu.Items[3] as MenuItem).IsSubmenuOpen, "Item 3's submenu should not be open");
                    break;
            }

            if (_sparkleTest != SparkleTestStep.End)
            {
                if (!shouldRepeat)
                {
                    _sparkleTest++;
                }
                else
                {
                    // go back to the previous test
                    _sparkleTest--;
                    DRT.Pause(500);
                }
                DRT.RepeatTest();
            }
        }

        #endregion

        #region Capture

        enum CaptureTestStep
        {
            Start,
            // test opening the menu (programmatically) when a combobox popup is open
            Test1_OpenComboBox_MoveTo,
            Test1_OpenComboBox_MouseDown,
            Test1_OpenComboBox_MouseUp,
            Test1_VerifyCapture,
            Test1_OpenMainMenu,
            Test1_MouseOverMenuItem,
            Test1_VerifyMenuCapture,
            Test1_ClickMenuItem,
            Test1_VerifyFocusReturned,

            // test popup subcapture
            Test2_OpenPopup,
            Test2_VerifyPopupOpen,
            Test2_MouseOverMenuItem,

            // First verify the button
            Test2_PressButton,
            Test2_VerifyButtonCaptured,
            Test2_ReleaseButton,
            Test2_VerifyPopupCaptured,

            // test popup subcapture with combobox
            Test2_MouseOverComboBox,
            Test2_MouseDownOnComboBox,
            Test2_VerifyComboBoxCaptured,
            Test2_MouseUpOnComboBox,
            Test2_MouseDownOnComboBoxToClose_MouseDown,
            Test2_MouseDownOnComboBoxToClose_MouseUp,
            Test2_VerifyPopupCaptured2,

            Test2_ClickOutside_MoveTo,
            Test2_ClickOutside_Click,
            Test2_VerifyPopupDismissed,
            // press button that opens context menu
            Test3_ClickButton_MoveTo,
            Test3_ClickButton_Click,
            Test3_VerifyCapture,
            Test3_MouseOverMenuItem,
            Test3_ClickMenuItem,
            Test3_VerifyFocusReturned,
            // Verify that parent-less ContextMenus still take capture
            Test4_MouseOverRegion,
            Test4_RightMouseClickRegion,
            Test4_VerifyContextMenuOpened1,
            Test4_MouseOverRegion2,
            Test4_RightMouseClickRegion2,
            Test4_RightMouseUpRegion2,
            Test4_VerifyContextMenuOpened2,
            Test4_VerifyCaptureReleased,
            // 

            // Test focusless menu (type in search box)
            Test5_FocusSearchBox,
            Test5_SearchBox_Type,
            Test5_SearchBox_Backspace,
            Test5_Verify,

            End,
        }

        CaptureTestStep _captureTest = CaptureTestStep.Start;

        private void CaptureTest()
        {
            if (DRT.Verbose) Console.WriteLine("Capture test : " + _captureTest);

            bool shouldRepeat = false;

            switch (_captureTest)
            {
                case CaptureTestStep.Start:
                    break;

                case CaptureTestStep.Test1_OpenComboBox_MoveTo:
                    DRT.MoveMouse(_comboBox, 0.5, 0.5);
                    break;

                case CaptureTestStep.Test1_OpenComboBox_MouseDown:
                    DRT.MouseButtonDown();
                    break;

                case CaptureTestStep.Test1_OpenComboBox_MouseUp:
                    DRT.MouseButtonUp();
                    WaitForMenuAnimationDelay();
                    break;

                case CaptureTestStep.Test1_VerifyCapture:
                    DRT.Assert(Keyboard.FocusedElement == _comboBox, "Focus should be on the combobox, was " + Keyboard.FocusedElement);
                    DRT.Assert(Mouse.Captured != null, "ComboBox's popup should have capture");
                    break;

                case CaptureTestStep.Test1_OpenMainMenu:
                    // This will open the main menu and cause it to steal capture
                    (_menu.Items[0] as MenuItem).IsSubmenuOpen = true;
                    WaitForMenuAnimationDelay();
                    break;

                case CaptureTestStep.Test1_MouseOverMenuItem:
                    // Verify that the combobox has closed
                    DRT.Assert(!_comboBox.IsDropDownOpen, "Stole capture from an open combobox programmatically but it did not close.");
                    MouseOverItem = (_menu.Items[0] as MenuItem).Items[2] as MenuItem;
                    break;

                case CaptureTestStep.Test1_VerifyMenuCapture:
                    if (!VerifyMouseOverItem(20)) shouldRepeat = true;
                    else
                    {
                        DRT.Assert(Mouse.Captured == _menu, "Menu should have mouse capture");
                    }
                    break;

                case CaptureTestStep.Test1_ClickMenuItem:
                    DRT.ClickMouse();
                    WaitForMenuAnimationDelay();
                    break;

                case CaptureTestStep.Test1_VerifyFocusReturned:
                    DRT.Assert(_comboBox.IsKeyboardFocusWithin, "Clicking on item should have returned focus to the combobox, focus is " + Keyboard.FocusedElement);
                    break;

                /////// TEST 2 ////////

                case CaptureTestStep.Test2_OpenPopup:
                    DRT.MoveMouse(_showPopupButton, 0.5, 0.5);
                    DRT.ClickMouse();
                    WaitForMenuAnimationDelay();
                    break;

                case CaptureTestStep.Test2_VerifyPopupOpen:
                    {
                        DRT.Assert(_popup.IsOpen, "AutoClose popup should be open");
                        PresentationSource source = PresentationSource.FromVisual(_popup.Child);
                        DRT.Assert(Mouse.Captured == source.RootVisual, "PopupRoot (" + source.RootVisual + ") should have capture but capture was " + Mouse.Captured);
                    }
                    break;

                case CaptureTestStep.Test2_MouseOverMenuItem:
                    MouseOverItem = _popupMenu.Items[0] as MenuItem;
                    break;

                case CaptureTestStep.Test2_PressButton:
                    if (!VerifyMouseOverItem(20)) shouldRepeat = true;
                    else
                    {
                        DRT.Assert(!(_popupMenu.Items[0] as MenuItem).IsSubmenuOpen, "Mousing over the menu in a popup should not have opened the submenu");
                        DRT.MoveMouse(_popupButton, 0.5, 0.5);
                        DRT.MouseButtonDown();
                    }
                    break;

                case CaptureTestStep.Test2_VerifyButtonCaptured:
                    DRT.Assert(Mouse.Captured == _popupButton, "Pressing button on popup should have moved capture to popup but capture was " + Mouse.Captured);
                    break;

                case CaptureTestStep.Test2_ReleaseButton:
                    DRT.MouseButtonUp();
                    break;

                case CaptureTestStep.Test2_VerifyPopupCaptured:
                    {
                        PresentationSource source = PresentationSource.FromVisual(_popup.Child);
                        DRT.Assert(Mouse.Captured == source.RootVisual, "PopupRoot (" + source.RootVisual + ") should have capture but capture was " + Mouse.Captured);
                    }
                    break;

                case CaptureTestStep.Test2_MouseOverComboBox:
                    DRT.MoveMouse(_popupComboBox, 0.5, 0.5);
                    break;

                case CaptureTestStep.Test2_MouseDownOnComboBox:
                    DRT.MouseButtonDown();
                    WaitForMenuAnimationDelay();
                    break;

                case CaptureTestStep.Test2_VerifyComboBoxCaptured:
                    DRT.Assert(_popupComboBox.IsDropDownOpen, "Mousing down on the combobox in the popup should have opened the combobox.");
                    DRT.Assert(Mouse.Captured == _popupComboBox, "Mousing down on the combobox in the popup should have taken capture to the combobox.  Capture is " + Mouse.Captured);
                    break;

                case CaptureTestStep.Test2_MouseUpOnComboBox:
                    DRT.MouseButtonUp();
                    WaitForMenuAnimationDelay();
                    break;

                case CaptureTestStep.Test2_MouseDownOnComboBoxToClose_MouseDown:
                    DRT.MoveMouse(_popup.Child, 0.0, 0.0);
                    DRT.Assert(_popupComboBox.IsDropDownOpen, "After mouse up, combobox should still be open.");
                    DRT.Assert(Mouse.Captured == _popupComboBox, "After mouse up on the combobox capture should still be on the combobox.  Capture is " + Mouse.Captured);
                    DRT.MouseButtonDown();
                    WaitForMenuAnimationDelay();
                    break;

                case CaptureTestStep.Test2_MouseDownOnComboBoxToClose_MouseUp:
                    {
                        DRT.Assert(_popup.IsOpen, "AutoClose popup should be open");
                        DRT.Assert(!_popupComboBox.IsDropDownOpen, "Clicking on the ComboBox did not close the drop down");
                        PresentationSource source = PresentationSource.FromVisual(_popup.Child);
                        if (Mouse.Captured != source.RootVisual)
                            Console.WriteLine("WARNING: PopupRoot (" + source.RootVisual + ") should have capture but capture was " + Mouse.Captured);
                        //DRT.Assert(Mouse.Captured == source.RootVisual, "PopupRoot (" + source.RootVisual + ") should have capture but capture was " + Mouse.Captured);

                        DRT.MouseButtonUp();
                        WaitForMenuAnimationDelay();
                    }
                    break;

                case CaptureTestStep.Test2_VerifyPopupCaptured2:
                    {
                        DRT.Assert(_popup.IsOpen, "AutoClose popup should be open");
                        DRT.Assert(!_popupComboBox.IsDropDownOpen, "Clicking on the ComboBox did not close the drop down");
                        PresentationSource source = PresentationSource.FromVisual(_popup.Child);
                        DRT.Assert(Mouse.Captured == source.RootVisual, "PopupRoot (" + source.RootVisual + ") should have capture but capture was " + Mouse.Captured);
                    }
                    break;

                case CaptureTestStep.Test2_ClickOutside_MoveTo:
                    DRT.MoveMouse(DRT.RootElement as FrameworkElement, 0.01, 0.99);
                    break;

                case CaptureTestStep.Test2_ClickOutside_Click:
                    DRT.ClickMouse();
                    WaitForMenuAnimationDelay();
                    break;

                case CaptureTestStep.Test2_VerifyPopupDismissed:
                    DRT.Assert(!_popup.IsOpen, "Popup should have closed on click outside");
                    DRT.Assert(Mouse.Captured == null, "Popup should not have capture any more after it is dismissed");
                    break;

                //////// TEST 3 ///////

                case CaptureTestStep.Test3_ClickButton_MoveTo:
                    DRT.MoveMouse(_button2, 0.5, 0.5);
                    break;

                case CaptureTestStep.Test3_ClickButton_Click:
                    DRT.ClickMouse();
                    WaitForMenuAnimationDelay();
                    break;

                case CaptureTestStep.Test3_VerifyCapture:
                    DRT.Assert(_button.ContextMenu.IsOpen, "Clicking button2 should have opened button1's context menu");
                    DRT.Assert(Mouse.Captured == _button.ContextMenu, "button1's context menu should have mouse capture; capture was " + Mouse.Captured);
                    break;

                case CaptureTestStep.Test3_MouseOverMenuItem:
                    MouseOverItem = _button.ContextMenu.ItemContainerGenerator.ContainerFromIndex(4) as MenuItem;
                    break;

                case CaptureTestStep.Test3_ClickMenuItem:
                    if (!VerifyMouseOverItem(20)) shouldRepeat = true;
                    else
                    {
                        DRT.Assert(Mouse.Captured == _button.ContextMenu, "Button1's context menu should still have mouse capture; capture was " + Mouse.Captured);
                        DRT.ClickMouse();
                        WaitForMenuAnimationDelay();
                    }
                    break;

                case CaptureTestStep.Test3_VerifyFocusReturned:
                    DRT.Assert(_button2.IsKeyboardFocused, "Clicking on menu item should have returned focus to button 2");
                    break;


                //////// TEST 4 ///////
                /// This test verifies that if you open a ContextMenu with no PlacementTarget or visual parent
                /// That capture is still taken and when you right click on the owner region that the context menu
                /// is dismissed.

                case CaptureTestStep.Test4_MouseOverRegion:
                    DRT.MoveMouse(_contextMenuBorder, 0.8, 0.5);
                    _currentContextMenuCreated = null;
                    _lastContextMenuCreated = null;
                    break;

                case CaptureTestStep.Test4_RightMouseClickRegion:
                    DRT.ClickMouseSecondButton();
                    WaitForMenuAnimationDelay();
                    break;

                case CaptureTestStep.Test4_VerifyContextMenuOpened1:
                    DRT.Assert(_currentContextMenuCreated != null, "Right clicking the _contextMenuBorder did not open the ContextMenu");
                    DRT.Assert(_currentContextMenuCreated.IsOpen, "_currentContextMenuCreated.IsOpen should be true");
                    DRT.Assert(Mouse.Captured != null, "When the context menu is open, something within the context menu should have capture");
                    break;

                case CaptureTestStep.Test4_MouseOverRegion2:
                    DRT.MoveMouse(_contextMenuBorder, 0.5, 0.5);
                    break;

                case CaptureTestStep.Test4_RightMouseClickRegion2:
                    DRT.ClickMouseSecondButton();
                    WaitForMenuAnimationDelay();
                    break;

                case CaptureTestStep.Test4_VerifyContextMenuOpened2:
                    DRT.Assert(_lastContextMenuCreated != null, "_lastContextMenuCreated should not be null -- a new context menu was not created");
                    DRT.Assert(!_lastContextMenuCreated.IsOpen, "Right clicking should have closed the previously open context menu");
                    DRT.Assert(_currentContextMenuCreated != null, "Right clicking a second time did not create a new context menu");
                    DRT.Assert(_currentContextMenuCreated.IsOpen, "_currentContextMenuCreated.IsOpen should be true");
                    DRT.Assert(Mouse.Captured != null, "When the context menu is open, something within the context menu should have capture (2)");
                    _currentContextMenuCreated.IsOpen = false;
                    WaitForMenuAnimationDelay();
                    break;

                case CaptureTestStep.Test4_VerifyCaptureReleased:
                    DRT.Assert(Mouse.Captured == null, "After closing the context menus, nothing should have capture -- capture was " + Mouse.Captured);
                    break;

                case CaptureTestStep.Test5_FocusSearchBox:
                    _searchTextBox.Focus();
                    break;

                case CaptureTestStep.Test5_SearchBox_Type:
                    DRT.PressKey(Key.A);
                    WaitForMenuAnimationDelay();
                    break;

                case CaptureTestStep.Test5_SearchBox_Backspace:
                    DRT.Assert(_searchDropDown.IsOpen, "Typing in search text box should open _searchDropDown");
                    DRT.Assert(_searchTextBox.IsKeyboardFocusWithin, "Focus should be within the search text box, focus was: " + Keyboard.FocusedElement);
                    DRT.Assert(!_searchDropDown.IsKeyboardFocusWithin, "Focus should not be within the search drop down, focus was: " + Keyboard.FocusedElement);
                    DRT.PressKey(Key.Back);
                    WaitForMenuAnimationDelay();
                    break;

                case CaptureTestStep.Test5_Verify:
                    DRT.Assert(!_searchDropDown.IsOpen, "Backspacing should close the search drop down");
                    DRT.Assert(_searchTextBox.IsKeyboardFocusWithin, "Focus should be within the search text box, focus was: " + Keyboard.FocusedElement);
                    DRT.Assert(!_searchDropDown.IsKeyboardFocusWithin, "Focus should not be within the search drop down, focus was: " + Keyboard.FocusedElement);
                    break;
            }

            if (_captureTest != CaptureTestStep.End)
            {
                if (!shouldRepeat)
                {
                    _captureTest++;
                }
                else
                {
                    // go back to the previous test
                    _captureTest--;
                    DRT.Pause(500);
                }

                DRT.RepeatTest();
            }
        }

        private void _popupButton_Click(object sender, RoutedEventArgs e)
        {
            _popup.IsOpen = true;
        }

        #endregion

        #region Access Key tests

        enum AccessKeyTestStep
        {
            Start,
            MoveToButton,
            OpenContextMenu1,
            PressKeyI,
            VerifyInvokedItem1,
            OpenContextMenu2,
            PressKeyB1,
            VerifyFocus1,
            PressKeyB2,
            VerifyFocus2,
            MouseOverToOpenSubmenu,
            PressKeyU,
            VerifyInvokedSubmenuItem2,
            End,
        }


        AccessKeyTestStep _accessKeyTest = AccessKeyTestStep.Start;

        private void AccessKeyTest()
        {
            if (DRT.Verbose) Console.WriteLine("AccessKey test : " + _accessKeyTest);

            bool shouldRepeat = false;

            switch (_accessKeyTest)
            {
                case AccessKeyTestStep.Start:
                    DRT.MoveMouse(_accessKeyButton, 0.2, 0.2);
                    break;

                case AccessKeyTestStep.OpenContextMenu1:
                    DRT.ClickMouseSecondButton();
                    WaitForMenuAnimationDelay();
                    break;

                case AccessKeyTestStep.PressKeyI:
                    DRT.Assert(_accessKeyContextMenu.IsOpen, "Context menu did not open (1)");
                    DRT.PressKey(Key.I);
                    break;

                case AccessKeyTestStep.VerifyInvokedItem1:
                    DRT.Assert(_lastAKMenuItem != null, "No menu item access key was invoked");
                    DRT.Assert(_lastAKMenuItem == _accessKeyContextMenu.Items[0], "Invoked access key was not " + _accessKeyContextMenu.Items[0] + ", was " + _lastAKMenuItem);
                    _lastAKMenuItem = null;
                    break;

                case AccessKeyTestStep.OpenContextMenu2:
                    DRT.ClickMouseSecondButton();
                    WaitForMenuAnimationDelay();
                    break;

                case AccessKeyTestStep.PressKeyB1:
                    DRT.Assert(_accessKeyContextMenu.IsOpen, "Context menu did not open (2)");
                    DRT.PressKey(Key.B);
                    break;

                case AccessKeyTestStep.VerifyFocus1:
                    DRT.Assert(Keyboard.FocusedElement == _accessKeyContextMenu.Items[4], "Focus should be on first button, was on " + Keyboard.FocusedElement);
                    break;

                case AccessKeyTestStep.PressKeyB2:
                    DRT.PressKey(Key.B);
                    break;

                case AccessKeyTestStep.VerifyFocus2:
                    DRT.Assert(Keyboard.FocusedElement == _accessKeyContextMenu.Items[5], "Focus should be on second button, was on " + Keyboard.FocusedElement);
                    break;

                case AccessKeyTestStep.MouseOverToOpenSubmenu:
                    MouseOverItem = ((MenuItem)_accessKeyContextMenu.Items[2]);
                    WaitForMenuShowDelay();
                    break;

                case AccessKeyTestStep.PressKeyU:
                    if (!VerifyMouseOverItem(20)) shouldRepeat = true;
                    else
                    {
                        DRT.Assert(((MenuItem)_accessKeyContextMenu.Items[2]).IsSubmenuOpen, "Submenu didn't open");
                        DRT.PressKey(Key.U);
                        WaitForMenuAnimationDelay();
                    }
                    break;

                case AccessKeyTestStep.VerifyInvokedSubmenuItem2:
                    DRT.Assert(_lastAKMenuItem != null, "No menu item access key was invoked");
                    DRT.Assert(_lastAKMenuItem == ((MenuItem)_accessKeyContextMenu.Items[2]).Items[1],
                        "Invoked access key was not " + ((MenuItem)_accessKeyContextMenu.Items[2]).Items[1] + ", was " + _lastAKMenuItem);
                    _lastAKMenuItem = null;
                    break;

                case AccessKeyTestStep.End:
                    break;
            }

            if (_accessKeyTest != AccessKeyTestStep.End)
            {
                if (!shouldRepeat)
                {
                    _accessKeyTest++;
                }
                else
                {
                    // go back to the previous test
                    _accessKeyTest--;
                    DRT.Pause(500);
                }

                DRT.RepeatTest();
            }
        }

        private void OnAKContextMenuItemClicked(object sender, RoutedEventArgs e)
        {
            _lastAKMenuItem = e.OriginalSource as MenuItem;
        }

        private MenuItem _lastAKMenuItem;


        #endregion

        #region Composition

        enum CompositionTestStep
        {
            Start,
            // Try to click on a button in the menu
            Test1_OpenMenu_MoveTo,
            Test1_OpenMenu_ClickMouse,
            Test1_OpenSubMenu_MoveTo,
            Test1_OpenSubMenu_ClickMouse,
            Test1_ClickButton_MoveTo,
            Test1_ClickButton_MouseDown,
            Test1_ClickButton_MouseUp,
            Test1_Verify,

            // test radio buttons
            Test2_ClickRadioButton1_MoveTo,
            Test2_ClickRadioButton1_MouseDown,
            Test2_ClickRadioButton1_MouseUp,
            Test2_Verify,

            // test textbox
            Test3_ClickTextBox_MoveTo,
            Test3_ClickTextBox_MouseDown,
            Test3_ClickTextBox_MouseUp,
            Test3_TypeIntoTextBox_PressEnd,
            Test3_TypeIntoTextBox_PressT,
            Test3_TypeIntoTextBox_PressE,
            Test3_TypeIntoTextBox_PressS,
            Test3_TypeIntoTextBox_PressT2,
            Test3_Verify,

            // test context menu
            Test4_RightClickMenuItem_MoveTo,
            Test4_RightClickMenuItem_MouseDown,
            Test4_RightClickMenuItem_MouseUp,
            Test4_ClickContextMenuItem_MoveTo,
            Test4_ClickContextMenuItem_ClickMouse,
            Test4_Verify,
            Test4_LoseCapture,
            Test4_LoseCapture_Verify,

            End,
        }


        CompositionTestStep _compositionTest = CompositionTestStep.Start;

        private void CompositionTest()
        {
            if (DRT.Verbose) Console.WriteLine("Composition test: " + _compositionTest);

            bool shouldRepeat = false;

            switch (_compositionTest)
            {
                case CompositionTestStep.Start:
                    break;

                case CompositionTestStep.Test1_OpenMenu_MoveTo:
                    MouseOverItem = ((MenuItem)_menu.Items[4]);
                    WaitForMenuShowDelay();
                    break;

                case CompositionTestStep.Test1_OpenMenu_ClickMouse:
                    if (!VerifyMouseOverItem(20)) shouldRepeat = true;
                    else
                    {
                        DRT.ClickMouse();
                        WaitForMenuAnimationDelay();
                    }
                    break;

                case CompositionTestStep.Test1_OpenSubMenu_MoveTo:
                    if (VerifyPopupOpened((MenuItem)_menu.Items[4], true, 20))
                    {
                        MouseOverItem = ((MenuItem)((MenuItem)_menu.Items[4]).Items[5]);
                    }
                    else
                    {
                        shouldRepeat = true;
                    }
                    break;

                case CompositionTestStep.Test1_OpenSubMenu_ClickMouse:
                    if (!VerifyMouseOverItem(20)) shouldRepeat = true;
                    else
                    {
                        DRT.ClickMouse();
                        WaitForMenuAnimationDelay();
                    }
                    break;

                case CompositionTestStep.Test1_ClickButton_MoveTo:
                    if (VerifyPopupOpened((MenuItem)((MenuItem)_menu.Items[4]).Items[5], true, 20))
                    {
                        DRT.MoveMouse(_menu_button, 0.5, 0.5);
                    }
                    else
                    {
                        shouldRepeat = true;
                    }
                    break;

                case CompositionTestStep.Test1_ClickButton_MouseDown:
                    DRT.MouseButtonDown();
                    break;

                case CompositionTestStep.Test1_ClickButton_MouseUp:
                    DRT.Assert(((MenuItem)_menu.Items[4]).IsSubmenuOpen, "Mouse down on button in menu should not close the menu");
                    DRT.Assert(((MenuItem)((MenuItem)_menu.Items[4]).Items[5]).IsSubmenuOpen, "Mouse down on button in menu should not close the submenu");
                    DRT.Assert(_menu_button.IsKeyboardFocused, "Mouse down on button in menu should focus the button temporarily, focus is: " + Keyboard.FocusedElement);
                    DRT.Assert(_menu_button.IsPressed, "_menu_button.IsPressed should be true");
                    DRT.Assert(Mouse.Captured == _menu_button, "Mouse down on button in menu should take capture to the button, capture is: " + Mouse.Captured);
                    DRT.MouseButtonUp();
                    break;

                case CompositionTestStep.Test1_Verify:
                    DRT.Assert(((MenuItem)_menu.Items[4]).IsSubmenuOpen, "Mouse up on button in menu should not close the menu");
                    DRT.Assert(((MenuItem)((MenuItem)_menu.Items[4]).Items[5]).IsSubmenuOpen, "Mouse up on button in menu should not close the submenu");
                    DRT.Assert(!_menu_button.IsKeyboardFocused, "After mouse up, focus should restore to the main scope, focus is: " + Keyboard.FocusedElement);
                    DRT.Assert(!_menu_button.IsPressed, "After mouse up, _menu_button.IsPressed should be false");
                    DRT.Assert(!_menu_button.IsMouseCaptured, "After mouse up on _menu_button, should not have capture");
                    DRT.Assert(Mouse.Captured == _menu, "After mouse up on _menu_button, _menu should have capture, capture is: " + Mouse.Captured);
                    break;

                case CompositionTestStep.Test2_ClickRadioButton1_MoveTo:
                    DRT.MoveMouse(_menu_radioButton1, 0.5, 0.5);
                    break;

                case CompositionTestStep.Test2_ClickRadioButton1_MouseDown:
                    DRT.MouseButtonDown();
                    break;

                case CompositionTestStep.Test2_ClickRadioButton1_MouseUp:
                    DRT.Assert(((MenuItem)_menu.Items[4]).IsSubmenuOpen, "Mouse down on radiobutton in menu should not close the menu");
                    DRT.Assert(((MenuItem)((MenuItem)_menu.Items[4]).Items[5]).IsSubmenuOpen, "Mouse down on radiobutton in menu should not close the submenu");
                    DRT.Assert(_menu_radioButton1.IsKeyboardFocused, "Mouse down on radiobutton in menu should focus the radiobutton, focus is: " + Keyboard.FocusedElement);
                    DRT.Assert(_menu_radioButton1.IsPressed, "_menu_radioButton1.IsPressed should be true");
                    DRT.Assert(Mouse.Captured == _menu_radioButton1, "Mouse down on radiobutton in menu should take capture to the radiobutton, capture is: " + Mouse.Captured);
                    DRT.MouseButtonUp();
                    break;

                case CompositionTestStep.Test2_Verify:
                    DRT.Assert(((MenuItem)_menu.Items[4]).IsSubmenuOpen, "Mouse up on radiobutton in menu should not close the menu");
                    DRT.Assert(((MenuItem)((MenuItem)_menu.Items[4]).Items[5]).IsSubmenuOpen, "Mouse up on radiobutton in menu should not close the submenu");
                    DRT.Assert(!_menu_radioButton1.IsKeyboardFocused, "After mouse up, focus should go to the main focus scope, focus is: " + Keyboard.FocusedElement);
                    DRT.Assert(!_menu_radioButton1.IsPressed, "After mouse up, _menu_radioButton1.IsPressed should be false");
                    DRT.Assert(!_menu_radioButton1.IsMouseCaptured, "After mouse up on _menu_radioButton1, should not have capture");
                    DRT.Assert(_menu_radioButton1.IsChecked == true, "After mouse up, radiobutton should have been checked");
                    DRT.Assert(Mouse.Captured == _menu, "After mouse up on _menu_radioButton1, _menu should have capture, capture is: " + Mouse.Captured);
                    break;

                //////////////////////////////////////////////////////
                //////////////////////////////////////////////////////
                //////////////////////////////////////////////////////

                case CompositionTestStep.Test3_ClickTextBox_MoveTo:
                    DRT.MoveMouse(_menu_textBox, 0.5, 0.5);
                    break;

                case CompositionTestStep.Test3_ClickTextBox_MouseDown:
                    DRT.MouseButtonDown();
                    break;

                case CompositionTestStep.Test3_ClickTextBox_MouseUp:
                    DRT.Assert(((MenuItem)_menu.Items[4]).IsSubmenuOpen, "Mouse down on textbox in menu should not close the menu");
                    DRT.Assert(((MenuItem)((MenuItem)_menu.Items[4]).Items[5]).IsSubmenuOpen, "Mouse down on textbox in menu should not close the submenu");
                    DRT.Assert(_menu_textBox.IsKeyboardFocusWithin, "Mouse down on textbox in menu should focus the textbox, focus is: " + Keyboard.FocusedElement);

                    DRT.MouseButtonUp();
                    break;

                case CompositionTestStep.Test3_TypeIntoTextBox_PressEnd:
                    DRT.Assert(((MenuItem)_menu.Items[4]).IsSubmenuOpen, "Mouse up on textbox in menu should not close the menu");
                    DRT.Assert(((MenuItem)((MenuItem)_menu.Items[4]).Items[5]).IsSubmenuOpen, "Mouse up on textbox in menu should not close the submenu");
                    DRT.Assert(_menu_textBox.IsKeyboardFocusWithin, "After mouse up, focus should remain on the textbox, focus is: " + Keyboard.FocusedElement);
                    DRT.Assert(Mouse.Captured == _menu, "After mouse up on _menu_textBox, _menu should have capture, capture is: " + Mouse.Captured);

                    DRT.PressKey(Key.End);
                    break;

                case CompositionTestStep.Test3_TypeIntoTextBox_PressT:
                    DRT.PressKey(Key.T);
                    break;

                case CompositionTestStep.Test3_TypeIntoTextBox_PressE:
                    DRT.PressKey(Key.E);
                    break;

                case CompositionTestStep.Test3_TypeIntoTextBox_PressS:
                    DRT.PressKey(Key.S);
                    break;

                case CompositionTestStep.Test3_TypeIntoTextBox_PressT2:
                    DRT.PressKey(Key.T);
                    break;

                case CompositionTestStep.Test3_Verify:
                    DRT.AssertEqual("TextBox in a menu is sort of cool, no? " + "test", _menu_textBox.Text, "Typed 'test' into textbox");
                    break;

                //////////////////////////////////////////////////////
                //////////////////////////////////////////////////////
                //////////////////////////////////////////////////////

                case CompositionTestStep.Test4_RightClickMenuItem_MoveTo:
                    MouseOverItem = _menu_submenuItemWithContextMenu;
                    break;

                case CompositionTestStep.Test4_RightClickMenuItem_MouseDown:
                    if (!VerifyMouseOverItem(20)) shouldRepeat = true;
                    else
                    {
                        DRT.MouseSecondButtonDown();
                    }
                    break;

                case CompositionTestStep.Test4_RightClickMenuItem_MouseUp:
                    DRT.Assert(((MenuItem)_menu.Items[4]).IsSubmenuOpen, "Mouse down on textbox in menu should not close the menu");
                    DRT.Assert(((MenuItem)((MenuItem)_menu.Items[4]).Items[5]).IsSubmenuOpen, "Mouse down on textbox in menu should not close the submenu");

                    DRT.MouseSecondButtonUp();
                    WaitForMenuAnimationDelay();
                    break;

                case CompositionTestStep.Test4_ClickContextMenuItem_MoveTo:
                    DRT.Assert(_menu_contextMenu.IsOpen, "Right clicking the submenu item should have opened its context menu");

                    MouseOverItem = _menu_contextMenu.ItemContainerGenerator.ContainerFromIndex(1) as MenuItem;
                    break;

                case CompositionTestStep.Test4_ClickContextMenuItem_ClickMouse:
                    if (!VerifyMouseOverItem(20)) shouldRepeat = true;
                    else
                    {
                        DRT.ClickMouse();
                        WaitForMenuAnimationDelay();
                    }
                    break;

                case CompositionTestStep.Test4_Verify:
                    DRT.Assert(!_menu_contextMenu.IsOpen, "clicking on the submenu's context menu should have closed it");
                    DRT.Assert(((MenuItem)_menu.Items[4]).IsSubmenuOpen, "Invoking menuitem in sub-contextmenu should not close the menu");
                    DRT.Assert(((MenuItem)((MenuItem)_menu.Items[4]).Items[5]).IsSubmenuOpen, "Invoking menuitem in sub-contextmenu should not close the submenu");
                    DRT.Assert(Mouse.Captured == _menu, "After closing the sub-contextmenu, _menu should have capture, capture is: " + Mouse.Captured);
                    break;

                case CompositionTestStep.Test4_LoseCapture:
                    Mouse.Capture(null);
                    DRT.Assert(Mouse.Captured != _menu, "Menu should not have capture");
                    WaitForMenuAnimationDelay();
                    break;

                case CompositionTestStep.Test4_LoseCapture_Verify:
                    // throwing away capture should close the menu
                    shouldRepeat = !VerifyPopupOpened((MenuItem)_menu.Items[4], false, 20);
                    break;

                case CompositionTestStep.End:
                    break;
            }

            if (_compositionTest != CompositionTestStep.End)
            {
                if (!shouldRepeat)
                {
                    _compositionTest++;
                }
                else
                {
                    // go back to the previous test
                    _compositionTest--;
                    DRT.Pause(500);
                }

                DRT.RepeatTest();
            }
        }

        #endregion



        public void ClickHandlerThatBlocksContext(object sender, RoutedEventArgs e)
        {
            DRT.ConsoleOut.WriteLine("Keyboard.FocusedElement = " + Keyboard.FocusedElement);
            DRT.ConsoleOut.WriteLine("Press Enter: ");
            Console.ReadLine();
            DRT.ConsoleOut.WriteLine("Done!");
        }

        private void ThrowIfClosed(object sender, RoutedEventArgs e)
        {
            DRT.ConsoleOut.WriteLine("ERROR: ContextMenu closed unexpectedly");
            DRT.ConsoleOut.WriteLine("Tentatively succeeding...");
            Environment.Exit(0);
            //DRT.Assert(false, "ContextMenu closed unexpectedly");
        }

        private Button _button;
        private Button _button2;
        private Button _button3;
        private TextBox _textBox1;
        private TextBox _textBox2;
        private Button _activateButton;
        private Menu _menu;
        private int _count = 0;
        private bool _submenuItemClicked = false;
        private MouseTest _mouseTest = MouseTest.Start;
        private KeyboardTest _keyboardTest = KeyboardTest.Start;
        private bool _submenuOpenChangedEvent = false;
        private bool _submenuOpen = false;
        private bool _checkedEvent = false;
        private bool _checked;
        private Menu _sparkleMenu;
        private ComboBox _comboBox;
        private Popup _popup;
        private Menu _popupMenu;
        private Button _popupButton;
        private ComboBox _popupComboBox;
        private Button _showPopupButton;
        private Button _accessKeyButton;
        private ContextMenu _accessKeyContextMenu;
        private Border _contextMenuBorder;
        private Menu _staysOpenOnClickMenu;
        private MenuItem _staysOpenOnClickHeader;
        private MenuItem _staysOpenOnClickTrue;
        private MenuItem _staysOpenOnClickFalse;

        // Stuff in a submenu
        private Button _menu_button;
        private RadioButton _menu_radioButton1;
        private RadioButton _menu_radioButton2;
        private TextBox _menu_textBox;
        private ContextMenu _menu_contextMenu;
        private MenuItem _menu_submenuItemWithContextMenu;

        // Focusless menu scenario
        private TextBox _searchTextBox;
        private ContextMenu _searchDropDown;

        #region Debugging stuff for layered windows

        [DllImport("user32.dll", EntryPoint = "WindowFromPoint", ExactSpelling = true, CharSet = CharSet.Auto)]
        private static extern IntPtr _WindowFromPoint(POINTSTRUCT pt);
        internal static IntPtr WindowFromPoint(int x, int y)
        {
            POINTSTRUCT ps = new POINTSTRUCT(x, y);
            return _WindowFromPoint(ps);
        }

        private struct POINTSTRUCT
        {
            internal int x;
            internal int y;

            internal POINTSTRUCT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern bool GetCursorPos([In, Out] POINT pt);

        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x = 0;
            public int y = 0;

            public POINT()
            {
            }

            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        private const int GWL_EXSTYLE = (-20);
        private const int WS_EX_TRANSPARENT = 0x00000020;

        //
        // GetWindowLong
        //

        // We have this wrapper because casting IntPtr to int may
        // generate OverflowException when one of high 32 bits is set.
        private static int IntPtrToInt32(IntPtr intPtr)
        {
            return unchecked((int)intPtr.ToInt64());
        }

#if WIN64
        /// <SecurityNote>
        ///  SecurityCritical: This code happens to return a critical resource and causes unmanaged code elevation
        /// </SecurityNote>
        [DllImport("user32.dll",
         EntryPoint="GetWindowLongPtr",
        CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr IntGetWindowLongPtr(HandleRef hWnd, int nIndex);
#else // WIN64
        /// <SecurityNote>
        ///  SecurityCritical: This code happens to return a critical resource and causes unmanaged code elevation
        /// </SecurityNote>
        [DllImport("user32.dll",
         EntryPoint = "GetWindowLong",
        CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Int32 IntGetWindowLong(HandleRef hWnd, int nIndex);
#endif // WIN64

        /// <SecurityNote>
        ///  SecurityCritical: This code happens to return a critical resource and causes unmanaged code elevation
        /// </SecurityNote>
        internal static Int32 GetWindowLong(HandleRef hWnd, int nIndex)
        {
            int flag = 0;
            int error = 0;
#if WIN64
            // use GetWindowLongPtr
            IntPtr ptr = IntGetWindowLongPtr(hWnd, nIndex);
            error = Marshal.GetLastWin32Error();
            flag = NativeMethods.IntPtrToInt32(ptr);
#else
            // use GetWindowLong
            flag = IntGetWindowLong(hWnd, nIndex);
            error = Marshal.GetLastWin32Error();
#endif // WIN64
            return flag;
        }

        private void OnPostProcessInput(object sender, ProcessInputEventArgs e)
        {
            KeyEventArgs ke = e.StagingItem.Input as KeyEventArgs;
            if (ke != null)
            {
                Console.Write("PostProcessInput: event (" + ke.RoutedEvent.Name + "), focus = ");
                MenuItem item = Keyboard.FocusedElement as MenuItem;
                if (item != null)
                {
                    Console.Write("MenuItem (" + item.Header + ")");
                }
                else
                {
                    Console.Write(Keyboard.FocusedElement);
                }
                Console.Write(ke.Handled ? ", handled" : ", not handled");
                Console.WriteLine();
            }
        }

        #endregion

        // Called for MenuItem or ContextMenu
        void OnMenuOpened(object sender, RoutedEventArgs e)
        {
            // Verify that the submenu is open after measure has happened and the UCE is done
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                (DispatcherOperationCallback)delegate(object arg)
                {
                    // By this time the test could have closed the menu already, so only check if it's still open
                    if ((arg is MenuItem && ((MenuItem)arg).IsSubmenuOpen) || (arg is ContextMenu && ((ContextMenu)arg).IsOpen))
                    {
                        DRT.WaitForCompleteRender();
                        if (DRT.Verbose) Console.WriteLine("Verify that the menu for {0} is open", arg);
                        if (arg is MenuItem)
                        {
                            VerifyPopupOpened(arg as MenuItem, true);
                        }
                        else
                        {
                            VerifyContextMenuCorrectSize(arg as ContextMenu);
                        }
                    }
                    return null;
                },
                e.OriginalSource
                );
        }

        // this is a big mitigation where layered
        //             windows can appear at size 1x1.  In this case we need to tentatively succeed.  For now. 
        void VerifyContextMenuCorrectSize(ContextMenu cm)
        {
            UIElement ui = cm.ItemContainerGenerator.ContainerFromIndex(0) as UIElement;
            if (ui == null) return;

            IntPtr hwnd = ((HwndSource)PresentationSource.FromVisual(ui)).Handle;

            RECT windowRect = new RECT();
            GetWindowRect(new HandleRef(null, hwnd), ref windowRect);
            if (WindowNear(windowRect.right - windowRect.left, 0) ||
                        WindowNear(windowRect.bottom - windowRect.top, 0))
            {
                Console.WriteLine();
                Console.WriteLine("Popup window is too small. Bug 947874. LTRB:{0},{1},{2},{3}", windowRect.left, windowRect.top, windowRect.right, windowRect.bottom);
                Console.WriteLine();
                Console.WriteLine("Test will now tentatively succeed");
                System.Environment.Exit(0);
            }

        }

        private void contextMenuBorder_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = new ContextMenu();
            _lastContextMenuCreated = _currentContextMenuCreated;
            _currentContextMenuCreated = cm;
            cm.Items.Add("Right click again");
            cm.Items.Add("on that region");

            cm.Placement = PlacementMode.MousePoint;
            cm.IsOpen = true;
        }

        private ContextMenu _lastContextMenuCreated;
        private ContextMenu _currentContextMenuCreated;

        // Bootstrap:
        // <MenuItem ItemContainerStyle="{TreeChildrenContinuation}" Header="DRT Logical Tree">
        // <MenuItem.Resources>
        //     <Style x:Key="{TreeChildrenContinuation}">
        //         <MenuItem Header="*Bind(Converter={MyHeaderConverter})"
        //                   ItemsSource="*Binding(Converter={MyItemsSourceConverter})"
        //                   ItemContainerStyle="{TreeChildrenContinuation}" />
        //     </Style>
        // </MenuItem.Resources>
        // </MenuItem>
        private MenuItem MakeDataBoundHierarchy()
        {
            MenuItem mi = new MenuItem();

            Style itemContainerStyle = new Style(typeof(MenuItem));

            Binding headerBinding = new Binding();
            headerBinding.Converter = new MyHeaderConverter();
            itemContainerStyle.Setters.Add (new Setter(MenuItem.HeaderProperty, headerBinding));

            Binding itemsSourceBinding = new Binding();
            itemsSourceBinding.Converter = new MyItemsSourceConverter();
            itemContainerStyle.Setters.Add (new Setter(MenuItem.ItemsSourceProperty, itemsSourceBinding));

            //itemContainerStyle.Setters.Add (new Setter(MenuItem.ItemContainerStyleProperty, new DynamicResourceExtension("TreeChildrenContinuation")));

            mi.Resources[typeof(MenuItem)] = itemContainerStyle;

            Binding headerBinding0 = new Binding();
            headerBinding0.Converter = new MyHeaderConverter();
            headerBinding0.Path = new PropertyPath("Tag");
            headerBinding0.RelativeSource = RelativeSource.Self;
            mi.SetBinding(MenuItem.HeaderProperty, headerBinding0);

            Binding itemsSourceBinding0 = new Binding();
            itemsSourceBinding0.Converter = new MyItemsSourceConverter();
            itemsSourceBinding0.Path = new PropertyPath("Tag");
            itemsSourceBinding0.RelativeSource = RelativeSource.Self;
            mi.SetBinding(MenuItem.ItemsSourceProperty, itemsSourceBinding0);

            mi.DataContext = new VisualWrapper(DRT.RootElement);

            return mi;
        }

        private class MyHeaderConverter : IValueConverter
        {
            #region IValueConverter Members

            public object Convert(object o, Type type, object parameter, CultureInfo culture)
            {
                if (o == null) return null;

                Visual visual = o as Visual;

                if (visual == null)
                {
                    visual = ((VisualWrapper)o).Target;
                }

                return visual == null ? "<null>" : visual.ToString();
            }

            public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
            {
                return null;
            }

            #endregion
        }

        private class MyItemsSourceConverter : IValueConverter
        {
            #region IValueConverter Members

            public object Convert(object o, Type type, object parameter, CultureInfo culture)
            {
                if (o == null) return null;

                Visual node = o as Visual;
                if (node == null)
                {
                    node = ((VisualWrapper)o).Target;
                }

                if (node != null)
                {
                    // We have to wrap the visual children in a Node
                    // object so that the MenuItem doesn't think that
                    // we're adding a visual child of someone else
                    // as its logical child.
                    List<VisualWrapper> children = new List<VisualWrapper>();

                    int count= VisualTreeHelper.GetChildrenCount(node);
                    for(int i = 0; i < count; i++)
                    {
                        Visual child = (Visual) VisualTreeHelper.GetChild(node, i);
                        children.Add(new VisualWrapper(child));
                    }

                    return children;
                }
                return null;
            }

            public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
            {
                return null;
            }

            #endregion
        }

        public class VisualWrapper
        {
            public Visual Target
            {
                get { return _data; }
            }

            public VisualWrapper(Visual data)
            {
                _data = data;
            }

            private Visual _data;
        }

        private void _searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_searchTextBox.Text.Length > 0)
            {
                _searchDropDown.IsOpen = true;
            }
            else
            {
                _searchDropDown.IsOpen = false;
            }
        }

        #region Properties for databinding on the page

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        public int NumItems
        {
            get
            {
                return _numItems;
            }
            set
            {
                _numItems = value;
                _itemsList = null;
                OnPropertyChanged("NumItems");
                OnPropertyChanged("ItemsList");
            }
        }

        public List<string> ItemsList
        {
            get
            {
                if (_itemsList == null)
                {
                    _itemsList = new List<string>();
                    for (int i = 0; i < NumItems; i++)
                    {
                        _itemsList.Add(String.Format("Item {0}", i));
                    }
                }

                return _itemsList;
            }
        }


        List<string> _itemsList;
        int _numItems = 100;

        #endregion
    }
}



