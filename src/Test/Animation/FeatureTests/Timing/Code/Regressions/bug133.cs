// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify passing an invalid value to Seek - TimeManager

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline and set its properties


//Pass Condition:
//  An appropriate exception should be thrown.

//Pass Verification:
//  The output of this test should match the expected output in bug133Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug133Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug133 :ITimeBVT
     {
          Clock _tClock;
          TimeManagerInternal _tManager;

          /*
           *  Function:   Test
           *  Arguments:  Framework
           */
          public override string Test()
          {
               _tManager = EstablishTimeManager(this);
               DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

               // Create a Timeline
               ParallelTimeline timeline = new ParallelTimeline();
               DEBUG.ASSERT(timeline != null, "Cannot create Timeline", " Created Timeline ");
               timeline.BeginTime       = TimeSpan.FromMilliseconds(20);
               timeline.Duration        = new Duration(TimeSpan.FromMilliseconds(100));
               timeline.Name            = "Timeline";


               // Create a Clocks, connected to the Timelines.
               _tClock = timeline.CreateClock();          
               DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
                              
               outString = "";

               //Run the Timer               
               TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 50, 10, ref outString);
               
               return outString;
          }

          public override void PreTick( int i )
          {
               if ( i == 40 )
               {
                    try
                    {
                         _tManager.Seek(200, (TimeSeekOrigin)3);
                    }
                    catch (Exception e1)
                    {
                        //If no exception occurs, the output will be blank and the test will fail.
                        TimeGenericWrappers.CHECKEXCEPTION( typeof(System.Reflection.TargetInvocationException), e1.GetType(), ref outString );
                    }
               }
          }
     }
}
