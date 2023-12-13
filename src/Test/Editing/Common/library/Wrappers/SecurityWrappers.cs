// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides wrappers for APIs that assert the appropriate permissions to succeed in partial trust.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Wrappers/SecurityWrappers.cs $")]

namespace Test.Uis.Wrappers
{
    #region Namespaces.

    using System;
    using System.Security;
    using System.Security.Permissions;
    using System.Windows;

    #endregion Namespaces.

    /// <summary>Provides helper methods and properties to security wrappers.</summary>
    public static class SecurityHelper
    {
        #region Public methods.

        /// <summary>
        /// Determines whether the current stack is fully trusted.
        /// </summary>
        /// <returns>true if the current stack is fully trusted; false otherwise.</returns>
        /// <remarks>
        /// Test cases should cache this value to avoid duplicated first-time 
        /// exceptions.
        /// </remarks>
        public static bool GetIsStackFullyTrusted()
        {
            try
            {
                new System.Security.Permissions.SecurityPermission(
                    System.Security.Permissions.PermissionState.Unrestricted)
                    .Demand();
                return true;
            }
            catch(System.Security.SecurityException)
            {
                return false;
            }
        }

        #endregion Public methods.

        #region Internal properties.

        /// <summary>Permission to access all clipboard functionality.</summary>
        internal static CodeAccessPermission AllClipboardPermission
        {
            get { return new UIPermission(UIPermissionClipboard.AllClipboard); }
        }

        /// <summary>Permission from SecurityPermission with all flags set.</summary>
        internal static CodeAccessPermission AllSecurityPermission
        {
            get { return new SecurityPermission(SecurityPermissionFlag.AllFlags); }
        }

        #endregion Internal properties.

        #region Internal methods.

        /// <summary>Creates a permission set that is the union of all supplied permissions.</summary>
        internal static PermissionSet Union(params CodeAccessPermission[] permissions)
        {
            PermissionSet result;

            result = new PermissionSet(PermissionState.Unrestricted);
            foreach(IPermission permission in permissions)
            {
                result.AddPermission(permission);
            }
            return result;
        }

        #endregion Internal methods.
    }

    /// <summary>This class provides full access to System.Windows.Clipboard.</summary>
    public static class ClipboardWrapper
    {
        #region Public methods.

        /// <summary>Gets the current clipboard data.</summary>
        /// <returns>The current clipboard data object.</returns>
        public static IDataObject GetDataObject()
        {
            SecurityHelper.AllClipboardPermission.Assert();
            return Clipboard.GetDataObject();
        }

        #endregion Public methods.
    }

    /// <summary>
    /// This class provides full access to 
    /// System.Windows.DataObject and System.Windows.IDataObject.
    /// </summary>
    public static class DataObjectWrapper
    {
        #region Public methods.

        /// <summary>Gets the current clipboard data.</summary>
        /// <returns>The current clipboard data object.</returns>
        public static string[] GetFormats(IDataObject dataObject)
        {
            SecurityHelper.Union(
                SecurityHelper.AllClipboardPermission,
                SecurityHelper.AllSecurityPermission).Assert();
            return dataObject.GetFormats();
        }

        /// <SecurityNote>
        ///     Crtitical: This asserts for UIpermission
        ///     PublicOK: This is a test case
        /// </SecurityNote>
        /// <returns></returns>
        [SecurityCritical]
        public static DataObject CreateXamlDataObject()
        {
            DataObject result;
            (new UIPermission(UIPermissionClipboard.AllClipboard)).Assert();
            try
            {
                result = new DataObject();
                result.SetData(DataFormats.Xaml, @"<Xaml Text />");
            }
            finally
            {
                UIPermission.RevertAssert();
            }

            return result;
        }

        #endregion Public methods.
    }

    /// <summary>Dummy IDataObject implementation.</summary>
    /// <remarks>This data object supports a single UnicodeText value.</remarks>
    /// <SecurityNote>
    ///     Critical: This code subclasses data object which is not allowed in partial trust
    ///     PublicOK: This is a test case
    /// </SecurityNote>
    [SecurityCritical(SecurityCriticalScope.Everything)]
    class CustomIDataObject : IDataObject
    {
        #region IDataObject Members

        public object GetData(string format, bool autoConvert)
        {
            return SampleValue;
        }

        public object GetData(Type format)
        {
            return SampleValue;
        }

        public object GetData(string format)
        {
            return SampleValue;
        }

        public bool GetDataPresent(string format, bool autoConvert)
        {
            return (format == DataFormats.UnicodeText);
        }

        public bool GetDataPresent(Type format)
        {
            return false;
        }

        public bool GetDataPresent(string format)
        {
            return (format == DataFormats.UnicodeText);
        }

        public string[] GetFormats(bool autoConvert)
        {
            return GetFormats();
        }

        public string[] GetFormats()
        {
            return new string[] { DataFormats.UnicodeText };
        }

        public void SetData(string format, object data, bool autoConvert)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void SetData(Type format, object data)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void SetData(string format, object data)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void SetData(object data)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion IDataObject Members

        #region Private fields.

        /// <summary>Sample value to use as a dummy placeholder.</summary>
        private const string SampleValue = "sample value";

        #endregion Private fields.
    }
}
