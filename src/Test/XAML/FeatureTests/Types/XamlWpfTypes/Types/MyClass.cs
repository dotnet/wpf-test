// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Microsoft.Test.Xaml.Types
{
    #region Struct MyStruct

    /// <summary>
    /// A custom struct to be used for multiple tests
    /// </summary>
    public struct MyStruct
    {
        /// <summary>
        /// My Motto data string
        /// </summary>
        public static string MyMottoField = "My motto";

        /// <summary>
        /// Gets or sets my font.
        /// </summary>
        /// <value>My font value.</value>
        public FontStyle MyFont { get; set; }
    }

    #endregion Struct MyStruct
    #region Class MyClass

    /// <summary>
    /// This class defines custom attached properties and a custom attached event that 
    /// can be used with other dependency objects.
    /// </summary>
    public class MyClass
    {
        /// <summary>
        /// Custom event.
        /// </summary>
        public static readonly RoutedEvent MyRoutedEvent = EventManager.RegisterRoutedEvent("MyRouted", RoutingStrategy.Bubble, typeof(MyRoutedEventHandler), typeof(MyClass));

        /// <summary>
        /// DependencyProperty for the attached MyTransparency property.
        /// </summary>
        public static readonly DependencyProperty MyTransparencyProperty = DependencyProperty.RegisterAttached("MyTransparency", typeof(string), typeof(MyClass), new FrameworkPropertyMetadata("0.0"));

        /// <summary>
        /// DependencyProperty for the attached MyColor property.
        /// </summary>
        public static readonly DependencyProperty MyColorProperty = DependencyProperty.RegisterAttached("MyColor", typeof(MyColor), typeof(MyClass));

        /// <summary>
        /// DependencyProperty for the attached MyChildren property.
        /// </summary>
        public static readonly DependencyProperty MyChildrenProperty = DependencyProperty.RegisterAttached("MyChildren", typeof(MyChildren), typeof(MyClass));

        /// <summary>
        /// DependencyProperty for the attached MyArrayList property.
        /// </summary>
        public static readonly DependencyProperty MyArrayListProperty = DependencyProperty.RegisterAttached("MyArrayList", typeof(ArrayList), typeof(MyClass));

        /// <summary>
        /// DependencyProperty for the attached MyBrushes property.
        /// </summary>
        public static readonly DependencyProperty MyBrushesProperty = DependencyProperty.RegisterAttached("MyBrushes", typeof(SolidColorBrush[]), typeof(MyClass));

        /// <summary>
        /// DependencyProperty for the attached MyIDict property.
        /// </summary>
        public static readonly DependencyProperty MyIDictProperty = DependencyProperty.RegisterAttached("MyIDict", typeof(Hashtable), typeof(MyClass));

        /// <summary>
        /// DependencyProperty for the attached MyColorSet property.
        /// </summary>
        public static readonly DependencyProperty MyColorSetProperty = DependencyProperty.RegisterAttached("MyColorSet", typeof(MyColorSet), typeof(MyClass));

        /// <summary>
        /// My Greeting
        /// </summary>
        private static string s_myGreetingField = "Hello world";

        /// <summary>
        /// My StringField
        /// </summary>
        private string _myStringField = "Foo bar baz";

        /// <summary>
        /// This flag denotes that a handler received MyClrEvent. 
        /// We clear it in the FireMyClrEvent() function above, before firing the event. 
        /// We set it in a handler function, so that we can verify that the event was properly fired.
        /// </summary>
        private bool _myClrEventReached = false;

        /// <summary>
        /// This delegate must used by handlers of the MyRoutedEvent event.
        /// </summary>
        /// <param name="sender">The current element along the event's route.</param>
        /// <param name="e">The event arguments containing additional information about the event.</param>
        public delegate void MyRoutedEventHandler(object sender, RoutedEventArgs e);

        /// <summary>
        /// Delegate for the event.
        /// </summary>
        /// <param name="source">Source of the event</param>
        /// <param name="e">Event args</param>
        public delegate void MyClrEventHandler(object source, EventArgs e);

        /// <summary>
        /// Custom CLR Event 
        /// </summary>
        public event MyClrEventHandler MyClrEvent;

        #region Properties

        /// <summary>
        /// Gets or sets my greeting field.
        /// </summary>
        /// <value>My greeting field.</value>
        public static string MyGreetingField
        {
            get
            {
                return s_myGreetingField;
            }

            set
            {
                s_myGreetingField = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [my CLR handlers added].
        /// </summary>
        /// <value><c>true</c> if [my CLR handlers added]; otherwise, <c>false</c>.</value>
        public bool MyClrHandlersAdded
        {
            get
            {
                return MyClrEvent != null;
            }
        }

        /// <summary>
        /// Gets my string field.
        /// </summary>
        /// <value>My string field.</value>
        public string MyStringField
        {
            get
            {
                return _myStringField;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [my CLR event reached].
        /// </summary>
        /// <value><c>true</c> if [my CLR event reached]; otherwise, <c>false</c>.</value>
        public bool MyClrEventReached
        {
            get
            {
                return _myClrEventReached;
            }

            set
            {
                _myClrEventReached = value;
            }
        }

        #endregion

        /// <summary>
        /// Reads the attached property MyTransparency from the given element.
        /// </summary>
        /// <param name="e">The element from which to read the attached property.</param>
        /// <returns>The property's value.</returns>
        public static string GetMyTransparency(DependencyObject e)
        {
            return (string)e.GetValue(MyTransparencyProperty);
        }

        /// <summary>
        /// Writes the attached property MyTransparency to the given element.
        /// </summary>
        /// <param name="e">The element to which to write the attached property.</param>
        /// <param name="myTransparency">The property value to set</param>
        public static void SetMyTransparency(DependencyObject e, string myTransparency)
        {
            if (e is UIElement)
            {
                (e as UIElement).RaiseEvent(new RoutedEventArgs(MyRoutedEvent, e));
            }
        }

        /// <summary>
        /// Reads the attached property MyColor from the given element.
        /// </summary>
        /// <param name="e">The element from which to read the attached property.</param>
        /// <returns>The property's value.</returns>
        public static MyColor GetMyColor(DependencyObject e)
        {
            return (MyColor)e.GetValue(MyColorProperty);
        }

        /// <summary>
        /// Writes the attached property MyColor to the given element.
        /// </summary>
        /// <param name="e">The element to which to write the attached property.</param>
        /// <param name="myColor">The property value to set</param>
        public static void SetMyColor(DependencyObject e, MyColor myColor)
        {
        }

        /// <summary>
        /// Reads the attached property MyChildren from the given element.
        /// </summary>
        /// <param name="e">The element from which to read the attached property.</param>
        /// <returns>The property's value.</returns>
        public static MyChildren GetMyChildren(DependencyObject e)
        {
            return (MyChildren)e.GetValue(MyChildrenProperty);
        }

        /// <summary>
        /// Writes the attached property MyChildren to the given element.
        /// </summary>
        /// <param name="e">The element to which to write the attached property.</param>
        /// <param name="myChildren">The property value to set</param>
        public static void SetMyChildren(DependencyObject e, MyChildren myChildren)
        {
            e.SetValue(MyChildrenProperty, myChildren);
        }

        /// <summary>
        /// Reads the attached property MyArrayList from the given element.
        /// </summary>
        /// <param name="e">The element from which to read the attached property.</param>
        /// <returns>The property's value.</returns>
        public static ArrayList GetMyArrayList(DependencyObject e)
        {
            return (ArrayList)e.GetValue(MyArrayListProperty);
        }

        /// <summary>
        /// Writes the attached property MyArrayList to the given element.
        /// </summary>
        /// <param name="e">The element to which to write the attached property.</param>
        /// <param name="myArrayList">The property value to set</param>
        public static void SetMyArrayList(DependencyObject e, ArrayList myArrayList)
        {
        }

        #region MyBrushesProperty

        /// <summary>
        /// Reads the attached property MyBrushes from the given element.
        /// </summary>
        /// <param name="e">The element from which to read the attached property.</param>
        /// <returns>The property's value.</returns>
        public static SolidColorBrush[] GetMyBrushes(DependencyObject e)
        {
            return (SolidColorBrush[])e.GetValue(MyBrushesProperty);
        }

        /// <summary>
        /// Writes the attached property MyBrushes to the given element.
        /// </summary>
        /// <param name="e">The element to which to write the attached property.</param>
        /// <param name="myBrushes">The property value to set</param>
        public static void SetMyBrushes(DependencyObject e, SolidColorBrush[] myBrushes)
        {
            e.SetValue(MyBrushesProperty, myBrushes);
        }

        #endregion MyBrushesProperty

        #region MyIDictProperty

        /// <summary>
        /// Reads the attached property MyIDict from the given element.
        /// </summary>
        /// <param name="e">The element from which to read the attached property.</param>
        /// <returns>The property's value.</returns>
        public static Hashtable GetMyIDict(DependencyObject e)
        {
            return (Hashtable)e.GetValue(MyIDictProperty);
        }

        /// <summary>
        /// Writes the attached property MyIDict to the given element.
        /// </summary>
        /// <param name="e">The element to which to write the attached property.</param>
        /// <param name="value">The property value to set</param>
        public static void SetMyIDict(DependencyObject e, Hashtable value)
        {
            e.SetValue(MyIDictProperty, value);
        }

        #endregion MyIDictProperty

        #region MyColorSetProperty

        /// <summary>
        /// Reads the attached property MyColorSet from the given element.
        /// </summary>
        /// <param name="e">The element from which to read the attached property.</param>
        /// <returns>The property's value.</returns>
        public static MyColorSet GetMyColorSet(DependencyObject e)
        {
            return (MyColorSet)e.GetValue(MyColorSetProperty);
        }

        /// <summary>
        /// Writes the attached property MyColorSet to the given element.
        /// </summary>
        /// <param name="e">The element to which to write the attached property.</param>
        /// <param name="myColorSet">The property value to set</param>
        public static void SetMyColorSet(DependencyObject e, MyColorSet myColorSet)
        {
        }

        #endregion MyColorSetProperty

        #region MyRoutedEvent

        /// <summary>
        /// Add MyRoutedEvent handler
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="handler">The handler.</param>
        public static void AddMyRoutedEventHandler(DependencyObject d, MyRoutedEventHandler handler)
        {
            (d as UIElement).AddHandler(MyRoutedEvent, handler);
        }

        /// <summary>
        /// Remove MyRoutedEvent handler
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="handler">The handler.</param>
        public static void RemoveMyRoutedEventHandler(DependencyObject d, MyRoutedEventHandler handler)
        {
            (d as UIElement).RemoveHandler(MyRoutedEvent, handler);
        }

        #endregion MyRoutedEvent

        #region MyClrEvent

        /// <summary>
        /// Fires the MyClr event with EventArgs.Empty
        /// </summary>
        public void FireMyClrEvent()
        {
            // Erase any previously set flag value.
            MyClrEventReached = false;

            // Fire the event
            if (MyClrEvent != null)
            {
                MyClrEvent(this, EventArgs.Empty);
            }
        }

        #endregion MyClrEvent

        #region Class MyInnerClass

        /// <summary>
        /// Inner class definition
        /// </summary>
        public class MyInnerClass
        {
            /// <summary>
            /// My LabelField
            /// </summary>
            private static string s_myLabelField = "Inner class";

            /// <summary>
            /// Gets or sets my temperature.
            /// </summary>
            /// <value>My temperature.</value>
            public double MyTemperature { get; set; }

            /// <summary>
            /// Gets my label field.
            /// </summary>
            /// <value>My label field.</value>
            public string MyLabelField
            {
                get
                {
                    return s_myLabelField;
                }
            }
        }

        #endregion Class MyInnerClass
    }

    #endregion Class MyClass

    #region Class MyColor

    /// <summary>
    /// The type of MyClass's MyColor property
    /// </summary>
    [TypeConverter(typeof(MyColorConverter))]
    [ContentProperty("Color")]
    public class MyColor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyColor"/> class.
        /// </summary>
        public MyColor()
        {
            Color = "Red"; // default
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>Value of the color string to set.</value>
        public string Color { get; set; }
    }

    #endregion Class MyColor

    #region Class MyChildren

    /// <summary>
    /// The type of MyClass's MyChildren property
    /// </summary>
    [ContentProperty("Children")]
    public class MyChildren
    {
        /// <summary>
        /// List of Child nodes
        /// </summary>
        private readonly ArrayList _children = new ArrayList();

        /// <summary>
        /// Initializes a new instance of the <see cref="MyChildren"/> class.
        /// </summary>
        public MyChildren()
        {
            _children = new ArrayList();
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ArrayList Children
        {
            get
            {
                return _children;
            }
        }

        /// <summary>
        /// Add a child.
        /// </summary>
        /// <param name="o">Child to be added</param>
        public void Add(object o)
        {
            _children.Add(o);
        }

        /// <summary>
        /// Add a text child
        /// </summary>
        /// <param name="s">Text to be added as child</param>
        public void Add(string s)
        {
            _children.Add(s);
        }
    }

    #endregion Class MyChildren

    #region Class MyColorConverter

    /// <summary>
    /// Typeconverter for MyColor class.
    /// </summary>
    public class MyColorConverter : TypeConverter
    {
        /// <summary>
        /// CanConvertFrom - Returns whether or not MyColor can convert from a given type.
        /// </summary>
        /// <returns>
        /// bool - True if the provided type is string, false if not.
        /// </returns>
        /// <param name="typeDescriptorContext"> The ITypeDescriptorContext for this call. </param>
        /// <param name="sourceType"> The Type being queried for support. </param>
        public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type sourceType)
        {
            // We can only handle string.
            if (sourceType == typeof(string))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// CanConvertTo - Returns whether or not MyColor can convert to a given type.
        /// </summary>
        /// <returns>
        /// bool - True if the provided type is string, false if not.
        /// </returns>
        /// <param name="typeDescriptorContext"> The ITypeDescriptorContext for this call. </param>
        /// <param name="destinationType"> The Type being queried for support. </param>
        public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type destinationType)
        {
            // We can convert to an InstanceDescriptor or to a string.
            if (destinationType == typeof(string))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// ConvertFrom - Attempt to convert to a MyColor from the given object
        /// </summary>
        /// <param name="typeDescriptorContext"> The ITypeDescriptorContext for this call. </param>
        /// <param name="cultureInfo"> The CultureInfo which is respected when converting. </param>
        /// <param name="value"> The object to convert to a MyColor. </param>
        /// <returns>The MyColor object created.</returns>
        /// <exception>
        /// An ArgumentException is thrown if the example object is not null and is not a valid type
        /// which can be converted to a MyColor.
        /// </exception>
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value)
        {
            string s = value as string;

            if (null == s)
            {
                throw new ArgumentException("The given type cannot be converted to MyColor");
            }

            MyColor m = new MyColor();
            m.Color = s;
            return m;
        }

        /// <summary>
        /// ConvertTo - Attempt to convert from a given object (should be MyColor) to an object of the given type
        /// </summary>
        /// <param name="typeDescriptorContext"> The ITypeDescriptorContext for this call. </param>
        /// <param name="cultureInfo"> The CultureInfo which is respected when converting. </param>
        /// <param name="value"> The object given to convert. Should be MyColor</param>
        /// <param name="destinationType"> The type to which this will convert the given object. </param>
        /// <returns>The object that was created.</returns>
        /// <exception>
        /// An ArgumentException is thrown if the example object is not null and is not a MyColor,
        /// or if the destinationType isn't one of the valid destination types.
        /// </exception>
        public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value, Type destinationType)
        {
            MyColor m = value as MyColor;

            if (m == null)
            {
                throw new ArgumentException("The given object is not a MyColor");
            }

            if (destinationType != typeof(string))
            {
                new ArgumentException("MyColor cannot be converted to the given type.");
            }

            return m.Color;
        }
    }

    #endregion Class MyColorConverter

    #region Class MyColorSet

    /// <summary>
    /// The type of MyClass's MyColorSet property
    /// </summary>
    public class MyColorSet
    {
        /// <summary>
        /// MyColor[] store
        /// </summary>
        private readonly MyColor[] _store;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyColorSet"/> class.
        /// </summary>
        public MyColorSet()
        {
            _store = null;
        }

        /// <summary>
        /// Gets or sets the <see cref="Microsoft.Test.Xaml.Types.MyColor"/> at the specified index.
        /// </summary>
        /// <param name="index">int index value</param>
        /// <value></value>
        public MyColor this[int index]
        {
            get
            {
                return _store[index];
            }

            set
            {
                _store[index] = value;
            }
        }
    }

    #endregion Class MyColorSet
}
