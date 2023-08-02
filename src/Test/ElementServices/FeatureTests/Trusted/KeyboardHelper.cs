// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************************
*    Purpose:  General-use static class that wraps specific keyboard
*              simulation implementations. In addition to high-level
*              helper routines, it provides synchronization with Avalon events.
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
using System.Globalization; 

namespace Avalon.Test.CoreUI.Trusted
{
    /// <summary>
    /// General-use static class that wraps specific keyboard
    /// simulation implementations. In addition to high-level
    /// helper routines, it provides synchronization with Avalon events.
    /// </summary>
    public static class KeyboardHelper
    {
        private static Win32Keyboard s_win32Keyboard = new Win32Keyboard();

        /// <summary>
        /// Sets focus to a given element.  Makes the element focusable if necessary.
        /// </summary>
        /// <param name="inputElement">UIElement or ContentElement.</param>
        public static void EnsureFocus(IInputElement inputElement)
        {
            CoreLogger.LogStatus("Setting focus to the element....");

            bool isFocusSet = false;

            UIElement uiElement = inputElement as UIElement;
            if (uiElement != null)
            {
                if (!uiElement.Focusable)
                {
                    uiElement.Focusable = true;
                }

                isFocusSet = uiElement.Focus();
            }

            ContentElement contentElement = inputElement as ContentElement;
            if (contentElement != null)
            {
                if (!contentElement.Focusable)
                {
                    contentElement.Focusable = true;
                }

                isFocusSet = contentElement.Focus();
            }

            if (uiElement != null || contentElement != null)
            {
                TryWaitForInputEvents();

                if (!isFocusSet)
                {
                    throw new Microsoft.Test.TestValidationException("Focus could not be set to element.");
                }

                if (InputHelper.GetFocusedElement() != inputElement)
                {
                    throw new Microsoft.Test.TestValidationException("Focus was set but the FocusedElement property on KeyboardDevice is not the expected element.");
                }
            
            }
            else
            {
                throw new ArgumentException("inputElement", "The element must be a UIElement or ContentElement.");
            }
        }

        /// <summary>
        /// Reset all keyboard keys to released and untoggled.
        /// </summary>
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
		public static void ResetKeyboardState()
        {            
            byte[] states = new byte[256];

            NativeMethods.GetKeyboardState(states);

            // Turn off toggle keys.
            // This needs to be done before clearing all key states.
            foreach (byte vk in s_vkeysToReset)
            {
                if ((states[vk] & 1) > 0)
                {
                    NativeMethods.Keybd_event(vk, 0x45,
                        NativeConstants.KEYEVENTF_EXTENDEDKEY | 0,
                        IntPtr.Zero);

                    NativeMethods.Keybd_event(vk, 0x45,
                        NativeConstants.KEYEVENTF_EXTENDEDKEY | NativeConstants.KEYEVENTF_KEYUP,
                        IntPtr.Zero);
                }
            }

            // Release all keys.
            for(int i=0; i < 256; i++)
            {
                states[i] &= 0x00;
            }
            NativeMethods.SetKeyboardState(states);            
        }

        /// <summary>
        /// Types a speficied key.
        /// </summary>
        /// <param name="key">Key to type</param>
        public static void TypeKey(Key key)
        {
            TypeKey(key, ModifierKeys.None);       
        }

        /// <summary>
        /// Types a series of speficied keys. It will type on the same order
        /// as it is on the array of keys.
        /// </summary>
        /// <param name="keys">Keys to type.</param>
        public static void TypeKey(Key[] keys)
        {
            TypeKey(keys, ModifierKeys.None);       
        }

        /// <summary>
        /// Types the specified string.
        /// </summary>
        /// <param name="str"></param>
        public static void TypeKey(string str)
        {
            TypeKey(str, ModifierKeys.None);
        }

        /// <summary>
        /// Type the specified string on the scope of the key modifiers.
        /// </summary>
        /// <param name="str">String to type.</param>
        /// <param name="modifiers">Modifiers that will affect the specified string.</param>
        public static void TypeKey(string str, ModifierKeys modifiers)
        {
            Key[] keys = ConvertFromString(str);
            TypeKey(keys, modifiers);
        }
       
