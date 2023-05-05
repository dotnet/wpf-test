// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  


//Instructions:
//  1. Create a Timeline 
//  2. Create a Clock, connecting it to the Timeline.
//  3. Read the Timeline property.

//Pass Condition:
//   This test passes if the Timeline property returns the correct Timeline.

//Pass Verification:
//   The output of this test should match the expected output in TimelineGetExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected TimelineGetExpect.txt file

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
    class TimelineGet :ITimeBVT
    {
        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a Timeline and Checks whether the Timeline property is correct.
         *              Logs the results
         */
        public override string Test()
        {
            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeContainer
            ParallelTimeline tContainer = new ParallelTimeline();
            DEBUG.ASSERT(tContainer != null, "Cannot create TimeContainer", " Created TimeContainer ");
            // Set Properties to the TimeContainer
            tContainer.BeginTime    = TimeSpan.FromMilliseconds(0);
            tContainer.Duration = new Duration(TimeSpan.FromMilliseconds(100));
            tContainer.Name           = "Container";
            DEBUG.LOGSTATUS(" Set TimeContainer Properties ");

            // Create a TimeNode
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create TimeNode" , " Created Timeline " );          
            // Set Properties to the TimeNode
            tNode.BeginTime    = TimeSpan.FromMilliseconds(0);
            tNode.Duration = new Duration(TimeSpan.FromMilliseconds(50));
            tNode.Name           = "TimeNode";
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            //Attach TimeNode to the TimeContainer
            tContainer.Children.Add(tNode);
            DEBUG.LOGSTATUS(" Attached TimeNode3 to the TimeContainer ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            DEBUG.LOGSTATUS(" Attached EventHandlers to the Clock ");

            //Intialize output String
            outString = ""; 
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 100, 20, ref outString);
            
            return outString;
        }
    
        public override void OnProgress(Clock subject)
        {
            outString += "-----Timeline.Name       = " + ((Clock) subject).Timeline.Name + "\n";
            outString += "-----Timeline.Duration = " + ((Clock) subject).Timeline.Duration + "\n";
        }
    }
}
