// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace Avalon.Test.CoreUI.Common
{
    /// <summary>
    /// Framework element with publicly exposed inheritance behavior.
    /// </summary>
    /// <remarks>
    /// Property is normally marked as protected.
    /// </remarks>
    public class InheritanceBehaviorFrameworkElement : FrameworkElement
    {
        /// <summary>
        /// Expose protected property InheritanceBehavior as public
        /// </summary>
        public new InheritanceBehavior InheritanceBehavior
        {
            get
            {
                return base.InheritanceBehavior;
            }
            set
            {
                base.InheritanceBehavior = value;
            }
        }

	}
}
