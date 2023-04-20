// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Markup;
using System.Xaml;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Test.Xaml.Types.Schema
{
    /// <summary>
    /// Type WithPublicConstructor
    /// </summary>
    public class TypeWithPublicConstructor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeWithPublicConstructor"/> class.
        /// </summary>
        public TypeWithPublicConstructor()
        {
        }
    }

    /// <summary>
    /// Type WithPrivateConstructor
    /// </summary>
    public class TypeWithPrivateConstructor
    {
        /// <summary>
        /// Prevents a default instance of the TypeWithPrivateConstructor class from being created.
        /// Initializes a new instance of the <see cref="TypeWithPrivateConstructor"/> class.
        /// </summary>
        private TypeWithPrivateConstructor()
        {
        }
    }

    /// <summary>
    /// Abstract Type
    /// </summary>
    public abstract class AbstractType
    {
    }

    /// <summary>
    /// Generic Type With TwoTypeArguments
    /// </summary>
    /// <typeparam name="TArg1">The type of the arg1.</typeparam>
    /// <typeparam name="TArg2">The type of the arg2.</typeparam>
    public class GenericTypeWithTwoTypeArguments<TArg1, TArg2>
    {
    }

    /// <summary>
    /// DerivedType That Partially Closes GenericType With TwoTypeArguments
    /// </summary>
    /// <typeparam name="TArg">The type of the arg.</typeparam>
    public class DerivedTypeThatPartiallyClosesGenericTypeWithTwoTypeArguments<TArg> : GenericTypeWithTwoTypeArguments<int, TArg>
    {
    }

    /// <summary>
    /// Type InheritingMarkupExtension
    /// </summary>
    public class TypeInheritingMarkupExtension : MarkupExtension
    {
        /// <summary>
        /// Provides the value.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns> null value</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return null;
        }
    }

    /// <summary>
    /// Type NotInheriting Or ImplementingAnything
    /// </summary>
    public class TypeNotInheritingOrImplementingAnything
    {
    }

    /// <summary>
    /// Type ImplementingINamescope
    /// </summary>
    public class TypeImplementingINamescope : INameScope
    {
        /// <summary>
        /// Finds the name.
        /// </summary>
        /// <param name="name">The name value.</param>
        /// <returns>null value</returns>
        public object FindName(string name)
        {
            return null;
        }

        /// <summary>
        /// Registers the name.
        /// </summary>
        /// <param name="name">The name value .</param>
        /// <param name="scopedElement">The scoped element.</param>
        public void RegisterName(string name, object scopedElement)
        {
        }

        /// <summary>
        /// Unregisters the name.
        /// </summary>
        /// <param name="name">The name value.</param>
        public void UnregisterName(string name)
        {
        }
    }

    /// <summary>
    /// Public Type
    /// </summary>
    public class PublicType
    {
    }

    /// <summary>
    /// Type Implementing IXmlSerializable
    /// </summary>
    public class TypeImplementingIXmlSerializable : IXmlSerializable
    {
        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
        /// </returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
        public void ReadXml(XmlReader reader)
        {
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        public void WriteXml(XmlWriter writer)
        {
        }
    }

    /// <summary>
    /// NonGeneric Type
    /// </summary>
    public class NonGenericType
    {
    }

    /// <summary>
    /// Generic Type
    /// </summary>
    /// <typeparam name="T">Generic Type value</typeparam>
    public class GenericType1<T>
    {
    }

    /// <summary>
    /// TypeWith TypeConverter
    /// </summary>
    [TypeConverter(typeof(TypeInheritingTypeConverter))]
    public class TypeWithTypeConverter
    {
    }

    /// <summary>
    /// Type Inheriting TypeConverter
    /// </summary>
    public class TypeInheritingTypeConverter : TypeConverter
    {
    }

    /// <summary>
    /// GenericType With OneTypeArgument
    /// </summary>
    /// <typeparam name="T">Generic Type</typeparam>
    public class GenericTypeWithOneTypeArgument<T>
    {
    }

    /// <summary>
    /// MarkupExtension With MarkupExtensionReturnType
    /// </summary>
    [MarkupExtensionReturnType(typeof(AnyType))]
    public class MarkupExtensionWithMarkupExtensionReturnType : MarkupExtension
    {
        /// <summary>
        /// Provides the value.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>null value</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return null;
        }
    }

    /// <summary>
    /// Type With OneContentProperty
    /// </summary>
    [ContentProperty("StringProperty")]
    public class TypeWithOneContentProperty
    {
        /// <summary>
        /// Gets or sets the string property.
        /// </summary>
        /// <value>The string property.</value>
        public string StringProperty { get; set; }

        /// <summary>
        /// Gets or sets the int property.
        /// </summary>
        /// <value>The int property.</value>
        public int IntProperty { get; set; }
    }

    /// <summary>
    /// ContentWrapper With StringContent
    /// </summary>
    [ContentProperty("StringProperty")]
    public class ContentWrapperWithStringContent
    {
        /// <summary>
        /// Gets or sets the string property.
        /// </summary>
        /// <value>The string property.</value>
        public string StringProperty { get; set; }
    }

    /// <summary>
    /// ContentWrapper With IntContent
    /// </summary>
    [ContentProperty("IntProperty")]
    public class ContentWrapperWithIntContent
    {
        /// <summary>
        /// Gets or sets the int property.
        /// </summary>
        /// <value>The int property.</value>
        public int IntProperty { get; set; }
    }

    /// <summary>
    /// CollectionType With TwoContentWrappers
    /// </summary>
    [ContentWrapper(typeof(ContentWrapperWithStringContent))]
    [ContentWrapper(typeof(ContentWrapperWithIntContent))]
    public class CollectionTypeWithTwoContentWrappers
    {
        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>IEnumerator value</returns>
        public IEnumerator GetEnumerator()
        {
            yield return null;
        }

        /// <summary>
        /// Adds the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Add(object element)
        {
        }
    }

    #region Collection Types 

    /// <summary>
    /// Type Implementing IEnumerableAndWithOneParameterAddMethod
    /// </summary>
    public class TypeImplementingIEnumerableAndWithOneParameterAddMethod : IEnumerable
    {
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return null;
        }

        /// <summary>
        /// Adds the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Add(string element)
        {
        }
    }

    /// <summary>
    /// Type Implementing IEnumerableAndICollectionOfString
    /// </summary>
    public class TypeImplementingIEnumerableAndICollectionOfString : IEnumerable, ICollection<string>
    {
        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <value></value>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</returns>
        int ICollection<string>.Count
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.</returns>
        bool ICollection<string>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return null;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            yield return null;
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        void ICollection<string>.Add(string item)
        {
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
        void ICollection<string>.Clear()
        {
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        bool ICollection<string>.Contains(string item)
        {
            return false;
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array"/> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex"/> is less than 0.</exception>
        void ICollection<string>.CopyTo(string[] array, int arrayIndex)
        {
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        bool ICollection<string>.Remove(string item)
        {
            return false;
        }
    }

    /// <summary>
    /// Type With GetEnumeratorAndOneParameterAddMethod
    /// </summary>
    public class TypeWithGetEnumeratorAndOneParameterAddMethod
    {
        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>IEnumerator value</returns>
        public IEnumerator GetEnumerator()
        {
            yield return null;
        }

        /// <summary>
        /// Adds the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Add(string element)
        {
        }
    }

    /// <summary>
    /// Type With GetEnumeratorAndAddObjectMethod
    /// </summary>
    public class TypeWithGetEnumeratorAndAddObjectMethod
    {
        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>IEnumerator value</returns>
        public IEnumerator GetEnumerator()
        {
            yield return null;
        }

        /// <summary>
        /// Adds the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Add(object element)
        {
        }
    }

    /// <summary>
    /// Type With GetEnumeratorAndImplementingIList
    /// </summary>
    public class TypeWithGetEnumeratorAndImplementingIList : IList
    {
        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> has a fixed size.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.IList"/> has a fixed size; otherwise, false.</returns>
        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.IList"/> is read-only; otherwise, false.</returns>
        bool IList.IsReadOnly
        {
            get
            {
                return false;
            }
        }
        
        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <value></value>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.ICollection"/>.</returns>
        int ICollection.Count
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
        /// </summary>
        /// <value></value>
        /// <returns>true if access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe); otherwise, false.</returns>
        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <value></value>
        /// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.</returns>
        object ICollection.SyncRoot
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> at the specified index.
        /// </summary>
        /// <param name="index">index value</param>
        /// <value></value>
        object IList.this[int index]
        {
            get
            {
                return null;
            }

            set
            {
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            yield return null;
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"/> to add to the <see cref="T:System.Collections.IList"/>.</param>
        /// <returns>
        /// The position into which the new element was inserted.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception>
        int IList.Add(object value)
        {
            return 0;
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only. </exception>
        void IList.Clear()
        {
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.IList"/> contains a specific value.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"/> to locate in the <see cref="T:System.Collections.IList"/>.</param>
        /// <returns>
        /// true if the <see cref="T:System.Object"/> is found in the <see cref="T:System.Collections.IList"/>; otherwise, false.
        /// </returns>
        bool IList.Contains(object value)
        {
            return false;
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"/> to locate in the <see cref="T:System.Collections.IList"/>.</param>
        /// <returns>
        /// The index of <paramref name="value"/> if found in the list; otherwise, -1.
        /// </returns>
        int IList.IndexOf(object value)
        {
            return 0;
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.IList"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="value"/> should be inserted.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to insert into the <see cref="T:System.Collections.IList"/>.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>. </exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception>
        /// <exception cref="T:System.NullReferenceException">
        /// <paramref name="value"/> is null reference in the <see cref="T:System.Collections.IList"/>.</exception>
        void IList.Insert(int index, object value)
        {
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"/> to remove from the <see cref="T:System.Collections.IList"/>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception>
        void IList.Remove(object value)
        {
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.IList"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>. </exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception>
        void IList.RemoveAt(int index)
        {
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array"/> is null. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero. </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="array"/> is multidimensional.-or- <paramref name="index"/> is equal to or greater than the length of <paramref name="array"/>.-or- The number of elements in the source <see cref="T:System.Collections.ICollection"/> is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>. </exception>
        /// <exception cref="T:System.ArgumentException">The type of the source <see cref="T:System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <paramref name="array"/>. </exception>
        void ICollection.CopyTo(Array array, int index)
        {
        }
    }

    /// <summary>
    /// Type With GetEnumeratorAndImplementingICollectionOfString
    /// </summary>
    public class TypeWithGetEnumeratorAndImplementingICollectionOfString : ICollection<string>
    {
        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <value></value>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</returns>
        int ICollection<string>.Count
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.</returns>
        bool ICollection<string>.IsReadOnly
        {
            get
            {
                return false;
            }
        }
        
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            yield return null;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            yield return null;
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        void ICollection<string>.Add(string item)
        {
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
        void ICollection<string>.Clear()
        {
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        bool ICollection<string>.Contains(string item)
        {
            return false;
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array"/> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">.</exception>
        void ICollection<string>.CopyTo(string[] array, int arrayIndex)
        {
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        bool ICollection<string>.Remove(string item)
        {
            return false;
        }
    }

    /// <summary>
    /// Type With GetEnumeratorAndTwoOneParameterAddMethodsAndAddObjectMethod
    /// </summary>
    public class TypeWithGetEnumeratorAndTwoOneParameterAddMethodsAndAddObjectMethod
    {
        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>null value</returns>
        public IEnumerator GetEnumerator()
        {
            yield return null;
        }

        /// <summary>
        /// Adds the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Add(string element)
        {
        }

        /// <summary>
        /// Adds the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Add(int element)
        {
        }

        /// <summary>
        /// Adds the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Add(object element)
        {
        }
    }

    /// <summary>
    /// AmbiguousCollectionInterface class
    /// </summary>
    public class TypeWithAmbiguousCollectionInterface : ICollection<string>, ICollection<int>
    {
        #region ICollection<int> Properties

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        int ICollection<int>.Count
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        bool ICollection<int>.IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region ICollection<string> Properties

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        int ICollection<string>.Count
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        bool ICollection<string>.IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region ICollection<string> Members

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        void ICollection<string>.Add(string item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        void ICollection<string>.Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        bool ICollection<string>.Contains(string item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        void ICollection<string>.CopyTo(string[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        bool ICollection<string>.Remove(string item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<string> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICollection<int> Members

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        void ICollection<int>.Add(int item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        void ICollection<int>.Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        bool ICollection<int>.Contains(int item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        void ICollection<int>.CopyTo(int[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        bool ICollection<int>.Remove(int item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<int> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<int> IEnumerable<int>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Collection Type With TwoParameterAdd
    /// </summary>
    public class CollectionTypeWithTwoParameterAdd : List<string>
    {
        /// <summary>
        /// Adds the specified s.
        /// </summary>
        /// <param name="s">The string value.</param>
        /// <param name="y">The int value.</param>
        public void Add(string s, int y)
        {
        }
    }

    #endregion

    #region Dictionary Types

    /// <summary>
    /// Dictionary with AmbiguousInterface
    /// </summary>
    public class TypeWithAmbiguousDictionaryInterface : IDictionary<string, string>, IDictionary<int, int>
    {
        #region IDictionary<string,string> Properties

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        ICollection<string> IDictionary<string, string>.Keys
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        ICollection<string> IDictionary<string, string>.Values
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region ICollection<KeyValuePair<string,string>> Properties

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        int ICollection<KeyValuePair<string, string>>.Count
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        bool ICollection<KeyValuePair<string, string>>.IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region ICollection<KeyValuePair<int,int>> Properties

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        int ICollection<KeyValuePair<int, int>>.Count
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        bool ICollection<KeyValuePair<int, int>>.IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IDictionary<int,int> Properties

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        ICollection<int> IDictionary<int, int>.Keys
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        ICollection<int> IDictionary<int, int>.Values
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Indexers

        /// <summary>
        /// Gets or sets the <see cref="System.String"/> with the specified key.
        /// </summary>
        /// <value></value>
        /// <param name="key">key value.</param>
        string IDictionary<string, string>.this[string key]
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Int32"/> with the specified key.
        /// </summary>
        /// <value></value>
        /// <param name="key">int value.</param>
        int IDictionary<int, int>.this[int key]
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region IDictionary<string,string> Members

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        void IDictionary<string, string>.Add(string key, string value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</param>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the key; otherwise, false.
        /// </returns>
        bool IDictionary<string, string>.ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        bool IDictionary<string, string>.Remove(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
        /// <returns>
        /// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        bool IDictionary<string, string>.TryGetValue(string key, out string value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICollection<KeyValuePair<string,string>> Members

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        void ICollection<KeyValuePair<string, string>>.Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,string>> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDictionary<int,int> Members

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        void IDictionary<int, int>.Add(int key, int value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</param>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the key; otherwise, false.
        /// </returns>
        bool IDictionary<int, int>.ContainsKey(int key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        bool IDictionary<int, int>.Remove(int key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
        /// <returns>
        /// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        bool IDictionary<int, int>.TryGetValue(int key, out int value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICollection<KeyValuePair<int,int>> Members

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        void ICollection<KeyValuePair<int, int>>.Add(KeyValuePair<int, int> item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        void ICollection<KeyValuePair<int, int>>.Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        bool ICollection<KeyValuePair<int, int>>.Contains(KeyValuePair<int, int> item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        void ICollection<KeyValuePair<int, int>>.CopyTo(KeyValuePair<int, int>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        bool ICollection<KeyValuePair<int, int>>.Remove(KeyValuePair<int, int> item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<KeyValuePair<int,int>> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<KeyValuePair<int, int>> IEnumerable<KeyValuePair<int, int>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Type Implementing IEnumerableAndWithTwoParameterAddMethod
    /// </summary>
    public class TypeImplementingIEnumerableAndWithTwoParameterAddMethod : IEnumerable
    {
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return null;
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key value.</param>
        /// <param name="item">The item value.</param>
        public void Add(int key, string item)
        {
        }
    }

    /// <summary>
    /// Type Implementing IEnumerableAndIDictionaryOfIntString
    /// </summary>
    public class TypeImplementingIEnumerableAndIDictionaryOfIntString : IEnumerable, IDictionary<int, string>
    {
        #region Properties

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <value></value>
        /// <returns>An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.</returns>
        ICollection<string> IDictionary<int, string>.Values
        {
            get
            {
                return null;
            }
        }
        
        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <value></value>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</returns>
        int ICollection<KeyValuePair<int, string>>.Count
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <value></value>
        /// <returns>An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.</returns>
        ICollection<int> IDictionary<int, string>.Keys
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.</returns>
        bool ICollection<KeyValuePair<int, string>>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.String"/> with the specified key.
        /// </summary>
        /// <param name="key">index value</param>
        /// <value></value>
        string IDictionary<int, string>.this[int key]
        {
            get
            {
                return null;
            }

            set
            {
            }
        }

        #endregion

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return null;
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key"/> is null.</exception>
        /// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
        void IDictionary<int, string>.Add(int key, string value)
        {
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</param>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the key; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key"/> is null.</exception>
        bool IDictionary<int, string>.ContainsKey(int key)
        {
            return false;
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key"/> is null.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
        bool IDictionary<int, string>.Remove(int key)
        {
            return false;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
        /// <returns>
        /// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key"/> is null.</exception>
        bool IDictionary<int, string>.TryGetValue(int key, out string value)
        {
            value = null;
            return false;
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        void ICollection<KeyValuePair<int, string>>.Add(KeyValuePair<int, string> item)
        {
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
        void ICollection<KeyValuePair<int, string>>.Clear()
        {
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        bool ICollection<KeyValuePair<int, string>>.Contains(KeyValuePair<int, string> item)
        {
            return false;
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array"/> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">.</exception>
        void ICollection<KeyValuePair<int, string>>.CopyTo(KeyValuePair<int, string>[] array, int arrayIndex)
        {
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        bool ICollection<KeyValuePair<int, string>>.Remove(KeyValuePair<int, string> item)
        {
            return false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<KeyValuePair<int, string>> IEnumerable<KeyValuePair<int, string>>.GetEnumerator()
        {
            yield return new KeyValuePair<int, string>(0, String.Empty);
        }
    }

    /// <summary>
    /// Type With GetEnumeratorAndTwoParameterAddMethod
    /// </summary>
    public class TypeWithGetEnumeratorAndTwoParameterAddMethod
    {
        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns> Ienumerator value </returns>
        public IEnumerator GetEnumerator()
        {
            yield return null;
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key value.</param>
        /// <param name="item">The item value.</param>
        public void Add(int key, string item)
        {
        }
    }

    /// <summary>
    /// Type With GetEnumeratorAndAddObjectObjectMethod
    /// </summary>
    public class TypeWithGetEnumeratorAndAddObjectObjectMethod
    {
        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>null value</returns>
        public IEnumerator GetEnumerator()
        {
            yield return null;
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key value.</param>
        /// <param name="item">The item value.</param>
        public void Add(object key, object item)
        {
        }
    }

    /// <summary>
    /// Type With GetEnumeratorAndImplementingIDictionary
    /// </summary>
    public class TypeWithGetEnumeratorAndImplementingIDictionary : IDictionary
    {
        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IDictionary"/> object has a fixed size.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.IDictionary"/> object has a fixed size; otherwise, false.</returns>
        bool IDictionary.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IDictionary"/> object is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.IDictionary"/> object is read-only; otherwise, false.</returns>
        bool IDictionary.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.ICollection"/> object containing the keys of the <see cref="T:System.Collections.IDictionary"/> object.
        /// </summary>
        /// <value></value>
        /// <returns>An <see cref="T:System.Collections.ICollection"/> object containing the keys of the <see cref="T:System.Collections.IDictionary"/> object.</returns>
        ICollection IDictionary.Keys
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.ICollection"/> object containing the values in the <see cref="T:System.Collections.IDictionary"/> object.
        /// </summary>
        /// <value></value>
        /// <returns>An <see cref="T:System.Collections.ICollection"/> object containing the values in the <see cref="T:System.Collections.IDictionary"/> object.</returns>
        ICollection IDictionary.Values
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <value></value>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.ICollection"/>.</returns>
        int ICollection.Count
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
        /// </summary>
        /// <value></value>
        /// <returns>true if access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe); otherwise, false.</returns>
        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <value></value>
        /// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.</returns>
        object ICollection.SyncRoot
        {
            get
            {
                return null;
            }
        }
        
        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <param name="key">index value</param>
        /// <value></value>
        object IDictionary.this[object key]
        {
            get
            {
                return null;
            }

            set
            {
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            yield return null;
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.IDictionary"/> object.
        /// </summary>
        /// <param name="key">The <see cref="T:System.Object"/> to use as the key of the element to add.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to use as the value of the element to add.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key"/> is null. </exception>
        /// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.IDictionary"/> object. </exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IDictionary"/> is read-only.-or- The <see cref="T:System.Collections.IDictionary"/> has a fixed size. </exception>
        void IDictionary.Add(object key, object value)
        {
        }

        /// <summary>
        /// Removes all elements from the <see cref="T:System.Collections.IDictionary"/> object.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IDictionary"/> object is read-only. </exception>
        void IDictionary.Clear()
        {
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.IDictionary"/> object contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.IDictionary"/> object.</param>
        /// <returns>
        /// true if the <see cref="T:System.Collections.IDictionary"/> contains an element with the key; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key"/> is null. </exception>
        bool IDictionary.Contains(object key)
        {
            return false;
        }

        /// <summary>
        /// Returns an <see cref="T:System.Collections.IDictionaryEnumerator"/> object for the <see cref="T:System.Collections.IDictionary"/> object.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IDictionaryEnumerator"/> object for the <see cref="T:System.Collections.IDictionary"/> object.
        /// </returns>
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return null;
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.IDictionary"/> object.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key"/> is null. </exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IDictionary"/> object is read-only.-or- The <see cref="T:System.Collections.IDictionary"/> has a fixed size. </exception>
        void IDictionary.Remove(object key)
        {
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array"/> is null. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero. </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="array"/> is multidimensional.-or- <paramref name="index"/> is equal to or greater than the length of <paramref name="array"/>.-or- The number of elements in the source <see cref="T:System.Collections.ICollection"/> is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>. </exception>
        /// <exception cref="T:System.ArgumentException">The type of the source <see cref="T:System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <paramref name="array"/>. </exception>
        void ICollection.CopyTo(Array array, int index)
        {
        }
    }

    /// <summary>
    /// Type With GetEnumeratorAndImplementingIDictionaryOfIntString
    /// </summary>
    public class TypeWithGetEnumeratorAndImplementingIDictionaryOfIntString : IDictionary<int, string>
    {
        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <value></value>
        /// <returns>An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.</returns>
        ICollection<int> IDictionary<int, string>.Keys
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <value></value>
        /// <returns>An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.</returns>
        ICollection<string> IDictionary<int, string>.Values
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <value></value>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</returns>
        int ICollection<KeyValuePair<int, string>>.Count
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.</returns>
        bool ICollection<KeyValuePair<int, string>>.IsReadOnly
        {
            get
            {
                return false;
            }
        }
        
        /// <summary>
        /// Gets or sets the <see cref="System.String"/> with the specified key.
        /// </summary>
        /// <param name="key">index value</param>
        /// <value></value>
        string IDictionary<int, string>.this[int key]
        {
            get
            {
                return null;
            }

            set
            {
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            yield return null;
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key"/> is null.</exception>
        /// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
        void IDictionary<int, string>.Add(int key, string value)
        {
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</param>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the key; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key"/> is null.</exception>
        bool IDictionary<int, string>.ContainsKey(int key)
        {
            return false;
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key"/> is null.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
        bool IDictionary<int, string>.Remove(int key)
        {
            return false;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
        /// <returns>
        /// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key"/> is null.</exception>
        bool IDictionary<int, string>.TryGetValue(int key, out string value)
        {
            value = null;
            return false;
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        void ICollection<KeyValuePair<int, string>>.Add(KeyValuePair<int, string> item)
        {
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
        void ICollection<KeyValuePair<int, string>>.Clear()
        {
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        bool ICollection<KeyValuePair<int, string>>.Contains(KeyValuePair<int, string> item)
        {
            return false;
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array"/> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex"/> is less than 0.</exception>
        void ICollection<KeyValuePair<int, string>>.CopyTo(KeyValuePair<int, string>[] array, int arrayIndex)
        {
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        bool ICollection<KeyValuePair<int, string>>.Remove(KeyValuePair<int, string> item)
        {
            return false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<KeyValuePair<int, string>> IEnumerable<KeyValuePair<int, string>>.GetEnumerator()
        {
            yield return new KeyValuePair<int, string>(0, String.Empty);
        }
    }

    /// <summary>
    /// Type With GetEnumeratorAndTwoTwoParameterAddMethodsAndAddObjectObjectMethod
    /// </summary>
    public class TypeWithGetEnumeratorAndTwoTwoParameterAddMethodsAndAddObjectObjectMethod
    {
        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>null value</returns>
        public IEnumerator GetEnumerator()
        {
            yield return null;
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key value.</param>
        /// <param name="item">The item value.</param>
        public void Add(int key, string item)
        {
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key value.</param>
        /// <param name="item">The item value.</param>
        public void Add(string key, int item)
        {
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key value.</param>
        /// <param name="item">The item value.</param>
        public void Add(object key, object item)
        {
        }
    }

    #endregion

    /// <summary>
    /// Type With PublicConstructorWithThreeParameters
    /// </summary>
    public class TypeWithPublicConstructorWithThreeParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeWithPublicConstructorWithThreeParameters"/> class.
        /// </summary>
        /// <param name="parameterOne">The parameter one.</param>
        /// <param name="parameterTwo">The parameter two.</param>
        /// <param name="parameterThree">if set to <c>true</c> [parameter three].</param>
        public TypeWithPublicConstructorWithThreeParameters(string parameterOne, int parameterTwo, bool parameterThree)
        {
        }
    }

    /// <summary>
    /// AnyType class
    /// </summary>
    public class AnyType
    {
    }

    /// <summary>
    /// Type UnrelatedToAnyType
    /// </summary>
    public class TypeUnrelatedToAnyType
    {
    }

    /// <summary>
    /// Type With PropertiesAndEvents
    /// </summary>
    public class TypeWithPropertiesAndEvents
    {
        /// Events
        /// <summary>
        /// Occurs when [public event1].
        /// </summary>
        public event EventHandler PublicEvent1
        {
            add
            {
            }

            remove
            {
            }
        }

        /// <summary>
        /// Occurs when [public event2].
        /// </summary>
        public event EventHandler PublicEvent2
        {
            add
            {
            }

            remove
            {
            }
        }

        /// <summary>
        /// Occurs when [internal event1].
        /// </summary>
        internal event EventHandler InternalEvent1
        {
            add
            {
            }

            remove
            {
            }
        }

        /// <summary>
        /// Occurs when [internal event2].
        /// </summary>
        internal event EventHandler InternalEvent2
        {
            add
            {
            }

            remove
            {
            }
        }

        /// <summary>
        /// Occurs when [private event1].
        /// </summary>
        private event EventHandler PrivateEvent1
        {
            add
            {
            }

            remove
            {
            }
        }

        /// <summary>
        /// Occurs when [private event2].
        /// </summary>
        private event EventHandler PrivateEvent2
        {
            add
            {
            }

            remove
            {
            }
        }

        /// Static properties
        /// <summary>
        /// Gets or sets the public static string property.
        /// </summary>
        /// <value>The public static string property.</value>
        public static string PublicStaticStringProperty { get; set; }

        /// <summary>
        /// Gets or sets the public static int property.
        /// </summary>
        /// <value>The public static int property.</value>
        public static int PublicStaticIntProperty { get; set; }

        /// <summary>
        /// Gets or sets the public string property.
        /// </summary>
        /// <value>The public string property.</value>
        public string PublicStringProperty { get; set; }

        /// <summary>
        /// Gets or sets the public int property.
        /// </summary>
        /// <value>The public int property.</value>
        public int PublicIntProperty { get; set; }

        /// <summary>
        /// Gets or sets the internal static string property.
        /// </summary>
        /// <value>The internal static string property.</value>
        internal static string InternalStaticStringProperty { get; set; }

        /// <summary>
        /// Gets or sets the internal static int property.
        /// </summary>
        /// <value>The internal static int property.</value>
        internal static int InternalStaticIntProperty { get; set; }

        /// <summary>
        /// Gets or sets the internal string property.
        /// </summary>
        /// <value>The internal string property.</value>
        internal string InternalStringProperty { get; set; }

        /// <summary>
        /// Gets or sets the internal int property.
        /// </summary>
        /// <value>The internal int property.</value>
        internal int InternalIntProperty { get; set; }

        /// <summary>
        /// Gets or sets the private static string property.
        /// </summary>
        /// <value>The private static string property.</value>
        private static string PrivateStaticStringProperty { get; set; }

        /// <summary>
        /// Gets or sets the private static int property.
        /// </summary>
        /// <value>The private static int property.</value>
        private static int PrivateStaticIntProperty { get; set; }

        /// <summary>
        /// Gets or sets the private string property.
        /// </summary>
        /// <value>The private string property.</value>
        private string PrivateStringProperty { get; set; }

        /// <summary>
        /// Gets or sets the private int property.
        /// </summary>
        /// <value>The private int property.</value>
        private int PrivateIntProperty { get; set; }
        
        /// Attachable properties
        /// <summary>
        /// Gets the public attachable string property.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>null value</returns>
        public static string GetPublicAttachableStringProperty(object target)
        {
            return null;
        }

        /// <summary>
        /// Sets the public attachable string property.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="prop">The property value</param>
        public static void SetPublicAttachableStringProperty(object target, string prop)
        {
        }

        /// <summary>
        /// Gets the public attachable int property.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>null value</returns>
        public static int GetPublicAttachableIntProperty(object target)
        {
            return 0;
        }

        /// <summary>
        /// Sets the public attachable int property.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="prop">The property value</param>
        public static void SetPublicAttachableIntProperty(object target, int prop)
        {
        }

        /// Attachable events
        /// <summary>
        /// Adds the public attachable event1 handler.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="handler">The handler.</param>
        public static void AddPublicAttachableEvent1Handler(object target, EventHandler handler)
        {
        }

        /// <summary>
        /// Removes the public attachable event1 handler.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="handler">The handler.</param>
        public static void RemovePublicAttachableEvent1Handler(object target, EventHandler handler)
        {
        }

        /// <summary>
        /// Adds the public attachable event2 handler.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="handler">The handler.</param>
        public static void AddPublicAttachableEvent2Handler(object target, EventHandler handler)
        {
        }

        /// <summary>
        /// Removes the public attachable event2 handler.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="handler">The handler.</param>
        public static void RemovePublicAttachableEvent2Handler(object target, EventHandler handler)
        {
        }

        #region Private Methods

        /// <summary>
        /// Gets the internal attachable string property.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>null value</returns>
        internal static string GetInternalAttachableStringProperty(object target)
        {
            return null;
        }

        /// <summary>
        /// Sets the internal attachable string property.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="prop">The property value</param>
        internal static void SetInternalAttachableStringProperty(object target, string prop)
        {
        }

        /// <summary>
        /// Gets the internal attachable int property.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>null value</returns>
        internal static int GetInternalAttachableIntProperty(object target)
        {
            return 0;
        }

        /// <summary>
        /// Sets the internal attachable int property.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="prop">The property value</param>
        internal static void SetInternalAttachableIntProperty(object target, int prop)
        {
        }

        /// <summary>
        /// Adds the internal attachable event1 handler.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="handler">The handler.</param>
        internal static void AddInternalAttachableEvent1Handler(object target, EventHandler handler)
        {
        }

        /// <summary>
        /// Removes the internal attachable event1 handler.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="handler">The handler.</param>
        internal static void RemoveInternalAttachableEvent1Handler(object target, EventHandler handler)
        {
        }

        /// <summary>
        /// Adds the internal attachable event2 handler.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="handler">The handler.</param>
        internal static void AddInternalAttachableEvent2Handler(object target, EventHandler handler)
        {
        }

        /// <summary>
        /// Removes the internal attachable event2 handler.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="handler">The handler.</param>
        internal static void RemoveInternalAttachableEvent2Handler(object target, EventHandler handler)
        {
        }

        /// <summary>
        /// Gets the private attachable string property.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>null value</returns>
        private static string GetPrivateAttachableStringProperty(object target)
        {
            return null;
        }

        /// <summary>
        /// Sets the private attachable string property.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="prop">The property value</param>
        private static void SetPrivateAttachableStringProperty(object target, string prop)
        {
        }

        /// <summary>
        /// Gets the private attachable int property.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>null value</returns>
        private static int GetPrivateAttachableIntProperty(object target)
        {
            return 0;
        }

        /// <summary>
        /// Sets the private attachable int property.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="prop">The property value</param>
        private static void SetPrivateAttachableIntProperty(object target, int prop)
        {
        }

        /// <summary>
        /// Adds the private attachable event1 handler.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="handler">The handler.</param>
        private static void AddPrivateAttachableEvent1Handler(object target, EventHandler handler)
        {
        }

        /// <summary>
        /// Removes the private attachable event1 handler.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="handler">The handler.</param>
        private static void RemovePrivateAttachableEvent1Handler(object target, EventHandler handler)
        {
        }

        /// <summary>
        /// Adds the private attachable event2 handler.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="handler">The handler.</param>
        private static void AddPrivateAttachableEvent2Handler(object target, EventHandler handler)
        {
        }

        /// <summary>
        /// Removes the private attachable event2 handler.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="handler">The handler.</param>
        private static void RemovePrivateAttachableEvent2Handler(object target, EventHandler handler)
        {
        }

        #endregion
    }

    /// <summary>
    /// Type With DictionaryKeyProperty
    /// </summary>
    [DictionaryKeyProperty("KeyProperty")]
    public class TypeWithDictionaryKeyProperty
    {
        /// <summary>
        /// Gets or sets the key property.
        /// </summary>
        /// <value>The key property.</value>
        public string KeyProperty { get; set; }

        /// <summary>
        /// Gets or sets the item property.
        /// </summary>
        /// <value>The item property.</value>
        public int ItemProperty { get; set; }
    }

    /// <summary>
    /// Type With RuntimeNameProperty
    /// </summary>
    [RuntimeNameProperty("NameProperty")]
    public class TypeWithRuntimeNameProperty
    {
        /// <summary>
        /// Gets or sets the name property.
        /// </summary>
        /// <value>The name property.</value>
        public string NameProperty { get; set; }

        /// <summary>
        /// Gets or sets The data value
        /// </summary>
        /// <value>The data value</value>
        public int Data { get; set; }
    }

    /// <summary>
    /// Type With UidProperty
    /// </summary>
    [UidProperty("UidProperty")]
    public class TypeWithUidProperty
    {
        /// <summary>
        /// Gets or sets the uid property.
        /// </summary>
        /// <value>The uid property.</value>
        public string UidProperty { get; set; }

        /// <summary>
        /// Gets or sets The data value
        /// </summary>
        /// <value>The data value</value>
        public int Data { get; set; }
    }

    /// <summary>
    /// Type With XmlLangProperty
    /// </summary>
    [XmlLangProperty("XmlLangProperty")]
    public class TypeWithXmlLangProperty
    {
        /// <summary>
        /// Gets or sets the XML lang property.
        /// </summary>
        /// <value>The XML lang property.</value>
        public string XmlLangProperty { get; set; }

        /// <summary>
        /// Gets or sets The data value
        /// </summary>
        /// <value>The data value</value>
        public int Data { get; set; }
    }

    /// <summary>
    /// Converter Type
    /// </summary>
    public class ConverterType : XamlDeferringLoader
    {
        /// <summary>
        /// XamlDeferringLoader.Load implementaion
        /// </summary>
        /// <param name="xamlReader">xaml reder</param>
        /// <param name="serviceProvider">service provider</param>
        /// <returns>loaded object</returns>
        public override object Load(System.Xaml.XamlReader xamlReader, IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Save object to XamlReader
        /// </summary>
        /// <param name="value">object to save</param>
        /// <param name="serviceProvider">service provider</param>
        /// <returns>xamlreader instance</returns>
        public override System.Xaml.XamlReader Save(object value, IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Content Type
    /// </summary>
    public class ContentType
    {
    }

    /// <summary>
    /// Type With TemplateConverter
    /// </summary>
    [XamlDeferLoad(typeof(ConverterType), typeof(ContentType))]
    public class TypeWithTemplateConverter
    {
    }

    /// <summary>
    /// Type With NoAttributes
    /// </summary>
    public class TypeWithNoAttributes
    {
    }
}
