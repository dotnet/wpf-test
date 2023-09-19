// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Runtime.InteropServices;
using Microsoft.Test.Graphics.ReferenceRender;
using Microsoft.Test.Graphics.TestTypes;

using System.Reflection;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// General helpers for XAML based scenarios
    /// </summary>
    public sealed class ScenarioUtility
    {
        private ScenarioUtility()
        {
        }

        public static void UpdateCameraToLookAtPrimitive(GeometryModel3D primitive, Viewport3D viewport)
        {
            Rect3D bounds = primitive.Clone().Bounds;

            Point3D cameraLookAt = bounds.Location;
            cameraLookAt.X += bounds.Size.X / 2;
            cameraLookAt.Y += bounds.Size.Y / 2;
            cameraLookAt.Z += bounds.Size.Z / 2;

            Point3D cameraPosition = bounds.Location;
            cameraPosition.X += bounds.Size.X * 1.7;
            cameraPosition.Y += bounds.Size.Y * 1.7 + 10;
            cameraPosition.Z += bounds.Size.Z * 1.7;

            ProjectionCamera cam = (ProjectionCamera)viewport.Camera.Clone();
            cam.Position = cameraPosition;
            cam.LookDirection = cameraLookAt - cameraPosition;

            viewport.Camera = cam;
        }

        public static void Serialize(string fileName, FrameworkElement vp)
        {
            System.IO.FileStream fs = null;
            System.IO.StreamWriter sw = null;

            try
            {
                fs = System.IO.File.Open(fileName, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write);
                sw = new System.IO.StreamWriter(fs);
                string serialization = System.Windows.Markup.XamlWriter.Save(vp);
                sw.Write(serialization);
            }
            finally
            {
                if (sw != null)
                {
                    sw.Flush();
                    sw.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        public static System.IO.Stream GetStreamFromString(string text)
        {
            System.Text.Encoding encoding = new System.Text.UTF8Encoding(false, true);
            return new System.IO.MemoryStream(encoding.GetBytes(text));
        }

        public static void SetToleranceFromParameters(System.Collections.IDictionary v)
        {
            // Factory defaults
            RenderTolerance.ResetDefaults();

            // Scenario defaults
            //RenderTolerance.TextureLookUpTolerance = .55;
            //RenderTolerance.PixelToEdgeTolerance = 0.005;
            //RenderTolerance.SilhouetteEdgeTolerance = 1;
            //RenderTolerance.DefaultColorTolerance = Color.FromArgb( 8, 8, 8, 8 );

            // Parse for overrides
            if (v["PixelToEdgeTolerance"] != null)
            {
                RenderTolerance.PixelToEdgeTolerance = StringConverter.ToDouble(v["PixelToEdgeTolerance"] as string);
            }
            if (v["LightingRangeTolerance"] != null)
            {
                RenderTolerance.LightingRangeTolerance = StringConverter.ToDouble(v["LightingRangeTolerance"] as string);
            }
            if (v["SpotLightAngleTolerance"] != null)
            {
                RenderTolerance.SpotLightAngleTolerance = StringConverter.ToDouble(v["SpotLightAngleTolerance"] as string);
            }
            if (v["ZBufferTolerance"] != null)
            {
                RenderTolerance.ZBufferTolerance = StringConverter.ToDouble(v["ZBufferTolerance"] as string);
            }
            if (v["SpecularLightDotProductTolerance"] != null)
            {
                RenderTolerance.SpecularLightDotProductTolerance = StringConverter.ToDouble(v["SpecularLightDotProductTolerance"] as string);
            }
            if (v["DefaultColorTolerance"] != null)
            {
                RenderTolerance.DefaultColorTolerance = StringConverter.ToColor(v["DefaultColorTolerance"] as string);
            }
            if (v["TextureLookUpTolerance"] != null)
            {
                RenderTolerance.TextureLookUpTolerance = StringConverter.ToDouble(v["TextureLookUpTolerance"] as string);
            }
            if (v["SilhouetteEdgeTolerance"] != null)
            {
                RenderTolerance.SilhouetteEdgeTolerance = StringConverter.ToDouble(v["SilhouetteEdgeTolerance"] as string);
            }
        }

        public static void NavigateToPage(string uri)
        {
            Application.Current.Properties[pageURI] = uri;
            NavigationWindow NavWin = ((ScenarioApplication)Application.Current).NavWindow;

            NavWin.Navigate(new Uri("pack://application:,,,/scenario3d;component/Tests/" + uri + ".xaml"));
        }

        public static string[] GetCommandLineArgs()
        {
            // This asserts Environment permissions
            new System.Security.Permissions.EnvironmentPermission(
                System.Security.Permissions.PermissionState.Unrestricted).Assert();

            // Echo requested stuff
            return System.Environment.GetCommandLineArgs();
        }

        public static string CurrentTestPrefix
        {
            get
            {
                string page = (Application.Current as Application).Properties[pageURI] as string;
                if (page != null)
                {
                    page = page.Replace(".xaml", "");
                    page = page.Replace("pack://application:,,,/scenario3d;component/Tests/", "");
                }
                return page;
            }
        }

        public interface IHelp
        {
            string Help { get; }
        }

        public static void PrintHelp(IHelp help)
        {
            Logger logger = Logger.Create();
            logger.AddFailure("\nScenario: " + ScenarioUtility.CurrentTestPrefix + "\n" + help.Help);
            logger.Close();
        }

        const string pageURI = "MIL3D_PAGE_URI";

        /// <summary>
        /// This class is a variant of the Framework's Photographer class that omits a few memory copies
        /// that are not needed in this case.
        /// </summary>
        public sealed class ScreenCapture
        {
            public const int SRCCOPY = 0x00CC0020;

            public static BitmapSource TakeScreenCapture(IntPtr windowHandle)
            {
                RECT clientArea = new RECT(0, 0, 0, 0);
                GetClientRect(windowHandle, ref clientArea);

                if (clientArea.Width == 0 || clientArea.Height == 0)
                {
                    clientArea.bottom = 1024;
                    clientArea.right = 1200;
                }
                return CreateBitmap(clientArea, windowHandle);
            }

            private static BitmapSource CreateBitmap(RECT clientArea, IntPtr windowHandle)
            {
                IntPtr sourceDC = GetDC(windowHandle);

                if (sourceDC.ToInt32() == 0)
                {
                    throw new Exception("Could not access the window source's device context");
                }
                try
                {
                    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(clientArea.Width, clientArea.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);
                    IntPtr destination = graphics.GetHdc();
                    BitBlt(destination, 0, 0, clientArea.Width, clientArea.Height, sourceDC, 0, 0, SRCCOPY);
                    graphics.ReleaseHdc(destination);
                    graphics.Dispose();

                    System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
                    System.Drawing.Imaging.BitmapData bmpData = bitmap.LockBits(
                            rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                            bitmap.PixelFormat);
                    IntPtr pixels = bmpData.Scan0;

                    BitmapSource bimg = BitmapSource.Create(
                            bitmap.Width,
                            bitmap.Height,
                            96.0, 96.0, PixelFormats.Bgr24, null,
                            pixels, bmpData.Stride * bitmap.Height, bmpData.Stride);

                    return bimg;
                }
                finally
                {
                    ReleaseDC(windowHandle, sourceDC);
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct RECT
            {
                public RECT(int left, int top, int right, int bottom)
                {
                    this.left = left;
                    this.top = top;
                    this.right = right;
                    this.bottom = bottom;
                }

                public bool IsEmpty { get { return left >= right || top >= bottom; } }
                public int Height { get { return bottom - top; } }
                public int Width { get { return right - left; } }

                public int left;
                public int top;
                public int right;
                public int bottom;
            }

            [DllImport("user32.dll")]
            private static extern bool GetClientRect(IntPtr hwnd, [In, Out] ref RECT rc);

            [DllImport("user32.dll")]
            private static extern IntPtr GetDC(IntPtr hwnd);

            [DllImport("user32.dll")]
            private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

            [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
            private static extern bool BitBlt(IntPtr hdcDest, int destX, int destY, int destWidth, int destHeight, IntPtr hdcSrc, int sourceX, int sourceY, int dwRop);
        }
    }
}