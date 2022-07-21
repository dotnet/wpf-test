// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace DRT
{
    internal partial class TimingSuite : DrtTestSuite
    {   
        /// <summary>
        /// This runs tests to verify that we're robust against 
        /// MediaContext._timeManager becoming null (such as 
        /// when the app is shutting down).    
        /// </summary>
        void NullTimeManager()
        {
            Clock rootClock;
            Clock parent;
            double speedRatio;
            DoubleAnimation animation = new DoubleAnimation(0, 150, new Duration(TimeSpan.FromSeconds(10)));
            Rectangle rect = new Rectangle();

            Console.WriteLine("Running null TimeManager test");
            manager.SetToNull();
            rootClock = animation.CreateClock(true);

            
            //
            // Check that the parent is null
            //
            parent = rootClock.Parent;

            DRT.Assert(parent == null,
                "The parent of this clock is the TimeManager -- it should be null");

            //
            // Perform all interactive operations on the Clock
            //
            rootClock.Controller.Begin();
            rootClock.Controller.Pause();
            rootClock.Controller.Remove();
            rootClock.Controller.SeekAlignedToLastTick(TimeSpan.FromSeconds(0), TimeSeekOrigin.BeginTime);
            rootClock.Controller.Seek(TimeSpan.FromSeconds(2), TimeSeekOrigin.BeginTime);
            rootClock.Controller.Stop();

            //
            // Grab a property
            //

            speedRatio = rootClock.Controller.SpeedRatio;

            //  
            // Begin an animation
            //
            rect.BeginAnimation(Rectangle.RadiusXProperty, animation);

            manager.Restore();
        }
    }
}
