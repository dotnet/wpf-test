// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides helps for finding location of adorners

namespace Test.Uis.TextEditing
{
    #region Namespaces.
	using System;
	using System.Windows.Documents;
	using System.Windows;
    using Microsoft.Test.Imaging;
    #endregion Namespaces.

	/// <summary>This class help to calculating the location of adorner </summary>
	public class AdornerUtils
	{
		/// <summary>
		/// Specifies the relationship between the adorner and a coordinate system.
		/// </summary>
		public enum AdornerRelatives
		{
			/// <summary>
			/// Specifies that a point is relative to the screen origin.
			/// </summary>
			RelativeToScreen = 1,
			/// <summary>
			/// Specifies that a point is relative to the client area origin.
			/// </summary>
			RelativeToClientArea = 2,
			/// <summary>
			/// Specifies that a point is relative to the element origin.
			/// </summary>
			RelativeToTheElement = 3
		}

		/// <summary>
		/// Find a location of a grab hanle of an adorner for an element.
		/// if the element have more than one adorners, we can only find one.
		/// </summary>
		/// <param name="relative">Specifc the reference</param>
		/// <param name="element">Given an element</param>
		/// <param name="handle">Given a handle</param>
		/// <returns></returns>
		static public Point GetGrabHandleLocation(AdornerRelatives relative, UIElement element, GrabHandles handle)
		{
			if (null == element)
				throw new ArgumentNullException("element");

			return GetGrabHandleLocation(relative, GetAdornerForUIElement(element) as GrabHandleAdorner, handle);
		}
		
		/// <summary>
		/// Find the location for a grabHandle Adorner.
		/// </summary>
		/// <param name="relative"></param>
		/// <param name="gAdorner"></param>
		/// <param name="handle"></param>
		/// <returns></returns>
		static public Point GetGrabHandleLocation(AdornerRelatives relative, GrabHandleAdorner gAdorner, GrabHandles handle)
		{
			if (null == gAdorner)
				throw new ArgumentNullException("gAdorner");

			Rect rc = GetRectForUIElement(relative, gAdorner.AdornedElement);

			Point point = CalculateGetGrabHandleLocation(rc, gAdorner, handle);

			if (relative == AdornerRelatives.RelativeToTheElement)
			{
				point.X = point.X - rc.Left;
				point.Y = point.Y - rc.Top;
			}

			return point;
		}
		

		/// <summary>
		/// Find the location of Lollipophandle for an UIElement at a specific position
		/// </summary>
		/// <param name="relative">Given the reference</param>
		/// <param name="element">Given the element</param>
		/// <param name="position">Given the handle position</param>
		/// <returns>a point</returns>
		
		static public Point GetLollipopHandleLocation(AdornerRelatives relative, UIElement element, LollipopPosition position)
		{
			if (null == element)
				throw new ArgumentNullException("element");

			return GetLollipopHandleLocation(relative, GetAdornerForUIElement(element) as LollipopAdorner,position) ;
		}

		/// <summary>
		/// Find the location for a handler of LollipopAdorner
		/// </summary>
		/// <param name="relative"></param>
		/// <param name="lAdorner"></param>
		/// <param name="position"></param>
		/// <returns></returns>
		static public Point GetLollipopHandleLocation(AdornerRelatives relative, LollipopAdorner lAdorner, LollipopPosition position)
		{
			if (null == lAdorner)
				throw new ArgumentNullException("lAdorner");

			Rect rc = GetRectForUIElement(relative, lAdorner.AdornedElement);
			Point point = CalculateLllipopAdonerLocation(rc, lAdorner, position, LollipopHandle.Lollipop);

			if (relative == AdornerRelatives.RelativeToTheElement)
			{
				point.X = point.X - rc.Left;
				point.Y = point.Y - rc.Top;
			}

			return point;
		}
		/// <summary>
		/// Find the location of Anchor of a LollipopAdorner
		/// </summary>
		/// <param name="relative"></param>
		/// <param name="lAdorner"></param>
		/// <returns></returns>
		static public Point GetLollipopAnchorLocation(AdornerRelatives relative, LollipopAdorner lAdorner)
		{
			if (null == lAdorner)
				throw new ArgumentNullException("lAdorner");

			Rect rc = GetRectForUIElement(relative, lAdorner.AdornedElement);
			Point point = CalculateLllipopAdonerLocation(rc, lAdorner, LollipopPosition.TopLeft, LollipopHandle.Anchor);

			if (relative == AdornerRelatives.RelativeToTheElement)
			{
				point.X = point.X - rc.Left;
				point.Y = point.Y - rc.Top;
			}

			return point;
		}

