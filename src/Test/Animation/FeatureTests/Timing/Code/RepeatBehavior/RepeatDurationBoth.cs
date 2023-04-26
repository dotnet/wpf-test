// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify a RepeatBehavior is set with a TimeSpan, then set with a double
  */

//Instructions:
//  1. Create a Timeline, setting new RepeatBehavior twice
//  2. Start the TimeManager

//Pass Condition:
//  This test passes if the the Timeline repeats according to the last setting.

//Pass Verification:
//  The output of this test should match the expected output in RepeatDurationBothExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected RepeatDurationBothExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class RepeatDurationBoth :ITimeBVT
    {
        ParallelTimeline    _timeline;
        Clock       _tClock1;
        Clock       _tClock2;
        
        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a Timeline and Checks whether events are properly caught.
         *              Logs the results.
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";

            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a Timeline
            _timeline = new ParallelTimeline();
            DEBUG.ASSERT(_timeline != null, "Cannot create Timeline" , " Created Timeline " );
            
            // Set Properties on the Timeline, to be used by two different Clocks
            _timeline.BeginTime       = null;
            _timeline.Name              = "Timeline1";
            _timeline.Duration        = new Duration(TimeSpan.FromMilliseconds(3));
            _timeline.RepeatBehavior  = new RepeatBehavior(TimeSpan.FromMilliseconds(12));
            _timeline.RepeatBehavior  = new RepeatBehavior(5d);  //Last one wins.
            DEBUG.LOGSTATUS(" Set Timeline Properties ");
            
            // Run the Timer
            tMan.Start();
            for(int i = 0; i <= 31; i += 1 )
            {
                outString += "Processing time " + i + " ms\n";                       
                CurrentTime = TimeSpan.FromMilliseconds(i);
                
                if (i >= 0 && i < 16)
                {
                    if (i == 0)
                    {
                        StartClock1();
                    }
                    outString += "  CurrentProgress  = " + _tClock1.CurrentProgress + "\n";
                    outString += "  CurrentIteration = " + _tClock1.CurrentIteration + "\n";
                }
                if (i >= 16)
                {
                    if (i == 16)
                    {
                        _tClock1.Controller.Stop();
                        StartClock2();
                    }
                    outString += "  CurrentProgress  = " + _tClock2.CurrentProgress + "\n";
                    outString += "  CurrentIteration = " + _tClock2.CurrentIteration + "\n";
                }
                
                tMan.Tick();
            }           
  
            return outString;
        }
        
        private void StartClock1()
        {
            // Create Clock #1, connected to the Timeline.
            _tClock1 = _timeline.CreateClock();       
            DEBUG.ASSERT(_tClock1 != null, "Cannot create Clock 1" , " Created Clock 1" );
            
            AttachRepeatTC(_tClock1);
            DEBUG.LOGSTATUS(" Attached EventHandler to Clock 1 ");
            
            outString += "RepeatBehavior = " +  _timeline.RepeatBehavior + "\n";
            outString += "HasDuration    = " +  _timeline.RepeatBehavior.HasDuration + "\n";
            outString += "HasCount       = " +  _timeline.RepeatBehavior.HasCount + "\n";
            
            _tClock1.Controller.Begin();
        }        
        private void StartClock2()
        {
            //Change the Timeline properties for Clock #2
            _timeline.RepeatBehavior  = new RepeatBehavior(5d);
            _timeline.RepeatBehavior  = new RepeatBehavior(TimeSpan.FromMilliseconds(12)); //Last one wins.
            _timeline.Name              = "Timeline2";

            // Create Clock #2, connected to the changed Timeline. 
            _tClock2 = _timeline.CreateClock();       
            DEBUG.ASSERT(_tClock2 != null, "Cannot create Clock 2" , " Created Clock 2" );
            
            AttachRepeatTC(_tClock2);
            DEBUG.LOGSTATUS(" Attached EventHandler to Clock 2 ");
            
            iterationCount = 0;  //Reset counter.
            
            outString += "RepeatBehavior = " +  _timeline.RepeatBehavior + "\n";
            outString += "HasDuration    = " +  _timeline.RepeatBehavior.HasDuration + "\n";
            outString += "HasCount       = " +  _timeline.RepeatBehavior.HasCount + "\n";
                    
            _tClock2.Controller.Begin();
        }
    }
}
