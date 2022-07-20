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
using System.Windows.Media;

namespace Avalon.Test.Annotations
{
	/// <summary>
	/// Defines that state of a StickyNoteControl at some fixed point in time.
	/// </summary>
	public class StickyNoteStateInfo : AnnotationStateInfo
	{
		public StickyNoteStateInfo(Point position, bool isMaximized, int zOrder, HighlightStateInfo anchor) : base(zOrder)
		{
			_isMaximized = isMaximized;
			_position = position;
			_anchor = anchor;
		}

		public Point Position
		{
			get { return _position; }
		}

		public bool IsMaximized
		{
			get { return _isMaximized; }
		}

		public HighlightStateInfo AnchorState
		{
			get { return _anchor; }
		}

		/// <summary>
		/// Create a StickyNoteStateInfo object from a StickyNoteControl.
		/// </summary>
		public static StickyNoteStateInfo FromStickyNote(StickyNoteControl sn)
		{
            //Visual parent = sn;
            //do
            //{
            //    parent = (Visual)VisualOperations.GetParent(parent);
            //}
            //while (!(parent is AdornerDecorator) && parent != null);
            
            //GeneralTransform transform = VisualOperations.TransformToVisual(sn, parent);
            //Point location;
            //transform.TryTransform(new Point(0, 0), out location);

            Rect boundingRect = (Rect)ReflectionHelper.GetProperty(sn, "StickyNoteBounds");
            Point location = new Point(boundingRect.Left, boundingRect.Top);

			int zorder = (int)ReflectionHelper.GetProperty(sn, "ZOrder");
			object markedHighlight = ReflectionHelper.GetField(sn, "_anchor");
			HighlightStateInfo anchorState = HighlightStateInfo.FromHighlightComponent(ReflectionHelper.GetField(markedHighlight, "_highlightAnchor"));
			return new StickyNoteStateInfo(location, sn.IsExpanded, zorder, anchorState);
		}

		public override bool Equals(object obj)
		{
			StickyNoteStateInfo toCompare = (StickyNoteStateInfo)obj;
			double tolerance = 1e-5;
			
			return base.Equals(obj)			
				&& TestSuite.Compare(Position.X, toCompare.Position.X, tolerance) 
				&& TestSuite.Compare(Position.Y, toCompare.Position.Y, tolerance)
				&& IsMaximized.Equals(toCompare.IsMaximized);
		}

		public override string ToString()
		{
			return base.ToString() + ", Position=(" + Position.X + ", " + Position.Y + ")" + ", IsMaximized=" + IsMaximized + " with anchor " + _anchor.ToString();
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private bool _isMaximized;
		private Point _position;
		private HighlightStateInfo _anchor;
	}
}	
