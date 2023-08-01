// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  DwmApi
//
//  Managed wrapper of DWM API functions.
//

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace Avalon.Test.CoreUI.Dwm
{
    /// <summary>
    /// 
    /// </summary>
    public class DwmApi
    {
        //
        //  Composition
        //

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fEnabled"></param>
        /// <returns></returns>
        [DllImport("DwmApi.dll")]
        public static extern int DwmEnableComposition(
            bool fEnabled);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fEnabled"></param>
        /// <returns></returns>
        [DllImport("DwmApi.dll", SetLastError=true)]
        public static extern int DwmIsCompositionEnabled(
            ref bool fEnabled);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DllImport("DwmApi.dll", EntryPoint = "#103")]
        public static extern int DwmpRestartComposition();



        //
        //  Thumbnails
        //

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct DWM_THUMBNAIL_PROPERTIES
        {   
            /// <summary>
            /// 
            /// </summary>
            public int dwFlags;
            /// <summary>
            /// 
            /// </summary>
            public Win32.RECT rcDestination;
            /// <summary>
            /// 
            /// </summary>
            public Win32.RECT rcSource;
            /// <summary>
            /// 
            /// </summary>
            public byte opacity;
            /// <summary>
            /// 
            /// </summary>
            public bool fVisible;
            /// <summary>
            /// 
            /// </summary>
            public bool fSourceClientAreaOnly;
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwndDestination"></param>
        /// <param name="hwndSource"></param>
        /// <param name="minimizedSize"></param>
        /// <param name="hThumbnailId"></param>
        /// <returns></returns>
        [DllImport("DwmApi.dll")]
        public static extern int DwmRegisterThumbnail(
            IntPtr hwndDestination,
            IntPtr hwndSource,
            ref Win32.SIZE minimizedSize,
            ref IntPtr hThumbnailId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hThumbnailId"></param>
        /// <returns></returns>
        [DllImport("DwmApi.dll")]
        public static extern int DwmUnregisterThumbnail(
            IntPtr hThumbnailId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hThumbnailId"></param>
        /// <param name="tp"></param>
        /// <returns></returns>
        [DllImport("DwmApi.dll")]
        public static extern int DwmUpdateThumbnailProperties(
            IntPtr hThumbnailId,
            ref DWM_THUMBNAIL_PROPERTIES tp);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hThumbnailId"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [DllImport("DwmApi.dll")]
        public static extern int DwmQueryThumbnailSourceSize(
            IntPtr hThumbnailId,
            ref Win32.SIZE size);

        //
        //  Window Attributes
        //

        /// <summary>
        /// 
        /// </summary>
        public const int DWMNCRP_USEWINDOWSTYLE = 0;  // Enable/disable non-client rendering based on window style
        /// <summary>
        /// 
        /// </summary>
        public const int DWMNCRP_DISABLED = 1;        // Disabled non-client rendering; window style is ignored
        /// <summary>
        /// 
        /// </summary>
        public const int DWMNCRP_ENABLED = 2;         // Enabled non-client rendering; window style is ignored

        /// <summary>
        /// 
        /// </summary>
        public const int DWMWA_NCRENDERING_ENABLED = 1;       // Enable/disable non-client rendering Use DWMNCRP_* values
        /// <summary>
        /// 
        /// </summary>
        public const int DWMWA_NCRENDERING_POLICY = 2;        // Non-client rendering policy
        /// <summary>
        /// 
        /// </summary>
        public const int DWMWA_TRANSITIONS_FORCEDISABLED = 3; // Potentially enable/forcibly disable transitions 0 or 1

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="dwAttributeToSet"></param>
        /// <param name="pvAttributeValue"></param>
        /// <param name="cbAttribute"></param>
        /// <returns></returns>
        [DllImport("DwmApi.dll")]
        public static extern int DwmSetWindowAttribute(
            IntPtr hwnd,
            uint dwAttributeToSet, //DWMWA_* values
            IntPtr pvAttributeValue,
            uint cbAttribute);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="dwAttributeToGet"></param>
        /// <param name="pvAttributeValue"></param>
        /// <param name="cbAttribute"></param>
        /// <returns></returns>
        [DllImport("DwmApi.dll")]
        public static extern int DwmGetWindowAttribute(
            IntPtr hwnd,
            uint dwAttributeToGet, //DWMWA_* values
            IntPtr pvAttributeValue,
            uint cbAttribute);



        //
        //  Client Area Blur
        //

        /// <summary>
        /// 
        /// </summary>
        public const int DWM_BB_ENABLE = 0x00000001;  // fEnable has been specified
        /// <summary>
        /// 
        /// </summary>
        public const int DWM_BB_BLURREGION = 0x00000002;  // hRgnBlur has been specified
        /// <summary>
        /// 
        /// </summary>
        public const int DWM_BB_TRANSITIONONMAXIMIZED = 0x00000004;  // fTransitionOnMaximized has been specified

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct DWM_BLURBEHIND
        {
            /// <summary>
            /// 
            /// </summary>
            public int dwFlags;
            /// <summary>
            /// 
            /// </summary>
            public bool fEnable;
            /// <summary>
            /// 
            /// </summary>
            public IntPtr hRgnBlur;
            /// <summary>
            /// 
            /// </summary>
            public bool fTransitionOnMaximized;
        };

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            /// <summary>
            /// 
            /// </summary>
            public int cxLeftWidth;      // width of left border that retains its size
            /// <summary>
            /// 
            /// </summary>
            public int cxRightWidth;     // width of right border that retains its size
            /// <summary>
            /// 
            /// </summary>
            public int cyTopHeight;      // height of top border that retains its size
            /// <summary>
            /// 
            /// </summary>
            public int cyBottomHeight;   // height of bottom border that retains its size
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="bb"></param>
        /// <returns></returns>
        [DllImport("DwmApi.dll")]
        public static extern int DwmEnableBlurBehindWindow(
            IntPtr hwnd,
            ref DWM_BLURBEHIND bb);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        [DllImport("DwmApi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(
            IntPtr hwnd,
            ref MARGINS m);



        //
        //  Colorization
        //


        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="opaqueBlend"></param>
        /// <returns></returns>
        [DllImport("DwmApi.dll")]
        public static extern int DwmGetColorizationColor(
            ref int color, ref int opaqueBlend);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="isOpaqueBlend"></param>
        /// <param name="isPreviewOnly"></param>
        /// <returns></returns>
        [DllImport("DwmApi.dll", EntryPoint = "#104")]
        public static extern int DwmpSetColorizationColor(
            int color,
            bool isOpaqueBlend,
            bool isPreviewOnly);

        /// <summary>
        /// 
        /// </summary>
        public const int WM_DWMCOMPOSITIONCHANGED = 0x031E;
        /// <summary>
        /// 
        /// </summary>
        public const int WM_DWMNCRENDERINGCHANGED = 0x031F;
        /// <summary>
        /// 
        /// </summary>
        public const int WM_DWMCOLORIZATIONCOLORCHANGED  = 0x0320;
        /// <summary>
        /// 
        /// </summary>
        public const int WM_DWMWINDOWMAXIMIZEDCHANGE = 0x0321;

    }

}

