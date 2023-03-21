// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Context object that can be passed as an argument to 
//				 various asynchronous tasks.

using Annotations.Test.Framework;				// TestSuite.

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls.Primitives;
using Proxies.System.Windows.Annotations;
using AnnotationService = Proxies.System.Windows.Annotations.AnnotationService;
using Proxies.MS.Internal.Annotations.Anchoring;
using Proxies.MS.Internal.Annotations.Component;
using System.Windows.Annotations.Storage;
using System.Windows.Annotations;

namespace Avalon.Test.Annotations
{
	/// <summary>
	/// Context that is sufficient for a number of Service API tests.
	/// Is necessary because of the ansynchronous nature of Enabling Services.
	/// </summary>
	public class ServiceTestContext
	{
		public const string FILENAME = "annotations.xml";

		public AnnotationStore store;
		public AnnotationService service;
		public DependencyObject enablementNode;
		public ServiceEventListener eventListener;

		public static ServiceTestContext SetupWithServiceAndStore(TestSuite suite, DependencyObject enablementNode, string filename)
		{
			// Cleanup from previous runs.
			if (File.Exists(filename))
				File.Delete(filename);

			ServiceTestContext context = new ServiceTestContext();
			AnnotationService annoService = new AnnotationService(enablementNode);
			context.enablementNode = enablementNode;
			// context.service = AnnotationService.GetService((DocumentViewerBase)context.enablementNode); Alternative?
			context.service = annoService;
            context.store = AnnotationTestHelper.CreateStore(filename);
			annoService.Enable(context.store);
			context.eventListener = new ServiceEventListener(suite, context.service);
			return context;
		}

		//public static ServiceTestContext SetupWithService(TestSuite suite, DependencyObject enablementNode)
		//{
		//    ServiceTestContext context = new ServiceTestContext();
		//    AnnotationService.Enable(enablementNode);
		//    context.enablementNode = enablementNode;
		//    context.service = AnnotationService.GetService(context.enablementNode);
		//    context.eventListener = new ServiceEventListener(suite, context.service);
		//    return context;
		//}

		public static ServiceTestContext SetupWithServiceAndStore(TestSuite suite, DependencyObject enablementNode)
		{
			return ServiceTestContext.SetupWithServiceAndStore(suite, enablementNode, FILENAME);
		}

		/// <summary>
		/// Create an simple annotation with a single anchor on the node with the given Id. 
		/// </summary>
		/// <exception cref="ArgumentException">If nodeId is not a valid node Id.</exception>
		public Annotation MakeAnnotation(string nodeId)
		{
			DependencyObject node = LogicalTreeHelper.FindLogicalNode(enablementNode, nodeId);
			if (node == null)
				throw new ArgumentException("Node name '" + nodeId + "' is not a valid node Id.");
			return AnnotationTestHelper.makeSimpleAnnotation(service, node);
		}

		/// <summary>
		/// Create an annotation with a single partially resolving anchor.
		/// Uses AnnotationTestHelper.MakePartiallyResolvedAnnotation.
		/// </summary>
		/// <exception cref="ArgumentException">If nodeId is not a UIElement, or parent of nodeId is not 
		/// a Panel</exception>
		public Annotation MakePartiallyResolvedAnnotation(string nodeId)
		{
			return AnnotationTestHelper.MakePartiallyResolvedAnnotation(service, enablementNode, nodeId);
		}

		/// <summary>
		/// Create an anchor for node with the given Id. 
		/// </summary>
		/// <exception cref="ArgumentException">If nodeId is not a valid node Id.</exception>
		public AnnotationResource MakeAnchor(string nodeId)
		{
			DependencyObject node = LogicalTreeHelper.FindLogicalNode(enablementNode, nodeId);
			if (node == null)
				throw new ArgumentException("Node name '" + nodeId + "' is not a valid node Id.");
			return AnnotationTestHelper.makeAnchor(service, node);
		}

		/// <summary>
		/// Create an anchor that has partially resolving locators.
		/// Uses AnnotationTestHelper.MakePartiallyResolvedAnchor.
		/// </summary>
		/// <exception cref="ArgumentException">If nodeId is not a UIElement, or parent of nodeId is not 
		/// a Panel</exception>
		public AnnotationResource MakePartiallyResolvedAnchor(string nodeId)
		{
			return AnnotationTestHelper.MakePartiallyResolvedAnchor(service, enablementNode, nodeId);
		}

		/// <summary>
		/// Generate locators for node with the given node id.
		/// </summary>
		public IList<ContentLocatorBase> GenerateLocators(string nodeId)
		{
			DependencyObject node = LogicalTreeHelper.FindLogicalNode(enablementNode, nodeId);
			if (node == null)
				throw new ArgumentException("Node name '" + nodeId + "' is not a valid node Id.");
			return service.LocatorManager.GenerateLocators(node);
		}
	}
}

