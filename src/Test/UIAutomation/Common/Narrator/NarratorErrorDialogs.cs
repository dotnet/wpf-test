// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Automation;
using System.Diagnostics;

namespace Microsoft.Test
{
    class NarratorErrorDialogs
    {
        #region variables

        /// -------------------------------------------------------------------
        /// <summary>Event listener that will watch for the dialogs</summary>
        /// -------------------------------------------------------------------
        static AutomationEventHandler s_handler = null;

        /// -------------------------------------------------------------------
        /// <summary>Thread for watching for events</summary>
        /// -------------------------------------------------------------------
        static Thread s_thread = null;

        #endregion variables

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        ~NarratorErrorDialogs()
        {
            Trace.WriteLine("Removing window handler for the language error dialog");
            if (NarratorErrorDialogs.s_handler != null)
                Automation.RemoveAllEventHandlers();

        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public void HandleVoiceLanguageWarningForm()
        {
            s_thread = new Thread(VoiceLanguageWarningForm);
            s_thread.Start();
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        static void VoiceLanguageWarningForm()
        {
            Trace.WriteLine("Setting up window handler for the language error dialog");
            s_handler = new AutomationEventHandler(WindowOpenedEventHandler);
            Automation.AddAutomationEventHandler(WindowPattern.WindowOpenedEvent, AutomationElement.RootElement, TreeScope.Subtree, s_handler);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        static public void WindowOpenedEventHandler(object obj, AutomationEventArgs argument)
        {
            try
            {
                if (obj is AutomationElement)
                {
                    AutomationElement element = (AutomationElement)obj;
                    Trace.WriteLine("Window Opened: \"" + element.Current.Name + "\" with AutomationId({" + element.Current.AutomationId + "})");
                    switch (element.Current.AutomationId)
                    {
                        case "VoiceLanguageWarningForm":
                            // I know this UIAutomationID supports WindowPattern...
                            ((WindowPattern)element.GetCurrentPattern(WindowPattern.Pattern)).Close();
                            s_thread.Abort();
                            break;

                    }
                }
            }
            catch (Exception exception)
            {
                // Just eat the exception
                Trace.WriteLine("Exception thrown (" + exception.GetType().ToString() + ") - " + exception.Message + "\"");
            }
        }
    }
}
