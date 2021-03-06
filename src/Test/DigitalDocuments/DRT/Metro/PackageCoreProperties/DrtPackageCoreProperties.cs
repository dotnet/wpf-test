// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.IO;
using System.Diagnostics;           // For Debug.Assert
using System.Globalization;         // For DateTimeFormatInfo
using System.IO.Packaging;
using System.Xml;                   // For XmlConvert

public class DrtPackageCoreProperties
{
    public static void Main()
    {
        Console.WriteLine("Package Core Properties DRT starting [Microsoft]...");
        DrtPackageCoreProperties testInstance = new DrtPackageCoreProperties();
        testInstance.RunTest();

        // If we made it this far, the test was successful.
        Console.WriteLine("SUCCESS");
        Console.Error.WriteLine("SUCCESS");
        Console.Error.Flush();
        testInstance.Cleanup();

        // By default, status code returned to environment is 0.
    }

    /// <summary>
    /// Constructor. Direct standard error to DrtPackageCoreProperties.log.
    /// </summary>
    private DrtPackageCoreProperties()
    {
        _stderr = Console.Error;
        Console.SetError(new StreamWriter(
            new FileStream("DrtPackageCoreProperties.log", FileMode.Create, FileAccess.Write)));
    }

    /// <summary>
    /// Return 'executionStatus' to the environment after dumping the log file to stderr.
    /// </summary>
    private void ExitWithError(int executionStatus)
    {
        Debug.Assert(executionStatus != 0);
        DumpLogToConsole();
        Cleanup();
        Environment.Exit(executionStatus);
    }