		/// <summary>
		/// Calculate the location for a lollipop anchor.
		/// </summary>
		/// <param name="relative">Referece to where?</param>
		/// <param name="element">Given an element</param>
		/// <returns></returns>
		static public Point GetLollipopAnchorLocation(AdornerRelatives relative, UIElement element)
		{
			if (null == element)
				throw new ArgumentNullException("element");

			return GetLollipopAnchorLocation(relative, GetAdornerForUIElement(element) as LollipopAdorner);
		}
		
		/// <summary>
		/// find the Rect for an UIElement.
		/// </summary>
		/// <param name="relative">given the reference point</param>
		/// <param name="element">Given the element</param>
		/// <returns></returns>
		static private Rect GetRectForUIElement(AdornerRelatives relative, UIElement element)
		{
			Rect rc;

			switch (relative)
			{
				case AdornerRelatives.RelativeToClientArea: rc = ElementUtils.GetClientRelativeRect(element);
					break;

				case AdornerRelatives.RelativeToScreen:
				case AdornerRelatives.RelativeToTheElement:
				default:
					rc = ElementUtils.GetScreenRelativeRect(element);
					break;
			}
			return rc;
		}
	
		/// <summary>
		/// Calculate the location for LollipopAdorner.
		/// </summary>
		/// <param name="rc"></param>
		/// <param name="lAdorner"></param>
		/// <param name="position"></param>
		/// <param name="handle"></param>
		/// <returns></returns>
		static private Point CalculateLllipopAdonerLocation(Rect rc, LollipopAdorner lAdorner, LollipopPosition position, LollipopHandle handle)
		{
			Point point = new Point(0, 0);
			int root2 = (int)Math.Sqrt(Math.Pow((lAdorner.LollipopStemLength + lAdorner.LollipopHeadDiameter / 2.0), 2) / 2.0);

			if (handle == LollipopHandle.Anchor)
			{
				point = new Point(rc.Left + lAdorner.CenterPoint.X, rc.Top + lAdorner.CenterPoint.Y);
			}
			else if (handle == LollipopHandle.Lollipop)
			{
				switch (position)
				{
					case LollipopPosition.TopLeft:
						point = new Point(rc.Left - root2, rc.Top - root2);
						break;

					case LollipopPosition.TopCenter:
						point = new Point((rc.Left + rc.Right) / 2, rc.Top - lAdorner.LollipopStemLength - lAdorner.LollipopHeadDiameter / 2);
						break;

					case LollipopPosition.TopRight:
						point = new Point(rc.TopRight.X + root2, rc.Top - root2);
						break;

					case LollipopPosition.LeftCenter:
						point = new Point(rc.Left - lAdorner.LollipopStemLength - lAdorner.LollipopHeadDiameter / 2, (rc.Top + rc.Bottom) / 2);
						break;

					case LollipopPosition.RightCenter:
						point = new Point(rc.TopRight.X + lAdorner.LollipopStemLength + lAdorner.LollipopHeadDiameter / 2, (rc.Top + rc.Bottom) / 2);
						break;

					case LollipopPosition.BottomLeft:
						point = new Point(rc.Left - root2, rc.BottomLeft.Y + root2);
						break;

					case LollipopPosition.BottomCenter:
						point = new Point((rc.Left + rc.Right) / 2, rc.Bottom + lAdorner.LollipopStemLength + lAdorner.LollipopHeadDiameter / 2);
						break;

					case LollipopPosition.BottomRight:
						point = new Point(rc.BottomRight.X + root2, rc.BottomRight.Y + root2);
						break;

					default:
						point = new Point(0, 0);
						break;
				}
			}

			return point;
		}
		/// <summary>
		/// Find an adorner for an UIElement. We will through an exception if no adorner found.
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		static private Adorner GetAdornerForUIElement(UIElement element)
		{
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(element);

			if (null == adornerLayer)
				throw new ArgumentNullException("adornerLayer");

			if (null == adornerLayer)
				throw new Exception("Can't find AdornerLayer!!!");

			Adorner[] ads = adornerLayer.GetAdorners(element);
			
			if (ads == null || ads[0] == null)
				throw new Exception("No adorners can be found for specified element!!!");
			//if more than one adorners is associated to an element return the Bottom one or the top most one?
			return ads[0];
		}

