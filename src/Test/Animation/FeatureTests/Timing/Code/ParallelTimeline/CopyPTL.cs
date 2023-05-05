// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the Copy method for ParallelTimeline
 */

//Instructions:
//     1. Create a ParallelTimeline
//     2. Use Copy() to copy the ParallelTimeline to a second ParallelTimeline

//Pass Condition:
//     This test passes if a second TimelinetGroup is created.

//Warnings:
//     Any changes made in the output should be reflected in CopyPTLExpect.txt file

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
     class CopyPTL :ITimeBVT
     {
          /*
           *  Function:  Test
           *  Arguments: Framework
           */
          public override string Test()
          {
               //Intialize output String
               outString = "";

               // Create a TimeManager
               TimeManagerInternal tMan = EstablishTimeManager(this);
               DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

               // Create a ParallelTimeline
               ParallelTimeline PTL1 = new ParallelTimeline();
               DEBUG.ASSERT(PTL1 != null, "Cannot create ParallelTimeline", " Created ParallelTimeline ");
               PTL1.BeginTime  = TimeSpan.FromMilliseconds(4);
               PTL1.Duration   = new Duration(TimeSpan.FromMilliseconds(8));
               PTL1.Name         = "ParallelTimeline1";

               ParallelTimeline PTL2 = PTL1.Clone();
               PTL2.Name         = "ParallelTimeline2";
               DEBUG.LOGSTATUS(" Applied Copy ");

               outString += "--- " + PTL1.Name + "-- " + PTL1 + "\n";
               outString += "--- " + PTL2.Name + "-- " + PTL2 + "\n";

               return outString;
          }
     }
}