    /// <summary>
    /// Dump the content of the log file to stderr.
    /// </summary>
    private void DumpLogToConsole()
    {
        Console.Error.Flush();
        using (StreamReader messageStream = new StreamReader(
            new FileStream("DrtPackageCoreProperties.log", FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
        {
            string messages = messageStream.ReadToEnd();
            _stderr.Write(messages);
        }
    }

    /// <summary>
    /// Delete any temporary file(s) used by the test.
    /// </summary>
    private void Cleanup()
    {
        _package.Close();
        File.Delete(_packagePath);
    }

    private void RunTest()
    {
        // Make sure an empty package returns null for any property value.
        CreateEmptyPackage();
        _package = Package.Open(_packagePath, FileMode.Open, FileAccess.ReadWrite);
        ReadSamplePropertiesAndValidate(null, null, null, null);

        // Set one property of each possible type and reread it.
        string titleValue = "War and Peace";

        DateTime dateCreatedValue = XmlConvert.ToDateTime("2005-07-12T01:13:56.1415926-08:00", _dateTimeFormats);
        DateTime dateModifiedValue = XmlConvert.ToDateTime("2005-07-12T01:13:56.1415926Z", _dateTimeFormats);
        DateTime lastPrintedValue = XmlConvert.ToDateTime("2005-07-12T01:13:56.1415926", _dateTimeFormats);
        SetSampleProperties(titleValue, dateCreatedValue, dateModifiedValue, lastPrintedValue);
        ReadSamplePropertiesAndValidate(titleValue, dateCreatedValue, dateModifiedValue, lastPrintedValue);

        // Reread it after flushing and reopening the package.
        _package.Close();
        _package = Package.Open(_packagePath, FileMode.Open, FileAccess.ReadWrite);
        ReadSamplePropertiesAndValidate(titleValue, dateCreatedValue, dateModifiedValue, lastPrintedValue);

        // Delete the properties by setting them to null.
        SetSampleProperties(null, null, null, null);

        // Make sure we get null from in-core structure and flushed package.
        ReadSamplePropertiesAndValidate(null, null, null, null);
        _package.Close();
        _package = Package.Open(_packagePath, FileMode.Open, FileAccess.ReadWrite);
        ReadSamplePropertiesAndValidate(null, null, null, null);
        _package.Close();

        // Following we test the globalization capability.
        Console.Error.WriteLine("Testing Globalization...");

        for (int i = 0; i < _globalStrings.Length; i++)
        {
            TestGlobalization(_globalStrings[i]);
        }
    }

    private void CreateEmptyPackage()
    {
        Package.Open(_packagePath, FileMode.Create, FileAccess.ReadWrite).Close();
    }

    private void SetSampleProperties(string titleValue,
        Nullable<DateTime> dateCreatedValue,
        Nullable<DateTime> dateModifiedValue,
        Nullable<DateTime> lastPrintedValue)
    {
        PackageProperties properties = _package.PackageProperties;

        properties.Title = titleValue;
        properties.Created = dateCreatedValue;
        properties.Modified = dateModifiedValue;
        properties.LastPrinted = lastPrintedValue;
    }

    private void ReadSamplePropertiesAndValidate(string titleValue,
        Nullable<DateTime> dateCreatedValue,
        Nullable<DateTime> dateModifiedValue,
        Nullable<DateTime> lastPrintedValue)
    {
        PackageProperties properties = _package.PackageProperties;

        ValidateStringValue("title", properties.Title, titleValue);
        ValidateDateTimeValue("created", properties.Created, dateCreatedValue);
        ValidateDateTimeValue("modified", properties.Modified, dateModifiedValue);
        ValidateDateTimeValue("lastPrinted", properties.LastPrinted, lastPrintedValue);
    }

    private void ValidateStringValue(string propertyName, string observedValue, string expectedValue)
    {
        if (observedValue != expectedValue)
        {
            Console.Error.WriteLine("Expected {0} Value: {1}. Observed value: {2}.",
                propertyName, expectedValue, observedValue);
            ExitWithError(1);
        }
    }

    private void ValidateDateTimeValue(
        string propertyName,
        Nullable<DateTime> observedValue,
        Nullable<DateTime> expectedValue)
    {
        if (observedValue != expectedValue)
        {
            Console.Error.WriteLine("Expected {0} Value: {1}. Observed value: {2}.",
                propertyName, expectedValue, observedValue);
            ExitWithError(1);
        }
    }

    private void TestGlobalization(string currentString)
    {
        Console.Error.WriteLine("Input String: " + currentString);

        _package = Package.Open(_packagePath, FileMode.Open, FileAccess.ReadWrite);
        PackageProperties properties = _package.PackageProperties;

        try
        {
            properties.Category = currentString;
            properties.Title = currentString;
            properties.ContentStatus = currentString;

            // ContentType must be "type/subtype", reqiured by the internal PackageProperties.
			// So we don't test it.
            //properties.ContentType = "type/subtype";

            properties.Description = currentString;
            properties.Identifier = currentString;
            properties.Keywords = currentString;
            properties.Revision = currentString;
            properties.Version = currentString;
            properties.Creator = currentString;
            properties.Language = currentString;
            properties.Subject = currentString;
            properties.LastModifiedBy = currentString;

            // Test if it matches
            if (!CheckCFCoreProperties(currentString))
            {
                Console.Error.WriteLine("Globlization Test failed!");
                ExitWithError(1);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("**************************************************");
            Console.Error.WriteLine("FAILURE: in CF Metadata :" + ex.Message);
            Console.Error.WriteLine("**************************************************");
            ExitWithError(1);
        }

        _package.Close();
    }

    private bool CheckCFCoreProperties(string actualString)
    {
        PackageProperties myCoreProperties = _package.PackageProperties;
        if (myCoreProperties.Creator != actualString)
        {
            Console.Error.WriteLine("  Expected: " + actualString + " Creator  Actual  : " + myCoreProperties.Creator);
            return false;
        }
        if (myCoreProperties.Category != actualString)
        {
            Console.Error.WriteLine("  Expected: " + actualString + " Category  Actual  : " + myCoreProperties.Category);
            return false;
        }
        if (myCoreProperties.Title != actualString)
        {
            Console.Error.WriteLine("  Expected: " + actualString + " Title  Actual  : " + myCoreProperties.Title);
            return false;
        }
        if (myCoreProperties.ContentStatus != actualString)
        {
            Console.Error.WriteLine("  Expected: " + actualString + " ContentStatus  Actual  : " + myCoreProperties.ContentStatus);
            return false;
        }

        if (myCoreProperties.LastModifiedBy != actualString)
        {
            Console.Error.WriteLine("  Expected: " + actualString + " LastModifiedBy  Actual  : " + myCoreProperties.ContentStatus);
            return false;
        }

        if (myCoreProperties.Description != actualString)
        {
            Console.Error.WriteLine("  Expected: " + actualString + " Description  Actual  : " + myCoreProperties.Description);
            return false;
        }
        if (myCoreProperties.Identifier != actualString)
        {
            Console.Error.WriteLine("  Expected: " + actualString + "  Identifier Actual  : " + myCoreProperties.Identifier);
            return false;
        }
        if (myCoreProperties.Keywords != actualString)
        {
            Console.Error.WriteLine("  Expected: " + actualString + "  KeyWords  : " + myCoreProperties.Keywords);
            return false;
        }
        if (myCoreProperties.Revision != actualString)
        {
            Console.Error.WriteLine("  Expected: " + actualString + " Revision  Actual  : " + myCoreProperties.Revision);
            return false;
        }
        if (myCoreProperties.Subject != actualString)
        {
            Console.Error.WriteLine("  Expected: " + actualString + "   Subject Actual  : " + myCoreProperties.Subject);
            return false;
        }
        if (myCoreProperties.Version != actualString)
        {
            Console.Error.WriteLine("  Expected: " + actualString + "  Version Actual  : " + myCoreProperties.Version);
            return false;
        }
        if (myCoreProperties.Language != actualString)
        {
            Console.Error.WriteLine("  Expected: " + actualString + " Language  Actual  : " + myCoreProperties.Language);
            return false;
        }

        return true;

    }

    private Package _package;
    private string _packagePath = "PackageCorePropertiesTestPackage.xps";
    private TextWriter _stderr;

    private readonly static string[] _dateTimeFormats = new string[] {
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-ddTHH:mm:ssZ",
            "yyyy-MM-ddTHH:mm:sszzz",
            @"\-yyyy-MM-ddTHH:mm:ss",
            @"\-yyyy-MM-ddTHH:mm:ssZ",
            @"\-yyyy-MM-ddTHH:mm:sszzz",

            "yyyy-MM-ddTHH:mm:ss.ff",
            "yyyy-MM-ddTHH:mm:ss.fZ",
            "yyyy-MM-ddTHH:mm:ss.fzzz",
            @"\-yyyy-MM-ddTHH:mm:ss.f",
            @"\-yyyy-MM-ddTHH:mm:ss.fZ",
            @"\-yyyy-MM-ddTHH:mm:ss.fzzz",

            "yyyy-MM-ddTHH:mm:ss.ff",
            "yyyy-MM-ddTHH:mm:ss.ffZ",
            "yyyy-MM-ddTHH:mm:ss.ffzzz",
            @"\-yyyy-MM-ddTHH:mm:ss.ff",
            @"\-yyyy-MM-ddTHH:mm:ss.ffZ",
            @"\-yyyy-MM-ddTHH:mm:ss.ffzzz",

            "yyyy-MM-ddTHH:mm:ss.fff",
            "yyyy-MM-ddTHH:mm:ss.fffZ",
            "yyyy-MM-ddTHH:mm:ss.fffzzz",
            @"\-yyyy-MM-ddTHH:mm:ss.fff",
            @"\-yyyy-MM-ddTHH:mm:ss.fffZ",
            @"\-yyyy-MM-ddTHH:mm:ss.fffzzz",

            "yyyy-MM-ddTHH:mm:ss.ffff",
            "yyyy-MM-ddTHH:mm:ss.ffffZ",
            "yyyy-MM-ddTHH:mm:ss.ffffzzz",
            @"\-yyyy-MM-ddTHH:mm:ss.ffff",
            @"\-yyyy-MM-ddTHH:mm:ss.ffffZ",
            @"\-yyyy-MM-ddTHH:mm:ss.ffffzzz",

            "yyyy-MM-ddTHH:mm:ss.fffff",
            "yyyy-MM-ddTHH:mm:ss.fffffZ",
            "yyyy-MM-ddTHH:mm:ss.fffffzzz",
            @"\-yyyy-MM-ddTHH:mm:ss.fffff",
            @"\-yyyy-MM-ddTHH:mm:ss.fffffZ",
            @"\-yyyy-MM-ddTHH:mm:ss.fffffzzz",

            "yyyy-MM-ddTHH:mm:ss.ffffff",
            "yyyy-MM-ddTHH:mm:ss.ffffffZ",
            "yyyy-MM-ddTHH:mm:ss.ffffffzzz",
            @"\-yyyy-MM-ddTHH:mm:ss.ffffff",
            @"\-yyyy-MM-ddTHH:mm:ss.ffffffZ",
            @"\-yyyy-MM-ddTHH:mm:ss.ffffffzzz",

            "yyyy-MM-ddTHH:mm:ss.fffffff",
            "yyyy-MM-ddTHH:mm:ss.fffffffZ",
            "yyyy-MM-ddTHH:mm:ss.fffffffzzz",
            @"\-yyyy-MM-ddTHH:mm:ss.fffffff",
            @"\-yyyy-MM-ddTHH:mm:ss.fffffffZ",
            @"\-yyyy-MM-ddTHH:mm:ss.fffffffzzz",
        };

    // Put Unicode/ASCII strings here for testing the globalization capability.
    private readonly static string[] _globalStrings = new string[] {
        "đṇ̃óôơö÷øùúûüư₫ÿ°±²³´µ¶·¸¹º»¼½¾¿E0ąįāćäåęēčéźėģķīļ¡¢£¤¥¦§¨©ª«¬­¯ðñòóôõö÷øùúûüýþÿŠŃŅÓŌÕÖ×ŲŁŚŪÜŻŽßıiđṇ̃óôơö÷øùúûüư₫ÿ°±²³´µ¶·¸¹º»¼½¾¿E0ąįāćäåęēčéźėģķīļ¡¢£¤¥¦§¨©ª«¬­¯ðñòóôõö÷øùúûüýþÿŠŃŅÓŌÕÖ×ŲŁŚŪÜŻŽßıi",
        "您好！",
        "简体中文字",
        "Hello!",
        "@#$%^&!)()+_}{|\'><",
        "",
    };
}


