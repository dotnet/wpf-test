// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************************
*    Purpose:  General-use static class that wraps specific mouse
*              simulation implementations. In addition to high-level
*              helper routines, it provides synchronization with Avalon events.
*    
 
  
*    Revision:         $Revision: 3 $
 
*    Filename:         $Source: //depot/devdiv/Orcas/feature/element3d/wpf/Test/ElementServices/FeatureTests/Trusted/MouseHelper.cs $
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
using System.Windows.Media;
using System.Windows.Threading;

using Microsoft.Test.Input;
using Microsoft.Test.Threading;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.Trusted
{
    /// <summary>
    /// General-use static class that wraps specific mouse
    /// simulation implementations. In addition to high-level
    /// helper routines, it provides synchronization with Avalon events.
    /// </summary>
    public static partial class MouseHelper
    {
        /// <summary>
        /// Drags the mouse to the specified location of the specifed element3d.
        /// </summary>
        /// <param name="element3d">UIElement3D that it will be the final position for the mouse.</param>
        /// <param name="location">Location on the UIElement3D where the mouse will be set.</param>
        /// <param name="button"></param>  
        /// <param name="interior">Location on the UIElement3D where the mouse will be set.</param>  
        /// <remarks>
        /// Drag is defined as these steps:
        /// 1. Push a mouse button down.
        /// 2. Move the mouse to a new location.
        /// 3. Release the mouse button.
        /// 
        /// Since the OLE drag/drop component pumps messages, drag simulation must always be done synchronously.
        /// The button press &amp; release and mouse movements are queued before any of them run.  Then, we 
        /// start processing the queue (by pumping messages).  When the drag start, OLE starts pumping messages, 
        /// and that will process more of the queue. The drag ends when the button release is processed. 
        /// </remarks>
        private static void DragImpl(UIElement3D element3d, MouseLocation location, MouseButton button, bool interior)
        {
            // Bring the HWND to the front of the screen.
            PresentationSource source = PresentationSource.FromDependencyObject(element3d);
            HandleRef hwnd = PresentationHelper.GetHwnd(source);

            DragImpl((IntPtr)hwnd, location, button, interior);
        }



        /// <summary>
        /// Move the mouse to the center of the specified UIElement3D.
        /// </summary>
        /// <param name="element3d">UIElement3D that will be the final position for the mouse.</param>
        public static void Move(UIElement3D element3d)
        {
            Move(element3d, MouseLocation.Center, false);
        }

        /// <summary>
        /// Move the mouse to the specified location of the specified UIElement3D.
        /// </summary>
        /// <param name="element3d">UIElement3D that will be the final position for the mouse.</param>
        /// <param name="location">Specified location for the mouse on the UIElement3D.</param>
        public static void Move(UIElement3D element3d, MouseLocation location)
        {
            MoveImpl(element3d, location, true, false);
        }

        /// <summary>
        /// Move the mouse to the specified location of the specified UIElement3D.
        /// </summary>
        /// <param name="element3d">UIElement3D that will be the final position for the mouse.</param>
        /// <param name="location">Specified location for the mouse on the UIElement3D.</param>
        /// <param name="isImmediate">Whether or not to interpolate points between the start and end points.</param>        
        public static void Move(UIElement3D element3d, MouseLocation location, bool isImmediate)
        {
            MoveImpl(element3d, location, true, isImmediate);
        }


        /// <summary>
        /// Mouse down at the current position, move the mouse to the specified location 
        /// of the specified UIElement3D, and release the mouse.
        /// </summary>
        /// <param name="element3d">UIElement3D that it will be the final position for the mouse.</param>
        /// <param name="location">Specified location for the mouse on the UIElement3D.</param>
        /// <param name="button">Mouse button that will be use for the drag.</param>
        /// <remarks>
        /// Since the OLE drag/drop component pumps messages, drag simulation must always be done synchronously.
        /// The button press &amp; release and mouse movements are queued before any of them run.  Then, we 
        /// start processing the queue (by pumping messages).  When the drag start, OLE starts pumping messages, 
        /// and that will process more of the queue. The drag ends when the button release is processed.
        /// </remarks>
        public static void Drag(UIElement3D element3d, MouseLocation location, MouseButton button)
        {
            DragImpl(element3d, location, button, true);
        }
     
        /// <summary>
        /// Move the mouse to the specified location of the specified UIElement3D.
        /// </summary>
        /// <param name="element3d">UIElement3D that it will be the final position for the mouse.</param>
        /// <param name="location">Specified location for the mouse on the UIElement3D.</param>
        /// <param name="interior">Specified location for the mouse on the UIElement3D.</param>
        /// <param name="isImmediate">Whether or not to interpolate points between the start and end points.</param>           
        private static void MoveImpl(UIElement3D element3d, MouseLocation location, bool interior, bool isImmediate)
        {
            // Bring the HWND to the front of the screen.
            PresentationSource source = PresentationSource.FromDependencyObject(element3d);
            HandleRef hwnd = PresentationHelper.GetHwnd(source);
            SetHwndForeground((IntPtr)hwnd);

            // Getting the final point where the mouse will be set.
            Point finalPoint = s_avalonMouse.GetPoint(element3d, location, interior);

            // Getting the current point where the mouse is located.
            Point currentPosition = MouseSimulation.GetCurrentMousePosition();

            MoveImpl(currentPosition, finalPoint, isImmediate);
        }

        /// <summary>
        /// Generates a left button click (or mouse primary button click) effect 
        /// on the center position of the specified UIElement3D. (a.k.a. Win32 LButtonDown &amp; LButtonUp)
        /// </summary>
        /// <param name="element3d"></param>
        public static void Click(UIElement3D element3d)
        {
            Click(MouseButton.Left, element3d, MouseLocation.Center);
        }


        /// <summary>
        /// Generates a left button click (or mouse primary button click) effect 
        /// on the center position of the specified UIElement3D. (a.k.a. Win32 LButtonDown &amp; LButtonUp)
        /// </summary>
        /// <param name="element3d"></param>
        /// <param name="amountOfClicks">Number of Clicks.</param>
        public static void Click(UIElement3D element3d, int amountOfClicks)
        {
            Move(element3d);
            Click(MouseButton.Left, amountOfClicks);
        }

        /// <summary>
        /// Generates a left button click (or mouse primary button click) effect 
        /// on the center position of the specified UIElement3D. (a.k.a. Win32 LButtonDown &amp; LButtonUp)
        /// </summary>
        /// <param name="button"></param>
        /// <param name="element3d"></param>
        /// <param name="amountOfClicks">Number of Clicks.</param>
        public static void Click(MouseButton button, UIElement3D element3d, int amountOfClicks)
        {
            Move(element3d);
            Click(button, amountOfClicks);
        }

        /// <summary>
        /// Generates a click effect using the specified button
        /// on the center position of the specified UIElement3D. (a.k.a. Win32 ?ButtonDown &amp; ?ButtonUp)
        /// </summary>
        /// <param name="button">Mouse button that will be use for the click.</param>
        /// <param name="element3d">UIElement3D used to move the mouse to its center position.</param>
        public static void Click(MouseButton button, UIElement3D element3d)
        {
            Click(button, element3d, MouseLocation.Center);
        }

        /// <summary>
        /// Generates a click effect using the specified button
        /// on the specified position of the specified UIElement3D. (a.k.a. Win32 ?ButtonDown &amp; ?ButtonUp)
        /// </summary>
        /// <param name="button">Mouse button that will be use for the click.</param>
        /// <param name="element3d">UIElement3D used to move the mouse to its center position.</param>
        /// <param name="location">Location relative to the UIElement3D.</param>
        public static void Click(MouseButton button, UIElement3D element3d, MouseLocation location)
        {
            Move(element3d, location);
            Click(button);
        }
    }
}
