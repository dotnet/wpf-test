// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify adding Timelines to parent Timelines

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline tree


//Pass Condition:
//  The Progress property should be rounded appropriately.

//Pass Verification:
//  The output of this test should match the expected output in bug137Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug137Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug137 :ITimeBVT
     {
          /*
           *  Function:   Test
           *  Arguments:  Framework
           */
          public override string Test()
          {
               //Intialize output String
               outString = "";

               ParallelTimeline timeline1 = new ParallelTimeline();
               ParallelTimeline timeline2 = new ParallelTimeline();
               ParallelTimeline timeline3 = new ParallelTimeline();

               timeline2.Children.Add(timeline3);
               timeline1.Children.Add(timeline2); //This statement originally produced an Assert.
               
               //Obtain a TimelineCollection for the Container
               TimelineCollection TLC1 = timeline1.Children;
               TimelineCollection TLC2 = timeline2.Children;

               outString += "timeline1 Count:    " + TLC1.Count + "\n";
               outString += "timeline2 Count:    " + TLC2.Count + "\n";
               
               return outString;
          }
     }
}
