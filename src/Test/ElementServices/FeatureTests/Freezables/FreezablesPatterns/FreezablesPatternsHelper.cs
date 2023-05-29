// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2004
 *
 *   Program:   FreezablesPatternsHelper class
 
 *
 ************************************************************/

using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.ElementServices.Freezables.Objects;


namespace Microsoft.Test.ElementServices.Freezables
{

    /**********************************************************************************
    * CLASS:          FreezablesPatternsHelper
    **********************************************************************************/
    public class FreezablesPatternsHelper
    {

        #region Static Members
        /******************************************************************************
        * Function:          CheckNotChangeable
        ******************************************************************************/
        /// <summary>
        /// Checks if the object is immutable (IsFrozen == true).
        /// </summary>
        /// <param name="obj">The Freezable object to be tested.</param>
        /// <returns>A boolean indicating whether or not the Freezable is immutable.</returns>
        internal static bool CheckNotChangeable(Freezable obj)
        {
            if (obj.IsFrozen)
            {
                return true;
            }
            else
            {
                GlobalLog.LogEvidence("%%%   FAIL ON CHECK NotChangeable STATE:{0} %%%", obj.IsFrozen);
                return false;
            }
        }

        /******************************************************************************
        * Function:          CheckDispatcher
        ******************************************************************************/
        /// <summary>
        /// Checks Dispatcher context.
        /// </summary>
        /// <param name="t">The Type of the Freezable object to be tested.</param>
        /// <param name="obj">The Freezable object to be tested.</param>
        /// <param name="context">The Dispatcher context.</param>
        /// <returns>A boolean indicating whether or not the Dispatcher matches the context passed in.</returns>
        internal static bool CheckDispatcher(Type t, Freezable obj, Dispatcher context)
        {
            if (obj.Dispatcher == context)
            {
                return true;
            }
            else
            {
                GlobalLog.LogEvidence("%%% Fail on Comparing Dispatcher: {0} %%%", t);
                return false;
            }
        }


        /******************************************************************************
        * Function:          CheckNullDispatcher
        ******************************************************************************/
        /// <summary>
        /// Checks for null Dispatcher.
        /// </summary>
        /// <param name="t">The Type of the Freezable object to be tested.</param>
        /// <param name="obj">The Freezable object to be tested.</param>
        /// <returns>A boolean indicating whether or not the Dispatcher is null.</returns>
        internal static bool CheckNullDispatcher(Type t, Freezable obj)
        {
            if (obj.Dispatcher == null)
            {
                return true;
            }
            else
            {
                GlobalLog.LogEvidence("%%% Dispatcher is not null, after calling Freeze: {0}, {1} %%%", t, obj.Dispatcher);

                return false;
            }
        }

        #endregion

        /******************************************************************************
        * Function:          CreateNewChangeable
        ******************************************************************************/
        /// <summary>
        /// Creates a new Freezable object.
        /// </summary>
        /// <param name="t">The Type of the Freezable object to be tested.</param>
        /// <returns>A Freezable object.</returns>
        internal static Freezable CreateNewChangeable (Type t)
        {
                if (t.IsAbstract)
                {
                    return null;
                }

                Freezable retval = null;
                ConstructorInfo[] ci = t.GetConstructors ();
           
                // for each constructor try to construct the object
                // if successfull, return the object, else keep trying
                // unit the loop is terminated
                for (int i = 0; i < ci.Length; i++)
                {
                    try
                    {
                        retval = FreezablesPatternsHelper.MakeChangeableObject(t, ci , i);
                        if (retval != null)
                        {
                            return retval;
                        }
                    }
                    catch (System.Reflection.TargetInvocationException e)
                    {
                        if (!e.InnerException.ToString ().StartsWith ("System.ArgumentNullException"))
                        {
                            GlobalLog.LogEvidence ("        !!! NewObject: Exception: {0}", e.InnerException.ToString ());
                        }

                        return null;
                    }
                }

                return retval;
        }

