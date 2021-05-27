// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Create a public wrapper for StorageRoot, which is internal.
 * 
 *
********************************************************************/
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Input;

namespace DRT
{
    /// <summary>
    /// Wrap an internal StorageRoot by giving it a public api that mirrors the internal one.
    /// Use reflection to invoke the internal properties and methods.
    /// 
    /// Most of the methods on StorageRoot are static, and they return a StorageRoot. 
    /// Wrappers for those methods return not a StorageRoot, but a StorageRootWrapper, 
    /// so that the following method/property calls come back to us.
    /// 
    /// Only one public member of StorageRoot is not mirrored here:
    /// public DataSpaceManager GetDataSpaceManager();
    /// That's because DataSpaceManager itself is internal now, and we don't want to wrap it.
    /// We don't have any dataspace tests, so we don't use the above method anyway.
    /// </summary>
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    public class StorageRootWrapper
    {
        /// <summary>
        /// Private constructor. Customers should not be able to create StorageRootWrapper directly.
        /// They can only get one using one of the static methods.
        /// </summary>
        private StorageRootWrapper(object storageRoot)
        {
            _storageRoot = storageRoot;
        }

        #region Methods/Properties declared on StorageRoot
        /// <summary>
        /// Close
        /// </summary>
        public void Close()
        {
            _storageRootType.InvokeMember("Close",
                InstanceMethodBindFlags, null, _storageRoot, new object[] { });
        }

        /// <summary>
        /// CreateOnIStorage
        /// </summary>
        /// <param name="rootUnknown"></param>
        /// <returns></returns>
        public static StorageRootWrapper CreateOnIStorage(object rootUnknown)
        {
            object storageRoot = _storageRootType.InvokeMember("CreateOnIStorage",
                                      StaticMethodBindFlags, null, null,
                                      new object[1] { rootUnknown });

            return (new StorageRootWrapper(storageRoot));
        }

        /// <summary>
        /// CreateOnStream
        /// </summary>
        /// <param name="baseStream"></param>
        /// <returns></returns>
        public static StorageRootWrapper CreateOnStream(Stream baseStream)
        {
            object storageRoot = _storageRootType.InvokeMember("CreateOnStream",
                                      StaticMethodBindFlags, null, null,
                                      new object[1] { baseStream });

            return (new StorageRootWrapper(storageRoot));
        }

        /// <summary>
        /// CreateOnStream
        /// </summary>
        /// <param name="baseStream"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static StorageRootWrapper CreateOnStream(Stream baseStream, FileMode mode)
        {
            object storageRoot = _storageRootType.InvokeMember("CreateOnStream",
                                      StaticMethodBindFlags, null, null,
                                      new object[2] { baseStream, mode });

            return (new StorageRootWrapper(storageRoot));
        }

        /// <summary>
        /// Open
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static StorageRootWrapper Open(string path)
        {
            object storageRoot = _storageRootType.InvokeMember("Open",
                                      StaticMethodBindFlags, null, null,
                                      new object[1] { path });

            return (new StorageRootWrapper(storageRoot));
        }

        /// <summary>
        /// Open
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static StorageRootWrapper Open(string path, FileMode mode)
        {
            object storageRoot = _storageRootType.InvokeMember("Open",
                                      StaticMethodBindFlags, null, null,
                                      new object[2] { path, mode });

            return (new StorageRootWrapper(storageRoot));
        }

        /// <summary>
        /// Open
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <param name="access"></param>
        /// <returns></returns>
        public static StorageRootWrapper Open(string path, FileMode mode, FileAccess access)
        {
            object storageRoot = _storageRootType.InvokeMember("Open",
                                      StaticMethodBindFlags, null, null,
                                      new object[3] { path, mode, access });

            return (new StorageRootWrapper(storageRoot));
        }

        /// <summary>
        /// Open
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <param name="access"></param>
        /// <param name="share"></param>
        /// <returns></returns>
        public static StorageRootWrapper Open(string path, FileMode mode, FileAccess access, FileShare share)
        {
            object storageRoot = _storageRootType.InvokeMember("Open",
                                      StaticMethodBindFlags, null, null,
                                      new object[4] { path, mode, access, share });

            return (new StorageRootWrapper(storageRoot));
        }

        /// <summary>
        /// Open
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <param name="access"></param>
        /// <param name="share"></param>
        /// <param name="sectorSize"></param>
        /// <returns></returns>
        public static StorageRootWrapper Open(string path, FileMode mode, FileAccess access, FileShare share, int sectorSize)
        {
            object storageRoot = _storageRootType.InvokeMember("Open",
                                      StaticMethodBindFlags, null, null,
                                      new object[5] { path, mode, access, share, sectorSize });

            return (new StorageRootWrapper(storageRoot));
        }

        /// <summary>
        /// OpenAccess
        /// </summary>
        public FileAccess OpenAccess 
        {
            get
            {
                PropertyInfo pi = _storageRootType.GetProperty("OpenAccess", InstancePropertyBindFlags);
                return (FileAccess)pi.GetValue(_storageRoot, null);
            }
        }
        #endregion Methods/Properties declared on StorageRoot

        #region Methods/Properties inherited from StorageInfo
        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get
            {
                PropertyInfo pi = _storageRootType.GetProperty("Name", InstancePropertyBindFlags);
                return (string)pi.GetValue(_storageRoot, null);
            }
        }