        /// <summary>
        /// Type the specified key on the scope of the key modifiers.
        /// </summary>
        /// <param name="key">Key to type.</param>
        /// <param name="modifiers">Modifiers that will affect the specified string.</param>
        public static void TypeKey(Key key, ModifierKeys modifiers)
        {
            Key[] keyArray = {key};
            TypeKey(keyArray, modifiers);
        }

        /// <summary>
        /// Type the specified keys on the scope of the key modifiers. It will bring the 
        /// specified window to the foreground.
        /// </summary>
        /// <param name="window">Window that will be move to the foreground.</param>
        /// <param name="keys">Keys to type.</param>
        /// <param name="modifiers">Modifiers that will affect the specified string.</param>
        public static void TypeKey(Window window, Key[] keys, ModifierKeys modifiers)
        {
            IntPtr hwnd = (IntPtr)PresentationHelper.GetHwnd(window);
            TypeKey(hwnd, keys, modifiers);
        }

        /// <summary>
        /// Type the specified keys on the scope of the key modifiers. It will bring the 
        /// specified HWND to the foreground.
        /// </summary>
        /// <param name="hwnd">HWND that will be move to the foreground.</param>
        /// <param name="key">Key to type.</param>
        /// <param name="modifiers">Modifiers that will affect the specified string.</param>
        public static void TypeKey(IntPtr hwnd, Key key, ModifierKeys modifiers)
        {
            SetHwndForeground(hwnd);
            Key[] keys = {key};
            TypeKey(keys,modifiers);
        }


        /// <summary>
        /// Type the specified keys on the scope of the key modifiers. It will bring the 
        /// specified HWND to the foreground.
        /// </summary>
        /// <param name="hwnd">HWND that will be move to the foreground.</param>
        /// <param name="keys">Keys to type.</param>
        /// <param name="modifiers">Modifiers that will affect the specified string.</param>
        public static void TypeKey(IntPtr hwnd, Key[] keys, ModifierKeys modifiers)
        {
            SetHwndForeground(hwnd);
            TypeKey(keys,modifiers);
        }

        /// <summary>
        /// Type the specified string on the scope of the key modifiers. It will bring the 
        /// specified window to the foreground.
        /// </summary>
        /// <param name="window">Window that will be move to the foreground.</param>
        /// <param name="str">String to type.</param>
        /// <param name="modifiers">Modifiers that will affect the specified string.</param>
        public static void TypeKey(Window window, string str, ModifierKeys modifiers)
        {
            IntPtr hwnd = (IntPtr)PresentationHelper.GetHwnd(window);
            TypeKey(hwnd, str, modifiers);
        }


        /// <summary>
        /// Type the specified keys on the scope of the key modifiers. It will bring the 
        /// specified HWND to the foreground.
        /// </summary>
        /// <param name="hwnd">HWND that will be move to the foreground.</param>
        /// <param name="str">String to type.</param>
        /// <param name="modifiers">Modifiers that will affect the specified string.</param>
        public static void TypeKey(IntPtr hwnd, string str, ModifierKeys modifiers)
        {
            SetHwndForeground(hwnd);
            TypeKey(str, modifiers);
        }


        /// <summary>
        /// Type the specified keys on the scope of the key modifiers.
        /// </summary>
        /// <param name="keys">Keys to type.</param>
        /// <param name="modifiers">Modifiers that will affect the specified string.</param>
        public static void TypeKey(Key[] keys, ModifierKeys modifiers)
        {
            ModifierKeysAction(modifiers, true);

            for (int i=0; i < keys.Length; i++)
            {
                InternalKeyAction(keys[i],true);
                InternalKeyAction(keys[i], false);
                TryWaitForInputEvents();
            }
            
            ModifierKeysAction(modifiers, false);
        }

        /// <summary>
        /// Presses the specified key (key down).
        /// </summary>
        /// <param name="key">Key to be pressed.</param>
        public static void PressKey(Key key)
        {
            InternalKeyAction(key, true);
            TryWaitForInputEvents();

        }

