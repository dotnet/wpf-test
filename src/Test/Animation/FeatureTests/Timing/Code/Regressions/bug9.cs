// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify a TimelineClock's CurrentGlobalSpeed during a fill period

*/

//Instructions:
//  1. Create a Timeline, in which Duration is set to Duration.Forever.
//  2. Create a Clock, based on the Timeline.
//  3. Read the CurrentGlobalSpeed property.


//Pass Condition:
//  This test passes if CurrentGlobalSpeed is null during the fill period.

//Pass Verification:
//  The output of this test should match the expected output in bug9Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected in the bug9Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug9 :ITimeBVT
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
            timeline.BeginTime      = TimeSpan.FromMilliseconds(0);
            timeline.Duration       = new Duration (new TimeSpan(0,0,0,0,10));
            timeline.FillBehavior   = FillBehavior.HoldEnd;
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            // Create a Clock, connected to the Timeline.
            _tClock = timeline.CreateClock();       
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, _tClock, tMan, 0, 12, 1, ref outString);

            return outString;
        }
        
        public override void OnProgress( Clock subject )
        {
            outString += "-----------" + ((Clock)subject).Timeline.Name + ": CurrentGlobalSpeed   = " + ((Clock)subject).CurrentGlobalSpeed + "\n";
            outString += "-----------" + ((Clock)subject).Timeline.Name + ": CurrentState         = " + ((Clock)subject).CurrentState + "\n";
        }
    }
}
