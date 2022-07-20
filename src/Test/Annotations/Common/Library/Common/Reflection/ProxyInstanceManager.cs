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
	/// We don't want to create a new proxy for the same object each time it is routed.
	/// This class keeps a map of delegates to proxies so that if we already have a proxy
	/// for a given object we just return it instead of creating an new one.
	/// </summary>
	/// MOREMORE - may need to use weak references to prevent memory leaking.
	public class ProxyInstanceManager
	{
		/// <summary>
		/// Map from a Delegate object to an existing proxy for that delegate.
		/// </summary>
		private static IDictionary<object, object> proxyMap = new Dictionary<object, object>();

		#region Public Methods

		/// <summary>
		/// Get a proxy of the given type for the given delegate.  If one has previously been
		/// created return that, otherwise create a new one and cache it.
		/// </summary>
		/// <param name="delegateObject">Object to create a proxy for.</param>
		/// <param name="proxyType">Type of proxy to create for the 'delegateObject'.</param>
		/// <returns>New proxy instance if a proxy has never been created for this delegate before.
		/// Otherwise a cached proxy for this delegate.</returns>
		public static object GetProxy(object delegateObject, Type proxyType)
		{
			object proxy;
			if (proxyMap.ContainsKey(delegateObject))
			{
				proxy = proxyMap[delegateObject];
			}
			else
			{
				proxy = ReflectionHelper.GetInstance(proxyType.ToString(),
														new Type[] { typeof(object) },
														new object[] { delegateObject });
				proxyMap.Add(delegateObject, proxy);
			}
			return proxy;
		}

		/// <summary>
		/// Used when 
		/// Return the delegate represented by this proxy.  If this proxy does not exist in the cache
		/// (e.g. it is being passed in from an application) then add it to the cache.
		/// </summary>
		/// <returns>Delegate represented by given proxy.</returns>
		public static object GetDelegate(AReflectiveProxy proxy)
		{
			object delegateObject = proxy.Delegate;
			if (!proxyMap.ContainsKey(delegateObject))
			{
				proxyMap.Add(delegateObject, proxy);
			}
			return delegateObject;
		}

		#endregion Public Methods
	}
}


