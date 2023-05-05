// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Driver
{
    using System;
    using System.Xaml;

    /// <summary>
    /// SchemaExtensibility test case information
    /// </summary>
    public class SchemaExtensibilityTestCaseInfo : TestCaseInfo
    {
        /// <summary>
        /// Gets or sets the Context to use for the schema test
        /// </summary>
        public Func<XamlSchemaContext> Context { get; set; }
    }
}
