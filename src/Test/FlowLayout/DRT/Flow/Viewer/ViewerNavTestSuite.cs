// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Common functionality for document viewers navigation test suites.
//

using System;                               // string
using System.Windows;                       // Size, Rect
using System.Windows.Controls;              // FlowDocumentPageViewer
using System.Windows.Documents;             // TextElement
using System.Windows.Media;                 // VisualTreeHelper
using System.Windows.Navigation;            // NavigationWindow

namespace DRT
{
    /// <summary>
    /// Common functionality for document viewers navigation test suites.
    /// </summary>
    internal abstract class ViewerNavTestSuite : ViewerTestSuite
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="suiteName">Name of the suite.</param>
        protected ViewerNavTestSuite(string suiteName) : 
            base(suiteName)
        {
        }

        /// <summary>
        /// Returns collection of tests.
        /// </summary>
        protected override DrtTest[] CreateTests()
        {
            // Return the lists of tests to run against the tree
            return new DrtTest[] {
                new DrtTest(ShowNavWindow),                 new DrtTest(VerifyEmptyAndResize),
                new DrtTest(NavigateToTOC),                 new DrtTest(UpdateViewerAndDumpCreate),
                new DrtTest(NavigateInsideTOC),             new DrtTest(DumpAppend),
                new DrtTest(NavigateToSecondDoc),           new DrtTest(UpdateViewerAndDumpAppend),
                new DrtTest(GoToPageCommand),               new DrtTest(DumpAppend),
                new DrtTest(BrowseBackCommand),             new DrtTest(UpdateViewerAndDumpAppend),
                new DrtTest(BrowseBackCommand),             new DrtTest(DumpAppend),
                new DrtTest(BrowseForwardCommand),          new DrtTest(DumpAppend),
                new DrtTest(IncreaseZoomCommand),           new DrtTest(DumpAppend),
                new DrtTest(BrowseForwardCommand),          new DrtTest(UpdateViewerAndDumpAppend),
                new DrtTest(CustomViewerCommand),           new DrtTest(DumpAppend),
                new DrtTest(BrowseBackCommand),             new DrtTest(UpdateViewerAndDumpAppend),
                new DrtTest(BrowseForwardCommand),          new DrtTest(UpdateViewerAndDumpAppend),
                new DrtTest(BrowseBackCommand),             new DrtTest(UpdateViewerAndDumpAppend),
                new DrtTest(DecreaseZoomCommand),           new DrtTest(DumpAppend),
                new DrtTest(NavigateToFirstDoc),            new DrtTest(UpdateViewerAndDumpAppend),
                new DrtTest(NavigateInside1),               new DrtTest(DumpAppend),
                new DrtTest(NavigateInside2),               new DrtTest(DumpAppend),
                new DrtTest(NavigateToUI),                  new DrtTest(UpdateViewerAndDumpAppend),
                new DrtTest(NavigateToTOC),                 new DrtTest(UpdateViewerAndDumpAppend),
                new DrtTest(BrowseBackCommand),             new DrtTest(UpdateViewerAndDumpAppend),
                new DrtTest(BrowseBackCommand),             new DrtTest(UpdateViewerAndDumpAppend),
                new DrtTest(BringIntoView),                 new DrtTest(DumpAppend),
                new DrtTest(BringIntoView),                 new DrtTest(DumpFinalizeAndVerify),
                new DrtTest(NavigateInside1),               new DrtTest(UpdateViewer),
                new DrtTest(BrowseBackCommandWithChange),   new DrtTest(UpdateViewer),
                new DrtTest(CloseNavWindow),                new DrtTest(UpdateViewer),
            };
        }

        /// <summary>
        /// Creates empty NavigationWindow with no content.
        /// </summary>
        protected virtual void ShowNavWindow()
        {
            _document = null;
            _suspended = false;
            _navWindow = new NavigationWindow();
            _navWindow.Show();
            _navWindow.Width = 820;
            _navWindow.Height = 650;
            this.Root.Child = null;
        }

        /// <summary>
        /// Update reference to the viewer object.
        /// </summary>
        protected abstract void UpdateViewer();

        /// <summary>
        /// Resets the document in the viewer to new FD.
        /// </summary>
        protected abstract void ResetDocument();

        /// <summary>
        /// Update viewer and dump content.
        /// </summary>
        private void UpdateViewerAndDumpCreate()
        {
            UpdateViewer();
            DumpCreate();
        }

