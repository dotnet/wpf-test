// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  HelperClass - Commonly used verification and message functions
//
//
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Globalization;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class HelperClass
    {
        public HelperClass()
        {
        }

        #region BuildColorStringARGB - Color
        // BuildColorString(Color color)
        // A protected helper function that returns a string representation of the 
        // ARGB values in a Color object.
        // Parameters: 
        //     Color color - The color to parse.
        //
        // Returns:
        //      string
        //  
        public string BuildColorStringARGB(Color color)
        {
            return (("(alpha=" + color.A + " red=" + color.R + " green=" + color.G + " blue=" + color.B + ")"));
        }
        #endregion End BuildColorStringARGB - Color

        #region BuildColorStringScArgb - Color
        // BuildColorString(Color color)
        // A protected helper function that returns a string representation of the 
        // ScArgb values in a Color object.
        // Parameters: 
        //     Color color - The color to parse.
        //
        // Returns:
        //      string
        //  
        public string BuildColorStringScArgb(Color color)
        {
            return (("(alpha=" + color.ScA + " red=" + color.ScR + " green=" + color.ScG + " blue=" + color.ScB + ")"));
        }
        #endregion End BuildColorStringScArgb - Color

        #region BuildColorStringFloats
        // BuildColorStringFloats(float a, float r, float g, float b)
        // A protected helper function that returns a string representation of the 
        // values in a Color object.
        // Parameters: 
        //      float a, float r, float g, float b - The values of the four color
        //      channels.
        //
        // Returns:
        //      string
        //  
        public string BuildColorStringFloats(float a, float r, float g, float b)
        {
            return (("(alpha=" + a + " red=" + r + " green=" + g + " blue=" + b + ")"));
        }
        #endregion

        #region BuildColorStringInts
        // BuildColorStringInts(int a, int r, int g, int b)
        // A protected helper function that returns a string representation of the 
        // values in a Color object.
        // Parameters: 
        //      int a, int r, int g, int b - The values of the four color
        //      channels.
        //
        // Returns:
        //      string
        //  
        public string BuildColorStringInts(int a, int r, int g, int b)
        {
            return (("(alpha=" + a + " red=" + r + " green=" + g + " blue=" + b + ")"));
        }
        #endregion BuildColorStringInts

        #region ConvertsRGBToScRGB
        // ConvertsRGBToScRGB(byte bval)
        // A protected helper function that converts a byte Color value (sRGB) to it's 
        // equivalent float scRGB value using the same math that is used to do this in 
        // PresentationCore.dll (in the file Color.cs). 
        // Parameters: 
        //      byte bval - The sRGB Color value to convert
        //
        // Returns:
        //      float
        //  
        public float ConvertsRGBToScRGB(byte bval)
        {
            float fval = ((float)bval / 255.0f);

            if (!(fval > 0.0))       // Handles NaN case too. (Though, NaN isn't actually
            // possible in this case.)
            {
                return (0.0f);
            }
            else if (fval <= 0.04045)
            {
                return (fval / 12.92f);
            }
            else if (fval < 1.0f)
            {
                return (float)Math.Pow(((double)fval + 0.055) / 1.055, 2.4);
            }
            else
            {
                return (1.0f);
            }
        }
        #endregion ConvertsRGBToScRGB

        #region ConvertScRGBTosRGB
        // ConvertScRGBTosRGB(float fval)
        // A protected helper function that converts a float Color value (ScRGB) to it's 
        // equivalent byte sRGB value using using the same math that is used to do this in 
        // PresentationCore.dll (in the file Color.cs). 
        // Parameters: 
        //      float fval - The ScRGB Color value to convert/
        //
        // Returns:
        //      byte
        //  
        public byte ConvertScRGBTosRGB(float fval)
        {
            if (!(fval > 0.0))       // Handles NaN case too
            {
                return (0);
            }
            else if (fval <= 0.0031308)
            {
                return ((byte)((255.0f * fval * 12.92f) + 0.5f));
            }
            else if (fval < 1.0)
            {
                return ((byte)((255.0f * ((1.055f * (float)Math.Pow((double)fval, (1.0 / 2.4))) - 0.055f)) + 0.5f));
            }
            else
            {
                return (255);
            }
        }
        #endregion ConvertScRGBTosRGB

        #region MultiplyRGBColorChannel
        // MultiplyRGBColorChannel(byte color_channel_value, float multiplier)
        // A helper function that multiplies an RGB color channel value by
        // a float multiplier as it is done in Color.cs.  This is used for verifying
        // Color math operations.
        // Parameters: 
        //      byte color_channel_value - The color channel value to multiply
        //      float multiplier         - The value to multiply with.
        //
        // Returns:
        //      byte 
        //  
        public byte MultiplyRGBColorChannel(byte color_channel_value, float multiplier)
        {
            // Convert sRGB color channel value to ScRGB
            float ScRGB_Value = ConvertsRGBToScRGB(color_channel_value);

            // Do Multiplication
            ScRGB_Value *= multiplier;

            // Convert ScRGB color channel value to sRGB
            byte sRGB_Value = ConvertScRGBTosRGB(ScRGB_Value);

            // Return the new value.
            return (sRGB_Value);
        }
        #endregion MultiplyRGBColorChannel

        #region AddRGBColorChannel
        // AddRGBColorChannel(byte color_channel_value1, byte color_channel_value2)
        // A helper function that adds two RGB color channel values together as it
        // is done in Color.cs.  This is used for verirfying Color math operations.
        // Parameters: 
        //      byte color_channel_value1 - One color channel value to add
        //      byte color_channel_value2 - The other color channel value to add
        //
        // Returns:
        //      byte 
        //  
        public byte AddRGBColorChannel(byte color_channel_value1, byte color_channel_value2)
        {
            // Convert both sRGB color channel values to ScRGB
            float ScRGB_Value1 = ConvertsRGBToScRGB(color_channel_value1);
            float ScRGB_Value2 = ConvertsRGBToScRGB(color_channel_value2);

            // Do Addition
            float ScRGB_Value = ScRGB_Value1 + ScRGB_Value2;

            // Convert ScRGB color channel value to sRGB
            byte sRGB_Value = ConvertScRGBTosRGB(ScRGB_Value);

            // Return the new value.
            return (sRGB_Value);
        }
        #endregion AddRGBColorChannel

        #region SubtractRGBColorChannel
        // SubtractRGBColorChannel(byte color_channel_value1, byte color_channel_value2)
        // A helper function that subtracts one RGB color channel value from another as it
        // is done in Color.cs.  This is used for verirfying Color math operations.
        // Parameters: 
        //      byte color_channel_value1 - One color channel value
        //      byte color_channel_value2 - The other color channel value to subtract
        //
        // Returns:
        //      byte 
        //  
        public byte SubtractRGBColorChannel(byte color_channel_value1, byte color_channel_value2)
        {
            // Convert both sRGB color channel values to ScRGB
            float ScRGB_Value1 = ConvertsRGBToScRGB(color_channel_value1);
            float ScRGB_Value2 = ConvertsRGBToScRGB(color_channel_value2);

            // Do Subtracton
            float ScRGB_Value = ScRGB_Value1 - ScRGB_Value2;

            // Convert ScRGB color channel value to sRGB
            byte sRGB_Value = ConvertScRGBTosRGB(ScRGB_Value);

            // Return the new value.
            return (sRGB_Value);
        }
        #endregion SubtractRGBColorChannel

        #region BuildMatrixString - Six doubles
        // BuildMatrixString(double M11, double M12, double M21, double M22, double MOfX, double MOfY)
        // A protected helper function that returns a string representation of the 
        // values in a Matrix.
        // Parameters: 
        //      double M11, double M12, double M21, double M22, double MOfX, double MOfY
        //      - The values of the Matrix
        //
        // Returns:
        //      string
        //  
        public string BuildMatrixString(double M11, double M12, double M21, double M22, double MOfX, double MOfY)
        {
            return ("(" + M11 + ", " + M12 + ", " + M21 + ", " + M22 + ", " + MOfX + ", " + MOfY + ")");
        }
        #endregion BuildMatrixString - Six doubles

        #region CheckType
        // bool CheckType(object obj, string objectType)
        // A protected helper function to display a LogStatus message based on the
        // expected type of an Object and return a pass/fail result.
        // Parameters: 
        //      object obj - The object to check.
        //      string objectType - The type of object that is expected.
        //
        // Returns:
        //      bool (Pass = true, Fail = false)
        //
        public bool CheckType(object obj, Type objectType)
        {
            // Confirm that the expected object type was created successfully.
            if (obj.GetType()== objectType)
            {
                CommonLib.LogStatus("Pass: " + objectType.ToString() + " was created successfully");
            }
            else
            {
                CommonLib.LogFail("Fail: " + objectType.ToString() + " was NOT created a " + obj.GetType().ToString() + " was created.");
                return false;
            }

            return true;
        }
        #endregion End CheckType

        #region CompareProp - Two strings
        // bool CompareProp(string prop, string propvalue, string expected)
        // A protected helper function to compare two string values, output the result
        // to the Automation log and return a pass/fail result. 
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      string propvalue - The value of the property.
        //      string expected - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public bool CompareProp(string prop, Type propvalue, Type expected)
        {
            if (propvalue == expected)
            {
                CommonLib.LogStatus("Pass: " + prop + "=" + propvalue.ToString());
            }
            else
            {
                CommonLib.LogFail("Fail: " + prop + "=" + propvalue.ToString() + " should be " + expected.ToString());
                return false;
            }

            return true;
        }
        #endregion 
        #region CompareProp - Two strings
        // bool CompareProp(string prop, string propvalue, string expected)
        // A protected helper function to compare two string values, output the result
        // to the Automation log and return a pass/fail result. 
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      string propvalue - The value of the property.
        //      string expected - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public bool CompareProp(string prop, string propvalue, string expected)
        {
            if (propvalue == expected)
            {
                CommonLib.LogStatus("Pass: " + prop + "=" + propvalue);
            }
            else
            {
                CommonLib.LogFail("Fail: " + prop + "=" + propvalue + " should be " + expected);
                return false;
            }

            return true;
        }
        #endregion CompareProp - Two doubles

        #region CompareProp - Two doubles
        // bool CompareProp(string prop, double propvalue, double expected)
        // A protected helper function to compare two double values, output the result
        // to the Automation log and return a pass/fail result. 
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      double propvalue - The value of the property.
        //      double expected - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public bool CompareProp(string prop, double propvalue, double expected)
        {
            if (propvalue == expected)
            {
                CommonLib.LogStatus("Pass: " + prop + "=" + propvalue.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
            else
            {
                CommonLib.LogFail("Fail: " + prop + "=" + propvalue.ToString(System.Globalization.CultureInfo.InvariantCulture) + " should be " + expected.ToString(System.Globalization.CultureInfo.InvariantCulture));
                return false;
            }

            return true;
        }
        #endregion CompareProp - Two doubles

        #region CompareProp - Two doubles with tolerance
        // bool CompareProp(string prop, double propvalue, double expected, double tolerance)
        // A protected helper function to compare two double values, output the result
        // to the Automation log and return a pass/fail result. 
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      double propvalue - The value of the property.
        //      double expected - The expected value.
        //      double tolerance
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public bool CompareProp(string prop, double propvalue, double expected, double tolerance)
        {
            if (Math.Abs(propvalue - expected) < tolerance)
            {
                CommonLib.LogStatus("Pass: " + prop + "=" + propvalue.ToString(System.Globalization.CultureInfo.InvariantCulture)
                    + " expected=" + expected.ToString(System.Globalization.CultureInfo.InvariantCulture)
                    + " tolerance=" + tolerance.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
            else
            {
                CommonLib.LogFail("Fail: " + prop + "=" + propvalue.ToString(System.Globalization.CultureInfo.InvariantCulture)
                    + " expected=" + expected.ToString(System.Globalization.CultureInfo.InvariantCulture)
                    + " tolerance=" + tolerance.ToString(System.Globalization.CultureInfo.InvariantCulture));
                return false;
            }

            return true;
        }
        #endregion CompareProp - Two doubles with tolerance

        #region CompareProp - Two ints
        // bool CompareProp(string prop, int propvalue, int expected)
        // A protected helper function to compare two int values, output the result
        // to the Automation log and return a pass/fail result. 
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      int propvalue - The value of the property.
        //      int expected - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public bool CompareProp(string prop, int propvalue, int expected)
        {
            if (propvalue == expected)
            {
                CommonLib.LogStatus("Pass: " + prop + "=" + propvalue.ToString());
            }
            else
            {
                CommonLib.LogFail("Fail: " + prop + "=" + propvalue.ToString() + " should be " + expected.ToString());
                return false;
            }

            return true;
        }
        #endregion CompareProp - Two ints

        #region CompareProp - Two bools
        // bool CompareProp(string prop, bool propvalue, bool expected)
        // A protected helper function to compare two bool values, output the result
        // to the Automation log and return a pass/fail result. 
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      bool propvalue - The value of the property.
        //      bool expected - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public bool CompareProp(string prop, bool propvalue, bool expected)
        {
            if (propvalue == expected)
            {
                CommonLib.LogStatus("Pass: " + prop + "=" + propvalue.ToString());
            }
            else
            {
                CommonLib.LogFail("Fail: " + prop + "=" + propvalue.ToString() + " should be " + expected.ToString());
                return false;
            }

            return true;
        }
        #endregion CompareProp - Two bools

        #region CompareProp - Two Rects
        // bool CompareProp(string prop, System.Windows.Rect propvalue, System.Windows.Rect expected)
        // A protected helper function to compare two Rect values, output the result
        // to the Automation log and return a pass/fail result. 
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      Rect propvalue - The value of the property.
        //      Rect expected - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public bool CompareProp(string prop, System.Windows.Rect propvalue, System.Windows.Rect expected)
        {
            if (System.Windows.Rect.Equals(propvalue, expected))
            {
                CommonLib.LogStatus("Pass: " + prop + "=" + propvalue.ToString());
            }
            else
            {
                CommonLib.LogFail("Fail: " + prop + "=" + propvalue.ToString() + " should be " + expected.ToString());
                return false;
            }

            return true;
        }
        #endregion CompareProp - Two Rects

        #region CompareProp - Two Sizes
        // bool CompareProp(string prop, System.Windows.Rect propvalue, System.Windows.Rect expected)
        // A protected helper function to compare two Rect values, output the result
        // to the Automation log and return a pass/fail result. 
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      Rect propvalue - The value of the property.
        //      Rect expected - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public bool CompareProp(string prop, System.Windows.Size propvalue, System.Windows.Size expected)
        {
            if (System.Windows.Size.Equals(propvalue, expected))
            {
                CommonLib.LogStatus("Pass: " + prop + "=" + propvalue.ToString());
            }
            else
            {
                CommonLib.LogFail("Fail: " + prop + "=" + propvalue.ToString() + " should be " + expected.ToString());
                return false;
            }

            return true;
        }
        #endregion CompareProp - Two Sizes

        #region CompareProp - Two GradientStops
        // bool CompareProp(string prop, GradientStop propvalue, GradientStop expected)
        // A protected helper function to compare two GradientStops, output the result
        // to the Automation log and return a pass/fail result. 
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      GradientStop propvalue - The value of the property.
        //      GradientStop expected - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public bool CompareProp(string prop, GradientStop propvalue, GradientStop expected)
        {
            if ((propvalue.Color == expected.Color) && (propvalue.Offset == expected.Offset))
            {
                CommonLib.LogStatus("Pass: " + prop + " = GradientStop has the expected values");
            }
            else
            {
                CommonLib.LogFail("Fail: " + prop + " = GradientStop does not have the expected values");
                return false;
            }

            return true;
        }
        #endregion CompareProp - Two GradientStops

        #region CompareProp - Two Float Arrays
        // bool CompareProp(string prop, float[] propvalue, float[] expected)
        // A protected helper function to compare two Float Arrays, output the result
        // to the Automation log and return a pass/fail result. 
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      float[] propvalue - The value of the property.
        //      float[] expected - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public bool CompareProp(string prop, float[] propvalue, float[] expected)
        {
            if (propvalue.Length != expected.Length)
            {
                CommonLib.LogFail("Fail: " + prop + " = Float Array contains " + propvalue.Length + " values should have " + expected.Length + " values");
                return false;
            }

            if (CompareFloatArrays(propvalue, expected))
            {
                CommonLib.LogStatus("Pass: " + prop + " = Float Array has the expected values");
            }
            else
            {
                CommonLib.LogFail("Fail: " + prop + " = Float Array does not have the expected values");
                return false;
            }

            return true;
        }
        #endregion CompareProp - Two Float Arrays

        #region CompareFloatArrays
        // bool CompareFloatArrays(float[] F1, float[] F2)
        // A protected helper function to compare two DoubleCollections, and return a pass/fail result. 
        // Parameters: 
        //      float[] F1 - The value to compare.
        //      float[] F2 - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public bool CompareFloatArrays(float[] F1, float[] F2)
        {
            if (F1.Length != F2.Length)
            {
                return false;
            }

            for (int i = 0; i < F1.Length; i++)
            {
                if (F1[i] != F2[i])
                {
                    return false;
                }
            }

            return true;
        }
        #endregion CompareGradientStopCollections

        #region CompareProp - Two DoubleCollections
        // bool CompareProp(string prop, DoubleCollection propvalue, DoubleCollection expected)
        // A protected helper function to compare two DoubleCollections, output the result
        // to the Automation log and return a pass/fail result. 
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      DoubleCollection propvalue - The value of the property.
        //      DoubleCollection expected - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public bool CompareProp(string prop, DoubleCollection propvalue, DoubleCollection expected)
        {
            if (propvalue.Count != expected.Count)
            {
                CommonLib.LogFail("Fail: " + prop + " = DoubleCollection contains " + propvalue.Count + " values should have " + expected.Count + " values");
                return false;
            }

            if (CompareDoubleCollections(propvalue, expected))
            {
                CommonLib.LogStatus("Pass: " + prop + " = DoubleCollection has the expected values");
            }
            else
            {
                CommonLib.LogFail("Fail: " + prop + " = DoubleCollection does not have the expected values");
                return false;
            }

            return true;
        }
        #endregion CompareProp - Two DoubleCollections

        #region CompareDoubleCollections
        // bool CompareDoubleCollections(DoubleCollection D1, DoubleCollection D2)
        // A protected helper function to compare two DoubleCollections, and return a pass/fail result. 
        // Parameters: 
        //      DoubleCollection D1 - The value to compare.
        //      DoubleCollection D2 - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public bool CompareDoubleCollections(DoubleCollection D1, DoubleCollection D2)
        {
            if (D1.Count != D2.Count)
            {
                return false;
            }

            for (int i = 0; i < D1.Count; i++)
            {
                if (D1[i] != D2[i])
                {
                    return false;
                }
            }

            return true;
        }
        #endregion CompareGradientStopCollections

        #region CompareProp - Two ColorCollections
        // bool CompareProp(string prop, ColorCollection propvalue, ColorCollection expected)
        // A protected helper function to compare two ColorCollections, output the result
        // to the Automation log and return a pass/fail result. 
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //     ColorCollection propvalue - The value of the property.
        //     ColorCollection expected - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public bool CompareProp(string prop, List<Color> propvalue, List<Color> expected)
        {
            if (propvalue.Count != expected.Count)
            {
                CommonLib.LogFail("Fail: " + prop + " = ColorCollection contains " + propvalue.Count + " values should have " + expected.Count + " values");
                return false;
            }

            if (CompareColorCollections(propvalue, expected))
            {
                CommonLib.LogStatus("Pass: " + prop + " = ColorCollection has the expected values");
            }
            else
            {
                CommonLib.LogFail("Fail: " + prop + " = ColorCollection does not have the expected values");
                return false;
            }

            return true;
        }
        #endregion CompareProp - Two ColorCollections

        #region CompareColorCollections
        // bool CompareColorCollections(ColorCollection C1, ColorCollection C2)
        // A protected helper function to compare two ColorCollections, and return a pass/fail result. 
        // Parameters: 
        //     ColorCollection C1 - The value to compare.
        //     ColorCollection C2 - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public bool CompareColorCollections(List<Color> C1, List<Color> C2)
        {
            if (C1.Count != C2.Count)
            {
                return false;
            }

            for (int i = 0; i < C1.Count; i++)
            {
                if (C1[i] != C2[i])
                {
                    return false;
                }
            }

            return true;
        }
        #endregion CompareGradientStopCollections

        #region CompareProp - Two GradientStopCollections
        // bool CompareProp(string prop, GradientStopCollection propvalue, GradientStopCollection expected)
        // A protected helper function to compare two GradientStopCollections, output the result
        // to the Automation log and return a pass/fail result. 
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      GradientStopCollection propvalue - The value of the property.
        //      GradientStopCollection expected - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public bool CompareProp(string prop, GradientStopCollection propvalue, GradientStopCollection expected)
        {
            if (propvalue.Count != expected.Count)
            {
                CommonLib.LogFail("Fail: " + prop + " = GradientStopCollection contains " + propvalue.Count + " values should have " + expected.Count + " values");
                return false;
            }

            if (CompareGradientStopCollections(propvalue, expected))
            {
                CommonLib.LogStatus("Pass: " + prop + " = GradientStopCollection has the expected values");
            }
            else
            {
                CommonLib.LogFail("Fail: " + prop + " = GradientStopCollection does not have the expected values");
                return false;
            }

            return true;
        }
        #endregion CompareProp - Two GradientStopCollections

        #region CompareProp - Two DrawingGroups
        // bool ComparePropstring prop, (DrawingGroup propvalue, DrawingGroup expected)
        // A protected helper function to compare two DrawingGroups by comparing the Bounds of each child Drawing in the
        // group, and log a pass/fail result using Framework LogStatus. 
        // Parameters:
        //      string prop - Name of the Property to compare. 
        //      DrawingGroup propvalue - The value to compare.
        //      DrawingGroup expected - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public bool CompareProp(string prop, DrawingGroup propvalue, DrawingGroup expected)
        {
            if (propvalue.Children.Count != expected.Children.Count)
            {
                CommonLib.LogFail("Fail: " + prop + " = DrawingGroup contains the wrong number of Drawings");
                return false;
            }

            for (int i = 0; i < expected.Children.Count; i++)
            {
                // Call compareProp for Rect
                if (!CompareProp((prop + "[" + i.ToString() + "]"), propvalue.Children[i].Bounds, expected.Children[i].Bounds))
                {
                    return false;
                }
            }

            return true;
        }
        #endregion CompareProp - Two DrawingGroups

        #region CompareProp - Two Colors
        // bool CompareProp(string prop, Color propvalue, Color expected)
        // A protected helper function to compare two Colors, using the Equals
        // operator in the Avalon Color object. Returns a pass/fail result and
        // logs a message.
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //     Color propvalue - The value of the property.
        //     Color expected - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public bool CompareProp(string prop, Color propvalue, Color expected)
        {
            if (propvalue == expected)
            {
                CommonLib.LogStatus("Pass: " + prop + "=" + BuildColorStringScArgb(propvalue));
            }
            else
            {
                CommonLib.LogFail("Fail: " + prop + "=" + BuildColorStringScArgb(propvalue) + " should be " + BuildColorStringScArgb(expected));
                return false;
            }

            return true;
        }
        #endregion CompareProp - Two Colors

        #region CompareGradientStopCollections
        // bool CompareGradientStopCollections(GradientStopCollection gs1, GradientStopCollection gs2)
        // A protected helper function to compare two GradientStopCollections, and return a pass/fail result. 
        // Parameters: 
        //      GradientStopCollection gs1 - The value to compare.
        //      GradientStopCollection gs2 - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public bool CompareGradientStopCollections(GradientStopCollection gs1, GradientStopCollection gs2)
        {
            if (gs1.Count != gs2.Count)
            {
                return false;
            }

            for (int i = 0; i < gs1.Count; i++)
            {
                if ((gs1[i].Color != gs2[i].Color) || (gs1[i].Offset != gs2[i].Offset))
                {
                    return false;
                }
            }

            return true;
        }
        #endregion CompareGradientStopCollections

        #region CompareImageSourceProperties
        // bool CompareImageSourceProperties(BitmapSource propvalue, BitmapSource expected)
        // A helper function to compare the properties of two BitmapSource objects.
        // Returns a true/false result.
        // Parameters: 
        //      BitmapSource propvalue- The BitmapSource object to compare.
        //      BitmapSource expected - The expected BitmapSource object to compare against.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public bool CompareImageSourceProperties(ImageSource propvalue, ImageSource expected)
        {
            if ((propvalue.Width == expected.Width) &&
                (propvalue.Height == expected.Height))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion CompareColorPropFloats

        #region CompareColorPropFloats
        // bool CompareColorPropFloats(string prop, Color color, float expected_ScA, float expected_ScR,
        //                  float expected_ScG, float expected_ScB)
        // A protected helper function to compare a Color Property against four float
        // values, output the result to the Automation log using the ShowColorLogStatus
        // Method and return a pass/fail result.
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //     Color color - Color to compare.
        //      float expected_ScA - The expected Alpha value.
        //      float expected_ScR - The expected Red value.
        //      float expected_ScG - The expected Green value.
        //      float expected_ScB - The expected Blue value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public bool CompareColorPropFloats(string prop, Color color, float expected_ScA, float expected_ScR, float expected_ScG, float expected_ScB)
        {
            if ((color.ScA == expected_ScA) && (color.ScR == expected_ScR) && (color.ScG == expected_ScG) && (color.ScB == expected_ScB))
            {
                CommonLib.LogStatus("Pass: " + prop + "=" + BuildColorStringScArgb(color));
            }
            else
            {
                CommonLib.LogFail("Fail: " + prop + "=" + BuildColorStringScArgb(color) + " should be " + BuildColorStringFloats(expected_ScA, expected_ScR, expected_ScG, expected_ScB));
                return false;
            }

            return true;
        }
        #endregion CompareColorPropFloats

        #region CompareColorPropInts
        // bool CompareColorPropInts(string prop, Color color, int expected_A, int expected_R,
        //                  int expected_G, int expected_B)
        // A protected helper function to compare a Color Property against four int
        // values, output the result to the Automation log using the ShowColorLogStatus
        // Method and return a pass/fail result.
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //     Color color - Color to compare.
        //      int expected_A - The expected Alpha value.
        //      int expected_R - The expected Red value.
        //      int expected_G - The expected Green value.
        //      int expected_B - The expected Blue value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public bool CompareColorPropInts(string prop, Color color, int expected_A, int expected_R, int expected_G, int expected_B)
        {
            if ((color.A == expected_A) && (color.R == expected_R) && (color.G == expected_G) && (color.B == expected_B))
            {
                CommonLib.LogStatus("Pass: " + prop + "=" + BuildColorStringARGB(color));
            }
            else
            {
                CommonLib.LogFail("Fail: " + prop + "=" + BuildColorStringARGB(color) + " should be " + BuildColorStringInts(expected_A, expected_R, expected_G, expected_B));
                return false;
            }

            return true;
        }
        #endregion CompareColorPropInts

        #region CompareColorPropUint
        // bool CompareColorPropUint(string prop, Color color, uint expected)
        // A helper function to compare a Color Property against one uint value,
        // output the result to the Automation log and return a pass/fail result.
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //     Color color - Color to compare.
        //      uint expected - The expected color value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        // 
        public bool CompareColorPropUint(string prop, Color color, uint expected)
        {
            if ((color.A == (byte)((expected & 0xff000000) >> 24)) && (color.R == (byte)((expected & 0x00ff0000) >> 16)) && (color.G == (byte)((expected & 0x0000ff00) >> 8)) && (color.B == (byte)(expected & 0x000000ff)))
            {
                CommonLib.LogStatus("Pass: " + prop + "=" + BuildColorStringARGB(color));
            }
            else
            {
                CommonLib.LogFail("Fail: " + prop + "=" + BuildColorStringARGB(color) + " should be " + expected.ToString());
                return false;
            }

            return true;
        }
        #endregion CompareColorPropUint

        #region CompareMatrix
        // bool CompareMatrix(string prop, Transform trans, double M11, double M12,
        //               double M21, double M22, double MOfX, double MOfY)
        // A protected helper function to compare the Matrix value in a Transform with
        // Six Matrix values, output the result to the Automation logand return a
        // pass/fail result.
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //     Color color - Color to compare.
        //      Transform trans - The Transform to compare.
        //      double M11, M12, M21, M22, MOfX, MOfY - The expected Matrix values.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public bool CompareMatrix(string prop, Transform trans, double M11, double M12, double M21, double M22, double MOfX, double MOfY)
        {
            if ((trans.Value.M11 == M11) && (trans.Value.M12 == M12) && (trans.Value.M21 == M21) && (trans.Value.M22 == M22) && (trans.Value.OffsetX == MOfX) && (trans.Value.OffsetY == MOfY))
            {
                CommonLib.LogStatus("Pass: " + prop + "=" + BuildMatrixString(trans.Value.M11, trans.Value.M12, trans.Value.M21, trans.Value.M22, trans.Value.OffsetX, trans.Value.OffsetY));
            }
            else
            {
                CommonLib.LogFail("Fail: " + prop + "=" + BuildMatrixString(trans.Value.M11, trans.Value.M12, trans.Value.M21, trans.Value.M22, trans.Value.OffsetX, trans.Value.OffsetY) + " should be " + BuildMatrixString(M11, M12, M21, M22, MOfX, MOfY) + ")");
                return false;
            }

            return true;
        }
        // bool CompareMatrix(Matrix matrix, double M11, double M12,
        //               double M21, double M22, double MOfX, double MOfY)
        // A protected helper function to compare the Matrix value in a Transform with
        // Six Matrix values, output the result to the Automation logand return a
        // pass/fail result.
        // This function uses a tolerance since there could be slightly differences 
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //     Color color - Color to compare.
        //      Matrix - The Matrix to compare.
        //      double M11, M12, M21, M22, MOfX, MOfY - The expected Matrix values.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public bool CompareMatrix(string prop, Matrix matrix, double M11, double M12, double M21, double M22, double MOfX, double MOfY)
        {
            double tolerance = 0.00001;
            if ((Math.Abs(matrix.M11 - M11) < tolerance) && (Math.Abs(matrix.M12 - M12) < tolerance) && (Math.Abs(matrix.M21 - M21) < tolerance) && (Math.Abs(matrix.M22 - M22) < tolerance) && (Math.Abs(matrix.OffsetX - MOfX) < tolerance) && (Math.Abs(matrix.OffsetY - MOfY) < tolerance))
            {
                CommonLib.LogStatus("Pass: " + prop + "=" + BuildMatrixString(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX, matrix.OffsetY));
            }
            else
            {
                CommonLib.LogFail("Fail: " + prop + "=" + BuildMatrixString(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX, matrix.OffsetY) + " should be " + BuildMatrixString(M11, M12, M21, M22, MOfX, MOfY) + ")");
                return false;
            }
            /*
            if ((matrix.M11 == M11) && (matrix.M12 == M12) && (matrix.M21 == M21) && (matrix.M22 == M22) && (matrix.OffsetX == MOfX) && (matrix.OffsetY == MOfY))
            {
                CommonLib.Log ("Pass: " + prop + "=" + BuildMatrixString (matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX, matrix.OffsetY));
            }
            else
            {
                CommonLib.LogFail("Fail: " + prop + "=" + BuildMatrixString (matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX, matrix.OffsetY) + " should be " + BuildMatrixString (M11, M12, M21, M22, MOfX, MOfY) + ")");
                return false;
            }
            */

            return true;
        }
        #endregion CompareMatrix

        #region ComparePoint
        // ComparePoint(string prop, Point point1, Point point2)
        public bool ComparePoint(string prop, Point point1, Point point2)
        {
            if ((point1.X == point2.X) && (point1.Y == point2.Y))
            {
                CommonLib.LogStatus("Pass: " + prop + "=(" + point1.X + "," + point1.Y + ")");
                return true;
            }
            else
            {
                CommonLib.LogFail("Fail: " + prop + " should equal (" + point2.X + "," + point2.Y + ") not (" + point1.X + "," + point1.Y + ")");
                return false;
            }
        }
        #endregion

        #region CompareResults
        public bool CompareResults(string testName, bool propvalue, bool expected)
        {
            if (propvalue == expected)
            {
                CommonLib.LogStatus("Pass: " + testName + " returned " + propvalue.ToString());
                return true;
            }
            else
            {
                CommonLib.LogFail("Fail: " + testName + " returned " + propvalue.ToString() + " instead of " + expected.ToString());
                return false;
            }
        }
        public bool CompareResults(string testName, string propvalue, string expected)
        {
            if (string.Compare(propvalue, expected, false, System.Globalization.CultureInfo.CurrentCulture) == 0)
            {
                CommonLib.LogStatus("Pass: " + testName + " returned " + propvalue);
                return true;
            }
            else
            {
                CommonLib.LogFail("Fail: " + testName + " returned " + propvalue + " instead of " + expected);
                return false;
            }
        }
        #endregion

        #region CompareVector

        #region CompareVector with Tolerance explicitly specified
        public bool CompareVector(string testName, Vector value, Vector expected, double tolerance)
        {
            if ((Math.Abs(value.X - expected.X) < tolerance) && (Math.Abs(value.Y - expected.Y) < tolerance))
            {
                CommonLib.LogStatus("Pass: " + testName + " vector.X=" + value.X + " vector.Y=" + value.Y);
                return true;
            }
            else
            {
                CommonLib.LogFail("Fail: " + testName + " vector.X=" + value.X + " should=" + expected.X + ",vector.Y=" + value.Y + " should=" + expected.Y);
                return false;
            }
        }
        #endregion


        #region CompareVector without Tolerance explicitly specified, default 0.000001
        public bool CompareVector(string testName, Vector value, Vector expected)
        {
            // default tolerance 0.000001
            return CompareVector(testName, value, expected, 0.000001);
        }
        #endregion

        #region Compare Rectangle Width and Height with the given range
        // Ensure that the Width and Height of a Rect are within the given min and max of the range.
        // Output the result to the Automation log using and return a pass/fail result.
        //
        public bool CompareRectWithRange(string prop, Rect rect, float min, float max)
        {
            if ((rect.Width >= min && rect.Width <= max) && (rect.Height >= min && rect.Height <= max))
            {
                CommonLib.LogStatus("Pass: " + prop + "=(" + rect.Width.ToString() + "," + rect.Height.ToString() + ")");
                return true;
            }
            else
            {
                CommonLib.LogFail("Fail: " + prop + "=(" + rect.Width.ToString() + "," + rect.Height.ToString() + ")");
                return false;
            }
        }
        #endregion

        #region Compare RGB Color values with the given range
        // Ensure that the R, G and B values of a Color are within the given min and max.
        // Output the result to the Automation log using and return a pass/fail result.
        //
        public bool CompareColorWithRange(string prop, Color color, double min, double max)
        {
            if ((color.ScR >= min && color.ScR <= max) && (color.ScG >= min && color.ScG <= max) && (color.ScB >= min && color.ScB <= max))
            {
                CommonLib.LogStatus("Pass: " + prop + BuildColorStringScArgb(color));
                return true;
            }
            else
            {
                CommonLib.LogFail("Fail: " + prop + BuildColorStringScArgb(color));
                return false;
            }
        }
        #endregion

        #region Compare Point values with the given range
        // Ensure that the X and Y values of a Poiont are within the given min and max.
        // Output the result to the Automation log using and return a pass/fail result.
        //
        public bool ComparePointWithRange(string prop, Point point, float min, float max)
        {
            if ((point.X >= min && point.X <= max) && (point.Y >= min && point.Y <= max))
            {
                CommonLib.LogStatus("Pass: " + prop + "=(" + point.X + "," + point.Y + ")");
                return true;
            }
            else
            {
                CommonLib.LogFail("Fail: " + prop + "=(" + point.X + "," + point.Y + ")");
                return false;
            }
        }
        #endregion

        #region Compare DrawingBrush properties for equality
        // Compare the DrawingBounds, Viewbox and Viewport of two DrawingBrushes for equality.
        // Output the result to the Automation log using and return a pass/fail result.
        //
        public bool CompareDrawingBrushProperties(string prop, DrawingBrush brush1, DrawingBrush brush2)
        {
            if ((System.Windows.Rect.Equals(brush1.Drawing.Bounds, brush2.Drawing.Bounds)) &&
                (System.Windows.Rect.Equals(brush1.Viewport, brush2.Viewport)) &&
                (System.Windows.Rect.Equals(brush1.Viewbox, brush2.Viewbox)))
            {
                CommonLib.LogStatus("Pass: " + prop + "DrawingBrush properties are equal");
                return true;
            }
            else
            {
                CommonLib.LogFail("Fail: " + prop + "DrawingBrush properties are NOT equal");
                return false;
            }
        }
        #endregion

        #endregion

        #region ToLocale - Convert a string to use a specific locale
        public string ToLocale(string s, CultureInfo info)
        {
            if (info.NumberFormat.NumberDecimalSeparator == ",")
            {
                string[] vals = s.Split(ValueSeparator);
                string retval = string.Empty;

                for (int i = 0; i < vals.Length; i++)
                {
                    string val = vals[i];

                    // Swap decimals and group separators
                    val = val.Replace(',', ValueSeparator);
                    val = val.Replace('.', ',');
                    retval += val.Replace(ValueSeparator, '.') + ";";
                }
                // Remove trailing semicolon
                return retval.Substring(0, retval.Length - 1);
            }
            else
            {
                return s.Replace(ValueSeparator, ',');
            }
        }
        #endregion

        //------------------------------------------------------
        public char ValueSeparator { get { return '@'; } }
    }

    #region MyTypeConverter Class
    // We need an easily modifiable TypeConverter that implements the ITypeDescriptorContext
    // So lets make a basic one.  This is used to test a branch of the CanConvertTo method in
    // the BrushConverter class and also to test the IsSimpleBrush method in the basic brushes.
    internal class MyTypeConverter : System.ComponentModel.ITypeDescriptorContext
    {
        private object _mytype = null;

        public MyTypeConverter(object MyType)
        {
            _mytype = MyType;
        }

        public void OnComponentChanged()
        {
        }

        public bool OnComponentChanging()
        {
            return false;
        }

        public System.ComponentModel.IContainer Container
        {
            get { return null; }
        }

        public Object Instance
        {
            get { return _mytype; }
        }

        public System.ComponentModel.PropertyDescriptor PropertyDescriptor
        {
            get { return null; }
        }

        public Object GetService(System.Type t)
        {
            return null;
        }
    }
    #endregion
}
