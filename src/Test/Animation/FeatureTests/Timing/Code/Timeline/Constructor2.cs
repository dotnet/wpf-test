// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify creating a Timeline using a Constructor with two arguments: BeginTime & Duration
 */

//Instructions:
//  1. Create a Timeline with Begin and Duration specified via the Constructor
//  2. Create a Clock and associate with the Timeline
//  3. Create a TimeManager with Start=0 and Step = 100

//Pass Condition:
//  This test passes if the events fire at the designated times.

//Pass Verification:
//  The output of this test should match the expected output in Constructor2Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected Constructor2Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class Constructor2 :ITimeBVT
    {
        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a Timeline and Checks whether events are properly caught.
         *              Logs the results.
         */
        public override string Test()
        {
            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeNode
            ParallelTimeline tNode = new ParallelTimeline(TimeSpan.FromMilliseconds(100),new System.Windows.Duration(TimeSpan.FromMilliseconds(400)));
            DEBUG.ASSERT(tNode != null, "Cannot create TimeNode" , " Created Timeline " );          
            // Set Properties to the TimeNode
            tNode.RepeatBehavior  = new RepeatBehavior(2);
            tNode.AutoReverse     = true;
            tNode.Name            = "TimeNode";          
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            //Intialize output String
            outString = "";
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 1800, 100, ref outString);

            return outString;
        }
    }
}
