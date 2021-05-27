// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Tests UsesPerPixelTransparency property
 * 
 *
********************************************************************/
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using System.Runtime.InteropServices;   // NativeMethods
using System.Windows.Interop;
using System.Collections.Generic;
namespace DRT
{

// Disable warning for obsolete fields as we still want to test them
#pragma warning disable 0612

    public class PerPixelTransparencySuite : DrtTestSuite
    {
        System.Windows.Controls.Border _rootBorder;

		public PerPixelTransparencySuite() : base("PerPixelTransparency Suite")
        {
            Contact = "Microsoft";
        }

        /// <summary>
        /// Returns True if the platform on which the test is running supports transparent WS_CHILD windows
        /// </summary>
        bool DoesPlatformSupportTransparentChildWindows
        {
            get
            {
                return System.Environment.OSVersion.Version >= new Version(6, 2);
            }
        }

        /// <summary>
        /// Creates a parent border to host the tests and returns the list of the tests
        /// </summary>
        /// <returns></returns>
        public override DrtTest[] PrepareTests()
        {
            _rootBorder = new System.Windows.Controls.Border();
            DRT.Show(_rootBorder);

            if (DRT.KeepAlive)
            {
                return new DrtTest[0];
            }
            else
            {
                return new DrtTest[] {
                    new DrtTest(CheckEffectiveOpacity),
                    new DrtTest(CheckResultingStyles),
                };
            }
        }

        /// <summary>
        /// Tests the EffectivePerPixelOpacity property of the HwndSourceParameters class
        /// </summary>
        private void CheckEffectiveOpacity()
        {
            TestOpacity(platformSupport : false, child : false, usesPerPixelOpacity : false, usesPerPixelTransparency : false, expectedEffectiveOpacity : false, expectedException : null);
            TestOpacity(platformSupport : false, child : false, usesPerPixelOpacity : true, usesPerPixelTransparency : false, expectedEffectiveOpacity : true, expectedException : null);
            TestOpacity(platformSupport : false, child : false, usesPerPixelOpacity : false, usesPerPixelTransparency : true, expectedEffectiveOpacity : true, expectedException : null);
            TestOpacity(platformSupport : false, child : false, usesPerPixelOpacity : true, usesPerPixelTransparency : true, expectedEffectiveOpacity : false, expectedException : typeof(InvalidOperationException));
            TestOpacity(platformSupport : false, child : true, usesPerPixelOpacity : false, usesPerPixelTransparency : false, expectedEffectiveOpacity : false, expectedException : null);
            TestOpacity(platformSupport : false, child : true, usesPerPixelOpacity : true, usesPerPixelTransparency : false, expectedEffectiveOpacity : false, expectedException : null);
            TestOpacity(platformSupport : false, child : true, usesPerPixelOpacity : false, usesPerPixelTransparency : true, expectedEffectiveOpacity : false, expectedException : null);
            TestOpacity(platformSupport : false, child : true, usesPerPixelOpacity : true, usesPerPixelTransparency : true, expectedEffectiveOpacity : false, expectedException : typeof(InvalidOperationException));
            TestOpacity(platformSupport : true, child : false, usesPerPixelOpacity : false, usesPerPixelTransparency : false, expectedEffectiveOpacity : false, expectedException : null);
            TestOpacity(platformSupport : true, child : false, usesPerPixelOpacity : true, usesPerPixelTransparency : false, expectedEffectiveOpacity : true, expectedException : null);
            TestOpacity(platformSupport : true, child : false, usesPerPixelOpacity : false, usesPerPixelTransparency : true, expectedEffectiveOpacity : true, expectedException : null);
            TestOpacity(platformSupport : true, child : false, usesPerPixelOpacity : true, usesPerPixelTransparency : true, expectedEffectiveOpacity : false, expectedException : typeof(InvalidOperationException));
            TestOpacity(platformSupport : true, child : true, usesPerPixelOpacity : false, usesPerPixelTransparency : false, expectedEffectiveOpacity : false, expectedException : null);
            TestOpacity(platformSupport : true, child : true, usesPerPixelOpacity : true, usesPerPixelTransparency : false, expectedEffectiveOpacity : false, expectedException : null);
            TestOpacity(platformSupport : true, child : true, usesPerPixelOpacity : false, usesPerPixelTransparency : true, expectedEffectiveOpacity : true, expectedException : null);
            TestOpacity(platformSupport : true, child : true, usesPerPixelOpacity : true, usesPerPixelTransparency : true, expectedEffectiveOpacity : false, expectedException : typeof(InvalidOperationException));

            // Restore the real platform support for Transparent Child Windows
            SetPlatformSupportsTransparentChildWindows(DoesPlatformSupportTransparentChildWindows);
        }

