// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  API Tests - Testing PathFigureCollection Exceptions
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
    internal class PathFigureCollectionErrorHandling : ApiTest
    {
        public PathFigureCollectionErrorHandling( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }


        protected override void OnRender(DrawingContext DC)
        {
            //Getting the exception message directly from the ExceptionStringTable in PresentationCore.dll in order
            //to the trouble of prepare all of those localized exception strings running on different locales.
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager("FxResources.PresentationCore.SR", typeof(PathFigure).Assembly);
            int testCaseID = 0;
            string failMessage = "";

            #region Test #4: ArgumentNullException when the array is null in CopyTo(Array array, int index)
            CommonLib.LogStatus("Test #4: ArgumentNullException when the array is null in CopyTo(Array array, int index) ");
            testCaseID = 4;
            try
            {
                PathFigureCollection pfc4 = new PathFigureCollection();
                pfc4.Add(new PathFigure());

                pfc4.CopyTo(null, 0);
            }
            catch (System.ArgumentNullException expectedException)
            {
                string expectedMessage4 = Exceptions.GetExceptionMessage(typeof(ArgumentNullException), "points");
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

            #region Test #5: ArgumentException when the passed array is not one dimentional in PathFigureCollection(params PathFigure[] collection)
            CommonLib.LogStatus("Test #5: ArgumentException when the passed array is not one dimentional in PathFigureCollection(params PathFigure[] collection)");
            testCaseID = 5;
            try
            {
                int[,] testarray = new int[2, 2];

                PathFigureCollection pfc5 = new PathFigureCollection();
                pfc5.Add(new PathFigure());

                //We need to cast to ICollection since the other CopyTo is now strongly typed
                ((ICollection)pfc5).CopyTo(testarray, 0);
            }
            catch (System.ArgumentException expectedException)
            {
                //Getting the exception message directly from the ExceptionStringTable in PresentationCore.dll in order
                //to the trouble of prepare all of those localized exception strings running on different locales.
                string message = rm.GetString("Collection_BadRank");

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

            #region Test #6: ArgumentOutOfRangeException when the passed in array is not long enough in PathFigureCollection(params PathFigure[] collection)
            CommonLib.LogStatus("Test #6: ArgumentOutOfRangeException when the passed in array is not long enough in PathFigureCollection(params PathFigure[] collection)");
            testCaseID = 6;
            try
            {
                int[] testArray = new int[1];
                PathFigureCollection pfc6 = new PathFigureCollection();
                pfc6.Add(new PathFigure());
                pfc6.Add(new PathFigure());

                //We need to cast to ICollection since the other CopyTo is now strongly typed
                ((ICollection)pfc6).CopyTo(testArray, 0);
            }
            catch (System.ArgumentException expectedException)
            {
                //Getting the exception message directly from the ExceptionStringTable in PresentationCore.dll in order
                //to the trouble of prepare all of those localized exception strings running on different locales.
                string message = "Specified argument was out of the range of valid values.\r\nParameter name: index";

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

            #region Test #7: ArgumentException when adding bad type object in IList.Add(object value)
            CommonLib.LogStatus("Test #7: ArgumentException when adding bad type object in IList.Add(object value)");
            testCaseID = 7;
            try
            {
                PathFigureCollection pfc7 = new PathFigureCollection();
                pfc7.Add(new PathFigure());
                pfc7.Add(new PathFigure());

                ((IList)pfc7).Add(2323);
            }
            catch (System.ArgumentException expectedException)
            {
                //Getting the exception message directly from the ExceptionStringTable in PresentationCore.dll in order
                //to the trouble of prepare all of those localized exception strings running on different locales.
                string message = rm.GetString("Collection_BadType");
                message = string.Format(message, "PathFigureCollection", "Int32", "PathFigure");
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

            #region Test #8: ArgumentException with bad type when trying  to insert a Null value in IList.Insert(int index, object value)
            CommonLib.LogStatus("Test #8: ArgumentException with bad type when trying  to insert a Null value in IList.Insert(int index, object value)");
            testCaseID = 8;
            try
            {
                PathFigureCollection pfc8 = new PathFigureCollection();
                pfc8.Add(new PathFigure());
                pfc8.Add(new PathFigure());

                ((IList)pfc8).Insert(0, null);
            }
            catch (System.ArgumentNullException expectedException)
            {
                ArgumentNullException ane = new ArgumentNullException("value");

                if (String.Compare(expectedException.Message, ane.Message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
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

            #region Test #9: ArgumentException when adding bad type object in IList.Add(object value)
            CommonLib.LogStatus("Test #9: ArgumentException when adding bad type object in IList.Add(object value)");
            testCaseID = 9;
            try
            {
                PathFigureCollection pfc9 = new PathFigureCollection();
                pfc9.Add(new PathFigure());
                pfc9.Add(new PathFigure());

                ((IList)pfc9).Insert(1, 23232);
            }
            catch (System.ArgumentException expectedException)
            {
                //Getting the exception message directly from the ExceptionStringTable in PresentationCore.dll in order
                //to the trouble of prepare all of those localized exception strings running on different locales.
                string message = rm.GetString("Collection_BadType");

                message = string.Format(message, "PathFigureCollection", "Int32", "PathFigure");
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

            #region Test #10: ArgumentOutOfRangeException when the index is out of range of the collection in Insert(int index, Object value)
            CommonLib.LogStatus("Test #10: ArgumentOutOfRangeException when the index is out of range of the collection in Insert(int index, Object value)");
            testCaseID = 10;
            try
            {
                PathFigureCollection pfc10 = new PathFigureCollection();
                pfc10.Add(new PathFigure());
                pfc10.Add(new PathFigure());

                ((IList)pfc10).Insert(5, new PathFigure());
            }
            catch (System.ArgumentException expectedException)
            {
                //Getting the exception message directly from the ExceptionStringTable in PresentationCore.dll in order
                //to the trouble of prepare all of those localized exception strings running on different locales.
                string message = "Specified argument was out of the range of valid values.\r\nParameter name: index";
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

            #region Test #11: ArgumentException when trying to remove an object that is not PathFigure by using IList.Remove(object value)
            CommonLib.LogStatus("Test #11: ArgumentException when trying to remove an object that is not PathFigure by using IList.Remove(object value)");
            testCaseID = 11;
            try
            {
                PathFigureCollection pfc11 = new PathFigureCollection();
                pfc11.Add(new PathFigure());
                pfc11.Add(new PathFigure());

                ((IList)pfc11).Remove(23);
            }
            catch (System.ArgumentException expectedException)
            {
                //Getting the exception message directly from the ExceptionStringTable in PresentationCore.dll in order
                //to the trouble of prepare all of those localized exception strings running on different locales.
                string message = rm.GetString("Collection_BadType");

                message = string.Format(message, "PathFigure");
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

            #region Test #12: ArgumentOutOfRangeException when trying to remove at an out of bound index in IList.RemoveAt(int index)
            CommonLib.LogStatus("Test #12: ArgumentOutOfRangeException when trying to remove at an out of bound index in IList.RemoveAt(int index)");
            testCaseID = 12;
            try
            {
                PathFigureCollection pfc12 = new PathFigureCollection();
                pfc12.Add(new PathFigure());
                pfc12.Add(new PathFigure());

                ((IList)pfc12).RemoveAt(3);
            }
            catch (System.ArgumentException expectedException)
            {
                //Getting the exception message directly from the ExceptionStringTable in PresentationCore.dll in order
                //to the trouble of prepare all of those localized exception strings running on different locales.
                string message = "Specified argument was out of the range of valid values.\r\nParameter name: index";
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

            if (string.Compare(failMessage, "", false, System.Globalization.CultureInfo.InvariantCulture) != 0)
            {
                CommonLib.LogFail(failMessage);
            }
            else
            {
                CommonLib.LogStatus("Test case passes!");
            }
        }
    }
}