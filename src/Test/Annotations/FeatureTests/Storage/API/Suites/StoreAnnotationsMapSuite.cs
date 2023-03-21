// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Whitebox tests of the internal caching mechanism of an
//				AnnotationStore.


using StoreContentAction = System.Windows.Annotations.Storage.StoreContentAction;
using StoreContentChangedEventArgs = System.Windows.Annotations.Storage.StoreContentChangedEventArgs;
using StoreContentChangedEventHandler = System.Windows.Annotations.Storage.StoreContentChangedEventHandler;
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

using System.Windows.Annotations.Storage;
using Proxies.System.Windows.Annotations;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Xml;
using System.Windows.Controls;
using System.Reflection;
using System.Windows.Documents;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Annotations.Test.Framework;

namespace Avalon.Test.Annotations.Suites
{
    [ExecutionGroupCompatible(false)]
    [TestDimension("-xmlstream")]
	public class StoreAnnotationsMapSuite_Pri1 : AnnotationStoreAPI
	{
		// ----------------------------------------------------------------------------------
		//                                   TEST CASES
		// ----------------------------------------------------------------------------------

		#region Test Cases

		/// <summary>
		/// GetAnnotations(): AddAnnotation.  GetAnnotations returns correct.
		/// </summary>
        [Priority(1)]
        public void annotationmap1(AnnotationStore store)
		{
			Annotation anno = AnnotationStoreTestHelpers.MakeAnnotation1();
			store.AddAnnotation(anno);
	
			IList<Annotation> result = store.GetAnnotations();
			VerifyAnnotationIdentities(result, new Annotation[] { anno });
			passTest("Simplest identity test passed.");
		}

		/// <summary>
		/// GetAnnotations(): Add 2 annotations. Flush.  Delete annotation. GetAnnotations returns 
		/// correct annotations.
		/// </summary>
        [Priority(1)]
        public void annotationmap2(AnnotationStore store)
		{
			Annotation anno1 = AnnotationStoreTestHelpers.MakeAnnotation1();
			Annotation anno2 = AnnotationStoreTestHelpers.MakeAnnotation2();
			store.AddAnnotation(anno1);
			store.AddAnnotation(anno2);
			store.Flush();
			store.DeleteAnnotation(anno1.Id);

			VerifyAnnotationIdentities(store.GetAnnotations(), new Annotation[] { anno2 });
			passTest("Verified add,add,flush,delete.");
		}

		/// <summary>
		/// GetAnnotations(): Add 2 annotations.  Flush.  Modify 1.  GetAnnotation(id) returns 
		/// expected annotation, verify modification.
		/// </summary>
        [Priority(1)]
        public void annotationmap6(AnnotationStore store)
		{
			Annotation anno1 = AnnotationStoreTestHelpers.MakeAnnotation2();
			Annotation anno2 = new Annotation(new XmlQualifiedName("simple", "Test"));
			store.AddAnnotation(anno1);
			store.AddAnnotation(anno2);
			store.Flush();

			AnnotationResource cargo = AnnotationTestHelper.MakeCargo("foo", "bar", "some content");
			anno2.Cargos.Add(cargo);

			VerifyAnnotationIdentities(store.GetAnnotations(), new Annotation[] { anno1, anno2 });
			Assert("Verify modified cargo exists.", store.GetAnnotations()[1].Cargos.Contains(cargo));

			passTest("Verified modify after flush.");
		}

		/// <summary>
		/// GetAnnotation(Id): AddAnnotation. GetAnnotation(id) returns correct instance.
		/// </summary>
        [Priority(1)]
        public void annotationmap7(AnnotationStore store)
		{
			Annotation anno1 = AnnotationStoreTestHelpers.MakeAnnotation1();
			store.AddAnnotation(anno1);
			Assert("Verify identity of GetAnnotation(Id).", anno1 == store.GetAnnotation(anno1.Id));
			passTest("Verified map returns single instance.");
		}

