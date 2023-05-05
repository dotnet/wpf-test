// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify setting the Begin property of TimeNode, with no Duration.
 */

//Instructions:
//  1. Create a Timeline with Begin = 100
//  2. Attach BeginActive, ProgressChanged, EndActive handlers to the TimeNode
//  3. Create a TimeContainer with none of the attributes set and add the Timenode
//  4. Create a TimeManager with Start=0 and Step = 50 and add the container

//Pass Condition:
//   This test passes if no timing events fire.

//Pass Verification:
//   The output of this test should match the expected output in BeginNoDurationExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected BeginNoDurationExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class BeginNoDuration :ITimeBVT
    {
        /*
         *  Function:  Test
         *  Arguments: Framework
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
            tNode.BeginTime     = TimeSpan.FromMilliseconds(100);
            tNode.Name            = "Timeline";
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            //Attach Handler to the TimeNode
            AttachCurrentStateInvalidated(tNode);
            DEBUG.LOGSTATUS(" Attached EventHandler to the Timeline ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            //Intialize output String
            outString = "";
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock,  tMan, 0, 1000, 100, ref outString);
            
            outString += "Begin returns: " + tNode.BeginTime;
            return outString;
        }       
    }
}
