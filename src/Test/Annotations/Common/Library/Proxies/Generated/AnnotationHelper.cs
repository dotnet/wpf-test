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
using Proxies.System.Windows.Annotations;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Xml;
using System.Collections.Generic;
using Proxies.MS.Internal.Annotations;

namespace Proxies.System.Windows.Annotations
{
	abstract public class AnnotationHelper : AReflectiveProxy
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

		static AnnotationHelper() { InitializeStaticFields(static_DelegateAssembly, MethodBase.GetCurrentMethod().DeclaringType); }
		protected AnnotationHelper(Type[] types, object[] values) : base (types, values) { }
		protected AnnotationHelper(object delegateObject) : base (delegateObject) { }

		//------------------------------------------------------
		//
		//  Delegate Methods
		//
		//------------------------------------------------------

		static public void CreateHighlightForSelection(AnnotationService service, String author, Brush highlightBrush)
		{
			object [] parameters = new object[3];
			parameters[0] = service;
			parameters[1] = author;
			parameters[2] = highlightBrush;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public void CreateTextStickyNoteForSelection(AnnotationService service, String author)
		{
			object [] parameters = new object[2];
			parameters[0] = service;
			parameters[1] = author;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public void CreateInkStickyNoteForSelection(AnnotationService service, String author)
		{
			object [] parameters = new object[2];
			parameters[0] = service;
			parameters[1] = author;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public void ClearHighlightsForSelection(AnnotationService service)
		{
			object [] parameters = new object[1];
			parameters[0] = service;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public void DeleteTextStickyNotesForSelection(AnnotationService service)
		{
			object [] parameters = new object[1];
			parameters[0] = service;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public void DeleteInkStickyNotesForSelection(AnnotationService service)
		{
			object [] parameters = new object[1];
			parameters[0] = service;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public void OnCreateHighlightCommand(Object sender, ExecutedRoutedEventArgs e)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = e;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public void OnCreateTextStickyNoteCommand(Object sender, ExecutedRoutedEventArgs e)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = e;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public void OnCreateInkStickyNoteCommand(Object sender, ExecutedRoutedEventArgs e)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = e;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public void OnClearHighlightsCommand(Object sender, ExecutedRoutedEventArgs e)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = e;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public void OnDeleteStickyNotesCommand(Object sender, ExecutedRoutedEventArgs e)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = e;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public void OnDeleteAnnotationsCommand(Object sender, ExecutedRoutedEventArgs e)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = e;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public void OnQueryCreateHighlightCommand(Object sender, CanExecuteRoutedEventArgs e)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = e;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public void OnQueryCreateTextStickyNoteCommand(Object sender, CanExecuteRoutedEventArgs e)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = e;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public void OnQueryCreateInkStickyNoteCommand(Object sender, CanExecuteRoutedEventArgs e)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = e;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public void OnQueryClearHighlightsCommand(Object sender, CanExecuteRoutedEventArgs e)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = e;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public void OnQueryDeleteStickyNotesCommand(Object sender, CanExecuteRoutedEventArgs e)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = e;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public void OnQueryDeleteAnnotationsCommand(Object sender, CanExecuteRoutedEventArgs e)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = e;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public DocumentPageView FindView(DocumentViewerBase viewer, Int32 pageNb)
		{
			object [] parameters = new object[2];
			parameters[0] = viewer;
			parameters[1] = pageNb;
			DocumentPageView routedResult = (DocumentPageView) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
			return routedResult;
		}
		static public void CreateStickyNoteForSelection(AnnotationService service, XmlQualifiedName noteType, String author)
		{
			object [] parameters = new object[3];
			parameters[0] = service;
			parameters[1] = noteType;
			parameters[2] = author;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public Boolean AreAllPagesVisibile(AnnotationService service, Int32 startPage, Int32 endPage)
		{
			object [] parameters = new object[3];
			parameters[0] = service;
			parameters[1] = startPage;
			parameters[2] = endPage;
			Boolean routedResult = (Boolean) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
			return routedResult;
		}
		static public IList<IAttachedAnnotation> GetSpannedAnnotations(AnnotationService service)
		{
			object [] parameters = new object[1];
			parameters[0] = service;
			IList<IAttachedAnnotation> routedResult = (IList<IAttachedAnnotation>) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
			return routedResult;
		}
		static public void AddRange(List<Annotation> annotations, IList<Annotation> newAnnotations)
		{
			object [] parameters = new object[2];
			parameters[0] = annotations;
			parameters[1] = newAnnotations;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public List<IAttachedAnnotation> ResolveAnnotations(AnnotationService service, IList<Annotation> annotations)
		{
			object [] parameters = new object[2];
			parameters[0] = service;
			parameters[1] = annotations;
			List<IAttachedAnnotation> routedResult = (List<IAttachedAnnotation>) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
			return routedResult;
		}
		static public void DeleteSpannedAnnotations(AnnotationService service, XmlQualifiedName annotationType)
		{
			object [] parameters = new object[2];
			parameters[0] = service;
			parameters[1] = annotationType;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public void InsertAttachedAnnotation(List<IAttachedAnnotation> annotations, IAttachedAnnotation attachedAnnot)
		{
			object [] parameters = new object[2];
			parameters[0] = annotations;
			parameters[1] = attachedAnnot;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public void Highlight(AnnotationService service, String author, Brush highlightBrush, Boolean create)
		{
			object [] parameters = new object[4];
			parameters[0] = service;
			parameters[1] = author;
			parameters[2] = highlightBrush;
			parameters[3] = create;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public Boolean CheckHighlightColor(Annotation annotation, Nullable<Color> color)
		{
			object [] parameters = new object[2];
			parameters[0] = annotation;
			parameters[1] = color;
			Boolean routedResult = (Boolean) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
			return routedResult;
		}
		static public void CheckInputs(AnnotationService service)
		{
			object [] parameters = new object[1];
			parameters[0] = service;
			AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
		}
		static public Boolean IsCommandEnabled(Object sender, Boolean checkForEmpty)
		{
			object [] parameters = new object[2];
			parameters[0] = sender;
			parameters[1] = checkForEmpty;
			Boolean routedResult = (Boolean) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
			return routedResult;
		}

		//------------------------------------------------------
		//
		//  Proxy Methods
		//
		//------------------------------------------------------

		public override string DelegateClassName()
		{
			return "System.Windows.Annotations.AnnotationHelper";
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

	}
}
