// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify removing timing events after the Timeline is finished.
 */

//Instructions:
//   1. Create a TimeManager with Start=0 and Step = 10
//   2. Create a Timeline with Begin = 0, Duration = 20, AutoReverse = True, RepeatCount = 2
//   3. Attach the event handlers to the Timeline
//   4. Create a Clock, based on the Timeline tNode.
//   5. Execute the Tick loop
//   6. Detach the event handlers

//Pass Condition:
//   This test passes if the events fire correctly.

//Pass Verification:
//   The output of this test should match the expected output in EventsRemoveAfterExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected EventsRemoveAfterExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class EventsRemoveAfter :ITimeBVT
    {
        private int        _bCurrentStateCount       = 0;
        private int        _bGlobalSpeedCount        = 0;
        private int        _bCurrentTimeCount        = 0;
        private int        _bCurrentCompletedCount   = 0;
        
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

            // Create a TimeNode
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create Timeline" , " Created Timeline " );
            
            // Set Properties to the TimeNode
            tNode.BeginTime       = TimeSpan.FromMilliseconds(0);
            tNode.Duration        = new System.Windows.Duration(TimeSpan.FromMilliseconds(20));
            tNode.RepeatBehavior  = new RepeatBehavior(2);
            tNode.AutoReverse     = true;
            tNode.Name            = "SimpleTimeNode";     
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            //Attach Handlers to the Timeline
            AttachAllHandlers(tNode);
            DEBUG.LOGSTATUS(" Attached EventHandlers to the Timeline ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            //Intialize output String
            outString = "";
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 110, 10, ref outString);

            //Detach Handlers to the TimeNode
            DetachAllHandlers(tNode);
            DEBUG.LOGSTATUS(" Detached Event Handlers from the Timeline ");
            
            outString += "CurrentStateInvalidated fired:       " + _bCurrentStateCount + "\n";
            outString += "CurrentGlobalSpeedInvalidated fired: " + _bGlobalSpeedCount  + "\n";
            outString += "CurrentTimeInvalidated fired:        " + _bCurrentTimeCount  + "\n";
            outString += "Completed fired:                     " + _bCurrentCompletedCount  + "\n";
            
            return outString;
        }
        
        public override void OnCurrentStateInvalidated(object subject, EventArgs args)
        {
            _bCurrentStateCount++;
        }
        
        public override void OnCurrentGlobalSpeedInvalidated(object subject, EventArgs args)
        {    
            _bGlobalSpeedCount++;
        }
        
        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {    
            _bCurrentTimeCount++;
        }
        
        public override void OnCompleted(object subject, EventArgs args)
        {    
            _bCurrentCompletedCount++;
        }
    }
}
