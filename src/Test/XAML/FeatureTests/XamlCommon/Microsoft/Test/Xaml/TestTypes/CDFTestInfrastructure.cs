// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This file contains types in Microsoft.Infrastructure.dll and Microsoft.Infrastructure.Test.dll 
// shipped by http://etcm. We are using these sources so that we dont have to checkin these binaries
// into the wpf test framework
// These types are required by cdf xaml tests to build. 

using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Test.CDFInfrastructure;

namespace Microsoft.Test.CDFInfrastructure
{
    /// <summary>
    /// delegate AddTestCaseEventHandler
    /// </summary>
    /// <param name="testCase">TestCase Attribute</param>
    /// <param name="methodInfo">MethodInfo value</param>
    /// <param name="methodParaemters">method parameters</param>
    public delegate void AddTestCaseEventHandler(TestCaseAttribute testCase, MethodInfo methodInfo, params object[] methodParaemters);

    /// <summary>
    /// TestSystem Exception
    /// </summary>
    [Serializable]
    public class TestSystemException : InfrastructureException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestSystemException"/> class.
        /// </summary>
        public TestSystemException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestSystemException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public TestSystemException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestSystemException"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments</param>
        public TestSystemException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestSystemException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public TestSystemException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestSystemException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected TestSystemException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// TestCase Exception
    /// </summary>
    [Serializable]
    public class TestCaseException : TestSystemException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseException"/> class.
        /// </summary>
        public TestCaseException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public TestCaseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseException"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments</param>
        public TestCaseException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public TestCaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected TestCaseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// TestCaseFailed Exception
    /// </summary>
    [Serializable]
    public class TestCaseFailedException : TestCaseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseFailedException"/> class.
        /// </summary>
        public TestCaseFailedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseFailedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public TestCaseFailedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseFailedException"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments</param>
        public TestCaseFailedException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseFailedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public TestCaseFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseFailedException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected TestCaseFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// TestCaseSkipped Exception
    /// </summary>
    [Serializable]
    public class TestCaseSkippedException : TestCaseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseSkippedException"/> class.
        /// </summary>
        public TestCaseSkippedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseSkippedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public TestCaseSkippedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseSkippedException"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments</param>
        public TestCaseSkippedException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseSkippedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public TestCaseSkippedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseSkippedException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected TestCaseSkippedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// Infrastructure Exception
    /// </summary>
    [Serializable]
    public class InfrastructureException : Exception
    {
        /// <summary>
        /// ListDictionary DisabledList
        /// </summary>
        [NonSerialized] 
        private static ListDictionary s_disabledList;

        /// <summary>
        /// Singleton lock
        /// </summary>
        [NonSerialized] 
        private static object s_lockObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="InfrastructureException"/> class.
        /// </summary>
        public InfrastructureException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InfrastructureException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InfrastructureException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InfrastructureException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="disableLogging">if set to <c>true</c> [disable logging].</param>
        public InfrastructureException(string message, bool disableLogging)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InfrastructureException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InfrastructureException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InfrastructureException"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments</param>
        public InfrastructureException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InfrastructureException"/> class.
        /// </summary>
        /// <param name="disableLogging">if set to <c>true</c> [disable logging].</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments</param>
        public InfrastructureException(bool disableLogging, string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InfrastructureException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="disableLogging">if set to <c>true</c> [disable logging].</param>
        public InfrastructureException(string message, Exception innerException, bool disableLogging)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InfrastructureException"/> class.
        /// </summary>
        /// <param name="info">The info value.</param>
        /// <param name="context">The context.</param>
        /// <param name="disableLogging">if set to <c>true</c> [disable logging].</param>
        protected InfrastructureException(SerializationInfo info, StreamingContext context, bool disableLogging)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InfrastructureException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected InfrastructureException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Disables the exception logging.
        /// </summary>
        /// <param name="exceptionType">Type of the exception.</param>
        public static void DisableExceptionLogging(Type exceptionType)
        {
            if (s_lockObject == null)
            {
                s_lockObject = new object();
            }

            if (s_lockObject != null)
            {
                lock (s_lockObject)
                {
                    if (s_disabledList == null)
                    {
                        s_disabledList = new ListDictionary();
                    }

                    if ((s_disabledList != null) && (s_disabledList[exceptionType] == null))
                    {
                        s_disabledList.Add(exceptionType, true);
                    }
                }
            }
        }

        /// <summary>
        /// Enables the exception logging.
        /// </summary>
        /// <param name="exceptionType">Type of the exception.</param>
        public static void EnableExceptionLogging(Type exceptionType)
        {
            if (s_lockObject != null)
            {
                lock (s_lockObject)
                {
                    if (s_disabledList != null)
                    {
                        s_disabledList.Remove(exceptionType);
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether [is exception logging enabled] [the specified exception type].
        /// </summary>
        /// <param name="exceptionType">Type of the exception.</param>
        /// <returns>
        /// <c>true</c> if [is exception logging enabled] [the specified exception type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsExceptionLoggingEnabled(Type exceptionType)
        {
            bool flag = true;
            if (s_lockObject != null)
            {
                lock (s_lockObject)
                {
                    if (s_disabledList[exceptionType] != null)
                    {
                        flag = false;
                    }
                }
            }

            return flag;
        }
    }
}
