// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
* Purpose: 
* Owner: Microsoft
* Contributors:
*******************************************************************/
using System;
using System.Runtime.Serialization;

// This suppresses warnings #'s not recognized by the compiler.
#pragma warning disable 1634, 1691

namespace InternalHelper.Tests
{
	/// -----------------------------------------------------------------------
	/// <summary>
	/// Control is not configured for the desired test
	/// </summary>
	/// -----------------------------------------------------------------------
	[Serializable]
	internal class IncorrectElementConfigurationForTestException : ApplicationException
	{
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public IncorrectElementConfigurationForTestException() : base() { }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public IncorrectElementConfigurationForTestException(string Reason) : base(Reason) { }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public IncorrectElementConfigurationForTestException(string Reason, Exception e) : base(Reason, e) { }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        protected IncorrectElementConfigurationForTestException(SerializationInfo serializationInfo, StreamingContext streamContext) : base(serializationInfo, streamContext) { }
	}

	/// -----------------------------------------------------------------------
	/// <summary>
	/// Warning that the test has a problem but cannot determine if this
	/// is an error.  Tester will need to determine if this scenario
	/// is a problem or not.
	/// </summary>
    /// -----------------------------------------------------------------------
    [Serializable]
	internal class TestWarningException : ApplicationException
	{
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public TestWarningException() : base() { }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public TestWarningException(string Reason) : base(Reason) { }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public TestWarningException(string Reason, Exception e) : base(Reason, e) { }

		protected TestWarningException(SerializationInfo serializationInfo, StreamingContext streamContext) : base(serializationInfo, streamContext) { }
	}

    /// -----------------------------------------------------------------------
    /// <summary>
    /// Error when the test fails according to the test plan
    /// </summary>
    /// -----------------------------------------------------------------------
    [Serializable]
    internal class TestErrorException : ApplicationException
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public TestErrorException() : base() { }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public TestErrorException(string Reason) : base(Reason) { }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public TestErrorException(string Reason, Exception e) : base(Reason, e) { }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        protected TestErrorException(SerializationInfo serializationInfo, StreamingContext streamContext) : base(serializationInfo, streamContext) { }
    }

    /// -----------------------------------------------------------------------
    /// <summary>
    /// Exception that is thrown by tests that are for know product issues. Example
    /// is an issue that is moved to Vienna.
    /// </summary>
    /// -----------------------------------------------------------------------
    [Serializable]
    internal class KnownProductIssueException : ApplicationException
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public KnownProductIssueException() : base() { }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public KnownProductIssueException(string Reason) : base(Reason) { }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public KnownProductIssueException(string Reason, Exception e) : base(Reason, e) { }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        protected KnownProductIssueException(SerializationInfo serializationInfo, StreamingContext streamContext) : base(serializationInfo, streamContext) { }
    }
}

