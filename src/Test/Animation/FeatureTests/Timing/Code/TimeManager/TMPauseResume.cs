// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the Pause and Resume methods of TimeManager
 */

//Instructions:
//  1. Create a TimeManager
//  2. Attach two nodes (one with reverse and one with repeat) to it
//  3. Start the TimeManager at timeline = 0
//  4. Pause the TimeManager at every timeline%7 == 0
//  5. Resume the TimeManager at every timeline%9 == 0

//Pass Verification:
//   The output of this test should match the expected output in TMPauseResumeExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected TMPauseResumeExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class TMPauseResume :ITimeBVT
    {
        TimeManagerInternal _tManager;

        /*
         *  Function:   Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";

            _tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeContainer
            ParallelTimeline tContainer = new ParallelTimeline();
            DEBUG.ASSERT(tContainer != null, "Cannot create TimeContainer", " Created TimeContainer ");
            tContainer.BeginTime   = TimeSpan.FromMilliseconds(0);
            tContainer.Duration = new Duration(TimeSpan.FromMilliseconds(80));
            tContainer.Name          = "Container";

            // Create a TimeNode with AutoReverse = true
            ParallelTimeline tNode1 = new ParallelTimeline();
            DEBUG.ASSERT(tNode1 != null, "Cannot create TimeNode" , " Created TimeNode " );
            tNode1.BeginTime    = TimeSpan.FromMilliseconds(0);
            tNode1.Duration = new Duration(TimeSpan.FromMilliseconds(20));
            tNode1.AutoReverse  = true ;            
            tNode1.Name = "TimeNode1";
            
            // Create a TimeNode with RepeatCount=2
            ParallelTimeline tNode2 = new ParallelTimeline();
            DEBUG.ASSERT(tNode2 != null, "Cannot create TimeNode" , " Created TimeNode " );
            tNode2.BeginTime       = TimeSpan.FromMilliseconds(0);
            tNode2.Duration = new Duration(TimeSpan.FromMilliseconds(20));
            tNode2.RepeatBehavior  = new RepeatBehavior(2);
            tNode2.Name = "TimeNode2";

            //Attach TimeNodes to the Container
            tContainer.Children.Add(tNode1);
            tContainer.Children.Add(tNode2);
            DEBUG.LOGSTATUS(" Attached TimeNodes to the Container ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = tContainer.CreateClock();      
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            AttachCurrentTimeInvalidatedTC(tClock);
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, _tManager, 0, 80, 1, ref outString);
            
            return outString;
        }

        public override void PostTick( int i )
        {
            if ( i > 1 )
            {
                if ( i % 7 == 0 )
                    _tManager.Pause();
                if ( i % 9 == 0 )
                    _tManager.Resume();
            }
        }       

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}