		/// <summary>
		/// direct to calculate for each position
		/// </summary>
		/// <param name="rc"></param>
		/// <param name="gb"></param>
		/// <param name="handle"></param>
		/// <returns></returns>
		static private Point CalculateGetGrabHandleLocation(Rect rc, GrabHandleAdorner gb, GrabHandles handle)
		{
			//What do we do if the location does not have a grab handle?
			/*if ((gb.GrabHandles & handle)==0)
			{
				throw new Exception("Adorner at the specified location does not exsist!!!");
			}
			*/
			switch (handle)
			{
				case GrabHandles.TopLeft: return GetTopLeftHandlerLocation(rc, gb);

				case GrabHandles.TopCenter: return GetTopCenterHandlerLocation(rc,  gb);

				case GrabHandles.TopRight: return GetTopRightHandlerLocation(rc,  gb);

				case GrabHandles.LeftCenter: return GetLeftCenterHandlerLocation(rc,  gb);

				case GrabHandles.RightCenter: return GetRightCenterHandlerLocation(rc,  gb);

				case GrabHandles.BottomLeft: return GetBottomLeftHandlerLocation(rc, gb);

				case GrabHandles.BottomCenter: return GetBottomCenterHandlerLocation(rc, gb);

				default: return GetBottomRightHandlerLocation(rc, gb);
			}
		}

		/// <summary>
		/// Calculate for TopLeft
		/// </summary>
		/// <param name="rc"></param>
		/// <param name="gb"></param>
		/// <returns></returns>
		static Point  GetTopLeftHandlerLocation(Rect rc, GrabHandleAdorner gb)
		{
			switch (gb.GrabHandleAnchor)
			{
				case GrabHandleAnchor.Centered:
					return new Point(rc.Left, rc.Top);
					
				case GrabHandleAnchor.Inside:
					return new Point(rc.Left + gb.Size.Width / 2, rc.Top + gb.Size.Height / 2);
					
				case GrabHandleAnchor.Outside:
				default:
				return new Point(rc.Left - gb.Size.Width / 2, rc.Top - gb.Size.Height / 2);	
			}
		}

		/// <summary>
		/// Calculate for TopCenter
		/// </summary>
		/// <param name="rc"></param>
		/// <param name="gb"></param>
		/// <returns></returns>
		static Point GetTopCenterHandlerLocation(Rect rc, GrabHandleAdorner gb)
		{
			switch (gb.GrabHandleAnchor)
			{
				case GrabHandleAnchor.Centered:	return new Point(rc.Left + rc.Width / 2, rc.Top);
				case GrabHandleAnchor.Inside:	return  new Point(rc.Left + rc.Width / 2, rc.Top + gb.Size.Height / 2);
				case GrabHandleAnchor.Outside:
				default:						return new Point(rc.Left + rc.Width / 2, rc.Top - gb.Size.Height / 2);
			}
		}

		/// <summary>
		/// Calculate for TopRight
		/// </summary>
		/// <param name="rc"></param>
		/// <param name="gb"></param>
		/// <returns></returns>
		static Point GetTopRightHandlerLocation(Rect rc, GrabHandleAdorner gb)
		{
			switch (gb.GrabHandleAnchor)
			{
				case GrabHandleAnchor.Centered:	return new Point(rc.TopRight.X, rc.TopRight.Y);
				case GrabHandleAnchor.Inside:	return new Point(rc.TopRight.X - gb.Size.Width / 2, rc.TopRight.Y + gb.Size.Height / 2);
				case GrabHandleAnchor.Outside:
				default:						return new Point(rc.TopRight.X + gb.Size.Width / 2, rc.TopRight.Y - gb.Size.Height / 2);
			}
		}

