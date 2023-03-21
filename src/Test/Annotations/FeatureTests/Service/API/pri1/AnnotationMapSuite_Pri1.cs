// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Whitebox tests for the internal AnnotationMap which is used
//				 by the AnnotationService to store attached annotations.

using Annotations.Test.Framework;				// TestSuite.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading; using System.Windows.Threading;						// DispatcherOperationCallback.
using System.Windows;
using Proxies.System.Windows.Annotations;
using Proxies.MS.Internal.Annotations.Anchoring;
using Proxies.MS.Internal.Annotations.Component;
using System.Windows.Annotations.Storage;
using System.Windows.Controls;
using System.Xml;
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

namespace Avalon.Test.Annotations.Pri1s
{
	public class AnnotationMapSuite_Pri1 : TestSuite
	{
		protected override void RunTestCase()
		{
			Canvas canvas = AnnotationTestHelper.BuildMultiBranchTree();
			ServiceTestContext context = ServiceTestContext.SetupWithServiceAndStore(this, canvas);

			switch (CaseNumber)
			{
				case "annotationmap1": queueTask(new DispatcherOperationCallback(annotationmap1), context); break;

				default:
					failTest("TestSuite '" + this.ToString() + "' does not contain the given test case: '" + CaseNumber + "'.");
					break;
			}
		}

		/// <summary>
		/// Add annotation.  Service.GetAttachedAnnotations().  Call clear on list.  Verify that second call to 
		/// Service.GetAttachedAnnotations() returns original contents.
		/// </summary>
		public object annotationmap1(object inObj)
		{
			ServiceTestContext context = (ServiceTestContext)inObj;

			Annotation anno = context.MakeAnnotation("textbox1");
			context.store.AddAnnotation(anno);

			assertEquals("Verify number of attached annos.", 1, context.service.GetAttachedAnnotations().Count);
			IAttachedAnnotation aa = AnnotationTestHelper.GetOnlyAttachedAnnotation(context.service);

			context.service.GetAttachedAnnotations().Clear();
			assertEquals("Verify that we didn't change the map.", 1, context.service.GetAttachedAnnotations().Count);
			passTest("Verified that we didn't gain unprotected access to AnnotationMap contents.");
			return null;
		}
	}
}

