// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************************
*    Purpose:  Win32-specific implementation for mouse simulation.
*    
 
  
*    Revision:         $Revision: 2 $
 
******************************************************************************/
using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Security;
using System.Security.Permissions;
using Microsoft.Test.Win32;
using Microsoft.Test.Threading;

namespace Avalon.Test.CoreUI.Trusted
{
    /// <summary>
    /// Win32-specific implementation for primitive mouse simulation. 
    /// </summary>
    internal class Win32MouseSimulation : MouseSimulation
    {
        /// <summary>
        /// Moves the mouse on the specified location on the specified HWND.
        /// </summary>
        /// <param name="hwnd">HWND where the mouse will be set</param>
        /// <param name="location">Relative postion where the mouse final position will be.</param>
        public void MoveOutside(IntPtr hwnd, MouseLocation location) 
        { 
            MoveOutside(hwnd, location, false);
        }

        /// <summary>
        /// Moves the mouse on the specified location on the specified HWND indicating if the
        /// non-client area should be counted.
        /// </summary>
        /// <param name="hwnd">HWND where the mouse will be set</param>
        /// <param name="location">Relative postion where the mouse final position will be.</param>
        /// <param name="includeNonClient">True the movement will be relative to the 
        /// non-client area. False the movement will be relative to the client area.</param>
        public void MoveOutside(IntPtr hwnd, MouseLocation location, bool includeNonClient) 
        {
            Point pt = GetPoint(hwnd, location, includeNonClient, false);
            ((MouseSimulation)this).Move((int)pt.X,(int)pt.Y);
        }

        /// <summary>
        /// Moves the mouse on the specified location on the specified HWND.
        /// </summary>
        /// <param name="hwnd">HWND where the mouse will be set</param>
        /// <param name="location">Relative postion where the mouse final position will be.</param>
        public void Move(IntPtr hwnd, MouseLocation location) 
        { 
            Move(hwnd, location, false);
        }

        /// <summary>
        /// Moves the mouse on the specified location on the specified HWND indicating if the
        /// non-client area should be counted.
        /// </summary>
        /// <param name="hwnd">HWND where the mouse will be set</param>
        /// <param name="location">Relative postion where the mouse final position will be.</param>
        /// <param name="includeNonClient">True the movement will be relative to the 
        /// non-client area. False the movement will be relative to the client area.</param>
        public void Move(IntPtr hwnd, MouseLocation location, bool includeNonClient) 
        {
            Point pt = GetPoint(hwnd, location, includeNonClient);
            ((MouseSimulation)this).Move((int)pt.X,(int)pt.Y);
        }


        /// <summary>
        /// Returns the position for the specied HWND on the specified relative location.
        /// It is possible to specified client area vs window area.
        /// </summary>
        /// <param name="hwnd">HWND where the mouse will be set</param>
        /// <param name="location">Relative postion where the mouse final position will be.</param>
        /// <param name="includeNonClient">True the movement will be relative to the 
        /// non-client area. False the movement will be relative to the client area.</param>
        /// <returns>Position</returns>
        public Point GetPoint(IntPtr hwnd, MouseLocation location, bool includeNonClient)
        {
            return GetPoint(hwnd, location, includeNonClient, true);
        }


