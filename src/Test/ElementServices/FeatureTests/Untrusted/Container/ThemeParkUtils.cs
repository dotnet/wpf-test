// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.IO.Packaging;
using System.IO.IsolatedStorage;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;


namespace Avalon.Test.CoreUI.Container
{
    /// <summary>
    /// Required utility classes for StorageRootThemePark Tests
    /// </summary>
    public class ThemeParkUtils
    {
        /// <summary>
        /// get the isolatedfilestream from isolatedstoragefile
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static IsolatedStorageFileStream GetIsolatedFileStream( string path, FileMode mode )
        {
            IsolatedStorageFile isf = IsolatedStorageFile.GetStore( IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain, null, null );
            return new IsolatedStorageFileStream( path, mode, isf );
        }

        /// <summary>
        /// get the isolatedfilestream from isolatedstoragefile
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <param name="access"></param>
        /// <returns></returns>
        public static IsolatedStorageFileStream GetIsolatedFileStream( string path, FileMode mode, FileAccess access )
        {
            IsolatedStorageFile isf = IsolatedStorageFile.GetStore( IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain, null, null );
            return new IsolatedStorageFileStream( path, mode, access, isf );
        }

        /// <summary>
        /// Deletes IsolatedStorageFile
        /// </summary>
        public static void CleanStorage()
        {
            IsolatedStorageFile isf = IsolatedStorageFile.GetStore( IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain, null, null );
            string[] files = isf.GetFileNames("container.xmf");

            if ( files != null )
            {
                foreach(string s in files)
                {
                    isf.DeleteFile( s );
                }
            }
        }

        /// <summary>
        /// Checks to see if the container file exists
        /// </summary>
        /// <returns>true if it exists</returns>
        public static bool ContainerFileExists()
        {
            IsolatedStorageFile isf = IsolatedStorageFile.GetStore( IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain, null, null );
            string[] files = isf.GetFileNames("container.xmf");

            if ( files != null && files.Length > 0 )
            {
                return true;
            }
            return false;
        }
    }
}

