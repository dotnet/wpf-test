// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the Equals method for RepeatBehavior -- RB.Equals(object)
  */

//Instructions:
//  1. Create a TimeManager
//  2. Create a TimeContainer and four Timelines added as Children.
//  3. Obtain two RepeatBehavior objects and compare them using Equals method

//Pass Condition:
//  This test passes if only the comparison returns false.

//Pass Verification:
//  The output of this test should match the expected output in RBEqualsObjectExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected RBEqualsObjectExpect.txt file.

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class RBEqualsObject :ITimeBVT
    {
        /*
         *  Function:   Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";
    
            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");
            
            // Create Timeline 1
            ParallelTimeline timeline1 = new ParallelTimeline( null , System.Windows.Duration.Automatic);
            timeline1.RepeatBehavior  = new RepeatBehavior(1);
            
            // Create Timeline 2
            ParallelTimeline timeline2 = new ParallelTimeline( null , System.Windows.Duration.Automatic);
            timeline2.RepeatBehavior  = new RepeatBehavior(1);
            
            // Create Timeline 3
            ParallelTimeline timeline3 = new ParallelTimeline( null , System.Windows.Duration.Automatic);
            timeline3.RepeatBehavior  = new RepeatBehavior(0);
            
            // Create Timeline 4
            ParallelTimeline timeline4 = new ParallelTimeline( null , System.Windows.Duration.Automatic);
            timeline4.RepeatBehavior  = new RepeatBehavior(TimeSpan.FromMilliseconds(99));
            
            // Create Timeline 5
            ParallelTimeline timeline5 = new ParallelTimeline( null , System.Windows.Duration.Automatic);
            timeline5.RepeatBehavior  = new RepeatBehavior(TimeSpan.FromMilliseconds(99));
            
            // Create Timeline 6
            ParallelTimeline timeline6 = new ParallelTimeline( null , System.Windows.Duration.Automatic);
            timeline6.RepeatBehavior  = new RepeatBehavior(TimeSpan.FromMilliseconds(1));

            RepeatBehavior RB1 = timeline1.RepeatBehavior;
            object RB2 = (object)timeline2.RepeatBehavior;
            object RB3 = (object)timeline3.RepeatBehavior;
            RepeatBehavior RB4 = timeline4.RepeatBehavior;
            object RB5 = (object)timeline5.RepeatBehavior;
            object RB6 = (object)timeline6.RepeatBehavior;
                        
            outString += "---1 Equals 2--- " + Convert.ToString( RB1.Equals(RB2) ) + "\n";
            outString += "---2 Equals 1--- " + Convert.ToString( RB2.Equals(RB1) ) + "\n";
            outString += "---1 Equals 3--- " + Convert.ToString( RB1.Equals(RB3) ) + "\n";
            outString += "---1 Equals 5--- " + Convert.ToString( RB1.Equals(RB5) ) + "\n";
            outString += "---4 Equals 5--- " + Convert.ToString( RB4.Equals(RB5) ) + "\n";
            outString += "---4 Equals 6--- " + Convert.ToString( RB4.Equals(RB6) );
            
            return outString ;
        }
    }
}