// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Markup;

// Loose resources should be declared in the .proj file.  Since this project does
// not have a .proj file the required attributes can be added manually. 
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/xamlcontainer/navbyuri.xaml")]

namespace Test
{
    class DrtXamlContainer : Application
    {
        /// <summary>
        /// This DRT navigates to a number of pages, each with two DisposableButton controls called "button1" and
        /// "button2" on it. After each navigation, the two buttons are checked to see if they were disposed or not
        /// according to the rules, which are:
        /// - When a page is navigated away from, any controls in the logical tree that implement IDisposable will
        ///   be Disposed, with the following two exceptions:
        ///   1) Pages journaled by KeepAlive
        ///   2) PageFunctions with KeepAlive=true
        /// </summary>
        static bool s_passed = false;

        static DRT.Logger s_logger  = new DRT.Logger("DrtXamlContainer", "Microsoft", "Testing XamlContainer navigation functionality");

        public DrtXamlContainer()
        {
            this.StartupUri = new Uri(@"DrtFiles\XamlContainer\NavByUri.xaml", UriKind.RelativeOrAbsolute);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }

        protected override void OnLoadCompleted(NavigationEventArgs e)
        {
            NextState();
        }

        int _counter = 0;

        public void NextState()
        {
            switch (_counter++)
            {
                case 0:
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new DispatcherOperationCallback(this.State0), null);
                    break;

                case 1:
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new DispatcherOperationCallback(this.State1), null);
                    break;

                case 2:
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new DispatcherOperationCallback(this.State2), null);
                    break;

