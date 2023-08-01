// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Wrapper for resizing windows of various types.
 * 
 *
 
  
 * Revision:         $Revision: 2 $
 
 *********************************************************************/

using System;
using System.Threading;
using System.Runtime.InteropServices;

using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

using Microsoft.Test.Input;
using Microsoft.Test.Threading;
using Microsoft.Test.Win32;

using Avalon.Test.CoreUI.Trusted;

namespace Avalon.Test.CoreUI.Trusted
{
    /// <summary>
    /// Method for resizing.
    /// </summary>
    public enum ResizeMethod 
    { 
        /// <summary>
        /// </summary>
        Mouse,
        /// <summary>
        /// </summary>
        Keyboard,
        /// <summary>
        /// </summary>
        Programmatic 
    };

    /// <summary>
    /// Handle to use for resizing.
    /// </summary>
    public enum ResizeHandle
    {
        /// <summary>
        /// </summary>
        North,
        /// <summary>
        /// </summary>
        South,
        /// <summary>
        /// </summary>
        East,
        /// <summary>
        /// </summary>
        West,
        /// <summary>
        /// </summary>
        Northwest,
        /// <summary>
        /// </summary>
        Northeast,
        /// <summary>
        /// </summary>
        Southwest,
        /// <summary>
        /// </summary> 
        Southeast 
    };

    /// <summary>
    /// Direction to move window handle once acquired.
    /// </summary>
    public enum ResizeDirection
    {
        /// <summary>
        /// </summary>
        North,
        /// <summary>
        /// </summary>
        South,
        /// <summary>
        /// </summary>
        East,
        /// <summary>
        /// </summary>
        West,
        /// <summary>
        /// </summary>
        Northwest,
        /// <summary>
        /// </summary>
        Northeast,
        /// <summary>
        /// </summary>
        Southwest,
        /// <summary>
        /// </summary>
        Southeast 
    };

    /// <summary>
    /// Delegate for giving the user an opportunity to do verification during resize.
    /// </summary>
    public delegate void ResizeHandler();

    /// <summary>
    /// Class to resize a surface using keyboard, mouse and programmatic methods.
    /// </summary>
    /// <remarks>
    /// Extra-super not thread safe.
    /// </remarks>
    public class WindowResizeHelper
    {
        private Size _virtualScreenSize;
        private Point _virtualScreenPoint;
        private Size _primaryScreen;

        /// <summary>
        /// </summary>
        public WindowResizeHelper(ResizeHandler r)
        {
            // Action to perform after resize.
            _resizeHandler = r;

            _virtualScreenSize = MultiMonitorHelper.GetVirtualScreenSize();
            _virtualScreenPoint = MultiMonitorHelper.GetVirtualScreenTopLeftPoint();

            _primaryScreen = new Size(
                NativeMethods.GetSystemMetrics(NativeConstants.SM_CXSCREEN),
                NativeMethods.GetSystemMetrics(NativeConstants.SM_CYSCREEN));
        }


        /// <summary>
        /// Do everything resize method. Single entry point to all supported resizing.
        /// </summary>
        public void Resize(Surface surface, ResizeMethod method, ResizeHandle handle, ResizeDirection direction)
        {
            switch (method)
            {
                case ResizeMethod.Mouse:
                    MouseResize(surface, handle, direction);
                    break;

                case ResizeMethod.Keyboard:
                    if (DoMultimonResize(surface, handle, direction)) _keyCount = 100;
                    else _keyCount = 10;
                    KeyboardResize(handle, direction);
                    break;

                case ResizeMethod.Programmatic:
                    ProgrammaticResize(surface, direction);
                    break;
            }
        }

        #region Mouse Resize

        // todo: Many of these methods are implemented internally elsewhere, move this class to
        // coreteststrusted and use those methods directly.

