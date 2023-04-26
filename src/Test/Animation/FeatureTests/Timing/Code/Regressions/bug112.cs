// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/*
 * Description:  Verify Begin() fires the Begun event after a Pause()
 *               NOTE: replacing BeginIn(10) at i=60 with Begin() at i=70

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline (tNode)
//  3. Create a Clock and associate it with the Timeline (tNode)

//Pass Condition:
//   This test passes if all events fire appropriately

//Pass Verification:
//   The output of this test should match the expected output in bug112Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected in bug112Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug112 : ITimeBVT
    {
         Clock _tClock;
        /*
         *  Function :  Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";     

            // Create a TimeManager
            TimeManagerInternal tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a container Timeline
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create Timeline", " Created Timeline ");
            tNode.BeginTime             = TimeSpan.FromMilliseconds(0);
            tNode.Duration              = new Duration(TimeSpan.FromMilliseconds(50));
            tNode.Name                    = "Timeline";
            //Attach Event Handler to tNode
            AttachCurrentStateInvalidated( tNode );

            // Create a Clock, connected to the Timeline.
            _tClock = tNode.CreateClock();     
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
            
            //HACK-HACK: CurrentTimeInvalidated must be associated to the clock to force a Tick.
            AttachCurrentTimeInvalidatedTC(_tClock);

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, tManager, 0, 150, 10, ref outString);

            return outString;
          }

          public override void PreTick( int i )
          {
               if ( i == 20 )
               {
                _tClock.Controller.Pause();
               }
               if ( i == 70 )
               {
                _tClock.Controller.Resume();
                _tClock.Controller.Begin();
               }
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}
