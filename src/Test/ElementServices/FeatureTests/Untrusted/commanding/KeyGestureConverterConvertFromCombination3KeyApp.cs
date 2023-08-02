// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;

using System.ComponentModel;
using System.Globalization;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
using System.Threading;

using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.Parser.Common;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.Commanding
{
    /// <summary>
    /// Verify KeyGestureConverter ConvertFrom method for three Modifier combination keys.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <



    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify KeyGestureConverter ConvertFrom method for three Modifier combination key.")]
    [TestCasePriority("2")]
    [TestCaseArea(@"Commanding\TypeConverter")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    public class KeyGestureConverterConvertFromCombination3KeyApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest() 
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new KeyGestureConverterConvertFromCombination3KeyApp();
            Debug.Assert( app!=null, "App does not exist!");
            CoreLogger.LogStatus("App object: "+app.ToString());

            CoreLogger.LogStatus("Running app...");
            app.RunTestApp();
            CoreLogger.LogStatus("App run!");
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
                _converter = TypeDescriptor.GetConverter(typeof(KeyGesture));
                Debug.Assert(_converter is KeyGestureConverter);

                // Tell the parser how to map XML namespaces to CLR namespaces and assemblies.
                CoreLogger.LogStatus("Getting Type Descriptor Context....");
                ParserContext pc = new ParserContext();
                pc.XamlTypeMapper = XamlTypeMapper.DefaultMapper;
                pc.XamlTypeMapper.AddMappingProcessingInstruction ("inputcore", "System.Windows.Input", "PresentationCore");
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
            CoreLogger.LogStatus("Validating...");

            // Note: for this test we are concerned about whether the proper conversion occurred.
            
            CoreLogger.LogStatus("Constructing conversion table showing invalid conversions...");

            int nConversionFails = 0;
            int nConversionPasses = 0;

            // Loop through valid strings
            for (int i = 0; i < _convertedKeys.Length; i++)
            {
                _convertedKeys[i] = (_converter.ConvertFrom( _typeDescriptorContext, CultureInfo.InvariantCulture,
                    ValidKeyStrings[i]));

                if (_convertedKeys[i] == null)
                {
                    // Not a valid conversion
                    CoreLogger.LogStatus("X " + ValidKeyStrings[i]);
                    nConversionFails++;
                }
                else
                {
                    // Valid conversion - safe to typecast!
                    KeyGesture k = (KeyGesture)_convertedKeys[i];
                    CoreLogger.LogStatus("O "+ValidKeyStrings[i]+" (Key="+k.ToString()+")");
                    nConversionPasses++;
                }
            }

            CoreLogger.LogStatus("Total conversion failures: " + nConversionFails);
            CoreLogger.LogStatus("Total conversion successes: " + nConversionPasses);

            bool actual = (nConversionFails == 0);
            bool expected = true;
            CoreLogger.LogStatus("Converted? " + actual + ", expected: "+expected);

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
        private object[] _convertedKeys = new object[ValidKeyStrings.Length];

        /// <summary>
        /// Strings representing valid Key enumeration values.
        /// </summary>
        public static string[] ValidKeyStrings = {
            "Ctrl+Alt+Shift+F14",
            "Ctrl+Shift+Alt+V",
            "Shift+Ctrl+Alt+9",
            "Shift+Alt+Ctrl+f16",
            "Alt+Shift+Ctrl+n",
            "Alt+Ctrl+Shift+Space",
        };
    }
}
