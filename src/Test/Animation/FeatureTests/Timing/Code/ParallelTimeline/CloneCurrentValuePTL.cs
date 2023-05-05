// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the CloneCurrentValue method for ParallelTimeline
 */

//Instructions:
//     1. Create a ParallelTimeline
//     2. Use CloneCurrentValue() to copy the ParallelTimeline to a second ParallelTimeline

//Pass Condition:
//     This test passes if a second ParallelTimeline is created.

//Warnings:
//     Any changes made in the output should be reflected in CloneCurrentValuePTLExpect.txt file

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
    class CloneCurrentValuePTL :ITimeBVT
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

            // Create a ParallelTimeline
            ParallelTimeline PTL1 = new ParallelTimeline();
            DEBUG.ASSERT(PTL1 != null, "Cannot create ParallelTimeline", " Created ParallelTimeline ");
            PTL1.BeginTime  = TimeSpan.FromMilliseconds(2);
            PTL1.Duration   = new Duration(TimeSpan.FromMilliseconds(5));
            PTL1.Name         = "Timeline1";

            ParallelTimeline PTL2 = PTL1.CloneCurrentValue();
            PTL2.Name         = "Timeline2";
            DEBUG.LOGSTATUS(" Applied CloneCurrentValue ");

            outString += "--- " + PTL1.Name + "-- " + PTL1 + "\n";
            outString += "--- " + PTL2.Name + "-- " + PTL2 + "\n";

            return outString;
        }
    }
}
