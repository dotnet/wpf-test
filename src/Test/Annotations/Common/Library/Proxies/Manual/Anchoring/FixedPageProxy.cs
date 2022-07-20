// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  *** FILE IS AUTOMATICALLY GENERATED, DO NOT EDIT BY HAND ***
//
//        Generated: 1/14/2005 11:54:34 AM

// Required proxy imports.
using Annotations.Test.Reflection;
using System.Reflection;

// Delegate specific imports.
using System;
using System.Windows;
using System.Windows.Controls.Primitives;

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
using System.Collections.Generic;

namespace Proxies.MS.Internal.Annotations.Anchoring
{
	public class FixedPageProxy : AReflectiveProxy
	{

		//------------------------------------------------------
		//
		//  Delegate Constructors
		//
		//------------------------------------------------------

		public FixedPageProxy(DependencyObject parent, Int32 page)
		: base (new Type[] { typeof(DependencyObject), typeof(Int32) }, new object[] { parent, page })
		{
			//Empty.
		}

		//------------------------------------------------------
		//
		//  Proxy Constructors
		//
		//------------------------------------------------------

		static FixedPageProxy() { InitializeStaticFields(staticstatic_DelegateAssembly, MethodBase.GetCurrentMethod().DeclaringType); }
		protected FixedPageProxy(Type[] types, object[] values) : base (types, values) { }
		protected FixedPageProxy(object delegateObject) : base (delegateObject) { }

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
			return "MS.Internal.Annotations.Anchoring.FixedTextSelectionProcessor+FixedPageProxy";
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

		public Int32 Page
		{
			get
			{
				return (Int32)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
        public IList<PointSegment> Segments
		{
			get
			{
				return (IList<PointSegment>)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
			set
			{
				RouteInstance(MethodBase.GetCurrentMethod(), new object[] { value });
			}
		}
        public void AddSegment(PointSegment seg)
        {
            object segments = ReflectionHelper.GetProperty(Delegate, "Segments");
            object actualSeg = ProxyTypeConverter.Convert(delegateAssembly, seg, ConversionType.Unwrap);
            ReflectionHelper.InvokeMethod(segments, "Add", new object[] { actualSeg });
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
        
        protected static string staticstatic_DelegateAssembly = "PresentationFramework";

		//------------------------------------------------------
		//
		//  Events
		//
		//------------------------------------------------------

	}

    public class PointSegment : AReflectiveProxy
	{

		//------------------------------------------------------
		//
		//  Delegate Constructors
		//
		//------------------------------------------------------

        public PointSegment(Point start, Point end)
            : base(new Type[] { typeof(Point), typeof(Point) }, new object[] { start, end })
		{
			//Empty.
		}

		//------------------------------------------------------
		//
		//  Proxy Constructors
		//
		//------------------------------------------------------

		static PointSegment() { InitializeStaticFields(staticstatic_DelegateAssembly, MethodBase.GetCurrentMethod().DeclaringType); }
		protected PointSegment(Type[] types, object[] values) : base (types, values) { }
        protected PointSegment(object delegateObject) : base(delegateObject) { }

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
            return "MS.Internal.Annotations.Anchoring.FixedTextSelectionProcessor+PointSegment";
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

        public Point Start
        {
            get
            {
                return (Point)RouteInstance(MethodBase.GetCurrentMethod(), null);
            }
            set
            {
                RouteInstance(MethodBase.GetCurrentMethod(), new object[] { value });
            }
        }

        public Point End
        {
            get
            {
                return (Point)RouteInstance(MethodBase.GetCurrentMethod(), null);
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

        public static readonly Point NotAPoint = new Point(double.NaN, double.NaN);

        //------------------------------------------------------
        //
        //  Proxy Static Fields
        //
        //------------------------------------------------------

        //So that static methods can load the correct assembly.
        
        protected static string staticstatic_DelegateAssembly = "PresentationFramework";

        //------------------------------------------------------
        //
        //  Events
        //
        //------------------------------------------------------
    }
}
