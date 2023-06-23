//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Xml;
using System.Drawing;
using System.Windows;
using System.Windows.Media;

using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using Avalon.Test.ComponentModel.UnitTests;
using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;


namespace Avalon.Test.ComponentModel.UnitTests
{
    /// <summary>
    /// This class is used to test Background property for StatusBar
    /// before use it, you should create .xtc file for it using scrgen.mdb
    /// run example: CMLoader.exe StatussBarBkground000001.xtc
    /// </summary>
    [TargetType(typeof(StatusBar))]
    class StatusBarBkGroundColor : IUnitTest
    {
        public const int TOLERANCE = 3;
        /// <summary>
        /// do test job
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="variation"></param>
        /// <returns></returns>
        public TestResult Perform(object obj, XmlElement variation)
        {
            GlobalLog.LogStatus("StatusBar Background color test starting... ");
            
            StatusBar statusbar;
            statusbar = (StatusBar)obj;

            //capture StatusBar's render image
            Rectangle controlRect = ImageUtility.GetScreenBoundingRectangle(statusbar);
            GlobalLog.LogStatus("controlRect : " + controlRect);

            System.Drawing.Point controlOffset = new System.Drawing.Point((int)controlRect.X, (int)controlRect.Y);
            GlobalLog.LogStatus("controlOffset : " + controlOffset);

            System.Drawing.Rectangle boundingBox = new System.Drawing.Rectangle(controlOffset, new System.Drawing.Size((int)statusbar.RenderSize.Width, (int)statusbar.RenderSize.Height));
            GlobalLog.LogStatus("boundingBox : " + boundingBox);

            Bitmap bmp = ImageUtility.CaptureScreen(boundingBox);

            System.Windows.Media.SolidColorBrush brush =  statusbar.Background as System.Windows.Media.SolidColorBrush;
            GlobalLog.LogStatus("Background Color:" + brush.Color.ToString());

            System.Windows.Media.Color col;
            col = brush.Color;

            System.Drawing.Color actual = bmp.GetPixel((int)(statusbar.RenderSize.Width / 2), (int)(statusbar.RenderSize.Height / 2));

            

            //compare StatusBar Render Image's Center Pixel to Background color
            if (actual.R == col.R && actual.G == col.G && actual.B == col.B)
            {
                return TestResult.Pass;
            }

            GlobalLog.LogStatus("Fail:the background color is not expected one");
            return TestResult.Fail; 
        }


        /// <summary>
        /// test two kinds of color(System.Drawing.Color and System.Windows.Media.Color)
        /// if they stand for the same color
        /// </summary>
        /// <param name="col1"></param>
        /// <param name="col2"></param>
        /// <returns></returns>
        private bool IsEqual(System.Drawing.Color col1, System.Windows.Media.Color col2)
        {
            GlobalLog.LogStatus("Col1:" + col1.ToString());
            GlobalLog.LogStatus("Col2:" + col2.ToString());

            //test Red, Green, Blue component value, not include alpha
            if (Math.Abs(col1.R - col2.R) < TOLERANCE && Math.Abs(col1.G - col2.G) < TOLERANCE && Math.Abs(col1.B - col2.B) < TOLERANCE)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
