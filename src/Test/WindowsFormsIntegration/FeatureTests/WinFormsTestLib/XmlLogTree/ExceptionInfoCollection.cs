// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace WFCTestLib.XmlLogTree
{
	using System;
	using System.Collections;

	//  Project   : WFCTestLib.XmlLogTree
	//  Class     : ExceptionInfoCollection
	// 
	//  <summary>
	//  Strongly-typed collection of ExceptionInfo objects
	//  </summary>
	//  <remarks></remarks>
	[Serializable()]
	public class ExceptionInfoCollection : CollectionBase
	{
		///  <summary>
		///       Initializes a new instance of <see cref="WFCTestLib.XmlLogTree.ExceptionInfoCollection"/>.
		///  </summary>
		///  <remarks></remarks>
		public ExceptionInfoCollection()
		{
		}

		///  <summary>
		///       Initializes a new instance of <see cref="WFCTestLib.XmlLogTree.ExceptionInfoCollection"/> based on another <see cref="WFCTestLib.XmlLogTree.ExceptionInfoCollection"/>.
		///  </summary>
		///  <param name="value">
		///       A <see cref="WFCTestLib.XmlLogTree.ExceptionInfoCollection"/> from which the contents are copied
		///  </param>
		///  <remarks></remarks>
		public ExceptionInfoCollection(ExceptionInfoCollection value)
		{
			this.AddRange(value);
		}

		///  <summary>
		///       Initializes a new instance of <see cref="WFCTestLib.XmlLogTree.ExceptionInfoCollection"/> containing any array of <see cref="WFCTestLib.XmlLogTree.ExceptionInfo"/> objects.
		///  </summary>
		///  <param name="value">
		///       A array of <see cref="WFCTestLib.XmlLogTree.ExceptionInfo"/> objects with which to intialize the collection
		///  </param>
		///  <remarks></remarks>
		public ExceptionInfoCollection(ExceptionInfo[] value)
		{
			this.AddRange(value);
		}

		///  <summary>
		///  Represents the entry at the specified index of the <see cref="WFCTestLib.XmlLogTree.ExceptionInfo"/>.
		///  </summary>
		///  <param name="index">The zero-based index of the entry to locate in the collection.</param>
		///  <value>
		///  The entry at the specified index of the collection.
		///  </value>
		///  <remarks><exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> is outside the valid range of indexes for the collection.</exception></remarks>
		public ExceptionInfo this[int index]
		{
			get
			{
				return ((ExceptionInfo)(List[index]));
			}
			set
			{
				List[index] = value;
			}
		}

		///  <summary>
		///    Adds a <see cref="WFCTestLib.XmlLogTree.ExceptionInfo"/> with the specified value to the 
		///    <see cref="WFCTestLib.XmlLogTree.ExceptionInfoCollection"/> .
		///  </summary>
		///  <param name="value">The <see cref="WFCTestLib.XmlLogTree.ExceptionInfo"/> to add.</param>
		///  <returns>
		///    The index at which the new element was inserted.
		///  </returns>
		///  <remarks><seealso cref="WFCTestLib.XmlLogTree.ExceptionInfoCollection.AddRange"/></remarks>
		public int Add(ExceptionInfo value)
		{
			return List.Add(value);
		}

		///  <summary>
		///  Copies the elements of an array to the end of the <see cref="WFCTestLib.XmlLogTree.ExceptionInfoCollection"/>.
		///  </summary>
		///  <param name="value">
		///    An array of type <see cref="WFCTestLib.XmlLogTree.ExceptionInfo"/> containing the objects to add to the collection.
		///  </param>
		///  <remarks><seealso cref="WFCTestLib.XmlLogTree.ExceptionInfoCollection.Add"/></remarks>
		public void AddRange(ExceptionInfo[] value)
		{
			for (int i = 0; (i < value.Length); i = (i + 1))
			{
				this.Add(value[i]);
			}
		}

		///  <summary>
		///     
		///       Adds the contents of another <see cref="WFCTestLib.XmlLogTree.ExceptionInfoCollection"/> to the end of the collection.
		///    
		///  </summary>
		///  <param name="value">
		///    A <see cref="WFCTestLib.XmlLogTree.ExceptionInfoCollection"/> containing the objects to add to the collection.
		///  </param>
		///  <remarks><seealso cref="WFCTestLib.XmlLogTree.ExceptionInfoCollection.Add"/></remarks>
		public void AddRange(ExceptionInfoCollection value)
		{
			for (int i = 0; (i < value.Count); i = (i + 1))
			{
				this.Add(value[i]);
			}
		}

		///  <summary>
		///  Gets a value indicating whether the 
		///    <see cref="WFCTestLib.XmlLogTree.ExceptionInfoCollection"/> contains the specified <see cref="WFCTestLib.XmlLogTree.ExceptionInfo"/>.
		///  </summary>
		///  <param name="value">The <see cref="WFCTestLib.XmlLogTree.ExceptionInfo"/> to locate.</param>
		///  <returns>
		///  <see langword="true"/> if the <see cref="WFCTestLib.XmlLogTree.ExceptionInfo"/> is contained in the collection; 
		///   otherwise, <see langword="false"/>.
		///  </returns>
		///  <remarks><seealso cref="WFCTestLib.XmlLogTree.ExceptionInfoCollection.IndexOf"/></remarks>
		public bool Contains(ExceptionInfo value)
		{
			return List.Contains(value);
		}

		///  <summary>
		///  Copies the <see cref="WFCTestLib.XmlLogTree.ExceptionInfoCollection"/> values to a one-dimensional <see cref="System.Array"/> instance at the 
		///    specified index.
		///  </summary>
		///  <param name="array">The one-dimensional <see cref="System.Array"/> that is the destination of the values copied from <see cref="WFCTestLib.XmlLogTree.ExceptionInfoCollection"/> .</param>
		///  <param name="index">The index in <paramref name="array"/> where copying begins.</param>
		///  <remarks><exception cref="System.ArgumentException"><paramref name="array"/> is multidimensional. <para>-or-</para> <para>The number of elements in the <see cref="WFCTestLib.XmlLogTree.ExceptionInfoCollection"/> is greater than the available space between <paramref name="arrayIndex"/> and the end of <paramref name="array"/>.</para></exception>
		///  <exception cref="System.ArgumentNullException"><paramref name="array"/> is <see langword="null"/>. </exception>
		///  <exception cref="System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than <paramref name="array"/>"s lowbound. </exception>
		///  <seealso cref="System.Array"/>
		///  </remarks>
		public void CopyTo(ExceptionInfo[] array, int index)
		{
			List.CopyTo(array, index);
		}

		///  <summary>
		///    Returns the index of a <see cref="WFCTestLib.XmlLogTree.ExceptionInfo"/> in 
		///       the <see cref="WFCTestLib.XmlLogTree.ExceptionInfoCollection"/> .
		///  </summary>
		///  <param name="value">The <see cref="WFCTestLib.XmlLogTree.ExceptionInfo"/> to locate.</param>
		///  <returns>
		///  The index of the <see cref="WFCTestLib.XmlLogTree.ExceptionInfo"/> of <paramref name="value"/> in the 
		///  <see cref="WFCTestLib.XmlLogTree.ExceptionInfoCollection"/>, if found; otherwise, -1.
		///  </returns>
		///  <remarks><seealso cref="WFCTestLib.XmlLogTree.ExceptionInfoCollection.Contains"/></remarks>
		public int IndexOf(ExceptionInfo value)
		{
			return List.IndexOf(value);
		}

		///  <summary>
		///  Inserts a <see cref="WFCTestLib.XmlLogTree.ExceptionInfo"/> into the <see cref="WFCTestLib.XmlLogTree.ExceptionInfoCollection"/> at the specified index.
		///  </summary>
		///  <param name="index">The zero-based index where <paramref name="value"/> should be inserted.</param>
		///  <param name=" value">The <see cref="WFCTestLib.XmlLogTree.ExceptionInfo"/> to insert.</param>
		///  <remarks><seealso cref="WFCTestLib.XmlLogTree.ExceptionInfoCollection.Add"/></remarks>
		public void Insert(int index, ExceptionInfo value)
		{
			List.Insert(index, value);
		}

		///  <summary>
		///    Returns an enumerator that can iterate through 
		///       the <see cref="WFCTestLib.XmlLogTree.ExceptionInfoCollection"/> .
		///  </summary>
		///  <returns>An enumerator for the collection</returns>
		///  <remarks><seealso cref="System.Collections.IEnumerator"/></remarks>
		public new ExceptionInfoEnumerator GetEnumerator()
		{
			return new ExceptionInfoEnumerator(this);
		}

		///  <summary>
		///     Removes a specific <see cref="WFCTestLib.XmlLogTree.ExceptionInfo"/> from the 
		///    <see cref="WFCTestLib.XmlLogTree.ExceptionInfoCollection"/> .
		///  </summary>
		///  <param name="value">The <see cref="WFCTestLib.XmlLogTree.ExceptionInfo"/> to remove from the <see cref="WFCTestLib.XmlLogTree.ExceptionInfoCollection"/> .</param>
		///  <remarks><exception cref="System.ArgumentException"><paramref name="value"/> is not found in the Collection. </exception></remarks>
		public void Remove(ExceptionInfo value)
		{
			List.Remove(value);
		}

		public class ExceptionInfoEnumerator : object, IEnumerator
		{

			private IEnumerator _baseEnumerator;

			private IEnumerable _temp;

			public ExceptionInfoEnumerator(ExceptionInfoCollection mappings)
			{
				this._temp = ((IEnumerable)(mappings));
				this._baseEnumerator = _temp.GetEnumerator();
			}

			public ExceptionInfo Current
			{
				get
				{
					return ((ExceptionInfo)(_baseEnumerator.Current));
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return _baseEnumerator.Current;
				}
			}

			public bool MoveNext()
			{
				return _baseEnumerator.MoveNext();
			}

			bool IEnumerator.MoveNext()
			{
				return _baseEnumerator.MoveNext();
			}

			public void Reset()
			{
				_baseEnumerator.Reset();
			}

			void IEnumerator.Reset()
			{
				_baseEnumerator.Reset();
			}
		}
	}
}
