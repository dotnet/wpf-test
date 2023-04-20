// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Core Parser test automation 
// (C) Microsoft Corporation 2003
// File 
//      CustomControl.cs
// Description 
//      Contains Custom control
//             - PanelFlow 
//             - Item
//             - Label
//             - ILoadControl
//             - Changable Test
//             - Open File Tag
//             - Test Databind
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;

namespace Microsoft.Test.Xaml.Types
{   
    #region CustomEnumControl

    /// <summary>
    /// Custom Enum used for values of CustomProperty.
    /// </summary>
    public enum CustomEnum
    {
        /// <summary>
        /// Symbolizes a possible value of our custom Enum.
        /// </summary>
        Value1,

        /// <summary>
        /// Symbolizes a possible value of our custom Enum.
        /// </summary>
        Value2
    }

    /// <summary>Custom element used for tests of Parser's Enum syntax support.</summary>
    public class CustomEnumControl : CustomControl
    {
        /// <summary>
        /// The CustomDock dependency property. 
        /// </summary>
        public static readonly DependencyProperty CustomDockProperty;

        /// <summary>
        /// The CustomAttached dependency property. 
        /// </summary>
        public static readonly DependencyProperty CustomProperty;

        /// <summary>
        /// The CustomAttachedBool dependency property. 
        /// </summary>
        public static readonly DependencyProperty CustomAttachedBoolProperty;

        /// <summary>
        /// CLR field.
        /// </summary>
        private static bool s_publicStaticBoolClrField = false;

        /// <summary>
        /// Initializes static members of the CustomEnumControl class.
        /// </summary>
        static CustomEnumControl()
        {
            CustomDockProperty = DependencyProperty.RegisterAttached(
                                                                "CustomDock", 
                                                                 typeof(Dock), 
                                                                 typeof(CustomEnumControl),
                                                                 new FrameworkPropertyMetadata(Dock.Left, FrameworkPropertyMetadataOptions.AffectsParentMeasure));
            CustomProperty = DependencyProperty.RegisterAttached(
                                                            "Custom", 
                                                              typeof(CustomEnum), 
                                                              typeof(CustomEnumControl),
                                                              new FrameworkPropertyMetadata(CustomEnum.Value1));
            CustomAttachedBoolProperty = DependencyProperty.RegisterAttached("CustomAttachedBool", typeof(bool), typeof(CustomEnumControl));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomEnumControl"/> class.
        /// </summary>
        public CustomEnumControl() : base()
        {
        }

        /// <summary>
        /// Gets a value indicating whether [public static bool CLR property].
        /// </summary>
        /// <value>
        /// <c>true</c> if [public static bool CLR property]; otherwise, <c>false</c>.
        /// </value>
        public static bool PublicStaticBoolClrProperty
        {
            get
            {
                return s_publicStaticBoolClrField;
            }
        }

        /// <summary>
        /// Get accessor for CustomDockProperty.
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <returns>Dock value </returns>
        public static Dock GetCustomDock(DependencyObject d)
        {
            return (Dock) d.GetValue(CustomDockProperty);
        }

        /// <summary>
        /// Set accessor for CustomDockProperty.
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="value">The value.</param>
        public static void SetCustomDock(DependencyObject d, Dock value)
        {
            d.SetValue(CustomDockProperty, value);
        }
        
        /// <summary>
        /// Get accessor for CustomProperty.
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <returns>CustomEnum value</returns>
        public static CustomEnum GetCustom(DependencyObject d)
        {
            return (CustomEnum) d.GetValue(CustomProperty);
        }

        /// <summary>
        /// Set accessor for CustomProperty.
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="value">The value.</param>
        public static void SetCustom(DependencyObject d, CustomEnum value)
        {
            d.SetValue(CustomProperty, value);
        }

        /// <summary>
        /// Get accessor for CustomAttachedBoolProperty.
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <returns>bool value</returns>
        public static bool GetCustomAttachedBool(DependencyObject d)
        {
            return (bool) d.GetValue(CustomAttachedBoolProperty);
        }

        /// <summary>
        /// Set accessor for CustomAttachedBoolProperty.
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetCustomAttachedBool(DependencyObject d, bool value)
        {
            d.SetValue(CustomAttachedBoolProperty, value);
        }
    }

