// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Core Parser test automation 
// (C) Microsoft Corporation 2003
// File 
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
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
using System.Xml;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Avalon.Test.CoreUI.Common;

namespace Avalon.Test.CoreUI.Parser
{
    #region CustomControl
    /// <summary>Basic custom element derived from Control.</summary>
    public class CustomControl : Control
    {
        /// <summary>
        /// 
        /// </summary>
        public CustomControl() : base()
        {
        }
     
    }
    #endregion CustomControl
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
        /// 
        /// </summary>
        public CustomEnumControl() : base()
        {
        }



        /// <summary>
        /// Registers custom dependency properties.
        /// </summary>
        static CustomEnumControl()
        {
            CustomDockProperty = DependencyProperty.RegisterAttached("CustomDock", typeof(Dock), typeof(CustomEnumControl),
                                          new FrameworkPropertyMetadata(Dock.Left, FrameworkPropertyMetadataOptions.AffectsParentMeasure));
            CustomProperty = DependencyProperty.RegisterAttached("Custom", typeof(CustomEnum), typeof(CustomEnumControl),
                                          new FrameworkPropertyMetadata(CustomEnum.Value1));
            CustomAttachedBoolProperty = DependencyProperty.RegisterAttached("CustomAttachedBool", typeof(bool), typeof(CustomEnumControl));
        }

        /// <summary>
        /// The CustomDock dependency property. 
        /// </summary>
        public static readonly DependencyProperty CustomDockProperty;

        /// <summary>
        /// Get accessor for CustomDockProperty.
        /// </summary>
        public static Dock GetCustomDock(DependencyObject d)
        { return (Dock)d.GetValue(CustomDockProperty); }

        /// <summary>
        /// Set accessor for CustomDockProperty.
        /// </summary>
        public static void SetCustomDock(DependencyObject d, Dock value)
        { d.SetValue(CustomDockProperty, value); }

        /// <summary>
        /// The CustomAttached dependency property. 
        /// </summary>
        public static readonly DependencyProperty CustomProperty;

        /// <summary>
        /// Get accessor for CustomProperty.
        /// </summary>
        public static CustomEnum GetCustom(DependencyObject d)
        { return (CustomEnum)d.GetValue(CustomProperty); }

        /// <summary>
        /// Set accessor for CustomProperty.
        /// </summary>
        public static void SetCustom(DependencyObject d, CustomEnum value)
        { d.SetValue(CustomProperty, value); }

        /// <summary>
        /// The CustomAttachedBool dependency property. 
        /// </summary>
        public static readonly DependencyProperty CustomAttachedBoolProperty;

        /// <summary>
        /// Get accessor for CustomAttachedBoolProperty.
        /// </summary>
        public static bool GetCustomAttachedBool(DependencyObject d)
        { return (bool)d.GetValue(CustomAttachedBoolProperty); }

        /// <summary>
        /// Set accessor for CustomAttachedBoolProperty.
        /// </summary>
        public static void SetCustomAttachedBool(DependencyObject d, bool value)
        { d.SetValue(CustomAttachedBoolProperty, value); }

