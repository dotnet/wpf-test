// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify calling Pause() given SetDesiredFrameRate()
 */

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline with Begin = 0, Duration = 50,  and DFR = 100
//  3. Call Pause()

//Pass Condition:
//   This test passes if the methods affect the CurrentTimeInvalidated event appropriately.


//Pass Verification:
//   The output of this test should match the expected output in DFRPauseExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected in DFRPauseExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class DFRPause : ITimeBVT
    {
        /*
         *  Function :  Test
         *  Arguments:  Framework
         */
         
        Clock _tClock;
         
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
            timeline.Duration      = new Duration(TimeSpan.FromMilliseconds(10));
            
            // Set a desired frame rate.
            Timeline.SetDesiredFrameRate(timeline, 100);

            // Create a Clock, connected to the tContainer.
            _tClock = timeline.CreateClock();     
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock for Timeline" , " Created Clock for Timeline " );
            
            AttachAllHandlersTC(_tClock);

            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, _tClock, tMan, 0, 12, 1, ref outString);
        
            return outString;
        }

        public override void PostTick( int i )
        {
            if ( i == 3 )
            {
                _tClock.Controller.Pause();
            }
        }
    }
}
