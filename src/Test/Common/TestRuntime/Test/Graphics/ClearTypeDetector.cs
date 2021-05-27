// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

﻿/******************************************************************* 
 * Purpose: ClearTypeHint testing
 ********************************************************************/
using System.Drawing;

namespace Microsoft.Test.Graphics
{
    public static class ClearTypeDetector
    {

        const int GREYLEVELQUANTIZATION = 256;
        const int REDBLUEDIFFERENCEMINIMUM = 10;
        #region helper functions
        /// <summary>
        /// Default - no left/ right start/stop column specified
        /// </summary>
        /// <param name="rowToScan">The row of the image in which to look for cleartype-style colour dispersion.</param>
        /// <returns>TestResult</returns>
        static public bool CheckForRedBlueShift(Bitmap capture, int rowToScan)
        {       
            return CheckForRedBlueShift(capture, rowToScan, 0, 0);
        }
        /// Determine if the rendered image has cleartype rendering or not by looking for non-monochromatic colors in output.
        /// This relies on the text brush and the background being monochromatic and the same hue.
        /// </summary>
        /// <param name="rowToScan">The row of the image in which to look for cleartype-style colour dispersion.</param>
        /// <returns>TestResult</returns>
        static public bool CheckForRedBlueShift(Bitmap capture, int rowToScan, int ignoredColumnsOnLeft, int ignoredColumnsOnRight )
        {

            bool redDetected = false;
            bool blueDetected = false;
            int x, y;

            System.Windows.Point ptFrom, ptTo;
            ptFrom = new System.Windows.Point(ignoredColumnsOnLeft, rowToScan);
            ptTo = new System.Windows.Point(capture.Width-ignoredColumnsOnRight, rowToScan);

            //scan across the given line of the image (ideally midline of target text block)
            //blacken non-interesting pixels; saturate interesting ones
            double dx = ptTo.X - ptFrom.X;
            double dy = ptTo.Y - ptFrom.Y;
            double slope = dy / dx;
            y = (int)ptFrom.Y;
            System.Drawing.Color c;
            for (x = (int)ptFrom.X; x < (int)ptTo.X; x++)
            {
                c = capture.GetPixel(x, y);
                if (c.R - c.B > REDBLUEDIFFERENCEMINIMUM) // if the pixel is more red than blue
                {
                    capture.SetPixel(x, y, System.Drawing.Color.Red);
                    redDetected = true;
                }
                else
                    if ((c.B - c.R > REDBLUEDIFFERENCEMINIMUM)) // if the pixel is more blue than red
                    {
                        capture.SetPixel(x, y, System.Drawing.Color.Blue);
                        blueDetected = true;
                    }
                    else
                    {
                        capture.SetPixel(x, y, System.Drawing.Color.Black);
                    }
                y = (int)((float)y + slope);
            }

            //if we found both red and blue, we say that this was cleartype
            return (redDetected && blueDetected);
        }

        /// <summary>
        /// Determine if the rendered image has cleartype rendering or not by looking for non-monochromatic colors in output.
        /// This relies on the text brush and the background being monochromatic and the same hue.
        /// </summary>
        /// <param name="rowToScan">The row of the image in which tocollect and bin grey levels</param>
        /// <returns>TestResult</returns>
        static public int CountGreyLevels(Bitmap capture, int rowToScan)
        {
           
            int x, y;
            int[] greyLevels = new int[GREYLEVELQUANTIZATION];//we're going to quantize
            for(int i=0;i<GREYLEVELQUANTIZATION;i++)
            {
                greyLevels[i]=0;
            }            

            System.Windows.Point ptFrom, ptTo;
            ptFrom = new System.Windows.Point(0, rowToScan);
            ptTo = new System.Windows.Point(capture.Width, rowToScan);

            //scan across the given line of the image (ideally midline of target text block)            
            double dx = ptTo.X - ptFrom.X;
            double dy = ptTo.Y - ptFrom.Y;
            double slope = dy / dx;
            y = (int)ptFrom.Y;
            System.Drawing.Color c;
            int g;
            for (x = (int)ptFrom.X; x < (int)ptTo.X; x++)
            {
                c = capture.GetPixel(x, y);		
                g=(int)(c.GetBrightness()*GREYLEVELQUANTIZATION);
                greyLevels[g]++;

                y = (int)((float)y + slope);
            }

            //now count how many levels of grey we detected.
            int numberOfGreyLevels=0;
            for(int i=0;i<GREYLEVELQUANTIZATION;i++)
            {
                if(greyLevels[i]>0)numberOfGreyLevels++;
            }  
            return numberOfGreyLevels;
        }
        #endregion

    }

}

