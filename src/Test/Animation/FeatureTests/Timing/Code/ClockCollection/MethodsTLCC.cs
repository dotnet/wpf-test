// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify applying ClockCollection methods
 */

//Instructions:
//  1. Create a Timeline.
//  2. Create a Clock from the Timeline.
//  3. Create a ClockCollection

//Pass Condition:
//  An appropriate exception should be thrown, until these methods are implemented.

//Pass Verification:
//  The output of this test should match the expected output in MethodsTLCCExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected MethodsTLCCExpect.txt file

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
    class MethodsTLCC :ITimeBVT
    {
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
            ParallelTimeline timeline1 = new ParallelTimeline();
            DEBUG.ASSERT(timeline1 != null, "Cannot create Timeline 1" , " Created Timeline 1" );

            // Create another Timeline
            ParallelTimeline timeline2 = new ParallelTimeline();
            DEBUG.ASSERT(timeline2 != null, "Cannot create Timeline 2" , " Created Timeline 2" );

            // Create a Clock, connected to Timeline 1.
            ClockGroup clock1 = timeline1.CreateClock();       
            DEBUG.ASSERT(clock1 != null, "Cannot create Clock 1" , " Created Clock 1" );

            // Create a Clock, connected to Timeline 2.
            Clock clock2 = timeline2.CreateClock();       
            DEBUG.ASSERT(clock2 != null, "Cannot create Clock 2" , " Created Clock 2" );

            //Establish a ClockCollection
            ClockCollection TLCC = (ClockCollection)clock1.Children;

            try
            {
                TLCC.Add(clock2);
            }
            catch (Exception e1)
            {
                TimeGenericWrappers.CHECKEXCEPTION( typeof(System.NotSupportedException), e1.GetType(), ref outString );
            }

            try
            {
                TLCC.Remove(clock2);
            }
            catch (Exception e2)
            {
                TimeGenericWrappers.CHECKEXCEPTION( typeof(System.NotSupportedException), e2.GetType(), ref outString );
            }

            try
            {
                TLCC.Clear();
            }
            catch (Exception e3)
            {
                TimeGenericWrappers.CHECKEXCEPTION( typeof(System.NotSupportedException), e3.GetType(), ref outString );
            }

            return outString;
        }
    }
}
