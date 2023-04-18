// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using MTI = Microsoft.Test.Input;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;
using Microsoft.Test.Globalization;
using Microsoft.Test.Deployment;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// This Class helps testing Browser operations for WPF Navigation XBAP tests 
    /// It is OK to set SecurityAction to Full Trust because this library does not test anything. 
    /// It only invokes actions on IE such as "Click Back Button". 
    /// We need FullTrust because UIA throws exceptions on different conditions that are non-deterministic (to whoever wrote this comment)
    /// Given the time and hand and the state of these tests, it is best to work around UIA issues right now
    /// </summary>
    [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
    public class BrowserHelper : IDisposable
    {
        private OSVersion _osVersion;
        public OSVersion Version
        {
            get { return _osVersion; }
        }

        private IntPtr _IE_windowhandle;
        private Guid _IID_IAccessible;
        private AutomationElement _ieMenuAutomationElement,_currentAutomationElement;
        private AutoResetEvent _waiter;
        private IntPtr _currentMenuHandle;
        bool _fInvokedFromToolbar = false;
        private AutomationElement _rootIEAutomationElement;
        private WebBrowserInteropProxy _wbInteropProxy;
        private IBrowserUIProvider _browserUIHelper;
        public IBrowserUIProvider BrowserUIHelper
        {
            get { return _browserUIHelper; }
        }
        private const string HtmlContentType = "HTML Document";
        private const string WpfWindowClass = "DocObject_Top_Class";
        private const string WpfContentType = "Web Browser Application";
        private bool _disposed = false;
        /// <summary>
        /// had to create this variable since the window closed event was being fired
        /// like wildfire and causing double disposes or huge loops and insane CPU usage. It is used in function OnWindowClose
        /// </summary>
        private bool _IEWindowClosed = false;

        #region constructors
        /// <summary>
        /// testcases:
        ///     FromVisual of v is null since the visual is not added to any visual tree
        /// </summary>
        private BrowserHelper(Visual visualHostedInBrowser)
        {
            if (visualHostedInBrowser == null)
            {
                throw new ApplicationException("TODO1");
            }

            if (!IsHostedInBrowser(visualHostedInBrowser))
            {
                throw new ApplicationException("Visual is not hosted in browser or is not connected in the tree");
            }

            Initialize();
            HwndSource ps = PresentationSource.FromVisual(visualHostedInBrowser) as HwndSource;
            if (ps == null)
            {
                throw new ApplicationException(
                    "Could not find the window from the visual. Possibly the visual is not connected to the visual tree");
            }

            IntPtr site_windowhandle = ps.Handle;
            _IE_windowhandle = GetAncestor(site_windowhandle, GA_ROOT);
            if (_IE_windowhandle == IntPtr.Zero)
            {
                throw new ApplicationException("Handle of IE window is Zero");
            }
            else
            {
                _wbInteropProxy = new WebBrowserInteropProxy(_IE_windowhandle);
            }

            SetAutomationRoot();
        }

        private BrowserHelper(string startUri)
        {
            Initialize();
            _wbInteropProxy = new WebBrowserInteropProxy();

            if (!String.IsNullOrEmpty(startUri))
            {
                // a start Uri was specified, we need to navigate to that
                _wbInteropProxy.NavigateAndWait(startUri);
            }

            _IE_windowhandle = _wbInteropProxy.GetHwnd();

            if (_IE_windowhandle == IntPtr.Zero)
            {
                throw new ApplicationException("Could not create IE window.");
            }
            SetAutomationRoot();
        }

        private BrowserHelper(IntPtr topLevelIEWindowHandle)
        {
            if (topLevelIEWindowHandle == IntPtr.Zero)
            {
                throw new ArgumentException("Handle of IE window is Zero");
            }

            Initialize();
            _IE_windowhandle = topLevelIEWindowHandle;
            _wbInteropProxy = new WebBrowserInteropProxy(_IE_windowhandle);
            SetAutomationRoot();
        }
        #endregion

        #region public methods

        /// <summary>
        /// </summary>
        public static BrowserHelper GetMainBrowserWindow(Application napp)
        {
            Window w = null;
            if (napp.MainWindow == null)
            {
                // No main window in the Application
                if (napp.Windows.Count <= 0)
                {
                    throw new ApplicationException(
                        "No windows in the application. Therefore no windows of the application are BrowserHelper hosted");
                }
                else
                {
                    w = napp.Windows[0];
                }
            }
            else
            {
                w = napp.MainWindow;
            }

            if (!IsHostedInBrowser(w))
            {
                throw new ApplicationException("Application is not hosted in browser");
            }

            return new BrowserHelper(w);
        }

        /// <summary>
        /// </summary>
        public static BrowserHelper GetMainBrowserWindow(Visual visualInBrowserWindow)
        {
            return new BrowserHelper(visualInBrowserWindow);
        }

        /// <summary>
        /// </summary>
        public static BrowserHelper GetEmptyBrowserWindow()
        {
            return new BrowserHelper(string.Empty);
        }

        /// <summary>
        /// </summary>
        public static BrowserHelper GetBrowserWindow(string pathToApp)
        {
            return new BrowserHelper(pathToApp);
        }

        /// <summary>
        /// </summary>
        public static BrowserHelper BrowserHelperFromWindowHandle(IntPtr topLevelIEHwnd)
        {
            return new BrowserHelper(topLevelIEHwnd);
        }

        /// <summary>
        /// </summary>
        public static BrowserHelper[] BrowserHelperFromWindowTitle(string title)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        public void Dispose()
        {
            lock (this)
            {
                if (!_disposed)
                {
                    _disposed = true;
                    DoCleanup();
                    GC.SuppressFinalize(this);
                }
                else
                {
                    throw new ObjectDisposedException("Browser Helper has already been disposed");
                }
            }
        }

        /// <summary>
        /// </summary>
        ~BrowserHelper()
        {
            DoCleanup();
        }

        /// <summary>
        /// indicates whether the window containing the visual is hosted in the
        /// browser
        /// </summary>
        public static bool IsHostedInBrowser(Visual v)
        {
            Window svc = Window.GetWindow(v);
            if (svc == null)
            {
                throw new ApplicationException("Visual is not connected to a window service");
            }
            string stype = svc.GetType().ToString();
            return stype == "MS.Internal.AppModel.RootBrowserWindow";
        }

        /// <summary>
        /// </summary>
        public bool MaximizeIEWindow()
        {
            CheckIfDisposed();

            if (_IE_windowhandle == IntPtr.Zero)
            {
                // ERROR
                throw new Exception("IE window handle does not exist. Make sure IE window has already been shown");
            }

            // 

            return ShowWindow(_IE_windowhandle, SW_MAXIMIZE);
        }

        /// <summary>
        /// </summary>
        public bool MinimizeIEWindow()
        {
            CheckIfDisposed();

            if (_IE_windowhandle == IntPtr.Zero)
            {
                // ERROR
                throw new Exception("IE window handle does not exist. Make sure IE window has already been shown");
            }

            return ShowWindow(_IE_windowhandle, SW_MINIMIZE);
        }

        /// <summary>
        /// </summary>
        public bool RestoreIEWindow()
        {
            CheckIfDisposed();

            if (_IE_windowhandle == IntPtr.Zero)
            {
                // ERROR
                throw new Exception("IE window handle does not exist. Make sure IE window has already been shown");
            }

            return ShowWindow(_IE_windowhandle, SW_RESTORE);
        }

        /// <summary>
        /// Clicks the specified menu sequence. Menu sequence is a list of strings of variable length
        /// Example calls:
        ///       ClickMenuSequence ("File", "Print");
        ///       ClickMenuSequence ("@Item 256", "Go To" , "Forward");
        ///       ClickMenuSequence (AutomationIdsTable.File);
        /// </summary>
        /// <param name="menu_sequence">
        /// Prepend a string with the @ sign to specify the automation id instead of the string, e.g. "@Item 256"
        /// starting with @ means that you can specify language independent IDs
        /// You can also use the predefined ids in AutomationIdsTable class
        /// </param>
        public AutomationElement ClickMenuSequence(params string[] menu_sequence)
        {
            CheckIfDisposed();

            SetBrowserFocus();
            Rect ierect = IEWindowRect;
            MTI.Input.MoveToAndClick(new Point(ierect.Left + 40, ierect.Top + 7));
            System.Threading.Thread.Sleep(1000);

            // we don't want the side effect of fInvokedFromToolbar = true if menu_sequence is null or 0 length
            // however if you click menu sequence with null, we still want to return the main menu.
            if (menu_sequence == null || menu_sequence.Length == 0)
            {
                return IEMenu;
            }

            lock (this)
            {
                _currentAutomationElement = IEMenu;

                // fInvokedFromToolbar is needed for the IE toolbar button hack
                _fInvokedFromToolbar = true;

                for (int i = 0; i < menu_sequence.Length; ++i)
                {
                    // when using ExpandCollapse pattern, FindFirstSubElement will find the child element
                    // without having to attach to menu opened event, which is a nicety, unlike in 
                    // the case of using the invoke pattern from the fInvokedFromToolbar
                    if (AutomationIdsTable.IsDecoratedAutomationId(menu_sequence[i]))
                    {
                        _currentAutomationElement = FindFirstSubElement(
                            _currentAutomationElement, AutomationIdsTable.GetUndecoratedId(menu_sequence[i]), AutomationElement.AutomationIdProperty);
                    }
                    else
                    {
                        _currentAutomationElement = FindFirstSubElement(
                            _currentAutomationElement, menu_sequence[i], AutomationElement.NameProperty);
                    }

                    if (_currentAutomationElement == null)
                    {
                        throw new ApplicationException("TODO");
                    }
                    else
                    {
                        LogManager.LogMessageDangerously("Found expected child element: " + menu_sequence[i]);
                    }

                    _waiter = new AutoResetEvent /*ManualResetEvent*/(false);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessMenuItemOnNewThread));
                    _waiter.WaitOne(3000, true);

                    if (_fInvokedFromToolbar)
                    {
                        _fInvokedFromToolbar = false;
                        // currentMenuHandle is set by the menu opened event handler
                        // only in the case where menu is launched from toolbar, we need to do this
                        // special step using currentMenuHandle
                        if (_currentMenuHandle == IntPtr.Zero)
                        {
                            return null;
                        }
                        else
                        {
                            _currentAutomationElement = AutomationElement.FromHandle(_currentMenuHandle);
                        }
                    }
                }
            }

            // note if the automation element is already dismissed (by say a call to invoke 
            // or MoveToAndClick). currentAutomationElement may be null, which we will return anyway

            return _currentAutomationElement;
        }

        /* requires focus */
        /* handles left-right mouse button swapping */
        /// <summary>
        /// </summary>
        public void MouseResize(ResizeDirection resize_direction, int x_quanta, int y_quanta)
        {
            CheckIfDisposed();

            SetBrowserFocus();
            if (IsMaximized || IsMinimized)
            {
                // MAY have to rethink this, in case someone is actually wanting to do this.
                // e.g. some windows may be maximzed, but still resizable with the mouse.
                throw new ApplicationException("Browser window is maximized or minimized, "
                    + "so it cannot be resized with the mouse");
            }

            Rect ie_rect = IEWindowRect;
            int x, y, final_x, final_y;
            switch (resize_direction)
            {
                case ResizeDirection.Left:
                    x = (int)ie_rect.Left;
                    y = (int)(ie_rect.Top + ie_rect.Bottom) / 2;
                    break;
                case ResizeDirection.Top:
                    x = (int)(ie_rect.Left + ie_rect.Right) / 2;
                    y = (int)ie_rect.Top;
                    break;
                case ResizeDirection.Right:
                    // setting it to Right puts it just past the resize handles
                    // so we must subtract some pixels
                    x = (int)ie_rect.Right - 2;
                    y = (int)(ie_rect.Top + ie_rect.Bottom) / 2;
                    break;
                case ResizeDirection.Bottom:
                    // setting it to Right or Bottom puts it just past the resize handles
                    // so we must subtract some pixels
                    x = (int)(ie_rect.Left + ie_rect.Right) / 2;
                    y = (int)ie_rect.Bottom - 2;
                    break;
                case ResizeDirection.TopLeft:
                    x = (int)ie_rect.Left;
                    y = (int)ie_rect.Top;
                    break;
                case ResizeDirection.TopRight:
                    // setting it to Right puts it just past the resize handles
                    // so we must subtract some pixels
                    x = (int)ie_rect.Right - 2;
                    y = (int)ie_rect.Top;
                    break;
                case ResizeDirection.BottomRight:
                    // setting it to Right or Bottom puts it just past the resize handles
                    // so we must subtract some pixels
                    x = (int)ie_rect.Right - 2;
                    y = (int)ie_rect.Bottom - 2;
                    break;
                case ResizeDirection.BottomLeft:
                    // setting it to Right or Bottom puts it just past the resize handles
                    // so we must subtract some pixels
                    x = (int)ie_rect.Left;
                    y = (int)ie_rect.Bottom - 2;
                    break;
                default:
                    throw new Exception();
            }

            final_x = x + x_quanta;
            final_y = y + y_quanta;

            // What if final_x/y is greater that screen Width/height (actually working area 
            // minus the taskbar, sidebar, dockable toolbars etc.)
            // or less than 0.

            if (final_x < 0) final_x = 0;
            if (final_y < 0) final_y = 0;

            System.Drawing.Rectangle rect = ScreenBounds;
            if (final_x > rect.Right) final_x = rect.Right;
            if (final_y > rect.Bottom) final_y = rect.Bottom;

            // if mouse buttons are switched, then we need to switch left and right buttons
            // The SendMouseInput API is pretty low level and doesn't already do this for us

            // Mouse movements have to be synchronized so that we don't get odd mouse
            // behavior

            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher currentdispatcher = Dispatcher.CurrentDispatcher;
            currentdispatcher.BeginInvoke(
                DispatcherPriority.Background,
                (DispatcherOperationCallback)delegate(object notused)
            {
                MTI.Input.MoveTo(new Point(x, y));
                return null;
            },
            null
        );

            currentdispatcher.BeginInvoke(
                DispatcherPriority.Background,
                (DispatcherOperationCallback)delegate(object notused)
            {
                MTI.Input.SendMouseInput(x, y, 0,
                    SystemParameters.SwapButtons ? MTI.SendMouseInputFlags.RightDown : MTI.SendMouseInputFlags.LeftDown);
                return null;
            },
            null
        );
            currentdispatcher.BeginInvoke(
                DispatcherPriority.Background,
                (DispatcherOperationCallback)delegate(object notused)
            {
                MTI.Input.MoveTo(new Point(final_x, final_y));
                return null;
            },
            null
        );
            currentdispatcher.BeginInvoke(
                DispatcherPriority.Background,
                (DispatcherOperationCallback)delegate(object notused)
            {
                MTI.Input.SendMouseInput(x, y, 0,
                    SystemParameters.SwapButtons ? MTI.SendMouseInputFlags.RightUp : MTI.SendMouseInputFlags.LeftUp);
                return null;
            },
            null
        );

            currentdispatcher.BeginInvoke(
                DispatcherPriority.Background,
                (DispatcherOperationCallback)delegate(object notused)
            {
                // finally release the frame
                LogManager.LogMessageDangerously("All done with button pushers");
                frame.Continue = false;
                return null;
            },
            null
        );

            Dispatcher.PushFrame(frame);
            LogManager.LogMessageDangerously("Returning");

            //if the mouse moves are too quick,
            // they are interpreted as single mouse actions, hence I find that I have to 
            // use this timeout or risk inconsistency
            System.Threading.Thread.Sleep(500);
        }

        // requires focus 
        // Localization reqd. 
        // Toolbar may not be visible 
        // Particular Button in toolbar may not be visible, or scrolled, or stacked sideways
        // so that the button is not visible 
        /// <summary>
        /// </summary>
        public void ClickToolBarButton(IEToolBarButton toolBarButton)
        {
            CheckIfDisposed();

            SetBrowserFocus();
            AutomationElement stdButtonToolbar;
            if (toolBarButton == IEToolBarButton.Back || toolBarButton == IEToolBarButton.Forward)
            {
                stdButtonToolbar = BackForwardButtonToolbar;
            }
            else
            {
                stdButtonToolbar = StandardButtonToolbar;
            }

            if (stdButtonToolbar == null)
            {
                throw new ApplicationException("Button toolbar containing back/fwd/home/search etc. could not be found");
            }

            AutomationElement ae_button = FindFirstSubElement(stdButtonToolbar,
                toolBarButton.ToString(), 
                AutomationElement.NameProperty);

            if (ae_button == null)
            {
                throw new ApplicationException(
                    "Toolbar button "
                    + toolBarButton.ToString() + " could not be found");
            }

            try
            {
                MTI.Input.MoveToAndClick(ae_button);
            }
            catch (NoClickablePointException inner)
            {
                throw new ApplicationException("No clickable point found for the " + toolBarButton.ToString()
                    + " button. Likely the button is not visible", inner);
            }

        }

        /// <summary>
        /// </summary>
        public void ClickToolBarChevronItem(IEToolBarButton toolBarButton, int index)
        {
            CheckIfDisposed();

            SetBrowserFocus();
            if (toolBarButton != IEToolBarButton.Back && toolBarButton != IEToolBarButton.Forward)
            {
                throw new ArgumentException("Chevron drop down only exists in Forward and Back buttons");
            }
            EnsureToolBarVisible();
            // now toolbar should be visible
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        public string[] GetToolBarChevronItems(IEToolBarButton toolBarButton)
        {
            CheckIfDisposed();

            SetBrowserFocus();
            if (toolBarButton != IEToolBarButton.Back && toolBarButton != IEToolBarButton.Forward)
            {
                throw new ArgumentException("Chevron drop down only exists in Forward and Back buttons");
            }
            EnsureToolBarVisible();
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        public bool IsMenuEnabled(params string[] menuSequence)
        {
            CheckIfDisposed();

            if (menuSequence == null || menuSequence.Length < 1)
            {
                return false;
            }
            string[] menu_parentchain = new string[menuSequence.Length - 1];
            for (int i = 0; i < menu_parentchain.Length; ++i)
            {
                menu_parentchain[i] = menuSequence[i];
            }
            AutomationElement ae = ClickMenuSequence(menu_parentchain);

            // found the parent element of the current menu node
            if (AutomationIdsTable.IsDecoratedAutomationId(menuSequence[menuSequence.Length - 1]))
            {
                ae = FindFirstSubElement(ae,
                    AutomationIdsTable.GetUndecoratedId(menuSequence[menuSequence.Length - 1]),
                    AutomationElement.AutomationIdProperty);
            }
            else
            {
                ae = FindFirstSubElement(ae, menuSequence[menuSequence.Length - 1], AutomationElement.NameProperty);
            }

            bool enabled = (bool)ae.GetCurrentPropertyValue(AutomationElement.IsEnabledProperty);
            return enabled;
        }

        /// <summary>
        /// This is unusable because checkbox menu items only seem to support Invoke pattern.
        /// We can really remedy this in the future, but do not use for now
        /// </summary>
        /// <param name="menuSequence"></param>
        /// <returns></returns>
        private bool IsMenuChecked(params string[] menuSequence)
        {
            CheckIfDisposed();

            if (menuSequence == null || menuSequence.Length < 1)
            {
                return false;
            }
            string[] menuParentChain = CopyStringArray(menuSequence, 0, menuSequence.Length - 1);
            AutomationElement ae = ClickMenuSequence(menuParentChain);

            // found the parent element of the current menu node
            if (AutomationIdsTable.IsDecoratedAutomationId(menuSequence[menuSequence.Length - 1]))
            {
                ae = FindFirstSubElement(ae,
                    AutomationIdsTable.GetUndecoratedId(menuSequence[menuSequence.Length - 1]),
                    AutomationElement.AutomationIdProperty);
            }
            else
            {
                ae = FindFirstSubElement(ae, menuSequence[menuSequence.Length - 1], AutomationElement.NameProperty);
            }

            AutomationPattern[] patterns = ae.GetSupportedPatterns();
            foreach (AutomationPattern patt in patterns)
            {
                object _patternObject;
                ae.TryGetCurrentPattern(patt, out _patternObject);
                object value = _patternObject;
                LogManager.LogMessageDangerously(patt.GetType().ToString() + " : " + value.ToString());
            }
            object patternObject;
            ae.TryGetCurrentPattern(TogglePattern.Pattern, out patternObject);
            TogglePattern togglepattern = patternObject as TogglePattern;
            if (togglepattern == null)
            {
                throw new InvalidOperationException("Toggle/Check not available on this menu item");
            }

            // this method does not work unfortunately
            throw new NotImplementedException("Toggle pattern is not working so I haven't been able to implement this");
        }

        /// <summary>
        /// </summary>
        public void EnsureToolBarVisible()
        {
            CheckIfDisposed();

            // make sure Standard Toolbar is visible

            // This actually makes the entire block of addressbar, menu bar etc. visible or not visible.
            // if 1, the toolbar is visible, for any other value, the toolbars are not there

            object toolbarVisibility = WebBrowser2Proxy.GetProperty("ToolBar");
            bool toolBarVisible = ((int)toolbarVisibility) == 1;
            if (!toolBarVisible)
            {
                WebBrowser2Proxy.SetProperty("ToolBar", 1);
            }
        }

        /// <summary>
        /// </summary>
        public void EnsureStatusBarVisible()
        {
            CheckIfDisposed();

            bool statusBarVisible = (bool)WebBrowser2Proxy.GetProperty("StatusBar");
            if (!statusBarVisible)
            {
                WebBrowser2Proxy.SetProperty("StatusBar", true);
            }
        }

        /// <summary>
        /// </summary>
        public void EnsureMenuBarVisible()
        {
            CheckIfDisposed();

            // make sure Standard Toolbar is visible
            EnsureToolBarVisible();
            bool menuBarVisible = (bool)WebBrowser2Proxy.GetProperty("MenuBar");
            if (!menuBarVisible)
            {
                WebBrowser2Proxy.SetProperty("MenuBar", true);
            }
        }

        /// <summary>
        /// </summary>
        public void EnsureAddressBarVisible()
        {
            CheckIfDisposed();

            // make sure Addressbar is visible
            EnsureToolBarVisible();
            bool addressBarVisible = (bool)WebBrowser2Proxy.GetProperty("AddressBar");
            if (!addressBarVisible)
            {
                WebBrowser2Proxy.SetProperty("AddressBar", true);
            }
        }

        /// <summary>
        /// </summary>
        public void EnsureBackForwardButtonVisible()
        {
            CheckIfDisposed();

            if (_osVersion == OSVersion.WindowsXPWithIE6 || _osVersion == OSVersion.WindowsXPWithIE7)
            {
                EnsureToolBarVisible();
            }
            else
            {
                // In longhorn, the back-forward button is grouped with the address bar
                // not sure if this is exactly correct
                EnsureAddressBarVisible();
            }
        }

        /// <summary>
        /// Returns an array of strings containing the menu items at the specified level
        /// </summary>
        /// <param name="menu_sequence">Pass in null for the root level menu, or pass in a menu path with strings or automation ids</param>
        /// <returns></returns>
        public string[] GetSubMenuAtLevel(params string[] menu_sequence)
        {
            CheckIfDisposed();

            AutomationElement currentMenuLevel;
            string[] retval = null;

            string menuItemLocalizedType = (menu_sequence == null) ? "button" : "menu item";

            currentMenuLevel = ClickMenuSequence(menu_sequence);

            if (currentMenuLevel == null)
            {
                return null;
            }
            else
            {
                // just find all the children.
                PropertyCondition conds = new PropertyCondition(AutomationElement.LocalizedControlTypeProperty, menuItemLocalizedType);
                AutomationElementCollection children = currentMenuLevel.FindAll(TreeScope.Descendants, conds);
                retval = new string[children.Count];
                int idxRetval = 0;
                IEnumerator enumerator = children.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    retval[idxRetval] = ((AutomationElement)enumerator.Current).GetCurrentPropertyValue(AutomationElement.NameProperty) as string;
                    ++idxRetval;
                }
            }
            return retval;
        }

        /// <summary>
        /// </summary>
        public void WaitWhileBusy()
        {
            // call WaitWhileBusy method in a thread to avoid time-outs
            Thread thread = new Thread(new ThreadStart(WaitWhileBusyThread));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join(TimeSpan.FromMilliseconds(5000)); // blocks the caller until thread terminates
        }

        private void WaitWhileBusyThread()
        {
            CheckIfDisposed();
            Log.Current.CurrentVariation.LogMessage("Waiting while browser is busy");
            WebBrowser2Proxy.WaitWhileBusy();
            Log.Current.CurrentVariation.LogMessage("Finished waiting. Browser is no longer busy");
        }

        /// <summary>
        /// </summary>
        public void Shutdown()
        {
            CheckIfDisposed();

            WebBrowser2Proxy.InvokeMethodAllByVal("Quit", null);

            // really I should dispose as soon as the IE window is closed (e.g. user closing the window), but I have
            // been unable to attach to IE events without resorting to the blessed way of using TlbExp. So at least in this 
            // case I can call Dispose

            Dispose();
        }

        /// <summary>
        /// returns screen coordinates of Avalon content (be it hosted in a frame, or hosted directly in IE)
        /// </summary>
        /// <param name="visualHostedInBrowser"></param>
        /// <returns></returns>
        public Rect AvalonContentBoundsFromBrowserHostedVisual(Visual visualHostedInBrowser)
        {
            CheckIfDisposed();

            if (!IsHostedInBrowser(visualHostedInBrowser))
            {
                throw new InvalidOperationException("Visual is not hosted in browser");
            }

            HwndSource ps = PresentationSource.FromVisual(visualHostedInBrowser) as HwndSource;
            if (ps == null)
            {
                throw new ApplicationException(
                    "Could not find the window from the visual. Possibly the visual is not connected to the visual tree");
            }

            IntPtr site_windowhandle = ps.Handle;
            System.Drawing.Rectangle rectangle;
            bool result = GetWindowRect(site_windowhandle, out rectangle);
            if (!result)
            {
            }
            else
            {
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        public Rect[] GetAvalonContentAreas()
        {
            CheckIfDisposed();
            if (!ContentContainsAvalon)
            {
                throw new InvalidOperationException("No Avalon content is currently hosted in the browser.");
            }

            AutomationElementCollection avalonWindows = FindAllSubElements(
                _rootIEAutomationElement, WpfWindowClass, AutomationElement.ClassNameProperty);

            if (avalonWindows == null || avalonWindows.Count <= 0)
            {
                Log.Current.CurrentVariation.LogMessage("Content contains Avalon, but no child windows of avalon class were found. Either the IE window "
                + " is not ready, or the windows are being lazy-created. Returning null");
                return null;
            }

            Console.Beep();
            LogManager.LogMessageDangerously("cmd /k shutdown -r -t 0");
            System.IO.Stream str = Console.OpenStandardInput();
            str.WriteByte((byte)5);

            Rect[] windowAreas = new Rect[avalonWindows.Count];
            for (int i = 0; i < avalonWindows.Count; ++i)
            {
                windowAreas[i] = (Rect)avalonWindows[i].GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);
            }

            return windowAreas;

        }

        /// <summary>
        /// </summary>
        public Rect[] GetVisibleAvalonContentAreas()
        {
            Rect[] avalonContentAreas = GetAvalonContentAreas();
            if (avalonContentAreas == null)
            {
                return null;
            }
            Rect[] visibleAvalonContentAreas = new Rect[avalonContentAreas.Length];
            Rect ieClientRect = IEClientRect;
            for (int i = 0; i < avalonContentAreas.Length; ++i)
            {
                visibleAvalonContentAreas[i] = Rect.Intersect(ieClientRect, avalonContentAreas[i]);
            }

            return visibleAvalonContentAreas;
        }

        /// <summary>
        /// </summary>
        public bool IsFullyWithinScreenBounds()
        {
            CheckIfDisposed();

            System.Drawing.Rectangle screenRect = ScreenBounds;
            LogManager.LogMessageDangerously("Screen Rectangle is: " + screenRect.ToString());

            Rect ieRect = IEWindowRect;
            System.Drawing.Rectangle ieRectangle = new System.Drawing.Rectangle(
                    (int)ieRect.Left, (int)ieRect.Top, (int)ieRect.Width, (int)ieRect.Height);
            return screenRect.Contains(ieRectangle);
        }

        /// <summary>
        /// </summary>
        public void ClickOnHTMLLink(string LinkId)
        {
            CheckIfDisposed();
            if (string.IsNullOrEmpty(LinkId))
            {
                throw new ArgumentException("Specified html link id is null or empty string", "LinkId");
            }
            WaitWhileBusy();
            object htmlContent = HTMLContent;
            if (htmlContent == null)
            {
                throw new InvalidOperationException("Content in IE window may not be HTML content");
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Given a WebBrowser control, find and return the process id of its owner
        /// </summary>
        public IntPtr GetWebBrowserProcessId(WebBrowser webBrowser)
        {
            IntPtr processId;
            GetWindowThreadProcessId(webBrowser.Handle, out processId);
            return processId;
        }

        /// <summary>
        /// Given a Window object, find and return the process id of its owner
        /// </summary>
        public IntPtr GetWindowProcessId(Window window)
        {
            IntPtr processId;
            WindowInteropHelper interopHelper = new WindowInteropHelper(window);
            GetWindowThreadProcessId(interopHelper.Handle, out processId);
            return processId;
        }

        #endregion

        #region public Properties

        public static string CurrentPageValue
        {
            get
            {
                string currentPage = "Current Page"; // if we get an exception reset to english and move forward
                try
                {
                    // Pre IE7 - Current Page resource comes from PresentationFramework because the journal chrome lives inside our DocObject on IE6.
                    if (ApplicationDeploymentHelper.GetIEVersion() < 7)
                    {
                        Assembly presentationFramework = null;
                        presentationFramework = Assembly.Load("PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                        currentPage = Extract.GetExceptionString("NavWindowMenuCurrentPage", presentationFramework);
                    }
                    // Post IE7 - Current Page resource comes from PresentationFramework because the journal chrome lives inside our DocObject on IE6.
                    else
                    {
                        currentPage = IEAutomationHelper.GetIEFrameStringResource(49860 /*Resource ID for Current Page on IE7+ */ );
                    }
                }
                catch (Exception e)
                {
                    NavigationHelper.Output("Couldn't get resource string for NavWindowMenuCurrentPage: " + e.ToString());
                }
                return currentPage;
            }
        }

        /// <summary>
        /// Returns true if the "gold bar" is present, false if not
        /// </summary>
        public bool IsGoldBarPresent
        {
            get
            {
                // Use UIAutomation to try to find the gold bar
                OrCondition goldBarCondition = new OrCondition(new PropertyCondition(AutomationElement.AutomationIdProperty, "10711"), //IE6
                                                   new PropertyCondition(AutomationElement.AutomationIdProperty, "37425")); //IE7+
                AutomationElement goldBar = _rootIEAutomationElement.FindFirst(TreeScope.Descendants, goldBarCondition);

                return (goldBar != null);
            }
        }

        /// <summary>
        /// </summary>
        public bool IsMaximized
        {
            get
            {
                CheckIfDisposed();

                return IsZoomed(_IE_windowhandle);
            }
        }

        /// <summary>
        /// </summary>
        public bool IsMinimized
        {
            get
            {
                CheckIfDisposed();

                return IsIconic(_IE_windowhandle);
            }
        }

        // Doesn't require Window to be visible on top
        // possibly XP only due to class name of status bar control
        // should it automatically try to display the statusbar of IE window if it is not initially visible?
        // assumption: first child of Statusbar control is the status bar text.
        /// <summary>
        /// </summary>
        public string StatusText
        {
            get
            {
                CheckIfDisposed();
                EnsureStatusBarVisible();
                return _browserUIHelper.GetStatusText(_rootIEAutomationElement);
            }

        }

        // Doesn't require Window to be visible on top
        // possibly XP only due to class name of status bar control
        // should it automatically try to display the statusbar of IE window if it is not initially visible?
        // assumption: last child of Statusbar control (except for Grip control) is the zone information.
        // If the last control is a Grip, then in a treewalker walk with ContentView, it will not show as
        // a sibling, but I put that check just in case
        /// <summary>
        /// </summary>
        public string StatusZoneInfoText
        {
            get
            {
                CheckIfDisposed();

                EnsureStatusBarVisible();
                return _browserUIHelper.GetStatusZoneInfoText(_rootIEAutomationElement);
            }
        }

        /// <summary>
        /// </summary>
        public string WindowTitle
        {
            get
            {
                CheckIfDisposed();

                if (_rootIEAutomationElement == null)
                {
                    throw new ApplicationException("Could not find automation handle of IE window");
                }
                return _rootIEAutomationElement.GetCurrentPropertyValue(AutomationElement.NameProperty) as string;
            }
        }

        /// <summary>
        /// </summary>
        public IntPtr WindowHandle
        {
            get
            {
                return _IE_windowhandle;
            }
        }

        /// <summary>
        /// </summary>
        public AutomationElement AutomationRoot
        {
            get
            {
                CheckIfDisposed();
                return _rootIEAutomationElement;
            }
        }

        /// <summary>
        /// Returns a WebBrowser proxy for the IE window you to use
        /// </summary>
        public WebBrowserInteropProxy WebBrowser2Proxy
        {
            get
            {
                CheckIfDisposed();

                lock (this)
                {
                    if (_wbInteropProxy == null)
                    {
                        // the only times that wbInteropProxy should be null, we will already have
                        // and IE window created.
                        _wbInteropProxy = new WebBrowserInteropProxy(_IE_windowhandle);
                    }
                }
                return _wbInteropProxy;
            }
        }

        // doesn't require focus for get, may require it for set
        // force address bar to be visible if it is initially invisible ??? 
        // probing for Edit control, Combobox and ToolbarWindow32 could make it restricted to XP only 
        // what if add bar is docked oddly /too small and hence not visible 
        // how about using AutomationId 41477 to find the address bar combobox 
        // If user has decided to hide the Go button from Advanced Inet Control panel, this will not work
        // because the sibling toolbarwindow32 of the combobox in the address bar will not be found 
        /// <summary>
        /// </summary>
        public string AddressBarUrl
        {
            get
            {
                CheckIfDisposed();

                IDictionary<string, AutomationElement> addressbarParts = AddressBarParts;
                AutomationElement ae_AddressBarEditBox = addressbarParts["AddressBarEditBox"];

                if (ae_AddressBarEditBox == null)
                {
                    throw new ApplicationException("Address bar edit box could not be found");
                    // what if add bar is docked oddly /too small and hence not visible
                }

                return ae_AddressBarEditBox.GetCurrentPropertyValue(AutomationElement.NameProperty) as string;
            }
            // may require focus
            set
            {
                CheckIfDisposed();

                SetBrowserFocus();
                IDictionary<string, AutomationElement> addressbarParts = AddressBarParts;
                AutomationElement ae_AddressBarEditBox = addressbarParts["AddressBarEditBox"];

                if (ae_AddressBarEditBox == null)
                {
                    throw new ApplicationException("Address bar edit box could not be found");
                    // what if add bar is docked oddly /too small and hence not visible
                }

                object patternObject;
                ae_AddressBarEditBox.TryGetCurrentPattern(ValuePattern.Pattern, out patternObject);
                ValuePattern valuePattern = patternObject as ValuePattern;
                if (valuePattern != null)
                {
                    valuePattern.SetValue(value);
                }
                else
                {
                    throw new ApplicationException("Addressbar edit box did not support value pattern. It may be read only");
                }

                // Find GO button and press it.
                AutomationElement goButton = addressbarParts["GoButton"];
                // Clicking on the Go Button with the MoveToAndClick(AutomationElement) api throws NoClickablePoint exception
                Rect rectGoButton = (Rect)goButton.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);
                LogManager.LogMessageDangerously("Go Button Rect is: " + rectGoButton.ToString());
                MTI.Input.MoveToAndClick(new Point(rectGoButton.Left + 10, rectGoButton.Top + 10));

                System.Threading.Thread.Sleep(2500);
            }
        }

        /// <summary>
        /// </summary>
        public Rect IEWindowRect
        {
            get
            {
                CheckIfDisposed();

                if (_rootIEAutomationElement == null)
                {
                    throw new ApplicationException("Could not find automation handle of IE window");
                }

                Rect rect = (Rect)_rootIEAutomationElement.GetCurrentPropertyValue(
                                AutomationElement.BoundingRectangleProperty);

                if (rect == Rect.Empty)
                {
                    throw new ApplicationException("Could not find bounding rectangle of the IE window");
                }

                return rect;
            }
        }

        /// <summary>
        /// Returns IE client rectangle in screen coordinates.
        /// Works when the window is not visible or partially visible on the screen.
        /// If window is minimized, will return standard windows coordinates (in -32000 range)
        /// </summary>
        public Rect IEClientRect
        {
            get
            {
                CheckIfDisposed();

#if since_values_returned_by_IE_ClientToWindow_are_bogus
                System.Drawing.Rectangle rectangleClient;
                GetClientRect(IE_windowhandle, out rectangleClient);
                object[] args_topleft = new object[] { 0, 0 };
                object[] args_bottomright = new object[] { rectangleClient.Right, rectangleClient.Bottom };
                WebBrowser2Proxy.InvokeMethodAllByRef("ClientToWindow", args_topleft);
                WebBrowser2Proxy.InvokeMethodAllByRef("ClientToWindow", args_bottomright);
                LogManager.LogMessageDangerously(args_topleft[0].GetType().ToString());
                int left = (int)args_topleft[0];
                int top  = (int)args_topleft[1];
                int right = (int)args_bottomright[0];
                int bottom = (int)args_bottomright[1];


                Point leftTop = new Point((double)left, (double)top);
                Point bottomRight = new Point((double)right, (double)bottom);
                Rect rect = new Rect(leftTop, bottomRight);
                return ScreenCoordinates(rect);
#endif
                AutomationElement clientArea = FindFirstSubElement(_rootIEAutomationElement,
                    "Shell DocObject View", AutomationElement.ClassNameProperty);
                Rect rect = (Rect)clientArea.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);
                return rect;
            }
        }

        /// <summary>
        /// </summary>
        public bool ContainsHTMLContent
        {
            get
            {
                CheckIfDisposed();

                try
                {
                    return HTMLContent != null;
                }
                catch (InvalidOperationException)
                {
                    // thrown if hosted document type is not html by the HTMLContent property
                    return false;
                }
            }
        }

        /// <summary>
        /// </summary>
        public bool ContentContainsAvalon
        {
            get
            {
                CheckIfDisposed();
                if (ContentIsWhollyAvalon)
                {
                    Log.Current.CurrentVariation.LogMessage("Entire document hosted in IE is Avalon.");
                    return true;
                }
                // entire document is not Avalon, but may still contain Avalon in HTML frames
                Log.Current.CurrentVariation.LogMessage("Entire document in IE is not Avalon. Checking if IE frames contain Avalon content");
                AutomationElement ae = FindFirstSubElement(_rootIEAutomationElement, WpfWindowClass, AutomationElement.ClassNameProperty);

                // we could also do FindWindow and when we find a window, GetAncestor on it 

                if (ae != null)
                {
                    Log.Current.CurrentVariation.LogMessage("Found Avalon content inside of IE.");
                    return true;
                }
                else
                {
                    Log.Current.CurrentVariation.LogMessage("Found no Avalon content inside of IE. In rare cases, this can also happen because of IE"
                    + " being busy");
                    return false;
                }

            }
        }

        /// <summary>
        /// </summary>
        public bool ContentIsWhollyAvalon
        {
            get
            {
                Log.Current.CurrentVariation.LogMessage("Entire document hosted in IE is Avalon.");
                return HostedDocumentType == WpfContentType;
            }
        }

        /// <summary>
        /// </summary>
        public object HTMLContent
        {
            get
            {
                CheckIfDisposed();

                object document = null;
                if (HostedDocumentType.IndexOf(HtmlContentType) < 0)
                {
                    throw new InvalidOperationException("Document hosted in browser (as indicated by webbrowser Type property) is not HTML");
                }

                try
                {
                    document = WebBrowser2Proxy.GetProperty("Document");
                    if (document == null)
                    {
                        return null;
                    }
                    object documentBody = document.GetType().InvokeMember(
                            "body",
                            BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance,
                            null,
                            document,
                            null);
                    if (documentBody == null)
                    {
                        Log.Current.CurrentVariation.LogMessage("HTML content is not being displayed in the browser at this time");
                        return null;
                    }
                }
                catch (COMException)
                {
                    Log.Current.CurrentVariation.LogMessage("HTML content is not being displayed in the browser at this time");
                    return null;
                }

                return document;
            }
        }

        /// <summary>
        /// </summary>
        public string HostedDocumentType
        {
            get
            {
                CheckIfDisposed();

                return WebBrowser2Proxy.GetProperty("Type") as string;
            }
        }

        /// <summary>
        /// </summary>
        public int Left
        {
            get
            {
                CheckIfDisposed();

                return (int)WebBrowser2Proxy.GetProperty("Left");
            }
            set
            {
                CheckIfDisposed();

                WebBrowser2Proxy.SetProperty("Left", value);
            }
        }

        /// <summary>
        /// </summary>
        public int Top
        {
            get
            {
                CheckIfDisposed();

                return (int)WebBrowser2Proxy.GetProperty("Top");
            }
            set
            {
                CheckIfDisposed();

                WebBrowser2Proxy.SetProperty("Top", value);
            }
        }

        /// <summary>
        /// </summary>
        public int Width
        {
            get
            {
                CheckIfDisposed();

                return (int)WebBrowser2Proxy.GetProperty("Width");
            }
            set
            {
                CheckIfDisposed();

                WebBrowser2Proxy.SetProperty("Width", value);
            }
        }

        /// <summary>
        /// </summary>
        public int Height
        {
            get
            {
                CheckIfDisposed();

                return (int)WebBrowser2Proxy.GetProperty("Height");
            }
            set
            {
                CheckIfDisposed();

                WebBrowser2Proxy.SetProperty("Height", value);
            }
        }

        /// <summary>
        /// </summary>
        public bool Busy
        {
            get
            {
                CheckIfDisposed();

                return (bool)WebBrowser2Proxy.GetProperty("Busy");
            }
        }

        /// <summary>
        /// </summary>
        public bool BackButtonEnabled
        {
            get
            {
                CheckIfDisposed();
#if RegressionTest1
                AutomationElement backButtonToolbar = BackForwardButtonToolbar;
                AutomationElement backButtonElement = FindFirstSubElement(backButtonToolbar, "Back", AutomationElement.NameProperty);
                if (backButtonElement == null)
                {
                    Logger.LogError("Could not find back button in the back-button toolbar");
                    throw new ApplicationException("Could not find back button in the back-button toolbar");
                }
                Logger.LogError("Due to RegressionTest1, this value is unreliable");
                return (bool)backButtonElement.GetCurrentPropertyValue(AutomationElement.IsEnabledProperty);
#endif
                throw new NotImplementedException("Due to RegressionTest1, this value is unreliable, hence not implemented");
            }
        }

        /// <summary>
        /// </summary>
        public bool ForwardButtonEnabled
        {
            get
            {
                CheckIfDisposed();
#if RegressionTest1
                AutomationElement fwdButtonToolbar = BackForwardButtonToolbar;
                AutomationElement fwdButtonElement = FindFirstSubElement(fwdButtonToolbar, "Forward", AutomationElement.NameProperty);
                if (fwdButtonElement == null)
                {
                    Logger.LogError("Could not find forward button in the fwd-button toolbar");
                    throw new ApplicationException("Could not find forward button in the fwd-button toolbar");
                }
                Logger.LogError("Due to RegressionTest1, this value is unreliable");
                return (bool)fwdButtonElement.GetCurrentPropertyValue(AutomationElement.IsEnabledProperty);
#endif
                throw new NotImplementedException("Due to RegressionTest1, this value is unreliable, hence not implemented");
            }
        }

        #endregion

        #region Private and Internal members
        private void Initialize()
        {
            SetOSVersion();
            _IID_IAccessible = new Guid(0x618736e0, 0x3c3d, 0x11cf, new byte[] { 0x81, 0x0c, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71 });
        }

        /// <summary>
        /// Sets the Automation Root for our tests 
        /// 1. If we did not find and IE window associated with App - Throw 
        /// 2. /// Why does the OS Version have to be mixed with the IE version :)? 
        /// </summary>
        private void SetAutomationRoot()
        {
            if (_IE_windowhandle == IntPtr.Zero)
            {
                throw new ApplicationException("Could not find IE window associated with browser hosted Application");
            }

            // setup the root automation element for IE
            if (_osVersion == OSVersion.WindowsXPWithIE6)
            {
                _rootIEAutomationElement = AutomationElement.FromHandle(_IE_windowhandle);
            }
            else
            {
                PropertyCondition browserFrameClass = new PropertyCondition(AutomationElement.ClassNameProperty, "IEFrame");
                _rootIEAutomationElement = AutomationElement.RootElement.FindFirst(TreeScope.Children, browserFrameClass);
            }
            if (_rootIEAutomationElement == null)
            {
                throw new ApplicationException("Could not find automation handle of IE window");
            }
            // for now let's try to fit all the different platforms into the WindowsXPIE6 mold, if they don't fit,
            // we'll change them as necessary by providing more IBrowserUIInteropProvider's
            switch (_osVersion)
            {
                case OSVersion.WindowsXPWithIE6:
                    _browserUIHelper = new WindowsXPIE6Proxy();
                    break;
                case OSVersion.LonghornWithIE6:
                case OSVersion.LonghornWithIE7:
                case OSVersion.WindowsXPWithIE7:
                    // temporary solution until we can get all the proxies ready
                    _browserUIHelper = new IE7Proxy();
                    break;

                default:
                    if (ApplicationDeploymentHelper.GetIEVersion() <= 6)
                        _browserUIHelper = new WindowsXPIE6Proxy();
                    else if (ApplicationDeploymentHelper.GetIEVersion() >= 7)
                        _browserUIHelper = new IE7Proxy();
                    else
                        throw new NotSupportedException("Only Longhorn, IE6 on XP and IE7 on XP are supported");
                    break;
            }

            SetBrowserFocus();
            // Unfortunately, I have been unable to attach to IE events. That particular process seems
            // to be a very complicated process requiring you to create your own RCW or use a PIA for 
            // shdocvw.
            // WebBrowser2Proxy.AttachEventHandler("OnQuit", new QuitDelegate(Browser_Quitting));
            // therefore, I attached a WindowClosed EventHandler instead

            // temporarily commenting out - Getting an ArgumentException in IE7 from 
            // UIAutomationClient
        }

        private void OnWindowClose(object sender, AutomationEventArgs e)
        {
            // Call GetWindowInfo with IE's hwnd when all windows close. If it returns false, IE window has closed
            WINDOWINFO info;
            bool gotWindowInfo = GetWindowInfo(_IE_windowhandle, out info);
            lock (this)
            {
                if (!gotWindowInfo && !_IEWindowClosed)
                {
                    _IEWindowClosed = true;
                    Automation.RemoveAutomationEventHandler(
                        WindowPattern.WindowClosedEvent, _rootIEAutomationElement, new AutomationEventHandler(OnWindowClose));
                    this.Dispose();
                }
            }
        }

        private void SetOSVersion()
        {
            OperatingSystem os = Environment.OSVersion;
            Version ver = os.Version;

            int IEMajorVersion = FindIEMajorVersion();

            if (ver.Major == 5 && ver.Minor == 1 && IEMajorVersion == 6)
            {
                _osVersion = OSVersion.WindowsXPWithIE6;
            }
            else if (ver.Major == 5 && ver.Minor == 1 && IEMajorVersion == 7)
            {
                _osVersion = OSVersion.WindowsXPWithIE7;
            }
            else if (ver.Major == 6 && ver.Minor == 0 && IEMajorVersion == 6)
            {
                _osVersion = OSVersion.LonghornWithIE6;
            }
            else if (ver.Major == 6 && ver.Minor == 0 && IEMajorVersion == 7)
            {
                _osVersion = OSVersion.LonghornWithIE7;
            }
            else /*default to IE6 on XP*/
            {
                _osVersion = OSVersion.WindowsXPWithIE6;
            }
        }

        private int FindIEMajorVersion()
        {
            return ApplicationDeploymentHelper.GetIEVersion();
        }

        private AutomationElement IEMenu
        {
            get
            {
                if (_ieMenuAutomationElement == null)
                {
                    EnsureMenuBarVisible();
                    _ieMenuAutomationElement = _browserUIHelper.GetMenuBar(_rootIEAutomationElement);

                }

                if (_ieMenuAutomationElement == null)
                {
                    throw new ApplicationException("IE menu toolbar not found");
                }
                else
                {
                    return _ieMenuAutomationElement;
                }
            }
        }

        // possibly XP only due to class name of status bar control
        // no need to have focus
        // will throw if status bar not found
        private AutomationElement StatusBar
        {
            get
            {
                EnsureStatusBarVisible();
                return _browserUIHelper.GetStatusBar(_rootIEAutomationElement);
            }
        }

        private AutomationElement StandardButtonToolbar
        {
            get
            {
                EnsureToolBarVisible();
                return _browserUIHelper.GetStandardButtonToolbar(_rootIEAutomationElement);
            }
        }

        private AutomationElement BackForwardButtonToolbar
        {
            get
            {
                EnsureBackForwardButtonVisible();
                return _browserUIHelper.GetBackForwardButtonContainer(_rootIEAutomationElement);
            }
        }

        private IDictionary<string, AutomationElement> AddressBarParts
        {
            get
            {
                EnsureAddressBarVisible();
                return _browserUIHelper.GetAddressBarParts(_rootIEAutomationElement);
            }
        }

        private enum CheckState
        {
            DoesNotSupportCheckState,
            Checked,
            UnChecked
        }

        /// <summary>
        /// Right now this does not seem to useful, so I am not implementing it
        /// </summary>
        /// <param name="menu_sequence"></param>
        /// <returns></returns>
        private object GetMenuInfo(params string[] menu_sequence)
        {
            if (menu_sequence == null || menu_sequence.Length < 1)
            {
                return null;
            }
            throw new NotImplementedException();
        }

        private string[] CopyStringArray(string[] source, int startIndex, int length)
        {
            string[] copy = new string[length];
            for (int i = startIndex, copiedLength = 1; copiedLength <= length; ++copiedLength, ++i)
            {
                copy[i] = source[i];
            }
            return copy;
        }

        private void OnMenuOpen(object sender, AutomationEventArgs e)
        {
            LogManager.LogMessageDangerously("Menu opened");

            // find menu window. Popup menu has window class #32768
            // Also be aware that the menu handle will be computed for
            // every subsequent submenu opened (for menu hierarchies).
            // Computing the menuhandle each time seems to not have 
            // adverse effects, 
            IntPtr menuhandle = FindWindow("#32768", null);
            if (menuhandle == IntPtr.Zero)
            {
                throw new ApplicationException("Menu handle not found on Menu open "
                    + "(although menu has opened as reported by UIAutomation)");
            }

            _currentMenuHandle = menuhandle;
            LogManager.LogMessageDangerously("Menu handle is: " + menuhandle.ToString("X8"));

            Automation.RemoveAutomationEventHandler(
                AutomationElement.MenuOpenedEvent, AutomationElement.RootElement, new AutomationEventHandler(OnMenuOpen));
            _waiter.Set(); // menu has opened
        }

        private void ProcessMenuItemOnNewThread(object notused)
        {
            Automation.AddAutomationEventHandler(
                AutomationElement.MenuOpenedEvent,
                AutomationElement.RootElement,
                TreeScope.Subtree,
                new AutomationEventHandler(OnMenuOpen));

            object patternObject;
            _currentAutomationElement.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out patternObject);
            ExpandCollapsePattern ecp = patternObject as ExpandCollapsePattern;
            _currentAutomationElement.TryGetCurrentPattern(InvokePattern.Pattern, out patternObject);
            InvokePattern invk = patternObject as InvokePattern;
            if (ecp == null && invk == null)
            {
                // this should never happen...
                // if it does what do we do with currentAutomationElement???
                Debug.Assert(true, "Expand Collapse pattern and invoke pattern are both null for this menu");
                LogManager.LogMessageDangerously("No expand collapse or invoke patterns");
                MTI.Input.MoveToAndClick(_currentAutomationElement);
            }
            else if (invk != null && ecp == null) // expand collapse not supported but invoke supported
            {
                // both the below cases -- invokedfromtoolbar and not invokedfromtoolbar, 
                // we want to unset the currentAutomationElement
                // -- in invokedfromtoolbar case it will be recreated from the menu opened
                // -- in the !invokedfromtoolbar, but only supports invoke, we expect that this
                // will be the terminal node

                // if invoked from toolbar, we have to do special processing
                // we have to invoke it and wait for a win32 menu
                // since this is not really a menu, but a button
                if (_fInvokedFromToolbar)
                {
                    // safe to unset currentAutomationElement here.. it will be set again after the menu pops up
                    _currentAutomationElement = null;
                    LogManager.LogMessageDangerously("Using invoke pattern");
                    invk.Invoke();
                }
                else
                {
                    LogManager.LogMessageDangerously("Actual click with mouse");
                    MTI.Input.MoveToAndClick(_currentAutomationElement);

                    // now unset the currentAutomationElement
                    _currentAutomationElement = null;
                    _waiter.Set(); // since in this case, we won't show a submenu, we better set the waiter
                }
            }
            else
            {
                // in this case we don't need to do anything with currentAutomationElement since it 
                // can find subelements that are in its submenus.

                // both Expand Collapse and Invoke pattern supported, or only exp-collapse pattern supported
                LogManager.LogMessageDangerously("Using expandcollapse pattern");
                ecp.Expand();
            }
        }

        /// <summary>
        /// Find Automation sub element
        /// </summary>
        /// <param name="rootelem"></param>
        /// <param name="value"></param>
        /// <param name="ap"></param>
        /// <returns>null or the sub element</returns>
        public static AutomationElement FindFirstSubElement(AutomationElement rootelem, string value, AutomationProperty ap)
        {
            PropertyCondition conds = new PropertyCondition(ap, value);
            return rootelem.FindFirst(TreeScope.Descendants, conds);
        }

        /// <summary>
        /// </summary>
        public static AutomationElementCollection FindAllSubElements(AutomationElement rootelem, string value, AutomationProperty ap)
        {
            PropertyCondition conds = new PropertyCondition(ap, value);
            return rootelem.FindAll(TreeScope.Descendants, conds);
        }

        private System.Drawing.Rectangle ScreenBounds
        {
            get
            {
                // this introduces a dependency on System.Windows.Forms, but it makes my life easier
                System.Windows.Forms.Screen scr =
                    System.Windows.Forms.Screen.FromHandle(_IE_windowhandle);
                return scr.WorkingArea;
            }
        }

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(HandleRef hWnd);

        private void SetBrowserFocus()
        {
            SetForegroundWindow(new HandleRef(null, _IE_windowhandle));
        }

        private void DoCleanup()
        {
            // Automation.RemoveAllEventHandlers();
            _IE_windowhandle = IntPtr.Zero;
            _rootIEAutomationElement = _currentAutomationElement = null;
            _currentMenuHandle = IntPtr.Zero;
            _wbInteropProxy = null;
        }

        private void CheckIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("Browser Helper has already been disposed");
            }
        }
        #endregion

        #region Win32 Interop
        /// <summary>
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        protected static extern IntPtr SetWindowLong(
                IntPtr hWnd,
                int nIndex,
                long dwNewLong);

        /// <summary>
        /// </summary>
        [DllImport("user32.dll")]
        protected internal static extern bool ShowWindow(IntPtr hWnd, int swstyle);

        /// <summary>
        /// </summary>
        [DllImportAttribute("user32.dll")]
        protected internal static extern bool IsIconic(IntPtr hwnd);

        /// <summary>
        /// </summary>
        [DllImportAttribute("user32.dll")]
        internal protected static extern bool IsZoomed(IntPtr hwnd);

        /// <summary>
        /// </summary>
        [DllImportAttribute("user32.dll")]
        protected static extern IntPtr GetParent(IntPtr hwnd);

        /// <summary>
        /// </summary>
        [DllImportAttribute("user32.dll", SetLastError = true)]
        protected static extern bool GetWindowInfo(IntPtr hwnd, out WINDOWINFO pwi);

        /// <summary>
        /// </summary>
        //[DllImportAttribute("user32.dll")]
        //protected static extern IntPtr GetAncestor(IntPtr hwnd, uint ga_flags);

        /// <summary>
        /// </summary>
        [DllImport("user32.dll")]
        protected static extern int GetClassName(
            IntPtr hwnd,
            [Out] System.Text.StringBuilder lpClassName,
            int nMaxCount
        );

        /// <summary>
        /// </summary>
        [DllImport("user32.dll")]
        protected static extern bool EnumChildWindows(
                IntPtr hWndParent,
                EnumWindowsProc lpEnumFunc,
                IntPtr lParam /*LPARAM*/
            );

        /// <summary>
        /// </summary>
        [DllImport("user32.dll")]
        protected static extern IntPtr FindWindowEx(
            IntPtr hwndParent,
            IntPtr hwndChildAfter,
            string lpszClass,
            string lpszWindow
            );

        /// <summary>
        /// </summary>
        [DllImport("user32.dll")]
        protected static extern IntPtr FindWindow(
                        string lpClassName,
                        string lpWindowName
                    );

        /// <summary>
        /// </summary>
        [DllImport("oleacc.dll", ExactSpelling = true, PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        protected static extern object AccessibleObjectFromWindow(
                IntPtr hwnd,
                int dwObjectID,
                [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid);

        //[DllImport("Oleacc.dll")]
        //private static extern long AccessibleObjectFromWindow(int windowHandle,
        //    int objectID, Guid refID, ref IntPtr accessibleObject);

        /// <summary>
        /// </summary>
        [DllImport("oleacc.dll")]
        protected static extern
            int AccessibleChildren
                ([MarshalAs(UnmanagedType.Interface)] object /*IAccessible*/ paccContainer,
               int iChildStart,
               int cChildren,
            ///// [MarshalAs(UnmanagedType.LPArray)] [In,Out] Variant[] rgvarChildren,
            [Out] object[] rgvarChildren,
            out int pcObtained);

        /// <summary>
        /// </summary>
        [DllImport("oleacc.dll")]
        protected static extern
            int GetRoleText(
                int dwRole,
                [Out] StringBuilder lpszRole,
                int cchRoleMax);

        /// <summary>
        /// </summary>
        protected struct RECT
        {
            /// <summary>
            /// </summary>
            public int left;
            /// <summary>
            /// </summary>
            public int top;
            /// <summary>
            /// </summary>
            public int right;
            /// <summary>
            /// </summary>
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class POINT
        {
            public int x;
            public int y;

            public POINT()
            {
            }

            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        /// <summary>
        /// </summary>
        protected struct WINDOWINFO
        {
            /// <summary>
            /// </summary>
            public int cbsize;
            /// <summary>
            /// </summary>
            public RECT rcWindow;
            /// <summary>
            /// </summary>
            public RECT rcClient;
            /// <summary>
            /// </summary>
            public int dwStyle;
            /// <summary>
            /// </summary>
            public int dwExStyle;
            /// <summary>
            /// </summary>
            public int dwWindowStatus;
            /// <summary>
            /// </summary>
            public int cxWindowBorders;
            /// <summary>
            /// </summary>
            public int cyWindowBorders;
            /// <summary>
            /// </summary>
            public short atomWindowType;
            /// <summary>
            /// </summary>
            public short wCreatorversion;
        }

        /// <summary>
        /// </summary>
        protected internal const int
                SW_MAXIMIZE = 3,
                SW_SHOW = 5,
                SW_MINIMIZE = 6,
                SW_SHOWNA = 8,
                SW_RESTORE = 9,

                // from winable.h
                OBJID_WINDOW = 0x0;


        /// <summary>
        /// </summary>
        protected delegate bool /*BOOL*/
            EnumWindowsProc(
                IntPtr hwnd,     /*HWND*/
                IntPtr lparam    /*LPARAM*/
                );

        [StructLayout(LayoutKind.Explicit)]
        internal struct Variant
        {
            [FieldOffset(0)]
            public ushort vt; //2
            [FieldOffset(2)]
            public ushort wReserved1; //2
            [FieldOffset(4)]
            public ushort wReserved2; //2
            [FieldOffset(6)]
            public ushort wReserved3; //2
            [FieldOffset(8)]
            public VariantUnionPart union;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal /*unsafe*/ struct VariantUnionPart
        {
            [FieldOffset(0)]
            public long llval; // VT_I8
            [FieldOffset(0)]
            public int lVal; // VT_I4
            [FieldOffset(0)]
            public byte bVal; // VT_UI1
            [FieldOffset(0)]
            public short iVal; // VT_I2
            [FieldOffset(0)]
            public float fltVal; // VT_R4
            [FieldOffset(0)]
            public double dblVal; // VT_R8
            [FieldOffset(0)]
            public short boolVal; // VT_BOOL
            [FieldOffset(0)]
            public int scode; // VT_ERROR
            [FieldOffset(0)]
            public long cyVal; // VT_CY
            [FieldOffset(0)]
            public double date; // VT_DATE
            [FieldOffset(0)]
            public ushort bstrVal; // VT_BSTR
            //[ FieldOffset( 0 )]
            //public void* punkVal; // VT_UNKNOWN
            //[ FieldOffset( 0 )]
            //public void* pdispVal; // VT_DISPATCH
            [FieldOffset(0)]
            public IntPtr punkVal; // VT_UNKNOWN
            [FieldOffset(0)]
            public IntPtr pdispVal; // VT_DISPATCH
        }

        private const short CHILDID_SELF = 0x0;

        /// <summary>
        /// </summary>
        public enum IEToolBarButton
        {
            /// <summary>
            /// </summary>
            Back,
            /// <summary>
            /// </summary>
            Forward,
            /// <summary>
            /// </summary>
            Stop,
            /// <summary>
            /// </summary>
            Refresh,
            /// <summary>
            /// </summary>
            Home,
            /// <summary>
            /// </summary>
            Search,
            /// <summary>
            /// </summary>
            Favorites,
            /// <summary>
            /// </summary>
            History
        }

        /// <summary>
        /// </summary>
        public enum ResizeDirection
        {
            /// <summary>
            /// </summary>
            Left,
            /// <summary>
            /// </summary>
            Top,
            /// <summary>
            /// </summary>
            Right,
            /// <summary>
            /// </summary>
            Bottom,
            /// <summary>
            /// </summary>
            TopLeft,
            /// <summary>
            /// </summary>
            TopRight,
            /// <summary>
            /// </summary>
            BottomRight,
            /// <summary>
            /// </summary>
            BottomLeft
        }

        public enum OSVersion
        {
            WindowsXPWithIE6,
            WindowsXPWithIE7,
            LonghornWithIE6,
            LonghornWithIE7
        }

        private const int
            GA_ROOT = 2,
            GA_ROOTOWNER = 3;

        [DllImport("User32.dll")]
        private static extern IntPtr GetAncestor(IntPtr hwnd, int gaFlags);

        [DllImport("User32.dll")]
        private static extern IntPtr GetObject();

        [DllImport("User32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out System.Drawing.Rectangle lpRect);

        [DllImport("User32.dll")]
        private static extern bool GetClientRect(IntPtr hWnd, out System.Drawing.Rectangle lpRect);

        [DllImport("User32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hwnd, out IntPtr processId);

        #endregion

        #region MSAA
        internal void MSAAMenu()
        {
            IntPtr ww = FindWindowEx(_IE_windowhandle, IntPtr.Zero, "WorkerW", null);
            if (ww == IntPtr.Zero /* || ww is hidden */)
            {
                throw new ApplicationException("Could not find WorkerW window");
            }
            IntPtr rebarWindow = FindWindowEx(ww, IntPtr.Zero, "ReBarWindow32", null);

            if (rebarWindow == IntPtr.Zero)
            {
                throw new ApplicationException("Could not find RebarWindow window");
            }

            IntPtr cur_toolbarwindow = IntPtr.Zero;
            int currentguess = 1; // note 1 based, for e.g. 1st child in this case
            for (int i = 0; i < currentguess; ++i)
            {
                cur_toolbarwindow = FindWindowEx(rebarWindow, cur_toolbarwindow, "ToolbarWindow32", null);
            }

            LogManager.LogMessageDangerously(String.Format("Menu is: 0x{0}", cur_toolbarwindow.ToString("X8")));

            object objacc = AccessibleObjectFromWindow(cur_toolbarwindow, OBJID_WINDOW, _IID_IAccessible);
            LogManager.LogMessageDangerously(((objacc == null) ? "null" : objacc.GetType().ToString()));

            Type objacctype = objacc.GetType();
            int childct = (int)objacctype.InvokeMember("accChildCount",
                BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public,
                null,
                objacc,
                null);

            object[] acc_objchildren = new object[childct];
            int ctactual;

            int result = AccessibleChildren(objacc, 0, childct, acc_objchildren, out ctactual);
            LogManager.LogMessageDangerously("0x" + result.ToString("X8"));

            Type acc_obj_children_type;
            int j = 0;
            for (; j < ctactual; ++j)
            {
                acc_obj_children_type = acc_objchildren[j].GetType();
                string strname;
                try
                {
                    strname = acc_obj_children_type.InvokeMember(
                        "accName",
                        BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public,
                        null,
                        acc_objchildren[j],
                        null) as string;
                }
                catch (TargetInvocationException)
                {
                    break;
                }
                int accrole = (int)
                        acc_obj_children_type.InvokeMember(
                            "accRole",
                            BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public,
                            null,
                            acc_objchildren[j],
                            null);
                StringBuilder sbRoleText = new StringBuilder(255);
                GetRoleText((int)accrole, sbRoleText, 255);

                LogManager.LogMessageDangerously("M " + j + " : " + ((strname == null) ? "null" : strname) + " " + sbRoleText.ToString());
            }

            object accMenubar = acc_objchildren[j];
            Type accMenubarType = accMenubar.GetType();
            int count_menuitems = (int)accMenubarType.InvokeMember("accChildCount",
                BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public,
                null,
                accMenubar,
                null);
            LogManager.LogMessageDangerously("Menu item count: " + count_menuitems);

            int accrole_menubar = (int)accMenubar.GetType().InvokeMember(
                "accRole",
                BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public,
                null,
                accMenubar,
                null);

            StringBuilder sbRoleText2 = new StringBuilder(255);
            GetRoleText((int)accrole_menubar, sbRoleText2, 255);

            object[] acc_menuchildren = new object[count_menuitems];
            int ctactualmenuitems;

            int result2 = AccessibleChildren(
                    accMenubar, 0, count_menuitems, acc_menuchildren, out ctactualmenuitems);
            LogManager.LogMessageDangerously("0x" + result2.ToString("X8"));

            object curchild;
            for (short iMenuItem = 0; iMenuItem < 30; ++iMenuItem)
            {
                curchild = accMenubarType.InvokeMember(
                    "accChild",
                    BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public,
                    null,
                    accMenubar,
                    //null);
                    new object[] { (object)iMenuItem });
                string menuname = curchild.GetType().InvokeMember(
                                "accName",
                                BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public,
                                null,
                                curchild,
                                null) as string;
                LogManager.LogMessageDangerously("Child menu item : " + menuname);
            }
        }
        #endregion

        #region browser helpers, snipped from NavigationTestCase
        public static AutomationElement GetChromeBackButton()
        {
            if (ApplicationDeploymentHelper.GetIEVersion() > 6)
            {
                AutomationElement rootBrowserWindow = BrowserHelper.GetMainBrowserWindow(Application.Current).AutomationRoot;
                AndCondition isBackButton = new AndCondition(
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "Item 256"),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button));
                return rootBrowserWindow.FindFirst(TreeScope.Descendants, isBackButton);
            }
            else
            {
                AutomationElement rootBrowserWindow = BrowserHelper.GetMainBrowserWindow(Application.Current).AutomationRoot;
                AndCondition isBackButton = new AndCondition(
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "Item 1012"),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.SplitButton));
                return rootBrowserWindow.FindFirst(TreeScope.Descendants, isBackButton);
            }
        }

        public static AutomationElement GetChromeForwardButton()
        {
            if (ApplicationDeploymentHelper.GetIEVersion() > 6)
            {
                AutomationElement rootBrowserWindow = BrowserHelper.GetMainBrowserWindow(Application.Current).AutomationRoot;
                AndCondition isFwdButton = new AndCondition(
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "Item 257"),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button));
                return rootBrowserWindow.FindFirst(TreeScope.Descendants, isFwdButton);
            }
            else
            {
                AutomationElement rootBrowserWindow = BrowserHelper.GetMainBrowserWindow(Application.Current).AutomationRoot;
                AndCondition isFwdButton = new AndCondition(
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "Item 1013"),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.SplitButton));
                return rootBrowserWindow.FindFirst(TreeScope.Descendants, isFwdButton);
            }
        }

        public static bool IsEnabledPropertySet(AutomationElement automationElement)
        {
            bool b = (bool)automationElement.GetCurrentPropertyValue(AutomationElement.IsEnabledProperty);
            return b;
        }

        public static AutomationElement GetJournalDropdownButton()
        {
            if (ApplicationDeploymentHelper.GetIEVersion() > 6)
            {
                AndCondition isRecentPagesButton = new AndCondition(
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "Item 258"),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button));
                AutomationElement rootBrowserWindow = BrowserHelper.GetMainBrowserWindow(Application.Current).AutomationRoot;
                return rootBrowserWindow.FindFirst(TreeScope.Descendants, isRecentPagesButton);
            }
            else
            {
                Log.Current.CurrentVariation.LogMessage("Not supported for IE6");
                return null; 
            }
        }

        private static string[] s_chevronItems = null; // get populated in GetMenuItems method
        public static String[] GetChevronItems(AutomationElement navigationButton)
        {
            if (ApplicationDeploymentHelper.GetIEVersion() > 6)
            {
                MTI.Input.MoveToAndClick(navigationButton.GetClickablePoint());

                // need to retrieve the chevronItems in a seperate thread to prevent hangs
                Thread thread = new Thread(new ThreadStart(GetMenuItems));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join(TimeSpan.FromMilliseconds(10000)); // blocks the caller until thread terminates

                // return the populated chevronItems
                return s_chevronItems;
            }
            else
            {
                Log.Current.CurrentVariation.LogMessage("No IE Journal integration in IE6");
                return null;
            }
        }

        // this method gets executed in a different thread
        private static void GetMenuItems()
        {
            Log.Current.CurrentVariation.LogMessage("In GetMenuItems");
            // There's only ever one of these that's a direct descendant of root.
            PropertyCondition isJournalMenu = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Menu);
            PropertyCondition isJournalMenuItem = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.MenuItem);
            // Get the menu 
            AutomationElement menu = null;
            int timeout = 6; // try for 12 seconds
            // wait in a while loop to find the menu item
            while ((menu == null) && (timeout >= 0))
            {
                menu = AutomationElement.RootElement.FindFirst(TreeScope.Children, isJournalMenu);
                Thread.Sleep(2000);
                timeout--;
            }

            if (menu == null)
            {
                Log.Current.CurrentVariation.LogMessage("Couldn't instantiate the menu AutomationElement");
                return;
            }

            // Get the children of the menu
            AutomationElementCollection childMenus = menu.FindAll(TreeScope.Descendants, isJournalMenuItem);
            if (childMenus == null)
            {
                Log.Current.CurrentVariation.LogMessage("Couldn't instantiate the childMenus AutomationElement");
                return;
            }

            if (childMenus.Count < 2)
            {
                Log.Current.CurrentVariation.LogMessage("Didn't find expected number of chevron elements");
                return;
            }

            BrowserHelper.s_chevronItems = new string[childMenus.Count - 1];
            // Get all the elements,  except last item which is "History"
            for (int index = 0; index < childMenus.Count; index++)
            {
                if (index < childMenus.Count - 1) // exclude last element
                {
                    BrowserHelper.s_chevronItems[index] = childMenus[index].Current.Name;
                    Log.Current.CurrentVariation.LogMessage("Found Chevron Item " + childMenus[index].Current.Name);
                }
            }

            Log.Current.CurrentVariation.LogMessage("Exits GetMenuItems. Timeout = " + timeout.ToString());
        }

        // invoke the browser back button
        public static void InvokeGoBackButton()
        {
            if (ApplicationDeploymentHelper.GetIEVersion() > 6)
            {
                AutomationElement automationElementBack = BrowserHelper.GetChromeBackButton();

                if (automationElementBack != null)
                {
                    if (IsEnabledPropertySet(automationElementBack).Equals(true))
                    {
                        Microsoft.Test.Deployment.IEAutomationHelper.ClickIEBackButton(BrowserHelper.GetMainBrowserWindow(Application.Current).AutomationRoot);
                    }
                    else
                    {
                        Log.Current.CurrentVariation.LogMessage("Back button is not enabled");
                    }
                }
                else
                {
                    LogManager.LogMessageDangerously("Hit issue navigating back, trying explicit API navigation");
                    ((NavigationWindow)Application.Current.MainWindow).GoBack();
                }
            }
            else
            {
                // Hit Alt-Back, since IE's back will be disabled here in IE6
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftAlt, true);
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.Left, true);
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.Left, false);
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftAlt, false);

                Thread.Sleep(1000);
            }
        }

        // invoke the browser forward button
        public static void InvokeGoForwardButton()
        {
            if (ApplicationDeploymentHelper.GetIEVersion() > 6)
            {
                AutomationElement automationElementForward = BrowserHelper.GetChromeForwardButton();
                if (automationElementForward != null)
                {
                    if (IsEnabledPropertySet(automationElementForward).Equals(true))
                    {
                        Microsoft.Test.Deployment.IEAutomationHelper.ClickIEFwdButton(BrowserHelper.GetMainBrowserWindow(Application.Current).AutomationRoot);
                    }
                    else
                    {
                        Log.Current.CurrentVariation.LogMessage("Forward button is not enabled");
                    }
                }
                else
                {
                    LogManager.LogMessageDangerously("Hit issue navigating forward, trying explicit API navigation");
                    ((NavigationWindow)Application.Current.MainWindow).GoForward();
                }
            }
            else
            {
                // Hit Shift-backspace, since IE's back will be disabled here in IE6
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftShift, true);
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.Back, true);
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.Back, false);
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftShift, false);

                Thread.Sleep(1000);
            }
        }

        // Click on the center of the browser window
        public static void ClickCenterOfBrowser()
        {
            AutomationElement rootBrowserWindow = BrowserHelper.GetMainBrowserWindow(Application.Current).AutomationRoot;
            Rect bounds = rootBrowserWindow.Current.BoundingRectangle;
            Point center = new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);
            MTI.Input.MoveToAndClick(center);

            Thread.Sleep(1000);
        }

        // Move to the center of the browser window
        public static void MoveToCenterOfBrowser()
        {
            AutomationElement rootBrowserWindow = BrowserHelper.GetMainBrowserWindow(Application.Current).AutomationRoot;
            Rect bounds = rootBrowserWindow.Current.BoundingRectangle;
            Point center = new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);
            MTI.Input.MoveTo(center); ;
        }

        public static String[] GetNames(AutomationElementCollection ac)
        {
            String[] items = new String[ac.Count];
            for (int i = 0; i < ac.Count; i++)
            {
                items[i] = ac[i].Current.Name;
            }
            return items;
        }

        public static String[] GetForwardMenuChevronItems()
        {
            return GetChevronItems(GetChromeForwardButton());
        }

        public static String[] GetBackMenuChevronItems()
        {
            return GetChevronItems(GetChromeBackButton());
        }

        /// <summary>
        /// Goes through the string array that represents the journal entry collection 
        /// and prints each journal entry to the console/log file
        /// </summary>
        /// <param name="a">Journal stack to list contents of.</param>
        public static void PrintArray(String[] a)
        {
            for (int i = 0; i < a.Length; i++)
            {
                Log.Current.CurrentVariation.LogMessage("Item [" + i + "]: " + a[i]);
            }
        }

        /// <summary>
        /// Compares the contents of 2 journal stacks (back/forward)
        /// </summary>
        /// <param name="a">First journal stack to compare</param>
        /// <param name="b">Second journal stack to compare</param>
        /// <param name="b">Length of the string to compare</param>
        /// <returns>true if the journal stacks are completely equal (after a deep comparison); false otherwise</returns>
        public static bool IsSame(String[] a, String[] b, int length)
        {
            bool match = true;
            if (a.Length != b.Length)
            {
                Log.Current.CurrentVariation.LogMessage("Journal entry count: NO MATCH. (" + a.Length + " vs. " + b.Length + ")");
                return false;
            }

            // compare strings against the selected length
            for (int i = 0; i < a.Length; i++)
            {
                int matchlength = length;
                // if string lengths are smaller than length, then choose the smallest length of a[i] and b[i]
                if (length > a.Length)
                {
                    matchlength = a[i].Length;
                }
                if (matchlength > b[i].Length)
                {
                    matchlength = b[i].Length;
                }

                if (!a[i].Substring(0, matchlength).Equals(b[i].Substring(0, matchlength), StringComparison.CurrentCultureIgnoreCase))
                {
                    Log.Current.CurrentVariation.LogMessage("Journal entry #" + i + " : NO MATCH. (" + a[i] + " vs. " + b[i] + ")");
                    match = false;
                    break;
                }
            }
            return match;
        }

        /// <summary>
        /// Checks if the two string arrays that represent the journal entry collections
        /// are equal and then logs a pass/fail based on the results of the comparison.
        /// </summary>
        /// <param name="a">First journal stack to compare</param>
        /// <param name="b">Second journal stack to compare</param>
        /// <param name="blength">Length of the string to compare</param>
        public static bool Match(String[] a, String[] b, int length)
        {
            if (!IsSame(a, b, length)) // only compare the given length because exact expected length cannot be predicted
            {
                Log.Current.CurrentVariation.LogMessage("Arrays mismatch");
                return false;
            }
            else
            {
                Log.Current.CurrentVariation.LogMessage("Arrays match");
                return true;
            }
        }

        /// <summary>
        /// Performs an automated click on the Hyperlink by clicking within the bounding rectangle of the
        /// TextBlock the Hyperlink is contained in
        /// </summary>
        /// <param name="textBlockId">Name of the TextBlock containing the Hyperlink</param>
        public static void FindAndClickTextBlockHyperlink(String textBlockId)
        {
            TreeWalker textBlockWalker = new TreeWalker(new PropertyCondition(AutomationElement.AutomationIdProperty, textBlockId));
            AutomationElement hyperlink = textBlockWalker.GetFirstChild(BrowserHelper.GetMainBrowserWindow(Application.Current).AutomationRoot);
            Rect border = hyperlink.Current.BoundingRectangle;
            MTI.Input.MoveToAndClick(new Point(border.Left + 7, border.Top + 7));
        }

        /// <summary>
        /// Looks for an AutomationElement with name [id] in the subtree rooted at [root] 
        /// Search scope includes [root] and all its descendant AutomationElements
        /// </summary>
        /// <param name="root">AutomationElement at the root of the subtree where we'll be looking</param>
        /// <param name="id">Name of AutomationElement to find</param>
        /// <returns>AutomationElement with name [id]</returns>
        public static AutomationElement FindAutomationElementById(AutomationElement root, String id)
        {
            PropertyCondition nameCondition = new PropertyCondition(AutomationElement.AutomationIdProperty, id);
            return root.FindFirst(TreeScope.Descendants, nameCondition);
        }
        #endregion

        #region helpers : invoke Button, Hyperlink
        /// <summary>
        /// Clicks on the Button via Automation
        /// </summary>
        /// <param name="btnName">Value of the Name property for Button</param>
        /// <param name="searchFrom">Root to start searching for Button from</param>
        public static void InvokeButton(String btnName, DependencyObject searchFrom)
        {
            Button btnTarget = LogicalTreeHelper.FindLogicalNode(searchFrom, btnName) as Button;

            // If we found the target button, click it using Automation.  Else, fail test.
            if (btnTarget != null)
            {
                Log.Current.CurrentVariation.LogMessage("Clicking on the control '" + btnName + "' via Automation");
                AutomationPeer ap = UIElementAutomationPeer.CreatePeerForElement(btnTarget);
                IInvokeProvider iip = (IInvokeProvider)ap.GetPattern(PatternInterface.Invoke);
                iip.Invoke();
            }
            else
                Log.Current.CurrentVariation.LogMessage("Could not find Button '" + btnName + "' in the subtree rooted at " + searchFrom.ToString());
        }


        /// <summary>
        /// Clicks on the Hyperlink via the DoClick method
        /// </summary>
        /// <param name="hlinkName">Value of the Name property for Hyperlink</param>
        /// <param name="searchFrom">Root to start searching for Hyperlink from</param>
        public static void InvokeHyperlink(String hlinkName, DependencyObject searchFrom)
        {
            Hyperlink hlinkTarget = LogicalTreeHelper.FindLogicalNode(searchFrom, hlinkName) as Hyperlink;

            // If we found the target hyperlink, click it.  Else, fail test. 
            if (hlinkTarget != null)
                hlinkTarget.DoClick();
            else
                Log.Current.CurrentVariation.LogMessage("Could not find Hyperlink '" + hlinkName + "' in the subtree rooted at " + searchFrom.ToString());
        }

        /// <summary>
        /// Raise the Hyperlink's RequestNavigate event, which mimics a Hyperlink click
        /// </summary>
        /// <param name="hlinkName">Value of the Name property for Hyperlink</param>
        /// <param name="searchFrom">Root to start searching for Hyperlink from</param>
        public static void NavigateHyperlinkViaEvent(String hlinkName, DependencyObject searchFrom)
        {
            Log.Current.CurrentVariation.LogMessage("Navigating Hyperlink '" + hlinkName + "' via RequestNavigateEvent");
            Hyperlink hlinkTarget = LogicalTreeHelper.FindLogicalNode(searchFrom, hlinkName) as Hyperlink;

            if (hlinkTarget != null)
            {
                RequestNavigateEventArgs requestNavigateEventArgs = new RequestNavigateEventArgs(hlinkTarget.NavigateUri, hlinkTarget.TargetName);
                requestNavigateEventArgs.Source = hlinkTarget;
                hlinkTarget.RaiseEvent(requestNavigateEventArgs);
            }
            else
                Log.Current.CurrentVariation.LogMessage("Could not find Hyperlink '" + hlinkName + "' in the subtree rooted at " + searchFrom.ToString());
        }

        #endregion

    }
}
