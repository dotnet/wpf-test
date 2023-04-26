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
using System.Collections.Generic;
using System.Collections.Specialized;

using Microsoft.Test.Animation.ObjectPatterns;

//---------------------------------------------------------------------------------------------

namespace                       Microsoft.Test.Animation
{
    //------------------------------------------------------------------------------------------
    public class DataTypeFactory
    {
        public static void GetData(PropertyInfo p, out ListDictionary goodValues, out ListDictionary badValues)
        {
            // ListDictionary.Key is the input value
            // ListDictionary.Value is the expected value
            goodValues = new ListDictionary();
            badValues = new ListDictionary();
                        
            switch (p.PropertyType.ToString())
            {
                // ---------------------- ANIMATION DATA -----------------------
                case "System.Boolean":
                {
                    goodValues.Add(true, true);
                    goodValues.Add(false, false);
                    break;
                }
                case "System.Nullable`1[System.Byte]":
                {
                    goodValues.Add(Byte.MinValue, Byte.MinValue);
                    goodValues.Add(Byte.MaxValue, Byte.MaxValue);
                    break;
                }
                case "System.Nullable`1[System.Windows.Media.Color]":
                {
                    goodValues.Add((Nullable<Color>)Colors.Blue, (Nullable<Color>)Colors.Blue);
                    break;
                }
       
                case "System.Nullable`1[System.Decimal]":
                {
                    goodValues.Add(Decimal.Zero, Decimal.Zero);
                    goodValues.Add(Decimal.MinValue, Decimal.MinValue);
                    goodValues.Add(Decimal.MaxValue, Decimal.MaxValue);
                    goodValues.Add(Decimal.MinusOne, Decimal.MinusOne);
                    break;
                }
                case "System.Nullable`1[System.Double]":
                {
                    goodValues.Add(Double.Epsilon, Double.Epsilon);
                    goodValues.Add(Double.MinValue, Double.MinValue);
                    goodValues.Add(Double.MaxValue, Double.MaxValue);

                    badValues.Add(Double.NegativeInfinity, s_invalidException1);
                    badValues.Add(Double.PositiveInfinity, s_invalidException1 );
                    badValues.Add(Double.NaN, s_invalidException1);
                    break;
                }
                case "System.Nullable`1[System.Int16]":
                {
                    goodValues.Add((Int16)0, (Int16)0);
                    goodValues.Add(Int16.MaxValue, Int16.MaxValue);
                    goodValues.Add(Int16.MinValue, Int16.MinValue);
                    break;
                }
                case "System.Nullable`1[System.Int32]":
                {
                    goodValues.Add((Int32)0, (Int32)0);
                    goodValues.Add(Int32.MaxValue, Int32.MaxValue);
                    goodValues.Add(Int32.MinValue, Int32.MinValue);
                    break;
                }
                case "System.Nullable`1[System.Int64]":
                {
                    goodValues.Add((Int64)0, (Int64)0);
                    goodValues.Add(Int64.MaxValue, Int64.MaxValue);
                    goodValues.Add(Int64.MinValue, Int64.MinValue);
                    break;
                }
                case "System.Nullable`1[System.Windows.Point]":
                {
                    goodValues.Add(new Point(), new Point());
                    //goodValue.Add(new Point(0, 0), new Point(0, 0));
                    goodValues.Add(new Point(Double.MaxValue, Double.MaxValue), new Point(Double.MaxValue, Double.MaxValue));
                    goodValues.Add(new Point(Double.MinValue, Double.MinValue), new Point(Double.MinValue, Double.MinValue));
                    goodValues.Add(new Point(Double.Epsilon, Double.Epsilon), new Point(Double.Epsilon, Double.Epsilon));
                 
                    badValues.Add(new Point(Double.PositiveInfinity, Double.PositiveInfinity), s_invalidException1);
                    badValues.Add(new Point(Double.NaN, Double.NaN), s_invalidException1);
                    break;
                
                }
                case "System.Nullable`1[System.Windows.Media.Media3D.Point3D]" :
                {
                    goodValues.Add(new Point3D(), new Point3D() );
                    goodValues.Add(new Point3D(Double.MaxValue, Double.MaxValue, Double.MaxValue), new Point3D(Double.MaxValue, Double.MaxValue, Double.MaxValue) );
                    goodValues.Add(new Point3D(Double.MinValue, Double.MinValue, Double.MinValue), new Point3D(Double.MinValue, Double.MinValue, Double.MinValue) );
                    goodValues.Add(new Point3D(Double.Epsilon, Double.Epsilon, Double.Epsilon), new Point3D(Double.Epsilon, Double.Epsilon, Double.Epsilon));

                    badValues.Add(new Point3D(Double.PositiveInfinity, Double.PositiveInfinity, Double.PositiveInfinity), s_invalidException1);
                    badValues.Add(new Point3D(Double.NaN,Double.NaN,Double.NaN), s_invalidException1 );
                    break;
                }
                case "System.Nullable`1[System.Windows.Rect]":
                {
                    goodValues.Add(new Rect(), new Rect());
                    goodValues.Add(new Rect(Double.MaxValue, Double.MaxValue, Double.MaxValue, Double.MaxValue), new Rect(Double.MaxValue, Double.MaxValue, Double.MaxValue, Double.MaxValue));
                    goodValues.Add(new Rect(Double.Epsilon, Double.Epsilon, Double.Epsilon, Double.Epsilon), new Rect(Double.Epsilon, Double.Epsilon, Double.Epsilon, Double.Epsilon));

                    badValues.Add(new Rect(Double.PositiveInfinity, Double.PositiveInfinity, Double.PositiveInfinity, Double.PositiveInfinity), s_invalidException1);
                    badValues.Add(new Rect(Double.NaN, Double.NaN, Double.NaN, Double.NaN), s_invalidException1);
                    break;
                }
                case "System.Nullable`1[System.Single]":
                {
                    goodValues.Add(Single.Epsilon, Single.Epsilon);
                    goodValues.Add(Single.MinValue, Single.MinValue);
                    goodValues.Add(Single.MaxValue, Single.MaxValue);

                    badValues.Add(Single.NegativeInfinity, s_invalidException1);
                    badValues.Add(Single.PositiveInfinity, s_invalidException1);
                    badValues.Add(Single.NaN, s_invalidException1);
                    break;
                }
                case "System.Nullable`1[System.Windows.Media.Media3D.Quaternion]":
                {
                    Quaternion q = new Quaternion();
                    goodValues.Add(q, q);
                    
                    q = new Quaternion(new Vector3D(20d, 30d, 40d), 1d);
                    goodValues.Add(q, q);
                    
                    q = new Quaternion(new Vector3D(-21d, -30d, -40d), -50d);
                    goodValues.Add(q, q);
                    
                    q = new Quaternion(new Vector3D(-20d, Double.Epsilon, -40d), Double.MaxValue);
                    goodValues.Add(q, q);
                    
                    q = new Quaternion(new Vector3D(-20d, -30d, -41d), Double.MaxValue);
                    goodValues.Add(q, q);
                    
                    q = new Quaternion(new Vector3D(Double.MaxValue, Double.MaxValue, Double.MaxValue), Double.MinValue);
                    goodValues.Add(q, q);
                    
                    q = new Quaternion(new Vector3D(Double.MaxValue, Double.MaxValue, Double.MaxValue), 33);
                    goodValues.Add(q, q);
                    
                    q = new Quaternion(new Vector3D(Double.MaxValue, Double.MaxValue, Double.MaxValue), 44);
                    goodValues.Add(q, q);
                    
                    q = new Quaternion(new Vector3D(Double.MaxValue, Double.MaxValue, Double.MaxValue), 55);
                    goodValues.Add(q, q);
                    
                    q = new Quaternion(new Vector3D(Double.MinValue, Double.MinValue, Double.MinValue), 50d);
                    goodValues.Add(q, q);
                    
                    q = new Quaternion(new Vector3D(1d, 0d, 0d), Double.MaxValue);
                    goodValues.Add(q, q);
                    
                    q = new Quaternion(new Vector3D(0d, 1d, 0d), Double.MinValue);
                    goodValues.Add(q, q);
                    
                    q = new Quaternion(Double.MinValue, Double.MinValue, Double.MaxValue, Double.MinValue);
                    goodValues.Add(q, q);
                    
                    q = new Quaternion(Double.MaxValue, Double.MaxValue, Double.MaxValue, Double.MaxValue);
                    goodValues.Add(q, q);
                    
                    q = new Quaternion(Double.Epsilon, Double.MinValue, Double.MaxValue, 130d);
                    goodValues.Add(q, q);

                    break;
                }
                case "System.Windows.Media.Media3D.Rotation3D":
                {
                    goodValues.Add(new AxisAngleRotation3D(), new AxisAngleRotation3D());
                    goodValues.Add(new AxisAngleRotation3D(new Vector3D(20, 30, 40), 50), new AxisAngleRotation3D(new Vector3D(20, 30, 40), 50));
                    goodValues.Add(new AxisAngleRotation3D(new Vector3D(-20, -30, -40), -50), new AxisAngleRotation3D(new Vector3D(-20, -30, -40), -50));
                    goodValues.Add(new AxisAngleRotation3D(new Vector3D(-20, -30, -40), Double.Epsilon), new AxisAngleRotation3D(new Vector3D(-20, -30, -40), Double.Epsilon));
                    goodValues.Add(new AxisAngleRotation3D(new Vector3D(-20, -30, -40), Double.MaxValue), new AxisAngleRotation3D(new Vector3D(-20, -30, -40), Double.MaxValue));
                    goodValues.Add(new AxisAngleRotation3D(new Vector3D(Double.MaxValue, Double.MaxValue, Double.MaxValue), -50), new AxisAngleRotation3D(new Vector3D(Double.MaxValue, Double.MaxValue, Double.MaxValue), -50));
                    goodValues.Add(new AxisAngleRotation3D(new Vector3D(Double.MaxValue, Double.MaxValue, Double.MaxValue), Double.MaxValue), new AxisAngleRotation3D(new Vector3D(Double.MaxValue, Double.MaxValue, Double.MaxValue), Double.MaxValue));
                    goodValues.Add(new AxisAngleRotation3D(new Vector3D(Double.MaxValue, Double.MaxValue, Double.MaxValue), Double.MinValue), new AxisAngleRotation3D(new Vector3D(Double.MaxValue, Double.MaxValue, Double.MaxValue), Double.MinValue));
                    goodValues.Add(new AxisAngleRotation3D(new Vector3D(Double.MaxValue, Double.MaxValue, Double.MaxValue), Double.Epsilon), new AxisAngleRotation3D(new Vector3D(Double.MaxValue, Double.MaxValue, Double.MaxValue), Double.Epsilon));
                    goodValues.Add(new AxisAngleRotation3D(new Vector3D(Double.MinValue, Double.MinValue, Double.MinValue), 50), new AxisAngleRotation3D(new Vector3D(Double.MinValue, Double.MinValue, Double.MinValue), 50));
                    goodValues.Add(new AxisAngleRotation3D(new Vector3D(Double.MinValue, Double.MinValue, Double.MinValue), Double.MaxValue), new AxisAngleRotation3D(new Vector3D(Double.MinValue, Double.MinValue, Double.MinValue), Double.MaxValue));
                    goodValues.Add(new AxisAngleRotation3D(new Vector3D(Double.MinValue, Double.MinValue, Double.MinValue), Double.MinValue), new AxisAngleRotation3D(new Vector3D(Double.MinValue, Double.MinValue, Double.MinValue), Double.MinValue));
                    goodValues.Add(new AxisAngleRotation3D(new Vector3D(Double.MinValue, Double.MinValue, Double.MinValue), Double.Epsilon), new AxisAngleRotation3D(new Vector3D(Double.MinValue, Double.MinValue, Double.MinValue), Double.Epsilon));
                    goodValues.Add(new AxisAngleRotation3D(new Vector3D(Double.MaxValue, Double.Epsilon, Double.MinValue), Double.Epsilon), new AxisAngleRotation3D(new Vector3D(Double.MaxValue, Double.Epsilon, Double.MinValue), Double.Epsilon));
                    goodValues.Add(new AxisAngleRotation3D(new Vector3D(1, 0, 0), Double.MaxValue), new AxisAngleRotation3D(new Vector3D(1, 0, 0), Double.MaxValue));
                    goodValues.Add(new AxisAngleRotation3D(new Vector3D(0, 1, 0), Double.MinValue), new AxisAngleRotation3D(new Vector3D(0, 1, 0), Double.MinValue));
                    goodValues.Add(new AxisAngleRotation3D(new Vector3D(0, 0, 1), Double.Epsilon), new AxisAngleRotation3D(new Vector3D(0, 0, 1), Double.Epsilon));
       
                    Quaternion q1 = new Quaternion(0, 0, 0, 0);
                    goodValues.Add(new AxisAngleRotation3D(q1.Axis, q1.Angle), new AxisAngleRotation3D(q1.Axis, q1.Angle));
                 
                    q1 = new Quaternion(Double.MaxValue, Double.MaxValue, Double.MaxValue, Double.MaxValue);
                    goodValues.Add(new AxisAngleRotation3D(q1.Axis, q1.Angle), new AxisAngleRotation3D(q1.Axis, q1.Angle));
                    
                    q1 = new Quaternion(Double.MinValue, Double.MinValue, Double.MinValue, Double.MinValue);
                    goodValues.Add(new AxisAngleRotation3D(q1.Axis, q1.Angle), new AxisAngleRotation3D(q1.Axis, q1.Angle));
                    
                    q1 = new Quaternion(Double.Epsilon, Double.Epsilon, Double.Epsilon, Double.Epsilon);
                    goodValues.Add(new AxisAngleRotation3D(q1.Axis, q1.Angle), new AxisAngleRotation3D(q1.Axis, q1.Angle));

                    badValues.Add(new AxisAngleRotation3D(new Vector3D(), Double.Epsilon), s_invalidException1);
                    badValues.Add(new AxisAngleRotation3D(new Vector3D(), Double.NaN), s_invalidException1);
                    badValues.Add(new AxisAngleRotation3D(new Vector3D(), Double.MinValue), s_invalidException1);
                    badValues.Add(new AxisAngleRotation3D(new Vector3D(0, 0, 0), 60), s_invalidException1);

                    badValues.Add(new AxisAngleRotation3D(new Vector3D(Double.Epsilon, Double.Epsilon, Double.Epsilon), Double.Epsilon), s_invalidException1);
                    badValues.Add(new AxisAngleRotation3D(new Vector3D(Double.Epsilon, Double.Epsilon, Double.Epsilon), 20), s_invalidException1);
                    badValues.Add(new AxisAngleRotation3D(new Vector3D(Double.Epsilon, Double.Epsilon, Double.Epsilon), Double.MaxValue), s_invalidException1);
                    badValues.Add(new AxisAngleRotation3D(new Vector3D(Double.Epsilon, Double.Epsilon, Double.Epsilon), Double.MinValue), s_invalidException1);
                    badValues.Add(new AxisAngleRotation3D(new Vector3D(Double.Epsilon, Double.Epsilon, Double.Epsilon), Double.NaN), s_invalidException1);
                    badValues.Add(new AxisAngleRotation3D(new Vector3D(Double.NaN, Double.NaN, Double.NaN), Double.NaN), s_invalidException1);
                    badValues.Add(new AxisAngleRotation3D(new Vector3D(Double.NaN, Double.NaN, Double.NaN), 30), s_invalidException1);
                    badValues.Add(new AxisAngleRotation3D(new Vector3D(Double.NaN, Double.NaN, Double.NaN), Double.MaxValue), s_invalidException1);
                    badValues.Add(new AxisAngleRotation3D(new Vector3D(Double.NaN, Double.NaN, Double.NaN), Double.MinValue), s_invalidException1);
                    badValues.Add(new AxisAngleRotation3D(new Vector3D(Double.NaN, Double.NaN, Double.NaN), Double.Epsilon), s_invalidException1);

                    break;
                }
                case "System.Nullable`1[System.Windows.Media.Media3D.Size3D]":
                {
                    goodValues.Add(new Size3D(0, 0, 0), new Size3D(0, 0, 0));
                    goodValues.Add(new Size3D(Double.MaxValue, Double.MaxValue, Double.MaxValue), new Size3D(Double.MaxValue, Double.MaxValue, Double.MaxValue));
                    goodValues.Add(new Size3D(Double.Epsilon, Double.Epsilon, Double.Epsilon), new Size3D(Double.Epsilon, Double.Epsilon, Double.Epsilon));

                    Size3D size3D = new Size3D(Double.PositiveInfinity, Double.PositiveInfinity, Double.PositiveInfinity);
                    badValues.Add(size3D, s_invalidException1);
                    badValues.Add(new Size3D(Double.NaN, Double.NaN, Double.NaN), s_invalidException1);
                    break;
                }
                case "System.Nullable`1[System.Windows.Size]":
                {
                    goodValues.Add(new Size(0, 0), new Size(0, 0));
                    goodValues.Add(new Size(Double.MaxValue, Double.MaxValue), new Size(Double.MaxValue, Double.MaxValue));
                    goodValues.Add(new Size(Double.Epsilon, Double.Epsilon), new Size(Double.Epsilon, Double.Epsilon));

                    badValues.Add(new Size(Double.PositiveInfinity, Double.PositiveInfinity), s_invalidException1);
                    badValues.Add(new Size(Double.NaN, Double.NaN), s_invalidException1);
                    break;
                }
                case "System.Nullable`1[System.Windows.Media.Media3D.Vector3D]":
                {
                    goodValues.Add(new Vector3D(0, 0, 0), new Vector3D(0, 0, 0));
                    goodValues.Add(new Vector3D(Double.MaxValue, Double.MaxValue, Double.MaxValue), new Vector3D(Double.MaxValue, Double.MaxValue, Double.MaxValue));
                    goodValues.Add(new Vector3D(Double.Epsilon, Double.Epsilon, Double.Epsilon), new Vector3D(Double.Epsilon, Double.Epsilon, Double.Epsilon));
                    goodValues.Add(new Vector3D(Double.MinValue, Double.MinValue, Double.MinValue), new Vector3D(Double.MinValue, Double.MinValue, Double.MinValue));
                    goodValues.Add(new Vector3D(Double.MinValue, Double.MaxValue, Double.Epsilon), new Vector3D(Double.MinValue, Double.MaxValue, Double.Epsilon));

                    Vector3D vector3D = new Vector3D(Double.PositiveInfinity, Double.PositiveInfinity, Double.PositiveInfinity);
                    badValues.Add(vector3D, s_invalidException1);
                    badValues.Add(new Vector3D(Double.NaN, Double.NaN, Double.NaN), s_invalidException1);
                    break;
                }
                case "System.Nullable`1[System.Windows.Thickness]":
                {
                    goodValues.Add(new Thickness(0, 0, 0, 0), new Thickness(0, 0, 0, 0));
                    goodValues.Add(new Thickness(Single.MaxValue, Single.MaxValue, Single.MaxValue, Single.MaxValue), new Thickness(Single.MaxValue, Single.MaxValue, Single.MaxValue, Single.MaxValue));
                    goodValues.Add(new Thickness(Single.Epsilon, Single.Epsilon, Single.Epsilon, Single.Epsilon), new Thickness(Single.Epsilon, Single.Epsilon, Single.Epsilon, Single.Epsilon));

                    badValues.Add(new Thickness(Single.PositiveInfinity, Single.PositiveInfinity, Single.PositiveInfinity, Single.PositiveInfinity), s_invalidException1);
                    badValues.Add(new Thickness(Single.NaN, Single.NaN, Single.NaN, Single.NaN), s_invalidException1);
                    break;
                }
                case "System.Nullable`1[System.Windows.Vector]":
                {
                    goodValues.Add(new Vector(0, 0), new Vector(0, 0));
                    goodValues.Add(new Vector(Double.MaxValue, Double.MaxValue), new Vector(Double.MaxValue, Double.MaxValue));
                    goodValues.Add(new Vector(Double.Epsilon, Double.Epsilon), new Vector(Double.Epsilon, Double.Epsilon));

                    badValues.Add(new Vector(Double.PositiveInfinity, Double.PositiveInfinity), s_invalidException1);
                    badValues.Add(new Vector(Double.NaN, Double.NaN), s_invalidException1);
                    break;
                }
                case "System.Double":
                {
                    if (p.Name == "SpeedRatio" && p.DeclaringType.ToString() == "System.Windows.Media.Animation.Timeline")
                    {
                        //SpeedRatio value must be greater than 0.
                        goodValues.Add(.00001d, .00001d);
                        goodValues.Add(Double.Epsilon, Double.Epsilon);
                        goodValues.Add(Double.MaxValue, Double.MaxValue);

                        badValues.Add(Double.NaN, s_invalidException1);
                        badValues.Add(Double.PositiveInfinity, s_invalidException2);
                        badValues.Add(0d, s_invalidException2);
                        badValues.Add(-1d, s_invalidException2);
                    }
                    else
                    {
                        //Generic boundary conditions.
                        goodValues.Add(1d, 1d);
                        goodValues.Add(0d, 0d);
                        goodValues.Add(-1d, -1d);
                        goodValues.Add(Double.Epsilon, Double.Epsilon);
                        goodValues.Add(Double.MinValue, Double.MinValue);
                        goodValues.Add(Double.MaxValue, Double.MaxValue);
                        goodValues.Add(Double.PositiveInfinity, Double.PositiveInfinity);
                        goodValues.Add(Double.NegativeInfinity, Double.NegativeInfinity);
                    }
                    break;
                }
                case "System.Windows.Point":
                {
                    goodValues.Add(new Point(0,0), new Point(0,0));
                    goodValues.Add(new Point(0,1), new Point(0,1));
                    goodValues.Add(new Point(1,0), new Point(1,0));
                    goodValues.Add(new Point(1,1), new Point(1,1));
                    goodValues.Add(new Point(Double.Epsilon, Double.Epsilon), new Point(Double.Epsilon, Double.Epsilon));

                    if (p.DeclaringType.ToString() == "System.Windows.Media.Animation.KeySpline")
                    {
                        badValues.Add(new Point(-1,-1), s_invalidException1);
                        badValues.Add(new Point(1.000001,1.000001), s_invalidException1);
                        badValues.Add(new Point(Double.MaxValue, Double.MaxValue), s_invalidException1);
                        badValues.Add(new Point(Double.MinValue, Double.MinValue), s_invalidException1);
                        badValues.Add(new Point(Double.PositiveInfinity, Double.PositiveInfinity), s_invalidException1);
                        badValues.Add(new Point(Double.NaN, Double.NaN), s_invalidException1);
                    }
                    else
                    {
                        goodValues.Add(new Point(-1,-1), new Point(-1,-1));
                        goodValues.Add(new Point(1.000001,1.000001), new Point(1.000001,1.000001));
                        goodValues.Add(new Point(Double.MaxValue, Double.MaxValue), new Point(Double.MaxValue, Double.MaxValue));
                        goodValues.Add(new Point(Double.MinValue, Double.MinValue), new Point(Double.MinValue, Double.MinValue));
                        goodValues.Add(new Point(Double.PositiveInfinity, Double.PositiveInfinity), new Point(Double.PositiveInfinity, Double.PositiveInfinity));
                    }
                    break;
                
                }
                case "System.Windows.Media.Animation.SlipBehavior":
                {
                    goodValues.Add(0, SlipBehavior.Grow);
                    goodValues.Add(1, SlipBehavior.Slip);
                    goodValues.Add(SlipBehavior.Grow, SlipBehavior.Grow);
                    goodValues.Add(SlipBehavior.Slip, SlipBehavior.Slip);

                    badValues.Add((2 + (int)SlipBehavior.Grow), s_invalidException1);
                    badValues.Add((int)SlipBehavior.Slip + 2, s_invalidException1);
                    badValues.Add(-1, s_invalidException1);
                    break;
                }
                case "System.String":
                {
                    if (p.Name == "Name" && p.DeclaringType.ToString() == "System.Windows.Media.Animation.Timeline")
                    {
                        string nameString = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                        goodValues.Add("", "");
                        goodValues.Add(nameString, nameString);
                        string badNameString = "`1234567890-=[]\\;',./~!@#$%^&*()_+{}|:<>?abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
                        badValues.Add(badNameString, s_invalidException4);
                    }
                    else
                    {
                        string testString = "nopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
                        goodValues.Add("", "");
                        goodValues.Add(testString, testString);
                    }
                    break;
                }
                case "System.Nullable`1[System.TimeSpan]":
                {
                    goodValues.Add(TimeSpan.Zero, TimeSpan.Zero);

                    Nullable<TimeSpan> timeSpan2 = (Nullable<TimeSpan>)new TimeSpan(0, 0, 0, 0, 1000);
                    goodValues.Add(timeSpan2, timeSpan2);

                    Nullable<TimeSpan> timeSpan3 = (Nullable<TimeSpan>)new TimeSpan(0, 0, 0, 0, 0000001);
                    goodValues.Add(timeSpan3, timeSpan3);

                    //fails
                    Nullable<TimeSpan> timeSpan4 = (Nullable<TimeSpan>)TimeSpan.MaxValue;
                    goodValues.Add(timeSpan4, timeSpan4);

                    //fails
                    Nullable<TimeSpan> timeSpan5 = (Nullable<TimeSpan>)TimeSpan.MinValue;
                    goodValues.Add(timeSpan5, timeSpan5);
          
                    break;
                }
                case "System.Windows.Media.Animation.FillBehavior":
                {
                    goodValues.Add(0, FillBehavior.HoldEnd);
                    goodValues.Add(1, FillBehavior.Stop);
                    goodValues.Add((FillBehavior)FillBehavior.Stop, (FillBehavior)FillBehavior.Stop);
                    goodValues.Add((FillBehavior)FillBehavior.HoldEnd, (FillBehavior)FillBehavior.HoldEnd);

                    badValues.Add((2 + (int)FillBehavior.HoldEnd), s_invalidException3);
                    badValues.Add((int)FillBehavior.Stop + 2, s_invalidException3);
                    badValues.Add(-1, s_invalidException1);

                    break;
                }
                case "System.Windows.Duration":
                {
                    goodValues.Add(new TimeSpan(0, 0, 0, 0, 1000), new TimeSpan(0, 0, 0, 0, 1000));
                    goodValues.Add(TimeSpan.Zero, TimeSpan.Zero);
                    goodValues.Add(TimeSpan.MaxValue, TimeSpan.MaxValue);

                    badValues.Add(new TimeSpan(0, 0, 0, 0, 1).Negate(), s_invalidException1);
                    badValues.Add(TimeSpan.MinValue, s_invalidException1);

                    break;
                }
                case "System.Windows.Media.Animation.RepeatBehavior":
                {
                    //Duration.
                    goodValues.Add(new TimeSpan(0, 0, 0, 0, 1000), new TimeSpan(0, 0, 0, 0, 1000));
                    goodValues.Add(TimeSpan.Zero, TimeSpan.Zero);
                    goodValues.Add(TimeSpan.MaxValue, TimeSpan.MaxValue);

                    badValues.Add(new TimeSpan(0, 0, 0, 0, 1).Negate(), s_invalidException1);
                    badValues.Add(TimeSpan.MinValue, s_invalidException1);
                
                    break;
                }
                case "System.Windows.Media.PathGeometry":
                {
                    goodValues.Add(new PathGeometry(), new PathGeometry());

                    break;
                }
                case "System.Windows.Media.Animation.KeyTime":
                {
                    goodValues.Add(TimeSpan.Zero, TimeSpan.Zero);
                    goodValues.Add(TimeSpan.MaxValue, TimeSpan.MaxValue);
                    
                    badValues.Add(TimeSpan.MinValue, s_invalidException2);

                    break;
                }
                case "System.Windows.Media.Animation.IEasingFunction":
                {
                    goodValues.Add(new ElasticEase(), new ElasticEase());
                    goodValues.Add(new BounceEase(), new BounceEase());

                    break;
                }
                default:
                {
                    object obj = null;
                    if (p.PropertyType.ToString().EndsWith("Collection"))
                    {
                        obj = PredefinedObjects.MakeCollection(p.PropertyType.ToString());
                    }
                    else
                    {
                        obj = PredefinedObjects.MakeValue(p);
                    }
                    if (obj == null)
                    {
                        throw new ApplicationException("Unknown dataType: " + p.PropertyType.ToString());
                    }
                    goodValues.Add(obj,obj);
                    break;
                }
            }
            
            
        }
        #region Member
        private static string s_invalidException1 = "System.InvalidOperationException";
        private static string s_invalidException2 = "System.ArgumentOutOfRangeException";
        private static string s_invalidException3 = "System.ArgumentException";
        private static string s_invalidException4 = "System.Reflection.TargetInvocationException";
        #endregion

    }
}
