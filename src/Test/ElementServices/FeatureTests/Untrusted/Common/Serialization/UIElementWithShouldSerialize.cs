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
using System.Windows.Controls;

namespace Avalon.Test.CoreUI.Serialization
{
    #region Class UIElementWithShouldSerialize
    /// <summary>
    /// This class defines custom clr class with various custom properties
    /// </summary>
    public class UIElementWithShouldSerialize : UIElement
    {
        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        public UIElementWithShouldSerialize() : base()
        {
            SetValue(HiddenShouldDPProperty, "ShouldNotBeSerialized");
        }
        #endregion Constructor
        #region Dependency Property     


        #region ShouldDP
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public bool ShouldSerializeShouldDP(DependencyObject o)
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public bool ShouldSerializeShouldDP(DependencyObject o, XamlDesignerSerializationManager manager)
        {
            return true;
        }


        /// <summary>
        /// DependencyProperty for the attached CustomDP property.
        /// </summary>
        public static DependencyProperty ShouldDPProperty = DependencyProperty.RegisterAttached("ShouldDP", typeof(String), typeof(UIElementWithShouldSerialize), new FrameworkPropertyMetadata("ShouldBeSerialized"));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static String GetShouldDP(DependencyObject e)
        {
            return (String)e.GetValue(ShouldDPProperty);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetShouldDP(DependencyObject e, String myProperty)
        {
            e.SetValue(ShouldDPProperty, myProperty);
        }
        #endregion ShouldDP


        #region ShouldNotDP
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool ShouldSerializeShouldNotDP(DependencyObject o)
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static bool ShouldSerializeShouldNotDP(DependencyObject o, XamlDesignerSerializationManager manager)
        {
            return true;
        }


        /// <summary>
        /// DependencyProperty for the attached CustomDP property.
        /// </summary>
        public static DependencyProperty ShouldNotDPProperty = DependencyProperty.RegisterAttached("ShouldNotDP", typeof(String), typeof(UIElementWithShouldSerialize), new FrameworkPropertyMetadata("ShouldNotBeSerialized"));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static String GetShouldNotDP(DependencyObject e)
        {
            return (String)e.GetValue(ShouldNotDPProperty);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetShouldNotDP(DependencyObject e, String myProperty)
        {
            e.SetValue(ShouldNotDPProperty, myProperty);
        }
        #endregion ShouldNotDP


        #region ShouldDPNoOtherShould

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static bool ShouldSerializeShouldDPNoOtherShould(DependencyObject o, XamlDesignerSerializationManager manager)
        {
            return true;
        }


        /// <summary>
        /// DependencyProperty for the attached CustomDPNoOtherShould property.
        /// </summary>
        public static DependencyProperty ShouldDPNoOtherShouldProperty = DependencyProperty.RegisterAttached("ShouldDPNoOtherShould", typeof(String), typeof(UIElementWithShouldSerialize), new FrameworkPropertyMetadata("ShouldBeSerialized"));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static String GetShouldDPNoOtherShould(DependencyObject e)
        {
            return (String)e.GetValue(ShouldDPNoOtherShouldProperty);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetShouldDPNoOtherShould(DependencyObject e, String myProperty)
        {
            e.SetValue(ShouldDPNoOtherShouldProperty, myProperty);
        }
        #endregion ShouldDPNoOtherShould


        #region ShouldNotDPNoOtherShould

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static bool ShouldSerializeShouldNotDPNoOtherShould(DependencyObject o, XamlDesignerSerializationManager manager)
        {
            return false;
        }


        /// <summary>
        /// DependencyProperty for the attached CustomDPNoOtherShould property.
        /// </summary>
        public static DependencyProperty ShouldNotDPNoOtherShouldProperty = DependencyProperty.RegisterAttached("ShouldNotDPNoOtherShould", typeof(String), typeof(UIElementWithShouldSerialize), new FrameworkPropertyMetadata("ShouldNotBeSerialized"));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static String GetShouldNotDPNoOtherShould(DependencyObject e)
        {
            return (String)e.GetValue(ShouldNotDPNoOtherShouldProperty);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetShouldNotDPNoOtherShould(DependencyObject e, String myProperty)
        {
            e.SetValue(ShouldNotDPNoOtherShouldProperty, myProperty);
        }
        #endregion ShouldNotDPNoOtherShould

        #region HiddenShouldDP
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool ShouldSerializeHiddenShouldDP(DependencyObject o)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static bool ShouldSerializeHiddenShouldDP(DependencyObject o, XamlDesignerSerializationManager manager)
        {
            return false;
        }


        /// <summary>
        /// DependencyProperty for the attached CustomDP property.
        /// </summary>
        public static DependencyProperty HiddenShouldDPProperty = DependencyProperty.RegisterAttached("HiddenShouldDP", typeof(String), typeof(UIElementWithShouldSerialize), new FrameworkPropertyMetadata("ShouldNotBeSerialized"));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static String GetHiddenShouldDP(DependencyObject e)
        {
            return (String)e.GetValue(HiddenShouldDPProperty);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetHiddenShouldDP(DependencyObject e, String myProperty)
        {
            e.SetValue(HiddenShouldDPProperty, myProperty);
        }
        #endregion HiddenShouldDP


        #endregion Dependency Property  

        #region Clr Property    

        #region ShouldClrProperty
        private string _ShouldClrProperty = "ShouldBeSerialized";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeShouldClrProperty()
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public bool ShouldSerializeShouldClrProperty(XamlDesignerSerializationManager manager)
        {
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string ShouldClrProperty
        {
            get
            {
                return _ShouldClrProperty;
            }
            set
            {
                _ShouldClrProperty = value;
            }
        }
        #endregion ShouldClrProperty

        #region ShouldNotClrProperty
        private string _ShouldNotClrProperty = "ShouldNotBeSerialized";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeShouldNotClrProperty()
        {
            return true;
        }
        private bool ShouldSerializeShouldNotClrProperty(XamlDesignerSerializationManager manager)
        {
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string ShouldNotClrProperty
        {
            get
            {
                return _ShouldNotClrProperty;
            }
            set
            {
                _ShouldNotClrProperty = value;
            }
        }
        #endregion ShouldNotClrProperty


        #region ShouldClrPropertyNoOtherShouldMethod
        private string _ShouldClrPropertyNoOtherShouldMethod = "ShouldBeSerialized";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public bool ShouldSerializeShouldClrPropertyNoOtherShouldMethod(XamlDesignerSerializationManager manager)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string ShouldClrPropertyNoOtherShouldMethod
        {
            get
            {
                return _ShouldClrPropertyNoOtherShouldMethod;
            }
            set
            {
                _ShouldClrPropertyNoOtherShouldMethod = value;
            }
        }
        #endregion ShouldClrPropertyNoOtherShouldMethod

        #region ShouldNotClrPropertyNoOtherShouldMethod
        private string _ShouldNotClrPropertyNoOtherShouldMethod = "ShouldNotBeSerialized";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public bool ShouldSerializeShouldNotClrPropertyNoOtherShouldMethod(XamlDesignerSerializationManager manager)
        {
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string ShouldNotClrPropertyNoOtherShouldMethod
        {
            get
            {
                return _ShouldNotClrPropertyNoOtherShouldMethod;
            }
            set
            {
                _ShouldNotClrPropertyNoOtherShouldMethod = value;
            }
        }
        #endregion ShouldNotClrPropertyNoOtherShouldMethod


        #region ShouldClrReadOnlyProperty
        private string _ShouldClrReadOnlyProperty = "ShouldBeSerialized";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeShouldClrReadOnlyProperty()
        {
            return true;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public bool ShouldSerializeShouldClrReadOnlyProperty(XamlDesignerSerializationManager manager)
        {
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string ShouldClrReadOnlyProperty
        {
            get
            {
                return _ShouldClrReadOnlyProperty;
            }
        }
        #endregion ShouldClrReadOnlyProperty

        #region ShouldNotClrReadOnlyProperty
        private string _ShouldNotClrReadOnlyProperty = "ShouldNotBeSerialized";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeShouldNotClrReadOnlyProperty()
        {
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public bool ShouldSerializeShouldNotClrReadOnlyProperty(XamlDesignerSerializationManager manager)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string ShouldNotClrReadOnlyProperty
        {
            get
            {
                return _ShouldNotClrReadOnlyProperty;
            }
        }
        #endregion ShouldNotClrReadOnlyProperty


        #region ShouldClrReadOnlyPropertyNoOtherShouldMethod
        private string _ShouldClrReadOnlyPropertyNoOtherShouldMethod = "ShouldBeSerialized";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public bool ShouldSerializeShouldClrReadOnlyPropertyNoOtherShouldMethod(XamlDesignerSerializationManager manager)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string ShouldClrReadOnlyPropertyNoOtherShouldMethod
        {
            get
            {
                return _ShouldClrReadOnlyPropertyNoOtherShouldMethod;
            }
        }
        #endregion ShouldClrReadOnlyPropertyNoOtherShouldMethod

        #region ShouldNotClrReadOnlyPropertyNoOtherShouldMethod
        private string _ShouldNotClrReadOnlyPropertyNoOtherShouldMethod = "ShouldNotBeSerialized";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public bool ShouldSerializeShouldNotClrReadOnlyPropertyNoOtherShouldMethod(XamlDesignerSerializationManager manager)
        {
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string ShouldNotClrReadOnlyPropertyNoOtherShouldMethod
        {
            get
            {
                return _ShouldNotClrReadOnlyPropertyNoOtherShouldMethod;
            }
        }
        #endregion ShouldNotClrReadOnlyPropertyNoOtherShouldMethod


        #region ShouldHiddenClrProperty
        private string _ShouldHiddenClrProperty = "ShouldNotBeSerialized";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeShouldHiddenClrProperty()
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public bool ShouldSerializeShouldHiddenClrProperty(XamlDesignerSerializationManager manager)
        {
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ShouldHiddenClrProperty
        {
            get
            {
                return _ShouldHiddenClrProperty;
            }
            set
            {
                _ShouldHiddenClrProperty = value;
            }
        }
        #endregion ShouldHiddenClrProperty

        #region ShouldNotHiddenClrProperty
        private string _ShouldNotHiddenClrProperty = "ShouldNotBeSerialized";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeShouldNotHiddenClrProperty()
        {
            return false;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public bool ShouldSerializeShouldNotHiddenClrProperty(XamlDesignerSerializationManager manager)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ShouldNotHiddenClrProperty
        {
            get
            {
                return _ShouldNotHiddenClrProperty;
            }
            set
            {
                _ShouldNotHiddenClrProperty = value;
            }
        }
        #endregion ShouldNotHiddenClrProperty


        #region ShouldHiddenClrPropertyNoOtherShouldMethod
        private string _ShouldHiddenClrPropertyNoOtherShouldMethod = "ShouldNotBeSerialized";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public bool ShouldSerializeShouldHiddenClrPropertyNoOtherShouldMethod(XamlDesignerSerializationManager manager)
        {
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ShouldHiddenClrPropertyNoOtherShouldMethod
        {
            get
            {
                return _ShouldHiddenClrPropertyNoOtherShouldMethod;
            }
            set
            {
                _ShouldHiddenClrPropertyNoOtherShouldMethod = value;
            }
        }
        #endregion ShouldHiddenClrPropertyNoOtherShouldMethod

        #region ShouldNotHiddenClrPropertyNoOtherShouldMethod
        private string _ShouldNotHiddenClrPropertyNoOtherShouldMethod = "ShouldNotBeSerialized";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public bool ShouldSerializeShouldNotHiddenClrPropertyNoOtherShouldMethod(XamlDesignerSerializationManager manager)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ShouldNotHiddenClrPropertyNoOtherShouldMethod
        {
            get
            {
                return _ShouldNotHiddenClrPropertyNoOtherShouldMethod;
            }
            set
            {
                _ShouldNotHiddenClrPropertyNoOtherShouldMethod = value;
            }
        }
        #endregion ShouldNotHiddenClrPropertyNoOtherShouldMethod


        #endregion Clr Property 


    }
    #endregion Class UIElementWithShouldSerialize
}
