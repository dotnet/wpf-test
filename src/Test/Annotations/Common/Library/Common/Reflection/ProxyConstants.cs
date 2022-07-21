// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace Annotations.Test.Reflection
{
	public class ProxyConstants
	{
		/// <summary>
		/// Proxy classes are infered through symmetric namespaces, affixing this prefix to the
		/// namespace of a delegate will resolve to a proxy class (if one exists).
		/// </summary>
		public static string PROXY_NAMESPACE_PREFIX = "Proxies";
	}
}


