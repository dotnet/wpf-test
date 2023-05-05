// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/********************************************************************************************
********************************************************************************************/
using System;
using System.ComponentModel;
using System.Reflection;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    class VerifyGetValue : NavigationWindow
    {
        private static FrameworkElement  s_root;

        private static double            s_expectedDouble;
        private static Color             s_expectedColor;
        private static Rect              s_expectedRect;
        private static Point             s_expectedPoint;
        private static Thickness         s_expectedThickness;
        private static Size              s_expectedSize;
        private static Int32             s_expectedInt32;
        private static string            s_expectedString;
        private static bool              s_expectedBoolean;
        
        private static string            s_animationType      = "";
        private static string            s_animationValue     = "";
        private static string            s_animationDP        = "";
        
        private static string            s_outputMessages;


        /******************************************************************************
           * Function:          FindTag
           ******************************************************************************/
        /// <summary>
        /// FindTag: Retrieves the root element and returns the value of the Tag property,
        /// if it exists. ASSUMPTION: the root element's Name is "Root".
        /// </summary>
        /// <returns>An error string if an error occurs. A boolean returned ByRef, indicating if a Tag was found</returns>
        public static string FindTag(NavigationWindow navWin, ref bool tagFound)
        {
            string errMessage = "";
            tagFound = false;

            s_root = (FrameworkElement)LogicalTreeHelper.FindLogicalNode((DependencyObject)navWin.Content,"Root");
            if (s_root != null)
            {
                if (s_root.Tag != null)
                {
                    string tag = (string)s_root.Tag;
                    string [] parms = tag.Split(' ');
                    if (parms.Length != 3)
                    {
                        errMessage = "ERROR!!! FindTag: Incorrect number of values specified on the Tag property";
                    }
                    else
                    {
                        s_animationType  = parms[0];
                        s_animationValue = parms[1];
                        s_animationDP    = parms[2];
                        
                        if (s_animationType != "")
                        {
                            s_animationType = s_animationType.ToLower(CultureInfo.InvariantCulture);
                            switch (s_animationType)
                            {
                                case "double":
                                    s_expectedDouble = (double)Convert.ToDouble(s_animationValue, CultureInfo.InvariantCulture);
                                    break;
                                case "color":
                                    s_expectedColor = (Color)ColorConverter.ConvertFromString(s_animationValue);
                                    break;
                                case "rect":
                                    RectConverter rectConverter = new RectConverter();
                                    s_expectedRect = (Rect)rectConverter.ConvertFrom(null, CultureInfo.InvariantCulture, s_animationValue);
                                    break;
                                case "point":
                                    PointConverter pointConverter = new PointConverter();
                                    s_expectedPoint = (Point)pointConverter.ConvertFrom(null, CultureInfo.InvariantCulture, s_animationValue);
                                    break;
                                case "thickness":
                                    ThicknessConverter thicknessConverter = new ThicknessConverter();
                                    s_expectedThickness = (Thickness)thicknessConverter.ConvertFrom(null, CultureInfo.InvariantCulture, s_animationValue);
                                    break;
                                case "size":
                                    SizeConverter sizeConverter = new SizeConverter();
                                    s_expectedSize = (Size)sizeConverter.ConvertFrom(null, CultureInfo.InvariantCulture, s_animationValue);
                                    break;
                                case "int32":
                                    s_expectedInt32 = Convert.ToInt32(s_animationValue);
                                    break;
                                case "string":
                                    StringConverter stringConverter = new StringConverter();
                                    s_expectedString = (string)stringConverter.ConvertFrom(null, CultureInfo.InvariantCulture, s_animationValue);
                                    break;
                                case "boolean":
                                    BooleanConverter booleanConverter = new BooleanConverter();
                                    s_expectedBoolean = (bool)booleanConverter.ConvertFrom(null, CultureInfo.InvariantCulture, s_animationValue);
                                    break;
                                default:
                                    errMessage = "ERROR!!! FindTag: The animationType was incorrectly read from Markup";
                                    break;
                            }
                            tagFound = true;
                        }
                        else
                        {
                            errMessage = "ERROR!!! FindTag: No animationType was read from Markup";
                        }
                    }
                }
                else
                {
                    GlobalLog.LogEvidence("----No Tag found, GetValue not verified---");
                }
            }
            else
            {
                GlobalLog.LogEvidence("----No Root element found, GetValue not verified---");
            }
            
            return errMessage;
        }

        /******************************************************************************
           * Function:          VerifyValue
           ******************************************************************************/
        /// <summary>
        /// VerifyValue: Determines whether or not the expected value (specified by a Tag property
        /// on the root element) matches the actual value of the animated property.
        /// This verification will not take place unless the markup contains a Tag property on the root.
        /// Assumption: if a Tag on the root exists, the Markup contains a to-be-animated 
        /// object that is named either
        ///   o "AnimatedFE"        -- the to-be-animated object is a FrameworkElement
        ///   o "AnimatedAnimatable -- the to-be-animated object is an Animatable
        ///   o "AnimateTemplate"   -- "AnimatedFE" or "AnimatedAnimatable" is in a ControlTemplate
        ///                            based on a ContentControl named "TemplateControl"
        /// NOT SUPPORTED: an Animatable located in a Style's Setter.
        /// </summary>
        /// <returns>A boolean indicating whether or not the verification succeeded</returns>
        public static bool VerifyValue(NavigationWindow navWin, ref string outputData)
        {
            bool                result              = false;
            DependencyObject    animatedDO          = null;
            DependencyObject    controlInTemplate   = null;
            Animatable          animatable          = null;
            
            s_outputMessages = outputData;
            
            if (s_animationType == "")
            {
                //No Tag property exists in the Markup, so the DP change is not verified.
                result = true;
            }
            else
            {
                //Return an element named "AnimatedFE", if any.
                animatedDO = (DependencyObject)LogicalTreeHelper.FindLogicalNode((DependencyObject)navWin.Content,"AnimatedFE");
                
                //Return an element named "TemplateControl", associated with "AnimatedFE", if any.
                DependencyObject control = (DependencyObject)LogicalTreeHelper.FindLogicalNode((DependencyObject)navWin.Content,"AnimateTemplate");
                controlInTemplate = GetTemplateControl(control);

                if (controlInTemplate == null)
                {
                    //Look for the animated FrameworkElement; if found, verify its final value.
                    if (animatedDO != null)
                    {
                        result = CheckFE(animatedDO);
                    }
                    else
                    {
                        //Look for the animated Animatable; if found, verify its final value.
                        //First, look under the root element.
                        animatable = (Animatable)s_root.FindName("AnimatedAnimatable");

                        if (animatable == null)
                        {
                            try
                            {
                                //Then, if not found, look inside the root resources (Key="ResKey").
                                animatable = (Animatable)s_root.FindResource("ResKey");
                            }
                            catch (Exception e)
                            {
                                s_outputMessages += "ERROR!!! VerifyValue: the Animatable was not found in Root Resources: " + e.Message + "\n";
                                result = false;
                            }
                        }

                        if (animatable == null)
                        {
                            s_outputMessages += "ERROR!!! VerifyValue: the Animatable was not found" + "\n";
                            result = false;
                        }
                        else
                        {
                            result = CheckAnimatable(animatable);
                        }
                    }
                }
                else
                {
                    //The animation is specified inside a ControlTemplate.
                    animatable = (Animatable)((Control)control).Template.FindName("AnimatedAnimatable", (Control)control);
                    if (animatable != null)
                    {
                        result = CheckAnimatable(animatable);
                    }
                    else
                    {
                        result = CheckFE(controlInTemplate);
                    }
                }
            }

            outputData = s_outputMessages;
            
            return result;
        }
        
        /******************************************************************************
           * Function:          GetTemplateControl
           ******************************************************************************/
        /// <summary>
        /// GetTemplateControl: Looks for a DependencyObject named "TemplateControl" inside
        /// a ControlTemplate, if one exists.  Return null if not found.
        /// </summary>
        /// <returns>A DependencyObject or null</returns>
        private static DependencyObject GetTemplateControl(DependencyObject control)
        {
            DependencyObject templateDO = null;
            
            if (control != null)
            {
                templateDO = (DependencyObject)((Control)control).Template.FindName("TemplateControl", (Control)control);
            }
            
            return templateDO;
        }
        
        /******************************************************************************
           * Function:          CheckAnimatable
           ******************************************************************************/
        /// <summary>
        /// ASSUMPTION: the animated element's name is "AnimatedFE".
        /// </summary>
        /// <returns>A boolean indicating whether or not the expected value was found</returns>
        private static bool CheckAnimatable(Animatable animatable)
        {
            bool result = false;

            GlobalLog.LogStatus("---------------------------------------");
            GlobalLog.LogStatus("Verifying GetValue (Animatable)");
            GlobalLog.LogStatus("Input Parameters (from Tag):");
            GlobalLog.LogStatus("  Type:           " + s_animationType);
            GlobalLog.LogStatus("  Expected Value: " + s_animationValue);
            GlobalLog.LogStatus("  Animated DP:    " + s_animationDP);
            GlobalLog.LogStatus("DO: " + animatable.GetType());
            GlobalLog.LogStatus("---------------------------------------");

            Type type = animatable.GetType();
            FieldInfo fi = type.GetField(s_animationDP + "Property", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            
            if (fi != null)
            {
                DependencyProperty dp = (DependencyProperty)fi.GetValue(null);

                DependencyObject DO = (DependencyObject)animatable.CloneCurrentValue();

                switch (s_animationType)
                {
                    case "double":
                        result = VerifyDouble(DO, dp);
                        break;
                    case "color":
                        result = VerifyColorAnimatable(type, DO);
                        break;
                    case "rect":
                        result = VerifyRect(DO, dp);
                        break;
                    case "point":
                        result = VerifyPoint(DO, dp);
                        break;
                    case "size":
                        result = VerifySize(DO, dp);
                        break;
                    case "int32":
                        result = VerifyInt32(DO, dp);
                        break;
                    case "boolean":
                        result = VerifyBoolean(DO, dp);
                        break;
                    default:
                        s_outputMessages += " ERROR!!! CheckAnimatable: Animation Type not found for Animatable" + "\n";
                        result = false;
                        break;
                }
            }
            else
            {
                s_outputMessages += " ERROR!!! CheckAnimatable: The requested DP was not found for: " + animatable + "\n";
                result = false;
            }

            return result;
        }

        /******************************************************************************
           * Function:          CheckFE
           ******************************************************************************/
        /// <summary>
        /// ASSUMPTION: the animated element's name is "AnimatedFE".
        /// </summary>
        /// <returns>A boolean indicating whether or not the expected value was found</returns>
        private static bool CheckFE(DependencyObject animElement)
        {
            bool                result  = false;
            DependencyProperty  dp      = null;
            
            GlobalLog.LogStatus("---------------------------------------");
            GlobalLog.LogStatus("Verifying GetValue (FE)");
            GlobalLog.LogStatus("Input Parameters (from Tag):");
            GlobalLog.LogStatus("  Type:           " + s_animationType);
            GlobalLog.LogStatus("  Expected Value: " + s_animationValue);
            GlobalLog.LogStatus("  Animated DP:    " + s_animationDP);
            GlobalLog.LogStatus("DO: " + animElement.ToString());
            GlobalLog.LogStatus("---------------------------------------");

            switch (s_animationDP)
            {
                case "Left" :
                    //SPECIAL CASE for verifying the attached property Canvas.Left.
                    dp = Canvas.LeftProperty;
                    break;
                case "Top" :
                    //SPECIAL CASE for verifying the attached property Canvas.Top.
                    dp = Canvas.TopProperty;
                    break;
                case "Column" :
                    //SPECIAL CASE for verifying the attached property Grid.Column.
                    dp = Grid.ColumnProperty;
                    break;
                case "Row" :
                    //SPECIAL CASE for verifying the attached property Grid.Row.
                    dp = Grid.RowProperty;
                    break;
                case "ColumnSpan" :
                    //SPECIAL CASE for verifying the attached property Grid.ColumnSpan.
                    dp = Grid.ColumnSpanProperty;
                    break;
                case "RowSpan" :
                    //SPECIAL CASE for verifying the attached property Grid.RowSpan.
                    dp = Grid.RowSpanProperty;
                    break;
                default:
                    dp = GetDP(animElement);
                    break;
            }

            if (dp != null)
            {
                switch (s_animationType)
                {
                    case "double":
                        result = VerifyDouble(animElement, dp);
                        break;
                    case "thickness":
                        result = VerifyThickness(animElement, dp);
                        break;
                    case "color":
                        result = VerifyColorFE(animElement, dp);
                        break;
                    case "int32":
                        result = VerifyInt32(animElement, dp);
                        break;
                    case "string":
                        result = VerifyString(animElement, dp);
                        break;
                    case "boolean":
                        result = VerifyBoolean(animElement, dp);
                        break;
                    default:
                        s_outputMessages += " ERROR!!! CheckFE: Animation Type not found for the DependencyObject" + "\n";
                        result = false;
                        break;
                }
            }

            return result;
        }

        /******************************************************************************
           * Function:          GetDP
           ******************************************************************************/
        /// <summary>
        /// GetDP: Use Reflection to obtain a DependencyProperty object from the string requested.
        /// </summary>
        /// <returns>A DependencyProperty</returns>
        private static DependencyProperty GetDP(DependencyObject animatedElement)
        {
            DependencyProperty dependencyProp = null;

            Type type = animatedElement.GetType();
            FieldInfo fi = type.GetField(s_animationDP + "Property", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            if (fi != null)
            {
                dependencyProp = (DependencyProperty)fi.GetValue(null);
            }
            else
            {
                s_outputMessages += " ERROR!!! GetDP: The requested DP was not found for: " + animatedElement + "\n";
            }
            return dependencyProp;
        }

        /******************************************************************************
           * Function:          VerifyDouble
           ******************************************************************************/
        private static bool VerifyDouble(DependencyObject DO, DependencyProperty DP)
        {
            double actualDouble             = 0d;

            switch (s_animationDP)
            {
                case "Left" :
                    //SPECIAL CASE for verifying the attached property Canvas.Left.
                    actualDouble = (double)Canvas.GetLeft((UIElement)DO);
                    break;
                case "Top" :
                    //SPECIAL CASE for verifying the attached property Canvas.Top.
                    actualDouble = (double)Canvas.GetTop((UIElement)DO);
                    break;
                default:
                    actualDouble  = (double)DO.GetValue(DP);
                    break;
            }

            //SPECIAL CASE for verifying Height/Width of a container with no base value specified.
            //This is used for certain Layout tests, in which a containter's Height/Width
            //change due to animating its child.
            if (Double.IsNaN(actualDouble))
            {
                if (DP.ToString() == "Width")
                {
                    actualDouble = (double)((FrameworkElement)DO).ActualWidth;
                    actualDouble = Math.Truncate(actualDouble);
                }
                else if (DP.ToString() == "Height")
                {
                    actualDouble = (double)((FrameworkElement)DO).ActualHeight;
                    actualDouble = Math.Truncate(actualDouble);
                }
            }

            double tolerance = 0.005d;
            
            bool passed = CompareDoubles(actualDouble, s_expectedDouble, tolerance);

            s_outputMessages += CreateResultString(passed, "    Expected:\t" + s_expectedDouble + "\n" + "    Actual:\t" + actualDouble);

            return passed;
        }
        
        /******************************************************************************
           * Function:          VerifyThickness
           ******************************************************************************/
        private static bool VerifyThickness(DependencyObject DO, DependencyProperty DP)
        {
            Thickness actualThickness   = (Thickness)DO.GetValue(DP);
            double tolerance = 0.005d;

            bool b1 = CompareDoubles(((Thickness)actualThickness).Left,   ((Thickness)s_expectedThickness).Left,   tolerance);
            bool b2 = CompareDoubles(((Thickness)actualThickness).Top,    ((Thickness)s_expectedThickness).Top,    tolerance);
            bool b3 = CompareDoubles(((Thickness)actualThickness).Right,  ((Thickness)s_expectedThickness).Right,  tolerance);
            bool b4 = CompareDoubles(((Thickness)actualThickness).Bottom, ((Thickness)s_expectedThickness).Bottom, tolerance);
            bool passed = (b1 && b2 && b3 && b4);

            s_outputMessages += CreateResultString(passed, "    Expected:\t" + s_expectedThickness + "\n" + "    Actual:\t" + actualThickness);

            return passed;
        }
        
        /******************************************************************************
           * Function:          VerifyColorFE
           ******************************************************************************/
        private static bool VerifyColorFE(DependencyObject DO, DependencyProperty DP)
        {
            SolidColorBrush actualColor = (SolidColorBrush)DO.GetValue(DP);

            bool passed = (actualColor.ToString() == s_expectedColor.ToString());

            s_outputMessages += CreateResultString(passed, "    Expected:\t" + s_expectedColor + "\n" + "    Actual:\t" + actualColor);

            return passed;
        }
        
        /******************************************************************************
           * Function:          VerifyColorAnimatable
           ******************************************************************************/
        private static bool VerifyColorAnimatable(Type type, DependencyObject DO)
        {
            Color tempColor = Colors.Black;
            switch (type.ToString())
            {
                case "System.Windows.Media.SolidColorBrush":
                    tempColor = (Color)((SolidColorBrush)DO).Color;
                    break;
                case "System.Windows.Media.GradientStop":
                    tempColor = (Color)((GradientStop)DO).Color;
                    break;
                default:
                    s_outputMessages += " ERROR!!! VerifyColorAnimatable: Color Animation testing on " + type.ToString() + " is not supported" + "\n";
                    return false;
             }

            Color onlyColor = Color.FromRgb(tempColor.R, tempColor.G, tempColor.B);
            string actualColor = onlyColor.ToString();

            bool passed = (actualColor.ToString() == s_expectedColor.ToString());

            s_outputMessages += CreateResultString(passed, "    Expected:\t" + s_expectedColor + "\n" + "    Actual:\t" + actualColor);

            return passed;
        }
           
        /******************************************************************************
           * Function:          VerifyRect
           ******************************************************************************/
        private static bool VerifyRect(DependencyObject DO, DependencyProperty DP)
        {
            Rect actualRect = (Rect)DO.GetValue(DP);

            double actualLeft       = actualRect.Left;
            double actualRight      = actualRect.Right;
            double actualTop        = actualRect.Top;
            double actualBottom     = actualRect.Bottom;

            double expectedLeft     = s_expectedRect.Left;
            double expectedRight    = s_expectedRect.Right;
            double expectedTop      = s_expectedRect.Top;
            double expectedBottom   = s_expectedRect.Bottom;

            double tolerance = 0.005d;

            bool b1 = CompareDoubles(actualLeft,   expectedLeft,   tolerance);
            bool b2 = CompareDoubles(actualRight,  expectedRight,  tolerance);
            bool b3 = CompareDoubles(actualTop,    expectedTop,    tolerance);
            bool b4 = CompareDoubles(actualBottom, expectedBottom, tolerance);
            bool passed = (b1 && b2 && b3 && b4);

            s_outputMessages += CreateResultString(passed, "    Expected:\t" + s_expectedRect + "\n" + "    Actual:\t" + actualRect);

            return passed;
        }
           
        /******************************************************************************
           * Function:          VerifyPoint
           ******************************************************************************/
        private static bool VerifyPoint(DependencyObject DO, DependencyProperty DP)
        {
            Point actualPoint = (Point)DO.GetValue(DP);

            double actualX      = actualPoint.X;
            double actualY      = actualPoint.Y;

            double expectedX    = s_expectedPoint.X;
            double expectedY    = s_expectedPoint.Y;

            double tolerance = 0.005d;

            bool b1 = CompareDoubles(actualX, expectedX, tolerance);
            bool b2 = CompareDoubles(actualY, expectedY, tolerance);

            bool passed = (b1 && b2);

            s_outputMessages += CreateResultString(passed, "    Expected:\t" + s_expectedPoint + "\n" + "    Actual:\t" + actualPoint);

            return passed;
        }
           
        /******************************************************************************
           * Function:          VerifySize
           ******************************************************************************/
        private static bool VerifySize(DependencyObject DO, DependencyProperty DP)
        {
            Size actualSize = (Size)DO.GetValue(DP);

            double actualHeight     = actualSize.Height;
            double actualWidth      = actualSize.Width;

            double expectedHeight   = s_expectedSize.Height;
            double expectedWidth    = s_expectedSize.Width;

            double tolerance = 0.005d;

            bool b1 = CompareDoubles(actualHeight, expectedHeight, tolerance);
            bool b2 = CompareDoubles(actualWidth,  expectedWidth,  tolerance);

            bool passed = (b1 && b2);

            s_outputMessages += CreateResultString(passed, "    Expected:\t" + s_expectedSize + "\n" + "    Actual:\t" + actualSize);

            return passed;
        }

        /******************************************************************************
           * Function:          VerifyInt32
           ******************************************************************************/
        private static bool VerifyInt32(DependencyObject DO, DependencyProperty DP)
        {
            Int32 actualInt32 = 0;
            Type doType;

            switch (s_animationDP)
            {
                case "Column" :
                    //SPECIAL CASE for verifying the attached property Grid.Column.
                    actualInt32 = (Int32)Grid.GetColumn((UIElement)DO);
                    break;
                case "Row" :
                    //SPECIAL CASE for verifying the attached property Grid.Row.
                    actualInt32 = (Int32)Grid.GetRow((UIElement)DO);
                    break;
                case "ColumnSpan" :
                    doType = DO.GetType();
                    if (doType == typeof(System.Windows.Documents.TableCell))
                    {
                        //SPECIAL CASE for verifying the attached property TableCell.ColumnSpan.
                        actualInt32 = (Int32)((TableCell)DO).ColumnSpan;
                    }
                    else
                    {
                        //SPECIAL CASE for verifying the attached property Grid.ColumnSpan.
                        actualInt32 = (Int32)Grid.GetColumnSpan((UIElement)DO);
                    }
                    break;
                case "RowSpan" :
                    doType = DO.GetType();
                    if (doType == typeof(System.Windows.Documents.TableCell))
                    {
                        //SPECIAL CASE for verifying the attached property TableCell.RowSpan.
                        actualInt32 = (Int32)((TableCell)DO).RowSpan;
                    }
                    else
                    {
                        //SPECIAL CASE for verifying the attached property Grid.RowSpan.
                        actualInt32 = (Int32)Grid.GetRowSpan((UIElement)DO);
                    }
                    break;
                default:
                    actualInt32 = (Int32)DO.GetValue(DP);
                    break;
            }

            bool passed = (actualInt32 == s_expectedInt32);

            s_outputMessages += CreateResultString(passed, "    Expected:\t" + s_expectedInt32 + "\n" + "    Actual:\t" + actualInt32);

            return passed;
        }

        /******************************************************************************
           * Function:          VerifyString
           ******************************************************************************/
        private static bool VerifyString(DependencyObject DO, DependencyProperty DP)
        {
            string actualString = (string)DO.GetValue(DP);

            bool passed = (actualString == s_expectedString);

            s_outputMessages += CreateResultString(passed, "    Expected:\t" + s_expectedString + "\n" + "    Actual:\t" + actualString);

            return passed;
        }

        /******************************************************************************
           * Function:          VerifyBoolean
           ******************************************************************************/
        private static bool VerifyBoolean(DependencyObject DO, DependencyProperty DP)
        {
            bool actualBoolean = (bool)DO.GetValue(DP);

            bool passed = (actualBoolean == s_expectedBoolean);

            s_outputMessages += CreateResultString(passed, "    Expected:\t" + s_expectedBoolean + "\n" + "    Actual:\t" + actualBoolean);

            return passed;
        }

        /******************************************************************************
           * Function:          CompareDoubles
           ******************************************************************************/
        /// <summary>
        /// CompareDoubles: Compares two doubles, using a tolerance.
        /// </summary>
        /// <returns>A bool, indicating whether or not the two doubles are equal</returns>
        private static bool CompareDoubles(double actDouble, double expDouble, double tolerance)
        {
            bool result = false;
            double percentDiff  = 0d;

            if (actDouble == 0)
            {
                percentDiff = actDouble - expDouble;
            }
            else
            {
                percentDiff = (actDouble - expDouble) / actDouble;
            }

            result = (Math.Abs(percentDiff) <= tolerance);

            return result;
        }

        /******************************************************************************
           * Function:          CreateResultString
           ******************************************************************************/
        /// <summary>
        /// CreateResultString: Creates a string to display the test results.
        /// </summary>
        /// <returns>A string indicating test results.</returns>
        private static string CreateResultString(bool testPassed, string details)
        {
            string result = "";

            if (testPassed)
            {
                result = "  GetValue is correct." + "\n";
            }
            else
            {
                result = "  GetValue is incorrect." + "\n";
            }
            result += details + "\n";

            return result;
        }
    }
}
