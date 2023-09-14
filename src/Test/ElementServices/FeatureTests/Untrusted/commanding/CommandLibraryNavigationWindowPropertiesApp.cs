// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Input;
using Avalon.Test.CoreUI.Trusted;

using Avalon.Test.CoreUI.Common;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.Commanding
{
    /// <summary>
    /// Verify properties of commands in navigation window are set correctly on initialization.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <remarks>
    /// CommandLibraryPropertiesApp performs same tests on other surfaces.
    /// </remarks>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class CommandLibraryNavigationWindowPropertiesApp : CommandLibraryPropertiesApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"Commanding\Library", "NavigationWindow", TestCaseSecurityLevel.FullTrust, @"Compile and Verify properties of commands in navigation window are set correctly on initialization in NavigationWindow.")]
        public new static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.Commanding",
                "CommandLibraryNavigationWindowPropertiesApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"Commanding\Library", "NavigationWindow", TestCaseSecurityLevel.FullTrust, @"Verify properties of commands in navigation window are set correctly on initialization in NavigationWindow.")]
        public new static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new CommandLibraryNavigationWindowPropertiesApp(), "Run");
        }

        /// <summary>
        /// Are the command properties correct for when current element does not have focus?
        /// </summary>
        /// <param name="cmd">RoutedCommand</param>
        /// <returns>true if correct, false otherwise.</returns>
        protected override bool IsValidPropertiesNotFocused(RoutedCommand cmd)
        {
            if ((cmd != NavigationCommands.Refresh) && (cmd != NavigationCommands.BrowseStop))
            {
                return (IsValidLibraryCommand(cmd) && !IsLibraryCommandEnabled(cmd));
            }

            return true;
        }
    }
}
