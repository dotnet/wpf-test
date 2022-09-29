// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// Description:
//  This is an test harness that simplifies testing of internal FormatVersion APIs, by exposing them as public classes 
//
//

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO.Packaging;
    
namespace DRT
{
    internal class DrtVersionedStream : Stream
    {
        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------
        #region Stream Methods
        /// <summary>
        /// Return the bytes requested from the container
        /// </summary>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        /// <summary>
        /// Write
        /// </summary>
        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }

        /// <summary>
        /// ReadByte
        /// </summary>
        public override int ReadByte()
        {
            return _stream.ReadByte();
        }

        /// <summary>
        /// WriteByte
        /// </summary>
        public override void WriteByte(byte b)
        {
            _stream.WriteByte(b);
        }

        /// <summary>
        /// Seek
        /// </summary>
        /// <param name="offset">offset</param>
        /// <param name="origin">origin</param>
        /// <returns>zero</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        /// <summary>
        /// SetLength
        /// </summary>
        public override void SetLength(long newLength)
        {
            _stream.SetLength(newLength);
        }

        /// <summary>
        /// Flush
        /// </summary>
        public override void Flush()
        {
            _stream.Flush();
        }
        #endregion Stream Methods

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------
        #region Stream Properties
        /// <summary>
        /// Current logical position within the stream
        /// </summary>
        public override long Position
        {
            get
            {
                return _stream.Position;
            }
            set
            {
                _stream.Position = value;
            }
        }

        /// <summary>
        /// Length
        /// </summary>
        public override long Length
        {
            get
            {
                return _stream.Length;
            }
        }

        /// <summary>
        /// Is stream readable?
        /// </summary>
        /// <remarks>returns false when called on disposed stream</remarks>
        public override bool CanRead
        {
            get
            {
                return _stream.CanRead;
            }
        }

        /// <summary>
        /// Is stream seekable - should be handled by our owner
        /// </summary>
        /// <remarks>returns false when called on disposed stream</remarks>
        public override bool CanSeek
        {
            get
            {
                return _stream.CanSeek;
            }
        }

        /// <summary>
        /// Is stream writeable?
        /// </summary>
        /// <remarks>returns false when called on disposed stream</remarks>
        public override bool CanWrite
        {
            get
            {
                return _stream.CanWrite;
            }
        }

        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------
        public DrtVersionedStream(Stream baseStream, DrtVersionedStreamOwner updater)
        {
            _versionedStreamObject = s_constructorInfo.Invoke(new Object[] { 
                baseStream, updater.VersionedStreamOwner });

            _stream = (Stream)_versionedStreamObject;
        }

        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------
        /// <summary>
        /// Dispose(bool)
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    _stream.Close();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------
        private object _versionedStreamObject = null;

        //  get WindowsBase.dll into memory 
        Uri _uriRes = PackUriHelper.CreatePartUri(new Uri("a", UriKind.Relative));

        private static string s_versionedStreamClassName = "MS.Internal.IO.Packaging.CompoundFile.VersionedStream";
        private static Type s_versionedStreamClassType = typeof(System.Windows.Rect).Assembly.GetType(s_versionedStreamClassName);

        private static ConstructorInfo s_constructorInfo = s_versionedStreamClassType.GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase, null,
            new Type[] { typeof(Stream), DrtVersionedStreamOwner._versionUpdaterClassType  }, null);
               
        // the VersionedStream
        private Stream _stream;

        #endregion
    }

    public class DrtVersionedStreamOwner
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="codeVersion">version of calling code</param>
        /// <param name="s">stream where format is to be persisted to/from</param>
        public DrtVersionedStreamOwner(Stream s, DrtFormatVersion codeVersion)
        {
            _versionUpdaterObject = s_constructorInfo.Invoke(new Object[] { 
                s, codeVersion.FormatVersion });
        }

        /// <summary>
        /// Callback for when a stream is written to
        /// </summary>
        public void WriteAttempt()
        {
            s_writeAttemptMethodInfo.Invoke(_versionUpdaterObject, null);
        }
