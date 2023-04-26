// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify the Exception when setting Deceleration and Acceleration + Deceleration > 1 

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline, with Acceleration + Deceleration > 1

//Pass Condition:
//  An appropriate Exception should be thrown.

//Pass Verification:
//  The output of this test should match the expected output in bug126Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug126Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug126 :ITimeBVT
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
                    DEBUG.ASSERT(timeline != null, "Cannot create Timeline" , " Created Timeline" );
                    timeline.AccelerationRatio = 0.02f;
                    timeline.DecelerationRatio = 0.99;
                    timeline.Freeze();  //Accel+Decel checked only when the Timeline is frozen.
               }
               catch (Exception e)
               {
                    //If no exception occurs, the output will be blank and the test will fail.
                    TimeGenericWrappers.CHECKEXCEPTION( typeof(InvalidOperationException), e.GetType(), ref outString );
               }

               return outString;
          }
     }
}
