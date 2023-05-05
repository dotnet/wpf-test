// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Framework
{
    /// <summary>
    /// Helper methods for the Xaml Test Framework
    /// </summary>
    public static class FrameworkHelper
    {
        #region Static Methods

        /// <summary>
        /// Loads any assemblies specified in the SupportingAssemblies driver param.
        /// These assemblies must be in the current directory.
        /// </summary>
        /// <param name="assemblies">A comma separated list of assembly names</param>
        public static void LoadSupportingAssemblies(string assemblies)
        {
            if (String.IsNullOrEmpty(assemblies))
            {
                return;
            }

            string[] assemblyArray = assemblies.Split(new char[] { ',' });
            foreach (string asm in assemblyArray)
            {
                GlobalLog.LogStatus("Loading assembly " + asm);
                Assembly.LoadFrom(asm + ".dll");
            }
        }
         
        /// <summary>
        /// Reflects into a test assembly to find the desired verify() method
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="verifierName">Type name containing Verify() method</param>
        /// <returns>MethodInfo value</returns>
        public static MethodInfo LoadVerifier(string assemblyName, string verifierName)
        {
            if (!assemblyName.EndsWith(".dll"))
            {
                assemblyName += ".dll";
            }

            Assembly testAssembly = Assembly.LoadFrom(assemblyName);
            if (testAssembly == null)
            {
                throw new Exception("testAssembly is null");
            }

            Type testType = testAssembly.GetType(verifierName, true);
            if (testType == null)
            {
                throw new Exception("testType is null");
            }

            MethodInfo verifier = testType.GetMethod("Verify", BindingFlags.Static | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (verifier == null)
            {
                throw new Exception(String.Format("Could not find method: public static Verify(UIElement) on type: {0} in assembly: {1}", verifierName, testAssembly.ToString()));
            }

            return verifier;
        }

        /// <summary>
        /// Reflects into a test assembly to find the desired test method
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="className">Name of the class.</param>
        /// <param name="methodName">Name of the method.</param>
        public static void InvokeTestMethod(string assemblyName, string className, string methodName)
        {
            if (!assemblyName.EndsWith(".dll"))
            {
                assemblyName += ".dll";
            }

            Assembly testAssembly = Assembly.LoadFrom(assemblyName);
            if (testAssembly == null)
            {
                throw new Exception("testAssembly is null");
            }

            Type testType = testAssembly.GetType(className, true);
            if (testType == null)
            {
                throw new Exception("testType is null");
            }

            MethodInfo methodInfo = testType.GetMethod(methodName, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static);
            if (methodInfo == null)
            {
                throw new Exception(String.Format("Could not find method: {0} on type: {1} in assembly: {2}", methodName, className, testAssembly.ToString()));
            }

            if (methodInfo.IsStatic)
            {
                methodInfo.Invoke(null, null);
            }
            else
            {
                ConstructorInfo constructor = testType.GetConstructor(new Type[0]);
                if (constructor == null)
                {
                    throw new Exception("No parameterless constructor found..");
                }

                object testObj = constructor.Invoke(new object[0]);
                if (testObj == null)
                {
                    throw new Exception("Constructor did not return an object");
                }

                methodInfo.Invoke(testObj, new object[0]);
            }
        }

        #endregion
    }
}
