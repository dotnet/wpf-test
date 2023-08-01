// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
* Description: Custom ContentControl with propertyies of various types. 
* Owner: Microsoft
*
 
  
* Revision:         $Revision: 1 $
 
********************************************************************/

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.ComponentModel;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Controls;

namespace Avalon.Test.CoreUI.Serialization.Converter
{

    #region CustomElementWithCustomTypeProperties
    /// <summary>
    /// A contentControl with properties of various types.
    /// </summary>
    public class CustomElementWithCustomTypeProperties : ContentControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CustomElementWithCustomTypeProperties()
            : base()
        {
        }

        #region property 

        #region CustomElementNoneNoneInvalidTypeDP
        /// <summary>
        /// Settor for CustomElementNoneNoneInvalidTypeDP.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static CustomElementNoneNoneInvalidType GetCustomElementNoneNoneInvalidTypeDP(DependencyObject e)
        {
            return e.GetValue(CustomElementNoneNoneInvalidTypeDPProperty) as CustomElementNoneNoneInvalidType;
        }

        /// <summary>
        /// Gettor for CustomElementNoneNoneInvalidTypeDP.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomElementNoneNoneInvalidTypeDP(DependencyObject e, CustomElementNoneNoneInvalidType myProperty)
        {
            e.SetValue(CustomElementNoneNoneInvalidTypeDPProperty, myProperty);
        }
        /// <summary>
        ///  Depedency Property of type CustomElementNoneNoneInvalidType.
        /// </summary>
        public static DependencyProperty CustomElementNoneNoneInvalidTypeDPProperty =
            DependencyProperty.Register("CustomElementNoneNoneInvalidTypeDP", typeof(CustomElementNoneNoneInvalidType), typeof(CustomElementWithCustomTypeProperties));
        #endregion CustomElementNoneNoneInvalidTypeDP
        
        #region CustomElementNoneNoneNoNoneAttached
        /// <summary>
        /// Settor for CustomElementNoneNoneNoNoneAttached.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static CustomElementNoneNoneNoNone GetCustomElementNoneNoneNoNoneAttached(DependencyObject e)
        {
            return e.GetValue(CustomElementNoneNoneNoNoneAttachedProperty) as CustomElementNoneNoneNoNone;
        }
        /// <summary>
        /// Gettor for CustomElementNoneNoneNoNoneAttached.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomElementNoneNoneNoNoneAttached(DependencyObject e, CustomElementNoneNoneNoNone myProperty)
        {
            e.SetValue(CustomElementNoneNoneNoNoneAttachedProperty, myProperty);
        }
        /// <summary>
        ///  Attached Dependency property of type CustomElementNoneNoneNoNone.
        /// </summary>
        public static DependencyProperty CustomElementNoneNoneNoNoneAttachedProperty =
            DependencyProperty.RegisterAttached("CustomElementNoneNoneNoNoneAttached", typeof(CustomElementNoneNoneNoNone), typeof(CustomElementWithCustomTypeProperties));
        #endregion CustomElementNoneNoneNoNoneAttached


        #region CustomElementNoneNoneInvalidPropertyDP
        /// <summary>
        /// Settor for CustomElementNoneNoneInvalidPropertyDP.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [TypeConverter(typeof(InvalidTypeConverter))]
        public static CustomElementNoneNoneInvalidProperty GetCustomElementNoneNoneInvalidPropertyDP(DependencyObject e)
        {
            return e.GetValue(CustomElementNoneNoneInvalidPropertyDPProperty) as CustomElementNoneNoneInvalidProperty;
        }
        /// <summary>
        /// Gettor for CustomElementStrinCustomElementNoneNoneInvalidPropertyDP.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomElementNoneNoneInvalidPropertyDP(DependencyObject e, CustomElementNoneNoneInvalidProperty myProperty)
        {
            e.SetValue(CustomElementNoneNoneInvalidPropertyDPProperty, myProperty);
        }

