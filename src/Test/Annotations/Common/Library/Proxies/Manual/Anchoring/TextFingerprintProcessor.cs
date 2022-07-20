// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  *** FILE IS AUTOMATICALLY GENERATED, DO NOT EDIT BY HAND ***
//
//        Generated: 11/30/2004 5:20:21 PM

// Required proxy imports.
using Annotations.Test.Reflection;
using System.Reflection;

// Delegate specific imports.
using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using Proxies.System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using System.Windows.Documents;
using System.Xml;

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

namespace Proxies.MS.Internal.Annotations.Anchoring
{
	public class TextFingerprintProcessor : SubTreeProcessor
	{

		//------------------------------------------------------
		//
		//  Delegate Constructors
		//
		//------------------------------------------------------

		public TextFingerprintProcessor(LocatorManager manager)
		: base (new Type[] { typeof(LocatorManager) }, new object[] { manager })
		{
			//Empty.
		}
		public TextFingerprintProcessor()
		: base (null, null)
		{
			//Empty.
		}

		//------------------------------------------------------
		//
		//  Proxy Constructors
		//
		//------------------------------------------------------

		static TextFingerprintProcessor() { InitializeStaticFields(staticstatic_DelegateAssembly, MethodBase.GetCurrentMethod().DeclaringType); }
		protected TextFingerprintProcessor(Type[] types, object[] values) : base (types, values) { }
		protected TextFingerprintProcessor(object delegateObject) : base (delegateObject) { }

		//------------------------------------------------------
		//
		//  Delegate Methods
		//
		//------------------------------------------------------

		override public IList<IAttachedAnnotation> PreProcessNode(DependencyObject node, out Boolean calledProcessAnnotations)
		{
			object [] parameters = new object[2];
			parameters[0] = node;
			IList<IAttachedAnnotation> result = (IList<IAttachedAnnotation>) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			calledProcessAnnotations = (Boolean) parameters[1];
			return result;
		}
		override public ContentLocator GenerateLocator(PathNode node, out Boolean continueGenerating)
		{
			object [] parameters = new object[2];
			parameters[0] = node;
			ContentLocator result = (ContentLocator) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			continueGenerating = (Boolean) parameters[1];
			return result;
		}
		override public DependencyObject ResolveLocatorPart(ContentLocatorPart locatorPart, DependencyObject startNode, out Boolean continueResolving)
		{
			object [] parameters = new object[3];
			parameters[0] = locatorPart;
			parameters[1] = startNode;
			DependencyObject result = (DependencyObject) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			continueResolving = (Boolean) parameters[2];
			return result;
		}
		override public XmlQualifiedName[] GetLocatorPartTypes()
		{
			object [] parameters = new object[0];
			XmlQualifiedName[] result = (XmlQualifiedName[]) RouteInstance(MethodBase.GetCurrentMethod(), parameters);
			return result;
		}

		//------------------------------------------------------
		//
		//  Proxy Methods
		//
		//------------------------------------------------------

		public override string DelegateClassName()
		{
			return "System.Windows.Annotations.Anchoring.TextFingerprintProcessor";
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

		public static String Id = "TextFingerprint";

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

	}
}
