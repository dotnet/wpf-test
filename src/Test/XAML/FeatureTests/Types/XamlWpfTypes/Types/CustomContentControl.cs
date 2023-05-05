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
    #region Class CustomContentControl

    /// <summary>
    /// This class defines custom clr class with various custom properties
    /// </summary>
    public class CustomContentControl : ContentControl
    {
        /// <summary>
        /// DependencyProperty for the attached CustomDP property.
        /// </summary>
        private static DependencyProperty s_customDPProperty = DependencyProperty.RegisterAttached("CustomDP", typeof(string), typeof(CustomContentControl), new FrameworkPropertyMetadata("0.0"));

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomContentControl"/> class.
        /// </summary>
        public CustomContentControl() : base()
        {
        }

        #endregion Constructor

        #region Dependency Property     

        #region MyContentDP

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

        #endregion CustomDP

        #endregion Dependency Property  

        #region Clr Property    

        /// <summary>
        /// Gets or sets the custom CLR property.
        /// </summary>
        /// <value>The custom CLR property.</value>
        public string CustomClrProperty { get; set; }

        #endregion Clr Property 

        #region CustomDP Methods

        /// <summary>
        /// Gets the custom DP.
        /// </summary>
        /// <param name="e">The DependencyObject.</param>
        /// <returns>custom DP value</returns>
        public static string GetCustomDP(DependencyObject e)
        {
            return (string)e.GetValue(CustomDPProperty);
        }

        /// <summary>
        /// Sets the custom DP.
        /// </summary>
        /// <param name="e">The DependencyObject.</param>
        /// <param name="myProperty">My property.</param>
        public static void SetCustomDP(DependencyObject e, string myProperty)
        {
            e.SetValue(CustomDPProperty, myProperty);
        }

        #endregion
    }

    #endregion Class CustomContentControl
}
