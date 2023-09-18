// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides convenience methods to work with text files.

namespace Test.Uis.IO
{
    #region Namespaces.

    using System;
    using System.IO;
    using System.Text;

    #endregion Namespaces.

    /// <summary>
    /// Provides convenience methods to work with text files.
    /// </summary>
    public class TextFileUtils
    {
        #region Private methods.

        /// <summary>Hide the constructor.</summary>
        private TextFileUtils() { }

        #endregion Private XamlUtils..

        #region Public methods.

        /// <summary>Loads all text from a file.</summary>
        /// <param name='fileName'>File to load text from.</param>
        /// <returns>File contents.</returns>
        public static string LoadFromFile(string fileName)
        {
            string tempFileName; 
            
            new System.Security.Permissions.FileIOPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();

            tempFileName = fileName; 
            
            //if the file does not exist in the currently folder, look for it from the x:\work
            if(!Exists(tempFileName))
            {
                tempFileName = Path.Combine("work", tempFileName);
                tempFileName = Path.Combine(Path.GetPathRoot(System.Environment.SystemDirectory), tempFileName);
            }

            using (StreamReader sr = new StreamReader(tempFileName))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>Saves text to a file.</summary>
        /// <param name='contents'>Contents to save.</param>
        /// <param name='fileName'>File to save text to.</param>
        public static void SaveToFile(string contents, string fileName)
        {
            new System.Security.Permissions.FileIOPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.Write(contents);
            }
        }

        /// <summary>Deletes the specified file.</summary>
        /// <param name='fileName'>Name of file to delete.</param>
        public static void Delete(string fileName)
        {
            new System.Security.Permissions.FileIOPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            File.Delete(fileName);
        }

        /// <summary>Checks whether the specified file exists.</summary>
        /// <param name='fileName'>Name of file to check.</param>
        /// <returns>true if the file exists, false otherwise.</returns>
        public static bool Exists(string fileName)
        {
            new System.Security.Permissions.FileIOPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return File.Exists(fileName);
        }

        #endregion Public methods.
    }
}