// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security;

namespace Avalon.Test.Win32
{
    /// <summary>
    /// Unmanaged Interop helper functions
    /// </summary>
    [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
    public sealed class Interop
    {
        /// <summary />
        public static unsafe string StringFromCharPtr(IntPtr ptr)
        {
            return new string((char*)ptr);
        }

        /// <summary />
        public static unsafe string StringFromSBytePtr(IntPtr ptr)
        {
            return new string((sbyte*)ptr);
        }
        
        /// <summary>
        /// StringFromSecureString
        /// </summary>
        /// <param name="sStr"></param>
        /// <returns></returns>
        public static unsafe string StringFromSecureString(System.Security.SecureString sStr)
        {
            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(sStr);
            return new string((char*)ptr);
        }

        /// <summary>
        /// hMemStringToHGlobalAnsi
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static IntPtr hMemStringToHGlobalAnsi(string str)
        {
            return System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(str);
        }

        /// <summary>
        /// hMemStringToHGlobalAuto
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static IntPtr hMemStringToHGlobalAuto(string str)
        {
            return System.Runtime.InteropServices.Marshal.StringToHGlobalAuto(str);
        }

        /// <summary>
        /// hMemStringToHGlobalUni
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static IntPtr hMemStringToHGlobalUni(string str)
        {
            return System.Runtime.InteropServices.Marshal.StringToHGlobalUni(str);
        }
        
        /// <summary>
        /// LaunchAProcess
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public static System.Diagnostics.Process LaunchAProcess(string processName, string Parameters)
        {
            return System.Diagnostics.Process.Start(processName, Parameters);
        }
        
        /// <summary>
        /// ProcessWait
        /// </summary>
        /// <param name="process"></param>
        /// <param name="milliseconds"></param>
        public static void ProcessWait(System.Diagnostics.Process process, int milliseconds)
        {
            process.WaitForExit(milliseconds);
        }
        
        /// <summary>
        /// ProcessExit
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static bool ProcessExit(System.Diagnostics.Process process)
        {
            return process.HasExited;
        }
    }
}
