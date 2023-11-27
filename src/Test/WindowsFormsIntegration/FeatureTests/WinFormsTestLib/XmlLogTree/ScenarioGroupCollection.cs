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
    ///       A collection that stores <see cref='LogTree.ScenarioGroup'/> objects.
    ///    </para>
    /// </summary>
    /// <seealso cref='LogTree.ScenarioGroupCollection'/>
    [Serializable()]
    public class ScenarioGroupCollection : CollectionBase {
        TestResult _parent;
        
        /// <summary>
        ///     <para>
        ///       Initializes a new instance of <see cref='LogTree.ScenarioGroupCollection'/>.
        ///    </para>
        /// </summary>
        public ScenarioGroupCollection(TestResult parent) {
            this._parent = parent;
        }
        
        /// <summary>
        ///     <para>
        ///       Initializes a new instance of <see cref='LogTree.ScenarioGroupCollection'/> based on another <see cref='LogTree.ScenarioGroupCollection'/>.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///       A <see cref='LogTree.ScenarioGroupCollection'/> from which the contents are copied
        /// </param>
        public ScenarioGroupCollection(TestResult parent, ScenarioGroupCollection value) {
            this.AddRange(value);
            this._parent = parent;
        }
        
        /// <summary>
        ///     <para>
        ///       Initializes a new instance of <see cref='LogTree.ScenarioGroupCollection'/> containing any array of <see cref='LogTree.ScenarioGroup'/> objects.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///       A array of <see cref='LogTree.ScenarioGroup'/> objects with which to intialize the collection
        /// </param>
        public ScenarioGroupCollection(ScenarioGroup[] value) {
            this.AddRange(value);
        }
        
        /// <summary>
        /// <para>Represents the entry at the specified index of the <see cref='LogTree.ScenarioGroup'/>.</para>
        /// </summary>
        /// <param name='index'><para>The zero-based index of the entry to locate in the collection.</para></param>
        /// <value>
        ///    <para> The entry at the specified index of the collection.</para>
        /// </value>
        /// <exception cref='System.ArgumentOutOfRangeException'><paramref name='index'/> is outside the valid range of indexes for the collection.</exception>
        public ScenarioGroup this[int index] {
            get {
                return ((ScenarioGroup)(List[index]));
            }
            set {
                List[index] = value;
            }
        }
        
        /// <summary>
        ///    <para>Adds a <see cref='LogTree.ScenarioGroup'/> with the specified value to the 
        ///    <see cref='LogTree.ScenarioGroupCollection'/> .</para>
        /// </summary>
        /// <param name='value'>The <see cref='LogTree.ScenarioGroup'/> to add.</param>
        /// <returns>
        ///    <para>The index at which the new element was inserted.</para>
        /// </returns>
        /// <seealso cref='LogTree.ScenarioGroupCollection.AddRange'/>
        public int Add(ScenarioGroup value) {
            value.SetParent(_parent);
            return List.Add(value);
        }
        
        /// <summary>
        /// <para>Copies the elements of an array to the end of the <see cref='LogTree.ScenarioGroupCollection'/>.</para>
        /// </summary>
        /// <param name='value'>
        ///    An array of type <see cref='LogTree.ScenarioGroup'/> containing the objects to add to the collection.
        /// </param>
        /// <returns>
        ///   <para>None.</para>
        /// </returns>
        /// <seealso cref='LogTree.ScenarioGroupCollection.Add'/>
        public void AddRange(ScenarioGroup[] value) {
            for (int i = 0; (i < value.Length); i = (i + 1)) {
                value[i].SetParent(_parent);
                this.Add(value[i]);
            }
        }
        
        /// <summary>
        ///     <para>
        ///       Adds the contents of another <see cref='LogTree.ScenarioGroupCollection'/> to the end of the collection.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///    A <see cref='LogTree.ScenarioGroupCollection'/> containing the objects to add to the collection.
        /// </param>
        /// <returns>
        ///   <para>None.</para>
        /// </returns>
        /// <seealso cref='LogTree.ScenarioGroupCollection.Add'/>
        public void AddRange(ScenarioGroupCollection value) {
            for (int i = 0; (i < value.Count); i = (i + 1)) {
                value[i].SetParent(_parent);
                this.Add(value[i]);
            }
        }
        
        /// <summary>
        /// <para>Gets a value indicating whether the 
        ///    <see cref='LogTree.ScenarioGroupCollection'/> contains the specified <see cref='LogTree.ScenarioGroup'/>.</para>
        /// </summary>
        /// <param name='value'>The <see cref='LogTree.ScenarioGroup'/> to locate.</param>
        /// <returns>
        /// <para><see langword='true'/> if the <see cref='LogTree.ScenarioGroup'/> is contained in the collection; 
        ///   otherwise, <see langword='false'/>.</para>
        /// </returns>
        /// <seealso cref='LogTree.ScenarioGroupCollection.IndexOf'/>
        public bool Contains(ScenarioGroup value) {
            return List.Contains(value);
        }
        
        /// <summary>
        /// <para>Copies the <see cref='LogTree.ScenarioGroupCollection'/> values to a one-dimensional <see cref='System.Array'/> instance at the 
        ///    specified index.</para>
        /// </summary>
        /// <param name='array'><para>The one-dimensional <see cref='System.Array'/> that is the destination of the values copied from <see cref='LogTree.ScenarioGroupCollection'/> .</para></param>
        /// <param name='index'>The index in <paramref name='array'/> where copying begins.</param>
        /// <returns>
        ///   <para>None.</para>
        /// </returns>
        /// <exception cref='System.ArgumentException'><para><paramref name='array'/> is multidimensional.</para> <para>-or-</para> <para>The number of elements in the <see cref='LogTree.ScenarioGroupCollection'/> is greater than the available space between <paramref name='arrayIndex'/> and the end of <paramref name='array'/>.</para></exception>
        /// <exception cref='System.ArgumentNullException'><paramref name='array'/> is <see langword='null'/>. </exception>
        /// <exception cref='System.ArgumentOutOfRangeException'><paramref name='arrayIndex'/> is less than <paramref name='array'/>'s lowbound. </exception>
        /// <seealso cref='System.Array'/>
        public void CopyTo(ScenarioGroup[] array, int index) {
            List.CopyTo(array, index);
        }
        
        /// <summary>
        ///    <para>Returns the index of a <see cref='LogTree.ScenarioGroup'/> in 
        ///       the <see cref='LogTree.ScenarioGroupCollection'/> .</para>
        /// </summary>
        /// <param name='value'>The <see cref='LogTree.ScenarioGroup'/> to locate.</param>
        /// <returns>
        /// <para>The index of the <see cref='LogTree.ScenarioGroup'/> of <paramref name='value'/> in the 
        /// <see cref='LogTree.ScenarioGroupCollection'/>, if found; otherwise, -1.</para>
        /// </returns>
        /// <seealso cref='LogTree.ScenarioGroupCollection.Contains'/>
        public int IndexOf(ScenarioGroup value) {
            return List.IndexOf(value);
        }
        
        /// <summary>
        /// <para>Inserts a <see cref='LogTree.ScenarioGroup'/> into the <see cref='LogTree.ScenarioGroupCollection'/> at the specified index.</para>
        /// </summary>
        /// <param name='index'>The zero-based index where <paramref name='value'/> should be inserted.</param>
        /// <param name=' value'>The <see cref='LogTree.ScenarioGroup'/> to insert.</param>
        /// <returns><para>None.</para></returns>
        /// <seealso cref='LogTree.ScenarioGroupCollection.Add'/>
        public void Insert(int index, ScenarioGroup value) {
            value.SetParent(_parent);
            List.Insert(index, value);
        }
        
        /// <summary>
        ///    <para>Returns an enumerator that can iterate through 
        ///       the <see cref='LogTree.ScenarioGroupCollection'/> .</para>
        /// </summary>
        /// <returns><para>None.</para></returns>
        /// <seealso cref='System.Collections.IEnumerator'/>
        public new ScenarioGroupEnumerator GetEnumerator() {
            return new ScenarioGroupEnumerator(this);
        }
        
        /// <summary>
        ///    <para> Removes a specific <see cref='LogTree.ScenarioGroup'/> from the 
        ///    <see cref='LogTree.ScenarioGroupCollection'/> .</para>
        /// </summary>
        /// <param name='value'>The <see cref='LogTree.ScenarioGroup'/> to remove from the <see cref='LogTree.ScenarioGroupCollection'/> .</param>
        /// <returns><para>None.</para></returns>
        /// <exception cref='System.ArgumentException'><paramref name='value'/> is not found in the Collection. </exception>
        public void Remove(ScenarioGroup value) {
            value.SetParent(null);
            List.Remove(value);
        }
        
        public class ScenarioGroupEnumerator : object, IEnumerator {
            
            private IEnumerator _baseEnumerator;
            
            private IEnumerable _temp;
            
            public ScenarioGroupEnumerator(ScenarioGroupCollection mappings) {
                this._temp = ((IEnumerable)(mappings));
                this._baseEnumerator = _temp.GetEnumerator();
            }
            
            public ScenarioGroup Current {
                get {
                    return ((ScenarioGroup)(_baseEnumerator.Current));
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
