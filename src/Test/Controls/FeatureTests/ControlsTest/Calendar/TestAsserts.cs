using System;
using System.Runtime.Serialization;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Assertions used in unit test validation.
    /// </summary>
    public static class TestAsserts
    {
        /// <summary>
        /// Assert strings are equal
        /// </summary>
        /// <exception cref="TestFailedException">If strings are not equal</exception>
        public static void AreEqual(object actual, object expected)
        {
            if (!Object.Equals(actual, expected))
            {
                throw new TestFailedException(string.Format("Object values are not equal. {0} should be == {1}", actual, expected));
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
                throw new TestFailedException(string.Format("String values are not equal. {0} should be == {1}", actual, expected));
            }
        }

        /// <summary>
        /// Assert strings are equal
        /// </summary>
        /// <exception cref="TestFailedException">If strings are not equal</exception>
        public static void AreEqual(string actual, string expected, string message)
        {
            if (!String.Equals(actual, expected))
            {
                throw new TestFailedException(string.Format("String values are not equal. {0}", message));
            }
        }

        /// <summary>
        /// Assert int are equal
        /// </summary>
        /// <exception cref="TestFailedException">If ints are not equal</exception>
        public static void AreEqual(int actual, int expected, string message)
        {
            if (!int.Equals(actual, expected))
            {
                throw new TestFailedException(string.Format("Int values are not equal. {0} Actual: {1}, Expected: {2}", message, actual, expected));
            }
        }

        /// <summary>
        /// Assert DateTime's are equal
        /// </summary>
        /// <exception cref="TestFailedException">If DateTime objects are not equal</exception>
        public static void AreEqual(DateTime actual, DateTime expected)
        {
            //if (!DateTime.Equals(actual,expected))
            if (actual.Year != expected.Year || actual.Month != expected.Month || actual.Day != expected.Day)
            {
                throw new TestFailedException(string.Format("DateTime values are not equal. {0} should be == {1}", actual.ToString(), expected.ToString()));
            }
        }

        /// <summary>
        /// Assert DayOfWeek's are equal
        /// </summary>
        /// <exception cref="TestFailedException">If DayOfWeek objects are not equal</exception>
        public static void AreEqual(DayOfWeek actual, DayOfWeek expected)
        {
            if (!Enum.Equals(actual, expected))
            {
                throw new TestFailedException(string.Format("DayOfWeek values are not equal. {0} should be == {1}", actual.ToString(), expected.ToString()));
            }
        }

        /// <summary>
        /// Assert DatePickerFormats are equal
        /// </summary>
        /// <exception cref="TestFailedException">If DatePickerFormat objects are not equal</exception>
        public static void AreEqual(DatePickerFormat actual, DatePickerFormat expected)
        {
            if (!Enum.Equals(actual, expected))
            {
                throw new TestFailedException(string.Format("DatePickerFormat values are not equal. {0} should be == {1}", actual.ToString(), expected.ToString()));
            }
        }

        /// <summary>
        /// Assert CalendarMode are equal
        /// </summary>
        /// <exception cref="TestFailedException">If CalendarMode objects are not equal</exception>
        public static void AreEqual(CalendarMode actual, CalendarMode expected)
        {
            if (!Enum.Equals(actual, expected))
            {
                throw new TestFailedException(string.Format("CalendarMode values are not equal. {0} should be == {1}", actual.ToString(), expected.ToString()));
            }
        }

        /// <summary>
        /// Assert CalendarSelectionMode are equal
        /// </summary>
        /// <exception cref="TestFailedException">If CalendarSelectionMode  objects are not equal</exception>
        public static void AreEqual(CalendarSelectionMode actual, CalendarSelectionMode expected)
        {
            if (!Enum.Equals(actual, expected))
            {
                throw new TestFailedException(string.Format("CalendarSelectionMode  values are not equal. {0} should be == {1}", actual.ToString(), expected.ToString()));
            }
        }

        /// <summary>
        /// Assert Doubles are equal
        /// </summary>
        /// <exception cref="TestFailedException">If CalendarSelectionMode  objects are not equal</exception>
        public static void AreEqual(double actual, double expected)
        {
            if (!Double.Equals(actual, expected))
            {
                throw new TestFailedException(string.Format("Double values are not equal. {0} should be == {1}", actual.ToString(), expected.ToString()));
            }
        }

        /// <summary>
        /// Assert Doubles are equal
        /// </summary>
        /// <exception cref="TestFailedException">If CalendarSelectionMode  objects are not equal</exception>
        public static void AreEqual(double actual, double expected, string message)
        {
            if (!Double.Equals(actual, expected))
            {
                throw new TestFailedException(string.Format("Double values are not equal. {0}", message));
            }
        }

        /// <summary>
        /// Assert AutomationControlType are equal
        /// </summary>
        /// <exception cref="TestFailedException">If CalendarSelectionMode  objects are not equal</exception>
        public static void AreEqual(AutomationControlType actual, AutomationControlType expected, string message)
        {
            if (!Enum.Equals(actual, expected))
            {
                throw new TestFailedException(string.Format("AutomationControlType values are not equal. {0}", message));
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
                throw new TestFailedException(message);
            }
        }

        /// <summary>
        /// Assert that DateTime is not null
        /// </summary>
        /// <exception cref="TestFailedException">If Nullable DateTime is null (HasValue)</exception>
        public static void IsNotNull(DateTime? datetime, string message)
        {
            if (!datetime.HasValue)
            {
                throw new TestFailedException(message);
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
                throw new TestFailedException(message);
            }
        }

        /// <summary>
        /// Assert DateTime is null
        /// </summary>
        /// <exception cref="TestFailedException">If Nullable DateTime is not null (!HasValue)</exception>
        public static void IsNull(DateTime? datetime, string message)
        {
            if (datetime.HasValue)
            {
                throw new TestFailedException(message);
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
                throw new TestFailedException(string.Format("{0}.", message));
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
                throw new TestFailedException(string.Format("{0}.", message));
            }
        }

        /// <summary>
        /// Assert that object is of specific type.
        /// </summary>
        /// <exception cref="TestFailedException">If object o is not of Type type</exception>
        public static void IsInstanceOfType(object o, Type type)
        {
            if (o.GetType().Name != type.Name && o.GetType().FullName != type.FullName)
                throw new TestFailedException(string.Format("Object {0} is not of type {1}.", o.ToString(), type.FullName));
        }

        /// <summary>
        /// Assert that exception is thrown
        /// </summary>
        /// <exception cref="TestFailedException">If method does not throw exception of Type exceptionType</exception>
        public static void IsExceptionThrown(Type exceptionType, ExceptionMethodDelegate method)
        {
            try
            {
                method.Invoke();
            }
            catch (Exception e)
            {
                if (e.GetType() == exceptionType)
                {
                    return;
                }
                else
                {
                    throw new TestFailedException(string.Format("Exception thrown was of the type '{0}' but the test expected '{1}'", e.GetType().Name, exceptionType.Name));
                }
            }
            throw new TestFailedException(string.Format("{0} was not thrown", exceptionType.Name));
        }
    }

    public delegate void ExceptionMethodDelegate();

    /// <summary>
    /// Exception thrown when tests fail.
    /// TestRuntime and our test framework does a great job of handling exceptions.  This is an easy way to 
    /// minimize test logging and managing of TestResults.  Now all i have to do is return TestResult.Pass 
    /// in each RunSteps, and if the test makes it that far without exceptions we pass.
    /// </summary>
    public class TestFailedException : Exception, ISerializable
    {
        public TestFailedException(string message) : base(message) { }

        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        #endregion
    }
}
