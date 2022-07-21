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
using System.Windows.Controls.Primitives;
using System.Windows;
using System;
using System.Windows.Annotations.Storage;
using Proxies.MS.Internal.Annotations.Anchoring;
using Proxies.System.Windows.Annotations;
using Proxies.MS.Internal.Annotations;
using System.Collections.Generic;
using System.Collections;
using Proxies.MS.Internal.Annotations.Component;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows.Input;

namespace Proxies.System.Windows.Annotations
{
	public class AnnotationService : AReflectiveProxy
	{

		//------------------------------------------------------
		//
		//  Delegate Constructors
		//
		//------------------------------------------------------

		public AnnotationService()
		: base (Type.EmptyTypes, new object[0])
		{
			//Empty.
		}
		public AnnotationService(DocumentViewerBase viewer)
		: base (new Type[] { typeof(DocumentViewerBase) }, new object[] { viewer })
		{
			//Empty.
		}
		public AnnotationService(DependencyObject root)
		: base (new Type[] { typeof(DependencyObject) }, new object[] { root })
		{
			//Empty.
		}

		//------------------------------------------------------
		//
		//  Proxy Constructors
		//
		//------------------------------------------------------

		static AnnotationService() { InitializeStaticFields(static_DelegateAssembly, MethodBase.GetCurrentMethod().DeclaringType); }
		protected AnnotationService(Type[] types, object[] values) : base (types, values) { }
		protected AnnotationService(object delegateObject) : base (delegateObject) { }

		//------------------------------------------------------
		//
		//  Delegate Methods
		//
		//------------------------------------------------------

