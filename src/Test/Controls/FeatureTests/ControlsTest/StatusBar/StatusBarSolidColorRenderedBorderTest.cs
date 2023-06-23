//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Xml;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using Microsoft.Test.Logging;

using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.RenderingVerification;
using System.Drawing;

using System.Windows.Media;


namespace Avalon.Test.ComponentModel.UnitTests
{
    /// <summary>
    /// this class is used to test StatusBar's BorderBrush and BorderThickness property
    /// before use it, you should create .xtc file for it using GTO
    /// Run example: CMLoader.exe StatusBarBorder000001.xtc
    /// </summary>
    [TargetType(typeof(StatusBar))]
    class StatusBarSolidColorRenderedBorderTest : IUnitTest
    {
        public const int TOLERANCE = 3;
        /// <summary>
        /// do test code
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="variation"></param>
        /// <returns></returns>
        public TestResult Perform(object obj, XmlElement variation)
        {
            GlobalLog.LogStatus("StatusBar Border(BorderBrush and BorderThickness) test starting... ");
            StatusBar statusbar;
            statusbar = (StatusBar)obj;
         
            //capture StatusBar's render image
            Rectangle controlRect = ImageUtility.GetScreenBoundingRectangle(statusbar);
            System.Drawing.Point controlOffset = new System.Drawing.Point((int)controlRect.X, (int)controlRect.Y);
            System.Drawing.Rectangle boundingBox = new System.Drawing.Rectangle(controlOffset, new System.Drawing.Size((int)statusbar.RenderSize.Width, (int)statusbar.RenderSize.Height));
            Bitmap bmp = ImageUtility.CaptureScreen(boundingBox);
            
            //get statusbar's backcolor
            System.Windows.Media.Color col;
            System.Windows.Media.SolidColorBrush brush = statusbar.BorderBrush as System.Windows.Media.SolidColorBrush;
            if (brush == null)
            {
                if (statusbar.BorderBrush == null)
                {
                    GlobalLog.LogEvidence("StatusBar's BorderBrush is null, so nothing needed to test");
                }
                else
                {
                    GlobalLog.LogEvidence("The Brush type is" + statusbar.BorderBrush.GetType().ToString());
                    GlobalLog.LogEvidence("Since this test case is used to test solid color border, so this test need a SolidColorBrush");
                }
                GlobalLog.LogStatus("The BorderBrush is now expected one (SolidColorBrush is the exact one)");
                return TestResult.Unknown;
            }
            GlobalLog.LogStatus("BorderBrush Color:" + brush.Color.ToString());
            col = brush.Color;

            //compare color if it was expected.
            GlobalLog.LogStatus("BorderThickness:" + statusbar.BorderThickness.Left);
            for (int i = 0; i < statusbar.BorderThickness.Left; i++)
            {
                if (!IsEqual(bmp.GetPixel(i, (int)(statusbar.RenderSize.Height / 2)), col))
                {
                    GlobalLog.LogStatus("The rendered pixel color dismatch the expected color");
                    return TestResult.Fail;
                }
            }
    
            return TestResult.Pass;
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
