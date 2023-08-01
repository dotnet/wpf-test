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
	public class ExpressionElement : FrameworkElement
    {
        #region Construction

        /// <summary>
        /// 
        /// </summary>
		public ExpressionElement()
        {

        }
        
        #endregion Construction



        #region CustomExpression

        /// <summary>
        ///     This tests that a dependency property 
        ///     value that is set to a custom expression
        ///     gets serialized correctly as "[identity]"
        /// </summary>
        public static DependencyProperty CustomExprDPProperty = 
            DependencyProperty.Register("CustomExprDP", typeof(CustomItem2), typeof(ExpressionElement));

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
		public CustomItem2 CustomExprDP
        {
            get { return (CustomItem2)GetValue(CustomExprDPProperty); }
            set { SetValue(CustomExprDPProperty, value); }
        }
        
        #endregion CustomExpression
       
    }
}

