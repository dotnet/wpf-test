// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***************************************************************************\
*
*
*  ExpressionElement control used for Serializer tests;
*
*
\***************************************************************************/
using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.ComponentModel;

using System.Windows;

using System.Windows.Markup;

namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// 
    /// </summary>
	public class ResourceReferenceExpressionElement : FrameworkElement
	{
        #region Construction
        /// <summary>
        /// 
        /// </summary>
		public ResourceReferenceExpressionElement()
        {
        }      
        #endregion Construction



        #region ResourceReferenceExpression
		/// <summary>
		///     This tests that a dependency property 
		///     value that is a resource reference 
		///     gets serialized correctly as "{resourceName}"
		/// </summary>
		public static DependencyProperty ResourceRefDP1Property = DependencyProperty.Register("ResourceRefDP1", typeof(CustomItem2), typeof(ResourceReferenceExpressionElement));

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public CustomItem2 ResourceRefDP1
		{
			get { return (CustomItem2)GetValue(ResourceRefDP1Property); }
			set { SetValue(ResourceRefDP1Property, value); }
		}

		/// <summary>
		///     This tests that a dependency property 
		///     value that is a resource reference 
		///     gets serialized correctly as "{*typeof(resourceName)}"
		/// </summary>
		public static DependencyProperty ResourceRefDP2Property = DependencyProperty.Register("ResourceRefDP2", typeof(CustomItem2), typeof(ResourceReferenceExpressionElement));

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public CustomItem2 ResourceRefDP2
		{
			get { return (CustomItem2)GetValue(ResourceRefDP2Property); }
			set { SetValue(ResourceRefDP2Property, value); }
		} 
        #endregion ResourceReferenceExpression
       
    }
}

