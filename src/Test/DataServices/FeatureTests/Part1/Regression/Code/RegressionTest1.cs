// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using System.Windows.Interop;
using System.Windows.Automation;
using Microsoft.Test.Input;


namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// </summary>
    [Test(0, "Regression", "RegressionTest1")]
    public class RegressionTest1 : RegressionTest
    {
        public override TestResult IdentifyFix()
        {
            // no api changes to detect, we will run this at all times.
            return TestResult.Pass;
        }

        public override TestResult RunTest()
        {
            try
            {
                Regressions.RegressionTest1 repro = new Regressions.RegressionTest1();

                Window window = new Window { Top = 25, Left = 25, Title = "Regression : RegressionTest1" };
                window.Content = repro;
                window.Show();

                DispatcherHelper.DoEvents(DispatcherPriority.SystemIdle);

                for (int i = 0; i <= 25; i++) 
                {
                    repro.ZoomFactor.Value++;
                    DispatcherHelper.DoEvents(DispatcherPriority.SystemIdle);
                }
            }
            catch (ArgumentNullException anex)
            {
                Log.LogEvidence(anex);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
    }
}
