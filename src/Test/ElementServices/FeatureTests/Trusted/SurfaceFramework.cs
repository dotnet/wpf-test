// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************************
*    Source Control Information
*    
 
  
*    Revision:         $Revision: 2 $
 
******************************************************************************/
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;

using Microsoft.Test.WindowsForms;
using Microsoft.Test.Win32;

using SWF = System.Windows.Forms;

namespace Avalon.Test.CoreUI.Trusted
{
    /// <summary>
    /// Abstracts all the surfaces that exists at the Core and Framework 
    /// product layers.
    /// 
    /// Surfaces: 
    ///     HwndSource (aggregated from Core layer)
    ///     Window
    ///     NavigationWindow
    ///     Browser
    ///     Popup
    /// </summary>
    public class SurfaceFramework : SurfaceCore
    {
        /// <summary>
        /// static ctor.
        /// </summary>
        static SurfaceFramework()
        {
            // Register all publicly accessible Framework surfaces.
            Surface.RegisterSurface("Window", typeof(Window));
            Surface.RegisterSurface("NavigationWindow", typeof(NavigationWindow));
            Surface.RegisterSurface("WindowsFormSource", typeof(WindowsFormSource));
            Surface.RegisterSurface("Popup", typeof(Popup));

            // RootBrowserWindow (belonging to IE as a host) can also be a Framework surface.
            // We treat it specially. 

            // Register RootBrowserWindow's inner object as our surface.
            Type windowType = typeof(Window);
            Assembly assembly = windowType.Assembly;
            Type browserWindowType = assembly.GetType("MS.Internal.AppModel.RootBrowserWindow");

            Surface.RegisterSurface("RootBrowserWindow", browserWindowType);
        }

        /// <summary>
        /// Constructor for wrapping an existing surface with a
        /// SurfaceCore or SurfaceFramework object.
        /// </summary>
        public SurfaceFramework(object objectSurface)
            : base(objectSurface)
        {
            if (objectSurface is Window)
            {
                _window = (Window)objectSurface;
                surfaceObject = _window;

                Type type = objectSurface.GetType();

                if (type.FullName.IndexOf("RootBrowserWindow") != -1)
                {
                    _hostWindowType = HostWindowType.RootBrowserWindow;

                    IntPtr browserHandle = GetBrowserHandle();

                    HookWindowMessages(browserHandle, "Browser");
                }
            }
            else if (objectSurface is WindowsFormSource)
            {
                surfaceObject = objectSurface;
            }
            else if (objectSurface is Popup)
            {
                surfaceObject = objectSurface;
            }

            // Assume user supplied surface is already visible.
            isVisible = true;

            HookWindowMessages(this.Handle, "Surface");            
        }

        /// <summary>
        /// This constructor creates the specified surface with given dimensions.
        /// </summary>
        /// <remarks>
        /// It first checks with the base class, Core, to find if it is a core surface.
        /// If not, it will come here and find if it is a Framework surface.
        /// If so, it instantiates the specified surface.
        /// </remarks>
        public SurfaceFramework(string typeOfSurface,
            int left,
            int top,
            int width,
            int height)
            : base(typeOfSurface, left, top, width, height)
        {
            Initialize(typeOfSurface, left, top, width, height, true);
        }

        /// <summary>
        /// Constructor that will create the surface that is requested.
        /// </summary>
        public SurfaceFramework(string typeOfSurface,
            int left,
            int top,
            int width,
            int height,
            bool visibleSurface)
            : base(typeOfSurface, left, top, width, height, visibleSurface)
        {
            Initialize(typeOfSurface, left, top, width, height, visibleSurface);
        }

        /// <summary>
        /// Close the window         
        /// </summary>
        public override void Close()
        {
            base.Close();

            if (_window != null && _hostWindowType != HostWindowType.RootBrowserWindow)
            {
                System.Security.PermissionSet set = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
                set.AddPermission(new System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode));
                set.Assert();

                _window.Close();
            }
            else if (surfaceObject is WindowsFormSource)
            {
                WindowsFormSource wfs = (WindowsFormSource)surfaceObject;
                wfs.Close();
            }
            else if (surfaceObject is Popup)
            {
                Popup popup = (Popup)surfaceObject;

                popup.IsOpen = false;
            }
        }

