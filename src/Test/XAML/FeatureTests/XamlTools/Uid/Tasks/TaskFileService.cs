// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using Microsoft.Build.Utilities;
using System.Diagnostics;
using System.Text;
using System.Security.Cryptography;
    
namespace MS.Internal
{
    //
    // Declaration of interface ITaskFileService
    // Description:
    //      
    //    File service for the input source files in a build task. 
    //
    //    The service does something like below:
    //          return stream for a given source file, 
    //          get last modification time in memory or disk.
    //          or get the link metadata part of a source file.
    //
    //
    internal interface ITaskFileService
    {
        //
        // Get content stream for a given source file
        //
        Stream GetContent(string srcFile);

        //
        // Get MD5 Check Sum for a given source file
        //
        byte[] GetChecksum(string srcFile, Guid hashGuid);

        //
        // Get the last modificatin time for a given file.
        //
        DateTime GetLastChangeTime(string srcFile);

        //
        // Checks to see if file exists
        //
        bool Exists(string fileName);

        //
        // Deletes the specified file
        //
        void Delete(string fileName);

        //
        // Save content stream for the given destination file
        //
        void WriteFile(byte[] contentArray, string destinationFile);

        //
        // Save content stream for the given generated code file.
        // This function decides which extension to use for the file
        // and whether to put it into memory or merely write it to 
        // disk based on whether it is a real build or an intellisense
        // build operation. It also takes care of deleting the corresponding
        // unused generated file (i.e. if real build then it deletes
        // the intellisense file otherwise it deletes the non-intellisense
        // file. 
        // NOTE: UTF8 BOM should not be added to the contentArray.  This
        // method adds BOM before writing file to disk.
        //
        void WriteGeneratedCodeFile(byte[] contentArray, string destinationFileBaseName,
            string generatedExtension, string intellisenseGeneratedExtension, string languageSourceExtension);
        
        //
        // Determines if this is a real build operation or an 
        // intellisense build operation
        //
        bool IsRealBuild {get;}
    }

    //
    // TaskFileService class 
    //
    // This class can be used by different build tasks to get source file 
    // service, the instance is created by those build tasks when task's 
    // Execute( ) method is called.
    //
    internal class TaskFileService : MarshalByRefObject, ITaskFileService
    {
        //
        // Ctor
        //
        public TaskFileService(Task buildTask)
        {
            _buildTask = buildTask;
            _hostFileManager = null;
            _isRealBuild = null;
        }

        #region ITaskFileService

        //
        // Get content stream for a given source file, the content could
        // come from memory or disk based on the build host environment.
        //
        // It is the caller's responsibility to close the stream.
        //
        public Stream GetContent(string srcFile)
        {
            Stream fileStream = null;

            if (String.IsNullOrEmpty(srcFile))
            {
                throw new ArgumentNullException("srcFile");
            }

            if (HostFileManager != null)
            {
                //
                // Build Host environment has a FileManager, use it to get
                // file content from edit buffer in memory.  GetFileContents
                // removes the BOM before returning the string.
                //
                string strFileContent = HostFileManager.GetFileContents(srcFile);


                // IVsMsBuildTaskFileManager.GetFileContents should never return null.
                // GetBytes might throw when input is null, but that should be fine.
                //
                // For xaml file, UTF8 is the standard encoding
                //
                UTF8Encoding utf8Encoding = new UTF8Encoding();
                byte[] baFileContent = utf8Encoding.GetBytes(strFileContent);
                fileStream = new MemoryStream(baFileContent);

            }
            else
            {
                fileStream = File.OpenRead(srcFile);
            }

            return fileStream;
        }

        public byte[] GetChecksum(string fileName, Guid md5HashGuid)
        {
            byte[] hashData=null;

            if (HostFileManager != null)
            {
                object docData = HostFileManager.GetFileDocData(fileName);
                IPersistFileCheckSum fileChecksummer = docData as IPersistFileCheckSum;
                if (fileChecksummer != null)
                {
                    byte[] tempBytes = new byte[1024];
                    int actualSize;
                    fileChecksummer.CalculateCheckSum(md5HashGuid, tempBytes.Length, tempBytes, out actualSize);
                    hashData = new byte[actualSize];
                    for (int i = 0; i < actualSize; i++)
                    {
                        hashData[i] = tempBytes[i];
                    }
                }
            }
            if (hashData == null)
            {
                HashAlgorithm MD5cryptoProvider = new MD5CryptoServiceProvider();
                using (Stream fileStream = File.OpenRead(fileName))
                {
                    hashData = MD5cryptoProvider.ComputeHash(fileStream);
                }
            }
            return hashData;
        }

        //
        // Get the last modificatin time for a given file.
        //
        public DateTime GetLastChangeTime(string srcFile)
        {
            DateTime lastChangeDT = new DateTime(0);

            if (String.IsNullOrEmpty(srcFile))
            {
                throw new ArgumentNullException("srcFile");
            }

            if (HostFileManager != null)
            {
                long fileTime = HostFileManager.GetFileLastChangeTime(srcFile);
                lastChangeDT = DateTime.FromFileTime(fileTime);
            }
            else
            {
                lastChangeDT = File.GetLastWriteTime(srcFile);
            }

            return lastChangeDT;
        }


