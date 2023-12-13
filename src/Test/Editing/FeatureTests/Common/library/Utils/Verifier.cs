// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides verification services for test cases.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Utils/Verifier.cs $")]

namespace Test.Uis.Loggers
{
    #region Namespaces.

    using System;
    using System.Diagnostics;
    using System.Reflection;

    #endregion Namespaces.

    /// <summary>A delegate for methods that verify property values.</summary>
    internal delegate void VerifyPropertyHandler(string sourceDescription,
        object target, string propertyName, object expectedValue);

    /// <summary>Provides condition verification services.</summary>
    /// <example>The following sample shows how to use this class.<code>
    /// public void VerifyConditions() {
    ///   Verifier.Verify(myObject.value == expectedValue);
    ///   const string description = "Value matches.";
    ///   const bool logAlways = true;
    ///   Verifier.Verify(myObject.value == expectedValue,
    ///     description, logAlways);
    ///   // This will delay the verification, allowing queue items
    ///   // time to be processed before being verified.
    ///   Verifier.QueueVerification(myObject, "value", expectedValue);
    /// }
    /// </code></example>
    public static class Verifier
    {
        #region Public methods.

        /// <summary>
        /// Compares two values for deep equality (rather than shallow or
        /// reference equality).
        /// </summary>
        /// <param name="a">First object to compare.</param>
        /// <param name="b">Second object to compare.</param>
        /// <returns>true if both objects are equal or both are null; false otherwise.</returns>
        public static bool AreValuesEqual(object a, object b)
        {
            Type type;

            if (a == null && b == null)
            {
                return true;
            }
            if (a == null || b == null)
            {
                return false;
            }

            type = a.GetType();
            if (type != b.GetType())
            {
                return false;
            }

            if (type == typeof(System.Windows.Media.SolidColorBrush))
            {
                return ((System.Windows.Media.SolidColorBrush)a).Color == ((System.Windows.Media.SolidColorBrush)b).Color;
            }
            else
            {
                return a.Equals(b);
            }
        }

        /// <summary>
        /// Verifies whether a condition is true; throws an exception otherwise.
        /// </summary>
        /// <param name="condition">Expression that evaluates to true to
        /// continue execution.</param>
        /// <example>The following sample shows how to use this method.<code>...
        /// private void DoWork(object required) {
        ///   Verifier.Current.Verify(required != null);
        /// }</code></example>
        public static void Verify(bool condition)
        {
            InternalVerify(condition, "", false);
        }

        /// <summary>
        /// Verifies whether a condition is true; throws an exception otherwise.
        /// </summary>
        /// <param name="condition">Expression that evaluates to true to continue execution.</param>
        /// <param name="description">Description of condition evaluated.</param>
        /// <example>The following sample shows how to use this method.<code>...
        /// private void DoWork(string name) {
        ///   Verifier.Verify(name.Length > 0, "Name is not empty.");
        /// }</code></example>
        public static void Verify(bool condition, string description)
        {
            InternalVerify(condition, description, false);
        }

        /// <summary>
        /// Verifies whether a condition is true; throws an exception otherwise.
        /// </summary>
        /// <param name="condition">Expression that evaluates to true to continue execution.</param>
        /// <param name="description">Description of condition evaluated.</param>
        /// <param name="logAlways">If set to true, logs verification even when condition is true.</param>
        /// <example>The following sample shows how to use this method.<code>...
        /// private void DoWork(string name) {
        ///   Verifier.Verify(name.Length > 0,
        ///     "Name is not empty.", true);
        /// }</code></example>
        public static void Verify(bool condition, string description, bool logAlways)
        {
            InternalVerify(condition, description, logAlways);
        }

        /// <summary>Verifies whether a reference is non-null.</summary>
        /// <param name="o">Object reference to verify.</param>
        /// <param name="failDescription">Description of failure case, possibly null.</param>
        /// <remarks>This method can be used in place of IfNullFail.</remarks>
        public static void VerifyAssigned(object o, string failDescription)
        {
            if (o == null)
            {
                if (failDescription == null)
                    failDescription = "Unexpected null reference.";
               
                if (WillThrow)
                {
                    throw new Exception(failDescription);
                }
                else
                {
                    Failed = true;
                    Log(failDescription);
                }
            }
        }

        /// <summary>
        /// Verifies whether an expression has the expected value. The comparison
        /// goes beyond simple object.Equals().
        /// </summary>
        /// <param name="valueName">Name of the evaluated expression.</param>
        /// <param name="expectedValue">Value that valueName is expected to have.</param>
        /// <param name="actualValue">Value that valueName evaluates to.</param>
        /// <example>The following sample shows how to use this method.<code>...
        /// private void DoWork(string name) {
        ///   Verifier.VerifyValue("name", "Marcelo", name);
        /// }</code></example>
        public static void VerifyValue(string valueName, object expectedValue, object actualValue)
        {
            string description;     // Message to log.

            description = String.Format(
                "expected [{0}] to be [{1}] found to be [{2}]", valueName,
                expectedValue, actualValue);

            InternalVerify(AreValuesEqual(actualValue, expectedValue), description, true);
        }