        /// <summary>
        /// </summary>
        public override Point DeviceUnitsFromMeasureUnits(Point point)
        {
            if (_window != null)
            {
                PresentationSource source = Interop.PresentationSourceFromVisual(_window);
                return source.CompositionTarget.TransformToDevice.Transform(point);
            }
            else if (surfaceObject is WindowsFormSource)
            {
                WindowsFormSource wfs = (WindowsFormSource)surfaceObject;               
                return wfs.CurrentHwndSource.CompositionTarget.TransformToDevice.Transform(point);
            }
            else if (surfaceObject is Popup)
            {
                Popup popup = (Popup)surfaceObject;

                PresentationSource source = (PresentationSource)GetPopupHwndSource(popup);
                return source.CompositionTarget.TransformToDevice.Transform(point);
            }

            return base.DeviceUnitsFromMeasureUnits(point);
        }

        /// <summary>
        /// This method will present the specified object on the Surface
        /// </summary>
        public override void DisplayObject(object visual)
        {
            if (_window != null)
            {
                Log("Displaying Object");
                if (_window is NavigationWindow)
                {
                    Log("..navigate to it");
                    bool bResult = ((NavigationWindow)_window).Navigate(visual);
                    Log("..successful? " + bResult);
                }
                else
                {
                    Log("..replace content with it");
                    _window.Content = visual;
                }
            }
            else if (surfaceObject is WindowsFormSource)
            {
                WindowsFormSource wfs = (WindowsFormSource)surfaceObject;
                wfs.RootVisual = (UIElement)visual;
            }
            else if (surfaceObject is Popup)
            {
                Popup popup = (Popup)surfaceObject;

                popup.Child = (UIElement)visual;
            }
            
            base.DisplayObject(visual);
        }

        /// <summary>
        /// Go back.
        /// </summary>
        public override void GoBack()
        {
            base.GoBack();

            if (_window != null)
            {
                if (_window is NavigationWindow)
                {
                    Log("Going Back");
                    ((NavigationWindow)_window).GoBack();
                }
                else
                {
                    Log("Navigation functionality is not implemented on SurfaceFramework for non-NavigationWindow objects.");
                }
            }
        }

        /// <summary>
        /// Go forward.
        /// </summary>
        public override void GoForward()
        {
            if ((_window != null) && (_window is NavigationWindow))
            {
                Log("Going Forward");
                ((NavigationWindow)_window).GoForward();
            }
            else
            {
                Log("Navigation functionality is not implemented on SurfaceFramework for non-NavigationWindow objects.");
            }
        }

        /// <summary>
        /// Force the Surface to be active.
        /// </summary>
        public override void ForceActivation()
        {
            IntPtr handle;
            if (_hostWindowType == HostWindowType.RootBrowserWindow)
            {
                handle = GetBrowserHandle();
            }
            else
            {
                handle = this.Handle;
            }
            ForceActivationCore(handle);
        }

        /// <summary>
        /// Return the PresentationSource for the Surface.
        /// </summary>
        public override PresentationSource GetPresentationSource()
        {
            PresentationSource baseSource = base.GetPresentationSource();
            if (baseSource != null)
            {
                return baseSource;
            }

            if (_window != null)
            {
                new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Assert();

                return PresentationSource.FromVisual(_window);
            }
            else if (surfaceObject is WindowsFormSource)
            {
                WindowsFormSource wfs = (WindowsFormSource)surfaceObject;
                return wfs.CurrentHwndSource;
            }
            else if (surfaceObject is Popup)
            {
                HwndSource popupSource = GetPopupHwndSource((Popup)surfaceObject);
                return popupSource;
            }

            return null;
        }

