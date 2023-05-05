// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Packaging;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.Markup;
using Microsoft.Test.Serialization;
using Microsoft.Test.Threading;
using Microsoft.Test.Windows;
using Microsoft.Test.Xaml.Async;

namespace Microsoft.Test.Xaml.Parser
{
    /// <summary>
    /// Wrapper class for the WPF 3.0 XamlParser
    /// </summary>
    public class WPFXamlParser : IXamlTestParser
    {
        /// <summary>
        /// Dictionary containing parse mode combinations
        /// </summary>
        private static Dictionary<string, string> s_parseModeDict = null;

        /// <summary>
        /// Initializes static members of the WPFXamlParser class
        /// </summary>
        static WPFXamlParser()
        {
            if (s_parseModeDict == null)
            {
                string loadStream = XamlParseMode.LoadStream.ToString();
                string loadStreamContext = XamlParseMode.LoadStreamContext.ToString();

                string loadAsyncStream = XamlParseMode.LoadAsyncStream.ToString();
                string loadAsyncStreamContext = XamlParseMode.LoadAsyncStreamContext.ToString();

                string loadXmlReader = XamlParseMode.LoadXmlReader.ToString();
                string loadAsyncXmlReader = XamlParseMode.LoadAsyncXmlReader.ToString();

                string parseString = XamlParseMode.ParseString.ToString();
                string parseStringContext = XamlParseMode.ParseStringContext.ToString();

                string coreParsing = loadAsyncStreamContext + "," + loadStreamContext;
                string allModes = loadStream + "," + loadStreamContext + "," + loadAsyncStream + "," +
                            loadAsyncStreamContext + "," + loadAsyncXmlReader + "," + loadXmlReader + "," +
                            parseString + "," + parseStringContext;

                s_parseModeDict = new Dictionary<string, string>();
                s_parseModeDict.Add("loadstream", loadStream);
                s_parseModeDict.Add("loadstreamcontext", loadStreamContext);
                s_parseModeDict.Add("loadasyncstream", loadAsyncStream);
                s_parseModeDict.Add("loadasyncstreamcontext", loadAsyncStreamContext);
                s_parseModeDict.Add("loadxmlreader", loadXmlReader);
                s_parseModeDict.Add("loadasyncxmlreader", loadAsyncXmlReader);
                s_parseModeDict.Add("parsestring", parseString);
                s_parseModeDict.Add("parsestringcontext", parseStringContext);
                s_parseModeDict.Add("coreparsing", coreParsing);
                s_parseModeDict.Add("allModes", allModes);
            }
        }

        #region Public Members

        /// <summary>
        /// Enum for different parse modes
        /// </summary>
        public enum XamlParseMode
        {
            /// <summary>
            /// XamlReader.Load (Stream)
            /// </summary>
            LoadStream,

            /// <summary>
            /// XamlReader.Load (XmlReader)
            /// </summary>
            LoadXmlReader,

            /// <summary>
            /// XamlReader.Load(Stream, ParserContext)
            /// </summary>
            LoadStreamContext,

            /// <summary>
            /// XamlReader.LoadAsync (Stream)
            /// </summary>
            LoadAsyncStream,

            /// <summary>
            /// XamlReader.LoadAsync (XmlReader)
            /// </summary>
            LoadAsyncXmlReader,

            /// <summary>
            /// XamlReader.LoadAsync(Stream, ParserContext)
            /// </summary>
            LoadAsyncStreamContext,

            /// <summary>
            /// XamlReader.Parse (string)
            /// </summary>
            ParseString,

            /// <summary>
            /// XamlReader.Parse(string, ParserContext)
            /// </summary>
            ParseStringContext
        }

        #endregion

        #region IXamlTestParser Members

        /// <summary>
        /// Loads the specified xaml file
        /// </summary>
        /// <param name="xamlFileName">string value</param>
        /// <param name="xamlParseParam">object value.</param>
        /// <returns>object value</returns>
        public object LoadXaml(string xamlFileName, object xamlParseParam)
        {
            if (String.IsNullOrEmpty(xamlFileName))
            {
                throw new ArgumentNullException("xamlFileName");
            }

