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
using Microsoft.Test.Logging;

namespace Annotations.Test.Reflection
{
	/// <summary>
	/// Methods for performing conversions on an array of objects.
	/// </summary>
	public class ArrayModule
	{
		#region Public Methods

		/// <summary>
		/// Unwrap and array of proxies.
		/// Create a new array of a non-proxy type, then call UnwrapObject on each element
		/// within the array.
		/// </summary>
		public static object[] UnwrapArray(Assembly assembly, object[] array)
		{
			return Convert(assembly, array, ConversionType.Unwrap);
		}

		/// <summary>
		/// Unwrap and array of proxies.
		/// Create a new array of a non-proxy type, then call UnwrapObject on each element
		/// within the array.
		/// </summary>
		public static object[] WrapArray(Assembly assembly, object[] array)
		{
			return Convert(assembly, array, ConversionType.Wrap);
		}

		/// <summary>
		/// Convert an array of objects between proxy and delegate types.
		/// </summary>
		/// <param name="assembly">Assembly to load delegate from.</param>
		/// <param name="array">Objects to convert.</param>
		/// <param name="conversion">If ConversionType.Wrap, convert from delegate to proxy. 
		/// If ConversionType.Unwrap, convert from proxy to delegate.</param>
		/// <returns>Array of converted objects.</returns>
		public static object[] Convert(Assembly assembly, object[] array, ConversionType conversion)
		{
			if (array == null)
				return null;
			
			Type arrayType = array.GetType();
			string arrayTypeName = arrayType.FullName;
			string contentTypeName = arrayTypeName.Substring(0, arrayTypeName.Length - 2); // remove trailing '[]'.
			try
			{
				Type contentType = LoadType(assembly, contentTypeName);

				if (conversion == ConversionType.Wrap)
					contentType = ProxyTypeConverter.DelegateToProxyType(contentType);
				else
					contentType = ProxyTypeConverter.ProxyToDelegateType(assembly, contentType);

				arrayType = LoadType(assembly, contentType.FullName + "[]"); // replace trailing '[]'.
			}
			catch (TypeLoadException)
			{
				// We may fail to load some types that don't need to be wrapped/unwrapped.  We don't
				// want to fail for these types, so just ignore the conversion.
			}

			object[] convertedArray = (object[])ReflectionHelper.GetInstance(arrayType, new Type[] { typeof(int) }, new object[] { array.Length });
			for (int i = 0; i < array.Length; i++)
			{
				convertedArray[i] = ProxyTypeConverter.Convert(assembly, array[i], conversion);
			}
			return convertedArray;
		}

		#endregion Public Methods

		#region

		private static Type LoadType(Assembly assembly, string typename)
		{
			Type result = null;
			result = Type.GetType(typename); // try local assembly.
			if (result == null)
				result= assembly.GetType(typename);

			if (result == null)
				throw new TypeLoadException("Could not load type '" + typename + "'.");
			return result;
		}

		#endregion
	}
}


