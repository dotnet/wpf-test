// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the TimelineCollection Constructor: new TimelineCollection()
 */

//Instructions:
//   1. Create a Timeline tree
//   2. Create a TimelineCollection and the container Timeline to it


//Warnings:
//   Any changes made in the output should be reflected in Constructor1TLCExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class Constructor1TLC :ITimeBVT
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

            // Create a TimeContainer
            ParallelTimeline tContainer = new ParallelTimeline();
            DEBUG.ASSERT(tContainer != null, "Cannot create TimeContainer", " Created TimeContainer ");

            // Create TimeNode 1
            ParallelTimeline tNode1 = new ParallelTimeline();
            DEBUG.ASSERT(tNode1 != null, "Cannot create TimeNode 1" , " Created TimeNode 1" );

            // Create TimeNode 2
            ParallelTimeline tNode2 = new ParallelTimeline();
            DEBUG.ASSERT(tNode2 != null, "Cannot create TimeNode 2" , " Created TimeNode 2" );

            // Attach TimeNodes to the TimeContainer
            tContainer.Children.Add(tNode1);
            tContainer.Children.Add(tNode2);
            DEBUG.LOGSTATUS(" Attached Timelines to the TimeContainer ");

            // Create a new TimelineCollection
            _TLC = new TimelineCollection();
            _TLC.Add(tContainer);

            outString += "TimelineCollection Count: " + _TLC.Count + "\n";
            outString += "TimelineCollection Item:  " + _TLC[0] + "\n";
            outString += "Item Children             " + ((TimelineGroup)_TLC[0]).Children.Count + "\n";

            return outString;
        }
    }
}