#if false

        /// <summary>
        /// Callback for when a Stream is read from
        /// </summary>
        public void ReadAttempt()
        {
            _readAttemptMethodInfo.Invoke(_versionUpdaterObject, null);
        }
        /// <summary>
        /// Ensure that the version is persisted
        /// </summary>
        /// <remarks>Leaves stream at position just after the FormatVersion.
        /// Destructive.  This is called automatically from WriteAttempt but callers
        /// can call directly if they have changed the stream contents to a format
        /// that is no longer compatible with the persisted FormatVersion.  If
        /// this is not called directly, and a FormatVersion was found in the file
        /// then only the Updater field is modified.
        /// </remarks>
        public void PersistVersion()
        {
            _persistVersionMethodInfo.Invoke(_versionUpdaterObject, null);
        }
#endif
        /// <summary>
        /// Ensure that the version is persisted
        /// </summary>
        /// <remarks>Leaves stream at position just after the FormatVersion.
        /// Destructive.  This is called automatically from WriteAttempt but callers
        /// can call directly if they have changed the stream contents to a format
        /// that is no longer compatible with the persisted FormatVersion.  If
        /// this is not called directly, and a FormatVersion was found in the file
        /// then only the Updater field is modified.
        /// </remarks>
        public void PersistVersion(DrtFormatVersion version)
        {
            s_persistVersionMethodInfo2.Invoke(_versionUpdaterObject, new Object[] { version.FormatVersion });
        }
#if false
        /// <summary>
        /// Parse FileVersion from stream
        /// </summary>
        /// <remarks>leaves stream at position just after the FormatVersion</remarks>
        /// <returns>Returns null if stream is empty</returns>
        public DrtFormatVersion GetFileVersion()
        {
            return new DrtFormatVersion(_getFileVersionMethodInfo.Invoke(_versionUpdaterObject, null));
        }
#endif
        public object VersionedStreamOwner
        {
            get
            {
                return _versionUpdaterObject;
            }
        }

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------
        private object _versionUpdaterObject = null;

        //  get WindowsBase.dll into memory 
        Uri _uriRes = PackUriHelper.CreatePartUri(new Uri("a", UriKind.Relative));

        private static string s_versionUpdaterClassName = "MS.Internal.IO.Packaging.CompoundFile.VersionedStreamOwner";
        public static Type _versionUpdaterClassType = typeof(System.Windows.Rect).Assembly.GetType(s_versionUpdaterClassName);

        //        [ComVisibleAttribute(true)] public ConstructorInfo GetConstructor ( BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers ) 
        private static ConstructorInfo s_constructorInfo = _versionUpdaterClassType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase, null,
            new Type[] { typeof(Stream), DrtFormatVersion._formatVersionClassType }, null);

//        private static MethodInfo _getFileVersionMethodInfo = _versionUpdaterClassType.GetMethod("GetFileVersion", BindingFlags.Instance | BindingFlags.NonPublic);
//        private static MethodInfo _persistVersionMethodInfo = _versionUpdaterClassType.GetMethod("PersistVersion", 
//            BindingFlags.Instance | BindingFlags.NonPublic, null,
//            new Type[] { }, null);
        private static MethodInfo s_persistVersionMethodInfo2 = _versionUpdaterClassType.GetMethod("PersistVersion", 
            BindingFlags.Instance | BindingFlags.NonPublic, null,
            new Type[] { DrtFormatVersion._formatVersionClassType }, null);

        private static MethodInfo s_writeAttemptMethodInfo = _versionUpdaterClassType.GetMethod("WriteAttempt", BindingFlags.Instance | BindingFlags.NonPublic);
