// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************************
*    Source Control Information
*    
 
  
*    Revision:         $Revision: 2 $
 
******************************************************************************/
using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Collections.Generic;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;
using System.Security;
using Microsoft.Test.Win32;
using System.Security.Permissions;

namespace Avalon.Test.CoreUI.Trusted
{
    /// <summary>
    /// Abstraction about the Surface that will be used.
    /// </summary>
    public abstract class Surface
    {

        private static Point GetMonitorTopLeftPosition()
        {
            if (MultiMonitorHelper.IsMultiMonAvailable())
            {
                NativeStructs.MONITORINFOEX primaryMonitor;
                
                NativeStructs.MONITORINFOEX[] monitorArray = MultiMonitorHelper.GetAllMonitors(out primaryMonitor);

                int monitorIndex = 0;

                if (monitorArray[monitorIndex] == primaryMonitor)
                {
                    monitorIndex++;
                }

                NativeStructs.MONITORINFOEX monitor = monitorArray[monitorIndex];

                return new Point(monitor.rcMonitor.left, monitor.rcMonitor.top);
                
            }
            return new Point(0,0);

        }

        
        /// <summary>
        /// Classes derivaring from surface should register the type of surface
        /// that they will be served.
        /// </summary>
        public static void RegisterSurface(string surfaceName, Type surfaceType)
        {
            lock(s_globalsyncRoot)
            {
                s_registeredSurfaces.Add(surfaceName, surfaceType);
            }
        }

        /// <summary>
        /// Query if the Surface support some specified surface.
        /// </summary>
        public static bool IsSurfaceRegistered(string surfaceName)
        {
            bool v = false;

            lock(s_globalsyncRoot)
            {
               v = s_registeredSurfaces.ContainsKey(surfaceName);
            }            
            
            return v;
        }

        /// <summary>
        /// Query if the Surface support some specified surface.        
        /// </summary>
        public static bool IsSurfaceRegistered(Type surfaceType)
        {
            Log("The surface type looking for is: " + surfaceType.ToString());
            bool v = false;

            lock(s_globalsyncRoot)
            {
               v = s_registeredSurfaces.ContainsValue(surfaceType);
            }            
            
            Log("Is the surface type found? " + v.ToString());
            return v;
        }

        /// <summary>
        /// Contructor for wrapping an existing surface with a Surface object.
        /// </summary>
        public Surface(object objectSurface)
        {
            if (null == objectSurface)
            {
                throw new ArgumentNullException("objectSurface");
            }

            if (!IsSurfaceRegistered(objectSurface.GetType()))
            {
                throw new ArgumentException("The surface type is not registed");
            }

            surfaceObject = objectSurface;
        }

        /// <summary>
        /// Constructor that will create the surface that is requested.
        /// </summary>
        public Surface(string typeOfSurface,
            int left, 
            int top, 
            int width, 
            int height)
        {
            Initialize(typeOfSurface, left,top,width,height,true);
        }


        /// <summary>
        /// Constructor that will create the surface that is requested.
        /// </summary>
        public Surface(string typeOfSurface,
            int left, 
            int top, 
            int width, 
            int height, 
            bool visibleSurface)
        {
            Initialize(typeOfSurface, left,top,width,height,visibleSurface);
        }

        /// <summary>
        /// Abstracts the Close          
        /// </summary>
        public virtual void Close()
        {
            _isClosed = true;
        }   

        /// <summary>
        /// </summary>
        public abstract Point DeviceUnitsFromMeasureUnits(Point point);

        /// <summary>
        /// Abstracts how the object is presented on the abstracted surface.
        /// </summary>
        public virtual void DisplayObject(object visual)
        {
            rootDisplayedObject = visual;
        }

        /// <summary>
        /// Abstracts how the object goes back in a stack of object trees.
        /// </summary>
        public virtual void GoBack()
        {
        }
        
        /// <summary>
        /// Abstracts how the object goes forward in a stack of object trees.
        /// </summary>
        public virtual void GoForward()
        {
        }

        /// <summary>
        /// Force the Surface to be active.
        /// </summary>
        public virtual void ForceActivation()
        {
        }

        /// <summary>
        /// Return the PresentationSource for the Surface
        /// </summary>
        public virtual PresentationSource GetPresentationSource()
        {
            return null;
        }

        /// <summary>
        /// </summary>
        public abstract Point MeasureUnitsFromDeviceUnits(Point devicePoint);

        /// <summary>
        /// Abstracts the position         
        /// </summary>
        public virtual void SetPosition(int left, int top)
        {
            Point position = GetMonitorTopLeftPosition();

            _left = (int)position.X + left;
            _top = (int)position.Y + top;
        }  

