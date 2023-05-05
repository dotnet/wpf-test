// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify setting Duration to a negative value

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline and set its properties


//Pass Condition:
//  An appropriate exception should be thrown.

//Pass Verification:
//  The output of this test should match the expected output in bug134Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug134Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug134 :ITimeBVT
     {
          /*
           *  Function:   Test
           *  Arguments:  Framework
           */
          public override string Test()
          {
               TimeManagerInternal tManager = EstablishTimeManager(this);
               DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

               //Intialize output String
               outString = "";

               try
               {
                    ParallelTimeline timeline = new ParallelTimeline();
                    timeline.Duration = new Duration(TimeSpan.FromMilliseconds(-100));

               }
               catch (Exception e)
               {
                    //If no exception occurs, the output will be blank and the test will fail.
                    TimeGenericWrappers.CHECKEXCEPTION( typeof(System.ArgumentException), e.GetType(), ref outString );
               }
               
               return outString;
          }
     }
}
