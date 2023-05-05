// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the CurrentGlobalSpeedInvalidated event of a Clock
 */

//Instructions:
//  1. Create a TimeManager.
//  2. Create a Timeline with Begin=0 and Duration=600.
//  3. Attach the CurrentGlobalSpeedInvalidated event.
//  4. Add the Clock.
//  5. Invoke the Pause and Resume methods during Tick intervals to fire the events.

//Pass Condition:
//   This test passes if the CurrentGlobalSpeedInvalidated event fires appropriately.

//Pass Verification:
//   The output of this test should match the expected output in EventsPauseResumeExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected EventsPauseResumeExpect.txt file

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
    class EventsPauseResume :ITimeBVT
    {
        /*
         *  Function:   Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            TimeManagerInternal tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a simple TimeNode
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create TimeNode" , " Created Timeline " );
            tNode.BeginTime     = TimeSpan.FromMilliseconds(0);
            tNode.Duration      = new Duration(TimeSpan.FromMilliseconds(600));         
            tNode.Name          = "TimeNode1";           
            
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );
            
            AttachCurrentGlobalSpeedInvalidatedTC(tClock);
            DEBUG.LOGSTATUS(" Attached EventHandler to the Timeline ");
                    
            //Intialize output String
            outString = "";
                        
            //Run the Timer     
            tManager.Start();
                        
            for(int i = 0 ; i <= 900; i += 100 )
            {
                outString += "----" + i.ToString() + "\n";

                CurrentTime = TimeSpan.FromMilliseconds(i);
                tManager.Tick();
                
                if (200 == i)
                {
                    tClock.Controller.Pause();   
                }
                if (500 == i)
                {
                    tClock.Controller.Resume();  
                }
                outString += "IsPaused:               "  + tClock.IsPaused.ToString()              + "\n";
                outString += "CurrentGlobalSpeed:     "  + tClock.CurrentGlobalSpeed.ToString()           + "\n";
            }
            
            return outString;
        }       
    }
}