        /// <summary>
        /// Mocks different values for the support of WS_CHILD transparent windows by the platform
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// This cannot be used to create HwndSource because the Win32 API CreateWindow will fail under W7 and older versions
        /// </remarks>
        void SetPlatformSupportsTransparentChildWindows(bool value)
        {
            Type t = typeof(System.Windows.Interop.HwndSourceParameters);
            var method = t.GetMethod("SetPlatformSupportsTransparentChildWindowsForTestingOnly", BindingFlags.NonPublic | BindingFlags.Static);
            if (method != null)
            {
                method.Invoke(null, new object[] { value });
            }
            else
            {
                throw new Exception("SetPlatformSupportsTransparentChildWindowsForTestingOnly not found");
            }
        }

        /// <summary>
        /// Bypasses the "internal" aspect of EffectivePerPixelOpacity property
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        bool GetEffectivePerPixelOpacity(HwndSourceParameters p)
        {
            Type t = p.GetType();
            var property = t.GetProperty("EffectivePerPixelOpacity", BindingFlags.NonPublic | BindingFlags.Instance);
            if (property != null)
            {
                return (bool)property.GetValue(p);
            }
            else
            {
                throw new Exception("EffectivePerPixelOpacity property not found");
            }
        }

        /// <summary>
        /// Tests that EffectivePerPixelOpacity has the expected value
        /// </summary>
        /// <param name="platformSupport"></param>
        /// <param name="child"></param>
        /// <param name="usesPerPixelOpacity"></param>
        /// <param name="usesPerPixelTransparency"></param>
        /// <param name="expectedEffectiveOpacity"></param>
        /// <param name="expectedException"></param>
        void TestOpacity(bool platformSupport, bool child, bool usesPerPixelOpacity, bool usesPerPixelTransparency, bool expectedEffectiveOpacity, Type expectedException)
        {
            string description = string.Format("platformSupport={0}, child={1}, usesPerPixelOpacity={2}, usesPerPixelTransparency={3}", platformSupport, child, usesPerPixelOpacity, usesPerPixelTransparency);
            try
            {
                SetPlatformSupportsTransparentChildWindows(platformSupport);
                HwndSourceParameters p = new HwndSourceParameters();
                p.WindowStyle = child ? Win32.WS_CHILD : 0;
                p.UsesPerPixelOpacity = usesPerPixelOpacity;
                p.UsesPerPixelTransparency = usesPerPixelTransparency;
                DRT.Assert(GetEffectivePerPixelOpacity(p) == expectedEffectiveOpacity, description);
            }
            catch (TargetInvocationException ex)
            {
                // Caused by use of reflection to get EffectivePerPixelOpacity
                DRT.Assert(ex.InnerException.GetType() == expectedException, description);
            }
            catch (Exception ex)
            {
                DRT.Assert(ex.GetType() == expectedException, description);
            }
        }

