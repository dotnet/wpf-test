// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Abstracts input-related product operations.  The 
 *          intention of abstracting the operations is to reduce the 
 *          effect of future API changes in the product's core level.
 *
 
  
 * Revision:         $Revision: 2 $
 
********************************************************************/
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Microsoft.Test.Win32;
using Microsoft.Test.Threading;

namespace Avalon.Test.CoreUI.Trusted
{
    /// <summary>
    /// Abstracts input-related product operations.
    /// </summary>
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    public class InputHelper
    {
        /// <summary>
        /// Gets the element that has keyboard focus.
        /// </summary>
        public static object GetFocusedElement()
        {
            return InputManager.Current.PrimaryKeyboardDevice.FocusedElement;
        }

        /// <summary>
        /// Creates a Window object.
        /// </summary>
        public static Window CreateWindow()
        {
            return new Window();
        }

        /// <summary>
        /// Makes a Window visible.
        /// </summary>
        public static void ShowWindow(Window window)
        {
            window.Show();
        }

        /// <summary>
        /// Checks focus is within a input sink.
        /// </summary>
        public static bool HasFocusWithin(IKeyboardInputSink sink)
        {
            return sink.HasFocusWithin();
        }

        /// <summary>
        /// Is this event an input report?
        /// </summary>
        /// <param name="e">ID of event.</param>
        /// <returns>True if the event is a report, false otherwise.</returns>
        /// <remarks>
        /// This is meant to standardize the check for reports 
        /// if/when the associated classes become internal.
        /// </remarks>
        public static bool IsInputReport(RoutedEvent e)
        {
            return (e.Name == @"InputReport");
        }

        /// <summary>
        /// Is this event a preview input report?
        /// </summary>
        /// <param name="e">ID of event.</param>
        /// <returns>True if the event is a report, false otherwise.</returns>
        /// <remarks>
        /// This is meant to standardize the check for reports 
        /// if/when the associated classes become internal.
        /// </remarks>
        public static bool IsPreviewInputReport(RoutedEvent e)
        {
            return (e.Name == @"PreviewInputReport");
        }

        /// <summary>
        /// Wait for all input to get processed.
        /// </summary>
        public static void WaitForAllInputEvents()
        {
            do
            {
                DispatcherHelper.DoEventsPastInput();
            }
            while (InputHelper.IsInputPending());
        }

        /// <summary>
        /// Determines if input messages are pending in the current
        /// thread's Win32 message queue.
        /// </summary>
        public static bool IsInputPending()
        {
            int retVal = 0;

            // We need to know if there is any pending input in the Win32
            // queue because we want to only process Avalon "background"
            // items after Win32 input has been processed.
            //
            // Win32 provides the GetQueueStatus API -- but it has a major
            // drawback: it only counts "new" input.  This means that
            // sometimes it could return false, even if there really is input
            // that needs to be processed.  This results in very hard to
            // find bugs.
            //
            // Luckily, Win32 also provides the MsgWaitForMultipleObjectsEx
            // API.  While more awkward to use, this API can return queue
            // status information even if the input is "old".  The various
            // flags we use are:
            //
            // QS_INPUT
            // This represents any pending input - such as mouse moves, or
            // key presses.  It also includes the new GenericInput messages.
            //
            // QS_EVENT
            // This is actually a private flag that represents the various
            // events that can be queued in Win32.  Some of these events
            // can cause input, but Win32 doesn't include them in the
            // QS_INPUT flag.  An example is WM_MOUSELEAVE.
            //
            // QS_POSTMESSAGE
            // If there is already a message in the queue, we need to process
            // it before we can process input.
            //
            // MWMO_INPUTAVAILABLE
            // This flag indicates that any input (new or old) is to be
            // reported.
            //
            retVal = NativeMethods.MsgWaitForMultipleObjectsEx(0, null, 0,
                                                                 NativeConstants.QS_INPUT | 
                                                                 NativeConstants.QS_EVENT,
                                                                 NativeConstants.MWMO_INPUTAVAILABLE);
            return retVal == 0;
        }
    }
}

