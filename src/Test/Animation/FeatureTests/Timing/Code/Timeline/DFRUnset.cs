// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify dynamically passing null to SetDesiredFrameRate()
 */

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline with Begin = 0, Duration = 50, and DFR = 200

//Pass Condition:
//   This test passes if the methods affect the CurrentTimeInvalidated event appropriately.

//Pass Verification:
//   The output of this test should match the expected output in DFRUnsetExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected in DFRUnsetExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class DFRUnset : ITimeBVT
    {
        /*
         *  Function :  Test
         *  Arguments:  Framework
         */
         
        ParallelTimeline _timeline;
        TimeManagerInternal _tMan;
        Clock _tClock;
         
        public override string Test()
        {
            //Intialize output String
            outString = "";
            
            // Create a TimeManager
            _tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(_tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a Timeline
            _timeline = new ParallelTimeline();
            DEBUG.ASSERT(_timeline != null, "Cannot create Timeline" , " Created Timeline " );
            _timeline.BeginTime     = TimeSpan.FromMilliseconds(0);
            _timeline.Duration      = new Duration(TimeSpan.FromMilliseconds(20));
            
            // Set a desired frame rate.
            Timeline.SetDesiredFrameRate(_timeline, 500);
            Timeline.SetDesiredFrameRate(_timeline, null);

            // Create a Clock, connected to the tContainer.
            _tClock = _timeline.CreateClock();     
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock for Timeline" , " Created Clock for Timeline " );
            
            AttachCurrentTimeInvalidatedTC(_tClock);

            outString += "GetDesiredFrameRate returns: " + Timeline.GetDesiredFrameRate(_timeline) + "\n";
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, _tClock, _tMan, 0, 21, 1, ref outString);
        
            return outString;
        }
        
        public override void OnProgress(Clock subject)
        {
        }
    }
}
