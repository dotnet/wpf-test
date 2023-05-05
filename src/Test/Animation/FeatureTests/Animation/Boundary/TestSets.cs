// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Windows;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Globalization;
using System.Collections;
using System.Collections.Specialized;

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

//---------------------------------------------------------------------------------------------

namespace                       Microsoft.Test.Animation
{
    //------------------------------------------------------------------------------------------
    public class TestSets
    {
        #region Methods
        //--------------------------------------------------------------------------------------
        public static bool SetBadNullable(object o, PropertyInfo p, ListDictionary badValues)
        {
            bool messageResult = true;

            foreach (DictionaryEntry badInput in badValues)
            {
                s_tstNumber++;
                //The test will pass only if it produces an exception.
                GlobalLog.LogStatus("---SetBadNullable---" + p.Name);
                try
                {
                    switch (p.PropertyType.ToString())
                    {
                        case "System.Nullable`1[System.Double]":
                            {
                                p.SetValue(o, (Nullable<Double>)(double)badInput.Key, null);
                                break;
                            }
                        case "System.Nullable`1[System.Int16]":
                            {
                                p.SetValue(o, (Nullable<Int16>)(int)badInput.Key, null);
                                break;
                            }
                        case "System.Nullable`1[System.Windows.Media.Media3D.Point3D]":
                            {
                                p.SetValue(o, (Nullable<Point3D>)(Point3D)badInput.Key, null);
                                break;
                            }
                        case "System.Nullable`1[System.Windows.Point]":
                            {
                                p.SetValue(o, (Nullable<Point>)(Point)badInput.Key, null);
                                break;
                            }
                        case "System.Nullable`1[System.Windows.Rect]":
                            {
                                p.SetValue(o, (Nullable<Rect>)(Rect)badInput.Key, null);
                                break;
                            }
                        case "System.Windows.Media.Media3D.AxisAngleRotation3D":
                            {
                                p.SetValue(o, (AxisAngleRotation3D)badInput.Key, null);
                                break;
                            }
                        case "System.Windows.Media.Media3D.Rotation3D":
                            {
                                p.SetValue(o, (Rotation3D)badInput.Key, null);
                                break;
                            }
                        case "System.Nullable`1[System.Single]":
                            {
                                p.SetValue(o, (Nullable<Single>)(float)badInput.Key, null);
                                break;
                            }
                        case "System.Nullable`1[System.Windows.Media.Media3D.Size3D]":
                            {
                                p.SetValue(o, (Nullable<Size3D>)(Size3D)badInput.Key, null);
                                break;
                            }
                        case "System.Nullable`1[System.Windows.Size]":
                            {
                                p.SetValue(o, (Nullable<Size>)(Size)badInput.Key, null);
                                break;
                            }
                        case "System.Nullable`1[System.Windows.Vector]":
                            {
                                p.SetValue(o, (Nullable<Vector>)(Vector)badInput.Key, null);
                                break;
                            }
                        case "System.Nullable`1[System.Windows.Media.Media3D.Vector3D]":
                            {
                                p.SetValue(o, (Nullable<Vector3D>)(Vector3D)badInput.Key, null);
                                break;
                            }
                        case "System.Nullable`1[System.Windows.Thickness]":
                            {
                                p.SetValue(o, (Nullable<Thickness>)(Thickness)badInput.Key, null);
                                break;
                            }
                        default:
                            {
                                GlobalLog.LogEvidence("!!!FAIL (PropertyType not found!!!)");
                                GlobalLog.LogEvidence("----Actual PropertyType: " + p.PropertyType.ToString());
                                break;
                            }
                    }
                    GlobalLog.LogEvidence(" -RESULT: False (No Exception found)");
                }
                catch (Exception e9)
                {
                    messageResult &= HandleException(s_tstNumber, e9, (string)badInput.Value);
                }
            }
            return messageResult;
        }
        //--------------------------------------------------------------------------------------
        public static bool SetBadQuaternion(object o, PropertyInfo p, ListDictionary badValues)
        {
            //The test will pass only if it produces an exception.
            GlobalLog.LogStatus("---SetBadQuaternion---" + p.Name);
            
            bool messageResult = false;
            foreach (DictionaryEntry badInput in badValues)
            {
                s_tstNumber++;
                try
                {
                    p.SetValue(o, (Quaternion)badInput.Key, null);
                    GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: False (No Exception found)");
                    return true;
                }
                catch (Exception e1)
                {
                    messageResult = HandleException(s_tstNumber, e1, (string)badInput.Value);
                }
            }
            return messageResult;
        } 
        //--------------------------------------------------------------------------------------
        public static bool SetBadRotation3D(object o, PropertyInfo p, ListDictionary badValues)
        {
            //The test will pass only if it produces an exception.
            GlobalLog.LogStatus("---SetBadRotation3D---" + p.Name);
            
            bool messageResult = false;
            foreach (DictionaryEntry badInput in badValues)
            {
                s_tstNumber++;
                try
                {
                    p.SetValue(o, (AxisAngleRotation3D)badInput.Key, null);
                    GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: False (No Exception found)");
                    return true;
                }
                catch (Exception e1)
                {
                    messageResult = HandleException(s_tstNumber, e1, (string)badInput.Value);
                }
            }
            return messageResult;
        } 
        //---------------------------------------------------------
        public static bool SetGeneric(object o, PropertyInfo p, ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                if (p.Name == "Item")
                {
                    GlobalLog.LogEvidence("Cannot set Item, by design");
                    continue;
                }
                s_tstNumber++;
                GlobalLog.LogStatus("---SetGeneric---" + p.Name);
                GlobalLog.LogEvidence("--------------------------------------------");
                //----SET THE VALUE
                p.SetValue(o, goodInput.Key, null);

                //----GET THE VALUE
                object actualValue = (object)p.GetValue(o, null);

                //----COMPARE VALUES
                s_compareResult &= CompareGenericValues(p, goodInput.Value, actualValue);
                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + goodInput.Value + " -Actual: " + actualValue);
           
            }
            return s_compareResult;
        }
        //--------------------------------------------------------------------------------------
        public static bool SetNullableByte(object o, PropertyInfo p, ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                s_tstNumber++;
                
                GlobalLog.LogStatus("---SetNullableByte---" + p.Name);
                //----SET THE VALUE
                p.SetValue(o, goodInput.Key, null);

                //----GET THE VALUE
                Nullable<Byte> actualValue = (Nullable<Byte>)p.GetValue(o, null);

                //----COMPARE VALUES
                s_compareResult &= (actualValue == (Nullable<Byte>)goodInput.Value);
                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + goodInput.Value + " -Actual: " + actualValue);
            }
            return s_compareResult;
        }
        //--------------------------------------------------------------------------------------
        public static bool SetNullableColor(object o, PropertyInfo p, ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                s_tstNumber++;
                GlobalLog.LogStatus("---SetNullableColor---" + p.Name);
         
               //----SET THE VALUE
                p.SetValue(o, goodInput.Key, null);

                //----GET THE VALUE
                Nullable<Color> actualValue = (Nullable<Color>)p.GetValue(o, null);

                //----COMPARE VALUES
                s_compareResult &= (actualValue == (Nullable < Color >) goodInput.Value);
                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + goodInput.Value + " -Actual: " + actualValue);
            }
            return s_compareResult;
        }

        //---------------------------------------------------------
        public static bool SetNullableDecimal(object o, PropertyInfo p, ListDictionary goodValues)
        {
            //GlobalLog.LogStatus("---SetNullableDecimal---" + p.Name);
            GlobalLog.LogEvidence("----------------------------------------");
            foreach (DictionaryEntry goodInput in goodValues)
            {
                s_tstNumber++;
                //----SET THE VALUE
                p.SetValue(o, goodInput.Key, null);

                //----GET THE VALUE
                Nullable<Decimal> actualValue = (Nullable<Decimal>)p.GetValue(o, null);

                //----COMPARE VALUES
                s_compareResult &= ((Nullable<Decimal>)goodInput.Value == actualValue);
                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + goodInput.Value + " -Actual: " + actualValue);
            }
            return s_compareResult;
        }
        //--------------------------------------------------------------------------------------
        public static bool SetNullableDouble(object o, PropertyInfo p, ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                s_tstNumber++;
                GlobalLog.LogStatus("---SetNullableDouble---" + p.Name);
                //----SET THE VALUE
                p.SetValue(o, goodInput.Key, null);

                //----GET THE VALUE
                Nullable<Double> actualValue = (Nullable<Double>)p.GetValue(o, null);

                //----COMPARE VALUES
                s_compareResult &= (actualValue == (Nullable<Double>)goodInput.Value);
                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + goodInput.Value + " -Actual: " + actualValue);
            }

            return s_compareResult;
        }
        //--------------------------------------------------------------------------------------
        public static bool SetNullableInt16(object o, PropertyInfo p, ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                s_tstNumber++;
                GlobalLog.LogStatus("---SetNullableInt16---" + p.Name);
                
                //----SET THE VALUE
                p.SetValue(o, goodInput.Key, null);

                //----GET THE VALUE
                Nullable<Int16> actualValue = (Nullable<Int16>)p.GetValue(o, null);

                //----COMPARE VALUES
                s_compareResult &= (actualValue == (Nullable<Int16>)goodInput.Value);
                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + goodInput.Value + " -Actual: " + actualValue);
            }
            return s_compareResult;
        }
        //--------------------------------------------------------------------------------------
        public static bool SetNullableInt32(object o, PropertyInfo p, ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                s_tstNumber++;
                GlobalLog.LogStatus("---SetNullableInt32---" + p.Name);
                
                //----SET THE VALUE
                p.SetValue(o, goodInput.Key, null);

                //----GET THE VALUE
                Nullable<Int32> actualValue = (Nullable<Int32>)p.GetValue(o, null);

                //----COMPARE VALUES
                s_compareResult &= (actualValue == (Nullable<Int32>)goodInput.Value);
                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + goodInput.Value + " -Actual: " + actualValue);
            }
            return s_compareResult;
        }
        //--------------------------------------------------------------------------------------
        public static bool SetNullableInt64(object o, PropertyInfo p, ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                s_tstNumber++;
                GlobalLog.LogStatus("---SetNullableInt64---" + p.Name);

                //----SET THE VALUE
                p.SetValue(o, goodInput.Key, null);

                //----GET THE VALUE
                Nullable<Int64> actualValue = (Nullable<Int64>)p.GetValue(o, null);

                //----COMPARE VALUES
                s_compareResult &= (actualValue == (Nullable<Int64>)goodInput.Value);
                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + goodInput.Value + " -Actual: " + actualValue);
            }
            return s_compareResult;
        }
        //------------------------------------------------------------
        public static bool SetNullablePoint(object o, PropertyInfo p, ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                s_tstNumber++;
                GlobalLog.LogStatus("---SetNullablePoint---" + p.Name);
         
                //----SET THE VALUE
                p.SetValue(o, goodInput.Key, null);

                //----GET THE VALUE
                Nullable<System.Windows.Point> actualValue = (Nullable<System.Windows.Point>)p.GetValue(o, null);

                //----COMPARE VALUES
                s_compareResult &= (actualValue == (Nullable<System.Windows.Point>)goodInput.Value);
                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + goodInput.Value + " -Actual: " + actualValue);
            }
            return s_compareResult;
        }
        //------------------------------------------------------------
        public static bool SetNullablePoint3D(object o, PropertyInfo p, ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                s_tstNumber++;
                GlobalLog.LogStatus("---SetNullablePoint3D---" + p.Name);

                //----SET THE VALUE
                p.SetValue(o, goodInput.Key, null);

                //----GET THE VALUE
                Nullable<Point3D> actualValue = (Nullable<Point3D>)p.GetValue(o, null);

                //----COMPARE VALUES
                s_compareResult &= (actualValue == (Nullable<Point3D>)goodInput.Value);
                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + goodInput.Value + " -Actual: " + actualValue);
            }
            return s_compareResult;
        }

        //------------------------------------------------------------
        public static bool SetNullableRect(object o, PropertyInfo p, ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                s_tstNumber++;
                GlobalLog.LogStatus("---SetNullableRect---" + p.Name);

                //----SET THE VALUE
                p.SetValue(o, goodInput.Key, null);

                //----GET THE VALUE
                Nullable<System.Windows.Rect> actualValue = (Nullable<System.Windows.Rect>)p.GetValue(o, null);

                //----COMPARE VALUES
                s_compareResult &= (actualValue == (Nullable<System.Windows.Rect>)goodInput.Value);
                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + goodInput.Value + " -Actual: " + actualValue);
            }
            return s_compareResult;
        }
        //--------------------------------------------------------------------------------------
        public static bool SetNullableSingle(object o, PropertyInfo p, ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                s_tstNumber++;
                GlobalLog.LogStatus("---SetNullableSingle---" + p.Name);

                //----SET THE VALUE
                p.SetValue(o, goodInput.Key, null);

                //----GET THE VALUE
                Nullable<Single> actualValue = (Nullable<Single>)p.GetValue(o, null);

                //----COMPARE VALUES
                s_compareResult &= (actualValue == (Nullable<Single>)goodInput.Value);
                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + goodInput.Value + " -Actual: " + actualValue);
            }
            return s_compareResult;
        }
        //--------------------------------------------------------------------------------------
        public static bool SetNullableSize(object o, PropertyInfo p, ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                s_tstNumber++;
                GlobalLog.LogStatus("---SetNullableSize---" + p.Name);

                //----SET THE VALUE
                p.SetValue(o, goodInput.Key, null);

                //----GET THE VALUE
                Nullable<Size> actualValue = (Nullable<Size>)p.GetValue(o, null);

                //----COMPARE VALUES
                s_compareResult &= (actualValue == (Nullable<Size>)goodInput.Value);
                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + goodInput.Value + " -Actual: " + actualValue);
            }
            return s_compareResult;
        }
        //--------------------------------------------------------------------------------------
        public static bool SetNullableSize3D(object o, PropertyInfo p, ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                s_tstNumber++;
                GlobalLog.LogStatus("---SetNullableSize3D---" + p.Name);

                //----SET THE VALUE
                p.SetValue(o, goodInput.Key, null);

                //----GET THE VALUE
                Nullable<Size3D> actualValue = (Nullable<Size3D>)p.GetValue(o, null);

                //----COMPARE VALUES
                s_compareResult &= (actualValue == (Nullable<Size3D>)goodInput.Value);
                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + goodInput.Value + " -Actual: " + actualValue);
            }
            return s_compareResult;
        }
        //--------------------------------------------------------------------------------------
        public static bool SetNullableThickness(object o, PropertyInfo p, ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                s_tstNumber++;
                GlobalLog.LogStatus("---SetNullableThickness---" + p.Name);

                //----SET THE VALUE
                p.SetValue(o, goodInput.Key, null);

                //----GET THE VALUE
                Nullable<Thickness> actualValue = (Nullable<Thickness>)p.GetValue(o, null);

                //----COMPARE VALUES
                s_compareResult &= (actualValue == (Nullable<Thickness>)goodInput.Value);
                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + goodInput.Value + " -Actual: " + actualValue);
            }
            return s_compareResult;
        }
        //--------------------------------------------------------------------------------------
        public static bool SetNullableVector(object o, PropertyInfo p, ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                s_tstNumber++;
                GlobalLog.LogStatus("---SetNullableVector---" + p.Name);

                //----SET THE VALUE
                p.SetValue(o, goodInput.Key, null);

                //----GET THE VALUE
                Nullable<Vector> actualValue = (Nullable<Vector>)p.GetValue(o, null);

                //----COMPARE VALUES
                s_compareResult &= (actualValue == (Nullable<Vector>)goodInput.Value);
                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + goodInput.Value + " -Actual: " + actualValue);
            }
            return s_compareResult;
        }
        //--------------------------------------------------------------------------------------
        public static bool SetNullableVector3D(object o, PropertyInfo p, ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                s_tstNumber++;
                GlobalLog.LogStatus("---SetNullableVector3D---" + p.Name);

                //----SET THE VALUE
                p.SetValue(o, goodInput.Key, null);

                //----GET THE VALUE
                Nullable<Vector3D> actualValue = (Nullable<Vector3D>)p.GetValue(o, null);

                //----COMPARE VALUES
                s_compareResult &= (actualValue == (Nullable<Vector3D>)goodInput.Value);
                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + goodInput.Value + " -Actual: " + actualValue);
            }
            return s_compareResult;
        }
   
        //------------------------------------------------------------
        public static bool SetPathGeometry(object o, PropertyInfo p, ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                s_tstNumber++;
                GlobalLog.LogStatus("---SetPathGeometry---" + p.Name);
         
                //----SET THE VALUE
                p.SetValue(o, goodInput.Key, null);

                //----GET THE VALUE
                System.Windows.Media.PathGeometry actualValue = (System.Windows.Media.PathGeometry)p.GetValue(o, null);
                System.Windows.Media.PathGeometry expectedValue = (System.Windows.Media.PathGeometry)goodInput.Value;

                //----COMPARE VALUES
                s_compareResult &= (actualValue.IsEmpty() == expectedValue.IsEmpty());
                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + expectedValue.IsEmpty() + " -Actual: " + actualValue.IsEmpty());
            }
            return s_compareResult;
        }
   
        //------------------------------------------------------------
        public static bool SetPoint(object o, PropertyInfo p, ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                s_tstNumber++;
                GlobalLog.LogStatus("---SetPoint---" + p.Name);
         
                //----SET THE VALUE
                p.SetValue(o, goodInput.Key, null);

                //----GET THE VALUE
                System.Windows.Point actualValue = (System.Windows.Point)p.GetValue(o, null);

                //----COMPARE VALUES
                s_compareResult &= (actualValue == (Point)goodInput.Value);
                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + goodInput.Value + " -Actual: " + actualValue);
            }
            return s_compareResult;
        }

        //--------------------------------------------------------------------------------------
        public static bool SetQuaternion(object o, PropertyInfo p, ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                s_tstNumber++;
                GlobalLog.LogStatus("---SetQuaternion---" + p.Name);
                
                //----SET THE VALUE
                p.SetValue(o, goodInput.Key, null);

                //----GET THE VALUE
                Quaternion actualValue = (Quaternion)p.GetValue(o, null);

                //----COMPARE VALUES
                s_compareResult &= (actualValue.Angle == ((Quaternion)goodInput.Value).Angle && actualValue.Axis == ((Quaternion)goodInput.Value).Axis);

                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + ((Quaternion)goodInput.Value).Axis + " -Actual: " + actualValue);
            }
            return s_compareResult;
        }

        //--------------------------------------------------------------------------------------
        public static bool SetRotation3D(object o, PropertyInfo p, ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                s_tstNumber++;
                GlobalLog.LogStatus("---SetRotation3D---" + p.Name);
                
                //----SET THE VALUE
                p.SetValue(o, goodInput.Key, null);

                //----GET THE VALUE
                AxisAngleRotation3D actualValue = (AxisAngleRotation3D)p.GetValue(o, null);

                //----COMPARE VALUES
                s_compareResult &= (actualValue.Angle == ((AxisAngleRotation3D)goodInput.Value).Angle && actualValue.Axis == ((AxisAngleRotation3D)goodInput.Value).Axis);

                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + ((AxisAngleRotation3D)goodInput.Value).Axis + " -Actual: " + actualValue);
            }
            return s_compareResult;
        }
        //------ timing ----------
        //--------------------------------------------------------------------------------------
        public static bool SetBadAccelDecel(double assignedValueA, double assignedValueD, string expectedException)
        {
            bool messageResult = false;
            //The test will pass only if it produces an exception.
            GlobalLog.LogStatus("--------------------------------------------");
            
            s_tstNumber++;

            ParallelTimeline PTL = new ParallelTimeline();

            try
            {
                PTL.AccelerationRatio = assignedValueA;
                PTL.DecelerationRatio = assignedValueD;
                PTL.Freeze();  //Accel+Decel checked only when the Timeline is frozen, as of 10-25-05.
                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: False (No Exception found)");
            }
            catch (Exception e2)
            {
                messageResult = HandleException(s_tstNumber, e2, expectedException);
            }

            return messageResult;
        }
        //--------------------------------------------------------------------------------------
        public static bool SetBadGeneric(object o, PropertyInfo p, ListDictionary badValues)
        {
            bool messageResult = true;
            foreach (DictionaryEntry badInput in badValues)
            {
                s_tstNumber++;
                //The test will pass only if it produces an exception.
                try
                {
                    p.SetValue(o, badInput.Key, null);
                    GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: False (No Exception found)");
                    messageResult &= false;
                }
                catch (Exception e1)
                {
                    messageResult &= HandleException(s_tstNumber, e1, (string)badInput.Value);
                }

            }
            return messageResult;
        }
        //--------------------------------------------------------------------------------------
        public static bool SetAccelDecel(double assignedValueA, double assignedValueD)
        {
            //GlobalLog.LogStatus("---SetAccelDecel---" + p.Name);
            GlobalLog.LogEvidence("--------------------------------------------");
            
            s_tstNumber++;

            ParallelTimeline PTL = new ParallelTimeline();
            PTL.AccelerationRatio = assignedValueA;
            PTL.DecelerationRatio = assignedValueD;

            double actualValueA = PTL.AccelerationRatio;
            double actualValueD = PTL.DecelerationRatio;

            bool compareResultA = (assignedValueA == actualValueA);
            bool compareResultD = (assignedValueD == actualValueD);
            GlobalLog.LogEvidence(FormatNum(s_tstNumber) + "a -RESULT: " + compareResultA + "-Expected: " + assignedValueA + " -Actual: " + actualValueA);
            GlobalLog.LogEvidence(FormatNum(s_tstNumber) + "b -RESULT: " + compareResultD + "-Expected: " + assignedValueD + " -Actual: " + actualValueD);

            return (compareResultA && compareResultD);
        }
        //--------------------------------------------------------------------------------------
        public static bool SetDoubleNaN(object o, PropertyInfo p, double assignedValue)
        {
            //GlobalLog.LogStatus("---SetDoubleNaN---" + p.Name);
            GlobalLog.LogEvidence("--------------------------------------------");
            
            s_tstNumber++;

            //----SET THE VALUE
            p.SetValue(o, assignedValue, null);

            //----GET THE VALUE
            double actualValue = (double)p.GetValue(o, null);
            bool isNaNResult = Double.IsNaN(actualValue);

            GlobalLog.LogEvidence(FormatNum(s_tstNumber) + "RESULT: " + isNaNResult + " (for NaN assigned)");

            return isNaNResult;
        }
        //--------------------------------------------------------------------------------------
        public static bool SetNullableTimeSpan(PropertyInfo p, ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                GlobalLog.LogStatus("---SetNullableTimeSpan---" + p.Name);
                GlobalLog.LogEvidence("--------------------------------------------");
                s_tstNumber++;
                ParallelTimeline PTL = new ParallelTimeline();
                Nullable<TimeSpan> actualValue = null;

                switch (p.Name)
                {
                    case "BeginTime":
                    {
                        PTL.BeginTime =(Nullable < TimeSpan >) goodInput.Key;
                        actualValue = PTL.BeginTime;

                        break;
                    }
                    default:
                    {
                        break;
                    }
                }

                s_compareResult &= ((Nullable<TimeSpan>)goodInput.Value == actualValue);
                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + (Nullable<TimeSpan>)goodInput.Value + " -Actual: " + actualValue);
            }
            return s_compareResult;
        }
        //------------------------------------------------
        public static bool SetTimeSpan(PropertyInfo p, ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                s_tstNumber++;
                GlobalLog.LogStatus("---SetTimeSpan---" + p.Name);
                GlobalLog.LogEvidence("--------------------------------------------");

                ParallelTimeline PTL = new ParallelTimeline();
                string actualValue = "";

                switch (p.Name)
                {
                    case "Duration":
                    {
                        PTL.Duration = new Duration((TimeSpan)goodInput.Key);
                        actualValue = PTL.Duration.ToString();
                        break;
                    }
                    case "RepeatBehavior":
                    {
                        PTL.RepeatBehavior = new RepeatBehavior((TimeSpan)goodInput.Key);
                        actualValue = PTL.RepeatBehavior.ToString();
                        break;
                    }
                    default:
                    {
                        if (p.DeclaringType.ToString() == "System.Windows.Media.Animation.SeekStoryboard")
                        {
                            SeekStoryboard s = new SeekStoryboard();
                            p.SetValue(s, goodInput.Key, null);
                            actualValue = s.Offset.ToString();
                        }
                        break;
                    }
                }

                s_compareResult &= (goodInput.Value.ToString() == actualValue);
                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + goodInput.Value.ToString() + " -Actual: " + actualValue + " -Result: " + s_compareResult);
            }
            return s_compareResult;
        }
        //--------------------------------------------------------------------------------------
        public static bool SetBadTimeSpan(PropertyInfo p, ListDictionary badValues)
        {
            //The test will pass only if it produces an exception.
            bool messageResult = true;
            foreach (DictionaryEntry badInput in badValues)
            {
                s_tstNumber++;
                GlobalLog.LogEvidence("--------------------------------------------");
                ParallelTimeline PTL = new ParallelTimeline();

                try
                {
                    switch (p.Name)
                    {
                        case "Duration":
                            {
                                PTL.Duration = new Duration((TimeSpan)badInput.Key);
                                break;
                            }
                        case "RepeatBehavior":
                            {
                                PTL.RepeatBehavior = new RepeatBehavior((TimeSpan)badInput.Key);
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                    GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: False (No Exception found)");
                }
                catch (Exception e)
                {
                    messageResult &= HandleException(s_tstNumber, e, (string)badInput.Value);
                }
            }
            return messageResult;
        }
        //--------------------------------------------------------------------------------------
        public static bool SetBadKeyTime(PropertyInfo p, ListDictionary badValues)
        {
            //The test will pass only if it produces an exception.
            bool messageResult = true;
            foreach (DictionaryEntry badInput in badValues)
            {
                s_tstNumber++;
                GlobalLog.LogEvidence("--------------------------------------------");
                KeyTime keyTime = new KeyTime();

                try
                {
                    keyTime = KeyTime.FromTimeSpan((TimeSpan)badInput.Key);
                    GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: False (No Exception found)");
                }
                catch (Exception e)
                {
                    messageResult &= HandleException(s_tstNumber, e, (string)badInput.Value);
                }
            }
            return messageResult;
        }
        //--------------------------------------------------------------------------------------
        public static bool SetEnum(object o, PropertyInfo p, ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                s_tstNumber++;
                
                //----SET THE VALUE
                p.SetValue(o, goodInput.Key, null);

                //----GET THE VALUE
                object actualValue = p.GetValue(o, null);

                //----COMPARE VALUES
                s_compareResult &= (actualValue.ToString() == goodInput.Value.ToString());
                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + goodInput.Value.ToString() + " -Actual: " + actualValue.ToString());
            }
            return s_compareResult;
        }
        //--------------------------------------------------------------------------------------
        public static bool SetFillBehavior(ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                s_tstNumber++;
                
                //GlobalLog.LogStatus("---SetFillBehavior---");
                GlobalLog.LogEvidence("--------------------------------------------");

                ParallelTimeline PTL = new ParallelTimeline();
                PTL.FillBehavior = (FillBehavior)goodInput.Key;

                FillBehavior actualValue = PTL.FillBehavior;

                s_compareResult &= ((FillBehavior)goodInput.Value == actualValue);
                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + (FillBehavior)goodInput.Value + " -Actual: " + actualValue);
            }
            return s_compareResult;
        }
        public static bool SetHandoffBehavior(ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                s_tstNumber++;
                //GlobalLog.LogStatus("---SetHandoffBehavior---");
                GlobalLog.LogEvidence("--------------------------------------------");

                BeginStoryboard PTL = new BeginStoryboard();
                PTL.HandoffBehavior = (HandoffBehavior)goodInput.Key;

                HandoffBehavior actualValue = PTL.HandoffBehavior;

                s_compareResult &= ((HandoffBehavior)goodInput.Value == actualValue);
                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + (HandoffBehavior)goodInput.Value + " -Actual: " + actualValue);
            }
            return s_compareResult;
        }
        //--------------------------------------------------------------------------------------
        public static bool SetKeySpline(object o, PropertyInfo p, ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                s_tstNumber++;
                GlobalLog.LogStatus("---SetKeySpline---" + p.Name);
                GlobalLog.LogEvidence("--------------------------------------------");

                //----SET THE VALUE
                p.SetValue(o, goodInput.Key, null);

                //----GET THE VALUE
                KeySpline actualValue = (KeySpline)p.GetValue(o, null);
                s_compareResult &= ((KeySpline)goodInput.Value == actualValue);

                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + (KeySpline)goodInput.Value + " -Actual: " + actualValue);
   
            }
            return s_compareResult;
        }
        //--------------------------------------------------------------------------------------
        public static bool SetKeyTime(object o, PropertyInfo p, ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                s_tstNumber++;
                GlobalLog.LogStatus("---SetKeyTime---" + p.Name);
                GlobalLog.LogEvidence("--------------------------------------------");

                //----SET THE VALUE
                p.SetValue(o, KeyTime.FromTimeSpan((TimeSpan)goodInput.Key), null);

                //----GET THE VALUE
                KeyTime actualValue = (KeyTime)p.GetValue(o, null);;
                s_compareResult &= ((TimeSpan)goodInput.Value == actualValue.TimeSpan);

                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + (TimeSpan)goodInput.Value+ " -Actual: " + actualValue.TimeSpan);
   
            }
            return s_compareResult;
        }
        //--------------------------------------------------------------------------------------
        public static bool SetDiscreteObjectKeyFrame(object o, PropertyInfo p, ListDictionary goodValues)
        {
            foreach (DictionaryEntry goodInput in goodValues)
            {
                s_tstNumber++;
                GlobalLog.LogStatus("---SetDiscreteObjectKeyFrame---" + p.Name);
                GlobalLog.LogEvidence("--------------------------------------------");

                //----SET THE VALUE
                p.SetValue(o, goodInput.Key, null);

                //----GET THE VALUE
                DiscreteObjectKeyFrame actualValue = (DiscreteObjectKeyFrame)p.GetValue(o, null);
                s_compareResult &= ((DiscreteObjectKeyFrame)goodInput.Value == actualValue);

                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + s_compareResult + "-Expected: " + (DiscreteObjectKeyFrame)goodInput.Value + " -Actual: " + actualValue);
   
            }
            return s_compareResult;
        }
        //--------------------------------------------------------------------------------------
        public static bool SetBadFillBehavior(ListDictionary badValues)
        {
            bool messageResult = true;
            foreach (DictionaryEntry badInput in badValues)
            {
                s_tstNumber++;
                
                //The test will pass only if it produces an exception.
                GlobalLog.LogEvidence("--------------------------------------------");

                ParallelTimeline PTL = new ParallelTimeline();

                try
                {
                    PTL.FillBehavior = (FillBehavior)badInput.Key;
                    GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: False (No Exception found)");
                }
                catch (Exception e)
                {
                    messageResult &= HandleException(s_tstNumber, e, (string)badInput.Value);
                }
            }
            return messageResult;
        }
        //--------------------------------------------------------------------------------------
        public static bool SetRepeatBehaviorDouble(object assignedValue, object expectedValue)
        {
            //GlobalLog.LogStatus("---SetRepeatBehaviorDouble---");
            GlobalLog.LogEvidence("--------------------------------------------");
            
            s_tstNumber++;

            ParallelTimeline PTL = new ParallelTimeline();
            PTL.RepeatBehavior = new RepeatBehavior((double)assignedValue);

            string actualValue = PTL.RepeatBehavior.ToString();

            bool compareResult = ((string)expectedValue == actualValue);
            GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: " + compareResult + "-Expected: " + expectedValue + " -Actual: " + actualValue);

            return compareResult;
        }
        //--------------------------------------------------------------------------------------
        public static bool SetBadRepeatBehaviorDouble(double assignedValue, string expectedException)
        {
            //The test will pass only if it produces an exception.
            GlobalLog.LogEvidence("--------------------------------------------");
            
            s_tstNumber++;

            bool messageResult = false;
            ParallelTimeline PTL = new ParallelTimeline();

            try
            {
                //----SET THE VALUE
                PTL.RepeatBehavior = new RepeatBehavior(assignedValue);
                GlobalLog.LogEvidence(FormatNum(s_tstNumber) + " -RESULT: False (No Exception found)");
            }
            catch (Exception e3)
            {
                messageResult = HandleException(s_tstNumber, e3, expectedException);
            }

            return messageResult;
        } 
     
        //--------------------------------------------------------------------
        public static bool CompareGenericValues(PropertyInfo p, object expValue, object actValue)
        {
            switch (p.PropertyType.ToString())
            {
                case "System.Boolean":
                    {
                        if (expValue == null)
                        {
                            return (actValue == null);
                        }
                        else
                        {
                            return (((bool)expValue) == ((bool)actValue));
                        }
                    }
                case "System.Byte":
                    {
                        if (expValue == null)
                        {
                            return (actValue == null);
                        }
                        else
                        {
                            return (((byte)expValue) == ((byte)actValue));
                        }
                    }
                case "System.Char":
                    {
                        if (expValue == null)
                        {
                            return (actValue == null);
                        }
                        else
                        {
                            return (((char)expValue) == ((char)actValue));
                        }
                    }
                case "System.Windows.Media.Color":
                    {
                        if (expValue == null)
                        {
                            return (actValue == null);
                        }
                        else
                        {
                            return (((System.Windows.Media.Color)expValue) == ((System.Windows.Media.Color)actValue));
                        }
                    }
                case "System.Decimal":
                    {
                        if (expValue == null)
                        {
                            return (actValue == null);
                        }
                        else
                        {
                            return (((decimal)expValue) == ((decimal)actValue));
                        }
                    }
                case "System.Double":
                    {
                        if (expValue == null)
                        {
                            return (actValue == null);
                        }
                        else
                        {
                            return (((double)expValue) == ((double)actValue));
                        }
                    }
                case "System.Int16":
                    {
                        if (expValue == null)
                        {
                            return (actValue == null);
                        }
                        else
                        {
                            return (((Int16)expValue) == ((Int16)actValue));
                        }
                    }
                case "System.Int32":
                    {
                        if (expValue == null)
                        {
                            return (actValue == null);
                        }
                        else
                        {
                            return (((Int32)expValue) == ((Int32)actValue));
                        }
                    }
                case "System.Int64":
                    {
                        if (expValue == null)
                        {
                            return (actValue == null);
                        }
                        else
                        {
                            return (((Int64)expValue) == ((Int64)actValue));
                        }
                    }
                case "System.Windows.Media.Matrix":
                    {
                        if (expValue == null)
                        {
                            return (actValue == null);
                        }
                        else
                        {
                            return (((Matrix)expValue) == ((Matrix)actValue));
                        }
                    }
                case "System.Windows.Media.Animation.IEasingFunction":
                    {
                        if (expValue == null)
                        {
                            return (actValue == null);
                        }
                        else
                        {
                            return expValue.GetType().FullName == actValue.GetType().FullName;
                        }
                    }
                case "System.Object":
                    {
                        if (expValue == null)
                        {
                            return (actValue == null);
                        }
                        else
                        {
                            return (((object)expValue) == ((object)actValue));
                        }
                    }
                case "System.Windows.Media.Point":
                    {
                        if (expValue == null)
                        {
                            return (actValue == null);
                        }
                        else
                        {
                            return (((Point)expValue) == ((Point)actValue));
                        }
                    }
                case "System.Windows.Media.Media3D.Point3D":
                    {
                        if (expValue == null)
                        {
                            return (actValue == null);
                        }
                        else
                        {
                            return (((Point3D)expValue) == ((Point3D)actValue));
                        }
                    }
                case "System.Windows.Media.Media3D.Quaternion":
                    {
                        if (expValue == null)
                        {
                            return (actValue == null);
                        }
                        else
                        {
                            return (((Quaternion)expValue) == ((Quaternion)actValue));
                        }
                    }
                case "System.Windows.Rect":
                    {
                        if (expValue == null)
                        {
                            return (actValue == null);
                        }
                        else
                        {
                            return (((Rect)expValue) == ((Rect)actValue));
                        }
                    }
                case "System.Windows.Media.Media3D.Rotation3D":
                    {
                        if (expValue == null)
                        {
                            return (actValue == null);
                        }
                        else
                        {
                            return (((Rotation3D)expValue) == ((Rotation3D)actValue));
                        }
                    }
                case "System.Windows.Media.Animation.PathAnimationSource":
                    {
                        if (expValue == null)
                        {
                            return (actValue == null);
                        }
                        else
                        {
                            return (((PathAnimationSource)expValue) == ((PathAnimationSource)actValue));
                        }
                    }
                case "System.Windows.Media.PathGeometry":
                    {
                        if (expValue == null)
                        {
                            return (actValue == null);
                        }
                        else
                        {
                            return (((PathGeometry)expValue) == ((PathGeometry)actValue));
                        }
                    }
                case "System.Single":
                    {
                        if (expValue == null)
                        {
                            return (actValue == null);
                        }
                        else
                        {
                            return (((Single)expValue) == ((Single)actValue));
                        }
                    }
                case "System.Windows.Size":
                    {
                        if (expValue == null)
                        {
                            return (actValue == null);
                        }
                        else
                        {
                            return (((Size)expValue) == ((Size)actValue));
                        }
                    }
                case "System.String":
                    {
                        if (expValue == null)
                        {
                            return (actValue == null);
                        }
                        else
                        {
                            return ( (string)expValue == (string)actValue );
                        }
                    }
                case "System.Windows.Thickness":
                    {
                        if (expValue == null)
                        {
                            return (actValue == null);
                        }
                        else
                        {
                            return (((Thickness)expValue) == ((Thickness)actValue));
                        }
                    }
                case "System.Windows.Media.Media3D.Vector3D":
                    {
                        if (expValue == null)
                        {
                            return (actValue == null);
                        }
                        else
                        {
                            return (((Vector3D)expValue) == ((Vector3D)actValue));
                        }
                    }
                case "System.Windows.Vector":
                    {
                        if (expValue == null)
                        {
                            return (actValue == null);
                        }
                        else
                        {
                            return (((Vector)expValue) == ((Vector)actValue));
                        }
                    }
                default:
                    {
                        if (p.PropertyType.ToString().EndsWith("Collection"))
                        {
                            return expValue.Equals(actValue);
                        }
                        else if (p.PropertyType.ToString() == "System.Windows.Media.Animation.Storyboard"
                                ||p.PropertyType.ToString() == "System.Windows.Media.Animation.TimeSeekOrigin")
                        {
                            return expValue.Equals(actValue);
                        }
                        else
                        {

                            GlobalLog.LogEvidence("!!!ERROR -- Property Type not found in CompareGenericValues!!!");
                            return false;
                        }
                    }
            }
        }
        //--------------------------------------------------------------------------------------
        public static bool HandleException (int tstNumber, Exception actException, string expExceptionString )
        {
            bool result = false;
            
            Exception expException = new Exception(expExceptionString);
            
            result = (expException.GetType() == actException.GetType() || actException.GetType().IsSubclassOf(expException.GetType()));
            
            if (result)
            {
                GlobalLog.LogEvidence(FormatNum(tstNumber) + " -RESULT: " + result + "(" + actException.GetType() + ")");
            }
            else
            {
                GlobalLog.LogEvidence(FormatNum(tstNumber) + " -RESULT: " + result);
                GlobalLog.LogEvidence("\n-ACTUAL:   " + actException.GetType());
                GlobalLog.LogEvidence("\n-EXPECTED: " + expException.GetType());
            }
            
            return result;
        }
        
        //--------------------------------------------------------------------------------------
        public static string FormatNum (int num)
        {
            string tstNumber = "(" + num.ToString() + ")";
            return tstNumber;
        }
        
        #endregion
        #region Members
        private static bool s_compareResult = true;
        private static int s_tstNumber = 0;
        #endregion

    }
}
