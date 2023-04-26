// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the Equals(ClockCollection) property of a ClockCollection
 */

//Instructions:
//     1. Create a Timelines
//     2. Create a Clocks, connected to the Timelines
//     3. Establish ClockCollections

//Warnings:
//     Any changes made in the output should be reflected in EqualsTLCCExpect.txt file

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
    class EqualsTLCC :ITimeBVT
    {
        /*
        *  Function:  Test
        *  Arguments: Framework
        */
        public override string Test()
        {
            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create Timeline1
            ParallelTimeline tNode1 = new ParallelTimeline();
            DEBUG.ASSERT(tNode1 != null, "Cannot create Timeline1", " Created Timeline1 ");
            tNode1.BeginTime  = TimeSpan.FromMilliseconds(0);
            tNode1.Duration = new Duration(TimeSpan.FromMilliseconds(5));

            // Create Timeline2
            ParallelTimeline tNode2 = new ParallelTimeline();
            DEBUG.ASSERT(tNode2 != null, "Cannot create Timeline2", " Created Timeline2 ");
            tNode2.BeginTime  = TimeSpan.FromMilliseconds(1);
            tNode2.Duration = new Duration(TimeSpan.FromMilliseconds(0));

            // Create Clock1, connected to Timeline1.
            ClockGroup tClock1 = tNode1.CreateClock();       
            DEBUG.ASSERT(tClock1 != null, "Cannot create Clock1" , " Created Clock1 " );

            // Create Clock2, connected to Timeline2.
            ClockGroup tClock2 = tNode1.CreateClock();       
            DEBUG.ASSERT(tClock2 != null, "Cannot create Clock2" , " Created Clock2 " );

            //Establish ClockCollections
            ClockCollection T1a = (ClockCollection)tClock1.Children;
            ClockCollection T1b = (ClockCollection)tClock1.Children;
            ClockCollection T2  = (ClockCollection)tClock2.Children;

            outString += Convert.ToString( T1a.Equals(T1b) ) + "\n";
            outString += Convert.ToString( T1b.Equals(T1a) ) + "\n";
            outString += Convert.ToString( T2.Equals(T1a) ) + "\n";
            outString += Convert.ToString( T2.Equals(T1b) ) + "\n";
            outString += Convert.ToString( T1a.Equals(T2) ) + "\n";
            outString += Convert.ToString( T1b.Equals(T2) );

            return outString;
        }
    }
}