        /// <summary>
        /// </summary>
        public override Point MeasureUnitsFromDeviceUnits(Point devicePoint)
        {
            if (_window != null)
            {
                PresentationSource source = PresentationSource.FromVisual(_window);
                return source.CompositionTarget.TransformFromDevice.Transform(devicePoint);
            }
            else if (surfaceObject is WindowsFormSource)
            {
                WindowsFormSource wfs = (WindowsFormSource)surfaceObject;               
                return wfs.CurrentHwndSource.CompositionTarget.TransformFromDevice.Transform(devicePoint);
            }
            else if (surfaceObject is Popup)
            {
                PresentationSource source = (PresentationSource)GetPopupHwndSource((Popup)surfaceObject);
                return source.CompositionTarget.TransformFromDevice.Transform(devicePoint);
            }

            return base.MeasureUnitsFromDeviceUnits(devicePoint);
        }


        /// <summary>
        /// This method will present the specified object on the surface
        /// </summary>
        public override void SetPosition(int left, int top)
        {
            base.SetPosition(left, top);

            if (_window != null)
            {
                Log("Setting Window Position");
                if (_hostWindowType == HostWindowType.RootBrowserWindow)
                {
                    IntPtr ieHwnd = GetBrowserHandle();

                    if (ieHwnd == IntPtr.Zero)
                    {
                        return;
                    }

                    NativeMethods.SetWindowPos(ieHwnd, IntPtr.Zero, Left, Top, 0, 0, NativeConstants.SWP_NOSIZE);

                }
                else
                {
                    _window.Top = Top;
                    _window.Left = Left;
                }
            }
            else if (surfaceObject is WindowsFormSource)
            {
                WindowsFormSource wfs = (WindowsFormSource)surfaceObject;

                wfs.Left = Left;
                wfs.Top = Top;
            }
            else if (surfaceObject is Popup)
            {
                Popup popup = (Popup)surfaceObject;

                popup.VerticalOffset = (double)Top;
                popup.HorizontalOffset = (double)Left;
            }
        }


        /// <summary>
        /// This method will present the specified object on the surface
        /// </summary>
        public override bool SetSize(int width, int height)
        {
            bool x = base.SetSize(width, height);

            if (!x)
            {
                return false;
            }
            
            if (_window != null && x)
            {
                Log("Setting Window Size");
                if (_hostWindowType == HostWindowType.RootBrowserWindow)
                {
                    ResizeBrowser(WidthPixels, HeightPixels);
                }
                else
                {
                    _window.Width = Width;
                    _window.Height = Height;
                }
            }
            else if (surfaceObject is WindowsFormSource)
            {
                WindowsFormSource wfs = (WindowsFormSource)surfaceObject;

                wfs.Resize(WidthPixels,HeightPixels);
            }
            else if (surfaceObject is Popup)
            {
                Popup popup = (Popup)surfaceObject;
                
                popup.Width = Width;
                popup.Height = Height;
            }

            return true;
        }


        /// <summary>
        /// This method will present the specified object on the surface
        /// </summary>
        public override bool SetSizePixels(int width, int height)
        {
            bool x = base.SetSizePixels(width, height);

            if (!x)
            {
                return false;
            }
            
            if (_window != null)
            {
                Log("Setting Window Size");
                if (_hostWindowType == HostWindowType.RootBrowserWindow)
                {
                    ResizeBrowser(WidthPixels, HeightPixels);
                }
                else
                {
                    _window.Width = WidthPixels;
                    _window.Height = HeightPixels;
                }
            }
            else if (surfaceObject is WindowsFormSource)
            {
                WindowsFormSource wfs = (WindowsFormSource)surfaceObject;

                wfs.Resize(WidthPixels,HeightPixels);
            }
            else if (surfaceObject is Popup)
            {
                Popup popup = (Popup)surfaceObject;

                popup.Width = Width;
                popup.Height = Height;
            }

            return true;

        }

        /// <summary>
        /// Displays the surface. 
        /// </summary>
        public override void Show()
        {
            base.Show();

            if (_window != null)
            {
                _window.Show();
            }
            else if (surfaceObject is WindowsFormSource)
            {
                WindowsFormSource wfs = (WindowsFormSource)surfaceObject;
                wfs.Show();
            }
            else if (surfaceObject is Popup)
            {
                Popup popup = (Popup)surfaceObject;

                popup.IsOpen = true;
            }
        }

