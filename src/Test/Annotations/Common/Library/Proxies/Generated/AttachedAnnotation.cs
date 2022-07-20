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
using System;
using Proxies.MS.Internal.Annotations;
using Proxies.System.Windows.Annotations;
using System.Windows;
using System.Windows.Annotations.Storage;

namespace Proxies.MS.Internal.Annotations
{
	public enum AttachmentLevel
	{
		Full,
		StartPortion,
		MiddlePortion,
		EndPortion,
		Incomplete,
		Unresolved
	}
	public interface IAttachedAnnotation : IReflectiveProxy
	{

		//------------------------------------------------------
		//
		//  Delegate Methods
		//
		//------------------------------------------------------

		Boolean IsAnchorEqual(Object o);

		//------------------------------------------------------
		//
		//  Properties
		//
		//------------------------------------------------------

		Annotation Annotation
		{
			get;
		}
		AnnotationResource Anchor
		{
			get;
		}
		Object AttachedAnchor
		{
			get;
		}
		AttachmentLevel AttachmentLevel
		{
			get;
		}
		DependencyObject Parent
		{
			get;
		}
		Point AnchorPoint
		{
			get;
		}
		AnnotationStore Store
		{
			get;
		}
	}
}
