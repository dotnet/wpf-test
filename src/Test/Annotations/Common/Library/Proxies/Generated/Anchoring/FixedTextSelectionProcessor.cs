// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  *** FILE IS AUTOMATICALLY GENERATED, DO NOT EDIT BY HAND ***
//
//        Generated: 10/6/2005 4:11:54 PM

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
using System.Windows.Documents;

namespace Proxies.MS.Internal.Annotations.Anchoring
{
	public class FixedTextSelectionProcessor : SelectionProcessor
	{

		//------------------------------------------------------
		//
		//  Delegate Constructors
		//
		//------------------------------------------------------

		public FixedTextSelectionProcessor()
		: base (Type.EmptyTypes, new object[0])
		{
			//Empty.
		}

		//------------------------------------------------------
		//
		//  Proxy Constructors
		//
		//------------------------------------------------------

		static FixedTextSelectionProcessor() { InitializeStaticFields(staticstatic_DelegateAssembly, MethodBase.GetCurrentMethod().DeclaringType); }
		protected FixedTextSelectionProcessor(Type[] types, object[] values) : base (types, values) { }
		protected FixedTextSelectionProcessor(object delegateObject) : base (delegateObject) { }

		//------------------------------------------------------
		//
		//  Delegate Methods
		//
		//------------------------------------------------------

		override public Boolean MergeSelections(Object selection1, Object selection2, out Object newSelection)
		{
			object [] parameters = new object[3];
			parameters[0] = selection1;
			parameters[1] = selection2;
			Boolean routedResult = (Boolean) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			newSelection = (Object) parameters[2];
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
		public DocumentPage GetDocumentPage(FixedPage page)
		{
			object [] parameters = new object[1];
			parameters[0] = page;
			DocumentPage routedResult = (DocumentPage) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public Point GetPoint(String xstr, String ystr, String pointType)
		{
			object [] parameters = new object[3];
			parameters[0] = xstr;
			parameters[1] = ystr;
			parameters[2] = pointType;
			Point routedResult = (Point) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}

		//------------------------------------------------------
		//
		//  Proxy Methods
		//
		//------------------------------------------------------

		public override string DelegateClassName()
		{
			return "MS.Internal.Annotations.Anchoring.FixedTextSelectionProcessor";
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


		//------------------------------------------------------
		//
		//  Delegate Static Fields
		//
		//------------------------------------------------------

		public static String StartXAttribute = "StartX";
		public static String StartYAttribute = "StartY";
		public static String EndXAttribute = "EndX";
		public static String EndYAttribute = "EndY";
		public static XmlQualifiedName FixedTextElementName;
		public static XmlQualifiedName[] LocatorPartTypeNames;

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

	}
}
