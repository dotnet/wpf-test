// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Deployment;
using System.Deployment.Application;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Microsoft.Test.WPF.AppModel.Deployment
{
    public partial class BasicFTAppWindow : NavigationWindow
    {
        #region Hosted Accelerator Regression test members

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = (HwndSource)PresentationSource.FromVisual(this);
        }

        #endregion
    }

    public partial class BasicFTAppContent : Page
    {
        #region Hosted Accelerator Regression test members

        public ControlHost AccControlHost { get { return controlHost; } }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            tb1.Text = "Primary Button Click Handler Succeeded";
        }

        #endregion

        #region Basic Functional Test helpers
        // Fires when the button is clicked, attempting to navigate to whatever is inputted into the textbox.
        void OnNavButtonClicked(object sender, EventArgs e)
        {
            try
            {
                string actUri = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.ActivationUri.ToString();
                MessageBox.Show(actUri);

                dummyLink.NavigateUri = new Uri(theURL.Text);
                dummyLink.DoClick();
            }
            catch
            {
                // Do nothing, as we have failed to get the URI right.  Time out the test...
            }
        }

        void CheckURLParams(object sender, EventArgs e)
        {
            try
            {
                string actUri = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.ActivationUri.ToString();
                if (actUri.IndexOf("?") >= 0)
                    urlParamBtn.Text = actUri.Substring(actUri.IndexOf("?") + 1);
            }
            catch
            {
                // Do nothing... for when there are no URL params.
            }
        }
        #endregion
    }

    #region Hosted Accelerator Regression classes

    public class ControlHost : HwndHost
    {
        HwndSource _childWindow;

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            HwndSourceParameters hsp = new HwndSourceParameters("Control Host");
            hsp.ParentWindow = hwndParent.Handle;
            hsp.WindowStyle = /*WS_CHILD*/0x40000000;
            _childWindow = new HwndSource(hsp);
            _childWindow.RootVisual = new NestedToolbar();
            return new HandleRef(this, _childWindow.Handle);
        }

        protected override void DestroyWindowCore(HandleRef hwnd) { }

#if TESTBUILD_CLR40
        protected override bool TranslateAcceleratorCore(ref MSG msg, System.Windows.Input.ModifierKeys modifiers)
        {
            return ((IKeyboardInputSink)childWindow).TranslateAccelerator(ref msg, modifiers);
        }
        protected override bool OnMnemonicCore(ref MSG msg, System.Windows.Input.ModifierKeys modifiers)
        {
            bool handled = ((IKeyboardInputSink)childWindow).OnMnemonic(ref msg, modifiers);
            return handled;
        }
#endif

    };

    class FakeInputSink : IKeyboardInputSink
    {
        bool IKeyboardInputSink.HasFocusWithin()
        {
            return false;
        }

        IKeyboardInputSite IKeyboardInputSink.KeyboardInputSite
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        bool IKeyboardInputSink.OnMnemonic(ref MSG msg, ModifierKeys modifiers)
        {
            throw new NotImplementedException();
        }

        IKeyboardInputSite IKeyboardInputSink.RegisterKeyboardInputSink(IKeyboardInputSink sink)
        {
            throw new NotImplementedException();
        }

        bool IKeyboardInputSink.TabInto(TraversalRequest request)
        {
            throw new NotImplementedException();
        }

        bool IKeyboardInputSink.TranslateAccelerator(ref MSG msg, ModifierKeys modifiers)
        {
            throw new NotImplementedException();
        }

        bool IKeyboardInputSink.TranslateChar(ref MSG msg, ModifierKeys modifiers)
        {
            throw new NotImplementedException();
        }
    }

    class ChildWPFWindowHost : HwndHost
    {
        protected override System.Runtime.InteropServices.HandleRef BuildWindowCore(System.Runtime.InteropServices.HandleRef hwndParent)
        {
            HwndSourceParameters hsp = new HwndSourceParameters();
            hsp.ParentWindow = hwndParent.Handle;
            hsp.WindowStyle = /*WS_CHILD*/0x40000000;
            HwndSource childWindow = new HwndSource(hsp);
            TextBox childWPFWindowTextBox = new TextBox() { Width = 120 };
            childWPFWindowTextBox.Name = "childWPFWindowTextBox";
            childWindow.RootVisual = childWPFWindowTextBox;
            return new HandleRef(this, childWindow.Handle);
        }

        protected override void DestroyWindowCore(System.Runtime.InteropServices.HandleRef hwnd)
        {
        }
#if TESTBUILD_CLR40
        protected override bool TranslateAcceleratorCore(ref MSG msg, ModifierKeys modifiers)
        {
            IKeyboardInputSink sink = ((BasicFTAppContent)Window.GetWindow(this).Content).AccControlHost;
            return sink.TranslateAccelerator(ref msg, modifiers);
        }
#endif
    };

    class ChildWin32WindowHost : HwndHost
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr CreateWindowEx(int dwExStyle, string lpClassName, string lpWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hwndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        protected override System.Runtime.InteropServices.HandleRef BuildWindowCore(System.Runtime.InteropServices.HandleRef hwndParent)
        {
            IntPtr hwnd = CreateWindowEx(0, "EDIT", "", /*WS_CHILD*/0x40000000 |/*WS_BORDER*/0x00800000, 0, 0, 50, 50,
                hwndParent.Handle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            if (hwnd == IntPtr.Zero)
                throw new Win32Exception();
            return new HandleRef(this, hwnd);
        }

        protected override void DestroyWindowCore(System.Runtime.InteropServices.HandleRef hwnd)
        {
        }
#if TESTBUILD_CLR40
        protected override bool TranslateAcceleratorCore(ref MSG msg, ModifierKeys modifiers)
        {
            IKeyboardInputSink sink = ((BasicFTAppContent)Window.GetWindow(this).Content).AccControlHost;
            return sink.TranslateAccelerator(ref msg, modifiers);
        }
#endif
    };

    #endregion

}
