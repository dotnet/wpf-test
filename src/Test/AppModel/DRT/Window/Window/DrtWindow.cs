// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Windows.Documents;

using System.Windows.Media;

namespace DRT
{
    public class CrossElement : UIElement
    {
        private int _height;
        private int _width;

        public CrossElement( Dock dock, int width, int height )
        {
           DockPanel.SetDock(this, dock);
           _height = height;
           _width = width;
        }

        protected override Size MeasureCore(Size constraint)
        {
            return new Size(_width, _height);
        }

        protected override void OnRender(DrawingContext ctx)
        {
            ctx.DrawLine(
                new Pen(new SolidColorBrush(Color), 2.0f),
                new Point(0, 0),
                new Point(_width, _height));
            ctx.DrawLine(
                new Pen(new SolidColorBrush(Color), 2.0f),
                new Point(0, _height),
                new Point(_width, 0));
        }

        public Color Color = Color.FromRgb(0x00, 0xff, 0x00);
    }


    public class DrtWindow
    {
        internal static bool _fClosing2Fired; 
        internal static bool _fClosedFired; 
        internal static int _nClosing1Fired;       

        [STAThread]
        public static int Main()
        {

             // Creates an app and a window
             // 1) Closing event is fired
             // 2) Closing is cancelable
             // 3) Closed is fired
             // 3a) Setting Icon as null on shown window should display default icon.
             // 4) Text property is set correctly
             // 5) Tests that the default visual style is correct
             // 6) Sets custom style visual tree and tests its correctness
             // 7) Tests that we throw exceptions when window is 
             //     logically or visually parented.

            WindowTestApplication theApp = new WindowTestApplication();

            try
            {                
                theApp.Run();
            }
            catch(ApplicationException e)
            {
                theApp.Logger.Log(e);
                return 1;
            }

            int retVal = 0;

            if (_nClosing1Fired != 1)
            {
                theApp.Logger.Log( "ERROR: - DrtWindow. Listener to Window.Closing was fired incorrectly or not fired");
                retVal = 1;
            }
            else if (_fClosing2Fired == false) 
            {
                theApp.Logger.Log( "ERROR: - DrtWindow. Listener to Window.Closing not called");
                retVal = 1;
            }
            else if (_fClosedFired == false)
            {
                theApp.Logger.Log("ERROR: - DrtWindow. Listener to Window.Closed not called");
                retVal = 1;
            }
            else
            {
                theApp.Logger.Log("Passed");
            }            

            return retVal;
        }
    }
    public class WindowTestApplication : Application
    {
        private static Logger _logger;

        private Window _win;
        private DockPanel _dockPanel;
        private string _title = "Drt Window";
        CancelEventHandler _onClosing1;
        CrossElement _firstChild;

        internal Logger Logger
        {
            get
            {
                return _logger;
            }
        }

        public WindowTestApplication() : base()
        {
            _logger = new Logger("DrtWindow", "Microsoft", "Testing Window Styling, Icon, parenting and Close related events");

            _onClosing1 = new CancelEventHandler(OnClosing1);
        }

        protected override void OnStartup(StartupEventArgs e) 
        {
            UIElementCollection dockChildren;
            _dockPanel = new DockPanel();
            _firstChild = new CrossElement(Dock.Left, 200, 600);
            _win = new Window
            {
                Title = _title,
                Top = 100,
                Left = 100,
                Width = 400,
                Height = 400,
            };
            dockChildren = _dockPanel.Children;
            dockChildren.Add(_firstChild);
            dockChildren.Add(new CrossElement(Dock.Right, 200, 600));
            dockChildren.Add(new CrossElement(Dock.Bottom, 400, 300));
            dockChildren.Add(new CrossElement(Dock.Left, 400, 300)); 	// Child will Fill
            _win.Content = _dockPanel;
            _win.Closing += _onClosing1;
            _win.Closed += new EventHandler(OnClosed);
            _win.Show();
            TestIcon();
            //the logic tree
            if ( _dockPanel.Parent != _win )
            {
                throw new ApplicationException( "Logical Parent of first CrossElement is not the Window object.");
            }

            VerifyDefaultWindowStyleVisualTree();

            //create new style and apply
            _win.Resources = new System.Windows.ResourceDictionary();
            _win.Resources.Add("DrtWindow", this.ApplyNewStyle());
            _win.SetResourceReference(System.Windows.FrameworkElement.StyleProperty, "DrtWindow");            

            _win.ResizeMode = ResizeMode.CanResizeWithGrip;

            TestWindowParenting();
            
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(CloseWindow),
                this);
            
