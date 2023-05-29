// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Avalon.Test.CoreUI;
using System.Windows;
using System.Threading;
using System.Windows.Threading;

using System.Windows.Media;
using System.Collections;

namespace Avalon.Test.CoreUI.Events
{
    /// <summary>
    ///     VisualElement class is a subclass of DrawingVisual. It implementApependChild and RemoveChild
    /// </summary>
    public class VisualElement : DrawingVisual
    {
        #region Construction

        /// <summary>
        ///     Constructor for  CustomControl
        /// </summary>
        public VisualElement () : base()
        {
        }

        #endregion Construction

        #region External API

        /// <summary>
        /// Appends a child.
        /// </summary>
        public void AppendChild(Visual child)
        {
            this.Children.Add(child);
        }
        /// <summary>
        /// Remove a child.
        /// </summary>
        public void RemoveChild(Visual child)
        {
            this.Children.Remove(child);
        }
        #endregion External API

    }
}
