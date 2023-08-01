// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Runtime.InteropServices;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.Common
{
    /// <summary>
    /// Library Cursor functionality.
    /// </summary>
    public static class TestCursorLibrary
    {
        /// <summary>
        /// List of all public Avalon Cursors.
        /// </summary>
        /// <remarks>
        /// This list should be updated whenever a public Cursor is added to or removed from Avalon.
        /// </remarks>
        public static Cursor[] AllCursors
        {
            get
            {
                return s_allCursors;
            }
        }

        private static Cursor[] s_allCursors = new Cursor[] {
            Cursors.AppStarting,
            Cursors.Arrow,
            Cursors.ArrowCD,
            Cursors.Cross,
            Cursors.Hand,
            Cursors.Help,
            Cursors.IBeam,
            Cursors.No,
            Cursors.None,
            Cursors.Pen,
            Cursors.ScrollAll,
            Cursors.ScrollE,
            Cursors.ScrollN,
            Cursors.ScrollNE,
            Cursors.ScrollNS,
            Cursors.ScrollNW,
            Cursors.ScrollS,
            Cursors.ScrollSE,
            Cursors.ScrollSW,
            Cursors.ScrollW,
            Cursors.ScrollWE,
            Cursors.SizeAll,
            Cursors.SizeNESW,
            Cursors.SizeNS,
            Cursors.SizeNWSE,
            Cursors.SizeWE,
            Cursors.UpArrow,
            Cursors.Wait,
        };

        /// <summary>
        /// A new dictionary of Cursors, associating them with expected default cursor handles.
        /// </summary>
        private static Dictionary<Cursor, IntPtr> s_globalCursorDictionary =
            new Dictionary<Cursor, IntPtr>();

        /// <summary>
        /// Fill in default cursor handle dictionary with our handles.
        /// </summary>
        static TestCursorLibrary()
        {
            s_globalCursorDictionary.Add(Cursors.AppStarting, 
                NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_APPSTARTING));
            s_globalCursorDictionary.Add(Cursors.Arrow,
                NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_ARROW));
            s_globalCursorDictionary.Add(Cursors.ArrowCD,
                NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_ARROWCD));
            s_globalCursorDictionary.Add(Cursors.Cross,
                NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_CROSS));
            s_globalCursorDictionary.Add(Cursors.Hand,
                NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_HAND));
            s_globalCursorDictionary.Add(Cursors.Help,
                NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_HELP));
            s_globalCursorDictionary.Add(Cursors.IBeam,
                NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_IBEAM));
            s_globalCursorDictionary.Add(Cursors.No,
                NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_NO));
            s_globalCursorDictionary.Add(Cursors.None,
                IntPtr.Zero);
            s_globalCursorDictionary.Add(Cursors.Pen,
                NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_PEN));
            s_globalCursorDictionary.Add(Cursors.ScrollAll,
                NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_SCROLLALL));
            s_globalCursorDictionary.Add(Cursors.ScrollE,
                NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_SCROLLE));
            s_globalCursorDictionary.Add(Cursors.ScrollN,
                NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_SCROLLN));
            s_globalCursorDictionary.Add(Cursors.ScrollNE,
                NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_SCROLLNE));
            s_globalCursorDictionary.Add(Cursors.ScrollNS,
                NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_SCROLLNS));
            s_globalCursorDictionary.Add(Cursors.ScrollNW,
                NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_SCROLLNW));
            s_globalCursorDictionary.Add(Cursors.ScrollS,
                NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_SCROLLS));
            s_globalCursorDictionary.Add(Cursors.ScrollSE,
                NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_SCROLLSE));
            s_globalCursorDictionary.Add(Cursors.ScrollSW,
                NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_SCROLLSW));
            s_globalCursorDictionary.Add(Cursors.ScrollW,
                NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_SCROLLW));
            s_globalCursorDictionary.Add(Cursors.ScrollWE,
                NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_SCROLLWE));
            s_globalCursorDictionary.Add(Cursors.SizeAll,
                NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_SIZEALL));
            s_globalCursorDictionary.Add(Cursors.SizeNESW,
                NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_SIZENESW));
            s_globalCursorDictionary.Add(Cursors.SizeNS,
                NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_SIZENS));
            s_globalCursorDictionary.Add(Cursors.SizeNWSE,
                NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_SIZENWSE));
            s_globalCursorDictionary.Add(Cursors.SizeWE,
                NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_SIZEWE));
            s_globalCursorDictionary.Add(Cursors.UpArrow,
                NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_UPARROW));
            s_globalCursorDictionary.Add(Cursors.Wait,
                NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_WAIT));

        }

        /// <summary>
        /// Checks if the given cursor is the current cursor.
        /// </summary>
        public static bool IsCurrentCursor(Cursor cursor)
        {
            if (cursor == null) 
            {
                throw new ArgumentNullException("cursor");
            }

            IntPtr expectedHandle = IntPtr.Zero;
            s_globalCursorDictionary.TryGetValue(cursor, out expectedHandle);

            return IsCurrentCursor(expectedHandle);
        }

        /// <summary>
        /// Checks if the given cursor handle matches the current cursor.
        /// </summary>
        public static bool IsCurrentCursor(SafeHandle cursorHandle)
        {
            if (cursorHandle == null)
            {
                throw new ArgumentNullException("cursorHandle");
            }

            return IsCurrentCursor(cursorHandle.DangerousGetHandle());
        }

        /// <summary>
        /// Checks if the given cursor handle matches the current cursor.
        /// </summary>
        public static bool IsCurrentCursor(IntPtr intPtr)
        {
            return (NativeMethods.GetCursor() == intPtr);
        }

        /// <summary>
        /// Returns name for a cursor (typically a stock cursor).
        /// </summary>
        /// <param name="c">Cursor object.</param>
        /// <returns>String representing name of object.</returns>
        public static string GetName(Cursor c)
        {
            return s_cursorTypeConverter.ConvertTo(c, typeof(string)) as string;
        }

        private static TypeConverter s_cursorTypeConverter = TypeDescriptor.GetConverter(typeof(Cursor));
    }
}
