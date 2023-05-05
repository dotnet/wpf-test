// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Windows.Data;

namespace Microsoft.Test.DataServices
{

    #region SQLDataSource
// To be used with ObjectDataProvider with Parameters.
// Created to replace test cases that used SQLDataSource
    public class SQLDataSource : DataSet
    {
        public SQLDataSource(string _sql, string _connection, string _table)
        {
            Tables.Add(_table);
            SqlDataAdapter dAdapter = new SqlDataAdapter(_sql, _connection);
            dAdapter.Fill(Tables[0]);
        }


    }
    #endregion

    #region Library ObservableCollection<Book>

    public class Library : ObservableCollection<Book>
    {

        #region Constructors

        public Library(int books, double revision)
        {
            if (books < 0)
                books = 0;

            for (int i = 0; i < books; i++)
            {
                Add(new Book(i + revision));
            }
        }

        // Default Constructor
        public Library() { }

        // Constructor creates specified number of books
        public Library(string books) : this(Int32.Parse(books), 0.0) { }

        // Constructor creates specified number of books with specified revision
        public Library(string books, string revision) : this(Int32.Parse(books), Double.Parse(revision)) { }

        #endregion

        public object SyncRoot { get { return new object(); } }

        #region XmlDocument from items

        /// <summary>
        /// Creates an XmlDocument for all items in the specified range
        /// startAt >= x > endBefore (does not include endBefore)
        /// </summary>
        /// <param name="startAt">First item to create xml</param>
        /// <param name="endBefore">Stop before this index</param>
        /// <returns>XmlDocument of items</returns>
        public XmlDocument ToXmlDocument(int startAt, int endBefore)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Library xmlns=\"\"></Library>");
            XmlNode node;

            for (int i = startAt; i < endBefore; i++)
            {
                node = this[i].ToXml(doc);
                doc.DocumentElement.AppendChild(node);
            }

