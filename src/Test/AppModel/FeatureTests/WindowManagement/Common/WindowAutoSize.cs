// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Reflection;
using System.Windows;
using System.ComponentModel;
using System.Windows.Navigation;
using System.Windows.Input;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Windows.Automation;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Test.Win32;
using Microsoft.Windows.Test.Client.AppSec.BVT.ELCommon;
using MTI = Microsoft.Test.Input;


namespace WindowTest
{

/// <summary>
/// Create HwndSource and Add RootVisualHwndSource
/// Perform AutoSize tests for cases SizeToContent.Width, SizeToContect.Height, SizeToContent.WidthAndHeight
/// and SizeToContent.Manual
/// </summary>
public partial class WindowAutoSize
{
    private Window _window;
    private Canvas _root;

    private static double s_mainWindowWidth = 700;
    private static double s_mainWindowHeight = 700;

    private static double s_rootWidth = 300;
    private static double s_rootHeight = 200;

    private IntPtr _handle;

    private Thread _controlThread;

    // Type of test to perform
    private enum TestSizeToContent
    {
        Width,              // Width is sized to content
        Height,             // Height is sized to content
        WidthAndHeight,     // Both height and width are sized to content
        Manual,             // SizeToContent is manual
        None                // Initializer
    }

    private TestSizeToContent _testSizeToContent = TestSizeToContent.None;

    private void OnAppStartup(object sender, StartupEventArgs e)
    {
        Logger.Status("App OnAppStartup event caught\n");

        if (TestHelper.Current.AppArgs[1].ToString() == string.Empty)
        {
            Logger.LogFail("WindowAutoSize - Test type is not provided");
            return;
        }

        switch (TestHelper.Current.AppArgs[1].ToString())
        {
            case "1":
                _testSizeToContent = TestSizeToContent.Width;
                Logger.Status("Performing the test WindowAutoSize - Width is sized to content");
                break;
            case "2":
                _testSizeToContent = TestSizeToContent.Height;
                Logger.Status("Performing the test WindowAutoSize - Height is sized to content");
                break;
            case "3":
                _testSizeToContent = TestSizeToContent.WidthAndHeight;
                Logger.Status("Performing the test WindowAutoSize - Both height and width are sized to content");
                break;
            case "4":
                _testSizeToContent = TestSizeToContent.Manual;
                Logger.Status("Performing the test WindowAutoSize - SizeToContent is manual");
                break;
            default:
                Logger.LogFail("Invalid test case found");
                return;
        }

        _window = new Window();
        _window.Top = 0;
        _window.Left = 0;
        _window.Title = "Window SizeToContent test";

        switch (_testSizeToContent)
        {
            case TestSizeToContent.Width:
                // Testing SizeToContent.Width 
                _window.SizeToContent = SizeToContent.Width;
                break;

            case TestSizeToContent.Height:
                // Testing SizeToContent.Height 
                _window.SizeToContent = SizeToContent.Height;
                break;

            case TestSizeToContent.WidthAndHeight:
                // Testing SizeToContent.WidthAndHeight 
                _window.SizeToContent = SizeToContent.WidthAndHeight;
                break;

            case TestSizeToContent.Manual:
                // Testing SizeToContent.Manual 
                _window.SizeToContent = SizeToContent.Manual;
                break;
        }

        // and setting Width should not be effective 
        _window.Width = s_mainWindowWidth;  // 700

        // but setting Height should work fine
        _window.Height = s_mainWindowHeight;  // 700

        _root = new Canvas();
        // Set root's width and height 
        _root.Width = s_rootWidth;  // 300
        _root.Height = s_rootHeight;    //200

        _root.Background = new SolidColorBrush(Colors.Blue);
        _window.Content = _root;

        _window.Show();
        _handle = (new WindowInteropHelper(_window)).Handle;

        // Create a new thread
        _controlThread = new Thread(new ThreadStart(ControlThreadEntry));

        Logger.Status("Start FlowControl Thread\n");
        _controlThread.Start();
    }

    // *********************************
    // FlowControl Thread entry function
    // *********************************
    public void ControlThreadEntry()
    {
        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (DispatcherOperationCallback)delegate(object o)
        {
            InitializeHwndSourceAndVerify();
            return null;

        }, null);
    }

    private void InitializeHwndSourceAndVerify()
    {
        Logger.Status("Entering InitializeHwndSourceAndVerify");

        // Get hwndSource from PresentationSource
        HwndSource hwndSource = PresentationSource.FromVisual(_root) as HwndSource;
        if (hwndSource == null)
        {
            Logger.LogFail("hwndSource is NULL");
            return;
        }

        RECT rect = new RECT();
        GetClientRect(_handle, ref rect);

        Point visualClientSizeLogicalUnits = hwndSource.CompositionTarget.TransformFromDevice.Transform(
            new Point(rect.right - rect.left, rect.bottom - rect.top));

        GetWindowRect(_handle, ref rect);

        Point visualWindowSizeLogicalUnits = hwndSource.CompositionTarget.TransformFromDevice.Transform(
            new Point(rect.right - rect.left, rect.bottom - rect.top));

        // Verifying Window Size
        VerifyWindowSize(visualClientSizeLogicalUnits, visualWindowSizeLogicalUnits, 
                                               s_rootWidth, s_rootHeight, s_mainWindowWidth, s_mainWindowHeight);

        // Clean up test and quit app
        Logger.Status("Exiting InitializeHwndSourceAndVerify");

        TestHelper.Current.TestCleanup();
    }

