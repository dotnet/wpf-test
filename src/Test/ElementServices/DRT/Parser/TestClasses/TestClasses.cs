// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


/***************************************************************************\
*
*
*  Parser test classes used by DrtParser.exe
*
*
\***************************************************************************/

using System;
using System.Threading;

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Globalization;
using System.Windows.Markup;
using System.Windows.Media;

using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

[assembly:System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/ParserTest/2005", "DRTText")]

namespace DRTText
{
    /// <summary>
    ///  Test flags enum for parser testing
    /// </summary>
    [Flags]
    public enum TextTestFlags
    {
        /// <summary> </summary>
        one        = 1,
        /// <summary> </summary>
        two        = 2,
        /// <summary> </summary>
        four       = 4,
    }
        
    /// <summary>
    ///     Test control used for TextContainer drts.  The purpose of this class
    ///     is to expose some of the TextContainer apis for testing, and to provide some
    ///     properties used in parser testing.
    /// </summary>
    public class TextTestControl : ListBox
    {        

        static public int FieldOne = 111;
        static public double FieldTwo = 2.22;
        static public int PropertyThree
        {
            get { return 333; }
        }

        /// <summary>
        ///     Test DependencyProperty with default uri
        /// </summary>
        public static readonly DependencyProperty AlphaProperty = DependencyProperty.RegisterAttached(
            "Alpha", typeof(int), typeof(TextTestControl));
        /// <summary>
        ///     Test DependencyProperty with new uri
        /// </summary>
        public static readonly DependencyProperty GreekAlphaProperty = DependencyProperty.RegisterAttached(
            "GreekAlpha", typeof(int), typeof(TextTestControl));
        /// <summary>
        ///     Test DependencyProperty with new uri
        /// </summary>
        public static readonly DependencyProperty BackgroundDPProperty = DependencyProperty.RegisterAttached(
            "BackgroundDP", typeof(SolidColorBrush), typeof(TextTestControl));
        /// <summary>
        ///     Test DependencyProperty with new uri
        /// </summary>
        public static readonly DependencyProperty ForegroundDPProperty = DependencyProperty.RegisterAttached(
            "ForegroundDP", typeof(SolidColorBrush), typeof(TextTestControl));
        /// <summary>
        ///     Test DependencyProperty with enums
        /// </summary>
        public static readonly DependencyProperty NumberEnumProperty = DependencyProperty.RegisterAttached(
            "NumberEnum", typeof(TextTestFlags), typeof(TextTestControl));
        /// <summary>
        ///     Test DependencyProperty with IList
        /// </summary>
        private static PropertyMetadata BetaPMD = new PropertyMetadata((object)null);
        private static readonly DependencyPropertyKey BetaIListPropertyKey = DependencyProperty.RegisterReadOnly(
            "BetaIList", typeof(MyIList), typeof(TextTestControl), BetaPMD);
        public static readonly DependencyProperty BetaIListProperty = 
            BetaIListPropertyKey.DependencyProperty;


        /// <summary>
        ///     Test Type native prop
        /// </summary>
        public Type TestType
        {
            get { return _testType; }
            set { _testType = value; }
        }
        Type _testType = null;

        /// <summary>
        ///     Test DependencyProperty native prop
        /// </summary>
        public int Alpha
        {
            get { return (int)GetValue(AlphaProperty); }
            set { SetValue(AlphaProperty,value); }
        }

        ///<summary>
        ///  Test prop for culture
        ///</summary>
        [TypeConverter(typeof(System.Windows.CultureInfoIetfLanguageTagConverter))]
        public CultureInfo MyCulture
        { 
            get { return _myCulture; }
            set { _myCulture = value; }
        }
        private CultureInfo _myCulture = null;

        /// <summary>
        ///     Test array property
        /// </summary>
        public MyText[] MyTextArrayProp
        {
            get { return _mtap; }
            set {_mtap = value; }
        }

        /// <summary>
        ///     Test DependencyProperty prop
        /// </summary>
        public SolidColorBrush BackgroundDP
        {
            get { return GetValue(BackgroundDPProperty) as SolidColorBrush; }
            set { SetValue(BackgroundDPProperty,value); }
        }

        /// <summary>
        ///  Test prop that is read only and implements IAddChild
        /// </summary>
        public IACC IAddChildCollection
        {
            get { return _iacc; }
        }

        /// <summary>
        ///  Test prop that is read only and implements IEnumerable
        /// </summary>
        public IEnumerable IEnumerableChildren
        {
            get { return _ienum; }
        }

        /// <summary>
        ///  Test prop that is read only and implements IList
        /// </summary>
        public MyIList BetaIList
        {
            get { return (MyIList)GetValue(BetaIListProperty); }
        }

        /// <summary>
        ///  Test prop that is read-write and implements IDictionary
        /// </summary>
        public IDictionary IDict
        {
            get { return _idict; }
            set { _idict = value; }
        }
        
        /// <summary>
        ///  Test prop that is read-write and implements IList
        /// </summary>
        public IList RWList
        {
            get { return _rwlist; }
            set { _rwlist = value; }
        }
        
        /// <summary>
        ///     Test DependencyProperty prop
        /// </summary>
        public SolidColorBrush ForegroundDP
        {
            get { return null; }
            set { }
        }

        /// <summary>
        ///     Test DependencyProperty attached prop 
        /// </summary>
        public static void SetAlpha(DependencyObject target, int value)
        {
        }

        /// <summary>
        ///     Test DependencyProperty attached prop 
        /// </summary>
        public static void SetGreekAlpha(DependencyObject target, int value)
        {
        }
        
        /// <summary>
        ///     Test DependencyProperty attached prop
        /// </summary>
        public static void SetNumberEnum(DependencyObject target, TextTestFlags value)
        {
        }
        
        /// <summary>
        ///     Test DependencyProperty attached prop
        /// </summary>
        public static MyIList GetBetaIListDependency(Object target)
        {
            return ((TextTestControl)target).BetaIList;
        }
        
        /// <summary>
        ///     Test constructor.  Used only for TextContainer drts.
        /// </summary>
        public TextTestControl() 
        {
             SetValue(BetaIListPropertyKey, new MyIList(777));
        }
        
        /// <summary>
        ///     Test constructor.  Used only for TextContainer drts.
        /// </summary>
        public TextTestControl(DependencyObject root)
        {
            _root = root;
            if (root != null)
                ((IAddChild)root).AddChild(this);
            SetValue(BetaIListPropertyKey, new MyIList(777));
        }

        private IACC             _iacc = new IACC();
        private IENUM            _ienum = new IENUM();
        private IDictionary      _idict = new MyHashtable();
        private IList            _rwlist = new ArrayList();
        private MyText[]         _mtap = null;
        private DependencyObject _root;
    }

    // Test FrameworkContentElement class
    [ContentProperty("Kids")]
    public class FCETest : FrameworkContentElement, IAddChild
    {
        /// <summary>
        ///     Test DependencyProperty with default uri
        /// </summary>
        public static readonly DependencyProperty AlphaFCEProperty = DependencyProperty.Register(
            "AlphaFCE", typeof(int), typeof(FCETest));
        
        /// <summary>
        ///     Test DependencyProperty native prop
        /// </summary>
        public int AlphaFCE
        {
            get { return (int)GetValue(AlphaFCEProperty); }
            set { SetValue(AlphaFCEProperty,value); }
        }

        /// <summary>
        ///     Returns children
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ArrayList Kids
        {
            get { return _children; }
        }

        ///<summary>
        /// Called to Add the object as a Child.
        ///</summary>
        ///<param name="o">
        /// Object to add as a child
        ///</param>
        void IAddChild.AddChild(Object o)
        {
            _children.Add(o);
            AddLogicalChild(o);
        }
        ArrayList _children = new ArrayList();

        ///<summary>
        /// Called when text appears under the tag in markup
        ///</summary>
        ///<param name="s">
        /// Text to Add to the Object
        ///</param> 
        void IAddChild.AddText(string s)
        {
            _text = s;
        }
        string _text = null;

        /// <summary>
        ///     Returns enumerator to logical children
        /// </summary>
        protected override IEnumerator LogicalChildren { get { return _children.GetEnumerator(); } }
    }
    
    // Test FrameworkContentElement class
    [ContentProperty("Kids")]
    public class FCETestTwo : FrameworkContentElement, IAddChild
    {
        /// <summary>
        ///     Test DependencyProperty with default uri
        /// </summary>
        public static readonly DependencyProperty AlphaFCE2Property = DependencyProperty.Register(
            "AlphaFCE2", typeof(int), typeof(FCETestTwo));
        
        /// <summary>
        ///     Test DependencyProperty native prop
        /// </summary>
        public int AlphaFCE2
        {
            get { return (int)GetValue(AlphaFCE2Property); }
            set { SetValue(AlphaFCE2Property,value); }
        }

        /// <summary>
        ///     Returns children
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ArrayList Kids
        {
            get { return _children; }
        }

        ///<summary>
        /// Called to Add the object as a Child.
        ///</summary>
        ///<param name="o">
        /// Object to add as a child
        ///</param>
        void IAddChild.AddChild(Object o)
        {
            _children.Add(o);
            AddLogicalChild(o);
        }
        ArrayList _children = new ArrayList();

        ///<summary>
        /// Called when text appears under the tag in markup
        ///</summary>
        ///<param name="s">
        /// Text to Add to the Object
        ///</param> 
        void IAddChild.AddText(string s)
        {
            _text = s;
        }
        string _text = null;

        /// <summary>
        ///     Returns enumerator to logical children
        /// </summary>
        protected override IEnumerator LogicalChildren { get { return _children.GetEnumerator(); } }

    }
    
    // Test FrameworkElement class
    [ContentProperty("Kids")]
    public class FETest : FrameworkElement, IAddChild
    {
        ///<summary>
        /// Called when text appears under the tag in markup
        ///</summary>
        protected override Size MeasureOverride(Size constraint)
        {
            return constraint;
        }
        
        /// Called to Add the object as a Child.
        ///</summary>
        ///<param name="o">
        /// Object to add as a child
        ///</param>
        void IAddChild.AddChild(Object o)
        {
            _children.Add(o);
            AddLogicalChild(o);
       }
        ArrayList _children = new ArrayList();

        ///<summary>
        /// Called when text appears under the tag in markup
        ///</summary>
        ///<param name="s">
        /// Text to Add to the Object
        ///</param> 
        void IAddChild.AddText(string s)
        {
            _text = s;
        }
        string _text = null;

        /// <summary>
        ///     Returns children
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ArrayList Kids
        {
            get { return _children; }
        }

        /// <summary>
        ///     Returns enumerator to logical children
        /// </summary>
        protected override IEnumerator LogicalChildren { get { return _children.GetEnumerator(); } }

     }

    /// <summary>
    ///     Test class that implements IDictionary.
    /// </summary>
    public class MyHashtable : Hashtable
    {
        public MyHashtable() : base ()
        {
        }

        public int MYTP
        {
            get { return _mytp; }
            set { _mytp = value; }
        }

        int _mytp;
    }
    
    /// <summary>
    ///     Test class that implements IList.
    /// </summary>
    public class MyIList : ArrayList
    {
        public MyIList() : base ()
        {
        }
        public MyIList(int i) : this ()
        {
            _myitp = i;
        }

        public int MYITP
        {
            get { return _myitp; }
            set { _myitp = value; }
        }

        int _myitp;
    }
    
    /// <summary>
    ///  Test class that implements IAddChild.
    /// </summary>
    [ContentProperty("Children")]
    public class IACC : IAddChild, IEnumerable
    {
         /// <summary> test </summary>
         public IACC()
         {
         }

         /// <summary> test </summary>
         public int SomeInt
         {
            get { return _int; }
            set { _int = value; }
         }

         /// <summary> test </summary>
         public void AddChild(object o)
         {
            Children.Add(o);
         }

         /// <summary> test </summary>
         public void AddText(string s)
         {
            Children.Add(s);
         }

         public IEnumerator GetEnumerator()
         {
            return Children.GetEnumerator();
         }

         public List<object> Children
         {
             get 
             {
                 if (_children == null)
                     _children = new List<object>();
                 return _children;
             }
         }
         private List<object> _children;
         private int       _int;
         
    }
        
    /// <summary>
    ///  Test class that implements IEnumerable
    /// </summary>
    public class IENUM : IEnumerable
    {
         /// <summary> test </summary>
         public IENUM()
         {
            _children = new ArrayList();
         }

         /// <summary> test </summary>
         public int SomeInt
         {
            get { return _int; }
            set { _int = value; }
         }
         public IEnumerator GetEnumerator()
         {
            return _children.GetEnumerator();
         }

         private ArrayList _children;
         private int       _int;
         
    }
        
    /// <summary>
    ///     Test control used for Parser drts.  The sole purpose of this class
    ///     is to expose some DependencyProperties.
    /// </summary>
    public class TextTestControlSubclass : TextTestControl
    {        
        /// <summary>
        ///     Test constructor.  Used only for TextContainer drts.
        /// </summary>
        public TextTestControlSubclass(TextElement root) : base(root)
        {
        }

        /// <summary>
        ///     Test constructor.  Used only for TextContainer drts.
        /// </summary>
        public TextTestControlSubclass() : base(null)
        {
        }

        /// <summary>
        ///     Test DependencyProperty with default uri
        /// </summary>
        public static readonly DependencyProperty BetaProperty = DependencyProperty.RegisterAttached(
            "Beta", typeof(int), typeof(TextTestControlSubclass));
        /// <summary>
        ///     Test DependencyProperty with new uri
        /// </summary>
        public static readonly DependencyProperty GreekBetaProperty = DependencyProperty.RegisterAttached(
            "GreekBeta", typeof(int), typeof(TextTestControlSubclass));


        /// <summary>
        ///     Test DependencyProperty native prop
        /// </summary>
        public int Beta
        {
            get { return 1; }
            set { }
        }
        

        /// <summary>
        ///     Test DependencyProperty attached prop
        /// </summary>
        public static void SetGreekBeta(DependencyObject target, int value)
        {
        }
    }


    // Generic ContentElement to insert into TextContainer
    public class MyControl : FrameworkElement, IContentHost
    {
        #region HitTest
        
        /// <summary>
        ///     Hit tests to the correct ContentElement 
        ///     within the ContentHost that the mouse 
        ///     is over
        /// </summary>
        /// <param name="point">
        ///     Mouse coordinates relative to 
        ///     the ContentHost
        /// </param>
        IInputElement IContentHost.InputHitTest(Point point)
        {
            return this;
        }

        // Not implemented
        ReadOnlyCollection<Rect> IContentHost.GetRectangles(ContentElement child)
        {
            throw new NotImplementedException("[IContentHost.GetRectangles] not implemented!");
        }

        // Not implemented
        IEnumerator<IInputElement> IContentHost.HostedElements
        {
            get
            {
                throw new NotImplementedException("[IContentHost.HostedElements] not implemented!");
            }
        }

        // Not implemented
        void IContentHost.OnChildDesiredSizeChanged(UIElement child)
        {
            throw new NotImplementedException("[IContentHost.OnChildDesiredSizeChanged] not implemented!");
        }

        #endregion HitTest   

        /// <summary> testing </summary>
        public static void Whatever()
        {
        }
    }

    /// <summary> testing </summary>
    public class MyControlsSubclass : MyControl
    {
        /// <summary> testing </summary>
        public static new void Whatever()
        {
        }
    }

// 

    public class MyText : TextBlock
    {
        public MyText() : this(null)
        {
        }

        public MyText(DependencyObject parent)
        {
            if (parent != null)
                ((IAddChild)parent).AddChild(this);
        }
    }

    // MarkupExtension expansion testing.
    public class LiteralRepeatExtension : MarkupExtension
    {
        public LiteralRepeatExtension() : base()
        {
        }

        public LiteralRepeatExtension(string baseStr, int repeat)
        {
            _baseStr = baseStr;
            _repeat = repeat;
        }

        public override object ProvideValue(
            IServiceProvider serviceProvider )
        {
            if (_baseStr == null || _repeat == 0)
            {
                throw new InvalidOperationException();
            }

            string s = string.Empty;
            for (int i=1; i<=_repeat; i++)
                s += _baseStr;

            return s;
        }
        
        public int RepeatCount
        {
            get { return _repeat; }
            set { _repeat = value; }
        }

        public string BaseStr
        {
            get { return _baseStr; }
            set { _baseStr = value; }
        }

        string _baseStr = null;
        int    _repeat = 0;
    }

    // Xml island parsing
    [ContentProperty("XmlSerializer")]
    public class MyXmlIsland
    {
        public MyXmlIsland()
        {
            _xmlSerializer = new MySerializer(this);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IXmlSerializable XmlSerializer
        {
            get { return _xmlSerializer; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeXmlSerializer()
        {
            return _inlineXmlLoaded;
        }

        public XmlDocument Document
        {
            get { return _doc; }
        }

        private MySerializer _xmlSerializer;
        private XmlDocument _doc = new XmlDocument();
        private bool _inlineXmlLoaded;

        private class MySerializer : IXmlSerializable
        {
            internal MySerializer(MyXmlIsland host)
            {
                _host = host;
            }

            public XmlSchema GetSchema()
            {
                return null;
            }

            public void WriteXml(XmlWriter writer)
            {
                _host.Document.Save(writer);
            }

            public void ReadXml(XmlReader reader)
            {
                _host.Document.Load(reader);
                _host._inlineXmlLoaded = true;
            }

            private MyXmlIsland _host;
        }
    }

    public class XmlIslandDocument  : XmlDocument, IXmlSerializable
    {
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void WriteXml(XmlWriter writer)
        {
            this.Save(writer);
        }

        public void ReadXml(XmlReader reader)
        {
            this.Load(reader);
        }
    }
   
}

