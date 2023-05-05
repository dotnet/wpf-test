// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify reading the Clock's CurrentTime during its Fill period
 */

//Instructions:
//  1. Create a Timeline with Begin = 0, Duration = 50, AutoReverse = True, RepeatCount = 2
//  2. Create a TimeManager with Start=0 and Step = 100 and add the Timeline

//Pass Condition:
//   This test passes if CurrentTimeFill returns the correct time each time it is invoked.

//Pass Verification:
//   The output of this test should match the expected output in CurrentTimeFillExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected CurrentTimeFillExpect.txt file

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
    class CurrentTimeFill :ITimeBVT
    {
        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a Timeline and Checks Clock properties.
         *              Logs the results
         */
        public override string Test()
        {
            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeNode
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create TimeNode" , " Created Timeline " );
            
            // Set Properties to the TimeNode
            tNode.BeginTime       = TimeSpan.FromMilliseconds(0);
            tNode.Duration        = new Duration(TimeSpan.FromMilliseconds(4));
            tNode.RepeatBehavior  = new RepeatBehavior(3);
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            AttachCurrentTimeInvalidatedTC(tClock);

            //Intialize output String
            outString = ""; 
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 16, 1, ref outString);
            
            return outString;
        }
        
        public override void OnProgress(Clock subject)
        {
            outString += "    CurrentTime = " + ((Clock)subject).CurrentTime + "\n";
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}
