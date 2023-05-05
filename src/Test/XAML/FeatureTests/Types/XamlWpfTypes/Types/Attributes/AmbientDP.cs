// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xaml;

namespace Microsoft.Test.Xaml.Types.Attributes
{
    /// <summary>
    /// Custom type for the Ambient Dependency property scenario
    /// </summary>
    public class AmbientDP : DependencyObject
    {
        /// <summary>
        /// Using a DependencyProperty as the backing store for MyDP.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MyDPProperty =
            DependencyProperty.Register("MyDP", typeof(int), typeof(AmbientDP), new UIPropertyMetadata(0));

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientDP"/> class.
        /// </summary>
        public AmbientDP()
        {
        }

        /// <summary>
        /// Gets or sets my DP.
        /// </summary>
        /// <value>My DP value.</value>
        [Ambient]
        public int MyDP
        {
            get
            {
                return (int) GetValue(MyDPProperty);
            }

            set
            {
                SetValue(MyDPProperty, value);
            }
        }
    }

    /// <summary>
    /// Custom type for the Ambient Attached property scenario
    /// </summary>
    public class AmbientAttached : DependencyObject
    {
        /// <summary>
        /// Using a DependencyProperty as the backing store for MyAttached.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MyAttachedProperty =
            DependencyProperty.Register("MyAttached", typeof(int), typeof(AmbientAttached), new UIPropertyMetadata(0));

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientAttached"/> class.
        /// </summary>
        public AmbientAttached()
        {
        }

        /// <summary>
        /// Gets or sets my attached.
        /// </summary>
        /// <value>My attached.</value>
        public int MyAttached
        {
            get
            {
                return (int) GetValue(MyAttachedProperty);
            }

            set
            {
                SetValue(MyAttachedProperty, value);
            }
        }
    }
}
