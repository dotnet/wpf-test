// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace WFCTestLib.Util
{
  public class GraphicsTools
  {
    static RandomUtil s_ru = new RandomUtil();

    public GraphicsTools() : this(new Random().Next())
    {} // End ctor
      
    public GraphicsTools(int seed)
    {
      s_ru.SeedRandomGenerator(seed);
    } // End ctor

    static private float s_defaultFloatTolerance = 0.5f;
    static private double s_defaultDoubleTolerance = 0.05;


    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //  Bitmap methods
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////

	  // <doc>
    // <desc>
    //  Do a bitwise compare of two Bitmap objects
    // </desc>
    // <param term="bmp1">
    //  The first Bitmap to compare.
    // </param>
    // <param term="bmp2">
    //  The second Bitmap to compare
    // </param>
    // <param term="bmp3">
    //  An output-only Bitmap to contain the difference between bmp1 and bmp2.
    //  The differences are color coded.
    //      Color.Empty -> bmp1 and bmp2 pixels are identical
    //      Color.Green -> bmp1 has a pixel drawn where bmp2 is Color.Empty
    //      Color.Red   -> bmp2 has a pixel drawn where bmp1 is Color.Empty
    //      Color.Yellow-> both Bitmaps have different colors drawn
    // </param>
	// <retvalue>
	//  True if bmp1 and bmp2 are identical in size and contents; false otherwise
    // </retvalue>
    // </doc>
	  [SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
	  public Boolean Compare(Bitmap bmp1, Bitmap bmp2, out Bitmap bmp3)
	  {
		  return Compare(bmp1, bmp2, out bmp3, 0);
	  }

    // <doc>
    // <desc>
    //  Do a bitwise compare of two Bitmap objects
    // </desc>
    // <param term="bmp1">
    //  The first Bitmap to compare.
    // </param>
    // <param term="bmp2">
    //  The second Bitmap to compare
    // </param>
    // <param term="bmp3">
    //  An output-only Bitmap to contain the difference between bmp1 and bmp2.
    //  The differences are color coded.
    //      Color.Empty -> bmp1 and bmp2 pixels are identical
    //      Color.Green -> bmp1 has a pixel drawn where bmp2 is Color.Empty
    //      Color.Red   -> bmp2 has a pixel drawn where bmp1 is Color.Empty
    //      Color.Yellow-> both Bitmaps have different colors drawn
    // </param>
	  // <param term="tolerance">
	  //  amount bitmaps can be different to be considered an error
	// </param>
	// <retvalue>
	//  True if bmp1 and bmp2 are identical in size and contents; false otherwise
    // </retvalue>
    // </doc>
    [SecurityPermission(SecurityAction.Assert, UnmanagedCode=true)]
    public Boolean Compare(Bitmap bmp1, Bitmap bmp2, out Bitmap bmp3, float tolerance)
    {
      bmp3 = null;
  
      if ((bmp1.Width != bmp2.Width)  ||
          (bmp1.Height!= bmp2.Height))
        {
          Console.WriteLine("The bitmaps are not the same dimensions.");
          Console.WriteLine("B1.Size " + bmp1.Width.ToString() + "x" + bmp1.Height.ToString());
          Console.WriteLine("B2.Size " + bmp2.Width.ToString() + "x" + bmp2.Height.ToString());
          return false;
        }
        else if (bmp1.PixelFormat != bmp2.PixelFormat)
        {
          Console.WriteLine("The bitmaps do not have the same PixelFormat.");
          Console.WriteLine("B1.PixelFormat " + EnumTools.GetEnumStringFromValue(typeof(PixelFormat), (int)bmp1.PixelFormat));
          Console.WriteLine("B2.PixelFormat " + EnumTools.GetEnumStringFromValue(typeof(PixelFormat), (int)bmp2.PixelFormat));
          return false;
        }
        else
          bmp3 = new Bitmap(bmp1.Width, bmp1.Height, bmp1.PixelFormat);
  
        Rectangle rc = new Rectangle(0, 0, bmp1.Width, bmp1.Height); // They're all the same size
        BitmapData data1 = bmp1.LockBits(rc, ImageLockMode.ReadOnly, bmp1.PixelFormat);
        BitmapData data2 = bmp2.LockBits(rc, ImageLockMode.ReadOnly, bmp2.PixelFormat);
        BitmapData data3 = bmp3.LockBits(rc, ImageLockMode.WriteOnly, bmp3.PixelFormat);

        Int64 scan1,  scan2,  scan3;
        int color1, color2, color3;
        int height = bmp1.Height;
        int width  = bmp1.Width;
		int numPixels = 0, numDifferentPixels = 0;

		int empty = Color.Empty.ToArgb(); // Nothing has been drawn
        int red   = Color.Red.ToArgb();
        int green = Color.Green.ToArgb();
        int yellow= Color.Yellow.ToArgb();
  
        for (int y = 0; y < height; y++) 
        {
        scan1 = data1.Scan0.ToInt64() + y * data1.Stride;
        scan2 = data2.Scan0.ToInt64() + y * data2.Stride;
        scan3 = data3.Scan0.ToInt64() + y * data3.Stride;		

        for (int x = 0; x < width; x++) 
        {
			color1 = Marshal.ReadInt32((IntPtr)(scan1 + x * 4));
			color2 = Marshal.ReadInt32((IntPtr)(scan2 + x * 4));
			if (color1 == empty && color2 == empty)	// both pixels are empty
				color3 = empty;
			if (color1 == color2)			// pixels are equal.
			{
				color3 = empty;
				numPixels++;
			}
			else
			{
				numPixels++;
				numDifferentPixels++;
				if (color1 == empty)		// bmp2 has a pixel drawn where bmp1 doesn't
					color3 = red;
				else if (color2 == empty)	// bmp1 has a pixel drawn where bmp2 doesn't
					color3 = green;
				else						// they both have pixels drawn, but they aren't the same color
					color3 = yellow;
			}
			Marshal.WriteInt32((IntPtr)(scan3 + x * 4), color3);
		}
      }
      bmp1.UnlockBits(data1);
      bmp2.UnlockBits(data2);
      bmp3.UnlockBits(data3);

	  float pixelDiff = numDifferentPixels / numPixels;
	  return (pixelDiff <= tolerance);
  } // End Compare

    //
    // <doc>
    // <desc>
    public Boolean CheckPointInBitmap(Bitmap b, Point pt, Color c)
    {
      return CheckPointInBitmap(b, pt, c, "");
    } // End CheckPointInBitmap

    //
    // <doc>
    // <desc>
    public Boolean CheckPointInBitmap(Bitmap b, Point pt, Color c, String name)
    {
      Color cp = b.GetPixel(pt.X, pt.Y);
      if (Compare(cp, c))
        return true;
      else
      {
        if (name.Length>0) Console.WriteLine("Validation failed for point:: " + name);
        Console.WriteLine("    Location      : " + pt.ToString());
        Console.WriteLine("    Actual color  : " + cp.ToArgb().ToString());
        Console.WriteLine("    Expected color: " + c.ToArgb().ToString());
        return false;
      }
    } // End CheckPointInBitmap

    //
    // <doc>
    // <desc>
    public Boolean CheckPointsInBitmap(Bitmap b, Point[] pts, Color c )
    {
      Boolean b1 = true;

      for (int i=0; i< pts.Length; i++)
      {
        Boolean b2 = CheckPointInBitmap(b, pts[i], c);
        if (!b2) Console.WriteLine("  Point #" + i.ToString() + " failed validation.");
        b1 = b1 && b2;
      }
      return b1;
    } // End CheckPointsInBitmap

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //  Int/Float value methods
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////

    // 
    // <doc>
    // <desc>
    //  Return an integer angle value within +/- 360 degrees
    // </desc>
    // <retvalue>
    //  An integer value between -360 and 360
    // </retvalue>
    // </doc>
    public int GetAngle()
    { return s_ru.GetRange( -360, 360 ); }

    // 
    // <doc>
    // <desc>
    // Return a floating point angle value within +/- 360.0 degrees
    // </desc>
    // <retvalue>
    //  A float value between -360.0 and 360.0
    // </retvalue>
    // </doc>

    public float GetAngleF()
    { return s_ru.GetFloat( (float)-360.0, (float)360.0 ); }

    public Boolean Compare(double d1,double d2)
    {
	return Compare(d1, d2, s_defaultDoubleTolerance);
    }

    public Boolean Compare(double d1, double d2, double tolerance)
    {
      if (Math.Abs(d1-d2) > tolerance)
      {
        Console.WriteLine(d1.ToString() + " is not within tolerance of " + d2.ToString());
        return false;
      }
      else
        return true;
    }

    public Boolean Compare(double[] d1, double[] d2)
    {
      return Compare(d1, d2, s_defaultDoubleTolerance);
    }

    public Boolean Compare(double[] d1, double[] d2, double tolerance)
    {
      Boolean b1 = true;

      if ( (d1==null) || (d2==null) || (d1.Length!=d2.Length))
      {
        Console.WriteLine("Can't compare null or unequal arrays");
        return false;
      }

      for (int i=0; i< d1.Length; i++)
      {
        Boolean b2 = Compare(d1[i], d2[i], tolerance);
        if (!b2) Console.WriteLine("  Double #" + i.ToString() + " failed validation.");
        b1 = b1 && b2;
      }
      return b1;
    }

    public Boolean Compare(float f1,float f2)
    {
      return Compare(f1, f2, s_defaultFloatTolerance);
    }

    public Boolean Compare(float f1, float f2, float tolerance)
    {
		Console.WriteLine ("F1=" + f1.ToString ());
		Console.WriteLine ("F2=" + f2.ToString ());
		float abs = Math.Abs (f1 - f2);
		Console.WriteLine ("ABS=" + abs.ToString ());
		Console.WriteLine ("F1=" + f1.ToString ());
		Console.WriteLine ("F2=" + f2.ToString ());
		Console.WriteLine ("TOL=" + tolerance.ToString ());


      if (Math.Abs(f1-f2) > tolerance)
      {
        Console.WriteLine(f1.ToString() + " is not within tolerance of " + f2.ToString());
	Console.WriteLine("difference: " + Math.Abs(f1-f2).ToString());
        return false;
      }
      else
        return true;
    }

    public Boolean Compare(float[] f1, float[] f2)
    {
      return Compare(f1, f2, s_defaultFloatTolerance);
    }

    public Boolean Compare(float[] f1, float[] f2, float tolerance)
    {
      Boolean b1 = true;

      if ( (f1==null) || (f2==null) || (f1.Length!=f2.Length))
      {
        Console.WriteLine("Can't compare null or unequal arrays");
        return false;
      }

      for (int i=0; i< f1.Length; i++)
      {
        Boolean b2 = Compare(f1[i], f2[i], tolerance);
        if (!b2) Console.WriteLine("  Float #" + i.ToString() + " failed validation.");
        b1 = b1 && b2;
      }
      return b1;
    }
    
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //  Rectangle/Point methods
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////

    //
    // <doc>
    // <desc>
    // Find the minimum bounding Rectangle for two Points
    // </desc>
    // <param term="p1">
    //  First input Point
    // </param>
    // <param term="p2">
    //  Second input Point
    // </param>
    // <retvalue>
    //  Returns a Rectangle described by the min and max values of the input points.
    // </retvalue>
    // </doc>
    public Rectangle FindBounds(Point p1, Point p2)
    {
      int x = Math.Min(p1.X, p2.X);
      int y = Math.Min(p1.Y, p2.Y);
      int width  = Math.Abs(p1.X - p2.X + 1);
      int height = Math.Abs(p1.Y - p2.Y + 1);
      return new Rectangle(x, y, width, height);
    } // End FindBounds

    //
    // <doc>
    // <desc>
    // Find the minimum bounding Rectangle for two PointFs
    // </desc>
    // <param term="p1">
    //  First input PointF
    // </param>
    // <param term="p2">
    //  Second input PointF
    // </param>
    // <retvalue>
    //  Returns a Rectangle described by the min and max values of the input points.
    // </retvalue>
    // </doc>
    public Rectangle FindBounds(PointF p1, PointF p2)
    {
      Single x = Math.Min(p1.X, p2.X);
      Single y = Math.Min(p1.Y, p2.Y);
      Single width  = Math.Abs(p1.X - p2.X + 1);
      Single height = Math.Abs(p1.Y - p2.Y + 1);
      return new Rectangle((int)Math.Round(x), (int)Math.Round(y), (int)Math.Round(width), (int)Math.Round(height));
    } // End FindBounds

    //
    // <doc>
    // <desc>
    // Find the minimum bounding RectangleF for two PointFs
    // </desc>
    // <param term="p1">
    //  First input PointF
    // </param>
    // <param term="p2">
    //  Second input PointF
    // </param>
    // <retvalue>
    //  Returns a RectangleF described by the min and max values of the input points.
    // </retvalue>
    // </doc>
    public RectangleF FindBoundsF(PointF p1, PointF p2)
    {
      Single x = Math.Min(p1.X, p2.X);
      Single y = Math.Min(p1.Y, p2.Y);
      Single width  = Math.Abs(p1.X - p2.X + 1);
      Single height = Math.Abs(p1.Y - p2.Y + 1);
      return new RectangleF(x, y, width, height);
    } // End FindBounds

    //
    // <doc>
    // <desc>
    // Find the minimum bounding Rectangle for an array of Points
    // </desc>
    // <param term="points">
    //  Array of Points
    // </param>
    // <retvalue>
    //  Returns a Rectangle described by the min and max values of the input points.
    // </retvalue>
    // </doc>
    public Rectangle FindBounds(Point[] points)
    {
      Point max = points[0];
      Point min = points[0];

      for (int i=1; i<points.Length; i++)
      {
        max.X = Math.Max(max.X, points[i].X);
        max.Y = Math.Max(max.Y, points[i].Y);
        min.X = Math.Min(min.X, points[i].X);
        min.Y = Math.Min(min.Y, points[i].Y);
      }
      return new Rectangle(min.X, min.Y, max.X-min.X, max.Y-min.Y);
    } // End FindBounds

    //
    // <doc>
    // <desc>
    // Find the minimum bounding Rectangle for an array of PointFs
    // </desc>
    // <param term="points">
    //  Array of PointFs
    // </param>
    // <retvalue>
    //  Returns a Rectangle described by the min and max values of the input points.
    // </retvalue>
    // </doc>
    public Rectangle FindBounds(PointF[] points)
    {
      PointF max = points[0];
      PointF min = points[0];

      for (int i=1; i<points.Length; i++)
      {
        max.X = Math.Max(max.X, points[i].X);
        max.Y = Math.Max(max.Y, points[i].Y);
        min.X = Math.Min(min.X, points[i].X);
        min.Y = Math.Min(min.Y, points[i].Y);
      }
      return new Rectangle((int)min.X, (int)min.Y, (int)(max.X-min.X+0.5), (int)(max.Y-min.Y+0.5));
    } // End FindBounds

    //
    // <doc>
    // <desc>
    // Find the minimum bounding RectangleF for an array of PointFs
    // </desc>
    // <param term="points">
    //  Array of PointFs
    // </param>
    // <retvalue>
    //  Returns a RectangleF described by the min and max values of the input points.
    // </retvalue>
    // </doc>
    public RectangleF FindBoundsF(PointF[] points)
    {
      PointF max = points[0];
      PointF min = points[0];

      for (int i=1; i<points.Length; i++)
      {
        max.X = Math.Max(max.X, points[i].X);
        max.Y = Math.Max(max.Y, points[i].Y);
        min.X = Math.Min(min.X, points[i].X);
        min.Y = Math.Min(min.Y, points[i].Y);
      }
      return new RectangleF(min.X, min.Y, max.X-min.X, max.Y-min.Y);
    } // End FindBounds

    public Rectangle Intersect(Rectangle r1, Rectangle r2)
    {
      Rectangle r = new Rectangle();

      r.X     = Math.Max(r1.Left, r2.Left);
      r.Y     = Math.Max(r1.Top,  r2.Top);
      checked
      {
        try
        {
          r.Width = Math.Min(r1.Right, r2.Right) - r.Left;
          r.Height= Math.Min(r1.Bottom, r2.Bottom) - r.Top;
        }
        catch (System.ArgumentException)
        {
          return Rectangle.Empty;
        }
      }
      if (r.Width<=0 || r.Height<=0) 
        return Rectangle.Empty;

      return r;
    }
    
    // Return a contained rectangle with at least min height and width
    // If that is not possible, return Rectangle.Empty.
    public Rectangle GetContainedRectangle(Rectangle r)
    { return GetContainedRectangle(r, 0); }
    public Rectangle GetContainedRectangle(Rectangle r, int min)
    { 
      int x, y, w, h;

      if (r.Width<min || r.Height<min)
        return Rectangle.Empty;
      else
      {
        x = s_ru.GetRange(r.X, r.Right-min);
        y = s_ru.GetRange(r.Y, r.Bottom-min);
        w = s_ru.GetRange(min, r.Right-x);
        h = s_ru.GetRange(min, r.Bottom-y);
        return new Rectangle(x, y, w, h);
      }
    }
    
    public Rectangle GetIntersectingRectangle(Rectangle r)
    { return GetIntersectingRectangle(r, 0); }
    public Rectangle GetIntersectingRectangle(Rectangle r, int min)
    { 
      int x, y, w, h;
      
      if (r.Width<min || r.Height<min)
        return Rectangle.Empty;
      else
      {
        Point p1 = GetPointInRectangle(r);
	x = (int)Math.Max((long)r.X - (long)r.Width, (long)int.MinValue);
	y = (int)Math.Max((long)r.Y - (long)r.Height, (long)int.MinValue);
	w = (int)Math.Min((long)r.Width * 2, (long)int.MaxValue);
	h = (int)Math.Min((long)r.Height * 2, (long)int.MaxValue);
	
	Point p2 = GetPointInRectangle(new Rectangle(x, y, w, h));
	Rectangle rect = FindBounds(p1, p2);
	if (rect.Height == 0)
	  rect.Height = 1;
	if (rect.Width == 0)
	  rect.Width = 1;
	return rect;
      }
    }
    
    // Generate a Rectangle guarantee not to intersect with the given rectangle
    public Rectangle GetNonIntersectingRectangle(Rectangle r)
    {
      Point pt = GetPointNotInRectangle(r);

      Size max = new Size(int.MaxValue-pt.X, int.MaxValue-pt.Y);
      if (pt.X < r.Right) max.Height = (r.Y - pt.Y);
      if (pt.Y < r.Bottom) max.Width = (r.X - pt.X);

      Size sz = s_ru.GetSize(1, max.Width, 1, max.Height);      
      return new Rectangle(pt, sz);
    }

    //
    // <doc>
    // <desc>
    public Boolean Compare(RectangleF r1, RectangleF r2, float tolerance)
    {
        return ((Math.Abs(r1.X - r2.X) <= tolerance)
             && (Math.Abs(r1.Y - r2.Y) <= tolerance) 
             && (Math.Abs(r1.Width  - r2.Width) <= tolerance)
             && (Math.Abs(r1.Height - r2.Height) <= tolerance));
    } // End Compare

    //
    // <doc>
    // <desc>
    public Boolean Compare(RectangleF r1, RectangleF r2, int precision)
    {
		float tolerance;

		tolerance = (r1.X + r2.X) / 2.0f / (10 ^ precision);
		if (Math.Abs(r1.X - r2.X) > tolerance)
			return false;

		tolerance = (r1.Y + r2.Y) / 2.0f / (float)(10 ^ precision);
		if (Math.Abs(r1.Y - r2.Y) > tolerance)
			return false;

		tolerance = (r1.Width + r2.Width) / 2.0f / (float)(10 ^ precision);
		if (Math.Abs(r1.Width - r2.Width) > tolerance)
			return false;

		tolerance = (r1.Height + r2.Height) / 2.0f / (float)(10 ^ precision);
		if (Math.Abs(r1.Height - r2.Height) > tolerance)
			return false;

		return true;
    } // End Compare

    //
    // <doc>
    // <desc>
    // Return a random Point contained within the given Rectangle
    // </desc>
    // <param term="r">
    //  Bounding rectangle for the desired Point 
    // </param>
    // <retvalue>
    //  Returns a Point within the given Rectangle
    // </retvalue>
    // </doc>
    public Point GetPointInRectangle(Rectangle rc)
    {
        return new Point(s_ru.GetRange(rc.Left, rc.Right-1),
                         s_ru.GetRange(rc.Top , rc.Bottom-1));
    }

    // Generate a point guarantee not to be within a given rectangle
    public Point GetPointNotInRectangle(Rectangle r)
    {
      int horizontal;
      int vertical;
      Point pt = new Point();

      do 
      {
        horizontal = s_ru.GetRange(0, 2);
        vertical   = s_ru.GetRange(0, 2);
      } while (((vertical==1) && (horizontal==1)) ||       // Not both within rect bounds
               ((vertical==0) && (r.Left<int.MinValue+2)) || // Not less than Left when Left is min
               ((horizontal==0) && (r.Top<int.MinValue+2)) );// Not less than Top when Top is min

      switch (horizontal)
      {
      case 0:
        pt.X = s_ru.GetRange(int.MinValue, r.X-1);
        break;
      case 1:
        pt.X = s_ru.GetRange(r.X, r.Right-1);
        break;
      case 2:
        pt.X = s_ru.GetRange(r.Right, int.MaxValue);
        break;
      }
      switch (vertical)
      {
      case 0:
        pt.Y = s_ru.GetRange(int.MinValue, r.Y-1);
        break;
      case 1:
        pt.Y = s_ru.GetRange(r.Y, r.Bottom-1);
        break;
      case 2:
        pt.Y = s_ru.GetRange(r.Bottom, int.MaxValue);
        break;
      }
      return pt;
    }

    //
    // <doc>
    // <desc>
    // If the Point within the Rectangle, return true, otherwise return false.
    // </desc>
    // <param term="p">
    //  Target Point to be tested
    // </param>
    // <retvalue>
    // <param term="r">
    //  Rectangle bounds to check Point against
    // </param>
    // <retvalue>
    // Return true if the given Point is within the given Rectangle; false otherwise
    // </retvalue>
    // </doc>
    public Boolean IsPointInRectangle(Point pt, Rectangle rc)
    {
        return ((pt.X >= rc.Left) && (pt.X <  rc.Right) &&
                (pt.Y >= rc.Top)  && (pt.Y <  rc.Bottom));
    }

    //
    // <doc>
    // <desc>
    // If the Point within the RectangleF, return true, otherwise return false.
    // </desc>
    // <param term="p">
    //  Target Point to be tested
    // </param>
    // <retvalue>
    // <param term="r">
    //  RectangleF bounds to check Point against
    // </param>
    // <retvalue>
    // Return true if the given Point is within the given RectangleF; false otherwise
    // </retvalue>
    // </doc>
    public Boolean IsPointInRectangle(Point pt, RectangleF rc)
    {
        return (((float)pt.X >= rc.Left) && ((float)pt.X < rc.Right) &&
                ((float)pt.Y >= rc.Top)  && ((float)pt.Y < rc.Bottom));
    }

    //
    // <doc>
    // <desc>
    public Boolean Compare(Point p1, Point p2)
    {
        return ((p1.X==p2.X) &&
                (p1.Y==p2.Y));
    } // End Compare

    //
    // <doc>
    // <desc>
    public Boolean Compare(Point[] p1, Point[] p2)
    {
      Boolean b1 = true;

      if ((p1 == null) || (p2 == null) || (p1.Length != p2.Length))
        return false;

      for (int i=0; i< p1.Length; i++)
      {
        Boolean b2 = Compare(p1[i], p2[i]);
        if (!b2) Console.WriteLine("  Point #" + i.ToString() + " failed validation.");
        b1 &= b2;
      }
      return b1;
    } // End Compare

    //
    // <doc>
    // <desc>
    public Boolean Compare(PointF p1, PointF p2)
    {
        return ((p1.X==p2.X) &&
                (p1.Y==p2.Y));
    } // End Compare

    //
    // <doc>
    // <desc>
    public Boolean Compare(PointF[] p1, PointF[] p2)
    {
      Boolean b1 = true;

      if ((p1 == null) || (p2 == null) || (p1.Length != p2.Length))
        return false;

      for (int i=0; i< p1.Length; i++)
      {
        Boolean b2 = Compare(p1[i], p2[i]);
        if (!b2) Console.WriteLine("  Point #" + i.ToString() + " failed validation.");
        b1 &= b2;
      }
      return b1;
    } // End Compare

    //
    // <doc>
    // <desc>
    public Boolean Compare(PointF p1, PointF p2, float tolerance)
    {
        return ((Math.Abs(p1.X - p2.X) <= tolerance) &&
                (Math.Abs(p1.Y - p2.Y) <= tolerance));
    } // End Compare

    //
    // <doc>
    // <desc>
    public Boolean Compare(PointF[] p1, PointF[] p2, float tolerance)
    {
      Boolean b1 = true;

      if ((p1 == null) || (p2 == null) || (p1.Length != p2.Length))
        return false;

      for (int i=0; i< p1.Length; i++)
      {
        Boolean b2 = Compare(p1[i], p2[i], tolerance);
        if (!b2) Console.WriteLine("  Point #" + i.ToString() + " failed validation.");
        b1 &= b2;
      }
      return b1;
    } // End Compare

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //  Color methods
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////

    //
    // <doc>
    // <desc>
    public Boolean Compare(Color c1, Color c2)
    {
      Boolean retval = ((c1.A==c2.A) 
                     && (c1.R==c2.R) 
                     && (c1.G==c2.G) 
                     && (c1.B==c2.B));
      if (!retval)
      {
        Console.WriteLine("Color1: A:" + c1.A.ToString() + " R:" + c1.R.ToString() + " G:" + c1.G.ToString() + " B:" + c1.B.ToString()
                    + " != Color2: A:" + c2.A.ToString() + " R:" + c2.R.ToString() + " G:" + c2.G.ToString() + " B:" + c2.B.ToString());
      }
      return retval;
    } // End Compare

    //
    // <doc>
    // <desc>
    public Boolean Compare(Color[] c1, Color[] c2)
    {
      Boolean b1 = true;

      if ((c1 == null) || (c2 == null))
      {
        Console.WriteLine("One of the color arrays was null.");
        return false;
      }

      if (c1.Length != c2.Length)
      {
        Console.WriteLine("The color arrays are of different length.");
        Console.WriteLine("C1: " + c1.Length.ToString());
        Console.WriteLine("C2: " + c2.Length.ToString());
        return false;
      }

      for (int i=0; i< c1.Length; i++)
      {
        Boolean b2 = Compare(c1[i], c2[i]);
        if (!b2) Console.WriteLine("  Color #" + i.ToString() + " failed validation.");
        b1 &= b2;
      }
      return b1;
    } // End Compare

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //  Matrix methods
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////

    //
    // <doc>
    // <desc>
    public Boolean Compare(Matrix m1, Matrix m2) 
    {
      return Compare(m1, m2, s_defaultFloatTolerance);
    } // End Compare

    //
    // <doc>
    // <desc>
    public Boolean Compare(Matrix m1, Matrix m2, float tolerance)
    {
      Boolean retval = true;

      if (m1==null)
      {
        Console.WriteLine("Matrix1 is null.");
        return false;
      }
      else Console.WriteLine("Matrix1 is not null.");

      if (m2==null)
      {
        Console.WriteLine("Matrix2 is null.");
        return false;
      }
      else Console.WriteLine("Matrix2 is not null.");

      float[] m1Elements = m1.Elements;
      float[] m2Elements = m2.Elements;
      if (m1Elements.Length!=m2Elements.Length)
      {
        Console.WriteLine("Matrix1 has " + m1Elements.Length.ToString() + " elements.");
        Console.WriteLine("Matrix2 has " + m2Elements.Length.ToString() + " elements.");
        Console.WriteLine("These should be equal.");
        return false;
      }
      else Console.WriteLine("Matrices have same number of elements.");

      for (int i=0; i<m1Elements.Length; i++)
      {
        // If the difference between the two elements is greater than tolerance, fail
        if (Math.Abs(m1Elements[i]-m2Elements[i]) > tolerance)
        {
          Console.WriteLine("Matrix elements index " + i.ToString() + " don't match.");
          Console.WriteLine("  m1[" + i.ToString() + "] == " + m1Elements[i].ToString());
          Console.WriteLine("  m2[" + i.ToString() + "] == " + m2Elements[i].ToString());
          retval = false;
        }
      }
      if (retval) 
      {
        Console.WriteLine("All matrix elements are the same.");
        return true;
      }
      else return false;
    } // End Compare

    //
    // <doc>
    // <desc>
    public Matrix GetMatrixInt()
    {
      return GetMatrixInt(int.MinValue, int.MaxValue);
    } // End GetMatrixInt

    //
    // <doc>
    // <desc>
    public Matrix GetMatrixInt(int min, int max)
    {
      return new Matrix(s_ru.GetRange(min, max), 
                        s_ru.GetRange(min, max), 
                        s_ru.GetRange(min, max), 
                        s_ru.GetRange(min, max), 
                        s_ru.GetRange(min, max), 
                        s_ru.GetRange(min, max));
    } // End GetMatrixInt
    
    //
    // <doc>
    // <desc>
    public Matrix GetMatrixFloat()
    {
      return GetMatrixFloat(float.MinValue, float.MaxValue);
    } // End GetMatrixFloat

    //
    // <doc>
    // <desc>
    public Matrix GetMatrixFloat(float min, float max)
    {
      return new Matrix(s_ru.GetFloat(min, max), 
                        s_ru.GetFloat(min, max), 
                        s_ru.GetFloat(min, max), 
                        s_ru.GetFloat(min, max), 
                        s_ru.GetFloat(min, max), 
                        s_ru.GetFloat(min, max));
    } // End GetMatrixFloat

    public Boolean ValidateMultiply(Matrix A, Matrix B, Matrix C, MatrixOrder order)
    {
      Boolean retval = true;

      switch (order)
      {
      case MatrixOrder.Prepend:
        Console.WriteLine("Multiply Prepend");
        retval = ValidateMultiply( A, B, C);
        break;
      case MatrixOrder.Append:
        Console.WriteLine("Multiply Append");
        retval = ValidateMultiply( B, A, C);
        break;
      }

      return retval;
    } // End ValidateMultiply

    public Boolean ValidateMultiply(Matrix A, Matrix B, Matrix C)
    {
      Boolean retval;
      float[] result = new float[6];
      
      result[0] = A.Elements[0] * B.Elements[0] + A.Elements[2]*B.Elements[1];
      result[1] = A.Elements[1] * B.Elements[0] + A.Elements[3]*B.Elements[1];
      result[2] = A.Elements[0] * B.Elements[2] + A.Elements[2]*B.Elements[3];
      result[3] = A.Elements[1] * B.Elements[2] + A.Elements[3]*B.Elements[3];
      result[4] = A.Elements[0] * B.Elements[4] + A.Elements[2]*B.Elements[5] + A.Elements[4];
      result[5] = A.Elements[1] * B.Elements[4] + A.Elements[3]*B.Elements[5] + A.Elements[5];
      retval = Compare(result, C.Elements);
      if (!retval)
      {
        ToConsole("Matrix A : ", A.Elements);
        Console.WriteLine("Multiplied by ...");
        ToConsole("Matrix B : ", B.Elements);
        Console.WriteLine("Should equal ...");
        ToConsole("NewMatrix: ", result);
        Console.WriteLine("Actual value returned ...");
        ToConsole("Matrix C: ", C.Elements);
      }
      return retval;
    }
    
    public Boolean ValidateTranslate(Matrix A, PointF offset, Matrix C)
    {
      Boolean retval;
      float[] result = new float[6];
      
      result[0] = A.Elements[0];
      result[1] = A.Elements[1];
      result[2] = A.Elements[2];
      result[3] = A.Elements[3];
      result[4] = offset.X * A.Elements[0] + offset.Y * A.Elements[2] + A.Elements[4];
      result[5] = offset.Y * A.Elements[3] + offset.X * A.Elements[1] + A.Elements[5];
      retval = Compare(result, C.Elements);
      if (!retval)
      {
        ToConsole("Matrix A : ", A.Elements);
        Console.WriteLine("Translated (Prepend) by ...");
        Console.WriteLine("Offset   : " +  (Point.Truncate(offset)).ToString());
        Console.WriteLine("Should equal ...");
        ToConsole("NewMatrix: ", result);
        Console.WriteLine("Actual value returned ...");
        ToConsole("Matrix C: ", C.Elements);
      }
      return retval;
    }

    public Boolean ValidateTranslate(Matrix A, PointF offset, Matrix C, MatrixOrder mo)
    {
      switch (mo)
      {
      case MatrixOrder.Prepend:
        return ValidateTranslate(A, offset, C);
      case MatrixOrder.Append:
        Boolean retval = false;
        float[] result = new float[6];

        result[0] = A.Elements[0];
        result[1] = A.Elements[1];
        result[2] = A.Elements[2];
        result[3] = A.Elements[3];
        result[4] = offset.X + A.Elements[4];
        result[5] = offset.Y + A.Elements[5];
        retval = Compare(result, C.Elements);
        if (!retval)
        {
          ToConsole("Matrix A : ", A.Elements);
          Console.WriteLine("Translated (Append) by ...");
          Console.WriteLine("Offset   : " + (Point.Truncate(offset)).ToString());
          Console.WriteLine("Should equal ...");
          ToConsole("NewMatrix: ", result);
          Console.WriteLine("Actual value returned ...");
          ToConsole("Matrix C: ", C.Elements);
        }
        return retval;
      }
      return false;
    }

    public Boolean ValidateScale(Matrix A, PointF scale, Matrix C)
    {
      Boolean retval;
      float[] result = new float[6];

      result[0] = A.Elements[0] * scale.X;
      result[1] = A.Elements[1] * scale.X;
      result[2] = A.Elements[2] * scale.Y;
      result[3] = A.Elements[3] * scale.Y;
      result[4] = A.Elements[4];
      result[5] = A.Elements[5];

      retval = Compare(result, C.Elements);
      if (!retval)
      {
        ToConsole("Matrix A : ", A.Elements);
        Console.WriteLine("Scaled (Prepend) by ...");
        Console.WriteLine("Scale    : " + (Point.Truncate(scale)).ToString());
        Console.WriteLine("Should equal ...");
        ToConsole("NewMatrix: ", result);
        Console.WriteLine("Actual value returned ...");
        ToConsole("Matrix C: ", C.Elements);
      }
      return retval;
    }

    public Boolean ValidateScale(Matrix A, PointF scale, Matrix C, MatrixOrder mo)
    {
      switch (mo)
      {
      case MatrixOrder.Prepend:
        return ValidateScale(A, scale, C);
      case MatrixOrder.Append:
        Boolean retval;
        float[] result = new float[6];
        result[0] = A.Elements[0] * scale.X;
        result[1] = A.Elements[1] * scale.Y;
        result[2] = A.Elements[2] * scale.X;
        result[3] = A.Elements[3] * scale.Y;
        result[4] = A.Elements[4] * scale.X;
        result[5] = A.Elements[5] * scale.Y;

        retval = Compare(result, C.Elements);
        if (!retval)
        {
          ToConsole("Matrix A : ", A.Elements);
          Console.WriteLine("Scaled (Append) by ...");
          Console.WriteLine("Scale    : " + (Point.Truncate(scale)).ToString());
          Console.WriteLine("Should equal ...");
          ToConsole("NewMatrix: ", result);
          Console.WriteLine("Actual value returned ...");
          ToConsole("Matrix C: ", C.Elements);
        }
        return retval;
      }
      return false;
    }

    public Boolean ValidateRotate(Matrix A, float angle, Matrix C, float tolerance)
    {
      Boolean retval;
      float sx = (float)Math.Cos(angle / (180.0f / Math.PI));
      float sy = (float)Math.Sin(angle / (180.0f / Math.PI));
      float[] B = new float[6];

      B[0] = A.Elements[0] * sx + A.Elements[2] * sy;
      B[1] = A.Elements[1] * sx + A.Elements[3] * sy;
      B[2] = A.Elements[2] * sx - A.Elements[0] * sy;
      B[3] = A.Elements[3] * sx - A.Elements[1] * sy;
      B[4] = A.Elements[4];
      B[5] = A.Elements[5];
      retval = Compare(C.Elements, B, tolerance);

      if (!retval)
      {
        ToConsole("Matrix A : ", A.Elements);
        Console.WriteLine("Rotated (Prepend) by ...");
        Console.WriteLine("Angle    : " + angle.ToString());
        Console.WriteLine("Should equal ...");
        ToConsole("NewMatrix: ", B);
        Console.WriteLine("Actual value returned ...");
        ToConsole("Matrix C: ", C.Elements);
      }
      return retval;
    }

    public Boolean ValidateRotate(Matrix A, float angle, Matrix C, float tolerance, MatrixOrder mo)
    {
      switch (mo)
      {
      case MatrixOrder.Prepend:
        return ValidateRotate(A, angle, C, tolerance);
      case MatrixOrder.Append:
        Boolean retval;

        float sx = (float)Math.Cos(angle / (180.0f / Math.PI));
        float sy = (float)Math.Sin(angle / (180.0f / Math.PI));
        float[] B = new float[6];
        B[0] = A.Elements[0] * sx - A.Elements[1] * sy;
        B[1] = A.Elements[1] * sx + A.Elements[0] * sy;
        B[2] = A.Elements[2] * sx - A.Elements[3] * sy;
        B[3] = A.Elements[3] * sx + A.Elements[2] * sy;
        B[4] = A.Elements[4] * sx - A.Elements[5] * sy;
        B[5] = A.Elements[5] * sx + A.Elements[4] * sy;
        retval = Compare(C.Elements, B, tolerance);
        if (!retval)
        {
          ToConsole("Matrix A : ", A.Elements);
          Console.WriteLine("Rotated (Append) by ...");
          Console.WriteLine("Angle    : " + angle.ToString());
          Console.WriteLine("Should equal ...");
          ToConsole("NewMatrix: ", B);
          Console.WriteLine("Actual value returned ...");
          ToConsole("Matrix C: ", C.Elements);
        }
        return retval;
      }
      return false;
    }

    public Boolean ValidateRotateAt(Matrix A, float angle, PointF pt, Matrix C, float tolerance)
    {
      return ValidateRotateAt(A, angle, pt, C, tolerance, MatrixOrder.Prepend);
    }

    public Boolean ValidateRotateAt(Matrix A, float angle, PointF pt, Matrix C, float tolerance, MatrixOrder mo)
    {
      Matrix B = A.Clone();

      switch (mo)
      {
      case MatrixOrder.Append:
        Console.WriteLine("RotateAt (Append)");
        B.Translate(-pt.X, -pt.Y, mo);
        B.Rotate(angle, mo);
        B.Translate(pt.X, pt.Y, mo);
        break;
      case MatrixOrder.Prepend:
        Console.WriteLine("RotateAt (Prepend)");
        B.Translate(pt.X, pt.Y, mo);
        B.Rotate(angle, mo);
        B.Translate(-pt.X, -pt.Y, mo);
        break;
      }

      Boolean retval = Compare(C.Elements, B.Elements, tolerance);
      if (!retval)
      {
        ToConsole("Matrix A : ", A.Elements);
        Console.WriteLine("Rotated by ...");
        Console.WriteLine("Angle    : " + angle.ToString());
        Console.WriteLine("Around Point ...");
        Console.WriteLine("Point    : " + (Point.Truncate(pt)).ToString());
        Console.WriteLine("Should equal ...");
        ToConsole("NewMatrix: ", B.Elements);
        Console.WriteLine("Actual value returned ...");
        ToConsole("Matrix C: ", C.Elements);
      }
      return retval;
    }
    
    public Boolean ValidateShear(Matrix A, PointF shear, Matrix C, float tolerance)
    {
      float[] B = new float[6];
      B[0] = shear.Y * A.Elements[2]+ A.Elements[0];
      B[1] = shear.Y * A.Elements[3]+ A.Elements[1];
      B[2] = shear.X * A.Elements[0]+ A.Elements[2];
      B[3] = shear.X * A.Elements[1]+ A.Elements[3];
      B[4] = A.Elements[4];
      B[5] = A.Elements[5];

      Boolean retval = Compare(C.Elements, B, tolerance);
      if (!retval)
      {
        ToConsole("Matrix A : ", A.Elements);
        Console.WriteLine("Sheared (Append) by ...");
        Console.WriteLine("PointF   : " + (Point.Truncate(shear)).ToString());
        Console.WriteLine("Should equal ...");
        ToConsole("NewMatrix: ", B);
        Console.WriteLine("Actual value returned ...");
        ToConsole("Matrix C: ", C.Elements);
      }
      return retval;
    }

    public Boolean ValidateShear(Matrix A, PointF shear, Matrix C, float tolerance, MatrixOrder mo)
    {
      switch (mo)
      {
      case MatrixOrder.Prepend:
        return ValidateShear(A, shear, C, tolerance);
      case MatrixOrder.Append:
        float[] B = new float[6];
        B[0] = shear.X * A.Elements[1] + A.Elements[0];
        B[1] = shear.Y * A.Elements[0] + A.Elements[1];
        B[2] = shear.X * A.Elements[3] + A.Elements[2];
        B[3] = shear.Y * A.Elements[2] + A.Elements[3];
        B[4] = shear.X * A.Elements[5] + A.Elements[4];
        B[5] = shear.Y * A.Elements[4] + A.Elements[5];

        Boolean retval = Compare(C.Elements, B, tolerance);
        if (!retval)
        {
          ToConsole("Matrix A : ", A.Elements);
          Console.WriteLine("Sheared (Prepend) by ...");
          Console.WriteLine("PointF   : " + (Point.Truncate(shear)).ToString());
          Console.WriteLine("Should equal ...");
          ToConsole("NewMatrix: ", B);
          Console.WriteLine("Actual value returned ...");
          ToConsole("Matrix C: ", C.Elements);
        }
        return retval;
      }
      return false;
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //  Font methods
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////

    public Boolean Compare(Font f1, Font f2, Boolean fAdjustGraphicsUnits)
    {
      if (!(f1.Name.Equals(f2.Name)
         && f1.Style.Equals(f2.Style)) )
      {
        ToConsole(f1);
        ToConsole(f2);
        Console.WriteLine("Font settings are not the same.");
        return false;
      }

      if (fAdjustGraphicsUnits)
      {
        if (Compare(f1.SizeInPoints, f2.SizeInPoints, 0.5f))
          return true;
        else
        {
          ToConsole(f1);
          ToConsole(f2);
          Console.WriteLine("Font are not equivalent sizes after adjusting for GraphicsUnit.");
          Console.WriteLine("Font1 Size and Unit: " + f1.Size.ToString() + " " + EnumTools.GetEnumStringFromValue(typeof(GraphicsUnit), (int)f1.Unit));
          Console.WriteLine("Font2 Size and Unit: " + f2.Size.ToString() + " " + EnumTools.GetEnumStringFromValue(typeof(GraphicsUnit), (int)f2.Unit));
          Console.WriteLine("Font1.SizeInPoints: " + f1.SizeInPoints.ToString());
          Console.WriteLine("Font2.SizeInPoints: " + f2.SizeInPoints.ToString());
          return false;
        }
      }
      else 
      {
        if (!f1.Equals(f2))
        {
          ToConsole(f1);
          ToConsole(f2);
          Console.WriteLine("Font settings are not the same.");
          return false;
        }
        else
          return true;
      }
    }

    public Font GetRandomFont()
    { 
      FontFamily   ff;
      float        es;
      FontStyle    fs;
      GraphicsUnit gu;

      InstalledFontCollection ifc = new InstalledFontCollection();
      ff = ifc.Families[s_ru.GetRange(0, ifc.Families.Length-1)];

      es = s_ru.GetRange(1, 144);
      fs = (FontStyle)s_ru.GetEnumValue(typeof(FontStyle), true);
      if (ff.Name == "Monotype Corsiva") // If Monotype Corsiva is not Italic, it causes exceptions.
        fs |= FontStyle.Italic;

      gu = (GraphicsUnit)s_ru.GetDifferentEnumValue(typeof(GraphicsUnit), (int)GraphicsUnit.Display);
      
      Font f = new Font(ff, es, fs, gu);

      ToConsole(f);

      return f;
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //  Console output methods
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    public void ToConsole(Font f)
    {
      Console.WriteLine("FontFamily : " + f.FontFamily.ToString());
      Console.WriteLine("Size       : " + f.Size.ToString());

      // Mask off individual bits from global FontStyle
      Boolean isBold      = ((int)f.Style & (int)FontStyle.Bold     ) > 0;
      Boolean isItalic    = ((int)f.Style & (int)FontStyle.Italic   ) > 0;
      Boolean isUnderline = ((int)f.Style & (int)FontStyle.Underline) > 0;
      Boolean isStrikeout = ((int)f.Style & (int)FontStyle.Strikeout) > 0;
      Console.WriteLine("FontStyle bits ..." + f.Style.ToString());
      if (isBold)      Console.WriteLine("Bold.");
      if (isItalic)    Console.WriteLine("Italic.");
      if (isUnderline) Console.WriteLine("Underline.");
      if (isStrikeout) Console.WriteLine("Strikeout.");
      if (! (isBold || isItalic || isUnderline || isStrikeout)) Console.WriteLine("Regular.");

      Console.WriteLine("Unit : " + EnumTools.GetEnumStringFromValue(typeof(GraphicsUnit), (int)f.Unit));
      Console.WriteLine("Font: " + f.ToString());
    }

    public void ToConsole(String s, float[] f)
    {
      Console.WriteLine(s);
      for (int i=0; i<f.Length; i++)
        Console.WriteLine(f[i].ToString());
    } // End ToConsole

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //  Brush and Pen methods
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    public Boolean Compare(PathGradientBrush br1, PathGradientBrush br2)
    {
      Boolean retval = true;

      if (!Compare(br1.CenterColor, br2.CenterColor))
      {
        Console.WriteLine("CenterColors don't match.");
        Console.WriteLine("br1.CenterColor " + br1.CenterColor.ToString());
        Console.WriteLine("br2.CenterColor " + br2.CenterColor.ToString());
        retval = false;
      }
      if (!Compare(br1.SurroundColors, br2.SurroundColors))
      {
        Console.WriteLine("SurroundColors don't match.");
        Console.WriteLine("br1.SurroundColors " + br1.SurroundColors.ToString());
        Console.WriteLine("br2.SurroundColors " + br2.SurroundColors.ToString());
        retval = false;
      }
      if (br1.Rectangle != br2.Rectangle)
      {
        Console.WriteLine("Rectangles don't match.");
        Console.WriteLine("br1.Rectangle " + br1.Rectangle.ToString());
        Console.WriteLine("br2.Rectangle " + br2.Rectangle.ToString());
        retval = false;
      }
      if ( (br1.InterpolationColors != null) 
       &&  (br2.InterpolationColors != null) )
      if (!(Compare(br1.InterpolationColors.Colors,    br2.InterpolationColors.Colors))
       &&  (Compare(br1.InterpolationColors.Positions, br2.InterpolationColors.Positions)) )
      {
        Console.WriteLine("InterpolationColors.Color and Position values don't match.");
        retval = false;
      }
      if (br1.WrapMode != br2.WrapMode)
      {
        Console.WriteLine("WrapModes don't match.");
        Console.WriteLine("br1.WrapMode " + br1.WrapMode.ToString());
        Console.WriteLine("br2.WrapMode " + br2.WrapMode.ToString());
        retval = false;
      }
      if (!Compare(br1.Transform, br2.Transform))
      {
        Console.WriteLine("Transforms don't match.");
        Console.WriteLine("br1.Transforms " + br1.Transform.ToString());
        Console.WriteLine("br2.Transforms " + br2.Transform.ToString());
        retval = false;
      }
      return retval;
    } // End Compare

  } // End class GraphicsTools 
} // End namespace GraphicsTestLib
