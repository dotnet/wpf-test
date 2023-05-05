// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.DataServices
{
    using System;
    using System.Runtime.Serialization;

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
