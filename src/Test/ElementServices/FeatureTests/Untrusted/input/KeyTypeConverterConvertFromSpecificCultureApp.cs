// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.ComponentModel;

using System.Globalization;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
using System.Threading;
using System.Windows.Threading;

using Avalon.Test.CoreUI.Common;

using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.Parser.Common;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify KeyConverter ConvertFrom method for all specific and neutral cultures.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class KeyConverterConvertFromSpecificCultureApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\TypeConverter","Window",@"Compile and KeyConverter ConvertFrom method for all specific and neutral cultures in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);


            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "KeyConverterConvertFromSpecificCultureApp",
                "Run", 
                hostType,null,null );
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\TypeConverter","HwndSource",@"KeyConverter ConvertFrom method for all specific and neutral cultures in HwndSource.")]
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new KeyConverterConvertFromSpecificCultureApp(),"Run");
            
        }


        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender) 
        {
            
            {
                CoreLogger.LogStatus("Getting Converter....");
                _converter = TypeDescriptor.GetConverter(typeof(Key));

                // Tell the parser how to map XML namespaces to CLR namespaces and assemblies.
                CoreLogger.LogStatus("Getting Type Descriptor Context....");
                ParserContext pc = new ParserContext();
                pc.XamlTypeMapper = XamlTypeMapper.DefaultMapper;
                pc.XamlTypeMapper.AddMappingProcessingInstruction ("inputbase", "System.Windows.Input", "WindowsBase");
                _typeDescriptorContext = new TestTypeDescriptorContext(pc);
            }
            return null;
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object arg)
        {
            CoreLogger.LogStatus("Validating..."); ;

            // Note: for this test we are concerned about whether the proper conversion occurred.
            // If proper conversions, we get a Key.

            CoreLogger.LogStatus("Constructing conversion table showing invalid conversions...");

            int nConversionFails = 0;
            int nConversionPasses = 0;

            CultureInfo[][] cultureGroup = {
                CultureInfo.GetCultures(CultureTypes.NeutralCultures),
                CultureInfo.GetCultures(CultureTypes.SpecificCultures),
            };

            foreach (CultureInfo[] cgItem in cultureGroup)
            {
                foreach (CultureInfo ci in cgItem)
                {
                    CoreLogger.LogStatus("Culture: " + ci.Name + " (" + ci.EnglishName + ")");
                    for (int i = 0; i < _convertedKeys.Length; i++)
                    {
                        try
                        {
                            _convertedKeys[i] = (_converter.ConvertFrom(_typeDescriptorContext, ci, KeyStrings[i]));
                        }
                        catch (ArgumentException)
                        {
                            _convertedKeys[i] = null;
                        }

                        if (_convertedKeys[i] == null)
                        {
                            CoreLogger.LogStatus("X " + KeyStrings[i]);
                            nConversionFails++;
                        }
                        else
                        {
                            nConversionPasses++;
                        }
                    }
                }
            }

            CoreLogger.LogStatus("Total conversion failures: " + nConversionFails);
            CoreLogger.LogStatus("Total conversion successes: " + nConversionPasses);

            bool actual = (nConversionFails == 0);
            bool expected = true;
            CoreLogger.LogStatus("Converted? " + actual + ", expected: " + expected);

            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        private TypeConverter _converter;

        /// <summary>
        /// Store a type descriptor context, for use with the ConvertFrom API.
        /// </summary>
        private ITypeDescriptorContext _typeDescriptorContext;

        /// <summary>
        /// Keys converted from strings.
        /// </summary>
        private object[] _convertedKeys = new object[KeyStrings.Length];

        /// <summary>
        /// Strings representing all valid Key enumeration values.
        /// </summary>
        public static string[] KeyStrings = {
            // Alphanumeric strings, uppercase and lowercase
            "T",
            "t",
            
            // Non-alphanumeric strings, uppercase and lowercase
            "F7",
            "f7",
            
            // These two strings test for 'Turkish I' problem.
            "I",
            "i",

            // Cursor keys, uppercase and lowercase
            "Up",
            "UP",
            "up",
        };
    }
}

