// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Xaml.Common;

    /// <summary>
    /// Commonly used utilities to work with Exceptions
    /// </summary>
    public static class ExceptionHelpers
    {
        /// <summary>
        /// Check the exception thrown by the given code
        /// </summary>
        /// <param name="exceptionType">Type of exception that should be thrown</param>
        /// <param name="tryCode">code that is expected to throw</param>
        public static void CheckForException(
            Type exceptionType,
            Action tryCode)
        {
            CheckForException(
                exceptionType,
                new Dictionary<string, string>(),
                tryCode,
                false);
        }

        /// <summary>
        /// Check the thrown exception
        /// </summary>
        /// <param name="exceptionType">type of expected exception</param>
        /// <param name="message">expected message</param>
        /// <param name="tryCode">code that is expected to throw</param>
        public static void CheckForException(Type exceptionType, string message, Action tryCode)
        {
            CheckForException(exceptionType, message, tryCode, false);
        }

        /// <summary>
        /// Check the thrown exception
        /// </summary>
        /// <param name="exceptionType">type of expected exception</param>
        /// <param name="message">expected message</param>
        /// <param name="tryCode">code that is expected to throw</param>
        /// <param name="checkValidationException">check for validation exception</param>
        public static void CheckForException(Type exceptionType, string message, Action tryCode, bool checkValidationException)
        {
            Dictionary<string, string> exceptionProperties = new Dictionary<string, string>();
            exceptionProperties.Add("Message", message);
            CheckForException(exceptionType, exceptionProperties, tryCode, checkValidationException);
        }

        /// <summary>
        /// Check for exception
        /// </summary>
        /// <param name="exceptionType">type of exception</param>
        /// <param name="exceptionProperties">exception properties</param>
        /// <param name="tryCode">code expected to throw</param>
        public static void CheckForException(
            Type exceptionType,
            Dictionary<string, string> exceptionProperties,
            Action tryCode)
        {
            CheckForException(
                exceptionType,
                exceptionProperties,
                tryCode,
                false);
        }

        /// <summary>
        /// Check for exception
        /// </summary>
        /// <param name="exceptionType">type of exception</param>
        /// <param name="exceptionProperties">exception properties</param>
        /// <param name="tryCode">code expected to throw</param>
        /// <param name="checkValidationException">check the validation exception</param>
        public static void CheckForException(
            Type exceptionType,
            Dictionary<string, string> exceptionProperties,
            Action tryCode,
            bool checkValidationException)
        {
            bool gotCorrectException = false;
            try
            {
                // call delegate
                tryCode();
            }
            catch (Exception exc)
            {
                ValidateException(exc, exceptionType, exceptionProperties);
                gotCorrectException = true;
            }

            if (!gotCorrectException)
            {
                throw new DataTestException(String.Format(
                                                        CultureInfo.InvariantCulture,
                                                        "Expected {0} to be thrown, but no exception was thrown.",
                                                        exceptionType.FullName));
            }
        }

        /// <summary>
        /// Validate the exception
        /// </summary>
        /// <param name="exception">expected exception</param>
        /// <param name="exceptionType">expected exception type</param>
        /// <param name="exceptionProperties">exception properties</param>
        public static void ValidateException(
            Exception exception,
            Type exceptionType,
            Dictionary<string, string> exceptionProperties)
        {
            // check for exception type mismatch
            if (exception.GetType().FullName != exceptionType.FullName)
            {
                throw new DataTestException(
                    String.Format(
                    CultureInfo.InvariantCulture,
                    "Expected {0} to be thrown, but {1} was thrown.",
                    exceptionType.FullName,
                    exception.GetType().FullName),
                    exception);
            }

            // check for property values
            if (exceptionProperties != null)
            {
                foreach (string propertyName in exceptionProperties.Keys)
                {
                    string expectedPropertyValue = exceptionProperties[propertyName];

                    PropertyInfo pi = exception.GetType().GetProperty(propertyName);

                    if (pi == null)
                    {
                        throw new Exception(
                            String.Format(
                            CultureInfo.InvariantCulture,
                            "Test issue: {0} doesn't have a property {1}",
                            exceptionType.FullName,
                            propertyName),
                            exception);
                    }

                    object actualPropertyObjectValue = pi.GetValue(exception, null);

                    if (actualPropertyObjectValue == null)
                    {
                        throw new DataTestException(
                            String.Format(
                            CultureInfo.InvariantCulture,
                            "Property {0} on {1} was expected to contain {2}. The actual value is null",
                            propertyName,
                            exceptionType.FullName,
                            expectedPropertyValue),
                            exception);
                    }

                    string actualPropertyValue = actualPropertyObjectValue.ToString();

                    if (!ExceptionMessageHelper.IsExceptionStringMatch(expectedPropertyValue, actualPropertyValue))
                    {
                        throw new DataTestException(
                            String.Format(
                            CultureInfo.InvariantCulture,
                            "Property {0} on {1} was expected to contain {2}. The actual value is {3}",
                            propertyName,
                            exceptionType.FullName,
                            expectedPropertyValue,
                            actualPropertyValue),
                            exception);
                    }
                }
            }

            Tracer.LogTrace("Exception was validated successfully");
        }

        /// <summary>
        /// Should be used when the test is blocked by the product.
        /// Helper added because throwing an exception at the beginning of a test case will give you a build warning,
        /// unreachable code detected, which because of build settings will be an error.  To resolve this you need to 
        /// have code like following:
        /// bool flag = true;
        /// if (flag)
        ///     throw new Exception();
        /// </summary>
        /// <param name="reason">The reason the test was blocked.</param>
        public static void ThrowTestCaseBlockedException(string reason)
        {
            throw new Exception(String.Format(CultureInfo.InvariantCulture, "TEST CASE BLOCKED - {0}", reason));
        }

        /// <summary>
        /// Should be used when the machine configuration does not included something
        /// required by the test case.  e.g. Sql or IIS not installed.
        /// Helper added because throwing an exception at the beginning of a test case will give you a build warning,
        /// unreachable code detected, which because of build settings will be an error.  To resolve this you need to 
        /// have code like following:
        /// bool flag = true;
        /// if (flag)
        ///     throw new Exception();
        /// </summary>
        /// <param name="reason">The reason the test was skipped.</param>
        public static void ThrowTestCaseSkippedException(string reason)
        {
            throw new TestCaseSkippedException(reason);
        }
    }
}
