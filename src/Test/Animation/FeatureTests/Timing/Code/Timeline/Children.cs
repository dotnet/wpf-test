// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify methods on a Timeline's Children collection
 */

//Instructions:
//  1. Create a Timeline tree and attach properties
//  2. Create a Clock and associate it with the root Timeline
//  3. Create a TimeManager with Start=0 and Step = 1 and add the Container


//Warnings:
//  Any changes made in the output should be reflected in ChildrenExpect.txt file

//Pass Condtion:
//  Test Passes when the output matches the file ChildrenExpect.txt

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class Children :ITimeBVT
    {
        TimeManagerInternal         _tMan;
        ParallelTimeline    _tContainer,_tNode1,_tNode2,_tNode3,_tNode4;
        Clock       _tClock;
        /*
         *  Function:  Test
         *  Arguments: Framework
         */
        public override string Test()
        {
            // Create a TimeManager
            _tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(_tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeContainer
            _tContainer = new ParallelTimeline();
            DEBUG.ASSERT(_tContainer != null, "Cannot create TimeContainer", " Created TimeContainer ");

            // Set Properties to the TimeContainer
            _tContainer.BeginTime   = TimeSpan.FromMilliseconds(0);
            _tContainer.Name        = "Parent";
            DEBUG.LOGSTATUS(" Set TimeContainer Properties ");
            
            // Create TimeNode 1
            _tNode1 = new ParallelTimeline();
            DEBUG.ASSERT(_tNode1 != null, "Cannot create TimeNode1" , " Created TimeNode1 " );           
            // Set Properties to the TimeNode
            _tNode1.BeginTime        = TimeSpan.FromMilliseconds(0);
            _tNode1.Duration         = new System.Windows.Duration(TimeSpan.FromMilliseconds(4));
            _tNode1.Name             = "Child1";
            DEBUG.LOGSTATUS(" Set Timeline 1 Properties ");

            // Create TimeNode 2
            _tNode2 = new ParallelTimeline();
            DEBUG.ASSERT(_tNode2 != null, "Cannot create TimeNode2" , " Created TimeNode2 " );           
            // Set Properties to the TimeNode
            _tNode2.BeginTime        = TimeSpan.FromMilliseconds(0);
            _tNode2.Duration         = new System.Windows.Duration(TimeSpan.FromMilliseconds(10));
            _tNode2.Name             = "Child2";
            DEBUG.LOGSTATUS(" Set Timeline 2 Properties ");
            
            // Create TimeNode 3
            _tNode3 = new ParallelTimeline();
            DEBUG.ASSERT(_tNode3 != null, "Cannot create TimeNode3" , " Created TimeNode3 " );           
            // Set Properties to the TimeNode
            _tNode3.BeginTime        = TimeSpan.FromMilliseconds(8);
            _tNode3.Duration         = new System.Windows.Duration(TimeSpan.FromMilliseconds(2));
            _tNode3.Name             = "Child3";
            DEBUG.LOGSTATUS(" Set Timeline 3 Properties ");
            
            // Create TimeNode 4
            _tNode4 = new ParallelTimeline();
            DEBUG.ASSERT(_tNode4 != null, "Cannot create TimeNode4" , " Created TimeNode4 " );           
            // Set Properties to the TimeNode
            _tNode4.BeginTime        = TimeSpan.FromMilliseconds(0);
            _tNode4.Duration         = new System.Windows.Duration(TimeSpan.FromMilliseconds(4));
            _tNode4.Name             = "Child4";
            DEBUG.LOGSTATUS(" Set Timeline 4 Properties ");

            //Attach TimeNode to the TimeContainer
            _tContainer.Children.Add(_tNode1);
            _tContainer.Children.Add(_tNode3);    
            DEBUG.LOGSTATUS(" Attached TimeNode3 to the TimeContainer ");

            //Add, then Remove tNode2
 
            _tNode2.Freeze();
            _tContainer.Children.Add(_tNode2);
            _tContainer.Children.Remove(_tNode2);

            // Create a Clock, connected to the container.
            _tClock = _tContainer.CreateClock();        
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            //Intialize output String
            outString = "";
            
            //Run the Timer         
            _tMan.Start();           
            for(int i = 0; i <= 22; i += 1 )
            {
                outString += "----------------------------------Processing time " + i + " ms\n";
                outString += "Children Count = " + _tContainer.Children.Count + "\n";
                        
                CurrentTime = TimeSpan.FromMilliseconds(i);
                _tMan.Tick();
                   
                TimeGenericWrappers.FIREONPROGRESS( this, _tClock );    
                
                if ( i == 4 )
                {       
                    _tMan.Stop();
                    _tContainer.Children.Add(_tNode4);
                    _tClock = _tContainer.CreateClock();
                    _tMan.Start();
                }
                        
                CurrentTime = TimeSpan.FromMilliseconds(i);
                _tMan.Tick();
            }           
            
            return outString;   
        }
    }
}