        /// <summary>
        ///  Dependency property of type CustomElementNoneNoneInvalidProperty.
        /// </summary>
        public static DependencyProperty CustomElementNoneNoneInvalidPropertyDPProperty =
            DependencyProperty.Register("CustomElementNoneNoneInvalidPropertyDP", typeof(CustomElementNoneNoneInvalidProperty), typeof(CustomElementWithCustomTypeProperties));
        #endregion CustomElementNoneNoneInvalidPropertyDP
        #region CustomElementNoneNoneValidPropertyDP
        /// <summary>
        /// Settor for CustomElementNoneNoneValidPropertyDP.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [TypeConverter(typeof(ValidTypeConverterForCustomElementNoneNoneValidProperty))]
        public static CustomElementNoneNoneValidProperty GetCustomElementNoneNoneValidPropertyDP(DependencyObject e)
        {
            return e.GetValue(CustomElementNoneNoneValidPropertyDPProperty) as CustomElementNoneNoneValidProperty;
        }
        /// <summary>
        /// Gettor for CustomElementStrinCustomElementNoneNoneValidPropertyDP.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomElementNoneNoneValidPropertyDP(DependencyObject e, CustomElementNoneNoneValidProperty myProperty)
        {
            e.SetValue(CustomElementNoneNoneValidPropertyDPProperty, myProperty);
        }

        /// <summary>
        ///  Dependency property of type CustomElementNoneNoneValidProperty.
        /// </summary>
        public static DependencyProperty CustomElementNoneNoneValidPropertyDPProperty =
            DependencyProperty.Register("CustomElementNoneNoneValidPropertyDP", typeof(CustomElementNoneNoneValidProperty), typeof(CustomElementWithCustomTypeProperties));
        #endregion CustomElementNoneNoneValidPropertyDP

        #region CustomElementNoneNoneValidTypeClr
        /// <summary>
        ///  Clr property of type CustomElementNoneNoneValidType.
        /// </summary>
        public CustomElementNoneNoneValidType CustomElementNoneNoneValidTypeClr
        {
            get
            {
                return _CustomElementNoneNoneValidTypeClr;
            }
            set
            {
                _CustomElementNoneNoneValidTypeClr = value;
            }
        }
        private CustomElementNoneNoneValidType _CustomElementNoneNoneValidTypeClr;
        #endregion

        #region CustomElementNoneNoneValidTypeDP
        /// <summary>
        /// Settor for CustomElementNoneNoneValidTypeDP.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static CustomElementNoneNoneValidType GetCustomElementNoneNoneValidTypeDP(DependencyObject e)
        {
            return e.GetValue(CustomElementNoneNoneValidTypeDPProperty) as CustomElementNoneNoneValidType;
        }
        /// <summary>
        /// Gettor for CustomElementStrinCustomElementNoneNoneValidTypeDP.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomElementNoneNoneValidTypeDP(DependencyObject e, CustomElementNoneNoneValidType myProperty)
        {
            e.SetValue(CustomElementNoneNoneValidTypeDPProperty, myProperty);
        }

        /// <summary>
        ///  Dependency property of type CustomElementNoneNoneValidType.
        /// </summary>
        public static DependencyProperty CustomElementNoneNoneValidTypeDPProperty =
            DependencyProperty.Register("CustomElementNoneNoneValidTypeDP", typeof(CustomElementNoneNoneValidType), typeof(CustomElementWithCustomTypeProperties));
        #endregion CustomElementNoneNoneValidTypeDP
        #region CustomElementStringBothValidTypeAttached
        /// <summary>
        /// Settor for CustomElementStringBothValidTypeAttached.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [TypeConverter("Avalon.Test.CoreUI.Serialization.Converter.InvalidTypeConverter, CoreTestsUntrusted")]
        [ValueSerializer("Avalon.Test.CoreUI.Serialization.Converter.ValueSerializer1, CoreTestsUntrusted")]
        public static CustomElementStringBothValidType GetCustomElementStringBothValidTypeAttached(DependencyObject e)
        {
            return e.GetValue(CustomElementStringBothValidTypeAttachedProperty) as CustomElementStringBothValidType;
        }
        /// <summary>
        /// Gettor for CustomElementStringBothValidTypeAttached.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomElementStringBothValidTypeAttached(DependencyObject e, CustomElementStringBothValidType myProperty)
        {
            e.SetValue(CustomElementStringBothValidTypeAttachedProperty, myProperty);
        }
        /// <summary>
        ///  Attached DP of type CustomElementStringBothValidType.
        /// </summary>
        public static DependencyProperty CustomElementStringBothValidTypeAttachedProperty =
            DependencyProperty.RegisterAttached("CustomElementStringBothValidTypeAttached", typeof(CustomElementStringBothValidType), typeof(CustomElementWithCustomTypeProperties));
                #endregion CustomElementStringBothValidTypeAttached
        #region CustomElementStringBothInvalidProperty
        /// <summary>
        /// Settor for CustomElementStringBothInvalidProperty.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [TypeConverter("Avalon.Test.CoreUI.Serialization.Converter.InvalidTypeConverter, CoreTestsUntrusted")]
        [ValueSerializer("Avalon.Test.CoreUI.Serialization.Converter.ValueSerializer1, CoreTestsUntrusted")]
        public static CustomElementStringBothInvalidProperty GetCustomElementStringBothInvalidPropertyDP(DependencyObject e)
        {
            return e.GetValue(CustomElementStringBothInvalidPropertyDPProperty) as CustomElementStringBothInvalidProperty;
        }
        /// <summary>
        /// Gettor for CustomElementStrinCustomElementStringBothInvalidProperty.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomElementStringBothInvalidPropertyDP(DependencyObject e, CustomElementStringBothInvalidProperty myProperty)
        {
            e.SetValue(CustomElementStringBothInvalidPropertyDPProperty, myProperty);
        }