        /// <summary>
        /// Creates the HwndSource and check the resulting style
        /// </summary>
        private void CheckResultingStyles()
        {
            TestHwndHost test = null;

            // First series with a top level window
            test = new TestHwndHost(child : false, usesPerPixelOpacity : false, usesPerPixelTransparency : false, style: 0);
            _rootBorder.Child = test;
            DRT.Assert(test.SourceCreated && (test.OutUsesPerPixelOpacity == false) && (test.LayeredWindow == false) && (test.ExceptionType == null), test.Description);

            test = new TestHwndHost(child : false, usesPerPixelOpacity : false, usesPerPixelTransparency : true, style: 0);
            _rootBorder.Child = test;
            DRT.Assert(test.SourceCreated && (test.OutUsesPerPixelOpacity == true) && (test.LayeredWindow == true) && (test.ExceptionType == null), test.Description);

            test = new TestHwndHost(child : false, usesPerPixelOpacity : true, usesPerPixelTransparency : false, style: 0);
            _rootBorder.Child = test;
            DRT.Assert(test.SourceCreated && (test.OutUsesPerPixelOpacity == true) && (test.LayeredWindow == true) && (test.ExceptionType == null), test.Description);

            test = new TestHwndHost(child : false, usesPerPixelOpacity : true, usesPerPixelTransparency : true, style: 0);
            _rootBorder.Child = test;
            DRT.Assert(!test.SourceCreated && (test.OutUsesPerPixelOpacity == false) && (test.LayeredWindow == false) && (test.ExceptionType == typeof(InvalidOperationException)), test.Description);


            // Second series with a child window
            test = new TestHwndHost(child : true, usesPerPixelOpacity : false, usesPerPixelTransparency : false, style: 0);
            _rootBorder.Child = test;
            DRT.Assert(test.SourceCreated && (test.OutUsesPerPixelOpacity == false) && (test.LayeredWindow == false) && (test.ExceptionType == null), test.Description);

            // This is the only test where W7 and W8 should differ
            test = new TestHwndHost(child : true, usesPerPixelOpacity : false, usesPerPixelTransparency : true, style: 0);
            _rootBorder.Child = test;
            DRT.Assert(test.SourceCreated && (test.OutUsesPerPixelOpacity == DoesPlatformSupportTransparentChildWindows) && (test.LayeredWindow == DoesPlatformSupportTransparentChildWindows) && (test.ExceptionType == null), test.Description);

            test = new TestHwndHost(child : true, usesPerPixelOpacity : true, usesPerPixelTransparency : false, style: 0);
            _rootBorder.Child = test;
            DRT.Assert(test.SourceCreated && (test.OutUsesPerPixelOpacity == false) && (test.LayeredWindow == false) && (test.ExceptionType == null), test.Description);

            test = new TestHwndHost(child : true, usesPerPixelOpacity : true, usesPerPixelTransparency : true, style: 0);
            _rootBorder.Child = test;
            DRT.Assert(!test.SourceCreated && (test.OutUsesPerPixelOpacity == false) && (test.LayeredWindow == false) && (test.ExceptionType == typeof(InvalidOperationException)), test.Description);
        }

	}

    /// <summary>
    /// Win32 helper class
    /// </summary>
    class Win32
    {
        public const int WS_EX_LAYERED  = 0x00080000;
        public const int WS_VISIBLE     = 0x10000000;
        public const int WS_CHILD       = 0x40000000;

        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MessageBox(IntPtr hWnd, String text, String caption, int options);
    }

    /// <summary>
    /// Host providing the required parent HWND for hosting WS_CHILD window
    /// </summary>
    class TestHwndHost : HwndHost
    {


        bool _child;
        bool _usesPerPixelOpacity;
        bool _usesPerPixelTransparency;
        int _style;

        HwndSource _childSource = null;
        Type _exceptionType = null;
        bool _layeredWindow = false;
        bool _outUsesPerPixelOpacity = false;
        bool _sourceCreated = false;

