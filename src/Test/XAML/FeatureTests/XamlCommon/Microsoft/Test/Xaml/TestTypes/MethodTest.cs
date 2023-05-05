// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Framework;

namespace Microsoft.Test.Xaml.TestTypes
{
    /// <summary>
    /// Method Test
    /// </summary>
    public class MethodTest : XamlTestType
    {
        /// <summary>
        /// Runs a test contained within a method
        /// Used to run legacy tests that formerly used the TestAttribute adaptor
        /// </summary>
        public override void Run()
        {
            string assemblyName = DriverState.DriverParameters["TestAssembly"];
            string className = DriverState.DriverParameters["TestClass"];
            string methodName = DriverState.DriverParameters["TestMethod"];
            if (String.IsNullOrEmpty(assemblyName))
            {
                throw new Exception("assemblyName cannot be null");
            }

            if (String.IsNullOrEmpty(className))
            {
                throw new Exception("className cannot be null");
            }

            if (String.IsNullOrEmpty(methodName))
            {
                throw new Exception("methodName cannot be null");
            }

            // Load any supporting assemblies
            if (!String.IsNullOrEmpty(DriverState.DriverParameters["SupportingAssemblies"]))
            {
                string assemblies = DriverState.DriverParameters["SupportingAssemblies"];
                GlobalLog.LogStatus("Loading Assemblies: " + assemblies);
                FrameworkHelper.LoadSupportingAssemblies(assemblies);
            }

            TestLog log = new TestLog(DriverState.TestName);
            try
            {
                GlobalLog.LogStatus("Starting Test");
                FrameworkHelper.InvokeTestMethod(assemblyName, className, methodName);
                TestLog.Current.Result = TestResult.Pass;
            }
            catch (Exception e)
            {
                TestLog.Current.Result = TestResult.Fail;
                GlobalLog.LogEvidence("Test failed due to exception: " + e.Message);
                GlobalLog.LogEvidence(e.ToString());
            }

            log.Close();
        }
    }
}
