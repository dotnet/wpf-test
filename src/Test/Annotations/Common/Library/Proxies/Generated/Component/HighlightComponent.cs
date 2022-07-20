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
using System.Xml;
using System.Collections;
using Proxies.MS.Internal.Annotations.Component;
using System.Windows.Media;
using System.Windows;
using Proxies.MS.Internal.Annotations;
using System.Windows.Shapes;
using Proxies.System.Windows.Annotations;

namespace Proxies.MS.Internal.Annotations.Component
{
	public class HighlightComponent : AReflectiveProxy, IAnnotationComponent
	{

		//------------------------------------------------------
		//
		//  Delegate Constructors
		//
		//------------------------------------------------------

		public HighlightComponent()
		: base (Type.EmptyTypes, new object[0])
		{
			//Empty.
		}
		public HighlightComponent(Int32 priority, XmlQualifiedName type)
		: base (new Type[] { typeof(Int32), typeof(XmlQualifiedName) }, new object[] { priority, type })
		{
			//Empty.
		}

		//------------------------------------------------------
		//
		//  Proxy Constructors
		//
		//------------------------------------------------------

		static HighlightComponent() { InitializeStaticFields(static_DelegateAssembly, MethodBase.GetCurrentMethod().DeclaringType); }
		protected HighlightComponent(Type[] types, object[] values) : base (types, values) { }
		protected HighlightComponent(object delegateObject) : base (delegateObject) { }

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
		virtual public void AddAttachedAnnotation(IAttachedAnnotation attachedAnnotation)
		{
			object [] parameters = new object[1];
			parameters[0] = attachedAnnotation;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		virtual public void RemoveAttachedAnnotation(IAttachedAnnotation attachedAnnotation)
		{
			object [] parameters = new object[1];
			parameters[0] = attachedAnnotation;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		virtual public void ModifyAttachedAnnotation(IAttachedAnnotation attachedAnnotation, Object previousAttachedAnchor, AttachmentLevel previousAttachmentLevel)
		{
			object [] parameters = new object[3];
			parameters[0] = attachedAnnotation;
			parameters[1] = previousAttachedAnchor;
			parameters[2] = previousAttachmentLevel;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		public void Activate(Boolean active)
		{
			object [] parameters = new object[1];
			parameters[0] = active;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		virtual public void AddChild(Shape child)
		{
			object [] parameters = new object[1];
			parameters[0] = child;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		virtual public void RemoveChild(Shape child)
		{
			object [] parameters = new object[1];
			parameters[0] = child;
			RouteInstance(MethodBase.GetCurrentMethod(), parameters);
		}
		static public Color GetColor(String color)
		{
			object [] parameters = new object[1];
			parameters[0] = color;
			Color routedResult = (Color) AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), parameters, static_DelegateAssembly);
			return routedResult;
		}
		public void OnAnnotationUpdated(Object sender, AnnotationResourceChangedEventArgs args)
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
			return "MS.Internal.Annotations.Component.HighlightComponent";
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

		virtual public IList AttachedAnnotations
		{
			get
			{
				return (IList)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		virtual public PresentationContext PresentationContext
		{
			get
			{
				return (PresentationContext)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
			set
			{
				RouteInstance(MethodBase.GetCurrentMethod(), new object[] { value });
			}
		}
		virtual public Int32 ZOrder
		{
			get
			{
				return (Int32)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
			set
			{
				RouteInstance(MethodBase.GetCurrentMethod(), new object[] { value });
			}
		}
		static public XmlQualifiedName TypeName
		{
			get
			{
				return (XmlQualifiedName)AReflectiveProxy.RouteStatic(MethodBase.GetCurrentMethod(), null, Assembly.Load(static_DelegateAssembly));
			}
		}
		public Color DefaultBackground
		{
			get
			{
				return (Color)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
			set
			{
				RouteInstance(MethodBase.GetCurrentMethod(), new object[] { value });
			}
		}
		public Color DefaultActiveBackground
		{
			get
			{
				return (Color)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
			set
			{
				RouteInstance(MethodBase.GetCurrentMethod(), new object[] { value });
			}
		}
		public Brush HighlightBrush
		{
			set
			{
				RouteInstance(MethodBase.GetCurrentMethod(), new object[] { value });
			}
		}
		virtual public UIElement AnnotatedElement
		{
			get
			{
				return (UIElement)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		virtual public Color Background
		{
			get
			{
				return (Color)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		virtual public Color SelectedBackground
		{
			get
			{
				return (Color)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}
		virtual public Int32 Priority
		{
			get
			{
				return (Int32)RouteInstance(MethodBase.GetCurrentMethod(), null);
			}
		}

		//------------------------------------------------------
		//
		//  Delegate Static Fields
		//
		//------------------------------------------------------

		public static DependencyProperty HighlightBrushProperty;
        
        public static XmlQualifiedName name;
		public static String HighlightResourceName = "Highlight";
		public static String ColorsContentName = "Colors";
		public static String BackgroundAttributeName = "Background";
		public static String ActiveBackgroundAttributeName = "ActiveBackground";

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
        
		private Color _background
		{
			get { return (Color) GetField("_background"); }
			set { SetField("_background", value); }
		}

        private Color _selectedBackground
		{
			get { return (Color) GetField("_selectedBackground"); }
			set { SetField("_selectedBackground", value); }
		}

        private IAttachedAnnotation _attachedAnnotation
		{
			get { return (IAttachedAnnotation) GetField("_attachedAnnotation"); }
			set { SetField("_attachedAnnotation", value); }
		}

        private PresentationContext _presentationContext
		{
			get { return (PresentationContext) GetField("_presentationContext"); }
			set { SetField("_presentationContext", value); }
		}

        private XmlQualifiedName _type
		{
			get { return (XmlQualifiedName) GetField("_type"); }
			set { SetField("_type", value); }
		}

        private Int32 _priority
		{
			get { return (Int32) GetField("_priority"); }
			set { SetField("_priority", value); }
		}

        private Boolean _active
		{
			get { return (Boolean) GetField("_active"); }
			set { SetField("_active", value); }
		}

        private Color _defaultBackroundColor
		{
			get { return (Color) GetField("_defaultBackroundColor"); }
			set { SetField("_defaultBackroundColor", value); }
        }

        private Color _defaultActiveBackgroundColor
		{
			get { return (Color) GetField("_defaultActiveBackgroundColor"); }
			set { SetField("_defaultActiveBackgroundColor", value); }
		}
	}
}
