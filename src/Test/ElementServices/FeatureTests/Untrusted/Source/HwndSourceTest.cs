// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Markup;
using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Avalon.Test.CoreUI;
using System.Threading;
using System.Windows.Threading;
using SysInfo=System.Windows.Forms.SystemInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Documents;
using System.Windows.Media;
using Avalon.Test.CoreUI.Common;

using System.Runtime.InteropServices;
using Avalon.Test.CoreUI.Source;
using Microsoft.Test.Discovery;
using Microsoft.Test.Win32;
using Microsoft.Test.Threading;

namespace Avalon.Test.CoreUI.Source.Hwnd
{
    /// <summary>
    /// Verify basic construction works.     
    /// </summary>
    [TestDefaults]
    public class HwndSourceTest
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public HwndSourceTest()
        {
        }

        /// <summary>
        /// Verify HwndSource.CreateHandleRef().
        /// </summary>
        [TestAttribute(1, @"Source\HwndSource", TestCaseSecurityLevel.FullTrust, "VerifyHwndSourceCreateHandleRef", Description = "Verify HwndSource.CreateHandleRef()", Area = "AppModel")]
        public void VerifyHwndSourceCreateHandleRef()
        {
            CoreLogger.BeginVariation();
            // Create and dispose HwndSource.
            HwndSource hwndSource = _ConstructHwndSource(
                  100, 100, // x, y
                  400, 400, // width, height
                  false);   // adjustSizingForNonClientArea

            hwndSource.RootVisual = _ConstructRootVisual(Brushes.Red);

            HandleRef handleRef1 = hwndSource.CreateHandleRef();
            hwndSource.Dispose();
            HandleRef handleRef2 = hwndSource.CreateHandleRef();

            // Create another HwndSource.
            HwndSource hwndSource2 = _ConstructHwndSource(
                  500, 100, // x, y
                  400, 400, // width, height
                  false);   // adjustSizingForNonClientArea

            hwndSource2.RootVisual = _ConstructRootVisual(Brushes.Green);

            HandleRef handleRef3 = hwndSource.CreateHandleRef();

            // Briefly run the dispatcher.
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.SystemIdle,
                new DispatcherOperationCallback(_VerifyHandleRef),
                new HandleRef[] { handleRef1, handleRef2, handleRef3 });

