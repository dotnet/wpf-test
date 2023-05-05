// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// Yeah, this is a non-generic type with name GenericType.
    /// Meant to test the scenario where non-generic type and generic type have same name
    /// </summary>
    public class GenericType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericType"/> class.
        /// </summary>
        public GenericType()
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
    }

    /// <summary>
    /// GenericType class
    /// </summary>
    /// <typeparam name="T">Type T for the Generic class</typeparam>
    public class GenericType<T>
    {
        /// <summary>
        /// Initializes a new instance of the GenericType class.
        /// </summary>
        public GenericType()
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
        /// Gets or sets the T prop.
        /// </summary>
        /// <value>The T prop.</value>
        public T TProp { get; set; }
    }

    /// <summary>
    /// GenericType class
    /// </summary>
    /// <typeparam name="T">Type T value</typeparam>
    /// <typeparam name="U">Type U value</typeparam>
    public class GenericType<T, U>
    {
        /// <summary>
        /// Initializes a new instance of the GenericType class.
        /// </summary>
        public GenericType()
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
        /// Gets or sets the T prop.
        /// </summary>
        /// <value>The T prop.</value>
        public T TProp { get; set; }

        /// <summary>
        /// Gets or sets the U prop.
        /// </summary>
        /// <value>The U prop.</value>
        public U UProp { get; set; }
    }

    /// <summary>
    /// GenericType class
    /// </summary>
    /// <typeparam name="T">Type value T</typeparam>
    /// <typeparam name="U">Type value U</typeparam>
    /// <typeparam name="V">Type value V</typeparam>
    public class GenericType<T, U, V>
    {
        /// <summary>
        /// Initializes a new instance of the GenericType class.
        /// </summary>
        public GenericType()
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
        /// Gets or sets the T prop.
        /// </summary>
        /// <value>The T prop.</value>
        public T TProp { get; set; }

        /// <summary>
        /// Gets or sets the U prop.
        /// </summary>
        /// <value>The U prop.</value>
        public U UProp { get; set; }

        /// <summary>
        /// Gets or sets the V prop.
        /// </summary>
        /// <value>The V prop.</value>
        public V VProp { get; set; }
    }
}
