//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------


#region Using Statements

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Xml;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;

using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;

using Avalon.Test.ComponentModel.Utilities;
using Avalon.Test.ComponentModel.Validations;
using Avalon.Test.ComponentModel.Utilities.VScanTools;

#endregion

namespace Avalon.Test.ComponentModel.UnitTests
{
    /// <summary>
    /// VScan Unit Test for controls with Popup window
    /// It is created because VScanUnitTest does not support this kind of controls
    /// </summary>
    class PopupVScanUnitTest : IUnitTest
    {
        #region IUnitTest Members

        public TestResult Perform(object testElement, XmlElement variation)
        {
            XmlElement actionsXml = variation["Actions"];
            if (actionsXml != null)
            {
                XtcTestHelper.DoActions(testElement as FrameworkElement, actionsXml);
                QueueHelper.WaitTillQueueItemsProcessed();
            }
            // find Popup visual
            Popup popup = (Popup)VisualTreeUtils.FindPartByType(testElement as Visual, typeof(Popup), 0);
            if (popup == null)
            {
                GlobalLog.LogEvidence("No Popup Found");
                return TestResult.Fail;
            }

            // compute union rect of the visual itself and its popup window
            Rectangle popupRect = ImageUtility.GetScreenBoundingRectangle(popup.Child);
            Rectangle mainRect = ImageUtility.GetScreenBoundingRectangle(testElement as UIElement);
            Rectangle unionRect = Rectangle.Union(popupRect, mainRect);

            CompareImages ci = new CompareImages();
            ci.SetFirstImage(unionRect);

            string MasterFileName = ((XmlElement)variation["VScanInfo"]).GetAttribute("MasterFileName"); ;
            ci.SetSecondImage(MasterFileName);

            bool isTheSame = ci.ImagesEqual(CompareImages.ImageTolerances.Low);

            string testName = ((XmlElement)variation["VScanInfo"]).GetAttribute("TestName");
            ci.SaveAllData(testName);

            if (isTheSame)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        #endregion
    }
}

