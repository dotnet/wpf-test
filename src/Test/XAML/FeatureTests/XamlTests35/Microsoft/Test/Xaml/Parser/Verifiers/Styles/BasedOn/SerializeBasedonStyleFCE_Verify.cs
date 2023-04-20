// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;
using System.Windows;
using Microsoft.Test.Xaml.Utilities;
using System.Windows.Media;
using System.Windows.Documents;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Styles.BasedOn
{
    /// <summary/>
    public static class SerializeBasedonStyleFCE_Verify
    {
        /// <summary>
        /// NoTargetType
        /// </summary>
        internal static bool _noTargetType = false;

        /// <summary>
        /// Verifier for SerializeBasedonStyleFCE.xaml
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;
            GlobalLog.LogStatus("Verifying SerializeBasedonStyleFCE.xaml.");
            FrameworkElement root = rootElement as FrameworkElement;
            VerifyElement.VerifyBool(root != null, true, ref result);

            //Verify effect
            Bold bold1 = root.FindName("bold1") as Bold;
            Bold bold2 = root.FindName("bold2") as Bold;
            VerifyElement.VerifyBool(bold1 != null && bold2 != null, true, ref result);

            //Verify implicit BasedOn
            SolidColorBrush myForeground = bold1.Foreground as SolidColorBrush;
            VerifyElement.VerifyBool(null == myForeground, false, ref result);
            VerifyElement.VerifyColor(myForeground.Color, Colors.Red, ref result);

            double mySize = bold1.FontSize;
            VerifyElement.VerifyDouble(mySize, 29, ref result);

            myForeground = bold2.Foreground as SolidColorBrush;
            VerifyElement.VerifyBool(null == myForeground, false, ref result);
            VerifyElement.VerifyColor(myForeground.Color, Colors.Red, ref result);

            mySize = bold2.FontSize;
            VerifyElement.VerifyDouble(mySize, 29, ref result);

            //Verify styles
            Style extendedStyleMultipleFCE = root.Resources["extendedStyleMultipleFCE"] as Style;
            Style extendedStyleFCE         = root.Resources["extendedStyleFCE"] as Style;
            Style baseStyleFCE             = root.Resources["baseStyleFCE"] as Style;

            GlobalLog.LogStatus("Verifying base style for FCE...");
            VerifyElement.VerifyBool(baseStyleFCE != null, true, ref result);
            VerifyBaseStyleForFCE(baseStyleFCE, ref result);

            VerifyElement.VerifyBool(extendedStyleFCE != null, true, ref result);
            VerifyElement.VerifyBool(extendedStyleFCE.BasedOn != null, true, ref result);
            VerifyBaseStyleForFCE(extendedStyleFCE.BasedOn, ref result);

            VerifyElement.VerifyBool(extendedStyleMultipleFCE != null, true, ref result);
            VerifyElement.VerifyBool(extendedStyleMultipleFCE.BasedOn != null, true, ref result);
            VerifyElement.VerifyBool(extendedStyleMultipleFCE.BasedOn.BasedOn != null, true, ref result);
            VerifyBaseStyleForFCE(extendedStyleMultipleFCE.BasedOn.BasedOn, ref result);

            GlobalLog.LogStatus("Verifying extented style for FCE...");

            VerifyExtendedStyleForFCE(extendedStyleFCE, ref result);
            VerifyExtendedStyleForFCE(extendedStyleMultipleFCE.BasedOn, ref result);

            GlobalLog.LogStatus("Verifying the style based on the extented style for FCE.");

            if (!_noTargetType)
            {
                VerifyElement.VerifyBool(extendedStyleMultipleFCE.TargetType.Equals(typeof(Bold)), true, ref result);
            }
            VerifyElement.VerifyBool(extendedStyleMultipleFCE.Setters != null, true, ref result);
            VerifyElement.VerifyInt(extendedStyleMultipleFCE.Setters.Count, 1, ref result);
            Setter setter = extendedStyleMultipleFCE.Setters[0] as Setter;
            VerifyElement.VerifyBool(setter.Property == TextElement.BackgroundProperty, true, ref result);
            VerifySolidColorBrushSetter(setter, Colors.Green, ref result);
            _noTargetType = false; //reset flag
            return result;
        }

        private static void VerifyBaseStyleForFCE(Style style, ref bool result)
        {
            if (!_noTargetType)
            {
                VerifyElement.VerifyBool(style.TargetType.Equals(typeof(Bold)), true, ref result);
            }
            VerifyElement.VerifyBool(style.Setters != null, true, ref result);

            VerifyElement.VerifyInt(style.Setters.Count, 1, ref result);
            Setter setter = style.Setters[0] as Setter;
            VerifyElement.VerifyBool(setter.Property == TextElement.ForegroundProperty, true, ref result);
            VerifySolidColorBrushSetter(setter, Colors.Red, ref result);
        }

        private static void VerifyExtendedStyleForFCE(Style style, ref bool result)
        {
            if (!_noTargetType)
            {
                VerifyElement.VerifyBool(style.TargetType.Equals(typeof(Bold)), true, ref result);
            }
            VerifyElement.VerifyBool(style.Setters != null, true, ref result);

            VerifyElement.VerifyInt(style.Setters.Count, 1, ref result);
            Setter setter = style.Setters[0] as Setter;
            VerifyElement.VerifyBool(setter.Property == TextElement.FontSizeProperty, true, ref result);
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
