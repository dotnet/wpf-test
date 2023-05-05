// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//  Desc: Integration wrapper object to explore PMEs form objects
//




using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Navigation;

using MS.Internal;

namespace Microsoft.Test.Animation
{

    /// <summary>
    /// Data container for Dynamic property information
    /// </summary>
    public class IntegrationProperty
    {
        // Properties

        /// <summary>
        /// Property name, e.g "Background"
        /// </summary>
        public string Name { get{ return _Name; } set{ _Name = value; } }
        /// <summary>
        /// Property's type name, e.g "Brush"
        /// </summary>
        public string Type { get{ return _Type; } set{ _Type = value; } }
        /// <summary>
        /// Class that declared the property, may be different for attached properties
        /// </summary>
        public string Owner { get{ return _Owner; } set{ _Owner = value; } }
        /// <summary>
        /// String representation of the value given by default, on property declaration
        /// </summary>
        public string DefaultValue { get{ return _DefaultValue; } set{ _DefaultValue = value; } }
        /// <summary>
        /// String representation of the value currently on the analyzed object
        /// </summary>
        public string CurrentValue { get{ return _CurrentValue; } set{ _CurrentValue = value; } }
        /// <summary>
        /// Whether it is read only or read/write
        /// </summary>
        public bool ReadOnly { get{ return _ReadOnly; } set{ _ReadOnly = value; } }
        /// <summary>
        /// Whether it is considered private
        /// </summary>
        public bool Private { get{ return _Private; } set{ _Private = value; } }
        /// <summary>
        /// Whether it is considered bindable
        /// </summary>
        public bool DataBindable { get{ return _DataBindable; } set{ _DataBindable = value; } }
        /// <summary>
        /// Reference to the actual dynamic property we are referencing
        /// </summary>
        public DependencyProperty OriginalProperty { get{ return _OriginalProperty; } set{ _OriginalProperty = value; } }
        /// <summary>
        /// Reference to the actual dynamic property we are referencing
        /// </summary>
        public Type OwnerType { get{ return _OwnerType; } set{ _OwnerType = value; } }

        // FrameworkElement metadata

        /// <summary>This property affects measurement</summary>
        public bool AffectsMeasure { get{ return _AffectsMeasure; } set{ _AffectsMeasure = value; } }
        /// <summary>This property affects arragement</summary>
        public bool AffectsArrange { get{ return _AffectsArrange; } set{ _AffectsArrange = value; } }
        /// <summary>This property affects parent's measurement</summary>
        public bool AffectsParentMeasure { get{ return _AffectsParentMeasure; } set{ _AffectsParentMeasure = value; } }
        /// <summary>This property affects parent's arrangement</summary>
        public bool AffectsParentArrange { get{ return _AffectsParentArrange; } set{ _AffectsParentArrange = value; } }
        /// <summary>This property affects rendering</summary>
        public bool AffectsRender { get{ return _AffectsRender; } set{ _AffectsRender = value; } }
        /// <summary>This property inherits to children</summary>
        public bool Inherits { get{ return _Inherits; } set{ _Inherits = value; } }
        /// <summary>This property spans sparated trees during inheritance</summary>
        public bool SpanSeparatedTrees { get{ return _SpanSeparatedTrees; } set{ _SpanSeparatedTrees = value; } }


        // Internal fields
        string _Name;
        string _Type;
        string _Owner;
        string _DefaultValue;
        string _CurrentValue;
        bool _ReadOnly;
        bool _Private;
        bool _DataBindable;
        bool _AffectsMeasure;
        bool _AffectsArrange;
        bool _AffectsParentMeasure;
        bool _AffectsParentArrange;
        bool _AffectsRender;
        bool _Inherits;
        bool _SpanSeparatedTrees;
        DependencyProperty _OriginalProperty;
        Type _OwnerType;

        // IComparer Implementations for this class

