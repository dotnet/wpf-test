// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Driver class that multiplexes TestId to the correct test classes


using Annotations.Test.Framework;
using System;

namespace Avalon.Test.Annotations.BVTs
{
	public class ObjectModel_BVT : SimpleDriver
	{
		[STAThread]
		static void Main(string[] args)
		{
            new ObjectModel_BVT().Run(args);
		}

		public override TestSuite  TestSuiteToRun
		{
            get
            {
                TestSuite suite = null;
                if (TestId.StartsWith("constructor"))
                    suite = new ConstructorBVTs();
                if (TestId.StartsWith("author"))
                    suite = new AuthorBVTs();
                if (TestId.StartsWith("anchor"))
                    suite = new AnchorBVTs();
                if (TestId.StartsWith("resource"))
                    suite = new ResourceBVTs();
                if (TestId.StartsWith("locator"))
                    suite = new LocatorBVTs();
                if (TestId.StartsWith("content"))
                    suite = new ContentBVTs();
                if (TestId.StartsWith("annotation"))
                    suite = new AnnotationSuite_BVT();
                return suite;
            }
		}
	}
}


