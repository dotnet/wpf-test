// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify SetDesiredFrameRate() on a Timeline and SpeedRatio on its Clock
 */

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline with Begin = 0, Duration = 50, and DFR = 200

//Pass Condition:
//   This test passes if the methods affect the CurrentTimeInvalidated event appropriately.

//Pass Verification:
//   The output of this test should match the expected output in DFRSpeed2Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected in DFRSpeed2Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class DFRSpeed2 : ITimeBVT
    {
        /*
         *  Function :  Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            outString = "";

            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a Timeline
            ParallelTimeline timeline = new ParallelTimeline();
            DEBUG.ASSERT(timeline != null, "Cannot create Timeline" , " Created Timeline " );
            timeline.BeginTime      = TimeSpan.FromMilliseconds(0);
            timeline.Duration       = new Duration(TimeSpan.FromMilliseconds(20));
            
            // Set a desired frame rate.
            Timeline.SetDesiredFrameRate(timeline, 500);

            // Create a Clock, connected to the tContainer.
            Clock clock = timeline.CreateClock();     
            DEBUG.ASSERT(clock != null, "Cannot create Clock for Timeline" , " Created Clock for Timeline " );
            
            AttachCurrentTimeInvalidatedTC(clock);

            clock.Controller.SpeedRatio = 2;

            outString += "GetDesiredFrameRate returns: " + Timeline.GetDesiredFrameRate(timeline) + "\n";
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, clock, tMan, 0, 21, 1, ref outString);
        
            return outString;
        }

        public override void OnProgress(Clock subject)
        {
        }
    }
}
