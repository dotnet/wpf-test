// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.Trusted {

    /// <summary>
    ///     Cursor specific implementation of a SafeHandle for testers
    /// </summary>
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    public class TestCursorSafeHandle: SafeHandle
    {
        #region Constructors

        /// <summary>
        ///     Constructs a TestCursorSafeHandle from a native Win32 cursor
        /// </summary>
        /// <param name="handle">
        ///     Handle to native Win32 Cursor
        /// </param>
        /// <remarks>
        ///     Defaults "ownHandle" param of SafeHandle to "true"
        /// </remarks>
        public TestCursorSafeHandle(IntPtr handle)
            : base(handle, true)
        {
        }

        /// <summary>
        ///     Constructs a TestCursorSafeHandle from a native Win32 cursor
        /// </summary>
        /// <param name="handle">
        ///     Handle to native Win32 Cursor
        /// </param>
        /// <param name="ownHandle">
        ///     Specifies ownership of native Win32 Cursor
        /// </param>
        public TestCursorSafeHandle(IntPtr handle, bool ownHandle)
            : base(handle, ownHandle)
        {
        }

        #endregion Constructors


        #region Public Methods

        // Do not provide a finalizer - SafeHandle's critical finalizer will
        // call ReleaseHandle for you.
        /// <summary>
        ///     Determines if the native Win32 handle is valid for a Cursor
        /// </summary>
        public override bool IsInvalid
        {
            get
            {
                return IsClosed || handle == IntPtr.Zero;
            }
        }

        #endregion Public Methods


        #region Protected Methods

        /// <summary>
        ///     Releases the handle in a Cursor specific manner, via DestroyCursor
        /// </summary>
        /// <returns></returns>
        override protected bool ReleaseHandle()
        {
            return NativeMethods.DestroyCursor(handle);
        }
        
        #endregion Protected Methods
    }
}
