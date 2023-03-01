// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Main class responsible for routing all FixedContent API 
//  BVT test cases.

using System;
using System.Collections;
using Annotations.Test.Framework;


namespace Avalon.Test.Annotations.BVTs
{

	public class VisualFixedContent_BVT
	{
		[STAThread]
		static void Main(string[] args)
		{
			new TestSuiteDriver(
                 new Type[] { 
                     typeof(Scenario1_5Suite_BVT),
                     typeof(Scenario1Suite_BVT),
                     typeof(Scenario2Suite_BVT),
                     typeof(Scenario3Suite_BVT),
                     typeof(Scenario4Suite_BVT),
                     typeof(FlowContentSuite_BVT),
                     typeof(FlowPagintationSuite_BVT),
                     typeof(MultipageSelectionSuite_BVT),
                     typeof(FixedContentSuite_BVT),
                     typeof(TableSuite_BVT),
                     typeof(AnchoredBlockSuite_BVT),
            }).Run(args);
		}
	}
}

