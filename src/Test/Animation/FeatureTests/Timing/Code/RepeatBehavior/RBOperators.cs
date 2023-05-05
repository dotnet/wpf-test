// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
// (c) Microsoft Corporation , 2005

/* 
 * Description: Verify the Duration methods
 */

//Instructions:
//     1. Create RepeatBehavior objects
//     2. Test different operators

//Warnings:
//     Any changes made in the output should be reflected RBOperatorsExpect.txt file

//Pass Condition:
//     The output from the program is compared with RBOperatorsExpect.txt file.

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class RBOperators :ITimeBVT
    {
        /*
        *  Function : Test
        *  Arguments: Framework
        */
        public override string Test()
        {
            // Create Timeline 1
            ParallelTimeline timeline1 = new ParallelTimeline( null , System.Windows.Duration.Automatic);
            timeline1.RepeatBehavior  = new RepeatBehavior(0);
            
            // Create Timeline 2
            ParallelTimeline timeline2 = new ParallelTimeline( null , System.Windows.Duration.Automatic);
            timeline2.RepeatBehavior  = new RepeatBehavior(0);
            
            // Create Timeline 3
            ParallelTimeline timeline3 = new ParallelTimeline( null , System.Windows.Duration.Automatic);
            timeline3.RepeatBehavior  = new RepeatBehavior(20);
            
            // Create Timeline 4
            ParallelTimeline timeline4 = new ParallelTimeline( null , System.Windows.Duration.Automatic);
            timeline4.RepeatBehavior  = new RepeatBehavior(TimeSpan.FromMilliseconds(99));
            
            // Create Timeline 5
            ParallelTimeline timeline5 = new ParallelTimeline( null , System.Windows.Duration.Automatic);
            timeline5.RepeatBehavior  = new RepeatBehavior(TimeSpan.FromMilliseconds(99));
            
            // Create Timeline 6
            ParallelTimeline timeline6 = new ParallelTimeline( null , System.Windows.Duration.Automatic);
            timeline6.RepeatBehavior  = new RepeatBehavior(TimeSpan.FromMilliseconds(1));


            DEBUG.LOGSTATUS("Checking Equality");
            Check( (timeline1.RepeatBehavior == timeline2.RepeatBehavior) == true , " Equality 1" );

            DEBUG.LOGSTATUS("Checking Equality");
            Check( (timeline2.RepeatBehavior == timeline1.RepeatBehavior) == true , " Equality 2" );

            DEBUG.LOGSTATUS("Checking Equality");
            Check( (timeline1.RepeatBehavior == timeline3.RepeatBehavior) == false , " Equality 3" );

            DEBUG.LOGSTATUS("Checking Equality");
            Check( (timeline4.RepeatBehavior == timeline5.RepeatBehavior) == true , " Equality 4" );

            DEBUG.LOGSTATUS("Checking Equality");
            Check( (timeline5.RepeatBehavior == timeline4.RepeatBehavior) == true , " Equality 5" );

            DEBUG.LOGSTATUS("Checking Equality");
            Check( (timeline6.RepeatBehavior == timeline5.RepeatBehavior) == false , " Equality 6" );

            DEBUG.LOGSTATUS("Checking Equality");
            Check( (timeline1.RepeatBehavior == timeline5.RepeatBehavior) == false , " Equality 7" );

            DEBUG.LOGSTATUS("Checking Inequality");
            Check( (timeline1.RepeatBehavior != timeline2.RepeatBehavior) == false , " Inequality 1" );

            DEBUG.LOGSTATUS("Checking Inequality");
            Check( (timeline2.RepeatBehavior != timeline3.RepeatBehavior) == true , " Inequality 2" );

            DEBUG.LOGSTATUS("Checking Inequality");
            Check( (timeline4.RepeatBehavior != timeline5.RepeatBehavior) == false , " Inequality 3" );

            DEBUG.LOGSTATUS("Checking Inequality");
            Check( (timeline4.RepeatBehavior != timeline6.RepeatBehavior) == true , " Inequality 4" );

            DEBUG.LOGSTATUS("Checking Inequality");
            Check( (timeline1.RepeatBehavior != timeline6.RepeatBehavior) == true , " Inequality 5" );


            return outString;
        }

        private void Check( bool condition, string message )
        {
            outString += message + " : " + ( condition ? "SUCCESS" : "FAILED" ) + "\n" ;
        }
    }
}
