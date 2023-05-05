// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify setting and getting the Timeline's AutoReverse property
*/

//Instructions:
//  1. Create a Timeline tree and attach properties
//  2. Create a Clock and associate it with the root Timeline
//  3. Create a TimeManager with Start=0 and Step = 1 and add the Container


//Warnings:
//  Any changes made in the output should be reflected in AutoReverseExpect.txt file

//Pass Condition:
//  Test Passes when the output matches the file AutoReverseExpect.txt

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class AutoReverse :ITimeBVT
    {
        ParallelTimeline    _tContainer,_tNode1,_tNode2;
        Clock       _tClock;
        /*
         *  Function:  Test
         *  Arguments: Framework
         */
        public override string Test()
        {
            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeContainer
            _tContainer = new ParallelTimeline();
            DEBUG.ASSERT(_tContainer != null, "Cannot create TimeContainer", " Created TimeContainer ");

            // Set Properties to the TimeContainer
            _tContainer.BeginTime    = TimeSpan.FromMilliseconds(0);
            _tContainer.Duration     = new System.Windows.Duration(TimeSpan.FromMilliseconds(100));
            _tContainer.AutoReverse  = true;
            _tContainer.Name         = "Container";
            DEBUG.LOGSTATUS(" Set TimeContainer Properties ");
            
            // Create TimeNode 1
            _tNode1 = new ParallelTimeline();
            DEBUG.ASSERT(_tNode1 != null, "Cannot create TimeNode1" , " Created TimeNode1 " );           
            // Set Properties to the TimeNode
            _tNode1.BeginTime        = TimeSpan.FromMilliseconds(10);
            _tNode1.Duration         = new System.Windows.Duration(TimeSpan.FromMilliseconds(20));
            _tNode1.AutoReverse      = true;
            _tNode1.Name             = "TimeNode1";
            DEBUG.LOGSTATUS(" Set Timeline 1 Properties ");

            // Create TimeNode 2
            _tNode2 = new ParallelTimeline();
            DEBUG.ASSERT(_tNode2 != null, "Cannot create TimeNode2" , " Created TimeNode2 " );           
            // Set Properties to the TimeNode
            _tNode2.BeginTime        = TimeSpan.FromMilliseconds(20);
            _tNode2.Duration         = new System.Windows.Duration(TimeSpan.FromMilliseconds(20));
            _tNode2.AutoReverse      = false;
            _tNode2.Name             = "TimeNode2";
            DEBUG.LOGSTATUS(" Set Timeline 2 Properties ");

            //Create a tree of Timelines
            _tContainer.Children.Add(_tNode1);
            _tContainer.Children.Add(_tNode2);
            //tContainer .Children.Add(tNode3);
            DEBUG.LOGSTATUS(" Create a tree of Timelines ");

            // Create a Clock, connected to the container.
            _tClock = _tContainer.CreateClock();        
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            //Intialize output String
            outString = "";
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, _tClock, tMan, 0, 220, 10, ref outString);
            
            return outString;   
        }
        
        public override void OnProgress( Clock subject )
        {
            outString += "  " + ((Clock)subject).Timeline.Name + ": CurrentProgress    = " + subject.CurrentProgress + "\n";
            outString += "  " + ((Clock)subject).Timeline.Name + ": CurrentState       = " + subject.CurrentState + "\n";
            outString += "  " + ((Clock)subject).Timeline.Name + ": CurrentGlobalSpeed = " + subject.CurrentGlobalSpeed + "\n";
            outString += "  " + ((Clock)subject).Timeline.Name + ": CurrentIteration   = " + subject.CurrentIteration + "\n";
        }
    }
}
