// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the GetHashCode property of a ClockCollection
 */

//Instructions:
//     1. Create a Timeline
//     2. Create a Clock, connected to the Timeline
//     3. Establish a ClockCollection

//Warnings:
//     Any changes made in the output should be reflected in GetHashCodeTLCCExpect.txt file

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
    class GetHashCodeTLCC :ITimeBVT
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
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create TimeContainer", " Created TimeContainer ");
            tNode.BeginTime  = TimeSpan.FromMilliseconds(0);
            tNode.Duration = new Duration(TimeSpan.FromMilliseconds(1));
            tNode.Name         = "Timeline";

            // Create a Clock, connected to the container Timeline.
            ClockGroup tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            //Establish a ClockCollection
            ClockCollection TLCC = (ClockCollection)tClock.Children;

            outString += "GetHashCode > 0:  " + (TLCC.GetHashCode() > 0).ToString();

            return outString;
        }
    }
}
