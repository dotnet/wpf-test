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
using System;
using Proxies.MS.Internal.Annotations;

namespace Proxies.MS.Internal.Annotations
{
	public enum AttachedAnnotationAction
	{
		Loaded,
		Unloaded,
		AnchorModified,
		Added,
		Deleted
	}
	public class AttachedAnnotationChangedEventArgs : AReflectiveProxy
	{

		//------------------------------------------------------
		//
		//  Delegate Constructors
		//
		//------------------------------------------------------

		public AttachedAnnotationChangedEventArgs(AttachedAnnotationAction action, IAttachedAnnotation attachedAnnotation, Object previousAttachedAnchor, AttachmentLevel previousAttachmentLevel)
		: base (new Type[] { typeof(AttachedAnnotationAction), typeof(IAttachedAnnotation), typeof(Object), typeof(AttachmentLevel) }, new object[] { action, attachedAnnotation, previousAttachedAnchor, previousAttachmentLevel })
		{
			//Empty.
		}

		//------------------------------------------------------
		//
		//  Proxy Constructors
		//
		//------------------------------------------------------

		static AttachedAnnotationChangedEventArgs() { InitializeStaticFields(static_DelegateAssembly, MethodBase.GetCurrentMethod().DeclaringType); }
		protected AttachedAnnotationChangedEventArgs(Type[] types, object[] values) : base (types, values) { }
		protected AttachedAnnotationChangedEventArgs(object delegateObject) : base (delegateObject) { }

		//------------------------------------------------------
		//
		//  Delegate Methods
		//
		//------------------------------------------------------

		static public AttachedAnnotationChangedEventArgs Added(IAttachedAnnotation attachedAnnotation)
		{
			object [] parameters = new object[1];
			parameters[0] = attachedAnnotation;
			AttachedAnnotationChangedEventArgs routedResult = (AttachedAnnotationChangedEventArgs) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
			return routedResult;
		}
		static public AttachedAnnotationChangedEventArgs Loaded(IAttachedAnnotation attachedAnnotation)
		{
			object [] parameters = new object[1];
			parameters[0] = attachedAnnotation;
			AttachedAnnotationChangedEventArgs routedResult = (AttachedAnnotationChangedEventArgs) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
			return routedResult;
		}
		static public AttachedAnnotationChangedEventArgs Deleted(IAttachedAnnotation attachedAnnotation)
		{
			object [] parameters = new object[1];
			parameters[0] = attachedAnnotation;
			AttachedAnnotationChangedEventArgs routedResult = (AttachedAnnotationChangedEventArgs) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
			return routedResult;
		}
		static public AttachedAnnotationChangedEventArgs Unloaded(IAttachedAnnotation attachedAnnotation)
		{
			object [] parameters = new object[1];
			parameters[0] = attachedAnnotation;
			AttachedAnnotationChangedEventArgs routedResult = (AttachedAnnotationChangedEventArgs) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
			return routedResult;
		}
		static public AttachedAnnotationChangedEventArgs Modified(IAttachedAnnotation attachedAnnotation, Object previousAttachedAnchor, AttachmentLevel previousAttachmentLevel)
		{
			object [] parameters = new object[3];
			parameters[0] = attachedAnnotation;
			parameters[1] = previousAttachedAnchor;
			parameters[2] = previousAttachmentLevel;
			AttachedAnnotationChangedEventArgs routedResult = (AttachedAnnotationChangedEventArgs) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
			return routedResult;
		}

		//------------------------------------------------------
		//
		//  Proxy Methods
		//
		//------------------------------------------------------

		public override string DelegateClassName()
		{
			return "MS.Internal.Annotations.AttachedAnnotationChangedEventArgs";
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

		public AttachedAnnotationAction Action
		{
			get
			{
				return (AttachedAnnotationAction)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		public IAttachedAnnotation AttachedAnnotation
		{
			get
			{
				return (IAttachedAnnotation)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		public Object PreviousAttachedAnchor
		{
			get
			{
				return (Object)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		public AttachmentLevel PreviousAttachmentLevel
		{
			get
			{
				return (AttachmentLevel)RouteInstance(MethodBase.GetCurrentMethod(), null);
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
        
		private AttachedAnnotationAction _action
		{
			get { return (AttachedAnnotationAction) GetField("_action"); }
			set { SetField("_action", value); }
		}
        
		private IAttachedAnnotation _attachedAnnotation
		{
			get { return (IAttachedAnnotation) GetField("_attachedAnnotation"); }
			set { SetField("_attachedAnnotation", value); }
		}
        
		private Object _previousAttachedAnchor
		{
			get { return (Object) GetField("_previousAttachedAnchor"); }
			set { SetField("_previousAttachedAnchor", value); }
		}
        
		private AttachmentLevel _previousAttachmentLevel
		{
			get { return (AttachmentLevel) GetField("_previousAttachmentLevel"); }
			set { SetField("_previousAttachmentLevel", value); }
		}
	}
    public delegate void AttachedAnnotationChangedEventHandler(object sender, AttachedAnnotationChangedEventArgs e);
}
