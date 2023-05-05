// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Test.Xaml.Common.TestObjects.Xaml.GraphCore;
using Microsoft.Test.Xaml.Common.TestObjects.Xaml.GraphOperations.Comparers;

namespace Microsoft.Test.Xaml.Common.TestObjects.Xaml.GraphOperations.Viewers
{
    /// <summary>
    /// This is a helper class for debugging - Dotty needs to be installed on the 
    /// machine for this to work. To generate a jpg file from a dotty file do the following
    /// neato -Kdot -Tjpg -Gsep=.1 -Gmaxiter=1 -Goverlap=false [dot file]  -O [jpg file]
    /// </summary>
    public class Dotty
    {
        /// <summary>
        /// List of visited nodes
        /// </summary>
        private List<GraphNode> _visited = new List<GraphNode>();

        /// <summary>
        /// Save a given object graph
        /// </summary>
        /// <param name="root">root of the object graph</param>
        /// <param name="path">file path to save to</param>
        public static void Save(ObjectGraph root, string path)
        {
            new Dotty().SaveDottyGraph(root, path);
        }

        /// <summary>
        /// Write the given node
        /// </summary>
        /// <param name="treeNode">node to write</param>
        /// <param name="writer">writer to use</param>
        private void WriteNode(IGraphNode treeNode, StreamWriter writer)
        {
            ObjectGraph node = treeNode as ObjectGraph;

            CompareError error = (CompareError)node.GetValue(ObjectGraphComparer.CompareErrorProperty);
            string color = "lightblue";
            string errorValue2 = String.Empty;
            if (error != null)
            {
                color = "orange";
                errorValue2 = " \\n Value2=" + error.Node2.Data;
            }

            if (node.Children.Count == 0)
            {
                writer.WriteLine(String.Format(
                    CultureInfo.InvariantCulture, 
                    "{0}[shape=box, color={1}, style=filled, label=\"Name={2} \\n Type={3} \\n Value={4}  {5}\"];",
                    node.QualifiedName, 
                    color, 
                    node.QualifiedName, 
                    node.DataType, 
                    node.Data,
                    String.Empty));
            }
            else
            {
                writer.WriteLine(String.Format(
                                                CultureInfo.InvariantCulture, 
                                                "{0}[color=grey, color={1}, style=filled, label=\"Name={2} \\n Type={3} \\n Value={4} {5} \"];",
                                                node.QualifiedName, 
                                                color, 
                                                node.QualifiedName, 
                                                node.DataType, 
                                                node.Data,
                                               String.Empty));
            }
        }

        /// <summary>
        /// Write a link
        /// </summary>
        /// <param name="node1">link from node</param>
        /// <param name="node2">link to node</param>
        /// <param name="writer">writer to use</param>
        private void WriteLink(GraphNode node1, GraphNode node2, StreamWriter writer)
        {
            writer.WriteLine(String.Format(CultureInfo.InvariantCulture, "{0}->{1}", node1.QualifiedName, node2.QualifiedName));
        }

        /// <summary>
        /// Write a tree out 
        /// </summary>
        /// <param name="root">root of the tree</param>
        /// <param name="writer">writer to use</param>
        private void WriteTree(GraphNode root, StreamWriter writer)
        {
            writer.WriteLine("digraph graph1");
            writer.WriteLine("{");

            List<IGraphNode> descendants = root.Descendants;

            // Write out the nodes //
            foreach (IGraphNode node in descendants)
            {
                WriteNode(node, writer);
            }

            // write out the links //
            foreach (GraphNode node in descendants)
            {
                foreach (GraphNode child in node.Children)
                {
                    WriteLink(node, child, writer);
                }
            }

            writer.WriteLine("}");
        }

        /// <summary>
        /// Save as dotty graph
        /// </summary>
        /// <param name="root">root of object graph</param>
        /// <param name="path">file path to save to</param>
        private void SaveDottyGraph(ObjectGraph root, string path)
        {
            StreamWriter writer = new StreamWriter(path);
            WriteTree(root, writer);
            writer.Flush();
            writer.Close();
        }
    }
}
