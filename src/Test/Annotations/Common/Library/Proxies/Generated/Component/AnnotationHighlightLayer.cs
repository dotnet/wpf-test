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
using System.Collections.Generic;
using System.Windows.Documents;
using Proxies.MS.Internal.Annotations.Component;
using System.Windows.Media;

namespace Proxies.MS.Internal.Annotations.Component
{
	public class AnnotationHighlightLayer : AReflectiveProxy
	{

		//------------------------------------------------------
		//
		//  Delegate Constructors
		//
		//------------------------------------------------------

		public AnnotationHighlightLayer()
		: base (Type.EmptyTypes, new object[0])
		{
			//Empty.
		}

		//------------------------------------------------------
		//
		//  Proxy Constructors
		//
		//------------------------------------------------------

		static AnnotationHighlightLayer() { InitializeStaticFields(static_DelegateAssembly, MethodBase.GetCurrentMethod().DeclaringType); }
		protected AnnotationHighlightLayer(Type[] types, object[] values) : base (types, values) { }
		protected AnnotationHighlightLayer(object delegateObject) : base (delegateObject) { }

		//------------------------------------------------------
		//
		//  Delegate Methods
		//
		//------------------------------------------------------


		//------------------------------------------------------
		//
		//  Proxy Methods
		//
		//------------------------------------------------------

		public override string DelegateClassName()
		{
			return "MS.Internal.Annotations.Component.AnnotationHighlightLayer";
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

		virtual public Type OwnerType
		{
			get
			{
				return (Type)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		public Boolean IsFixedContainer
		{
			get
			{
				return (Boolean)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
			set
			{
				RouteInstance(MethodBase.GetCurrentMethod(), new object[] { value });
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
        
		private List<HighlightSegment> _segments
		{
			get { return (List<HighlightSegment>) GetField("_segments"); }
			set { SetField("_segments", value); }
		}
        
		private Boolean _isFixedContainer
		{
			get { return (Boolean) GetField("_isFixedContainer"); }
			set { SetField("_isFixedContainer", value); }
		}
	}
	public class HighlightSegment : AReflectiveProxy
	{

		//------------------------------------------------------
		//
		//  Delegate Constructors
		//
		//------------------------------------------------------


		//------------------------------------------------------
		//
		//  Proxy Constructors
		//
		//------------------------------------------------------

		static HighlightSegment() { InitializeStaticFields(static_DelegateAssembly, MethodBase.GetCurrentMethod().DeclaringType); }
		protected HighlightSegment(Type[] types, object[] values) : base (types, values) { }
		protected HighlightSegment(object delegateObject) : base (delegateObject) { }

		//------------------------------------------------------
		//
		//  Delegate Methods
		//
		//------------------------------------------------------

		public void UpdateOwners()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void Discard()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}

		//------------------------------------------------------
		//
		//  Proxy Methods
		//
		//------------------------------------------------------

		public override string DelegateClassName()
		{
			return "MS.Internal.Annotations.Component.AnnotationHighlightLayer+HighlightSegment";
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

		virtual public Geometry DefiningGeometry
		{
			get
			{
				return (Geometry)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		public Brush OwnerColor
		{
			get
			{
				return (Brush)RouteInstance(MethodBase.GetCurrentMethod(), null);
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

	}
}
