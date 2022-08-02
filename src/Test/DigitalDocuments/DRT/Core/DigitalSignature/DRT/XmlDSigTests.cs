// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// Description:
//  The set of Developer Regression Tests for the Metro XML DigitalSignature internal classes
//
using System;
using System.Xml;
using System.Reflection;
using System.IO.Packaging;      // for PackageDigitalSignatureManager
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Globalization;

namespace DRT
{
/// <summary>
/// Wraps MS.Internal.IO.Packaging.XmlSignatureManifest class
/// </summary>
public class XmlSignatureManifest
{
    public XmlSignatureManifest(PackageDigitalSignatureManager manager, XmlTextWriter writer)
    {
        if (s_xmlTextWriterClassType == null)
        {
            s_xmlTextWriterClassType = writer.GetType();
//            _manifestConstructor = _XmlSignatureManifestClassType.GetConstructor(BindingFlags.NonPublic, null, new Type[] { _PackageDigitalSignatureManagerClassType, _XmlTextWriterClassType }, null);
            s_generateManifest = s_xmlSignatureManifestClassType.GetMethod("GenerateManifest", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        _XmlSignatureManifestObject = Activator.CreateInstance(s_xmlSignatureManifestClassType,
            BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { manager, writer },
            CultureInfo.InvariantCulture, null);
    }

    public void GenerateManifest(HashAlgorithm hashAlgorithm, IEnumerable<Uri> parts, IEnumerable<PackageRelationship> relationships)
    {
        // first arg is null because the method is static
        s_generateManifest.Invoke(_XmlSignatureManifestObject, new object[] { hashAlgorithm, parts, relationships });
    }

    // class
    // this reference to the Uri Helper will ensure loading of the right DLL in memory 
    private System.Uri _u = System.IO.Packaging.PackUriHelper.Create(new Uri("http://www.mic.com/blat.container"), new Uri("/part", UriKind.Relative));

    private static string s_xmlSignatureManifestClassName = "MS.Internal.IO.Packaging.XmlSignatureManifest";
    private static Type s_xmlSignatureManifestClassType = typeof(System.Windows.Rect).Assembly.GetType(s_xmlSignatureManifestClassName);
    private static string s_packageDigitalSignatureManagerClassName = "System.IO.Packaging.PackageDigitalSignatureManager";
    private static Type s_packageDigitalSignatureManagerClassType = typeof(System.Windows.Rect).Assembly.GetType(s_packageDigitalSignatureManagerClassName);
    private static Type s_xmlTextWriterClassType; //= Type.GetType("System.Xml.XmlTextWriter, System.Xml");

    private object _XmlSignatureManifestObject = null;

    // methods
//    private static ConstructorInfo _manifestConstructor; // = _XmlSignatureManifestClassType.GetConstructor(new Type[] { _PackageDigitalSignatureManagerClassType, _XmlTextWriterClassType });
    private static MethodInfo s_generateManifest; // = _XmlSignatureManifestClassType.GetMethod("GenerateManifest", BindingFlags.Instance | BindingFlags.NonPublic);
}

/// <summary>
/// Wraps MS.Internal.IO.Packaging.XmlSignatureProperties class
/// </summary>
public class XmlSignatureProperties
{
    public static XmlWriter GenerateSignatureProperties(XmlWriter writer, DateTime dateTime, String dateTimeFormat)
    {
        // first arg is null because the method is static
        object result = s_generateSignatureProperties.Invoke(null, new object[] { writer, dateTime, dateTimeFormat });
        return (XmlWriter)result;
    }

    public static DateTime ParseSigningTime(XmlTextReader reader, int depthOffset)
    {
        // first arg is null because the method is static
        object result = s_parseSigningTime.Invoke(null, new object[] { reader, depthOffset });
        return (DateTime)result;
    }

    // class
    private static string s_xmlSignaturePropertiesClassName = "MS.Internal.IO.Packaging.XmlSignatureProperties";
    private static Type s_xmlSignaturePropertiesClassType = typeof(System.Windows.Rect).Assembly.GetType(s_xmlSignaturePropertiesClassName);
//    private object _XmlSignaturePropertiesObject = null;

    // methods
    private static MethodInfo s_generateSignatureProperties = s_xmlSignaturePropertiesClassType.GetMethod("GenerateSignatureProperties", BindingFlags.Static | BindingFlags.NonPublic);
    private static MethodInfo s_parseSigningTime = s_xmlSignaturePropertiesClassType.GetMethod("ParseSigningTime", BindingFlags.Static | BindingFlags.NonPublic);

#if false
    public Stream GetStream(FileMode mode, FileAccess access)
    {
        object result = _getStreamMethodInfo.Invoke(_zipFileInfoObject, new object[] { mode, access });
        return (Stream)result;
    }


    public string Name
    {
        get
        {
            object result = _namePropertyInfo.GetValue(_zipFileInfoObject, null);
            return (string)result;
        }
    }

    public DrtZipArchive ZipArchive
    {
        get
        {
            object result = _zipArchivePropertyInfo.GetValue(_zipFileInfoObject, null);
            return new DrtZipArchive(result);
        }
    }

    public DrtCompressionMethodEnum CompressionMethod
    {
        get
        {
            object result = _compressionMethodPropertyInfo.GetValue(_zipFileInfoObject, null);
            return (DrtCompressionMethodEnum)result;
        }
    }

    public DateTime LastModFileDateTime
    {
        get
        {
            object result = _lastModFileDateTimePropertyInfo.GetValue(_zipFileInfoObject, null);
            return (DateTime)result;
        }
    }

    public UInt32 CRC32
    {
        get
        {
            object result = _CRC32PropertyInfo.GetValue(_zipFileInfoObject, null);
            return (UInt32)result;
        }
    }

    public UInt64 CompressedSize
    {
        get
        {
            object result = _compressedSizePropertyInfo.GetValue(_zipFileInfoObject, null);
            return (UInt64)result;
        }
    }

    public UInt64 UncompressedSize
    {
        get
        {
            object result = _uncompressedSizePropertyInfo.GetValue(_zipFileInfoObject, null);
            return (UInt64)result;
        }
    }

    public bool EncryptedFlag
    {
        get
        {
            object result = _encryptedFlagPropertyInfo.GetValue(_zipFileInfoObject, null);
            return (bool)result;
        }
    }

    public DrtDeflateOptionEnum DeflateOption
    {
        get
        {
            object result = _deflateOptionPropertyInfo.GetValue(_zipFileInfoObject, null);
            return (DrtDeflateOptionEnum)result;
        }
    }

    public bool StreamingCreationFlag
    {
        get
        {
            object result = _streamingCreationFlagPropertyInfo.GetValue(_zipFileInfoObject, null);
            return (bool)result;
        }
    }

    public bool EnhancedDeflatingFlag
    {
        get
        {
            object result = _enhancedDeflatingFlagPropertyInfo.GetValue(_zipFileInfoObject, null);
            return (bool)result;
        }
    }

    public byte VersionMadeByPlatform
    {
        get
        {
            object result = _versionMadeByPlatformPropertyInfo.GetValue(_zipFileInfoObject, null);
            return (byte)result;
        }
        set
        {
            _versionMadeByPlatformPropertyInfo.SetValue(_zipFileInfoObject, value, null);
        }
    }

    public byte VersionMadeBy
    {
        get
        {
            object result = _versionMadeByPropertyInfo.GetValue(_zipFileInfoObject, null);
            return (byte)result;
        }
    }

    public UInt32 ExternalFileAttributes
    {
        get
        {
            object result = _externalFileAttributesPropertyInfo.GetValue(_zipFileInfoObject, null);
            return (UInt32)result;
        }
        set
        {
            _externalFileAttributesPropertyInfo.SetValue(_zipFileInfoObject, value, null);
        }
    }

    public string Comment
    {
        get
        {
            object result = _commentPropertyInfo.GetValue(_zipFileInfoObject, null);
            return (string)result;
        }
        set
        {
            _commentPropertyInfo.SetValue(_zipFileInfoObject, value, null);
        }
    }

    internal DrtZipFileInfo(object zipFileInfoObject)
    {
        _zipFileInfoObject = zipFileInfoObject;
    }

    // this reference to the Uri Helper will ensure loading of the right DLL in memory 
    System.Uri u = System.IO.Packaging.PackUriHelper.Create(new Uri("http://www.mic.com/blat.contyainer"), new Uri("/part", UriKind.Relative));

    private object _zipFileInfoObject = null;

    private static string _zipFileInfoClassName = "MS.Internal.IO.Zip.ZipFileInfo";
    private static Type _zipFileInfoClassType = typeof(System.Windows.Rect).Assembly.GetType(_zipFileInfoClassName);

    private static MethodInfo _getStreamMethodInfo = _zipFileInfoClassType.GetMethod("GetStream", BindingFlags.Instance | BindingFlags.NonPublic);

    private static PropertyInfo _namePropertyInfo = _zipFileInfoClassType.GetProperty("Name", BindingFlags.Instance | BindingFlags.NonPublic);
    private static PropertyInfo _zipArchivePropertyInfo = _zipFileInfoClassType.GetProperty("ZipArchive", BindingFlags.Instance | BindingFlags.NonPublic);
    private static PropertyInfo _compressionMethodPropertyInfo = _zipFileInfoClassType.GetProperty("CompressionMethod", BindingFlags.Instance | BindingFlags.NonPublic);
    private static PropertyInfo _lastModFileDateTimePropertyInfo = _zipFileInfoClassType.GetProperty("LastModFileDateTime", BindingFlags.Instance | BindingFlags.NonPublic);
    private static PropertyInfo _CRC32PropertyInfo = _zipFileInfoClassType.GetProperty("CRC32", BindingFlags.Instance | BindingFlags.NonPublic);
    private static PropertyInfo _compressedSizePropertyInfo = _zipFileInfoClassType.GetProperty("CompressedSize", BindingFlags.Instance | BindingFlags.NonPublic);
    private static PropertyInfo _uncompressedSizePropertyInfo = _zipFileInfoClassType.GetProperty("UncompressedSize", BindingFlags.Instance | BindingFlags.NonPublic);
    private static PropertyInfo _encryptedFlagPropertyInfo = _zipFileInfoClassType.GetProperty("EncryptedFlag", BindingFlags.Instance | BindingFlags.NonPublic);
    private static PropertyInfo _deflateOptionPropertyInfo = _zipFileInfoClassType.GetProperty("DeflateOption", BindingFlags.Instance | BindingFlags.NonPublic);
    private static PropertyInfo _streamingCreationFlagPropertyInfo = _zipFileInfoClassType.GetProperty("StreamingCreationFlag", BindingFlags.Instance | BindingFlags.NonPublic);
    private static PropertyInfo _enhancedDeflatingFlagPropertyInfo = _zipFileInfoClassType.GetProperty("EnhancedDeflatingFlag", BindingFlags.Instance | BindingFlags.NonPublic);
    private static PropertyInfo _versionMadeByPlatformPropertyInfo = _zipFileInfoClassType.GetProperty("VersionMadeByPlatform", BindingFlags.Instance | BindingFlags.NonPublic);
    private static PropertyInfo _versionMadeByPropertyInfo = _zipFileInfoClassType.GetProperty("VersionMadeBy", BindingFlags.Instance | BindingFlags.NonPublic);
    private static PropertyInfo _externalFileAttributesPropertyInfo = _zipFileInfoClassType.GetProperty("ExternalFileAttributes", BindingFlags.Instance | BindingFlags.NonPublic);
    private static PropertyInfo _commentPropertyInfo = _zipFileInfoClassType.GetProperty("Comment", BindingFlags.Instance | BindingFlags.NonPublic);
#endif
}
}