        /******************************************************************************
        * Function:          MakeChangeableObject
        ******************************************************************************/
        /// <summary>
        /// Creates a new Freezable object, based on Predefined objects.
        /// </summary>
        /// <param name="t">The Type of the Freezable object to be tested.</param>
        /// <param name="ci">Parameter info.</param>
        /// <param name="index">Index for the Parameter info.</param>
        /// <returns>A Freezable object.</returns>
        internal static Freezable MakeChangeableObject(Type t, ConstructorInfo [] ci , int index)
        {
            ParameterInfo[] pi = ci[index].GetParameters();
            Object[] args = new Object[pi.Length]; // the constructor arguments
            // for each parameter

            for (int j = 0; j < pi.Length; j++)
            {
                if (j == pi[j].Position) // verify the position of the arguments
                {
                    GlobalLog.LogStatus("Object: "  + j.ToString() + "--" + pi[j].ParameterType.FullName);

                    if (pi[j].ParameterType.FullName.ToString() == "System.Collections.ICollection"
                        || pi[j].ParameterType.FullName.StartsWith("System.Collections.Generic.IEnumerable"))
                    {
                        args[j] = PredefinedObjects.MakeCollection(t.FullName.ToString());
                        if (args[j] == null)
                        {
                            return null;
                        }
                    }
                    else
                    {
                        args[j] = PredefinedObjects.MakeValue(pi[j].ParameterType);
                    }
                }
            }
            ModifyArgsIfSpecialCase(t, pi, args);

            Freezable obj = (Freezable)t.InvokeMember(null, BindingFlags.Public | BindingFlags.Instance | BindingFlags.CreateInstance, null, null, args);
          
            return obj;
        }

        /******************************************************************************
        * Function:          GetComplexChangeable
        ******************************************************************************/
        /// <summary>
        /// Creates a new 'complex' Freezable object.
        /// </summary>
        /// <param name="pi">Property info.</param>
        /// <param name="obj">A Freezable object.</param>
        /// <returns>A Freezable object.</returns>
        internal static Freezable GetComplexChangeable(PropertyInfo pi, Freezable obj)
        {
            if (pi.CanRead && !TypeHelper.IsType(pi.DeclaringType, "Freezable"))
            {
                // if complex freezable ECT
                if (TypeHelper.IsFreezable(pi.PropertyType))
                {
                    //  Object[] index = null;
                    if (pi.Name == "Item" && ((System.Collections.ICollection)obj).Count == 0)
                    {
                        // Some Animation collection classes does not have public constuctor
                        // that enable us to construct properly. This is the work around for 
                        // the OutOfRange exception.
                        return null;
                    }

                    // get the object
                    Freezable complexChangeable = (Freezable)pi.GetValue(obj, null);

                    return complexChangeable;
                }

                return null;
            }

            return null;
        }
        #region SpecialCases

        /******************************************************************************
        * Function:          IsSpecialCase
        ******************************************************************************/
        /// <summary>
        /// Creates a new 'complex' Freezable object.
        /// </summary>
        /// <param name="t">The Type of a Freezable object.</param>
        /// <param name="obj">A Freezable object.</param>
        /// <returns>A boolean, indicating whether or not the Freezable is a special case.</returns>
        internal static bool IsSpecialCase(Type t, Freezable obj)
        {
            // Special case, Cannot call Freeze on the Setter object
            // whose target and property are null
            /*if (t.ToString() == "System.Windows.Media.Animation.Setter" && ((Animation.Setter)obj).Target == null && ((Animation.Setter)obj).Property == null)
            {
                return true;
            }
            else */
            if (t.ToString() == "System.Windows.Media.VisualTargetSource")
            {
                return true;
            }
            else if (t.ToString() == "System.Windows.Media.Imaging.BitmapSource+ImageSourceNull")
            {
                return true;
            }

            return false;
        }
  

        /******************************************************************************
        * Function:          ModifyArgsIfSpecialCase
        ******************************************************************************/
        /// <summary>
        /// Creates a new 'complex' Freezable object.
        /// </summary>
        /// <param name="t">The Type of a Freezable object.</param>
        /// <param name="pi">Parameter info.</param>
        /// <param name="args">Ana array of objects.</param>
        /// <returns></returns>
        private static void ModifyArgsIfSpecialCase(Type t, ParameterInfo[] pi, Object[] args)
        {
            if (t.ToString() == "System.Windows.Media.Imaging.BitmapImage" && pi.Length == 10)
            {
                // The 8th paramter (stride) of BitmapImage constructor needs value stride = Width * BytePerpixel
                // Ctor forImageData:
                // (Int32 pixelWidth, Int32 pixelHeight, Double dpiX, Double dpiY, PixelFormat pixelFormat, BitmapPalette imagePalette, Byte[] pixels, Int32 stride, Int32Rect sourceRect, BitmapSizeOptions sizeOptions)
                PixelFormat p = (PixelFormat)args[4];
                args[6] = new byte[32];
                args[7] = (Int32)args[0] * p.BitsPerPixel / 8;
            }
            else if (t.ToString() == "System.Windows.Media.Animation.KeySpline" && pi.Length == 2 && pi[0].ParameterType.FullName.ToString() == "System.Windows.Point" && pi[1].ParameterType.FullName.ToString() == "System.Windows.Point")
            {
                // For Keyspline, points must be betwen 0.0 and 1.0
                args[0] = new Point(0.1, 0.2);
                args[1] = new Point(0.2, 0.7);
            }
            else if (t.ToString() == "System.Windows.Media.Animation.Setter" && pi.Length == 3)
            {
                // For animation Setter, generic objects from PredefinesObject will not work.
                args[0] = new LineGeometry(new Point(10, 10), new Point(20, 20)); //DependencyObject();
                args[1] = LineGeometry.StartPointProperty;
                args[2] = new Point(1.0, 2.0);
            }
            else if (t.ToString() == "System.Windows.Media.Imaging.BitmapMetadata")
            {
                // The format of the bitmap image, specified as "gif", "jpeg", "png", or "tiff".
                args[0] = "jpeg";
            }
            else if (t.ToString() == "System.Windows.Media.Imaging.RenderTargetBitmap" && pi.Length == 5)
            {
                // pixelWidth and pixelHeight must be greater then 0
                args[0] = 1;
                args[1] = 1;
            }
        }

