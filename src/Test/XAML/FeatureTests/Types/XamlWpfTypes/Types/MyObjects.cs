// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Controls;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// enum MyEnum
    /// </summary>
    [TypeConverter(typeof(MyEnumCvt))]
    public enum MyEnum
    {
        /// <summary> One value. </summary>
        One,

        /// <summary> Two value. </summary>
        Two,

        /// <summary> Three value </summary>
        Three
    }

    /// <summary>
    /// MyButton class
    /// </summary>
    public class MyButton : Button
    {
        /// <summary>
        /// MyEnum fooMyEnum
        /// </summary>
        private MyEnum _fooMyEnum;

        /// <summary>
        /// Gets or sets the foo my enum.
        /// </summary>
        /// <value>The foo my enum.</value>
        public MyEnum FooMyEnum
        {
            get
            {
                return _fooMyEnum;
            }

            set
            {
                _fooMyEnum = value;
                this.Content = _fooMyEnum.ToString();
            }
        }
    }

    /// <summary>
    /// The type converter class used in this test.
    /// </summary>
    public class MyEnumCvt : TypeConverter
    {
        /// <summary>
        /// To define type converter class we are required to override this
        /// method
        /// </summary>
        /// <param name="tpx">The TPX value.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="obj">The obj value.</param>
        /// <returns>object value</returns>
        public override object ConvertFrom(ITypeDescriptorContext tpx, System.Globalization.CultureInfo culture, object obj)
        {
            if (obj is string)
            {
                string s = (string) obj;
                if (String.Compare(s, "Uno") == 0)
                {
                    return MyEnum.One;
                }

                if (String.Compare(s, "Dos") == 0)
                {
                    return MyEnum.Two;
                }

                if (String.Compare(s, "Tres") == 0)
                {
                    return MyEnum.Three;
                }

                if (Enum.IsDefined(typeof(MyEnum), obj))
                {
                    return Enum.Parse(typeof(MyEnum), s, true);
                }
            }

            if (obj is Enum)
            {
                return obj.ToString();
            }

            throw new Exception("Invalid Value");
        }

        /// <summary>
        /// Determines whether this instance [can convert from] the specified TDC.
        /// </summary>
        /// <param name="tdc">The ITypeDescriptorContext.</param>
        /// <param name="t">The Type t.</param>
        /// <returns>
        /// <c>true</c> if this instance [can convert from] the specified TDC; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext tdc, Type t)
        {
            return true;
        }
    }
}