    private void VerifyWindowSize(Point visualClientSizeLogicalUnits, Point visualWindowSizeLogicalUnits, 
                                  double contentWidth, double contentHeight, double windowWidth, double windowHeight)
    {
        Logger.Status("Entering VerifyWindowSize");

        switch (_window.SizeToContent)
        {
            // contentWidth should be respected. 
            // contentHeight is ignored
            // window.Width should reflect contentWidth
            // window.Height should be respected.
            case SizeToContent.Width:
                if (!TestUtil.IsEqual(visualClientSizeLogicalUnits.X, contentWidth))
                {
                    Logger.LogFail(String.Format("Setting SizeToContent to Width was not effective when setting SizeToContent to Width -- visualClientSizeLogcialUnits.X = {0}, contentWidth = {1}", visualClientSizeLogicalUnits.X, contentWidth));
                }

                if (!TestUtil.IsEqual(_window.ActualWidth, visualWindowSizeLogicalUnits.X))
                {
                    Logger.LogFail(String.Format("Window.Width didn't reflect content's size when setting SizeToContent to Width -- _win.ActualWidth = {0}, visualWindowSizeLogicalUnits.X = {1}", _window.ActualWidth, visualWindowSizeLogicalUnits.X));
                }

                if (!TestUtil.IsEqual(_window.ActualHeight, windowHeight) || !TestUtil.IsEqual(visualWindowSizeLogicalUnits.Y, windowHeight))
                {
                    Logger.LogFail(String.Format("Setting Window.Height was not effective when SizeToContent is Width -- _win.ActualHeight = {0}, windowHeight = {1}, visualWindowSizeLogicalUnits.Y = {2}", _window.ActualHeight, windowHeight, visualWindowSizeLogicalUnits.Y));
                }

                break;

            // contentWidth is ignored
            // contentHeight should be respected. 
            // window.Width should be respected
            // window.Height should reflect contentHeight                
            case SizeToContent.Height:
                if (!TestUtil.IsEqual(visualClientSizeLogicalUnits.Y, contentHeight))
                {
                    Logger.LogFail("Setting SizeToContent to Height was not effective when setting SizeToContent to Height");
                }

                if (!TestUtil.IsEqual(_window.ActualHeight, visualWindowSizeLogicalUnits.Y))
                {
                    Logger.LogFail("Window.Height didn't reflect content's size when setting SizeToContent to Height");
                }

                if (!TestUtil.IsEqual(_window.ActualWidth, windowWidth) || !TestUtil.IsEqual(visualWindowSizeLogicalUnits.X, windowWidth))
                {
                    Logger.LogFail("Setting Window.Width was not effective when SizeToContent is Height");
                }

                break;

            // contentWidth and contentHeight should both be respected
            // window.Width and window.Height should reflect content size
            case SizeToContent.WidthAndHeight:
                if (!TestUtil.IsEqual(visualClientSizeLogicalUnits.X, contentWidth))
                {
                    Logger.LogFail(String.Format("Window client width not equal to contentWidth when setting SizeToContent to WidthAndHeight -- visualClientSizeLogicalUnits.X = {0}, contentWidth = {1}", visualClientSizeLogicalUnits.X, contentWidth));
                }

                if (!TestUtil.IsEqual(visualClientSizeLogicalUnits.Y, contentHeight))
                {
                    Logger.LogFail(String.Format("Window client height not equal to contentHeight when Setting SizeToContent to WidthAndHeight -- visualClientSizeLogicalUnits.Y = {0}, contentHeight = {1}", visualClientSizeLogicalUnits.Y, contentHeight));
                }

                if (!TestUtil.IsEqual(_window.ActualWidth, visualWindowSizeLogicalUnits.X))
                {
                    Logger.LogFail(String.Format("Window.Width didn't reflect content's size when setting SizeToContent to WidthAndHeight -- _win.ActualWidth = {0}, visualWindowSizeLogicalUnits.X = {1}", _window.ActualWidth, visualWindowSizeLogicalUnits.X));
                }

                if (!TestUtil.IsEqual(_window.ActualHeight, visualWindowSizeLogicalUnits.Y))
                {
                    Logger.LogFail(String.Format("Window.Height didn't reflect content's size when setting SizeToContent to WidthAndHeight -- _win.ActualHeight = {0}, visualWindowSizeLogicalUnits.Y = {1}", _window.ActualHeight, visualWindowSizeLogicalUnits.Y));
                }

                break;

            // contentWidth is ignored
            // contentHeight is ignored
            // window.Width should reflect Visual Width
            // window.Height should refect visual height                
            case SizeToContent.Manual:
                if (!TestUtil.IsEqual(_window.ActualWidth, visualWindowSizeLogicalUnits.X) || !TestUtil.IsEqual(_window.ActualWidth, windowWidth))
                {
                    Logger.LogFail("Window.Width was not effective after setting SizeToContent to None");
                }

                if (!TestUtil.IsEqual(_window.ActualHeight, visualWindowSizeLogicalUnits.Y) || !TestUtil.IsEqual(_window.ActualHeight, windowHeight))
                {
                    Logger.LogFail("Window.Height was not effective after setting SizeToContent to None");
                }

                break;
        }

        Logger.Status("Exiting VerifyWindowSize");
    }

    private struct RECT
    {
        public uint left;
        public uint top;
        public uint right;
        public uint bottom;
    }

    [DllImport("user32.dll")]
    private static extern bool GetClientRect(IntPtr hwnd, ref RECT rect);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hwnd, ref RECT rect);

}

}
