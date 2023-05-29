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
    /// Verify parsing and serializing multiple key bindings.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for commanding.
    /// </description>
    /// <





    [TestCaseTitle(@"Verify parsing and serializing multiple key bindings.")]
    [TestCasePriority("1")]
    public class MultipleKeyBindingVerifiers
    {
        /// <summary>
        /// VerifyKeyBindingValue
        /// </summary>
        /// <param name="uie">Root of tree in XAML.</param>
        public static void VerifyKeyBindingValue(UIElement uie)
        {
            string[] ids = new string[] {
                "OpenCommand", "NewCommand", "MoveRightByWordCommand", "PrintCommand", "SaveAsCommand", "CutCommand"
            };


            TypeConverter kbtc = TypeDescriptor.GetConverter(typeof(KeyBinding));
            TypeConverter cmdtc = TypeDescriptor.GetConverter(typeof(RoutedCommand));

            for (int i = 0; i < ids.Length; i++)
            {
                string elId = ids[i];
                
                FrameworkElement el = (FrameworkElement)LogicalTreeHelper.FindLogicalNode(uie, elId);

                Assert(el != null, "Expected element for string '" + elId + "' not found!");
                
                CoreLogger.LogStatus("el='" + el.ToString() + "', Name='" + elId + "'");

                // Construct command link from our element -- based on Name
                RoutedCommand cmd = cmdtc.ConvertFrom(elId) as RoutedCommand;
                Assert(el != null, "Expected command not found!");
                //CommandBinding link = el.CommandBindings.Find(cmd).
                //Assert(link != null, "Expected link to RoutedCommand '"+elId+"' not found!").

                // Get the key binding
                //KeyBinding c = link.KeyBinding.
                //KeyBinding c = link.KeyBinding.
                //string sFromKeyBinding = kbtc.ConvertTo(c, typeof(string)) as string.

                // Compare to see if keybinding exists.
                //Assert(((sFromKeyBinding != "") || (sFromKeyBinding != null)), "no real key binding!").
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

