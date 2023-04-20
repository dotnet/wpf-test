// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Windows.Controls;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.Verifiers.BugRepros
{
    /// <summary>
    /// Verifier for regression test 
    /// </summary>
    public static class RegressionIssue9Repro_Verify
    {
        public static bool Verify(object rootElement)
        {
            bool result = true;
            StackPanel stackPanel = (StackPanel)rootElement;
            Label label = stackPanel.Children[1] as Label;

            if (label == null)
            {
                GlobalLog.LogEvidence("StackPanel's second child is not a label");
                result = false;
            }

            if (label.Content.ToString() != "Content")
            {
                GlobalLog.LogEvidence("Data Binding failed - label's content is not 'Content'");
                result = false;
            }

            return result;
        }
    }
}
