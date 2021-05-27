// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// <summary>

// These are the registry helpers for the WebServer class.  WebServer uses the

// registry to alter security zones for the server it provides.

// </summary>




using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Security;

using Microsoft.Win32;

namespace DRT
{
public partial class WebServer
{
/// <summary>
/// These are the registry helpers for the WebServer class.  WebServer uses the
/// registry to alter security zones for the server it provides.
/// </summary>
private static class RegistryHelpers
{
    #region Internal Methods
    //--------------------------------------------------------------------------
    // Internal Methods
    //--------------------------------------------------------------------------

    /// <summary>
    /// Gets the content type from the registry by checking the Content Type under
    /// the extension in the registry, and caches the result for next time.
    /// </summary>
    /// <param name="uri">The file to get the content type for</param>
    /// <returns>Content type (default application/octet-stream)</returns>
    internal static string GetContentType(string fileName)
    {
        string extension = Path.GetExtension(fileName);
        string contentType;
        _contentTypes.TryGetValue(extension, out contentType);

        if (contentType == null)
        {
            using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(extension))
            {
                if (key != null)
                {
                    contentType = key.GetValue("Content Type", null) as string;
                }
                else
                {
                    TestServices.Warning("Unabled to get content type for {0}.", extension);
                }
            }

            if (string.IsNullOrEmpty(contentType))
            {
                contentType = _defaultContentType;
            }

            _contentTypes[extension] = contentType;
        }

        return contentType;
    }

    /// <summary>
    /// Gets the current Internet Explorer security zone for this server.
    /// </summary>
    /// <returns>The current security zone for this server.</returns>
    internal static SecurityZone GetSecurityZone()
    {
        // if the key is missing then there is no zone set
        SecurityZone zone = SecurityZone.NoZone;

        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
            _securityZoneRegKey + @"\" + _serverIpAddress))
        {
            if (key != null)
            {
                // if the registry value is not an int; the inner cast
                // will catch that; the outer will catch an invalid zone
                zone = (SecurityZone)(int)key.GetValue(Uri.UriSchemeHttp);
            }
        }

        return zone;
    }

    /// <summary>
    /// Sets the current Internet Explorer security zone for this server.
    /// </summary>
    /// <param name="zone">The desired security zone for this server.</param>
    internal static void SetSecurityZone(SecurityZone zone)
    {
        // NoZone is to remove the security zone value
        if (zone == SecurityZone.NoZone)
        {
            DeleteSecurityZone();
        }
        else
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(
                _securityZoneRegKey + @"\" + _serverIpAddress))
            {
                if (key != null)
                {
                    key.SetValue(Uri.UriSchemeHttp, (int)zone);
                }
                else
                {
                    TestServices.Warning("Unable to set security zone.");
                }
            }
        }
    }
    #endregion Internal Methods

    #region Private Methods
    //--------------------------------------------------------------------------
    // Private Methods
    //--------------------------------------------------------------------------

    /// <summary>
    /// Removes the current Internet Explorer security zone for this server.
    /// </summary>
    private static void DeleteSecurityZone()
    {
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
            _securityZoneRegKey, true))
        {
            if (key != null)
            {
                key.DeleteSubKey(_serverIpAddress, false);
            }
            else
            {
                TestServices.Warning("Unable to delete security zone.");
            }
        }
    }
    #endregion Private Methods

    #region Private Fields
    //--------------------------------------------------------------------------
    // Private Fields
    //--------------------------------------------------------------------------

    private const string _defaultContentType = "application/octet-stream";
    private const string _securityZoneRegKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap\Domains";

    /// <summary>
    /// A cache of known content types.
    /// </summary>
    /// <remarks>
    /// It is reasonable to make this static because the values are at the machine level.
    /// </remarks>
    private static Dictionary<string, string> _contentTypes = new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    #endregion Private Fields
}
}
}