        /// <summary>
        /// Verifies whether an expression is different from a given value.
        /// The comparison goes beyond a simple object.Equals().
        /// </summary>
        /// <param name="valueName">Name of the evaluated expression.</param>
        /// <param name="expectedDifferentValue">Value that valueName is expected to not have.</param>
        /// <param name="actualValue">Value that valueName evaluates to.</param>
        /// <example>The following sample shows how to use this method.<code>...
        /// private void DoWork(string name) {
        ///   Verifier.VerifyValueDifferent("name", "Marcelo", name);
        /// }</code></example>
        public static void VerifyValueDifferent(string valueName, object expectedDifferentValue, object actualValue)
        {
            string description;     // Message to log.

            description = String.Format(
                "expected [{0}] to not be [{1}] found to be [{2}]", valueName,
                expectedDifferentValue, actualValue);

            InternalVerify(!AreValuesEqual(actualValue, expectedDifferentValue), description, true);
        }

        /// <summary>
        /// Verifies whether an expression has the expected string value.
        /// </summary>
        /// <param name="description">Description of condition evaluated.</param>
        /// <param name="expectedValue">Value that valueName is expected to have.</param>
        /// <param name="actualValue">Value that valueName evaluates to.</param>
        /// <param name="logAlways">If set to true, logs verification even when condition is true.</param>
        /// <remarks>
        /// This method will ensure that the comparison is readable.
        /// </remarks>
        /// <example>The following sample shows how to use this method.<code>...
        /// private void DoWork(string name) {
        ///   Verifier.VerifyText("Name matches calculated value", "Marcelo", name, true);
        /// }</code></example>
        public static void VerifyText(string description, string expectedValue,
            string actualValue, bool logAlways)
        {
            bool condition;

            condition = expectedValue == actualValue;
            if (!condition || logAlways)
            {
                Logger.Current.Log(
                    "Evaluation of " + description + " is " + condition +
                    "\r\nExpected value [" + expectedValue + "]\r\nActual value   [" + actualValue + "]");
                InternalVerify(condition, description, true);
            }
        }

        /// <summary>
        /// Verifies whether an object property has an expected value.
        /// </summary>
        /// <param name='o'>Object with the property to evaluate.</param>
        /// <param name='propertyName'>Property name.</param>
        /// <param name='expectedValue'>Expected value for property.</param>
        /// <example>The following sample shows how to use this method.<code>...
        /// private void CheckZeroLength(TextBox box) {
        ///   Verifier.VerifyProperty(box, "Length", 0);
        /// }</code></example>
        public static void VerifyProperty(object o, string propertyName, object expectedValue)
        {
            object propertyValue = GetValue(o, propertyName);
            string description = String.Format(
                "{0} is [{1}] (actual value is [{2}])",
                propertyName, expectedValue, propertyValue);
            InternalVerify(
                (propertyValue == null && (expectedValue.ToString() == "null")) ||
                expectedValue.Equals(propertyValue), description, true);
        }

        /// <summary>Queues the verification of a property value.</summary>
        /// <param name="target">The object whose property will be verified.</param>
        /// <param name="propertyName">The name of the property to verify.</param>
        /// <param name="expectedValue">The expected value of the property.</param>
        /// <example>The following sample shows how to use this method.<code>...
        /// private void DoWork(TextBox box) {
        ///   QueueTyping(box, "Expected content.");
        ///   Verifier.QueueVerification(
        ///     box, "Text", "Expected content.");
        /// }</code></example>
        public static void QueueVerification(object target, string propertyName,
            object expectedValue)
        {
            string description = GetVerifyCaller(InternalQueuedFramesToSkip);
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(
                new VerifyPropertyHandler(ExecuteVerifyProperty),
                description, target, propertyName, expectedValue);
        }

        #endregion Public methods.


        #region Public properties.

        /// <summary>
        /// Indicates the number of stack frames external to the verifier
        /// that should not be investigated when reporting a dump.
        /// </summary>
        /// <remarks>
        /// The frames internal to the verifier are always ignored.
        /// </remarks>
        public static int StackFramesToSkip
        {
            get { return s_stackFramesToSkip; }
            set
            {
                s_stackFramesToSkip = (value < 0) ? 0 : value;
            }
        }

        ///<summary>
        ///Set/Get if we will through on a fail. 
        /// </summary>
        public static bool WillThrow
        {
            get
            {
                return s_willThrow;
            }
            set
            {
                s_willThrow = value;
            }
        }

        /// <summary>
        /// Set/Get the failed flag.
        /// </summary>
        public static bool Failed
        {
            get
            {
                return s_failed; 
            }
            set
            {
                s_failed = value; 
            }
        }

        #endregion Public properties.


        #region Private methods.

