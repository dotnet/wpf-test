// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Automation;
using System.Collections.Generic;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{

    /// <summary>
    /// interface for Platform specific browser ui operation
    /// </summary>
    public interface IBrowserUIProvider
    {
        /// <summary>
        /// Must return a Dictionary of AutomationElements with key "AddressBarEditBox" and "GoButton"
        /// </summary>
        /// <returns>Dictionary of AutomationElements with only the keys "AddressBarEditBox" and "GoButton"</returns>
        IDictionary<string, AutomationElement> GetAddressBarParts(AutomationElement RootIEAutomationElement);

        AutomationElement GetStandardButtonToolbar(AutomationElement RootIEAutomationElement);

        /// <summary>
        /// This separate method is needed because in Longhorn, the back-forward button is no longer located in the
        /// standard toolbar.
        /// In IE6 on XP, this will return the standard toolbar itself
        /// </summary>
        /// <param name="RootIEAutomationElement"></param>
        /// <returns>Automation element of UI widget containing the back-forward buttons</returns>
        AutomationElement GetBackForwardButtonContainer (AutomationElement RootIEAutomationElement);

        AutomationElement GetMenuBar(AutomationElement RootIEAutomationElement);

        AutomationElement GetStatusBar(AutomationElement RootIEAutomationElement);

        string GetStatusText(AutomationElement RootIEAutomationElement);

        string GetStatusZoneInfoText(AutomationElement RootIEAutomationElement);

        string Description { get;}

        void InvokeGoBack(AutomationElement RootIEAutomationElement);
        void InvokeGoForward(AutomationElement RootIEAutomationElement);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RootIEAutomationElement"></param>
        /// <param name="dropdownIndex"> Index range 1-9 </param>

        AutomationElement GetBackButton(AutomationElement RootIEAutomationElement);
        AutomationElement GetForwardButton(AutomationElement RootIEAutomationElement);
        AutomationElement GetRefreshButton(AutomationElement RootIEAutomationElement);
        AutomationElement GetStopLoadingButton(AutomationElement RootIEAutomationElement);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RootIEAutomationElement"></param>
        /// <param name="dropdownIndex">Expect input between 1-9 (9 being topmost; 1 being bottommost)</param>
        void InvokeDropdownGoBack(AutomationElement RootIEAutomationElement, int dropdownIndex);
        void InvokeDropdownGoForward(AutomationElement RootIEAutomationElement, int dropdownIndex);
        void InvokeRefresh(AutomationElement RootIEAutomationElement);
        Stack<String> GetBackChevronItems(AutomationElement RootIEAutomationElement);
        Stack<String> GetForwardChevronItems(AutomationElement RootIEAutomationElement);
        void NavigateByAddressBar(AutomationElement RootIEAutomationElement, String uri);
        void Minimize(AutomationElement RootIEAutomationElement);
        void Maximize(AutomationElement RootIEAutomationElement);
        void Close(AutomationElement RootIEAutomationElement);
    }
}
