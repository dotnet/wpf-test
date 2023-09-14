using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// EventArgs class for the PagedDataRequest event
    /// </summary>
    /// <typeparam name="T">Any type that inherits the IObjectWithChangeTracker interface</typeparam>
    public class PagedDataEventArgs<T> : EventArgs
    {
        internal PagedDataEventArgs(int startPos, int pageSize, int currentSize)
        {
            StartPos = startPos;
            PageSize = pageSize;
            CurrentSize = currentSize;
        }

        /// <summary>
        /// The start page to start retrieving data from
        /// </summary>
        public int StartPos { get; private set; }
        /// <summary>
        /// The size of the pages to retrieve
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// Gets the current size of the paged collection currrently using.
        /// It is suggested, that is this different from the request to recreate the pagecollection
        /// to avoid data corruption.
        /// </summary>
        public int CurrentSize { get; private set; }

        /// <summary>
        /// Set this to the result of the paged request of data
        /// </summary>
        public IEnumerable<T> Result { get; set; }

        /// <summary>
        /// Gets or sets if this request should be cancelled. Should be set to true if the PagedCollection
        /// is recreated due to total record changes.
        /// </summary>
        public bool Cancel { get; set; }
    }

    /// <summary>
    /// PagedCollection enables paged data to limit the traffic of data over the network to be done in chunks instead of 
    /// complete data retrievals. PagedCollection is a read only class to avoid mismatch between the datasource and data in the paged class.
    /// </summary>
    /// <typeparam name="T">Any type that inherits the IObjectWithChangeTracker interface</typeparam>
    public class PagedCollection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerator<T>, IList, ICollection, IEnumerable, IEnumerator, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public event EventHandler<PagedDataEventArgs<T>> PagedDataRequest;

        [NonSerialized]
        private int currentPosInEnumerator = -1;

        [NonSerialized]
        private object syncRoot;
        private IList<T> objItems;
        private int defaultPageSize;

        /// <summary>
        /// Initializes a new instance of PagedCollection that starts with the items passed in items. The size of the collection can be passed in totalSize, and 
        /// default pagesize can be passed in defaultPageSizes
        /// </summary>
        /// <param name="items">The items to initially create the collection with</param>
        /// <param name="totalSize">The total size of the collection, this can be bigger than the size of the items. If not supplied or null, the
        /// size of the PagedCollection equals the size of items</param>
        /// <param name="defaultPageSizes">The default size of a paged request whenever the collection is missing data</param>
        public PagedCollection(IEnumerable<T> items, int? totalSize = null, int? defaultPageSizes = null)
        {
            objItems = new List<T>(items);
            defaultPageSize = defaultPageSizes.GetValueOrDefault(objItems.Count);

            if (objItems.Count < totalSize)
            {
                int startSize = objItems.Count;
                for (int i = 0; i < totalSize - startSize; i++)
                {
                    objItems.Add(default(T));
                }
            }
        }

        /// <summary>
        /// Returns the position of the item supplied in item
        /// </summary>
        /// <param name="item">The item to find the position of</param>
        /// <returns>If found it returns a zero based index of the item in the collection. If not found it returns -1.</returns>
        public int IndexOf(T item)
        {
            if (item == null)
                return -1;

            return IndexOf((object)item);
        }

        /// <summary>
        /// Not supported in the PagedCollection since it is read only. Throws a NotSupportedException
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, T item)
        {
            throw new NotSupportedException("Cannot modify a paged collection.");
        }

        /// <summary>
        /// Not supported in the PagedCollection since it is read only. Throws a NotSupportedException
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            throw new NotSupportedException("Cannot modify a paged collection.");
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                T result = objItems[index];
                if (result == null)
                {
                    GetNewPageFromIndex(index);
                    result = objItems[index];
                }

                return result;
            }
            set
            {
                throw new NotSupportedException("Cannot modify a paged collection.");
            }
        }

        /// <summary>
        /// Not supported in the PagedCollection since it is read only. Throws a NotSupportedException
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            throw new NotSupportedException("Cannot modify a paged collection.");
        }

        /// <summary>
        /// Clears the pages collection for items
        /// </summary>
        public void Clear()
        {
            objItems.Clear();
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Item[]"));
                PropertyChanged(this, new PropertyChangedEventArgs("Count"));
            }
        }

        /// <summary>
        /// Determines whether the PagedCollection contains a specific value.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            if (item == null)
                return false;

            return Contains((object)item);
        }

        /// <summary>
        /// Copies the entire PagedCollection to a compatible one-dimensional Array, starting at the specified index of the target array. 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (objItems.Count == 0)
                return;

            if (arrayIndex < 0 || arrayIndex >= objItems.Count)
                throw new ArgumentException("arrayIndex out of range", "arrayIndex");

            for (int i = arrayIndex; i < arrayIndex + array.Length; i++)
            {
                //if any of the items is null, we need to query for them
                if (objItems[i] == null)
                    GetNewPageFromIndex(i);
                array.SetValue(objItems[i], i - arrayIndex);
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the PagedCollection instance.
        /// </summary>
        public int Count
        {
            get { return objItems.Count; }
        }

        /// <summary>
        /// Returns true to indicate that this is a read only collection
        /// </summary>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Not supported in the PagedCollection since it is read only. Throws a NotSupportedException
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            throw new NotSupportedException("Cannot modify a paged collection.");
        }

        /// <summary>
        /// Returns an enumerator that iterates through the PagedCollection
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            this.Reset();
            return this as IEnumerator<T>;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the PagedCollection
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            this.Reset();
            return this as IEnumerator;
        }

        /// <summary>
        /// Not supported in the PagedCollection since it is read only. Throws a NotSupportedException
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int Add(object value)
        {
            throw new NotSupportedException("Cannot modify a paged collection.");
        }

        /// <summary>
        /// Determines whether the PagedCollection contains a specific value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (!(value is T))
                throw new ArgumentException(string.Format("value must be of type {0}", typeof(T)), "value");

            for (int i = 0; i < objItems.Count; i++)
            {
                if (objItems[i] == null)
                {
                    GetNewPageFromIndex(i);
                }
                if (object.ReferenceEquals(value, objItems[i]) == true)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines the index of a specific item in the PagedCollection.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(object value)
        {
            if (value == null)
                throw new ArgumentNullException("item");

            if (!(value is T))
                throw new ArgumentException(string.Format("value must be of type {0}", typeof(T)), "value");

            return objItems.IndexOf((T)value);
        }

        /// <summary>
        /// Not supported in the PagedCollection since it is read only. Throws a NotSupportedException
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void Insert(int index, object value)
        {
            throw new NotSupportedException("Cannot modify a paged collection.");
        }

        /// <summary>
        /// Returns true since this is a read only collection
        /// </summary>
        public bool IsFixedSize
        {
            get { return true; }
        }

        /// <summary>
        /// Not supported in the PagedCollection since it is read only. Throws a NotSupportedException
        /// </summary>
        /// <param name="value"></param>
        public void Remove(object value)
        {
            throw new NotSupportedException("Cannot modify a paged collection.");
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        object IList.this[int index]
        {
            get
            {
                T result = objItems[index];
                if (result == null)
                {
                    GetNewPageFromIndex(index);
                    result = objItems[index];
                }

                return result;
            }
            set
            {
                throw new NotSupportedException("Cannot modify a paged collection.");
            }
        }

        /// <summary>
        /// Copies the elements of the ICollection to an Array, starting at a particular Array index. 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Array array, int index)
        {
            if (index < 0 || index >= objItems.Count)
                throw new ArgumentException("index out of range", "index");

            for (int i = index; i < index + array.Length; i++)
            {
                //if any of the items is null, we need to query for them
                if (objItems[i] == null)
                    GetNewPageFromIndex(i);
                array.SetValue(objItems[i], i - index);
            }
        }

        /// <summary>
        /// Gets a value indicating whether access to the PagedCollection is synchronized (thread safe).
        /// </summary>
        public bool IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the PagedCollection
        /// </summary>
        public object SyncRoot
        {
            get
            {
                if (this.syncRoot == null)
                {
                    ICollection items = objItems as ICollection;
                    if (items != null)
                    {
                        this.syncRoot = items.SyncRoot;
                    }
                    else
                    {
                        Interlocked.CompareExchange<object>(ref this.syncRoot, new object(), null);
                    }
                }
                return this.syncRoot;
            }
        }

        /// <summary>
        /// Gets the element at the current position of the enumerator.
        /// </summary>
        public T Current
        {
            get
            {
                if (currentPosInEnumerator < 0 || currentPosInEnumerator >= objItems.Count)
                    return default(T);

                return objItems[currentPosInEnumerator];
            }
        }

        public void Dispose()
        {

        }

        /// <summary>
        /// Gets the element at the current position of the enumerator.
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                if (currentPosInEnumerator < 0 || currentPosInEnumerator >= objItems.Count)
                    return null;

                return objItems[currentPosInEnumerator];
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the PagedCollection.
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            currentPosInEnumerator++;

            if (currentPosInEnumerator < 0 || currentPosInEnumerator >= objItems.Count)
                return false;

            if (objItems[currentPosInEnumerator] == null)
            {
                GetNewPageFromIndex(currentPosInEnumerator);
            }

            return true;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the PagedCollection.
        /// </summary>
        public void Reset()
        {
            currentPosInEnumerator = -1;
        }

        /// <summary>
        /// Called from various function inside this collection to retrieve the next available dataresult from the PagedDataRequest event
        /// </summary>
        /// <param name="index">The index where the element that should be retrieved is</param>
        /// <remarks>This function retrieves the current page where index is placed. This is calculated from the default 
        /// page size of this PagedCollection</remarks>
        private void GetNewPageFromIndex(int index)
        {
            if (PagedDataRequest == null)
                throw new InvalidOperationException("The PagedDataRequest event is not subscribed to, the execution cannot continue");

            int startPos = (index - (index % defaultPageSize));

            PagedDataEventArgs<T> eventArgs = new PagedDataEventArgs<T>(startPos, defaultPageSize, objItems.Count);

            PagedDataRequest(this, eventArgs);

            if (eventArgs.Cancel == true)
                return;

            if (eventArgs.Result != null)
            {
                int pos = 0;
                foreach (T item in eventArgs.Result)
                {
                    objItems[startPos + pos] = item;
                    pos++;
                }
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