        /// <summary>
        /// Presses the specified key+modifier combination (key down).
        /// </summary>
        /// <param name="key">Key to be pressed.</param>
        /// <param name="modifiers">Modifiers to be pressed.</param>
        public static void PressKey(Key key, ModifierKeys modifiers)
        {
            ModifierKeysAction(modifiers, true);

            InternalKeyAction(key, true);
            TryWaitForInputEvents();
        }

        /// <summary>
        /// Releases the specified key (key up).
        /// </summary>
        /// <param name="key">Key to be released.</param>
        public static void ReleaseKey(Key key)
        {
            InternalKeyAction(key, false);
            TryWaitForInputEvents();
        }

        /// <summary>
        /// Releases the specified key+modifier combination (key up).
        /// </summary>
        /// <param name="key">Key to be released.</param>
        /// <param name="modifiers">Modifiers to be released.</param>
        public static void ReleaseKey(Key key, ModifierKeys modifiers)
        {
            ModifierKeysAction(modifiers, false);

            InternalKeyAction(key, false);
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


        // If synchronous option is set, handle it here.
        private static void TryWaitForInputEvents()
        {
            if (IsSynchronous)
            {
                do
                {
                    DispatcherHelper.DoEvents(15, DispatcherPriority.SystemIdle);
                }
                while (InputHelper.IsInputPending());
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd"></param>
        private static void SetHwndForeground(IntPtr hwnd)
        {
            if (hwnd != IntPtr.Zero && NativeMethods.IsWindow(new HandleRef(null, hwnd)))
            {
                NativeMethods.SetForegroundWindow(new HandleRef(null, hwnd));                
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modifiers"></param>
        /// <param name="keyDown"></param>
        private static void ModifierKeysAction(ModifierKeys modifiers, bool keyDown)
        {
            if (modifiers != ModifierKeys.None)
            {
                if ((modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                {
                    InternalKeyAction(Key.LeftAlt,keyDown);       
                }

                if ((modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    InternalKeyAction(Key.LeftCtrl,keyDown);       
                }

                if ((modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    InternalKeyAction(Key.LeftShift,keyDown);       
                }
            }

            TryWaitForInputEvents();
        }


        private static Key[] ConvertFromString(string str)
        {
            if (str == null)
            {
                throw new InvalidOperationException("The string cannot be null");
            }

            List<Key> keys = new List<Key>(str.Length);
            KeyConverter converter = new KeyConverter();

            for (int i = 0; i < str.Length; i++)
            {
                Key key = (Key)converter.ConvertFrom(null, CultureInfo.InvariantCulture, str[i].ToString());
                keys.Add(key);
            }

            return keys.ToArray();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keyDown"></param>
        private static void InternalKeyAction(Key key, bool keyDown)
        {
            if (keyDown)
            {
                s_win32Keyboard.KeyDown(key);
            }
            else
            {
                s_win32Keyboard.KeyUp(key);
            }
        }     


        /// <summary>
        /// Keys to "unpress" when user asks for a keyboard reset.
        /// </summary>
        private static byte[] s_vkeysToReset = new byte[] {
            VKeys.VkCapital, // This doesn't actually do anything.
            VKeys.VkNumlock, // This doesn't actually do anything.
            VKeys.VkScroll,  // This doesn't actually do anything.
        };
    }

    /// <summary>
    /// Simulates Win32 Keyboard Actions.
    /// </summary>
    internal class Win32Keyboard
    {
        /// <summary>
        /// Simulates a key down (hold press key).
        /// </summary>
        /// <param name="key">Key to be pressed.</param>
        public void KeyDown(Key key)
        {
            Win32KeyDown(key, true);
        }

        /// <summary>
        /// Simulated a Key up.
        /// </summary>
        /// <param name="key">Key to be released.</param>
        public void KeyUp(Key key)
        {
            Win32KeyDown(key, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="isDown"></param>
        private void Win32KeyDown(Key key, bool isDown)
        {
           byte b = (byte)KeyInterop.VirtualKeyFromKey(key);
           Input.SendKeyboardInput(b, isDown);
        }
    }        
}

