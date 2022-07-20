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
	public enum ConversionType
	{
		Wrap,	// Convert from Delegate to Proxy.
		Unwrap	// Convert from Proxy to Delegate.
	}
}


