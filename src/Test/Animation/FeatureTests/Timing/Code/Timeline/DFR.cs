// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify SetDesiredFrameRate() and GetDesiredFrameRate methods of a Timeline
 */

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline with Begin = 0, Duration = 50, and DFR = 200

//Pass Condition:
//   This test passes if the methods affect the CurrentTimeInvalidated event appropriately.

//Pass Verification:
//   The output of this test should match the expected output in DFRExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected in DFRExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class DFR : ITimeBVT
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
            timeline.BeginTime     = TimeSpan.FromMilliseconds(0);
            timeline.Duration      = new Duration(TimeSpan.FromMilliseconds(50));
            
            // Set a desired frame rate.
            Timeline.SetDesiredFrameRate(timeline, 100);

            // Create a Clock, connected to the tContainer.
            Clock tClock = timeline.CreateClock();     
            DEBUG.ASSERT(tClock != null, "Cannot create Clock for Timeline" , " Created Clock for Timeline " );
            
            AttachCurrentTimeInvalidatedTC(tClock);

            outString += "GetDesiredFrameRate returns: " + Timeline.GetDesiredFrameRate(timeline) + "\n";
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 52, 1, ref outString);
        
            return outString;
        }

        public override void OnProgress(Clock subject)
        {
        }
    }
}
