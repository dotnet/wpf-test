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
using System.Windows;
using System.Windows.Controls;
using System;

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
using Proxies.MS.Internal.Annotations.Component;
using Proxies.MS.Internal.Annotations;
using Proxies.System.Windows.Annotations;

namespace Proxies.System.Windows.Annotations
{
	public class AnnotationComponentChooser : AReflectiveProxy
	{

		//------------------------------------------------------
		//
		//  Delegate Constructors
		//
		//------------------------------------------------------

		public AnnotationComponentChooser()
		: base (Type.EmptyTypes, new object[0])
		{
			//Empty.
		}

		//------------------------------------------------------
		//
		//  Proxy Constructors
		//
		//------------------------------------------------------

        static AnnotationComponentChooser() { InitializeStaticFields(staticstatic_DelegateAssembly, MethodBase.GetCurrentMethod().DeclaringType); }
		protected AnnotationComponentChooser(Type[] types, object[] values) : base (types, values) { }
		protected AnnotationComponentChooser(object delegateObject) : base (delegateObject) { }

		//------------------------------------------------------
		//
		//  Delegate Methods
		//
		//------------------------------------------------------

		virtual public IAnnotationComponent ChooseAnnotationComponent(IAttachedAnnotation attachedAnnotation)
		{
			object [] parameters = new object[1];
			parameters[0] = attachedAnnotation;
			IAnnotationComponent routedResult = (IAnnotationComponent) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}

		//------------------------------------------------------
		//
		//  Proxy Methods
		//
		//------------------------------------------------------

		public override string DelegateClassName()
		{
			return "System.Windows.Annotations.AnnotationComponentChooser";
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

		public static AnnotationComponentChooser None;

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
