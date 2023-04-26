// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify invoking SkipToFill() when the Timeline's RepeatBehavior is Forever

*/

//Instructions:
//  1. Create a Timeline, in which Duration is set to Duration.Forever.
//  2. Create a Clock, based on the Timeline.
//  3. Invoke SkipToFill().


//Pass Condition:
//  This test passes if no exception is thrown.

//Pass Verification:
//  The output of this test should match the expected output in bug14Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected in the bug14Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug14 :ITimeBVT
    {
        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a Timeline and Seeks past the end of the Duration.
         *               Logs the results.
         */
        private Clock _tClock;
        
        public override string Test()
        {
            //Intialize output String
            outString = "";

            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeNode
            ParallelTimeline timeline = new ParallelTimeline();
            DEBUG.ASSERT(timeline != null, "Cannot create Timeline" , " Created Timeline " );
            
            // Set Properties to the TimeNode
            timeline.BeginTime          = TimeSpan.FromMilliseconds(0);
            timeline.RepeatBehavior     = RepeatBehavior.Forever;
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            // Create a Clock, connected to the Timeline.
            _tClock = timeline.CreateClock();       
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, _tClock, tMan, 0, 8, 1, ref outString);

            return outString;
        }
        
        public override void PostTick( int i )
        {
            if ( i == 5 )
            {
                _tClock.Controller.SkipToFill();
            }
        } 
    }
}