//        private static MethodInfo _readAttempt2MethodInfo = _versionUpdaterClassType.GetMethod("ReadAttempt", new Type[] { typeof(bool) });
//        private static MethodInfo _readAttemptMethodInfo = _versionUpdaterClassType.GetMethod("ReadAttempt", new Type[] { } );

    }

    public class DrtFormatVersion
    {
        public DrtFormatVersion(object formatVersion)
        {
            _fVersionObject = formatVersion;
        }

        public DrtFormatVersion(String featureId,
                                DrtVersionPair writerVersion,
                                DrtVersionPair readerVersion,
                                DrtVersionPair updaterVersion)
        {
            _fVersionObject = s_constructorInfo.Invoke(new Object[] { 
                featureId,
                writerVersion.VersionPair, 
                readerVersion.VersionPair, 
                updaterVersion.VersionPair });
        }

        public object FormatVersion
        {
            get
            {
                return _fVersionObject;
            }
        }
#if false
        public DrtVersionPair Reader
        {
            get
            {
                return 
            }
        }
#endif
        private object _fVersionObject = null;

        //  get WindowsBase.dll into memory 
        Uri _uriRes = PackUriHelper.CreatePartUri(new Uri("a", UriKind.Relative));

        private static string s_formatVersionClassName = "MS.Internal.IO.Packaging.CompoundFile.FormatVersion";
        public static Type _formatVersionClassType = typeof(System.Windows.Rect).Assembly.GetType(s_formatVersionClassName);

        //        [ComVisibleAttribute(true)] public ConstructorInfo GetConstructor ( BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers ) 
        private static ConstructorInfo s_constructorInfo = _formatVersionClassType.GetConstructor(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase, null,
            new Type[] { typeof(String), DrtVersionPair._versionPairClassType, DrtVersionPair._versionPairClassType, DrtVersionPair._versionPairClassType }, null);
    }

    public class DrtVersionPair : IComparable
    {
        public DrtVersionPair(Int16 major, Int16 minor)
        {
            _vPairObject = s_constructorInfo.Invoke(new Object[] { major, minor });
        }

        /// <summary>
        /// IComparable.CompareTo
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(Object obj)
        {
            return (int)(s_compareToMethodInfo.Invoke(_vPairObject, new Object[] { obj }));
        }

        /// <summary>
        /// Major number of version
        /// </summary>
        public Int16 Major
        {
            get
            {
                return (Int16)(s_majorPropertyInfo.GetValue(_vPairObject, null));
            }
        }

        /// <summary>
        /// Minor number of version
        /// </summary>
        public Int16 Minor
        {
            get
            {
                return (Int16)(s_minorPropertyInfo.GetValue(_vPairObject, null));
            }
        }

        /// <summary>
        /// Returns a string that represents the current VersionPair object.
        /// The string is of the form (major,minor).
        /// </summary>
        public override string ToString()
        {
            return (string)(s_toStringMethodInfo.Invoke(_vPairObject, null));
        }

        public object VersionPair
        {
            get
            {
                return _vPairObject;
            }
        }
#if false
        public static DrtZipArchive OpenOnFile(string path, FileMode mode, FileAccess access, FileShare share, bool streaming)
        {
            object result = _openOnFileMethodInfo.Invoke(null, new object [] {path, mode, access, share, streaming});
            return new DrtZipArchive (result);
        }

        public static DrtZipArchive OpenOnStream(Stream stream, FileMode mode, FileAccess access, bool streaming)
        {
            object result = _openOnStreamMethodInfo.Invoke(null, new object [] {stream, mode, access, streaming});        
            return new DrtZipArchive (result);
        }

        public DrtZipFileInfo AddFile(string zipFileName, DrtCompressionMethodEnum compressionMethod, DrtDeflateOptionEnum defalteOption)
        {
            object result = _addFileMethodInfo.Invoke(_zipArchiveObject, new object [] {zipFileName, compressionMethod, defalteOption});
            return new DrtZipFileInfo(result);
        }

        public DrtZipFileInfo GetFile(string zipFileName)
        {
            object result = _getFileMethodInfo.Invoke(_zipArchiveObject, new object [] {zipFileName});
            return new DrtZipFileInfo(result);
        }

        public bool FileExists (string zipFileName)
        {
            object result = _fileExistsMethodInfo.Invoke(_zipArchiveObject, new object [] {zipFileName});
            return (bool)result;
        }

        public void DeleteFile (string zipFileName)
        {
             _deleteFileMethodInfo.Invoke(_zipArchiveObject, new object [] {zipFileName});
         }

        public DrtZipFileInfoCollection GetFiles()
        {
            object result = _getFilesMethodInfo.Invoke(_zipArchiveObject, null);
            return new DrtZipFileInfoCollection((IEnumerable)result);
        }
        
        public void Flush()
        {
             _flushMethodInfo.Invoke(_zipArchiveObject, null);
        }
        
        public void Close()
        {
             _closeMethodInfo.Invoke(_zipArchiveObject, null);
        }
        
        public void Dispose()
        {
             _disposeMethodInfo.Invoke(_zipArchiveObject, null);
        }
    
        //------------------------------------------------------
        //
        //  Internal Properties
        //
        //------------------------------------------------------


        public FileAccess OpenAccess
        {
            get
            {
                object result = _openAccesPropertyInfo.GetValue(_zipArchiveObject, null);
                return (FileAccess)result;
            }
        }

        public UInt16 CentralDirectoryCount
        {
            get
            {
                object result = _centralDirectoryCountPropertyInfo.GetValue(_zipArchiveObject, null);
                return (UInt16)result;
            }
        }                            

        public string Comment
        {
            get
            {
                object result = _commentPropertyInfo.GetValue(_zipArchiveObject, null);
                return (string)result;
            }
            set
            {
                _commentPropertyInfo.SetValue(_zipArchiveObject, value, null);
            }
        }
#endif
        private object _vPairObject = null;

        //  get WindowsBase.dll into memory 
        Uri _uriRes = PackUriHelper.CreatePartUri(new Uri("a", UriKind.Relative));

        private static string s_versionPairClassName = "MS.Internal.IO.Packaging.CompoundFile.VersionPair";
        public static Type _versionPairClassType = typeof(System.Windows.Rect).Assembly.GetType(s_versionPairClassName);

//        [ComVisibleAttribute(true)] public ConstructorInfo GetConstructor ( BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers ) 
        private static ConstructorInfo s_constructorInfo = _versionPairClassType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase, null,
            new Type[] { typeof(Int16), typeof(Int16) }, null);
        private static MethodInfo s_compareToMethodInfo = _versionPairClassType.GetMethod("CompareTo", BindingFlags.Instance | BindingFlags.NonPublic);
        private static MethodInfo s_toStringMethodInfo = _versionPairClassType.GetMethod("ToString", BindingFlags.Instance | BindingFlags.NonPublic);

        private static PropertyInfo s_majorPropertyInfo = _versionPairClassType.GetProperty("Major", BindingFlags.Instance | BindingFlags.NonPublic);
        private static PropertyInfo s_minorPropertyInfo = _versionPairClassType.GetProperty("Minor", BindingFlags.Instance | BindingFlags.NonPublic);

