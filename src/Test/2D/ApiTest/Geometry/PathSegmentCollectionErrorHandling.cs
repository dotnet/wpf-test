// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  API Tests - Testing PathSegmentCollection Exceptions
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
    internal class PathSegmentCollectionErrorHandling : ApiTest
    {
        public PathSegmentCollectionErrorHandling( double left, double top, double width, double height)
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
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager("FxResources.PresentationCore.SR", typeof(PathSegment).Assembly);

            #region Test #4: ArgumentNullException when the array is null in CopyTo(Array array, int index)
            CommonLib.LogStatus("Test #4: ArgumentNullException when the array is null in CopyTo(Array array, int index) ");
            testCaseID = 4;
            try
            {
                PathSegmentCollection pfc4 = new PathSegmentCollection();
                pfc4.Add(new LineSegment());

                pfc4.CopyTo(null, 0);
            }
            catch (System.ArgumentNullException expectedException)
            {
                string expectedMessage4 = Exceptions.GetExceptionMessage(typeof(ArgumentNullException), "array");
                
                if (String.Compare(expectedException.Message, expectedMessage4, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
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

            #region Test #5: ArgumentException when the passed array is not one dimentional in PathSegmentCollection(params PathSegment[] collection)
            CommonLib.LogStatus("Test #5: ArgumentException when the passed array is not one dimentional in PathSegmentCollection(params PathSegment[] collection)");
            testCaseID = 5;
            try
            {
                int[,] testarray = new int[2, 2];
                PathSegmentCollection pfc5 = new PathSegmentCollection();
                pfc5.Add(new LineSegment());

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
                }
            }
            catch (Exception e)
            {
                CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                CommonLib.LogFail("Test fails at case #" + testCaseID);
            }
            #endregion

            #region Test #6: ArgumentOutOfRangeException when the passed in array is not long enough in PathSegmentCollection(params PathSegment[] collection)
            CommonLib.LogStatus("Test #6: ArgumentOutOfRangeException when the passed in array is not long enough in PathSegmentCollection(params PathSegment[] collection)");
            testCaseID = 6;
            try
            {
                int[] testArray = new int[1];
                PathSegmentCollection pfc6 = new PathSegmentCollection();
                pfc6.Add(new LineSegment());
                pfc6.Add(new LineSegment());

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
                }
            }
            catch (Exception e)
            {
                CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                CommonLib.LogFail("Test fails at case #" + testCaseID);
            }
            #endregion

            #region Test #7: ArgumentException when adding bad type object in IList.Add(object value)
            CommonLib.LogStatus("Test #7: ArgumentException when adding bad type object in IList.Add(object value)");
            testCaseID = 7;
            try
            {
                PathSegmentCollection pfc7 = new PathSegmentCollection();

                ((IList)pfc7).Add(2323);
            }
            catch (System.ArgumentException expectedException)
            {
                //Getting the exception message directly from the ExceptionStringTable in PresentationCore.dll in order
                //to the trouble of prepare all of those localized exception strings running on different locales.
                string message = rm.GetString("Collection_BadType");

                message = string.Format(message, "PathSegmentCollection", "Int32", "PathSegment");
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

            #region Test #8: ArgumentException when trying  to insert a Null value in IList.Insert(int index, object value)
            CommonLib.LogStatus("Test #8: ArgumentException when trying  to insert a Null value in IList.Insert(int index, object value)");
            testCaseID = 8;
            try
            {
                PathSegmentCollection pfc8 = new PathSegmentCollection();
                pfc8.Add(new LineSegment());

                ((IList)pfc8).Insert(0, null);
            }
            catch (System.ArgumentNullException expectedException)
            {
                string expectedMessage8 = Exceptions.GetExceptionMessage(typeof(ArgumentNullException), "value");

                if (String.Compare(expectedException.Message, expectedMessage8, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
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
                PathSegmentCollection pfc9 = new PathSegmentCollection();
                pfc9.Add(new LineSegment());
                pfc9.Add(new LineSegment());

                ((IList)pfc9).Insert(1, 23232);
            }
            catch (System.ArgumentException expectedException)
            {
                //Getting the exception message directly from the ExceptionStringTable in PresentationCore.dll in order
                //to the trouble of prepare all of those localized exception strings running on different locales.
                string message = rm.GetString("Collection_BadType");

                message = string.Format(message, "PathSegmentCollection", "Int32", "PathSegment");
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
                PathSegmentCollection pfc10 = new PathSegmentCollection();

                ((IList)pfc10).Insert(5, new LineSegment());
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

            #region Test #11: ArgumentException when trying to remove an object that is not PathSegment by using IList.Remove(object value)
            CommonLib.LogStatus("Test #11: ArgumentException when trying to remove an object that is not PathSegment by using IList.Remove(object value)");
            testCaseID = 11;
            try
            {
                PathSegmentCollection pfc11 = new PathSegmentCollection();

                ((IList)pfc11).Remove(23);
            }
            catch (System.ArgumentException expectedException)
            {
                //Getting the exception message directly from the ExceptionStringTable in PresentationCore.dll in order
                //to the trouble of prepare all of those localized exception strings running on different locales.
                string message = rm.GetString("Collection_BadType");

                message = string.Format(message, "PathSegment");
                if (String.Compare(expectedException.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
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
                failMessage += "Test fails at case #" + testCaseID + "\n";
            }
            #endregion

            #region Test #12: ArgumentOutOfRangeException when trying to remove at an out of bound index in IList.RemoveAt(int index)
            CommonLib.LogStatus("Test #12: ArgumentOutOfRangeException when trying to remove at an out of bound index in IList.RemoveAt(int index)");
            testCaseID = 12;
            try
            {
                PathSegmentCollection pfc12 = new PathSegmentCollection();

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
                }
            }
            catch (Exception e)
            {
                CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                CommonLib.LogFail("Test fails at case #" + testCaseID);
            }
            #endregion

            #region Test #13: ArgumentNullException when trying to remove a Null value in Remove(PathSegment value)
            CommonLib.LogStatus("Test #13: ArgumentNullException when trying to remove a Null value in Remove(PathSegment value)");
            testCaseID = 13;
            try
            {
                PathSegmentCollection pfc13 = new PathSegmentCollection();
                pfc13.Add(new LineSegment());
                pfc13.Add(new LineSegment());

                pfc13.Remove(null);
            }
            catch (System.ArgumentNullException expectedException)
            {
                string expectedMessage13 = Exceptions.GetExceptionMessage(typeof(ArgumentNullException), "value");

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

            #region Test #14: ArgumentNullException when trying to Insert a Null value in Insert(int index, PathSegment value)
            CommonLib.LogStatus("Test #14: ArgumentNullException when trying to Insert a Null value in Insert(int index, PathSegment value)");
            testCaseID = 14;
            try
            {
                PathSegmentCollection pfc14 = new PathSegmentCollection();
                pfc14.Add(new LineSegment());
                pfc14.Add(new LineSegment());

                pfc14.Insert(0, null);
            }
            catch (System.ArgumentException expectedException)
            {
                //Getting the exception message directly from the ExceptionStringTable in PresentationCore.dll in order
                //to the trouble of prepare all of those localized exception strings running on different locales.
                string message = rm.GetString("Collection_NoNull");
                if (String.Compare(expectedException.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
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

            #region Test #22: ArgumentOutOfRangeException when Index < 0 or Index > _collection.Count at inserting item through PathSegmentCollection[index]
            CommonLib.LogStatus("Test #22:  ArgumentOutOfRangeException when Index < 0 or Index > _collection.Count at inserting item through PathSegmentCollection[index]");
            testCaseID = 22;
            try
            {
                PathSegmentCollection pfc22 = new PathSegmentCollection();
                pfc22[10] = new LineSegment();
            }
            catch (System.ArgumentOutOfRangeException expectedException)
            {
                //Getting the exception message directly from the ExceptionStringTable in PresentationCore.dll in order
                //to the trouble of prepare all of those localized exception strings running on different locales.
                string message = Exceptions.GetExceptionMessage(typeof(ArgumentOutOfRangeException), "index");
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

            #region Test #23: ArgumentNullException when passing NULL through PathSegmentCollection[index]
            CommonLib.LogStatus("Test #23:  ArgumentNullException when passing NULL through PathSegmentCollection[index]");
            testCaseID = 23;
            try
            {
                PathSegmentCollection pfc23 = new PathSegmentCollection();
                pfc23[0] = null;
            }
            catch (System.ArgumentException expectedException)
            {
                //Getting the exception message directly from the ExceptionStringTable in PresentationCore.dll in order
                //to the trouble of prepare all of those localized exception strings running on different locales.
                string message = rm.GetString("Collection_NoNull");

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

            #region Test #24: ArgumentOutOfRangeException when Index < 0 or Index > _collection.Count at inserting item through Insert(int index, PathSegment value)
            CommonLib.LogStatus("Test #24:  ArgumentOutOfRangeException when Index < 0 or Index > _collection.Count at inserting item through Insert(int index, PathSegment value)");
            testCaseID = 24;
            try
            {
                PathSegmentCollection pfc24 = new PathSegmentCollection();
                pfc24.Insert(10, new LineSegment());
            }
            catch (System.ArgumentOutOfRangeException expectedException)
            {
                //Getting the exception message directly from the ExceptionStringTable in PresentationCore.dll in order
                //to the trouble of prepare all of those localized exception strings running on different locales.
                string message = Exceptions.GetExceptionMessage(typeof(ArgumentOutOfRangeException), "index");
                if (String.Compare(expectedException.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
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

            CommonLib.LogTest("Test case Result:");
        }

    }
}