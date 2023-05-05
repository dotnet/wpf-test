// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Utilities
{
    /// <summary>
    /// Assert Utility
    /// </summary>
    public static class Assert
    {
        /// <summary>
        /// Are equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        public static void AreEqual(decimal expected, decimal actual)
        {
            AreEqual(expected, actual, "Are not equal");
        }

        /// <summary>
        /// Are equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        public static void AreEqual(int expected, int actual)
        {
            AreEqual(expected, actual, "Are not equal");
        }

        /// <summary>
        /// Are equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        public static void AreEqual(uint expected, uint actual)
        {
            AreEqual(expected, actual, "Are not equal");
        }

        /// <summary>
        /// Are equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        public static void AreEqual(string expected, string actual)
        {
            AreEqual(expected, actual, "Are not equal");
        }

        /// <summary>
        /// Are equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        public static void AreEqual(float expected, float actual)
        {
            AreEqual(expected, actual, "Are not equal");
        }

        /// <summary>
        /// Are equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        public static void AreEqual(double expected, double actual)
        {
            AreEqual(expected, actual, "Are not equal");
        }

        /// <summary>
        /// Are equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        public static void AreEqual(object expected, object actual)
        {
            AreEqual(expected, actual, "Are not equal");
        }

        /// <summary>
        /// Are equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        /// <param name="message">The message.</param>
        public static void AreEqual(decimal expected, decimal actual, string message)
        {
            if (expected != actual)
            {
                Fail(message);
            }
        }

        /// <summary>
        /// Are equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        /// <param name="message">The message.</param>
        public static void AreEqual(int expected, int actual, string message)
        {
            if (expected != actual)
            {
                Fail(message);
            }
        }

        /// <summary>
        /// Are equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        /// <param name="message">The message.</param>
        public static void AreEqual(uint expected, uint actual, string message)
        {
            if (expected != actual)
            {
                Fail(message);
            }
        }

        /// <summary>
        /// Are equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        /// <param name="message">The message.</param>
        public static void AreEqual(string expected, string actual, string message)
        {
            if (expected != actual)
            {
                GlobalLog.LogEvidence("Expected:" + expected);
                GlobalLog.LogEvidence("Actual:" + actual);
                Fail(message);
            }
        }

        /// <summary>
        /// Are equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        /// <param name="message">The message.</param>
        public static void AreEqual(float expected, float actual, string message)
        {
            if (expected != actual)
            {
                Fail(message);
            }
        }

        /// <summary>
        /// Are equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        /// <param name="message">The message.</param>
        public static void AreEqual(double expected, double actual, string message)
        {
            if (expected != actual)
            {
                Fail(message);
            }
        }

        /// <summary>
        /// Are equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        /// <param name="message">The message.</param>
        public static void AreEqual(object expected, object actual, string message)
        {
            if (expected == null && actual == null)
            {
                return;
            }

            if (!expected.Equals(actual))
            {
                Fail(message);
            }
        }

        /// <summary>
        /// Are not equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        public static void AreNotEqual(decimal expected, decimal actual)
        {
            AreNotEqual(expected, actual, "Should not be equal");
        }

        /// <summary>
        /// Are not equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        public static void AreNotEqual(int expected, int actual)
        {
            AreNotEqual(expected, actual, "Should not be equal");
        }

        /// <summary>
        /// Are not equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        public static void AreNotEqual(uint expected, uint actual)
        {
            AreNotEqual(expected, actual, "Should not be equal");
        }

        /// <summary>
        /// Are not equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        public static void AreNotEqual(string expected, string actual)
        {
            AreNotEqual(expected, actual, "Should not be equal");
        }

        /// <summary>
        /// Are not equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        public static void AreNotEqual(float expected, float actual)
        {
            AreNotEqual(expected, actual, "Should not be equal");
        }

        /// <summary>
        /// Are not equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        public static void AreNotEqual(double expected, double actual)
        {
            AreNotEqual(expected, actual, "Should not be equal");
        }

        /// <summary>
        /// Are not equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        public static void AreNotEqual(object expected, object actual)
        {
            AreNotEqual(expected, actual, "Should not be equal");
        }

        /// <summary>
        /// Are not equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        /// <param name="message">The message.</param>
        public static void AreNotEqual(decimal expected, decimal actual, string message)
        {
            if (expected == actual)
            {
                Fail(message);
            }
        }

        /// <summary>
        /// Are not equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        /// <param name="message">The message.</param>
        public static void AreNotEqual(int expected, int actual, string message)
        {
            if (expected == actual)
            {
                Fail(message);
            }
        }

        /// <summary>
        /// Are not equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        /// <param name="message">The message.</param>
        public static void AreNotEqual(uint expected, uint actual, string message)
        {
            if (expected == actual)
            {
                Fail(message);
            }
        }

        /// <summary>
        /// Are not equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        /// <param name="message">The message.</param>
        public static void AreNotEqual(string expected, string actual, string message)
        {
            if (expected == actual)
            {
                Fail(message);
            }
        }

        /// <summary>
        /// Are not equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        /// <param name="message">The message.</param>
        public static void AreNotEqual(float expected, float actual, string message)
        {
            if (expected == actual)
            {
                Fail(message);
            }
        }

        /// <summary>
        /// Are not equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        /// <param name="message">The message.</param>
        public static void AreNotEqual(double expected, double actual, string message)
        {
            if (expected == actual)
            {
                Fail(message);
            }
        }

        /// <summary>
        /// Are not equal.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        /// <param name="message">The message.</param>
        public static void AreNotEqual(object expected, object actual, string message)
        {
            if (object.Equals(expected, actual))
            {
                Fail(message);
            }
        }

        /// <summary>
        /// Are the same.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        public static void AreSame(object expected, object actual)
        {
            AreSame(expected, actual, "Objects are not the same");
        }

        /// <summary>
        /// Are the same.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        /// <param name="message">The message.</param>
        public static void AreSame(object expected, object actual, string message)
        {
            if (!object.ReferenceEquals(expected, actual))
            {
                Assert.Fail(message);
            }
        }

        /// <summary>
        /// Are not same.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        public static void AreNotSame(object expected, object actual)
        {
            AreNotSame(expected, actual, "Objects are the same");
        }

        /// <summary>
        /// Are not same.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        /// <param name="message">The message.</param>
        public static void AreNotSame(object expected, object actual, string message)
        {
            if (object.ReferenceEquals(expected, actual))
            {
                Assert.Fail(message);
            }
        }

        /// <summary>
        /// Fails this instance.
        /// </summary>
        public static void Fail()
        {
            Fail("Please call Fail(message) not Fail()");
        }

        /// <summary>
        /// Fails the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Fail(string message)
        {
            throw new TestValidationException(message);
        }

        /// <summary>
        /// Determines whether the specified collection is empty.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public static void IsEmpty(ICollection collection)
        {
            IsEmpty(collection, "Collection is not empty");
        }

        /// <summary>
        /// Determines whether the specified collection is empty.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="message">The message.</param>
        public static void IsEmpty(ICollection collection, string message)
        {
            if (collection != null)
            {
                AreEqual(0, collection.Count, message);
            }
        }

        /// <summary>
        /// Determines whether the specified condition is false.
        /// </summary>
        /// <param name="condition">if set to <c>true</c> [condition].</param>
        public static void IsFalse(bool condition)
        {
            IsFalse(condition, "Is not False");
        }

        /// <summary>
        /// Determines whether the specified condition is false.
        /// </summary>
        /// <param name="condition">if set to <c>true</c> [condition].</param>
        /// <param name="message">The message.</param>
        public static void IsFalse(bool condition, string message)
        {
            if (condition)
            {
                Assert.Fail(message);
            }
        }

        /// <summary>
        /// Determines whether [is not null] [the specified o].
        /// </summary>
        /// <param name="obj">The object.</param>
        public static void IsNotNull(object obj)
        {
            IsNotNull(obj, "Object is null");
        }

        /// <summary>
        /// Determines whether [is not null] [the specified o].
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="message">The message.</param>
        public static void IsNotNull(object obj, string message)
        {
            if (obj == null)
            {
                Assert.Fail(message);
            }
        }

        /// <summary>
        /// Determines whether the specified o is null.
        /// </summary>
        /// <param name="obj">The object.</param>
        public static void IsNull(object obj)
        {
            IsNull(obj, "Reference is not null");
        }

        /// <summary>
        /// Determines whether the specified o is null.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="message">The message.</param>
        public static void IsNull(object obj, string message)
        {
            if (obj != null)
            {
                Assert.Fail(message);
            }
        }

        /// <summary>
        /// Determines whether the specified condition is true.
        /// </summary>
        /// <param name="condition">if set to <c>true</c> [condition].</param>
        public static void IsTrue(bool condition)
        {
            IsTrue(condition, "is not True");
        }

        /// <summary>
        /// Determines whether the specified condition is true.
        /// </summary>
        /// <param name="condition">if set to <c>true</c> [condition].</param>
        /// <param name="message">The message.</param>
        public static void IsTrue(bool condition, string message)
        {
            AreEqual(true, condition, message);
        }

        /// <summary>
        /// Determines whether [is instance of type] [the specified expected].
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        public static void IsInstanceOfType(Type expected, object actual)
        {
            IsInstanceOfType(expected, actual, String.Format("Object is not an instance of type '{0}'", expected.ToString()));
        }

        /// <summary>
        /// Determines whether [is instance of type] [the specified expected].
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        /// <param name="message">The message.</param>
        public static void IsInstanceOfType(Type expected, object actual, string message)
        {
            if (!expected.IsInstanceOfType(actual))
            {
                Assert.Fail(message);
            }
        }
    }
}
