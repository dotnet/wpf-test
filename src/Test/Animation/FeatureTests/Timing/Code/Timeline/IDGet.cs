// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify reading the Name property via Timeline events
 */

//Instructions:
//  1. Create a Timeline with Begin = 0, Duration = 50, AutoReverse = True, RepeatCount = 2
//  2. Attach event handlers to the Timeline
//  3. Create a TimeManager with Start=0 and Step = 50 and add the Timeline

//Pass Condition:
//   This test passes if the Name property is read correctly each time (default=empty string).

//Pass Verification:
//   The output of this test should match the expected output in IDGetExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected IDGetExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class IDGet :ITimeBVT
    {
        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a Timeline and Checks whether events are properly caught.
         *              Logs the results
         */
        ParallelTimeline _tNode;
         
        public override string Test()
        {
            //Intialize output String
            outString = "";

            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeNode
            _tNode = new ParallelTimeline();
            DEBUG.ASSERT(_tNode != null, "Cannot create TimeNode" , " Created Timeline " );
            
            // Set Properties to the TimeNode
            _tNode.BeginTime         = TimeSpan.FromMilliseconds(0);
            _tNode.Duration          = new System.Windows.Duration(TimeSpan.FromMilliseconds(50));
            _tNode.RepeatBehavior    = new RepeatBehavior(2);
            _tNode.AutoReverse       = true;
            _tNode.Name              = "Timeline1";
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = _tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            //Attach Handlers to the Clock
            //HACK-HACK: CurrentTimeInvalidated must be associated to the clock to force a click.
            AttachAllHandlersTC(tClock);
            DEBUG.LOGSTATUS(" Attached EventHandlers to the Clock ");
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 210, 10, ref outString);
            
            return outString;
        }
        
        public override void OnCurrentStateInvalidated(object subject, EventArgs args)
        {
            outString += "---Name  = " + _tNode.Name + "\n";
        }
        
        public override void OnCurrentGlobalSpeedInvalidated(object subject, EventArgs args)
        {    
            outString += "---Name  = " + _tNode.Name + "\n";
        }
        
        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {    
            outString += "---Name  = " + _tNode.Name + "\n";
        }
        
        public override void OnCompleted(object subject, EventArgs args)
        {    
            outString += "---Name  = " + _tNode.Name + "\n";
        }

        public override void OnProgress(Clock subject)
        {
            outString += "-------------Progress :  Name  = " + subject.Timeline.Name + "\n";
        }
    }
}
