// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify setting the BeginTime property of a Timeline null - Duration Forever
 */

//Instructions:
//  1. Create a Timeline with BeginTime set to null
//  2. Attach event handlers to the TimeNode
//  3. Create a TimeManager with Start=0 and Step = 100 and add the container

//Pass Condition:
//   This test passes if no timing events fire.

//Pass Verification:
//   The output of this test should match the expected output in BeginInfiniteExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected BeginInfiniteExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class BeginInfinite :ITimeBVT
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

            // Create a TimeNode
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create TimeNode" , " Created Timeline " );
            
            // Set Properties to the TimeNode
            tNode.BeginTime     = null;
            tNode.Duration      = System.Windows.Duration.Forever;
            tNode.Name            = "Timeline";
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            //Attach Handlers to the TimeNode
            AttachAllHandlers(tNode);
            DEBUG.LOGSTATUS(" Attached EventHandlers to the Timeline ");

            // Create a Clock, connected to tNode.
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock for the Timeline" , " Created Clock for the Timeline" );

            //Intialize output String
            outString = "";
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 1000, 100, ref outString);
            
            outString += "Begin returns: " + tNode.BeginTime;
            
            return outString;
        }
    }
}
