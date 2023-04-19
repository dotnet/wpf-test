// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace Microsoft.Test.AddIn
{
    /// <summary>
    /// HostViews should derive from this class if they do not derive from FrameworkElement
    /// </summary>
    public abstract class HostViewBase : IHostView
    {
        /// <summary>
        /// For AddIn Hosts that have Ui, and are not derived from FrameworkElement
        /// </summary>
        /// <returns>Gets the UI in the Host View</returns>
        public abstract FrameworkElement GetAddInUserInterface();

        /// <summary>
        /// Prepares AddIn to be used.
        /// </summary>
        /// <param name="addInParameters">string of the AddIn Parameters</param>
        public abstract void Initialize(string addInParameters);
    }
}
