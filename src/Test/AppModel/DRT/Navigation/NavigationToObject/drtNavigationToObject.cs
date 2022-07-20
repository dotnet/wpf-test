// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Windows.Threading;

using System.Collections;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace DrtNavigationToObject
{
    public class navapp : System.Windows.Application
    {

        [STAThread]
        static int Main(string[] args)
        {
            try
            {
                new navapp().Run();
            }
            catch (ApplicationException e)
            {
                _logger.Log(e);
                return 1;
            }

            _logger.Log("Passed");
            return 0;
        }

        NavigationWindow _win;
        Button _b1, _b2;
        Frame _frame;
        int _navWinContentRenderedCount;

        protected override void OnStartup(StartupEventArgs e)
        {
            _win = new NavigationWindow();
            _win.Title = "Hello Hua's test";

            _b1 = new Button();
            _b1.Content = "Click to navigate";
            //_b1.Click += new RoutedEventHandler(navigate);
            _b1.SetValue(DockPanel.DockProperty, Dock.Left);
            _b1.Width = 200;
            _b1.Height = 300;

            _b2 = new Button();
            _b2.Content = "Click to navigate inside frame";
            //_b2.Click += new RoutedEventHandler(navigate);
            _b2.SetValue(DockPanel.DockProperty, Dock.Left);
            _b2.Width = 200;
            _b2.Height = 300;

            _frame = new Frame();
            _frame.Width = 400;
            _frame.Height = 500;
            _frame.Content = _b2;

            _win.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(winLoadCompleted);
            _win.ContentRendered += new EventHandler(OnNavWinContentRendered);

            DockPanel dp = new DockPanel();
            dp.Children.Add(_b1);
            dp.Children.Add(_frame);

            TextBlock tb = new TextBlock(new Run("[bottom]"));
            tb.SetValue(DockPanel.DockProperty, Dock.Bottom);
            tb.Name = "bottom";
            dp.Children.Add(tb);

            _win.Content = dp;
            _win.Show();

            base.OnStartup(e);
        }

        private void winLoadCompleted(object o, System.Windows.Navigation.NavigationEventArgs args)
        {
            _logger.Log("NavigationWindow.LoadCompleted");

            NavigationService ns_win = NavigationService.GetNavigationService(_b1);
            NavigationService ns_frame = NavigationService.GetNavigationService(_b2);

            Assert(_navWinContentRenderedCount == 0, "LoadCompleted event should occur before ContentRendeed.");
            Assert(ns_win != null, "NavigationService.GetNavigationService return null. It should return the NavigationWindow");
            Assert(ns_frame != null, "NavigationService.GetNavigationService return null. It should return the Frame");

            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(NavigateToFD),
                ns_frame);            
        }

        void OnNavWinContentRendered(object sender, EventArgs e)
        {
            _logger.Log("NavigationWindow.ContentRendered");
            _navWinContentRenderedCount++;
        }

        private object NavigateToFD(object obj)
        {
            Assert(_navWinContentRenderedCount == 1, "ContentRendeed event for NavigationWindow did not occur.");

            NavigationService ns_frame = obj as NavigationService;
            _frame.ContentRendered += new EventHandler(OnFrameContentRendered);
            _frame.SandboxExternalContent = false;
            ns_frame.Source = new Uri("pack://siteoforigin:,,,/DrtFiles/NavigationToObject/flowdocument.xaml",  UriKind.RelativeOrAbsolute);
            return null;
        }

        private void OnFrameContentRendered(object sender, EventArgs e)
        {
            _logger.Log("Frame.ContentRendered");
            Frame frame = sender as Frame;
            frame.ContentRendered -= new EventHandler(OnFrameContentRendered);
            
            FlowDocument fd = frame.Content as FlowDocument;

            if (fd == null)
            {
                throw new ApplicationException("Frame's Content should be FlowDocument");
            }

            // Find the FlowDocumentPageViewer as the first child of Frame's ContentPresenter
            ContentPresenter cp = frame.Template.FindName("PART_FrameCP", frame) as ContentPresenter;
            if (cp == null)
            {
                throw new ApplicationException("Frame Styling: The first visual child of broder should be a ContentPresenter");
            }
            FlowDocumentReader fdr = VisualTreeHelper.GetChild(cp, 0) as FlowDocumentReader;
            if (fdr == null)
            {
                throw new ApplicationException("FlowDocument Styling: FlowDocument should be hosted inside FlowDocumentReader");
            }

            TestRefresh();
        }

        delegate void TestStepDelegate();

        void TestRefresh()
        {
            _win.LoadCompleted -= winLoadCompleted;

            bool loadCompletedCalled = false;
            _win.LoadCompleted += delegate {
                _logger.Log("NavigationWindow.LoadCompleted");
                loadCompletedCalled = true;
            };

            _logger.Log("Refreshing the NavigationWindow...");
            _win.Refresh();

            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal, new TestStepDelegate(
                delegate() {
                    Assert(loadCompletedCalled, "LoadCompleted did not occur after object Refresh.");
                }));

            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Input, new TestStepDelegate(
                delegate()
                {
                    Assert(_navWinContentRenderedCount == 2, "ContentRendered did not occur after object Refresh (or occurred more than once).");
                }));

            //
            // Test 2: Refresh from object#fragment
            // Even though NavigationService has a URI available for the current Content, it should 
            // still handle the refresh as object navigation. And it should also redo the fragment navigation.

            _logger.Log("Navigating NW to #bottom target");
            string fragmentTarget = null;
            _win.FragmentNavigation += new FragmentNavigationEventHandler(
                delegate(object sender, FragmentNavigationEventArgs e) 
                {
                    fragmentTarget = e.Fragment;
                    _logger.Log("NavigationWindow.FragmentNavigation(#"+fragmentTarget+")");
                });
            Assert(_win.CurrentSource == null);
            Uri target = new Uri("#bottom", UriKind.Relative);
            _win.Source = target;
            // Assumption: Fragment navigation is handled synchronously.
            Assert(_win.CurrentSource == target);

            _logger.Log("Refreshing the NavigationWindow...");
            fragmentTarget = null;
            _win.Refresh();

            loadCompletedCalled = false;
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal, new TestStepDelegate(
                delegate()
                {
                    Assert(fragmentTarget == null, "Fragment navigation should not be attempted before content is rendered.");
                    Assert(_win.CurrentSource == target, "#fragment target was not preserved after object Refresh.");
                    Assert(loadCompletedCalled, "LoadCompleted did not occur after object#fragment Refresh.");
                }));
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Input, new TestStepDelegate(
                delegate()
                {
                    Assert(_navWinContentRenderedCount == 3, "ContentRendered did not occur after object#fragment Refresh (or occurred more than once).");
                    Assert(fragmentTarget == "bottom", "Fragment navigation did not occur after object#fragment Refresh.");
                }));

            // Mini-test 3
            _logger.Log("Going Back");
            bool scrolledToTop = false;
            _win.CommandBindings.Add(
                new CommandBinding(ScrollBar.ScrollToTopCommand, 
                new ExecutedRoutedEventHandler(delegate {
                    scrolledToTop = true;
                })));
            _win.GoBack();
            Assert(_win.CurrentSource == null && scrolledToTop && _navWinContentRenderedCount == 3,
                "Going back from object#fragment to just object should be handled as fragment navigation, "+
                "scrolling content to top.");


            ////////////////////////////////////////////////////////////
            // Exit DRT.
            Shutdown();
        }


        private void Assert(bool cond)
        {
            if (!cond)
                throw new ApplicationException();
        }
        private void Assert(bool cond, string msg)
        {
            if (!cond)
                throw new ApplicationException(msg);
        }

        private static DRT.Logger _logger = new DRT.Logger("DrtNavigationToObject", "Microsoft", "Testing NavigationService.GetService and Navigation to FlowDocument (Object)");
    }
}




