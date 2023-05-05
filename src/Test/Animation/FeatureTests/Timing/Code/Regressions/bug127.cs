// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify Freeze() on a Timeline makes child timelines unchangeable

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline tree
//  3. Create a Clock, associated with the topmost Timeline
//  4. Start the TimeManager


//Pass Condition:
//  All appropriate events should fire.

//Pass Verification:
//  The output of this test should match the expected output in bug127Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug127Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug127 :ITimeBVT
     {
          /*
           *  Function:   Test
           *  Arguments:  Framework
           */
          public override string Test()
          {
               TimeManagerInternal tManager = EstablishTimeManager(this);
               DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

               outString = "";

               ParallelTimeline parent = new ParallelTimeline();
               ParallelTimeline child = new ParallelTimeline();

              
               parent.Children.Add(child);

               // parent may "use" child, so get child back from parent:
               child = (ParallelTimeline)parent.Children[0];

               outString += "--- Parent !IsFrozen: "  + !parent.IsFrozen + "\n";
               outString += "--- Child !IsFrozen:  "  + !child.IsFrozen  + "\n";

               // Make parent unchangeable. Should make both objects unchangeable.
               // This might change the child reference -- that's OK, so get it
               // back after calling Freeze
               parent.Freeze();
               child = (ParallelTimeline)parent.Children[0];

               // Now, parent is only truly unchangeable if all of its children
               // are also unchangeable. Otherwise, I can make a change that has
               // a side-effect on parent by changing child:
               outString += "--- Parent !IsFrozen: "  + !parent.IsFrozen + "\n";
               outString += "--- Child !IsFrozen:  "  + !child.IsFrozen;
               
               return outString;
          }
     }
}
