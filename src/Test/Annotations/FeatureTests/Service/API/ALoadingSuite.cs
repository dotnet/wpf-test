// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Abstract base class that contains helper methods and
//				classes useful for testing AnnotationService's loading
//				API.

using Annotations.Test.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using Proxies.System.Windows.Annotations;
using Proxies.MS.Internal.Annotations.Anchoring;
using Proxies.MS.Internal.Annotations.Component;
using System.Windows.Annotations.Storage;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Controls.Primitives;
using System.Reflection;
using Proxies.MS.Internal.Annotations;
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

namespace Avalon.Test.Annotations
{
	public abstract class ALoadingSuite : TestSuite
	{
		/// <summary>
		/// FAIL if 'expectedAnnotation' is not in the list of AttachedAnnotations.
		/// </summary>
		protected void VerifyAnnotationIsAttached(ICollection<IAttachedAnnotation> attachedAnnotations, Annotation expectedAnnotation)
		{
			bool found = false;
			IEnumerator<IAttachedAnnotation> enumer = attachedAnnotations.GetEnumerator();
			while (enumer.MoveNext())
			{
				if (enumer.Current.Annotation == expectedAnnotation)
				{
					found = true;
					break;
				}
			}

			if (!found)
				failTest("Expected annotation to be in the list of AttachedAnnotations: " + expectedAnnotation.ToString());
		}

		/// <summary>
		/// FAIL test if annotations set is not equivalent to the AttachedAnnotations set.
		/// </summary>
		protected void VerifyAttachedAnnotations(ICollection<IAttachedAnnotation> attachedAnnotations, Annotation[] annotations)
		{
			assertEquals("Check number of attached annotations.", annotations.Length, attachedAnnotations.Count);
			for (int i = 0; i < annotations.Length; i++)
				VerifyAnnotationIsAttached(attachedAnnotations, annotations[i]);
		}

		/// <summary>
		/// FAIL test if any of the given annotations appear in the list of AttachedAnnotations.
		/// </summary>
		protected void VerifyAnnotationsAreNotAttached(ICollection<IAttachedAnnotation> attachedAnnotations, Annotation[] notAttached)
		{
			for (int i = 0; i < notAttached.Length; i++)
			{
				IEnumerator<IAttachedAnnotation> enumer = attachedAnnotations.GetEnumerator();
				while (enumer.MoveNext())
				{
					if (enumer.Current.Annotation == notAttached[i])
						failTest("Given annotation should not have been attached but was: " + notAttached[i].ToString());
				}
			}
		}

