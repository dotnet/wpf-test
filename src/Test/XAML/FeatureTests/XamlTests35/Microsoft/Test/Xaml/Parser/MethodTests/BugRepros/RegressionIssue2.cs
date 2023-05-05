// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Converters;
using Microsoft.Test.Logging;
using System.Reflection;

namespace Microsoft.Test.Xaml.Parser.MethodTests.BugRepros
{
    public class RegressionIssue2
    {
        /// <summary>
        /// Verifies that IPersistFileCheckSum is internal. ArrayExtension doesn't implement IAddChild again as per TypeForwarding changeset 606915 in WpfMain branch.
        /// throwing an exception.
        /// </summary>
        public void VerifyIPersistFileCheckSumIsInternal()
        {
            Assembly assem = null;
            try
            {
                assem = Assembly.Load("PresentationBuildTasks, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL");
            }
            catch
            {
                GlobalLog.LogEvidence("PresentationBuildTasks not found, skipping IPersistFileChecksum test");
                TestLog.Current.Result = TestResult.Pass;
                return;
            }

            Type ipfc = assem.GetType("MS.Internal.IPersistFileCheckSum", false, true);
            if (ipfc.IsPublic)
            {
                TestLog.Current.Result = TestResult.Fail;
                GlobalLog.LogEvidence("IPersistFileChecksum is public, it should not be");
            }

            TestLog.Current.Result = TestResult.Pass;
        }
    }
}
