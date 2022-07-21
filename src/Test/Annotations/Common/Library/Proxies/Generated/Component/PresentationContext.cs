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
using System.Windows;
using Proxies.MS.Internal.Annotations.Component;
using System;

namespace Proxies.MS.Internal.Annotations.Component
{
	abstract public class PresentationContext : AReflectiveProxy
	{

		//------------------------------------------------------
		//
		//  Delegate Constructors
		//
		//------------------------------------------------------

		public PresentationContext()
		: base (Type.EmptyTypes, new object[0])
		{
			//Empty.
		}

		//------------------------------------------------------
		//
		//  Proxy Constructors
		//
		//------------------------------------------------------

		static PresentationContext() { InitializeStaticFields(static_DelegateAssembly, MethodBase.GetCurrentMethod().DeclaringType); }
		protected PresentationContext(Type[] types, object[] values) : base (types, values) { }
		protected PresentationContext(object delegateObject) : base (delegateObject) { }

		//------------------------------------------------------
		//
		//  Delegate Methods
		//
		//------------------------------------------------------

		abstract public void AddToHost(IAnnotationComponent component);
		abstract public void RemoveFromHost(IAnnotationComponent component, Boolean reorder);
		abstract public void InvalidateTransform(IAnnotationComponent component);
		abstract public void BringToFront(IAnnotationComponent component);
		abstract public void SendToBack(IAnnotationComponent component);

		//------------------------------------------------------
		//
		//  Proxy Methods
		//
		//------------------------------------------------------

		public override string DelegateClassName()
		{
			return "MS.Internal.Annotations.Component.PresentationContext";
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

		abstract public UIElement Host
		{
			get;
		}
		abstract public PresentationContext EnclosingContext
		{
			get;
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
        
        protected static string static_DelegateAssembly = "PresentationFramework";

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
