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
    /// Verify properties of commands without execute handlers in navigation window are set correctly on initialization.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class CommandLibraryNavigationWindowPropertiesNoExecuteHandlerApp : CommandLibraryPropertiesNoExecuteHandlerApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"Commanding\Library", "NavigationWindow", TestCaseSecurityLevel.FullTrust, @"Compile and Verify properties of commands without execute handlers in navigation window are set correctly on initialization.")]
        public new static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.Commanding",
                "CommandLibraryNavigationWindowPropertiesNoExecuteHandlerApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test. 
        /// </summary>
        [TestCase("1", @"Commanding\Library", "NavigationWindow", TestCaseSecurityLevel.FullTrust, @"Verify properties of commands without execute handlers in navigation window are set correctly on initialization.")]
        public new static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new CommandLibraryNavigationWindowPropertiesNoExecuteHandlerApp(), "Run");
        }

        /// <summary>
        /// Are the command properties correct for when current element has focus?
        /// </summary>
        /// <param name="cmd">RoutedCommand</param>
        /// <returns>true if correct, false otherwise.</returns>
        protected override bool IsValidPropertiesFocused(RoutedCommand cmd)
        {
            if ((cmd != NavigationCommands.Refresh) && (cmd != NavigationCommands.BrowseStop))
            {
                return (IsValidLibraryCommand(cmd) && !IsLibraryCommandEnabled(cmd));
            }

            return true;
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
