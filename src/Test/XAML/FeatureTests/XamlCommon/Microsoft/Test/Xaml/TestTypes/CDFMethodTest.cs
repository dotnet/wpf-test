// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Test.CDFInfrastructure;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.TestTypes
{
    /// <summary>
    /// CDF MethodTest
    /// </summary>
    public class CDFMethodTest : XamlTestType
    {
        /// <summary>
        /// List of CDFTestInfo
        /// </summary>
        private List<CDFTestInfo> _testCases;

        /// <summary>
        /// Runs a test host test (either method or generator)
        /// This is used to run CDF Xaml tests that use testhost
        /// More details at http://etcm
        /// </summary>
        public override void Run()
        {
            string assemblyName = DriverState.DriverParameters["TestAssembly"];
            string className = DriverState.DriverParameters["TestClass"];
            string methodName = DriverState.DriverParameters["TestMethod"];
            string isGenerator = DriverState.DriverParameters["IsGenenerator"];
            bool isGeneratorTest = !String.IsNullOrEmpty(isGenerator);

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

            MethodInfo methodInfo;
            try
            {
                methodInfo = Assembly.Load(assemblyName).GetType(className).GetMethod(methodName);
            }
            catch (Exception exception)
            {
                Trace.TraceError(String.Format("Exception loading testcases from assembly {0}:", exception.ToString()));
                throw;
            }

            if (isGeneratorTest)
            {
                RunGeneratorTest(methodInfo);
            }
            else
            {
                RunMethodTest(methodInfo, new object[] { });
            }
        }

        /// <summary>
        /// Method used by testhost generator test case generation 
        /// </summary>
        /// <param name="testCaseAttribute">TestCase Attribute</param>
        /// <param name="methodInfo">MethodInfo value</param>
        /// <param name="arguments">object array</param>
        [CLSCompliant(false)]
        public void AddTestCase(TestCaseAttribute testCaseAttribute, MethodInfo methodInfo, params object[] arguments)
        {
            CDFTestInfo testInfo = new CDFTestInfo();
            testInfo.Name = GetTestName(methodInfo.Name, arguments);
            
            Assembly assembly = Assembly.Load(methodInfo.DeclaringType.Assembly.FullName);
            Type type = assembly.GetType(methodInfo.DeclaringType.FullName);

            testInfo.MethodInfo = type.GetMethod(methodInfo.Name);
            testInfo.Parameters = arguments;
            if (testCaseAttribute.TestType == Microsoft.Test.CDFInfrastructure.TestType.BlockedProductIssue)
            {
                string description = testCaseAttribute.Description;

                int index = testCaseAttribute.Description.IndexOf("Bug=") + "Bug=".Length;
                string bugNumber = testCaseAttribute.Description.Substring(index, description.Length - index);
                testInfo.BugNumber = int.Parse(bugNumber);
            }

            _testCases.Add(testInfo);
        }

        /// <summary>
        /// Run a method tests givent the CDFTestInfo object
        /// </summary>
        /// <param name="testInfo">CDFTestInfo value</param>
        private void RunMethodTest(CDFTestInfo testInfo)
        {
            TestLog log = new TestLog(testInfo.Name);
            try
            {
                GlobalLog.LogStatus("Starting Test");
                InvokeMethod(testInfo.MethodInfo, testInfo.Parameters);
                SetResult(testInfo, true);
            }
            catch (TargetInvocationException targetInvocationException)
            {
                SetResult(testInfo, false);
                GlobalLog.LogEvidence("Test failed due to exception: " + targetInvocationException.InnerException.Message);
                GlobalLog.LogEvidence(targetInvocationException.InnerException.ToString());
            }
            catch (Exception e)
            {
                SetResult(testInfo, false);
                GlobalLog.LogEvidence("Test failed due to exception: " + e.Message);
                GlobalLog.LogEvidence(e.ToString());
            }

            log.Close();
        }

        /// <summary>
        /// Sets the result.
        /// </summary>
        /// <param name="testInfo">The test info.</param>
        /// <param name="passed">if set to <c>true</c> [passed].</param>
        private void SetResult(CDFTestInfo testInfo, bool passed)
        {
            bool bugExists = testInfo.BugNumber != 0;
            if (passed)
            {
                if (!bugExists)
                {
                    // Test passed //
                    TestLog.Current.Result = TestResult.Pass;
                }
                else
                {
                    GlobalLog.LogEvidence("Testcase passed but has a bug associated with it " + testInfo.BugNumber);
                    TestLog.Current.Result = TestResult.Fail;
                }
            }
            else
            {
                if (!bugExists)
                {
                    // Test failure //
                    TestLog.Current.Result = TestResult.Fail;
                }
                else
                {
                    GlobalLog.LogEvidence("Testcase failed but has a bug associated with it " + testInfo.BugNumber);
                    TestLog.Current.Result = TestResult.Ignore;
                }
            }
        }

        /// <summary>
        /// Runs a Method as a test given the MethodInfo and arguments
        /// </summary>
        /// <param name="methodInfo">MethodInfo of method to run</param>
        /// <param name="arguments">Parametres for the method to run</param>
        private void RunMethodTest(MethodInfo methodInfo, object[] arguments)
        {
            CDFTestInfo testInfo = new CDFTestInfo()
                                       {
                                           Name = DriverState.TestName,
                                           MethodInfo = methodInfo,
                                           Parameters = arguments,
                                       };
            RunMethodTest(testInfo);
        }

        /// <summary>
        /// Given the generator method, this method generates the tests
        /// and runs each one as a test creating its own log
        /// </summary>
        /// <param name="methodInfo">MethodInfo value </param>
        private void RunGeneratorTest(MethodInfo methodInfo)
        {
            List<CDFTestInfo> generatedTests = GenerateTests(methodInfo);
            foreach (CDFTestInfo testCase in generatedTests)
            {
                RunMethodTest(testCase);
            }
        }

        /// <summary>
        /// Generate a list of CDFTestInfo objects tests given the generator method
        /// </summary>
        /// <param name="methodInfo">The method info.</param>
        /// <returns>List CDFTestInfo</returns>
        private List<CDFTestInfo> GenerateTests(MethodInfo methodInfo)
        {
            this._testCases = new List<CDFTestInfo>();
            InvokeMethod(methodInfo, new object[] { new AddTestCaseEventHandler(this.AddTestCase) });

            return _testCases;
        }

        /// <summary>
        /// Create an instance of a type in a test assembly  
        /// </summary>
        /// <param name="assembly"> Name of the assembly </param>
        /// <param name="typeName"> Name of the type in the assembly </param>
        /// <returns>object value</returns>
        private object CreateInstance(Assembly assembly, string typeName)
        {
            object instance = null;
            Type type = assembly.GetType(typeName);
            if ((type != null) && (type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new Type[0], null) != null))
            {
                instance = assembly.CreateInstance(typeName);
            }

            return instance;
        }

        /// <summary>
        ///  Invoke a static or instance method
        /// </summary>
        /// <param name="methodInfo">MethodInfo value</param>
        /// <param name="arguments">object array</param>
        private void InvokeMethod(MethodInfo methodInfo, object[] arguments)
        {
            object instance = null;
            if (!methodInfo.IsStatic)
            {
                instance = CreateInstance(methodInfo.DeclaringType.Assembly, methodInfo.DeclaringType.FullName);
            }

            methodInfo.Invoke(instance, arguments);
        }

        /// <summary>
        /// Get a formatted name for a generated test
        /// </summary>
        /// <param name="methodName">string value </param>
        /// <param name="arguments">object array of arguments</param>
        /// <returns>test name value</returns>
        private string GetTestName(string methodName, object[] arguments)
        {
            string name = methodName;
            if (arguments.Length != 0)
            {
                for (int argIndex = 0; argIndex < arguments.Length; argIndex++)
                {
                    name += "_" + arguments[argIndex].ToString();
                }
            }

            return name;
        }

        /// <summary>
        /// Class used to hold information about 
        /// a CDF Test
        /// </summary>
        private class CDFTestInfo
        {
            /// <summary>
            /// Gets or sets the method info.
            /// </summary>
            /// <value>The method info.</value>
            public MethodInfo MethodInfo { get; set; }

            /// <summary>
            /// Gets or sets the parameters.
            /// </summary>
            /// <value>The parameters.</value>
            public object[] Parameters { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name value.</value>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the bug number.
            /// </summary>
            /// <value>The bug number.</value>
            public int BugNumber { get; set; }
        }
    }
}
