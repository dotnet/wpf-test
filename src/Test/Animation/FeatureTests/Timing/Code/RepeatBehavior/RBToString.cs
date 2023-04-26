// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the ToString method for RepeatBehavior
 */

//Instructions:
//  1. Create a RepeatBehavior object
//  2. Apply RepeatBehavior.ToString

//Pass Condition:
//  This test passes if an integer is returned.

//Pass Verification:
//  The output of this test should match the expected output in RBToStringExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected RBToStringExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class RBToString :ITimeBVT
    {
        /*
         *  Function:   Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";
    
            // Create Timeline 1
            ParallelTimeline timeline1 = new ParallelTimeline();
            timeline1.RepeatBehavior  = new RepeatBehavior(TimeSpan.FromMilliseconds(2));
    
            // Create Timeline 2
            ParallelTimeline timeline2 = new ParallelTimeline();
            timeline2.RepeatBehavior  = new RepeatBehavior(1048576);
            
            outString += timeline1.RepeatBehavior.ToString() + "\n";
            outString += timeline2.RepeatBehavior.ToString();
            
            return outString;
        }
    }
}
