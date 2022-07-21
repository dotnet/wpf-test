// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  *** FILE IS AUTOMATICALLY GENERATED, DO NOT EDIT BY HAND ***
//
//        Generated: 11/30/2005 5:20:17 PM

// Required proxy imports.
using Annotations.Test.Reflection;
using System.Reflection;
using System.Windows.Controls;

// Following are used to access public types that don't have proxies
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


// Delegate specific imports.
using System;
using System.Collections.Generic;
using System.Windows;
using Proxies.System.Windows.Annotations;
using Proxies.MS.Internal.Annotations;
using System.Xml;
using System.Windows.Controls.Primitives;

namespace Proxies.MS.Internal.Annotations.Anchoring
{
	public class TextSelectionProcessor : SelectionProcessor
	{

		//------------------------------------------------------
		//
		//  Delegate Constructors
		//
		//------------------------------------------------------

		public TextSelectionProcessor()
		: base (Type.EmptyTypes, new object[0])
		{
			//Empty.
		}

		//------------------------------------------------------
		//
		//  Proxy Constructors
		//
		//------------------------------------------------------

		static TextSelectionProcessor() { InitializeStaticFields(staticstatic_DelegateAssembly, MethodBase.GetCurrentMethod().DeclaringType); }
		protected TextSelectionProcessor(Type[] types, object[] values) : base (types, values) { }
		protected TextSelectionProcessor(object delegateObject) : base (delegateObject) { }

		//------------------------------------------------------
		//
		//  Delegate Methods
		//
		//------------------------------------------------------

		override public Boolean MergeSelections(Object anchor1, Object anchor2, out Object newAnchor)
		{
			object [] parameters = new object[3];
			parameters[0] = anchor1;
			parameters[1] = anchor2;
			Boolean routedResult = (Boolean) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			newAnchor = (Object) parameters[2];
			return routedResult;
		}
		override public IList<DependencyObject> GetSelectedNodes(Object selection)
		{
			object [] parameters = new object[1];
			parameters[0] = selection;
			IList<DependencyObject> routedResult = (IList<DependencyObject>) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		override public UIElement GetParent(Object selection)
		{
			object [] parameters = new object[1];
			parameters[0] = selection;
			UIElement routedResult = (UIElement) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		override public Point GetAnchorPoint(Object selection)
		{
			object [] parameters = new object[1];
			parameters[0] = selection;
			Point routedResult = (Point) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		override public IList<ContentLocatorPart> GenerateLocatorParts(Object selection, DependencyObject startNode)
		{
			object [] parameters = new object[2];
			parameters[0] = selection;
			parameters[1] = startNode;
			IList<ContentLocatorPart> routedResult = (IList<ContentLocatorPart>) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		override public Object ResolveLocatorPart(ContentLocatorPart locatorPart, DependencyObject startNode, out AttachmentLevel attachmentLevel)
		{
			object [] parameters = new object[3];
			parameters[0] = locatorPart;
			parameters[1] = startNode;
			Object routedResult = (Object) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			attachmentLevel = (AttachmentLevel) parameters[2];
			return routedResult;
		}
		override public XmlQualifiedName[] GetLocatorPartTypes()
		{
			object [] parameters = new object[0];
			XmlQualifiedName[] routedResult = (XmlQualifiedName[]) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public void SetTargetDocumentPageView(DocumentPageView target)
		{
			object [] parameters = new object[1];
			parameters[0] = target;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}

		//------------------------------------------------------
		//
		//  Proxy Methods
		//
		//------------------------------------------------------

		public override string DelegateClassName()
		{
			return "MS.Internal.Annotations.Anchoring.TextSelectionProcessor";
		}
		protected override string DelegateAssemblyName()
		{
			return "PresentationFramework";
		}

		//------------------------------------------------------
		//
		//  Properties
		//
		//------------------------------------------------------

		public Boolean Clamping
		{
			set
			{
				RouteInstance(MethodBase.GetCurrentMethod(), new object[] { value });
			}
		}

		//------------------------------------------------------
		//
		//  Delegate Static Fields
		//
		//------------------------------------------------------

		public static XmlQualifiedName CharacterRangeElementName;
		public static XmlQualifiedName[] LocatorPartTypeNames;
		public static String SegmentAttribute = "Segment";
		public static String CountAttribute = "Count";
		public static String IncludeOverlaps = "IncludeOverlaps";

		//------------------------------------------------------
		//
		//  Proxy Static Fields
		//
		//------------------------------------------------------


		//------------------------------------------------------
		//
		//  Events
		//
		//------------------------------------------------------


		//------------------------------------------------------
		//
		//  Delegate Non-Static Fields (as properties)
		//
		//------------------------------------------------------
        
		private DocumentPageView _targetPage
		{
			get { return (DocumentPageView) GetField("_targetPage"); }
			set { SetField("_targetPage", value); }
		}

        private Boolean _clamping
		{
			get { return (Boolean) GetField("_clamping"); }
			set { SetField("_clamping", value); }
		}
	}
}
