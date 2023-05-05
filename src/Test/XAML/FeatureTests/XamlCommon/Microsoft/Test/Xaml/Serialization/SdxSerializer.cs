// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.IO;
using System.Xaml;
using System.Xml;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Serialization
{
    /// <summary>
    /// Sdx Serializer
    /// </summary>
    public class SdxSerializer : IXamlTestSerializer
    {
        #region IXamlTestSerializer Members

        /// <summary>
        /// Serialized the given object tree with XamlWriter
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="treeRoot">The tree root.</param>
        public void SerializeObjectTree(string fileName, object treeRoot)
        {
            if (File.Exists(fileName))
            {
                GlobalLog.LogStatus("Deleting pre-existing file: " + fileName);
                File.Delete(fileName);
            }

            SerializeObjectTree(treeRoot, fileName);
        }

        #endregion

        /// <summary>
        /// Serializes the object tree.
        /// </summary>
        /// <param name="objectTree">The object tree.</param>
        /// <param name="fileName">Name of the file.</param>
        private static void SerializeObjectTree(object objectTree, string fileName)
        {
            string outer = String.Empty;

            if (objectTree == null)
            {
                throw new ArgumentNullException("objectTree");
            }

            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            FileStream fs = null;
            StreamWriter sw = null;

            try
            {
                if (ShouldUseWpfContext())
                {
                    bool requireExplicitContentVisibility = RequireContentVisibility();
                    XamlSchemaContext context = System.Windows.Markup.XamlReader.GetWpfSchemaContext();
                    XamlObjectReader xamlObjectReader = new XamlObjectReader(objectTree, context, new XamlObjectReaderSettings { RequireExplicitContentVisibility = requireExplicitContentVisibility });

                    TextWriter textWriter = new StringWriter(CultureInfo.InvariantCulture);
                    XmlTextWriter xmlWriter = new XmlTextWriter(textWriter);
                    xmlWriter.Formatting = Formatting.Indented;
                    XamlXmlWriter xamlXmlWriter = new XamlXmlWriter(xmlWriter, xamlObjectReader.SchemaContext);

                    XamlServices.Transform(xamlObjectReader, xamlXmlWriter);
                    outer = textWriter.ToString();
                }
                else
                {
                    outer = XamlServices.Save(objectTree);
                }

                fs = new FileStream(fileName, FileMode.Create);
                sw = new StreamWriter(fs);
                sw.Write(outer);
            }
            finally
            {
                if (null != sw)
                {
                    sw.Close();
                }

                if (null != fs)
                {
                    fs.Close();
                }
            }
        }

        /// <summary>
        /// Use the WpfSchemaContext instead of the default
        /// XamlSchemaContext
        /// </summary>
        /// <returns>true if Wpf schema context should be used</returns>
        private static bool ShouldUseWpfContext()
        {
            string context = DriverState.DriverParameters["UseWpfContext"];
            bool useWpfContext = false;
            if (string.IsNullOrEmpty(context) || !bool.TryParse(context, out useWpfContext))
            {
                useWpfContext = true;
            }

            return useWpfContext;
        }

        /// <summary>
        /// Check if RequireExplicitContentVisibility is passed in 
        /// as a parameter
        /// </summary>
        /// <returns>true if RequireExplicitContentVisibility parameter is set</returns>
        private static bool RequireContentVisibility()
        {
            string requireExplicitContentVisibility = DriverState.DriverParameters["RequireExplicitContentVisibility"];
            bool contentVisibility = false;
            if (string.IsNullOrEmpty(requireExplicitContentVisibility) || !bool.TryParse(requireExplicitContentVisibility, out contentVisibility))
            {
                // Set RequireExplicitContentVisibility to be compatable with WPF.
                contentVisibility = true;
            }

            return contentVisibility;
        }
    }
}
