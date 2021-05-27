// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.IO.Packaging;
using System.Reflection;
using Microsoft.Test.Serialization;

/*******************************************************************
 * Purpose: Create a public wrapper for StorageRoot. StorageRoot is internal.
 * 
 *
********************************************************************/


namespace Microsoft.Test.Container
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

        /// <summary>
        /// StorageRootWrapper emulates the behavior of StorageRoot.
        /// 
        /// StorageRoot inherits from StorageInfo, so StorageRoot objects can be implicitly
        /// type cast to StorageInfo.
        /// 
        /// StorageRootWrapper doesn't have this ability, since it doesn't inherit from StorageInfo.
        /// 
        /// Following overloaded implicit type cast operator gives StorageRootWrapper that ability. 
        /// </summary>
        /// <param name="wrapper"></param>
        /// <returns></returns>
        public static implicit operator StorageInfo(StorageRootWrapper wrapper)
        {
            return (wrapper._storageRoot as StorageInfo);
        }

        #region Methods/Properties declared on StorageRoot
        /// <summary>
        /// Close
        /// </summary>
        public void Close()
        {
            try
            {
                _storageRootType.InvokeMember("Close",
                    InstanceMethodBindFlags, null, _storageRoot, new object[] { });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }

        /// <summary>
        /// CreateOnIStorage
        /// </summary>
        /// <param name="rootUnknown"></param>
        /// <returns></returns>
        public static StorageRootWrapper CreateOnIStorage(object rootUnknown)
        {
            object storageRoot = null;
            try
            {
                storageRoot = _storageRootType.InvokeMember("CreateOnIStorage",
                                      StaticMethodBindFlags, null, null,
                                      new object[1] { rootUnknown });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }

            return (new StorageRootWrapper(storageRoot));
        }

        /// <summary>
        /// CreateOnStream
        /// </summary>
        /// <param name="baseStream"></param>
        /// <returns></returns>
        public static StorageRootWrapper CreateOnStream(Stream baseStream)
        {
            object storageRoot = null;
            try
            {
                storageRoot = _storageRootType.InvokeMember("CreateOnStream",
                                          StaticMethodBindFlags, null, null,
                                          new object[1] { baseStream });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }

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
            object storageRoot = null;
            try
            {
                storageRoot = _storageRootType.InvokeMember("CreateOnStream",
                                          StaticMethodBindFlags, null, null,
                                          new object[2] { baseStream, mode });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }

            return (new StorageRootWrapper(storageRoot));
        }

        /// <summary>
        /// Open
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static StorageRootWrapper Open(string path)
        {
            object storageRoot = null;
            try
            {
                storageRoot = _storageRootType.InvokeMember("Open",
                                          StaticMethodBindFlags, null, null,
                                          new object[1] { path });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }

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
            object storageRoot = null;
            try
            {
                storageRoot = _storageRootType.InvokeMember("Open",
                                          StaticMethodBindFlags, null, null,
                                          new object[2] { path, mode });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }

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
            object storageRoot = null;
            try
            {                
                storageRoot = _storageRootType.InvokeMember("Open",
                                          StaticMethodBindFlags, null, null,
                                          new object[3] { path, mode, access });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }

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
            object storageRoot = null;
            try
            {
                storageRoot = _storageRootType.InvokeMember("Open",
                                          StaticMethodBindFlags, null, null,
                                          new object[4] { path, mode, access, share });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }

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
            object storageRoot = null;
            try
            {
                storageRoot = _storageRootType.InvokeMember("Open",
                                          StaticMethodBindFlags, null, null,
                                          new object[5] { path, mode, access, share, sectorSize });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }

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

        /// <summary>
        /// CreateStream
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public StreamInfo CreateStream(string name)
        {
            object retVal = null;
            try
            {                
                retVal = _storageRootType.InvokeMember("CreateStream",
                    InstanceMethodBindFlags, null, _storageRoot, new object[1] { name });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }

            return (StreamInfo)retVal;
        }

        /// <summary>
        /// CreateStream
        /// </summary>
        /// <param name="name"></param>
        /// <param name="compressionOption"></param>
        /// <param name="encryptionOption"></param>
        /// <returns></returns>
        public StreamInfo CreateStream(string name, CompressionOption compressionOption, EncryptionOption encryptionOption)
        {
            object retVal = null;
            try
            {
                retVal = _storageRootType.InvokeMember("CreateStream",
                    InstanceMethodBindFlags, null, _storageRoot, new object[3] { name, compressionOption, encryptionOption });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }

            return (StreamInfo)retVal;
        }

        /// <summary>
        /// GetStreamInfo
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public StreamInfo GetStreamInfo(string name)
        {
            object retVal = null;
            try
            {
                retVal = _storageRootType.InvokeMember("GetStreamInfo",
                    InstanceMethodBindFlags, null, _storageRoot, new object[1] { name });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }

            return (StreamInfo)retVal;
        }

        /// <summary>
        /// StreamExists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool StreamExists(string name)
        {
            object retVal = null;
            try
            {            
                retVal = _storageRootType.InvokeMember("StreamExists",                
                    InstanceMethodBindFlags, null, _storageRoot, new object[1] { name });        
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }

            return (bool)retVal;
        }

        /// <summary>
        /// DeleteStream
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public void DeleteStream(string name)
        {
          
            try
            {
                 _storageRootType.InvokeMember("DeleteStream",
                    InstanceMethodBindFlags, null, _storageRoot, new object[1] { name });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }

            return ;
        }

        // Storage methods

        /// <summary>
        /// CreateSubStorage
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public StorageInfo CreateSubStorage(string name)
        {
            object retVal = null;
            try
            {
                retVal = _storageRootType.InvokeMember("CreateSubStorage",
                    InstanceMethodBindFlags, null, _storageRoot, new object[1] { name });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }

            return (StorageInfo)retVal;
        }

        /// <summary>
        /// GetStorageInfo
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public StorageInfo GetSubStorageInfo(string name)
        {
            object retVal = null;
            try
            {
                retVal = _storageRootType.InvokeMember("GetSubStorageInfo",
                    InstanceMethodBindFlags, null, _storageRoot, new object[1] { name });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }

            return (StorageInfo)retVal;
        }

        /// <summary>
        /// SubStorageExists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool SubStorageExists(string name)
        {
            object retVal = null;
            try
            {
                retVal = _storageRootType.InvokeMember("SubStorageExists",
                    InstanceMethodBindFlags, null, _storageRoot, new object[1] { name });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }

            return (bool)retVal;
        }

        /// <summary>
        /// DeleteStorage
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public void  DeleteSubStorage(string name)
        {
        
            try
            {
                _storageRootType.InvokeMember("DeleteSubStorage",
                    InstanceMethodBindFlags, null, _storageRoot, new object[1] { name });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }

            return ;
        }

        /// <summary>
        /// GetStreams
        /// </summary>
        /// <returns></returns>
        public StreamInfo[] GetStreams()
        {
            object retVal = null;
            try
            {
                retVal = _storageRootType.InvokeMember("GetStreams",
                    InstanceMethodBindFlags, null, _storageRoot, new object[] { });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }

            return (StreamInfo[])retVal;
        }

        /// <summary>
        /// GetSubStorages
        /// </summary>
        /// <returns></returns>
        public StorageInfo[] GetSubStorages()
        {
            object retVal = null;
            try
            {
                retVal = _storageRootType.InvokeMember("GetSubStorages",
                    InstanceMethodBindFlags, null, _storageRoot, new object[] { });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }

            return (StorageInfo[])retVal;
        }
        #endregion Methods/Properties inherited from StorageInfo

        private static Type _storageRootType = WrapperUtil.AssemblyWB.GetType("System.IO.Packaging.StorageRoot");
        // The "real" StorageRoot object, that gets all the method/property calls. 
        internal object _storageRoot;

        private static BindingFlags StaticMethodBindFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod;
        private static BindingFlags InstanceMethodBindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod;
        private static BindingFlags InstancePropertyBindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty;
    }
}