        /// <summary>
        /// CLR property.
        /// </summary>
        public static bool PublicStaticBoolClrProperty
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// CLR field.
        /// </summary>
        public static bool PublicStaticBoolClrField = false;
    }
    #endregion CustomEnumControl
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
        /// 
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
        /// Create Event Handler when Render is called.
        /// </summary>
        /// <param name="ctx"></param>
        protected override void OnRender(DrawingContext ctx)
        {
            if (RenderCalledEvent != null)
            {
                RenderCalledEvent(this, new EventArgs());
            }
        }

        /// <summary>
        /// 
        /// </summary> 
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
        /// <param name="child"></param>
        public void CallAddLogicalChild(object child)
        {
            AddLogicalChild(child);
        }

        /// <summary>
        /// 
        /// </summary>
        private LogicalChildCollection _logicalChildren;     
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
        /// 
        /// </summary>
        public Item() : base()
        {
            _content = new LogicalChildCollection(this, true);
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LogicalChildCollection Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
            }
        }

        /// <summary>
        /// This method is to be called to update the logical tree. 
        /// </summary>
        /// <param name="child"></param>
        public void CallAddLogicalChild(object child)
        {
            AddLogicalChild(child);
        }

        LogicalChildCollection _content;

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ColorProperty;
        /// <summary>
        /// 
        /// </summary>
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
        /// 
        /// </summary>
        public static readonly DependencyProperty CCWidthProperty;
        /// <summary>
        /// 
        /// </summary>
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
        /// 
        /// </summary>
        public static readonly DependencyProperty CCHeightProperty;
        /// <summary>
        /// 
        /// </summary>
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
        /// 
        /// </summary>
        public new static readonly DependencyProperty BorderBrushProperty;
        /// <summary>
        /// 
        /// </summary>
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
        /// 
        /// </summary>
        public static readonly DependencyProperty BackgroundBrushProperty;
        /// <summary>
        /// 
        /// </summary>
        
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
        ///   Gradient brush for Freezable pattern testing
        /// </summary>
        public static readonly DependencyProperty SolidBackgroundBrushProperty;

        /// <summary>
        /// 
        /// </summary>
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
        /// 
        /// </summary>
        public Label() : base()
        {
            _logicalChildren = new LogicalChildCollection(this, true);

            MouseEnter += new MouseEventHandler(OnMouseTestEnter);
        }

        /// <summary>
        /// 
        /// </summary>
        static Label()
        {
            FrameworkPropertyMetadata fMeta;
            TextProperty = DependencyProperty.RegisterAttached("Text", typeof(string), typeof(Label));
            fMeta = new FrameworkPropertyMetadata(string.Empty);
            fMeta.AffectsMeasure = true;
            TextProperty.OverrideMetadata(typeof(Label), fMeta);
            FakeFontSizeProperty = DependencyProperty.RegisterAttached("FakeFontSize", typeof(double), typeof(Label));
            fMeta = new FrameworkPropertyMetadata(10.0 * (96.0 / 72.0));
            fMeta.AffectsMeasure = true;
            //fMeta.PropertyChangedCallback = new PropertyChangedCallback(OnFakeFontSizeInvalidated);
            //fMeta.GetValueOverride = new GetValueOverride(FakeFontSizeCachingAccessor);
            FakeFontSizeProperty.OverrideMetadata(typeof(Label), fMeta);
            ForegroundProperty = DependencyProperty.RegisterAttached("Foreground", typeof(Brush), typeof(Label));
            fMeta = new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromScRgb(1.0f, 0, 0, 0)));
            fMeta.AffectsRender = true;
            //fMeta.PropertyChangedCallback = new PropertyChangedCallback(OnForegroundInvalidated);
            //fMeta.GetValueOverride = new GetValueOverride(ForegroundCachingAccessor);
            ForegroundProperty.OverrideMetadata(typeof(Label), fMeta);
        }
        /// <summary>
        /// Event OnMouseEnter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseTestEnter(object sender, MouseEventArgs e)
        {
        }


        /// <summary>
        /// 
        /// </summary>
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
        /// <param name="child"></param>
        public void CallAddLogicalChild(object child)
        {
            AddLogicalChild(child);
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TextProperty;
        /// <summary>
        /// The Text property
        /// </summary>
        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty FakeFontSizeProperty;
        /// <summary>
        /// The FakeFontSize property describes the desired size of the text.
        /// </summary>
        public double FakeFontSize
        {
            get { return (double) GetValue(FakeFontSizeProperty); }
            set { SetValue(FakeFontSizeProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public new static readonly DependencyProperty ForegroundProperty;
        /// <summary>
        /// 
        /// </summary>
        public new Brush Foreground
        {
            get { return (Brush) GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        private LogicalChildCollection _logicalChildren;
    }
    #endregion Label
    #region CLRObject
    /// <summary>
    /// 
    /// </summary>
    public class ClrObject1
    {
        /// <summary>
        /// 
        /// </summary>
        public string _prop = "Public"; // Set some known default value
        /// <summary>
        /// 
        /// </summary>
        public string Prop
        {
            set { _prop = value; }
            get { return _prop; }
        }
        /// <summary>
        /// 
        /// </summary>
        private string _testprop = "Private";
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
        protected string _testpro = "Protected";
        /// <summary>
        /// 
        /// </summary>
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
        /// 
        /// </summary>
        internal string _testint = "Internal";
        /// <summary>
        /// 
        /// </summary>
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
    }
    /// <summary>
    /// 
    /// </summary>
    class ClrPrivate
    {
        /// <summary>
        /// 
        /// </summary>
        private ClrPrivate ()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        private string _prop = "Private";

        /// <summary>
        /// 
        /// </summary>
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
    /// 
    /// </summary>
    internal sealed class ClrInternal
    {
        /// <summary>
        /// 
        /// </summary>
        public ClrInternal ()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        internal string _prop = "Internal";
        /// <summary>
        /// 
        /// </summary>
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
    /// 
    /// </summary>
    class ClrProtected
    {
        /// <summary>
        /// 
        /// </summary>
        protected ClrProtected ()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        protected string _prop = "Protected"; // Set some known default value

        /// <summary>
        /// 
        /// </summary>
        protected string Prop
        {
            set { _prop = value; }
            get { return _prop; }
        }
    }
    #endregion CLRObject
    #region FakeBrush
    /// <summary>
    /// 
    /// </summary>
    [TypeConverter(typeof(BrushConverter))] 
    public abstract class FakeBrush
    {
        /// <summary>
        /// 
        /// </summary>
        public FakeBrush ()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="opacity"></param>
        public FakeBrush (double opacity)
        {
            _opacity = opacity;
        }
        /// <summary>
        /// 
        /// </summary>
        [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
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
        /// <summary>
        /// 
        /// </summary>
        protected double _opacity = 1.0;
    }
    #endregion FakeBrush
    #region FakeSolidColorBrush
    /// <summary>
    /// Simple, flat example resource
    /// </summary>
    public class FakeSolidColorBrush : FakeBrush
    {
        /// <summary>
        /// 
        /// </summary>
        public FakeSolidColorBrush ()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="opacity"></param>
        public FakeSolidColorBrush (string color, double opacity) : base(opacity)
        {
            _color = color;
        }
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
        private string _color;
    }
    #endregion FakeSolidColorBrush
    
    #region ChangeableTest
    /// <summary>
    /// Instantiating of Freezable used for resource test
    /// </summary>
    public class ChangeableTest : Freezable
    {
        /// <summary>
        /// 
        /// </summary>
        private int _isChangeTest;
        /// <summary>
        /// 
        /// </summary>
        public int IsChangeTest
        {
            get { return _isChangeTest; }
            set { _isChangeTest = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="theValue"></param>
        public ChangeableTest(int theValue)
        {
            _isChangeTest = theValue;
        }
        /// <summary>
        /// 
        /// </summary>
        public ChangeableTest()
        {
            _isChangeTest = 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        public new ChangeableTest GetAsFrozen()
        {
            return (ChangeableTest)base.GetAsFrozen();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        protected override void CloneCore(Freezable sourceFreezable)
        {
            ChangeableTest changeableTest = (ChangeableTest)sourceFreezable;
      
            base.CloneCore(sourceFreezable);
            _isChangeTest = changeableTest._isChangeTest;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        protected override void GetAsFrozenCore(Freezable sourceFreezable)
        {
            ChangeableTest changeableTest = (ChangeableTest)sourceFreezable;
            
            base.GetAsFrozenCore(sourceFreezable);
            _isChangeTest = changeableTest._isChangeTest;
        }
        /// <returns></returns>
        protected override void GetCurrentValueAsFrozenCore(Freezable sourceFreezable)
        {
            ChangeableTest changeableTest = (ChangeableTest)sourceFreezable;

            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            _isChangeTest = changeableTest._isChangeTest;
        }/// <summary>
        /// 
        /// </summary>
        /// <param></param>
        /// <returns></returns>
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
        /// 
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
        /// <summary>
        /// 
        /// </summary>
        public TestDataBind() : this("Hello world", "red")
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="color"></param>
        public TestDataBind(string s, string color)
        {
            _string = s;
            _color = color;
        }
        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 
        /// </summary>
        public string String
        {
            get
            {
                return _string;
            }
            set
            {
                string old = _string;

                _string = value;
                RaisePropertyChangedEvent("String", old, _string);
            }
        }
        /// <summary>
        /// 
        /// </summary>
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
        /// 
        /// </summary>
        /// <param name="newString"></param>
        public void ChangeAll(string newString)
        {
            _string = newString;
            if(PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(null)); // null means all bound properties should be considered "changed"
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        void RaisePropertyChangedEvent(string name, object oldValue, object newValue)
        {
            if(PropertyChanged != null && oldValue != newValue)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        string _string = "This is DataBind Test...";
        string _color = "blue";
    }
    #endregion TestDataBind
    #region TestIList
    /// <summary>
    /// 
    /// </summary>
    public class Human
    {
        string _name;
        ArrayList _friends = new ArrayList();
        /// <summary>
        /// 
        /// </summary>
        public Human()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        public Human(string Name)
        {
            this._name = Name;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public ArrayList Friends
        {
            get
            {
                return _friends;
            }
        }
    }
    #endregion TestIList
}
