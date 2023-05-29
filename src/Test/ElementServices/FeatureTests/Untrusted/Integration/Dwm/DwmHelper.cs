// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.IO;
using Avalon.Test.CoreUI.Trusted;
using System.Windows.Threading;
using Microsoft.Test.WindowsForms;

namespace Avalon.Test.CoreUI.Dwm
{       
    /// <summary>
    /// 
    /// </summary>
    public class DwmAPIHelper
    {

        private const int S_OK = 0;
        private HwndSourceHook _hwndSourceHook = null;
        private Hashtable _messageStore;

        /// <summary>
        /// Disable DWM
        /// Return true/false to indicate success/failure
        /// </summary>
        /// <returns></returns>
        public bool EnableDWM()
        {
            return EnableDWM(true);
        }

        /// <summary>
        /// Enable DWM
        /// Return true/false to indicate success/failure
        /// </summary>
        /// <returns></returns>
        public bool DisableDWM()
        {
            return EnableDWM(false);
        }

        /// <summary>
        /// Return true if enabled else false
        /// </summary>
        /// <returns></returns>
        public bool IsCompositionEnabled()
        {
            bool isCompositionEnabled = false;

            //Check dwmapi.dll
            if (!DwmAPIExists())
            {
                return false;
            }

            int ret = DwmApi.DwmIsCompositionEnabled(ref isCompositionEnabled);

            if (ret != S_OK)
            {
                CoreLogger.LogStatus(("DwmIsCompositionEnabled call failed with error code (HRESULT) " + ret.ToString()));
                return false;
            }

            if (isCompositionEnabled)
            {
                CoreLogger.LogStatus("Dwm CompositionEnabled");
            }
            else
            {
                CoreLogger.LogStatus("Dwm Composition not Enabled");
            }

            return isCompositionEnabled;
        }

        /// <summary>
        /// Blur behind the entire window
        /// Return true/false to indicate success/failure
        /// </summary>
        /// <param name="surfaceObject"></param>
        /// <returns></returns>
        public bool BlurBehindEntireWindow(object surfaceObject)
        {
            return BlurBehindWindow(surfaceObject, IntPtr.Zero);
        }

        /// <summary>
        /// Blur behind avalon client area
        /// Return true/false to indicate success/failure
        /// </summary>
        /// <param name="surfaceObject"></param>
        /// <returns></returns>
        public bool BlurBehindAvalonClientArea(object surfaceObject)
        {
            if (surfaceObject == null)
            {
                CoreLogger.LogStatus("surfaceObject null");
                return false;
            }

            Win32.RECT r = new Win32.RECT();

            Win32.GetClientRect(GetHwnd(surfaceObject), ref r);

            IntPtr hrgn = Win32.CreateRectRgn(0, 0, r.right - r.left, r.bottom - r.top);

            return BlurBehindWindow(surfaceObject, hrgn);
        }

