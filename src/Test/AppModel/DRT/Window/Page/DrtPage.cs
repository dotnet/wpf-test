// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

using System.Windows;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Markup;
using System.Reflection;

namespace DRT
{
    public class PageApp : Application
    {
        [STAThread]
        public static int Main(string[] args)
        {
            PageApp.DisableProcessWindowsGhosting();         
            PageApp myApp = new PageApp();

            try
            {
                myApp.Run();
            }
            catch (ApplicationException e )
            {
                myApp._logger.Log("Failed: " + e);
                return 1;
            }

            // For the visual tree parenting test, we cannot let the default dispatcher context handler 
            // show the default UI b/c it causes asserts as a result of bug in Style.InstantiateVisualTree
            // code.  Thus, we hook in a dispatcher context exception handler before that test.  
            // Typically, we throw ApplicationException in this DRT to report errors.  However, for this 
            // case we cannot b/c that kicks in the dispatcher context exception handler.  Thus, we have
            // this extra variable to track success/failure of this test.
            if (myApp._visualParentingTestPassed == false)
            {
                myApp._logger.Log("Failed: " + String.Format("Making Page a child of another Page did not throw InvalidOperationException with exception text ({0})", VisualParentingTest._expectedExceptionText));
                return -1;
            }  
            
            myApp._logger.Log("Passed" );
            return 0;
        }

        private Logger      _logger = new Logger("DrtPage", "Microsoft", "Testing Page/GetWindowService functionalities");
        internal bool       _visualParentingTestPassed = false;

        private bool        _verbose;
        Page                _page1;
        Page                _page2;
        Page                _page3;
        NavigationWindow    _navWin2;
        Window              _win1, _win3, _win4;
        string              _windowTitle1 = "My First Window, woohoooo!!";
        string              _windowTitle2 = "My Second Window, woohoooo!!";
        public static string _ptitle = "This is Page";
        double              _navWin2Height = 500d;
        TextBlock           _textb;
        
