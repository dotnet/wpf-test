// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: A Custom Control with Attached property, Dependency property and Clr 
 * property of type Type. 
 *
 
  
 * Revision:         $Revision: 2 $
 
********************************************************************/
using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Controls;
using System.ComponentModel;

namespace Avalon.Test.CoreUI.Serialization
{
    #region Class CustomControlWithTypeProperties
    /// <summary>
    ///  Custom Control with clr, dp and attached property of type Type.
    /// </summary>
    public class CustomControlWithTypeProperties : ContentControl
    {
        #region Constructor
        /// <summary>
        /// Default contructor
        /// </summary>
        public CustomControlWithTypeProperties()
            : base()
        {
        }
        #endregion Constructor

        #region CustomDP
        /// <summary>
        /// DependencyProperty for the attached CustomDP property.
        /// </summary>
        public static DependencyProperty CustomDPProperty = DependencyProperty.Register("CustomDP", typeof(Type), typeof(CustomControlWithTypeProperties));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Type GetCustomDP(DependencyObject e)
        {
            return (Type)e.GetValue(CustomDPProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomDP(DependencyObject e, Type myProperty)
        {
            e.SetValue(CustomDPProperty, myProperty);
        }
        #endregion CustomDP

        #region Clr Property

        private Type _customClrProperty;

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [DefaultValue(null)]
        public Type CustomClrProperty
        {
            get
            {
                return _customClrProperty;
            }
            set
            {
                _customClrProperty = value;
            }
        }

        #endregion Clr Property

        #region CustomAttached
        /// <summary>
        /// DependencyProperty for the attached CustomAttached property.
        /// </summary>
        public static DependencyProperty CustomAttachedProperty = DependencyProperty.RegisterAttached("CustomAttached", typeof(Type), typeof(CustomControlWithTypeProperties));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Type GetCustomAttached(DependencyObject e)
        {
            return (Type)e.GetValue(CustomAttachedProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomAttached(DependencyObject e, Type myProperty)
        {
            e.SetValue(CustomAttachedProperty, myProperty);
        }
        #endregion CustomAttached
    }
    #endregion Class CustomContentControl
}
