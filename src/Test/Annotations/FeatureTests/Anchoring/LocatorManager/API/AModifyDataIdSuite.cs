// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Superclass for the ModifyDataId test suite.  This provides
//  some common functions and builds the tree used by the ModifyDataId tests.
//
//  ModifyDataId changes the DataId Set on elements in the tree and checks whether
//  annotations are unloaded then reloaded correctly to the renamed object
//

using System;
using System.Windows;
using System.Windows.Controls;

using System.Collections;

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

using Proxies.System.Windows.Annotations;
using Proxies.MS.Internal.Annotations.Anchoring;
using Annotations.Test.Framework;					// TestSuite.


namespace Avalon.Test.Annotations
{
	public abstract class AModifyDataIdSuite : AAnchoringAPITests
	{
		#region globals
		// Tree elements
		protected Window w1 = null;
		protected Canvas a1 = null;
		protected Canvas b1 = null;
		protected Canvas b2 = null;

		// Keeps track of attached annotations
		protected int annotationsCount_before;
		protected int annotationsCount_after;
		#endregion
	
		/// <summary>
		/// Make the tree to be used by all ModifyDataId tests.  Create annotations
		/// on nodes in the tree
		/// </summary>
		[TestCase_Setup()]
        protected void BuildTree()
		{
            DoSetup();
			w1 = new Window();

			EnableService(w1, annotationStore);

			a1 = new Canvas();
			b1 = new Canvas();
			b2 = new Canvas();

			a1.Name = "a1";
			b1.Name = "b1";
			b2.Name = "b2";

			a1.Children.Add(b1);
			a1.Children.Add(b2);

			DataIdProcessor.SetDataId(a1, a1.Name);
			DataIdProcessor.SetDataId(b1, b1.Name);
			DataIdProcessor.SetDataId(b2, b2.Name);
			w1.Content = a1;

			Annotation annotation_b1 = AnchoringAPIHelpers.CreateBasicAnnotation(b1, manager, annotationStore);
			Annotation annotation_b2 = AnchoringAPIHelpers.CreateBasicAnnotation(b2, manager, annotationStore);
		}

		/// <summary>
		/// Checks the number of attached annotations on the tree before the DataId
		/// of a node used in the locator is changed
		/// </summary>
		protected object AttachedAnnotationsBefore(object obj)
		{
			AnnotationService svc = AnnotationService.GetService((DependencyObject)w1);
			annotationsCount_before = svc.GetAttachedAnnotations().Count;

			return null;
		}

		/// <summary>
		/// Checks the number of attached annotations on the tree after the DataId 
		/// of a node used in the locator is changed
		/// </summary>
		protected object AttachedAnnotationsAfter(object obj)
		{
			AnnotationService svc = AnnotationService.GetService((DependencyObject)w1);
			annotationsCount_after = svc.GetAttachedAnnotations().Count;

			return null;
		}

		/// <summary>
		/// Checks the number of attached annotations on the tree after the DataId 
		/// of a node used in the locator is changed
		/// </summary>
		protected object VerifyAnnotationsLoaded(object obj)
		{
			object[] data = obj as object[];
			int expectedBefore = (int)data[0];
			int expectedAfter = (int)data[1];

			AttachedAnnotationsAfter(w1);
			if ((annotationsCount_before == expectedBefore) && (annotationsCount_after == expectedAfter))
				passTest(annotationsCount_before + " annotations before modifying Name, " + annotationsCount_after + 
					" after correctly returned");
			else
				failTest(expectedBefore + " annotations before modifying Name, " + expectedAfter + 
					" annotation after expected.  " +  annotationsCount_before + " annotations before modifying Name, " 
					+ annotationsCount_after + " annotations after returned");
			
			return null;
		}


	}		// end of AModifyDataIdSuite class

}			// end of namespace

