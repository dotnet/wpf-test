// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************************
*    Source Control Information
*    
 
  
*    Revision:         $Revision: 2 $
 
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;
using Microsoft.Test.Win32;
using System.Security;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using Microsoft.Test.Threading;

namespace Avalon.Test.CoreUI.Trusted
{
    /// <summary>
    /// Abstracts all the surfaces that exists at the Core
    /// product layer.
    /// 
    /// Surfaces: 
    ///     HwndSource
    /// </summary>
    public class SurfaceCore : Surface
    {
        /// <summary>
        /// </summary>
        static SurfaceCore()
        {
            Surface.RegisterSurface("HwndSource", typeof(HwndSource));
            Surface.RegisterSurface("LayeredHwndSource", typeof(HwndSource));

            // Check if we should log window messages.
            if (null != Environment.GetEnvironmentVariable("LOGWM"))
            {
                s_shouldLogWM = true;
            }
        }

        /// <summary>
        /// Constructor for wrapping an existing HwndSource with a SurfaceCore object.
        /// </summary>
        public SurfaceCore(object objectSurface)
            : base(objectSurface)
        {
            if (objectSurface is HwndSource)
            {
                _hwndSource = (HwndSource)objectSurface;
                surfaceObject = _hwndSource;

                isVisible = true;

                if (!s_hwndSubclasses.ContainsKey(this.Handle))
                {
                    HookWindowMessages(this.Handle, "Surface");
                }
            }
        }

        /// <summary>
        /// </summary>
        public SurfaceCore(string typeOfSurface,
            int left,
            int top,
            int width,
            int height)
            : base(typeOfSurface, left, top, width, height)
        {
            Initialize(typeOfSurface, left, top, width, height, true);
        }


        /// <summary>
        /// Constructor that will create the surface that is requested.
        /// </summary>
        public SurfaceCore(string typeOfSurface,
            int left,
            int top,
            int width,
            int height,
            bool visibleSurface)
            : base(typeOfSurface, left, top, width, height, visibleSurface)
        {
            Initialize(typeOfSurface, left, top, width, height, visibleSurface);
        }

        void Initialize(string typeOfSurface,
            int left,
            int top,
            int width,
            int height,
            bool visibleSurface)
        {
            if (("HwndSource" == typeOfSurface) || ("LayeredHwndSource" == typeOfSurface))
            {
                HwndSourceParameters parameters = new HwndSourceParameters(typeOfSurface, width, height);

                if (visibleSurface)
                {
                    parameters.WindowStyle |= NativeConstants.WS_VISIBLE;
                    isVisible = true; 
                }

                parameters.WindowStyle |= NativeConstants.WS_OVERLAPPEDWINDOW;
                

                if ("LayeredHwndSource" == typeOfSurface)
                {
                    parameters.UsesPerPixelOpacity = true;
                }

                System.Security.PermissionSet set = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
                set.AddPermission(new System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode));
                set.AddPermission(new System.Security.Permissions.UIPermission(System.Security.Permissions.UIPermissionWindow.AllWindows));
                set.Assert();

                _hwndSource = new HwndSource(parameters);
                surfaceObject = _hwndSource;
                SetSize(width, height);
                SetPosition(left, top);

                HookWindowMessages(this.Handle, "Surface");
            }
        }

        /// <summary>
        /// Abstracts the Close          
        /// </summary>
        public override void Close()
        {
            base.Close();

            if (_hwndSource != null)
            {
                (new System.Security.Permissions.UIPermission(PermissionState.Unrestricted)).Assert();
                _hwndSource.Dispose();
            }
        }

        /// <summary>
        /// Hooks window messages.
        /// </summary>
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
        protected void HookWindowMessages(IntPtr handle, string friendlyName)
        {
            Log("Hooking window messages...");

            if (s_hwndSubclasses.ContainsKey(handle))
            {
                throw new InvalidOperationException("The handle '" + handle.ToString() + "' has already been hooked.");
            }

            Log(friendlyName + " hwnd = " + handle.ToString());

            HwndSubclass hwndSubclass = new HwndSubclass(s_hwndHook);
            hwndSubclass.Attach(handle);

            s_hwndSubclasses.Add(handle, hwndSubclass);
        }

        /// <summary>
        /// Unhooks window messages.
        /// </summary>
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
        private static void _UnhookWindowMessages(IntPtr handle)
        {
            Log("Unhooking window messages...");

            if (!s_hwndSubclasses.ContainsKey(handle))
            {
                System.Diagnostics.Debug.WriteLine("The handle '" + handle.ToString() + "' has not been hooked.");
                return;
            }

            HwndSubclass hwndSubclass = s_hwndSubclasses[handle];

            hwndSubclass.Detach(true);
            s_hwndSubclasses.Remove(handle);
        }

        // WNDPROC hook callback that logs messages that pass through wndprocs.
        private static IntPtr _HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Log message only if the environment variable was set.
            if (s_shouldLogWM)
            {
                string msgString = NativeConstants.ConvertToString(msg, NativeConstantType.WM);

                CoreLogger.LogStatus("WM message '" + msgString + "' on hwnd '" + hwnd.ToString() + "'");
            }

            // Unhook messages when the window is destroyed.
            if (msg == NativeConstants.WM_DESTROY)
            {
                _UnhookWindowMessages(hwnd);
            }

