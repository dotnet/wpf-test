// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides methods to manipulate XAML content.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Utils/XamlUtils.cs $")]

namespace Test.Uis.Utils
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Runtime.InteropServices;
    using Test.Uis.IO;
    using System.IO;
    using Test.Uis.Utils;
    using Test.Uis.Management;
    using System.Security;
    using System.Security.Permissions;

    #endregion Namespaces.

    /// <summary>
    /// Provides methods to manipulate XAML content.
    /// </summary>
    /// <remarks>
    /// Includes the ability to replace escaped strings with contents from
    /// configuration settings.
    /// </remarks>
    [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
    public static class XamlUtils
    {
        #region Public methods.

        /// <summary>
        /// Returns the contents of a XAML file, possibly from a cache,
        /// without any escaping process.
        /// </summary>
        /// <param name='fileName'>Name of the XAML file.</param>
        /// <returns>The original contents of the file.</returns>
        /// <remarks>
        /// This static method will return the file as it was first requested,
        /// which allows test cases to re-read it even after modifying the
        /// file on disk.
        /// </remarks>
        public static string GetXamlFileContents(string fileName)
        {
            string result;
            result = (string) s_xamlCache[fileName];

            // If the result has not been cache, read it from the file system
            // and add it to the cache.
            if (result == null)
            {
                result = TextFileUtils.LoadFromFile(fileName);
                s_xamlCache[fileName] = result;
            }
            return result;
        }

        /// <summary>
        /// Parses the content of the XAML text into an object,
        /// possibly with nested objects. Note that no replacements
        /// take place.
        /// </summary>
        /// <param name='xamlText'>Text to parse.</param>
        /// <returns>A newly constructed object.</returns>
        public static object ParseToObject(string xamlText)
        {
            new System.Security.Permissions.FileIOPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();

            if (xamlText == null)
            {
                throw new ArgumentNullException("xamlText");
            }
            if (xamlText.Length == 0)
            {
                throw new ArgumentException("xamlText");
            }
            const bool ignorePreamble = false;
            System.Text.Encoding encoding = new System.Text.UnicodeEncoding();
            StringStream stream = new StringStream(xamlText, encoding, ignorePreamble);
            return System.Windows.Markup.XamlReader.Load(stream);
        }

        /// <summary>
        /// return the current directory.
        /// This should be removed to other place later.
        /// </summary>
        public static string CurrentDirectory
        {
            get
            {
                new System.Security.Permissions.EnvironmentPermission(
                    System.Security.Permissions.PermissionState.Unrestricted)
                    .Assert();
                return System.Environment.CurrentDirectory; 
            }
        }

        /// <summary>
        /// return the System directory.
        /// This should be removed to other place later.
        /// </summary>
        public static string SystemDirectory
        {
            get
            {
                new System.Security.Permissions.FileIOPermission(
                    System.Security.Permissions.PermissionState.Unrestricted)
                    .Assert();
                return System.Environment.SystemDirectory;
            }
        }

        /// <summary>
        /// Retrieves the XAML for content with replacements from
        /// the current configuration settings.
        /// </summary>
        /// <param name='text'>XAML text to replace.</param>
        /// <returns>The number of replaced strings.</returns>
        /// <remarks><p>
        /// Creates the new text replacing strings between
        /// '$$' characters with the value of the strings in
        /// the ConfigurationSettings object.
        /// </p><p>
        /// The resulting text is returned in the text argument.
        /// </p>
        /// </remarks>
        /// <example>The following sample shows how to use this method.<code>...
        /// public void MyMethod() {
        ///   string xamlSnippet = "TextBox MyProperty="$$MySetting$$" /...";
        ///   if (XamlUtils.ReplaceEscapedXaml(xamlSnippet) == 0)
        ///     Console.WriteLine("No replacements were made.");
        ///   // use xamlSnippet
        /// }
        /// </code></example>
        public static int ReplaceEscapedXaml(ref string text)
        {
            const string Delimiters = "$$";

            int replacementCount = 0;
            int position = 0;
            while (position <= text.Length)
            {
                // Look for the position of the first delimiters string.
                int firstPosition = text.IndexOf(Delimiters, position);
                if (firstPosition < 0) break;
                if (firstPosition == text.Length - Delimiters.Length) break;

                // If the delimiter is doubled, then remove an instance. This
                // acts as an escape to insert the delimiter as a literal.
                if (text.Substring(firstPosition + Delimiters.Length, Delimiters.Length) == Delimiters)
                {
                    text.Remove(firstPosition, Delimiters.Length);
                    position = firstPosition + Delimiters.Length;
                    continue;
                }

                // Look for the position of the second percentage char.
                int secondPosition = text.IndexOf(Delimiters,
                    firstPosition + Delimiters.Length);
                if (secondPosition < 0) break;

                // Replace the name and adjust the position accordingly.
                replacementCount++;
                int nameLength = secondPosition - firstPosition - Delimiters.Length;
                string name = text.Substring(firstPosition + Delimiters.Length, nameLength);
                string nameValue = ConfigurationSettings.Current.GetArgument(name);
                text =
                    text.Substring(0, firstPosition) +
                    nameValue +
                    text.Substring(secondPosition + Delimiters.Length);

                // Skip replaced text.
                position = firstPosition + nameValue.Length;
            }
            return replacementCount;
        }

        /// <summary>
        /// Set xml to TextRange.Xml property.
        /// </summary>
        /// <param name="range">TextRange</param>
        /// <param name="xaml">Xaml to be set</param>
        public static void TextRange_SetXml(TextRange range, string xaml)
        {
            MemoryStream mstream; 
            if (null == xaml)
            {
                throw new ArgumentNullException("xaml");
            }
            if (range == null)
            {
                throw new ArgumentNullException("range");
            }

            mstream = new MemoryStream();
            StreamWriter sWriter = new StreamWriter(mstream);

            mstream.Seek(0, SeekOrigin.Begin); //this line may not be needed.
            sWriter.Write(xaml);
            sWriter.Flush();

            //move the stream pointer to the beginning. 
            mstream.Seek(0, SeekOrigin.Begin);
            
            range.Load(mstream, DataFormats.Xaml);
        }

        /// <summary>
        /// Get xaml from TextRange.Xml property
        /// </summary>
        /// <param name="range">TextRange</param>
        /// <returns>return a string serialized from the TextRange</returns>
        public static string TextRange_GetXml(TextRange range)
        {
            MemoryStream mstream;

            if (range == null)
            {
                throw new ArgumentNullException("range");
            }
            
            mstream = new MemoryStream();
            range.Save(mstream, DataFormats.Xaml);

            //must move the stream pointer to the beginning since range.save() will move it to the end.
            mstream.Seek(0, SeekOrigin.Begin);

            //Create a stream reader to read the xaml.
            StreamReader stringReader = new StreamReader(mstream);
            
            return stringReader.ReadToEnd();
        }

        /// <summary>Gets Rtf from TextRange through Save API</summary>
        /// <param name="range">TextRange whose Rtf is to be retrieved</param>
        /// <returns>rtf string converted from xaml of range</returns>
        public static string GetRtfFromTextRange(TextRange range)
        {
            MemoryStream mstream;

            if (range == null)
            {
                throw new ArgumentNullException("range");
            }

            mstream = new MemoryStream();
            range.Save(mstream, DataFormats.Rtf);

            //must move the stream pointer to the beginning since range.save() will move it to the end.
            mstream.Seek(0, SeekOrigin.Begin);

            //Create a stream reader to read the rtf.
            StreamReader stringReader = new StreamReader(mstream);

            return stringReader.ReadToEnd();
        }

        /// <summary>Restores all xaml files that have been replaced.</summary>
        /// <remarks>
        /// This will only work from the test manager, which holds the
        /// real cache and is the most stable component. For standalone
        /// application, this will hold the real cache and continue to work.
        /// This call is only required if the files on disk are modified.
        /// </remarks>
        /// <example>The following sample shows how to use this method.<code>...
        /// public class MyTest {
        ///   ...
        ///   public void DoTest() {
        ///     string contents = XamlUtils.GetXamlFileContents("file.xaml");
        ///     XamlUtils.ReplaceEscapedXaml(ref contents);
        ///     TextFileUtils.SaveToFile(contents, "file.xaml");
        ///     // Use files directly off-disk, eg by navigating to them.
        ///     ...
        ///     XamlUtils.RestoreXamlFiles();
        ///   }
        /// }
        /// </code></example>
        public static void RestoreXamlFiles()
        {
            foreach(System.Collections.DictionaryEntry de in s_xamlCache)
            {
                string fileName = (string) de.Key;
                string contents = (string) de.Value;
                TextFileUtils.SaveToFile(contents, fileName);
            }
        }

        #region TextOM-specific XAML methods.

        /// <summary>
        /// Appends the specified XAML content to a TextRange, without
        /// requiring the default namesapce declarations or the
        /// TextRange markers.
        /// </summary>
        public static void SetXamlContent(TextRange range, string xamlContent)
        {
            if (range == null)
            {
                throw new ArgumentNullException("range");
            }
            if (xamlContent == null)
            {
                throw new ArgumentNullException("xamlContent");
            }
            if (!(xamlContent.StartsWith("<Paragraph") || xamlContent.StartsWith("<List") || xamlContent.StartsWith("<Table")))
            {
                xamlContent = "<Paragraph>" + xamlContent + "</Paragraph>";
            }

            xamlContent = "<Section xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xml:space='preserve'>" + 
                StartRangeMarker + xamlContent + EndRangeMarker +
                "</Section>";
            XamlUtils.TextRange_SetXml(range, xamlContent);
        }

        /// <summary>
        /// Verifies that the specified TextRange is serialized correctly.
        /// </summary>
        /// <param name='range'>TextRange to verify.</param>
        /// <param name='expectedRangeXaml'>Expected text between start and end markers.</param>
        public static void VerifyTextRangeXaml(TextRange range, string expectedRangeXaml)
        {
            if (range == null)
            {
                throw new ArgumentNullException("range");
            }
            if (expectedRangeXaml == null)
            {
                throw new ArgumentNullException("expectedRangeXaml");
            }

            string serializedRange = TextRange_GetXml(range);
            int startIndex = serializedRange.IndexOf(StartRangeMarker) + StartRangeMarker.Length;
            int endIndex = serializedRange.IndexOf(EndRangeMarker);
            int rangeLength = endIndex - startIndex;
            string rangeXaml = serializedRange.Substring(startIndex, rangeLength);
            if (expectedRangeXaml != rangeXaml)
            {
                string message =
                    "Expecting XAML range [" + expectedRangeXaml +
                    "] but found [" + rangeXaml + "].";
                throw new Exception(message);
            }
        }

        #endregion TextOM-specific XAML methods.

        #endregion Public methods.

        #region Private properties.

        private const string StartRangeMarker = "<!--StartFragment-->";
        private const string EndRangeMarker = "<!--EndFragment-->";

        /// <summary>
        /// Keeps a static cache of original XAML file contents.
        /// </summary>
        private static System.Collections.Hashtable s_xamlCache =
            new System.Collections.Hashtable();

        #endregion Private properties.
    }
}