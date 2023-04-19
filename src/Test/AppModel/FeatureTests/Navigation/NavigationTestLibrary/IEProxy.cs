// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Automation;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows;
using System.Threading;
using MTI = Microsoft.Test.Input;
using Microsoft.Test.Deployment;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// IE browser ui interop class
    /// </summary>
    abstract public class IEProxy : IBrowserUIProvider
    {
        protected Dictionary<String, String> AutomationIds = new Dictionary<String, String>();

        public IEProxy()
        {
            AutomationIds.Add("AddressBar", "41477");
            AutomationIds.Add("Minimize", "Minimize");
            AutomationIds.Add("Maximize", "Maximize");
            AutomationIds.Add("Close", "Close");
        }
        /// <summary>
        /// Gets the status bar automation element
        /// </summary>
        public AutomationElement GetStatusBar(AutomationElement RootIEAutomationElement)
        {
            if (RootIEAutomationElement == null)
            {
                throw new ArgumentException("Automation handle passed in for IE window was invalid");
            }

            AutomationElement ae_statusbar = null;

            AutomationElementCollection statusbarcollection = RootIEAutomationElement.FindAll(
                    TreeScope.Descendants | TreeScope.Element,
                    new PropertyCondition(AutomationElement.ClassNameProperty, "msctls_statusbar32"));
            if (statusbarcollection.Count <= 0)
            {
                throw new ApplicationException("Status bar not found in IE window");
            }
            else if (statusbarcollection.Count > 1)
            {
                throw new ApplicationException("More than one status bar control found in IE");
            }
            else
            {
                ae_statusbar = statusbarcollection[0];
            }

            // if (ae_statusbar == null)
            // {
            //     throw new ApplicationException("StatusBar Element of IE was unable to be found");
            // }

            return ae_statusbar;
        }

        /// <summary>
        /// Gets the status bar text
        /// </summary>
        public string GetStatusText(AutomationElement RootIEAutomationElement)
        {
            if (RootIEAutomationElement == null)
            {
                throw new ArgumentException("Automation handle passed in for IE window was invalid");
            }
            AutomationElement statusBar = GetStatusBar(RootIEAutomationElement);
            if (statusBar == null)
            {
                throw new ApplicationException("Could not find the IE status bar");
            }

            // assumption: first child of Statusbar control is the status bar text.
            TreeWalker twalker = TreeWalker.ContentViewWalker;
            AutomationElement ae_statustext = twalker.GetFirstChild(statusBar);
            if (ae_statustext == null)
            {
                throw new ApplicationException("Status Text area of status bar not found");
            }

            string statusText = ae_statustext.GetCurrentPropertyValue(AutomationElement.NameProperty) as string;
            //ae_statusbar.
            return statusText;
        }

        /// <summary>
        /// Gets the zone information
        /// </summary>
        public string GetStatusZoneInfoText(AutomationElement RootIEAutomationElement)
        {
            if (RootIEAutomationElement == null)
            {
                throw new ArgumentException("Automation handle passed in for IE window was invalid");
            }

            AutomationElement statusBar = GetStatusBar(RootIEAutomationElement);
            if (statusBar == null)
            {
                throw new ApplicationException("Could not find the IE status bar");
            }

            // assumption: last child of Statusbar control is the Zone information (except maybe grip control)
            TreeWalker twalker = TreeWalker.ContentViewWalker;
            AutomationElement zoneTextFinder = twalker.GetFirstChild(statusBar);
            if (zoneTextFinder == null)
            {
                throw new ApplicationException("Status Text area of status bar not found");
            }
            AutomationElement nextSibling;
            while ((nextSibling = twalker.GetNextSibling(zoneTextFinder)) != null)
            {
                // as soon as we encounter Grip property, we will just quit since we assume that is
                // the last sibling.
                if ((nextSibling.GetCurrentPropertyValue(AutomationElement.LocalizedControlTypeProperty) as string) == "Grip")
                {
                    // in contentview walk, looks like the resize grip will never 
                    // be a sibling, so we really don't need to do this but I left this code in here anyway.
                    break;
                }
                zoneTextFinder = nextSibling;
            }

            return zoneTextFinder.GetCurrentPropertyValue(AutomationElement.NameProperty) as string;
        }

        /// <summary>
        /// Gets the menu bar
        /// </summary>
        public AutomationElement GetMenuBar(AutomationElement RootIEAutomationElement)
        {
            if (RootIEAutomationElement == null)
            {
                throw new ArgumentException("Automation handle passed in for IE window was invalid");
            }

            /*
             * 1. We cannot just use the IE window and search for the Menu bar
             * automationelement b/c of the preponderance of names like Edit / Favorites 
             * which occur in many places. We must first find the toolbarwindow assoc with the menu bar and
             * and restrict searches to that.
             * 
             * for now we use a hack since "File" menu item only appears in the menu bar. 
             * "File" menu item has AutomationID 
             * The MenuBar property of WebBrowser should already be set to true.
             * 
             * 2. Trying to find parent throws exception saying only TreeScope.Children, Descendants etc. 
             * are permitted. hence the roundabout code.
             * 
             * 3. Since we search for ToolbarWindow32, this may be specific to this OS
             */

            AutomationElementCollection toolbarCollection = RootIEAutomationElement.FindAll(
                    TreeScope.Descendants | TreeScope.Element,
                    new PropertyCondition(AutomationElement.ClassNameProperty, "ToolbarWindow32"));

            // find the File menu item (locale independently) in the toolbars. This will signal that this is
            // the Menu toolbar
            PropertyCondition fileMenuCond = new PropertyCondition(
                AutomationElement.AutomationIdProperty, AutomationIdsTable.GetUndecoratedId(AutomationIdsTable.File));
            foreach (AutomationElement el in toolbarCollection)
            {
                if (el.FindFirst(TreeScope.Descendants | TreeScope.Element, fileMenuCond) != null)
                {
                    // contains the File menu option.. so this is likely the menubar.
                    return el;
                }
            }
            return null;
        }

        abstract public IDictionary<string, AutomationElement> GetAddressBarParts(AutomationElement RootIEAutomationElement);
        abstract public AutomationElement GetBackForwardButtonContainer(AutomationElement RootIEAutomationElement);
        abstract public string Description { get;}

        public AutomationElement GetStandardButtonToolbar(AutomationElement RootIEAutomationElement)
        {
            PropertyCondition pc = new PropertyCondition(AutomationElement.AutomationIdProperty, "40960");
            return RootIEAutomationElement.FindFirst(TreeScope.Descendants, pc);
        }
        
        InteractionModes _interactionMode = InteractionModes.Mouse;
        public InteractionModes InteractionMode
        {
            get { return _interactionMode; }
            set { _interactionMode = value; }
        }

        abstract public void InvokeDropdownGoBack(AutomationElement RootIEAutomationElement, int dropdownIndex);
        abstract public void InvokeDropdownGoForward(AutomationElement RootIEAutomationElement, int dropdownIndex);
        
        abstract public Stack<String> GetBackChevronItems(AutomationElement RootIEAutomationElement);
        abstract public Stack<String> GetForwardChevronItems(AutomationElement RootIEAutomationElement);

        public void InvokeRefresh(AutomationElement RootIEAutomationElement)
        {
            if (InteractionMode == InteractionModes.Mouse)
            {
                AutomationElement refreshElement = GetRefreshButton(RootIEAutomationElement);
                MTI.Input.MoveToAndClick(refreshElement.GetClickablePoint());
                Thread.Sleep(5000);
            }
            else if (InteractionMode == InteractionModes.KeyboardShortcut)
            {
                MTI.Input.SendKeyboardInput(Key.BrowserRefresh, true);
                MTI.Input.SendKeyboardInput(Key.BrowserRefresh, false);
            }
            else if (InteractionMode == InteractionModes.Menu)
            {

            }
        }

        protected AutomationElementCollection GetDesktopPopupItems()
        {
            PropertyCondition menuCondition = new PropertyCondition(AutomationElement.NameProperty, "Menu");
            AutomationElement menu = AutomationElement.RootElement.FindFirst(TreeScope.Children, menuCondition);
            PropertyCondition invokeAble = new PropertyCondition(AutomationElement.IsInvokePatternAvailableProperty, true);
            AutomationElementCollection ac = menu.FindAll(TreeScope.Descendants, invokeAble);
            if (ac.Count == 0)
            {
                throw new ApplicationException("No desktop popup items were found");
            }
            return ac;
        }

        public AutomationElement GetBackButton(AutomationElement RootIEAutomationElement)
        {
            AutomationElement travelBand = GetBackForwardButtonContainer(RootIEAutomationElement);
            PropertyCondition backButtonCondition = new PropertyCondition(AutomationElement.NameProperty, "Back");
            AutomationElement backButton = travelBand.FindFirst(TreeScope.Descendants, backButtonCondition);
            return backButton;
        }

        public AutomationElement GetForwardButton(AutomationElement RootIEAutomationElement)
        {
            AutomationElement travelBand = GetBackForwardButtonContainer(RootIEAutomationElement);
            PropertyCondition forwardButtonCondition = new PropertyCondition(AutomationElement.NameProperty, "Forward");
            AutomationElement forwardButton = travelBand.FindFirst(TreeScope.Descendants, forwardButtonCondition);
            return forwardButton;
        }

        public AutomationElement FindAutomationElementById(AutomationElement root, String id)
        {
            PropertyCondition nameCondition = new PropertyCondition(AutomationElement.AutomationIdProperty, id);
            return root.FindFirst(TreeScope.Descendants, nameCondition);
        }
            
        public abstract AutomationElement GetRefreshButton(AutomationElement RootIEAutomationElement);
                
        public AutomationElement GetStopLoadingButton(AutomationElement RootIEAutomationElement)
        {
            return FindAutomationElementById(RootIEAutomationElement, "Stop");
        }
        
        public void InvokeGoBack(AutomationElement RootIEAutomationElement)
        {
            if (InteractionMode == InteractionModes.Mouse)
            {
            }
            else if (InteractionMode == InteractionModes.KeyboardShortcut)
            {
                MTI.Input.SendKeyboardInput(Key.BrowserBack, true);
                MTI.Input.SendKeyboardInput(Key.BrowserBack, false);
            }
            else if (InteractionMode == InteractionModes.Menu)
            {
                
            }
        }

        public void InvokeGoForward(AutomationElement RootIEAutomationElement)
        {
            if (InteractionMode == InteractionModes.Mouse)
            {
            }
            else if (InteractionMode == InteractionModes.KeyboardShortcut)
            {
                MTI.Input.SendKeyboardInput(Key.BrowserForward, true);
                MTI.Input.SendKeyboardInput(Key.BrowserForward, false);
            }
            else if (InteractionMode == InteractionModes.Menu)
            {

            }
        }

        protected void InvokeIEDropdownItem(AutomationElementCollection chevronItems, int dropdownIndex)
        {
            String[] splits;
            int n = -1;
            foreach (AutomationElement a in chevronItems)
            {
                splits = a.Current.AutomationId.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
                if (splits.Length == 2)
                {
                    n = int.Parse(splits[1]);
                    if (splits[0].Equals("Item") && n == dropdownIndex)
                    {
                        // this is a back stack item
                        MTI.Input.MoveToAndClick(a.GetClickablePoint());
                        Thread.Sleep(10000);
                    }
                }
            }
        }

        protected Stack<String> GetHistoryStack(AutomationElementCollection allItems, int itemStart, int itemEnd)
        {
            Stack<String> stack = new Stack<String>();
            List<String> itemsList = new List<String>();
            String[] splits;
            int n = -1;
            // select only the ones with Item index 1-9
            foreach (AutomationElement a in allItems)
            {
                splits = a.Current.AutomationId.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (splits.Length == 2)
                {
                    n = int.Parse(splits[1]);
                    if (splits[0].Equals("Item") && n > itemStart && n < itemEnd)
                    {
                        // this is a back stack item
                        itemsList.Add(a.Current.Name);
                    }
                }
            }
            for (int i = 0; i < itemsList.Count; i++)
            {
                stack.Push(itemsList[i]);
            }
            if (stack.Count == 0)
            {
                throw new ApplicationException("Stack is empty");
            }
            return stack;
        }

        public void NavigateByAddressBar(AutomationElement RootIEAutomationElement, String uri)
        {
            AutomationElement element = FindAutomationElementById(RootIEAutomationElement, AutomationIds["AddressBar"]);
            Rect bounds = element.Current.BoundingRectangle;
            Point p = new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);
            MTI.Input.MoveToAndClick(p);
            MTI.Input.MoveToAndClick(p);
            Thread.Sleep(5000);            
            MTI.Input.SendUnicodeString(uri);
            MTI.Input.SendKeyboardInput(System.Windows.Input.Key.Enter, true);
            MTI.Input.SendKeyboardInput(System.Windows.Input.Key.Enter, false);            
        }

        protected void ClickElementWithAutomationId(AutomationElement RootIEAutomationElement, String id)
        {
            AutomationElement element = FindAutomationElementById(RootIEAutomationElement, id);
            if (element == null)
            {
                throw new ApplicationException("AutomationElement wasn't found - id = " + id
                            + " RootIEAutomationElement Name = " + RootIEAutomationElement.Current.Name
                            + " ClassName = " + RootIEAutomationElement.Current.ClassName);
            }
            Rect bounds = element.Current.BoundingRectangle;
            Point p = new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);           
            MTI.Input.MoveToAndClick(p);
            System.Threading.Thread.Sleep(5000);                
        }
        public void Minimize(AutomationElement RootIEAutomationElement)
        {
            ClickElementWithAutomationId(RootIEAutomationElement, AutomationIds["Minimize"]);            
        }
        
        public void Maximize(AutomationElement RootIEAutomationElement)
        {
            ClickElementWithAutomationId(RootIEAutomationElement, AutomationIds["Maximize"]);            
        }
        
        public void Close(AutomationElement RootIEAutomationElement)
        {
            ClickElementWithAutomationId(RootIEAutomationElement, AutomationIds["Close"]);            
        }
    }
}
