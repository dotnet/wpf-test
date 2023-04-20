// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows.Data;
using System.Collections;
using System.Collections.Specialized;
using System;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Test.DataServices;



namespace Microsoft.Test.DataServices
{
    public class MyLibraryEnumerable : IEnumerable, INotifyCollectionChanged
    {
        private ArrayList _lib;
        private bool _actionWasSet;
        private bool _changedItemWasSet;
        private bool _indexWasSet;
        private int _numGetEnumeratorCalls;

        public MyLibraryEnumerable(int books)
        {
            NumGetEnumeratorCalls = 0;
            if (books < 0)
                books = 0;

            this._lib = new ArrayList(books);

            for (int i = 0; i < books; i++)
            {
                this._lib.Add(new Book(i));
            }
            _actionWasSet = false;
            _changedItemWasSet = false;
            _indexWasSet = false;
        }

        private NotifyCollectionChangedAction _actionToBePassed;

        public NotifyCollectionChangedAction ActionToBePassed
        {
            get { return _actionToBePassed; }
            set 
            { 
                _actionToBePassed = value;
                _actionWasSet = true;
            }
        }

        private object _changedItemToBePassed;

        public object ChangedItemToBePassed
        {
            get { return _changedItemToBePassed; }
            set 
            { 
                _changedItemToBePassed = value;
                _changedItemWasSet = true;
            }
        }
        private int _indexToBePassed;

        public int IndexToBePassed
        {
            get { return _indexToBePassed; }
            set 
            { 
                _indexToBePassed = value;
                _indexWasSet = true;
            }
        }

        public IEnumerator GetEnumerator()
        {
            NumGetEnumeratorCalls++;
            return this._lib.GetEnumerator();
        }

        public int NumGetEnumeratorCalls
        {
            get { return _numGetEnumeratorCalls; }
            set 
            { 
                _numGetEnumeratorCalls = value;
            }
        }

        public int IndexOf(Book b)
        {
            return this._lib.IndexOf(b);
        }

        public int Count
        {
            get { return this._lib.Count; }
        }

        public void Add(Book b)
        {
            this._lib.Add(b);
            int i = this._lib.IndexOf(b);
            ChooseOverloadRaiseEvent();
        }

        public void Remove(Book b)
        {
            int i = this._lib.IndexOf(b);
            this._lib.Remove(b);
            ChooseOverloadRaiseEvent();
        }

        public void Clear()
        {
            this._lib.Clear();
            ChooseOverloadRaiseEvent();
        }

        public void ChooseOverloadRaiseEvent()
        {
            if (_indexWasSet)
            {
                RaiseCollectionChangedEvent(_actionToBePassed, _changedItemToBePassed, _indexToBePassed);
            }
            else if (_changedItemWasSet)
            {
                RaiseCollectionChangedEvent(_actionToBePassed, _changedItemToBePassed);
            }
            else if (_actionWasSet)
            {
                RaiseCollectionChangedEvent(_actionToBePassed);
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void RaiseCollectionChangedEvent(NotifyCollectionChangedAction action)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action));
            }
        }

        private void RaiseCollectionChangedEvent(NotifyCollectionChangedAction action, object changedItem)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, changedItem));
            }
        }

        private void RaiseCollectionChangedEvent(NotifyCollectionChangedAction action, object changedItem, int index)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, changedItem, index));
            }
        }

    }
}
