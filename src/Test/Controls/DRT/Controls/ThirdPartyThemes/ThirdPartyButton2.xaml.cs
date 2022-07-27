// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

using System.Windows.Controls;

namespace DRT.ThirdPartyThemes
{
    public class ThirdPartyButton2 : Button
    {
        static ThirdPartyButton2()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ThirdPartyButton2), new FrameworkPropertyMetadata(typeof(ThirdPartyButton2)));
        }

        private static readonly System.Windows.DependencyProperty s_indexProperty = 
                System.Windows.DependencyProperty.Register(
                        "Index", 
                        typeof(Int32), 
                        typeof(ThirdPartyButton2), 
                        new System.Windows.FrameworkPropertyMetadata(1));
        
        
        /// <summary>
        /// Index CLR Property Wrapper
        /// </summary>
        public Int32 Index
        {
            get { return (Int32) GetValue(s_indexProperty); }
            set { this.SetValue(s_indexProperty, value); }
        }
    }
}