        protected override void OnStartup(StartupEventArgs e)
        {
            foreach (string arg in e.Args)
            {
                switch(arg)
                {
                    case "/verbose":
                    case "-verbose":
                        _verbose = true;
                        break;
                    default:
                        break;
                }
            }
            
            _win1 = new Window();  
            _page1 = new Page();
            _win1.Content = _page1;
            _page1.WindowTitle = _windowTitle1;
            _page1.Title = PageApp._ptitle + " 1";
            _win1.Closing += new CancelEventHandler( OnClosing1 );
 
            _navWin2 = new NavigationWindow();
            _navWin2.Show();
            VerboseOutput("OnStartingUp -- _navWin2.Height: " + _navWin2.Height);
            _navWin2.LoadCompleted += new LoadCompletedEventHandler(OnLoadCompleted2);
            _page2 = new Page();
            _page2.WindowTitle = _windowTitle2;
            _page2.Title = PageApp._ptitle + " 2";
            _page2.WindowHeight = _navWin2Height;

            if (   (Brushes.Black != _page2.Foreground)
                || (SystemFonts.MessageFontFamily.Source != _page2.FontFamily.Source)
                || (SystemFonts.MessageFontSize != _page2.FontSize)
               )
            {
                throw new ApplicationException("One of the 3 font properties not defaulting correctly.");
            }
            
            _page2.Foreground = Brushes.DarkGreen;
            _page2.FontFamily = new FontFamily("Palatino Linotype");
            _page2.FontSize = 36*96.0/72.0;

            _textb = new TextBlock();
            _textb.Text = "Page Tests for Font Inheritance";
            _page2.Content = _textb;
                        
            // this result in navigating the window, thus we close the window from LoadCompleted
            // handler
            _navWin2.Content = _page2;            
            
            _win1.Show();
            _win3 = new Window();
            _page3 = new Page();
            _page3.WindowTitle = "Third Window";
            _page3.Title = PageApp._ptitle + " 3";

            _win3.Closing += new CancelEventHandler(OnClosing3);
            _win3.Content = _page3;
            _win3.Show();
            _win3.Content = null;
            
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(CloseWindows),
                this);

            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(
                    delegate(object notUsed)
                    {
                        new PageTest();
                        return null;
                    }), 
                this);

            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(
                    delegate(object notUsed)
                    {
                        new VisualParentingTest();
                        return null;
                    }), 
                this);

            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(
                    delegate(object notUsed)
                    {
                        new SerializationTest();
                        return null;
                    }),
                this);
            
            base.OnStartup (e);
        }

        void OnClosing1 ( object sender, CancelEventArgs e )
        {   
            Window win = Window.GetWindow(_page1);
            if (win.Title != _windowTitle1)
            {
                throw new ApplicationException("Title value of First Window different than what set through Page element...error");
            }
        }

        void OnLoadCompleted2(object sender, NavigationEventArgs e)
        {
            VerboseOutput("OnLoadCompleted2 -- Hooking up _page2.Closing event handler and posting a message to close the NavigationWindow");
            _navWin2.Closing += new CancelEventHandler( OnClosing2 );
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(CloseNavigationWindow),
                this);
        }
        
        void OnClosing2 ( object sender, CancelEventArgs e )
        {
            Window win = Window.GetWindow(_page2);
            if (win == null || win.Title != _windowTitle2)
            {
                throw new ApplicationException("Title value for Second Window different than what set through Page element...error");
            }

            VerboseOutput("OnClosing2 -- _navWin2.Height: " + _navWin2.Height);
            if (win.Height != _navWin2Height )
            {
                throw new ApplicationException("Height of Second Window is not equal to the value set through Page elemenet...error");
            }

            if (_page2.NavigationService == null)
            {
                throw new ApplicationException("Page NavigationService.NavigationService attached property returned null on NavigationWindow...error");
            }
            
            // Let's assert that everything inherits as expected
            if (   _textb.Foreground != _page2.Foreground
                || _textb.FontFamily != _page2.FontFamily
                || _textb.FontSize   != _page2.FontSize
               )
            {
                throw new ApplicationException("One of the 3 inheriting font properties not inheriting correctly.");
            }
        }

        void OnClosing3(object sender, CancelEventArgs e)
        {
            _win3.Closing -= new CancelEventHandler(OnClosing3);
            _win4 = new Window();
            _win4.Activated += new EventHandler(OnActivated4);
            _win4.Content = _page3;
            _page3.WindowTitle = "Fourth Window";
            _win4.Show();            
        }

        void OnActivated4(object sender, EventArgs e)
        {
            _win4.Close();
        }

        private object CloseNavigationWindow(object obj)
        {
            _navWin2.Close();
            return null;
        }
        
        private object CloseWindows(object obj)
        {
            _win1.Close();
            _win3.Close();

            return null;
        }

        // Tests top level page functionalities.
        // Makes sure that child pages do not propagate properties to the Window.
        private class PageTest
        {
            private NavigationWindow    _nw;
            private double              _nwLength = 500;
            
            private Page                _topLevelPage;
            private string              _topLevelPageText = "TopLevelPage Test";
            private double              _topLevelPageLength = 400;

            private double              _innerPageLength = 800;
            
            public PageTest()
            {
                _topLevelPage = new Page();
                _topLevelPage.WindowTitle = _topLevelPageText;
                _topLevelPage.Title = PageApp._ptitle + " Top Level";

                Page pageWithinFrame = new Page();
                pageWithinFrame.WindowTitle = "Page Within Frame Test";
                pageWithinFrame.Title = PageApp._ptitle + " Within Frame";
                pageWithinFrame.WindowWidth = _innerPageLength;
                pageWithinFrame.WindowHeight = _innerPageLength;

                Frame frame = new Frame();
                frame.Navigate(pageWithinFrame);

                _topLevelPage.Content = frame;

                _nw = new NavigationWindow();
                _nw.Width = _nwLength;
                _nw.Height = _nwLength;
                _nw.Navigate(_topLevelPage);

                Page topLevelPage2 = new Page();
                topLevelPage2.WindowTitle = _topLevelPageText + "2";

                Dispatcher.CurrentDispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    (DispatcherOperationCallback) delegate(object unused)
                    {
                        _nw.Navigate(topLevelPage2);
                        return null;
                    },
                    null);

                Dispatcher.CurrentDispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    (DispatcherOperationCallback) delegate(object unused)
                    {
                      _nw.GoBack();
                      return null;
                    },
                    null);

                _nw.Show();

                Dispatcher.CurrentDispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new DispatcherOperationCallback(Verify),
                    this);
            }

            private object Verify(object obj)
            {
                if (_nw.Title.Equals(_topLevelPageText) == false)
                {
                    throw new ApplicationException(String.Format("NavigationWindow title is not that same as top level Page's title. Expected ({0}), Actual ({1})", _topLevelPageText, _nw.Title));
                }

                if ((_nw.ActualWidth == _innerPageLength) ||
                    (_nw.ActualHeight == _innerPageLength))
                {
                    throw new ApplicationException(String.Format("_innerPage Width/Height should not have propagated to the NavigationWindow. Expected (W={0} H={0}), Actual (W={1} H={2})", _nwLength, _nw.ActualWidth, _nw.ActualHeight));
                }

                _topLevelPage.WindowWidth = _topLevelPageLength;
                _topLevelPage.WindowHeight = _topLevelPageLength;

                if ((_nw.ActualWidth != _topLevelPageLength) ||
                    (_nw.ActualHeight != _topLevelPageLength))
                {
                    throw new ApplicationException(String.Format("_topLevelPage Width/Height did not propagate to the NavigationWindow.  Exptected (W={0} H={0}), Actual (W={1} H={1})", _topLevelPageLength, _nw.ActualWidth, _nw.ActualHeight));
                }
                _nw.Close();
                return null;
            }
        }                

        // Tests that Page can be parented by only a Window/Frame.
        private class VisualParentingTest
        {
            System.Resources.ResourceManager rm;
            private NavigationWindow    _nw;
            internal static string      _expectedExceptionText;
            private bool                _exceptionRaised = false;
            
            public VisualParentingTest()
            {
                Assembly assembly = typeof(Page).Assembly;
                rm = new System.Resources.ResourceManager(String.Format("FxResources.{0}.SR", assembly.GetName().Name), assembly);
                _expectedExceptionText = rm.GetString("ParentOfPageMustBeWindowOrFrame");
                _nw = new NavigationWindow();
                _nw.Resources = new System.Windows.ResourceDictionary();
                _nw.Resources.Add("DrtPageNavWindowStyle", this.NavWinStyle());
                _nw.SetResourceReference(System.Windows.FrameworkElement.StyleProperty, "DrtPageNavWindowStyle");

                Page page = new Page();
                page.Title = PageApp._ptitle + " VisualParenting";
                page.WindowWidth = 300;
                page.WindowHeight = 300;
                _nw.Navigate(page);

                _nw.Show();

                Dispatcher.CurrentDispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new DispatcherOperationCallback(NavigatetoInvalidTree),
                    this);

                Dispatcher.CurrentDispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new DispatcherOperationCallback(Verify),
                    this);
            }

            private Style NavWinStyle()
            {
                Style style = new Style(typeof(NavigationWindow));
                return style;
            }

            private object NavigatetoInvalidTree(object obj)
            {
                Application.Current.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(DispatcherExceptionHandler);
                Page page1 = new Page();
                Page page2 = new Page();
                page1.Title = PageApp._ptitle + " Invalid1";
                page2.Title = PageApp._ptitle + " Invalid2";
                ((IAddChild)page1).AddChild(page2);
                _nw.Navigate(page1);
                return null;
            }

            private void DispatcherExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs args)
            {
                if ((args.Exception.GetType() == typeof(InvalidOperationException)) &&
                    (String.Compare(args.Exception.Message,VisualParentingTest._expectedExceptionText, false, System.Globalization.CultureInfo.InvariantCulture) == 0))
                {
                    _exceptionRaised = true;
                    args.Handled = true;
                }
                Application.Current.DispatcherUnhandledException -= new DispatcherUnhandledExceptionEventHandler(DispatcherExceptionHandler);
            }
            
            private object Verify(object obj)
            {
                _nw.Close();
                if (_exceptionRaised == true)
                {
                    ((PageApp)Application.Current)._visualParentingTestPassed = true;
                }
                return null;
            }
        }

        private class SerializationTest
        {
            // throw new ApplicationException
            public SerializationTest()
            {
                TestSerializationWhenNoPropertiesAreSet(
                    "<Page xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" />", 2);
                TestSerializationWhenNoPropertiesAreSet(
                    "<Page xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Title=\"Hello\"/>", 3);
            }

            public void TestSerializationWhenNoPropertiesAreSet(string source, int count)
            {
                // define a page with no non-required properties
                System.IO.MemoryStream stream = new System.IO.MemoryStream(System.Text.ASCIIEncoding.UTF8.GetBytes(source));
                ParserContext pc = new ParserContext();

                // parse the xaml into a pa/ge object
                object tree = XamlReader.Load(stream, pc);

                // serialize back to a string
                string result = XamlWriter.Save(tree);

                // Validate that it serialized cleanly
                stream = new System.IO.MemoryStream(System.Text.UnicodeEncoding.Default.GetBytes(result.ToCharArray()));
                System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(stream);
                reader.Read();

                if (reader.AttributeCount > count)
                {
                    // The serialized page should look like this: 
                    // "<Page xml:space=\"preserve\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" />"
                    // no more attributes should have been serialized.
                    throw new ApplicationException(string.Format("Page was serialized with {0} attributes.  Should have been less.", reader.AttributeCount));
                }
            }
        }
        
        
        private void VerboseOutput(string s)
        {
            if (_verbose == true)
            {
                _logger.Log(s);
            }
        }

        [DllImport("user32.dll", ExactSpelling=true, CharSet=CharSet.Auto)]
        internal extern static void DisableProcessWindowsGhosting();        
    }
}