        /// <summary>
        /// Displays the surface in as Modal
        /// </summary>
        public override void ShowModal()
        {
            base.ShowModal();

            if (_window != null)
            {
                System.Security.PermissionSet set = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
                set.AddPermission(new System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode));
                set.Assert();

                _window.ShowDialog();
            }
            else if (surfaceObject is WindowsFormSource)
            {
                WindowsFormSource wfs = (WindowsFormSource)surfaceObject;
                wfs.Show();
            }
            else if (surfaceObject is Popup)
            {
                throw new NotSupportedException("Modal Popup");
            }
        }

        /// <summary>
        /// Retrieves the HWND for the surface.
        /// </summary>
        [UIPermission(SecurityAction.Assert, Unrestricted = true)]
        protected override IntPtr GetHandle()
        {
            IntPtr handle = base.GetHandle();

            if (_window != null)
            {
                HandleRef helper = Interop.HandleRefFromWindow(_window);

                handle = helper.Handle;
            }
            else if (surfaceObject is WindowsFormSource)
            {
                WindowsFormSource wfs = (WindowsFormSource)surfaceObject;
                handle = wfs.CurrentHwndSource.Handle;
            }
            else if (surfaceObject is Popup)
            {
                // Popup doesn't create an HwndSource until it is open.
                if (!isVisible)
                    throw new Exception("Cannot get Popup handle until Popup is open");

                HwndSource source = GetPopupHwndSource((Popup)surfaceObject);
                handle = source.Handle;
            }

            return handle;
        }

        private IntPtr GetBrowserHandle()
        {
            IntPtr ieHwnd = IntPtr.Zero;

            if (_hostWindowType == HostWindowType.RootBrowserWindow)
            {
                IntPtr tmp = NativeMethods.GetParent(new HandleRef(null, this.Handle));
                while (IntPtr.Zero != tmp)
                {
                    ieHwnd = tmp;
                    tmp = NativeMethods.GetParent(new HandleRef(null, tmp));
                }
            }

            return ieHwnd;
        }

        private HwndSource GetPopupHwndSource(Popup popup)
        {
            Type popupType = popup.GetType();

            // Get the private PopupSecurityHelper field, instance and type from the Popup
            FieldInfo popupSecurityHelperField = popupType.GetField("_secHelper", BindingFlags.NonPublic | BindingFlags.Instance);
            if (null == popupSecurityHelperField)
                throw new Exception("Could not get private Popup field _secHelper.");
            object popupSecurityHelperInstance = popupSecurityHelperField.GetValue(popup);
            Type popupSecurityHelperType = popupSecurityHelperInstance.GetType();

            // Get _window SecurityCriticalDataClass field, instance and type from the security helper.
            FieldInfo securityWrapperField = popupSecurityHelperType.GetField("_window", BindingFlags.NonPublic | BindingFlags.Instance);
            if (null == securityWrapperField)
                throw new Exception("Could not get private PopupSecurityHelper field _window.");
            object securityWrapperInstance = securityWrapperField.GetValue(popupSecurityHelperInstance);
            Type securityWrapperType = securityWrapperInstance.GetType();

            // Get the FieldInfo and instance for the Value in the security data class, this is the popup's HwndSource.
            FieldInfo valueField = securityWrapperType.GetField("_value", BindingFlags.Instance | BindingFlags.OptionalParamBinding | BindingFlags.NonPublic);
            if (null == valueField)
                throw new Exception("Could not get private _value field from SecurityCriticalDataHelper in PopupSecurityHelper.");

            return (HwndSource)valueField.GetValue(securityWrapperInstance);
        }


        private void Initialize(string typeOfSurface,
            int left,
            int top,
            int width,
            int height,
            bool visibleSurface)
        {
            System.Security.PermissionSet set = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            set.AddPermission(new System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode));
            set.AddPermission(new System.Security.Permissions.UIPermission(System.Security.Permissions.UIPermissionWindow.AllWindows));

            _isVisibleChangedHandler = new DependencyPropertyChangedEventHandler(_OnIsVisibleChanged);
            _isVisibleFormChangedHandler = new EventHandler(_OnIsVisibleFormChanged);
            
            switch (typeOfSurface)
            {
                case "WindowsFormSource":

                    Log("Creating a WindowsFormSource object");
                    
                    WindowsFormSource wfs = new WindowsFormSource();
                    surfaceObject = wfs;

                    wfs.VisibleChanged += _isVisibleFormChangedHandler;

                    wfs.Visible = visibleSurface;
                    SetPosition(left, top);
                    SetSize(width, height);
                    
                    break;
                
                case "Window":

                    Log("Creating a Window object");

                    set.Assert();

                    _window = new Window();
                    _window.Title = "Window";
                    surfaceObject = _window;

                    _window.IsVisibleChanged += _isVisibleChangedHandler;

                    if (visibleSurface)
                    {
                        _window.Show();
                        SetPosition(left, top);
                        SetSize(width, height);
                    }

                    break;

                case "NavigationWindow":

                    Log("Creating a NavigationWindow object");

                    set.Assert();

                    NavigationWindow navWindow = new NavigationWindow();
                    _window = navWindow;
                    _window.Title = "NavigationWindow";
                    surfaceObject = _window;

                    // To update the root displayed object field, it is absolutely necessary
                    // to handle LoadCompleted for each completed navigation.
                    // rootDisplayedObject should be altered inside this event handler.
                    navWindow.LoadCompleted += new LoadCompletedEventHandler(_OnNavigationWindowLoaded);

                    _window.IsVisibleChanged += _isVisibleChangedHandler;

                    // The chrome (Back and Forward buttons) on the Window messes up our measurements.
                    // For testing purposes, we need to take account of it on our surface.
                    // We do this by auto-expanding the height of the window.

                    // Alternately, it may be more robust to just ditch the chrome altogether:
                    ////_window.Style = BuildChromelessStyle();

                    if (visibleSurface)
                    {
                        navWindow.Show();
                        double chromeHeight = GetChromeHeight(navWindow);
                        double newWindowHeight = (height + chromeHeight);
                        SetPosition(left, top);
                        SetSize(width, (int)newWindowHeight);
                    }

                    break;

                case "Popup":

                    Log("Creating a Popup object");

                    Popup popup = new Popup();
                    surfaceObject = popup;

                    popup.Opened += _OnPopupOpened;

                    if (visibleSurface)
                    {
                        popup.IsOpen = true;
                        SetPosition(left, top);
                        SetSize(width, height);
                    }

                    break;
            }
        }

        private void _OnPopupOpened(object sender, EventArgs e)
        {
            OnVisibleChanged(true);
            ((Popup)sender).Opened -= _OnPopupOpened;
        }

        // Executes when new content has completed loading inside 
        // the Navigation Window.
        private void _OnNavigationWindowLoaded(object sender, NavigationEventArgs e)
        {
            Log("Nav window loaded. ContentType='" + e.Content + "'");
            rootDisplayedObject = e.Content;
            Log("... updated rootDisplayedObject");
        }

        // Listens until the window is visible, then makes the call
        // to hook hwnd messages.
        private void _OnIsVisibleFormChanged(object sender, EventArgs args)
        {
            OnVisibleChanged(((WindowsFormSource)sender).Visible);
            if (((WindowsFormSource)sender).Visible)
            {
                ((WindowsFormSource)sender).VisibleChanged -= _isVisibleFormChangedHandler;
            }
        }


        // Listens until the window is visible, then makes the call
        // to hook hwnd messages.
        private void _OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            OnVisibleChanged((bool)args.NewValue);
            if ((bool)args.NewValue)
            {
                ((UIElement)sender).IsVisibleChanged -= _isVisibleChangedHandler;            
            }
        }

        // Add message hook to surface.
        void OnVisibleChanged(bool value)
        {
            if (value)
            {
                isVisible = true;
                //Dispatcher.CurrentDispatcher.BeginInvoke(
                //    DispatcherPriority.Loaded,
                //    (DispatcherOperationCallback)delegate(object o)
                //    {
                //        HookWindowMessages(this.Handle, "Surface");
                //        return null;
                //    },
                //    null);
            }
        }


        /// <summary>
        /// Get height of chrome in a Navigation Window.
        /// </summary>
        /// <param name="window">Window</param>
        /// <returns>Height in default units (double type) if chrome can be detected, 0 otherwise.</returns>
        /// <remarks>
        /// This method attempts to detect a Back button in the chrome, 
        /// and tries to use the height of its immediate containing panel to approximate the height of the chrome.
        /// </remarks>
        private double GetChromeHeight(NavigationWindow window)
        {
            FrameworkElement btn = GetBackButton(window);
            if (btn != null)
            {
                // We have back button ... find containing panel if any
                Panel p = btn.Parent as Panel;
                if (p != null)
                {
                    // There's a containing panel. Use its height.
                    return p.ActualHeight;
                }
                else
                {
                    // We can't find a containing panel for our back button.
                    // Best guess is to assume the chrome is as tall as the button.
                    return btn.ActualHeight;
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Retrieve back button in a Navigation Window.
        /// </summary>
        /// <param name="window">Window</param>
        /// <returns>FrameworkElement if back button exists, null if it doesn't.</returns>
        /// <remarks>
        /// This method assumes every navigation window style contains a Back button 
        /// with the correct binding to NavigationCommands.BrowseBack.
        /// </remarks>
        private FrameworkElement GetBackButton(NavigationWindow window)
        {
            return (ElementUtils.FindVisualByPropertyValue(
                window,
                Button.CommandProperty,
                NavigationCommands.BrowseBack,
                true) as FrameworkElement);
        }

        /// <summary>
        /// Create and return a NavigationWindow style that removes all chrome from the window.
        /// </summary>
        /// <returns>Style object.</returns>
        private Style BuildChromelessStyle()
        {
            Style chromeRemoved = new Style(typeof(NavigationWindow));

            FrameworkElementFactory cp = new FrameworkElementFactory(typeof(ContentPresenter), "ContentPresenter");
            cp.SetValue(ContentControl.ContentProperty, new TemplateBindingExtension(Window.ContentProperty));

            FrameworkElementFactory dockPanel = new FrameworkElementFactory(typeof(DockPanel), "DockPanel");
            dockPanel.SetValue(DockPanel.LastChildFillProperty, true);
            dockPanel.SetValue(DockPanel.BackgroundProperty, new SolidColorBrush(Colors.AliceBlue));

            dockPanel.AppendChild(cp);

            FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border), "Border");
            border.AppendChild(dockPanel);

            ControlTemplate template = new ControlTemplate(typeof(NavigationWindow));
            template.VisualTree = border;
            chromeRemoved.Setters.Add(new Setter(NavigationWindow.TemplateProperty, template));
            return chromeRemoved;
        }


        private void ResizeBrowser(int w, int h)
        {
            IntPtr ieHwnd = GetBrowserHandle();

            if (ieHwnd == IntPtr.Zero)
            {
                return;
            }

            NativeStructs.RECT rcIE = new NativeStructs.RECT();
            NativeMethods.GetWindowRect(new HandleRef(null, ieHwnd), ref rcIE);

            NativeStructs.RECT rcAvalon = new NativeStructs.RECT();
            NativeMethods.GetWindowRect(new HandleRef(null, this.Handle), ref rcAvalon);

            int diffTop = rcAvalon.top - rcIE.top;

            NativeMethods.SetWindowPos(ieHwnd, IntPtr.Zero, 0, 0, w, h + diffTop + 50, NativeConstants.SWP_NOMOVE);
        }


        private Window _window = null;
        private HostWindowType _hostWindowType = HostWindowType.Window;
        private DependencyPropertyChangedEventHandler _isVisibleChangedHandler = null;
        private EventHandler _isVisibleFormChangedHandler = null;

        /// <summary>
        /// </summary>
        enum HostWindowType
        {
            /// <summary>
            /// </summary>
            RootBrowserWindow,
            /// <summary>
            /// </summary>
            Window,
            /// <summary>
            /// </summary>
            NavigationWindow,
            /// <summary>
            /// </summary>
            WindowsFormSource
        }
    }
}

