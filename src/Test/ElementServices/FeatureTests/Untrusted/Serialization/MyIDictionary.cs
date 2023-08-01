// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Controls;


namespace Avalon.Test.CoreUI.Serialization
{
	/// <summary>
	/// 
	/// </summary>
	public class MyIDictionary : IDictionary
    {
		/// <summary>
		/// 
		/// </summary>
		public MyIDictionary()
		{
			_base = new Hashtable(10);
		}

		#region IDictionary
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		void IDictionary.Add(object key, object val) 
		{
			((IDictionary)_base).Add(key, val);
		}
		/// <summary>
		/// 
		/// </summary>
		void IDictionary.Clear()
		{
			_base.Clear();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		bool IDictionary.Contains(object key)
		{
			return _base.Contains(key);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return ((IDictionary)_base).GetEnumerator();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		void IDictionary.Remove(object key)
		{
			((IDictionary)_base).Remove(key);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		object IDictionary.this[ object index ]
        {
            get
            {
				return ((IDictionary)_base)[index];
            }
            set
            {
				((IDictionary)_base)[index] = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		ICollection IDictionary.Keys
        {
            get
            {
				return ((IDictionary)_base).Keys;
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		ICollection IDictionary.Values
		{
			get
			{
				return ((IDictionary)_base).Values;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		bool IDictionary.IsReadOnly
		{
			get
			{
				return ((IDictionary)_base).IsReadOnly;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		bool IDictionary.IsFixedSize
		{
			get
			{
				return ((IDictionary)_base).IsFixedSize;
			}
		}
		#endregion IDictionary
		#region ICollection
		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)_base).CopyTo(array, index);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		int ICollection.Count
		{
			get
			{
				return ((ICollection)_base).Count;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		object ICollection.SyncRoot
		{
			get
			{
				return ((ICollection)_base).SyncRoot;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		bool ICollection.IsSynchronized
		{
			get
			{
				return ((ICollection)_base).IsSynchronized;
			}
		}
		#endregion ICollection
		#region IEnumerable
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_base).GetEnumerator();
		}
		#endregion IEnumerable

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		public string PropStr
		{
			get { return _propStr; }
			set { _propStr = value; }
		}


		Hashtable _base;
		string _propStr=null;

	}
}
