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
    public class                AnimationTests 
    {
  
        //------------------------------------------------------------------------------------------
        public static bool TestProperty ( object o, PropertyInfo p, ListDictionary goodValues, ListDictionary badValues )
        {
            bool result                 = true;
            string invalidException1    = "System.InvalidOperationException";
            string invalidException2    = "System.ArgumentOutOfRangeException";
            string invalidException3    = "System.ArgumentException";
            //string invalidException4    = "System.Reflection.TargetInvocationException";
            
            switch (p.PropertyType.ToString())
            {
                case "System.Boolean":  
                case "System.Double":
                {
                    if (((p.Name == "AccelerationRatio" || p.Name == "DecelerationRatio") && p.DeclaringType.ToString() == "System.Windows.Media.Animation.Timeline")
                        || (p.Name == "SpeedRatio") && p.DeclaringType.ToString() == "System.Windows.Media.Animation.ClockController") 
                    {
                        //AccelerationRatio value must be between 0 and 1, inclusive.
                        // Cannot make call common test
                        //These tests set both Accelaration and Deceleration on the same ParallelTimeline.
                        result &= TestSets.SetAccelDecel(1d, 0d);
                        result &= TestSets.SetAccelDecel(0d, 1d);
                        result &= TestSets.SetAccelDecel(0d, 0d);

                        result &= TestSets.SetBadAccelDecel(1d, 1d, invalidException1);
                        result &= TestSets.SetBadAccelDecel(1.000000000000001, 0d, invalidException3);
                        result &= TestSets.SetBadAccelDecel(-0.000000000000001, 1d, invalidException3);
                        result &= TestSets.SetBadAccelDecel(0d, 1.000000000000001, invalidException3);
                        result &= TestSets.SetBadAccelDecel(1d, -0.000000000000001, invalidException3);
                        result &= TestSets.SetBadAccelDecel(0d, Double.MaxValue, invalidException3);
                        result &= TestSets.SetBadAccelDecel(0d, Double.MinValue, invalidException3);
                        result &= TestSets.SetBadAccelDecel(1d, Double.MaxValue, invalidException3);
                        result &= TestSets.SetBadAccelDecel(1d, Double.MinValue, invalidException3);
                        result &= TestSets.SetBadAccelDecel(Double.MaxValue, 0d, invalidException3);
                        result &= TestSets.SetBadAccelDecel(Double.MinValue, 0d, invalidException3);
                        result &= TestSets.SetBadAccelDecel(Double.MaxValue, 1d, invalidException3);
                        result &= TestSets.SetBadAccelDecel(Double.MinValue, 1d, invalidException3);
                    }
                    else if (p.Name == "SpeedRatio" && p.DeclaringType.ToString() == "System.Windows.Media.Animation.Timeline")
                    {
                        //SpeedRatio value must be greater than 0.
                        result &= TestSets.SetGeneric(o, p, goodValues);
                        result &= TestSets.SetBadGeneric(o, p, badValues);
                    }
                    else
                    {
                        result &= TestSets.SetGeneric(o, p, goodValues);
                    }
                    break;
                }
                case "System.Nullable`1[System.Byte]":
                {
                    result &= TestSets.SetNullableByte(o, p, goodValues);
                    break;
                }
                case "System.Nullable`1[System.Windows.Media.Color]" :
                {
                    result &= TestSets.SetNullableColor(o, p, goodValues);
                    break;
                }
                case "System.Nullable`1[System.Decimal]" :
                {
                    result &= TestSets.SetNullableDecimal(o, p, goodValues);
                    break;
                }
                case "System.Nullable`1[System.Double]" :
                {
                    result &= TestSets.SetNullableDouble(o, p, goodValues);
                    result &= TestSets.SetBadNullable(o, p, badValues);
                    break;
                }
                case "System.Nullable`1[System.Int16]" :
                {
                    result &= TestSets.SetNullableInt16(o, p, goodValues);
                    break;
                }
                case "System.Nullable`1[System.Int32]" :
                {
                    result &= TestSets.SetNullableInt32(o, p, goodValues);
                    break;
                }
                case "System.Nullable`1[System.Int64]" :
                {
                    result &= TestSets.SetNullableInt64(o, p, goodValues);
                    break;
                }
                case "System.Nullable`1[System.Windows.Point]":
                {
                    result &= TestSets.SetNullablePoint(o, p, goodValues);
                    result &= TestSets.SetBadNullable(o, p, badValues);
                    break;
                }
                case "System.Nullable`1[System.Windows.Media.Media3D.Point3D]":
                {
                    result &= TestSets.SetNullablePoint3D(o, p, goodValues);
                    result &= TestSets.SetBadNullable(o, p, badValues);
                    break;
                }

                case "System.Nullable`1[System.Windows.Rect]" :
                {
                    result &= TestSets.SetNullableRect(o, p, goodValues);
                    result &= TestSets.SetBadNullable(o, p, badValues);
                    break;
                }
                case "System.Nullable`1[System.Windows.Media.Media3D.Quaternion]" :
                {
                    result &= TestSets.SetQuaternion(o, p, goodValues);
                    break;
                }
                case "System.Windows.Media.Media3D.Rotation3D" :
                {
                    result &= TestSets.SetRotation3D(o, p, goodValues);
                    result &= TestSets.SetBadRotation3D(o, p, badValues);
                    break;
                }
                case "System.Nullable`1[System.Single]" :
                {
                    result &= TestSets.SetNullableSingle(o, p, goodValues );
                    result &= TestSets.SetBadNullable(o, p, badValues);
                    break;
                }
                case "System.Nullable`1[System.TimeSpan]":
                {
                    result &= TestSets.SetNullableTimeSpan(p, goodValues);
                    break;
                }
                case "System.Nullable`1[System.Windows.Media.Media3D.Size3D]":
                {
                    result &= TestSets.SetNullableSize3D(o, p, goodValues);
                    result &=  TestSets.SetBadNullable(o, p, badValues);
                    break;
                }
                case "System.Nullable`1[System.Windows.Size]" :
                {
                    result &= TestSets.SetNullableSize(o, p, goodValues);
                    result &= TestSets.SetBadNullable(o, p, badValues);
                    break;
                }
                case "System.Nullable`1[System.Windows.Media.Media3D.Vector3D]" :
                {
                    result &= TestSets.SetNullableVector3D(o, p, goodValues);
                    result &= TestSets.SetBadNullable(o, p, badValues);
                    break;
                }
                case "System.Nullable`1[System.Windows.Thickness]":
                {
                    result &= TestSets.SetNullableThickness(o, p, goodValues);
                    result &= TestSets.SetBadNullable(o, p, badValues);
                    break;
                }
                case "System.Nullable`1[System.Windows.Vector]":
                {
                    result &= TestSets.SetNullableVector(o, p, goodValues);
                    result &= TestSets.SetBadNullable(o, p, badValues);
                    break;
                }
                case "System.Windows.Point":
                {
                    result &= TestSets.SetPoint(o, p, goodValues);
                    result &= TestSets.SetBadGeneric(o, p, badValues);
                    break;
                }
                case "System.String":
                {
                    result &= TestSets.SetGeneric(o, p, goodValues);
                    if (p.Name == "Name" && p.DeclaringType.ToString() == "System.Windows.Media.Animation.Timeline")
                    {
                        result &= TestSets.SetBadGeneric(o, p, badValues);
                    }
                    break;
                }
                case "System.TimeSpan":
                {
                    result &= TestSets.SetTimeSpan(p, goodValues);
                    break;
                }
                case "System.Windows.Duration":
                {
                    result &= TestSets.SetTimeSpan(p, goodValues);
                    result &= TestSets.SetBadTimeSpan(p, badValues);
                    break;
                }
                case "System.Windows.Media.Animation.FillBehavior":
                {
                    result &= TestSets.SetEnum(o, p, goodValues);
                    result &= TestSets.SetBadFillBehavior(badValues);
                    break;
                }
                case "System.Windows.Media.Animation.HandoffBehavior":
                {
                    result &= TestSets.SetHandoffBehavior(goodValues);
                    //The error string will be determined in SetBadHandoffBehavior.
                   // result &= TestSets.SetBadFillBehavior(badValues);
                    break;
                }
                case "System.Windows.Media.Animation.KeySpline":
                {
                    result &= TestSets.SetKeySpline(o, p, goodValues);
                    break;
                }
                case "System.Windows.Media.Animation.KeyTime":
                {
                    result &= TestSets.SetKeyTime(o, p, goodValues);
                    result &= TestSets.SetBadKeyTime(p, badValues);
                    break;
                }
                case "System.Windows.Media.Animation.RepeatBehavior":
                {
                    //Duration.
                    result &= TestSets.SetTimeSpan(p, goodValues);
                    result &= TestSets.SetBadTimeSpan(p, badValues);
                    
                    result &= TestSets.SetRepeatBehaviorDouble(1d, "1x");
                    result &= TestSets.SetRepeatBehaviorDouble(0d, "0x");
                    result &= TestSets.SetRepeatBehaviorDouble(2d, "2x");

                    result &= TestSets.SetRepeatBehaviorDouble(2d, "2x");
                    result &= TestSets.SetRepeatBehaviorDouble(Double.Epsilon, Double.Epsilon.ToString() + "x");
                    result &= TestSets.SetRepeatBehaviorDouble(Double.MaxValue, Double.MaxValue.ToString() + "x");

                    result &= TestSets.SetBadRepeatBehaviorDouble(Double.MinValue, invalidException2);
                    result &= TestSets.SetBadRepeatBehaviorDouble(Double.PositiveInfinity, invalidException2);
                    result &= TestSets.SetBadRepeatBehaviorDouble(Double.NegativeInfinity, invalidException2);
             
                    break;
                }
                case "System.Windows.Media.Animation.SlipBehavior":
                {
                    result &= TestSets.SetEnum(o, p, goodValues);
                    break;
                }
                case "System.Windows.Media.PathGeometry":
                {
                    result &= TestSets.SetPathGeometry(o, p, goodValues);
                    break;
                }
                default:
                {
                    result &= TestSets.SetGeneric(o, p, goodValues);
                    break;
                }
            }
            return result;
        }
    }
}
