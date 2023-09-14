// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Markup;
using System.Threading;
using System.Windows.Threading;

using Avalon.Test.CoreUI.Common;

using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Microsoft.Test.Win32;
using Microsoft.Test.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify intermediate points with ContentElement.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class GetIntermediatePointsContentElementApp : GetIntermediatePointsBaseApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Mouse", "HwndSource",TestCaseSecurityLevel.FullTrust, @"Compile and Verify intermediate points with ContentElement in HwndSource.")]
        [TestCase("2", @"CoreInput\Mouse", "Window", TestCaseSecurityLevel.FullTrust,@"Compile and Verify intermediate points with ContentElement in window.")]
        [TestCase("2", @"CoreInput\Mouse", "NavigationWindow", TestCaseSecurityLevel.FullTrust, @"Compile and Verify intermediate points with ContentElement in NavigationWindow.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "GetIntermediatePointsContentElementApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"CoreInput\Mouse", "HwndSource", TestCaseSecurityLevel.FullTrust,@"Verify intermediate points with ContentElement in HwndSource.")]
        [TestCase("2", @"CoreInput\Mouse", "Window",TestCaseSecurityLevel.FullTrust, @"Verify intermediate points with ContentElement in window.")]
        [TestCase("1", @"CoreInput\Mouse", "NavigationWindow", TestCaseSecurityLevel.FullTrust, @"Verify intermediate points with ContentElement in NavigationWindow.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new GetIntermediatePointsContentElementApp(), "Run");

        }


        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing tree....");
            InstrContentPanelHost host = new InstrContentPanelHost();
            FrameworkContentElement fce = new InstrFrameworkContentPanel("rootLeaf", "Sample", host);
            host.AddChild(fce);
            InputElement = fce;
            _rootElement = host;

            // Put the test element on the screen
            DisplayMe(_rootElement, 1, 1, 100, 100);

            return null;
        }
    }
}

