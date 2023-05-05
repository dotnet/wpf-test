// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Test.Logging;
using System.Windows.Media;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Template
{
    /// <summary/>
    public static class Template_PropertyTagSyntax_Verify
    {
        /// <summary>
        /// Verifies PropertyTag syntax works in Template.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool    result  = true;
            Control element = (Control)(((FrameworkElement) rootElement).FindName("element1"));

            //
            // Verify element property value set in Template.
            //
            GlobalLog.LogStatus("Verifying Element's property...");

            if (element.Background == Brushes.Red)
            {
                GlobalLog.LogEvidence("Element's Background != Brushes.Red.");
                result = false;
            }
            return result;
        }
    }
}
