// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Common.TestObjects.Xaml
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Xml;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Common.TestObjects.Xaml.GraphCore;
    using Microsoft.Test.Xaml.Common.TestObjects.Xaml.GraphOperations.Builders;

    /// <summary>
    /// Object graph information
    /// </summary>
    [Serializable]
    public class ObjectGraph : GraphNode
    {
        /// <summary>
        /// Datatype property
        /// </summary>
        private static XName s_dataTypeProperty = XName.Get("DataType", String.Empty);

        /// <summary>
        /// data as string rep
        /// </summary>
        private static XName s_dataAsStringProperty = XName.Get("DataAsString", String.Empty);

        /// <summary>
        /// The data might not be serializable - so we have to lose it we store 
        /// </summary>
        [NonSerialized]
        private object _data;

        /// <summary>
        /// Initializes a new instance of the ObjectGraph class.
        /// </summary>
        public ObjectGraph()
            : base("NoName", null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ObjectGraph class.
        /// </summary>
        /// <param name="nodeData">data stored in the node</param>
        public ObjectGraph(object nodeData)
            : base("Root", null)
        {
            this.Data = nodeData;
            this.DataType = nodeData.GetType();
        }

        /// <summary>
        /// Initializes a new instance of the ObjectGraph class.
        /// </summary>
        /// <param name="name">name of the node</param>
        /// <param name="nodeData">node input data</param>
        /// <param name="nodeType">type of data</param>
        /// <param name="parent">parent node to this node</param>
        public ObjectGraph(string name, object nodeData, Type nodeType, IGraphNode parent)
            : base(name, parent)
        {
            this.Data = nodeData;
            this.DataType = nodeType;
        }

        /// <summary>
        /// Gets or sets the Data property
        /// </summary>
        public object Data
        {
            get
            {
                return _data;
            }

            set
            {
                this._data = value;
                if (this._data != null)
                {
                    // use the roundtrip format for DateTime or ToString will fail for certain cultures (I'm looking at you ar-sa)
                    SetValue(s_dataAsStringProperty, value is DateTime ? ((DateTime)_data).ToString("o") : _data.ToString());
                }
            }
        }

        /// <summary>
        /// Gets or sets the data as string property
        /// </summary>
        public string DataAsString
        {
            get
            {
                return (string)GetValue(s_dataAsStringProperty);
            }

            set
            {
                SetValue(s_dataAsStringProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the data's type
        /// </summary>
        public Type DataType
        {
            get
            {
                return (Type)GetValue(s_dataTypeProperty);
            }

            set
            {
                SetValue(s_dataTypeProperty, value);
            }
        }

        /// <summary>
        /// Serialize the object graph to a file
        /// </summary>
        /// <param name="root">root object to serialize</param>
        /// <param name="fileName">file to save to</param>
        public static void Serialize(ObjectGraph root, string fileName)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Create);
            BinaryFormatter formatter  = new BinaryFormatter();
            try 
            {
                formatter.Serialize(fileStream, root);
            }
            finally 
            {
                fileStream.Close();
            }
        }

        /// <summary>
        /// Deserialize the object graph to a file
        /// </summary>
        /// <param name="fileName">file to load from</param>
        /// <returns>deserialized object graph</returns>
        public static ObjectGraph Deserialize(string fileName)
        {            
            FileStream fileStream = new FileStream(fileName, FileMode.Open);
            
            try 
            {
                BinaryFormatter formatter = new BinaryFormatter();

                // Deserialize the hashtable from the file and 
                // assign the reference to the local variable.
                ObjectGraph root = (ObjectGraph) formatter.Deserialize(fileStream);
                return root;
            }
            finally 
            {
                fileStream.Close();
            }
        }

        /// <summary>
        /// Clone this object graph
        /// </summary>
        /// <returns>cloned object graph</returns>
        public ObjectGraph Clone()
        {
            return ObjectGraphWalker.Create(this.Data);
        }
    }
}
