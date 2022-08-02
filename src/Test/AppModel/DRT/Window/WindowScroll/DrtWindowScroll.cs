// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;

namespace DRT
{
    /// <summary>
    /// Drt to test the ScrollViewer in Page's style
    /// </summary>
    class DrtWindowScroll
    {
        [STAThread]
        public static int Main(string[] args)
        {
            WindowScrollTestApplication theApp = new WindowScrollTestApplication();

            try
            {                
                theApp.Run();
            }
            catch (ApplicationException e)
            {
                WindowScrollTestApplication.Logger.Log( e );
                WindowScrollTestApplication.Logger.Log("Failed.");
                return 1;
            }

            WindowScrollTestApplication.Logger.Log("Passed.");
            return 0;
        }
    }

    public class WindowScrollTestApplication : Application
    {
        private static Logger _logger;

        internal static Logger Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = new Logger("DrtWindowScroll", "Microsoft", "Tests using ScrollViewer in NavigationWindow");
                }
                return _logger;
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SetupPage();

            _win = new NavigationWindow();
            _win.Closed += new EventHandler(OnClosed);
            _win.Content = _page;
            _win.Show();
        }

        /// <summary>
        /// Finds the ScrollViewer in Window's style
        /// </summary>
        private void VerifyContentPresenterInPageStyle()
        {
            Border border = VisualTreeHelper.GetChild(_page, 0) as Border;
            if (border == null)
            {
                throw new ApplicationException("Cannot find Border in visual tree of Page.");
            }

            ContentPresenter cp = VisualTreeHelper.GetChild(border, 0) as ContentPresenter;
            if (cp == null)
            {
                throw new ApplicationException("Cannot find ContentPresenter in visual tree of Page.");
            }
        }

        /// <summary>
        /// Run a series of tests. Just makes sure scrollbars appear when they should.
        /// </summary>
        private void SetupPage()
        {
            _page = new Page();
            _page.Loaded += new RoutedEventHandler(OnLoaded);

            Rectangle rect = new Rectangle();
            rect.Fill = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0xff));

            _scv = new ScrollViewer();
            _scv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _scv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            _scv.Content = rect;

            rect.Height = 300;
            rect.Width = 300;
            _page.WindowWidth = 150;
            _page.WindowHeight = 150;

            _page.Content = _scv;
        }

        // tests are really performed here
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            VerifyContentPresenterInPageStyle();
            
            VerifyScrollBarVisibility("Content is smaller.", Visibility.Visible, Visibility.Visible);

            _win.SizeToContent = SizeToContent.Width;
            VerifyScrollBarVisibility("SizeToContent.Width.", Visibility.Collapsed, Visibility.Visible);

            _win.SizeToContent = SizeToContent.Manual;
            _win.Width = 151;
            _win.SizeToContent = SizeToContent.Height;
            VerifyScrollBarVisibility("SizeToContent.Height.", Visibility.Visible, Visibility.Collapsed);

            _win.SizeToContent = SizeToContent.WidthAndHeight;
            VerifyScrollBarVisibility("SizeToContent.WidthAndHeight.", Visibility.Collapsed, Visibility.Collapsed);

            _win.SizeToContent = SizeToContent.Manual;
            _win.Width = 152;
            _scv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _scv.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            _win.SizeToContent = SizeToContent.Height;
            VerifyScrollBarVisibility("Page properties.", Visibility.Collapsed, Visibility.Visible);

            _passed = true;
            _win.Close();
        }

        // Make sure the window was self-closed
        private void OnClosed(object sender, EventArgs args)
        {
            if (!_passed)
            {
                throw new ApplicationException("Window closed without all tests passing");
            }
        }

        private void VerifyScrollBarVisibility(String message, Visibility visibilityX, Visibility visibilityY)
        {
            String errorMessage;
            if (visibilityX != _scv.ComputedHorizontalScrollBarVisibility)
            {
                errorMessage = message +
                               " ComputedHorizontalScrollBarVisibility:" +
                               " expected " + visibilityX +
                               " found " + _scv.ComputedHorizontalScrollBarVisibility;
                throw new ApplicationException(errorMessage);
            }
            if (visibilityY != _scv.ComputedVerticalScrollBarVisibility)
            {
                errorMessage = message + 
                               " ComputedVerticalScrollBarVisibility:" +
                               " expected " + visibilityY +
                               " found " + _scv.ComputedVerticalScrollBarVisibility;
                throw new ApplicationException(errorMessage);
            }
        }

        private NavigationWindow _win;
        private ScrollViewer _scv;
        private Page _page;
        private bool _passed = false;
    }
}

