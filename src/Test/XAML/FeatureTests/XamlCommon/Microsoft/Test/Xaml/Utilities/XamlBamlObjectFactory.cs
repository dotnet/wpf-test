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

namespace Microsoft.Test.Xaml.Utilities
{
    /******************************************************************************
    * CLASS:          XamlBamlObjectFactory
    ******************************************************************************/

    /// <summary>
    /// Class for creating objects originally specified from xaml or baml.
    /// </summary>
    public class XamlBamlObjectFactory
    {
        #region Public and Protected Members

        /******************************************************************************
        * Function:          GenerateObjectFromXaml
        ******************************************************************************/

        /// <summary>
        /// Create an object tree from the original Xaml.
        /// </summary>
        /// <param name="xamlFileName">The name of the xaml file to be tested.</param>
        /// <returns>An object tree.</returns>
        public static object GenerateObjectFromXaml(string xamlFileName)
        {
            XamlSchemaContextSettings settings = new XamlSchemaContextSettings();
            XamlSchemaContext xsc = new XamlSchemaContext(settings);

            XmlReader xmlReader = XmlReader.Create(xamlFileName);
            XamlReader xamlReader = new XamlXmlReader(xmlReader, xsc);

            XamlObjectWriter xamlObjectWriter = new XamlObjectWriter(xamlReader.SchemaContext);

            XamlServices.Transform(xamlReader, xamlObjectWriter);

            return xamlObjectWriter.Result;
        }

        /******************************************************************************
        * Function:          GenerateObjectFromWpfXaml
        ******************************************************************************/

        /// <summary>
        /// Create an object tree from the original Xaml.
        /// </summary>
        /// <param name="xamlFileName">The name of the xaml file to be tested.</param>
        /// <returns>An object tree.</returns>
        public static object GenerateObjectFromWpfXaml(string xamlFileName)
        {
            object obj = null;
            using (XmlReader xmlReader = XmlReader.Create(xamlFileName))
            {
                obj = System.Windows.Markup.XamlReader.Load(xmlReader);
            }

            return obj;
        }

        /******************************************************************************
        * Function:          GenerateObjectFromBaml
        ******************************************************************************/

        /// <summary>
        /// Create an object tree from the original Baml.
        /// </summary>
        /// <param name="bamlFileName">The name of the baml file being used.</param>
        /// <param name="valuesMustBeString">Indicates setting of XamlReaderSettings.ValuesMustBeString.</param>
        /// <returns>An object tree.</returns>
        public static object GenerateObjectFromBaml(string bamlFileName, bool valuesMustBeString)
        {
            using (MemoryStream memStream = GetMemoryStream(bamlFileName))
            {
                XamlReaderSettings settings = new XamlReaderSettings();

                // When valuesMustBeString is true, the Infoset created is a BRAT ("Baml Reader as Text") version
                // which has few optimizations.  Therefore, it is close to the Infoset created from Xaml.
                settings.ValuesMustBeString = valuesMustBeString;
                Baml2006Reader baml2006Reader = new Baml2006Reader(memStream, settings);
                if (baml2006Reader == null)
                {
                    throw new TestSetupException("ERROR: baml2006Reader returned null.");
                }

                XamlObjectWriter xamlObjectWriter = new XamlObjectWriter(baml2006Reader.SchemaContext);
                if (xamlObjectWriter == null)
                {
                    throw new TestSetupException("ERROR: xamlObjectWriter returned null.");
                }

                XamlServices.Transform(baml2006Reader, xamlObjectWriter);

                memStream.Close();
                return xamlObjectWriter.Result;
            }
        }

        /******************************************************************************
        * Function:          GenerateObjectFromBaml
        ******************************************************************************/

        /// <summary>
        /// Create an object tree from the original Baml.
        /// </summary>
        /// <param name="bamlFileName">The name of the baml file being used.</param>
        /// <param name="valuesMustBeString">Indicates setting of XamlReaderSettings.ValuesMustBeString.</param>
        /// <returns>An object tree.</returns>
        public static object GenerateObjectFromWpfBaml(string bamlFileName, bool valuesMustBeString)
        {
            using (MemoryStream memStream = GetMemoryStream(bamlFileName))
            {
                XamlReaderSettings settings = new XamlReaderSettings();

                // When valuesMustBeString is true, the Infoset created is a BRAT ("Baml Reader as Text") version
                // which has few optimizations.  Therefore, it is close to the Infoset created from Xaml.
                settings.ValuesMustBeString = valuesMustBeString;
                settings.BaseUri = new Uri(new Uri(Directory.GetCurrentDirectory()), bamlFileName);
                Baml2006Reader baml2006Reader = new Baml2006Reader(memStream, settings);
                if (baml2006Reader == null)
                {
                    throw new TestSetupException("ERROR: baml2006Reader returned null.");
                }

                object obj = System.Windows.Markup.XamlReader.Load(baml2006Reader);

                memStream.Close();
                return obj;
            }
        }

        /// <summary>
        /// Get a memory stream from the given file name
        /// </summary>
        /// <param name="bamlFileName">the input file name</param>
        /// <returns>Memory stream of file contents</returns>
        public static MemoryStream GetMemoryStream(string bamlFileName)
        {
            using (FileStream fileStream = new FileStream(bamlFileName, FileMode.Open, FileAccess.Read))
            {
                if (fileStream == null)
                {
                    throw new TestSetupException("ERROR: fileStream returned null.");
                }

                byte[] bamlData = new byte[fileStream.Length];
#pragma warning disable CA2022 // Avoid inexact read
                fileStream.Read(bamlData, 0, Convert.ToInt32(fileStream.Length));
#pragma warning restore CA2022
                fileStream.Close();

                MemoryStream memStream = new MemoryStream(bamlData);
                if (memStream == null)
                {
                    throw new TestSetupException("ERROR: memStream returned null.");
                }

                return memStream;
            }
        }

        #endregion
    }
}
