// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Threading;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Automation;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Reflection;

namespace DRT
{
    public class AccessKeyManagerTestSuite : DrtTestSuite
    {
        public AccessKeyManagerTestSuite() : base("AccessKeyManager")
        {
            Contact = "Microsoft";
        }

        Button _b,_b2;
        StackPanel _sp;
        List<FrameworkElement> _elements = new List<FrameworkElement>();

        public override DrtTest[] PrepareTests()
        {
            Border border = new Border();
            border.Background = Brushes.White;

            _sp = new StackPanel();
            _sp.Orientation = System.Windows.Controls.Orientation.Horizontal;

            _b = new Button();
            _b2 = new Button();
            _sp.Children.Add(_b);
            _sp.Children.Add(_b2);

            border.Child = _sp;
            DRT.Show(border);

            return new DrtTest[]
            {
                new DrtTest(RegisterMultiple),
                new DrtTest(AccessKeyPropertySetup),
                new DrtTest(AccessKeyProperty),
                new DrtTest(Cleanup),
            };
        }

        private void RegisterMultiple()
        {
            AccessKeyManager.Register("a", _b);
            AccessKeyManager.Register("b", _b);
            DRT.Assert(AccessKeyManager.IsKeyRegistered(((DrtControls)DRT).MainWindow, "A"), "1. AKM: 'A' should be registered");
            DRT.Assert(AccessKeyManager.IsKeyRegistered(((DrtControls)DRT).MainWindow, "B"), "2. AKM: 'B' should be registered");
            AccessKeyManager.Unregister("b", _b);
            DRT.Assert(AccessKeyManager.IsKeyRegistered(null, "A"), "3. AKM: 'A' should be registered (passed null scope, make sure that null scope defaults to the active window -- bug 985541)");
            DRT.Assert(!AccessKeyManager.IsKeyRegistered(null, "B"), "4. AKM: 'B' should not be registered (passed null scope, make sure that null scope defaults to the active window -- bug 985541)");
            AccessKeyManager.Register("c", _b);
            DRT.Assert(AccessKeyManager.IsKeyRegistered(((DrtControls)DRT).MainWindow, "A"), "5. AKM: 'A' should be registered");
            DRT.Assert(!AccessKeyManager.IsKeyRegistered(((DrtControls)DRT).MainWindow, "B"), "6. AKM: 'B' should not be registered");
            DRT.Assert(AccessKeyManager.IsKeyRegistered(((DrtControls)DRT).MainWindow, "C"), "7. AKM: 'C' should be registered");

            AccessKeyManager.Unregister("a", _b);
            AccessKeyManager.Unregister("c", _b);
            DRT.Assert(!AccessKeyManager.IsKeyRegistered(((DrtControls)DRT).MainWindow, "A"), "8. AKM: 'A' should not be registered");
            DRT.Assert(!AccessKeyManager.IsKeyRegistered(((DrtControls)DRT).MainWindow, "B"), "9. AKM: 'B' should not be registered");
            DRT.Assert(!AccessKeyManager.IsKeyRegistered(((DrtControls)DRT).MainWindow, "C"), "10. AKM: 'C' should not be registered");

            AccessKeyManager.Register("a", _b);
            AccessKeyManager.Register("b", _b);
            AccessKeyManager.Unregister("a", _b);
            AccessKeyManager.Unregister("b", _b);
            DRT.Assert(!AccessKeyManager.IsKeyRegistered(((DrtControls)DRT).MainWindow, "A"), "11. AKM: 'A' should not be registered");
            DRT.Assert(!AccessKeyManager.IsKeyRegistered(((DrtControls)DRT).MainWindow, "B"), "12. AKM: 'B' should not be registered");

            AccessKeyManager.Register("a", _b);
            AccessKeyManager.Register("a", _b2);
            AccessKeyManager.Register("b", _b);
            AccessKeyManager.Register("b", _b2);
            AccessKeyManager.Unregister("a", _b2);
            AccessKeyManager.Unregister("b", _b);
            DRT.Assert(AccessKeyManager.IsKeyRegistered(((DrtControls)DRT).MainWindow, "A"), "13. AKM: 'A' should be registered");
            DRT.Assert(AccessKeyManager.IsKeyRegistered(((DrtControls)DRT).MainWindow, "B"), "14. AKM: 'B' should be registered");
            AccessKeyManager.Unregister("a", _b);
            AccessKeyManager.Unregister("b", _b2);
            DRT.Assert(!AccessKeyManager.IsKeyRegistered(((DrtControls)DRT).MainWindow, "A"), "15. AKM: 'A' should not be registered");
            DRT.Assert(!AccessKeyManager.IsKeyRegistered(((DrtControls)DRT).MainWindow, "B"), "16. AKM: 'B' should not be registered");

            AccessKeyManager.Unregister("d", _b);
        }

        string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        private void AccessKeyPropertySetup()
        {
            // Add a number of buttons with access keys and text elements with labels.
            for (int i = 0; i < 5; i++)
            {
                string currchar = _chars.Substring(i % _chars.Length, 1);
                Button button = new Button();
                button.MinWidth = 0;
                button.Content = "AK _" + currchar;
                button.Name = currchar;
                _sp.Children.Add(button);

                _elements.Add(button);
            }

            for (int i = 0; i < 5; i++)
            {
                string currchar = _chars.Substring(i % _chars.Length, 1);
                Label label = new Label();
                label.Content = "AK _" + currchar;

                TextBox tb = new TextBox();
                tb.Name = currchar;
                tb.Width = 20;
                tb.Height = 10;
                label.Target = tb;

                _sp.Children.Add(label);
                _sp.Children.Add(tb);

                _elements.Add(tb);
            }
        }

        private void AccessKeyProperty()
        {
            DateTime before = DateTime.Now;
            int num = 0;
            foreach (FrameworkElement fe in _elements)
            {
                DRT.AssertEqual(fe.Name, GetAccessKeyCharacter(fe), "AccessKeyCharacter retrieved didn't match expected");
                num++;
            }
            DateTime after = DateTime.Now;

            if (DRT.Verbose) Console.WriteLine("Time to get each AccessKeyChar: " + ((TimeSpan)(after - before)).TotalMilliseconds/num);
        }