        public StreamInfo CreateStream(
                                            string name,
                                            CompressionOption compressionOption,
                                           EncryptionOption encryptionOption)
        {
            return (StreamInfo)_storageRootType.InvokeMember("CreateStream",
                InstanceMethodBindFlags, null, _storageRoot, new object[3] { name, compressionOption, encryptionOption});
        }

        /// <summary>
        /// CreateStream
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public StreamInfo CreateStream(string name)
        {
            return (StreamInfo)_storageRootType.InvokeMember("CreateStream",
                InstanceMethodBindFlags, null, _storageRoot, new object[1] { name });
        }

        /// <summary>
        /// GetStreamInfo
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public StreamInfo GetStreamInfo(string name)
        {
            return (StreamInfo)_storageRootType.InvokeMember("GetStreamInfo",
                InstanceMethodBindFlags, null, _storageRoot, new object[1] { name });
        }

        /// <summary>
        /// StreamExists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool StreamExists(string name)
        {
            return (bool)_storageRootType.InvokeMember("StreamExists",
                InstanceMethodBindFlags, null, _storageRoot, new object[1] { name });
        }

        /// <summary>
        /// DeleteStream
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public void DeleteStream(string name)
        {
            _storageRootType.InvokeMember("DeleteStream",
                InstanceMethodBindFlags, null, _storageRoot, new object[1] { name });
        }

        // Storage methods

        /// <summary>
        /// CreateSubStorage
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public StorageInfo CreateSubStorage(string name)
        {
            return (StorageInfo)_storageRootType.InvokeMember("CreateSubStorage",
                InstanceMethodBindFlags, null, _storageRoot, new object[1] { name });
        }

        /// <summary>
        /// GetStorageInfo
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public StorageInfo GetSubStorageInfo(string name)
        {
            return (StorageInfo)_storageRootType.InvokeMember("GetSubStorageInfo",
                InstanceMethodBindFlags, null, _storageRoot, new object[1] { name });
        }

        /// <summary>
        /// SubStorageExists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool SubStorageExists(string name)
        {
            return (bool)_storageRootType.InvokeMember("SubStorageExists",
                InstanceMethodBindFlags, null, _storageRoot, new object[1] { name });
        }

        /// <summary>
        /// DeleteStorage
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public void DeleteSubStorage(string name)
        {
            _storageRootType.InvokeMember("DeleteSubStorage",
                InstanceMethodBindFlags, null, _storageRoot, new object[1] { name });
        }

        // Enumeration methods
        // These methods must return arrays, because they can't honor
        // the IEnumerator contract of keeping in sync with the underlying
        // collection.  DirectoryInfo has the same pattern.

        /// <summary>
        /// GetStreams
        /// </summary>
        /// <returns></returns>
        public StreamInfo[] GetStreams()
        {
            return (StreamInfo[])_storageRootType.InvokeMember("GetStreams",
                InstanceMethodBindFlags, null, _storageRoot, new object[] {});
        }

        /// <summary>
        /// GetStorages
        /// </summary>
        /// <returns></returns>
        public StorageInfo[] GetStorages()
        {
            return (StorageInfo[])_storageRootType.InvokeMember("GetStorages",
                InstanceMethodBindFlags, null, _storageRoot, new object[] {});
        }
        #endregion Methods/Properties inherited from StorageInfo

        private static Type _storageRootType = typeof(System.Windows.DependencyObject).Assembly.GetType("System.IO.Packaging.StorageRoot");
        // The "real" StorageRoot object, that gets all the method/property calls. 
        private object _storageRoot;

        private static BindingFlags StaticMethodBindFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod |BindingFlags.NonPublic ;
        private static BindingFlags InstanceMethodBindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod |BindingFlags.NonPublic ;
//        private static BindingFlags StaticPropertyBindFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty;
        private static BindingFlags InstancePropertyBindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty |BindingFlags.NonPublic ;
    }
}
