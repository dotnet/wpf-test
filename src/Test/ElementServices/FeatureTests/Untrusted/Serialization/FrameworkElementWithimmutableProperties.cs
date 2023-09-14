// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Windows;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Avalon.Test.CoreUI.Serialization
{
    #region Class FrameworkElementWithimmutableProperties
    /// <summary>
    /// This class inherits UIElement and has additional clr and DP for 
    /// Style serailization test
    /// </summary>
    public class FrameworkElementWithimmutableProperties : FrameworkElement
    {
        #region Constructor
 

		/// <summary>
		/// 
		/// </summary>
		public FrameworkElementWithimmutableProperties() : base()
		{
		}
        #endregion Constructor

        #region Clr Property    
		/// <summary>
		///  
		/// </summary>
		public string ClrString
		{
			get { return _clrStringProperty; }
			set { _clrStringProperty=value; }
		}

		string _clrStringProperty = null;

		/// <summary>
		///  
		/// </summary>
		public Int32 ClrInt32
		{
			get { return _clrInt32Property; }
			set { _clrInt32Property = value; }
		}

		Int32 _clrInt32Property = 0;
		#endregion Clr Property   
		#region Dependency Property   
		
		/// <summary>
		/// StringDPProperty for the attached Dependency property.
		/// </summary>
		public static DependencyProperty StringDPProperty = DependencyProperty.RegisterAttached("StringDP", typeof(String), typeof(FrameworkElementWithimmutableProperties), new FrameworkPropertyMetadata("0.0"));

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public static String GetStringDP(DependencyObject e)
		{
			return (String)e.GetValue(StringDPProperty);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		/// <param name="myProperty"></param>
		public static void SetStringDP(DependencyObject e, String myProperty)
		{
			e.SetValue(StringDPProperty, myProperty);
		}

		/// <summary>
		/// Int32DPProperty for the attached Dependency property.
		/// </summary>
		public static DependencyProperty Int32DPProperty = DependencyProperty.RegisterAttached("Int32DP", typeof(Int32), typeof(FrameworkElementWithimmutableProperties), new FrameworkPropertyMetadata(8));

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public static Int32 GetInt32DP(DependencyObject e)
		{
			return (Int32)e.GetValue(Int32DPProperty);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		/// <param name="myProperty"></param>
		public static void SetInt32DP(DependencyObject e, Int32 myProperty)
		{
			e.SetValue(Int32DPProperty, myProperty);
		}

        #endregion Dependency Property 

	}
    #endregion Class MyUIElementWithCustomProperties
}