    #endregion CustomEnumControl

    #region CustomControl

    /// <summary>Basic custom element derived from Control.</summary>
    public class CustomControl : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomControl"/> class.
        /// </summary>
        public CustomControl() : base()
        {
        }
    }

    #endregion CustomControl

    #region PanelFlow

    /// <summary>Custom Control with ContentProperty for Core Parser Test.</summary>
    /// <remarks>
    /// Create custom control to with ContentProperty for Parser Test.
    /// <para>
    ///     Contains Custom control
    ///             - PanelFlow 
    ///             - CustomControl
    ///             - Item
    ///             - Label
    /// </para> 
    /// </remarks>
    [ContentProperty("Children")]
    public class PanelFlow : CustomControl, ILogicalParent
    {
        /// <summary>
        /// Logical ChildCollection
        /// </summary>
        private readonly LogicalChildCollection _logicalChildren;

        /// <summary>
        /// Initializes a new instance of the <see cref="PanelFlow"/> class.
        /// </summary>
        public PanelFlow()
            : base()
        {
            _logicalChildren = new LogicalChildCollection(this, true);
        }

        /// <summary>
        /// Create Event Handler when Render is called.
        /// </summary>
        public event EventHandler RenderCalledEvent;

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LogicalChildCollection Children
        {
            get
            {
                return _logicalChildren;
            }
        }

        /// <summary>
        /// This method is to be called to update the logical tree.
        /// </summary>
        /// <param name="child">object value</param>
        public void CallAddLogicalChild(object child)
        {
            AddLogicalChild(child);
        }

        /// <summary>
        /// Create Event Handler when Render is called.
        /// </summary>
        /// <param name="ctx">The DrawingContext.</param>
        protected override void OnRender(DrawingContext ctx)
        {
            if (RenderCalledEvent != null)
            {
                RenderCalledEvent(this, new EventArgs());
            }
        }
    }

    #endregion PanelFlow

    #region Item

    /// <summary>
    /// Item custom control to add in PanelFlow for Parser Testing
    /// Custom Control with ContentProperty for Core Parser Test.</summary>
    /// <remarks>
    /// Create custom control with ContentProperty for Parser Test.
    /// <para>
    ///     Contains Custom control
    ///             - PanelFlow 
    ///             - CustomControl
    ///             - Item
    ///             - Label
    /// </para> 
    /// </remarks>
    [ContentProperty("Content")]
    public class Item : CustomControl, ILogicalParent
    {
        /// <summary>
        /// Dependency Property ColorProperty
        /// </summary>
        public static readonly DependencyProperty ColorProperty;

        /// <summary>
        /// DependencyProperty CCWidthProperty
        /// </summary>
        public static readonly DependencyProperty CCWidthProperty;

        /// <summary>
        /// DependencyProperty CCHeightProperty;
        /// </summary>
        public static readonly DependencyProperty CCHeightProperty;

        /// <summary>
        /// DependencyProperty BorderBrushProperty
        /// </summary>
        public static new readonly DependencyProperty BorderBrushProperty;

        /// <summary>
        /// DependencyProperty BackgroundBrushProperty
        /// </summary>
        public static readonly DependencyProperty BackgroundBrushProperty;

        /// <summary>
        ///   Gradient brush for Freezable pattern testing
        /// </summary>
        public static readonly DependencyProperty SolidBackgroundBrushProperty;

