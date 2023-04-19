// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Automation;
using System.Collections.Generic;
using System.Diagnostics;
using MTI = Microsoft.Test.Input;
using System.Windows.Input;
using System.Windows;
using System.Threading;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// IE7 browser ui interop class
    /// </summary>
    public class IE7Proxy : IEProxy
    {
        public IE7Proxy()
        {
            AutomationIds.Add("Refresh", "Item 102");
        }
        /// <summary>
        /// Quick description
        /// </summary>
        public override string Description
        {
            get
            {
                return "IE7 Browser";
            }
        }

        public override IDictionary<string, AutomationElement> GetAddressBarParts(AutomationElement RootIEAutomationElement)
        {
            return null;
        }

        public override AutomationElement GetBackForwardButtonContainer(AutomationElement RootIEAutomationElement)
        {
            // Find all the ToolbarWindow32's in the IEWindow
            PropertyCondition toolBarName = new PropertyCondition(AutomationElement.ClassNameProperty, "ToolbarWindow32");
            PropertyCondition toolBarClass = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolBar);
            AndCondition toolBar = new AndCondition(toolBarName, toolBarClass);

            AutomationElementCollection ieToolBars = RootIEAutomationElement.FindAll(TreeScope.Descendants, toolBar);
            if (ieToolBars.Count == 0)
            {
                throw new ApplicationException("Did not find any ToolbarWindow32 items in Internet Explorer");
            }
            else
            {
                // Find the ToolbarWindow32 that contains the 'Back' button
                PropertyCondition ieButtonControlType = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button);
                PropertyCondition isBackButton = new PropertyCondition(AutomationElement.AutomationIdProperty, "Item 256");
                AndCondition backButton = new AndCondition(ieButtonControlType, isBackButton);

                for (int i = 0; i < ieToolBars.Count; i++)
                {
                    AutomationElement journalToolbar = ieToolBars[i].FindFirst(TreeScope.Descendants, backButton);
                    if (journalToolbar != null)
                        return ieToolBars[i];
                }
            }

            return null;
        }
                               
        public override void InvokeDropdownGoBack(AutomationElement RootIEAutomationElement, int dropdownIndex)
        {
            AutomationElementCollection chevronItems = GetDropdownAutomationElements(RootIEAutomationElement);
            dropdownIndex = 10 - dropdownIndex;
            if (dropdownIndex > 0 && dropdownIndex < 10)
            {                
                InvokeIEDropdownItem(chevronItems, dropdownIndex);
            }
        }

        public override void InvokeDropdownGoForward(AutomationElement RootIEAutomationElement, int dropdownIndex)
        {
            AutomationElementCollection chevronItems = GetDropdownAutomationElements(RootIEAutomationElement);
            dropdownIndex =  10 + dropdownIndex;
            if (dropdownIndex > 10 && dropdownIndex < 20)
            {
                InvokeIEDropdownItem(chevronItems, dropdownIndex);
            }                               
        }
                
        protected void ClickDropDown(AutomationElement travelBand)
        {
	    PropertyCondition pc = new PropertyCondition(AutomationElement.NameProperty, "Recent Pages");
            AutomationElement dropdownButton = travelBand.FindFirst(TreeScope.Descendants, pc);
            MTI.Input.MoveToAndClick(dropdownButton.GetClickablePoint());
            Thread.Sleep(10000);
        }

        protected AutomationElementCollection GetDropdownAutomationElements(AutomationElement RootIEAutomationElement)
        {
            AutomationElement travelBand = GetBackForwardButtonContainer(RootIEAutomationElement);
            ClickDropDown(travelBand);
            return GetDesktopPopupItems();
        }
       
        public override Stack<String> GetBackChevronItems(AutomationElement RootIEAutomationElement)
        {
            AutomationElement backButton = GetBackButton(RootIEAutomationElement);
            Stack<String> items = new Stack<string>();
            if (backButton.Current.IsEnabled)
            {
                AutomationElementCollection allItems = GetDropdownAutomationElements(RootIEAutomationElement);
                // back stack items are 1-9
                items = GetHistoryStack(allItems, 0, 10);
                AutomationElement travelBand = GetBackForwardButtonContainer(RootIEAutomationElement);
                ClickDropDown(travelBand);
            }
            return items;
        }

        public override Stack<String> GetForwardChevronItems(AutomationElement RootIEAutomationElement)
        {
            AutomationElement fwdButton = GetForwardButton(RootIEAutomationElement);
            Stack<String> items = new Stack<string>();
            if (fwdButton.Current.IsEnabled)
            {
                AutomationElementCollection allItems = GetDropdownAutomationElements(RootIEAutomationElement);
                // forward stack items are 11-19
                items = GetHistoryStack(allItems, 10, 20);
                AutomationElement travelBand = GetBackForwardButtonContainer(RootIEAutomationElement);
                ClickDropDown(travelBand);
            }
            return items;
        }

        public override AutomationElement GetRefreshButton(AutomationElement RootIEAutomationElement)
        {
            return FindAutomationElementById(RootIEAutomationElement, AutomationIds["Refresh"]);
        }
    }
}
