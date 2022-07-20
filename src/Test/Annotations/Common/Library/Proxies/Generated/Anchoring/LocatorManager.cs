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
using System.Windows.Annotations.Storage;
using System;
using Proxies.MS.Internal.Annotations.Anchoring;
using System.Windows;
using Proxies.System.Windows.Annotations;
using System.Collections.Generic;
using Proxies.MS.Internal.Annotations;
using System.Collections;

namespace Proxies.MS.Internal.Annotations.Anchoring
{
	public class LocatorManager : AReflectiveProxy
	{

		//------------------------------------------------------
		//
		//  Delegate Constructors
		//
		//------------------------------------------------------

		public LocatorManager()
		: base (Type.EmptyTypes, new object[0])
		{
			//Empty.
		}
		public LocatorManager(AnnotationStore store)
		: base (new Type[] { typeof(AnnotationStore) }, new object[] { store })
		{
			//Empty.
		}

		//------------------------------------------------------
		//
		//  Proxy Constructors
		//
		//------------------------------------------------------

		static LocatorManager() { InitializeStaticFields(static_DelegateAssembly, MethodBase.GetCurrentMethod().DeclaringType); }
		protected LocatorManager(Type[] types, object[] values) : base (types, values) { }
		protected LocatorManager(object delegateObject) : base (delegateObject) { }

		//------------------------------------------------------
		//
		//  Delegate Methods
		//
		//------------------------------------------------------

		public void RegisterSubTreeProcessor(SubTreeProcessor processor, String processorId)
		{
			object [] parameters = new object[2];
			parameters[0] = processor;
			parameters[1] = processorId;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public SubTreeProcessor GetSubTreeProcessor(DependencyObject node)
		{
			object [] parameters = new object[1];
			parameters[0] = node;
			SubTreeProcessor routedResult = (SubTreeProcessor) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public SubTreeProcessor GetSubTreeProcessorForLocatorPart(ContentLocatorPart locatorPart)
		{
			object [] parameters = new object[1];
			parameters[0] = locatorPart;
			SubTreeProcessor routedResult = (SubTreeProcessor) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public void RegisterSelectionProcessor(SelectionProcessor processor, Type selectionType)
		{
			object [] parameters = new object[2];
			parameters[0] = processor;
			parameters[1] = selectionType;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public SelectionProcessor GetSelectionProcessor(Type selectionType)
		{
			object [] parameters = new object[1];
			parameters[0] = selectionType;
			SelectionProcessor routedResult = (SelectionProcessor) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public SelectionProcessor GetSelectionProcessorForLocatorPart(ContentLocatorPart locatorPart)
		{
			object [] parameters = new object[1];
			parameters[0] = locatorPart;
			SelectionProcessor routedResult = (SelectionProcessor) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public IList<IAttachedAnnotation> ProcessAnnotations(DependencyObject node)
		{
			object [] parameters = new object[1];
			parameters[0] = node;
			IList<IAttachedAnnotation> routedResult = (IList<IAttachedAnnotation>) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public IList<ContentLocatorBase> GenerateLocators(Object selection)
		{
			object [] parameters = new object[1];
			parameters[0] = selection;
			IList<ContentLocatorBase> routedResult = (IList<ContentLocatorBase>) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public Object ResolveLocator(ContentLocatorBase locator, Int32 offset, DependencyObject startNode, out AttachmentLevel attachmentLevel)
		{
			object [] parameters = new object[4];
			parameters[0] = locator;
			parameters[1] = offset;
			parameters[2] = startNode;
			Object routedResult = (Object) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			attachmentLevel = (AttachmentLevel) parameters[3];
			return routedResult;
		}
		static public void SetSubTreeProcessorId(DependencyObject d, String id)
		{
			object [] parameters = new object[2];
			parameters[0] = d;
			parameters[1] = id;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public String GetSubTreeProcessorId(DependencyObject d)
		{
			object [] parameters = new object[1];
			parameters[0] = d;
			String routedResult = (String) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
			return routedResult;
		}
		public IList<IAttachedAnnotation> ProcessSubTree(DependencyObject subTree)
		{
			object [] parameters = new object[1];
			parameters[0] = subTree;
			IList<IAttachedAnnotation> routedResult = (IList<IAttachedAnnotation>) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public IList<ContentLocatorBase> GenerateLocators(SubTreeProcessor processor, PathNode startNode, Object selection)
		{
			object [] parameters = new object[3];
			parameters[0] = processor;
			parameters[1] = startNode;
			parameters[2] = selection;
			IList<ContentLocatorBase> routedResult = (IList<ContentLocatorBase>) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public ContentLocatorBase GenerateLocatorGroup(PathNode node, Object selection)
		{
			object [] parameters = new object[2];
			parameters[0] = node;
			parameters[1] = selection;
			ContentLocatorBase routedResult = (ContentLocatorBase) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public IList<ContentLocatorBase> Merge(ContentLocatorBase initialLocator, IList<ContentLocatorBase> additionalLocators)
		{
			object [] parameters = new object[2];
			parameters[0] = initialLocator;
			parameters[1] = additionalLocators;
			IList<ContentLocatorBase> routedResult = (IList<ContentLocatorBase>) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}

		//------------------------------------------------------
		//
		//  Proxy Methods
		//
		//------------------------------------------------------

		public override string DelegateClassName()
		{
			return "MS.Internal.Annotations.Anchoring.LocatorManager";
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

		public static DependencyProperty SubTreeProcessorIdProperty;

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
        
		private Hashtable _locatorPartHandlers
		{
			get { return (Hashtable) GetField("_locatorPartHandlers"); }
			set { SetField("_locatorPartHandlers", value); }
		}
        
		private Hashtable _subtreeProcessors
		{
			get { return (Hashtable) GetField("_subtreeProcessors"); }
			set { SetField("_subtreeProcessors", value); }
		}
        
		private Hashtable _selectionProcessors
		{
			get { return (Hashtable) GetField("_selectionProcessors"); }
			set { SetField("_selectionProcessors", value); }
		}
		private Char[] Separators
		{
			get { return (Char[]) GetField("Separators"); }
			set { SetField("Separators", value); }
		}
        
		private AnnotationStore _internalStore
		{
			get { return (AnnotationStore) GetField("_internalStore"); }
			set { SetField("_internalStore", value); }
		}
	}
}