                case 3:
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new DispatcherOperationCallback(this.State3), null);
                    break;

                case 4:
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new DispatcherOperationCallback(this.State4), null);
                    break;
            }
        }

        NavigationWindow NavWin
        {
            get { return this.MainWindow as NavigationWindow; }
        }

        DisposableButton _button1;
        DisposableButton _button2;
        DisposableUIElement _disposableUIElement;

        void GetControls()
        {
            _button1 = (DisposableButton)LogicalTreeHelper.FindLogicalNode((DependencyObject)NavWin.Content, "button1");
            _button2 = (DisposableButton)LogicalTreeHelper.FindLogicalNode((DependencyObject)NavWin.Content, "button2");
            _disposableUIElement = FindDisposableUIElement((DependencyObject)NavWin.Content);
        }

        void ReleaseControls()
        {
            _button1 = null;
            _button2 = null;
        }

        DisposableUIElement FindDisposableUIElement(object node)
        {
            if (node is DisposableUIElement)
            {
                return (DisposableUIElement)node;
            }

            IEnumerator children = LogicalTreeHelper.GetChildren((DependencyObject)node).GetEnumerator();

            if (children != null)
            {
                while (children.MoveNext())
                {
                    DisposableUIElement child = children.Current as DisposableUIElement;
                    if (child == null)
                    {
                        DependencyObject logicalChild = children.Current as DependencyObject;
                        if (logicalChild != null)
                        {
                            child = FindDisposableUIElement(logicalChild);
                        }
                    }

                    if (child != null)
                    {
                        return child;
                    }
                }
            }

            return null;
        }

        object State0(object param)
        {
            // Previous Page: none
            // Current  Page: NavByUri.xaml=

            this.GetControls();
            NavWin.Navigate(this.GetTree());
            return null;
        }

        object State1(object param)
        {
            // Previous Page: NavByUri.xaml
            // Current  Page: element tree, journaled by KeepAlive
            // Navigating away from page with disposable controls should have disposed controls

            // Check buttons from previous page
            if (!_button1.IsDisposed)
                throw new ApplicationException("button1 should have been disposed");

            if (!_button2.IsDisposed)
                throw new ApplicationException("button2 should have been disposed");

            this.GetControls();
            NavWin.Navigate(this.GetPageFunction(false));
            return null;
        }

        object State2(object param)
        {
            // Previous Page: element tree, journaled by KeepAlive
            // Current  Page: page function, with KeepAlive set to false
            // Navigating away from page journaled by KeepAlive should not have disposed controls

            // Check buttons from previous page
            if (_button1.IsDisposed)
                throw new ApplicationException("button1 should NOT have been disposed since the page is being journaled by KeepAlive");

            if (_button2.IsDisposed)
                throw new ApplicationException("button2 should NOT have been disposed since the page is being journaled by KeepAlive");

            if (_disposableUIElement.IsDisposed)
                throw new ApplicationException("disposableUIElement should NOT have been disposed since the page is being journaled by KeepAlive");

            this.GetControls();
            NavWin.Navigate(this.GetPageFunction(true));
            return null;
        }

        object State3(object param)
        {
            // Previous Page: page function, with KeepAlive set to false
            // Current  Page: page function, with KeepAlive set to true
            // Navigating away from page function with KeepAlive=false should not have disposed controls

            // Check buttons from previous page
            if (! _button1.IsDisposed)
                throw new ApplicationException("button1 should have been disposed since the page function is KeepAlive=false");

            if (! _button2.IsDisposed)
                throw new ApplicationException("button2 should have been disposed since the page function is KeepAlive=false");

            if (!_disposableUIElement.IsDisposed)
                throw new ApplicationException("disposableUIElement should have been disposed since the page function is KeepAlive=false");

            this.GetControls();
            NavWin.Navigate(this.StartupUri);
            return null;
        }

        object State4(object param)
        {
            // Previous Page: page function, with KeepAlive set to true
            // Current  Page: NavByUri.xaml
            // Navigating away from page function with KeepAlive=true should have disposed controls

            // Check buttons from previous page
            if (_button1.IsDisposed)
                throw new ApplicationException("button1 should have been disposed since the page function is KeepAlive=true");

            if (_button2.IsDisposed)
                throw new ApplicationException("button2 should have been disposed since the page function is KeepAlive=true");

            if (_disposableUIElement.IsDisposed)
                throw new ApplicationException("disposableUIElement should have been disposed since the page function is KeepAlive=true");

            this.Shutdown();
            return null;
        }

        UIElement GetTree()
        {
            StackPanel panel = GetChildren();
            // This is supposed to be the default, but just in case, we'll set in anyway.
            JournalEntry.SetKeepAlive(panel, true);

            return panel;
        }

        UIElement GetPageFunction(bool keepAlive)
        {
            PageFunction<string> pf = new PageFunction<string>();
            pf.Content = GetChildren();
            JournalEntry.SetKeepAlive(pf, keepAlive);

            return pf;
        }

        StackPanel GetChildren()
        {
            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;
            DisposableButton button1 = new DisposableButton();
            button1.Name = "button1";
            button1.Content = "Button1";
            panel.Children.Add(button1);

            DisposableButton button2 = new DisposableButton();
            button2.Name = "button2";
            button2.Content = "Button2";
            panel.Children.Add(button2);

            Button button3 = new Button();
            button3.Name = "button3";
            button3.Content = "Button3";
            panel.Children.Add(button3);

            StackPanel panel2 = new StackPanel();
            DisposableUIElement disposableUIElement = new DisposableUIElement();
            panel2.Children.Add(disposableUIElement);

            panel.Children.Add(panel2);

            return panel;
        }

        [STAThread]
        public static int Main()
        {
            try
            {
                new DrtXamlContainer().Run();
                s_passed = true;
            }
            catch(ApplicationException e)
            {
                s_logger.Log(e);
                s_logger.Log(e.StackTrace);
            }

            if (s_passed)
            {
                s_logger.Log("Passed.");
                return 0;
            }
            else
            {
                s_logger.Log("Failed.");
                return 1;
            }
        }
    }

    public class DisposableButton : Button, IDisposable
    {
        private bool _disposed = false;

        public DisposableButton()
        {
        }

        public DisposableButton(string text)
        {
            this.Content = text;
        }

        public bool IsDisposed
        {
            get { return _disposed; }
            set { _disposed = value; }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.IsDisposed)
                {
                    throw new ObjectDisposedException("DisposableButton");
                }

                this.IsDisposed = true;
            }
        }
    }

    class DisposableUIElement : UIElement, IDisposable
    {
        private bool _disposed = false;

        public bool IsDisposed
        {
            get { return _disposed; }
            set { _disposed = value; }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.IsDisposed)
                {
                    throw new ObjectDisposedException("DisposableUIElement");
                }

                this.IsDisposed = true;
            }
        }
    }
}
