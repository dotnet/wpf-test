// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// This class defines custom clr class with various custom properties
    /// </summary>
    public class UIElementWithShouldSerialize : UIElement
    {
        /// <summary>
        /// DependencyProperty for the attached CustomDP property.
        /// </summary>
        private static DependencyProperty s_shouldDPProperty = DependencyProperty.RegisterAttached("ShouldDP", typeof(string), typeof(UIElementWithShouldSerialize), new FrameworkPropertyMetadata("ShouldBeSerialized"));

        /// <summary>
        /// DependencyProperty for the attached CustomDP property.
        /// </summary>
        private static DependencyProperty s_shouldNotDPProperty = DependencyProperty.RegisterAttached("ShouldNotDP", typeof(string), typeof(UIElementWithShouldSerialize), new FrameworkPropertyMetadata("ShouldNotBeSerialized"));

        /// <summary>
        /// DependencyProperty for the attached CustomDPNoOtherShould property.
        /// </summary>
        private static DependencyProperty s_shouldDPNoOtherShouldProperty = DependencyProperty.RegisterAttached("ShouldDPNoOtherShould", typeof(string), typeof(UIElementWithShouldSerialize), new FrameworkPropertyMetadata("ShouldBeSerialized"));

        /// <summary>
        /// DependencyProperty for the attached CustomDPNoOtherShould property.
        /// </summary>
        private static DependencyProperty s_shouldNotDPNoOtherShouldProperty = DependencyProperty.RegisterAttached("ShouldNotDPNoOtherShould", typeof(string), typeof(UIElementWithShouldSerialize), new FrameworkPropertyMetadata("ShouldNotBeSerialized"));

        /// <summary>
        /// DependencyProperty for the attached CustomDP property.
        /// </summary>
        private static DependencyProperty s_hiddenShouldDPProperty = DependencyProperty.RegisterAttached("HiddenShouldDP", typeof(string), typeof(UIElementWithShouldSerialize), new FrameworkPropertyMetadata("ShouldNotBeSerialized"));

        /// <summary>
        /// shouldNot Hidden ClrProperty NoOtherShouldMethod
        /// </summary>
        private string _shouldNotHiddenClrPropertyNoOtherShouldMethod = "ShouldNotBeSerialized";

        /// <summary>
        /// shouldHidden ClrProperty NoOtherShouldMethod
        /// </summary>
        private string _shouldHiddenClrPropertyNoOtherShouldMethod = "ShouldNotBeSerialized";

        /// <summary>
        /// should Hidden ClrProperty
        /// </summary>
        private string _shouldHiddenClrProperty = "ShouldNotBeSerialized";

        /// <summary>
        /// shouldNot Hidden ClrProperty
        /// </summary>
        private string _shouldNotHiddenClrProperty = "ShouldNotBeSerialized";

        /// <summary>
        /// shouldNot ClrReadOnly Property NoOtherShouldMethod
        /// </summary>
        private string _shouldNotClrReadOnlyPropertyNoOtherShouldMethod = "ShouldNotBeSerialized";

        /// <summary>
        /// shouldClr ReadOnly Property NoOther ShouldMethod
        /// </summary>
        private string _shouldClrReadOnlyPropertyNoOtherShouldMethod = "ShouldBeSerialized";

        /// <summary>
        /// shouldNot ClrReadOnly Property
        /// </summary>
        private string _shouldNotClrReadOnlyProperty = "ShouldNotBeSerialized";

        /// <summary>
        /// shouldClr ReadOnly Property
        /// </summary>
        private string _shouldClrReadOnlyProperty = "ShouldBeSerialized";

        /// <summary>
        /// shouldClr Property
        /// </summary>
        private string _shouldClrProperty = "ShouldBeSerialized";

        /// <summary>
        /// shouldNot Clr Property
        /// </summary>
        private string _shouldNotClrProperty = "ShouldNotBeSerialized";

        /// <summary>
        /// shouldClrProperty NoOther ShouldMethod
        /// </summary>
        private string _shouldClrPropertyNoOtherShouldMethod = "ShouldBeSerialized";

        /// <summary>
        /// shouldNot ClrProperty NoOther ShouldMethod
        /// </summary>
        private string _shouldNotClrPropertyNoOtherShouldMethod = "ShouldNotBeSerialized";

        /// <summary>
        /// Initializes a new instance of the <see cref="UIElementWithShouldSerialize"/> class.
        /// </summary>
        public UIElementWithShouldSerialize()
            : base()
        {
            SetValue(s_hiddenShouldDPProperty, "ShouldNotBeSerialized");
        }

        /// <summary>
        /// Gets the should not CLR read only property.
        /// </summary>
        /// <value>The should not CLR read only property.</value>
        public string ShouldNotClrReadOnlyProperty
        {
            get
            {
                return _shouldNotClrReadOnlyProperty;
            }
        }

        /// <summary>
        /// Gets the should CLR read only property no other should method.
        /// </summary>
        /// <value>The should CLR read only property no other should method.</value>
        public string ShouldClrReadOnlyPropertyNoOtherShouldMethod
        {
            get
            {
                return _shouldClrReadOnlyPropertyNoOtherShouldMethod;
            }
        }

        /// <summary>
        /// Gets or sets the should hidden CLR property.
        /// </summary>
        /// <value>The should hidden CLR property.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ShouldHiddenClrProperty
        {
            get
            {
                return _shouldHiddenClrProperty;
            }

            set
            {
                _shouldHiddenClrProperty = value;
            }
        }

        /// <summary>
        /// Gets or sets the should not hidden CLR property.
        /// </summary>
        /// <value>The should not hidden CLR property.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ShouldNotHiddenClrProperty
        {
            get
            {
                return _shouldNotHiddenClrProperty;
            }

            set
            {
                _shouldNotHiddenClrProperty = value;
            }
        }

        /// <summary>
        /// Gets or sets the should hidden CLR property no other should method.
        /// </summary>
        /// <value>The should hidden CLR property no other should method.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ShouldHiddenClrPropertyNoOtherShouldMethod
        {
            get
            {
                return _shouldHiddenClrPropertyNoOtherShouldMethod;
            }

            set
            {
                _shouldHiddenClrPropertyNoOtherShouldMethod = value;
            }
        }

        /// <summary>
        /// Gets or sets the should not hidden CLR property no other should method.
        /// </summary>
        /// <value>The should not hidden CLR property no other should method.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ShouldNotHiddenClrPropertyNoOtherShouldMethod
        {
            get
            {
                return _shouldNotHiddenClrPropertyNoOtherShouldMethod;
            }

            set
            {
                _shouldNotHiddenClrPropertyNoOtherShouldMethod = value;
            }
        }

        /// <summary>
        /// Gets or sets the should CLR property.
        /// </summary>
        /// <value>The should CLR property.</value>
        public string ShouldClrProperty
        {
            get
            {
                return _shouldClrProperty;
            }

            set
            {
                _shouldClrProperty = value;
            }
        }

        /// <summary>
        /// Gets or sets the should not CLR property.
        /// </summary>
        /// <value>The should not CLR property.</value>
        public string ShouldNotClrProperty
        {
            get
            {
                return _shouldNotClrProperty;
            }

            set
            {
                _shouldNotClrProperty = value;
            }
        }

        /// <summary>
        /// Gets the should not CLR read only property no other should method.
        /// </summary>
        /// <value>The should not CLR read only property no other should method.</value>
        public string ShouldNotClrReadOnlyPropertyNoOtherShouldMethod
        {
            get
            {
                return _shouldNotClrReadOnlyPropertyNoOtherShouldMethod;
            }
        }

        /// <summary>
        /// Gets the should CLR read only property.
        /// </summary>
        /// <value>The should CLR read only property.</value>
        public string ShouldClrReadOnlyProperty
        {
            get
            {
                return _shouldClrReadOnlyProperty;
            }
        }

        /// <summary>
        /// Gets or sets the should CLR property no other should method.
        /// </summary>
        /// <value>The should CLR property no other should method.</value>
        public string ShouldClrPropertyNoOtherShouldMethod
        {
            get
            {
                return _shouldClrPropertyNoOtherShouldMethod;
            }

            set
            {
                _shouldClrPropertyNoOtherShouldMethod = value;
            }
        }

        /// <summary>
        /// Gets or sets the should not CLR property no other should method.
        /// </summary>
        /// <value>The should not CLR property no other should method.</value>
        public string ShouldNotClrPropertyNoOtherShouldMethod
        {
            get
            {
                return _shouldNotClrPropertyNoOtherShouldMethod;
            }

            set
            {
                _shouldNotClrPropertyNoOtherShouldMethod = value;
            }
        }

        /// <summary>
        /// Gets the should DP.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>bool value</returns>
        public static string GetShouldDP(DependencyObject obj)
        {
            return (string)obj.GetValue(s_shouldDPProperty);
        }

        /// <summary>
        /// Sets the should DP.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="myProperty">My property.</param>
        public static void SetShouldDP(DependencyObject obj, string myProperty)
        {
            obj.SetValue(s_shouldDPProperty, myProperty);
        }

        /// <summary>
        /// ShouldSerialize ShouldDP
        /// </summary>
        /// <param name="obj">Dependency Object</param>
        /// <returns>bool value</returns>
        public static bool ShouldSerializeShouldDP(DependencyObject obj)
        {
            return false;
        }

        /// <summary>
        /// Shoulds the serialize should DP.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="manager">The manager.</param>
        /// <returns>bool value</returns>
        public static bool ShouldSerializeShouldDP(DependencyObject obj, XamlDesignerSerializationManager manager)
        {
            return true;
        }

        /// <summary>
        /// Shoulds the serialize should not DP.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>bool value</returns>
        public static bool ShouldSerializeShouldNotDP(DependencyObject obj)
        {
            return false;
        }

        /// <summary>
        /// Shoulds the serialize should not DP.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="manager">The manager.</param>
        /// <returns>bool value</returns>
        public static bool ShouldSerializeShouldNotDP(DependencyObject obj, XamlDesignerSerializationManager manager)
        {
            return true;
        }

        /// <summary>
        /// Gets the should not DP.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>bool value</returns>
        public static string GetShouldNotDP(DependencyObject obj)
        {
            return (string)obj.GetValue(s_shouldNotDPProperty);
        }

        /// <summary>
        /// Sets the should not DP.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="myProperty">My property.</param>
        public static void SetShouldNotDP(DependencyObject obj, string myProperty)
        {
            obj.SetValue(s_shouldNotDPProperty, myProperty);
        }

        /// <summary>
        /// Shoulds the serialize should DP no other should.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="manager">The manager.</param>
        /// <returns>bool value</returns>
        public static bool ShouldSerializeShouldDPNoOtherShould(DependencyObject obj, XamlDesignerSerializationManager manager)
        {
            return true;
        }

        /// <summary>
        /// Gets the should DP no other should.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>bool value</returns>
        public static string GetShouldDPNoOtherShould(DependencyObject obj)
        {
            return (string)obj.GetValue(s_shouldDPNoOtherShouldProperty);
        }

        /// <summary>
        /// Sets the should DP no other should.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="myProperty">My property.</param>
        public static void SetShouldDPNoOtherShould(DependencyObject obj, string myProperty)
        {
            obj.SetValue(s_shouldDPNoOtherShouldProperty, myProperty);
        }

        /// <summary>
        /// Shoulds the serialize should not DP no other should.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="manager">The manager.</param>
        /// <returns>bool value</returns>
        public static bool ShouldSerializeShouldNotDPNoOtherShould(DependencyObject obj, XamlDesignerSerializationManager manager)
        {
            return false;
        }

        /// <summary>
        /// Gets the should not DP no other should.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>bool value</returns>
        public static string GetShouldNotDPNoOtherShould(DependencyObject obj)
        {
            return (string)obj.GetValue(s_shouldNotDPNoOtherShouldProperty);
        }

        /// <summary>
        /// Sets the should not DP no other should.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="myProperty">My property.</param>
        public static void SetShouldNotDPNoOtherShould(DependencyObject obj, string myProperty)
        {
            obj.SetValue(s_shouldNotDPNoOtherShouldProperty, myProperty);
        }

        /// <summary>
        /// Shoulds the serialize hidden should DP.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>bool value</returns>
        public static bool ShouldSerializeHiddenShouldDP(DependencyObject obj)
        {
            return true;
        }

        /// <summary>
        /// Shoulds the serialize hidden should DP.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="manager">The manager.</param>
        /// <returns>bool value</returns>
        public static bool ShouldSerializeHiddenShouldDP(DependencyObject obj, XamlDesignerSerializationManager manager)
        {
            return false;
        }

        /// <summary>
        /// Gets the hidden should DP.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>bool value</returns>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static string GetHiddenShouldDP(DependencyObject obj)
        {
            return (string)obj.GetValue(s_hiddenShouldDPProperty);
        }

        /// <summary>
        /// Sets the hidden should DP.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="myProperty">My property.</param>
        public static void SetHiddenShouldDP(DependencyObject obj, string myProperty)
        {
            obj.SetValue(s_hiddenShouldDPProperty, myProperty);
        }

        /// <summary>
        /// Serialize should CLR property.
        /// </summary>
        /// <returns>bool value</returns>
        public bool ShouldSerializeShouldClrProperty()
        {
            return true;
        }

        /// <summary>
        /// Shoulds the serialize should CLR property.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <returns>bool value</returns>
        public bool ShouldSerializeShouldClrProperty(XamlDesignerSerializationManager manager)
        {
            return false;
        }

        /// <summary>
        /// Shoulds the serialize should not CLR property.
        /// </summary>
        /// <returns>bool value</returns>
        public bool ShouldSerializeShouldNotClrProperty()
        {
            return true;
        }

        /// <summary>
        /// Shoulds the serialize should CLR property no other should method.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <returns>bool value</returns>
        public bool ShouldSerializeShouldClrPropertyNoOtherShouldMethod(XamlDesignerSerializationManager manager)
        {
            return true;
        }

        /// <summary>
        /// Shoulds the serialize should not CLR property no other should method.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <returns>bool value</returns>
        public bool ShouldSerializeShouldNotClrPropertyNoOtherShouldMethod(XamlDesignerSerializationManager manager)
        {
            return false;
        }

        /// <summary>
        /// Shoulds the serialize should CLR read only property.
        /// </summary>
        /// <returns>bool value</returns>
        public bool ShouldSerializeShouldClrReadOnlyProperty()
        {
            return true;
        }

        /// <summary>
        /// Shoulds the serialize should CLR read only property.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <returns>bool value</returns>
        public bool ShouldSerializeShouldClrReadOnlyProperty(XamlDesignerSerializationManager manager)
        {
            return false;
        }

        /// <summary>
        /// Shoulds the serialize should not CLR read only property.
        /// </summary>
        /// <returns>bool value</returns>
        public bool ShouldSerializeShouldNotClrReadOnlyProperty()
        {
            return false;
        }

        /// <summary>
        /// Shoulds the serialize should not CLR read only property.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <returns>bool value</returns>
        public bool ShouldSerializeShouldNotClrReadOnlyProperty(XamlDesignerSerializationManager manager)
        {
            return true;
        }

        /// <summary>
        /// Shoulds the serialize should CLR read only property no other should method.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <returns>bool value</returns>
        public bool ShouldSerializeShouldClrReadOnlyPropertyNoOtherShouldMethod(XamlDesignerSerializationManager manager)
        {
            return true;
        }

        /// <summary>
        /// Shoulds the serialize should not CLR read only property no other should method.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <returns>bool value</returns>
        public bool ShouldSerializeShouldNotClrReadOnlyPropertyNoOtherShouldMethod(XamlDesignerSerializationManager manager)
        {
            return false;
        }

        /// <summary>
        /// Shoulds the serialize should hidden CLR property.
        /// </summary>
        /// <returns>bool value</returns>
        public bool ShouldSerializeShouldHiddenClrProperty()
        {
            return true;
        }

        /// <summary>
        /// Shoulds the serialize should hidden CLR property.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <returns>bool value</returns>
        public bool ShouldSerializeShouldHiddenClrProperty(XamlDesignerSerializationManager manager)
        {
            return false;
        }

        /// <summary>
        /// Shoulds the serialize should not hidden CLR property.
        /// </summary>
        /// <returns>bool value</returns>
        public bool ShouldSerializeShouldNotHiddenClrProperty()
        {
            return false;
        }

        /// <summary>
        /// Shoulds the serialize should not hidden CLR property.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <returns>bool value</returns>
        public bool ShouldSerializeShouldNotHiddenClrProperty(XamlDesignerSerializationManager manager)
        {
            return true;
        }

        /// <summary>
        /// Shoulds the serialize should hidden CLR property no other should method.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <returns>bool value</returns>
        public bool ShouldSerializeShouldHiddenClrPropertyNoOtherShouldMethod(XamlDesignerSerializationManager manager)
        {
            return false;
        }

        /// <summary>
        /// Shoulds the serialize should not hidden CLR property no other should method.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <returns>bool value</returns>
        public bool ShouldSerializeShouldNotHiddenClrPropertyNoOtherShouldMethod(XamlDesignerSerializationManager manager)
        {
            return true;
        }

        /// <summary>
        /// Shoulds the serialize should not CLR property.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <returns>bool value</returns>
        private bool ShouldSerializeShouldNotClrProperty(XamlDesignerSerializationManager manager)
        {
            return false;
        }
    }
}
