// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************************
*    Purpose:  Abstract base class for mouse simulation. Subclasses provide
*              domain-specific implementations for primitive mouse operations 
*              such as Move and Click.
*    
 
  
*    Revision:         $Revision: 2 $
 
******************************************************************************/
using System;
using System.Windows;
using System.Security;
using System.Threading;
using System.Collections;
using System.Diagnostics;
using System.Windows.Input;
using Microsoft.Test.Win32;
using System.Windows.Media;
using Microsoft.Test.Threading;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Avalon.Test.CoreUI.Trusted
{
    /// <summary>
    /// Mouse location relative to some target (hwnd, element, etc.).
    /// </summary>
    public enum MouseLocation
    {
        /// <summary>
        /// 
        /// </summary>
        TopLeft,
        
        /// <summary>
        /// 
        /// </summary>
        Top,
        
        /// <summary>
        /// 
        /// </summary>
        TopRight,
        
        /// <summary>
        /// 
        /// </summary>
        CenterLeft,
        
        /// <summary>
        /// 
        /// </summary>
        Center,
        
        /// <summary>
        /// 
        /// </summary>
        CenterRight,
        
        /// <summary>
        /// 
        /// </summary>
        BottomLeft,

        /// <summary>
        /// 
        /// </summary>
        Bottom,
        
        /// <summary>
        /// 
        /// </summary>
        BottomRight 
    }


    /// <summary>
    /// Abstract base class for mouse simulation. Subclasses provide
    /// domain-specific implementations.
    /// </summary>   
    abstract internal class MouseSimulation
    {
        /// <summary>
        /// 
        /// </summary>
        public static Point GetCurrentMousePosition()
        {
            // We need to start with the original point.
            NativeStructs.POINT ptNative = new NativeStructs.POINT(0, 0);
            NativeMethods.GetCursorPos(ref ptNative);
            Point pt = new Point(ptNative.x, ptNative.y);
            return pt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        internal void Move(int x, int y)
        {
            Input.SendMouseInput(x, y, 0, SendMouseInputFlags.Move | SendMouseInputFlags.Absolute);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        public virtual void Down(MouseButton button)
        {
            Win32Button(button, true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        public virtual void Up(MouseButton button)
        {
            Win32Button(button, false);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        public virtual void Click(MouseButton button)
        {
            Click(button, 1);
        }


        /// <summary>
        /// 
        /// </summary>
        public virtual void Click(MouseButton button, int numberOfClicks)
        {
            for (int i = 0; i < numberOfClicks; i++)
            {
                Down(button);
                Up(button);
            } 
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="delta"></param>
        public virtual void MoveWheel(int delta)
        {
            Win32Wheel(delta);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void MoveWheel(MouseWheelDirection direction, int notchCount)
        {
            int delta = Input.WheelDeltaFromWheelMovement(direction, notchCount);
            Win32Wheel(delta);
        }


        // Private default implementation of mouse wheel input.
        void Win32Wheel(int delta)
        {
            Input.SendMouseInput(0, 0, delta, SendMouseInputFlags.Wheel);
        }


        // Private default implementation of mouse button input.
        void Win32Button(MouseButton button, bool isDown)
        {
            SendMouseInputFlags flag;
            flag = SendMouseInputFlags.Move;
            int data = 0;

            SetButtonSettings(button, isDown, out flag, out data);

            Input.SendMouseInput(0, 0, data, flag);
        }

        // Private helper routine for setting button values.
        void SetButtonSettings(MouseButton button, bool isDown,
            out SendMouseInputFlags flag, out int data)
        {
            flag = SendMouseInputFlags.Move;
            data = 0;

            switch (button)
            {
                case MouseButton.Left:

                    if (isDown)
                    {
                        flag = SendMouseInputFlags.LeftDown;
                    }
                    else
                    {
                        flag = SendMouseInputFlags.LeftUp;
                    }
                    break;
                case MouseButton.Right:

                    if (isDown)
                    {
                        flag = SendMouseInputFlags.RightDown;
                    }
                    else
                    {
                        flag = SendMouseInputFlags.RightUp;
                    }
                    break;
                case MouseButton.Middle:

                    if (isDown)
                    {
                        flag = SendMouseInputFlags.MiddleDown;
                    }
                    else
                    {
                        flag = SendMouseInputFlags.MiddleUp;
                    }
                    break;
                case MouseButton.XButton1:

                    if (isDown)
                    {
                        flag = SendMouseInputFlags.XDown;
                    }
                    else
                    {
                        flag = SendMouseInputFlags.XUp;
                    }

                    data = Input.XButton1;

                    break;
                case MouseButton.XButton2:

                    if (isDown)
                    {
                        flag = SendMouseInputFlags.XDown;

                    }
                    else
                    {
                        flag = SendMouseInputFlags.XUp;
                    }

                    data = Input.XButton2;

                    break;
            }
        }
    }
}


