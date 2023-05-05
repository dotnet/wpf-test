// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// Custom IPVTObject
    /// </summary>
    [TypeConverter(typeof(Custom_IPVTConverter))]
    public class Custom_IPVTObject
    {
        /// <summary>
        /// Gets or sets the target object.
        /// </summary>
        /// <value>The target object.</value>
        public object TargetObject { get; set; }

        /// <summary>
        /// Gets or sets the target property.
        /// </summary>
        /// <value>The target property.</value>
        public object TargetProperty { get; set; }

        /// <summary>
        /// Gets or sets the type of the target property.
        /// </summary>
        /// <value>The type of the target property.</value>
        public string TargetPropertyType { get; set; }
    }

    /// <summary>
    /// Custom IPVT_DO
    /// </summary>
    [RuntimeNameProperty("Name")]
    [ContentProperty("Content")]
    public class Custom_IPVT_DO : DependencyObject
    {
        /// <summary>
        /// Using a DependencyProperty as the backing store for IPVTDependencyProperty.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty IPVTDependencyPropertyProperty =
            DependencyProperty.Register("IPVTDependencyProperty", typeof(Custom_IPVTObject), typeof(Custom_IPVT_DO), null);

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name value.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public Custom_IPVTObject Content { get; set; }

        #region IPVTDependencyProperty

        /// <summary>
        /// Gets or sets the IPVT dependency property.
        /// </summary>
        /// <value>The IPVT dependency property.</value>
        public Custom_IPVTObject IPVTDependencyProperty
        {
            get
            {
                return (Custom_IPVTObject) GetValue(IPVTDependencyPropertyProperty);
            }

            set
            {
                SetValue(IPVTDependencyPropertyProperty, value);
            }
        }

        #endregion
    }
}
