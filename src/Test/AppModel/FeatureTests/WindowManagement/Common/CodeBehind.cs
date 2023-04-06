// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Navigation;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Threading;
using MTI = Microsoft.Test.Input;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders;
using Microsoft.Windows.Test.Client.AppSec.BVT.ELCommon;


namespace WindowTest
{
    public static class ResourceString
    {
        private static string s_dir = "Resources\\";
        public static string MasterImage_TransparentWindow = s_dir + "TransparentWindow_MasterImage.bmp";
        public static string MasterImage_BlueWindow = s_dir + "Blue_MasterImage.bmp";
        public static string MasterImage_BlueWindowRTL = s_dir + "RTL_Blue_MasterImage.bmp";
        public static string MasterImage_WhiteWindow = s_dir + "White_MasterImage.bmp";
        public static string ToleranceFile = s_dir + "DefaultTolerance.xml";
        public static string MasterImage_IconDefault = s_dir + "Icon_AppDefault_MasterImage.bmp";
        public static string MasterImage_Icon16x16_32 = s_dir + "TestIcon_16x16_32_MasterImage.bmp";
        public static string Icon_TestIcon_Embedded = s_dir + "TestIcon_Embedded.ico";
        public static string Icon_TestIcon_Loose = s_dir + "TestIcon_Loose.ico";
        public static string Icon_TestIconPNG_Loose = s_dir + "TestIcon_Loose.PNG";
        public static string TestPage1 = s_dir + "TestPage1.xaml";
        public static string TestPage2 = s_dir + "TestPage2.xaml";
    }

    /// <summary>
    /// </summary>
    public partial class REGRESSION_LayoutIssueWhenTurnOffSTC
    {
        AutomationHelper _AH = new AutomationHelper();
        double _expectedWidth,_expectedHeight;

        void OnContentRendered(object sender, EventArgs e)
        {
            _expectedWidth = btn.ActualWidth;
            _expectedHeight = btn.ActualHeight;

            Point bottomRightDevicePixels = TestUtil.LogicalToDeviceUnits(new Point(this.ActualWidth, this.ActualHeight), this);

            _AH.WaitThenMoveToAndClick(new Point(bottomRightDevicePixels.X - AutomationHelper.ResizeGripPixel, bottomRightDevicePixels.Y - AutomationHelper.ResizeGripPixel), Validate);
        }

        void Validate()
        {
            Logger.Status("ExpectedWidth=" + _expectedWidth.ToString() + " ExpectedHeight=" + _expectedHeight.ToString() + " SizeToContent=Manual");
            if (btn.ActualWidth != _expectedWidth ||
                btn.ActualHeight != _expectedHeight ||
                this.SizeToContent != SizeToContent.Manual)
            {
                Logger.LogFail("ActualWidth=" + this.ActualWidth.ToString() + " ActualHeight=" + this.ActualHeight.ToString() + " SizeToContent=" + this.SizeToContent.ToString());
            }
            else
            {
                Logger.LogPass("Passed");
            }
            this.Close();
        }
    }

    /// <summary>
    /// </summary>
    public partial class REGRESSION_WindowRenderAfterMoveOffScreen
    {
        System.Timers.Timer _timer;
        double _expectedWidth = 200;
        double _expectedHeight = 200;

