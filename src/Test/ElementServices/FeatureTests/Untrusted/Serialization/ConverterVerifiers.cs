// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Verification routines for xaml related to Converter
 *
 
  
 * Revision:         $Revision: $
 
 * Filename:         $Source: $
********************************************************************/

using System;
using System.Windows;
using System.Windows.Controls;
using Avalon.Test.CoreUI.Common;
using System.Windows.Media;
using System.Collections;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test.Serialization.CustomElements;

namespace Avalon.Test.CoreUI.Serialization.Converter
{
    /// <summary>
    /// Holds verification routines for xaml related to converter.
    /// </summary>
    public class ConverterVerifiers
    {
        /// <summary>
        /// Verification for CustomTypesInAttribute1.xaml
        /// </summary>
        public static void CustomTypesInAttribute1Verify(UIElement uie)
        {
            CoreLogger.LogStatus("Inside ConverterVerifiers.CustomTypesInAttribute1Verify()...");
            CustomTypesInAttributeVerify(uie, 1);
        }
        /// <summary>
        /// Verification for CustomTypesInAttribute2.xaml
        /// </summary>
        public static void CustomTypesInAttribute2Verify(UIElement uie)
        {
            CoreLogger.LogStatus("Inside ConverterVerifiers.CustomTypesInAttribute2Verify()...");
            CustomTypesInAttributeVerify(uie, 2);
        }

        static void CustomTypesInAttributeVerify(UIElement uie, int index)
        {
            CoreLogger.LogStatus("Inside LiteralwithResourcesVerifier.Verify()...");
            FrameworkElement fe = uie as FrameworkElement;
            CustomControlWithTypeProperties element = fe.Resources["ElementInResources"] as CustomControlWithTypeProperties;
            CoreLogger.LogStatus("Verifying element in Resources ...");
            VerifyCustomElement(element, index);
            element = fe.FindName("ElementInLogicalTree") as CustomControlWithTypeProperties;
            CoreLogger.LogStatus("Verifying element in logical tree ...");
            VerifyCustomElement(element, index);
        }        

        static void VerifyCustomElement(CustomControlWithTypeProperties element, int index)
        {
            //verifying resources
            CoreLogger.LogStatus("Verifing properties...");
            VerifyElement.VerifyBool(null == element, false);
            Type propertyValue = CustomControlWithTypeProperties.GetCustomDP(element);
            VerifyElement.VerifyBool(typeof(CustomControlWithTypeProperties).Equals(propertyValue), true);
            propertyValue = CustomControlWithTypeProperties.GetCustomAttached(element);
            VerifyElement.VerifyBool(typeof(CustomControlWithTypeProperties).Equals(propertyValue), true);
            propertyValue = element.CustomClrProperty;            
            VerifyElement.VerifyBool(typeof(CustomControlWithTypeProperties).Equals(propertyValue), true);
            CoreLogger.LogStatus("Verifing style...");
            Style style = element.Style;
            VerifyElement.VerifyBool(null == style, false);
            Type targetType = style.TargetType;
            VerifyElement.VerifyBool(null == targetType, false);
            if(1 == index)
                    VerifyStyle1(style);
            else if(2 == index)
                    VerifyStyle2(style);
        }
        /// <summary>
        /// Verify the Style in CustomTypesInAttribute1.xaml
        /// </summary>
        /// <param name="style"></param>
        static void VerifyStyle1(Style style)
        {
            SetterBaseCollection setters = style.Setters;
            VerifyElement.VerifyInt(setters.Count, 2);
            Setter setter = setters[0] as Setter;
            VerifyElement.VerifyBool(null == setter, false);
            VerifyElement.VerifyBool(CustomControlWithTypeProperties.CustomAttachedProperty == setter.Property, true);
            Type valueType = setter.Value as Type;
            VerifyElement.VerifyBool(null == valueType, false);
            VerifyElement.VerifyBool(typeof(Button).Equals(valueType), true);

            setter = setters[1] as Setter;
            VerifyElement.VerifyBool(null == setter, false);
            VerifyElement.VerifyBool(CustomControlWithTypeProperties.CustomDPProperty == setter.Property, true);
            valueType = setter.Value as Type;
            VerifyElement.VerifyBool(null == valueType, false);
            VerifyElement.VerifyBool(typeof(Label).Equals(valueType), true);
        }
        
        /// <summary>
        /// Verify the Style in CustomTypesInAttribute2.xaml
        /// </summary>
        /// <param name="style"></param>
        static void VerifyStyle2(Style style)
        {
            TriggerCollection triggers = style.Triggers;
            VerifyElement.VerifyInt(triggers.Count, 2);
            Trigger trigger = triggers[0] as Trigger;
            VerifyElement.VerifyBool(null == trigger, false);
            VerifyElement.VerifyBool(CustomControlWithTypeProperties.CustomAttachedProperty == trigger.Property, true);
            Type conditionType = trigger.Value as Type;
            VerifyElement.VerifyBool(null == conditionType, false);
            VerifyElement.VerifyBool(typeof(TextBox).Equals(conditionType), true);
            
            Setter setter = trigger.Setters[0] as Setter;
            VerifyElement.VerifyBool(null == setter, false);
            VerifyElement.VerifyBool(CustomControlWithTypeProperties.CustomDPProperty == setter.Property, true);
            Type valueType = setter.Value as Type;
            VerifyElement.VerifyBool(null == valueType, false);
            VerifyElement.VerifyBool(typeof(ContentElement).Equals(valueType), true);

            trigger = triggers[1] as Trigger;
            VerifyElement.VerifyBool(null == trigger, false);
            VerifyElement.VerifyBool(CustomControlWithTypeProperties.CustomDPProperty == trigger.Property, true);
            conditionType = trigger.Value as Type;
            VerifyElement.VerifyBool(null == conditionType, false);
            VerifyElement.VerifyBool(typeof(ContentElement).Equals(conditionType), true);

            setter = trigger.Setters[0] as Setter;
            VerifyElement.VerifyBool(null == setter, false);
            VerifyElement.VerifyBool(CustomControlWithTypeProperties.CustomAttachedProperty == setter.Property, true);
            valueType = setter.Value as Type;
            VerifyElement.VerifyBool(null == valueType, false);
            VerifyElement.VerifyBool(typeof(TextBox).Equals(valueType), true);
        }
    }
}
