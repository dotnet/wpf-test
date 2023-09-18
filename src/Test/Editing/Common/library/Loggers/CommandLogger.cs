// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides logging services to describe the commanding system.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 6 $ $Source: //depot/private/WCP_dev_platform/Windowstest/client/wcptests/uis/Text/BVT/Editing/TextEditorTests.cs $")]

namespace Test.Uis.Loggers
{
    #region Namespaces.
    
    using System;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;
    
    #endregion Namespaces.

    /// <summary>
    /// Provides methods to log information about the commanding system.
    /// </summary>
    public static class CommandLogger
    {
        /// <summary>
        /// Describes a System.Windows.Input.RoutedCommand instance, including
        /// the default command links associated with it.
        /// </summary>
        /// <param name="command">Instance to describe.</param>
        /// <returns>A text description of the command.</returns>
        public static string DescribeCommand(RoutedCommand command)
        {
            string links;   // Links to be described.

            if (command == null)
                throw new ArgumentNullException("command");

            // Describe all the default links.
            links = "";

            // obsolete
            // foreach (CommandBinding link in command.Defaults)
            // {
            //     links += "\n" + DescribeCommandBinding(link);
            // }

            return String.Format("Command {0}{1}", command.Name, links);
        }

        /// <summary>
        /// Describes a System.Windows.Input.RoutedCommandBinding instance.
        /// </summary>
        /// <param name="link">Instance to describe.</param>
        /// <returns>A text description of the command link.</returns>
        /// <remarks>
        /// Known CommandBinding subtypes are logged appropriately.
        /// </remarks>
        public static string DescribeCommandBinding(CommandBinding link)
        {
            string result;          // Result of description.
            
            if (link == null)
                throw new ArgumentNullException("link");

            result = String.Format(
                "Command Binding for command " +
                // obsolete
                // "{0} (enabled={1},checked={2},key={3},mouse{4})",
                // link.Command.Name,
                // link.Command.Enabled,
                // link.Checked, 
                // DescribeKeyBinding(link.KeyBinding),
                // DescribeMouseBinding(link.MouseBinding)
                " (enabled={0})",
                link.Command.CanExecute(null)

                );

            // obsolete
            // UICommandBinding uiLink;   // Link to UI element.
            // uiLink = link as UICommandBinding;
            // if (uiLink != null)
            // {
            //     result += String.Format(
            //         "\n(menu={0},statusbar={1},tooltip={2})",
            //         uiLink.MenuText, uiLink.StatusBarText, uiLink.TooltipText);
            // }
            return result;
        }

        /// <summary>
        /// Describes a System.Windows.Input.KeyBinding instance.
        /// </summary>
        /// <param name="binding">Instance to describe.</param>
        /// <returns>A text description of the key binding.</returns>
        public static string DescribeKeyBinding(KeyBinding binding)
        {
            if (binding == null)
                return "[null]";

            return String.Format("KeyBinding: {0}-{1}", 
                binding.Modifiers, binding.Key);
        }

        /// <summary>
        /// Describes a System.Windows.Input.MouseBinding instance.
        /// </summary>
        /// <param name="binding">Instance to describe.</param>
        /// <returns>A text description of the mouse binding.</returns>
        public static string DescribeMouseBinding(MouseBinding binding)
        {
            if (binding == null)
                return "[null]";

            return String.Format("MouseBinding: {0}", binding.MouseAction);
        }

        /// <summary>
        /// Describes the commands for a System.Windows.UIElement instance.
        /// </summary>
        /// <param name="element">Instance to describe.</param>
        /// <returns>
        /// A text description of the commands associated with the specified element.
        /// </returns>
        public static string DescribeUIElementCommands(UIElement element)
        {
            string commands;    // List of command descriptions.
            
            if (element == null)
                throw new ArgumentNullException("element");

            // List commands.
            commands = "";
            foreach (CommandBinding link in element.CommandBindings)
            {
                commands += "\n" + DescribeCommandBinding(link);
            }
            commands = Test.Uis.Utils.TextUtils.IndentLines(commands, "  ");

            return String.Format("Commands for {0} ({1}):{2}",
                element.ToString(), element.CommandBindings.Count, commands);
        }
    }
}
