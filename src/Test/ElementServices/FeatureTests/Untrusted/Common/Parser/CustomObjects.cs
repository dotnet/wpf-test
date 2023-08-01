// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;

using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;

using System.Windows.Documents;

using Avalon.Test.CoreUI.Common;

namespace Avalon.Test.CoreUI.Parser
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
        #region Singular_ClrProp_RO
        private Button _fieldfor_Singular_ClrProp_RO = null;
        /// <summary>
        /// Content
        /// </summary>
        public Button Singular_ClrProp_RO
        {
            get { return _fieldfor_Singular_ClrProp_RO; }
        }
        #endregion Singular_ClrProp_RO

        #region Singular_ClrProp_RW
        private Button _fieldfor_Singular_ClrProp_RW = null;
        /// <summary>
        /// Content
        /// </summary>
        public Button Singular_ClrProp_RW
        {
            get { return _fieldfor_Singular_ClrProp_RW; }
            set { _fieldfor_Singular_ClrProp_RW = value; }
        }
        #endregion Singular_ClrProp_RW

        #region Array_ClrProp_RW
        private Button[] _fieldfor_Array_ClrProp_RW = null;
        /// <summary>
        /// Content
        /// </summary>
        public Button[] Array_ClrProp_RW
        {
            get { return _fieldfor_Array_ClrProp_RW; }
            set { _fieldfor_Array_ClrProp_RW = value; }
        }
        #endregion Array_ClrProp_RW

        #region Array_ClrProp_RO
        private Button[] _fieldfor_Array_ClrProp_RO = null;
        /// <summary>
        /// Content
        /// </summary>
        public Button[] Array_ClrProp_RO
        {
            get { return _fieldfor_Array_ClrProp_RO; }
        }
        #endregion Array_ClrProp_RO

        #region IList_ClrProp_RW_NonNull
        private ArrayList _fieldfor_IList_ClrProp_RW_NonNull = null;
        /// <summary>
        /// Content
        /// </summary>
        public ArrayList IList_ClrProp_RW_NonNull
        {
            get 
            {
                if (_fieldfor_IList_ClrProp_RW_NonNull == null)
                {
                    _fieldfor_IList_ClrProp_RW_NonNull = new ArrayList();
                }
                return _fieldfor_IList_ClrProp_RW_NonNull; 
            }
            set { _fieldfor_IList_ClrProp_RW_NonNull = value; }
        }
        #endregion IList_ClrProp_RW_NonNull

        #region IList_ClrProp_RO_NonNull
        private ArrayList _fieldfor_IList_ClrProp_RO_NonNull = null;
        /// <summary>
        /// Content
        /// </summary>
        public ArrayList IList_ClrProp_RO_NonNull
        {
            get
            {
                if (_fieldfor_IList_ClrProp_RO_NonNull == null)
                {
                    _fieldfor_IList_ClrProp_RO_NonNull = new ArrayList();
                }
                return _fieldfor_IList_ClrProp_RO_NonNull;
            }
        }
        #endregion IList_ClrProp_RO_NonNull

        #region IList_ClrProp_RW_Null
        private ArrayList _fieldfor_IList_ClrProp_RW_Null = null;
        /// <summary>
        /// Content
        /// </summary>
        public ArrayList IList_ClrProp_RW_Null
        {
            get
            {
                return _fieldfor_IList_ClrProp_RW_Null;
            }
            set { _fieldfor_IList_ClrProp_RW_Null = value; }
        }
        #endregion IList_ClrProp_RW_Null

        #region IList_ClrProp_RO_Null
        private ArrayList _fieldfor_IList_ClrProp_RO_Null = null;
        /// <summary>
        /// Content
        /// </summary>
        public ArrayList IList_ClrProp_RO_Null
        {
            get
            {
                return _fieldfor_IList_ClrProp_RO_Null;
            }
        }
        #endregion IList_ClrProp_RO_Null

        #region IDictionary_ClrProp_RW_NonNull
        private Hashtable _fieldfor_IDictionary_ClrProp_RW_NonNull = null;
        /// <summary>
        /// Content
        /// </summary>
        public Hashtable IDictionary_ClrProp_RW_NonNull
        {
            get
            {
                if (_fieldfor_IDictionary_ClrProp_RW_NonNull == null)
                {
                    _fieldfor_IDictionary_ClrProp_RW_NonNull = new Hashtable();
                }
                return _fieldfor_IDictionary_ClrProp_RW_NonNull;
            }
            set { _fieldfor_IDictionary_ClrProp_RW_NonNull = value; }
        }
        #endregion IDictionary_ClrProp_RW_NonNull

        #region IDictionary_ClrProp_RO_NonNull
        private Hashtable _fieldfor_IDictionary_ClrProp_RO_NonNull = null;
        /// <summary>
        /// Content
        /// </summary>
        public Hashtable IDictionary_ClrProp_RO_NonNull
        {
            get
            {
                if (_fieldfor_IDictionary_ClrProp_RO_NonNull == null)
                {
                    _fieldfor_IDictionary_ClrProp_RO_NonNull = new Hashtable();
                }
                return _fieldfor_IDictionary_ClrProp_RO_NonNull;
            }
        }
        #endregion IDictionary_ClrProp_RO_NonNull

        #region IDictionary_ClrProp_RW_Null
        private Hashtable _fieldfor_IDictionary_ClrProp_RW_Null = null;
        /// <summary>
        /// Content
        /// </summary>
        public Hashtable IDictionary_ClrProp_RW_Null
        {
            get
            {
                return _fieldfor_IDictionary_ClrProp_RW_Null;
            }
            set { _fieldfor_IDictionary_ClrProp_RW_Null = value; }
        }
        #endregion IDictionary_ClrProp_RW_Null

        #region IDictionary_ClrProp_RO_Null
        private Hashtable _fieldfor_IDictionary_ClrProp_RO_Null = null;
        /// <summary>
        /// Content
        /// </summary>
        public Hashtable IDictionary_ClrProp_RO_Null
        {
            get
            {
                return _fieldfor_IDictionary_ClrProp_RO_Null;
            }
        }
        #endregion IDictionary_ClrProp_RO_Null
    }

    #endregion class Custom_Clr_With_Properties

    #region class Custom_Clr_With_IAddChild
    /// <summary>
    /// Custom CLR with IAddChild
    /// </summary>
    public class Custom_Clr_With_IAddChild : IAddChild
    {               
        /// <summary>
        /// Default constructor
        /// </summary>    
        public Custom_Clr_With_IAddChild()
        {
            _children = new ArrayList();
        }

        void IAddChild.AddText(string text)
        {
            _children.Add(text);
        }

        void IAddChild.AddChild(object o)
        {
            _children.Add(o);
        }
            
        private ArrayList _children;
        /// <summary>
        /// Children 
        /// </summary>        
        public ArrayList Children            
        {                
            get { return ArrayList.ReadOnly(_children); } 
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
        /// Default constructor
        /// </summary>
        public Custom_Clr_With_IList()
        {
            _children = new ArrayList();
        }

        int IList.Add(object val)
        {
            return _children.Add(val);
        }

        void IList.Clear()
        {
            _children.Clear();
        }

        bool IList.Contains(Object val)
        {
            return _children.Contains(val);
        }

        int IList.IndexOf(object indexItem)
        {
            return _children.IndexOf(indexItem);
        }

        void IList.Insert(int insertIndex, object insertItem)
        {
            _children.Insert(insertIndex, insertItem);
        }

        void IList.Remove(object removeItem)
        {
            _children.Remove(removeItem);
        }

        void IList.RemoveAt(int removeIndex)
        {
            _children.RemoveAt(removeIndex);
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        object IList.this[int itemIndex]
        {
            get { return _children[itemIndex]; }
            set { _children[itemIndex] = value; }
        }

        bool ICollection.IsSynchronized
        {
            get { return _children.IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return _children.SyncRoot; }
        }

        void ICollection.CopyTo(Array destinationArray, int destinationStart)
        {
            _children.CopyTo(destinationArray, destinationStart);
        }

        int ICollection.Count
        {
            get { return _children.Count; }
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        private ArrayList _children;
        /// <summary>
        /// Children 
        /// </summary>        
        public ArrayList Children
        {
            get { return ArrayList.ReadOnly(_children); }
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
        /// Default constructor
        /// </summary>
        public Custom_Clr_With_IDictionary()
        {
            _children = new Hashtable();
            _childrenList = new ArrayList();
        }

        void IDictionary.Add(object key, object value)
        {
            _children.Add(key, value);
            _childrenList.Add(value);
        }

        void IDictionary.Clear()
        {
            _children.Clear();
            _childrenList.Clear();
        }

        bool IDictionary.Contains(object key)
        {
            return _children.Contains(key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        bool IDictionary.IsFixedSize
        {
            get { return _children.IsFixedSize; }
        }

        bool IDictionary.IsReadOnly
        {
            get { return _children.IsReadOnly; }
        }

        ICollection IDictionary.Keys
        {
            get { return _children.Keys; }
        }

        void IDictionary.Remove(object key)
        {
            object value = _children[key];
            _childrenList.Remove(value);
            _children.Remove(key);
        }

        ICollection IDictionary.Values
        {
            get { return _children.Values; }
        }

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

        void ICollection.CopyTo(System.Array array, int index)
        {
            _children.CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return _children.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return _children.IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return _children.SyncRoot; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        private Hashtable _children;
        private ArrayList _childrenList;

        /// <summary>
        /// Children 
        /// </summary>        
        public ArrayList Children
        {
            get { return ArrayList.ReadOnly(_childrenList); }
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
        /// Default constructor
        /// </summary>
        public Custom_Clr_With_IAddChild_IList()
        {
            _children = new ArrayList();
        }

        void IAddChild.AddText(string text)
        {
            _children.Add(text);
        }

        void IAddChild.AddChild(object o)
        {
            _children.Add(o);
        }

        int IList.Add(object value)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        void IList.Clear()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        bool IList.Contains(object value)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        int IList.IndexOf(object value)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        void IList.Insert(int index, object value)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        bool IList.IsFixedSize
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        bool IList.IsReadOnly
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        void IList.Remove(object value)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        void IList.RemoveAt(int index)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

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

        void ICollection.CopyTo(System.Array array, int index)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        int ICollection.Count
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        bool ICollection.IsSynchronized
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        object ICollection.SyncRoot
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        private ArrayList _children;
        /// <summary>
        /// Children 
        /// </summary>        
        public ArrayList Children
        {
            get { return ArrayList.ReadOnly(_children); }
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
        /// Default constructor
        /// </summary>    
        public Custom_Clr_With_IAddChild_IDictionary()
        {
            _children = new ArrayList();
        }

        void IAddChild.AddText(string text)
        {
            _children.Add(text);
        }

        void IAddChild.AddChild(object o)
        {
            _children.Add(o);
        }

        void IDictionary.Add(object key, object value)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        void IDictionary.Clear()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        bool IDictionary.Contains(object key)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        bool IDictionary.IsFixedSize
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        bool IDictionary.IsReadOnly
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        ICollection IDictionary.Keys
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        void IDictionary.Remove(object key)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        ICollection IDictionary.Values
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

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

        void ICollection.CopyTo(System.Array array, int index)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        int ICollection.Count
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        bool ICollection.IsSynchronized
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        object ICollection.SyncRoot
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        private ArrayList _children;
        /// <summary>
        /// Children 
        /// </summary>        
        public ArrayList Children
        {
            get { return ArrayList.ReadOnly(_children); }
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
        /// Default constructor
        /// </summary>    
        public Custom_Clr_With_IAddChild_CPA()
        {
            _children = new ArrayList();
        }

        void IAddChild.AddText(string text)
        {
            _children.Add(text);
        }

        void IAddChild.AddChild(object o)
        {
            _children.Add(o);
        }

        /// <summary>
        /// Content
        /// </summary>
        public ArrayList Content
        {
            get { return ArrayList.ReadOnly(new ArrayList()); }
        }

        private ArrayList _children;
        /// <summary>
        /// Children 
        /// </summary>        
        public ArrayList Children
        {
            get { return ArrayList.ReadOnly(_children); }
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
        /// Default constructor
        /// </summary>    
        public Custom_Clr_With_CPA_Singular_ClrProp_RO()
        {
            _children = new ArrayList();
        }

        private Button _content = null;
        /// <summary>
        /// Content
        /// </summary>
        public Button Content
        {
            get { return _content; }
        }

        private ArrayList _children;        
        /// <summary>
        /// Children 
        /// </summary>        
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
        /// <summary>
        /// Default constructor
        /// </summary>    
        public Custom_Clr_With_CPA_Singular_ClrProp_RW()
        {
            _children = new ArrayList();
        }

        private Button _content = null;
        /// <summary>
        /// Content
        /// </summary>
        public Button Content
        {
            get { return _content;  }
            set { _content = value; }
        }

        private ArrayList _children;
        /// <summary>
        /// Children 
        /// </summary>        
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
        /// <summary>
        /// Default constructor
        /// </summary>
        public Custom_Clr_With_CPA_IList_ClrProp_RO()
        {
            _content = new ArrayList();
        }

        private ArrayList _content;
        /// <summary>
        /// Content
        /// </summary>
        public ArrayList Content
        {
            get { return _content;  }
        }

        /// <summary>
        /// Children 
        /// </summary>        
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
        /// <summary>
        /// Default constructor
        /// </summary>
        public Custom_Clr_With_CPA_IList_ClrProp_RW()
        {
            _content = new ArrayList();
        }

        private ArrayList _content;
        /// <summary>
        /// Content
        /// </summary>
        public ArrayList Content
        {
            get { return _content; }
            set { _content = value; }
        }

        /// <summary>
        /// Children 
        /// </summary>        
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
        private string _culture;

        /// <summary>
        /// Get/Set the CultureInfo 
        /// </summary>
        public string Culture
        {
            get
            {
                return _culture;
            }

            set
            {
                _culture = value;
            }
        }
    }
    #endregion class Custom_Clr_Accepting_XmlLang

    #region class Custom_DO
    /// <summary>
    /// Custom DO
    /// </summary>
    public class Custom_DO : DependencyObject
    {
    }
    #endregion class Custom_DO

    #region class Custom_DO_With_IAddChild
    /// <summary>
    /// Custom DO with IAddChild
    /// </summary>
    public class Custom_DO_With_IAddChild : DependencyObject, IAddChild
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Custom_DO_With_IAddChild()
        {
            _children = new ArrayList();
        }

        void IAddChild.AddText(string text)
        {
            _children.Add(text);
        }

        void IAddChild.AddChild(object o)
        {
            _children.Add(o);
        }

        private ArrayList _children;
        /// <summary>
        /// Children 
        /// </summary>        
        public ArrayList Children
        {
            get { return ArrayList.ReadOnly(_children); }
        }
    }
    #endregion class Custom_DO_With_IAddChild

    #region class Custom_DO_With_IList
    /// <summary>
    /// Custom DO with IList. 
    /// </summary>
    public class Custom_DO_With_IList : DependencyObject, IList
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Custom_DO_With_IList()
        {
            _children = new ArrayList();
        }

        int IList.Add(object val)
        {
            return _children.Add(val);
        }

        void IList.Clear()
        {
            _children.Clear();
        }

        bool IList.Contains(Object val)
        {
            return _children.Contains(val);
        }

        int IList.IndexOf(object indexItem)
        {
            return _children.IndexOf(indexItem);
        }

        void IList.Insert(int insertIndex, object insertItem)
        {
            _children.Insert(insertIndex, insertItem);
        }

        void IList.Remove(object removeItem)
        {
            _children.Remove(removeItem);
        }

        void IList.RemoveAt(int removeIndex)
        {
            _children.RemoveAt(removeIndex);
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        object IList.this[int itemIndex]
        {
            get { return _children[itemIndex]; }
            set { _children[itemIndex] = value; }
        }

        bool ICollection.IsSynchronized
        {
            get { return _children.IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return _children.SyncRoot; }
        }

        void ICollection.CopyTo(Array destinationArray, int destinationStart)
        {
            _children.CopyTo(destinationArray, destinationStart);
        }

        int ICollection.Count
        {
            get { return _children.Count; }
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        private ArrayList _children;
        /// <summary>
        /// Children 
        /// </summary>        
        public ArrayList Children
        {
            get { return ArrayList.ReadOnly(_children); }
        }
    }
    #endregion class Custom_DO_With_IList

    #region class Custom_DO_With_IDictionary
    /// <summary>
    /// Custom DO with IDictionary
    /// </summary>
    public class Custom_DO_With_IDictionary : DependencyObject, IDictionary
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Custom_DO_With_IDictionary()
        {
            _children = new Hashtable();
            _childrenList = new ArrayList();
        }

        void IDictionary.Add(object key, object value)
        {
            _children.Add(key, value);
            _childrenList.Add(value);
        }

        void IDictionary.Clear()
        {
            _children.Clear();
            _childrenList.Clear();
        }

        bool IDictionary.Contains(object key)
        {
            return _children.Contains(key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        bool IDictionary.IsFixedSize
        {
            get { return _children.IsFixedSize; }
        }

        bool IDictionary.IsReadOnly
        {
            get { return _children.IsReadOnly; }
        }

        ICollection IDictionary.Keys
        {
            get { return _children.Keys; }
        }

        void IDictionary.Remove(object key)
        {
            object value = _children[key];
            _childrenList.Remove(value);
            _children.Remove(key);
        }

        ICollection IDictionary.Values
        {
            get { return _children.Values; }
        }

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

        void ICollection.CopyTo(System.Array array, int index)
        {
            _children.CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return _children.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return _children.IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return _children.SyncRoot; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        private Hashtable _children;
        private ArrayList _childrenList;

        /// <summary>
        /// Children 
        /// </summary>        
        public ArrayList Children
        {
            get { return ArrayList.ReadOnly(_childrenList); }
        }
    }
    #endregion class Custom_DO_With_IDictionary

    #region class Custom_DO_With_IAddChild_IList
    /// <summary>
    /// Custom DO with both IAddChild and IList.
    /// Even though IList is implemented, all of IList members
    /// throw exceptions. This is to ensure that IAddChild is used (takes
    /// precedence) if both IAddChild and IList are implemented.
    /// </summary>
    public class Custom_DO_With_IAddChild_IList : DependencyObject, IAddChild, IList
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Custom_DO_With_IAddChild_IList()
        {
            _children = new ArrayList();
        }

        void IAddChild.AddText(string text)
        {
            _children.Add(text);
        }

        void IAddChild.AddChild(object o)
        {
            _children.Add(o);
        }

        int IList.Add(object value)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        void IList.Clear()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        bool IList.Contains(object value)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        int IList.IndexOf(object value)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        void IList.Insert(int index, object value)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        bool IList.IsFixedSize
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        bool IList.IsReadOnly
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        void IList.Remove(object value)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        void IList.RemoveAt(int index)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

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

        void ICollection.CopyTo(System.Array array, int index)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        int ICollection.Count
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        bool ICollection.IsSynchronized
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        object ICollection.SyncRoot
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        private ArrayList _children;
        /// <summary>
        /// Children 
        /// </summary>        
        public ArrayList Children
        {
            get { return ArrayList.ReadOnly(_children); }
        }
    }
    #endregion class Custom_DO_With_IAddChild_IList

    #region class Custom_DO_With_IAddChild_IDictionary
    /// <summary>
    /// Custom DO with IAddChild and IDictionary.
    /// Even though IDictionary is implemented, all of IDictionary members
    /// throw exceptions. This is to ensure that IAddChild is used (takes
    /// precedence) if both IAddChild and IDictionary are implemented.
    /// </summary>
    public class Custom_DO_With_IAddChild_IDictionary : DependencyObject, IAddChild, IDictionary
    {
        /// <summary>
        /// Default constructor
        /// </summary>    
        public Custom_DO_With_IAddChild_IDictionary()
        {
            _children = new ArrayList();
        }

        void IAddChild.AddText(string text)
        {
            _children.Add(text);
        }

        void IAddChild.AddChild(object o)
        {
            _children.Add(o);
        }

        void IDictionary.Add(object key, object value)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        void IDictionary.Clear()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        bool IDictionary.Contains(object key)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        bool IDictionary.IsFixedSize
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        bool IDictionary.IsReadOnly
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        ICollection IDictionary.Keys
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        void IDictionary.Remove(object key)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        ICollection IDictionary.Values
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

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

        void ICollection.CopyTo(System.Array array, int index)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        int ICollection.Count
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        bool ICollection.IsSynchronized
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        object ICollection.SyncRoot
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        private ArrayList _children;
        /// <summary>
        /// Children 
        /// </summary>        
        public ArrayList Children
        {
            get { return ArrayList.ReadOnly(_children); }
        }
    }
    #endregion class Custom_DO_With_IAddChild_IDictionary

    #region class Custom_DO_With_IAddChild_CPA
    /// <summary>
    /// Custom DO with IAddChild and CPA.
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
        /// Default constructor
        /// </summary>    
        public Custom_DO_With_IAddChild_CPA()
        {
            _children = new ArrayList();
        }

        void IAddChild.AddText(string text)
        {
            _children.Add(text);
        }

        void IAddChild.AddChild(object o)
        {
            _children.Add(o);
        }

        /// <summary>
        /// Content
        /// </summary>
        public ArrayList Content
        {
            get { return ArrayList.ReadOnly(new ArrayList()); }
        }

        private ArrayList _children;
        /// <summary>
        /// Children 
        /// </summary>        
        public ArrayList Children
        {
            get { return ArrayList.ReadOnly(_children); }
        }
    }
    #endregion class Custom_DO_With_IAddChild_CPA

    #region class Custom_DO_With_CPA_Singular_ClrProp_RO
    /// <summary>
    /// Custom DO with a CPA that points to a Singular RO Clr property.
    /// </summary>
    [ContentProperty("Content")]
    public class Custom_DO_With_CPA_Singular_ClrProp_RO : DependencyObject
    {
        /// <summary>
        /// Default constructor
        /// </summary>    
        public Custom_DO_With_CPA_Singular_ClrProp_RO()
        {
            _children = new ArrayList();
        }

        private Button _content = null;
        /// <summary>
        /// Content
        /// </summary>
        public Button Content
        {
            get { return _content; }
        }

        private ArrayList _children;
        /// <summary>
        /// Children 
        /// </summary>        
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
    /// Custom DO with a CPA that points to a Singular RW Clr property.
    /// </summary>
    [ContentProperty("Content")]
    public class Custom_DO_With_CPA_Singular_ClrProp_RW : DependencyObject
    {
        /// <summary>
        /// Default constructor
        /// </summary>    
        public Custom_DO_With_CPA_Singular_ClrProp_RW()
        {
            _children = new ArrayList();
        }

        private Button _content = null;
        /// <summary>
        /// Content
        /// </summary>
        public Button Content
        {
            get { return _content; }
            set { _content = value; }
        }

        private ArrayList _children;
        /// <summary>
        /// Children 
        /// </summary>        
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
    /// Custom DO with a CPA that points to a Singular RW Dependency property.
    /// </summary>
    [ContentProperty("Content")]
    public class Custom_DO_With_CPA_Singular_DP_RW : DependencyObject
    {
        /// <summary>
        /// Default constructor
        /// </summary>    
        public Custom_DO_With_CPA_Singular_DP_RW()
        {
            _children = new ArrayList();
        }

        /// <summary>
        /// ContentProperty
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
                DependencyProperty.Register(
                        "Content",
                        typeof(Button),
                        typeof(Custom_DO_With_CPA_Singular_DP_RW),
                        new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Content
        /// </summary>
        public Button Content
        {
            get { return (Button)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        private ArrayList _children;
        /// <summary>
        /// Children 
        /// </summary>        
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
    /// Custom DO with a CPA that points to a RO Dependency property
    /// of a singular type.
    /// </summary>
    [ContentProperty("Content")]
    public class Custom_DO_With_CPA_Singular_DP_RO : DependencyObject
    {
        /// <summary>
        /// Default constructor
        /// </summary>    
        public Custom_DO_With_CPA_Singular_DP_RO()
        {
            _children = new ArrayList();
        }

        private static readonly DependencyPropertyKey s_contentPropertyKey =
            DependencyProperty.RegisterReadOnly(
                        "Content",
                        typeof(Button),
                        typeof(Custom_DO_With_CPA_Singular_DP_RO),
                        new FrameworkPropertyMetadata(new Button())
            );

        /// <summary>
        /// ContentProperty
        /// </summary>
        public static readonly DependencyProperty ContentProperty = s_contentPropertyKey.DependencyProperty;

        /// <summary>
        /// Content
        /// </summary>
        public Button Content
        {
            get
            {
                return (Button)GetValue(ContentProperty);
            }
        }

        private ArrayList _children;
        /// <summary>
        /// Children 
        /// </summary>        
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
    /// Custom DO with a CPA that points to a RO CLR property of 
    /// a type that implements IList.
    /// </summary>
    [ContentProperty("Content")]
    public class Custom_DO_With_CPA_IList_ClrProp_RO : DependencyObject
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Custom_DO_With_CPA_IList_ClrProp_RO()
        {
            _content = new ArrayList();
        }

        private ArrayList _content;
        /// <summary>
        /// Content
        /// </summary>
        public ArrayList Content
        {
            get { return _content; }
        }

        /// <summary>
        /// Children 
        /// </summary>        
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
    /// Custom DO with a CPA that points to a RW CLR property of 
    /// a type that implements IList.
    /// </summary>
    [ContentProperty("Content")]
    public class Custom_DO_With_CPA_IList_ClrProp_RW : DependencyObject
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Custom_DO_With_CPA_IList_ClrProp_RW()
        {
            _content = new ArrayList();
        }

        private ArrayList _content;
        /// <summary>
        /// Content
        /// </summary>
        public ArrayList Content
        {
            get { return _content; }
            set { _content = value; }
        }

        /// <summary>
        /// Children 
        /// </summary>        
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
    /// Custom DO with a CPA that points to a RW Dependency property
    /// of a type that implements IList.
    /// </summary>
    [ContentProperty("Content")]
    public class Custom_DO_With_CPA_IList_DP_RW : DependencyObject
    {
        /// <summary>
        /// Default constructor
        /// </summary>    
        public Custom_DO_With_CPA_IList_DP_RW()
        {
        }

        /// <summary>
        /// ContentProperty
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
                DependencyProperty.Register(
                        "Content",
                        typeof(ArrayList),
                        typeof(Custom_DO_With_CPA_IList_DP_RW),
                        new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Content
        /// </summary>
        public ArrayList Content
        {
            get 
            { 
                ArrayList content = (ArrayList)GetValue(ContentProperty);
                if (null == content)
                {
                    content = new ArrayList();
                    SetValue(ContentProperty, content);
                }
                return content;
            }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Children 
        /// </summary>        
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
    /// Custom DO with a CPA that points to a RO Dependency property
    /// of a type that implements IList.
    /// </summary>
    [ContentProperty("Content")]
    public class Custom_DO_With_CPA_IList_DP_RO : DependencyObject
    {
        /// <summary>
        /// Default constructor
        /// </summary>    
        public Custom_DO_With_CPA_IList_DP_RO()
        {
        }

        private static readonly DependencyPropertyKey s_contentPropertyKey =
            DependencyProperty.RegisterReadOnly(
                        "Content",
                        typeof(ArrayList),
                        typeof(Custom_DO_With_CPA_IList_DP_RO),
                        new FrameworkPropertyMetadata(new ArrayList())
            );

        /// <summary>
        /// ContentProperty
        /// </summary>
        public static readonly DependencyProperty ContentProperty = s_contentPropertyKey.DependencyProperty;

        /// <summary>
        /// Content
        /// </summary>
        public ArrayList Content
        {
            get
            {
                ArrayList content = (ArrayList)GetValue(ContentProperty);
                return content;
            }
        }

        /// <summary>
        /// Children 
        /// </summary>        
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
    /// Custom DO with a property marked with [XmlLang] attribute, 
    /// thus accepts xml:lang attribute on it in XAML
    /// </summary>
        [XmlLangProperty("Culture")]
    public class Custom_DO_Accepting_XmlLang : DependencyObject
    {
        private CultureInfo _culture;

        /// <summary>
        /// Get/Set the CultureInfo 
        /// </summary>
        public CultureInfo Culture
        {
            get
            {
                return _culture;
            }

            set
            {
                _culture = value;
            }
        }
    }
    #endregion class Custom_DO_Accepting_XmlLang

    #region Custom_DO_With_Properties
    /// <summary>
    /// A custom DO with various properties.
    /// </summary>
    public class Custom_DO_With_Properties : DependencyObject
    {
        #region PrivateProperty
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty PrivatePropertyProperty =
                DependencyProperty.Register(
                        "PrivateProperty",
                        typeof(object),
                        typeof(Custom_DO_With_Properties),
                        new FrameworkPropertyMetadata(null)
            );

        private object PrivateProperty
        {
            get { return GetValue(PrivatePropertyProperty); }
            set { SetValue(PrivatePropertyProperty, value); }
        }
        #endregion PrivateProperty

        #region InternalProperty
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty InternalPropertyProperty =
                DependencyProperty.Register(
                        "InternalProperty",
                        typeof(object),
                        typeof(Custom_DO_With_Properties),
                        new FrameworkPropertyMetadata(null)
            );

        internal object InternalProperty
        {
            get { return GetValue(InternalPropertyProperty); }
            set { SetValue(InternalPropertyProperty, value); }
        }
        #endregion InternalProperty

        #region NonPublicSetterProperty
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty NonPublicSetterPropertyProperty =
                DependencyProperty.Register(
                        "NonPublicSetterProperty",
                        typeof(object),
                        typeof(Custom_DO_With_Properties),
                        new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// 
        /// </summary>
        public object NonPublicSetterProperty
        {
            get { return GetValue(NonPublicSetterPropertyProperty); }
            internal set { SetValue(NonPublicSetterPropertyProperty, value); }
        }
        #endregion NonPublicSetterProperty

        #region InternalTypeConverterProperty
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty InternalTypeConverterPropertyProperty =
                DependencyProperty.Register(
                        "InternalTypeConverterProperty",
                        typeof(Custom_Clr),
                        typeof(Custom_DO_With_Properties),
                        new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// 
        /// </summary>
        [TypeConverter(typeof(InternalTypeConverterForCustomClr))]
        public Custom_Clr InternalTypeConverterProperty
        {
            get { return (Custom_Clr)GetValue(InternalTypeConverterPropertyProperty); }
            set { SetValue(InternalTypeConverterPropertyProperty, value); }
        }
        #endregion InternalTypeConverterProperty

        #region Singular_DP_RO
        private static readonly DependencyPropertyKey s_singular_DP_RO_Property_Key =
            DependencyProperty.RegisterReadOnly(
                        "Singular_DP_RO_Property",
                        typeof(Button),
                        typeof(Custom_DO_With_Properties),
                        new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// DependencyProperty
        /// </summary>
        public static readonly DependencyProperty Singular_DP_RO_Property = s_singular_DP_RO_Property_Key.DependencyProperty;

        /// <summary>
        /// Clr accessor to the DP
        /// </summary>
        public Button Singular_DP_RO
        {
            get { return (GetValue(Singular_DP_RO_Property) as Button); }
        }
        #endregion Singular_DP_RO

        #region Singular_DP_RW
        /// <summary>
        /// DependencyProperty
        /// </summary>
        public static readonly DependencyProperty Singular_DP_RW_Property =
                DependencyProperty.Register(
                        "Singular_DP_RW_Property",
                        typeof(Button),
                        typeof(Custom_DO_With_Properties),
                        new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Clr accessor to the DP
        /// </summary>
        public Button Singular_DP_RW
        {
            get { return (GetValue(Singular_DP_RW_Property) as Button); }
            set { SetValue(Singular_DP_RW_Property, value); }
        }
        #endregion Singular_DP_RW

        #region Array_DP_RO
        private static readonly DependencyPropertyKey s_array_DP_RO_Property_Key =
            DependencyProperty.RegisterReadOnly(
                        "Array_DP_RO_Property",
                        typeof(Button[]),
                        typeof(Custom_DO_With_Properties),
                        new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// DependencyProperty
        /// </summary>
        public static readonly DependencyProperty Array_DP_RO_Property = s_array_DP_RO_Property_Key.DependencyProperty;

        /// <summary>
        /// Clr accessor to the DP
        /// </summary>
        public Button[] Array_DP_RO
        {
            get { return (GetValue(Array_DP_RO_Property) as Button[]); }
        }
        #endregion Array_DP_RO

        #region Array_DP_RW
        /// <summary>
        /// DependencyProperty
        /// </summary>
        public static readonly DependencyProperty Array_DP_RW_Property =
                DependencyProperty.Register(
                        "Array_DP_RW_Property",
                        typeof(Button[]),
                        typeof(Custom_DO_With_Properties),
                        new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Clr accessor to the DP
        /// </summary>
        public Button[] Array_DP_RW
        {
            get { return (GetValue(Array_DP_RW_Property) as Button[]); }
            set { SetValue(Array_DP_RW_Property, value); }
        }
        #endregion Array_DP_RW

        #region IList_DP_RO_Null
        private static readonly DependencyPropertyKey s_IList_DP_RO_Null_Property_Key =
            DependencyProperty.RegisterReadOnly(
                        "IList_DP_RO_Null_Property",
                        typeof(ArrayList),
                        typeof(Custom_DO_With_Properties),
                        new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// DependencyProperty
        /// </summary>
        public static readonly DependencyProperty IList_DP_RO_Null_Property = s_IList_DP_RO_Null_Property_Key.DependencyProperty;

        /// <summary>
        /// Clr accessor to the DP
        /// </summary>
        public ArrayList IList_DP_RO_Null
        {
            get { return (GetValue(IList_DP_RO_Null_Property) as ArrayList); }
        }
        #endregion IList_DP_RO_Null

        #region IList_DP_RO_NonNull
        private static readonly DependencyPropertyKey s_IList_DP_RO_NonNull_Property_Key =
            DependencyProperty.RegisterReadOnly(
                        "IList_DP_RO_NonNull_Property",
                        typeof(ArrayList),
                        typeof(Custom_DO_With_Properties),
                        new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// DependencyProperty
        /// </summary>
        public static readonly DependencyProperty IList_DP_RO_NonNull_Property = s_IList_DP_RO_NonNull_Property_Key.DependencyProperty;

        /// <summary>
        /// Clr accessor to the DP
        /// </summary>
        public ArrayList IList_DP_RO_NonNull
        {
            get 
            {
                if (GetValue(IList_DP_RO_NonNull_Property) == null)
                {
                    SetValue(s_IList_DP_RO_NonNull_Property_Key, new ArrayList());
                }
                return (GetValue(IList_DP_RO_NonNull_Property) as ArrayList); 
            }
        }
        #endregion IList_DP_RO_NonNull

        #region IList_DP_RW_Null
        /// <summary>
        /// DependencyProperty
        /// </summary>
        public static readonly DependencyProperty IList_DP_RW_Null_Property =
                DependencyProperty.Register(
                        "IList_DP_RW_Null_Property",
                        typeof(ArrayList),
                        typeof(Custom_DO_With_Properties),
                        new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Clr accessor to the DP
        /// </summary>
        public ArrayList IList_DP_RW_Null
        {
            get { return (GetValue(IList_DP_RW_Null_Property) as ArrayList); }
            set { SetValue(IList_DP_RW_Null_Property, value); }
        }
        #endregion IList_DP_RW_Null

        #region IList_DP_RW_NonNull
        /// <summary>
        /// DependencyProperty
        /// </summary>
        public static readonly DependencyProperty IList_DP_RW_NonNull_Property =
                DependencyProperty.Register(
                        "IList_DP_RW_NonNull_Property",
                        typeof(ArrayList),
                        typeof(Custom_DO_With_Properties),
                        new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Clr accessor to the DP
        /// </summary>
        public ArrayList IList_DP_RW_NonNull
        {
            get 
            {
                if (GetValue(IList_DP_RW_NonNull_Property) == null)
                {
                    SetValue(IList_DP_RW_NonNull_Property, new ArrayList());
                }
                return (GetValue(IList_DP_RW_NonNull_Property) as ArrayList); 
            }
            set { SetValue(IList_DP_RW_NonNull_Property, value); }
        }
        #endregion IList_DP_RW_NonNull

        #region IDictionary_DP_RO_Null
        private static readonly DependencyPropertyKey s_IDictionary_DP_RO_Null_Property_Key =
            DependencyProperty.RegisterReadOnly(
                        "IDictionary_DP_RO_Null_Property",
                        typeof(Hashtable),
                        typeof(Custom_DO_With_Properties),
                        new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// DependencyProperty
        /// </summary>
        public static readonly DependencyProperty IDictionary_DP_RO_Null_Property = s_IDictionary_DP_RO_Null_Property_Key.DependencyProperty;

        /// <summary>
        /// Clr accessor to the DP
        /// </summary>
        public Hashtable IDictionary_DP_RO_Null
        {
            get { return (GetValue(IDictionary_DP_RO_Null_Property) as Hashtable); }
        }
        #endregion IDictionary_DP_RO_Null

        #region IDictionary_DP_RO_NonNull
        private static readonly DependencyPropertyKey s_IDictionary_DP_RO_NonNull_Property_Key =
            DependencyProperty.RegisterReadOnly(
                        "IDictionary_DP_RO_NonNull_Property",
                        typeof(Hashtable),
                        typeof(Custom_DO_With_Properties),
                        new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// DependencyProperty
        /// </summary>
        public static readonly DependencyProperty IDictionary_DP_RO_NonNull_Property = s_IDictionary_DP_RO_NonNull_Property_Key.DependencyProperty;

        /// <summary>
        /// Clr accessor to the DP
        /// </summary>
        public Hashtable IDictionary_DP_RO_NonNull
        {
            get
            {
                if (GetValue(IDictionary_DP_RO_NonNull_Property) == null)
                {
                    SetValue(s_IDictionary_DP_RO_NonNull_Property_Key, new Hashtable());
                }
                return (GetValue(IDictionary_DP_RO_NonNull_Property) as Hashtable);
            }
        }
        #endregion IDictionary_DP_RO_NonNull

        #region IDictionary_DP_RW_Null
        /// <summary>
        /// DependencyProperty
        /// </summary>
        public static readonly DependencyProperty IDictionary_DP_RW_Null_Property =
                DependencyProperty.Register(
                        "IDictionary_DP_RW_Null_Property",
                        typeof(Hashtable),
                        typeof(Custom_DO_With_Properties),
                        new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Clr accessor to the DP
        /// </summary>
        public Hashtable IDictionary_DP_RW_Null
        {
            get { return (GetValue(IDictionary_DP_RW_Null_Property) as Hashtable); }
            set { SetValue(IDictionary_DP_RW_Null_Property, value); }
        }
        #endregion IDictionary_DP_RW_Null

        #region IDictionary_DP_RW_NonNull
        /// <summary>
        /// DependencyProperty
        /// </summary>
        public static readonly DependencyProperty IDictionary_DP_RW_NonNull_Property =
                DependencyProperty.Register(
                        "IDictionary_DP_RW_NonNull_Property",
                        typeof(Hashtable),
                        typeof(Custom_DO_With_Properties),
                        new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Clr accessor to the DP
        /// </summary>
        public Hashtable IDictionary_DP_RW_NonNull
        {
            get
            {
                if (GetValue(IDictionary_DP_RW_NonNull_Property) == null)
                {
                    SetValue(IDictionary_DP_RW_NonNull_Property, new Hashtable());
                }
                return (GetValue(IDictionary_DP_RW_NonNull_Property) as Hashtable);
            }
            set { SetValue(IDictionary_DP_RW_NonNull_Property, value); }
        }
        #endregion IDictionary_DP_RW_NonNull

        #region DPWithNotSetGet
        /// <summary>
        /// DependencyProperty with not set/get CLR accessor
        /// Used in negative parser tests
        /// </summary>
        public static readonly DependencyProperty DPWithNotSetGetProperty =
                DependencyProperty.Register(
                        "DPWithNotSetGet",
                        typeof(string),
                        typeof(Custom_DO_With_Properties),
                        new FrameworkPropertyMetadata(null)
            );
        #endregion DPWithNotSetGet
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
        private DateTime _dateTime;

        /// <summary>
        /// 
        /// </summary>
        public DateTime DateTimeProperty
        {
            get { return _dateTime; }
            set { _dateTime = value; }
        }
        #endregion DateTimeProperty
    }
    #endregion Custom_DO_With_DateTimeProperty

    #region Custom_ClrProp_Attacher
    /// <summary>
    /// 
    /// </summary>
    public class Custom_ClrProp_Attacher
    {
        #region Singular_ClrProp_RO
        // Property storage.
        private static Hashtable s_fieldfor_Singular_ClrProp_RO = new Hashtable();

        /// <summary>
        /// Static getter
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Button GetSingular_ClrProp_RO(object target)
        {
            return (s_fieldfor_Singular_ClrProp_RO[target] as Button);
        }
        #endregion Singular_ClrProp_RO

        #region Singular_ClrProp_RW
        // Property storage.
        private static Hashtable s_fieldfor_Singular_ClrProp_RW = new Hashtable();

        /// <summary>
        /// Static setter
        /// </summary>
        public static void SetSingular_ClrProp_RW(object target, Button value)
        {
            s_fieldfor_Singular_ClrProp_RW[target] = value;
        }
        /// <summary>
        /// Static getter
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Button GetSingular_ClrProp_RW(object target)
        {
            return (s_fieldfor_Singular_ClrProp_RW[target] as Button);
        }
#endregion Singular_ClrProp_RW

        #region Array_ClrProp_RO
        // Property storage.
        private static Hashtable s_fieldfor_Array_ClrProp_RO = new Hashtable();

        /// <summary>
        /// Static getter
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Button[] GetArray_ClrProp_RO(object target)
        {
            return (s_fieldfor_Array_ClrProp_RO[target] as Button[]);
        }
        #endregion Array_ClrProp_RO

        #region Array_ClrProp_RW
        // Property storage.
        private static Hashtable s_fieldfor_Array_ClrProp_RW = new Hashtable();

        /// <summary>
        /// Static setter
        /// </summary>
        public static void SetArray_ClrProp_RW(object target, Button[] value)
        {
            s_fieldfor_Array_ClrProp_RW[target] = value;
        }
        /// <summary>
        /// Static getter
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Button[] GetArray_ClrProp_RW(object target)
        {
            return (s_fieldfor_Array_ClrProp_RW[target] as Button[]);
        }
        #endregion Array_ClrProp_RW

        #region IList_ClrProp_RO_Null
        // Property storage.
        private static Hashtable s_fieldfor_IList_ClrProp_RO_Null = new Hashtable();

        /// <summary>
        /// Static getter
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static ArrayList GetIList_ClrProp_RO_Null(object target)
        {
            return (s_fieldfor_IList_ClrProp_RO_Null[target] as ArrayList);
        }
        #endregion IList_ClrProp_RO_Null

        #region IList_ClrProp_RO_NonNull
        // Property storage.
        private static Hashtable s_fieldfor_IList_ClrProp_RO_NonNull = new Hashtable();

        /// <summary>
        /// Static getter
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static ArrayList GetIList_ClrProp_RO_NonNull(object target)
        {
            if (s_fieldfor_IList_ClrProp_RO_NonNull[target] == null)
            {
                s_fieldfor_IList_ClrProp_RO_NonNull[target] = new ArrayList();
            }
            return (s_fieldfor_IList_ClrProp_RO_NonNull[target] as ArrayList);
        }
        #endregion IList_ClrProp_RO_NonNull

        #region IList_ClrProp_RW_Null
        // Property storage.
        private static Hashtable s_fieldfor_IList_ClrProp_RW_Null = new Hashtable();

        /// <summary>
        /// Static setter
        /// </summary>
        public static void SetIList_ClrProp_RW_Null(object target, ArrayList value)
        {
            s_fieldfor_IList_ClrProp_RW_Null[target] = value;
        }
        /// <summary>
        /// Static getter
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static ArrayList GetIList_ClrProp_RW_Null(object target)
        {            
            return (s_fieldfor_IList_ClrProp_RW_Null[target] as ArrayList);
        }
        #endregion IList_ClrProp_RW_Null

        #region IList_ClrProp_RW_NonNull
        // Property storage.
        private static Hashtable s_fieldfor_IList_ClrProp_RW_NonNull = new Hashtable();

        /// <summary>
        /// Static setter
        /// </summary>
        public static void SetIList_ClrProp_RW_NonNull(object target, ArrayList value)
        {
            s_fieldfor_IList_ClrProp_RW_NonNull[target] = value;
        }
        /// <summary>
        /// Static getter
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static ArrayList GetIList_ClrProp_RW_NonNull(object target)
        {
            if(s_fieldfor_IList_ClrProp_RW_NonNull[target] == null)
            {
                s_fieldfor_IList_ClrProp_RW_NonNull[target] = new ArrayList();
            }
            return (s_fieldfor_IList_ClrProp_RW_NonNull[target] as ArrayList);
        }
        #endregion IList_ClrProp_RW_NonNull

        #region IDictionary_ClrProp_RO_Null
        // Property storage.
        private static Hashtable s_fieldfor_IDictionary_ClrProp_RO_Null = new Hashtable();

        /// <summary>
        /// Static getter
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Hashtable GetIDictionary_ClrProp_RO_Null(object target)
        {
            return (s_fieldfor_IDictionary_ClrProp_RO_Null[target] as Hashtable);
        }
        #endregion IDictionary_ClrProp_RO_Null

        #region IDictionary_ClrProp_RO_NonNull
        // Property storage.
        private static Hashtable s_fieldfor_IDictionary_ClrProp_RO_NonNull = new Hashtable();

        /// <summary>
        /// Static getter
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Hashtable GetIDictionary_ClrProp_RO_NonNull(object target)
        {
            if (s_fieldfor_IDictionary_ClrProp_RO_NonNull[target] == null)
            {
                s_fieldfor_IDictionary_ClrProp_RO_NonNull[target] = new Hashtable();
            }
            return (s_fieldfor_IDictionary_ClrProp_RO_NonNull[target] as Hashtable);
        }
        #endregion IDictionary_ClrProp_RO_NonNull

        #region IDictionary_ClrProp_RW_Null
        // Property storage.
        private static Hashtable s_fieldfor_IDictionary_ClrProp_RW_Null = new Hashtable();

        /// <summary>
        /// Static setter
        /// </summary>
        public static void SetIDictionary_ClrProp_RW_Null(object target, Hashtable value)
        {
            s_fieldfor_IDictionary_ClrProp_RW_Null[target] = value;
        }
        /// <summary>
        /// Static getter
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Hashtable GetIDictionary_ClrProp_RW_Null(object target)
        {
            return (s_fieldfor_IDictionary_ClrProp_RW_Null[target] as Hashtable);
        }
        #endregion IDictionary_ClrProp_RW_Null

        #region IDictionary_ClrProp_RW_NonNull
        // Property storage.
        private static Hashtable s_fieldfor_IDictionary_ClrProp_RW_NonNull = new Hashtable();

        /// <summary>
        /// Static setter
        /// </summary>
        public static void SetIDictionary_ClrProp_RW_NonNull(object target, Hashtable value)
        {
            s_fieldfor_IDictionary_ClrProp_RW_NonNull[target] = value;
        }
        /// <summary>
        /// Static getter
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Hashtable GetIDictionary_ClrProp_RW_NonNull(object target)
        {
            if (s_fieldfor_IDictionary_ClrProp_RW_NonNull[target] == null)
            {
                s_fieldfor_IDictionary_ClrProp_RW_NonNull[target] = new Hashtable();
            }
            return (s_fieldfor_IDictionary_ClrProp_RW_NonNull[target] as Hashtable);
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
        #region AttachedPrivateProperty
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty AttachedPrivatePropertyProperty = DependencyProperty.RegisterAttached(
            "AttachedPrivateProperty", 
            typeof(object), 
            typeof(Custom_DP_Attacher),
            new FrameworkPropertyMetadata(null)
            );

        private static object GetAttachedPrivateProperty(DependencyObject e)
        {
            return e.GetValue(AttachedPrivatePropertyProperty);
        }

        private static void SetAttachedPrivateProperty(DependencyObject e, object value)
        {
            e.SetValue(AttachedPrivatePropertyProperty, value);
        }
        #endregion AttachedPrivateProperty

        #region AttachedInternalProperty
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty AttachedInternalPropertyProperty = DependencyProperty.RegisterAttached(
            "AttachedInternalProperty",
            typeof(object),
            typeof(Custom_DP_Attacher),
            new FrameworkPropertyMetadata(null)
            );

        internal static object GetAttachedInternalProperty(DependencyObject e)
        {
            return e.GetValue(AttachedInternalPropertyProperty);
        }

        internal static void SetAttachedInternalProperty(DependencyObject e, object value)
        {
            e.SetValue(AttachedInternalPropertyProperty, value);
        }
        #endregion AttachedInternalProperty

        #region AttachedInternalTypeConverterProperty
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty AttachedInternalTypeConverterPropertyProperty = DependencyProperty.RegisterAttached(
            "AttachedInternalTypeConverterProperty",
            typeof(Custom_Clr),
            typeof(Custom_DP_Attacher),
            new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [TypeConverter(typeof(InternalTypeConverterForCustomClr))]
        public static Custom_Clr GetAttachedInternalTypeConverterProperty(DependencyObject e)
        {
            return (Custom_Clr)e.GetValue(AttachedInternalTypeConverterPropertyProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="value"></param>
        public static void SetAttachedInternalTypeConverterProperty(DependencyObject e, Custom_Clr value)
        {
            e.SetValue(AttachedInternalTypeConverterPropertyProperty, value);
        }
        #endregion AttachedInternalTypeConverterProperty

        #region Attached DP with no Static Setters
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty AttachedPropertyWithNoSetGetProperty = DependencyProperty.RegisterAttached(
            "AttachedPropertyWithNoSetGet",
            typeof(string),
            typeof(Custom_DP_Attacher),
            new FrameworkPropertyMetadata(null)
            );

        #endregion Attached DP with no Static Setters

        #region Attached DP with no Static Set, but got Get Accessor
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty AttachedPropertyWithNoSetOnlyGetProperty = DependencyProperty.RegisterAttached(
            "AttachedPropertyWithNoSetOnlyGet",
            typeof(string),
            typeof(Custom_DP_Attacher),
            new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public static object GetAttachedPropertyWithNoSetOnlyGet(DependencyObject e)
        {
            return e.GetValue(AttachedPropertyWithNoSetOnlyGetProperty);
        }

        #endregion Attached DP with no Static Set, but got Get Accessor

        #region Attached DP with Static Set, but no Get Accessor
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty AttachedPropertyWithSetNoGetProperty = DependencyProperty.RegisterAttached(
            "AttachedPropertyWithSetNoGet",
            typeof(string),
            typeof(Custom_DP_Attacher),
            new FrameworkPropertyMetadata(null)
            );
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="value"></param>
        public static void SetAttachedPropertyWithSetNoGetProperty(DependencyObject e, object value)
        {
            e.SetValue(AttachedPropertyWithSetNoGetProperty, value);
        }
        #endregion Attached DP with Static Set, but no Get Accessor

        #region AttachedDateTimeProperty
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty AttachedDateTimePropertyProperty = DependencyProperty.RegisterAttached(
            "AttachedDateTimeProperty",
            typeof(DateTime),
            typeof(Custom_DP_Attacher),
            new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// 
        /// </summary>
        public static DateTime GetAttachedDateTimeProperty(DependencyObject e)
        {
            return (DateTime)e.GetValue(AttachedDateTimePropertyProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void SetAttachedDateTimeProperty(DependencyObject e, DateTime value)
        {
            e.SetValue(AttachedDateTimePropertyProperty, value);
        }
        #endregion AttachedDateTimeProperty
    }
    #endregion Custom_DP_Attacher

    #region InternalTypeConverterForCustomClr
    internal class InternalTypeConverterForCustomClr : TypeConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="td"></param>
        /// <param name="t"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="td"></param>
        /// <param name="t"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="td"></param>
        /// <param name="ci"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext td, System.Globalization.CultureInfo ci, object value)
        {
            return (new Custom_Clr());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return String.Empty;
        }
    }
    #endregion InternalTypeConverterForCustomClr

    #region Custom_Class_With_Events
    /// <summary>
    /// A custom class with events of various access levels, such as 
    /// internal, private etc. Each of them is a CLR accessor to a RoutedEvent.
    /// </summary>
    public class Custom_Class_With_Events : UIElement
    {
        #region PrivateEvent
        /// <summary>
        /// 
        /// </summary>
        public static readonly RoutedEvent PrivateEventEvent = EventManager.RegisterRoutedEvent("PrivateEvent", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(Custom_Class_With_Events));

        private event RoutedEventHandler PrivateEvent 
        { 
            add { AddHandler(PrivateEventEvent, value); } 
            remove { RemoveHandler(PrivateEventEvent, value); } 
        }
        #endregion PrivateEvent

        #region InternalEvent
        /// <summary>
        /// 
        /// </summary>
        public static readonly RoutedEvent InternalEventEvent = EventManager.RegisterRoutedEvent("InternalEvent", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(Custom_Class_With_Events));

        internal event RoutedEventHandler InternalEvent
        {
            add { AddHandler(InternalEventEvent, value); }
            remove { RemoveHandler(InternalEventEvent, value); }
        }
        #endregion InternalEvent
    }
    #endregion Custom_Class_With_Events

    #region Custom_Event_Attacher
    /// <summary>
    /// A custom class declaring attached RoutedEvents with static Add/RemoveHandler
    /// methods of various access levels, such as internal, private etc. 
    /// </summary>
    public class Custom_Event_Attacher : DependencyObject
    {
        #region AttachedPrivateEvent
        /// <summary>
        /// 
        /// </summary>
        public static readonly RoutedEvent AttachedPrivateEventEvent = EventManager.RegisterRoutedEvent("AttachedPrivateEvent", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(Custom_Event_Attacher));

        private static void AddAttachedPrivateEventHandler(DependencyObject d, RoutedEventHandler handler)
        {
            (d as UIElement).AddHandler(AttachedPrivateEventEvent, handler);
        }

        private static void RemoveAttachedPrivateEventHandler(DependencyObject d, RoutedEventHandler handler)
        {
            (d as UIElement).RemoveHandler(AttachedPrivateEventEvent, handler);
        }
        #endregion AttachedPrivateEvent

        #region AttachedInternalEvent
        /// <summary>
        /// 
        /// </summary>
        public static readonly RoutedEvent AttachedInternalEventEvent = EventManager.RegisterRoutedEvent("AttachedInternalEvent", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(Custom_Event_Attacher));

        internal static void AddAttachedInternalEventHandler(DependencyObject d, RoutedEventHandler handler)
        {
            (d as UIElement).AddHandler(AttachedInternalEventEvent, handler);
        }

        internal static void RemoveAttachedInternalEventHandler(DependencyObject d, RoutedEventHandler handler)
        {
            (d as UIElement).RemoveHandler(AttachedInternalEventEvent, handler);
        }
        #endregion AttachedInternalEvent
    }
    #endregion Custom_Event_Attacher

    #region Custom_MarkupExt_With_Private_Ctor
    /// <summary>
    /// A custom Markup Extension with a private constructor
    /// </summary>
    public class Custom_MarkupExt_With_Private_Ctor : MarkupExtension
    {
        private Custom_MarkupExt_With_Private_Ctor(string param1, string param2)
        {
        }

        /// <summary>
        /// ProvideValue.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
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
        internal Custom_MarkupExt_With_Internal_Ctor(string param1, string param2)
        {
        }

        /// <summary>
        /// ProvideValue.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        public static readonly DependencyProperty PrivatePropertyProperty =
                DependencyProperty.Register(
                        "PrivateProperty",
                        typeof(object),
                        typeof(Custom_Class_With_Private_ContentProperty),
                        new FrameworkPropertyMetadata(null)
            );

        private object PrivateProperty
        {
            get { return GetValue(PrivatePropertyProperty); }
            set { SetValue(PrivatePropertyProperty, value); }
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
        /// 
        /// </summary>
        public static readonly DependencyProperty InternalPropertyProperty =
                DependencyProperty.Register(
                        "InternalProperty",
                        typeof(object),
                        typeof(Custom_Class_With_Internal_ContentProperty),
                        new FrameworkPropertyMetadata(null)
            );

        internal object InternalProperty
        {
            get { return GetValue(InternalPropertyProperty); }
            set { SetValue(InternalPropertyProperty, value); }
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
        /// 
        /// </summary>
        public static readonly DependencyProperty NonPublicSetterPropertyProperty =
                DependencyProperty.Register(
                        "NonPublicSetterProperty",
                        typeof(object),
                        typeof(Custom_Class_With_NonPublicSetter_ContentProperty),
                        new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// 
        /// </summary>
        public object NonPublicSetterProperty
        {
            get { return GetValue(NonPublicSetterPropertyProperty); }
            internal set { SetValue(NonPublicSetterPropertyProperty, value); }
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
        /// 
        /// </summary>
        public Custom_Class_With_NonPublicGetter_ContentProperty()
        {
            _children = new ArrayList();
        }
        #endregion Constructor

        #region NonPublicGetterProperty
        private ArrayList _children;
        /// <summary>
        /// 
        /// </summary>
        public ArrayList NonPublicGetterProperty
        {
            internal get 
            {
                return _children; 
            }

            set { _children = value; }
        }
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
        /// <summary>
        /// Constructor
        /// </summary>
        public Custom_FrameworkElement() : base()
        {
            _logicalChildren = new ArrayList();
            _visualChildren = new VisualCollection(this /*parent*/);
        }

        /// <summary>
        /// Adds the given child as either logical child or visual child or both.
        /// </summary>
        /// <param name="child"></param>
        /// <param name="logical"></param>
        /// <param name="visual"></param>
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
        /// <param name="child"></param>
        /// <param name="logical"></param>
        /// <param name="visual"></param>
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
        /// Get an enumerator for logical children.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get { return _logicalChildren.GetEnumerator(); }
        }

        /// <summary>
        ///  Derived classes override this property to enable the Visual code to enumerate 
        ///  the Visual children. Derived classes need to return the number of children
        ///  from this method.
        ///
        ///  By default a Visual does not have any children.
        /// </summary>        
        protected override int VisualChildrenCount
        {
            get
            {
                //_visualChildren cannot be null as its initialized in the constructor
                return _visualChildren.Count;
            }
        }

        /// <summary>
        ///   Derived class must implement to support Visual children. The method must return
        ///    the child at the specified index. Index must be between 0 and GetVisualChildrenCount-1.
        ///
        ///   By default a Visual does not have any children.
        /// </summary>
        protected override Visual GetVisualChild(int index)
        {
            //_visualChildren cannot be null as its initialized in the constructor
            // index range check done by VisualCollection        
            return _visualChildren[index];
        }

        private ArrayList _logicalChildren;
        private VisualCollection _visualChildren;
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
        /// Constructor
        /// </summary>
        public Custom_FrameworkElement_With_IContentHost()
            : base()
        {
        }

        #region IContentHost implementation

        /// <summary>
        ///     Hit tests to the correct ContentElement 
        ///     within the ContentHost that the mouse 
        ///     is over
        /// </summary>
        /// <param name="p">
        ///     Mouse coordinates relative to 
        ///     the ContentHost
        /// </param>
        IInputElement IContentHost.InputHitTest(Point p)
        {
            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        ReadOnlyCollection<Rect> IContentHost.GetRectangles(ContentElement e) { return null; }

        /// <summary>
        /// Returns elements hosted by the content host as an enumerator class
        /// </summary>
        IEnumerator<IInputElement> IContentHost.HostedElements
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="child" />
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
        /// <summary>
        /// Constructor
        /// </summary>
        public Custom_FrameworkContentElement(): base()
        {
            _logicalChildren = new ArrayList();
        }

        /// <summary>
        /// Adds the given child (as a logical child).
        /// </summary>
        /// <param name="child"></param>
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
        /// <param name="child"></param>
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

        /// <summary>
        /// Get an enumerator for logical children.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get { return _logicalChildren.GetEnumerator(); }
        }

        private ArrayList _logicalChildren;
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
        #region Public constructors
        /// <summary>
        /// Default ctor
        /// </summary>
        public CustomColorBlenderExtension() : base()
        {
            _core = Brushes.Black;
            _additive = Colors.Black;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="core">Core brush</param>
        public CustomColorBlenderExtension(Brush core) : base()
        {
            _core = (null == core ? Brushes.Black : core);
            _additive = Colors.Black;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="additive">Additive color</param>
        public CustomColorBlenderExtension(Color additive)
            : base()
        {
            _core = Brushes.Black;
            _additive = additive; //Color is a struct, so can't be null.
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="core">Core brush</param>
        /// <param name="additive">Additive color</param>
        public CustomColorBlenderExtension(Brush core, Color additive) : base()
        {
            _core = (null == core ? Brushes.Black : core);
            _additive = additive; // Color is a struct, so cannot be null
        }
        #endregion Public constructors

        #region Public methods
        /// <summary>
        /// Return the value of the blended brush
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // Add the additive color and mixers to the core brush
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

        #region Public properties
        /// <summary>
        /// Core brush.
        /// </summary>
        /// <value></value>
        public Brush Core
        {
            get { return _core; }
            set { _core = (null == value ? Brushes.Black : value); }
        }

        /// <summary>
        /// Color to be added to the core brush
        /// </summary>
        /// <value></value>        
        public Color Additive
        {
            get { return _additive; }
            set { _additive = value; } // Color is a struct, so cannot be null
        }

        /// <summary>
        /// Other colors to be added to the core brush.
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

        #region Private variables
        private Brush _core;
        private Color _additive;
        private List<Color> _mixers;
        #endregion Private variables
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
        private XmlContent _content = null;

        /// <summary>
        /// Xml Content
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
    /// XmlContent
    /// </summary>
    public class XmlContent : IXmlSerializable
    {
        /// <summary>
        /// Returns null.  
        /// </summary>        
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
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
        /// <param name="reader"></param>
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

        private string _xml = null;
        /// <summary>
        /// ToString();
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _xml;
        }
    }
    #endregion class XmlContentControl

    #region Custom_InternalButton
    internal class Custom_InternalButton : Button
    {
    }
    #endregion Custom_InternalButton

    #region Custom_Inline
    /// <summary>
    /// A custom class that derives from LineBreak, and allows 
    /// doesn't trim surrounding WS
    /// </summary>
    public class Custom_Inline : Inline
    {
        /// <summary>
        /// Constructor
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
        /// Constructor
        /// </summary>
        public Custom_DO_With_GenericCollection_Properties()
            : base()
        {
            _collection = new CustomGenericCollection();
        }

        /// <summary>
        /// CustomGenericCollection
        /// </summary>
        public CustomGenericCollection CustomGenericCollection
        {
            get { return _collection; }
            set { _collection = value; }
        }

        CustomGenericCollection _collection = null;
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
        /// ctor
        /// </summary>
        public CustomGenericCollection()
        {
        }

        int IList.Add(object value)
        {
            Inline inline = null;

            if (value is string)
                inline = new Run((string)value);
            else if (value is UIElement)
                inline = new InlineUIContainer((UIElement)value);
            else
                inline = value as Inline;

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
        /// Constructor
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
        private String _color;

        /// <summary>
        /// Default constructor
        /// </summary>
        public CustomColor()
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
            String s = value as string;

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
}  