        /// <summary>
        /// Constructor: memorizes the parameters to use them in the overriden method
        /// </summary>
        /// <param name="child"></param>
        /// <param name="usesPerPixelOpacity"></param>
        /// <param name="usesPerPixelTransparency"></param>
        /// <param name="style"></param>
        public TestHwndHost(bool child, bool usesPerPixelOpacity, bool usesPerPixelTransparency, int style)
        {
            _child = child;
            _usesPerPixelOpacity = usesPerPixelOpacity;
            _usesPerPixelTransparency = usesPerPixelTransparency;
            _style = style;
        }

        /// <summary>
        /// Exception raised when creating the HwndSource if any
        /// </summary>
        public Type ExceptionType
        {
            get
            {
                return _exceptionType;
            }
        }

        /// <summary>
        /// true if the created HWND has the WS_EX_LAYERED style
        /// </summary>
        public bool LayeredWindow
        {
            get
            {
                return _layeredWindow;
            }
        }

        /// <summary>
        /// true if the HwndSource has been successfully created
        /// </summary>
        public bool SourceCreated
        {
            get
            {
                return _sourceCreated;
            }
        }

        /// <summary>
        /// returns the UsesPerPixelOpacity of the created HwndSource
        /// </summary>
        public bool OutUsesPerPixelOpacity
        {
            get
            {
                return _outUsesPerPixelOpacity;
            }
        }

        /// <summary>
        /// Real test method: creates the HwndSource with the given parameters and checks the resulting style
        /// </summary>
        /// <param name="hwndParent"></param>
        /// <returns></returns>
        protected override System.Runtime.InteropServices.HandleRef BuildWindowCore(System.Runtime.InteropServices.HandleRef hwndParent)
        {
            // We always create two windows: the first one always a child from hwndParent, in order to be a "good citizen"
            // And the second one, a child or dependent of the first child

            try
            {
                HwndSourceParameters p = new HwndSourceParameters();
                p.WindowStyle = Win32.WS_CHILD;
                p.ParentWindow = hwndParent.Handle;
                _childSource = new HwndSource(p);

                p = new HwndSourceParameters();
                p.ExtendedWindowStyle = _style;
                p.WindowStyle = Win32.WS_VISIBLE;
                if (_child) p.WindowStyle |= Win32.WS_CHILD;
                p.UsesPerPixelOpacity = _usesPerPixelOpacity;
                p.UsesPerPixelTransparency = _usesPerPixelTransparency;
                p.ParentWindow = _childSource.Handle;

                HwndSource source = new HwndSource(p);
                // Read the Window Style
                if (source.Handle != IntPtr.Zero)
                {
                    int exStyle = Win32.GetWindowLong(source.Handle, Win32.GWL_EXSTYLE);
                    _layeredWindow = ((exStyle & Win32.WS_EX_LAYERED) == Win32.WS_EX_LAYERED);
                    _sourceCreated = true;
                    _outUsesPerPixelOpacity = source.UsesPerPixelOpacity;
                }
                source.Dispose();
            }
            catch (Exception ex)
            {
                _exceptionType = ex.GetType();
            }
            return new HandleRef(this, _childSource.Handle);
        }

        /// <summary>
        /// Destroys the hosted window
        /// </summary>
        /// <param name="hwnd"></param>
        protected override void DestroyWindowCore(System.Runtime.InteropServices.HandleRef hwnd)
        {
            if (_childSource != null)
            {
                _childSource.Dispose();
            }
        }

        /// <summary>
        /// Helper for test result
        /// </summary>
        public string Description
        {
            get
            {
                return string.Format("Input: WS_CHILD={0} UsesPerPixelOpacity={1} UsesPerPixelTransparency={2} style={3} (Platform={4})\nOutput: SourceCreated={5} LayeredWindow={6} UsesPerPixelOpacity={7}",
                                     _child, _usesPerPixelOpacity, _usesPerPixelTransparency, _style, Environment.OSVersion.Version.ToString(),
                                     SourceCreated, LayeredWindow, OutUsesPerPixelOpacity);
            }
        }
    }
}
