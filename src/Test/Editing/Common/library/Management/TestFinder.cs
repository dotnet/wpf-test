// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides a TestFinder type to find named test cases.

using System;
using System.Collections;
using System.Reflection;
using System.Security;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Test.Uis.Loggers;
using Test.Uis.Utils;

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Management/TestFinder.cs $")]

namespace Test.Uis.Management
{
    /// <summary>
    /// Provides utility methods to find test cases and entry points.
    /// </summary>
    /// <example>The followin sample shows how to use this type.<code>
    /// public static void Main(string args[]) {
    ///   // The type of the test is the first argument. Run it.
    ///   string typeName = args[0];
    ///   TestFinder.RunNamedTestCase(typeName);
    /// }</code></example>
    public class TestFinder
    {
        #region Internal methods.

        /// <summary>
        /// Checks whether an assembly is known not to have test cases.
        /// </summary>
        /// <param name="assembly">Assembly to check.</param>
        /// <returns>
        /// true if the assembly is known not to have test cases, false otherwise.
        /// </returns>
        internal static bool IsAssemblyKnownNotTest(Assembly assembly)
        {
            string bareName;    // Bare name of assembly.
            int commaIndex;     // Comma separator in assembly name.

            if (assembly == null)
                throw new ArgumentNullException("assembly");

            bareName = assembly.FullName;
            if (bareName != null)
            {
                commaIndex = bareName.IndexOf(",");
                if (commaIndex != -1)
                {
                    bareName = bareName.Substring(0, commaIndex);
                    if (bareName == "mscorlib" ||
                        bareName == "EditingTestLib" ||
                        bareName == "PresentationFramework" ||
                        bareName == "PresentationCore" ||
                        bareName == "System.Drawing" ||
                        bareName == "System.Xml" ||
                        bareName == "System.Web" ||
                        bareName == "TestRuntime" ||
                        bareName == "System.Runtime.Remoting" ||
                        bareName == "System.Activities"
                        )
                        return true;
                }
            }
            return false;
        }

        #endregion Internal methods.

        #region Private methods.

        /// <summary>
        /// Returns the first MethodBase of the specified type that has
        /// a TestEntryPointAttribute assigned.
        /// </summary>
        /// <param name='testCaseType'>Type to find entry point for.</param>
        /// <returns>The entry point method; null if none is found.</returns>
        private static MethodBase GetEntryPoint(Type testCaseType)
        {
            if (testCaseType == null)
            {
                throw new ArgumentNullException("testCaseType");
            }

            new System.Security.Permissions.ReflectionPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();

            const bool inherit = true;
            const BindingFlags flags =
                BindingFlags.Instance | BindingFlags.Static |
                BindingFlags.Public | BindingFlags.NonPublic;

            Type entryPointType = typeof(TestEntryPointAttribute);

            MethodInfo[] methods = testCaseType.GetMethods(flags);
            foreach (MethodInfo method in methods)
            {
                object[] attributes = method.GetCustomAttributes(
                    entryPointType, inherit);
                if (attributes.Length > 0)
                {
                    return method;
                }
            }

            // Failed to find an attribute.
            return null;
        }

        /// <summary>
        /// Creates a test case instance.
        /// </summary>
        /// <param name='typeName'>Name of type to crate, without namespace.</param>
        /// <returns>Created object, null if not found.</returns>
        /// <remarks>
        /// Note that the type must be in an assembly already loaded into the
        /// AppDomain.
        /// </remarks>
        private static object GetNamedTestCase(string typeName)
        {
            new System.Security.Permissions.ReflectionPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                // Skip some well-known assemblies that do not have test cases.
                if (IsAssemblyKnownNotTest(assembly))
                    continue;