        /// <summary>
        /// Blur behind window - applying glass effect
        /// Return true/false to indicate success/failure
        /// </summary>
        /// <param name="surfaceObject"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public bool BlurBehindWindow(object surfaceObject, IntPtr region)
        {
            if (!IsCompositionEnabled())
            {
                return false;
            }

            if (surfaceObject == null)
            {
                CoreLogger.LogStatus("surfaceObject null");
                return false;
            }

            ApplyWorkAround(surfaceObject);

            DwmApi.DWM_BLURBEHIND bb = new DwmApi.DWM_BLURBEHIND();
            bb.dwFlags = DwmApi.DWM_BB_ENABLE | DwmApi.DWM_BB_BLURREGION;
            bb.fEnable = true;
            bb.hRgnBlur = region;

            IntPtr handle = GetHwnd(surfaceObject);

            if (handle != IntPtr.Zero)
            {
                CoreLogger.LogStatus("Window handle null");
            }

            int ret = DwmApi.DwmEnableBlurBehindWindow(handle, ref bb);

            if (ret != S_OK)
            {
                CoreLogger.LogStatus(("DwmIsCompositionEnabled call failed with error code (HRESULT) " + ret.ToString()));
                return false;
            }

            return true;
        }


       
        /// <summary>
        /// Extend Window frame to client area
        /// To enable blur using DwmExtendFrameIntoClientArea, 
        /// MARGINS are used to indicate how much to extend into the client area. 
        /// </summary>
        /// <param name="surfaceObject"></param>
        /// <param name="margin"></param>/// 
        /// <returns></returns>
        /// <remarks>Affects the Window this visual connected</remarks>
        public bool ExtendWindowFrameToClientArea(object surfaceObject, int margin)
        {

            if (surfaceObject == null)
            {
                CoreLogger.LogStatus("surfaceObject null");
                return false;
            }
            
            //Set margins
            DwmApi.MARGINS margins = new DwmApi.MARGINS();
            margins.cxLeftWidth = margin;
            margins.cxRightWidth = margin;
            margins.cyTopHeight = margin;
            margins.cyBottomHeight = margin;

            if (!ApplyWorkAround(surfaceObject))
                return false;

            IntPtr handle = GetHwnd(surfaceObject);

            if (handle == IntPtr.Zero)
            {
                CoreLogger.LogStatus("handle null");
                return false;
            }

            int ret = DwmApi.DwmExtendFrameIntoClientArea(handle, ref margins);

            if (ret != S_OK)
            {
                CoreLogger.LogStatus(("DwmIsCompositionEnabled call failed with error code (HRESULT) " + ret.ToString()));
                return false;
            }

            return true;
        }

        /// <summary>
        /// This will disable non client rendering
        /// Return true/false to indicate success/failure
        /// </summary>
        /// <param name="surfaceObject"></param>
        /// <returns></returns>
        public bool DisableNonClientRendering(object surfaceObject)
        {

            if (surfaceObject == null)
            {
                CoreLogger.LogStatus("surfaceObject null");
                return false;
            }

            IntPtr data = IntPtr.Zero;

            IntPtr handle = GetHwnd(surfaceObject);

            if (handle == IntPtr.Zero)
            {
                CoreLogger.LogStatus("handle null");
                return false;
            }

            try
            {
                data = System.Runtime.InteropServices.Marshal.AllocHGlobal(System.Runtime.InteropServices.Marshal.SizeOf(DwmApi.DWMNCRP_DISABLED.GetType()));
                System.Runtime.InteropServices.Marshal.StructureToPtr(DwmApi.DWMNCRP_DISABLED, data, false);

                int ret = DwmApi.DwmSetWindowAttribute(
                    handle,
                    DwmApi.DWMWA_NCRENDERING_POLICY,
                    data,
                    (uint)System.Runtime.InteropServices.Marshal.SizeOf(DwmApi.DWMNCRP_DISABLED.GetType()));

                if (ret != S_OK)
                {
                    CoreLogger.LogStatus(("DwmIsCompositionEnabled call failed with error code (HRESULT) " + ret.ToString()));
                    return false;
                }

            }
            finally
            { 
                if (data != IntPtr.Zero)
                {
                    System.Runtime.InteropServices.Marshal.FreeHGlobal(data);
                }
            }

            return true;
        }

