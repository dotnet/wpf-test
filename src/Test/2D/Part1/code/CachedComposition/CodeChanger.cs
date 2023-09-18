// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Logging;


namespace Microsoft.Test.Graphics.CachedComposition
{
    /// <summary>
    /// Change the content by directly manipulating colors through object references.
    /// </summary>
    class CodeChanger : Changer
    {
        public override TestResult Change()
        {
            this.content.ChangeColor();
            return TestResult.Pass;
        }
    }
}
