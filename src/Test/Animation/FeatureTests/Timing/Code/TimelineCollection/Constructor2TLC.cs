// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the TimelineCollection Constructor: new TimelineCollection(capacity)
 */

//Instructions:
//   1. Create a Timeline tree
//   2. Create a TimelineCollection and the container Timeline to it


//Warnings:
//   Any changes made in the output should be reflected in Constructor2TLCExpect.txt file

//Dependencies:
//TestRuntime.dll

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class Constructor2TLC :ITimeBVT
    {
        TimelineCollection _TLC;

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

            // Create Timeline #1
            ParallelTimeline timeline1 = new ParallelTimeline();
            DEBUG.ASSERT(timeline1 != null, "Cannot create TimeLine 1", " Created TimeLine 1");
            timeline1.Name = "TimeLine1";
            
            // Create Timeline #2
            ParallelTimeline timeline2 = new ParallelTimeline();
            DEBUG.ASSERT(timeline2 != null, "Cannot create TimeLine 2", " Created TimeLine 2");
            timeline2.Name = "TimeLine2";

            // Create Timeline #3
            ParallelTimeline timeline3 = new ParallelTimeline();
            DEBUG.ASSERT(timeline3 != null, "Cannot create TimeLine 3", " Created TimeLine 3");
            timeline3.Name = "TimeLine3";

            // Create a new TimelineCollection
            _TLC = new TimelineCollection(0);
            _TLC.Add(timeline1);
            _TLC.Add(timeline2);
            _TLC.Add(timeline3);

            outString += "TimelineCollection Count:   " + _TLC.Count + "\n";
            outString += "TimelineCollection Item 1:  " + _TLC[0].Name + "\n";
            outString += "TimelineCollection Item 2:  " + _TLC[1].Name + "\n";
            outString += "TimelineCollection Item 3:  " + _TLC[2].Name + "\n";

            return outString;
        }
    }
}
