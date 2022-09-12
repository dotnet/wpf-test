// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//***************************************************************************
// HOW TO USE THIS FILE
//
// If you need access to a Win32 API that is not exposed, simply uncomment
// it in one of the following files:
// 
// NativeMethods.cs
// UnsafeNativeMethods.cs
// SafeNativeMethods.cs
//
// Only uncomment what you need to avoid code bloat.
//
// DO NOT adjust the visibility of anything in these files.  They are marked
// internal on pupose.
//***************************************************************************

using System.Threading;


namespace MS.Win32
{
    using Accessibility;
    using System.Runtime.InteropServices;
    using System;
    using System.Security.Permissions;
    using System.Collections;
    using System.IO;
    using System.Text;
    using System.Security;

    [SuppressUnmanagedCodeSecurity()]
    internal class UnsafeNativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool SystemParametersInfo(int nAction, int nParam, [In, Out] NativeMethods.NONCLIENTMETRICS metrics, int nUpdate);
    }
}

