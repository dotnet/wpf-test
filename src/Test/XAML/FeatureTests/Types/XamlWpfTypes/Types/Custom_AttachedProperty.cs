// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// CustomAttached Test class
    /// </summary>
    public class CustomAttached
    {
        /// <summary>
        /// double property
        /// </summary>
        private static double s_prop;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAttached"/> class.
        /// </summary>
        public CustomAttached()
        {
        }

        /// <summary>
        /// Sets the attached prop.
        /// </summary>
        /// <value>The attached prop.</value>
        public double AttachedProp
        {
            set
            {
                s_prop = value;
            }
        }

        /// <summary>
        /// Gets the attached prop.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>value of attached prop</returns>
        public static double GetAttachedProp(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return s_prop;
        }

        /// <summary>
        /// Sets the attached prop.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="val">The double val.</param>
        public static void SetAttachedProp(UIElement element, double val)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            s_prop = val;
        }
    }

    /// <summary>
    /// CustomItem Test class
    /// </summary>
    public class CustomItem : UIElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomItem"/> class.
        /// </summary>
        public CustomItem()
        {
        }
    }
}
