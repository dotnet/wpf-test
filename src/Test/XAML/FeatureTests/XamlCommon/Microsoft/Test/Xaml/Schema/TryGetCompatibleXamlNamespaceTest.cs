// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Schema
{
    /// <summary>
    /// Test XamlSchemaContext.TryGetCompatibleXamlNamespace method
    /// </summary>
    public abstract class TryGetCompatibleXamlNamespaceTest : SchemaTestBase
    {
        /// <summary>
        /// Call TryGetCompatibleXamlNamespace with given inputs and verify its return value and out value
        /// </summary>
        /// <param name="inputXamlNamespace">input argument</param>
        /// <param name="expectedReturnValue">expected return value</param>
        /// <param name="expectedOutValue">expected out value</param>
        protected void CallAndVerify(string inputXamlNamespace, bool expectedReturnValue, string expectedOutValue)
        {
            string outValue = null;
            bool returnValue = SchemaContext.TryGetCompatibleXamlNamespace(inputXamlNamespace, out outValue);

            GlobalLog.LogEvidence("Returned: " + returnValue.ToString() + " Expected: " + expectedReturnValue);
            GlobalLog.LogEvidence("Output: " + outValue + " Expected: " + expectedOutValue);

            if (returnValue != expectedReturnValue || outValue != expectedOutValue)
            {
                throw new SchemaTestFailedException();
            }
        }
    }
}
