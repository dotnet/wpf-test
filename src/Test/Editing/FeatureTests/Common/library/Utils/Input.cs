// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Keyboard and mouse emulation services for test cases.

#region Namespaces.

using System;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

using Win32 = Test.Uis.Wrappers.Win32;
using Test.Uis.Loggers;
using Microsoft.Test.Imaging;
using System.Text;
using Microsoft.Win32;
using System.Xml.Serialization;
using System.Collections.Generic;

#endregion Namespaces.

namespace Test.Uis.Utils
{
    /// <summary>Provides keyboard input emulation.</summary>
    /// <remarks>
    /// <p>
    /// Note that all mouse input emulation is sent to the
    /// system input queue. It is not processed immediately. Therefore,
    /// test cases are advised to queue an item with low priority
    /// to continue processing after input events are processed.
    /// </p>
    /// </remarks>
    /// <example>The following sample uses this class and also shows
    /// how to explicitly press a key without releasing it using
    /// the Automation Input class.<code>
    /// private void MyMethod() {
    ///   // Types a string.
    ///   KeyboardInput.TypeString("Hello, world!");
    ///   // Selects to the beginning.
    ///   KeyboardInput.TypeShift(System.Windows.Automation.VKeys.VkHome);
    ///   // Sets to bold.
    ///   KeyboardInput.TypeControlChar('b');
    ///   // Press the Control key while we type two letters.
    ///   System.Windows.Automation.Input.SendKeyboardInput(
    ///     System.Windows.Automation.VKeys.VkControl, true);
    ///   KeyboardInput.TypeString("ab");
    ///   System.Windows.Automation.Input.SendKeyboardInput(
    ///     System.Windows.Automation.VKeys.VkControl, false);
    /// }</code></example>
    [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
    public static class KeyboardInput
    {
        [DllImport("user32.dll", EntryPoint = "SendInput")]
        private static extern uint SendKeyboardInput(uint nInputs,
            InputGenerator.KeyboardInput[] pInputs, int cbSize);


        #region Public Methods

        /// <summary>
        /// Adds the input locale identifier (formerly called the keyboard
        /// layout handle) for the calling thread.
        /// </summary>
        /// <param name="inputLocale">
        /// A string composed of the hexadecimal value of the Language
        /// Identifier (low word) and a device identifier (high word)
        /// (the composition is known as input locale identifier).
        /// </param>
        /// <example>The following code shows how to use this method.<code>...
        /// // Set input locale to: Arabic (Saudi Arabia) - Arabic (101)
        /// KeyboardInput.AddInputLocale("00000401");
        /// // Set input locale to: English (United States) - US
        /// KeyboardInput.AddInputLocale("00000409");
        /// // Set input locale to: Hebrew - Hebrew
        /// KeyboardInput.AddInputLocale("0000040d");
        /// // Set input locale to: Japanese - Japanese Input System (MS-IME2002)
        /// KeyboardInput.AddInputLocale("e0010411");
        /// // Set input locale to: Spanish (Argentina) - Latin American
        /// KeyboardInput.AddInputLocale("00002c0a");
        /// </code></example>
        public static void AddInputLocale(string inputLocale)
        {
            IntPtr hkl;     // New layout handle.
            //IntPtr hklOld;  // Old layout handle.

            if (inputLocale == null)
            {
                throw new ArgumentNullException("inputLocale");
            }
            if (inputLocale.Length != 8)
            {
                throw new ArgumentException(
                    "Input locale values should be 8 hex digits.", "inputLocale");
            }

            // Attempt to load the keyboard layout, but do not use
            // the return value. For some reason, this is always
            // returning the English keyboard. 
            hkl = Win32.SafeLoadKeyboardLayout(inputLocale, 0);
            if (hkl == IntPtr.Zero)
            {
                throw new Exception("Unable to add input locale " + inputLocale);
            }
        }

        /// <summary>ResetCapsLock.</summary>
        public static void ResetCapsLock()
        {
            if (System.Windows.Forms.Control.IsKeyLocked(System.Windows.Forms.Keys.CapsLock))
            {
                Microsoft.Test.Input.Input.SendKeyboardInput(System.Windows.Input.Key.CapsLock, true);
                Microsoft.Test.Input.Input.SendKeyboardInput(System.Windows.Input.Key.CapsLock, false);
            }
        }

        /// <summary>Enables IME.</summary>
        public static void EnableIME(UIElement element)
        {
            //This toggle is maintained so that if this function is called multiple times,
            //we retain the original state before the test was run
            if (IsImeFunctionCalled == false)
            {
                IsImeFunctionCalled = true;
                ConfigurationSettings.Current.ImeState = System.Windows.Input.InputMethod.Current.ImeState;
            }
            System.Windows.Input.InputMethod.SetPreferredImeState(element, System.Windows.Input.InputMethodState.On);
        }

        /// <summary>
        /// Retrieves the active input locale identifier (formerly
        /// called the keyboard layout) for the current thread.
        /// </summary>
        /// <returns>The active input locale identifier for the current thread.</returns>
        public static string GetActiveInputLocaleString()
        {
            IntPtr currentLocale;   // Current locale handle.

            currentLocale = Win32.SafeGetKeyboardLayout(IntPtr.Zero);
            return ((uint)(currentLocale)).ToString("x8", CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///  Check to see if a system have BIDI languages installed.
        /// </summary>
        /// <returns></returns>
        public static bool IsBidiInputLanguageInstalled()
        {
            bool bidiInputLanguageInstalled;

            bidiInputLanguageInstalled = false;

            int keyboardListCount = Test.Uis.Wrappers.Win32.SafeGetKeyboardLayoutList(0, null);
            if (keyboardListCount > 0)
            {
                int keyboardListIndex;
                IntPtr[] keyboardList;

                keyboardList = new IntPtr[keyboardListCount];

                keyboardListCount = Test.Uis.Wrappers.Win32.SafeGetKeyboardLayoutList(keyboardListCount, keyboardList);

                for (keyboardListIndex = 0;
                     (keyboardListIndex < keyboardList.Length) && (keyboardListIndex < keyboardListCount);
                     keyboardListIndex++)
                {
                    CultureInfo cultureInfo = new CultureInfo((short)keyboardList[keyboardListIndex]);
                    if (IsBidiInputLanguage(cultureInfo))
                    {
                        bidiInputLanguageInstalled = true;
                        break;
                    }
                }
            }

            return bidiInputLanguageInstalled;
        }

        /// <summary>
        ///  Check to see if a system have BIDI languages installed.
        /// </summary>
        /// <returns></returns>
        public static bool IsInputLanguageInstalled(string _ThreeLetterWindowsLanguageName)
        {
            bool InputLanguageInstalled;

            InputLanguageInstalled = false;

            int keyboardListCount = Test.Uis.Wrappers.Win32.SafeGetKeyboardLayoutList(0, null);
            if (keyboardListCount > 0)
            {
                int keyboardListIndex;
                IntPtr[] keyboardList;

                keyboardList = new IntPtr[keyboardListCount];

                keyboardListCount = Test.Uis.Wrappers.Win32.SafeGetKeyboardLayoutList(keyboardListCount, keyboardList);

                for (keyboardListIndex = 0;
                     (keyboardListIndex < keyboardList.Length) && (keyboardListIndex < keyboardListCount);
                     keyboardListIndex++)
                {
                    CultureInfo cultureInfo = new CultureInfo((short)keyboardList[keyboardListIndex]);
                    if (cultureInfo.ThreeLetterWindowsLanguageName.ToLower() == _ThreeLetterWindowsLanguageName.ToLower())
                    {
                        InputLanguageInstalled = true;
                        break;
                    }
                }
            }

            return InputLanguageInstalled;
        }

        /// <summary>Presses a virtual key.</summary>
        /// <param name="vkbyte">VKey of the key</param>
        public static void PressVirtualKey(byte vkbyte)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            InputGenerator.PressVirtualKey(vkbyte);
        }

        /// <summary>Releases a virtual key.</summary>
        /// <param name="vkbyte">VKey of the key</param>
        public static void ReleaseVirtualKey(byte vkbyte)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            InputGenerator.ReleaseVirtualKey(vkbyte);
        }

        /// <summary>
        /// In case control, shift or alt key is pressed when exception happens,
        /// these keys will be held even if this process exits unless
        /// they are released.
        /// Need to do clean up to avoid this affecting next test
        /// </summary>
        public static void ResetKeyboardState()
        {
            byte[] byteKeyboardState;

            byteKeyboardState = new byte[256];
            if (!Win32.SafeGetKeyboardState(byteKeyboardState))
            {
                return;
            }
            byteKeyboardState[Win32.VK_SHIFT] &= 0x0F;
            byteKeyboardState[Win32.VK_MENU] &= 0x0F;
            byteKeyboardState[Win32.VK_CONTROL] &= 0x0F;
            Win32.SafeSetKeyboardState(byteKeyboardState);
        }

        /// <summary>
        /// Hold / release one key.
        /// This method takes keystrokeDescription string (e.g. "A" or "{BACK}")
        /// and this string can only hold *one* key. If the keystrokeDescription
        /// contain more than 1 key, it throws exception. Also, this method doesn't
        /// do key combination (e.g. +{BACK}) This method assumes that keystrokeDescription
        /// is interpreted as US English HKL
        /// </summary>
        /// <param name="keystrokeDescription">keystroke description string</param>
        /// <param name="pressed">hole key = true, false otherwise</param>
        public static void PressOrReleaseOneKey(string keystrokeDescription, bool pressed)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();

            InputGenerator.PressOrReleaseOneKey(keystrokeDescription, (IntPtr)0x04090409, pressed);
        }

        /// <summary>
        /// Sets the input locale identifier (formerly called the keyboard
        /// layout handle) for the calling thread.
        /// </summary>
        /// <param name="inputLocale">
        /// A string composed of the hexadecimal value of the Language
        /// Identifier (low word) and a device identifier (high word)
        /// (the composition is known as input locale identifier).
        /// </param>
        /// <example>The following code shows how to use this method.<code>...
        /// // Set input locale to: Arabic (Saudi Arabia) - Arabic (101)
        /// KeyboardInput.SetActiveInputLocale("00000401");
        /// // Set input locale to: English (United States) - US
        /// KeyboardInput.SetActiveInputLocale("00000409");
        /// // Set input locale to: Hebrew - Hebrew
        /// KeyboardInput.SetActiveInputLocale("0000040d");
        /// // Set input locale to: Japanese - Japanese Input System (MS-IME2002)
        /// KeyboardInput.SetActiveInputLocale("e0010411");
        /// // Set input locale to: Spanish (Argentina) - Latin American
        /// KeyboardInput.SetActiveInputLocale("00002c0a");
        /// </code></example>
        public static void SetActiveInputLocale(string inputLocale)
        {
            IntPtr hkl;     // New layout handle.
            IntPtr hklOld;  // Old layout handle.

            if (inputLocale == null)
            {
                throw new ArgumentNullException("inputLocale");
            }
            if (inputLocale.Length != 8)
            {
                throw new ArgumentException(
                    "Input locale values should be 8 hex digits.", "inputLocale");
            }

            // Attempt to load the keyboard layout, but do not use
            // the return value. For some reason, this is always
            // returning the English keyboard. 
            hkl = Win32.SafeLoadKeyboardLayout(inputLocale, 0);
            Win32.SafeActivateKeyboardLayout(hkl, 0);
            if (hkl == IntPtr.Zero)
            {
                throw new Exception("Unable to set input locale " + inputLocale);
            }

            hkl = (IntPtr)(uint.Parse(inputLocale, NumberStyles.HexNumber, CultureInfo.InvariantCulture));
            hklOld = Win32.SafeActivateKeyboardLayout(hkl, 0);
            if (hklOld == IntPtr.Zero)
            {
                throw new Exception("Locale " + inputLocale + " loaded, " +
                    "but activation failed.");
            }
            if (hkl == hklOld)
            {
                System.Diagnostics.Debug.WriteLine("WARNING: changing keyboard layout " +
                    "to " + inputLocale + " keeps the same layout " +
                    "(input locale " + ((uint)hkl).ToString("x8") + ")");
            }
        }

        /// <summary>
        /// Emulates typing a string in US English locale, which is common in
        /// testing.
        /// </summary>
        /// <param name="s">String to type.</param>
        /// <remarks>
        /// Note that the mapping from a character to a key might
        /// need multiple keystrokes and may depend on the current language.
        /// </remarks>
        public static void TypeString(string s)
        {
            if (Test.Uis.Threading.TestQueue.IsThreadApplicationCurrent)
            {
                DispatcherOperationCallback callback;

                callback = DelayedTypeString;
                Test.Uis.Threading.TestQueue.ThreadQueue
                    .AddInput(callback, new object[] { s });
            }
            else
            {
                DelayedTypeString(s);
            }
        }

        /// <summary>
        /// Type string using keyboard. 
        /// </summary>
        /// <param name="text"></param>
        public static void NoDelaySendString(string text)
        {
            IntPtr hkl;

            hkl = Test.Uis.Wrappers.Win32.SafeLoadKeyboardLayout("04090409", 0);
            InputGenerator.KeyboardInput[] inputs = SendKeysParser.ConvertLiteralStringToKeyCodeStructures(text, hkl);

            unsafe
            {
                SendKeyboardInput((uint)inputs.Length, inputs, sizeof(InputGenerator.KeyboardInput));
            }
        }

        #endregion Public Methods


        #region Private Methods

        /// <summary>
        /// Emulates typing a string in US English locale, which is common in
        /// testing.
        /// </summary>
        /// <param name="s">String to type.</param>
        /// <remarks>
        /// This is invoked by the TestQueue with a delay to account for
        /// Cicero components.
        /// </remarks>
        private static object DelayedTypeString(object s)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            InputGenerator.SendString(s.ToString(), "04090409");
            return null;
        }

