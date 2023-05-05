// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Common.TestObjects.Xaml.GraphOperations.Comparers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Security;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Common.TestObjects.Xaml.GraphCore;
    using Microsoft.Test.Xaml.Common.TestObjects.Xaml.GraphOperations.Builders;
    using Microsoft.Test.Xaml.Common.TestObjects.XamlObjectComparer;
    using Microsoft.Test.Xaml.Utilities;

    /// <summary>
    /// Object graph comparer
    /// </summary>
    public static class ObjectGraphComparer
    {
        /// <summary>
        /// Compare mode property
        /// </summary>
        public static readonly XName CompareModeProperty = XName.Get("CompareMode", string.Empty);

        /// <summary>
        /// Compare error property 
        /// </summary>
        public static readonly XName CompareErrorProperty = XName.Get("CompareError", string.Empty);

        /// <summary>
        /// Helper for testing parity with the TreeComparer
        /// </summary>
        /// <param name="root1">first object to compare</param>
        /// <param name="root2">second object to compare</param>
        /// <param name="compareResult">results of v1 compare</param>
        public static void XamlCompareParity(object root1, object root2, CompareResult compareResult)
        {
            GraphCompareResults compareResults = ObjectGraphComparer.XamlCompare(root1, root2);

            bool traceGraphs = false;

            //// If results dont match between the two comparers //
            if ((compareResults.Passed == true && compareResult == CompareResult.Different) ||
                (compareResults.Passed == false && compareResult == CompareResult.Equivalent))
            {
                Tracer.LogTrace("Results do not match between XamlTreeComparer and ObjectGraphComparer");
                traceGraphs = true;
            }

            //// If it was a failure or the compares dont match //
            if (compareResult == CompareResult.Different || traceGraphs == true)
            {
                string tmpName = DirectoryAssistance.GetArtifactDirectory(Guid.NewGuid().ToString());
                Tracer.LogTrace("Logging object graphs - " + tmpName + "*.xml");

                //// Disable this due to 603649
                //// It's confusing to see the security exception and immediate crash
                ////try
                ////{
                ////    ObjectGraph.Serialize(ObjectGraphWalker.Create(root1), tmpName + "root1.xml");
                ////    ObjectGraph.Serialize(ObjectGraphWalker.Create(root2), tmpName + "root2.xml");
                ////    ObjectGraph.Serialize(compareResults.ResultGraph, tmpName + "result.xml");
                ////}
                ////catch (SecurityException se)
                ////{
                ////    // Ignore, trace and continue if running under partial trust.. //
                ////    Tracer.LogTrace("Security exception when serializing trace information " + se.ToString());
                ////}

                Tracer.LogTrace("Exceptions are as follows:");

                foreach (CompareError error in compareResults.Errors)
                {
                    Tracer.LogTrace(String.Format(CultureInfo.InvariantCulture, "Node: {0}  Message= {1}", error.Node1.QualifiedName, error.Error.Message));
                }
            }
        }

        /// <summary>
        /// Do the compare
        /// </summary>
        /// <param name="root1">first object to compare</param>
        /// <param name="root2">second object to compare</param>
        /// <returns>the compare results</returns>
        public static GraphCompareResults XamlCompare(object root1, object root2)
        {
            ObjectGraph graph1 = ObjectGraphWalker.Create(root1);
            ObjectGraph graph2 = ObjectGraphWalker.Create(root2);

            return Compare(graph1, graph2, null);
        }

        /// <summary>
        /// Compare two objects
        /// </summary>
        /// <param name="root1">frist object to compare</param>
        /// <param name="root2">second object to compare</param>
        /// <param name="ignores">list of properties to ignore</param>
        /// <returns>the results of the compare</returns>
        public static GraphCompareResults Compare(ObjectGraph root1, ObjectGraph root2, List<object> ignores)
        {
            //// Do the comparison here //
            List<IGraphNode> nodes1 = root1.Descendants;
            List<IGraphNode> nodes2 = root2.Descendants;

            GraphCompareResults compareResults = new GraphCompareResults();

            ObjectGraph compareResultGraph = root1.Clone();
            List<IGraphNode> resultTreeNodes = compareResultGraph.Descendants;

            if (nodes1.Count != nodes2.Count)
            {
                CompareError error = new CompareError((ObjectGraph)nodes1[0], (ObjectGraph)nodes2[0], new Exception("Number of nodes do not match"));
                resultTreeNodes[0].SetValue(ObjectGraphComparer.CompareErrorProperty, error);
                compareResults.Errors.Add(error);
            }

            for (int i = 0; i < nodes1.Count; i++)
            {
                ObjectGraph node1 = (ObjectGraph)nodes1[i];

                var nodelist = from node in nodes2
                               where node1.QualifiedName.Equals(node.QualifiedName)
                               select node;

                List<IGraphNode> matchingNodes = nodelist.ToList<IGraphNode>();
                if (matchingNodes.Count == 0)
                {
                    CompareError error = new CompareError(node1, null, new Exception("Node not present in second tree"));
                    compareResults.Errors.Add(error);
                    resultTreeNodes[i].SetValue(ObjectGraphComparer.CompareErrorProperty, error);
                    continue;
                }

                if (matchingNodes.Count > 1)
                {
                    CompareError error = new CompareError(node1, null, new Exception("more than one match for this node in second tree"));
                    compareResults.Errors.Add(error);
                    resultTreeNodes[i].SetValue(ObjectGraphComparer.CompareErrorProperty, error);
                    continue;
                }

                ObjectGraph node2 = (ObjectGraph)matchingNodes[0];

                CompareError error1 = CompareNodes(node1, node2);
                if (error1 != null)
                {
                    compareResults.Errors.Add(error1);
                    resultTreeNodes[i].SetValue(ObjectGraphComparer.CompareErrorProperty, error1);
                }
            }

            compareResults.Passed = compareResults.Errors.Count == 0 ? true : false;
            compareResults.ResultGraph = compareResultGraph;
            return compareResults;
        }

        /// <summary>
        /// Compare two nodes
        /// </summary>
        /// <param name="node1">first node to compare</param>
        /// <param name="node2">second node to compare</param>
        /// <returns>The resulting errors on compare if any</returns>
        private static CompareError CompareNodes(ObjectGraph node1, ObjectGraph node2)
        {
            //// Compare two nodes - just the nodes //
            //// - compare the property name
            //// - compare the property value
            object compareMode = node1.GetValue(CompareModeProperty);
            if (compareMode == null)
            {
                compareMode = CompareMode.PropertyNameAndValue;
            }

            CompareMode mode = (CompareMode)compareMode;

            //// default is compare name and value //

            if (mode == CompareMode.PropertyNameAndValue || mode == CompareMode.PropertyName)
            {
                //// comapre name //
                if (!node1.Name.Equals(node2.Name))
                {
                    CompareError error = new CompareError(node1, node2, new Exception("Node names do not match"));
                    return error;
                }
            }

            if (mode == CompareMode.PropertyNameAndValue || mode == CompareMode.PropertyValue)
            {
                //// compare values - only compare if they are primitive types, if it is a complex
                //// type, its properties will be child nodes in the metadata graph 
                if (node1.DataType.IsPrimitive)
                {
                    ValueType value1 = node1.Data as ValueType;
                    ValueType value2 = node2.Data as ValueType;

                    //// Need special handling for double and float as well as strings //
                    if (!ComparePrimitive(value1, value2))
                    {
                        CompareError error = new CompareError(node1, node2, new Exception("Node values do not match"));
                        return error;
                    }

                    return null;
                }

                //// string is not a primitive type // 
                if (node1.DataType == typeof(string))
                {
                    //// for string case //
                    if (node1.Data == null && node2.Data == null)
                    {
                        return null;
                    }

                    if (node1.Data == null || node2.Data == null)
                    {
                        CompareError error = new CompareError(node1, node2, new Exception("Node values do not match"));
                        return error;
                    }

                    if (!node1.Data.Equals(node2.Data))
                    {
                        CompareError error = new CompareError(node1, node2, new Exception("Node values do not match"));
                        return error;
                    }
                }
            }

            return null;
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

            if (obj1 == null && obj2 == null)
                return true;

            if (obj1 == null || obj2 == null)
                return false;

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

            return same;
        }
    }
}
