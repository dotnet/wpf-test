// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  GMC API Tests - Testing all public properties and methods in Shape class
//	Author:  Microsoft
//
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class ShareTest
    {
        public static bool RunCommonTest(HelperClass helper, Shape obj)
        {
            bool result = true;
            #region Test public properties in Shape class
            #region Fill property            
            CommonLib.LogStatus("Testing Fill property");
            obj.Fill = new SolidColorBrush(Colors.Red);
            if (obj.Fill is SolidColorBrush && ((SolidColorBrush)obj.Fill).Color == Colors.Red)
            {
                CommonLib.LogStatus("Pass: Fill = Brush has the expected value");
            }
            else
            {
                CommonLib.LogFail("Fail: Fill - Expected '" + Colors.Red.ToString() + "' color, Actual value: '" + obj.Fill.ToString() + "'");
                result = false;
            }
            #endregion
            #region Stroke property
            CommonLib.LogStatus("Testing Stroke property");
            obj.Stroke = new SolidColorBrush(Colors.Aqua);
            if (obj.Stroke is SolidColorBrush && ((SolidColorBrush)obj.Stroke).Color == Colors.Aqua)
            {
                CommonLib.LogStatus("Pass: Stroke = Brush has the expected value");
            }
            else
            {
                CommonLib.LogFail("Fail: Stroke - Expected '" + Colors.Aqua.ToString() + "' color, Actual value: '" + ((SolidColorBrush)obj.Stroke).Color.ToString() + "'");
                result = false;
            }
            #endregion
            #region StokeDashArray property
            CommonLib.LogStatus("Testing StokeDashArray property");
            DoubleCollection dc = new DoubleCollection();
            dc.Add(89.2);
            dc.Add(0.1);
            dc.Add(1.1);

            obj.StrokeDashArray = dc;
            result &= helper.CompareProp("StrokeDashArray", obj.StrokeDashArray, dc);
            #endregion

            #region StrokeDashOffset property
            CommonLib.LogStatus("Testing StrokeDashOffset property");
            obj.StrokeDashOffset = 10.333;
            result &= helper.CompareProp("StokeDashOffset", obj.StrokeDashOffset, 10.333);
            #endregion

            #region StrokeDashCap property
            CommonLib.LogStatus("Testing StrokeDashCap property");
            obj.StrokeDashCap = PenLineCap.Triangle;
            if (obj.StrokeDashCap == PenLineCap.Triangle)
            {
                CommonLib.LogStatus("Pass: StrokeDashCap = PenLineCap has the expected value");
            }
            else
            {
                CommonLib.LogFail("Fail: StrokeDashCap - Expected value 'Triangle', Actual value: '" + obj.StrokeDashCap.ToString() + "'");
                result = false;
            }
            #endregion

            #region StrokeEndLineCap property
            CommonLib.LogStatus("Testing StrokeEndLineCap property");
            obj.StrokeEndLineCap = PenLineCap.Round;
            if (obj.StrokeEndLineCap == PenLineCap.Round)
            {
                CommonLib.LogStatus("Pass: StrokeEndLineCap = PenLineCap has the expected value");
            }
            else
            {
                CommonLib.LogFail("Fail: StrokeEndLineCap - Expected value 'Round', Actual value: '" + obj.StrokeEndLineCap.ToString() + "'");
                result = false;
            }
            #endregion

            #region StrokeLineJoin property
            CommonLib.LogStatus("Testing StrokeLineJoin property");
            obj.StrokeLineJoin = PenLineJoin.Miter;
            if (obj.StrokeLineJoin == PenLineJoin.Miter)
            {
                CommonLib.LogStatus("Pass: StrokeLineJoin = PenLineJoin has the expected value");
            }
            else
            {
                CommonLib.LogFail("Fail: StrokeLineJoin - Expected value 'Miter', Actual value: '" + obj.StrokeLineJoin.ToString() + "'");
                result = false;
            }
            #endregion

            #region StrokeMiterLimit property
            CommonLib.LogStatus("Testing StrokeMiterLimit property");
            obj.StrokeMiterLimit = 2.3;
            result &= helper.CompareProp("StrokeMiterLimit", obj.StrokeMiterLimit, 2.3);
            #endregion

            #region StrokeStartLineCap property
            CommonLib.LogStatus("Testing StrokeStartLineCap property");
            obj.StrokeStartLineCap = PenLineCap.Square;
            if (obj.StrokeStartLineCap == PenLineCap.Square)
            {
                CommonLib.LogStatus("Pass: StrokeStartLineCap = PenLineCap has the expected value");
            }
            else
            {
                CommonLib.LogFail("Fail: StrokeStartLineCap - Expected value 'Square', Actual value: '" + obj.StrokeStartLineCap.ToString() + "'");
                result = false;
            }
            #endregion

            #region StrokeThickness property
            CommonLib.LogStatus("Testing StrokeThickness property");
            obj.StrokeThickness = 29;
            if (obj.StrokeThickness == 29)
            {
                CommonLib.LogStatus("Pass: StrokeThickness = Length has the expected value");
            }
            else
            {
                CommonLib.LogFail("Fail: StrokeThickness - Expected value Length 29, Actual value: '" + obj.StrokeThickness.ToString() + "'");
                result = false;
            }
            #endregion
            #endregion

            return result;
        }

        public static void ForceRender(Shape shape)
        {
            shape.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            shape.Arrange(new Rect(shape.DesiredSize));
            shape.UpdateLayout();
        }
    }
}