        /// <summary>
        /// Resize surface by dragging handle in some direction.
        /// </summary>
        public void MouseResize(Surface surface, ResizeHandle handle, ResizeDirection direction)
        {
            // Put mouse over surface edge.
            MouseHelper.MoveOutside(surface.Handle, _getMouseLocation(handle));

            // Drag mouse, can't use MouseHelper for this :(
            MouseDragWithTimer(GetCurrentMousePosition(), _getMouseDragTarget(surface, direction, DoMultimonResize(surface, handle, direction)));

            MouseHelper.MoveOnVirtualScreenMonitor();

            // Done with resize, allow user to perform verification.
            _resizeHandler();
        }

        /// <summary>
        /// Translate ResizeHandle to MouseLocation used by MouseHelper.
        /// </summary>
        private MouseLocation _getMouseLocation(ResizeHandle handle)
        {
            switch (handle)
            {
                case ResizeHandle.North:
                    return MouseLocation.Top;

                case ResizeHandle.South:
                    return MouseLocation.Bottom;

                case ResizeHandle.East:
                    return MouseLocation.CenterRight;

                case ResizeHandle.West:
                    return MouseLocation.CenterLeft;

                case ResizeHandle.Northwest:
                    return MouseLocation.TopLeft;

                case ResizeHandle.Northeast:
                    return MouseLocation.TopRight;

                case ResizeHandle.Southwest:
                    return MouseLocation.BottomLeft;

                case ResizeHandle.Southeast:
                    return MouseLocation.BottomRight;
            }

            throw new Exception("Invalid resize handle, " + handle);
        }

        /// <summary>
        /// Get a point to drag the mouse toward.
        /// </summary>
        private Point _getMouseDragTarget(Surface surface, ResizeDirection direction, bool multipleMonitors)
        {
            // Target starts from current mouse position.
            Point target = GetCurrentMousePosition();

            int heightDistance = surface.Height / 2;
            int widthDistance = surface.Width / 2;

            if (multipleMonitors)
            {
                heightDistance = (int)_primaryScreen.Height;
                widthDistance = (int)_primaryScreen.Width;
            }

            // All north and south directions affect target's Y value.
            switch (direction)
            {
                case ResizeDirection.North:
                case ResizeDirection.Northwest:
                case ResizeDirection.Northeast:
                    target.Y += heightDistance;
                    break;

                case ResizeDirection.South:
                case ResizeDirection.Southwest:
                case ResizeDirection.Southeast:
                    target.Y -= heightDistance;
                    break;
            }

            // All east west directions affect target's X value.
            switch (direction)
            {
                case ResizeDirection.East:
                case ResizeDirection.Northeast:
                case ResizeDirection.Southeast:
                    target.X += widthDistance;
                    break;

                case ResizeDirection.West:
                case ResizeDirection.Northwest:
                case ResizeDirection.Southwest:
                    target.X -= widthDistance;
                    break;
            }

            return target;
        }

        /// <summary>
        /// Click, drag and release mouse between two points.
        /// </summary>
        /// <remarks>
        /// Works around MouseHelper's nested pumps using a ----py hack.
        /// </remarks>
        private void MouseDragWithTimer(Point startPoint, Point endPoint)
        {
            MouseHelper.IsSynchronous = false;

            MouseHelper.PressButton();

            _currentPoint = 0;
            _points = GetPoints(startPoint, endPoint);

            TimerCallback timerDelegate = new TimerCallback(_mouseDragCallback);
            _t = new Timer(timerDelegate, null, 0, 10);

            // Push frame and wait until timer is done resizing.
            while (_currentPoint < _points.Length)
            {
                DispatcherHelper.DoEvents(500);
            }
        }

        /// <summary>
        /// Move the mouse to the next drag point. Release and stop timer when we reach the last point.
        /// </summary>
        private void _mouseDragCallback(object s)
        {
            if (_currentPoint > _points.Length - 1)
            {
                _t.Change(Timeout.Infinite, Timeout.Infinite);
                MouseHelper.ReleaseButton();
            }
            else
            {
                Move((int)_points[_currentPoint].X, (int)_points[_currentPoint].Y);
            }

            _currentPoint++;
        }

