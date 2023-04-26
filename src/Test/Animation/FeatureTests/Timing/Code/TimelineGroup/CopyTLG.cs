// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the Copy method for TimelineGroup
 */

//Instructions:
//     1. Create a TimelineGroup
//     2. Use Copy() to copy the TimelineGroup to a second TimelineGroup

//Pass Condition:
//     This test passes if a second TimelinetGroup is created.

//Warnings:
//     Any changes made in the output should be reflected in CopyTLGExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class CopyTLG :ITimeBVT
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

               // Create a TimelineGroup
               ParallelTimeline TL = new ParallelTimeline();
               TimelineGroup TLG1 = (TimelineGroup)TL;
               DEBUG.ASSERT(TLG1 != null, "Cannot create TimelineGroup", " Created TimelineGroup ");
               TLG1.BeginTime  = TimeSpan.FromMilliseconds(4);
               TLG1.Duration   = new Duration(TimeSpan.FromMilliseconds(8));
               TLG1.Name         = "Group1";

               TimelineGroup TLG2 = TLG1.Clone();
               TLG2.Name         = "Group2";
               DEBUG.LOGSTATUS(" Applied Copy ");

               outString += "--- " + TLG1.Name + "-- " + TLG1 + "\n";
               outString += "--- " + TLG2.Name + "-- " + TLG2 + "\n";

               return outString;
          }
     }
}
