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

namespace Avalon.Test.CoreUI.Serialization
{
    #region Class MyUIElementWithReadOnlyProperties
    /// <summary>
    /// This class defines custom class with various readonly properties
    /// </summary>
    public class MyUIElementWithReadOnlyProperties : UIElement
    {
        #region Constructor


        /// <summary>
        /// 
        /// </summary>
        public MyUIElementWithReadOnlyProperties() : base()
        {
            SetValue(s_myReadOnlyDPPropertyKey, "ReadOnly DP");
            SetValue(s_myContentDPPropertyKey, "Content ReadOnly DP");
            SetValue(s_myShouldDPPropertyKey, "should serialized ReadOnly DP");
            SetValue(s_myShouldNotDPPropertyKey, "should not serialized ReadOnly DP");
            SetValue(s_myClrDPPropertyKey, "DP with clr accessor");
        }
        #endregion Constructor
        #region readonly Dependency Property     
        #region MyReadOnlyDP

        private static DependencyPropertyKey s_myReadOnlyDPPropertyKey = 
                                                  DependencyProperty.RegisterAttachedReadOnly(
                                                  "MyReadOnlyDP", 
                                                  typeof(String), 
                                                  typeof(MyUIElementWithReadOnlyProperties), 
                                                  new FrameworkPropertyMetadata("0.0"));
        /// <summary>
        /// DependencyProperty for the attached MyReadOnlyDP property.
        /// </summary>
        public static DependencyProperty MyReadOnlyDPProperty = s_myReadOnlyDPPropertyKey.DependencyProperty;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static String GetMyReadOnlyDP(DependencyObject e)
        {
            return (String)e.GetValue(MyReadOnlyDPProperty);
        }

        #endregion MyReadOnlyDP

        #region Readonly MyContentDP
        private static DependencyPropertyKey s_myContentDPPropertyKey =
                         DependencyProperty.RegisterAttachedReadOnly("MyContentDP",
                         typeof(String), typeof(MyUIElementWithReadOnlyProperties),
                         new FrameworkPropertyMetadata("0.0"));

        /// <summary>
        /// DependencyProperty for the attached MyContentDP property.
        /// </summary>
        public static DependencyProperty MyContentDPProperty = s_myContentDPPropertyKey.DependencyProperty;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public static String GetMyContentDP(DependencyObject e)
        {
            return (String)e.GetValue(MyContentDPProperty);
        }

        #endregion Readonly ContentDP

        #region Readonly MyShouldDP

        private static DependencyPropertyKey  s_myShouldDPPropertyKey =
                            DependencyProperty.RegisterAttachedReadOnly("MyShouldDP",
                            typeof(String),
                            typeof(MyUIElementWithReadOnlyProperties),
                            new FrameworkPropertyMetadata("0.0"));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MyShouldDPProperty =
            s_myShouldDPPropertyKey.DependencyProperty;



        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static String GetMyShouldDP(DependencyObject e)
        {
            return (String)(e.GetValue(MyShouldDPProperty));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool ShouldSerializeMyShouldDP(DependencyObject e)
        {
            return true;
        }
        #endregion Readonly ShouldSerializeDP
        #region Readonly MyShouldNotDP
        private static  DependencyPropertyKey s_myShouldNotDPPropertyKey =
                            DependencyProperty.RegisterAttachedReadOnly("MyShouldNotDP",
                                          typeof(String),
                                          typeof(MyUIElementWithReadOnlyProperties),
                                          new FrameworkPropertyMetadata("0.0"));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MyShouldNotDPProperty =
            s_myShouldNotDPPropertyKey.DependencyProperty;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static String GetMyShouldNotDP(DependencyObject e)
        {
            return (String)(e.GetValue(MyShouldNotDPProperty));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool ShouldSerializeMyShouldNotDP(DependencyObject e)
        {
            return false;
        }
        #endregion Readonly ShouldNotDP

        #endregion readonly Dependency Property  

        #region Clr readonly Property    
        #region readonly MyReadOnlyProperty

        private string _myReadOnlyProperty = "readonly";
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string MyReadOnlyProperty
        {
            get
            {
                return _myReadOnlyProperty;
            }
        }
        #endregion just ReadOnly

        #region Readonly MyContentClrProperty

        private string _myContentClrProperty = "ContentReadonly";
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public string MyContentClrProperty
        {
            get
            {
                return _myContentClrProperty;
            }
        }
        #endregion Readonly ContentClrProperty

        #region Readonly MyShouldSerializeClrProperty
        private String _myShouldSerialized= "Red";
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public String MyShouldSerialized
        {
            get
            {
                return _myShouldSerialized;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeMyShouldSerialized()
        {
            return true;
        }
        #endregion Readonly ShouldSerializeClrProperty
        #region Readonly MyShouldNotSerializeClrProperty
        private String _myShouldNotSerialized = "Blue";

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public String MyShouldNotSerialized
        {
            get
            {
                return _myShouldNotSerialized;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeMyShouldNotSerialized()
        {
            return false;
        }
        #endregion Readonly ShouldNotSerializeClrProperty
        #endregion Clr readonly Property 
        #region DP with clr accessor
        private static DependencyPropertyKey s_myClrDPPropertyKey =
                                         DependencyProperty.RegisterAttachedReadOnly(
                                         "MyClrDP",
                                         typeof(String),
                                         typeof(MyUIElementWithReadOnlyProperties),
                                         new FrameworkPropertyMetadata("0.0"));

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty MyClrDPProperty =
                                         s_myClrDPPropertyKey.DependencyProperty;
        
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public String MyClrDP
        {
            get
            {
                return (String)GetValue(MyClrDPProperty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeMyClrDP()
        {
            return true;
        }
        #endregion DP with clr accessor
    }
    #endregion Class MyUIElementWithReadOnlyProperties
}
