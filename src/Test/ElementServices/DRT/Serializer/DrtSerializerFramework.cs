// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Framework used by DrtSerializer
//
//

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;

using System.Windows.Media;
using System.Windows.Markup;
using System.Xml;

namespace DRT
{
    [ContentProperty("Children")]
    public class Panel : FrameworkElement, IAddChild, ILogicalTreeParent
    {
        #region Construction

        public Panel()
            : base()
        {
            _self = null;
        }

        #endregion Construction

        #region IAddChild

        /// <summary>
        ///  Add a text string to this control
        /// </summary>
        void IAddChild.AddText(string text)
        {
        }

        /// <summary>
        ///  Add an object child to this control
        /// </summary>
        void IAddChild.AddChild(object o)
        {
            Children.Add((UIElement)o);
        }

        #endregion IAddChild

        #region Properties

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public UIElementCollection2 Children
        {
            get
            {
                if (_children == null)
                    _children = new UIElementCollection2(this);
                return _children;
            }
        }

        /// <summary>
        ///     This tests that an infinite loop is detected
        /// </summary>
        [DefaultValue(null)]
        public Panel Self
        {
            get { return _self; }
            set { _self = value; }
        }

        #endregion Properties

        /// <summary>
        ///   Derived class must implement to support Visual children. The method must return
        ///    the child at the specified index. Index must be between 0 and GetVisualChildrenCount-1.
        ///
        ///    By default a Visual does not have any children.
        ///
        ///  Remark: 
        ///       During this virtual call it is not valid to modify the Visual tree. 
        /// </summary>
        protected override Visual GetVisualChild(int index)
        {
            if (Children == null)
            {
                throw new ArgumentOutOfRangeException("index is out of range");
            }
            if (index < 0 || index >= Children.Count)
            {
                throw new ArgumentOutOfRangeException("index is out of range");
            }

            return Children[index];
        }

        /// <summary>
        ///  Derived classes override this property to enable the Visual code to enumerate 
        ///  the Visual children. Derived classes need to return the number of children
        ///  from this method.
        ///
        ///    By default a Visual does not have any children.
        ///
        ///  Remark: During this virtual method the Visual tree must not be modified.
        /// </summary>        
        protected override int VisualChildrenCount
        {
            get
            {
                if (Children == null)
                {
                    throw new ArgumentOutOfRangeException("_childrenCollection is null");
                }
                return Children.Count;
            }
        }

        #region Data
        private UIElementCollection2 _children;
        Panel _self;

        #endregion Data

        #region ILogicalTreeParent Members

        void ILogicalTreeParent.AddLogicalChild(UIElement element)
        {
            AddLogicalChild(element);
        }

        void ILogicalTreeParent.RemoveLogicalChild(UIElement element)
        {
            RemoveLogicalChild(element);
        }
        #endregion
    }

    /* Add the following to the hosting element class
     

   */


    public class Item : FrameworkElement
    {
        #region Construction

        public Item()
        {
            _stringProp = string.Empty;
            _hiddenString = string.Empty;
            _intProp = 1;
            _intPropWithManager = 1;
            _listProp = new ArrayList();
            _dictProp = new Hashtable();
            _self = this;

            // ReadOnly properties cannot be set in markup
            SetValue(IntReadOnlyDP1PropertyKey, 1);
            SetValue(IntReadOnlyDP2Property, 2);

            // We have to do a setvalue on these DPs 
            // because otherwise they will never be serialized 
            // since the Parser only uses get value to get the 
            // collection and then set the items into it for 
            // these readonly DPs            
            SetValue(ListDP1PropertyKey, new ArrayList());
            SetValue(ListDP2PropertyKey, new ArrayList());
            SetValue(DictionaryDP1PropertyKey, new Hashtable());
            SetValue(DictionaryDP2PropertyKey, new Hashtable());
        }

        #endregion Construction

        #region Properties

        /// <summary>
        ///     This tests that property value does 
        ///     get serialized normally
        /// </summary>
        public string StringNormalProp
        {
            get { return _stringProp; }
            set { _stringProp = value; }
        }

        /// <summary>
        ///     This tests that property value does 
        ///     not get serialized due to hidden attribute
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string StringHiddenProp
        {
            get { return _hiddenString; }
            set { _hiddenString = value; }
        }

        /// <summary>
        ///     This tests that property with default 
        ///     value does not get serialized
        /// </summary>
        [DefaultValue(1)]
        public int IntDefaultProp
        {
            get { return _intProp; }
            set { _intProp = value; }
        }

