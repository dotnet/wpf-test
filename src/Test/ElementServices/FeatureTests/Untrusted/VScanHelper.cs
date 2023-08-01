// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Visual verification wrappers.
 * 
 * Contributor: Microsoft
 *
 
  
 * Revision:         $Revision: 3 $
 
********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using System.Windows;
using System.Drawing;
using System.Windows.Interop;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;   // Logger

using Microsoft.Test.RenderingVerification;
using Microsoft.Test.RenderingVerification.Model.Analytical;

namespace Avalon.Test.CoreUI.Common
{
    /// <summary>
    /// VScanProfile blatantly copied from MIL test. Defines useful values for image comparison histogram.
    /// </summary>
    internal class VScanProfile
    {
        public enum Profile { Good, Exact, Poor, Quality0, Quality1, Quality2, QualityCT, QualityHWSW, QualityVisualTransform }

        public static SortedList GetProfile(Profile p)
        {
            SortedList retVal = new SortedList();
            switch (p)
            {
                case Profile.QualityVisualTransform:
                    {
                        retVal.Add(0.0, 1.0);
                        retVal.Add(8.0, 0.0146);
                        retVal.Add(12.0, 0.00326);
                        retVal.Add(18.0, 0.00116);
                        retVal.Add(50.0, 0.000186);
                        return retVal;
                    }

                case Profile.QualityHWSW:
                    {
                        retVal.Add(0.0, 1.0);
                        retVal.Add(2.0, 0.03);
                        retVal.Add(18.0, 0.01);
                        retVal.Add(25.0, 0.0003);
                        retVal.Add(35.0, 0.0002);
                        retVal.Add(45.0, 0.0001);
                        retVal.Add(55.0, 0.00001);
                        return retVal;
                    }

                case Profile.QualityCT:
                    {
                        retVal.Add(0.0, 1.0);
                        retVal.Add(2.0, 0.01);
                        retVal.Add(3.0, 0.0005);
                        retVal.Add(15.0, 0.0003);
                        retVal.Add(25.0, 0.0002);
                        retVal.Add(35.0, 0.0001);
                        retVal.Add(45.0, 0.00001);
                        return retVal;
                    }

                case Profile.Poor:
                    {
                        retVal.Add(0.0, 1.0);
                        retVal.Add(2.0, 0.01);
                        return retVal;
                    }

                case Profile.Good:
                    {
                        retVal.Add(0.0, 1.0);
                        retVal.Add(2.0, 0.0005);
                        retVal.Add(10.0, 0.0004);
                        retVal.Add(15.0, 0.0003);
                        retVal.Add(25.0, 0.0002);
                        retVal.Add(35.0, 0.0001);
                        retVal.Add(45.0, 0.00001);
                        return retVal;
                    }

                case Profile.Exact:
                    {
                        retVal.Add(0.0, 0.0);
                        return retVal;
                    }

                case Profile.Quality2:
                    {
                        retVal.Add(0.0, 1.0);
                        retVal.Add(2.0, 0.0005);
                        retVal.Add(10.0, 0.0004);
                        retVal.Add(15.0, 0.0003);
                        retVal.Add(25.0, 0.0002);
                        retVal.Add(35.0, 0.0001);
                        retVal.Add(45.0, 0.00001);
                        return retVal;
                    }

                case Profile.Quality1:
                    {
                        retVal.Add(1.0, 1.0);
                        retVal.Add(2.0, 0.0);
                        return retVal;
                    }

                case Profile.Quality0:
                    {
                        retVal.Add(0.0, 0.0);
                        return retVal;
                    }

                default:
                    {
                        throw new ApplicationException("Undefined Profile ");
                    }
            }
        }
    }


    /// <summary>
    /// RenderingVerification wrapper for Core tests.
    /// </summary>
    internal class VScanHelper
    {
        //
        // Screen capture wrappers.
        //

        /// <summary>
        /// Capture an image.
        /// </summary>
        /// <returns>IImageAdaptor for comparison.</returns>
        public static IImageAdapter Capture(Window win)
        {
            WindowInteropHelper iwh = new WindowInteropHelper(win);
            return Capture(iwh.Handle);
        }

        /// <summary>
        /// Capture an image.
        /// </summary>
        /// <returns>IImageAdaptor for comparison.</returns>
        public static IImageAdapter Capture(IntPtr windowHandle)
        {
            return new ImageAdapter(windowHandle, true /* Client area only. */);
        }

        /// <summary>
        /// Capture an image.
        /// </summary>
        /// <returns>IImageAdaptor for comparison.</returns>
        public static IImageAdapter Capture(IntPtr windowHandle, bool clientAreaOnly)
        {
            return new ImageAdapter(windowHandle, clientAreaOnly);
        }

        /// <summary>
        /// Capture an image.
        /// </summary>
        /// <returns>IImageAdaptor for comparison.</returns>
        public static IImageAdapter Capture(string fileName)
        {
            return new ImageAdapter(fileName);
        }

        //
        // Image comparison wrappers.
        //

