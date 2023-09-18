// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  GMC API Tests - Testing GeometryConverter class
//  Author:   Microsoft
//
using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Summary description for GeometryConverterClass.
    /// </summary>
    internal class GeometryConverterClass : ApiTest
    {
        public GeometryConverterClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus("GeometryCoverter Class");
            MyTypeConverter valid_type_converter = new MyTypeConverter(new StreamGeometry());
            MyTypeConverter invalid_type_converter = new MyTypeConverter(Colors.Red);

            string objectType = "System.Windows.Media.GeometryConverter";

            #region Section I: Constructor
            CommonLib.Stage = TestStage.Initialize;
            CommonLib.LogStatus("Test #1: default constructor");
            GeometryConverter pgc = new GeometryConverter();
            CommonLib.TypeVerifier(pgc, objectType);
            #endregion

            #region Section II: Public methods
            #region Test #2 CanConvertTo()
            CommonLib.LogStatus("Test #2 CanConvertTo() with non null typeDescriptorContext");
            bool result22 = pgc.CanConvertTo(valid_type_converter, typeof(string));
            result22 &= !pgc.CanConvertTo(null, typeof(int));

            try
            {
                result22 &= pgc.CanConvertTo(invalid_type_converter, typeof(string));
            }
            catch (System.Exception)
            {
                CommonLib.LogStatus("An exception is thrown, it is actually correct!");
            }

            CommonLib.GenericVerifier(result22 == true,
                                 "CanConvertTo()");
            #endregion

            #region Test #3: ConvertTo()
            //May need to change logter Eric brings the Converter fully to parity
            CommonLib.LogStatus("Test #3: ConvertTo()");
            bool result3 = true;

            PathGeometry pg = new PathGeometry();
            PathFigure pf = new PathFigure();
            pf.StartPoint = new Point(2, 2);
            pf.Segments.Add(new LineSegment(new Point(300, 213), true));
            pf.IsClosed = true;
            pg.Figures.Add(pf);

            string returnString = pgc.ConvertTo(null, System.Globalization.CultureInfo.CurrentCulture, pg, typeof(string)) as string;
            string expectedString = "M2,2L300,213z";
            result3 &= CommonLib.CompareString(returnString, expectedString);

            returnString = pgc.ConvertTo(null,
                                         System.Globalization.CultureInfo.CurrentCulture,
                                         new LineGeometry(new Point(32, 23), new Point(0, 0)),
                                         typeof(string)) as string;
            expectedString = "LineGeometry";
            result3 &= CommonLib.CompareString(returnString, expectedString);

            returnString = pgc.ConvertTo(null,
                            System.Globalization.CultureInfo.InvariantCulture,
                            null,
                            typeof(string)) as string;
            expectedString = "";
            result3 &= CommonLib.CompareString(returnString, expectedString);

            // Calling the ConvertTo Method with a Type argument that is null
            // should generate the expected exception.
            try
            {
                returnString = pgc.ConvertTo(null, System.Globalization.CultureInfo.InvariantCulture, new PathGeometry(), null) as string;
                // If program can get to the line below, it means no exception is fired.  It is INCORRECT!
                result3 &= false;
            }
            catch (System.ArgumentNullException)
            {
                result3 &= true;
            }

            // Calling the ConvertTo Method with the wrong type of object should
            // generate the expected exception
            try
            {
                returnString = pgc.ConvertTo(null,
                                            System.Globalization.CultureInfo.InvariantCulture,
                                            new RectangleGeometry(new Rect(0, 1, 324, 2)),
                                            typeof(Geometry)) as string;
                // If the program can reach the code below, that means no exception is fired. 
                // It is INCORRECT  
                result3 &= false;
            }
            catch (System.NotSupportedException)
            {
                result3 &= true;
            }

            try
            {
                returnString = pgc.ConvertTo(invalid_type_converter,
                                            System.Globalization.CultureInfo.InstalledUICulture,
                                            new RectangleGeometry(new Rect(1, 320, 1230, 10)),
                                            typeof(string)) as string;
                result3 &= false;
            }
            catch (System.NotSupportedException)
            {
                result3 &= true;
            }

            CommonLib.GenericVerifier(result3, "ConvertTo()");
            #endregion

            #region Test #4: ConvertFrom()
            CommonLib.LogStatus("Test #4: ConvertFrom()");
            bool test4_result = true;
            object test4_object = pgc.ConvertFrom(null, System.Globalization.CultureInfo.CurrentCulture, "M 100 100 L 200 100 L 200 100 z");
            test4_result &= test4_object != null && test4_object is StreamGeometry;

            try
            {
                // Calling the ConvertFrom method with a null string should generate the
                // expected exception.
                object test4_null = pgc.ConvertFrom(null, System.Globalization.CultureInfo.InvariantCulture, null) as string;
                // If the program can reach the code below, it means no exception is fired which is INCORRECT.
                test4_result &= false;
            }
            catch (System.Exception)
            {
                // An exception is fired from the ConvertFrom which is  CORRECT! 
                test4_result &= true;
            }

            try
            {
                // Calling the ConvertFrom Method with the wrong type of object should
                // generate the expected exception.
                test4_object = pgc.ConvertFrom(null, System.Globalization.CultureInfo.CurrentCulture, Colors.Green) as string;
                // If the program can reach the code below, it means no exception is fired which is INCORRECT.
                test4_result &= false;
            }
            catch (System.Exception)
            {
                // An exception is fired from the ConvertFrom which is  CORRECT! 
                test4_result &= true;
            }

            CommonLib.GenericVerifier(test4_result, "ConvertFrom() ");
            #endregion

            #region Test #5: ConvertFromString()
            CommonLib.LogStatus("Test #5: ConvertFromString()");
            object testObject5 = pgc.ConvertFromString("M 100 100 L 200 19 L 100 90 L 32 23 L 32 11 z");
            PathGeometry pg5 = (testObject5 != null && testObject5 is StreamGeometry) ? PathGeometry.CreateFromGeometry((Geometry)testObject5) : null;
            CommonLib.GenericVerifier(pg5.Figures[0].Segments[0] is PolyLineSegment, "ConvertFromString(string)");
            #endregion

            #region Test #6: CanConvertFrom()
            CommonLib.LogStatus("Test #6: CanConvertFrom()");
            //GeometryCoverter is able to cover string
            //GeometryCoverter should not be able to cover typeof(double)
            if (pgc.CanConvertFrom(null, typeof(string)) == true && pgc.CanConvertFrom(null, typeof(double)) == false)
            {
                CommonLib.GenericVerifier(true, "CanConvertFrom()");
            }
            else
            {
                CommonLib.GenericVerifier(false, "CanConvertFrom()");
            }
            #endregion

            #region Test #7:  Regression test for Regression_Bug5
            CommonLib.LogStatus("Test #7:  Regression test for Regression_Bug5");
            try
            {
                string returnString7 = pgc.ConvertTo(null, System.Globalization.CultureInfo.CurrentCulture, new EllipseGeometry(), typeof(string)) as string;
                string expectedString7 = "System.Windows.Media.EllipseGeometry";

                if (string.Compare(returnString7, expectedString7, false, System.Globalization.CultureInfo.CurrentCulture) >= 0)
                {
                    CommonLib.LogStatus("GeometryConverter is capable of converting a EllipseGeometry object to its destinate type");
                    CommonLib.LogStatus("Regression_Bug5 is still fixed, no regression!");
                }
            }
            catch (System.Exception)
            {
                CommonLib.GenericVerifier(false, "Regression_Bug5");
            }

            #endregion

            #endregion

            CommonLib.LogTest("Result for:" + objectType);

        }
    }
}