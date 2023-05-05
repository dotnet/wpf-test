// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify event handlers are not copied when Copy is applied to a Timeline

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline, with attached event handlers
//  3. Create a Clock, associated with the parent Timeline
//  4. Start the TimeManager

//Pass Condition:
//  All event handlers should fire appropriately.

//Pass Verification:
//  The output of this test should match the expected output in bug104Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug104Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug104 :ITimeBVT
     {
          TimeManagerInternal   _tManager;

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

            _tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            //Create a TimeContainer
            ParallelTimeline tContainer = new ParallelTimeline();
            tContainer.BeginTime        = TimeSpan.FromMilliseconds(0);
            tContainer.Duration         = new Duration(TimeSpan.FromMilliseconds(20));
            tContainer.Name             = "TimeContainer";
            DEBUG.ASSERT(tContainer != null, "Cannot create a Container " , " Created a Container " );

            // Create a TimeNode
            ParallelTimeline tNode1 = new ParallelTimeline();
            DEBUG.ASSERT(tNode1 != null, "Cannot create TimeNode" , " Created Timeline " );         
            // Set Properties to the TimeNode
            tNode1.Name             = "Child";
            tNode1.BeginTime        = TimeSpan.FromMilliseconds(0);
            tNode1.Duration         = new Duration(TimeSpan.FromMilliseconds(4));
            tNode1.AutoReverse      = true;
            tNode1.RepeatBehavior   = new RepeatBehavior(2);
            DEBUG.LOGSTATUS(" Set Timeline 1 Properties ");

            //Attach Handlers to the TimeNode
            AttachAllHandlers(tNode1);
            DEBUG.LOGSTATUS(" Attached EventHandlers to the Timeline ");

            //Copy tNode1 to tNode2.
            Timeline tNode2 = tNode1.Clone();
            DEBUG.ASSERT(tNode2 != null, "Cannot create TimeNode" , " Created Timeline " );
            DEBUG.LOGSTATUS(" Set Timeline 2 Properties ");
            
            //Attach TimeNodes to the TimeContainer
            tContainer.Children.Add(tNode1);
            tContainer.Children.Add(tNode2);
            DEBUG.LOGSTATUS(" Attached TimeNodes to the TimeContainer ");

            // Create a Clock, connected to the Container.
            Clock tClock = tContainer.CreateClock();      
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );
    
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, _tManager, 0, 21, 1, ref outString);

            WriteAllEvents();
            
            return outString;
        }  
    }
}
