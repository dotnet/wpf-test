// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/*
 * Description:  Verify calling Current before the first call to MoveNext


*/

//Instructions:
//     1. Create a Timeline and add children to it
//     2. Obtain an Enumerator

//Pass Condition:
//   This test passes if all events fire appropriately

//Pass Verification:
//   The output of this test should match the expected output in bug65Expect.txt.

//Warnings:
//     Any changes made in the output should be reflected in bug65Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Collections;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug65 : ITimeBVT
     {
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

               // Create TimeContainer
               ParallelTimeline tRoot = new ParallelTimeline( null , Duration.Automatic);
               DEBUG.ASSERT(tRoot != null, "Cannot create TimeContainer tRoot", " Created TimeContainer tRoot");
               tRoot.Name = "Root";

               // Create TimeNode 1
               ParallelTimeline tNode1 = new ParallelTimeline( TimeSpan.FromMilliseconds(100) , new Duration(TimeSpan.FromMilliseconds(200)));
               DEBUG.ASSERT(tNode1 != null, "Cannot create TimeNode1", " Created TimeNode1 ");
               tNode1.Name ="TimeNode1";

               // Create TimeNode 2
               ParallelTimeline tNode2 = new ParallelTimeline( TimeSpan.FromMilliseconds(0) , new Duration(TimeSpan.FromMilliseconds(250)));
               DEBUG.ASSERT(tNode2 != null, "Cannot create TimeNode2", " Created TimeNode2 ");
               tNode2.Name ="TimeNode2";

               // Create TimeNode 3
               ParallelTimeline tNode3 = new ParallelTimeline( TimeSpan.FromMilliseconds(300) , new Duration(TimeSpan.FromMilliseconds(0)));
               DEBUG.ASSERT(tNode3 != null, "Cannot create TimeNode3", " Created TimeNode3 ");
               tNode3.Name ="TimeNode3";

               // Create TimeNode 4
               ParallelTimeline tNode4 = new ParallelTimeline( TimeSpan.FromMilliseconds(111) , new Duration(TimeSpan.FromMilliseconds(222)));
               DEBUG.ASSERT(tNode4 != null, "Cannot create TimeNode4" , " Created TimeNode4 " );
               tNode4.Name = "TimeNode4";

               // Create TimeNode 5
               ParallelTimeline tNode5 = new ParallelTimeline( null , Duration.Automatic);
               DEBUG.ASSERT(tNode5 != null, "Cannot create TimeNode5" , " Created TimeNode5 " );
               tNode5.Name = "TimeNode5";
          
               //Attach TimeNodes to the TimeContainer
               tRoot.Children.Add(tNode1);
               tRoot.Children.Add(tNode2);
               tRoot.Children.Add(tNode3);
               tRoot.Children.Add(tNode4);
               tNode4.Children.Add(tNode5);
               DEBUG.LOGSTATUS(" Attached TimeNodes to the TimeContainer ");

               // Create a Clock, connected to the container.
               Clock tClock = tRoot.CreateClock();          
               DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

               //Obtain an Enumerator object.
               IEnumerator E = ((IEnumerable)tRoot.Children).GetEnumerator();
               DEBUG.LOGSTATUS(" Called GetEnumerator ");

               try
               {
                    Timeline timeLine = (Timeline)E.Current;
               }
               catch (Exception e)
               {
                    //If no exception occurs, the output will be blank and the test will fail.
                    TimeGenericWrappers.CHECKEXCEPTION( typeof(System.InvalidOperationException), e.GetType(), ref outString );
               }

               return outString;
          }
     }
}
