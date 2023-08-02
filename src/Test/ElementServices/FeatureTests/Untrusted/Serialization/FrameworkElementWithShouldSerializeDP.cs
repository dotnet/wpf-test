// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 
  
 * Revision:         $Revision: $
 
 * Filename:         $Source: $
********************************************************************/
using System;
using System.Windows;

namespace Avalon.Test.CoreUI.Serialization.Security
{
    #region Class FrameworkElementWithShouldSerializeDP
    /// <summary>
    /// This class defines custom FrameworkElement with a private ShouldSerialize function for Dependency property.
    /// </summary>
    public class FrameworkElementWithShouldSerializeDP : FrameworkElement
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public FrameworkElementWithShouldSerializeDP() : base()
        {
        }
        #endregion Constructor  

        #region ShouldDP

        /// <summary>
        /// Private ShouldSerialize function.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private bool ShouldSerializeShouldDP(DependencyObject o)
        {
            return true;
        }

        /// <summary>
        /// DependencyProperty with private ShouldSerialize function.
        /// </summary>
        public static DependencyProperty ShouldDPProperty = DependencyProperty.RegisterAttached("ShouldDP", typeof(String), typeof(FrameworkElementWithShouldSerializeDP), new FrameworkPropertyMetadata("ShouldBeSerialized"));

        /// <summary>
        ///  Getter.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static String GetShouldDP(DependencyObject e)
        {
            return (String)e.GetValue(ShouldDPProperty);
        }

        /// <summary>
        /// Setter.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetShouldDP(DependencyObject e, String myProperty)
        {
            e.SetValue(ShouldDPProperty, myProperty);
        }
        #endregion ShouldDP
    }
    #endregion Class FrameworkElementWithShouldSerializeDP
}
