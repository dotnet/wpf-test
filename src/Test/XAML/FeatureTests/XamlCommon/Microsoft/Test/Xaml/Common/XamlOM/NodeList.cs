// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Common.XamlOM
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Xaml;

    /// <summary>
    /// NodeList - contains an ordered collection of Nodes
    /// implements IList so that we can provide custom Add methods
    /// for adding a Node or a NodeList to the collection.
    /// </summary>
    public class NodeList : IList
    {
        /// <summary>
        /// Initializes a new instance of the NodeList class.
        /// </summary>
        public NodeList() : this("UnnamedNodeList") 
        {
        }

        /// <summary>
        ///  Initializes a new instance of the NodeList class.
        /// </summary>
        /// <param name="name">name of the nodelist</param>
        public NodeList(string name)
        {
            this.Name = name;
            this.Nodes = new List<Node>();
        }

        /// <summary>
        /// Gets the Empty nodelist
        /// </summary>
        public static NodeList Empty
        {
            get { return new NodeList(string.Empty); }
        }

        /// <summary>
        /// Gets or sets the Name property
        /// Name of the NodeList (identity)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets a value indicating whether list is fixed size
        /// </summary>
        public bool IsFixedSize
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the list is readonly
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the Count property
        /// </summary>
        public int Count
        {
            get { return this.Nodes.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the list is Synchronized
        /// </summary>
        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the SyncRoot property
        /// </summary>
        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the Nodes property
        /// </summary>
        private List<Node> Nodes { get; set; }

        /// <summary>
        /// Gets or sets the indexer
        /// </summary>
        /// <param name="index">index into the list</param>
        /// <returns>value at the index</returns>
        public object this[int index]
        {
            get
            {
                return this.Nodes[index];
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// ToString provides an indented string representation 
        /// of the entire NodeList 
        /// </summary>
        /// <returns>entire node list as indented string</returns>
        public override string ToString()
        {
            StringWriter stringWriter = new StringWriter();
            int indent = 0;
            foreach (Node node in this.Nodes)
            {
                if (node is EndMember || node is EndObject)
                {
                    indent--;
                }

                stringWriter.WriteLine(Indent(indent) + node.ToString());

                if (node is StartMember || node is StartObject || node is GetObject)
                {
                    indent++;
                }
            }

            return stringWriter.ToString();
        }

        /// <summary>
        /// Add a Node or NodeList.
        /// var nodeList = NodeList.Empty;
        /// nodeList.Add(new Node());
        /// nodeList.Add(NodeList.Empty);
        /// </summary>
        /// <param name="value">Node or NodeList instance to add</param>
        /// <returns>indext where added</returns>
        public int Add(object value)
        {
            ValidateItem(value);

            Node node = value as Node;
            if (node != null)
            {
                this.Nodes.Add(node);
                return this.Nodes.Count - 1;
            }

            NodeList nodes = value as NodeList;
            if (nodes != null)
            {
                foreach (Node n in nodes)
                {
                    this.Nodes.Add(n);
                }

                return this.Nodes.Count - 1;
            }

            throw new ArgumentException("value is not a Node or NodeList", "value");
        }

        /// <summary>
        /// Clear the nodes
        /// </summary>
        public void Clear()
        {
            this.Nodes.Clear();
        }

        /// <summary>
        /// check if list contains the value
        /// </summary>
        /// <param name="value">value to search for</param>
        /// <returns>true if contained</returns>
        public bool Contains(object value)
        {
            if (!(value is Node))
            {
                throw new ArgumentException("Contains can only take a Node", "value");
            }

            return this.Nodes.Contains(value as Node);
        }

        /// <summary>
        /// Find the index of a node
        /// </summary>
        /// <param name="value">node to search for</param>
        /// <returns>index in the list</returns>
        public int IndexOf(object value)
        {
            if (!(value is Node))
            {
                throw new ArgumentException("IndexOf can only take a Node", "value");
            }

            return this.Nodes.IndexOf(value as Node);
        }

        /// <summary>
        /// Insert a node into the list
        /// </summary>
        /// <param name="index">index into the list</param>
        /// <param name="value">node to insert</param>
        public void Insert(int index, object value)
        {
            if (!(value is Node))
            {
                throw new ArgumentException("Insert can only take a Node", "value");
            }

            this.Nodes.Insert(index, value as Node);
        }

        /// <summary>
        /// Remove a node
        /// </summary>
        /// <param name="value">node to remove</param>
        public void Remove(object value)
        {
            if (!(value is Node))
            {
                throw new ArgumentException("Remove can only take a Node", "value");
            }

            this.Nodes.Remove(value as Node);
        }

        /// <summary>
        /// Remove a node at index
        /// </summary>
        /// <param name="index">index location to remove from</param>
        public void RemoveAt(int index)
        {
            this.Nodes.RemoveAt(index);
        }

        /// <summary>
        /// CopyTo a different an array
        /// </summary>
        /// <param name="array">array to copy to</param>
        /// <param name="index">index into the array</param>
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the enumerator
        /// </summary>
        /// <returns>enumerator to walk through the list of nodes</returns>
        public IEnumerator GetEnumerator()
        {
            return this.Nodes.GetEnumerator();
        }

        /// <summary>
        /// indent using spaces
        /// </summary>
        /// <param name="indent">number of spaces to indent</param>
        /// <returns>string with indent spaces</returns>
        private string Indent(int indent)
        {
            return string.Empty.PadLeft(indent);
        }

        /// <summary>
        /// Validate an item
        /// </summary>
        /// <param name="item">item to validate</param>
        private void ValidateItem(object item)
        {
            if (!(item is Node) &&
                !(item is NodeList))
            {
                throw new InvalidOperationException("Cannot use item that is not a Node or NodeList");
            }
        }
    }
}
