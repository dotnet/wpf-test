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
using Proxies.System.Windows.Annotations;
using System;
using Proxies.MS.Internal.Annotations;
using Proxies.MS.Internal.Annotations.Component;
using System.Collections.Generic;

namespace Proxies.MS.Internal.Annotations.Component
{
	public class AnnotationComponentManager : AReflectiveProxy
	{

		//------------------------------------------------------
		//
		//  Delegate Constructors
		//
		//------------------------------------------------------

		public AnnotationComponentManager(AnnotationService service)
		: base (new Type[] { typeof(AnnotationService) }, new object[] { service })
		{
			//Empty.
		}

		//------------------------------------------------------
		//
		//  Proxy Constructors
		//
		//------------------------------------------------------

		static AnnotationComponentManager() { InitializeStaticFields(static_DelegateAssembly, MethodBase.GetCurrentMethod().DeclaringType); }
		protected AnnotationComponentManager(Type[] types, object[] values) : base (types, values) { }
		protected AnnotationComponentManager(object delegateObject) : base (delegateObject) { }

		//------------------------------------------------------
		//
		//  Delegate Methods
		//
		//------------------------------------------------------

		public void AddAttachedAnnotation(IAttachedAnnotation attachedAnnotation, Boolean reorder)
		{
			object [] parameters = new object[2];
			parameters[0] = attachedAnnotation;
			parameters[1] = reorder;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void RemoveAttachedAnnotation(IAttachedAnnotation attachedAnnotation, Boolean reorder)
		{
			object [] parameters = new object[2];
			parameters[0] = attachedAnnotation;
			parameters[1] = reorder;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void AttachedAnnotationUpdateEventHandler(Object sender, AttachedAnnotationChangedEventArgs e)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = e;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public IAnnotationComponent FindComponent(IAttachedAnnotation attachedAnnotation)
		{
			object [] parameters = new object[1];
			parameters[0] = attachedAnnotation;
			IAnnotationComponent routedResult = (IAnnotationComponent) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public void AddComponent(IAttachedAnnotation attachedAnnotation, IAnnotationComponent component, Boolean reorder)
		{
			object [] parameters = new object[3];
			parameters[0] = attachedAnnotation;
			parameters[1] = component;
			parameters[2] = reorder;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void ModifyAttachedAnnotation(IAttachedAnnotation attachedAnnotation, Object previousAttachedAnchor, AttachmentLevel previousAttachmentLevel)
		{
			object [] parameters = new object[3];
			parameters[0] = attachedAnnotation;
			parameters[1] = previousAttachedAnchor;
			parameters[2] = previousAttachmentLevel;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void AddToAttachedAnnotations(IAttachedAnnotation attachedAnnotation, IAnnotationComponent component)
		{
			object [] parameters = new object[2];
			parameters[0] = attachedAnnotation;
			parameters[1] = component;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}

		//------------------------------------------------------
		//
		//  Proxy Methods
		//
		//------------------------------------------------------

		public override string DelegateClassName()
		{
			return "MS.Internal.Annotations.Component.AnnotationComponentManager";
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
        
		public Dictionary<IAttachedAnnotation, IList<IAnnotationComponent>> attachedAnnotations
		{
			get { return (Dictionary<IAttachedAnnotation, IList<IAnnotationComponent>>) GetField("_attachedAnnotations"); }
			set { SetField("_attachedAnnotations", value); }
		}
	}
}
