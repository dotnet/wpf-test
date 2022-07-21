// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: 
//
using System;
using System.Windows;
using Annotations.Test.Framework;
using System.IO;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Windows.Controls;
using Annotations.Test.Reflection;

namespace Avalon.Test.Annotations
{
	/// <summary>
	/// Defines the state of an IAnnotationComponent at some fixed point in time.
	/// </summary>
	public abstract class AnnotationStateInfo
	{
		protected AnnotationStateInfo(int zOrder)
		{			
			_zOrder = zOrder;
		}		

		public int ZOrder
		{
			get { return _zOrder; }
		}

		public override bool Equals(object obj)
		{
			AnnotationStateInfo toCompare = (AnnotationStateInfo)obj;
			return ZOrder.Equals(toCompare.ZOrder);
		}

		public override string ToString()
		{
			return "ZOrder=" + ZOrder;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		private int _zOrder;
	}
}	
