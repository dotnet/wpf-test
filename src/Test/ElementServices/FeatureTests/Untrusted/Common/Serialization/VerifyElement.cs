// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Windows;
using System.Windows.Controls;
using Avalon.Test.CoreUI.Common;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using System.Collections;
using System.Windows.Media.Animation;

namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// Verification methods for some common element
    /// Author: Microsoft
    /// </summary>
    public class VerifyElement
    {
        /// <summary>
        /// Verify two thicknesses are equal
        /// </summary>
        public static void VerifyThickness(Thickness thickness1, Thickness thinkness2)
        {
            if (thickness1 != thinkness2)
            {
                throw new Microsoft.Test.TestValidationException("Thicknesses are different: " + thickness1.ToString() + " vs " + thinkness2.ToString());
            }
            else
            {
                CoreLogger.LogStatus("Thicknesses are the same:  " + thickness1.ToString());
            }
        }
        /// <summary>
        /// Verify two thicknesses are equal
        /// </summary>
        public static void VerifyThickness(Thickness thickness1, double thickness2)
        {
            VerifyThickness(thickness1, new Thickness(thickness2));
        }

        /// <summary>
        /// Verify two color are the same
        ///
        /// </summary>
        /// <param name="color1"></param>
        /// <param name="color2"></param>
        public static void VerifyColor(Color color1, Color color2)
        {
            if (!color1.Equals(color2))
            {
                throw new Microsoft.Test.TestValidationException("Colors are different:  " + color1.ToString() + " vs  " + color2.ToString());
            }
            else
            {
                CoreLogger.LogStatus("Colors are the same:  " + color1.ToString());
            }
        }
        /// <summary>
        /// Verify strings
        /// </summary>
        /// <param name="string1"></param>
        /// <param name="string2"></param>
        public static void VerifyString(String string1, String string2)
        {
            if (0 != String.Compare(string1, string2, StringComparison.InvariantCulture))
            {
                throw new Microsoft.Test.TestValidationException("Strings are different:  " + string1 + " vs  " + string2);
            }
            else
            {
                CoreLogger.LogStatus("strings are the same:  " + string1);
            }
        }
        /// <summary>
        ///  Verify two docks are the same
        /// </summary>
        /// <param name="dock1"></param>
        /// <param name="dock2"></param>
        public static void VerifyDock(Dock dock1, Dock dock2)
        {
            if (dock1 != dock2)
            {
                throw new Microsoft.Test.TestValidationException("Docks are different:  " + dock1.ToString() + " vs  " + dock1.ToString());
            }
            else
            {
                CoreLogger.LogStatus("Docks are the same:  " + dock1.ToString());
            }
        }
        /// <summary>
        ///  Verify a DockPanel has expected LastChildFill.
        /// </summary>
        public static void VerifyDock(DockPanel dockPanel, bool lastChildFill)
        {
            if (dockPanel.LastChildFill != lastChildFill)
            {
                throw new Microsoft.Test.TestValidationException("LastChildFill is different:  " + dockPanel.LastChildFill.ToString() + " vs  " + lastChildFill.ToString());
            }
            else
            {
                CoreLogger.LogStatus("LastChildFill is the same:  " + dockPanel.LastChildFill.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        public static void VerifyInt(int i1, int i2)
        {
            if (i1 != i2)
            {
                throw new Microsoft.Test.TestValidationException("ints are different:  " + i1 + " vs  " + i2);
            }
            else
            {
                CoreLogger.LogStatus("Integers are the same:  " + i1);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        public static void VerifyBool(bool b1, bool b2)
        {
            if (b1 != b2)
            {
                throw new Microsoft.Test.TestValidationException("bools are different:  " + b1.ToString() + " vs  " + b2.ToString());
            }
            else
            {
                CoreLogger.LogStatus("Bools are the same:  " + b1.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        public static void VerifyFloat(float b1, float b2)
        {
            if (b1 != b2)
            {
                throw new Microsoft.Test.TestValidationException("floats are different:  " + b1.ToString() + " vs  " + b2.ToString());
            }
            else
            {
                CoreLogger.LogStatus("floats are the same:  " + b1.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        public static void VerifyDouble(double b1, double b2)
        {
            bool close = true;
            double error=0.00001;
            if (Math.Abs(b2) < error)
            {
                if (Math.Abs(b1-b2) > error*error) close = false;
            }
            else if (Math.Abs(b1/b2 -1 )> error) close = false;

            if (Double.IsNaN(b1) ^ Double.IsNaN(b2)) close = false;
            if (!close)
            {
                throw new Microsoft.Test.TestValidationException("doubles are note close:  " + b1.ToString() + " vs  " + b2.ToString());
            }
            else
            {
                CoreLogger.LogStatus("double  are the same:  " + b1.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        public static void VerifyImageSource(ImageSource i1, string i2)
        {
            BitmapSource imgsrc = new BitmapImage(new Uri(i2, UriKind.RelativeOrAbsolute));
            BitmapSource bitmap = i1 as BitmapSource;
            if ( i1 != null)
            { 
                VerifyElement.VerifyInt(bitmap.PixelHeight, imgsrc.PixelHeight);
                VerifyElement.VerifyInt(bitmap.PixelWidth, imgsrc.PixelWidth);
            }
            else
            {
                VerifyElement.VerifyDouble(i1.Height, imgsrc.Height);
                VerifyElement.VerifyDouble(i1.Width, imgsrc.Width);
            }
            CoreLogger.LogStatus("two Image Source are the same" );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        public static void VerifyRect(Rect r1, Rect r2)
        {

            if (!(r1.Equals(r2)))
            {
                throw new Microsoft.Test.TestValidationException("Rectangles are different:  " + r1.ToString() + " vs  " + r2.ToString());
            }
            else
            {
                CoreLogger.LogStatus("Rectangles are the same:  " + r1.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public static void VerifyPoint(Point p1, Point p2)
        {
            if (!(p1.Equals(p2)))
            {
                throw new Microsoft.Test.TestValidationException("Points are different:  " + p1.ToString() + " vs  " + p2.ToString());
            }
            else
            {
                CoreLogger.LogStatus("Points are the same:  " + p1.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public static void VerifySize(Size p1, Size p2)
        {
            if (!(p1==p2))
            {
                throw new Microsoft.Test.TestValidationException("Sizes are different:  " + p1.ToString() + " vs  " + p2.ToString());
            }
            else
            {
                CoreLogger.LogStatus("Sizes are the same:  " + p1.ToString());
            }
        }

    }
}
