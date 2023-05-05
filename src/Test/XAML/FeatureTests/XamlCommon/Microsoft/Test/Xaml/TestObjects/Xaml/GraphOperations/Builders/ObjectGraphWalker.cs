// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Common.TestObjects.Xaml.GraphOperations.Builders
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Microsoft.Test.Xaml.Common.TestObjects.Xaml.GraphCore;

    /// <summary>
    /// Object graph walker
    /// </summary>
    public static class ObjectGraphWalker
    {
        /// <summary>
        /// Data contained in the object
        /// </summary>
        private const string ObjectDataProp = "ObjectData";

        /// <summary>
        /// Type of the object
        /// </summary>
        private const string ObjectTypeProp = "ObjectType";

        /// <summary>
        /// Build and ObjectGraph following the rules of XAML
        /// - gives back an ObjectGraph which wraps the object 
        /// </summary>
        /// <param name="data">input data value</param>
        /// <returns>Created object graph</returns>
        public static ObjectGraph Create(object data)
        {
            XamlObjectGraphBuilder builder = new XamlObjectGraphBuilder();
            return (ObjectGraph)Create(data, builder);
        }

        /// <summary>
        /// Create a graph with the data and builder provided
        /// </summary>
        /// <param name="data">input object data</param>
        /// <param name="builder">builder to use</param>
        /// <returns>created object graph</returns>
        public static IGraphNode Create(object data, IXamlObjectGraphBuilder builder)
        {
            return BuildGraph("Root", data, data.GetType(), null, builder);
        }

        /// <summary>
        /// Get object dependency property value
        /// </summary>
        /// <param name="props">dependency object</param>
        /// <returns>property's value</returns>
        public static object GetObjectData(ITestDependencyObject props)
        {
            return props.GetValue(ObjectDataProp);
        }

        /// <summary>
        /// Set object dependency property
        /// </summary>
        /// <param name="props">dependency object</param>
        /// <param name="value">value to set</param>
        public static void SetObjectData(ITestDependencyObject props, object value)
        {
            props.SetValue(ObjectDataProp, value);
        }

        /// <summary>
        /// Clear the object data
        /// </summary>
        /// <param name="props">dependency object</param>
        public static void ClearObjectData(ITestDependencyObject props)
        {
            props.Properties.Remove(ObjectDataProp);
        }

        /// <summary>
        /// Get the type property
        /// </summary>
        /// <param name="props">dependency object</param>
        /// <returns>Type contained</returns>
        public static Type GetObjectType(ITestDependencyObject props)
        {
            return (Type)props.GetValue(ObjectTypeProp);
        }

        /// <summary>
        /// Set the object type
        /// </summary>
        /// <param name="props">dependency object</param>
        /// <param name="value">value to set</param>
        public static void SetObjectType(ITestDependencyObject props, Type value)
        {
            props.SetValue(ObjectTypeProp, value);
        }

        /// <summary>
        /// clear the type property
        /// </summary>
        /// <param name="props">dependency object</param>
        public static void ClearObjectType(ITestDependencyObject props)
        {
            props.Properties.Remove(ObjectTypeProp);
        }

        /// <summary>
        /// Build the graph
        /// </summary>
        /// <param name="name">name of graph</param>
        /// <param name="data">data object to bulid from</param>
        /// <param name="type">type of this node</param>
        /// <param name="parent">parent of this node</param>
        /// <param name="builder">builder to use</param>
        /// <returns>Created object graph</returns>
        private static IGraphNode BuildGraph(string name, object data, Type type, IGraphNode parent, IXamlObjectGraphBuilder builder)
        {
            Queue<IGraphNode> pendingQueue = new Queue<IGraphNode>();
            Dictionary<int, IGraphNode> visitedObjects = new Dictionary<int, IGraphNode>();

            IGraphNode root = BuildNodeHelper(name, data, type, parent, null, builder, false);
            pendingQueue.Enqueue(root);

            while (pendingQueue.Count != 0)
            {
                IGraphNode node = pendingQueue.Dequeue();
                object nodeData = GetObjectData(node);
                Type nodeType = GetObjectType(node);

                //// clear the properties so they don't potentially get serialized
                ClearObjectData(node);
                ClearObjectType(node);

                if (nodeData == null || nodeType.IsPrimitive == true ||
                    nodeType == typeof(string))
                {
                    //// we have reached a leaf node //
                    continue;
                }

                if (visitedObjects.Keys.Contains(nodeData.GetHashCode()))
                {
                    //// Caused by a cycle - alredy seen this node //
                    IGraphNode builtNode = visitedObjects[nodeData.GetHashCode()];

                    foreach (IGraphNode newChild in builder.BuildVisitedNode(builtNode, node))
                    {
                        node.Children.Add(newChild);
                    }

                    //// node.Children.Add(visitedObjects[nodeData.GetHashCode()]);
                    continue;
                }

                //// ok the type is a complex type - query all properties //
                //// create children for clr properties //
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(nodeData);

                foreach (PropertyDescriptor property in properties)
                {
                    if (IsReadablePropertyDescriptor(property))
                    {
                        object value = null;
                        try
                        {
                            value = property.GetValue(nodeData);
                        }
                        catch (Exception)
                        {
                            //// 
                        }

                        IGraphNode childNode = BuildNodeHelper(property.Name, value, property.PropertyType, node, property.Name, builder, property.IsReadOnly);

                        if (childNode == null)
                        {
                            continue;
                        }

                        IGraphNode actualChild = childNode;
                        while (actualChild.Parent != node)
                        {
                            actualChild = actualChild.Parent;
                            if (actualChild == null)
                            {
                                throw new InvalidOperationException("Node returned from BuildNode has invalid parent.");
                            }
                        }

                        node.Children.Add(actualChild);
                        pendingQueue.Enqueue(childNode);
                    }
                }

                //// IEnumerable support //
                int count = 0;
                IEnumerable enumerableData = nodeData as IEnumerable;
                if (enumerableData != null && nodeData.GetType() != typeof(string))
                {
                    IGraphNode collectionParent = builder.BuildCollectionWrapperNode(node);

                    if (collectionParent != null)
                    {
                        node.Children.Add(collectionParent);
                    }
                    else
                    {
                        collectionParent = node;
                    }

                    IEnumerator enumerator = enumerableData.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        IGraphNode childNode = BuildNodeHelper("IEnumerable" + count++, enumerator.Current, enumerator.Current == null ? typeof(object) : enumerator.Current.GetType(), collectionParent, null, builder, false);
                        if (childNode == null)
                        {
                            continue;
                        }

                        collectionParent.Children.Add(childNode);
                        pendingQueue.Enqueue(childNode);
                    }
                }

                visitedObjects.Add(nodeData.GetHashCode(), node);
            }

            return root;
        }

        /// <summary>
        /// Helper method
        /// </summary>
        /// <param name="name">name of node</param>
        /// <param name="data">data held by node</param>
        /// <param name="type">type of node</param>
        /// <param name="parent">parent node to this node</param>
        /// <param name="propertyName">name of the property</param>
        /// <param name="builder">buider to use</param>
        /// <param name="isReadOnly">is this readonly</param>
        /// <returns>Built graph node</returns>
        private static IGraphNode BuildNodeHelper(string name, object data, Type type, IGraphNode parent, string propertyName, IXamlObjectGraphBuilder builder, bool isReadOnly)
        {
            IGraphNode node = builder.BuildNode(name, data, type, parent, propertyName, isReadOnly);
            if (node == null)
            {
                return node;
            }

            SetObjectData(node, data);
            SetObjectType(node, type);
            return node;
        }

        /// <summary>
        /// Checks if GetValue may be called on the given PropertyDescriptor.
        /// </summary>
        /// <param name="property">property information</param>
        /// <returns>true if readable</returns>
        private static bool IsReadablePropertyDescriptor(PropertyDescriptor property)
        {
            return !(property.ComponentType is System.Reflection.MemberInfo)
                   || !IsGenericTypeMember(property.ComponentType, property.Name);
        }

        /// <summary>
        /// Checks if the given type member is a generic-only member on a non-generic type.
        /// </summary>
        /// <param name="type">type of object</param>
        /// <param name="memberName">member name to be used</param>
        /// <returns>true if generic property</returns>
        private static bool IsGenericTypeMember(Type type, string memberName)
        {
            return !type.IsGenericType
                   && (memberName == "GenericParameterPosition"
                       || memberName == "GenericParameterAttributes"
                       || memberName == "GetGenericArguments"
                       || memberName == "GetGenericParameterConstraints"
                       || memberName == "GetGenericTypeDefinition"
                       || memberName == "IsGenericTypeDefinition"
                       || memberName == "DeclaringMethod");
        }
    }
}
