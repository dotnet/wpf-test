// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Schema
{
    using System;

    /// <summary>
    /// Attribute to be attached to all concrete schema tests.
    /// This is necessary to discover the classes at runtime.
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Class)]
    public sealed class SchemaTestAttribute : Attribute
    {
    }
}
