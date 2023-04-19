// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Automation;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using MTI = Microsoft.Test.Input;
using System.Threading;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// Windows XP IE6 browser ui interop class
    /// </summary>
    public class WindowsXPIE6Proxy : IEProxy
    {
        public WindowsXPIE6Proxy()
        {
            // AutomationIds.Add("Refresh", "Item 1018");
        }
        /// <summary>
        /// Quick description
        /// </summary>
        public override string Description 
        {
            get
            {
                return "IE6 Browser chrome provider for Windows XP";
            }
        }

        /// <summary>
        /// Gets the address bar parts for IE6
        /// </summary>
        public override IDictionary<string, AutomationElement> GetAddressBarParts(AutomationElement RootIEAutomationElement)
        {            
            AutomationElement addressbar = null;
            if (RootIEAutomationElement == null)
            {
                throw new ArgumentException("Automation handle passed in for IE window was invalid");
            }
            PropertyCondition conds = new PropertyCondition(AutomationElement.LocalizedControlTypeProperty,
                                        "Rebar Band");
            AutomationElementCollection rebar_coll = RootIEAutomationElement.FindAll(
                TreeScope.Descendants | TreeScope.Element,
                conds);
            if (rebar_coll.Count < 1)
            {
                // no rebar found -- a preliminary check
                throw new ApplicationException("No rebars visible, the Addressbar isn't visible.");
            }

            // the address bar contains a Combobox and the Go button
            // note if there were odd things like google toolbar, etc. the logic might 
            // not work and return the wrong toolbar

            AutomationElement ae_AddressBarComboBox = null, ae_AddressBarComboBox_41477 = null, ae_goButton = null;

            foreach (AutomationElement elem in rebar_coll)
            {
                ae_AddressBarComboBox =
                        BrowserHelper.FindFirstSubElement(elem, "ComboBox", AutomationElement.ClassNameProperty);

                ae_AddressBarComboBox_41477 =
                        BrowserHelper.FindFirstSubElement(elem, "41477", AutomationElement.AutomationIdProperty);

                ae_goButton =
                        BrowserHelper.FindFirstSubElement(elem, "ToolbarWindow32", AutomationElement.ClassNameProperty);
                if (
                    ae_AddressBarComboBox != null
                    && ae_goButton != null
                    && ae_AddressBarComboBox_41477 != null // using Automation Id
                    )
                {
                    addressbar = elem;
                    break;
                }
            }

            if (ae_AddressBarComboBox != ae_AddressBarComboBox_41477 || ae_AddressBarComboBox == null)
            {
                // if the two computed addressbar comboboxes are not equal, or if they are equal and both are null
                // then we haven't found the address bar. This points to Automation Id method being unreliable
                throw new NotSupportedException(
                    "Addressbar combo box computed from Automation Id and address " +
                    " bar combobox computed from probing child element of AddressBar Toolbar was different");
            }

            AutomationElement ae_AddressBarEditBox;
            if ((ae_AddressBarEditBox =
                BrowserHelper.FindFirstSubElement(ae_AddressBarComboBox, "Edit", AutomationElement.ClassNameProperty)) == null)
            {
                throw new ApplicationException("Could not find the Edit address bar control within the Address bar");
            }

            Dictionary<string, AutomationElement> addressBarElements = new Dictionary<string, AutomationElement>();
            addressBarElements["GoButton"] = ae_goButton;
            addressBarElements["AddressBarEditBox"] = ae_AddressBarEditBox;

            return addressBarElements;
        }

        /// <summary>
        /// </summary>
        public override AutomationElement GetBackForwardButtonContainer(AutomationElement RootIEAutomationElement)
        {
            return GetStandardButtonToolbar(RootIEAutomationElement);
        }

        public override void InvokeDropdownGoBack(AutomationElement RootIEAutomationElement, int dropdownIndex)
        {
            AutomationElement backButton = GetBackButton(RootIEAutomationElement);
            ClickDropDown(backButton);
            AutomationElementCollection chevronItems = GetDesktopPopupItems();
            InvokeIEDropdownItem(chevronItems, dropdownIndex);
        }

        public override void InvokeDropdownGoForward(AutomationElement RootIEAutomationElement, int dropdownIndex)
        {
            AutomationElement forwardButton = GetForwardButton(RootIEAutomationElement);
            ClickDropDown(forwardButton);
            AutomationElementCollection chevronItems = GetDesktopPopupItems();
            InvokeIEDropdownItem(chevronItems, dropdownIndex);
        }

        protected void ClickDropDown(AutomationElement button)
        {
            // not able to find any AutomationElement for the chevron - just use 10 pixels left of far end
            // of backButton's bounding rectangle - similar for forward button
            Rect bounds = button.Current.BoundingRectangle;
            double x = bounds.Right - 10;
            double y = bounds.Top + bounds.Height / 2;
            MTI.Input.MoveToAndClick(new Point(x, y));
            Thread.Sleep(10000);
        }

        public override Stack<string> GetBackChevronItems(AutomationElement RootIEAutomationElement)
        {
            AutomationElement backButton = GetBackButton(RootIEAutomationElement);
            if (backButton.Current.IsEnabled)
            {
                return GetChevronItems(backButton);
            }
            return new Stack<string>();
        }

        protected Stack<String> GetChevronItems(AutomationElement button)
        {
            ClickDropDown(button);
            AutomationElementCollection ac = GetDesktopPopupItems();
            Stack<String> items = GetHistoryStack(ac, 0, 10);
            ClickDropDown(button);
            return items;
        }

        public override Stack<string> GetForwardChevronItems(AutomationElement RootIEAutomationElement)
        {
            AutomationElement forwardButton = GetForwardButton(RootIEAutomationElement);
            if (forwardButton.Current.IsEnabled)
            {
                return GetChevronItems(forwardButton);
            }
            return new Stack<string>();
        }

        public override AutomationElement GetRefreshButton(AutomationElement RootIEAutomationElement)
        {
            PropertyCondition nameCondition = new PropertyCondition(AutomationElement.NameProperty, "Refresh");
            AutomationElement toolbar = GetStandardButtonToolbar(RootIEAutomationElement);
            return toolbar.FindFirst(TreeScope.Descendants, nameCondition);
        }

    }
}
