// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the GetHashCode method for RepeatBehavior
 */

//Instructions:
//  1. Create a RepeatBehavior object
//  2. Read RepeatBehavior.GetHashCode

//Pass Condition:
//  This test passes if an integer is returned.

//Pass Verification:
//  The output of this test should match the expected output in RBGetHashCodeExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected RBGetHashCodeExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class RBGetHashCode :ITimeBVT
    {
        /*
         *  Function:   Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            // Create a Timeline object
            ParallelTimeline timeline = new ParallelTimeline();
            DEBUG.ASSERT(timeline != null, "Cannot create Timeline" , " Created Timeline " );
            
            // Create a RepeatBehavior object
            timeline.RepeatBehavior  = new RepeatBehavior(TimeSpan.FromMilliseconds(2));
            
            int Hash = timeline.RepeatBehavior.GetHashCode();
            
            bool Result = (Hash.GetType().ToString() == "System.Int32");
            
            return Result.ToString();
        }
    }
}
