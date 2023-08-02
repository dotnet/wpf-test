// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Custimized Canvas which implements INameScope.
 *
 
  
 * Revision:         $Revision: $
 
 * Filename:         $Source: $
********************************************************************/
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using Microsoft.Test.Discovery;

namespace Avalon.Test.CoreUI.IdTest
{
    /// <summary>
    /// Custimized Button With RuntimeNamePropertyAttribute.
    /// </summary>
    [RuntimeNamePropertyAttribute("MyName")]
    public class ButtonWithRuntimeNamePropertyAttribute : Button
    {
        /// <summary>
        /// 
        /// </summary>
        public ButtonWithRuntimeNamePropertyAttribute()
            : base()
        { }
        #region MyName
        /// <summary>
        /// 
        /// </summary>
        public string MyName
        {
            set
            {
                _myName = value;
            }
            get
            {
                return _myName;
            }
        }
        private string _myName = string.Empty;
        #endregion 
    }
    /// <summary>
    /// Custimized Bold With RuntimeNamePropertyAttribute.
    /// </summary>
    [RuntimeNamePropertyAttribute("MyName")]
    public class BoldWithRuntimeNamePropertyAttribute : Bold
    {
        /// <summary>
        /// 
        /// </summary>
        public BoldWithRuntimeNamePropertyAttribute()
            : base()
        { }
        #region MyName
        /// <summary>
        /// 
        /// </summary>
        public string MyName
        {
            set
            {
                _myName = value;
            }
            get
            {
                return _myName;
            }
        }
        private string _myName = string.Empty;
        #endregion 
    }
    /// <summary>
    /// Custimized UIElement With RuntimeNamePropertyAttribute.
    /// </summary>
    [RuntimeNamePropertyAttribute("MyName")]
    public class UIElementWithRuntimeNamePropertyAttribute : UIElement
    {
        /// <summary>
        /// 
        /// </summary>
        public UIElementWithRuntimeNamePropertyAttribute()
            : base()
        { }
        #region MyName
        /// <summary>
        /// 
        /// </summary>
        public string MyName
        {
            set
            {
                _myName = value;
            }
            get
            {
                return _myName;
            }
        }
        private string _myName = string.Empty;
        #endregion 
    }
    /// <summary>
    /// Custimized TextBlock With RuntimeNamePropertyAttribute.
    /// </summary>
    [RuntimeNamePropertyAttribute("MyName")]
    public class TextBlockWithRuntimeNamePropertyAttribute : TextBlock
    {
        /// <summary>
        /// 
        /// </summary>
        public TextBlockWithRuntimeNamePropertyAttribute()
            : base()
        { }
        #region MyName
        /// <summary>
        /// 
        /// </summary>
        public string MyName
        {
            set
            {
                SetValue(MyNameProperty, value);
            }
            get
            {
                return GetValue(MyNameProperty) as String;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty MyNameProperty = DependencyProperty.RegisterAttached("MyName", typeof(String), typeof(TextBlockWithRuntimeNamePropertyAttribute), new FrameworkPropertyMetadata(String.Empty));

        #endregion
    }
}
