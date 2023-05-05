// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Test.Xaml.Types
{
    #region class Custom_Clr

    /// <summary>
    /// Custom CLR object
    /// </summary>
    public class Custom_Clr
    {
    }

    #endregion class Custom_Clr

    #region class Custom_Clr_With_Properties

    /// <summary>
    /// Custom CLR class with various properties.
    /// </summary>
    public class Custom_Clr_With_Properties
    {
        #region Private Data

        /// <summary>
        /// Array_ClrProp RO
        /// </summary>
        private readonly Button[] _fieldforArrayClrPropRO = null;

        /// <summary> Singular_ClrProp RO </summary>
        private Button _fieldforSingularClrPropRO = null;

        /// <summary>
        /// IList_ClrProp_RW NonNull
        /// </summary>
        private ArrayList _fieldforIListClrPropRWNonNull = null;

        /// <summary>
        /// IList_ClrProp_RO NonNull
        /// </summary>
        private ArrayList _fieldforIListClrPropRONonNull = null;

        /// <summary>
        /// IList_ClrProp_RO Null
        /// </summary>
        private ArrayList _fieldforIListClrPropRONull = null;

        /// <summary>
        /// IDictionary_ClrProp_RW NonNull
        /// </summary>
        private Hashtable _fieldforIDictionaryClrPropRWNonNull = null;

        /// <summary>
        /// IDictionary_ClrProp_RO NonNull
        /// </summary>
        private Hashtable _fieldforIDictionaryClrPropRONonNull = null;

        /// <summary>
        /// IDictionary_ClrProp_RO Null
        /// </summary>
        private Hashtable _fieldforIDictionaryClrPropRONull = null;

        #endregion 

        #region Properties

        /// <summary>
        /// Gets the Singular_ClrProp_RO
        /// </summary>
        public Button Singular_ClrProp_RO
        {
            get
            {
                return _fieldforSingularClrPropRO;
            }
        }

        /// <summary>
        /// Gets or sets singular_ CLR prop_ RW.
        /// </summary>
        /// <value>The singular_ CLR prop_ RW.</value>
        public Button Singular_ClrProp_RW { get; set; }

        /// <summary>
        /// Gets or sets the array_ CLR prop_ RW.
        /// </summary>
        /// <value>The array_ CLR prop_ RW.</value>
        public Button[] Array_ClrProp_RW { get; set; }

        /// <summary>
        /// Gets the Content property value
        /// </summary>
        public Button[] Array_ClrProp_RO
        {
            get
            {
                return _fieldforArrayClrPropRO;
            }
        }

        /// <summary>
        /// Gets or sets the IList_ClrProp_RW_NonNull.
        /// </summary>
        /// <value>The IList_ClrProp_RW_NonNull.</value>
        public ArrayList IList_ClrProp_RW_NonNull
        {
            get
            {
                if (_fieldforIListClrPropRWNonNull == null)
                {
                    _fieldforIListClrPropRWNonNull = new ArrayList();
                }

                return _fieldforIListClrPropRWNonNull;
            }

            set
            {
                _fieldforIListClrPropRWNonNull = value;
            }
        }

        /// <summary>
        /// Gets the I list_ CLR prop_ Ro_ non null.
        /// </summary>
        /// <value>The I list_ CLR prop_ R o_ non null.</value>
        public ArrayList IList_ClrProp_RO_NonNull
        {
            get
            {
                if (_fieldforIListClrPropRONonNull == null)
                {
                    _fieldforIListClrPropRONonNull = new ArrayList();
                }

                return _fieldforIListClrPropRONonNull;
            }
        }

        /// <summary>
        /// Gets or sets the I list_ CLR prop_ Rw_ null.
        /// </summary>
        /// <value>The I list_ CLR prop_ R w_ null.</value>
        public ArrayList IList_ClrProp_RW_Null { get; set; }

        /// <summary>
        /// Gets the I list_ CLR prop_ Ro_ null.
        /// </summary>
        /// <value>The I list_ CLR prop_ Ro_ null.</value>
        public ArrayList IList_ClrProp_RO_Null
        {
            get
            {
                return _fieldforIListClrPropRONull;
            }
        }

        /// <summary>
        /// Gets or sets the I dictionary_ CLR prop_ Rw_ non null.
        /// </summary>
        /// <value>The I dictionary_ CLR prop_ RW_ non null.</value>
        public Hashtable IDictionary_ClrProp_RW_NonNull
        {
            get
            {
                if (_fieldforIDictionaryClrPropRWNonNull == null)
                {
                    _fieldforIDictionaryClrPropRWNonNull = new Hashtable();
                }

                return _fieldforIDictionaryClrPropRWNonNull;
            }

            set
            {
                _fieldforIDictionaryClrPropRWNonNull = value;
            }
        }

        /// <summary>
        /// Gets the I dictionary_ CLR prop_ Ro_ non null.
        /// </summary>
        /// <value>The I dictionary_ CLR prop_ Ro_ non null.</value>
        public Hashtable IDictionary_ClrProp_RO_NonNull
        {
            get
            {
                if (_fieldforIDictionaryClrPropRONonNull == null)
                {
                    _fieldforIDictionaryClrPropRONonNull = new Hashtable();
                }

                return _fieldforIDictionaryClrPropRONonNull;
            }
        }

        /// <summary>
        /// Gets or sets the I dictionary_ CLR prop_ Rw_ null.
        /// </summary>
        /// <value>The I dictionary_ CLR prop_ Rw_ null.</value>
        public Hashtable IDictionary_ClrProp_RW_Null { get; set; }

        /// <summary>
        /// Gets the I dictionary_ CLR prop_ Ro_ null.
        /// </summary>
        /// <value>The I dictionary_ CLR prop_ Ro_ null.</value>
        public Hashtable IDictionary_ClrProp_RO_Null
        {
            get
            {
                return _fieldforIDictionaryClrPropRONull;
            }
        }

        #endregion 
    }

    #endregion class Custom_Clr_With_Properties

    #region class Custom_Clr_With_IAddChild

    /// <summary>
    /// Custom CLR with IAddChild
    /// </summary>
    public class Custom_Clr_With_IAddChild : IAddChild
    {
        /// <summary>
        /// List of child objects
        /// </summary>
        private readonly ArrayList _children;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_Clr_With_IAddChild"/> class.
        /// </summary>
        public Custom_Clr_With_IAddChild()
        {
            _children = new ArrayList();
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public ArrayList Children
        {
            get
            {
                return ArrayList.ReadOnly(_children);
            }
        }

        /// <summary>
        /// Adds the text.
        /// </summary>
        /// <param name="text">The text value.</param>
        void IAddChild.AddText(string text)
        {
            _children.Add(text);
        }

        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <param name="o">The object to be added.</param>
        void IAddChild.AddChild(object o)
        {
            _children.Add(o);
        }
    }

    #endregion class Custom_Clr_With_IAddChild

    #region class Custom_Clr_With_IList

    /// <summary>
    /// Custom CLR with IList. 
    /// </summary>
    public class Custom_Clr_With_IList : IList
    {
        /// <summary>
        /// List of Child objects
        /// </summary>
        private readonly ArrayList _children;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_Clr_With_IList"/> class.
        /// </summary>
        public Custom_Clr_With_IList()
        {
            _children = new ArrayList();
        }

        #region Properties

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public ArrayList Children
        {
            get
            {
                return ArrayList.ReadOnly(_children);
            }
        }

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
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
        /// </summary>
        /// <value></value>
        /// <returns>true if access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe); otherwise, false.</returns>
        bool ICollection.IsSynchronized
        {
            get
            {
                return _children.IsSynchronized;
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
                return _children.SyncRoot;
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
                return _children.Count;
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified item index.
        /// </summary>
        /// <param name="itemIndex">int index. </param>
        /// <value>int index value. </value>
        object IList.this[int itemIndex]
        {
            get
            {
                return _children[itemIndex];
            }

            set
            {
                _children[itemIndex] = value;
            }
        }

        /// <summary>
        /// Adds the specified val.
        /// </summary>
        /// <param name="val">The object value.</param>
        /// <returns>int value.</returns>
        int IList.Add(object val)
        {
            return _children.Add(val);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only. </exception>
        void IList.Clear()
        {
            _children.Clear();
        }

        /// <summary>
        /// Determines whether [contains] [the specified val].
        /// </summary>
        /// <param name="val">The object value.</param>
        /// <returns>
        /// <c>true</c> if [contains] [the specified val]; otherwise, <c>false</c>.
        /// </returns>
        bool IList.Contains(object val)
        {
            return _children.Contains(val);
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="indexItem">The index item.</param>
        /// <returns>int value. </returns>
        int IList.IndexOf(object indexItem)
        {
            return _children.IndexOf(indexItem);
        }

        /// <summary>
        /// Inserts the specified insert index.
        /// </summary>
        /// <param name="insertIndex">Index of the insert.</param>
        /// <param name="insertItem">The insert item.</param>
        void IList.Insert(int insertIndex, object insertItem)
        {
            _children.Insert(insertIndex, insertItem);
        }

        /// <summary>
        /// Removes the specified remove item.
        /// </summary>
        /// <param name="removeItem">The remove item.</param>
        void IList.Remove(object removeItem)
        {
            _children.Remove(removeItem);
        }

        /// <summary>
        /// Removes at.
        /// </summary>
        /// <param name="removeIndex">Index of the remove.</param>
        void IList.RemoveAt(int removeIndex)
        {
            _children.RemoveAt(removeIndex);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="destinationArray">The destination array.</param>
        /// <param name="destinationStart">The destination start.</param>
        void ICollection.CopyTo(Array destinationArray, int destinationStart)
        {
            _children.CopyTo(destinationArray, destinationStart);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _children.GetEnumerator();
        }
    }

    #endregion class Custom_Clr_With_IList

    #region class Custom_Clr_With_IDictionary

    /// <summary>
    /// Custom Clr with IDictionary
    /// </summary>
    public class Custom_Clr_With_IDictionary : IDictionary
    {
        /// <summary>
        /// Hashtable of children
        /// </summary>
        private readonly Hashtable _children;

        /// <summary>
        /// List of child nodes
        /// </summary>
        private readonly ArrayList _childrenList;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_Clr_With_IDictionary"/> class.
        /// </summary>
        public Custom_Clr_With_IDictionary()
        {
            _children = new Hashtable();
            _childrenList = new ArrayList();
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IDictionary"/> object has a fixed size.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.IDictionary"/> object has a fixed size; otherwise, false.</returns>
        bool IDictionary.IsFixedSize
        {
            get
            {
                return _children.IsFixedSize;
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
                return _children.IsReadOnly;
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
                return _children.Keys;
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
                return _children.Values;
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
                return _children.Count;
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
                return _children.IsSynchronized;
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
                return _children.SyncRoot;
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public ArrayList Children
        {
            get
            {
                return ArrayList.ReadOnly(_childrenList);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <param name="key">object key</param>
        /// <value></value>
        object IDictionary.this[object key]
        {
            get
            {
                return _children[key];
            }

            set
            {
                object oldValue = _children[key]; // can be null
                _childrenList.Remove(oldValue);
                _children[key] = value;
                _childrenList.Add(value);
            }
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
            _children.Add(key, value);
            _childrenList.Add(value);
        }

        /// <summary>
        /// Removes all elements from the <see cref="T:System.Collections.IDictionary"/> object.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IDictionary"/> object is read-only. </exception>
        void IDictionary.Clear()
        {
            _children.Clear();
            _childrenList.Clear();
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
            return _children.Contains(key);
        }

        /// <summary>
        /// Returns an <see cref="T:System.Collections.IDictionaryEnumerator"/> object for the <see cref="T:System.Collections.IDictionary"/> object.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IDictionaryEnumerator"/> object for the <see cref="T:System.Collections.IDictionary"/> object.
        /// </returns>
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return _children.GetEnumerator();
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
            object value = _children[key];
            _childrenList.Remove(value);
            _children.Remove(key);
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
        void ICollection.CopyTo(System.Array array, int index)
        {
            _children.CopyTo(array, index);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _children.GetEnumerator();
        }
    }

    #endregion class Custom_Clr_With_IDictionary

    #region class Custom_Clr_With_IAddChild_IList

    /// <summary>
    /// Custom CLR with IAddChild and IList.
    /// Even though IList is implemented, all of IList members
    /// throw exceptions. This is to ensure that IAddChild is used (takes
    /// precedence) if both IAddChild and IList are implemented.
    /// </summary>
    public class Custom_Clr_With_IAddChild_IList : IAddChild, IList
    {
        /// <summary>
        /// List of child nodes
        /// </summary>
        private readonly ArrayList _children;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_Clr_With_IAddChild_IList"/> class.
        /// </summary>
        public Custom_Clr_With_IAddChild_IList()
        {
            _children = new ArrayList();
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> has a fixed size.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.IList"/> has a fixed size; otherwise, false.</returns>
        bool IList.IsFixedSize
        {
            get
            {
                throw new NotImplementedException("The method or operation is not implemented.");
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
                throw new NotImplementedException("The method or operation is not implemented.");
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
                throw new NotImplementedException("The method or operation is not implemented.");
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
                throw new NotImplementedException("The method or operation is not implemented.");
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
                throw new NotImplementedException("The method or operation is not implemented.");
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public ArrayList Children
        {
            get
            {
                return ArrayList.ReadOnly(_children);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> at the specified index.
        /// </summary>
        /// <param name="index">int index. </param>
        /// <value></value>
        object IList.this[int index]
        {
            get
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }

            set
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
        }

        /// <summary>
        /// Adds the text.
        /// </summary>
        /// <param name="text">The text value.</param>
        void IAddChild.AddText(string text)
        {
            _children.Add(text);
        }

        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <param name="o">The object value.</param>
        void IAddChild.AddChild(object o)
        {
            _children.Add(o);
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
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only. </exception>
        void IList.Clear()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
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
            throw new NotImplementedException("The method or operation is not implemented.");
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
            throw new NotImplementedException("The method or operation is not implemented.");
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
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"/> to remove from the <see cref="T:System.Collections.IList"/>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception>
        void IList.Remove(object value)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
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
            throw new NotImplementedException("The method or operation is not implemented.");
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
        void ICollection.CopyTo(System.Array array, int index)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
    }

    #endregion class Custom_Clr_With_IAddChild_IList

    #region class Custom_Clr_With_IAddChild_IDictionary

    /// <summary>
    /// Custom CLR with IAddChild and IDictionary.
    /// Even though IDictionary is implemented, all of IDictionary members
    /// throw exceptions. This is to ensure that IAddChild is used (takes
    /// precedence) if both IAddChild and IDictionary are implemented.
    /// </summary>
    public class Custom_Clr_With_IAddChild_IDictionary : IAddChild, IDictionary
    {
        /// <summary>
        /// List of child nodes
        /// </summary>
        private readonly ArrayList _children;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_Clr_With_IAddChild_IDictionary"/> class.
        /// </summary>
        public Custom_Clr_With_IAddChild_IDictionary()
        {
            _children = new ArrayList();
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IDictionary"/> object has a fixed size.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.IDictionary"/> object has a fixed size; otherwise, false.</returns>
        bool IDictionary.IsFixedSize
        {
            get
            {
                throw new NotImplementedException("The method or operation is not implemented.");
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
                throw new NotImplementedException("The method or operation is not implemented.");
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
                throw new NotImplementedException("The method or operation is not implemented.");
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
                throw new NotImplementedException("The method or operation is not implemented.");
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
                throw new NotImplementedException("The method or operation is not implemented.");
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
                throw new NotImplementedException("The method or operation is not implemented.");
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
                throw new NotImplementedException("The method or operation is not implemented.");
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public ArrayList Children
        {
            get
            {
                return ArrayList.ReadOnly(_children);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <param name="key">object key value</param>
        /// <value></value>
        object IDictionary.this[object key]
        {
            get
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }

            set
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
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
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Adds the text.
        /// </summary>
        /// <param name="text">The text value.</param>
        void IAddChild.AddText(string text)
        {
            _children.Add(text);
        }

        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <param name="o">The object value.</param>
        void IAddChild.AddChild(object o)
        {
            _children.Add(o);
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
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Removes all elements from the <see cref="T:System.Collections.IDictionary"/> object.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IDictionary"/> object is read-only. </exception>
        void IDictionary.Clear()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
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
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Returns an <see cref="T:System.Collections.IDictionaryEnumerator"/> object for the <see cref="T:System.Collections.IDictionary"/> object.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IDictionaryEnumerator"/> object for the <see cref="T:System.Collections.IDictionary"/> object.
        /// </returns>
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
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
        void ICollection.CopyTo(System.Array array, int index)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
    }

    #endregion class Custom_Clr_With_IAddChild_IDictionary

    #region class Custom_Clr_With_IAddChild_CPA

    /// <summary>
    /// Custom CLR with IAddChild and CPA.
    /// It has a CPA that points to a RO CLR property of type ArrayList (implements IList)
    /// However, the ArrayList returned is Read-only, so it can't be used to add children.
    /// Thus, the CPA mechanism can't be used here to add children. IAddChild has to be used.
    /// If parser tries to use CPA mechanism to add children, that will fail.
    /// This class is implemented like this in order to test that IAddChild is used 
    /// (takes precedence) if both IAddChild and CPA are implemented.
    /// </summary>
    [ContentProperty("Content")]
    public class Custom_Clr_With_IAddChild_CPA : IAddChild
    {
        /// <summary>
        /// List of child nodes
        /// </summary>
        private readonly ArrayList _children;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_Clr_With_IAddChild_CPA"/> class.
        /// </summary>
        public Custom_Clr_With_IAddChild_CPA()
        {
            _children = new ArrayList();
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        public ArrayList Content
        {
            get
            {
                return ArrayList.ReadOnly(new ArrayList());
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public ArrayList Children
        {
            get
            {
                return ArrayList.ReadOnly(_children);
            }
        }

        /// <summary>
        /// Adds the text.
        /// </summary>
        /// <param name="text">The text value.</param>
        void IAddChild.AddText(string text)
        {
            _children.Add(text);
        }

        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <param name="o">The object value.</param>
        void IAddChild.AddChild(object o)
        {
            _children.Add(o);
        }
    }

    #endregion class Custom_Clr_With_IAddChild_CPA

    #region class Custom_Clr_With_CPA_Singular_ClrProp_RO

    /// <summary>
    /// Custom CLR with a CPA that points to a Singular RO Clr property.
    /// </summary>
    [ContentProperty("Content")]
    public class Custom_Clr_With_CPA_Singular_ClrProp_RO
    {
        /// <summary>
        /// List of Child nodes
        /// </summary>
        private readonly ArrayList _children;

        /// <summary>
        /// content prop
        /// </summary>
        private Button _content = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_Clr_With_CPA_Singular_ClrProp_RO"/> class.
        /// </summary>
        public Custom_Clr_With_CPA_Singular_ClrProp_RO()
        {
            _children = new ArrayList();
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        public Button Content
        {
            get
            {
                return _content;
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public ArrayList Children
        {
            get
            {
                _children.Add(_content);
                return ArrayList.ReadOnly(_children);
            }
        }
    }

    #endregion class Custom_Clr_With_CPA_Singular_ClrProp_RO

    #region class Custom_Clr_With_CPA_Singular_ClrProp_RW

    /// <summary>
    /// Custom CLR with a CPA that points to a Singular RW Clr property.
    /// </summary>
    [ContentProperty("Content")]
    public class Custom_Clr_With_CPA_Singular_ClrProp_RW
    {
        /// <summary> List of Child nodes </summary>
        private readonly ArrayList _children;

        /// <summary> Content prop </summary>
        private Button _content = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_Clr_With_CPA_Singular_ClrProp_RW"/> class.
        /// </summary>
        public Custom_Clr_With_CPA_Singular_ClrProp_RW()
        {
            _children = new ArrayList();
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public Button Content
        {
            get
            {
                return _content;
            }

            set
            {
                _content = value;
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public ArrayList Children
        {
            get
            {
                _children.Add(_content);
                return ArrayList.ReadOnly(_children);
            }
        }
    }

    #endregion class Custom_Clr_With_CPA_Singular_ClrProp_RW

    #region Custom_Clr_With_CPA_IList_ClrProp_RO

    /// <summary>
    /// Custom CLR with a CPA that points to a RO CLR property of 
    /// a type that implements IList.
    /// </summary>
    [ContentProperty("Content")]
    public class Custom_Clr_With_CPA_IList_ClrProp_RO
    {
        /// <summary> List of Child nodes </summary>
        private readonly ArrayList _content;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_Clr_With_CPA_IList_ClrProp_RO"/> class.
        /// </summary>
        public Custom_Clr_With_CPA_IList_ClrProp_RO()
        {
            _content = new ArrayList();
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        public ArrayList Content
        {
            get
            {
                return _content;
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public ArrayList Children
        {
            get
            {
                return ArrayList.ReadOnly(_content);
            }
        }
    }

    #endregion Custom_Clr_With_CPA_IList_ClrProp_RO

    #region Custom_Clr_With_CPA_IList_ClrProp_RW

    /// <summary>
    /// Custom CLR with a CPA that points to a RW CLR property of 
    /// a type that implements IList.
    /// </summary>
    [ContentProperty("Content")]
    public class Custom_Clr_With_CPA_IList_ClrProp_RW
    {
        /// <summary> List of Child nodes </summary>
        private ArrayList _content;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_Clr_With_CPA_IList_ClrProp_RW"/> class.
        /// </summary>
        public Custom_Clr_With_CPA_IList_ClrProp_RW()
        {
            _content = new ArrayList();
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public ArrayList Content
        {
            get
            {
                return _content;
            }

            set
            {
                _content = value;
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public ArrayList Children
        {
            get
            {
                return ArrayList.ReadOnly(_content);
            }
        }
    }

    #endregion Custom_Clr_With_CPA_IList_ClrProp_RW

    #region class Custom_Clr_Accepting_XmlLang

    /// <summary>
    /// Custom CLR class with a property marked with [XmlLang] attribute, 
    /// thus accepts xml:lang attribute on it in XAML
    /// </summary>
    [XmlLangProperty("Culture")]
    public class Custom_Clr_Accepting_XmlLang
    {
        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        /// <value>The culture.</value>
        public string Culture { get; set; }
    }

    #endregion class Custom_Clr_Accepting_XmlLang

    #region class Custom_DO

    /// <summary>
    /// Custom DependencyObject
    /// </summary>
    public class Custom_DO : DependencyObject
    {
    }

    #endregion class Custom_DO

    #region class Custom_DO_With_IAddChild

    /// <summary>
    /// Custom DependencyObject with IAddChild
    /// </summary>
    public class Custom_DO_With_IAddChild : DependencyObject, IAddChild
    {
        /// <summary> List of Child nodes </summary>
        private readonly ArrayList _children;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_DO_With_IAddChild"/> class.
        /// </summary>
        public Custom_DO_With_IAddChild()
        {
            _children = new ArrayList();
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public ArrayList Children
        {
            get
            {
                return ArrayList.ReadOnly(_children);
            }
        }

        /// <summary>
        /// Adds the text.
        /// </summary>
        /// <param name="text">The text value.</param>
        void IAddChild.AddText(string text)
        {
            _children.Add(text);
        }

        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <param name="o">The object value.</param>
        void IAddChild.AddChild(object o)
        {
            _children.Add(o);
        }
    }

    #endregion class Custom_DO_With_IAddChild

    #region class Custom_DO_With_IList

    /// <summary>
    /// Custom DependencyObject with IList. 
    /// </summary>
    public class Custom_DO_With_IList : DependencyObject, IList
    {
        /// <summary> List of Child nodes </summary>
        private readonly ArrayList _children;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_DO_With_IList"/> class.
        /// </summary>
        public Custom_DO_With_IList()
        {
            _children = new ArrayList();
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public ArrayList Children
        {
            get
            {
                return ArrayList.ReadOnly(_children);
            }
        }

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
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
        /// </summary>
        /// <value></value>
        /// <returns>true if access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe); otherwise, false.</returns>
        bool ICollection.IsSynchronized
        {
            get
            {
                return _children.IsSynchronized;
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
                return _children.SyncRoot;
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
                return _children.Count;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified item index.
        /// </summary>
        /// <param name="itemIndex"> index value</param>
        /// <value>int value .</value>
        object IList.this[int itemIndex]
        {
            get
            {
                return _children[itemIndex];
            }

            set
            {
                _children[itemIndex] = value;
            }
        }

        /// <summary>
        /// Adds the specified val.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <returns>int value.</returns>
        int IList.Add(object val)
        {
            return _children.Add(val);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only. </exception>
        void IList.Clear()
        {
            _children.Clear();
        }

        /// <summary>
        /// Determines whether [contains] [the specified val].
        /// </summary>
        /// <param name="val">The value.</param>
        /// <returns>
        /// <c>true</c> if [contains] [the specified val]; otherwise, <c>false</c>.
        /// </returns>
        bool IList.Contains(object val)
        {
            return _children.Contains(val);
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="indexItem">The index item.</param>
        /// <returns>int value.</returns>
        int IList.IndexOf(object indexItem)
        {
            return _children.IndexOf(indexItem);
        }

        /// <summary>
        /// Inserts the specified insert index.
        /// </summary>
        /// <param name="insertIndex">Index of the insert.</param>
        /// <param name="insertItem">The insert item.</param>
        void IList.Insert(int insertIndex, object insertItem)
        {
            _children.Insert(insertIndex, insertItem);
        }

        /// <summary>
        /// Removes the specified remove item.
        /// </summary>
        /// <param name="removeItem">The remove item.</param>
        void IList.Remove(object removeItem)
        {
            _children.Remove(removeItem);
        }

        /// <summary>
        /// Removes at.
        /// </summary>
        /// <param name="removeIndex">Index of the remove.</param>
        void IList.RemoveAt(int removeIndex)
        {
            _children.RemoveAt(removeIndex);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="destinationArray">The destination array.</param>
        /// <param name="destinationStart">The destination start.</param>
        void ICollection.CopyTo(Array destinationArray, int destinationStart)
        {
            _children.CopyTo(destinationArray, destinationStart);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _children.GetEnumerator();
        }
    }

    #endregion class Custom_DO_With_IList

    #region class Custom_DO_With_IDictionary

    /// <summary>
    /// Custom DependencyObject with IDictionary
    /// </summary>
    public class Custom_DO_With_IDictionary : DependencyObject, IDictionary
    {
        /// <summary> Hashtable of Child nodes </summary>
        private readonly Hashtable _children;

        /// <summary> List of Child nodes </summary>
        private readonly ArrayList _childrenList;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_DO_With_IDictionary"/> class.
        /// </summary>
        public Custom_DO_With_IDictionary()
        {
            _children = new Hashtable();
            _childrenList = new ArrayList();
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IDictionary"/> object has a fixed size.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.IDictionary"/> object has a fixed size; otherwise, false.</returns>
        bool IDictionary.IsFixedSize
        {
            get
            {
                return _children.IsFixedSize;
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
                return _children.IsReadOnly;
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
                return _children.Keys;
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
                return _children.Values;
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
                return _children.Count;
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
                return _children.IsSynchronized;
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
                return _children.SyncRoot;
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public ArrayList Children
        {
            get
            {
                return ArrayList.ReadOnly(_childrenList);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <param name="key">object key</param>
        /// <value></value>
        object IDictionary.this[object key]
        {
            get
            {
                return _children[key];
            }

            set
            {
                object oldValue = _children[key]; // can be null
                _childrenList.Remove(oldValue);
                _children[key] = value;
                _childrenList.Add(value);
            }
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
            _children.Add(key, value);
            _childrenList.Add(value);
        }

        /// <summary>
        /// Removes all elements from the <see cref="T:System.Collections.IDictionary"/> object.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IDictionary"/> object is read-only. </exception>
        void IDictionary.Clear()
        {
            _children.Clear();
            _childrenList.Clear();
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
            return _children.Contains(key);
        }

        /// <summary>
        /// Returns an <see cref="T:System.Collections.IDictionaryEnumerator"/> object for the <see cref="T:System.Collections.IDictionary"/> object.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IDictionaryEnumerator"/> object for the <see cref="T:System.Collections.IDictionary"/> object.
        /// </returns>
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return _children.GetEnumerator();
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
            object value = _children[key];
            _childrenList.Remove(value);
            _children.Remove(key);
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
        void ICollection.CopyTo(System.Array array, int index)
        {
            _children.CopyTo(array, index);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _children.GetEnumerator();
        }
    }

    #endregion class Custom_DO_With_IDictionary

    #region class Custom_DO_With_IAddChild_IList

    /// <summary>
    /// Custom DependencyObject with both IAddChild and IList.
    /// Even though IList is implemented, all of IList members
    /// throw exceptions. This is to ensure that IAddChild is used (takes
    /// precedence) if both IAddChild and IList are implemented.
    /// </summary>
    public class Custom_DO_With_IAddChild_IList : DependencyObject, IAddChild, IList
    {
        /// <summary> List of Child nodes </summary>
        private readonly ArrayList _children;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_DO_With_IAddChild_IList"/> class.
        /// </summary>
        public Custom_DO_With_IAddChild_IList()
        {
            _children = new ArrayList();
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> has a fixed size.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.IList"/> has a fixed size; otherwise, false.</returns>
        bool IList.IsFixedSize
        {
            get
            {
                throw new NotImplementedException("The method or operation is not implemented.");
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
                throw new NotImplementedException("The method or operation is not implemented.");
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
                throw new NotImplementedException("The method or operation is not implemented.");
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
                throw new NotImplementedException("The method or operation is not implemented.");
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
                throw new NotImplementedException("The method or operation is not implemented.");
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public ArrayList Children
        {
            get
            {
                return ArrayList.ReadOnly(_children);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> at the specified index.
        /// </summary>
        /// <param name="index"> index value</param>
        /// <value></value>
        object IList.this[int index]
        {
            get
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }

            set
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
        }

        /// <summary>
        /// Adds the text.
        /// </summary>
        /// <param name="text">The text value.</param>
        void IAddChild.AddText(string text)
        {
            _children.Add(text);
        }

        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <param name="o">The object value.</param>
        void IAddChild.AddChild(object o)
        {
            _children.Add(o);
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
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only. </exception>
        void IList.Clear()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
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
            throw new NotImplementedException("The method or operation is not implemented.");
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
            throw new NotImplementedException("The method or operation is not implemented.");
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
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"/> to remove from the <see cref="T:System.Collections.IList"/>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception>
        void IList.Remove(object value)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
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
            throw new NotImplementedException("The method or operation is not implemented.");
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
        void ICollection.CopyTo(System.Array array, int index)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
    }

    #endregion class Custom_DO_With_IAddChild_IList

    #region class Custom_DO_With_IAddChild_IDictionary

    /// <summary>
    /// Custom DependencyObject with IAddChild and IDictionary.
    /// Even though IDictionary is implemented, all of IDictionary members
    /// throw exceptions. This is to ensure that IAddChild is used (takes
    /// precedence) if both IAddChild and IDictionary are implemented.
    /// </summary>
    public class Custom_DO_With_IAddChild_IDictionary : DependencyObject, IAddChild, IDictionary
    {
        /// <summary> List of Child nodes </summary>
        private readonly ArrayList _children;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_DO_With_IAddChild_IDictionary"/> class.
        /// </summary>
        public Custom_DO_With_IAddChild_IDictionary()
        {
            _children = new ArrayList();
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IDictionary"/> object has a fixed size.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.IDictionary"/> object has a fixed size; otherwise, false.</returns>
        bool IDictionary.IsFixedSize
        {
            get
            {
                throw new NotImplementedException("The method or operation is not implemented.");
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
                throw new NotImplementedException("The method or operation is not implemented.");
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
                throw new NotImplementedException("The method or operation is not implemented.");
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
                throw new NotImplementedException("The method or operation is not implemented.");
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
                throw new NotImplementedException("The method or operation is not implemented.");
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
                throw new NotImplementedException("The method or operation is not implemented.");
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
                throw new NotImplementedException("The method or operation is not implemented.");
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public ArrayList Children
        {
            get
            {
                return ArrayList.ReadOnly(_children);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <param name="key">object key </param>
        /// <value></value>
        object IDictionary.this[object key]
        {
            get
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }

            set
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
        }

        /// <summary>
        /// Adds the text.
        /// </summary>
        /// <param name="text">The text value.</param>
        void IAddChild.AddText(string text)
        {
            _children.Add(text);
        }

        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <param name="o">The object value.</param>
        void IAddChild.AddChild(object o)
        {
            _children.Add(o);
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
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Removes all elements from the <see cref="T:System.Collections.IDictionary"/> object.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IDictionary"/> object is read-only. </exception>
        void IDictionary.Clear()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
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
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Returns an <see cref="T:System.Collections.IDictionaryEnumerator"/> object for the <see cref="T:System.Collections.IDictionary"/> object.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IDictionaryEnumerator"/> object for the <see cref="T:System.Collections.IDictionary"/> object.
        /// </returns>
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
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
            throw new NotImplementedException("The method or operation is not implemented.");
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
        void ICollection.CopyTo(System.Array array, int index)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
    }

    #endregion class Custom_DO_With_IAddChild_IDictionary

    #region class Custom_DO_With_IAddChild_CPA

    /// <summary>
    /// Custom DependencyObject with IAddChild and CPA.
    /// It has a CPA that points to a RO CLR property of type ArrayList (implements IList)
    /// However, the ArrayList returned is Read-only, so it can't be used to add children.
    /// Thus, the CPA mechanism can't be used here to add children. IAddChild has to be used.
    /// If parser tries to use CPA mechanism to add children, that will fail.
    /// This class is implemented like this in order to test that IAddChild is used 
    /// (takes precedence) if both IAddChild and CPA are implemented.
    /// </summary>
    [ContentProperty("Content")]
    public class Custom_DO_With_IAddChild_CPA : DependencyObject, IAddChild
    {
        /// <summary>
        /// List of Child nodes
        /// </summary>
        private readonly ArrayList _children;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_DO_With_IAddChild_CPA"/> class.
        /// </summary>
        public Custom_DO_With_IAddChild_CPA()
        {
            _children = new ArrayList();
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        public ArrayList Content
        {
            get
            {
                return ArrayList.ReadOnly(new ArrayList());
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public ArrayList Children
        {
            get
            {
                return ArrayList.ReadOnly(_children);
            }
        }

        /// <summary>
        /// Adds the text.
        /// </summary>
        /// <param name="text">The text value.</param>
        void IAddChild.AddText(string text)
        {
            _children.Add(text);
        }

        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <param name="o">The object value.</param>
        void IAddChild.AddChild(object o)
        {
            _children.Add(o);
        }
    }

    #endregion class Custom_DO_With_IAddChild_CPA

    #region class Custom_DO_With_CPA_Singular_ClrProp_RO

    /// <summary>
    /// Custom DependencyObject with a CPA that points to a Singular RO Clr property.
    /// </summary>
    [ContentProperty("Content")]
    public class Custom_DO_With_CPA_Singular_ClrProp_RO : DependencyObject
    {
        /// <summary> List of Child nodes </summary>
        private readonly ArrayList _children;

        /// <summary> Content Prop. </summary>
        private Button _content = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_DO_With_CPA_Singular_ClrProp_RO"/> class.
        /// </summary>
        public Custom_DO_With_CPA_Singular_ClrProp_RO()
        {
            _children = new ArrayList();
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        public Button Content
        {
            get
            {
                return _content;
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public ArrayList Children
        {
            get
            {
                _children.Add(_content);
                return ArrayList.ReadOnly(_children);
            }
        }
    }

    #endregion class Custom_Clr_With_CPA_Singular_ClrProp_RO

    #region class Custom_DO_With_CPA_Singular_ClrProp_RW

    /// <summary>
    /// Custom DependencyObject with a CPA that points to a Singular RW Clr property.
    /// </summary>
    [ContentProperty("Content")]
    public class Custom_DO_With_CPA_Singular_ClrProp_RW : DependencyObject
    {
        /// <summary> List of Child nodes </summary>
        private readonly ArrayList _children;

        /// <summary> Content Prop. </summary>
        private Button _content = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_DO_With_CPA_Singular_ClrProp_RW"/> class.
        /// </summary>
        public Custom_DO_With_CPA_Singular_ClrProp_RW()
        {
            _children = new ArrayList();
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public Button Content
        {
            get
            {
                return _content;
            }

            set
            {
                _content = value;
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public ArrayList Children
        {
            get
            {
                _children.Add(_content);
                return ArrayList.ReadOnly(_children);
            }
        }
    }

    #endregion class Custom_DO_With_CPA_Singular_ClrProp_RW

    #region class Custom_DO_With_CPA_Singular_DP_RW

    /// <summary>
    /// Custom DependencyObject with a CPA that points to a Singular RW Dependency property.
    /// </summary>
    [ContentProperty("Content")]
    public class Custom_DO_With_CPA_Singular_DP_RW : DependencyObject
    {
        /// <summary>
        /// Gets the Content property valueProperty
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                "Content",
                typeof(Button),
                typeof(Custom_DO_With_CPA_Singular_DP_RW),
                new FrameworkPropertyMetadata(null));

        /// <summary> List of Child nodes </summary>
        private readonly ArrayList _children;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_DO_With_CPA_Singular_DP_RW"/> class.
        /// </summary>
        public Custom_DO_With_CPA_Singular_DP_RW()
        {
            _children = new ArrayList();
        }
        
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public Button Content
        {
            get
            {
                return (Button) GetValue(ContentProperty);
            }

            set
            {
                SetValue(ContentProperty, value);
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public ArrayList Children
        {
            get
            {
                _children.Add(Content);
                return ArrayList.ReadOnly(_children);
            }
        }
    }

    #endregion class Custom_DO_With_CPA_Singular_DP_RW

    #region class Custom_DO_With_CPA_Singular_DP_RO

    /// <summary>
    /// Custom DependencyObject with a CPA that points to a RO Dependency property
    /// of a singular type.
    /// </summary>
    [ContentProperty("Content")]
    public class Custom_DO_With_CPA_Singular_DP_RO : DependencyObject
    {
        /// <summary>
        /// Gets the Content property valueProperty
        /// </summary>
        public static readonly DependencyProperty ContentProperty = s_contentPropertyKey.DependencyProperty;

        /// <summary>
        /// DependencyPropertyKey ContentPropertyKey
        /// </summary>
        private static readonly DependencyPropertyKey s_contentPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "Content",
                typeof(Button),
                typeof(Custom_DO_With_CPA_Singular_DP_RO),
                new FrameworkPropertyMetadata(new Button()));

        /// <summary> List of Child nodes </summary>
        private readonly ArrayList _children;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_DO_With_CPA_Singular_DP_RO"/> class.
        /// </summary>
        public Custom_DO_With_CPA_Singular_DP_RO()
        {
            _children = new ArrayList();
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        public Button Content
        {
            get
            {
                return (Button) GetValue(ContentProperty);
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public ArrayList Children
        {
            get
            {
                _children.Add(Content);
                return ArrayList.ReadOnly(_children);
            }
        }
    }

    #endregion class Custom_DO_With_CPA_Singular_DP_RO

    #region Custom_DO_With_CPA_IList_ClrProp_RO

    /// <summary>
    /// Custom DependencyObject with a CPA that points to a RO CLR property of 
    /// a type that implements IList.
    /// </summary>
    [ContentProperty("Content")]
    public class Custom_DO_With_CPA_IList_ClrProp_RO : DependencyObject
    {
        /// <summary> List of Child nodes </summary>
        private readonly ArrayList _content;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_DO_With_CPA_IList_ClrProp_RO"/> class.
        /// </summary>
        public Custom_DO_With_CPA_IList_ClrProp_RO()
        {
            _content = new ArrayList();
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        public ArrayList Content
        {
            get
            {
                return _content;
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public ArrayList Children
        {
            get
            {
                return ArrayList.ReadOnly(_content);
            }
        }
    }

    #endregion Custom_DO_With_CPA_IList_ClrProp_RO

    #region Custom_DO_With_CPA_IList_ClrProp_RW

    /// <summary>
    /// Custom DependencyObject with a CPA that points to a RW CLR property of 
    /// a type that implements IList.
    /// </summary>
    [ContentProperty("Content")]
    public class Custom_DO_With_CPA_IList_ClrProp_RW : DependencyObject
    {
        /// <summary> List of content nodes </summary>
        private ArrayList _content;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_DO_With_CPA_IList_ClrProp_RW"/> class.
        /// </summary>
        public Custom_DO_With_CPA_IList_ClrProp_RW()
        {
            _content = new ArrayList();
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public ArrayList Content
        {
            get
            {
                return _content;
            }

            set
            {
                _content = value;
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public ArrayList Children
        {
            get
            {
                return ArrayList.ReadOnly(_content);
            }
        }
    }

    #endregion Custom_DO_With_CPA_IList_ClrProp_RW

    #region class Custom_DO_With_CPA_IList_DP_RW

    /// <summary>
    /// Custom DependencyObject with a CPA that points to a RW Dependency property
    /// of a type that implements IList.
    /// </summary>
    [ContentProperty("Content")]
    public class Custom_DO_With_CPA_IList_DP_RW : DependencyObject
    {
        /// <summary>
        /// Gets the Content property valueProperty
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                "Content",
                typeof(ArrayList),
                typeof(Custom_DO_With_CPA_IList_DP_RW),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_DO_With_CPA_IList_DP_RW"/> class.
        /// </summary>
        public Custom_DO_With_CPA_IList_DP_RW()
        {
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public ArrayList Content
        {
            get
            {
                ArrayList content = (ArrayList) GetValue(ContentProperty);
                if (null == content)
                {
                    content = new ArrayList();
                    SetValue(ContentProperty, content);
                }

                return content;
            }

            set
            {
                SetValue(ContentProperty, value);
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public ArrayList Children
        {
            get
            {
                return ArrayList.ReadOnly(Content);
            }
        }
    }

    #endregion class Custom_DO_With_CPA_IList_DP_RW

    #region class Custom_DO_With_CPA_IList_DP_RO

    /// <summary>
    /// Custom DependencyObject with a CPA that points to a RO Dependency property
    /// of a type that implements IList.
    /// </summary>
    [ContentProperty("Content")]
    public class Custom_DO_With_CPA_IList_DP_RO : DependencyObject
    {  
        /// <summary>
        /// Gets the Content property valueProperty
        /// </summary>
        public static readonly DependencyProperty ContentProperty = s_contentPropertyKey.DependencyProperty;
        
        /// <summary>
        /// DependencyPropertyKey ContentPropertyKey
        /// </summary>
        private static readonly DependencyPropertyKey s_contentPropertyKey =
                                                            DependencyProperty.RegisterReadOnly(
                                                                "Content",
                                                                typeof(ArrayList),
                                                                typeof(Custom_DO_With_CPA_IList_DP_RO),
                                                                new FrameworkPropertyMetadata(new ArrayList()));

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_DO_With_CPA_IList_DP_RO"/> class.
        /// </summary>
        public Custom_DO_With_CPA_IList_DP_RO()
        {
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        public ArrayList Content
        {
            get
            {
                ArrayList content = (ArrayList)GetValue(ContentProperty);
                return content;
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public ArrayList Children
        {
            get
            {
                return ArrayList.ReadOnly(Content);
            }
        }
    }

    #endregion class Custom_DO_With_CPA_IList_DP_RO

    #region class Custom_DO_Accepting_XmlLang

    /// <summary>
    /// Custom DependencyObject with a property marked with [XmlLang] attribute, 
    /// thus accepts xml:lang attribute on it in XAML
    /// </summary>
    [XmlLangProperty("Culture")]
    public class Custom_DO_Accepting_XmlLang : DependencyObject
    {
        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        /// <value>The culture.</value>
        public CultureInfo Culture { get; set; }
    }

    #endregion class Custom_DO_Accepting_XmlLang

    #region Custom_DO_With_Properties

    /// <summary>
    /// A custom DO with various properties.
    /// </summary>
    public class Custom_DO_With_Properties : DependencyObject
    {
        /// <summary>
        /// Dependency Property PrivatePropertyProperty
        /// </summary>
        public static readonly DependencyProperty PrivatePropertyProperty =
            DependencyProperty.Register(
                "PrivateProperty",
                typeof(object),
                typeof(Custom_DO_With_Properties),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Dependency Property InternalPropertyProperty
        /// </summary>
        public static readonly DependencyProperty InternalPropertyProperty =
            DependencyProperty.Register(
                "InternalProperty",
                typeof(object),
                typeof(Custom_DO_With_Properties),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Dependency Property NonPublicSetterPropertyProperty
        /// </summary>
        public static readonly DependencyProperty NonPublicSetterPropertyProperty =
            DependencyProperty.Register(
                "NonPublicSetterProperty",
                typeof(object),
                typeof(Custom_DO_With_Properties),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Dependency Property InternalTypeConverterPropertyProperty
        /// </summary>
        public static readonly DependencyProperty InternalTypeConverterPropertyProperty =
            DependencyProperty.Register(
                "InternalTypeConverterProperty",
                typeof(Custom_Clr),
                typeof(Custom_DO_With_Properties),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Dependency Property
        /// </summary>
        public static readonly DependencyProperty SingularDependencyPropRWProperty =
            DependencyProperty.Register(
                "Singular_DP_RW_Property",
                typeof(Button),
                typeof(Custom_DO_With_Properties),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Dependency Property
        /// </summary>
        public static readonly DependencyProperty ArrayDependencyPropRWProperty =
            DependencyProperty.Register(
                "Array_DP_RW_Property",
                typeof(Button[]),
                typeof(Custom_DO_With_Properties),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Dependency Property
        /// </summary>
        public static readonly DependencyProperty IListDependencyPropRONullProperty = s_IListDependencyPropRONullPropertyKey.DependencyProperty;

        /// <summary>
        /// Dependency Property
        /// </summary>
        public static readonly DependencyProperty IListDependencyPropRONonNullProperty = s_IListDependencyPropRONonNullPropertyKey.DependencyProperty;

        /// <summary>
        /// Dependency Property
        /// </summary>
        public static readonly DependencyProperty IListDependencyPropRWNullProperty =
            DependencyProperty.Register(
                "IList_DP_RW_Null_Property",
                typeof(ArrayList),
                typeof(Custom_DO_With_Properties),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Dependency Property
        /// </summary>
        public static readonly DependencyProperty IListDependencyPropRWNonNullProperty =
            DependencyProperty.Register(
                "IList_DP_RW_NonNull_Property",
                typeof(ArrayList),
                typeof(Custom_DO_With_Properties),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Dependency Property
        /// </summary>
        public static readonly DependencyProperty IDictionaryDependencyPropRONullProperty = s_IDictionaryDependencyPropRONullPropertyKey.DependencyProperty;

        /// <summary>
        /// Dependency Property
        /// </summary>
        public static readonly DependencyProperty IDictionaryDependencyPropRONonNullProperty = s_IDictionaryDependencyPropRONonNullPropertyKey.DependencyProperty;

        /// <summary>
        /// Dependency Property
        /// </summary>
        public static readonly DependencyProperty IDictionaryDependencyPropRWNullProperty =
            DependencyProperty.Register(
                "IDictionary_DP_RW_Null_Property",
                typeof(Hashtable),
                typeof(Custom_DO_With_Properties),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Dependency Property
        /// </summary>
        public static readonly DependencyProperty IDictionaryDependencyPropRWNonNullProperty =
            DependencyProperty.Register(
                "IDictionary_DP_RW_NonNull_Property",
                typeof(Hashtable),
                typeof(Custom_DO_With_Properties),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Dependency Property with not set/get CLR accessor
        /// Used in negative parser tests
        /// </summary>
        public static readonly DependencyProperty DPWithNotSetGetProperty =
            DependencyProperty.Register(
                "DPWithNotSetGet",
                typeof(string),
                typeof(Custom_DO_With_Properties),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Dependency Property
        /// </summary>
        public static readonly DependencyProperty SingularDPROProperty = s_singularDependencyPropROPropertyKey.DependencyProperty;

        /// <summary>
        /// Dependency Property
        /// </summary>
        public static readonly DependencyProperty ArrayDependencyPropROProperty = s_arrayDependencyPropROPropertyKey.DependencyProperty;

        /// <summary>
        /// DependencyPropertyKey IList_DP_RO_Null_Property_Key
        /// </summary>
        private static readonly DependencyPropertyKey s_IListDependencyPropRONullPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "IList_DP_RO_Null_Property",
                typeof(ArrayList),
                typeof(Custom_DO_With_Properties),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// DependencyPropertyKey IList_DP_RO_NonNull_Property_Key
        /// </summary>
        private static readonly DependencyPropertyKey s_IListDependencyPropRONonNullPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "IList_DP_RO_NonNull_Property",
                typeof(ArrayList),
                typeof(Custom_DO_With_Properties),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// DependencyPropertyKey IDictionary_DP_RO_Null_Property_Key
        /// </summary>
        private static readonly DependencyPropertyKey s_IDictionaryDependencyPropRONullPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "IDictionary_DP_RO_Null_Property",
                typeof(Hashtable),
                typeof(Custom_DO_With_Properties),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// DependencyPropertyKey Singular_DP_RO_Property_Key 
        /// </summary>
        private static readonly DependencyPropertyKey s_singularDependencyPropROPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "Singular_DP_RO_Property",
                typeof(Button),
                typeof(Custom_DO_With_Properties),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// DependencyPropertyKey Array_DP_RO_Property_Key 
        /// </summary>
        private static readonly DependencyPropertyKey s_arrayDependencyPropROPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "Array_DP_RO_Property",
                typeof(Button[]),
                typeof(Custom_DO_With_Properties),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// DependencyPropertyKey IDictionary_DP_RO_NonNull_Property_Key
        /// </summary>
        private static readonly DependencyPropertyKey s_IDictionaryDependencyPropRONonNullPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "IDictionary_DP_RO_NonNull_Property",
                typeof(Hashtable),
                typeof(Custom_DO_With_Properties),
                new FrameworkPropertyMetadata(null));

        #region NonPublicSetterProperty

        /// <summary>
        /// Gets the non public setter property.
        /// </summary>
        /// <value>The non public setter property.</value>
        public object NonPublicSetterProperty
        {
            get
            {
                return GetValue(NonPublicSetterPropertyProperty);
            }

            internal set
            {
                SetValue(NonPublicSetterPropertyProperty, value);
            }
        }

        #endregion NonPublicSetterProperty

        #region InternalTypeConverterProperty

        /// <summary>
        /// Gets or sets the internal type converter property.
        /// </summary>
        /// <value>The internal type converter property.</value>
        [TypeConverter(typeof(InternalTypeConverterForCustomClr))]
        public Custom_Clr InternalTypeConverterProperty
        {
            get
            {
                return (Custom_Clr) GetValue(InternalTypeConverterPropertyProperty);
            }

            set
            {
                SetValue(InternalTypeConverterPropertyProperty, value);
            }
        }

        #endregion InternalTypeConverterProperty

        #region Singular_DP_RO

        /// <summary>
        /// Gets the Singular_DP_RO.
        /// </summary>
        /// <value>The Singular_DP_RO.</value>
        public Button Singular_DP_RO
        {
            get
            {
                return GetValue(SingularDPROProperty) as Button;
            }
        }

        #endregion Singular_DP_RO

        #region Singular_DP_RW

        /// <summary>
        /// Gets or sets the singular_ Dp_ RW.
        /// </summary>
        /// <value>The singular_ Dp_ RW.</value>
        public Button Singular_DP_RW
        {
            get
            {
                return GetValue(SingularDependencyPropRWProperty) as Button;
            }

            set
            {
                SetValue(SingularDependencyPropRWProperty, value);
            }
        }

        #endregion Singular_DP_RW

        #region Array_DP_RO

        /// <summary>
        /// Gets the Array_DP_RO.
        /// </summary>
        /// <value>The Array_DP_RO.</value>
        public Button[] Array_DP_RO
        {
            get
            {
                return GetValue(ArrayDependencyPropROProperty) as Button[];
            }
        }

        #endregion Array_DP_RO

        #region Array_DP_RW

        /// <summary>
        /// Gets or sets the array_ D p_ RW.
        /// </summary>
        /// <value>The array_ D p_ RW.</value>
        public Button[] Array_DP_RW
        {
            get
            {
                return GetValue(ArrayDependencyPropRWProperty) as Button[];
            }

            set
            {
                SetValue(ArrayDependencyPropRWProperty, value);
            }
        }

        #endregion Array_DP_RW

        #region IList_DP_RO_Null

        /// <summary>
        /// Gets the IList_DP_RO_Null.
        /// </summary>
        /// <value>The IList_DP_RO_Null.</value>
        public ArrayList IList_DP_RO_Null
        {
            get
            {
                return GetValue(IListDependencyPropRONullProperty) as ArrayList;
            }
        }

        #endregion IList_DP_RO_Null

        #region IList_DP_RO_NonNull

        /// <summary>
        /// Gets the IList_DP_RO_NonNull
        /// </summary>
        /// <value>The IList_DP_RO_NonNull</value>
        public ArrayList IList_DP_RO_NonNull
        {
            get
            {
                if (GetValue(IListDependencyPropRONonNullProperty) == null)
                {
                    SetValue(s_IListDependencyPropRONonNullPropertyKey, new ArrayList());
                }

                return GetValue(IListDependencyPropRONonNullProperty) as ArrayList;
            }
        }

        #endregion IList_DP_RO_NonNull

        #region IList_DP_RW_Null

        /// <summary>
        /// Gets or sets the IList_DP_RW_Null.
        /// </summary>
        /// <value>The IList_DP_RW_Null.</value>
        public ArrayList IList_DP_RW_Null
        {
            get
            {
                return GetValue(IListDependencyPropRWNullProperty) as ArrayList;
            }

            set
            {
                SetValue(IListDependencyPropRWNullProperty, value);
            }
        }

        #endregion IList_DP_RW_Null

        #region IList_DP_RW_NonNull

        /// <summary>
        /// Gets or sets the IList_DP_RW_NonNull
        /// </summary>
        /// <value>The IList_DP_RW_NonNull.</value>
        public ArrayList IList_DP_RW_NonNull
        {
            get
            {
                if (GetValue(IListDependencyPropRWNonNullProperty) == null)
                {
                    SetValue(IListDependencyPropRWNonNullProperty, new ArrayList());
                }

                return GetValue(IListDependencyPropRWNonNullProperty) as ArrayList;
            }

            set
            {
                SetValue(IListDependencyPropRWNonNullProperty, value);
            }
        }

        #endregion IList_DP_RW_NonNull

        #region IDictionary_DP_RO_Null

        /// <summary>
        /// Gets the IDictionary_DP_RO_Null .
        /// </summary>
        /// <value>The IDictionary_DP_RO_Null.</value>
        public Hashtable IDictionary_DP_RO_Null
        {
            get
            {
                return GetValue(IDictionaryDependencyPropRONullProperty) as Hashtable;
            }
        }

        #endregion IDictionary_DP_RO_Null

        #region IDictionary_DP_RO_NonNull

        /// <summary>
        /// Gets the IDictionary_DP_RO_NonNull.
        /// </summary>
        /// <value>The IDictionary_DP_RO_NonNull.</value>
        public Hashtable IDictionary_DP_RO_NonNull
        {
            get
            {
                if (GetValue(IDictionaryDependencyPropRONonNullProperty) == null)
                {
                    SetValue(s_IDictionaryDependencyPropRONonNullPropertyKey, new Hashtable());
                }

                return GetValue(IDictionaryDependencyPropRONonNullProperty) as Hashtable;
            }
        }

        #endregion IDictionary_DP_RO_NonNull

        #region IDictionary_DP_RW_Null

        /// <summary>
        /// Gets or sets the IDictionary_DP_RW_Null
        /// </summary>
        /// <value>The IDictionary_DP_RW_Null.</value>
        public Hashtable IDictionary_DP_RW_Null
        {
            get
            {
                return GetValue(IDictionaryDependencyPropRWNullProperty) as Hashtable;
            }

            set
            {
                SetValue(IDictionaryDependencyPropRWNullProperty, value);
            }
        }

        #endregion IDictionary_DP_RW_Null

        #region IDictionary_DP_RW_NonNull

        /// <summary>
        /// Gets or sets the IDictionary_DP_RW_NonNull.
        /// </summary>
        /// Gets or sets the IDictionary_DP_RW_NonNull.
        public Hashtable IDictionary_DP_RW_NonNull
        {
            get
            {
                if (GetValue(IDictionaryDependencyPropRWNonNullProperty) == null)
                {
                    SetValue(IDictionaryDependencyPropRWNonNullProperty, new Hashtable());
                }

                return GetValue(IDictionaryDependencyPropRWNonNullProperty) as Hashtable;
            }

            set
            {
                SetValue(IDictionaryDependencyPropRWNonNullProperty, value);
            }
        }

        #endregion IDictionary_DP_RW_NonNull

        #region InternalProperty

        /// <summary>
        /// Gets or sets the internal property.
        /// </summary>
        /// <value>The internal property.</value>
        internal object InternalProperty
        {
            get
            {
                return GetValue(InternalPropertyProperty);
            }

            set
            {
                SetValue(InternalPropertyProperty, value);
            }
        }

        #endregion InternalProperty

        #region PrivateProperty

        /// <summary>
        /// Gets or sets the private property.
        /// </summary>
        /// <value>The private property.</value>
        private object PrivateProperty
        {
            get
            {
                return GetValue(PrivatePropertyProperty);
            }

            set
            {
                SetValue(PrivatePropertyProperty, value);
            }
        }

        #endregion PrivateProperty
    }

    #endregion Custom_DO_With_Properties

    #region Custom_DO_With_DateTimeProperty

    /// <summary>
    /// A custom DO with a property of type DateTime.
    /// This is used in DateTime typeconverter testing.
    /// </summary>
    public class Custom_DO_With_DateTimeProperty : DependencyObject
    {
        #region DateTimeProperty

        /// <summary>
        /// Gets or sets the date time property.
        /// </summary>
        /// <value>The date time property.</value>
        public DateTime DateTimeProperty { get; set; }

        #endregion DateTimeProperty
    }

    #endregion Custom_DO_With_DateTimeProperty

    #region Custom_ClrProp_Attacher

    /// <summary>
    /// Custom_ClrProp_Attacher class
    /// </summary>
    public class Custom_ClrProp_Attacher
    {
        /// <summary> Property storage. </summary>
        private static readonly Hashtable s_fieldforSingularClrPropRO = new Hashtable();

        /// <summary> Property storage. </summary>
        private static readonly Hashtable s_fieldforSingularClrPropRW = new Hashtable();

        /// <summary> Property storage. </summary>
        private static readonly Hashtable s_fieldforArrayClrPropRO = new Hashtable();

        /// <summary> Property storage. </summary>
        private static readonly Hashtable s_fieldForArrayClrPropRW = new Hashtable();

        /// <summary> Property storage. </summary>
        private static readonly Hashtable s_fieldforIListClrPropRONull = new Hashtable();

        /// <summary> Property storage. </summary>
        private static readonly Hashtable s_fieldforIListClrPropRONonNull = new Hashtable();

        /// <summary> Property storage. </summary>
        private static readonly Hashtable s_fieldforIListClrPropRWNull = new Hashtable();

        /// <summary> Property storage. </summary>
        private static readonly Hashtable s_fieldforIListClrPropRWNonNull = new Hashtable();

        /// <summary> Property storage. </summary>
        private static readonly Hashtable s_fieldforIDictionaryClrPropRONull = new Hashtable();

        /// <summary> Property storage. </summary>
        private static readonly Hashtable s_fieldforIDictionaryClrPropRONonNull = new Hashtable();

        /// <summary> Property storage. </summary>
        private static readonly Hashtable s_fieldforIDictionaryClrPropRWNull = new Hashtable();

        /// <summary> Property storage. </summary>
        private static readonly Hashtable s_fieldforIDictionaryClrPropRWNonNull = new Hashtable();

        #region Singular_ClrProp_RO

        /// <summary>
        /// Static getter
        /// </summary>
        /// <param name="target">object value</param>
        /// <returns>Button value</returns>
        public static Button GetSingular_ClrProp_RO(object target)
        {
            return s_fieldforSingularClrPropRO[target] as Button;
        }

        #endregion Singular_ClrProp_RO

        #region Singular_ClrProp_RW

        /// <summary>
        /// Static setter
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public static void SetSingular_ClrProp_RW(object target, Button value)
        {
            s_fieldforSingularClrPropRW[target] = value;
        }

        /// <summary>
        /// Static getter
        /// </summary>
        /// <param name="target">object value</param>
        /// <returns>Button value</returns>
        public static Button GetSingular_ClrProp_RW(object target)
        {
            return s_fieldforSingularClrPropRW[target] as Button;
        }

        #endregion Singular_ClrProp_RW

        #region Array_ClrProp_RO

        /// <summary>
        /// Static getter
        /// </summary>
        /// <param name="target">object value</param>
        /// <returns>Button array value</returns>
        public static Button[] GetArray_ClrProp_RO(object target)
        {
            return s_fieldforArrayClrPropRO[target] as Button[];
        }

        #endregion Array_ClrProp_RO

        #region Array_ClrProp_RW

        /// <summary>
        /// Static setter
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public static void SetArray_ClrProp_RW(object target, Button[] value)
        {
            s_fieldForArrayClrPropRW[target] = value;
        }

        /// <summary>
        /// Static getter
        /// </summary>
        /// <param name="target">object value</param>
        /// <returns>BUtton array value</returns>
        public static Button[] GetArray_ClrProp_RW(object target)
        {
            return s_fieldForArrayClrPropRW[target] as Button[];
        }

        #endregion Array_ClrProp_RW

        #region IList_ClrProp_RO_Null

        /// <summary>
        /// Static getter
        /// </summary>
        /// <param name="target">Object value</param>
        /// <returns>ArrayList value</returns>
        public static ArrayList GetIList_ClrProp_RO_Null(object target)
        {
            return s_fieldforIListClrPropRONull[target] as ArrayList;
        }

        #endregion IList_ClrProp_RO_Null

        #region IList_ClrProp_RO_NonNull

        /// <summary>
        /// Static getter
        /// </summary>
        /// <param name="target">object value</param>
        /// <returns>ArrayList value</returns>
        public static ArrayList GetIList_ClrProp_RO_NonNull(object target)
        {
            if (s_fieldforIListClrPropRONonNull[target] == null)
            {
                s_fieldforIListClrPropRONonNull[target] = new ArrayList();
            }

            return s_fieldforIListClrPropRONonNull[target] as ArrayList;
        }

        #endregion IList_ClrProp_RO_NonNull

        #region IList_ClrProp_RW_Null

        /// <summary>
        /// Static setter
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public static void SetIList_ClrProp_RW_Null(object target, ArrayList value)
        {
            s_fieldforIListClrPropRWNull[target] = value;
        }

        /// <summary>
        /// Static getter
        /// </summary>
        /// <param name="target">Object value</param>
        /// <returns>ArrayList value</returns>
        public static ArrayList GetIList_ClrProp_RW_Null(object target)
        {
            return s_fieldforIListClrPropRWNull[target] as ArrayList;
        }

        #endregion IList_ClrProp_RW_Null

        #region IList_ClrProp_RW_NonNull

        /// <summary>
        /// Static setter
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public static void SetIList_ClrProp_RW_NonNull(object target, ArrayList value)
        {
            s_fieldforIListClrPropRWNonNull[target] = value;
        }

        /// <summary>
        /// Static getter
        /// </summary>
        /// <param name="target">Object value</param>
        /// <returns>ArrayList value</returns>
        public static ArrayList GetIList_ClrProp_RW_NonNull(object target)
        {
            if (s_fieldforIListClrPropRWNonNull[target] == null)
            {
                s_fieldforIListClrPropRWNonNull[target] = new ArrayList();
            }

            return s_fieldforIListClrPropRWNonNull[target] as ArrayList;
        }

        #endregion IList_ClrProp_RW_NonNull

        #region IDictionary_ClrProp_RO_Null

        /// <summary>
        /// Static getter
        /// </summary>
        /// <param name="target">Object value</param>
        /// <returns>Hashtable value</returns>
        public static Hashtable GetIDictionary_ClrProp_RO_Null(object target)
        {
            return s_fieldforIDictionaryClrPropRONull[target] as Hashtable;
        }

        #endregion IDictionary_ClrProp_RO_Null

        #region IDictionary_ClrProp_RO_NonNull

        /// <summary>
        /// Static getter
        /// </summary>
        /// <param name="target">Object value</param>
        /// <returns>Hashtable value</returns>
        public static Hashtable GetIDictionary_ClrProp_RO_NonNull(object target)
        {
            if (s_fieldforIDictionaryClrPropRONonNull[target] == null)
            {
                s_fieldforIDictionaryClrPropRONonNull[target] = new Hashtable();
            }

            return s_fieldforIDictionaryClrPropRONonNull[target] as Hashtable;
        }

        #endregion IDictionary_ClrProp_RO_NonNull

        #region IDictionary_ClrProp_RW_Null

        /// <summary>
        /// Static setter
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public static void SetIDictionary_ClrProp_RW_Null(object target, Hashtable value)
        {
            s_fieldforIDictionaryClrPropRWNull[target] = value;
        }

        /// <summary>
        /// Static getter
        /// </summary>
        /// <param name="target">Object value</param>
        /// <returns>Hashtable value</returns>
        public static Hashtable GetIDictionary_ClrProp_RW_Null(object target)
        {
            return s_fieldforIDictionaryClrPropRWNull[target] as Hashtable;
        }

        #endregion IDictionary_ClrProp_RW_Null

        #region IDictionary_ClrProp_RW_NonNull

        /// <summary>
        /// Static setter
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public static void SetIDictionary_ClrProp_RW_NonNull(object target, Hashtable value)
        {
            s_fieldforIDictionaryClrPropRWNonNull[target] = value;
        }

        /// <summary>
        /// Static getter
        /// </summary>
        /// <param name="target">Object value</param>
        /// <returns>Hashtable value</returns>
        public static Hashtable GetIDictionary_ClrProp_RW_NonNull(object target)
        {
            if (s_fieldforIDictionaryClrPropRWNonNull[target] == null)
            {
                s_fieldforIDictionaryClrPropRWNonNull[target] = new Hashtable();
            }

            return s_fieldforIDictionaryClrPropRWNonNull[target] as Hashtable;
        }

        #endregion IDictionary_ClrProp_RW_NonNull
    }

    #endregion Custom_ClrProp_Attacher

    #region Custom_DP_Attacher

    /// <summary>
    /// A custom class declaring attached DPs with static getters/setters of various 
    /// access levels, such as internal, private etc. 
    /// </summary>
    public class Custom_DP_Attacher : DependencyObject
    {
        /// <summary>
        /// Dependency Property AttachedPrivatePropertyProperty
        /// </summary>
        public static readonly DependencyProperty AttachedPrivatePropertyProperty = DependencyProperty.RegisterAttached(
            "AttachedPrivateProperty",
            typeof(object),
            typeof(Custom_DP_Attacher),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Dependency Property AttachedInternalPropertyProperty
        /// </summary>
        public static readonly DependencyProperty AttachedInternalPropertyProperty = DependencyProperty.RegisterAttached(
            "AttachedInternalProperty",
            typeof(object),
            typeof(Custom_DP_Attacher),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Dependency Property AttachedInternalTypeConverterPropertyProperty
        /// </summary>
        public static readonly DependencyProperty AttachedInternalTypeConverterPropertyProperty = DependencyProperty.RegisterAttached(
            "AttachedInternalTypeConverterProperty",
            typeof(Custom_Clr),
            typeof(Custom_DP_Attacher),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Dependency Property AttachedPropertyWithNoSetGetProperty
        /// </summary>
        public static readonly DependencyProperty AttachedPropertyWithNoSetGetProperty = DependencyProperty.RegisterAttached(
            "AttachedPropertyWithNoSetGet",
            typeof(string),
            typeof(Custom_DP_Attacher),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Dependency Property AttachedPropertyWithNoSetOnlyGetProperty
        /// </summary>
        public static readonly DependencyProperty AttachedPropertyWithNoSetOnlyGetProperty = DependencyProperty.RegisterAttached(
            "AttachedPropertyWithNoSetOnlyGet",
            typeof(string),
            typeof(Custom_DP_Attacher),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Dependency Property AttachedPropertyWithSetNoGetProperty
        /// </summary>
        public static readonly DependencyProperty AttachedPropertyWithSetNoGetProperty = DependencyProperty.RegisterAttached(
            "AttachedPropertyWithSetNoGet",
            typeof(string),
            typeof(Custom_DP_Attacher),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Dependency Property AttachedDateTimePropertyProperty
        /// </summary>
        public static readonly DependencyProperty AttachedDateTimePropertyProperty = DependencyProperty.RegisterAttached(
            "AttachedDateTimeProperty",
            typeof(DateTime),
            typeof(Custom_DP_Attacher),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets the attached internal type converter property.
        /// </summary>
        /// <param name="e">The dependency object value.</param>
        /// <returns>Custom_Clr value</returns>
        [TypeConverter(typeof(InternalTypeConverterForCustomClr))]
        public static Custom_Clr GetAttachedInternalTypeConverterProperty(DependencyObject e)
        {
            return (Custom_Clr) e.GetValue(AttachedInternalTypeConverterPropertyProperty);
        }

        /// <summary>
        /// Sets the attached internal type converter property.
        /// </summary>
        /// <param name="e">The dependency object value.</param>
        /// <param name="value">The value.</param>
        public static void SetAttachedInternalTypeConverterProperty(DependencyObject e, Custom_Clr value)
        {
            e.SetValue(AttachedInternalTypeConverterPropertyProperty, value);
        }

        /// <summary>
        /// Gets the attached property with no set only get.
        /// </summary>
        /// <param name="e">The dependency object value.</param>
        /// <returns>object value</returns>
        public static object GetAttachedPropertyWithNoSetOnlyGet(DependencyObject e)
        {
            return e.GetValue(AttachedPropertyWithNoSetOnlyGetProperty);
        }

        /// <summary>
        /// Sets the attached property with set no get property.
        /// </summary>
        /// <param name="e">The dependency object value.</param>
        /// <param name="value">The value.</param>
        public static void SetAttachedPropertyWithSetNoGetProperty(DependencyObject e, object value)
        {
            e.SetValue(AttachedPropertyWithSetNoGetProperty, value);
        }

        /// <summary>
        /// Gets the attached date time property.
        /// </summary>
        /// <param name="e">The dependency object value.</param>
        /// <returns>DateTime value.</returns>
        public static DateTime GetAttachedDateTimeProperty(DependencyObject e)
        {
            return (DateTime) e.GetValue(AttachedDateTimePropertyProperty);
        }

        /// <summary>
        /// Sets the attached date time property.
        /// </summary>
        /// <param name="e">The dependency object value.</param>
        /// <param name="value">The value.</param>
        public static void SetAttachedDateTimeProperty(DependencyObject e, DateTime value)
        {
            e.SetValue(AttachedDateTimePropertyProperty, value);
        }

        /// <summary>
        /// Gets the attached internal property.
        /// </summary>
        /// <param name="e">The dependency object value.</param>
        /// <returns>object value</returns>
        internal static object GetAttachedInternalProperty(DependencyObject e)
        {
            return e.GetValue(AttachedInternalPropertyProperty);
        }

        /// <summary>
        /// Sets the attached internal property.
        /// </summary>
        /// <param name="e">The DependencyObject e.</param>
        /// <param name="value">The value.</param>
        internal static void SetAttachedInternalProperty(DependencyObject e, object value)
        {
            e.SetValue(AttachedInternalPropertyProperty, value);
        }

        /// <summary>
        /// Gets the attached private property.
        /// </summary>
        /// <param name="e">The dependency object value.</param>
        /// <returns>object value</returns>
        private static object GetAttachedPrivateProperty(DependencyObject e)
        {
            return e.GetValue(AttachedPrivatePropertyProperty);
        }

        /// <summary>
        /// Sets the attached private property.
        /// </summary>
        /// <param name="e">The dependency object value.</param>
        /// <param name="value">The value.</param>
        private static void SetAttachedPrivateProperty(DependencyObject e, object value)
        {
            e.SetValue(AttachedPrivatePropertyProperty, value);
        }
    }

    #endregion Custom_DP_Attacher

    #region Custom_Class_With_Events

    /// <summary>
    /// A custom class with events of various access levels, such as 
    /// internal, private etc. Each of them is a CLR accessor to a RoutedEvent.
    /// </summary>
    public class Custom_Class_With_Events : UIElement
    {
        /// <summary>
        /// RoutedEvent PrivateEventEvent
        /// </summary>
        public static readonly RoutedEvent PrivateEventEvent = EventManager.RegisterRoutedEvent("PrivateEvent", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(Custom_Class_With_Events));

        /// <summary>
        /// RoutedEvent InternalEventEvent
        /// </summary>
        public static readonly RoutedEvent InternalEventEvent = EventManager.RegisterRoutedEvent("InternalEvent", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(Custom_Class_With_Events));

        /// <summary>
        /// Occurs when [internal event].
        /// </summary>
        internal event RoutedEventHandler InternalEvent
        {
            add
            {
                AddHandler(InternalEventEvent, value);
            }

            remove
            {
                RemoveHandler(InternalEventEvent, value);
            }
        }

        /// <summary>
        /// Occurs when [private event].
        /// </summary>
        private event RoutedEventHandler PrivateEvent
        {
            add
            {
                AddHandler(PrivateEventEvent, value);
            }

            remove
            {
                RemoveHandler(PrivateEventEvent, value);
            }
        }
    }

    #endregion Custom_Class_With_Events

    #region Custom_Event_Attacher

    /// <summary>
    /// A custom class declaring attached RoutedEvents with static Add/RemoveHandler
    /// methods of various access levels, such as internal, private etc. 
    /// </summary>
    public class Custom_Event_Attacher : DependencyObject
    {
        /// <summary>
        /// RoutedEvent AttachedPrivateEventEvent
        /// </summary>
        public static readonly RoutedEvent AttachedPrivateEventEvent = EventManager.RegisterRoutedEvent("AttachedPrivateEvent", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(Custom_Event_Attacher));

        /// <summary>
        /// RoutedEvent AttachedInternalEventEvent
        /// </summary>
        public static readonly RoutedEvent AttachedInternalEventEvent = EventManager.RegisterRoutedEvent("AttachedInternalEvent", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(Custom_Event_Attacher));

        /// <summary>
        /// Adds the attached internal event handler.
        /// </summary>
        /// <param name="d">The dependency object d.</param>
        /// <param name="handler">The handler.</param>
        internal static void AddAttachedInternalEventHandler(DependencyObject d, RoutedEventHandler handler)
        {
            (d as UIElement).AddHandler(AttachedInternalEventEvent, handler);
        }

        /// <summary>
        /// Removes the attached internal event handler.
        /// </summary>
        /// <param name="d">The dependency object d.</param>
        /// <param name="handler">The handler.</param>
        internal static void RemoveAttachedInternalEventHandler(DependencyObject d, RoutedEventHandler handler)
        {
            (d as UIElement).RemoveHandler(AttachedInternalEventEvent, handler);
        }

        /// <summary>
        /// Adds the attached private event handler.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="handler">The handler.</param>
        private static void AddAttachedPrivateEventHandler(DependencyObject d, RoutedEventHandler handler)
        {
            (d as UIElement).AddHandler(AttachedPrivateEventEvent, handler);
        }

        /// <summary>
        /// Removes the attached private event handler.
        /// </summary>
        /// <param name="d">The dependency object d.</param>
        /// <param name="handler">The handler.</param>
        private static void RemoveAttachedPrivateEventHandler(DependencyObject d, RoutedEventHandler handler)
        {
            (d as UIElement).RemoveHandler(AttachedPrivateEventEvent, handler);
        }
    }

    #endregion Custom_Event_Attacher

    #region Custom_MarkupExt_With_Private_Ctor

    /// <summary>
    /// A custom Markup Extension with a private constructor
    /// </summary>
    public class Custom_MarkupExt_With_Private_Ctor : MarkupExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_MarkupExt_With_Private_Ctor"/> class.
        /// </summary>
        /// <param name="param1">The param1.</param>
        /// <param name="param2">The param2.</param>
        private Custom_MarkupExt_With_Private_Ctor(string param1, string param2)
        {
        }

        /// <summary>
        /// Provide Value.
        /// </summary>
        /// <param name="serviceProvider">service Provider</param>
        /// <returns>object value</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return String.Empty;
        }
    }

    #endregion Custom_MarkupExt_With_Private_Ctor

    #region Custom_MarkupExt_With_Internal_Ctor

    /// <summary>
    /// A custom Markup Extension with a internal constructor
    /// </summary>
    public class Custom_MarkupExt_With_Internal_Ctor : MarkupExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_MarkupExt_With_Internal_Ctor"/> class.
        /// </summary>
        /// <param name="param1">The param1.</param>
        /// <param name="param2">The param2.</param>
        internal Custom_MarkupExt_With_Internal_Ctor(string param1, string param2)
        {
        }

        /// <summary>
        /// Provide Value.
        /// </summary>
        /// <param name="serviceProvider">service Provider</param>
        /// <returns>object value</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return String.Empty;
        }
    }

    #endregion Custom_MarkupExt_With_Internal_Ctor

    #region Custom_Class_With_Private_ContentProperty

    /// <summary>
    /// Custom class with a private content property.
    /// </summary>
    [ContentProperty("PrivateProperty")]
    public class Custom_Class_With_Private_ContentProperty : DependencyObject
    {
        #region PrivateProperty

        /// <summary>
        /// Dependency Property PrivatePropertyProperty
        /// </summary>
        public static readonly DependencyProperty PrivatePropertyProperty =
            DependencyProperty.Register(
                "PrivateProperty",
                typeof(object),
                typeof(Custom_Class_With_Private_ContentProperty),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the private property.
        /// </summary>
        /// <value>The private property.</value>
        private object PrivateProperty
        {
            get
            {
                return GetValue(PrivatePropertyProperty);
            }

            set
            {
                SetValue(PrivatePropertyProperty, value);
            }
        }

        #endregion PrivateProperty
    }

    #endregion Custom_Class_With_Private_ContentProperty

    #region Custom_Class_With_Internal_ContentProperty

    /// <summary>
    /// Custom class with an internal content property.
    /// </summary>
    [ContentProperty("InternalProperty")]
    public class Custom_Class_With_Internal_ContentProperty : DependencyObject
    {
        #region InternalProperty

        /// <summary>
        /// Dependency Property InternalPropertyProperty
        /// </summary>
        public static readonly DependencyProperty InternalPropertyProperty =
            DependencyProperty.Register(
                "InternalProperty",
                typeof(object),
                typeof(Custom_Class_With_Internal_ContentProperty),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the internal property.
        /// </summary>
        /// <value>The internal property.</value>
        internal object InternalProperty
        {
            get
            {
                return GetValue(InternalPropertyProperty);
            }

            set
            {
                SetValue(InternalPropertyProperty, value);
            }
        }

        #endregion InternalProperty
    }

    #endregion Custom_Class_With_Internal_ContentProperty

    #region Custom_Class_With_NonPublicSetter_ContentProperty

    /// <summary>
    /// Custom class with a public content property that has a non-public setter.
    /// </summary>
    [ContentProperty("NonPublicSetterProperty")]
    public class Custom_Class_With_NonPublicSetter_ContentProperty : DependencyObject
    {
        #region NonPublicSetterProperty

        /// <summary>
        /// Dependency Property NonPublicSetterPropertyProperty
        /// </summary>
        public static readonly DependencyProperty NonPublicSetterPropertyProperty =
            DependencyProperty.Register(
                "NonPublicSetterProperty",
                typeof(object),
                typeof(Custom_Class_With_NonPublicSetter_ContentProperty),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets the non public setter property.
        /// </summary>
        /// <value>The non public setter property.</value>
        public object NonPublicSetterProperty
        {
            get
            {
                return GetValue(NonPublicSetterPropertyProperty);
            }

            internal set
            {
                SetValue(NonPublicSetterPropertyProperty, value);
            }
        }

        #endregion NonPublicSetterProperty
    }

    #endregion Custom_Class_With_NonPublicSetter_ContentProperty

    #region Custom_Class_With_NonPublicGetter_ContentProperty

    /// <summary>
    /// Custom class with a public content property of a collection type 
    /// (implementing IList) that has a non-public getter.
    /// </summary>
    [ContentProperty("NonPublicGetterProperty")]
    public class Custom_Class_With_NonPublicGetter_ContentProperty : DependencyObject
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_Class_With_NonPublicGetter_ContentProperty"/> class.
        /// </summary>
        public Custom_Class_With_NonPublicGetter_ContentProperty()
        {
            NonPublicGetterProperty = new ArrayList();
        }

        #endregion Constructor

        #region NonPublicGetterProperty

        /// <summary>
        /// Gets or sets the non public getter property.
        /// </summary>
        /// <value>The non public getter property.</value>
        public ArrayList NonPublicGetterProperty { internal get; set; }

        #endregion NonPublicGetterProperty
    }

    #endregion Custom_Class_With_NonPublicGetter_ContentProperty

    #region Custom_FrameworkElement

    /// <summary>
    /// A custom class that derives from FrameworkElement, and allows 
    /// adding a child through a logical link or a visual link or both
    /// </summary>
    public class Custom_FrameworkElement : FrameworkElement
    {
        /// <summary> ArrayList logicalChildren </summary>
        private readonly ArrayList _logicalChildren;

        /// <summary> VisualCollection visualChildren </summary>
        private readonly VisualCollection _visualChildren;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_FrameworkElement"/> class.
        /// </summary>
        public Custom_FrameworkElement() : base()
        {
            _logicalChildren = new ArrayList();
            _visualChildren = new VisualCollection(this /*parent*/);
        }

        /// <summary>
        /// Gets an enumerator for logical children.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                return _logicalChildren.GetEnumerator();
            }
        }

        /// <summary>
        /// Gets the VisualChildrenCount
        /// Derived classes override this property to enable the Visual code to enumerate
        /// the Visual children. Derived classes need to return the number of children
        /// from this method.
        /// By default a Visual does not have any children.
        /// </summary>
        /// <value>The visual children count.</value>
        protected override int VisualChildrenCount
        {
            get
            {
                //// _visualChildren cannot be null as its initialized in the constructor
                return _visualChildren.Count;
            }
        }

        /// <summary>
        /// Adds the given child as either logical child or visual child or both.
        /// </summary>
        /// <param name="child">child object</param>
        /// <param name="logical">Is Logical child</param>
        /// <param name="visual">Is Visual child</param>
        public void AddChild(object child, bool logical, bool visual)
        {
            // Ensure non-null child
            if (null == child)
            {
                throw new ArgumentNullException("child");
            }

            // Add child as a logical child
            if (logical)
            {
                _logicalChildren.Add(child);
                AddLogicalChild(child);
            }

            // Add child as a visual child
            if (visual)
            {
                Visual visualChild = child as Visual;
                if (null == visualChild)
                {
                    throw new ArgumentException("Child has to derive from Visual.");
                }

                // VisualCollection.Add eventually calls AddVisualChild()
                _visualChildren.Add(visualChild);
            }
        }

        /// <summary>
        /// Removes the given child by disconnecting its logical link or visual link or both.
        /// </summary>
        /// <param name="child">child object</param>
        /// <param name="logical">Is Logical child</param>
        /// <param name="visual">Is Visual child</param>
        public void RemoveChild(object child, bool logical, bool visual)
        {
            // Ensure non-null child
            if (null == child)
            {
                throw new ArgumentNullException("child");
            }

            // Remove child as a logical child
            if (logical)
            {
                _logicalChildren.Remove(child);
                RemoveLogicalChild(child);
            }

            // Remove child as a visual child
            if (visual)
            {
                Visual visualChild = child as Visual;
                if (null == visualChild)
                {
                    throw new ArgumentException("Child has to derive from Visual.");
                }

                // VisualCollection.Remove eventually calls RemoveVisualChild()
                _visualChildren.Remove(visualChild);
            }
        }

        /// <summary>
        /// Derived class must implement to support Visual children. The method must return
        /// the child at the specified index. Index must be between 0 and GetVisualChildrenCount-1.
        /// By default a Visual does not have any children.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Visual element</returns>
        protected override Visual GetVisualChild(int index)
        {
            //// _visualChildren cannot be null as its initialized in the constructor
            //// index range check done by VisualCollection        
            return _visualChildren[index];
        }
    }

    #endregion Custom_FrameworkElement

    #region Custom_FrameworkElement_With_IContentHost

    /// <summary>
    /// A custom class that has the functionality of both Custom_FrameworkElement
    /// (which is defined elsewhere in this file), and IContentHost
    /// </summary>
    public class Custom_FrameworkElement_With_IContentHost : Custom_FrameworkElement, IContentHost
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_FrameworkElement_With_IContentHost"/> class.
        /// </summary>
        public Custom_FrameworkElement_With_IContentHost()
            : base()
        {
        }

        /// <summary>
        /// Gets elements hosted by the content host as an enumerator class
        /// </summary>
        /// <value>The hosted elements.</value>
        IEnumerator<IInputElement> IContentHost.HostedElements
        {
            get
            {
                return null;
            }
        }

        #region IContentHost implementation

        /// <summary>
        /// Hit tests to the correct ContentElement
        /// within the ContentHost that the mouse
        /// is over
        /// </summary>
        /// <param name="p">Mouse coordinates relative to the ContentHost</param>
        /// <returns>IInputElement value </returns>
        IInputElement IContentHost.InputHitTest(System.Windows.Point p)
        {
            return null;
        }

        /// <summary>
        /// Gets the rectangles.
        /// </summary>
        /// <param name="e">The ContentElement.</param>
        /// <returns>ReadOnlyCollection Rect value</returns>
        ReadOnlyCollection<Rect> IContentHost.GetRectangles(ContentElement e)
        {
            return null;
        }

        /// <summary>
        /// Called when [child desired size changed].
        /// </summary>
        /// <param name="child">The child.</param>
        void IContentHost.OnChildDesiredSizeChanged(UIElement child)
        {
            return;
        }

        #endregion IContentHost implementation
    }

    #endregion Custom_FrameworkElement_With_IContentHost

    #region Custom_FrameworkContentElement

    /// <summary>
    /// A custom class that derives from FrameworkContentElement, and allows 
    /// adding a child through a logical link
    /// </summary>
    public class Custom_FrameworkContentElement : FrameworkContentElement
    {
        /// <summary> ArrayList logicalChildren </summary>
        private readonly ArrayList _logicalChildren;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_FrameworkContentElement"/> class.
        /// </summary>
        public Custom_FrameworkContentElement() : base()
        {
            _logicalChildren = new ArrayList();
        }

        /// <summary>
        /// Gets an enumerator for logical children.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                return _logicalChildren.GetEnumerator();
            }
        }

        /// <summary>
        /// Adds the given child (as a logical child).
        /// </summary>
        /// <param name="child">child object</param>
        public void AddChild(object child)
        {
            // Ensure non-null child
            if (null == child)
            {
                throw new ArgumentNullException("child");
            }

            // Add child as a logical child
            _logicalChildren.Add(child);
            AddLogicalChild(child);
        }

        /// <summary>
        /// Removes the given child by disconnecting its logical link.
        /// </summary>
        /// <param name="child">child object</param>
        public void RemoveChild(object child)
        {
            // Ensure non-null child
            if (null == child)
            {
                throw new ArgumentNullException("child");
            }

            // Remove child as a logical child
            _logicalChildren.Remove(child);
            RemoveLogicalChild(child);
        }
    }

    #endregion Custom_FrameworkContentElement

    #region Class CustomColorBlenderExtension

    /// <summary>
    /// A custom MarkupExtension.
    /// Adds various colors to a brush and returns the blended brush
    /// </summary>
    [ContentProperty("Mixers")]
    [MarkupExtensionReturnType(typeof(SolidColorBrush))]
    public class CustomColorBlenderExtension : MarkupExtension
    {
        #region Private variables

        /// <summary> Core brush  </summary>
        private Brush _core;

        /// <summary> Additive color </summary>
        private Color _additive;

        /// <summary> Color mixers </summary>
        private List<Color> _mixers;

        #endregion Private variables

        #region Public constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomColorBlenderExtension"/> class.
        /// </summary>
        public CustomColorBlenderExtension() : base()
        {
            _core = Brushes.Black;
            _additive = Colors.Black;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomColorBlenderExtension"/> class.
        /// </summary>
        /// <param name="core">Core brush</param>
        public CustomColorBlenderExtension(Brush core) : base()
        {
            this._core = (null == core ? Brushes.Black : core);
            _additive = Colors.Black;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomColorBlenderExtension"/> class.
        /// </summary>
        /// <param name="additive">Additive color</param>
        public CustomColorBlenderExtension(Color additive)
            : base()
        {
            this._core = Brushes.Black;
            this._additive = additive; // Color is a struct, so can't be null.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomColorBlenderExtension"/> class.
        /// </summary>
        /// <param name="core">Core brush</param>
        /// <param name="additive">Additive color</param>
        public CustomColorBlenderExtension(Brush core, Color additive) : base()
        {
            this._core = (null == core ? Brushes.Black : core);
            this._additive = additive; // Color is a struct, so cannot be null
        }

        #endregion Public constructors

        #region Public properties

        /// <summary>
        /// Gets or sets the core.
        /// </summary>
        /// <value>The core value.</value>
        public Brush Core
        {
            get
            {
                return _core;
            }

            set
            {
                _core = (null == value ? Brushes.Black : value);
            }
        }

        /// <summary>
        /// Gets or sets the additive.
        /// </summary>
        /// <value>The additive.</value>
        public Color Additive
        {
            get
            {
                return _additive;
            }

            set
            {
                _additive = value;
            } // Color is a struct, so cannot be null
        }

        /// <summary>
        /// Gets Other colors to be added to the core brush.
        /// </summary>
        public List<Color> Mixers
        {
            get
            {
                if (null == _mixers)
                {
                    _mixers = new List<Color>();
                }

                return _mixers;
            }
        }

        #endregion Public properties

        #region Public methods

        /// <summary>
        /// Return the value of the blended brush
        /// </summary>
        /// <param name="serviceProvider">service Provider</param>
        /// <returns>object value</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            //// Add the additive color and mixers to the core brush
            Color coreColor = (_core as SolidColorBrush).Color;
            Color resultColor = coreColor + _additive;
            if (null != _mixers)
            {
                for (int i = 0; i < _mixers.Count; i++)
                {
                    resultColor = resultColor + _mixers[i];
                }
            }

            if (resultColor.ScA > 1.0f)
            {
                resultColor.ScA = 1.0f;
            }

            return new SolidColorBrush(resultColor);
        }

        #endregion Public methods
    }

    #endregion Class CustomColorBlenderExtension

    #region class XmlContentControl

    /// <summary>
    /// XmlContentControl is a test control. Its Content property (pointed to by ContentProperty 
    /// attribute) is of a type that implements IXmlSerializable. So, content under
    /// XmlContentControl tag in a Xaml can be any valid Xml (doesn't have to be Xaml)
    /// and it should be stored (and later processed) as a LiteralContent record in Baml. 
    /// We implemented this class to test parser's ability to process Baml records of
    /// type LiteralContent. 
    /// </summary>
    [ContentProperty("Content")]
    public class XmlContentControl : Control
    {
        /// <summary> Xml Content </summary>
        private XmlContent _content = null;

        /// <summary>
        /// Gets the Xml Content
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public XmlContent Content
        {
            get
            {
                if (null == _content)
                {
                    _content = new XmlContent();
                }

                return _content;
            }
        }
    }

    /// <summary>
    /// Xml Content
    /// </summary>
    public class XmlContent : IXmlSerializable
    {
        /// <summary> xml data string </summary>
        private string _xml = null;

        /// <summary>
        /// Returns null.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
        /// </returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        public void WriteXml(XmlWriter writer)
        {
            if (null == writer)
            {
                throw new ArgumentNullException("XmlWriter is null");
            }

            writer.WriteRaw(_xml);
        }

        /// <summary>
        /// Deserializes an XmlContent from the XmlReader passed in.
        /// </summary>
        /// <param name="reader">Xml Reader</param>
        /// <exception cref="ArgumentNullException">reader is null</exception>
        public void ReadXml(XmlReader reader)
        {
            if (null == reader)
            {
                throw new ArgumentNullException("XmlReader is null");
            }

            // Read the first node. 
            reader.Read();
            _xml = reader.ReadOuterXml();
        }

        /// <summary>
        /// To String();
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return _xml;
        }
    }

    #endregion class XmlContentControl

    #region Custom_Inline

    /// <summary>
    /// A custom class that derives from LineBreak, and allows 
    /// doesn't trim surrounding WS
    /// </summary>
    public class Custom_Inline : Inline
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_Inline"/> class.
        /// </summary>
        public Custom_Inline()
            : base()
        {
        }
    }

    #endregion Custom_Inline

    #region Custom_ContentWrapper

    /// <summary>
    /// A custom class that derives from DependencyObject, and implements
    /// a generic collection with ContentWrapperAttribute set.
    /// </summary>
    [ContentProperty("CustomGenericCollection")]
    public class Custom_DO_With_GenericCollection_Properties : FrameworkElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_DO_With_GenericCollection_Properties"/> class.
        /// </summary>
        public Custom_DO_With_GenericCollection_Properties()
            : base()
        {
            CustomGenericCollection = new CustomGenericCollection();
        }

        /// <summary>
        /// Gets or sets the custom generic collection.
        /// </summary>
        /// <value>The custom generic collection.</value>
        public CustomGenericCollection CustomGenericCollection { get; set; }
    }

    /// <summary>
    /// A custom class that derives from DependencyObject, and implements
    /// a generic collection with ContentWrapperAttribute set.
    /// </summary>
    [ContentWrapper(typeof(Run))]
    [ContentWrapper(typeof(InlineUIContainer))]
    [WhitespaceSignificantCollection]
    public class CustomGenericCollection : Collection<Inline>, IList
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomGenericCollection"/> class.
        /// </summary>
        public CustomGenericCollection()
        {
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
            Inline inline = null;

            if (value is string)
            {
                inline = new Run((string) value);
            }
            else if (value is UIElement)
            {
                inline = new InlineUIContainer((UIElement) value);
            }
            else
            {
                inline = value as Inline;
            }

            if (inline == null)
            {
                throw new ArgumentException("Must be a string or UIElement.", "value");
            }

            this.Add(inline);

            return this.IndexOf(inline);
        }
    }

    #endregion Custom_Inline

    #region Custom_Button

    /// <summary>
    /// A custom class that derives from Button, and  
    /// trims surrounding WS
    /// </summary>
    [TrimSurroundingWhitespace]
    public class Custom_Button : Button
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_Button"/> class.
        /// </summary>
        public Custom_Button()
            : base()
        {
        }
    }

    #endregion Custom_Button

    #region CustomColor

    /// <summary>
    /// The type of MyClass's CustomColor property
    /// </summary>
    [TypeConverter(typeof(CustomColorConverter))]
    public class CustomColor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomColor"/> class.
        /// </summary>
        public CustomColor()
        {
            Color = "Red"; // default
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>Value of the color string to set.</value>
        public string Color { get; set; }
    }

    #endregion CustomColor

    #region CustomColorConverter

    /// <summary>
    /// Typeconverter for CustomColor class.
    /// </summary>
    public class CustomColorConverter : TypeConverter
    {
        /// <summary>
        /// CanConvertFrom - Returns whether or not CustomColor can convert from a given type.
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
        /// CanConvertTo - Returns whether or not CustomColor can convert to a given type.
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
        /// ConvertFrom - Attempt to convert to a CustomColor from the given object
        /// </summary>
        /// <param name="typeDescriptorContext"> The ITypeDescriptorContext for this call. </param>
        /// <param name="cultureInfo"> The CultureInfo which is respected when converting. </param>
        /// <param name="value"> The object to convert to a CustomColor. </param>
        /// <returns>The CustomColor object created.</returns>
        /// <exception>
        /// An ArgumentException is thrown if the example object is not null and is not a valid type
        /// which can be converted to a CustomColor.
        /// </exception>
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value)
        {
            string s = value as string;

            if (null == s)
            {
                throw new ArgumentException("The given type cannot be converted to CustomColor");
            }

            CustomColor m = new CustomColor();
            m.Color = s;
            return m;
        }

        /// <summary>
        /// ConvertTo - Attempt to convert from a given object (should be CustomColor) to an object of the given type
        /// </summary>
        /// <param name="typeDescriptorContext"> The ITypeDescriptorContext for this call. </param>
        /// <param name="cultureInfo"> The CultureInfo which is respected when converting. </param>
        /// <param name="value"> The object given to convert. Should be CustomColor</param>
        /// <param name="destinationType"> The type to which this will convert the given object. </param>
        /// <returns>The object that was created.</returns>
        /// <exception>
        /// An ArgumentException is thrown if the example object is not null and is not a CustomColor,
        /// or if the destinationType isn't one of the valid destination types.
        /// </exception>
        public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value, Type destinationType)
        {
            CustomColor m = value as CustomColor;

            if (m == null)
            {
                throw new ArgumentException("The given object is not a CustomColor");
            }

            if (destinationType != typeof(string))
            {
                new ArgumentException("CustomColor cannot be converted to the given type.");
            }

            return m.Color;
        }
    }

    #endregion CustomColorConverter

    #region InternalTypeConverterForCustomClr

    /// <summary>
    /// InternalTypeConverter For CustomClr
    /// </summary>
    internal class InternalTypeConverterForCustomClr : TypeConverter
    {
        /// <summary>
        /// Determines whether this instance [can convert from] the specified td.
        /// </summary>
        /// <param name="td">The ITypeDescriptorContext td.</param>
        /// <param name="t">The Type t.</param>
        /// <returns>
        /// <c>true</c> if this instance [can convert from] the specified td; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext td, Type t)
        {
            if (t == typeof(string))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether this instance [can convert to] the specified td.
        /// </summary>
        /// <param name="td">The ITypeDescriptorContext td.</param>
        /// <param name="t">The Type t.</param>
        /// <returns>
        /// <c>true</c> if this instance [can convert to] the specified td; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext td, Type t)
        {
            if (t == typeof(string))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Converts from.
        /// </summary>
        /// <param name="td">The ITypeDescriptorContext td.</param>
        /// <param name="ci">The CultureInfo ci.</param>
        /// <param name="value">The value.</param>
        /// <returns>object value</returns>
        public override object ConvertFrom(ITypeDescriptorContext td, System.Globalization.CultureInfo ci, object value)
        {
            return new Custom_Clr();
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="destinationType"/> parameter is null. </exception>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return String.Empty;
        }
    }

    #endregion InternalTypeConverterForCustomClr

    #region Custom_InternalButton

    /// <summary>
    /// Custom InternalButton
    /// </summary>
    internal class Custom_InternalButton : Button
    {
    }

    #endregion Custom_InternalButton
}