        /// <summary>
        /// Compare a vscan master-image file to an Avalon window.
        /// </summary>
        /// <param name="vscanFile">VScan file</param>
        /// <param name="win">Window</param>
        /// <returns>Success or failure of comparison</returns>
        public static bool Compare(string vscanFile, Window win)
        {
            WindowInteropHelper iwh = new WindowInteropHelper(win);

            return Compare(vscanFile, iwh.Handle);
        }

        /// <summary>
        /// Compare a vscan master-image file to an Hwnd.
        /// </summary>
        /// <param name="vscanFile">VScan file</param>
        /// <param name="handle">Hwnd</param>
        /// <returns>Success or failure of comparison</returns>
        public static bool Compare(string vscanFile, IntPtr handle)
        {
            IImageAdapter vscanImage = Capture(vscanFile);

            IImageAdapter screenImage = Capture(handle);

            return Compare(vscanImage, screenImage, VScanProfile.Profile.QualityVisualTransform);
        }

        //
        // Image comparison.
        //

        /// <summary>
        /// Compare two images using the specified VScanProfile. If comparison fails or there is
        /// an error comparing the images a vscan package is created for analysis.
        /// </summary>
        /// <param name="image1">First image</param>
        /// <param name="image2">Second image</param>
        /// <param name="profile">VScanProfile.Profile specifying tolerance curve.</param>
        /// <returns>Success or failure of comparison.</returns>
        public static bool Compare(IImageAdapter image1, IImageAdapter image2, VScanProfile.Profile profile)
        {
            ImageComparator comparator = new ImageComparator();

            // ImageComparator functions fail for non default dpiRatios
            System.Drawing.Graphics desktopInfo = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
            CoreLogger.LogStatus("DpiX " + desktopInfo.DpiX);
            if (Math.Abs(desktopInfo.DpiX - 96) > 0.01)
            {
                double dpiRatio = desktopInfo.DpiX / 96.0;
                CoreLogger.LogStatus("Ignoring test for dpiRatio " + dpiRatio.ToString());
                return true;
            }

            // Common values.
            comparator.FilterLevel = 1;
            comparator.ChannelsInUse = ChannelCompareMode.ARGB;

            // Create tolerance curve.
            comparator.Curve.CurveTolerance.Entries.Clear();
            SortedList sl = VScanProfile.GetProfile(profile);
            for(int t = 0; t < sl.Count; t++)
            {
                object x = sl.GetKey(t);
                double y = (double)sl[x];
                comparator.Curve.CurveTolerance.Entries.Add(Convert.ToByte(x), y);
            }

            //
            // Do comparison and save vscan package on failure.
            //

            bool match = false;
            try
            {
                match = comparator.Compare(image1, image2);

                if (!match)
                {
                    CoreLogger.LogStatus("Images do not match. See package...", ConsoleColor.Red);
                }
                else
                {
                    CoreLogger.LogStatus("Images are an exact match.", ConsoleColor.Green);
                    return true;
                }
            }
            catch (Exception e)
            {
                match = false;
                CoreLogger.LogStatus("Error comparing images: " + e.Message + e.StackTrace);
                CoreLogger.LogTestResult(false, "Failed, See the StackTrace and package ...");
            }
            finally
            {
                // Save package if match failed or there was an error comparing.
                if (!match)
                {
                    // Create a Package object for autopsy.
                    string packageName = ".\\__" + Path.ChangeExtension(Path.GetRandomFileName(), ".vscan");

                    Package package = Package.Create(packageName, ImageUtility.ToBitmap(image1), ImageUtility.ToBitmap(image2));
                    package.Save();
                    CoreLogger.LogStatus("Wrote package to " + packageName);

                    Microsoft.Test.Logging.GlobalLog.LogFile(packageName);
                }
            }

            return false;
            
        }
    }

    /// <summary>
    /// Class for recording a series of screenshots for easy in order comparison 
    /// against another series later.
    /// </summary>
    internal class VScanRecorder
    {
        int _currentFrame;
        List<IImageAdapter> _frames;

        public VScanRecorder()
        {
            _currentFrame = 0;
            _frames = new List<IImageAdapter>();
        }

        /// <summary>
        /// Save a screenshot.
        /// </summary>
        public void Record(IImageAdapter capture)
        {
            _frames.Add(capture);

            // Note, recording does not affect current frame, 
            // new frames may be captured after comparison of first image.
        }

        /// <summary>
        /// Compare current screenshot against current frame then advance current frame.
        /// </summary>
        /// <remarks>
        /// Current frame will advance to next capture even if comparison fails.
        /// </remarks>
        /// <returns>
        /// Success or failure of comparison.
        /// </returns>
        public bool Compare(IImageAdapter capture)
        {
            if (_currentFrame >= _frames.Count)
            {
                throw new Exception("No image captured with this index, " + _currentFrame);
            }

            bool result = VScanHelper.Compare(_frames[_currentFrame], capture, VScanProfile.Profile.QualityVisualTransform);

            _currentFrame++;

            return result;
        }

        /// <summary>
        /// Reset current frame to the beginning.
        /// </summary>
        public void Rewind()
        {
            _currentFrame = 0;
        }

        /// <summary>
        ///  Clear all stored frames and reset frame count.
        /// </summary>
        public void Clear()
        {
            _frames.Clear();
            Rewind();
        }
    }
 }










