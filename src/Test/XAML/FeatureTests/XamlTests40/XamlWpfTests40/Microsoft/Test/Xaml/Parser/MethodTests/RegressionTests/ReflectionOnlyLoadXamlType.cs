// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Xaml;
using System.Xaml.Schema;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.MethodTests.RegressionTests
{
    /// <summary>
    /// Regression test
    /// </summary>
    public static class ReflectionOnlyLoadXamlType
    {
        /// <summary>
        /// Scenario:  Load an assembly using reflection only load
        /// and try to resolve the type through schema context. The issue
        /// was that this used to throw instead of not resolving and returning
        /// a null XamlType
        /// </summary>
        public static void RunTest()
        {
            bool testPassed = false;
            XamlSchemaContext context = new XamlSchemaContext();
            Assembly assemblyWithXmlns = Assembly.ReflectionOnlyLoad("AssemblyWithXmlns");

            XamlType xamlType = context.GetXamlType(new XamlTypeName("http:\\foobar", "SomeType"));
            if (xamlType == null)
            {
                testPassed = true;
            }

            if (testPassed)
            {
                GlobalLog.LogEvidence("Obtained null for XamlType as expected");
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("Obtained non null XamlType. Expected null");
                TestLog.Current.Result = TestResult.Fail;
            }
        }
    }
}
