// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test all public API's in the ImageBrush class
//
using System;
using System.ComponentModel.Design.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class WCP_ImageBrushClass : ApiTest
    {
        public WCP_ImageBrushClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(ImageBrush);
            _helper = new HelperClass();
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus(_objectType.ToString());

            // Test variables
            BrushConverter BConverter = new BrushConverter();
            System.Globalization.CultureInfo Cinfo = System.Globalization.CultureInfo.InvariantCulture;
            String ValidType = "String";
            InstanceDescriptor ValidType2 = new InstanceDescriptor(null, null);
            bool Comp2;
            String SimpleBrushString, NonSimpleBrushString;
            Type BaseClassType = typeof(Brush);
            Type ParentClassType = typeof(TileBrush);

            // Expected exception messages
            string ConvertToNotSupportedException = "Cannot convert to type.";

            // Animations to be used for testing Properties
            RectAnimation AnimRect = new RectAnimation(new Rect(new Point(50, 50), new Point(150, 150)), new System.Windows.Duration(TimeSpan.FromMilliseconds(1000)));
            DoubleAnimation AnimDouble = new DoubleAnimation(0, 1.0, new System.Windows.Duration(TimeSpan.FromMilliseconds(0)), FillBehavior.Stop);
            Transform AnimTransform = new TranslateTransform(0, 0);
            AnimTransform.BeginAnimation(TranslateTransform.XProperty, AnimDouble);

            // SimpleBrush and Non-SimpleBrush for Serialization method tests
            ImageBrush SimpleBrush = new ImageBrush();
            ImageBrush NonSimpleBrush = new ImageBrush(new BitmapImage(new Uri("RedSquare.TIF", UriKind.RelativeOrAbsolute)));

            #region SECTION I - CONSTRUCTORS
            CommonLib.LogStatus("***** SECTION I - CONSTRUCTORS *****");

            #region Test #1 - Default Constructor
            // Usage: ImageBrush()
            // Notes: Default Constructor creates an empty ImageBrush.
            CommonLib.LogStatus("Test #1 - Default Constructor");

            // Create an ImageBrush 
            ImageBrush IB11 = new ImageBrush();

            // Confirm that a ImageBrush was created successfully.
            _class_testresult &= _helper.CheckType(IB11, _objectType);
            #endregion

            #region Test #2 - Constructor with an BitmapSource
            // Usage: ImageBrush(BitmapSource)
            // Notes: Returns an ImageBrush constructed using an BitmapSource.
            CommonLib.LogStatus("Test #2 - Constructor with an BitmapSource");

            // Create an BitmapSource 
            BitmapImage imgsrc12 = new BitmapImage(new Uri("RedSquare.TIF", UriKind.RelativeOrAbsolute));

            // Create an ImageBrush 
            ImageBrush IBprop = new ImageBrush(imgsrc12);

            // Confirm that a ImageBrush was created successfully.
            _class_testresult &= _helper.CheckType(IBprop, _objectType);
            #endregion
            #endregion End Of SECTION I

            #region SECTION II - PROPERTIES
            CommonLib.LogStatus("***** SECTION II - PROPERTIES *****");

            #region Test #1 - The BitmapSource Property
            // Usage: BitmapSource = ImageBrush.BitmapSource (R/W)
            // Notes: Gets or Sets the BitmapSource that is contained in the Brush.
            CommonLib.LogStatus("Test #1 - The BitmapSource Property");

            // Create an BitmapSource
            BitmapImage ImgSource = new BitmapImage(new Uri("v5sRGB16table.bmp", UriKind.RelativeOrAbsolute));

            // Set the BitmapSource property.
            IBprop.ImageSource = ImgSource;

            // Get the BitmapSource value to assure that it was the one that was set by
            // checking its properties.
            if (_helper.CompareImageSourceProperties(ImgSource, IBprop.ImageSource))
            {
                CommonLib.LogStatus("Pass: BitmapSource - BitmapSource properties are equal");
            }
            else
            {
                CommonLib.LogFail("Fail: BitmapSource - BitmapSource properties are NOT equal");
                _class_testresult &= false;
            }
            #endregion

            #region Test #2 - The ViewboxUnits Property
            // Usage: BrushMappingMode = ImageBrush.ViewboxUnits (R/W)
            // Notes: Gets or Sets the BrushMappingMode that indicates whether the portion of the content
            // described by the Viewbox will be in Absolute coordinates or in coordinates relative to the content.
            CommonLib.LogStatus("Test #2 - The ViewboxUnits Property");

            // Set the ViewboxUnits property.
            IBprop.ViewboxUnits = BrushMappingMode.Absolute;

            // Get the ViewboxUnits value as an Int to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("ViewboxUnits", (int)IBprop.ViewboxUnits, (int)BrushMappingMode.Absolute);
            #endregion

            #region Test #3 - The AlignmentX Property
            // Usage: AlignmentX = ImageBrush.AlignmentX (R/W)
            // Notes: Gets or Sets a AlignmentX that indicates how the Viewbox is aligned
            // horizontally in the Viewport.
            CommonLib.LogStatus("Test #3 - The AlignmentX Property");

            // Set the AlignmentX property.
            IBprop.AlignmentX = AlignmentX.Center;

            // Get the AlignmentX value as an Int to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("AlignmentX", (int)IBprop.AlignmentX, (int)AlignmentX.Center);
            #endregion

            #region Test #4 - The Stretch Property
            // Usage: Stretch = ImageBrush.Stretch (R/W)
            // Notes: Gets or Sets a Stretch that indicates how the Viewbox will be stretched to fit the Viewport.
            CommonLib.LogStatus("Test #4 - The Stretch Property");

            // Set the Stretch property.
            IBprop.Stretch = Stretch.UniformToFill;

            // Get the Stretch value as an Int to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("Stretch", (int)IBprop.Stretch, (int)Stretch.UniformToFill);
            #endregion

            #region Test #5 - The TileMode Property
            // Usage: TileMode = ImageBrush.TileMode (R/W)
            // Notes: Gets or Sets a TileMode that indicates how the Viewbox will be Tiled in the Viewport.
            CommonLib.LogStatus("Test #5 - The TileMode Property");

            // Set the TileMode property.
            IBprop.TileMode = TileMode.FlipXY;

            // Get the TileMode value as an Int to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("TileMode", (int)IBprop.TileMode, (int)TileMode.FlipXY);
            #endregion

            #region Test #6 - The AlignmentY Property
            // Usage: AlignmentY = ImageBrush.AlignmentY (R/W)
            // Notes: Gets or Sets a AlignmentY that indicates how the Viewbox is aligned
            // Vertically in the Viewport.
            CommonLib.LogStatus("Test #6 - The AlignmentY Property");

            // Set the AlignmentY property.
            IBprop.AlignmentY = AlignmentY.Center;

            // Get the AlignmentY value as an Int to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("AlignmentY", (int)IBprop.AlignmentY, (int)AlignmentY.Center);
            #endregion

            #region Test #7 - The Viewbox Property
            // Usage: Rect = ImageBrush.Viewbox (R/W)
            // Notes: Gets or Sets a Rectangle that describes the bounds of the Viewbox
            // (the content that is being mapped from).
            CommonLib.LogStatus("Test #7 - The Viewbox Property");

            // Set the Viewbox property.
            Rect rect27 = new Rect(new Point(0.0, 0.0), new Point(200.0, 200.0));
            IBprop.Viewbox = rect27;

            // Get the Viewbox value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("Viewbox", IBprop.Viewbox, rect27);

            // Add a Rect Animation to the Brush
            IBprop.BeginAnimation(ImageBrush.ViewboxProperty, AnimRect);

            // Animated Viewbox Width and Height should be between 0,0 and the Width and Height of the Rect that was set.
            _class_testresult &= _helper.CompareRectWithRange("Viewbox (With Animation)", IBprop.Viewbox, 0, 200);
            #endregion

            #region Test #8 - The Viewport Property
            // Usage: Rect = ImageBrush.Viewport (R/W)
            // Notes: Gets or Sets a Rectangle that describes the bounds of the Viewport
            // (the area that is being mapped into)
            CommonLib.LogStatus("Test #8 - The Viewport Property");

            // Set the Viewport property.
            Rect rect28 = new Rect(new Point(0.0, 0.0), new Point(0.5, 0.5));
            IBprop.Viewport = rect28;

            // Get the Viewport value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("Viewport", IBprop.Viewport, rect28);

            // Add a Rect Animation to the Brush
            IBprop.BeginAnimation(ImageBrush.ViewportProperty, AnimRect);

            // Animated Viewport Width and Height should be between 0,0 and the Width and Height of the Rect that was set.
            _class_testresult &= _helper.CompareRectWithRange("Viewport (With Animation)", IBprop.Viewport, 0, 100);
            #endregion

            #region Test #9 - The ViewportUnits Property
            // Usage: BrushMappingMode = ImageBrush.ViewportUnits (R/W)
            // Notes: Gets or Sets the BrushMappingMode that indicates whether the Viewport
            // will map it's coordinates to the entire user space or just to the shape that
            // is being filled.
            CommonLib.LogStatus("Test #9 - The ViewportUnits Property");

            // Set the ViewportUnits property.
            IBprop.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;

            // Get the ViewportUnits value as an Int to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("ViewportUnits", (int)IBprop.ViewportUnits, (int)BrushMappingMode.RelativeToBoundingBox);
            #endregion

            #region Test #10 - The Opacity Property
            // Usage: double = ImageBrush.Opacity (R/W)
            // Notes: Gets or Sets the Opacity value of the Brush.
            CommonLib.LogStatus("Test #10 - The Opacity Property");

            // Set the Opacity property.
            IBprop.Opacity = 0.7;

            // Get the Opacity value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("Opacity", IBprop.Opacity, 0.7);

            // Add an Opacity Animation to the Brush
            IBprop.BeginAnimation(ImageBrush.OpacityProperty, AnimDouble);

            // Get the Opacity value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("Opacity (With Animation)", IBprop.Opacity, 0.7);
            #endregion

            #region Test #11 - The Transform Property
            // Usage: Transform = ImageBrush.Transform (R/W)
            // Notes: Gets or Sets the Transform for the Brush.
            CommonLib.LogStatus("Test #11 - The Transform Property");

            // Set the Transform property.
            IBprop.Transform = new ScaleTransform(0.5, 0.5, 100, 100);

            // Get the Transform value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareMatrix("Transform", IBprop.Transform, 0.5, 0, 0, 0.5, 50, 50);

            // Set the Transform property to an Animated Transform.
            IBprop.Transform = AnimTransform;

            // Get the Transform value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareMatrix("Transform (With Animation)", IBprop.Transform, 1, 0, 0, 1, 0, 0);
            #endregion

            #region Test #12 - The RelativeTransform Property
            // Usage: Transform = ImageBrush.RelativeTransform (R/W)
            // Notes: Gets or Sets the Transform for the Brush relative to the object it is filling.
            CommonLib.LogStatus("Test #12 - The RelativeTransform Property");

            // Use the RelativeTransform property to set the Brush Transform relative to the object it is filling.
            IBprop.RelativeTransform = new ScaleTransform(2, 2, .5, .5);

            // Get the RelativeTransform value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareMatrix("RelativeTransform", IBprop.RelativeTransform, 2, 0, 0, 2, -.5, -.5);

            // Set the RelativeTransform property to an Animated Transform.
            IBprop.RelativeTransform = AnimTransform;

            // Get the Transform value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareMatrix("RelativeTransform (With Animation)", IBprop.RelativeTransform, 1, 0, 0, 1, 0, 0);
            #endregion

            #region Test #13 - The CanFreeze Property
            // Usage: bool = ImageBrush.CanFreeze (Read Only)
            // Notes: Returns a bool that indicates whether the Brush is animatable.
            CommonLib.LogStatus("Test #13 - The CanFreeze Property");

            // Create an BitmapSource 
            BitmapImage imgsrc213 = new BitmapImage(new Uri("S1.PNG", UriKind.RelativeOrAbsolute));

            // Create an ImageBrush 
            ImageBrush IB213 = new ImageBrush(imgsrc213);

            // Check the CanFreeze value of a non-animated Brush.
            _class_testresult &= _helper.CompareProp("CanFreeze", IB213.CanFreeze, true);

            // Add an Animation to the Brush
            IB213.BeginAnimation(ImageBrush.ViewportProperty, AnimRect);

            // Check the CanFreeze value of an animated Brush.
            _class_testresult &= _helper.CompareProp("CanFreeze (Animated Brush)", IB213.CanFreeze, false);
            #endregion

            #region Test #14 - Basic rendering with an ImageBrush
            // Just fill the surface with a ImageBrush to see if we crash or not.
            CommonLib.LogStatus("Test #14 - Basic rendering with an ImageBrush");

            // Clear Animations before rendering.
            IBprop.BeginAnimation(ImageBrush.ViewboxProperty, null);
            IBprop.BeginAnimation(ImageBrush.ViewportProperty, null);
            IBprop.BeginAnimation(ImageBrush.OpacityProperty, null);
            IBprop.Transform = new ScaleTransform(0.5, 0.5, 100, 100);
            IBprop.RelativeTransform = new ScaleTransform(2, 2, .5, .5);

            DC.DrawRectangle(IBprop, null, new System.Windows.Rect(m_top, m_left, m_width, m_height));
            #endregion
            #endregion End Of SECTION II

            #region SECTION III - METHODS
            CommonLib.LogStatus("***** SECTION III - METHODS *****");

            #region Test #1 - The Copy Method
            // Usage: ImageBrush = ImageBrush.Clone()
            // Notes: Returns a ImageBrush that is a copy of this ImageBrush
            CommonLib.LogStatus("Test #1 - The Copy Method");

            // Create an BitmapSource 
            BitmapImage imgsrc31 = new BitmapImage(new Uri("S1.PNG", UriKind.RelativeOrAbsolute));

            // Create an ImageBrush 
            ImageBrush IB31a = new ImageBrush(imgsrc31);

            // Use the Copy method to create a new ImageBrush that has the same value.
            ImageBrush IB31b = IB31a.Clone();

            // Check the ImageBrush objects for equality.
            if (_helper.CompareImageSourceProperties(IB31a.ImageSource, IB31b.ImageSource) &&
               (System.Windows.Rect.Equals(IB31a.Viewport, IB31b.Viewport)) &&
               (System.Windows.Rect.Equals(IB31a.Viewbox, IB31b.Viewbox)))
            {
                CommonLib.LogStatus("Pass: Copy - ImageBrush properties are equal");
            }
            else
            {
                CommonLib.LogFail("Fail: Copy - ImageBrush properties are NOT equal");
                _class_testresult &= false;
            }
            #endregion

            #region Test #2 - The CloneCurrentValue Method
            // Usage: ImageBrush = ImageBrush.CloneCurrentValue()
            // Notes: Returns a new Brush that has the same CurrentValue as this Brush
            CommonLib.LogStatus("Test #2 - The CloneCurrentValue Method");

            // Create an BitmapSource 
            BitmapImage imgsrc32 = new BitmapImage(new Uri("v5sRGB16table.bmp", UriKind.RelativeOrAbsolute));

            // Create an ImageBrush 
            ImageBrush IB32a = new ImageBrush(imgsrc32);

            // Use the CloneCurrentValue method to create a new ImageBrush that has the same current value.
            ImageBrush IB32b = IB32a.CloneCurrentValue();

            // Check the ImageBrush objects for equality.
            if (_helper.CompareImageSourceProperties(IB32a.ImageSource, IB32b.ImageSource) &&
               (Rect.Equals(IB32a.Viewport, IB32b.Viewport)) &&
               (Rect.Equals(IB32a.Viewbox, IB32b.Viewbox)))
            {
                CommonLib.LogStatus("Pass: CloneCurrentValue - ImageBrush properties are equal");
            }
            else
            {
                CommonLib.LogFail("Fail: CloneCurrentValue - ImageBrush properties are NOT equal");
                _class_testresult &= false;
            }
            #endregion
            #endregion End Of SECTION III

            #region SECTION IV - INTERNAL METHODS
            CommonLib.LogStatus("***** SECTION IV - INTERNAL METHODS *****");

            #region Test #1 - The IsSimpleBrush Method
            // Usage: bool = ImageBrush.IsSimpleBrush()
            // Notes: Returns a bool that indicates whether the Brush is "simple" enough to
            // be converted to a String or Serialized.
            CommonLib.LogStatus("Test #1 - The IsSimpleBrush Method");

            // Test a valid but non null ITypeDescriptorContext.  This should return
            // the value of Brush.IsSimpleBrush().  
            MyTypeConverter NonSimpleTypeConverter = new MyTypeConverter(NonSimpleBrush);
            Comp2 = BConverter.CanConvertTo(NonSimpleTypeConverter, ValidType.GetType());
            _class_testresult &= _helper.CompareProp("IsSimpleBrush - Non Simple", Comp2, false);
            #endregion

            #region Test #2 - The ConvertToString Method
            // Usage: string ImageBrush.ConvertToString(IFormatProvider)
            // Notes: Returns a string representation of the Brush.  If the Brush is not a
            // simple brush then a "not supported exception" is raised.
            CommonLib.LogStatus("Test #2 - The ConvertToString Method");

            try
            {
                SimpleBrushString = (String)BConverter.ConvertTo(null, Cinfo, SimpleBrush, ValidType.GetType());
                _class_testresult &= _helper.CompareProp("ConvertToString - Simple Brush", SimpleBrushString, _objectType.ToString());
            }
            catch (System.Exception e)
            {
                if (e.Message == ConvertToNotSupportedException)
                {
                    CommonLib.LogStatus("Pass: ConvertToString - Simple Brush returns the expected exception");
                }
                else
                {
                    CommonLib.LogFail("Fail: ConvertToString - Simple Brush returns " + e.Message + " should be " + ConvertToNotSupportedException);
                    _class_testresult &= false;
                }
            }

            try
            {
                NonSimpleBrushString = (String)BConverter.ConvertTo(null, Cinfo, NonSimpleBrush, ValidType.GetType());
            }
            catch (System.Exception e)
            {
                if (e.Message == ConvertToNotSupportedException)
                {
                    CommonLib.LogStatus("Pass: ConvertToString - Non Simple Brush returns the expected exception");
                }
                else
                {
                    CommonLib.LogFail("Fail: ConvertToString - Non Simple Brush returns " + e.Message + " should be " + ConvertToNotSupportedException);
                    _class_testresult &= false;
                }
            }
            #endregion
            #endregion End Of SECTION IV

            #region SECTION V - STATIC PROPERTIES
            CommonLib.LogStatus("*** SECTION V - STATIC PROPERTIES *****");

            #region Test #1 - The ImageSourceProperty Property
            // Usage: DependencyProperty = ImageBrush.ImageSourceProperty (Read only)
            // Notes: Returns a Dependency Property for BitmapSource.
            CommonLib.LogStatus("Test #1 - The ImageSourceProperty Property");

            System.Windows.DependencyProperty ImageSourceDp = ImageBrush.ImageSourceProperty;

            _class_testresult &= _helper.CompareProp("ImageSourceProperty", (ImageSourceDp.OwnerType.ToString() + "::" + ImageSourceDp.PropertyType.ToString()), (_objectType + "::" + typeof(ImageSource).ToString()));
            #endregion

            #region Test #3 - The ViewboxUnitsProperty Property
            // Usage: DependencyProperty = ImageBrush.ViewboxUnitsProperty (Read only)
            // Notes: Returns a Dependency Property for ViewboxUnits
            CommonLib.LogStatus("Test #3 - The ViewboxUnitsProperty Property");

            System.Windows.DependencyProperty ViewboxUnitsDp = ImageBrush.ViewboxUnitsProperty;

            _class_testresult &= _helper.CompareProp("ViewboxUnitsProperty", (ViewboxUnitsDp.OwnerType.ToString() + "::" + ViewboxUnitsDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(BrushMappingMode).ToString()));
            #endregion

            #region Test #4 - The AlignmentXProperty Property
            // Usage: DependencyProperty = ImageBrush.AlignmentXProperty (Read only)
            // Notes: Returns a Dependency Property for AlignmentX.
            CommonLib.LogStatus("Test #4 - The AlignmentXProperty Property");

            System.Windows.DependencyProperty AlignmentXDp = ImageBrush.AlignmentXProperty;

            _class_testresult &= _helper.CompareProp("AlignmentXProperty", (AlignmentXDp.OwnerType.ToString() + "::" + AlignmentXDp.PropertyType.ToString()), (ParentClassType + "::" +typeof(AlignmentX).ToString()));
            #endregion

            #region Test #5 - The StretchProperty Property
            // Usage: DependencyProperty = ImageBrush.StretchProperty (Read only)
            // Notes: Returns a Dependency Property for Stretch
            CommonLib.LogStatus("Test #5 - The StretchProperty Property");

            System.Windows.DependencyProperty StretchDp = ImageBrush.StretchProperty;

            _class_testresult &= _helper.CompareProp("StretchProperty", (StretchDp.OwnerType.ToString() + "::" + StretchDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(Stretch).ToString()));
            #endregion

            #region Test #6 - The TileModeProperty Property
            // Usage: DependencyProperty = ImageBrush.TileModeProperty (Read only)
            // Notes: Returns a Dependency Property for TileMode
            CommonLib.LogStatus("Test #6 - The TileModeProperty Property");

            System.Windows.DependencyProperty TileModeDp = ImageBrush.TileModeProperty;

            _class_testresult &= _helper.CompareProp("TileModeProperty", (TileModeDp.OwnerType.ToString() + "::" + TileModeDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(TileMode).ToString()));
            #endregion

            #region Test #7 - The AlignmentYProperty Property
            // Usage: DependencyProperty = ImageBrush.AlignmentYProperty (Read only)
            // Notes: Returns a Dependency Property for AlignmentY
            CommonLib.LogStatus("Test #7 - The AlignmentYProperty Property");

            System.Windows.DependencyProperty AlignmentYDp = ImageBrush.AlignmentYProperty;

            _class_testresult &= _helper.CompareProp("AlignmentYProperty", (AlignmentYDp.OwnerType.ToString() + "::" + AlignmentYDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(AlignmentY).ToString()));
            #endregion

            #region Test #8 - The ViewboxProperty Property
            // Usage: DependencyProperty = ImageBrush.Viewbox (Read only)
            // Notes: Returns a Dependency Property for Viewbox
            CommonLib.LogStatus("Test #8 - The ViewboxProperty Property");

            System.Windows.DependencyProperty ViewboxDp = ImageBrush.ViewboxProperty;

            _class_testresult &= _helper.CompareProp("ViewboxProperty", (ViewboxDp.OwnerType.ToString() + "::" + ViewboxDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(Rect).ToString()));
            #endregion

            #region Test #9 - The ViewportProperty Property
            // Usage: DependencyProperty = ImageBrush.Viewport (Read only)
            // Notes: Returns a Dependency Property for Viewport
            CommonLib.LogStatus("Test #9 - The ViewportProperty Property");

            System.Windows.DependencyProperty ViewportDp = ImageBrush.ViewportProperty;

            _class_testresult &= _helper.CompareProp("ViewportProperty", (ViewportDp.OwnerType.ToString() + "::" + ViewportDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(Rect).ToString()));
            #endregion

            #region Test #10 - The ViewportUnitsProperty Property
            // Usage: DependencyProperty = ImageBrush.ViewportUnits (Read only)
            // Notes: Returns a Dependency Property for ViewportUnits
            CommonLib.LogStatus("Test #10 - The ViewportUnitsProperty Property");

            System.Windows.DependencyProperty ViewportUnitsDp = ImageBrush.ViewportUnitsProperty;

            _class_testresult &= _helper.CompareProp("ViewportUnitsProperty", (ViewportUnitsDp.OwnerType.ToString() + "::" + ViewportUnitsDp.PropertyType.ToString()), (ParentClassType + "::" + typeof(BrushMappingMode).ToString()));
            #endregion

            #region Test #11 - The OpacityProperty Property
            // Usage: DependencyProperty = ImageBrush.Opacity (Read only)
            // Notes: Returns a Dependency Property for Opacity.
            CommonLib.LogStatus("Test #11 - The OpacityProperty Property");

            System.Windows.DependencyProperty OpacityDp = ImageBrush.OpacityProperty;

            _class_testresult &= _helper.CompareProp("OpacityProperty", (OpacityDp.OwnerType.ToString() + "::" + OpacityDp.PropertyType.ToString()), (BaseClassType + "::" + typeof(Double).ToString()));
            #endregion

            #region Test #12 - The TransformProperty Property
            // Usage: DependencyProperty = ImageBrush.TransformProperty (Read only)
            // Notes: Returns a Dependency Property for Transform.
            CommonLib.LogStatus("Test #12 - The TransformProperty Property");

            System.Windows.DependencyProperty TransformDp = ImageBrush.TransformProperty;

            _class_testresult &= _helper.CompareProp("TransformProperty", (TransformDp.OwnerType.ToString() + "::" + TransformDp.PropertyType.ToString()), (BaseClassType + "::" + typeof(Transform).ToString()));
            #endregion

            #region Test #13 - The RelativeTransformProperty Property
            // Usage: DependencyProperty = ImageBrush.RelativeTransformProperty (Read only)
            // Notes: Returns a Dependency Property for RelativeTransform.
            CommonLib.LogStatus("Test #13 - The RelativeTransformProperty Property");

            System.Windows.DependencyProperty RelativeTransformDp = ImageBrush.RelativeTransformProperty;

            _class_testresult &= _helper.CompareProp("RelativeTransformProperty", (RelativeTransformDp.OwnerType.ToString() + "::" + RelativeTransformDp.PropertyType.ToString()), (BaseClassType + "::" + typeof(Transform).ToString()));
            #endregion
            #endregion End Of SECTION V

            CommonLib.LogTest("Results from :" + _objectType);

        }

        private bool _class_testresult;
        private Type _objectType;
        private HelperClass _helper;
    }
}
