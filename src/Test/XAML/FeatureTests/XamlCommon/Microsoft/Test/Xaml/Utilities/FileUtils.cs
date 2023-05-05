// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;

namespace Microsoft.Test.Xaml.Utilities
{
    /// <summary>
    /// Utility class to copy Dirs
    /// </summary>
    public static class FileUtils
    {
        /// <summary>
        /// Copy directory structure recursively
        /// </summary>
        /// <param name="src">source dir</param>
        /// <param name="dst">dst directory. </param>
        public static void CopyDirectory(string src, string dst)
        {
            string[] files;

            if (!Directory.Exists(dst)) Directory.CreateDirectory(dst);
            files = Directory.GetFileSystemEntries(src);
            foreach (string element in files)
            {
                //// Sub directories
                if (Directory.Exists(element))
                    CopyDirectory(element, Path.Combine(dst, Path.GetFileName(element)));
                //// Files in directory
                else
                    File.Copy(element, Path.Combine(dst, Path.GetFileName(element)), true);
            }
        }

        /// <summary>
        /// CopyFiles To Directory
        /// </summary>
        /// <param name="files">List of files</param>
        /// <param name="dstDirPath">dir path to copy files to</param>
        public static void CopyFilesToDirectory(List<FileInfo> files, string dstDirPath)
        {
            if (Directory.Exists(dstDirPath) == false)
            {
                Directory.CreateDirectory(dstDirPath);
            }

            foreach (FileInfo file in files)
            {
                File.Copy(file.FullName, Path.Combine(dstDirPath, file.Name), true);
            }
        }
    }
}
