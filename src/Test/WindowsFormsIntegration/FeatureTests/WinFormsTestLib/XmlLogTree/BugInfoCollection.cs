// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//    Information Contained Herein is Proprietary and Confidential.       
// 
namespace WFCTestLib.XmlLogTree {
    using System;
    using System.Collections;
    
    
    /// <summary>
    ///     <para>
    ///       A collection that stores <see cref='XmlLogTree.BugInfo'/> objects.
    ///    </para>
    /// </summary>
    /// <seealso cref='XmlLogTree.BugInfoCollection'/>
    [Serializable()]
    public class BugInfoCollection : CollectionBase {
        
        /// <summary>
        ///     <para>
        ///       Initializes a new instance of <see cref='XmlLogTree.BugInfoCollection'/>.
        ///    </para>
        /// </summary>
        public BugInfoCollection() {
        }
        
        /// <summary>
        ///     <para>
        ///       Initializes a new instance of <see cref='XmlLogTree.BugInfoCollection'/> based on another <see cref='XmlLogTree.BugInfoCollection'/>.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///       A <see cref='XmlLogTree.BugInfoCollection'/> from which the contents are copied
        /// </param>
        public BugInfoCollection(BugInfoCollection value) {
            this.AddRange(value);
        }
        
        /// <summary>
        ///     <para>
        ///       Initializes a new instance of <see cref='XmlLogTree.BugInfoCollection'/> containing any array of <see cref='XmlLogTree.BugInfo'/> objects.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///       A array of <see cref='XmlLogTree.BugInfo'/> objects with which to intialize the collection
        /// </param>
        public BugInfoCollection(BugInfo[] value) {
            this.AddRange(value);
        }
        
        /// <summary>
        /// <para>Represents the entry at the specified index of the <see cref='XmlLogTree.BugInfo'/>.</para>
        /// </summary>
        /// <param name='index'><para>The zero-based index of the entry to locate in the collection.</para></param>
        /// <value>
        ///    <para> The entry at the specified index of the collection.</para>
        /// </value>
        /// <exception cref='System.ArgumentOutOfRangeException'><paramref name='index'/> is outside the valid range of indexes for the collection.</exception>
        public BugInfo this[int index] {
            get {
                return ((BugInfo)(List[index]));
            }
            set {
                List[index] = value;
            }
        }
        
        /// <summary>
        ///    <para>Adds a <see cref='XmlLogTree.BugInfo'/> with the specified value to the 
        ///    <see cref='XmlLogTree.BugInfoCollection'/> .</para>
        /// </summary>
        /// <param name='value'>The <see cref='XmlLogTree.BugInfo'/> to add.</param>
        /// <returns>
        ///    <para>The index at which the new element was inserted.</para>
        /// </returns>
        /// <seealso cref='XmlLogTree.BugInfoCollection.AddRange'/>
        public int Add(BugInfo value) {
            return List.Add(value);
        }
        
        /// <summary>
        /// <para>Copies the elements of an array to the end of the <see cref='XmlLogTree.BugInfoCollection'/>.</para>
        /// </summary>
        /// <param name='value'>
        ///    An array of type <see cref='XmlLogTree.BugInfo'/> containing the objects to add to the collection.
        /// </param>
        /// <returns>
        ///   <para>None.</para>
        /// </returns>
        /// <seealso cref='XmlLogTree.BugInfoCollection.Add'/>
        public void AddRange(BugInfo[] value) {
            for (int i = 0; (i < value.Length); i = (i + 1)) {
                this.Add(value[i]);
            }
        }
        
        /// <summary>
        ///     <para>
        ///       Adds the contents of another <see cref='XmlLogTree.BugInfoCollection'/> to the end of the collection.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///    A <see cref='XmlLogTree.BugInfoCollection'/> containing the objects to add to the collection.
        /// </param>
        /// <returns>
        ///   <para>None.</para>
        /// </returns>
        /// <seealso cref='XmlLogTree.BugInfoCollection.Add'/>
        public void AddRange(BugInfoCollection value) {
            for (int i = 0; (i < value.Count); i = (i + 1)) {
                this.Add(value[i]);
            }
        }
        
        /// <summary>
        /// <para>Gets a value indicating whether the 
        ///    <see cref='XmlLogTree.BugInfoCollection'/> contains the specified <see cref='XmlLogTree.BugInfo'/>.</para>
        /// </summary>
        /// <param name='value'>The <see cref='XmlLogTree.BugInfo'/> to locate.</param>
        /// <returns>
        /// <para><see langword='true'/> if the <see cref='XmlLogTree.BugInfo'/> is contained in the collection; 
        ///   otherwise, <see langword='false'/>.</para>
        /// </returns>
        /// <seealso cref='XmlLogTree.BugInfoCollection.IndexOf'/>
        public bool Contains(BugInfo value) {
            return List.Contains(value);
        }
        