                Type[] testTypes = SafeGetTypes(assembly);
                foreach (Type type in testTypes)
                {
                    if (type != null && type.Name == typeName)
                    {
                        return Activator.CreateInstance(type, (new object[] { }), null);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieves types, guaranteeing that no exceptions will be thown.
        /// </summary>
        /// <param name="assembly">Assembly to get types from.</param>
        /// <returns>
        /// The types in the specifies assembly, or a zero-length if an
        /// exception was thrown.
        /// </returns>
        private static Type[] SafeGetTypes(Assembly assembly)
        {
            Type[] testTypes;   // Returned test types.

            try
            {
                testTypes = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException re)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                sb.Append("ReflectionTypeLoadException thrown while getting types for ");
                sb.Append(assembly.FullName);
                sb.Append(System.Environment.NewLine);
                sb.Append(re.ToString());
                sb.Append(System.Environment.NewLine);
                sb.Append("Loader exceptions: ");
                sb.Append(re.LoaderExceptions.Length);
                sb.Append(System.Environment.NewLine);
                foreach (Exception exception in re.LoaderExceptions)
                {
                    sb.Append(exception.ToString());
                    sb.Append(System.Environment.NewLine);
                }

                Logger.Current.Log(sb.ToString());

                testTypes = new Type[0];
            }
            catch (Exception exception)
            {
                Logger.Current.Log(
                    "Exception thrown while getting types for {0}{1}{2}",
                    assembly.FullName, Environment.NewLine,
                    exception.ToString());
                testTypes = new Type[0];
            }
            return testTypes;
        }

        /// <summary>Logs test data based on attributes.</summary>
        /// <param name='testType'>Type of test case to log.</param>
        private static void LogTestData(Type testType)
        {
            string msg; // Message to log out with test case information.

            if (testType == null)
            {
                throw new ArgumentNullException("testType");
            }
            msg = "\r\n\r\n ===============================================================\r\n";
            msg += "\r\n ***** Test type: " + testType.ToString() + ". ***** \r\n";

            if (classTestAttribute == null)
            {
                try
                {
                    string[] commandArr = ConfigurationSettings.Current.CommandLineArguments;
                    string commandString = "";
                    foreach (string str in commandArr)
                    {
                        commandString += str + " ";
                    }
                    commandString = commandString.Trim();
                    TestAttribute[] classTestAttributeArray = ((TestAttribute[])(testType.GetCustomAttributes(typeof(TestAttribute), false)));
                    foreach (TestAttribute t in classTestAttributeArray)
                    {
                        if (t.Name == DriverState.TestName)
                        {
                            classTestAttribute = t;
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    //throw new Exception("Check if TestAttribute is present on the test case");
                }
            }

            //int timeout =classTestAttributes["TimeOut"];
            TestTitleAttribute title = TestTitleAttribute.FromType(testType);
            if (title != null)
                msg += "\r\n ***** Test Title: " + title.Title + ". ***** \r\n";

            TestOwnerAttribute owner = TestOwnerAttribute.FromType(testType);
            if (owner != null)
                msg += " ***** Owner: " + owner.OwnerAlias + ". ***** \r\n";

            TestBugsAttribute bugs = TestBugsAttribute.FromType(testType);
            if (bugs != null)
                msg += " ***** Bugs: " + bugs.Bugs + ". ***** \r\n";

            TestTacticsAttribute tacticsID = TestTacticsAttribute.FromType(
                testType);
            if (tacticsID != null)
                msg += " ***** Tactics ID: " + tacticsID.ToString() + ". ***** \r\n";

            TestLastUpdatedOnAttribute updateDate = TestLastUpdatedOnAttribute.FromType(
                testType);
            if (updateDate != null)
                msg += " ***** Test Last Updated On: " + updateDate.UpdateDate + ". ***** \r\n";

            if (WindowlessTestAttribute.IsTestWindowless(testType))
            {
                msg += " Windowless test case.";
            }
            msg += "\r\n ===============================================================\r\n";

            TestArgumentAttribute[] arguments = TestArgumentAttribute.FromType(
                testType);
            for (int i = 0; i < arguments.Length; i++)
            {
                msg += Environment.NewLine + arguments[i].Name + " - " +
                    arguments[i].Description;
            }

            Logger.Current.Log(msg);
        }

        #endregion Private methods.

        #region Public methods.

        /// <summary>
        /// Creates a list of test cases available in the loaded assemblies.
        /// </summary>
        /// <param name="includeNamespace">
        /// Whether to include the namespace in the test case names.
        /// </param>
        /// <returns>An array of test case type names or an error description.</returns>
        public static string[] ListAvailableTestCases(bool includeNamespace)
        {
            try
            {
                new System.Security.Permissions.ReflectionPermission(
                    System.Security.Permissions.PermissionState.Unrestricted)
                    .Assert();
                ArrayList results = new ArrayList(128);
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly assembly in assemblies)
                {
                    if (IsAssemblyKnownNotTest(assembly))
                        continue;

                    Type[] testTypes = SafeGetTypes(assembly);
                    foreach (Type type in testTypes)
                    {
                        if (type != null && !type.IsAbstract &&
                            GetEntryPoint(type) != null)
                        {
                            string result;
                            if (includeNamespace)
                                result = type.FullName;
                            else
                                result = type.Name;
                            results.Add(result);
                        }
                    }
                }
                return (string[])results.ToArray(typeof(string));
            }
            catch (Exception exception)
            {
                string[] result = new string[1];
                result[0] = exception.ToString();
                return result;
            }
        }

        /// <summary>
        /// Creates a test case instance and executes its entry point method.
        /// </summary>
        /// <remarks>
        /// Note that the type must be in an assembly already loaded into the
        /// AppDomain.
        /// </remarks>
        public static void RunNamedTestCase(string typeName)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException("typeName");
            }
            if (typeName.Length == 0)
            {
                throw new ArgumentException("Type name cannot be empty", "typeName");
            }



            new System.Security.Permissions.ReflectionPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            typeName = Test.Uis.Utils.ReflectionUtils.GetNameFromFullTypeName(typeName);
            object testCase = GetNamedTestCase(typeName);
            if (testCase == null)
            {
                throw new ArgumentException(
                    "There is no loaded type with the name [" + typeName + "]",
                    "typeName");
            }
            else
            {
                Type type = testCase.GetType();
                LogTestData(type);
                MethodBase method = GetEntryPoint(type);
                if (method == null)
                {
                    throw new InvalidOperationException(
                        "There is no entry point in type [" + type + "]");
                }

                // Verify that the entry point takes no arguments.
                ParameterInfo[] param = method.GetParameters();
                if (param.Length > 0)
                {
                    throw new InvalidOperationException(
                        "Entry point method " + method + " in type " + type +
                        " takes parameters. A method with no parameters" +
                        " is expected.");
                }

                CodeAccessPermission.RevertAll();

                ConfigurationSettings.Current.SetInstalledKeyboardLayouts(KeyboardLayoutHelper.GetCurrentKeyboardLayouts());
                
                KeyboardInput.ResetCapsLock();                

                method.Invoke(testCase, null);
            }
        }

        #endregion Public methods.

        /// <summary>
        /// classTestAttribute
        /// </summary>
        public static TestAttribute classTestAttribute = null;

    }
}
