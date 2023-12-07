// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace WFCTestLib.XmlLogTree {
    using System;
    using System.Collections;
    
    
    /// <summary>
    ///     <para>
    ///       A collection that stores <see cref='LogTree.Scenario'/> objects.
    ///    </para>
    /// </summary>
    /// <seealso cref='LogTree.ScenarioCollection'/>
    [Serializable()]
    public class ScenarioCollection : CollectionBase {
        ScenarioGroup _parent;
        
        /// <summary>
        ///     <para>
        ///       Initializes a new instance of <see cref='LogTree.ScenarioCollection'/>.
        ///    </para>
        /// </summary>
        public ScenarioCollection(ScenarioGroup parent) {
            this._parent = parent;
        }
        
        /// <summary>
        ///     <para>
        ///       Initializes a new instance of <see cref='LogTree.ScenarioCollection'/> based on another <see cref='LogTree.ScenarioCollection'/>.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///       A <see cref='LogTree.ScenarioCollection'/> from which the contents are copied
        /// </param>
        public ScenarioCollection(ScenarioGroup parent, ScenarioCollection value) {
            this.AddRange(value);
            this._parent = parent;
        }
        
        /// <summary>
        ///     <para>
        ///       Initializes a new instance of <see cref='LogTree.ScenarioCollection'/> containing any array of <see cref='LogTree.Scenario'/> objects.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///       A array of <see cref='LogTree.Scenario'/> objects with which to intialize the collection
        /// </param>
        public ScenarioCollection(Scenario[] value) {
            this.AddRange(value);
        }
        
        /// <summary>
        /// <para>Represents the entry at the specified index of the <see cref='LogTree.Scenario'/>.</para>
        /// </summary>
        /// <param name='index'><para>The zero-based index of the entry to locate in the collection.</para></param>
        /// <value>
        ///    <para> The entry at the specified index of the collection.</para>
        /// </value>
        /// <exception cref='System.ArgumentOutOfRangeException'><paramref name='index'/> is outside the valid range of indexes for the collection.</exception>
        public Scenario this[int index] {
            get {
                return ((Scenario)(List[index]));
            }
            set {
                List[index] = value;
            }
        }
        
        /// <summary>
        ///    <para>Adds a <see cref='LogTree.Scenario'/> with the specified value to the 
        ///    <see cref='LogTree.ScenarioCollection'/> .</para>
        /// </summary>
        /// <param name='value'>The <see cref='LogTree.Scenario'/> to add.</param>
        /// <returns>
        ///    <para>The index at which the new element was inserted.</para>
        /// </returns>
        /// <seealso cref='LogTree.ScenarioCollection.AddRange'/>
        public int Add(Scenario value) {
            value.SetParent(_parent);
            return List.Add(value);
        }
        
        /// <summary>
        /// <para>Copies the elements of an array to the end of the <see cref='LogTree.ScenarioCollection'/>.</para>
        /// </summary>
        /// <param name='value'>
        ///    An array of type <see cref='LogTree.Scenario'/> containing the objects to add to the collection.
        /// </param>
        /// <returns>
        ///   <para>None.</para>
        /// </returns>
        /// <seealso cref='LogTree.ScenarioCollection.Add'/>
        public void AddRange(Scenario[] value) {
            for (int i = 0; (i < value.Length); i = (i + 1)) {
                value[i].SetParent(_parent);
                this.Add(value[i]);
            }
        }
        
        /// <summary>
        ///     <para>
        ///       Adds the contents of another <see cref='LogTree.ScenarioCollection'/> to the end of the collection.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///    A <see cref='LogTree.ScenarioCollection'/> containing the objects to add to the collection.
        /// </param>
        /// <returns>
        ///   <para>None.</para>
        /// </returns>
        /// <seealso cref='LogTree.ScenarioCollection.Add'/>
        public void AddRange(ScenarioCollection value) {
            for (int i = 0; (i < value.Count); i = (i + 1)) {
                value[i].SetParent(_parent);
                this.Add(value[i]);
            }
        }
        
        /// <summary>
        /// <para>Gets a value indicating whether the 
        ///    <see cref='LogTree.ScenarioCollection'/> contains the specified <see cref='LogTree.Scenario'/>.</para>
        /// </summary>
        /// <param name='value'>The <see cref='LogTree.Scenario'/> to locate.</param>
        /// <returns>
        /// <para><see langword='true'/> if the <see cref='LogTree.Scenario'/> is contained in the collection; 
        ///   otherwise, <see langword='false'/>.</para>
        /// </returns>
        /// <seealso cref='LogTree.ScenarioCollection.IndexOf'/>
        public bool Contains(Scenario value) {
            return List.Contains(value);
        }
        
        /// <summary>
        /// <para>Copies the <see cref='LogTree.ScenarioCollection'/> values to a one-dimensional <see cref='System.Array'/> instance at the 
        ///    specified index.</para>
        /// </summary>
        /// <param name='array'><para>The one-dimensional <see cref='System.Array'/> that is the destination of the values copied from <see cref='LogTree.ScenarioCollection'/> .</para></param>
        /// <param name='index'>The index in <paramref name='array'/> where copying begins.</param>
        /// <returns>
        ///   <para>None.</para>
        /// </returns>
        /// <exception cref='System.ArgumentException'><para><paramref name='array'/> is multidimensional.</para> <para>-or-</para> <para>The number of elements in the <see cref='LogTree.ScenarioCollection'/> is greater than the available space between <paramref name='arrayIndex'/> and the end of <paramref name='array'/>.</para></exception>
        /// <exception cref='System.ArgumentNullException'><paramref name='array'/> is <see langword='null'/>. </exception>
        /// <exception cref='System.ArgumentOutOfRangeException'><paramref name='arrayIndex'/> is less than <paramref name='array'/>'s lowbound. </exception>
        /// <seealso cref='System.Array'/>
        public void CopyTo(Scenario[] array, int index) {
            List.CopyTo(array, index);
        }
        
        /// <summary>
        ///    <para>Returns the index of a <see cref='LogTree.Scenario'/> in 
        ///       the <see cref='LogTree.ScenarioCollection'/> .</para>
        /// </summary>
        /// <param name='value'>The <see cref='LogTree.Scenario'/> to locate.</param>
        /// <returns>
        /// <para>The index of the <see cref='LogTree.Scenario'/> of <paramref name='value'/> in the 
        /// <see cref='LogTree.ScenarioCollection'/>, if found; otherwise, -1.</para>
        /// </returns>
        /// <seealso cref='LogTree.ScenarioCollection.Contains'/>
        public int IndexOf(Scenario value) {
            return List.IndexOf(value);
        }
        
        /// <summary>
        /// <para>Inserts a <see cref='LogTree.Scenario'/> into the <see cref='LogTree.ScenarioCollection'/> at the specified index.</para>
        /// </summary>
        /// <param name='index'>The zero-based index where <paramref name='value'/> should be inserted.</param>
        /// <param name=' value'>The <see cref='LogTree.Scenario'/> to insert.</param>
        /// <returns><para>None.</para></returns>
        /// <seealso cref='LogTree.ScenarioCollection.Add'/>
        public void Insert(int index, Scenario value) {
            value.SetParent(_parent);
            List.Insert(index, value);
        }
        
        /// <summary>
        ///    <para>Returns an enumerator that can iterate through 
        ///       the <see cref='LogTree.ScenarioCollection'/> .</para>
        /// </summary>
        /// <returns><para>None.</para></returns>
        /// <seealso cref='System.Collections.IEnumerator'/>
        public new ScenarioEnumerator GetEnumerator() {
            return new ScenarioEnumerator(this);
        }
        
        /// <summary>
        ///    <para> Removes a specific <see cref='LogTree.Scenario'/> from the 
        ///    <see cref='LogTree.ScenarioCollection'/> .</para>
        /// </summary>
        /// <param name='value'>The <see cref='LogTree.Scenario'/> to remove from the <see cref='LogTree.ScenarioCollection'/> .</param>
        /// <returns><para>None.</para></returns>
        /// <exception cref='System.ArgumentException'><paramref name='value'/> is not found in the Collection. </exception>
        public void Remove(Scenario value) {
            value.SetParent(null);
            List.Remove(value);
        }
        
        public class ScenarioEnumerator : object, IEnumerator {
            
            private IEnumerator _baseEnumerator;
            
            private IEnumerable _temp;
            
            public ScenarioEnumerator(ScenarioCollection mappings) {
                this._temp = ((IEnumerable)(mappings));
                this._baseEnumerator = _temp.GetEnumerator();
            }
            
            public Scenario Current {
                get {
                    return ((Scenario)(_baseEnumerator.Current));
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
