// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: PropertyChanged Callback verification Test
 * 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/

#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Test.Modeling;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
#endregion

namespace Avalon.Test.CoreUI.PropertyEngine.PropertyChanged
{
    #region Customized DependencyObject
    /// <summary>
    /// Custom DO used for DependencyPropertyChangedEventArgs test.
    /// </summary>
    public class CustomDO : DependencyObject
    {
        #region DependencyProperties
        /// <summary>
        /// Generic DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty FirstProperty =
            DependencyProperty.Register(
                        "First",
                        typeof(object),
                        typeof(CustomDO),
                        new PropertyMetadata("FirstDefault",
                                             new PropertyChangedCallback(OnGenericPropertyChanged),
                                             new CoerceValueCallback(CoerceFirst))
            );

        /// <summary>
        /// DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty RegularFalseProperty =
            DependencyProperty.Register(
                        "RegularFalse",
                        typeof(object),
                        typeof(CustomDO),
                        new PropertyMetadata("RegularFalse Default",
                                             new PropertyChangedCallback(OnGenericPropertyChanged),
                                             new CoerceValueCallback(CoerceRegularFalse))
            );

        /// <summary>
        /// DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty AttachedFalseProperty =
            DependencyProperty.RegisterAttached(
                        "AttachedFalse",
                        typeof(object),
                        typeof(CustomDO),
                        new PropertyMetadata("AttachedFalse Default",
                                             new PropertyChangedCallback(OnGenericPropertyChanged),
                                             new CoerceValueCallback(CoerceAttachedFalse))
            );

        /// <summary>
        /// DependencyProperty.
        /// </summary>
        public static readonly DependencyPropertyKey RegularTruePropertyKey =
            DependencyProperty.RegisterReadOnly(
                        "RegularTrue",
                        typeof(object),
                        typeof(CustomDO),
                        new PropertyMetadata("RegularTrue Default",
                                             new PropertyChangedCallback(OnGenericPropertyChanged),
                                             new CoerceValueCallback(CoerceRegularTrue))
            );
        /// <summary>
        /// DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty RegularTrueProperty = RegularTruePropertyKey.DependencyProperty;

        /// <summary>
        /// DependencyProperty.
        /// </summary>
        public static readonly DependencyPropertyKey AttachedTruePropertyKey =
            DependencyProperty.RegisterAttachedReadOnly(
                        "AttachedTrue",
                        typeof(object),
                        typeof(CustomDO),
                        new PropertyMetadata("AttachedTrue Default",
                                             new PropertyChangedCallback(OnGenericPropertyChanged),
                                             new CoerceValueCallback(CoerceAttachedTrue))
            );
        /// <summary>
        /// DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty AttachedTrueProperty = AttachedTruePropertyKey.DependencyProperty;


        #endregion

        /// <summary>
        /// Constructor for Custom DO
        /// </summary>
        public CustomDO(State inParameters)
        {
            // Assign parameter values to variables.
            //RegistrationType = inParameters["RegisterationType"].ToString();
            //ReadOnly = inParameters["ReadOnly"].ToString();
            //PropertyType = inParameters["PropertyType"].ToString();

            //MetaData = inParameters["MetaData"].ToString();
            //TargetType = inParameters["TargetType"].ToString();

            PropertyMetadata metadata = null;

            if (inParameters["MetaData"].ToString() == "None")
            {
                // Do Nothing.
                metadata = new PropertyMetadata();
            }
            else if (inParameters["MetaData"].ToString() == "Regular")
            {
                metadata = new PropertyMetadata("REGULAR",
                                                new PropertyChangedCallback(OnGenericPropertyChanged),
                                                new CoerceValueCallback(CoerceFirst));
            }
            else if (inParameters["MetaData"].ToString() == "Coerced")
            {
                metadata = new PropertyMetadata("COERCED",
                                                new PropertyChangedCallback(OnGenericPropertyChanged),
                                                new CoerceValueCallback(CoerceFirst));
            }
            else if (inParameters["MetaData"].ToString() == "Derived")
            {
                metadata = new PropertyMetadata("DERIVED",
                                                new PropertyChangedCallback(OnGenericPropertyChanged),
                                                new CoerceValueCallback(CoerceFirst));
            }

            //FirstProperty.OverrideMetadata(typeof(CustomDO), metadata);
        }

        /// <summary>
        /// Clr accessor to the First DP
        /// </summary>
        public object First
        {
            get { return (GetValue(FirstProperty)); }
            set { SetValue(FirstProperty, value); }
        }

        /// <summary>
        /// Clr accessor to the AttachedFalse DP
        /// </summary>
        public object AttachedFalse
        {
            get { return (GetValue(AttachedFalseProperty)); }
            set { SetValue(AttachedFalseProperty, value); }
        }

        /// <summary>
        /// Clr accessor to the RegularFalse DP
        /// </summary>
        public object RegularFalse
        {
            get { return (GetValue(RegularFalseProperty)); }
            set { SetValue(RegularFalseProperty, value); }
        }

