// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify setting RepeatBehavior when no Duration is specified
 */

//Pass Condition:
//  This test passes if the timeline progresses once.

//Pass Verification:
//  The output of this test should match the expected output in RBNoDurationExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected RBNoDurationExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class RBNoDuration :ITimeBVT
    {
        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a Timeline and Checks whether events are properly caught.
         *              Logs the results.
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";

            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeNode
            ParallelTimeline timeline = new ParallelTimeline();
            DEBUG.ASSERT(timeline != null, "Cannot create Timeline" , " Created Timeline " );
            
            // Set Properties to the TimeNode
            timeline.BeginTime       = TimeSpan.FromMilliseconds(0);
            timeline.RepeatBehavior  = new RepeatBehavior(1.5);
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = timeline.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 5, 1, ref outString);

            return outString;
        }
        
        public override void OnProgress( Clock subject )
        {
            outString += "  " + ((Clock)subject).Timeline.Name + ": Progress = " + ((Clock)subject).CurrentProgress + "\n";
            outString += "  " + ((Clock)subject).Timeline.Name + ": State    = " + ((Clock)subject).CurrentState + "\n";
        }
    }
}
