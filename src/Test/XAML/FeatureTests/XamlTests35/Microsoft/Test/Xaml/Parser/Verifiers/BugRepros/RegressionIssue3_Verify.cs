// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Microsoft.Test.Logging;
using System.Windows.Controls;

namespace Microsoft.Test.Xaml.Parser.Verifiers.BugRepros
{
    /// <summary/>
    public static class RegressionIssue3_Verify
    {
        /// <summary>
        ///  Verifier for RegressionIssue3.xaml
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;
            GlobalLog.LogStatus("Inside RegressionIssue3.Verify1()...");

            DockPanel myPanel = rootElement as DockPanel;

            if (null == myPanel)
            {
                GlobalLog.LogEvidence("Should be DockPanel");
                result = false;
            }
            return result;
        }
    }
}
