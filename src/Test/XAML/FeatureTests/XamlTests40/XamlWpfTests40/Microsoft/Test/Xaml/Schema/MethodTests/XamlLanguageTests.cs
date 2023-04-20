// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Schema.MethodTests
{
    using System;
    using System.Collections.Generic;
    using System.Xaml;
    using System.Xaml.Schema;
    using Logging;

    /// <summary>
    /// Test to cover System.Xaml.XamlLanguage API
    /// </summary>
    public class XamlLanguageTests
    {
        /// <summary>
        /// Expected directive names from XamlLanguage.AllDirectives
        /// </summary>
        private static readonly List<string> s_expectedDirectives = new List<string>()
                                                                      {
                                                                          "Arguments",
                                                                          "Class",
                                                                          "ClassAttributes",
                                                                          "ClassModifier",
                                                                          "ConnectionId",
                                                                          "FactoryMethod",
                                                                          "FieldModifier",
                                                                          "Key",
                                                                          "Members",
                                                                          "Name",
                                                                          "SynchronousMode",
                                                                          "TypeArguments",
                                                                          "AsyncRecords",
                                                                          "Code",
                                                                          "Shared",
                                                                          "Subclass",
                                                                          "Uid",
                                                                          "space",
                                                                          "base",
                                                                          "lang",
                                                                          "_Items",
                                                                          "_Initialization",
                                                                          "_PositionalParameters",
                                                                          "_UnknownContent"
                                                                      };

        /// <summary>
        /// Expected XamlType names from XamlLanguage.AllTypes
        /// </summary>
        private static readonly List<string> s_expectedTypes = new List<string>()
                                                                 {
                                                                     "ArrayExtension",
                                                                     "Boolean",
                                                                     "Byte",
                                                                     "Char",
                                                                     "Decimal",
                                                                     "Double",
                                                                     "Int16",
                                                                     "Int32",
                                                                     "Int64",
                                                                     "Member",
                                                                     "NullExtension",
                                                                     "Object",
                                                                     "Property",
                                                                     "Reference",
                                                                     "Single",
                                                                     "StaticExtension",
                                                                     "String",
                                                                     "TimeSpan",
                                                                     "TypeExtension",
                                                                     "Uri",
                                                                     "XData"
                                                                 };

        /// <summary>
        ///  Tests the XamlLanguage.AllDirectives api 
        /// </summary>
        public void VerifyAllDirectives()
        {
            List<string> actualDirectives = new List<string>();
            foreach (XamlMember directiveProperty in XamlLanguage.AllDirectives)
            {
                actualDirectives.Add(directiveProperty.Name);
            }

            if (!UnOrderedCompare(s_expectedDirectives, actualDirectives))
            {
                GlobalLog.LogDebug("Expected directives = " + string.Join("\r\n", s_expectedDirectives.ToArray()));
                GlobalLog.LogDebug("Actual directives = " + string.Join("\r\n", actualDirectives.ToArray()));
                TestLog.Current.Result = TestResult.Fail;
            }

            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// Tests the XamlLanguage.AllTypes api
        /// </summary>
        public void VerifyAllTypes()
        {
            List<string> actualTypes = new List<string>();
            foreach (XamlType type in XamlLanguage.AllTypes)
            {
                actualTypes.Add(type.Name);
            }

            if (!UnOrderedCompare(s_expectedTypes, actualTypes))
            {
                GlobalLog.LogDebug("Expected type = " + string.Join("\r\n", s_expectedTypes.ToArray()));
                GlobalLog.LogDebug("Actual type = " + string.Join("\r\n", actualTypes.ToArray()));
                TestLog.Current.Result = TestResult.Fail;
            }

            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        ///  Makes sure that the directives returned by XamlLanguage is the
        ///  same as the one returned by XamlSchemaContext
        /// </summary>
        public void DirectivesMatchSchemaContext()
        {
            XamlSchemaContext xamlSchemaContext = new XamlSchemaContext();
            bool passed = true;
            foreach (XamlMember directiveProperty in XamlLanguage.AllDirectives)
            {
                foreach (string ns in directiveProperty.GetXamlNamespaces())
                {
                    XamlMember directivePropertyFromSchemaContext = xamlSchemaContext.GetXamlDirective(ns, directiveProperty.Name);

                    if (directivePropertyFromSchemaContext != directiveProperty)
                    {
                        GlobalLog.LogDebug("Directive property does not match XamlSchemaContext - " + directiveProperty);
                        passed = false;
                    }
                }
            }

            if (!passed)
            {
                TestLog.Current.Result = TestResult.Fail;
            }
            else
            {
                TestLog.Current.Result = TestResult.Pass;
            }
        }

        /// <summary>
        /// Verifies that the Types returned by XamlLanguage are the same as the ones
        /// returned by XamlSchemaContext
        /// </summary>
        public void TypesMatchSchemaContext()
        {
            XamlSchemaContext xamlSchemaContext = new XamlSchemaContext();
            bool passed = true;
            foreach (XamlType languageType in XamlLanguage.AllTypes)
            {
                foreach (string ns in languageType.GetXamlNamespaces())
                {
                    XamlType typeFromSchemaContext = xamlSchemaContext.GetXamlType(new XamlTypeName(ns, languageType.Name));
                    if (typeFromSchemaContext != languageType)
                    {
                        GlobalLog.LogDebug("XamlType does not match the one from XamlSchemaContext - " + languageType);
                        passed = false;
                    }
                }
            }

            if (!passed)
            {
                TestLog.Current.Result = TestResult.Fail;
            }
            else
            {
                TestLog.Current.Result = TestResult.Pass;
            }
        }

        /// <summary>
        /// Tests if the XamlLanguage types roundtrip properly
        /// </summary>
        public void TypesRoundTrip()
        {
            string[] xamlStrings = new string[]
            {
                @"<x:Boolean xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">False</x:Boolean>",
                @"<x:Char xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""></x:Char>",
                @"<x:String xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""></x:String>",
                @"<x:Int16 xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">0</x:Int16>",
                @"<x:Int32 xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">0</x:Int32>",
                @"<x:Int64 xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">0</x:Int64>",
                @"<x:Byte xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">0</x:Byte>",
                @"<x:Single xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">0</x:Single>",
                @"<x:Double xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">0</x:Double>",
                @"<x:Decimal xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">0</x:Decimal>",
                @"<x:Object xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" />",
                @"<x:Uri xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">http://testuri</x:Uri>",
                @"<x:TimeSpan xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">00:00:00</x:TimeSpan>",
                @"<x:Array Type=""x:String"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" />",
                @"<x:Null xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" />",
                @"<x:Type Type=""x:Object"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" />",
                @"<SelfReferencer Ref=""{x:Reference __ReferenceID0}"" x:Name=""__ReferenceID0"" xmlns=""clr-namespace:Microsoft.Test.Xaml.Schema.MethodTests;assembly=XamlWpfTests40"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" />",
            };

            bool passed = true;
            foreach (string xaml in xamlStrings)
            {
                if (RoundTrip(xaml) == false)
                {
                    passed = false;
                }
            }

            if (!passed)
            {
                TestLog.Current.Result = TestResult.Fail;
            }
            else
            {
                TestLog.Current.Result = TestResult.Pass;
            }
        }

        /// <summary>
        /// Compare list of two strings - unordered 
        /// </summary>
        /// <param name="expected"> The first list </param>
        /// <param name="actual"> The second list </param>
        /// <returns>returns true if equal</returns>
        private bool UnOrderedCompare(List<string> expected, List<string> actual)
        {
            expected.Sort();
            actual.Sort();

            if (expected.Count != actual.Count)
            {
                GlobalLog.LogEvidence("Expected Count [" + expected.Count + "] Actual Count [" + actual.Count + "]");
                return false;
            }

            for (int i = 0; i < expected.Count; i++)
            {
                if (expected[i] != actual[i])
                {
                    GlobalLog.LogEvidence("Expected [" + expected[i] + "] Actual [" + actual[i] + "]");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Round trips the input xaml through a load/save cycle
        /// </summary>
        /// <param name="inputXaml">Input xaml</param>
        /// <returns>true if round trip was successful</returns>
        private bool RoundTrip(string inputXaml)
        {
            GlobalLog.LogDebug("Roundtripping: " + inputXaml);
            string outputXaml = XamlServices.Save(XamlServices.Parse(inputXaml));

            if (outputXaml == inputXaml)
            {
                return true;
            }
            else
            {
                GlobalLog.LogEvidence("Expected xaml: " + inputXaml);
                GlobalLog.LogEvidence("Returned xaml: " + outputXaml);
                return false;
            }
        }
    }

    /// <summary>
    /// Type used to test x:Reference
    /// </summary>
    public class SelfReferencer
    {
        /// <summary>
        /// Gets or sets a SelfReference object
        /// </summary>
        public SelfReferencer Ref { get; set; }
    }
}
