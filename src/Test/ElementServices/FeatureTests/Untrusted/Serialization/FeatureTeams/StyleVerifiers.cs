// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Threading;
using System.Timers;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Input;
using Avalon.Test.CoreUI.Common;
using Microsoft.Test.Windows;
using System.Globalization;


namespace Avalon.Test.CoreUI.Serialization
{
   /// <summary>
   /// Verify xaml files for Storyboard
   /// </summary>
   public class StyleVerifiers
   {
      /// <summary>
      /// Verifier for bvt\Serialization\AddOwnerKeyDown.xaml
      /// </summary>
       public static void AddOwnerKeyDown_Verify(UIElement uie)
      {
//         TimeSpan begin;
//         Duration duration;
//         String path;
//         object value;

         //
         // Verify logical structure
         //

         //
         // Root

         CoreLogger.LogStatus("Root element is a Page?");
         Page page = uie as Page;
         VerifyElement.VerifyBool(page != null, true);

         //
         // Style

         CoreLogger.LogStatus("1 Style exists?");
         VerifyElement.VerifyBool(page.Resources != null, true);
         VerifyElement.VerifyBool(page.Resources.Count == 2, true);

         CoreLogger.LogStatus("Style is of type Button.");
         Style styleForButton = page.Resources["styleInResourcesForUIE"] as Style;
         VerifyElement.VerifyBool(styleForButton != null, true);
         VerifyElement.VerifyBool(styleForButton.TargetType.Equals(typeof(Button)), true);

         VerifyStyleInAddOwnerKeyDown(styleForButton);
         CoreLogger.LogStatus("Style is of type Block.");
         Style styleForBlock = page.Resources["styleInResourcesForCE"] as Style;
         VerifyElement.VerifyBool(styleForBlock != null, true);
         VerifyElement.VerifyBool(styleForBlock.TargetType.Equals(typeof(Block)), true);
         VerifyStyleInAddOwnerKeyDown(styleForBlock);
      }
       private static void VerifyStyleInAddOwnerKeyDown(Style style)
       {
           CoreLogger.LogStatus("Verify style ...");
           TriggerCollection triggers = style.Triggers;
           VerifyElement.VerifyBool(triggers != null, true);
           VerifyElement.VerifyInt(triggers.Count, 1);

           EventTrigger trigger = triggers[0] as EventTrigger;
           VerifyElement.VerifyBool(trigger != null, true);

           RoutedEvent eventId= trigger.RoutedEvent;
           VerifyElement.VerifyBool(eventId == Keyboard.KeyDownEvent, true);

           TriggerActionCollection actions = trigger.Actions as TriggerActionCollection;
           VerifyElement.VerifyInt(actions.Count, 1);
           BeginStoryboard action = actions[0] as BeginStoryboard;
           VerifyElement.VerifyBool(action != null, true);
       }

       /// <summary>
       /// Verifier for SerializeBasedonStyleFE.xaml
       /// </summary>
       public static void SerializeBasedonStyleFE_Verify(UIElement uie)
       {
           CoreLogger.LogStatus("Verifying SerializeBasedonStyleFE.xaml.");
           FrameworkElement root = uie as FrameworkElement;
           VerifyElement.VerifyBool(root != null, true);

           //Verify effect
           Button button1 = root.FindName("button1") as Button;
           Button button2 = root.FindName("button2") as Button;
           VerifyElement.VerifyBool(button1 != null && button2 != null, true);

           SolidColorBrush myForeground = button1.Foreground as SolidColorBrush;
           VerifyElement.VerifyBool(null == myForeground, false);
           VerifyElement.VerifyColor(myForeground.Color, Colors.Red);

           double mySize = button1.FontSize;
           VerifyElement.VerifyDouble(mySize, 29);

           myForeground = button2.Foreground as SolidColorBrush;
           VerifyElement.VerifyBool(null == myForeground, false);
           VerifyElement.VerifyColor(myForeground.Color, Colors.Red);

           mySize = button2.FontSize;
           VerifyElement.VerifyDouble(mySize, 29);
           //Verify styles
           Style extendedStyleMultipleFE = root.Resources["extendedStyleMultipleFE"] as Style;
           Style extendedStyleFE = root.Resources["extendedStyleFE"] as Style;
           Style baseStyleFE = root.Resources["baseStyleFE"] as Style;

           VerifyElement.VerifyBool(baseStyleFE != null, true);
           
           VerifyElement.VerifyBool(extendedStyleFE != null, true);
           VerifyElement.VerifyBool(extendedStyleFE.BasedOn != null, true);

           VerifyElement.VerifyBool(extendedStyleMultipleFE != null, true);
           VerifyElement.VerifyBool(extendedStyleMultipleFE.BasedOn != null, true);
           VerifyElement.VerifyBool(extendedStyleMultipleFE.BasedOn.BasedOn != null, true);
           
           CoreLogger.LogStatus("Verifying base style...");
           VerifyBaseStyleForFE(baseStyleFE);
           VerifyBaseStyleForFE(extendedStyleFE.BasedOn);
           VerifyBaseStyleForFE(extendedStyleMultipleFE.BasedOn.BasedOn);

           CoreLogger.LogStatus("Verifying extented style for FE...");

           VerifyExtendedStyleForFE(extendedStyleFE);
           VerifyExtendedStyleForFE(extendedStyleMultipleFE.BasedOn);

           CoreLogger.LogStatus("Verifying the style based on the extented style for FE...");

           if (!s_noTargetType)
                VerifyElement.VerifyBool(extendedStyleMultipleFE.TargetType.Equals(typeof(Button)), true);
           VerifyElement.VerifyBool(extendedStyleMultipleFE.Setters != null, true);
           VerifyElement.VerifyInt(extendedStyleMultipleFE.Setters.Count, 1);
           Setter setter = extendedStyleMultipleFE.Setters[0] as Setter;
           VerifyElement.VerifyBool(setter.Property == Control.BackgroundProperty, true);
           VerifySolidColorBrushSetter(setter, Colors.Green);
       }


