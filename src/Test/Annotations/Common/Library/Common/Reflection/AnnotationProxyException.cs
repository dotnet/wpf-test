// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Exception indicating an error in the proxy layer.

using System;
using System.Windows;

namespace Annotations.Test.Reflection
{
	class AnnotationProxyException : Exception 
	{
		public AnnotationProxyException(string msg) : base(msg)
		{

		}
	}
}


