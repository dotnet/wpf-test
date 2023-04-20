// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// This is very simple type with Value property.
    /// </summary>
    public class CustomContentElementCustomPropertyType : FrameworkContentElement
    {
        /// <summary>
        ///     This tests that a readonly dependency property 
        ///     does not get serialized
        /// </summary>
        private static DependencyProperty s_customDPProperty =
            DependencyProperty.RegisterAttached("CustomDP", typeof(CustomPropertyType), typeof(CustomContentElementCustomPropertyType));

        /// <summary>
        /// CustomProperty Type
        /// </summary>
        private CustomPropertyType _customClr = new CustomPropertyType();

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomContentElementCustomPropertyType"/> class.
        /// </summary>
        public CustomContentElementCustomPropertyType() : base()
        {
        }

        /// <summary>
        /// Gets or sets the custom DP property.
        /// </summary>
        /// <value>The custom DP property.</value>
        public static DependencyProperty CustomDPProperty
        {
            get
            {
                return s_customDPProperty;
            }

            set
            {
                s_customDPProperty = value;
            }
        }

        /// <summary>
        /// Gets or sets the custom CLR.
        /// </summary>
        /// <value>The custom CLR.</value>
        public CustomPropertyType CustomClr
        {
            get
            {
                return _customClr;
            }

            set
            {
                _customClr = value;
            }
        }

        /// <summary>
        /// Settor for CustomDP
        /// </summary>
        /// <param name="e">The DependencyObject.</param>
        /// <returns>CustomProperty Type</returns>
        public static CustomPropertyType GetCustomDP(DependencyObject e)
        {
            return e.GetValue(CustomDPProperty) as CustomPropertyType;
        }

        /// <summary>
        /// Gettor for CustomDP
        /// </summary>
        /// <param name="e">The DependencyObject.</param>
        /// <param name="myProperty">My property.</param>
        public static void SetCustomDP(DependencyObject e, CustomPropertyType myProperty)
        {
            e.SetValue(CustomDPProperty, myProperty);
        }
    }
}
