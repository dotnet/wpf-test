// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// Description:
//   This is very simple type with Value property.
//
// Owner: Microsoft
 
//

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Windows;
using System.ComponentModel;
using System.Windows.Markup;
using System.Threading;
using System.Windows.Threading;

namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// This is very simple type with Value property.
    /// </summary>
    public class CustomUIElementCustomPropertyType : UIElement
    {
        /// <summary>
        /// Default constructor for CustomUIElementCustomPropertyType.
        /// </summary>
        public CustomUIElementCustomPropertyType() : base()
        {
        }


        /// <summary>
        /// Property Value
        /// </summary>
        /// <value></value>
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
        /// <param name="e"></param>
        /// <returns></returns>
        public static CustomPropertyType GetCustomDP(DependencyObject e)
        {
            return e.GetValue(CustomDPProperty) as CustomPropertyType;
        }
        /// <summary>
        /// Gettor for CustomDP
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomDP(DependencyObject e, CustomPropertyType myProperty)
        {
            e.SetValue(CustomDPProperty, myProperty);
        }
        /// <summary>
        ///     This tests that a readonly dependency property 
        ///     does not get serialized
        /// </summary>
        public static DependencyProperty CustomDPProperty =
            DependencyProperty.RegisterAttached("CustomDP", typeof(CustomPropertyType), typeof(CustomUIElementCustomPropertyType));


        CustomPropertyType _customClr = new CustomPropertyType();
    }
}

