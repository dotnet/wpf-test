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
using System.Collections.Generic;
using System.Windows;
using System;
using Proxies.System.Windows.Annotations;
using System.Xml;

namespace Proxies.MS.Internal.Annotations.Anchoring
{
	public class DataIdProcessor : SubTreeProcessor
	{

		//------------------------------------------------------
		//
		//  Delegate Constructors
		//
		//------------------------------------------------------

		public DataIdProcessor(LocatorManager manager)
		: base (new Type[] { typeof(LocatorManager) }, new object[] { manager })
		{
			//Empty.
		}
		public DataIdProcessor()
		: base (Type.EmptyTypes, new object[0])
		{
			//Empty.
		}

		//------------------------------------------------------
		//
		//  Proxy Constructors
		//
		//------------------------------------------------------

		static DataIdProcessor() { InitializeStaticFields(staticstatic_DelegateAssembly, MethodBase.GetCurrentMethod().DeclaringType); }
		protected DataIdProcessor(Type[] types, object[] values) : base (types, values) { }
		protected DataIdProcessor(object delegateObject) : base (delegateObject) { }

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
		override public IList<IAttachedAnnotation> PostProcessNode(DependencyObject node, Boolean childrenCalledProcessAnnotations, out Boolean calledProcessAnnotations)
		{
			object [] parameters = new object[3];
			parameters[0] = node;
			parameters[1] = childrenCalledProcessAnnotations;
			IList<IAttachedAnnotation> routedResult = (IList<IAttachedAnnotation>) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			calledProcessAnnotations = (Boolean) parameters[2];
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
		static public void SetDataId(DependencyObject d, String id)
		{
			object [] parameters = new object[2];
			parameters[0] = d;
			parameters[1] = id;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, staticstatic_DelegateAssembly);
		}
		static public String GetDataId(DependencyObject d)
		{
			object [] parameters = new object[1];
			parameters[0] = d;
			String routedResult = (String) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, staticstatic_DelegateAssembly);
			return routedResult;
		}
		static public void SetFetchAnnotationsAsBatch(DependencyObject d, Boolean id)
		{
			object [] parameters = new object[2];
			parameters[0] = d;
			parameters[1] = id;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, staticstatic_DelegateAssembly);
		}
		static public Boolean GetFetchAnnotationsAsBatch(DependencyObject d)
		{
			object [] parameters = new object[1];
			parameters[0] = d;
			Boolean routedResult = (Boolean) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, staticstatic_DelegateAssembly);
			return routedResult;
		}
		static public void OnDataIdPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			object [] parameters = new object[2];
			parameters[0] = d;
			parameters[1] = e;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, staticstatic_DelegateAssembly);
		}
		static public Object CoerceDataId(DependencyObject d, Object value)
		{
			object [] parameters = new object[2];
			parameters[0] = d;
			parameters[1] = value;
			Object routedResult = (Object) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, staticstatic_DelegateAssembly);
			return routedResult;
		}
		public ContentLocatorPart CreateLocatorPart(DependencyObject node)
		{
			object [] parameters = new object[1];
			parameters[0] = node;
			ContentLocatorPart routedResult = (ContentLocatorPart) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public String GetNodeId(DependencyObject d)
		{
			object [] parameters = new object[1];
			parameters[0] = d;
			String routedResult = (String) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}

		//------------------------------------------------------
		//
		//  Proxy Methods
		//
		//------------------------------------------------------

		public override string DelegateClassName()
		{
			return "MS.Internal.Annotations.Anchoring.DataIdProcessor";
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

		public static DependencyProperty DataIdProperty;
		public static DependencyProperty FetchAnnotationsAsBatchProperty;
		public static XmlQualifiedName DataIdElementName;
		public static XmlQualifiedName[] LocatorPartTypeNames;
		public static String Id = "Id";
		public static String ValueAttributeName = "Value";

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
