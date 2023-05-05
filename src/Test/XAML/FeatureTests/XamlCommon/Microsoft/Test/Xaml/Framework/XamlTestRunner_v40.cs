// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using Microsoft.Test.Integration;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Parser;
using Microsoft.Test.Xaml.TestTypes;

namespace Microsoft.Test.Xaml.Framework
{
    /// <summary>
    /// Class for running Xaml Tests
    /// </summary>
    public static class XamlTestRunner
    {
        #region Static Methods

        /// <summary>
        /// Entry point for Xaml Test Framework
        /// </summary>
        public static void RunTest()
        {
            string testType = DriverState.DriverParameters["XamlTestType"].ToUpperInvariant();
            XamlTestType test;
            switch (testType)
            {
                case "LOADTEST":
                    test = new ParserLoadTest();
                    break;
                case "LOADEXCEPTIONTEST":
                    test = new ParserLoadExceptionTest();
                    break;
                case "METHODTEST":
                    test = new MethodTest();
                    break;
                case "MARKUPCOMPILERTEST":
                    test = new MarkupCompilerTest();
                    break;
                case "MARKUPCOMPILERERRORTEST":
                    test = new MarkupCompilerErrorTest();
                    break;
                case "XAMLROUNDTRIPTEST":
                    test = new XamlRoundTripTest();
                    break;
                case "XAMLTEXTREADERLOADTEST":
                    test = new XamlTextReaderLoadTest();
                    break;
                case "CDFMETHODTEST":
                    test = new CDFMethodTest();
                    break;
                case "BAMLDUMPTEST":
                    test = new BamlDumpTest();
                    break;
                case "BAMLREADERTEST":
                    test = new BamlReaderTest();
                    break;
                case "BAMLTOXAMLTEST":
                    test = new BamlToXamlTest();
                    break;
                case "BAMLREFERENCETEST":
                    test = new BamlReferenceTest();
                    break;
                case "BAMLTOOBJECTTEST":
                    test = new BamlToObjectTest();
                    break;
                case "XAMLTOXAMLTEST":
                    test = new XamlToXamlTest();
                    break;
                case "POMBAMLAPPTEST":
                    test = new PomBamlAppTest();
                    break;
                case "LOCBAMLTEST":
                    test = new LocBamlTest();
                    break;
                case "XAMLHARVESTERTEST":
                    test = new XamlHarvesterTest();
                    break;
                default:
                    throw new Exception("Unrecognized Test Type");
            }

            test.Run();
        }

        #endregion
    }
}
