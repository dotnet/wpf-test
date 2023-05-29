// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Generalize the code InputManager class.
 * 
 * Contributor: Microsoft
 *
 
  
 * Revision:         $Revision: 2 $
 
********************************************************************/
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Test.Serialization;

namespace Avalon.Test.CoreUI.Trusted
{
    /// <summary>
    ///     The InputManagerHelper is a wrapper around Avalon Input Manager.
    /// </summary>
    public sealed class InputManagerHelper
    {
        /// <summary>
        ///     Return the input manager associated with the current context.
        /// </summary>
        public InputManagerHelper(InputManager inputManager)
        {
            _inputManager = inputManager;
        }

        private InputManager _inputManager;

        /// <summary>
        ///     Return the input manager associated with the current context.
        /// </summary>
        public static InputManager Current
        {
            get
            {
                return InputManager.Current;
            }
        }


        /// <summary>
        ///     Return the input manager associated with the current context.
        /// </summary>
        public static InputManagerHelper CurrentHelper
        {
            get
            {
                return new InputManagerHelper(InputManagerHelper.Current);
            }
        }
        /// <summary>PreProcessInput</summary>
        /// <SecurityNote>
        ///     Critical: This event lets people subscribe to all events in the system
        ///     PublicOk: Method is link demanded. 
        /// </SecurityNote>
        public event PreProcessInputEventHandler PreProcessInput
        {
            add
            {
                _inputManager.PreProcessInput += value;
            }
            remove
            {
                _inputManager.PreProcessInput -= value;
            }
        }


        /// <summary>PreNotifyInput</summary>
        public event NotifyInputEventHandler PreNotifyInput
        {
            add
            {
                _inputManager.PreNotifyInput += value;
            }
            remove
            {
                _inputManager.PreNotifyInput -= value;
            }

        }
        /// <summary>PostNotifyInput</summary>
        public event NotifyInputEventHandler PostNotifyInput
        {
            add
            {
                _inputManager.PostNotifyInput += value;
            }
            remove
            {
                _inputManager.PostNotifyInput -= value;

            }
        }

        /// <summary>PostProcessInput</summary>
        public event ProcessInputEventHandler PostProcessInput
        {
            add
            {
                _inputManager.PostProcessInput += value;
            }
            remove
            {
                _inputManager.PostProcessInput -= value;
            }
        }

        /// <summary>
        /// InputReportEvent
        /// </summary>
        public static RoutedEvent InputReportEvent
        {
            [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
            get
            {
                FieldInfo info = typeof(InputManager).GetField("InputReportEvent", s_fieldBindFlags);
                return ((RoutedEvent)(info.GetValue(null)));
            }
        }

        /// <summary>
        /// PreviewInputReportEvent
        /// </summary>
        public static RoutedEvent PreviewInputReportEvent
        {
            [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
            get
            {
                FieldInfo info = typeof(InputManager).GetField("PreviewInputReportEvent", s_fieldBindFlags);
                return ((RoutedEvent)(info.GetValue(null)));
            }
        }

        /// <summary>
        /// Default flags to reflect into fields.
        /// </summary>
        private static BindingFlags s_fieldBindFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetField;
    }
}

