// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify reading a ParallelTimeline's default properties
 */

//Instructions:
//  1. Create a ParallelTimeline, without setting any properties
//  2. Read ParallelTimeline properties.

//Pass Condition:
//  This test passes if the actual values of the properties match the expected values.

//Pass Verification:
//  The output of this test should match the expected output in DefaultsTLExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected DefaultsTLExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class DefaultsTL :ITimeBVT
    {
        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a Timeline and checks default values.
         *              Logs the results.
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";

            // Create a Timeline
            ParallelTimeline timeline = new ParallelTimeline();
            DEBUG.ASSERT(timeline != null, "Cannot create Timeline" , " Created Timeline " );

            outString += "Timeline            = " +  timeline + "\n";
            outString += "AccelerationRatio   = " +  timeline.AccelerationRatio + "\n";
            outString += "AutoReverse         = " +  timeline.AutoReverse + "\n";
            outString += "BeginTime           = " +  timeline.BeginTime + "\n";
            outString += "Children            = " +  timeline.Children + "\n";
            outString += "DecelerationRatio   = " +  timeline.DecelerationRatio + "\n";
            outString += "Duration            = " +  timeline.Duration + "\n";
            outString += "FillBehavior        = " +  timeline.FillBehavior + "\n";
            outString += "Name                  = " +  timeline.Name + "\n";
            outString += "RepeatBehavior      = " +  timeline.RepeatBehavior + "\n";
            outString += "SpeedRatio          = " +  timeline.SpeedRatio + "\n";

            return outString;
        }
    }
}
