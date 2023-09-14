// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Markup;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls;

namespace Avalon.Test.CoreUI.Serialization
{
    #region Class CustomContentControl
    /// <summary>
    /// This class defines custom clr class with various custom properties
    /// </summary>
    public class CustomContentControl : ContentControl
    {
        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        public CustomContentControl() : base()
        {

        }

        #endregion Constructor
        #region Dependency Property     


        #region MyContentDP
        /// <summary>
        /// DependencyProperty for the attached CustomDP property.
        /// </summary>
        public static DependencyProperty CustomDPProperty = DependencyProperty.RegisterAttached("CustomDP", typeof(String), typeof(CustomContentControl), new FrameworkPropertyMetadata("0.0"));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static String GetCustomDP(DependencyObject e)
        {
            return (String)e.GetValue(CustomDPProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomDP(DependencyObject e, String myProperty)
        {
            e.SetValue(CustomDPProperty, myProperty);
        }
        #endregion CustomDP

        #endregion Dependency Property  

        #region Clr Property    

        private string _customClrProperty;

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string CustomClrProperty
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


    }
    #endregion Class CustomContentControl
}
