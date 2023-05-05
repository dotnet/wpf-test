// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.Xaml.TestTypes
{
    /// <summary>
    /// Abstract class for the test types used in Xaml
    /// </summary>
    public abstract class XamlTestType
    {
        /// <summary>
        /// Main method of execution for each test type
        /// </summary>
        public abstract void Run();
    }
}
