// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Avalon.Test.CoreUI.CoreInput.Common.Controls
{
    /// <summary>
    /// Interface exposing a class with text.
    /// </summary>
    /// <remarks>
    /// Implemented by content panel classes.
    /// Hosts use classes with this interface to do their work.
    /// </remarks>
    public interface IHasText
    {
        /// <summary>
        /// Text for this class.
        /// </summary>
        /// <value>UI-visible string of text.</value>
        string Text
        {
            get;
        }
    }
}