            base.OnStartup(e);
        }

        private void TestIcon()
        {
            _win.Icon = null;
            // setting Icon to null should reset window to use default icon
            // icon getter should return null if Icon is set to null.
            if (_win.Icon != null)
            {
                throw new ApplicationException(String.Format("Setting Window Icon to null didn't retreive null for Icon property; Icon property set value={0}, Icon property get value={1}", null, _win.Icon));
            }
        }
        
        private void VerifyDefaultWindowStyleVisualTree()
        {
            //walk down the visual tree to make sure it is right.
            Border mainBorder = VisualTreeHelper.GetChild(_win, 0) as Border;
            if (mainBorder == null)
            {
                throw new ApplicationException("Window Styling: The first visual child of Window is not a Border");
            }                        

            AdornerDecorator adornerDecorator = VisualTreeHelper.GetChild(mainBorder, 0) as AdornerDecorator;
            if (adornerDecorator == null)
            {
                throw new ApplicationException("Window Styling: The first visual child of grid is not AdornerDecorator");
            }
          
            ContentPresenter cp = VisualTreeHelper.GetChild(adornerDecorator, 0) as ContentPresenter;
            if (cp == null)
            {
                throw new ApplicationException("Window Styling: The visual child of adornerDecorator is not ContentPresenter");
            }            
        }
        
