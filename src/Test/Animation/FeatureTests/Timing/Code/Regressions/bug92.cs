// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/*
 * Description:  Verify Duration when adding a revised Clock after pausing the original clock

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline (tContainer)
//  3. Create a Clock and associate it with the Timeline (tContainer)

//Pass Condition:
//   This test passes if CurrentProgress and event firing are correct.

//Pass Verification:
//   The output of this test should match the expected output in bug92Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected in bug92Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug92 : ITimeBVT
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

            // Create a TimeManager
            _tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a container Timeline
            _tContainer = new ParallelTimeline();
            DEBUG.ASSERT(_tContainer != null, "Cannot create Container", " Created Container ");
            _tContainer.BeginTime        = TimeSpan.FromMilliseconds(0);
            _tContainer.Duration         = new Duration(TimeSpan.FromMilliseconds(20));
            _tContainer.Name               = "Container";
            //Attach Event Handler to tNode
            AttachCurrentStateInvalidated( _tContainer );

            // Create child Timeline 1
            _tNode1 = new ParallelTimeline();
            DEBUG.ASSERT(_tNode1 != null, "Cannot create TimeNode 1" , " Created TimeNode 1" );
            _tNode1.BeginTime        = TimeSpan.FromMilliseconds(10);
            _tNode1.Duration         = new Duration(TimeSpan.FromMilliseconds(5));
            _tNode1.Name               = "TimeNode1";
            //Attach Event Handler to tNode1
            AttachCurrentStateInvalidated( _tNode1 );

            // Create child Timeline 2
            _tNode2 = new ParallelTimeline();
            DEBUG.ASSERT(_tNode2 != null, "Cannot create TimeNode 2" , " Created TimeNode 2" );
            _tNode2.BeginTime        = TimeSpan.FromMilliseconds(12);
            _tNode2.Duration         = new Duration(TimeSpan.FromMilliseconds(5));
            _tNode2.Name               = "TimeNode2";
            //Attach Event Handler to tNode2
            AttachCurrentStateInvalidated( _tNode2 );

            //Attach only TimeNode 1 to the Container
            _tContainer.Children.Add(_tNode1);
            DEBUG.LOGSTATUS(" Attached TimeNode to the Container ");

            // Create a Clock, connected to the Timeline.
            _tClock = _tContainer.CreateClock();        
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

               
           //Run the Timer
           _tManager.Start();

           for(int i = 0 ; i <= 32; i += 1 )
           {
                outString += "------------------------------" + i.ToString() + "\n";

                CurrentTime = TimeSpan.FromMilliseconds(i);
                _tManager.Tick();
                
                outString += "CurrentProgress:  "  + _tClock.CurrentProgress  + "\n";
                
                if ( i == 8 )
                {
                    _tClock.Controller.Pause();
                    _tContainer.Children.Add(_tNode2);
                    _tClock = _tContainer.CreateClock();
                    //tClock.Controller.Resume();
                }
            }

            return outString;
        }
    }
}
