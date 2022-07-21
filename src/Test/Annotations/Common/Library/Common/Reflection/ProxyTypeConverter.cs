// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: 

using System;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Permissions;	// ReflectionPermission.
using System.Text.RegularExpressions;

namespace Annotations.Test.Reflection
{
	/// <summary>
	/// Methods for converting between delegate and proxy types.
	///		Wrap = delegate -> proxy.
	///		Unwrap = proxy -> delegate.
	/// </summary>
	public class ProxyTypeConverter
	{
		/// <summary>
		/// Iterate through all this method's parameters and figure out what
		/// their corresponding types would be in the Delegate or Proxy space.
		/// 
		/// If ConversionType.Wrap, delegate -> proxy.  If ConversionType.Unwrap, proxy -> delegate.
		/// </summary>
		/// <param name="method"></param>
		/// <returns>Parameter Types of Delegate method's signature.</returns>
		public static Type[] ConvertParameterTypes(Assembly assembly, MethodBase method, ConversionType conversion)
		{
			ParameterInfo[] parameterInfo = method.GetParameters();
			Type[] types = new Type[parameterInfo.Length];
			for (int i = 0; i < parameterInfo.Length; i++)
			{
				if (conversion == ConversionType.Unwrap)
					types[i] = ProxyToDelegateType(assembly, parameterInfo[i].ParameterType);
				else
					types[i] = DelegateToProxyType(parameterInfo[i].ParameterType);
			}
			return types;
		}

		/// <summary>
		/// Convert all types from Proxy -> Delegate.
		/// </summary>
		/// <param name="method"></param>
		public static Type[] ProxyToDelegateType(Assembly assembly, Type[] types)
		{
			Type[] converted = new Type[types.Length];
			for (int i = 0; i < types.Length; i++)
				converted[i] = ProxyToDelegateType(assembly, types[i]);
			return converted;
		}

		public static object Convert(Assembly assembly, object toConvert, ConversionType conversion)
		{
			if (toConvert == null)
				return null;

			Type objectType = toConvert.GetType();
			object converted = toConvert;
			if (objectType.IsArray) // Convert Arrays.
			{
				converted = ArrayModule.Convert(assembly, (object[])toConvert, conversion);
			}
			else if (objectType.IsGenericType) // Convert Generic objects.
			{
				converted = GenericModule.Convert(assembly, toConvert, conversion);
			}
			else
			{
				if (conversion == ConversionType.Wrap)
				{

					Type proxyType = DelegateToProxyType(objectType);
					if (!proxyType.Equals(objectType))
					{
						if (proxyType.IsEnum)
						{
							converted = EnumModule.Convert(toConvert, proxyType);
						}
						else
						{
							converted = ProxyInstanceManager.GetProxy(toConvert, proxyType);
						}						
					}
				}
				else
				{
					Type delegateType = ProxyTypeConverter.ProxyToDelegateType(assembly, objectType);
					bool delegateTypeExists = !Type.Equals(delegateType, objectType);

					// Handle case where Proxy is for an inner class.
					if (!delegateTypeExists)
					{
						AReflectiveProxy objAsProxy = toConvert as AReflectiveProxy;
						if (objAsProxy != null)
						{
							delegateType = assembly.GetType(objAsProxy.DelegateClassName());
							delegateTypeExists = (delegateType != null);
						}
					}

					// If delegate type exists.
					if (delegateTypeExists)
					{
						if (objectType.IsEnum)
						{
							converted = EnumModule.Convert(toConvert, delegateType);
						}
						else if (toConvert is EventHandler)
						{
							converted = ((EventHandler)toConvert).Method;
						}
						else if (objectType.IsSubclassOf(typeof(AReflectiveProxy)))
						{
							converted = ProxyInstanceManager.GetDelegate(toConvert as AReflectiveProxy);
						}
					}					
				}
			}
			return converted;
		}

		/// <summary>
		/// Convert delegate object to corresponding proxy.
		/// (e.g. ClassA -> Proxies.ClassA).
		/// 
		/// If object does not have a proxy, original object is returned.
		/// </summary>
		/// <param name="assembly">Unused.</param>
		/// <param name="toWrap">Object to wrap in proxy.</param>
		public static object WrapObject(Assembly assembly, object toWrap)
		{
			return Convert(assembly, toWrap, ConversionType.Wrap);
		}

		/// <summary>
		/// Convert AReflectiveProxy (or collection of proxies) to delegates.
		/// (e.g. Proxies.ClassA -> ClassA).
		/// 
		/// If object is not a proxy, original object is returned.
		/// </summary>
		/// <param name="assembly">Assembly inwhich proxy's delegate can be found.</param>
		/// <returns>Unwrapped delegate object.</returns>
		public static object UnwrapObject(Assembly assembly, object toUnwrap)
		{
			return Convert(assembly, toUnwrap, ConversionType.Unwrap);
		}

		/// <summary>
		/// Use namespace symmetry to infer the Type of the delegate for the given
		/// Proxy type.
		/// If given type is not a proxy then the same type will be returned.
		/// </summary>
		/// <param name="proxyType"></param>
		/// <returns>Type of this proxy's delegate.</returns>
		/// <exception cref="AnnotationProxyException">If no delegate type is found and given Type is a
		/// subclass of AReflectiveProxy.</exception>
		public static Type ProxyToDelegateType(Assembly assembly, Type proxyType)
		{
			Type delegateType = null;
			if (proxyType != null)
			{
				if (proxyType.IsGenericType)
				{
					delegateType = GenericModule.ProxyToDelegate(assembly, proxyType);
				}
				else
				{
					string delegateName = proxyType.FullName.Substring(ProxyConstants.PROXY_NAMESPACE_PREFIX.Length + 1);
					delegateType = assembly.GetType(delegateName, false);
				}

				if (delegateType == null)
					delegateType = proxyType;
			}
			return delegateType;
		}

		/// <summary>
		/// Take a delegate type and use Namespace symmetry to return its corresponding Proxy type.
		/// If no proxy exists, returns same type that was passed in, never returns null.
		/// </summary>
		/// <returns>Type of this object's corresponding proxy or null.</returns>
		public static Type DelegateToProxyType(Type delegateType)
		{
			Type proxyType = null;
			if (delegateType != null)
			{
				if (delegateType.IsGenericType || delegateType.IsGenericTypeDefinition)
				{
					proxyType = GenericModule.DelegateToProxy(null, delegateType);
				}
				else
				{
					proxyType = Type.GetType(ProxyConstants.PROXY_NAMESPACE_PREFIX + "." + delegateType.ToString(), false);
					// If nested type, check and see if there is a non-nested proxy equivalent.
					if (proxyType == null && delegateType.IsNested)
					{
						string unNestedName = delegateType.DeclaringType.Namespace + "." + delegateType.Name;
						proxyType = Type.GetType(ProxyConstants.PROXY_NAMESPACE_PREFIX + "." + unNestedName, false);
					}

					// If no proxy has been found, start looking up the inheritence hierarchy.
					if (proxyType == null)
					{
						proxyType = DelegateToProxyType(delegateType.BaseType);
						// Handle case where no proxy is found, need to unwind Type hierarchy.
						if (proxyType != null && !proxyType.IsSubclassOf(typeof(AReflectiveProxy)))
							proxyType = null;
					}
				}

				if (proxyType == null)
					return delegateType;
			}
			return proxyType;
		}
	}
}


