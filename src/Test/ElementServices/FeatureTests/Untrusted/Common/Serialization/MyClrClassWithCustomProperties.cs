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
    #region Class MyClrClassWithCustomProperties
    /// <summary>
    /// This class defines custom clr class with various custom properties
    /// </summary>
    public class MyClrClassWithCustomProperties
    {
		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		public MyClrClassWithCustomProperties()
		{
			_ilistProp = new ArrayList();
		}
		#endregion Constructor

        #region Dependency Property     
        #region MyHiddenDP
        /// <summary>
        /// DependencyProperty for the attached MyHiddenDP property.
        /// </summary>
        public static DependencyProperty MyHiddenDPProperty = 
                                                  DependencyProperty.RegisterAttached(
                                                  "MyHiddenDP", 
                                                  typeof(String), 
                                                  typeof(MyClrClassWithCustomProperties), 
                                                  new FrameworkPropertyMetadata("0.0"));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static String GetMyHiddenDP(DependencyObject e)
        {
            return (String)e.GetValue(MyHiddenDPProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetMyHiddenDP(DependencyObject e, String myProperty)
        {
            e.SetValue(MyHiddenDPProperty, myProperty);
        }
        #endregion HiddenDP

        #region MyContentDP
        /// <summary>
        /// DependencyProperty for the attached MyContentDP property.
        /// </summary>
        public static DependencyProperty MyContentDPProperty = DependencyProperty.RegisterAttached("MyContentDP", typeof(String), typeof(MyClrClassWithCustomProperties), new FrameworkPropertyMetadata("0.0"));

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetMyContentDP(DependencyObject e, String myProperty)
        {
            e.SetValue(MyContentDPProperty, myProperty);
        }
        #endregion ContentDP

        #region MyShouldDP

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MyShouldDPProperty = 
                            DependencyProperty.RegisterAttached("MyShouldDP", 
                            typeof(String), 
                            typeof(MyClrClassWithCustomProperties), 
                            new FrameworkPropertyMetadata("0.0"));

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
        /// <param name="myProperty"></param>
        public static void SetMyShouldDP(DependencyObject e, String myProperty)
        {
            e.SetValue(MyShouldDPProperty, myProperty);
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
        #endregion ShouldSerializeDP
        #region MyShouldNotDP

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MyShouldNotDPProperty = DependencyProperty.RegisterAttached("MyShouldNotDP", typeof(String), typeof(MyClrClassWithCustomProperties));

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
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetMyShouldNotDP(DependencyObject e, String myProperty)
        {
            e.SetValue(MyShouldNotDPProperty, myProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool ShouldSerializeMyShouldNotDP(DependencyObject e)
        {
            return false;
        }
        #endregion ShouldNotDP

        #endregion Dependency Property  

        #region Clr Property    
        #region MyHiddenClrProperty

        private string _myHiddenClrProperty;
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string MyHiddenClrProperty
        {
            get
            {
                return _myHiddenClrProperty;
            }
            set
            {
                _myHiddenClrProperty = value;
            }
        }

        #endregion HiddenClrProperty

        #region MyDefaultClrProperty

        private string _myDefaultClrProperty="DefaultValue";
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [DefaultValue("DefaultValue")]
        public string MyDefaultClrProperty
        {
            get
            {
                return _myDefaultClrProperty;
            }
            set
            {
                _myDefaultClrProperty = value;
            }
        }
        #endregion DefaultClrProperty


        #region MyContentClrProperty

        private string _myContentClrProperty;

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
            set
            {
                _myContentClrProperty = value;
            }
        }
        #endregion ContentClrProperty

        #region MyShouldSerializeClrProperty
        private String _myShouldSerializeClrColor= "Red";
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public String MyShouldSerializeClrColor
        {
            get
            {
                return _myShouldSerializeClrColor;
            }
            set
            {
                _myShouldSerializeClrColor = value;
            }
        }
        private bool ShouldSerializeMyShouleSerializeClrColor()
        {
            return true;
        }
        #endregion ShouldSerializeClrProperty
        #region MyShouldNotSerializeClrProperty
        private String _myShouldNotSerializeClrColor = "Blue";

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public String MyShouldNotSerializeClrColor
        {
            get
            {
                return _myShouldNotSerializeClrColor;
            }
            set
            {
                _myShouldNotSerializeClrColor = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeMyShouldNotSerializeClrColor()
        {
            return false;
        }
        #endregion ShouldNotSerializeClrProperty

        #endregion Clr Property 
		#region ILIST

/// <summary>
/// 
/// </summary>
/// <value></value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ArrayList IListProp
		{
			get { return _ilistProp; }
		}

		private ArrayList _ilistProp;

		#endregion IList
    }
    #endregion Class MyClrClassWithCustomProperties
}
