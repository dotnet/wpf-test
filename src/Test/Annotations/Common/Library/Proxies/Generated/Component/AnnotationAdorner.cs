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
using Proxies.MS.Internal.Annotations.Component;
using System.Windows;
using System;
using System.Windows.Media;

namespace Proxies.MS.Internal.Annotations.Component
{
	public class AnnotationAdorner : AReflectiveProxy
	{

		//------------------------------------------------------
		//
		//  Delegate Constructors
		//
		//------------------------------------------------------

		public AnnotationAdorner(IAnnotationComponent component, UIElement annotatedElement)
		: base (new Type[] { typeof(IAnnotationComponent), typeof(UIElement) }, new object[] { component, annotatedElement })
		{
			//Empty.
		}

		//------------------------------------------------------
		//
		//  Proxy Constructors
		//
		//------------------------------------------------------

		static AnnotationAdorner() { InitializeStaticFields(static_DelegateAssembly, MethodBase.GetCurrentMethod().DeclaringType); }
		protected AnnotationAdorner(Type[] types, object[] values) : base (types, values) { }
		protected AnnotationAdorner(object delegateObject) : base (delegateObject) { }

		//------------------------------------------------------
		//
		//  Delegate Methods
		//
		//------------------------------------------------------

		virtual public GeneralTransform GetDesiredTransform(GeneralTransform transform)
		{
			object [] parameters = new object[1];
			parameters[0] = transform;
			GeneralTransform routedResult = (GeneralTransform) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		virtual public Visual GetVisualChild(Int32 index)
		{
			object [] parameters = new object[1];
			parameters[0] = index;
			Visual routedResult = (Visual) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		virtual public Size MeasureOverride(Size availableSize)
		{
			object [] parameters = new object[1];
			parameters[0] = availableSize;
			Size routedResult = (Size) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		virtual public Size ArrangeOverride(Size finalSize)
		{
			object [] parameters = new object[1];
			parameters[0] = finalSize;
			Size routedResult = (Size) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public void RemoveChildren()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void InvalidateTransform()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void OnLayoutUpdated(Object sender, EventArgs args)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = args;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}

		//------------------------------------------------------
		//
		//  Proxy Methods
		//
		//------------------------------------------------------

		public override string DelegateClassName()
		{
			return "MS.Internal.Annotations.Component.AnnotationAdorner";
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

		virtual public Int32 VisualChildrenCount
		{
			get
			{
				return (Int32)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		public IAnnotationComponent AnnotationComponent
		{
			get
			{
				return (IAnnotationComponent)RouteInstance(MethodBase.GetCurrentMethod(), null);
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
        
		private IAnnotationComponent _annotationComponent
		{
			get { return (IAnnotationComponent) GetField("_annotationComponent"); }
			set { SetField("_annotationComponent", value); }
		}

        private UIElement _annotatedElement
		{
			get { return (UIElement) GetField("_annotatedElement"); }
			set { SetField("_annotatedElement", value); }
		}
	}
}
