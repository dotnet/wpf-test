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
    /// Verify KeyConverter ConvertFrom method for all keys.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <





    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify KeyConverter ConvertFrom method for all keys.")]
    [TestCasePriority("1")]
    [TestCaseArea(@"CoreInput\TypeConverter")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("1")]
    public class KeyConverterConvertFromAllKeysApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest() 
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new KeyConverterConvertFromAllKeysApp();
            Debug.Assert( app!=null, "App does not exist!");
            CoreLogger.LogStatus("App object: "+app.ToString());

            CoreLogger.LogStatus("Running app...");
            app.Run();
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

            for (int i = 0; i < _convertedKeys.Length; i++)
            {
                _convertedKeys[i] = (_converter.ConvertFrom( _typeDescriptorContext, CultureInfo.InvariantCulture,
                    KeyStrings[i]));

                if (_convertedKeys[i] == null)
                {
                    CoreLogger.LogStatus("X " + KeyStrings[i]);
                    nConversionFails++;
                }
                else
                {
                    CoreLogger.LogStatus("O "+KeyStrings[i]);
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
        private object[] _convertedKeys = new object[KeyStrings.Length];

        /// <summary>
        /// Strings representing all valid Key enumeration values.
        /// </summary>
        public static string[] KeyStrings = {
            "None",
            "Cancel",
            "Back",
            "Tab",
            "LineFeed",
            "Clear",
            "Return",
            "Enter",
            "Pause",
            "Capital",
            "CapsLock",
            "KanaMode",
            "HanguelMode",
            "HangulMode",
            "JunjaMode",
            "FinalMode",
            "HanjaMode",
            "KanjiMode",
            "Escape",
            "ImeConvert",
            "ImeNonConvert",
            "ImeAccept",
            "ImeModeChange",
            "Space",
            "Prior",
            "PageUp",
            "Next",
            "PageDown",
            "End",
            "Home",
            "Left",
            "Up",
            "Right",
            "Down",
            "Select",
            "Print",
            "Execute",
            "Snapshot",
            "PrintScreen",
            "Insert",
            "Delete",
            "Help",
            "D0",
            "D1",
            "D2",
            "D3",
            "D4",
            "D5",
            "D6",
            "D7",
            "D8",
            "D9",
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z",
            "LWin",
            "RWin",
            "Apps",
            "Sleep",
            "NumPad0",
            "NumPad1",
            "NumPad2",
            "NumPad3",
            "NumPad4",
            "NumPad5",
            "NumPad6",
            "NumPad7",
            "NumPad8",
            "NumPad9",
            "Multiply",
            "Add",
            "Separator",
            "Subtract",
            "Decimal",
            "Divide",
            "F1",
            "F2",
            "F3",
            "F4",
            "F5",
            "F6",
            "F7",
            "F8",
            "F9",
            "F10",
            "F11",
            "F12",
            "F13",
            "F14",
            "F15",
            "F16",
            "F17",
            "F18",
            "F19",
            "F20",
            "F21",
            "F22",
            "F23",
            "F24",
            "NumLock",
            "Scroll",
            "LeftShift",
            "RightShift",
            "LeftCtrl",
            "RightCtrl",
            "LeftAlt",
            "RightAlt",
            "BrowserBack",
            "BrowserForward",
            "BrowserRefresh",
            "BrowserStop",
            "BrowserSearch",
            "BrowserFavorites",
            "BrowserHome",
            "VolumeMute",
            "VolumeDown",
            "VolumeUp",
            "MediaNextTrack",
            "MediaPreviousTrack",
            "MediaStop",
            "MediaPlayPause",
            "LaunchMail",
            "SelectMedia",
            "LaunchApplication1",
            "LaunchApplication2",
            "Oem1",
            "OemSemicolon",
            "OemPlus",
            "OemComma",
            "OemMinus",
            "OemPeriod",
            "Oem2",
            "OemQuestion",
            "Oem3",
            "OemTilde",
            "Oem4",
            "OemOpenBrackets",
            "Oem5",
            "OemPipe",
            "Oem6",
            "OemCloseBrackets",
            "Oem7",
            "OemQuotes",
            "Oem8",
            "Oem102",
            "OemBackslash",
            "ImeProcessed",
            "TextInput",
            "Attn",
            "CrSel",
            "ExSel",
            "EraseEof",
            "Play",
            "Zoom",
            "NoName",
            "Pa1",
            "OemClear"
        };
    }
}

