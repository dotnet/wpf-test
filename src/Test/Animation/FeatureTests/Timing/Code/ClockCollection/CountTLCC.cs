// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the Count property of the ClockCollection when it is empty
 */

//Instructions:
//     1. Create four Timelines
//     2. Create a Clock, connected to the container Timeline.
//     3. Establish a ClockCollection, based on the Children of the root Clock

//Warnings:
//     Any changes made in the output should be reflected in CountTLCCExpect.txt file

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
    class CountTLCC :ITimeBVT
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

            // Create a TimeContainer
            ParallelTimeline timeline = new ParallelTimeline();
            DEBUG.ASSERT(timeline != null, "Cannot create TimeContainer", " Created TimeContainer ");
            timeline.BeginTime  = TimeSpan.FromMilliseconds(4);
            timeline.Duration = new Duration(TimeSpan.FromMilliseconds(8));
            timeline.Name         = "Timeline";

            // Create a Clock, connected to the container Timeline.
            ClockGroup tClock = timeline.CreateClock();        
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            //Establish a ClockCollection
            ClockCollection TLCC = (ClockCollection)tClock.Children;

            outString += "Count:  " + TLCC.Count + "\n";

            return outString;
        }
    }
}
