// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: BVTS testing the enablement API for AnnotationService.


using Annotations.Test.Framework;				// TestSuite.
using System;
using System.IO;
using System.Threading; using System.Windows.Threading;						// DispatcherOperationCallback.
using System.Windows;
using Proxies.System.Windows.Annotations;
using Proxies.MS.Internal.Annotations.Anchoring;
using Proxies.MS.Internal.Annotations.Component;
using System.Windows.Annotations.Storage;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Avalon.Test.Annotations.BVTs
{
	public class EnablementSuite_BVT : TestSuite
	{		
		protected override void RunTestCase()
		{
			switch (CaseNumber)
			{
				// case "enablement1": enablement1(); break; No longer possible to Enable without a store
				case "enablement2": enablement2(); break;

				default:
					failTest("TestSuite '" + this.ToString() + "' does not contain the given test case: '" + CaseNumber + "'.");
					break;
			}
		}

		// ----------------------------------------------------------------------------------
		//                                 TEST METHODS
		// ----------------------------------------------------------------------------------

		/// <summary>
		/// Enable once on non-root.  GetService returns service where IsReady == true.
		/// </summary>
		public void enablement2()
		{
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			DependencyObject non_root = LogicalTreeHelper.FindLogicalNode(root, "_dockpanel2");
			AnnotationService service = new AnnotationService(non_root);
			Stream annotationStream = new FileStream(FILENAME, FileMode.OpenOrCreate);
			service.Enable(new XmlStreamStore(annotationStream));
			queueTask(new DispatcherOperationCallback(VerifyServiceEnabledTask), service);
		}

		// ----------------------------------------------------------------------------------
		//									  TASKS
		// ----------------------------------------------------------------------------------

		/// <summary>
		/// PASS if AnnotationService.IsEnabled and that its LocatorManager is initialized.
		/// </summary>
		/// <param name="inObj">AnnotationService to verify.</param>
		public object VerifyServiceEnabledTask(object inObj)
		{
			AnnotationService service = (AnnotationService)inObj;
			assert("Verify that service is ready.", service.IsEnabled);
			assert("Verify that AnnoationService.LocatorManager is not null.", service.LocatorManager != null);

			passTest("Service enablement verified.");
			return null;
		}

		protected string FILENAME = "enableTest.xml";
	}
}

