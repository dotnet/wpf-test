// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify CurrentProgress when Duration is set to Forever
 */

//Instructions:
//  1. Create a Timeline with Duration="Forever"
//  2. Create a TimeManager with Start=0 and Step = 10 and add the container

//Pass Condition:
//   This test passes if the CurrentProgress property of the Clock returns correct values.

//Pass Verification:
//   The output of this test should match the expected output in DurationForeverExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected DurationForeverExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class DurationForever :ITimeBVT
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

            // Create a Timeline
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create Timeline" , " Created Timeline " );
            
            // Set Properties to the Timeline
            tNode.BeginTime     = new TimeSpan(0,0,0,0,0);
            tNode.Duration      = System.Windows.Duration.Forever;
            tNode.Name            = "Timeline";

            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            // Create a Clock, connected to tNode.
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock for the Timeline" , " Created Clock for the Timeline" );

            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 12, 1, ref outString);
            
            return outString;
        }
    }
}
