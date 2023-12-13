// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  API Tests - Testing EllipseGeometry Exceptions
//  Author:   Microsoft
//
using System;
using System.Reflection;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class EllipseGeometryErrorHandling : ApiTest
    {
        public EllipseGeometryErrorHandling( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            int testCaseID = 1;

            //Getting the exception message directly from the ExceptionStringTable in PresentationCore.dll in order
            //to the trouble of prepare all of those localized exception strings running on different locales.
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager("FxResources.PresentationCore.SR", typeof(PathFigure).Assembly);

            #region Test #1:  ArgumentException for Rect.Empty pass into constructor EllipseGeometry(Rect rect)
            CommonLib.LogStatus("Test #1:  ArgumentException for Rect.Empty pass into constructor EllipseGeometry(Rect rect)");
            try
            {
                EllipseGeometry pg3 = new EllipseGeometry(Rect.Empty);
            }
            catch (System.ArgumentException expectedException)
            {
                //Getting the exception message directly from the ExceptionStringTable in PresentationCore.dll in order
                //to the trouble of prepare all of those localized exception strings running on different locales.
                string message = rm.GetString("Rect_Empty");
                message = string.Format(message, "rect");
                if (String.Compare(expectedException.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Correct Exception is thrown:  " + expectedException.Message);
                    CommonLib.LogStatus("Case #" + testCaseID + " passes");
                    testCaseID++;
                }
                else
                {
                    CommonLib.LogStatus("Incorrect exception being thrown: " + expectedException.Message);
                    CommonLib.LogFail("Test fails at case #" + testCaseID);
                    return;
                }
            }
            catch (Exception e)
            {
                CommonLib.LogStatus("Incorrect exception being thrown: " + e);
                CommonLib.LogFail("Test fails at case #" + testCaseID);
                return;
            }
            #endregion

            CommonLib.LogTest("Test Result:");
        }
    }
}