        /// <summary>
        ///  Dependency Property of type CustomElementStringBothInvalidProperty.
        /// </summary>
        public static DependencyProperty CustomElementStringBothInvalidPropertyDPProperty =
            DependencyProperty.Register("CustomElementStringBothInvalidPropertyDP", typeof(CustomElementStringBothInvalidProperty), typeof(CustomElementWithCustomTypeProperties));
        #endregion CustomElementStringBotherInvalidPropertyDP

        #region CustomElementStringPropertyValidPropertyClr
        /// <summary>
        /// Clr property of type CustomElementStringPropertyValidProperty.
        /// </summary>
        [ValueSerializer("Avalon.Test.CoreUI.Serialization.Converter.ValueSerializer1, CoreTestsUntrusted")]
        [TypeConverter(typeof(ValidTypeConverterForCustomElementStringPropertyValidProperty))]
        public CustomElementStringPropertyValidProperty CustomElementStringPropertyValidPropertyClr
        {
            get
            {
                return _customElementStringPropertyValidPropertyClr;
            }
            set
            {
                _customElementStringPropertyValidPropertyClr= value;
            }
        }

        private CustomElementStringPropertyValidProperty _customElementStringPropertyValidPropertyClr;
        #endregion


        #region CustomElementStringPropertyValidPropertyAttached
        /// <summary>
        /// Settor for CustomElementStringPropertyValidPropertyAttached.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [TypeConverter(typeof(ValidTypeConverterForCustomElementStringPropertyValidProperty))]
        [ValueSerializer("Avalon.Test.CoreUI.Serialization.Converter.ValueSerializer1, CoreTestsUntrusted")]
        public static CustomElementStringPropertyValidProperty GetCustomElementStringPropertyValidPropertyAttached(DependencyObject e)
        {
            return e.GetValue(CustomElementStringPropertyValidPropertyAttachedProperty) as CustomElementStringPropertyValidProperty;
        }
        /// <summary>
        /// Gettor for CustomElementStringPropertyValidPropertyAttached.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomElementStringPropertyValidPropertyAttached(DependencyObject e, CustomElementStringPropertyValidProperty myProperty)
        {
            e.SetValue(CustomElementStringPropertyValidPropertyAttachedProperty, myProperty);
        }
        /// <summary>
        ///  Attached Dependency Property of type CustomElementStringPropertyValidProperty.
        /// </summary>
        public static DependencyProperty CustomElementStringPropertyValidPropertyAttachedProperty =
            DependencyProperty.RegisterAttached("CustomElementStringPropertyValidPropertyAttached", typeof(CustomElementStringPropertyValidProperty), typeof(CustomElementWithCustomTypeProperties));
        #endregion CustomElementStringPropertyValidPropertyAttached

        #region CustomElementStringTypeNoNoneDP
        /// <summary>
        /// Settor for CustomElementStringTypeNoNoneDP.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static CustomElementStringTypeNoNone GetCustomElementStringTypeNoNoneDP(DependencyObject e)
        {
            return e.GetValue(CustomElementStringTypeNoNoneDPProperty) as CustomElementStringTypeNoNone;
        }
        /// <summary>
        /// Gettor for CustomElementStrinCustomElementStringTypeNoNoneDP.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomElementStringTypeNoNoneDP(DependencyObject e, CustomElementStringTypeNoNone myProperty)
        {
            e.SetValue(CustomElementStringTypeNoNoneDPProperty, myProperty);
        }

