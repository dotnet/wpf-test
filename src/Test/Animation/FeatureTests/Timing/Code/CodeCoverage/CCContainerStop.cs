// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//*********************************************************************************************
// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the method Stop() for class ClockController

 */

//Instructions: 
//  1. Create a TimeNode with Begin = 2 and Duration = 10. 
//  2. Create a TimeContainer with Begin = 0 and Duration 20. 
//  3. Attach Handler to TimeNode.
//  3. Attach TimeNode to Container.
//  4. Create a TimeManager with Start=0 and Step = 1 and add the Container.
//  5. Stop the TimeContainer at tick = 5.
//  6. Check Stop event: after Stop event gets triggered, any event related to timeline should be ceased (without being "fired").

//Pass Condition:
//  Test Passes when event handler with timeline attached gets triggered.

//Pass Verification:
//  The output of this test should match the expected output in CCContainerStopExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected CCContainerStopExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    class CCContainerStop : ITimeBVT
    {
        private TimelineGroup _tNode,_tContainer;
        private Clock _tClock;

        /*
         *  Function:  Test
         *  Arguments:  Framework
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
            _tContainer.Duration     = new Duration(TimeSpan.FromMilliseconds(20));
            _tContainer.Name         = "Container";
            DEBUG.LOGSTATUS(" Set TimeContainer Properties ");

            // Create TimeNode
            _tNode = new ParallelTimeline();
            DEBUG.ASSERT(_tNode != null, "Cannot create TimeNode" , " Created TimeNode " );         
            // Set Properties to the TimeNode
            _tNode.BeginTime        = TimeSpan.FromMilliseconds(2);
            _tNode.Duration         = new Duration(TimeSpan.FromMilliseconds(10));
            _tNode.Name             = "TimeNode";
            DEBUG.LOGSTATUS(" Set TimeNode Properties ");         
            
            //Attach Handlers to TimeNode
            AttachCurrentStateInvalidated( _tNode );
            DEBUG.LOGSTATUS(" Attached EventHandlers to TimeNode ");

            //Attach TimeNodes to the TimeContainer
            _tContainer.Children.Add(_tNode);
            DEBUG.LOGSTATUS(" Attached TimeNodes to the TimeContainer ");

            // Create a Clock, connected to the Timeline.
            _tClock = _tContainer.CreateClock();        
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            //Intialize output String
            outString = "";
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, _tClock, tMan, 0, 20, 1, ref outString);
            
            return outString;   
        }

       public override void PostTick( int i )
        {
            if ( i == 5 )
                _tClock.Controller.Stop();
        }
    }
}