        /// <summary>
        ///     This tests that property with 
        ///     ShouldSerialize<PropertyName> 
        ///     return false does not get serialized
        /// </summary>
        public int IntValueProp
        {
            get { return _intProp; }
            set { _intProp = value; }
        }

        public bool ShouldSerializeIntValueProp()
        {
            if (_intProp == 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     This tests that property with 
        ///     ShouldSerialize<PropertyName>(manager) 
        ///     return false does not get serialized
        /// </summary>
        public int IntValuePropWithManager
        {
            get { return _intPropWithManager; }
            set { _intPropWithManager = value; }
        }

        public bool ShouldSerializeIntValuePropWithManager(XamlDesignerSerializationManager manager)
        {
            if (_intPropWithManager == 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     This tests that an IList property value gets 
        ///     serialized even though the property is readonly
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ArrayList ListProp
        {
            get { return _listProp; }
        }

        /// <summary>
        ///     This tests that an IDictionary property value gets 
        ///     serialized even though the property is readonly
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Hashtable DictionaryProp
        {
            get { return _dictProp; }
        }

        #endregion Properties

        #region DepedencyProperties

        /// <summary>
        ///     This tests that a dependency property 
        ///     value does get serialized normally
        /// </summary>
        public static DependencyProperty StringNormalDPProperty =
            DependencyProperty.Register("StringNormalDP", typeof(string), typeof(Item));

        public string StringNormalDP
        {
            get { return (string)GetValue(StringNormalDPProperty); }
            set { SetValue(StringNormalDPProperty, value); }
        }

        /// <summary>
        ///     This tests that a dependency property 
        ///     value does not get serialized when hidden
        /// </summary>
        public static DependencyProperty StringHiddenDP1Property =
            DependencyProperty.Register("StringHiddenDP1", typeof(string), typeof(Item));

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string StringHiddenDP1
        {
            get { return (string)GetValue(StringHiddenDP1Property); }
            set { SetValue(StringHiddenDP1Property, value); }
        }

        /// <summary>
        ///     This tests that a dependency property 
        ///     value does not get serialized when hidden
        /// </summary>
        public static DependencyProperty StringHiddenDP2Property =
            DependencyProperty.Register("StringHiddenDP2", typeof(string), typeof(Item));

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string StringHiddenDP2
        {
            get { return (string)GetValue(StringHiddenDP2Property); }
            set { SetValue(StringHiddenDP2Property, value); }
        }

        /// <summary>
        ///     This tests that a dependency property 
        ///     with default value and not locally set 
        ///     does not get serialized
        /// </summary>
        public static DependencyProperty IntDefaultDP1Property =
            DependencyProperty.Register("IntDefaultDP1", typeof(int), typeof(Item), new PropertyMetadata(1));

        public int IntDefaultDP1
        {
            get { return (int)GetValue(IntDefaultDP1Property); }
            set { SetValue(IntDefaultDP1Property, value); }
        }

        /// <summary>
        ///     This tests that a dependency property locally set but 
        ///     with default value does note get serialized
        /// </summary>
        public static DependencyProperty IntDefaultDP2Property =
            DependencyProperty.Register("IntDefaultDP2", typeof(int), typeof(Item), new PropertyMetadata(1));

        [DefaultValue(2)]
        public int IntDefaultDP2
        {
            get { return (int)GetValue(IntDefaultDP2Property); }
            set { SetValue(IntDefaultDP2Property, value); }
        }

        /// <summary>
        ///     This tests that dependency property with 
        ///     ShouldSerialize<PropertyName> 
        ///     return false does not get serialized
        /// </summary>
        public static DependencyProperty IntValueDP1Property =
            DependencyProperty.Register("IntValueDP1", typeof(int), typeof(Item));

        public int IntValueDP1
        {
            get { return (int)GetValue(IntValueDP1Property); }
            set { SetValue(IntValueDP1Property, value); }
        }

        /*
        public static bool ShouldSerializeIntValueDP1(DependencyObject d)
        {
            if ((int)d.GetValue(IntValueDP1Property) == 1)
            {
                return false;
            }

            return true;
        }
        */

        protected override bool ShouldSerializeProperty( DependencyProperty dp )
        {
            if( dp != IntValueDP1Property )
                return base.ShouldSerializeProperty( dp );

            if( IntValueDP1 == 1 )
                return false;

            return true;
        }

        /// <summary>
        ///     This tests that dependency property with 
        ///     ShouldSerialize<PropertyName> 
        ///     return false does not get serialized
        /// </summary>
        public static DependencyProperty IntValueDP2Property =
            DependencyProperty.Register("IntValueDP2", typeof(int), typeof(Item));

        public int IntValueDP2
        {
            get { return (int)GetValue(IntValueDP2Property); }
            set { SetValue(IntValueDP2Property, value); }
        }

        public bool ShouldSerializeIntValueDP2()
        {
            return false;
        }

        /// <summary>
        ///     This tests that dependency property with 
        ///     ShouldSerialize<PropertyName>(manager) 
        ///     return false does not get serialized
        /// </summary>
        public static DependencyProperty IntValueDP3Property =
            DependencyProperty.Register("IntValueDP3", typeof(int), typeof(Item));

        public int IntValueDP3
        {
            get { return (int)GetValue(IntValueDP3Property); }
            set { SetValue(IntValueDP3Property, value); }
        }

        public static bool ShouldSerializeIntValueDP3(DependencyObject d, XamlDesignerSerializationManager manager)
        {
            if ((int)d.GetValue(IntValueDP3Property) == 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     This tests that a readonly dependency property 
        ///     does not get serialized
        /// </summary>
        private static DependencyPropertyKey IntReadOnlyDP1PropertyKey =
            DependencyProperty.RegisterReadOnly("IntReadOnlyDP1", typeof(int), typeof(Item), new PropertyMetadata(1));
        public static DependencyProperty IntReadOnlyDP1Property =
            IntReadOnlyDP1PropertyKey.DependencyProperty;

        public int IntReadOnlyDP1
        {
            get { return (int)GetValue(IntReadOnlyDP1Property); }
            set { SetValue(IntReadOnlyDP1PropertyKey, value); } // Doesn't this defeat the purpose of read-only DPs?
        }

        /// <summary>
        ///     This tests that a dependency property with a 
        ///     gettor only clr wrapper does not get serialized
        /// </summary>
        public static DependencyProperty IntReadOnlyDP2Property =
            DependencyProperty.Register("IntReadOnlyDP2", typeof(int), typeof(Item));

        public int IntReadOnlyDP2
        {
            get { return (int)GetValue(IntReadOnlyDP2Property); }
        }

        /// <summary>
        ///     This tests that an IList dependency property 
        ///     value gets serialized even though the property 
        ///     is readonly when marked as Content
        /// </summary>
        private static DependencyPropertyKey ListDP1PropertyKey =
            DependencyProperty.RegisterReadOnly("ListDP1", typeof(ArrayList), typeof(Item), new PropertyMetadata(new ArrayList()));
        public static DependencyProperty ListDP1Property =
            ListDP1PropertyKey.DependencyProperty;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ArrayList ListDP1
        {
            get { return (ArrayList)GetValue(ListDP1Property); }
        }

        /// <summary>
        ///     This tests that an IList dependency property 
        ///     value gets serialized even though the property 
        ///     is readonly when marked as Content
        /// </summary>
        private static DependencyPropertyKey ListDP2PropertyKey =
            DependencyProperty.RegisterReadOnly("ListDP2", typeof(ArrayList), typeof(Item), new PropertyMetadata(new ArrayList()));
        public static DependencyProperty ListDP2Property =
            ListDP2PropertyKey.DependencyProperty;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IList ListDP2
        {
            get { return (ArrayList)GetValue(ListDP2Property); }
        }

        /// <summary>
        ///     This tests that an IDictionary dependency property 
        ///     value gets serialized even though the property 
        ///     is readonly when marked as Content
        /// </summary>
        private static DependencyPropertyKey DictionaryDP1PropertyKey =
            DependencyProperty.RegisterReadOnly("DictionaryDP1", typeof(Hashtable), typeof(Item), new PropertyMetadata(new Hashtable()));
        public static DependencyProperty DictionaryDP1Property =
            DictionaryDP1PropertyKey.DependencyProperty;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Hashtable DictionaryDP1
        {
            get { return (Hashtable)GetValue(DictionaryDP1Property); }
        }

        /// <summary>
        ///     This tests that an IDictionary dependency property 
        ///     value gets serialized even though the property 
        ///     is readonly when marked as Content
        /// </summary>
        private static DependencyPropertyKey DictionaryDP2PropertyKey =
            DependencyProperty.RegisterReadOnly("DictionaryDP2", typeof(Hashtable), typeof(Item), new PropertyMetadata(new Hashtable()));
        public static DependencyProperty DictionaryDP2Property =
            DictionaryDP2PropertyKey.DependencyProperty;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IDictionary DictionaryDP2
        {
            get { return (Hashtable)GetValue(DictionaryDP2Property); }
        }

        #endregion DependencyProperties

        #region ResourceReferences

        /// <summary>
        ///     This tests that a dependency property 
        ///     value that is a resource reference 
        ///     gets serialized correctly as "{resourceName}"
        /// </summary>
        public static DependencyProperty ResourceRefDP1Property =
            DependencyProperty.Register("ResourceRefDP1", typeof(Label), typeof(Item));

        public Label ResourceRefDP1
        {
            get { return (Label)GetValue(ResourceRefDP1Property); }
            set { SetValue(ResourceRefDP1Property, value); }
        }

        /// <summary>
        ///     This tests that a dependency property 
        ///     value that is a resource reference 
        ///     gets serialized correctly as "{*typeof(resourceName)}"
        /// </summary>
        public static DependencyProperty ResourceRefDP2Property =
            DependencyProperty.Register("ResourceRefDP2", typeof(Label), typeof(Item));

        public Label ResourceRefDP2
        {
            get { return (Label)GetValue(ResourceRefDP2Property); }
            set { SetValue(ResourceRefDP2Property, value); }
        }

        /// <summary>
        ///     This tests that a dependency property 
        ///     value that is a resource reference 
        ///     gets serialized correctly as "{*Enum.Field}"
        /// </summary>
        public static DependencyProperty ResourceRefDP3Property =
            DependencyProperty.Register("ResourceRefDP3", typeof(Label), typeof(Item));

        public Label ResourceRefDP3
        {
            get { return (Label)GetValue(ResourceRefDP3Property); }
            set { SetValue(ResourceRefDP3Property, value); }
        }

        #endregion ResourceReferences

        #region Data

        string _stringProp;
        string _hiddenString;
        int _intProp;
        int _intPropWithManager;
        ArrayList _listProp;
        Hashtable _dictProp;
        Item _self;

        #endregion Data

    }

    public class Label : FrameworkElement
    {
        #region Construction

        public Label()
        {
        }

        #endregion Construction

        #region Properties

        [TypeConverter(typeof(MyTypeConverter))]
        public MyType MyType
        {
            get { return _myType; }
            set { _myType = value; }
        }

        public static DependencyProperty IntNoWrapperAttachedDPProperty =
            DependencyProperty.RegisterAttached("IntNoWrapperAttachedDP", typeof(int), typeof(Label));

        public static int GetIntNoWrapperAttachedDP(DependencyObject d)
        {
            return (int)d.GetValue(IntNoWrapperAttachedDPProperty);
        }

        public static void SetIntNoWrapperAttachedDP(DependencyObject d, int value)
        {
            d.SetValue(IntNoWrapperAttachedDPProperty, value);
        }

        public static DependencyProperty IntHiddenAttachedDPProperty =
            DependencyProperty.RegisterAttached("IntHiddenAttachedDP", typeof(int), typeof(Label));

        public static int GetIntHiddenAttachedDP(DependencyObject d)
        {
            return (int)d.GetValue(IntHiddenAttachedDPProperty);
        }

        public static void SetIntHiddenAttachedDP(DependencyObject d, int value)
        {
            d.SetValue(IntHiddenAttachedDPProperty, value);
        }

        public static bool ShouldSerializeIntHiddenAttachedDP(DependencyObject d)
        {
            if ((int)d.GetValue(IntHiddenAttachedDPProperty) == 1)
            {
                return false;
            }

            return true;
        }

        #endregion Properties

        #region Commands

        public static RoutedCommand MyCommand
        {
            get { return new RoutedCommand("My", typeof(Label)); }
        }

        #endregion Commands

        #region Data

        private MyType _myType;

        #endregion Data
    }

    public class CustomControl
    {
        #region Construction

        public CustomControl()
        {
            _customProp1 = typeof(XmlTextWriter);
            _customProp2 = new HybridDictionary();
            _customProp2.Add("three", new Label());
            _customProp3 = "mywrongstring";
        }

        #endregion Construction

        #region Properties

        /// <summary>
        ///     This is to test that properties that would 
        ///     normally not be serialized are correctly 
        ///     serialized by the custom serializer for this type
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Type CustomProp1
        {
            get { return _customProp1; }
            set { _customProp1 = value; }
        }

        /// <summary>
        ///     This is to test that properties that would 
        ///     normally not be serialized are correctly 
        ///     serialized by the custom serializer for this type
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public HybridDictionary CustomProp2
        {
            get { return _customProp2; }
        }


        public string CustomProp3
        {
            get { return _customProp3; }
        }

        #endregion Properties

        #region Data


        private Type _customProp1;
        private HybridDictionary _customProp2;

        private string _customProp3;

        #endregion Data

    }

    public class MyButton : System.Windows.Controls.Button
    {
        public static readonly DependencyProperty MyContentProperty
            = DependencyProperty.RegisterAttached("MyContent", typeof(object), typeof(MyButton),
                                          new FrameworkPropertyMetadata((object)null));

        public object MyContent
        {
            get
            {
                return GetValue(MyContentProperty);
            }
            set
            {
                SetValue(MyContentProperty, value);
            }
        }
    }

    public class MyType
    {
    }

    public class GreekStandard
    {
        public static readonly DependencyProperty BetaProperty
            = DependencyProperty.RegisterAttached("Beta", typeof(int), typeof(GreekStandard),
                                          new FrameworkPropertyMetadata(0));

        public static readonly DependencyProperty DeltaProperty
            = DependencyProperty.RegisterAttached("Delta", typeof(double), typeof(GreekStandard),
                                          new FrameworkPropertyMetadata(0.0));
    }

    [ContentProperty("Children")]
    public class MyFrameworkElement : FrameworkElement, IAddChild
    {
        public MyFrameworkElement()
            : base()
        {
        }

        public int Beta
        {
            get { return (int)GetValue(GreekStandard.BetaProperty); }
            set { SetValue(GreekStandard.BetaProperty, value); }
        }

        public double Delta
        {
            get { return (double)GetValue(GreekStandard.DeltaProperty); }
            set { SetValue(GreekStandard.DeltaProperty, value); }
        }

        static MyFrameworkElement()
        {
        }

        public void AppendModelChild(UIElement modelChild)
        {
            Children.Add(modelChild);
        }

        public void RemoveModelChild(UIElement modelChild)
        {
            Children.Remove(modelChild);
        }

        void IAddChild.AddChild(object o)
        {
            AppendModelChild(o as UIElement);
        }

        void IAddChild.AddText(string s)
        {
        }

        protected override IEnumerator LogicalChildren
        {
            get { return Children.GetEnumerator(); }
        }

        public void AppendChild(Visual child)
        {
            Children.Add(child as UIElement);
        }

        /// <summary>
        ///   Derived class must implement to support Visual children. The method must return
        ///    the child at the specified index. Index must be between 0 and GetVisualChildrenCount-1.
        ///
        ///    By default a Visual does not have any children.
        ///
        ///  Remark: 
        ///       During this virtual call it is not valid to modify the Visual tree. 
        /// </summary>
        protected override Visual GetVisualChild(int index)
        {
            if (Children == null)
            {
                throw new ArgumentOutOfRangeException("index is out of range");
            }
            if (index < 0 || index >= Children.Count)
            {
                throw new ArgumentOutOfRangeException("index is out of range");
            }

            return Children[index];
        }

        /// <summary>
        ///  Derived classes override this property to enable the Visual code to enumerate 
        ///  the Visual children. Derived classes need to return the number of children
        ///  from this method.
        ///
        ///    By default a Visual does not have any children.
        ///
        ///  Remark: During this virtual method the Visual tree must not be modified.
        /// </summary>        
        protected override int VisualChildrenCount
        {
            get
            {
                if (Children == null)
                {
                    throw new ArgumentOutOfRangeException("_children is null");
                }
                return Children.Count;
            }
        }


        private UIElementCollection2 _children;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public UIElementCollection2 Children
        {
            get
            {
                if (_children == null)
                    _children = new UIElementCollection2(this);
                return _children;
            }
        }
    }

    public interface ILogicalTreeParent
    {
        void AddLogicalChild(UIElement element);
        void RemoveLogicalChild(UIElement element);
    }

    public class UIElementCollection2 : Collection<UIElement>
    {
        private VisualCollection _visuals;
        private ILogicalTreeParent _logicalTreeParent;

        internal UIElementCollection2(FrameworkElement parent)
        {
            _visuals = new VisualCollection(parent);
            _logicalTreeParent = parent as ILogicalTreeParent; //If the class is a logical tree parent, we'll maintain LogicalChildren with the help of the parent
        }

        protected override void InsertItem(int index, UIElement item)
        {
            _visuals.Insert(index, item);
            if (_logicalTreeParent != null)
                _logicalTreeParent.AddLogicalChild(item);
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, UIElement item)
        {
            _visuals[index] = item;
            if (_logicalTreeParent != null)
            {
                _logicalTreeParent.RemoveLogicalChild(this[index]);
                _logicalTreeParent.AddLogicalChild(item);
            }
            base.SetItem(index, item);
        }
        protected override void RemoveItem(int index)
        {
            _visuals.RemoveAt(index);
            if (_logicalTreeParent != null)
            {
                _logicalTreeParent.RemoveLogicalChild(this[index]);
            }
            base.RemoveItem(index);
        }
        protected override void ClearItems()
        {
            _visuals.Clear();
            base.ClearItems();
        }
    }

}
