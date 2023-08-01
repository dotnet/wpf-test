// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************************
*    Purpose:  Generates a series of points interpolated from end points.
*    
 
  
*    Revision:         $Revision: 2 $
 
******************************************************************************/
using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Security;
using System.Security.Permissions;
using Microsoft.Test.Win32;
using Microsoft.Test.Threading;

namespace Avalon.Test.CoreUI.Trusted
{
    /// <summary>
    /// Generates a series of points interpolated from end points.
    /// </summary>
    public static class InterpolationHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public static Point[] GetPoints(int x1, int y1, int x2, int y2)
        {
            return InterpolationHelper.GetPoints(x1, y1, x2, y2, 100);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        public static Point[] GetPoints(int x1, int y1, int x2, int y2, int maximum)
        {
            int slowBuffer = 20;

            Point[] allPoints = InterpolationHelper.GetAllPoints(x1,y1,x2,y2);

            //
            // After getting every point between start and end, we
            // reduce the number of points by skipping some.
            // To calculate the step size,
            // Divide the total points (minus the slow buffer)
            // by the desired maximum points (minus the slow buffer).
            // Force the step size to be at least 1.
            //
            double skipPoints = ((double)allPoints.Length - slowBuffer) / ((double)(maximum - slowBuffer));
            skipPoints = skipPoints < 1.0 ? 1.0 : skipPoints;

            double i = 0.0;
            List<Point> points = new List<Point>(maximum);

            // 
            // Add points to the final list from the fast portion of 
            // the total points, skipping some points each iteration. 
            // Stop when we reach the slow portion of the total points.
            //
            while (i < allPoints.Length - slowBuffer - 1)
            {
                points.Add(allPoints[(int)i]);
                i += skipPoints;
            }

            // Set index to start of slow portion.
            i = allPoints.Length - slowBuffer - 1;
            i = i < 0.0 ? 0.0 : i;

            //
            // Add every point to the final list from the slow portion
            // of the total points (the last 'slowBuffer' points).
            // 
            while (i < allPoints.Length)
            {
                points.Add(allPoints[(int)i]);
                i += 1.0;
            }
            
            return points.ToArray();
        }

        
        /// <summary>
        /// Construct a list of points between a start point and an end point.
        /// </summary>
        /// <param name="x1">x coordinate of start point.</param>
        /// <param name="y1">y coordinate of start point.</param>
        /// <param name="x2">x coordinate of end point.</param>
        /// <param name="y2">y coordinate of end point.</param>
        /// <returns>An array of points between (x1,y1) and (x2,y2).</returns>
        public static Point[] GetAllPoints(int x1, int y1, int x2, int y2)
        {
            // Bresenham's line interpolation algorithm:
            // http://web.archive.org/web/19981203145759/http://intranet.ca/~sshah/waste/art7.html
            int x = x1;
            int y = y1;

            Debug.WriteLine("Start point at " + x + "," + y);

            // Calculate delta values to use
            int dx = x2 - x1;
            int dy = y2 - y1;

            // Change signs of delta values if necessary
            int xChange;

            if (dx < 0)
            {
                xChange = -1;
                dx = -dx;
            }
            else
            {
                xChange = 1;
            }

            int yChange;

            if (dy < 0)
            {
                yChange = -1;
                dy = -dy;
            }
            else
            {
                yChange = 1;
            }

            // We log the interpolated points to a generic collection.
            int error = 0;
            int i = 0;
            int length = 0;
            ArrayList pointsList = new ArrayList();

            // Create the path
            if (dx < dy)
            {
                length = dy;
                while (i < length)
                {
                    y += yChange;
                    error += dx;
                    if (error > dy)
                    {
                        x += xChange;
                        error -= dy;
                    }

                    pointsList.Add(new Point(x, y));
                    i++;
                }
            }
            else
            {
                length = dx;
                while (i < length)
                {
                    x += xChange;
                    error += dy;
                    if (error > dx)
                    {
                        y += yChange;
                        error -= dx;
                    }

                    pointsList.Add(new Point(x, y));
                    i++;
                }
            }

            Debug.WriteLine("End point at " + x + "," + y);
            Debug.WriteLine("pointList length: " + pointsList.Count);

            // Add an extra log to ensure we are at the exact target point.
            pointsList.Add(new Point(x2, y2));

            // Put our logged points into type-safe point array.
            Point[] points = new Point[pointsList.Count];

            pointsList.CopyTo(points);
            Debug.WriteLine("points length: " + points.Length);
            return points;
        }
    }
}