        /// <summary>
        ///  Dependency Property of type CustomElementStringTypeNoNone.
        /// </summary>
        public static DependencyProperty CustomElementStringTypeNoNoneDPProperty =
            DependencyProperty.Register("CustomElementStringTypeNoNoneDP", typeof(CustomElementStringTypeNoNone), typeof(CustomElementWithCustomTypeProperties));
        #endregion CustomElementStringTypeNoNoneDP

        #region CustomElementStringTypeValidTypeDP
        /// <summary>
        /// Settor for CustomElementStringTypeValidTypeDP.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static CustomElementStringTypeValidType GetCustomElementStringTypeValidTypeDP(DependencyObject e)
        {
            return e.GetValue(CustomElementStringTypeValidTypeDPProperty) as CustomElementStringTypeValidType;
        }
        /// <summary>
        /// Gettor for CustomElementStrinCustomElementStringTypeValidTypeDP.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomElementStringTypeValidTypeDP(DependencyObject e, CustomElementStringTypeValidType myProperty)
        {
            e.SetValue(CustomElementStringTypeValidTypeDPProperty, myProperty);
        }

        /// <summary>
        ///  Dependency Property of type CustomElementStringTypeValidType.
        /// </summary>
        public static DependencyProperty CustomElementStringTypeValidTypeDPProperty =
            DependencyProperty.Register("CustomElementStringTypeValidTypeDP", typeof(CustomElementStringTypeValidType), typeof(CustomElementWithCustomTypeProperties));
        #endregion CustomElementStringTypeValidTypeDP

        #region CustomElementStringTypeInvalidPropertyClr
        /// <summary>
        /// Clr property of type CustomElementStringTypeInvalidPropert.
        /// </summary>
        public CustomElementStringTypeInvalidProperty CustomElementStringTypeInvalidPropertyClr
        {
            get
            {
                return _CustomElementStringTypeInvalidPropertyClr;
            }
            set
            {
                _CustomElementStringTypeInvalidPropertyClr = value;
            }
        }

        private CustomElementStringTypeInvalidProperty _CustomElementStringTypeInvalidPropertyClr;
        #endregion

        #region CustomElementStringTypeInvalidPropertyAttached
        /// <summary>
        /// Settor for CustomElementStringTypeInvalidPropertyAttached
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [TypeConverter("Avalon.Test.CoreUI.Serialization.Converter.InvalidTypeConverter, CoreTestsUntrusted")]
        public static CustomElementStringTypeInvalidProperty GetCustomElementStringTypeInvalidPropertyAttached(DependencyObject e)
        {
            return e.GetValue(CustomElementStringTypeInvalidPropertyAttachedProperty) as CustomElementStringTypeInvalidProperty;
        }
        /// <summary>
        /// Gettor for CustomElementStrinCustomElementStringTypeInvalidPropertyDP
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomElementStringTypeInvalidPropertyAttached(DependencyObject e, CustomElementStringTypeInvalidProperty myProperty)
        {
            e.SetValue(CustomElementStringTypeInvalidPropertyAttachedProperty, myProperty);
        }

        /// <summary>
        ///  Attached Dependency Property of type CustomElementStringTypeInvalidProperty.
        /// </summary>
        public static DependencyProperty CustomElementStringTypeInvalidPropertyAttachedProperty =
            DependencyProperty.RegisterAttached("CustomElementStringTypeInvalidPropertyAttached", typeof(CustomElementStringTypeInvalidProperty), typeof(CustomElementWithCustomTypeProperties));
        #endregion CustomElementStringTypeInvalidPropertyAttached

        #region CustomElementTypeBothNoNoneAttached
        /// <summary>
        /// Settor for CustomElementTypeBothNoNoneAttached.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [ValueSerializer("Avalon.Test.CoreUI.Serialization.Converter.ValueSerializer1, CoreTestsUntrusted")]
        public static CustomElementTypeBothNoNone GetCustomElementTypeBothNoNoneAttached(DependencyObject e)
        {
            return e.GetValue(CustomElementTypeBothNoNoneAttachedProperty) as CustomElementTypeBothNoNone;
        }
        /// <summary>
        /// Gettor for CustomElementTypeBothNoNoneDP.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomElementTypeBothNoNoneAttached(DependencyObject e, CustomElementTypeBothNoNone myProperty)
        {
            e.SetValue(CustomElementTypeBothNoNoneAttachedProperty, myProperty);
        }

