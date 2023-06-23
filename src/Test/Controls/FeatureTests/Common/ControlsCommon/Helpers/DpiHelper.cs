using System;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Helper class for high dpi test
    /// </summary>
    public static class DpiHelper
    {
        /// <summary>
        /// Get DPI of the system
        /// </summary>
        /// <returns></returns>
        public static float GetDpi()
        {
            using (System.Drawing.Graphics graph = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
            {
                if (graph == null)
                {
                    throw new NullReferenceException("Graphics not found");
                }

                if (!graph.DpiX.Equals(graph.DpiY))
                {
                    throw new ArithmeticException("DpiX != DpiY");
                }

                return graph.DpiX;
            }
        }

        // Default DPI is 96.0
        public const float DEFAULT_DPI = 96.0f;

        /// <summary>
        /// Get actual length in screen from the unit of length in Avalon
        /// 
        /// Avalon uses its own unit to measure size. The unit is independent of pixels in screen.
        /// The relationship between the two units are explained below.
        /// When DPI=96, one unit in Avalon means one pixel in screen.
        /// When DPI=120 or others, one unit in Avalon will mean DPI/96 pixels in screen.
        /// </summary>
        /// <param name="length">the length measured in Avalon units</param>
        /// <returns>the length measured in pixels in screen</returns>
        public static int GetScreenLength(int length)
        {
            return (int)Math.Round(length / DEFAULT_DPI * GetDpi());
        }

        /// <summary>
        /// WPF has its own pixel system in double value type, and screen pixel includes different DPIs is in int value type.
        /// In 96 dpi, wpf and screen pixels are the same, but other dpi, we need to convert wpf logical pixel to screen physical 
        /// pixel by using formula (wpf pixel value * dpi / 96.0).
        /// </summary>
        /// <param name="logicalPixel">Logical(WPF) pixel value</param>
        /// <returns>Physical(Screen) pixel value</returns>
        public static int ConvertToPhysicalPixel(double logicalPixel)
        {
            return Convert.ToInt32(logicalPixel * GetDpi() / 96.0);
        }
    }
}
