// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the Stop method of TimeManager
 */

//Instructions:
//  1. Create a TimeManager
//  2. Create a TimeNode with begin=0 and duration=20 and attach to Manager
//  3. Start the TimeManager at timeline = 0
//  4. At time = 7 stop the Manager by calling stop() method and time = 9 start the Manager again
//  5. Check the CurrentProgress and CurrentState properties after each Tick

//Pass Verification:
//   The output of this test should match the expected output in TMStopStartExpect.txt

//Warnings:
//  Any changes made in the output should be reflected TMStopStartExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class TMStopStart :ITimeBVT
    {
        TimeManagerInternal _tManager;
        /*
         *  Function:   Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            _tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");
            
            // Create a TimeNode
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create TimeNode" , " Created TimeNode " );
            tNode.BeginTime     = TimeSpan.FromMilliseconds(0);
            tNode.Duration      = new Duration(TimeSpan.FromMilliseconds(20));
            tNode.Name          = "TimeNode";
            
            // Create a Clock, connected to the Timeline.
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            AttachCurrentTimeInvalidatedTC(tClock);

            // Intialize output String
            outString = "";
            
            // Run the Timer         
            TimeGenericWrappers.TESTSTATE( this, tClock, _tManager, 0, 30, 1, ref outString);
            
            return outString;
        }

        public override void PostTick( int i )
        {
            if ( i == 7 )
            {
                _tManager.Stop();
            }

            if ( i == 9 )
            {
                _tManager.Start();
            }
        }       

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}