        /// <summary>
        /// Move mouse to point x, y
        /// </summary>
        private static void Move(int x, int y)
        {
            Input.SendMouseInput(x, y, 0, SendMouseInputFlags.Absolute | SendMouseInputFlags.Move);
        }

        /// <summary>
        /// Build array of point for mouse to move along.
        /// </summary>
        private static Point[] GetPoints(Point startPoint, Point endPoint)
        {
            return InterpolationHelper.GetPoints((int)startPoint.X, (int)startPoint.Y, (int)endPoint.X, (int)endPoint.Y);
        }

        /// <summary>
        /// Return current mouse position.
        /// </summary>
        private static Point GetCurrentMousePosition()
        {
            NativeStructs.POINT point1 = new NativeStructs.POINT(0, 0);
            NativeMethods.GetCursorPos(ref point1);
            return new Point((double)point1.x, (double)point1.y);
        }

        #endregion

        #region Keyboard Resize

        /// <summary>
        /// </summary>
        public void KeyboardResize(ResizeHandle handle, ResizeDirection direction)
        {
            KeyboardHelper.IsSynchronous = false;

            // Open system menu and select Size menu entry.
            KeyboardHelper.TypeKey(Key.Space, ModifierKeys.Alt);
            KeyboardHelper.TypeKey(Key.S);

            //
            // Select direction handle, handle will deterimine arrow(s) used to resize window. 
            //

            // Select cardinal direction handle.
            switch (handle)
            {
                case ResizeHandle.North:
                case ResizeHandle.Northwest:
                case ResizeHandle.Northeast:
                    KeyboardHelper.TypeKey(Key.Up);
                    break;

                case ResizeHandle.South:
                case ResizeHandle.Southwest:
                case ResizeHandle.Southeast:
                    KeyboardHelper.TypeKey(Key.Down);
                    break;

                case ResizeHandle.East:
                    KeyboardHelper.TypeKey(Key.Right);
                    break;

                case ResizeHandle.West:
                    KeyboardHelper.TypeKey(Key.Left);
                    break;
            }

            // Select ordinal direction handle.
            switch (handle)
            {
                case ResizeHandle.Northwest:
                case ResizeHandle.Southwest:
                    KeyboardHelper.TypeKey(Key.Left);
                    break;

                case ResizeHandle.Northeast:
                case ResizeHandle.Southeast:
                    KeyboardHelper.TypeKey(Key.Right);
                    break;

            }

            // Do Keyboard resize with a timer. 
            KeyboardResizeWithTimer(direction);

            // Done with resize, allow user to perform verification.
            _resizeHandler();
        }

        private void KeyboardResizeWithTimer(ResizeDirection direction)
        {
            _direction = direction;

            TimerCallback timerDelegate = new TimerCallback(_KeyboardResizeCallback);
            _t = new Timer(timerDelegate, null, 0, 100);

            // Push frame and wait until timer is done resizing.
            // This should only happen once, when resizing starts we get stuck in
            // a nested pump.
            while (_keyCount > 0)
            {
                DispatcherHelper.DoEvents(500);
            }

        }

        private void _KeyboardResizeCallback(object s)
        {
            if (_keyCount <= 0)
            {
                // Stop timer and resizing, we're done.
                _t.Change(Timeout.Infinite, Timeout.Infinite);
                KeyboardHelper.TypeKey(Key.Enter);
            }
            else
            {
                // Resize in cardinal direction.
                switch (_direction)
                {
                    case ResizeDirection.North:
                    case ResizeDirection.Northwest:
                    case ResizeDirection.Northeast:
                        KeyboardHelper.TypeKey(Key.Up);
                        break;

                    case ResizeDirection.South:
                    case ResizeDirection.Southwest:
                    case ResizeDirection.Southeast:
                        KeyboardHelper.TypeKey(Key.Down);
                        break;

                    case ResizeDirection.East:
                        KeyboardHelper.TypeKey(Key.Right);
                        break;

                    case ResizeDirection.West:
                        KeyboardHelper.TypeKey(Key.Left);
                        break;
                }

                // Resize in ordinal direction.
                switch (_direction)
                {
                    case ResizeDirection.Northwest:
                    case ResizeDirection.Southwest:
                        KeyboardHelper.TypeKey(Key.Left);
                        break;

                    case ResizeDirection.Northeast:
                    case ResizeDirection.Southeast:
                        KeyboardHelper.TypeKey(Key.Right);
                        break;
                }
            }

            // Let test app do whatever it wants between resizes.
            //if (_keyCount == 5) _resizeHandler();

            _keyCount--;
        }

