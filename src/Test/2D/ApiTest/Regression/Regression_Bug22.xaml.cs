// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics.Regression
{
    /// <summary>
    /// Regression Test for Regression_Bug240
    /// (Using RenderTargetBitmap from the Loaded callback messes up the display.)
    ///
    /// Test Logic
    ///    - Create two identical windows (MasterWindow and CompareWindow)
    ///    - Attach Loaded event on both window objects
    ///    - Inside CompareWindow's Loaded event, create RenderTargetBitmap and call Render()
    ///      ---> this should reproduce the bug without fix.
    ///    - Take screenshot of both window objects inside their Loaded events
    ///    - Compare screenshots. No tolerance curve is needed since they are expected to be identical.
    /// </summary>
    public partial class Regression_Bug22 : System.Windows.Application
    {
        IImageAdapter _masterImageAdapter,_compareImageAdapter;
        Window _masterWindow,_compareWindow;
        
        
        protected override void OnStartup(StartupEventArgs e)
        {
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            CommonLib.Log.LogStatus("Creating window objects");
            _masterWindow = CreateWindow();
            _compareWindow = CreateWindow();

            // Updating CompareWindow's left property so it appears 
            // next to MasterWindow
            CommonLib.Log.LogStatus("Updating CompareWindow position");
            _compareWindow.Left = _masterWindow.Left + _masterWindow.Width;

            CommonLib.Log.LogStatus("Attaching Loaded event handlers");
            _masterWindow.Loaded += new RoutedEventHandler(MasterWindow_Loaded);
            _compareWindow.Loaded += new RoutedEventHandler(CompareWindow_Loaded);

            CommonLib.Log.LogStatus("Call Show() on both windows");
            _masterWindow.Show();
            _compareWindow.Show();

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (DispatcherOperationCallback)delegate(object o)
            {
                CommonLib.Log.LogStatus("Compare Screenshots");
                Compare();
                CommonLib.Log.LogStatus("Shutting down app");
                this.Shutdown();
                return null;
            },
            null
            );

            base.OnStartup(e);
        }

        private Window CreateWindow()
        {
            Window win = new Window();
            win.WindowStyle = WindowStyle.None;
            win.ResizeMode = ResizeMode.NoResize;
            win.Width = win.Height = 200;
            win.Top = win.Left = 0;

            Button btn = new Button();
            btn.Content = "Hello World";
            btn.Width = btn.Height = 150;

            TextBox textbox = new TextBox();
            textbox.Width = 150;
            textbox.Text = "Hello World";

            StackPanel panel = new StackPanel();
            panel.Children.Add(btn);
            panel.Children.Add(textbox);

            win.Content = panel;
            
            return win;
        }
        

        private void MasterWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (DispatcherOperationCallback)delegate(object o)
            {
                System.Threading.Thread.Sleep(1000);
                _masterImageAdapter = new ImageAdapter(new System.Drawing.Rectangle((int)_masterWindow.Left, (int)_masterWindow.Top, (int)_masterWindow.ActualWidth, (int)_masterWindow.ActualHeight));

                CommonLib.Log.LogStatus("Closing MasterWindow");
                _masterWindow.Close();
                
                return null;
            },
            null
            );

        }

        private void CompareWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)_compareWindow.Width,
                                                            (int)_compareWindow.Height,
                                                            96,
                                                            96,
                                                            PixelFormats.Pbgra32);

            bmp.Render(_compareWindow);

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (DispatcherOperationCallback)delegate(object o)
            {
                System.Threading.Thread.Sleep(1000);
                _compareImageAdapter = new ImageAdapter(new System.Drawing.Rectangle((int)_compareWindow.Left, (int)_compareWindow.Top, (int)_compareWindow.ActualWidth, (int)_compareWindow.ActualHeight));

                CommonLib.Log.LogStatus("Closing CompareWindow");
                _compareWindow.Close();
                
                return null;
                
            },
            null
            );
        }

        private void Compare()
        {
            ImageComparator comparator = new ImageComparator();
            bool TestPassed = comparator.Compare(_compareImageAdapter, _masterImageAdapter, true);
    
            if (!TestPassed)
            {
                CommonLib.Log.Result = TestResult.Fail;
                CommonLib.Log.LogEvidence("Visual Validation Failed. Saving Failure analysis package");
                Package package = Package.Create(".\\FailurePackage.vscan",
                                                  ImageUtility.ToBitmap(_compareImageAdapter),
                                                  ImageUtility.ToBitmap(_masterImageAdapter),
                                                  comparator.Curve.CurveTolerance.WriteToleranceToNode());
                package.Save();
                CommonLib.Log.LogFile(package.PackageName);
            }
            else
            {
                CommonLib.Log.Result = TestResult.Pass;
                CommonLib.Log.LogEvidence("Test Passed");
            }
        }
    }
}