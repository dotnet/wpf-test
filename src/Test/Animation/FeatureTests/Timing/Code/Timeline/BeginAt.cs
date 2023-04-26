// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: This is to test the Timeline's BeginTime Property - passing an integer value.
 *              Basic Get-Set test.  No events are bound.
 */

//Instructions:
//  1. Create a TimeNode, with Begin(1000) and Duration(500).
//  2. Create a TimeManager with Start=0 and Step = 100 and add the TimeNode

//Pass Condition:
//   This test passes if the Begin returns 1000.

//Pass Verification:
//   The output of this test should match the expected output in BeginAtExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected BeginAtExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class BeginAt :ITimeBVT
    {

        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a Timeline and Checks whether the Begin event fires correctly.
         *              Logs the results
         */
        public override string Test()
        {
            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create TimeNode
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create Timeline " , " Created Timeline " );
            
            // Set Properties to TimeNode
            tNode.BeginTime     = TimeSpan.FromMilliseconds(1000);
            tNode.Duration      = new System.Windows.Duration(TimeSpan.FromMilliseconds(500));
            tNode.Name          = "TimeNode";
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            //Intialize output String
            outString = "";
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 2000, 100, ref outString);
            
            outString += "BeginTime returns: " + tNode.BeginTime;
            return outString;
        }
    }
}
