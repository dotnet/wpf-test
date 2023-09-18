// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  API Tests - Testing all Exceptions in geometry converter classes.
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
    internal class GeometryConverterClassErrorHandling : ApiTest
    {
        public GeometryConverterClassErrorHandling( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            int testCaseID = 1;
            string failMessage = "";

            //Getting the exception message directly from the ExceptionStringTable in PresentationCore.dll in order
            //to the trouble of prepare all of those localized exception strings running on different locales.
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager("FxResources.PresentationCore.SR", typeof(PathFigure).Assembly);

            // ----- GeometryConverter.cs section -----
            #region Test #7: ArgumentNullException of value parameter
            CommonLib.LogStatus("Test #7: ArgumentNullException of value parameter");
            testCaseID = 7;
            try
            {
                GeometryConverter pgc7 = new GeometryConverter();
                object returnValue = pgc7.ConvertTo(null, System.Globalization.CultureInfo.CurrentCulture, null, typeof(int));
            }
            catch (System.NotSupportedException expectedException)
            {
                CommonLib.LogStatus("Exception Message:  " + expectedException.Message);
                CommonLib.LogStatus("Case #" + testCaseID + " passes");
            }
            catch (Exception e)
            {
                CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                CommonLib.LogFail( "Test fails at case #" + testCaseID);
                failMessage += "Test fails at case #" + testCaseID + "\n";
            }
            #endregion

            #region Test #8: ArgumentNullException of destinationType parameter
            CommonLib.LogStatus("Test #8: ArgumentNullException of valdestinationTypeue parameter");
            testCaseID = 8;
            try
            {
                GeometryConverter pgc8 = new GeometryConverter();
                object returnValue = pgc8.ConvertTo(null, System.Globalization.CultureInfo.CurrentCulture, 23, null);
            }
            catch (System.ArgumentNullException expectedException)
            {
                string expectedMessage8 = Exceptions.GetExceptionMessage(typeof(ArgumentNullException), "destinationType");

                if (String.Compare(expectedException.Message, expectedMessage8, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Correct Exception is thrown:  " + expectedException.Message);
                    CommonLib.LogStatus("Case #" + testCaseID + " passes");
                }
                else
                {
                    CommonLib.LogStatus("Incorrect exception being thrown: " + expectedException.Message);
                    CommonLib.LogFail( "Test fails at case #" + testCaseID);
                    failMessage += "Test fails at case #" + testCaseID + "\n";
                }
            }
            catch (Exception e)
            {
                CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                CommonLib.LogFail( "Test fails at case #" + testCaseID);
                failMessage += "Test fails at case #" + testCaseID + "\n";
            }
            #endregion

            #region Test #9: ArgumentException for non PathGeometry type object passed in value parameter in ConvertTo
            CommonLib.LogStatus("Test #9: ArgumentException for non PathGeometry type object passed in value parameter in ConvertTo");
            testCaseID = 9;
            try
            {
                GeometryConverter pgc9 = new GeometryConverter();
                object returnValue = pgc9.ConvertTo(null, System.Globalization.CultureInfo.CurrentCulture, "It is not PathFigure", typeof(int));
            }
            catch (System.NotSupportedException expectedException)
            {
                CommonLib.LogStatus("Exception Message:  " + expectedException.Message);
                CommonLib.LogStatus("Case #" + testCaseID + " passes");
            }
            catch (Exception e)
            {
                CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                CommonLib.LogFail( "Test fails at case #" + testCaseID);
                failMessage += "Test fails at case #" + testCaseID + "\n";
            }
            #endregion

            CommonLib.LogTest("For GeometryConverter Error Handling...");
        }
    }
}