        private string GetAccessKeyCharacter(DependencyObject d)
        {
            MethodInfo info = typeof(AccessKeyManager).GetMethod("InternalGetAccessKeyCharacter", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
            if (info == null) throw new Exception("Can't find AccessKeyManager.InternalGetAccessKeyCharacter method");
            return (string)info.Invoke(null, new object[] { d });
        }

        int _retryCount = 0;

        private void Cleanup()
        {
            _sp.Children.Clear();
            _elements.Clear();

            GC.Collect();

            string stillRegistered = String.Empty;
            for (int i = 0; i < _chars.Length; i++)
            {
                string curr = _chars.Substring(i, 1);
                if (AccessKeyManager.IsKeyRegistered(((DrtControls)DRT).MainWindow, curr))
                {
                    stillRegistered += curr;
                }
            }

            if (stillRegistered != String.Empty)
            {
                if (_retryCount++ < 5)
                {
                    Console.WriteLine("There are still keys registered: " + stillRegistered);
                    DRT.RepeatTest();
                }
                else
                {
                    DRT.Assert(false, "There are still keys registered: " + stillRegistered);
                }
            }
        }

    }

    public class KeyboardNavigationTestSuite : DrtTestSuite
    {
        public KeyboardNavigationTestSuite() : base("KeyboardNavigation")
        {
            Contact = "Microsoft";
        }


        private DispatcherTimer _timer, _statusTimer;
        private TestButton _button1;
        private Button _button2;

        private Button _button3;

        private Button _button4;

        private Button _button5;

        private CheckBox _checkbox;

        private RadioButton _radiobutton;

        private StackPanel _radiobuttonlist;

        private RepeatButton _repeatbutton;

        private Thumb _thumb;
        private Label _label1, _label2, _label3, _label4;
        private Button _button6;
        private TextBox _textbox;

        private TextBlock _statusText;

        private Menu _menu;


