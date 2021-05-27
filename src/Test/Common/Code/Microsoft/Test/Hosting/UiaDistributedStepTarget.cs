// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Hosting
{
    /// <summary>
    /// Target for the Distributed Test Step
    /// </summary>
    public enum UiaDistributedStepTarget
    {
        /// <summary>
        /// Runs out of proc to the avalon app and is given an Automation Element
        /// </summary>
        AutomationElement,

        /// <summary>
        /// Run in proc of the Avalon App and is given the UIElement
        /// </summary>
        UiElement
    }


}
