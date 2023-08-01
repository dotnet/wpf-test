// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************************
*    Purpose:  Avalon-specific implementation for primitive mouse simulation. 
*    
 
  
*    Revision:         $Revision: 2 $
 
******************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

using Microsoft.Test.Win32;
using Microsoft.Test.Threading;

namespace Avalon.Test.CoreUI.Trusted
{
    /// <summary>
    /// Avalon-specific implementation for primitive mouse simulation.
    /// </summary>
    internal partial class AvalonMouseSimulation : MouseSimulation
    {
        /// <summary>
        /// Moves the mouse on the specified location on the specified HWND.
        /// </summary>
        /// <param name="element">UIElement where the mouse will be set</param>
        /// <param name="location">Relative postion where the mouse final position will be.</param>
        public void MoveOutside(UIElement element, MouseLocation location)
        {
            Point pt = GetPoint(element, location, false);
            ((MouseSimulation)this).Move((int)pt.X, (int)pt.Y);
        }


        /// <summary>
        /// Moves the mouse on the specified location on the specified HWND.
        /// </summary>
        /// <param name="element">UIElement where the mouse will be set</param>
        /// <param name="location">Relative postion where the mouse final position will be.</param>
        public void Move(UIElement element, MouseLocation location)
        {
            Point pt = GetPoint(element, location);
            ((MouseSimulation)this).Move((int)pt.X, (int)pt.Y);
        }

        /// <summary>
        /// Returns the position for the specied UIElement on the specified relative location.       
        /// </summary>
        /// <param name="element">UIElement where the mouse will be set</param>
        /// <param name="location">Relative postion where the mouse final position will be.</param>
        /// <returns>Point</returns>
        public Point GetPoint(UIElement element, MouseLocation location)
        {
            return GetPoint(element,location,true);
        }

        /// <summary>
        /// Returns the position for the specied UIElement on the specified relative location.       
        /// </summary>
        /// <param name="element">UIElement where the mouse will be set</param>
        /// <param name="location">Relative postion where the mouse final position will be.</param>
        /// <param name="inside">This will return a position outside of the element. The distance is
        /// defined on the offset proprety. True will be inside.</param>        
        /// <returns>Point</returns>
        public Point GetPoint(UIElement element, MouseLocation location, bool inside)
        {
            Rect r = ElementUtils.GetScreenRelativeRect(element);

            return GetPointInRect(r, location, inside);
        }

        /// <summary>
        /// Returns the position for the specied UIElement on the specified relative location.       
        /// </summary>
        /// <param name="r">Rect where the mouse will be set</param>
        /// <param name="location">Relative postion where the mouse final position will be.</param>
        /// <param name="inside">This will return a position outside of the element. The distance is
        /// defined on the offset proprety. True will be inside.</param>        
        /// <returns>Point</returns>
        private Point GetPointInRect(Rect r, MouseLocation location, bool inside)
        {
            int x = 0;
            int y = 0;

            switch (location)
            {
                case MouseLocation.TopLeft:

                    x = (int)(r.TopLeft.X);
                    y = (int)(r.TopLeft.Y);

                    if (inside)
                    {
                        x += _offset;
                        y += _offset;
                    }
                    else
                    {
                        x -= _offset;
                        y -= _offset;
                    }

                    break;

                case MouseLocation.TopRight:

                    x = (int)(r.TopRight.X);
                    y = (int)(r.TopRight.Y);

                    if (inside)
                    {
                        x -= _offset;
                        y += _offset;
                    }
                    else
                    {
                        x += _offset;
                        y -= _offset;
                    }

                    break;

                case MouseLocation.Top:

                    x = (int)(r.Left + r.Width / 2);
                    y = (int)(r.Top);

                    if (inside)
                    {                      
                        y += _offset;
                    }
                    else
                    {
                        y -= _offset;
                    }

                    break;

                case MouseLocation.CenterLeft:

                    x = (int)(r.Left);
                    y = (int)(r.Top + r.Height / 2);

                    if (inside)
                    {
                        x += _offset;
                    }
                    else
                    {
                        x -= _offset;
                    }
                    
                    break;

                case MouseLocation.CenterRight:

                    x = (int)(r.Right);
                    y = (int)(r.Top + r.Height / 2);

                    if (inside)
                    {
                        x -= _offset;
                    }
                    else
                    {
                        x += _offset;
                    }
                    
                    break;

                case MouseLocation.Center:

                    x = (int)(r.Left + r.Width / 2);
                    y = (int)(r.Top + r.Height / 2);

                    break;

                case MouseLocation.BottomLeft:

                    x = (int)(r.BottomLeft.X);
                    y = (int)(r.BottomLeft.Y);

                    if (inside)
                    {
                        x += _offset;
                        y -= _offset;
                    }
                    else
                    {
                        x -= _offset;
                        y += _offset;
                    }

                    break;

                case MouseLocation.BottomRight:

                    x = (int)(r.BottomRight.X);
                    y = (int)(r.BottomRight.Y);

                    if (inside)
                    {
                        x -= _offset;
                        y -= _offset;
                    }
                    else
                    {
                        x += _offset;
                        y += _offset;
                    }


                    break;

                case MouseLocation.Bottom:

                    x = (int)(r.Left + r.Width / 2);
                    y = (int)(r.Bottom);

                    if (inside)
                    {
                        y -= _offset;
                    }
                    else
                    {
                        y += _offset;
                    }

                    break;
            }

            return new Point(x, y);
        }

        // Distance from edges of elements.
        int _offset = 3;  
    }
}
