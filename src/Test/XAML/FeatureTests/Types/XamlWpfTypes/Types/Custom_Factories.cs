// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Media;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// CustomFactoryNested Test class
    /// </summary>
    public class CustomFactoryNested
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFactoryNested"/> class.
        /// </summary>
        /// <param name="d1">The double value d1.</param>
        /// <param name="c">The SimpleCustomObject c.</param>
        /// <param name="d2">The  double value d2.</param>
        public CustomFactoryNested(double d1, SimpleCustomObject c, double d2)
        {
            ActualDouble1 = d1;
            ActualDouble2 = d2;
        }

        /// <summary>
        /// Gets or sets the actual double1.
        /// </summary>
        /// <value>The actual double1.</value>
        public double ActualDouble1 { get; set; }

        /// <summary>
        /// Gets or sets the actual double2.
        /// </summary>
        /// <value>The actual double2.</value>
        public double ActualDouble2 { get; set; }
    }

    /// <summary>
    ///  SimpleCustomObject Test class
    /// </summary>
    public class SimpleCustomObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCustomObject"/> class.
        /// </summary>
        /// <param name="s">The string value</param>
        public SimpleCustomObject(string s)
        {
        }
    }

    /// <summary>
    /// CustomFactoryWithStringArg Test class
    /// </summary>
    public class CustomFactoryWithStringArg : UIElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFactoryWithStringArg"/> class.
        /// </summary>
        /// <param name="s">The string value</param>
        public CustomFactoryWithStringArg(string s)
        {
        }
        
        /// <summary>
        /// Gets or sets the actual string.
        /// </summary>
        /// <value>The actual string.</value>
        public static string ActualString { get; set; }

        /// <summary>
        /// Factories the specified s.
        /// </summary>
        /// <param name="s">The string value</param>
        /// <returns> CustomFactoryWithStringArg object </returns>
        public static CustomFactoryWithStringArg Factory(string s)
        {
            ActualString = s;
            CustomFactoryWithStringArg obj = new CustomFactoryWithStringArg(s);
            return obj;
        }
    }

    /// <summary>
    /// CustomFactoryWithDoubleArg Test class
    /// </summary>
    public class CustomFactoryWithDoubleArg
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFactoryWithDoubleArg"/> class.
        /// </summary>
        /// <param name="d">The double value</param>
        public CustomFactoryWithDoubleArg(double d)
        {
            ActualDouble4 = d;
        }
        
        /// <summary>
        /// Gets or sets the actual double4.
        /// </summary>
        /// <value>The actual double4.</value>
        public double ActualDouble4 { get; set; }
    }

    /// <summary>
    /// CustomFactory With CustomProp
    /// </summary>
    public class CustomFactoryWithCustomProp
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFactoryWithCustomProp"/> class.
        /// </summary>
        public CustomFactoryWithCustomProp()
        {
        }

        /// <summary>
        /// Gets or sets the custom prop.
        /// </summary>
        /// <value>The custom prop.</value>
        public CustomFactoryWithColorProp CustomProp { get; set; }

        /// <summary>
        /// Factories this instance.
        /// </summary>
        /// <returns>CustomFactoryWithCustomProp object</returns>
        public static CustomFactoryWithCustomProp Factory()
        {
            return new CustomFactoryWithCustomProp();
        }
    }

    /// <summary>
    /// CustomFactory With ColorProp
    /// </summary>
    public class CustomFactoryWithColorProp
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFactoryWithColorProp"/> class.
        /// </summary>
        /// <param name="d">The double value</param>
        public CustomFactoryWithColorProp(double d)
        {
        }

        /// <summary>
        /// Gets or sets the color prop.
        /// </summary>
        /// <value>The color prop.</value>
        public Color ColorProp { get; set; }

        /// <summary>
        /// Factories the specified d.
        /// </summary>
        /// <param name="d">The double value</param>
        /// <returns>CustomFactoryWithColorProp object</returns>
        public static CustomFactoryWithColorProp Factory(double d)
        {
            return new CustomFactoryWithColorProp(d);
        }
    }

    /// <summary>
    /// CustomFactory With NoArgs
    /// </summary>
    public class CustomFactoryWithNoArgs : UIElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFactoryWithNoArgs"/> class.
        /// </summary>
        public CustomFactoryWithNoArgs()
        {
        }

        /// <summary>
        /// Factories this instance.
        /// </summary>
        /// <returns>CustomFactoryWithNoArgs object</returns>
        public static CustomFactoryWithNoArgs Factory()
        {
            return new CustomFactoryWithNoArgs();
        }
    }

    /// <summary>
    /// CustomFactory With ArgAndProps
    /// </summary>
    public class CustomFactoryWithArgAndProps : UIElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFactoryWithArgAndProps"/> class.
        /// </summary>
        /// <param name="s">The string value.</param>
        public CustomFactoryWithArgAndProps(string s)
        {
        }
        
        /// <summary>
        /// Gets or sets the int prop.
        /// </summary>
        /// <value>The int prop.</value>
        public int IntProp { get; set; }

        /// <summary>
        /// Gets or sets the double prop.
        /// </summary>
        /// <value>The double prop.</value>
        public double DoubleProp { get; set; }

        /// <summary>
        /// Gets or sets the string prop.
        /// </summary>
        /// <value>The string prop.</value>
        public string StringProp { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [boolean prop].
        /// </summary>
        /// <value><c>true</c> if [boolean prop]; otherwise, <c>false</c>.</value>
        public bool BooleanProp { get; set; }

        /// <summary>
        /// Factories the specified object.
        /// </summary>
        /// <param name="s">The string value.</param>
        /// <returns>CustomFactoryWithArgAndProps object</returns>
        public static CustomFactoryWithArgAndProps Factory(string s)
        {
            return new CustomFactoryWithArgAndProps(s);
        }
    }

    /// <summary>
    /// CustomObject With MultipleArgs
    /// </summary>
    public class CustomObjectWithMultipleArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomObjectWithMultipleArgs"/> class.
        /// </summary>
        /// <param name="s1">The string value s1.</param>
        /// <param name="d">The double value d.</param>
        /// <param name="b">if set to <c>true</c> [b].</param>
        /// <param name="s2">The string value s2.</param>
        /// <param name="arr">The string array arr.</param>
        public CustomObjectWithMultipleArgs(string s1, double d, bool b, string s2, string[] arr)
        {
            ActualString1 = s1;
            ActualDouble = d;
            ActualBool = b;
            ActualString2 = s2;
            ActualStringArray = arr;
        }

        /// <summary>
        /// Gets or sets the actual string1.
        /// </summary>
        /// <value>The actual string1.</value>
        public string ActualString1 { get; set; }

        /// <summary>
        /// Gets or sets the actual double.
        /// </summary>
        /// <value>The actual double.</value>
        public double ActualDouble { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [actual bool].
        /// </summary>
        /// <value><c>true</c> if [actual bool]; otherwise, <c>false</c>.</value>
        public bool ActualBool { get; set; }

        /// <summary>
        /// Gets or sets the actual string2.
        /// </summary>
        /// <value>The actual string2.</value>
        public string ActualString2 { get; set; }

        /// <summary>
        /// Gets or sets the actual string array.
        /// </summary>
        /// <value>The actual string array.</value>
        public string[] ActualStringArray { get; set; }
    }
}
