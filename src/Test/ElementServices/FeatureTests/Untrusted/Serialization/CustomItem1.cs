// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***************************************************************************\
*
*
*  CustomItem1 control used for Serializer tests;
*
*
\***************************************************************************/
using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.ComponentModel;

using System.Windows;

using System.Windows.Markup;

namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomItem1 : FrameworkElement
    {
        #region Construction

        static CustomItem1()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public CustomItem1()
        {
            _stringProp = string.Empty;
            _hiddenString = string.Empty;
            _intProp = 1;
            _skipProp = 1;
            _noCallbackProp = 1;
            _listProp = new ArrayList();
            _dictProp = new Hashtable();
            _arraySkipProp = new ArrayList();
            _arrayNoCallbackProp = new ArrayList();
            _self = this;

            // ReadOnly properties cannot be set in markup
            SetValue(s_intReadOnlyDP1PropertyKey, 1);
            SetValue(IntReadOnlyDP2Property, 2);

            // We have to do a setvalue on these DPs 
            // because otherwise they will never be serialized 
            // since the Parser only uses get value to get the 
            // collection and then set the items into it for 
            // these readonly DPs            
            SetValue(s_listDP1PropertyKey, new ArrayList());
            SetValue(s_listDP2PropertyKey, new ArrayList());
            SetValue(s_dictionaryDP1PropertyKey, new Hashtable());
            SetValue(s_dictionaryDP2PropertyKey, new Hashtable());

            // Add Event Handlers
            NormalEvent += new EventHandler(OnNormalEvent);
            NormalRE += new EventHandler(OnNormalEvent);
            SkipEvent += new EventHandler(OnSkipEvent);
            NoCallbackEvent += new EventHandler(OnNoCallbackEvent);
            AttachedRE += new EventHandler(OnAttachedEvent);
            AddHandler(CustomItem2.NoWrapperAttachedREEvent, new EventHandler(OnNoWrapperAttachedEvent));
        }
        
        #endregion Construction

        #region Events

        /// <summary>
        ///     This tests if a normal Clr 
        ///     Event gets serialized correctly
        /// </summary>
        public event EventHandler NormalEvent;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        public void OnNormalEvent(object source, EventArgs args) {}

        /// <summary>
        ///     This tests if a normal Routed 
        ///     Event gets serialized correctly
        /// </summary>
        public static readonly RoutedEvent NormalREEvent = 
            EventManager.RegisterRoutedEvent("NormalRE", RoutingStrategy.Bubble, typeof(EventHandler), typeof(CustomItem1));
        
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public event EventHandler NormalRE
        {
            add { AddHandler(NormalREEvent, value); }
            remove { RemoveHandler(NormalREEvent, value); }
        }

        /// <summary>
        ///     This tests if an attached Routed 
        ///     Event with a Clr wrapper gets serialized correctly
        /// </summary>
        public event EventHandler AttachedRE
        {
            add { AddHandler(CustomItem2.AttachedREEvent, value); }
            remove { RemoveHandler(CustomItem2.AttachedREEvent, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        public void OnAttachedEvent(object source, EventArgs args) {}

        /// <summary>
        ///     This tests if an attached Routed 
        ///     Event without a Clr wrapper 
        ///     gets serialized correctly
        /// </summary>
        public void OnNoWrapperAttachedEvent(object source, EventArgs args) {}
        
        #endregion Events

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
        /// 
        /// </summary>
        /// <value></value>
        public int IntValueProp
        {
            get { return _intProp; }
            set { _intProp = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeIntValueProp()
        {
            if (_intProp == 1)
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
        #region Alias property 
        /// <summary>
        /// DependencyProperty for <see cref="AliasDP" /> property.
        /// </summary>
        public static readonly DependencyProperty AliasDPProperty = CustomItem2.AliasDPProperty.AddOwner(typeof(CustomItem1));

        /// <summary>
        /// The AliasDP property specifies the name of font family.
        /// </summary>
        public string AliasDP
        {
            get { return (string)GetValue(CustomItem2.AliasDPProperty); }
            set { SetValue(CustomItem2.AliasDPProperty, value); }
        }

        #endregion Alias property 

        #region DepedencyProperties

        /// <summary>
        ///     This tests that a dependency property 
        ///     value does get serialized normally
        /// </summary>
        public static DependencyProperty StringNormalDPProperty = 
            DependencyProperty.Register("StringNormalDP", typeof(string), typeof(CustomItem1));

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
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
            DependencyProperty.Register("StringHiddenDP1", typeof(string), typeof(CustomItem1));

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
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
            DependencyProperty.Register("StringHiddenDP2", typeof(string), typeof(CustomItem1));

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
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
            DependencyProperty.Register("IntDefaultDP1", typeof(int), typeof(CustomItem1), new PropertyMetadata(1));

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
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
            DependencyProperty.Register("IntDefaultDP2", typeof(int), typeof(CustomItem1), new PropertyMetadata(1));

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [DefaultValue(2)]
        public int IntDefaultDP2
        {
            get { return (int)GetValue(IntDefaultDP2Property); }
            set { SetValue(IntDefaultDP2Property, value); }
        }


        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty IntValueDP1Property = 
            DependencyProperty.Register("IntValueDP1", typeof(int), typeof(CustomItem1));

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int IntValueDP1
        {
            get { return (int)GetValue(IntValueDP1Property); }
            set { SetValue(IntValueDP1Property, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool ShouldSerializeIntValueDP1(DependencyObject d)
        {
            if ((int)d.GetValue(IntValueDP1Property) == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty IntValueDP2Property = 
            DependencyProperty.Register("IntValueDP2", typeof(int), typeof(CustomItem1));

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int IntValueDP2
        {
            get { return (int)GetValue(IntValueDP2Property); }
            set { SetValue(IntValueDP2Property, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeIntValueDP2()
        {
            return false;
        }

        private static DependencyPropertyKey s_intReadOnlyDP1PropertyKey =
            DependencyProperty.RegisterReadOnly("IntReadOnlyDP1", 
                                         typeof(int), 
                                         typeof(CustomItem1), 
                                         new PropertyMetadata(1));

        /// <summary>
        ///     This tests that a readonly dependency property 
        ///     does not get serialized
        /// </summary>
        public static DependencyProperty IntReadOnlyDP1Property =
            s_intReadOnlyDP1PropertyKey.DependencyProperty;
        
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int IntReadOnlyDP1
        {
            get { return (int)GetValue(IntReadOnlyDP1Property); }
            set { SetValue(s_intReadOnlyDP1PropertyKey, value); }
        }

        /// <summary>
        ///     This tests that a dependency property with a 
        ///     gettor only clr wrapper does not get serialized
        /// </summary>
        public static DependencyProperty IntReadOnlyDP2Property =
            DependencyProperty.Register(
            "IntReadOnlyDP2",
            typeof(int),
            typeof(CustomItem1));

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int IntReadOnlyDP2
        {
            get { return (int)GetValue(IntReadOnlyDP2Property); }
        }
        private static DependencyPropertyKey s_listDP1PropertyKey =
            DependencyProperty.RegisterReadOnly("ListDP1", 
            typeof(ArrayList), 
            typeof(CustomItem1), 
            new PropertyMetadata(new ArrayList()));

        /// <summary>
        ///     This tests that an IList dependency property 
        ///     value gets serialized even though the property 
        ///     is readonly when marked as Content
        /// </summary>
        public static DependencyProperty ListDP1Property =
            s_listDP1PropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ArrayList ListDP1
        {
            get { return (ArrayList)GetValue(ListDP1Property); }
        }

        private static DependencyPropertyKey s_listDP2PropertyKey =
             DependencyProperty.RegisterReadOnly("ListDP2", 
                 typeof(ArrayList), 
                 typeof(CustomItem1), 
                 new PropertyMetadata(new ArrayList()));

        /// <summary>
        ///     This tests that an IList dependency property 
        ///     value gets serialized even though the property 
        ///     is readonly when marked as Content
        /// </summary>
        public static DependencyProperty ListDP2Property =
            s_listDP2PropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IList ListDP2
        {
            get { return (ArrayList)GetValue(ListDP2Property); }
        }

        private static DependencyPropertyKey s_dictionaryDP1PropertyKey =
            DependencyProperty.RegisterReadOnly("DictionaryDP1", 
            typeof(Hashtable), 
            typeof(CustomItem1), 
            new PropertyMetadata(new Hashtable()));

        /// <summary>
        ///     This tests that an IDictionary dependency property 
        ///     value gets serialized even though the property 
        ///     is readonly when marked as Content
        /// </summary>
        public static DependencyProperty DictionaryDP1Property =
            s_dictionaryDP1PropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Hashtable DictionaryDP1
        {
            get { return (Hashtable)GetValue(DictionaryDP1Property); }
        }

        private static DependencyPropertyKey s_dictionaryDP2PropertyKey =
            DependencyProperty.RegisterReadOnly("DictionaryDP2", 
            typeof(Hashtable), 
            typeof(CustomItem1), 
            new PropertyMetadata(new Hashtable()));


        /// <summary>
        ///     This tests that an IDictionary dependency property 
        ///     value gets serialized even though the property 
        ///     is readonly when marked as Content
        /// </summary>
        public static DependencyProperty DictionaryDP2Property =
            s_dictionaryDP2PropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IDictionary DictionaryDP2
        {
            get { return (Hashtable)GetValue(DictionaryDP2Property); }
        }

        #endregion DependencyProperties

        #region DesignerHooks

        /// <summary>
        ///     This tests if a serializable Clr 
        ///     Event is skipped by designer action
        /// </summary>
        public event EventHandler SkipEvent;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        public void OnSkipEvent(object source, EventArgs args) {}

        /// <summary>
        ///     This tests if a serializable Clr 
        ///     Event callbacks are avoided by designer action
        /// </summary>
        public event EventHandler NoCallbackEvent;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        public void OnNoCallbackEvent(object source, EventArgs args) {}

        /// <summary>
        ///     This tests if a serializable property attribute 
        ///     is being skipped by designer action
        /// </summary>
        public int IntSkipProp
        {
            get { return _skipProp; }
            set { _skipProp = value; }
        }

        /// <summary>
        ///     This tests if a serializable complex property 
        ///     and its contents are being skipped by 
        ///     designer action
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ArrayList ArraySkipProp
        {
            get { return _arraySkipProp; }
        }

        /// <summary>
        ///     This tests if a serializable property attribute 
        ///     callbacks are avoided by designer action
        /// </summary>
        public int IntNoCallbackProp
        {
            get { return _noCallbackProp; }
            set { _noCallbackProp = value; }
        }

        /// <summary>
        ///     This tests if a serializable complex property 
        ///     and its contents callbacks are being avoided by 
        ///     designer action
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ArrayList ArrayNoCallbackProp
        {
            get { return _arrayNoCallbackProp; }
        }
        
        #endregion DesignerHooks

        #region ResourceReferences

        /// <summary>
        ///     This tests that a dependency property 
        ///     value that is a resource reference 
        ///     gets serialized correctly as "{resourceName}"
        /// </summary>
        public static DependencyProperty ResourceRefDP1Property = 
            DependencyProperty.Register("ResourceRefDP1", typeof(CustomItem2), typeof(CustomItem1));

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public CustomItem2 ResourceRefDP1
        {
            get { return (CustomItem2)GetValue(ResourceRefDP1Property); }
            set { SetValue(ResourceRefDP1Property, value); }
        }

        /// <summary>
        ///     This tests that a dependency property 
        ///     value that is a resource reference 
        ///     gets serialized correctly as "{*typeof(resourceName)}"
        /// </summary>
        public static DependencyProperty ResourceRefDP2Property = 
            DependencyProperty.Register("ResourceRefDP2", typeof(CustomItem2), typeof(CustomItem1));

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public CustomItem2 ResourceRefDP2
        {
            get { return (CustomItem2)GetValue(ResourceRefDP2Property); }
            set { SetValue(ResourceRefDP2Property, value); }
        }

        #endregion ResourceReferences

        #region CustomExpression

        /// <summary>
        ///     This tests that a dependency property 
        ///     value that is set to a custom expression
        ///     gets serialized correctly as "[identity]"
        /// </summary>
        public static DependencyProperty CustomExprDPProperty = 
            DependencyProperty.Register("CustomExprDP", typeof(CustomItem2), typeof(CustomItem1));

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public CustomItem2 CustomExprDP
        {
            get { return (CustomItem2)GetValue(CustomExprDPProperty); }
            set { SetValue(CustomExprDPProperty, value); }
        }
        
        #endregion CustomExpression
        
        #region Data

        string _stringProp;
        string _hiddenString;
        int _intProp;
        int _skipProp;
        int _noCallbackProp;
        ArrayList _listProp;
        Hashtable _dictProp;
        ArrayList _arraySkipProp;
        ArrayList _arrayNoCallbackProp;
        CustomItem1 _self;
        
        #endregion Data
        
    }
}

