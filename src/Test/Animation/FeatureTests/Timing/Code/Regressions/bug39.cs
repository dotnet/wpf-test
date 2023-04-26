// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify adding a Timeline multiple times 

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline with attached event handlers
//  3. Create a Clock, associated with the Timeline
//  4. Start the TimeManager


//Pass Condition:
//  All appropriate events should fire.

//Pass Verification:
//  The output of this test should match the expected output in bug39Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug39Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug39 :ITimeBVT
     {
          /*
           *  Function:   Test
           *  Arguments:  Framework
           */
          public override string Test()
          {
               TimeManagerInternal tManager = EstablishTimeManager(this);
               DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            for (int i = 0; i < 100; i++)
            {
                ParallelTimeline tNode = new ParallelTimeline();
                tNode.BeginTime    = TimeSpan.FromMilliseconds(i);
                tNode.Duration = new Duration(TimeSpan.FromMilliseconds(1));
                tNode.Name           = "Timeline" + i.ToString();
                AttachCurrentStateInvalidated( tNode );
                Clock tClock = tNode.CreateClock();          
            }
              
               //Intialize output String
               outString = "";
                             
               //Run the Timer
               tManager.Start();

               for (int j = 0; j <= 105; j += 1 )
               {
                    //outString += "-------------------------Processing time: " + (int) j + " ms\n";
                    CurrentTime = TimeSpan.FromMilliseconds(j);       
                    tManager.Tick();                    
               }               
               
               return outString;
          }
     }
}
