// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Avalon.Test.CoreUI.PropertyEngine
{

    /******************************************************************************
    * CLASS:          AttachedPropertyObject
    ******************************************************************************/
    class AttachedPropertyObject : DependencyObject
    {
        #region AttachedPropertyObject Dependency Properties
        //
        // This attached property can attach itself only to 
        // things of type SimplePropertyObject
        //
        public static readonly DependencyProperty AttachedProperty =
            DependencyProperty.RegisterAttached("Attached", typeof(string),
            typeof(AttachedPropertyObject),
            new PropertyMetadata("DefaultValue"));

        public static readonly DependencyProperty HiddenAttachedProperty =
            DependencyProperty.RegisterAttached("HiddenAttached", typeof(string),
            typeof(AttachedPropertyObject),
            new PropertyMetadata("DefaultValue"));

        public static readonly DependencyProperty AttachedSpecialMethodProperty =
            DependencyProperty.RegisterAttached("AttachedSpecialMethod", typeof(string),
            typeof(AttachedPropertyObject),
            new PropertyMetadata("DefaultValue"));

        public static readonly DependencyProperty ReadOnlyAttachedCLRProperty =
            DependencyProperty.RegisterAttached("ReadOnlyAttachedCLR", typeof(string),
            typeof(AttachedPropertyObject),
            new PropertyMetadata("DefaultValue"));

        public static readonly DependencyProperty ReadOnlyAttachedCLRMetadataProperty =
            DependencyProperty.RegisterAttached("ReadOnlyAttachedCLRMetadata", typeof(string),
            typeof(AttachedPropertyObject),
            new PropertyMetadata("DefaultValue"));

        [Description("AttachedProperty")]
        [AttachedPropertyBrowsableForType(typeof(SimplePropertyObject))]
        [AttachedPropertyBrowsableForType(typeof(AttachedPropertyObject))]
        [AttachedPropertyBrowsableWhenAttributePresent(typeof(DescriptionAttribute))]
        public static string GetAttached(SimplePropertyObject o)
        {
            return (string)o.GetValue(AttachedProperty);
        }

        [Description("ReadOnlyAttachedCLRProperty")]
        [AttachedPropertyBrowsableForType(typeof(SimplePropertyObject))]
        public static string GetReadOnlyAttachedCLR(SimplePropertyObject o)
        {
            return (string)o.GetValue(ReadOnlyAttachedCLRProperty);
        }
        // no Set method on this guy

        [ReadOnly(true)]
        [Description("ReadOnlyAttachedCLRMetadataProperty")]
        [AttachedPropertyBrowsableForType(typeof(SimplePropertyObject))]
        [AttachedPropertyBrowsableForType(typeof(AttachedPropertyObject))]
        public static string GetReadOnlyAttachedCLRMetadata(SimplePropertyObject o)
        {
            return (string)o.GetValue(ReadOnlyAttachedCLRMetadataProperty);
        }

        public static void SetReadOnlyAttachedCLRMetadata(SimplePropertyObject o, string value)
        {
            o.SetValue(ReadOnlyAttachedCLRMetadataProperty, value);
        }

        public static void SetAttached(SimplePropertyObject o, string value)
        {
            o.SetValue(AttachedProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        [Description("HiddenAttachedProperty")]
        public static string GetHiddenAttached(UIElement d)
        {
            return (string)d.GetValue(HiddenAttachedProperty);
        }

        public static void SetHiddenAttached(UIElement d, string value)
        {
            d.SetValue(HiddenAttachedProperty, value);
        }

        public static bool ShouldSerializeAttachedSpecialMethod(DependencyObject d)
        {
            string currentValue = GetAttachedSpecialMethod(d);
            return !(currentValue.Equals("NoSerialize"));
        }

        public static void ResetAttachedSpecialMethod(DependencyObject d)
        {
            SetAttachedSpecialMethod(d, "Reset");
        }

        [Description("AttachedSpecialMethodProperty")]
        [AttachedPropertyBrowsableForType(typeof(SimplePropertyObject))]
        [AttachedPropertyBrowsableForType(typeof(AttachedPropertyObject))]
        public static string GetAttachedSpecialMethod(DependencyObject d)
        {
            return (string)d.GetValue(AttachedSpecialMethodProperty);
        }

        public static void SetAttachedSpecialMethod(DependencyObject d, string value)
        {
            d.SetValue(AttachedSpecialMethodProperty, value);
        }
        #endregion
    }


    /******************************************************************************
    * CLASS:          SimplePropertyObject
    ******************************************************************************/
    [Description("SimplePropertyObject")]
    class SimplePropertyObject : DependencyObject
    {
        #region SimplePropertyObject Dependency Properties
        public static readonly DependencyProperty TestProperty =
            DependencyProperty.Register("Test", typeof(string),
            typeof(SimplePropertyObject),
            new PropertyMetadata("DefaultValue"));

        private static readonly DependencyProperty s_hiddenPrivateProperty =
            DependencyProperty.Register("HiddenPrivate", typeof(string),
            typeof(SimplePropertyObject),
            new PropertyMetadata("DefaultValue"));

        public static readonly DependencyProperty SpecialMethodTestProperty =
            DependencyProperty.Register("SpecialMethodTest", typeof(string),
            typeof(SimplePropertyObject),
            new PropertyMetadata("DefaultValue"));

        public static readonly DependencyProperty ReadOnlyCLRProperty =
            DependencyProperty.Register("ReadOnlyCLR", typeof(string),
            typeof(SimplePropertyObject),
            new PropertyMetadata("DefaultValue"));

        public static readonly DependencyProperty ReadOnlyCLRMetadataProperty =
            DependencyProperty.Register("ReadOnlyCLRMetadata", typeof(string),
            typeof(SimplePropertyObject),
            new PropertyMetadata("DefaultValue"));

        private static readonly DependencyPropertyKey s_readOnlyDPMetadataKey =
            DependencyProperty.RegisterReadOnly("ReadOnlyDPMetadata", typeof(string),
            typeof(SimplePropertyObject),
            new PropertyMetadata("DefaultValue"));

        public static readonly DependencyProperty ReadOnlyDPMetadataProperty = s_readOnlyDPMetadataKey.DependencyProperty;

        [Description("TestProperty")]
        public string Test
        {
            get { return (string)GetValue(TestProperty); }
            set { SetValue(TestProperty, value); }
        }

        [Description("ReadOnlyCLRProperty")]
        public string ReadOnlyCLR
        {
            get
            {
                return (string)GetValue(ReadOnlyCLRProperty);
            }
        }

        [ReadOnly(true)]
        [Description("ReadOnlyCLRMetadataProperty")]
        public string ReadOnlyCLRMetadata
        {
            get
            {
                return (string)GetValue(ReadOnlyCLRMetadataProperty);
            }
            set
            {
                SetValue(ReadOnlyCLRMetadataProperty, value);
            }
        }

        [Description("ReadOnlyDPMetadataProperty")]
        public string ReadOnlyDPMetadata
        {
            get
            {
                return (string)GetValue(ReadOnlyDPMetadataProperty);
            }
            set
            {
                SetValue(s_readOnlyDPMetadataKey, value);
            }
        }

        [Description("ClrTestProperty")]
        public string ClrTest
        {
            get { return "hello"; }
        }

        public bool ShouldSerializeSpecialMethodTest()
        {
            string currentValue = SpecialMethodTest;
            return !(currentValue.Equals("NoSerialize"));
        }

        public void ResetSpecialMethodTest()
        {
            SpecialMethodTest = "Reset";
        }

        [Description("SpecialMethodTestProperty")]
        public string SpecialMethodTest
        {
            get { return (string)GetValue(SpecialMethodTestProperty); }
            set { SetValue(SpecialMethodTestProperty, value); }
        }
        #endregion
    }


    /******************************************************************************
    * CLASS:          CallbackObject
    ******************************************************************************/
    //
    // Sets up a whole bunch of attached properties with various attach targets.
    //
    class CallbackObject : Canvas
    {
        #region CallBackObject Dependency Properties
        //
        // Property that is valid on immediate children
        //
        public static readonly DependencyProperty ChildrenProperty =
            DependencyProperty.RegisterAttached("Children", typeof(string),
            typeof(CallbackObject),
            new PropertyMetadata("DefaultValue"));

        [AttachedPropertyBrowsableForChildren()]
        public static string GetChildren(DependencyObject d) { return (string)d.GetValue(ChildrenProperty); }
        public static void SetChildren(DependencyObject d, string value) { d.SetValue(ChildrenProperty, value); }

        //
        // Property that is valid on all descendants
        //
        public static readonly DependencyProperty DescendantsProperty =
            DependencyProperty.RegisterAttached("Descendants", typeof(string),
            typeof(CallbackObject),
            new PropertyMetadata("DefaultValue"));


        [AttachedPropertyBrowsableForChildren(IncludeDescendants = true)]
        public static string GetDescendants(DependencyObject d) { return (string)d.GetValue(DescendantsProperty); }
        public static void SetDescendants(DependencyObject d, string value) { d.SetValue(DescendantsProperty, value); }

        //
        // Property that is valid on a single subtype (Button, in this case).  The
        // subtype is automatically determined by the data type of the first parameter
        //
        public static readonly DependencyProperty SingleSubtypeProperty =
            DependencyProperty.RegisterAttached("SingleSubtype", typeof(string),
            typeof(CallbackObject),
            new PropertyMetadata("DefaultValue"));

        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static string GetSingleSubtype(Button d) { return (string)d.GetValue(SingleSubtypeProperty); }
        public static void SetSingleSubtype(Button d, string value) { d.SetValue(SingleSubtypeProperty, value); }

        //
        // Property that is valid on two different subtypes(CheckBox and TextBox, in this case).
        //
        public static readonly DependencyProperty MultipleSubtypeProperty =
            DependencyProperty.RegisterAttached("MultipleSubtype", typeof(string),
            typeof(CallbackObject),
            new PropertyMetadata("DefaultValue"));

        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static string GetMultipleSubtype(FrameworkElement d) { return (string)d.GetValue(MultipleSubtypeProperty); }
        public static void SetMultipleSubtype(FrameworkElement d, string value) { d.SetValue(MultipleSubtypeProperty, value); }

        //
        // Property that is valid on all classes that define an EnablePropertyAttribute
        //
        public static readonly DependencyProperty AttributeProperty =
            DependencyProperty.RegisterAttached("Attribute", typeof(string),
            typeof(CallbackObject),
            new PropertyMetadata("DefaultValue"));

        [AttachedPropertyBrowsableWhenAttributePresent(typeof(EnablePropertyAttribute))]
        public static string GetAttribute(DependencyObject d) { return (string)d.GetValue(AttributeProperty); }
        public static void SetAttribute(DependencyObject d, string value) { d.SetValue(AttributeProperty, value); }
        #endregion
    }


    /******************************************************************************
    * CLASS:          EnablePropertyAttribute
    ******************************************************************************/
    [AttributeUsage(AttributeTargets.Class)]
    class EnablePropertyAttribute : Attribute
    {
        #region EnablePropertyAttribute
        private bool _enable;

        internal EnablePropertyAttribute(bool enable)
        {
            _enable = enable;
        }

        public override bool IsDefaultAttribute()
        {
            // Our default state is enabled = false
            return !_enable;
        }
        #endregion
    }
}
