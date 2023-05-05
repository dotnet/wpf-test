// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify SetDesiredFrameRate() via the CurrentStateInvalidated event
 */

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline with Begin = 0, Duration = 50, and DFR = 200
//  3. In the CurrentStateInvalidated event hander, call SetDesiredFrameRate.
//     It should have no effect.

//Pass Condition:
//   This test passes if the methods affect the CurrentTimeInvalidated event appropriately.

//Pass Verification:
//   The output of this test should match the expected output in DFRStateExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected in DFRStateExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class DFRState : ITimeBVT
    {
        ParallelTimeline _timeline;
        
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
            _timeline = new ParallelTimeline();
            DEBUG.ASSERT(_timeline != null, "Cannot create Timeline" , " Created Timeline " );
            _timeline.BeginTime     = TimeSpan.FromMilliseconds(0);
            _timeline.Duration      = new Duration(TimeSpan.FromMilliseconds(10));
            
            // Create a Clock, connected to the tContainer.
            Clock tClock = _timeline.CreateClock();     
            DEBUG.ASSERT(tClock != null, "Cannot create Clock for Timeline" , " Created Clock for Timeline " );
            
            AttachCurrentTimeInvalidatedTC(tClock);
            AttachCurrentStateInvalidatedTC(tClock);

            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 15, 1, ref outString);
        
            return outString;
        }
        
        public override void OnCurrentStateInvalidated(object subject, EventArgs args)
        {    
            // Set a desired frame rate.
            Timeline.SetDesiredFrameRate(_timeline, 200);
            outString += "GetDesiredFrameRate returns: " + Timeline.GetDesiredFrameRate(_timeline) + "\n";
        }
        
        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {    
        }
    }
}
