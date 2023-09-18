// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test all public API's in the DrawingBrush class
//
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class WCP_DrawingBrushClass : ApiTest
    {
        public WCP_DrawingBrushClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(DrawingBrush);
            _helper = new HelperClass();
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus(_objectType.ToString());

            // Test variables
            Type BaseClassType = typeof(Brush);
            Type ParentClassType = typeof(TileBrush);

            // Animations to be used for testing Properties
            RectAnimation AnimRect = new RectAnimation(new Rect(new Point(50, 50), new Point(150, 150)), new System.Windows.Duration(TimeSpan.FromMilliseconds(1000)));
            DoubleAnimation AnimDouble = new DoubleAnimation(0, 1.0, new System.Windows.Duration(TimeSpan.FromMilliseconds(0)), FillBehavior.Stop);
            Transform AnimTransform = new TranslateTransform(0, 0);
            AnimTransform.BeginAnimation(TranslateTransform.XProperty, AnimDouble);

            #region SECTION I - CONSTRUCTORS
            CommonLib.LogStatus("***** SECTION I - CONSTRUCTORS *****");

            #region Test #1 - Default Constructor
            // Usage: DrawingBrush()
            // Notes: Default Constructor creates an empty DrawingBrush.
            CommonLib.LogStatus("Test #1 - Default Constructor");

            // Create an DrawingBrush 
            DrawingBrush DB11 = new DrawingBrush();

            // Confirm that a DrawingBrush was created successfully.
            _class_testresult &= _helper.CheckType(DB11, _objectType);
            #endregion

            #region Test #2 - Constructor with a Drawing
            // Usage: DrawingBrush(Drawing)
            // Notes: Returns a DrawingBrush constructed using a Drawing.
            CommonLib.LogStatus("Test #2 - Constructor with a Drawing");

            // Create a Drawing 
            DrawingGroup D12 = new DrawingGroup();
            DrawingContext dc12 = D12.Open();
            dc12.DrawRectangle(Brushes.Red, null, new System.Windows.Rect(10, 10, 110, 60));
            dc12.Close();

            // Create a DrawingBrush 
            DrawingBrush DBprop = new DrawingBrush(D12);

            // Confirm that a DrawingBrush was created successfully.
            _class_testresult &= _helper.CheckType(DBprop, _objectType);
            #endregion
            #endregion End Of SECTION I

            #region SECTION II - PROPERTIES
            CommonLib.LogStatus("***** SECTION II - PROPERTIES *****");

            #region Test #1 - The Drawing Property
            // Usage: Drawing = DrawingBrush.Drawing (R/W)
            // Notes: Gets or Sets the Drawing that is contained in the Brush.
            CommonLib.LogStatus("Test #1 - The Drawing Property");

            // Create a Drawing 
            DrawingGroup NewDrawing = new DrawingGroup();
            DrawingContext dc21 = NewDrawing.Open();
            dc21.DrawGeometry(Brushes.Blue, null, new RectangleGeometry(new Rect(10, 10, 150, 150), 90, 90));
            dc21.DrawGeometry(Brushes.White, null, new RectangleGeometry(new Rect(40, 40, 90, 90), 90, 90));
            dc21.DrawGeometry(Brushes.Red, null, new RectangleGeometry(new Rect(70, 70, 30, 30), 90, 90));
            dc21.Close();

            // Set the Drawing property.
            DBprop.Drawing = NewDrawing;

            DrawingBrush DBNull = DBprop.Clone();
            DBNull.Drawing = null;
            DC.DrawRectangle(DBNull, null, new System.Windows.Rect(m_top, m_left, m_width, m_height));

            // Get the Drawing.Bounds value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("Drawing", NewDrawing.Bounds, DBprop.Drawing.Bounds);
            #endregion

            #region Test #2 - The ViewboxUnits Property
            // Usage: BrushMappingMode = DrawingBrush.ViewboxUnits (R/W)
            // Notes: Gets or Sets the BrushMappingMode that indicates whether the portion of the content
            // described by the Viewbox will be in Absolute coordinates or in coordinates relative to the content.
            CommonLib.LogStatus("Test #2 - The ViewboxUnits Property");

            // Set the ViewboxUnits property.
            DBprop.ViewboxUnits = BrushMappingMode.Absolute;

            // Get the ViewboxUnits value as an Int to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("ViewboxUnits", (int)DBprop.ViewboxUnits, (int)BrushMappingMode.Absolute);
            #endregion

            #region Test #3 - The AlignmentX Property
            // Usage: AlignmentX = DrawingBrush.AlignmentX (R/W)
            // Notes: Gets or Sets a AlignmentX that indicates how the Viewbox is aligned
            // horizontally in the Viewport.
            CommonLib.LogStatus("Test #3 - The AlignmentX Property");

            // Set the AlignmentX property.
            DBprop.AlignmentX = AlignmentX.Left;

            // Get the AlignmentX value as an Int to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("AlignmentX", (int)DBprop.AlignmentX, (int)AlignmentX.Left);
            #endregion

            #region Test #4 - The Stretch Property
            // Usage: Stretch = DrawingBrush.Stretch (R/W)
            // Notes: Gets or Sets a Stretch that indicates how the Viewbox will be stretched to fit the Viewport.
            CommonLib.LogStatus("Test #4 - The Stretch Property");

            // Set the Stretch property.
            DBprop.Stretch = Stretch.Uniform;

            // Get the Stretch value as an Int to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("Stretch", (int)DBprop.Stretch, (int)Stretch.Uniform);
            #endregion

            #region Test #5 - The TileMode Property
            // Usage: TileMode = DrawingBrush.TileMode (R/W)
            // Notes: Gets or Sets a TileMode that indicates how the Viewbox will be Tiled in the Viewport.
            CommonLib.LogStatus("Test #5 - The TileMode Property");

            // Set the TileMode property.
            DBprop.TileMode = TileMode.Tile;

            // Get the TileMode value as an Int to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("TileMode", (int)DBprop.TileMode, (int)TileMode.Tile);
            #endregion

            #region Test #6 - The AlignmentY Property
            // Usage: AlignmentY = DrawingBrush.AlignmentY (R/W)
            // Notes: Gets or Sets a AlignmentY that indicates how the Viewbox is aligned
            // Vertically in the Viewport.
            CommonLib.LogStatus("Test #6 - The AlignmentY Property");

            // Set the AlignmentY property.
            DBprop.AlignmentY = AlignmentY.Top;

            // Get the AlignmentY value as an Int to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("AlignmentY", (int)DBprop.AlignmentY, (int)AlignmentY.Top);
            #endregion

            #region Test #7 - The Viewbox Property
            // Usage: Rect = DrawingBrush.Viewbox (R/W)
            // Notes: Gets or Sets a Rectangle that describes the bounds of the Viewbox
            // (the content that is being mapped from).
            CommonLib.LogStatus("Test #7 - The Viewbox Property");

            // Set the Viewbox property.
            Rect rect27 = new Rect(new Point(0.0, 0.0), new Point(200.0, 200.0));
            DBprop.Viewbox = rect27;

            // Get the Viewbox value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("Viewbox", DBprop.Viewbox, rect27);

            // Add a Rect Animation to the Brush
            DBprop.BeginAnimation(DrawingBrush.ViewboxProperty, AnimRect);

            // Animated Viewbox Width and Height should be between 0,0 and the Width and Height of the Rect that was set.
            _class_testresult &= _helper.CompareRectWithRange("Viewbox (With Animation)", DBprop.Viewbox, 0, 200);
            #endregion

            #region Test #8 - The Viewport Property
            // Usage: Rect = DrawingBrush.Viewport (R/W)
            // Notes: Gets or Sets a Rectangle that describes the bounds of the Viewport
            // (the area that is being mapped into)
            CommonLib.LogStatus("Test #8 - The Viewport Property");

            // Set the Viewport property.
            Rect rect28 = new Rect(new Point(0.0, 0.0), new Point(0.5, 0.5));

            DBprop.Viewport = rect28;

            // Get the Viewport value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("Viewport", DBprop.Viewport, rect28);

            // Add a Rect Animation to the Brush
            DBprop.BeginAnimation(DrawingBrush.ViewportProperty, AnimRect);

            // Animated Viewport Width and Height should be between 0,0 and the Width and Height of the Rect that was set.
            _class_testresult &= _helper.CompareRectWithRange("Viewport (With Animation)", DBprop.Viewport, 0, 200);
            #endregion

            #region Test #9 - The ViewportUnits Property
            // Usage: BrushMappingMode = DrawingBrush.ViewportUnits (R/W)
            // Notes: Gets or Sets the BrushMappingMode that indicates whether the Viewport
            // will map it's coordinates to the entire user space or just to the shape that
            // is being filled.
            CommonLib.LogStatus("Test #9 - The ViewportUnits Property");

            // Set the ViewportUnits property.
            DBprop.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;

            // Get the ViewportUnits value as an Int to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("ViewportUnits", (int)DBprop.ViewportUnits, (int)BrushMappingMode.RelativeToBoundingBox);
            #endregion

            #region Test #10 - The Opacity Property
            // Usage: double = DrawingBrush.Opacity (R/W)
            // Notes: Gets or Sets the Opacity value of the Brush.
            CommonLib.LogStatus("Test #10 - The Opacity Property");

            // Set the Opacity property.
            DBprop.Opacity = 0.9;

            // Get the Opacity value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("Opacity", DBprop.Opacity, 0.9);

            // Add an Opacity Animation to the Brush
            DBprop.BeginAnimation(DrawingBrush.OpacityProperty, AnimDouble);

            // Get the Opacity value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("Opacity (With Animation)", DBprop.Opacity, 0.9);
            #endregion

            #region Test #11 - The Transform Property
            // Usage: Transform = DrawingBrush.Transform (R/W)
            // Notes: Gets or Sets the Transform for the Brush.
            CommonLib.LogStatus("Test #11 - The Transform Property");

            // Set the Transform property.
            DBprop.Transform = new TranslateTransform(30, 30);

            // Get the Transform value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareMatrix("Transform", DBprop.Transform, 1, 0, 0, 1, 30, 30);

            // Set the Transform property to an Animated Transform.
            DBprop.Transform = AnimTransform;

            // Get the Transform value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareMatrix("Transform (With Animation)", DBprop.Transform, 1, 0, 0, 1, 0, 0);
            #endregion

            #region Test #12 - The RelativeTransform Property
            // Usage: Transform = DrawingBrush.RelativeTransform (R/W)
            // Notes: Gets or Sets the Transform for the Brush relative to the object it is filling.
            CommonLib.LogStatus("Test #12 - The RelativeTransform Property");

            // Use the RelativeTransform property to set the Brush Transform relative to the object it is filling.
            DBprop.RelativeTransform = new TranslateTransform(.05, .05);

            // Get the RelativeTransform value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareMatrix("RelativeTransform", DBprop.RelativeTransform, 1, 0, 0, 1, .05, .05);

            // Set the RelativeTransform property to an Animated Transform.
            DBprop.RelativeTransform = AnimTransform;

            // Get the RelativeTransform value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareMatrix("RelativeTransform (With Animation)", DBprop.RelativeTransform, 1, 0, 0, 1, 0, 0);
            #endregion

            #region Test #13 - The CanFreeze Property
            // Usage: bool = DrawingBrush.CanFreeze (Read Only)
            // Notes: Returns a bool that indicates whether the Brush is animatable.
            CommonLib.LogStatus("Test #13 - The CanFreeze Property");

            // Create a Drawing 
            DrawingGroup D213 = new DrawingGroup();
            DrawingContext dc213 = D213.Open();
            dc213.DrawRectangle(Brushes.Yellow, null, new System.Windows.Rect(10, 10, 60, 60));
            dc213.Close();

            // Create a DrawingBrush 
            DrawingBrush DB213 = new DrawingBrush(D213);

            // Check the CanFreeze value of a non-animated Brush.
            _class_testresult &= _helper.CompareProp("CanFreeze", DB213.CanFreeze, true);

            // Add an Animation to the Brush
            DB213.BeginAnimation(DrawingBrush.ViewportProperty, AnimRect);

            // Check the CanFreeze value of an animated Brush.
            _class_testresult &= _helper.CompareProp("CanFreeze (Animated Brush)", DB213.CanFreeze, false);
            #endregion

            #region Test #14 - Basic rendering with a DrawingBrush
            // Just fill the surface with a DrawingBrush to see if we crash or not.
            CommonLib.LogStatus("Test #14 - Basic rendering with a DrawingBrush");

            // Clear all Animations before rendering.
            DrawingBrush DBRender = DBprop.Clone();
            DBRender.BeginAnimation(DrawingBrush.ViewboxProperty, null);
            DBRender.BeginAnimation(DrawingBrush.ViewportProperty, null);
            DBRender.BeginAnimation(DrawingBrush.OpacityProperty, null);
            DC.DrawRectangle(DBRender, null, new System.Windows.Rect(m_top, m_left, m_width, m_height));
            #endregion
            #endregion End Of SECTION II

            #region SECTION III - METHODS
            CommonLib.LogStatus("***** SECTION III - METHODS *****");

            #region Test #1 - The Copy Method
            // Usage: DrawingBrush = DrawingBrush.Clone()
            // Notes: Returns a DrawingBrush that is a copy of this DrawingBrush
            CommonLib.LogStatus("Test #1 - The Copy Method");

            // Create a Drawing 
            DrawingGroup D31 = new DrawingGroup();
            DrawingContext dc31 = D31.Open();
            dc31.DrawRectangle(Brushes.Red, null, new System.Windows.Rect(10, 10, 110, 60));
            dc31.DrawRectangle(Brushes.Blue, null, new System.Windows.Rect(50, 50, 150, 90));

            // Create a DrawingBrush 
            DrawingBrush DB31a = new DrawingBrush(D31);

            // Use the Copy method to create a new DrawingBrush that has the same value.
            DrawingBrush DB31b = DB31a.Clone();

            // Check the DrawingBrush properties for equality.
            _class_testresult &= _helper.CompareDrawingBrushProperties("Copy - ", DB31a, DB31b);
            #endregion

            #region Test #2 - The CloneCurrentValue Method
            // Usage: DrawingBrush = DrawingBrush.CloneCurrentValue()
            // Notes: Returns a new Brush that has the same CurrentValue as this Brush
            CommonLib.LogStatus("Test #2 - The CloneCurrentValue Method");

            // Create a Drawing 
            DrawingGroup D32 = new DrawingGroup();
            DrawingContext dc32 = D32.Open();

            dc32.DrawRectangle(Brushes.Yellow, null, new System.Windows.Rect(10, 10, 110, 110));
            dc32.DrawRectangle(Brushes.Blue, null, new System.Windows.Rect(50, 50, 150, 90));

            // Create a DrawingBrush 
            DrawingBrush DB32a = new DrawingBrush(D32);

            // Use the CloneCurrentValue method to create a new DrawingBrush that has the same current value.
            DrawingBrush DB32b = (DrawingBrush)DB32a.CloneCurrentValue();

            // Check the DrawingBrush properties for equality.
            _class_testresult &= _helper.CompareDrawingBrushProperties("CloneCurrentValue - ", DB32a, DB32b);
            #endregion
            #endregion End Of SECTION III

            #region SECTION IV - STATIC PROPERTIES
            CommonLib.LogStatus("***** SECTION IV - STATIC PROPERTIES *****");

            #region Test #1 - The DrawingProperty Property
            // Usage: DependencyProperty = DrawingBrush.DrawingProperty (Read only)
            // Notes: Returns a Dependency Property for Drawing.
            CommonLib.LogStatus("Test #1 - The DrawingProperty Property");

            System.Windows.DependencyProperty DrawingDp = DrawingBrush.DrawingProperty;

            _class_testresult &= _helper.CompareProp("DrawingProperty", (DrawingDp.OwnerType.ToString() + "::" + DrawingDp.PropertyType.ToString()), (_objectType + "::" + typeof(Drawing).ToString()));
            #endregion

            #region Test #2 - This test intentionally left blank (was ContentUnitsProperty)
            #endregion

            #region Test #3 - The AlignmentXProperty Property
            // Usage: DependencyProperty = DrawingBrush.AlignmentXProperty (Read only)
            // Notes: Returns a Dependency Property for AlignmentX.
            CommonLib.LogStatus("Test #3 - The AlignmentXProperty Property");

            System.Windows.DependencyProperty AlignmentXDp = DrawingBrush.AlignmentXProperty;

            _class_testresult &= _helper.CompareProp("AlignmentXProperty", (AlignmentXDp.OwnerType.ToString() + "::" + AlignmentXDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(AlignmentX).ToString()));
            #endregion

            #region Test #4 - The StretchProperty Property
            // Usage: DependencyProperty = DrawingBrush.StretchProperty (Read only)
            // Notes: Returns a Dependency Property for Stretch
            CommonLib.LogStatus("Test #4 - The StretchProperty Property");

            System.Windows.DependencyProperty StretchDp = DrawingBrush.StretchProperty;

            _class_testresult &= _helper.CompareProp("StretchProperty", (StretchDp.OwnerType.ToString() + "::" + StretchDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(Stretch).ToString()));
            #endregion

            #region Test #5 - The TileModeProperty Property
            // Usage: DependencyProperty = DrawingBrush.TileModeProperty (Read only)
            // Notes: Returns a Dependency Property for TileMode
            CommonLib.LogStatus("Test #5 - The TileModeProperty Property");

            System.Windows.DependencyProperty TileModeDp = DrawingBrush.TileModeProperty;

            _class_testresult &= _helper.CompareProp("TileModeProperty", (TileModeDp.OwnerType.ToString() + "::" + TileModeDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(TileMode).ToString()));
            #endregion

            #region Test #6 - The AlignmentYProperty Property
            // Usage: DependencyProperty = DrawingBrush.AlignmentYProperty (Read only)
            // Notes: Returns a Dependency Property for AlignmentY
            CommonLib.LogStatus("Test #6 - The AlignmentYProperty Property");

            System.Windows.DependencyProperty AlignmentYDp = DrawingBrush.AlignmentYProperty;

            _class_testresult &= _helper.CompareProp("AlignmentYProperty", (AlignmentYDp.OwnerType.ToString() + "::" + AlignmentYDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(AlignmentY).ToString()));
            #endregion

            #region Test #7 - The ViewboxProperty Property
            // Usage: DependencyProperty = DrawingBrush.Viewbox (Read only)
            // Notes: Returns a Dependency Property for Viewbox
            CommonLib.LogStatus("Test #7 - The ViewboxProperty Property");

            System.Windows.DependencyProperty ViewboxDp = DrawingBrush.ViewboxProperty;

            _class_testresult &= _helper.CompareProp("ViewboxProperty", (ViewboxDp.OwnerType.ToString() + "::" + ViewboxDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(System.Windows.Rect).ToString()));
            #endregion

            #region Test #8 - The ViewportProperty Property
            // Usage: DependencyProperty = DrawingBrush.Viewport (Read only)
            // Notes: Returns a Dependency Property for Viewport
            CommonLib.LogStatus("Test #8 - The ViewportProperty Property");

            System.Windows.DependencyProperty ViewportDp = DrawingBrush.ViewportProperty;

            _class_testresult &= _helper.CompareProp("ViewportProperty", (ViewportDp.OwnerType.ToString() + "::" + ViewportDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(System.Windows.Rect).ToString()));
            #endregion

            #region Test #9 - The ViewportUnitsProperty Property
            // Usage: DependencyProperty = DrawingBrush.ViewportUnits (Read only)
            // Notes: Returns a Dependency Property for ViewportUnits
            CommonLib.LogStatus("Test #9 - The ViewportUnitsProperty Property");

            System.Windows.DependencyProperty ViewportUnitsDp = DrawingBrush.ViewportUnitsProperty;

            _class_testresult &= _helper.CompareProp("ViewportUnitsProperty", (ViewportUnitsDp.OwnerType.ToString() + "::" + ViewportUnitsDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(BrushMappingMode).ToString()));
            #endregion

            #region Test #10 - The ViewboxUnitsProperty Property
            // Usage: DependencyProperty = DrawingBrush.ViewboxUnits (Read only)
            // Notes: Returns a Dependency Property for ViewboxUnits
            CommonLib.LogStatus("Test #10 - The ViewboxUnitsProperty Property");

            System.Windows.DependencyProperty ViewboxUnitsDp = DrawingBrush.ViewboxUnitsProperty;

            _class_testresult &= _helper.CompareProp("ViewboxUnitsProperty", (ViewboxUnitsDp.OwnerType.ToString() + "::" + ViewboxUnitsDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(BrushMappingMode).ToString()));
            #endregion

            #region Test #11 - The OpacityProperty Property
            // Usage: DependencyProperty = DrawingBrush.Opacity (Read only)
            // Notes: Returns a Dependency Property for Opacity.
            CommonLib.LogStatus("Test #11 - The OpacityProperty Property");

            System.Windows.DependencyProperty OpacityDp = DrawingBrush.OpacityProperty;

            _class_testresult &= _helper.CompareProp("OpacityProperty", (OpacityDp.OwnerType.ToString() + "::" + OpacityDp.PropertyType.ToString()), (BaseClassType + "::" + typeof(System.Double).ToString()));
            #endregion

            #region Test #12 - The TransformProperty Property
            // Usage: DependencyProperty = DrawingBrush.TransformProperty (Read only)
            // Notes: Returns a Dependency Property for Transform.
            CommonLib.LogStatus("Test #12 - The TransformProperty Property");

            System.Windows.DependencyProperty TransformDp = DrawingBrush.TransformProperty;

            _class_testresult &= _helper.CompareProp("TransformProperty", (TransformDp.OwnerType.ToString() + "::" + TransformDp.PropertyType.ToString()), (BaseClassType + "::" + typeof(Transform).ToString()));
            #endregion

            #region Test #13 - The RelativeTransformProperty Property
            // Usage: DependencyProperty = DrawingBrush.RelativeTransformProperty (Read only)
            // Notes: Returns a Dependency Property for RelativeTransform.
            CommonLib.LogStatus("Test #13 - The RelativeTransformProperty Property");

            System.Windows.DependencyProperty RelativeTransformDp = DrawingBrush.RelativeTransformProperty;

            _class_testresult &= _helper.CompareProp("RelativeTransformProperty", (RelativeTransformDp.OwnerType.ToString() + "::" + RelativeTransformDp.PropertyType.ToString()), (BaseClassType + "::" + typeof(Transform).ToString()));
            #endregion
            #endregion End Of SECTION IV

            CommonLib.LogTest("Result for :" + _objectType);
        }

        private bool _class_testresult;
        private Type _objectType;
        private HelperClass _helper;
    }
}
