// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  *** FILE IS AUTOMATICALLY GENERATED, DO NOT EDIT BY HAND ***
//
//        Generated: 11/30/2004 5:20:21 PM

// Required proxy imports.
using Annotations.Test.Reflection;
using System.Reflection;
using System.Windows;

// Delegate specific imports.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Proxies.System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml;

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

namespace Proxies.MS.Internal.Annotations.Anchoring
{
	public class DynamicSelectionProcessor : SelectionProcessor
	{

		//------------------------------------------------------
		//
		//  Delegate Constructors
		//
		//------------------------------------------------------

		public DynamicSelectionProcessor(LocatorManager manager)
		: base (new Type[] { typeof(LocatorManager) }, new object[] { manager })
		{
			//Empty.
		}
		public DynamicSelectionProcessor()
		: base (null, null)
		{
			//Empty.
		}

		//------------------------------------------------------
		//
		//  Proxy Constructors
		//
		//------------------------------------------------------

		static DynamicSelectionProcessor() { InitializeStaticFields(staticstatic_DelegateAssembly, MethodBase.GetCurrentMethod().DeclaringType); }
		protected DynamicSelectionProcessor(Type[] types, object[] values) : base (types, values) { }
		protected DynamicSelectionProcessor(object delegateObject) : base (delegateObject) { }

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
			Boolean result = (Boolean) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			newSelection = (Object) parameters[2];
			return result;
		}
		override public IList<DependencyObject> GetSelectedNodes(Object selection)
		{
			object [] parameters = new object[1];
			parameters[0] = selection;
			IList<DependencyObject> result = (IList<DependencyObject>)RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return result;
		}
		override public UIElement GetParent(Object selection)
		{
			object [] parameters = new object[1];
			parameters[0] = selection;
			UIElement result = (UIElement)RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return result;
		}
		override public Point GetAnchorPoint(Object selection)
		{
			object [] parameters = new object[1];
			parameters[0] = selection;
			Point result = (Point) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return result;
		}
		override public IList<ContentLocatorPart> GenerateLocatorParts(Object selection, DependencyObject startNode)
		{
			object [] parameters = new object[2];
			parameters[0] = selection;
			parameters[1] = startNode;
			IList<ContentLocatorPart> result = (IList<ContentLocatorPart>) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return result;
		}
		public override object ResolveLocatorPart(ContentLocatorPart locatorPart, DependencyObject startNode, out AttachmentLevel attachmentLevel)
		{		
			object [] parameters = new object[3];
			parameters[0] = locatorPart;
			parameters[1] = startNode;
			Object result = (Object) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			attachmentLevel = (AttachmentLevel) parameters[2];
			return result;
		}
		override public XmlQualifiedName[] GetLocatorPartTypes()
		{
			object [] parameters = new object[0];
			XmlQualifiedName[] result = (XmlQualifiedName[]) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return result;
		}

		//------------------------------------------------------
		//
		//  Proxy Methods
		//
		//------------------------------------------------------

		public override string DelegateClassName()
		{
			return "System.Windows.Annotations.Anchoring.DynamicSelectionProcessor";
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

}
}