        /// <summary>
        /// Clr accessor to the AttachedTrue DP
        /// </summary>
        public object AttachedTrue
        {
            get { return (GetValue(AttachedTrueProperty)); }
        }

        /// <summary>
        /// Clr accessor to the RegularTrue DP
        /// </summary>
        public object RegularTrue
        {
            get { return (GetValue(RegularTrueProperty)); }
        }

        // Property changed metadata callback.
        private static void OnGenericPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == FirstProperty)
            {
                FirstEventArgsList.Add(e);
            }
            else if (e.Property == AttachedFalseProperty)
            {
                AttachedFalseEventArgsList.Add(e);
            }
            else if (e.Property == RegularFalseProperty)
            {
                RegularFalseEventArgsList.Add(e);
            }
            else if (e.Property == AttachedTrueProperty)
            {
                AttachedTrueEventArgsList.Add(e);
            }
            else if (e.Property == RegularTrueProperty)
            {
                RegularTrueEventArgsList.Add(e);
            }
        }

        /// <summary>
        /// Notification that a specified property has been changed.
        /// </summary>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.ShouldCallBase)
            {
                base.OnPropertyChanged(e);
            }

            if (e.Property == FirstProperty)
            {
                FirstEventArgsList.Add(e);
            }
            else if (e.Property == AttachedFalseProperty)
            {
                AttachedFalseEventArgsList.Add(e);
            }
            else if (e.Property == RegularFalseProperty)
            {
                RegularFalseEventArgsList.Add(e);
            }
            else if (e.Property == AttachedTrueProperty)
            {
                AttachedTrueEventArgsList.Add(e);
            }
            else if (e.Property == RegularTrueProperty)
            {
                RegularTrueEventArgsList.Add(e);
            }
            else
            {
                CustomDO.OtherEventArgsList.Add(e);
            }
        }

        // Coerce Value CallBack
        private static object CoerceFirst(DependencyObject o, object value)
        {
            // Record an Event.
            CoerceFirstList.Add("called");

            return value;
        }

        // Coerce Value CallBack
        private static object CoerceRegularFalse(DependencyObject o, object value)
        {
            // Record an Event.
            CoerceRegularFalseList.Add("called");

            return value;
        }

        // Coerce Value CallBack
        private static object CoerceAttachedFalse(DependencyObject o, object value)
        {
            // Record an Event.
            CoerceAttachedFalseList.Add("called");

            return value;
        }

        // Coerce Value CallBack
        private static object CoerceRegularTrue(DependencyObject o, object value)
        {
            // Record an Event.
            CoerceRegularTrueList.Add("called");

            return value;
        }

        // Coerce Value CallBack
        private static object CoerceAttachedTrue(DependencyObject o, object value)
        {
            // Record an Event.
            CoerceAttachedTrueList.Add("called");

            return value;
        }

        #region DependencyPropertyChangedEvent Lists
        /// <summary>
        /// Record of property changed virtual and metadata callbacks.
        /// </summary>
        public static List<DependencyPropertyChangedEventArgs> FirstEventArgsList = new List<DependencyPropertyChangedEventArgs>();
        /// <summary>
        /// Record of coerce value callbacks.
        /// </summary>
        public static List<string> CoerceFirstList = new List<string>();

        /// <summary>
        /// Record of property changed virtual and metadata callbacks.
        /// </summary>
        public static List<DependencyPropertyChangedEventArgs> RegularFalseEventArgsList = new List<DependencyPropertyChangedEventArgs>();
        /// <summary>
        /// Record of coerce value callbacks.
        /// </summary>
        public static List<string> CoerceRegularFalseList = new List<string>();

        /// <summary>
        /// Record of property changed virtual and metadata callbacks.
        /// </summary>
        public static List<DependencyPropertyChangedEventArgs> AttachedFalseEventArgsList = new List<DependencyPropertyChangedEventArgs>();
        /// <summary>
        /// Record of coerce value callbacks.
        /// </summary>
        public static List<string> CoerceAttachedFalseList = new List<string>();

        /// <summary>
        /// Record of property changed virtual and metadata callbacks.
        /// </summary>
        public static List<DependencyPropertyChangedEventArgs> RegularTrueEventArgsList = new List<DependencyPropertyChangedEventArgs>();
        /// <summary>
        /// Record of coerce value callbacks.
        /// </summary>
        public static List<string> CoerceRegularTrueList = new List<string>();

        /// <summary>
        /// Record of property changed virtual and metadata callbacks.
        /// </summary>
        public static List<DependencyPropertyChangedEventArgs> AttachedTrueEventArgsList = new List<DependencyPropertyChangedEventArgs>();
        /// <summary>
        /// Record of coerce value callbacks.
        /// </summary>
        public static List<string> CoerceAttachedTrueList = new List<string>();

        /// <summary>
        /// Record of property changed virtual and metadata callbacks.
        /// </summary>
        public static List<DependencyPropertyChangedEventArgs> OtherEventArgsList = new List<DependencyPropertyChangedEventArgs>();

        #endregion

        /// <summary>
        /// Controls whether or not the virtual calls its base.
        /// </summary>
        public bool ShouldCallBase = true;
    }
    #endregion
}
