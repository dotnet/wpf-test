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
using System.Collections;
using System;
using Proxies.MS.Internal.Annotations.Anchoring;

namespace Proxies.MS.Internal.Annotations.Anchoring
{
	public class PathNode : AReflectiveProxy
	{

		//------------------------------------------------------
		//
		//  Delegate Constructors
		//
		//------------------------------------------------------

		public PathNode(DependencyObject node)
		: base (new Type[] { typeof(DependencyObject) }, new object[] { node })
		{
			//Empty.
		}
		public PathNode()
		: base (Type.EmptyTypes, new object[0])
		{
			//Empty.
		}

		//------------------------------------------------------
		//
		//  Proxy Constructors
		//
		//------------------------------------------------------

		static PathNode() { InitializeStaticFields(static_DelegateAssembly, MethodBase.GetCurrentMethod().DeclaringType); }
		protected PathNode(Type[] types, object[] values) : base (types, values) { }
		protected PathNode(object delegateObject) : base (delegateObject) { }

		//------------------------------------------------------
		//
		//  Delegate Methods
		//
		//------------------------------------------------------

		public new Boolean Equals(Object obj)
		{
			object [] parameters = new object[1];
			parameters[0] = obj;
			Boolean routedResult = (Boolean) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public new Int32 GetHashCode()
		{
			object [] parameters = new object[0];
			Int32 routedResult = (Int32) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		static public PathNode BuildPathForElements(ICollection nodes)
		{
			object [] parameters = new object[1];
			parameters[0] = nodes;
			PathNode routedResult = (PathNode) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
			return routedResult;
		}
		static public DependencyObject GetParent(DependencyObject node)
		{
			object [] parameters = new object[1];
			parameters[0] = node;
			DependencyObject routedResult = (DependencyObject) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
			return routedResult;
		}
		static public PathNode BuildPathForElement(DependencyObject node)
		{
			object [] parameters = new object[1];
			parameters[0] = node;
			PathNode routedResult = (PathNode) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
			return routedResult;
		}
		static public PathNode AddBranchToPath(PathNode path, PathNode branch)
		{
			object [] parameters = new object[2];
			parameters[0] = path;
			parameters[1] = branch;
			PathNode routedResult = (PathNode) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
			return routedResult;
		}
		public void AddChild(Object child)
		{
			object [] parameters = new object[1];
			parameters[0] = child;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void FreezeChildren()
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
			return "MS.Internal.Annotations.Anchoring.PathNode";
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

		public DependencyObject Node
		{
			get
			{
				return (DependencyObject)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		public IList Children
		{
			get
			{
				return (IList)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}

		//------------------------------------------------------
		//
		//  Delegate Static Fields
		//
		//------------------------------------------------------

		public static DependencyProperty HiddenParentProperty;

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
        
		private DependencyObject _node
		{
			get { return (DependencyObject) GetField("_node"); }
			set { SetField("_node", value); }
		}
        
		private ArrayList _children
		{
			get { return (ArrayList) GetField("_children"); }
			set { SetField("_children", value); }
		}
	}
}
