// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using Microsoft.Test.Serialization;
using System.Xml;
using System.Windows.Markup;
using Microsoft.Test.Logging;
using System.Windows.Controls;
using System.Text;

namespace Microsoft.Test.Xaml.Parser.MethodTests.Misc
{
    public class SaveXamlDoesntCloseStream 
    {
        /// <summary>
        /// Called by SerializationBaseCase as entry point
        /// </summary>
        public void RunTest()
        {
            string fileName = DriverState.DriverParameters["TestParams"];
            object root = new Canvas();
            //verify that method SaveAsXml(object obj, TextWriter writer) doesn't close writer
            VerifyObjWriter(root);
            //verify that method SaveAsXml(object obj, Stream stream) doesn't close Stream
            VerifyObjStream(root);

            //verify that method SaveAsXml(object obj, XmlWriter writer) doesn't close writer
            VerifyObjXmlWriter(root);
        }

        /// <summary>
        /// verify that method SaveAsXml(object obj, TextWriter writer) doesn't close writer
        /// </summary>
        /// <param name="obj">The object.</param>
        void VerifyObjWriter(object obj)
        {
            GlobalLog.LogStatus("Inside : VerifyObjectXmlWriter.");
            StringBuilder sb = new StringBuilder();
            TextWriter writer = new StringWriter(sb);
            GlobalLog.LogStatus("Serialize in to writer.");
            SerializationHelper.SerializeObjectTree(obj, writer);
            GlobalLog.LogStatus("Handle writer after SaveAsXml.");
            try
            {
                writer.WriteLine("Write some text");
            }
            catch (ObjectDisposedException)
            {
                throw new Microsoft.Test.TestValidationException("VerifyObjWriter: TextWriter should not have been closed by Parser.");
            }
        }

        /// <summary>
        /// verify that method SaveAsXml(object obj, Stream stream) doesn't close Stream
        /// </summary>
        /// <param name="obj">The object.</param>
        void VerifyObjStream(object obj)
        {
            GlobalLog.LogStatus("Delete file.");
            if (File.Exists("TemporaryFile.Xaml"))
            {
                File.Delete("TemporaryFile.Xaml");
            }

            GlobalLog.LogStatus("Open file.");
            Stream stream = File.OpenWrite("TemporaryFile.Xaml");

            GlobalLog.LogStatus("Serialize in to steam.");
            SerializationHelper.SerializeObjectTree(obj, stream);

            GlobalLog.LogStatus("Flush it.");

            try
            {
                stream.Flush();
            }
            catch (ObjectDisposedException)
            {
                throw new IOException("VerifyObjStream: Stream should not have been closed by Parser.");
            }
            finally
            {
                stream.Close();
                File.Delete("TemporaryFile.Xaml");
            }
        }

        /// <summary>
        /// verify that method SaveAsXml(object obj, XmlWriter writer) doesn't close writer
        /// </summary>
        /// <param name="obj">The object.</param>
        void VerifyObjXmlWriter(object obj)
        {
            GlobalLog.LogStatus("Inside : VerifyObjectXmlWriter.");
            StringBuilder sb = new StringBuilder();
            TextWriter writer = new StringWriter(sb);
            XmlTextWriter xmlWriter = new XmlTextWriter(writer);

            GlobalLog.LogStatus("Serialize in to writer.");
            SerializationHelper.SerializeObjectTree(obj, xmlWriter);
            GlobalLog.LogStatus("Handle writer after SaveAsXml.");
            try
            {
                writer.WriteLine("ddkd");
            }
            catch (ObjectDisposedException)
            {
                throw new Microsoft.Test.TestValidationException("VerifyObjXmlWriter: TextWriter should not have been closed by Parser.");
            }
        }
    }
}
