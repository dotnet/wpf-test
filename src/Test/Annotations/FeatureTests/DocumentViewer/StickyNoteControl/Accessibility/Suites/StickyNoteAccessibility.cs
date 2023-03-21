// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Selects what TestSuite to run based on command line args.

using System;
using System.Windows;
using Annotations.Test.Framework;

namespace Avalon.Test.Annotations.Pri1s
{
    public class DriverTemplate
    {
        [STAThread]
		static void Main(string[] args)
        {
            new TestSuiteDriver(
                 new Type[] { 
                    typeof(SingleNoteSuite),
                     typeof(HighlightSuite),
                     typeof(MultiNoteSuite),
                     typeof(FlowSuite),
                     typeof(FixedSuite),
            }).Run(args);
		}			
    }
}	

