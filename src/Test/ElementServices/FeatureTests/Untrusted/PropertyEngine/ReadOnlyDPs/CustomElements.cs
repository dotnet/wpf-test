// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Custom Elements Definition for ReadOnly DP / Inheritance Behavior Test Suite
 * 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Test.Modeling;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
#endregion


namespace Avalon.Test.CoreUI.PropertyEngine.ReadOnlyDPModel
{
    #region Custom Framework Element with Custom DPs
    /// <summary>
    /// A custom FrameworkElement where the inheitanceBehavior can be modified and set as desired.
    /// </summary>
    public class CustomFE : FrameworkElement
    {
        /// <summary>
        /// Constructor for custom FrameworkElement. We can set the inheritanceBehavior property from here.
        /// </summary>
        public CustomFE()
        {

        }

        /// <summary>
        /// Constructor for custom FrameworkElement. We can set the inheritanceBehavior property from here.
        /// </summary>
        public CustomFE(string action)
        {
            if (action == "OverrideMetadata")
            {
                string regException = "";
                string attException = "";
                try
                {
                    RegularProperty.OverrideMetadata(typeof(CustomFE), new PropertyMetadata("Overriden Regular property Value"));
                }
                catch (InvalidOperationException ex)
                {
                    regException = ex.Message;
                }

                if (regException == "" || regException == null)
                {
                    throw new Microsoft.Test.TestValidationException("No Exception thrown when attempting to override DP metadata in inherits FE");
                }
                else
                {
                    CoreLogger.LogStatus("Got Expected Exception: " + regException, ConsoleColor.Green);
                }


                try
                {
                    AttachedProperty.OverrideMetadata(typeof(CustomFE), new FrameworkPropertyMetadata("Overriden Attached property Value"));
                }
                catch (InvalidOperationException ex)
                {
                    attException = ex.Message;
                }

                if (attException == "" || attException == null)
                {
                    throw new Microsoft.Test.TestValidationException("No Exception thrown when attempting to override DP metadata in inherits FE");
                }
                else
                {
                    CoreLogger.LogStatus("Got Expected Exception: " + attException, ConsoleColor.Green);
                }
            }
        }

        /// <summary>
        /// The Stretch property determines how the shape may be stretched to accommodate shape size
        /// </summary>
        public UIElementCollection Children
        {
            get
            {
                if (_Children == null)
                {
                    _Children = new UIElementCollection(this, this);
                }

                return _Children;
            }
        }

        private UIElementCollection _Children = null;

        /// <summary>
        /// The Stretch property determines how the shape may be stretched to accommodate shape size
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                // otherwise, its logical children is its visual children                
                return this.Children.GetEnumerator();
            }
        }

        /// <summary>
        /// Regular Dependency Property Key.
        /// </summary>
        public static readonly DependencyPropertyKey RegularPropertyKey
            = DependencyProperty.RegisterReadOnly(
                "Regular",                  // Property name
                typeof(string),            // Property type
                typeof(CustomFE),
                new FrameworkPropertyMetadata("Default Value", FrameworkPropertyMetadataOptions.Inherits)
            );

        /// <summary>
        /// Regular Dependency Property.
        /// </summary>
        public static readonly DependencyProperty RegularProperty = RegularPropertyKey.DependencyProperty;

        /// <summary>
        /// Attached Dependency Property Key.
        /// </summary>
        public static readonly DependencyPropertyKey AttachedPropertyKey
            = DependencyProperty.RegisterAttachedReadOnly(
                "Attached",                  // Property name
                typeof(string),            // Property type
                typeof(CustomFE),
                new FrameworkPropertyMetadata("Default Value2", FrameworkPropertyMetadataOptions.Inherits)
            );

        /// <summary>
        /// Attached Dependency Property.
        /// </summary>
        public static readonly DependencyProperty AttachedProperty = AttachedPropertyKey.DependencyProperty;

        /// <summary>
        /// The Stretch property determines how the shape may be stretched to accommodate shape size
        /// </summary>
        public string Regular
        {
            get { return (string)GetValue(RegularProperty); }
            set { SetValue(RegularProperty, value); }
        }

        /// <summary>
        /// The Stretch property determines how the shape may be stretched to accommodate shape size
        /// </summary>
        public string Attached
        {
            get { return (string)GetValue(AttachedProperty); }
            set { SetValue(AttachedProperty, value); }
        }
    }


    #endregion

    #region Custom Framework Element that inherits Custom DPs from the first Custom Framework Elements

    /// <summary>
    /// A custom FrameworkElement which inherits / or attempts to the inherit readonly properties.
    /// </summary>
    public class InheritsCustomFE : CustomFE
    {
        /// <summary>
        /// Constructor for custom FrameworkElement
        /// </summary>
        public InheritsCustomFE(string action)
        {

        }
    }

    #endregion
}
