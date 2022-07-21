// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
	/// Methods for converting enums of one type to their corresponding values in another
	/// type.
	/// </summary>
	public class EnumModule
	{
		/// <summary>
		/// Convert 'enumValue' to the corresponding value of 'targetType'.
		/// </summary>
		/// <param name="enumValue"></param>
		/// <param name="targetType"></param>
		/// <returns>Converted enum value.</returns>
		/// <exception cref="AnnotationProxyException">If error converting.</exception>
		public static object Convert(object enumValue, Type targetType)
		{
			object result = null;

			string[] names = Enum.GetNames(targetType);
			for (int k = 0; k < names.Length; k++)
			{
				if (names[k].Equals(enumValue.ToString()))
				{
					result = Enum.GetValues(targetType).GetValue(k);
					break;
				}
			}

			if (result == null)
				throw new AnnotationProxyException("Could not convert enum '" + enumValue.GetType().ToString() + "." + enumValue.ToString() + "' to type '" + targetType.ToString() + "'.");
			return result;
		}
	}
}