            // Always set handled=false and return the hwnd.
            handled = false;

            return hwnd;
        }

        /// <summary>
        /// Force the Surface to be active.
        /// </summary>
        public override void ForceActivation()
        {
            ForceActivationCore(this.Handle);
        }

        /// <summary>
        /// Force a given handle to be activated.
        /// </summary>
        /// <param name="handle">Window handle.</param>
        protected virtual void ForceActivationCore(IntPtr handle)
        {
            Log("Forcing activation...");

            //
            // Send it to bottom of z-order and then to the top of z-order.
            //
            NativeMethods.SetWindowPos(
                handle,
                NativeConstants.HWND_BOTTOM,
                0, 0, 0, 0,
                NativeConstants.SWP_NOMOVE | NativeConstants.SWP_NOSIZE);


            NativeMethods.SetWindowPos(
                handle,
                NativeConstants.HWND_TOP,
                0, 0, 0, 0,
                NativeConstants.SWP_NOMOVE | NativeConstants.SWP_NOSIZE);

            //
            // Put it in foreground focus.
            //
            HandleRef handleRef = new HandleRef(null, handle);
            NativeMethods.SetForegroundWindow(handleRef);
            DispatcherHelper.DoEvents(100);
        }

        /// <summary>
        /// </summary>
        public override Point DeviceUnitsFromMeasureUnits(Point point)
        {
            if (_hwndSource != null)
            {
                return _hwndSource.CompositionTarget.TransformToDevice.Transform(point);
            }

            return new Point(double.NaN, double.NaN);
        }

        /// <summary>
        /// </summary>
        public override void DisplayObject(object visual)
        {
            base.DisplayObject(visual);

            if (visual == null || visual is Visual)
            {
                if (_hwndSource != null)
                {
                    _hwndSource.RootVisual = (Visual)visual;

                }
            }

        }

        /// <summary>
        /// Return the PresentationSource for the Surface.
        /// </summary>
        public override PresentationSource GetPresentationSource()
        {
            if (_hwndSource != null)
            {
                return (PresentationSource)_hwndSource;
            }

            return null;
        }

        /// <summary>
        /// </summary>
        public override Point MeasureUnitsFromDeviceUnits(Point devicePoint)
        {
            if (_hwndSource != null)
            {
                return _hwndSource.CompositionTarget.TransformFromDevice.Transform(devicePoint);
            }

            return new Point(double.NaN, double.NaN);
        }



        /// <summary>
        /// This method will present the specified object on the surface
        /// </summary>
        public override void SetPosition(int left, int top)
        {
            base.SetPosition(left, top);


            if (_hwndSource != null)
            {
                CoreLogger.LogStatus(" SurfaceCore SetSize hwndsource != null");
                NativeMethods.SetWindowPos(GetHandle(), IntPtr.Zero, Left, Top, 0, 0, NativeConstants.SWP_NOSIZE);
            }
        }

        /// <summary>
        /// Set the size of the surface in measureunits
        /// </summary>
        public override bool SetSize(int width, int height)
        {
            CoreLogger.LogStatus(" SurfaceCore SetSize " + width + "," + height);

            bool x = base.SetSize(width, height);

            if (x)
            {
                if (_hwndSource != null)
                {
                    CoreLogger.LogStatus(" SurfaceCore SetSize hwndsource != null");
                    ResizeWindow(WidthPixels, HeightPixels);
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Set the size of the surface in pixels
        /// </summary>
        public override bool SetSizePixels(int width, int height)
        {
            bool x = base.SetSizePixels(width, height);

            if (x)
            {
                if (_hwndSource != null)
                {
                    ResizeWindow(WidthPixels, HeightPixels);
                }
                return true;
            }

            return false;
        }


        /// <summary>
        /// Displays the surface. In this case the HwndSource if a HwndSource surface was created
        /// </summary>
        public override void Show()
        {
            base.Show();

            if (_hwndSource != null)
            {
                NativeMethods.ShowWindow(new HandleRef(null, GetHandle()), NativeConstants.SW_SHOW);
            }
        }

        /// <summary>
        /// Retrieves the HWND for the surface.
        /// </summary>        
        protected override IntPtr GetHandle()
        {
            IntPtr handle = IntPtr.Zero;

            if (_hwndSource != null)
            {
                System.Security.PermissionSet set = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
                set.AddPermission(new System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode));
                set.AddPermission(new System.Security.Permissions.UIPermission(System.Security.Permissions.UIPermissionWindow.AllWindows));
                set.Assert();

                handle = _hwndSource.Handle;
            }

            return handle;
        }

        private void ResizeWindow(int width, int height)
        {
            CoreLogger.LogStatus(" SurfaceCore: ResizeWindow " + width + "," + height);

            NativeMethods.SetWindowPos(GetHandle(), IntPtr.Zero, 0, 0, width, height, NativeConstants.SWP_NOMOVE);
        }

        private HwndSource _hwndSource = null;

        // Used for hooking window messages.
        private static bool s_shouldLogWM = false;
        private static HwndWrapperHook s_hwndHook = new HwndWrapperHook(_HwndHook);
        private static Dictionary<IntPtr, HwndSubclass> s_hwndSubclasses = new Dictionary<IntPtr, HwndSubclass>();
    }


}


