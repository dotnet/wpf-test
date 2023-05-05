// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.DataServices
{
    using System;
    using Microsoft.Test.Logging;

    /// <summary>
    /// Assertions used in test validation.
    /// </summary>
    public static class Assert
    {
        public static void Throw(string message)
        {
            throw new TestFailedException(message);
        }

        /// <summary>
        /// Assert objects are equal
        /// </summary>
        /// <exception cref="TestFailedException">If strings are not equal</exception>
        public static void AreEqual(object actual, object expected)
        {
            if (!Object.Equals(actual, expected))
            {
                Throw(string.Format("Object values are not equal. '{0}' should be == '{1}'", actual, expected));
            }
        }

        /// <summary>
        /// Assert strings are equal
        /// </summary>
        /// <exception cref="TestFailedException">If strings are not equal</exception>
        public static void AreEqual(string actual, string expected)
        {
            if (!String.Equals(actual, expected))
            {
                Throw(string.Format("String values are not equal. '{0}' should be == '{1}'", actual, expected));
            }
        }

        /// <summary>
        /// Assert Doubles are equal
        /// </summary>
        /// <exception cref="TestFailedException">If objects are not equal</exception>
        public static void AreEqual(double actual, double expected)
        {
            if (!Double.Equals(actual, expected))
            {
                Throw(string.Format("Double values are not equal. '{0}' should be == '{1}'", actual.ToString(), expected.ToString()));
            }
        }

        /// <summary>
        /// Assert Enum are equal
        /// </summary>
        /// <exception cref="TestFailedException">If Enum values are not equal</exception>
        public static void AreEqual(Enum actual, Enum expected)
        {
            if (actual.CompareTo(expected) != 0)
            {
                Throw(string.Format("Enum values are not equal. '{0}' should be == '{1}'", actual.ToString(), expected.ToString()));
            }
        }

        /// <summary>
        /// Assert that object is not null
        /// </summary>
        /// <exception cref="TestFailedException">If if object is null</exception>
        public static void IsNotNull(object o, string message)
        {
            if (o == null)
            {
                Throw(message);
            }
        }

        /// <summary>
        /// Assert that string is not null or empty
        /// </summary>
        /// <exception cref="TestFailedException">If if string is null or empty</exception>
        public static void IsNotNull(string s, string message)
        {
            if (string.IsNullOrEmpty(s))
            {
                Throw(message);
            }
        }

        /// <summary>
        /// Assert object is null
        /// </summary>
        /// <exception cref="TestFailedException">If object is not null</exception>
        public static void IsNull(object o, string message)
        {
            if (o != null)
            {
                Throw(message);
            }
        }

        /// <summary>
        /// Assert string is null or empty
        /// </summary>
        /// <exception cref="TestFailedException">If string is not null or empty</exception>
        public static void IsNull(string s, string message)
        {
            if (!string.IsNullOrEmpty(s))
            {
                Throw(message);
            }
        }

        /// <summary>
        /// Assert that condition is true.
        /// </summary>
        /// <exception cref="TestFailedException">If bool condition is false</exception>
        public static void IsTrue(bool condition, string message)
        {
            if (!condition)
            {
                Throw(message);
            }
        }

        /// <summary>
        /// Assert that condition is false.
        /// </summary>
        /// <exception cref="TestFailedException">If bool condition is true</exception>
        public static void IsFalse(bool condition, string message)
        {
            if (condition)
            {
                Throw(message);
            }
        }

        /// <summary>
        /// Assert that object is of specific type.
        /// </summary>
        /// <exception cref="TestFailedException">If object o is not of Type type</exception>
        public static void IsInstanceOfType(object o, Type type)
        {
            if (o.GetType().Name != type.Name && o.GetType().FullName != type.FullName)
                Throw(string.Format("Object {0} is not of type {1}.", o.ToString(), type.FullName));
        }
    }
}
