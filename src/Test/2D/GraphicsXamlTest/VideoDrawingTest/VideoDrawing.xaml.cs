// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//This is a list of commonly used namespaces for an application class.
using System;
using System.Windows;
using System.Windows.Media;
using System.IO;
using Microsoft.Test.RenderingVerification;

//------------------------------------------------------------------
// Inject SD stamp into the assembly for each file
namespace List.Of.Sources
{
    public class VideoDrawing_xaml_cs
    {
        public static string version = "$Id$ $Change$";
    }
}

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Interaction logic for Application.xaml
    /// </summary>

    public partial class VideoDrawingTest : Window
    {

        public VideoDrawingTest()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This test tests VideoDrawing and DrawVideo in the DrawingContext.DrawVideo
        /// 1.  Create VideoDrawing with a DrawingGroup
        /// 2.  Setup DrawingBrush to fill the Rectangle( rect1 )
        /// 3.  After 10 seconds, the video is supposely finish, then take a snapshot
        ///     to verify if the video is indeed rendered and played
        /// 4.  Remove the DrawingBrush from Rectangle.Fill
        /// 5.  Prepare a new DrawingGroup and using DrawView to create the video stream in DrawingGroup
        /// 6.  Create DrawingBrush with the DrawingGroup and fill the Rectangle with this DrawBrush
        /// 7.  After 10 seconds, take a snapshot of the surface, and verify if the video played.
        /// </summary>
        public void RunTest(object sender, System.EventArgs e)
        {
            XamlTestHelper.LogStatus("ContentRendered: Start the test");
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot, "Master.bmp");
            XamlTestHelper.AddStep(CreateVideoDrawing, null, 10000);            
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot);
            XamlTestHelper.AddStep(Verify);
            XamlTestHelper.AddStep(ResetRectangle);
            XamlTestHelper.AddStep(PlayVideo, null, 10000);
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot);
            XamlTestHelper.AddStep(Verify);
            XamlTestHelper.AddStep(XamlTestHelper.Quit);

            // Starting running the test steps
            XamlTestHelper.Run();
        }

        public object CreateVideoDrawing(object arg)
        {
            XamlTestHelper.LogStatus("*** Create VideoDrawing and fill the Rectangle with VideoDrawing in the DrawingBrush ");
            string videoPath = System.Environment.CurrentDirectory + "/Ball.wmv";
            MediaTimeline mt = new MediaTimeline(new Uri(videoPath));
            MediaClock mc = mt.CreateClock();
            MediaPlayer mp = new MediaPlayer();
            mp.Clock = mc;
            DrawingGroup dg = new DrawingGroup();
            VideoDrawing vd = new VideoDrawing();
            vd.Player = mp;
            vd.Rect = new Rect(0, 0, 200, 200);
            dg.Children.Add(vd);
            DrawingBrush db = new DrawingBrush();
            db.Drawing = dg;

            rect1.Fill = db;
            return null;
        }

        public object ResetRectangle(object arg)
        {
            XamlTestHelper.LogStatus("Resetting Rectangle");
            rect1.Fill = null;
            return null;
        }

        public object PlayVideo(object arg)
        {
            XamlTestHelper.LogStatus("*** PlayVideo in DrawingGroup ");
            string videoPath = System.Environment.CurrentDirectory + "/Ball.wmv";
            MediaTimeline mt = new MediaTimeline(new Uri(videoPath));
            MediaClock mc = mt.CreateClock();
            MediaPlayer mp = new MediaPlayer();
            mp.Clock = mc;
            DrawingGroup dg = new DrawingGroup();
            using (DrawingContext ctx = dg.Open())
            {
                ctx.DrawVideo(mp, new Rect(0, 0, 200, 200));
            }

            XamlTestHelper.LogStatus("*** Create DrawingBrush with the DrawingGroup");
            DrawingBrush db = new DrawingBrush();
            db.Drawing = dg;

            XamlTestHelper.LogStatus("*** Fill the Brush with the DrawingBrush");
            rect1.Fill = db;

            return null;
        }

        public object Verify(object arg)
        {
            XamlTestHelper.LogStatus("Taking Video snapshot");
            XamlTestHelper.LogStatus("Compare to the master bitmap");
            // The master and the capture should not be the same
            // The master is just a blank white, and the capture should have contained the video
            // The second parameter is for expectFailure.  true means we expect the capture is not the same
            //  as the master
            if (!XamlTestHelper.Compare("Master.bmp", true))
            {
                XamlTestHelper.LogFail(" The video didn't start correctly");
                XamlTestHelper.Quit(null);
            }
            XamlTestHelper.LogStatus("Cool, the Video starts and plays correctly!");
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