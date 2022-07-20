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
	abstract public class SubTreeProcessor : AReflectiveProxy
	{

		//------------------------------------------------------
		//
		//  Delegate Constructors
		//
		//------------------------------------------------------

		public SubTreeProcessor(LocatorManager manager)
		: base (new Type[] { typeof(LocatorManager) }, new object[] { manager })
		{
			//Empty.
		}

		//------------------------------------------------------
		//
		//  Proxy Constructors
		//
		//------------------------------------------------------

		static SubTreeProcessor() { InitializeStaticFields(staticstatic_DelegateAssembly, MethodBase.GetCurrentMethod().DeclaringType); }
		protected SubTreeProcessor(Type[] types, object[] values) : base (types, values) { }
		protected SubTreeProcessor(object delegateObject) : base (delegateObject) { }

		//------------------------------------------------------
		//
		//  Delegate Methods
		//
		//------------------------------------------------------

		abstract public IList<IAttachedAnnotation> PreProcessNode(DependencyObject node, out Boolean calledProcessAnnotations);
		virtual public IList<IAttachedAnnotation> PostProcessNode(DependencyObject node, Boolean childrenCalledProcessAnnotations, out Boolean calledProcessAnnotations)
		{
			object [] parameters = new object[3];
			parameters[0] = node;
			parameters[1] = childrenCalledProcessAnnotations;
			IList<IAttachedAnnotation> routedResult = (IList<IAttachedAnnotation>) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			calledProcessAnnotations = (Boolean) parameters[2];
			return routedResult;
		}
		abstract public ContentLocator GenerateLocator(PathNode node, out Boolean continueGenerating);
		abstract public DependencyObject ResolveLocatorPart(ContentLocatorPart locatorPart, DependencyObject startNode, out Boolean continueResolving);
		abstract public XmlQualifiedName[] GetLocatorPartTypes();

		//------------------------------------------------------
		//
		//  Proxy Methods
		//
		//------------------------------------------------------

		public override string DelegateClassName()
		{
			return "MS.Internal.Annotations.Anchoring.SubTreeProcessor";
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

		public LocatorManager Manager
		{
			get
			{
				return (LocatorManager)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}

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

		//So that static methods can load the correct assembly.
        
        protected static string staticstatic_DelegateAssembly = "PresentationFramework";

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
        
		public LocatorManager locatorManager
		{
			get { return (LocatorManager) GetField("_manager"); }
			set { SetField("_manager", value); }
		}
	}
}
