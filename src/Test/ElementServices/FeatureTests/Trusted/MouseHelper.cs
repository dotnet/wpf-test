// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************************
*    Purpose:  General-use static class that wraps specific mouse
*              simulation implementations. In addition to high-level
*              helper routines, it provides synchronization with Avalon events.
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
        private static Win32MouseSimulation s_win32Mouse = new Win32MouseSimulation();
        private static AvalonMouseSimulation s_avalonMouse = new AvalonMouseSimulation();
        private delegate void _NoParameterOperationCallback();

        /// <summary>
        /// Returns the point of a location on an element.
        /// </summary>
        /// <param name="element">Element with position.</param>
        /// <param name="location">Specified location on the Element.</param>
        public static Point GetPoint(UIElement element, MouseLocation location)
        {
            return s_avalonMouse.GetPoint(element, location, true);
        }

        /// <summary>
        /// Moves the mouse to the Center of the specifed Win32 HWND.
        /// </summary>
        /// <param name="hwnd">HWND where the mouse will be centered.</param>
        public static void Move(IntPtr hwnd)
        {
            Move(hwnd, MouseLocation.Center, false);
        }

        /// <summary>
        /// Moves the mouse to the specified location of the specifed Win32 HWND.
        /// </summary>
        /// <param name="hwnd">HWND where the mouse will be moved.</param>
        /// <param name="location">Location on the HWND where the mouse will be set.</param>
        public static void MoveOutside(IntPtr hwnd, MouseLocation location)
        {
            MoveImpl(hwnd, location, false, false);
        }

        /// <summary>
        /// Moves the mouse to the specified location of the specifed Win32 HWND.
        /// </summary>
        /// <param name="hwnd">HWND where the mouse will be moved.</param>
        /// <param name="location">Location on the HWND where the mouse will be set.</param>
        /// <param name="isImmediate">Whether or not to interpolate points between the start and end points.</param>        
        public static void MoveOutside(IntPtr hwnd, MouseLocation location, bool isImmediate)
        {
            MoveImpl(hwnd, location, false, isImmediate);
        }

        /// <summary>
        /// Moves the mouse to the specified location of the specifed Win32 HWND.
        /// </summary>
        /// <param name="hwnd">HWND where the mouse will be moved.</param>
        /// <param name="location">Location on the HWND where the mouse will be set.</param>
        public static void Move(IntPtr hwnd, MouseLocation location)
        {
            MoveImpl(hwnd, location, true, false);
        }

        /// <summary>
        /// Moves the mouse to the specified location of the specifed Win32 HWND.
        /// </summary>
        /// <param name="hwnd">HWND where the mouse will be moved.</param>
        /// <param name="location">Location on the HWND where the mouse will be set.</param>
        /// <param name="isImmediate">Whether or not to interpolate points between the start and end points.</param>        
        public static void Move(IntPtr hwnd, MouseLocation location, bool isImmediate)
        {
            MoveImpl(hwnd, location, true, isImmediate);
        }


        /// <summary>
        /// Moves the mouse to the specified location of the specifed Win32 HWND.
        /// </summary>
        /// <param name="hwnd">HWND where the mouse will be moved.</param>
        /// <param name="location">Location on the HWND where the mouse will be set.</param>
        /// <param name="interior">Location on the HWND where the mouse will be set.</param>  
        /// <param name="isImmediate">Whether or not to interpolate points between the start and end points.</param>        
        private static void MoveImpl(IntPtr hwnd, MouseLocation location, bool interior, bool isImmediate)
        {
            if (IntPtr.Zero != hwnd)
            {
                SetHwndForeground(hwnd);
            }
            
            // Getting the final point where the mouse will be set.
            Point finalPoint = s_win32Mouse.GetPoint(hwnd, location, false, interior);

            // Getting the current point where the mouse is located.
            Point currentPosition = MouseSimulation.GetCurrentMousePosition();

            MoveImpl(currentPosition, finalPoint, isImmediate);
        }

        /// <summary>
        /// Moves the mouse from one point to another point.
        /// </summary>
        private static void MoveImpl(Point startPoint, Point endPoint, bool isImmediate)
        {
            // Gettting all the points
            Point[] points = GetPoints(startPoint, endPoint, isImmediate);

            // Move the mouse over all the points. This will generate a smooth mouse move.
            for (int i = 0; i < points.Length; i++)
            {
                ((MouseSimulation)s_win32Mouse).Move((int)points[i].X, (int)points[i].Y);
                TryWaitForWin32InputMessages();
            }

            TryWaitForInputEvents();
        }

            
        /// <summary>
        /// Drags the mouse to the specified location of the specifed element.
        /// </summary>
        /// <param name="element">UIElement that it will be the final position for the mouse.</param>
        /// <param name="location">Location on the UIElement where the mouse will be set.</param>
        /// <param name="button"></param>  
        /// <param name="interior">Location on the UIElement where the mouse will be set.</param>  
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
        private static void DragImpl(UIElement element, MouseLocation location, MouseButton button, bool interior)
        {
            // Bring the HWND to the front of the screen.
            HandleRef hwnd = PresentationHelper.GetHwnd(element);

            DragImpl((IntPtr)hwnd, location, button, interior);
        }

        /// <summary>
        /// Drags the mouse to the specified location of the specifed Hwnd.
        /// </summary>
        /// <param name="hwnd">Hwnd that it will be the final position for the mouse.</param>
        /// <param name="location">Location on the target where the mouse will be set.</param>
        /// <param name="button"></param>  
        /// <param name="interior">Location on the target where the mouse will be set.</param>  
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
        private static void DragImpl(IntPtr hwnd, MouseLocation location, MouseButton button, bool interior)
        {
            SetHwndForeground(hwnd);

            // Getting the final point where the mouse will be set.
            Point endPoint = s_win32Mouse.GetPoint(hwnd, location, false, interior);

            // Getting the current point where the mouse is located.
            Point startPoint = MouseSimulation.GetCurrentMousePosition();

            // Get all the points
            Point[] points = GetPoints(startPoint, endPoint, false);

            // Send mouse input from another thread so the dragdrop nested pump
            // doesn't block additional input. The other thread will wait for 
            // processing of each input, but it will timeout if it takes too long.
            Thread thread = new Thread(new ParameterizedThreadStart(_DragImplThread));

            thread.Start(new object[] { Dispatcher.CurrentDispatcher, points, button });

            // Wait until thread is dead.
            while (thread.IsAlive)
            {
                DispatcherHelper.DoEvents(DispatcherPriority.Input);
            }

            // Wait until the mouse messages are handled by Avalon.
            DispatcherHelper.DoEventsPastInput();
        }

        private static void _DragImplThread(object obj)
        {
            object[] objects = (object[])obj;

            Dispatcher dispatcher = (Dispatcher)objects[0];
            Point[] points = (Point[])objects[1];
            MouseButton button = (MouseButton)objects[2];

            //
            // Press the button.
            //
            dispatcher.Invoke(
                DispatcherPriority.Input,
                (_NoParameterOperationCallback)delegate()
                {
                    ((MouseSimulation)s_avalonMouse).Down(button);
                });

            //
            // Post the mouse moves over all the points.
            // Try to wait for each move in the queue, but timeout if 
            // it takes too long.  That's necessary because dragdrop 
            // will nest a pump that depends on another move. If we didn't 
            // timeout, the wait would never end once the dragdrop started.
            //
            TimeSpan timeout = TimeSpan.FromMilliseconds(50);
            _NoParameterOperationCallback emptyCallback =
                (_NoParameterOperationCallback)delegate()
                   {
                   };

            for (int i = 0; i < points.Length; i++)
            {
                Point pt = points[i];
                ((MouseSimulation)s_win32Mouse).Move((int)pt.X, (int)pt.Y);

                dispatcher.Invoke(DispatcherPriority.Input, timeout, emptyCallback);
            }

            //
            // Post the button release.
            //
            dispatcher.Invoke(
                DispatcherPriority.Input,
                (_NoParameterOperationCallback)delegate()
                {
                    ((MouseSimulation)s_avalonMouse).Up(button);
                });
        }


        /// <summary>
        /// Moves the mouse to the TopLeft of the primary monitor.
        /// </summary>
        public static void MoveOnVirtualScreenMonitor()
        {
            Point point = MultiMonitorHelper.GetVirtualScreenTopLeftPoint();            
            ((MouseSimulation)s_win32Mouse).Move((int)point.X, (int)point.Y);
        }



        /// <summary>
        /// Moves the mouse to the TopLeft of the primary monitor.
        /// </summary>
        public static void MoveOnPrimaryMonitor()
        {
            MoveOnPrimaryMonitor(MouseLocation.TopLeft, false);
        }


        /// <summary>
        /// Moves the mouse to the specified location of the primary monitor.
        /// </summary>
        /// <param name="location">Location for the mouse relative to the desktop. Default: TopLeft</param>
        /// <param name="synchronous">Wait for input events or return immediately. Default: false</param>
        public static void MoveOnPrimaryMonitor(MouseLocation location, bool synchronous)
        {
            bool sync = s_isSynchronous;

            // Temporarily force sync/async behavior
            // for the caller.

            try
            {
                s_isSynchronous = synchronous;
                Move((IntPtr)NativeMethods.GetDesktopWindow(), location, true);
            }
            finally
            {
                s_isSynchronous = sync;
            }
        }


        /// <summary>
        /// Move the mouse to the center of the specified UIElement.
        /// </summary>
        /// <param name="element">Specified UIElement.</param>
        public static void Move(UIElement element)
        {
            Move(element, MouseLocation.Center, false);
        }

        /// <summary>
        /// Move the mouse to the specified location of the specified UIElement.
        /// </summary>
        /// <param name="element">UIElement that will be the final position for the mouse.</param>
        /// <param name="location">Specified location for the mouse on the UIElement.</param>
        public static void Move(UIElement element, MouseLocation location)
        {
            MoveImpl(element, location, true, false);
        }

        /// <summary>
        /// Move the mouse to the specified location of the specified UIElement.
        /// </summary>
        /// <param name="element">UIElement that it will be the final position for the mouse.</param>
        /// <param name="location">Specified location for the mouse on the UIElement.</param>
        /// <param name="isImmediate">Whether or not to interpolate points between the start and end points.</param>        
        public static void Move(UIElement element, MouseLocation location, bool isImmediate)
        {
            MoveImpl(element, location, true, isImmediate);
        }

        /// <summary>
        /// Move the mouse to the specified location of the specified UIElement.
        /// </summary>
        /// <param name="element">UIElement that it will be the final position for the mouse.</param>
        /// <param name="location">Specified location for the mouse on the UIElement.</param>
        public static void MoveOutside(UIElement element, MouseLocation location)
        {
            MoveImpl(element, location, false, false);
        }

        /// <summary>
        /// Move the mouse to the specified location of the specified UIElement.
        /// </summary>
        /// <param name="element">UIElement that it will be the final position for the mouse.</param>
        /// <param name="location">Specified location for the mouse on the UIElement.</param>
        /// <param name="isImmediate">Whether or not to interpolate points between the start and end points.</param>        
        public static void MoveOutside(UIElement element, MouseLocation location, bool isImmediate)
        {
            MoveImpl(element, location, false, isImmediate);            
        }

        /// <summary>
        /// Mouse down at the current position, move the mouse to the specified location 
        /// of the specified UIElement, and release the mouse.
        /// </summary>
        /// <param name="element">UIElement that it will be the final position for the mouse.</param>
        /// <param name="location">Specified location for the mouse on the UIElement.</param>
        /// <param name="button">Mouse button that will be use for the drag.</param>
        /// <remarks>
        /// Since the OLE drag/drop component pumps messages, drag simulation must always be done synchronously.
        /// The button press &amp; release and mouse movements are queued before any of them run.  Then, we 
        /// start processing the queue (by pumping messages).  When the drag start, OLE starts pumping messages, 
        /// and that will process more of the queue. The drag ends when the button release is processed.
        /// </remarks>
        public static void Drag(UIElement element, MouseLocation location, MouseButton button)
        {
            DragImpl(element, location, button, true);
        }
     
        /// <summary>
        /// Move the mouse to the specified location of the specified UIElement.
        /// </summary>
        /// <param name="element">UIElement that it will be the final position for the mouse.</param>
        /// <param name="location">Specified location for the mouse on the UIElement.</param>
        /// <param name="interior">Specified location for the mouse on the UIElement.</param>
        /// <param name="isImmediate">Whether or not to interpolate points between the start and end points.</param>  
        private static void MoveImpl(UIElement element, MouseLocation location, bool interior, bool isImmediate)
        {
            // Bring the HWND to the front of the screen.
            HandleRef hwnd = PresentationHelper.GetHwnd(element);
            SetHwndForeground((IntPtr)hwnd);

            // Getting the final point where the mouse will be set.
            Point finalPoint = s_avalonMouse.GetPoint(element, location, interior);

            // Getting the current point where the mouse is located.
            Point currentPosition = MouseSimulation.GetCurrentMousePosition();

            MoveImpl(currentPosition, finalPoint, isImmediate);
        }

        // Returns Point array with optional interopolation.
        private static Point[] GetPoints(Point startPoint, Point endPoint, bool isImmediate)
        {
            Point[] points = null;

            if (isImmediate)
            {
                points = new Point[] { startPoint, endPoint };
            }
            else
            {
                points = InterpolationHelper.GetPoints((int)startPoint.X,
                    (int)startPoint.Y,
                    (int)endPoint.X,
                    (int)endPoint.Y);
            }

            return points;
        }

        /// <summary>
        /// Generates a left button click (or mouse primary button click) effect on the current mouse 
        /// position. (a.k.a. Win32 LButtonDown &amp; LButtonUp)
        /// </summary>
        public static void Click()
        {
            Click(MouseButton.Left);
        }

        /// <summary>
        /// Generates a click effect on the current mouse position using the specified mouse button.
        /// </summary>
        /// <param name="button">Mouse button that will be use for the click.</param>
        public static void Click(MouseButton button)
        {
            s_avalonMouse.Click(button);
            TryWaitForInputEvents();
        }

        /// <summary>
        /// Generates a speficied amount of clicks effect on the current mouse position 
        /// using the specified mouse button.
        /// </summary>
        /// <param name="button">Mouse button that will be use for the click.</param>
        /// <param name="amountOfClicks">Number of Clicks.</param>
        public static void Click(MouseButton button, int amountOfClicks)
        {
            s_avalonMouse.Click(button, amountOfClicks);
            TryWaitForInputEvents();
        }


        /// <summary>
        /// Generates a click effect on the specified position relative to the 
        /// specified HWND.
        /// </summary>
        /// <param name="hwnd">HWND that will use to calculate the relative position.</param>
        public static void Click(IntPtr hwnd)
        {
            Click(MouseButton.Left,hwnd,MouseLocation.Center);
        }



        /// <summary>
        /// Generates a click effect on the specified position relative to the 
        /// specified HWND.
        /// </summary>
        /// <param name="hwnd">HWND that will use to calculate the relative position.</param>
        /// <param name="location">Specified relative location.</param>
        public static void Click(IntPtr hwnd, MouseLocation location)
        {
            Click(MouseButton.Left,hwnd,location);
        }


        /// <summary>
        /// Generates a click effect on the specified position relative to the 
        /// specified HWND.
        /// </summary>
        /// <param name="button">Mouse button that will be use for the click.</param>
        /// <param name="hwnd">HWND that will use to calculate the relative position.</param>
        public static void Click(MouseButton button, IntPtr hwnd)
        {
            Move(hwnd, MouseLocation.Center);
            Click(button);
        }


        /// <summary>
        /// Generates a click effect on the specified position relative to the 
        /// specified HWND.
        /// </summary>
        /// <param name="button">Mouse button that will be use for the click.</param>
        /// <param name="hwnd">HWND that will use to calculate the relative position.</param>
        /// <param name="location">Specified relative location.</param>
        public static void Click(MouseButton button, IntPtr hwnd, MouseLocation location)
        {
            Move(hwnd,location);
            Click(button);
        }

        /// <summary>
        /// Generates a left button click (or mouse primary button click) effect 
        /// on the center position of the specified UIElement. (a.k.a. Win32 LButtonDown &amp; LButtonUp)
        /// </summary>
        /// <param name="element"></param>
        public static void Click(UIElement element)
        {
            Click(MouseButton.Left, element, MouseLocation.Center);
        }


        /// <summary>
        /// Generates a left button click (or mouse primary button click) effect 
        /// on the center position of the specified UIElement. (a.k.a. Win32 LButtonDown &amp; LButtonUp)
        /// </summary>
        /// <param name="element"></param>
        /// <param name="amountOfClicks">Number of Clicks.</param>
        public static void Click(UIElement element, int amountOfClicks)
        {
            Move(element);
            Click(MouseButton.Left, amountOfClicks);
        }

        /// <summary>
        /// Generates a left button click (or mouse primary button click) effect 
        /// on the center position of the specified UIElement. (a.k.a. Win32 LButtonDown &amp; LButtonUp)
        /// </summary>
        /// <param name="button"></param>
        /// <param name="element"></param>
        /// <param name="amountOfClicks">Number of Clicks.</param>
        public static void Click(MouseButton button, UIElement element, int amountOfClicks)
        {
            Move(element);
            Click(button, amountOfClicks);
        }

        /// <summary>
        /// Generates a click effect using the specified button
        /// on the center position of the specified UIElement. (a.k.a. Win32 ?ButtonDown &amp; ?ButtonUp)
        /// </summary>
        /// <param name="button">Mouse button that will be use for the click.</param>
        /// <param name="element">UIElement used to move the mouse to its center position.</param>
        public static void Click(MouseButton button, UIElement element)
        {
            Click(button, element, MouseLocation.Center);
        }

        /// <summary>
        /// Generates a click effect using the specified button
        /// on the specified position of the specified UIElement. (a.k.a. Win32 ?ButtonDown &amp; ?ButtonUp)
        /// </summary>
        /// <param name="button">Mouse button that will be use for the click.</param>
        /// <param name="element">UIElement used to move the mouse to its center position.</param>
        /// <param name="location">Location relative to the UIElement.</param>
        public static void Click(MouseButton button, UIElement element, MouseLocation location)
        {
            Move(element,location);
            Click(button);
        }

        /// <summary>
        /// Moves the mouse wheel the specified delta.
        /// </summary>
        /// <param name="delta">Delta for moving the mouse wheel.
        /// Using a positive number will be up. Negative will be down.</param>
        public static void MoveWheel(int delta)
        {
            ((MouseSimulation)s_avalonMouse).MoveWheel(delta);
            TryWaitForInputEvents();
        }

        /// <summary>
        /// Moves the mouse wheel on the specified direction and the specified amount.
        /// </summary>
        /// <param name="direction">Direction to move the wheel.</param>
        /// <param name="notchCount">Amount of movement.</param>
        public static void MoveWheel(MouseWheelDirection direction, int notchCount) 
        {
            ((MouseSimulation)s_avalonMouse).MoveWheel(direction, notchCount);
            TryWaitForInputEvents();
        }

        /// <summary>
        /// Waits for all to get processed.
        /// </summary>
        public static void PressButton()
        {
            PressButton(MouseButton.Left);
        }


        /// <summary>
        /// Waits for all to get processed.
        /// </summary>
        public static void ReleaseButton()
        {
            ReleaseButton(MouseButton.Left);
        }


        /// <summary>
        /// Waits for all to get processed.
        /// </summary>
        public static void PressButton(MouseButton button)
        {
            ((MouseSimulation)s_avalonMouse).Down(button);
            TryWaitForInputEvents();
        }


        /// <summary>
        /// Waits for all to get processed.
        /// </summary>
        public static void ReleaseButton(MouseButton button)
        {
            ((MouseSimulation)s_avalonMouse).Up(button);
            TryWaitForInputEvents();
        }

        /// <summary>
        /// Determines if input should be "complete" before the input 
        /// routine returns.
        /// </summary>
        public static bool IsSynchronous
        {
            get
            {
                return s_isSynchronous;
            }
            set
            {
                s_isSynchronous = value;
            }
        }

        private static bool s_isSynchronous = true;

        /// <summary>
        /// Wait for new low-level input messages if IsSynchronous is true.
        /// </summary>
        private static void TryWaitForWin32InputMessages()
        {
            if (IsSynchronous)
            {
                do
                {
                    DispatcherHelper.DoEvents(DispatcherPriority.Input);
                }
                while (InputHelper.IsInputPending());
            }
        }

        /// <summary>
        /// Wait for all input events if IsSynchronous is true.
        /// </summary>
        private static void TryWaitForInputEvents()
        {
            if (IsSynchronous)
            {
                do
                {
                    DispatcherHelper.DoEventsPastInput();
                }
                while (InputHelper.IsInputPending());
            }
        }

        /// <summary>
        /// Move the specified HWND to the foreground of the desktop.
        /// </summary>
        /// <param name="hwnd"></param>
        private static void SetHwndForeground(IntPtr hwnd)
        {
            if (hwnd != IntPtr.Zero)
            {
                NativeMethods.SetForegroundWindow(new HandleRef(null,hwnd));
                Thread.Sleep(30);
            }
        }
    }
}