        /// <summary>
        /// Check to see if a language supports BIDI
        /// </summary>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        private static bool IsBidiInputLanguage(CultureInfo cultureInfo)
        {
            bool bidiInput;
            string fontSignature;

            bidiInput = false;

            //FONTSIGNATURE_SIZE = 16
            fontSignature = new String(new Char[16]);

            // Get the font signature to know the current LCID is BiDi(Arabic, Hebrew etc.) or not.
            //LOCALE_FONTSIGNATURE = 0x00000058
            if (Test.Uis.Wrappers.Win32.GetLocaleInfoW(cultureInfo.LCID, 0x00000058, fontSignature, 16) != 0)
            {
                // Compare fontSignature[7] with 0x0800 to detect BiDi language.
                if ((fontSignature[7] & 0x0800) != 0)
                {
                    bidiInput = true;
                }
            }

            return bidiInput;
        }

        /// <summary>Logs a formatted string if the service is available.</summary>
        private static void Log(string format, params object[] args)
        {
            Test.Uis.Loggers.Logger.Current.Log(format, args);
        }

        #endregion Private Methods


        public static bool IsImeFunctionCalled = false;
    }

    /// <summary>Provides mouse input emulation.</summary>
    /// <remarks>Note that all mouse input emulation is sent to the
    /// system input queue. It is not processed immediately. Therefore,
    /// test cases are advised to queue an item with low priority
    /// to continue processing after input events are processed.</remarks>
    /// <example>The following sample shows how to use some of the
    /// methods in this class.<code>...
    /// private void MyMethod() {
    ///   // Move the mouse.
    ///   MouseInput.MouseMove(0, 0);
    ///   // Press the button.
    ///   MouseInput.MouseDown();
    ///   // Drag to another place.
    ///   MouseInput.MouseDrag(0, 0, 100, 100);
    ///   // Release mouse.
    ///   MouseInput.MouseUp();
    /// }
    /// </code></example>
    [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
    public static class MouseInput
    {

        #region Public Methods

        /// <summary>
        /// Moves the mouse pointer to the x and y coordinates of the screen.
        /// </summary>
        /// <param name="x">x coordinate of pointer relative to desktop origin</param>
        /// <param name="y">y coordinate of pointer relative to desktop origin</param>
        public static void MouseMove(int x, int y)
        {
            Log("Moving mouse to ({0};{1})", x.ToString(), y.ToString());
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            InputGenerator.SendMouseMoveAbsolute(x, y);

        }

        /// <summary>
        /// Moves the mouse pointer to the specified point on the screen.
        /// </summary>
        /// <param name="point">Coordinate relative to desktop origin.</param>
        public static void MouseMove(System.Windows.Point point)
        {
            MouseMove((int)Math.Round(point.X, 0), (int)Math.Round(point.Y, 0));
        }

        /// <summary>Centers the mouse pointer on an element.</summary>
        /// <param name="element">Element to center the pointer on.</param>
        public static void MouseMove(UIElement element)
        {
            Rect r = ElementUtils.GetScreenRelativeRect(element);
            int x = (int)Math.Round(r.Left + r.Width / 2, 0);
            int y = (int)Math.Round(r.Top + r.Height / 2, 0);
            MouseMove(x, y);
        }

        /// <summary>Left clicks the mouse in its current position.</summary>
        public static void MouseClick()
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            InputGenerator.SendMouseButton(true, false, false, true);
            InputGenerator.SendMouseButton(true, false, false, false);
        }

        /// <summary>
        /// Moves the mouse pointer to the x and y coordinates of the screen
        /// and left clicks.
        /// </summary>
        /// <param name="x">x coordinate of pointer relative to desktop origin</param>
        /// <param name="y">y coordinate of pointer relative to desktop origin</param>
        public static void MouseClick(int x, int y)
        {
            Log("Clicking on (" + x.ToString() + ";" + y.ToString() + ")");
            MouseMove(x, y);
            MouseClick();
        }

        /// <summary>
        /// Moves the mouse pointer to the p point of the screen
        /// and left clicks.
        /// </summary>
        /// <param name="p">Point to click on relative to desktop origin.</param>
        public static void MouseClick(System.Windows.Point p)
        {
            MouseClick((int)p.X, (int)p.Y);
        }

        /// <summary>
        /// Moves the mouse pointer over the element and left clicks.
        /// </summary>
        /// <param name="element">Element to click.</param>
        public static void MouseClick(UIElement element)
        {
            Rect r = ElementUtils.GetScreenRelativeRect(element);
            int x = (int)(r.Left + r.Width / 2);
            int y = (int)(r.Top + r.Height / 2);
            MouseClick(x, y);
        }


        /// <summary>Presses the left mouse button.</summary>
        /// <remarks>Note also that an input item is queued; when the method
        /// returns, the button will not have been pressed yet.</remarks>
        public static void MouseDown()
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            InputGenerator.SendMouseButton(true, false, false, true);
        }

        /// <summary>Presses the left mouse button on a screen point.</summary>
        /// <param name="x">x coordinate of pointer relative to desktop origin</param>
        /// <param name="y">y coordinate of pointer relative to desktop origin</param>
        public static void MouseDown(int x, int y)
        {
            MouseMove(x, y);
            MouseDown();
        }

        /// <summary>Presses the left mouse button on a System.Windows.Point.</summary>
        /// <param name="p">Point to click on relative to desktop origin.</param>
        public static void MouseDown(System.Windows.Point p)
        {
            MouseMove(p);
            MouseDown();
        }

        /// <summary>Moves the mouse pointer in multiple increments.</summary>
        /// <param name="x">x coordinate of pointer relative to desktop origin
        ///   to start from</param>
        /// <param name="y">y coordinate of pointer relative to desktop origin
        ///   to start from</param>
        /// <param name="xDest">x coordinate of pointer relative to desktop origin
        ///   to move to </param>
        /// <param name="yDest">y coordinate of pointer relative to desktop origin
        ///   to move to</param>
        /// <remarks>Note that this operation does not actually hold the button
        /// pressed. To achieve this effect, call MouseDown before and MouseUp
        /// after. This design allows test cases to move to multiple destinations
        /// before releasing the mouse button.</remarks>
        public static void MouseDrag(int x, int y, int xDest, int yDest)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            int stepCount = InputGenerator.GenerateMouseDrag(x, y, xDest, yDest);
            Log(String.Format("MouseInput: Moving from ({0};{1}) to ({2};{3}) in {4} steps.",
                x, y, xDest, yDest, stepCount));
        }

        /// <summary>Moves the mouse pointer in multiple increments.</summary>
        /// <param name="pointStart">starting point of MouseDrag</param>
        /// <param name="pointEnd">ending point of MouseDrag</param>
        public static void MouseDrag(System.Windows.Point pointStart, System.Windows.Point pointEnd)
        {
            MouseDrag((int)Math.Round(pointStart.X, 0), (int)Math.Round(pointStart.Y, 0), (int)Math.Round(pointEnd.X, 0), (int)Math.Round(pointEnd.Y, 0));
        }

        /// <summary>
        /// Performs a mouse drag operation in a different thread.
        /// </summary>
        /// <param name="start">Start point in screen coordinates for drag operation.</param>
        /// <param name="end">End point in screen coordinates for drag operation.</param>
        /// <param name="pressed">Whether the left button should be pressed and then released.</param>
        /// <param name="delayBeforeRun">A delay before the other thread starts running.</param>
        /// <param name="callback">Callback after the operation is finished.</param>
        /// <param name="dispatcher">Dispatcher in which to queue callback (null to call from other thread).</param>
        public static void MouseDragInOtherThread(System.Windows.Point start,
            System.Windows.Point end, bool pressed, TimeSpan delayBeforeRun,
            SimpleHandler callback, Dispatcher dispatcher)
        {
            MouseDragInOtherThread((int)start.X, (int)start.Y, (int)end.X, (int)end.Y, pressed,
                delayBeforeRun, callback, dispatcher);
        }

        /// <summary>
        /// Performs a mouse drag operation in a different thread.
        /// </summary>
        /// <param name="startX">Horizontal offset for start point in screen coordinates.</param>
        /// <param name="startY">Vertical offset for start point in screen coordinates.</param>
        /// <param name="endX">Horizontal offset for end point in screen coordinates.</param>
        /// <param name="endY">Vertical offset for end point in screen coordinates.</param>
        /// <param name="pressed">Whether the left button should be pressed and then released.</param>
        /// <param name="delayBeforeRun">A delay before the other thread starts running.</param>
        /// <param name="callback">Callback (on a different thread) after the operation is finished.</param>
        /// <param name="dispatcher">Dispatcher in which to queue callback (null to call from other thread).</param>
        public static void MouseDragInOtherThread(int startX, int startY, int endX, int endY,
            bool pressed, TimeSpan delayBeforeRun, SimpleHandler callback, Dispatcher dispatcher)
        {
            System.Threading.Thread otherThread;

            otherThread = new System.Threading.Thread(delegate()
            {
                System.Threading.Thread.Sleep(delayBeforeRun);
                InputMonitorManager.Current.IsEnabled = false;
                if (pressed)
                {
                    MouseInput.MouseMove(startX, startY);
                    MouseInput.MouseDown();
                }
                MouseInput.MouseDrag(startX, startY, endX, endY);
                if (pressed)
                {
                    MouseInput.MouseUp();
                }
                if (callback != null)
                {
                    if (dispatcher == null)
                    {
                        callback();
                    }
                    else
                    {
                        dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, callback);
                    }
                }
            });
            otherThread.Start();
        }

        /// <summary>Presses the left mouse button on the starting point, moves
        /// the mouse cursor to the ending point and then releases the button.</summary>
        /// <param name="ps">Starting point.</param>
        /// <param name="pe">Ending point.</param>
        public static void MouseDragPressed(System.Windows.Point ps, System.Windows.Point pe)
        {
            MouseDown(ps);
            MouseMove(pe);
            MouseUp();
        }

        /// <summary>Releases the left mouse button.</summary>
        public static void MouseUp()
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            InputGenerator.SendMouseButton(true, false, false, false);
        }

        /// <summary>
        /// Moves the mouse pointer over the element and right clicks.
        /// </summary>
        /// <param name="element">Element to click.</param>
        public static void RightMouseClick(UIElement element)
        {
            Rect r = ElementUtils.GetScreenRelativeRect(element);
            int x = (int)Math.Round(r.Left + r.Width / 2, 0);
            int y = (int)Math.Round(r.Top + r.Height / 2, 0);
            RightMouseDown(x, y);
            RightMouseUp();
        }

        /// <summary>Presses the right mouse button.</summary>
        public static void RightMouseDown()
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            InputGenerator.SendMouseButton(false, false, true, true);
        }

        /// <summary>Presses the right mouse button on a screen point.</summary>
        /// <param name="x">x coordinate of pointer relative to desktop origin</param>
        /// <param name="y">y coordinate of pointer relative to desktop origin</param>
        public static void RightMouseDown(int x, int y)
        {
            MouseMove(x, y);
            RightMouseDown();
        }

        /// <summary>Presses the right mouse button on a System.Windows.Point</summary>
        /// <param name="p">Point to click on relative to desktop origin.</param>
        public static void RightMouseDown(System.Windows.Point p)
        {
            MouseMove(p);
            RightMouseDown();
        }

        /// <summary>Releases the right mouse button.</summary>
        public static void RightMouseUp()
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            InputGenerator.SendMouseButton(false, false, true, false);
        }

        /// <summary>
        /// Moves the mouse wheel specified number of clicks.
        /// negative value - moves the wheel towards the user - down
        /// postive value - moves the wheel away from the user - up        
        /// </summary>
        /// <param name="clicks">Number of clicks to move wheel.</param>
        public static void MouseWheel(int clicks)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            InputGenerator.SendMouseMoveWheel(clicks);
        }

        #endregion Public Methods


        #region Private Methods

        private static void Log(string format, params object[] args)
        {
            Test.Uis.Loggers.Logger.Current.Log(format, args);
        }

        #endregion Private Methods
    }

    /// <summary>Provides functions for loading and unloading keyboard layouts.</summary>
    /// <remarks>This is more of a reference and current code for loading and unloading keyboard layouts is sufficient.</remarks>
    [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
    public static class IMEInput
    {

        #region bool InstallOrUninstallLayoutOrTip(string layoutOrTip, bool uninstall)
        // String format of layoutOrTip:
        //
        // Down-level: LLLL:KKKKKKKK
        //
        //   LLLL - Language ID
        //   KKKKKKKK - Keyboard Layout ID
        //   (see HKLM\SYSTEM\CurrentControlSet\Control\Keyboard Layouts)
        //
        //   Examples:
        //     "0407:00000407" - German (Germany) - German keyboard
        //     "0804:e00e0804" - Chinese (Simplified) - Microsoft Pinyin IME 3.0
        //
        // Vista: LLLL:KKKKKKKK;LLLL:{CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC}{PPPPPPPP-PPPP-PPPP-PPPP-PPPPPPPPPPPP};...
        //
        //   {CCC...C} - Class ID of Text Service
        //   {PPP...P} - Profile GUID of Text Service
        //   (see HKLM\SOFTWARE\Microsoft\CTF\TIP)
        //
        //   Examples:
        //     "0804:00000804;0804:{81D4E9C9-1D3B-41BC-9E6C-4B40BF79E35E}{F3BA9077-6C7E-11D4-97FA-0080C882687E}"
        //     will add (or remove),
        //     Chinese (PRC) - Chinese (Simplified) - US keyboard and
        //     Chinese (PRC) - Microsoft Pinyin IME
        //
        /// <summary>
        /// Installs the keyboard layout correponding to the layout tip
        /// This is similar to the loadinputloacale but is included in the library for reference
        /// language ID/tip can be obtained from the other public function in this class
        /// </summary>
        /// <param name="layoutOrTip">language id to be installed</param>
        /// <param name="uninstall">install.uninstall parameter</param>
        public static bool InstallOrUninstallLayoutOrTip(string layoutOrTip, bool uninstall)
        {
            if (String.IsNullOrEmpty(layoutOrTip))
                return false;

            if (Environment.OSVersion.Version.Major < 6)
            {
                // Use input!InstallInputLayout (@102) and 
                // UnInstallInputLayout (@103) on down-level
                // (XP and WS03)
                //

                int colon = layoutOrTip.IndexOf(":");
                ushort lcid = ushort.Parse(layoutOrTip.Substring(0, colon), NumberStyles.HexNumber);
                uint dwLayout = uint.Parse(layoutOrTip.Substring(colon + 1), NumberStyles.HexNumber);
                bool bDefLayout = false;
                IntPtr hklDefault = IntPtr.Zero;
                bool bDefUser = false;
                bool bSysLocale = false;

                if (uninstall)
                {
                    UnInstallInputLayout(lcid, dwLayout, bDefUser);
                    // input!UnInstallInputLayout may not have this layout in
                    // its static locale info. Need to call UnloadKeyboardLayout
                    // in this case.
                    //
                    UnloadInputLayout(lcid, dwLayout);
                }
                else
                {
                    InstallInputLayout(lcid, dwLayout, bDefLayout, hklDefault, bDefUser, bSysLocale);
                }
            }
            else
            {
                // Use input!InstallLayoutOrTip on Vista
                //
                if (QueryLayoutOrTipString(layoutOrTip, 0) != IntPtr.Zero)
                    return false;

                uint dwFlags = uninstall ? ILOT_UNINSTALL : 0;
                InstallLayoutOrTip(layoutOrTip, dwFlags);
            }
            return true;
        }

        private static bool UnloadInputLayout(ushort lcid, uint dwLayout)
        {
            uint dwHkl = 0;

            if ((dwLayout & 0xf0000000) == 0xe0000000)
            {
                // IMM32's IME layout is HKL value.
                //
                dwHkl = dwLayout;
            }
            else
            {
                // Create hKL value with/without Layout ID.
                //
                string key = String.Format(@"SYSTEM\CurrentControlSet\Control\Keyboard Layouts\{0:x8}", dwLayout);
                using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(key))
                {
                    if (regKey != null)
                    {
                        string layoutId = regKey.GetValue("Layout Id") as string;
                        if (String.IsNullOrEmpty(layoutId))
                        {
                            dwHkl = (dwLayout << 16) + lcid;
                        }
                        else
                        {
                            uint dwLayoutId = uint.Parse(layoutId, NumberStyles.HexNumber);
                            dwHkl = 0xf0000000 + (dwLayoutId << 16) + lcid;
                        }
                    }
                }
            }
            if (dwHkl == 0)
                return false;

            return UnloadKeyboardLayout(new IntPtr((int)dwHkl));
        }

        // extern "C" BOOL WINAPI InstallInputLayout(LCID lcid, DWORD dwLayout, BOOL bDefLayout, HKL hklDefault,
        //      BOOL bDefUser, BOOL bSysLocale);
        [DllImport("input.dll", EntryPoint = "#102")]
        private static extern bool InstallInputLayout(ushort lcid, uint dwLayout, bool bDefLayout, IntPtr hklDefault,
            bool bDefUser, bool bSysLocale);

        // extern "C" BOOL WINAPI UnInstallInputLayout(LCID lcid, DWORD dwLayout, BOOL bDefUser);
        [DllImport("input.dll", EntryPoint = "#103")]
        private static extern bool UnInstallInputLayout(ushort lcid, uint dwLayout, bool bDefUser);

        // extern "C" HRESULT WINAPI QueryLayoutOrTipString(_In_ LPCWSTR psz, DWORD dwFlags);
        [DllImport("input.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr QueryLayoutOrTipString(string psz, uint dwFlags);

        // extern "C" BOOL WINAPI InstallLayoutOrTip(_In_ LPCWSTR psz, DWORD dwFlags);
        [DllImport("input.dll", CharSet = CharSet.Unicode)]
        private static extern bool InstallLayoutOrTip(string psz, uint dwFlags);

        // extern "C" BOOL WINAPI SetDefaultLayoutOrTip(_In_ LPCWSTR psz, DWORD dwFlags);
        [DllImport("input.dll", CharSet = CharSet.Unicode)]
        private static extern bool SetDefaultLayoutOrTip(string psz, uint dwFlags);

        // BOOL UnloadKeyboardLayout(HKL hkl);
        [DllImport("user32.dll")]
        private static extern bool UnloadKeyboardLayout(IntPtr hkl);

        private const uint ILOT_UNINSTALL = 0x00000001;

        #endregion // InstallOrUninstallLayoutOrTip

        #region string[] GetLayoutOrTipForLocale(uint locale)

        // This function returns a list of layout or TIP for specified locale.
        // (Keyboards or TIPs added by the system when user or system locale is changed.)
        //
        // This list is defined under [Locales] section in intl.inf on down-level (XP and WS03)
        //
        // On Vista, NLS API GetLocaleInfo(LOCALE_SKEYBOARDSTOINSTALL) provides this information.
        //
        //   Examples:
        //     locale = 0x00000804
        //
        //     Return value on down-level:
        //       { "0804:00000804", "0804:e00e0804", "0804:e0010804", "0804:e0030804", "0804:e0040804" }
        //
        //     Return value on Vista:
        //       { "0804:00000804;0804:{81D4E9C9-1D3B-41BC-9E6C-4B40BF79E35E}{F3BA9077-6C7E-11D4-97FA-0080C882687E}" }
        //
        /// <summary>
        /// Gets the language id corresponding to locale id
        /// </summary>
        /// <param name="locale">locale id</param>
        public static string[] GetLayoutOrTipForLocale(uint locale)
        {
            string[] layoutOrTip = new string[] { null };

            if (Environment.OSVersion.Version.Major < 6)
            {
                // Get a list of language:layout ID pair
                // for the specified locale from [Locales]
                // section in intl.inf on down-level
                // (XP and WS03)
                //
                // XPSP2 intl.inf sample:
                // [Locales]
                // 00000409 = %English_United_States%      ,437     ,1,,0409:00000409
                // 00000407 = %German_Standard%            ,850     ,1,,0407:00000407,0409:00000409
                // 00000404 = %Chinese_Taiwan%             ,950     ,9,,0404:00000404,0404:e0080404,0404:E0010404
                //
                IntPtr hInf = SetupOpenInfFile("intl.inf", null, INF_STYLE_WIN4, IntPtr.Zero);
                if (hInf != s_INVALID_HANDLE_VALUE)
                {
                    string key = String.Format("{0:x8}", locale);
                    INFCONTEXT context = new INFCONTEXT();
                    if (SetupFindFirstLine(hInf, "Locales", key, context))
                    {
                        ArrayList list = new ArrayList();
                        StringBuilder layout = new StringBuilder(14);

                        for (uint i = 5; SetupGetStringField(context, i, layout, layout.Capacity, IntPtr.Zero); i++)
                        {
                            list.Add(layout.ToString());
                        }
                        if (list.Count > 0)
                        {
                            layoutOrTip = (string[])list.ToArray(typeof(string));
                        }
                    }
                    SetupCloseInfFile(hInf);
                }
            }
            else
            {
                // Use kernel32!GetLocaleInfo(LOCALE_SKEYBOARDSTOINSTALL) on Vista
                //
                int cchData = GetLocaleInfo(locale, LOCALE_SKEYBOARDSTOINSTALL, IntPtr.Zero, 0);
                if (cchData > 0)
                {
                    StringBuilder lpLCData = new StringBuilder(cchData);
                    if (GetLocaleInfo(locale, LOCALE_SKEYBOARDSTOINSTALL, lpLCData, cchData) == cchData)
                    {
                        layoutOrTip[0] = lpLCData.ToString();
                    }
                }
            }
            return layoutOrTip;
        }



        // HINF SetupOpenInfFile(PCTSTR FileName, PCTSTR InfClass, DWORD InfStyle,
        //      PUINT ErrorLine);
        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetupOpenInfFile(string FileName, string InfClass, uint InfStyle,
            IntPtr pErrorLine);

        // void SetupCloseInfFile(HINF InfHandle);
        [DllImport("setupapi.dll")]
        private static extern void SetupCloseInfFile(IntPtr InfHandle);

        // BOOL SetupFindFirstLine(HINF InfHandle, PCTSTR Section, PCTSTR Key, PINFCONTEXT Context);
        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        private static extern bool SetupFindFirstLine(IntPtr InfHandle, string Section, string Key,
            [In, Out, MarshalAs(UnmanagedType.LPStruct)] INFCONTEXT Context);

        // BOOL SetupGetStringField(PINFCONTEXT Context,
        //      DWORD FieldIndex, PTSTR ReturnBuffer, DWORD ReturnBufferSize, PDWORD RequiredSize);
        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        private static extern bool SetupGetStringField([In, MarshalAs(UnmanagedType.LPStruct)] INFCONTEXT Context,
            uint FieldIndex, StringBuilder ReturnBuffer, int ReturnBufferSize, IntPtr RequiredSize);

        private const uint INF_STYLE_WIN4 = 0x00000002;
        private static readonly IntPtr s_INVALID_HANDLE_VALUE = new IntPtr(-1);

        [StructLayout(LayoutKind.Sequential)]
        private class INFCONTEXT
        {
            public IntPtr /*PVOID*/ Inf;
            public IntPtr /*PVOID*/ CurrentInf;
            public uint /*UINT*/ Section;
            public uint /*UINT*/ Line;
        }

        // int GetLocaleInfo(LCID Locale, LCTYPE LCType, LPTSTR lpLCData, int cchData);
        [DllImport("kernel32.dll")]
        private static extern int GetLocaleInfo(uint Locale, uint LCType, IntPtr lpLCData, int cchData);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetLocaleInfo(uint Locale, uint LCType, StringBuilder lpLCData, int cchData);

        private const uint LOCALE_SKEYBOARDSTOINSTALL = 0x0000005e;

        #endregion // GetLayoutOrTipForLocale

        #region Console Application
        /*
        
        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                new InputDll().Run(args);
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}", e);
            }
        }

        public void Run(string[] args)
        {
            bool uninstall = false;

            foreach (string arg in args)
            {
                if (arg.StartsWith("-") || arg.StartsWith("/"))
                {
                    if (arg.ToLowerInvariant().Substring(1) == "u")
                    {
                        uninstall = true;
                    }
                    else
                    {
                        Console.WriteLine("Adds or removes keyboard/IME/Text Service.\r\n");
                        Console.WriteLine("InputDll.exe [-U] langid | langid:layout | langid:{clsid}{profile}\r\n");
                        Console.WriteLine("  langid  Specifies input language.");
                        Console.WriteLine("  layout  Specifies keyboard layout ID.");
                        Console.WriteLine("  clsid   Specifies class ID of Text Service.");
                        Console.WriteLine("  profile Specifies profile GUID of Text Service.");
                        Console.WriteLine("  -U      Removes specified layout or Text Service.\r\n");
                        Console.WriteLine("Examples:");
                        Console.WriteLine("  InputDll.exe 0411");
                        Console.WriteLine("  InputDll.exe -U 0407:00000407");
                        Console.WriteLine("(Vista only)");
                        Console.WriteLine("  InputDll.exe 0804:{81D4E9C9-1D3B-41BC-9E6C-4B40BF79E35E}" +
                            "{F3BA9077-6C7E-11D4-97FA-0080C882687E}");
                        break;
                    }
                }
                else if (arg.IndexOf(":") > 0)
                {
                    if (!InstallOrUninstallLayoutOrTip(arg, uninstall))
                    {
                        Console.WriteLine("ERROR: {0}", arg);
                    }
                }
                else
                {
                    uint locale = uint.Parse(arg, NumberStyles.HexNumber);
                    foreach (string layoutOrTip in GetLayoutOrTipForLocale(locale))
                    {
                        if (!InstallOrUninstallLayoutOrTip(layoutOrTip, uninstall))
                        {
                            Console.WriteLine("ERROR: {0}", layoutOrTip);
                        }
                    }
                }
            }
        }
        */
        #endregion Console Application

    }

    /// <summary>Provides functions for loading and unloading keyboard layouts.</summary>
    [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
    public static class KeyboardLayoutHelper
    {
        #region bool InstallUninstallLayout

        #region Public Members

        // String format of layoutOrTip:
        //
        // Down-level: LLLL:KKKKKKKK
        //
        //   LLLL - Language ID
        //   KKKKKKKK - Keyboard Layout ID
        //   (see HKLM\SYSTEM\CurrentControlSet\Control\Keyboard Layouts)
        //
        //   Examples:
        //     "0407:00000407" - German (Germany) - German keyboard
        //     "0804:e00e0804" - Chinese (Simplified) - Microsoft Pinyin IME 3.0
        //
        // Vista: LLLL:KKKKKKKK;LLLL:{CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC}{PPPPPPPP-PPPP-PPPP-PPPP-PPPPPPPPPPPP};...
        //
        //   {CCC...C} - Class ID of Text Service
        //   {PPP...P} - Profile GUID of Text Service
        //   (see HKLM\SOFTWARE\Microsoft\CTF\TIP)
        //
        //   Examples:
        //     "0804:00000804;0804:{81D4E9C9-1D3B-41BC-9E6C-4B40BF79E35E}{F3BA9077-6C7E-11D4-97FA-0080C882687E}"
        //     will add (or remove),
        //     Chinese (PRC) - Chinese (Simplified) - US keyboard and
        //     Chinese (PRC) - Microsoft Pinyin IME
        //

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layoutData"></param>
        /// <returns></returns>
        public static bool TryInstallLayout(KeyboardLayout layoutData)
        {
            if (layoutData == null)
            {
                return false;
            }

            if (Environment.OSVersion.Version.Major < 6)
            {
                // Use input!InstallInputLayout (@102) and 
                // UnInstallInputLayout (@103) on down-level
                // (XP and WS03)
                //
                string xpLayoutString = layoutData.XpLayout;
                int colon = xpLayoutString.IndexOf(":");
                ushort lcid = ushort.Parse(xpLayoutString.Substring(0, colon), NumberStyles.HexNumber);
                uint layout = uint.Parse(xpLayoutString.Substring(colon + 1), NumberStyles.HexNumber);
                bool defLayout = false;
                IntPtr hklDefault = IntPtr.Zero;
                bool defUser = false;
                bool sysLocale = false;

                return InstallInputLayout(lcid, layout, defLayout, hklDefault, defUser, sysLocale);
            }
            else
            {
                // Use input!InstallLayoutOrTip on Vista
                //
                string vistaTipString = layoutData.VistaTip;
                if (QueryLayoutOrTipString(vistaTipString, 0) != IntPtr.Zero)
                    return false;
                uint flags = 0;
                bool val;
                val = InstallLayoutOrTip(vistaTipString, flags);
                val = IsLayoutInstalled(layoutData);
                return val;
            }
        }

        // String format of layoutOrTip:
        //
        // Down-level: LLLL:KKKKKKKK
        //
        //   LLLL - Language ID
        //   KKKKKKKK - Keyboard Layout ID
        //   (see HKLM\SYSTEM\CurrentControlSet\Control\Keyboard Layouts)
        //
        //   Examples:
        //     "0407:00000407" - German (Germany) - German keyboard
        //     "0804:e00e0804" - Chinese (Simplified) - Microsoft Pinyin IME 3.0
        //
        // Vista: LLLL:KKKKKKKK;LLLL:{CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC}{PPPPPPPP-PPPP-PPPP-PPPP-PPPPPPPPPPPP};...
        //
        //   {CCC...C} - Class ID of Text Service
        //   {PPP...P} - Profile GUID of Text Service
        //   (see HKLM\SOFTWARE\Microsoft\CTF\TIP)
        //
        //   Examples:
        //     "0804:00000804;0804:{81D4E9C9-1D3B-41BC-9E6C-4B40BF79E35E}{F3BA9077-6C7E-11D4-97FA-0080C882687E}"
        //     will add (or remove),
        //     Chinese (PRC) - Chinese (Simplified) - US keyboard and
        //     Chinese (PRC) - Microsoft Pinyin IME
        //

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layoutData"></param>
        /// <returns></returns>
        public static bool TryUninstallLayout(KeyboardLayout layoutData)
        {
            if (layoutData == null)
            {
                return false;
            }

            if (Environment.OSVersion.Version.Major < 6)
            {
                // Use input!InstallInputLayout (@102) and 
                // UnInstallInputLayout (@103) on down-level
                // (XP and WS03)
                //
                string xpLayoutString = layoutData.XpLayout;
                int colon = xpLayoutString.IndexOf(":");
                ushort lcid = ushort.Parse(xpLayoutString.Substring(0, colon), NumberStyles.HexNumber);
                uint layout = uint.Parse(xpLayoutString.Substring(colon + 1), NumberStyles.HexNumber);
                IntPtr hklDefault = IntPtr.Zero;
                bool defUser = false;

                UnInstallInputLayout(lcid, layout, defUser);
                // input!UnInstallInputLayout may not have this layout in
                // its static locale info. Need to call UnloadKeyboardLayout
                // in this case.
                //
                UnloadInputLayout(lcid, layout);
            }
            else
            {
                // Use input!InstallLayoutOrTip on Vista
                //
                string vistaTipString = layoutData.VistaTip;
                if (QueryLayoutOrTipString(vistaTipString, 0) != IntPtr.Zero)
                    return false;

                uint flags = ILOT_UNINSTALL;
                InstallLayoutOrTip(vistaTipString, flags);
            }
            return true;
        }

        // Down-level: LLLL:KKKKKKKK
        //
        //   LLLL - Language ID
        //   KKKKKKKK - Keyboard Layout ID
        //   (see HKLM\SYSTEM\CurrentControlSet\Control\Keyboard Layouts)
        //
        //   Examples:
        //     "0407:00000407" - German (Germany) - German keyboard
        //     "0804:e00e0804" - Chinese (Simplified) - Microsoft Pinyin IME 3.0
        //
        // Vista: LLLL:KKKKKKKK;LLLL:{CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC}{PPPPPPPP-PPPP-PPPP-PPPP-PPPPPPPPPPPP};...
        //
        //   {CCC...C} - Class ID of Text Service
        //   {PPP...P} - Profile GUID of Text Service
        //   (see HKLM\SOFTWARE\Microsoft\CTF\TIP)
        //
        //   Examples:
        //     "0804:00000804;0804:{81D4E9C9-1D3B-41BC-9E6C-4B40BF79E35E}{F3BA9077-6C7E-11D4-97FA-0080C882687E}"
        //     will add (or remove),
        //     Chinese (PRC) - Chinese (Simplified) - US keyboard and
        //     Chinese (PRC) - Microsoft Pinyin IME
        //
        /// <summary>
        /// Activates specific keyboard layout
        /// </summary>
        /// <param name="layoutData"></param>
        /// <returns></returns>
        public static bool ActivateKeyboardLayout(KeyboardLayout layoutData)
        {
            if (layoutData == null)
            {
                return false;
            }

            string layoutString;
            if (Environment.OSVersion.Version.Major < 6)
            {
                layoutString = layoutData.XpLayout;
            }
            else
            {
                layoutString = layoutData.VistaTip;
            }

            //logic or parsing layout string
            int colonIndex = layoutString.IndexOf(":");
            ushort localeId = ushort.Parse(layoutString.Substring(0, colonIndex), NumberStyles.HexNumber);
            string[] layoutOrGuid = layoutString.Substring(colonIndex + 1).Replace("}", "").Split('{');
            bool activated = false;

            
            if (layoutOrGuid.Length == 1) //usually for XP string
            {
                uint layout = uint.Parse(layoutOrGuid[0], NumberStyles.HexNumber);
                IntPtr keyboardPtr = KeyboardLayoutIdToHkl(localeId, layout);
                activated = (ActivateKeyboardLayout(keyboardPtr, 0) != IntPtr.Zero);
            }
            else //Vista string
            {
                Guid localeGuid = new Guid(layoutOrGuid[1]);
                Guid profile = new Guid(layoutOrGuid[2]);
                InputProfiles langProfiles;
                if (TF_CreateInputProcessorProfiles(out langProfiles) == IntPtr.Zero)
                {
                    if (langProfiles.ChangeCurrentLanguage(localeId) == 0 &&
                        langProfiles.ActivateLanguageProfile(localeGuid, localeId, profile) == 0)
                    {
                        activated = true;
                    }
                    Marshal.ReleaseComObject(langProfiles);
                }
            }
            return activated;
        }

        #endregion

        #region Private Members

        //calls below are related to the activate layout Function call
        // HKL ActivateKeyboardLayout(HKL hkl, UINT Flags);
        [DllImport("user32.dll")]
        private static extern IntPtr ActivateKeyboardLayout(IntPtr keyboardPtr, uint Flags);

        // HRESULT WINAPI TF_CreateInputProcessorProfiles(ITfInputProcessorProfiles **ppipr);
        [DllImport("msctf.dll")]
        private static extern IntPtr TF_CreateInputProcessorProfiles(out InputProfiles profiles);

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("1F02B6C5-7842-4EE6-8A0B-9A24183A95CA"),
        System.Security.SuppressUnmanagedCodeSecurity]
        private interface InputProfiles
        {
            // HRESULT Register([in] REFCLSID localeGuid);
            [PreserveSig]
            int Register([In, MarshalAs(UnmanagedType.LPStruct)] Guid localeGuid);

            // HRESULT Unregister([in] REFCLSID localeGuid);
            [PreserveSig]
            int Unregister([In, MarshalAs(UnmanagedType.LPStruct)] Guid localeGuid);

            // HRESULT AddLanguageProfile([in] REFCLSID localeGuid,
            //                            [in] LANGID langid,
            //                            [in] REFGUID guidProfile,
            //                            [in, size_is(cchDesc)] const WCHAR *pchDesc,
            //                            [in] ULONG cchDesc,
            //                            [in, size_is(cchFile)] const WCHAR *pchIconFile,
            //                            [in] ULONG cchFile,
            //                            [in] ULONG uIconIndex);
            [PreserveSig]
            int AddLanguageProfile(
                    [In, MarshalAs(UnmanagedType.LPStruct)] Guid localeGuid,
                    UInt16 langid,
                    [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidProfile,
                    [MarshalAs(UnmanagedType.LPWStr)] string pchDesc,
                    int cchDesc,
                    [MarshalAs(UnmanagedType.LPWStr)] string iconFile,
                    int cchFile,
                    int iconIndex);

            // HRESULT RemoveLanguageProfile([in] REFCLSID localeGuid,
            //                               [in] LANGID langid,
            //                               [in] REFGUID guidProfile);
            [PreserveSig]
            int RemoveLanguageProfile(
                    [In, MarshalAs(UnmanagedType.LPStruct)] Guid localeGuid,
                    UInt16 langid,
                    [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidProfile);

            // HRESULT EnumInputProcessorInfo([out] IEnumGUID **ppEnum);
            [PreserveSig]
            int EnumInputProcessorInfo([Out, MarshalAs(UnmanagedType.Interface)] out IEnumGUID enumGuid);

            // HRESULT GetDefaultLanguageProfile([in] LANGID langid,
            //                                   [in] REFGUID catid,
            //                                   [out] CLSID *localeGuid,
            //                                   [out] GUID *pguidProfile);
            [PreserveSig]
            int GetDefaultLanguageProfile(
                    ushort langid,
                    [In, MarshalAs(UnmanagedType.LPStruct)] Guid catid,
                    out Guid localeGuid,
                    out Guid guidProfile);

            // HRESULT SetDefaultLanguageProfile([in] LANGID langid,
            //                                   [in] REFCLSID localeGuid,
            //                                   [in] REFGUID guidProfiles);
            [PreserveSig]
            int SetDefaultLanguageProfile(
                    UInt16 langid,
                    [In, MarshalAs(UnmanagedType.LPStruct)] Guid localeGuid,
                    [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidProfile);

            // HRESULT ActivateLanguageProfile([in] REFCLSID localeGuid, 
            //                                 [in] LANGID langid, 
            //                                 [in] REFGUID guidProfiles);
            [PreserveSig]
            int ActivateLanguageProfile(
                    [In, MarshalAs(UnmanagedType.LPStruct)] Guid localeGuid,
                    UInt16 langid,
                    [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidProfile);

            // HRESULT GetActiveLanguageProfile([in] REFCLSID localeGuid, 
            //                                  [out] LANGID *plangid, 
            //                                  [out] GUID *pguidProfile);
            [PreserveSig]
            int GetActiveLanguageProfile(
                    [In, MarshalAs(UnmanagedType.LPStruct)] Guid localeGuid,
                    out UInt16 langid,
                    out Guid guidProfile);

            // HRESULT GetLanguageProfileDescription([in] REFCLSID localeGuid, 
            //                                       [in] LANGID langid, 
            //                                       [in] REFGUID guidProfile,
            //                                       [out] BSTR *pbstrProfile);
            [PreserveSig]
            int GetLanguageProfileDescription(
                    [In, MarshalAs(UnmanagedType.LPStruct)] Guid localeGuid,
                    UInt16 langid,
                    [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidProfile,
                    [Out, MarshalAs(UnmanagedType.BStr)] out string strProfile);

            // HRESULT GetCurrentLanguage([out] LANGID *plangid);
            [PreserveSig]
            int GetCurrentLanguage(out UInt16 langid);

            // HRESULT ChangeCurrentLanguage([in] LANGID langid);
            [PreserveSig]
            int ChangeCurrentLanguage(UInt16 langid);


        }

        /// <summary>
        /// IEnumGUID interface (defined in ComCat.idl)
        /// </summary>
        [
            ComImport,
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
            Guid("0002E000-0000-0000-C000-000000000046"),
            System.Security.SuppressUnmanagedCodeSecurity
        ]
        public interface IEnumGUID
        {
            // HRESULT Next([in] ULONG celt,
            //              [out, size_is(celt), length_is(*pceltFetched)] GUID *rgelt,
            //              [out] ULONG *pceltFetched);
            /// <summary></summary>
            /// <param name="count"></param>
            /// <param name="guidArray"></param>
            /// <param name="fetched"></param>
            /// <returns></returns>
            [PreserveSig]
            int Next(int count,
                      [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] Guid[] guidArray,
                      out int fetched);

            // HRESULT Skip([in] ULONG celt);
            /// <summary></summary>
            /// <param name="count"></param>
            /// <returns></returns>
            [PreserveSig]
            int Skip(int count);

            // HRESULT Reset();
            /// <summary></summary>
            /// <returns></returns>
            [PreserveSig]
            int Reset();

            // HRESULT Clone([out] IEnumGUID **ppenum);
            /// <summary></summary>
            /// <param name="enumGuid"></param>
            /// <returns></returns>
            [PreserveSig]
            int Clone([Out, MarshalAs(UnmanagedType.Interface)] out IEnumGUID enumGuid);
        }

        /// Example: "0407:00000407" LCID: 0407
        ///                          LAYOUT:0000407
        /// <summary>
        /// Internal function for unloading layout
        /// </summary>
        /// <param name="lcid">LocaleId</param>
        /// <param name="layout">keyboard layout</param>
        /// <returns></returns>
        private static bool UnloadInputLayout(ushort lcid, uint layout)
        {
            IntPtr hkl = KeyboardLayoutIdToHkl(lcid, layout);
            return UnloadKeyboardLayout(hkl);
        }

        /// Example: "0407:00000407" LCID: 0407
        ///                          LAYOUT:0000407
        /// <summary>
        /// Function to return pointer to crresponding keyboard layout
        /// </summary>
        /// <param name="lcid">Locale ID</param>
        /// <param name="layout">keyboard layout</param>
        /// <returns></returns>
        private static IntPtr KeyboardLayoutIdToHkl(ushort lcid, uint layout)
        {
            uint layoutPtr = 0;

            if ((layout & 0xf0000000) == 0xe0000000)
            {
                // IMM32's IME layout is HKL value.
                //
                layoutPtr = layout;
            }
            else
            {
                // Create hKL value with/without Layout ID.
                //
                string key = String.Format(@"SYSTEM\CurrentControlSet\Control\Keyboard Layouts\{0:x8}", layout);
                using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(key))
                {
                    if (regKey != null)
                    {
                        string layoutId = regKey.GetValue("Layout Id") as string;
                        if (String.IsNullOrEmpty(layoutId))
                        {
                            layoutPtr = (layout << 16) + lcid;
                        }
                        else
                        {
                            uint hexLayoutId = uint.Parse(layoutId, NumberStyles.HexNumber);
                            layoutPtr = 0xf0000000 + (hexLayoutId << 16) + lcid;
                        }
                    }
                }
            }

            if (layoutPtr == 0)
                return IntPtr.Zero;

            return new IntPtr((int)layoutPtr);
        }

        // extern "C" BOOL WINAPI InstallInputLayout(LCID lcid, DWORD dwLayout, BOOL bDefLayout, HKL hklDefault,
        //      BOOL bDefUser, BOOL bSysLocale);
        [DllImport("input.dll", EntryPoint = "#102")]
        private static extern bool InstallInputLayout(ushort lcid, uint dwLayout, bool bDefLayout, IntPtr hklDefault,
            bool bDefUser, bool bSysLocale);

        // extern "C" BOOL WINAPI UnInstallInputLayout(LCID lcid, DWORD dwLayout, BOOL bDefUser);
        [DllImport("input.dll", EntryPoint = "#103")]
        private static extern bool UnInstallInputLayout(ushort lcid, uint dwLayout, bool bDefUser);

        // extern "C" HRESULT WINAPI QueryLayoutOrTipString(_In_ LPCWSTR psz, DWORD dwFlags);
        [DllImport("input.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr QueryLayoutOrTipString(string psz, uint dwFlags);

        // extern "C" BOOL WINAPI InstallLayoutOrTip(_In_ LPCWSTR psz, DWORD dwFlags);
        [DllImport("input.dll", CharSet = CharSet.Unicode)]
        private static extern bool InstallLayoutOrTip(string psz, uint dwFlags);

        // extern "C" BOOL WINAPI SetDefaultLayoutOrTip(_In_ LPCWSTR psz, DWORD dwFlags);
        [DllImport("input.dll", CharSet = CharSet.Unicode)]
        private static extern bool SetDefaultLayoutOrTip(string psz, uint dwFlags);

        // BOOL UnloadKeyboardLayout(HKL hkl);
        [DllImport("user32.dll")]
        private static extern bool UnloadKeyboardLayout(IntPtr hkl);

        private const uint ILOT_UNINSTALL = 0x00000001;

        #endregion

        #endregion // InstallOrUninstallLayoutOrTip

        #region GetLayoutsForCulture

        #region Public Members

        // This function returns a list of layout or TIP for specified locale.
        // (Keyboards or TIPs added by the system when user or system locale is changed.)
        //
        // This list is defined under [Locales] section in intl.inf on down-level (XP and WS03)
        //
        // On Vista, NLS API GetLocaleInfo(LOCALE_SKEYBOARDSTOINSTALL) provides this information.
        //
        //   Examples:
        //     locale = 0x00000804
        //
        //     Return value on down-level:
        //       { "0804:00000804", "0804:e00e0804", "0804:e0010804", "0804:e0030804", "0804:e0040804" }
        //
        //     Return value on Vista:
        //       { "0804:00000804, 0804:{81D4E9C9-1D3B-41BC-9E6C-4B40BF79E35E}{F3BA9077-6C7E-11D4-97FA-0080C882687E}" }
        /// <summary>
        /// Gets the language id corresponding to locale id
        /// </summary>
        /// <param name="cultureInfo">Culture Info</param>
        /// <returns>array of layout ID's</returns>
        public static KeyboardLayout[] GetLayoutsForCulture(CultureInfo cultureInfo)
        {
            uint locale = (uint)cultureInfo.LCID;
            string[] layoutOrTipArr = new string[] { null };
            KeyboardLayout[] layoutArr;

            if (Environment.OSVersion.Version.Major < 6)
            {
                // Get a list of language:layout ID pair
                // for the specified locale from [Locales]
                // section in intl.inf on down-level
                // (XP and WS03)
                //
                // XPSP2 intl.inf sample:
                // [Locales]
                // 00000409 = %English_United_States%      ,437     ,1,,0409:00000409
                // 00000407 = %German_Standard%            ,850     ,1,,0407:00000407,0409:00000409
                // 00000404 = %Chinese_----%             ,950     ,9,,0404:00000404,0404:e0080404,0404:E0010404
                //
                IntPtr inf = SetupOpenInfFile("intl.inf", null, INF_STYLE_WIN4, IntPtr.Zero);
                if (inf != s_INVALID_HANDLE_VALUE)
                {
                    string key = String.Format("{0:x8}", locale);
                    INFCONTEXT context = new INFCONTEXT();
                    if (SetupFindFirstLine(inf, "Locales", key, context))
                    {
                        ArrayList list = new ArrayList();
                        StringBuilder layout = new StringBuilder(14);

                        for (uint i = 5; SetupGetStringField(context, i, layout, layout.Capacity, IntPtr.Zero); i++)
                        {
                            list.Add(layout.ToString());
                        }
                        if (list.Count > 0)
                        {
                            layoutOrTipArr = (string[])list.ToArray(typeof(string));
                        }

                    }
                    SetupCloseInfFile(inf);
                }
            }
            else
            {
                // Use kernel32!GetLocaleInfo(LOCALE_SKEYBOARDSTOINSTALL) on Vista
                //
                int cchData = GetLocaleInfo(locale, LOCALE_SKEYBOARDSTOINSTALL, IntPtr.Zero, 0);
                if (cchData > 0)
                {
                    StringBuilder stringData = new StringBuilder(cchData);
                    if (GetLocaleInfo(locale, LOCALE_SKEYBOARDSTOINSTALL, stringData, cchData) == cchData)
                    {
                        layoutOrTipArr = stringData.ToString().TrimEnd(';').Split(';');
                    }
                }
            }

            int index = 0;
            layoutArr = new KeyboardLayout[layoutOrTipArr.Length];
            foreach (string layoutStr in layoutOrTipArr)
            {
                layoutArr[index] = new KeyboardLayout("UNKNOWN", layoutStr, layoutStr);
                index++;
            }
            return layoutArr;
        }

        #endregion

        #region Private Members

        // HINF SetupOpenInfFile(PCTSTR FileName, PCTSTR InfClass, DWORD InfStyle,
        //      PUINT ErrorLine);
        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetupOpenInfFile(string FileName, string InfClass, uint InfStyle,
            IntPtr pErrorLine);

        // void SetupCloseInfFile(HINF InfHandle);
        [DllImport("setupapi.dll")]
        private static extern void SetupCloseInfFile(IntPtr InfHandle);

        // BOOL SetupFindFirstLine(HINF InfHandle, PCTSTR Section, PCTSTR Key, PINFCONTEXT Context);
        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        private static extern bool SetupFindFirstLine(IntPtr InfHandle, string Section, string Key,
            [In, Out, MarshalAs(UnmanagedType.LPStruct)] INFCONTEXT Context);

        // BOOL SetupGetStringField(PINFCONTEXT Context,
        //      DWORD FieldIndex, PTSTR ReturnBuffer, DWORD ReturnBufferSize, PDWORD RequiredSize);
        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        private static extern bool SetupGetStringField([In, MarshalAs(UnmanagedType.LPStruct)] INFCONTEXT Context,
            uint FieldIndex, StringBuilder ReturnBuffer, int ReturnBufferSize, IntPtr RequiredSize);

        private const uint INF_STYLE_WIN4 = 0x00000002;
        private static readonly IntPtr s_INVALID_HANDLE_VALUE = new IntPtr(-1);

        [StructLayout(LayoutKind.Sequential)]
        private class INFCONTEXT
        {
            public IntPtr /*PVOID*/ Inf;
            public IntPtr /*PVOID*/ CurrentInf;
            public uint /*UINT*/ Section;
            public uint /*UINT*/ Line;
        }

        // int GetLocaleInfo(LCID Locale, LCTYPE LCType, LPTSTR lpLCData, int cchData);
        [DllImport("kernel32.dll")]
        private static extern int GetLocaleInfo(uint Locale, uint LCType, IntPtr lpLCData, int cchData);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetLocaleInfo(uint Locale, uint LCType, StringBuilder lpLCData, int cchData);

        private const uint LOCALE_SKEYBOARDSTOINSTALL = 0x0000005e;

        #endregion

        #endregion // GetLayoutOrTipForLocale

        #region IsLayoutInstalled

        #region Public Members
        /// <summary>
        /// This function determines if specified layout or TIP is enabled.
        /// </summary>
        /// <param name="layoutData"></param>
        /// <returns></returns>       
        public static bool IsLayoutInstalled(KeyboardLayout layoutData)
        {
            if (layoutData == null)
            {
                return false;
            }

            string layoutOrTip = (Environment.OSVersion.Version.Major < 6) ? layoutData.XpLayout : layoutData.VistaTip;

            //Chinese Pinyin SampleFast value on version below vista is "" 
            if (String.IsNullOrEmpty(layoutOrTip))
            {
                return false;
            }

            int colon = layoutOrTip.IndexOf(":");
            ushort lcid = ushort.Parse(layoutOrTip.Substring(0, colon), NumberStyles.HexNumber);
            string[] layoutOrGuid = layoutOrTip.Substring(colon + 1).Replace("}", "").Split('{');
            bool enabled = false;

            if (layoutOrGuid.Length == 1)
            {
                uint layoutId = uint.Parse(layoutOrGuid[0], NumberStyles.HexNumber);
                IntPtr hkl = KeyboardLayoutIdToHkl(lcid, layoutId);
                int count = GetKeyboardLayoutList(0, IntPtr.Zero);
                if (count > 0)
                {
                    IntPtr[] installedLayoutList = new IntPtr[count];
                    GetKeyboardLayoutList(count, installedLayoutList);
                    for (int i = 0; !enabled && i < count; i++)
                    {
                        enabled = (hkl == installedLayoutList[i]);
                    }
                }
            }
            else
            {
                Guid clsid = new Guid(layoutOrGuid[1]);
                Guid profile = new Guid(layoutOrGuid[2]);
                string key = String.Format(@"SOFTWARE\Microsoft\CTF\TIP\{0}\LanguageProfile\0x{1:x8}\{2}",
                    clsid.ToString("B"), lcid, profile.ToString("B"));
                using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(key))
                {
                    if (regKey != null)
                    {
                        object enable = regKey.GetValue("Enable");
                        enabled = (enable == null) ? false : (0 != (int)enable);
                    }
                }
            }
            return enabled;
        }

        /// <summary>
        /// Gets the current installed Keyboard Layouts
        /// Made public since its safe
        /// </summary>
        public static List<KeyboardLayout> GetCurrentKeyboardLayouts()
        {
            int count = GetKeyboardLayoutList(0, IntPtr.Zero);
            if (count > 0)
            {
                List<KeyboardLayout> layoutArr = new List<KeyboardLayout>();
                IntPtr[] installedLayoutList = new IntPtr[count];
                GetKeyboardLayoutList(count, installedLayoutList);
                for (int i = 0; i < count; i++)
                {
                    string layout = String.Format("{0:x}", installedLayoutList[i].ToInt32());
                    layout = layout.Substring(layout.Length - 4);
                    CultureInfo culture = new CultureInfo(Int32.Parse(layout, NumberStyles.HexNumber));
                    switch (layout)
                    {
                        case ChineseLayout:
                            if (KeyboardLayoutHelper.IsLayoutInstalled(KeyboardLayouts.ChinesePinyin))
                            {
                                layoutArr.Add(KeyboardLayouts.ChinesePinyin);
                            }
                            if (KeyboardLayoutHelper.IsLayoutInstalled(KeyboardLayouts.ChineseQuanPin))
                            {
                                layoutArr.Add(KeyboardLayouts.ChineseQuanPin);
                            }
                            if (KeyboardLayoutHelper.IsLayoutInstalled(new KeyboardLayout("Chinese English", layout+":"+layout,layout+":"+layout)))
                            {
                                layoutArr.Add(new KeyboardLayout("Chinese English", layout+":"+layout,layout+":"+layout));
                            }
                            //add Microsoft Chinese SimpleFast KeyboardLayout
                            if (KeyboardLayoutHelper.IsLayoutInstalled(KeyboardLayouts.ChinesePinyinSimpleFast))
                            {
                                layoutArr.Add(KeyboardLayouts.ChinesePinyinSimpleFast);
                            }
                            break;

                        case KoreanLayout:
                            if (KeyboardLayoutHelper.IsLayoutInstalled(KeyboardLayouts.Korean))
                            {
                                layoutArr.Add(KeyboardLayouts.Korean);
                            }
                            break;

                        default:
                            layout = layout + ":" + layout;
                            layoutArr.Add(new KeyboardLayout(culture.DisplayName, layout, layout));
                            break;
                    }

                }
                return layoutArr;
            }
            return null;
        }

        /// <summary>
        /// Gets the users deafult layout
        /// Public since this doesnt change the sys state
        /// </summary>
        /// <returns></returns>
        public static KeyboardLayout GetUsersDefaultLayout()
        {
            ushort localeId = GetUserDefaultLCID();
            string layout = String.Format("{0:x}", localeId);
            layout = layout + ":" + layout;
            CultureInfo culture = new CultureInfo(localeId);
            return (new KeyboardLayout(culture.DisplayName, layout, layout));
        }

        #endregion

        #region Internal Members
        /// <summary>
        /// Sets the users default layout during reinstate operation
        /// This is internal since users are unlikely to change the default layout
        /// A more likely operation is switching layouts.
        /// </summary>
        /// <param name="layout"></param>
        /// <returns></returns>
        internal static bool SetUsersDefaultLayout(KeyboardLayout layout)
        {
            uint flags = 0;
            string layoutStr = (Environment.OSVersion.Version.Major < 6) ? layout.XpLayout : layout.VistaTip;
            return SetDefaultLayoutOrTip(layoutStr, flags);
        }

        #endregion

        #region Private Members

        [DllImport("kernel32.dll")]
        private static extern ushort GetUserDefaultLCID();
        // UINT GetKeyboardLayoutList(int nBuff, HKL* lpList);
        [DllImport("user32.dll")]
        private static extern int GetKeyboardLayoutList(int nBuff, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IntPtr[] lpList);
        [DllImport("user32.dll", EntryPoint = "GetKeyboardLayoutList")]
        private static extern int GetKeyboardLayoutList(int nBuff, IntPtr lpList);

        #endregion

        #region const Members

        const string ChineseLayout = "0804";
        const string KoreanLayout = "0412";

        #endregion
        #endregion
    }

    /// <summary>
    /// static class wrapping Keyboard layout info
    /// </summary>
    public class KeyboardLayout
    {
        #region Private Data

        private string _displayName;
        private string _vistaTip;
        private string _xpLayout;

        #endregion

        #region Contructors
        /// <summary>
        /// KeyboardLayout constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="vistaStr"></param>
        /// <param name="xpStr"></param>
        public KeyboardLayout(string name, string vistaStr, string xpStr)
        {
            _displayName = name;
            _vistaTip = vistaStr;
            _xpLayout = xpStr;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// DisplayName
        /// </summary>
        [XmlAttribute()]
        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }

        /// <summary>
        /// VistaTip
        /// </summary>
        [XmlAttribute()]
        public string VistaTip
        {
            get { return _vistaTip; }
            set { _vistaTip = value; }
        }

        /// <summary>
        /// XpLayout
        /// </summary>
        [XmlAttribute()]
        public string XpLayout
        {
            get { return _xpLayout; }
            set { _xpLayout = value; }
        }

        #endregion

        #region Override Members

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            KeyboardLayout otherLayout = obj as KeyboardLayout;
            return ((this.DisplayName == otherLayout.DisplayName) &&
                    (this.VistaTip == otherLayout.VistaTip) &&
                    (this.XpLayout == otherLayout.XpLayout));
        }

        /// <summary>
        /// GetHashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (this._displayName.GetHashCode() ^ this.VistaTip.GetHashCode() ^ this.XpLayout.GetHashCode());
        }

        /// <summary>
        /// == operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(KeyboardLayout a, KeyboardLayout b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (Object.Equals(a, null) || Object.Equals(b, null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        /// <summary>
        /// != operator
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(KeyboardLayout a, KeyboardLayout b)
        {
            return !(a == b);
        }


        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Display Name: " + this.DisplayName + "\r\n" +
                   "Vista Tip: " + this.VistaTip + "\r\n" +
                   "XP Layout: " + this.XpLayout;
        }

        #endregion
    }

    /// <summary>
    /// static class to provide the cultureInfos for different locales
    /// ex: ClutureInfos.ChinesePrc 
    /// returns the cultureInfo for the chinesePRC locale
    /// </summary>
    public static class CultureInfos
    {
        #region Public Members

        /// <summary>
        /// ArabicQatar
        /// </summary>
        public static CultureInfo ArabicQatar
        {
            get { return new CultureInfo("ar-QA"); }
        }

        /// <summary>
        /// ChinesePrc
        /// </summary>
        public static CultureInfo ChinesePrc
        {
            get { return new CultureInfo("zh-CN"); }
        }

        /// ChineseTraditional
        /// </summary>
        public static CultureInfo ChineseTraditional
        {
            get { return new CultureInfo("zh-TW"); }
        }

        /// <summary>
        /// EnglishUnitedStates
        /// </summary>
        public static CultureInfo EnglishUnitedStates
        {
            get { return new CultureInfo("en-US"); }
        }

        /// <summary>
        /// GermanGermany
        /// </summary>
        public static CultureInfo GermanGermany
        {
            get { return new CultureInfo("de-DE"); }
        }

        /// <summary>
        /// SpanishSpain
        /// </summary>
        public static CultureInfo SpanishSpain
        {
            get { return new CultureInfo("es-ES"); }
        }

        /// <summary>
        /// PortugueseBrazil
        /// </summary>
        public static CultureInfo PortugueseBrazil
        {
            get { return new CultureInfo("pt-BR"); }
        }

        #endregion
    }

    /// <summary>
    /// static class wrapping Keyboard layout info
    /// ex: KeyboardLayouts.English 
    /// return layout for the English language
    /// </summary>
    public static class KeyboardLayouts
    {
        #region Public Members

        /// <summary>
        /// English layout
        /// </summary>
        public static KeyboardLayout English
        {
            get
            {
                return FindByLocaleString(EnglishDisplayName);
            }
        }

        /// <summary>
        /// German Layout
        /// </summary>
        public static KeyboardLayout German
        {
            get
            {
                return FindByLocaleString(GermanDisplayName);
            }
        }

        /// <summary>
        /// Chinese Pinyin layout
        /// </summary>
        public static KeyboardLayout ChinesePinyin
        {
            get
            {
                return FindByLocaleString(ChinesePinyinDisplayName);
            }
        }

        /// <summary>
        /// Chinese Quanpin layout
        /// </summary>
        public static KeyboardLayout ChineseQuanPin
        {
            get
            {
                return FindByLocaleString(ChineseQuanpinDisplayName);
            }
        }
        /// <summary>
        /// Chinese NewPhonetic layout
        /// </summary>
        public static KeyboardLayout ChineseNewPhonetic
        {
            get
            {
                return FindByLocaleString(ChineseNewPhoneticDisplayName);
            }
        }

        /// <summary>
        /// Chinese NewChangJie layout
        /// </summary>
        public static KeyboardLayout ChineseNewChangJie
        {
            get
            {
                return FindByLocaleString(ChineseNewChangJieDisplayName);
            }
        }

        /// <summary>
        /// Japanese layout
        /// </summary>
        public static KeyboardLayout Japanese
        {
            get
            {
                return FindByLocaleString(JapaneseImeDisplayName);
            }
        }

        /// <summary>
        /// Korean layout
        /// </summary>
        public static KeyboardLayout Korean
        {
            get
            {
                return FindByLocaleString(KoreanImeDisplayName);
            }
        }

        /// <summary>
        /// Chinese Pinyin SimpleFast layout
        /// </summary>
        public static KeyboardLayout ChinesePinyinSimpleFast
        {
            get
            {
                return FindByLocaleString(ChinesePinyinSimpleFastDisplayName);
            }
        }


        private static KeyboardLayout FindByLocaleString(string localeString)
        {
            for (int i = 0; i < KeyboardLayoutArray.Length; i++)
            {
                if (KeyboardLayoutArray[i].DisplayName == localeString)
                {
                    return KeyboardLayoutArray[i];
                }
            }
            return null;
        }

        #endregion

        #region Private methods.

        /// <summary>
        /// Creates a new KeyboardLayout instance for the given script.
        /// </summary>
        private static KeyboardLayout ForValues(string displayName,
            string vistaTip, string xpLayout)
        {
            KeyboardLayout result = new KeyboardLayout(displayName, vistaTip, xpLayout);
            return result;
        }

        #endregion

        #region Internal Members

        internal static string EnglishDisplayName = @"English (United States)";
        internal static string GermanDisplayName = @"German (Germany)";
        internal static string ChinesePinyinDisplayName = @"Microsoft Pinyin IME";
        internal static string ChineseNewPhoneticDisplayName = @"Microsoft NewPhonetic IME";
        internal static string ChineseNewChangJieDisplayName = @"Microsoft NewChangJie IME";
        internal static string ChineseQuanpinDisplayName = @"Microsoft Quanpin IME";
        internal static string JapaneseImeDisplayName = @"Japanese IME";
        internal static string KoreanImeDisplayName = @"Korean IME";
        internal static string ChinesePinyinSimpleFastDisplayName = @"Microsoft Chinese Pinyin SimpleFast";

        /// <summary>
        /// KeyboardLayout values.
        /// </summary>
        internal static KeyboardLayout[] KeyboardLayoutArray = new KeyboardLayout[] {
            ForValues(EnglishDisplayName, "0409:00000409", "0409:00000409"),
            ForValues(GermanDisplayName, "0407:00000407", "0407:00000407"),
            ForValues(ChinesePinyinDisplayName, @"0804:{81D4E9C9-1D3B-41BC-9E6C-4B40BF79E35E}{F3BA9077-6C7E-11D4-97FA-0080C882687E}", "0804:e00e0804"),
            ForValues(ChineseQuanpinDisplayName,  @"0804:{E429B25A-E5D3-4D1F-9BE3-0C608477E3A1}{54FC610E-6ABD-4685-9DDD-A130BDF1B170}", "0804:e0010804"),            
            ForValues(ChineseNewPhoneticDisplayName,  @"0404:{531FDEBF-9B4C-4A43-A2AA-960E8FCDC732}{B2F9C502-1742-11D4-9790-0080C882687E}", "0404:e0080404"),
            ForValues(ChineseNewChangJieDisplayName,  @"0404:{531FDEBF-9B4C-4A43-A2AA-960E8FCDC732}{F3BA907A-6C7E-11D4-97FA-0080C882687E}", "0404:e0090404"),
            ForValues(JapaneseImeDisplayName, @"0411:{03B5835F-F03C-411B-9CE2-AA23E1171E36}{A76C93D9-5523-4E90-AAFA-4DB112F9AC76}", "0411:e0010411"),
            ForValues(KoreanImeDisplayName, @"0412:{A028AE76-01B1-46C2-99C4-ACD9858AE02F}{B5FE1F02-D5F2-4445-9C03-C568F23C99A1}", "0412:e0010412"),
            ForValues(ChinesePinyinSimpleFastDisplayName, @"0804:{81d4e9c9-1d3b-41bc-9e6c-4b40bf79e35e}{FA550B04-5AD7-411f-A5AC-CA038EC515D7}", "")
        };

        #endregion
    }
}