        /// <summary>
        /// Set the size of the surface in measureunits
        /// </summary>
        public virtual bool SetSize(int width, int height)
        {
            CoreLogger.LogStatus("Surface: SetSize(" + width + "," + height + ")");
            Point point =  DeviceUnitsFromMeasureUnits(new Point(width, height));

            CoreLogger.LogStatus(" DeviceUnitsFromMeasureUnits " + point.X + "," + point.Y);
            if (!double.IsNaN(point.X) && !double.IsNaN(point.Y))
            {
                _width = width;
                _height = height;

                _widthPixels = (int)Math.Floor(point.X);
                _heightPixels = (int)Math.Floor(point.Y);

                return true;
            }
            
            return false;
        }   

        /// <summary>
        /// Set the size of the surface in pixels
        /// </summary>
        public virtual bool SetSizePixels(int width, int height)
        {
            Point point =  MeasureUnitsFromDeviceUnits(new Point(width, height));

            if (!double.IsNaN(point.X) && !double.IsNaN(point.Y))
            {
                _widthPixels = width;
                _heightPixels = height;

                _width = (int)Math.Floor(point.X);
                _height = (int)Math.Floor(point.Y);
                return true;
            }
            
            return false;
        }  
        
        /// <summary>
        /// Displays the surface
        /// </summary>
        public virtual void Show()
        {     
            isVisible = true;
        }

        /// <summary>
        /// Displays the surface in as Modal
        /// </summary>
        public virtual void ShowModal()
        {     
            isVisible = true;
            isModal = true;
        }
           
        /// <summary>
        /// Returns the HWND for this surface.
        /// </summary>
        public IntPtr Handle
        {
            get
            {
                return GetHandle();
            }
        }

        /// <summary>
        /// </summary>
        public int Height
        {
            get
            {
                return _height;
            }
        }

        /// <summary>
        /// </summary>
        public int HeightPixels
        {
            get
            {
                return _heightPixels;
            }
        }                

        /// <summary>
        /// 
        /// </summary>
        public bool IsClosed
        {
            get
            {
                return _isClosed;
            }
        }

        /// <summary>
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return isVisible;
            }
        }

        /// <summary>
        /// </summary>
        public int Left
        {
            get
            {
                return _left;
            }
        }

        /// <summary>
        /// Object that is the root of the tree stored on the surface. 
        /// </summary>
        /// <remarks>
        /// Objects passed to DisplayMe() are typically returned by this property.
        /// </remarks>
        public object RootDisplayedObject
        {
            get
            {
                return rootDisplayedObject;
            }
        }

        /// <summary>
        /// Object that is the root of the surface that contains a tree.
        /// </summary>
        /// <remarks>
        /// Objects of type NavigationWindow, Window, and HwndSource are typically returned by this property.
        /// </remarks>
        public object SurfaceObject
        {
            get
            {
                return surfaceObject;
            }
        }

        /// <summary>
        /// </summary>
        public int Top
        {
            get
            {
                return _top;
            }
        }
                
        /// <summary>
        /// </summary>
        public int Width
        {
            get
            {
                return _width;
            }
        }
               
        /// <summary>
        /// </summary>
        public int WidthPixels
        {
            get
            {
                return _widthPixels;
            }
        }
       
        /// <summary>
        /// </summary>
        public static void Log(string s)
        {
            if (TestLog.Current != null)
            {
                TestLog.Current.LogStatus(s);
            }
            else
            {
                //Fallback to Console.
                Console.WriteLine(s);
            }
        }

        void Initialize(string typeOfSurface,
            int left, 
            int top, 
            int width, 
            int height, 
            bool visibleSurface)
        {
            if (!IsSurfaceRegistered(typeOfSurface))
            {
                throw new ArgumentException("The surface type is not registed");
            }

            _left = left;
            _top = top;
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Retrieves the HWND for the surface.
        /// </summary>
        protected abstract IntPtr GetHandle();

        /// <summary>
        /// </summary>
        protected object rootDisplayedObject = null;

        /// <summary>
        /// </summary>
        protected object surfaceObject = null;

        /// <summary>
        /// </summary>
        protected bool isVisible = false;

        /// <summary>
        /// </summary>
        protected bool isModal = false;

        /// <summary>
        /// </summary>
        private bool _isClosed = false;

        /// <summary>
        /// </summary>
        private int _left = 0;

        /// <summary>
        /// </summary>
        private int _top = 0;

        /// <summary>
        /// </summary>
        private int _width = 0;

        /// <summary>
        /// </summary>
        private int _height = 0;

        /// <summary>
        /// </summary>
        private int _widthPixels = 0;

        /// <summary>
        /// </summary>
        private int _heightPixels = 0;
        
        private static Dictionary<string, Type> s_registeredSurfaces = new Dictionary<string, Type>();
        private static object s_globalsyncRoot = new Object();
        
    }
    
}

    
