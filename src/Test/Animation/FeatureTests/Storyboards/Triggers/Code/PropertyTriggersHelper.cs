// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* ***************  Animation Integration Test *****************
*     Purpose:          Helper functions for PropertyTrigger testing
*     Area:             Animation       
*     Dependencies:     TestRuntime.dll
*     Support Files:    

**********************************************************/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.Input;   //UserInput


namespace Microsoft.Test.Animation
{

    public class PropertyTriggersHelper
    {
        /******************************************************************************
        * Function:          TriggerAnimation
        ******************************************************************************/
        /// <summary>
        /// Establish an Animation.
        /// </summary>
        /// <param name="inputString">the property to be animated</param>
        /// <param name="button">the element on which the animation will take place</param>
        /// <param name="beginTime">the begin time of the animation</param>
        /// <param name="durationTime">the duration of the animation</param>
        /// <param name="mouseMoved">indicates whether or not the mouse moved</param>
        /// <param name="expectTriggering">indicates whether or not the animation can be validated</param>
        public static void TriggerAnimation(string inputString, Button button, TimeSpan beginTime, Duration durationTime, ref bool mouseMoved, ref bool expectTriggering)
        {
            Trigger trigger = new Trigger();

            inputString = inputString.ToLower(CultureInfo.InvariantCulture);
            try
            {
                switch (inputString)
                {
                    //TEST 1: WIDTH -- Animate when the Width property changes.
                    case "width":
                        trigger.Property    = Button.WidthProperty;
                        trigger.Value       = 150d;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.Width        = (double)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;

                    //TEST 2: ISDEFAULT -- Animate when the IsDefault property is set to true.
                    case "isdefaulttrue":
                        trigger.Property    = Button.IsDefaultProperty;
                        trigger.Value       = true;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.IsDefault    = (bool)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;

                    //TEST 3: ISDEFAULT -- Not Animate when the IsDefault property is set to false.
                    case "isdefaultfalse1":
                        //Now expect trigger to work 

                        trigger.Property    = Button.IsDefaultProperty;
                        trigger.Value       = false;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.IsDefault    = (bool)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;

                    //TEST 4: ISDEFAULT -- Animate when the IsDefault property is set to false, after being set to true.
                    case "isdefaultfalse2":
                        button.IsDefault    = true; //Need to first change the default value.
                        trigger.Property    = Button.IsDefaultProperty;
                        trigger.Value       = false;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.IsDefault    = (bool)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;

                    //TEST 5: ISCANCEL -- Animate when the IsCancel property is changed.
                    case "iscancel":
                        trigger.Property    = Button.IsCancelProperty;
                        trigger.Value       = true;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.IsCancel     = (bool)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;

                    //TEST 6: ISPRESSED -- Animate when IsPressed changes due to a button click.
                    case "ispressed":
                        trigger.Property    = Button.IsPressedProperty;
                        trigger.Value       = true;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        mouseMoved = true;
                        UserInput.MouseLeftClickCenter(button);  //TRIGGER THE ANIMATION.
                        break;

                    //TEST 7: CLICKMODE -- Animate when the ClickMode property is changed.
                    case "clickmode":
                        trigger.Property    = Button.ClickModeProperty;
                        trigger.Value       = ClickMode.Hover;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.ClickMode     = (ClickMode)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;

                    //TEST 8: C0NTENT -- Animate when the Content property is changed.
                    case "content":
                        trigger.Property    = Button.ContentProperty;
                        trigger.Value       = "New Content";
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.Content      = (string)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;

                    //TEST 9: HASCONTENT -- Animate when the HasContent property is changed.
                    case "hascontent":
                        trigger.Property    = Button.HasContentProperty;
                        trigger.Value       = true;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.Content      = "New Content";  //TRIGGER THE ANIMATION.
                        break;

                    //TEST 10: BACKGROUND -- Animate when the Background property is changed.
                    case "background":
                        button.Background   = Brushes.Orange;
                        trigger.Property    = Button.BackgroundProperty;
                        trigger.Value       = Brushes.Blue;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.Background   = (Brush)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;

                    //TEST 11: BORDERBRUSH -- Animate when the BorderBrush property is changed.
                    case "borderbrush":
                        button.BorderThickness  = new Thickness(5,5,5,5);
                        trigger.Property        = Button.BorderBrushProperty;
                        trigger.Value           = Brushes.Red;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.BorderBrush      = (Brush)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;

                    //TEST 12: BORDERTHICKNESS -- Animate when the BorderThickness property is changed.
                    case "borderthickness":
                        button.BorderBrush      = Brushes.Yellow;
                        trigger.Property        = Button.BorderThicknessProperty;
                        trigger.Value           = new Thickness(5,5,5,5);
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.BorderThickness  = (Thickness)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;

                    //TEST 13: FONTFAMILY -- Animate when the FontFamily property is changed.
                    case "fontfamily":
                        button.Content          = "Avalon";
                        trigger.Property        = Button.FontFamilyProperty;
                        trigger.Value           = new FontFamily("Trebuchet");
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.FontFamily       = (FontFamily)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;

                    //TEST 14: FONTSIZE -- Animate when the FontSize property is changed.
                    case "fontsize":
                        button.Content          = "Avalon";
                        trigger.Property        = Button.FontSizeProperty;
                        trigger.Value           = 12d;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.FontSize         = (double)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;

                    //TEST 15: FONTSTRETCH -- Animate when the FontStretch property is changed.
                    case "fontstretch":
                        button.Content          = "Avalon";
                        trigger.Property        = Button.FontStretchProperty;
                        trigger.Value           = FontStretches.Condensed;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.FontStretch      = (FontStretch)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;

                    //TEST 16: FONTSTYLE -- Animate when the FontStyle property is changed.
                    case "fontstyle":
                        button.Content          = "Avalon";
                        trigger.Property        = Button.FontStyleProperty;
                        trigger.Value           = FontStyles.Italic;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.FontStyle        = (FontStyle)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;

                    //TEST 17: FONTWEIGHT -- Animate when the FontWeight property is changed.
                    case "fontweight":
                        button.Content          = "Avalon";
                        trigger.Property        = Button.FontWeightProperty;
                        trigger.Value           = FontWeights.Thin;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.FontWeight  = (FontWeight)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;

                    //TEST 18: FOREGROUND -- Animate when the Foreground property is changed.
                    case "foreground":
                        button.Content          = "Avalon";
                        trigger.Property        = Button.ForegroundProperty;
                        trigger.Value           = Brushes.White;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.Foreground       = (Brush)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;

                    //TEST 19: HORIZONTALCONTENTALIGNMENT -- Animate when the HorizontalContentAlignment property is changed.
                    case "horizontalcontentalignment":
                        button.Content          = "Avalon";
                        trigger.Property        = Button.HorizontalContentAlignmentProperty;
                        trigger.Value           = HorizontalAlignment.Right;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.HorizontalContentAlignment  = (HorizontalAlignment)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;

                    //TEST 20: ISTABSTOP -- Animate when the IsTabStop property is changed.
                    case "istabstop":
                        trigger.Property        = Button.IsTabStopProperty;
                        trigger.Value           = false;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.IsTabStop        = (bool)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;

                    //TEST 21: PADDING -- Animate when the Padding property is changed.
                    case "padding":
                        button.Content          = "Avalon";
                        trigger.Property        = Button.PaddingProperty;
                        trigger.Value           = new Thickness(5,5,5,5);
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.Padding        = (Thickness)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;

                    //TEST 22: TABINDEX -- Animate when the TabIndex property is changed.
                    case "tabindex":
                        trigger.Property        = Button.TabIndexProperty;
                        trigger.Value           = 3;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.TabIndex        = (Int32)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;
                        
                    //TEST 23: COMMANDTARGET -- Animate when the CommandTarget property is changed.
                    case "commandtarget":
                        TextBox textbox         = new TextBox();
                        trigger.Property        = Button.CommandTargetProperty;
                        trigger.Value           = textbox;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.CommandTarget    = (IInputElement)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;
                        
                    //TEST 24: ACTUALHEIGHT -- Animate when the ActualHeight property is changed.
                    case "actualheight":
                        trigger.Property        = Button.ActualHeightProperty;
                        trigger.Value           = 150d;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.Height           = (double)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;
                        
                    //TEST 25: CONTEXTMENU -- Animate when the ContextMenu property is changed.
                    case "contextmenu":
                        ContextMenu cm          = new ContextMenu();
                        trigger.Property        = Button.ContextMenuProperty;
                        trigger.Value           = cm;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.ContextMenu  = (ContextMenu)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;
                        
                    //TEST 26: CURSOR -- Animate when the Cursor property is changed.
                    case "cursor":
                        trigger.Property        = Button.CursorProperty;
                        trigger.Value           = Cursors.Hand;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.Cursor  = (Cursor)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;
                        
                    //TEST 27: FOCUSABLE -- Animate when the Focusable property is changed.
                    case "focusable":
                        trigger.Property        = Button.FocusableProperty;
                        trigger.Value           = false;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.Focusable  = (bool)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;
                        
                    //TEST 28: LAYOUTTRANSFORM -- Animate when the LayoutTransform property is changed.
                    case "layouttransform":
                        trigger.Property        = Button.LayoutTransformProperty;
                        trigger.Value           = new ScaleTransform(2,2);
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.LayoutTransform  = (Transform)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;
                        
                    //TEST 29: MARGIN -- Animate when the Margin property is changed.
                    case "margin":
                        trigger.Property        = Button.MarginProperty;
                        trigger.Value           = new Thickness(5,5,5,5);
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.Margin  = (Thickness)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;
                        
                    //TEST 30: MAXWIDTH -- Animate when the MaxWidth property is changed.
                    case "maxwidth":
                        trigger.Property        = Button.MaxWidthProperty;
                        trigger.Value           = 250d;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.MaxWidth  = (double)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;
                        
                    //TEST 31: NAME -- Animate when the Name property is changed.
                    case "name":
                        trigger.Property        = Button.NameProperty;
                        trigger.Value           = "NewName";
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.Name  = (string)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;
                        
                    //TEST 32: OVERRIDESDEFAULTSTYLE -- Animate when the OverridesDefaultStyle property is changed.
                    case "overridesdefaultstyle":
                        trigger.Property                = Button.OverridesDefaultStyleProperty;
                        trigger.Value                   = false;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.OverridesDefaultStyle    = (bool)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;
                        
                    //TEST 33: TAG -- Animate when the Tag property is changed.
                    case "tag":
                        trigger.Property            = Button.TagProperty;
                        trigger.Value               = "NewTag";
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.Tag                  = (string)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;
                        
                    //TEST 34: TOOLTIP -- Animate when the ToolTip property is changed.
                    case "tooltip":
                        ToolTip tt = new ToolTip();
                        tt.Content = "Howdy";
                        trigger.Property            = Button.ToolTipProperty;
                        trigger.Value               = tt;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.ToolTip              = (ToolTip)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;
                        
                    //TEST 35: VERTICALALIGNMENT -- Animate when the VerticalAlignment property is changed.
                    case "verticalalignment":
                        trigger.Property            = Button.VerticalAlignmentProperty;
                        trigger.Value               = VerticalAlignment.Top;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.VerticalAlignment    = (VerticalAlignment)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;
                        
                    //TEST 36: FLOWDIRECTION -- Animate when the FlowDirection property is changed.
                    case "flowdirection":
                        button.Content              = "Avalon";
                        button.FontSize             = 24d;
                        trigger.Property            = Button.FlowDirectionProperty;
                        trigger.Value               = FlowDirection.RightToLeft;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.FlowDirection        = (FlowDirection)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;
                        
                    //TEST 37: ISMOUSEOVER -- Animate when the IsMouseOver property is changed.
                    case "ismouseover":
                        trigger.Property            = Button.IsMouseOverProperty;
                        trigger.Value               = true;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        mouseMoved = true;
                        UserInput.MouseMove(button,20,20);  //TRIGGER THE ANIMATION.
                        break;
                        
                    //TEST 38: ISENABLED -- Animate when the IsEnabled property is changed.
                    case "isenabled":
                        button.IsEnabled            = false;
                        trigger.Property            = Button.IsEnabledProperty;
                        trigger.Value               = true;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.IsEnabled            = (bool)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;
                        
                    //TEST 39: ISFOCUSED -- Animate when IsFocused changes due to clicking on the Button.
                    case "isfocused":
                        trigger.Property            = Button.IsFocusedProperty;
                        trigger.Value               = true;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        mouseMoved = true;
                        UserInput.MouseLeftClickCenter(button);  //TRIGGER THE ANIMATION.
                        break;
                        
                    //TEST 40: ISMOUSEOVER -- Animate when the mouse moves over the Button.
                    case "ismouseover2":
                        trigger.Property            = UIElement.IsMouseOverProperty;
                        trigger.Value               = true;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        mouseMoved = true;
                        UserInput.MouseMove(button,20,20);   //TRIGGER THE ANIMATION.
                        break;
                        
                    //TEST 41: ISVISIBLE -- Animate when the IsVisible property is set to true.
                    case "isvisible":
                        button.Visibility           = Visibility.Hidden;
                        trigger.Property            = Button.IsVisibleProperty;
                        trigger.Value               = true;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.Visibility           = Visibility.Visible;  //TRIGGER THE ANIMATION.
                        break;
                        
                    //TEST 42: OPACITY -- Animate when the Opacity property is set to 0.
                    case "opacity0":
                        expectTriggering            = false; //Animation is not visible in this case.
                        trigger.Property            = UIElement.OpacityProperty;
                        trigger.Value               = 0d;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.Opacity              = (double)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;
                        
                    //TEST 43: OPACITY -- Animate when the Opacity property is set to 1.
                    case "opacity1":
                        //Now expect trigger to work 

                        trigger.Property            = UIElement.OpacityProperty;
                        trigger.Value               = 1d;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.Opacity              = (double)trigger.Value;
                        break;
                        
                    //TEST 44: OPACITY -- Animate when the Opacity property is set to 0, then 1.
                    case "opacity2":
                        button.Opacity              = 0;
                        trigger.Property            = UIElement.OpacityProperty;
                        trigger.Value               = 1d;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.Opacity              = (double)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;
                        
                    //TEST 45: RENDERTRANSFORM -- Animate when the RenderTransform property is changed.
                    case "rendertransform":
                        trigger.Property        = Button.RenderTransformProperty;
                        trigger.Value           = new SkewTransform(2,2);
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.RenderTransform  = (Transform)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;
                        
                    //TEST 46: SNAPSTODEVICEPIXELS -- Animate when the SnapsToDevicePixels property is changed.
                    case "snapstodevicepixels":
                        trigger.Property            = Button.SnapsToDevicePixelsProperty;
                        trigger.Value               = true;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.SnapsToDevicePixels  = (bool)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;
                        
                    //TEST 47: ISHITTESTVISIBLE -- Animate when the IsHitTestVisible property is changed.
                    case "ishittestvisible":
                        trigger.Property            = Button.IsHitTestVisibleProperty;
                        trigger.Value               = false;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        button.IsHitTestVisible  = (bool)trigger.Value;  //TRIGGER THE ANIMATION.
                        break;
                        
                    //TEST 48: BACKGROUNDOPACITY -- Animate when the IsHitTestVisible property is changed.
                    case "backgroundopacity":
                        SolidColorBrush scb = new SolidColorBrush();
                        scb.Color   = Colors.Blue;
                        scb.Opacity = 0d;
                        button.Background   = scb;
                        trigger.Property    = SolidColorBrush.OpacityProperty;
                        trigger.Value       = 1d;
                        SetUpAnimation(trigger, button, beginTime, durationTime);
                        scb.Opacity = 1d;
                        button.Background   = scb;  //TRIGGER THE ANIMATION.
                        break;

                    default:
                        throw new TestSetupException("ERROR!!! TriggerAnimation: Incorrect argument.\n");
                }
            }
            catch (Exception e1)
            {
                throw new TestValidationException("ERROR!!! TriggerAnimation: Exception 1 caught:" + e1.Message + "\n");
            }
        }

