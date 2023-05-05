// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Reflection;
using Microsoft.Test.Animation.ObjectPatterns;
using System.Collections;
using System.Collections.Generic;

//------------------------------------------------------------------

namespace Microsoft.Test.Animation.ObjectPatterns
{
    //--------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    public class PredefinedObjects
    {
        //----------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        public static object MakeValue (PropertyInfo p)
        {
            return MakeTypeNameValue (p.PropertyType.ToString (), true);
        }

        /// <summary>
        /// 
        /// </summary>
        public static object MakeValue (Type t)
        {
             return MakeTypeNameValue(t.FullName.ToString(), true);
        }

        /// <summary>
        /// 
        /// </summary>
        public static object MakeSecondValue(Type t)
        {
            return MakeTypeNameValue(t.FullName.ToString(), false);
        }


        /// <summary>
        /// 
        /// </summary>
        private static object MakeTypeNameValue(string name, bool firstOption)
        {
            switch (name)
            {
                case "System.Boolean":
                    {
                        return (firstOption) ? true : false;
                    }

                case "System.Char":
                    {
                        return '0';
                    }

                case "System.Single" :
                    {
                        return (firstOption) ? Convert.ToSingle(true):Convert.ToSingle(false);
                    }

                case "System.Int32" :
                    {
                        return (firstOption) ? 0 : 2;
                    }
                case "System.Int32[]":
                    {
                        System.Int32[] retval = new System.Int32[1];

                        return (firstOption) ? retval[0] = 2 : retval[0] = 1;
                    }
                case "System.Double":
                    {
                        return (firstOption) ? 0.4 : 0.3;
                    }
                case "System.Double[]":
                    {
                        System.Double[] retval = new System.Double[1];

                        return (firstOption) ? retval[0] = 0.4 : retval[0] = 0.3;
                    }
                case "System.String":
                    {
                        return (firstOption) ? "":"a";
                    }

                case "System.Byte":
                    {
                        return  Convert.ToByte("0", System.Globalization.CultureInfo.InvariantCulture);
                    }

                case "System.Byte[]":
                    {
                        System.Byte[] retval = new System.Byte[1];
                        retval[0] = Convert.ToByte("10", System.Globalization.CultureInfo.InvariantCulture);
                        return retval;
                    }
                case "System.Decimal":
                    {
                        return Convert.ToDecimal("0", System.Globalization.CultureInfo.InvariantCulture);
                    }

                case "System.Int16":
                    {
                        return Convert.ToInt16("0", System.Globalization.CultureInfo.InvariantCulture);
                    }

                case "System.Int64":
                    {
                        return Convert.ToInt64("0", System.Globalization.CultureInfo.InvariantCulture);
                    }

                case "System.IntPtr" :
                    {
                        return IntPtr.Zero;
                    }
                case "System.Object":
                    {
                        return new System.Object();
                    }
                case "System.TimeSpan":
                    {
                        return new System.TimeSpan(0, 0, 1);
                    }

                case "System.UInt16[]":
                    {
                        System.UInt16[] retval = new System.UInt16[1];

                        retval[0] = Convert.ToUInt16("0", System.Globalization.CultureInfo.InvariantCulture);
                        return retval;
                    }
                case "System.Uri":
                    {
                        System.Uri retval = new System.Uri(s_jpegFile);
                        return retval;
                    }
                case "System.Windows.DependencyProperty":
                    {
                        System.Windows.DependencyObject dobj = new System.Windows.DependencyObject();
                        return null;//System.Windows.DependencyProperty.FromName("ChangeableTest", dobj.GetType());
                    }

                case "System.Windows.DependencyObject":
                    {

                        return new System.Windows.DependencyObject();
                    }
                case "System.Windows.Vector" :
                    {
                        return (firstOption) ? new System.Windows.Vector(1.2, 3.7) : new System.Windows.Vector(1.3, 3.8);
                    }
                case "System.Windows.Vector[]":
                    {
                        System.Windows.Vector[] retval = new System.Windows.Vector[1];

                        return (firstOption) ? retval[0] = new System.Windows.Vector(1.2, 3.7) : retval[0] = new System.Windows.Vector(1.3, 3.8);
                    }

                case "System.Windows.Media.BrushMappingMode" :
                    {
                        return (firstOption) ?  System.Windows.Media.BrushMappingMode.RelativeToBoundingBox : System.Windows.Media.BrushMappingMode.Absolute ;
                    }
                case "System.Windows.Media.Stretch" :
                    {
                        return (firstOption) ? System.Windows.Media.Stretch.Fill : System.Windows.Media.Stretch.None;
                    }
                case "System.Windows.Media.HorizontalAlignment" :
                    {
                        return (firstOption) ? System.Windows.Media.AlignmentX.Left : System.Windows.Media.AlignmentX.Right;
                    }
                case "System.Windows.Media.VerticalAlignment" :
                    {
                        return (firstOption) ? System.Windows.Media.AlignmentY.Top : System.Windows.Media.AlignmentY.Bottom;
                    }

                case "System.Windows.Media.TileMode" :
                    {
                        return (firstOption) ? System.Windows.Media.TileMode.None : System.Windows.Media.TileMode.Tile;
                    }
                case "System.Windows.Media.ColorInterpolationMode" :
                    {
                        return (firstOption) ? System.Windows.Media.ColorInterpolationMode.SRgbLinearInterpolation : System.Windows.Media.ColorInterpolationMode.ScRgbLinearInterpolation;
                    }
                case "System.Windows.Media.GradientSpreadMethod" :
                    {
                        return (firstOption) ? System.Windows.Media.GradientSpreadMethod.Pad : System.Windows.Media.GradientSpreadMethod.Reflect;
                    }

                case "System.Windows.Point" :
                    {
                        return (firstOption) ? new System.Windows.Point(10.0, 20.0) : new System.Windows.Point(10.1, 20.1);
                    }
                case "System.Windows.Point[]":
                    {
                        System.Windows.Point[] retval = new System.Windows.Point[1];
                        retval[0] = new System.Windows.Point (10.0, 20.0);
                        return retval;
                    }

                case "System.Windows.Size":
                    {
                        return firstOption ? new System.Windows.Size(50.0, 70.0) : new System.Windows.Size(50.1, 70.1);
                    }

                case "System.Windows.Rect":
                    {
                        return firstOption ? new System.Windows.Rect(10.0, 20.0, 50.0, 70.0) : new System.Windows.Rect(10.1, 20.0, 50.1, 70.0);
                    }
                case "System.Windows.FrameworkContentElement":
                    {
                        return new System.Windows.FrameworkContentElement();
                    }
                case "System.Windows.FrameworkElement":
                    {
                        return new System.Windows.Controls.Button();
                    }
                case "System.Windows.FrameworkTemplate":
                    {
                        return new System.Windows.Controls.ControlTemplate();
                    }
                
                case "System.Windows.Media.Color":
                    {
                        return firstOption ? System.Windows.Media.Color.FromScRgb(0.8f, 0.7f, 0.5f, 0.1f) : System.Windows.Media.Color.FromScRgb(0.9f, 0.8f, 0.6f, 0.2f);
                    }
                case "System.Windows.Media.Color[]":
                    {
                        System.Windows.Media.Color[] retval = new System.Windows.Media.Color[1];

                        return firstOption ? retval[0] = System.Windows.Media.Color.FromScRgb(0.8f, 0.7f, 0.5f, 0.1f) : retval[0] = System.Windows.Media.Color.FromScRgb(0.9f, 0.8f, 0.6f, 0.2f);
                    }

                case "System.Windows.Color[]":
                    {
                        System.Windows.Media.Color[] retval = new System.Windows.Media.Color[1];

                        retval[0] = System.Windows.Media.Color.FromScRgb (0.8f, 0.7f, 0.5f, 0.1f);
                        return retval;
                    }
                case "System.Windows.Media.Drawing":
                    {
                        return new System.Windows.Media.DrawingGroup ();
                    }
                case "System.Windows.Media.Matrix":
                    {
                        return firstOption ? new System.Windows.Media.Matrix(1.0, 0.0, 0.0, 1.0, 0.0, 0.0) : new System.Windows.Media.Matrix(2.0, 0.0, 0.0, 1.0, 0.0, 0.0);
                    }
                case "System.Windows.Media.Transform":
                    {
                        return new System.Windows.Media.TranslateTransform (17.0, 3.0);
                    }
                case "System.Windows.Media.Transform[]":
                    {
                        System.Windows.Media.Transform[] retval = new System.Windows.Media.Transform[1];
                        retval[0] = new System.Windows.Media.TranslateTransform (17.0, 3.0);
                        return retval;
                    }
                case "System.Windows.Media.Brush":
                    {
                        return new System.Windows.Media.SolidColorBrush (System.Windows.Media.Color.FromScRgb (0.7f, 0.55f, 1.0f, 0.1f));
                    }

                case "System.Windows.Media.GeometryCombineMode":
                    {
                        return (firstOption) ? System.Windows.Media.GeometryCombineMode.Union : System.Windows.Media.GeometryCombineMode.Intersect;
                    }

                case "System.Windows.Media.FillRule":
                    {
                        return firstOption ? System.Windows.Media.FillRule.EvenOdd : System.Windows.Media.FillRule.Nonzero;
                    }
                case "System.Windows.Media.PenLineCap" :
                    {
                        return (firstOption) ? System.Windows.Media.PenLineCap.Flat : System.Windows.Media.PenLineCap.Round;
                    }
                case "System.Windows.Media.PenLineJoin" :
                    {
                        return (firstOption) ? System.Windows.Media.PenLineJoin.Miter : System.Windows.Media.PenLineJoin.Round;
                    }

                case "System.Windows.Media.Geometry" :
                    {
                        return new System.Windows.Media.EllipseGeometry (new System.Windows.Rect (10.0, 20.0, 150.0, 90.0));
                    }
                case "System.Windows.Media.Geometry[]":
                    {
                        System.Windows.Media.Geometry[] retval = new System.Windows.Media.Geometry[1];
                        retval[0] = new System.Windows.Media.EllipseGeometry (new System.Windows.Rect (10.0, 20.0, 150.0, 90.0));
                        return retval;
                    }
                case "System.Windows.Media.GradientStop":
                    {
                        return new System.Windows.Media.GradientStop (System.Windows.Media.Color.FromScRgb (0.7f, 0.55f, 1.0f, 0.1f), 0.0);
                    }
                case "System.Windows.Media.GradientStop[]":
                    {
                        System.Windows.Media.GradientStop[] retval = new System.Windows.Media.GradientStop[1];

                        retval[0] = new System.Windows.Media.GradientStop (System.Windows.Media.Color.FromScRgb (0.7f, 0.55f, 1.0f, 0.1f), 0.0);
                        return retval;
                    }

                case "System.Windows.Media.GradientStopCollection":
                    {
                        System.Windows.Media.GradientStopCollection retval = new System.Windows.Media.GradientStopCollection ();

                        retval.Add (new System.Windows.Media.GradientStop (System.Windows.Media.Color.FromScRgb (0.7f, 1.0f, 0.0f, 0.1f), 0.0));
                        retval.Add (new System.Windows.Media.GradientStop (System.Windows.Media.Color.FromScRgb (0.2f, 0.0f, 1.0f, 0.7f), 1.0));
                        return retval;
                    }

                case "System.Windows.Media.PathSegmentCollection":
                    {
                        return MakePathSegmentCollection ();
                    }

                case "System.Windows.Media.PathSegment":
                    {
                        return new System.Windows.Media.LineSegment (new System.Windows.Point (10.0, 20.0), true);
                    }
                case "System.Windows.Media.PathSegment[]":
                    {
                        System.Windows.Media.PathSegment[] retval = new System.Windows.Media.PathSegment[1];
                        retval[0] = new System.Windows.Media.LineSegment (new System.Windows.Point (10.0, 20.0), true);
                        return retval;
                    }
                case "System.Windows.Media.PathFigureCollection":
                    {
                        System.Windows.Media.PathFigureCollection retval = new System.Windows.Media.PathFigureCollection ();

                        retval.Add (new System.Windows.Media.PathFigure (new System.Windows.Point(0, 0), MakePathSegmentCollection (), false));
                        return retval;
                    }

                case "System.Windows.Media.PathFigure":
                    {
                        return new System.Windows.Media.PathFigure(new System.Windows.Point(0, 0), MakePathSegmentCollection(), false);
                    }

                case "System.Windows.Media.PathFigure[]":
                    {
                        System.Windows.Media.PathFigure[] retval = new System.Windows.Media.PathFigure[1];

                        retval[0] = new System.Windows.Media.PathFigure(new System.Windows.Point(0, 0), MakePathSegmentCollection(), false);
                        return retval;
                    }
                case "System.Windows.Media.PathGeometry" :
                    {
                        System.Windows.Media.PathFigure[] tmp = new System.Windows.Media.PathFigure[1];

                        tmp[0] = new System.Windows.Media.PathFigure(new System.Windows.Point(0, 0), MakePathSegmentCollection(), false);
                        return new System.Windows.Media.PathGeometry(tmp);
                    }
                case "System.Windows.PointCollection":
                    {
                        return MakePointCollection();
                    }

                case "System.Collections.ICollection":
                    {
                        return null;
                    }
                case "System.Windows.Media.ColorContext":
                    {
                        return new System.Windows.Media.ColorContext(System.Windows.Media.PixelFormats.Bgra32);
                    }
                case "System.Windows.Media.PixelFormat":
                    {
                        return System.Windows.Media.PixelFormats.Pbgra32;
                    }
                case "System.IO.Stream":
                    {
                        try
                        {
                            return new FileStream(s_jpegFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                        }
                        catch (System.IO.IOException e)
                        {
                            Console.WriteLine("Image could not be accesssed: {0}", e.Message);
                            return null;
                        }
                    }
                case "System.Windows.Length":
                    {
                        return 3.0;
                    }
                case "System.Windows.Thickness":
                    {
                        return new System.Windows.Thickness(3.0);
                    }
                case "System.Windows.Media.Imaging.CodecInfo":
                    {
                        System.Windows.Media.Imaging.JpegBitmapEncoder ie = new System.Windows.Media.Imaging.JpegBitmapEncoder();
                        return ie.CodecInfo;
                    }
                case "System.Windows.Media.Imaging.BitmapCacheOption":
                    {
                        return System.Windows.Media.Imaging.BitmapCacheOption.Default;
                    }
                case "System.Windows.Media.Imaging.BitmapCreateOptions":
                    {
                        return System.Windows.Media.Imaging.BitmapCreateOptions.None;
                    }
                case "System.Windows.Media.Imaging.BitmapPalette":
                    {
                        List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
                        colors.Add(System.Windows.Media.Color.FromScRgb(0.7f, 0.5f, 0.3f, 0.1f));
                        colors.Add(System.Windows.Media.Color.FromScRgb(0.1f, 0.3f, 0.5f, 0.7f));
                        return new System.Windows.Media.Imaging.BitmapPalette(colors);
                    }

                case "System.Windows.Media.Media3D.Geometry3D":
                    {
                        //Geometry3D is an abstact class, so use MeshGeometry3D instead
                        return new System.Windows.Media.Media3D.MeshGeometry3D();
                    }
                case "System.Windows.Media.Media3D.Quaternion":
                    {
                        return new System.Windows.Media.Media3D.Quaternion (0.0, 1.0, 2.0, 3.0);
                    }
                case "System.Windows.Media.Media3D.Material":
                    {
                        return new System.Windows.Media.Media3D.DiffuseMaterial();
                    }

                case "System.Windows.Media.Media3D.Model3D":
                    {
                        return new System.Windows.Media.Media3D.AmbientLight();
                    }

                case "System.Windows.Media.Media3D.Model3D[]":
                    {
                        System.Windows.Media.Media3D.Model3D[] retval = new System.Windows.Media.Media3D.Model3D[1];

                        retval[0] = new System.Windows.Media.Media3D.AmbientLight();
                        return retval;
                    }

                case "System.Windows.Media.Media3D.Point3D":
                    {
                        return new System.Windows.Media.Media3D.Point3D(1.0, 2.0, 3.0);
                    }

                case "System.Windows.Media.Media3D.Point3D[]":
                    {
                        System.Windows.Media.Media3D.Point3D[] retval = new System.Windows.Media.Media3D.Point3D[1];

                        retval[0] = new System.Windows.Media.Media3D.Point3D(1.0, 2.0, 3.0);
                        return retval;
                    }

                case "System.Windows.Media.Media3D.Vector3D":
                    {
                        return new System.Windows.Media.Media3D.Vector3D (100.0, 200.0, 3.0);
                    }
                case "System.Windows.Media.Media3D.Vector3D[]":
                    {
                        System.Windows.Media.Media3D.Vector3D[] retval = new System.Windows.Media.Media3D.Vector3D[1];
                        retval[0] =  new System.Windows.Media.Media3D.Vector3D(100.0, 200.0, 3.0);
                        return retval;
                    }
                case "System.Windows.Media.Media3D.Matrix3D":
                    {
                        return new System.Windows.Media.Media3D.Matrix3D (0.5, 1.2, 1.1, 0.0, 0.5, 1.2, 1.1, 0.0, 0.5, 1.2, 1.1, 0.0, 0.5, 1.2, 1.1, 0.0);
                    }

                case "System.Windows.Media.Media3D.Transform3D":
                    {
                        return new System.Windows.Media.Media3D.TranslateTransform3D (new System.Windows.Media.Media3D.Vector3D (10.0, 20.0, 3.0));
                    }
                case "System.Windows.Media.Media3D.Transform3D[]":
                    {
                        System.Windows.Media.Media3D.Transform3D[] retval = new System.Windows.Media.Media3D.Transform3D[1];

                        retval[0] = new System.Windows.Media.Media3D.TranslateTransform3D (new System.Windows.Media.Media3D.Vector3D (10.0, 20.0, 3.0));
                        return retval;
                    }

                case "System.Windows.Media.Media3D.MeshGeometry3D":
                    {
                        return new System.Windows.Media.Media3D.MeshGeometry3D ();
                    }
                case "System.Windows.Media.Imaging.BitmapCodecInfo":
                    {
                        Stream imageStream = new System.IO.FileStream (s_jpegFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                        System.Windows.Media.Imaging.BitmapDecoder dec = System.Windows.Media.Imaging.BitmapDecoder.Create(imageStream, System.Windows.Media.Imaging.BitmapCreateOptions.None, System.Windows.Media.Imaging.BitmapCacheOption.Default);
                        System.Windows.Media.Imaging.BitmapCodecInfo ci = dec.CodecInfo;
                        imageStream.Close();
                        return ci;
                    }

                case "System.Windows.Media.Imaging.BitmapSource":
                    {
                        try
                        {
                            Stream imageStream = new FileStream(s_jpegFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                            System.Windows.Media.Imaging.BitmapSource bsrc = System.Windows.Media.Imaging.BitmapFrame.Create(imageStream);

                            return bsrc;
                        }
                        catch (System.IO.IOException e)
                        {
                            Console.WriteLine("Image could not be accesssed: {0}", e.Message);
                            return null;
                        }
                    }
                case "System.Windows.Media.Imaging.BitmapSizeOptions":
                    {
                        return System.Windows.Media.Imaging.BitmapSizeOptions.FromWidthAndHeight(100, 100);
                    }

                case "System.Windows.Media.Imaging.Rotation":
                    {
                        return System.Windows.Media.Imaging.Rotation.Rotate0;
                    }

                case "System.Windows.Int32Rect" :
                    {
                        return new System.Windows.Int32Rect(0, 0, 100, 100);
                    }

                case "System.Windows.Media.Animation.AnimationEffect":
                    {
                        // 
                        return null;
                    }
                case "System.Windows.Media.Animation.BooleanKeyFrame":
                    {
                        return new System.Windows.Media.Animation.DiscreteBooleanKeyFrame(true);
                    }
                case "System.Windows.Media.Animation.BooleanKeyFrame[]":
                    {
                        System.Windows.Media.Animation.DiscreteBooleanKeyFrame [] bkf = new System.Windows.Media.Animation.DiscreteBooleanKeyFrame[1];
                        bkf[0] = new System.Windows.Media.Animation.DiscreteBooleanKeyFrame(true);
                        return bkf;
                    }
                case "System.Windows.Media.Animation.ByteKeyFrame":
                    {
                        return new System.Windows.Media.Animation.LinearByteKeyFrame(Convert.ToByte("1", System.Globalization.CultureInfo.InvariantCulture));
                    }
                case "System.Windows.Media.Animation.ByteKeyFrame[]":
                    {
                        System.Windows.Media.Animation.LinearByteKeyFrame [] retval = new System.Windows.Media.Animation.LinearByteKeyFrame[1];
                        retval[0] = new System.Windows.Media.Animation.LinearByteKeyFrame(Convert.ToByte("1", System.Globalization.CultureInfo.InvariantCulture));
                        return retval;
                    }
                case "System.Windows.Media.Animation.CharKeyFrame":
                    {
                        return new System.Windows.Media.Animation.DiscreteCharKeyFrame('a');
                    }
                case "System.Windows.Media.Animation.CharKeyFrame[]":
                    {
                        System.Windows.Media.Animation.DiscreteCharKeyFrame[] bkf = new System.Windows.Media.Animation.DiscreteCharKeyFrame[1];
                        bkf[0] = new System.Windows.Media.Animation.DiscreteCharKeyFrame('a');
                        return bkf;
                    }
                case "System.Windows.Media.Animation.Clock":
                    {
                        System.Windows.Media.Animation.DoubleAnimation timeline = new System.Windows.Media.Animation.DoubleAnimation();
                        return timeline.CreateClock();
                    }
                case "System.Windows.Media.Animation.ClockGroup":
                    {
                        System.Windows.Media.Animation.ParallelTimeline timeline = new System.Windows.Media.Animation.ParallelTimeline();
                        System.Windows.Media.Animation.ClockGroup cg = timeline.CreateClock();
                        return cg;
                    }
                case "System.Windows.Media.Animation.ClockController":
                    {
                        System.Windows.Media.Animation.DoubleAnimation timeline = new System.Windows.Media.Animation.DoubleAnimation();
                        System.Windows.Media.Animation.Clock clock = timeline.CreateClock();
                        return clock.Controller;
                    }
                case "System.Windows.Media.Animation.ColorKeyFrame":
                    {
                        return new System.Windows.Media.Animation.DiscreteColorKeyFrame(System.Windows.Media.Color.FromScRgb(1.0f, 0.2f, 0.2f, 0.4f));
                    }
                case "System.Windows.Media.Animation.ColorKeyFrame[]":
                    {
                        System.Windows.Media.Animation.DiscreteColorKeyFrame[] bkf = new System.Windows.Media.Animation.DiscreteColorKeyFrame[1];
                        bkf[0] = new System.Windows.Media.Animation.DiscreteColorKeyFrame(System.Windows.Media.Color.FromScRgb(1.0f, 0.2f, 0.2f, 0.4f));
                        return bkf;
                    }
                case "System.Windows.Media.Animation.DecimalKeyFrame":
                    {
                        return new System.Windows.Media.Animation.DiscreteDecimalKeyFrame(Convert.ToDecimal("0", System.Globalization.CultureInfo.InvariantCulture));
                    }
                case "System.Windows.Media.Animation.DecimalKeyFrame[]":
                    {
                        System.Windows.Media.Animation.DiscreteDecimalKeyFrame[] bkf = new System.Windows.Media.Animation.DiscreteDecimalKeyFrame[1];
                        bkf[0] = new System.Windows.Media.Animation.DiscreteDecimalKeyFrame(Convert.ToDecimal("0", System.Globalization.CultureInfo.InvariantCulture));
                        return bkf;
                    }
                case "System.Windows.Media.Animation.DoubleKeyFrame":
                    {
                        return new System.Windows.Media.Animation.DiscreteDoubleKeyFrame(2.0);
                    }
                case "System.Windows.Media.Animation.DoubleKeyFrame[]":
                    {
                        System.Windows.Media.Animation.DiscreteDoubleKeyFrame[] bkf = new System.Windows.Media.Animation.DiscreteDoubleKeyFrame[1];
                        bkf[0] = new System.Windows.Media.Animation.DiscreteDoubleKeyFrame(2.0);
                        return bkf;
                    }
                case "System.Windows.Media.Animation.HandoffBehavior":
                    {
                        return System.Windows.Media.Animation.HandoffBehavior.Compose; 
                    }
                case "System.Windows.Media.Animation.Int16KeyFrame":
                    {
                        return new System.Windows.Media.Animation.DiscreteInt16KeyFrame(Convert.ToInt16("0", System.Globalization.CultureInfo.InvariantCulture));
                    }
                case "System.Windows.Media.Animation.Int16KeyFrame[]":
                    {
                        System.Windows.Media.Animation.DiscreteInt16KeyFrame[] bkf = new System.Windows.Media.Animation.DiscreteInt16KeyFrame[1];
                        bkf[0] = new System.Windows.Media.Animation.DiscreteInt16KeyFrame(Convert.ToInt16("0", System.Globalization.CultureInfo.InvariantCulture));
                        return bkf;
                    }
                case "System.Windows.Media.Animation.Int32KeyFrame":
                    {
                        return new System.Windows.Media.Animation.DiscreteInt32KeyFrame(0);
                    }
                case "System.Windows.Media.Animation.Int32KeyFrame[]":
                    {
                        System.Windows.Media.Animation.DiscreteInt32KeyFrame[] bkf = new System.Windows.Media.Animation.DiscreteInt32KeyFrame[1];
                        bkf[0] = new System.Windows.Media.Animation.DiscreteInt32KeyFrame(0);
                        return bkf;
                    }
                case "System.Windows.Media.Animation.Int64KeyFrame":
                    {
                        return new System.Windows.Media.Animation.DiscreteInt64KeyFrame(Convert.ToInt64("0", System.Globalization.CultureInfo.InvariantCulture));
                    }
                case "System.Windows.Media.Animation.Int64KeyFrame[]":
                    {
                        System.Windows.Media.Animation.DiscreteInt64KeyFrame[] bkf = new System.Windows.Media.Animation.DiscreteInt64KeyFrame[1];
                        bkf[0] = new System.Windows.Media.Animation.DiscreteInt64KeyFrame(Convert.ToInt64("0", System.Globalization.CultureInfo.InvariantCulture));
                        return bkf;
                    }
                case "System.Windows.Media.Animation.MatrixKeyFrame":
                    {
                        return new System.Windows.Media.Animation.DiscreteMatrixKeyFrame(new System.Windows.Media.Matrix(1.0, 1.0, 2.0, 2.0, 1.0, 1.0));
                    }
                case "System.Windows.Media.Animation.MatrixKeyFrame[]":
                    {
                        System.Windows.Media.Animation.DiscreteMatrixKeyFrame[] bkf = new System.Windows.Media.Animation.DiscreteMatrixKeyFrame[1];
                        bkf[0] = new System.Windows.Media.Animation.DiscreteMatrixKeyFrame(new System.Windows.Media.Matrix(1.0, 1.0, 2.0, 2.0, 1.0, 1.0));
                        return bkf;
                    }
                case "System.Windows.Media.Animation.ObjectKeyFrame":
                    {
                        return new System.Windows.Media.Animation.DiscreteObjectKeyFrame(new object());
                    }
                case "System.Windows.Media.Animation.ObjectKeyFrame[]":
                    {
                        System.Windows.Media.Animation.DiscreteObjectKeyFrame[] bkf = new System.Windows.Media.Animation.DiscreteObjectKeyFrame[1];
                        bkf[0] = new System.Windows.Media.Animation.DiscreteObjectKeyFrame(new object());
                        return bkf;
                    }
                case "System.Windows.Media.Animation.Point3DKeyFrame":
                    {
                        return new System.Windows.Media.Animation.DiscretePoint3DKeyFrame();
                    }
                case "System.Windows.Media.Animation.Point3DKeyFrame[]":
                    {
                        System.Windows.Media.Animation.DiscretePoint3DKeyFrame[] bkf = new System.Windows.Media.Animation.DiscretePoint3DKeyFrame[1];
                        bkf[0] = new System.Windows.Media.Animation.DiscretePoint3DKeyFrame();
                        return bkf;
                    }
                case "System.Windows.Media.Animation.PointKeyFrame":
                    {
                        return new System.Windows.Media.Animation.DiscretePointKeyFrame();
                    }
                case "System.Windows.Media.Animation.PointKeyFrame[]":
                    {
                        System.Windows.Media.Animation.DiscretePointKeyFrame[] bkf = new System.Windows.Media.Animation.DiscretePointKeyFrame[1];
                        bkf[0] = new System.Windows.Media.Animation.DiscretePointKeyFrame();
                        return bkf;
                    }
                case "System.Windows.Media.Animation.QuaternionKeyFrame":
                    {
                        return new System.Windows.Media.Animation.DiscreteQuaternionKeyFrame();
                    }
                case "System.Windows.Media.Animation.QuaternionKeyFrame[]":
                    {
                        System.Windows.Media.Animation.DiscreteQuaternionKeyFrame[] bkf = new System.Windows.Media.Animation.DiscreteQuaternionKeyFrame[1];
                        bkf[0] = new System.Windows.Media.Animation.DiscreteQuaternionKeyFrame();
                        return bkf;
                    }
                case "System.Windows.Media.Animation.RectKeyFrame":
                    {
                        return new System.Windows.Media.Animation.DiscreteRectKeyFrame();
                    }
                case "System.Windows.Media.Animation.RectKeyFrame[]":
                    {
                        System.Windows.Media.Animation.DiscreteRectKeyFrame[] bkf = new System.Windows.Media.Animation.DiscreteRectKeyFrame[1];
                        bkf[0] = new System.Windows.Media.Animation.DiscreteRectKeyFrame();
                        return bkf;
                    }
                case "System.Windows.Media.Animation.Rotation3DKeyFrame":
                    {
                        return new System.Windows.Media.Animation.DiscreteRotation3DKeyFrame();
                    }
                case "System.Windows.Media.Animation.Rotation3DKeyFrame[]":
                    {
                        System.Windows.Media.Animation.DiscreteRotation3DKeyFrame[] bkf = new System.Windows.Media.Animation.DiscreteRotation3DKeyFrame[1];
                        bkf[0] = new System.Windows.Media.Animation.DiscreteRotation3DKeyFrame();
                        return bkf;
                    }
                case "System.Windows.Media.Animation.SingleKeyFrame":
                    {
                        return new System.Windows.Media.Animation.DiscreteSingleKeyFrame();
                    }
                case "System.Windows.Media.Animation.SingleKeyFrame[]":
                    {
                        System.Windows.Media.Animation.DiscreteSingleKeyFrame[] bkf = new System.Windows.Media.Animation.DiscreteSingleKeyFrame[1];
                        bkf[0] = new System.Windows.Media.Animation.DiscreteSingleKeyFrame();
                        return bkf;
                    }
                case "System.Windows.Media.Animation.SizeKeyFrame":
                    {
                        return new System.Windows.Media.Animation.DiscreteSizeKeyFrame();
                    }
                case "System.Windows.Media.Animation.SizeKeyFrame[]":
                    {
                        System.Windows.Media.Animation.DiscreteSizeKeyFrame[] bkf = new System.Windows.Media.Animation.DiscreteSizeKeyFrame[1];
                        bkf[0] = new System.Windows.Media.Animation.DiscreteSizeKeyFrame();
                        return bkf;
                    }
                case "System.Windows.Media.Animation.StringKeyFrame":
                    {
                        return new System.Windows.Media.Animation.DiscreteStringKeyFrame();
                    }
                case "System.Windows.Media.Animation.StringKeyFrame[]":
                    {
                        System.Windows.Media.Animation.DiscreteStringKeyFrame[] bkf = new System.Windows.Media.Animation.DiscreteStringKeyFrame[1];
                        bkf[0] = new System.Windows.Media.Animation.DiscreteStringKeyFrame();
                        return bkf;
                    }
                case "System.Windows.Media.Animation.Storyboard":
                    {
                        return new System.Windows.Media.Animation.Storyboard();
                    }
                case "System.Windows.Media.Animation.ThicknessKeyFrame":
                    {
                        return new System.Windows.Media.Animation.DiscreteThicknessKeyFrame(new System.Windows.Thickness(3.0));
                    }
                case "System.Windows.Media.Animation.ThicknessKeyFrame[]":
                    {
                        System.Windows.Media.Animation.DiscreteThicknessKeyFrame [] tkf = new System.Windows.Media.Animation.DiscreteThicknessKeyFrame[1];
                        tkf[0] =  new System.Windows.Media.Animation.DiscreteThicknessKeyFrame(new System.Windows.Thickness(3.0));
                        return tkf;
                    }
                case "System.Windows.Media.Animation.TimeSeekOrigin":
                    {
                        return System.Windows.Media.Animation.TimeSeekOrigin.BeginTime;
                    }
                case "System.Windows.Media.Animation.Vector3DKeyFrame":
                    {
                        return new System.Windows.Media.Animation.DiscreteVector3DKeyFrame();
                    }
                case "System.Windows.Media.Animation.Vector3DKeyFrame[]":
                    {
                        System.Windows.Media.Animation.DiscreteVector3DKeyFrame[] bkf = new System.Windows.Media.Animation.DiscreteVector3DKeyFrame[1];
                        bkf[0] = new System.Windows.Media.Animation.DiscreteVector3DKeyFrame();
                        return bkf;
                    }
                case "System.Windows.Media.Animation.VectorKeyFrame":
                    {
                        return new System.Windows.Media.Animation.DiscreteVectorKeyFrame();
                    }
                case "System.Windows.Media.Animation.VectorKeyFrame[]":
                    {
                        System.Windows.Media.Animation.DiscreteVectorKeyFrame[] bkf = new System.Windows.Media.Animation.DiscreteVectorKeyFrame[1];
                        bkf[0] = new System.Windows.Media.Animation.DiscreteVectorKeyFrame();
                        return bkf;
                    }
                case "System.Windows.Media.Animation.KeySpline":
                    {
                        return new System.Windows.Media.Animation.KeySpline(0.1, 0.2, 0.3, 0.4);
                    }

                case "System.Windows.Media.Animation.KeyTime":
                    {
                        return System.Windows.Media.Animation.KeyTime.FromTimeSpan(new System.TimeSpan(0,0,0,0,4000));
                    }

                case "System.Windows.Media.Animation.FillBehavior":
                    {
                        return System.Windows.Media.Animation.FillBehavior.Stop;
                    }

                case "System.Windows.Media.Animation.PathAnimationSource":
                    {
                        return System.Windows.Media.Animation.PathAnimationSource.Angle;
                    }

                case "System.Windows.Media.Animation.ParallelTimeline" :
                    {
                        return new System.Windows.Media.Animation.ParallelTimeline();
                    }

                case "System.Windows.Media.Animation.PointAnimationBase":
                    {
                        return new PointModifier (new ModifierController (), 7.5, 13.2);
                    }

                case "System.Windows.Media.Animation.SizeAnimationBase":
                    {
                        return new SizeModifier (new ModifierController (), 7.5, 13.2);
                    }
                case "System.Windows.Media.Animation.BooleanAnimationBase":
                    {
                        return (firstOption) ? new BooleanModifier(new ModifierController(), true) : new BooleanModifier(new ModifierController(), false);
                    }
                case "System.Windows.Media.Animation.ByteAnimationBase":
                    {
                        return new ByteModifier(new ModifierController(), Convert.ToByte("0", System.Globalization.CultureInfo.InvariantCulture));
                    }
                case "System.Windows.Media.Animation.CharAnimationBase":
                    {
                        return new CharModifier(new ModifierController(), 'a');
                    }

                case "System.Windows.Media.Animation.DecimalAnimationBase":
                    {
                        return new DecimalModifier(new ModifierController(), 7.5m);
                    }

                case "System.Windows.Media.Animation.DoubleAnimationBase":
                    {
                        return new DoubleModifier (new ModifierController (), 7.5);
                    }
                case "System.Windows.Media.Animation.SingleAnimationBase":
                    {
                        return new SingleModifier(new ModifierController(), 7.5f);
                    }

                case "System.Windows.Media.Animation.Int32AnimationBase":
                    {
                        return new Int32Modifier(new ModifierController(), 1);
                    }
                case "System.Windows.Media.Animation.Int64AnimationBase":
                    {
                        return new Int64Modifier(new ModifierController(), 1);
                    }
                case "System.Windows.Media.Animation.Int16AnimationBase":
                    {
                        return new Int16Modifier(new ModifierController(), 1);
                    }
                case "System.Windows.Media.Animation.ColorAnimationBase":
                    {
                        return new ColorModifier (new ModifierController (), 0.2f, 0.1f, 0.1f, 0.1f);
                    }
                case "System.Windows.Media.Animation.MatrixAnimationBase":
                    {
                        return new MatrixModifier (new ModifierController (), 1.1, 0.2, 0.1, 2.0, 0.3, 0.4);
                    }
                case "System.Windows.Media.Animation.RectAnimationBase":
                    {
                        return new RectModifier (new ModifierController (), 7.5, 13.2, 12.4, 66.7);
                    }
                case "System.Windows.Media.Animation.SlipBehavior":
                    {
                        return System.Windows.Media.Animation.SlipBehavior.Grow; 
                    }
                case "System.Windows.Media.Animation.StringAnimationBase":
                    {
                        return new StringModifier(new ModifierController(), "a");
                    }
                case "System.Windows.Media.Animation.VectorAnimationBase":
                    {
                        return new VectorModifier(new ModifierController(), 7.5, 13.2);
                    }
                case "System.Windows.Media.Animation.Point3DAnimationBase":
                    {
                        return new Point3DModifier (new ModifierController (), 7.5, 13.2, 12.4);
                    }
                case "System.Windows.Media.Animation.Timeline":
                    {
                        return new System.Windows.Media.Animation.DoubleAnimation();
                    }
                case "System.Windows.Media.Animation.Timeline[]":
                    {
                        System.Windows.Media.Animation.Timeline[] timeline = new System.Windows.Media.Animation.Timeline[1];
                        timeline[0] = new System.Windows.Media.Animation.DoubleAnimation();
                        return timeline;
                    }
                case "System.Windows.Media.Animation.Vector3DAnimationBase":
                    {
                        return new Vector3DModifier (new ModifierController (), 7.5, 13.2, 12.4);
                    }
                case "System.Windows.Media.TextEffect":
                    {
                        return new System.Windows.Media.TextEffect();
                    }
                case "System.Windows.Media.TextEffect[]":
                    {
                        System.Windows.Media.TextEffect [] retval = new System.Windows.Media.TextEffect[1];
                        retval[0] = new System.Windows.Media.TextEffect();
                        return retval;
                    }
                default:
                    {
                        Console.WriteLine ("************* Do not know how to make object for " + name);
                        return null;
                    }
            }
        }



        //----------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        public static System.Windows.Media.PathSegmentCollection MakePathSegmentCollection ()
        {
            System.Windows.Media.PathSegmentCollection retval = new System.Windows.Media.PathSegmentCollection ();

            retval.Add (new System.Windows.Media.LineSegment (new System.Windows.Point (10.0, 20.0), false));
            retval.Add (new System.Windows.Media.LineSegment (new System.Windows.Point (100.0, 20.0), true));
            return retval;
        }
        //----------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        public static System.Windows.Media.PointCollection MakePointCollection()
        {
            // 6 points will do both for PolyQuadratic and PolyCubic Beziers
            System.Windows.Media.PointCollection retval = new System.Windows.Media.PointCollection();

            retval.Add(new System.Windows.Point(0.0, 0.0));
            retval.Add(new System.Windows.Point(10.0, 0.0));
            retval.Add(new System.Windows.Point(0.0, 10.0));
            retval.Add(new System.Windows.Point(0.0, 0.0));
            retval.Add(new System.Windows.Point(10.0, 0.0));
            retval.Add(new System.Windows.Point(0.0, 10.0));
            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        public static object MakeCollection (string name)
        {
            switch (name)
            {
                case "System.Windows.Media.Animation.BooleanKeyFrameCollection":
                    {
                        System.Windows.Media.Animation.DiscreteBooleanKeyFrame bokf = new System.Windows.Media.Animation.DiscreteBooleanKeyFrame();
                        System.Windows.Media.Animation.BooleanKeyFrameCollection retval = new System.Windows.Media.Animation.BooleanKeyFrameCollection();

                        retval.Add(bokf);
                        return retval;
                    }

                case "System.Windows.Media.Animation.ByteKeyFrameCollection":
                    {
                        System.Windows.Media.Animation.DiscreteByteKeyFrame bykf = new System.Windows.Media.Animation.DiscreteByteKeyFrame();
                        System.Windows.Media.Animation.ByteKeyFrameCollection retval = new System.Windows.Media.Animation.ByteKeyFrameCollection();

                        retval.Add(bykf);
                        return retval;
                    }

                case "System.Windows.Media.Animation.CharKeyFrameCollection":
                    {
                        System.Windows.Media.Animation.DiscreteCharKeyFrame chkf = new System.Windows.Media.Animation.DiscreteCharKeyFrame();
                        System.Windows.Media.Animation.CharKeyFrameCollection retval = new System.Windows.Media.Animation.CharKeyFrameCollection();

                        retval.Add(chkf);
                        return retval;
                    }
                case "System.Windows.Media.Animation.ClockCollection":
                    {
                        System.Windows.Media.Animation.ParallelTimeline timeline = new System.Windows.Media.Animation.ParallelTimeline();
                        System.Windows.Media.Animation.ClockGroup cg = timeline.CreateClock();
                        System.Windows.Media.Animation.ClockCollection cc = cg.Children;
                        return cc;
                    }
              
            
                case "System.Windows.Media.Animation.ColorKeyFrameCollection":
                    {
                        System.Windows.Media.Animation.DiscreteColorKeyFrame cokf = new System.Windows.Media.Animation.DiscreteColorKeyFrame();
                        System.Windows.Media.Animation.ColorKeyFrameCollection retval = new System.Windows.Media.Animation.ColorKeyFrameCollection();

                        retval.Add(cokf);
                        return retval;
                    }

                case "System.Windows.Media.Animation.DecimalKeyFrameCollection":
                    {
                        System.Windows.Media.Animation.DiscreteDecimalKeyFrame dekf = new System.Windows.Media.Animation.DiscreteDecimalKeyFrame();
                        System.Windows.Media.Animation.DecimalKeyFrameCollection retval = new System.Windows.Media.Animation.DecimalKeyFrameCollection();

                        retval.Add(dekf);
                        return retval;
                    }

                case "System.Windows.Media.Animation.DoubleKeyFrameCollection":
                    {
                        System.Windows.Media.Animation.DiscreteDoubleKeyFrame dukf = new System.Windows.Media.Animation.DiscreteDoubleKeyFrame();
                        System.Windows.Media.Animation.DoubleKeyFrameCollection retval = new System.Windows.Media.Animation.DoubleKeyFrameCollection();

                        retval.Add(dukf);
                        return retval;
                    }

                case "System.Windows.Media.Animation.Int16KeyFrameCollection":
                    {
                        System.Windows.Media.Animation.DiscreteInt16KeyFrame i16kf = new System.Windows.Media.Animation.DiscreteInt16KeyFrame();
                        System.Windows.Media.Animation.Int16KeyFrameCollection retval = new System.Windows.Media.Animation.Int16KeyFrameCollection();

                        retval.Add(i16kf);
                        return retval;
                    }

                case "System.Windows.Media.Animation.Int32KeyFrameCollection":
                    {
                        System.Windows.Media.Animation.DiscreteInt32KeyFrame i32kf = new System.Windows.Media.Animation.DiscreteInt32KeyFrame();
                        System.Windows.Media.Animation.Int32KeyFrameCollection retval = new System.Windows.Media.Animation.Int32KeyFrameCollection();

                        retval.Add(i32kf);
                        return retval;
                    }

                case "System.Windows.Media.Animation.Int64KeyFrameCollection":
                    {
                        System.Windows.Media.Animation.DiscreteInt64KeyFrame i64kf = new System.Windows.Media.Animation.DiscreteInt64KeyFrame();
                        System.Windows.Media.Animation.Int64KeyFrameCollection retval = new System.Windows.Media.Animation.Int64KeyFrameCollection();

                        retval.Add(i64kf);
                        return retval;
                    }

                case "System.Windows.Media.Animation.MatrixKeyFrameCollection":
                    {
                        System.Windows.Media.Animation.DiscreteMatrixKeyFrame mkf = new System.Windows.Media.Animation.DiscreteMatrixKeyFrame();
                        System.Windows.Media.Animation.MatrixKeyFrameCollection retval = new System.Windows.Media.Animation.MatrixKeyFrameCollection();

                        retval.Add(mkf);
                        return retval;
                    }

                case "System.Windows.Media.Animation.ObjectKeyFrameCollection":
                    {
                        System.Windows.Media.Animation.DiscreteObjectKeyFrame okf = new System.Windows.Media.Animation.DiscreteObjectKeyFrame();
                        System.Windows.Media.Animation.ObjectKeyFrameCollection retval = new System.Windows.Media.Animation.ObjectKeyFrameCollection();

                        retval.Add(okf);
                        return retval;
                    }

                case "System.Windows.Media.Animation.PointKeyFrameCollection":
                    {
                        System.Windows.Media.Animation.DiscretePointKeyFrame pkf = new System.Windows.Media.Animation.DiscretePointKeyFrame();
                        System.Windows.Media.Animation.PointKeyFrameCollection retval = new System.Windows.Media.Animation.PointKeyFrameCollection();

                        retval.Add(pkf);
                        return retval;
                    }

                case "System.Windows.Media.Animation.Point3DKeyFrameCollection":
                    {
                        System.Windows.Media.Animation.DiscretePoint3DKeyFrame p3dkf = new System.Windows.Media.Animation.DiscretePoint3DKeyFrame();
                        System.Windows.Media.Animation.Point3DKeyFrameCollection retval = new System.Windows.Media.Animation.Point3DKeyFrameCollection();

                        retval.Add(p3dkf);
                        return retval;
                    }

                case "System.Windows.Media.Animation.QuaternionKeyFrameCollection":
                    {
                        System.Windows.Media.Animation.DiscreteQuaternionKeyFrame qkf = new System.Windows.Media.Animation.DiscreteQuaternionKeyFrame();
                        System.Windows.Media.Animation.QuaternionKeyFrameCollection retval = new System.Windows.Media.Animation.QuaternionKeyFrameCollection();

                        retval.Add(qkf);
                        return retval;
                    }

                case "System.Windows.Media.Animation.RectKeyFrameCollection":
                    {
                        System.Windows.Media.Animation.DiscreteRectKeyFrame rkf = new System.Windows.Media.Animation.DiscreteRectKeyFrame();
                        System.Windows.Media.Animation.RectKeyFrameCollection retval = new System.Windows.Media.Animation.RectKeyFrameCollection();

                        retval.Add(rkf);
                        return retval;
                    }

                case "System.Windows.Media.Animation.Rotation3DKeyFrameCollection":
                    {
                        System.Windows.Media.Animation.DiscreteRotation3DKeyFrame r3dkf = new System.Windows.Media.Animation.DiscreteRotation3DKeyFrame();
                        System.Windows.Media.Animation.Rotation3DKeyFrameCollection retval = new System.Windows.Media.Animation.Rotation3DKeyFrameCollection();

                        retval.Add(r3dkf);
                        return retval;
                    }


                case "System.Windows.Media.Animation.SingleKeyFrameCollection":
                    {
                        System.Windows.Media.Animation.DiscreteSingleKeyFrame sikf = new System.Windows.Media.Animation.DiscreteSingleKeyFrame();
                        System.Windows.Media.Animation.SingleKeyFrameCollection retval = new System.Windows.Media.Animation.SingleKeyFrameCollection();

                        retval.Add(sikf);
                        return retval;
                    }

                case "System.Windows.Media.Animation.SizeKeyFrameCollection":
                    {
                        System.Windows.Media.Animation.DiscreteSizeKeyFrame szkf = new System.Windows.Media.Animation.DiscreteSizeKeyFrame();
                        System.Windows.Media.Animation.SizeKeyFrameCollection retval = new System.Windows.Media.Animation.SizeKeyFrameCollection();

                        retval.Add(szkf);
                        return retval;
                    }

                case "System.Windows.Media.Animation.StringKeyFrameCollection":
                    {
                        System.Windows.Media.Animation.DiscreteStringKeyFrame stkf = new System.Windows.Media.Animation.DiscreteStringKeyFrame();
                        System.Windows.Media.Animation.StringKeyFrameCollection retval = new System.Windows.Media.Animation.StringKeyFrameCollection();

                        retval.Add(stkf);
                        return retval;
                    }
                case "System.Windows.Media.Animation.ThicknessKeyFrameCollection":
                    {
                        System.Windows.Media.Animation.DiscreteThicknessKeyFrame thkf = new System.Windows.Media.Animation.DiscreteThicknessKeyFrame();
                        System.Windows.Media.Animation.ThicknessKeyFrameCollection retval = new System.Windows.Media.Animation.ThicknessKeyFrameCollection();

                        retval.Add(thkf);
                        return retval;
                    }
                    
                case "System.Windows.Media.Animation.TimelineCollection":
                    {
                        System.Windows.Media.Animation.ParallelTimeline ptl = new System.Windows.Media.Animation.ParallelTimeline();
                        System.Windows.Media.Animation.TimelineCollection retval = new System.Windows.Media.Animation.TimelineCollection();

                        retval.Add(ptl);
                        return retval;
                    }

                case "System.Windows.Media.Animation.VectorKeyFrameCollection":
                    {
                        System.Windows.Media.Animation.DiscreteVectorKeyFrame vkf = new System.Windows.Media.Animation.DiscreteVectorKeyFrame();
                        System.Windows.Media.Animation.VectorKeyFrameCollection retval = new System.Windows.Media.Animation.VectorKeyFrameCollection();

                        retval.Add(vkf);
                        return retval;
                    }

                case "System.Windows.Media.Animation.Vector3DKeyFrameCollection":
                    {
                        System.Windows.Media.Animation.DiscreteVector3DKeyFrame v3dkf = new System.Windows.Media.Animation.DiscreteVector3DKeyFrame(new System.Windows.Media.Media3D.Vector3D(1.0,2.0,3.0));
                        System.Windows.Media.Animation.Vector3DKeyFrameCollection retval = new System.Windows.Media.Animation.Vector3DKeyFrameCollection();

                        retval.Add(v3dkf);
                        return retval;
                    }

                case "System.Windows.Media.DoubleCollection":
                    {
                        System.Windows.Media.DoubleCollection retval = new System.Windows.Media.DoubleCollection();
                        retval.Add(2.0);
                        return retval;
                    }
                case "System.Windows.Media.Int32Collection":
                    {
                        System.Windows.Media.Int32Collection retval = new System.Windows.Media.Int32Collection();
                        retval.Add(1);
                        return retval;
                    }

                case "System.Windows.Media.TransformCollection":
                    {
                        System.Windows.Media.TransformCollection retval = new System.Windows.Media.TransformCollection ();

                        retval.Add (new System.Windows.Media.TranslateTransform (17.0, 3.0));
                        retval.Add (new System.Windows.Media.RotateTransform (30.0));
                        return retval;
                    }

                case "System.Windows.Media.GeometryGroup":
                    {
                        System.Windows.Media.GeometryGroup retval = new System.Windows.Media.GeometryGroup();
                        retval.Children.Add(new System.Windows.Media.EllipseGeometry(new System.Windows.Rect(10.0, 20.0, 150.0, 90.0)));
                        retval.Children.Add(new System.Windows.Media.EllipseGeometry(new System.Windows.Rect(20.0, 30.9, 150.1, 99.9)));
                        return retval;
                    }

                case "System.Windows.Media.CombinedGeometry":
                    {
                        System.Windows.Media.CombinedGeometry retval = new System.Windows.Media.CombinedGeometry(
                   System.Windows.Media.GeometryCombineMode.Xor,
                               new System.Windows.Media.EllipseGeometry(new System.Windows.Rect(10.0, 20.0, 150.0, 90.0)),
                               new System.Windows.Media.EllipseGeometry(new System.Windows.Rect(20.0, 30.9, 150.1, 99.9)));
                        return retval;
                    }

                case "System.Windows.Media.PathSegmentCollection":
                    {
                        return MakePathSegmentCollection ();
                    }

                case "System.Windows.Media.PathFigure":
                    {
                        return MakePathSegmentCollection ();
                    }
                case "System.Windows.Media.PathFigureCollection":
                    {
                        System.Windows.Media.PathFigureCollection retval = new System.Windows.Media.PathFigureCollection ();
                        retval.Add(new System.Windows.Media.PathFigure(new System.Windows.Point(0, 0), MakePathSegmentCollection(), false));
                        return retval;
                    }
                case "System.Windows.Media.PathGeometry":
                    {
                        System.Windows.Media.PathFigureCollection retval = new System.Windows.Media.PathFigureCollection ();

                        retval.Add(new System.Windows.Media.PathFigure(new System.Windows.Point(0, 0), MakePathSegmentCollection(), false));
                        return retval;
                    }
                case "System.Windows.Media.GradientStopCollection":
                    {
                        System.Windows.Media.GradientStopCollection retval = new System.Windows.Media.GradientStopCollection ();

                        retval.Add (new System.Windows.Media.GradientStop (System.Windows.Media.Color.FromScRgb (0.7f, 1.0f, 0.0f, 0.1f), 0.0));
                        retval.Add (new System.Windows.Media.GradientStop (System.Windows.Media.Color.FromScRgb (0.2f, 0.0f, 1.0f, 0.7f), 1.0));
                        return retval;
                    }
                case "System.Windows.Media.Media3D.Model3DCollection":
                    {
                        System.Windows.Media.Media3D.Model3DCollection retval = new System.Windows.Media.Media3D.Model3DCollection();
                        // Model3D is an abstract class, so use AmbientLight instead 
                        System.Windows.Media.Media3D.Model3D model3d = new System.Windows.Media.Media3D.AmbientLight();
                        retval.Add(model3d);
                        return retval;
                    }
                case "System.Windows.Media.Media3D.Model3DGroup":
                    {
                        System.Windows.Media.Media3D.Model3D[] model3d = new System.Windows.Media.Media3D.Model3D[1];

                        model3d[0] = new System.Windows.Media.Media3D.AmbientLight();
                        System.Windows.Media.Media3D.Model3DGroup group = new System.Windows.Media.Media3D.Model3DGroup();
                        group.Children = new System.Windows.Media.Media3D.Model3DCollection( model3d );
                        return group;
                    }

                case "System.Windows.Media.Media3D.Transform3DCollection":
                    {
                        System.Windows.Media.Media3D.Transform3D[] tx3d = new System.Windows.Media.Media3D.Transform3D[1];

                        tx3d[0] = new System.Windows.Media.Media3D.TranslateTransform3D(new System.Windows.Media.Media3D.Vector3D(10.0, 20.0, 3.0));
                        System.Windows.Media.Media3D.Transform3DCollection retval = new System.Windows.Media.Media3D.Transform3DCollection();
                        retval.Add(tx3d[0]);
                        return retval;
                    }
                case "System.Windows.Media.Media3D.Transform3DGroup":
                    {
                        System.Windows.Media.Media3D.Transform3D [] tx3d = new System.Windows.Media.Media3D.Transform3D[1];

                        tx3d[0] = new System.Windows.Media.Media3D.TranslateTransform3D(new System.Windows.Media.Media3D.Vector3D(10.0, 20.0, 3.0));
                        System.Windows.Media.Media3D.Transform3DGroup group = new System.Windows.Media.Media3D.Transform3DGroup();
                        group.Children = new System.Windows.Media.Media3D.Transform3DCollection(tx3d);
                        return group;
                    }

                // the below 3 cases can use the same data
                case "System.Windows.Media.PolyBezierSegment":
                case "System.Windows.Media.PolyLineSegment":
                case "System.Windows.Media.PolyQuadraticBezierSegment":
                    {
                        System.Windows.Media.PointCollection retval = new System.Windows.Media.PointCollection();
                        retval.Add(new System.Windows.Point(0.0, 0.0));
                        retval.Add(new System.Windows.Point(10.0, 10.0));
                        retval.Add(new System.Windows.Point(100.0, 100.0));
                        retval.Add(new System.Windows.Point(50.0, 50.0));
                        retval.Add(new System.Windows.Point(110.0, 110.0));
                        retval.Add(new System.Windows.Point(150.0, 150.0));
                        return retval;
                    }
                case "System.Windows.PointCollection":
                    {
                        return MakePointCollection();
                    }
                
                case "System.Windows.Media.Media3D.Point3DCollection":
                    {
                        System.Windows.Media.Media3D.Point3DCollection retval = new System.Windows.Media.Media3D.Point3DCollection();
                        retval.Add(new System.Windows.Media.Media3D.Point3D(1.0, 2.0, 3.0));
                        return retval;
                    }

                case "System.Windows.Media.TextEffectCollection":
                    {
                        System.Windows.Media.TextEffectCollection retval = new System.Windows.Media.TextEffectCollection();
                        System.Windows.Media.TextEffect te = new System.Windows.Media.TextEffect();
                        retval.Add(te);
                        return retval;
                    }

                case "System.Windows.Media.Media3D.Vector3DCollection":
                    {
                        System.Windows.Media.Media3D.Vector3DCollection retval = new System.Windows.Media.Media3D.Vector3DCollection();
                        retval.Add(new System.Windows.Media.Media3D.Vector3D(100.0, 200.0, 3.0));
                        return retval;
                    }

                case "System.Windows.Media.VectorCollection":
                    {
                        System.Windows.Media.VectorCollection retval = new System.Windows.Media.VectorCollection();
                        retval.Add(new System.Windows.Vector(1.2, 3.7));
                        return retval;
                    }

                default:
                    {
                        Console.WriteLine ("************* Do not know how to make object collection for " + name);
                        return null;
                    }
            }
        }

        //----------------------------------
        private static string s_jpegFile = "file://" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +  "/borg.jpg";

    }
}
