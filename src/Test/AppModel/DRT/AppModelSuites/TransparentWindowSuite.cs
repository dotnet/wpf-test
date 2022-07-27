// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;
using System.ComponentModel;
using System.Text;
using System.Security;

using System.Windows.Input;
using System.Windows.Automation;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace DRT
{
    public class TransparentWindowSuite : DrtTestSuite
    {
        public TransparentWindowSuite() : base("TransparentWindow")
        {
            TeamContact = "WPF";
            Contact     = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            return new DrtTest[]
                    {
                        new DrtTest(VerifyProperties),
                        new DrtTest(VerifyShow),
                        new DrtTest(VerifyRendered),
                    };
        }

        private void VerifyWindowException(WindowStyle style)
        {
            Window _window = new Window();
            _window.AllowsTransparency = true;
            _window.WindowStyle = style;

            try
            {
                _window.Show();
                _window.Close();
                DRT.Assert(false, "Should not have been able to show _window with AllowsTransparency=true and WindowStyle={0}", style);
            }
            catch (InvalidOperationException)
            {
                // We expect to get an InvalidOperationException
            }
        }

        private void VerifyProperties()
        {
            VerifyWindowException(WindowStyle.SingleBorderWindow);
            VerifyWindowException(WindowStyle.ToolWindow);
            VerifyWindowException(WindowStyle.ThreeDBorderWindow);
        }

        private void VerifyShow()
        {
            _window = new Window();
            _window.Title = WindowName;
            _window.Name = WindowName;
            _window.AllowsTransparency = true;
            _window.WindowStyle = WindowStyle.None;
            _window.Background = Brushes.Transparent;
            _window.SizeToContent = SizeToContent.WidthAndHeight;
            _window.Left = 300;
            _window.Top = 300;

            Border border = new Border();
            border.Width = 200;
            border.Height = 200;

            border.CornerRadius = new CornerRadius(25);
            border.Background = Brushes.Thistle;

            Button button = new Button();
            button.Name = ButtonName;
            button.Content = "Button";
            button.Height = 25;
            button.Width = 48;
            border.Child = button;

            _window.Content = border;

            _window.ContentRendered += new EventHandler(window_ContentRendered);

            _window.Show();
        }

        private void VerifyRendered()
        {
            DRT.Assert(_rendered, "Window should have rendered");
            AutomationElement windowElement = AutomationHelper.FindDescendantByName(AutomationElement.RootElement, WindowName);
            DRT.Assert(windowElement != null, "Could not find window");
        }

        void window_ContentRendered(object sender, EventArgs e)
        {
            _rendered = true;
        }

        Window _window = null;
        bool _rendered = false;

        const string WindowName = "TransparentWindow";
        const string ButtonName = "DaButton";
    }
}

