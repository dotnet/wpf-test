// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
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
    /// Verify CursorConverter ConvertTo method with current culture.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class CursorTypeConverterConvertToApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Cursor","Browser",@"Compile and Verify CursorConverter ConvertTo method with current culture in Browser.")]
        [TestCase("2",@"CoreInput\Cursor","Window",@"Compile and Verify CursorConverter ConvertTo method with current culture in window.")]        
        [TestCaseSupportFile(@"star.cur")]
        [TestCaseSupportFile(@"anitest.ani")]
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);
            string[] contents = {"anitest.ani","star.cur"};

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "CursorTypeConverterConvertToApp",
                "Run", 
                hostType,null,contents );
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1",@"CoreInput\Cursor","HwndSource",@"Verify CursorConverter ConvertTo method with current culture in HwndSource.")]      
        [TestCaseSupportFile(@"star.cur")]
        [TestCaseSupportFile(@"anitest.ani")]        
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new CursorTypeConverterConvertToApp(),"Run");
            
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender) 
        {
            _aniFile = "anitest.ani";
            _curFile = "star.cur";

            if (!File.Exists(_aniFile))
            {
                CoreLogger.LogStatus("The file " + _aniFile + " was not found.");
            }

            if (!File.Exists(_curFile))
            {
                CoreLogger.LogStatus("The file " + _curFile + " was not found.");
            }

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

            // Note: for this test we are concerned about whether the proper conversion occurs.
            
            // expect matching conversion results
            string actualString = (_converter.ConvertTo(null, CultureInfo.CurrentCulture, Cursors.Hand, typeof(string))) as string;

            string actualStringCur = (_converter.ConvertTo(null, CultureInfo.CurrentCulture, IOHelper.LoadCursorObjectFromFile(_curFile), typeof(string))) as string;

            string actualStringAni = (_converter.ConvertTo(null, CultureInfo.CurrentCulture, IOHelper.LoadCursorObjectFromFile(_aniFile), typeof(string))) as string;
            
            InstanceDescriptor actualInstanceDescriptor;
            try
            {
                actualInstanceDescriptor = (_converter.ConvertTo(null, CultureInfo.CurrentCulture, Cursors.Hand, typeof(InstanceDescriptor))) as InstanceDescriptor;
            }
            catch (NotSupportedException e)
            {
                // expected this one!
                CoreLogger.LogStatus("Expected exception: " + e.Message);
                actualInstanceDescriptor = null;
            }
            CoreLogger.LogStatus("Converts to string, instance descriptor? (true,false) " + (actualString != null) + "," + (actualInstanceDescriptor != null));
            CoreLogger.LogStatus("Converts to ani,cur string? (true,true) " + (actualStringCur != null) + "," + (actualStringAni != null));
            if (actualString!=null)
            {
                CoreLogger.LogStatus("Converted string: " + actualString);
            }
            if (actualStringCur!=null)
            {
                CoreLogger.LogStatus("Converted string: " + actualStringCur);
            }
            if (actualStringAni!=null)
            {
                CoreLogger.LogStatus("Converted string: " + actualStringAni);
            }

            // We only want to verify the converted strings are non-empty
            bool actual = (actualString!=null) && (actualString.Length > 0) &&
                          (actualStringCur!=null) && (actualStringCur.Length > 0) &&
                          (actualStringAni!=null) && (actualStringCur.Length > 0) &&
                          (actualInstanceDescriptor==null);
            bool expected = true;
            CoreLogger.LogStatus("Converted? " + actual + ", expected: "+expected);
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

