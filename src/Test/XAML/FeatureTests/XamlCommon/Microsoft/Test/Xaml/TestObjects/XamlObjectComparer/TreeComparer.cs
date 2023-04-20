// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Test.Xaml.Common.TestObjects.XamlObjectComparer
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Common.TestObjects.Xaml.GraphOperations.Comparers;

    /// <summary>
    /// A class providing a static method to compare two objects
    /// by comparing node by node and property by property.
    /// </summary>
    public static class TreeComparer
    {   
        /// <summary>
        /// properties to skip
        /// </summary>
        private static readonly Dictionary<string, PropertyToIgnore> s_skipPropertiesDefault = ReadSkipProperties();

        /// <summary>
        /// cache of objects in the tree
        /// </summary>
        private static readonly List<int>[] s_objectsInTree = new List<int>[2];

        /// <summary>
        /// properties to skip
        /// </summary>
        private static Dictionary<string, PropertyToIgnore> s_skipProperties = null;

        /// <summary>
        /// Gets or sets a value indicating whether BreakOnError
        /// </summary>
        public static bool BreakOnError { get; set; }

        /// <summary>
        /// Compare two object trees. If all the descendant logical nodes 
        /// are equivalent, return true, otherwise, return false.
        /// </summary>
        /// <param name="firstTree">The root for the first tree</param>
        /// <param name="secondTree">The root for the second tree</param>
        /// <remarks>
        /// Compares every event and property for each the node.
        /// </remarks>
        /// <returns>
        /// A structure containing result. If the returned variable name is result.
        /// result.Result is CompareResult.Equivalent in the case two nodes are equivalent,
        /// and CompareResult.Different otherwise.
        /// </returns>
        public static CompareResult CompareLogical(
            object firstTree,
            object secondTree)
        {
            CompareResult result = CompareLogical(firstTree, secondTree, s_skipPropertiesDefault);
            ObjectGraphComparer.XamlCompareParity(firstTree, secondTree, result);

            return result;
        }

        /// <summary>
        /// Read properties to skip from PropertiesToSkip.xml. If this file
        /// exists under the current working directory, use the one there. 
        /// Otherwise, use the file built in the ClientTestLibrary Assembly.
        /// </summary>
        /// <param name="fileName">Name of config file for specifying properties.</param>
        /// <returns>Hashtable containing properties should be skiped</returns>
        public static Dictionary<string, PropertyToIgnore> ReadSkipProperties(string fileName)
        {
            Dictionary<string, PropertyToIgnore> propertiesToSkip = new Dictionary<string, PropertyToIgnore>();

            //// Load PropertiesToSkip.xml document from assembly resources.
            XmlDocument doc = new XmlDocument();
            Stream xmlFileStream = null;
            if (File.Exists(fileName))
            {
                SendCompareMessage("Opening '" + fileName + "' from the current directory.");
                xmlFileStream = File.OpenRead(fileName);
            }
            else
            {
                SendCompareMessage("Opening '" + fileName + "' from the Assembly.");
                Assembly asm = Assembly.GetAssembly(typeof(TreeComparer));
                xmlFileStream = asm.GetManifestResourceStream(fileName);

                if (xmlFileStream == null)
                {
                    // Tracer.LogTrace("The file '" + fileName + "' cannot be loaded.", "fileName");
                    return propertiesToSkip;
                }
            }

            try
            {
                StreamReader reader = new StreamReader(xmlFileStream);
                doc.LoadXml(reader.ReadToEnd());
            }
            finally
            {
                xmlFileStream.Close();
            }

            //// Store properties to skip in collection.
            XmlNodeList properties = doc.GetElementsByTagName("PropertyToSkip");

            foreach (XmlNode property in properties)
            {
                string propertyName = GetAttributeValue(property, "PropertyName");
                string ignore = GetAttributeValue(property, "Ignore");
                string owner = GetAttributeValue(property, "Owner");

                IgnoreProperty whatToIgnore;

                if (null == ignore || 0 == String.Compare(ignore, "ValueOnly", StringComparison.Ordinal))
                {
                    whatToIgnore = IgnoreProperty.IgnoreValueOnly;
                }
                else if (0 == String.Compare(ignore, "NameAndValue", StringComparison.Ordinal))
                {
                    whatToIgnore = IgnoreProperty.IgnoreNameAndValue;
                }
                else
                {
                    throw new Exception("'Ignore' attribute value not recognized: " + ignore);
                }

                PropertyToIgnore newItem = new PropertyToIgnore();

                newItem.WhatToIgnore = whatToIgnore;

                if (!String.IsNullOrEmpty(owner))
                {
                    newItem.Owner = owner;
                }

                try
                {
                    if (propertiesToSkip.ContainsKey(propertyName))
                    {
                        SendCompareMessage(propertyName);
                    }

                    propertiesToSkip.Add(propertyName + "___owner___" + owner, newItem);
                }
                catch (Exception ex)
                {
                    SendCompareMessage(ex.Message);
                }
            }

            return propertiesToSkip;
        }

        /// <summary>
        /// Compare two object trees. If all the descendant logical nodes 
        /// are equivalent, return true, otherwise, return false.
        /// </summary>
        /// <param name="firstTree">The root for the first tree.</param>
        /// <param name="secondTree">The root for the second tree.</param>
        /// <param name="fileName">Custom list of properties to ignore.</param>
        /// <remarks>
        /// Compares every event and property for each the node.
        /// </remarks>
        /// <returns>
        /// A structure containing result. If the returned variable name is result.
        /// result.Result is CompareResult.Equivalent in the case two nodes are equivalent,
        /// and CompareResult.Different otherwise.
        /// </returns>
        private static CompareResult CompareLogical(
            object firstTree,
            object secondTree,
            string fileName)
        {
            Dictionary<string, PropertyToIgnore> props = ReadSkipProperties(fileName);

            return CompareLogical(firstTree, secondTree, props);
        }

        /// <summary>
        /// Get attribute value
        /// </summary>
        /// <param name="node">Xml node input</param>
        /// <param name="attributeName">name of the attribute</param>
        /// <returns>string abttribute value</returns>
        private static string GetAttributeValue(
            XmlNode node,
            string attributeName)
        {
            XmlAttributeCollection attributes = node.Attributes;
            XmlAttribute attribute = attributes[attributeName];

            if (null == attribute)
            {
                return null;
            }

            return attribute.Value;
        }

        /// <summary>
        /// Checks if GetValue may be called on the given PropertyDescriptor.\ 
        /// </summary>
        /// <param name="property">property information</param>
        /// <returns>true if getvalue can be called</returns>
        private static bool IsReadablePropertyDescriptor(PropertyDescriptor property)
        {
            return !(property.ComponentType is System.Reflection.MemberInfo)
                   || !IsGenericTypeMember(property.ComponentType, property.Name);
        }

        /// <summary>
        /// Checks if the given type member is a generic-only member on a non-generic type.
        /// </summary>
        /// <param name="type">type of member</param>
        /// <param name="memberName">member's name to check</param>
        /// <returns>true if type's member is generic</returns>
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

        /// <summary>
        /// Compare two clr properties
        /// </summary>
        /// <param name="owner1">owner of property1</param>
        /// <param name="property1">property1 information</param>
        /// <param name="owner2">owner of property2</param>
        /// <param name="property2">property2 information</param>
        /// <returns>true if properties match</returns>
        private static bool CompareClrProperty(
            object owner1,
            PropertyDescriptor property1,
            object owner2,
            PropertyDescriptor property2)
        {
            //// both are simple property, convert them into string and compare
            object obj1;
            object obj2;

            //// Show property to be compared.
            //// SendCompareMessage("Compare Clr property '" + property1.Name + " owner: " + owner1.GetType().Name);
            try
            {
                if (IsReadablePropertyDescriptor(property1))
                {
                    obj1 = property1.GetValue(owner1);
                    obj2 = property2.GetValue(owner2);

                    bool same = CompareObjects(obj1, obj2);

                    if (!same)
                    {
                        SendCompareMessage("Clr property '" + property1.Name + "' is different.");
                        Break();
                    }

                    return same;
                }
                else
                {
                    return true;
                }
            }
            catch (System.Reflection.TargetInvocationException e)
            {
                if (!(e.InnerException is NotSupportedException))
                {
                    throw;
                }
            }

            return true;
        }

        /// <summary>
        /// Compare the value for of a property of LogicalTreeNode.
        /// If the value are not of the same type, return false.
        /// if the value can be convert to string and the result is not
        /// the same just return false.
        /// For logical tree nodes, call CompareLogicalTree to compare recursively.
        /// Otherwise, use CompareAsGenericObject
        /// to compare
        /// </summary>
        /// <param name="obj1">The first value</param>
        /// <param name="obj2">The second value</param>
        /// <returns>
        /// true, if value is regarded as the same
        /// false, otherwise use CompareAsGenericObject to compare
        /// </returns>
        private static bool CompareObjects(object obj1, object obj2)
        {
            bool same = false;

            if (null == obj1 && null == obj2)
            {
                return true;
            }

            if (null == obj1)
            {
                SendCompareMessage("Values is different: 'null' vs. '" + obj2.ToString() + "'.");
                Break();
                return false;
            }

            if (null == obj2)
            {
                SendCompareMessage("Values are different: '" + obj1.ToString() + "' vs. 'null'.");
                Break();
                return false;
            }

            //// Compare Type
            Type type1 = obj1.GetType();
            Type type2 = obj2.GetType();

            if (!type1.Equals(type2))
            {
                SendCompareMessage("Type of value is different: '" + type1.FullName + "' vs. '" + type2.FullName + "'.");
                Break();
                return false;
            }

            if (type1.IsPrimitive)
            {
                same = ComparePrimitive(obj1, obj2);
                return same;
            }

            if (s_objectsInTree[0].Contains(obj1.GetHashCode()) || s_objectsInTree[1].Contains(obj2.GetHashCode()))
            {
                return true;
            }

            s_objectsInTree[0].Add(obj1.GetHashCode());
            s_objectsInTree[1].Add(obj2.GetHashCode());

            return CompareGenericObject(obj1, obj2);
        }

        /// <summary>
        /// Compare two value types
        /// </summary>
        /// <param name="obj1">The first value</param>
        /// <param name="obj2">The second value</param>
        /// <returns>
        /// true, if they are the same
        /// false, otherwise
        /// </returns>
        private static bool ComparePrimitive(object obj1, object obj2)
        {
            bool same = false;
            double errorAllowed = 0.000001;

            //// for double or float comparison, certain error should be allowed. 
            if (obj1 is double)
            {
                double double1 = (double)obj1;
                double double2 = (double)obj2;
                if ((obj1.Equals(double.NaN) && obj2.Equals(double.NaN))
                    || (double.IsInfinity(double1) && double.IsInfinity(double2)))
                {
                    return true;
                }

                same = Math.Abs(double2) > errorAllowed ?
                    (double1 / double2) > (1 - errorAllowed) && (double1 / double2) < (1 + errorAllowed) :
                     Math.Abs(double1 - double2) < errorAllowed;
            }
            else if (obj1 is float)
            {
                float float1 = (float)obj1;
                float float2 = (float)obj2;
                if ((obj1.Equals(float.NaN) && obj2.Equals(float.NaN))
                    || (float.IsInfinity(float1) && float.IsInfinity(float2)))
                {
                    return true;
                }

                same = Math.Abs(float2) > errorAllowed ?
                    (float1 / float2) > (1 - errorAllowed) && (float1 / float2) < (1 + errorAllowed) :
                    Math.Abs(float1 - float2) < errorAllowed;
            }
            else
            {
                same = obj1.Equals(obj2);
            }

            if (!same)
            {
                SendCompareMessage("Values are different: '" + obj1.ToString() + "' vs. '" + obj2.ToString() + "'.");
                Break();
            }

            return same;
        }

        /// <summary>
        /// For a generic object value, just compare their properties.
        /// </summary>
        /// <param name="obj1">object1 to compare</param>
        /// <param name="obj2">object2 to compare</param>
        /// <returns>true if equal</returns>
        private static bool CompareGenericObject(object obj1, object obj2)
        {
            //// Compare properties
            if (!CompareObjectProperties(obj1, obj2))
            {
                SendCompareMessage("Not all the properties are the same for object '" + obj1.GetType().ToString() + "'.");
                Break();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Break on error
        /// </summary>
        private static void Break()
        {
            if (BreakOnError)
            {
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// Compare collections of properties
        /// </summary>
        /// <param name="properties1">The first property that is collection</param>
        /// <param name="properties2">The second property that is collection</param>
        /// <returns>
        /// true, if they are the same
        /// false, otherwise
        /// </returns>
        private static bool ComparePropertyAsIEnumerable(
            object properties1,
            object properties2)
        {
            IEnumerable firstEnumerable = properties1 as IEnumerable;
            IEnumerable secondEnumerable = properties2 as IEnumerable;

            if (firstEnumerable == null && secondEnumerable == null)
            {
                return true;
            }

            if (firstEnumerable == null)
            {
                SendCompareMessage("properties1 is not IEnumerable");
                Break();
                return false;
            }

            if (secondEnumerable == null)
            {
                SendCompareMessage("properties2 is not IEnumerable");
                Break();
                return false;
            }

            IEnumerator firstEnumerator = firstEnumerable.GetEnumerator();
            IEnumerator secondEnumerator = secondEnumerable.GetEnumerator();
            uint firstNodeCount = 0;
            uint secondNodeCount = 0;

            while (firstEnumerator.MoveNext())
            {
                firstNodeCount++;
                if (!secondEnumerator.MoveNext())
                {
                    break;
                }

                secondNodeCount++;

                if (!CompareGenericObject(firstEnumerator.Current, secondEnumerator.Current))
                {
                    SendCompareMessage("The first node and the second node have different values in collection");
                    Break();
                    return false;
                }
            }

            if (secondEnumerator.MoveNext())
            {
                secondNodeCount++;
            }

            if (firstNodeCount != secondNodeCount)
            {
                SendCompareMessage("The first node and the second node have different lengths");
                Break();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Log message to output
        /// </summary>
        /// <param name="message">message to send</param>
        private static void SendCompareMessage(string message)
        {
            // Log to console temporarily
            Tracer.LogTrace(message);
        }

        /// <summary>
        /// Compare two object trees. If all the descendant logical nodes 
        /// are equivalent, return true, otherwise, return false.
        /// </summary>
        /// <param name="firstTree">The root for the first tree.</param>
        /// <param name="secondTree">The root for the second tree.</param>
        /// <param name="propertiesToIgnore">Custom list of properties to ignore.</param>
        /// <remarks>
        /// Compares every event and property for each the node.
        /// </remarks>
        /// <returns>
        /// A structure containing result. If the returned variable name is result.
        /// result.Result is CompareResult.Equivalent in the case two nodes are equivalent,
        /// and CompareResult.Different otherwise.
        /// </returns>
        private static CompareResult CompareLogical(
            object firstTree,
            object secondTree,
            Dictionary<string, PropertyToIgnore> propertiesToIgnore)
        {
            if (propertiesToIgnore == null)
            {
                throw new ArgumentNullException("propertiesToIgnore", "Argument must be a non-null Dictionary.");
            }

            CompareResult result;

            result = CompareResult.Equivalent;

            // Validate parameters, both objects are null
            if (null == firstTree && null == secondTree)
            {
                return result;
            }

            result = CompareResult.Different;

            // Validate parameters, only one object is null
            if (null == firstTree || null == secondTree)
            {
                return result;
            }

            // Compare the types 
            if (!firstTree.GetType().Equals(secondTree.GetType()))
            {
                SendCompareMessage("Two nodes have different types: '" + firstTree.GetType().FullName + "' vs. '" + secondTree.GetType().FullName + "'.");
                Break();
                return result;
            }

            bool same = false;
            lock (s_objectsInTree)
            {
                // Create hashtables that will contain objects in the trees.
                // This is used to break loops.
                s_objectsInTree[0] = new List<int>();
                s_objectsInTree[1] = new List<int>();

                s_skipProperties = propertiesToIgnore;

                // Include default skip properties if necessary.
                if (s_skipProperties != s_skipPropertiesDefault)
                {
                    _MergeDictionaries(s_skipProperties, s_skipPropertiesDefault);
                }

                try
                {
                    same = CompareObjects(firstTree, secondTree);
                }
                finally
                {
                    s_objectsInTree[0] = null;
                    s_objectsInTree[1] = null;
                    s_skipProperties = null;
                }
            }

            // Two trees are equivalent
            if (same)
            {
                result = CompareResult.Equivalent;
            }

            return result;
        }

        /// <summary>
        /// Recursively compare two object trees following logical tree structure
        /// </summary>
        /// <param name="firstTree">Root for first tree</param>
        /// <param name="secondTree">Root for second tree</param>
        /// <returns>
        ///   True, if two object tree are equivalent
        ///   False, otherwise
        /// </returns>
        private static bool CompareLogicalTree(
            object firstTree,
            object secondTree)
        {
            return CompareObjects(firstTree, secondTree);
        }

        /// <summary>
        /// Merge dictionaries
        /// </summary>
        /// <param name="dictionary1">dictionary1 to merge</param>
        /// <param name="dictionary2">dictionary2 to merge</param>
        /// <returns>merged count</returns>
        private static int _MergeDictionaries(Dictionary<string, PropertyToIgnore> dictionary1, Dictionary<string, PropertyToIgnore> dictionary2)
        {
            int cnt = 0;

            foreach (string propName in dictionary2.Keys)
            {
                if (!dictionary1.ContainsKey(propName))
                {
                    dictionary1.Add(propName, dictionary2[propName]);
                    cnt++;
                }
            }

            return cnt;
        }

        /// <summary>
        /// Compare Properties for two nodes. If all the properties for these two
        /// nodes have the same value, return true. Otherwise, return false.
        /// </summary>
        /// <param name="firstNode">The first node</param>
        /// <param name="secondNode">The second node</param>
        /// <returns>true if matched</returns>
        private static bool CompareObjectProperties(object firstNode, object secondNode)
        {
            //// Compare CLR properties.
            Dictionary<string, PropertyDescriptor> clrProperties1 = GetClrProperties(firstNode);
            Dictionary<string, PropertyDescriptor> clrProperties2 = GetClrProperties(secondNode);

            if (!CompareClrPropertyCollection(firstNode, clrProperties1, secondNode, clrProperties2))
            {
                SendCompareMessage("The first node and the second node are different in one or more CLR properties.");
                Break();
                return false;
            }

            if (!ComparePropertyAsIEnumerable(firstNode, secondNode))
            {
                SendCompareMessage("The first node and the second node are different collections.");
                Break();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Compare two clr properties
        /// </summary>
        /// <param name="firstNode">first object to compare</param>
        /// <param name="properties1">property1 information</param>
        /// <param name="secondNode">second object to compare</param>
        /// <param name="properties2">property2 information</param>
        /// <returns>true if matched</returns>
        private static bool CompareClrPropertyCollection(
            object firstNode,
            Dictionary<string, PropertyDescriptor> properties1,
            object secondNode,
            Dictionary<string, PropertyDescriptor> properties2)
        {
            IEnumerator<string> ie1 = properties1.Keys.GetEnumerator();

            while (ie1.MoveNext())
            {
                string propertyName = ie1.Current;

                //// Check that the second tree contains the property.
                if (!properties2.ContainsKey(propertyName))
                {
                    SendCompareMessage("Property '" + propertyName + "' is not in second tree.");
                    Break();
                    return false;
                }

                //// If property was in skip collection, ignore it
                if (!ShouldIgnoreProperty(propertyName, firstNode, IgnoreProperty.IgnoreValueOnly))
                {
                    //// Compare properties
                    if (!CompareClrProperty(
                             firstNode,
                             properties1[propertyName],
                             secondNode,
                             properties2[propertyName]))
                    {
                        SendCompareMessage("Value of property '" + propertyName + "' is different.");
                        Break();
                        return false;
                    }
                }

                properties2.Remove(propertyName);
            }

            //// Check that the second tree doesn't have more properties than the first tree.
            if (properties2.Count > 0)
            {
                IEnumerator<string> ie2 = properties2.Keys.GetEnumerator();
                ie2.MoveNext();

                SendCompareMessage("Property '" + properties2[ie2.Current].Name + "' is not in first tree.");
                Break();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get the clr properties of the object
        /// </summary>
        /// <param name="owner">object under question</param>
        /// <returns>set of all properties</returns>
        private static Dictionary<string, PropertyDescriptor> GetClrProperties(object owner)
        {
            Dictionary<string, PropertyDescriptor> clrProperties = new Dictionary<string, PropertyDescriptor>();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(owner);

            foreach (PropertyDescriptor property in properties)
            {
                //// skip properties
                if (ShouldIgnoreProperty(property.Name, owner, IgnoreProperty.IgnoreNameAndValue))
                {
                    continue;
                }

                clrProperties.Add(property.Name, property);
            }

            return clrProperties;
        }

        /// <summary>
        ///  Shoud ignore this property?
        /// </summary>
        /// <param name="propertyName">property name</param>
        /// <param name="owner">owner of the property</param>
        /// <param name="whatToIgnore">Value only or Value and name to ignore?</param>
        /// <returns>true if should be ignored</returns>
        private static bool ShouldIgnoreProperty(string propertyName, object owner, IgnoreProperty whatToIgnore)
        {
            PropertyToIgnore property = null;
            foreach (string key in s_skipProperties.Keys)
            {
                if (String.Equals(key, propertyName, StringComparison.Ordinal)
                    || key.StartsWith(propertyName + "___owner___", StringComparison.Ordinal))
                {
                    property = s_skipProperties[key];
                    if (whatToIgnore == property.WhatToIgnore && ((null == property.Owner) || _DoesTypeMatch(owner.GetType(), property.Owner)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Do types match
        /// </summary>
        /// <param name="ownerType">owner's type</param>
        /// <param name="typeName">name of the type</param>
        /// <returns>true if types match</returns>
        private static bool _DoesTypeMatch(Type ownerType, string typeName)
        {
            Type type = ownerType;
            bool isMatch = false;

            while (type != null && !isMatch)
            {
                if (0 == String.Compare(type.Name, typeName, StringComparison.Ordinal))
                {
                    isMatch = true;
                }

                type = type.BaseType;
            }

            return isMatch;
        }

        /// <summary>
        /// Read properties to skip from PropertiesToSkip.xml. If this file
        /// exists under the current working directory, use the one there. 
        /// Otherwise, use the file built in the ClientTestLibrary Assembly.
        /// </summary>
        /// <returns>Hashtable containing properties should be skiped</returns>
        private static Dictionary<string, PropertyToIgnore> ReadSkipProperties()
        {
            //// File name for the properties to skip.
            return ReadSkipProperties("PropertiesToSkip.xml");
        }
    }
}
