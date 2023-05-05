// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Common.XamlOM
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Holder of a collection of Key/value pairs (metadata)
    /// </summary>
    public class PropertyBag : IDictionary<string, object>
    {  
        /// <summary>
        /// Initializes a new instance of the PropertyBag class.
        /// </summary>
        public PropertyBag()
        {
            this.TestMetadata = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets the Keys collection
        /// </summary>
        public ICollection<string> Keys
        {
            get { return this.TestMetadata.Keys; }
        }

        /// <summary>
        /// Gets the count property
        /// </summary>
        public int Count
        {
            get { return this.TestMetadata.Count; }
        }

        /// <summary>
        /// Gets the collection of values
        /// </summary>
        public ICollection<object> Values
        {
            get { return this.TestMetadata.Values; }
        }

        /// <summary>
        /// Gets a value indicating whether this is readonly 
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets or sets the test metadata holder
        /// </summary>
        private Dictionary<string, object> TestMetadata { get; set; }
        
        /// <summary>
        /// Get or set the metadata property/value
        /// </summary>
        /// <param name="propertyName">name of the property</param>
        /// <returns>value of the property</returns>
        public object this[string propertyName]
        {
            get
            {
                if (this.TestMetadata.ContainsKey(propertyName))
                {
                    return this.TestMetadata[propertyName];
                }
                else
                {
                    return null;
                }
            }

            set
            {
                this.TestMetadata[propertyName] = value;
            }
        }
       
        /// <summary>
        /// Get the string value property 
        /// </summary>
        /// <param name="propertyName">name of the property</param>
        /// <returns>value of the property</returns>
        public string GetStringProperty(string propertyName)
        {
            string value = this[propertyName] as string;
            return value == null ? (string)null : value as string;
        }

        /// <summary>
        /// Get a boolean property
        /// </summary>
        /// <param name="propertyName">name of the property</param>
        /// <returns>value of the property</returns>
        public bool GetBoolProperty(string propertyName)
        {
            object value = this[propertyName];
            return value == null ? false : (bool)value;
        }

        /// <summary>
        /// Add to the dictionary
        /// </summary>
        /// <param name="key">key into the dictionary</param>
        /// <param name="value">value to add</param>
        public void Add(string key, object value)
        {
            this.TestMetadata.Add(key, value);
        }

        /// <summary>
        /// Check if the dictionary contains the key
        /// </summary>
        /// <param name="key">name of the key</param>
        /// <returns>true if exists</returns>
        public bool ContainsKey(string key)
        {
            return this.TestMetadata.ContainsKey(key);
        }

        /// <summary>
        /// Remove an entry from the dictionary
        /// </summary>
        /// <param name="key">key into the dictionary</param>
        /// <returns>true if removed</returns>
        public bool Remove(string key)
        {
            return this.TestMetadata.Remove(key);
        }

        /// <summary>
        /// Try get a particular value
        /// </summary>
        /// <param name="key">key into the dictionary</param>
        /// <param name="value">value to get out</param>
        /// <returns>true if exists</returns>
        public bool TryGetValue(string key, out object value)
        {
            return this.TestMetadata.TryGetValue(key, out value);
        }

        /// <summary>
        /// Add item to dictionary
        /// </summary>
        /// <param name="item">item to add</param>
        public void Add(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clear all the items
        /// </summary>
        public void Clear()
        {
            this.TestMetadata.Clear();
        }

        /// <summary>
        /// Check if item is available
        /// </summary>
        /// <param name="item">item to check</param>
        /// <returns>true if exists</returns>
        public bool Contains(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Copy items
        /// </summary>
        /// <param name="array">array of items</param>
        /// <param name="arrayIndex">index into the array</param>
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove an item
        /// </summary>
        /// <param name="item">item to remove</param>
        /// <returns>true if removed</returns>
        public bool Remove(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the enumerator
        /// </summary>
        /// <returns>Enumerator to loop through items</returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get enumerator
        /// </summary>
        /// <returns>enumerator to loop through items</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.TestMetadata.GetEnumerator();
        }
    }
}
