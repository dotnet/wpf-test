// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: BVTS testing the Selection and Subtree Processor API for AnnotationService.


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

namespace Avalon.Test.Annotations.BVTs
{
	public class ProcessorSuite_BVT : AProcessorSuite
	{
		protected override void RunTestCase()
		{
			// Enable service on root.
			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			_service = new AnnotationService((DependencyObject)root);
			Stream annotationStream = new FileStream(_storeFilename, FileMode.Create);
			_service.Enable(new XmlStreamStore(annotationStream));

			switch (CaseNumber)
			{
				case "processor1": queueTask(new DispatcherOperationCallback(processor1), root); break;
				case "processor3": queueTask(new DispatcherOperationCallback(processor3), root); break;

				default:
					failTest("TestSuite '" + this.ToString() + "' does not contain the given test case: '" + CaseNumber + "'.");
					break;
			}
		}

		// ----------------------------------------------------------------------------------
		//                                 TEST METHODS
		// ----------------------------------------------------------------------------------

		/// <summary>
		/// Register/SetSubTreeProcessor on root, check that processor is set for all children.
		/// </summary>
		public object processor1(object rootNode)
		{
			Canvas root = (Canvas)rootNode;
			//AnnotationService service = AnnotationService.GetService(root);
			CountingSubTreeProcessor processor = new CountingSubTreeProcessor(_service.LocatorManager);
			_service._locatorManager.RegisterSubTreeProcessor(processor, "p1");
			AnnotationService.SetSubTreeProcessorId(root, "p1");

			// Verify that the correct processor is used for different nodes by 
			// (1) checking the dp, (2) executing an operation that will use the 
			// processor and checking the its call count increases by one.
			int count = 1;
			for (int i = 0; i < root.Children.Count; i++)
			{
				assert("Verifying SubTreeProcessor was DP was inherited by child '" + i + "'.",
						processor == _service.LocatorManager.GetSubTreeProcessor(root.Children[i]));
				
				// GenerateLocators will call use the set SubTreeProcessor.
				_service.LocatorManager.GenerateLocators(root.Children[i]);
				assertEquals("Check that correct processor was used to GenerateLocators for child '" + i + "'.", count, processor.CallCount());

				count++;
			}

			passTest("Verified SubTreeProcessor for all children.");
			return null;
		}

		/// <summary>
		/// RegisterSelectionProcessor for some type A, verify that GenerateLocators for A uses 
		/// the correct SelectionProcessor.
		/// </summary>
		public object processor3(object rootNode)
		{
			Canvas root = (Canvas)rootNode;
			//AnnotationService service = AnnotationService.GetService(root);
			CountingSelectionProcessor processor = new CountingSelectionProcessor(_service.LocatorManager);
			_service._locatorManager.RegisterSelectionProcessor(processor, new TextBox().GetType());

			_service.LocatorManager.GenerateLocators(new TextBox());
			assert("Check that correct SelectionProcessor was used.", processor.CallCount > 0);

			passTest("Verified SelectionProcessor was registered properly.");
			return null;
		}

		private string _storeFilename = "annotations.xml";
		protected AnnotationService _service;
	}
}

