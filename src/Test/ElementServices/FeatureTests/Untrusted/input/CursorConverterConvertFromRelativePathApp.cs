// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.ComponentModel;
using System.Globalization;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Avalon.Test.CoreUI.Parser.Common;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify CursorConverter ConvertFrom method on string with relative path.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <


    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class CursorConverterConvertFromRelativePathApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"CoreInput\Cursor", "HwndSource", @"1", TestCaseSecurityLevel.FullTrust, @"Verify CursorConverter ConvertFrom method on string with relative path in HwndSource.")]
        [TestCase("2", @"CoreInput\Cursor", "Window", @"1", TestCaseSecurityLevel.FullTrust, @"Verify CursorConverter ConvertFrom method on string with relative path in Window.")]
        [TestCaseSupportFile(@"star.cur")]
        [TestCaseSupportFile(@"anitest.ani")]
        public static void LaunchTest()
        {
            ExeStubContainerCore exe = new ExeStubContainerCore();
            exe.Run(new CursorConverterConvertFromRelativePathApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            _aniFile = @"anitest.ani";
            _curFile = @"star.cur";

            if (!File.Exists(_aniFile))
            {
                CoreLogger.LogStatus("The file " + _aniFile + " was not found in working directory");
            }

            if (!File.Exists(_curFile))
            {
                CoreLogger.LogStatus("The file " + _curFile + " was not found in working directory");
            }

            CoreLogger.LogStatus("Getting Converter....");
            _converter = TypeDescriptor.GetConverter(typeof(Cursor));

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
            CoreLogger.LogStatus("Validating...");

            // Note: for this test we are concerned about whether the proper conversion occurred.
            // If proper conversions, we get a Cursor.

            CoreLogger.LogStatus("Validating conversion from string (cur file)...");
            Cursor actualCursorCur = (_converter.ConvertFrom(_typeDescriptorContext, CultureInfo.InvariantCulture, _curFile)) as Cursor;

            CoreLogger.LogStatus("Validating conversion from string (ani file)...");
            Cursor actualCursorAni = (_converter.ConvertFrom(_typeDescriptorContext, CultureInfo.InvariantCulture, _aniFile)) as Cursor;

            CoreLogger.LogStatus("cur,ani?  (expect true,true) " + (actualCursorCur != null) + "," + (actualCursorAni != null));

            bool actual = (actualCursorCur != null) && (actualCursorAni != null);
            bool expected = true;
            CoreLogger.LogStatus("Converted? " + actual + ", expected: " + expected);

            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        private string _aniFile;
        private string _curFile;

        private TypeConverter _converter;

        /// <summary>
        /// Store a type descriptor context, for use with the ConvertFrom API.
        /// </summary>
        private ITypeDescriptorContext _typeDescriptorContext;

    }
}

