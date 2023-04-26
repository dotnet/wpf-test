// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify applying Seek, Pause, and Resume

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline tree, with attached event handlers
//  3. Create a Clock, associated with the parent Timeline
//  4. Start the TimeManager; invoke Pause(), Resume(), and Seek() at different intervals

//Pass Condition:
//  The appropriate events should fire.

//Pass Verification:
//  The output of this test should match the expected output in bug83Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug83Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug83 :ITimeBVT
     {
          TimeManagerInternal   _tManager;
          Clock                 _tClock;
          
          /*
           *  Function:   Test
           *  Arguments:  Framework
           */
          public override string Test()
          {
               _tManager = EstablishTimeManager(this);
               DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

               // Create a container Timeline
               ParallelTimeline parent = new ParallelTimeline();
               DEBUG.ASSERT(parent != null, "Cannot create Parent", " Created Parent ");
               parent.BeginTime         = TimeSpan.FromMilliseconds(0);
               parent.Duration          = new Duration(TimeSpan.FromMilliseconds(10));
               parent.Name              = "Parent";
               parent.FillBehavior      = FillBehavior.Stop;

               // Create a child Timeline
               ParallelTimeline child = new ParallelTimeline();
               DEBUG.ASSERT(child != null, "Cannot create Child" , " Created Child" );
               child.BeginTime          = TimeSpan.FromMilliseconds(5);
               child.Duration           = new Duration(TimeSpan.FromMilliseconds(2));
               child.AutoReverse        = true;
               child.Name               = "Child";
               child.FillBehavior       = FillBehavior.Stop;
               
               //Attach Event Handler to child
               AttachCurrentGlobalSpeedInvalidated( child );
              
               //Attach TimeNode to the Container
               parent.Children.Add(child);
               DEBUG.LOGSTATUS(" Attached TimeNode to the Container ");

               // Create a Clock, connected to the container Timeline.
               _tClock = parent.CreateClock();          
               DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

               //Intialize output String
               outString = "";
               
               //Run the Timer               
               TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 10, 1, ref outString);
               
               return outString;
          }

          public override void PreTick( int i )
          {
               if ( i == 3 )
               {
                    _tClock.Controller.Pause();
               }
               if ( i == 5 )
               {
                    _tClock.Controller.Resume();
               }
          }          

          public override void PostTick( int i )
          {
               if ( i == 0 )
               {
                    _tClock.Controller.Seek(TimeSpan.FromMilliseconds(6), TimeSeekOrigin.BeginTime);
               }
          }          
 
         public override void OnProgress(Clock subject)
         {
              outString += "----CurrentProgress    = " + ((Clock)subject).Timeline.Name + "  " + ((Clock)subject).CurrentProgress + "\n";
              outString += "----CurrentGlobalSpeed = " + ((Clock)subject).Timeline.Name + "  " + ((Clock)subject).CurrentGlobalSpeed + "\n";
         }
     }
}