        /// <summary>
        /// Initializes static members of the Item class.
        /// </summary>
        static Item()
        {
            FrameworkPropertyMetadata metadata;
            ColorProperty = DependencyProperty.RegisterAttached("Color", typeof(string), typeof(Item));
            CCWidthProperty = DependencyProperty.RegisterAttached("CCWidth", typeof(double), typeof(Item));
            metadata = new FrameworkPropertyMetadata(double.PositiveInfinity);
            CCWidthProperty.OverrideMetadata(typeof(Item), metadata);
            CCHeightProperty = DependencyProperty.RegisterAttached("CCHeight", typeof(double), typeof(Item));
            metadata = new FrameworkPropertyMetadata(double.PositiveInfinity);
            CCHeightProperty.OverrideMetadata(typeof(Item), metadata);
            BorderBrushProperty = DependencyProperty.RegisterAttached("BorderBrush", typeof(Brush), typeof(Item));
            metadata = new FrameworkPropertyMetadata(Brushes.Gray);
            BorderBrushProperty.OverrideMetadata(typeof(Item), metadata);
            BackgroundBrushProperty = DependencyProperty.RegisterAttached("BackgroundBrush", typeof(Brush), typeof(Item));
            metadata = new FrameworkPropertyMetadata(Brushes.White);
            BackgroundBrushProperty.OverrideMetadata(typeof(Item), metadata);
            SolidBackgroundBrushProperty = DependencyProperty.RegisterAttached("SolidBackgroundBrush", typeof(FakeBrush), typeof(Item));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        public Item() : base()
        {
            Content = new LogicalChildCollection(this, true);
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        public string Color
        {
            get
            {
                return (string)GetValue(ColorProperty);
            }

            set
            {
                SetValue(ColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the CC.
        /// </summary>
        /// <value>The width of the CC.</value>
        public double CCWidth
        {
            get
            {
                return (double)GetValue(CCWidthProperty);
            }

            set
            {
                SetValue(CCWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of the CC.
        /// </summary>
        /// <value>The height of the CC.</value>
        public double CCHeight
        {
            get
            {
                return (double)GetValue(CCHeightProperty);
            }

            set
            {
                SetValue(CCHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the border brush.
        /// </summary>
        /// <value>The border brush.</value>
        public new Brush BorderBrush
        {
            get
            {
                return (Brush)GetValue(BorderBrushProperty);
            }

            set
            {
                SetValue(BorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the background brush.
        /// </summary>
        /// <value>The background brush.</value>
        public Brush BackgroundBrush
        {
            get
            {
                return (Brush)GetValue(BackgroundBrushProperty);
            }

            set
            {
                SetValue(BackgroundBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the solid background brush.
        /// </summary>
        /// <value>The solid background brush.</value>
        public FakeBrush SolidBackgroundBrush
        {
            get
            {
                return (FakeBrush)GetValue(SolidBackgroundBrushProperty);
            }

            set
            {
                SetValue(SolidBackgroundBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LogicalChildCollection Content { get; set; }

        /// <summary>
        /// This method is to be called to update the logical tree.
        /// </summary>
        /// <param name="child">object value </param>
        public void CallAddLogicalChild(object child)
        {
            AddLogicalChild(child);
        }
    }

    #endregion Item

    #region Label

    /// <summary>Custom Control adding IAddChild for Core Parser Test.</summary>
    /// <remarks>
    /// Create custom control to implement IAddChild for Parser Test.
    /// <para>
    ///     Contains Custom control
    ///             - PanelFlow 
    ///             - CustomControl
    ///             - Item
    ///             - Label
    /// </para> 
    /// </remarks>
    [ContentProperty("LogicalChildren")]
    public class Label : CustomControl, ILogicalParent
    {
        /// <summary>
        /// DependencyProperty TextProperty
        /// </summary>
        public static readonly DependencyProperty TextProperty;

        /// <summary>
        /// DependencyProperty FakeFontSizeProperty
        /// </summary>
        public static readonly DependencyProperty FakeFontSizeProperty;

        /// <summary>
        /// DependencyProperty ForegroundProperty
        /// </summary>
        public static new readonly DependencyProperty ForegroundProperty;

        /// <summary>
        /// Logical ChildCollection
        /// </summary>
        private readonly LogicalChildCollection _logicalChildren;

        /// <summary>
        /// Initializes static members of the Label class.
        /// </summary>
        static Label()
        {
            FrameworkPropertyMetadata frameworkPropMeta;
            TextProperty = DependencyProperty.RegisterAttached("Text", typeof(string), typeof(Label));
            frameworkPropMeta = new FrameworkPropertyMetadata(string.Empty);
            frameworkPropMeta.AffectsMeasure = true;
            TextProperty.OverrideMetadata(typeof(Label), frameworkPropMeta);
            FakeFontSizeProperty = DependencyProperty.RegisterAttached("FakeFontSize", typeof(double), typeof(Label));
            frameworkPropMeta = new FrameworkPropertyMetadata(10.0 * (96.0 / 72.0));
            frameworkPropMeta.AffectsMeasure = true;
            //// fMeta.PropertyChangedCallback = new PropertyChangedCallback(OnFakeFontSizeInvalidated);
            //// fMeta.GetValueOverride = new GetValueOverride(FakeFontSizeCachingAccessor);
            FakeFontSizeProperty.OverrideMetadata(typeof(Label), frameworkPropMeta);
            ForegroundProperty = DependencyProperty.RegisterAttached("Foreground", typeof(Brush), typeof(Label));
            frameworkPropMeta = new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromScRgb(1.0f, 0, 0, 0)));
            frameworkPropMeta.AffectsRender = true;
            //// fMeta.PropertyChangedCallback = new PropertyChangedCallback(OnForegroundInvalidated);
            //// fMeta.GetValueOverride = new GetValueOverride(ForegroundCachingAccessor);
            ForegroundProperty.OverrideMetadata(typeof(Label), frameworkPropMeta);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Label"/> class.
        /// </summary>
        public Label() : base()
        {
            _logicalChildren = new LogicalChildCollection(this, true);

            MouseEnter += new MouseEventHandler(OnMouseTestEnter);
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text value.</value>
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }

            set
            {
                SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the size of the fake font.
        /// The FakeFontSize property describes the desired size of the text.
        /// </summary>
        /// <value>The size of the fake font.</value>
        public double FakeFontSize
        {
            get
            {
                return (double)GetValue(FakeFontSizeProperty);
            }

            set
            {
                SetValue(FakeFontSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the foreground.
        /// </summary>
        /// <value>The foreground.</value>
        public new Brush Foreground
        {
            get
            {
                return (Brush)GetValue(ForegroundProperty);
            }

            set
            {
                SetValue(ForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets the logical children.
        /// </summary>
        /// <value>The logical children.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public new LogicalChildCollection LogicalChildren
        {
            get
            {
                return _logicalChildren;
            }
        }

        /// <summary>
        /// This method is to be called to update the logical tree.
        /// </summary>
        /// <param name="child">object value</param>
        public void CallAddLogicalChild(object child)
        {
            AddLogicalChild(child);
        }

        /// <summary>
        /// Event OnMouseEnter
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void OnMouseTestEnter(object sender, MouseEventArgs e)
        {
        }
    }

    #endregion Label

    #region FakeBrush

    /// <summary>
    /// FakeBrush class
    /// </summary>
    [TypeConverter(typeof(BrushConverter))]
    public abstract class FakeBrush
    {
        /// <summary>
        /// Brush opacity
        /// </summary>
        private double _opacity = 1.0;

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeBrush"/> class.
        /// </summary>
        public FakeBrush()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeBrush"/> class.
        /// </summary>
        /// <param name="opacity">The opacity.</param>
        public FakeBrush(double opacity)
        {
            this._opacity = opacity;
        }

        /// <summary>
        /// Gets or sets the opacity.
        /// </summary>
        /// <value>The opacity.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public double Opacity
        {
            get
            {
                return _opacity;
            }

            set
            {
                if (_opacity != value)
                {
                    _opacity = value;
                }
            }
        }
    }

    #endregion FakeBrush

    #region FakeSolidColorBrush

    /// <summary>
    /// Simple, flat example resource
    /// </summary>
    public class FakeSolidColorBrush : FakeBrush
    {
        /// <summary>
        /// Color variable
        /// </summary>
        private string _color;

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeSolidColorBrush"/> class.
        /// </summary>
        public FakeSolidColorBrush()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeSolidColorBrush"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="opacity">The opacity.</param>
        public FakeSolidColorBrush(string color, double opacity) : base(opacity)
        {
            this._color = color;
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        public string Color
        {
            get
            {
                return _color;
            }

            set
            {
                if (_color != value)
                {
                    _color = value;
                }
            }
        }
    }

    #endregion FakeSolidColorBrush

    #region ChangeableTest

    /// <summary>
    /// Instantiating of Freezable used for resource test
    /// </summary>
    public class ChangeableTest : Freezable
    {
        /// <summary>
        /// change test value
        /// </summary>
        private int _isChangeTest;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeableTest"/> class.
        /// </summary>
        /// <param name="theValue">The value.</param>
        public ChangeableTest(int theValue)
        {
            _isChangeTest = theValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeableTest"/> class.
        /// </summary>
        public ChangeableTest()
        {
            _isChangeTest = 0;
        }

        /// <summary>
        /// Gets or sets the is change test.
        /// </summary>
        /// <value>The is change test.</value>
        public int IsChangeTest
        {
            get
            {
                return _isChangeTest;
            }

            set
            {
                _isChangeTest = value;
            }
        }

        /// <summary>
        /// Gets as frozen.
        /// </summary>
        /// <returns>Changeable Test</returns>
        public new ChangeableTest GetAsFrozen()
        {
            return (ChangeableTest) base.GetAsFrozen();
        }

        /// <summary>
        /// Clones the core.
        /// </summary>
        /// <param name="sourceFreezable">The source freezable.</param>
        protected override void CloneCore(Freezable sourceFreezable)
        {
            ChangeableTest changeableTest = (ChangeableTest) sourceFreezable;

            base.CloneCore(sourceFreezable);
            _isChangeTest = changeableTest._isChangeTest;
        }

        /// <summary>
        /// Gets as frozen core.
        /// </summary>
        /// <param name="sourceFreezable">The source freezable.</param>
        protected override void GetAsFrozenCore(Freezable sourceFreezable)
        {
            ChangeableTest changeableTest = (ChangeableTest) sourceFreezable;

            base.GetAsFrozenCore(sourceFreezable);
            _isChangeTest = changeableTest._isChangeTest;
        }

        /// <summary>
        /// Gets the current value as frozen core.
        /// </summary>
        /// <param name="sourceFreezable">The source freezable.</param>
        protected override void GetCurrentValueAsFrozenCore(Freezable sourceFreezable)
        {
            ChangeableTest changeableTest = (ChangeableTest) sourceFreezable;

            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            _isChangeTest = changeableTest._isChangeTest;
        }

        /// <summary>
        /// Creates the instance core.
        /// </summary>
        /// <returns>Freezable value</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new ChangeableTest();
        }
    }

    #endregion ChangeableTest

    #region OpenTextTag

    /// <summary>
    /// For Testing parsing unsecure tag.
    /// </summary>
    public class OpenTextTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenTextTag"/> class.
        /// </summary>
        public OpenTextTag()
        {
            // Partial trust AppDomain is given FileIOPermissionAccess.Read permission at site-of-origin
            // which causes TestUnSecureTag test under XAML\Parser\ClrObject to fail
            // Fixing the test by going one level up to escape the site-of-origin
            File.OpenText(@"..\clrobject.xaml").ReadToEnd();
        }
    }

    #endregion OpenTextTag

    #region TestDataBind

    /// <summary>
    /// A data item, used as a source for databinding in the Parser Test
    /// </summary>
    public class TestDataBind : INotifyPropertyChanged
    {
        /// <summary> data string </summary>
        private string _dataString = "This is DataBind Test...";

        /// <summary> color string </summary>
        private string _color = "blue";

        /// <summary>
        /// Initializes a new instance of the <see cref="TestDataBind"/> class.
        /// </summary>
        public TestDataBind() : this("Hello world", "red")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestDataBind"/> class.
        /// </summary>
        /// <param name="s">The string s.</param>
        /// <param name="color">The color.</param>
        public TestDataBind(string s, string color)
        {
            _dataString = s;
            this._color = color;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the string.
        /// </summary>
        /// <value>The string.</value>
        public string String
        {
            get
            {
                return _dataString;
            }

            set
            {
                string old = _dataString;

                _dataString = value;
                RaisePropertyChangedEvent("String", old, _dataString);
            }
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        public string Color
        {
            get
            {
                return _color;
            }

            set
            {
                string old = _color;

                _color = value;
                RaisePropertyChangedEvent("Color", old, _color);
            }
        }

        /// <summary>
        /// Changes all.
        /// </summary>
        /// <param name="newString">The new string.</param>
        public void ChangeAll(string newString)
        {
            _dataString = newString;
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(null)); // null means all bound properties should be considered "changed"
            }
        }

        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="name">The name prop.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void RaisePropertyChangedEvent(string name, object oldValue, object newValue)
        {
            if (PropertyChanged != null && oldValue != newValue)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    #endregion TestDataBind

    #region TestIList

    /// <summary>
    /// Human test class
    /// </summary>
    public class Human
    {
        /// <summary>
        /// Array list of friends
        /// </summary>
        private readonly ArrayList _friends = new ArrayList();

        /// <summary>
        /// Initializes a new instance of the <see cref="Human"/> class.
        /// </summary>
        public Human()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Human"/> class.
        /// </summary>
        /// <param name="name">The name prop.</param>
        public Human(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name prop.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets the friends.
        /// </summary>
        /// <value>The friends.</value>
        public ArrayList Friends
        {
            get
            {
                return _friends;
            }
        }
    }

    #endregion TestIList

    #region CLRObject

    /// <summary>
    /// ClrObject1 class
    /// </summary>
    public class ClrObject1
    {
        /// <summary>
        /// string property
        /// </summary>
        private string _prop = "Public"; // Set some known default value

        /// <summary>
        /// test property 
        /// </summary>
        private string _testprop = "Private";

        /// <summary>
        /// test string prop
        /// </summary>
        private string _testpro = "Protected";

        /// <summary>
        /// Test int var
        /// </summary>
        private string _testint = "Internal";

        /// <summary>
        /// Gets or sets the prop.
        /// </summary>
        /// <value>The prop value.</value>
        public string Prop
        {
            set
            {
                _prop = value;
            }

            get
            {
                return _prop;
            }
        }

        /// <summary>
        /// Gets or sets the test int.
        /// </summary>
        /// <value>The test int.</value>
        internal string TestInt
        {
            set
            {
                _testint = value;
            }

            get
            {
                return _testint;
            }
        }

        /// <summary>
        /// Gets or sets the test pro.
        /// </summary>
        /// <value>The test pro.</value>
        protected string TestPro
        {
            set
            {
                _testpro = value;
            }

            get
            {
                return _testpro;
            }
        }

        /// <summary>
        /// Gets or sets the test prop.
        /// </summary>
        /// <value>The test prop.</value>
        private string TestProp
        {
            set
            {
                _testprop = value;
            }

            get
            {
                return _testprop;
            }
        }
    }

    /// <summary>
    /// ClrPrivate class
    /// </summary>
    internal class ClrPrivate
    {
        /// <summary>
        /// String prop
        /// </summary>
        private string _prop = "Private";

        /// <summary>
        /// Prevents a default instance of the ClrPrivate class from being created
        /// Initializes a new instance of the <see cref="ClrPrivate"/> class.
        /// </summary>
        private ClrPrivate()
        {
        }

        /// <summary>
        /// Gets or sets the prop.
        /// </summary>
        /// <value>The prop value.</value>
        private string Prop
        {
            set
            {
                _prop = value;
            }

            get
            {
                return _prop;
            }
        }
    }

    /// <summary>
    /// ClrInternal class
    /// </summary>
    internal sealed class ClrInternal
    {
        /// <summary>
        /// String Property
        /// </summary>
        private string _prop = "Internal";

        /// <summary>
        /// Initializes a new instance of the <see cref="ClrInternal"/> class.
        /// </summary>
        public ClrInternal()
        {
        }

        /// <summary>
        /// Gets or sets the prop.
        /// </summary>
        /// <value>The prop value.</value>
        internal string Prop
        {
            set
            {
                _prop = value;
            }

            get
            {
                return _prop;
            }
        }
    }

    /// <summary>
    /// ClrProtected class
    /// </summary>
    internal class ClrProtected
    {
        /// <summary>
        /// string property
        /// </summary>
        private string _prop = "Protected"; // Set some known default value

        /// <summary>
        /// Initializes a new instance of the <see cref="ClrProtected"/> class.
        /// </summary>
        protected ClrProtected()
        {
        }

        /// <summary>
        /// Gets or sets the prop.
        /// </summary>
        /// <value>The prop value.</value>
        protected string Prop
        {
            set
            {
                _prop = value;
            }

            get
            {
                return _prop;
            }
        }
    }

    #endregion CLRObject
}
