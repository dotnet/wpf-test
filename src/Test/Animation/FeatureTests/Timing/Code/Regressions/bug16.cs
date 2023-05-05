// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify CurrentStateInvalidated for Parent and Child when Begin() is invoked

*/

//Pass Verification:
//  The output of this test should match the expected output in bug16Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug16Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug16 :ITimeBVT
    {
        TimeManagerInternal   _tManager;
        ClockGroup            _tClock;

        /*
        *  Function:   Test
        *  Arguments:  Framework
        */
        public override string Test()
        {
            //Intialize output String
            outString = "";

            _tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a parent
            ParallelTimeline parent = new ParallelTimeline();
            DEBUG.ASSERT(parent != null, "Cannot create Parent", " Created Parent ");
            parent.BeginTime            = TimeSpan.FromMilliseconds(10);
            parent.Duration             = new Duration(TimeSpan.FromMilliseconds(20));
            parent.FillBehavior         = FillBehavior.Stop;
            parent.Name                 = "Parent";
               
            // Create a child
            ParallelTimeline child = new ParallelTimeline();
            DEBUG.ASSERT(child != null, "Cannot create Child", " Created Child ");
            child.BeginTime            = TimeSpan.FromMilliseconds(5);
            child.Duration             = new Duration(TimeSpan.FromMilliseconds(5));
            child.Name                 = "Child";
            
            parent.Children.Add( child );
            
            // Create a Clock, connected to the Timeline.
            _tClock = parent.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
            
            //Attach events
            AttachCurrentStateInvalidatedTC( _tClock );
            AttachCurrentStateInvalidatedTC( _tClock.Children[0] );
            
            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 32, 1, ref outString);

            return outString;
        }

        public override void PostTick( int i )
        {
            if ( i == 5 )
            {
                _tClock.Controller.Begin();
            }
        } 
          
        public override void OnProgress( Clock subject )
        {
            outString += "CurrentProgress [" + ((Clock)subject).Timeline.Name + "] = " + subject.CurrentProgress + "\n";
            outString += "CurrentState    [" + ((Clock)subject).Timeline.Name + "] = " + subject.CurrentState + "\n";
        }
    }
}