            if (xamlParseParam is XamlParseMode)
            {
                return ParseXaml(xamlFileName, (XamlParseMode)xamlParseParam);
            }
            else if (xamlParseParam is ParserContext)
            {
                return LoadWithCustomContext(xamlFileName, (ParserContext)xamlParseParam);
            }
            else if (xamlParseParam == null)
            {
                return LoadStreamContext(xamlFileName);
            }
            else
            {
                throw new Exception("xamlParseParam must be either a XamlParseMode, ParserContext, or null");
            }
        }

        /// <summary>
        /// Compares the tree loaded by LoadXaml with a tree loaded with a different mode
        /// </summary>
        /// <param name="xamlFileName">string value</param>
        /// <param name="rootElement">object value.</param>
        /// <param name="xamlParseMode">object value xamlParseMode</param>
        /// <returns>object value</returns>
        public bool CompareXamlTrees(string xamlFileName, object rootElement, object xamlParseMode)
        {
            object reloadRoot = ParseXaml(xamlFileName, (XamlParseMode)xamlParseMode);
            if (reloadRoot == null)
            {
                GlobalLog.LogEvidence("ParseXaml returned null when attempting to load " + xamlFileName + " with XamlParseMode: " + xamlParseMode.ToString());
                return false;
            }

            TreeCompareResult treeCompareResult = TreeComparer.CompareLogical(rootElement, reloadRoot);
            if (treeCompareResult.Result == CompareResult.Different)
            {
                GlobalLog.LogEvidence("Object trees did not match for xaml file: " + xamlFileName + " with XamlParseMode: " + xamlParseMode.ToString());
                return false;
            }
            else
            {
                GlobalLog.LogStatus("Xaml load and tree comparison successful");
                return true;
            }
        }

        /// <summary>
        /// Iterates through the different XamlParseModes
        /// </summary>
        /// <param name="selectedParseMode">The user selected parse mode</param>
        /// <returns>IEnumerable value</returns>
        public IEnumerable<object> ParseModes(string selectedParseMode)
        {
            foreach (string parseModeIter in selectedParseMode.Split(','))
            {
                foreach (string parseMode in GetParseModesAsString(parseModeIter.Trim()).Split(','))
                {
                    yield return Enum.Parse(typeof(XamlParseMode), parseMode);
                }
            }
        }

