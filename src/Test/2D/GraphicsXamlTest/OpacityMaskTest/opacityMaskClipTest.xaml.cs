// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//This is a list of commonly used namespaces for an application class.
using System;
using System.Windows;
using Microsoft.Test.RenderingVerification;

//------------------------------------------------------------------
// Inject SD stamp into the assembly for each file
namespace List.Of.Sources
{
    public class OpacityMaskClipTest_xaml_cs
    {
        public static string version = "$Id: opacityMaskClipTest.xaml.cs#2 $Change: 52370 $";
    }
}

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Interaction logic for Application.xaml
    /// </summary>

    public partial class OpacityMaskClipTest : Window
    {

        public OpacityMaskClipTest()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This test takes the snapshot of the window rendered by the xaml and 
        /// 1. Clips the Height and compare the snapshot with clipped base snapshot
        /// 2. Clips the Width and compare the snapshot with clipped base snapshot
        /// 3. Increases the Height and compare the snapshot with clipped base snapshot
        /// This is a regression test for 
        /// Regression_Bug242 OpacityMask renders incorrectly or completely hides element
        /// </summary>
        public void RunTest(object sender, System.EventArgs e)
        {
            
            XamlTestHelper.LogStatus("ContentRendered: Start the test");
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot, "OpacityMaskClipTest.bmp");
            XamlTestHelper.AddStep(ClipHeightTest);
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot);
            XamlTestHelper.AddStep(VerifyClipHeight); 
            XamlTestHelper.AddStep(ClipWidthTest);
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot);
            XamlTestHelper.AddStep(VerifyClipWidth);
            XamlTestHelper.AddStep(IncreaseHeightTest);
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot);
            XamlTestHelper.AddStep(VerifyIncreaseHeight);
            XamlTestHelper.AddStep(XamlTestHelper.Quit);

            XamlTestHelper.Run();
        }

        public object ClipHeightTest(object arg)
        {
            this.Height = this.Height / 2;
            return null;
        }

        public object VerifyClipHeight(object arg)
        {
            System.Drawing.Bitmap clipHeightBMP = new System.Drawing.Bitmap("OpacityMaskClipTest.bmp");
            clipHeightBMP = ImageUtility.ClipBitmap(clipHeightBMP, new System.Drawing.Rectangle(0, 0, (int)XamlTestHelper.captureBMP.Width, (int)XamlTestHelper.captureBMP.Height));
            clipHeightBMP.Save("clippedImage.bmp");
            if (XamlTestHelper.Compare("clippedImage.bmp"))
            {
                XamlTestHelper.LogStatus( "Yeah, the rendering is fine on clipping height");
            }
            else
            {
                XamlTestHelper.LogFail( "Incorrect rendering on clipping height");
            }
            return null;

        }
        public object ClipWidthTest(object arg)
        {
            this.Width = this.Width / 2;
            return null;
        }

        public object VerifyClipWidth(object arg)
        {
            System.Drawing.Bitmap clipWidthBMP = new System.Drawing.Bitmap("OpacityMaskClipTest.bmp");
            clipWidthBMP = ImageUtility.ClipBitmap(clipWidthBMP, new System.Drawing.Rectangle(0, 0, (int)XamlTestHelper.captureBMP.Width, (int)XamlTestHelper.captureBMP.Height));
            clipWidthBMP.Save("clippedImage.bmp");
            if (XamlTestHelper.Compare("clippedImage.bmp"))
            {
                XamlTestHelper.LogStatus( "Yeah, the rendering is fine on clipping width");
            }
            else
            {
                XamlTestHelper.LogFail( "Incorrect rendering on clipping width");
            }
            return null;
        }

        public object IncreaseHeightTest(object arg)
        {
            this.Height = this.Height * 2;
            return null;
        }

        public object VerifyIncreaseHeight(object arg)
        {
            System.Drawing.Bitmap clipHeightBMP = new System.Drawing.Bitmap("OpacityMaskClipTest.bmp");
            clipHeightBMP = ImageUtility.ClipBitmap(clipHeightBMP, new System.Drawing.Rectangle(0, 0, (int)XamlTestHelper.captureBMP.Width, (int)XamlTestHelper.captureBMP.Height));
            clipHeightBMP.Save("clippedImage.bmp");
            if (XamlTestHelper.Compare("clippedImage.bmp"))
            {
                XamlTestHelper.LogStatus( "Yeah, the rendering is fine on increasing height");
            }
            else
            {
                XamlTestHelper.LogFail( "Incorrect rendering on increasing height");
            }
            return null;
        }
    }
}