// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// action indicates a state of a touch device
    /// </summary>
    public enum TouchDeviceAction
    {
        /// <summary>
        /// Add a touch
        /// </summary>
        TouchAdd,

        /// <summary>
        /// Remove a touch
        /// </summary>
        TouchRemove,

        /// <summary>
        /// Change a touch
        /// </summary>
        TouchChange,

        /// <summary>
        /// a Tap gesture
        /// </summary>
        TouchTapGesture,

        /// <summary>
        /// a Hold gesture
        /// </summary>
        TouchHoldGesture,
    }
}
