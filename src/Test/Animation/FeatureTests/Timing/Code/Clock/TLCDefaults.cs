// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify reading a Clock's default properties
 */

//Instructions:
//  1. Create a ParallelTimeline, without setting any properties
//  2. Read Clock properties.

//Pass Condition:
//  This test passes if the actual values of the properties match the expected values.

//Pass Verification:
//  The output of this test should match the expected output in TLCDefaultsExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected TLCDefaultsExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    class TLCDefaults :ITimeBVT
    {
        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a ParallelTimeline and checks default values.
         *              Logs the results.
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";

            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeNode
            ParallelTimeline timeline = new ParallelTimeline();
            DEBUG.ASSERT(timeline != null, "Cannot create ParallelTimeline" , " Created ParallelTimeline " );

            // Create a Clock, connected to the ParallelTimeline.
            ClockGroup tClock = timeline.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            outString += "Clock         = " +  tClock + "\n";
            outString += "Children              = " +  tClock.Children + "\n";
            outString += "CurrentGlobalSpeed    = " +  tClock.CurrentGlobalSpeed + "\n";
            outString += "CurrentTime           = " +  tClock.CurrentTime + "\n";
            outString += "ClockController       = " +  tClock.Controller + "\n";
            outString += "CurrentState          = " +  tClock.CurrentState + "\n";
            outString += "IsPaused              = " +  tClock.IsPaused + "\n";
            outString += "Parent                = " +  tClock.Parent + "\n";
            outString += "Progress              = " +  tClock.CurrentProgress + "\n";
            outString += "Timeline              = " +  tClock.Timeline + "\n";
            outString += "NaturalDuration       = " +  tClock.NaturalDuration + "\n";
            outString += "CurrentIteration      = " +  tClock.CurrentIteration + "\n";
            return outString;
        }
    }
}