        /// <summary>
        /// Returns the position for the specied HWND on the specified relative location.
        /// It is possible to specified client area vs window area.
        /// </summary>
        /// <param name="hwnd">HWND where the mouse will be set</param>
        /// <param name="location">Relative postion where the mouse final position will be.</param>
        /// <param name="includeNonClient">True the movement will be relative to the 
        /// non-client area. False the movement will be relative to the client area.</param>
        /// <param name="interior"></param>
        /// <returns>Position</returns>
        public Point GetPoint(IntPtr hwnd, MouseLocation location, bool includeNonClient, bool interior)
        {
            NativeStructs.RECT windowRect = new NativeStructs.RECT();
            NativeMethods.GetWindowRect(new HandleRef(null,hwnd), ref windowRect);  

            NativeStructs.RECT clientRect = new NativeStructs.RECT();
            NativeMethods.GetClientRect(new HandleRef(null,hwnd) , ref clientRect);

            int windowWidth = windowRect.right - windowRect.left;
            int windowHeight = windowRect.bottom - windowRect.top;

            int clientWidth = clientRect.right - clientRect.left;
            int clientHeight = clientRect.bottom - clientRect.top;


            int x = 0;
            int y = 0;            
                      
            switch(location)
            {
                case MouseLocation.TopLeft:

                    if (includeNonClient)
                    {
                        x = windowRect.left + s_offSet;
                        y = windowRect.top + s_offSet;
                    }
                    else
                    {
                        
                        NativeStructs.POINT pt = new NativeStructs.POINT(0 + s_offSet, 0 + s_offSet);
                        NativeMethods.ClientToScreen(hwnd, ref pt);

                        x = pt.x;
                        y = pt.y;

                    }
                    
                    if (!interior)
                    {
                        x -= s_outsideOffSet;
                        y -= s_outsideOffSet;
                    }
                    
                    break;
                case MouseLocation.TopRight:

                    if (includeNonClient)
                    {
                       x = windowRect.right - s_offSet;
                       y = windowRect.top + s_offSet;
                    }
                    else
                    {                                                                   
                        NativeStructs.POINT pt = new NativeStructs.POINT(clientRect.right - s_offSet, 0 + s_offSet);
                        NativeMethods.ClientToScreen(hwnd, ref pt);

                        x = pt.x;
                        y = pt.y;

                    }

                    if (!interior)
                    {
                        x += s_outsideOffSet;
                        y -= s_outsideOffSet;
                    }
                    
                    break;

                case MouseLocation.Top:

                    if (includeNonClient)
                    {
                        x = windowRect.right - (windowWidth / 2);
                        y = windowRect.top + s_offSet;
                    }
                    else
                    {
                        NativeStructs.POINT pt = new NativeStructs.POINT(clientRect.right - (clientWidth / 2) , 0 + s_offSet);
                        NativeMethods.ClientToScreen(hwnd, ref pt);

                        x = pt.x;
                        y = pt.y;

                    }

                    if (!interior)
                    {
                        y -= s_outsideOffSet;
                    }
                    
                    break;

                case MouseLocation.CenterLeft:

                    if (includeNonClient)
                    {
                        x = windowRect.left + s_offSet;
                        y = windowRect.bottom - (windowHeight / 2);
                    }
                    else
                    {
                        NativeStructs.POINT pt = new NativeStructs.POINT(0 + s_offSet, clientRect.bottom - (clientHeight / 2));
                        NativeMethods.ClientToScreen(hwnd, ref pt);

                        x = pt.x;
                        y = pt.y;

                    }

                    if (!interior)
                    {
                        x -= s_outsideOffSet;
                    }
                    
                    break;
                case MouseLocation.CenterRight:

                    if (includeNonClient)
                    {
                        x = windowRect.right - s_offSet;
                        y = windowRect.bottom - (windowHeight / 2);
                    }
                    else
                    {
                        NativeStructs.POINT pt = new NativeStructs.POINT(clientRect.right - s_offSet, clientRect.bottom - (clientHeight / 2));
                        NativeMethods.ClientToScreen(hwnd, ref pt);

                        x = pt.x;
                        y = pt.y;

                    }

                    if (!interior)
                    {
                        x += s_outsideOffSet;
                    }
                    
                    break;

                case MouseLocation.Center:

                    if (includeNonClient)
                    {
                        x = windowRect.right - (windowWidth / 2);
                        y = windowRect.bottom - (windowHeight / 2);
                    }
                    else
                    {
                        NativeStructs.POINT pt = new NativeStructs.POINT(clientRect.right - (clientWidth / 2), clientRect.bottom - (clientHeight / 2));
                        NativeMethods.ClientToScreen(hwnd, ref pt);

                        x = pt.x;
                        y = pt.y;

                    }
                    break;


                case MouseLocation.BottomLeft:

                    if (includeNonClient)
                    {
                        x = windowRect.left + s_offSet;
                        y = windowRect.bottom - s_offSet;
                    }
                    else
                    {
                        NativeStructs.POINT pt = new NativeStructs.POINT(0 + s_offSet, clientRect.bottom - s_offSet);
                        NativeMethods.ClientToScreen(hwnd, ref pt);

                        x = pt.x;
                        y = pt.y;

                    }

                    if (!interior)
                    {
                        x -= s_outsideOffSet;
                        y += s_outsideOffSet;
                    }
                    
                    break;
                case MouseLocation.BottomRight:

                    if (includeNonClient)
                    {
                        x = windowRect.right - s_offSet;
                        y = windowRect.bottom - s_offSet;
                    }
                    else
                    {
                        NativeStructs.POINT pt = new NativeStructs.POINT(clientRect.right - s_offSet, clientRect.bottom - s_offSet);
                        NativeMethods.ClientToScreen(hwnd, ref pt);

                        x = pt.x;
                        y = pt.y;

                    }

                    if (!interior)
                    {
                        x += s_outsideOffSet;
                        y += s_outsideOffSet;
                    }
                    
                    break;

                case MouseLocation.Bottom:

                    if (includeNonClient)
                    {
                        x = windowRect.right - (windowWidth / 2);
                        y = windowRect.bottom - s_offSet;
                    }
                    else
                    {
                        NativeStructs.POINT pt = new NativeStructs.POINT(clientRect.right - (clientWidth / 2), clientRect.bottom - s_offSet);
                        NativeMethods.ClientToScreen(hwnd, ref pt);

                        x = pt.x;
                        y = pt.y;

                    }
                    if (!interior)
                    {
                        y += s_outsideOffSet;
                    }
                    
                    break;
            }
            
            return new Point(x,y);
            
        }

        /// <summary>
        /// Offset that it is used.
        /// </summary>
        static int s_offSet = 2;

        /// <summary>
        /// Offset that it is used.
        /// </summary>
        static int s_outsideOffSet = 5;

    }
}
