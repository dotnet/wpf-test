// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Visual verification of HwndSources
 *
 * Contributor: Microsoft
 *
 
  
 * Revision:         $Revision: 2 $
 
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
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
//using Avalon.Test.CoreUI.Trusted.Controls;
//using Avalon.Test.Framework.Dispatchers;
//using Avalon.Test.Host.Controls;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.RenderingVerification.Model.Analytical;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.Hosting
{


    ///<summary>
    /// VScan based tests for HwndSource.
    ///</summary>
    //[CoreTestsLoader(CoreTestsTestType.MethodBase)]
    [TestDefaults]
    public class HwndSourceSimpleVscan
    {
        private static string s_masterImage = "HwndSourceSimple.png";

        /// <summary>
        /// Vscan a simple HwndSource with some WPF content.
        /// </summary>
        [Test(0, @"Source\VisualVerification\Simple", TestCaseSecurityLevel.FullTrust, "HwndSourceSimpleVScan", Description = "HwndSource simple vscan test", SupportFiles = @"FeatureTests\ElementServices\HwndSourceSimple.png", Area = "AppModel", Disabled = true)]
        public void Run()
        {
            CoreLogger.BeginVariation();
            using (CoreLogger.AutoStatus("Test has started"))
            {
                // Create HwndSource
                CreateWindow();

                if (VScanHelper.Compare(s_masterImage, _source.Handle))
                {
                    CoreLogger.LogTestResult(true, "Pass");
                }
                else
                {
                    CoreLogger.LogTestResult(false, "Fail, images did not match.");
                }
            }
            CoreLogger.EndVariation();
        }

        /// <summary>
        /// Create HwndSource with some Avalon content.
        /// No changes are made to the Avalon content on or after load.
        /// </summary>
        private void CreateWindow()
        {
            // Create layered HwndSource.
            HwndSourceParameters sourceParams = new HwndSourceParameters("HwndSource", _width, _height);
            sourceParams.PositionX = 10;
            sourceParams.PositionY = 40;

            _source = new HwndSource(sourceParams);

            //
            // Construct layered window contents.
            //

            _rect = new Rectangle();
            _rect.Width = 250;
            _rect.Height = 50;
            _rect.Fill = Brushes.Cyan;

            Canvas.SetTop(_rect, 0);
            Canvas.SetLeft(_rect, 0);

            Canvas c = new Canvas();
            c.Width = _width;
            c.Height = _height;
            c.Children.Add(_rect);

            _source.RootVisual = (Visual)c;

            Microsoft.Test.Threading.DispatcherHelper.DoEvents(1000);
        }

        const int _width = 450;
        const int _height = 450;

        Rectangle _rect = null;
        HwndSource _source = null;
    }

    ///<summary>
    /// VScan based tests for HwndSource.
    ///</summary>
    [TestDefaults]
    public class HwndSourceLayeredVscan
    {
        /// <summary>
        /// Test Layered HwndSource using VScan.
        /// </summary>
        [Test(0, @"Source\VisualVerification\Layered", TestCaseSecurityLevel.FullTrust, "HwndSourceLayeredVScan", Description = "HwndSource layered window vscan test", SupportFiles = @"FeatureTests\ElementServices\HwndSourceLayered.png", Area = "AppModel")]
        public void Run()
        {
            CoreLogger.BeginVariation();
            // Parse params.
            //TestCaseInfo testCaseInfo = TestCaseInfo.GetCurrentInfo();
            //string masterImage = testCaseInfo.Params;
            string masterImage = "HwndSourceLayered.png";

            using (CoreLogger.AutoStatus("Test has started"))
            {
                CreateBackgroundWindow();
                CreateLayeredWindow();

                if (VScanHelper.Compare(masterImage, _source.Handle))
                {
                    CoreLogger.LogTestResult(true, "Pass");
                }
                else
                {
                    CoreLogger.LogTestResult(false, "Fail, images did not match.");
                }
            }
            CoreLogger.EndVariation();
        }

        /// <summary>
        /// Create a layered Window with some Avalon content.
        /// </summary>
        private void CreateLayeredWindow()
        {
            // Create layered HwndSource.
            HwndSourceParameters sourceParams = new HwndSourceParameters("Layered Window", _width, _height);
            sourceParams.PositionX = 10;
            sourceParams.PositionY = 40;

#if TESTBUILD_NET_ATLEAST_46
            sourceParams.UsesPerPixelTransparency = true;
#else
            sourceParams.UsesPerPixelOpacity = true;
#endif

            _source = new HwndSource(sourceParams);

            //
            // Construct layered window contents.
            //

            _button = new Button();
            _button.Template = _MakeButtonTemplate();
            _button.Background = Brushes.Cyan;
            _button.Opacity = 0.5;
            _button.Loaded += OnLoadedChangeOpacities;
            Canvas.SetTop(_button, 0);
            Canvas.SetLeft(_button, 0);

            _setTransparentButton = new Button();
            _setTransparentButton.Template = _MakeButtonTemplate();
            _setTransparentButton.Opacity = 1.0;  // Opacity will be set to 0.5 in Loaded handler.
            _setTransparentButton.Background= Brushes.Magenta;
            Canvas.SetTop(_setTransparentButton, 50);
            Canvas.SetLeft(_setTransparentButton, 0);

            _setOpaqueButton = new Button();
            _setOpaqueButton.Template = _MakeButtonTemplate();
            _setOpaqueButton.Opacity = 0.5; // Opacity will be set to 1.0 in Loaded handler.
            _setOpaqueButton.Background = Brushes.Yellow;
            Canvas.SetTop(_setOpaqueButton, 100);
            Canvas.SetLeft(_setOpaqueButton, 0);

            _animateTransparentButton = new Button();
            _animateTransparentButton.Template = _MakeButtonTemplate();
            _animateTransparentButton.Opacity = 1.0;
            _animateTransparentButton.Background = Brushes.Cyan;
            Canvas.SetTop(_animateTransparentButton, 150);
            Canvas.SetLeft(_animateTransparentButton, 0);

            _animateOpaqueButton = new Button();
            _animateOpaqueButton.Template = _MakeButtonTemplate();
            _animateOpaqueButton.Opacity = 0.25;
            _animateOpaqueButton.Background = Brushes.Magenta;
            Canvas.SetTop(_animateOpaqueButton, 200);
            Canvas.SetLeft(_animateOpaqueButton, 0);

            // Bottom right corner ellipse should be clipped.
            Ellipse corner = new Ellipse();
            corner.Fill = Brushes.Orange;
            corner.Width = 50;
            corner.Height = 50;
            Canvas.SetTop(corner, _width - 25);
            Canvas.SetLeft(corner, _height - 25);

            Canvas c = new Canvas();
            c.Width = _width;
            c.Height = _height;
            c.Children.Add(_button);
            c.Children.Add(_setTransparentButton);
            c.Children.Add(_setOpaqueButton);
            c.Children.Add(_animateTransparentButton);
            c.Children.Add(_animateOpaqueButton);
            c.Children.Add(corner);

            _source.RootVisual = (Visual)c;

            Microsoft.Test.Threading.DispatcherHelper.DoEvents(3000);
        }

        private ControlTemplate _MakeButtonTemplate()
        {
            FrameworkElementFactory canvasFef = new FrameworkElementFactory(typeof(Canvas));

            FrameworkElementFactory rectFef = new FrameworkElementFactory(typeof(Rectangle));
            rectFef.SetValue(Rectangle.WidthProperty, 250.0);
            rectFef.SetValue(Rectangle.HeightProperty, 50.0);
            rectFef.SetValue(Canvas.TopProperty, 0.0);
            rectFef.SetValue(Canvas.LeftProperty, 0.0);
            rectFef.SetValue(Rectangle.FillProperty, new TemplateBindingExtension(Button.BackgroundProperty));

            FrameworkElementFactory textBlockFef = new FrameworkElementFactory(typeof(TextBlock));
            textBlockFef.SetValue(TextBlock.TextProperty, new TemplateBindingExtension(Button.ContentProperty));
            textBlockFef.SetValue(Canvas.TopProperty, 10.0);
            textBlockFef.SetValue(Canvas.LeftProperty, 10.0);

            canvasFef.AppendChild(rectFef);
            canvasFef.AppendChild(textBlockFef);

            ControlTemplate buttonTemplate = new ControlTemplate(typeof(Button));
            buttonTemplate.VisualTree = canvasFef;

            return buttonTemplate;
        }

        /// <summary>
        /// Create a background window so wallpaper does not affect captured image.
        /// </summary>
        private void CreateBackgroundWindow()
        {
            _backgroundWindow = new Window();
            _backgroundWindow.Background = Brushes.Cornsilk;
            _backgroundWindow.Top = 0;
            _backgroundWindow.Left = 0;
            _backgroundWindow.Width = _width + 50;
            _backgroundWindow.Height = _height + 50;
            _backgroundWindow.Show();

        }

        /// <summary>
        /// Change some properties on the Loaded event of the window. This verifies changing
        /// opacity values after the window has been loaded.
        /// </summary>
        private void OnLoadedChangeOpacities(object s, EventArgs e)
        {
            _setTransparentButton.Opacity = 0.5;
            _setOpaqueButton.Opacity = 1.0;

            DoubleAnimation anim = new DoubleAnimation();
            anim.Duration = TimeSpan.FromSeconds(1.5);
            anim.To = 0.25;
            _animateTransparentButton.BeginAnimation(Button.OpacityProperty, anim);

            anim.To = 1.0;
            _animateOpaqueButton.BeginAnimation(Button.OpacityProperty, anim);
        }

        const int _width = 450;
        const int _height = 450;

        HwndSource _source = null;
        Window _backgroundWindow = null;

        Button _button = null;
        Button _setTransparentButton = null;
        Button _setOpaqueButton = null;

        Button _animateTransparentButton = null;
        Button _animateOpaqueButton = null;
    }

 }










