// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Styles.BasedOn
{
    /// <summary/>
    public static class SerializeBasedonStyleFE_Verify
    {
        /// <summary>
        /// NoTargetType
        /// </summary>
        internal static bool _noTargetType = false;

        /// <summary>
        /// Verifier for SerializeBasedonStyleFE.xaml
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;
            GlobalLog.LogStatus("Verifying SerializeBasedonStyleFE.xaml.");
            FrameworkElement root = rootElement as FrameworkElement;
            VerifyElement.VerifyBool(root != null, true, ref result);

            //Verify effect
            Button button1 = root.FindName("button1") as Button;
            Button button2 = root.FindName("button2") as Button;
            VerifyElement.VerifyBool(button1 != null && button2 != null, true, ref result);

            SolidColorBrush myForeground = button1.Foreground as SolidColorBrush;
            VerifyElement.VerifyBool(null == myForeground, false, ref result);
            VerifyElement.VerifyColor(myForeground.Color, Colors.Red, ref result);

            double mySize = button1.FontSize;
            VerifyElement.VerifyDouble(mySize, 29, ref result);

            myForeground = button2.Foreground as SolidColorBrush;
            VerifyElement.VerifyBool(null == myForeground, false, ref result);
            VerifyElement.VerifyColor(myForeground.Color, Colors.Red, ref result);

            mySize = button2.FontSize;
            VerifyElement.VerifyDouble(mySize, 29, ref result);
            //Verify styles
            Style extendedStyleMultipleFE = root.Resources["extendedStyleMultipleFE"] as Style;
            Style extendedStyleFE         = root.Resources["extendedStyleFE"] as Style;
            Style baseStyleFE             = root.Resources["baseStyleFE"] as Style;

            VerifyElement.VerifyBool(baseStyleFE != null, true, ref result);

            VerifyElement.VerifyBool(extendedStyleFE != null, true, ref result);
            VerifyElement.VerifyBool(extendedStyleFE.BasedOn != null, true, ref result);

            VerifyElement.VerifyBool(extendedStyleMultipleFE != null, true, ref result);
            VerifyElement.VerifyBool(extendedStyleMultipleFE.BasedOn != null, true, ref result);
            VerifyElement.VerifyBool(extendedStyleMultipleFE.BasedOn.BasedOn != null, true, ref result);

            GlobalLog.LogStatus("Verifying base style...");
            VerifyBaseStyleForFE(baseStyleFE, ref result);
            VerifyBaseStyleForFE(extendedStyleFE.BasedOn, ref result);
            VerifyBaseStyleForFE(extendedStyleMultipleFE.BasedOn.BasedOn, ref result);

            GlobalLog.LogStatus("Verifying extented style for FE...");

            VerifyExtendedStyleForFE(extendedStyleFE, ref result);
            VerifyExtendedStyleForFE(extendedStyleMultipleFE.BasedOn, ref result);

            GlobalLog.LogStatus("Verifying the style based on the extented style for FE...");

            if (!_noTargetType)
            {
                VerifyElement.VerifyBool(extendedStyleMultipleFE.TargetType.Equals(typeof(Button)), true, ref result);
            }
            VerifyElement.VerifyBool(extendedStyleMultipleFE.Setters != null, true, ref result);
            VerifyElement.VerifyInt(extendedStyleMultipleFE.Setters.Count, 1, ref result);
            Setter setter = extendedStyleMultipleFE.Setters[0] as Setter;
            VerifyElement.VerifyBool(setter.Property == Control.BackgroundProperty, true, ref result);
            VerifySolidColorBrushSetter(setter, Colors.Green, ref result);
            _noTargetType = false; //reset flag
            return result;
        }

        private static void VerifyBaseStyleForFE(Style style, ref bool result)
        {
            if (!_noTargetType)
            {
                VerifyElement.VerifyBool(style.TargetType.Equals(typeof(Button)), true, ref result);
            }
            VerifyElement.VerifyBool(style.Setters != null, true, ref result);

            VerifyElement.VerifyInt(style.Setters.Count, 2, ref result);
            Setter setter = style.Setters[0] as Setter;
            VerifyElement.VerifyBool(setter.Property == Control.ForegroundProperty, true, ref result);
            VerifySolidColorBrushSetter(setter, Colors.Red, ref result);

            setter = style.Setters[1] as Setter;
            VerifyElement.VerifyBool(setter.Property == Control.TemplateProperty, true, ref result);
            GlobalLog.LogStatus("Verifying template...");
            ControlTemplate template = setter.Value as ControlTemplate;
            VerifyElement.VerifyBool(template != null, true, ref result);
            if (!_noTargetType)
            {
                VerifyElement.VerifyBool(template.TargetType.Equals(typeof(Button)), true, ref result);
            }
            TriggerCollection triggers = template.Triggers;
            VerifyElement.VerifyBool(null != triggers, true, ref result);
            VerifyElement.VerifyInt(triggers.Count, 1, ref result);
            MultiTrigger trigger = triggers[0] as MultiTrigger;
            VerifyElement.VerifyBool(null != trigger, true, ref result);
            VerifyElement.VerifyInt(trigger.Conditions.Count, 1, ref result);
            VerifyElement.VerifyInt(trigger.Setters.Count, 1, ref result);
            setter = trigger.Setters[0] as Setter;
            VerifyElement.VerifyBool(setter.Property == Control.ForegroundProperty, true, ref result);
            VerifySolidColorBrushSetter(setter, Colors.Red, ref result);
            //Verify Style Triggers
            VerifyElement.VerifyInt(style.Triggers.Count, 2, ref result);
            trigger = style.Triggers[0] as MultiTrigger;
            VerifyElement.VerifyInt(trigger.Conditions.Count, 2, ref result);
            VerifyElement.VerifyInt(trigger.Setters.Count, 2, ref result);
            setter = trigger.Setters[0] as Setter;
            VerifyElement.VerifyBool(setter.Property == Control.BackgroundProperty, true, ref result);
            VerifySolidColorBrushSetter(setter, Colors.Green, ref result);
            setter = trigger.Setters[1] as Setter;
            VerifyElement.VerifyBool(setter.Property == Control.ForegroundProperty, true, ref result);
            VerifySolidColorBrushSetter(setter, Colors.Blue, ref result);

            Trigger trigger1 = style.Triggers[1] as Trigger;
            VerifyElement.VerifyInt(trigger1.Setters.Count, 1, ref result);
            setter = trigger1.Setters[0] as Setter;
            VerifyElement.VerifyBool(setter.Property == Control.BackgroundProperty, true, ref result);
            VerifySolidColorBrushSetter(setter, Colors.Red, ref result);
        }

        private static void VerifyExtendedStyleForFE(Style style, ref bool result)
        {
            if (!_noTargetType)
            {
                VerifyElement.VerifyBool(style.TargetType.Equals(typeof(Button)), true, ref result);
            }
            VerifyElement.VerifyBool(style.Setters != null, true, ref result);

            VerifyElement.VerifyInt(style.Setters.Count, 1, ref result);
            Setter setter = style.Setters[0] as Setter;
            VerifyElement.VerifyBool(setter.Property == Control.FontSizeProperty, true, ref result);
            VerifyElement.VerifyDouble((double)(setter.Value), 29.0, ref result);
        }

        private static void VerifySolidColorBrushSetter(Setter setter, Color color, ref bool result)
        {
            SolidColorBrush brush = setter.Value as SolidColorBrush;
            VerifyElement.VerifyBool(null != brush, true, ref result);
            VerifyElement.VerifyColor(brush.Color, color, ref result);
        }
    }
}
