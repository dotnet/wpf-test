// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Basic
{
    /// <summary>
    /// Verification class for Xaml File
    /// \src\wpf\Test\XAML\FeatureTests\Data\Parser\Basic\DefArrayTest.xaml
    /// </summary>
    public static class DefArrayTest_Verify
    {
        /// <summary>
        /// This method checks that when a button's content is set to an array of type object, 
        /// all of the objects in the array are present
        /// </summary>
        /// <param name="rootElement"></param>
        /// <returns></returns>
        public static bool Verify(UIElement rootElement)
        {
            GlobalLog.LogStatus("Inside Verifier");
            bool   result  = true;
            Button button0 = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Button0");

            // Verify the content property of Button
            Object[] contents = (Object[]) button0.GetValue(ContentControl.ContentProperty);
            if ("Hello" != (contents[0] as TextBlock).Text)
            {
                result = false;
                GlobalLog.LogEvidence("contents[0] did not match, expected: \"Hello\" found: \"{0}\"", (contents[0] as TextBlock).Text);
            }
            if ("World" != (contents[1] as TextBlock).Text)
            {
                result = false;
                GlobalLog.LogEvidence("contents[1] did not match, expected: \"World\" found: \"{0}\"", (contents[1] as TextBlock).Text);
            }
            if ("foo" != contents[2] as String)
            {
                result = false;
                GlobalLog.LogEvidence("contents[2] did not match, expected: \"foo\" found: \"{0}\"", contents[2] as String);
            }
            return result;
        }
    }
}
