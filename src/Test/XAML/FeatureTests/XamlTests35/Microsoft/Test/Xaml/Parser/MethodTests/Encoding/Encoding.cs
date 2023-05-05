// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.IO;
using System.IO.Packaging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Threading;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test.Xaml.Parser;

namespace Microsoft.Test.Xaml.Parser.MethodTests.Encoding
{
    /// <summary>
    /// Test the encoding support in parser.
    /// </summary>
    /// <remarks>
    /// This is Parser BVT test that tries to parse files encoded with different encodings like
    /// UTF-7, UTF-8, UTF-16, UTF-32, local encodings, etc. and tries to make sure that parser should
    /// behave the way it is supposed to.
    /// </remarks>
    public class EncodingTest
    {
        #region RunTest
        /// <summary>
        /// Test case Entry point
        /// </summary>
        public void RunTest()
        {
            CreateContext();
            string strParams = DriverState.DriverParameters["TestParams"];

            GlobalLog.LogStatus("Core:Encoding Started ..." + "\n"); // Start the test

            switch (strParams)
            {
                case "UTF-16BE-BOM-noDeclare":
                    TestSuccessfulParsing(strParams + ".xaml");
                    break;

                case "UTF-16BE-BOM-falseDeclare":
                    TestFailedParsing(strParams + ".xaml");
                    break;

                case "UTF-16LE-BOM-noDeclare":
                    TestSuccessfulParsing(strParams + ".xaml");
                    break;

                case "UTF-7-noBOM-Declare":
                    TestFailedParsing(strParams + ".xaml");
                    break;

                case "UTF-7-noBOM-noDeclare":
                    TestFailedParsing(strParams + ".xaml");
                    break;

                // 


                case "UTF-8-BOM-Declare":
                    TestSuccessfulParsing(strParams + ".xaml");
                    break;

                case "UTF-8-BOM-noDeclare":
                    TestSuccessfulParsing(strParams + ".xaml");
                    break;

                case "ASCII-noBOM-Declare":
                    TestSuccessfulParsing(strParams + ".xaml");
                    break;

                default:
                    GlobalLog.LogStatus("Encoding.RunTest was called with an unsupported parameter.");
                    throw new Microsoft.Test.TestSetupException("Parameter is not supported");
            }
        }
        #endregion RunTest

        #region TestSuccessfulParsing
        /// <summary>
        /// Passes the test if the passed XAML file parses without an exception.
        /// </summary>
        /// <param name="filename">XAML filename</param>
        private void TestSuccessfulParsing(string filename)
        {
            UIElement root = null;
            IXamlTestParser parser = XamlTestParserFactory.Create();
            root = (UIElement)parser.LoadXaml(filename, null);

            // Exception not thrown, so parsed successfully.
            GlobalLog.LogStatus("Parser test passed");
            TestLog.Current.Result = TestResult.Pass;
        }
        #endregion TestSuccessfulParsing
        #region TestFailedParsing
        /// <summary>
        /// Passes the test if the passed XAML file throws an exception during parsing.
        /// </summary>
        /// <param name="filename">XAML filename</param>
        private void TestFailedParsing(string filename)
        {
            GlobalLog.LogStatus("Parse XAML file named " + filename);
            IXamlTestParser parser = XamlTestParserFactory.Create();
            if (filename.Contains("UTF-7"))
            {
                XamlParseException xpe = CreateXamlParseException(1, 1, null);
                ExceptionHelper.ExpectException(delegate { parser.LoadXaml(filename, null); }, xpe);
            }
            else
            {
                XamlParseException xpe = CreateXamlParseException(3, 12, null);
                ExceptionHelper.ExpectException(delegate { parser.LoadXaml(filename, null); }, xpe);
            }
        }

        /// <summary>
        /// Creates a XamlParseException with the BaseUri property set to pack://siteoforigin:,,,/
        /// </summary>
        XamlParseException CreateXamlParseException(int? lineNum, int? linePos, Exception innerException)
        {
            XamlParseException xpe = null;
            if (lineNum != null && linePos != null)
            {
                if (innerException != null)
                {
                    xpe = new XamlParseException(null, (int)lineNum, (int)linePos, innerException);
                }
                else
                {
                    xpe = new XamlParseException(null, (int)lineNum, (int)linePos);
                }
            }
            else
            {
                xpe = new XamlParseException();
            }

            return xpe;
        }
        #endregion TestFailedParsing
        #region Context
        static Dispatcher s_dispatcher;
        /// <summary>
        /// Creating Dispatcher
        /// </summary>
        private void CreateContext()
        {
            s_dispatcher = Dispatcher.CurrentDispatcher;
        }
        #endregion Context
    }
}