		/// <summary>
		/// Calculate for RightCener
		/// </summary>
		/// <param name="rc"></param>
		/// <param name="gb"></param>
		/// <returns></returns>
		static Point GetRightCenterHandlerLocation(Rect rc, GrabHandleAdorner gb)
		{
			switch (gb.GrabHandleAnchor)
			{
				case GrabHandleAnchor.Centered:	return new Point(rc.Right, (rc.Top + rc.Bottom) / 2);
				case GrabHandleAnchor.Inside:	return new Point(rc.Right - gb.Size.Width / 2, (rc.Top + rc.Bottom) / 2);
				case GrabHandleAnchor.Outside:
				default:						return new Point(rc.Right + gb.Size.Width / 2, (rc.Top + rc.Bottom) / 2);
			}
		}

		/// <summary>
		/// Calculate for LeftCenter
		/// </summary>
		/// <param name="rc"></param>
		/// <param name="gb"></param>
		/// <returns></returns>
		static Point GetLeftCenterHandlerLocation(Rect rc, GrabHandleAdorner gb)
		{
			switch (gb.GrabHandleAnchor)
			{
				case GrabHandleAnchor.Centered:	return new Point(rc.Left, (rc.Top + rc.Bottom) / 2);
				case GrabHandleAnchor.Inside:	return new Point(rc.Left + gb.Size.Width / 2, (rc.Top + rc.Bottom) / 2);
				case GrabHandleAnchor.Outside:
				default:						return new Point(rc.Left - gb.Size.Width / 2, (rc.Top + rc.Bottom) / 2);
			}
		}

		/// <summary>
		/// Calculate for BottomRight
		/// </summary>
		/// <param name="rc"></param>
		/// <param name="gb"></param>
		/// <returns></returns>
		static Point GetBottomRightHandlerLocation(Rect rc, GrabHandleAdorner gb)
		{
			switch (gb.GrabHandleAnchor)
			{
				case GrabHandleAnchor.Centered:	return new Point(rc.Right, rc.Bottom);
				case GrabHandleAnchor.Inside:	return new Point(rc.BottomRight.X - gb.Size.Width / 2, rc.Bottom - gb.Size.Height / 2);
				case GrabHandleAnchor.Outside:
				default:						return new Point(rc.BottomRight.X + gb.Size.Width / 2, rc.Bottom + gb.Size.Height / 2);
			}
		}

		/// <summary>
		/// Calculate for BottomLeft
		/// </summary>
		/// <param name="rc"></param>
		/// <param name="gb"></param>
		/// <returns></returns>
		static Point GetBottomLeftHandlerLocation(Rect rc, GrabHandleAdorner gb)
		{
			switch (gb.GrabHandleAnchor)
			{
				case GrabHandleAnchor.Centered:	return new Point(rc.Left, rc.Bottom);
				case GrabHandleAnchor.Inside:	return new Point(rc.Left + gb.Size.Width / 2, rc.Bottom - gb.Size.Height / 2);
				case GrabHandleAnchor.Outside:
				default:						return new Point(rc.Left - gb.Size.Width / 2, rc.Bottom + gb.Size.Height / 2);
			}
		}

		/// <summary>
		/// Calculate for BottomCenter
		/// </summary>
		/// <param name="rc"></param>
		/// <param name="gb"></param>
		/// <returns></returns>
		static Point GetBottomCenterHandlerLocation(Rect rc, GrabHandleAdorner gb)
		{
			switch (gb.GrabHandleAnchor)
			{
				case GrabHandleAnchor.Centered:	return new Point((rc.BottomLeft.X + rc.BottomRight.X) / 2, rc.Bottom);
				case GrabHandleAnchor.Inside:	return new Point((rc.BottomLeft.X + rc.BottomRight.X) / 2, rc.Bottom - gb.Size.Height / 2);
				case GrabHandleAnchor.Outside:
				default:						return new Point((rc.BottomLeft.X + rc.BottomRight.X) / 2, rc.Bottom + gb.Size.Height / 2);				
			}
		}		
	}
}
