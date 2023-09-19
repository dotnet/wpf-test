// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: basic input emulation.
//

namespace Microsoft.Test.Graphics
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Windows.Interop;

    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;

    #endregion Namespaces.

    /// <summary>Provides input emulation for DRT.</summary>
    internal static class Input
    {
        #region Input emulation support.

        #region Mouse support.

        [DllImport("user32.dll", EntryPoint = "SendInput")]
        private static extern uint SendMouseInput(uint nInputs,
            MouseInput[] pInputs, int cbSize);
        [DllImport("user32.dll", EntryPoint = "SendInput")]
        private static extern uint SendMouseInput(uint nInputs,
            ref MouseInput pInputs, int cbSize);

        // Some marshalling notes:
        // - arrays are a-ok
        // - DWORD is UInt32
        // - UINT is UInt32
        // - CHAR is char is Char with ANSI decoration
        // - LONG is Int32
        // - WORD is UInt16

        [StructLayout(LayoutKind.Sequential)]
        private struct MouseInput
        {
            public IntPtr type;

            public int dx;              // 32
            public int dy;              // 32 - 64
            public int mouseData;       // 32 - 96
            public uint dwFlags;        // 32 - 128
            public IntPtr time;         // 32 - 160
            public IntPtr dwExtraInfo;  // 32 - 192
        }

        /// <summary>
        /// Sets up a mouse move message, adjusting the coordinates appropriately.
        /// </summary>
        private static void SetupScreenMouseMove(ref MouseInput input, int x, int y)
        {
            input.type = new IntPtr(INPUT_MOUSE);

            // Hard-coded to a point inside the client area. Correct
            // thing to do is map from client area to screen points,
            // but it requires more P/Invoke.
            input.dx = x;
            input.dy = y;
            input.dwFlags = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE_NOCOALESCE;

            // Absolute pixels must be specified in a screen of 65,535 by
            // 65,535 regardless of real size. Add half a pixel to account
            // for rounding problems.
            int screenWidth = GetSystemMetrics(SM_CXSCREEN);
            int screenHeight = GetSystemMetrics(SM_CYSCREEN);
            float ratioX = 65536 / screenWidth;
            float ratioY = 65536 / screenHeight;
            float halfX = 65536 / (screenWidth * 2);
            float halfY = 65536 / (screenHeight * 2);

            input.dx = (int)(input.dx * ratioX + halfX);
            input.dy = (int)(input.dy * ratioY + halfY);
        }

        /// <summary>GetSystemMetrics wrapper.</summary>
        /// <param name="nIndex">Index of metric to get.</param>
        /// <returns>System value.</returns>
        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        private const uint INPUT_MOUSE = 0;
        private const uint INPUT_KEYBOARD = 1;

        private const uint MOUSEEVENTF_MOVE = 0x0001;
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const uint MOUSEEVENTF_MOVE_NOCOALESCE = 0x2000;
        private const uint MOUSEEVENTF_ABSOLUTE = 0x8000;

        /// <summary>Width of the screen of the primary display monitor, in pixels.</summary>
        public const int SM_CXSCREEN = 0;
        /// <summary>Height of the screen of the primary display monitor, in pixels.</summary>
        public const int SM_CYSCREEN = 1;

        public static void ClickScreenPoint(int x, int y)
        {
            MouseInput[] input = new MouseInput[3];

            SetupScreenMouseMove(ref input[0], x, y);

            input[1].type = new IntPtr(INPUT_MOUSE);
            input[1].dwFlags = MOUSEEVENTF_LEFTDOWN;

            input[2].type = new IntPtr(INPUT_MOUSE);
            input[2].dwFlags = MOUSEEVENTF_LEFTUP;

            unsafe
            {
                SendMouseInput((uint)input.Length, input, sizeof(MouseInput));
            }
        }

        public static void MouseDown()
        {
            MouseInput[] input = new MouseInput[1];
            input[0].type = new IntPtr(INPUT_MOUSE);
            input[0].dwFlags = MOUSEEVENTF_LEFTDOWN;

            unsafe
            {
                SendMouseInput((uint)input.Length, input, sizeof(MouseInput));
            }
        }

        public static void MouseMove(int x, int y)
        {
            MouseInput[] input = new MouseInput[1];

            SetupScreenMouseMove(ref input[0], x, y);

            unsafe
            {
                SendMouseInput((uint)input.Length, input, sizeof(MouseInput));
            }
        }

        public static void MouseUp()
        {
            MouseInput[] input = new MouseInput[1];
            input[0].type = new IntPtr(INPUT_MOUSE);
            input[0].dwFlags = MOUSEEVENTF_LEFTUP;

            unsafe
            {
                SendMouseInput((uint)input.Length, input, sizeof(MouseInput));
            }
        }

        #endregion Mouse support.

        #endregion Input emulation support.

        #region Element positioning helpers.

        /// <summary>Defines the x- and y- coordinates of a point.</summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            /// <summary>Specifies the x-coordinate of the point.</summary>
            public int x;
            /// <summary>Specifies the x-coordinate of the point.</summary>
            public int y;

            /// <summary>Creates a new Test.Uis.Wrappers.Win32.POINT instance.</summary>
            /// <param name="x">Specifies the x-coordinate of the point.</param>
            /// <param name="y">Specifies the y-coordinate of the point.</param>
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        /// <summary>
        /// Gets the rectangle that bounds the specified element, relative
        /// to the client area of the window the element is in.
        /// </summary>
        /// <param name='element'>Element to get rectangle for.</param>
        /// <returns>The System.Windows.Rect that bounds the element.</returns>
        public static Rect GetClientRelativeRect(UIElement element)
        {
            Visual parent;  // Topmost parent of element.
            Matrix m;       // Matrix to transform corodinates.
            Point[] points; // Points around element.

            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            parent = GetTopMostVisual(element);
            points = GetRenderSizeBoxPoints(element);

            System.Windows.Media.GeneralTransform gt =
                element.TransformToAncestor(parent);
            System.Windows.Media.Transform t = gt as System.Windows.Media.Transform;
            if (t != null)
            {
                m = t.Value;
            }
            else
            {
                throw new System.ApplicationException("//TODO: Handle GeneralTransform Case - introduced by Transforms Breaking Change");
            }

            m.Transform(points);

            // Assume there is no rotation.

            return new Rect(points[0], points[3]);
        }

        /// <summary>
        /// Gets an array of four bounding points for the computed
        /// size of the specified element. The top-left corner
        /// is (0;0) and the bottom-right corner is (width;height).
        /// </summary>
        private static Point[] GetRenderSizeBoxPoints(UIElement element)
        {
            // Get the points for the rectangle and transform them.
            double height = element.RenderSize.Height;
            double width = element.RenderSize.Width;
            Point[] points = new Point[4];
            points[0] = new Point(0, 0);
            points[1] = new Point(width, 0);
            points[2] = new Point(0, height);
            points[3] = new Point(width, height);
            return points;
        }

        /// <summary>
        /// Gets the rectangle that bounds the specified element, relative
        /// to the top-left corner of the screen.
        /// </summary>
        /// <param name='element'>Element to get rectangle for.</param>
        /// <returns>The rectangle that bounds the element.</returns>
        internal static Rect GetScreenRelativeRect(UIElement element)
        {
            POINT topLeft;
            Rect clientRect;

            PresentationSource source;

            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            source = PresentationSource.FromVisual(element);

            if (source == null)
            {
                throw new InvalidOperationException("element is not connected to visual tree");
            }

            clientRect = GetClientRelativeRect(element);

            // Ignore high-DPI adjustment.
            topLeft = new POINT((int)Math.Round(clientRect.Left), (int)Math.Round(clientRect.Top));
            ClientToScreen(((HwndSource)source).Handle, ref topLeft);

            return new Rect(topLeft.x, topLeft.y, clientRect.Width, clientRect.Height);
        }

        /// <summary>
        /// Gets the top-most visual for the specified visual element.
        /// </summary>
        private static Visual GetTopMostVisual(Visual element)
        {
            PresentationSource source;

            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            source = PresentationSource.FromVisual(element);

            if (source == null)
            {
                throw new InvalidOperationException("The specified UiElement is not connected to a rendering Visual Tree.");
            }

            return source.RootVisual;
        }

        /// <summary>Converts the client-area coordinates of a specified point to screen coordinates.</summary>
        /// <param name="hwndFrom">Handle to the window whose client area is used for the conversion.</param>
        /// <param name="pt">POINT structure that contains the client coordinates to be converted.</param>
        /// <returns>true if the function succeeds, false otherwise.</returns>
        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(IntPtr hwndFrom, [In, Out] ref POINT pt);

        #endregion Element positiong helpers.
    }
}