        /// <summary>
        /// Update viewer and dump content.
        /// </summary>
        private void UpdateViewerAndDumpAppend()
        {
            UpdateViewer();
            DumpAppend();
        }

        /// <summary>
        /// Verify empty content and resize content of the window to uniform size.
        /// </summary>
        private void VerifyEmptyAndResize()
        {
            DRT.Assert(VisualTreeHelper.GetChildrenCount(_navWindow) == 1, "Expecting exactly one visual child in NavigationWindow.");
            FrameworkElement root = VisualTreeHelper.GetChild(_navWindow, 0) as FrameworkElement;
            DRT.Assert(root != null, "Expecting FrameworkElement as visual child of NavigationWindow.");
            root.Width = 800;
            root.Height = 600;
        }

        /// <summary>
        /// Navigates to FlowDocument with TOC.
        /// </summary>
        private void NavigateToTOC()
        {
            DRT.Assert(!_suspended, "Should not execute the next test, if DRT is suspended.");
            _suspended = true;
            DRT.Suspend();

            _navWindow.LoadCompleted += new LoadCompletedEventHandler(OnLoadCompleted);
            _navWindow.NavigationStopped += new NavigationStoppedEventHandler(OnNavigationStopped);

            _navWindow.Navigate(new Uri(this.DrtFilesDirectory + "FlowDocumentTOC.xaml", UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Hyperlink navigation within the TOC.
        /// </summary>
        private void NavigateInsideTOC()
        {
            Hyperlink hyperlink = DRT.FindElementByID("htoc", _navWindow) as Hyperlink;
            DRT.Assert(hyperlink != null, "Cannot find element with Name='htoc'.");
            InvokeHyperlink(hyperlink);
        }

        /// <summary>
        /// Hyperlink navigation to another document.
        /// </summary>
        private void NavigateToSecondDoc()
        {
            DRT.Assert(!_suspended, "Should not execute the next test, if DRT is suspended.");
            _suspended = true;
            DRT.Suspend();

            _navWindow.LoadCompleted += new LoadCompletedEventHandler(OnLoadCompleted);
            _navWindow.NavigationStopped += new NavigationStoppedEventHandler(OnNavigationStopped);

            Hyperlink hyperlink = DRT.FindElementByID("hseconddoc", _navWindow) as Hyperlink;
            DRT.Assert(hyperlink != null, "Cannot find element with Name='hseconddoc'.");
            InvokeHyperlink(hyperlink);
        }

        /// <summary>
        /// Go to page #5.
        /// </summary>
        protected virtual void GoToPageCommand()
        {
            GoToPageCommand(5);
        }

        /// <summary>
        /// Custom command
        /// </summary>
        protected virtual void CustomViewerCommand()
        {
        }

        /// <summary>
        /// Executes BrowseBack command.
        /// </summary>
        private void BrowseBackCommand()
        {
            DRT.Assert(!_suspended, "Should not execute the next test, if DRT is suspended.");
            _suspended = true;
            DRT.Suspend();

            _navWindow.LoadCompleted += new LoadCompletedEventHandler(OnLoadCompleted);
            _navWindow.NavigationStopped += new NavigationStoppedEventHandler(OnNavigationStopped);

            _navWindow.GoBack();
        }


        /// <summary>
        /// Executes BrowseBack command.
        /// </summary>
        private void BrowseBackCommandWithChange()
        {
            DRT.Assert(!_suspended, "Should not execute the next test, if DRT is suspended.");
            _suspended = true;
            DRT.Suspend();

            _navWindow.LoadCompleted += new LoadCompletedEventHandler(OnLoadCompletedWithChange);
            _navWindow.NavigationStopped += new NavigationStoppedEventHandler(OnNavigationStopped);

            _navWindow.GoBack();
        }

        /// <summary>
        /// Executes BrowseForward command.
        /// </summary>
        private void BrowseForwardCommand()
        {
            DRT.Assert(!_suspended, "Should not execute the next test, if DRT is suspended.");
            _suspended = true;
            DRT.Suspend();

            _navWindow.LoadCompleted += new LoadCompletedEventHandler(OnLoadCompleted);
            _navWindow.NavigationStopped += new NavigationStoppedEventHandler(OnNavigationStopped);

            _navWindow.GoForward();
        }

        /// <summary>
        /// Hyperlink navigation to another document.
        /// </summary>
        private void NavigateToFirstDoc()
        {
            DRT.Assert(!_suspended, "Should not execute the next test, if DRT is suspended.");
            _suspended = true;
            DRT.Suspend();

            _navWindow.LoadCompleted += new LoadCompletedEventHandler(OnLoadCompleted);
            _navWindow.NavigationStopped += new NavigationStoppedEventHandler(OnNavigationStopped);

            Hyperlink hyperlink = DRT.FindElementByID("hfirstdoc", _navWindow) as Hyperlink;
            DRT.Assert(hyperlink != null, "Cannot find element with Name='hfirstdoc'.");
            InvokeHyperlink(hyperlink);
        }

        /// <summary>
        /// Hyperlink navigation within the document.
        /// </summary>
        private void NavigateInside1()
        {
            Hyperlink hyperlink = DRT.FindElementByID("htenth", _navWindow) as Hyperlink;
            DRT.Assert(hyperlink != null, "Cannot find element with Name='htenth'.");
            InvokeHyperlink(hyperlink);
        }

        /// <summary>
        /// Hyperlink navigation within the document.
        /// </summary>
        private void NavigateInside2()
        {
            Hyperlink hyperlink = DRT.FindElementByID("hnineteenth", _navWindow) as Hyperlink;
            DRT.Assert(hyperlink != null, "Cannot find element with Name='hnineteenth'.");
            InvokeHyperlink(hyperlink);
        }

        /// <summary>
        /// Navigate to UI object.
        /// </summary>
        private void NavigateToUI()
        {
            DRT.Assert(!_suspended, "Should not execute the next test, if DRT is suspended.");
            _suspended = true;
            DRT.Suspend();

            _navWindow.LoadCompleted += new LoadCompletedEventHandler(OnLoadCompleted);
            _navWindow.NavigationStopped += new NavigationStoppedEventHandler(OnNavigationStopped);

            Border border = new Border();
            border.Background = Brushes.DarkGreen;
            border.BorderBrush = Brushes.YellowGreen;
            border.BorderThickness = new Thickness(20);
            border.Name = "UI_Object";
            _navWindow.Navigate(border);
        }

        /// <summary>
        /// Bring Hyperlink into view.
        /// </summary>
        private void BringIntoView()
        {
            TextElement element = DRT.FindElementByID("hfirst", _navWindow) as TextElement;
            DRT.Assert(element != null, "Cannot find element with Name='hfirst'.");
            element.BringIntoView();
        }

        /// <summary>
        /// Closes NavigationWindow.
        /// </summary>
        private void CloseNavWindow()
        {
            _document = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            _navWindow.Close();
        }

        /// <summary>
        /// Responds to load completed event.
        /// </summary>
        private void OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            if (_suspended)
            {
                _suspended = false;
                DRT.Resume();
            }

            _navWindow.LoadCompleted -= new LoadCompletedEventHandler(OnLoadCompleted);
            _navWindow.NavigationStopped -= new NavigationStoppedEventHandler(OnNavigationStopped);

            DRT.Assert(e.Content != null, "Content of NavigationWindows is NULL.");
            if (e.Content is Border)
            {
                _document = null;
                DRT.Assert(((Border)e.Content).Name == "UI_Object", "Expecting Border with Name='UI_Object', got '{0}'.", ((Border)e.Content).Name);
            }
            else
            {
                _document = e.Content as FlowDocument;
                DRT.Assert(_document != null, "Expecting FlowDocument as content of NavigationWindow, got {0}.", e.Content.GetType().ToString());
            }
        }

        /// <summary>
        /// Responds to load completed event, replaces the flow document
        /// </summary>
        private void OnLoadCompletedWithChange(object sender, NavigationEventArgs e)
        {
            OnLoadCompleted(sender, e);
            UpdateViewer();           
            ResetDocument();           
        }

        /// <summary>
        /// Responds to load completed event.
        /// </summary>
        private void OnNavigationStopped(object source, NavigationEventArgs e)
        {
            if (_suspended)
            {
                _suspended = false;
                DRT.Resume();
            }

            _navWindow.LoadCompleted -= new LoadCompletedEventHandler(OnLoadCompleted);
            _navWindow.NavigationStopped -= new NavigationStoppedEventHandler(OnNavigationStopped);

            DRT.Assert(false, "Navigation has been stopped.");
        }

        protected FlowDocument _document;
        protected NavigationWindow _navWindow;
        private bool _suspended;
    }
}
