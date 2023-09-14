using System;
using System.Collections;
using System.Windows;
using System.Windows.Media;

using WpfControlToolkit;

namespace Avalon.Test.ComponentModel.Actions
{
    namespace TreeMapTest
    {

        /// <summary>
        /// On-demand-sized circular enumerator of Brush colors.
        /// </summary>
        internal static class BrushCycler
        {
            static private Brush[] backgrounds = new Brush[]{Brushes.Yellow, 
                                                        Brushes.Blue,
                                                        Brushes.Beige,
                                                        Brushes.Chartreuse,
                                                        Brushes.Gold,
                                                        Brushes.Lavender,
                                                        Brushes.Magenta,
                                                        Brushes.Cyan,
                                                        Brushes.LimeGreen,
                                                        Brushes.Chocolate,
                                                        Brushes.MediumVioletRed,
                                                        Brushes.Salmon,
                                                        Brushes.SeaGreen,
                                                        Brushes.Orange,
                                                        Brushes.Red,
                                                        Brushes.Plum,
                                                        Brushes.Gray,
                                                        Brushes.Tomato,
                                                        Brushes.Violet,
                                                        Brushes.DeepSkyBlue};
            private static Int32 index = 0;
            public static IEnumerable GetBrushes(Int32 Count)
            {
                for (Int32 i = 0; i < Count; ++i)
                {
                    Int32 idx = (index + i) % backgrounds.Length;
                    yield return backgrounds[idx];
                }
                index = (index + Count) % backgrounds.Length;
            }
        }
    }
}
