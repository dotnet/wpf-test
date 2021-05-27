// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;


namespace DRT
{
    // This suite ensures that the window class associated with each hwnd is cleaned up 
    // properly when the hwnd is destroyed.
    internal class WindowClassSuite : DrtTestSuite
    {
        #region DrtTestSuite

        public WindowClassSuite() : base("Window class suite")
        {
            Contact = "Microsoft";
        }


        public override DrtTest[] PrepareTests()
        {
            // return the list of tests to run against the tree
            return new DrtTest[] {
                new DrtTest(RunTest),
                new DrtTest(VerifyHwndDestruction)};
        }

        #endregion // DrtTestSuite

        private void RunTest()
        {
            int numTests = 2;

            _classAtoms = new List<ushort>(numTests);
            List<HwndSource> sources = new List<HwndSource>(numTests);
            HwndSourceParameters parameters = new HwndSourceParameters("Child Window", 5, 5);


            //
            // Create HwndSources and save their window class atoms
            //
            for (int i = 0; i < numTests; i++)
            {
                parameters.WindowName = "Child Window " + i.ToString();
                sources.Add(new HwndSource(parameters));
                _classAtoms.Add(GetClassAtom(sources[i]));
            }

            //
            // Destroy via a direct call to Dispose
            //
            sources[0].Dispose();


            //
            // Call destroy window manually
            //
            DestroyWindow(new HandleRef(null, sources[1].Handle));

            //
            // Destroy via the finalizer (i.e. on the wrong thread). 
            //
            // I can't test this case since we always hold on to the HwndSource
            // via the InputManager.

#if FinalizationOfHwndSourceWorks
             sources[2] = null;
             GC.Collect(GC.MaxGeneration);
             GC.WaitForPendingFinalizers();
#endif
        }

        private void VerifyHwndDestruction()
        {
            // Run through each class atom and attempt to query info on the class
            // If that fails, we know it's already been cleaned up.  
            // Note that we don't need to verify the windows are gone since a 
            // class can't be unregistered until all its windows are destroyed.

            int returnValue;
            WNDCLASSEX wndclass = new WNDCLASSEX();

            IntPtr hInstance = GetModuleHandle(null);

            if (hInstance == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            for (int i = 0; i < _classAtoms.Count; i++)
            {

                returnValue = GetClassInfoEx(hInstance, new IntPtr(_classAtoms[i]), wndclass);

                // A return value of 0 means that UnregisterClass failed
                DRT.Assert(returnValue == 0, "Error: Window class wasn't unregistered properly");
            }
        }

        #region Helper Methods

        /// <summary>
        /// This pokes into a given a HwndSource and extracts the window class atom for its hwnd
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private ushort GetClassAtom(HwndSource source)
        {
            object hwndWrapper;
            Type hwndWrapperType;

            // Grab WindowsBase and extract HwndWrapper's type
            Assembly windowsBase = Assembly.GetAssembly(typeof(DependencyObject));  // get WindowsBase
            hwndWrapperType = windowsBase.GetType("MS.Win32.HwndWrapper");

            // Get the HwndWrapper from the HwndSource
            FieldInfo hwndWrapperField = source.GetType().GetField("_hwndWrapper",
                BindingFlags.NonPublic | BindingFlags.Instance);

            hwndWrapper = hwndWrapperField.GetValue(source);

            // Extract the class atom from the hwndWrapper
            FieldInfo atomField = hwndWrapperType.GetField("_classAtom",
                BindingFlags.NonPublic | BindingFlags.Instance);

            return (ushort)atomField.GetValue(hwndWrapper);
        }

        #endregion

        #region Win32 declarations

        [DllImport("User32", EntryPoint = "DestroyWindow", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool DestroyWindow(HandleRef hWnd);

        [DllImport("Kernel32", EntryPoint = "GetModuleHandle", CharSet = CharSet.Auto, BestFitMapping = false, ThrowOnUnmappableChar = true, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string modName);


        [DllImport("User32", EntryPoint = "GetClassInfoEx", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
        private static extern int GetClassInfoEx(IntPtr hInstance, IntPtr atomString /*lpClassName*/, WNDCLASSEX wndclass);


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class WNDCLASSEX
        {
            public int cbSize = Marshal.SizeOf(typeof(WNDCLASSEX));
            public int style = 0;
            public WndProc lpfnWndProc = null;
            public int cbClsExtra = 0;
            public int cbWndExtra = 0;
            public IntPtr hInstance = IntPtr.Zero;
            public IntPtr hIcon = IntPtr.Zero;
            public IntPtr hCursor = IntPtr.Zero;
            public IntPtr hbrBackground = IntPtr.Zero;
            public string lpszMenuName = null;
            public string lpszClassName = null;
            public IntPtr hIconSm = IntPtr.Zero;
        }

        private delegate IntPtr WndProc(IntPtr hWnd, Int32 msg, IntPtr wParam, IntPtr lParam);

        #endregion // Win32 declarations

        #region Data

        List<ushort> _classAtoms;

        #endregion
    }

    

}
