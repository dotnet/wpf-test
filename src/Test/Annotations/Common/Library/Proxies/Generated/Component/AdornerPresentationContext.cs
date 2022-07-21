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
using System.Windows.Documents;
using Proxies.MS.Internal.Annotations.Component;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Collections;

namespace Proxies.MS.Internal.Annotations.Component
{
	public class AdornerPresentationContext : PresentationContext
	{

		//------------------------------------------------------
		//
		//  Delegate Constructors
		//
		//------------------------------------------------------

		public AdornerPresentationContext(AdornerLayer adornerLayer, AnnotationAdorner adorner)
		: base (new Type[] { typeof(AdornerLayer), typeof(AnnotationAdorner) }, new object[] { adornerLayer, adorner })
		{
			//Empty.
		}
		public AdornerPresentationContext()
		: base (Type.EmptyTypes, new object[0])
		{
			//Empty.
		}

		//------------------------------------------------------
		//
		//  Proxy Constructors
		//
		//------------------------------------------------------

		static AdornerPresentationContext() { InitializeStaticFields(static_DelegateAssembly, MethodBase.GetCurrentMethod().DeclaringType); }
		protected AdornerPresentationContext(Type[] types, object[] values) : base (types, values) { }
		protected AdornerPresentationContext(object delegateObject) : base (delegateObject) { }

		//------------------------------------------------------
		//
		//  Delegate Methods
		//
		//------------------------------------------------------

		override public void AddToHost(IAnnotationComponent component)
		{
			object [] parameters = new object[1];
			parameters[0] = component;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		override public void RemoveFromHost(IAnnotationComponent component, Boolean reorder)
		{
			object [] parameters = new object[2];
			parameters[0] = component;
			parameters[1] = reorder;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		override public void InvalidateTransform(IAnnotationComponent component)
		{
			object [] parameters = new object[1];
			parameters[0] = component;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		override public void BringToFront(IAnnotationComponent component)
		{
			object [] parameters = new object[1];
			parameters[0] = component;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		override public void SendToBack(IAnnotationComponent component)
		{
			object [] parameters = new object[1];
			parameters[0] = component;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		static public Boolean op_Equality(AdornerPresentationContext left, AdornerPresentationContext right)
		{
			object [] parameters = new object[2];
			parameters[0] = left;
			parameters[1] = right;
			Boolean routedResult = (Boolean) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
			return routedResult;
		}
		static public Boolean op_Inequality(AdornerPresentationContext c1, AdornerPresentationContext c2)
		{
			object [] parameters = new object[2];
			parameters[0] = c1;
			parameters[1] = c2;
			Boolean routedResult = (Boolean) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
			return routedResult;
		}
		public void UpdateComponentZOrder(IAnnotationComponent component)
		{
			object [] parameters = new object[1];
			parameters[0] = component;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		static public void HostComponent(AdornerLayer adornerLayer, IAnnotationComponent component, UIElement annotatedElement, Boolean reorder)
		{
			object [] parameters = new object[4];
			parameters[0] = adornerLayer;
			parameters[1] = component;
			parameters[2] = annotatedElement;
			parameters[3] = reorder;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public void SetTypeZLevel(Type type, Int32 level)
		{
			object [] parameters = new object[2];
			parameters[0] = type;
			parameters[1] = level;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public void SetZLevelRange(Int32 level, Int32 min, Int32 max)
		{
			object [] parameters = new object[3];
			parameters[0] = level;
			parameters[1] = min;
			parameters[2] = max;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		public void ResetInternalAnnotationAdorner()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public Boolean IsInternalComponent(IAnnotationComponent component)
		{
			object [] parameters = new object[1];
			parameters[0] = component;
			Boolean routedResult = (Boolean) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public AnnotationAdorner FindAnnotationAdorner(IAnnotationComponent component)
		{
			object [] parameters = new object[1];
			parameters[0] = component;
			AnnotationAdorner routedResult = (AnnotationAdorner) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public List<AnnotationAdorner> GetTopAnnotationAdorners(Int32 level, IAnnotationComponent component)
		{
			object [] parameters = new object[2];
			parameters[0] = level;
			parameters[1] = component;
			List<AnnotationAdorner> routedResult = (List<AnnotationAdorner>) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public void AddAdorner(List<AnnotationAdorner> adorners, AnnotationAdorner adorner)
		{
			object [] parameters = new object[2];
			parameters[0] = adorners;
			parameters[1] = adorner;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		static public Int32 GetNextZOrder(AdornerLayer adornerLayer, Int32 level)
		{
			object [] parameters = new object[2];
			parameters[0] = adornerLayer;
			parameters[1] = level;
			Int32 routedResult = (Int32) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
			return routedResult;
		}
		public AnnotationAdorner GetAnnotationAdorner(IAnnotationComponent component)
		{
			object [] parameters = new object[1];
			parameters[0] = component;
			AnnotationAdorner routedResult = (AnnotationAdorner) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		static public Int32 GetComponentLevel(IAnnotationComponent component)
		{
			object [] parameters = new object[1];
			parameters[0] = component;
			Int32 routedResult = (Int32) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
			return routedResult;
		}
		static public Int32 ComponentToAdorner(Int32 zOrder, Int32 level)
		{
			object [] parameters = new object[2];
			parameters[0] = zOrder;
			parameters[1] = level;
			Int32 routedResult = (Int32) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
			return routedResult;
		}

		//------------------------------------------------------
		//
		//  Proxy Methods
		//
		//------------------------------------------------------

		public override string DelegateClassName()
		{
			return "MS.Internal.Annotations.Component.AdornerPresentationContext";
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

		override public UIElement Host
		{
			get
			{
				return (UIElement)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		override public PresentationContext EnclosingContext
		{
			get
			{
				return (PresentationContext)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}

		//------------------------------------------------------
		//
		//  Delegate Static Fields
		//
		//------------------------------------------------------
        
		public static Hashtable ZLevel;
        
		public static Hashtable ZRanges;

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
        
		private AnnotationAdorner _annotationAdorner
		{
			get { return (AnnotationAdorner) GetField("_annotationAdorner"); }
			set { SetField("_annotationAdorner", value); }
		}
        
		private AdornerLayer _adornerLayer
		{
			get { return (AdornerLayer) GetField("_adornerLayer"); }
			set { SetField("_adornerLayer", value); }
		}
	}
}