        /// <summary>
        /// Verifies whether a condition is true; throws an exception otherwise.
        /// </summary>
        /// <param name="condition">Expression that evaluates to true to continue execution.</param>
        /// <param name="description">Description of condition evaluated.</param>
        /// <param name="logAlways">If set to true, logs verification even when condition is true.</param>
        /// <remarks>
        /// This method attempts to retrieve the original caller of one of the exposed
        /// verification methods. To do this, it is necessary that the Verify methods
        /// call this method directly, without any intermediate method calls.
        /// </remarks>
        private static void InternalVerify(bool condition, string description, bool logAlways)
        {
            // Only request the caller information if it is required.
            string caller = (logAlways || !condition)?
                GetVerifyCaller(InternalFramesToSkip) : String.Empty;
            if (!condition)
            {
                string message = String.Format("Condition in [{0}] is false: {1}",
                    caller.ToString(), description);

                if (WillThrow)
                {
                    throw new Exception(message);
                }
                else
                {
                    Failed = true;
                    Log(message);
                }
            }
            if (logAlways)
            {
                Log("Condition in "+ caller.ToString()+" is true: "+ description);
            }
        }

        /// <summary>
        /// Retrieves the caller of a public/protected verification method, with
        /// best-effort semantics.
        /// </summary>
        /// <remarks>
        /// This method should only be called by InternalVerify or QueueVerification;
        /// otherwise, the wrong stack frame may be inspected. The method will
        /// add the number of external frames to skip itself, based on the
        /// StackFramesToSkip property.
        /// </remarks>
        /// <returns>The name (possibly with debug information) of the method that
        /// called the verification method, null if impossible to retrieve.</returns>
        private static string GetVerifyCaller(int framesToSkip)
        {
            System.Security.Permissions.ReflectionPermission perm =
                new System.Security.Permissions.ReflectionPermission(
                System.Security.Permissions.PermissionState.Unrestricted);
            perm.Assert();
            framesToSkip += StackFramesToSkip;
            try
            {
                StackTrace stack = new StackTrace(framesToSkip);
                StackFrame frame = stack.GetFrame(0);
                string fileName = frame.GetFileName();
                if (fileName == null)
                    return frame.GetMethod().Name;
                else
                    return String.Format("{0} ({1}:{2})",
                        frame.GetMethod().Name, fileName, frame.GetFileLineNumber());
            }
            catch(Exception)
            {
                return "unknown method";
            }
        }

        /// <summary>
        /// Verifies the property.
        /// </summary>
        /// <param name="sourceDescription">Description of where the verification was requested.</param>
        /// <param name="target">Object whose property is to be tested.</param>
        /// <param name="propertyName">Name of the property to test.</param>
        /// <param name="expectedValue">Matching value.</param>
        private static void ExecuteVerifyProperty(string sourceDescription,
            object target, string propertyName, object expectedValue)
        {
            object actualValue = GetValue(target, propertyName);
            if (actualValue.Equals(expectedValue))
            {
                Log(String.Format("Queued verification of {0} passed.",
                    propertyName));
            }
            else
            {
                string description = String.Format(
                    "Queued verification requested by {0} expected " +
                    "property [{1}] to be [{2}] found to be [{3}]",
                    sourceDescription, propertyName,
                    expectedValue.ToString(), actualValue.ToString());

                if (WillThrow)
                {
                    throw new Exception(description);
                }
                else
                {
                    Failed = true;
                    Log(description);
                }
            }
        }

        /// <summary>
        /// Returns a named property value from an object.
        /// </summary>
        /// <param name="target">Object to retrieve the value from.</param>
        /// <param name="propertyName">Case-sensitive name of the property.</param>
        /// <returns>Value of the property on target.</returns>
        public static object GetValue(object target, string propertyName)
        {
            System.Security.Permissions.ReflectionPermission perm =
                new System.Security.Permissions.ReflectionPermission(
                System.Security.Permissions.PermissionState.Unrestricted);
            perm.Assert();
            Type t = target.GetType();
            PropertyInfo prop = t.GetProperty(propertyName);
            return prop.GetValue(target, null);
        }

        /// <summary>Centralizes logging.</summary>
        private static void Log(string format, params object[] args)
        {
            Logger.Current.Log(format, args);
        }

        #endregion Private methods.


        #region Private properties.

        /// <summary>Number of stack frames to skip for Verifier calls.</summary>
        private const int InternalFramesToSkip = 3;

        /// <summary>Number of stack frames to skip for queued callbacks.</summary>
        private const int InternalQueuedFramesToSkip = 2;

        /// <summary>user can decide if we should throw exception when failed</summary>
        private static bool s_willThrow = true;

        /// <summary>Remember if there is a fail</summary>
        private static bool s_failed = false; 

        #endregion Private properties.


        #region Private fields.

        /// <summary>Number of stack frames to skip when reflecting.</summary>
        private static int s_stackFramesToSkip = 0;

        #endregion Private fields.
    }
}
