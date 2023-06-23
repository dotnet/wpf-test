using System;

namespace WFCTestLib.Util
{
    public enum RegionType
    {
        // <doc>
        // <desc>
        //  There is not clipping region
        // </desc>
        // </doc>
        None = 0,

        // <doc>
        // <desc>
        //  The clipping region is empty
        // </desc>
        // </doc>
        Empty = 1,

        // <doc>
        // <desc>
        //  The clipping region is a rectangle
        // </desc>
        // </doc>
        Rect = 2,

        // <doc>
        // <desc>
        //  The clipping region is a rounded rectangle
        // </desc>
        // </doc>
        RoundRect = 3,

        // <doc>
        // <desc>
        //  The clipping region is an ellipse
        // </desc>
        // </doc>
        Ellipse = 4,

        // <doc>
        // <desc>
        //  The clipping region is an "Alternate" polygon
        // </desc>
        // </doc>
        PolygonAlternate = 5,

        // <doc>
        // <desc>
        //  The clipping region is a "Winding" polygon
        // </desc>
        // </doc>
        PolygonWinding = 6,

        // <doc>
        // <desc>
        //  The clipping region is a Path
        // </desc>
        // </doc>
        Path = 7
    }
}
