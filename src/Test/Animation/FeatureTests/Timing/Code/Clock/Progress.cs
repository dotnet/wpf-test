// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify reading the Clock's CurrentProgress property
 */

//Instructions:
//  1. Create a Timeline with Begin = 0, Duration = 50, AutoReversed = True, RepeatedCount = 2
//  2. Create a TimeManager with Start=0 and Step = 100 and add the TimeNode

//Pass Condition:
//   This test passes if the Progress property is read correctly.

//Pass Verification:
//   The output of this test should match the expected output in ProgressExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected ProgressExpect.txt file

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
    class Progress :ITimeBVT
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
            tNode.BeginTime      = TimeSpan.FromMilliseconds(0);
            tNode.Duration       = new Duration(TimeSpan.FromMilliseconds(50));
            tNode.RepeatBehavior = new RepeatBehavior(3);
            tNode.AutoReverse    = true;
            tNode.Name           = "SimpleTimeNode";
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            //Intialize output String
            outString = "";
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 210, 10, ref outString);
            
            return outString;
        }

        public override void OnProgress(Clock subject)
        {
            outString += "    ProgressChanged:  Progress = " + subject.CurrentProgress + "\n";
        }
    }
}
