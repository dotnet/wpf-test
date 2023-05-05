// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Xaml;
using Microsoft.Test;
using Microsoft.Test.Globalization;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.MethodTests.Error
{
    /// <summary>
    /// DuplicateProperty Tests
    /// </summary>
    public static class DuplicatePropertyTests
    {
        /// <summary> expectedXaml Property </summary>
        private static string s_expectedXamlProperty = string.Empty;

        /// <summary> expectedXaml Type </summary>
        private static string s_expectedXamlType = string.Empty;

        /// <summary> xamlFile name </summary>
        private static string s_xamlFile = string.Empty;

        /// <summary>
        /// Runs the test.
        /// </summary>
        public static void RunTest()
        {
            FrameworkElement fe = new FrameworkElement();
            fe = null;
            ParseArgs();
            GlobalLog.LogStatus("Loading: " + s_xamlFile);

            IXamlTestParser parser = XamlTestParserFactory.Create();

            XamlSchemaContext xSC = new XamlSchemaContext();
            string xNS = @"http://XamlTestTypes";
            XamlType xamlType = xSC.GetXamlType(xNS, s_expectedXamlType);
            XamlMember xamlMember = xamlType.GetMember(s_expectedXamlProperty);

            ExceptionHelper.ExpectException<XamlDuplicateMemberException>(() => parser.LoadXaml(s_xamlFile, null), new XamlDuplicateMemberException(xamlMember, xamlType), "DuplicateMemberSet", WpfBinaries.SystemXaml);
        }

        /// <summary>
        /// Parses the args.
        /// </summary>
        private static void ParseArgs()
        {
            string input = DriverState.DriverParameters["TestParams"];
            string[] args = input.Split(new string[1] { "|" }, StringSplitOptions.None);
            if (args.Length != 3)
            {
                throw new Exception("Invalid TestParams.  Must be: XamlFile,XamlType,XamlProperty");
            }
            else
            {
                s_expectedXamlProperty = args[0];
                s_expectedXamlType = args[1];
                s_xamlFile = args[2];
            }
        }
    }
}
