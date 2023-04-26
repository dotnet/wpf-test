// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify creating a ParallelTimeline using a Constructor with no arguments
 */

//Instructions:
//  1. Create a ParallelTimeline via the Constructor
//  2. Create a Clock and associate it with the ParallelTimeline
//  3. Create a TimeManager with Start=0 and Step=50

//Pass Condition:
//   This test passes if the events fire at the designated times.

//Pass Verification:
//   The output of this test should match the expected output in ConstructorPTL4Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected ConstructorPTL4Expect.txt file

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
    class ConstructorPTL4 :ITimeBVT
    {
        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a ParallelTimeline and checks whether events are properly caught.
         *              Logs the results.
         */
        public override string Test()
        {
            // Intialize output String
            outString = "";

            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a ParallelTimeline
            ParallelTimeline parallelTimeline = new ParallelTimeline();
            DEBUG.ASSERT(parallelTimeline != null, "Cannot create ParallelTimeline" , " Created ParallelTimeline " );          
            // Set Properties to the ParallelTimeline
            parallelTimeline.Name           = "ParallelTimeline";
            parallelTimeline.BeginTime      = TimeSpan.FromMilliseconds(2);
            parallelTimeline.Duration       = new System.Windows.Duration(TimeSpan.FromMilliseconds(5));
            DEBUG.LOGSTATUS(" Set ParallelTimeline Properties ");

            // Create a Clock, connected to the ParallelTimeline.
            Clock tClock = parallelTimeline.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            AttachCurrentTimeInvalidatedTC(tClock);
            
            // Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 10, 1, ref outString);

            return outString;
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}