        /// <summary>
        ///  Attached Dependency Property of type CustomElementTypeBothNoNone.
        /// </summary>
        public static DependencyProperty CustomElementTypeBothNoNoneAttachedProperty =
            DependencyProperty.RegisterAttached("CustomElementTypeBothNoNoneAttached", typeof(CustomElementTypeBothNoNone), typeof(CustomElementWithCustomTypeProperties));
        #endregion CustomElementTypeBothNoNoneAttached

        #region CustomElementTypeBothValidPropertyClr
        /// <summary>
        /// CLR propery of type CustomElementTypeBothValidProperty
        /// </summary>
        [ValueSerializer(typeof(ValueSerializer1))]
        [TypeConverter(typeof(ValidTypeConverterForCustomElementTypeBothValidProperty))]
        public CustomElementTypeBothValidProperty CustomElementTypeBothValidPropertyClr
        {
            get
            {
                return _CustomElementTypeBothValidPropertyClr;
            }
            set
            {
                _CustomElementTypeBothValidPropertyClr = value;
            }
        }

        private CustomElementTypeBothValidProperty _CustomElementTypeBothValidPropertyClr;
        

        #region CustomElementTypePropertyInvalidTypeAttached
        /// <summary>
        /// Settor for CustomElementTypePropertyInvalidTypeAttached.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [TypeConverter(typeof(InvalidTypeConverter))]
        [ValueSerializer(typeof(ValueSerializer1))]
        public static CustomElementTypePropertyInvalidType GetCustomElementTypePropertyInvalidTypeAttached(DependencyObject e)
        {
            return e.GetValue(CustomElementTypePropertyInvalidTypeAttachedProperty) as CustomElementTypePropertyInvalidType;
        }
        /// <summary>
        /// Gettor for CustomElementStrinCustomElementTypePropertyInvalidTypeAttached.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomElementTypePropertyInvalidTypeAttached(DependencyObject e, CustomElementTypePropertyInvalidType myProperty)
        {
            e.SetValue(CustomElementTypePropertyInvalidTypeAttachedProperty, myProperty);
        }

        /// <summary>
        ///  Attached Dependency Property of type CustomElementTypePropertyInvalidType.
        /// </summary>
        public static DependencyProperty CustomElementTypePropertyInvalidTypeAttachedProperty =
            DependencyProperty.RegisterAttached("CustomElementTypePropertyInvalidTypeAttached", typeof(CustomElementTypePropertyInvalidType), typeof(CustomElementWithCustomTypeProperties));
        #endregion

        #region CustomElementTypeTypeNoNoneDP
        /// <summary>
        /// Settor for CustomElementTypeTypeNoNoneDP.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static CustomElementTypeTypeNoNone GetCustomElementTypeTypeNoNoneDP(DependencyObject e)
        {
            return e.GetValue(CustomElementTypeTypeNoNoneDPProperty) as CustomElementTypeTypeNoNone;
        }
        /// <summary>
        /// Gettor for CustomElementTypeTypeNoNoneDP.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomElementTypeTypeNoNoneDP(DependencyObject e, CustomElementTypeTypeNoNone myProperty)
        {
            e.SetValue(CustomElementTypeTypeNoNoneDPProperty, myProperty);
        }
        /// <summary>
        ///  Dependency Property of type CustomElementTypeTypeNoNone.
        /// </summary>
        public static DependencyProperty CustomElementTypeTypeNoNoneDPProperty =
            DependencyProperty.Register("CustomElementTypeTypeNoNoneDP", typeof(CustomElementTypeTypeNoNone), typeof(CustomElementWithCustomTypeProperties));
        #endregion CustomElementTypeTypeNoNoneDP

        #region CustomElementTypeBothValidPropertyDP
        /// <summary>
        /// Settor for CustomElementTypeBothValidPropertyDP.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [TypeConverter(typeof(ValidTypeConverterForCustomElementTypeBothValidProperty))]
        [ValueSerializer(typeof(ValueSerializer1))]
        public static CustomElementTypeBothValidProperty GetCustomElementTypeBothValidPropertyDP(DependencyObject e)
        {
            return e.GetValue(CustomElementTypeBothValidPropertyDPProperty) as CustomElementTypeBothValidProperty;
        }
        /// <summary>
        /// Gettor for CustomElementStrinCustomElementTypeBothValidPropertyDP.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomElementTypeBothValidPropertyDP(DependencyObject e, CustomElementTypeBothValidProperty myProperty)
        {
            e.SetValue(CustomElementTypeBothValidPropertyDPProperty, myProperty);
        }

