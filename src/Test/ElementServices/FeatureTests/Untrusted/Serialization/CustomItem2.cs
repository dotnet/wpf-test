// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***************************************************************************\
*
*
*  CustomItem2 control used for Serializer tests;
*
*
\***************************************************************************/
using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Windows;


namespace Avalon.Test.CoreUI.Serialization
{
	/// <summary>
	/// 
	/// </summary>
	public class CustomItem2 : FrameworkElement
    {
        #region Construction

       /// <summary>
       /// 
       /// </summary>
		public CustomItem2()
        {
        }
        
        #endregion Construction

        #region Events
        
        /// <summary>
        /// 
        /// </summary>
		public static readonly RoutedEvent AttachedREEvent = 
            EventManager.RegisterRoutedEvent("AttachedRE", RoutingStrategy.Bubble, typeof(EventHandler), typeof(CustomItem2));

        /// <summary>
        /// 
        /// </summary>
		public static readonly RoutedEvent NoWrapperAttachedREEvent = 
            EventManager.RegisterRoutedEvent("NoWrapperAttachedRE", RoutingStrategy.Bubble, typeof(EventHandler), typeof(CustomItem2));
        
        #endregion Events
        
        #region Properties

        /// <summary>
        /// 
        /// </summary>
		public static DependencyProperty IntNoWrapperAttachedDPProperty = 
            DependencyProperty.Register("IntNoWrapperAttachedDP", typeof(int), typeof(CustomItem2));
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
		public int GetIntNoWrapperAttachedDP(DependencyObject d)
        {
            return (int)d.GetValue(IntNoWrapperAttachedDPProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
		public void SetIntNoWrapperAttachedDP(DependencyObject d, int value)
        {
            d.SetValue(IntNoWrapperAttachedDPProperty, value);
        }


        /// <summary>
        /// 
        /// </summary>
		public static DependencyProperty IntHiddenAttachedDPProperty = 
            DependencyProperty.Register("IntHiddenAttachedDP", typeof(int), typeof(CustomItem2));
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
		public static int GetIntHiddenAttachedDP(DependencyObject d)
        {
            return (int)d.GetValue(IntHiddenAttachedDPProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
		public static void SetIntHiddenAttachedDP(DependencyObject d, int value)
        {
            d.SetValue(IntHiddenAttachedDPProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
		public static bool ShouldSerializeIntHiddenAttachedDP(DependencyObject d)
        {
            if ((int)d.GetValue(IntHiddenAttachedDPProperty) == 1)
            {
                return false;
            }

            return true;
        }

		/// <summary>
		/// 
		/// </summary>
		public static DependencyProperty AliasDPProperty = DependencyProperty.Register("AliasDP", typeof(string), typeof(CustomItem2));

		/// <summary>
		/// 
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public string GetAliasDP(DependencyObject d)
		{
			return (string)d.GetValue(AliasDPProperty);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="d"></param>
		/// <param name="value"></param>
		public void SetAliasDP(DependencyObject d, string value)
		{
			d.SetValue(AliasDPProperty, value);
		}
        
        #endregion Properties
    }
}

