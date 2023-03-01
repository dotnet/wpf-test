// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;


namespace Avalon.Test.Annotations.Pri1s
{
    public class Driver
    {
        [STAThread]
        static void Main(string[] args)
        {
            new TestSuiteDriver(new Type[] {			
			    typeof(EventingSuite),
			    typeof(AnnotationSuite),
			    typeof(LocatorGroupSuite),
                typeof(LocatorSuite),
                typeof(NodeTestSuite),
                typeof(AttributeTestSuite),
            }).Run(args);
        }
    }
}	

