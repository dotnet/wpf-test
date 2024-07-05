// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace DRT
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// XamlRtfConverter is a static class that convert from/to rtf string to/from xaml string.
    /// </summary>
    internal static class XamlRtfConverter
    {
        #region Internal Methods

        /// <summary>
        /// Converts an xaml string into rtf string.
        /// </summary>
        /// <param name="xamlString">
        /// Input xaml string.
        /// </param>
        /// <returns>
        /// Well-formed representing rtf equivalent for the input xaml string.
        /// </returns>
        internal static string ConvertXamlToRtf(string xamlContent)
        {
            // Get XamlRtfConverter type and get the internal ConvertXamlToRtf method
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly(typeof(System.Windows.FrameworkElement));
            Type xamlRtfConverterType = assembly.GetType("System.Windows.Documents.XamlRtfConverter");

            // Create an instance of XamlToRtfConverter class
            object xamlRtfConverter = Activator.CreateInstance(xamlRtfConverterType, /*nonPublic:*/true);

            // Converts xaml to rtf by using XamlRtfConverter
            System.Reflection.MethodInfo convertXamlToRtf = xamlRtfConverterType.GetMethod("ConvertXamlToRtf", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            string rtfContent = (string)convertXamlToRtf.Invoke(xamlRtfConverter, new object[] { xamlContent });

            // Return rtf content as string
            return rtfContent;
        }

        /// <summary>
        /// Converts an rtf string into xaml string.
        /// </summary>
        /// <param name="rtfString">
        /// Input rtf string.
        /// </param>
        /// <returns>
        /// Well-formed xml representing XAML equivalent for the input rtf string.
        /// </returns>
        internal static string ConvertRtfToXaml(BinaryReader rtfBinaryReader)
        {
            // Get XamlRtfConverter type and get the internal ConvertRtfToXaml method
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly(typeof(System.Windows.FrameworkElement));
            Type xamlRtfConverterType = assembly.GetType("System.Windows.Documents.XamlRtfConverter");

            // Create an instance of XamlToRtfConverter class
            object xamlRtfConverter = Activator.CreateInstance(xamlRtfConverterType, /*nonPublic:*/true);

            // Converts xaml from rtf by using XamlRtfConverter
            System.Reflection.MethodInfo convertRtfToXaml = xamlRtfConverterType.GetMethod("ConvertRtfToXaml", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            // Get rtf bytes from rtfBinaryReader
            byte[] rtfBytes = new byte[rtfBinaryReader.BaseStream.Length];
#pragma warning disable CA2022 // Avoid inexact read
            rtfBinaryReader.BaseStream.Read(rtfBytes, 0, rtfBytes.Length);
#pragma warning restore CA2022
            Encoding ansiEncoding = Encoding.GetEncoding(RtfCodePage);

            // Get rtf content string
            string rtfContent = ansiEncoding.GetString(rtfBytes);

            // Converts rtf to xaml by using XamlRtfConverter
            string xamlContent = (string)convertRtfToXaml.Invoke(xamlRtfConverter, new object[] { rtfContent });

            // Return xaml content as string
            return xamlContent;
        }

        #endregion Internal Methods

        #region Internal Fields

        internal const int RtfCodePage = 1252;

        #endregion Internal Fields
    }
}