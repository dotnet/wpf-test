// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Globalization;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;

using Avalon.Test.CoreUI.Common;

namespace Avalon.Test.CoreUI.Parser
{
    #region Class MyClass
    /// <summary>
    /// This class defines custom attached properties and a custom attached event that 
    /// can be used with other dependency objects.
    /// </summary>
    public class MyClass
    {
        #region MyTransparencyProperty
        /// <summary>
        /// DependencyProperty for the attached MyTransparency property.
        /// </summary>
        public static readonly DependencyProperty MyTransparencyProperty = DependencyProperty.RegisterAttached("MyTransparency", typeof(String), typeof(MyClass), new FrameworkPropertyMetadata("0.0"));

        /// <summary>
        /// Reads the attached property MyTransparency from the given element.
        /// </summary>
        /// <param name="e">The element from which to read the attached property.</param>
        /// <returns>The property's value.</returns>
        public static String GetMyTransparency(DependencyObject e)
        {
            return (String)e.GetValue(MyTransparencyProperty);
        }

        /// <summary>
        /// Writes the attached property MyTransparency to the given element.
        /// </summary>
        /// <param name="e">The element to which to write the attached property.</param>
        /// <param name="myTransparency">The property value to set</param>
        public static void SetMyTransparency(DependencyObject e, String myTransparency)
        {
            if (e is UIElement)
            {
                (e as UIElement).RaiseEvent(new RoutedEventArgs(MyRoutedEvent, e));
            }
        }
        #endregion MyTransparencyProperty

        #region MyColorProperty
        /// <summary>
        /// DependencyProperty for the attached MyColor property.
        /// </summary>
        public static readonly DependencyProperty MyColorProperty = DependencyProperty.RegisterAttached("MyColor", typeof(MyColor), typeof(MyClass));


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
        #endregion MyColorProperty

        #region MyChildrenProperty
        /// <summary>
        /// DependencyProperty for the attached MyChildren property.
        /// </summary>
        public static readonly DependencyProperty MyChildrenProperty = DependencyProperty.RegisterAttached("MyChildren", typeof(MyChildren), typeof(MyClass));


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
        #endregion MyChildrenProperty

        #region MyArrayListProperty
        /// <summary>
        /// DependencyProperty for the attached MyArrayList property.
        /// </summary>
        public static readonly DependencyProperty MyArrayListProperty = DependencyProperty.RegisterAttached("MyArrayList", typeof(ArrayList), typeof(MyClass));


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
        #endregion MyArrayListProperty

        #region MyBrushesProperty
        /// <summary>
        /// DependencyProperty for the attached MyBrushes property.
        /// </summary>
        public static readonly DependencyProperty MyBrushesProperty = DependencyProperty.RegisterAttached("MyBrushes", typeof(SolidColorBrush[]), typeof(MyClass));
                

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
        /// DependencyProperty for the attached MyIDict property.
        /// </summary>
        public static readonly DependencyProperty MyIDictProperty = DependencyProperty.RegisterAttached("MyIDict", typeof(Hashtable), typeof(MyClass));

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
        /// DependencyProperty for the attached MyColorSet property.
        /// </summary>
        public static readonly DependencyProperty MyColorSetProperty = DependencyProperty.RegisterAttached("MyColorSet", typeof(MyColorSet), typeof(MyClass));


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
        /// <param name="MyColorSet">The property value to set</param>
        public static void SetMyColorSet(DependencyObject e, MyColorSet MyColorSet)
        {
        }
        #endregion MyColorSetProperty

        #region MyRoutedEvent
        /// <summary>
        /// Custom event.
        /// </summary>
        public static readonly RoutedEvent MyRoutedEvent = EventManager.RegisterRoutedEvent("MyRouted", RoutingStrategy.Bubble, typeof(MyRoutedEventHandler), typeof(MyClass));
        
        /// <summary>
        /// Add MyRoutedEvent handler
        /// </summary>
        public static void AddMyRoutedEventHandler(DependencyObject d, MyRoutedEventHandler handler)
        { 
            (d as UIElement).AddHandler(MyRoutedEvent, handler); 
        }

        /// <summary>
        /// Remove MyRoutedEvent handler
        /// </summary>
        public static void RemoveMyRoutedEventHandler(DependencyObject d, MyRoutedEventHandler handler)
        {
            (d as UIElement).RemoveHandler(MyRoutedEvent, handler);
        }

