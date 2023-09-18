// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  API Tests - Testing PathSegment Exceptions
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
    internal class PathSegmentErrorHandling : ApiTest
    {
        public PathSegmentErrorHandling( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            double testCaseID = 1.0;
            string failMessage = "";

            //Getting the exception message directly from the ExceptionStringTable in PresentationCore.dll in order
            //to the trouble of prepare all of those localized exception strings running on different locales.
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager("FxResources.PresentationCore.SR", typeof(PathSegment).Assembly);

            #region Exception in PolyBezierSegment
            #region Test 1:  ArgumentNULLException when passing NULL points collection in PolyBezierSegment's constructor
            CommonLib.LogStatus("Test 1:  ArgumentNULLException when passing NULL points collection in PolyBezierSegment's constructor");
            try
            {
                PolyBezierSegment pbs1 = new PolyBezierSegment(null, true);
            }
            catch (System.ArgumentNullException expectedException)
            {
                string expectedMessage = Exceptions.GetExceptionMessage(typeof(ArgumentNullException), "points");

                if (String.Compare(expectedException.Message, expectedMessage, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
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

            #endregion

            #region Exceptions in PolyLineSegment
            #region Test 4:  ArgumentNULLException when passing NULL points collection in PolyLineSegment's constructor
            CommonLib.LogStatus("Test 4:  ArgumentNULLException when passing NULL points collection in PolyLineSegment's constructor");
            testCaseID = 4;
            try
            {
                PolyLineSegment pls4 = new PolyLineSegment(null, true);
            }
            catch (System.ArgumentNullException expectedException)
            {
                string expectedMessage = Exceptions.GetExceptionMessage(typeof(ArgumentNullException), "points");

                if (String.Compare(expectedException.Message, expectedMessage, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
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

            #endregion

            #region Exceptions in PolyQuadraticBezierSegment
            #region Test 7:  ArgumentNULLException when passing NULL points collection in PolyQuadraticBezierSegment's constructor
            CommonLib.LogStatus("Test 7:  ArgumentNULLException when passing NULL points collection in PolyQuadraticBezierSegment's constructor");
            testCaseID = 7;
            try
            {
                PolyQuadraticBezierSegment pqs7 = new PolyQuadraticBezierSegment(null, true);
            }
            catch (System.ArgumentNullException expectedException)
            {
                string expectedMessage = Exceptions.GetExceptionMessage(typeof(ArgumentNullException), "points");
                
                if (String.Compare(expectedException.Message, expectedMessage, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
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

            #endregion

            CommonLib.LogTest("Test result:");
        }
    }
}