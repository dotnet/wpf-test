// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Win32;

namespace MS.Internal
{
    //=========================================================================
    /// <summary>
    /// SNKey: An internal static helper class to handle the strong name 
    ///        validation for the private wpf package.
    /// </summary>
    internal static class SNKey 
    {
        // Create StrongName skipVerification registry key for private wpf package.
        internal static void Create(HostCPU cpu)
        {
            RegistryKey regWPFSNKey;

            regWPFSNKey = Registry.LocalMachine.CreateSubKey(s_SNKeyRegPath);
            regWPFSNKey.Close();

            if (cpu == HostCPU.x64)
            {
                // Create the strong name key for WOW supoort.
                regWPFSNKey = Registry.LocalMachine.CreateSubKey(s_SNKeyWowRegPath);
                regWPFSNKey.Close();
            }
        }


        // Remove the StrongName SkipVerification registry for private WPF package.
        internal static void Remove(HostCPU cpu)
        {
            Registry.LocalMachine.DeleteSubKey(s_SNKeyRegPath, false);

            if (cpu == HostCPU.x64)
            {
                // Remove the strong name key for WOW support.
                Registry.LocalMachine.DeleteSubKey(s_SNKeyWowRegPath, false);
            }
        }

        private static string s_SNKeyRegPath = @"SOFTWARE\Microsoft\StrongName\Verification\*," + "_WCP_PUBLIC_KEY_TOKEN_";
        private static string s_SNKeyWowRegPath = @"SOFTWARE\Wow6432Node\Microsoft\StrongName\Verification\*," + "_WCP_PUBLIC_KEY_TOKEN_";
       
    }
}
