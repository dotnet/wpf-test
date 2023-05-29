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
    /// Verify KeyGestureConverter ConvertFrom method for standard keys.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
    /// <


    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify KeyGestureConverter ConvertFrom method for standard keys.")]
    [TestCasePriority("1")]
    [TestCaseArea(@"Commanding\TypeConverter")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    public class KeyGestureConverterConvertFromApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");

            TestApp app = new KeyGestureConverterConvertFromApp();

            Debug.Assert(app != null, "App does not exist!");
            CoreLogger.LogStatus("App object: " + app.ToString());

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
            CoreLogger.LogStatus("Getting Converter....");
            _converter = TypeDescriptor.GetConverter(typeof(KeyGesture));
            Debug.Assert(_converter is KeyGestureConverter);

            // Tell the parser how to map XML namespaces to CLR namespaces and assemblies.
            CoreLogger.LogStatus("Getting Type Descriptor Context....");

            ParserContext pc = new ParserContext();

            pc.XamlTypeMapper = XamlTypeMapper.DefaultMapper;
            pc.XamlTypeMapper.AddMappingProcessingInstruction("inputcore", "System.Windows.Input", "PresentationCore");
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

            CoreLogger.LogStatus("Constructing conversion table showing invalid conversions...");

            int nConversionFails = 0;
            int nConversionPasses = 0;

            // Loop through valid strings
            for (int i = 0; i < _convertedKeys.Length; i++)
            {
                _convertedKeys[i] = (_converter.ConvertFrom(_typeDescriptorContext, CultureInfo.InvariantCulture,
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

                    if ((k.Key == ValidResultKeyGestures[i].Key) && (k.Modifiers == ValidResultKeyGestures[i].Modifiers))
                    {
                        CoreLogger.LogStatus("O " + ValidKeyStrings[i] + " (Key=" + k.ToString() + ")");
                        nConversionPasses++;
                    }
                    else
                    {
                        CoreLogger.LogStatus("X " + ValidKeyStrings[i] + " (Key=" + k.ToString() + ")");
                        nConversionFails++;
                    }
                }
            }

            CoreLogger.LogStatus("Total conversion failures (expect 0): " + nConversionFails);
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
        private object[] _convertedKeys = new object[ValidKeyStrings.Length];

        /// <summary>
        /// Strings representing valid Key enumeration values.
        /// </summary>
        public static string[] ValidKeyStrings = {
            "ctrl+a", "CTRL+A", "Ctrl+f13", "control+F13",
            "CONTROL+taB", "Control+Escape", "shift+Space", "SHIFT +f15",
            "Shift+ f18", "alt+f19", "Alt+f20", "ALT+f21",
            " ctrl+f22", "\tCONTROL+f23 ", "Shift + f24",
            "", "None",
        };

        /// <summary>
        /// Strings representing valid KeyGesture results from ValidKeyStrings.
        /// </summary>
        public static KeyGesture[] ValidResultKeyGestures = {
            new KeyGesture(Key.A, ModifierKeys.Control),
            new KeyGesture(Key.A, ModifierKeys.Control),
            new KeyGesture(Key.F13, ModifierKeys.Control),
            new KeyGesture(Key.F13, ModifierKeys.Control),
            new KeyGesture(Key.Tab, ModifierKeys.Control),
            new KeyGesture(Key.Escape, ModifierKeys.Control),
            new KeyGesture(Key.Space, ModifierKeys.Shift),
            new KeyGesture(Key.F15, ModifierKeys.Shift),
            new KeyGesture(Key.F18, ModifierKeys.Shift),
            new KeyGesture(Key.F19, ModifierKeys.Alt),
            new KeyGesture(Key.F20, ModifierKeys.Alt),
            new KeyGesture(Key.F21, ModifierKeys.Alt),
            new KeyGesture(Key.F22, ModifierKeys.Control),
            new KeyGesture(Key.F23, ModifierKeys.Control),
            new KeyGesture(Key.F24, ModifierKeys.Shift),
            new KeyGesture(Key.None, ModifierKeys.None),
            new KeyGesture(Key.None, ModifierKeys.None),
        };

    }
}

