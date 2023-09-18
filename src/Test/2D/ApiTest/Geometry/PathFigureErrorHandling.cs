// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  API Tests - Testing PathFigure Exceptions
//  Author:   Microsoft
//
using System;
using System.Reflection;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;
using Microsoft.Test.Globalization;


namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Summary description for PathFigureErrorHandling class.
    /// </summary>
    internal class PathFigureErrorHandling : ApiTest
    {
        public PathFigureErrorHandling( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            int testCaseID = 1;
            CommonLib.LogStatus("PathFigure  Class");

            //Getting the exception message directly from the ExceptionStringTable in PresentationCore.dll in order
            //to the trouble of prepare all of those localized exception strings running on different locales.
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager("FxResources.PresentationCore.SR", typeof(PathFigure).Assembly);
            #region Test #1: ArgumentNULLException for segmentCollection argument in constructor PathFigure(ICollection segmentCollection)
            try
            {
                CommonLib.LogStatus("Test #1: ArgumentNULLException for segmentCollection argument in constructor PathFigure(ICollection segmentCollection)");

                PathFigure pf1 = new PathFigure(new Point(0, 0), null, false);
            }
            catch (System.ArgumentNullException expectedException)
            {
                string expectedMessage1 = Exceptions.GetExceptionMessage(typeof(ArgumentNullException), "segments");
                if (String.Compare(expectedException.Message, expectedMessage1, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Correct Exception is thrown:  " + expectedException.Message);
                    CommonLib.LogStatus("Case #" + testCaseID + " passes");
                }
                else
                {
                    CommonLib.LogStatus("Incorrect exception being thrown: " + expectedException.Message);
                    CommonLib.LogFail("Test fails at case #" + testCaseID);
                }
            }
            catch (Exception e)
            {
                CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                CommonLib.LogFail("Test fails at case #" + testCaseID);
            }
            #endregion

            CommonLib.LogTest("Test result:");

        }
    }
}