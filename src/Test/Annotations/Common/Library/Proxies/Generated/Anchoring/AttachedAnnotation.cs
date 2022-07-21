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
using Proxies.System.Windows.Annotations;
using System;
using Proxies.MS.Internal.Annotations;
using System.Windows;
using System.Windows.Annotations.Storage;

namespace Proxies.MS.Internal.Annotations.Anchoring
{
	public class AttachedAnnotation : AReflectiveProxy, IAttachedAnnotation
	{

		//------------------------------------------------------
		//
		//  Delegate Constructors
		//
		//------------------------------------------------------

		public AttachedAnnotation(LocatorManager manager, Annotation annotation, AnnotationResource anchor, Object attachedAnchor, AttachmentLevel attachmentLevel)
		: base (new Type[] { typeof(LocatorManager), typeof(Annotation), typeof(AnnotationResource), typeof(Object), typeof(AttachmentLevel) }, new object[] { manager, annotation, anchor, attachedAnchor, attachmentLevel })
		{
			//Empty.
		}
		public AttachedAnnotation(LocatorManager manager, Annotation annotation, AnnotationResource anchor, Object attachedAnchor, AttachmentLevel attachmentLevel, DependencyObject parent)
		: base (new Type[] { typeof(LocatorManager), typeof(Annotation), typeof(AnnotationResource), typeof(Object), typeof(AttachmentLevel), typeof(DependencyObject) }, new object[] { manager, annotation, anchor, attachedAnchor, attachmentLevel, parent })
		{
			//Empty.
		}

		//------------------------------------------------------
		//
		//  Proxy Constructors
		//
		//------------------------------------------------------

		static AttachedAnnotation() { InitializeStaticFields(static_DelegateAssembly, MethodBase.GetCurrentMethod().DeclaringType); }
		protected AttachedAnnotation(Type[] types, object[] values) : base (types, values) { }
		protected AttachedAnnotation(object delegateObject) : base (delegateObject) { }

		//------------------------------------------------------
		//
		//  Delegate Methods
		//
		//------------------------------------------------------

		virtual public Boolean IsAnchorEqual(Object o)
		{
			object [] parameters = new object[1];
			parameters[0] = o;
			Boolean routedResult = (Boolean) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public void Update(Object attachedAnchor, AttachmentLevel attachmentLevel, DependencyObject parent)
		{
			object [] parameters = new object[3];
			parameters[0] = attachedAnchor;
			parameters[1] = attachmentLevel;
			parameters[2] = parent;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public AnnotationStore GetStore()
		{
			object [] parameters = new object[0];
			AnnotationStore routedResult = (AnnotationStore) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}

		//------------------------------------------------------
		//
		//  Proxy Methods
		//
		//------------------------------------------------------

		public override string DelegateClassName()
		{
			return "MS.Internal.Annotations.Anchoring.AttachedAnnotation";
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

		virtual public Annotation Annotation
		{
			get
			{
				return (Annotation)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		virtual public AnnotationResource Anchor
		{
			get
			{
				return (AnnotationResource)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		virtual public Object AttachedAnchor
		{
			get
			{
				return (Object)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		virtual public AttachmentLevel AttachmentLevel
		{
			get
			{
				return (AttachmentLevel)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		virtual public DependencyObject Parent
		{
			get
			{
				return (DependencyObject)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		virtual public Point AnchorPoint
		{
			get
			{
				return (Point)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		virtual public AnnotationStore Store
		{
			get
			{
				return (AnnotationStore)RouteInstance(MethodBase.GetCurrentMethod(), null);
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
        
		private Annotation _annotation
		{
			get { return (Annotation) GetField("_annotation"); }
			set { SetField("_annotation", value); }
		}
        
		private AnnotationResource _anchor
		{
			get { return (AnnotationResource) GetField("_anchor"); }
			set { SetField("_anchor", value); }
		}
        
		private Object _attachedAnchor
		{
			get { return (Object) GetField("_attachedAnchor"); }
			set { SetField("_attachedAnchor", value); }
		}
        
		private AttachmentLevel _attachmentLevel
		{
			get { return (AttachmentLevel) GetField("_attachmentLevel"); }
			set { SetField("_attachmentLevel", value); }
		}
        
		private DependencyObject _parent
		{
			get { return (DependencyObject) GetField("_parent"); }
			set { SetField("_parent", value); }
		}
        
		private SelectionProcessor _selectionProcessor
		{
			get { return (SelectionProcessor) GetField("_selectionProcessor"); }
			set { SetField("_selectionProcessor", value); }
		}
        
		private LocatorManager _locatorManager
		{
			get { return (LocatorManager) GetField("_locatorManager"); }
			set { SetField("_locatorManager", value); }
		}
	}
}