        void OnLoaded(object sender, EventArgs e)
        {
            _timer = new System.Timers.Timer(9000);
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            _timer.Start();
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _timer.Stop();

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate(object o)
            {
                Logger.Status("[EXPECTED] Window.ActualWidth = " + _expectedWidth.ToString());
                Logger.Status("[EXPECTED] Window.ActualHeight = " + _expectedHeight.ToString());
                if ((this.ActualWidth != _expectedWidth) || (this.ActualHeight != _expectedHeight))
                {
                    Logger.Status("[ACTUAL] Window.ActualWidth = " + this.ActualWidth.ToString());
                    Logger.Status("[ACTUAL] Window.ActualHeight = " + this.ActualHeight.ToString());
                    Logger.LogFail("Property Validation Failed!");
                }
                else
                {
                    Logger.Status("[VALIDATION PASSED] Window.ActualWidth  = " + this.ActualWidth.ToString());
                    Logger.Status("[VALIDATION PASSED] Window.ActualHeight = " + this.ActualHeight.ToString());
                }
                Logger.LogPass();
                Application.Current.Shutdown();
                return null;
            }, null);

        }
    }

    public partial class Background_code
    {
        void OnContentRendered(object sender, EventArgs e)
        {
            Brush expectedBrush = Brushes.White;

            // Validate Default Background
            ValidateProperty(expectedBrush);

            expectedBrush = Brushes.Blue;

            Logger.Status("[SET] Window.Background = " + expectedBrush.ToString());
            this.Background = expectedBrush;
            ValidateProperty(expectedBrush);

            TestHelper.Current.TestCleanup();
        }

        private void ValidateProperty(Brush expectedBrush)
        {
            Logger.Status("[EXPECTED] Background = " + expectedBrush.ToString());
            if (this.Background.ToString() != expectedBrush.ToString())
            {
                Logger.LogFail("[ACTUAL] Background = " + this.Background.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Background = " + this.Background.ToString());
            }
        }
    }

    public partial class Background_Markup
    {
        void OnContentRendered(object sender, EventArgs e)
        {
            Brush expectedBrush = Brushes.Blue;

            ValidateProperty(expectedBrush);

            expectedBrush = Brushes.White;
            Logger.Status("[SET] Window.Background = " + expectedBrush.ToString());
            this.Background = expectedBrush;
            ValidateProperty(expectedBrush);

            TestHelper.Current.TestCleanup();
        }

        private void ValidateProperty(Brush expectedBrush)
        {
            Logger.Status("[EXPECTED] Background = " + expectedBrush.ToString());
            if (this.Background.ToString() != expectedBrush.ToString())
            {
                Logger.LogFail("[ACTUAL] Background = " + this.Background.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Background = " + this.Background.ToString());
            }
        }
    }

    public partial class FlowDirection_LeftToRight_Code
    {
        void OnContentRendered(object sender, EventArgs e)
        {
            Logger.Status("[SET] Window.FlowDirection = LeftToRight");
            this.FlowDirection = FlowDirection.LeftToRight;
            ValidateProperty(FlowDirection.LeftToRight);

            Logger.Status("[SET] Window.FlowDirection = RightToLeft");
            this.FlowDirection = FlowDirection.RightToLeft;
            ValidateProperty(FlowDirection.RightToLeft);

            Logger.Status("[SET] Window.FlowDirection = LeftToRight");
            this.FlowDirection = FlowDirection.LeftToRight;

            TestHelper.Current.TestCleanup();

        }

        private void ValidateProperty(FlowDirection expectedFlowDirection)
        {
            Logger.Status("[EXPECTED] FlowDirection = " + expectedFlowDirection.ToString());
            if (this.FlowDirection != expectedFlowDirection)
            {
                Logger.LogFail("[ACTUAL] FlowDirection = " + this.FlowDirection.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] FlowDirection = " + this.FlowDirection.ToString());
            }
        }
    }

    public partial class FlowDirection_LeftToRight_Markup
    {
        void OnContentRendered(object sender, EventArgs e)
        {
            ValidateProperty(FlowDirection.LeftToRight);

            Logger.Status("[SET] Window.FlowDirection = RightToLeft");
            this.FlowDirection = FlowDirection.RightToLeft;
            ValidateProperty(FlowDirection.RightToLeft);

            TestHelper.Current.TestCleanup();

        }

        private void ValidateProperty(FlowDirection expectedFlowDirection)
        {
            Logger.Status("[EXPECTED] FlowDirection = " + expectedFlowDirection.ToString());
            if (this.FlowDirection != expectedFlowDirection)
            {
                Logger.LogFail("[ACTUAL] FlowDirection = " + this.FlowDirection.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] FlowDirection = " + this.FlowDirection.ToString());
            }
        }
    }

    public partial class FlowDirection_RightToLeft_Code
    {
        void OnContentRendered(object sender, EventArgs e)
        {
            // Validate default FlowDirection
            Logger.Status("Validate Default FlowDirection");
            ValidateProperty(FlowDirection.LeftToRight);

            Logger.Status("[SET] Window.FlowDirection = RightToLeft");
            this.FlowDirection = FlowDirection.RightToLeft;
            ValidateProperty(FlowDirection.RightToLeft);

            TestHelper.Current.TestCleanup();
        }

        private void ValidateProperty(FlowDirection expectedFlowDirection)
        {
            Logger.Status("[EXPECTED] FlowDirection = " + expectedFlowDirection.ToString());
            if (this.FlowDirection != expectedFlowDirection)
            {
                Logger.LogFail("[ACTUAL] FlowDirection = " + this.FlowDirection.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] FlowDirection = " + this.FlowDirection.ToString());
            }
        }
    }

    public partial class FlowDirection_RightToLeft_Markup
    {
        void OnContentRendered(object sender, EventArgs e)
        {
            ValidateProperty(FlowDirection.RightToLeft);

            this.FlowDirection = FlowDirection.LeftToRight;
            ValidateProperty(FlowDirection.LeftToRight);

            TestHelper.Current.TestCleanup();
        }

        private void ValidateProperty(FlowDirection expectedFlowDirection)
        {
            Logger.Status("[EXPECTED] FlowDirection = " + expectedFlowDirection.ToString());
            if (this.FlowDirection != expectedFlowDirection)
            {
                Logger.LogFail("[ACTUAL] FlowDirection = " + this.FlowDirection.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] FlowDirection = " + this.FlowDirection.ToString());
            }
        }
    }

    public partial class ApplicationStyledNavigationWindow
    {
        NavigationWindow _win;

        void OnStartup(object sender, EventArgs e)
        {
            _win = new NavigationWindow();
            _win.ContentRendered += new EventHandler(win_ContentRendered);
            _win.Show();
        }

        void win_ContentRendered(object sender, EventArgs e)
        {
            TestHelper.Current.TestCleanup();
        }
    }

    public partial class ApplicationStyledNavigationWindowViaStartupUri_Window
    {
        void OnLoaded(object sender, EventArgs e)
        {
            Logger.Status("Calling TryFindResource(\"WindowContent\")");

            if (this.TryFindResource("WindowContent") == null)
            {
                Logger.LogFail("Unable to find resource WindowContent");
            }
            else
            {
                Logger.Status("VALIDATION PASSED: Resource found");
            }

            Logger.Status("Calling TryFindResource(\"UnknownStyleKey\")");
            if (this.TryFindResource("UnknownStyleKey") == null)
            {
                Logger.Status("VALIDATION PASSED: Unable to find resource as expected");
            }
            else
            {
                Logger.LogFail("Should not have found this resource");
            }
            TestHelper.Current.TestCleanup();
        }
    }

    public partial class Width_Code
    {
        double _expectedValue;

        void OnContentRendered(object sender, EventArgs e)
        {
            this.Height = 300;
            double expectedHeight = 300;

            // Expected Value
            _expectedValue = 300;
            Logger.Status("[SET] Window.Width = " + _expectedValue.ToString());
            this.Width = _expectedValue;
            ValidateWidth(_expectedValue);
            ValidateHeight(expectedHeight);

            // Expected Value
            _expectedValue = 112;
            Logger.Status("[SET] Window.Width = " + (_expectedValue - _expectedValue).ToString());
            this.Width = (_expectedValue - _expectedValue);
            if (this.ActualWidth < _expectedValue)
            {
                Logger.LogFail("[ACTUAL] Window.ActualWidth = " + this.ActualWidth.ToString() + "< ExpectedWidth = " + _expectedValue);
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Window.ActualWidth = " + this.ActualWidth.ToString() + ">= ExpectedWidth = " + _expectedValue);
            }

            _expectedValue = -10;
            Logger.Status("[SET] Window.Width = " + _expectedValue.ToString() + " should throw exception");
            try
            {
                this.Width = _expectedValue;
                Logger.LogFail("[EXCEPTION NOT CAUGHT] Expected Exception not caught");
            }
            catch (System.ArgumentException argex)
            {
                Logger.Status("[VALIDATION PASSED] System.ArgumentException caught as expected\n" + argex.ToString());
            }
            catch (System.Exception exception)
            {
                Logger.LogFail("Unexpected Exception caught!!\n " + exception.ToString());
            }

            Logger.Status("[SET] WindowState=Maximized, Width=500");
            this.WindowState = WindowState.Maximized;
            this.Width = 500;
            Logger.Status("[SET] WindowState=Normal");
            this.WindowState = WindowState.Normal;
            _expectedValue = 500;
            ValidateWidth(_expectedValue);
            ValidateHeight(expectedHeight);

            Logger.Status("[SET] WindowState=Maximized, SizeToContent=Width");
            this.WindowState = WindowState.Maximized;
            this.SizeToContent = SizeToContent.Width;
            Logger.Status("[SET] WindowState=Normal");
            this.WindowState = WindowState.Normal;

            _expectedValue = page.Width + WindowValidator.GetBorderLength(this.Title) * 2;
            Logger.Status("[EXPECTED] Width=" + _expectedValue.ToString());
            Logger.Status("[EXPECTED] Height=" + expectedHeight.ToString());
            if (!TestUtil.IsEqual(this.ActualWidth, _expectedValue) 
                || !TestUtil.IsEqual(this.ActualHeight, expectedHeight))
                Logger.LogFail("Window.ActualWidth==" + this.ActualWidth.ToString() + " Window.ActualHeight==" + this.ActualHeight.ToString());
            else
                Logger.Status("Validation Passed!");

            this.SizeChanged += OnSizeChanged;
            Logger.Status("[SET] page.Width=300");
            page.Width = 300;
            _expectedValue = page.Width + WindowValidator.GetBorderLength(this.Title) * 2;

        }

        void OnSizeChanged(object sender, EventArgs e)
        {
            Logger.Status("[EXPECTED] Width=" + _expectedValue.ToString());
            if (!TestUtil.IsEqual(this.ActualWidth, _expectedValue))
                Logger.LogFail("Window.ActualWidth==" + this.ActualWidth.ToString() + "ExpectedValue = " + _expectedValue);
            else
                Logger.Status("Validation Passed!");

            this.SizeChanged -= OnSizeChanged;

            TestHelper.Current.TestCleanup();
        }

        void ValidateWidth(double expectedValue)
        {
            Logger.Status("[EXPECTED] Width=" + expectedValue.ToString());
            // Validate Property Value
            if (!TestUtil.IsEqual(this.Width, expectedValue) 
                || !TestUtil.IsEqual((double)GetValue(Window.WidthProperty), expectedValue))
                Logger.LogFail("Window.Width==" + this.Width.ToString() + " GetValue(WidthProperty)==" + GetValue(Window.WidthProperty).ToString());
            else
                Logger.Status("[VALIDATION PASSED] Property Value Validated");

            // Win32 Validation
            if (WindowValidator.ValidateWidth(this.Title, expectedValue))
                Logger.Status("[VALIDATION PASSED] Win32 Validation Passed");
            else
                Logger.LogFail("Win32 Validation Failed!");

        }

        void ValidateHeight(double expectedValue)
        {
            Logger.Status("[EXPECTED] Height=" + expectedValue.ToString());
            // Validate Property Value
            if (!TestUtil.IsEqual(this.Height, expectedValue) 
                || !TestUtil.IsEqual((double)GetValue(Window.HeightProperty), expectedValue))
                Logger.LogFail("Window.Height==" + this.Height.ToString() + " GetValue(HeightProperty)==" + GetValue(Window.HeightProperty).ToString());
            else
                Logger.Status("[VALIDATION PASSED] Property Value Validated");

            // Win32 Validation
            if (WindowValidator.ValidateHeight(this.Title, expectedValue))
                Logger.Status("[VALIDATION PASSED] Win32 Validation Passed");
            else
                Logger.LogFail("Win32 Validation Failed!");

        }
    }

    public partial class Height_Code
    {
        double _expectedValue;
        void OnContentRendered(object sender, EventArgs e)
        {
            this.Top = this.Left = 100;
            this.Width = 300;
            double expectedWidth = 300;

            // Expected Value
            _expectedValue = 300;
            Logger.Status("[SET] Window.Height = " + _expectedValue.ToString());
            this.Height = _expectedValue;
            ValidateHeight(_expectedValue);
            ValidateWidth(expectedWidth);

            // Expected Value
            if ((!TestUtil.IsThemeClassic) && (!TestUtil.IsThemeRoyale))
            {
                // DPI ratios can be changed in XP from .2 to 5 (20 % to 500 %)
                // therefore, we need to use a small value that will work for all the DPI ratios
                _expectedValue = TestUtil.LogicalToDeviceUnits(
                                    new Point(1, 1),
                                    this).X;
                Logger.Status("[SET] Window.Height = " + (_expectedValue - _expectedValue).ToString());
                this.Height = (_expectedValue - _expectedValue);

                if (this.ActualHeight < _expectedValue)
                {
                    Logger.LogFail("[ACTUAL] Window.ActualHeight = " + this.ActualHeight.ToString() + "< ExpectedHeight = " + _expectedValue);
                }
                else
                {
                    Logger.Status("[VALIDATION PASSED] Window.ActualHeight = " + this.ActualHeight.ToString() + ">= ExpectedHeight = " + _expectedValue);
                }
            }

            _expectedValue = -1;
            Logger.Status("[SET] Window.Height = " + _expectedValue.ToString() + " should throw exception");
            try
            {
                this.Height = _expectedValue;
                Logger.LogFail("[EXCEPTION NOT CAUGHT] Expected Exception not caught");
            }
            catch (System.ArgumentException argex)
            {
                Logger.Status("[VALIDATION PASSED] System.ArgumentException caught as expected\n" + argex.ToString());
            }
            catch (System.Exception exception)
            {
                Logger.LogFail("Unexpected Exception caught!!\n " + exception.ToString());
            }

            Logger.Status("[SET] WindowState=Maximized, Width=500");
            this.WindowState = WindowState.Maximized;
            this.Height = 300;
            Logger.Status("[SET] WindowState=Normal");
            this.WindowState = WindowState.Normal;
            _expectedValue = 300;
            ValidateHeight(_expectedValue);
            ValidateWidth(expectedWidth);

            Logger.Status("[SET] WindowState=Minimized, SizeToContent=Height");
            this.WindowState = WindowState.Minimized;
            this.SizeToContent = SizeToContent.Height;

            Logger.Status("[SET] WindowState=Normal");
            this.WindowState = WindowState.Normal;

            expectedWidth = (TestUtil.DeviceToLogicalUnits(
                                    new Point(160,160),
                                    this)).X;

            _expectedValue = page.Height + WindowValidator.GetCaptionHeight(this.Title) + WindowValidator.GetBorderLength(this.Title);
            Logger.Status("[EXPECTED] Height=" + _expectedValue.ToString());
            Logger.Status("[EXPECTED] Width=" + expectedWidth.ToString());
            if (!TestUtil.IsEqual(this.ActualHeight, _expectedValue)
                || !TestUtil.IsEqual(this.ActualWidth, expectedWidth))
                Logger.LogFail("Window.ActualWidth==" + this.ActualWidth.ToString() + " Window.ActualHeight==" + this.ActualHeight.ToString());
            else
                Logger.Status("Validation Passed!");

            this.SizeChanged += OnSizeChanged;
            Logger.Status("[SET] page.Height=300");
            page.Height = 300;
            _expectedValue = page.Height + WindowValidator.GetCaptionHeight(this.Title) + WindowValidator.GetBorderLength(this.Title);
        }

        void OnSizeChanged(object sender, EventArgs e)
        {
            Logger.Status("[EXPECTED] Height=" + _expectedValue.ToString());
            if (!TestUtil.IsEqual(this.ActualHeight, _expectedValue))
                Logger.LogFail("Window.ActualHeight==" + this.ActualHeight.ToString());
            else
                Logger.Status("Validation Passed!");

            TestHelper.Current.TestCleanup();
        }

        void ValidateHeight(double expectedValue)
        {
            Logger.Status("[EXPECTED] Height=" + expectedValue.ToString());
            // Validate Property Value
            if (!TestUtil.IsEqual(this.Height, expectedValue) 
                || !TestUtil.IsEqual((double)GetValue(Window.HeightProperty), expectedValue))
                Logger.LogFail("Window.Height==" + this.Height.ToString() + " GetValue(HeightProperty)==" + GetValue(Window.HeightProperty).ToString());
            else
                Logger.Status("[VALIDATION PASSED] Window.Height==" + this.Height.ToString() + " GetValue(HeightProperty)==" + GetValue(Window.HeightProperty).ToString());

            // Win32 Validation
            if (WindowValidator.ValidateHeight(this.Title, expectedValue))
                Logger.Status("[VALIDATION PASSED] Win32 Validation Passed");
            else
                Logger.LogFail("Win32 Validation Failed!");

        }

        void ValidateWidth(double expectedValue)
        {
            Logger.Status("[EXPECTED] Width=" + expectedValue.ToString());
            // Validate Property Value
            if (!TestUtil.IsEqual(this.Width, expectedValue) 
                || !TestUtil.IsEqual((double)GetValue(Window.WidthProperty), expectedValue))
                Logger.LogFail("Window.Width==" + this.Width.ToString() + " GetValue(WidthProperty)==" + GetValue(Window.WidthProperty).ToString());
            else
                Logger.Status("[VALIDATION PASSED] Window.Width==" + this.Width.ToString() + " GetValue(WidthProperty)==" + GetValue(Window.WidthProperty).ToString());

            // Win32 Validation
            if (WindowValidator.ValidateWidth(this.Title, expectedValue))
                Logger.Status("[VALIDATION PASSED] Win32 Validation Passed");
            else
                Logger.LogFail("Win32 Validation Failed!");

        }
    }

    public partial class Title_Code
    {
        void OnContentRendered(object sender, EventArgs e)
        {
            string expectedValue;

            expectedValue = "Avalon.Window.Test";
            this.Title = expectedValue;
            Validate(expectedValue);

            expectedValue = "\n\tAvalon.Window.Test\n";
            this.Title = expectedValue;
            Validate(expectedValue);

            expectedValue = "TEMPORARY STRING! TO BE REPLACED BY AUTODATA"; //AutoData.Generate.GetRandomString();
            this.Title = expectedValue;
            Validate(expectedValue);

            TestHelper.Current.TestCleanup();
        }

        void Validate(string expectedValue)
        {
            Logger.Status("[EXPECTED] Window.Title = " + expectedValue);
            if (this.Title != expectedValue)
                Logger.LogFail("Title != " + expectedValue.ToString());
            else
                Logger.Status("[VALIDATION PASSED] Default Window.Title == " + expectedValue.ToString());
            if (!WindowValidator.ValidateTitle(this.Title))
                Logger.LogFail("Win32 Validation Failed!");
            else
                Logger.Status("[VALIDATION PASSED] Win32 Validation Passed");
        }
    }

    public partial class CreateWindowOnWorkerThread
    {
        void OnLoaded(object sender, EventArgs e)
        {
            System.Threading.Thread CreateWindowThread = new System.Threading.Thread(CreateWindow);
            CreateWindowThread.SetApartmentState(System.Threading.ApartmentState.STA);
            CreateWindowThread.Start();
        }

        void CreateWindow()
        {
            //System.Windows.Threading.Dispatcher.Run();
            Window win = new Window();
            win.Title = "Window on worker thread";
            win.Content = new Button(); ;
            win.ContentRendered += OnContentRendered;
            win.Show();


            //Window win2 = new Window();
            //win2.Title = "Dialog on worker thread";
            //win2.Content = new ComboBox(); ;
            //win2.ContentRendered += OnContentRendered;
            //win2.ShowDialog();
        }

        void OnContentRendered(object sender, EventArgs e)
        {
            Logger.Status("Window ContentRendered Event Caught!");
            ((Window)sender).Close();
        }
    }

    public partial class WindowIsResizeable
    {
        // new and old points for mouse dragging
        Point _fromPoint, _toPoint;

        // Expected values which will be used for validation
        double _expectedWidth, _expectedHeight;

        Thread _controlThread;

        // resizemodes
        enum _resizeMode : int
        {
            ShrinkFromUpperLeft = 1,
            GrowFromLowerRight = 2,
            ShrinkFromTop = 3,
            GrowFromBottom = 4,
            ShrinkFromLeft = 5,
            GrowFromRight = 6,
            quit = 7
        }

        private void OnContentRendered(object sender, EventArgs e)
        {
            Logger.Status("Win ContentRendered event caught!\n");

            // Create a new thread
            _controlThread = new Thread(new ThreadStart(ControlThreadEntry));

            // Calling method to resize by property
            ResizeByProperty();

            Logger.Status("Start FlowControl Thread\n");
            _controlThread.Start();
        }

        private void ResizeByProperty()
        {
            Logger.Status("*** RESIZE MODE: Resize by property ***");
            // Updating toPoint.X which will be used for validation upon Resize Event fires 
            Logger.Status("Set window to 400x400");
            this.Height = 400;
            this.Width = 400;

            // Updating toPoint.X which will be used for validation upon Resize Event fires.
            _expectedHeight = 400;
            _expectedWidth = 400;

            // Validate result
            Validate();
        }

        // *********************************
        // FlowControl Thread entry function
        // *********************************
        public void ControlThreadEntry()
        {
            for (int currResizeMode = 1; currResizeMode != (int)_resizeMode.quit; currResizeMode++)
            {
                // Calling PrepareResizeMode method
                PrepareResizeMode(currResizeMode);

                // Sleep thread for 1 sec
                System.Threading.Thread.Sleep(1000);

                // Move mouse to origin point
                Microsoft.Test.Input.Input.MoveTo(_fromPoint);

                // Sleep thread for 1 sec
                System.Threading.Thread.Sleep(1000);

                // Left mouse button down
                Microsoft.Test.Input.Input.SendMouseInput(0, 0, 0, Microsoft.Test.Input.SendMouseInputFlags.LeftDown);

                // Sleep thread for 1 sec
                System.Threading.Thread.Sleep(1000);

                // Move mouse to destination point
                Microsoft.Test.Input.Input.MoveTo(_toPoint);

                // sleep thread for 1 sec
                System.Threading.Thread.Sleep(1000);

                // Mouse button up
                Microsoft.Test.Input.Input.SendMouseInput(0, 0, 0, Microsoft.Test.Input.SendMouseInputFlags.LeftUp);

                // sleep thread for 1 sec
                System.Threading.Thread.Sleep(1000);

                // Validate result
                Validate();
            }

            // Clean up test and quit app
            TestHelper.Current.TestCleanup();
        }

        //********************************************
        // Name:  PrepareResizeMode
        // Param: Current Resize Mode as INT
        // Purpose: This function will, based on the current
        //          resize mode, set the mouse origin
        //          and destination points, and expected values
        //          which will be used for validation later
        // *********************************************/
        private void PrepareResizeMode(int CurrentMode)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate(object o)
                {
                    Point deltaLogicalUnits = new Point(50,50);
                    Point deltaDeviceUnits = TestUtil.LogicalToDeviceUnits(deltaLogicalUnits, this);

                    Point currentLocationDeviceUnits = TestUtil.LogicalToDeviceUnits(
                        new Point(Left, Top), 
                        this);

                    Point currentSizeDeviceUnits = TestUtil.LogicalToDeviceUnits(
                        new Point(ActualWidth, ActualHeight),
                        this);
                    
                    switch (CurrentMode)
                    {
                        case (int)_resizeMode.ShrinkFromUpperLeft:
                            Logger.Status("*** RESIZE MODE: Shrink From Upper Left Edge ***");
                            // Update fromPoint and toPoint for this resize mode.
                            // fromPoint needs to be 2 pixels more than top and left of a themed window
                            // due to WinBug #921017 which was resolved by design

                            _fromPoint.X = currentLocationDeviceUnits.X + 2;
                            _fromPoint.Y = currentLocationDeviceUnits.Y + 2;
                            _toPoint.X = _fromPoint.X + deltaDeviceUnits.X;
                            _toPoint.Y = _fromPoint.Y + deltaDeviceUnits.Y;
                            Logger.Status("fromPoint=(" + _fromPoint.ToString() + ") toPoint(" + _toPoint.ToString() + ")");
                            _expectedWidth = this.ActualWidth - deltaLogicalUnits.X;
                            _expectedHeight = this.ActualHeight - deltaLogicalUnits.Y;
                            break;

                        case (int)_resizeMode.GrowFromBottom:
                            Logger.Status("*** RESIZE MODE: Grow From Bottom ***");
                            // setting pt to upper left edge of window
                            _fromPoint.X = (currentLocationDeviceUnits.X + currentSizeDeviceUnits.X) / 2;
                            _fromPoint.Y = currentLocationDeviceUnits.Y + currentSizeDeviceUnits.Y - 2;
                            _toPoint.X = _fromPoint.X;
                            _toPoint.Y = _fromPoint.Y + deltaDeviceUnits.Y;
                            Logger.Status("fromPoint=(" + _fromPoint.ToString() + ") toPoint(" + _toPoint.ToString() + ")");
                            _expectedWidth = this.ActualWidth;
                            _expectedHeight = this.ActualHeight + deltaLogicalUnits.Y;
                            break;

                        case (int)_resizeMode.GrowFromLowerRight:
                            Logger.Status("*** RESIZE MODE: Grow Lower Right Edge ***");
                            // setting pt to upper left edge of window
                            _fromPoint.X = currentLocationDeviceUnits.X + currentSizeDeviceUnits.X - 2;
                            _fromPoint.Y = currentLocationDeviceUnits.Y + currentSizeDeviceUnits.Y - 2;
                            _toPoint.X = _fromPoint.X + deltaDeviceUnits.X;
                            _toPoint.Y = _fromPoint.Y;  // Horizonal resize
                            Logger.Status("fromPoint=(" + _fromPoint.ToString() + ") toPoint(" + _toPoint.ToString() + ")");
                            _expectedWidth = this.ActualWidth + deltaLogicalUnits.X;
                            _expectedHeight = this.ActualHeight;
                            break;

                        case (int)_resizeMode.GrowFromRight:
                            Logger.Status("*** RESIZE MODE: Grow From Right ***");

                            // setting pt to upper left edge of window
                            _fromPoint.X = currentLocationDeviceUnits.X + currentSizeDeviceUnits.X - 2;
                            _fromPoint.Y = (currentLocationDeviceUnits.Y + currentSizeDeviceUnits.Y) / 2;
                            _toPoint.X = _fromPoint.X + deltaDeviceUnits.X;
                            _toPoint.Y = _fromPoint.Y;
                            Logger.Status("fromPoint=(" + _fromPoint.ToString() + ") toPoint(" + _toPoint.ToString() + ")");
                            _expectedWidth = this.ActualWidth + deltaLogicalUnits.X;
                            _expectedHeight = this.ActualHeight;
                            break;

                        case (int)_resizeMode.ShrinkFromLeft:
                            Logger.Status("*** RESIZE MODE: Shrink From Left ***");
                            // setting pt to upper left edge of window
                            _fromPoint.X = currentLocationDeviceUnits.X;
                            _fromPoint.Y = (currentLocationDeviceUnits.Y + currentSizeDeviceUnits.Y) / 2;
                            _toPoint.X = _fromPoint.X + deltaDeviceUnits.X;
                            _toPoint.Y = _fromPoint.Y;
                            Logger.Status("fromPoint=(" + _fromPoint.ToString() + ") toPoint(" + _toPoint.ToString() + ")");
                            _expectedWidth = this.ActualWidth - deltaLogicalUnits.X;
                            _expectedHeight = this.ActualHeight;
                            break;

                        case (int)_resizeMode.ShrinkFromTop:
                            Logger.Status("*** RESIZE MODE: Shrink From Top ***");
                            // setting pt to upper left edge of window
                            _fromPoint.X = (currentLocationDeviceUnits.X + currentSizeDeviceUnits.X) / 2;
                            _fromPoint.Y = currentLocationDeviceUnits.Y;
                            _toPoint.X = _fromPoint.X;
                            _toPoint.Y = _fromPoint.Y + deltaDeviceUnits.Y;
                            Logger.Status("fromPoint=(" + _fromPoint.ToString() + ") toPoint(" + _toPoint.ToString() + ")");
                            _expectedWidth = this.ActualWidth;
                            _expectedHeight = this.ActualHeight - deltaLogicalUnits.Y;
                            break;

                        default:
                            break;
                    }

                    return null;
                }, null
             );

        }

        private void Validate()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (DispatcherOperationCallback)delegate(object o)
                {
                    // **************
                    // * VALIDATION *
                    // **************
                    if (!TestUtil.IsEqual(_expectedWidth, this.ActualWidth) 
                        || !TestUtil.IsEqual(_expectedHeight, this.ActualHeight))
                    {
                        Logger.LogFail("Expected: [" + _expectedWidth + "x" + _expectedHeight + "]   Actual: [" + this.ActualWidth + "x" + this.ActualHeight + "]\n");
                    }
                    else
                    {
                        Logger.Status("Expected: [" + _expectedWidth + "x" + _expectedHeight + "]   Actual: [" + this.ActualWidth + "x" + this.ActualHeight + "]\n");
                    }

                    return null;
                }, null
            );


        }
    }

    public partial class WindowNotResizeable
    {
        System.Timers.Timer _t1, _t2, _t3, _t4;
        Point _fromPoint, _toPoint;
        Double _expectedWidth = 500, _expectedHeight = 500;

        private void OnContentRendered(object sender, EventArgs e)
        {
            this.SizeChanged += OnSizeChanged;

            // Creating timer controls to prevent race conditions in automation
            Logger.Status("Create timer controls");
            _t1 = new System.Timers.Timer(1000);
            _t2 = new System.Timers.Timer(1000);
            _t3 = new System.Timers.Timer(2000);
            _t4 = new System.Timers.Timer(1000);
            // Creating Elapsed event handler for each timer control
            _t1.Elapsed += new System.Timers.ElapsedEventHandler(T1_MouseMove);
            _t2.Elapsed += new System.Timers.ElapsedEventHandler(T2_LeftButtonDown);
            _t3.Elapsed += new System.Timers.ElapsedEventHandler(T3_MouseMove);
            _t4.Elapsed += new System.Timers.ElapsedEventHandler(T4_LeftButtonUp);
            Logger.Status("Created Elapsed handlers for all timer controls");

            // Save from and to Point for dragging
            _fromPoint = TestUtil.LogicalToDeviceUnits(
                                    new Point(Left, Top),
                                    this);
            
            _toPoint = TestUtil.LogicalToDeviceUnits(
                                    new Point(this.Left + 100, this.Top + 100),
                                    this);

            // Start timer1
            Logger.Status("start timer1");
            _t1.Start();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Logger.LogFail("SizeChanged event caught. This should not happen");
            this.Close();
        }

        private void T1_MouseMove(object sender, System.Timers.ElapsedEventArgs e)
        {
            Logger.Status("timer1 elapsed. Move mouse to " + _fromPoint.ToString());
            _t1.Dispose();
            Microsoft.Test.Input.Input.MoveTo(_fromPoint);

            Logger.Status("start timer2");
            _t2.Start();
        }

        private void T2_LeftButtonDown(object sender, System.Timers.ElapsedEventArgs e)
        {
            Logger.Status("timer2 elapsed. LeftButtonDown");
            _t2.Dispose();
            Microsoft.Test.Input.Input.SendMouseInput(_fromPoint.X, _fromPoint.Y, 0, Microsoft.Test.Input.SendMouseInputFlags.LeftDown);

            Logger.Status("start timer3");
            _t3.Start();
        }

        private void T3_MouseMove(object sender, System.Timers.ElapsedEventArgs e)
        {
            Logger.Status("t3 elapsed. Move mouse to " + _toPoint.ToString());
            _t3.Dispose();
            Microsoft.Test.Input.Input.MoveTo(_toPoint);

            Logger.Status("start timer4");
            _t4.Start();
        }

        private void T4_LeftButtonUp(object sender, System.Timers.ElapsedEventArgs e)
        {
            Logger.Status("t4 elapsed. LeftButtonUp");
            _t4.Dispose();
            Logger.Status("Sending LeftButtonUp");
            Microsoft.Test.Input.Input.SendMouseInput(_toPoint.X, _toPoint.Y, 0, Microsoft.Test.Input.SendMouseInputFlags.LeftUp);
            Logger.Status("Expected:   " + _expectedWidth.ToString() + "," + _expectedHeight.ToString());
            Logger.Status("Actual:     " + this.ActualWidth.ToString() + "," + this.ActualHeight.ToString());
            if (TestUtil.IsEqual(_expectedWidth, this.ActualWidth)
                && TestUtil.IsEqual(_expectedHeight, this.ActualHeight))
            {
                Logger.LogPass("ActualWidth and ActualHeight are as expected");
            }
            else
            {
                Logger.LogFail("ActualWidth and ActualHeight are not as expected");
            }

            TestHelper.Current.TestCleanup();
        }      
    }

    public partial class ResizeWindowWithContentShowing
    {
        System.Timers.Timer _timer;
        int _sizeChangedHitCounter = 0;
        int _expectedSizeChangedHitCount = 50;
        double _expectedWidth = 300;
        double _expectedHeight = 300;

        void OnSizeChanged(object sender, EventArgs e)
        {
            _sizeChangedHitCounter++;
        }

        void OnLoaded(object sender, EventArgs e)
        {
            _timer = new System.Timers.Timer(20000);
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            _timer.Start();
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _timer.Stop();

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate(object o)
            {
                Logger.Status("[EXPECTED] Expected SizeChanged HitCount > " + _expectedSizeChangedHitCount.ToString());
                if (_sizeChangedHitCounter < _expectedSizeChangedHitCount)
                {
                    Logger.LogFail("[ACTUAL] SizeChanged HitCount = " + _sizeChangedHitCounter.ToString());
                }
                else
                {
                    Logger.Status("[VALIDATION PASSED] SizeChanged HitCount = " + _sizeChangedHitCounter.ToString());
                }

                Logger.Status("[EXPECTED] Window.ActualWidth = " + _expectedWidth.ToString());
                Logger.Status("[EXPECTED] Window.ActualHeight = " + _expectedHeight.ToString());
                if ((ActualWidth != _expectedWidth) || (ActualHeight != _expectedHeight))
                {
                    Logger.Status("[ACTUAL] Window.ActualWidth = " + ActualWidth.ToString());
                    Logger.Status("[ACTUAL] Window.ActualHeight = " + ActualHeight.ToString());
                    Logger.LogFail("Property Validation Failed!");
                }
                else
                {
                    Logger.Status("[VALIDATION PASSED] Window.ActualWidth  = " + ActualWidth.ToString());
                    Logger.Status("[VALIDATION PASSED] Window.ActualHeight = " + ActualHeight.ToString());
                }

                Logger.LogPass();
                Application.Current.Shutdown();
                return null;
            }, null);
        }
    }

    public partial class REGRESSION_CallAppDotRunInRunningApp
    {
        private void Call_Run_In_Application_Startup(object sender, StartupEventArgs e)
        {
            Logger.SetStage("Running");
            Logger.Status("In Application Startup Event.  Calling App.Run()");

            Assembly presFramework = typeof(Application).Assembly;
            ResourceManager resManager = new ResourceManager("ExceptionStringTable", presFramework);
            string expectedException = resManager.GetString("ApplicationAlreadyRunning");
            
            try
            {
                Application.Current.Run();
            }
            catch (Exception exc)
            {
                Logger.Status("Caught: " + exc.ToString() + "\n" + exc.StackTrace.ToString());

                if ((exc.Message == expectedException) && (exc.GetType() == typeof(System.InvalidOperationException)))
                {
                    Logger.LogPass("Success! Got expected exception (and not Invariant Assert) calling App.Run() in running app");
                    Variation.Current.LogResult(Result.Pass);
                }
                else
                {
                    Logger.LogFail("Fail: Caught unexpected exception");
                    Variation.Current.LogResult(Result.Fail);
                }
                Variation.Current.Close();
                
                ApplicationMonitor.NotifyStopMonitoring();
            }
        }
    }

    // Regression Test 
    public partial class REGRESSION_WindowRepaint
    {
        AutomationHelper _AH = new AutomationHelper();
        BackgroundWindow _win;
        void OnContentRendered(object sender, EventArgs e)
        {
            Logger.Status("Showing a new Window");
            _win = new BackgroundWindow();
            _win.Left = 10;
            _win.Top = 10;
            _win.Show();
            
            _AH.WaitThenMoveToAndClick(new Point((int)(this.Left + 5), (int)(this.Top + 3)), CompareImage);
        }

        void CompareImage()
        {
            _win.Left = _win.Top = 0;

            TestHelper.Current.TestCleanup();
        }
    }

    public partial class SizeChanged_Attach_Markup
    {
        AutomationHelper _AH = new AutomationHelper();
        int _expectedHitCount = 1; // SizeEvent will fire up Showing window
        double _expectedWidth = 200,_expectedHeight = 200;
        // we set in markup to start window from origin of desktop

        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Logger.RecordHit("OnSizeChanged");
        }

        void OnContentRendered(object sender, EventArgs e)
        {
            Logger.Status("[SET] Window.Width - Expect HitCount");
            this.Width++;
            _expectedHitCount++;
            _expectedWidth++;
            ValidateSize();

            Logger.Status("[SET] Width.Height - Expect HitCount");
            this.Height++;
            _expectedHitCount++;
            _expectedHeight++;
            ValidateSize();

            Logger.Status("[AUTOMATION] DragDrop Upper Left Corner");
            Point deltaDeviceUnits = new Point(20,20);
            Point deltaLogicalUnits = TestUtil.DeviceToLogicalUnits(deltaDeviceUnits, this);
            
            // on Classic theme, we need to click the exact corner of the window due to the square shape.
            // on other themes, we need to be inside a bit due to the rounded corners.
            // we also need to adjust the offset back a bit on 120 DPI.
            int offset = (int)(3.0 / TestUtil.DPIXRatio);
            if (TestUtil.IsThemeClassic)
            {
                offset = 0;
            }

            Point fromPointDeviceUnits = TestUtil.LogicalToDeviceUnits(new Point(Left + offset, Top + offset), this);
            Point toPointDeviceUnits = new Point(fromPointDeviceUnits.X - deltaDeviceUnits.X, fromPointDeviceUnits.Y - deltaDeviceUnits.Y);

            _expectedHitCount++;
            _expectedWidth += deltaLogicalUnits.X;
            _expectedHeight += deltaLogicalUnits.Y;

            _AH.DragDrop(fromPointDeviceUnits, toPointDeviceUnits, Cleanup);
        }

        void Cleanup()
        {
            Logger.Status("[EXPECTED] HitCount = " + _expectedHitCount.ToString());
            int ActualHitCount = Logger.GetHitCount("OnSizeChanged");
            if (ActualHitCount != _expectedHitCount)
            {
                Logger.LogFail("[ACTUAL] HitCount = " + ActualHitCount.ToString());
            }
            else
            {
                Logger.Status("[ACTUAL] HitCount = " + ActualHitCount.ToString());
            }

            ValidateSize();

            TestHelper.Current.TestCleanup();
        }

        void ValidateSize()
        {
            Logger.Status("VALIDATE PROPERTY");
            Logger.Status("[EXPECTED] Size = " + _expectedWidth.ToString() + "x" + _expectedHeight.ToString());
            if (!TestUtil.IsEqual(this.ActualWidth, _expectedWidth)
                || !TestUtil.IsEqual(this.ActualHeight, _expectedHeight))
            {
                Logger.Status("[ACTUAL] Size = " + this.ActualWidth.ToString() + "x" + this.ActualHeight.ToString());
                Logger.LogFail("");
            }
            else
            {
                Logger.Status("[ACTUAL] Size = " + this.ActualWidth.ToString() + "x" + this.ActualHeight.ToString());
            }
        }
    }

    public partial class SizeChanged_Attach_Code
    {
        AutomationHelper _AH = new AutomationHelper();
        int _expectedSizeChangedHitCount = 0;
        int _expectedLocationChangedHitCount = 0;
        double _expectedWidth = 200,_expectedHeight = 200;
        // we set in markup to start window from origin of desktop

        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Logger.RecordHit("OnSizeChanged");
        }

        void OnLocationChanged(object sender, EventArgs e)
        {
            Logger.RecordHit("OnLocationChanged");
        }

        void OnContentRendered(object sender, EventArgs e)
        {
            Logger.Status("Attaching Window.SizeChanged Event Handler");
            this.SizeChanged += new SizeChangedEventHandler(OnSizeChanged);
            this.LocationChanged += new EventHandler(OnLocationChanged);
            Logger.Status("[SET] Window.Width - Expect HitCount");
            this.Width++;
            _expectedSizeChangedHitCount++;
            _expectedWidth++;
            ValidateSize();

            Logger.Status("[SET] Width.Height - Expect HitCount");
            this.Height++;
            _expectedSizeChangedHitCount++;
            _expectedHeight++;
            ValidateSize();

            Logger.Status("[AUTOMATION] DragDrop Upper Left Corner");

            Point deltaDeviceUnits = new Point(20,20);
            Point deltaLogicalUnits = TestUtil.DeviceToLogicalUnits(deltaDeviceUnits, this);
            
            // on Classic theme, we need to click the exact corner of the window due to the square shape.
            // on other themes, we need to be inside a bit due to the rounded corners.
            // we also need to adjust the offset back a bit on 120 DPI.
            int offset = (int)(3.0 / TestUtil.DPIXRatio);
            if (TestUtil.IsThemeClassic)
            {
                offset = 0;
            }

            Point fromPointDeviceUnits = TestUtil.LogicalToDeviceUnits(new Point(Left + offset, Top + offset), this);
            Point toPointDeviceUnits = new Point(fromPointDeviceUnits.X - deltaDeviceUnits.X, fromPointDeviceUnits.Y - deltaDeviceUnits.Y);
           
            _expectedSizeChangedHitCount++;
            _expectedLocationChangedHitCount++;
            _expectedWidth += deltaLogicalUnits.X;
            _expectedHeight += deltaLogicalUnits.Y;
            
            _AH.DragDrop(fromPointDeviceUnits, toPointDeviceUnits, Cleanup);
        }

        void Cleanup()
        {
            Logger.Status("[EXPECTED] SizeChangedHitCount = " + _expectedSizeChangedHitCount.ToString());
            Logger.Status("[EXPECTED] LocationChangedHitCount = " + _expectedLocationChangedHitCount.ToString());
            int ActualSizeChangedHitCount = Logger.GetHitCount("OnSizeChanged");
            int ActualLocationChangedHitCount = Logger.GetHitCount("OnLocationChanged");
            if (ActualSizeChangedHitCount != _expectedSizeChangedHitCount ||
                ActualLocationChangedHitCount != _expectedLocationChangedHitCount)
            {
                Logger.LogFail("[ACTUAL] ActualSizeChangedHitCount = " + ActualSizeChangedHitCount.ToString() + "; " + 
                               "[ACTUAL] ActualLocationChangedHitCount = " + ActualLocationChangedHitCount.ToString());
            }
            else
            {
                Logger.Status("HitCount Validated");
            }

            ValidateSize();

            TestHelper.Current.TestCleanup();
        }

        void ValidateSize()
        {
            Logger.Status("VALIDATE PROPERTY");
            Logger.Status("[EXPECTED] Size = " + _expectedWidth.ToString() + "x" + _expectedHeight.ToString());
            if (!TestUtil.IsEqual(this.ActualWidth, _expectedWidth)
                || !TestUtil.IsEqual(this.ActualHeight, _expectedHeight))
            {
                Logger.Status("[ACTUAL] Size = " + this.ActualWidth.ToString() + "x" + this.ActualHeight.ToString());
                Logger.LogFail("");
            }
            else
            {
                Logger.Status("[ACTUAL] Size = " + this.ActualWidth.ToString() + "x" + this.ActualHeight.ToString());
            }
        }
    }

    public partial class LocationChanged_Attach_Markup
    {
        AutomationHelper _AH = new AutomationHelper();
        int _expectedHitCount = 1;
        // starting with 1 since the even is attached prior showing a window
        // so we will receiving one location changed event when showing the window
        double _expectedLeft = 0,_expectedTop = 0;

        void OnLocationChanged(object sender, EventArgs e)
        {
            Logger.RecordHit("OnLocationChanged");
        }

        void OnContentRendered(object sender, EventArgs e)
        {
            Logger.Status("[SET] Window.Left - Expect HitCount");
            _expectedHitCount++;
            this.Left++;
            _expectedLeft++;
            ValidateLocation();

            Logger.Status("[SET] Width.Top - Expect HitCount");
            _expectedHitCount++;
            this.Top++;
            _expectedTop++;
            ValidateLocation();

            Logger.Status("[AUTOMATION] DragDrop TopLeft Corner");
            Point deltaDeviceUnits = new Point(20,20);
            Point deltaLogicalUnits = TestUtil.DeviceToLogicalUnits(deltaDeviceUnits, this);
            Point currentLocationDeviceUnits = TestUtil.LogicalToDeviceUnits(new Point(Left, Top), this);
            Point fromPointDeviceUnits = new Point(currentLocationDeviceUnits.X + 50, currentLocationDeviceUnits.Y + 10);
            Point toPointDeviceUnits = new Point(fromPointDeviceUnits.X + deltaDeviceUnits.X, fromPointDeviceUnits.Y + deltaDeviceUnits.Y);
            
            _expectedHitCount++;
            _expectedLeft = this.Left + deltaLogicalUnits.X;
            _expectedTop = this.Top + deltaLogicalUnits.Y;

            _AH.DragDrop(fromPointDeviceUnits, toPointDeviceUnits, Cleanup);
        }

        void Cleanup()
        {
            Logger.Status("[EXPECTED] HitCount = " + _expectedHitCount.ToString());
            int ActualHitCount = Logger.GetHitCount("OnLocationChanged");
            if (ActualHitCount != _expectedHitCount)
            {
                Logger.LogFail("[ACTUAL] HitCount = " + ActualHitCount.ToString());
            }
            else
            {
                Logger.Status("[ACTUAL] HitCount = " + ActualHitCount.ToString());
            }

            ValidateLocation();

            TestHelper.Current.TestCleanup();
        }

        void ValidateLocation()
        {
            Logger.Status("VALIDATE PROPERTY");
            Logger.Status("[EXPECTED] Location = " + _expectedLeft.ToString() + "," + _expectedTop.ToString());
            if (!TestUtil.IsEqual(this.Left, _expectedLeft) 
                || !TestUtil.IsEqual(this.Top, _expectedTop))
            {
                Logger.Status("[ACTUAL] Location = " + this.Left.ToString() + "," + this.Top.ToString());
                Logger.LogFail("");
            }
            else
            {
                Logger.Status("[ACTUAL] Location = " + this.Left.ToString() + "," + this.Top.ToString());
            }
        }
    }

    public partial class ResizeGripAfterChangeResizeMode
    {
        AutomationHelper _AH = new AutomationHelper();
        
        void OnContentRendered(object sender, EventArgs e)
        {
            this.SizeChanged += new SizeChangedEventHandler(OnSizeChanged);
            Logger.Status("[SET] Window.ResizeMode = CanMinimize->CanResizeWithGrip->CanResize->NoResize->CanResizeWithGrip");
            this.ResizeMode = ResizeMode.CanMinimize;
            this.ResizeMode = ResizeMode.CanResizeWithGrip;
            this.ResizeMode = ResizeMode.CanResize;
            this.ResizeMode = ResizeMode.NoResize;
            this.ResizeMode = ResizeMode.CanResizeWithGrip;

            Point FromPointLogicalUnits = new Point(this.Left + this.ActualWidth - this.BorderThickness.Right - 9,
                                        this.Top + this.ActualHeight - this.BorderThickness.Bottom - 9);
            
            Point FromPointDeviceUnits = TestUtil.LogicalToDeviceUnits(FromPointLogicalUnits, this);
            Point ToPointDeviceUnits = new Point(FromPointDeviceUnits.X+10, FromPointDeviceUnits.Y+10);
                       
            Logger.Status("[AUTOMATION] DragDrop Lower Right Corner");

            _AH.DragDrop(FromPointDeviceUnits, ToPointDeviceUnits, null);
        }

        void OnSizeChanged(object sender, EventArgs e)
        {
            Logger.LogPass("SizeChanged Event Caught");
            TestHelper.Current.TestCleanup();
        }
    }


    public partial class ResizeGripAfterChangeResizeModeInRTLWindow
    {
        AutomationHelper _AH = new AutomationHelper();

        void OnContentRendered(object sender, EventArgs e)
        {
            this.SizeChanged += new SizeChangedEventHandler(OnSizeChanged);
            Logger.Status("[SET] Window.ResizeMode = CanMinimize->CanResizeWithGrip->CanResize->NoResize->CanResizeWithGrip");
            this.ResizeMode = ResizeMode.CanMinimize;
            this.ResizeMode = ResizeMode.CanResize;
            this.ResizeMode = ResizeMode.NoResize;
            this.ResizeMode = ResizeMode.CanResizeWithGrip;

            Point FromPointLogicalUnits = new Point(this.Left + this.BorderThickness.Right + 9,
                                        this.Top + this.ActualHeight - this.BorderThickness.Bottom - 9);

            Point FromPointDeviceUnits = TestUtil.LogicalToDeviceUnits(FromPointLogicalUnits, this);
            
            Point ToPointDeviceUnits = new Point(FromPointDeviceUnits.X - 10, FromPointDeviceUnits.Y + 10);

            Logger.Status("[AUTOMATION] DragDrop Lower Left Corner");

            _AH.DragDrop(FromPointDeviceUnits, ToPointDeviceUnits, null);
        }

        void OnSizeChanged(object sender, EventArgs e)
        {
            Logger.LogPass("SizeChanged Event Caught");
            TestHelper.Current.TestCleanup();
        }
    }

    public partial class Icon_Embedded_Code
    {
        void OnContentRendered(object sender, EventArgs e)
        {
            Logger.Status("[SET] window.Icon = " + ResourceString.Icon_TestIcon_Embedded);
            this.Icon = BitmapFrame.Create(new Uri(@"pack://application:,,,/" + ResourceString.Icon_TestIcon_Embedded, UriKind.RelativeOrAbsolute));

            TestHelper.Current.TestCleanup();

        }
    }

    public partial class Icon_Default
    {

        void OnContentRendered(object sender, EventArgs e)
        {
            Logger.Status("[SET] window.Icon = " + ResourceString.Icon_TestIcon_Embedded);
            this.Icon = BitmapFrame.Create(new Uri(@"pack://application:,,,/" + ResourceString.Icon_TestIcon_Embedded, UriKind.RelativeOrAbsolute));
            
            Logger.Status("[SET] window.Icon = null");
            this.Icon = null;

            TestHelper.Current.TestCleanup();
        }
    }

    public partial class Icon_Embedded_Markup
    {

        void OnContentRendered(object sender, EventArgs e)
        {
            TestHelper.Current.TestCleanup();
        }
    }

    public partial class Icon_Loose_Code
    {

        void OnContentRendered(object sender, EventArgs e)
        {
            Logger.Status("[SET] window.Icon = " + ResourceString.Icon_TestIcon_Loose);
            this.Icon = BitmapFrame.Create(new Uri(ResourceString.Icon_TestIcon_Loose, UriKind.RelativeOrAbsolute));

            Logger.Status("[SET] window.Icon = null");
            this.Icon = null;

            TestHelper.Current.TestCleanup();
        }
    }

    public partial class Icon_Loose_Markup
    {
        void OnContentRendered(object sender, EventArgs e)
        {
            //REGRESSION ( NullReferenceException thrown when setting Window.Icon to a loose PNG file)
            Logger.Status("Set icon to PNG file");
            this.Icon = BitmapFrame.Create(new Uri(ResourceString.Icon_TestIconPNG_Loose, UriKind.RelativeOrAbsolute));
            
            TestHelper.Current.TestCleanup();
        }
    }

    public partial class LocationChanged_Attach_Code
    {
        AutomationHelper _AH = new AutomationHelper();
        int _expectedHitCount = 0;
        double _expectedLeft = 0,_expectedTop = 0;
        // we set in markup to start window from origin of desktop

        void OnLocationChanged(object sender, EventArgs e)
        {
            Logger.RecordHit("OnLocationChanged");
        }

        void OnContentRendered(object sender, EventArgs e)
        {
            Logger.Status("Attaching Window.LocationChanged Event Handler");
            this.LocationChanged += new EventHandler(OnLocationChanged);

            Logger.Status("[SET] Window.Left - Expect HitCount");
            _expectedHitCount++;
            this.Left++;
            _expectedLeft++;
            ValidateLocation();

            Logger.Status("[SET] Width.Top - Expect HitCount");
            _expectedHitCount++;
            this.Top++;
            _expectedTop++;
            ValidateLocation();

            Logger.Status("[AUTOMATION] DragDrop TopLeft Corner");

            Point deltaDeviceUnits = new Point(20,20);
            Point deltaLogicalUnits = TestUtil.DeviceToLogicalUnits(deltaDeviceUnits, this);
            Point currentLocationDeviceUnits = TestUtil.LogicalToDeviceUnits(new Point(Left, Top), this);
            Point fromPointDeviceUnits = new Point(currentLocationDeviceUnits.X + 50, currentLocationDeviceUnits.Y + 10);
            Point toPointDeviceUnits = new Point(fromPointDeviceUnits.X + deltaDeviceUnits.X, fromPointDeviceUnits.Y + deltaDeviceUnits.Y);
            
            _expectedHitCount++;
            _expectedLeft = this.Left + deltaLogicalUnits.X;
            _expectedTop = this.Top + deltaLogicalUnits.Y;

            _AH.DragDrop(fromPointDeviceUnits, toPointDeviceUnits, Cleanup);
        }

        void Cleanup()
        {
            Logger.Status("[EXPECTED] HitCount = " + _expectedHitCount.ToString());
            int ActualHitCount = Logger.GetHitCount("OnLocationChanged");
            if (ActualHitCount != _expectedHitCount)
            {
                Logger.LogFail("[ACTUAL] HitCount = " + ActualHitCount.ToString());
            }
            else
            {
                Logger.Status("[ACTUAL] HitCount = " + ActualHitCount.ToString());
            }

            ValidateLocation();

            TestHelper.Current.TestCleanup();
        }

        void ValidateLocation()
        {
            Logger.Status("VALIDATE PROPERTY");
            Logger.Status("[EXPECTED] Location = " + _expectedLeft.ToString() + "," + _expectedTop.ToString());
            if (!TestUtil.IsEqual(this.Left, _expectedLeft) 
                || !TestUtil.IsEqual(this.Top, _expectedTop))
            {
                Logger.Status("[ACTUAL] Location = " + this.Left.ToString() + "," + this.Top.ToString());
                Logger.LogFail("");
            }
            else
            {
                Logger.Status("[ACTUAL] Location = " + this.Left.ToString() + "," + this.Top.ToString());
            }
        }
    }

    public partial class NavigationEventsOnApplication
    {
        TestHelper _TH = new TestHelper();
        void OnAppActivated(object sender, EventArgs e)
        {
            try
            {
                this.Navigated -= OnAppNavigated;
                this.Navigating -= OnAppNavigating;
                this.NavigationProgress -= OnAppNavigationProgress;
                this.NavigationStopped -= OnAppNavigationStopped;
                this.FragmentNavigation -= OnAppFragmentNavigation;
                this.SessionEnding -= OnAppSessionEnding;
            }
            catch(System.Exception ex)
            {
                Logger.LogFail("Unexpected exceptin caught!\n" + ex.StackTrace);
            }

            NavWin.Navigate(new Uri(ResourceString.TestPage1, UriKind.RelativeOrAbsolute));

            _TH.TestCleanup();
        }

        void OnAppNavigated(object sender, EventArgs e)
        {
            Logger.LogFail("OnAppNavigated Event Caught");
        }

        void OnAppNavigating(object sender, EventArgs e)
        {
            Logger.LogFail("OnAppNavigating Event Caught");
        }

        void OnAppNavigationProgress(object sender, EventArgs e)
        {
            Logger.LogFail("OnAppNavigationProgress Event Caught");
        }

        void OnAppNavigationStopped(object sender, EventArgs e)
        {
            Logger.LogFail("OnAppNavigationStopped Event Caught");
        }

        void OnAppFragmentNavigation(object sender, EventArgs e)
        {
            Logger.LogFail("OnAppFragmentNavigation Event Caught");
        }

        void OnAppSessionEnding(object sender, EventArgs e)
        {
            Logger.LogFail("OnAppSessionEnding Event Caught!");
        }
    }

    public partial class ApplicationStaticMethods
    {
        void OnAppActivated(object sender, EventArgs e)
        {
            Uri AbsoluteUri = new Uri("pack://application:,,,/TestPage1.xaml", UriKind.RelativeOrAbsolute);
            Uri RelativeUri = new Uri("TestPage1.xaml", UriKind.RelativeOrAbsolute);
            Uri RelativeContentUri = new Uri("Resources\\Blue_MasterImage_ENG_CLASSIC.bmp", UriKind.RelativeOrAbsolute);
            Uri AbsoluteContentUri = new Uri(System.Environment.CurrentDirectory + "\\Resources\\Blue_MasterImage_ENG_CLASSIC.bmp", UriKind.RelativeOrAbsolute);
            Uri EmptyUri = new Uri(string.Empty, UriKind.RelativeOrAbsolute);
            

            // Sceanrio1: LoadComponent(null, URI) should throw ArgumentNullException
            try
            {
                Application.LoadComponent(null, RelativeUri);
                Logger.LogFail("LoadComponent(null,RelativeUri) should throw ArgumentNullException");
            }
            catch (System.ArgumentNullException ex)
            {
                Logger.Status("ArgumentNullException caught as expected\n" + ex.ToString());
            }
            catch (System.Exception ex)
            {
                Logger.LogFail("Unexpected exception caught! " + ex.StackTrace);
            }

            // Sceanrio2: LoadComponent(object, null) should throw ArgumentNullException
            try
            {
                Application.LoadComponent(this, null);
                Logger.LogFail("LoadComponent(Component,null) should throw ArgumentNullException");
            }
            catch (System.ArgumentNullException ex)
            {
                Logger.Status("ArgumentNullException caught as expected\n" + ex.ToString());
            }
            catch (System.Exception ex)
            {
                Logger.LogFail("Unexpected exception caught! " + ex.StackTrace);
            }

            /* Sceanrio3: LoadComponent(object, EmptyUri) should throw ArgumentException
             * THIS WILL NEVER HIT SINCE URI CLASS CHECKS TO MAKE SURE OriginalSource != NULL
            try
            {
                Application.LoadComponent(this, EmptyUri);
                Logger.LogFail("LoadComponent(null,EmptyUri) should throw ArgumentNullException");
            }
            catch (System.ArgumentException ex)
            {
                Logger.Status("ArgumentException caught as expected");
            }
            catch (System.Exception ex)
            {
                Logger.LogFail("Unexpected exception caught! " + ex.StackTrace);
            }
             * */

            // Sceanrio4: LoadComponent(object, AbsoluteUri) should throw ArgumentException
            try
            {
                Application.LoadComponent(this, AbsoluteUri);
                Logger.LogFail("LoadComponent(null,AbsoluteUri) should throw ArgumentNullException");
            }
            catch (System.ArgumentException ex)
            {
                Logger.Status("ArgumentException caught as expected\n" + ex.ToString());
            }
            catch (System.Exception ex)
            {
                Logger.LogFail("Unexpected exception caught! " + ex.StackTrace);
            }

            // Sceanrio5: LoadComponent(null) should throw ArgumentNullException
            try
            {
                Application.LoadComponent(null);
                Logger.LogFail("LoadComponent(null) should throw ArgumentNullException");
            }
            catch (System.ArgumentNullException ex)
            {
                Logger.Status("ArgumentNullException caught as expected\n" + ex.ToString());
            }
            catch (System.Exception ex)
            {
                Logger.LogFail("Unexpected exception caught! " + ex.StackTrace);
            }


            /* Sceanrio6: LoadComponent(EmptyUri) should throw ArgumentException
             * THIS WILL NEVER HIT SINCE URI CLASS CHECKS TO MAKE SURE OriginalSource != NULL
            try
            {
                Application.LoadComponent(EmptyUri);
                Logger.LogFail("LoadComponent(EmptyUri) should throw ArgumentNullException");
            }
            catch (System.ArgumentException ex)
            {
                Logger.Status("ArgumentException caught as expected");
            }
            catch (System.Exception ex)
            {
                Logger.LogFail("Unexpected exception caught! " + ex.StackTrace);
            }
             * */

            // Sceanrio7: LoadComponent(AbsoluteUri) should throw ArgumentException
            try
            {
                Application.LoadComponent(AbsoluteUri);
                Logger.LogFail("LoadComponent(AbsoluteUri) should throw ArgumentNullException");
            }
            catch (System.ArgumentException ex)
            {
                Logger.Status("ArgumentException caught as expected\n" + ex.ToString());
            }
            catch (System.Exception ex)
            {
                Logger.LogFail("Unexpected exception caught! " + ex.StackTrace);
            }

            try
            {
                Application.GetContentStream(null);
                Logger.LogFail("GetContentStream(null) should throw ArgumentNullException");
            }
            catch (System.ArgumentNullException ex)
            {
                Logger.Status("ArgumentException caught as expected\n" + ex.ToString());
            }
            catch (System.Exception ex)
            {
                Logger.LogFail("Unexpected exception caught! " + ex.StackTrace);
            }

            /* Assert: Calling Application.GetContentStream() with an abolute non-pack uri of a content file cases assertion failure @ BaseUriHelper.IsPackApplicationUri(Uri)
            try
            {
                Application.GetContentStream(AbsoluteContentUri);
                Logger.LogFail("GetContentStream(AbsoluteContentUri) should throw ArgumentException");
            }
            catch (System.ArgumentException ex)
            {
                Logger.Status("ArgumentException caught as expected");
            }
            catch (System.Exception ex)
            {
                Logger.LogFail("Unexpected exception caught! " + ex.StackTrace);
            }
            */


            Application.GetContentStream(RelativeContentUri);

            try
            {
                this.ShutdownMode = (ShutdownMode)(5);
                Logger.LogFail("InvalidEnumArgumentException expected and was not caught");
            }
            catch (System.ComponentModel.InvalidEnumArgumentException ex)
            {
                Logger.Status("InvalidEnumArgumentException caught as expected\n" + ex.ToString());
            }
            catch (System.Exception ex)
            {
                Logger.LogFail("Unexpected exception caught!\n" + ex.StackTrace);
            }


            // Regression Test - (Application.MainWindow property does not get updated after MainWindow closes)
            
            Window win = new Window();
            win.Title = "Non Main Window";
            win.Show();

            Logger.Status("Closing Application.MainWindow");
            NavWin.Close();

            if (this.MainWindow != null || Application.Current.MainWindow != null)
            {
                Logger.LogFail("Application.Current.MainWindow is not null!");
            }
            else
            {
                Logger.Status("MainWindow is null, as expected");
            }

            
            (new TestHelper()).TestCleanup();
        }
    }

    public partial class ApplicationSessionEndingInCode
    {
        void OnAppActivated(object sender, EventArgs e)
        {
            //this.SessionEnding += new SessionEndingCancelEventHandler(ApplicationSessionEnding_SessionEnding);
            WindowValidator.SimulateLogoff();
            WindowValidator.SimulateShutdown();
            //TestHelper.Current.TestCleanup();            
        }

        void ApplicationSessionEnding_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            e.Cancel = true;
            MessageBox.Show("Hello");
        }
    }

    public partial class MessageBoxShow
    {
        string _messageBoxText = "MessageBox Test";
        string _messageBoxCaption = "MessageBox Caption";

        void OnContentRendered(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(_messageBoxText, _messageBoxCaption, (MessageBoxButton)(5));
                Logger.LogFail("InvalidEnumArgumentException expected for invalid MessageBoxButton");
            }
            catch (InvalidEnumArgumentException ex)
            {
                Logger.Status("InvalidEnumArgumentException caught as expected for MessageBoxButton\n" + ex.ToString());
            }
            catch (System.Exception ex)
            {
                Logger.LogFail("Unexpected exception caught! " + ex.StackTrace);
            }

            try
            {
                MessageBox.Show(_messageBoxText, _messageBoxCaption, MessageBoxButton.OK, (MessageBoxImage)(100));
                Logger.LogFail("Invalid MessageBoxImage expects InvalidEnumArgumentException");
            }
            catch (InvalidEnumArgumentException ex)
            {
                Logger.Status("InvalidEnumArgumentException caught as expected for invalid MessageBoxImage\n" + ex.ToString());
            }
            catch (System.Exception ex)
            {
                Logger.LogFail("Unexpected exception caught! " + ex.StackTrace);
            }

            try
            {
                MessageBox.Show(_messageBoxText, _messageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Asterisk, (MessageBoxResult)8);
                Logger.LogFail("Invalid MessageBoxResult expects InvalidEnumArgumentException");
            }
            catch (InvalidEnumArgumentException ex)
            {
                Logger.Status("InvalidEnumArgumentException caught as expected for invalid MessageBoxResult\n" + ex.ToString());
            }
            catch (System.Exception ex)
            {
                Logger.LogFail("Unexpected exception caught! " + ex.StackTrace);
            }

            try
            {
                MessageBox.Show(_messageBoxText, _messageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Asterisk, MessageBoxResult.Cancel, (MessageBoxOptions)1);
                Logger.LogFail("Invalid MessageBoxOptions expects InvalidEnumArgumentException");
            }
            catch (InvalidEnumArgumentException ex)
            {
                Logger.Status("InvalidEnumArgumentException caught as expected for invalid MessageBoxOptions\n" + ex.ToString());
            }
            catch (System.Exception ex)
            {
                Logger.LogFail("Unexpected exception caught! " + ex.StackTrace);
            }

            // Specifically test ServiceNotification and DefaultDesktopOnly values
            // We want to make doubly sure no one can cast to these
            MessageBoxOptions options = MessageBoxOptions.ServiceNotification | MessageBoxOptions.DefaultDesktopOnly;
            try
            {
                MessageBox.Show(_messageBoxText, _messageBoxCaption, (MessageBoxButton)options);
                Logger.LogFail("Failed to get InvalidEnumArgumentException when casting ServiceNotification | DefaultDesktopOnly to MessageBoxButton");
            }
            catch (InvalidEnumArgumentException ex)
            {
                Logger.Status("InvalidEnumArgumentException caught as expected for ServiceNotification | DefaultDesktopOnly casted to MessageBoxButton\n" + ex.ToString());
            }
            catch (System.Exception ex)
            {
                Logger.LogFail("Unexpected exception caught! " + ex.StackTrace);
            }

            try
            {
                MessageBox.Show(_messageBoxText, _messageBoxCaption, MessageBoxButton.OK, (MessageBoxImage)options);
                Logger.LogFail("Failed to get InvalidEnumArgumentException when casting ServiceNotification | DefaultDesktopOnly to MessageBoxImage");
            }
            catch (InvalidEnumArgumentException ex)
            {
                Logger.Status("InvalidEnumArgumentException caught as expected for ServiceNotification | DefaultDesktopOnly casted to MessageBoxImage\n" + ex.ToString());
            }
            catch (System.Exception ex)
            {
                Logger.LogFail("Unexpected exception caught! " + ex.StackTrace);
            }

            try
            {
                MessageBox.Show(_messageBoxText, _messageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Asterisk, (MessageBoxResult)options);
                Logger.LogFail("Failed to get InvalidEnumArgumentException when casting ServiceNotification | DefaultDesktopOnly to MessageBoxResult");
            }
            catch (InvalidEnumArgumentException ex)
            {
                Logger.Status("InvalidEnumArgumentException caught as expected for ServiceNotification | DefaultDesktopOnly casted to MessageBoxResult\n" + ex.ToString());
            }
            catch (System.Exception ex)
            {
                Logger.LogFail("Unexpected exception caught! " + ex.StackTrace);
            }


            Thread WorkerThread = new Thread(WorkerThreadStart);
            WorkerThread.Start();
        }

        void OnButtonClick(object sender, RoutedEventArgs e)
        {
            Logger.LogFail("Test Failed. Button was clicked");
        }


        void WorkerThreadStart()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (DispatcherOperationCallback)delegate(object o)
            {
                if (MessageBox.Show(_messageBoxText) != MessageBoxResult.OK)
                    Logger.LogFail("MessageBox.Show(messageBoxText) did not return correct Result value");
                else
                    Logger.Status("MessageBox.Show(messageBoxText) returned correct result");
                return null;
            }, null);

            System.Threading.Thread.Sleep(500);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Enter, true);
            System.Threading.Thread.Sleep(500);

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (DispatcherOperationCallback)delegate(object o)
            {
                if (MessageBox.Show(_messageBoxText, _messageBoxCaption) != MessageBoxResult.OK)
                    Logger.LogFail("MessageBox.Show(messageBoxText, messageBoxCaption) did not return correct Result value");
                else
                    Logger.Status("MessageBox.Show(messageBoxText, messageBoxCaption) returned correct result");
                return null;
            }, null);

            
            System.Threading.Thread.Sleep(500);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Enter, true);
            System.Threading.Thread.Sleep(500);

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (DispatcherOperationCallback)delegate(object o)
            {
                if (MessageBox.Show(this, _messageBoxText)!= MessageBoxResult.OK)
                    Logger.LogFail("MessageBox.Show(this, messageBoxText) did not return correct Result value");
                else
                    Logger.Status("MessageBox.Show(this, messageBoxText) returned correct result");
                return null;
            }, null);

            System.Threading.Thread.Sleep(500);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Enter, true);
            System.Threading.Thread.Sleep(500);

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (DispatcherOperationCallback)delegate(object o)
            {
                if (MessageBox.Show(_messageBoxText, _messageBoxCaption, MessageBoxButton.OK) != MessageBoxResult.OK)
                    Logger.LogFail("MessageBox.Show(messageBoxText, messageBoxCaption, MessageBoxButton.OK) did not return correct Result value");
                else
                    Logger.Status("MessageBox.Show(messageBoxText, messageBoxCaption, MessageBoxButton.OK) returned correct result");

                return null;
            }, null);

            System.Threading.Thread.Sleep(500);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Enter, true);
            System.Threading.Thread.Sleep(500);
            
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (DispatcherOperationCallback)delegate(object o)
            {
                if (MessageBox.Show(this, _messageBoxText, _messageBoxCaption) != MessageBoxResult.OK)
                    Logger.LogFail("MessageBox.Show(this, messageBoxText, messageBoxCaption) did not return correct Result value");
                else
                    Logger.Status("MessageBox.Show(this, messageBoxText, messageBoxCaption) returned correct result");
                return null;
            }, null);

            System.Threading.Thread.Sleep(500);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Enter, true);
            System.Threading.Thread.Sleep(500);
            
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (DispatcherOperationCallback)delegate(object o)
            {
                if (MessageBox.Show(_messageBoxText, _messageBoxCaption, MessageBoxButton.OKCancel, MessageBoxImage.Asterisk) != MessageBoxResult.OK)
                    Logger.LogFail("MessageBox.Show(messageBoxText, messageBoxCaption, MessageBoxButton.OKCancel, MessageBoxImage.Asterisk) did not return correct Result value");
                else
                    Logger.Status("MessageBox.Show(messageBoxText, messageBoxCaption, MessageBoxButton.OKCancel, MessageBoxImage.Asterisk) returned correct result");

                return null;
            }, null);

            System.Threading.Thread.Sleep(500);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Enter, true);
            System.Threading.Thread.Sleep(500);
            
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (DispatcherOperationCallback)delegate(object o)
            {
                if (MessageBox.Show(this, _messageBoxText, _messageBoxCaption, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                    Logger.LogFail("MessageBox.Show(this, messageBoxText, messageBoxCaption, MessageBoxButton.YesNo) did not return correct Result value");
                else
                    Logger.Status("MessageBox.Show(this, messageBoxText, messageBoxCaption, MessageBoxButton.YesNo) returned correct result");
                return null;
            }, null);

            System.Threading.Thread.Sleep(500);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Enter, true);
            System.Threading.Thread.Sleep(500);
            
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (DispatcherOperationCallback)delegate(object o)
            {
                if (MessageBox.Show(_messageBoxText, _messageBoxCaption, MessageBoxButton.YesNoCancel, MessageBoxImage.Error) != MessageBoxResult.Yes)
                {
                    Logger.LogFail("MessageBox.Show(messageBoxText, messageBoxCaption, MessageBoxButton.YesNoCancel, MessageBoxImage.Error) did not return correct Result value");
                }
                else
                {
                    Logger.Status("MessageBox.Show(messageBoxText, messageBoxCaption, MessageBoxButton.YesNoCancel, MessageBoxImage.Error) returned correct result");
                }
                return null;
            }, null);

            System.Threading.Thread.Sleep(500);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Enter, true);
            System.Threading.Thread.Sleep(500);
            
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (DispatcherOperationCallback)delegate(object o)
            {
                if (MessageBox.Show(this, _messageBoxText, _messageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Exclamation) != MessageBoxResult.OK)
                {
                    Logger.LogFail("MessageBox.Show(this, messageBoxText, messageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Exclamation) did not return correct Result value");
                }
                else
                {
                    Logger.Status("MessageBox.Show(this, messageBoxText, messageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Exclamation) returned correct result");
                }
                return null;
            }, null);

            System.Threading.Thread.Sleep(500);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Enter, true);
            System.Threading.Thread.Sleep(500);
            
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (DispatcherOperationCallback)delegate(object o)
            {
                if (MessageBox.Show(_messageBoxText, _messageBoxCaption, MessageBoxButton.OKCancel, MessageBoxImage.Hand, MessageBoxResult.Cancel, MessageBoxOptions.DefaultDesktopOnly) != MessageBoxResult.Cancel)
                {
                    Logger.LogFail("MessageBox.Show(messageBoxText, messageBoxCaption, MessageBoxButton.OKCancel, MessageBoxImage.Hand, MessageBoxResult.Cancel, MessageBoxOptions.DefaultDesktopOnly) did not return correct Result value");
                }
                else
                {
                    Logger.Status("MessageBox.Show(messageBoxText, messageBoxCaption, MessageBoxButton.OKCancel, MessageBoxImage.Hand, MessageBoxResult.Cancel, MessageBoxOptions.DefaultDesktopOnly) returned correct result");
                }

                return null;
            }, null);

            System.Threading.Thread.Sleep(500);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Enter, true);
            System.Threading.Thread.Sleep(500);
            
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (DispatcherOperationCallback)delegate(object o)
            {
                if (MessageBox.Show(this, _messageBoxText, _messageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.OK)
                {
                    Logger.LogFail("MessageBox.Show(this, messageBoxText, messageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Question, MessageBoxResult.No) did not return correct Result value");
                }
                else
                {
                    Logger.Status("MessageBox.Show(this, messageBoxText, messageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Question, MessageBoxResult.No) returned correct result");
                }

                return null;
            }, null);

            System.Threading.Thread.Sleep(500);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Enter, true);
            System.Threading.Thread.Sleep(500);
            
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (DispatcherOperationCallback)delegate(object o)
            {
                if (MessageBox.Show(this, _messageBoxText, _messageBoxCaption, MessageBoxButton.OKCancel, MessageBoxImage.Stop, MessageBoxResult.None, MessageBoxOptions.None) != MessageBoxResult.OK)
                    Logger.LogFail("MessageBox.Show(this, messageBoxText, messageBoxCaption, MessageBoxButton.OKCancel, MessageBoxImage.Stop, MessageBoxResult.None, MessageBoxOptions.None) did not return correct Result value");
                else
                    Logger.Status("MessageBox.Show(this, messageBoxText, messageBoxCaption, MessageBoxButton.OKCancel, MessageBoxImage.Stop, MessageBoxResult.None, MessageBoxOptions.None) returned correct result");
                return null;
            }, null);

            System.Threading.Thread.Sleep(500);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Enter, true);
            System.Threading.Thread.Sleep(500);

            (new TestHelper()).TestCleanup();
            
        }
    }

    /// <summary>
    /// Regression (Setting Content to Null in NavigationWindow has no effect)
    /// </summary>
	
    public partial class REGRESSION_SetNullToNavigationWindowContent
    {
        int _clickCounter = 0;
        void OnLoaded(object sender, EventArgs e)
        {
            Button btn = new Button();
            btn.Click += OnClick;
            btn.IsDefault = true;
            Logger.Status("[SET] NavigationWindow.Content=DefaultButton");
            this.Content = btn;

            Thread WorkerThread = new Thread(WorkerThreadStart);
            WorkerThread.Start();
        }

        void WorkerThreadStart()
        {
            System.Threading.Thread.Sleep(3000);
            Logger.Status("[SEND INPUT] Enter Key. Expect ClickHit");
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Enter, true);
            
            System.Threading.Thread.Sleep(1000);
            Logger.Status("[SET] NavigationWindow.Content=null");

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (DispatcherOperationCallback)delegate(object o)
            {
                this.Content = null;
                return null;
            }, null);
            

            System.Threading.Thread.Sleep(3000);
            Logger.Status("[SEND INPUT] Enter Key. Not Expecting ClickHit");
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Enter, true);

            System.Threading.Thread.Sleep(1000);

            if (_clickCounter != 1)
                Logger.LogFail("ClickCounter should be 1, but it is " + _clickCounter.ToString());
            else
                Logger.LogPass("ClickCounter is as expected");

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (DispatcherOperationCallback)delegate(object o)
            {
                this.Close();
                return null;
            }, null);
        }

        void OnClick(object sender, RoutedEventArgs e)
        {
            _clickCounter++;
            Logger.Status("Button Clicked. (ClickCounter=" + _clickCounter.ToString() + ")");
        }
    }



    public partial class REGRESSION_ExceptionWhenChangingResizeModeInStateChanged
    {
        void OnContentRendered(object sender, EventArgs e)
        {
            Logger.Status("[SET] Window.ResizeMode = CanMinimize");
            this.ResizeMode = ResizeMode.CanMinimize;
            Logger.Status("[SET] Window.WindowState = Normal");
            this.WindowState = WindowState.Normal;
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            Logger.Status("[SET] Window.ResizeMode = CanResizeWithGrip");
            try
            {
                this.ResizeMode = ResizeMode.CanResizeWithGrip;
            }
            catch(System.Exception ex)
            {
                Logger.LogFail("Unexpected Exception Caught!\n" + ex.StackTrace);
            }
            Logger.LogPass("Test Passed");
            this.Close();
        }
    }

    public partial class SourceInitialized
    {
        int _SICounter = 0;
        void OnSourceInitialized(object sender, EventArgs e)
        {
            Logger.Status("SourceInitialized fired by " + ((Window)sender).Title);
            _SICounter++;
        }

        void OnContentRendered(object sender, EventArgs e)
        {
            Logger.Status("Attach and Remove SourceInitialized before Show()");
            Window win = new Window();
            win.Title = "win";
            win.SourceInitialized += OnSourceInitialized;
            win.SourceInitialized -= OnSourceInitialized;
            win.Show();
            win.Close();

            Logger.Status("Attach SourceInitialized before Show() and remove after Show()");
            Window win2 = new Window();
            win2.Title = "win2";
            win2.SourceInitialized += OnSourceInitialized;
            win2.Show();
            win2.SourceInitialized -= OnSourceInitialized;
            win2.Close();

            if (_SICounter != 2)
                Logger.LogFail("Expecting SourceInitialized to fire twice, but it actually fired " + _SICounter.ToString() + " times");
            else
                Logger.LogPass("Test Passed");
            this.Close();
        }


    }



    public partial class REGRESSION_CallCloseInSizeChangedAttachedBeforeShow
    {
        void OnContentRendered(object sender, EventArgs e)
        {
        }

        void OnSizeChanged(object sender, EventArgs e)
        {
            Logger.Status("[OnSizeChanged] Calling Close()");
            this.Close();
        }

        void OnClosed(object Sender, EventArgs e)
        {
            Logger.Status("Closed Event Fired - Test case passes if Window was not forced to close due to Timeout.");
            TestHelper.Current.TestCleanup();
        }
    }

    public partial class MethodShowDialog
    {
        bool? _expectedReturn = false;
        double _expectedWidthHeight = 200;
        double _expectedTopLeft = 0;
        Window _dialog;
        void OnContentRendered(object sender, EventArgs e)
        {
            try
            {
                this.ShowDialog();
                Logger.LogFail("InvalidOperationException is expected to call ShowDialog() to a visible Window");
            }
            catch(System.InvalidOperationException ex)
            {
                Logger.Status("InvalidOperationException caught as expected\n" + ex.ToString());
            }
            catch(System.Exception ex)
            {
                Logger.LogFail("Unexpected exception caught! " + ex.StackTrace);
            }

            _dialog = new Window();
            _dialog.Loaded += OnLoaded;
            _dialog.Height = _dialog.Width = _expectedWidthHeight;
            _dialog.Top = _dialog.Left = _expectedTopLeft;
            if (_dialog.ShowDialog() != _expectedReturn)
                Logger.LogFail("ShowDialog expected to return " + _expectedReturn.ToString());
            else
                Logger.Status("Return Value Validation Passed");

            TestHelper.Current.TestCleanup();

        }

        void OnLoaded(object sender, EventArgs e)
        {
            try
            {
                ((Window)sender).ShowDialog();
                Logger.LogFail("InvalidOperationException is expected to call ShowDialog() to a visible Dialog");
            }
            catch (System.InvalidOperationException ex)
            {
                Logger.Status("InvalidOperationException caught as expected\n" + ex.ToString());
            }
            catch (System.Exception ex)
            {
                Logger.LogFail("Unexpected exception caught! " + ex.StackTrace);
            }

            _dialog.DialogResult = _expectedReturn;
            Validate();
            //dialog.Close();

        }

        void Validate()
        {
            Logger.Status("[EXPECTED] ExpectedWidthHeight=" + _expectedWidthHeight.ToString() + " ExpectedTopLeft=" + _expectedTopLeft.ToString());
            // Validate Property Value
            if ((_dialog.Width != _expectedWidthHeight) ||
                (_dialog.Height != _expectedWidthHeight) ||
                (_dialog.ActualWidth != _expectedWidthHeight) ||
                (_dialog.ActualHeight != _expectedWidthHeight))
                Logger.LogFail("Dialog.Width=" + _dialog.Width.ToString() +
                               " Dialog.ActualWidth=" + _dialog.ActualWidth.ToString() +
                               " Dialog.Height=" + _dialog.Height.ToString() +
                               " Dialog.ActualHeight=" + _dialog.ActualHeight.ToString());
            else
                Logger.Status("[VALIDATION PASSED] Sizing Properties Value Validated");

            if ((_dialog.Top != _expectedTopLeft) ||
                (_dialog.Left != _expectedTopLeft))
                Logger.LogFail("Dialog.Top=" + _dialog.Top.ToString() + " Dialog.Left=" + _dialog.Left.ToString());
            else
                Logger.Status("[VALIDATION PASSED] Location Properties Value Validated");
        }
    }

    public partial class SizeToContent_WidthAndHeight_Markup
    {
        void OnClick(object sender, RoutedEventArgs e)
        {
            btn.Content = this.ActualWidth.ToString() + "x" + this.ActualHeight.ToString();
        }

        void OnContentRendered(object sender, EventArgs e)
        {
            double captionHeight = WindowValidator.GetCaptionHeight(this.Title);
            double borderLength = WindowValidator.GetBorderLength(this.Title);
            Logger.Status("borderLength=" + borderLength.ToString() + " CaptionHeight=" + captionHeight.ToString());

            SizeToContent expectedSTC = SizeToContent.WidthAndHeight;
            double expectedWindowWidth = btn.Width + borderLength * 2;
            double expectedWindowHeight = btn.Height + captionHeight + borderLength;
            double expectedContentWidth = btn.Width;
            double expectedContentHeight = btn.Height;
            ValidateActualDimension(expectedSTC, expectedWindowWidth, expectedWindowHeight);
            Win32ValidateContentDimension(expectedContentWidth, expectedContentHeight);

            expectedSTC = SizeToContent.Height;
            btn.Width = 300;
            btn.Height = 300;
            this.SizeToContent = expectedSTC;
            expectedWindowHeight = btn.Height + borderLength + captionHeight;
            expectedContentHeight = btn.Height;

            ValidateActualDimension(expectedSTC, expectedWindowWidth, expectedWindowHeight);
            Win32ValidateContentDimension(expectedContentWidth, expectedContentHeight);

            TestHelper.Current.TestCleanup();

        }

        void ValidateActualDimension(SizeToContent expectedSTC, double expectedWidth, double expectedHeight)
        {
            Logger.Status("[EXPECTED] Window.SizeToContent = " + expectedSTC.ToString());

            // Validate Property Value
            if (this.SizeToContent != expectedSTC)
            {
                Logger.LogFail("[ACTUAL] Window.SizeToContent = " + this.SizeToContent.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] SizeToContent = " + expectedSTC.ToString());
            }

            Logger.Status("[EXPECTED] Window.ActualWidth = " + expectedWidth.ToString());
            if (!TestUtil.IsEqual(this.ActualWidth, expectedWidth))
            {
                Logger.LogFail("[ACTUAL] Window.ActualWidth = " + this.ActualWidth.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Window.ActualWidth = " + this.ActualWidth.ToString());
            }

            Logger.Status("[EXPECTED] Window.ActualHeight = " + expectedHeight.ToString());
            if (!TestUtil.IsEqual(this.ActualHeight, expectedHeight))
            {
                Logger.LogFail("[ACTUAL] Window.ActualHeight = " + this.ActualHeight.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Window.ActualHeight = " + this.ActualHeight.ToString());
            }
        }

        void Win32ValidateContentDimension(double expectedContentWidth, double expectedContentHeight)
        {
            // win32 width validation
            Logger.Status("[WIN32 VALIDATION]");
            if (!WindowValidator.ValidateSizeToContent(this.Title, expectedContentWidth, expectedContentHeight))
            {
                Logger.LogFail("Win32 Validation failed!");
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Win32 Validation Passed");
            }
        }
    }

    public partial class ClipToBounds
    {
        private void OnContentRendered(object sender, EventArgs e)
        {
            Validate();

            try
            {
                this.ClipToBounds = true;
                Logger.LogFail("Excepting InvalidOperationException for setting ClipToBounds=False");
            }
            catch (System.InvalidOperationException ex)
            {
                Logger.Status("InvalidOperationException caught as expected\n" + ex.ToString());
            }
            catch (System.Exception ex)
            {
                Logger.LogFail("Unexpected exception caught. " + ex.StackTrace);
            }

            Validate();

            try
            {
                this.ClipToBounds = false;
            }
            catch (System.Exception ex)
            {
                Logger.LogFail("Unexpected exception caught. " + ex.StackTrace);
            }

            Validate();

            (new TestHelper()).TestCleanup();

        }

        private void Validate()
        {
            if (this.ClipToBounds)
                Logger.LogFail("ClipToBounds should be false");
            else
                Logger.Status("ClipToBounds Verified");
        }
    }

    public partial class AllowsTransparency
    {
        private void OnContentRendered(object sender, EventArgs e)
        {
            Window bgWindow = new BackgroundWindow();
            bgWindow.Background = Brushes.Blue;
            bgWindow.Show();

            this.Activate();
            // try chaning AT=false
            Logger.Status("[SET] AllowsTransparency=False");
            try
            {
                this.AllowsTransparency = !this.AllowsTransparency;
                Logger.LogFail("InvalidOperationException expected");
            }
            catch (System.InvalidOperationException ex)
            {
                Logger.Status("InvalidOperationException as expected\n" + ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogFail("Unexpected exception caught!" + ex.ToString());
            }

            // try Show() without Setting WindowStyle=None
            Logger.Status("Try Show() without Setting WindowStyle=None");
            try
            {
                Window win = new Window();
                win.AllowsTransparency = true;
                win.Show();
            }
            catch (System.InvalidOperationException ex)
            {
                Logger.Status("InvalidOperationException as expected\n" + ex.Message);
            }
            catch(Exception ex)
            {
                Logger.LogFail("Unexpected exception caught!" + ex.ToString());
            }

            // try ShowDialog() without Setting WindowStyle=None
            Logger.Status("Try ShowDialog() without Setting WindowStyle=None");
            try
            {
                Window win = new Window();
                win.AllowsTransparency = true;
                win.WindowStyle = WindowStyle.ToolWindow;
                win.ShowDialog();
            }
            catch (System.InvalidOperationException ex)
            {
                Logger.Status("InvalidOperationException as expected\n" + ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogFail("Unexpected exception caught!" + ex.ToString());
            }

            (new TestHelper()).TestCleanup();

        }
    }

    public partial class WindowStartupLocation_Markup
    {
        void OnContentRendered(object sender, EventArgs e)
        {
            Logger.Status("[SET] WindowStartupLocation to Invalid Enum and expect InvalidEnumArgumentException");
            try
            {
                this.WindowStartupLocation = (WindowStartupLocation)(3);
                Logger.LogFail("InvalidEnumArgumentException did not get caught!");
            }
            catch (InvalidEnumArgumentException ex)
            {
                Logger.Status("InvalidEnumArgumentException caught as expected!\n" + ex.ToString());
            }
            catch (System.Exception ex)
            {
                Logger.LogFail("Unexpected exception caught!\n" + ex.StackTrace);
            }

            Logger.Status("[EXPECTED] WindowStartupLocation=CenterScreen");
            if (this.WindowStartupLocation != WindowStartupLocation.CenterScreen)
                Logger.LogFail("[ACTUAL] WindowStartupLocation=" + this.WindowStartupLocation.ToString());
            else
                Logger.Status("Property Validation Passed");

            if (WindowValidator.ValidateWindowStartupLocation(this.Title, WindowStartupLocation.CenterScreen))
                Logger.Status("[VALIDATION PASSED] Win32 Validation Passed");
            else
                Logger.LogFail("Win32 Validation Failed!");

            TestHelper.Current.TestCleanup();
        }
    }

    public partial class PageProperties
    {
        Page _page1,_page2;
        bool _endTest = false;
        void OnWindowLoaded(object sender, EventArgs e)
        {
            _page2 = new Page();
            _page2.Loaded += new RoutedEventHandler(Page2_Loaded);
            _page2.ShowsNavigationUI = true;
            _page2.WindowTitle = "WindowPage2";
            _page2.Title = "Page2";
            _page2.Width = 200;
            _page2.Height = 250;
            _page2.WindowWidth = 300;
            _page2.WindowHeight = 350;
            _page2.KeepAlive = false;
            _page2.Content = new CheckBox();

            _page1 = new Page();
            _page1.WindowTitle = "WindowPage1";
            _page1.Loaded += new RoutedEventHandler(Page1_Loaded);
            _page1.Title = "Page1";
            _page1.FontFamily = new FontFamily("Arial");
            _page1.FontSize = 12;
            _page1.Foreground = Brushes.Aqua;
            _page1.Background = Brushes.Brown;
            _page1.KeepAlive = true;
            _page1.ShowsNavigationUI = false;
            _page1.WindowWidth = 200;
            _page1.WindowHeight = 250;
            _page1.Width = 100;
            _page1.Height = 150;
            _page1.Content = new Button();

            this.Navigate(_page1);
            
        }

        void Page1_Loaded(object sender, RoutedEventArgs e)
        {
            if (_page1.ShowsNavigationUI ||
                _page1.WindowTitle != "WindowPage1" ||
                _page1.Title != "Page1" ||
                !_page1.FontFamily.Source.Equals("Arial") ||
                _page1.FontSize != 12 ||
                _page1.Foreground != Brushes.Aqua ||
                _page1.Background != Brushes.Brown ||
                !_page1.KeepAlive ||
                _page1.WindowWidth != 200 ||
                _page1.WindowHeight != 250 ||
                this.ActualWidth != 200 ||
                this.ActualHeight != 250 ||
                _page1.Width != 100 ||
                _page1.Height != 150)
                // Validate Page.KeepAlive??
                Logger.LogFail("One or more Page1 properties failed to validate");
            else
                Logger.Status("[VALIDATED] All Page1 properties returned as expected");

            if (_endTest)
            {
                (new TestHelper()).TestCleanup();
            }
            else
            {
                this.Navigate(_page2);
            }
        }

        void Page2_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_page2.ShowsNavigationUI ||
                _page2.WindowTitle != "WindowPage2" ||
                _page2.Title != "Page2" ||
                _page2.KeepAlive ||
                _page2.WindowWidth != 300 ||
                _page2.WindowHeight != 350 ||
                this.ActualWidth != 300 ||
                this.ActualHeight != 350 ||
                _page2.Width != 200 ||
                _page2.Height != 250)
                Logger.LogFail("One or more Page2 properties failed to validate");
            else
                Logger.Status("[VALIDATED] All Page2 properties returned as expected");

            _endTest = true;
            Logger.Status("GoBack to Page1");
            this.GoBack(); // GoBack and Validate Page1 properties 
        }
    }

    public partial class OverrideApplicationEvents
    {
        Thread _controlThread;
        Hashtable _actualHit = new Hashtable();
        Hashtable _expectedHit = new Hashtable();
        string _validatingEvent = null; 
        Window _childWin,_parentWin;
        Dispatcher _childWinDispatcher,_parentWinDispatcher;

        private void DetachEventes()
        {
            this.Startup -= OnAppStartup;
            this.Exit -= OnAppExit;
            this.SessionEnding -= OnAppSessionEnding;
            this.Deactivated -= OnAppDeactivated;
            this.Activated -= OnAppActivated;
            this.DispatcherUnhandledException -= OnAppDispatcherException;
        }
        
        private void PopulateHitCounts()
        {
            _expectedHit.Add("OnStartup", 1);
            _expectedHit.Add("OnActivated", 3);
            _expectedHit.Add("OnDeactivated", 3);
            _expectedHit.Add("OnSessionEnding", 0); // 
            _expectedHit.Add("OnExit", 1);
            _expectedHit.Add("OnAppStartup", 1);
            _expectedHit.Add("OnAppActivated", 2);
            _expectedHit.Add("OnAppDeactivated", 1);
            _expectedHit.Add("OnAppSessionEnding", 0);
            _expectedHit.Add("OnAppExit", 0);
            _expectedHit.Add("OnAppDispatcherException", 0);

            _actualHit.Add("OnStartup", 0);
            _actualHit.Add("OnDeactivated", 0);
            _actualHit.Add("OnActivated", 0);
            _actualHit.Add("OnSessionEnding", 0); // 
            _actualHit.Add("OnExit", 0);
            _actualHit.Add("OnAppStartup", 0);
            _actualHit.Add("OnAppActivated", 0);
            _actualHit.Add("OnAppDeactivated", 0);
            _actualHit.Add("OnAppSessionEnding", 0);
            _actualHit.Add("OnAppExit", 0);
            _actualHit.Add("OnAppDispatcherException", 0);            
        }
        
        private void RecordHit(string key)
        {
            int val = 0;
            if (_actualHit[key] != null)
                val = (int)_actualHit[key];
           
            _actualHit[key] = ++val;
            Logger.Status("  --> Recording Event [" + key + "] - HitCount=" + val.ToString());
            return;
        }

        private void VaidateWindowProperties(Window win)
        {
            //TO BE ADDED LATER
        }

        private void ValidateResult()
        {
            Logger.Current.LogStatus("** HITCOUNT VALIDATION **");
            if (String.IsNullOrEmpty(_validatingEvent))
            {
                IDictionaryEnumerator enumerator = _expectedHit.GetEnumerator();
                while ( enumerator.MoveNext() )
                {
                    if ((int)enumerator.Value != (int)_actualHit[enumerator.Key])
                        Logger.LogFail( "** FAIL! ** EXPECTED=" + enumerator.Value.ToString() + " ACTUAL=" + _actualHit[enumerator.Key.ToString()].ToString() + " [" + enumerator.Key.ToString() + "]");
                    else
                        Logger.Current.LogStatus("** PASS! ** EXPECTED=" + enumerator.Value.ToString() + " ACTUAL=" + _actualHit[enumerator.Key.ToString()].ToString() + " [" + enumerator.Key.ToString() + "]");
                }
            }
            else
            {
                if (!_expectedHit.ContainsKey(_validatingEvent) || !_actualHit.ContainsKey(_validatingEvent))
                {
                    Logger.LogFail( _validatingEvent + " is an unrecognized application event");
                    return;
                }
                
                if ((int)_expectedHit[_validatingEvent] != (int)_actualHit[_validatingEvent])
                    Logger.LogFail( "** FAIL! ** EXPECTED=" + _expectedHit[_validatingEvent].ToString() + " ACTUAL=" + _actualHit[_validatingEvent].ToString() + " [" + _validatingEvent + "]");
                else
                    Logger.LogPass( "** PASS! ** EXPECTED=" + _expectedHit[_validatingEvent].ToString() + " ACTUAL=" + _actualHit[_validatingEvent].ToString() + " [" + _validatingEvent + "]");
            }

            Logger.LogPass( "TEST COMPLETED");
            TestHelper.CloseAndSetVariationResults();
        }
        

        public void ControlThreadEntry()
        {
            // Activate Parent Window [No change in event counter]
            Logger.Current.LogStatus("ACTION: parentWin.Activate()");
            _parentWinDispatcher.Invoke(DispatcherPriority.Normal, (DispatcherOperationCallback) delegate (object o) 
                { 
                    _parentWin.Activate();
                    return null;
                }, null);

            Logger.Current.LogStatus("ACTION: Sleep()");
            System.Threading.Thread.Sleep(1000);
            
            // Activate Child window [No change in event counter]
            Logger.Current.LogStatus("ACTION: childWin.Activate()");
            _childWinDispatcher.Invoke(DispatcherPriority.Normal, (DispatcherOperationCallback) delegate (object o)
                {
                    _childWin.Activate(); 
                    return null;
                }, null);
            

            // Change Window States [No change in event counter]
            Logger.Current.LogStatus("ACTION: ParentWindow.Minimize");
            _childWinDispatcher.Invoke(DispatcherPriority.Normal, (DispatcherOperationCallback) delegate (object o)
                {
                    _childWin.WindowState = WindowState.Minimized;
                    return null;
                 }, null);           

            Logger.Current.LogStatus("ACTION: Sleep()");
            System.Threading.Thread.Sleep(2000);

            // Kill focus on both Windows [Deactivate + 1]
            Logger.Current.LogStatus("ACTION: Desktop.Click()");
            _parentWinDispatcher.Invoke(DispatcherPriority.Normal, (DispatcherOperationCallback) delegate (object o) 
            { 
            MTI.Input.MoveToAndClick(new Point(SystemParameters.PrimaryScreenWidth,                     SystemParameters.PrimaryScreenHeight));
                return null;
            },null);

            Logger.Current.LogStatus("ACTION: Sleep()");
            System.Threading.Thread.Sleep(1000);

            // Activate Parent Window --> [Activate + 1]
            Logger.Current.LogStatus("ACTION: parentWin.Activate()");
            _parentWinDispatcher.Invoke(DispatcherPriority.Normal, (DispatcherOperationCallback) delegate (object o) 
                { 
                    _parentWin.Activate();
                    return null;
                }, null);


            // Close child Window
            Logger.Current.LogStatus("ACTION: childWin.Close()");
            _childWinDispatcher.Invoke(DispatcherPriority.Normal, (DispatcherOperationCallback) delegate (object o) 
                {
                    _childWin.Close(); 
                    return null;
                }, null);


            // Close parent Window [No change in event counter]
            // This should NOT shuttdown app. Since first time closing will be cancelled
            Logger.Current.LogStatus("ACTION: parentWin.Close()");
            _parentWinDispatcher.Invoke(DispatcherPriority.Normal, (DispatcherOperationCallback) delegate (object o) 
                {
                    _parentWin.Close(); 
                    return null;
                }, null);


            // Remove Event Handlers [No change in event counter]
            Logger.Current.LogStatus("ACTION: Application.EventsDetach()");           
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (DispatcherOperationCallback) delegate (object o) 
                {
                    DetachEventes();
                    return null;
                }, null);                

            Logger.Current.LogStatus("ACTION: Sleep()");
            System.Threading.Thread.Sleep(2000);

            // Kill focus on both Windows [Deactivate + 1]
            Logger.Current.LogStatus("ACTION: Desktop.Click()");
            _parentWinDispatcher.Invoke(DispatcherPriority.Normal, (DispatcherOperationCallback) delegate (object o) 
                { 
                    MTI.Input.MoveToAndClick(new Point(SystemParameters.PrimaryScreenWidth,                             SystemParameters.PrimaryScreenHeight));
                        return null;
                }, null);

            Logger.Current.LogStatus("ACTION: Sleep()");
            System.Threading.Thread.Sleep(1000);
            
            // Activate parent window [Activated + 1]
            Logger.Current.LogStatus("ACTION: parentWin.Activate()");
            _parentWinDispatcher.Invoke(DispatcherPriority.Normal, (DispatcherOperationCallback) delegate (object o) 
                {
                    _parentWin.Activate(); 
                    return null;
                }, null);


            Logger.Current.LogStatus("ACTION: Sleep()");
            System.Threading.Thread.Sleep(1000);

            _parentWinDispatcher.Invoke(DispatcherPriority.Normal, (DispatcherOperationCallback) delegate (object o)
                {
                    _parentWin.Close(); 
                    return null;
                }, null);

        }
            

        protected override void OnDeactivated(EventArgs e)
        {
            RecordHit("OnDeactivated");
            base.OnDeactivated(e);
        }

        protected override void OnActivated(EventArgs e)
        {
            RecordHit("OnActivated");
            base.OnActivated(e);
        }

        protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
        {
            RecordHit("OnSessionEnding");
            base.OnSessionEnding(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            RecordHit("OnExit");
            base.OnExit(e);

            // Validate Result
            ValidateResult();
        }

        protected override void OnStartup(StartupEventArgs e)
        {   
            if (e.Args.Length != 1)
                _validatingEvent = e.Args[1];

            PopulateHitCounts();

            RecordHit("OnStartup");
            base.OnStartup(e);
        }

        private void OnAppStartup(object sender, StartupEventArgs e)
        {           
            RecordHit("OnAppStartup");   
            _childWin = new Window();
            _childWinDispatcher = _childWin.Dispatcher;
            _childWin.Title = "child Window";
            _childWin.Top = 20;
            _childWin.Left = 20;
            _childWin.Width=150;
            _childWin.Height = 150;
            
            _parentWin = new Window();
            _parentWinDispatcher = _parentWin.Dispatcher;
            _parentWin.Title = "parent Window";
            _parentWin.Top = 0;
            _parentWin.Left = 0;
            _parentWin.Width = 200;
            _parentWin.Height = 200;
            _parentWin.Closing += OnParentWinClosing;

            _parentWin.Show(); 
            _childWin.Owner = _parentWin;
            _childWin.Show();

            _controlThread = new Thread(new ThreadStart(ControlThreadEntry));
            _controlThread.Start();
        }
        
        private void OnAppActivated(object sender, EventArgs e)
        {
            RecordHit("OnAppActivated");
        }

        private void OnAppDeactivated(object sender, EventArgs e)
        {
            RecordHit("OnAppDeactivated");
        }

        private void OnAppSessionEnding(object sender, EventArgs e)
        {
            RecordHit("OnAppSessionEnding");
        }

        private void OnAppExit(object sender, EventArgs e)
        {
            RecordHit("OnAppExit");
        }

        private void OnAppDispatcherException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            RecordHit("OnAppDispatcherException");
        }

        private void OnParentWinClosing(object sender, CancelEventArgs e)
        {
            Logger.Current.LogStatus("  --> Cancel closing");
            e.Cancel = true;

            Logger.Current.LogStatus("  --> parentWin.Closing.Detach()");
            _parentWin.Closing -= OnParentWinClosing;
        }
    }

    // we needed more code coverage on WindowInteropHelper, so Microsoft added this case.
    public partial class WindowInteropHelperTest
    {
        void OnContentRendered(object sender, EventArgs e)
        {
            bool caught = false;
            WindowInteropHelper wih;
            
            // Case 1: Call WindowInteropHelper's constructor, passing it a null Window argument.
            // We expect WPF to throw an ArgumentException in this case.

            Logger.Status("Call WindowInteropHelper ctor null window arg");
            try
            {
                wih = new WindowInteropHelper(null);
            }
            catch (ArgumentException)
            {
                caught = true;
            }

            if (caught)
            {
                Logger.LogPass("Caught expected ArgumentException");
            }
            else
            {
                Logger.LogFail("Failed to catch expected ArgumentException");
            }

            Window newWindow = new Window();
            newWindow.Owner = Application.Current.MainWindow;
            newWindow.Show();
            wih = new WindowInteropHelper(newWindow);

            //Case 2: Test WindowInteropHelper.Owner property get.
            //We create a window, assigning the owner object to be the app's main window.
            //Then we call WindowInteropHelper.Owner on the window we created to get the Owner's window handle.
            //To verify it, we compare it to the main window's handle (since they should be the same).

            Logger.Status("[GET] WindowInteropHelper Owner property");
            IntPtr ownerHandle = wih.Owner;
            IntPtr parentHandle = new WindowInteropHelper(Application.Current.MainWindow).Handle;
            if (parentHandle == ownerHandle)
            {
                Logger.LogPass("Owner handle matches parent handle");
            }
            else
            {
                Logger.LogFail("Owner handle does not match parent handle");
            }

            //Case 3: Test WindowInteropHelper.Owner property set.
            //Set the Owner property to a null window handle (IntPtr.Zero).  Verify it got set to zero.

            Logger.Status("[SET] WindowInteropHelper Owner property");
            wih.Owner = IntPtr.Zero;
            if (wih.Owner == IntPtr.Zero)
            {
                Logger.LogPass("Owner handle was set to zero");
            }
            else
            {
                Logger.LogFail("Owner handle was not set to zero");
            }

            newWindow.Close();
            this.Close();
        }
    }

    //set WindowState to maximized and ShowActivated to false.  Catch expected InvalidOperationException on Show.
    public partial class ShowActivatedWhenMaximizedTest
    {
        void OnLoaded(object sender, EventArgs e)
        {
            Logger.Status("Set  ShowActivated = false and WindowState = maximized");
            ShowActivated = false;
            WindowState = WindowState.Maximized;
            bool caught = false;

            try
            {
                Show();
            }
            catch (InvalidOperationException)
            {
                caught = true;
            }
            catch (Exception ex)
            {
                Logger.LogFail("Caught unexpected Exception: " + ex.ToString());
            }

            if (caught)
            {
                Logger.LogPass("Caught expected InvalidOperationException");
            }
            else
            {
                Logger.LogFail("Failed to catch expected InvalidOperationException");
            }
            this.Close();
            
        }
    }

    // Set WindowState to minimized, then ShowActivated to false.  Set WindowState to normal and verify we didn't activate.
    public partial class ShowActivatedWhenMinimizedTest
    {
        void OnContentRendered(object sender, EventArgs e)
        {
            Logger.Status("Setting WindowState = minimized");

            WindowState = WindowState.Minimized;

            try
            {
                Logger.Status("Setting ShowActivated = false");
                ShowActivated = false;
            }
            catch (Exception ex)
            {
                Logger.LogFail("Caught unexpected Exception: " + ex.ToString());
            }

            WindowInteropHelper helper = new WindowInteropHelper(this);
            IntPtr hwnd = helper.Handle;

            Logger.Status("Setting WindowState = normal");
            WindowState = WindowState.Normal;

            if (hwnd == TestUtil.GetActiveWindow())
            {
                Logger.LogFail("Window was activated when it should not have been");
            }
            else
            {
                Logger.LogPass("As expected, window was not activated");
            }

            this.Close();
        }
    }

    // In markup, set Window.Topmost = true, and ShowActivated to false. Show window.  Verify we didn't activate.
    public partial class ShowActivatedWhenTopmostTest
    {
        void OnContentRendered(object sender, EventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            IntPtr hwnd = helper.Handle;

            if (hwnd == TestUtil.GetActiveWindow())
            {
                Logger.LogFail("Window was activated when it should not have been");
            }
            else
            {
                Logger.LogPass("As expected, window was not activated");
            }

            this.Close();
        }
    }

    // Create a modal child dialog, then another modal child dialog from that dialog.  Both dialogs have the main window as owner.
    // Close the sub-dialog.  Expect the remaining modal dialog to have focus.
    public partial class ModalDialogFocusReturn
    {                   
        Window _dialogWindow,_dialogSubWindow;
           
        void OnContentRendered(object sender, EventArgs e)
        {
            _dialogWindow = new Window();
            _dialogWindow.Left = 100;
            _dialogWindow.Top = 100;
            _dialogWindow.Width = 200;
            _dialogWindow.Height = 200;
            _dialogWindow.Title = "DialogWindow";
            _dialogWindow.Content = new DockPanel();
            _dialogWindow.Owner = this;
                
            Logger.Status("Creating ContentRendered event handler for dialogWindow");
            _dialogWindow.ContentRendered += new EventHandler(DialogWindow_ContentRendered);            

            Logger.Status("Showing dialogWindow");
            _dialogWindow.ShowDialog();
        }

        private void DialogWindow_ContentRendered(object sender, EventArgs e)
        {
            Logger.Status("dialogWindow shown, creating dialogSubWindow");
            _dialogSubWindow = new Window();
            _dialogSubWindow.Left = 300;
            _dialogSubWindow.Top = 300;
            _dialogSubWindow.Width = 200;
            _dialogSubWindow.Height = 200;
            _dialogSubWindow.Title = "DialogSubWindow";
            _dialogSubWindow.Content = new DockPanel();
            _dialogSubWindow.Owner = this;
                
            Logger.Status("Creating ContentRendered event handler for dialogSubWindow");
            _dialogSubWindow.ContentRendered += new EventHandler(DialogSubWindow_ContentRendered);            

            Logger.Status("Showing dialogSubWindow");
            _dialogSubWindow.ShowDialog();

            //After subdialog is closed, we return here.  Verify which window is active.
            IntPtr hwnd = new WindowInteropHelper(sender as Window).Handle;
            IntPtr hwndMain = new WindowInteropHelper(this).Handle;
            IntPtr hwndActive = TestUtil.GetActiveWindow();

            if(hwndActive == hwnd)
            {
                Logger.LogPass("dialogWindow is active, test passed.");
            }
            else if(hwndActive == hwndMain)
            {
                Logger.LogFail("Main window was activated instead of dialog.");
            }
            else
            {
                Logger.LogFail("Unknown window was activated instead of dialog.");
            }
            Application.Current.Shutdown();
        }

        private void DialogSubWindow_ContentRendered(object sender, EventArgs e)
        {
            (sender as Window).Close();
        }
    }

    // Regression test for DevDivBugs # 186010
    // Make sure HwndSourceParameters.Equals supports null input
    public partial class HwndSourceParametersEqualsNull
    {
        void OnContentRendered(object sender, EventArgs e)
        {
            HwndSourceParameters parameters = new HwndSourceParameters();
            bool returnValue = true;
            bool caught = false;
            try
            {
                Logger.Status("calling HwndSourceParameters.Equals(null)");
                returnValue = parameters.Equals(null);
            }
            catch (Exception exception)
            {
                Logger.Status("Got exception: " + exception.ToString());
                caught = true;
            }

            if (caught || (returnValue == true))
            {
                Logger.LogFail("Calling HwndSourceParameters.Equals(null) threw an exception or returned true - test fails");
            }
            else
            {
                Logger.LogPass("As expected, calling HwndSourceParameters.Equals(null) returned false.");
            }

            this.Close();
        }
    }

    // Regression test for DevDivBugs # 189790
    // Create a window, set Visibility to Visible, and call Close.  Expect the window to close, not be shown.
    public partial class VisibleBeforeClose
    {
        string _state = "";
        DispatcherTimer _timer = new DispatcherTimer();

        void OnContentRendered(object sender, EventArgs e)
        {
            Logger.Status("Creating new window");
            Window testWindow = new Window();

            Logger.Status("Setting window to Visibility.Visible");
            testWindow.Visibility = Visibility.Visible;

            testWindow.Closed += new EventHandler(window_Closed);
            testWindow.Activated += new EventHandler(window_Activated);

            Logger.Status("Closing window");
            testWindow.Close();

            _timer.Interval = new TimeSpan(0,0,2);
            _timer.Tick += new EventHandler(timer_Tick);
            _timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();

            if (_state != "closed, ")
            {
                Logger.LogFail("State should have been 'closed, ' .  Instead it was:  '" + _state + "'");
            }
            else
            {
                Logger.LogPass("As expected, window was closed and not activated.");
            }

            this.Close();
        }

        void window_Activated(object sender, EventArgs e)
        {
            Logger.Status("Activated event fired");
            _state += "activated, ";
        }

        void window_Closed(object sender, EventArgs e)
        {
            Logger.Status("Closed event fired");
            _state += "closed, ";
        }
    }

    // Regression test
	// Test that specifying window sizing and MinHeight/Width via style, then showing the window, sizes the window to the proper size 500x500
    public partial class SetMinHeightWidthAfterEnsureHandle
    {
        void OnInitialized(object sender, EventArgs e)
        {
            WindowInteropHelper wih = new WindowInteropHelper(this);

// Needs #if to compile for 3.5.  The test will fail if run as a 4.0 test.
#if TESTBUILD_CLR40
            wih.EnsureHandle();
#endif
            MinWidth = 200;
            MinHeight = 200;
        }

        void OnContentRendered(object sender, EventArgs e)
        {
            if((ActualWidth == 500) && (ActualHeight == 500))
            {
                Logger.LogPass("As expected, window was sized to 500 x 500.");
            }
            else
            {
                Logger.LogFail("Window width/height should have been 500 x 500.  Instead it was: " + ActualWidth +  " x " + ActualHeight);
            }

            Close();
        }
    }

}
