// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.IO;
using System.Windows.Baml2006;
using System.Xaml;
using System.Xml;

namespace Microsoft.Test.Xaml.Utilities
{
    /******************************************************************************
    * CLASS:          InfosetFactory
    ******************************************************************************/

    /// <summary>
    /// Class for creating an Infoset from .xaml or .baml files.
    /// </summary>
    public class InfosetFactory
    {
        #region Public and Protected Members

        /******************************************************************************
        * Function:          GenerateInfosetFromBaml
        ******************************************************************************/

        /// <summary>
        /// Generates an Infoset (.DiagBaml) from a .baml file using a Baml2006Reader.
        /// </summary>
        /// <param name="bamlFileName">Name of the baml file from which the Infoset is generated.</param>
        /// <param name="valuesMustBeString">Indicates setting of XamlReaderSettings.ValuesMustBeString.</param>
        /// <returns>string value</returns>
        public static string GenerateInfosetFromBaml(string bamlFileName, bool valuesMustBeString)
        {
            byte[] bamlData;
            using (FileStream fileStream = new FileStream(bamlFileName, FileMode.Open, FileAccess.Read))
            {
                if (fileStream == null)
                {
                    throw new TestSetupException("ERROR: fileStream returned null.");
                }

                bamlData = new byte[fileStream.Length];
#pragma warning disable CA2022 // Avoid inexact read
                fileStream.Read(bamlData, 0, Convert.ToInt32(fileStream.Length));
#pragma warning restore CA2022
            }

            string diagOutputBaml = String.Empty;
            using (MemoryStream memStream = new MemoryStream(bamlData))
            {
                if (memStream == null)
                {
                    throw new TestSetupException("ERROR: memStream returned null.");
                }

                XamlReaderSettings settings = new XamlReaderSettings();

                // When valuesMustBeString is true, the Infoset created is a BRAT ("Baml Reader as Text") version
                // which has few optimizations.  Therefore, it is close to the Infoset created from Xaml.
                settings.ValuesMustBeString = valuesMustBeString;
                Baml2006Reader baml2006Reader = new Baml2006Reader(memStream, settings);
                if (baml2006Reader == null)
                {
                    throw new TestSetupException("ERROR: baml2006Reader returned null.");
                }

                using (TextWriter textWriter = new StringWriter(CultureInfo.InvariantCulture))
                {
                    if (textWriter == null)
                    {
                        throw new TestSetupException("ERROR: textWriter returned null.");
                    }

                    using (DiagnosticWriter diagWriterBaml = new DiagnosticWriter(textWriter, baml2006Reader.SchemaContext))
                    {
                        if (diagWriterBaml == null)
                        {
                            throw new TestSetupException("ERROR: diagWriterBaml returned null.");
                        }

                        XamlServices.Transform(baml2006Reader, diagWriterBaml);

                        diagOutputBaml = textWriter.ToString();
                        if (String.IsNullOrEmpty(diagOutputBaml))
                        {
                            throw new TestSetupException("ERROR: The Infoset failed to be created from Baml.");
                        }
                    }
                }
            }

            return diagOutputBaml;
        }

        /******************************************************************************
        * Function:          GenerateInfosetFromXaml
        ******************************************************************************/

        /// <summary>
        /// Generates an Infoset (.DiagXaml) from a .xaml file.
        /// </summary>
        /// <param name="xamlFileName">Name of the xaml file.</param>
        /// <returns>string value</returns>
        public static string GenerateInfosetFromXaml(string xamlFileName)
        {
            TextWriter textWriter = new StringWriter(CultureInfo.InvariantCulture);
            if (textWriter == null)
            {
                throw new TestSetupException("ERROR: textWriter returned null.");
            }

            XmlReader xmlReader = XmlReader.Create(xamlFileName);
            if (xmlReader == null)
            {
                throw new TestSetupException("ERROR: xmlReader returned null.");
            }

            XamlXmlReader xamlTextReader = new XamlXmlReader(xmlReader);
            if (xamlTextReader == null)
            {
                throw new TestSetupException("ERROR: xamlTextReader returned null.");
            }

            DiagnosticWriter diagWriterXaml = new DiagnosticWriter(textWriter, xamlTextReader.SchemaContext);
            if (diagWriterXaml == null)
            {
                throw new TestSetupException("ERROR: diagnosticWriter returned null.");
            }

            diagWriterXaml.FromMemberText = "Retrieved";

            XamlServices.Transform(xamlTextReader, diagWriterXaml);

            string diagOutputXaml = textWriter.ToString();
            if (String.IsNullOrEmpty(diagOutputXaml))
            {
                throw new TestSetupException("ERROR: The Infoset failed to be created from Xaml.");
            }

            return diagOutputXaml;
        }

        #endregion
    }
}
