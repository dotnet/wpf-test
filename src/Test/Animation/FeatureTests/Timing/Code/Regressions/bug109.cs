// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  


//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline
//  3. Create a Clock, associated with the parent Timeline; attach event handlers
//  4. Start the TimeManager; invoke Timing methods at various intervals


//Pass Condition:
//  This test passes if the CurrentTime is correct after each Timing method is invoked.
//  Also, events must fire appropriately. NOTE: invoking Seek() causes ALL events to fire.

//Pass Verification:
//  The output of this test should match the expected output in bug109Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug109Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug109 :ITimeBVT
     {
          /*
           *  Function:   Test
           *  Arguments:  Framework
           */
          public override string Test()
          {
               //Intialize output String
               outString = "";
            
               //Verify events by listing them at the end.
               eventsVerify = true;

               TimeManagerInternal tManager = EstablishTimeManager(this);
               DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

               // Create a simple TimeNode
               ParallelTimeline tNode = new ParallelTimeline();
               DEBUG.ASSERT(tNode != null, "Cannot create Timeline" , " Created Timeline " );
               tNode.BeginTime          = null;
               tNode.Duration           = new Duration(TimeSpan.FromMilliseconds(1000));
               tNode.Name                 = "Timeline";

               // Create a Clock, connected to the container Timeline.
               Clock tClock = tNode.CreateClock();          
               DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

               //Attach Event Handlers to the Clock
               AttachCurrentStateInvalidatedTC( tClock );
               AttachCurrentGlobalSpeedInvalidatedTC( tClock );
               
               //Run the Timer
               tManager.Start();
                              
               for(int i = 0 ; i <= 1200; i += 100 )
               {
                    tickNumber = i;
                    
                    outString += "------------------------------" + i.ToString() + "\n";

                    CurrentTime = TimeSpan.FromMilliseconds(i);
                    tManager.Tick();
                    
                    if (200 == i)
                    {
                         tClock.Controller.Begin();     
                    }
                    if (400 == i)
                    {
                         tClock.Controller.Pause();     
                    }
                    if (600 == i)
                    {
                         tClock.Controller.Seek(TimeSpan.FromMilliseconds(800),TimeSeekOrigin.BeginTime);
                    }
                    if (800 == i)
                    {
                         tClock.Controller.Resume();     
                    }
                    outString += "CurrentTime:    "  + tClock.CurrentTime.ToString()  + "\n";
                    outString += "CurrentState:   "  + tClock.CurrentState.ToString()  + "\n";
               }

               WriteAllEvents();
               
               return outString;
          }
     }
}
