// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Program:  GMC API Tests - Testing CloseSegment class
//  Author:   Microsoft
//

using System;
using System.Windows;
using System.Windows.Media;
using System.Collections;
using System.Reflection;
using System.Globalization;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Summary description for CommonLib.
    /// </summary>
    internal class CommonLib
    {
        static CommonLib()
        {
            s_log = new TestLog("ApiTest");
            s_log.Result = TestResult.Pass;
        }

        internal static void CloseLog()
        {
            s_log.Close();
        }

        #region Public functions
        public static void LogFail(string failMessage)
        {
            s_log.LogStatus("*** Fail: " + failMessage);
            errorMessage += "*** " + failMessage + "\n";
            ErrorFlag = true;
        }

        public static void LogStatus(string message)
        {
            s_log.LogStatus(message);
        }

        public static void LogTest(bool testPassed, string testReport)
        {
            if (!testPassed)
            {
                ErrorFlag = true;
            }
            LogTest(testReport);
        }

        public static void LogTest( string testReport)
        {
            LogStatus(testReport);
            LogResult();
        }


        public static void LogResult()
        {
            if (ErrorFlag)
            {
                s_log.LogStatus("Test failed!");
                s_log.Result=TestResult.Fail;
            }
            else
            {
                s_log.LogStatus("Test Passed.");
                s_log.Result = TestResult.Pass;
            }
        }

        public static void TypeVerifier(object type, string typeName)
        {
            if (type == null)
            {
                LogFail("it should not be NULL");
                return;
            }

            if (type.GetType().ToString() == typeName)
            {
                s_log.LogStatus("Pass: " + typeName + " was created successfully");
            }
            else
            {
                LogFail(typeName + " was NOT created, a " + type.GetType().ToString() + " was created");
            }
        }

        public static void GenericVerifier(bool result, string testName)
        {
            if (result)
            {
                s_log.LogStatus("Pass: " + testName + " was fully successful");
            }
            else
            {
                LogFail(testName + " failed");
            }
        }

        public static void DoubleCollectionVerifier(double[] doubles, DoubleCollection dc)
        {
            DoubleCollectionVerifier(new DoubleCollection(doubles), dc);
        }

        public static void DoubleCollectionVerifier(DoubleCollection dc1, DoubleCollection dc2)
        {
            bool result = true;

            if (dc1.Count != dc2.Count)
            {
                LogFail("Error:  these two DoubleCollections contain different double(s)");
                LogFail("\tCount of first DoubleCollection = " + dc1.Count);
                LogFail("\tCount of second DoubleCollection = " + dc2.Count);
            }

            for (int i = 0; i < dc1.Count; i++)
            {
                if (!Double.Equals(dc1[i], dc2[i]))
                {
                    LogFail("The double at index " + i + " are mismatch");
                    result = false;
                    break;
                }
            }
            if (result)
            {
                LogStatus("Pass!");
            }

        }

        public static void PointVerifier(Point newPoint, Point expectedPoint)
        {
            if (newPoint.X == expectedPoint.X && newPoint.Y == expectedPoint.Y)
            {
                CommonLib.LogStatus("Pass: New point was expected, (" + newPoint.X.ToString() + ", " + newPoint.Y.ToString() + ") ");
            }
            else
            {
                LogFail("New Point value was unexpected, (" + newPoint.X.ToString() + ", " + newPoint.Y.ToString() + ") ");
            }
        }

        public static void SizeVerifier(Size newSize, Size expectedSize)
        {
            if (newSize.Height == expectedSize.Height && newSize.Width == expectedSize.Width)
            {
                CommonLib.LogStatus("Pass: New size was expected, (" + newSize.Height.ToString() + ", " + newSize.Width.ToString() + ") ");
            }
            else
            {
                LogFail("New size value was unexpected, (" + newSize.Height.ToString() + ", " + newSize.Width.ToString() + ") ");
            }
        }

        public static void RectVerifier(Rect newRect, Rect expectedRect)
        {
            if (Math.Round(newRect.Top, 2) == Math.Round(expectedRect.Top, 2) && Math.Round(newRect.Left, 2) == Math.Round(expectedRect.Left, 2) && Math.Round(newRect.Width, 2) == Math.Round(expectedRect.Width, 2) && Math.Round(newRect.Height, 2) == Math.Round(expectedRect.Height, 2))
            {
                CommonLib.LogStatus("Pass: new rect value was expected, (" + newRect.Left + ", " + newRect.Top + ", " + newRect.Width + ", " + newRect.Height + ") ");
            }
            else
            {
                LogFail("new rect value was unexpected, (" + newRect.Left + ", " + newRect.Top + ", " + newRect.Width + ", " + newRect.Height + ") ");
            }
        }

        public static bool CompareString(string s1, string s2)
        {
            return CompareString(s1, s2, CultureInfo.InvariantCulture);
        }

        public static bool CompareString(string s1, string s2, System.IFormatProvider iprovider)
        {
            if (string.Compare(s1, s2, false, (CultureInfo)iprovider) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Perform the test on a given geometry.
        /// Return false if the test failed
        /// </summary>
        public static void CircularTestGeometry(Geometry geometry)
        {
            int resolution = 100;
            int probes = resolution * resolution;
            double margin = 0.1;
            double ComputedArea = geometry.GetArea(0, ToleranceType.Absolute);
            double BoundsArea = Math.Abs(geometry.Bounds.Width * geometry.Bounds.Height);
            int hits = 0;
            double dx = geometry.Bounds.Width / resolution;
            double dy = geometry.Bounds.Height / resolution;

            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    if (geometry.FillContains(new Point(geometry.Bounds.Left + i * dx,
                                                        geometry.Bounds.Top + j * dy)))
                    {
                        hits += 1;
                    }
                }
            }

            // hits / probes should be roughly equal to ComputedArea / BoundsArea
            double HitArea = BoundsArea * hits;
            HitArea /= probes;

            double error = Math.Abs(HitArea - ComputedArea);

            if (error <= BoundsArea * margin)
                CommonLib.GenericVerifier(true, geometry.ToString() + " HitTest and GetArea");
            else
                CommonLib.GenericVerifier(false, geometry.ToString() + " HitTest and GetArea");

        }

        /// <summary>
        /// Get HitTest hit number
        /// </summary>
        /// <param name="geo">The tested Geometry</param>
        /// <param name="resolution">the partition ratio</param>
        /// <returns># of Hits on the tested Geometry</returns>
        public static int HitTest(Geometry geo, int resolution)
        {
            int hits = 0;

            double dx = geo.Bounds.Width / resolution;
            double dy = geo.Bounds.Height / resolution;

            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    if (geo.FillContains(new Point(geo.Bounds.Left + i * dx,
                                                        geo.Bounds.Top + j * dy)))
                    {
                        hits += 1;
                    }
                }
            }

            return hits;
        }

        private static bool IsPropertyProblematic(string propertyName)
        {
            // These properties cause exceptions, infinite loops, or are useless info

            return propertyName == "Inverse";
        }


        /// <summary>
        /// Comparing all of the properties of two objects recursively
        /// Copy it from CoreGraphics ObjectUtils.cs
        /// Author: Microsoft
        /// </summary>
        /// <param name="obj1">First object</param>
        /// <param name="obj2">Second object</param>
        /// <returns>Boolean value of the result</returns>
        public static bool DeepEqual(object obj1, object obj2)
        {
            if (object.ReferenceEquals(obj1, obj2))
            {
                // Shortcut- if they are the same object, return true;
                return true;
            }
            Type type1 = obj1.GetType();
            Type type2 = obj2.GetType();
            if (type1 != type2)
            {
                return false;
            }
            if (type1 == typeof(string))
            {
                return obj1.ToString() == obj2.ToString();
            }

            bool equals;
            PropertyInfo[] properties = type1.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in properties)
            {
                if (IsPropertyProblematic(property.Name))
                {
                    continue;
                }
                if (property.Name == "Item")
                {
                    if (obj1 is IEnumerable)
                    {
                        equals = DeepEqual((IEnumerable)obj1, (IEnumerable)obj2);
                    }
                    else
                    {
                        // This is an indexer.  We can't really compare it.
                        equals = true;
                    }
                }
                else
                {
                    object value1 = property.GetValue(obj1, null);
                    object value2 = property.GetValue(obj2, null);

                    if (property.PropertyType.IsValueType)
                    {
                        equals = object.Equals(value1, value2);
                    }
                    else
                    {
                        equals = DeepEqual(value1, value2);
                    }
                }
                if (!equals)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool DeepEqual(IEnumerable obj1, IEnumerable obj2)
        {
            IEnumerator e1 = obj1.GetEnumerator();
            IEnumerator e2 = obj2.GetEnumerator();

            while (true)
            {
                bool more1 = e1.MoveNext();
                bool more2 = e2.MoveNext();
                if (more1 != more2)
                {
                    // They don't have the same number of items.
                    return false;
                }
                if (more1 == false)
                {
                    // No more objects to compare
                    return true;
                }
                object value1 = e1.Current;
                object value2 = e2.Current;

                if (!DeepEqual(value1, value2))
                {
                    return false;
                }
            }
        }

        public static bool TestPassing
        {
            get { return !ErrorFlag; }
        }

        
        public static TestStage Stage
        {
            set { s_log.Stage = value; }
            get { return s_log.Stage; }
        }

        public static TestLog Log
        {
            get { return s_log; }
        }
        #endregion

        #region Public variable
        private static TestLog s_log;

        public static string errorMessage;
        public static bool ErrorFlag 
        {
            get{return(errorFlag);}
            set{errorFlag=value;}
        }
        public static bool errorFlag = false;
        #endregion
    }
}