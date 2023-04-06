// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// Make use of this class when you want to get journaling info 
    /// from WITHIN your app using the back/fwd DependencyProperties and 
    /// CanGoBack / CanGoForward properties.
    /// 
    /// Use NavigationWindowHelper / BrowserHelper when you want to get journaling 
    /// info by inspecting properties through UIAutomation and testing OUT-OF-PROC.
    /// </summary>
    public class JournalHelper : IProvideJournalingState
    {
        private NavigationWindow _navWin = null;
        private Frame            _frame = null;

        public JournalHelper()
        {
            _navWin = Application.Current.MainWindow as NavigationWindow;
        }

        public bool IsBackEnabled(ContentControl c)
        {
            if (c is NavigationWindow)
            {
                _navWin = c as NavigationWindow;
                return _navWin.CanGoBack;
            }
            else if (c is Frame)
            {
                _frame = c as Frame;
                return _frame.CanGoBack;
            }
            else
            {
                Log.Current.CurrentVariation.LogMessage("Cannot check a non-Frame, non-NavigationWindow object's CanGoBack property.");
                return false;
            }
        }

        public bool IsForwardEnabled(ContentControl c)
        {
            if (c is NavigationWindow)
            {
                _navWin = c as NavigationWindow;
                return _navWin.CanGoForward;
            }
            else if (c is Frame)
            {
                _frame = c as Frame;
                return _frame.CanGoForward;
            }
            else
            {
                Log.Current.CurrentVariation.LogMessage("Cannot check a non-Frame, non-NavigationWindow object's CanGoForward property");
                return false;
            }
        }

        protected String[] GetNames(IEnumerable stack)
        {
            if (stack != null)
            {
                List<string> items = new List<string>();
                foreach (JournalEntry je in stack)
                {
                    items.Add(je.Name);
                }
                return items.ToArray();
            }
            else
            {
                return null;
            }
        }

        public String[] GetBackMenuItems(ContentControl c)
        {
            IEnumerable backStack = null; 

            if (c is NavigationWindow)
            {
                _navWin = c as NavigationWindow;
                backStack = (IEnumerable)_navWin.GetValue(NavigationWindow.BackStackProperty);
                return GetNames(backStack);
            }
            else if (c is Frame)
            {
                _frame = c as Frame;
                backStack = (IEnumerable)_frame.GetValue(Frame.BackStackProperty);
                return GetNames(backStack);
            }
            else
            {
                Log.Current.CurrentVariation.LogMessage("Cannot check a non-Frame, non-NavigationWindow's BackStackProperty");
                return null;
            }
        }

        public String[] GetForwardMenuItems(ContentControl c)
        {
            IEnumerable forwardStack = null;

            if (c is NavigationWindow)
            {
                _navWin = c as NavigationWindow;
                forwardStack = (IEnumerable)_navWin.GetValue(NavigationWindow.ForwardStackProperty);
                return GetNames(forwardStack);
            }
            else if (c is Frame)
            {
                _frame = c as Frame;
                forwardStack = (IEnumerable)_frame.GetValue(Frame.ForwardStackProperty);
                return GetNames(forwardStack);
            }
            else
            {
                Log.Current.CurrentVariation.LogMessage("Cannot check a non-Frame, non-NavigationWindow's ForwardStackProperty");
                return null;
            }
        }

        public String WindowTitle
        {
            get 
            { 
                if (_navWin == null && 
                    Application.Current.MainWindow is NavigationWindow)
                    return ((NavigationWindow)Application.Current.MainWindow).Title;
                else
                    return _navWin.Title;
            }
        }

        #region NavigationWindow specific JournalHelper
        public JournalHelper(NavigationWindow navigationWindow)
        {
            _navWin = navigationWindow;
        }

        public NavigationWindow ActiveNavigationWindow
        {
            get { return _navWin; }
            set { _navWin = value; }
        }

        public bool IsBackEnabled()
        {
            return _navWin.CanGoBack;
        }

        public bool IsForwardEnabled()
        {
            return _navWin.CanGoForward;
        }

        public String[] GetBackMenuItems()
        {
            IEnumerable backStack = (IEnumerable)_navWin.GetValue(NavigationWindow.BackStackProperty);
            return GetNames(backStack);
        }

        public String[] GetForwardMenuItems()
        {
            IEnumerable forwardStack = (IEnumerable)_navWin.GetValue(NavigationWindow.ForwardStackProperty);
            return GetNames(forwardStack);
        }
        #endregion
    }
}
