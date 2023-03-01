// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Superclass for the FindAttachedAnchor test suite.  This provides
//  some common functions and builds the tree used by the FindAttachedAnchor tests.
//
// 	FindAttachedAnchor starts looking for the node in the tree identified by the
//  given ContentLocatorBase, starting at startNode and searching downwards.
//
using System;
using System.Windows;
using System.Windows.Controls;

using System.Collections;
using System.Collections.Generic;

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
using Annotations.Test.Framework;
using Proxies.MS.Internal.Annotations;					// TestSuite.


namespace Avalon.Test.Annotations
{
	public abstract class AFindAttachedAnchorSuite : AAnchoringAPITests
	{
		#region globals
		protected Canvas a = null;
		protected Canvas b = null;
		protected Canvas c = null;
		protected Canvas d = null;
		protected Canvas e = null;
		protected Canvas f = null;
		protected Canvas g = null;
		protected TextBlock txt_h = null;
		#endregion

		/// <summary>
		/// Make the tree to be used by all FindAttachedAnchor tests.
		/// </summary>
		[TestCase_Setup()]
        protected void BuildTree()
		{
            DoSetup();
			a = new Canvas();
			EnableService(a, annotationStore);

			b = new Canvas();
			c = new Canvas();
			d = new Canvas();
			e = new Canvas();
			f = new Canvas();
			g = new Canvas();
			txt_h = new TextBlock();

			a.Children.Add(b);
			a.Children.Add(c);
			b.Children.Add(d);
			b.Children.Add(e);
			c.Children.Add(f);
			d.Children.Add(g);
			g.Children.Add(txt_h);

			DataIdProcessor.SetDataId(a, "a");
			DataIdProcessor.SetDataId(b, "b");
			DataIdProcessor.SetDataId(c, "c");
			DataIdProcessor.SetDataId(d, "d");
			DataIdProcessor.SetDataId(e, "e");
			DataIdProcessor.SetDataId(f, "f");
			DataIdProcessor.SetDataId(g, "g");
			DataIdProcessor.SetDataId(txt_h, "h");

			LocatorManager.SetSubTreeProcessorId(a, DataIdProcessor.Id);
			//LocatorManager.SetSubTreeProcessorId(g, TextFingerprintProcessor.Id);
		}

		/// <summary>
		/// Calls the FindAttachedAnchor (internal) method, packs all the parameters and
		/// unpacks the result
		/// </summary>
		/// <param name="startNode">Node to start searching from</param>
		/// <param name="targetNode">Node to look for</param>
		/// <param name="anchorType">Level of resolution achieved</param>
		/// <returns>the resolved anchor and the attachment level (as an out parameter)</returns>
		protected Object FindAttachedAnchor(DependencyObject startNode, DependencyObject targetNode, 
			out AttachmentLevel anchorType)
		{
			Object[] paramsArray = new Object[4];
			paramsArray[0] = startNode;
			paramsArray[1] = GetStartNodePrefix(startNode);
			paramsArray[2] = GetTargetLocator(targetNode);

			Object result = AnchoringAPIHelpers.CallInternalMethod("FindAttachedAnchor", manager, paramsArray);
			anchorType = (AttachmentLevel)paramsArray[3];

			return result;
		}


		/// <summary>
		/// Calls the FindAttachedAnchor (internal) method and passes in a null ContentLocatorBase
		/// as its third argument.  Also packs all the parameters and unpacks the result
		/// </summary>
		/// <param name="startNode">Node to start searching from</param>
		/// <param name="anchorType">Level of resolution achieved</param>
		/// <returns>the resolved anchor and the attachment level (as an out parameter)</returns>
		protected Object FindAttachedAnchor_NullLocatorArg(DependencyObject startNode, out AttachmentLevel anchorType)
		{
			Object[] paramsArray = new Object[4];
			paramsArray[0] = startNode;
			paramsArray[1] = GetStartNodePrefix(startNode);
			paramsArray[2] = null;

			Object result = AnchoringAPIHelpers.CallInternalMethod("FindAttachedAnchor", manager, paramsArray);
			anchorType = (AttachmentLevel)paramsArray[3];

			return result;
		}


		// -----------------------------------------------------------------------------------------------
		//										HELPER METHODS
		// -----------------------------------------------------------------------------------------------

		/// <summary>
		/// Generates Locators for the startNode and pass out the first ContentLocator
		/// from the generated ContentLocatorBase
		/// </summary>
		/// <param name="startNode">node we want the ContentLocatorBase prefix of</param>
		/// <returns>the first ContentLocator of the node's ContentLocatorBase</returns>
		private ContentLocator[] GetStartNodePrefix(DependencyObject startNode)
		{
			IList<ContentLocatorBase> startNodeLocators = manager.GenerateLocators(startNode);
			return new ContentLocator[1] { (ContentLocator)startNodeLocators[0] };
		}

		/// <summary>
		/// Generates Locators for the targetNode and returns the first ContentLocatorBase
		/// </summary>
		/// <param name="targetNode">node we want the ContentLocatorBase of</param>
		/// <returns>the first ContentLocatorBase in the list of generated Locators for the node</returns>
		private ContentLocatorBase GetTargetLocator(DependencyObject targetNode)
		{
			IList<ContentLocatorBase> targetLocators = manager.GenerateLocators(targetNode);
			return targetLocators[0] as ContentLocatorBase;
		}

	}		// end of AFindAttachedAnchorSuite class

}			// end of namespace

