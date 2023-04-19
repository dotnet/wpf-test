// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Interop;
using Microsoft.Test;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using System.Windows.Controls;
using System.Xml;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // [Serializable]
    /// <summary>
    /// The NavigationState class keeps track of the NavigationWindow title, the state (disabled/enabled)
    /// of the back/forward journal buttons, the contents of the back/forward stacks, and a description
    /// of what caused us to reach that state.  The class also provides API to:
    ///    - save the IProvideJournalingState's current conditions into a NavigationState object
    ///    - compare one NavigationState to another
    /// </summary>
    public class NavigationState
    {
        public String stateDescription = String.Empty;
        public String windowTitle = String.Empty;
        public bool backButtonEnabled = false;
        public bool forwardButtonEnabled = false;
        public String[] backStack;
        public String[] forwardStack;

        public enum StackType { Back, Forward };

        /// <summary>
        /// Empty Constructor
        /// </summary>
        public NavigationState()
        {
        }

        /// <summary>
        /// Set the state of the NavigationState object
        /// </summary>
        /// <param name="stateDescription">Description</param>
        /// <param name="windowTitle">Window Title</param>
        /// <param name="backButtonEnabled">is back button enabled</param>
        /// <param name="forwardButtonEnabled">is forward button enabled</param>
        /// <param name="backStack">backStack</param>
        /// <param name="forwardStack">forwardStack</param>
        public NavigationState(String stateDescription, String windowTitle,
            bool backButtonEnabled, bool forwardButtonEnabled,
            String[] backStack, String[] forwardStack)
        {
            this.stateDescription = stateDescription;
            this.windowTitle = windowTitle;
            this.backButtonEnabled = backButtonEnabled;
            this.forwardButtonEnabled = forwardButtonEnabled;
            this.backStack = backStack;
            this.forwardStack = forwardStack; 
        }

        /// <summary>
        /// Saves the state of the journal buttons, the contents of the journal stack, the window title into
        /// a NavigationState object.
        /// </summary>
        /// <param name="navProvider">Usually a JournalHelper object that keeps track of journal state</param>
        public void RecordNavigationState(IProvideJournalingState navProvider)
        {
            if (navProvider != null)
            {
                backButtonEnabled = navProvider.IsBackEnabled();
                forwardButtonEnabled = navProvider.IsForwardEnabled();
                windowTitle = navProvider.WindowTitle;
                backStack = navProvider.GetBackMenuItems();
                forwardStack = navProvider.GetForwardMenuItems();
            }
        }

        /// <summary>
        /// Saves the state of the journal buttons, the contents of the journal stack, the window title into
        /// a NavigationState object.
        /// </summary>
        /// <param name="navProvider">Usually a JournalHelper object that keeps track of journal state</param>
        /// <param name="contentControl">Content (Frame) related to that journal is recorded</param>
        public void RecordNavigationState(IProvideJournalingState navProvider, ContentControl contentControl)
        {
            if (navProvider != null)
            {
                backButtonEnabled = navProvider.IsBackEnabled(contentControl);
                forwardButtonEnabled = navProvider.IsForwardEnabled(contentControl);
                windowTitle = navProvider.WindowTitle;
                backStack = navProvider.GetBackMenuItems(contentControl);
                forwardStack = navProvider.GetForwardMenuItems(contentControl);
            }
        }

        /// <summary>
        /// Saves the state of the journal buttons, the contents of the journal stack, the window title and 
        /// a description of the state/action into a NavigationState object.
        /// </summary>
        /// <param name="navProvider">Usually a JournalHelper object that keeps track of journal state</param>
        /// <param name="description">String describing the action we just saved NavigationState for</param>
        public void RecordNavigationState(IProvideJournalingState navProvider, String description)
        {
            if (navProvider != null)
            {
                Log.Current.CurrentVariation.LogMessage("Recording NavigationState: " + description);
                stateDescription = description;
                RecordNavigationState(navProvider);
            }
        }

        /// <summary>
        /// Saves the state of the journal buttons, the contents of the journal stack, the window title and 
        /// a description of the state/action into a NavigationState object.
        /// </summary>
        /// <param name="navProvider">Usually a JournalHelper object that keeps track of journal state</param>
        /// <param name="contentControl">Content (Frame) related to that journal is recorded</param>
        /// <param name="description">String describing the action we just saved NavigationState for</param>
        public void RecordNavigationState(IProvideJournalingState navProvider, ContentControl contentControl, String description)
        {
            if (navProvider != null)
            {
                Log.Current.CurrentVariation.LogMessage("Recording NavigationState: " + description);
                stateDescription = description;
                RecordNavigationState(navProvider, contentControl);
            }
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current NavigationState instance.
        /// </summary>
        /// <param name="obj">The second NavigationState object to compare to</param>
        /// <returns>true if all the properties match the expected values, false otherwise</returns>
        public override bool Equals(object obj)
        {
            NavigationState state = obj as NavigationState;
            if (state == null)
                return false;

            // Warn if any of our atomic criteria do not match
            if (backButtonEnabled != state.backButtonEnabled)
                Log.Current.CurrentVariation.LogMessage("backButtonEnabled is not equal. EXPECTED: " + state.backButtonEnabled + "; ACTUAL: " + this.backButtonEnabled);
            if (forwardButtonEnabled != state.forwardButtonEnabled)
                Log.Current.CurrentVariation.LogMessage("forwardButtonEnabled is not equal. EXPECTED: " + state.forwardButtonEnabled + "; ACTUAL: " + this.forwardButtonEnabled);
            if (!(windowTitle.Equals(state.windowTitle)))
                Log.Current.CurrentVariation.LogMessage("windowTitle is not equal. EXPECTED: " + state.windowTitle + "; ACTUAL: " + this.windowTitle);

            // Test consideration: We need to relax checking for state equality in some cases.
            // Reason: NavigationWindow.CanGoBack and NavigationWindow.CanGoForward are not consistent for some events in IE6 compared to IE7.
           
            // Note: For FragmentNavigation and LoadCompleted events, this inconsistency is not a critical test failure.
            // If this failure is happening within a Navigated event, it is a severe problem and requires a product fix.

            // Is the current state browser-dependent and associated with one of our problematic events?
            bool isBrowserDependentEventState =
                BrowserInteropHelper.IsBrowserHosted && (
                    (stateDescription.IndexOf(@"FragmentNavigation", StringComparison.InvariantCultureIgnoreCase) > -1) ||
                    (stateDescription.IndexOf(@"LoadCompleted", StringComparison.InvariantCultureIgnoreCase) > -1)
                );

            // Does supplied state match our current state?
            bool isEqual = (
                // Back and forward buttons match (test not applicable if this is a browser-dependent event state)
                (isBrowserDependentEventState ?
                    true :
                    ((backButtonEnabled == state.backButtonEnabled) && (forwardButtonEnabled == state.forwardButtonEnabled))) &&
                // Window titles match
                windowTitle.Equals(state.windowTitle) && 
                // Back and forward stacks match
                IsSameStack(StackType.Back, backStack, state.backStack) && 
                IsSameStack(StackType.Forward, forwardStack, state.forwardStack)
            );

            NavigationHelper.CacheTestResult((isEqual) ? Result.Pass : Result.Fail);

            return isEqual;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Represent NavigationState as string.
        /// </summary>
        /// <returns>String representing current state of the object.</returns>
        public override string ToString()
        {
            return (String.Format(
                CultureInfo.InvariantCulture,
                "Description={0}, BackButtonEnabled={1}, ForwardButtonEnabled={2}, WindowTitle={3}",
                String.IsNullOrEmpty(stateDescription) ? "(null)" : stateDescription,
                backButtonEnabled.ToString(CultureInfo.InvariantCulture),
                forwardButtonEnabled.ToString(CultureInfo.InvariantCulture),
                String.IsNullOrEmpty(windowTitle) ? "(null)" : windowTitle));
        }

        /// <summary>
        /// Compares the contents of 2 journal stacks (back/forward)
        /// </summary>
        /// <param name="a">First journal stack to compare</param>
        /// <param name="b">Second journal stack to compare</param>
        /// <returns></returns>
        private static bool IsSameStack(StackType stackType, String[] a, String[] b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null && b.Length == 0)
            {
                // actual journal stacks(a) are returned from JournalHelper.GetBackMenuItems() and
                // JournalHelper.GetForwardMenuItems() that can be null in some cases
                // Make sure the expected stack length is zero in this case for validation to pass
                return true;
            }

            if (b == null && a.Length > 0)
            {
                Log.Current.CurrentVariation.LogMessage("Found expected stack null but actual stack length is not zero");
                return false;
            }

            if (a.Length != b.Length)
            {
                StringBuilder sb = new StringBuilder();
                if (stackType == StackType.Back)
                {
                    sb.Append("Journal entry count: Back Stack NO MATCH. (Actual = ");
                }
                else
                {
                    sb.Append("Journal entry count: Forward Stack NO MATCH. (Actual = ");
                }

                sb.Append(a.Length.ToString());
                sb.Append(" Expected = ");
                sb.Append(b.Length.ToString());
                sb.Append(")");
                Log.Current.CurrentVariation.LogMessage(sb.ToString());
                return false;
            }
            bool match = true;
            for (int i = 0; i < a.Length; i++)
            {
                if (!a[i].Equals(b[i], StringComparison.InvariantCultureIgnoreCase))
                {
                    Log.Current.CurrentVariation.LogMessage("Journal entry #" + i + " : NO MATCH. (" + a[i] + " vs. " + b[i] + ")");
                    match = false;
                    break;
                }
            }
            return match;
        }

    }
}
