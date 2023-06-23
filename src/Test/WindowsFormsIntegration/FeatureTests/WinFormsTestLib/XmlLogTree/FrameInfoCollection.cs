namespace WFCTestLib.XmlLogTree
{
	using System;
	using System.Collections;


	//  Project   : WFCTestLib.XmlLogTree
	//  Class     : FrameInfoCollection
	// 
	//  Copyright (C) 2002, Microsoft Corporation
	// ------------------------------------------------------------------------------
	//  <summary>
	//  Strongly-typed collection of FrameInfo objects
	//  </summary>
	//  <remarks></remarks>
	//  <history>
	//      [dineshc] 12/17/2004  Created
	//  </history>
	[Serializable()]
	public class FrameInfoCollection : CollectionBase
	{

		///  <summary>
		///       Initializes a new instance of <see cref="WFCTestLib.XmlLogTree.FrameInfoCollection"/>.
		///  </summary>
		///  <remarks></remarks>
		///  <history>
		///      [dineshc] 12/17/2004  Created
		///  </history>
		public FrameInfoCollection()
		{
		}

		///  <summary>
		///       Initializes a new instance of <see cref="WFCTestLib.XmlLogTree.FrameInfoCollection"/> based on another <see cref="WFCTestLib.XmlLogTree.FrameInfoCollection"/>.
		///  </summary>
		///  <param name="value">
		///       A <see cref="WFCTestLib.XmlLogTree.FrameInfoCollection"/> from which the contents are copied
		///  </param>
		///  <remarks></remarks>
		///  <history>
		///      [dineshc] 12/17/2004  Created
		///  </history>
		public FrameInfoCollection(FrameInfoCollection value)
		{
			this.AddRange(value);
		}

		///  <summary>
		///       Initializes a new instance of <see cref="WFCTestLib.XmlLogTree.FrameInfoCollection"/> containing any array of <see cref="WFCTestLib.XmlLogTree.FrameInfo"/> objects.
		///  </summary>
		///  <param name="value">
		///       A array of <see cref="WFCTestLib.XmlLogTree.FrameInfo"/> objects with which to intialize the collection
		///  </param>
		///  <remarks></remarks>
		///  <history>
		///      [dineshc] 12/17/2004  Created
		///  </history>
		public FrameInfoCollection(FrameInfo[] value)
		{
			this.AddRange(value);
		}

		///  <summary>
		///  Represents the entry at the specified index of the <see cref="WFCTestLib.XmlLogTree.FrameInfo"/>.
		///  </summary>
		///  <param name="index">The zero-based index of the entry to locate in the collection.</param>
		///  <value>
		///  The entry at the specified index of the collection.
		///  </value>
		///  <remarks><exception cref="System.ArgumentOutOfRangeException"><paramref name="index"/> is outside the valid range of indexes for the collection.</exception></remarks>
		///  <history>
		///      [dineshc] 12/17/2004  Created
		///  </history>
		public FrameInfo this[int index]
		{
			get
			{
				return ((FrameInfo)(List[index]));
			}
			set
			{
				List[index] = value;
			}
		}

		///  <summary>
		///    Adds a <see cref="WFCTestLib.XmlLogTree.FrameInfo"/> with the specified value to the 
		///    <see cref="WFCTestLib.XmlLogTree.FrameInfoCollection"/> .
		///  </summary>
		///  <param name="value">The <see cref="WFCTestLib.XmlLogTree.FrameInfo"/> to add.</param>
		///  <returns>
		///    The index at which the new element was inserted.
		///  </returns>
		///  <remarks><seealso cref="WFCTestLib.XmlLogTree.FrameInfoCollection.AddRange"/></remarks>
		///  <history>
		///      [dineshc] 12/17/2004  Created
		///  </history>
		public int Add(FrameInfo value)
		{
			return List.Add(value);
		}

		///  <summary>
		///  Copies the elements of an array to the end of the <see cref="WFCTestLib.XmlLogTree.FrameInfoCollection"/>.
		///  </summary>
		///  <param name="value">
		///    An array of type <see cref="WFCTestLib.XmlLogTree.FrameInfo"/> containing the objects to add to the collection.
		///  </param>
		///  <remarks><seealso cref="WFCTestLib.XmlLogTree.FrameInfoCollection.Add"/></remarks>
		///  <history>
		///      [dineshc] 12/17/2004  Created
		///  </history>
		public void AddRange(FrameInfo[] value)
		{
			for (int i = 0; (i < value.Length); i = (i + 1))
			{
				this.Add(value[i]);
			}
		}

		///  <summary>
		///     
		///       Adds the contents of another <see cref="WFCTestLib.XmlLogTree.FrameInfoCollection"/> to the end of the collection.
		///    
		///  </summary>
		///  <param name="value">
		///    A <see cref="WFCTestLib.XmlLogTree.FrameInfoCollection"/> containing the objects to add to the collection.
		///  </param>
		///  <remarks><seealso cref="WFCTestLib.XmlLogTree.FrameInfoCollection.Add"/></remarks>
		///  <history>
		///      [dineshc] 12/17/2004  Created
		///  </history>
		public void AddRange(FrameInfoCollection value)
		{
			for (int i = 0; (i < value.Count); i = (i + 1))
			{
				this.Add(value[i]);
			}
		}

		///  <summary>
		///  Gets a value indicating whether the 
		///    <see cref="WFCTestLib.XmlLogTree.FrameInfoCollection"/> contains the specified <see cref="WFCTestLib.XmlLogTree.FrameInfo"/>.
		///  </summary>
		///  <param name="value">The <see cref="WFCTestLib.XmlLogTree.FrameInfo"/> to locate.</param>
		///  <returns>
		///  <see langword="true"/> if the <see cref="WFCTestLib.XmlLogTree.FrameInfo"/> is contained in the collection; 
		///   otherwise, <see langword="false"/>.
		///  </returns>
		///  <remarks><seealso cref="WFCTestLib.XmlLogTree.FrameInfoCollection.IndexOf"/></remarks>
		///  <history>
		///      [dineshc] 12/17/2004  Created
		///  </history>
		public bool Contains(FrameInfo value)
		{
			return List.Contains(value);
		}

		///  <summary>
		///  Copies the <see cref="WFCTestLib.XmlLogTree.FrameInfoCollection"/> values to a one-dimensional <see cref="System.Array"/> instance at the 
		///    specified index.
		///  </summary>
		///  <param name="array">The one-dimensional <see cref="System.Array"/> that is the destination of the values copied from <see cref="WFCTestLib.XmlLogTree.FrameInfoCollection"/> .</param>
		///  <param name="index">The index in <paramref name="array"/> where copying begins.</param>
		///  <remarks><exception cref="System.ArgumentException"><paramref name="array"/> is multidimensional. <para>-or-</para> <para>The number of elements in the <see cref="WFCTestLib.XmlLogTree.FrameInfoCollection"/> is greater than the available space between <paramref name="arrayIndex"/> and the end of <paramref name="array"/>.</para></exception>
		///  <exception cref="System.ArgumentNullException"><paramref name="array"/> is <see langword="null"/>. </exception>
		///  <exception cref="System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than <paramref name="array"/>"s lowbound. </exception>
		///  <seealso cref="System.Array"/>
		///  </remarks>
		///  <history>
		///      [dineshc] 12/17/2004  Created
		///  </history>
		public void CopyTo(FrameInfo[] array, int index)
		{
			List.CopyTo(array, index);
		}

		///  <summary>
		///    Returns the index of a <see cref="WFCTestLib.XmlLogTree.FrameInfo"/> in 
		///       the <see cref="WFCTestLib.XmlLogTree.FrameInfoCollection"/> .
		///  </summary>
		///  <param name="value">The <see cref="WFCTestLib.XmlLogTree.FrameInfo"/> to locate.</param>
		///  <returns>
		///  The index of the <see cref="WFCTestLib.XmlLogTree.FrameInfo"/> of <paramref name="value"/> in the 
		///  <see cref="WFCTestLib.XmlLogTree.FrameInfoCollection"/>, if found; otherwise, -1.
		///  </returns>
		///  <remarks><seealso cref="WFCTestLib.XmlLogTree.FrameInfoCollection.Contains"/></remarks>
		///  <history>
		///      [dineshc] 12/17/2004  Created
		///  </history>
		public int IndexOf(FrameInfo value)
		{
			return List.IndexOf(value);
		}

		///  <summary>
		///  Inserts a <see cref="WFCTestLib.XmlLogTree.FrameInfo"/> into the <see cref="WFCTestLib.XmlLogTree.FrameInfoCollection"/> at the specified index.
		///  </summary>
		///  <param name="index">The zero-based index where <paramref name="value"/> should be inserted.</param>
		///  <param name=" value">The <see cref="WFCTestLib.XmlLogTree.FrameInfo"/> to insert.</param>
		///  <remarks><seealso cref="WFCTestLib.XmlLogTree.FrameInfoCollection.Add"/></remarks>
		///  <history>
		///      [dineshc] 12/17/2004  Created
		///  </history>
		public void Insert(int index, FrameInfo value)
		{
			List.Insert(index, value);
		}

		///  <summary>
		///    Returns an enumerator that can iterate through 
		///       the <see cref="WFCTestLib.XmlLogTree.FrameInfoCollection"/> .
		///  </summary>
		///  <returns>An enumerator for the collection</returns>
		///  <remarks><seealso cref="System.Collections.IEnumerator"/></remarks>
		///  <history>
		///      [dineshc] 12/17/2004  Created
		///  </history>
		public new FrameInfoEnumerator GetEnumerator()
		{
			return new FrameInfoEnumerator(this);
		}

		///  <summary>
		///     Removes a specific <see cref="WFCTestLib.XmlLogTree.FrameInfo"/> from the 
		///    <see cref="WFCTestLib.XmlLogTree.FrameInfoCollection"/> .
		///  </summary>
		///  <param name="value">The <see cref="WFCTestLib.XmlLogTree.FrameInfo"/> to remove from the <see cref="WFCTestLib.XmlLogTree.FrameInfoCollection"/> .</param>
		///  <remarks><exception cref="System.ArgumentException"><paramref name="value"/> is not found in the Collection. </exception></remarks>
		///  <history>
		///      [dineshc] 12/17/2004  Created
		///  </history>
		public void Remove(FrameInfo value)
		{
			List.Remove(value);
		}

		public class FrameInfoEnumerator : object, IEnumerator
		{

			private IEnumerator baseEnumerator;

			private IEnumerable temp;

			public FrameInfoEnumerator(FrameInfoCollection mappings)
			{
				this.temp = ((IEnumerable)(mappings));
				this.baseEnumerator = temp.GetEnumerator();
			}

			public FrameInfo Current
			{
				get
				{
					return ((FrameInfo)(baseEnumerator.Current));
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return baseEnumerator.Current;
				}
			}

			public bool MoveNext()
			{
				return baseEnumerator.MoveNext();
			}

			bool IEnumerator.MoveNext()
			{
				return baseEnumerator.MoveNext();
			}

			public void Reset()
			{
				baseEnumerator.Reset();
			}

			void IEnumerator.Reset()
			{
				baseEnumerator.Reset();
			}
		}
	}
}