        /// <summary>
        /// This will enable non client rendering
        /// Return true/false to indicate success/failure
        /// </summary>
        /// <param name="surfaceObject"></param>
        /// <returns></returns>
        public bool EnableNonClientRendering(object surfaceObject)
        {

            if (surfaceObject == null)
            {
                CoreLogger.LogStatus("surfaceObject null");
                return false;
            }
            
            IntPtr data = IntPtr.Zero;

            IntPtr handle = GetHwnd(surfaceObject);

            if (handle == IntPtr.Zero)
            {
                CoreLogger.LogStatus("handle null");
                return false;
            }

            try
            {
                data = System.Runtime.InteropServices.Marshal.AllocHGlobal(System.Runtime.InteropServices.Marshal.SizeOf(DwmApi.DWMNCRP_ENABLED.GetType()));
                System.Runtime.InteropServices.Marshal.StructureToPtr(DwmApi.DWMNCRP_ENABLED, data, false);

                int ret = DwmApi.DwmSetWindowAttribute(
                    handle,
                    DwmApi.DWMWA_NCRENDERING_POLICY,
                    data,
                    (uint)System.Runtime.InteropServices.Marshal.SizeOf(DwmApi.DWMNCRP_ENABLED.GetType()));

                if (ret != S_OK)
                {
                    CoreLogger.LogStatus(("DwmIsCompositionEnabled call failed with error code (HRESULT) " + ret.ToString()));
                    return false;
                }

            }
            finally
            {
                if (data != IntPtr.Zero)
                {
                    System.Runtime.InteropServices.Marshal.FreeHGlobal(data);
                }
            }

            return true;
        }

        /// <summary>
        /// Insert a Source
        /// </summary>
        /// <param name="destWindow"></param>
        /// <param name="sourceWindow"></param>
        /// <param name="destRect"></param>
        /// <param name="clientAreaOnly"></param>
        /// <returns></returns>
        public IntPtr InsertAvalonWindowsAsThumbNailToAvalonWindow(IntPtr destWindow, IntPtr sourceWindow, Win32.RECT destRect, bool clientAreaOnly)
        {
            IntPtr hThumbNail = IntPtr.Zero;
            Win32.SIZE size = new Win32.SIZE();

            int retVal = DwmApi.DwmRegisterThumbnail(destWindow, sourceWindow, ref size, ref hThumbNail);

            if (retVal != S_OK)
            {
                CoreLogger.LogStatus("FAIL - DwmRegisterThumbnail failed with HRESULT " + retVal.ToString());
                return hThumbNail;
            }

            if (hThumbNail == IntPtr.Zero)
            {
                CoreLogger.LogStatus("FAIL - Handle to thumb nail is null ");
                return hThumbNail;
            }

            DwmApi.DWM_THUMBNAIL_PROPERTIES dwmProp = new DwmApi.DWM_THUMBNAIL_PROPERTIES();
            dwmProp.dwFlags = 0x00000008 | 0x00000010 | 0x00000001;
            dwmProp.fSourceClientAreaOnly = clientAreaOnly;
            dwmProp.fVisible = true;
            dwmProp.rcDestination = destRect;

            retVal = DwmApi.DwmUpdateThumbnailProperties(hThumbNail, ref dwmProp);

            if (retVal != S_OK)
            {
                CoreLogger.LogStatus("DwmUpdateThumbnailProperties failed with HRESULT " + retVal.ToString());
                return hThumbNail;
            }

            return hThumbNail;
        }


        /// <summary>
        /// Check dwmapi.dll exists in the system directory
        /// Return true if exists else false
        /// </summary>
        /// <returns></returns>
        private bool DwmAPIExists()
        {
            string dwmPath = Path.Combine(Environment.SystemDirectory, "dwmapi.dll");

            if (File.Exists(dwmPath))
            {
                CoreLogger.LogStatus("dwmapi.dll exists in the system drive");
                return true;
            }
            else
            {
                CoreLogger.LogStatus("dwmapi.dll not exists in the system drive");
                return false;
            }
        }

        /// <summary>
        /// See 


        private bool ApplyWorkAround(object surfaceObject)
        {
            HwndSource hSource = GetHwndSource(surfaceObject);

            hSource.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

            if (surfaceObject is Window)
                (surfaceObject as Window).Background = Brushes.Transparent;
   
            return true;
        }

        /// <summary>
        /// Start collecting DWM Window Messages
        /// </summary>
        /// <param name="surfaceObject"></param>
        /// <returns></returns>
        public bool StartDWMWindowMessageCollect(object surfaceObject)
        {
            _messageStore = new Hashtable();

            HwndSource hSource = GetHwndSource(surfaceObject);

            if (hSource == null)
            {
                CoreLogger.LogStatus("HwndSource null");
                return false;
            }

            _hwndSourceHook = new HwndSourceHook(WindowMessageHandler);
            hSource.AddHook(_hwndSourceHook);

            return true;
        }

