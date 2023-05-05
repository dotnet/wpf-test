// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify setting the Name property of Timeline via events
 */

//Instructions:
//  1. Create a Timeline with Begin = 100, Duration = 500
//  2. Attach event handlers to the TimeNode
//  3. Create a TimeManager with Start=0 and Step = 50

//Pass Condition:
//   This test passes if the Name property is read correctly each time it is set.
//   NOTE:  the value returned via clock.Timeline in the event handler will return the original
//   name, because it is retrieving it from a frozen copy of the original Timeline.

//Pass Verification:
//   The output of this test should match the expected output in IDInBegunInEndedExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected IDInBegunInEndedExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class IDInBegunInEnded :ITimeBVT
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
            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeNode
            _tNode = new ParallelTimeline();
            _tNode.Name   = "Timeline1";
            DEBUG.ASSERT(_tNode != null, "Cannot create TimeNode" , " Created Timeline " );
            
            // Set Properties to the TimeNode
            _tNode.BeginTime         = TimeSpan.FromMilliseconds(100);
            _tNode.Duration          = new System.Windows.Duration(TimeSpan.FromMilliseconds(200));
            _tNode.AutoReverse       = true;
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            //Attach Handlers to the TimeNode
            AttachAllHandlers(_tNode);

            DEBUG.LOGSTATUS(" Attached EventHandlers to the Timeline ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = _tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );
    
            outString = "BEFORE TIMELINE BEGINS --- Name: " + _tNode.Name + "\n";
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 1000, 100, ref outString);
            
            return outString;
        }
        
        public override void OnCurrentStateInvalidated(object subject, EventArgs args)
        {
            string newName1 = "NewName1";
            _tNode.Name   = newName1;
            outString += "--------- Name correct: " + (_tNode.Name == newName1) + "\n";
        }
        
        public override void OnCurrentGlobalSpeedInvalidated(object subject, EventArgs args)
        {    
            string newName2 = "NewName2";
            _tNode.Name   = newName2;
            outString += "--------- Name correct: " + (_tNode.Name == newName2) + "\n";
        }
        
        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {    
            string newName3 = "NewName3";
            _tNode.Name   = newName3;
            outString += "--------- Name correct: " + (_tNode.Name == newName3) + "\n";
        }
        
        public override void OnCompleted(object subject, EventArgs args)
        {    
            string newName4 = "NewName4";
            _tNode.Name   = newName4;
            outString += "--------- Name correct: " + (_tNode.Name == newName4) + "\n";
        }
    }
}
