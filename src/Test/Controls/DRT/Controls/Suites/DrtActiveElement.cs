// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace DRT
{
/// <summary>
/// Test navigating the initial focus to Window.ActiveElement
/// </summary>
    public class DrtActiveElementSuite : DrtTestSuite
    {
        public DrtActiveElementSuite() : base("ActiveElement")
        {
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            _previousWarningLevel = ((DrtControlsBase)DRT).WindowDeactivatedWarningLevel;
            ((DrtControlsBase)DRT).WindowDeactivatedWarningLevel = WarningLevel.Ignore;
            DrtControls.NotUsingDrtWindow(DRT);

            _window = TestWindow();

            _window.Title = "DRT for ActiveElement";
            _window.Show();

            return new DrtTest[]
                {
                    new DrtTest(VerifyFocusInitial),
                    new DrtTest(HwndSourceInitialFocus),
                    new DrtTest(VerifyHwndSourceInitialFocus),
                    new DrtTest(Cleanup),
                };
        }


        private void VerifyFocusInitial()
        {
            DRT.Assert(Keyboard.FocusedElement == _b2, "Focus should be on second button. Current focus:" + Keyboard.FocusedElement);
            _window.Close();
        }

        private void HwndSourceInitialFocus()
        {
            HwndSourceParameters param = new HwndSourceParameters("HwndSource");
            param.SetPosition(10, 10);
            param.SetSize(100, 100);
            _hwndSource = new HwndSource(param);
            _hwndSource.RootVisual = new MyPage();
        }

        private void VerifyHwndSourceInitialFocus()
        {
            DRT.Assert(Keyboard.FocusedElement == ((Page)(_hwndSource.RootVisual)).Content, "Focus should be on TextBox within the Page. Current focus:" + Keyboard.FocusedElement);
            _hwndSource.Dispose();
        }

        private void Cleanup()
        {
            ((DrtControlsBase)DRT).WindowDeactivatedWarningLevel = _previousWarningLevel;
        }


        HwndSource _hwndSource;
        private Button _b1;
        private Button _b2;
        private Window _window;
        private WarningLevel _previousWarningLevel;

        Window TestWindow()
        {
            Window window = new Window();
            window.Width = 200;
            window.Height = 100;

            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            _b1 = new Button();
            _b2 = new Button();

            sp.Children.Add(_b1);
            sp.Children.Add(_b2);
            window.Content = sp;
            FocusManager.SetFocusedElement(window, (IInputElement)_b2);
            return window;
        }

        public partial class MyPage : Page
        {
            public MyPage()
            {
                TextBox myTextbox = new TextBox();
                this.Content = myTextbox;
                myTextbox.Focus();
            }
        }


    }
}
