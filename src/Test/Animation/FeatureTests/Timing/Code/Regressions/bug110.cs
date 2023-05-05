// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/*
 * Description:  Verify pausing a Timeline at the same time one of its descendants begins

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline (tContainer)
//  3. Create a Clock and associate it with the Timeline (tContainer)

//Pass Condition:
//   This test passes if the Timelines pause appropriately.

//Pass Verification:
//   The output of this test should match the expected output in b.110Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected in bug110Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug110 : ITimeBVT
    {
        ParallelTimeline    _tContainer;
        ParallelTimeline    _tNode1;
        ParallelTimeline    _tNode2;
        Clock       _tClock;
        
        TimeManagerInternal _tManager;
        /*
         *  Function :  Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";
            
            //Verify events by listing them at the end.
            eventsVerify = true;

            // Create a TimeManager
            _tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a container Timeline
            _tContainer = new ParallelTimeline();
            DEBUG.ASSERT(_tContainer != null, "Cannot create Container", " Created Container ");
            _tContainer.BeginTime            = TimeSpan.FromMilliseconds(0);
            _tContainer.Duration             = new Duration(TimeSpan.FromMilliseconds(100));
            _tContainer.Name                   = "Container";
            //Attach Event Handler to tNode
            AttachCurrentGlobalSpeedInvalidated( _tContainer );

            // Create child Timeline 1
            _tNode1 = new ParallelTimeline();
            DEBUG.ASSERT(_tNode1 != null, "Cannot create TimeNode 1" , " Created TimeNode 1" );
            _tNode1.BeginTime    = TimeSpan.FromMilliseconds(10);
            _tNode1.Duration     = new Duration(TimeSpan.FromMilliseconds(50));
            _tNode1.Name           = "TimeNode1";
            //Attach Event Handler to tNode1
            AttachCurrentGlobalSpeedInvalidated( _tNode1 );

            // Create child Timeline 2
            _tNode2 = new ParallelTimeline();
            DEBUG.ASSERT(_tNode2 != null, "Cannot create TimeNode 2" , " Created TimeNode 2" );
            _tNode2.BeginTime    = TimeSpan.FromMilliseconds(20);   //Begins at 30ms from tContainer.
            _tNode2.Duration     = new Duration(TimeSpan.FromMilliseconds(30));
            _tNode2.Name           = "TimeNode2";
            //Attach Event Handler to tNode2
            AttachCurrentGlobalSpeedInvalidated( _tNode2 );

            //Attach TimeNode 2 to TimeNode 1
            _tNode1.Children.Add(_tNode2);
            DEBUG.LOGSTATUS(" Attached TimeNode to the Container ");

            //Attach TimeNode 1 to the Container
            _tContainer.Children.Add(_tNode1);
            DEBUG.LOGSTATUS(" Attached TimeNode to the Container ");

            // Create a Clock, connected to the Timeline.
            _tClock = _tContainer.CreateClock();        
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 50, 10, ref outString);

            WriteAllEvents();

            return outString;
        }

        public override void PreTick( int i )
        {
            if ( i == 30 )
            {
                _tClock.Controller.Pause();
            }
        }
    }
}
