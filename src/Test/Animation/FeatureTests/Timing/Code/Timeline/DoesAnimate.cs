// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify reading a Timeline's DoesAnimate property
 */

//Instructions:
//  1. Create a Timeline 
//  2. Create a Clock associated with the Timeline
//  3. Create a TimeManager

//Pass Condition:
//   This test passes if the DoesAnimate property returns the correct value.

//Pass Verification:
//   The output of this test should match the expected output in DoesAnimateExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected DoesAnimateExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class DoesAnimate :ITimeBVT
    {
        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a Timeline and checks DoesAnimate.
         *              Logs the results
         */
        ParallelTimeline _timeline;
         
        public override string Test()
        {
            //Intialize output String
            outString = "";
            
            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeNode
            _timeline = new ParallelTimeline();
            DEBUG.ASSERT(_timeline != null, "Cannot create TimeNode" , " Created Timeline " );
            
            // Set Properties to the TimeNode
            _timeline.BeginTime       = TimeSpan.FromMilliseconds(0);
            _timeline.Duration        = new System.Windows.Duration(TimeSpan.FromMilliseconds(8));
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = _timeline.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock,  tMan, 0, 10, 1, ref outString);
            
            return outString;
        }

        public override void OnProgress( Clock subject )
        {
            outString += "------------Progress--: " + !subject.Timeline.CanFreeze + "\n";
        }
    }
}
