// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the CreateClock method for TimelineGroup
 */

//Instructions:
//     1. Create a TimelineGroup
//     2. Apply the CreateClock method

//Pass Condition:
//     This test passes if a Clock is created.

//Warnings:
//     Any changes made in the output should be reflected in CreateClockTLGExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class CreateClockTLG :ITimeBVT
    {
        /*
        *  Function:  Test
        *  Arguments: Framework
        */
        public override string Test()
        {
            //Intialize output String
            outString = "";

            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            ParallelTimeline tNode = new ParallelTimeline();
            tNode.BeginTime         = TimeSpan.FromMilliseconds(1);
            tNode.Duration          = new Duration(TimeSpan.FromMilliseconds(5));
            tNode.Name                = "Timeline";
            DEBUG.ASSERT(tNode != null, "Cannot create Timeline" , " Created Timeline " );
            
            // Create a TimelineGroup
            TimelineGroup timelineGroup = (TimelineGroup)tNode;
            DEBUG.ASSERT(timelineGroup != null, "Cannot create TimelineGroup", " Created TimelineGroup ");
            timelineGroup.Name         = "Group1";

            Clock clock = timelineGroup.CreateClock();
            DEBUG.ASSERT(clock != null, "Cannot create Clock" , " Created Clock " );

            //Attach Handler to the Clock
            AttachCurrentStateInvalidatedTC(clock);
            DEBUG.LOGSTATUS(" Attached EventHandler to the Clock ");

            //Intialize output String
            outString = ""; 

            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, clock, tMan, 0, 10, 1, ref outString );

            return outString;
        }
    }
}
