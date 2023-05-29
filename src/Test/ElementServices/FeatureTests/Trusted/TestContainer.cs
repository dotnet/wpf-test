// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.IO;
using System.IO.Packaging;
using System.Threading; 
using System.Windows.Threading;
using System.Security;
using System.Security.Permissions;

using Microsoft.Test.Container;

namespace Avalon.Test.CoreUI.Trusted
{

    /// <summary>
    /// Package Security Wrapper class
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class TestContainer
    {
        /// <summary>
        /// Package base class
        /// </summary>
        public StorageRootWrapper  stRoot;
        /// <summary>
        /// Access to manipulate streams in a container file
        /// </summary>
        public StreamInfo   stInfo;
        /// <summary>
        /// A sequence of bytes
        /// </summary>
        public Stream       strm;
        /// <summary>
        /// filename of the container
        /// </summary>
        public String filename;
        /// <summary>
        /// Create a Storage Root
        /// </summary>
        /// <param name="filename"></param>
        /// <remarks>
        /// StorageRootWrapper.Open requires both FileIOPermission and SecurityPermission, which we grant only for that line.
        /// </remarks>
        public void CreateStorageRoot(String filename)
        {
            this.filename = filename;
            FileIOPermission pIO = new FileIOPermission(FileIOPermissionAccess.AllAccess,Directory.GetCurrentDirectory());
            SecurityPermission pSP = new SecurityPermission(SecurityPermissionFlag.AllFlags);

            PermissionSet pSet = new PermissionSet(PermissionState.Unrestricted);
            pSet.AddPermission(pIO);
            pSet.AddPermission(pSP);
            pSet.Assert();

            //begin with increased permissions
            stRoot = StorageRootWrapper.Open(filename);
            //end increased permissions

            CodeAccessPermission.RevertAssert();

        }
            /// <summary>
            /// TestContainer contstructor
            /// </summary>
        public TestContainer()
        {

        }
        /// <summary>
        /// Creates a stream
        /// </summary>
        /// <param name="filename">filename of the storageroot to create</param>
        /// <returns>the created stream</returns>

        public Stream CreateStream(String filename)
        {
            this.CreateStorageRoot(filename);
            
            FileInfo fInfo = new FileInfo(filename);
            if (!fInfo.Exists)
                throw new Microsoft.Test.TestSetupException("the container was not created successfully.");
            
            stInfo = stRoot.CreateStream("TestStream");
            strm = stInfo.GetStream();
            return strm;
        }
        /// <summary>
        /// Opens a stream that is existing
        /// </summary>
        /// <returns>the stream</returns>
        public Stream OpenStream()
        {
            this.CreateStorageRoot(filename);
            
            stInfo = stRoot.GetStreamInfo("TestStream");
            strm = stInfo.GetStream();
            return strm;
        }
        /// <summary>
        /// Close a stream
        /// </summary>
        public void CloseStream()
        {
            strm.Close();
            stRoot.Close();
        }

    }

}
