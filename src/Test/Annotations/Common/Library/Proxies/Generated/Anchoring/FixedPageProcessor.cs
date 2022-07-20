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
using Proxies.MS.Internal.Annotations.Anchoring;
using System;
using System.Collections.Generic;
using System.Windows;
using Proxies.System.Windows.Annotations;
using System.Xml;

namespace Proxies.MS.Internal.Annotations.Anchoring
{
	public class FixedPageProcessor : SubTreeProcessor
	{

		//------------------------------------------------------
		//
		//  Delegate Constructors
		//
		//------------------------------------------------------

		public FixedPageProcessor(LocatorManager manager)
		: base (new Type[] { typeof(LocatorManager) }, new object[] { manager })
		{
			//Empty.
		}
		public FixedPageProcessor()
		: base (Type.EmptyTypes, new object[0])
		{
			//Empty.
		}

		//------------------------------------------------------
		//
		//  Proxy Constructors
		//
		//------------------------------------------------------

		static FixedPageProcessor() { InitializeStaticFields(staticstatic_DelegateAssembly, MethodBase.GetCurrentMethod().DeclaringType); }
		protected FixedPageProcessor(Type[] types, object[] values) : base (types, values) { }
		protected FixedPageProcessor(object delegateObject) : base (delegateObject) { }

		//------------------------------------------------------
		//
		//  Delegate Methods
		//
		//------------------------------------------------------

		override public IList<IAttachedAnnotation> PreProcessNode(DependencyObject node, out Boolean calledProcessAnnotations)
		{
			object [] parameters = new object[2];
			parameters[0] = node;
			IList<IAttachedAnnotation> routedResult = (IList<IAttachedAnnotation>) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			calledProcessAnnotations = (Boolean) parameters[1];
			return routedResult;
		}
		override public ContentLocator GenerateLocator(PathNode node, out Boolean continueGenerating)
		{
			object [] parameters = new object[2];
			parameters[0] = node;
			ContentLocator routedResult = (ContentLocator) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			continueGenerating = (Boolean) parameters[1];
			return routedResult;
		}
		override public DependencyObject ResolveLocatorPart(ContentLocatorPart locatorPart, DependencyObject startNode, out Boolean continueResolving)
		{
			object [] parameters = new object[3];
			parameters[0] = locatorPart;
			parameters[1] = startNode;
			DependencyObject routedResult = (DependencyObject) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			continueResolving = (Boolean) parameters[2];
			return routedResult;
		}
		override public XmlQualifiedName[] GetLocatorPartTypes()
		{
			object [] parameters = new object[0];
			XmlQualifiedName[] routedResult = (XmlQualifiedName[]) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		static public ContentLocatorPart CreateLocatorPart(Int32 page)
		{
			object [] parameters = new object[1];
			parameters[0] = page;
			ContentLocatorPart routedResult = (ContentLocatorPart) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, staticstatic_DelegateAssembly);
			return routedResult;
		}

		//------------------------------------------------------
		//
		//  Proxy Methods
		//
		//------------------------------------------------------

		public override string DelegateClassName()
		{
			return "MS.Internal.Annotations.Anchoring.FixedPageProcessor";
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

		public Boolean UseLogicalTree
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

		public static String Id = "FixedPage";
		public static String ValueAttributeName = "Value";
		public static XmlQualifiedName PageNumberElementName;
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
        
		private Boolean _useLogicalTree
		{
			get { return (Boolean) GetField("_useLogicalTree"); }
			set { SetField("_useLogicalTree", value); }
		}
	}
}
