// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the Duration methods
 */

//Instructions:
//     1. Create Duration objects
//     2. Test different methods

//Warnings:
//     Any changes made in the output should be reflected DurationMethodsExpect.txt file

//Pass Condition:
//     The output from the program is compared with DurationMethodsExpect.txt file.

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;


namespace Microsoft.Test.Animation
{
     class DurationMethods :ITimeBVT
     {
          /*
           *  Function : Test
           *  Arguments: Framework
           */
          public override string Test()
          {
               Duration dur1 = new Duration (  TimeSpan.FromMilliseconds(100) );
               Duration dur2 = new Duration (  TimeSpan.FromMilliseconds(100) );
               Duration dur3 = new Duration (  TimeSpan.FromMilliseconds(512) );
               Duration dur4 = new Duration (  TimeSpan.FromMilliseconds(0) );
               
               DEBUG.LOGSTATUS("Checking Add Method");
               Check( dur1.Add(dur2) == TimeSpan.FromMilliseconds(200) , " Add Method " );

               DEBUG.LOGSTATUS("Checking Equals(duration) Method");
               Duration dur6 = new Duration (  TimeSpan.FromMilliseconds(512) );               
               Check(  dur3.Equals(dur6) == true    , " Equals Method");
               
               DEBUG.LOGSTATUS("Checking Equals(duration,duration) Method");
               Check( (Equals(dur1,dur4) == false)  , " Equals(duration,duration) Method");
               
               DEBUG.LOGSTATUS("Checking Equals(duration,duration) Method");
               Check( (Duration.Equals(dur1,dur4) == false)  , " Duration.Equals(duration,duration) Method");

               DEBUG.LOGSTATUS("Checking Equals(object) Method");
               Duration nts7 = new Duration (  TimeSpan.FromMilliseconds(512) );               
               object dur7 = (object)nts7;
               Check( (dur6.Equals(dur7) == true)   , " Equals(object) Method");
               
               DEBUG.LOGSTATUS("Checking GetHashCode Method");
               int hash = dur2.GetHashCode();
               bool hashResult = (hash.GetType().ToString() == "System.Int32");
               Check( hashResult == true, " GetHashCode Method " );
               
               DEBUG.LOGSTATUS("Checking Subtract Method");
               Check( dur1.Subtract(dur2) == TimeSpan.FromMilliseconds(0) , " Subtract Method " );
               
               DEBUG.LOGSTATUS("Checking ToString Method");
               Check( dur2.ToString() == "00:00:00.1000000", " ToString Method " );
               
               DEBUG.LOGSTATUS("Checking Compare Method");
               Check( Duration.Compare(dur1,dur1) == 0, " Compare Method 1" );
               Check( Duration.Compare(dur1,dur2) == 0, " Compare Method 2" );
               Check( Duration.Compare(dur2,dur1) == 0, " Compare Method 3" );
               Check( Duration.Compare(dur2,dur3) == -1, " Compare Method 4" );
               Check( Duration.Compare(dur3,dur2) == 1, " Compare Method 5" );

               DEBUG.LOGSTATUS("Checking Plus Method");
               Check( Duration.Plus(dur2) == dur2, " Plus Method" );
          
               return outString;
          }

          private void Check( bool condition, string message )
          {
               outString += message + " : " + ( condition ? "SUCCESS" : "FAILED" ) + "\n" ;
          }
     }
}