        private Style ApplyNewStyle()
        {
            Style ds = new Style(typeof(Window));
            // Note that we can't set Top/Left/Width/Height in the style because the current implementation
            // of Window doesn't respect those properties set through a style.

            ControlTemplate template = new ControlTemplate(typeof(Window));
            Trigger trigger;
       
            // border
            FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border), "Border");
            border.SetValue(Border.BackgroundProperty, new SolidColorBrush(Colors.Aqua));

            //dockpanel
            FrameworkElementFactory dockPanel = new FrameworkElementFactory(typeof(DockPanel), "DockPanel");
            //button
            FrameworkElementFactory backButton = new FrameworkElementFactory(typeof(Button), "Button");
            backButton.SetValue(DockPanel.DockProperty, Dock.Left);
            FrameworkElementFactory backButtonBorder = new FrameworkElementFactory(typeof(Border), "ButtonImageContent");
            backButtonBorder.SetValue(Button.WidthProperty, 54.0);
            backButtonBorder.SetValue(Button.HeightProperty, 38.0);
            backButton.SetValue(DockPanel.DockProperty, Dock.Top);
            backButton.AppendChild(backButtonBorder);

            // Bottom DockPanel
            FrameworkElementFactory bottomDockPanel = new FrameworkElementFactory(typeof(DockPanel), "BottomDockPanel");
            bottomDockPanel.SetValue(DockPanel.DockProperty, Dock.Bottom);            
            
            // Brushes for fore/back ground
            SolidColorBrush backgroundBrush = new SolidColorBrush(Colors.Yellow);
            backgroundBrush.Freeze();
            SolidColorBrush foregroundBrush = new SolidColorBrush(Colors.Red);
            foregroundBrush.Freeze();

            //ResizeGrip
            FrameworkElementFactory resizeGrip = new FrameworkElementFactory(typeof(ResizeGrip), "ResizeGrip");
            resizeGrip.SetValue(UIElement.VisibilityProperty, Visibility.Collapsed);
            resizeGrip.SetValue(DockPanel.DockProperty, Dock.Right);
            resizeGrip.SetValue(FrameworkElement.WidthProperty, 14.0d);
            resizeGrip.SetValue(FrameworkElement.HeightProperty, 23.0d);
            resizeGrip.SetValue(Control.BackgroundProperty, backgroundBrush);
            resizeGrip.SetValue(Control.ForegroundProperty, foregroundBrush);
            resizeGrip.SetValue(Control.IsTabStopProperty, false);

            trigger = new Trigger();
            trigger.Property = Window.ResizeModeProperty;
            trigger.Value = ResizeMode.CanResizeWithGrip;
            trigger.Setters.Add(new Setter(UIElement.VisibilityProperty, Visibility.Visible, "ResizeGrip"));
            template.Triggers.Add(trigger);

            // Make bottom visual tree
            bottomDockPanel.AppendChild(resizeGrip);
            
            // content presenter
            FrameworkElementFactory cp = new FrameworkElementFactory(typeof(ContentPresenter), "ContentSite");
            cp.SetValue(ContentControl.ContentProperty, new TemplateBindingExtension(Window.ContentProperty));

            border.AppendChild(dockPanel);
            dockPanel.AppendChild(backButton);
            dockPanel.AppendChild(bottomDockPanel);
            dockPanel.AppendChild(cp);

            template.VisualTree = border;
            ds.Setters.Add(new Setter(Control.TemplateProperty, template));

            return ds;
        }

        private void OnClosing1(object sender, CancelEventArgs e)
        {
            DrtWindow._nClosing1Fired++;
            Window win = sender as Window;
            if (win == null)
            {
                throw new ApplicationException ("sender in Window.Closing handler is not a Window");
            }
            e.Cancel = true;
            _win.Closing -= _onClosing1;
            _win.Closing += new CancelEventHandler(OnClosing2);

            _dockPanel.Children.Remove(_firstChild);
            // try to close the window again
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(CloseWindow),
                this);
        }


        private void OnClosing2(object sender, CancelEventArgs e)
        {
            DrtWindow._fClosing2Fired = true;

            Window win = sender as Window;
            if (win == null)
            {
                throw new ApplicationException ("sender in Window.Closing handler is not a Window");
            }
        
            if (win.Title != _title)
            {
                throw new ApplicationException ("win.Title != _title in Window.Closing handler");
            }

            if (win.Visibility != Visibility.Visible)
            {
                throw new ApplicationException ( "win.Visibility != Visibility.Visible in Window.Closing handler" );
            }                                
        }

        private void OnClosed(object sender, EventArgs e)
        {
            DrtWindow._fClosedFired = true;

            Window win = sender as Window;
            if ( win == null)
            {
                throw new ApplicationException("sender in Window.Closed Handler is not a Window");
            }
        }

        private void TestWindowParenting()
        {
            TestVisualWindowParenting();
            TestLogicalWindowParenting();
        }
        private void TestVisualWindowParenting()        
        {
            ContainerVisual cv = new ContainerVisual();
            Window window = new Window();

            // Test Visual parenting
            try
            {
                cv.Children.Add(window);
                window.Show();
            }
            catch (InvalidOperationException)
            {
                cv.Children.Remove(window);              
                window.Close();            
                return;
            }

            throw new ApplicationException("Visual parenting of Window did not throw InvalidOperationExceptoin");    
        }           

        private void TestLogicalWindowParenting()        
        {
            Window window = new Window();  
            try
            {
                window.Show();
                _win.Content = window;
            }
            catch(InvalidOperationException)
            {
                _win.Content = null;
                window.Close();                
                return;
            }
            throw new ApplicationException("Logical parenting of Window did not throw InvalidOperationExceptoin");                
        }
        
        private object CloseWindow(object obj)
        {
            if ((_win.Top != 100) || (_win.Left != 100) || (_win.ActualWidth != 400) || (_win.ActualHeight != 400))
            {
                throw new ApplicationException("Window: T/L/W/H did not pick up from explicit properties");
            }

            //verify whether the new style's been applied before closing           
            Border border = VisualTreeHelper.GetChild(_win,0) as Border;
            if (border == null)
            {
                throw new ApplicationException("Window Styling: The first visual child of Window is not Border");
            }

            DockPanel dockpanel = VisualTreeHelper.GetChild(border,0)  as DockPanel;
            if (dockpanel == null)
            {
                throw new ApplicationException("Window Styling: The first visual child of Border is not DockPanel");
            }
       
            Button button = VisualTreeHelper.GetChild(dockpanel,0) as Button;
            if (button == null)
            {
                throw new ApplicationException("Window Styling: The first visual child of the dockpanel is not Button");
            }        

            DockPanel bottomDockPanel = VisualTreeHelper.GetChild(dockpanel,1) as DockPanel;
            if (bottomDockPanel == null)
            {
                throw new ApplicationException("Window Styling: The second visual child of the dockpanel is not the BottomDockPanel");
            }

            ContentPresenter cp = VisualTreeHelper.GetChild(dockpanel,2) as ContentPresenter;
            if (cp == null)
            {
                throw new ApplicationException("Window Styling: The third visual child of the dockpanel is not ContentPresenter");
            }

            ResizeGrip rg = VisualTreeHelper.GetChild(bottomDockPanel,0) as ResizeGrip;
            if (rg == null)
            {
                throw new ApplicationException("Window Styling: The first visual child of the BottomDockPanel is not the ResizeGrip");
            }
            _win.Close();

            return null;
        }
    }
}


