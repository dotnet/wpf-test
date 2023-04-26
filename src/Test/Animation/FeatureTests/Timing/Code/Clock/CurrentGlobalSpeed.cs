// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify reading the CurrentGlobalSpeed property of the Clock.
 */

//Instructions:
//  1. Create a Timeline
//  2. Create a TimlineClock, connecting it with the Timeline
//  3. Start the TimeManager

//Pass Condition:
//   This test passes if CurrentGlobalSpeed return the expected value.

//Pass Verification:
//   The output of this test should match the expected output in CurrentGlobalSpeedExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected CurrentGlobalSpeedExpect.txt file

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
    class CurrentGlobalSpeed :ITimeBVT
    {
        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a Timeline and Clock
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
            tNode.BeginTime         = TimeSpan.FromMilliseconds(0);
            tNode.Duration          = new Duration(TimeSpan.FromMilliseconds(10));
            tNode.RepeatBehavior    = new RepeatBehavior(2);
            tNode.AutoReverse       = true;
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            DEBUG.LOGSTATUS(" Attached EventHandlers to the Clock ");

            //Intialize output String
            outString = ""; 
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 110, 10, ref outString);
            
            return outString;
        }
    
        public override void OnProgress(Clock subject)
        {
            outString += "---------CurrentProgress =    " + ((Clock)subject).CurrentProgress + "\n";
            outString += "---------CurrentGlobalSpeed = " + ((Clock)subject).CurrentGlobalSpeed + "\n";
        }
    }
}