        //
        // Checks to see if file exists
        //
        public bool Exists(string fileName)
        {
            bool fileExists = false;
            
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            if (HostFileManager != null) 
            {
                fileExists = HostFileManager.Exists(fileName, IsRealBuild);
            }
            else
            {
                fileExists = File.Exists(fileName);
            }

            return fileExists;
        }

        //
        // Deletes the specified file
        //
        public void Delete(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            if (HostFileManager != null)
            {
                HostFileManager.Delete(fileName);
            }
            else
            {
                File.Delete(fileName);
            }
        }

        //
        // Save content stream for the given destination file
        // UTF8 BOM should not be added to the contentArray.  This
        // method adds BOM before writing file to disk.
        //
        public void WriteFile(byte[] contentArray, string destinationFile)
        {
            if (String.IsNullOrEmpty(destinationFile))
            {
                throw new ArgumentNullException("destinationFile");                
            }
            
            if (contentArray == null)
            {
                throw new ArgumentNullException("contentArray");
            }

            UTF8Encoding utf8Encoding = new UTF8Encoding();
            string contentStr = utf8Encoding.GetString(contentArray);

            if (HostFileManager != null)
            {
                // PutGeneratedFileContents adds BOM to the file when saving
                // to memory or disk.
                HostFileManager.PutGeneratedFileContents(destinationFile, contentStr);
            }
            else
            {
                // Add BOM for UTF8Encoding since the input contentArray is not supposed to
                // have it already.
                using (StreamWriter sw = new StreamWriter(destinationFile, false, new UTF8Encoding(true)))
                {
                    sw.WriteLine(contentStr);
                }
            }
        }

        // Save content stream for the given generated code file.
        // This function decides which extension to use for the file
        // and whether to put it into memory or merely write it to 
        // disk based on whether it is a real build or an intellisense
        // build operation. It also takes care of deleting the corresponding
        // unused generated file (i.e. if real build then it deletes
        // the intellisense file otherwise it deletes the non-intellisense
        // file. 
        // NOTE: UTF8 BOM should not be added to the contentArray.  This
        // method adds BOM before writing file to disk.
        //
        public void WriteGeneratedCodeFile(byte[] contentArray, string destinationFileBaseName,
            string generatedExtension, string intellisenseGeneratedExtension, string languageSourceExtension)
        {
            if (String.IsNullOrEmpty(destinationFileBaseName))
            {
                throw new ArgumentNullException("destinationFileBaseName");
            }

            if (contentArray == null)
            {
                throw new ArgumentNullException("contentArray");
            }

            string destinationFile = destinationFileBaseName
                + (IsRealBuild ? generatedExtension : intellisenseGeneratedExtension)
                + languageSourceExtension;

            string deleteFile = destinationFileBaseName
                + (IsRealBuild ? intellisenseGeneratedExtension : generatedExtension)
                + languageSourceExtension;

            UTF8Encoding utf8Encoding = new UTF8Encoding();
            string contentStr = utf8Encoding.GetString(contentArray);
            bool putFileIntoMemory = false;

            if (HostFileManager != null)
            {
                // We are being called from Visual Studio

                if (!IsRealBuild)
                {
                    // If it is an intellisense/design-time generated file,
                    // then always put it into memory.
                    putFileIntoMemory = true;
                }
                else
                {
                    // If it is a real build operation, then only put the destinationFile
                    // into memory if the document is already open in memory. Otherwise,
                    // simply write the file to disk.
                    if (null != HostFileManager.GetFileDocData(destinationFile))
                    {
                        putFileIntoMemory = true;
                    }
                }
            }

            if (putFileIntoMemory)
            {
                // PutGeneratedFileContents adds BOM to the file when saving
                // to memory or disk.
                HostFileManager.PutGeneratedFileContents(destinationFile, contentStr);
            }
            else
            {
                // Add BOM for UTF8Encoding since the input contentArray is not supposed to
                // have it already.
                using (StreamWriter sw = new StreamWriter(destinationFile, false, new UTF8Encoding(true)))
                {
                    sw.WriteLine(contentStr);
                }
            }

            if (HostFileManager != null)
            {
                // If called from within Visual Studio, then delete the unused
                // generated file.
                HostFileManager.Delete(deleteFile);
            }
        }

        //
        // Tells if this is an intellisense build or a real build.
        // Caching the result since this is called a lot of times.
        //
        public bool IsRealBuild
        {
            get
            {
                if (_isRealBuild == null)
                {
                    bool isRealBuild = true;
                    
                    if (HostFileManager != null)
                    {
                        isRealBuild = HostFileManager.IsRealBuildOperation();
                    }

                    _isRealBuild = isRealBuild;
                }

                return _isRealBuild.Value;
            }
        }

        #endregion ITaskFileService

        #region private property

        //
        // Get the FileManager implemented by the Host system
        //
        private IVsMSBuildTaskFileManager HostFileManager
        {
            get
            {
                if (_hostFileManager == null)
                {
                    if (_buildTask != null && _buildTask.HostObject != null)
                    {
                        _hostFileManager = _buildTask.HostObject as IVsMSBuildTaskFileManager;
                    }
                }

                return _hostFileManager;
            }
        }

        #endregion private property
        

        #region private data field

        private Task _buildTask;
        private IVsMSBuildTaskFileManager _hostFileManager;
        private Nullable<bool> _isRealBuild;
        
        #endregion private data field

    }


}
