// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify NOT setting the BeginTime property of a Timeline
 */

//Instructions:
//  1. Create a Timeline with no BeginTime and Duration = 500
//  2. Attach a CurrentStateInvalidated handler to the Timeline
//  3. Create a TimeManager with Start=0 and Step = 50 and add the container

//Pass Condition:
//   This test passes if CurrentStateInvalidated fires once, and CurrentState = ClockState.Stopped.

//Pass Verification:
//   The output of this test should match the expected output in BeginNotSpecifiedExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected BeginNotSpecifiedExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class BeginNotSpecified :ITimeBVT
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

            // Create a Timeline
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create Timeline" , " Created Timeline " );
            
            // Set Properties to the Timeline
            tNode.Duration = new System.Windows.Duration(TimeSpan.FromMilliseconds(500));         
            tNode.Name         = "SimpleTimeNode";
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            //Attach Handler to the Timeline
            AttachCurrentStateInvalidated(tNode);
            DEBUG.LOGSTATUS(" Attached EventHandler to the Timeline ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock" );

            //Intialize output String
            outString = "";
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 1000, 100, ref outString);

            return outString;
        }       
    }
}
