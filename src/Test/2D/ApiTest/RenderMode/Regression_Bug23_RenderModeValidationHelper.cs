// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Interop;
using Microsoft.Test.Display;

namespace Microsoft.Test.Graphics.Regression
{
    class RenderModeValidationHelper
    {
        [DllImport("milctrl.dll", EntryPoint = "MediaControl_CanAttach")]
        private static extern int CanAttach([MarshalAs(UnmanagedType.LPWStr)] string sectionName, out bool canAccess);

        [DllImport("milctrl.dll", EntryPoint = "MediaControl_Attach")]
        private static extern int Attach([MarshalAs(UnmanagedType.LPWStr)] string sectionName, out MediaControlHandle debugControl);

        [DllImport("milctrl.dll", EntryPoint = "MediaControl_Release")]
        private static extern void Release(IntPtr pMediaControl);

        [DllImport("milctrl.dll", EntryPoint = "MediaControl_GetDataPtr")]
        private static extern int GetDataPtr(MediaControlHandle debugControl, out IntPtr value);
        
        [StructLayout(LayoutKind.Sequential)]
        private class MediaControlFile
        {
            public UInt32 ShowDirtyRegionOverlay;
            public UInt32 ClearBackBufferBeforeRendering;
            public UInt32 DisableDirtyRegionSupport;
            public UInt32 EnableTranslucentRendering;
            public UInt32 FrameRate;
            public UInt32 DirtyRectAddRate;
            public UInt32 PercentElapsedTimeForComposition;

            public UInt32 TrianglesPerFrame;
            public UInt32 TrianglesPerFrameMax;
            public UInt32 TrianglesPerFrameCumulative;

            public UInt32 PixelsFilledPerFrame;
            public UInt32 PixelsFilledPerFrameMax;
            public UInt32 PixelsFilledPerFrameCumulative;

            public UInt32 TextureUpdatesPerFrame;
            public UInt32 TextureUpdatesPerFrameMax;
            public UInt32 TextureUpdatesPerFrameCumulative;

            public UInt32 VideoMemoryUsage;
            public UInt32 VideoMemoryUsageMin;
            public UInt32 VideoMemoryUsageMax;

            public int NumSoftwareRenderTargets;
            public int NumHardwareRenderTargets;

            // Provides a per-frame count of hw IRTs
            public UInt32 NumHardwareIntermediateRenderTargets;
            public UInt32 NumHardwareIntermediateRenderTargetsMax;

            // Provides a per-frame count of sw IRTs
            public UInt32 NumSoftwareIntermediateRenderTargets;
            public UInt32 NumSoftwareIntermediateRenderTargetsMax;

            public UInt32 AlphaEffectsDisabled;
            public UInt32 PrimitiveSoftwareFallbackDisabled;
            public UInt32 PurpleSoftwareFallback;
            public UInt32 FantScalerDisabled;
            public UInt32 Draw3DDisabled;
        }

        public static int GetRenderTargetNumber(RenderMode mode)
        {
            bool canAccess;
            MediaControlHandle debugControl;
            IntPtr pFile;
            string sectionName = "MilCore-" + Process.GetCurrentProcess().Id.ToString(System.Globalization.CultureInfo.InvariantCulture);

            CanAttach(sectionName, out canAccess);

            if (canAccess)
            {
                Attach(sectionName, out debugControl);
                using (debugControl)
                {
                    GetDataPtr(debugControl, out pFile);
                    MediaControlFile pM = new MediaControlFile();
                    Marshal.PtrToStructure(pFile, pM);
                    
                    if (mode == RenderMode.Default)
                    {
                        return pM.NumHardwareRenderTargets / Monitor.GetDisplayCount();
                    }
                    else
                    {
                        return pM.NumSoftwareRenderTargets / Monitor.GetDisplayCount();
                    }
                }
            }
            else
            {
                return -1;
            }

        }



        private class MediaControlHandle : SafeHandle
        {
            internal MediaControlHandle()
                : base(IntPtr.Zero, true)
            {
            }

            public override bool IsInvalid
            {
                get
                {
                    return handle == IntPtr.Zero;
                }
            }

            protected override bool ReleaseHandle()
            {
                if (!IsInvalid)
                {
                    Release(handle);
                }
                return true;
            }
        }
    }
}