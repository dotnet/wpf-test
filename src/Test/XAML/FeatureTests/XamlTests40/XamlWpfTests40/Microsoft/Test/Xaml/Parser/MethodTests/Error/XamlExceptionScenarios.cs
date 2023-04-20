// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xaml;
using System.Xml;
using Microsoft.Test.Globalization;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.MethodTests.Error
{
    /// <summary>
    /// DuplicateProperty Tests
    /// </summary>
    public static class XamlExceptionScenarios
    {
        /// <summary> xamlFile name </summary>
        private static string s_xamlFileName = string.Empty;

        /// <summary>
        /// Verifies the XamlObjectReader exception thrown for a Trigger element without any properties.
        /// </summary>
        public static void VerifyEmptyTrigger()
        {
            GlobalLog.LogStatus("TEST: Verify the exception thrown for a Trigger element without any properties.");

            Initialize();  // Read the file name from the .xtc file.

            XmlReader xmlReader = XmlReader.Create(s_xamlFileName);
            object objectTree = System.Windows.Markup.XamlReader.Load(xmlReader);

            XamlSchemaContext context = System.Windows.Markup.XamlReader.GetWpfSchemaContext();
            XamlObjectReaderSettings settings = new XamlObjectReaderSettings { RequireExplicitContentVisibility = true };
            XamlObjectReader xamlObjectReader;
            
            ExceptionHelper.ExpectException(
                delegate { xamlObjectReader = new XamlObjectReader(objectTree, context, settings); },
                new System.Xaml.XamlObjectReaderException(),
                "ObjectReader_TypeNotVisible",
                WpfBinaries.SystemXaml);

            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// Verify the inner exception thrown when duplicate names are present inside a Template.
        /// </summary>
        public static void VerifyDuplicateNames()
        {
            GlobalLog.LogStatus("TEST: Verify the inner exception thrown when duplicate names are present inside a Template.");

            Initialize();

            try
            {
                using (XmlReader xmlReader = XmlReader.Create("DuplicateNamesInTemplate.xaml"))
                {
                    object obj = System.Windows.Markup.XamlReader.Load(xmlReader);
                    Console.WriteLine(obj.ToString());
                }
            }
            catch (System.Exception ex)
            {
                if (ex.GetType() == typeof(System.Windows.Markup.XamlParseException))
                {
                    if (ex.InnerException.GetType() == typeof(System.ArgumentException))
                    {
                        Assembly mscorlibAssembly = Assembly.GetAssembly(typeof(object));
                        if (Exceptions.CompareMessage(ex.InnerException.Message, "Argument_AddingDuplicate", mscorlibAssembly, "mscorlib"))
                        {
                            TestLog.Current.Result = TestResult.Pass;
                        }
                        else
                        {
                            GlobalLog.LogEvidence("FAIL.");
                            TestLog.Current.Result = TestResult.Fail;
                        }
                    }
                    else
                    {
                        GlobalLog.LogEvidence("FAIL. Expected InnerException: System.ArgumentException / Actual: " + ex.InnerException.GetType().ToString()); 
                        TestLog.Current.Result = TestResult.Fail;
                    }
                }
                else
                {
                    GlobalLog.LogEvidence("FAIL. Expected Exception: System.Windows.Markup.XamlParseException / Actual: " + ex.GetType().ToString());
                    TestLog.Current.Result = TestResult.Fail;
                }    
            }
        }

        /// <summary>
        /// Carries out initial actions common to each test.
        /// </summary>
        private static void Initialize()
        {
            FrameworkElement fe = new FrameworkElement();
            fe = null;

            s_xamlFileName = DriverState.DriverParameters["TestParams"];
        }
    }
}
