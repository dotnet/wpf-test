// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: 

// Required proxy imports.
using Annotations.Test.Reflection;
using System.Reflection;
using System.Windows;

// Delegate specific imports.
using System;
using System.Collections;
using System.Windows.Media;
using Proxies.System.Windows.Annotations;

namespace Proxies.MS.Internal.Annotations.Component
{
	class Proxy_IAnnotationComponent : AReflectiveProxy, IAnnotationComponent
	{
		private object RouteDirectly(MethodBase method, object[] parameters)
		{
			ParameterInfo []  parameterInfos = method.GetParameters();
			Type [] types = new Type[parameterInfos.Length];
			for (int i=0; i < parameterInfos.Length; i++) 
			{
				types[i] = parameterInfos[i].ParameterType;
			}

			return ReflectionHelper.InvokeMethod(Delegate, method.Name, types, parameters);
		}
	
		//------------------------------------------------------
		//
		//  Proxy Constructors
		//
		//------------------------------------------------------

		protected Proxy_IAnnotationComponent(Type[] types, object[] values) : base (types, values) { throw new NotSupportedException("Class should only be instantiated via reflection using protected base constructor."); }
		public Proxy_IAnnotationComponent(object delegateObject) : base (delegateObject) { }
		protected Proxy_IAnnotationComponent(object delegateObject, Assembly asm) : base (delegateObject, asm) { }

		//------------------------------------------------------
		//
		//  Delegate Methods
		//
		//------------------------------------------------------

		public GeneralTransform GetDesiredTransform(GeneralTransform transform)
		{
			object[] parameters = new object[1];
			parameters[0] = transform;
			return (GeneralTransform) RouteDirectly(MethodBase.GetCurrentMethod(), parameters);
		}
		public void AddAttachedAnnotation(IAttachedAnnotation attachedAnnotation)
		{
			object[] parameters = new object[1];
			parameters[0] = attachedAnnotation;
			RouteDirectly(MethodBase.GetCurrentMethod(), parameters);
		}
		public void RemoveAttachedAnnotation(IAttachedAnnotation attachedAnnotation)
		{
			object[] parameters = new object[1];
			parameters[0] = attachedAnnotation;
			RouteDirectly(MethodBase.GetCurrentMethod(), parameters);
		}
		public void ModifyAttachedAnnotation(IAttachedAnnotation attachedAnnotation, Object previousAttachedAnchor, AttachmentLevel previousAttachmentLevel)
		{
			object[] parameters = new object[3];
			parameters[0] = attachedAnnotation;
			parameters[1] = previousAttachedAnchor;
			parameters[2] = previousAttachmentLevel;
			RouteDirectly(MethodBase.GetCurrentMethod(), parameters);
		}

		//------------------------------------------------------
		//
		//  Properties
		//
		//------------------------------------------------------

		public int ZOrder
		{
			get
			{
				return (int)RouteDirectly(MethodBase.GetCurrentMethod(), new object[0]);
			}
			set
			{
				object[] parameters = new object[1];
				parameters[0] = value;
				RouteDirectly(MethodBase.GetCurrentMethod(), parameters);
			}
		}
		public IList AttachedAnnotations
		{
			get
			{
				return (IList) RouteDirectly(MethodBase.GetCurrentMethod(), new object[0]);
			}
		}
		public PresentationContext PresentationContext
		{
			get
			{
				return (PresentationContext)RouteDirectly(MethodBase.GetCurrentMethod(), new object[0]);
			}
			set
			{
				object[] parameters = new object[1];
				parameters[0] = value;
				RouteDirectly(MethodBase.GetCurrentMethod(), parameters);
			}
		}
		public UIElement AnnotatedElement
		{
			get
			{
				return (UIElement)RouteDirectly(MethodBase.GetCurrentMethod(), new object[0]);
			}
		}

		public override string DelegateClassName()
		{
			return null;
		}

		protected override string DelegateAssemblyName()
		{
			return "PresentationFramework";
		}

}
}