            // Run dispatcher.
            Dispatcher.Run();
            CoreLogger.EndVariation();
        }

        private object _VerifyHandleRef(object obj)
        {
            HandleRef[] handleRefs = (HandleRef[])obj;

            DispatcherHelper.DoEvents(500);

            // Collect.
            GC.GetTotalMemory(true);
            GC.WaitForPendingFinalizers();

            // Verify handle is not null.
            CoreLogger.LogStatus("Verifying handle on first hwnd, retrieved before it was disposed...");
            if (HandleRef.ToIntPtr(handleRefs[0]) == IntPtr.Zero)
            {
                throw new Microsoft.Test.TestValidationException("HwndSource.CreateHandleRef() returned a null handle.");
            }

            // Verify post-disposed handle is null.
            CoreLogger.LogStatus("Verifying handle on first hwnd, retrieved after it was disposed...");
            if (HandleRef.ToIntPtr(handleRefs[1]) != IntPtr.Zero)
            {
                throw new Microsoft.Test.TestValidationException("HwndSource.CreateHandleRef() returned a non-null handle on a disposed HwndSource.");
            }

            //  Verify handles are unique.
            CoreLogger.LogStatus("Verifying handles on two HwndSource instances are unique...");
            if (HandleRef.ToIntPtr(handleRefs[0]) == HandleRef.ToIntPtr(handleRefs[2]))
            {
                throw new Microsoft.Test.TestValidationException("HwndSource.CreateHandleRef() returned a same handle for multiple windows.");
            }

            Dispatcher.CurrentDispatcher.InvokeShutdown();

            return null;
        }

        /// <summary>
        /// Verify basic HwndSourceParameters (in)equality and GetHashCode().
        /// </summary>
        [Test(1, @"Source\HwndSourceParameters", TestCaseSecurityLevel.FullTrust, "VerifyHwndSourceParameterInequality", Description="Verify basic HwndSourceParameters (in)equality and GetHashCode().", Area = "AppModel")]
        public void VerifyHwndSourceParameters()
        {
            CoreLogger.BeginVariation();
            // Create HwndSource1 
            HwndSourceParameters hwndParams1 = new HwndSourceParameters(
                "Avalon Test HwndSource 1", 400, 400);
            hwndParams1.SetPosition(100, 100); // x, y
            hwndParams1.WindowStyle |= NativeConstants.WS_VISIBLE | NativeConstants.WS_OVERLAPPEDWINDOW | NativeConstants.WS_CLIPCHILDREN;

            HwndSource hwndSource1 = new HwndSource(hwndParams1);
            Visual visual1 = _ConstructRootVisual(Brushes.Red);
            hwndSource1.RootVisual = visual1;

            // Create HwndSource2
            HwndSourceParameters hwndParams2 = new HwndSourceParameters(
                "Avalon Test HwndSource 2", 400, 400);
            hwndParams2.SetPosition(500, 100); // x, y
            hwndParams2.WindowStyle |= NativeConstants.WS_VISIBLE | NativeConstants.WS_OVERLAPPEDWINDOW | NativeConstants.WS_CLIPCHILDREN;

            HwndSource hwndSource2 = new HwndSource(hwndParams2);
            Visual visual2 = _ConstructRootVisual(Brushes.Green);
            hwndSource2.RootVisual = visual2;

            //
            // Verify HwndSourceParameters (in)equality and GetHashCode().
            //
            if (hwndParams1.Equals(hwndParams2) || hwndParams1.Equals((object)hwndParams2) ||
                hwndParams1 == hwndParams2 || !(hwndParams1 != hwndParams2) || 
                hwndParams1.GetHashCode() != hwndParams2.GetHashCode())
            {
                throw new Microsoft.Test.TestValidationException("First HwndSourceParameters is unexpectedly equal to second HwndSourceParameters.");
            }

            // Briefly run the dispatcher.
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.SystemIdle,
                (DispatcherOperationCallback)delegate(object obj)
                {
                    // Wait a short time to make debugging easier.
                    DispatcherHelper.DoEvents(500);

                    Dispatcher.CurrentDispatcher.InvokeShutdown();

                    return null;
                },
                null);

            // Run dispatcher.
            Dispatcher.Run();
            CoreLogger.EndVariation();
        }

        /// <summary>
        /// Verify basic HwndSource construction with adjustSizingForNonClientArea parameter.
        /// </summary>
        [Test(1, @"Source\HwndSource", TestCaseSecurityLevel.FullTrust, "AdjustNonClientAreaSize", Description = "Verify basic HwndSource construction with adjustSizingForNonClientArea parameter.", Area = "AppModel")]
        public void VerifyAdjustSizingForNonClientArea()
        {
            CoreLogger.BeginVariation();
            System.Drawing.Size size = SysInfo.FrameBorderSize;
            int frameBorderSize = size.Width + size.Height;

            // Create HwndSource with
            // adjustSizingForNonClientArea = false
            HwndSource hwndSource = _ConstructHwndSource(
                  100, 100, // x, y
                  400, 400, // width, height
                  false);   // adjustSizingForNonClientArea

            Visual visual_NonAdjustForFrame = _ConstructRootVisual(Brushes.Red);
            hwndSource.RootVisual = visual_NonAdjustForFrame;

            // Create HwndSource with
            // adjustSizingForNonClientArea = true
            hwndSource = _ConstructHwndSource(
                  500, 100, // x, y
                  400, 400, // width, height
                  true);    // adjustSizingForNonClientArea

            Visual visual_AdjustForFrame = _ConstructRootVisual(Brushes.Green);
            hwndSource.RootVisual = visual_AdjustForFrame;

            //
            // Verify size asynchronously.
            //
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.SystemIdle,
                (DispatcherOperationCallback)delegate(object obj)
                {
                    // Wait a short time to make debugging easier.
                    DispatcherHelper.DoEvents(500);

                    int nonAdjustWidth = (int)((FrameworkElement)visual_NonAdjustForFrame).ActualWidth;
                    int adjustWidth = (int)((FrameworkElement)visual_AdjustForFrame).ActualWidth;

                    CoreLogger.LogStatus("NonAdjust actual width: " + nonAdjustWidth);
                    CoreLogger.LogStatus("Adjust actual width: " + adjustWidth);
                    CoreLogger.LogStatus("Frame border size: " + frameBorderSize);

                    // Check that the difference between the 2 windows content size
                    // is the frame border size.
                    if (nonAdjustWidth + frameBorderSize != adjustWidth)
                    {
                        throw new Microsoft.Test.TestValidationException("adjustSizingForNonClientArea parameter of HwndSource constructor wasn't respected.");
                    }

                    Dispatcher.CurrentDispatcher.InvokeShutdown();

                    return null;
                }, 
                null);

            // Run dispatcher.
            Dispatcher.Run();
            CoreLogger.EndVariation();
        }

        private Visual _ConstructRootVisual(Brush brush)
        {
            Border border = new Border();
            border.BorderBrush = brush;
            border.BorderThickness = new Thickness(5);
            border.Background = Brushes.White;

            return border;
        }

        private HwndSource _ConstructHwndSource(
                  int x, 
                  int y, 
                  int width, 
                  int height,  
                  bool adjustSizingForNonClientArea)
        {

            HwndSource hwndSource = new HwndSource(
                  0, 
                  NativeConstants.WS_VISIBLE | NativeConstants.WS_OVERLAPPEDWINDOW | NativeConstants.WS_CLIPCHILDREN, 
                  0, 
                  x, 
                  y, 
                  width, 
                  height,
                  "Avalon Test HwndSource", 
                  IntPtr.Zero /*parent*/, 
                  adjustSizingForNonClientArea);

            return hwndSource;
        }
    }
}



