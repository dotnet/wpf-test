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
    internal class PathGeometryErrorHandling : ApiTest
    {
        public PathGeometryErrorHandling( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            int testCaseID = 1;
            string failMessage = "";

            CommonLib.LogStatus("PathGeometry  Class");

            //Getting the exception message directly from the ExceptionStringTable in PresentationCore.dll in order
            //to the trouble of prepare all of those localized exception strings running on different locales.
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager("FxResources.PresentationCore.SR", typeof(PathFigure).Assembly);

            #region Test #2: ArgumentNullException for figureCollection para in SetPathGeometry()
            CommonLib.LogStatus("Test #2: ArgumentNullException for figureCollection para in SetPathGeometry()");
            testCaseID = 2;
            try
            {
                PathGeometry pg2 = new PathGeometry(null, FillRule.Nonzero, new RotateTransform(23.1, 0, 0));
            }
            catch (System.ArgumentNullException expectedException)
            {
                string expectedMessage2 = Exceptions.GetExceptionMessage(typeof(ArgumentNullException), "figures");
                
                if (String.Compare(expectedException.Message, expectedMessage2, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Correct Exception is thrown:  " + expectedException.Message);
                    CommonLib.LogStatus("Case #" + testCaseID + " passes");
                }
                else
                {
                    CommonLib.LogStatus("Incorrect exception being thrown: " + expectedException.Message);
                    CommonLib.LogFail("Test fails at case #" + testCaseID);
                    failMessage += "Test fails at case #" + testCaseID + "\n";
                }
            }
            catch (Exception e)
            {
                CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                CommonLib.LogFail("Test fails at case #" + testCaseID);
                failMessage += "Test fails at case #" + testCaseID + "\n";
            }
            #endregion

            #region Test #4: ArgumentNullException for figureCollection para in SetPathGeometry()
            CommonLib.LogStatus("Test #4: ArgumentNullException for geometry para in AddGeometry()");
            testCaseID = 4;
            try
            {
                PathGeometry pg4 = new PathGeometry();
                pg4.AddGeometry(null);
            }
            catch (System.ArgumentNullException expectedException)
            {
                string expectedMessage4 = Exceptions.GetExceptionMessage(typeof(ArgumentNullException), "geometry");
                
                if (String.Compare(expectedException.Message, expectedMessage4, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Correct Exception is thrown:  " + expectedException.Message);
                    CommonLib.LogStatus("Case #" + testCaseID + " passes");
                }
                else
                {
                    CommonLib.LogStatus("Incorrect exception being thrown: " + expectedException.Message);
                    CommonLib.LogFail("Test fails at case #" + testCaseID);
                    failMessage += "Test fails at case #" + testCaseID + "\n";
                }
            }
            catch (Exception e)
            {
                CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                CommonLib.LogFail("Test fails at case #" + testCaseID);
                failMessage += "Test fails at case #" + testCaseID + "\n";
            }
            #endregion

            #region Test #13: ArgumentNullException for pen param in GetWidenedPathGeometry(Pen pen)
            CommonLib.LogStatus("Test #13: ArgumentNullException for pen param in GetWidenedPathGeometry(Pen pen)");
            testCaseID = 13;
            try
            {
                PathGeometry pg13 = new PathGeometry();
                pg13.GetWidenedPathGeometry(null);
            }
            catch (System.ArgumentNullException expectedException)
            {
                string expectedMessage13 = Exceptions.GetExceptionMessage(typeof(ArgumentNullException), "pen");

                if (String.Compare(expectedException.Message, expectedMessage13, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Correct Exception is thrown:  " + expectedException.Message);
                    CommonLib.LogStatus("Case #" + testCaseID + " passes");
                }
                else
                {
                    CommonLib.LogStatus("Incorrect exception being thrown: " + expectedException.Message);
                    CommonLib.LogFail("Test fails at case #" + testCaseID);
                    failMessage += "Test fails at case #" + testCaseID + "\n";
                }
            }
            catch (Exception e)
            {
                CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                CommonLib.LogFail("Test fails at case #" + testCaseID);
                failMessage += "Test fails at case #" + testCaseID + "\n";
            }
            #endregion

            //Regression case for Regression_Bug59
            #region Test #14: InvalidEnumArgumentException when passing in incorrect FillRule enum value
            CommonLib.LogStatus("Test #14: InvalidEnumArgumentException when passing in incorrect FillRule enum value");
            testCaseID = 14;
            try
            {
                PathFigureCollection pfc14 = new PathFigureCollection();
                PathGeometry pg14 = new PathGeometry(pfc14, (FillRule)99, null);
            }
            catch (System.ComponentModel.InvalidEnumArgumentException expectedException)
            {
                //Getting the exception message directly from the ExceptionStringTable in PresentationCore.dll in order
                //to the trouble of prepare all of those localized exception strings running on different locales.
                string message = rm.GetString("Enum_Invalid");

                message = string.Format(message, "FillRule");
                if (String.Compare(expectedException.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Correct Exception is thrown:  " + expectedException.Message);
                    CommonLib.LogStatus("Case #" + testCaseID + " passes");
                }
                else
                {
                    CommonLib.LogStatus("Incorrect exception being thrown: " + expectedException.Message);
                    CommonLib.LogFail("Test fails at case #" + testCaseID);
                    failMessage += "Test fails at case #" + testCaseID + "\n";
                }
            }
            catch (Exception e)
            {
                CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                CommonLib.LogFail("Test fails at case #" + testCaseID);
                failMessage += "Test fails at case #" + testCaseID + "\n";
            }
            #endregion

            CommonLib.LogTest("Test case result:");
        }
    }
}