        /// <summary>
        /// IComparer implementation based on Name
        /// </summary>
        public class CompareName : System.Collections.IComparer
        {
            /// <summary>
            /// ICompare basic implementation
            /// </summary>
            /// <param name="x">lhs object to compare</param>
            /// <param name="y">rhs object to compare</param>
            /// <returns>0, 1 or -1 depending on comparison result</returns>
            public int Compare( object x, object y )
            {
                return string.Compare( ( x as IntegrationProperty).Name, ( y as IntegrationProperty).Name );
            }
        }
        /// <summary>
        /// IComparer implementation based on Owner
        /// </summary>
        public class CompareOwner : System.Collections.IComparer
        {
            /// <summary>
            /// ICompare basic implementation
            /// </summary>
            /// <param name="x">lhs object to compare</param>
            /// <param name="y">rhs object to compare</param>
            /// <returns>0, 1 or -1 depending on comparison result</returns>
            public int Compare( object x, object y )
            {
                return string.Compare( ( x as IntegrationProperty).Owner, ( y as IntegrationProperty).Owner );
            }
        }
        /// <summary>
        /// IComparer implementation based on DefaultValue
        /// </summary>
        public class CompareDefaultValue : System.Collections.IComparer
        {
            /// <summary>
            /// ICompare basic implementation
            /// </summary>
            /// <param name="x">lhs object to compare</param>
            /// <param name="y">rhs object to compare</param>
            /// <returns>0, 1 or -1 depending on comparison result</returns>
            public int Compare( object x, object y )
            {
                return string.Compare( ( x as IntegrationProperty).DefaultValue, ( y as IntegrationProperty).DefaultValue );
            }
        }
        /// <summary>
        /// IComparer implementation based on CurrentValue
        /// </summary>
        public class CompareCurrentValue : System.Collections.IComparer
        {
            /// <summary>
            /// ICompare basic implementation
            /// </summary>
            /// <param name="x">lhs object to compare</param>
            /// <param name="y">rhs object to compare</param>
            /// <returns>0, 1 or -1 depending on comparison result</returns>
            public int Compare( object x, object y )
            {
                return string.Compare( ( x as IntegrationProperty).CurrentValue, ( y as IntegrationProperty).CurrentValue );
            }
        }
        /// <summary>
        /// IComparer implementation based on Type
        /// </summary>
        public class PackageCompareTypes : System.Collections.IComparer
        {
            /// <summary>
            /// ICompare basic implementation
            /// </summary>
            /// <param name="x">lhs object to compare</param>
            /// <param name="y">rhs object to compare</param>
            /// <returns>0, 1 or -1 depending on comparison result</returns>
            public int Compare( object x, object y )
            {
                return string.Compare( ( x as IntegrationProperty).Type, ( y as IntegrationProperty).Type );
            }
        }
        /// <summary>
        /// IComparer implementation based on ReadOnly
        /// </summary>
        public class CompareReadOnly : System.Collections.IComparer
        {
            /// <summary>
            /// ICompare basic implementation
            /// </summary>
            /// <param name="x">lhs object to compare</param>
            /// <param name="y">rhs object to compare</param>
            /// <returns>0, 1 or -1 depending on comparison result</returns>
            public int Compare( object x, object y )
            {
                return string.Compare( ( x as IntegrationProperty).ReadOnly.ToString(), ( y as IntegrationProperty).ReadOnly.ToString() );
            }
        }

        /// <summary>
        /// IComparer implementation for multiple criteria
        /// </summary>
        public class CompareMultiple : System.Collections.IComparer
        {
            private System.Collections.IComparer[] _comparers;
            /// <summary>
            /// Constructor takes an array of IComparer interfaces for this class
            /// </summary>
            /// <param name="comparerList">array of any ICompare derived classed for this type</param>
            public CompareMultiple( System.Collections.IComparer[] comparerList )
            {
                _comparers = comparerList;
            }
            /// <summary>
            /// ICompare basic implementation, composes based on inner comparers
            /// </summary>
            /// <param name="x">lhs object to compare</param>
            /// <param name="y">rhs object to compare</param>
            /// <returns>0, 1 or -1 depending on comparison result</returns>
            public int Compare( object x, object y )
            {
                foreach( IComparer cmp in _comparers )
                {
                    // Compare current
                    int ans = cmp.Compare(x,y) ;
                    // Continue if equals
                    if ( ans != 0) return ans;
                }
                // if all else fails, they are equal
                return 0;
            }
        }

        /// <summary>
        /// IComparer implementation for inverse comparisons
        /// </summary>
        public class CompareInverse : System.Collections.IComparer
        {
            private System.Collections.IComparer _comparer;
            /// <summary>
            /// Constuctor takes another IComparer derived class and negates its comparison
            /// </summary>
            /// <param name="originalComparer">IComparer derived class for this type</param>
            public CompareInverse( System.Collections.IComparer originalComparer )
            {
                _comparer = originalComparer;
            }
            /// <summary>
            /// ICompare basic implementation, negates inner comparer
            /// </summary>
            /// <param name="x">lhs object to compare</param>
            /// <param name="y">rhs object to compare</param>
            /// <returns>0, 1 or -1 depending on comparison result</returns>
            public int Compare( object x, object y )
            {
                return -1 * _comparer.Compare(x,y);
            }
        }

        /// <summary>
        /// Prints a semicolon delimited header for this class, used for exporting
        /// </summary>
        /// <param name="sw">StreamWriter object to write to</param>
        public static void PrintHeader( StreamWriter sw )
        {
            sw.WriteLine("Name;Type;Owner;DefaultValue;ReadOnly;CurrentValue;SheetValid;");
        }

        /// <summary>
        /// Prints a semicolon delimited instance, used for exporting
        /// </summary>
        /// <param name="sw">StreamWriter object to write to</param>
        public void Print( StreamWriter sw )
        {
            sw.WriteLine("{0};{1};{2};{3};{4};{5};",Name,Type,Owner,DefaultValue,ReadOnly.ToString(),CurrentValue);
        }

        /// <summary>
        /// Prints Owner.Name to identify this item
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0}.{1}",_Owner, _Name);
        }
    }
}
