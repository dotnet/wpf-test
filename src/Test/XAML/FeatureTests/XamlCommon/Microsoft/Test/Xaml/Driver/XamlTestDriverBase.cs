// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xaml;
using System.Xml;
using Microsoft.Test.Xaml.Common;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Driver
{
    /// <summary>
    /// Base class for the different test drivers.
    /// Handles negative test handling
    /// Holds some serialization methods.
    /// </summary>
    public abstract class XamlTestDriverBase
    {
        /// <summary>
        /// Initializes a new instance of the XamlTestDriverBase class
        /// </summary>
        public XamlTestDriverBase()
        {
            this.TraceCache = new StringWriter();
        }

        /// <summary>
        /// Gets or sets the trace cache
        /// </summary>
        protected StringWriter TraceCache { get; set; }

        /// <summary>
        /// Execute the test and handle negative cases.
        /// </summary>
        /// <param name="source">test id to use</param>
        /// <param name="testCaseInfo">the test case information</param>
        /// <returns>true if test passed</returns>
        public bool Execute(string source, TestCaseInfo testCaseInfo)
        {
            bool pass;
            try
            {
                // Execute the actual test code in the derived class //
                ExecuteTest(source, testCaseInfo);

                pass = false;
                pass ^= testCaseInfo.ExpectedResult;

                if (pass)
                {
                    Tracer.Trace(source, "Pass");
                }
                else
                {
                    Tracer.Trace(source, "Expected to fail, but passed instead.");
                }
            }
            catch (Exception e)
            {
                pass = true;
                pass ^= testCaseInfo.ExpectedResult;
                FlushLogs(source);

                if (pass)
                {
                    if (!string.IsNullOrEmpty(testCaseInfo.ExpectedMessage))
                    {
                        if (!ExceptionMessageHelper.IsExceptionStringMatch(testCaseInfo.ExpectedMessage, e.Message))
                        {
                            pass = false;
                        }
                    }
                    else
                    {
                        pass = false;
                    }

                    if (pass)
                    {
                        Tracer.Trace(source, "Fail as expected. Failure detail:");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(testCaseInfo.ExpectedMessage))
                        {
                            Tracer.Trace(source, "ExpectedMessage must be set when an exception is expected.");
                        }
                        else
                        {
                            Tracer.Trace(source, "Expected {0}, but got following instead:", testCaseInfo.ExpectedMessage);
                        }
                    }
                }
                else
                {
                    Tracer.Trace(source, "Expected to pass, but fail with following error:");
                    Tracer.Trace(source, e.ToString());
                    throw;
                }

                Tracer.Trace(source, e.ToString());
            }

            if (!pass)
            {
                throw new DataTestException(String.Format(CultureInfo.InvariantCulture, "Test fails for '{0}'.", source));
            }

            return pass;
        }

        /// <summary>
        /// Implementation of the test driver/type
        /// </summary>
        /// <param name="source">test identifier</param>
        /// <param name="testCaseInfo">test case information</param>
        protected virtual void ExecuteTest(string source, TestCaseInfo testCaseInfo)
        {
        }

        /// <summary>
        /// Deserialize a memory stream
        /// </summary>
        /// <param name="stream">stream to deserialize</param>
        /// <returns>deserialized object</returns>
        protected object Deserialize(MemoryStream stream)
        {
            return Deserialize(stream, new XamlSchemaContext());
        }

        /// <summary>
        /// Deserialize memory stream
        /// </summary>
        /// <param name="stream">stream to deserialize</param>
        /// <param name="context">xaml schema context to use</param>
        /// <returns>deserialized object</returns>
        protected object Deserialize(MemoryStream stream, XamlSchemaContext context)
        {
            object obj = null;
            stream.Position = 0;

            using (XmlReader reader = XmlReader.Create(stream))
            {
                XamlXmlReader xamlReader = new XamlXmlReader(reader, context);
                XamlObjectWriter xamlWriter = new XamlObjectWriter(context);
                XamlServices.Transform(xamlReader, xamlWriter);
                obj = xamlWriter.Result;
            }

            stream.Position = 0;
            return obj;
        }

        /// <summary>
        /// Serialize an object to memory stream
        /// </summary>
        /// <param name="obj">object to serialize</param>
        /// <returns>serialized memory stream</returns>
        protected MemoryStream Serialize(object obj)
        {
            return Serialize(obj, new XamlSchemaContext());
        }

        /// <summary>
        /// Serialize to memory stream 
        /// </summary>
        /// <param name="obj">object to serialize</param>
        /// <param name="context">xaml schema context to use</param>
        /// <returns>serialized memory stream</returns>
        protected MemoryStream Serialize(object obj, XamlSchemaContext context)
        {
            MemoryStream stream = new MemoryStream();

            XmlWriterSettings setting = new XmlWriterSettings();
            setting.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(stream, setting))
            {
                XamlObjectReader xamlReader = new XamlObjectReader(obj, context);
                XamlXmlWriter xamlWriter = new XamlXmlWriter(writer, context);
                XamlServices.Transform(xamlReader, xamlWriter);
            }

            stream.Position = 0;

            if (Environment.GetEnvironmentVariable(Global.ToFileEnvironmentVariable) != null)
            {
                string outputFile = Path.Combine(Directory.GetCurrentDirectory(), (Global.UniqueResultFileName + ".xml"));
                XamlTestDriver.TraceFile(stream, outputFile);
            }

            return stream;
        }

        /// <summary>
        /// Flush string to logs
        /// </summary>
        /// <param name="source">string to output</param>
        private void FlushLogs(string source)
        {
            Tracer.LogTrace(source, this.TraceCache.ToString());
        }
    }
}
