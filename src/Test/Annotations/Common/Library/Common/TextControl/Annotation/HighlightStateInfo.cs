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
using Proxies.MS.Internal.Annotations.Component;
using System.Windows.Media;
using System.Collections;

namespace Avalon.Test.Annotations
{
	/// <summary>
	/// Defines that state of a HighlightComponent at some fixed point in time.
	/// </summary>
	public class HighlightStateInfo : AnnotationStateInfo
	{
		public HighlightStateInfo(Brush brush, int zOrder, object anchor) : base(zOrder)
		{
			_brush = brush;
            _anchor = anchor;
		}		

		public Brush HighlightBrush
		{
			get { return _brush; }
		}

		public Color HighlightBrushColor
		{
			get
			{
				return ((SolidColorBrush)_brush).Color;
			}
		}

		public object Anchor
		{
            get { return _anchor; }
		}

		/// <summary>
		/// Create a HighlightStateInfo object from a HighlightComponent proxy.
		/// </summary>
		public static HighlightStateInfo FromHighlightComponent(HighlightComponent highlight)
		{			
			Brush brush = GetBrush(highlight.Delegate);
			int zorder = highlight.ZOrder;
			object anchor = ReflectionHelper.GetProperty(highlight.AttachedAnnotations[0], "AttachedAnchor");
			return new HighlightStateInfo(brush, zorder, anchor);
		}

		/// <summary>
		/// Create a HighlightStateInfo object from a internal HighlightComponent object.
		/// </summary>
		public static HighlightStateInfo FromHighlightComponent(object highlight)
		{
			if (!AnnotationComponentFinder.HighlightComponentType.Equals(highlight.GetType()) && !highlight.GetType().IsSubclassOf(AnnotationComponentFinder.HighlightComponentType))
				throw new InvalidDataException("Object must be of type '" + AnnotationComponentFinder.HighlightComponentType.FullName + "' but was '" + highlight.GetType().FullName + "'.");

			Brush brush = GetBrush(highlight);
			int zorder = (int)ReflectionHelper.GetProperty(highlight, "ZOrder");
			IList attachedAnnotations = (IList)ReflectionHelper.GetProperty(highlight, "AttachedAnnotations");
			object anchor = ReflectionHelper.GetProperty(attachedAnnotations[0], "AttachedAnchor");
			return new HighlightStateInfo(brush, zorder, anchor);
		}

		private static Brush GetBrush(object highlightComponent)
		{
			DependencyProperty HighlightBrushProperty = (DependencyProperty)ReflectionHelper.FindFieldInHierarchy(highlightComponent.GetType(), "HighlightBrushProperty").GetValue(highlightComponent);
			return (Brush)((DependencyObject)highlightComponent).GetValue(HighlightBrushProperty);
		}

		public override bool Equals(object obj)
		{
			HighlightStateInfo state = ((HighlightStateInfo)obj);
			return base.Equals(obj)
				&& HighlightBrush is SolidColorBrush
				&& state.HighlightBrush is SolidColorBrush
				&& ((SolidColorBrush)HighlightBrush).Color.Equals(((SolidColorBrush)state.HighlightBrush).Color)
				&& AnnotationTestHelper.GetText(_anchor).Equals(AnnotationTestHelper.GetText(state.Anchor));
		}

		public override string ToString()
		{
			return base.ToString() + ", HighlightBrush=" + HighlightBrush.ToString();
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private Brush _brush;
		private object _anchor;
	}
}	
