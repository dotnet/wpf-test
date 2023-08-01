// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.IO;
using System.Windows.Markup;



namespace Avalon.Test.Xaml.Markup
{
    /// <summary>
    /// A set of parser utility methods
    /// </summary>
    public class ParserUtil
    {
        #region ParseXamlFile(filename)
        /// <summary>
        /// Utility method to parse a XAML using LoadXml()
        /// This method is a thin wrapper around XamlReader.Load()
        /// It does all the surrounding work like opening the file, etc.
        /// </summary>
        /// <param name="filename">File to be parsed</param>
        public static Object ParseXamlFile(string filename)
        {
            Stream xamlFileStream = File.OpenRead(filename);
            object root = null;
            try
            {
                ParserContext pc = new ParserContext();
                pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
                root = System.Windows.Markup.XamlReader.Load(xamlFileStream, pc);
            }
            finally
            {
                xamlFileStream.Close();

            }
            return root;
        }
        #endregion ParseXamlFile(filename)
        #region ParseXamlFile(filename, parserContext)
        /// <summary>
        /// Utility method to parse a XAML using LoadXml()
        /// This method is a thin wrapper around XamlReader.Load()
        /// It does all the surrounding work like opening the file, etc.
        /// </summary>
        /// <param name="filename">File to be parsed</param>
        /// <param name="parserContext">parser context</param>
        /// <returns>object root generated after XAML parsed</returns>
        public static Object ParseXamlFile(string filename,
            ParserContext parserContext)
        {
            Stream xamlFileStream = File.OpenRead(filename);
            object root = null;
            try
            {
                root = XamlReader.Load(xamlFileStream, parserContext);
            }
            finally
            {
                xamlFileStream.Close();
            }
            return root;
        }
        #endregion ParseXamlFile(filename, parserContext)
        #region LoadBamlFile(filename)
        /// <summary>
        /// Utility method to load a BAML file. 
        /// </summary>
        /// <param name="filename">Baml file to be loaded</param>
        public static Object LoadBamlFile(string filename)
        {
            object root = null;

            System.Uri resourceLocator = new System.Uri(filename, System.UriKind.RelativeOrAbsolute);
            root = Microsoft.Test.Serialization.IOHelper.LoadComponent(resourceLocator);

            return root;
        }
        #endregion LoadBamlFile(filename)
        #region CompileXamlToBaml
        ///// <summary>
        ///// Compiles the given XAML, thus generating a BAML.
        ///// Requires that the XAML file be in the current folder.
        ///// </summary>
        ///// <param name="xamlName">Name of the XAML file</param>
        ///// <returns>Full path of the resultant BAML file</returns>
        //public static string CompileXamlToBaml(string xamlName)
        //{
        //    CompilerHelper compiler = new CompilerHelper();
        //    // Cleanup old compile directories (obj\ and bin\) and files if necessary.
        //    compiler.CleanUpCompilation();
        //    compiler.AddDefaults();
            

        //    // Compile xaml.
        //    MarkupTestLog.LogStatus("Starting Compilation.....");
        //    compiler.CompileApp(xamlName);

        //    // Load compiled app in current AppDomain. 
        //    // This is necessary for the parser to find the generated
        //    // root type from the compiled executable.
        //    Assembly.LoadFile(compiler.CompiledExecutablePath);

        //    return compiler.BamlPath;
        //}
        #endregion CompileXamlToBaml
        #region GetNumericListSeparatorByCulture
        /// <summary>
        /// Gets the numeric list separator for the given culture
        /// This logic is taken from TokenizerHelper.GetNumericListSeparator() in TokenizerHelper.cs.
        /// That class is internal, so we can't use it directly.
        /// </summary>
        /// <param name="culture">Given culture</param>
        /// <returns>Numeric list separator char</returns>
        public static char GetNumericListSeparatorByCulture(CultureInfo culture)
        {
            char numericSeparator = ',';
            NumberFormatInfo numberFormat = NumberFormatInfo.GetInstance(culture);
            // Is the decimal separator is the same as the list separator?
            // If so, we use the ";".
            if ((numberFormat.NumberDecimalSeparator.Length > 0) && (numericSeparator == numberFormat.NumberDecimalSeparator[0]))
            {
                numericSeparator = ';';
            }

            return numericSeparator;
        }
        #endregion GetNumericListSeparatorByCulture
    }
}
