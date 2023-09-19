// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Logging;


namespace Microsoft.Test.Graphics.CachedComposition
{
    /// <summary>
    /// Change content by changing the theme of the OS - high contrast is the ideal case here.
    /// </summary>
    class ThemeChanger : Changer
    {
        public override TestResult Change()
        {
            Microsoft.Test.Display.DisplayConfiguration.SetTheme("Windows Classic");
            return TestResult.Pass;
        }
    }
}