        /// <summary>
        ///     This delegate must used by handlers of the MyRoutedEvent event.
        /// </summary>
        /// <param name="sender">The current element along the event's route.</param>
        /// <param name="e">The event arguments containing additional information about the event.</param>
        /// <returns>Nothing.</returns>
        public delegate void MyRoutedEventHandler(object sender, RoutedEventArgs e);

        #endregion MyRoutedEvent

        #region MyClrEvent
        /// <summary>
        /// Custom CLR Event 
        /// </summary>
        public event MyClrEventHandler MyClrEvent;

        /// <summary>
        /// Delegate for the event.
        /// </summary>
        /// <param name="source">Source of the event</param>
        /// <param name="e">Event args</param>
        public delegate void MyClrEventHandler(object source, EventArgs e);

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

        /// <summary>
        /// This flag denotes that a handler received MyClrEvent. 
        /// We clear it in the FireMyClrEvent() function above, before firing the event. 
        /// We set it in a handler function, so that we can verify that the event was properly fired.
        /// </summary>
        public bool MyClrEventReached = false;

        /// <summary>
        /// Informs whether there are event handlers currently added for MyClrEvent
        /// </summary>
        /// <value></value>
        public bool MyClrHandlersAdded
        {
            get { return (MyClrEvent != null); }
        }
        #endregion MyClrEvent

        #region MyGreetingField
        /// <summary>
        /// MyGreeting
        /// </summary>
        public static string MyGreetingField = "Hello world";
        #endregion MyGreetingField

        #region MyStringField
        /// <summary>
        /// MyStringField
        /// </summary>
        public string MyStringField = "Foo bar baz";
        #endregion MyStringField

        #region Class MyInnerClass
        /// <summary>
        /// Inner class definition
        /// </summary>
        public class MyInnerClass
        {
            /// <summary>
            /// MyLabelField
            /// </summary>
            public static string MyLabelField = "Inner class";

            private double _myTemperature;
            /// <summary>
            /// MyTemperature
            /// </summary>
            /// <value></value>
            public double MyTemperature
            {
                get { return _myTemperature; }
                set { _myTemperature = value; }
            }
        }
        #endregion Class MyInnerClass
    }
    #endregion Class MyClass

    #region Struct MyStruct
    /// <summary>
    /// A custom struct to be used for multiple tests
    /// </summary>
    public struct MyStruct
    {
        /// <summary>
        /// MyMotto
        /// </summary>
        public static string MyMottoField = "My motto";

        private FontStyle _myFont;
        /// <summary>
        /// MyFont
        /// </summary>
        /// <value></value>
        public FontStyle MyFont
        {
            get { return _myFont; }
            set { _myFont = value; }
        }
    }
    #endregion Struct MyStruct

    #region Class MyColor
    /// <summary>
    /// The type of MyClass's MyColor property
    /// </summary>
    [TypeConverter(typeof(MyColorConverter))]
    [ContentProperty("Color")]
    public class MyColor
    {
        private String _color;

        /// <summary>
        /// Default constructor
        /// </summary>
        public MyColor()
        {
            _color = "Red"; //default
        }

        /// <summary>
        /// Color property
        /// </summary>
        /// <value>Value of the color string to set.</value>
        public String Color
        {
            get { return _color; }
            set { _color = value; }
        }
    }
    #endregion Class MyColor

    #region Class MyChildren
    /// <summary>
    /// The type of MyClass's MyChildren property
    /// </summary>
    [ContentProperty("Children")]
    public class MyChildren
    {
        private ArrayList _Children = new ArrayList();

        /// <summary>
        /// Default constructor
        /// </summary>
        public MyChildren()
        {
            _Children = new ArrayList(); 
        }

        /// <summary>
        /// Add a child.
        /// </summary>
        /// <param name="o">Child to be added</param>
        public void Add(Object o)
        {
            _Children.Add(o);
        }

        /// <summary>
        /// Add a text child
        /// </summary>
        /// <param name="s">Text to be added as child</param>
        public void Add(string s)
        {
            _Children.Add(s);
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ArrayList Children
        {
            get { return _Children; }
        }
    }
    #endregion Class MyChildren
 
    #region Class MyColorConverter
    /// <summary>
    /// Typeconverter for MyColor class.
    /// </summary>
    public class MyColorConverter: TypeConverter
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
            String s = value as string;

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
        private MyColor[] _store;

        /// <summary>
        /// Default constructor
        /// </summary>
        public MyColorSet()
        {
            _store = null;
        }

        /// <summary>
        /// Indexers
        /// </summary>
        public MyColor this[int index]
        {
            get { return _store[index]; }
            set { _store[index] = value; }
        }
    }
    #endregion Class MyColorSet
}
