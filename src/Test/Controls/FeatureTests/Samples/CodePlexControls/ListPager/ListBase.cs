using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Collections.Specialized;

namespace WpfControlToolkit
{
    public abstract class ListBase<T> : IList<T>, IList
    {
        #region IList<T> Members

        public virtual int IndexOf(T item)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(this[i], item))
                {
                    return i;
                }
            }
            return -1;
        }

        public virtual void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        public virtual void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public T this[int index]
        {
            get
            {
                return GetItem(index);
            }
            set
            {
                SetItem(index, value);
            }
        }

        protected abstract T GetItem(int index);
        protected virtual void SetItem(int index, T value)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region ICollection<T> Members

        public virtual void Add(T item)
        {
            throw new NotSupportedException();
        }

        public virtual void Clear()
        {
            throw new NotSupportedException();
        }

        public virtual bool Contains(T item)
        {
            if (item == null)
            {
                for (int num1 = 0; num1 < this.Count; num1++)
                {
                    if (this[num1] == null)
                    {
                        return true;
                    }
                }
                return false;
            }
            EqualityComparer<T> comparer1 = EqualityComparer<T>.Default;
            for (int num2 = 0; num2 < this.Count; num2++)
            {
                if (comparer1.Equals(this[num2], item))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            CopyTo((Array)array, arrayIndex);
        }

        public abstract int Count
        {
            get;
        }

        public virtual bool IsReadOnly
        {
            get { return true; }
        }

        public virtual bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable<T> Members

        public virtual IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
            {
                yield return this[i];
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IList Members

        int IList.Add(object value)
        {
            VerifyValueType(value);
            this.Add((T)value);
            return (this.Count - 1);
        }

        bool IList.Contains(object value)
        {
            if (IsCompatibleObject(value))
            {
                return this.Contains((T)value);
            }
            return false;
        }

        int IList.IndexOf(object value)
        {
            if (IsCompatibleObject(value))
            {
                return this.IndexOf((T)value);
            }
            return -1;
        }

        void IList.Insert(int index, object value)
        {
            VerifyValueType(value);
            this.Insert(index, (T)value);
        }

        public virtual bool IsFixedSize
        {
            get { return true; }
        }

        void IList.Remove(object value)
        {
            if (IsCompatibleObject(value))
            {
                this.Remove((T)value);
            }
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                if (IsCompatibleObject(value))
                {
                    this[index] = (T)value;
                }
                //
            }
        }

        #endregion

        #region ICollection Members

        public virtual void CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (array.Rank != 1)
            {
                throw new ArgumentException("Array must be 1-dimentional.", "array");
            }
            if (array.Length < (this.Count + index))
            {
                throw new ArgumentOutOfRangeException("index");
            }
            for (int i = 0; i < this.Count; i++)
            {
                array.SetValue(this[i], index + i);
            }
        }

        public virtual bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public virtual object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region helpers
        private static bool IsCompatibleObject(object value)
        {
            if (!(value is T) && ((value != null) || typeof(T).IsValueType))
            {
                return false;
            }
            return true;
        }

        private static void VerifyValueType(object value)
        {
            if (!IsCompatibleObject(value))
            {
                throw new ArgumentException();
            }
        }

        #endregion
    }
}
