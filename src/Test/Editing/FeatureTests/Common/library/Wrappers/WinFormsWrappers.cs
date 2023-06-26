// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides wrappers for WinForm types.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 12 $ $Source: //depot/vbl_wcp_avalon_dev/windowstest/client/wcptests/uis/Common/Library/Utils/InputGenerator.cs $")]

namespace Test.Uis.Wrappers
{
    #region Namespaces.

    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    #endregion Namespaces.

    /// <summary>
    /// Provides wrappers for common WinForms APIs.
    /// </summary>
    /// <remarks>
    /// Unlike other classes, methods will assert permissions
    /// and then call a private method that requires the
    /// permissions. This design was adopted to make the
    /// WinForms APIs work seamlessly from Avalon test cases.
    /// </remarks>
    public static class WinForms
    {
        #region Public methods.

        /// <summary>Gets the Clipboard data.</summary>
        /// <returns>The data on the system Clipboard.</returns>
        public static IDataObject ClipboardGetDataObject()
        {
            new System.Security.Permissions.UIPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return System.Windows.Forms.Clipboard.GetDataObject();
        }

        /// <summary>Gets the data object as a string.</summary>
        /// <returns>
        /// The clipboard contents as a string, or null if there is no
        /// data or no conversion is possible.
        /// </returns>
        public static string ClipboardGetDataObjectAsString()
        {
            System.Windows.Forms.IDataObject data;
                
            new System.Security.Permissions.UIPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            data = System.Windows.Forms.Clipboard.GetDataObject();
            return (data == null)? null : (string) data.GetData(typeof(string));
        }

        /// <summary>
        /// Gets the formats for the clipboard data object.
        /// </summary>
        /// <returns>
        /// The formats for the clipboard data object., or null if there is no
        /// data or no conversion is possible.
        /// </returns>
        public static string[] ClipboardGetDataObjectFormats()
        {
            new System.Security.Permissions.UIPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return System.Windows.Forms.Clipboard.GetDataObject().GetFormats();
        }

        /// <summary>
        /// Sets the data specified data object on the clipboard.
        /// </summary>
        /// <param name='dataObject'>Object to set on clipboard.</param>
        public static void ClipboardSetDataObject(object dataObject)
        {
            new System.Security.Permissions.UIPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            System.Windows.Forms.Clipboard.SetDataObject(dataObject);
        }

        #endregion Public methods.
    }
}