#if false
        private static MethodInfo _openOnStreamMethodInfo = _versionPairClassType.GetMethod("OpenOnStream",  BindingFlags.Static | BindingFlags.NonPublic );
        private static MethodInfo _addFileMethodInfo = _versionPairClassType.GetMethod("AddFile",  BindingFlags.Instance | BindingFlags.NonPublic );
        private static MethodInfo _getFileMethodInfo = _versionPairClassType.GetMethod("GetFile",  BindingFlags.Instance | BindingFlags.NonPublic );
        private static MethodInfo _fileExistsMethodInfo = _versionPairClassType.GetMethod("FileExists",  BindingFlags.Instance | BindingFlags.NonPublic );
        private static MethodInfo _deleteFileMethodInfo = _versionPairClassType.GetMethod("DeleteFile",  BindingFlags.Instance | BindingFlags.NonPublic );
        private static MethodInfo _getFilesMethodInfo = _versionPairClassType.GetMethod("GetFiles",  BindingFlags.Instance | BindingFlags.NonPublic );
        private static MethodInfo _flushMethodInfo = _versionPairClassType.GetMethod("Flush",  BindingFlags.Instance | BindingFlags.NonPublic );
        private static MethodInfo _closeMethodInfo = _versionPairClassType.GetMethod("Close",  BindingFlags.Instance | BindingFlags.NonPublic );
        private static MethodInfo _disposeMethodInfo = _versionPairClassType.GetMethod("Dispose", BindingFlags.Instance | BindingFlags.Public);

        private static PropertyInfo _openAccesPropertyInfo = _versionPairClassType.GetProperty("OpenAccess",  BindingFlags.Instance | BindingFlags.NonPublic );
        private static PropertyInfo _centralDirectoryCountPropertyInfo = _versionPairClassType.GetProperty("CentralDirectoryCount",  BindingFlags.Instance | BindingFlags.NonPublic );
        private static PropertyInfo _commentPropertyInfo = _versionPairClassType.GetProperty("Comment",  BindingFlags.Instance | BindingFlags.NonPublic );
#endif
    }
}
