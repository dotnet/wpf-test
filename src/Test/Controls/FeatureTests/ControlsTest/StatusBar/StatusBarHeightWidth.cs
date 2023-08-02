//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Xml;
using System.Drawing;
using System.Windows.Controls.Primitives;

using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;

namespace Avalon.Test.ComponentModel.UnitTests
{
    /// <summary>
    /// This class is used to test Height/Width property for statusBar
    /// Before using it, you should create .xtc file using GTO first
    /// Run Example: CMLoader.exe StatusBarWidthHeight000001.xtc
    /// </summary>
    [TargetType(typeof(StatusBar))]
    class StatusBarHeightWidth : IUnitTest
    {
        /// <summary>
        /// do test job
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="variation"></param>
        /// <returns></returns>
        public TestResult Perform(object obj, XmlElement variation)
        {
            GlobalLog.LogStatus("StatusBar Width/Height property test starting... ");
            StatusBar statusbar;
            statusbar = (StatusBar)obj;

            //capture statusbar' render image
            Rectangle controlRect = ImageUtility.GetScreenBoundingRectangle(statusbar);
            System.Drawing.Point controlOffset = new System.Drawing.Point((int)controlRect.X, (int)controlRect.Y);
            System.Drawing.Rectangle boundingBox = new System.Drawing.Rectangle(controlOffset, new System.Drawing.Size((int)statusbar.RenderSize.Width, (int)statusbar.RenderSize.Height));
            Bitmap bmp = ImageUtility.CaptureScreen(boundingBox);

            //validate if render image size was equal the Width/Height property value 
            if (bmp.Width == statusbar.Width && bmp.Height == statusbar.Height)
            {
                return TestResult.Pass;
            }

            GlobalLog.LogStatus("rendered-size of statusbar dismatch the expected one");
            return TestResult.Fail;
        }
    }
}
