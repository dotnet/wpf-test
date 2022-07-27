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
using System.Windows.Automation;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Interop;

namespace DRT
{
    public enum MultiContextTest
    {
        OpenPopup,
    }

    public class MultiContextSuite : DrtTestSuite
    {
        public MultiContextSuite(MultiContextTest test) : base("MultiContext")
        {
            Contact = "Microsoft";
            _test = test;
        }

        MultiContextTest _test;
        private WarningLevel _previousWarningLevel;

        #region Window setup and common hooks

        public override DrtTest[] PrepareTests()
        {
            _previousWarningLevel = ((DrtControlsBase)DRT).WindowDeactivatedWarningLevel;
            ((DrtControls)DRT).WindowDeactivatedWarningLevel = WarningLevel.Ignore;

            _threadBLock.Reset();
            _threadB = new Thread(new ThreadStart(ThreadB));
            _threadB.ApartmentState = ApartmentState.STA;
            _threadB.Start();

            _threadC = new Thread(new ThreadStart(ThreadC));
            _threadC.ApartmentState = ApartmentState.STA;
            _threadC.Start();

            Keyboard.Focus(null);

            // Context #1

            Border border = new Border();
            border.Background = SystemColors.ControlBrush;

            DockPanel dockPanel = new DockPanel();
            border.Child = dockPanel;

            Text caption = new Text();
            caption.Text = "1st Context, Thread A";
            DockPanel.SetDock(caption, Dock.Top);
            dockPanel.Children.Add(caption);

            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            DockPanel.SetDock(stackPanel, Dock.Top);
            dockPanel.Children.Add(stackPanel);

            _button_1 = new Button();
            _button_1.Content = "1:Button";
            stackPanel.Children.Add(_button_1);

            _checkBox_1 = new CheckBox();
            _checkBox_1.Content = "1:CheckBox";
            stackPanel.Children.Add(_checkBox_1);

            _radioButton_1 = new RadioButton();
            _radioButton_1.Content = "1:RadioButton";
            stackPanel.Children.Add(_radioButton_1);

            _comboBox_1 = new ComboBox();
            _comboBox_1.Items.Add("1:ComboBoxItem 1");
            _comboBox_1.Items.Add("1:ComboBoxItem 2");
            _comboBox_1.Items.Add("1:ComboBoxItem 3");
            stackPanel.Children.Add(_comboBox_1);

            _scrollBar_1 = new HorizontalScrollBar();
            _scrollBar_1.Width = 75.0;
            _scrollBar_1.Height = 16.0;
            stackPanel.Children.Add(_scrollBar_1);

            _textBox_1 = new TextBox();
            _textBox_1.Text = "1:TextBox";
            stackPanel.Children.Add(_textBox_1);

            DRT.Show(border);

            // Context #2

            using (SecondContext.Access())
            {
                border = new Border(SecondContext);
                border.Background = SystemColors.ControlBrush;

                dockPanel = new DockPanel(SecondContext);
                border.Child = dockPanel;

                caption = new Text(SecondContext);
                caption.Text = "2nd Context, Thread A";
                DockPanel.SetDock(caption, Dock.Top);
                dockPanel.Children.Add(caption);

                stackPanel = new StackPanel(SecondContext);
                DockPanel.SetDock(stackPanel, Dock.Top);
                dockPanel.Children.Add(stackPanel);

                _button_2 = new Button(SecondContext);
                _button_2.Content = "2:Button";
                stackPanel.Children.Add(_button_2);

                _popup_2 = new Popup(SecondContext);
                _popup_2.Placement = PlacementMode.Right;
                stackPanel.Children.Add(_popup_2);

                Border popupBorder = new Border(SecondContext);
                popupBorder.Background = SystemColors.InfoBrush;
                _popup_2.Child = popupBorder;

                Text popupText = new Text(SecondContext);
                popupText.Text = "2:Popup Content";
                popupBorder.Child = popupText;

                _checkBox_2 = new CheckBox();
                _checkBox_2.Content = "2:CheckBox";
                stackPanel.Children.Add(_checkBox_2);

                _radioButton_2 = new RadioButton();
                _radioButton_2.Content = "2:RadioButton";
                stackPanel.Children.Add(_radioButton_2);

                _comboBox_2 = new ComboBox();
                _comboBox_2.Items.Add("2:ComboBoxItem 1");
                _comboBox_2.Items.Add("2:ComboBoxItem 2");
                _comboBox_2.Items.Add("2:ComboBoxItem 3");
                stackPanel.Children.Add(_comboBox_2);

                _scrollBar_2 = new HorizontalScrollBar();
                _scrollBar_2.Width = 75.0;
                _scrollBar_2.Height = 16.0;
                stackPanel.Children.Add(_scrollBar_2);

                _textBox_2 = new TextBox();
                _textBox_2.Text = "2:TextBox";
                stackPanel.Children.Add(_textBox_2);

                SecondWindow.RootVisual = border;
            }

            // Context #3

            _threadBLock.WaitOne();
            ThirdContext.BeginInvoke(new UIContextOperationCallback(CreateThirdVisualTree), null, UIContextPriority.Normal);
            _threadBLock.Reset();

            if (!DRT.KeepAlive)
            {
                List<DrtTest> tests = new List<DrtTest>();
                tests.Add(new DrtTest(Start));

                switch (_test)
                {
                    case MultiContextTest.OpenPopup:
                        tests.Add(new DrtTest(OpenPopupTest));
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

        private void ThreadB()
        {
            _thirdContext = new UIContext();
            _threadBLock.Set();
            using (_thirdContext.Access())
            {
                HwndSourceParameters param = new HwndSourceParameters("DrtMultiContextControl - 3rd Context - Thread B", 200, 500);
                param.SetPosition(550, 50);
                _thirdSource = new HwndSource(param);

                Border border = new Border();
                border.Background = SystemColors.ControlBrush;

                _thirdSource.RootVisual = border;
            }
            _dispatcherB = new HwndDispatcher();
            _dispatcherB.RegisterContext(_thirdContext);
            _dispatcherB.Run();
        }

        public object QuitThreadB(object arg)
        {
            _dispatcherB.Quit();
            return null;
        }

        private void ThreadC()
        {
            do
            {
                _threadCLock.WaitOne();

                if (_threadCTest != null)
                {
                    _threadCTest();
                }

                _threadCLock.Reset();
                _threadCWaitLock.Set();
            }
            while (_threadCTest != null);
        }

        private void Start()
        {
            if (!DRT.KeepAlive)
            {
                _suicideTimer = new UITimer();
                _suicideTimer.Interval = new TimeSpan(0, 5, 0);
                _suicideTimer.Tick += new EventHandler(OnTimeout);
                _suicideTimer.Start();
            }
        }

        public void Cleanup()
        {
            if (_suicideTimer != null)
            {
                _suicideTimer.Stop();
            }
            ThirdContext.BeginInvoke(new UIContextOperationCallback(QuitThreadB));

            ReleaseThreadC(null);

            if (_secondSource != null)
            {
                using (_secondContext.Access())
                {
                    _secondSource.Dispose();
                }
                _secondSource = null;
            }
            UIContext drtContext = UIContext.CurrentContext;
            if (_secondContext != null)
            {
                UIContext.CurrentContext.Dispatcher.UnregisterContext(_secondContext);
                _secondContext.Dispose();
                _secondContext = null;
            }

            ((DrtControlsBase)DRT).WindowDeactivatedWarningLevel = _previousWarningLevel;

        }

        private object CreateThirdVisualTree(object args)
        {
            Border border = new Border(ThirdContext);
            border.Background = SystemColors.ControlBrush;

            DockPanel dockPanel = new DockPanel(ThirdContext);
            border.Child = dockPanel;

            Text caption = new Text(ThirdContext);
            caption.Text = "3rd Context, 2nd Thread";
            DockPanel.SetDock(caption, Dock.Top);
            dockPanel.Children.Add(caption);

            StackPanel stackPanel = new StackPanel(ThirdContext);
            DockPanel.SetDock(stackPanel, Dock.Top);
            dockPanel.Children.Add(stackPanel);

            _button_3 = new Button(ThirdContext);
            _button_3.Content = "3:Button";
            stackPanel.Children.Add(_button_3);

            ThirdWindow.RootVisual = border;

            _threadBLock.Set();

            return null;
        }

        #endregion

        #region OpenPopup

        private void OpenPopupTest()
        {
            if (DRT.Verbose)
            {
                Console.WriteLine("Testing opening a popup from a different thread");
            }

            ReleaseThreadC(new DrtTest(OpenPopup));

            DRT.ResumeAt(new DrtTest(PopupTestWaitForThreadC));
        }

        private void OpenPopup()
        {
            using (_popup_2.Context.Access())
            {
                _popup_2.IsOpen = true;
            }
        }

        private void PopupTestWaitForThreadC()
        {
            // Wait for the other thread to open the popup
            WaitForThreadC(5000);

            DRT.ResumeAt(new DrtTest(OpenPopupTestVerify));
        }

        private void OpenPopupTestVerify()
        {
            if (DRT.Verbose)
            {
                Console.WriteLine("Verifying opened a popup from a different context");
            }

            using (_popup_2.Context.Access())
            {
                DRT.Assert(_popup_2.IsOpen, "_popup_2's drop down isn't open");
                _popup_2.IsOpen = false;
            }
        }

        #endregion

        private void EnsureSecondContext()
        {
            UIContext drtContext = UIContext.CurrentContext;
            if (_secondContext == null)
            {
                _secondContext = new UIContext();
                using (_secondContext.Access())
                {
                    HwndSourceParameters param = new HwndSourceParameters("DrtMultiContextControl - 2nd Context, Thread A", 200, 500);
                    param.SetPosition(300, 50);
                    _secondSource = new HwndSource(param);
                }
                drtContext.Dispatcher.RegisterContext(_secondContext);
            }
        }

        public UIContext SecondContext
        {
            get
            {
                EnsureSecondContext();
                return _secondContext;
            }
        }

        public HwndSource SecondWindow
        {
            get
            {
                EnsureSecondContext();
                return _secondSource;
            }
        }

        public UIContext ThirdContext
        {
            get
            {
                return _thirdContext;
            }
        }

        public HwndSource ThirdWindow
        {
            get
            {
                return _thirdSource;
            }
        }

        public void ReleaseThreadC(DrtTest test)
        {
            _threadCTest = test;
            _threadCWaitLock.Reset();
            _threadCLock.Set();
        }

        public void WaitForThreadC(int milliseconds)
        {
            _threadCWaitLock.WaitOne(milliseconds, false);
        }

        #region Suicide Timer

        UITimer _suicideTimer;

        private void OnTimeout(object sender, EventArgs e)
        {
            throw new TimeoutException();
        }

        private class TimeoutException : Exception
        {
            public TimeoutException() : base("Timeout expired, quitting") { }
        }

        #endregion

        private Button _button_1;
        private CheckBox _checkBox_1;
        private RadioButton _radioButton_1;
        private ComboBox _comboBox_1;
        private HorizontalScrollBar _scrollBar_1;
        private TextBox _textBox_1;

        private Button _button_2;
        private Popup _popup_2;
        private CheckBox _checkBox_2;
        private RadioButton _radioButton_2;
        private ComboBox _comboBox_2;
        private HorizontalScrollBar _scrollBar_2;
        private TextBox _textBox_2;

        private Button _button_3;
        private ManualResetEvent _threadBLock = new ManualResetEvent(false);

        private UIContext _secondContext;
        private HwndSource _secondSource;

        private HwndDispatcher _dispatcherB;
        private UIContext _thirdContext;
        private HwndSource _thirdSource;
        private Thread _threadB;

        private Thread _threadC;
        private ManualResetEvent _threadCLock = new ManualResetEvent(false);
        private ManualResetEvent _threadCWaitLock = new ManualResetEvent(true);
        private DrtTest _threadCTest;
    }
}
