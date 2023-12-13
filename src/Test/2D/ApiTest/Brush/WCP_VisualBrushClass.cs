// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  WCP_VisualBrushClass - 
//

using System;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;
using Microsoft.Test.Graphics;


namespace WCP_MILlogterRender
{
    internal class WCP_VisualBrushClass : WCP_MILlogterRenderDVTest
    {
        public WCP_VisualBrushClass(double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            testDescription = "VisualBrush";
            _class_testresult = true;
            _objectType = typeof(VisualBrush);
            _helper = new HelperClass();
            //            Update();
        }

        public override void DoRender()
        {
            CommonLib.LogStatus(testDescription);

            // Test variables
            DrawingContext DC = this.RenderOpen();
            Type BaseClassType = typeof(Brush);
            Type ParentClassType = typeof(TileBrush);

            // Animations to be used for testing Properties 
            RectAnimation AnimRect = new RectAnimation(new Rect(new Point(50, 50), new Point(150, 150)), System.Windows.Duration.Forever);
            DoubleAnimation AnimDouble = new DoubleAnimation(0, 1.0, new System.Windows.Duration(TimeSpan.FromMilliseconds(0)), FillBehavior.Stop);
            Transform AnimTransform = new TranslateTransform(0, 0);
            AnimTransform.BeginAnimation(TranslateTransform.XProperty, AnimDouble);

            #region SECTION I - CONSTRUCTORS
            CommonLib.LogStatus("***** SECTION I - CONSTRUCTORS *****");

            #region Test #1 - Default Constructor
            // Usage: VisualBrush()
            // Notes: Default Constructor creates an empty VisualBrush.
            CommonLib.LogStatus("Test #1 - Default Constructor");

            // Create an VisualBrush 
            VisualBrush VB11 = new VisualBrush();

            // Confirm that a VisualBrush was created successfully.
            _class_testresult &= _helper.CheckType(VB11, _objectType);
            #endregion

            #region Test #2 - Constructor with a Visual
            // Usage: VisualBrush(Visual)
            // Notes: Returns a VisualBrush constructed using a Visual.
            CommonLib.LogStatus("Test #2 - Constructor with a Visual");

            // Create a DrawingVisual
            DrawingVisual V12 = new DrawingVisual();
            DrawingContext dc12 = V12.RenderOpen();
            dc12.DrawRectangle(Brushes.Red, null, new Rect(new Point(0, 0), new Point(200, 200)));
            dc12.Close();

            // Create a VisualBrush 
            VBProp = new VisualBrush(V12);

            // Confirm that a VisualBrush was created successfully.
            _class_testresult &= _helper.CheckType(VBProp, _objectType);
            #endregion
            #endregion End Of SECTION I

            #region SECTION II - PROPERTIES
            CommonLib.LogStatus("***** SECTION II - PROPERTIES *****");

            #region Test #1 - The Visual Property
            // Usage: Visual = VisualBrush.Visual (R/W)
            // Notes: Gets or Sets the Visual that is contained in the Brush.
            CommonLib.LogStatus("Test #1 - The Visual Property");

            // Create a Visual 
            DrawingVisual DV21 = new DrawingVisual();
            DrawingContext dc21 = DV21.RenderOpen();
            dc21.DrawGeometry(Brushes.Blue, null, new RectangleGeometry(new Rect(10, 10, 150, 150), 90, 90));
            dc21.DrawGeometry(Brushes.White, null, new RectangleGeometry(new Rect(40, 40, 90, 90), 90, 90));
            dc21.DrawGeometry(Brushes.Red, null, new RectangleGeometry(new Rect(70, 70, 30, 30), 90, 90));
            dc21.Close();

            // Set the Visual property.
            VBProp.Visual = DV21;

            // Get the Visual as a DrawingVisual
            DrawingVisual DV21b = (DrawingVisual)VBProp.Visual;

            // Get the Visual value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("Drawing", DV21b.Drawing, DV21.Drawing);
            #endregion

            #region Test #2 - The AlignmentX Property
            // Usage: AlignmentX = VisualBrush.AlignmentX (R/W)
            // Notes: Gets or Sets a AlignmentX that indicates how the Viewbox is aligned
            // horizontally in the Viewport.
            CommonLib.LogStatus("Test #2 - The AlignmentX Property");

            // Set the AlignmentX property.
            VBProp.AlignmentX = AlignmentX.Left;

            // Get the AlignmentX value as an Int to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("AlignmentX", (int)VBProp.AlignmentX, (int)AlignmentX.Left);
            #endregion

            #region Test #3 - The Stretch Property
            // Usage: Stretch = VisualBrush.Stretch (R/W)
            // Notes: Gets or Sets a Stretch that indicates how the Viewbox will be stretched to fit the Viewport.
            CommonLib.LogStatus("Test #3 - The Stretch Property");

            // Set the Stretch property.
            VBProp.Stretch = Stretch.Uniform;

            // Get the Stretch value as an Int to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("Stretch", (int)VBProp.Stretch, (int)Stretch.Uniform);
            #endregion

            #region Test #4 - The TileMode Property
            // Usage: TileMode = VisualBrush.TileMode (R/W)
            // Notes: Gets or Sets a TileMode that indicates how the Viewbox will be Tiled in the Viewport.
            CommonLib.LogStatus("Test #4 - The TileMode Property");

            // Set the TileMode property.
            VBProp.TileMode = TileMode.Tile;

            // Get the TileMode value as an Int to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("TileMode", (int)VBProp.TileMode, (int)TileMode.Tile);
            #endregion

            #region Test #5 - The AlignmentY Property
            // Usage: AlignmentY = VisualBrush.AlignmentY (R/W)
            // Notes: Gets or Sets a AlignmentY that indicates how the Viewbox is aligned
            // Vertically in the Viewport.
            CommonLib.LogStatus("Test #5 - The AlignmentY Property");

            // Set the AlignmentY property.
            VBProp.AlignmentY = AlignmentY.Top;

            // Get the AlignmentY value as an Int to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("AlignmentY", (int)VBProp.AlignmentY, (int)AlignmentY.Top);
            #endregion

            #region Test #6 - The ViewboxUnits Property
            // Usage: BrushMappingMode = VisualBrush.ViewboxUnits (R/W)
            // Notes: Gets or Sets the BrushMappingMode that indicates whether the Viewbox
            // will map it's coordinates to the entire user space or just to the shape that
            // is being filled.
            CommonLib.LogStatus("Test #6 - The ViewboxUnits Property");

            // Set the ViewboxUnits property.
            VBProp.ViewboxUnits = BrushMappingMode.Absolute;

            // Get the ViewboxUnits value as an Int to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("ViewboxUnits", (int)VBProp.ViewboxUnits, (int)BrushMappingMode.Absolute);
            #endregion

            #region Test #7 - The Viewbox Property
            // Usage: Rect = VisualBrush.Viewbox (R/W)
            // Notes: Gets or Sets a Rectangle that describes the bounds of the Viewbox
            // (the content that is being mapped from).
            CommonLib.LogStatus("Test #7 - The Viewbox Property");

            // Set the Viewbox property.
            Rect rect27 = new Rect(new Point(0.0, 0.0), new Point(200.0, 200.0));
            VBProp.Viewbox = rect27;

            // Get the Viewbox value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("Viewbox", VBProp.Viewbox, rect27);

            // Add a Rect Animation to the Brush
            VBProp.BeginAnimation(VisualBrush.ViewboxProperty, AnimRect);

            // Animated Viewbox Width and Height should be between 0,0 and the Width and Height of the Rect that was set.
            _class_testresult &= _helper.CompareRectWithRange("Viewbox (With Animation)", VBProp.Viewbox, 0, 200);
            #endregion

            #region Test #8 - The Viewport Property
            // Usage: Rect = VisualBrush.Viewport (R/W)
            // Notes: Gets or Sets a Rectangle that describes the bounds of the Viewport
            // (the area that is being mapped into)
            CommonLib.LogStatus("Test #8 - The Viewport Property");

            // Set the Viewport property.
            Rect rect28 = new Rect(new Point(0.0, 0.0), new Point(0.5, 0.5));

            VBProp.Viewport = rect28;

            // Get the Viewport value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("Viewport", VBProp.Viewport, rect28);

            // Add a Rect Animation to the Brush
            VBProp.BeginAnimation(VisualBrush.ViewportProperty, AnimRect);

            // Animated Viewport Width and Height should be between 0,0 and the Width and Height of the Rect that was set.
            _class_testresult &= _helper.CompareRectWithRange("Viewport (With Animation)", VBProp.Viewport, 0, 100);
            #endregion

            #region Test #9 - The ViewportUnits Property
            // Usage: BrushMappingMode = VisualBrush.ViewportUnits (R/W)
            // Notes: Gets or Sets the BrushMappingMode that indicates whether the Viewport
            // will map it's coordinates to the entire user space or just to the shape that
            // is being filled.
            CommonLib.LogStatus("Test #9 - The ViewportUnits Property");

            // Set the ViewportUnits property.
            VBProp.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;

            // Get the ViewportUnits value as an Int to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("ViewportUnits", (int)VBProp.ViewportUnits, (int)BrushMappingMode.RelativeToBoundingBox);
            #endregion

            #region Test #10 - The Opacity Property
            // Usage: double = VisualBrush.Opacity (R/W)
            // Notes: Gets or Sets the Opacity value of the Brush.
            CommonLib.LogStatus("Test #10 - The Opacity Property");

            // Set the Opacity property.
            VBProp.Opacity = 0.9;

            // Get the Opacity value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("Opacity", VBProp.Opacity, 0.9);

            // Add an Opacity Animation to the Brush
            VBProp.BeginAnimation(VisualBrush.OpacityProperty, AnimDouble);

            // Get the Opacity value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("Opacity (With Animation)", VBProp.Opacity, 0.9);
            #endregion

            #region Test #11 - The Transform Property
            // Usage: Transform = VisualBrush.Transform (R/W)
            // Notes: Gets or Sets the Transform for the Brush.
            CommonLib.LogStatus("Test #11 - The Transform Property");

            // Set the Transform property.
            VBProp.Transform = new TranslateTransform(30, 30);

            // Get the Transform value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareMatrix("Transform", VBProp.Transform, 1, 0, 0, 1, 30, 30);

            // Set the Transform property to an Animated Transform.
            VBProp.Transform = AnimTransform;

            // Get the Transform value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareMatrix("Transform (With Animation)", VBProp.Transform, 1, 0, 0, 1, 0, 0);
            #endregion

            #region Test #12 - The RelativeTransform Property
            // Usage: Transform = VisualBrush.RelativeTransform (R/W)
            // Notes: Gets or Sets the Transform for the Brush relative to the object it is filling.
            CommonLib.LogStatus("Test #12 - The RelativeTransform Property");

            // Use the RelativeTransform property to set the Brush Transform relative to the object it is filling.
            VBProp.RelativeTransform = new TranslateTransform(.05, .05);

            // Get the RelativeTransform value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareMatrix("RelativeTransform", VBProp.RelativeTransform, 1, 0, 0, 1, .05, .05);

            // Set the RelativeTransform property to an Animated Transform.
            VBProp.RelativeTransform = AnimTransform;

            // Get the RelativeTransform value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareMatrix("RelativeTransform (With Animation)", VBProp.RelativeTransform, 1, 0, 0, 1, 0, 0);
            #endregion

            #region Test #13 - The CanFreeze Property
            // Usage: bool = VisualBrush.CanFreeze (Read Only)
            // Notes: Returns a bool that indicates whether the Brush is animatable.
            CommonLib.LogStatus("Test #13 - The CanFreeze Property");

            // Create a Visual 
            DrawingVisual DV213 = new DrawingVisual();
            DrawingContext dc213 = DV213.RenderOpen();
            dc213.DrawRectangle(Brushes.Yellow, null, new System.Windows.Rect(10, 10, 60, 60));
            dc213.Close();

            // Create a VisualBrush 
            VisualBrush VB213 = new VisualBrush(DV213);

            // Check the CanFreeze value of a non-animated Brush.
            _class_testresult &= _helper.CompareProp("CanFreeze", VB213.CanFreeze, true);

            // Add an Animation to the Brush
            VB213.BeginAnimation(VisualBrush.ViewportProperty, AnimRect);

            // Check the CanFreeze value of an animated Brush.
            _class_testresult &= _helper.CompareProp("CanFreeze (Animated Brush)", VB213.CanFreeze, true);
            #endregion

            #region Test #14 - The AutoLayoutContent Property
            // Usage: bool = VisualBrush.AutoLayoutContent (R/W)
            // Notes: Gets or Sets AutoLayoutContent that indicates how the VisualBrush is layed out
            CommonLib.LogStatus("Test #14 - The AutoLayoutContent Property");

            // Set the AutoLayoutContent property.
            VBProp.AutoLayoutContent = true;

            // Get the AutoLayoutContent to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("AutoLayoutContent", VBProp.AutoLayoutContent, true);
            #endregion

            #region Test #15 - Basic rendering with a VisualBrush
            // Just fill the surface with a VisualBrush to see if we crash or not.
            CommonLib.LogStatus("Test #15 - Basic rendering with a VisualBrush");

            // Clear all Animations before rendering.
            VBProp.BeginAnimation(VisualBrush.ViewboxProperty, null);
            VBProp.BeginAnimation(VisualBrush.ViewportProperty, null);
            VBProp.BeginAnimation(VisualBrush.OpacityProperty, null);
            VBProp.Transform = new TranslateTransform(30, 30);
            VBProp.RelativeTransform = new TranslateTransform(.05, .05);
            DC.DrawRectangle(VBProp, null, new System.Windows.Rect(m_top, m_left, m_width, m_height));
            DC.Close();
            #endregion
            #endregion End Of SECTION II


            #region SECTION III - METHODS
            CommonLib.LogStatus("***** SECTION III - METHODS *****");

            #region Test #1 - The Copy Method
            // Usage: VisualBrush = VisualBrush.Clone()
            // Notes: Returns a VisualBrush that is a copy of this VisualBrush
            CommonLib.LogStatus("Test #1 - The Copy Method");

            // Create a Visual 
            DrawingVisual DV31 = new DrawingVisual();
            DrawingContext dc31 = DV31.RenderOpen();
            dc31.DrawRectangle(Brushes.Red, null, new System.Windows.Rect(10, 10, 110, 60));
            dc31.DrawRectangle(Brushes.Blue, null, new System.Windows.Rect(50, 50, 150, 90));
            dc31.Close();

            // Create a VisualBrush 
            VisualBrush VB31a = new VisualBrush(DV31);

            // Use the Copy method to create a new VisualBrush that has the same value.
            VisualBrush VB31b = VB31a.Clone();

            // Check the VisualBrush properties for equality.
            DrawingVisual DVa = (DrawingVisual)VB31a.Visual;
            DrawingVisual DVb = (DrawingVisual)VB31b.Visual;

            if ((_helper.CompareProp("Drawing", DVb.Drawing, DVa.Drawing)) &&
                (System.Windows.Rect.Equals(VB31a.Viewport, VB31b.Viewport)) &&
                (System.Windows.Rect.Equals(VB31a.Viewbox, VB31b.Viewbox)))
            {
                CommonLib.LogStatus("Pass: Copy - VisualBrush properties are equal");
            }
            else
            {
                CommonLib.LogFail("Fail: Copy - VisualBrush properties are NOT equal");
                _class_testresult &= false;
            }
            #endregion

            #region Test #2 - The CloneCurrentValue Method
            // Usage: VisualBrush = VisualBrush.CloneCurrentValue()
            // Notes: Returns a new Brush that has the same CurrentValue as this Brush
            CommonLib.LogStatus("Test #2 - The CloneCurrentValue Method");

            // Create a Visual 
            DrawingVisual DV32 = new DrawingVisual();
            DrawingContext dc32 = DV32.RenderOpen();
            dc32.DrawRectangle(Brushes.Yellow, null, new System.Windows.Rect(10, 10, 110, 110));
            dc32.DrawRectangle(Brushes.Blue, null, new System.Windows.Rect(50, 50, 150, 90));
            dc32.Close();

            // Create a VisualBrush 
            VisualBrush VB32a = new VisualBrush(DV32);

            // Use the CloneCurrentValue method to create a new VisualBrush that has the same current value.
            VisualBrush VB32b = (VisualBrush)VB32a.CloneCurrentValue();

            // Check the VisualBrush properties for equality.
            DrawingVisual DV2a = (DrawingVisual)VB32a.Visual;
            DrawingVisual DV2b = (DrawingVisual)VB32b.Visual;

            if ((_helper.CompareProp("Drawing", DV2b.Drawing, DV2a.Drawing)) &&
                (System.Windows.Rect.Equals(VB32a.Viewport, VB32b.Viewport)) &&
                (System.Windows.Rect.Equals(VB32a.Viewbox, VB32b.Viewbox)))
            {
                CommonLib.LogStatus("Pass: CloneCurrentValue - VisualBrush properties are equal");
            }
            else
            {
                CommonLib.LogFail("Fail: CloneCurrentValue - VisualBrush properties are NOT equal");
                _class_testresult &= false;
            }
            #endregion
            #endregion End Of SECTION III


            #region SECTION IV - STATIC PROPERTIES
            CommonLib.LogStatus("***** SECTION IV - STATIC PROPERTIES *****");

            #region Test #1 - The VisualProperty Property
            // Usage: DependencyProperty = VisualBrush.VisualProperty (Read only)
            // Notes: Returns a Dependency Property for Visual.
            CommonLib.LogStatus("Test #1 - The VisualProperty Property");

            System.Windows.DependencyProperty VisualDp = VisualBrush.VisualProperty;

            _class_testresult &= _helper.CompareProp("VisualProperty", VisualDp.OwnerType, _objectType);
            _class_testresult &= _helper.CompareProp("VisualProperty", VisualDp.PropertyType, typeof(Visual));
            #endregion

            #region Test #2 - The AlignmentXProperty Property
            // Usage: DependencyProperty = VisualBrush.AlignmentXProperty (Read only)
            // Notes: Returns a Dependency Property for AlignmentX.
            CommonLib.LogStatus("Test #2 - The AlignmentXProperty Property");

            System.Windows.DependencyProperty AlignmentXDp = VisualBrush.AlignmentXProperty;

            _class_testresult &= _helper.CompareProp("AlignmentXProperty",AlignmentXDp.OwnerType,ParentClassType);
                _class_testresult &= _helper.CompareProp("AlignmentXProperty", AlignmentXDp.PropertyType,typeof(AlignmentX));
            #endregion

            #region Test #3 - The StretchProperty Property
            // Usage: DependencyProperty = VisualBrush.StretchProperty (Read only)
            // Notes: Returns a Dependency Property for Stretch
            CommonLib.LogStatus("Test #3 - The StretchProperty Property");

            System.Windows.DependencyProperty StretchDp = VisualBrush.StretchProperty;

            _class_testresult &= _helper.CompareProp("StretchProperty", (StretchDp.OwnerType.ToString() + "::" + StretchDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(Stretch).ToString()));
            #endregion

            #region Test #4 - The TileModeProperty Property
            // Usage: DependencyProperty = VisualBrush.TileModeProperty (Read only)
            // Notes: Returns a Dependency Property for TileMode
            CommonLib.LogStatus("Test #4 - The TileModeProperty Property");

            System.Windows.DependencyProperty TileModeDp = VisualBrush.TileModeProperty;

            _class_testresult &= _helper.CompareProp("TileModeProperty", (TileModeDp.OwnerType.ToString() + "::" + TileModeDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(TileMode).ToString()));
            #endregion

            #region Test #5 - The AlignmentYProperty Property
            // Usage: DependencyProperty = VisualBrush.AlignmentYProperty (Read only)
            // Notes: Returns a Dependency Property for AlignmentY
            CommonLib.LogStatus("Test #5 - The AlignmentYProperty Property");

            System.Windows.DependencyProperty AlignmentYDp = VisualBrush.AlignmentYProperty;

            _class_testresult &= _helper.CompareProp("AlignmentYProperty", (AlignmentYDp.OwnerType.ToString() + "::" + AlignmentYDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(AlignmentY).ToString()));
            #endregion

            #region Test #6 - The ViewboxProperty Property
            // Usage: DependencyProperty = VisualBrush.Viewbox (Read only)
            // Notes: Returns a Dependency Property for Viewbox
            CommonLib.LogStatus("Test #6 - The ViewboxProperty Property");

            System.Windows.DependencyProperty ViewboxDp = VisualBrush.ViewboxProperty;

            _class_testresult &= _helper.CompareProp("ViewboxProperty", (ViewboxDp.OwnerType.ToString() + "::" + ViewboxDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(System.Windows.Rect).ToString()));
            #endregion

            #region Test #7 - The ViewportProperty Property
            // Usage: DependencyProperty = VisualBrush.Viewport (Read only)
            // Notes: Returns a Dependency Property for Viewport
            CommonLib.LogStatus("Test #7 - The ViewportProperty Property");

            System.Windows.DependencyProperty ViewportDp = VisualBrush.ViewportProperty;

            _class_testresult &= _helper.CompareProp("ViewportProperty", (ViewportDp.OwnerType.ToString() + "::" + ViewportDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(System.Windows.Rect).ToString()));
            #endregion

            #region Test #8 - The ViewportUnitsProperty Property
            // Usage: DependencyProperty = VisualBrush.ViewportUnits (Read only)
            // Notes: Returns a Dependency Property for ViewportUnits
            CommonLib.LogStatus("Test #8 - The ViewportUnitsProperty Property");

            System.Windows.DependencyProperty ViewportUnitsDp = VisualBrush.ViewportUnitsProperty;

            _class_testresult &= _helper.CompareProp("ViewportUnitsProperty", (ViewportUnitsDp.OwnerType.ToString() + "::" + ViewportUnitsDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(BrushMappingMode).ToString()));
            #endregion

            #region Test #9 - The ViewboxUnitsProperty Property
            // Usage: DependencyProperty = VisualBrush.ViewboxUnits (Read only)
            // Notes: Returns a Dependency Property for ViewboxUnits
            CommonLib.LogStatus("Test #9 - The ViewboxUnitsProperty Property");

            System.Windows.DependencyProperty ViewboxUnitsDp = VisualBrush.ViewboxUnitsProperty;

            _class_testresult &= _helper.CompareProp("ViewboxUnitsProperty", (ViewboxUnitsDp.OwnerType.ToString() + "::" + ViewboxUnitsDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(BrushMappingMode).ToString()));
            #endregion

            #region Test #10 - The OpacityProperty Property
            // Usage: DependencyProperty = VisualBrush.Opacity (Read only)
            // Notes: Returns a Dependency Property for Opacity.
            CommonLib.LogStatus("Test #10 - The OpacityProperty Property");

            System.Windows.DependencyProperty OpacityDp = VisualBrush.OpacityProperty;

            _class_testresult &= _helper.CompareProp("OpacityProperty", (OpacityDp.OwnerType.ToString() + "::" + OpacityDp.PropertyType.ToString()), (BaseClassType + "::" + typeof(System.Double).ToString()));
            #endregion

            #region Test #11 - The TransformProperty Property
            // Usage: DependencyProperty = VisualBrush.TransformProperty (Read only)
            // Notes: Returns a Dependency Property for Transform.
            CommonLib.LogStatus("Test #11 - The TransformProperty Property");

            System.Windows.DependencyProperty TransformDp = VisualBrush.TransformProperty;

            _class_testresult &= _helper.CompareProp("TransformProperty", (TransformDp.OwnerType.ToString() + "::" + TransformDp.PropertyType.ToString()), (BaseClassType + "::" + typeof(Transform).ToString()));
            #endregion

            #region Test #12 - The RelativeTransformProperty Property
            // Usage: DependencyProperty = VisualBrush.RelativeTransformProperty (Read only)
            // Notes: Returns a Dependency Property for RelativeTransform.
            CommonLib.LogStatus("Test #12 - The RelativeTransformProperty Property");

            System.Windows.DependencyProperty RelativeTransformDp = VisualBrush.RelativeTransformProperty;

            _class_testresult &= _helper.CompareProp("RelativeTransformProperty", (RelativeTransformDp.OwnerType.ToString() + "::" + RelativeTransformDp.PropertyType.ToString()), (BaseClassType + "::" + typeof(Transform).ToString()));
            #endregion
            #endregion End Of SECTION IV
        }

        public override void logterRender()
        {
            CommonLib.LogStatus("Test: Update Content of a VisualBrush");
            VBProp.RelativeTransform = new ScaleTransform(2.0, 2.0);
        }

        public VisualBrush VBProp;

        private bool _class_testresult;
        private Type _objectType;
        private HelperClass _helper;
    }
}