        /// <summary>
        /// <para>Copies the <see cref='XmlLogTree.BugInfoCollection'/> values to a one-dimensional <see cref='System.Array'/> instance at the 
        ///    specified index.</para>
        /// </summary>
        /// <param name='array'><para>The one-dimensional <see cref='System.Array'/> that is the destination of the values copied from <see cref='XmlLogTree.BugInfoCollection'/> .</para></param>
        /// <param name='index'>The index in <paramref name='array'/> where copying begins.</param>
        /// <returns>
        ///   <para>None.</para>
        /// </returns>
        /// <exception cref='System.ArgumentException'><para><paramref name='array'/> is multidimensional.</para> <para>-or-</para> <para>The number of elements in the <see cref='XmlLogTree.BugInfoCollection'/> is greater than the available space between <paramref name='arrayIndex'/> and the end of <paramref name='array'/>.</para></exception>
        /// <exception cref='System.ArgumentNullException'><paramref name='array'/> is <see langword='null'/>. </exception>
        /// <exception cref='System.ArgumentOutOfRangeException'><paramref name='arrayIndex'/> is less than <paramref name='array'/>'s lowbound. </exception>
        /// <seealso cref='System.Array'/>
        public void CopyTo(BugInfo[] array, int index) {
            List.CopyTo(array, index);
        }
        
        /// <summary>
        ///    <para>Returns the index of a <see cref='XmlLogTree.BugInfo'/> in 
        ///       the <see cref='XmlLogTree.BugInfoCollection'/> .</para>
        /// </summary>
        /// <param name='value'>The <see cref='XmlLogTree.BugInfo'/> to locate.</param>
        /// <returns>
        /// <para>The index of the <see cref='XmlLogTree.BugInfo'/> of <paramref name='value'/> in the 
        /// <see cref='XmlLogTree.BugInfoCollection'/>, if found; otherwise, -1.</para>
        /// </returns>
        /// <seealso cref='XmlLogTree.BugInfoCollection.Contains'/>
        public int IndexOf(BugInfo value) {
            return List.IndexOf(value);
        }
        
        /// <summary>
        /// <para>Inserts a <see cref='XmlLogTree.BugInfo'/> into the <see cref='XmlLogTree.BugInfoCollection'/> at the specified index.</para>
        /// </summary>
        /// <param name='index'>The zero-based index where <paramref name='value'/> should be inserted.</param>
        /// <param name=' value'>The <see cref='XmlLogTree.BugInfo'/> to insert.</param>
        /// <returns><para>None.</para></returns>
        /// <seealso cref='XmlLogTree.BugInfoCollection.Add'/>
        public void Insert(int index, BugInfo value) {
            List.Insert(index, value);
        }
        
        /// <summary>
        ///    <para>Returns an enumerator that can iterate through 
        ///       the <see cref='XmlLogTree.BugInfoCollection'/> .</para>
        /// </summary>
        /// <returns><para>None.</para></returns>
        /// <seealso cref='System.Collections.IEnumerator'/>
        public new BugInfoEnumerator GetEnumerator() {
            return new BugInfoEnumerator(this);
        }
        
        /// <summary>
        ///    <para> Removes a specific <see cref='XmlLogTree.BugInfo'/> from the 
        ///    <see cref='XmlLogTree.BugInfoCollection'/> .</para>
        /// </summary>
        /// <param name='value'>The <see cref='XmlLogTree.BugInfo'/> to remove from the <see cref='XmlLogTree.BugInfoCollection'/> .</param>
        /// <returns><para>None.</para></returns>
        /// <exception cref='System.ArgumentException'><paramref name='value'/> is not found in the Collection. </exception>
        public void Remove(BugInfo value) {
            List.Remove(value);
        }
        
        public class BugInfoEnumerator : object, IEnumerator {
            
            private IEnumerator _baseEnumerator;
            
            private IEnumerable _temp;
            
            public BugInfoEnumerator(BugInfoCollection mappings) {
                this._temp = ((IEnumerable)(mappings));
                this._baseEnumerator = _temp.GetEnumerator();
            }
            
            public BugInfo Current {
                get {
                    return ((BugInfo)(_baseEnumerator.Current));
                }
            }
            
            object IEnumerator.Current {
                get {
                    return _baseEnumerator.Current;
                }
            }
            
            public bool MoveNext() {
                return _baseEnumerator.MoveNext();
            }
            
            bool IEnumerator.MoveNext() {
                return _baseEnumerator.MoveNext();
            }
            
            public void Reset() {
                _baseEnumerator.Reset();
            }
            
            void IEnumerator.Reset() {
                _baseEnumerator.Reset();
            }
        }
    }
}