        /// <summary>
        ///  Dependency Property of type CustomElementTypeBothValidProperty.
        /// </summary>
        public static DependencyProperty CustomElementTypeBothValidPropertyDPProperty =
            DependencyProperty.Register("CustomElementTypeBothValidPropertyDP", typeof(CustomElementTypeBothValidProperty), typeof(CustomElementWithCustomTypeProperties));
        #endregion CustomElementTypeBothValidPropertyDP
        #endregion

        #region CustomElementTypePropertyInvalidTypeDP
        /// <summary>
        /// Settor for CustomElementTypePropertyInvalidTypeDP.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [ValueSerializer(typeof(ValueSerializer1))]
        public static CustomElementTypePropertyInvalidType GetCustomElementTypePropertyInvalidTypeDP(DependencyObject e)
        {
            return e.GetValue(CustomElementTypePropertyInvalidTypeDPProperty) as CustomElementTypePropertyInvalidType;
        }
        /// <summary>
        /// Gettor for CustomElementStrinCustomElementTypePropertyInvalidTypeDP.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomElementTypePropertyInvalidTypeDP(DependencyObject e, CustomElementTypePropertyInvalidType myProperty)
        {
            e.SetValue(CustomElementTypePropertyInvalidTypeDPProperty, myProperty);
        }

        /// <summary>
        ///  Dependency Property of Type CustomElementTypePropertyInvalidType.
        /// </summary>
        public static DependencyProperty CustomElementTypePropertyInvalidTypeDPProperty =
            DependencyProperty.Register("CustomElementTypePropertyInvalidTypeDP", typeof(CustomElementTypePropertyInvalidType), typeof(CustomElementWithCustomTypeProperties));
        #endregion CustomElementTypePropertyInvalidTypeDP

        #region CustomElementTypeClr
        /// <summary>
        /// Clr property of type CustomElementStringTypeInvalidPropert.
        /// </summary>
        public CustomElementType CustomElementTypeClr
        {
            get
            {
                return _CustomElementTypeClr;
            }
            set
            {
                _CustomElementTypeClr = value;
            }
        }

        private CustomElementType _CustomElementTypeClr;
        #endregion

        #region CustomElementTypeDP
        /// <summary>
        /// Settor for CustomElementTypeDP.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>        
        public static CustomElementType GetCustomElementTypeDP(DependencyObject e)
        {
            return e.GetValue(CustomElementTypeDPProperty) as CustomElementType;
        }
        /// <summary>
        /// Gettor for CustomElementStrinCustomElementTypeDP.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomElementTypeDP(DependencyObject e, CustomElementType myProperty)
        {
            e.SetValue(CustomElementTypeDPProperty, myProperty);
        }

        /// <summary>
        ///  Dependency Property of Type CustomElementType.
        /// </summary>
        public static DependencyProperty CustomElementTypeDPProperty =
            DependencyProperty.Register("CustomElementTypeDP", typeof(CustomElementType), typeof(CustomElementWithCustomTypeProperties));
        #endregion CustomElementTypeDP

        #region CustomElementTypeAttached
        /// <summary>
        /// Settor for CustomElementTypeAttached.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static CustomElementType GetCustomElementTypeAttached(DependencyObject e)
        {
            return e.GetValue(CustomElementTypeAttachedProperty) as CustomElementType;
        }
        /// <summary>
        /// Gettor for CustomElementTypeDP.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomElementTypeAttached(DependencyObject e, CustomElementType myProperty)
        {
            e.SetValue(CustomElementTypeAttachedProperty, myProperty);
        }

        /// <summary>
        ///  Attached Dependency Property of type CustomElementType.
        /// </summary>
        public static DependencyProperty CustomElementTypeAttachedProperty =
            DependencyProperty.RegisterAttached("CustomElementTypeAttached", typeof(CustomElementType), typeof(CustomElementWithCustomTypeProperties));
        #endregion CustomElementTypeAttached

