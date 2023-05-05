// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using System.IO.Packaging;
using Microsoft.Test.Xaml.Async;
using Microsoft.Test.Threading;
using Microsoft.Test.Logging;
using System.IO;
using Microsoft.Test.Windows;
using Microsoft.Test.Markup;
using System.ComponentModel;
using Microsoft.Test.Serialization;
using System.Resources;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Microsoft.Test.Xaml.Parser
{
    /// <summary>
    /// Wrapper class for the WPF 3.0 XamlParser
    /// </summary>
    public class XamlTestParser : IXamlTestParser
    {
        #region IXamlTestParser Members
        /// <summary>
        /// Loads the specified xaml file using the Load(stream) override
        /// </summary>
        /// <param name="xamlFileName"></param>
        /// <param name="xamlParseMode"></param>
        /// <returns></returns>
        public object LoadXaml(string xamlFileName, object xamlParseMode)
        {
            if (String.IsNullOrEmpty(xamlFileName))
            {
                throw new ArgumentNullException("xamlFileName");
            }
            return ParseXaml(xamlFileName, (XamlParseMode)xamlParseMode);
        }
        /// <summary>
        /// Compares the tree loaded by LoadXaml with a tree loaded with a different mode
        /// </summary>
        /// <param name="xamlFileName"></param>
        /// <param name="rootElement"></param>
        /// <param name="xamlParseMode"></param>
        /// <returns></returns>
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
        /// <returns></returns>
        public IEnumerable<object> ParseModes()
        {
            yield return XamlParseMode.LoadStream;
            yield return XamlParseMode.LoadXmlReader;
            yield return XamlParseMode.LoadStreamContext;
            yield return XamlParseMode.LoadAsyncStream;
            yield return XamlParseMode.LoadAsyncXmlReader;
            yield return XamlParseMode.LoadAsyncStreamContext;            
        }

        /// <summary>
        /// Given and SRID name this will extract the exception string 
        /// </summary>
        /// <param name="sridName"></param>
        /// <returns></returns>
        public string ExtractExceptionMessage(string sridName)
        {
            //Get the assembly of the parser
            Assembly assem = Assembly.GetAssembly(typeof(XamlReader));
            Type sridType = assem.GetType("System.Windows.SRID");
            //Get the property of type SRID sridName on the type SRID
            PropertyInfo sridPI = sridType.GetProperty(sridName);
            //Get the actual SRID object
            object sridObject = sridPI.GetValue(sridType, null);
            Type srType = assem.GetType("System.Windows.SR");
            //Get the method: internal static string SR.Get(SRID id, object[] args)
            MethodInfo srGetMethod = srType.GetMethod("Get", BindingFlags.NonPublic | BindingFlags.Static);
            //Invoke the Get method, this will return a string formatted in the CurrentUICulture
            return srGetMethod.Invoke(srGetMethod, new object[] { sridObject, null }) as string;
        }

        

        #endregion

        #region Private Methods

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
                default:
                    return null;
            }
        }

        /// <summary>
        /// Loads xaml using the XamlReader.Load(Stream) method
        /// </summary>
        /// <param name="xamlFileName"></param>
        /// <returns></returns>
        private object LoadStream(string xamlFileName)
        {
            FileStream file = new FileStream(xamlFileName, FileMode.Open);
            object treeRoot = null;
            try
            {
                treeRoot = XamlReader.Load(file);
                //We must call SerializationHelper.DisplayTree to be sure the full tree has been parsed and created
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
        /// <param name="xamlFileName"></param>
        /// <returns></returns>
        private object LoadXmlReader(string xamlFileName)
        {
            FileStream file = new FileStream(xamlFileName, FileMode.Open);
            XmlReader reader = XmlReader.Create(file);
            object treeRoot = null;
            try
            {
                treeRoot = XamlReader.Load(reader);
                //We must call SerializationHelper.DisplayTree to be sure the full tree has been parsed and created
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
        /// <param name="xamlFileName"></param>
        /// <returns></returns>
        private object LoadStreamContext(string xamlFileName)
        {
            FileStream file = new FileStream(xamlFileName, FileMode.Open);
            ParserContext context = new ParserContext();
            context.BaseUri = PackUriHelper.Create(new Uri("siteoforigin://"));
            object treeRoot = null;
            try
            {
                treeRoot = XamlReader.Load(file, context);
                //We must call SerializationHelper.DisplayTree to be sure the full tree has been parsed and created
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
        /// <param name="xamlFileName"></param>
        /// <returns></returns>
        private object LoadAsyncStream(string xamlFileName)
        {
            //Create and setup the simulated server
            long xamlFileSize = (new FileInfo(xamlFileName)).Length;
            SimulatedServer server = new SimulatedServer(xamlFileName);
            server.UseRandomChunkSize(10 /*seed*/, (int)xamlFileSize / 4 /*maxChunkSize*/);
            server.UseRandomSleepTime(10 /*seed*/, 50 /*maxSleepTime in milliseconds*/);
            
            GlobalLog.LogStatus("Async loading the xaml file from a file server.");
            Stream stream = server.ServeFile();
            XamlReader xamlReader = new XamlReader();           
            object treeRoot = xamlReader.LoadAsync(stream);
            //We must call SerializationHelper.DisplayTree to be sure the full tree has been parsed and created
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
        /// <param name="xamlFileName"></param>
        /// <returns></returns>
        private object LoadAsyncXmlReader(string xamlFileName)
        {
            //Create and setup the simulated server
            long xamlFileSize = (new FileInfo(xamlFileName)).Length;
            SimulatedServer server = new SimulatedServer(xamlFileName);
            server.UseRandomChunkSize(10 /*seed*/, (int)xamlFileSize / 4 /*maxChunkSize*/);
            server.UseRandomSleepTime(10 /*seed*/, 50 /*maxSleepTime in milliseconds*/);

            GlobalLog.LogStatus("Async loading the xaml file from a file server.");
            Stream stream = server.ServeFile();
            XmlTextReader xmlReader = new XmlTextReader(stream);
            XamlReader xamlReader = new XamlReader();
            object treeRoot = xamlReader.LoadAsync(xmlReader);
            //We must call SerializationHelper.DisplayTree to be sure the full tree has been parsed and created
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
        /// <param name="xamlFileName"></param>
        /// <returns></returns>
        private object LoadAsyncStreamContext(string xamlFileName)
        {
            //Create and setup the simulated server
            long xamlFileSize = (new FileInfo(xamlFileName)).Length;
            SimulatedServer server = new SimulatedServer(xamlFileName);
            server.UseRandomChunkSize(10 /*seed*/, (int)xamlFileSize / 4 /*maxChunkSize*/);
            server.UseRandomSleepTime(10 /*seed*/, 50 /*maxSleepTime in milliseconds*/);

            GlobalLog.LogStatus("Async loading the xaml file from a file server.");
            Stream stream = server.ServeFile();
            XamlReader xamlReader = new XamlReader();
            ParserContext pc = new ParserContext();
            pc.BaseUri = PackUriHelper.Create(new Uri("siteoforigin://"));
            object treeRoot = xamlReader.LoadAsync(stream);
            //We must call SerializationHelper.DisplayTree to be sure the full tree has been parsed and created
            new SerializationHelper().DisplayTree(treeRoot, "LoadAsyncStreamContext Tree", true);
            if (stream != null)
            {
                stream.Close();
            }
            return treeRoot;
        }

        private string SetAsynchronousAttributes(string xamlFileName)
        {
            //
            const int asyncRecords = 2;
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
            rootElement.SetAttribute("AsyncRecords", "http://schemas.microsoft.com/winfx/2006/xaml", asyncRecords.ToString());

            // Save the DOM tree into a temporary Xaml file
            doc.Save(tempFilename);
            GlobalLog.LogFile(tempFilename);
            return tempFilename;
        }

        #endregion

        #region Public Members
        /// <summary>
        /// Enum for different parse modes
        /// </summary>
        public enum XamlParseMode  
        {
            /// <summary>
            /// XamlReader.Load(Stream)
            /// </summary>
            LoadStream,
            /// <summary>
            /// XamlReader.Load(XmlReader)
            /// </summary>
            LoadXmlReader,
            /// <summary>
            /// XamlReader.Load(Stream, ParserContext)
            /// </summary>
            LoadStreamContext,
            /// <summary>
            /// XamlReader.LoadAsync(Stream)
            /// </summary>
            LoadAsyncStream,
            /// <summary>
            /// XamlReader.LoadAsync(XmlReader)
            /// </summary>
            LoadAsyncXmlReader,
            /// <summary>
            /// XamlReader.LoadAsync(Stream, ParserContext)
            /// </summary>
            LoadAsyncStreamContext
        }

        #endregion

    }
}