            return doc;
        }

        /// <summary>
        /// Creates an XmlDocument for all items starting at startAt
        /// </summary>
        /// <param name="startAt">First item to create xml</param>
        /// <returns>XmlDocument of items</returns>
        public XmlDocument ToXmlDocument(int startAt)
        {
            return this.ToXmlDocument(startAt, this.Count);
        }

        /// <summary>
        /// Create an XmlDocument for all items in collection
        /// </summary>
        /// <returns>XmlDocument of items</returns>
        public XmlDocument ToXmlDocument()
        {
            return this.ToXmlDocument(0, this.Count);
        }

        #endregion

        #region Subset of the Library

        public Library Subset(int startAt, int endBefore)
        {
            Library l = new Library();
            for (int i = startAt; i < endBefore; i++)
            {
                l.Add(this[i]);
            }
            return l;
        }

        public Library Subset(int startAt)
        {
            return this.Subset(startAt, this.Count);
        }

        public Library Subset()
        {
            return this;
        }

        #endregion

        #region Sorting
        public void Sort()
        {
            SortItems(0, this.Count, null);
        }

        public void Sort(Comparison<Book> comparison)
        {
            if (comparison == null)
                throw new ArgumentNullException("comparison");

            IComparer<Book> comparer = new ComparerHelper(comparison);
            SortItems(0, this.Count, comparer);
        }

        public void Sort(IComparer<Book> comparer)
        {
            SortItems(0, this.Count, comparer);
        }

        public void Sort(int index, int count, IComparer<Book> comparer)
        {
            SortItems(index, count, comparer);
        }

        protected virtual void SortItems(int index, int count, IComparer<Book> comparer)
        {
            CheckReentrancy();
            if (count > 0)
            {
                ((List<Book>)this.Items).Sort(index, count, comparer);
                OnPropertyChanged(new PropertyChangedEventArgs(Binding.IndexerName));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        private class ComparerHelper : IComparer<Book>
        {
            public ComparerHelper(Comparison<Book> comparison)
            {
                _comparison = comparison;
            }

            public int Compare(Book book1, Book book2)
            {
                return _comparison(book1, book2);
            }

            private Comparison<Book> _comparison;
        }
        #endregion // Sorting

        public void TrimExcess()
        {
            ((List<Book>)this.Items).TrimExcess();
        }
    }

    #endregion

    #region Library as a Stack

    public class LibraryStack : Stack, INotifyCollectionChanged
    {
        #region Constructors

        public LibraryStack(int books, double revision)
        {
            if (books < 0)
                books = 0;

            for (int i = 0; i < books; i++)
            {
                this.Push(new Book(i + revision));
            }
        }

        // Default Constructor
        public LibraryStack() {}

        // Constructor creates specified number of books
        public LibraryStack(string books) : this(Int32.Parse(books), 0.0) { }

        // Constructor creates specified number of books with specified revision
        public LibraryStack(string books, string revision) : this(Int32.Parse(books), Double.Parse(revision)) { }

        #endregion

        public override void Push(object newItem)
        {
            base.Push(newItem);
            RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Add, newItem, 0);
        }

        new public void Pop()
        {
            object item = base.Pop();
            RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Remove, item, 0);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void RaiseCollectionChangedEvent(NotifyCollectionChangedAction action, object item, int index)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item, index));
            }
        }

    }

    #endregion

    #region Library as a Queue

    public class LibraryQueue : Queue, INotifyCollectionChanged
    {

        #region Constructors

        public LibraryQueue(int books, double revision)
        {
            if (books < 0)
                books = 0;

            for (int i = 0; i < books; i++)
            {
                this.Enqueue(new Book(i + revision));
            }
        }

        // Default Constructor
        public LibraryQueue() { }

        // Constructor creates specified number of books
        public LibraryQueue(string books) : this(Int32.Parse(books), 0.0) { }

        // Constructor creates specified number of books with specified revision
        public LibraryQueue(string books, string revision) : this(Int32.Parse(books), Double.Parse(revision)) { }

        #endregion

        public override void Enqueue(object newItem)
        {
            base.Enqueue(newItem);
            RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Add, newItem, Count - 1);
        }

        new public void Dequeue()
        {
            object item = base.Dequeue();
            RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Remove, item, 0);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void RaiseCollectionChangedEvent(NotifyCollectionChangedAction action, object item, int index)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item, index));
            }
        }

    }

    #endregion

    #region Library as a List<T>

    public class LibraryGenericList : List<Book>, INotifyCollectionChanged
    {

        #region Constructors

        public LibraryGenericList(int books, double revision)
        {

            if (books < 0)
                books = 0;

            for (int i = 0; i < books; i++)
            {
                this.Add(new Book(i + revision));
            }
        }

        // Default Constructor
        public LibraryGenericList() { }

        // Constructor creates specified number of books
        public LibraryGenericList(string books) : this(Int32.Parse(books), 0.0) { }

        // Constructor creates specified number of books with specified revision
        public LibraryGenericList(string books, string revision) : this(Int32.Parse(books), Double.Parse(revision)) { }

        #endregion

        new public void Add(Book newItem)
        {
            base.Add(newItem);
            RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Add, newItem, Count - 1);
        }

        new public void Remove(Book item)
        {
            int index = base.IndexOf(item);
            base.Remove(item);
            RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Remove, item, index);
        }

        new public void RemoveAt(int index)
        {
            object item = base[index];
            base.RemoveAt(index);
            RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Remove, item, index);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void RaiseCollectionChangedEvent(NotifyCollectionChangedAction action, object item, int index)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item, index));
            }
        }

    }

    #endregion

    #region Library as a List<T> with Move and Replace

    /// <summary>
    /// This class implements the INotifyCollectionChanged interface, 
    /// and will raise Move and Replace actions
    /// </summary>
    public class LibraryGenericListMR : List<Book>, INotifyCollectionChanged
    {

        #region Constructors

        public LibraryGenericListMR(int books, double revision)
        {

            if (books < 0)
                books = 0;

            for (int i = 0; i < books; i++)
            {
                this.Add(new Book(i + revision));
            }
        }

        // Default Constructor
        public LibraryGenericListMR() { }

        // Constructor creates specified number of books
        public LibraryGenericListMR(string books) : this(Int32.Parse(books), 0.0) { }

        // Constructor creates specified number of books with specified revision
        public LibraryGenericListMR(string books, string revision) : this(Int32.Parse(books), Double.Parse(revision)) { }

        #endregion

        new public void Add(Book newItem)
        {
            base.Add(newItem);
            RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Add, newItem, Count - 1);
        }

        new public void Remove(Book item)
        {
            int index = base.IndexOf(item);
            base.Remove(item);
            RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Remove, item, index);
        }

        new public void RemoveAt(int index)
        {
            object item = base[index];
            base.RemoveAt(index);
            RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Remove, item, index);
        }

        public void Move(int indexFrom, int indexTo)
        {
            Book temp = base[indexFrom];
            base.Remove(temp);
            base.Insert(indexTo, temp);
            RaiseCollectionChangedEventMove(NotifyCollectionChangedAction.Move, temp, indexTo, indexFrom);
        }

        new public Book this[int index]
        {
            get { return base[index]; }
            set
            {
                Book item = value;
                Book oldItem = base[index];
                base[index] = item;
                RaiseCollectionChangedEventReplace(NotifyCollectionChangedAction.Replace, item, oldItem, index);
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void RaiseCollectionChangedEvent(NotifyCollectionChangedAction action, object item, int index)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item, index));
            }
        }

        private void RaiseCollectionChangedEventMove(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
            }
        }

        private void RaiseCollectionChangedEventReplace(NotifyCollectionChangedAction action, object item, object oldItem, int index)
        {
            if (CollectionChanged != null)
            {
                
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item, oldItem, index));
            }
        }

    }

    #endregion
    
    #region Library as IEnumerable

    public class LibraryEnumerable : IEnumerable
    {

        #region Constructors

        private ArrayList _lib;

        public LibraryEnumerable(int books, double revision)
        {

            if (books < 0)
                books = 0;

            _lib = new ArrayList(books);

            for (int i = 0; i < books; i++)
            {
                _lib.Add(new Book(i + revision));
            }
        }

        // Default Constructor
        public LibraryEnumerable()
        {
            _lib = new ArrayList(100);
        }

        // Constructor creates specified number of books
        public LibraryEnumerable(string books) : this(Int32.Parse(books), 0.0) { }

        // Constructor creates specified number of books with specified revision
        public LibraryEnumerable(string books, string revision) : this(Int32.Parse(books), Double.Parse(revision)) { }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return _lib.GetEnumerator();
        }

        #endregion

        public void Add(Book newItem)
        {
            _lib.Add(newItem);
        }

        public void RemoveAt(Book item)
        {
            _lib.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _lib.RemoveAt(index);
        }
    }

    #endregion
    
    #region Library with IEnumerable and INotifyCollectionChanged

    public class LibraryEnumerableNotify : IEnumerable, INotifyCollectionChanged
    {

        #region Constructors

        private ArrayList _lib;

        public LibraryEnumerableNotify(int books, double revision)
        {

            if (books < 0)
                books = 0;

            _lib = new ArrayList(books);

            for (int i = 0; i < books; i++)
            {
                _lib.Add(new Book(i + revision));
            }
        }

        // Default Constructor
        public LibraryEnumerableNotify()
        {
            _lib = new ArrayList(100);
        }


        // Constructor creates specified number of books
        public LibraryEnumerableNotify(string books) : this(Int32.Parse(books), 0.0) { }

        // Constructor creates specified number of books with specified revision
        public LibraryEnumerableNotify(string books, string revision) : this(Int32.Parse(books), Double.Parse(revision)) { }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return _lib.GetEnumerator();
        }

        #endregion

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void RaiseCollectionChangedEvent(NotifyCollectionChangedAction action, object item, int index)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item, index));
            }
        }

        #endregion


        public void Add(Book newItem)
        {
            _lib.Add(newItem);
            RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Add, newItem, _lib.Count - 1);
        }

        public void RemoveAt(Book item)
        {
            int index = _lib.IndexOf(item);
            _lib.Remove(item);
            RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Remove, item, index);
        }

        public void RemoveAt(int index)
        {
            object item = _lib[index];
            _lib.RemoveAt(index);
            RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Remove, item, index);
        }

}

    #endregion

}
