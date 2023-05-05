// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the CloneCurrentValue method for TimelineGroup
 */

//Instructions:
//     1. Create a TimelineGroup
//     2. Use CloneCurrentValue() to copy the TimelineGroup to a second TimelineGroup

//Pass Condition:
//     This test passes if a second TimelineGroup is created.

//Warnings:
//     Any changes made in the output should be reflected in CloneCurrentValueTLGExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class CloneCurrentValueTLG :ITimeBVT
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

               TimelineGroup TLG2 = TLG1.CloneCurrentValue();
               TLG2.Name         = "Group2";
               DEBUG.LOGSTATUS(" Applied CloneCurrentValue ");

               outString += "TLG1.Name:         " + TLG1.Name + "  --  " + TLG1 + "\n";
               outString += "TLG1.Name:         " + TLG2.Name + "  --  " + TLG2 + "\n\n";
               
               outString += "TLG1.BeginTime:    " + TLG1.BeginTime + "\n";
               outString += "TLG2.BeginTime:    " + TLG2.BeginTime + "\n\n";

               outString += "TLG1.Duration:     " + TLG1.Duration + "\n";
               outString += "TLG2.Duration:     " + TLG2.Duration + "\n";

               return outString;
          }
     }
}
