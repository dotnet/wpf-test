// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/*
 * Description:  Verify calling TimeManager.Restart() from a child Timeline's Reversed event
*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline (parent)
//  3. Create a Clock and associate it with the Timeline (parent)

//Pass Condition:
//   This test passes the TimeManager restarts as expected

//Pass Verification:
//   The output of this test should match the expected output in b.51Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected in bug51Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug51 : ITimeBVT
    {   
        TimeManagerInternal     _tManager;
        ClockGroup              _clockParent;
        int                     _reverseCount = 0;
        
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

               // Create a Timeline
               ParallelTimeline parent = new ParallelTimeline();
               DEBUG.ASSERT(parent != null, "Cannot create parent Timeline", " Created parent Timeline ");
               parent.BeginTime        = TimeSpan.FromMilliseconds(0);
               parent.Duration = new Duration(TimeSpan.FromMilliseconds(10));
               parent.Name               = "Parent";

               // Create child Timeline
               ParallelTimeline child = new ParallelTimeline();
               DEBUG.ASSERT(child != null, "Cannot create Child Timeline" , " Created Child Timeline" );
               child.BeginTime    = TimeSpan.FromMilliseconds(0);
               child.AutoReverse  = true;
               child.Duration = new Duration(TimeSpan.FromMilliseconds(4));
               child.Name           = "Child";
              
               //Attach Child Timeline to Parent
               parent.Children.Add(child);
               DEBUG.LOGSTATUS(" Attached TimeNode to the Timeline ");

               // Create a Clock, connected to the Timeline.
               _clockParent = parent.CreateClock();       
               DEBUG.ASSERT(_clockParent != null, "Cannot create Clock" , " Created Clock " );
               //Attach Event Handlers to clockParent
               AttachCurrentStateInvalidatedTC( _clockParent );
            
               Clock clockChild = _clockParent.Children[0];         
               DEBUG.ASSERT(clockChild != null, "Cannot create child Clock" , " Created child Clock " );
               //Attach Event Handler to clockChild
               AttachCurrentGlobalSpeedInvalidatedTC( clockChild );

               //Run the Timer               
               TimeGenericWrappers.EXECUTE( this, _clockParent, _tManager, 0, 16, 1, ref outString);
               
               return outString;
          }
          
        public override void OnCurrentGlobalSpeedInvalidated(object subject, EventArgs args)
        {
            if (((Clock)subject).CurrentGlobalSpeed != null)
            {
                //Detect when the Timeline reverses.
                //CurrentGlobalSpeed changes to -1 when the clock reverses.
                if (((Clock)subject).CurrentGlobalSpeed < 0)
                {
                    if (Math.Sign((double)((Clock)subject).CurrentGlobalSpeed) != Math.Sign(saveSpeed))
                    {
                        outString += "  " + ((Clock)subject).Timeline.Name + ": CurrentGlobalSpeedInvalidated fired (Reversed)\n";

                        if (_reverseCount < 1)
                        {
                            _reverseCount++;
                            _tManager.Restart();
                        }
                    }
                }
                saveSpeed = (double)((Clock)subject).CurrentGlobalSpeed;
            }
        }
    }
}
