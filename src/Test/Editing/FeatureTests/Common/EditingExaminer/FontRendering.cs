// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************************
 *
 * Description: FontRendering.cs implement the immediate window with the following
 * features:
 *      1. defines Avalon font.
 *
 *******************************************************************************/

#region Using directives

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Collections;
using System.Windows.Shapes;
#endregion

namespace EditingExaminer
{
    ///<summary>
    ///create points from dobule
    ///</summary>
    public class PixelsFrom
    {
        public static double Points(double value)
        {
            return value*96.0/72.0;
        }
    }

    ///<summary>
    ///Get double from Pixels
    ///</summary>
    public class PixelsTo
    {
        public static double Points(double value)
        {
            return value*72.0/96.0;
        }
    }


    /// <summary>
    /// This class defines the Avalon font. 
    /// </summary>
    public class FontRendering
    {
        private Double _currentSize = 8;
        private TextDecorationCollection _textDecorationCollection;
        private SolidColorBrush _textColor = Brushes.Black;
        
        /// <summary>
        /// Get and set Font size maintained in Points
        /// </summary>
        public Double FontSize
        {
            get {return _currentSize;}
            set {_currentSize = value;}
        }

        /// <summary>
        /// Get and set the Text color
        /// </summary>
        public SolidColorBrush Foreground
        {
            get {return _textColor;}
            set {_textColor = value;}
        }

        /// <summary>
        /// return a array of color names
        /// </summary>
        public static string [] ColorNames
        {
            get {return KnownColor.ColorNames;}

        }

        /// <summary>
        /// Return a hastable that contains colors
        /// </summary>
        public static Hashtable ColorTable
        {
            get {return KnownColor.ColorTable;}
        }

        // Some useful sizes
        public static double[] CommonlyUsedFontSizes = new double[] {
                  3.0d,   4.0d,   5.0d,   6.0d,   6.5d,
                  7.0d,   7.5d,   8.0d,   8.5d,   9.0d,  
                  9.5d,  10.0d,  10.5d,  11.0d,  11.5d,  
                 12.0d,  12.5d,  13.0d,  13.5d,  14.0d,
                 15.0d,  16.0d,  17.0d,  18.0d,  19.0d,
                 20.0d,  22.0d,  24.0d,  26.0d,  28.0d,  30.0d,  32.0d,  34.0d,  36.0d,  38.0d,
                 40.0d,  44.0d,  48.0d,  52.0d,  56.0d,  60.0d,  64.0d,  68.0d,  72.0d,  76.0d,
                 80.0d,  88.0d,  96.0d, 104.0d, 112.0d, 120.0d, 128.0d, 136.0d, 144.0d, 152.0d,
                160.0d, 176.0d, 192.0d, 208.0d, 224.0d, 240.0d, 256.0d, 272.0d, 288.0d, 304.0d,
                320.0d, 352.0d, 384.0d, 416.0d, 448.0d, 480.0d, 512.0d, 544.0d, 576.0d, 608.0d,
                640.0d};

        /// <summary>
        /// UsefulDefaultFontSizeIndex
        /// </summary>
        public static int UsefulDefaultFontSizeIndex = 15;


        /// <summary>
        /// return a collection of font sizes.
        /// </summary>
        public static ICollection FontSizes
        {
            get
            {
                ArrayList aList = new ArrayList();
                for (int i = 0; i <= CommonlyUsedFontSizes.Length; i++)
                {
                    aList.Add(CommonlyUsedFontSizes[i]);
                }
                return aList;
            }
        }

        /// <summary>
        /// get and set text decorations
        /// </summary>
        public TextDecorationCollection TextDecorationCollection
        {
            get {return _textDecorationCollection;}
            set {_textDecorationCollection = value;}
        }

        /// <summary>
        /// Get or set Typographic properties
        /// </summary>
        public Typography Typography
        {
            get {return Typography;}
            set {Typography = value;}
        }
    }
}