        /******************************************************************************
        * Function:          IsSpecialFreezable
        ******************************************************************************/
        /// <summary>
        /// Checks if the Freezable falls into a 'SpecialFreezable' category.
        /// </summary>
        /// <param name="t">The Type of a Freezable object.</param>
        /// <returns>A boolean, indicating whether or not the Type is a 'Special Freezable'.</returns>
        internal static bool IsSpecialFreezable(Type t)
        {
            switch (t.ToString())
            {
                case "System.Windows.Media.DrawingImage":
                case "System.Windows.Media.Imaging.BitmapImage":
                case "System.Windows.Media.Imaging.CroppedBitmap":
                case "System.Windows.Media.Imaging.FormatConvertedBitmap":
                case "System.Windows.Media.Imaging.TransformedBitmap":
                    return true;
       
                default:
                    return false;
            }
        }

        /******************************************************************************
        * Function:          CreateSpecialFreezable
        ******************************************************************************/
        /// <summary>
        /// Creates a 'SpecialFreezable'.
        /// </summary>
        /// <param name="t">The Type of a Freezable object.</param>
        /// <returns>A Freezable.</returns>
        internal static Freezable CreateSpecialFreezable(Type t)
        {
            string jpegFile = "25DPI.jpg";
            switch (t.ToString())
            {
                case "System.Windows.Media.DrawingImage":
                {
                    DrawingGroup drawingGroup = new DrawingGroup();
                    drawingGroup.Opacity = 0.5;
                    drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(new Point(-1.5, -1.5), new Point(1.5, 1.5)));

                    DrawingImage drawingImage = new DrawingImage();
                    drawingImage.Drawing = drawingGroup;
                    return (Freezable)drawingImage;
                }
                case "System.Windows.Media.Imaging.BitmapImage":
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(jpegFile, UriKind.RelativeOrAbsolute);
                    image.EndInit();
                    return (Freezable)image;
                }
                case "System.Windows.Media.Imaging.CroppedBitmap":
                {
                    BitmapImage image = new BitmapImage(new Uri(jpegFile, UriKind.RelativeOrAbsolute));
                    CroppedBitmap crop = new CroppedBitmap();

                    Int32Rect rect = new Int32Rect(0, 0, 5, 5);

                    crop.BeginInit();
                    crop.SourceRect = rect;
                    crop.Source = image;
                    crop.EndInit();
                    return (Freezable)crop;
                }
                case "System.Windows.Media.Imaging.FormatConvertedBitmap":
                {
                    BitmapImage image = new BitmapImage(new Uri(jpegFile, UriKind.RelativeOrAbsolute));
                    FormatConvertedBitmap fmt = new FormatConvertedBitmap();
                    fmt.BeginInit();
                    fmt.Source = image;
                    fmt.DestinationFormat = PixelFormats.Gray32Float;
                    fmt.EndInit();
                    return (Freezable)fmt;
                }
                case "System.Windows.Media.Imaging.TransformedBitmap":
                {
                    BitmapImage image = new BitmapImage(new Uri(jpegFile, UriKind.RelativeOrAbsolute));
                    TransformedBitmap tb = new TransformedBitmap();
                    tb.BeginInit();
                    tb.Source = image;
                    tb.Transform = new RotateTransform(90);
                    tb.EndInit();
                    return (Freezable)tb;
                }
                default:
                    throw new ApplicationException("Unknown Freezable object: " + t.ToString());
            }
        }
        #endregion
     }
}