        /// <summary>
        /// Stop collecting DWM Window Messages
        /// </summary>
        /// <param name="surfaceObject"></param>
        public void StopDWMWindowMessageCollect(object surfaceObject)
        {
            HwndSource hSource = GetHwndSource(surfaceObject);

            if (hSource == null)
            {
                CoreLogger.LogStatus("HwndSource null");
                return;
            }

            if (_hwndSourceHook != null)
            {
                CoreLogger.LogStatus("No HwndSourceHook previously set");
                return;
            }
            
            hSource.RemoveHook(_hwndSourceHook);

        }


        /// <summary>
        /// Helper function to Window Handle
        /// Return window handle if success else IntPtr.Zero
        /// </summary>
        /// <param name="surfaceObject"></param>
        /// <returns></returns>
        public IntPtr GetHwnd(object surfaceObject)
        {

            if (surfaceObject == null)
            {
                CoreLogger.LogStatus("surfaceObject null");
                return IntPtr.Zero;
            }

            return GetHwndSource(surfaceObject).Handle;

        }

        private HwndSource GetHwndSource(object surfaceObject)
        {
            if (surfaceObject is WindowsFormSource)
            {
                return (surfaceObject as WindowsFormSource).CurrentHwndSource;
            }

            if (surfaceObject is HwndSource)
            {
                return surfaceObject as HwndSource;
            }
            else
            {
                PresentationSource pSource = PresentationSource.FromVisual(surfaceObject as Visual);
                return (pSource==null)?null:(pSource as HwndSource);
            }
        }

        /// <summary>
        /// Enable/Disable Desktop composition
        /// </summary>
        /// <param name="enable"></param>
        private bool EnableDWM(bool enable)
        {
            if (!DwmAPIExists())
                return false;
            
            int ret = DwmApi.DwmEnableComposition(enable);

            if (ret != S_OK)
            {
                CoreLogger.LogStatus(("DwmIsCompositionEnabled call failed with error code (HRESULT) " + ret.ToString()));
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="bHandled"></param>
        /// <returns></returns>
        private IntPtr WindowMessageHandler(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool bHandled)
        {
            switch (msg)
            {
                case DwmApi.WM_DWMCOMPOSITIONCHANGED:
                    CoreLogger.LogStatus("WM_DWMCOMPOSITIONCHANGED");
                    AddToHashTable(DwmApi.WM_DWMCOMPOSITIONCHANGED.ToString());
                    break;
                case DwmApi.WM_DWMNCRENDERINGCHANGED:
                    CoreLogger.LogStatus("WM_DWMNCRENDERINGCHANGED");
                    AddToHashTable(DwmApi.WM_DWMNCRENDERINGCHANGED.ToString());
                    break;
                case DwmApi.WM_DWMCOLORIZATIONCOLORCHANGED:
                    CoreLogger.LogStatus("WM_DWMCOLORIZATIONCOLORCHANGED");
                    AddToHashTable(DwmApi.WM_DWMCOLORIZATIONCOLORCHANGED.ToString());
                    break;
                case DwmApi.WM_DWMWINDOWMAXIMIZEDCHANGE:
                    CoreLogger.LogStatus("WM_DWMWINDOWMAXIMIZEDCHANGE");
                    AddToHashTable(DwmApi.WM_DWMWINDOWMAXIMIZEDCHANGE.ToString());
                    break;
            }

            return IntPtr.Zero;
        }

        /// <summary>  
        /// </summary>
        /// <param name="key"></param>
        private void AddToHashTable(string key)
        {
            if (_messageStore.Contains(key))
            {
                int count = Convert.ToInt32(_messageStore[key]) + 1;
                _messageStore[key]=count+1;
            }
            else
            {
                _messageStore.Add(key, 1);
            }

        }

    }
}