        public override DrtTest[] PrepareTests()
        {
            if (DRT.Verbose) Console.WriteLine("MostRecentInputDevice = " + InputManager.Current.MostRecentInputDevice);

            Brush Green = new SolidColorBrush(Color.FromArgb(0xb2, 0xcc, 0xff, 0xcc));

            // Build tree
            Border root = new Border();

            root.Background = Green;

            DockPanel dproot = new DockPanel();

            _statusText = new TextBlock();
            dproot.Background = Green;
            dproot.Children.Add(_statusText);
            DockPanel.SetDock(_statusText, Dock.Bottom);

            /*
            EnsureParserContext();

            /*
            Style menuItemStyle = new Style();

            ((System.Windows.Markup.IParseLiteralContent)(menuItemStyle)).Parse(new System.Xml.XmlTextReader(new System.IO.StringReader(@"
                <MenuItem BorderThickness=""2"" xmlns=""http://schemas.microsoft.com/2003/xaml""></MenuItem>
                <Style.Triggers xmlns=""http://schemas.microsoft.com/2003/xaml"">
                    <Trigger Property=""IsKeyboardFocused"" Value=""true"">
                        <Set PropertyPath=""BorderBrush"" Value=""Blue"" />
                    </Trigger>
                </Style.Triggers>
                ")), _pc);
            dproot.Resources = new ResourceDictionary();
            dproot.Resources.Add(typeof(MenuItem), menuItemStyle);
            */

            // Add menu
            _menu = new Menu();

            MenuItem menuItem, subMenuItem, subSubMenuItem;

            menuItem = new MenuItem();
            menuItem.Header = "_File";
            menuItem.Click += new RoutedEventHandler(OnMenuItemClick);
            menuItem.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnGotFocus);
            menuItem.TabIndex = 200;
            subMenuItem = new MenuItem();
            subMenuItem.Header = "_New";
            subMenuItem.Click += new RoutedEventHandler(OnMenuItemClick);
            subMenuItem.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnGotFocus);
            subMenuItem.TabIndex = 201;
            menuItem.Items.Add(subMenuItem);
            subMenuItem = new MenuItem();
            subMenuItem.Header = "_Nothing";
            subMenuItem.Click += new RoutedEventHandler(OnMenuItemClick);
            subMenuItem.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnGotFocus);
            subMenuItem.TabIndex = 202;
            menuItem.Items.Add(subMenuItem);
            subMenuItem = new MenuItem();
            subMenuItem.Header = "E_xit";
            subMenuItem.Click += new RoutedEventHandler(OnMenuItemClick);
            subMenuItem.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnGotFocus);
            subMenuItem.TabIndex = 203;
            menuItem.Items.Add(subMenuItem);
            _menu.Items.Add(menuItem);
            menuItem = new MenuItem();
            menuItem.Header = "_Help";
            menuItem.Click += new RoutedEventHandler(OnMenuItemClick);
            menuItem.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnGotFocus);
            menuItem.TabIndex = 300;
            subMenuItem = new MenuItem();
            subMenuItem.Header = "_Index";
            subMenuItem.Click += new RoutedEventHandler(OnMenuItemClick);
            subMenuItem.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnGotFocus);
            subMenuItem.TabIndex = 301;
            subSubMenuItem = new MenuItem();
            subSubMenuItem.Header = "_Thing 1";
            subSubMenuItem.Click += new RoutedEventHandler(OnMenuItemClick);
            subSubMenuItem.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnGotFocus);
            subSubMenuItem.TabIndex = 401;
            subMenuItem.Items.Add(subSubMenuItem);
            subSubMenuItem = new MenuItem();
            subSubMenuItem.Header = "_Thing 2";
            subSubMenuItem.Click += new RoutedEventHandler(OnMenuItemClick);
            subSubMenuItem.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnGotFocus);
            subSubMenuItem.TabIndex = 402;
            subMenuItem.Items.Add(subSubMenuItem);
            menuItem.Items.Add(subMenuItem);

            subMenuItem = new MenuItem();
            subMenuItem.Header = "_About...";
            subMenuItem.Click += new RoutedEventHandler(OnMenuItemClick);
            subMenuItem.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnGotFocus);
            subMenuItem.TabIndex = 302;
            menuItem.Items.Add(subMenuItem);
            _menu.Items.Add(menuItem);
            DockPanel.SetDock(_menu, Dock.Top);
            dproot.Children.Add(_menu);

            // Add content
            Canvas canvas = new Canvas();     // Returned root object
            canvas.Width = 500;
            canvas.Height = 500;

            KeyboardNavigation.SetTabNavigation(canvas, KeyboardNavigationMode.Cycle);
            canvas.SetValue(Control.TabIndexProperty, 1);
            dproot.Children.Add(canvas);
            _button1 = new TestButton();
            _button1.Click += new RoutedEventHandler(OnButtonClick);
            _button2 = new Button();
            _button3 = new Button();
            _button4 = new Button();
            _button5 = new Button();
            _label1 = new Label();
            _label2 = new Label();
            _button6 = new Button();
            _checkbox = new CheckBox();
            _radiobutton = new RadioButton();
            _repeatbutton = new RepeatButton();
            _thumb = new Thumb();
            _radiobuttonlist = new StackPanel();
            _textbox = new TextBox();

            RadioButton rb1 = new RadioButton();
            RadioButton rb2 = new RadioButton();
            RadioButton rb3 = new RadioButton();

            rb1.Content = "RadioButton 1";
            rb2.Content = "RadioButton 2";
            rb3.Content = "RadioButton 3";
            _radiobuttonlist.Children.Add(rb1);
            _radiobuttonlist.Children.Add(rb2);
            _radiobuttonlist.Children.Add(rb3);
            _repeatbutton.TabIndex = 200;
            _radiobutton.TabIndex = 200;
            KeyboardNavigation.SetTabIndex(_radiobuttonlist, 200);
            KeyboardNavigation.SetTabNavigation(_radiobuttonlist, KeyboardNavigationMode.Once);
            KeyboardNavigation.SetDirectionalNavigation(_radiobuttonlist, KeyboardNavigationMode.Cycle);
            _thumb.TabIndex = 200;
            _button1.TabIndex = 1;
            _button2.TabIndex = 3;
            _button3.TabIndex = 2;
            _button4.TabIndex = 4;
            _button4.Focusable = false;
            _button5.TabIndex = 5;
            _button6.TabIndex = 6;
            _checkbox.TabIndex = 100;
            _checkbox.IsThreeState = true;
            _button1.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnGotFocus);
            _button2.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnGotFocus);
            _button3.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnGotFocus);
            _button4.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnGotFocus);
            _button5.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnGotFocus);
            _button6.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnGotFocus);
            _button6.Click += new RoutedEventHandler(OnButton6Click);
            canvas.Children.Add(_button1);
            canvas.Children.Add(_button2);
            canvas.Children.Add(_button3);
            canvas.Children.Add(_button4);
            canvas.Children.Add(_button5);
            canvas.Children.Add(_checkbox);
            canvas.Children.Add(_radiobutton);
            canvas.Children.Add(_repeatbutton);
            canvas.Children.Add(_radiobuttonlist);
            canvas.Children.Add(_thumb);
            Canvas.SetLeft(_button1, 0);
            Canvas.SetTop(_button1, 0);

            //_button1.Content = "Button1";
            _button1.Content = "_a";
            Canvas.SetLeft(_button2, 0);
            Canvas.SetTop(_button2, 30);
            _button2.Content = "_b";
            Canvas.SetLeft(_button3, 0);
            Canvas.SetTop(_button3, 60);
            _button3.Content = "_b";
            Canvas.SetLeft(_button4, 0);
            Canvas.SetTop(_button4, 90);
            _button4.Content = "Button4 nonFocusable";
            Canvas.SetLeft(_button5, 0);
            Canvas.SetTop(_button5, 120);
            _button5.Content = "Button5";

            StackPanel sp = new StackPanel();
            sp.Orientation = System.Windows.Controls.Orientation.Horizontal;

            sp.Children.Add(_label1);
            sp.Children.Add(_button6);
            canvas.Children.Add(sp);
            Canvas.SetLeft(sp, 0);
            Canvas.SetTop(sp, 350);
            _label1.Target = _button6;
            _label1.Content = "_Label:";
            _button6.Content = "I have a label";

            DependencyProperty LabeledByProperty = DependencyPropertyFromName("LabeledByProperty", typeof(Label));
            DRT.Assert(_button6.GetValue(LabeledByProperty) == _label1, "_button6 should be LabeledBy _label1");

            sp.Children.Add(_label2);
            sp.Children.Add(_textbox);
            _label2.Target = _textbox;
            _label2.Content = "_Other label:";
            _textbox.Text = "hi";
            _textbox.Width = 100;
            _textbox.Height = 20;

            DRT.Assert(_textbox.GetValue(LabeledByProperty) == _label2, "_textbox should be LabeledBy _label2");

            Label hiddenLabel = new Label();
            hiddenLabel.Target = _textbox;

            DRT.Assert(_textbox.GetValue(LabeledByProperty) == hiddenLabel, "_textbox should be LabeledBy hiddenLabel");

            _label2.Target = null;
            _label2.Target = _textbox;
            hiddenLabel.Target = null;

            DRT.Assert(_textbox.GetValue(LabeledByProperty) == _label2, "_textbox should be LabeledBy _label2 (second time)");


            _label3 = new Label();
            sp.Children.Add(_label3);
            _label4 = new Label();
            sp.Children.Add(_label4);

            _label3.Content = "underlined";

            ((IAddChild)_label4).AddChild("text1");

            Canvas.SetLeft(_checkbox, 200);
            Canvas.SetTop(_checkbox, 20);
            _checkbox.Content = "CheckBox";
            Canvas.SetLeft(_radiobutton, 200);
            Canvas.SetTop(_radiobutton, 50);
            _radiobutton.Content = "RadioButton";
            Canvas.SetLeft(_repeatbutton, 200);
            Canvas.SetTop(_repeatbutton, 80);
            _repeatbutton.Content = "RepeatButton";
            _repeatbutton.Click += new RoutedEventHandler(OnButtonClick);
            Canvas.SetLeft(_radiobuttonlist, 200);
            Canvas.SetTop(_radiobuttonlist, 120);
            Canvas.SetLeft(_thumb, 200);
            Canvas.SetTop(_thumb, 170);
            _thumb.DragDelta += new DragDeltaEventHandler(OnDrag);
            DRT.Show(dproot);
            _statusTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _statusTimer.Tick += new EventHandler(UpdateStatus);
            _statusTimer.Interval = TimeSpan.FromMilliseconds(100);
            _statusTimer.Start();
            if (!DRT.KeepAlive)
            {
                _timer = new DispatcherTimer(DispatcherPriority.Normal);
                _timer.Tick += new EventHandler(OnTimeout);
                _timer.Interval = TimeSpan.FromMilliseconds(30000);
                _timer.Start();
            }

            if (!DRT.KeepAlive)
            {
                return new DrtTest[] {
                        new DrtTest(Start),
                        new DrtTest(ClickToBringToFront),
                        new DrtTest(MoveMouseAway),
                        new DrtTest(VerifyLabelUnderline),
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // tab
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // tab
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // tab
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // tab
                        new DrtTest(PressKey), new DrtTest(VerifyClick), // a
//                         new DrtTest(VerifyAccessKeyEvent),
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // b
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // b
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // l
                        /*
                        Menu looks like this:
                        &File[200] ---- &Help[300]
                          |               |
                        &New[201]       &Index[301]   --> &Thing 1 [401]
                        &Nothing[202]   &About...[302]    &Thing 2 [402]
                        E&xit[203]
                        */
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // ALT-H (go to help)
                        new DrtTest(PressKey), new DrtTest(VerifyMenuClick), // A (go to About)
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // ALT-F (go to File -> focus ends up on "New")
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // N  (go to "Nothing")
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // N  (go to "New")
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // ESC (close file menu)
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // ESC (exit menu)
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // ALT-F (goes to File)
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // RIGHT (Opens Help submenu and focuses "Index")
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // RIGHT (opens index's subsubmenu)
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // LEFT  (go back to "Index")
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // RIGHT (opens index's subsubmenu again)
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // RIGHT (should cycle back to "File" and opens that submenu)
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // DOWN (go to "Nothing")
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // DOWN (go to "Exit")
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // DOWN (should cycle back to "New")
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // UP   (go back to "Exit")
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // LEFT (should cycle back to "Help" menu with "Index" focused)
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // LEFT (should cycle back to "File" menu with "New" focused)
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // ALT  (collapse menu and return focus to button)
                        new DrtTest(Cleanup)
                };
            }
            else
            {
                return new DrtTest[] { };
            }
        }

        private DependencyProperty DependencyPropertyFromName(string propertyName, Type propertyType)
        {
            FieldInfo fi = propertyType.GetField(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            return (fi != null) ? fi.GetValue(null) as DependencyProperty : null;
        }

        private void Start()
        {
        }

        private void ClickToBringToFront()
        {
            DRT.MoveMouse(DRT.RootElement as UIElement, 0.0, 0.9);
            DRT.ClickMouse();
        }

        private void MoveMouseAway()
        {
            // The mouse will cause focus problems for the menus. 
            Input.MoveTo(new Point(0, 0));
        }


        private void Cleanup()
        {
            //if (_statusTimer != null) _statusTimer.Stop();
            if (_timer != null) _timer.Stop();
        }

        private void VerifyLabelUnderline()
        {
            TextBlock labelText;
            

            // Label3 should have its text element underlined
            labelText = FindElement<TextBlock>(_label3);

            DRT.Assert(labelText != null, "Couldn't find Text element in Label3");
        }

        private T FindElement<T>(DependencyObject visual) where T : Visual
        {
            if (visual == null) return default(T);
            int count = VisualTreeHelper.GetChildrenCount(visual);
            for(int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(visual,i);
                if (child is T) return (T)child;

                T temp = FindElement<T>(child);
                if (temp != null) return temp;
            }
            
            return null;
        }

        private void PressKey()
        {
            _lastClickedMenuItem = "";
            if (DRT.Verbose) Console.WriteLine("Send key [" + _count + "] = " + _keys[_count]);

            if (_keys[_count] is Key)
            {
                DRT.SendKeyboardInput((Key)_keys[_count], true);
                DRT.SendKeyboardInput((Key)_keys[_count], false);
            }
            if (_keys[_count] is KeyCombo)
            {
                KeyCombo combo = (KeyCombo)_keys[_count];
                DRT.SendKeyboardInput(combo.First, true);
                DRT.SendKeyboardInput(combo.Second, true);
                DRT.SendKeyboardInput(combo.Second, false);
                DRT.SendKeyboardInput(combo.First, false);
            }
            _count++;
        }


        private void VerifyFocus()
        {

            if(Keyboard.FocusedElement != null)
            {
                Control c = Keyboard.FocusedElement as Control;

                DRT.Assert(c != null, "Focused element was not a control");
                DRT.Assert(_focusedSequence[_count] == c.TabIndex, "Step " + _count + ", expect button with TabIndex = " + _focusedSequence[_count] + ", was " + c.TabIndex);
            }
            else
            {
                DRT.Assert(_focusedSequence[_count] == -1, "Aack");
            }
        }


        private void VerifyClick()
        {
            if (_count > 0 && _count < _focusedSequence.Length)
            {
                DRT.Assert(_count == 5, "Click out of sequence, count was " + _count);
                DRT.Assert(_buttonCount > 0, "Key press did not invoke button 1");
            }
        }


        private void VerifyMenuClick()
        {
            if (_count > 0 && _count < _focusedSequence.Length)
            {
                DRT.AssertEqual("_About...", _lastClickedMenuItem, "Wrong menu item was clicked");
            }
        }

        private void VerifyAccessKeyEvent()
        {
            if (DRT.Verbose) Console.WriteLine("Button was invoked with key " + _button1.invokedAccessKey);
            DRT.Assert(_button1.invokedAccessKey != null, "AccessKeyEventArgs to button1 did not include the key");
            DRT.Assert(_button1.invokedAccessKey == "A", "AccessKeyEventArgs to button1 did not include the correct key (should have been \"A\")");
        }

        private void OnTimeout(object sender, EventArgs e)
        {
            if (_count <= 1)
            {
                DRT.Assert(false, "WARNING:Timeout expired. No input detected. Window should be active to test the keyboard input.");
            }
            else
            {
                DRT.Assert(false, "Keyboard navigation failed.");
            }
        }


        private System.Windows.Markup.ParserContext _pc;


        private void EnsureParserContext()
        {
            _pc = new System.Windows.Markup.ParserContext();
            _pc.XamlTypeMapper = System.Windows.Markup.XamlTypeMapper.DefaultMapper;
        }

        private struct KeyCombo
        {
            public KeyCombo(Key first, Key second)
            {
                First = first;
                Second = second;
            }
            public Key First;
            public Key Second;
            public override string ToString()
            {
                return "(" + First + ", " + Second + ")";
            }
        }

        private int[] _focusedSequence = new int[] {0, 1, 2, 3, 5, 100, 3, 2, 6, 301, 0 /* menu click */, 201, 202, 201, 200, 6,
                /* #16 */ 201, 301, 401, 301, 401, 201,
                /* #22 */ 202, 203, 201, 203, 301, 201, 6 };
        private object[] _keys = new object[] {Key.Tab, Key.Tab, Key.Tab, Key.Tab, Key.A, Key.B, Key.B, Key.L,
                new KeyCombo(Key.LeftAlt, Key.H), Key.A, new KeyCombo(Key.LeftAlt, Key.F), Key.N, Key.N, Key.Escape, Key.Escape,
                new KeyCombo(Key.LeftAlt, Key.F), Key.Right, Key.Right, Key.Left, Key.Right, Key.Right,
                Key.Down, Key.Down, Key.Down, Key.Up, Key.Left, Key.Left, Key.LeftAlt };

        private int _count = 0;

        private int _buttonCount = 0;


        private void OnGotFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Control c = e.Source as Control;

            if (c == null)
                return;

            if (DRT.Verbose) Console.WriteLine("Got focus:" + c + " TabIndex=" + c.TabIndex);
            if (DRT.Verbose) Console.WriteLine("MostRecentInputDevice = " + InputManager.Current.MostRecentInputDevice);
        }


        public void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (DRT.Verbose) Console.WriteLine("Click Event");

            _buttonCount++;
        }


        private int _button6ClickCount = 1;


        public void OnButton6Click(object sender, RoutedEventArgs e)
        {
            _button6.Content = "I was clicked " + _button6ClickCount++ + " times";
            _button6.Focus();
        }


        public void OnDrag(object sender, DragDeltaEventArgs e)
        {
            double x = Canvas.GetLeft(_thumb);
            double y = Canvas.GetTop(_thumb);

            Canvas.SetLeft(_thumb, x + e.HorizontalChange);
            Canvas.SetTop(_thumb, y + e.VerticalChange);
        }


        private void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            string t = ((MenuItem)e.Source).Header as string;

            if (t != null)
            {
                _lastClickedMenuItem = t;
                if (DRT.Verbose) Console.WriteLine("Clicked MenuItem: " + _lastClickedMenuItem);
            }
            else
                _lastClickedMenuItem = "Not text";
        }


        string _lastClickedMenuItem = "";


        private void UpdateStatus(object sender, EventArgs e)
        {
            FrameworkElement focus = Keyboard.FocusedElement as FrameworkElement;

            if (focus != null)
            {
                _statusText.Text = "" + focus.GetType() + ":" + focus.Name + "," + focus.GetValue(Control.TabIndexProperty);
            }
            else
            {
                _statusText.Text = "Focus is not on a FrameworkElement";
            }
        }

        private class TestButton : Button
        {
            public string invokedAccessKey;
            protected override void OnAccessKey(AccessKeyEventArgs e)
            {
                base.OnAccessKey(e);
                invokedAccessKey = e.Key;
            }
        }
    }

    public class DefaultButtonsSuite: DrtTestSuite
    {
        public DefaultButtonsSuite() : base("DefaultButtons")
        {
            Contact = "Microsoft";
        }

        Button _ok, _cancel, _ok2, _cancel2;
        TextBox _textBox, _textBox2;
        Hashtable _clicks = new Hashtable();
        ListBox _chooseOK1, _chooseOK2, _chooseCancel1, _chooseCancel2;

        public override DrtTest[] PrepareTests()
        {
            DockPanel rootDP = new DockPanel();
            rootDP.Width = 500;
            rootDP.Height = 500;

            Canvas rootCanvas = new Canvas();
            rootCanvas.Width = 500;
            rootCanvas.Height = 500;
            rootCanvas.Background = Brushes.BlanchedAlmond;

            _ok = new Button();
            _ok.Content = "OK";
            _ok.Width = 75;

            _cancel = new Button();
            _cancel.Content = "Cancel";
            _cancel.Width = 75;

            _ok2 = new Button();
            _ok2.Content = "OK2";
            _ok2.Width = 75;

            _cancel2 = new Button();
            _cancel2.Content = "Cancel2";
            _cancel2.Width = 75;

            Canvas.SetBottom(_cancel, 10);
            Canvas.SetBottom(_ok, 10);
            Canvas.SetBottom(_cancel2, 50);
            Canvas.SetBottom(_ok2, 50);
            Canvas.SetRight(_cancel, 10);
            Canvas.SetRight(_ok, 10 + 75 + 10);
            Canvas.SetRight(_cancel2, 10);
            Canvas.SetRight(_ok2, 10 + 75 + 10);

            StackPanel sp = new StackPanel();
            sp.Orientation = System.Windows.Controls.Orientation.Horizontal;
            _textBox = new TextBox();
            _textBox.Width = 75;
            _textBox.Height = 20;
            sp.Children.Add(_textBox);

            _textBox2 = new TextBox();
            _textBox2.Width = 100;
            _textBox2.Height = 100;
            _textBox2.AcceptsReturn = true;
            sp.Children.Add(_textBox2);

            ListBox lb = new ListBox();
            lb.Items.Add("1");
            lb.Items.Add("2");
            lb.Items.Add("3");
            lb.Items.Add("4");
            lb.Items.Add("5");
            lb.Width = 50;
            lb.Margin = new Thickness(10);
            lb.Height = 200;

            sp.Children.Add(lb);

            DockPanel choices = new DockPanel();
            DockPanel dp1;
            Label label;

            dp1 = new DockPanel();
            label = new Label();
            label.Content = "OK:";
            label.Width = 60;
            dp1.Children.Add(label);
            _chooseOK1 = new ListBox();
            _chooseOK1.SelectionMode = SelectionMode.Multiple;
            _chooseOK1.Items.Add("IsDefault");
            _chooseOK1.Items.Add("IsCancel");
            dp1.Children.Add(_chooseOK1);
            DockPanel.SetDock(dp1, Dock.Top);
            choices.Children.Add(dp1);

            dp1 = new DockPanel();
            label = new Label();
            label.Content = "OK2:";
            label.Width = 60;
            dp1.Children.Add(label);
            _chooseOK2 = new ListBox();
            _chooseOK2.SelectionMode = SelectionMode.Multiple;
            _chooseOK2.Items.Add("IsDefault");
            _chooseOK2.Items.Add("IsCancel");
            dp1.Children.Add(_chooseOK2);
            DockPanel.SetDock(dp1, Dock.Top);
            choices.Children.Add(dp1);

            dp1 = new DockPanel();
            label = new Label();
            label.Content = "Cancel1:";
            label.Width = 60;
            dp1.Children.Add(label);
            _chooseCancel1 = new ListBox();
            _chooseCancel1.SelectionMode = SelectionMode.Multiple;
            _chooseCancel1.Items.Add("IsDefault");
            _chooseCancel1.Items.Add("IsCancel");
            dp1.Children.Add(_chooseCancel1);
            DockPanel.SetDock(dp1, Dock.Top);
            choices.Children.Add(dp1);

            dp1 = new DockPanel();
            label = new Label();
            label.Content = "Cancel2:";
            label.Width = 60;
            dp1.Children.Add(label);
            _chooseCancel2 = new ListBox();
            _chooseCancel2.SelectionMode = SelectionMode.Multiple;
            _chooseCancel2.Items.Add("IsDefault");
            _chooseCancel2.Items.Add("IsCancel");
            dp1.Children.Add(_chooseCancel2);
            DockPanel.SetDock(dp1, Dock.Top);
            choices.Children.Add(dp1);

            sp.Children.Add(choices);

            Border b = new Border();
            b.Child = sp;
            b.BorderThickness = new Thickness(1);
            b.BorderBrush = Brushes.Blue;

            Canvas.SetTop(b, 20);
            Canvas.SetLeft(b, 20);

            rootCanvas.Children.Add(b);
            rootCanvas.Children.Add(_ok);
            rootCanvas.Children.Add(_cancel);
            rootCanvas.Children.Add(_ok2);
            rootCanvas.Children.Add(_cancel2);

            _ok.Click += new RoutedEventHandler(ButtonClick);
            _cancel.Click += new RoutedEventHandler(ButtonClick);
            _ok2.Click += new RoutedEventHandler(ButtonClick);
            _cancel2.Click += new RoutedEventHandler(ButtonClick);

            _chooseOK1.SelectionChanged += new SelectionChangedEventHandler(SelectionChanged);
            _chooseOK2.SelectionChanged += new SelectionChangedEventHandler(SelectionChanged);
            _chooseCancel1.SelectionChanged += new SelectionChangedEventHandler(SelectionChanged);
            _chooseCancel2.SelectionChanged += new SelectionChangedEventHandler(SelectionChanged);

            _chooseOK1.SelectedItems.Add("IsDefault");
            _chooseCancel1.SelectedItems.Add("IsCancel");

            Brush greenBrush = Brushes.LightGreen.Clone();
            greenBrush.Opacity = 0.5;
            greenBrush.Freeze();

            Brush redBrush = Brushes.Pink.Clone();
            redBrush.Opacity = 0.5;
            redBrush.Freeze();

            Style myButtonStyle = new Style(typeof(Button), _ok.Style);
            Trigger trigger;
            trigger = new Trigger();
            trigger.Property = Button.IsDefaultedProperty;
            trigger.Value = true;
            trigger.Setters.Add(new Setter(Button.BackgroundProperty, greenBrush));
            myButtonStyle.Triggers.Add(trigger);

            trigger = new Trigger();
            trigger.Property = Button.IsCancelProperty;
            trigger.Value = true;
            trigger.Setters.Add(new Setter(Button.BackgroundProperty, redBrush));
            myButtonStyle.Triggers.Add(trigger);

            MultiTrigger multitrigger = new MultiTrigger();
            System.Windows.Condition condition = new System.Windows.Condition();
            condition.Property = Button.IsKeyboardFocusedProperty;
            condition.Value = true;
            multitrigger.Conditions.Add(condition);
            condition = new System.Windows.Condition();
            condition.Property = Button.IsDefaultProperty;
            condition.Value = true;
            multitrigger.Conditions.Add(condition);
            multitrigger.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.LightGreen));
            myButtonStyle.Triggers.Add(multitrigger);

            _ok.Style = myButtonStyle;
            _cancel.Style = myButtonStyle;
            _ok2.Style = myButtonStyle;
            _cancel2.Style = myButtonStyle;

            rootDP.Children.Add(rootCanvas);

            rootCanvas.TextInput += new TextCompositionEventHandler(OnTextInput);

            DRT.Show(rootDP);

            if (!DRT.KeepAlive)
            {
                return new DrtTest[]
                {
                    new DrtTest(Start),
                    new DrtTest(FocusFirst),
                    new DrtTest(PressEnter),
                    new DrtTest(VerifyOK),
                    new DrtTest(PressEscape),
                    new DrtTest(VerifyCancel),
                    new DrtTest(FocusSingleLineTextBox),
                    new DrtTest(VerifyIsDefaulted),
                    new DrtTest(FocusMultiLineTextBox),
                    new DrtTest(VerifyNotIsDefaulted),
                    new DrtTest(FocusSingleLineTextBox),
                    new DrtTest(VerifyIsDefaulted),
                    new DrtTest(FocusOK2Button),
                    new DrtTest(VerifyNotIsDefaulted),
                    new DrtTest(FocusSingleLineTextBox),
                    new DrtTest(DisableOKButton),
                    new DrtTest(VerifyNotIsDefaulted),
                    new DrtTest(EnableOKButton),
                    new DrtTest(VerifyIsDefaulted),
                    new DrtTest(Cleanup),
                };
            }
            else
            {
                return new DrtTest[] { new DrtTest(FocusFirst) };
            }
        }

        private void Start()
        {
        }

        private void FocusFirst()
        {
            _textBox.Focus();
        }

        private void PressEnter()
        {
            DRT.SendKeyboardInput(Key.Enter, true);
            DRT.SendKeyboardInput(Key.Enter, false);
        }

        private void PressEscape()
        {
            DRT.SendKeyboardInput(Key.Escape, true);
            DRT.SendKeyboardInput(Key.Escape, false);
        }

        private void VerifyOK()
        {
            DRT.Assert(_clicks[_ok] != null && (int)_clicks[_ok] == 1, "OK button should have been clicked once by pressing Enter");
        }

        private void VerifyCancel()
        {
            DRT.Assert(_clicks[_cancel] != null && (int)_clicks[_cancel] == 1, "Cancel button should have been clicked once by pressing Escape");
        }

        private void FocusSingleLineTextBox() { _textBox.Focus(); /*DRT.Assert(_textBox.Focus(), "Could not focus text box 1");*/ }
        private void FocusMultiLineTextBox() { _textBox2.Focus(); /*DRT.Assert(_textBox2.Focus(), "Could not focus text box 2");*/ }
        private void VerifyIsDefaulted() { DRT.Assert(_ok.IsDefaulted, "OK should be IsDefaulted = true."); }
        private void VerifyNotIsDefaulted() { DRT.Assert(!_ok.IsDefaulted, "OK should be IsDefaulted = false."); }
        private void FocusOK2Button() { DRT.Assert(_ok2.Focus(), "Could not focus OK2 button"); }
        private void DisableOKButton() { _ok.IsEnabled = false; }
        private void EnableOKButton() { _ok.IsEnabled = true; }


        private void Cleanup()
        {
        }


        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            if (_clicks[sender] == null) _clicks[sender] = 0;
            _clicks[sender] = (int)_clicks[sender] + 1;

            Button b = sender as Button;
            string[] temp = (b.Content as string).Split(' ');
            string orig = temp[0];

            b.Content = orig + " [" + _clicks[sender] + "]";
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Button b = null;
            ListBox lb = sender as ListBox;
            if (lb == _chooseOK1) b = _ok;
            if (lb == _chooseOK2) b = _ok2;
            if (lb == _chooseCancel1) b = _cancel;
            if (lb == _chooseCancel2) b = _cancel2;
            b.IsDefault = lb.SelectedItems.Contains("IsDefault");
            b.IsCancel = lb.SelectedItems.Contains("IsCancel");
        }


        private void OnTextInput(object sender, TextCompositionEventArgs e)
        {
            if (DRT.Verbose)
            {
                Console.WriteLine("Text Input: {0}", e.Text);
            }
        }
    }

    public class TabAndDirectionalNavigationTestSuite : DrtTestSuite
    {
        public TabAndDirectionalNavigationTestSuite() : base("TabAndDirectionalNavigation")
        {
            Contact = "Microsoft";
        }

        private DispatcherTimer _timer;

        private Canvas _canvas;
        private Button _buttonBrowse;
        private Button _buttonOK;
        private Button _buttonCancel;
        private CheckBox _checkbox;
        private ListBox _listbox;
        private ListBoxItem _listitem1;
        private ListBoxItem _listitem2;
        private ListBoxItem _listitem3;
        private ListBoxItem _listitem4;
        private ListBoxItem _listitem5;

        public override DrtTest[] PrepareTests()
        {
            if (DRT.Verbose) Console.WriteLine("MostRecentInputDevice = " + InputManager.Current.MostRecentInputDevice);

            // Create elements
            _canvas = new Canvas();
            _checkbox = new CheckBox();
            _buttonBrowse = new Button();
            _buttonOK = new Button();
            _buttonCancel = new Button();
            _listbox = new ListBox();
            _listitem1 = new ListBoxItem();
            _listitem2 = new ListBoxItem();
            _listitem3 = new ListBoxItem();
            _listitem4 = new ListBoxItem();
            _listitem5 = new ListBoxItem();

            // Build tree
            Border b = new Border();
            DRT.Show(b);
            b.Child = _canvas;
            _canvas.Width = 200;
            _canvas.Height = 200;

            _canvas.Children.Add(_checkbox);
            _canvas.Children.Add(_buttonBrowse);
            _canvas.Children.Add(_buttonOK);
            _canvas.Children.Add(_buttonCancel);
            _canvas.Children.Add(_listbox);
            _listbox.Items.Add(_listitem1);
            _listbox.Items.Add(_listitem2);
            _listbox.Items.Add(_listitem3);
            _listbox.Items.Add(_listitem4);
            _listbox.Items.Add(_listitem5);

            // Set properties
            _canvas.Background = Brushes.Yellow;
            KeyboardNavigation.SetTabNavigation(_canvas, KeyboardNavigationMode.Cycle);

            _checkbox.Content = "CheckBox";
            _checkbox.Background = Brushes.Green;
            _checkbox.TabIndex = 1;
            SetLocation(_checkbox, 10, 10, 100, 20);

            _buttonBrowse.Content = "Browse";
            _buttonBrowse.TabIndex = 3;
            SetLocation(_buttonBrowse, 120, 10, 50, 20);

            _buttonOK.Content = "OK";
            _buttonOK.TabIndex = 4;
            SetLocation(_buttonOK, 120, 120, 50, 20);

            _buttonCancel.Content = "Cancel";
            _buttonCancel.TabIndex = 5;
            SetLocation(_buttonCancel, 120, 90, 50, 20);

            _listbox.TabIndex = 2;
            SetLocation(_listbox, 10, 40, 100, 120);

            _listitem1.Content = "List Item 1";
            _listitem2.Content = "List Item 2";
            _listitem3.Content = "List Item 3";
            _listitem4.Content = "List Item 4";
            _listitem5.Content = "List Item 5";
            _listitem1.TabIndex = 11;
            _listitem2.TabIndex = 12;
            _listitem3.TabIndex = 13;
            _listitem4.TabIndex = 14;
            _listitem5.TabIndex = 15;

            if (!DRT.KeepAlive)
            {
                _timer = new DispatcherTimer(DispatcherPriority.Normal);
                _timer.Tick += new EventHandler(OnTimeout);
                _timer.Interval = TimeSpan.FromMilliseconds(30000);
                _timer.Start();
            }

            if (!DRT.KeepAlive)
            {
                return new DrtTest[] {
                    new DrtTest(Start),
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // tab
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // tab
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // tab
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // tab
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // tab

                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // shift+tab
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // shift+tab
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // shift+tab
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // shift+tab
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // shift+tab

                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // right
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // right
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // down
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // down
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // down
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // up
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // up
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // up
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // left
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // left

                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // down
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // down
                        new DrtTest(PressKey), new DrtTest(VerifyFocus), // down

                        new DrtTest(Cleanup)
                };
            }
            else
            {
                return new DrtTest[] { };
            }
        }

        private void SetLocation(Control control, int x, int y, int width, int height)
        {
            Canvas.SetLeft(control, x);
            Canvas.SetTop(control, y);
            control.Width = width;
            control.Height = height;
        }

        private void Start()
        {
            _checkbox.Focus();

            if (!DRT.KeepAlive)
            {
                // The mouse will cause focus problems for the menus. 
                Input.MoveTo(new Point(0, 0));
            }
        }

        private void Cleanup()
        {
            //if (_statusTimer != null) _statusTimer.Stop();
            if (_timer != null) _timer.Stop();
        }

        private void PressKey()
        {
            if (DRT.Verbose) Console.WriteLine("Send key [" + _count + "] = " + _keys[_count]);

            if (_keys[_count] is Key)
            {
                DRT.SendKeyboardInput((Key)_keys[_count], true);
                DRT.SendKeyboardInput((Key)_keys[_count], false);
            }

            if (_keys[_count] is KeyCombo)
            {
                KeyCombo combo = (KeyCombo)_keys[_count];

                DRT.SendKeyboardInput(combo.First, true);
                DRT.SendKeyboardInput(combo.Second, true);
                DRT.SendKeyboardInput(combo.Second, false);
                DRT.SendKeyboardInput(combo.First, false);
            }

            _count++;
        }

        private void VerifyFocus()
        {
            Control c = Keyboard.FocusedElement as Control;

            DRT.Assert(c != null, "Focused element was not a control");
            DRT.Assert(_focusedSequence[_count] == c.TabIndex, "Step " + _count + ", expect button with TabIndex = " + _focusedSequence[_count] + ", was " + c.TabIndex + " Control:"+c);
        }

        private void VerifyClick()
        {
            if (_count > 0 && _count < _focusedSequence.Length)
            {
                DRT.Assert(_count == 5, "Click out of sequence, count was " + _count);
            }
        }

        private void OnTimeout(object sender, EventArgs e)
        {
            if (_count <= 1)
            {
                DRT.Assert(false, "WARNING:Timeout expired. No input detected. Window should be active to test the keyboard input.");
            }
            else
            {
                DRT.Assert(false, "Keyboard navigation failed.");
            }
        }

        private struct KeyCombo
        {
            public KeyCombo(Key first, Key second)
            {
                First = first;
                Second = second;
            }

            public Key First;
            public Key Second;

            public override string ToString()
            {
                return "(" + First + ", " + Second + ")";
            }
        }

        private int[] _focusedSequence = new int[] {
            0, 11, 3, 4, 5, 1, // Tab thru all elements
            5, 4, 3, 11, 1, // Shift+Tab thru all elements
            3, 3, 5, 4, 4, 5, 3, 3, 1, 1,  // Directional navigation
            11, 12, 13,

        };

        private static KeyCombo s_shiftTab = new KeyCombo(Key.LeftShift, Key.Tab);

        private object[] _keys = new object[] {
            Key.Tab, Key.Tab, Key.Tab, Key.Tab, Key.Tab, // Tab thru all elements
            s_shiftTab, s_shiftTab, s_shiftTab, s_shiftTab, s_shiftTab, // Shift+Tab thru all elements
            Key.Right, Key.Right, Key.Down, Key.Down, Key.Down, Key.Up, Key.Up, Key.Up, Key.Left, Key.Left, // Directional navigation
            Key.Down, Key.Down, Key.Down,
        };

        private int _count = 0;
    }


    public class PrimaryTextSuite : DrtTestSuite
    {
        public PrimaryTextSuite() : base("PrimaryText")
        {
            Contact = "Microsoft";
        }

        Button _b;
        HeaderedContentControl _hcc;
        HeaderedItemsControl _hic;
        TextBlock _text;
        TextBox _textBox;

        public override DrtTest[] PrepareTests()
        {
            StackPanel sp = new StackPanel();
            sp.Orientation = System.Windows.Controls.Orientation.Horizontal;
            sp.Background = Brushes.White;

            DRT.Show(sp);

            _hcc = new HeaderedContentControl();
            _hcc.Header = "header";

            sp.Children.Add(_hcc);

            _b = new Button();
            _text = new TextBlock();
            ((IAddChild)_b).AddChild(_text);
            ((IAddChild)_text).AddText("a");
            ((IAddChild)_text).AddText("b");
            ((IAddChild)_text).AddText("c");

            sp.Children.Add(_b);

            _hic = new HeaderedItemsControl();
            _hic.Header = "def";

            sp.Children.Add(_hic);

            _textBox = new TextBox();
            _textBox.Text = "textbox";

            sp.Children.Add(_textBox);

            _text = new TextBlock();
            _text.Inlines.Add(new Run("text"));

            sp.Children.Add(_text);

            if (!DRT.KeepAlive)
            {
                return new DrtTest[]
                {
                    new DrtTest(Verify),
                };
            }
            else
            {
                return new DrtTest[] {};
            }
        }

        private void Verify()
        {
            DRT.Assert(InternalGetPrimaryText(_hcc) == "header", "HeaderedContentControl was not 'header'");
            DRT.Assert(InternalGetPrimaryText(_b) == "abc", "Button was not 'abc'");
            DRT.Assert(InternalGetPrimaryText(_hic) == "def", "HeaderedItemsControl was not 'def'");
            DRT.Assert(InternalGetPrimaryText(_textBox) == "textbox", "TextBox was not 'textBox'");
            DRT.Assert(InternalGetPrimaryText(_text) == "text", "Text was not 'text'");

            _b.SetValue(TextSearch.TextProperty, "override");
            DRT.Assert(InternalGetPrimaryText(_b) == "override", "Button was not 'override'");
        }

        private string InternalGetPrimaryText(FrameworkElement element)
        {
            return (string)CallInternal("GetPrimaryText", typeof(TextSearch), null, typeof(FrameworkElement), element);
        }

        private object CallInternal(string name, Type t, object instance, Type param, object arg)
        {
            MethodInfo info = t.GetMethod(name,
                       BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, new Type[] { param }, null);

            if (info == null)
            {
                info = t.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
            }

            DRT.Assert(info != null, "Couldn't find internal method " + t + "." + name);
            if (arg != null)
            {
                return info.Invoke(instance, new object[] { arg });
            }
            else
            {
                return info.Invoke(instance, new object[] {});
            }
        }
    }
}


