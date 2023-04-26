// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the Duration Constructor (Time) and operator functionality
 */

//Instructions:
//     1. Create Duration objects
//     2. Test different operators ==,!= and properties

//Warnings:
//     Any changes made in the output should be reflected DurationPropertiesExpect.txt file

//Pass Condition:
//     The output from the program is compared with DurationPropertiesExpect.txt file.

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
     class DurationProperties :ITimeBVT
     {
          /*
           *  Function : Test
           *  Arguments: Framework
           */
          public override string Test()
          {
               // Program Variables

               Duration dur1 = new Duration (  TimeSpan.FromMilliseconds(100) );               
               Duration dur2 = new Duration (  TimeSpan.FromMilliseconds(100) );               
               Duration dur3 = new Duration (  TimeSpan.FromMilliseconds(512) );               
               Duration dur4 = new Duration (  TimeSpan.FromMilliseconds(0) );               

               DEBUG.LOGSTATUS("Checking Duration Constructor");
               Check(dur1 == TimeSpan.FromMilliseconds(100) , " Duration Constructor ");

               DEBUG.LOGSTATUS("Checking the ==, != operators");
               Check( dur1 == TimeSpan.FromMilliseconds(100)      , " == operator " );          
               Check( TimeSpan.FromMilliseconds(100) == dur1      , " == operator " );          
               Check( dur1 == dur2 , " == operator " );          
               Check( dur1 != dur3 , " != operator " );
               Check( dur3 > dur2 ,  " > operator " );
               Check( dur1 >= dur2 , " >= operator " );
               Check( dur1 < dur3 ,  " < operator " );
               Check( dur1 <= dur2 , " ,= operator " );
               Check( (dur1 + dur3 == TimeSpan.FromMilliseconds(612)) , " + operator " );
               Check( (dur1 - dur2 == TimeSpan.FromMilliseconds(0)) , " - operator " );
               Check( (+dur3 == TimeSpan.FromMilliseconds(512)) , " unary + operator " );
               Check( dur4 != TimeSpan.FromMilliseconds(1)        , " != operator " );
               
               DEBUG.LOGSTATUS("Checking Automatic Property");
               Check( Duration.Automatic.ToString() == "Automatic" , " Automatic Property " );
               DEBUG.LOGSTATUS("Checking Forever Property");
               Check( Duration.Forever.ToString() == "Forever" , " Forever Property " );
               DEBUG.LOGSTATUS("Checking HasTimeSpan Property");
               Check( dur1.HasTimeSpan == true , " HasTimeSpan Property " );
               
               DEBUG.LOGSTATUS("Checking TimeSpan Property");
               Check( dur1.TimeSpan == TimeSpan.FromMilliseconds(100) , " TimeSpan Property " );
            
               return outString;
          }

          private void Check( bool condition, string message )
          {
               outString += message + " : " + ( condition ? "SUCCESS" : "FAILED" ) + "\n" ;
          }
     }
}
