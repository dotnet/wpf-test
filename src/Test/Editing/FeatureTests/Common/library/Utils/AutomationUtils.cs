// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 9 $ $Source: //depot/vbl_wcp_avalon_dev/windowstest/client/wcptests/uis/Common/Library/Utils/InputMonitorManager.cs $")]

namespace Test.Uis.Utils
{
    #region Namespaces.

    using System;
    using System.Threading; 
    using System.Windows.Threading;
    using System.Collections;
    using System.Windows.Input;
    using System.Windows;
    
    using System.Diagnostics;
    using System.Windows.Automation;

    #endregion

    /// <summary>
    /// static class having functions for automation elements
    /// </summary>
    public class AutomationUtils
    {
        /// <summary>
        /// Gets the AutomationElement object of a control with specified Name among
        /// descendants of the RootElement.
        /// </summary>
        /// <param name="controlID">Name to look for</param>
        /// <returns>AutomationElement instance.</returns>
        public static AutomationElement GetAutomationElement(string controlID)
        {
            AutomationElement autoElement;  // Automation element found.
            AutomationElement root;         // Automation to start search from.
            PropertyCondition condition;    // Control Name condition.
            if (controlID == null)
            {
                throw new ArgumentNullException("controlID");
            }
            root = AutomationElement.RootElement;
            condition = new PropertyCondition(AutomationElement.AutomationIdProperty, controlID);
            autoElement = root.FindFirst(TreeScope.Descendants, condition);
            return autoElement;
        }

        private static int s_temporaryAutomationIdCounter = 0;

        /// <summary>Gets the AutomationElement object of a control.</summary>
        public static AutomationElement GetAutomationElement(UIElement control)
        {
            AutomationElement autoElement;
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }
            string automationId = AutomationProperties.GetAutomationId(control);
            bool automationIdNotSet = automationId == null || automationId == string.Empty;

            if (automationIdNotSet)
            {
                automationId = "_temporaryAutomationId_" + (s_temporaryAutomationIdCounter++);
                AutomationProperties.SetAutomationId(control, automationId);
            }

            autoElement = GetAutomationElement(automationId);

            if (automationIdNotSet)
            {
                AutomationProperties.SetAutomationId(control, string.Empty);
            }
            return autoElement;
        }

        /// <summary>Returns a flag indicating whether the specified AutomationElement supports TextPattern or not.</summary>
        public static bool IsTextPatternAvailable(AutomationElement e)
        {
            object _patternObject;
            e.TryGetCurrentPattern(TextPattern.Pattern, out _patternObject);
            object pattern = _patternObject;
            if (pattern != null && pattern is TextPattern)
                return true;
            else
                return false;
        }
    }
}