        #endregion

        #region Programmatic Resize

        /// <summary>
        /// Programmatically resize a surface.
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="direction"></param>
        public void ProgrammaticResize(Surface surface, ResizeDirection direction)
        {
            // Start with current size.
            NativeStructs.RECT windowRect = NativeStructs.RECT.Empty;
            NativeMethods.GetWindowRect(new HandleRef(null, surface.Handle), ref windowRect);

            int width = windowRect.Width;
            int height = windowRect.Height;

            if (DoMultimonResize(surface, ResizeHandle.Northwest, ResizeDirection.Southeast))
            {
                width = (int)_primaryScreen.Width;
                height = (int)_primaryScreen.Height;
            }

            // Direction is applied to the bottom right corner relative to the top left corner. 

            // All northern directions will shorten the window height.
            // All southern directions will increase the window height.
            switch (direction)
            {
                case ResizeDirection.North:
                case ResizeDirection.Northwest:
                case ResizeDirection.Northeast:
                    height /= 2;
                    break;

                case ResizeDirection.South:
                case ResizeDirection.Southwest:
                case ResizeDirection.Southeast:
                    height *= 2;
                    break;

            }

            // All western directions will narrow the window width.
            // All eastern directions will expand the windows width.
            switch (direction)
            {
                case ResizeDirection.East:
                case ResizeDirection.Southeast:
                case ResizeDirection.Northeast:
                    width *= 2;
                    break;

                case ResizeDirection.West:
                case ResizeDirection.Northwest:
                case ResizeDirection.Southwest:
                    width *= 2;
                    break;
            }

            CoreLogger.LogStatus(" Programmatically resizing to " + width + "," + height);
            surface.SetSize(width, height);

            // Helpful pause.
            DispatcherHelper.DoEvents(500);

            // Allow user to verify resize.
            _resizeHandler();
        }

        #endregion

        #region MultipleMonitorSupport

        private bool MultipleMonitorsHorizontal()
        {
            if (_virtualScreenSize.Width > _primaryScreen.Width) return true;

            return false;
        }

        private bool MultipleMonitorsVertical()
        {
            if (_virtualScreenSize.Height > _primaryScreen.Height) return true;

            return false;
        }

