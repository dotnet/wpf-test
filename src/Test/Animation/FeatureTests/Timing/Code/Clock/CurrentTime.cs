// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify reading the Clock's CurrentTime property via the CurrentTimeInvalidated event
 */

//Instructions:
//  1. Create a Timeline with Begin = 0, Duration = 50, AutoReverse = True, RepeatCount = 2
//  2. Attach event handlers to the TimeNode
//  3. Create a TimeManager with Start=0 and Step = 100 and add the TimeNode

//Pass Condition:
//   This test passes if CurrentTime returns the correct time each time it is invoked.

//Pass Verification:
//   The output of this test should match the expected output in CurrentTimeExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected CurrentTimeExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    class CurrentTime :ITimeBVT
    {
        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a Timeline and Checks whether events are properly caught.
         *              Logs the results
         */
        public override string Test()
        {
            //Intialize output String
            outString = ""; 

            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeNode
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create TimeNode" , " Created Timeline " );
            
            // Set Properties to the TimeNode
            tNode.BeginTime       = TimeSpan.FromMilliseconds(0);
            tNode.Duration        = new Duration(TimeSpan.FromMilliseconds(30));
            tNode.RepeatBehavior  = new RepeatBehavior(3);
            tNode.AutoReverse     = true;
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            //Attach Handler to the Clock
            AttachCurrentTimeInvalidatedTC(tClock);
            DEBUG.LOGSTATUS(" Attached EventHandler to the Clock ");
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 210, 10, ref outString);
            
            return outString;
        }
        
        public override void OnProgress(Clock subject)
        {
            outString += "CurrentTime [Per Tick]:               " + ((Clock)subject).CurrentTime + "\n";
        }
        
        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
            outString += "CurrentTime [CurrentTimeInvalidated]: " + ((Clock)subject).CurrentTime + "\n";
        }
    }
}
