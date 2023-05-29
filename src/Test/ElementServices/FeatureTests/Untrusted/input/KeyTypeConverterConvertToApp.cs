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
    /// Verify KeyConverter ConvertTo method for standard keys.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class KeyConverterConvertToApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"CoreInput\TypeConverter", "HwndSource", @"Verify KeyConverter ConvertTo method for standard keys HwndSource.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new KeyConverterConvertToApp(), "Run");

        }


        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {

            CoreLogger.LogStatus("Getting Converter....");
            _converter = TypeDescriptor.GetConverter(typeof(Key));

            // Tell the parser how to map XML namespaces to CLR namespaces and assemblies.
            CoreLogger.LogStatus("Getting Type Descriptor Context....");
            ParserContext pc = new ParserContext();
            pc.XamlTypeMapper = XamlTypeMapper.DefaultMapper;
            pc.XamlTypeMapper.AddMappingProcessingInstruction("inputbase", "System.Windows.Input", "WindowsBase");
            _typeDescriptorContext = new TestTypeDescriptorContext(pc);

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

            // Loop through valid strings
            int arrayLength = _convertedKeys.GetUpperBound(0) + 1;
            for (int i = 0; i < arrayLength; i++)
            {
                _convertedKeys[i] = (_converter.ConvertTo(_typeDescriptorContext, CultureInfo.InvariantCulture,
                    (Key)(KeysAndExpectedStrings[i, 0]), typeof(string)));

                if (_convertedKeys[i] == null)
                {
                    // Not a valid conversion
                    CoreLogger.LogStatus("X " + (Key)KeysAndExpectedStrings[i, 0]);
                    nConversionFails++;
                }
                else
                {
                    // Valid conversion - safe to typecast!
                    string k = (string)_convertedKeys[i];
                    string kExpected = (string)KeysAndExpectedStrings[i, 1];

                    CoreLogger.LogStatus("O " + KeysAndExpectedStrings[i, 0] + " (Key=" + k + ")");
                    if (k == kExpected)
                    {
                        nConversionPasses++;
                    }
                    else
                    {
                        nConversionFails++;
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
        /// Store a type descriptor context, for use with the ConvertTo API.
        /// </summary>
        private ITypeDescriptorContext _typeDescriptorContext;

        /// <summary>
        /// Keys converted from strings.
        /// </summary>
        private object[] _convertedKeys = new object[KeysAndExpectedStrings.GetUpperBound(0) + 1];

        /// <summary>
        /// Strings representing valid Key and expected string enumeration values.
        /// </summary>
        public static object[,] KeysAndExpectedStrings = new object[,] {
            { Key.U, "U" },
            { Key.F8, "F8" },
        };
    }
}

