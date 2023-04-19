// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.Threading; 
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders;
using Microsoft.Test.Input;
using MTI = Microsoft.Test.Input;
using System.Security;
using System.Security.Policy;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Reflection;
using System.Windows.Automation;

/*****************************************************
 * The logic in this file is maintained by the AppSec team
 * contact: Microsoft
 *****************************************************/

namespace Microsoft.Test.Deployment 
{

    /// <summary>
    /// Helper functions for handling deployment of avalon applications
    /// </summary>
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    public static class UnmanagedStringHelper 
    {

        #region Public Members   

        #region String Resource Helper functions

        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern Int32 SHLoadIndirectString(String source, StringBuilder outBuffer, Int32 outBufferSize, IntPtr reserved);

        /// <summary>
        /// Loads an unmanaged resource string, using the indices given by tools like ResSpy/TokenMapper.  Returns null if not found.
        /// </summary>
        /// <param name="dllPath">Path to dll to load resource from</param>
        /// <param name="StringID">StringID value from ResSpy/TokenMapper</param>
        /// <param name="SubID">SubID value from ResSpy/TokenMapper</param>
        /// <returns></returns>
        public static string LoadUnmanagedResourceString(string dllPath, int StringID, int SubID)
        {
            int actualID = (((StringID - 1) * 16) + SubID);
            return LoadUnmanagedResourceString(dllPath + ",-" + actualID.ToString());
        }

        /// <summary>
        /// Loads an unmanaged resource string, using the full string needed by SHLoadIndirectString.  Returns null if not found.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string LoadUnmanagedResourceString(string path)
        {            
            if (Directory.Exists(Environment.GetEnvironmentVariable("SystemRoot") + "\\SysWow64"))
            {
                path = path.ToLowerInvariant().Replace("system32", "syswow64");
                #if (!STRESS_RUNTIME)
                GlobalLog.LogDebug("Updated resource load path to: " + path);
                #endif
            }

            StringBuilder loadedStringBuilder = new StringBuilder(260);
            if (SHLoadIndirectString(path, loadedStringBuilder, loadedStringBuilder.Capacity, IntPtr.Zero) == 0x00000000)
            {
                return loadedStringBuilder.ToString();
            }
            else
            {
                return null;
            }
        }
        #endregion


        #endregion
    }
}
