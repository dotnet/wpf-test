// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify reading a ClockController's default properties
*/

//Instructions:
//  1. Create a Timeline, without setting any properties
//  2. Read ClockController properties.

//Pass Condition:
//   This test passes if the actual values of the properties match the expected values.

//Pass Verification:
//   The output of this test should match the expected output in ICDefaultsExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected ICDefaultsExpect.txt file

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
    class ICDefaults :ITimeBVT
    {
        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a Timeline and checks default values.
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
            DEBUG.ASSERT(timeline != null, "Cannot create Timeline" , " Created Timeline " );

            // Create a Clock, connected to the Timeline.
            Clock tClock = timeline.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            outString += "ClockController = " +  tClock.Controller + "\n";
            outString += "Clock           = " +  tClock.Controller.Clock + "\n";
            outString += "SpeedRatio      = " +  tClock.Controller.SpeedRatio;

            return outString;
        }
    }
}
