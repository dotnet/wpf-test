// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Baml2006;
using System.Xaml;
using System.Xml;

namespace Microsoft.Test.Xaml.TestTypes
{
    /******************************************************************************
    * CLASS:          XamlFromBamlFactory
    ******************************************************************************/

    /// <summary>
    /// Class for generating Xaml from a .baml file.
    /// </summary>
    public class XamlFromBamlFactory
    {
        #region Public and Protected Members

        /******************************************************************************
        * Function:          GenerateXamlFromBaml
        ******************************************************************************/

        /// <summary>
        /// Generate a Xaml file using the Baml2006Reader.
        /// </summary>
        /// <param name="bamlFileName">The name of the .baml file.</param>
        /// <returns>The generated xaml string</returns>
        public static string GenerateXamlFromBaml(string bamlFileName)
        {
            FileStream fileStream = new FileStream(bamlFileName, FileMode.Open, FileAccess.Read);
            byte[] bamlData = new byte[fileStream.Length];
#pragma warning disable CA2022 // Avoid inexact read
            fileStream.Read(bamlData, 0, Convert.ToInt32(fileStream.Length));
#pragma warning restore CA2022
            fileStream.Close();

            MemoryStream memStream = new MemoryStream(bamlData);

            XamlReaderSettings settings = new XamlReaderSettings();
            settings.ValuesMustBeString = true;  // The BamlToXaml scenario only works with BRAT.

            Baml2006Reader reader = new Baml2006Reader(memStream, settings);

            XamlXmlWriterSettings writerSettings = new XamlXmlWriterSettings();
            writerSettings.CloseOutput = true;

            TextWriter textWriter = new StringWriter(CultureInfo.InvariantCulture);
            XmlTextWriter xmlWriter = new XmlTextWriter(textWriter);
            xmlWriter.Formatting = Formatting.Indented;
            XamlXmlWriter writer = new XamlXmlWriter(xmlWriter, reader.SchemaContext);

            XamlServices.Transform(reader, writer);

            return textWriter.ToString();
        }

        #endregion
    }
}
