// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the Equals(object) property of a ClockCollection
 */

//Instructions:
//     1. Create a Timeline
//     2. Create a Clock, connected to the Timeline.
//     3. Establish a ClockCollection

//Warnings:
//     Any changes made in the output should be reflected in EqualsObjectTLCCExpect.txt file

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
    class EqualsObjectTLCC :ITimeBVT
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
            tNode1.Duration = new Duration(TimeSpan.FromMilliseconds(1));

            // Create Timeline2
            ParallelTimeline tNode2 = new ParallelTimeline();
            DEBUG.ASSERT(tNode2 != null, "Cannot create Timeline2", " Created Timeline2 ");
            tNode2.BeginTime  = TimeSpan.FromMilliseconds(9999);
            tNode2.Duration = new Duration(TimeSpan.FromMilliseconds(9999));

            // Create Clock1, connected to Timeline1.
            ClockGroup tClock1 = tNode1.CreateClock();       
            DEBUG.ASSERT(tClock1 != null, "Cannot create Clock1" , " Created Clock1 " );

            // Create Clock2, connected to Timeline2.
            ClockGroup tClock2 = tNode1.CreateClock();       
            DEBUG.ASSERT(tClock2 != null, "Cannot create Clock2" , " Created Clock2 " );

            //Establish ClockCollections
            ClockCollection TLCC = (ClockCollection)tClock1.Children;
            object                  Obj1 = (object)tClock1.Children;
            object                  Obj2 = (object)tNode1;

            outString += Convert.ToString( TLCC.Equals(Obj1) ) + "\n";
            outString += Convert.ToString( TLCC.Equals(Obj2) ) + "\n";

            return outString;
        }
    }
}
