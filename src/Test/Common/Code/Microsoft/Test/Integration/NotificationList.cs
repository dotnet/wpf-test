// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.Integration
{
    /// <summary>
    /// 
    /// </summary>
    public enum CustomListActions
    {
        /// <summary>
        /// 
        /// </summary>
        Add
    }
    
    ///<summary>
    ///</summary>    
    public class CustomList<T> : List<T>
    {

        ///<summary>
        ///</summary>
        new public void Add(T ivg)
        {
            base.Add(ivg);

            CustomListEventArgs args = new CustomListEventArgs();
            args.Action = CustomListActions.Add;
            args.Object = ivg;
            args.CustomList = this;

            OnCollectionChanged(args);
        }

        ///<summary>
        ///</summary>
        new public void AddRange(IEnumerable<T> ivg)
        {
            base.AddRange(ivg);
            OnCollectionChanged();
        }

        ///<summary>
        ///</summary>
        new public void Clear()
        {
            base.Clear();
            OnCollectionChanged();
        }

        ///<summary>
        ///</summary>
        new public void Insert(int index, T ivg)
        {
            base.Insert(index, ivg);
            OnCollectionChanged();
        }

        ///<summary>
        ///</summary>
        new public void InsertRange(int index, IEnumerable<T> ivg)
        {
            base.InsertRange(index, ivg);
            OnCollectionChanged();
        }

        ///<summary>
        ///</summary>
        new public void Remove(T ivg)
        {
            base.Remove(ivg);
            OnCollectionChanged();
        }

        ///<summary>
        ///</summary>
        new public void RemoveAll(Predicate<T> ivg)
        {
            base.RemoveAll(ivg);
            OnCollectionChanged();
        }

        ///<summary>
        ///</summary>
        new public void RemoveAt(int index)
        {
            base.RemoveAt(index);
            OnCollectionChanged();
        }

        ///<summary>
        ///</summary>
        new public void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
            OnCollectionChanged();
        }

        ///<summary>
        ///</summary>
        public event EventHandler CollectionChanged;


        private void OnCollectionChanged()
        {
            OnCollectionChanged(null);
        }

        private void OnCollectionChanged(CustomListEventArgs args)
        {
            EventArgs arguments = EventArgs.Empty;

            if (args != null)
            {
                arguments = args;
            }

            if (CollectionChanged != null)
            {
                CollectionChanged(this, arguments);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CustomListEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public object Object
        {
            get
            {
                return _obj;
            }
            internal set
            {
                _obj = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public object CustomList
        {
            get
            {
                return _customList;
            }
            internal set
            {
                _customList = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public CustomListActions Action
        {
            get
            {
                return _customListAction;
            }
            internal set
            {
                _customListAction = value;
            }
        }

        CustomListActions _customListAction;
        object _obj = null;
        object _customList = null;
    }
}

