// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using System.AddIn.Pipeline;
using System.Windows.Controls;

namespace Microsoft.Test.AddIn
{

    [AddInBaseAttribute()]
    public abstract class AddInCountClicksView
    {
        /// <summary>
        /// Number of clicks the AddIn has counted
        /// </summary>
        public abstract int Clicks { get; set;}

        /// <summary>
        /// Prepares AddIn to be used.
        /// </summary>
        /// <param name="addInParameters">string of the AddIn Parameters</param>
        public abstract void Initialize(string addInParameters);

        /// <summary>
        /// Gets the AddIn's UI
        /// </summary>
        /// <returns>Root FrameworkElement of the AddIn's UI</returns>
        public abstract FrameworkElement GetAddInUserInterface();

        /// <summary>
        /// Gets the AddIn's UI child (the button)
        /// </summary>
        /// <returns>Child FrameworkElement of the AddIn's UI</returns>
        public abstract FrameworkElement GetAddInUserInterfaceChild();
    }
}
