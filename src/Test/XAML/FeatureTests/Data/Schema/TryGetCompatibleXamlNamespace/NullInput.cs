// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Schema
{
    /// <summary>
    /// Null input
    /// </summary>
    [SchemaTest]
    class NullInput : TryGetCompatibleXamlNamespaceTest
    {
        public override void Run()
        {
            try
            {
                string outValue = null;
                SchemaContext.TryGetCompatibleXamlNamespace(null, out outValue);
            }
            catch (ArgumentNullException)
            {
                GlobalLog.LogEvidence("ArgumentNullException thrown as expected");
                return;
            }

            GlobalLog.LogEvidence("ArgumentNullException not thrown");
            throw new SchemaTestFailedException();
        }
    }
}
