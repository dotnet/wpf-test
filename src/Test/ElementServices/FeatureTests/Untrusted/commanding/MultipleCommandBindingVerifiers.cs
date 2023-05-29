// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;

using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Threading;
using System.Windows.Threading;

using Avalon.Test.CoreUI.Common;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.Framework.Commanding
{
    /// <summary>
    /// Verify parsing and serializing multiple command bindings and input bindings.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [TestCaseTitle(@"Verify parsing and serializing multiple command bindings and input bindings.")]
    [TestCasePriority("1")]
    public class MultipleCommandBindingVerifiers
    {
        /// <summary>
        /// VerifyKeyBindingValue
        /// </summary>
        /// <param name="uie">Root of tree in XAML.</param>
        public static void VerifyCommandBindingValue(UIElement uie)
        {
            string[] ids = new string[] {
                "Open", "New", "MoveRightByWord", "Print", "SaveAs", "Cut",
                "AlignCenter", "AlignLeft", "AlignRight", "Backspace", "ToggleBold","AlignJustify",
            };

            TypeConverter cmdtc = TypeDescriptor.GetConverter(typeof(RoutedCommand));
            TypeConverter kgtc = TypeDescriptor.GetConverter(typeof(KeyGesture));
            TypeConverter mgtc = TypeDescriptor.GetConverter(typeof(MouseGesture));

            foreach (string elId in ids)
            {
                FrameworkElement el = LogicalTreeHelper.FindLogicalNode(uie, elId) as FrameworkElement;
                if (el != null)
                {
                    CoreLogger.LogStatus("el='" + el.ToString() + "', Name='" + elId + "'");

                    RoutedCommand cmd = cmdtc.ConvertFrom(elId) as RoutedCommand;
                    Assert(cmd != null, "Expected command not found!");

                    Assert(el.CommandBindings.Count > 0, "Expected command bindings not found!");

                    foreach (CommandBinding link in el.CommandBindings)
                    {
                        // Get the key binding
                        ICommand cmd2 = link.Command;
                        string sFromCommand = cmdtc.ConvertTo(cmd2, typeof(string)) as string;

                        // Compare to see if keybinding exists.
                        CoreLogger.LogStatus("converted cmd binding? '" + sFromCommand + "'");
                        Assert((sFromCommand == elId), "no real command (CommandBinding)!");
                    }

                    Assert(el.InputBindings.Count > 0, "Expected input bindings not found!");

                    foreach (InputBinding link in el.InputBindings)
                    {
                        // Get the key binding
                        ICommand cmd2 = link.Command;
                        InputGesture gesture = link.Gesture;

                        string sFromCommand = cmdtc.ConvertTo(cmd2, typeof(string)) as string;
                        Assert((sFromCommand != null) && (sFromCommand != ""), "no real command (InputBinding)!");
                        CoreLogger.LogStatus("converted input binding='" + sFromCommand + "'");

                        string sFromGesture = null;
                        if (gesture is KeyGesture)
                        {
                            sFromGesture = kgtc.ConvertTo(gesture, typeof(string)) as string;
                        }
                        else if (gesture is MouseGesture)
                        {
                            sFromGesture = mgtc.ConvertTo(gesture, typeof(string)) as string;
                        }
                        Assert((sFromGesture != null) && (sFromGesture != ""), "no real gesture (InputBinding)!");
                        CoreLogger.LogStatus("converted input gesture='" + sFromGesture + "'");
                    }
                }
            }
        }


        private static void Assert(bool condition, string exceptionMsg)
        {
            // Log intermediate result
            if (!condition)
            {
                // Intermediate result = FAIL
                string exceptionString = exceptionMsg;

                throw new Microsoft.Test.TestValidationException(exceptionMsg);
            }
        }

    }
}

