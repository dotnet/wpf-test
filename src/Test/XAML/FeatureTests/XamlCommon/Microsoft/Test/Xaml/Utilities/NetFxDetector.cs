// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32;

namespace Microsoft.Test.Xaml.Utilities
{
    /// <summary>
    /// Utility class to detect .Net framework installations
    /// </summary>
    public static class NetFxDetector
    {
        /// <summary>
        /// Check if .Net 3.5 is installed on the box by looking at the 
        /// registry key Software\\Microsoft\\NET Framework Setup\\NDP\\v3.5
        /// </summary>
        /// <returns>true if .Net 3.5 is installed</returns>
        public static bool IsNet35Installed()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\NET Framework Setup\\NDP\\v3.5");
            return key != null;
        }
    }
}