        /******************************************************************************
        * Function:          SetUpAnimation
        ******************************************************************************/
        /// <summary>
        /// SetUpAnimation: create the Button animation and storyboard for a Trigger.
        /// </summary>
        /// <param name="trigger">a property Trigger</param>
        /// <param name="button">the Button to which the Style is applied</param>
        /// <param name="beginTime">the begin time of the animation</param>
        /// <param name="durationTime">the duration of the animation</param>
        /// <param name="eventFired">indicates whether or not CurrentStateInvalidated fired</param>
        public static void SetUpAnimation(Trigger trigger, Button button, TimeSpan beginTime, Duration durationTime)
        {
            DoubleAnimation anim = new DoubleAnimation();
            anim.BeginTime            = beginTime;
            anim.Duration             = durationTime;
            anim.To                   = 0;
            anim.CurrentStateInvalidated += new EventHandler(OnCurrentStateInvalidated);

            Storyboard story = new Storyboard();
            story.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(0).(1)", Button.RenderTransformProperty, TranslateTransform.YProperty));
            story.Children.Add(anim);

            BeginStoryboard beginStoryboard = new BeginStoryboard();
            beginStoryboard.Storyboard = story;

            trigger.EnterActions.Add(beginStoryboard);

            Style style = new Style();
            style.TargetType = typeof(Button);
            style.Triggers.Add(trigger);

            button.Style = style;
        }