       private static void VerifyExtendedStyleForFE(Style style)
       {
           if (!s_noTargetType)
                VerifyElement.VerifyBool(style.TargetType.Equals(typeof(Button)), true);
           VerifyElement.VerifyBool(style.Setters != null, true);

           VerifyElement.VerifyInt(style.Setters.Count, 1);
           Setter setter = style.Setters[0] as Setter;
           VerifyElement.VerifyBool(setter.Property == Control.FontSizeProperty, true);
           VerifyElement.VerifyDouble((double)(setter.Value), 29.0);
       }

       private static void VerifyBaseStyleForFE(Style style)
       {
           if (!s_noTargetType)
                VerifyElement.VerifyBool(style.TargetType.Equals(typeof(Button)), true);
           VerifyElement.VerifyBool(style.Setters != null, true);

           VerifyElement.VerifyInt(style.Setters.Count, 2);
           Setter setter = style.Setters[0] as Setter;
           VerifyElement.VerifyBool(setter.Property == Control.ForegroundProperty, true);
           VerifySolidColorBrushSetter(setter, Colors.Red);

           setter = style.Setters[1] as Setter;
           VerifyElement.VerifyBool(setter.Property == Control.TemplateProperty, true);
           CoreLogger.LogStatus("Verifying template...");
           ControlTemplate template = setter.Value as ControlTemplate;
           VerifyElement.VerifyBool(template != null, true);
           if (!s_noTargetType)
                VerifyElement.VerifyBool(template.TargetType.Equals(typeof(Button)), true);
           TriggerCollection triggers = template.Triggers;
           VerifyElement.VerifyBool(null != triggers, true);
           VerifyElement.VerifyInt(triggers.Count, 1);
           MultiTrigger trigger = triggers[0] as MultiTrigger;
           VerifyElement.VerifyBool(null != trigger, true);
           VerifyElement.VerifyInt(trigger.Conditions.Count, 1);
           VerifyElement.VerifyInt(trigger.Setters.Count, 1);
           setter = trigger.Setters[0] as Setter;
           VerifyElement.VerifyBool(setter.Property == Control.ForegroundProperty, true);
           VerifySolidColorBrushSetter(setter, Colors.Red);
           //Verify Style Triggers
           VerifyElement.VerifyInt(style.Triggers.Count, 2);
           trigger = style.Triggers[0] as MultiTrigger;
           VerifyElement.VerifyInt(trigger.Conditions.Count, 2);
           VerifyElement.VerifyInt(trigger.Setters.Count, 2);
           setter = trigger.Setters[0] as Setter;
           VerifyElement.VerifyBool(setter.Property == Control.BackgroundProperty, true);
           VerifySolidColorBrushSetter(setter, Colors.Green);
           setter = trigger.Setters[1] as Setter;
           VerifyElement.VerifyBool(setter.Property == Control.ForegroundProperty, true);
           VerifySolidColorBrushSetter(setter, Colors.Blue);

           Trigger trigger1 = style.Triggers[1] as Trigger;
           VerifyElement.VerifyInt(trigger1.Setters.Count, 1);
           setter = trigger1.Setters[0] as Setter;
           VerifyElement.VerifyBool(setter.Property == Control.BackgroundProperty, true);
           VerifySolidColorBrushSetter(setter, Colors.Red);
       }

