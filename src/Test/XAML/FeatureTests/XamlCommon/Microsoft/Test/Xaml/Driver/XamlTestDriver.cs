// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Driver
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Windows.Markup;
    using System.Xaml;
    using System.Xml;
    using System.Xml.XPath;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Common.TestObjects.XamlObjectComparer;
    using Microsoft.Test.Xaml.Common.XamlOM;
    using Microsoft.Test.Xaml.Utilities;

    /// <summary>
    /// Xaml Test Driver
    /// </summary>
    public static class XamlTestDriver
    {
        /// <summary>
        /// Roundtrip and compare objects
        /// </summary>
        /// <param name="source">source object</param>
        /// <returns>roundtripped object</returns>
        public static object RoundtripAndCompareObjects(object source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            MemoryStream xamlStream = null;
            object roundTrippedObject = Roundtrip(source, out xamlStream);

            using (xamlStream)
            {
                CompareResult result = TreeComparer.CompareLogical(source, roundTrippedObject);

                if (result == CompareResult.Different)
                {
                    string sourceStr = new ObjectDumper().DumpToString(null, source);
                    string targetStr = new ObjectDumper().DumpToString(null, roundTrippedObject);
                    Tracer.LogTrace("Two objects are different when compared by XAML tree comparer.");
                    Tracer.LogTrace("Before roundtripping: ");
                    Tracer.LogTrace(sourceStr);
                    Tracer.LogTrace("After roundtripping: ");
                    Tracer.LogTrace(targetStr);
                    Tracer.LogTrace("XAML file generated during serializing: ");
                    TraceXamlFile(xamlStream);
                    throw new Exception("Two objects are different.");
                }

                return roundTrippedObject;
            }
        }

        /// <summary>
        /// Roundtrip and examine xaml
        /// </summary>
        /// <param name="value">object to roundtrip</param>
        /// <param name="expressions">xpath expressions to use</param>
        /// <param name="namespaceManager">xml namespace manager for mappings</param>
        /// <returns>roundtripped object</returns>
        public static object RoundtripAndExamineXaml(object value, string[] expressions, XmlNamespaceManager namespaceManager)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (expressions == null)
            {
                throw new ArgumentNullException("expressions");
            }

            MemoryStream xamlStream;
            object roundTrippedObject = Roundtrip(value, out xamlStream);

            using (xamlStream)
            {
                XPathDocument doc = new XPathDocument(xamlStream);
                XPathNavigator navigator = doc.CreateNavigator();
                foreach (string expression in expressions)
                {
                    XPathNodeIterator iterator = null;

                    if (namespaceManager != null)
                    {
                        iterator = navigator.Select(expression, namespaceManager);
                    }
                    else
                    {
                        iterator = navigator.Select(expression);
                    }

                    if (iterator == null)
                    {
                        Tracer.LogTrace("XAML file generated during serializing: ");
                        TraceXamlFile(xamlStream);
                        throw new Exception(String.Format(CultureInfo.InvariantCulture, "The xpath '{0}' does not map to any node in the above xaml document.", expression));
                    }
                }

                return roundTrippedObject;
            }
        }

        /// <summary>
        /// Roundtrip and modify xaml
        /// </summary>
        /// <param name="value">object to roundtrip</param>
        /// <param name="modifyXaml">delegate that modifies the xaml</param>
        /// <returns>roundtripped object</returns>
        public static object RoundtripAndModifyXaml(object value, Func<string, string> modifyXaml)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (modifyXaml == null)
            {
                throw new ArgumentNullException("modifyXaml");
            }

            using (MemoryStream xamlStream = new MemoryStream())
            {
                string modifiedXaml = string.Empty;
                try
                {
                    Serialize(value, xamlStream);
                    modifiedXaml = modifyXaml(GetStringFromMemoryStream(xamlStream));

                    using (XmlReader reader = XmlReader.Create(new StringReader(modifiedXaml)))
                    {
                        return Deserialize(reader);
                    }
                }
                catch (Exception e)
                {
                    Tracer.LogTrace("Exception while roundtripping: ");
                    Tracer.LogTrace(e.ToString());
                    Tracer.LogTrace("XAML file before modification: ");
                    TraceXamlFile(xamlStream);
                    Tracer.LogTrace("XAML file after modification: ");
                    TraceXamlFile(modifiedXaml);

                    throw;
                }
            }
        }

        /// <summary>
        /// Serialize object to stream
        /// </summary>
        /// <param name="value">object to serialize</param>
        /// <param name="xamlStream">serialized stream</param>
        public static void Serialize(object value, Stream xamlStream)
        {
            using (XmlWriter xmlWriter = XmlWriter.Create(xamlStream, new XmlWriterSettings { Indent = true }))
            {
                XamlServices.Save(xmlWriter, value);
            }
        }

        /// <summary>
        /// Serialize to string
        /// </summary>
        /// <param name="value">object to serialize</param>
        /// <returns>serialized string rep</returns>
        public static string Serialize(object value)
        {
            return XamlServices.Save(value);
        }

        /// <summary>
        /// Deserialize the given xaml
        /// </summary>
        /// <param name="xaml">xaml to deserialize</param>
        /// <returns>deserialized object</returns>
        public static object Deserialize(string xaml)
        {
            if (xaml == null)
            {
                throw new ArgumentNullException("xaml");
            }

            return XamlServices.Parse(xaml);
        }

        /// <summary>
        /// Deserialize stream into object
        /// </summary>
        /// <param name="xamlStream">stream to deserialize</param>
        /// <returns>deserialized object</returns>
        public static object Deserialize(Stream xamlStream)
        {
            if (xamlStream == null)
            {
                throw new ArgumentNullException("xamlStream");
            }

            using (XmlReader reader = XmlReader.Create(xamlStream))
            {
                return Deserialize(reader);
            }
        }

        /// <summary>
        /// Deserialize with given xamlreader
        /// </summary>
        /// <param name="xamlReader">xamlreader input to desreialize</param>
        /// <returns>deserialized object</returns>
        public static object Deserialize(XmlReader xamlReader)
        {
            if (xamlReader == null)
            {
                throw new ArgumentNullException("xamlReader");
            }

            return XamlServices.Load(xamlReader);
        }

        /// <summary>
        /// Roundtrip the given object
        /// </summary>
        /// <param name="target">object to roundtirp</param>
        /// <returns>roundtripped object</returns>
        public static object Roundtrip(object target)
        {
            MemoryStream xamlStream = null;
            try
            {
                return Roundtrip(target, out xamlStream);
            }
            finally
            {
                if (xamlStream != null)
                {
                    xamlStream.Close();
                }
            }
        }

        /// <summary>
        /// Deserialize the given xaml
        /// </summary>
        /// <param name="xaml">xaml to deserialize</param>
        /// <param name="namescope">namescope to use as input</param>
        /// <returns>deserialized object</returns>
        public static object Deserialize(string xaml, INameScope namescope)
        {
            if (namescope == null)
            {
                return XamlServices.Parse(xaml);
            }
            else
            {
                var settings = new XamlObjectWriterSettings
                {
                    ExternalNameScope = namescope
                };

                var schemaContext = new XamlSchemaContext();
                var reader = new XamlXmlReader(XmlReader.Create(new StringReader(xaml)), schemaContext);
                var writer = new XamlObjectWriter(schemaContext, settings);
                XamlServices.Transform(reader, writer);
                return writer.Result;
            }
        }

        /// <summary>
        /// Roundtrip and compare the given object
        /// </summary>
        /// <param name="value">object to roundtrip</param>
        /// <returns>serialized xaml</returns>
        public static string RoundTripCompare(object value)
        {
            string xaml = XamlTestDriver.Serialize(value);
            Tracer.LogTrace("Serialized xaml .." + xaml);

            RoundTripCompare(value, null);

            return xaml;
        }

        /// <summary>
        /// Roundtrip and compare - expect exception
        /// </summary>
        /// <param name="value">object to roundtrip</param>
        /// <param name="expectedErrorMessage">expected exception message</param>
        public static void RoundTripCompare(object value, string expectedErrorMessage)
        {
            TestCaseInfo testCaseInfo = new TestCaseInfo()
            {
                TestID = string.Empty,
                Target = value,
                ExpectedResult = expectedErrorMessage == null ? true : false,
                ExpectedMessage = expectedErrorMessage,
            };

            new ObjectDoubleRoundtripDriver().Execute("ObjectFirstTest", testCaseInfo);
        }

        /// <summary>
        /// Round trip and compare - also examine the xaml
        /// </summary>
        /// <param name="value">object to roundtrip</param>
        /// <param name="expressions">xpath expressions to validate on the xaml</param>
        /// <param name="namespaces">namespace mappings for the xpath</param>
        public static void RoundTripCompareExamineXaml(object value, string[] expressions, Dictionary<string, string> namespaces)
        {
            string xaml = XamlTestDriver.Serialize(value);
            Tracer.LogTrace("Serialized xaml .." + xaml);

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (expressions == null)
            {
                throw new ArgumentNullException("expressions");
            }

            TestCaseInfo testCaseInfo = new TestCaseInfo()
            {
                TestID = string.Empty,
                Target = value,
            };
            foreach (string xpathExpression in expressions)
            {
                testCaseInfo.XPathExpresions.Add(xpathExpression);
            }

            foreach (string key in namespaces.Keys)
            {
                if (!testCaseInfo.XPathNamespacePrefixMap.ContainsKey(key))
                {
                    testCaseInfo.XPathNamespacePrefixMap.Add(key, namespaces[key]);
                }
            }

            new ObjectDoubleRoundtripDriver().Execute("ObjectFirstTestExamineXaml", testCaseInfo);
        }

        /// <summary>
        /// Xaml first compare objects
        /// </summary>
        /// <param name="xaml">xaml input to deserialize</param>
        /// <param name="obj">object to compare to</param>
        /// <param name="errorMessage">expected error message</param>
        public static void XamlFirstCompareObjects(string xaml, object obj, string errorMessage)
        {
            Tracer.LogTrace("Xaml is .." + xaml);
            XamlFirstTestCaseInfo info = new XamlFirstTestCaseInfo
            {
                Target = xaml,
                TestID = string.Empty,
                InspectMethod = (target) => XamlObjectComparer.CompareObjects(target, obj),
                ExpectedResult = false,
                ExpectedMessage = errorMessage,
            };

            XamlDoubleRoundtripDriver driver = new XamlDoubleRoundtripDriver();
            driver.Execute("XamlFirstTest", info);
        }

        /// <summary>
        /// Xaml first compare objects
        /// </summary>
        /// <param name="xaml">xaml to deserialize</param>
        /// <param name="obj">object to compare to</param>
        public static void XamlFirstCompareObjects(string xaml, object obj)
        {
            Tracer.LogTrace("XamlFirstCompareObjects: Xaml is .." + xaml);
            XamlFirstTestCaseInfo info = new XamlFirstTestCaseInfo
            {
                Target = xaml,
                TestID = string.Empty,
                InspectMethod = (target) => XamlObjectComparer.CompareObjects(target, obj),
                CompareAttachedProperties = true,
            };

            XamlDoubleRoundtripDriver driver = new XamlDoubleRoundtripDriver();
            driver.Execute("XamlFirstTest", info);
        }

        /// <summary>
        /// Xaml first compare objects
        /// </summary>
        /// <param name="xaml">xaml to deserialize</param>
        /// <param name="obj">object to compare to</param>
        /// <param name="nameScope">namescope to use as input</param>
        public static void XamlFirstCompareObjects(string xaml, object obj, INameScope nameScope)
        {
            object deserialized = Deserialize(xaml, nameScope);
            XamlObjectComparer.CompareObjects(obj, deserialized);
        }

        /// <summary>
        /// Write output to file
        /// </summary>
        /// <param name="xaml">xaml to write out</param>
        /// <param name="fileName">file to write to</param>
        /// <returns>the file path</returns>
        public static string WriteToFile(string xaml, string fileName)
        {
            string filePath = DirectoryAssistance.GetArtifactDirectory(Guid.NewGuid() + fileName);
            WriteToAbsoluteFile(xaml, filePath);
            return filePath;
        }

        /// <summary>
        /// Write to absolute file path
        /// </summary>
        /// <param name="xaml">xaml to write out</param>
        /// <param name="filePath">file path to use</param>
        public static void WriteToAbsoluteFile(string xaml, string filePath)
        {
            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(xaml);
            streamWriter.Flush();
            streamWriter.Close();
        }

        /// <summary>
        /// Write to file
        /// </summary>
        /// <param name="value">object to serialize</param>
        /// <param name="fileName">file to write to</param>
        /// <returns>file path of file</returns>
        public static string WriteToFile(object value, string fileName)
        {
            string xaml = XamlTestDriver.Serialize(value);
            return WriteToFile(xaml, fileName);
        }

        /// <summary>
        /// Xaml first compare objects
        /// </summary>
        /// <param name="xamlDoc">Node list to use</param>
        /// <param name="obj">object to compare to</param>
        /// <returns>serialized xaml</returns>
        public static string XamlFirstCompareObjects(NodeList xamlDoc, object obj)
        {
            string serialized = xamlDoc.NodeListToXml();
            Tracer.LogTrace(serialized);

            XamlFirstCompareObjects(serialized, obj);

            return serialized;
        }

        /// <summary>
        /// Xaml first compare objects
        /// </summary>
        /// <param name="xamlDoc">Node list to use</param>
        /// <param name="obj">object to compare to</param>
        /// <param name="scope">namescope to use</param>
        /// <returns>serialized string</returns>
        public static string XamlFirstCompareObjects(NodeList xamlDoc, object obj, INameScope scope)
        {
            string serialized = xamlDoc.NodeListToXml();

            XamlFirstCompareObjects(serialized, obj, scope);

            return serialized;
        }

        /// <summary>
        /// Get test instances
        /// </summary>
        /// <param name="types">data types to use</param>
        /// <param name="instanceID">instance id name</param>
        /// <returns>Returns a test case info</returns>
        public static TestCaseInfo GetInstance(List<Type> types, string instanceID)
        {
            List<TestCaseInfo> testCases = GetInstances(types);
            return GetInstance(testCases, instanceID);
        }

        /// <summary>
        /// Get a particular test case info from a list
        /// </summary>
        /// <param name="testCases">list of test cases</param>
        /// <param name="instanceID">instance id to match</param>
        /// <returns>matching test case</returns>
        public static TestCaseInfo GetInstance(ICollection<TestCaseInfo> testCases, string instanceID)
        {
            foreach (TestCaseInfo testCase in testCases)
            {
                if (testCase.TestID == instanceID)
                {
                    return testCase;
                }
            }

            throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "[Helper] can not find the instance '{0}'", instanceID));
        }

        /// <summary>
        /// Create test case info's for given data types
        /// </summary>
        /// <param name="types">types to use</param>
        /// <returns>List of test case infos</returns>
        public static List<TestCaseInfo> GetInstances(List<Type> types)
        {
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            foreach (Type type in types)
            {
                ////ToDo: verify and log if type has GetTestCases implemented.
                ////ToDo: if the instance does not have the GetTestCases method, using instance creator to create objects.
                ////      It must be reproducible according to a seed. The ToString() must give an unique id that can identify the object.
                MethodInfo testMethod = type.GetMethod(Global.GetTestCasesMethodName);

                if (testMethod == null)
                {
                    Tracer.Trace(GetMethodFullName(MethodInfo.GetCurrentMethod()), "Method '{0}' is not defined in type '{1}'.", Global.GetTestCasesMethodName, type.FullName);
                    continue;
                }

                testCases.AddRange(testMethod.Invoke(null, null) as IEnumerable<TestCaseInfo>);
            }

            return testCases;
        }

        /// <summary>
        /// Generate test cases
        /// </summary>
        /// <param name="addCase">Add test case event handler</param>
        /// <param name="testTypes">list of test types</param>
        /// <param name="generatorMethod">generator method</param>
        public static void GenerateTestCases(AddTestCaseEventHandler addCase, List<Type> testTypes, MemberInfo generatorMethod)
        {
            GenerateTestCases(addCase, XamlTestDriver.GetInstances(testTypes), generatorMethod);
        }

        /// <summary>
        /// Generate the test cases
        /// </summary>
        /// <param name="addCase">add test case handler</param>
        /// <param name="testCaseInfos">test case inforamtions</param>
        /// <param name="generatorMethod">generator method</param>
        public static void GenerateTestCases(AddTestCaseEventHandler addCase, ICollection<TestCaseInfo> testCaseInfos, MemberInfo generatorMethod)
        {
            // unfortunately, TestHost requires that the generator method and generated method name to be different
            // The convention used here is - to append "Method" to the generator method name for generated method name
            MethodInfo testMethod = generatorMethod.DeclaringType.GetMethod(
                generatorMethod.Name + "Method",
                new Type[] { typeof(string) });

            foreach (TestCaseInfo testCaseInfo in testCaseInfos)
            {
                TestCaseAttribute testCase = new TestCaseAttribute();
                testCase.Category = TestCategory.IDW;
                if (testCaseInfo.BugNumber != 0)
                {
                    testCase.TestType = TestType.BlockedProductIssue;
                    testCase.Description += "Bug=" + testCaseInfo.BugNumber;
                }

                addCase(testCase, testMethod, testCaseInfo.TestID);
            }
        }

        /// <summary>
        /// Helper to run a test given the test info
        /// </summary>
        /// <param name="testMethod">test method to use</param>
        /// <param name="testCaseInfo">test case information</param>
        public static void RunTest(MethodBase testMethod, TestCaseInfo testCaseInfo)
        {
            string source = GetMethodFullName(testMethod) + "#" + testCaseInfo.TestID;

            switch (testCaseInfo.TestDriver)
            {
                case TestDrivers.ObjectDoubleRoundtripDriver:
                    new ObjectDoubleRoundtripDriver().Execute(source, testCaseInfo);
                    break;
                case TestDrivers.XamlDoubleRoundtripDriver:
                    new XamlDoubleRoundtripDriver().Execute(source, testCaseInfo);
                    break;
                case TestDrivers.NodeWriterXamlXmlReaderDriver:
                    new NodeWriterXamlXmlReaderTestDriver().Execute(source, testCaseInfo);
                    break;
                case TestDrivers.XamlXmlWriterXamlXmlReaderDriver:
                    new XamlXmlWriterXamlXmlReaderTestDriver().Execute(source, testCaseInfo);
                    break;

                default:
                    throw new NotImplementedException(String.Format("{0} is an unknown driver", testCaseInfo.TestDriver.ToString()));
            }
        }

        /// <summary>
        /// Trace a file to the logs
        /// </summary>
        /// <param name="stream">stream to trace out</param>
        /// <param name="outputFile">output file to write to</param>
        public static void TraceFile(Stream stream, string outputFile)
        {
            using (StreamWriter outWriter = new StreamWriter(outputFile))
            {
                //// Not closing the reader, otherwise the underlying memory stream will be closed.
                StreamReader reader = new StreamReader(stream);
                outWriter.Write(reader.ReadToEnd());
                outWriter.Flush();
            }

            stream.Position = 0;
        }

        /// <summary>
        /// Get an instance id prefix
        /// </summary>
        /// <param name="type">type to use</param>
        /// <returns>Instance ID prefix to use</returns>
        public static string GetInstanceIDPrefix(Type type)
        {
            return type.FullName + ".Instance";
        }

        /// <summary>
        /// Get the full name of a method
        /// </summary>
        /// <param name="testMethod">test method inforamtion</param>
        /// <returns>full name of the method</returns>
        private static string GetMethodFullName(MethodBase testMethod)
        {
            return testMethod.DeclaringType.FullName + "." + testMethod.Name;
        }

        /// <summary>
        /// caller to close the output stream which contains XAML. 
        /// </summary>
        /// <param name="obj">object to roundtrip</param>
        /// <param name="xamlStream">memory stream output</param>
        /// <returns>deserialized object</returns>
        private static object Roundtrip(object obj, out MemoryStream xamlStream)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            xamlStream = new MemoryStream();

            try
            {
                Serialize(obj, xamlStream);

                xamlStream.Position = 0;

                // Create Xaml subdirectory
                string xamlSubDirectory = DirectoryAssistance.GetArtifactDirectory("Xaml");
                if (!Directory.Exists(xamlSubDirectory))
                {
                    Directory.CreateDirectory(xamlSubDirectory);
                }

                string fileName = DirectoryAssistance.GetTempFileWithGuid("Xaml\\XamlRoundtrip_{0}.xaml");
                Tracer.LogTrace("Saving xaml to {0}.", fileName);
                Tracer.LogTrace("For official lab runs, the file will also be available on the file tab.");
                File.WriteAllText(fileName, GetStringFromMemoryStream(xamlStream));

                xamlStream.Position = 0;
                using (XmlReader reader = XmlReader.Create(xamlStream))
                {
                    return Deserialize(reader);
                }
            }
            finally
            {
                xamlStream.Position = 0;
            }
        }

        /// <summary>
        /// trace stream to output
        /// </summary>
        /// <param name="xamlStream">stream to trace out</param>
        private static void TraceXamlFile(MemoryStream xamlStream)
        {
            TraceXamlFile(GetStringFromMemoryStream(xamlStream));
            xamlStream.Position = 0;
        }

        /// <summary>
        /// trace out the given xaml
        /// </summary>
        /// <param name="xaml">xaml string to trace out</param>
        private static void TraceXamlFile(string xaml)
        {
            Tracer.LogTrace(xaml);
        }

        /// <summary>
        /// Get string from stream
        /// </summary>
        /// <param name="xamlStream">xaml input stream</param>
        /// <returns>string rep from the stream</returns>
        private static string GetStringFromMemoryStream(MemoryStream xamlStream)
        {
            //// Not closing the reader, otherwise the underlying memory stream 
            //// will be closed. It will be closed by the caller
            xamlStream.Position = 0;
            StreamReader reader = new StreamReader(xamlStream);
            return reader.ReadToEnd();
        }
    }
}
