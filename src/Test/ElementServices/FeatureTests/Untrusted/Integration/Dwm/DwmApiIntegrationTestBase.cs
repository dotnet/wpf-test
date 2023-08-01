// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using Microsoft.Win32;


using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Threading;

using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
using Microsoft.Test.Win32;


namespace Avalon.Test.CoreUI.Dwm
{
    ///<summary>
    ///</summary>
    ///<remarks>
    ///</remarks>
    public class DwmApiIntegrationTestBase : TestCase, IHostedTest
    {
        /// <summary>
        /// 
        /// </summary>
        protected ITestContainer _testContainer = null;

        /// <summary>
        /// 
        /// </summary>
        public DwmApiIntegrationTestBase()
        {
            helper = new DwmAPIHelper();
        }

        /// <summary>
        /// Represents the current ITestContainer for this IHostedTest.
        /// </summary>
        public ITestContainer TestContainer
        {
            get
            {
                return _testContainer;
            }
            set
            {
                _testContainer = value;
            }
        }

        /// <summary>
        /// To workaround opacity issue with window/apply blur
        /// </summary>
        /// <param name="handle"></param>
        protected void DoMinimizeRestore(IntPtr handle)
        {
            CoreLogger.LogStatus("DoMinimizeRestore");

            NativeMethods.ShowWindow(new HandleRef(null, handle), NativeConstants.SW_MINIMIZE);
            DispatcherHelper.DoEvents(100);
            NativeMethods.ShowWindow(new HandleRef(null, handle), NativeConstants.SW_RESTORE);
        }

         /// <summary>
         /// Register result
         /// </summary>
         /// <param name="result"></param>
        protected void RegisterResult(bool result)
        {
            if (!result)
            {
                CoreLogger.LogTestResult(false, "test failed");
            }
            else
            {
                CoreLogger.LogTestResult(true, "test passed");
            }

            CoreLogger.LogStatus("End Test");

            surface.Close();
            TestContainer.EndTest();

        }

        /// <summary>
        /// Avlon tree for test
        /// </summary>
        /// <returns></returns>
        protected Panel GetAvalonTree()
        {
            CoreLogger.LogStatus("Create and return avalon tree used for this test");

            panel = new StackPanel();

            button = new Button();
            button.Width = 100;
            button.Height = 100;
            button.Margin = new Thickness(20);
            button.Content = "Test";
            button.Background = Brushes.Red;

            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem1 = new MenuItem();
            menuItem1.Header = "Item0";
            MenuItem menuItem2 = new MenuItem();
            menuItem2.Header = "Item1";

            contextMenu.Items.Add(menuItem1);
            contextMenu.Items.Add(menuItem2);

            button.ContextMenu = contextMenu;

            button.ContextMenuOpening += new ContextMenuEventHandler(button_ContextMenuEvent);
            panel.Children.Add(button);


            rectangle = new Rectangle();
            rectangle.Margin = new Thickness(20);
            rectangle.Width = 100;
            rectangle.Height = 100;
            rectangle.Fill = Brushes.Yellow;

            panel.Children.Add(rectangle);


            return panel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void button_ContextMenuEvent(object sender, ContextMenuEventArgs args)
        {
            CoreLogger.LogStatus("button_ContextMenuEvent");
            contextMenuEvent++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="arrList"></param>
        protected void CaptureUIElement(UIElement element, ArrayList arrList)
        {
            CoreLogger.LogStatus("CaptureUIElement :" + element.ToString());

            ImageAdapter img = null;

            System.Drawing.Rectangle rect = System.Drawing.Rectangle.Empty;

            if (element != null)
            {
                rect = ImageUtility.GetScreenBoundingRectangle(element);
            }
            else
            {
                CoreLogger.LogStatus("element null");
            }

            if (rect != System.Drawing.Rectangle.Empty)
            {
                System.Drawing.Bitmap bmp = ImageUtility.CaptureScreen(rect);
                img = new ImageAdapter(bmp);
            }
            else
            {
                CoreLogger.LogStatus("System.Drawing.Rectangle.Empty");
            }

            arrList.Add(img);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool VerifyElementsAreSame()
        {
            bool bResult = true;

            for (int i = 0; i < elmImgBeforeGlassEffectApplied.Count; i++)
            {
                if (elmImgAfterGlassEffectApplied[i] == null)
                {
                    CoreLogger.LogStatus("Image missing for one element taken after glass effect applied");
                    bResult = false;
                    break;
                }

                if (elmImgBeforeGlassEffectApplied[i] == null)
                {
                    CoreLogger.LogStatus("Image missing for one element taken before glass effect applied");
                    bResult = false;
                    break;
                }

                bool imagesEqual = VScanHelper.Compare(elmImgBeforeGlassEffectApplied[i] as IImageAdapter, elmImgAfterGlassEffectApplied[i] as IImageAdapter, VScanProfile.Profile.Poor);

                if (!imagesEqual)
                {
                    bResult = false;
                    CoreLogger.LogStatus("FAIL-Images are different");
                }
                else
                {
                    CoreLogger.LogStatus("PASS-Images are same");
                }
            }

            return bResult;
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool dwmAPISuccess;
        /// <summary>
        /// 
        /// </summary>
        protected Rectangle rectangle;
        /// <summary>
        /// 
        /// </summary>
        protected Button button;
        /// <summary>
        /// 
        /// </summary>
        protected string testName = null;
        /// <summary>
        /// 
        /// </summary>
        protected Surface surface = null;
        /// <summary>
        /// 
        /// </summary>
        protected StackPanel panel;
        /// <summary>
        /// 
        /// </summary>
        protected ArrayList expectedDWMMessages = new ArrayList();
        /// <summary>
        /// 
        /// </summary>
        protected ArrayList elmImgBeforeGlassEffectApplied = new ArrayList();
        /// <summary>
        /// 
        /// </summary>
        protected ArrayList elmImgAfterGlassEffectApplied = new ArrayList();
        /// <summary>
        /// 
        /// </summary>
        protected DwmAPIHelper helper;
        /// <summary>
        /// 
        /// </summary>
        protected int contextMenuEvent;
    }
}

