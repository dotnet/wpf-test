// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: BVTS testing the Load/Unload API for AnnotationService.


using Annotations.Test.Framework;				// TestSuite.
using System;
using System.Collections;
using System.IO;							// File.
using System.Threading; using System.Windows.Threading;						// DispatcherOperationCallback.
using System.Windows;
using Proxies.System.Windows.Annotations;
using Proxies.MS.Internal.Annotations.Anchoring;
using Proxies.MS.Internal.Annotations.Component;
using System.Windows.Annotations.Storage;
using System.Windows.Controls;
using System.Windows.Documents;				// TextRange.
using System.Xml;							// XmlQualifiedName.
using Annotation = System.Windows.Annotations.Annotation;
using ContentLocatorBase = System.Windows.Annotations.ContentLocatorBase;
using ContentLocatorPart = System.Windows.Annotations.ContentLocatorPart;
using ContentLocator = System.Windows.Annotations.ContentLocator;
using ContentLocatorGroup = System.Windows.Annotations.ContentLocatorGroup;
using AnnotationResource = System.Windows.Annotations.AnnotationResource;
using AnnotationResourceChangedEventArgs = System.Windows.Annotations.AnnotationResourceChangedEventArgs;
using AnnotationAuthorChangedEventArgs = System.Windows.Annotations.AnnotationAuthorChangedEventArgs;
using AnnotationResourceChangedEventHandler = System.Windows.Annotations.AnnotationResourceChangedEventHandler;
using AnnotationAuthorChangedEventHandler = System.Windows.Annotations.AnnotationAuthorChangedEventHandler;
using AnnotationAction = System.Windows.Annotations.AnnotationAction;

namespace Avalon.Test.Annotations.BVTs
{
	public class LoadingSuite_BVT : ALoadingSuite
	{
		protected override void RunTestCase()
		{
			#region setup

			Canvas root = AnnotationTestHelper.BuildMultiBranchTree();
			ServiceTestContext context = ServiceTestContext.SetupWithServiceAndStore(this, root);
			
			// Set TextSelectionProcessor so that we can annotate TextRanges.
			context.service._locatorManager.RegisterSelectionProcessor(new TextSelectionProcessor(), typeof(TextRange));
			
			// - DISABLED 10/29/04 - 
			//
			// Set our own SubTreeProcessor for this test because the behavior of LoadAnnotations
			// depends upon it.
			//context.service.RegisterSubTreeProcessor(new TestSubTreeProcessor(context.service.LocatorManager), "test");
			//AnnotationService.SetSubTreeProcessorId(context.enablementNode, "test");
			//
			// - DISABLED - 
			DataIdProcessor.SetFetchAnnotationsAsBatch(root, true);

			#endregion setup

			switch (CaseNumber)
			{
				case "loading1": queueTask(new DispatcherOperationCallback(loading1), context); break;
				case "loading3": queueTask(new DispatcherOperationCallback(loading3), context); break;

				default:
					failTest("TestSuite '" + this.ToString() + "' does not contain the given test case: '" + CaseNumber + "'.");
					break;
			}
		}

		/// <summary>
		/// Call UnloadAnnotations on root with annotations throughout sub-tree. Verify that 
		/// there are no AttachedAnnotations.
		/// </summary>
		public object loading1(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Hashtable annotationMap = AnnotateMultiBranchTree(context.enablementNode);
			assertEquals("Verify initial number of attached annotations.", 8, context.service.GetAttachedAnnotations().Count);
			
			context.service.UnloadAnnotations(context.enablementNode);
			assertEquals("Verify number of attached annotations after 'Unload'.", 0, context.service.GetAttachedAnnotations().Count);

			passTest("Verified 'UnloadAnnotations'.");
			return null;
		}

		/// <summary>
		/// Call Unload on root. Call Load on root.  Verify that the ending annotations are 
		/// the same as the starting.
		/// </summary>
		public object loading3(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;
			Hashtable annotationMap = AnnotateMultiBranchTree(context.enablementNode);
			assertEquals("Verify initial number of attached annotations.", 8, context.service.GetAttachedAnnotations().Count);
			
			context.service.UnloadAnnotations(context.enablementNode);
			assertEquals("Verify number of attached annotations after 'Unload'.", 0, context.service.GetAttachedAnnotations().Count);
			
			context.service.LoadAnnotations(context.enablementNode);
			VerifyAttachedAnnotations(context.service.GetAttachedAnnotations(), (Annotation[])annotationMap["mainDockPanel"]);

			passTest("Verified 'LoadAnnotations'.");
			return null;
		}
	}
}

