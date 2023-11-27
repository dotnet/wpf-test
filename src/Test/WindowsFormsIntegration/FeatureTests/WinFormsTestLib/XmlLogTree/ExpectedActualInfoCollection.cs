// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace WFCTestLib.XmlLogTree
{
	using System;
	using System.Collections;

	//  <summary>
	//  Strongly-typed collection of ExpectedActualInfo objects
	//  </summary>
	//  <remarks></remarks>
	[Serializable()]
	public class ExpectedActualInfoCollection : CollectionBase
	{
		///  <summary>
		///       Initializes a new instance of <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfoCollection"/>.
		///  </summary>
		///  <remarks></remarks>
		public ExpectedActualInfoCollection()
		{
		}

		///  <summary>
		///       Initializes a new instance of <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfoCollection"/> based on another <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfoCollection"/>.
		///  </summary>
		///  <param name="value">
		///       A <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfoCollection"/> from which the contents are copied
		///  </param>
		///  <remarks></remarks>
		public ExpectedActualInfoCollection(ExpectedActualInfoCollection value)
		{
			this.AddRange(value);
		}

		///  <summary>
		///       Initializes a new instance of <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfoCollection"/> containing any array of <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfo"/> objects.
		///  </summary>
		///  <param name="value">
		///       A array of <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfo"/> objects with which to intialize the collection
		///  </param>
		///  <remarks></remarks>
		public ExpectedActualInfoCollection(ExpectedActualInfo[] value)
		{
			this.AddRange(value);
		}

		///  <summary>
		///  Represents the entry at the specified index of the <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfo"/>.
		///  </summary>
		///  <param name="index">The zero-based index of the entry to locate in the collection.</param>
		///  <value>
		///  The entry at the specified index of the collection.
		///  </value>
		///  <remarks><exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> is outside the valid range of indexes for the collection.</exception></remarks>
		public ExpectedActualInfo this[int index]
		{
			get
			{
				return ((ExpectedActualInfo)(List[index]));
			}
			set
			{
				List[index] = value;
			}
		}

		///  <summary>
		///    Adds a <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfo"/> with the specified value to the 
		///    <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfoCollection"/> .
		///  </summary>
		///  <param name="value">The <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfo"/> to add.</param>
		///  <returns>
		///    The index at which the new element was inserted.
		///  </returns>
		///  <remarks><seealso cref="WFCTestLib.XmlLogTree.ExpectedActualInfoCollection.AddRange"/></remarks>
		public int Add(ExpectedActualInfo value)
		{
			return List.Add(value);
		}

		///  <summary>
		///  Copies the elements of an array to the end of the <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfoCollection"/>.
		///  </summary>
		///  <param name="value">
		///    An array of type <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfo"/> containing the objects to add to the collection.
		///  </param>
		///  <remarks><seealso cref="WFCTestLib.XmlLogTree.ExpectedActualInfoCollection.Add"/></remarks>
		public void AddRange(ExpectedActualInfo[] value)
		{
			for (int i = 0; (i < value.Length); i = (i + 1))
			{
				this.Add(value[i]);
			}
		}

		///  <summary>
		///       Adds the contents of another <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfoCollection"/> to the end of the collection.
		///  </summary>
		///  <param name="value">
		///    A <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfoCollection"/> containing the objects to add to the collection.
		///  </param>
		///  <remarks><seealso cref="WFCTestLib.XmlLogTree.ExpectedActualInfoCollection.Add"/></remarks>
		public void AddRange(ExpectedActualInfoCollection value)
		{
			for (int i = 0; (i < value.Count); i = (i + 1))
			{
				this.Add(value[i]);
			}
		}

		///  <summary>
		///  Gets a value indicating whether the 
		///    <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfoCollection"/> contains the specified <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfo"/>.
		///  </summary>
		///  <param name="value">The <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfo"/> to locate.</param>
		///  <returns>
		///  <see langword="true"/> if the <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfo"/> is contained in the collection; 
		///   otherwise, <see langword="false"/>.
		///  </returns>
		///  <remarks><seealso cref="WFCTestLib.XmlLogTree.ExpectedActualInfoCollection.IndexOf"/></remarks>
		public bool Contains(ExpectedActualInfo value)
		{
			return List.Contains(value);
		}

		///  <summary>
		///  Copies the <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfoCollection"/> values to a one-dimensional <see cref="System.Array"/> instance at the 
		///    specified index.
		///  </summary>
		///  <param name="array">The one-dimensional <see cref="System.Array"/> that is the destination of the values copied from <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfoCollection"/> .</param>
		///  <param name="index">The index in <paramref name="array"/> where copying begins.</param>
		///  <remarks><exception cref="System.ArgumentException"><paramref name="array"/> is multidimensional. <para>-or-</para> <para>The number of elements in the <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfoCollection"/> is greater than the available space between <paramref name="arrayIndex"/> and the end of <paramref name="array"/>.</para></exception>
		///  <exception cref="System.ArgumentNullException"><paramref name="array"/> is <see langword="null"/>. </exception>
		///  <exception cref="System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than <paramref name="array"/>"s lowbound. </exception>
		///  <seealso cref="System.Array"/>
		///  </remarks>
		public void CopyTo(ExpectedActualInfo[] array, int index)
		{
			List.CopyTo(array, index);
		}

		///  <summary>
		///    Returns the index of a <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfo"/> in 
		///       the <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfoCollection"/> .
		///  </summary>
		///  <param name="value">The <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfo"/> to locate.</param>
		///  <returns>
		///  The index of the <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfo"/> of <paramref name="value"/> in the 
		///  <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfoCollection"/>, if found; otherwise, -1.
		///  </returns>
		///  <remarks><seealso cref="WFCTestLib.XmlLogTree.ExpectedActualInfoCollection.Contains"/></remarks>
		public int IndexOf(ExpectedActualInfo value)
		{
			return List.IndexOf(value);
		}

		///  <summary>
		///  Inserts a <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfo"/> into the <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfoCollection"/> at the specified index.
		///  </summary>
		///  <param name="index">The zero-based index where <paramref name="value"/> should be inserted.</param>
		///  <param name=" value">The <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfo"/> to insert.</param>
		///  <remarks><seealso cref="WFCTestLib.XmlLogTree.ExpectedActualInfoCollection.Add"/></remarks>
		public void Insert(int index, ExpectedActualInfo value)
		{
			List.Insert(index, value);
		}

		///  <summary>
		///    Returns an enumerator that can iterate through 
		///       the <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfoCollection"/> .
		///  </summary>
		///  <returns>An enumerator for the collection</returns>
		///  <remarks><seealso cref="System.Collections.IEnumerator"/></remarks>
		public new ExpectedActualInfoEnumerator GetEnumerator()
		{
			return new ExpectedActualInfoEnumerator(this);
		}

		///  <summary>
		///     Removes a specific <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfo"/> from the 
		///    <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfoCollection"/> .
		///  </summary>
		///  <param name="value">The <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfo"/> to remove from the <see cref="WFCTestLib.XmlLogTree.ExpectedActualInfoCollection"/> .</param>
		///  <remarks><exception cref="System.ArgumentException"><paramref name="value"/> is not found in the Collection. </exception></remarks>
		public void Remove(ExpectedActualInfo value)
		{
			List.Remove(value);
		}

		public class ExpectedActualInfoEnumerator : object, IEnumerator
		{

			private IEnumerator _baseEnumerator;

			private IEnumerable _temp;

			public ExpectedActualInfoEnumerator(ExpectedActualInfoCollection mappings)
			{
				this._temp = ((IEnumerable)(mappings));
				this._baseEnumerator = _temp.GetEnumerator();
			}

			public ExpectedActualInfo Current
			{
				get
				{
					return ((ExpectedActualInfo)(_baseEnumerator.Current));
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
