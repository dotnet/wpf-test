// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify the TimeManager's CurrentTime after it has stopped

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline
//  3. Create a Clock, associated with the Timeline
//  4. Start the TimeManager; at the 10th tick, stop the TimeManager

//Pass Condition:
//  The CurrentTime should return the current time.

//Pass Verification:
//  The output of this test should match the expected output in bug64Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug64Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug64 :ITimeBVT
     {
          TimeManagerInternal   _tManager;
          
          /*
           *  Function:   Test
           *  Arguments:  Framework
           */
          public override string Test()
          {
               _tManager = EstablishTimeManager(this);
               DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

               //Intialize output String
               outString = "";
               
               _tManager.Start();

               for (int i = 0; i <= 15; i += 1 )
               {
                    CurrentTime = TimeSpan.FromMilliseconds(i);       
                    _tManager.Tick();
                    
                if ( i == 10 )
                {
                    _tManager.Stop();
                }
                outString += "-------------------CurrentTime: " + _tManager.CurrentTime.TotalMilliseconds + "\n";     
              }               
               
               return outString;
          }
     }
}
