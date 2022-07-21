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

namespace Proxies.MS.Internal.Annotations.Anchoring
{
	abstract public class SelectionProcessor : AReflectiveProxy
	{

		//------------------------------------------------------
		//
		//  Delegate Constructors
		//
		//------------------------------------------------------

		public SelectionProcessor()
		: base (Type.EmptyTypes, new object[0])
		{
			//Empty.
		}

		//------------------------------------------------------
		//
		//  Proxy Constructors
		//
		//------------------------------------------------------

		static SelectionProcessor() { InitializeStaticFields(staticstatic_DelegateAssembly, MethodBase.GetCurrentMethod().DeclaringType); }
		protected SelectionProcessor(Type[] types, object[] values) : base (types, values) { }
		protected SelectionProcessor(object delegateObject) : base (delegateObject) { }

		//------------------------------------------------------
		//
		//  Delegate Methods
		//
		//------------------------------------------------------

		abstract public Boolean MergeSelections(Object selection1, Object selection2, out Object newSelection);
		abstract public IList<DependencyObject> GetSelectedNodes(Object selection);
		abstract public UIElement GetParent(Object selection);
		abstract public Point GetAnchorPoint(Object selection);
		abstract public IList<ContentLocatorPart> GenerateLocatorParts(Object selection, DependencyObject startNode);
		abstract public Object ResolveLocatorPart(ContentLocatorPart locatorPart, DependencyObject startNode, out AttachmentLevel attachmentLevel);
		abstract public XmlQualifiedName[] GetLocatorPartTypes();

		//------------------------------------------------------
		//
		//  Proxy Methods
		//
		//------------------------------------------------------

		public override string DelegateClassName()
		{
			return "MS.Internal.Annotations.Anchoring.SelectionProcessor";
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

	}
}