		/// <summary>
		/// GetAnnotation(Id): AddAnnotation. Flush. Modify annotation.  GetAnnotation(id) 
		/// returns correct instance.
		/// </summary>
        [Priority(1)]
        public void annotationmap8(AnnotationStore store)
		{
			Annotation anno1 = AnnotationStoreTestHelpers.MakeAnnotation1();
			store.AddAnnotation(anno1);
			store.Flush();
			
			AnnotationResource cargo = AnnotationTestHelper.MakeCargo("a", "b", "my cargo!!");
			anno1.Cargos.Add(cargo);

			Assert("Verify identity of GetAnnotation(Id).", anno1 == store.GetAnnotation(anno1.Id));
			Assert("Verify annotation modification is preserved.", store.GetAnnotation(anno1.Id).Cargos.Contains(cargo));
			passTest("Verify newer annotation in map than in store.");
		}

		/// <summary>
		/// GetAnnotation(Id): AddAnnotation. Flush. Set stream to empty store.  Set Stream back.  
		/// Multiple calls to GetAnnotation(id) return same instance.
		/// </summary>
        [Priority(1)]
        public void annotationmap9(AnnotationStore store)
		{
			Annotation anno1 = AnnotationStoreTestHelpers.MakeAnnotation2();
			store.AddAnnotation(anno1);
			store.Flush();

            CloseStore();
            store = CreateStore();

			Assert("Verify consecutive calls to GetAnnotation return same instance.", store.GetAnnotation(anno1.Id) == store.GetAnnotation(anno1.Id));

			passTest("Verified annotation in store but not in map.");
		}

		/// <summary>
		/// GetAnnotation(Id): Add annotation. Flush. Add annotation.  GetAnnotation(id) return correct 
		/// instance for both annotations.
		/// </summary>
		/// <param name="store"></param>
        [Priority(1)]
        public void annotationmap10(AnnotationStore store)
		{
			Annotation anno1 = AnnotationStoreTestHelpers.MakeAnnotation2();
			store.AddAnnotation(anno1);
			store.Flush();

			Annotation anno2 = AnnotationStoreTestHelpers.MakeAnnotation1();
			store.AddAnnotation(anno2);

			Assert("Verify identity of 1st GetAnnotation(Id).", anno1 == store.GetAnnotation(anno1.Id));
			Assert("Verify identity of 2nd GetAnnotation(Id).", anno2 == store.GetAnnotation(anno2.Id));

			passTest("Verified 2 annotations in map but only 1 in store.");
		}

		/// <summary>
		/// GetAnnotations(ContentLocator): Add 3 annotations to subtree. GetAnnotations 
		/// returns only annotations below given ContentLocator.
		/// </summary>
		/// <param name="store"></param>
        [Priority(1)]
        public object annotationmap11(object inObj)
		{
			StorageTestContext context = (StorageTestContext)inObj;
			AnnotationService service = AnnotationService.GetService(context.root);
			
			DependencyObject rootNode = LogicalTreeHelper.FindLogicalNode(context.root, "mainDockPanel");
			Annotation rootAnnotation = AnnotationTestHelper.makeSimpleAnnotation(service, rootNode);
			DependencyObject subtree1Node = LogicalTreeHelper.FindLogicalNode(context.root, "_dockpanel1");
			Annotation subtree1Annotation = AnnotationTestHelper.makeSimpleAnnotation(service, subtree1Node);
			DependencyObject subtree2Node = LogicalTreeHelper.FindLogicalNode(context.root, "textbox1");
			Annotation subtree2Annotation = AnnotationTestHelper.makeSimpleAnnotation(service, subtree2Node);

			context.store.AddAnnotation(rootAnnotation);
			context.store.AddAnnotation(subtree1Annotation);
			context.store.AddAnnotation(subtree2Annotation);

			// Test GetAnnotations above all annotations.
			IList<ContentLocatorBase> locators = service.LocatorManager.GenerateLocators(rootNode);
			VerifyAnnotationIdentities(context.store.GetAnnotations(locators[0] as ContentLocator), new Annotation[] { rootAnnotation, subtree1Annotation, subtree2Annotation });

