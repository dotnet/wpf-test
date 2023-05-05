// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Windows;
using System.Xaml;
using System.Xml;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.NonParserMethodTests
{
    /// <summary>
    /// XamlObjectWriterSettings BasicTests
    /// </summary>
    public static class XamlObjectWriterSettingsTests
    {
        /// <summary>The name of the .xaml file to be tested.</summary>
        private static string s_xamlFileName = string.Empty;

        /// <summary>
        /// Verify setting PreferUnconvertedDictionaryKeys to true, given markup containing
        /// a CharacterMetricsDictionary containing CharacterMetrics objects with invalid keys.
        /// Note: CharacterMetricsDictionary is tested because it:
        ///   o Implements IDictionary
        ///   o Implements IDictionary K V
        ///   o Performs custom conversion in IDictionary.Add(object) that is different from what the default TypeConverter does
        /// </summary>
        public static void PreferUnconvertedDictionaryKeysTrue()
        {
            s_xamlFileName = Initialize();

            using (XmlReader xmlReader = XmlReader.Create(s_xamlFileName))
            {
                XamlXmlReader xtr = new XamlXmlReader(xmlReader);

                XamlObjectWriterSettings settings = new XamlObjectWriterSettings();
                settings.PreferUnconvertedDictionaryKeys = true;
                XamlObjectWriter ow = new XamlObjectWriter(xtr.SchemaContext, settings);

                XamlServices.Transform(xtr, ow);

                if (ow.Result != null)
                {
                    TestLog.Current.Result = TestResult.Pass;
                }
                else
                {
                    TestLog.Current.Result = TestResult.Fail;
                }
            }
        }

        /// <summary>
        /// Verify setting PreferUnconvertedDictionaryKeys to true, given markup containing
        /// a CharacterMetricsDictionary containing CharacterMetrics objects with invalid keys.
        /// Note: CharacterMetricsDictionary is tested because it:
        ///   o Implements IDictionary
        ///   o Implements IDictionary K V
        ///   o Performs custom conversion in IDictionary.Add(object) that is different from what the default TypeConverter does
        /// </summary>
        public static void PreferUnconvertedDictionaryKeysFalse()
        {
            s_xamlFileName = Initialize();

            using (XmlReader xmlReader = XmlReader.Create(s_xamlFileName))
            {
                XamlXmlReader xtr = new XamlXmlReader(xmlReader);

                XamlObjectWriterSettings settings = new XamlObjectWriterSettings();
                settings.PreferUnconvertedDictionaryKeys = false;
                XamlObjectWriter ow = new XamlObjectWriter(xtr.SchemaContext, settings);

                ExceptionHelper.ExpectException<XamlObjectWriterException>(() => XamlServices.Transform(xtr, ow), new XamlObjectWriterException("TypeConverterFailed"));
            }
        }

        /// <summary>
        /// Load a WPF assembly and retrieve the xaml file name from the .xtc file.
        /// </summary>
        /// <returns>The name of the .xaml file to be tested.</returns>
        public static string Initialize()
        {
            bool loadWPFAssembly = Convert.ToBoolean(DriverState.DriverParameters["LoadWPFAssembly"]);

            if (loadWPFAssembly)
            {
                FrameworkElement frameworkElement = new FrameworkElement();
                frameworkElement = null;
            }

            string xamlFileName = DriverState.DriverParameters["XamlFileName"];

            if (String.IsNullOrEmpty(xamlFileName))
            {
                throw new TestSetupException("XamlFileName cannot be null.");
            }

            if (!File.Exists(xamlFileName))
            {
                throw new TestSetupException("ERROR: the Xaml file specified does not exist.");
            }

            return xamlFileName;
        }
    }
}
