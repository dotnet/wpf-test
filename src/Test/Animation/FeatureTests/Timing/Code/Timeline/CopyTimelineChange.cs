// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the Copy method of a Timeline
 */

//Instructions:
//  1. Create a Timeline with Begin = 100, Duration = 500
//  2. Copy the Timeline, then change its Begin, Duration, RepeatCount, and AutoReverse properties
//  3. Create a TimeManager with Start=0 and Step = 50

//Pass Condition:
//   This test passes if the Timeline is copied successfully.

//Pass Verification:
//   The output of this test should match the expected output in CopyTimelineChangeExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected CopyTimelineChangeExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class CopyTimelineChange :ITimeBVT
    {
        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a Timeline and Checks whether events are properly caught.
         *              Logs the results
         */
        public override string Test()
        {
            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            //Create a TimeContainer
            ParallelTimeline tContainer = new ParallelTimeline();
            tContainer.BeginTime    = TimeSpan.FromMilliseconds(0);
            tContainer.Duration     = new System.Windows.Duration(TimeSpan.FromMilliseconds(2000));
            tContainer.Name           = "TimeContainer";
            DEBUG.ASSERT(tContainer != null, "Cannot create a Container " , " Created a Container " );

            // Create a TimeNode
            ParallelTimeline tNode1 = new ParallelTimeline();
            DEBUG.ASSERT(tNode1 != null, "Cannot create Timeline" , " Created Timeline " );         
            // Set Properties to the TimeNode
            tNode1.Name               = "Timeline1";
            tNode1.BeginTime        = TimeSpan.FromMilliseconds(200);
            tNode1.Duration         = new System.Windows.Duration(TimeSpan.FromMilliseconds(500));
            DEBUG.LOGSTATUS(" Set Timeline 1 Properties ");

            //Copy tNode1 to tNode2.
            Timeline tNode2 = tNode1.Clone();
            DEBUG.ASSERT(tNode2 != null, "Cannot create Timeline" , " Created Timeline " );
            tNode2.Name               = "Timeline2";
            tNode2.BeginTime        = TimeSpan.FromMilliseconds(0);
            tNode2.Duration         = new System.Windows.Duration(TimeSpan.FromMilliseconds(400));
            tNode2.RepeatBehavior   = new RepeatBehavior(2);
            tNode2.AutoReverse      = true;
            DEBUG.LOGSTATUS(" Set Timeline 2 Properties ");
        
            //Attach TimeNodes to the TimeContainer
            tContainer.Children.Add(tNode1);
            tContainer.Children.Add(tNode2);
            DEBUG.LOGSTATUS(" Attached Timelines to the TimeContainer ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = tContainer.CreateClock();      
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );
    
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 2100, 100, ref outString);
            
            return outString;
        }       
    }
}
