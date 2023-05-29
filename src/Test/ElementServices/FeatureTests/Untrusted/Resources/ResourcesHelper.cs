// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon core property system test automation
// (C) Microsoft corporation 2003
// File
//      ResourceHelper.cs
// Description
//      Contains all the tree related utility functions
// History
//      [1] Microsoft created
using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Avalon.Test.CoreUI;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Threading;
using System.Windows.Threading;

using System.Windows.Input;
using System.Windows.Markup;

using System.Windows.Documents;


namespace Avalon.Test.CoreUI.Resources
{
        /// <summary>
    /// This is the TestResouceDictionaty class which can create ResouceDictionaries
    /// </summary>
    public class ResourceDictionaryHelper
    {
        /// <summary>
        /// Constructor for TestResourceDictionary
        /// </summary>
        public ResourceDictionaryHelper()
        {

        }


        /// <summary>
        /// Create ResourceDictionary for VerticalAlignment
        /// </summary>
        /// <remarks>
        /// return a ResourceDictionary Object which has vlaues set for VericalAlignment
        ///
        /// </remarks>

        public ResourceDictionary CreateVerticalAlignments ()
        {
            using(CoreLogger.AutoStatus(this.GetType().Name + ".CreateVerticalAlignments entered"))
            {
                ResourceDictionary rd;
                rd = new ResourceDictionary ();
                rd.Add ("VAlignTop", VerticalAlignment.Top);
                rd.Add ("VAlignBottom", VerticalAlignment.Bottom);
                rd.Add ("VAlignCenter", VerticalAlignment.Center);
                return rd;
            }
        }
        /// <summary>
        /// Create ResourceDictionary for Brushes
        /// </summary>
        /// <remarks>
        /// return a ResourceDictionary Object which has vlaues set for Brushes - red,green,blue,white,black
        ///
        /// </remarks>
        public ResourceDictionary CreateSolidColorBrushes ()
        {
            using(CoreLogger.AutoStatus(this.GetType().Name + ".CreateSolidColorBrushs entered"))
            {
                ResourceDictionary rd;
                rd = new ResourceDictionary ();
                rd.Add ("BrushRed", Brushes.Red);
                rd.Add ("BrushGreen", Brushes.Green);
                rd.Add ("BrushBlue", Brushes.Blue);
                rd.Add ("BrushBlack", Brushes.Black);
                rd.Add ("BrushWhite", Brushes.White);
                return rd;
            }
        }
        /// <summary>
        /// Create ResourceDictionary for Brushes+FontWeights
        /// </summary>
        /// <remarks>
        /// return a ResourceDictionary Object which has vlaues set for Brushes - red,green,blue,white,black and FontWeight - bold, medium, thin
        ///
        /// </remarks>
        public ResourceDictionary CreateBrushesFontWeights ()
        {
            using(CoreLogger.AutoStatus(this.GetType().Name + ".CreateBrushesFontWeights entered"))
            {
                ResourceDictionary rd;
                rd = new ResourceDictionary ();
                rd.Add ("BrushRed", Brushes.Red);
                rd.Add ("BrushGreen", Brushes.Green);
                rd.Add ("BrushBlue", Brushes.Blue);
                rd.Add ("BrushBlack", Brushes.Black);
                rd.Add ("BrushWhite", Brushes.White);
                rd.Add ("FontBold", FontWeights.Bold);
                rd.Add ("FontMedium", FontWeights.Medium);
                rd.Add ("FontThin", FontWeights.Thin);
                return rd;
            }
        }

        /// <summary>
        /// Create ResourceDictionary for FontWeights, FontStyles
        /// </summary>
        /// <remarks>
        /// return a ResourceDictionary Object which has vlaues set for FontWeight - bold, medium, thin
        ///
        /// </remarks>
        public ResourceDictionary CreateFontWeightsStyles ()
        {
            using(CoreLogger.AutoStatus(this.GetType().Name + ".CreateFontWeightsStyles entered"))
            {

                ResourceDictionary rd;

                rd = new ResourceDictionary ();
                rd.Add ("FontBold", FontWeights.Bold);
                rd.Add ("FontMedium", FontWeights.Medium);
                rd.Add ("FontThin", FontWeights.Thin);
                rd.Add ("FontLight", FontWeights.Light);
                rd.Add ("FontStyleNormal", FontStyles.Normal);
                rd.Add ("FontStyleItalic", FontStyles.Italic);
                return rd;
            }

        }

        /// <summary>
        /// Create ResourceDictionary for FontWeights with different pairs for testing invalidation, search
        /// </summary>
        /// <remarks>
        /// return a ResourceDictionary Object which has vlaues set for FontWeight - bold, medium, thin
        ///
        /// </remarks>
        public ResourceDictionary CreateTempFontWeights ()
        {
            using(CoreLogger.AutoStatus(this.GetType().Name + ".CreateTempFontWeights entered"))
            {
                ResourceDictionary rd;

                rd = new ResourceDictionary ();
                rd.Add ("FontBold", FontWeights.Medium);
                rd.Add ("FontMedium", FontWeights.Thin);
                rd.Add ("FontThin", FontWeights.Bold);
                return rd;
            }
        }
        /// <summary>
        /// Create ResourceDictionary for Greeting
        /// </summary>
        /// <remarks>
        /// return a ResourceDictionary Object which has vlaues set for greeting
        ///
        /// </remarks>

        public ResourceDictionary CreateGreetings ()
        {
            using(CoreLogger.AutoStatus(this.GetType().Name + ".CreateGreetings entered"))
            {
                ResourceDictionary rd;
                rd = new ResourceDictionary ();
                rd.Add ("Hello", "Hello");
                rd.Add ("Hey", "Hey");
                return rd;
            }
        }

        /// <summary>
        /// Create ResourceDictionary for Numbers
        /// </summary>
        /// <remarks>
        /// return a ResourceDictionary Object which has vlaues set for Numbers 1,2,3
        ///
        /// </remarks>
        public ResourceDictionary CreateNumbers ()
        {
            using(CoreLogger.AutoStatus(this.GetType().Name + ".CreateNumbers entered"))
            {
                ResourceDictionary rd;
                rd = new ResourceDictionary ();
                rd.Add ("1", 1);
                rd.Add ("2", 2);
                rd.Add ("3", 3);
                return rd;
            }
        }

        /// <summary>
        /// Create ResourceDictionary for CharacterCasing
        /// </summary>
        /// <returns></returns>
        public ResourceDictionary CreateCharacterCasing()
        {
            using(CoreLogger.AutoStatus(this.GetType().Name + ".CreateCharacterCasing entered"))
            {
                ResourceDictionary rd;
                rd = new ResourceDictionary ();
                rd.Add("CCNormal", CharacterCasing.Normal);
                rd.Add("CCLower", CharacterCasing.Lower);
                rd.Add("CCUpper", CharacterCasing.Upper);
                return rd;
            }
        }
    }
}
