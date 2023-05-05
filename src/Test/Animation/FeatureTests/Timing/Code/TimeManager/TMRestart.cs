// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the Restart method of TimeManager
 */

//Instructions:
//  1. Create a TimeManager
//  2. Create TimeContainer and attach two nodes to it
//  3. Start the TimeManager at timeline = 0
//  4. Restart the TimeManager after time = 9 ticks

//Pass Verification:
//   The output of this test should match the expected output in TMRestartExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected TMRestartExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class TMRestart :ITimeBVT
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

            // Create a TimeContainer
            ParallelTimeline tContainer = new ParallelTimeline();
            DEBUG.ASSERT(tContainer != null, "Cannot create TimeContainer", " Created TimeContainer ");
            tContainer.BeginTime   = TimeSpan.FromMilliseconds(0);
            tContainer.Duration = new Duration(TimeSpan.FromMilliseconds(25));
            tContainer.Name          = "Container";

            // Create a TimeNode with AutoReverse = true
            ParallelTimeline tNode1 = new ParallelTimeline();
            DEBUG.ASSERT(tNode1 != null, "Cannot create TimeNode" , " Created TimeNode " );
            tNode1.BeginTime        = TimeSpan.FromMilliseconds(0);
            tNode1.Duration         = new Duration(TimeSpan.FromMilliseconds(5));
            tNode1.AutoReverse      = true ;         
            tNode1.Name = "TimeNode1";
            
            // Create a TimeNode with RepeatCount=2
            ParallelTimeline tNode2 = new ParallelTimeline();
            DEBUG.ASSERT(tNode2 != null, "Cannot create TimeNode" , " Created TimeNode " );
            tNode2.BeginTime        = TimeSpan.FromMilliseconds(0);
            tNode2.Duration         = new Duration(TimeSpan.FromMilliseconds(5));
            tNode2.RepeatBehavior   = new RepeatBehavior(2);
            tNode2.Name = "TimeNode2";

            //Attach TimeNodes to the Container
            tContainer.Children.Add(tNode1);
            tContainer.Children.Add(tNode2);
            DEBUG.LOGSTATUS(" Attached TimeNodes to the Container ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = tContainer.CreateClock();      
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );
            //Attach Handlers to the Clock
            
            //HACK-HACK: CurrentTimeInvalidated must be associated to the clock to force a click.
            AttachCurrentTimeInvalidatedTC(tClock);
            DEBUG.LOGSTATUS(" Attached EventHandlers to the Clock ");

            //Intialize output String
            outString = "";
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, _tManager, 0, 30, 1, ref outString);
            
            return outString;
        }

        public override void PreTick( int i )
        {
            if ( i == 10 )
            {
                outString += "----------------Restarting the TimeManager now\n";
                _tManager.Restart();             
            }
        }   

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}