       /// <summary>
       /// Verifier for SerializeBasedonStyleFCE.xaml
       /// </summary>
       public static void SerializeBasedonStyleFCE_Verify(UIElement uie)
       {
           CoreLogger.LogStatus("Verifying SerializeBasedonStyleFCE.xaml.");
           FrameworkElement root = uie as FrameworkElement;
           VerifyElement.VerifyBool(root != null, true);

           //Verify effect
           Bold bold1 = root.FindName("bold1") as Bold;
           Bold bold2 = root.FindName("bold2") as Bold;
           VerifyElement.VerifyBool(bold1 != null && bold2 != null, true);

           //Verify implicit BasedOn
           SolidColorBrush myForeground = bold1.Foreground as SolidColorBrush;
           VerifyElement.VerifyBool(null == myForeground, false);
           VerifyElement.VerifyColor(myForeground.Color, Colors.Red);

           double mySize = bold1.FontSize;
           VerifyElement.VerifyDouble(mySize, 29);

           myForeground = bold2.Foreground as SolidColorBrush;
           VerifyElement.VerifyBool(null == myForeground, false);
           VerifyElement.VerifyColor(myForeground.Color, Colors.Red);

           mySize = bold2.FontSize;
           VerifyElement.VerifyDouble(mySize, 29);

           //Verify styles
           Style extendedStyleMultipleFCE = root.Resources["extendedStyleMultipleFCE"] as Style;
           Style extendedStyleFCE = root.Resources["extendedStyleFCE"] as Style;
           Style baseStyleFCE = root.Resources["baseStyleFCE"] as Style;

           CoreLogger.LogStatus("Verifying base style for FCE...");
           VerifyElement.VerifyBool(baseStyleFCE != null, true);
           VerifyBaseStyleForFCE(baseStyleFCE);

           VerifyElement.VerifyBool(extendedStyleFCE != null, true);
           VerifyElement.VerifyBool(extendedStyleFCE.BasedOn != null, true);
           VerifyBaseStyleForFCE(extendedStyleFCE.BasedOn);

           VerifyElement.VerifyBool(extendedStyleMultipleFCE != null, true);
           VerifyElement.VerifyBool(extendedStyleMultipleFCE.BasedOn != null, true);
           VerifyElement.VerifyBool(extendedStyleMultipleFCE.BasedOn.BasedOn != null, true);
           VerifyBaseStyleForFCE(extendedStyleMultipleFCE.BasedOn.BasedOn);

           CoreLogger.LogStatus("Verifying extented style for FCE...");

           VerifyExtendedStyleForFCE(extendedStyleFCE);
           VerifyExtendedStyleForFCE(extendedStyleMultipleFCE.BasedOn);

           CoreLogger.LogStatus("Verifying the style based on the extented style for FCE.");

           if (!s_noTargetType)
                VerifyElement.VerifyBool(extendedStyleMultipleFCE.TargetType.Equals(typeof(Bold)), true);
           VerifyElement.VerifyBool(extendedStyleMultipleFCE.Setters != null, true);
           VerifyElement.VerifyInt(extendedStyleMultipleFCE.Setters.Count, 1);
           Setter setter = extendedStyleMultipleFCE.Setters[0] as Setter;
           VerifyElement.VerifyBool(setter.Property == TextElement.BackgroundProperty, true);
           VerifySolidColorBrushSetter(setter, Colors.Green);
       }

       private static void VerifyExtendedStyleForFCE(Style style)
       {
           if (!s_noTargetType)
                VerifyElement.VerifyBool(style.TargetType.Equals(typeof(Bold)), true);
           VerifyElement.VerifyBool(style.Setters != null, true);

           VerifyElement.VerifyInt(style.Setters.Count, 1);
           Setter setter = style.Setters[0] as Setter;
           VerifyElement.VerifyBool(setter.Property == TextElement.FontSizeProperty, true);
           VerifyElement.VerifyDouble((double)(setter.Value), 29.0);
       }

       private static void VerifyBaseStyleForFCE(Style style)
       {
           if (!s_noTargetType)
                VerifyElement.VerifyBool(style.TargetType.Equals(typeof(Bold)), true);
           VerifyElement.VerifyBool(style.Setters != null, true);

           VerifyElement.VerifyInt(style.Setters.Count, 1);
           Setter setter = style.Setters[0] as Setter;
           VerifyElement.VerifyBool(setter.Property == TextElement.ForegroundProperty, true);
           VerifySolidColorBrushSetter(setter, Colors.Red);
       }
       private static void VerifySolidColorBrushSetter(Setter setter, Color color)
       {
           SolidColorBrush brush = setter.Value as SolidColorBrush;
           VerifyElement.VerifyBool(null != brush, true);
           VerifyElement.VerifyColor(brush.Color, color);
       }



       /// <summary>
       /// Verifier for SerializeBasedonStyleNoTargetType.xaml
       /// </summary>
       public static void SerializeBasedonStyleNoTargetType_Verify(UIElement uie)
       {
           s_noTargetType = true;
           SerializeBasedonStyleFE_Verify(uie);
           SerializeBasedonStyleFCE_Verify(uie);
       }
       static bool s_noTargetType = false;
   }
}
