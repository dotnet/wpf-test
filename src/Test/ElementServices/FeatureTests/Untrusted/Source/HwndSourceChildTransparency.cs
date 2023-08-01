// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Visual verification of HwndSources
 * 
 * Contributor: alainza
 *
 
  
 * Revision:         $Revision$
 
 * Filename:         $Source$
********************************************************************/

using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput;
using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Win32;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.RenderingVerification.UnmanagedProxies;

namespace Avalon.Test.CoreUI.Hosting
{
// Disable warning for obsolete fields as we still want to test them
#pragma warning disable 0612

    public abstract class HwndSourceChildTransparency : TestCase
    {
        HwndSource _parentWindow;
        HwndSource _childWindow;

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

        protected abstract bool Opacity { get;}
        protected abstract bool Transparency { get; }
        protected abstract string MasterName { get; }

        public const string OpaqueBackground = "HwndSourcePerPixelTransparencyOpaque.png";
        public const string TransparentBackground = "HwndSourcePerPixelTransparencyTransparent.png";

        public override void Run()
        {
            CoreLogger.BeginVariation();
            using (CoreLogger.AutoStatus("Test has started"))
            {
                if (!DoesPlatformSupportTransparentChildWindows)
                {
                    CoreLogger.LogTestResult(true, "Ignore, tests only applicable to Win8+.");
                }
                else
                {
                    // Create HwndSource
                    CreateWindow(Opacity, Transparency);

                    try
                    {
                        //IImageAdapter img2 = VScanHelper.Capture(_parentWindow.Handle, true);
                        //IImageAdapterUnmanaged test = img2 as IImageAdapterUnmanaged;
                        //test.Save(MasterName);

                        if (VScanHelper.Compare(MasterName, _parentWindow.Handle))
                        {
                            CoreLogger.LogTestResult(true, "Pass");
                        }
                        else
                        {
                            CoreLogger.LogTestResult(false, "Fail, images did not match.");
                        }
                    }
                    catch (Exception ex)
                    {
                        CoreLogger.LogTestResult(false, "Fail, unable to compare images:" + ex.Message);
                    }
                    CloseWindow();
                }
            }
            CoreLogger.EndVariation();

        }

        private void CreateWindow(bool usesPerPixelOpacity, bool usesPerPixelTransparency)
        {
            // 1. Create the HostWindow
            HwndSourceParameters p = new HwndSourceParameters();
            p.Width = 500;
            p.Height = 400;
            p.WindowStyle = _WS_OVERLAPPEDWINDOW;
            p.WindowName = "TestWindow";
            _parentWindow = new HwndSource(p);
            ShowWindow(_parentWindow.Handle, SW_SHOW);

            Rectangle root = new Rectangle();
            root.Fill = new SolidColorBrush(Colors.LightBlue);
            _parentWindow.RootVisual = root;

            // 2. Create the Child
            p.Width = 200;
            p.Height = 200;
            p.PositionX = 100;
            p.PositionY = 100;
            p.WindowStyle = WS_CHILD | WS_VISIBLE;
            p.WindowName = "ctrl";
            p.UsesPerPixelOpacity = usesPerPixelOpacity;
            p.UsesPerPixelTransparency = usesPerPixelTransparency;
            p.ParentWindow = _parentWindow.Handle;

            _childWindow = new HwndSource(p);

            root = new Rectangle();
            var gradient = new LinearGradientBrush(Colors.Transparent, Colors.Navy, 90.0);
            root.Fill = gradient;
            _childWindow.RootVisual = root;
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(2000);
        }

        private void CloseWindow()
        {
            _childWindow.Dispose();
            _parentWindow.Dispose();
        }

        #region Win32 interop
        private const int WS_OVERLAPPED = 0;
        private const int WS_BORDER = 0x00800000;
        private const int WS_CAPTION = 0x00C00000;
        private const int WS_MAXIMIZEBOX = 0x00010000;
        private const int WS_MINIMIZEBOX = 0x00020000;
        private const int WS_SYSMENU = 0x00080000;
        private const int WS_THICKFRAME = 0x00040000;
        private const int WS_CLIPCHILDREN = 0x02000000;
        private const int WS_CHILD = 0x40000000;
        private const int WS_VISIBLE = 0x10000000;

        private readonly int _WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX | WS_CLIPCHILDREN;

        private const int SW_SHOW = 5;

        [DllImport("user32.dll")]
        private extern static int ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MessageBox(IntPtr hWnd, String text, String caption, int options);
        #endregion
    }

    [Test(0, @"HwndSourceChildTransparency", TestCaseSecurityLevel.FullTrust, "HwndSourceNoPerPixelTransparency", Area = "W8Features", Disabled = false,
        SupportFiles = @"FeatureTests\ElementServices\HwndSourcePerPixelTransparencyOpaque.png")]
    public class HwndSourceNoPerPixelTransparency : HwndSourceChildTransparency
    {
        protected override bool Opacity { get { return false; } }
        protected override bool Transparency { get { return false; } }
        protected override string MasterName { get { return HwndSourceChildTransparency.OpaqueBackground; } }
    }

    [Test(0, @"HwndSourceChildTransparency", TestCaseSecurityLevel.FullTrust, "HwndSourcePerPixelOpacity", Area = "W8Features", Disabled = false,
        SupportFiles = @"FeatureTests\ElementServices\HwndSourcePerPixelTransparencyOpaque.png")]
    public class HwndSourcePerPixelOpacity : HwndSourceChildTransparency
    {
        protected override bool Opacity { get { return true; } }
        protected override bool Transparency { get { return false; } }
        protected override string MasterName { get { return HwndSourceChildTransparency.OpaqueBackground; } }
    }

    [Test(0, @"HwndSourceChildTransparency", TestCaseSecurityLevel.FullTrust, "HwndSourcePerPixelTransparency", Area = "W8Features", Disabled = false,
        SupportFiles = @"FeatureTests\ElementServices\HwndSourcePerPixelTransparencyOpaque.png, FeatureTests\ElementServices\HwndSourcePerPixelTransparencyTransparent.png")]
    public class HwndSourcePerPixelTransparency : HwndSourceChildTransparency
    {
        protected override bool Opacity { get { return false; } }
        protected override bool Transparency { get { return true; } }
        protected override string MasterName {
            get
            {
                // Check if current platform supports transparent WS_CHILD Windows
                if (System.Environment.OSVersion.Version >= new Version(6, 2))
                {
                    return HwndSourceChildTransparency.TransparentBackground;
                }
                else
                {
                    return HwndSourceChildTransparency.OpaqueBackground;
                }
            }
        }
    }
}
