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
using System.Windows.Media;

using Avalon.Test.CoreUI.Common;

using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Microsoft.Test.Win32;

using Microsoft.Test.Serialization;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify CursorConverter ConvertFrom method.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class CursorTypeConverterConvertFromApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Cursor", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify CursorConverter ConvertFrom method in HwndSource.")]
        [TestCase("2", @"CoreInput\Cursor", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify CursorConverter ConvertFrom method in window.")]
        [TestCaseSupportFile(@"star.cur")]
        [TestCaseSupportFile(@"anitest.ani")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);
            string[] contents = { "anitest.ani", "star.cur" };

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "CursorTypeConverterConvertFromApp",
                "Run",
                hostType, null, contents);
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"CoreInput\Cursor", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify CursorConverter ConvertFrom method in HwndSource.")]
        [TestCase("2", @"CoreInput\Cursor", "Window", TestCaseSecurityLevel.FullTrust, @"Verify CursorConverter ConvertFrom method in Window.")]
        [TestCaseSupportFile(@"star.cur")]
        [TestCaseSupportFile(@"anitest.ani")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new CursorTypeConverterConvertFromApp(), "Run");
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            string assemblyPath = GetDirectoryNameOfAssembly();
            _aniFile = assemblyPath + "\\anitest.ani";
            _curFile = assemblyPath + "\\star.cur";

            if (!File.Exists(_aniFile))
            {
                CoreLogger.LogStatus("The file " + _aniFile + " was not found at " + assemblyPath);
            }

            if (!File.Exists(_curFile))
            {
                CoreLogger.LogStatus("The file " + _curFile + " was not found at " + assemblyPath);
            }

            Cursor cur = IOHelper.LoadCursorObjectFromFile(_curFile);
            CoreLogger.LogStatus("Loaded cursor...");

            CoreLogger.LogStatus("Getting Converter....");
            _converter = TypeDescriptor.GetConverter(typeof(Cursor));

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

            CoreLogger.LogStatus("Validating conversion from string...");
            Cursor actualCursorHand = (_converter.ConvertFrom(null, CultureInfo.InvariantCulture, "Hand")) as Cursor;

            CoreLogger.LogStatus("Validating conversion from string (cur file)...");
            Cursor actualCursorCur = (_converter.ConvertFrom(null, CultureInfo.InvariantCulture, _curFile)) as Cursor;

            CoreLogger.LogStatus("Validating conversion from string (ani file)...");
            Cursor actualCursorAni = (_converter.ConvertFrom(null, CultureInfo.InvariantCulture, _aniFile)) as Cursor;

            CoreLogger.LogStatus("Validating conversion from cursor...");
            Cursor actualCursorAppStarting;

            try
            {
                actualCursorAppStarting = (_converter.ConvertFrom(null, CultureInfo.InvariantCulture, Cursors.AppStarting)) as Cursor;
            }
            catch (NotSupportedException e)
            {
                // expected this one!
                CoreLogger.LogStatus("Expected exception: " + e.Message);
                actualCursorAppStarting = null;
            }

            CoreLogger.LogStatus("Validating conversion from int...");
            Cursor actualCursorScrollSE;

            try
            {
                actualCursorScrollSE = (_converter.ConvertFrom(null, CultureInfo.InvariantCulture, (int)(CursorType.ScrollSE))) as Cursor;
            }
            catch (NotSupportedException e)
            {
                // expected this one!
                CoreLogger.LogStatus("Expected exception: " + e.Message);
                actualCursorScrollSE = null;
            }
            CoreLogger.LogStatus("Hand,AppStarting,ScrollSE? (expect true,false,false) " + (actualCursorHand != null) + "," + (actualCursorAppStarting != null) + "," + (actualCursorScrollSE != null));
            CoreLogger.LogStatus("cur,ani?  (expect true,true) " + (actualCursorCur != null) + "," + (actualCursorAni != null));

            bool actual = (actualCursorHand != null) && (actualCursorAppStarting == null) && (actualCursorScrollSE == null) &&
                          (actualCursorCur != null) && (actualCursorAni != null);
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

    }
}