        /// <summary>
        /// Test if a surface spans multiple monitors.
        /// </summary>
        /// <param name="surface">Surface to check.</param>
        /// <returns>True if surface crosses a monitor boundary.</returns>
        /// <remarks>
        /// Assumes both monitors have the same resolution and are arranged 
        /// horizontally or vertically.
        /// </remarks>
        private bool SurfaceAcrossMonitors(Surface surface)
        {
            if (MultipleMonitorsHorizontal())
            {
                // Horizontal monitors

                if (surface.Left < _virtualScreenPoint.X + _primaryScreen.Width &&
                    surface.Left + surface.Width > _virtualScreenPoint.X + _primaryScreen.Width)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (MultipleMonitorsVertical())
            {
                // Vertical monitors

                if (surface.Top < _virtualScreenPoint.Y + _primaryScreen.Height &&
                    surface.Top + surface.Height > _virtualScreenPoint.Y + _primaryScreen.Height)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // Only one monitor so surface is not across boundary.
            return false;
        }


        /// <summary>
        /// Test if the resize motion moves toward a multiple monitor boundary.
        /// </summary>
        /// <param name="surface">Surface, for current position</param>
        /// <param name="direction">Direction of resize</param>
        /// <returns>True if resize motion moves toward multiple monitor boundary</returns>
        private bool DoesResizeTowardBoundary(Surface surface, ResizeDirection direction)
        {
            if (MultipleMonitorsHorizontal())
            {

                if (surface.Left < _virtualScreenPoint.X + _primaryScreen.Width)
                {
                    // Surface is left of boundary

                    if (direction == ResizeDirection.East ||
                        direction == ResizeDirection.Northeast ||
                        direction == ResizeDirection.Southeast)
                    {
                        // East toward boundary.
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    // Surface is right of boundary.

                    if (direction == ResizeDirection.West ||
                        direction == ResizeDirection.Northwest ||
                        direction == ResizeDirection.Southwest)
                    {
                        // West toward boundar.
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (MultipleMonitorsVertical())
            {
                if (surface.Top < _virtualScreenPoint.Y + _primaryScreen.Height)
                {
                    // Surface is above boundary

                    if (direction == ResizeDirection.South ||
                        direction == ResizeDirection.Southwest ||
                        direction == ResizeDirection.Southeast)
                    {
                        // South toward boundary.
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    // Surface is below of boundary.

                    if (direction == ResizeDirection.North ||
                        direction == ResizeDirection.Northwest ||
                        direction == ResizeDirection.Northeast)
                    {
                        // North toward boundary.
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            // One monitor so no boundary.
            return false;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="handle"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        private bool DoesResizeShrink(Surface surface, ResizeHandle handle, ResizeDirection direction)
        {
            switch (handle)
            {
                case ResizeHandle.North:
                    if (direction == ResizeDirection.South)
                        return true;
                    break;
                case ResizeHandle.South:
                    if (direction == ResizeDirection.North)
                        return true;
                    break;
                case ResizeHandle.East:
                    if (direction == ResizeDirection.West)
                        return true;
                    break;
                case ResizeHandle.West:
                    if (direction == ResizeDirection.East)
                        return true;
                    break;
                case ResizeHandle.Northwest:
                    if (direction == ResizeDirection.Southeast ||
                        direction == ResizeDirection.South ||
                        direction == ResizeDirection.East)
                        return true;
                    break;
                case ResizeHandle.Northeast:
                    if (direction == ResizeDirection.Southwest ||
                        direction == ResizeDirection.South ||
                        direction == ResizeDirection.West)
                        return true;
                    break;
                case ResizeHandle.Southwest:
                    if (direction == ResizeDirection.Northeast ||
                        direction == ResizeDirection.North ||
                        direction == ResizeDirection.East)
                        return true;
                    break;
                case ResizeHandle.Southeast:
                    if (direction == ResizeDirection.Northwest ||
                        direction == ResizeDirection.North ||
                        direction == ResizeDirection.West)
                        return true;
                    break;
            }

            return false;
        }

        private bool DoMultimonResize(Surface surface, ResizeHandle handle, ResizeDirection direction)
        {
            // Need multiple monitors.
            if (!(MultipleMonitorsHorizontal() || MultipleMonitorsVertical())) return false;

            if (DoesResizeTowardBoundary(surface, direction))
            {
                // Surface is straddling monitors,
                // and resizing does shrink the window.
                if (SurfaceAcrossMonitors(surface) && DoesResizeShrink(surface, handle, direction))
                {
                    return true;
                }

                // Surface is on one monitor, 
                // and resizing does not shrink window.
                if (!SurfaceAcrossMonitors(surface) && !DoesResizeShrink(surface, handle, direction))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion


        #region Privates

        ResizeHandler _resizeHandler;

        Timer _t = null;

        Point[] _points = null;
        int _currentPoint = 0;

        ResizeDirection _direction;
        int _keyCount = 10;

        #endregion
    }
}