        #region CustomFrameworkElementTypeClr
        /// <summary>
        /// Clr property of type CustomElementStringTypeInvalidPropert.
        /// </summary>
        public CustomFrameworkElementType CustomFrameworkElementTypeClr
        {
            get
            {
                return _CustomFrameworkElementTypeClr;
            }
            set
            {
                _CustomFrameworkElementTypeClr = value;
            }
        }

        private CustomFrameworkElementType _CustomFrameworkElementTypeClr;
        #endregion

        #region CustomFrameworkElementTypeDP
        /// <summary>
        /// Settor for CustomFrameworkElementTypeDP.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>        
        public static CustomFrameworkElementType GetCustomFrameworkElementTypeDP(DependencyObject e)
        {
            return e.GetValue(CustomFrameworkElementTypeDPProperty) as CustomFrameworkElementType;
        }
        /// <summary>
        /// Gettor for CustomElementStrinCustomFrameworkElementTypeDP.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomFrameworkElementTypeDP(DependencyObject e, CustomFrameworkElementType myProperty)
        {
            e.SetValue(CustomFrameworkElementTypeDPProperty, myProperty);
        }

        /// <summary>
        ///  Dependency Property of Type CustomFrameworkElementType.
        /// </summary>
        public static DependencyProperty CustomFrameworkElementTypeDPProperty =
            DependencyProperty.Register("CustomFrameworkElementTypeDP", typeof(CustomFrameworkElementType), typeof(CustomElementWithCustomTypeProperties));
        #endregion CustomFrameworkElementTypeDP

        #region CustomFrameworkElementTypeAttached
        /// <summary>
        /// Settor for CustomFrameworkElementTypeAttached.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static CustomFrameworkElementType GetCustomFrameworkElementTypeAttached(DependencyObject e)
        {
            return e.GetValue(CustomFrameworkElementTypeAttachedProperty) as CustomFrameworkElementType;
        }
        /// <summary>
        /// Gettor for CustomFrameworkElementTypeDP.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomFrameworkElementTypeAttached(DependencyObject e, CustomFrameworkElementType myProperty)
        {
            e.SetValue(CustomFrameworkElementTypeAttachedProperty, myProperty);
        }

        /// <summary>
        ///  Attached Dependency Property of type CustomFrameworkElementType.
        /// </summary>
        public static DependencyProperty CustomFrameworkElementTypeAttachedProperty =
            DependencyProperty.RegisterAttached("CustomFrameworkElementTypeAttached", typeof(CustomFrameworkElementType), typeof(CustomElementWithCustomTypeProperties));
        #endregion CustomFrameworkElementTypeAttached


        #region CustomElementPropertyClr
        /// <summary>
        /// Clr property of type CustomElementStringTypeInvalidPropert.
        /// </summary>
        [ValueSerializer(typeof(ValueSerializer3))]
        public CustomElementProperty CustomElementPropertyClr
        {
            get
            {
                return _CustomElementPropertyClr;
            }
            set
            {
                _CustomElementPropertyClr = value;
            }
        }

        private CustomElementProperty _CustomElementPropertyClr;
        #endregion

        #region CustomElementPropertyDP
        /// <summary>
        /// Settor for CustomElementPropertyDP.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>       
        [ValueSerializer(typeof(ValueSerializer3))]
        public static CustomElementProperty GetCustomElementPropertyDP(DependencyObject e)
        {
            return e.GetValue(CustomElementPropertyDPProperty) as CustomElementProperty;
        }
        /// <summary>
        /// Gettor for CustomElementStrinCustomElementPropertyDP.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomElementPropertyDP(DependencyObject e, CustomElementProperty myProperty)
        {
            e.SetValue(CustomElementPropertyDPProperty, myProperty);
        }

        /// <summary>
        ///  Dependency Property of Type CustomElementProperty.
        /// </summary>
        public static DependencyProperty CustomElementPropertyDPProperty =
            DependencyProperty.Register("CustomElementPropertyDP", typeof(CustomElementProperty), typeof(CustomElementWithCustomTypeProperties));
        #endregion CustomElementPropertyDP