			// Test GetAnnotations in subtrees.
			locators = service.LocatorManager.GenerateLocators(subtree1Node);
			VerifyAnnotationIdentities(context.store.GetAnnotations(locators[0] as ContentLocator), new Annotation[] { subtree1Annotation });

			locators = service.LocatorManager.GenerateLocators(subtree2Node);
			VerifyAnnotationIdentities(context.store.GetAnnotations(locators[0] as ContentLocator), new Annotation[] { subtree2Annotation });

			// Test GetAnnotations below all annotations.
			DependencyObject leafNode = LogicalTreeHelper.FindLogicalNode(context.root, "dp1Canvas");
			locators = service.LocatorManager.GenerateLocators(leafNode);
			VerifyAnnotationIdentities(context.store.GetAnnotations(locators[0] as ContentLocator), new Annotation[] { });

			passTest("Verified GetAnnotations(ContentLocator) returns expected annotations.");
			return null;
		}

		/// <summary>
		/// GetAnnotations(ContentLocator): Add 1 annotation.  Flush. Verify result of GetAnnotations. 
		/// Modify annotation.  Verify GetAnnotations returns expected annotation with modification.
		/// </summary>
		/// <param name="store"></param>
        [Priority(1)]
        public object annotationmap12(object inObj)
		{
			StorageTestContext context = (StorageTestContext)inObj;
			AnnotationService service = AnnotationService.GetService(context.root);

			DependencyObject rootNode = LogicalTreeHelper.FindLogicalNode(context.root, "mainDockPanel");
			Annotation rootAnnotation = AnnotationTestHelper.makeSimpleAnnotation(service, rootNode);
			context.store.AddAnnotation(rootAnnotation);
			context.store.Flush();

			// Modify annotation.
			AnnotationResource cargo = AnnotationTestHelper.MakeCargo("simple", "Test", "some content");
			rootAnnotation.Cargos.Add(cargo);

			// Test GetAnnotations above all annotations.
			IList<ContentLocatorBase> locators = service.LocatorManager.GenerateLocators(rootNode);
			IList<Annotation> annos = context.store.GetAnnotations(locators[0] as ContentLocator);
			VerifyAnnotationIdentities(annos, new Annotation[] { rootAnnotation });
			Assert("Verify that cargo is present.", rootAnnotation.Cargos.Contains(cargo));

			passTest("Verified GetAnnotations(ContentLocator) when map more current than store.");
			return null;
		}

		/// <summary>
		/// GetAnnotations(ContentLocator): Add 1 annotation with 2 anchors each in a different subtree.  
		/// Verify that GetAnnotations on each subtree returns this annotation.  GetAnnotations above subtrees
		/// returns annotation once.
		/// </summary>
		/// <param name="store"></param>
        [Priority(1)]
        public object annotationmap13(object inObj)
		{
			StorageTestContext context = (StorageTestContext)inObj;
			AnnotationService service = AnnotationService.GetService(context.root);
			
			Annotation anno = new Annotation(new XmlQualifiedName("simple", "Test"));
			DependencyObject subtree1 = LogicalTreeHelper.FindLogicalNode(context.root, "_dockpanel1");
			DependencyObject subtree2 = LogicalTreeHelper.FindLogicalNode(context.root, "_dockpanel2");
			anno.Anchors.Add(AnnotationTestHelper.makeAnchor(service, subtree1));
			anno.Anchors.Add(AnnotationTestHelper.makeAnchor(service, subtree2));
			context.store.AddAnnotation(anno);

			IList<ContentLocatorBase> locators;
			Annotation[] expectedAnnos = new Annotation[] { anno };

			// Test GetAnnotations in subtree 1.
			locators = service.LocatorManager.GenerateLocators(subtree1);
			VerifyAnnotationIdentities(context.store.GetAnnotations(locators[0] as ContentLocator), expectedAnnos);

			// Test GetAnnotations in subtree 2.
			locators = service.LocatorManager.GenerateLocators(subtree2);
			VerifyAnnotationIdentities(context.store.GetAnnotations(locators[0] as ContentLocator), expectedAnnos);

