// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Main class responsible for routing all FixedContent API 
//  BVT test cases.

using System;
using Annotations.Test.Framework;

namespace Avalon.Test.Annotations.Pri1s
{

    public class VisualFixedContent
    {
        [STAThread]
        static void Main(string[] args)
		{
			new TestSuiteDriver(
                 new Type[] { 
                    typeof(Scenario1_5Suite),
                    typeof(Scenario1Suite),
                    typeof(Scenario2Suite),
                    typeof(Scenario3Suite),
                    typeof(Scenario4Suite),
                    typeof(Scenario5Suite),
                    typeof(FlowContentSuite),
                    typeof(NestedViewerSuite),
                    typeof(FlowPaginationSuite),
                    typeof(FixedContentSuite),
                    typeof(TableSuite),
                    typeof(AnchoredBlockSuite),
                    typeof(MiscSuite),
            }).Run(args);
		}
    }
}

