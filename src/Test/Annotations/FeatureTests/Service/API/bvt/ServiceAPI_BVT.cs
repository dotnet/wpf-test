// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Driver class for all Service API bvts.


using Annotations.Test.Framework; // TestSuite.

using System;

namespace Avalon.Test.Annotations.BVTs
{
	public class ServiceAPI_BVT : SimpleDriver
	{
        [STAThread]
        static void Main(string[] args)
        {
            new ServiceAPI_BVT().Run(args);
        }

		public override TestSuite  TestSuiteToRun
		{
            get
            {
                TestSuite suite = null;

                // Determine what TestSuite to run.
                if (TestId.StartsWith("enablement")) suite = new EnablementSuite_BVT();
                // GONE AS NO SETTER FOR STORE if (TestId.StartsWith("storeproperty")) suite = new StorePropertySuite_BVT();
                if (TestId.StartsWith("processor")) suite = new ProcessorSuite_BVT();
                if (TestId.StartsWith("loading")) suite = new LoadingSuite_BVT();
                if (TestId.StartsWith("storecontentchanged")) suite = new StoreContentChangedSuite_BVT();
                if (TestId.StartsWith("anchorchanged")) suite = new AnchorChangedSuite_BVT();
                if (TestId.StartsWith("aachanged")) suite = new AttachedAnnotationChangedSuite_BVT();

                return suite;
            }
		}
    }
}

