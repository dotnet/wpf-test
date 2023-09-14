// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/// <summary>
/// Class contains functions that requires additional security than what SEE gives
/// </summary>

using System;
using System.IO;
using System.Xml;
using System.Security;
using System.Security.Permissions;

using System.Windows;
using System.Windows.Markup;

namespace Avalon.Test.ComponentModel
{
    public static class SecureFunctions
    {
        /// <summary>
        /// Get XML document
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static XmlDocument GetXMLDocument(string fileName)
        {
            //Assert UnRestricted FileIOPermission
            FileIOPermission filePermission = new FileIOPermission(PermissionState.Unrestricted);

            filePermission.Assert();

            //Check whether file exists in the current directory
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName.ToString() + " cannot find in current directory");
            }

            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(fileName);

            return xmlDoc;
        }

        /// <summary>
        /// Load XAML page
        /// </summary>
        /// <param name="fileName"></param>
        public static object LoadXAML(string fileName)
        {
            object element = null;

            if ( String.IsNullOrEmpty(fileName) )
            {
                throw new ArgumentException("File name is empty or null");
            }

            //Assert UnRestricted FileIOPermission
            FileIOPermission filePermission = new FileIOPermission(PermissionState.Unrestricted);

            filePermission.Assert();

            //Check whether file exists in the current directory
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName.ToString() + " cannot find in current directory");
            }

            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, default(FileShare));
            element = XamlReader.Load(fs);

            if (element == null)
            {
                throw new NullReferenceException("XamlReader.Load returned null");
            }

            return element;
        }
    }
}
