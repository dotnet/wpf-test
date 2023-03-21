// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Driver class for all Service API Pri1 test cases.


using Annotations.Test.Framework;		// TestSuite.
using System;

namespace Avalon.Test.Annotations.Pri1s
{
	public class ServiceAPI_Pri1 : SimpleDriver
	{
        [STAThread]
        static void Main(string[] args)
        {
            new ServiceAPI_Pri1().Run(args);
        }

		public override TestSuite  TestSuiteToRun
		{
            get
            {
                TestSuite suite = null;

                // Determine what TestSuite to run.
                if (TestId.StartsWith("enablement")) suite = new EnablementSuite_Pri1();
                // if (TestId.StartsWith("storeproperty")) suite = new StorePropertySuite_Pri1();
                if (TestId.StartsWith("processor")) suite = new ProcessorSuite_Pri1();
                if (TestId.StartsWith("loading")) suite = new LoadingSuite_Pri1();
                if (TestId.StartsWith("storecontentchanged")) suite = new StoreContentChangedSuite_Pri1();
                if (TestId.StartsWith("anchorchanged")) suite = new AnchorChangedSuite_Pri1();
                // if (TestId.StartsWith("storestatechanged")) suite = new StoreStateChangedSuite_Pri1();
                if (TestId.StartsWith("aachanged")) suite = new AttachedAnnotationChangedSuite_Pri1();
                if (TestId.StartsWith("annotationmap")) suite = new AnnotationMapSuite_Pri1();

                return suite;
            }
		}
	}
}