		/// <summary>
		/// Add annotations to AnnotationTestHelper.BuildMultiBranchTree.
		/// </summary>
		/// <returns>A map from Node id (e.g. "mainDockPanel" etc) to all annotations 
		/// attached to the subtree rooted at this node.</returns>
		protected Hashtable AnnotateMultiBranchTree(DependencyObject root)
		{
			// Without a displayed Window you cannot add annotations to a TextRange.
			Window window = new Window();
			window.Content = (UIElement) root;
			window.Show();
			// End hack.

			AnnotationService service = AnnotationService.GetService(root);
			AnnotationStore store = service.Store;
	
			Annotation mainDPAnno = AnnotationTestHelper.makeSimpleAnnotation(service, LogicalTreeHelper.FindLogicalNode(root, "mainDockPanel"));
			store.AddAnnotation(mainDPAnno);

			// DockPanel1 subtree.
			Annotation dp1Anno = AnnotationTestHelper.makeSimpleAnnotation(service, LogicalTreeHelper.FindLogicalNode(root, "_dockpanel1"));
			Annotation dp1CanvasAnno = AnnotationTestHelper.makeSimpleAnnotation(service, LogicalTreeHelper.FindLogicalNode(root, "dp1Canvas"));
			Annotation dp1Btn1Anno = AnnotationTestHelper.makeSimpleAnnotation(service, LogicalTreeHelper.FindLogicalNode(root, "btnAddAnnot1"));
			store.AddAnnotation(dp1Anno);
			store.AddAnnotation(dp1CanvasAnno);
			store.AddAnnotation(dp1Btn1Anno);

			// DockPanel2 subtree.
			Annotation dp2Anno1 = AnnotationTestHelper.makeSimpleAnnotation(service, LogicalTreeHelper.FindLogicalNode(root, "_dockpanel2"));
			Annotation dp2Anno2 = AnnotationTestHelper.makeSimpleAnnotation(service, LogicalTreeHelper.FindLogicalNode(root, "_dockpanel2"));
			TextBox tb = (TextBox) LogicalTreeHelper.FindLogicalNode(root, "textbox1");
			Annotation dp2TextBoxAnno = AnnotationTestHelper.makeSimpleAnnotation(service, tb);

            store.AddAnnotation(dp2Anno1);
			store.AddAnnotation(dp2Anno2);
			store.AddAnnotation(dp2TextBoxAnno);

			// Map from Logical Node id -> array of Annotations at and below this node.
			// (e.g. All annotations attached to the subtree represented by node id.)
			Hashtable annotationAttachmentMap = new Hashtable();

			annotationAttachmentMap.Add("mainDockPanel", new Annotation[] { mainDPAnno, dp1Anno, dp1CanvasAnno, dp1Btn1Anno, dp2Anno1, dp2Anno2, dp2TextBoxAnno});
			annotationAttachmentMap.Add("_dockpanel1", new Annotation[] { dp1Anno, dp1CanvasAnno, dp1Btn1Anno });
			annotationAttachmentMap.Add("dp1Canvas", new Annotation[] { dp1CanvasAnno });
			annotationAttachmentMap.Add("btnAddAnnot1", new Annotation[] { dp1Btn1Anno });
			annotationAttachmentMap.Add("_dockpanel2", new Annotation[] { dp2Anno1, dp2Anno2, dp2TextBoxAnno });

			return annotationAttachmentMap;
		}

		/// <summary>
		/// SubTreeProcessor that processes all nodes that have DataIDProcessor.DataIdProperty set.
		/// e.g. DataIdProcessor with FetchAsBatch property removed.
		/// </summary>
		protected class TestSubTreeProcessor : DataIdProcessor
		{
			public TestSubTreeProcessor(LocatorManager manager) : base(manager)
			{
				// Empty.
			}

			public override System.Collections.Generic.IList<IAttachedAnnotation> PreProcessNode(DependencyObject node, out bool calledProcessAnnotations)
			{
				if (node == null)
					throw new ArgumentNullException("node");

				// We get the local value so we can distinguish between the property
				// being set or not.  We don't want to rely on null or String.Empty because
				// those might have been the values set.
				object dataId = node.ReadLocalValue(DataIdProcessor.DataIdProperty);

				// If the current node has an Name set on it and FetchAnnotationsAsBatch is
				// set to true, we process this node immediately and return.  All its children
				// will be processed indirectly.
				if (dataId != DependencyProperty.UnsetValue)
				{
					calledProcessAnnotations = true;
					return Manager.ProcessAnnotations(node);
				}

				calledProcessAnnotations = false;
				return null;
			}

			public override System.Collections.Generic.IList<IAttachedAnnotation> PostProcessNode(DependencyObject node, bool childrenCalledProcessAnnotations, out bool calledProcessAnnotations)
			{
				if (node == null)
					throw new ArgumentNullException("node");

				// We get the local value so we can distinguish between the property
				// being set or not.  We don't want to rely on null or String.Empty because
				// those might have been the values set.
				object dataId = node.ReadLocalValue(DataIdProcessor.DataIdProperty);

				// If no children were processed, we try and process this node
				if (!childrenCalledProcessAnnotations && dataId != DependencyProperty.UnsetValue)
				{
					calledProcessAnnotations = true;
					return Manager.ProcessAnnotations(node);
				}

				calledProcessAnnotations = false;
				return null;
			}
		}
	}
}

