// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify a Timeline's RepeatBehavior.Count when it is not set

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline tree
//  3. Create a Clock, associated with the topmost Timeline
//  4. Start the TimeManager
//  5. Read the value of RepeatCount at each tick


//Pass Condition:
//  The RepeatCount should return 0.

//Pass Verification:
//  The output of this test should match the expected output in bug37Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug37Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug37 :ITimeBVT
    {
        TimeManagerInternal         _tManager;
        ParallelTimeline            _tNode;
        Clock                       _tClock;
        
        /*
         *  Function:   Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            _tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeContainer
            _tNode = new ParallelTimeline();
            DEBUG.ASSERT(_tNode != null, "Cannot create TimeContainer", " Created TimeContainer ");
            _tNode.BeginTime    = TimeSpan.FromMilliseconds(2);
            _tNode.Duration     = new Duration(TimeSpan.FromMilliseconds(50));
            _tNode.Name           = "Timeline";

            //Attach Handlers to the Timeline
            AttachCurrentGlobalSpeedInvalidated( _tNode );
            DEBUG.LOGSTATUS(" Attached EventHandlers to the Timeliness ");

            // Create a Clock, connected to the Timeline.
            _tClock = _tNode.CreateClock();     
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            //Intialize output String
            outString = "";
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 60, 5, ref outString);
            
            return outString;
        }
          
        public override void OnProgress( Clock subject )
        {
            outString += "  " + ((Clock)subject).Timeline.Name + ": Progress.  RepeatCount: " + ((Clock)subject).Timeline.RepeatBehavior.Count + "\n";               
        }
    }
}
