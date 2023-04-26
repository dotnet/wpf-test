// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify reading the CurrentState property inside the Tick loop
 */

//Instructions:
//   1. Create a Timeline with Begin = 0, Duration = 500, AutoReverse = true, RepeatCount = 2
//   2. Create a TimeManager with Start = 0 and Step = 100 and add the TimeNode

//Pass Condition:
//   This test passes if Active returns True only during the Duration period .

//Pass Verification:
//   The output of this test should match the expected output in ActiveInLoopExpect.txt.

//Warnings:
//     Any changes made in the output should be reflected ActiveInLoopExpect.txt file

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
     class ActiveInLoop :ITimeBVT
     {
        Clock _tClock;
          /*
           *  Function:    Test
           *  Arguments:   Framework
           *  Description: Constructs a Timeline and Checks whether events are properly caught.
           *                    Logs the results
           */
          public override string Test()
          {
               // Create a TimeManager
               TimeManagerInternal tMan = EstablishTimeManager(this);
               DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

               // Create a TimeNode
               ParallelTimeline tNode = new ParallelTimeline();
               DEBUG.ASSERT(tNode != null, "Cannot create TimeNode" , " Created Timeline " );
               
               // Set Properties to the TimeNode
               tNode.BeginTime       = TimeSpan.FromMilliseconds(0);
               tNode.Duration        = new Duration(TimeSpan.FromMilliseconds(200));
               tNode.RepeatBehavior  = new RepeatBehavior(2);
               tNode.AutoReverse     = true;
               DEBUG.LOGSTATUS(" Set Timeline Properties ");

               // Create a Clock, connected to the Timeline.
               _tClock = tNode.CreateClock();          
               DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

               //Intialize output String
               outString = "";
               
               //Run the Timer
               tMan.Start();

               for(int i = 0; i <= 1000; i += 100 )
               {
                    outString += i + " ms  CurrentState: " + _tClock.CurrentState + "\n";
                    DEBUG.LOGSTATUS("Processing time " + i);
                    
                    CurrentTime = TimeSpan.FromMilliseconds(i);       
                    tMan.Tick();                    
               }               
               return outString;
          }
     }
}