        #region CustomElementPropertyAttached
        /// <summary>
        /// Settor for CustomElementPropertyAttached.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [ValueSerializer(typeof(ValueSerializer3))]
        public static CustomElementProperty GetCustomElementPropertyAttached(DependencyObject e)
        {
            return e.GetValue(CustomElementPropertyAttachedProperty) as CustomElementProperty;
        }
        /// <summary>
        /// Gettor for CustomElementPropertyDP.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomElementPropertyAttached(DependencyObject e, CustomElementProperty myProperty)
        {
            e.SetValue(CustomElementPropertyAttachedProperty, myProperty);
        }

        /// <summary>
        ///  Attached Dependency Property of type CustomElementProperty.
        /// </summary>
        public static DependencyProperty CustomElementPropertyAttachedProperty =
            DependencyProperty.RegisterAttached("CustomElementPropertyAttached", typeof(CustomElementProperty), typeof(CustomElementWithCustomTypeProperties));
        #endregion CustomElementPropertyAttached


        #region CustomFrameworkElementPropertyClr
        /// <summary>
        /// Clr property of type CustomElementStringTypeInvalidPropert.
        /// </summary>
        [ValueSerializer(typeof(ValueSerializer3))]
        public CustomFrameworkElementProperty CustomFrameworkElementPropertyClr
        {
            get
            {
                return _CustomFrameworkElementPropertyClr;
            }
            set
            {
                _CustomFrameworkElementPropertyClr = value;
            }
        }

        private CustomFrameworkElementProperty _CustomFrameworkElementPropertyClr;
        #endregion

        #region CustomFrameworkElementPropertyDP
        /// <summary>
        /// Settor for CustomFrameworkElementPropertyDP.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>        
        [ValueSerializer(typeof(ValueSerializer3))]
        public static CustomFrameworkElementProperty GetCustomFrameworkElementPropertyDP(DependencyObject e)
        {
            return e.GetValue(CustomFrameworkElementPropertyDPProperty) as CustomFrameworkElementProperty;
        }
        /// <summary>
        /// Gettor for CustomElementStrinCustomFrameworkElementPropertyDP.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomFrameworkElementPropertyDP(DependencyObject e, CustomFrameworkElementProperty myProperty)
        {
            e.SetValue(CustomFrameworkElementPropertyDPProperty, myProperty);
        }

        /// <summary>
        ///  Dependency Property of Type CustomFrameworkElementProperty.
        /// </summary>
        public static DependencyProperty CustomFrameworkElementPropertyDPProperty =
            DependencyProperty.Register("CustomFrameworkElementPropertyDP", typeof(CustomFrameworkElementProperty), typeof(CustomElementWithCustomTypeProperties));
        #endregion CustomFrameworkElementPropertyDP

        #region CustomFrameworkElementPropertyAttached
        /// <summary>
        /// Settor for CustomFrameworkElementPropertyAttached.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [ValueSerializer(typeof(ValueSerializer3))]
        public static CustomFrameworkElementProperty GetCustomFrameworkElementPropertyAttached(DependencyObject e)
        {
            return e.GetValue(CustomFrameworkElementPropertyAttachedProperty) as CustomFrameworkElementProperty;
        }
        /// <summary>
        /// Gettor for CustomFrameworkElementPropertyDP.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomFrameworkElementPropertyAttached(DependencyObject e, CustomFrameworkElementProperty myProperty)
        {
            e.SetValue(CustomFrameworkElementPropertyAttachedProperty, myProperty);
        }

        /// <summary>
        ///  Attached Dependency Property of type CustomFrameworkElementProperty.
        /// </summary>
        public static DependencyProperty CustomFrameworkElementPropertyAttachedProperty =
            DependencyProperty.RegisterAttached("CustomFrameworkElementPropertyAttached", typeof(CustomFrameworkElementProperty), typeof(CustomElementWithCustomTypeProperties));
        #endregion CustomFrameworkElementPropertyAttached


        #region StringProperty
        /// <summary>
        /// Settor for StringProperty
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetStringProperty(DependencyObject e)
        {
            return e.GetValue(StringPropertyProperty) as string;
        }

        /// <summary>
        /// Gettor for StringProperty
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetStringProperty(DependencyObject e, string myProperty)
        {
            e.SetValue(StringPropertyProperty, myProperty);
        }

        /// <summary>
        ///  A string property used to reflect the change caused by Triggers.
        /// </summary>
        public static DependencyProperty StringPropertyProperty =
            DependencyProperty.Register("StringProperty", typeof(String), typeof(CustomElementWithCustomTypeProperties));
        #endregion
        #endregion
    }
    #endregion
}