        /******************************************************************************
           * Function:          OnCurrentStateInvalidated
           ******************************************************************************/
        /// <summary>
        /// OnCurrentStateInvalidated: Used to validate that the animation began.
        /// </summary>
        private static void OnCurrentStateInvalidated(object sender, EventArgs e)
        {               
           PropertyTriggersTest.eventFired = true;
        }
        
        /******************************************************************************
           * Function:          CheckColor
           ******************************************************************************/
        /// <summary>
        /// CheckColor: check the actual vs. expected Color.
        /// </summary>
        /// <param name="x1">the first x coordinate at which the color is verified</param>
        /// <param name="y1">the first y coordinate at which the color is verified</param>
        /// <param name="x2">the second x coordinate at which the color is verified</param>
        /// <param name="y2">the second y coordinate at which the color is verified</param>
        /// <param name="expColor">the color value expected</param>
        /// <param name="verifier">a verifier object</param>
        /// <param name="outputString">message string</param>
        public static bool CheckColor(int x1, int y1, int x2, int y2, Color expColor, VisualVerifier verifier, ref StringBuilder outputString)
        {
            bool passed1        = true;
            bool passed2        = true;
            Color actColor;
            
            //At each Tick, check the color at two different points (x1,y1) and (x2,y2) on the page.
            actColor = verifier.getColorAtPoint(x1, y1);
            passed1 = CompareColors(x1, y1, expColor, actColor, ref outputString);
            
            actColor = verifier.getColorAtPoint(x2, y2);
            passed2 = CompareColors(x2, y2, expColor, actColor, ref outputString);

            return (passed1 && passed2);
        }

