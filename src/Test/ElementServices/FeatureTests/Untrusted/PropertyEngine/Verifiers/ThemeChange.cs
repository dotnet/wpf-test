// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;

using Microsoft.Test.Threading;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.CoreInput.Common;

namespace Avalon.Test.CoreUI.PropertyEngine
{
    /// <summary>
    /// All property engine verifiers in this class
    /// </summary>
    public static partial class Verifiers
    {
        /// <summary>
        /// Used to verify a series of PropertyTrigger-related test cases
        /// </summary>
        /// <param name="root">Root element of the VisualTree</param>
        /// <returns>true when verification completes, false otherwise.</returns>
        public static bool ThemeChangeVerifier(StackPanel root)
        {

            CoreLogger.LogStatus("Verifying PE corruption on theme change.");

            string currTheme = DisplayConfiguration.GetTheme().ToLowerInvariant();
            string newTheme  = "";

            string[] availableThemes = DisplayConfiguration.GetAvailableThemes();

            //Get the first available Theme that is different than the current Theme.
            foreach (string theme in availableThemes)
            {
                if (currTheme != newTheme.ToLowerInvariant())
                {
                    newTheme = theme;
                    break;
                }
            }

            CoreLogger.LogStatus("---currTheme: " + currTheme);
            CoreLogger.LogStatus("---newTheme:  " + newTheme);

            if (String.IsNullOrEmpty(newTheme)) 
            {
                throw new TestValidationException("ERROR: A new Theme is not available.");
            }
            else
            {
                DisplayConfiguration.SetTheme(newTheme);
            }

            // After verification is complete - change theme to what it was originally.
            Microsoft.Test.RenderingVerification.DisplayConfiguration.SetTheme(currTheme);

            return true;
        }
    }
}
