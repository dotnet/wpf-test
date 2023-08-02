// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Avalon.Test.CoreUI.Trusted;

namespace Avalon.Test.CoreUI.Common
{
    /// <summary>
    /// If you want to have a test case with an abstraction for Surface and Message Pump,
    /// the test case should implement this interface.
    /// </summary>
    public interface IHostedTest
    {
        /// <summary>
        /// Represents the current ITestContainer for this IHostedTest.
        /// </summary>
        ITestContainer TestContainer
        {
            get;
            set;
        }
    }
}