        /******************************************************************************
           * Function:          CompareColors
           ******************************************************************************/
        /// <summary>
        /// CompareColors: compares actual and expected Colors.
        /// </summary>
        /// <param name="x">the x coordinate at which the color is verified</param>
        /// <param name="y">the y coordinate at which the color is verified</param>
        /// <param name="expColor">expected Color value</param>
        /// <param name="actColor">actual Color value</param>
        /// <param name="outputString">message string</param>
        public static bool CompareColors(int x, int y, Color expColor, Color actColor, ref StringBuilder outputString)
        {
            bool colorMatched   = true;
            float expTolerance  = .15f;
            
            outputString.Append("\n---------- Result at (" + x + "," + y + ") ------\n");
            outputString.Append("\n Actual   : " + actColor.ToString() + "\n");
            outputString.Append("\n Expected : " + expColor.ToString() + "\n");

            if (Math.Abs(Math.Round((double)(Decimal.Round(actColor.R,3) - expColor.R) / expColor.R,4)) >= expTolerance)
            { 
                colorMatched = false; 
            }
            if (Math.Abs(Math.Round((double)(Decimal.Round(actColor.G,3) - expColor.G) / expColor.G,4)) >= expTolerance) 
            { 
                colorMatched = false; 
            }
            if (Math.Abs(Math.Round((double)(Decimal.Round(actColor.B,3) - expColor.B) / expColor.B,4)) >= expTolerance) 
            { 
                colorMatched = false; 
            }
            return colorMatched;
        }
    }
}
