// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify reading a FillBehavior's default properties
 */

//Instructions:
//  1. Create a Timeline, without setting any properties
//  2. Read FillBehavior properties.

//Pass Condition:
//   This test passes if the actual values of the properties match the expected values.

//Pass Verification:
//   The output of this test should match the expected output in FBDefaultsExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected FBDefaultsExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;


namespace Microsoft.Test.Animation
{
    class FBDefaults :ITimeBVT
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

            // Create a TimeNode
            ParallelTimeline timeline = new ParallelTimeline();
            DEBUG.ASSERT(timeline != null, "Cannot create Timeline" , " Created Timeline " );

            outString += timeline.FillBehavior + "\n";
            outString += FillBehavior.Stop + "\n";
            outString += FillBehavior.HoldEnd + "\n";

            return outString;
        }
    }
}