		public void Enable(AnnotationStore annotationStore)
		{
			object [] parameters = new object[1];
			parameters[0] = annotationStore;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void Disable()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		static public AnnotationService GetService(DocumentViewerBase viewer)
		{
			object [] parameters = new object[1];
			parameters[0] = viewer;
			AnnotationService routedResult = (AnnotationService) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
			return routedResult;
		}
		public void LoadAnnotations(DependencyObject element)
		{
			object [] parameters = new object[1];
			parameters[0] = element;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void UnloadAnnotations(DependencyObject element)
		{
			object [] parameters = new object[1];
			parameters[0] = element;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public IList<IAttachedAnnotation> GetAttachedAnnotations()
		{
			object [] parameters = new object[0];
			IList<IAttachedAnnotation> routedResult = (IList<IAttachedAnnotation>) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		static public AnnotationService GetService(DependencyObject d)
		{
			object [] parameters = new object[1];
			parameters[0] = d;
			AnnotationService routedResult = (AnnotationService) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
			return routedResult;
		}
		static public AnnotationComponentChooser GetChooser(DependencyObject d)
		{
			object [] parameters = new object[1];
			parameters[0] = d;
			AnnotationComponentChooser routedResult = (AnnotationComponentChooser) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
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
		static public void SetDataId(DependencyObject d, String id)
		{
			object [] parameters = new object[2];
			parameters[0] = d;
			parameters[1] = id;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public String GetDataId(DependencyObject d)
		{
			object [] parameters = new object[1];
			parameters[0] = d;
			String routedResult = (String) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
			return routedResult;
		}
		public void Initialize(DependencyObject root)
		{
			object [] parameters = new object[1];
			parameters[0] = root;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		static public Object LoadAnnotationsAsync(Object obj)
		{
			object [] parameters = new object[1];
			parameters[0] = obj;
			Object routedResult = (Object) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
			return routedResult;
		}
		static public void VerifyServiceConfiguration(DependencyObject root)
		{
			object [] parameters = new object[1];
			parameters[0] = root;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public Boolean VerifyNoServiceOnNode(DependencyObject node, Object data)
		{
			object [] parameters = new object[2];
			parameters[0] = node;
			parameters[1] = data;
			Boolean routedResult = (Boolean) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
			return routedResult;
		}
		public IAttachedAnnotation FindExistingAttachedAnnotation(IAttachedAnnotation attachedAnnotation)
		{
			object [] parameters = new object[1];
			parameters[0] = attachedAnnotation;
			IAttachedAnnotation routedResult = (IAttachedAnnotation) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public IList GetAllAttachedAnnotationsFor(DependencyObject element)
		{
			object [] parameters = new object[1];
			parameters[0] = element;
			IList routedResult = (IList) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public Boolean GetAttachedAnnotationsFor(DependencyObject node, List<IAttachedAnnotation> result)
		{
			object [] parameters = new object[2];
			parameters[0] = node;
			parameters[1] = result;
			Boolean routedResult = (Boolean) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public void OnStoreContentChanged(Object node, StoreContentChangedEventArgs args)
		{
			object [] parameters = new object[2];
			parameters[0] = node;
			parameters[1] = args;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void OnAnchorChanged(Object sender, AnnotationResourceChangedEventArgs args)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = args;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void AnnotationAdded(Annotation annotation)
		{
			object [] parameters = new object[1];
			parameters[0] = annotation;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void AnnotationDeleted(Guid annotationId)
		{
			object [] parameters = new object[1];
			parameters[0] = annotationId;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public AttachedAnnotationChangedEventArgs AnchorAdded(Annotation annotation, AnnotationResource anchor)
		{
			object [] parameters = new object[2];
			parameters[0] = annotation;
			parameters[1] = anchor;
			AttachedAnnotationChangedEventArgs routedResult = (AttachedAnnotationChangedEventArgs) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public AttachedAnnotationChangedEventArgs AnchorRemoved(Annotation annotation, AnnotationResource anchor)
		{
			object [] parameters = new object[2];
			parameters[0] = annotation;
			parameters[1] = anchor;
			AttachedAnnotationChangedEventArgs routedResult = (AttachedAnnotationChangedEventArgs) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public AttachedAnnotationChangedEventArgs AnchorModified(Annotation annotation, AnnotationResource anchor)
		{
			object [] parameters = new object[2];
			parameters[0] = annotation;
			parameters[1] = anchor;
			AttachedAnnotationChangedEventArgs routedResult = (AttachedAnnotationChangedEventArgs) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return routedResult;
		}
		public void DoAddAttachedAnnotation(IAttachedAnnotation attachedAnnotation)
		{
			object [] parameters = new object[1];
			parameters[0] = attachedAnnotation;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void DoRemoveAttachedAnnotation(IAttachedAnnotation attachedAnnotation)
		{
			object [] parameters = new object[1];
			parameters[0] = attachedAnnotation;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void FireEvents(List<AttachedAnnotationChangedEventArgs> eventsToFire)
		{
			object [] parameters = new object[1];
			parameters[0] = eventsToFire;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void RegisterOnDocumentViewer(DocumentViewerBase viewer)
		{
			object [] parameters = new object[1];
			parameters[0] = viewer;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void LoadAnnotationsForAllDPVs()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void UnloadAnnotationsForAllDPVs()
		{
			object [] parameters = new object[0];
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void UnregisterOnDocumentViewer(DocumentViewerBase viewer)
		{
			object [] parameters = new object[1];
			parameters[0] = viewer;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void OnPageViewsChanged(Object sender, EventArgs e)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = e;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void OnPageDisconnected(Object sender, EventArgs e)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = e;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void OnPageConnected(Object sender, EventArgs e)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = e;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}

		//------------------------------------------------------
		//
		//  Proxy Methods
		//
		//------------------------------------------------------

		public override string DelegateClassName()
		{
			return "System.Windows.Annotations.AnnotationService";
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

		public Boolean IsEnabled
		{
			get
			{
				return (Boolean)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		public AnnotationStore Store
		{
			get
			{
				return (AnnotationStore)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		public LocatorManager LocatorManager
		{
			get
			{
				return (LocatorManager)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
        public DependencyObject Root
        {
            get
            {
                return (DependencyObject)RouteInstance(MethodBase.GetCurrentMethod(), null);
            }
        }

		//------------------------------------------------------
		//
		//  Delegate Static Fields
		//
		//------------------------------------------------------

		public static RoutedUICommand CreateHighlightCommand;
		public static RoutedUICommand CreateTextStickyNoteCommand;
		public static RoutedUICommand CreateInkStickyNoteCommand;
		public static RoutedUICommand ClearHighlightsCommand;
		public static RoutedUICommand DeleteStickyNotesCommand;
		public static RoutedUICommand DeleteAnnotationsCommand;
		public static DependencyProperty ChooserProperty;
		public static DependencyProperty SubTreeProcessorIdProperty;
		public static DependencyProperty DataIdProperty;
		public static DependencyProperty ServiceProperty;
		public static DependencyProperty AttachedAnnotationsProperty;

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

		private event AttachedAnnotationChangedEventHandler Proxy_AttachedAnnotationChanged;
		private void Proxy_OnAttachedAnnotationChanged(object sender, AttachedAnnotationChangedEventArgs args)
		{
			if (Proxy_AttachedAnnotationChanged != null)
				Proxy_AttachedAnnotationChanged(sender, (AttachedAnnotationChangedEventArgs)ProxyTypeConverter.WrapObject(null, args));
		}
		public event AttachedAnnotationChangedEventHandler AttachedAnnotationChanged
		{
			add
			{
				Proxy_AttachedAnnotationChanged += value;
				RouteEventMethod(MethodBase.GetCurrentMethod(), new AttachedAnnotationChangedEventHandler(Proxy_OnAttachedAnnotationChanged));
			}
			remove
			{
				Proxy_AttachedAnnotationChanged -= value;
				RouteEventMethod(MethodBase.GetCurrentMethod(), new AttachedAnnotationChangedEventHandler(Proxy_OnAttachedAnnotationChanged));
			}
		}

		//------------------------------------------------------
		//
		//  Delegate Non-Static Fields (as properties)
		//
		//------------------------------------------------------
        
		private DependencyObject _root
		{
			get { return (DependencyObject) GetField("_root"); }
			set { SetField("_root", value); }
		}
        
		public AnnotationComponentManager AnnotationComponentManager
		{
			get { return (AnnotationComponentManager) GetField("_annotationComponentManager"); }
			set { SetField("_annotationComponentManager", value); }
		}
        
		private LocatorManager _locatorManager
		{
			get { return (LocatorManager) GetField("_locatorManager"); }
			set { SetField("_locatorManager", value); }
		}
        
		private Boolean _isEnabled
		{
			get { return (Boolean) GetField("_isEnabled"); }
			set { SetField("_isEnabled", value); }
		}
        
		private AnnotationStore _store
		{
			get { return (AnnotationStore) GetField("_store"); }
			set { SetField("_store", value); }
		}
        
		private Collection<DocumentPageView> _views
		{
			get { return (Collection<DocumentPageView>) GetField("_views"); }
			set { SetField("_views", value); }
		}
        
		private DispatcherOperation _asyncLoadOperation
		{
			get { return (DispatcherOperation) GetField("_asyncLoadOperation"); }
			set { SetField("_asyncLoadOperation", value); }
		}
	}
}