        /// <summary>
        /// Given and SRID name this will extract the exception string
        /// </summary>
        /// <param name="sridName">string value</param>
        /// <returns>string value.</returns>
        public string ExtractExceptionMessage(string sridName)
        {
            // Get the assembly of the parser
            Assembly assem = Assembly.GetAssembly(typeof(System.Windows.Markup.XamlReader));
            if (assem == null)
            {
                throw new Exception("Assembly containing System.Windows.XamlReader is not found");
            }

            Type sridType = assem.GetType("System.Windows.SRID");
            if (sridType == null)
            {
                throw new Exception("System.Windows.SRID type not found in assembly " + assem.FullName);
            }

            // Get the field of type SRID sridName on the type SRID
            FieldInfo sridFI = sridType.GetField(sridName);
            if (sridFI == null)
            {
                throw new Exception("Field named " + sridName + " is not found in SRID type");
            }

            // Get the actual SRID object
            object sridObject = sridFI.GetValue(sridType);
            if (sridObject == null)
            {
                throw new Exception("Cannot get value of " + sridFI.Name + " field in SRID type");
            }

            Type sysSrType = assem.GetType("System.Windows.SR");
            if (sysSrType == null)
            {
                throw new Exception("System.Windows.SR type not found in assembly " + assem.FullName);
            }

            // Get the method: internal static string SR.Get(SRID id, object[] args)
            MethodInfo sridGetMethod = sysSrType.GetMethod("Get", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
            if (sridGetMethod == null)
            {
                throw new Exception("Get method not found in SR type");
            }

            // Invoke the Get method, this will return a string formatted in the CurrentUICulture
            return sridGetMethod.Invoke(sridGetMethod, new object[] { sridObject }) as string;
        }

        #endregion

        #region Private Methods
        
        /// <summary>
        /// Gets the values for the parse mode
        /// </summary>
        /// <param name="selectedParseMode">selected Parse Mode</param>
        /// <returns>string of valid parse combinations</returns>
        private static string GetParseModesAsString(string selectedParseMode)
        {
            if (s_parseModeDict.ContainsKey(selectedParseMode.ToLower()))
            {
                return s_parseModeDict[selectedParseMode.ToLower()];
            }
            else
            {
                return s_parseModeDict["allModes"];
            }
        }
        
        /// <summary>
        /// Parses the xaml.
        /// </summary>
        /// <param name="xamlFileName">Name of the xaml file.</param>
        /// <param name="xpm">The XamlParseMode.</param>
        /// <returns>object value</returns>
        private object ParseXaml(string xamlFileName, XamlParseMode xpm)
        {
            switch (xpm)
            {
                case XamlParseMode.LoadStream:
                    return LoadStream(xamlFileName);
                case XamlParseMode.LoadXmlReader:
                    return LoadXmlReader(xamlFileName);
                case XamlParseMode.LoadStreamContext:
                    return LoadStreamContext(xamlFileName);
                case XamlParseMode.LoadAsyncStream:
                    return LoadAsyncStream(SetAsynchronousAttributes(xamlFileName));
                case XamlParseMode.LoadAsyncXmlReader:
                    return LoadAsyncXmlReader(SetAsynchronousAttributes(xamlFileName));
                case XamlParseMode.LoadAsyncStreamContext:
                    return LoadAsyncStreamContext(SetAsynchronousAttributes(xamlFileName));
                case XamlParseMode.ParseString:
                    return ParseString(xamlFileName);
                case XamlParseMode.ParseStringContext:
                    return ParseStringContext(xamlFileName);
                default:
                    throw new Exception("Unrecognized XamlParseMode");
            }
        }

        /// <summary>
        /// Loads xaml using the XamlReader.Load(Stream) method
        /// </summary>
        /// <param name="xamlFileName">Name of the xaml file.</param>
        /// <returns>object value</returns>
        private object LoadStream(string xamlFileName)
        {
            FileStream file = new FileStream(xamlFileName, FileMode.Open);
            object treeRoot = null;
            try
            {
                treeRoot = XamlReader.Load(file);

                // We must call SerializationHelper.DisplayTree to be sure the full tree has been parsed and created
                new SerializationHelper().DisplayTree(treeRoot, "LoadStream Tree", true);
            }
            finally
            {
                file.Close();
            }

            return treeRoot;
        }

        /// <summary>
        /// Loads xaml using the XamlReader.Load(XamlReader) method
        /// </summary>
        /// <param name="xamlFileName">Name of the xaml file.</param>
        /// <returns>object value</returns>
        private object LoadXmlReader(string xamlFileName)
        {
            FileStream file = new FileStream(xamlFileName, FileMode.Open);
            XmlReader reader = new XmlTextReader(file);
            object treeRoot = null;
            try
            {
                treeRoot = XamlReader.Load(reader);
                //// We must call SerializationHelper.DisplayTree to be sure the full tree has been parsed and created
                new SerializationHelper().DisplayTree(treeRoot, "LoadXmlReader Tree", true);
            }
            finally
            {
                file.Close();
            }

            return treeRoot;
        }

        /// <summary>
        /// Loads xaml using the XamlReader.Load(Stream, ParserContext) method
        /// </summary>
        /// <param name="xamlFileName">Name of the xaml file.</param>
        /// <returns>object value</returns>
        private object LoadStreamContext(string xamlFileName)
        {
            FileStream file = new FileStream(xamlFileName, FileMode.Open);
            ParserContext context = new ParserContext();
            context.BaseUri = PackUriHelper.Create(new Uri("siteoforigin://"));
            object treeRoot = null;
            try
            {
                treeRoot = XamlReader.Load(file, context);
                //// We must call SerializationHelper.DisplayTree to be sure the full tree has been parsed and created
                new SerializationHelper().DisplayTree(treeRoot, "LoadStreamContext Tree", true);
            }
            finally
            {
                file.Close();
            }

            return treeRoot;
        }

        /// <summary>
        /// Loads xaml using the XamlReader.LoadAsync(Stream) method
        /// </summary>
        /// <param name="xamlFileName">Name of the xaml file.</param>
        /// <returns>object value</returns>
        private object LoadAsyncStream(string xamlFileName)
        {
            //// Create and setup the simulated server
            long xamlFileSize = (new FileInfo(xamlFileName)).Length;
            SimulatedServer server = new SimulatedServer(xamlFileName);
            server.UseRandomChunkSize(10, (int)xamlFileSize / 4); // seed, maxChunkSize 
            server.UseRandomSleepTime(10 /* seed */, 50 /* maxSleepTime in milliseconds */);

            GlobalLog.LogStatus("Async loading the xaml file from a file server.");
            Stream stream = server.ServeFile();
            XamlReader xamlReader = new XamlReader();
            object treeRoot = xamlReader.LoadAsync(stream);
            //// We must call SerializationHelper.DisplayTree to be sure the full tree has been parsed and created
            new SerializationHelper().DisplayTree(treeRoot, "LoadAsyncStream Tree", true);
            if (stream != null)
            {
                stream.Close();
            }

            return treeRoot;
        }

        /// <summary>
        /// Loads xaml using the XamlReader.LoadAsync(XmlReader) method
        /// </summary>
        /// <param name="xamlFileName">Name of the xaml file.</param>
        /// <returns>object value</returns>
        private object LoadAsyncXmlReader(string xamlFileName)
        {
            //// Create and setup the simulated server
            long xamlFileSize = (new FileInfo(xamlFileName)).Length;
            SimulatedServer server = new SimulatedServer(xamlFileName);
            server.UseRandomChunkSize(10 /*seed*/, (int)xamlFileSize / 4 /*maxChunkSize*/);
            server.UseRandomSleepTime(10 /*seed*/, 50 /*maxSleepTime in milliseconds*/);

            GlobalLog.LogStatus("Async loading the xaml file from a file server.");
            Stream stream = server.ServeFile();
            XmlTextReader xmlReader = new XmlTextReader(stream);
            XamlReader xamlReader = new XamlReader();
            object treeRoot = xamlReader.LoadAsync(xmlReader);
            //// We must call SerializationHelper.DisplayTree to be sure the full tree has been parsed and created
            new SerializationHelper().DisplayTree(treeRoot, "LoadAsyncXmlReader Tree", true);
            if (stream != null)
            {
                stream.Close();
            }

            return treeRoot;
        }

        /// <summary>
        /// Loads xaml using the XamlReader.LoadAsync(Stream, ParserContext) method
        /// </summary>
        /// <param name="xamlFileName">Name of the xaml file.</param>
        /// <returns>object value</returns>
        private object LoadAsyncStreamContext(string xamlFileName)
        {
            //// Create and setup the simulated server
            long xamlFileSize = (new FileInfo(xamlFileName)).Length;
            SimulatedServer server = new SimulatedServer(xamlFileName);
            server.UseRandomChunkSize(10 /* seed */, (int)xamlFileSize / 4 /* maxChunkSize */);
            server.UseRandomSleepTime(10 /* seed */, 50 /* maxSleepTime in milliseconds */);

            GlobalLog.LogStatus("Async loading the xaml file from a file server.");
            Stream stream = server.ServeFile();
            XamlReader xamlReader = new XamlReader();
            ParserContext pc = new ParserContext();
            pc.BaseUri = PackUriHelper.Create(new Uri("siteoforigin://"));
            object treeRoot = xamlReader.LoadAsync(stream, pc);
            //// We must call SerializationHelper.DisplayTree to be sure the full tree has been parsed and created
            new SerializationHelper().DisplayTree(treeRoot, "LoadAsyncStreamContext Tree", true);
            if (stream != null)
            {
                stream.Close();
            }

            return treeRoot;
        }

        /// <summary>
        /// Load xaml using the XamlReader.Parse(string) method
        /// </summary>
        /// <param name="xamlFileName">Name of the xaml file.</param>
        /// <returns>object value</returns>
        private object ParseString(string xamlFileName)
        {
            FileStream file = new FileStream(xamlFileName, FileMode.Open);
            StreamReader streamReader = new StreamReader(file);
            string stringXaml = streamReader.ReadToEnd();
            object treeRoot = null;
            try
            {
                treeRoot = XamlReader.Parse(stringXaml);
                //// We must call SerializationHelper.DisplayTree to be sure the full tree has been parsed and created
                new SerializationHelper().DisplayTree(treeRoot, "LoadStreamContext Tree", true);
            }
            finally
            {
                file.Close();
            }

            return treeRoot;
        }

        /// <summary>
        /// Load xaml using the XamlReader.Parse(string, ParserContext) method
        /// </summary>
        /// <param name="xamlFileName">Name of the xaml file.</param>
        /// <returns>object value</returns>
        private object ParseStringContext(string xamlFileName)
        {
            FileStream file = new FileStream(xamlFileName, FileMode.Open);
            StreamReader streamReader = new StreamReader(file);

            string stringXaml = streamReader.ReadToEnd();
            ParserContext context = new ParserContext();
            context.BaseUri = PackUriHelper.Create(new Uri("siteoforigin://"));
            object treeRoot = null;
            try
            {
                treeRoot = XamlReader.Parse(stringXaml, context);
                //// We must call SerializationHelper.DisplayTree to be sure the full tree has been parsed and created
                new SerializationHelper().DisplayTree(treeRoot, "LoadStreamContext Tree", true);
            }
            finally
            {
                file.Close();
            }

            return treeRoot;
        }

        /// <summary>
        /// Loads the with custom context.
        /// </summary>
        /// <param name="xamlFileName">Name of the xaml file.</param>
        /// <param name="context">The context.</param>
        /// <returns>object value</returns>
        private object LoadWithCustomContext(string xamlFileName, ParserContext context)
        {
            FileStream file = new FileStream(xamlFileName, FileMode.Open);
            object treeRoot = null;
            try
            {
                treeRoot = XamlReader.Load(file, context);
                //// We must call SerializationHelper.DisplayTree to be sure the full tree has been parsed and created
                new SerializationHelper().DisplayTree(treeRoot, "LoadStreamContext Tree", true);
            }
            finally
            {
                file.Close();
            }

            return treeRoot;
        }

        /// <summary>
        /// Sets the asynchronous attributes.
        /// </summary>
        /// <param name="xamlFileName">Name of the xaml file.</param>
        /// <returns>string value</returns>
        private string SetAsynchronousAttributes(string xamlFileName)
        {
            // 
            const int AsyncRecords = 2;
            string tempFilename = "_AsyncLoadTempFile.xaml";
            if (File.Exists(tempFilename))
            {
                return tempFilename;
            }

            // Load the Xaml file into a DOM tree
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load(xamlFileName);

            // Add x:SynchronousMode="Async" and x:AsyncRecords 
            // to the root element
            XmlElement rootElement = doc.DocumentElement;
            rootElement.SetAttribute("SynchronousMode", "http://schemas.microsoft.com/winfx/2006/xaml", "Async");
            rootElement.SetAttribute("AsyncRecords", "http://schemas.microsoft.com/winfx/2006/xaml", AsyncRecords.ToString());

            // Save the DOM tree into a temporary Xaml file
            doc.Save(tempFilename);
            GlobalLog.LogFile(tempFilename);
            return tempFilename;
        }

        #endregion
    }
}