			// Test GetAnnotations above subtrees.
			DependencyObject rootNode = LogicalTreeHelper.FindLogicalNode(context.root, "mainDockPanel");
			locators = service.LocatorManager.GenerateLocators(rootNode);
			VerifyAnnotationIdentities(context.store.GetAnnotations(locators[0] as ContentLocator), expectedAnnos);

			passTest("Verified GetAnnotations(ContentLocator) for annotation with multiple anchors.");
			return null;
		}

		/// <summary>
		/// GetAnnotations(ContentLocator):  14- Add 1 annotation within a textbox. 
		/// Verify that GetAnnotations(ContentLocator) on the root returns this annotation.
		/// </summary>
        [Priority(1)]
        public object annotationmap14(object inObj)
		{
			StorageTestContext context = (StorageTestContext)inObj;
			AnnotationService service = AnnotationService.GetService(context.root);

			Annotation anno = new Annotation(new XmlQualifiedName("simple", "Test"));
			TextBox textbox = (TextBox) LogicalTreeHelper.FindLogicalNode(context.root, "textbox1");
			textbox.Select(0, 10);
            PropertyInfo TextSelectionInfo = typeof(TextBoxBase).GetProperty("TextSelectionInternal", BindingFlags.NonPublic | BindingFlags.Instance);
            if (TextSelectionInfo == null)
            {
                throw new Exception("TextBoxBase.TextSelectionInternal property is not accessible");
            }
            TextSelection textSelection = TextSelectionInfo.GetValue(textbox, null) as TextSelection;
            TextRange range = new TextRange(textSelection.Start, textSelection.End);
            anno.Anchors.Add(AnnotationTestHelper.makeAnchor(service, range));
			context.store.AddAnnotation(anno);

			// Test GetAnnotations above subtrees.
			DependencyObject rootNode = LogicalTreeHelper.FindLogicalNode(context.root, "mainDockPanel");
			IList<ContentLocatorBase> locators = service.LocatorManager.GenerateLocators(rootNode);
			VerifyAnnotationIdentities(context.store.GetAnnotations(locators[0] as ContentLocator), new Annotation[] { anno });

			passTest("Verified GetAnnotations(ContentLocator) for annotation with different processor.");
			return null;
		}

		#endregion Test Cases

		// ----------------------------------------------------------------------------------
		//                                 PRIVATE METHODS
		// ----------------------------------------------------------------------------------

		#region Private Methods


		/// <summary>
		/// Verify that the given list and array of annotations contain the same annotations (identity based test).
		/// </summary>
		private void VerifyAnnotationIdentities(IList<Annotation> annotationsToTest, Annotation[] expectedAnnotations)
		{
			AssertEquals("Verify expected number of annotations.", expectedAnnotations.Length, annotationsToTest.Count);
			for (int i = 0; i < expectedAnnotations.Length; i++)
			{
				Annotation current = expectedAnnotations[i];
				int idx = annotationsToTest.IndexOf(current);
				if (idx >= 0)
				{
					Assert("Verify annotation " + i + "'s identity.", current == annotationsToTest[idx]);
				}
			}
			printStatus("Verified that list of annotations is the same as expected annotations.");
		}

		#endregion Private Methods

		// ----------------------------------------------------------------------------------
		//                                 PRIVATE CLASSES
		// ----------------------------------------------------------------------------------

		#region Private Classes

		/// <summary>
		/// Context object used for passing multiple objects to a Context task.
		/// </summary>
		class StorageTestContext
		{
			public AnnotationStore store;
			public Canvas root;

			/// <summary>
			/// Create a logical tree and attach a Service to the root.  Return a context
			/// that contains the root of this tree.
			/// </summary>
			public static StorageTestContext SetupTreeWithService(AnnotationStore aStore) 
			{
				StorageTestContext context = new StorageTestContext();
				context.store = aStore;
				context.root = AnnotationTestHelper.BuildMultiBranchTree();
				AnnotationService service = new AnnotationService(context.root);
				service.Enable(context.store);
				// AnnotationService.SetStore(context.root, context.store);
				return context;
			}
		}

		#endregion Private Classes
	}
}

