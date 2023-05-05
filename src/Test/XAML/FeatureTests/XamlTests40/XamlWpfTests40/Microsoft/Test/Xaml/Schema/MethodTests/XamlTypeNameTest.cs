// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Schema.MethodTests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Xaml;
    using System.Xaml.Schema;
    using Microsoft.Test.Globalization;
    using Microsoft.Test.Logging;
    using Microsoft.Test.Xaml.Utilities;

    /// <summary>
    /// Contains tests for XamlTypeName type.
    /// </summary>
    public class XamlTypeNameTest
    {
        /// <summary>
        /// For Parse, TryParse and ToString tests.
        /// </summary>
        private readonly string[] _parseTestcases = new string[]
                                                       {
                                                           "TypeA",
                                                           "prefixA:TypeA",
                                                           "TypeA(TypeB)",
                                                           "prefixA:TypeA(TypeB)",
                                                           "prefixA:TypeA(prefixB:TypeB)",
                                                           "prefixA:TypeA(TypeB(TypeC))",
                                                           "prefixA:TypeA(prefixB:TypeB(TypeC))",

                                                           "prefixA:TypeA(TypeB1,TypeB2)",
                                                           "prefixA:TypeA(prefixB1:TypeB1,TypeB2)",
                                                           "prefixA:TypeA(TypeB1(TypeC),TypeB2)",
                                                           "prefixA:TypeA(prefixB1:TypeB1(TypeC),TypeB2)",

                                                           "prefixA:TypeA(TypeB1,prefixB2:TypeB2)",
                                                           "prefixA:TypeA(prefixB1:TypeB1,prefixB2:TypeB2)",
                                                           "prefixA:TypeA(TypeB1(TypeC),prefixB2:TypeB2)",
                                                           "prefixA:TypeA(prefixB1:TypeB1(TypeC),prefixB2:TypeB2)",

                                                           "prefixA:TypeA(TypeB1,TypeB2(TypeC))",
                                                           "prefixA:TypeA(prefixB1:TypeB1,TypeB2(TypeC))",
                                                           "prefixA:TypeA(TypeB1(TypeC),TypeB2(TypeC))",
                                                           "prefixA:TypeA(prefixB1:TypeB1(TypeC),TypeB2(TypeC))",

                                                           "prefixA:TypeA(TypeB1,prefixB2:TypeB2(TypeC))",
                                                           "prefixA:TypeA(prefixB1:TypeB1,prefixB2:TypeB2(TypeC))",
                                                           "prefixA:TypeA(TypeB1(TypeC),prefixB2:TypeB2(TypeC))",
                                                           "prefixA:TypeA(prefixB1:TypeB1(TypeC),prefixB2:TypeB2(TypeC))",
                                                           "TypeA(TypeB1,TypeB2,TypeB3,TypeB4,TypeB5,TypeB6,TypeB7,TypeB8,TypeB9,TypeB10)",
                                                           "TypeA(TypeB(TypeC(TypeD(TypeE(TypeF(TypeG(TypeF(TypeH(TypeI(TypeJ(TypeK)))))))))))",
                                                       };

        /// <summary>
        /// For ParseList, TryParseList and MakeStringList tests.
        /// </summary>
        private readonly string[] _parseListTestcases =
            {
                "TypeB1,TypeB2",
                "prefixB1:TypeB1,TypeB2",
                "TypeB1(TypeC),TypeB2",
                "prefixB1:TypeB1(TypeC),TypeB2",

                "TypeB1,prefixB2:TypeB2",
                "prefixB1:TypeB1,prefixB2:TypeB2",
                "TypeB1(TypeC),prefixB2:TypeB2",
                "prefixB1:TypeB1(TypeC),prefixB2:TypeB2",

                "TypeB1,TypeB2(TypeC)",
                "prefixB1:TypeB1,TypeB2(TypeC)",
                "TypeB1(TypeC),TypeB2(TypeC)",
                "prefixB1:TypeB1(TypeC),TypeB2(TypeC)",

                "TypeB1,prefixB2:TypeB2(TypeC)",
                "prefixB1:TypeB1,prefixB2:TypeB2(TypeC)",
                "TypeB1(TypeC),prefixB2:TypeB2(TypeC)",
                "prefixB1:TypeB1(TypeC),prefixB2:TypeB2(TypeC)",

                "TypeB1,TypeB2,TypeB3,TypeB4,TypeB5,TypeB6,TypeB7,TypeB8,TypeB9,TypeB10",
            };

        /// <summary>
        /// For ParseNegative and TryParseNegative tests.
        /// </summary>
        private readonly string[] _parseNegativeTestcases =
            {
                string.Empty,
                " ",
                ":",
                "prefix:",
                " :Type",
                "prefixA1:prefixA2:TypeA",
                "( )",
                "Type( )",
                "(Type)",
                "(",
                ")",
                "TypeA(TypeB",
                "Type)TypeB",
                "TypeA(TypeB(TypeC)",
                "TypeA(TypeB)TypeC)",
                " : ( )",
                "prefix: ( )",
                " :Type( )",
                " : (Type)",
                "prefix:TypeA(TypeB",
            };

        /// <summary>
        /// For ParseListNegative and TryParseListNegative tests.
        /// </summary>
        private readonly string[] _parseListNegativeTestcases =
            {
                string.Empty,
                ",",
                " ",
                "Type, ",
                " ,Type",
                " ,Type, ",
                "TypeA, ,TypeB",
                ": ( ), : ( )",
                "prefixA1:TypeA1(TypeB1,prefixA2:TypeA2(TypeB2)",
                "(:Type",
                ",:Type",
                "prefix:(",
                "prefix:,",
                "Type(:)",
                "Type(,)",
            };

        /// <summary>
        /// XamlNamespaceResolver for use by test methods.
        /// </summary>
        private readonly IXamlNamespaceResolver _xamlNamespaceResolver = null;

        /// <summary>
        /// NamespacePrefixLookup for use by test methods.
        /// </summary>
        private readonly INamespacePrefixLookup _namespacePrefixLookup = null;

        /// <summary>
        /// Initializes a new instance of the XamlTypeNameTest class.
        /// </summary>
        public XamlTypeNameTest()
        {
            _xamlNamespaceResolver = new CustomXamlNamespaceResolver();
            _namespacePrefixLookup = new CustomNamespacePrefixLookup();
        }

        /// <summary>
        /// Tests XamlTypeName.Parse method.
        /// </summary>
        public void Parse()
        {
            XamlTypeName xamlTypeName = null;
            foreach (string testcase in _parseTestcases)
            {
                GlobalLog.LogDebug("Testing: [" + testcase + "]");
                xamlTypeName = XamlTypeName.Parse(testcase, _xamlNamespaceResolver);
                string serializedXamlTypeName = xamlTypeName.Serialize();
                CompareStringAndLog(serializedXamlTypeName, testcase);
            }
        }

        /// <summary>
        /// Tests XamlTypeName.TryParse method.
        /// </summary>
        public void TryParse()
        {
            XamlTypeName xamlTypeName = null;
            foreach (string testcase in _parseTestcases)
            {
                GlobalLog.LogDebug("Testing: [" + testcase + "]");
                bool returnVal = XamlTypeName.TryParse(testcase, _xamlNamespaceResolver, out xamlTypeName);
                string serializedXamlTypeName = xamlTypeName.Serialize();
                CompareBoolAndLog(returnVal, true);
                CompareStringAndLog(serializedXamlTypeName, testcase);
            }
        }

        /// <summary>
        /// Tests XamlTypeName.ParseList method.
        /// </summary>
        public void ParseList()
        {
            IList<XamlTypeName> xamlTypeNames = null;
            foreach (string testcase in _parseListTestcases)
            {
                GlobalLog.LogDebug("Testing: [" + testcase + "]");
                xamlTypeNames = XamlTypeName.ParseList(testcase, _xamlNamespaceResolver);
                string[] expected = testcase.Split(new char[] { ',' });
                for (int index = 0; index < xamlTypeNames.Count; index++)
                {
                    string serializedXamlTypeName = xamlTypeNames[index].Serialize();
                    CompareStringAndLog(serializedXamlTypeName, expected[index]);
                }
            }
        }

        /// <summary>
        /// Tests XamlTypeName.TryParseList method.
        /// </summary>
        public void TryParseList()
        {
            IList<XamlTypeName> xamlTypeNames = null;
            foreach (string testcase in _parseListTestcases)
            {
                GlobalLog.LogDebug("Testing: [" + testcase + "]");
                bool returnVal = XamlTypeName.TryParseList(testcase, _xamlNamespaceResolver, out xamlTypeNames);
                if (!CompareBoolAndLog(returnVal, true))
                {
                    string[] expected = testcase.Split(new char[] { ',' });
                    for (int index = 0; index < xamlTypeNames.Count; index++)
                    {
                        string serializedXamlTypeName = xamlTypeNames[index].Serialize();
                        CompareStringAndLog(serializedXamlTypeName, expected[index]);
                    }
                }
            }
        }

        /// <summary>
        /// Negative tests for XamlTypeName.Parse method.
        /// </summary>
        public void ParseNegative()
        {
            foreach (string testcase in _parseNegativeTestcases)
            {
                GlobalLog.LogDebug("Testing: [" + testcase + "]");
                ExceptionHelper.ExpectException<FormatException>(() => XamlTypeName.Parse(testcase, _xamlNamespaceResolver), new FormatException(), "InvalidCharInTypeName", WpfBinaries.SystemXaml);
            }
        }

        /// <summary>
        /// Negative tests for XamlTypeName.TryParse method.
        /// </summary>
        public void TryParseNegative()
        {
            XamlTypeName xamlTypeName = null;
            foreach (string testcase in _parseNegativeTestcases)
            {
                GlobalLog.LogDebug("Testing: [" + testcase + "]");
                bool returnVal = XamlTypeName.TryParse(testcase, _xamlNamespaceResolver, out xamlTypeName);
                CompareBoolAndLog(returnVal, false);
                if (xamlTypeName != null)
                {
                    GlobalLog.LogEvidence("xamlTypeName is not null");
                    TestLog.Current.Result = TestResult.Fail;
                }
            }
        }

        /// <summary>
        /// Negative tests for XamlTypeName.ParseList method.
        /// </summary>
        public void ParseListNegative()
        {
            foreach (string testcase in _parseListNegativeTestcases)
            {
                GlobalLog.LogDebug("Testing: [" + testcase + "]");
                ExceptionHelper.ExpectException<FormatException>(() => XamlTypeName.ParseList(testcase, _xamlNamespaceResolver), new FormatException(), "InvalidCharInTypeName", WpfBinaries.SystemXaml);
            }
        }

        /// <summary>
        /// Negative tests for XamlTypeName.TryParseList method.
        /// </summary>
        public void TryParseListNegative()
        {
            IList<XamlTypeName> xamlTypeNames = null;
            foreach (string testcase in _parseListNegativeTestcases)
            {
                GlobalLog.LogDebug("Testing: [" + testcase + "]");
                bool returnVal = XamlTypeName.TryParseList(testcase, _xamlNamespaceResolver, out xamlTypeNames);
                CompareBoolAndLog(returnVal, false);
                if (xamlTypeNames != null)
                {
                    GlobalLog.LogEvidence("xamlTypeNames is not null");
                    TestLog.Current.Result = TestResult.Fail;
                }
            }
        }

        /// <summary>
        /// Negative tests for all XamlTypeName methods with null arguments
        /// </summary>
        public void NullArgument()
        {
            XamlTypeName xamlTypeName;
            IList<XamlTypeName> xamlTypeNames;

            ExceptionHelper.ExpectException<ArgumentNullException>(() => XamlTypeName.Parse(null, _xamlNamespaceResolver), new ArgumentNullException("typeName"));
            ExceptionHelper.ExpectException<ArgumentNullException>(() => XamlTypeName.Parse("Type", null), new ArgumentNullException("namespaceResolver"));
            ExceptionHelper.ExpectException<ArgumentNullException>(() => XamlTypeName.Parse(null, null), new ArgumentNullException("typeName"));

            ExceptionHelper.ExpectException<ArgumentNullException>(() => XamlTypeName.TryParse(null, _xamlNamespaceResolver, out xamlTypeName), new ArgumentNullException("typeName"));
            ExceptionHelper.ExpectException<ArgumentNullException>(() => XamlTypeName.TryParse("Type", null, out xamlTypeName), new ArgumentNullException("namespaceResolver"));
            ExceptionHelper.ExpectException<ArgumentNullException>(() => XamlTypeName.TryParse(null, null, out xamlTypeName), new ArgumentNullException("typeName"));

            ExceptionHelper.ExpectException<ArgumentNullException>(() => XamlTypeName.ParseList(null, _xamlNamespaceResolver), new ArgumentNullException("typeNameList"));
            ExceptionHelper.ExpectException<ArgumentNullException>(() => XamlTypeName.ParseList("Type", null), new ArgumentNullException("namespaceResolver"));
            ExceptionHelper.ExpectException<ArgumentNullException>(() => XamlTypeName.ParseList(null, null), new ArgumentNullException("typeNameList"));

            ExceptionHelper.ExpectException<ArgumentNullException>(() => XamlTypeName.TryParseList(null, _xamlNamespaceResolver, out xamlTypeNames), new ArgumentNullException("typeNameList"));
            ExceptionHelper.ExpectException<ArgumentNullException>(() => XamlTypeName.TryParseList("Type", null, out xamlTypeNames), new ArgumentNullException("namespaceResolver"));
            ExceptionHelper.ExpectException<ArgumentNullException>(() => XamlTypeName.TryParseList(null, null, out xamlTypeNames), new ArgumentNullException("typeNameList"));

            xamlTypeName = XamlTypeName.Parse("prefixA:TypeA", _xamlNamespaceResolver);

            xamlTypeNames = XamlTypeName.ParseList("prefixA:TypeA,prefixB:TypeB", _xamlNamespaceResolver);
            ExceptionHelper.ExpectException<ArgumentNullException>(() => XamlTypeName.ToString(null, _namespacePrefixLookup), new ArgumentNullException("typeNameList"));
            ExceptionHelper.ExpectException<ArgumentNullException>(() => XamlTypeName.ToString(null, null), new ArgumentNullException("typeNameList"));
        }

        /// <summary>
        /// Tests XamlTypeName.ToString(INamespacePrefixLookup) method.
        /// </summary>
        public void MakeString()
        {
            XamlTypeName xamlTypeName = null;
            foreach (string testcase in _parseTestcases)
            {
                GlobalLog.LogDebug("Testing: [" + testcase + "]");
                xamlTypeName = XamlTypeName.Parse(testcase, _xamlNamespaceResolver);
                string madeString = xamlTypeName.ToString(_namespacePrefixLookup);
                string normalizedInputString = NormalizeCommas(testcase);
                CompareStringAndLog(madeString, normalizedInputString);
            }
        }

        /// <summary>
        /// Tests XamlTypeName.ToString(IList of XamlTypeName, INamespacePrefixLookup) method.
        /// </summary>
        public void MakeStringList()
        {
            IList<XamlTypeName> xamlTypeNames = null;
            foreach (string testcase in _parseListTestcases)
            {
                GlobalLog.LogDebug("Testing: [" + testcase + "]");
                xamlTypeNames = XamlTypeName.ParseList(testcase, _xamlNamespaceResolver);
                string madeString = XamlTypeName.ToString(xamlTypeNames, _namespacePrefixLookup);
                string normalizedInputString = NormalizeCommas(testcase);
                CompareStringAndLog(madeString, normalizedInputString);
            }
        }

        /// <summary>
        /// Regression test : verify xmlns work correctly with newly loaded assemblies
        /// </summary>
        public void XmlNsOnNewAssemblies()
        {
            AppDomain isolatedDomain = AppDomain.CreateDomain("IsolatedDomain");
            IsolatedAppDomain isolated = (IsolatedAppDomain)isolatedDomain.CreateInstanceAndUnwrap(
                Assembly.GetExecutingAssembly().FullName,
                typeof(IsolatedAppDomain).FullName);

            isolated.RunTest();
        }

        #region Private helper methods

        /// <summary>
        /// Compare two strings. If they are different log error messages and flag test case failure
        /// </summary>
        /// <param name="returned">Returned value from a method.</param>
        /// <param name="expected">Expected value.</param>
        /// <returns>True of strings match. False otherwise.</returns>
        private bool CompareStringAndLog(string returned, string expected)
        {
            if (returned != expected)
            {
                GlobalLog.LogEvidence("Unexpected return value");
                GlobalLog.LogEvidence("Returned output: [" + returned + "]");
                GlobalLog.LogEvidence("Expected output: [" + expected + "]");
                TestLog.Current.Result = TestResult.Fail;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Compare two bools. If they are different log error messages and flag test case failure
        /// </summary>
        /// <param name="returned">Returned value from a method.</param>
        /// <param name="expected">Expected value.</param>
        /// <returns>True of bools match. False otherwise.</returns>
        private bool CompareBoolAndLog(bool returned, bool expected)
        {
            if (returned != expected)
            {
                GlobalLog.LogEvidence("Unexpected return value: " + returned.ToString());
                TestLog.Current.Result = TestResult.Fail;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Normalize string with a single space after a comma
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Normalized string.</returns>
        private string NormalizeCommas(string input)
        {
            return Regex.Replace(input, @",\s*", ", ");
        }

        /// <summary>
        /// Class to isoloate loading assemblies
        /// </summary>
        private class IsolatedAppDomain : MarshalByRefObject
        {
            /// <summary>
            /// Method called in new appdomain to isolate assembly loading
            /// </summary>
            internal void RunTest()
            {
                string xmlns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

                Assembly.LoadWithPartialName("WindowsBase");
                XamlSchemaContext context = new XamlSchemaContext();
                if (context.GetXamlType(new XamlTypeName(xmlns, "Vector")) == null)
                {
                    throw new Exception("Unable to load type Vector.  Most likely WindowsBase.dll was not loaded.");
                }

                if (context.GetXamlType(new XamlTypeName(xmlns, "FigureHorizontalAnchor")) != null)
                {
                    throw new Exception("Able to load type FigureHorizontalAnchor.  Most like PresentationFramework.dll has already been loaded into the AppDomain");
                }

                Assembly.LoadWithPartialName("PresentationFramework");
                if (context.GetXamlType(new XamlTypeName(xmlns, "FigureHorizontalAnchor")) == null)
                {
                    throw new Exception("Unable to load type FigureHorizontalAnchor after PresentationFramework.dll was loaded.");
                }
            }
        }
        #endregion
    }
}
