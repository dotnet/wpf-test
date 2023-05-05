// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Test.Xaml.Types
{
    #region Class FrameworkElementWithimmutableProperties

    /// <summary>
    /// This class inherits UIElement and has additional clr and DP for 
    /// Style serailization test
    /// </summary>
    public class FrameworkElementWithimmutableProperties : FrameworkElement
    {
        /// <summary>
        /// StringDPProperty for the attached Dependency property.
        /// </summary>
        private static DependencyProperty s_stringDPProperty = DependencyProperty.RegisterAttached("StringDP", typeof(string), typeof(FrameworkElementWithimmutableProperties), new FrameworkPropertyMetadata("0.0"));

        /// <summary>
        /// Int32DPProperty for the attached Dependency property.
        /// </summary>
        private static DependencyProperty s_int32DPProperty = DependencyProperty.RegisterAttached("Int32DP", typeof(int), typeof(FrameworkElementWithimmutableProperties), new FrameworkPropertyMetadata(8));

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkElementWithimmutableProperties"/> class.
        /// </summary>
        public FrameworkElementWithimmutableProperties() : base()
        {
        }

        #endregion Constructor

        #region Clr Property    

        /// <summary>
        /// Gets or sets the string DP property.
        /// </summary>
        /// <value>The string DP property.</value>
        public static DependencyProperty StringDPProperty
        {
            get
            {
                return s_stringDPProperty;
            }

            set
            {
                s_stringDPProperty = value;
            }
        }

        /// <summary>
        /// Gets or sets the int32 DP property.
        /// </summary>
        /// <value>The int32 DP property.</value>
        public static DependencyProperty Int32DPProperty
        {
            get
            {
                return s_int32DPProperty;
            }

            set
            {
                s_int32DPProperty = value;
            }
        }

        /// <summary>
        /// Gets or sets the CLR string.
        /// </summary>
        /// <value>The CLR string.</value>
        public string ClrString { get; set; }

        /// <summary>
        /// Gets or sets the CLR int32.
        /// </summary>
        /// <value>The CLR int32.</value>
        public int ClrInt32 { get; set; }

        #endregion Clr Property   

        #region Dependency Property   

        /// <summary>
        /// Gets the string DP.
        /// </summary>
        /// <param name="e">The dependencyobject e.</param>
        /// <returns>string value</returns>
        public static string GetStringDP(DependencyObject e)
        {
            return (string) e.GetValue(StringDPProperty);
        }

        /// <summary>
        /// Sets the string DP.
        /// </summary>
        /// <param name="e">The dependencyobject e.</param>
        /// <param name="myProperty">My property.</param>
        public static void SetStringDP(DependencyObject e, string myProperty)
        {
            e.SetValue(StringDPProperty, myProperty);
        }

        /// <summary>
        /// Gets the int32 DP.
        /// </summary>
        /// <param name="e">The dependencyobject e.</param>
        /// <returns>int value. </returns>
        public static int GetInt32DP(DependencyObject e)
        {
            return (int) e.GetValue(Int32DPProperty);
        }

        /// <summary>
        /// Sets the int32 DP.
        /// </summary>
        /// <param name="e">The dependencyobject e.</param>
        /// <param name="myProperty">My property.</param>
        public static void SetInt32DP(DependencyObject e, int myProperty)
        {
            e.SetValue(Int32DPProperty, myProperty);
        }

        #endregion Dependency Property 
    }

    #endregion Class MyUIElementWithCustomProperties
}
