// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// GenericTypeWithFactoryMethod CLASS
    /// </summary>
    /// <typeparam name="T">Type t value</typeparam>
    public class GenericTypeWithFactoryMethod<T>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the GenericTypeWithFactoryMethod class.
        /// </summary>
        /// <param name="i">The int i.</param>
        public GenericTypeWithFactoryMethod(int i)
        {
            IntProp = i;
        }

        /// <summary>
        /// Initializes a new instance of the GenericTypeWithFactoryMethod class.
        /// </summary>
        /// <param name="g">The GenericTypeWithFactoryMethod value.</param>
        public GenericTypeWithFactoryMethod(GenericTypeWithFactoryMethod<T> g)
        {
            GenericTypeProp = g;
        }

        /// <summary>
        /// Initializes a new instance of the GenericTypeWithFactoryMethod class.
        /// </summary>
        /// <param name="i">The int i.</param>
        /// <param name="d">The double d.</param>
        /// <param name="s">The string s.</param>
        /// <param name="b">if set to <c>true</c> [b].</param>
        /// <param name="g">The GenericTypeWithFactoryMethod g.</param>
        public GenericTypeWithFactoryMethod(int i, double d, string s, bool b, GenericTypeWithFactoryMethod<T> g)
        {
            IntProp = i;
            DoubleProp = d;
            StringProp = s;
            BooleanProp = b;
            GenericTypeProp = g;
        }

        /// <summary>
        /// Prevents a default instance of the GenericTypeWithFactoryMethod class from being created.
        /// </summary>
        private GenericTypeWithFactoryMethod()
        {
        }

        #endregion

        #region Properties

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
        /// Gets or sets the generic type prop.
        /// </summary>
        /// <value>The generic type prop.</value>
        public GenericTypeWithFactoryMethod<T> GenericTypeProp { get; set; }

        #endregion

        #region Factory methods

        /// <summary>
        /// Factories this instance.
        /// </summary>
        /// <returns>GenericTypeWithFactoryMethod value</returns>
        public static GenericTypeWithFactoryMethod<T> Factory()
        {
            return new GenericTypeWithFactoryMethod<T>();
        }

        /// <summary>
        /// Factories the specified i.
        /// </summary>
        /// <param name="i">The int  i.</param>
        /// <returns>GenericTypeWithFactoryMethod value</returns>
        public static GenericTypeWithFactoryMethod<T> Factory(int i)
        {
            GenericTypeWithFactoryMethod<T> obj = new GenericTypeWithFactoryMethod<T>();
            obj.IntProp = i;
            return obj;
        }

        /// <summary>
        /// Factories the specified g.
        /// </summary>
        /// <param name="g">The GenericTypeWithFactoryMethod g.</param>
        /// <returns>GenericTypeWithFactoryMethod value</returns>
        public static GenericTypeWithFactoryMethod<T> Factory(GenericTypeWithFactoryMethod<T> g)
        {
            GenericTypeWithFactoryMethod<T> obj = new GenericTypeWithFactoryMethod<T>();
            obj.GenericTypeProp = g;
            return obj;
        }

        /// <summary>
        /// Factories the specified i.
        /// </summary>
        /// <param name="i">The int i.</param>
        /// <param name="d">The double d.</param>
        /// <param name="s">The string s.</param>
        /// <param name="b">if set to <c>true</c> [b].</param>
        /// <param name="g">The GenericTypeWithFactoryMethod g.</param>
        /// <returns>GenericTypeWithFactoryMethod value</returns>
        public static GenericTypeWithFactoryMethod<T> Factory(int i, double d, string s, bool b, GenericTypeWithFactoryMethod<T> g)
        {
            GenericTypeWithFactoryMethod<T> obj = new GenericTypeWithFactoryMethod<T>();
            obj.IntProp = i;
            obj.DoubleProp = d;
            obj.StringProp = s;
            obj.BooleanProp = b;
            obj.GenericTypeProp = g;
            return obj;
        }

        #endregion
    }
}
